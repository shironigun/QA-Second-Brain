# QA Second Brain

AI agents investigate defects and stories on a team's Azure DevOps board, draft findings, and QA approves before anything is written back. Vision: https://claude.ai/artifact/LHHUUZtjvtUBi7E7wGUMay

Stack: React + MUI (Vite, TypeScript) · .NET 10 · PostgreSQL 16.

## How this repo is built

The plan is executed with the Ralph loop plugin, one sub-plan at a time.

| File | Purpose |
|---|---|
| `PROMPT.md` | The fixed prompt the loop feeds every iteration |
| `plan/00-master-plan.md` | Sub-plans, order, status |
| `plan/S*.md` | Tasks for each sub-plan |
| `specs/` | Architecture, data model, artifact schemas, conventions. The loop reads these but never edits them. |
| `progress/LOG.md` | One line per finished task |
| `progress/BLOCKERS.md` | Anything that needs a human |

## Before the first run

1. Repo: https://github.com/shironigun/QA-Second-Brain. Branches: `main` (protected, PR only), `dev`, and one `feature/<sub-plan>` branch per sub-plan. The loop commits and pushes every small piece of work to the feature branch and opens a PR into `dev` when the sub-plan is done. You merge it, then open or merge the `dev` → `main` PR.
2. LLM: local runs use the Claude CLI (`Llm:Provider=ClaudeCli`) until an Anthropic API key is available. `claude` must be on PATH and signed in.
3. Windows: the plugin's stop hook calls plain `bash`, which can resolve to WSL. If the loop does not continue after the first iteration, point the hook at Git Bash as the plugin README describes.
4. Settle the `[HUMAN]` task in `S0` (Entra app registration, ADO test org).

## Running one sub-plan

```
/ralph-loop "Read PROMPT.md and follow it. Work only on sub-plan S1." --completion-promise "SLICE DONE" --max-iterations 40
```

The loop stops on `SLICE DONE`, at the iteration cap, or when it writes `NEEDS HUMAN` to the transcript and `progress/BLOCKERS.md`. The completion promise matches one exact string, so `NEEDS HUMAN` does not end the loop by itself. When you see it, run `/cancel-ralph`, resolve the blocker, and start the loop again.

Review the sub-plan's demo (listed in its Exit check) before starting the next one.
