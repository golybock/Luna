using Luna.Pages.Repositories.Repositories.PageComment;
using Luna.Pages.Services.Commands.PageComment;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Luna.Pages.Services.Handlers.Command.PageComment;

public class DeletePageCommentCommandHandler : PageCommentCommandHandlerBase, IRequestHandler<DeletePageCommentCommand, bool>
{
	public DeletePageCommentCommandHandler(
		IPageCommentCommandRepository pageCommentCommandRepository,
		ILogger<DeletePageCommentCommandHandler> logger
	) : base(pageCommentCommandRepository, logger)
	{ }

	public async Task<bool> Handle(DeletePageCommentCommand request, CancellationToken cancellationToken)
	{
		return await PageCommentCommandRepository.DeletePageComment(request.Id);
	}
}