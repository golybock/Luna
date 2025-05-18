using System.Security.Claims;
using Luna.Auth.Models.Blank.Models;
using Luna.Auth.Models.Domain.Models;
using Luna.Auth.Models.View.Models;
using Luna.Auth.Services.Extensions;
using Luna.Auth.Services.Services.AuthService;
using Luna.Tools.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ControllerBase = Luna.Tools.Web.ControllerBase;

namespace Luna.Auth.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
	private readonly IAuthService _authService;
	private readonly JwtOptions _jwtOptions;

	public AuthController(
		IAuthService authService,
		JwtOptions jwtOptions
	)
	{
		_authService = authService;
		_jwtOptions = jwtOptions;
	}

	[AllowAnonymous]
	[HttpPost("[action]")]
	public async Task<ActionResult<AuthView>> Login(SignInBlank signInBlank)
	{
		AuthDomain result = await _authService.LoginAsync(signInBlank);

		SetAuthCookies(HttpContext, result.Token, result.RefreshToken);

		return Ok(result.ToView());
	}

	[AllowAnonymous]
	[HttpPost("[action]")]
	public async Task<ActionResult<AuthView>> Register(SignUpBlank signUpBlank)
	{
		AuthDomain result = await _authService.RegisterAsync(signUpBlank);

		SetAuthCookies(HttpContext, result.Token, result.RefreshToken);

		return Ok(result.ToView());
	}

	[AllowAnonymous]
	[HttpGet("[action]")]
	public ActionResult LoginOauth2(string provider)
	{
		if (provider == "google")
		{
			string? redirectUri = Url.Action("GoogleCallback", "Auth", null, Request.Scheme, Request.Host.Value);
			return Challenge(new AuthenticationProperties {
				RedirectUri = redirectUri
			}, GoogleDefaults.AuthenticationScheme);
		}

		return BadRequest();
	}

	[AllowAnonymous]
	[HttpGet("[action]")]
	public async Task<IActionResult> GoogleCallback()
	{
		AuthenticateResult authResult = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
		if (!authResult.Succeeded) return BadRequest("Ошибка аутентификации.");

		string? email = authResult.Principal.FindFirst(ClaimTypes.Email)?.Value;

		if (email == null)
		{
			return BadRequest("Email not provided.");
		}

		AuthDomain result = await _authService.LoginOauth2(email);

		SetAuthCookies(HttpContext, result.Token, result.RefreshToken);

		return Ok(result.ToView());
	}

	[Authorize]
	[HttpPost("[action]")]
	public async Task<ActionResult> Logout()
	{
		await _authService.LogoutAsync(UserId!.Value, SessionId!.Value);

		HttpContext.Response.Cookies.Delete("access_token");
		HttpContext.Response.Cookies.Delete("refresh_token");

		return Ok();
	}

	[Authorize]
	[HttpGet("[action]")]
	public ActionResult Validate()
	{
		return Ok(new
		{
			UserId = UserId!.Value,
			SessionId = SessionId!.Value,
			Email = Email
		});
	}

	private void SetAuthCookies(HttpContext context, string accessToken, string refreshToken)
	{
		CookieOptions accessTokenOptions = new CookieOptions
		{
			HttpOnly = true,
			Secure = true,
			SameSite = SameSiteMode.Strict,
			Expires = DateTime.UtcNow.AddDays(_jwtOptions.ValidInDays)
		};

		CookieOptions refreshTokenOptions = new CookieOptions
		{
			HttpOnly = true,
			Secure = true,
			SameSite = SameSiteMode.Strict,
			Expires = DateTime.UtcNow.AddDays(_jwtOptions.RefreshValidInDays)
		};

		context.Response.Cookies.Append("access_token", accessToken, accessTokenOptions);
		context.Response.Cookies.Append("refresh_token", refreshToken, refreshTokenOptions);
	}
}