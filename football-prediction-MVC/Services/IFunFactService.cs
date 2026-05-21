using football_prediction_MVC.Models;

namespace football_prediction_MVC.Services;

public interface IFunFactService
{
    Task<(FunFact? Fact, bool AllSeen)> GetNextForUserAsync(string userId, CancellationToken cancellationToken = default);
}
