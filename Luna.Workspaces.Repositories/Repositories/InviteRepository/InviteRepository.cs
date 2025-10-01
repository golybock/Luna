using System.Text.Json;
using Luna.Workspaces.Models.Blank.Models;
using Luna.Workspaces.Models.Database.Models;
using Microsoft.Extensions.Caching.Distributed;

namespace Luna.Workspaces.Repositories.Repositories.InviteRepository;

public class InviteRepository : IInviteRepository
{
	private readonly IDistributedCache _cache;

	public InviteRepository(IDistributedCache cache)
	{
		_cache = cache;
	}

	public async Task CreateInviteAsync(Guid inviteId, WorkspaceUserCache workspaceUserCache)
	{
		await _cache.SetStringAsync(inviteId.ToString(), JsonSerializer.Serialize(workspaceUserCache));
	}

	public async Task<WorkspaceUserBlank?> GetInviteByidAsync(Guid inviteId)
	{
		string? result = await _cache.GetStringAsync(inviteId.ToString());
		return result != null ? JsonSerializer.Deserialize<WorkspaceUserBlank>(result) : null;
	}

	public async Task DeleteInviteAsync(Guid inviteId)
	{
		await _cache.RemoveAsync(inviteId.ToString());
	}
}