# S3 · Defect verification

Goal: when a defect is Resolved with a PR, the app checks the fix, plans test cases against the existing test plan, waits for QA's execution result, and posts the QA UPDATE.

Exit check: `S3_resolved_to_closed` integration test passes; all always-on checks pass. Demo on a real resolved defect in the test project.

### S3-01 [ ] **[HUMAN]** Test plan scope and cap
- Do: confirm which test plan/suites the team uses for defects and the cap on new cases per defect (default 3).

### S3-02 [ ] Resolved trigger
- Touches: Core/Triggers, Worker handler
- Do: on mapped stage `resolved`, enqueue verification; the previous Triage Brief is upstream.
- Done when: `Resolved_enqueues_verification_with_upstream` passes.

### S3-03 [ ] Link completeness
- Touches: Agents/Checks/LinkCheck
- Do: compare commits mentioning the work item id with linked PRs; list missing links.
- Done when: `Unlinked_commit_is_reported` passes.

### S3-04 [ ] Production-branch check
- Touches: Agents/Checks/InProd
- Do: are the fix commits reachable from the production branch head? If not, verdict `NotInProd`.
- Done when: `Fix_on_qa_only_is_not_in_prod` passes.

### S3-05 [ ] Verify agent (code-review verdict)
- Read: ADO_Tester `CodeVerifierAgent`, `code_verifier_system.txt`
- Touches: Agents/Agents/Verify
- Do: PR diff vs defect vs triage root cause → Fixed / Partial / NotFixed / Inconclusive with evidence.
- Done when: fixture tests for each verdict pass.

### S3-06 [ ] Retest list
- Touches: Agents/Agents/Retest
- Do: other features the diff touches, from callers and KB atlas.
- Done when: `Retest_list_includes_callers_of_changed_method` passes.

### S3-07 [ ] Test-case planner
- Depends: S2-10
- Touches: Agents/Agents/TestCases (skill copy of `author-ui-cases` / `author-api-cases`)
- Do: four outcomes: link exact match, propose edit to close match (diff), draft new (capped, each with reason), flag existing case that passed but missed the bug.
- Done when: one fixture test per outcome; `Draft_count_respects_cap` passes.

### S3-08 [ ] Verification orchestrator
- Depends: S3-03 … S3-07
- Touches: Agents/Orchestrators/Verification
- Do: build the `VerificationBrief`; `executionVerdict` starts as `Unverified`.
- Done when: `Brief_starts_unverified_by_execution` passes.

### S3-09 [ ] Execution results
- Touches: Api/Execution, migration `execution_results`, web
- Do: QA records pass/fail and an evidence link per test case and retest item.
- Done when: `Cannot_approve_sev1_without_execution_evidence` passes.

### S3-10 [ ] Inbox: verification view
- Touches: web `pages/inbox/VerificationBrief.tsx`
- Do: two verdicts side by side, retest checklist, test-case plan with diffs, dry-run of all writes.
- Done when: Vitest `shows AI verdict and execution verdict separately` passes.

### S3-11 [ ] Writes: test cases and QA UPDATE
- Touches: Writes, Agents/Agents/Commentor
- Do: link existing cases, apply approved edits, create new cases in the suite and link them, post QA UPDATE in house format, optionally close the item if the team allows.
- Done when: `Writes_link_edit_create_in_order`, `Qa_update_matches_template` pass.

### S3-12 [ ] Not fixed → back to developer
- Touches: Writes, Core/Triggers
- Do: approved `NotFixed` / `NotInProd` posts a comment to the assignee and moves state back per mapping.
- Done when: `Not_fixed_returns_item` passes.

### S3-13 [ ] Reopen handling
- Touches: Core/Triggers, Agents/Orchestrators/Triage
- Do: closed→active re-triages with the failed fix as context; counts against verification accuracy.
- Done when: `Reopen_retriages_with_previous_brief` passes.

### S3-14 [ ] Superseded briefs
- Touches: Core/Artifacts, web
- Do: material change after approval marks the brief superseded and offers re-run.
- Done when: `Repro_change_after_approval_supersedes` passes.

### S3-15 [ ] End-to-end
- Done when: `S3_resolved_to_closed` passes.
