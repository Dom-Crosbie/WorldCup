using System.ComponentModel.DataAnnotations;

namespace WorldCup.API.Models;

public class CreateMatchRequest
{
    [Required]
    public string HomeTeam { get; set; } = string.Empty;

    [Required]
    public string AwayTeam { get; set; } = string.Empty;

    [Range(0, 99)]
    public int HomeScore { get; set; }

    [Range(0, 99)]
    public int AwayScore { get; set; }

    public DateTime MatchDate { get; set; } = DateTime.UtcNow;
}
