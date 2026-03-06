using System.Net;
using System.Net.Http.Json;
using MatchTracker;

namespace MatchTracker.Web;

public sealed class MatchStatsApi
{
    private readonly IHttpClientFactory _factory;

    public MatchStatsApi(IHttpClientFactory factory)
    {
        _factory = factory;
    }

    private HttpClient Client => _factory.CreateClient("MatchTrackerApi");

    public async Task<List<MatchStat>> GetAllAsync(CancellationToken ct = default)
        => await Client.GetFromJsonAsync<List<MatchStat>>("/matchstats", ct) ?? new();

    public async Task<MatchStat?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var resp = await Client.GetAsync($"/matchstats/{id}", ct);
        if (resp.StatusCode == HttpStatusCode.NotFound)
            return null;

        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<MatchStat>(cancellationToken: ct);
    }

    public async Task<MatchStat?> GetByMatchAsync(int matchId, CancellationToken ct = default)
        => await Client.GetFromJsonAsync<MatchStat>($"/matches/{matchId}/matchstats", ct);
}