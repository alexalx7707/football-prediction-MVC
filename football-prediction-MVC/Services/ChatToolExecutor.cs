using System.Text.Json;
using football_prediction_MVC.Models.Api;
using football_prediction_MVC.Models.Chat;

namespace football_prediction_MVC.Services;

// Bridges the chatbot's tool calls to the services the app already exposes.
// Each tool returns a JSON string that becomes a tool_result for the model.
public class ChatToolExecutor
{
    private const int MaxRows = 20;

    private readonly IPredictionService _prediction;
    private readonly IDataService _data;

    public ChatToolExecutor(IPredictionService prediction, IDataService data)
    {
        _prediction = prediction;
        _data = data;
        Tools = BuildTools();
    }

    public IReadOnlyList<ClaudeTool> Tools { get; }

    public async Task<string> ExecuteAsync(string name, JsonElement input, CancellationToken ct)
    {
        try
        {
            return name switch
            {
                "predict_match" => await PredictMatchAsync(input, ct),
                "get_team_elo" => await GetTeamEloAsync(input, ct),
                "search_matches" => await SearchMatchesAsync(input, ct),
                "list_teams" => await ListTeamsAsync(ct),
                "get_dataset_stats" => await GetDatasetStatsAsync(ct),
                "get_team_summary" => await GetTeamSummaryAsync(input, ct),
                "get_head_to_head" => await GetHeadToHeadAsync(input, ct),
                "list_seasons" => await ListSeasonsAsync(ct),
                "get_standings" => await GetStandingsAsync(input, ct),
                _ => Error($"Unknown tool '{name}'.")
            };
        }
        catch (Exception ex)
        {
            return Error(ex.Message);
        }
    }

    private async Task<string> PredictMatchAsync(JsonElement input, CancellationToken ct)
    {
        var home = Str(input, "home_team");
        var away = Str(input, "away_team");
        if (string.IsNullOrWhiteSpace(home) || string.IsNullOrWhiteSpace(away))
        {
            return Error("home_team and away_team are both required.");
        }

        var result = await _prediction.PredictAsync(
            new PredictionRequest { HomeTeam = home.Trim(), AwayTeam = away.Trim() }, ct);

        return JsonSerializer.Serialize(new
        {
            match = result.Match,
            prediction = result.Prediction,
            confidence = result.Confidence,
            home_team = result.HomeTeam,
            away_team = result.AwayTeam,
            home_elo = result.HomeElo,
            away_elo = result.AwayElo,
            home_win_prob = result.HomeWinProb,
            draw_prob = result.DrawProb,
            away_win_prob = result.AwayWinProb,
            reasoning = result.Reasoning?.Summary
        });
    }

    private async Task<string> GetTeamEloAsync(JsonElement input, CancellationToken ct)
    {
        var team = Str(input, "team");
        if (string.IsNullOrWhiteSpace(team))
        {
            return Error("team is required.");
        }

        var rows = await _data.GetEloRatingsAsync(
            new EloQueryParams { Club = team.Trim(), Limit = 5 }, ct);
        return JsonSerializer.Serialize(new { count = rows.Count, ratings = rows });
    }

    private async Task<string> SearchMatchesAsync(JsonElement input, CancellationToken ct)
    {
        var limit = input.TryGetProperty("limit", out var l) && l.TryGetInt32(out var n)
            ? Math.Clamp(n, 1, MaxRows)
            : 10;

        var query = new MatchQueryParams
        {
            HomeTeam = Str(input, "home_team"),
            AwayTeam = Str(input, "away_team"),
            DateFrom = Str(input, "date_from"),
            DateTo = Str(input, "date_to"),
            Season = Str(input, "season"),
            Order = Str(input, "order"),
            Limit = limit
        };

        var rows = await _data.GetMatchesAsync(query, ct);
        var limited = rows.Take(MaxRows).ToList();
        return JsonSerializer.Serialize(new { count = limited.Count, matches = limited });
    }

    private async Task<string> GetTeamSummaryAsync(JsonElement input, CancellationToken ct)
    {
        var team = Str(input, "team");
        if (string.IsNullOrWhiteSpace(team))
        {
            return Error("team is required.");
        }

        var result = await _data.GetTeamSummaryAsync(new TeamSummaryQuery
        {
            Team = team.Trim(),
            Season = Str(input, "season"),
            DateFrom = Str(input, "date_from"),
            DateTo = Str(input, "date_to")
        }, ct);
        return result.GetRawText();
    }

