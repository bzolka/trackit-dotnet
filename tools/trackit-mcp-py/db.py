"""Data access for the TrackIt MCP server: plain SQL on the API's SQLite file.

The schema belongs to the API (src/Api/Migrations/). If it changes,
change this file and tools/Trackit.Mcp/IssueDb.cs too.
"""
import os
import sqlite3
from contextlib import closing
from datetime import datetime, timezone
from pathlib import Path
from typing import NamedTuple

from mcp.server.mcpserver.exceptions import ToolError

REPO_ROOT = Path(__file__).resolve().parents[2]
STATES = ("open", "closed", "all")


class Issue(NamedTuple):
    id: int
    title: str
    state: str


def _setting(name: str, default: str) -> str:
    """Real environment variables win over the .env file in the repository root."""
    if name in os.environ:
        return os.environ[name]
    env_file = REPO_ROOT / ".env"
    if env_file.exists():
        for line in env_file.read_text(encoding="utf-8").splitlines():
            key, sep, value = line.strip().partition("=")
            if sep and not key.startswith("#") and key.strip() == name:
                return value.strip()
    return default


def _connect() -> sqlite3.Connection:
    path = REPO_ROOT / _setting("TRACKIT_DB", "data/trackit.db")
    # The API creates the database and its tables; this server never does.
    if not path.exists():
        raise ToolError(f"TrackIt database not found at {path}. "
                         "Start the API once (dotnet run --project src/Api) to create it.")
    return sqlite3.connect(f"file:{path}?mode=rw", uri=True)


def search(query: str, state: str = "open", limit: int = 20) -> list[Issue]:
    # Errors say how to fix the call: the model reads them and tries again.
    # (Only a ToolError reaches the model; other exceptions are hidden by the SDK.)
    if state not in STATES:
        raise ToolError("state must be one of: open, closed, all.")
    if not 1 <= limit <= 100:
        raise ToolError("limit must be between 1 and 100.")

    pattern = f"%{query}%"
    with closing(_connect()) as conn, conn:  # close, and commit on success
        rows = conn.execute(
            """SELECT Id, Title, Status FROM Issues
               WHERE (Title LIKE ? OR Description LIKE ?)
                 AND (? = 'all' OR Status = ?)
               ORDER BY Id
               LIMIT ?""",
            (pattern, pattern, state, state, limit)).fetchall()
    return [Issue(*row) for row in rows]


def create(title: str, description: str) -> Issue:
    # The same rules as the API's CreateIssueRequestValidator.
    if not title.strip() or len(title) > 200:
        raise ToolError("title is required and must be at most 200 characters.")
    if len(description) > 4000:
        raise ToolError("description must be at most 4000 characters.")

    # Same text format as EF Core uses for DateTime columns in SQLite.
    created_at = datetime.now(timezone.utc).strftime("%Y-%m-%d %H:%M:%S.%f")
    with closing(_connect()) as conn, conn:  # close, and commit on success
        (new_id,) = conn.execute(
            """INSERT INTO Issues (Title, Description, Status, CreatedAt)
               VALUES (?, ?, 'open', ?) RETURNING Id""",
            (title, description, created_at)).fetchone()
    return Issue(new_id, title, "open")
