using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Luna.Users.Models.Blank.Models;
using Luna.Users.Models.View.Models;
using Luna.Users.Services.Services.User;

namespace Luna.Users.gRPC.Services;

public class UserGrpcService : UserService.UserServiceBase
{
	private readonly IUserService _userService;
	private readonly ILogger<UserGrpcService> _logger;

	public UserGrpcService(IUserService userService, ILogger<UserGrpcService> logger)
	{
		_userService = userService;
		_logger = logger;
	}

	public override async Task<GetUserByIdResponse> GetUserById(GetUserByIdRequest request, ServerCallContext context)
	{
		GetUserByIdResponse response = new GetUserByIdResponse();

		try
		{
			if (string.IsNullOrEmpty(request.UserId))
			{
				response.Error = CreateErrorMessage("INVALID_INPUT", "User ID is required");
				return response;
			}

			if (!Guid.TryParse(request.UserId, out var userId))
			{
				response.Error = CreateErrorMessage("INVALID_INPUT", "Invalid User ID format");
				return response;
			}

			UserView? user = await _userService.GetUserByIdAsync(userId);

			if (user != null)
			{
				response.User = MapToUserMessage(user);
			}

			return response;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error getting user by ID: {UserId}", request.UserId);
			response.Error = CreateErrorMessage("INTERNAL_ERROR", "Internal server error", ex.Message);
			return response;
		}
	}

	public override async Task<GetUserByUsernameResponse> GetUserByUsername(GetUserByUsernameRequest request, ServerCallContext context)
	{
		GetUserByUsernameResponse response = new GetUserByUsernameResponse();

		try
		{
			if (string.IsNullOrEmpty(request.Username))
			{
				response.Error = CreateErrorMessage("INVALID_INPUT", "Username is required");
				return response;
			}

			UserView? user = await _userService.GetUserByUsernameAsync(request.Username);

			if (user != null)
			{
				response.User = MapToUserMessage(user);
			}

			return response;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error getting user by username: {Username}", request.Username);
			response.Error = CreateErrorMessage("INTERNAL_ERROR", "Internal server error", ex.Message);
			return response;
		}
	}

	public override async Task<GetUsersByIdsResponse> GetUsersByIds(GetUsersByIdsRequest request, ServerCallContext context)
	{
		GetUsersByIdsResponse response = new GetUsersByIdsResponse();

		try
		{
			if (!request.UserIds.Any())
			{
				response.Error = CreateErrorMessage("INVALID_INPUT", "At least one User ID is required");
				return response;
			}

			List<Guid> userIds = new List<Guid>();
			foreach (var userIdString in request.UserIds)
			{
				if (!Guid.TryParse(userIdString, out var userId))
				{
					response.Error = CreateErrorMessage("INVALID_INPUT", $"Invalid User ID format: {userIdString}");
					return response;
				}

				userIds.Add(userId);
			}

			IEnumerable<UserView> users = await _userService.GetUsersByIdsAsync(userIds);

			foreach (UserView user in users)
			{
				response.Users.Add(MapToUserMessage(user));
			}

			return response;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error getting users by IDs");
			response.Error = CreateErrorMessage("INTERNAL_ERROR", "Internal server error", ex.Message);
			return response;
		}
	}

	public override async Task<CreateUserResponse> CreateUser(CreateUserRequest request, ServerCallContext context)
	{
		CreateUserResponse response = new CreateUserResponse();

		try
		{
			if (request.User != null)
			{
				UserBlank userBlank = MapToUserBlank(request.User);
				Guid userId = Guid.Parse(request.User.Id);
				bool result = await _userService.CreateUserAsync(userId, userBlank);
				response.Success = result;
			}

			return response;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error creating user");

			if (ex.Message.Contains("already exists"))
			{
				response.Error = CreateErrorMessage("USER_EXISTS", "User already exists", ex.Message);
			}
			else if (ex.Message.Contains("Username is null"))
			{
				response.Error = CreateErrorMessage("INVALID_INPUT", "Username is required", ex.Message);
			}
			else
			{
				response.Error = CreateErrorMessage("INTERNAL_ERROR", "Internal server error", ex.Message);
			}

			return response;
		}
	}

