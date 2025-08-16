using Luna.Pages.Models.Blank.Models;
using Luna.Pages.Models.Database.Models;
using Luna.Pages.Models.View.Models;
using MongoDB.Bson;

namespace Luna.Pages.Models.Domain.Models;

public class PageCommentDomain
{
	public Guid Id { get; set; }
	public Guid PageId { get; set; }
	public Guid UserId { get; set; }
	public string? Content { get; set; }
	public DateTime? DeletedAt { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime? UpdatedAt { get; set; }
	public Guid? ParentId { get; set; }
	public Guid? BlockId { get; set; }
	public object? Reactions { get; set; }

	public static PageCommentDomain FromDatabase(PageCommentDatabase pageCommentDatabase)
	{
		return new PageCommentDomain()
		{
			Id = pageCommentDatabase.Id,
			PageId = pageCommentDatabase.PageId,
			UserId = pageCommentDatabase.UserId,
			Content = pageCommentDatabase.Content,
			DeletedAt = pageCommentDatabase.DeletedAt,
			CreatedAt = pageCommentDatabase.CreatedAt,
			UpdatedAt = pageCommentDatabase.UpdatedAt,
			ParentId = pageCommentDatabase.ParentId,
			BlockId = pageCommentDatabase.BlockId,
			Reactions = pageCommentDatabase.Reactions,
		};
	}

	public static PageCommentDomain FromBlank(Guid id, Guid operationBy, CreatePageCommentBlank createPageCommentBlank)
	{
		return new PageCommentDomain()
		{
			Id = id,
			PageId = createPageCommentBlank.PageId,
			ParentId = createPageCommentBlank.ParentId,
			BlockId = createPageCommentBlank.BlockId,
			Reactions = createPageCommentBlank.Reactions,
			Content = createPageCommentBlank.Content,
			UserId = operationBy
		};
	}

	public PageCommentDatabase ToDatabase()
	{
		return new PageCommentDatabase()
		{
			Id = Id,
			PageId = PageId,
			ParentId = ParentId,
			BlockId = BlockId,
			Reactions = Reactions.ToBsonDocument(),
			Content = Content,
			DeletedAt = DeletedAt,
			CreatedAt = CreatedAt,
			UpdatedAt = UpdatedAt,
		};
	}

	public PageCommentView ToView()
	{
		return new PageCommentView()
		{
			Id = Id,
			PageId = PageId,
			ParentId = ParentId,
			BlockId = BlockId,
			Reactions = Reactions.ToBsonDocument(),
			Content = Content,
			DeletedAt = DeletedAt,
			CreatedAt = CreatedAt,
			UpdatedAt = UpdatedAt,
		};
	}
}