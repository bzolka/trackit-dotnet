# TrackIt MCP server (Python)

The same server as `tools/Trackit.Mcp` (C#), written with the official
[`mcp`](https://pypi.org/project/mcp/) package. Optional: needs Python 3.14.
It reads the same SQLite file as the API (`TRACKIT_DB` in `.env`), so start the
API once first to create the database.

From the repository root:

```bash
pip install -r tools/trackit-mcp-py/requirements.txt
python tools/trackit-mcp-py/trackit_server.py
```

The server speaks MCP over stdio, so it waits silently for a client. Use it from
Claude Code (`trackit-py` in `.mcp.json`) or from the MCP Inspector:

```bash
npx @modelcontextprotocol/inspector python tools/trackit-mcp-py/trackit_server.py
```

Tip: use a virtual environment (`python -m venv .venv`, then activate it) and
start `claude` from the same shell, so that `python` finds the `mcp` package.

- `trackit_server.py`: the tools; the code shown on the lecture slides, plus `create_issue`.
- `db.py`: plain SQL on the API's database. Mirror schema changes in `tools/Trackit.Mcp/IssueDb.cs`.
