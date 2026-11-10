#!/bin/bash
# format.sh, registered for PostToolUse on Edit|Write in .claude/settings.json.
# Runs dotnet format on the C# file the agent has just edited.
FILE=$(jq -r '.tool_input.file_path // empty')
[[ "$FILE" == *.cs ]] || exit 0

# Claude Code on Windows passes C:\ paths; Git Bash wants /c/ paths.
if command -v cygpath >/dev/null; then FILE=$(cygpath -u "$FILE"); fi

# The containing project: the nearest folder upwards that has a .csproj file.
DIR=$(dirname "$FILE")
until compgen -G "$DIR/*.csproj" >/dev/null || [[ "$DIR" == "/" || "$DIR" == "." ]]; do
  DIR=$(dirname "$DIR")
done
PROJECT=$(compgen -G "$DIR/*.csproj" | head -1)
[[ -n "$PROJECT" ]] || exit 0   # a .cs file outside any project: nothing to do

# Format only this file. Both paths are relative to the current folder: with absolute
# paths, dotnet format silently skips the file. Output goes to stderr (stdout is hook JSON).
if ! dotnet format "$(realpath --relative-to=. "$PROJECT")" --include "$(realpath --relative-to=. "$FILE")" >&2; then
  echo "dotnet format failed for $FILE" >&2
  exit 1   # not 2: the edit has already happened, this only reports the problem
fi
