namespace Luna.Tools.SharedModels.Models.API;

public class UpdateRequest<T>
{
	public Guid UserId { get; set; }
	public Guid ObjectId { get; set; }
	public T Blank { get; set; } = default(T);
}