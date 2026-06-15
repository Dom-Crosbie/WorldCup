namespace WorldCup.Web.Models;

public class Match
{
    public Guid MatchId { get; set; }
    public string HomeTeam { get; set; } = string.Empty;
    public string AwayTeam { get; set; } = string.Empty;
    public int HomeScore { get; set; }
    public int AwayScore { get; set; }
    public DateTime MatchDate { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class TableEntry
{
    public string Team { get; set; } = string.Empty;
    public int Played { get; set; }
    public int Won { get; set; }
    public int Drawn { get; set; }
    public int Lost { get; set; }
    public int GoalsFor { get; set; }
    public int GoalsAgainst { get; set; }
    public int GoalDifference { get; set; }
    public int Points { get; set; }
}

public class SubmitScoreViewModel
{
    public string HomeTeam { get; set; } = string.Empty;
    public string AwayTeam { get; set; } = string.Empty;
    public int HomeScore { get; set; }
    public int AwayScore { get; set; }
    public DateTime MatchDate { get; set; } = DateTime.UtcNow;
}

public class DashboardViewModel
{
    public List<Match> RecentMatches { get; set; } = new();
    public List<TableEntry> Table { get; set; } = new();
    public SubmitScoreViewModel NewMatch { get; set; } = new();
    public string? SuccessMessage { get; set; }
    public string? ErrorMessage { get; set; }
}
