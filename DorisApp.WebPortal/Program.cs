using Blazored.LocalStorage;
using DorisApp.Data.Library.API;
using DorisApp.Data.Library.Plugins.Excel;
using DorisApp.WebPortal;
using DorisApp.WebPortal.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<IAuthenticationServices, AuthenticationServices>();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();

builder.Services.AddSingleton<IAPIHelper, APIHelper>();
builder.Services.AddTransient<CategoryEndpoint>();
builder.Services.AddTransient<SubCategoryEndpoint>();
builder.Services.AddTransient<BrandEndpoint>();
builder.Services.AddTransient<ProductEndpoint>();
builder.Services.AddTransient<InventoryEndpoint>();

//Plugins
builder.Services.AddTransient<ExcelConnector>();


builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();