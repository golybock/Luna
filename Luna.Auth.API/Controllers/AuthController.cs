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
	public async Task<ActionResult> RequestVerificationCodeAsync(SignInBlank signInBlank)
	{
		await _authService.RequestVerificationCodeAsync(signInBlank);
		return Ok();
	}

	[AllowAnonymous]
	[HttpPost("[action]")]
	public async Task<ActionResult<AuthView>> SignIn(SignInCodeBlank signInCodeBlank)
	{
		AuthDomain result = await _authService.SignInAsync(signInCodeBlank);

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
		string? username = authResult.Principal.FindFirst("preferred_username")?.Value;
		string? name = authResult.Principal.FindFirst(ClaimTypes.Name)?.Value;
		string? pictureUrl = authResult.Principal.FindFirst("picture")?.Value;

		if (email == null)
		{
			return BadRequest("Email not provided.");
		}

		OAuth2Blank oauth2Blank = new OAuth2Blank()
		{
			Email = email,
			Username = username ?? name ?? email
		};

		if (!string.IsNullOrEmpty(pictureUrl))
		{
			oauth2Blank.ImageLink =  pictureUrl;
		}

		AuthDomain result = await _authService.LoginOauth2(oauth2Blank);

		SetAuthCookies(HttpContext, result.Token, result.RefreshToken);

		return Redirect(Environment.GetEnvironmentVariable("OAUTH2_REDIRECT_URL"));
	}

	[Authorize]
	[HttpPost("[action]")]
	public async Task<ActionResult> Logout()
	{
		try
		{
			await _authService.LogoutAsync(UserId!.Value, SessionId!.Value);
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
		}
		finally
		{
			HttpContext.Response.Cookies.Delete("access_token");
			HttpContext.Response.Cookies.Delete("refresh_token");
		}

		return Ok();
	}

	[Authorize]
	[HttpGet("[action]")]
	public Task<ActionResult> Validate()
	{
		return Task.FromResult<ActionResult>(Ok(new AuthView()
		{
			UserId = UserId!.Value,
			Email = Email!,
			SessionId = SessionId!.Value
		}));
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