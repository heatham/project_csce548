using System.Net;
using System.Net.Http.Json;
using MatchTracker;

namespace MatchTracker.Web;

public sealed class MatchesApi
{
    private readonly IHttpClientFactory _factory;

    public MatchesApi(IHttpClientFactory factory)
    {
        _factory = factory;
    }

    private HttpClient Client => _factory.CreateClient("MatchTrackerApi");

    public async Task<List<Match>> GetAllAsync(CancellationToken ct = default)
        => await Client.GetFromJsonAsync<List<Match>>("/matches", ct) ?? new();

    public async Task<Match?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var resp = await Client.GetAsync($"/matches/{id}", ct);
        if (resp.StatusCode == HttpStatusCode.NotFound)
            return null;

        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<Match>(cancellationToken: ct);
    }

    public async Task<List<Match>> GetByGameAsync(int gameId, CancellationToken ct = default)
        => await Client.GetFromJsonAsync<List<Match>>($"/games/{gameId}/matches", ct) ?? new();
}