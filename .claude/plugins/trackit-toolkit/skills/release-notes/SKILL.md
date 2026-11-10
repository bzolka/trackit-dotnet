---
name: release-notes
description: Writes release notes for TrackIt from
  merged pull requests. Use when the user asks
  for release notes or a changelog.
---

# Writing release notes

1. Run `scripts/changes.sh <last-tag>` to list
   the merged pull requests.
2. Group them: Features, Fixes, Internal.
3. One line per item, in user-facing wording,
   with a link to the pull request.
4. Follow the layout in `template.md`.

## Details

- The last tag is the argument, if one was given: $ARGUMENTS.
  Without an argument, use the newest tag (`git describe --tags --abbrev=0`).
- `scripts/` and `template.md` are in this skill's folder, not in the repository root.
- The notes are for the next version after the last tag (after v1.4 comes 1.5),
  unless the user names the version.
- Features: what users can do now. Fixes: what no longer goes wrong. Internal: tests,
  CI, tooling, docs; keep these short, since users read Features and Fixes first.
- Write the notes into the conversation. Do not create a file or a tag unless asked.
