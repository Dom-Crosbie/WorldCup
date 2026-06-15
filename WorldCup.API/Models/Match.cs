namespace WorldCup.API.Models;

public class Match
{
    public Guid MatchId { get; set; } = Guid.NewGuid();
    public string HomeTeam { get; set; } = string.Empty;
    public string AwayTeam { get; set; } = string.Empty;
    public int HomeScore { get; set; }
    public int AwayScore { get; set; }
    public DateTime MatchDate { get; set; }
    public MatchStatus Status { get; set; } = MatchStatus.Completed;
}

public enum MatchStatus
{
    Scheduled,
    InProgress,
    Completed
}
