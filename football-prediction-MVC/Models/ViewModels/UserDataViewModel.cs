using System.Text.Json;
using football_prediction_MVC.Models;
using football_prediction_MVC.Models.Api;

namespace football_prediction_MVC.Models.ViewModels;

public class UserDataViewModel
{
    public MatchQueryParams MatchQuery { get; set; } = new();
    public EloQueryParams EloQuery { get; set; } = new();
    public DataStatsResponse? Stats { get; set; }
    public List<Dictionary<string, JsonElement>> Matches { get; set; } = new();
    public List<Dictionary<string, JsonElement>> EloRatings { get; set; } = new();
    public int MatchPage { get; set; } = 1;
    public int MatchPageSize { get; set; } = 50;
    public int EloPage { get; set; } = 1;
    public int EloPageSize { get; set; } = 50;
    public int MatchFetchedCount { get; set; }
    public int EloFetchedCount { get; set; }
    public string? InfoMessage { get; set; }
    public string? ErrorMessage { get; set; }
    public FunFact? CurrentFunFact { get; set; }
    public bool AllFactsSeen { get; set; }
}
