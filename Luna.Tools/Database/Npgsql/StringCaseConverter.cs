using System.Text;

namespace Luna.Tools.Database.Npgsql;

public static class StringCaseConverter
{
	public static string ToSnakeCase(this String text)
	{
		if(String.IsNullOrWhiteSpace(text))
			throw new ArgumentNullException(nameof(text));

		if(text.Length < 2)
			return text.ToLower();

		StringBuilder sb = new StringBuilder();

		sb.Append(char.ToLowerInvariant(text[0]));

		for(int i = 1; i < text.Length; ++i) {
			char c = text[i];
			if(char.IsUpper(c)) {
				sb.Append('_');
				sb.Append(char.ToLowerInvariant(c));
			} else {
				sb.Append(c);
			}
		}
		return sb.ToString();
	}
}