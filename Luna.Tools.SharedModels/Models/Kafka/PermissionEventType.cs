namespace Luna.Tools.SharedModels.Models.Kafka;

public enum PermissionEventType
{
	Created,
	Updated,
	DeletedById,
	DeletedByWorkspaceId,
	DeletedByUserId
}