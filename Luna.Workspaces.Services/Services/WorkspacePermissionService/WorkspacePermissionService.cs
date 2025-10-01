using Luna.Tools.SharedModels.Models;
using Luna.Workspaces.Domain.Models;
using Luna.Workspaces.Models.Database.Models;
using Luna.Workspaces.Repositories.Repositories.WorkspacePermissionRepository;
using Luna.Workspaces.Repositories.Repositories.WorkspaceRepository;
using Microsoft.Extensions.Logging;

namespace Luna.Workspaces.Services.Services.WorkspacePermissionService;

public class WorkspacePermissionService : IWorkspacePermissionService
{
	private readonly IWorkspacePermissionCacheRepository _workspacePermissionCacheRepository;
	private readonly IWorkspaceRepository _workspaceRepository;
	private readonly ILogger<WorkspacePermissionService> _logger;

	public WorkspacePermissionService(
		IWorkspacePermissionCacheRepository permissionCacheRepository,
		IWorkspaceRepository workspaceRepository,
		ILogger<WorkspacePermissionService> logger
	)
	{
		_workspacePermissionCacheRepository = permissionCacheRepository;
		_workspaceRepository = workspaceRepository;
		_logger = logger;
	}

	public async Task<bool> HasPermissionAsync(Guid workspaceId, Guid userId, string requiredPermission)
	{
		try
		{
			WorkspaceUserPermission? userPermission =
				await GetUserPermissionsFromCacheAsync(workspaceId, userId) ??
				await GetUserPermissionFromDatabaseAsync(workspaceId, userId);

			if (userPermission != null) return HasRequiredPermission(userPermission.Permissions, requiredPermission);

			_logger.LogWarning("No permissions found for user {UserId} in workspace {WorkspaceId}", userId,
				workspaceId);

			return false;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error checking permission {Permission} for user {UserId} in workspace {WorkspaceId}",
				requiredPermission, userId, workspaceId);

			return false;
		}
	}

	public async Task<bool> HasAnyPermissionAsync(Guid workspaceId, Guid userId, params string[] requiredPermissions)
	{
		try
		{
			WorkspaceUserPermission? userPermission =
				await GetUserPermissionsFromCacheAsync(workspaceId, userId) ??
				await GetUserPermissionFromDatabaseAsync(workspaceId, userId);

			if (userPermission == null)
				return false;

			return requiredPermissions.Any(permission =>
				HasRequiredPermission(userPermission.Permissions, permission));
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error checking multiple permissions for user {UserId} in workspace {WorkspaceId}",
				userId, workspaceId);

			return false;
		}
	}

	public async Task<bool> HasAllPermissionsAsync(Guid workspaceId, Guid userId, params string[] requiredPermissions)
	{
		try
		{
			WorkspaceUserPermission? userPermission = await GetUserPermissionsAsync(workspaceId, userId);

			if (userPermission == null)
				return false;

			return requiredPermissions.All(permission => HasRequiredPermission(userPermission.Permissions, permission));
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error checking all permissions for user {UserId} in workspace {WorkspaceId}", userId,
				workspaceId);
			return false;
		}
	}

	public async Task<WorkspaceUserPermission?> GetUserPermissionsFromCacheAsync(Guid workspaceId, Guid userId)
	{
		try
		{
			return await _workspacePermissionCacheRepository.GetUserPermissionAsync(workspaceId, userId);
		}
		catch (Exception)
		{
			_logger.LogError("Error get user permission from cache, userId: {UserId}, workspaceId: {WorkspaceId}", userId, workspaceId);
			throw;
		}
	}

	public async Task AddUserToWorkspaceAsync(WorkspaceUserDomain workspaceUser)
	{
		WorkspaceUserDatabase workspaceUserDatabase = workspaceUser.ToDatabase();
		workspaceUserDatabase.Id = Guid.NewGuid();

		try
		{
			await _workspaceRepository.CreateWorkspaceUserAsync(workspaceUserDatabase);

			// Если не получилось записать в кэш, пишем в логи ошибку, доставать из бд все еще допустимо
			try
			{
				await SetUserPermissionsInCacheAsync(workspaceUser);
			}
			catch (Exception)
			{
				_logger.LogError("Error set workspaceUser in cache, userId:{UserId}, workspaceId: {WorkspaceId}",
					workspaceUser.UserId, workspaceUser.WorkspaceId);
			}
		}
		catch (Exception)
		{
			_logger.LogError("Error create workspaceUser in database, userId:{UserId}, workspaceId: {WorkspaceId}",
				workspaceUser.UserId, workspaceUser.WorkspaceId);
			throw;
		}

		_logger.LogInformation(
			"User added to workspace, userId: {UserId}, workspaceId: {WorkspaceId}, permissions: {Permissions}",
			workspaceUser.UserId, workspaceUser.WorkspaceId, string.Join("|", workspaceUser.Permissions));
	}

	public async Task UpdateUserWorkspace(Guid workspaceId, Guid userId, WorkspaceUserDomain workspaceUser)
	{
		try
		{
			await _workspaceRepository.UpdateWorkspaceUserByIdsAsync(workspaceId, userId, workspaceUser.ToDatabase());

			// Если не получилось записать в кэш, пишем в логи ошибку, доставать из бд все еще допустимо
			try
			{
				await SetUserPermissionsInCacheAsync(workspaceUser);
			}
			catch (Exception)
			{
				_logger.LogError("Error update workspaceUser in cache, userId:{UserId}, workspaceId: {WorkspaceId}",
					workspaceUser.UserId, workspaceUser.WorkspaceId);
			}
		}
		catch (Exception)
		{
			_logger.LogError("Error update workspaceUser in database, userId:{UserId}, workspaceId: {WorkspaceId}",
				workspaceUser.UserId, workspaceUser.WorkspaceId);
			throw;
		}

		_logger.LogInformation(
			"User updated in workspace, userId: {UserId}, workspaceId: {WorkspaceId}, permissions: {Permissions}",
			workspaceUser.UserId, workspaceUser.WorkspaceId, string.Join("|", workspaceUser.Permissions));
	}

	public async Task DeleteUserFromWorkspaceAsync(Guid workspaceId, Guid userId)
	{
		WorkspaceUserDomain? workspaceUserDomain = await GetWorkspaceUserFromDatabaseAsync(workspaceId, userId);

		if (workspaceUserDomain == null) return;

		try
		{
			await _workspaceRepository.DeleteWorkspaceUserAsync(workspaceId, userId);

			try
			{
				// удаляем из кэша
				await _workspacePermissionCacheRepository.DeleteUserPermissionAsync(workspaceId, userId);
			}
			catch (Exception)
			{
				// если из кэша не было удалено, откатываем удаление из бд
				await _workspaceRepository.CreateWorkspaceUserAsync(workspaceUserDomain.ToDatabase());
				throw;
			}
		}
		catch (Exception)
		{
			_logger.LogInformation("Error remove User {UserId} from workspace {WorkspaceId} in Cache", userId,
				workspaceId);
			throw;
		}

		_logger.LogInformation("User {UserId} removed from workspace {WorkspaceId}", userId, workspaceId);
	}

	public async Task DeleteUserFromWorkspaceByWorkspaceIdAsync(Guid workspaceId)
	{
		try
		{
			await _workspaceRepository.DeleteWorkspaceUserByWorkspaceIdAsync(workspaceId);
			await _workspacePermissionCacheRepository.DeleteUserPermissionByWorkspaceIdAsync(workspaceId);
		}
		catch (Exception)
		{
			_logger.LogInformation("Error remove workspace users: {WorkspaceId}", workspaceId);
			throw;
		}

		_logger.LogInformation("Removed users from workspace: {WorkspaceId}", workspaceId);
	}

	public async Task DeleteUserFromWorkspaceByUserIdAsync(Guid userId)
	{
		try
		{
			await _workspaceRepository.DeleteWorkspaceUserByUserIdAsync(userId);
			await _workspacePermissionCacheRepository.DeleteUserPermissionByUserIdAsync(userId);
		}
		catch (Exception)
		{
			_logger.LogInformation("Error remove userId from workspaces: {UserId}", userId);
			throw;
		}

		_logger.LogInformation("Removed userId from workspaces {UserId}", userId);
	}

	#region Private methods with database

	private async Task<WorkspaceUserPermission?> GetUserPermissionsAsync(Guid workspaceId, Guid userId)
	{
		WorkspaceUserPermission? workspaceUserPermissionFromDatabase = null;
		WorkspaceUserPermission? userPermissionFromCache = null;

		userPermissionFromCache = await GetUserPermissionsFromCacheAsync(workspaceId, userId);

		if (userPermissionFromCache == null)
		{
			workspaceUserPermissionFromDatabase = await GetUserPermissionFromDatabaseAsync(workspaceId, userId);

			if (workspaceUserPermissionFromDatabase != null)
			{
				// если нет в кэше, но есть в бд, устанавливаем значение из бд
				await _workspacePermissionCacheRepository.SetUserPermissionAsync(workspaceUserPermissionFromDatabase);
			}
		}

		return userPermissionFromCache ?? workspaceUserPermissionFromDatabase;
	}

	private async Task<WorkspaceUserDomain?> GetWorkspaceUserFromDatabaseAsync(Guid workspaceId, Guid userId)
	{
		WorkspaceUserDatabase? workspaceUser =
			await _workspaceRepository.GetWorkspaceUserByIdsAsync(workspaceId, userId);

		return workspaceUser == null ? null : WorkspaceUserDomain.FromDatabase(workspaceUser);
	}

	private async Task<WorkspaceUserPermission?> GetUserPermissionFromDatabaseAsync(Guid workspaceId, Guid userId)
	{
		WorkspaceUserDatabase? workspaceUser =
			await _workspaceRepository.GetWorkspaceUserByIdsAsync(workspaceId, userId);

		return workspaceUser == null
			? null
			: WorkspaceUserDomain.FromDatabase(workspaceUser).ToWorkspaceUserPermission();
	}

	private async Task SetUserPermissionsInCacheAsync(WorkspaceUserDomain workspaceUserDomain)
	{
		string[] validatedPermissions = ValidatePermissions(workspaceUserDomain.Permissions);

		WorkspaceUserPermission userPermission = new WorkspaceUserPermission
		{
			UserId = workspaceUserDomain.UserId,
			WorkspaceId = workspaceUserDomain.WorkspaceId,
			Permissions = validatedPermissions
		};

		await _workspacePermissionCacheRepository.SetUserPermissionAsync(userPermission);

		_logger.LogInformation(
			"Updated permissions for user {UserId} in workspace {WorkspaceId}: {Permissions} in cache",
			workspaceUserDomain.UserId, workspaceUserDomain.WorkspaceId, string.Join(", ", validatedPermissions));
	}

	#endregion

	#region Private methods

	private static bool HasRequiredPermission(string[] userPermissions, string requiredPermission)
	{
		// Проверяем, есть ли у пользователя требуемое разрешение напрямую
		if (userPermissions.Contains(requiredPermission))
			return true;

		// Проверяем иерархию разрешений - есть ли у пользователя разрешение более высокого уровня
		foreach (string userPermission in userPermissions)
		{
			if (WorkspacePermissions.PermissionHierarchy.TryGetValue(userPermission,
				    out string[]? hierarchyPermissions) &&
			    hierarchyPermissions.Contains(requiredPermission))
			{
				return true;
			}
		}

		return false;
	}

	private static string[] ValidatePermissions(string[] permissions)
	{
		// Фильтруем только валидные разрешения
		string[] validPermissions = permissions
			.Where(p => WorkspacePermissions.AllPermissions.Contains(p))
			.Distinct()
			.ToArray();

		if (validPermissions.Length == permissions.Length) return validPermissions;
		IEnumerable<string> invalidPermissions = permissions.Except(validPermissions);
		throw new ArgumentException($"Invalid permissions: {string.Join(", ", invalidPermissions)}");
	}

	#endregion
}