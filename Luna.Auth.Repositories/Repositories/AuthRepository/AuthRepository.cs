using Luna.Auth.Models.Database.Models;
using Npgsql;
using Npgsql.Extension.Options;
using Npgsql.Extension.Repositories;

namespace Luna.Auth.Repositories.Repositories.AuthRepository;

public class AuthRepository : NpgsqlRepository, IAuthRepository
{
	private const string TableName = "auth_users";

	public AuthRepository(IDatabaseOptions databaseOptions) : base(databaseOptions) { }

	public async Task<AuthUserDatabase?> GetAuthUserAsync(Guid id)
	{
		string query = $"select * from {TableName} where id = $1";

		NpgsqlParameter[] parameters =
		[
			new NpgsqlParameter() {Value = id}
		];

		return await GetAsync<AuthUserDatabase>(query, parameters);
	}

	public async Task<bool> CreateAuthUserAsync(AuthUserDatabase userAuthDatabase)
	{
		string query = $"insert into {TableName} (id, password_hash, email)" +
		               $"values ($1, $2, $3)";

		NpgsqlParameter[] parameters =
		[
			new NpgsqlParameter() {Value = userAuthDatabase.Id},
			new NpgsqlParameter() {Value = userAuthDatabase.PasswordHash == null ? DBNull.Value : userAuthDatabase.PasswordHash},
			new NpgsqlParameter() {Value = userAuthDatabase.Email},
		];

		return await ExecuteAsync(query, parameters);
	}

	public async Task<bool> UpdateAuthUserAsync(Guid id, AuthUserDatabase userAuthDatabase)
	{
		string query = $"update {TableName} set password_hash = $2, email = $3, " +
		               $"status = $4, email_confirmded = $5, verification_token = $6, " +
		               $"reset_password_token = $7, reset_token_expires = $8 where id = $1";

		NpgsqlParameter[] parameters =
		[
			new NpgsqlParameter() {Value = userAuthDatabase.Id},
			new NpgsqlParameter() {Value = userAuthDatabase.PasswordHash == null ? DBNull.Value : userAuthDatabase.PasswordHash},
			new NpgsqlParameter() {Value = userAuthDatabase.Email},
			new NpgsqlParameter() {Value = userAuthDatabase.Status},
			new NpgsqlParameter() {Value = userAuthDatabase.EmailConfirmed},
			new NpgsqlParameter() {Value = userAuthDatabase.VerificationToken == null ? DBNull.Value : userAuthDatabase.VerificationToken},
			new NpgsqlParameter() {Value = userAuthDatabase.ResetPasswordToken == null ? DBNull.Value : userAuthDatabase.ResetPasswordToken},
			new NpgsqlParameter() {Value = userAuthDatabase.ResetTokenExpires == null ? DBNull.Value : userAuthDatabase.ResetTokenExpires},
		];

		return await ExecuteAsync(query, parameters);
	}

	public async Task<bool> DeleteAuthUserAsync(Guid id)
	{
		string query = $"delete from {TableName} where id = $1";

		NpgsqlParameter[] parameters =
		[
			new NpgsqlParameter() {Value = id}
		];

		return await ExecuteAsync(query, parameters);
	}
}