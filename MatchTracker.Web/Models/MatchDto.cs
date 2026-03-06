namespace MatchTracker.Web.Models;

public sealed class MatchDto
{
    public int Id { get; set; }
    public int GameId { get; set; }
    public int PlayerId { get; set; }
    public DateTime PlayedAt { get; set; }
    public bool Win { get; set; }
}