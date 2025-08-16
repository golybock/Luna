using Luna.Pages.Repositories.Repositories.PageComment;
using Microsoft.Extensions.Logging;

namespace Luna.Pages.Services.Handlers.Command.PageComment;

public abstract class PageCommentCommandHandlerBase
{
	protected readonly IPageCommentCommandRepository PageCommentCommandRepository;
	protected readonly ILogger Logger;

	public PageCommentCommandHandlerBase(
		IPageCommentCommandRepository pageCommentCommandRepository,
		ILogger logger
	)
	{
		PageCommentCommandRepository = pageCommentCommandRepository;
		Logger = logger;
	}
}