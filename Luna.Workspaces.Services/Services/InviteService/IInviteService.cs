using Luna.Workspaces.Models.Blank.Models;

namespace Luna.Workspaces.Services.Services.InviteService;

public interface IInviteService
{
	Task<string> CreateInviteAsync(WorkspaceUserBlank workspaceUserBlank, Guid operationBy);

	Task DeleteInviteAsync(Guid inviteId);
}