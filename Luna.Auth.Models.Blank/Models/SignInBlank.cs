using System.ComponentModel.DataAnnotations;

namespace Luna.Auth.Models.Blank.Models;

public class SignInBlank
{
	[Required]
	public string Email { get; set; } = null!;
}