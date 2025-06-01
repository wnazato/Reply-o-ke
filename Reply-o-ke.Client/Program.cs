using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Reply_o_ke.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Configure HTTP client
builder.Services.AddScoped(sp => new HttpClient 
{ 
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) 
});

// Register services
builder.Services.AddScoped<ApiService>();
builder.Services.AddScoped(sp => new SignalRService(builder.HostEnvironment.BaseAddress));

await builder.Build().RunAsync();
