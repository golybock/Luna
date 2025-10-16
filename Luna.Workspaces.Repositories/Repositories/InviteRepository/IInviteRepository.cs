using Luna.Workspaces.Models.Database.Models;

namespace Luna.Workspaces.Repositories.Repositories.InviteRepository;

public interface IInviteRepository
{
	Task CreateInviteAsync(Guid inviteId, InviteUserDatabase workspaceUserCache);

	Task<InviteUserDatabase?> GetInviteByidAsync(Guid inviteId);

	Task DeleteInviteAsync(Guid inviteId);
}