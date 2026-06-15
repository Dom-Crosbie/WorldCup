using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using WorldCup.Web.Models;

namespace WorldCup.Web.Controllers;

public class HomeController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<HomeController> _logger;
    private const string ApiBase = "http://localhost:6200";

    public HomeController(IHttpClientFactory httpClientFactory, ILogger<HomeController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var vm = new DashboardViewModel();
        var client = _httpClientFactory.CreateClient();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        try
        {
            var matchesResp = await client.GetAsync($"{ApiBase}/scores");
            if (matchesResp.IsSuccessStatusCode)
                vm.RecentMatches = JsonSerializer.Deserialize<List<Match>>(await matchesResp.Content.ReadAsStringAsync(), options) ?? new();

            var tableResp = await client.GetAsync($"{ApiBase}/table");
            if (tableResp.IsSuccessStatusCode)
                vm.Table = JsonSerializer.Deserialize<List<TableEntry>>(await tableResp.Content.ReadAsStringAsync(), options) ?? new();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch data from API");
            vm.ErrorMessage = "Could not connect to the API. Ensure WorldCup.API is running on port 5000.";
        }

        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> SubmitScore(SubmitScoreViewModel model)
    {
        if (!ModelState.IsValid)
            return RedirectToAction(nameof(Index));

        var client = _httpClientFactory.CreateClient();
        var json = JsonSerializer.Serialize(new
        {
            homeTeam = model.HomeTeam,
            awayTeam = model.AwayTeam,
            homeScore = model.HomeScore,
            awayScore = model.AwayScore,
            matchDate = model.MatchDate
        });

        var response = await client.PostAsync($"{ApiBase}/scores",
            new StringContent(json, Encoding.UTF8, "application/json"));

        TempData[response.IsSuccessStatusCode ? "Success" : "Error"] =
            response.IsSuccessStatusCode ? "Match result submitted!" : "Failed to submit match result.";

        return RedirectToAction(nameof(Index));
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
