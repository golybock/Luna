using Luna.Pages.Models.Database.Models;
using Luna.Pages.Models.View.Models;

namespace Luna.Pages.Models.Domain.Models;

public class PageBlockDomain
{
	public Guid Id { get; set; }
	public Guid PageId { get; set; }
	public string Type { get; set; } = null!;
	public object? Content { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime UpdatedAt { get; set; }
	public Guid CreatedBy { get; set; }
	public Guid UpdatedBy { get; set; }
	public Guid? ParentId { get; set; }
	public int Index { get; set; }
	public object? Properties { get; set; }

	public static PageBlockDomain FromDatabase(PageBlockDatabase block)
	{
		return new PageBlockDomain()
		{
			Id = block.Id,
			PageId = block.PageId,
			Type = block.Type,
			Content = block.Content,
			CreatedAt = block.CreatedAt,
			UpdatedAt = block.UpdatedAt,
			CreatedBy = block.CreatedBy,
			UpdatedBy = block.UpdatedBy,
			ParentId = block.ParentId,
			Index = block.Index,
			Properties = block.Properties
		};
	}

	public PageBlockView ToView()
	{
		return new PageBlockView()
		{
			Id = Id,
			Content = Content,
			Index = Index,
			ParentId = ParentId,
			Properties = Properties,
			Type = Type
		};
	}
}