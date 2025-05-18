namespace Luna.Auth.Services.Middleware.Exception;

public class BaseServiceException : System.Exception
{
	public BaseServiceException(string message) : base(message) {}
	public BaseServiceException(string message, System.Exception innerException) : base(message, innerException) {}
}

public class UserNotFoundException : BaseServiceException
{
	public UserNotFoundException(string message = "User not found") : base(message) {}
	public UserNotFoundException(Guid userId) : base($"User with id {userId} not found") {}
	public UserNotFoundException(string email, string? message = null)
		: base(message ?? $"User with email {email} not found") {}
}

public class InvalidCredentialsException() : BaseServiceException("Invalid email or password");

public class TokenExpiredException() : BaseServiceException("Token expired");