    private async Task<string> GetHeadToHeadAsync(JsonElement input, CancellationToken ct)
    {
        var teamA = Str(input, "team_a");
        var teamB = Str(input, "team_b");
        if (string.IsNullOrWhiteSpace(teamA) || string.IsNullOrWhiteSpace(teamB))
        {
            return Error("team_a and team_b are both required.");
        }

        var limit = input.TryGetProperty("limit", out var l) && l.TryGetInt32(out var n)
            ? Math.Clamp(n, 1, MaxRows)
            : 5;

        var result = await _data.GetHeadToHeadAsync(new HeadToHeadQuery
        {
            TeamA = teamA.Trim(),
            TeamB = teamB.Trim(),
            Season = Str(input, "season"),
            DateFrom = Str(input, "date_from"),
            DateTo = Str(input, "date_to"),
            Limit = limit
        }, ct);
        return result.GetRawText();
    }

    private async Task<string> ListSeasonsAsync(CancellationToken ct)
    {
        var seasons = await _data.GetSeasonsAsync(ct);
        return JsonSerializer.Serialize(new { count = seasons.Count, seasons });
    }

    private async Task<string> GetStandingsAsync(JsonElement input, CancellationToken ct)
    {
        var season = Str(input, "season");
        if (string.IsNullOrWhiteSpace(season))
        {
            return Error("season is required (e.g. '2023/24').");
        }

        var result = await _data.GetStandingsAsync(new StandingsQuery
        {
            Season = season.Trim(),
            League = Str(input, "league")
        }, ct);
        return result.GetRawText();
    }

    private async Task<string> ListTeamsAsync(CancellationToken ct)
    {
        var teams = await _data.GetTeamsAsync(ct);
        return JsonSerializer.Serialize(new { count = teams.Count, teams });
    }

    private async Task<string> GetDatasetStatsAsync(CancellationToken ct)
    {
        var stats = await _data.GetStatsAsync(ct);
        return JsonSerializer.Serialize(new
        {
            collections = stats.Collections.Select(c => new
            {
                collection = c.Collection,
                document_count = c.DocumentCount
            })
        });
    }

    private static string? Str(JsonElement input, string name) =>
        input.ValueKind == JsonValueKind.Object
            && input.TryGetProperty(name, out var v)
            && v.ValueKind == JsonValueKind.String
            ? v.GetString()
            : null;

    private static string Error(string message) =>
        JsonSerializer.Serialize(new { error = message });

