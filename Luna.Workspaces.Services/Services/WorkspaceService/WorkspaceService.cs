using Luna.Tools.SharedModels.Models;
using Luna.Tools.SharedModels.Models.Exceptions;
using Luna.Users.gRPC.Client.Services;
using Luna.Users.Models.Domain.Models;
using Luna.Users.Models.Extensions.Extensions;
using Luna.Workspaces.Domain.Models;
using Luna.Workspaces.Models.Blank.Models;
using Luna.Workspaces.Models.Database.Models;
using Luna.Workspaces.Models.Extensions.Extensions;
using Luna.Workspaces.Models.View.Models;
using Luna.Workspaces.Repositories.Repositories.InviteRepository;
using Luna.Workspaces.Repositories.Repositories.WorkspaceRepository;
using Luna.Workspaces.Services.Services.InviteService;
using Luna.Workspaces.Services.Services.PermissionEventService;
using Luna.Workspaces.Services.Services.WorkspacePermissionService;

namespace Luna.Workspaces.Services.Services.WorkspaceService;

public class WorkspaceService : IWorkspaceService
{
	private readonly IWorkspaceRepository _workspaceRepository;
	private readonly IInviteService _inviteService;
	private readonly IUserServiceClient _userServiceClient;
	private readonly IPermissionEventService _permissionEventService;
	private readonly IWorkspacePermissionService _workspacePermissionService;

	public WorkspaceService(
		IWorkspaceRepository workspaceRepository,
		IUserServiceClient userServiceClient,
		IPermissionEventService permissionEventService,
		IWorkspacePermissionService workspacePermissionService,
		IInviteService inviteService
	)
	{
		_workspaceRepository = workspaceRepository;
		_userServiceClient = userServiceClient;
		_permissionEventService = permissionEventService;
		_workspacePermissionService = workspacePermissionService;
		_inviteService = inviteService;
	}

	public async Task<WorkspaceView?> GetWorkspaceAsync(Guid workspaceId, Guid operationBy)
	{
		await CheckPermissionAsync(workspaceId, operationBy, WorkspacePermissions.View);

		WorkspaceDatabase? workspaceDatabase = await _workspaceRepository.GetWorkspaceAsync(workspaceId);

		return workspaceDatabase?.ToDomain().ToView();
	}

	public async Task<IEnumerable<WorkspaceView>> GetAvailableWorkspacesAsync(Guid operationBy)
	{
		IEnumerable<WorkspaceDatabase> workspaces =
			await _workspaceRepository.GetUserAvailableWorkspacesAsync(operationBy);

		return workspaces.Select(w => w.ToDomain().ToView());
	}

	public async Task<WorkspaceDetailedView?> GetWorkspaceDetailedViewAsync(Guid workspaceId, Guid operationBy)
	{
		await CheckPermissionAsync(workspaceId, operationBy, WorkspacePermissions.View);

		WorkspaceDatabase? workspaceDatabase = await _workspaceRepository.GetWorkspaceAsync(workspaceId);

		if (workspaceDatabase == null)
		{
			throw new NotFoundException("Workspace not found");
		}

		List<Guid> userIds = workspaceDatabase.WorkspaceUsers.Select(w => w.UserId).ToList();

		IEnumerable<UserDomain> users;

		// попытка загрузить пользователей с другого сервиса
		try
		{
			users = await _userServiceClient.GetUsersByIdsAsync(userIds);
		}
		catch (Exception e)
		{
			users = new List<UserDomain>();
		}

		// собираем полную модель
		List<WorkspaceUserDetailedView> workspaceUsersDetailed = workspaceDatabase.WorkspaceUsers.Select(item =>
		{
			UserDomain? userDomain = users.FirstOrDefault(u => u.Id == item.UserId);
			return item.ToDetailedView(userDomain?.ToView());
		}).ToList();

		return workspaceDatabase.ToDetailedView(workspaceUsersDetailed);
	}

	public async Task<Guid> CreateWorkspaceAsync(WorkspaceBlank workspaceBlank, Guid operationBy)
	{
		WorkspaceDatabase workspaceDatabase = workspaceBlank.ToDatabase();

		workspaceDatabase.Id = Guid.NewGuid();
		workspaceDatabase.OwnerId = operationBy;

		WorkspaceUserDomain workspaceUserDomain = new WorkspaceUserDomain()
		{
			Id = Guid.NewGuid(),
			UserId = operationBy,
			WorkspaceId = workspaceDatabase.Id,
			Permissions = [WorkspacePermissions.Admin]
		};

		await _workspaceRepository.CreateWorkspaceAsync(workspaceDatabase);
		await _workspacePermissionService.AddUserToWorkspaceAsync(workspaceUserDomain);
		// было бы хорошо переделать на транзакцию с таблицей outbox, но пока так
		await _permissionEventService.CreateWorkspaceUserPermissionsAsync(
			workspaceUserDomain.ToWorkspaceUserPermission());

		return workspaceDatabase.Id;
	}

