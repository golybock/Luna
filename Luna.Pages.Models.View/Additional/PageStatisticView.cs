namespace Luna.Pages.Models.View.Additional;

public class PageStatisticView
{
	public int TotalPages { get; set; }
	public int ActivePages { get; set; }
	public int ArchivedPages { get; set; }
	public int DeletedPages { get; set; }
	public int PinnedPages { get; set; }
	public int Templates { get; set; }
}