using System.Net.Http.Json;
using MatchTracker;

static string Prompt(string label)
{
    Console.Write(label);
    return Console.ReadLine() ?? "";
}

static int PromptInt(string label)
{
    while (true)
    {
        var s = Prompt(label);
        if (int.TryParse(s, out var v)) return v;
        Console.WriteLine("Please enter an integer.");
    }
}

static int? PromptIntNullable(string label)
{
    while (true)
    {
        var s = Prompt(label + " (blank for null): ");
        if (string.IsNullOrWhiteSpace(s)) return null;
        if (int.TryParse(s, out var v)) return v;
        Console.WriteLine("Please enter an integer or blank.");
    }
}

static char PromptWL(string label)
{
    while (true)
    {
        var s = Prompt(label + " (W/L): ").Trim().ToUpperInvariant();
        if (s == "W" || s == "L") return s[0];
        Console.WriteLine("Enter W or L.");
    }
}

static string Esc(string s) => Uri.EscapeDataString(s);

static void PrintHeader(string title)
{
    Console.WriteLine();
    Console.WriteLine("====================================");
    Console.WriteLine(title);
    Console.WriteLine("====================================");
}

static async Task<T?> ReadJsonOrNull<T>(HttpResponseMessage resp)
{
    if (!resp.IsSuccessStatusCode) return default;
    return await resp.Content.ReadFromJsonAsync<T>();
}

static async Task ShowError(HttpResponseMessage resp)
{
    Console.WriteLine($"HTTP {(int)resp.StatusCode} {resp.ReasonPhrase}");
    var body = await resp.Content.ReadAsStringAsync();
    if (!string.IsNullOrWhiteSpace(body))
        Console.WriteLine(body);
}

var baseUrl = Prompt("API base URL (default http://localhost:5000): ").Trim();
if (string.IsNullOrWhiteSpace(baseUrl)) baseUrl = "http://localhost:5000";

using var http = new HttpClient { BaseAddress = new Uri(baseUrl) };

Console.WriteLine();
Console.WriteLine("=== MatchTracker Client (CRUD via API) ===");
Console.WriteLine($"Using API: {baseUrl}");
Console.WriteLine("Tip: run the API first in another terminal: dotnet run --project .\\MatchTracker.Api\\MatchTracker.Api.csproj");

