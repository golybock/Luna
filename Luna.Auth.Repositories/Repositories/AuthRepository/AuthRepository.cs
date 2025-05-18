using Luna.Auth.Models.Database.Models;
using Luna.Tools.Database.Npgsql.Options;
using Luna.Tools.Database.Npgsql.Repositories;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Luna.Auth.Repositories.Repositories.AuthRepository;

public class AuthRepository : NpgsqlRepository, IAuthRepository
{
	private readonly ILogger<AuthRepository> _logger;

	public AuthRepository(IDatabaseOptions databaseOptions, ILogger<AuthRepository> logger) : base(databaseOptions)
	{
		_logger = logger;
	}

	public async Task<AuthUserDatabase?> GetAuthUserAsync(Guid id)
	{
		try
		{
			string query = SqlQueries.GetSql(SqlQueries.AuthRepository.GetById);

			NpgsqlParameter[] parameters =
			[
				new NpgsqlParameter() {Value = id}
			];

			return await GetAsync<AuthUserDatabase>(query, parameters);
		}
		catch (Exception e)
		{
			_logger.LogError("Error while executing {Method}: {Error}", nameof(GetAuthUserAsync), e.Message);
			throw;
		}
	}

	public async Task<AuthUserDatabase?> GetAuthUserAsync(string email)
	{
		try
		{
			string query = SqlQueries.GetSql(SqlQueries.AuthRepository.GetByEmail);

			NpgsqlParameter[] parameters =
			[
				new NpgsqlParameter() {Value = email}
			];

			return await GetAsync<AuthUserDatabase>(query, parameters);
		}
		catch (Exception e)
		{
			_logger.LogError("Error while executing {Method}: {Error}", nameof(GetAuthUserAsync), e.Message);
			throw;
		}
	}

	public async Task<AuthUserDatabase?> GetAuthUserByEmailTokenAsync(string verificationToken)
	{
		try
		{
			string query = SqlQueries.GetSql(SqlQueries.AuthRepository.GetByEmailToken);

			NpgsqlParameter[] parameters =
			[
				new NpgsqlParameter() {Value = verificationToken}
			];

			return await GetAsync<AuthUserDatabase>(query, parameters);
		}
		catch (Exception e)
		{
			_logger.LogError("Error while executing {Method}: {Error}", nameof(GetAuthUserAsync), e.Message);
			throw;
		}
	}

	public async Task<AuthUserDatabase?> GetAuthUserByResetTokenAsync(string resetToken)
	{
		try
		{
			string query = SqlQueries.GetSql(SqlQueries.AuthRepository.GetByResetToken);

			NpgsqlParameter[] parameters =
			[
				new NpgsqlParameter() {Value = resetToken}
			];

			return await GetAsync<AuthUserDatabase>(query, parameters);
		}
		catch (Exception e)
		{
			_logger.LogError("Error while executing {Method}: {Error}", nameof(GetAuthUserByResetTokenAsync), e.Message);
			throw;
		}
	}

	public async Task<bool> CreateAuthUserAsync(AuthUserDatabase userAuthDatabase)
	{
		try
		{
			string query = SqlQueries.GetSql(SqlQueries.AuthRepository.Create);

			NpgsqlParameter[] parameters =
			[
				new NpgsqlParameter() {Value = userAuthDatabase.Id},
				new NpgsqlParameter()
					{Value = userAuthDatabase.PasswordHash == null ? DBNull.Value : userAuthDatabase.PasswordHash},
				new NpgsqlParameter() {Value = userAuthDatabase.Email},
			];

			return await ExecuteAsync(query, parameters);
		}
		catch (Exception e)
		{
			_logger.LogError("Error while executing {Method}: {Error}", nameof(CreateAuthUserAsync), e.Message);
			throw;
		}
	}

	public async Task<bool> UpdateAuthUserAsync(Guid id, AuthUserDatabase userAuthDatabase)
	{
		try
		{
			string query = SqlQueries.GetSql(SqlQueries.AuthRepository.Update);

			NpgsqlParameter[] parameters =
			[
				new NpgsqlParameter() {Value = id},
				new NpgsqlParameter()
					{Value = userAuthDatabase.PasswordHash == null ? DBNull.Value : userAuthDatabase.PasswordHash},
				new NpgsqlParameter() {Value = userAuthDatabase.Email},
				new NpgsqlParameter() {Value = userAuthDatabase.Status},
				new NpgsqlParameter() {Value = userAuthDatabase.EmailConfirmed},
				new NpgsqlParameter()
				{
					Value = userAuthDatabase.VerificationToken == null
						? DBNull.Value
						: userAuthDatabase.VerificationToken
				},
				new NpgsqlParameter()
				{
					Value = userAuthDatabase.ResetPasswordToken == null
						? DBNull.Value
						: userAuthDatabase.ResetPasswordToken
				},
				new NpgsqlParameter()
				{
					Value = userAuthDatabase.ResetTokenExpires == null
						? DBNull.Value
						: userAuthDatabase.ResetTokenExpires
				},
			];

			return await ExecuteAsync(query, parameters);
		}
		catch (Exception e)
		{
			_logger.LogError("Error while executing {Method}: {Error}", nameof(UpdateAuthUserAsync), e.Message);
			throw;
		}
	}

	public async Task<bool> DeleteAuthUserAsync(Guid id)
	{
		try
		{
			string query = SqlQueries.GetSql(SqlQueries.AuthRepository.Delete);

			NpgsqlParameter[] parameters =
			[
				new NpgsqlParameter() {Value = id}
			];

			return await ExecuteAsync(query, parameters);
		}
		catch (Exception e)
		{
			_logger.LogError("Error while executing {Method}: {Error}", nameof(DeleteAuthUserAsync), e.Message);
			throw;
		}
	}
}