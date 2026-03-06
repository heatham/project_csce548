using System.Net.Http.Headers;
using MatchTracker.Web;
using MatchTracker.Web.Components;

var builder = WebApplication.CreateBuilder(args);

// Blazor Web App (Interactive Server)
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Read API base URL from appsettings.json
var apiBaseUrl = builder.Configuration["Api:BaseUrl"];
if (string.IsNullOrWhiteSpace(apiBaseUrl))
{
    throw new InvalidOperationException(
        "Missing configuration value: Api:BaseUrl. Add it to appsettings.json.");
}

// Register an HttpClient for your Project 2 API
builder.Services.AddHttpClient("MatchTrackerApi", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue("application/json"));
});

// Register your table API wrappers (Services/*.cs)
// NOTE: Change these names if your classes are in a different namespace.
builder.Services.AddScoped<GamesApi>();
builder.Services.AddScoped<MapsApi>();
builder.Services.AddScoped<CharactersApi>();
builder.Services.AddScoped<MatchesApi>();
builder.Services.AddScoped<MatchStatsApi>();

var app = builder.Build();

// Standard middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

// Map the Blazor app
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();