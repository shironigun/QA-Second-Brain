# S1 · Foundation

Goal: the safe skeleton of the whole loop, end to end with one agent. A QA signs in, configures a team, clicks "Analyse now" on a defect, sees a CodeFinding in the inbox, approves it, and a comment reaches the fake ADO through the write service.

Exit check: integration test `S1_end_to_end_analyse_approve_write` passes against FakeAdoServer + FakeLlmClient, plus all always-on checks. Demo the same flow by hand against the ADO test project.

### S1-00 [ ] **[HUMAN]** Entra and ADO test project
- Do: create the Entra app registration (SPA + API scopes) and an ADO test project with a PAT (read work items, code, test plans; write work items only). Record in `progress/BLOCKERS.md` → Resolved *where* each value is stored, never the value. Needed by S1-04 and S1-07.

### S1-01 [x] **[HUMAN]** Approve artifact envelope
- Read: `specs/data-and-artifacts.md`
- Do: confirm the envelope and the CodeFinding/TriageBrief shapes, or edit the spec. The loop builds on them from here.

### S1-02 [ ] Teams and membership schema
- Depends: S1-01
- Touches: Infrastructure/Data (entities, migration), Core/Teams
- Do: `teams`, `team_members`, `team_secrets_ref`, `state_mappings` tables and entities.
- Done when: `dotnet test --filter Teams_schema` passes.

### S1-03 [ ] Row-level security
- Depends: S1-02
- Touches: migration, `TeamScope` (sets `app.team_id` per connection), tests
- Do: enable RLS on every team table; a policy reading `current_setting('app.team_id')`. App role is not table owner.
- Done when: `Cross_team_read_returns_nothing` and `Cross_team_write_is_rejected` pass.

### S1-04 [ ] Entra sign-in (API)
- Depends: S1-02
- Touches: Api/Auth, tests
- Do: JWT bearer from Entra, role resolution from `team_members`, policies `Qa`, `Lead`, `Admin`. Test auth handler for integration tests.
- Done when: `Unauthenticated_is_401`, `Non_member_is_403` pass.

### S1-05 [ ] Entra sign-in (web)
- Depends: S0-06, S1-04
- Touches: `src/web/src/auth/**`
- Do: MSAL sign-in, token on API calls, team switcher in the app bar.
- Done when: Vitest `shows team switcher after sign-in` (MSAL mocked) passes.

### S1-06 [ ] Team settings API and page
- Depends: S1-03, S1-05
- Touches: Api/Teams controller, web `pages/settings/**`
- Do: create and edit a team: ADO org/project, area path, repo, production branch, env→branch map, state mappings, house profile text, people to always cc. Secrets are referenced by name, never returned.
- Done when: API tests `Lead_can_update_team`, `Qa_cannot_update_team`; Vitest `settings form validates required fields`.

