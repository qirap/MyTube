using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Shared.Security.Implementations;

public class MyTubeAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
	public MyTubeAuthenticationHandler(
		IOptionsMonitor<AuthenticationSchemeOptions> options,
		ILoggerFactory logger,
		UrlEncoder encoder,
		ISystemClock clock)
		: base(options, logger, encoder, clock)
	{ }

	protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
	{
		var claims = new List<Claim>();

		if (Request.Headers.TryGetValue("X-User-Id", out var userId))
		{
			claims.Add(new Claim(ClaimTypes.NameIdentifier, userId!));
		}

		if (Request.Headers.TryGetValue("X-User-Name", out var userName))
		{
			claims.Add(new Claim(ClaimTypes.Name, userName!));
		}

		if (Request.Headers.TryGetValue("X-User-Roles", out var roles))
		{
			var roleList = roles.ToString().Split(',', StringSplitOptions.RemoveEmptyEntries);
			foreach (var role in roleList)
			{
				claims.Add(new Claim(ClaimTypes.Role, role));
			}
		}

		if (claims.Count == 0)
		{
			return AuthenticateResult.Fail("No valid authentication headers found");
		}

		var identity = new ClaimsIdentity(claims, "MyTubeScheme");
		var principal = new ClaimsPrincipal(identity);
		var ticket = new AuthenticationTicket(principal, "MyTubeScheme");

		return AuthenticateResult.Success(ticket);
	}
}

