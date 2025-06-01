using Reply_o_ke.Components;
using Reply_o_ke.Data;
using Reply_o_ke.Services;
using Reply_o_ke.Hubs;
using Reply_o_ke.Client.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents()
    .AddInteractiveServerComponents();

// Add Entity Framework
builder.Services.AddDbContext<KaraokeDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? 
                     "Data Source=karaoke.db"));

// Add SignalR
builder.Services.AddSignalR();

// Add API controllers
builder.Services.AddControllers();

// Add custom services
builder.Services.AddScoped<KaraokeService>();
builder.Services.AddScoped<YouTubeService>();
builder.Services.AddScoped<QRCodeService>();

// Add HttpClient for client services
builder.Services.AddHttpClient();

// Add client services
builder.Services.AddScoped<Reply_o_ke.Client.Services.ApiService>(sp =>
{
    var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
    var httpClient = httpClientFactory.CreateClient();
    var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5218/";
    httpClient.BaseAddress = new Uri(apiBaseUrl);
    return new Reply_o_ke.Client.Services.ApiService(httpClient);
});
builder.Services.AddScoped(sp => {
    var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5218";
    return new Reply_o_ke.Client.Services.SignalRService(apiBaseUrl);
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<KaraokeDbContext>();
    context.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseCors("AllowAll");

// Map API controllers
app.MapControllers();

// Map SignalR hub
app.MapHub<KaraokeHub>("/karaokehub");

app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddInteractiveServerRenderMode();

app.Run();
