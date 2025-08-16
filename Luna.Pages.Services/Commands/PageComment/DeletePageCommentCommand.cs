using MediatR;

namespace Luna.Pages.Services.Commands.PageComment;

public record DeletePageCommentCommand(
	Guid Id,
	Guid OperationBy
) : IRequest<bool>;