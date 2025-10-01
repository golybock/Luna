using Luna.Workspaces.Domain.Models;

namespace Luna.Workspaces.Services.Services.WorkspacePermissionService;

public interface IWorkspacePermissionService
{
	Task<bool> HasPermissionAsync(Guid workspaceId, Guid userId, string requiredPermission);
	Task<bool> HasAnyPermissionAsync(Guid workspaceId, Guid userId, params string[] requiredPermissions);
	Task<bool> HasAllPermissionsAsync(Guid workspaceId, Guid userId , params string[] requiredPermissions);
	Task<WorkspaceUserPermission?> GetUserPermissionsFromCacheAsync(Guid workspaceId, Guid userId);
	Task AddUserToWorkspaceAsync(WorkspaceUserDomain workspaceUser);
	Task UpdateUserWorkspace(Guid workspaceId, Guid userId, WorkspaceUserDomain workspaceUser);
	Task DeleteUserFromWorkspaceAsync(Guid workspaceId, Guid userId);
	Task DeleteUserFromWorkspaceByWorkspaceIdAsync(Guid workspaceId);
	Task DeleteUserFromWorkspaceByUserIdAsync(Guid userId);
}