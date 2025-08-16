namespace Luna.Pages.Models.Blank.Models;

public class MovePageBlank
{
	public Guid PageId { get; set; }
	public Guid? NewParentId { get; set; }
	public int? NewIndex { get; set; }
}