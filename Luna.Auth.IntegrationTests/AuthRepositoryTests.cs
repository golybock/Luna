using FluentAssertions;
using Luna.Auth.Models.Database.Models;
using Luna.Auth.Repositories.Repositories.AuthRepository;
using Luna.Tools.Crypto;
using Luna.Tools.Database.Npgsql.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Luna.Auth.IntegrationTests;

public class AuthRepositoryTests : IClassFixture<DatabaseFixture>
{
	private readonly IAuthRepository _authRepository;

	public AuthRepositoryTests(DatabaseFixture fixture)
	{
		Logger<AuthRepository> logger = new Logger<AuthRepository>(new NullLoggerFactory());

		_authRepository = new AuthRepository(fixture.DatabaseOptions, logger);
	}

	[Fact]
	public async Task CreateAndGetAuthUser_ShouldWork()
	{
		Guid userId = Guid.NewGuid();
		AuthUserDatabase user = new AuthUserDatabase
		{
			Id = userId,
			Email = $"test_{userId}@example.com",
			PasswordHash = Crypto.HashSha512("password")
		};

		bool created = await _authRepository.CreateAuthUserAsync(user);
		AuthUserDatabase? retrieved = await _authRepository.GetAuthUserAsync(userId);

		created.Should().BeTrue();
		retrieved.Should().NotBeNull();
		retrieved.Email.Should().Be(user.Email);
	}
}

// Фикстура для настройки тестовой базы данных
public class DatabaseFixture : IDisposable
{
	public IDatabaseOptions DatabaseOptions { get; }

	public DatabaseFixture()
	{
		// Настройка тестовой БД
		DatabaseOptions = new DatabaseOptions
		{
			ConnectionString = "Host=localhost;Port=5432;Database=luna_auth_test;Username=postgres;Password=postgres"
		};

		// Инициализация тестовой БД (создание схемы)
		SetupDatabase();
	}

	private void SetupDatabase()
	{
		// Код для создания таблиц в тестовой БД
	}

	public void Dispose()
	{
		// Очистка тестовой БД
	}
}