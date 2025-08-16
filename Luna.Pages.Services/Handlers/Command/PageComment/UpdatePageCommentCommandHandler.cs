using System.Reflection;
using Luna.Pages.Repositories.Repositories.PageComment;
using Luna.Pages.Services.Commands.PageComment;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Luna.Pages.Services.Handlers.Command.PageComment;

public class UpdatePageCommentCommandHandler : PageCommentCommandHandlerBase, IRequestHandler<UpdatePageCommentCommand, bool>
{
	public UpdatePageCommentCommandHandler(
		IPageCommentCommandRepository pageCommentCommandRepository,
		ILogger logger
	) : base(pageCommentCommandRepository, logger) {}

	public async Task<bool> Handle(UpdatePageCommentCommand request, CancellationToken cancellationToken)
	{
		Dictionary<string, object?> updates = new();

		foreach (PropertyInfo property in request.PatchPageCommentBlank.GetType().GetProperties())
		{
			object? value = property.GetValue(request.PatchPageCommentBlank);

			if (value != null)
			{
				updates.Add(property.Name, value);
			}
		}

		return await PageCommentCommandRepository.PatchPageComment(request.Id, updates);
	}
}