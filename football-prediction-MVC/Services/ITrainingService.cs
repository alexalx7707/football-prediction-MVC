using football_prediction_MVC.Models.Api;

namespace football_prediction_MVC.Services;

public interface ITrainingService
{
    Task<TrainingResponse> TrainAsync(TrainingRequest request, CancellationToken cancellationToken = default);
    Task<TrainingStatusResponse> GetStatusAsync(CancellationToken cancellationToken = default);
}
