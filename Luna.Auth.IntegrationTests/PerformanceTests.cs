using System.Diagnostics;
using FluentAssertions;
using Luna.Auth.Models.Database.Models;
using Luna.Auth.Repositories.Repositories.SessionRepository;

namespace Luna.Auth.IntegrationTests;

public class PerformanceTests
{
	[Fact]
	public async Task SessionRepository_GetSessionAsync_Performance()
	{
		SessionRepository repository = new SessionRepository("localhost:6379");
		Guid userId = Guid.NewGuid();
		Guid sessionId = Guid.NewGuid();

		await repository.SetSessionAsync(userId, sessionId, new SessionDatabase(), TimeSpan.MaxValue);

		Stopwatch stopwatch = Stopwatch.StartNew();
		for (int i = 0; i < 10000; i++)
		{
			await repository.GetSessionAsync(userId, sessionId);
		}
		stopwatch.Stop();

		stopwatch.ElapsedMilliseconds.Should().BeLessThan(5000); // 5 секунд на 10000 запросов
	}
}