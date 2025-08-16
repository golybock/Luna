namespace Luna.Users.Models.View.Models;

public class UserView
{
	public Guid Id { get; set; }
	public string Username { get; set; } = null!;
	public string? DisplayName { get; set; }
	public string? Image { get; set; }
	public string? Bio { get; set; }
	public DateTime LastActive { get; set; }
}