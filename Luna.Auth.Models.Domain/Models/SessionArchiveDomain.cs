namespace Luna.Auth.Models.Domain.Models;

public class SessionArchiveDomain
{
	public Guid Id { get; set; }

	public Guid UserId { get; set; }

	public String Token { get; set; } = null!;

	public String RefreshToken { get; set; } = null!;

	public String? Device {get; set; }

	public DateTime CreatedAt { get; set; }

	public DateTime ExpiresAt { get; set; }

	public String? UserAgent { get; set; }

	public String? IpAddress { get; set; }

	public DateTime? RevokedAt { get; set; }
}