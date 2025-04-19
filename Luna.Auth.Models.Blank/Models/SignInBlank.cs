using System.ComponentModel.DataAnnotations;

namespace Luna.Auth.Models.Blank.Models;

public class SignInBlank
{
	[Required]
	public String Email { get; set; } = null!;

	[Required]
	public String Password { get; set; } = null!;
}