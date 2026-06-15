using WorldCup.API.Models;

namespace WorldCup.API.Services;

public interface IMatchService
{
    IEnumerable<Match> GetAllMatches();
    Match? GetMatch(Guid matchId);
    Match AddMatch(CreateMatchRequest request);
    IEnumerable<TableEntry> GetTable();
}

public class MatchService : IMatchService
{
    private readonly List<Match> _matches = new()
    {
        new Match { HomeTeam = "England", AwayTeam = "France", HomeScore = 1, AwayScore = 2, MatchDate = DateTime.UtcNow.AddDays(-3), Status = MatchStatus.Completed },
        new Match { HomeTeam = "Germany", AwayTeam = "Brazil", HomeScore = 2, AwayScore = 2, MatchDate = DateTime.UtcNow.AddDays(-2), Status = MatchStatus.Completed },
        new Match { HomeTeam = "Spain", AwayTeam = "Argentina", HomeScore = 0, AwayScore = 1, MatchDate = DateTime.UtcNow.AddDays(-1), Status = MatchStatus.Completed },
    };

    public IEnumerable<Match> GetAllMatches() => _matches.OrderByDescending(m => m.MatchDate);

    public Match? GetMatch(Guid matchId) => _matches.FirstOrDefault(m => m.MatchId == matchId);

    public Match AddMatch(CreateMatchRequest request)
    {
        var match = new Match
        {
            HomeTeam = request.HomeTeam,
            AwayTeam = request.AwayTeam,
            HomeScore = request.HomeScore,
            AwayScore = request.AwayScore,
            MatchDate = request.MatchDate,
            Status = MatchStatus.Completed
        };
        _matches.Add(match);
        return match;
    }

    public IEnumerable<TableEntry> GetTable()
    {
        var entries = new Dictionary<string, TableEntry>();

        foreach (var match in _matches.Where(m => m.Status == MatchStatus.Completed))
        {
            if (!entries.ContainsKey(match.HomeTeam))
                entries[match.HomeTeam] = new TableEntry { Team = match.HomeTeam };
            if (!entries.ContainsKey(match.AwayTeam))
                entries[match.AwayTeam] = new TableEntry { Team = match.AwayTeam };

            var home = entries[match.HomeTeam];
            var away = entries[match.AwayTeam];

            home.Played++; away.Played++;
            home.GoalsFor += match.HomeScore; home.GoalsAgainst += match.AwayScore;
            away.GoalsFor += match.AwayScore; away.GoalsAgainst += match.HomeScore;

            if (match.HomeScore > match.AwayScore)      { home.Won++; away.Lost++; }
            else if (match.HomeScore == match.AwayScore) { home.Drawn++; away.Drawn++; }
            else                                         { away.Won++; home.Lost++; }
        }

        return entries.Values
            .OrderByDescending(e => e.Points)
            .ThenByDescending(e => e.GoalDifference)
            .ThenByDescending(e => e.GoalsFor);
    }
}
