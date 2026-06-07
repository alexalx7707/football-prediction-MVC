# Football Prediction MVC

An ASP.NET Core 9 MVC web application that predicts football match outcomes using a machine learning model. Built as a university diploma project to demonstrate full-stack web development, role-based access control, ML model integration, and an AI-powered assistant.

## Features

- **Match prediction** — select a home and away team; the app calls a Python ML API and displays the predicted outcome
- **Dataset statistics** — live stats from the training dataset shown on the home page
- **AI chatbot** — a football assistant powered by the Claude API with tool use, grounded in the app's own data (logged-in users only)
- **Fun facts** — rotating football facts shown on the user dashboard
- **Admin panel** — import the dataset from Kaggle and trigger model retraining
- **Role-based access control** — three tiers: Guest, User, Admin

## Architecture

```
football-prediction-MVC/   ← This ASP.NET Core 9 MVC app
Python ML API (localhost:7777)  ← Separate service (not in this repo)
SQL Server LocalDB              ← Identity + user data
Anthropic API                   ← Claude chatbot
```

The MVC app is a pure front-end/orchestration layer. All prediction logic and training live in the external Python API.

## Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core 9 MVC |
| ORM | Entity Framework Core 9 |
| Database | SQL Server LocalDB |
| Auth | ASP.NET Core Identity with roles |
| AI chatbot | Anthropic Messages API (hand-rolled `HttpClient`, no SDK) |
| Caching | `IMemoryCache` (team list) |

## Roles

| Role | Access |
|---|---|
| Guest | Home page, match prediction |
| User | + Profile, chatbot |
| Admin | + Dataset import, model training dashboard |

A default admin account is seeded on first run: `admin@example.com` / `Admin@123456`.

## Prerequisites

- .NET 9 SDK
- SQL Server LocalDB (ships with Visual Studio)
- The Python ML API running on `http://localhost:7777`
- An Anthropic API key (for the chatbot)

## Setup

1. **Clone the repo**
   ```
   git clone <repo-url>
   cd football-prediction-MVC/football-prediction-MVC
   ```

2. **Set user secrets** (never commit the API key)
   ```
   dotnet user-secrets set "Anthropic:ApiKey" "<your-key>"
   ```

3. **Apply migrations**
   ```
   dotnet ef database update
   ```

4. **Start the Python ML API** on port 7777 (see that project's README)

5. **Run the app**
   ```
   dotnet run
   ```
   Navigate to `https://localhost:<port>` — the default admin account is created automatically.

## Configuration

All settings live in `appsettings.json`. The API key must be stored in user secrets, not in the file.

| Key | Default | Description |
|---|---|---|
| `ConnectionStrings:DefaultConnection` | LocalDB | SQL Server connection string |
| `Api:BaseUrl` | `http://localhost:7777/` | Python ML API base URL |
| `Anthropic:Model` | `claude-haiku-4-5` | Claude model for the chatbot |
| `Anthropic:MaxTokens` | `2048` | Max tokens per chatbot response |

## Chatbot

The assistant is built directly against the Anthropic Messages API without a NuGet SDK, using the tool-use feature. It has access to five tools:

- `predict_match` — predict a specific fixture
- `get_team_elo` — fetch a team's ELO rating
- `search_matches` — search historical match data
- `list_teams` — list all teams (resolves abbreviations like "PSG")
- `get_dataset_stats` — return dataset statistics

The server is stateless; the client holds the conversation transcript and sends it with each request.

## Notes

- The ML dataset is from last season — predictions are model-based, not historical lookups
- No rate limiting on the chatbot by design (single-user demo)
- The antiforgery token for the chat JSON endpoint is sent via the `RequestVerificationToken` request header
