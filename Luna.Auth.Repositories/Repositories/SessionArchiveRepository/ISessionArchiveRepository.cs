using Luna.Auth.Models.Database.Models;

namespace Luna.Auth.Repositories.Repositories.SessionArchiveRepository;

/// <summary>
/// Репозиторий для работы с хранением архива сессий
/// </summary>
public interface ISessionArchiveRepository
{
	/// <summary>
	/// Добавляет новую завершённую сессию в архив.
	/// </summary>
	Task<bool> CreateSessionAsync(SessionDatabase session);

	/// <summary>
	/// Получает сессию по её идентификатору.
	/// </summary>
	Task<SessionDatabase?> GetSessionAsync(Guid id);

	/// <summary>
	/// Получает все сессии пользователя по его идентификатору.
	/// </summary>
	Task<IEnumerable<SessionDatabase>> GetSessionsByUserIdAsync(Guid userId);

	/// <summary>
	/// Удаляет сессию из архива по её идентификатору.
	/// </summary>
	Task<bool> DeleteSessionAsync(Guid id);
}