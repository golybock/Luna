namespace Luna.Tools.Database.Npgsql.Options;

public class DatabaseOptions : IDatabaseOptions
{
	public string ConnectionString { get; set; } = null!;
}