	public async Task UpdateWorkspaceAsync(Guid workspaceId, Guid operationBy, WorkspaceBlank workspaceBlank)
	{
		await CheckPermissionAsync(workspaceId, operationBy, WorkspacePermissions.Edit);

		await _workspaceRepository.UpdateWorkspaceAsync(workspaceId, workspaceBlank.ToDatabase());
	}

	public async Task<WorkspaceView?> GetWorkspaceByInviteAsync(Guid inviteId, string operationByEmail)
	{
		InviteUserDomain? invite = await _inviteService.GetInviteByidAsync(inviteId);

		if (invite == null || invite.Email != operationByEmail)
		{
			throw new NotPermittedException("Invite not found or not available");
		}

		WorkspaceDatabase? workspace = await _workspaceRepository.GetWorkspaceAsync(invite.WorkspaceId);

		return workspace?.ToDomain().ToView();
	}

	public async Task DeleteWorkspaceAsync(Guid workspaceId, Guid operationBy)
	{
		WorkspaceDatabase? workspaceDatabase = await _workspaceRepository.GetWorkspaceAsync(workspaceId);

		if (workspaceDatabase == null)
		{
			throw new Exception("Workspace not found");
		}

		if (operationBy != workspaceDatabase.OwnerId)
		{
			throw new Exception("Only owner can delete workspace");
		}

		await _workspaceRepository.DeleteWorkspaceAsync(workspaceId);
		await _workspacePermissionService.DeleteUserFromWorkspaceByWorkspaceIdAsync(workspaceId);
		await _permissionEventService.DeleteWorkspaceUserPermissionsByWorkspaceId(workspaceId);
	}

	public async Task AcceptInviteAsync(Guid inviteId, string operationByEmail, Guid operationBy)
	{
		InviteUserDomain? invite = await _inviteService.GetInviteByidAsync(inviteId);

		if (invite == null)
		{
			throw new Exception("Invite not found");
		}

		if (invite.Email != operationByEmail)
		{
			throw new Exception("Only invited user can accept invite");
		}

		WorkspaceUserDatabase? userFromDatabase = await _workspaceRepository.GetWorkspaceUserByIdsAsync(invite.WorkspaceId, operationBy);

		if (userFromDatabase != null)
		{
			throw new Exception("You already in this workspace");
		}

		await _workspacePermissionService.AddUserToWorkspaceAsync(invite.ToWorkspaceUserDomain(operationBy));
		await _permissionEventService.CreateWorkspaceUserPermissionsAsync(invite.ToWorkspaceUserPermission(operationBy));
		await _inviteService.DeleteInviteAsync(inviteId);
	}

	public async Task UpdateWorkspaceUserAsync(Guid workspaceUserId, Guid operationBy,
		WorkspaceUserBlank workspaceUserBlank)
	{
		WorkspaceUserDatabase? workspaceUserDatabase =
			await _workspaceRepository.GetWorkspaceUserAsync(workspaceUserId);

		if (workspaceUserDatabase == null)
		{
			throw new Exception("Workspace user not found");
		}

		await CheckPermissionAsync(workspaceUserDatabase.WorkspaceId, operationBy, WorkspacePermissions.Admin);

		WorkspaceUserDomain workspaceUser = WorkspaceUserDomain.FromBlank(workspaceUserBlank);

		await _permissionEventService.UpdateWorkspaceUserPermissions(workspaceUser.ToWorkspaceUserPermission());
		await _workspacePermissionService.UpdateUserWorkspace(workspaceUser.WorkspaceId, workspaceUser.UserId,
			workspaceUser);
	}

	public async Task DeleteWorkspaceUserAsync(Guid workspaceUserId, Guid operationBy)
	{
		WorkspaceUserDatabase? workspaceUserDatabase =
			await _workspaceRepository.GetWorkspaceUserAsync(workspaceUserId);

		if (workspaceUserDatabase == null)
		{
			throw new NotFoundException("Workspace user not found");
		}

		await CheckPermissionAsync(workspaceUserDatabase.WorkspaceId, operationBy, WorkspacePermissions.Admin);

		await _permissionEventService.DeleteWorkspaceUserPermissionsById(workspaceUserDatabase.WorkspaceId,
			workspaceUserDatabase.UserId);
		await _workspacePermissionService.DeleteUserFromWorkspaceAsync(workspaceUserDatabase.WorkspaceId,
			workspaceUserDatabase.UserId);
	}

	private async Task CheckPermissionAsync(Guid workspaceId, Guid userId, string workspacePermission)
	{
		bool available = await _workspacePermissionService.HasPermissionAsync(workspaceId, userId,
			workspacePermission);

		if (!available) throw new NotPermittedException("You do not have permission to this workspace action");
	}
}