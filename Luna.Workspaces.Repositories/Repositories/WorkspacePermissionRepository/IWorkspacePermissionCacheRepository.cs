using Luna.Workspaces.Domain.Models;

namespace Luna.Workspaces.Repositories.Repositories.WorkspacePermissionRepository;

public interface IWorkspacePermissionCacheRepository
{
	Task<WorkspaceUserPermission?> GetUserPermissionAsync(Guid workspaceId, Guid userId);
	Task<IEnumerable<WorkspaceUserPermission>> GetWorkspacePermissionsAsync(Guid workspaceId);
	Task SetUserPermissionAsync(WorkspaceUserPermission permission);
	Task DeleteUserPermissionAsync(Guid workspaceId, Guid userId);
	Task DeleteUserPermissionByWorkspaceIdAsync(Guid workspaceId);
	Task DeleteUserPermissionByUserIdAsync(Guid userId);
}