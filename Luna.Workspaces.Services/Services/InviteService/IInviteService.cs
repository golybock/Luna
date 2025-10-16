using Luna.Workspaces.Domain.Models;
using Luna.Workspaces.Models.Blank.Models;
using Luna.Workspaces.Models.View.Models;

namespace Luna.Workspaces.Services.Services.InviteService;

public interface IInviteService
{
	Task<InviteUserDomain?> GetInviteByidAsync(Guid inviteId);
	Task<InviteUserView> CreateInviteAsync(InviteUserBlank inviteUserDomain, Guid operationBy);

	Task DeleteInviteAsync(Guid inviteId);
}