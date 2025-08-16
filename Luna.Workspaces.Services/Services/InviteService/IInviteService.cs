using Luna.Workspaces.Models.Blank.Models;

namespace Luna.Workspaces.Services.Services.InviteService;

public interface IInviteService
{
	Task<Guid> CreateInviteAsync(Guid operationBy, WorkspaceUserBlank workspaceUserBlank);

	Task<WorkspaceUserBlank?> GetInviteByidAsync(Guid operationBy, Guid inviteId);

	Task DeleteInviteAsync(Guid inviteId);
}