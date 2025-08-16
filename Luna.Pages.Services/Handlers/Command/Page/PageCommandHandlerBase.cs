using Luna.Pages.Repositories.Repositories.Page.Command;
using Microsoft.Extensions.Logging;

namespace Luna.Pages.Services.Handlers.Command.Page;

public abstract class PageCommandHandlerBase
{
	protected readonly IPageCommandRepository PageCommandRepository;

	protected PageCommandHandlerBase(IPageCommandRepository pageCommandRepository)
	{
		PageCommandRepository = pageCommandRepository;
	}
}