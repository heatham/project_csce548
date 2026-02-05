namespace MatchTracker;

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