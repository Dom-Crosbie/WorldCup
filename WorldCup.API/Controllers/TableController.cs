using Microsoft.AspNetCore.Mvc;
using WorldCup.API.Models;
using WorldCup.API.Services;

namespace WorldCup.API.Controllers;

[ApiController]
[Route("[controller]")]
[Produces("application/json")]
public class TableController : ControllerBase
{
    private readonly IMatchService _matchService;

    public TableController(IMatchService matchService) => _matchService = matchService;

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TableEntry>), StatusCodes.Status200OK)]
    public IActionResult GetTable()
    {
        return Ok(_matchService.GetTable());
    }
}
