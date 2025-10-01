using Luna.Workspaces.Models.Blank.Models;
using Luna.Workspaces.Models.Database.Models;
using Luna.Workspaces.Repositories.Repositories.InviteRepository;

namespace Luna.Workspaces.Services.Services.InviteService;

public class InviteService : IInviteService
{
	private readonly IInviteRepository _inviteRepository;

	public InviteService(IInviteRepository inviteRepository)
	{
		_inviteRepository = inviteRepository;
	}

	public async Task<string> CreateInviteAsync(WorkspaceUserBlank workspaceUserBlank, Guid operationBy)
	{
		Guid inviteId = Guid.NewGuid();

		WorkspaceUserCache workspaceUserCache = new WorkspaceUserCache()
		{
			InvitedBy = operationBy,
			Permissions = workspaceUserBlank.Permissions,
			UserId = workspaceUserBlank.UserId,
			WorkspaceId = workspaceUserBlank.WorkspaceId
		};

		await _inviteRepository.CreateInviteAsync(inviteId, workspaceUserCache);

		return inviteId.ToString();
	}

	public async Task DeleteInviteAsync(Guid inviteId)
	{
		await _inviteRepository.DeleteInviteAsync(inviteId);
	}
}