using Luna.Tools.Web;
using Luna.Users.Models.Blank.Models;
using Luna.Users.Models.View.Models;
using Luna.Users.Services.Services.User;
using Microsoft.AspNetCore.Mvc;

namespace Luna.Users.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class UserController : ServiceControllerBase
{
	private readonly IUserService _userService;

	public UserController(IUserService userService)
	{
		_userService = userService;
	}

	[HttpGet("[action]")]
	public async Task<ActionResult<UserView>> GetUser(Guid? userId, string? username)
	{
		UserView? user = null;

		if (userId != null)
		{
			user = await _userService.GetUserByIdAsync(userId.Value);
		}

		if (!string.IsNullOrEmpty(username))
		{
			user = await _userService.GetUserByUsernameAsync(username);
		}

		return user == null ? NotFound() : Ok(user);
	}

	[HttpGet("[action]")]
	public async Task<ActionResult<UserView>> GetUsers([FromQuery] List<Guid> userIds)
	{
		IEnumerable<UserView> users = await _userService.GetUsersByIdsAsync(userIds);

		return Ok(users);
	}

	[HttpPut("[action]")]
	public async Task<ActionResult> UpdateUser(UserBlank user)
	{
		// В случае ошибки вызовет исключение, которое будет обработано MiddleWare
		await _userService.UpdateUserAsync(UserId, user);
		return Ok();
	}
}