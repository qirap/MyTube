using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text.Json;
using Yarp.ReverseProxy.Transforms;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
	options.Limits.MaxRequestBodySize = 500 * 1024 * 1024; // 500MB
});

builder.Services.AddCors(policy =>
{
	policy.AddPolicy("CorsPolicy", opt => opt
		.AllowAnyOrigin()
		.AllowAnyHeader()
		.AllowAnyMethod());
});

var jwtSettings = builder.Configuration.GetSection("JwtBearer");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	{
		options.Authority = jwtSettings["Authority"];
		options.Audience = jwtSettings["Audience"];
		options.MetadataAddress = jwtSettings["Metadata"];
		options.RequireHttpsMetadata = false;
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true
		};
	});
builder.Services.AddAuthorization();

builder.Services.AddReverseProxy()
	.LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
	.AddTransforms(context =>
	{
		context.AddRequestTransform(async transformContext =>
		{
			var user = transformContext.HttpContext.User;

			if (user.Identity?.IsAuthenticated == true)
			{
				transformContext.ProxyRequest.Headers.Add("X-User-Id", user.FindFirst(ClaimTypes.NameIdentifier)?.Value);
				transformContext.ProxyRequest.Headers.Add("X-User-Name", user.FindFirst("preferred_username")?.Value);

				var roles = new List<string>();
				var resourceAccessClaim = user.FindFirst("resource_access")?.Value;

				if (!string.IsNullOrEmpty(resourceAccessClaim))
				{
					try
					{
						using var doc = JsonDocument.Parse(resourceAccessClaim);
						if (doc.RootElement.TryGetProperty("gateway", out var gatewayElement) &&
							gatewayElement.TryGetProperty("roles", out var rolesElement) &&
							rolesElement.ValueKind == JsonValueKind.Array)
						{
							roles = rolesElement.EnumerateArray()
											   .Select(r => r.GetString())
											   .Where(r => !string.IsNullOrEmpty(r))
											   .ToList()!;
						}
					}
					catch (JsonException ex)
					{
						Console.Error.WriteLine($"������ �������� �����: {ex}");
					}
				}

				transformContext.ProxyRequest.Headers.Add("X-User-Roles", string.Join(",", roles));
			}
		});
	});

var app = builder.Build();
app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.MapReverseProxy().RequireAuthorization();

Console.WriteLine(jwtSettings["Audience"]);

app.Run();