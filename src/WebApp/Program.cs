using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WebApp;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddOidcAuthentication(options =>
{
	options.ProviderOptions.Authority = "http://localhost:5000/auth/realms/test";
	options.ProviderOptions.ClientId = "blazor-client";
	options.ProviderOptions.ResponseType = "code";
	options.ProviderOptions.DefaultScopes.Add("blazor_api_scope");
});
builder.Services.AddAuthorizationCore();

builder.Services.AddHttpClient("Gateway", client => client.BaseAddress = new Uri("http://localhost:5000/"))
	.AddHttpMessageHandler(sp =>
	{
		var handler = sp.GetRequiredService<AuthorizationMessageHandler>()
			.ConfigureHandler(authorizedUrls: ["http://localhost:5000"]);
		return handler;
	});
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("Gateway"));

await builder.Build().RunAsync();