while (true)
{
    PrintHeader("Main Menu");
    Console.WriteLine("1) Games");
    Console.WriteLine("2) Maps");
    Console.WriteLine("3) Characters");
    Console.WriteLine("4) Matches");
    Console.WriteLine("5) MatchStats");
    Console.WriteLine("0) Exit");

    var choice = Prompt("Choice: ").Trim();
    if (choice == "0") break;

    try
    {
        switch (choice)
        {
            case "1": await GamesMenu(); break;
            case "2": await MapsMenu(); break;
            case "3": await CharactersMenu(); break;
            case "4": await MatchesMenu(); break;
            case "5": await MatchStatsMenu(); break;
            default: Console.WriteLine("Invalid choice."); break;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("ERROR: " + ex.Message);
    }
}

return;

// -------------------- Menus --------------------

async Task GamesMenu()
{
    while (true)
    {
        PrintHeader("Games");
        Console.WriteLine("1) Create game");
        Console.WriteLine("2) Get all games");
        Console.WriteLine("3) Get game by id");
        Console.WriteLine("4) Update game");
        Console.WriteLine("5) Delete game");
        Console.WriteLine("0) Back");

        var c = Prompt("Choice: ").Trim();
        if (c == "0") return;

        if (c == "1")
        {
            var name = Prompt("Game name: ");
            var resp = await http.PostAsync($"/games?name={Esc(name)}", null);
            if (!resp.IsSuccessStatusCode) { await ShowError(resp); continue; }
            var id = await resp.Content.ReadFromJsonAsync<int>();
            Console.WriteLine($"Created game_id={id}");
        }
        else if (c == "2")
        {
            var games = await http.GetFromJsonAsync<List<Game>>("/games");
            if (games is null) { Console.WriteLine("(no response)"); continue; }
            foreach (var g in games)
                Console.WriteLine($"{g.GameId}: {g.Name}");
        }
        else if (c == "3")
        {
            var id = PromptInt("Game id: ");
            var resp = await http.GetAsync($"/games/{id}");
            if (!resp.IsSuccessStatusCode) { await ShowError(resp); continue; }
            var g = await resp.Content.ReadFromJsonAsync<Game>();
            Console.WriteLine($"{g!.GameId}: {g.Name}");
        }
        else if (c == "4")
        {
            var id = PromptInt("Game id: ");
            var newName = Prompt("New name: ");
            var resp = await http.PutAsync($"/games/{id}?newName={Esc(newName)}", null);
            if (!resp.IsSuccessStatusCode) { await ShowError(resp); continue; }
            var rows = await resp.Content.ReadFromJsonAsync<int>();
            Console.WriteLine($"Updated rows={rows}");
        }
        else if (c == "5")
        {
            var id = PromptInt("Game id: ");
            var resp = await http.DeleteAsync($"/games/{id}");
            if (!resp.IsSuccessStatusCode) { await ShowError(resp); continue; }
            var rows = await resp.Content.ReadFromJsonAsync<int>();
            Console.WriteLine($"Deleted rows={rows}");
        }
        else Console.WriteLine("Invalid choice.");
    }
}

async Task MapsMenu()
{
    while (true)
    {
        PrintHeader("Maps");
        Console.WriteLine("1) Create map");
        Console.WriteLine("2) Get all maps");
        Console.WriteLine("3) Get map by id");
        Console.WriteLine("4) Get maps by game");
        Console.WriteLine("5) Update map");
        Console.WriteLine("6) Delete map");
        Console.WriteLine("0) Back");

        var c = Prompt("Choice: ").Trim();
        if (c == "0") return;

        if (c == "1")
        {
            var gameId = PromptInt("Game id: ");
            var name = Prompt("Map name: ");
            var resp = await http.PostAsync($"/maps?gameId={gameId}&name={Esc(name)}", null);
            if (!resp.IsSuccessStatusCode) { await ShowError(resp); continue; }
            var id = await resp.Content.ReadFromJsonAsync<int>();
            Console.WriteLine($"Created map_id={id}");
        }
        else if (c == "2")
        {
            var maps = await http.GetFromJsonAsync<List<Map>>("/maps");
            if (maps is null) { Console.WriteLine("(no response)"); continue; }
            foreach (var m in maps)
                Console.WriteLine($"{m.MapId}: game={m.GameId} name={m.Name}");
        }
        else if (c == "3")
        {
            var id = PromptInt("Map id: ");
            var resp = await http.GetAsync($"/maps/{id}");
            if (!resp.IsSuccessStatusCode) { await ShowError(resp); continue; }
            var m = await resp.Content.ReadFromJsonAsync<Map>();
            Console.WriteLine($"{m!.MapId}: game={m.GameId} name={m.Name}");
        }
        else if (c == "4")
        {
            var gameId = PromptInt("Game id: ");
            var maps = await http.GetFromJsonAsync<List<Map>>($"/games/{gameId}/maps");
            if (maps is null) { Console.WriteLine("(no response)"); continue; }
            foreach (var m in maps)
                Console.WriteLine($"{m.MapId}: name={m.Name}");
        }
        else if (c == "5")
        {
            var id = PromptInt("Map id: ");
            var newName = Prompt("New name: ");
            var resp = await http.PutAsync($"/maps/{id}?newName={Esc(newName)}", null);
            if (!resp.IsSuccessStatusCode) { await ShowError(resp); continue; }
            var rows = await resp.Content.ReadFromJsonAsync<int>();
            Console.WriteLine($"Updated rows={rows}");
        }
        else if (c == "6")
        {
            var id = PromptInt("Map id: ");
            var resp = await http.DeleteAsync($"/maps/{id}");
            if (!resp.IsSuccessStatusCode) { await ShowError(resp); continue; }
            var rows = await resp.Content.ReadFromJsonAsync<int>();
            Console.WriteLine($"Deleted rows={rows}");
        }
        else Console.WriteLine("Invalid choice.");
    }
}

async Task CharactersMenu()
{
    while (true)
    {
        PrintHeader("Characters");
        Console.WriteLine("1) Create character");
        Console.WriteLine("2) Get all characters");
        Console.WriteLine("3) Get character by id");
        Console.WriteLine("4) Get characters by game");
        Console.WriteLine("5) Update character");
        Console.WriteLine("6) Delete character");
        Console.WriteLine("0) Back");

        var c = Prompt("Choice: ").Trim();
        if (c == "0") return;

        if (c == "1")
        {
            var gameId = PromptInt("Game id: ");
            var name = Prompt("Character name: ");
            var role = Prompt("Role (blank for null): ");
            if (string.IsNullOrWhiteSpace(role)) role = "";

            var url = $"/characters?gameId={gameId}&name={Esc(name)}";
            if (!string.IsNullOrWhiteSpace(role))
                url += $"&role={Esc(role)}";

            var resp = await http.PostAsync(url, null);
            if (!resp.IsSuccessStatusCode) { await ShowError(resp); continue; }
            var id = await resp.Content.ReadFromJsonAsync<int>();
            Console.WriteLine($"Created character_id={id}");
        }
        else if (c == "2")
        {
            var chars = await http.GetFromJsonAsync<List<Character>>("/characters");
            if (chars is null) { Console.WriteLine("(no response)"); continue; }
            foreach (var ch in chars)
                Console.WriteLine($"{ch.CharacterId}: game={ch.GameId} name={ch.Name} role={(ch.Role ?? "null")}");
        }
        else if (c == "3")
        {
            var id = PromptInt("Character id: ");
            var resp = await http.GetAsync($"/characters/{id}");
            if (!resp.IsSuccessStatusCode) { await ShowError(resp); continue; }
            var ch = await resp.Content.ReadFromJsonAsync<Character>();
            Console.WriteLine($"{ch!.CharacterId}: game={ch.GameId} name={ch.Name} role={(ch.Role ?? "null")}");
        }
        else if (c == "4")
        {
            var gameId = PromptInt("Game id: ");
            var chars = await http.GetFromJsonAsync<List<Character>>($"/games/{gameId}/characters");
            if (chars is null) { Console.WriteLine("(no response)"); continue; }
            foreach (var ch in chars)
                Console.WriteLine($"{ch.CharacterId}: name={ch.Name} role={(ch.Role ?? "null")}");
        }
        else if (c == "5")
        {
            var id = PromptInt("Character id: ");
            var newName = Prompt("New name: ");
            var newRole = Prompt("New role (blank for null): ");
            var url = $"/characters/{id}?newName={Esc(newName)}";
            if (!string.IsNullOrWhiteSpace(newRole))
                url += $"&newRole={Esc(newRole)}";

            var resp = await http.PutAsync(url, null);
            if (!resp.IsSuccessStatusCode) { await ShowError(resp); continue; }
            var rows = await resp.Content.ReadFromJsonAsync<int>();
            Console.WriteLine($"Updated rows={rows}");
        }
        else if (c == "6")
        {
            var id = PromptInt("Character id: ");
            var resp = await http.DeleteAsync($"/characters/{id}");
            if (!resp.IsSuccessStatusCode) { await ShowError(resp); continue; }
            var rows = await resp.Content.ReadFromJsonAsync<int>();
            Console.WriteLine($"Deleted rows={rows}");
        }
        else Console.WriteLine("Invalid choice.");
    }
}

async Task MatchesMenu()
{
    while (true)
    {
        PrintHeader("Matches");
        Console.WriteLine("1) Create match");
        Console.WriteLine("2) Get all matches");
        Console.WriteLine("3) Get match by id");
        Console.WriteLine("4) Get matches by game");
        Console.WriteLine("5) Update match");
        Console.WriteLine("6) Delete match");
        Console.WriteLine("0) Back");

        var c = Prompt("Choice: ").Trim();
        if (c == "0") return;

        if (c == "1")
        {
            var gameId = PromptInt("Game id: ");
            var queue = Prompt("Queue type: ");
            var mapId = PromptIntNullable("Map id");
            var result = PromptWL("Result");
            var dur = PromptIntNullable("Duration seconds");
            var notes = Prompt("Notes (blank for null): ");
            if (string.IsNullOrWhiteSpace(notes)) notes = "";

            var m = new Match(
                MatchId: 0,
                GameId: gameId,
                MatchDate: DateTime.Now,
                QueueType: queue,
                MapId: mapId,
                Result: result,
                DurationSec: dur,
                Notes: string.IsNullOrWhiteSpace(notes) ? null : notes
            );

            var resp = await http.PostAsJsonAsync("/matches", m);
            if (!resp.IsSuccessStatusCode) { await ShowError(resp); continue; }
            var id = await resp.Content.ReadFromJsonAsync<int>();
            Console.WriteLine($"Created match_id={id}");
        }
        else if (c == "2")
        {
            var matches = await http.GetFromJsonAsync<List<Match>>("/matches");
            if (matches is null) { Console.WriteLine("(no response)"); continue; }
            foreach (var m in matches)
                Console.WriteLine($"{m.MatchId}: game={m.GameId} {m.MatchDate:g} {m.QueueType} result={m.Result} mapId={(m.MapId?.ToString() ?? "null")}");
        }
        else if (c == "3")
        {
            var id = PromptInt("Match id: ");
            var resp = await http.GetAsync($"/matches/{id}");
            if (!resp.IsSuccessStatusCode) { await ShowError(resp); continue; }
            var m = await resp.Content.ReadFromJsonAsync<Match>();
            Console.WriteLine($"{m!.MatchId}: game={m.GameId} {m.MatchDate:g} {m.QueueType} result={m.Result} duration={(m.DurationSec?.ToString() ?? "null")} notes={(m.Notes ?? "null")}");
        }
        else if (c == "4")
        {
            var gameId = PromptInt("Game id: ");
            var matches = await http.GetFromJsonAsync<List<Match>>($"/games/{gameId}/matches");
            if (matches is null) { Console.WriteLine("(no response)"); continue; }
            foreach (var m in matches)
                Console.WriteLine($"{m.MatchId}: {m.MatchDate:g} {m.QueueType} result={m.Result}");
        }
        else if (c == "5")
        {
            var id = PromptInt("Match id to update: ");

            // fetch existing first so you can edit fields without guessing defaults
            var getResp = await http.GetAsync($"/matches/{id}");
            if (!getResp.IsSuccessStatusCode) { await ShowError(getResp); continue; }
            var existing = await getResp.Content.ReadFromJsonAsync<Match>();
            if (existing is null) { Console.WriteLine("No match returned."); continue; }

            Console.WriteLine("Press Enter to keep existing value.");

            var queue = Prompt($"Queue type ({existing.QueueType}): ");
            if (string.IsNullOrWhiteSpace(queue)) queue = existing.QueueType;

            var mapIdStr = Prompt($"Map id ({existing.MapId?.ToString() ?? "null"}): ");
            int? mapId = existing.MapId;
            if (!string.IsNullOrWhiteSpace(mapIdStr))
                mapId = int.Parse(mapIdStr);

            var resStr = Prompt($"Result ({existing.Result}) W/L: ").Trim().ToUpperInvariant();
            char result = existing.Result;
            if (!string.IsNullOrWhiteSpace(resStr))
                result = (resStr == "W" || resStr == "L") ? resStr[0] : existing.Result;

            var durStr = Prompt($"Duration seconds ({existing.DurationSec?.ToString() ?? "null"}): ");
            int? dur = existing.DurationSec;
            if (!string.IsNullOrWhiteSpace(durStr))
                dur = int.Parse(durStr);

            var notes = Prompt($"Notes ({existing.Notes ?? "null"}): ");
            string? newNotes = existing.Notes;
            if (!string.IsNullOrWhiteSpace(notes))
                newNotes = notes;
            // if blank, keep existing

            var updated = existing with
            {
                MatchId = id,
                QueueType = queue,
                MapId = mapId,
                Result = result,
                DurationSec = dur,
                Notes = newNotes
            };

            var resp = await http.PutAsJsonAsync($"/matches/{id}", updated);
            if (!resp.IsSuccessStatusCode) { await ShowError(resp); continue; }
            var rows = await resp.Content.ReadFromJsonAsync<int>();
            Console.WriteLine($"Updated rows={rows}");
        }
        else if (c == "6")
        {
            var id = PromptInt("Match id: ");
            var resp = await http.DeleteAsync($"/matches/{id}");
            if (!resp.IsSuccessStatusCode) { await ShowError(resp); continue; }
            var rows = await resp.Content.ReadFromJsonAsync<int>();
            Console.WriteLine($"Deleted rows={rows}");
        }
        else Console.WriteLine("Invalid choice.");
    }
}

async Task MatchStatsMenu()
{
    while (true)
    {
        PrintHeader("MatchStats");
        Console.WriteLine("1) Create match stat");
        Console.WriteLine("2) Get all match stats");
        Console.WriteLine("3) Get match stat by id");
        Console.WriteLine("4) Get stats by match id");
        Console.WriteLine("5) Update match stat");
        Console.WriteLine("6) Delete match stat");
        Console.WriteLine("0) Back");

        var c = Prompt("Choice: ").Trim();
        if (c == "0") return;

        if (c == "1")
        {
            var matchId = PromptInt("Match id: ");
            var characterId = PromptIntNullable("Character id");
            var k = PromptInt("Kills: ");
            var d = PromptInt("Deaths: ");
            var a = PromptInt("Assists: ");
            var dmg = PromptIntNullable("Damage");
            var heal = PromptIntNullable("Healing");
            var obj = PromptIntNullable("Objective time sec");

            var s = new MatchStat(
                StatId: 0,
                MatchId: matchId,
                CharacterId: characterId,
                Kills: k,
                Deaths: d,
                Assists: a,
                Damage: dmg,
                Healing: heal,
                ObjectiveTimeSec: obj
            );

            var resp = await http.PostAsJsonAsync("/matchstats", s);
            if (!resp.IsSuccessStatusCode) { await ShowError(resp); continue; }
            var id = await resp.Content.ReadFromJsonAsync<int>();
            Console.WriteLine($"Created stat_id={id}");
        }
        else if (c == "2")
        {
            var stats = await http.GetFromJsonAsync<List<MatchStat>>("/matchstats");
            if (stats is null) { Console.WriteLine("(no response)"); continue; }
            foreach (var s in stats)
                Console.WriteLine($"{s.StatId}: match={s.MatchId} char={(s.CharacterId?.ToString() ?? "null")} K/D/A={s.Kills}/{s.Deaths}/{s.Assists}");
        }
        else if (c == "3")
        {
            var id = PromptInt("Stat id: ");
            var resp = await http.GetAsync($"/matchstats/{id}");
            if (!resp.IsSuccessStatusCode) { await ShowError(resp); continue; }
            var s = await resp.Content.ReadFromJsonAsync<MatchStat>();
            Console.WriteLine($"{s!.StatId}: match={s.MatchId} char={(s.CharacterId?.ToString() ?? "null")} K/D/A={s.Kills}/{s.Deaths}/{s.Assists}");
        }
        else if (c == "4")
        {
            var matchId = PromptInt("Match id: ");
            var resp = await http.GetAsync($"/matches/{matchId}/matchstats");
            if (!resp.IsSuccessStatusCode) { await ShowError(resp); continue; }
            var s = await resp.Content.ReadFromJsonAsync<MatchStat>();
            Console.WriteLine($"{s!.StatId}: match={s.MatchId} char={(s.CharacterId?.ToString() ?? "null")} K/D/A={s.Kills}/{s.Deaths}/{s.Assists}");
        }
        else if (c == "5")
        {
            var id = PromptInt("Stat id to update: ");

            // fetch existing first
            var getResp = await http.GetAsync($"/matchstats/{id}");
            if (!getResp.IsSuccessStatusCode) { await ShowError(getResp); continue; }
            var existing = await getResp.Content.ReadFromJsonAsync<MatchStat>();
            if (existing is null) { Console.WriteLine("No stat returned."); continue; }

            Console.WriteLine("Press Enter to keep existing value.");

            var kStr = Prompt($"Kills ({existing.Kills}): ");
            var dStr = Prompt($"Deaths ({existing.Deaths}): ");
            var aStr = Prompt($"Assists ({existing.Assists}): ");

            int k = string.IsNullOrWhiteSpace(kStr) ? existing.Kills : int.Parse(kStr);
            int d = string.IsNullOrWhiteSpace(dStr) ? existing.Deaths : int.Parse(dStr);
            int a = string.IsNullOrWhiteSpace(aStr) ? existing.Assists : int.Parse(aStr);

            var updated = existing with
            {
                StatId = id,
                Kills = k,
                Deaths = d,
                Assists = a
            };

            var resp = await http.PutAsJsonAsync($"/matchstats/{id}", updated);
            if (!resp.IsSuccessStatusCode) { await ShowError(resp); continue; }
            var rows = await resp.Content.ReadFromJsonAsync<int>();
            Console.WriteLine($"Updated rows={rows}");
        }
        else if (c == "6")
        {
            var id = PromptInt("Stat id: ");
            var resp = await http.DeleteAsync($"/matchstats/{id}");
            if (!resp.IsSuccessStatusCode) { await ShowError(resp); continue; }
            var rows = await resp.Content.ReadFromJsonAsync<int>();
            Console.WriteLine($"Deleted rows={rows}");
        }
        else Console.WriteLine("Invalid choice.");
    }
}