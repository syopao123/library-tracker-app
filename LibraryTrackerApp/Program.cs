using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using LibraryTrackerApp;
using LibraryTrackerApp.Services;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Blazor.LocalStorage.WebAssembly
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
});
builder.Services.AddLocalStorageServices();

builder.Services.AddScoped<AuthRefreshHandler>();

// LibraryTrackerApi
builder.Services.AddHttpClient("WebApi", httpClient =>
{
    httpClient.BaseAddress = new Uri("http://localhost:5068/api");
}).AddHttpMessageHandler<AuthRefreshHandler>();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Authorization
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, AuthService>();
builder.Services.AddSingleton<AuthService>();

builder.Services.AddSingleton<BookService>();

await builder.Build().RunAsync();