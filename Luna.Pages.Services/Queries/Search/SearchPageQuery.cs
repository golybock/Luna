using Luna.Pages.Models.View.Additional;
using MediatR;

namespace Luna.Pages.Services.Queries.Search;

public record SearchPageQuery(
	string Query,
	Guid WorkspaceId,
	int From = 0,
	int Size = 10
) : IRequest<List<LightPageView>>;