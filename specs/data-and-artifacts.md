# Data model and artifacts

## Tables (first version; later sub-plans add columns, never repurpose them)

| Table | Purpose | Added in |
|---|---|---|
| teams | Name, ADO org/project, area path, repo, branches, env→branch map, release signal, house profile, legal_enabled | S1 |
| team_members | user (Entra oid), team, role | S1 |
| team_secrets_ref | Key Vault / env reference per team credential (never the secret) | S1 |
| state_mappings | team's ADO states → stage (new, in_progress, resolved, closed, reopened, story_ready) | S1 |
| jobs | Queue: type, idempotency key, payload, status, attempts, run_after, locked_by | S1 |
| processed_events | Every hook or sweep event seen, for reconciliation | S1 |
| agent_runs | One orchestrator run: job, agents used, status, cost | S1 |
| llm_calls | Per call: run, agent, model, tokens in/out, cost_usd, ms | S1 |
| artifacts | Typed draft: type, version, work_item_id, source_revision, payload jsonb, status, superseded_by | S1 |
| reviews | Decision per artifact: reviewer, action, structured reason, edits diff, time_on_review | S1 |
| write_plans / write_steps | Dry-run plan and executed steps per approved artifact | S1 |
| audit_events | Append-only (no UPDATE/DELETE grant) | S1 |
| execution_results | QA-recorded run: test case or retest item, pass/fail, evidence link | S3 |
| test_case_index | Team's test plan cases, text, area, for search | S3 |
| releases / release_items | Detected releases and their tickets/commits | S6 |
| gate_policies / gate_overrides | Per-team rules and recorded overrides | S6 |
| kb_entries / kb_rules / kb_versions | KB with trust tier, provenance, cited symbols | S7 |
| kb_disputes | Agent evidence vs rule, awaiting QA | S7 |
| eval_sets / eval_runs / eval_results | Replay sets and scores | S2 (basic), S8 (gate) |
| baselines | Pre-pilot metrics per team | S5 |

## Artifact types

Every artifact payload has this envelope:

```json
{
  "artifactId": "…",
  "artifactType": "TriageBrief",
  "schemaVersion": 1,
  "createdAt": "2026-01-01T00:00:00Z",
  "runId": "…",
  "producedBy": { "agent": "lead", "skillVersion": "1.4.0", "model": "…" },
  "teamId": "…",
  "workItemId": 1001,
  "sourceRevision": 14,
  "codeCommit": "a1b2c3d",
  "environment": "prod",
  "upstream": ["artifact-id"],
  "claims": [
    { "id": "c1", "text": "…", "label": "Confirmed|Inferred|Assumption|Gap",
      "evidence": [{ "kind": "file|ticket|pr|testcase|kb", "ref": "path:line@commit or id" }],
      "risk": "high|normal" }
  ],
  "body": { }
}
```

`runId` points to the `agent_runs` row that produced the artifact, so the inbox can open the Agent Run View for it ("Watch run", replay). `producedBy` records the agent, the pinned skill version from `skills.lock.json` and the model.

| Type | `body` holds | Sub-plan |
|---|---|---|
| CodeFinding | summary (one line), rootCause, files[] (path, lines, why), confidence (0–1, overall) | S1 (single agent) |
| TriageBrief | screen (duplicate/config/data/needsInfo/proceed), rootCause, files, similarDefects, impact, specGapOrCodeBug, coverage (existing cases + caught?/gap), owner, cc, commentDraft | S2 |
| VerificationBrief | linkCheck, inProd, codeReviewVerdict (Fixed/Partial/NotFixed/NotInProd/Inconclusive), retestList, testCasePlan (link/edit/draft/flagWrong), executionVerdict (Unverified until QA), qaUpdateDraft | S3 |
| TestIntentBrief | gaps[] (question, about which criterion), perCriterion (covered/close/notCovered/willBreak + cases), regressionList, risk | S4 |
| BuildCheck | perCriterion (Implemented/Partial/NotFound + evidence), outOfScopeChanges, uncoveredPaths, retestList | S4 |
| ReleaseReport | items by theme, untrackedCommits, doneButNotShipped, riskByArea, gateResult | S6 |
| KbDelta | proposed entry/rule changes as diffs, source release | S7 |

Status path: `draft → in_review → approved | edited_approved | rejected → written | write_failed`, plus `superseded`.
