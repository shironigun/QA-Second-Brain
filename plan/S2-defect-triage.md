# S2 · Defect triage

Goal: a defect created on the SWAT board produces a full Triage Brief automatically, and after approval a root-cause comment in the house format. Replay on closed defects gives a scorecard.

Exit check: `S2_hook_to_posted_comment` integration test passes; replay over the fixture set produces a scorecard row per defect; all always-on checks pass. Demo: create a defect in the ADO test project and watch the brief appear.

### S2-01 [ ] Service hook intake
- Touches: Api/Hooks, tests
- Do: `POST /hooks/ado/{teamId}` with a per-team shared secret; accepts workitem.created / workitem.updated; ignores items outside the team's area path; maps state via `state_mappings`; enqueues idempotently.
- Done when: `Hook_with_bad_secret_is_401`, `Out_of_area_item_is_ignored`, `Duplicate_hook_enqueues_once` pass.

### S2-02 [ ] Meaningful-change filter and debounce
- Depends: S2-01
- Touches: Core/Triggers
- Do: re-run only when state, repro steps, linked PRs or severity change; debounce 2 minutes per item.
- Done when: `Title_edit_does_not_rerun`, `Repro_change_reruns_once_after_debounce` pass.

### S2-03 [ ] Reconciliation sweep
- Depends: S2-01
- Touches: Worker/Sweeps
- Do: every 15 minutes, WIQL for team items changed since last sweep; enqueue anything without a processed event.
- Done when: `Missed_hook_is_recovered_by_sweep` passes.

### S2-04 [ ] Budget cap and circuit breaker
- Depends: S2-01
- Touches: Core/Budget, Worker
- Do: monthly team budget and per-run token ceiling from settings; over budget → jobs park with status `budget_hold` and the UI shows it.
- Done when: `Over_budget_parks_job`, `Run_stops_at_token_ceiling` pass.

### S2-05 [ ] Environment → commit resolution
- Touches: Infrastructure/CodeContext
- Do: read the reported environment from a configured field, map to branch, resolve the head commit at the defect's created date; the brief records the commit.
- Done when: `Prod_defect_reads_prod_branch_commit`, `Unknown_env_is_a_gap_claim` pass.

### S2-06 [ ] Classifier (quick screen)
- Touches: Agents/Agents/Classifier
- Do: cheap model; outputs duplicate (with candidate ids), already fixed in prod, config/data, needs info, or proceed. Uses `history.similar_defects` (FTS over closed defects).
- Done when: fixture tests for each outcome pass.

### S2-07 [ ] History agent
- Touches: Agents/Agents/History, Infrastructure/Search (port `PostgresFtsSearch`)
- Do: similar past defects, earlier regressions in the same files, prior fixes.
- Done when: `History_agent_cites_ticket_ids` passes.

### S2-08 [ ] Impact agent
- Touches: Agents/Agents/Impact
- Do: affected flows, integrations, dealer segments, from code callers and KB.
- Done when: `Impact_agent_output_validates` passes.

### S2-09 [ ] Requirement-gap agent (defect mode)
- Touches: Agents/Agents/RequirementGap, skill copy of `feature-intelligence`
- Do: was the behaviour specified? Output spec gap / code bug / unclear with evidence.
- Done when: fixture tests for spec-gap and code-bug cases pass.

### S2-10 [ ] Coverage agent (triage mode)
- Touches: Agents/Agents/Coverage, Infrastructure/TestIndex (port ADO_Tester test-plan index), migration `test_case_index`
- Do: index the team's test plan; find cases for the affected area; say whether one exists and whether it should have caught the defect.
- Done when: `Coverage_reports_no_case`, `Coverage_reports_case_that_missed_it` pass.

### S2-11 [ ] Lead agent and orchestrator
- Depends: S2-06 … S2-10, S1-14
- Touches: Agents/Orchestrators/Triage
- Do: classifier first; on `proceed` run Code, History, Impact, RequirementGap, Coverage in parallel (effort per agent scales with module size); lead agent merges into a `TriageBrief`, resolves conflicting claims, suggests owner and cc.
- Done when: `Short_brief_skips_fanout`, `Full_brief_contains_all_sections`, `Conflicting_claims_are_marked` pass.

### S2-12 [ ] House-format root-cause comment
- Depends: S2-11
- Touches: Agents/Agents/Commentor (skill copy of `commentor`), Writes
- Do: comment layout: @assignee "estimated, please verify" → per-flow explanation → why the dealer saw it → cc line last; developer detail to the Root Cause Details field. Format from the team's house profile.
- Done when: `Comment_matches_house_template` (snapshot) passes.

### S2-13 [ ] Inbox: triage view
- Depends: S2-11
- Touches: web `pages/inbox/TriageBrief.tsx`
- Do: sections in flow order, high-risk claims need individual ticks before approve, coverage gap callout, dry-run of the comment.
- Done when: Vitest `approve disabled until high-risk claims ticked` passes.

### S2-14 [ ] Developer agree/disagree
- Depends: S2-12
- Touches: Api/Feedback, web ticket page
- Do: a developer marks the posted root cause Agree or Disagree with a note; stored for accuracy metrics.
- Done when: `Dev_feedback_is_recorded` passes.

### S2-15 [ ] **[HUMAN]** Pick the replay set
- Do: choose 10–20 closed, verified SWAT defects for the team. Record ids in `progress/BLOCKERS.md` → Resolved.

### S2-16 [ ] Replay and scorecard
- Depends: S2-11, S2-15
- Touches: Core/Evals, Worker/Replay, migration (`eval_sets`, `eval_runs`, `eval_results`), web `pages/evals/**`
- Do: run triage on each defect at its creation revision with final comments hidden; score root-cause match against the merged fix's files, owner match, coverage claim; show a scorecard.
- Done when: `Replay_hides_post_creation_revisions`, `Scorecard_scores_file_overlap` pass.

### S2-17 [ ] **[HUMAN]** Accept first scorecard
- Do: review the scorecard; decide whether hooks go live for the pilot team.

### S2-18 [ ] End-to-end
- Done when: `S2_hook_to_posted_comment` passes.
