using Luna.Users.gRPC.Client.Services;
using Luna.Users.Models.Domain.Models;
using Luna.Users.Models.Extensions.Extensions;
using Luna.Workspaces.Domain.Models;
using Luna.Workspaces.Models.Blank.Models;
using Luna.Workspaces.Models.Database.Models;
using Luna.Workspaces.Models.Extensions.Extensions;
using Luna.Workspaces.Models.View.Models;
using Luna.Workspaces.Repositories.Repositories.WorkspaceRepository;

namespace Luna.Workspaces.Services.Services.WorkspaceService;

public class WorkspaceService : IWorkspaceService
{
	private readonly IWorkspaceRepository _repository;
	private readonly IUserServiceClient _userServiceClient;

	public WorkspaceService(IWorkspaceRepository repository, IUserServiceClient userServiceClient)
	{
		_repository = repository;
		_userServiceClient = userServiceClient;
	}

	public async Task<WorkspaceView?> GetWorkspaceAsync(Guid operationBy, Guid id)
	{
		WorkspaceDatabase? workspaceDatabase = await _repository.GetWorkspaceAsync(id);

		if (workspaceDatabase == null || workspaceDatabase.OwnerId != operationBy)
		{
			return null;
		}

		return workspaceDatabase.ToDomain().ToView();
	}

	public async Task<IEnumerable<WorkspaceView>> GetUserAvailableWorkspacesAsync(Guid userId)
	{
		IEnumerable<WorkspaceDatabase> workspaces = await _repository.GetUserAvailableWorkspacesAsync(userId);

		return workspaces.Select(w => w.ToDomain().ToView());
	}

	public async Task<WorkspaceDetailedView?> GetWorkspaceDetailedViewAsync(Guid operationBy, Guid id)
	{
		WorkspaceDatabase? workspaceDatabase = await _repository.GetWorkspaceAsync(id);

		if (workspaceDatabase == null || workspaceDatabase.OwnerId != operationBy)
		{
			return null;
		}

		List<Guid> userIds = workspaceDatabase.WorkspaceUsers.Select(w => w.UserId).ToList();
		userIds.Add(workspaceDatabase.OwnerId);

		IEnumerable<UserDomain> users;

		try
		{
			users = await _userServiceClient.GetUsersByIdsAsync(userIds);
		}
		catch (Exception e)
		{
			users = new List<UserDomain>();
			Console.WriteLine(e);
		}

		List<WorkspaceUserDetailedView> workspaceUsersDetailed = workspaceDatabase.WorkspaceUsers.Select(item =>
		{
			return item.ToDetailedView(
				users.FirstOrDefault(u => u.Id == item.UserId)?.ToView()
			);
		}).ToList();

		workspaceUsersDetailed.Add(
			new WorkspaceUserDetailedView()
			{
				UserId = workspaceDatabase.OwnerId,
				User = users.FirstOrDefault(u => u.Id == workspaceDatabase.OwnerId)?.ToView(),
				Permissions = ["all"],
				Roles = ["admin"],
				WorkspaceId = workspaceDatabase.Id,
			}
		);

		return workspaceDatabase.ToDetailedView(workspaceUsersDetailed);
	}

	public async Task<IEnumerable<WorkspaceDetailedView>> GetUserAvailableWorkspacesDetailedViewAsync(Guid userId)
	{
		IEnumerable<WorkspaceDatabase> workspaces = await _repository.GetUserAvailableWorkspacesAsync(userId);

		List<Guid> userIds = workspaces.SelectMany(c => c.WorkspaceUsers).Select(c => c.Id).ToList();
		List<Guid> ownerIds = workspaces.Select(c => c.OwnerId).ToList();

		userIds.AddRange(ownerIds);
		userIds = userIds.Distinct().ToList();

		IEnumerable<UserDomain> users;

		try
		{
			users = await _userServiceClient.GetUsersByIdsAsync(userIds);
		}
		catch (Exception e)
		{
			users = new List<UserDomain>();
			Console.WriteLine(e);
		}

		List<WorkspaceDetailedView> workspaceDetailedViews = new List<WorkspaceDetailedView>();

		foreach (WorkspaceDatabase workspace in workspaces)
		{
			List<WorkspaceUserDetailedView> workspaceUserDetailedViews = new List<WorkspaceUserDetailedView>();

			foreach (WorkspaceUserDatabase workspaceUser in workspace.WorkspaceUsers)
			{
				foreach (UserDomain userDomain in users)
				{
					if (userDomain.Id == workspaceUser.UserId)
					{
						workspaceUserDetailedViews.Add(workspaceUser.ToDetailedView(userDomain.ToView()));
					}
				}
			}

			workspaceDetailedViews.Add(workspace.ToDetailedView(workspaceUserDetailedViews));
		}

		return workspaceDetailedViews;
	}

