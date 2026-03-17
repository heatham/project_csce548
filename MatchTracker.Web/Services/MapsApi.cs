using System.Net;
using System.Net.Http.Json;
using MatchTracker;

namespace MatchTracker.Web;

public sealed class MapsApi
{
    private readonly IHttpClientFactory _factory;

    public MapsApi(IHttpClientFactory factory)
    {
        _factory = factory;
    }

    private HttpClient Client => _factory.CreateClient("MatchTrackerApi");

    public async Task<List<Map>> GetAllAsync(CancellationToken ct = default)
        => await Client.GetFromJsonAsync<List<Map>>("/maps", ct) ?? new();

    public async Task<Map?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var resp = await Client.GetAsync($"/maps/{id}", ct);
        if (resp.StatusCode == HttpStatusCode.NotFound) return null;
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<Map>(cancellationToken: ct);
    }

    public async Task<List<Map>> GetByGameAsync(int gameId, CancellationToken ct = default)
        => await Client.GetFromJsonAsync<List<Map>>($"/games/{gameId}/maps", ct) ?? new();

    public async Task<int> CreateAsync(int gameId, string name, CancellationToken ct = default)
    {
        var resp = await Client.PostAsync(
            $"/maps?gameId={gameId}&name={Uri.EscapeDataString(name)}",
            null,
            ct);

        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<int>(cancellationToken: ct);
    }

    public async Task<int> UpdateAsync(int id, string newName, CancellationToken ct = default)
    {
        var resp = await Client.PutAsync($"/maps/{id}?newName={Uri.EscapeDataString(newName)}", null, ct);
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<int>(cancellationToken: ct);
    }

    public async Task<int> DeleteAsync(int id, CancellationToken ct = default)
    {
        var resp = await Client.DeleteAsync($"/maps/{id}", ct);
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<int>(cancellationToken: ct);
    }
}
//using System.Net;
//using System.Net.Http.Json;
//using MatchTracker;

//namespace MatchTracker.Web;

//public sealed class MapsApi
//{
//    private readonly IHttpClientFactory _factory;

//    public MapsApi(IHttpClientFactory factory)
//    {
//        _factory = factory;
//    }

//    private HttpClient Client => _factory.CreateClient("MatchTrackerApi");

//    public async Task<List<Map>> GetAllAsync(CancellationToken ct = default)
//        => await Client.GetFromJsonAsync<List<Map>>("/maps", ct) ?? new();

//    public async Task<Map?> GetByIdAsync(int id, CancellationToken ct = default)
//    {
//        var resp = await Client.GetAsync($"/maps/{id}", ct);
//        if (resp.StatusCode == HttpStatusCode.NotFound)
//            return null;

//        resp.EnsureSuccessStatusCode();
//        return await resp.Content.ReadFromJsonAsync<Map>(cancellationToken: ct);
//    }

//    public async Task<List<Map>> GetByGameAsync(int gameId, CancellationToken ct = default)
//        => await Client.GetFromJsonAsync<List<Map>>($"/games/{gameId}/maps", ct) ?? new();
//}