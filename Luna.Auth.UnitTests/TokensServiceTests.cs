using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FluentAssertions;
using Luna.Auth.Services.Services.TokensService;
using Luna.Tools.Web;
using Microsoft.IdentityModel.Tokens;

namespace Luna.Auth.UnitTests;

public class TokensServiceTests
{
    private readonly JwtOptions _jwtOptions;
    private readonly TokensService _tokensService;

    public TokensServiceTests()
    {
        _jwtOptions = new JwtOptions
        {
            Key = "your_test_secret_key_which_is_long_enough_for_testing",
            Issuer = "test_issuer",
            Audience = "test_audience",
            ValidInDays = 1
        };

        _tokensService = new TokensService(_jwtOptions);
    }

    [Fact]
    public void GenerateAccessToken_WithValidClaims_ReturnsValidToken()
    {
        List<Claim> claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Email, "test@example.com")
        };

        string token = _tokensService.GenerateAccessToken(claims);

        token.Should().NotBeNullOrEmpty();
        JwtSecurityTokenHandler jwtHandler = new JwtSecurityTokenHandler();
        TokenValidationParameters tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = _jwtOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = _jwtOptions.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = _jwtOptions.SymmetricSecurityKey,
            ValidateLifetime = true
        };

        ClaimsPrincipal? principal = jwtHandler.ValidateToken(token, tokenValidationParameters, out _);
        principal.Should().NotBeNull();
    }

    [Fact]
    public void ValidateJwt_WithValidToken_ReturnsJwtSecurityToken()
    {
        List<Claim> claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Email, "test@example.com")
        };

        string token = _tokensService.GenerateAccessToken(claims);

        JwtSecurityToken jwtToken = _tokensService.ValidateJwt(token, true);

        jwtToken.Should().NotBeNull();
    }
}