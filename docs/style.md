# Code style (C#)

Short on purpose: CLAUDE.md imports this file, so it is loaded in every session.

- File-scoped namespaces; one public type per file, except small records that belong to it.
- Primary constructors for services and DbContexts; records for request and response types.
- `var` when the type is obvious from the right-hand side.
- Names say what things are: `ListIssuesQuery`, not `Dto1`. Test names are sentences:
  `Closing_twice_returns_409`.
- Comments explain why, not what.
- Formatting is not discussed in reviews: `dotnet format` decides (a hook runs it after every edit).
