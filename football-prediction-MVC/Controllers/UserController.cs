using football_prediction_MVC.Models.ViewModels;
using football_prediction_MVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace football_prediction_MVC.Controllers;

[Authorize(Policy = "RequireUserRole")]
public class UserController : Controller
{
    private readonly IDataService _dataService;

    public UserController(IDataService dataService)
    {
        _dataService = dataService;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var model = new UserDataViewModel();
        try
        {
            model.Stats = await _dataService.GetStatsAsync(cancellationToken);
        }
        catch
        {
            model.ErrorMessage = "Could not load the statistics. Make sure the API is running.";
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SearchMatches(UserDataViewModel model, CancellationToken cancellationToken)
    {
        model.MatchPage = Math.Max(1, model.MatchPage);
        model.MatchPageSize = model.MatchPageSize <= 0 ? 50 : model.MatchPageSize;

        try
        {
            model.Stats = await _dataService.GetStatsAsync(cancellationToken);

            var limit = model.MatchPage * model.MatchPageSize;
            model.MatchQuery.Limit = Math.Min(limit, 5000);
            var matches = await _dataService.GetMatchesAsync(model.MatchQuery, cancellationToken);
            model.MatchFetchedCount = matches.Count;
            model.Matches = matches.Skip((model.MatchPage - 1) * model.MatchPageSize).Take(model.MatchPageSize).ToList();
            model.InfoMessage = $"Loaded {model.Matches.Count} matches (page {model.MatchPage}).";
        }
        catch
        {
            model.ErrorMessage = "Match search failed. Check the API and the filters.";
        }

        return View("Index", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SearchElo(UserDataViewModel model, CancellationToken cancellationToken)
    {
        model.EloPage = Math.Max(1, model.EloPage);
        model.EloPageSize = model.EloPageSize <= 0 ? 50 : model.EloPageSize;

        try
        {
            model.Stats = await _dataService.GetStatsAsync(cancellationToken);

            var limit = model.EloPage * model.EloPageSize;
            model.EloQuery.Limit = Math.Min(limit, 5000);
            var ratings = await _dataService.GetEloRatingsAsync(model.EloQuery, cancellationToken);
            model.EloFetchedCount = ratings.Count;
            model.EloRatings = ratings.Skip((model.EloPage - 1) * model.EloPageSize).Take(model.EloPageSize).ToList();
            model.InfoMessage = $"Loaded {model.EloRatings.Count} Elo rows (page {model.EloPage}).";
        }
        catch
        {
            model.ErrorMessage = "Elo search failed. Check the API and the filters.";
        }

        return View("Index", model);
    }

    public IActionResult Profile()
    {
        return View();
    }
}