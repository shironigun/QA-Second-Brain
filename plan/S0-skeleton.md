# S0 · Repo and skeleton

Goal: an empty but working app that every later task can build on.

Exit check: `docker compose -f deploy/docker-compose.yml up -d`, then all always-on checks pass, `GET /health` returns 200 with DB status `ok`, and the web shell renders the app bar with a "Sign in" button.

Task format: `[ ]` todo · `[x]` done · `[!]` blocked · **[HUMAN]** the loop stops here.

### S0-01 [x] Accounts moved
- Entra app registration and the ADO test project are not needed until S1. They are now task S1-00.

### S0-02 [x] Backend solution
- Depends: none
- Touches: `src/backend/**`, `global.json`, `Directory.Build.props`
- Do: create the five projects and two test projects from `specs/architecture.md`, with project references Api→Core/Agents/Infrastructure, Worker→same, Agents→Core, Infrastructure→Core. Turn on nullable, and warnings as errors for Core and Agents.
- Done when: `dotnet build src/backend/QaSecondBrain.sln -c Release` and `dotnet test src/backend/QaSecondBrain.sln` pass (one placeholder test per test project).

### S0-03 [x] Postgres and EF baseline
- Depends: S0-02
- Touches: `deploy/docker-compose.yml`, `src/backend/QaSecondBrain.Infrastructure/Data/**`, `src/backend/tests/QaSecondBrain.IntegrationTests/**`
- Do: compose file with Postgres 16. `AppDbContext` with no tables yet, Npgsql, an empty initial migration. A Testcontainers fixture `PostgresFixture` that applies migrations.
- Done when: integration test `Db_migrates_on_empty_database` passes.

### S0-04 [x] API host and health
- Depends: S0-03
- Touches: `src/backend/QaSecondBrain.Api/**`, tests
- Do: minimal host, ProblemDetails, structured logging, `GET /health` reporting DB connectivity, CORS for the web dev origin from config.
- Done when: integration test `Health_returns_ok_with_db` passes.

### S0-05 [x] Worker host
- Depends: S0-03
- Touches: `src/backend/QaSecondBrain.Worker/**`, tests
- Do: generic host with one no-op `BackgroundService` that logs a heartbeat and stops cleanly on cancellation.
- Done when: unit test `Worker_stops_on_cancellation` passes.

### S0-06 [x] Web shell
- Depends: none
- Touches: `src/web/**`
- Do: Vite + React + TS strict + MUI v7 + React Query + React Router + Vitest + ESLint. App bar, left nav (Inbox, Tickets, Releases, Knowledge, Dashboards, Settings) with placeholder pages, light/dark theme from tokens. Shared API client with `AbortSignal` support.
- Done when: `npm --prefix src/web run build`, `run test -- --run` (renders nav test), `run lint` pass.

### S0-07 [ ] Test fakes
- Depends: S0-02
- Touches: `src/backend/tests/**/Fakes/**`, `tests/fixtures/**`
- Do: `FakeLlmClient` (returns canned JSON by agent name from `tests/fixtures/llm/`), `FakeAdoServer` on WireMock with golden fixtures (one defect, one story, one PR with diff, one test plan) adapted from ADO_Tester's `golden/ado_golden.json`.
- Done when: unit tests `FakeLlm_returns_fixture` and `FakeAdo_serves_work_item` pass.

### S0-08 [ ] CI pipeline
- Depends: S0-04, S0-06
- Touches: `.github/workflows/ci.yml` or `azure-pipelines.yml` (pick the one matching where the repo will live; record the choice in the log)
- Do: run the always-on checks on every push.
- Done when: the workflow file passes a local lint (`actionlint` or `az pipelines` validate if available; otherwise YAML parse) and the checks it lists match `PROMPT.md`.
