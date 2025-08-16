namespace Luna.Tools.SharedModels.Models.API;

public class DeleteRequest
{
	public Guid UserId { get; set; }
	public Guid ObjectId { get; set; }
}