# TrackIt (.NET)

A small issue tracker Web API, and the running example of the lecture *Extending the coding agent:
context, MCP, skills and subagents* (course *AI Assisted Software Development*). Everything the
slides show about TrackIt is in this repository: the API, the CLI, the MCP server (in C# and in
Python), CLAUDE.md, the skills, the hooks, the reviewer subagent and the plugin.

The commands work in PowerShell (Windows) and in bash (macOS, Linux); where they differ, both are
shown. In cmd, write the CLI as `tools\trackit`.

## Prerequisites

| Tool | Needed for | Check |
|---|---|---|
| [.NET 10 SDK](https://dotnet.microsoft.com/download) | everything | `dotnet --version` |
| Git Bash (Windows only; comes with [Git for Windows](https://git-scm.com/)) | the hooks and skill scripts (bash scripts that Claude Code runs) | `git --version` |
| [jq](https://jqlang.org/) (optional) | the two hooks read their input with jq | `jq --version` |
| [GitHub CLI](https://cli.github.com/) (optional) | `release-notes` lists pull requests with `gh` (it falls back to `git log`) | `gh auth status` |
| Python 3.14 (optional) | the second MCP server, `tools/trackit-mcp-py` | `python --version` |
| [Claude Code](https://code.claude.com/docs) | the agent part | `claude --version` |

On Windows: `winget install jqlang.jq` installs jq.

## Build, test, run

```bash
dotnet build
dotnet test
dotnet run --project src/Api
```

`dotnet test` needs no setup: every test class gets a fresh SQLite file in the temp folder, never the
dev database. The API listens on http://localhost:5000 and creates `data/trackit.db` with the sample
issues on first start (#41 *CSV export is slow*, #42 *Login fails after timeout*, #43 *Add an
endpoint for closing an issue*, and a few more). Try http://localhost:5000/issues?q=login.

Settings are in `.env` (copy `.env.example`; the defaults work without it). The endpoints are
described in [docs/api.md](docs/api.md), the modules in [docs/architecture.md](docs/architecture.md).

**`dotnet build` fails with *The file is locked by: "Trackit.Mcp (…)"*?** A Claude Code session open
in this folder (terminal, desktop app or IDE) runs the `trackit` MCP server, and on Windows a build
cannot overwrite a running program. Exit the session (or, in it, `/mcp` → `trackit` → *Disable*),
build, then start it again.

## The CLI

After `dotnet build`, with the API running:

```bash
tools/trackit search "login" --state open --limit 20
tools/trackit new --title "Export issues as JSON" --label feature
tools/trackit close 43
tools/trackit --help
```

`tools/trackit` is a bash script, and `tools/trackit.cmd` is its Windows twin: PowerShell picks the
`.cmd` by itself, so the commands above work there unchanged (in cmd, write `tools\trackit`). Both
run the built `tools/Trackit.Cli` program, which calls the API over HTTP.

## The MCP server

Two implementations of the same server, with the same two tools, `search_issues(query, state="open",
limit=20)` and `create_issue(title, description)`. Both read the SQLite file directly, so start the
API once to create it.

```bash
dotnet run --project tools/Trackit.Mcp                    # C#, ModelContextProtocol package
```

```bash
pip install -r tools/trackit-mcp-py/requirements.txt      # Python, official mcp package
python tools/trackit-mcp-py/trackit_server.py
```

Both wait silently: they speak MCP over stdio. To call them by hand, use the MCP Inspector (needs
Node.js): `npx @modelcontextprotocol/inspector dotnet run --project tools/Trackit.Mcp`.

## Using it with Claude Code

Start `claude` in the repository root. It picks up:

| File | What it gives the agent |
|---|---|
| `CLAUDE.md`, `src/Api/CLAUDE.md`, `.claude/rules/db.md` | project knowledge (always / in `src/Api` / with migrations) |
| `.mcp.json` | the MCP servers `trackit` (C#) and `trackit-py` (Python) |
| `.claude/skills/` | `release-notes`, `api-conventions`, `trackit-issues` |
| `.claude/settings.json` | the hooks (format after every edit, protect `.env`) and MCP approval |
| `.claude/agents/code-reviewer.md` | a read-only reviewer subagent |

Things to try: `/mcp`; "Which open issues are about login?"; `/release-notes v1.4`; "Use the
code-reviewer agent on my changes".

### Which MCP server is active

`trackit` (C#) is approved in `.claude/settings.json`; `trackit-py` is not, so the same tools never
show up twice. On the first start Claude Code asks about `trackit-py`: choose *Continue without using
this MCP server* (the default). To use the Python server instead:

1. Install the `mcp` package in a virtual environment, and start `claude` from the shell where it is
   active, so that `python` finds the package:

   ```bash
   python -m venv .venv
   .venv/Scripts/Activate.ps1        # PowerShell; macOS/Linux: source .venv/bin/activate
   pip install -r tools/trackit-mcp-py/requirements.txt
   ```

   If PowerShell refuses with *running scripts is disabled on this system*, run
   `Set-ExecutionPolicy -Scope Process Bypass` first: it allows scripts in this window only.
2. If you declined `trackit-py` earlier, run `claude mcp reset-project-choices`.
3. Start `claude` and choose *Use this MCP server* when it asks about `trackit-py`.
4. In `/mcp`, select `trackit` and choose *Disable*. Now only `trackit-py` provides the tools.

To switch back: in `/mcp`, disable `trackit-py` and enable `trackit`.

### The plugin

`.claude/plugins/trackit-toolkit/` packages the skills, the MCP server, the hooks and the subagent as
one plugin, and the repository is its marketplace:

```
/plugin marketplace add ./
/plugin install trackit-toolkit@trackit
```

Inside this repository you then have everything twice (the project's `.claude/` plus the plugin), so
uninstall it after trying (`/plugin uninstall trackit-toolkit@trackit`), or install it at user scope
for use in another clone. Details: [the plugin's README](.claude/plugins/trackit-toolkit/README.md).

## Repository layout

```
src/Api/                 Web API (minimal API, EF Core + SQLite, FluentValidation)
src/Tests/               xUnit integration tests
tools/Trackit.Cli/       the trackit CLI; run it with tools/trackit (in cmd: tools\trackit)
tools/Trackit.Mcp/       MCP server, C#
tools/trackit-mcp-py/    MCP server, Python
scripts/                 format.sh (the format hook), sync-plugin.sh
docs/                    architecture and design decisions, API, product backlog, code style
CLAUDE.md  .mcp.json  .claude/  .claude-plugin/   the agent configuration
```

## Versions

Built and tested on 2026-11-07 with .NET SDK 10.0.401, EF Core 10.0.14, FluentValidation 12.2.0,
ModelContextProtocol 2.4.0, System.CommandLine 2.0.14, Python 3.14.10 with mcp 2.3.0, and
Claude Code 2.1.286 and 2.1.319. The Claude Code formats (hooks, skill frontmatter, plugin and
marketplace manifests, `enabledMcpjsonServers`) follow the documentation at code.claude.com/docs as of
version 2.1.319. They change quickly: check them again before you rely on them.

## License

Not open source; see [LICENSE](LICENSE).
