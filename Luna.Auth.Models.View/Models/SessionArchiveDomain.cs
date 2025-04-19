namespace Luna.Auth.Models.View.Models;

public class SessionArchiveDomain
{
	public Guid Id { get; set; }

	public String? Device {get; set; }

	public DateTime CreatedAt { get; set; }

	public DateTime ExpiresAt { get; set; }

	public String? UserAgent { get; set; }

	public String? IpAddress { get; set; }

	public DateTime? RevokedAt { get; set; }
}