namespace Luna.Tools.SharedModels.Models.Exceptions;

public class NotPermittedException : Exception
{
	public NotPermittedException(string message) : base(message)
	{
	}

	public NotPermittedException() : base() {}
}