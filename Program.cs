using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using YourCravings;
using YourCravings.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// 既存のサービス登録
builder.Services.AddScoped<AuthenticationService>();
builder.Services.AddScoped<EmailJsService>();
builder.Services.AddScoped<HistoryService>();
builder.Services.AddScoped<LocalStorageService>();
builder.Services.AddScoped<NavigationMessageService>();
builder.Services.AddSingleton<DateTimeService>();

await builder.Build().RunAsync();
