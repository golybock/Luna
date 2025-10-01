using Luna.Workspaces.Models.Blank.Models;
using Luna.Workspaces.Models.Database.Models;

namespace Luna.Workspaces.Repositories.Repositories.InviteRepository;

public interface IInviteRepository
{
	Task CreateInviteAsync(Guid inviteId, WorkspaceUserCache workspaceUserCache);

	Task<WorkspaceUserBlank?> GetInviteByidAsync(Guid inviteId);

	Task DeleteInviteAsync(Guid inviteId);
}