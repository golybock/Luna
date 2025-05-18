using Luna.Auth.Models.Database.Models;
using Luna.Auth.Models.View.Models;
using Luna.Auth.Repositories.Repositories.SessionArchiveRepository;
using Luna.Auth.Repositories.Repositories.SessionRepository;
using Luna.Auth.Services.Extensions;
using Microsoft.Extensions.Logging;

namespace Luna.Auth.Services.Services.SessionManagementService;

public class SessionManagementService : ISessionManagementService
{
	private readonly ISessionArchiveRepository _sessionArchiveRepository;
	private readonly ISessionRepository _sessionRepository;
	private readonly ILogger<SessionManagementService> _logger;

	public SessionManagementService(
		ISessionArchiveRepository sessionArchiveRepository,
		ISessionRepository sessionRepository,
		ILogger<SessionManagementService> logger
	)
	{
		_sessionArchiveRepository = sessionArchiveRepository;
		_sessionRepository = sessionRepository;
		_logger = logger;
	}

	public async Task<IEnumerable<SessionView>> GetUserSessionsAsync(Guid userId)
	{
		IEnumerable<SessionDatabase> sessions = await _sessionRepository.GetUserSessionsAsync(userId);

		IEnumerable<SessionView> sessionViews = sessions
			.Select(session => session.ToView());

		return sessionViews;
	}

	public async Task CloseSessionAsync(Guid userId, Guid sessionId)
	{
		SessionDatabase? session = await _sessionRepository.GetSessionAsync(userId, sessionId);

		if (session == null)
		{
			throw new UnauthorizedAccessException($"Сессия {sessionId} для пользователя {userId} не найдена");
		}

		bool archived = await _sessionArchiveRepository.CreateSessionAsync(session);

		if (!archived)
		{
			throw new UnauthorizedAccessException("Failed to archive session");
		}

		int closedSessions = await _sessionRepository.CloseSessionAsync(userId, sessionId);

		if (closedSessions == 0)
		{
			await _sessionArchiveRepository.DeleteSessionAsync(session.Id);

			throw new Exception("Failed to close session in Redis");
		}
	}

	public async Task CloseAllUserSessionsAsync(Guid userId, Guid? exceptSessionId = null)
	{
		IEnumerable<SessionDatabase> sessionsDatabase = await _sessionRepository.GetUserSessionsAsync(userId);
		List<SessionDatabase> sessions = sessionsDatabase.ToList();

		if (!sessions.Any())
		{
			throw new UnauthorizedAccessException("Sessions not found");
		}

		if (exceptSessionId != null)
		{
			SessionDatabase? exceptSession = sessions.FirstOrDefault(session => session.Id == exceptSessionId);
			if (exceptSession != null) sessions.Remove(exceptSession);
		}

		IEnumerable<Guid> sessionIds = sessions.Select(session => session.Id).ToList();

		int archived = await _sessionArchiveRepository.CreateSessionsAsync(sessions);

		if (archived == 0)
		{
			throw new UnauthorizedAccessException("Failed to archive sessions");
		}

		int closedSessions = await _sessionRepository.CloseSessionsAsync(sessionIds);

		if (closedSessions != archived)
		{
			// Компенсирующая транзакция: откатываем архивирование
			await _sessionArchiveRepository.DeleteSessionsAsync(sessionIds);

			throw new Exception($"Failed to close sessions in Redis: archived {archived}, closed {closedSessions}");
		}
	}
}