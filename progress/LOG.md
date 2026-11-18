# Progress log

One line per finished or failed task: `YYYY-MM-DD · <task id> · <what changed> · <checks run>`

2026-11-18 · S0-02 · .NET 10 solution: 5 projects + 2 test projects, refs, nullable, warnings-as-errors in Core/Agents · dotnet build Release, dotnet test (web checks skipped: src/web not yet created)
2026-11-18 · S0-03 · compose Postgres 16, AppDbContext + design-time factory, empty InitialCreate migration, PostgresFixture (Testcontainers) · dotnet build Release, dotnet test (Db_migrates_on_empty_database green); also touched src/backend/QaSecondBrain.Infrastructure csproj for EF packages
2026-11-18 · S0-04 · Api host: JSON console logging, ProblemDetails, CORS from Cors:WebOrigins, GET /health with DB check (503 when unreachable) · dotnet build Release, dotnet test (Health_returns_ok_with_db green)
2026-11-18 · S0-05 · Worker: HeartbeatService (TimeProvider, configurable interval, clean cancel), JSON logging · dotnet build Release, dotnet test (Worker_stops_on_cancellation green)
2026-11-18 · S0-06 · Vite+React+TS strict+MUI 7 shell: app bar w/ Sign in + theme toggle, left nav w/ 6 placeholder pages, light/dark from tokens, React Query, shared API client with AbortSignal; ESLint replaces scaffolded oxlint, TS pinned ~6.0 (typescript-eslint peer range) · web build, test --run (5 green incl. nav), lint; dotnet build/test
2026-11-18 · S0-07 · TestKit project (added to Touches so Unit and Integration tests share fakes): FakeLlmClient (fixtures by agent, call log, overrides), FakeAdoServer on WireMock serving synthetic golden fixtures (defect 1001, story 1002, PR 501 + changes, test plan 10); fixtures are synthetic (TestOrg/example.test), adapted in shape only since the ADO_Tester golden holds write payloads and its pull was blocked by local changes · dotnet build Release, dotnet test (FakeLlm_returns_fixture, FakeAdo_serves_work_item green)
