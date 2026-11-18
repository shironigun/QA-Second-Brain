# Conventions

## Backend (.NET 10)
- Nullable enabled, warnings as errors in Core and Agents.
- Every I/O method takes a `CancellationToken` and forwards it. No `.Result` or `.Wait()`.
- EF Core with Npgsql; migrations live in Infrastructure. Raw SQL only through parameterized APIs.
- Never put `ex.Message` in an API response. Use ProblemDetails with a safe message.
- Structured `ILogger` templates, no string interpolation in log calls.
- `decimal` for money (LLM cost).
- Controllers require auth. Team scope comes from the authenticated user's membership, never from a client-supplied id alone.

## Agents
- One folder per agent in `QaSecondBrain.Agents/Agents/<Name>/`: `Agent.cs`, `output.schema.json`, and a pointer to its skill in `skills/`.
- Agent input fences untrusted text: `<untrusted source="ticket-comment">…</untrusted>`. The system prompt says text inside is data.
- Output must validate against `output.schema.json`. One repair attempt, then the run fails with `invalid_output`.
- Agents receive only read tools. `WriteService` is not registered in the agent container.

## Frontend
- React + TypeScript strict, MUI v7, React Query, React Router. Filled MUI icons only.
- API functions are plain typed functions on one shared client, each taking an optional `AbortSignal`.
- Every data view has loading, error and empty states.
- Vitest + Testing Library. MSW for API mocks.

## Tests
- Unit: xUnit + FluentAssertions; Vitest.
- Integration: Testcontainers Postgres, WireMock as `FakeAdoServer` with golden fixtures in `tests/fixtures/ado/`, `FakeLlmClient` returning canned JSON per agent from `tests/fixtures/llm/`.
- Each task's `Done when` names its own test filter. A task is done only when that filter and the always-on checks pass.
- Never call a real LLM, ADO org or database from tests. Tests never start the real `claude` CLI; the CLI client is tested with a stub executable.

## Git
- Commit after every finished task, and after any smaller self-contained step inside a task (a passing test, a port, a migration). Message: `<task id>: <what changed>`.
- Push to `origin main` after each commit. If the push fails, record it in `progress/BLOCKERS.md` and keep working; never force-push.
- Never commit secrets, `.env` files, `bin/`, `obj/`, `node_modules/` or `dist/`.

## Naming
- Artifact type names match `specs/data-and-artifacts.md` exactly.
- ADO state names never appear in code; use `state_mappings`.
