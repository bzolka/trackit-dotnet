#!/usr/bin/env bash
# Copies the shared pieces of .claude/ into the trackit-toolkit plugin, so that the
# plugin and the repository never drift apart. CI runs this and fails on a diff.
# Hand-written in the plugin (paths differ): .claude-plugin/plugin.json, hooks/hooks.json, .mcp.json.
set -euo pipefail
cd "$(dirname "$0")/.."
plugin=.claude/plugins/trackit-toolkit

rm -rf "$plugin/skills" "$plugin/agents" "$plugin/scripts"
mkdir -p "$plugin/skills" "$plugin/agents" "$plugin/scripts"
cp -r .claude/skills/release-notes .claude/skills/api-conventions "$plugin/skills/"
cp .claude/agents/code-reviewer.md "$plugin/agents/"
cp scripts/format.sh .claude/hooks/protect-files.sh "$plugin/scripts/"
echo "Synced .claude/ into $plugin"
