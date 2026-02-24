using System;

namespace MatchTracker.Business;

// NOTE:
// This version assumes your table models and Repository live in namespace: MatchTracker
// i.e., Models.cs starts with: namespace MatchTracker;

public class MatchTrackerService
{
	// -------------------------
	// GAMES
	// -------------------------
	public int CreateGame(string name)
	{
		if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Game name required.");
		return MatchTracker.Repository.CreateGame(name.Trim());
	}

	public MatchTracker.Game? GetGame(int id) => MatchTracker.Repository.GetGameById(id);

	public MatchTracker.Game? GetGameByName(string name)
	{
		if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name required.");
		return MatchTracker.Repository.GetGameByName(name.Trim());
	}

	public System.Collections.Generic.List<MatchTracker.Game> GetAllGames()
		=> MatchTracker.Repository.GetAllGames();

	public int UpdateGame(int id, string newName)
	{
		if (string.IsNullOrWhiteSpace(newName)) throw new ArgumentException("New name required.");
		return MatchTracker.Repository.UpdateGame(id, newName.Trim());
	}

	public int DeleteGame(int id) => MatchTracker.Repository.DeleteGame(id);

	// -------------------------
	// MAPS
	// -------------------------
	public int CreateMap(int gameId, string name)
	{
		if (gameId <= 0) throw new ArgumentException("GameId required.");
		if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Map name required.");
		return MatchTracker.Repository.CreateMap(gameId, name.Trim());
	}

	public MatchTracker.Map? GetMap(int id) => MatchTracker.Repository.GetMapById(id);

	public System.Collections.Generic.List<MatchTracker.Map> GetAllMaps()
		=> MatchTracker.Repository.GetAllMaps();

	public System.Collections.Generic.List<MatchTracker.Map> GetMapsByGame(int gameId)
	{
		if (gameId <= 0) throw new ArgumentException("GameId required.");
		return MatchTracker.Repository.GetMapsByGame(gameId);
	}

	public int UpdateMap(int id, string newName)
	{
		if (string.IsNullOrWhiteSpace(newName)) throw new ArgumentException("New name required.");
		return MatchTracker.Repository.UpdateMap(id, newName.Trim());
	}

	public int DeleteMap(int id) => MatchTracker.Repository.DeleteMap(id);

	// -------------------------
	// CHARACTERS
	// -------------------------
	public int CreateCharacter(int gameId, string name, string? role)
	{
		if (gameId <= 0) throw new ArgumentException("GameId required.");
		if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Character name required.");

		var cleanRole = string.IsNullOrWhiteSpace(role) ? null : role.Trim();
		return MatchTracker.Repository.CreateCharacter(gameId, name.Trim(), cleanRole);
	}

	public MatchTracker.Character? GetCharacter(int id) => MatchTracker.Repository.GetCharacterById(id);

	public System.Collections.Generic.List<MatchTracker.Character> GetAllCharacters()
		=> MatchTracker.Repository.GetAllCharacters();

	public System.Collections.Generic.List<MatchTracker.Character> GetCharactersByGame(int gameId)
	{
		if (gameId <= 0) throw new ArgumentException("GameId required.");
		return MatchTracker.Repository.GetCharactersByGame(gameId);
	}

	public int UpdateCharacter(int id, string newName, string? newRole)
	{
		if (string.IsNullOrWhiteSpace(newName)) throw new ArgumentException("New name required.");
		var cleanRole = string.IsNullOrWhiteSpace(newRole) ? null : newRole.Trim();
		return MatchTracker.Repository.UpdateCharacter(id, newName.Trim(), cleanRole);
	}

	public int DeleteCharacter(int id) => MatchTracker.Repository.DeleteCharacter(id);

	// -------------------------
	// MATCHES
	// -------------------------
	public int CreateMatch(MatchTracker.Match m)
	{
		ValidateMatch(m);
		return MatchTracker.Repository.CreateMatch(m);
	}

	public MatchTracker.Match? GetMatch(int id) => MatchTracker.Repository.GetMatchById(id);

