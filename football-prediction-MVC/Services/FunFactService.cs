using football_prediction_MVC.Data;
using football_prediction_MVC.Models;
using Microsoft.EntityFrameworkCore;

namespace football_prediction_MVC.Services;

public class FunFactService : IFunFactService
{
    private static readonly TimeSpan CooldownWindow = TimeSpan.FromDays(2);

    private readonly ApplicationDbContext _db;
    private readonly IFunFactRepository _repository;

    public FunFactService(ApplicationDbContext db, IFunFactRepository repository)
    {
        _db = db;
        _repository = repository;
    }

    public async Task<(FunFact? Fact, bool AllSeen)> GetNextForUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        if (_repository.All.Count == 0)
        {
            return (null, false);
        }

        var cutoff = DateTime.UtcNow - CooldownWindow;

        var recentIds = await _db.UserFactHistories
            .Where(h => h.UserId == userId && h.ShownAt > cutoff)
            .Select(h => h.FactId)
            .ToListAsync(cancellationToken);

        var seen = new HashSet<int>(recentIds);
        var candidates = _repository.All.Where(f => !seen.Contains(f.Id)).ToList();

        if (candidates.Count > 0)
        {
            var chosen = candidates[Random.Shared.Next(candidates.Count)];
            _db.UserFactHistories.Add(new UserFactHistory
            {
                UserId = userId,
                FactId = chosen.Id,
                ShownAt = DateTime.UtcNow
            });
            await _db.SaveChangesAsync(cancellationToken);
            return (chosen, false);
        }

        var oldest = await _db.UserFactHistories
            .Where(h => h.UserId == userId)
            .OrderBy(h => h.ShownAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (oldest is null)
        {
            return (null, false);
        }

        oldest.ShownAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return (_repository.GetById(oldest.FactId), true);
    }
}
