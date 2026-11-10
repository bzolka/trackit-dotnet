---
paths: ["src/Api/Migrations/**", "**/*.sql"]
---

# Database conventions

- SQLite. The dev database is `data/trackit.db` (`TRACKIT_DB` in `.env`); tests create a temporary file.
- Schema changes start in the model: `src/Api/Data/Issue.cs` and `TrackItDb.cs`. Then generate the migration:
  `dotnet tool restore` (once), then `dotnet ef migrations add <Name> --project src/Api`.
- Never edit files under `Migrations/` by hand. A wrong migration is removed with
  `dotnet ef migrations remove --project src/Api` and generated again.
- Migrations are applied when the API starts (`Database.Migrate()` in `SeedData`).
- Table and column names are EF Core's defaults (`Issues`, `Id`, `Title`, `Status`, ...). Both MCP servers
  query them by name with plain SQL: mirror every schema change in `tools/Trackit.Mcp/IssueDb.cs` and
  `tools/trackit-mcp-py/db.py`.
- Timestamps are stored in UTC.
