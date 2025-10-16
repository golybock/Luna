using Luna.Workspaces.Models.Blank.Models;
using Luna.Workspaces.Models.Database.Models;

namespace Luna.Workspaces.Domain.Models;

public class InviteUserDomain
{
	public string Email { get; set; } = null!;
	public Guid WorkspaceId { get; set; }
	public string[] Permissions { get; set; } = null!;
	public Guid? InvitedBy { get; set; }

	public static InviteUserDomain FromBlank(InviteUserBlank inviteUserBlank, Guid invitedBy)
	{
		return new InviteUserDomain()
		{
			Email = inviteUserBlank.Email,
			WorkspaceId = inviteUserBlank.WorkspaceId,
			Permissions = inviteUserBlank.Permissions,
			InvitedBy = invitedBy
		};
	}

	public static InviteUserDomain FromDatabase(InviteUserDatabase inviteUserDatabase)
	{
		return new InviteUserDomain()
		{
			Email = inviteUserDatabase.Email,
			WorkspaceId = inviteUserDatabase.WorkspaceId,
			Permissions = inviteUserDatabase.Permissions,
			InvitedBy = inviteUserDatabase.InvitedBy
		};
	}

	public InviteUserDatabase ToDatabase()
	{
		return new InviteUserDatabase()
		{
			Email = Email,
			WorkspaceId = WorkspaceId,
			Permissions = Permissions,
			InvitedBy = InvitedBy
		};
	}

	public WorkspaceUserDomain ToWorkspaceUserDomain(Guid userId)
	{
		return new WorkspaceUserDomain
		{
			Id = Guid.NewGuid(),
			AcceptedAt = DateTime.UtcNow,
			Permissions = Permissions,
			WorkspaceId = WorkspaceId,
			UserId = userId,
			InvitedBy = InvitedBy
		};
	}

	public WorkspaceUserPermission ToWorkspaceUserPermission(Guid userId)
	{
		return new WorkspaceUserPermission()
		{
			UserId = userId,
			Permissions = Permissions,
			WorkspaceId = WorkspaceId,
		};
	}
}