### S1-07 [ ] ADO client port
- Depends: S0-07
- Read: ADO_Tester `AzureDevOpsClient.cs`, `AdoAuthHandler.cs`, `AdoModels.cs`
- Touches: Infrastructure/Ado/**
- Do: port read methods only (work item, revisions, comments, WIQL, PR, PR changes, file content, code search, test cases) plus `AddComment` and `UpdateField` for the write service. Credentials resolved per team via `team_secrets_ref`. Every call takes a `CancellationToken`.
- Done when: `Ado_client_*` tests pass against FakeAdoServer.

### S1-08 [ ] Connection tests
- Depends: S1-06, S1-07
- Touches: Api/Teams, web settings
- Do: "Test" buttons for ADO, repo access and AI key. Each returns pass/fail and a safe message.
- Done when: `Connection_test_reports_failure_without_secret_leak` passes.

### S1-09 [ ] PII redaction
- Depends: S0-02
- Touches: Infrastructure/Redaction/**
- Do: redact emails, phone numbers, person names from a team's known-name list, VINs and addresses before any prompt; keep a reversible map in memory for the UI only.
- Done when: `Redactor_*` table-driven tests pass (20+ cases).

### S1-10 [ ] Job queue
- Depends: S1-03
- Touches: Infrastructure/Jobs/**, Worker/JobRunner.cs, migration
- Do: `jobs` and `processed_events` tables, enqueue with idempotency key (team, work item, event, revision), claim with `FOR UPDATE SKIP LOCKED`, one active job per work item, retry with backoff, dead-letter after N attempts.
- Done when: `Duplicate_event_enqueues_once`, `Same_item_jobs_run_in_order`, `Failed_job_retries_then_dead_letters` pass.

### S1-11 [ ] LLM client and cost log
- Depends: S0-07
- Read: ADO_Tester `Llm/ILlmClient.cs`, `LlmClientFactory.cs`
- Touches: Infrastructure/Llm/**, migration (`llm_calls`, `agent_runs`)
- Do: `ILlmClient` with `ClaudeCliClient` first (see `specs/architecture.md`: `claude -p --output-format json`, prompt on stdin, no tools, timeout, cancellation, parse tokens and `total_cost_usd`). Add `AnthropicApiClient` behind the same interface, selected by `Llm:Provider`. Every call writes `llm_calls` with provider, tokens and `decimal` cost.
- Done when: `Llm_call_is_logged_with_cost` (FakeLlmClient decorated by the logging wrapper), `Cli_client_parses_json_result` and `Cli_client_kills_process_on_cancel` pass. CLI tests use a stub executable path, never the real `claude`.

### S1-12 [ ] Skill loader
- Depends: S0-02
- Touches: `skills/`, `skills.lock.json`, Agents/Skills/**
- Do: load a skill folder (SKILL.md + references + `tools.json`) at the locked version; fail when a declared tool is not registered. Copy `code-intelligence` from the marketplace as the first skill.
- Done when: `Loader_rejects_missing_tool`, `Loader_reads_locked_version` pass.

### S1-13 [ ] Artifact store and schema validation
- Depends: S1-03
- Touches: Core/Artifacts, Infrastructure (`artifacts`, `reviews`, `audit_events`), Agents/Validation
- Do: envelope type, JSON schema per artifact type, validate-then-store, status transitions per the spec, `superseded`. `audit_events` gets INSERT only.
- Done when: `Invalid_payload_is_rejected`, `Illegal_status_transition_throws`, `Audit_rows_cannot_be_updated` pass.

### S1-14 [ ] Code agent (single agent)
- Depends: S1-07, S1-09, S1-11, S1-12, S1-13
- Read: ADO_Tester `CodeContextService.cs`
- Touches: Agents/Agents/Code/**, Infrastructure/CodeContext/**
- Do: port CodeContext (anchor on linked PRs/commits, fall back to code search, byte budget). Agent reads the redacted ticket in `<untrusted>` fences, gets read tools only, and produces a `CodeFinding`. If over budget, the claim is a Gap labeled "insufficient context".
- Done when: `Code_agent_produces_valid_finding`, `Prompt_injection_text_does_not_reach_tools`, `Over_budget_returns_inconclusive` pass.

### S1-15 [ ] Analyse now
- Depends: S1-10, S1-14
- Touches: Api/Tickets, Worker handler, web ticket page
- Do: `POST /teams/{id}/tickets/{wi}/analyse` enqueues a job; the worker runs the Code agent and stores the artifact as `in_review`.
- Done when: `Analyse_now_creates_in_review_artifact` passes.

### S1-16 [ ] Live progress
- Depends: S1-15
- Read: ADO_Tester `SseWriter.cs`, `EventBus.cs`, `useEventStream.ts`
- Touches: Api/Streaming, Core/Events, web `hooks/useEventStream.ts`, `components/AgentProgress.tsx`
- Do: SSE stream per run, scoped by team, using the event contract in `specs/ui-ux.md` §5.6 (`seq`, `Last-Event-ID` resume). Events are stored so a run can be replayed.
- Done when: `Sse_stream_is_team_scoped`, `Sse_resumes_from_last_event_id`; Vitest `AgentProgress renders steps from events`.

### S1-16b [ ] Agent Run View
- Depends: S1-16
- Read: `specs/ui-ux.md` §5
- Touches: web `features/runs/**`, `pages/runs/**`, `src/web/package.json` (`@xyflow/react`, `elkjs`, `framer-motion`), `tests/fixtures/events/**`
- Do: the live flow graph (node types and states, animated edges, auto-layout, follow-live camera, minimap), inspector tabs, timeline with replay scrubber, run header with live totals and activity ticker, reduced-motion fallback, keyboard navigation between nodes. Driven only by events; a recorded event stream in `tests/fixtures/events/` powers tests and a demo route.
- Done when: Vitest `run graph adds nodes from events`, `node shows running then done states`, `replay scrubber moves graph to playhead`, `reduced motion disables glow`, `graph is keyboard navigable`, plus axe passes on the page.

### S1-17 [ ] Review inbox
- Depends: S1-15
- Touches: Api/Reviews, web `pages/inbox/**`
- Do: list in-review artifacts with age; detail view shows each claim with its label chip and evidence links; actions Approve, Edit then approve, Reject with a structured reason (wrong root cause, wrong module, missing context, hallucinated file, style), Re-run with note. Edits stored as a diff. Time on review recorded. Layout and shortcuts per `specs/ui-ux.md` §4 and §6 (split view, `a`/`e`/`r`/`n`/`p`, optimistic actions with undo, "Watch run" link to the Agent Run View).
- Done when: API `Reject_requires_reason`, `Edit_is_stored_as_diff`; Vitest `inbox shows label chips`, `reject dialog requires reason`.

### S1-18 [ ] Write plan and write service
- Depends: S1-07, S1-13, S1-17
- Touches: Core/Writes, Infrastructure/Writes/**, migration (`write_plans`, `write_steps`), web dry-run panel
- Do: approved artifact → write plan (dry run shown in the inbox before approval) → saga that posts the comment with a revision check, records each step, retries, marks `write_failed` with reason. `WriteService` is not resolvable from the agent container.
- Done when: `Unapproved_artifact_cannot_be_written`, `Revision_conflict_stops_write`, `Agents_container_has_no_write_service` pass.

### S1-19 [ ] End-to-end
- Depends: S1-01 through S1-18
- Touches: IntegrationTests/S1EndToEnd.cs
- Done when: `S1_end_to_end_analyse_approve_write` passes.
