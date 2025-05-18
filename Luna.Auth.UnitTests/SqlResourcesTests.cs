using System.Reflection;

namespace Luna.Auth.UnitTests;

public class SqlResourcesTests
{
	private readonly Assembly _repositoryAssembly;

	public SqlResourcesTests()
	{
		// Получаем сборку, содержащую SQL-файлы
		_repositoryAssembly = typeof(Repositories.SqlQueries).Assembly;
	}

	/// <summary>
	/// Более элегантный способ проверки всех SQL-файлов
	/// с использованием рефлексии и констант из класса SqlQueries
	/// </summary>
	[Fact]
	public void AllSqlConstantsInSqlQueries_ShouldPointToExistingFiles()
	{
		string[] resources = _repositoryAssembly.GetManifestResourceNames();

		OutputResourcesList(resources);

		// Получаем все вложенные классы из SqlQueries
		Type[] nestedTypes = typeof(Repositories.SqlQueries).GetNestedTypes();

		foreach (Type nestedType in nestedTypes)
		{
			// Получаем все константы из вложенного класса
			IEnumerable<FieldInfo> constants = nestedType
				.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
				.Where(fi => fi is {IsLiteral: true, IsInitOnly: false});

			foreach (FieldInfo constant in constants)
			{
				// Получаем значение константы (имя SQL-файла)
				string sqlFileName = (string) constant.GetValue(null)!;
				string fullResourceName = $"Luna.Auth.Repositories.SQL.{sqlFileName}";

				AssertResourceExists(resources, fullResourceName);
				AssertResourceReadable(fullResourceName);
			}
		}
	}

	private void OutputResourcesList(string[] resources)
	{
		// Выводим список всех ресурсов для отладки
		System.Diagnostics.Debug.WriteLine("Embedded resources in assembly:");
		foreach (string resource in resources)
		{
			System.Diagnostics.Debug.WriteLine($" - {resource}");
		}
	}

	private void AssertResourceExists(string[] resources, string resourceName)
	{
		Assert.Contains(resourceName, resources, StringComparer.OrdinalIgnoreCase);
	}

	private void AssertResourceReadable(string resourceName)
	{
		using Stream? stream = _repositoryAssembly.GetManifestResourceStream(resourceName);
		Assert.NotNull(stream);

		// Проверяем, что файл не пустой
		using StreamReader reader = new StreamReader(stream);
		string content = reader.ReadToEnd();
		Assert.False(string.IsNullOrWhiteSpace(content), $"SQL-файл {resourceName} не должен быть пустым");
	}

	private string ReadResourceContent(string resourceName)
	{
		using Stream? stream = _repositoryAssembly.GetManifestResourceStream(resourceName);
		Assert.NotNull(stream);

		using StreamReader reader = new StreamReader(stream);
		return reader.ReadToEnd();
	}
}