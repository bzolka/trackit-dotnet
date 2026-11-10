---
name: api-conventions
description: Conventions for TrackIt REST endpoints.
  Use when adding or changing an API endpoint.
---

# TrackIt API conventions

- Routes are plural nouns: `/issues/{id}/comments`.
- State changes are POST actions: `/issues/{id}/close`.
- Errors use the `ProblemDetails` format.
- Ids in routes use the constraint `{id:int:min(1)}`; a missing resource is 404,
  a state change that is not allowed (closing a closed issue) is 409.
- Handlers return `TypedResults`; with several outcomes, declare them: `Results<Ok<Issue>, ProblemHttpResult>`.
- List endpoints take filters as query parameters, bound with `[AsParameters]` to a record,
  and always have a `limit` (1-100, default 20).

## Adding an endpoint
1. Endpoint handler in `src/Api/Endpoints/` (minimal API; this repository has no controllers).
   Map it in the existing `MapGroup` and give the request a record type next to the handler.
2. Request validator in `src/Api/Validators/`: an `AbstractValidator<TRequest>` for every endpoint
   that takes a body or query parameters. Inject `IValidator<TRequest>` and return
   `TypedResults.ValidationProblem(result.ToDictionary())` when it fails. Messages say how to fix the input.
3. Integration test in `src/Tests/Api/`: a class with `IClassFixture<TrackItApiFactory>`, covering the
   success case and every validation and error case. The factory gives each test class a fresh
   SQLite file; never point a test at `data/trackit.db`.
4. Add it to `docs/api.md`.
5. If the endpoint needs a schema change, change the model and run
   `dotnet ef migrations add <Name> --project src/Api` (never edit `Migrations/` by hand), then mirror
   the change in both MCP servers (see `.claude/rules/db.md`).
6. `dotnet build` and `dotnet test` pass before you call it done.

## Example

`POST /issues/{id}/close` in `src/Api/Endpoints/IssueEndpoints.cs`: a POST action with 404 and 409 as
ProblemDetails. `src/Tests/Api/CloseIssueTests.cs` tests all three outcomes.