	public System.Collections.Generic.List<MatchTracker.Match> GetAllMatches()
		=> MatchTracker.Repository.GetAllMatches();

	public System.Collections.Generic.List<MatchTracker.Match> GetMatchesByGame(int gameId)
	{
		if (gameId <= 0) throw new ArgumentException("GameId required.");
		return MatchTracker.Repository.GetMatchesByGame(gameId);
	}

	public int UpdateMatch(MatchTracker.Match m)
	{
		if (m.MatchId <= 0) throw new ArgumentException("MatchId required.");
		ValidateMatch(m);
		return MatchTracker.Repository.UpdateMatch(m);
	}

	public int DeleteMatch(int id) => MatchTracker.Repository.DeleteMatch(id);

	// -------------------------
	// MATCH_STATS
	// -------------------------
	public int CreateMatchStat(MatchTracker.MatchStat s)
	{
		ValidateStats(s);
		return MatchTracker.Repository.CreateMatchStat(s);
	}

	public MatchTracker.MatchStat? GetMatchStat(int id) => MatchTracker.Repository.GetMatchStatById(id);

	public System.Collections.Generic.List<MatchTracker.MatchStat> GetAllMatchStats()
		=> MatchTracker.Repository.GetAllMatchStats();

	public MatchTracker.MatchStat? GetStatsByMatchId(int matchId)
	{
		if (matchId <= 0) throw new ArgumentException("MatchId required.");
		return MatchTracker.Repository.GetStatsByMatchId(matchId);
	}

	public int UpdateMatchStat(MatchTracker.MatchStat s)
	{
		if (s.StatId <= 0) throw new ArgumentException("StatId required.");
		ValidateStats(s);
		return MatchTracker.Repository.UpdateMatchStat(s);
	}

	public int DeleteMatchStat(int id) => MatchTracker.Repository.DeleteMatchStat(id);

	// -------------------------
	// Joined read (for display)
	// -------------------------
	public System.Collections.Generic.List<MatchTracker.MatchListRow> GetRecentMatchesJoined(int gameId, int limit = 10)
	{
		if (gameId <= 0) throw new ArgumentException("GameId required.");
		if (limit <= 0) limit = 10;
		return MatchTracker.Repository.ListRecentMatchesJoined(gameId, limit);
	}

	// -------------------------
	// Validation helpers
	// -------------------------
	private static void ValidateMatch(MatchTracker.Match m)
	{
		if (m.GameId <= 0) throw new ArgumentException("GameId required.");
		if (string.IsNullOrWhiteSpace(m.QueueType)) throw new ArgumentException("QueueType required.");
		if (m.Result != 'W' && m.Result != 'L') throw new ArgumentException("Result must be W or L.");
		if (m.DurationSec is < 0) throw new ArgumentException("DurationSec must be >= 0.");
	}

