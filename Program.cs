namespace MatchTracker;

public class Program
{
    public static void Main()
    {
        Console.WriteLine("=== Match Tracker (PostgreSQL + C#) ===");

        // This must match what’s in your seed data.
        // If you changed it to "Marvel Rivals", update here too.
        const string gameName = "Avoid Duplicates Demo Game";

        int gameId;
        try
        {
            gameId = Repository.GetGameIdByName(gameName);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine("Tip: Make sure you've run the SQL seed script.");
            return;
        }

        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("Menu:");
            Console.WriteLine("1) List recent matches");
            Console.WriteLine("2) Add a match + stats");
            Console.WriteLine("3) Update match result/duration/notes");
            Console.WriteLine("4) Update match stats (K/D/A)");
            Console.WriteLine("5) Delete a match");
            Console.WriteLine("6) Show overall win rate");
            Console.WriteLine("7) Show win rate by character (top 10)");
            Console.WriteLine("0) Exit");
            Console.Write("Choose: ");

            var choice = Console.ReadLine()?.Trim();
            try
            {
                switch (choice)
                {
                    case "1": ListRecent(gameId); break;
                    case "2": AddMatch(gameId); break;
                    case "3": UpdateMatch(); break;
                    case "4": UpdateStats(); break;
                    case "5": DeleteMatch(); break;
                    case "6": ShowWinRate(gameId); break;
                    case "7": ShowWinRateByCharacter(gameId); break;
                    case "0": return;
                    default: Console.WriteLine("Invalid choice."); break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
            }
        }
    }

    static void ListRecent(int gameId)
    {
        Console.Write("How many matches to list? (default 10): ");
        var s = Console.ReadLine();
        int limit = 10;
        if (!string.IsNullOrWhiteSpace(s) && int.TryParse(s, out var n) && n > 0) limit = n;

        var rows = Repository.ListRecentMatches(gameId, limit);

        // Fixed column widths
        const int wId = 8;
        const int wDate = 16;    // yyyy-MM-dd HH:mm
        const int wQueue = 10;
        const int wMap = 12;
        const int wRes = 3;
        const int wDur = 6;
        const int wChar = 14;
        const int wK = 4;
        const int wD = 4;
        const int wA = 4;

        static string Fit(string text, int width)
        {
            text ??= "";
            if (text.Length > width) return text.Substring(0, width - 1) + "…"; // keeps alignment
            return text.PadRight(width);
        }

        Console.WriteLine();
        Console.WriteLine(
            $"{Fit("match_id", wId)}| {Fit("date", wDate)} | {Fit("queue", wQueue)} | {Fit("map", wMap)} | {Fit("R", wRes)} | {Fit("dur", wDur)} | {Fit("character", wChar)} |" +
            $" {Fit("K", wK)}{Fit("D", wD)}{Fit("A", wA)}"
        );

        Console.WriteLine(new string('-', wId + wDate + wQueue + wMap + wRes + wDur + wChar + wK + wD + wA + 22));

        foreach (var r in rows)
        {
            var dateStr = r.MatchDate.ToString("yyyy-MM-dd HH:mm");

            Console.WriteLine(
                $"{r.MatchId,wId}|" +
                $" {Fit(dateStr, wDate)} |" +
                $" {Fit(r.QueueType, wQueue)} |" +
                $" {Fit(r.MapName, wMap)} |" +
                $" {Fit(r.Result.ToString(), wRes)} |" +
                $" {r.DurationSec,wDur} |" +
                $" {Fit(r.CharacterName, wChar)} |" +
                $" {r.Kills,wK}{r.Deaths,wD}{r.Assists,wA}"
            );
        }

        Console.WriteLine();
        Console.WriteLine($"Displayed {rows.Count} row(s).");
    }

