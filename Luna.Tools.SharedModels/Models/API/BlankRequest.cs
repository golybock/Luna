namespace Luna.Tools.SharedModels.Models.API;

public class BlankRequest<T>
{
	public Guid UserId { get; set; }
	public T Blank { get; set; } = default(T);
}