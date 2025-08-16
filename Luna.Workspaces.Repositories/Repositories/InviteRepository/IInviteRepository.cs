using Luna.Workspaces.Models.Blank.Models;

namespace Luna.Workspaces.Repositories.Repositories.InviteRepository;

public interface IInviteRepository
{
	Task CreateInviteAsync(Guid inviteId, WorkspaceUserBlank workspaceUserBlank);

	Task<WorkspaceUserBlank?> GetInviteByidAsync(Guid inviteId);

	Task DeleteInviteAsync(Guid inviteId);
}