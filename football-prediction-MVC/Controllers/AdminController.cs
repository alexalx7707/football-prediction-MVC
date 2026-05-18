using football_prediction_MVC.Models.ViewModels;
using football_prediction_MVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace football_prediction_MVC.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly IDataService _dataService;
    private readonly ITrainingService _trainingService;

    public AdminController(IDataService dataService, ITrainingService trainingService)
    {
        _dataService = dataService;
        _trainingService = trainingService;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var model = new AdminOpsViewModel();
        try
        {
            model.TrainingStatus = await _trainingService.GetStatusAsync(cancellationToken);
        }
        catch
        {
            model.ErrorMessage = "Could not load the training status. Make sure the API is running.";
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ImportData(AdminOpsViewModel model, CancellationToken cancellationToken)
    {
        try
        {
            model.ImportResult = await _dataService.ImportDatasetAsync(cancellationToken);
            model.InfoMessage = model.ImportResult.Message;
        }
        catch
        {
            model.ErrorMessage = "Dataset import failed. Check Kaggle access and the API.";
        }

        return View("Index", model);
    }

    public async Task<IActionResult> Dashboard(CancellationToken cancellationToken)
    {
        var model = new AdminOpsViewModel();
        try
        {
            model.TrainingStatus = await _trainingService.GetStatusAsync(cancellationToken);
        }
        catch
        {
            model.ErrorMessage = "Could not load the training status. Make sure the API is running.";
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> StartTraining(AdminOpsViewModel model, CancellationToken cancellationToken)
    {
        try
        {
            model.TrainingResult = await _trainingService.TrainAsync(model.TrainingRequest, cancellationToken);
            model.InfoMessage = model.TrainingResult.Message;
            model.TrainingStatus = await _trainingService.GetStatusAsync(cancellationToken);
        }
        catch
        {
            model.ErrorMessage = "Training did not start. Check the API and the parameters.";
        }

        return View("Dashboard", model);
    }
}