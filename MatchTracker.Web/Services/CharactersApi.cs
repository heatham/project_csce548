using System.Net;
using System.Net.Http.Json;
using MatchTracker;

namespace MatchTracker.Web;

public sealed class CharactersApi
{
    private readonly IHttpClientFactory _factory;

    public CharactersApi(IHttpClientFactory factory)
    {
        _factory = factory;
    }

    private HttpClient Client => _factory.CreateClient("MatchTrackerApi");

    public async Task<List<Character>> GetAllAsync(CancellationToken ct = default)
        => await Client.GetFromJsonAsync<List<Character>>("/characters", ct) ?? new();

    public async Task<Character?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var resp = await Client.GetAsync($"/characters/{id}", ct);
        if (resp.StatusCode == HttpStatusCode.NotFound) return null;
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<Character>(cancellationToken: ct);
    }

    public async Task<List<Character>> GetByGameAsync(int gameId, CancellationToken ct = default)
        => await Client.GetFromJsonAsync<List<Character>>($"/games/{gameId}/characters", ct) ?? new();

    public async Task<int> CreateAsync(int gameId, string name, string? role, CancellationToken ct = default)
    {
        var url = $"/characters?gameId={gameId}&name={Uri.EscapeDataString(name)}";
        if (!string.IsNullOrWhiteSpace(role))
            url += $"&role={Uri.EscapeDataString(role)}";

        var resp = await Client.PostAsync(url, null, ct);
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<int>(cancellationToken: ct);
    }

    public async Task<int> UpdateAsync(int id, string newName, string? newRole, CancellationToken ct = default)
    {
        var url = $"/characters/{id}?newName={Uri.EscapeDataString(newName)}";
        if (!string.IsNullOrWhiteSpace(newRole))
            url += $"&newRole={Uri.EscapeDataString(newRole)}";

        var resp = await Client.PutAsync(url, null, ct);
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<int>(cancellationToken: ct);
    }

    public async Task<int> DeleteAsync(int id, CancellationToken ct = default)
    {
        var resp = await Client.DeleteAsync($"/characters/{id}", ct);
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<int>(cancellationToken: ct);
    }
}
//using System.Net;
//using System.Net.Http.Json;
//using MatchTracker;

//namespace MatchTracker.Web;

//public sealed class CharactersApi
//{
//    private readonly IHttpClientFactory _factory;

//    public CharactersApi(IHttpClientFactory factory)
//    {
//        _factory = factory;
//    }

//    private HttpClient Client => _factory.CreateClient("MatchTrackerApi");

//    public async Task<List<Character>> GetAllAsync(CancellationToken ct = default)
//        => await Client.GetFromJsonAsync<List<Character>>("/characters", ct) ?? new();

//    public async Task<Character?> GetByIdAsync(int id, CancellationToken ct = default)
//    {
//        var resp = await Client.GetAsync($"/characters/{id}", ct);
//        if (resp.StatusCode == HttpStatusCode.NotFound)
//            return null;

//        resp.EnsureSuccessStatusCode();
//        return await resp.Content.ReadFromJsonAsync<Character>(cancellationToken: ct);
//    }

//    public async Task<List<Character>> GetByGameAsync(int gameId, CancellationToken ct = default)
//        => await Client.GetFromJsonAsync<List<Character>>($"/games/{gameId}/characters", ct) ?? new();
//}