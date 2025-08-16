using Luna.Pages.Models.Blank.Models;
using MediatR;

namespace Luna.Pages.Services.Commands.PageComment;

public record CreatePageCommentCommand(
	Guid OperationBy,
	CreatePageCommentBlank CreatePageCommentBlank
) : IRequest<bool>;