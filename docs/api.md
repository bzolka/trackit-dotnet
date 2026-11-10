# TrackIt API reference

Base URL: `http://localhost:5000` (`dotnet run --project src/Api`). Request and response bodies are JSON.
Errors are [ProblemDetails](https://www.rfc-editor.org/rfc/rfc9457) (`application/problem+json`);
validation errors list the failing fields under `errors`.

An issue:

```json
{ "id": 42, "title": "Login fails after timeout", "description": "...",
  "status": "open", "label": "bug", "createdAt": "2026-10-03T00:00:00" }
```

`status` is `open`, `in progress` or `closed`. `label` is `bug`, `feature` or `null`.

## GET /issues

List and search issues, ordered by number.

| Query | Meaning | Rules |
|---|---|---|
| `status` | only issues with this status | `open`, `in progress` or `closed`; omit for all |
| `q` | text to find in the title or description (case-insensitive) | at most 200 characters |
| `limit` | at most this many issues | 1-100, default 20 |

Example: `GET /issues?status=open&q=login&limit=20` returns #42 and #57. `400` if a parameter is invalid.

## GET /issues/{id}

One issue. `404` if it does not exist.

## POST /issues

Create an issue. It starts as `open`.

```json
{ "title": "Export issues as JSON", "description": "Like the CSV export.", "label": "feature" }
```

`title` is required (at most 200 characters); `description` is optional (at most 4000); `label` is
optional (`bug` or `feature`). Returns `201 Created` with the issue and a `Location` header; `400` if invalid.

## POST /issues/{id}/close

Close an issue. Returns the closed issue. `404` if it does not exist, `409` if it is already closed.
