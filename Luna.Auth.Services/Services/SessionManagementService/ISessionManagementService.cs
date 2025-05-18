using Luna.Auth.Models.View.Models;

namespace Luna.Auth.Services.Services.SessionManagementService;

// Управление сессиями
public interface ISessionManagementService
{
	Task<IEnumerable<SessionView>> GetUserSessionsAsync(Guid userId);
	Task CloseSessionAsync(Guid userId, Guid sessionId);
	Task CloseAllUserSessionsAsync(Guid userId, Guid? exceptSessionId = null);
}