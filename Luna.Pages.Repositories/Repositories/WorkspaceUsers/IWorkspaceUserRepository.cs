using Luna.Pages.Models.Database.WorkspaceUser;

namespace Luna.Pages.Repositories.Repositories.WorkspaceUsers;

public interface IWorkspaceUserRepository
{
	Task<WorkspaceUserDatabase?> GetWorkspaceUserAsync(Guid workspaceUserId);
	Task<WorkspaceUserDatabase?> GetWorkspaceUserByIdsAsync(Guid workspaceId, Guid userId);
	Task CreateWorkspaceUserAsync(WorkspaceUserDatabase workspaceUserDatabase);
	Task UpdateWorkspaceUserAsync(Guid workspaceUserId, WorkspaceUserDatabase workspaceUserDatabase);
	Task UpdateWorkspaceUserByIdsAsync(Guid workspaceId, Guid userId, WorkspaceUserDatabase workspaceUserDatabase);
	Task<Boolean> DeleteWorkspaceUserAsync(Guid workspaceUserId);
	Task<Boolean> DeleteWorkspaceUserAsync(Guid workspaceId, Guid userId);
	Task<Boolean> DeleteWorkspaceUserByUserIdAsync(Guid userId);
	Task<Boolean> DeleteWorkspaceUserByWorkspaceIdAsync(Guid workspaceId);
}