using MediatR;

namespace Luna.Pages.Services.Commands.Search;

public record DeletePageIndexCommand(
	string PageId
) : IRequest<bool>;