	private static void ValidateStats(MatchTracker.MatchStat s)
	{
		if (s.MatchId <= 0) throw new ArgumentException("MatchId required.");
		if (s.Kills < 0 || s.Deaths < 0 || s.Assists < 0) throw new ArgumentException("K/D/A must be >= 0.");
		if (s.Damage is < 0 || s.Healing is < 0 || s.ObjectiveTimeSec is < 0)
			throw new ArgumentException("Optional stats must be >= 0 when provided.");
	}
}
/*using MatchTracker; // your namespace from Models/Repository
					// If your Data project namespace differs, adjust.

namespace MatchTracker.Business;

public class MatchTrackerService
{
	// --- Games ---
	public int CreateGame(string name)
	{
		if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Game name required.");
		return Repository.CreateGame(name.Trim());
	}

	public Game? GetGame(int id) => Repository.GetGameById(id);
	public List<Game> GetAllGames() => Repository.GetAllGames();
	public int UpdateGame(int id, string newName)
	{
		if (string.IsNullOrWhiteSpace(newName)) throw new ArgumentException("New name required.");
		return Repository.UpdateGame(id, newName.Trim());
	}
	public int DeleteGame(int id) => Repository.DeleteGame(id);

	// --- Maps ---
	public int CreateMap(int gameId, string name)
	{
		if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Map name required.");
		return Repository.CreateMap(gameId, name.Trim());
	}
	public Map? GetMap(int id) => Repository.GetMapById(id);
	public List<Map> GetAllMaps() => Repository.GetAllMaps();
	public List<Map> GetMapsByGame(int gameId) => Repository.GetMapsByGame(gameId);
	public int UpdateMap(int id, string newName) => Repository.UpdateMap(id, newName.Trim());
	public int DeleteMap(int id) => Repository.DeleteMap(id);

	// --- Characters ---
	public int CreateCharacter(int gameId, string name, string? role)
		=> Repository.CreateCharacter(gameId, name.Trim(), string.IsNullOrWhiteSpace(role) ? null : role.Trim());

	public Character? GetCharacter(int id) => Repository.GetCharacterById(id);
	public List<Character> GetAllCharacters() => Repository.GetAllCharacters();
	public List<Character> GetCharactersByGame(int gameId) => Repository.GetCharactersByGame(gameId);
	public int UpdateCharacter(int id, string newName, string? newRole)
		=> Repository.UpdateCharacter(id, newName.Trim(), string.IsNullOrWhiteSpace(newRole) ? null : newRole.Trim());
	public int DeleteCharacter(int id) => Repository.DeleteCharacter(id);

	// --- Matches ---
	public int CreateMatch(Match m)
	{
		ValidateMatch(m);
		return Repository.CreateMatch(m);
	}
	public Match? GetMatch(int id) => Repository.GetMatchById(id);
	public List<Match> GetAllMatches() => Repository.GetAllMatches();
	public List<Match> GetMatchesByGame(int gameId) => Repository.GetMatchesByGame(gameId);
	public int UpdateMatch(Match m)
	{
		if (m.MatchId <= 0) throw new ArgumentException("MatchId required.");
		ValidateMatch(m);
		return Repository.UpdateMatch(m);
	}
	public int DeleteMatch(int id) => Repository.DeleteMatch(id);

	// --- MatchStats ---
	public int CreateMatchStat(MatchStat s)
	{
		ValidateStats(s);
		return Repository.CreateMatchStat(s);
	}
	public MatchStat? GetMatchStat(int id) => Repository.GetMatchStatById(id);
	public List<MatchStat> GetAllMatchStats() => Repository.GetAllMatchStats();
	public MatchStat? GetStatsByMatchId(int matchId) => Repository.GetStatsByMatchId(matchId);
	public int UpdateMatchStat(MatchStat s)
	{
		if (s.StatId <= 0) throw new ArgumentException("StatId required.");
		ValidateStats(s);
		return Repository.UpdateMatchStat(s);
	}
	public int DeleteMatchStat(int id) => Repository.DeleteMatchStat(id);

	// --- Joined read for your table display ---
	public List<MatchListRow> GetRecentMatchesJoined(int gameId, int limit = 10)
		=> Repository.ListRecentMatchesJoined(gameId, limit);

	// --- Simple “business rules” (lightweight but legit) ---
	private static void ValidateMatch(Match m)
	{
		if (m.GameId <= 0) throw new ArgumentException("GameId required.");
		if (string.IsNullOrWhiteSpace(m.QueueType)) throw new ArgumentException("QueueType required.");
		if (m.Result != 'W' && m.Result != 'L') throw new ArgumentException("Result must be W or L.");
		if (m.DurationSec is < 0) throw new ArgumentException("DurationSec must be >= 0.");
	}

	private static void ValidateStats(MatchStat s)
	{
		if (s.MatchId <= 0) throw new ArgumentException("MatchId required.");
		if (s.Kills < 0 || s.Deaths < 0 || s.Assists < 0) throw new ArgumentException("K/D/A must be >= 0.");
		if (s.Damage is < 0 || s.Healing is < 0 || s.ObjectiveTimeSec is < 0)
			throw new ArgumentException("Optional stats must be >= 0 when provided.");
	}
}*/