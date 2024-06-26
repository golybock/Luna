﻿using Luna.Models.Users.Blank.Users;
using Luna.Models.Users.View.Users;
using Luna.SharedDataAccess.Users.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ControllerBase = Luna.Tools.Web.ControllerBase;

namespace Luna.Users.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
	private readonly IUserService _userService;

	public UsersController(IUserService userService)
	{
		_userService = userService;
	}

	[HttpGet("[action]")]
	public async Task<IEnumerable<UserView>> GetUsersAsync()
	{
		return await _userService.GetUsersAsync();
	}

	[HttpGet("[action]")]
	public async Task<UserView?> GetUserByIdAsync(Guid id)
	{
		return await _userService.GetUserAsync(id);
	}

	[HttpGet("[action]")]
	public async Task<UserView?> GetMeAsync()
	{
		return await _userService.GetUserAsync(UserId);
	}

	[HttpGet("[action]")]
	public async Task<UserView?> GetUserAsync(string phoneOrEmail)
	{
		return await _userService.GetUserAsync(phoneOrEmail);
	}

	[HttpPost("[action]")]
	public async Task<Guid> CreateUserAsync(UserBlank userBlank)
	{
		return await _userService.CreateUserAsync(userBlank);
	}

	[HttpPut("[action]")]
	public async Task<bool> UpdateUserAsync(UserBlank userBlank)
	{
		return await _userService.UpdateUserAsync(UserId, userBlank);
	}

	[HttpDelete("[action]")]
	public async Task<bool> DeleteUserByIdAsync(Guid id)
	{
		return await _userService.DeleteUserAsync(id);
	}

	[HttpDelete("[action]")]
	public async Task<bool> DeleteUserAsync(string username)
	{
		return await _userService.DeleteUserAsync(username);
	}
}