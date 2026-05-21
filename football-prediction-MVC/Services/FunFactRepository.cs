using System.Text.Json;
using football_prediction_MVC.Models;

namespace football_prediction_MVC.Services;

public class FunFactRepository : IFunFactRepository
{
    private readonly Dictionary<int, FunFact> _byId;
    private readonly List<FunFact> _all;

    public FunFactRepository(IWebHostEnvironment environment)
    {
        var path = Path.Combine(environment.ContentRootPath, "Data", "funfacts.json");

        using var stream = File.OpenRead(path);
        var loaded = JsonSerializer.Deserialize<List<FunFact>>(stream, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? new List<FunFact>();

        _all = loaded;
        _byId = loaded.ToDictionary(f => f.Id);
    }

    public IReadOnlyList<FunFact> All => _all;

    public FunFact? GetById(int id) => _byId.TryGetValue(id, out var fact) ? fact : null;
}
