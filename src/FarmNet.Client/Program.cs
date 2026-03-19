using Blazorise;
using Blazorise.FluentValidation;
using Blazorise.Icons.FontAwesome;
using Blazorise.Tailwind;
using FluentValidation;
using FarmNet.Client.Auth;
using FarmNet.Client.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<FarmNet.Client.App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiBaseUrl = builder.Configuration["ApiBaseUrl"];
var productToken = builder.Configuration["Blazorise:ProductToken"];

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseUrl) });

builder.Services
    .AddBlazorise(options => { options.Immediate = true; options.ProductToken = productToken; })
    .AddTailwindProviders()
    .AddFontAwesomeIcons()
    .AddBlazoriseFluentValidation();

builder.Services.AddValidatorsFromAssemblyContaining<FarmNet.Client.App>();

builder.Services.AddScoped<JwtAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<JwtAuthStateProvider>());
builder.Services.AddAuthorizationCore();

builder.Services.AddScoped<ApiClient>();
builder.Services.AddScoped<AuthApiService>();

await builder.Build().RunAsync();