	public async Task<Guid> CreateWorkspaceAsync(Guid userId, WorkspaceBlank workspaceBlank)
	{
		WorkspaceDatabase workspaceDatabase = workspaceBlank.ToDatabase();

		workspaceDatabase.Id = Guid.NewGuid();
		workspaceDatabase.OwnerId = userId;

		await _repository.CreateWorkspaceAsync(workspaceDatabase);

		return workspaceDatabase.Id;
	}

	public async Task UpdateWorkspaceAsync(Guid userId, Guid workspaceId, WorkspaceBlank workspaceBlank)
	{
		await _repository.UpdateWorkspaceAsync(workspaceId, workspaceBlank.ToDatabase());
	}

	public async Task DeleteWorkspaceAsync(Guid userId, Guid workspaceId)
	{
		WorkspaceDatabase? workspaceDatabase = await _repository.GetWorkspaceAsync(workspaceId);

		if (workspaceDatabase == null)
		{
			throw new Exception("Workspace not found");
		}

		if (userId != workspaceDatabase.OwnerId)
		{
			throw new Exception("Operation not permitted");
		}

		await _repository.DeleteWorkspaceAsync(workspaceId);
	}


	public async Task CreateWorkspaceUserAsync(Guid userId, WorkspaceUserBlank workspaceUserBlank)
	{
		WorkspaceUserDomain workspaceUserDomain = workspaceUserBlank.ToDomain();

		workspaceUserDomain.UserId = userId;

		await _repository.CreateWorkspaceUserAsync(workspaceUserDomain.ToDatabase());
	}

	public async Task UpdateWorkspaceUserAsync(Guid userId, Guid workspaceUserId,
		WorkspaceUserBlank workspaceUserDatabase)
	{
		WorkspaceUserDatabase? workspaceUser = await _repository.GetWorkspaceUserAsync(workspaceUserId);

		if (workspaceUser == null)
		{
			throw new Exception("Workspace user not found");
		}

		WorkspaceDatabase? workspace = await _repository.GetWorkspaceAsync(workspaceUser.WorkspaceId);

		if (workspace == null)
		{
			throw new Exception("Workspace not found");
		}

		if (workspace.OwnerId != userId)
		{
			throw new Exception("Operation not permitted");
		}

		await _repository.UpdateWorkspaceUserAsync(workspaceUserId, workspaceUserDatabase.ToDomain().ToDatabase());
	}

	public async Task DeleteWorkspaceUserAsync(Guid userId, Guid workspaceUserId)
	{
		WorkspaceUserDatabase? workspaceUserDatabase = await _repository.GetWorkspaceUserAsync(workspaceUserId);

		if (workspaceUserDatabase == null)
		{
			throw new Exception("WorkspaceUserDatabase not found");
		}

		WorkspaceDatabase? workspace = await _repository.GetWorkspaceAsync(workspaceUserDatabase.WorkspaceId);

		if (workspace == null)
		{
			throw new Exception("Inner exception");
		}

		if (workspace.OwnerId != userId || workspaceUserDatabase.InvitedBy != userId)
		{
			throw new Exception("Operation not permitted");
		}

		await _repository.DeleteWorkspaceUserAsync(workspaceUserId);
	}
}