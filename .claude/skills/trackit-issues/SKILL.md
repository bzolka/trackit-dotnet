---
name: trackit-issues
description: Work with TrackIt issues (search,
  create, close) using the trackit CLI.
---

- Search:
  `tools/trackit search "<text>" --state open --limit 20`
- Create:
  `tools/trackit new --title "<t>" --label <bug|feature>`
  Always add a label.
- Never close an issue without a linked PR.
  Ask for the pull request first, then: `tools/trackit close <id>`
- More options: `tools/trackit --help`

The CLI calls the API. If it cannot reach it, start the API with
`dotnet run --project src/Api` (in the background) and try again.
In PowerShell or cmd the command is `tools\trackit.cmd`.
