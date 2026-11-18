# S4 · Story flow

Goal: a story moving to Ready produces a Test Intent Brief (gaps and coverage per acceptance criterion); after PRs land, a Build Check; bugs found during story testing enter the defect flow.

Exit check: `S4_ready_to_build_check` integration test passes; all always-on checks pass. Demo on a real story.

### S4-01 [ ] Story Ready trigger
- Do: mapped stage `story_ready` enqueues story intake; manual "Analyse now" works on stories.
- Done when: `Ready_story_enqueues_intake` passes.

### S4-02 [ ] Story context agent
- Touches: Agents/Agents/StoryContext (skill copy of `story-intelligence`)
- Do: description, AC, epic, siblings, linked docs, comments, related bugs, module KB rules.
- Done when: `Story_context_includes_parent_and_ac` passes.

### S4-03 [ ] AC parser
- Touches: Core/Stories
- Do: split acceptance criteria into numbered items from the team's AC field formats (list, Given/When/Then, table).
- Done when: table-driven tests for 10 real AC formats pass.

### S4-04 [ ] Gap agent (story mode)
- Touches: Agents/Agents/RequirementGap
- Do: missing/vague AC, conflicts, permissions, edge cases, dependencies, behaviour existing dealers rely on. Each gap is a question about what should happen, never how to build it.
- Done when: `Gap_questions_contain_no_implementation_advice` (checks a banned-phrases list) passes.

### S4-05 [ ] Coverage per criterion
- Depends: S3-07
- Touches: Agents/Agents/TestCases
- Do: per AC: covered / close match / not covered / will break (existing cases this story changes → regression list).
- Done when: one fixture per outcome passes.

### S4-06 [ ] Test Intent Brief and view
- Touches: Agents/Orchestrators/StoryIntake, web `pages/inbox/TestIntentBrief.tsx`
- Done when: `Test_intent_brief_validates`; Vitest `groups coverage by criterion`.

### S4-07 [ ] Writes for intake
- Do: gap questions @mention the product owner; cases linked/created in the story's suite.
- Done when: `Intake_writes_questions_and_cases` passes.

### S4-08 [ ] Story change re-check
- Do: story or AC edit supersedes the brief and re-checks only changed criteria.
- Done when: `Ac_edit_rechecks_only_changed_items` passes.

### S4-09 [ ] Build Check
- Touches: Agents/Agents/BuildCheck, Orchestrators
- Do: when PRs are linked and the story reaches the QA stage: per AC Implemented / Partial / NotFound with evidence, out-of-scope changes, uncovered paths, retest list, branch check.
- Done when: fixture tests for each per-AC outcome pass.

### S4-10 [ ] Bugs from story testing
- Touches: Core/Triggers
- Do: a child Bug created under a story enters the defect flow at the classifier with the story as context.
- Done when: `Child_bug_triage_includes_story_context` passes.

### S4-11 [ ] QA UPDATE roll-up for stories
- Read: marketplace `commentor` (QA UPDATE format)
- Do: open / closed / converted-to-defect / moved to SWAT counts with links.
- Done when: `Story_qa_update_counts_children` passes.

### S4-12 [ ] End-to-end
- Done when: `S4_ready_to_build_check` passes.
