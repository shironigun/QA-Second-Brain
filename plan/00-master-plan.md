# Master plan

Goal: a multi-team web app where agents analyse defects and stories on Azure DevOps, draft typed artifacts, and QA approves every write. The KB learns only from approved, released work. Full vision: https://claude.ai/artifact/LHHUUZtjvtUBi7E7wGUMay

## Sub-plans

| ID | Sub-plan | Delivers (demo at exit) | Depends | Status |
|---|---|---|---|---|
| S0 | Repo and skeleton | Empty app runs: API health, web shell, Postgres via compose, CI green | none | todo |
| S1 | Foundation | A QA signs in, configures a team, clicks "Analyse now" on a defect, sees a Code-agent draft in the inbox, approves it, and a comment reaches a fake ADO | S0 | todo |
| S2 | Defect triage | Service hook on defect create produces a full Triage Brief (classifier + 4 specialists + lead), posted in house format after approval; replay on closed defects produces a scorecard | S1 | todo |
| S3 | Defect verification | On Resolved: link and prod-branch checks, code-review verdict, retest list, test-plan check with 4 outcomes, execution result by QA, QA UPDATE comment, reopen handling | S2 | todo |
| S4 | Story flow | Story to Ready gives a Test Intent Brief (gaps + coverage per criterion); PRs give a Build Check; bugs from story testing enter the defect flow | S3 | todo |
| S5 | Dashboards and cost | Board metrics with "show query", baseline capture, cost per team/agent/run, budget breaker | S1 | todo |
| S6 | Releases | Release detection, Release Report, gate policy with overrides, sign-off per area, post-release watch, rollback detection | S3, S5 | todo |
| S7 | Knowledge base lifecycle | Trust tiers, KB Delta review, disputes, auto-rollback, symbol-level staleness, KB read API | S6 | todo |
| S8 | Evals and rollout | Eval gate on skill upgrades, calibration, canaries, AI-quality dashboard, onboarding wizard for a second team | S2–S7 | todo |

S5 can run in parallel with S2–S4 (separate files). All other sub-plans run in order.

## Cross-cutting rules (every sub-plan)

- Agents read; only `WriteService` writes to ADO, and only approved artifacts.
- Every artifact is schema-validated, versioned, and pinned to the source revision it read.
- Every query is scoped by `team_id` (Postgres row-level security).
- Ticket text passes through PII redaction before any prompt.
- Tests use `FakeLlmClient`, `FakeAdoServer` (WireMock) and Postgres Testcontainers. Nothing calls a real service.

## Human gates (the loop stops here)

| Where | Decision |
|---|---|
| S0 | Anthropic key and provider terms; Entra app registration; ADO test org and PAT; commit policy |
| S1 | Approve the artifact schemas before the inbox is built on them |
| S2 | Pick the closed defects for the replay set; accept the first scorecard |
| S3 | Confirm test-plan IDs and the cap on new test cases |
| S6 | Confirm the release signal per team; write the first gate policy |
| S8 | Approve onboarding of the second team |

## Reuse sources

| Source | Take |
|---|---|
| ADO_Tester | `AzureDevOpsClient`, `AdoAuthHandler`, ADO models, `ILlmClient` + factory (no CLI providers), `CodeContextService`, `CodeVerifierAgent` verdict schema, `PostgresFtsSearch`, `SseWriter` + `EventBus`, `ReportHtmlBuilder`, test-plan index, `TeamsNotifier` |
| Beacon2 | `metrics.ts`, `TrendChart`, `StatTiles`, `FeatureBars`, `FilterBar`, `WiqlPopover`, settings and usage panels |
| claude-marketplace | Agent instructions: story-intelligence, code-intelligence, kb-builder, feature-intelligence, harvester, releaser, commentor; evidence labels; gated write pattern |

Port a piece only inside the task that needs it, with tests.

Where the loop finds them (read-only, sibling folders of this repo):
- `../ADO_Tester`: run `git pull` first; the local clone was 25 commits behind on 2026-11-18.
- `../Beacon2`: not cloned locally yet; `gh repo clone shironigun/Beacon2 ../Beacon2` before S5.
- `../claude-marketplace`: use the `dev` branch, which is ahead of `main` and is the only branch with `qa-engineering` and `feature-intelligence`.

Tasks in S4–S8 list fewer `Touches` than S0–S3. When a task has none, the loop names the files it changed in its log line, keeping within the folders `specs/architecture.md` assigns to that kind of code.
