using Luna.Workspaces.Models.Blank.Models;
using Luna.Workspaces.Models.View.Models;

namespace Luna.Workspaces.Services.Services.WorkspaceService;

public interface IWorkspaceService
{
	Task<WorkspaceView?> GetWorkspaceAsync(Guid workspaceId, Guid operationBy);
	Task<IEnumerable<WorkspaceView>> GetAvailableWorkspacesAsync(Guid operationBy);
	Task<WorkspaceDetailedView?> GetWorkspaceDetailedViewAsync(Guid workspaceId, Guid operationBy);
	Task<Guid> CreateWorkspaceAsync(WorkspaceBlank workspaceBlank, Guid operationBy);
	Task UpdateWorkspaceAsync(Guid workspaceId, Guid operationBy, WorkspaceBlank workspaceBlank);
	Task DeleteWorkspaceAsync(Guid workspaceId, Guid operationBy);
	Task<WorkspaceView?> GetWorkspaceByInviteAsync(Guid inviteId, Guid operationBy);
	Task AcceptInviteAsync(Guid inviteId, Guid operationBy);
	Task UpdateWorkspaceUserAsync(Guid workspaceUserId, Guid operationBy, WorkspaceUserBlank workspaceUserBlank);
	Task DeleteWorkspaceUserAsync(Guid workspaceUserId, Guid operationBy);
}