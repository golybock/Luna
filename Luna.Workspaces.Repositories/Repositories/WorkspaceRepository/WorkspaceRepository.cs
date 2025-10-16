using Luna.Workspaces.Models.Database.Models;
using Luna.Workspaces.Repositories.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Luna.Workspaces.Repositories.Repositories.WorkspaceRepository;

public class WorkspaceRepository : IWorkspaceRepository
{
	private readonly LunaWorkspacesContext _context;

	public WorkspaceRepository(LunaWorkspacesContext context)
	{
		_context = context;
	}

	public async Task<WorkspaceDatabase?> GetWorkspaceAsync(Guid id)
	{
		return await _context.Workspaces
			.Include(c => c.WorkspaceUsers)
			.FirstOrDefaultAsync(workspace => workspace.Id == id);
	}

	public async Task<IEnumerable<WorkspaceDatabase>> GetUserAvailableWorkspacesAsync(Guid userId)
	{
		return await _context.Workspaces
			.Include(workspace => workspace.WorkspaceUsers)
			.Where(workspace =>
				workspace.OwnerId == userId || workspace.WorkspaceUsers.Any(item => item.UserId == userId))
			.ToListAsync();
	}

	public async Task CreateWorkspaceAsync(WorkspaceDatabase workspaceDatabase)
	{
		await _context.Workspaces.AddAsync(workspaceDatabase);
		await _context.SaveChangesAsync();
	}

	public async Task UpdateWorkspaceAsync(Guid workspaceId, WorkspaceDatabase workspaceDatabase)
	{
		WorkspaceDatabase? workspace = await _context.Workspaces
			.FirstOrDefaultAsync(workspace => workspace.Id == workspaceId);

		if (workspace == null)
		{
			throw new Exception("Workspace not found");
		}

		workspace.Name = workspaceDatabase.Name;
		workspace.Description = workspaceDatabase.Description;
		workspace.OwnerId = workspace.OwnerId;
		workspace.Icon = workspaceDatabase.Icon;
		workspace.DefaultPermission = workspaceDatabase.DefaultPermission;
		workspace.Settings = workspaceDatabase.Settings;
		workspace.DeletedAt = workspaceDatabase.DeletedAt;

		await _context.SaveChangesAsync();
	}

	public async Task DeleteWorkspaceAsync(Guid workspaceId)
	{
		WorkspaceDatabase? workspace = await GetWorkspaceAsync(workspaceId);

		if (workspace == null)
		{
			throw new Exception("Workspace not found");
		}

		_context.Workspaces.Remove(workspace);
		await _context.SaveChangesAsync();
	}

	public async Task<WorkspaceUserDatabase?> GetWorkspaceUserAsync(Guid workspaceUserId)
	{
		return await _context.WorkspaceUsers.FirstOrDefaultAsync(user => user.Id == workspaceUserId);
	}

	public async Task<WorkspaceUserDatabase?> GetWorkspaceUserByIdsAsync(Guid workspaceId, Guid userId)
	{
		return await _context.WorkspaceUsers.FirstOrDefaultAsync(user =>
			user.WorkspaceId == workspaceId && user.UserId == userId);
	}

	public async Task CreateWorkspaceUserAsync(WorkspaceUserDatabase workspaceUserDatabase)
	{
		await _context.WorkspaceUsers.AddAsync(workspaceUserDatabase);
		await _context.SaveChangesAsync();
	}

	public async Task UpdateWorkspaceUserAsync(Guid workspaceUserId, WorkspaceUserDatabase workspaceUserDatabase)
	{
		WorkspaceUserDatabase? workspaceUser =
			await _context.WorkspaceUsers.FirstOrDefaultAsync(workspaceUser => workspaceUser.Id == workspaceUserId);

		if (workspaceUser == null)
		{
			throw new Exception("Workspace user not found");
		}

		workspaceUser.Permissions = workspaceUserDatabase.Permissions;
		workspaceUser.AcceptedAt = workspaceUserDatabase.AcceptedAt;

		_context.WorkspaceUsers.Update(workspaceUser);
		await _context.SaveChangesAsync();
	}

	public async Task UpdateWorkspaceUserByIdsAsync(Guid workspaceId, Guid userId,
		WorkspaceUserDatabase workspaceUserDatabase)
	{
		WorkspaceUserDatabase? workspaceUser = await _context.WorkspaceUsers
			.FirstOrDefaultAsync(workspaceUser =>
				workspaceUser.WorkspaceId == workspaceId &&
				workspaceUser.UserId == userId
			);

		if (workspaceUser == null)
		{
			throw new Exception("Workspace user not found");
		}

		workspaceUser.Permissions = workspaceUserDatabase.Permissions;
		workspaceUser.AcceptedAt = workspaceUserDatabase.AcceptedAt;

		_context.WorkspaceUsers.Update(workspaceUser);
		await _context.SaveChangesAsync();
	}

