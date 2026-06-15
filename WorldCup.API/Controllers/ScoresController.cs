using Microsoft.AspNetCore.Mvc;
using WorldCup.API.Models;
using WorldCup.API.Services;

namespace WorldCup.API.Controllers;

[ApiController]
[Route("[controller]")]
[Produces("application/json")]
public class ScoresController : ControllerBase
{
    private readonly IMatchService _matchService;
    private readonly ILogger<ScoresController> _logger;

    public ScoresController(IMatchService matchService, ILogger<ScoresController> logger)
    {
        _matchService = matchService;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Match>), StatusCodes.Status200OK)]
    public IActionResult GetScores()
    {
        _logger.LogInformation("Fetching all match scores");
        return Ok(_matchService.GetAllMatches());
    }

    [HttpGet("{matchId:guid}")]
    [ProducesResponseType(typeof(Match), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetScore(Guid matchId)
    {
        var match = _matchService.GetMatch(matchId);
        if (match is null)
            return NotFound(new { message = $"Match {matchId} not found" });
        return Ok(match);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Match), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult PostScore([FromBody] CreateMatchRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _logger.LogInformation("Adding match: {Home} vs {Away}", request.HomeTeam, request.AwayTeam);
        var match = _matchService.AddMatch(request);
        return CreatedAtAction(nameof(GetScore), new { matchId = match.MatchId }, match);
    }
}
