using Luna.Workspaces.Models.Blank.Models;
using Luna.Workspaces.Models.Database.Models;

namespace Luna.Workspaces.Domain.Models;

public class WorkspaceUserDomain
{
	public Guid Id { get; set; }
	public Guid UserId { get; set; }
	public Guid WorkspaceId { get; set; }
	public string[] Permissions { get; set; } = null!;
	public DateTime CreatedAt { get; set; }
	public DateTime UpdatedAt { get; set; }
	public Guid? InvitedBy { get; set; }
	public DateTime? AcceptedAt { get; set; }

	public WorkspaceUserDatabase ToDatabase()
	{
		return new WorkspaceUserDatabase()
		{
			Id = Id,
			UserId = UserId,
			WorkspaceId = WorkspaceId,
			Permissions = Permissions,
			CreatedAt = CreatedAt,
			UpdatedAt = UpdatedAt,
			AcceptedAt = AcceptedAt,
			InvitedBy = InvitedBy
		};
	}

	public static WorkspaceUserDomain FromBlank(WorkspaceUserBlank workspaceUserBlank)
	{
		return new WorkspaceUserDomain()
		{
			UserId = workspaceUserBlank.UserId,
			WorkspaceId = workspaceUserBlank.WorkspaceId,
			Permissions = workspaceUserBlank.Permissions,
			UpdatedAt = DateTime.UtcNow
		};
	}

	public static WorkspaceUserDomain FromDatabase(WorkspaceUserDatabase database)
	{
		return new WorkspaceUserDomain()
		{
			Id = database.Id,
			UserId = database.UserId,
			WorkspaceId = database.WorkspaceId,
			Permissions = database.Permissions,
			CreatedAt = database.CreatedAt,
			UpdatedAt = database.UpdatedAt,
			AcceptedAt = database.AcceptedAt,
			InvitedBy = database.InvitedBy
		};
	}

	public WorkspaceUserPermission ToWorkspaceUserPermission()
	{
		return new WorkspaceUserPermission()
		{
			UserId = UserId,
			WorkspaceId = WorkspaceId,
			Permissions = Permissions,
		};
	}
}