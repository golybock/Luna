using Luna.Pages.Models.View.Additional;

namespace Luna.Pages.Models.Database.Additional;

public class PageStatistics
{
	public int TotalPages { get; set; }
	public int ArchivedPages { get; set; }
	public int DeletedPages { get; set; }
	public int PinnedPages { get; set; }
	public int Templates { get; set; }

	public PageStatisticView ToView()
	{
		return new PageStatisticView()
		{
			TotalPages = TotalPages,
			ArchivedPages = ArchivedPages,
			DeletedPages = DeletedPages,
			PinnedPages = PinnedPages,
			Templates = Templates
		};
	}
}