	public override async Task<UpdateUserResponse> UpdateUser(UpdateUserRequest request, ServerCallContext context)
	{
		UpdateUserResponse response = new UpdateUserResponse();

		try
		{
			if (string.IsNullOrEmpty(request.UserId))
			{
				response.Error = CreateErrorMessage("INVALID_INPUT", "User ID is required");
				return response;
			}

			if (!Guid.TryParse(request.UserId, out var userId))
			{
				response.Error = CreateErrorMessage("INVALID_INPUT", "Invalid User ID format");
				return response;
			}

			if (request.User == null)
			{
				response.Error = CreateErrorMessage("INVALID_INPUT", "User data is required");
				return response;
			}

			UserBlank userBlank = MapToUserBlank(request.User);
			bool result = await _userService.UpdateUserAsync(userId, userBlank);

			response.Success = result;

			return response;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error updating user: {UserId}", request.UserId);

			response.Error = ex.Message.Contains("not found")
				? CreateErrorMessage("USER_NOT_FOUND", "User not found", ex.Message)
				: CreateErrorMessage("INTERNAL_ERROR", "Internal server error", ex.Message);

			return response;
		}
	}

	public override async Task<DeleteUserResponse> DeleteUser(DeleteUserRequest request, ServerCallContext context)
	{
		DeleteUserResponse response = new DeleteUserResponse();

		try
		{
			if (string.IsNullOrEmpty(request.UserId))
			{
				response.Error = CreateErrorMessage("INVALID_INPUT", "User ID is required");
				return response;
			}

			if (!Guid.TryParse(request.UserId, out var userId))
			{
				response.Error = CreateErrorMessage("INVALID_INPUT", "Invalid User ID format");
				return response;
			}

			bool result = await _userService.DeleteUserAsync(userId);
			response.Success = result;

			return response;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error deleting user: {UserId}", request.UserId);
			response.Error = CreateErrorMessage("INTERNAL_ERROR", "Internal server error", ex.Message);
			return response;
		}
	}

	public override async Task<UserExistsResponse> UserExists(UserExistsRequest request, ServerCallContext context)
	{
		UserExistsResponse response = new UserExistsResponse();

		try
		{
			if (string.IsNullOrEmpty(request.UserId))
			{
				response.Error = CreateErrorMessage("INVALID_INPUT", "User ID is required");
				return response;
			}

			if (!Guid.TryParse(request.UserId, out var userId))
			{
				response.Error = CreateErrorMessage("INVALID_INPUT", "Invalid User ID format");
				return response;
			}

			bool exists = await _userService.UserExistsAsync(userId);
			response.Exists = exists;

			return response;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error checking if user exists: {UserId}", request.UserId);
			response.Error = CreateErrorMessage("INTERNAL_ERROR", "Internal server error", ex.Message);
			return response;
		}
	}

	public override async Task<UserExistsByUsernameResponse> UserExistsByUsername(UserExistsByUsernameRequest request, ServerCallContext context)
	{
		UserExistsByUsernameResponse response = new UserExistsByUsernameResponse();

		try
		{
			if (string.IsNullOrEmpty(request.Username))
			{
				response.Error = CreateErrorMessage("INVALID_INPUT", "Username is required");
				return response;
			}

			bool exists = await _userService.UserExistsByUsernameAsync(request.Username);
			response.Exists = exists;

			return response;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error checking if user exists by username: {Username}", request.Username);
			response.Error = CreateErrorMessage("INTERNAL_ERROR", "Internal server error", ex.Message);
			return response;
		}
	}

	public override async Task<UpdateLastActiveResponse> UpdateLastActive(UpdateLastActiveRequest request, ServerCallContext context)
	{
		UpdateLastActiveResponse response = new UpdateLastActiveResponse();

		try
		{
			if (string.IsNullOrEmpty(request.UserId))
			{
				response.Error = CreateErrorMessage("INVALID_INPUT", "User ID is required");
				return response;
			}

			if (!Guid.TryParse(request.UserId, out var userId))
			{
				response.Error = CreateErrorMessage("INVALID_INPUT", "Invalid User ID format");
				return response;
			}

			bool result = await _userService.UpdateLastActiveAsync(userId);
			response.Success = result;

			return response;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error updating last active for user: {UserId}", request.UserId);
			response.Error = CreateErrorMessage("INTERNAL_ERROR", "Internal server error", ex.Message);
			return response;
		}
	}

	private static ErrorMessage CreateErrorMessage(string code, string message, string? details = null)
	{
		return new ErrorMessage
		{
			Code = code,
			Message = message,
			Details = details ?? string.Empty
		};
	}

	private static UserMessage MapToUserMessage(UserView user)
	{
		return new UserMessage
		{
			Id = user.Id.ToString(),
			Username = user.Username,
			Bio = user.Bio,
			DisplayName = user.DisplayName,
			Image = user.Image,
			LastActive = Timestamp.FromDateTime(user.LastActive)
		};
	}

	private static UserBlank MapToUserBlank(UserBlankMessage userMessage)
	{
		return new UserBlank
		{
			Username = userMessage.Username,
			Bio = userMessage.Bio,
			DisplayName = userMessage.DisplayName,
			Image = userMessage.Image,
		};
	}
}