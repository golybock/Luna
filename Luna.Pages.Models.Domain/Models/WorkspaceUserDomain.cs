using Luna.Pages.Models.Database.WorkspaceUser;

namespace Luna.Pages.Models.Domain.Models;

public class WorkspaceUserDomain
{
	public Guid Id { get; set; }

	public Guid UserId { get; set; }

	public Guid WorkspaceId { get; set; }

	public List<string> Permissions { get; set; } = null!;

	public DateTime CreatedAt { get; set; }

	public DateTime UpdatedAt { get; set; }

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
		};
	}

	public static WorkspaceUserDomain FromPermission(WorkspaceUserPermission workspaceUserPermission)
	{
		return new WorkspaceUserDomain()
		{
			Id = Guid.NewGuid(),
			UserId = workspaceUserPermission.UserId,
			WorkspaceId = workspaceUserPermission.WorkspaceId,
			Permissions = workspaceUserPermission.Permissions.ToList(),
		};
	}

	public WorkspaceUserPermission ToWorkspaceUserPermission()
	{
		return new WorkspaceUserPermission()
		{
			UserId = UserId,
			WorkspaceId = WorkspaceId,
			Permissions = Permissions.ToArray(),
		};
	}
}