using Luna.Workspaces.Domain.Models;

namespace Luna.Workspaces.Services.Services.PermissionEventService;

public interface IPermissionEventService
{
	Task CreateWorkspaceUserPermissionsAsync(WorkspaceUserPermission workspaceUserPermission);
	Task UpdateWorkspaceUserPermissions(WorkspaceUserPermission workspaceUserPermission);
	Task DeleteWorkspaceUserPermissionsById(Guid workspaceId, Guid userId);
	Task DeleteWorkspaceUserPermissionsByWorkspaceId(Guid workspaceId);
	Task DeleteWorkspaceUserPermissionsByUserId(Guid userId);
}