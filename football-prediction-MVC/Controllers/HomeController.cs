using System.Diagnostics;
using football_prediction_MVC.Models;
using football_prediction_MVC.Models.Api;
using football_prediction_MVC.Models.ViewModels;
using football_prediction_MVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace football_prediction_MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IPredictionService _predictionService;
        private readonly IDataService _dataService;

        public HomeController(
            ILogger<HomeController> logger,
            IPredictionService predictionService,
            IDataService dataService)
        {
            _logger = logger;
            _predictionService = predictionService;
            _dataService = dataService;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var model = new PredictionPageViewModel();
            await PopulateStatsAsync(model, cancellationToken);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Predict(PredictionPageViewModel model, CancellationToken cancellationToken)
        {
            await PopulateStatsAsync(model, cancellationToken);

            if (string.IsNullOrWhiteSpace(model.HomeTeam) || string.IsNullOrWhiteSpace(model.AwayTeam))
            {
                model.ErrorMessage = "Fill in both teams before submitting.";
                return View("Index", model);
            }

            try
            {
                var request = new PredictionRequest
                {
                    HomeTeam = model.HomeTeam.Trim(),
                    AwayTeam = model.AwayTeam.Trim()
                };

                model.Result = await _predictionService.PredictAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Prediction failed");
                model.ErrorMessage = "Prediction failed. Check the API and try again.";
            }

            return View("Index", model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private async Task PopulateStatsAsync(PredictionPageViewModel model, CancellationToken cancellationToken)
        {
            try
            {
                model.Stats = await _dataService.GetStatsAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to load dataset stats for hero panel");
            }
        }
    }
}
