# TrackIt

Issue tracker Web API (ASP.NET Core, SQLite).
Code style: @docs/style.md

## Commands
- Build: `dotnet build`
- Test: `dotnet test` (uses a fresh SQLite file; never the dev DB)
- Run: `dotnet run --project src/Api` (serves http://localhost:5000)
- Issues from the shell: `tools/trackit --help` (needs the API running)

## Conventions
- Branch names: `feature/<issue-id>-short-name`
- Every endpoint needs a validator and a test.
- See docs/architecture.md for the module layout.

## Never
- Never edit files under `Migrations/` by hand.
- Never edit `.env`; change `.env.example` and tell the user.
