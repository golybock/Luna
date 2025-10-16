using System.ComponentModel.DataAnnotations;
using Luna.Tools.Validation;

namespace Luna.Workspaces.Models.Blank.Models;

public class InviteUserBlank
{
	[Required]
	[EmailAddress]
	public string Email { get; set; }

	[Required]
	public Guid WorkspaceId { get; set; }

	[Required]
	[MinLength(1, ErrorMessage = "Пользователь должен иметь хотя бы одно разрешение")]
	[PermissionsValidation]
	public string[] Permissions { get; set; } = null!;
}