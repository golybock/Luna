using Luna.Pages.Models.Domain.Models;
using Luna.Pages.Repositories.Repositories.PageComment;
using Luna.Pages.Services.Commands.PageComment;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Luna.Pages.Services.Handlers.Command.PageComment;

public class CreatePageCommentCommandHandler : PageCommentCommandHandlerBase, IRequestHandler<CreatePageCommentCommand, bool>
{
	public CreatePageCommentCommandHandler(
		IPageCommentCommandRepository pageCommentCommandRepository,
		ILogger logger
	) : base(pageCommentCommandRepository, logger) { }

	public async Task<bool> Handle(CreatePageCommentCommand request, CancellationToken cancellationToken)
	{
		Guid commentId = Guid.NewGuid();

		PageCommentDomain pageComment = PageCommentDomain.FromBlank(commentId, request.OperationBy, request.CreatePageCommentBlank);

		return await PageCommentCommandRepository.CreatePageComment(pageComment.ToDatabase());
	}
}