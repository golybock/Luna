using System.Reflection;
using Luna.Pages.Repositories.Repositories.Page.Command;
using Luna.Pages.Services.Commands.Page;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Luna.Pages.Services.Handlers.Command.Page;

public class UpdatePageCommandHandler : IRequestHandler<UpdatePageCommand, bool>
{
	private readonly IPageCommandRepository _pageCommandRepository;

	public UpdatePageCommandHandler(IPageCommandRepository commandRepository)
	{
		_pageCommandRepository = commandRepository;
	}

	public async Task<bool> Handle(UpdatePageCommand request, CancellationToken cancellationToken)
	{
		Dictionary<string, object?> updates = new();

		foreach (PropertyInfo property in request.PatchPageBlank.GetType().GetProperties())
		{
			object? value = property.GetValue(request.PatchPageBlank);

			if (value != null)
			{
				updates.Add(property.Name, value);
			}
		}

		return await _pageCommandRepository.PatchPageAsync(request.PageId, updates, cancellationToken);
	}
}
