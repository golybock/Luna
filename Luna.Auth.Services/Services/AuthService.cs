﻿using System.Security.Claims;
using Luna.Auth.Repositories.Repositories;
using Luna.Models.Auth.Blank.Auth;
using Luna.Models.Auth.Database.Auth;
using Luna.Models.Auth.Domain.Auth;
using Luna.Models.Users.Blank.Users;
using Luna.SharedDataAccess.Users.Services;
using Luna.Tools.Crypto;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Luna.Auth.Services.Services;

public class AuthService: IAuthService
{
	private readonly IUserAuthRepository _userAuthRepository;
	private readonly IUserService _userService;

	public AuthService(IUserAuthRepository userAuthRepository, IUserService userService)
	{
		_userAuthRepository = userAuthRepository;
		_userService = userService;
	}

	public async Task<IActionResult> SignIn(SignInBlank signInBlank, HttpContext context)
	{
		var user = await _userService.GetUserAsync(signInBlank.Email);

		if (user == null)
			return new NotFoundObjectResult("user not found");

		var auth = await _userAuthRepository.GetAuthUserAsync(user.Id);

		if (auth == null)
			return new NotFoundObjectResult("pass not found");

		var authDomain = new UserAuthDomain(auth);

		var hashedPassword = await Crypto.HashSha512Async(signInBlank.Password);

		if (!authDomain.PasswordIsValid(hashedPassword))
			return new UnauthorizedResult();

		var claims = new List<Claim>
		{
			new Claim(ClaimTypes.Authentication, user.Id.ToString(), CookieAuthenticationDefaults.AuthenticationScheme),
			new Claim(ClaimTypes.Name, user.Email, CookieAuthenticationDefaults.AuthenticationScheme)
			// new Claim(ClaimTypes.Role, "Administrator"),
		};

		var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

		var authProperties = new AuthenticationProperties
		{
			AllowRefresh = true,
		};

		await context.SignInAsync(new ClaimsPrincipal(claimsIdentity), authProperties);

		return new OkResult();
	}

	public async Task<IActionResult> SignUp(SignUpBlank signUpBlank, HttpContext context)
	{
		var user = await _userService.GetUserAsync(signUpBlank.Email);

		if (user != null)
			return new BadRequestObjectResult("User already registered");

		var newUser = new UserBlank()
		{
			Email = signUpBlank.Email,
			Username = signUpBlank.Email
		};

		var newUserId = await _userService.CreateUserAsync(newUser);

		var hashedPassword = await Crypto.HashSha512Async(signUpBlank.Password);

		var auth = new UserAuthDatabase()
		{
			Id = Guid.NewGuid(),
			Password = hashedPassword,
			UserId = newUserId
		};

		var res = await _userAuthRepository.CreateAuthUserAsync(auth);

		if (res == false)
			return new BadRequestResult();

		var claims = new List<Claim>
		{
			new Claim(ClaimTypes.Authentication, newUserId.ToString(), CookieAuthenticationDefaults.AuthenticationScheme),
			new Claim(ClaimTypes.Name, newUser.Email, CookieAuthenticationDefaults.AuthenticationScheme)
			// new Claim(ClaimTypes.Role, "Administrator"),
		};

		var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

		var authProperties = new AuthenticationProperties
		{
			AllowRefresh = true,
		};

		await context.SignInAsync(new ClaimsPrincipal(claimsIdentity), authProperties);

		return new OkResult();
	}

	public async Task<IActionResult> SignOut(HttpContext context)
	{
		await context.SignOutAsync();

		return new OkResult();
	}
}