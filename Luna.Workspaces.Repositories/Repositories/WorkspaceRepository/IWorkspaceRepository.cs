using Luna.Workspaces.Models.Database.Models;

namespace Luna.Workspaces.Repositories.Repositories.WorkspaceRepository;

public interface IWorkspaceRepository
{
	Task<WorkspaceDatabase?> GetWorkspaceAsync(Guid id);
	Task<IEnumerable<WorkspaceDatabase>> GetUserAvailableWorkspacesAsync(Guid userId);
	Task CreateWorkspaceAsync(WorkspaceDatabase workspaceDatabase);
	Task UpdateWorkspaceAsync(Guid workspaceId, WorkspaceDatabase workspaceDatabase);
	Task DeleteWorkspaceAsync(Guid workspaceId);

	Task<WorkspaceUserDatabase?> GetWorkspaceUserAsync(Guid workspaceUserId);
	Task CreateWorkspaceUserAsync(WorkspaceUserDatabase workspaceUserDatabase);
	Task UpdateWorkspaceUserAsync(Guid workspaceUserId, WorkspaceUserDatabase workspaceUserDatabase);
	Task DeleteWorkspaceUserAsync(Guid workspaceUserId);
}