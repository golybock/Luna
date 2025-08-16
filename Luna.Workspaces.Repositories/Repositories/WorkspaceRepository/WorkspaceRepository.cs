using Luna.Workspaces.Models.Database.Models;
using Luna.Workspaces.Repositories.Context;
using Microsoft.EntityFrameworkCore;

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
			.Where(workspace => workspace.OwnerId == userId || workspace.WorkspaceUsers.Any(item => item.UserId == userId))
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
		workspace.Visibility = workspaceDatabase.Visibility;
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

		workspaceUser.Roles = workspaceUserDatabase.Roles;
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
}