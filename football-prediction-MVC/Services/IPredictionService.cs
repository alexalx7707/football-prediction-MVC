using football_prediction_MVC.Models.Api;

namespace football_prediction_MVC.Services;

public interface IPredictionService
{
    Task<PredictionResponse> PredictAsync(PredictionRequest request, CancellationToken cancellationToken = default);
}
