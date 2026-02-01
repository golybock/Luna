using System.Reflection;

namespace Luna.Auth.Repositories;

public class SqlQueries
{
	private static readonly Assembly Assembly = typeof(SqlQueries).Assembly;

	public static string GetSql(string name)
	{
		string resourceName = $"Luna.Auth.Repositories.SQL.{name}";
		using Stream? stream = Assembly.GetManifestResourceStream(resourceName);

		if (stream == null)
		{
			throw new InvalidOperationException($"SQL-query '{name}' not found");
		}

		using StreamReader reader = new StreamReader(stream);
		return reader.ReadToEnd();
	}

	public static class AuthRepository
	{
		public const string GetById = "AuthRepository.GetAuthUserById.sql";
		public const string GetByEmail = "AuthRepository.GetAuthUserByEmail.sql";
		public const string GetByEmailToken = "AuthRepository.GetAuthUserByEmailToken.sql";
		public const string GetByResetToken = "AuthRepository.GetAuthUserByResetToken.sql";
		public const string Create = "AuthRepository.CreateAuthUser.sql";
		public const string Update = "AuthRepository.UpdateAuthUser.sql";
		public const string Delete = "AuthRepository.DeleteAuthUser.sql";
	}

	public static class SessionArchiveRepository
	{
		public const string Create = "SessionArchiveRepository.CreateSession.sql";
		public const string GetById = "SessionArchiveRepository.GetSessionById.sql";
		public const string GetByUserId = "SessionArchiveRepository.GetSessionByUserId.sql";
		public const string DeleteSession = "SessionArchiveRepository.DeleteSession.sql";
		public const string DeleteSessions = "SessionArchiveRepository.DeleteSessions.sql";
	}

	public static class OutboxRepository
	{
		public const string Create = "OutboxRepository.CreateOutboxMessage.sql";
		public const string AcquirePending = "OutboxRepository.AcquirePendingMessages.sql";
		public const string MarkProcessed = "OutboxRepository.MarkProcessed.sql";
		public const string MarkForRetry = "OutboxRepository.MarkForRetry.sql";
		public const string MarkFailed = "OutboxRepository.MarkFailed.sql";
	}
}