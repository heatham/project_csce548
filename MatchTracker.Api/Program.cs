using MatchTracker.Business;
using MatchTracker;

var builder = WebApplication.CreateBuilder(args);

// Register business layer
builder.Services.AddSingleton<MatchTrackerService>();
builder.Services.AddControllers();  // REQUIRED
builder.Services.AddEndpointsApiExplorer(); // for swagger

var app = builder.Build();

/*
HOSTING INSTRUCTIONS (Project 2 requirement):
- Run locally:   dotnet run --project MatchTracker.Api
- The API will host on Kestrel and print the URL(s) in the console.
- Example: https://localhost:5001 and http://localhost:5000
- You can host on IIS or Docker later if desired, but local Kestrel is acceptable for demonstrating service hosting.
*/

// ---- Games ----
app.MapPost("/games", (MatchTrackerService svc, string name) => Results.Ok(svc.CreateGame(name)));
app.MapGet("/games", (MatchTrackerService svc) => Results.Ok(svc.GetAllGames()));
app.MapGet("/games/{id:int}", (MatchTrackerService svc, int id) =>
	svc.GetGame(id) is { } g ? Results.Ok(g) : Results.NotFound());
app.MapPut("/games/{id:int}", (MatchTrackerService svc, int id, string newName) =>
	Results.Ok(svc.UpdateGame(id, newName)));
app.MapDelete("/games/{id:int}", (MatchTrackerService svc, int id) =>
	Results.Ok(svc.DeleteGame(id)));

// ---- Maps ----
app.MapPost("/maps", (MatchTrackerService svc, int gameId, string name) => Results.Ok(svc.CreateMap(gameId, name)));
app.MapGet("/maps", (MatchTrackerService svc) => Results.Ok(svc.GetAllMaps()));
app.MapGet("/maps/{id:int}", (MatchTrackerService svc, int id) =>
	svc.GetMap(id) is { } m ? Results.Ok(m) : Results.NotFound());
app.MapGet("/games/{gameId:int}/maps", (MatchTrackerService svc, int gameId) => Results.Ok(svc.GetMapsByGame(gameId)));
app.MapPut("/maps/{id:int}", (MatchTrackerService svc, int id, string newName) => Results.Ok(svc.UpdateMap(id, newName)));
app.MapDelete("/maps/{id:int}", (MatchTrackerService svc, int id) => Results.Ok(svc.DeleteMap(id)));

// ---- Characters ----
app.MapPost("/characters", (MatchTrackerService svc, int gameId, string name, string? role) =>
	Results.Ok(svc.CreateCharacter(gameId, name, role)));
app.MapGet("/characters", (MatchTrackerService svc) => Results.Ok(svc.GetAllCharacters()));
app.MapGet("/characters/{id:int}", (MatchTrackerService svc, int id) =>
	svc.GetCharacter(id) is { } c ? Results.Ok(c) : Results.NotFound());
app.MapGet("/games/{gameId:int}/characters", (MatchTrackerService svc, int gameId) =>
	Results.Ok(svc.GetCharactersByGame(gameId)));
app.MapPut("/characters/{id:int}", (MatchTrackerService svc, int id, string newName, string? newRole) =>
	Results.Ok(svc.UpdateCharacter(id, newName, newRole)));
app.MapDelete("/characters/{id:int}", (MatchTrackerService svc, int id) => Results.Ok(svc.DeleteCharacter(id)));

// ---- Matches ----
app.MapPost("/matches", (MatchTrackerService svc, Match m) => Results.Ok(svc.CreateMatch(m)));
app.MapGet("/matches", (MatchTrackerService svc) => Results.Ok(svc.GetAllMatches()));
app.MapGet("/matches/{id:int}", (MatchTrackerService svc, int id) =>
	svc.GetMatch(id) is { } m ? Results.Ok(m) : Results.NotFound());
app.MapGet("/games/{gameId:int}/matches", (MatchTrackerService svc, int gameId) => Results.Ok(svc.GetMatchesByGame(gameId)));
app.MapPut("/matches/{id:int}", (MatchTrackerService svc, int id, Match m) =>
	Results.Ok(svc.UpdateMatch(m with { MatchId = id })));
app.MapDelete("/matches/{id:int}", (MatchTrackerService svc, int id) => Results.Ok(svc.DeleteMatch(id)));

// ---- MatchStats ----
app.MapPost("/matchstats", (MatchTrackerService svc, MatchStat s) => Results.Ok(svc.CreateMatchStat(s)));
app.MapGet("/matchstats", (MatchTrackerService svc) => Results.Ok(svc.GetAllMatchStats()));
app.MapGet("/matchstats/{id:int}", (MatchTrackerService svc, int id) =>
	svc.GetMatchStat(id) is { } s ? Results.Ok(s) : Results.NotFound());
app.MapGet("/matches/{matchId:int}/matchstats", (MatchTrackerService svc, int matchId) =>
	svc.GetStatsByMatchId(matchId) is { } s ? Results.Ok(s) : Results.NotFound());
app.MapPut("/matchstats/{id:int}", (MatchTrackerService svc, int id, MatchStat s) =>
	Results.Ok(svc.UpdateMatchStat(s with { StatId = id })));
app.MapDelete("/matchstats/{id:int}", (MatchTrackerService svc, int id) => Results.Ok(svc.DeleteMatchStat(id)));

// Joined display endpoint
app.MapGet("/games/{gameId:int}/matches/recent", (MatchTrackerService svc, int gameId, int limit) =>
	Results.Ok(svc.GetRecentMatchesJoined(gameId, limit)));

app.MapControllers(); // for any [ApiController]s you add later (optional for this project, but common in real APIs)

app.Run();