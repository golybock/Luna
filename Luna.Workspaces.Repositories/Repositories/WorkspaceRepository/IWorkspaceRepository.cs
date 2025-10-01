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
	Task<WorkspaceUserDatabase?> GetWorkspaceUserByIdsAsync(Guid workspaceId, Guid userId);
	Task CreateWorkspaceUserAsync(WorkspaceUserDatabase workspaceUserDatabase);
	Task UpdateWorkspaceUserAsync(Guid workspaceUserId, WorkspaceUserDatabase workspaceUserDatabase);
	Task UpdateWorkspaceUserByIdsAsync(Guid workspaceId, Guid userId, WorkspaceUserDatabase workspaceUserDatabase);
	Task DeleteWorkspaceUserAsync(Guid workspaceUserId);
	Task DeleteWorkspaceUserAsync(Guid workspaceId, Guid userId);
	Task DeleteWorkspaceUserByWorkspaceIdAsync(Guid workspaceId);
	Task DeleteWorkspaceUserByUserIdAsync(Guid userId);

	// Транзакционные методы для WorkspaceUser
	Task CreateWorkspaceUserWithTransactionAsync(
		WorkspaceUserDatabase workspaceUserDatabase,
		Func<Task> additionalAction
	);

	Task UpdateWorkspaceUserWithTransactionAsync(
		Guid workspaceUserId,
		WorkspaceUserDatabase workspaceUserDatabase,
		Func<Task> additionalAction
	);

	Task UpdateWorkspaceUserByIdsWithTransactionAsync(
		Guid workspaceId,
		Guid userId,
		WorkspaceUserDatabase workspaceUserDatabase,
		Func<Task> additionalAction
	);

	Task DeleteWorkspaceUserWithTransactionAsync(
		Guid workspaceUserId,
		Func<Task> additionalAction
	);

	Task DeleteWorkspaceUserWithTransactionAsync(
		Guid workspaceId,
		Guid userId,
		Func<Task> additionalAction
	);

	Task CreateWorkspaceWithTransactionAsync(
		WorkspaceDatabase workspaceDatabase,
		Func<Task> additionalAction
	);

	Task DeleteWorkspaceWithTransactionAsync(
		Guid workspaceId,
		Func<Task> additionalAction
	);
}