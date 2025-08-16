namespace Luna.Pages.Models.Blank.Models;

public class UpdatePageContentBlank
{
	public IEnumerable<PageBlockBlank> Blocks { get; set; } = new List<PageBlockBlank>();
	public string ChangeDescription { get; set; } = null!;
}