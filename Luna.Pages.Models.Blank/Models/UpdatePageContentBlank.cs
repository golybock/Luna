using System.Text.Json.Serialization;

namespace Luna.Pages.Models.Blank.Models;

public class UpdatePageContentBlank
{
	[JsonPropertyName("document")]
	public object? Document { get; set; }
	public string ChangeDescription { get; set; } = null!;
}