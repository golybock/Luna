using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Luna.Auth.Services.Services.TokensService;

public interface ITokensService
{
	string GenerateAccessToken(IEnumerable<Claim> claims);

	string GenerateRefreshToken();

	JwtSecurityToken ValidateJwt(string token, bool validateLifetime = false);
}