# trackit-toolkit (a Claude Code plugin)

The pieces of this lecture, packaged as one installable unit:

| Part | In the plugin | Source in the repository |
|---|---|---|
| Skills | `skills/release-notes/`, `skills/api-conventions/` | `.claude/skills/` |
| MCP server | `.mcp.json`: `trackit` (the C# server) | `.mcp.json` |
| Hooks | `hooks/hooks.json`: format, protect-files | `.claude/settings.json` |
| Subagent | `agents/code-reviewer.md` | `.claude/agents/` |

The skills, the subagent and the hook scripts are copies. Change them in `.claude/` and run
`scripts/sync-plugin.sh`; CI fails if the copies are out of date.

The MCP server and the format hook expect a TrackIt checkout: the server is started from
`${CLAUDE_PROJECT_DIR}/tools/Trackit.Mcp`.

## Install from this checkout

In Claude Code, from the repository root:

```
/plugin marketplace add ./
/plugin install trackit-toolkit@trackit
```

The marketplace file is `.claude-plugin/marketplace.json` at the repository root. The plugin's skills
are namespaced: `/trackit-toolkit:release-notes`. Inside this repository the project's own `.claude/`
already provides the same things, so installing the plugin here gives you everything twice (two
format hooks, two `trackit` servers). Install it at user scope to use it in another clone, or
uninstall it after trying: `/plugin uninstall trackit-toolkit@trackit`.
