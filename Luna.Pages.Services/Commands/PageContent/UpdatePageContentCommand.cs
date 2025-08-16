using Luna.Pages.Models.Blank.Models;
using MediatR;

namespace Luna.Pages.Services.Commands.PageContent;

public record UpdatePageContentCommand(
	Guid PageId,
	Guid OperationBy,
	UpdatePageContentBlank UpdatePageContentBlank
) : IRequest<bool>;