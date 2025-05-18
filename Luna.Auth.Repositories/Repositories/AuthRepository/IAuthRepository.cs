using Luna.Auth.Models.Database.Models;

namespace Luna.Auth.Repositories.Repositories.AuthRepository;

/// <summary>
/// Репозиторий для работы с данными для авторизации пользователей
/// </summary>
public interface IAuthRepository
{
	/// <summary>
	/// Извлечение данных пользователя для авторизации по идентификатору
	/// </summary>
	/// <param name="id">Уникальный идентификатор пользователя</param>
	/// <returns>Объект AuthUserDatabase или null, если пользователь не найден</returns>
	public Task<AuthUserDatabase?> GetAuthUserAsync(Guid id);

	/// <summary>
	/// Извлечение данных пользователя для авторизации по идентификатору
	/// </summary>
	/// <param name="email">Уникальная почта пользователя</param>
	/// <returns>Объект AuthUserDatabase или null, если пользователь не найден</returns>
	public Task<AuthUserDatabase?> GetAuthUserAsync(string email);

	/// <summary>
	/// Извлечение данных пользователя для авторизации по идентификатору
	/// </summary>
	/// <param name="verificationToken">Токен подтверждения почты</param>
	/// <returns>Объект AuthUserDatabase или null, если пользователь не найден</returns>
	public Task<AuthUserDatabase?> GetAuthUserByEmailTokenAsync(string verificationToken);

	/// <summary>
	/// Извлечение данных пользователя для авторизации по идентификатору
	/// </summary>
	/// <param name="resetToken">Токен сброса пароля</param>
	/// <returns>Объект AuthUserDatabase или null, если пользователь не найден</returns>
	public Task<AuthUserDatabase?> GetAuthUserByResetTokenAsync(string resetToken);

	/// <summary>
	/// Создание нового пользователя для авторизации
	/// </summary>
	/// <param name="userAuthDatabase">Данные пользователя для создания записи в базе данных</param>
	/// <returns>Логическое значение, указывающее, было ли создание успешным</returns>
	public Task<bool> CreateAuthUserAsync(AuthUserDatabase userAuthDatabase);

	/// <summary>
	/// Обновление данных пользователя для авторизации
	/// </summary>
	/// <param name="id">Id пользователя</param>
	/// <param name="userAuthDatabase">Обновленные данные для авторизации</param>
	/// <returns>Логическое значение, указывающее, было ли обновление успешным</returns>
	public Task<bool> UpdateAuthUserAsync(Guid id, AuthUserDatabase userAuthDatabase);

	/// <summary>
	/// Удаление данных пользователя для авторизации
	/// </summary>
	/// <param name="id">Id пользователя</param>
	/// <returns>Логическое значение, указывающее, было ли удаление успешным</returns>
	public Task<bool> DeleteAuthUserAsync(Guid id);
}