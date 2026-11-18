# Ralph loop prompt: QA Second Brain

You are building QA Second Brain, one task per iteration. This prompt never changes. Everything you know about progress lives in files.

## Every iteration

1. Read `plan/00-master-plan.md`. Find the **active sub-plan**: the first one whose status is not `done`.
2. Open that sub-plan. Pick the **first task with `[ ]`** whose `Depends` are all `[x]`.
   - If it is tagged **[HUMAN]**, do not do it. Write what the human must decide or do to `progress/BLOCKERS.md`, then output `<promise>NEEDS HUMAN</promise>` and stop.
   - If every task in the sub-plan is `[x]`, run the sub-plan's **Exit check**. If it passes, set the sub-plan status to `done` in the master plan and continue with the next sub-plan in the next iteration.
3. Read only what the task needs: its `Read` list, `specs/`, and the files in its `Touches` list.
4. Do the task **test-first**: write the failing test, make it pass, refactor.
5. Run the task's **Done when** commands. Also run the always-on checks below.
6. If everything passes: change the task to `[x]`, and append one line to `progress/LOG.md`:
   `YYYY-MM-DD · <task id> · <what changed> · <checks run>`
7. If it fails: fix it in this iteration if you can. If the same task has failed in 3 iterations (count them in `progress/LOG.md`), mark it `[!]`, write the cause and what you tried to `progress/BLOCKERS.md`, output `<promise>NEEDS HUMAN</promise>` and stop.

## Always-on checks (from repo root)

- `dotnet build src/backend/QaSecondBrain.sln -c Release`
- `dotnet test src/backend/QaSecondBrain.sln`
- `npm --prefix src/web run build`
- `npm --prefix src/web run test -- --run`
- `npm --prefix src/web run lint`

Skip a check only if the project it targets does not exist yet (before S0 creates it).

## Rules

- One task per iteration. Do not start the next one.
- Only touch files in the task's `Touches` list, plus tests for them. If you need another file, add it to `Touches` and say why in the log line.
- Follow `specs/conventions.md`. Agents never get ADO write tools. Only the write service writes to ADO.
- Never call a real LLM, real ADO org or real database in tests. Use the fakes and Testcontainers described in `specs/conventions.md`.
- No secrets in tracked files. Config comes from environment variables or user-secrets.
- Do not edit `PROMPT.md`, `specs/`, or a task's `Done when` to make a check pass. If a spec looks wrong, record it in `progress/BLOCKERS.md` and stop with `NEEDS HUMAN`.
- Git: commit every small finished piece of work (a passing test, a port, a migration, a ticked task) with message `<task id>: <what changed>`, then `git push origin main`. Never force-push. If a push fails, log it in `progress/BLOCKERS.md` and continue.
- LLM: locally, agents run through the Claude CLI provider (`Llm:Provider=ClaudeCli`) until an API key exists.

## Finishing

Output `<promise>SLICE DONE</promise>` only when the sub-plan named in the loop command is fully `[x]` and its Exit check passes. Never output a promise that is not true.
