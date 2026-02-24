using Npgsql;

namespace MatchTracker;

public static class Repository
{
    // -------------------------
    // GAMES (CRUD)
    // -------------------------

    public static int CreateGame(string name)
    {
        using var conn = Db.Open();
        using var cmd = new NpgsqlCommand(@"
            INSERT INTO games(name)
            VALUES (@name)
            RETURNING game_id;
        ", conn);
        cmd.Parameters.AddWithValue("name", name);
        return (int)cmd.ExecuteScalar()!;
    }

    public static Game? GetGameById(int gameId)
    {
        using var conn = Db.Open();
        using var cmd = new NpgsqlCommand(@"
            SELECT game_id, name
            FROM games
            WHERE game_id = @id;
        ", conn);
        cmd.Parameters.AddWithValue("id", gameId);

        using var r = cmd.ExecuteReader();
        if (!r.Read()) return null;
        return new Game(r.GetInt32(0), r.GetString(1));
    }

    public static Game? GetGameByName(string name)
    {
        using var conn = Db.Open();
        using var cmd = new NpgsqlCommand(@"
            SELECT game_id, name
            FROM games
            WHERE name = @name;
        ", conn);
        cmd.Parameters.AddWithValue("name", name);

        using var r = cmd.ExecuteReader();
        if (!r.Read()) return null;
        return new Game(r.GetInt32(0), r.GetString(1));
    }

    public static List<Game> GetAllGames()
    {
        using var conn = Db.Open();
        using var cmd = new NpgsqlCommand(@"
            SELECT game_id, name
            FROM games
            ORDER BY name;
        ", conn);

        using var r = cmd.ExecuteReader();
        var list = new List<Game>();
        while (r.Read())
            list.Add(new Game(r.GetInt32(0), r.GetString(1)));
        return list;
    }

    public static int UpdateGame(int gameId, string newName)
    {
        using var conn = Db.Open();
        using var cmd = new NpgsqlCommand(@"
            UPDATE games
            SET name = @name
            WHERE game_id = @id;
        ", conn);
        cmd.Parameters.AddWithValue("name", newName);
        cmd.Parameters.AddWithValue("id", gameId);
        return cmd.ExecuteNonQuery();
    }

    public static int DeleteGame(int gameId)
    {
        using var conn = Db.Open();
        using var cmd = new NpgsqlCommand(@"
            DELETE FROM games
            WHERE game_id = @id;
        ", conn);
        cmd.Parameters.AddWithValue("id", gameId);
        return cmd.ExecuteNonQuery();
    }

    // -------------------------
    // MAPS (CRUD)
    // -------------------------

    public static int CreateMap(int gameId, string name)
    {
        using var conn = Db.Open();
        using var cmd = new NpgsqlCommand(@"
            INSERT INTO maps(game_id, name)
            VALUES (@gameId, @name)
            RETURNING map_id;
        ", conn);
        cmd.Parameters.AddWithValue("gameId", gameId);
        cmd.Parameters.AddWithValue("name", name);
        return (int)cmd.ExecuteScalar()!;
    }

    public static Map? GetMapById(int mapId)
    {
        using var conn = Db.Open();
        using var cmd = new NpgsqlCommand(@"
            SELECT map_id, game_id, name
            FROM maps
            WHERE map_id = @id;
        ", conn);
        cmd.Parameters.AddWithValue("id", mapId);

        using var r = cmd.ExecuteReader();
        if (!r.Read()) return null;
        return new Map(r.GetInt32(0), r.GetInt32(1), r.GetString(2));
    }

    public static List<Map> GetAllMaps()
    {
        using var conn = Db.Open();
        using var cmd = new NpgsqlCommand(@"
            SELECT map_id, game_id, name
            FROM maps
            ORDER BY game_id, name;
        ", conn);

        using var r = cmd.ExecuteReader();
        var list = new List<Map>();
        while (r.Read())
            list.Add(new Map(r.GetInt32(0), r.GetInt32(1), r.GetString(2)));
        return list;
    }

    public static List<Map> GetMapsByGame(int gameId)
    {
        using var conn = Db.Open();
        using var cmd = new NpgsqlCommand(@"
            SELECT map_id, game_id, name
            FROM maps
            WHERE game_id = @gameId
            ORDER BY name;
        ", conn);
        cmd.Parameters.AddWithValue("gameId", gameId);

        using var r = cmd.ExecuteReader();
        var list = new List<Map>();
        while (r.Read())
            list.Add(new Map(r.GetInt32(0), r.GetInt32(1), r.GetString(2)));
        return list;
    }

    public static int UpdateMap(int mapId, string newName)
    {
        using var conn = Db.Open();
        using var cmd = new NpgsqlCommand(@"
            UPDATE maps
            SET name = @name
            WHERE map_id = @id;
        ", conn);
        cmd.Parameters.AddWithValue("name", newName);
        cmd.Parameters.AddWithValue("id", mapId);
        return cmd.ExecuteNonQuery();
    }

    public static int DeleteMap(int mapId)
    {
        using var conn = Db.Open();
        using var cmd = new NpgsqlCommand(@"
            DELETE FROM maps
            WHERE map_id = @id;
        ", conn);
        cmd.Parameters.AddWithValue("id", mapId);
        return cmd.ExecuteNonQuery();
    }

    // -------------------------
    // CHARACTERS (CRUD)
    // -------------------------

    public static int CreateCharacter(int gameId, string name, string? role)
    {
        using var conn = Db.Open();
        using var cmd = new NpgsqlCommand(@"
            INSERT INTO characters(game_id, name, role)
            VALUES (@gameId, @name, @role)
            RETURNING character_id;
        ", conn);
        cmd.Parameters.AddWithValue("gameId", gameId);
        cmd.Parameters.AddWithValue("name", name);
        cmd.Parameters.AddWithValue("role", (object?)role ?? DBNull.Value);
        return (int)cmd.ExecuteScalar()!;
    }

    public static Character? GetCharacterById(int characterId)
    {
        using var conn = Db.Open();
        using var cmd = new NpgsqlCommand(@"
            SELECT character_id, game_id, name, role
            FROM characters
            WHERE character_id = @id;
        ", conn);
        cmd.Parameters.AddWithValue("id", characterId);

        using var r = cmd.ExecuteReader();
        if (!r.Read()) return null;

        return new Character(
            r.GetInt32(0),
            r.GetInt32(1),
            r.GetString(2),
            r.IsDBNull(3) ? null : r.GetString(3)
        );
    }

    public static List<Character> GetAllCharacters()
    {
        using var conn = Db.Open();
        using var cmd = new NpgsqlCommand(@"
            SELECT character_id, game_id, name, role
            FROM characters
            ORDER BY game_id, name;
        ", conn);

        using var r = cmd.ExecuteReader();
        var list = new List<Character>();
        while (r.Read())
        {
            list.Add(new Character(
                r.GetInt32(0),
                r.GetInt32(1),
                r.GetString(2),
                r.IsDBNull(3) ? null : r.GetString(3)
            ));
        }
        return list;
    }

    public static List<Character> GetCharactersByGame(int gameId)
    {
        using var conn = Db.Open();
        using var cmd = new NpgsqlCommand(@"
            SELECT character_id, game_id, name, role
            FROM characters
            WHERE game_id = @gameId
            ORDER BY name;
        ", conn);
        cmd.Parameters.AddWithValue("gameId", gameId);

        using var r = cmd.ExecuteReader();
        var list = new List<Character>();
        while (r.Read())
        {
            list.Add(new Character(
                r.GetInt32(0),
                r.GetInt32(1),
                r.GetString(2),
                r.IsDBNull(3) ? null : r.GetString(3)
            ));
        }
        return list;
    }

    public static int UpdateCharacter(int characterId, string newName, string? newRole)
    {
        using var conn = Db.Open();
        using var cmd = new NpgsqlCommand(@"
            UPDATE characters
            SET name = @name,
                role = @role
            WHERE character_id = @id;
        ", conn);
        cmd.Parameters.AddWithValue("name", newName);
        cmd.Parameters.AddWithValue("role", (object?)newRole ?? DBNull.Value);
        cmd.Parameters.AddWithValue("id", characterId);
        return cmd.ExecuteNonQuery();
    }

    public static int DeleteCharacter(int characterId)
    {
        using var conn = Db.Open();
        using var cmd = new NpgsqlCommand(@"
            DELETE FROM characters
            WHERE character_id = @id;
        ", conn);
        cmd.Parameters.AddWithValue("id", characterId);
        return cmd.ExecuteNonQuery();
    }

    // -------------------------
    // MATCHES (CRUD)  <-- NOTE: UpdateMatch takes a Match object (not matchId/result/etc)
    // -------------------------

    public static int CreateMatch(Match m)
    {
        using var conn = Db.Open();
        using var cmd = new NpgsqlCommand(@"
            INSERT INTO matches(game_id, match_date, queue_type, map_id, result, duration_sec, notes)
            VALUES (@gameId, @matchDate, @queueType, @mapId, @result, @durationSec, @notes)
            RETURNING match_id;
        ", conn);

        cmd.Parameters.AddWithValue("gameId", m.GameId);
        cmd.Parameters.AddWithValue("matchDate", m.MatchDate);
        cmd.Parameters.AddWithValue("queueType", m.QueueType);
        cmd.Parameters.AddWithValue("mapId", (object?)m.MapId ?? DBNull.Value);
        cmd.Parameters.AddWithValue("result", m.Result.ToString());
        cmd.Parameters.AddWithValue("durationSec", (object?)m.DurationSec ?? DBNull.Value);
        cmd.Parameters.AddWithValue("notes", (object?)m.Notes ?? DBNull.Value);

        return (int)cmd.ExecuteScalar()!;
    }

    public static Match? GetMatchById(int matchId)
    {
        using var conn = Db.Open();
        using var cmd = new NpgsqlCommand(@"
            SELECT match_id, game_id, match_date, queue_type, map_id, result, duration_sec, notes
            FROM matches
            WHERE match_id = @id;
        ", conn);
        cmd.Parameters.AddWithValue("id", matchId);

        using var r = cmd.ExecuteReader();
        if (!r.Read()) return null;

        return new Match(
            r.GetInt32(0),
            r.GetInt32(1),
            r.GetDateTime(2),
            r.GetString(3),
            r.IsDBNull(4) ? null : r.GetInt32(4),
            r.GetString(5)[0],
            r.IsDBNull(6) ? null : r.GetInt32(6),
            r.IsDBNull(7) ? null : r.GetString(7)
        );
    }

    public static List<Match> GetAllMatches()
    {
        using var conn = Db.Open();
        using var cmd = new NpgsqlCommand(@"
            SELECT match_id, game_id, match_date, queue_type, map_id, result, duration_sec, notes
            FROM matches
            ORDER BY match_date DESC, match_id DESC;
        ", conn);

        using var r = cmd.ExecuteReader();
        var list = new List<Match>();
        while (r.Read())
        {
            list.Add(new Match(
                r.GetInt32(0),
                r.GetInt32(1),
                r.GetDateTime(2),
                r.GetString(3),
                r.IsDBNull(4) ? null : r.GetInt32(4),
                r.GetString(5)[0],
                r.IsDBNull(6) ? null : r.GetInt32(6),
                r.IsDBNull(7) ? null : r.GetString(7)
            ));
        }
        return list;
    }

    public static List<Match> GetMatchesByGame(int gameId)
    {
        using var conn = Db.Open();
        using var cmd = new NpgsqlCommand(@"
            SELECT match_id, game_id, match_date, queue_type, map_id, result, duration_sec, notes
            FROM matches
            WHERE game_id = @gameId
            ORDER BY match_date DESC, match_id DESC;
        ", conn);
        cmd.Parameters.AddWithValue("gameId", gameId);

        using var r = cmd.ExecuteReader();
        var list = new List<Match>();
        while (r.Read())
        {
            list.Add(new Match(
                r.GetInt32(0),
                r.GetInt32(1),
                r.GetDateTime(2),
                r.GetString(3),
                r.IsDBNull(4) ? null : r.GetInt32(4),
                r.GetString(5)[0],
                r.IsDBNull(6) ? null : r.GetInt32(6),
                r.IsDBNull(7) ? null : r.GetString(7)
            ));
        }
        return list;
    }

    public static int UpdateMatch(Match m)
    {
        using var conn = Db.Open();
        using var cmd = new NpgsqlCommand(@"
            UPDATE matches
            SET match_date = @matchDate,
                queue_type = @queueType,
                map_id = @mapId,
                result = @result,
                duration_sec = @durationSec,
                notes = @notes
            WHERE match_id = @matchId;
        ", conn);

        cmd.Parameters.AddWithValue("matchDate", m.MatchDate);
        cmd.Parameters.AddWithValue("queueType", m.QueueType);
        cmd.Parameters.AddWithValue("mapId", (object?)m.MapId ?? DBNull.Value);
        cmd.Parameters.AddWithValue("result", m.Result.ToString());
        cmd.Parameters.AddWithValue("durationSec", (object?)m.DurationSec ?? DBNull.Value);
        cmd.Parameters.AddWithValue("notes", (object?)m.Notes ?? DBNull.Value);
        cmd.Parameters.AddWithValue("matchId", m.MatchId);

        return cmd.ExecuteNonQuery();
    }

    public static int DeleteMatch(int matchId)
    {
        using var conn = Db.Open();
        using var cmd = new NpgsqlCommand(@"DELETE FROM matches WHERE match_id = @id;", conn);
        cmd.Parameters.AddWithValue("id", matchId);
        return cmd.ExecuteNonQuery();
    }

    // -------------------------
    // MATCH_STATS (CRUD)
    // -------------------------

    public static int CreateMatchStat(MatchStat s)
    {
        using var conn = Db.Open();
        using var cmd = new NpgsqlCommand(@"
            INSERT INTO match_stats(match_id, character_id, kills, deaths, assists, damage, healing, objective_time_sec)
            VALUES (@matchId, @characterId, @kills, @deaths, @assists, @damage, @healing, @obj)
            RETURNING stat_id;
        ", conn);

        cmd.Parameters.AddWithValue("matchId", s.MatchId);
        cmd.Parameters.AddWithValue("characterId", (object?)s.CharacterId ?? DBNull.Value);
        cmd.Parameters.AddWithValue("kills", s.Kills);
        cmd.Parameters.AddWithValue("deaths", s.Deaths);
        cmd.Parameters.AddWithValue("assists", s.Assists);
        cmd.Parameters.AddWithValue("damage", (object?)s.Damage ?? DBNull.Value);
        cmd.Parameters.AddWithValue("healing", (object?)s.Healing ?? DBNull.Value);
        cmd.Parameters.AddWithValue("obj", (object?)s.ObjectiveTimeSec ?? DBNull.Value);

        return (int)cmd.ExecuteScalar()!;
    }

    public static MatchStat? GetMatchStatById(int statId)
    {
        using var conn = Db.Open();
        using var cmd = new NpgsqlCommand(@"
            SELECT stat_id, match_id, character_id, kills, deaths, assists, damage, healing, objective_time_sec
            FROM match_stats
            WHERE stat_id = @id;
        ", conn);
        cmd.Parameters.AddWithValue("id", statId);

        using var r = cmd.ExecuteReader();
        if (!r.Read()) return null;

        return new MatchStat(
            r.GetInt32(0),
            r.GetInt32(1),
            r.IsDBNull(2) ? null : r.GetInt32(2),
            r.GetInt32(3),
            r.GetInt32(4),
            r.GetInt32(5),
            r.IsDBNull(6) ? null : r.GetInt32(6),
            r.IsDBNull(7) ? null : r.GetInt32(7),
            r.IsDBNull(8) ? null : r.GetInt32(8)
        );
    }

    public static List<MatchStat> GetAllMatchStats()
    {
        using var conn = Db.Open();
        using var cmd = new NpgsqlCommand(@"
            SELECT stat_id, match_id, character_id, kills, deaths, assists, damage, healing, objective_time_sec
            FROM match_stats
            ORDER BY stat_id DESC;
        ", conn);

        using var r = cmd.ExecuteReader();
        var list = new List<MatchStat>();
        while (r.Read())
        {
            list.Add(new MatchStat(
                r.GetInt32(0),
                r.GetInt32(1),
                r.IsDBNull(2) ? null : r.GetInt32(2),
                r.GetInt32(3),
                r.GetInt32(4),
                r.GetInt32(5),
                r.IsDBNull(6) ? null : r.GetInt32(6),
                r.IsDBNull(7) ? null : r.GetInt32(7),
                r.IsDBNull(8) ? null : r.GetInt32(8)
            ));
        }
        return list;
    }

    public static MatchStat? GetStatsByMatchId(int matchId)
    {
        using var conn = Db.Open();
        using var cmd = new NpgsqlCommand(@"
            SELECT stat_id, match_id, character_id, kills, deaths, assists, damage, healing, objective_time_sec
            FROM match_stats
            WHERE match_id = @matchId;
        ", conn);
        cmd.Parameters.AddWithValue("matchId", matchId);

        using var r = cmd.ExecuteReader();
        if (!r.Read()) return null;

        return new MatchStat(
            r.GetInt32(0),
            r.GetInt32(1),
            r.IsDBNull(2) ? null : r.GetInt32(2),
            r.GetInt32(3),
            r.GetInt32(4),
            r.GetInt32(5),
            r.IsDBNull(6) ? null : r.GetInt32(6),
            r.IsDBNull(7) ? null : r.GetInt32(7),
            r.IsDBNull(8) ? null : r.GetInt32(8)
        );
    }

    public static int UpdateMatchStat(MatchStat s)
    {
        using var conn = Db.Open();
        using var cmd = new NpgsqlCommand(@"
            UPDATE match_stats
            SET character_id = @characterId,
                kills = @kills,
                deaths = @deaths,
                assists = @assists,
                damage = @damage,
                healing = @healing,
                objective_time_sec = @obj
            WHERE stat_id = @statId;
        ", conn);

        cmd.Parameters.AddWithValue("characterId", (object?)s.CharacterId ?? DBNull.Value);
        cmd.Parameters.AddWithValue("kills", s.Kills);
        cmd.Parameters.AddWithValue("deaths", s.Deaths);
        cmd.Parameters.AddWithValue("assists", s.Assists);
        cmd.Parameters.AddWithValue("damage", (object?)s.Damage ?? DBNull.Value);
        cmd.Parameters.AddWithValue("healing", (object?)s.Healing ?? DBNull.Value);
        cmd.Parameters.AddWithValue("obj", (object?)s.ObjectiveTimeSec ?? DBNull.Value);
        cmd.Parameters.AddWithValue("statId", s.StatId);

        return cmd.ExecuteNonQuery();
    }

    public static int DeleteMatchStat(int statId)
    {
        using var conn = Db.Open();
        using var cmd = new NpgsqlCommand(@"DELETE FROM match_stats WHERE stat_id = @id;", conn);
        cmd.Parameters.AddWithValue("id", statId);
        return cmd.ExecuteNonQuery();
    }

    // -------------------------
    // Joined READ for display
    // -------------------------

    public static List<MatchListRow> ListRecentMatchesJoined(int gameId, int limit = 10)
    {
        using var conn = Db.Open();
        using var cmd = new NpgsqlCommand(@"
            SELECT
              m.match_id,
              m.match_date,
              m.queue_type,
              COALESCE(mp.name, 'Unknown') AS map_name,
              m.result,
              COALESCE(m.duration_sec, 0) AS duration_sec,
              COALESCE(c.name, 'Unknown') AS character_name,
              s.kills, s.deaths, s.assists
            FROM matches m
            LEFT JOIN maps mp ON mp.map_id = m.map_id
            LEFT JOIN match_stats s ON s.match_id = m.match_id
            LEFT JOIN characters c ON c.character_id = s.character_id
            WHERE m.game_id = @gameId
            ORDER BY m.match_date DESC, m.match_id DESC
            LIMIT @limit;
        ", conn);

        cmd.Parameters.AddWithValue("gameId", gameId);
        cmd.Parameters.AddWithValue("limit", limit);

        using var r = cmd.ExecuteReader();
        var rows = new List<MatchListRow>();
        while (r.Read())
        {
            rows.Add(new MatchListRow(
                r.GetInt32(0),
                r.GetDateTime(1),
                r.GetString(2),
                r.GetString(3),
                r.GetString(4)[0],
                r.GetInt32(5),
                r.GetString(6),
                r.GetInt32(7),
                r.GetInt32(8),
                r.GetInt32(9)
            ));
        }
        return rows;
    }
}