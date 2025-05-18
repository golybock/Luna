using Luna.Auth.Models.Database.Models;
using Npgsql;

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
	/// Добавляет новую завершённую сессию в архив.
	/// </summary>
	Task<bool> CreateSessionAsync(SessionDatabase session, NpgsqlTransaction transaction);

	/// <summary>
	/// Добавляет новые завершённые сессии в архив.
	/// </summary>
	Task<int> CreateSessionsAsync(IEnumerable<SessionDatabase> sessions);

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

	/// <summary>
	/// Удаляет сессии из архива по их идентификатору.
	/// </summary>
	Task<bool> DeleteSessionsAsync(IEnumerable<Guid> ids);
}