using football_prediction_MVC.Models.Api;

namespace football_prediction_MVC.Models.ViewModels;

public class AdminOpsViewModel
{
    public TrainingRequest TrainingRequest { get; set; } = new();
    public ImportResponse? ImportResult { get; set; }
    public TrainingResponse? TrainingResult { get; set; }
    public TrainingStatusResponse? TrainingStatus { get; set; }
    public string? InfoMessage { get; set; }
    public string? ErrorMessage { get; set; }
}
