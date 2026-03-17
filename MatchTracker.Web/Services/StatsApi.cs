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
        if (resp.StatusCode == HttpStatusCode.NotFound) return null;
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<MatchStat>(cancellationToken: ct);
    }

    public async Task<MatchStat?> GetByMatchAsync(int matchId, CancellationToken ct = default)
        => await Client.GetFromJsonAsync<MatchStat>($"/matches/{matchId}/matchstats", ct);

    public async Task<int> CreateAsync(MatchStat stat, CancellationToken ct = default)
    {
        var resp = await Client.PostAsJsonAsync("/matchstats", stat, ct);
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<int>(cancellationToken: ct);
    }

    public async Task<int> UpdateAsync(int id, MatchStat stat, CancellationToken ct = default)
    {
        var resp = await Client.PutAsJsonAsync($"/matchstats/{id}", stat, ct);
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<int>(cancellationToken: ct);
    }

    public async Task<int> DeleteAsync(int id, CancellationToken ct = default)
    {
        var resp = await Client.DeleteAsync($"/matchstats/{id}", ct);
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<int>(cancellationToken: ct);
    }
}
//using System.Net;
//using System.Net.Http.Json;
//using MatchTracker;

//namespace MatchTracker.Web;

//public sealed class MatchStatsApi
//{
//    private readonly IHttpClientFactory _factory;

//    public MatchStatsApi(IHttpClientFactory factory)
//    {
//        _factory = factory;
//    }

//    private HttpClient Client => _factory.CreateClient("MatchTrackerApi");

//    public async Task<List<MatchStat>> GetAllAsync(CancellationToken ct = default)
//        => await Client.GetFromJsonAsync<List<MatchStat>>("/matchstats", ct) ?? new();

//    public async Task<MatchStat?> GetByIdAsync(int id, CancellationToken ct = default)
//    {
//        var resp = await Client.GetAsync($"/matchstats/{id}", ct);
//        if (resp.StatusCode == HttpStatusCode.NotFound)
//            return null;

//        resp.EnsureSuccessStatusCode();
//        return await resp.Content.ReadFromJsonAsync<MatchStat>(cancellationToken: ct);
//    }

//    public async Task<MatchStat?> GetByMatchAsync(int matchId, CancellationToken ct = default)
//        => await Client.GetFromJsonAsync<MatchStat>($"/matches/{matchId}/matchstats", ct);
//}