    private static List<ClaudeTool> BuildTools() => new()
    {
        new ClaudeTool
        {
            Name = "predict_match",
            Description = "Predict the outcome of a football match between two teams using the app's trained model. "
                + "Returns the predicted result, confidence, win/draw/loss probabilities, and each team's Elo rating. "
                + "Use the exact team names returned by list_teams (e.g. 'Paris SG', not 'PSG').",
            InputSchema = Schema(new
            {
                type = "object",
                properties = new
                {
                    home_team = new { type = "string", description = "Home team name, exactly as stored in the dataset." },
                    away_team = new { type = "string", description = "Away team name, exactly as stored in the dataset." }
                },
                required = new[] { "home_team", "away_team" }
            })
        },
        new ClaudeTool
        {
            Name = "get_team_elo",
            Description = "Look up the most recent Elo rating rows for a single club. Use list_teams first if unsure of the exact name.",
            InputSchema = Schema(new
            {
                type = "object",
                properties = new
                {
                    team = new { type = "string", description = "Club name, exactly as stored in the dataset." }
                },
                required = new[] { "team" }
            })
        },
        new ClaudeTool
        {
            Name = "search_matches",
            Description = "Search/list individual historical matches in the dataset. All filters are optional; omit them to get the "
                + "most recent matches. Dates are ISO (YYYY-MM-DD). Returns at most 20 rows, so this is for listing specific "
                + "fixtures or finding the latest/earliest match — NOT for counting or totalling (use get_team_summary or "
                + "get_head_to_head for records and tallies).",
            InputSchema = Schema(new
            {
                type = "object",
                properties = new
                {
                    home_team = new { type = "string", description = "Filter by home team (optional)." },
                    away_team = new { type = "string", description = "Filter by away team (optional)." },
                    date_from = new { type = "string", description = "Earliest match date, YYYY-MM-DD (optional)." },
                    date_to = new { type = "string", description = "Latest match date, YYYY-MM-DD (optional)." },
                    season = new { type = "string", description = "Filter to one season, formatted 'YYYY/YY' e.g. '2023/24' (optional). Call list_seasons if unsure." },
                    order = new { type = "string", description = "Sort by date: 'desc' (newest first, default) or 'asc'. Use 'desc' for the most recent/last match." },
                    limit = new { type = "integer", description = "Max rows to return (1-20, default 10)." }
                }
            })
        },
        new ClaudeTool
        {
            Name = "get_team_summary",
            Description = "Get one team's computed record (played, wins, draws, losses, goals for/against, goal difference, points, "
                + "clean sheets, and home/away splits) over a whole season or a date window. Use this for any 'how many wins/goals/"
                + "points' style question — the counts are computed server-side, so prefer it over counting search_matches rows.",
            InputSchema = Schema(new
            {
                type = "object",
                properties = new
                {
                    team = new { type = "string", description = "Team name, exactly as stored (use list_teams to resolve shorthand)." },
                    season = new { type = "string", description = "Season formatted 'YYYY/YY' e.g. '2023/24' (optional). Call list_seasons if unsure." },
                    date_from = new { type = "string", description = "Window start, YYYY-MM-DD (optional alternative to season)." },
                    date_to = new { type = "string", description = "Window end, YYYY-MM-DD (optional alternative to season)." }
                },
                required = new[] { "team" }
            })
        },
        new ClaudeTool
        {
            Name = "get_head_to_head",
            Description = "Get the computed head-to-head record between two teams (total meetings, each team's wins, draws, goals) "
                + "plus the most recent meetings. Optionally restrict to a season or date window.",
            InputSchema = Schema(new
            {
                type = "object",
                properties = new
                {
                    team_a = new { type = "string", description = "First team, exactly as stored (use list_teams to resolve shorthand)." },
                    team_b = new { type = "string", description = "Second team, exactly as stored." },
                    season = new { type = "string", description = "Season formatted 'YYYY/YY' (optional)." },
                    date_from = new { type = "string", description = "Window start, YYYY-MM-DD (optional)." },
                    date_to = new { type = "string", description = "Window end, YYYY-MM-DD (optional)." },
                    limit = new { type = "integer", description = "How many recent meetings to include (1-20, default 5)." }
                },
                required = new[] { "team_a", "team_b" }
            })
        },
        new ClaudeTool
        {
            Name = "list_seasons",
            Description = "List the seasons available in the dataset (formatted 'YYYY/YY'). Call this to learn the valid season range "
                + "before using a season filter. Note: early seasons may be only partially covered; recent seasons are complete.",
            InputSchema = Schema(new { type = "object", properties = new { } })
        },
        new ClaudeTool
        {
            Name = "get_standings",
            Description = "Get the final league table for a season, grouped by league. season is required. If the season spans "
                + "multiple leagues, the tool will report the available league codes and you must re-call with one of them in 'league' "
                + "(e.g. 'E0' for the English Premier League).",
            InputSchema = Schema(new
            {
                type = "object",
                properties = new
                {
                    season = new { type = "string", description = "Season formatted 'YYYY/YY' e.g. '2023/24'." },
                    league = new { type = "string", description = "League/division code (optional; required when a season has more than one league)." }
                },
                required = new[] { "season" }
            })
        },
        new ClaudeTool
        {
            Name = "list_teams",
            Description = "List every team name available in the dataset. Call this to resolve a user's shorthand "
                + "(e.g. 'PSG', 'Man Utd') to the exact stored name before calling other tools.",
            InputSchema = Schema(new { type = "object", properties = new { } })
        },
        new ClaudeTool
        {
            Name = "get_dataset_stats",
            Description = "Get document counts per collection in the dataset (matches, Elo ratings, etc.).",
            InputSchema = Schema(new { type = "object", properties = new { } })
        }
    };

    private static JsonElement Schema(object schema) => JsonSerializer.SerializeToElement(schema);
}
