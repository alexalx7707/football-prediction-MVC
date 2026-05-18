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

        public HomeController(ILogger<HomeController> logger, IPredictionService predictionService)
        {
            _logger = logger;
            _predictionService = predictionService;
        }

        public IActionResult Index()
        {
            return View(new PredictionPageViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Predict(PredictionPageViewModel model, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(model.HomeTeam) || string.IsNullOrWhiteSpace(model.AwayTeam))
            {
                model.ErrorMessage = "Completeaza ambele echipe inainte de trimitere.";
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
                model.ErrorMessage = "Predictia a esuat. Verifica API-ul si incearca din nou.";
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
    }
}
