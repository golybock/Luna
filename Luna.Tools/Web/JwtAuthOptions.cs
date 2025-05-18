using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;

namespace Luna.Tools.Web;

public class JwtAuthOptions : AuthenticationSchemeOptions
{
	private String Key { get; set; }

	public SymmetricSecurityKey SymmetricSecurityKey { get; set; }

	public String Issuer { get; set; }

	public String Audience { get; set; }

	public Int32 ValidInDays { get; set; }

	public Int32 RefreshValidInDays { get; set; }
}