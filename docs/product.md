# TrackIt: the product

TrackIt is a deliberately small issue tracker. A team records bugs and tasks as issues, searches them,
and closes them when the work is done. It has no users, no authentication and no web front end: it is
a Web API, used through HTTP, a command-line client and an MCP server.

## An issue

| Field | Meaning |
|---|---|
| number | shown as `#41`; assigned by the database |
| title | one line, at most 200 characters |
| description | what happens and what should happen instead |
| status | `open` → `in progress` → `closed` |
| label | `bug` or `feature` (optional) |
| created | when it was reported (UTC) |

## What it can do

- List and search issues: by text in the title and description, by status, with a limit.
- Show one issue.
- Create an issue.
- Close an issue.

## Backlog

The sample data that the API loads on first start is the product's own backlog. Three of the issues
appear on the lecture slides, so their numbers are fixed.

| # | Title | Status | Label | Note |
|---|---|---|---|---|
| 37 | Sort the issue list by creation date | closed | feature | |
| 38 | Login page shows the wrong error message | closed | bug | |
| 39 | Add a limit parameter to the issue search | closed | feature | |
| 40 | Issue titles longer than 200 characters are cut off | closed | bug | |
| **41** | **CSV export is slow** | open | bug | on the slides |
| **42** | **Login fails after timeout** | open | bug | on the slides: what the MCP server finds |
| **43** | **Add an endpoint for closing an issue** | in progress | feature | on the slides: the task in the opening story |
| 44 | Email notification when an issue is closed | open | feature | |
| 45 | Validation errors are not shown next to the field | open | bug | |
| 57 | Remember login on mobile | open | feature | on the slides: the second login hit |

Searching for "login" finds #38, #42 and #57 (#42 and #57 among the open ones); most other words find
one or two issues.

Issue #43 is a small irony of the example: the repository already has `POST /issues/{id}/close`. The
backlog belongs to the story of the lecture, in which a developer asks the agent to build
exactly that endpoint.

## Not planned

Users, authentication, comments, attachments, a web UI. TrackIt stays small enough to read in an
afternoon; the interesting part is how a coding agent works with it.
