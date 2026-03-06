using System.Net;
using System.Net.Http.Json;
using MatchTracker;

namespace MatchTracker.Web;

public sealed class GamesApi
{
    private readonly IHttpClientFactory _factory;
    public GamesApi(IHttpClientFactory factory) => _factory = factory;
    private HttpClient Client => _factory.CreateClient("MatchTrackerApi");

    // GET ALL
    public async Task<List<Game>> GetAllAsync(CancellationToken ct = default)
        => await Client.GetFromJsonAsync<List<Game>>("/games", ct) ?? new();

    // GET ONE
    public async Task<Game?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var resp = await Client.GetAsync($"/games/{id}", ct);
        if (resp.StatusCode == HttpStatusCode.NotFound) return null;
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<Game>(cancellationToken: ct);
    }
}