using FluentAssertions;
using Luna.Auth.Models.Database.Models;
using Luna.Auth.Repositories.Repositories.SessionRepository;

namespace Luna.Auth.IntegrationTests;

public class SessionRepositoryTests : IClassFixture<RedisFixture>
{
	private readonly ISessionRepository _sessionRepository;

	public SessionRepositoryTests(RedisFixture fixture)
	{
		_sessionRepository = new SessionRepository(fixture.ConnectionString);
	}

	[Fact]
	public async Task SetAndGetSession_ShouldWork()
	{
		Guid userId = Guid.NewGuid();
		Guid sessionId = Guid.NewGuid();
		SessionDatabase session = new SessionDatabase
		{
			Id = sessionId,
			UserId = userId,
			Token = "test_token",
			RefreshToken = "test_refresh_token",
			ExpiresAt = DateTime.UtcNow.AddDays(1)
		};

		bool set = await _sessionRepository.SetSessionAsync(userId, sessionId, session, TimeSpan.FromDays(1));
		SessionDatabase? retrieved = await _sessionRepository.GetSessionAsync(userId, sessionId);

		set.Should().BeTrue();
		retrieved.Should().NotBeNull();
		retrieved.Token.Should().Be(session.Token);
		retrieved.RefreshToken.Should().Be(session.RefreshToken);
	}
}

public class RedisFixture : IDisposable
{
	public string ConnectionString { get; }

	public RedisFixture()
	{
		ConnectionString = "localhost:6379";
	}

	public void Dispose()
	{
		// Очистка Redis
	}
}