namespace Luna.Pages.Models.View.Additional;

public class LightPageView
{
	public Guid Id { get; set; }
	public string Title { get; set; } = null!;
	public string? Emoji { get; set; }
	public string? Icon { get; set; }
	public string? Cover { get; set; }
	public List<LightPageView> ChildPages { get; set; } =  new List<LightPageView>();
}