    static void AddMatch(int gameId)
    {
        var maps = Repository.ListMaps(gameId);
        var chars = Repository.ListCharacters(gameId);

        Console.WriteLine("\nMaps:");
        for (int i = 0; i < maps.Count; i++)
            Console.WriteLine($"{i + 1}) {maps[i].Name}");
        int mapChoice = ReadInt("Choose map #: ", 1, maps.Count);
        int mapId = maps[mapChoice - 1].Id;

        Console.WriteLine("\nCharacters:");
        for (int i = 0; i < chars.Count; i++)
            Console.WriteLine($"{i + 1}) {chars[i].Name}");
        int charChoice = ReadInt("Choose character #: ", 1, chars.Count);
        int characterId = chars[charChoice - 1].Id;

        Console.Write("Queue type (Quickplay/Ranked/etc): ");
        string queueType = ReadNonEmpty();

        Console.Write("Result (W/L): ");
        char result = ReadCharIn("WL");

        int duration = ReadInt("Duration seconds: ", 0, 100000);

        int kills = ReadInt("Kills: ", 0, 999);
        int deaths = ReadInt("Deaths: ", 0, 999);
        int assists = ReadInt("Assists: ", 0, 999);

        Console.Write("Notes (optional): ");
        string? notes = Console.ReadLine();

        // Optional extra stats
        Console.Write("Damage (optional, blank to skip): ");
        int? damage = ReadNullableInt();
        Console.Write("Healing (optional, blank to skip): ");
        int? healing = ReadNullableInt();
        Console.Write("Objective time seconds (optional, blank to skip): ");
        int? obj = ReadNullableInt();

        int matchId = Repository.AddMatchWithStats(
            gameId,
            DateTime.Now,
            queueType,
            mapId,
            result,
            duration,
            string.IsNullOrWhiteSpace(notes) ? null : notes,
            characterId,
            kills, deaths, assists,
            damage, healing, obj
        );

        Console.WriteLine($"Inserted match_id = {matchId}");
    }

    static void UpdateMatch()
    {
        int matchId = ReadInt("Match ID to update: ", 1, int.MaxValue);
        Console.Write("New result (W/L): ");
        char result = ReadCharIn("WL");
        int duration = ReadInt("New duration seconds: ", 0, 100000);
        Console.Write("New notes (optional): ");
        string? notes = Console.ReadLine();

        int rows = Repository.UpdateMatch(matchId, result, duration, string.IsNullOrWhiteSpace(notes) ? null : notes);
        Console.WriteLine(rows == 0 ? "No match updated (check match_id)." : $"Updated {rows} match row(s).");
    }

    static void UpdateStats()
    {
        int matchId = ReadInt("Match ID to update stats: ", 1, int.MaxValue);
        int kills = ReadInt("New kills: ", 0, 999);
        int deaths = ReadInt("New deaths: ", 0, 999);
        int assists = ReadInt("New assists: ", 0, 999);

        int rows = Repository.UpdateStats(matchId, kills, deaths, assists);
        Console.WriteLine(rows == 0 ? "No stats updated (check match_id has stats)." : $"Updated {rows} stats row(s).");
    }

    static void DeleteMatch()
    {
        int matchId = ReadInt("Match ID to delete: ", 1, int.MaxValue);
        int rows = Repository.DeleteMatch(matchId);
        Console.WriteLine(rows == 0 ? "Nothing deleted (check match_id)." : $"Deleted {rows} match row(s). (stats deleted via cascade)");
    }

    static void ShowWinRate(int gameId)
    {
        var winRate = Repository.GetWinRatePercent(gameId);
        Console.WriteLine($"Overall win rate: {winRate}%");
    }

    static void ShowWinRateByCharacter(int gameId)
    {
        var rows = Repository.WinRateByCharacter(gameId);
        Console.WriteLine("\nCharacter       | Games | Wins | WinRate%");
        Console.WriteLine(new string('-', 45));
        foreach (var r in rows)
            Console.WriteLine($"{r.Character,-14} | {r.Games,5} | {r.Wins,4} | {r.WinRate,7}");
    }

    // -------- helpers --------

    static int ReadInt(string prompt, int min, int max)
    {
        while (true)
        {
            Console.Write(prompt);
            var s = Console.ReadLine();
            if (int.TryParse(s, out var v) && v >= min && v <= max) return v;
            Console.WriteLine($"Enter an integer in range [{min}, {max}].");
        }
    }

    static int? ReadNullableInt()
    {
        var s = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(s)) return null;
        if (int.TryParse(s, out var v) && v >= 0) return v;
        Console.WriteLine("Invalid number; skipping.");
        return null;
    }

    static string ReadNonEmpty()
    {
        while (true)
        {
            var s = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(s)) return s.Trim();
            Console.Write("Please enter a non-empty value: ");
        }
    }

    static char ReadCharIn(string allowed)
    {
        while (true)
        {
            var s = Console.ReadLine()?.Trim().ToUpperInvariant();
            if (!string.IsNullOrEmpty(s) && s.Length == 1 && allowed.Contains(s[0]))
                return s[0];
            Console.Write($"Enter one of [{string.Join(',', allowed.ToCharArray())}]: ");
        }
    }
}
