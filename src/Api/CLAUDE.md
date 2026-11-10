# The API layer (src/Api)

Loaded when you work in this folder. Project-wide rules are in the root CLAUDE.md.

- Endpoints are minimal-API handlers in `Endpoints/IssueEndpoints.cs`, mapped in one `MapGroup("/issues")`.
  Handlers are `static` methods that return `TypedResults` (with `Results<...>` for several outcomes).
- Request and query types are records next to the endpoints (`CreateIssueRequest`, `ListIssuesQuery`).
  Query strings bind with `[AsParameters]`.
- Validation: inject `IValidator<T>` into the handler and return `TypedResults.ValidationProblem(...)`.
  Validators in `Validators/` are registered automatically (`AddValidatorsFromAssemblyContaining`).
- Route ids are validated by the route constraint `{id:int:min(1)}`, not by a validator.
- Errors are ProblemDetails: `TypedResults.Problem(detail, statusCode: ...)`. 404 for a missing issue,
  409 for a state change that is not allowed (closing a closed issue).
- Status values are the strings in `IssueStatus` ("open", "in progress", "closed"); never invent new ones.
- `Data/SeedData.cs` seeds the sample issues. Their numbers (#41-#43 in particular) appear in the
  lecture slides; do not renumber them.
- Settings come from `.env` in the repository root (`DotEnv.cs`); `TRACKIT_DB` is the database path.
