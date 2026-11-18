# S5 · Dashboards and cost

Goal: board health, AI cost and the pre-pilot baseline on screen. Can run in parallel with S2–S4: it touches only `pages/dashboards/**`, `Api/Metrics/**`, `Core/Metrics/**`.

Exit check: dashboards render from fixture data with every number's query viewable; all always-on checks pass.

### S5-01 [ ] Board snapshot sync
- Do: scheduled WIQL pull of the team's board into a `board_items` table (normalised dates, stage via mapping, area).
- Done when: `Snapshot_normalises_states_via_mapping` passes.

### S5-02 [ ] Metrics engine
- Read: Beacon2 `lib/metrics.ts`
- Do: port to C#: opened vs closed, backlog trend, cycle time mean and median, reopen rate, by area and person.
- Done when: metric unit tests matching Beacon2's expected values pass.

### S5-03 [ ] Query transparency
- Read: Beacon2 `lib/wiql.ts`, `WiqlPopover.tsx`
- Do: each metric returns the WIQL / filter used; the web shows it in a popover.
- Done when: Vitest `popover shows query for each tile`.

### S5-04 [ ] Board dashboard page
- Read: Beacon2 `TrendChart`, `StatTiles`, `FeatureBars`, `FilterBar`
- Do: port components to the app's MUI theme; filters for time, type, area, person.
- Done when: Vitest render tests with loading, error, empty states.

### S5-05 [ ] Baseline capture
- Do: per team, compute QA minutes per defect (from state timestamps), reopen rate and escaped defects for the 90 days before go-live; freeze into `baselines`.
- Done when: `Baseline_is_frozen_at_go_live` passes.

### S5-06 [ ] Cost dashboard
- Read: Beacon2 `UsagePanel.tsx`
- Do: tokens and cost per team, agent, run and month from `llm_calls`; budget alerts.
- Done when: `Cost_rollup_matches_llm_calls`; Vitest `budget alert shows above 80%`.

### S5-07 [ ] Reviewer stats (private)
- Do: time on review, edits, rejections per reviewer, visible to that person and the lead only.
- Done when: `Viewer_cannot_see_reviewer_stats` passes.
