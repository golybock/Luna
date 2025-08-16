using Luna.Pages.Models.Database.Models;

namespace Luna.Pages.Models.Database.Additional;

public class PageWithVersion
{
	public PageDatabase Page { get; set; } = null!;
	public PageVersionDatabase VersionDatabase { get; set; } = null!;
}