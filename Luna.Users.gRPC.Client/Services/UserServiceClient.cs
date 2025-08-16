using Grpc.Net.Client;
using Luna.Users.Models.Blank.Models;
using Luna.Users.Models.Domain.Models;
using Luna.Users.Models.View.Models;
using Microsoft.Extensions.Logging;

namespace Luna.Users.gRPC.Client.Services;

public class UserServiceClient : IUserServiceClient, IDisposable
{
	private readonly GrpcChannel _channel;
	private readonly UserService.UserServiceClient _client;
	private readonly ILogger<UserServiceClient>? _logger;

	public UserServiceClient(string baseUrl, ILogger<UserServiceClient>? logger = null)
	{
		_logger = logger;

		_channel = GrpcChannel.ForAddress(baseUrl);
		_client = new UserService.UserServiceClient(_channel);
	}

	public async Task<UserDomain?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default)
	{
		try
		{
			GetUserByIdRequest request = new GetUserByIdRequest {UserId = userId.ToString()};
			GetUserByIdResponse? response =
				await _client.GetUserByIdAsync(request, cancellationToken: cancellationToken);

			if (response.Error?.Code != null)
			{
				throw new Exception(response.Error.Message);
			}

			return response.User != null ? MapToUserDomain(response.User) : null;
			;
		}
		catch (Exception ex)
		{
			_logger?.LogError(ex, "Error getting user by ID: {UserId}", userId);
			throw;
		}
	}

	public async Task<UserDomain?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default)
	{
		try
		{
			GetUserByUsernameRequest request = new GetUserByUsernameRequest {Username = username};
			GetUserByUsernameResponse? response =
				await _client.GetUserByUsernameAsync(request, cancellationToken: cancellationToken);

			if (response.Error?.Code != null)
			{
				throw new Exception(response.Error.Message);
			}

			return response.User != null ? MapToUserDomain(response.User) : null;
		}
		catch (Exception ex)
		{
			_logger?.LogError(ex, "Error getting user by username: {Username}", username);
			throw;
		}
	}

	public async Task<IEnumerable<UserDomain>> GetUsersByIdsAsync(IEnumerable<Guid> userIds, CancellationToken cancellationToken = default)
	{
		try
		{
			GetUsersByIdsRequest request = new GetUsersByIdsRequest();
			request.UserIds.AddRange(userIds.Select(id => id.ToString()));

			Console.WriteLine(string.Join(" ", request.UserIds));

			GetUsersByIdsResponse? response = await _client.GetUsersByIdsAsync(request, cancellationToken: cancellationToken);

			if (response.Error?.Code != null)
			{
				throw new Exception(response.Error.Message);
			}

			return response.Users.Select(MapToUserDomain).ToList();
		}
		catch (Exception ex)
		{
			_logger?.LogError(ex, "Error getting users by IDs");
			throw;
		}
	}

	public async Task CreateUserAsync(Guid userId, UserBlank? userBlank = null, CancellationToken cancellationToken = default)
	{
		UserBlank user = userBlank ?? new UserBlank();

		try
		{
			CreateUserRequest request = new CreateUserRequest
			{
				User = new UserBlankMessage
				{
					Id = userId.ToString(),
					Username = userBlank?.Username,
					Image = userBlank?.Image,
				}
			};

			CreateUserResponse? response = await _client.CreateUserAsync(request, cancellationToken: cancellationToken);

			if (response.Error?.Code != null)
			{
				throw new Exception(response.Error.Message);
			}
		}
		catch (Exception ex)
		{
			_logger?.LogError(ex, "Error creating user");
			throw;
		}
	}

	public async Task UpdateUserAsync(Guid userId, UserBlank userBlank, CancellationToken cancellationToken = default)
	{
		try
		{
			UpdateUserRequest request = new UpdateUserRequest
			{
				UserId = userId.ToString(),
				User = new UserBlankMessage
				{
					Username = userBlank.Username ?? string.Empty,
					DisplayName = userBlank.DisplayName ?? string.Empty,
					Image = userBlank.Image ?? string.Empty,
					Bio = userBlank.Bio ?? string.Empty
				}
			};

			UpdateUserResponse? response = await _client.UpdateUserAsync(request, cancellationToken: cancellationToken);

			if (response.Error?.Code != null)
			{
				throw new Exception(response.Error.Message);
			}
		}
		catch (Exception ex)
		{
			_logger?.LogError(ex, "Error updating user: {UserId}", userId);
			throw;
		}
	}

	public async Task DeleteUserAsync(Guid userId, CancellationToken cancellationToken = default)
	{
		try
		{
			DeleteUserRequest request = new DeleteUserRequest {UserId = userId.ToString()};
			DeleteUserResponse? response = await _client.DeleteUserAsync(request, cancellationToken: cancellationToken);

			if (response.Error?.Code != null)
			{
				throw new Exception(response.Error.Message);
			}
		}
		catch (Exception ex)
		{
			_logger?.LogError(ex, "Error deleting user: {UserId}", userId);
			throw;
		}
	}

	public async Task<bool> UserExistsAsync(Guid userId, CancellationToken cancellationToken = default)
	{
		try
		{
			UserExistsRequest request = new UserExistsRequest {UserId = userId.ToString()};
			UserExistsResponse? response = await _client.UserExistsAsync(request, cancellationToken: cancellationToken);

			if (response.Error?.Code != null)
			{
				throw new Exception(response.Error.Message);
			}

			return response.Exists;
		}
		catch (Exception ex)
		{
			_logger?.LogError(ex, "Error checking if user exists: {UserId}", userId);
			throw;
		}
	}

	public async Task<bool> UserExistsByUsernameAsync(string username,
		CancellationToken cancellationToken = default)
	{
		try
		{
			UserExistsByUsernameRequest request = new UserExistsByUsernameRequest {Username = username};
			UserExistsByUsernameResponse? response =
				await _client.UserExistsByUsernameAsync(request, cancellationToken: cancellationToken);

			if (response.Error?.Code != null)
			{
				throw new Exception(response.Error.Message);
			}

			return response.Exists;
		}
		catch (Exception ex)
		{
			_logger?.LogError(ex, "Error checking if user exists by username: {Username}", username);
			throw;
		}
	}

	public async Task UpdateLastActiveAsync(Guid userId, CancellationToken cancellationToken = default)
	{
		try
		{
			UpdateLastActiveRequest request = new UpdateLastActiveRequest {UserId = userId.ToString()};
			UpdateLastActiveResponse? response =
				await _client.UpdateLastActiveAsync(request, cancellationToken: cancellationToken);

			if (response.Error?.Code != null)
			{
				throw new Exception(response.Error.Message);
			}
		}
		catch (Exception ex)
		{
			_logger?.LogError(ex, "Error updating last active for user: {UserId}", userId);
			throw;
		}
	}

	private UserView MapToUserView(UserMessage userMessage)
	{
		return new UserView
		{
			Id = Guid.Parse(userMessage.Id),
			Username = userMessage.Username,
			DisplayName = userMessage.DisplayName,
			Image = userMessage.Image,
			Bio = userMessage.Bio,
			LastActive = userMessage.LastActive.ToDateTime()
		};
	}

	private UserDomain MapToUserDomain(UserMessage userMessage)
	{
		return new UserDomain()
		{
			Id = Guid.Parse(userMessage.Id),
			Username = userMessage.Username,
			DisplayName = userMessage.DisplayName,
			Image = userMessage.Image,
			Bio = userMessage.Bio,
			LastActive = userMessage.LastActive.ToDateTime()
		};
	}

	public void Dispose()
	{
		_channel.Dispose();
		GC.SuppressFinalize(this);
	}
}