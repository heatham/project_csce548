using Npgsql;

namespace MatchTracker;

public static class Repository
{
    public static int GetGameIdByName(string gameName)
    {
        using var conn = Db.Open();
        using var cmd = new NpgsqlCommand(@"
            SELECT game_id FROM games WHERE name = @name;
        ", conn);
        cmd.Parameters.AddWithValue("name", gameName);

        var result = cmd.ExecuteScalar();
        if (result is null)
            throw new Exception($"Game '{gameName}' not found. Check seed data or insert it.");
        return (int)result;
    }

    public static List<(int Id, string Name)> ListMaps(int gameId)
    {
        using var conn = Db.Open();
        using var cmd = new NpgsqlCommand(@"
            SELECT map_id, name
            FROM maps
            WHERE game_id = @gameId
            ORDER BY name;
        ", conn);
        cmd.Parameters.AddWithValue("gameId", gameId);

        using var reader = cmd.ExecuteReader();
        var maps = new List<(int, string)>();
        while (reader.Read())
            maps.Add((reader.GetInt32(0), reader.GetString(1)));
        return maps;
    }

    public static List<(int Id, string Name)> ListCharacters(int gameId)
    {
        using var conn = Db.Open();
        using var cmd = new NpgsqlCommand(@"
            SELECT character_id, name
            FROM characters
            WHERE game_id = @gameId
            ORDER BY name;
        ", conn);
        cmd.Parameters.AddWithValue("gameId", gameId);

        using var reader = cmd.ExecuteReader();
        var chars = new List<(int, string)>();
        while (reader.Read())
            chars.Add((reader.GetInt32(0), reader.GetString(1)));
        return chars;
    }

    // READ: list recent matches with stats
    public static List<MatchListRow> ListRecentMatches(int gameId, int limit = 10)
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

        using var reader = cmd.ExecuteReader();
        var rows = new List<MatchListRow>();
        while (reader.Read())
        {
            rows.Add(new MatchListRow(
                reader.GetInt32(0),
                reader.GetDateTime(1),
                reader.GetString(2),
                reader.GetString(3),
                reader.GetString(4)[0],
                reader.GetInt32(5),
                reader.GetString(6),
                reader.GetInt32(7),
                reader.GetInt32(8),
                reader.GetInt32(9)
            ));
        }
        return rows;
    }

    // CREATE: insert match + stats (transaction)
    public static int AddMatchWithStats(
        int gameId,
        DateTime matchDate,
        string queueType,
        int mapId,
        char result,
        int durationSec,
        string? notes,
        int characterId,
        int kills,
        int deaths,
        int assists,
        int? damage,
        int? healing,
        int? objectiveTimeSec
    )
    {
        using var conn = Db.Open();
        using var tx = conn.BeginTransaction();

        try
        {
            int matchId;
            using (var cmdMatch = new NpgsqlCommand(@"
                INSERT INTO matches(game_id, match_date, queue_type, map_id, result, duration_sec, notes)
                VALUES (@gameId, @matchDate, @queueType, @mapId, @result, @durationSec, @notes)
                RETURNING match_id;
            ", conn, tx))
            {
                cmdMatch.Parameters.AddWithValue("gameId", gameId);
                cmdMatch.Parameters.AddWithValue("matchDate", matchDate);
                cmdMatch.Parameters.AddWithValue("queueType", queueType);
                cmdMatch.Parameters.AddWithValue("mapId", mapId);
                cmdMatch.Parameters.AddWithValue("result", result.ToString());
                cmdMatch.Parameters.AddWithValue("durationSec", durationSec);
                cmdMatch.Parameters.AddWithValue("notes", (object?)notes ?? DBNull.Value);

                matchId = (int)cmdMatch.ExecuteScalar()!;
            }

            using (var cmdStats = new NpgsqlCommand(@"
                INSERT INTO match_stats(match_id, character_id, kills, deaths, assists, damage, healing, objective_time_sec)
                VALUES (@matchId, @characterId, @kills, @deaths, @assists, @damage, @healing, @obj);
            ", conn, tx))
            {
                cmdStats.Parameters.AddWithValue("matchId", matchId);
                cmdStats.Parameters.AddWithValue("characterId", characterId);
                cmdStats.Parameters.AddWithValue("kills", kills);
                cmdStats.Parameters.AddWithValue("deaths", deaths);
                cmdStats.Parameters.AddWithValue("assists", assists);
                cmdStats.Parameters.AddWithValue("damage", (object?)damage ?? DBNull.Value);
                cmdStats.Parameters.AddWithValue("healing", (object?)healing ?? DBNull.Value);
                cmdStats.Parameters.AddWithValue("obj", (object?)objectiveTimeSec ?? DBNull.Value);

                cmdStats.ExecuteNonQuery();
            }

            tx.Commit();
            return matchId;
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }

    // UPDATE: update match result/duration/notes
    public static int UpdateMatch(int matchId, char result, int durationSec, string? notes)
    {
        using var conn = Db.Open();
        using var cmd = new NpgsqlCommand(@"
            UPDATE matches
            SET result = @result,
                duration_sec = @durationSec,
                notes = @notes
            WHERE match_id = @matchId;
        ", conn);

        cmd.Parameters.AddWithValue("result", result.ToString());
        cmd.Parameters.AddWithValue("durationSec", durationSec);
        cmd.Parameters.AddWithValue("notes", (object?)notes ?? DBNull.Value);
        cmd.Parameters.AddWithValue("matchId", matchId);

        return cmd.ExecuteNonQuery();
    }

    // UPDATE: update stats
    public static int UpdateStats(int matchId, int kills, int deaths, int assists)
    {
        using var conn = Db.Open();
        using var cmd = new NpgsqlCommand(@"
            UPDATE match_stats
            SET kills = @kills, deaths = @deaths, assists = @assists
            WHERE match_id = @matchId;
        ", conn);

        cmd.Parameters.AddWithValue("kills", kills);
        cmd.Parameters.AddWithValue("deaths", deaths);
        cmd.Parameters.AddWithValue("assists", assists);
        cmd.Parameters.AddWithValue("matchId", matchId);

        return cmd.ExecuteNonQuery();
    }

    // DELETE: delete match (stats cascade)
    public static int DeleteMatch(int matchId)
    {
        using var conn = Db.Open();
        using var cmd = new NpgsqlCommand(@"
            DELETE FROM matches WHERE match_id = @matchId;
        ", conn);
        cmd.Parameters.AddWithValue("matchId", matchId);

        return cmd.ExecuteNonQuery();
    }

    // Retrieval/Analysis query: overall win rate
    public static decimal GetWinRatePercent(int gameId)
    {
        using var conn = Db.Open();
        using var cmd = new NpgsqlCommand(@"
            SELECT
              CASE WHEN COUNT(*) = 0 THEN 0
                   ELSE ROUND(100.0 * SUM(CASE WHEN result='W' THEN 1 ELSE 0 END) / COUNT(*), 2)
              END AS win_rate
            FROM matches
            WHERE game_id = @gameId;
        ", conn);
        cmd.Parameters.AddWithValue("gameId", gameId);

        return (decimal)cmd.ExecuteScalar()!;
    }

    // Retrieval/Analysis query: win rate by character (top 10 by games played)
    public static List<(string Character, int Games, int Wins, decimal WinRate)> WinRateByCharacter(int gameId)
    {
        using var conn = Db.Open();
        using var cmd = new NpgsqlCommand(@"
            SELECT
              c.name,
              COUNT(*) AS games_played,
              SUM(CASE WHEN m.result='W' THEN 1 ELSE 0 END) AS wins,
              ROUND(100.0 * SUM(CASE WHEN m.result='W' THEN 1 ELSE 0 END) / COUNT(*), 2) AS win_rate
            FROM match_stats s
            JOIN matches m ON m.match_id = s.match_id
            JOIN characters c ON c.character_id = s.character_id
            WHERE m.game_id = @gameId
            GROUP BY c.name
            ORDER BY games_played DESC
            LIMIT 10;
        ", conn);
        cmd.Parameters.AddWithValue("gameId", gameId);

        using var reader = cmd.ExecuteReader();
        var rows = new List<(string, int, int, decimal)>();
        while (reader.Read())
        {
            rows.Add((
                reader.GetString(0),
                reader.GetInt32(1),
                reader.GetInt32(2),
                reader.GetDecimal(3)
            ));
        }
        return rows;
    }
}