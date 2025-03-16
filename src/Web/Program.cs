using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Text.Json;
using Web;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var httpClient = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };

var response = await httpClient.GetAsync($"appsettings.{builder.HostEnvironment.Environment}.json");
if (!response.IsSuccessStatusCode)
{
	throw new Exception($"Failed to load configuration: {response.StatusCode}");
}
var configJson = await response.Content.ReadAsStringAsync();
var config = JsonSerializer.Deserialize<Dictionary<string, object>>(configJson);
foreach (var kv in config)
{
	builder.Configuration[kv.Key] = kv.Value?.ToString();
}

builder.Services.AddOidcAuthentication(options =>
{
	options.ProviderOptions.Authority = builder.Configuration["Keycloak:Authority"];
	options.ProviderOptions.ClientId = builder.Configuration["Keycloak:ClientId"];
	options.ProviderOptions.ResponseType = "code";
	options.ProviderOptions.DefaultScopes.Add(builder.Configuration["Keycloak:DefaultScope"]);
});

builder.Services.AddHttpClient("Gateway", client => client.BaseAddress = new Uri(builder.Configuration["Keycloak:GatewayUri"]))
	.AddHttpMessageHandler(sp =>
	{
		var handler = sp.GetRequiredService<AuthorizationMessageHandler>()
			.ConfigureHandler(authorizedUrls: [builder.Configuration["Keycloak:GatewayUri"]]);
		return handler;
	});
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("Gateway"));

await builder.Build().RunAsync();
