using Luna.Auth.Services.Services.AccountManagementService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ControllerBase = Luna.Tools.Web.ControllerBase;

namespace Luna.Auth.API.Controllers;

[ApiController, Authorize]
[Route("api/v1/[controller]")]
public class AccountManagementController : ControllerBase
{
	private readonly IAccountManagementService _accountManagementService;

	public AccountManagementController(IAccountManagementService accountManagementService)
	{
		_accountManagementService = accountManagementService;
	}

	[HttpPost("[action]")]
	public async Task<ActionResult> RequestEmailVerification()
	{
		await _accountManagementService.RequestEmailVerificationAsync(UserId!.Value);
		return Ok();
	}

	[HttpPost("[action]")]
	[AllowAnonymous]
	public async Task<ActionResult> VerifyEmail(string verificationToken)
	{
		await _accountManagementService.VerifyEmailAsync(verificationToken);
		return Ok();
	}
}