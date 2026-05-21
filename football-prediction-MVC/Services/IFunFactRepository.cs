using football_prediction_MVC.Models;

namespace football_prediction_MVC.Services;

public interface IFunFactRepository
{
    IReadOnlyList<FunFact> All { get; }
    FunFact? GetById(int id);
}
