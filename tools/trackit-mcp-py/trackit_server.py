"""The TrackIt MCP server (Python). Same tools as tools/Trackit.Mcp (C#).

Run:  pip install -r tools/trackit-mcp-py/requirements.txt
      python tools/trackit-mcp-py/trackit_server.py
"""
from mcp.server import MCPServer

import db

mcp = MCPServer("trackit")

@mcp.tool()
def search_issues(query: str, state: str = "open",
                  limit: int = 20) -> str:
    """Search TrackIt issues by text.
    Use for questions about bugs and tasks.

    Args:
        query: words to look for in title and description
        state: "open", "closed" or "all"
        limit: at most this many results
    """
    issues = db.search(query, state, limit)
    return "\n".join(
        f"#{i.id} {i.title} ({i.state})" for i in issues)

@mcp.tool()
def create_issue(title: str, description: str) -> str:
    """Create a new TrackIt issue.
    Use when the user asks to report, file or add a bug or task.

    Args:
        title: a short summary, at most 200 characters
        description: what happens, and what should happen instead
    """
    issue = db.create(title, description)
    return f"Created #{issue.id} {issue.title} ({issue.state})"

if __name__ == "__main__":
    mcp.run(transport="stdio")
