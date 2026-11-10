#!/bin/bash
# protect-files.sh, registered for PreToolUse
FILE=$(jq -r '.tool_input.file_path // empty')

if [[ "$FILE" == *".env"* ]]; then
  echo "Blocked: $FILE is protected" >&2
  exit 2     # 2 = block the tool call
fi
exit 0       # 0 = allow