	public async Task DeleteWorkspaceUserAsync(Guid workspaceUserId)
	{
		WorkspaceUserDatabase? workspaceUser =
			await _context.WorkspaceUsers.FirstOrDefaultAsync(workspaceUser => workspaceUser.Id == workspaceUserId);

		if (workspaceUser == null)
		{
			throw new Exception("Workspace user not found");
		}

		_context.WorkspaceUsers.Remove(workspaceUser);
		await _context.SaveChangesAsync();
	}

	public async Task DeleteWorkspaceUserAsync(Guid workspaceId, Guid userId)
	{
		WorkspaceUserDatabase? workspaceUser = await _context.WorkspaceUsers
			.FirstOrDefaultAsync(workspaceUser =>
				workspaceUser.WorkspaceId == workspaceId &&
				workspaceUser.UserId == userId
			);

		if (workspaceUser == null)
		{
			throw new Exception("Workspace user not found");
		}

		_context.WorkspaceUsers.Remove(workspaceUser);
		await _context.SaveChangesAsync();
	}

	public async Task DeleteWorkspaceUserByWorkspaceIdAsync(Guid workspaceId)
	{
		IEnumerable<WorkspaceUserDatabase> workspaceUsers = await _context.WorkspaceUsers
			.Where(workspaceUser => workspaceUser.WorkspaceId == workspaceId)
			.ToListAsync();

		if (!workspaceUsers.Any())
		{
			throw new Exception("Workspace users not found");
		}

		_context.WorkspaceUsers.RemoveRange(workspaceUsers);
		await _context.SaveChangesAsync();
	}

	public async Task DeleteWorkspaceUserByUserIdAsync(Guid userId)
	{
		IEnumerable<WorkspaceUserDatabase> workspaceUsers = await _context.WorkspaceUsers
			.Where(workspaceUser => workspaceUser.UserId == userId)
			.ToListAsync();

		if (!workspaceUsers.Any())
		{
			throw new Exception("Workspace users not found");
		}

		_context.WorkspaceUsers.RemoveRange(workspaceUsers);
		await _context.SaveChangesAsync();
	}

	#region Транзакционные методы

	public async Task CreateWorkspaceUserWithTransactionAsync(
		WorkspaceUserDatabase workspaceUserDatabase,
		Func<Task> additionalAction
	)
	{
		await ExecuteInTransactionAsync(
			() => CreateWorkspaceUserAsync(workspaceUserDatabase),
			additionalAction
		);
	}

	public async Task UpdateWorkspaceUserWithTransactionAsync(
		Guid workspaceUserId,
		WorkspaceUserDatabase workspaceUserDatabase,
		Func<Task> additionalAction)
	{
		await ExecuteInTransactionAsync(
			() => UpdateWorkspaceUserAsync(workspaceUserId, workspaceUserDatabase),
			additionalAction
		);
	}

	public async Task UpdateWorkspaceUserByIdsWithTransactionAsync(
		Guid workspaceId,
		Guid userId,
		WorkspaceUserDatabase workspaceUserDatabase,
		Func<Task> additionalAction
	)
	{
		await ExecuteInTransactionAsync(
			() => UpdateWorkspaceUserByIdsAsync(workspaceId, userId, workspaceUserDatabase),
			additionalAction
		);
	}

	public async Task DeleteWorkspaceUserWithTransactionAsync(
		Guid workspaceUserId,
		Func<Task> additionalAction
	)
	{
		await ExecuteInTransactionAsync(
			() => DeleteWorkspaceUserAsync(workspaceUserId),
			additionalAction
		);
	}

	public async Task DeleteWorkspaceUserWithTransactionAsync(
		Guid workspaceId,
		Guid userId,
		Func<Task> additionalAction
	)
	{
		await ExecuteInTransactionAsync(
			() => DeleteWorkspaceUserAsync(workspaceId, userId),
			additionalAction
		);
	}

	public async Task CreateWorkspaceWithTransactionAsync(
		WorkspaceDatabase workspaceDatabase,
		Func<Task> additionalAction
	)
	{
		await ExecuteInTransactionAsync(
			() => CreateWorkspaceAsync(workspaceDatabase),
			additionalAction
		);
	}

	public async Task DeleteWorkspaceWithTransactionAsync(
		Guid workspaceId,
		Func<Task> additionalAction
	)
	{
		await ExecuteInTransactionAsync(
			() => DeleteWorkspaceAsync(workspaceId),
			additionalAction
		);
	}

	private async Task ExecuteInTransactionAsync(Func<Task> databaseAction, Func<Task>? additionalAction = null)
	{
		await using IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync();
		try
		{
			await databaseAction();
			if (additionalAction != null) await additionalAction();
			await transaction.CommitAsync();
		}
		catch
		{
			await transaction.RollbackAsync();
			throw;
		}
	}

	#endregion
}