namespace MatchTracker;

// --- Table Models (5 tables) ---

public record Game(
    int GameId,
    string Name
);

public record Map(
    int MapId,
    int GameId,
    string Name
);

public record Character(
    int CharacterId,
    int GameId,
    string Name,
    string? Role
);

public record Match(
    int MatchId,
    int GameId,
    DateTime MatchDate,
    string QueueType,
    int? MapId,
    char Result,
    int? DurationSec,
    string? Notes
);

public record MatchStat(
    int StatId,
    int MatchId,
    int? CharacterId,
    int Kills,
    int Deaths,
    int Assists,
    int? Damage,
    int? Healing,
    int? ObjectiveTimeSec
);

// --- Read-only DTO for joined display (not a table) ---
public record MatchListRow(
    int MatchId,
    DateTime MatchDate,
    string QueueType,
    string MapName,
    char Result,
    int DurationSec,
    string CharacterName,
    int Kills,
    int Deaths,
    int Assists
);