using football_prediction_MVC.Models.Api;

namespace football_prediction_MVC.Models.ViewModels;

public class PredictionPageViewModel
{
    public string? HomeTeam { get; set; }
    public string? AwayTeam { get; set; }
    public PredictionResponse? Result { get; set; }
    public string? ErrorMessage { get; set; }
    public DataStatsResponse? Stats { get; set; }
    public List<string> Teams { get; set; } = new();
}
