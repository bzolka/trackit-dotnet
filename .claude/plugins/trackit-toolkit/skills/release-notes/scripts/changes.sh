#!/usr/bin/env bash
# Lists the pull requests merged since a tag, one per line.
# Usage: changes.sh <last-tag>
# Uses gh (GitHub CLI) when it is installed and logged in, git log otherwise.
set -euo pipefail

tag="${1:?usage: changes.sh <last-tag>}"
git rev-parse --verify --quiet "$tag^{commit}" >/dev/null || { echo "Unknown tag: $tag" >&2; exit 1; }

if command -v gh >/dev/null 2>&1 && gh auth status >/dev/null 2>&1; then
  echo "Source: GitHub pull requests via gh (merged after $tag)"
  # Keep the pull requests whose merge commit is between the tag and HEAD.
  commits=$(git rev-list "$tag"..HEAD)
  gh pr list --state merged --limit 200 \
     --json number,title,url,mergeCommit \
     --jq '.[] | "\(.mergeCommit.oid) #\(.number) \(.title) \(.url)"' |
  while read -r oid rest; do
    if grep -qx "$oid" <<<"$commits"; then echo "$rest"; fi
  done | sort -k1.2n
else
  echo "Source: git log (gh not available), merge commits after $tag"
  git log "$tag"..HEAD --merges --oneline
fi
