

using Luna.Users.Models.View.Models;
using Luna.Workspaces.Domain.Models;
using Luna.Workspaces.Models.Blank.Models;
using Luna.Workspaces.Models.Database.Models;
using Luna.Workspaces.Models.View.Models;

namespace Luna.Workspaces.Models.Extensions.Extensions;

public static class WorkspaceExtensions
{
	public static WorkspaceDomain ToDomain(this WorkspaceDatabase workspaceDatabase)
	{
		return new WorkspaceDomain()
		{
			CreatedAt = workspaceDatabase.CreatedAt,
			DefaultPermission = workspaceDatabase.DefaultPermission,
			DeletedAt = workspaceDatabase.DeletedAt,
			Description = workspaceDatabase.Description,
			Visibility = workspaceDatabase.Visibility,
			Icon = workspaceDatabase.Icon,
			Name = workspaceDatabase.Name,
			Settings = workspaceDatabase.Settings,
			Id = workspaceDatabase.Id,
			OwnerId = workspaceDatabase.OwnerId,
			UpdatedAt = workspaceDatabase.UpdatedAt,
			Users = workspaceDatabase.WorkspaceUsers.Select(user => user.ToDomain()).ToList()
		};
	}

	public static WorkspaceView ToView(this WorkspaceDomain workspaceDomain)
	{
		return new WorkspaceView()
		{
			DefaultPermission = workspaceDomain.DefaultPermission,
			DeletedAt = workspaceDomain.DeletedAt,
			Description = workspaceDomain.Description,
			Visibility = workspaceDomain.Visibility,
			Icon = workspaceDomain.Icon,
			Name = workspaceDomain.Name,
			Settings = workspaceDomain.Settings,
			Id = workspaceDomain.Id,
			OwnerId = workspaceDomain.OwnerId,
			Users = workspaceDomain.Users.Select(user => user.ToView()).ToList()
		};
	}

	public static WorkspaceUserDomain ToDomain(this WorkspaceUserDatabase workspaceUserDatabase)
	{
		return new WorkspaceUserDomain()
		{
			CreatedAt = workspaceUserDatabase.CreatedAt,
			Id = workspaceUserDatabase.Id,
			UpdatedAt = workspaceUserDatabase.UpdatedAt,
			AcceptedAt = workspaceUserDatabase.AcceptedAt,
			InvitedBy = workspaceUserDatabase.InvitedBy,
			Permissions = workspaceUserDatabase.Permissions,
			Roles = workspaceUserDatabase.Roles,
			UserId = workspaceUserDatabase.UserId,
			WorkspaceId = workspaceUserDatabase.WorkspaceId,
		};
	}

	public static WorkspaceUserView ToView(this WorkspaceUserDomain workspaceUserDomain)
	{
		return new WorkspaceUserView()
		{
			Id = workspaceUserDomain.Id,
			AcceptedAt = workspaceUserDomain.AcceptedAt,
			InvitedBy = workspaceUserDomain.InvitedBy,
			Permissions = workspaceUserDomain.Permissions,
			Roles = workspaceUserDomain.Roles,
			UserId = workspaceUserDomain.UserId,
			WorkspaceId = workspaceUserDomain.WorkspaceId,
		};
	}

	public static WorkspaceUserDetailedView ToDetailedView(this WorkspaceUserDatabase workspaceUserDatabase, UserView?  userView)
	{
		return new WorkspaceUserDetailedView()
		{
			Id = workspaceUserDatabase.Id,
			AcceptedAt = workspaceUserDatabase.AcceptedAt,
			InvitedBy = workspaceUserDatabase.InvitedBy,
			Permissions = workspaceUserDatabase.Permissions,
			Roles = workspaceUserDatabase.Roles,
			UserId = workspaceUserDatabase.UserId,
			WorkspaceId = workspaceUserDatabase.WorkspaceId,
			User = userView
		};
	}

	public static WorkspaceUserDomain ToDomain(this WorkspaceUserBlank workspaceUserBlank)
	{
		return new WorkspaceUserDomain()
		{
			UserId = workspaceUserBlank.UserId,
			WorkspaceId =workspaceUserBlank.WorkspaceId,
			Permissions = workspaceUserBlank.Permissions,
			Roles = workspaceUserBlank.Roles,
		};
	}

	public static WorkspaceUserDatabase ToDatabase(this WorkspaceUserDomain workspaceUserDomain)
	{
		return new WorkspaceUserDatabase()
		{
			Id = workspaceUserDomain.Id,
			AcceptedAt = workspaceUserDomain.AcceptedAt,
			InvitedBy = workspaceUserDomain.InvitedBy,
			Permissions = workspaceUserDomain.Permissions,
			Roles = workspaceUserDomain.Roles,
			UserId = workspaceUserDomain.UserId,
			WorkspaceId = workspaceUserDomain.WorkspaceId,
		};
	}

	public static WorkspaceDatabase ToDatabase(this WorkspaceBlank workspaceBlank)
	{
		return new WorkspaceDatabase()
		{
			DefaultPermission = workspaceBlank.DefaultPermission,
			Description = workspaceBlank.Description,
			Visibility = workspaceBlank.Visibility,
			Icon = workspaceBlank.Icon,
			Name = workspaceBlank.Name,
			Settings = workspaceBlank.Settings
		};
	}

	public static WorkspaceDetailedView ToDetailedView(this WorkspaceDatabase workspaceDatabase,
		IEnumerable<WorkspaceUserDetailedView> userViews)
	{
		return new WorkspaceDetailedView()
		{
			Id = workspaceDatabase.Id,
			DefaultPermission = workspaceDatabase.DefaultPermission,
			Description = workspaceDatabase.Description,
			Visibility = workspaceDatabase.Visibility,
			Icon = workspaceDatabase.Icon,
			Name = workspaceDatabase.Name,
			Settings = workspaceDatabase.Settings,
			DeletedAt = workspaceDatabase.DeletedAt,
			OwnerId = workspaceDatabase.OwnerId,
			Users = userViews
		};
	}
}