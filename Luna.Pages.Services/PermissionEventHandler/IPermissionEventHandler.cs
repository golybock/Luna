using Luna.Tools.SharedModels.Models.Kafka;

namespace Luna.Pages.Services.PermissionEventHandler;

public interface IPermissionEventHandler
{
	Task HandleAsync(PermissionEvent permissionEvent);
}