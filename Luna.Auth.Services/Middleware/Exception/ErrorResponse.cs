namespace Luna.Auth.Services.Middleware.Exception;

public record ErrorResponse(string Message, string? Details = null);