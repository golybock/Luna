using Luna.Pages.Models.Database.WorkspaceUser;
using Luna.Pages.Repositories.Context;
using Microsoft.EntityFrameworkCore;

namespace Luna.Pages.Repositories.Repositories.WorkspaceUsers;

public class WorkspaceUserRepository : IWorkspaceUserRepository
{
	private readonly LunaPagesContext _context;

	public WorkspaceUserRepository(LunaPagesContext context)
	{
		_context = context;
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

		_context.WorkspaceUsers.Update(workspaceUser);
		await _context.SaveChangesAsync();
	}

	public async Task UpdateWorkspaceUserByIdsAsync(Guid workspaceId, Guid userId,
		WorkspaceUserDatabase workspaceUserDatabase)
	{
		WorkspaceUserDatabase? workspaceUser = await _context
			.WorkspaceUsers
			.FirstOrDefaultAsync(workspaceUser =>
				workspaceUser.WorkspaceId == workspaceId &&
				workspaceUser.UserId == userId
			);

		if (workspaceUser == null)
		{
			throw new Exception("Workspace user not found");
		}

		workspaceUser.Permissions = workspaceUserDatabase.Permissions;

		_context.WorkspaceUsers.Update(workspaceUser);
		await _context.SaveChangesAsync();
	}

	public async Task<Boolean> DeleteWorkspaceUserAsync(Guid workspaceUserId)
	{
		WorkspaceUserDatabase? workspaceUser =
			await _context.WorkspaceUsers.FirstOrDefaultAsync(workspaceUser => workspaceUser.Id == workspaceUserId);

		if (workspaceUser == null)
		{
			return false;
		}

		_context.WorkspaceUsers.Remove(workspaceUser);
		return await _context.SaveChangesAsync() > 0;
	}

	public async Task<bool> DeleteWorkspaceUserAsync(Guid workspaceId, Guid userId)
	{
		WorkspaceUserDatabase? workspaceUser = await _context
			.WorkspaceUsers
			.FirstOrDefaultAsync(workspaceUser =>
				workspaceUser.WorkspaceId == workspaceId &&
				workspaceUser.UserId == userId
			);

		if (workspaceUser == null)
		{
			return false;
		}

		_context.WorkspaceUsers.Remove(workspaceUser);
		return await _context.SaveChangesAsync() > 0;
	}

	public async Task<bool> DeleteWorkspaceUserByUserIdAsync(Guid userId)
	{
		IEnumerable<WorkspaceUserDatabase> workspaceUsers = await _context.WorkspaceUsers
			.Where(workspaceUser => workspaceUser.UserId == userId)
			.ToListAsync();

		if (!workspaceUsers.Any())
		{
			return false;
		}

		_context.WorkspaceUsers.RemoveRange(workspaceUsers);
		return await _context.SaveChangesAsync() > 0;
	}

	public async Task<bool> DeleteWorkspaceUserByWorkspaceIdAsync(Guid workspaceId)
	{
		IEnumerable<WorkspaceUserDatabase> workspaceUsers = await _context.WorkspaceUsers
			.Where(workspaceUser => workspaceUser.WorkspaceId == workspaceId)
			.ToListAsync();

		if (!workspaceUsers.Any())
		{
			return false;
		}

		_context.WorkspaceUsers.RemoveRange(workspaceUsers);
		return await _context.SaveChangesAsync() > 0;
	}
}