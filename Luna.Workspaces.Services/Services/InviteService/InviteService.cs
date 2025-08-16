using Luna.Workspaces.Models.Blank.Models;
using Luna.Workspaces.Repositories.Repositories.InviteRepository;

namespace Luna.Workspaces.Services.Services.InviteService;

public class InviteService : IInviteService
{
	private readonly IInviteRepository _inviteRepository;

	public InviteService(IInviteRepository inviteRepository)
	{
		_inviteRepository = inviteRepository;
	}

	// todo добавить хранение operationBy
	public async Task<Guid> CreateInviteAsync(Guid operationBy, WorkspaceUserBlank workspaceUserBlank)
	{
		Guid inviteId = Guid.NewGuid();

		await _inviteRepository.CreateInviteAsync(inviteId, workspaceUserBlank);

		return inviteId;
	}

	public async Task<WorkspaceUserBlank?> GetInviteByidAsync(Guid operationBy, Guid inviteId)
	{
		WorkspaceUserBlank? invite = await _inviteRepository.GetInviteByidAsync(inviteId);

		if (invite == null || invite.UserId != operationBy)
		{
			return null;
		}

		return invite;
	}

	public async Task DeleteInviteAsync(Guid inviteId)
	{
		await _inviteRepository.DeleteInviteAsync(inviteId);
	}
}