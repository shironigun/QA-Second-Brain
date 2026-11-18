# S7 · Knowledge base lifecycle

Goal: a KB that learns only from approved, released work, can be disputed and rolled back, and is readable by dev tools.

Exit check: `S7_release_to_kb_version` integration test passes; all always-on checks pass.

### S7-01 [ ] KB schema
- Read: marketplace `kb-builder` (entry shape)
- Do: `kb_entries` (module), `kb_rules` (testable rule, cited symbols, trust tier, provenance), `kb_versions`.
- Done when: `Rule_requires_provenance` passes.

### S7-02 [ ] Seed from history
- Do: from the team's test plan and closed verified defects, propose entries in tier `Proposed` only.
- Done when: `Seeded_rules_are_proposed_tier` passes.

### S7-03 [ ] KB Delta after sign-off
- Do: on release sign-off, propose rule changes from that release's approved briefs as a `KbDelta` with diffs.
- Done when: `Delta_only_from_signed_off_release` passes.

### S7-04 [ ] KB review in the inbox
- Do: diff view; approve moves rules to `Approved` and creates a KB version.
- Done when: Vitest `kb delta shows before and after`.

### S7-05 [ ] Trust promotion
- Do: an approved rule that survives N releases with no contradiction becomes `Proven`.
- Done when: `Rule_promoted_after_n_clean_releases` passes.

### S7-06 [ ] Disputes
- Do: when agent evidence contradicts a rule, create a `kb_disputes` item instead of following either; resolve in the inbox.
- Done when: `Contradiction_creates_dispute_not_override` passes.

### S7-07 [ ] Rollback
- Depends: S6-09
- Do: a rolled-back release reverts its KB delta automatically.
- Done when: `Rollback_reverts_delta` passes.

### S7-08 [ ] Symbol-level staleness
- Do: when a release changes a rule's cited symbols or lines, flag the rule for review.
- Done when: `Unrelated_file_change_does_not_flag`, `Cited_method_change_flags` pass.

### S7-09 [ ] Agents use the KB
- Do: `kb.search` and `kb.get_entry` tools rank Proven > Approved > Proposed and show tier in evidence.
- Done when: `Search_ranks_by_tier` passes.

### S7-10 [ ] KB read API and MCP endpoint
- Do: read-only API and an MCP server exposing search and entries per team, for dev agents and Claude Code.
- Done when: `Mcp_search_is_team_scoped` passes.

### S7-11 [ ] Knowledge pages
- Do: browse modules, rules with tier and provenance, history; "Explain to QA / dev / product" view.
- Done when: Vitest render tests pass.

### S7-12 [ ] End-to-end
- Done when: `S7_release_to_kb_version` passes.
