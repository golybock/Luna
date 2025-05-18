using System.Data.Common;
using System.Reflection;

namespace Luna.Tools.Database.Npgsql.Reader;

public class Reader<T> : IReader<T> where T : new()
{
	public static async Task<T?> ReadAsync(DbDataReader reader)
	{
		if (await reader.ReadAsync())
		{
			T? obj = new T();

			foreach (PropertyInfo property in typeof(T).GetProperties())
			{
				object value = reader.GetValue(reader.GetOrdinal(property.Name.ToSnakeCase()));

				if (value != DBNull.Value)
				{
					if (property.CanWrite)
					{
						property.SetValue(obj, value);
					}
				}
			}

			return obj;
		}

		return default;
	}

	public static async Task<IEnumerable<T>> ReadListAsync(DbDataReader reader)
	{
		IList<T> objects = new List<T>();

		while (await reader.ReadAsync())
		{
			T? obj = new T();

			foreach (PropertyInfo property in typeof(T).GetProperties())
			{
				object value = reader.GetValue(reader.GetOrdinal(property.Name.ToSnakeCase()));

				if (value != DBNull.Value)
					property.SetValue(obj, value);
			}

			objects.Add(obj);
		}

		return objects;
	}
}