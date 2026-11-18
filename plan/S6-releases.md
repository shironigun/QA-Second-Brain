# S6 · Releases

Goal: detect each production release, report what shipped, gate the sign-off on policy, and watch for regressions afterwards.

Exit check: `S6_release_to_signoff` integration test passes; all always-on checks pass. Demo on the last real release.

### S6-01 [ ] **[HUMAN]** Release signal and first gate policy
- Do: per pilot team, choose the release signal (tag pattern, merge to branch, or pipeline) and write the first gate policy.

### S6-02 [ ] Release detection
- Do: detect the signal (hook + sweep), create `releases` with the previous release as base; hotfix as a release type; test-detect button in settings.
- Done when: `Tag_creates_release_with_previous_base`, `Hotfix_is_typed` pass.

### S6-03 [ ] Release inventory
- Read: marketplace `harvester`
- Do: commits between releases → linked work items; untracked commits; items marked Done that did not ship.
- Done when: `Untracked_commit_listed`, `Done_but_not_shipped_listed` pass.

### S6-04 [ ] Release report artifact
- Read: ADO_Tester `ReportService`, `ReportHtmlBuilder`
- Do: `ReleaseReport` grouped by theme, risk by area, retest checklist, offline HTML export.
- Done when: `Release_report_validates`, `Html_export_is_self_contained` pass.

### S6-05 [ ] Gate policies
- Do: rules: no open Sev1/Sev2, every shipped ticket has an approved Verification Brief, Sev1/2 have execution evidence, untracked commits explained. Evaluate per release.
- Done when: one test per rule.

### S6-06 [ ] Sign-off per area with overrides
- Do: QA signs each area; a failing rule needs an override with a reason, stored in `gate_overrides`; flagging an issue raises a defect from the report.
- Done when: `Signoff_blocked_by_failed_rule_without_override`, `Flag_creates_defect_draft` pass.

### S6-07 [ ] Release notes draft
- Read: marketplace `releaser`
- Do: dealer-facing notes drafted from the signed-off report, reviewed in the inbox.
- Done when: `Release_notes_only_after_signoff` passes.

### S6-08 [ ] Post-release watch
- Do: for N days after a release, new defects are matched against the release diff; matches are labelled "likely regression from release X".
- Done when: `Defect_touching_release_files_is_flagged` passes.

### S6-09 [ ] Rollback detection
- Do: detect a revert or rollback of a release and mark it, so S7 can roll back its KB delta.
- Done when: `Revert_marks_release_rolled_back` passes.

### S6-10 [ ] Release health dashboard
- Do: escaped defects per release, riskiest areas, untracked changes, sign-off time, overrides used.
- Done when: Vitest render tests pass.

### S6-11 [ ] End-to-end
- Done when: `S6_release_to_signoff` passes.
