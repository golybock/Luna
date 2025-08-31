using Luna.Pages.Models.Database.Models;
using MongoDB.Driver;

namespace Luna.Pages.Repositories.Filters;

public static class Filters
{
	/// <summary>
	/// Фильтр для активных (не удаленных и не архивированных) страниц
	/// </summary>
	public static FilterDefinition<PageDatabase> ActivePage()
	{
		return Builders<PageDatabase>.Filter.And(
			Builders<PageDatabase>.Filter.Eq(p => p.DeletedAt, null),
			Builders<PageDatabase>.Filter.Eq(p => p.ArchivedAt, null)
		);
	}

	/// <summary>
	/// Фильтр для не удаленных страниц (включает архивированные)
	/// </summary>
	public static FilterDefinition<PageDatabase> NonDeletedPage()
	{
		return Builders<PageDatabase>.Filter.Eq(p => p.DeletedAt, null);
	}

	/// <summary>
	/// Фильтр для активных (не удаленных и не архивированных) страниц
	/// </summary>
	public static FilterDefinition<PageDatabase> ActivePageById(string pageId)
	{
		return Builders<PageDatabase>.Filter.And(
			ActivePage(),
			Builders<PageDatabase>.Filter.Eq("_id", pageId)
		);
	}
}