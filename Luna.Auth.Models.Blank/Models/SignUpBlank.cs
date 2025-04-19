using System.ComponentModel.DataAnnotations;

namespace Luna.Auth.Models.Blank.Models;

public class SignUpBlank
{
	[Required]
	public String Username { get; set; } = null!;

	[Required]
	public String Email { get; set; } = null!;

	[Required]
	public String Password { get; set; } = null!;
}