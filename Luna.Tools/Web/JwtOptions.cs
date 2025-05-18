using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Luna.Tools.Web;

public class JwtOptions
{
	private readonly IConfiguration _configuration;

	public JwtOptions(IConfiguration configuration)
	{
		_configuration = configuration;

		Key = _configuration["JWT:Key"] ?? throw new ArgumentNullException("Key");
		Issuer = _configuration["JWT:Issuer"] ?? throw new ArgumentNullException("Issuer");
		Audience = _configuration["JWT:Audience"] ?? throw new ArgumentNullException("Audience");
		ValidInDays = Int32.Parse(_configuration["JWT:ValidInDays"] ?? throw new ArgumentNullException("ValidInDays"));
		RefreshValidInDays = Int32.Parse(_configuration["JWT:RefreshValidInDays"] ?? throw new ArgumentNullException("RefreshValidInDays"));
	}

	public JwtOptions() { }

	public String Key { get; set; }

	public SymmetricSecurityKey SymmetricSecurityKey => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));

	public String Issuer { get; set; }

	public String Audience { get; set; }

	public Int32 ValidInDays { get; set; }

	public Int32 RefreshValidInDays { get; set; }
}