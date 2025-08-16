using Luna.Pages.Models.Blank.Models;
using MediatR;

namespace Luna.Pages.Services.Commands.PageComment;

public record UpdatePageCommentCommand(
	Guid Id,
	Guid OperationBy,
	PatchPageCommentBlank PatchPageCommentBlank
) : IRequest<bool>;