using Luna.Pages.Models.Domain.Models;
using MediatR;

namespace Luna.Pages.Services.Commands.Search;

public record IndexPageCommand(
	PageVersionDomain? PageVersion,
	PageDomain PageDomain
) : IRequest<bool>;