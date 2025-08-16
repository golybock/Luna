using Luna.Workspaces.Models.Blank.Models;
using Luna.Workspaces.Models.View.Models;

namespace Luna.Workspaces.Services.Services.WorkspaceService;

public interface IWorkspaceService
{
	Task<WorkspaceView?> GetWorkspaceAsync(Guid operationBy, Guid id);
	Task<IEnumerable<WorkspaceView>> GetUserAvailableWorkspacesAsync(Guid userId);

	Task<WorkspaceDetailedView?> GetWorkspaceDetailedViewAsync(Guid operationBy, Guid id);
	Task<IEnumerable<WorkspaceDetailedView>> GetUserAvailableWorkspacesDetailedViewAsync(Guid userId);

	Task<Guid> CreateWorkspaceAsync(Guid operationBy, WorkspaceBlank workspaceBlank);
	Task UpdateWorkspaceAsync(Guid operationBy, Guid workspaceId, WorkspaceBlank workspaceBlank);
	Task DeleteWorkspaceAsync(Guid operationBy, Guid workspaceId);

	Task CreateWorkspaceUserAsync(Guid operationBy, WorkspaceUserBlank workspaceBlank);
	Task UpdateWorkspaceUserAsync(Guid operationBy, Guid workspaceUserId, WorkspaceUserBlank workspaceBlank);
	Task DeleteWorkspaceUserAsync(Guid operationBy, Guid workspaceUserId);
}