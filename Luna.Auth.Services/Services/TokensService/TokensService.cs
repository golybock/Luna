using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Luna.Tools.Web;
using Microsoft.IdentityModel.Tokens;

namespace Luna.Auth.Services.Services.TokensService;

public class TokensService : ITokensService
{
	private readonly JwtOptions _jwtOptions;

	public TokensService(JwtOptions jwtOptions)
	{
		_jwtOptions = jwtOptions;
	}

	public string GenerateAccessToken(IEnumerable<Claim> claims)
	{
		SymmetricSecurityKey secretKey = _jwtOptions.SymmetricSecurityKey;
		SigningCredentials signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
		DateTime expirationTimeStamp = DateTime.UtcNow.AddDays(_jwtOptions.ValidInDays);

		JwtSecurityToken tokenOptions = new JwtSecurityToken(
			issuer: _jwtOptions.Issuer,
			audience: _jwtOptions.Audience,
			claims: claims,
			expires: expirationTimeStamp,
			signingCredentials: signingCredentials
		);

		return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
	}

	public JwtSecurityToken ValidateJwt(string token, bool validateLifetime = false)
	{
		TokenValidationParameters validationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidIssuer = _jwtOptions.Issuer,
			ValidateAudience = true,
			ValidAudience = _jwtOptions.Audience,
			ValidateIssuerSigningKey = true,
			IssuerSigningKey = _jwtOptions.SymmetricSecurityKey,
			ValidateLifetime = validateLifetime
		};

		try
		{
			JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
			tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

			return (JwtSecurityToken) validatedToken;
		}
		catch (SecurityTokenValidationException ex)
		{
			throw new InvalidOperationException("Invalid JWT token: " + ex.Message, ex);
		}
	}

	public string GenerateRefreshToken()
	{
		return Guid.NewGuid().ToString();
	}
}