using Luna.Auth.Models.Database.Models;

namespace Luna.Auth.Repositories.Repositories.SessionRepository;

/// <summary>
/// Репозиторий для работы с сессиями в кэше
/// </summary>
public interface ISessionRepository
{
	/// <summary>
	/// Получает значение.
	/// Возвращается одно значение (точное соответствие),
	/// или пустое значение при отсутствии таковых
	/// </summary>
	/// <param name="userId">Идентификатор пользователя</param>
	/// <param name="sessionId">Идентификатор сессии</param>
	/// <returns>Сессия или null</returns>
	public Task<SessionDatabase?> GetSessionAsync(Guid userId, Guid sessionId);

	/// <summary>
	/// Получает сессии пользователя
	/// </summary>
	/// <param name="userId">Идентификатор пользователя</param>
	/// <returns>Коллекция найденных записей</returns>
	public Task<IEnumerable<SessionDatabase>> GetUserSessionsAsync(Guid userId);

	/// <summary>
	/// Устанавливает значение по составному ключу, состоящему из userId и sessionId.
	/// </summary>
	/// <param name="userId">Идентификатор пользователя</param>
	/// <param name="sessionId">Идентификатор сессии</param>
	/// <param name="session">Данные сессии</param>
	/// <param name="ttl">Время жизни значения</param>
	/// <returns></returns>
	public Task<Boolean> SetSessionAsync(Guid userId, Guid sessionId, SessionDatabase session, TimeSpan ttl);

	/// <summary>
	/// Возвращает есть ли значение по ключу (сессия)
	/// </summary>
	/// <param name="userId">Идентификатор пользователя</param>
	/// <param name="sessionId">Идентификатор сессии</param>
	/// <returns></returns>
	Task<Boolean> SessionExistsAsync(Guid? userId = null, Guid? sessionId = null);

	/// <summary>
	/// Возвращает есть ли значение по ключу (сессия) синхронно
	/// </summary>
	/// <param name="userId">Идентификатор пользователя</param>
	/// <param name="sessionId">Идентификатор сессии</param>
	/// <returns></returns>
	Boolean SessionExists(Guid? userId = null, Guid? sessionId = null);

	/// <summary>
	/// Обновляет значение по составному ключу или группе ключей.
	/// Если заданы оба параметра – обновляется значение по точному ключу,
	/// если задан только один – обновляются все записи, удовлетворяющие паттерну.
	/// </summary>
	/// <param name="session">Новое значение</param>
	/// <param name="userId">Идентификатор пользователя (опционально)</param>
	/// <param name="sessionId">Идентификатор сессии (опционально)</param>
	/// <param name="ttl">Время жизни нового значения</param>
	/// <returns>Количество обновлённых записей</returns>
	Task<Boolean> UpdateSessionAsync(Guid userId, Guid sessionId, SessionDatabase session, TimeSpan ttl);

	/// <summary>
	/// Удаляет запись или записи.
	/// Если заданы оба параметра – удаляется запись по точному ключу,
	/// если задан только один – удаляются все записи, удовлетворяющие паттерну.
	/// </summary>
	/// <param name="userId">Идентификатор пользователя (опционально)</param>
	/// <param name="sessionId">Идентификатор сессии (опционально)</param>
	/// <returns>Количество удалённых записей</returns>
	public Task<Int32> CloseSessionAsync(Guid? userId = null, Guid? sessionId = null);

	/// <summary>
	/// Удаляет записи.
	/// </summary>
	/// <param name="sessionIds">Идентификатор сессий</param>
	/// <returns>Количество удалённых записей</returns>
	public Task<Int32> CloseSessionsAsync(IEnumerable<Guid> sessionIds);
}