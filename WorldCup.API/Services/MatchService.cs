using WorldCup.API.Models;

namespace WorldCup.API.Services;

public interface IMatchService
{
    IEnumerable<Match> GetAllMatches();
    Match? GetMatch(Guid matchId);
    Match AddMatch(CreateMatchRequest request);
    IEnumerable<TableEntry> GetTable();
    int ClearAll();
}

public class MatchService : IMatchService
{
    private readonly List<Match> _matches = new();
    private readonly object _lock = new();
    private readonly ILogger<MatchService> _logger;

    public MatchService(ILogger<MatchService> logger)
    {
        _logger = logger;
    }

    public IEnumerable<Match> GetAllMatches()
    {
        lock (_lock) return _matches.OrderByDescending(m => m.MatchDate).ToList();
    }

    public Match? GetMatch(Guid matchId)
    {
        lock (_lock) return _matches.FirstOrDefault(m => m.MatchId == matchId);
    }

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

        var lockStartTime = DateTime.UtcNow;
        lock (_lock)
        {
            var addMatchStart = DateTime.UtcNow;
            _matches.Add(match);
            var addMatchTime = (DateTime.UtcNow - addMatchStart).TotalMilliseconds;
            var totalLockTime = (DateTime.UtcNow - lockStartTime).TotalMilliseconds;

            _logger.LogInformation("[TIMING-master] AddMatch breakdown (ms): AddMatch={AddMatchMs} TotalLock={TotalLockMs}",
                addMatchTime, totalLockTime);
        }
        return match;
    }

    public int ClearAll()
    {
        lock (_lock)
        {
            var count = _matches.Count;
            _matches.Clear();
            return count;
        }
    }

    public IEnumerable<TableEntry> GetTable()
    {
        List<Match> snapshot;
        lock (_lock) snapshot = _matches.ToList();

        var entries = new Dictionary<string, TableEntry>();

        foreach (var match in snapshot.Where(m => m.Status == MatchStatus.Completed))
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
