# S8 · Evals and rollout

Goal: prove agent quality with ground truth, guard skill upgrades with evals, and onboard a second team without hand-holding.

Exit check: a skill upgrade is blocked by a worse scorecard in `S8_eval_gate_blocks_regression`; the onboarding wizard completes for a fixture team; all always-on checks pass.

### S8-01 [ ] Ground-truth metrics
- Do: root-cause accuracy (claimed files vs merged fix files), verdict vs execution, reopen after verification, dev agree rate, defect-cause mix.
- Done when: one test per metric.

### S8-02 [ ] Eval gate on skill upgrades
- Do: a new skill version runs the team's replay set on the app runtime; promoted only if the scorecard is not worse beyond a tolerance.
- Done when: `S8_eval_gate_blocks_regression` passes.

### S8-03 [ ] Confidence calibration
- Do: bucket predicted confidence vs actual on the eval set; hide confidence in the UI until calibrated.
- Done when: `Uncalibrated_confidence_is_hidden` passes.

### S8-04 [ ] Canary findings
- Do: occasionally inject a known-wrong claim into a brief (never written to ADO); record whether the reviewer caught it; catch rate to the person and lead only.
- Done when: `Canary_never_reaches_write_plan`, `Catch_rate_recorded` pass.

### S8-05 [ ] Review SLAs and escalation
- Do: SLA per artifact type, inbox ageing, escalation to the lead, Teams notification (port `TeamsNotifier`).
- Done when: `Breached_sla_notifies_lead` passes.

### S8-06 [ ] AI-quality dashboard
- Do: S8-01 metrics over time per agent and skill version; acceptance rate shown as secondary.
- Done when: Vitest render tests pass.

### S8-07 [ ] Onboarding wizard
- Read: marketplace `scan-and-confirm`
- Do: steps: connect → test connections → legal enablement check → module scan → QA confirms modules → seed KB → capture baseline → replay eval → lead accepts → enable hooks.
- Done when: `Wizard_blocks_hooks_until_scorecard_accepted`, `Wizard_blocks_without_legal_enablement` pass.

### S8-08 [ ] Agent settings per team
- Do: turn agents on or off, model and effort per agent, house-profile edits, with version history.
- Done when: `Disabled_agent_is_skipped` passes.

### S8-09 [ ] **[HUMAN]** Onboard the second team
- Do: run the wizard with a real second team; accept its scorecard.
