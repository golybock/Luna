using Luna.Workspaces.Domain.Models;
using Luna.Workspaces.Models.Blank.Models;
using Luna.Workspaces.Models.Database.Models;
using Luna.Workspaces.Models.View.Models;
using Luna.Workspaces.Repositories.Repositories.InviteRepository;

namespace Luna.Workspaces.Services.Services.InviteService;

public class InviteService : IInviteService
{
	private readonly IInviteRepository _inviteRepository;

	public InviteService(IInviteRepository inviteRepository)
	{
		_inviteRepository = inviteRepository;
	}

	public async Task<InviteUserDomain?> GetInviteByidAsync(Guid inviteId)
	{
		InviteUserDatabase? inviteDatabase = await _inviteRepository.GetInviteByidAsync(inviteId);

		return inviteDatabase != null ? InviteUserDomain.FromDatabase(inviteDatabase) : null;
	}

	public async Task<InviteUserView> CreateInviteAsync(InviteUserBlank inviteUserBlank, Guid operationBy)
	{
		Guid inviteId = Guid.NewGuid();

		InviteUserDomain inviteDomain = InviteUserDomain.FromBlank(inviteUserBlank, operationBy);
		InviteUserDatabase inviteDatabase = inviteDomain.ToDatabase();

		await _inviteRepository.CreateInviteAsync(inviteId, inviteDatabase);

		return new InviteUserView(){InviteId = inviteId.ToString()};
	}

	public async Task DeleteInviteAsync(Guid inviteId)
	{
		await _inviteRepository.DeleteInviteAsync(inviteId);
	}
}