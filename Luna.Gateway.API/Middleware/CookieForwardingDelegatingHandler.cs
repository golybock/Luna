namespace Luna.Gateway.API.Middleware;

public class CookieForwardingDelegatingHandler : DelegatingHandler
{
	private readonly IHttpContextAccessor _httpContextAccessor;

	public CookieForwardingDelegatingHandler(IHttpContextAccessor httpContextAccessor)
	{
		_httpContextAccessor = httpContextAccessor;
	}

	protected override async Task<HttpResponseMessage> SendAsync(
		HttpRequestMessage request, CancellationToken cancellationToken)
	{
		// Пересылаем куки от клиента к нижестоящему сервису
		HttpContext? httpContext = _httpContextAccessor.HttpContext;
		if (httpContext?.Request.Cookies.Count > 0)
		{
			string cookieHeader = string.Join("; ",
				httpContext.Request.Cookies.Select(c => $"{c.Key}={c.Value}"));

			request.Headers.Add("Cookie", cookieHeader);
		}

		HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

		if (response.Headers.TryGetValues("Set-Cookie", out IEnumerable<string>? setCookieValues) && httpContext != null)
		{
			foreach (string cookieValue in setCookieValues)
			{
				httpContext.Response.Headers.Append("Set-Cookie", cookieValue);
			}
		}

		return response;
	}
}