# UI and UX standard

The bar: an enterprise tool a QA lead would choose over their current workflow. Calm and dense where people work, vivid where agents work. Every web task follows this file; a task is not done if its screen breaks a rule here.

## 1. Principles

1. **Glanceable first.** Each screen answers its main question in under three seconds (what needs me, what is running, what failed).
2. **Show the work.** Agents never act as a black box. Every claim links to evidence; every run can be watched live and replayed later.
3. **Human in control.** Nothing writes to ADO without a visible dry run and an explicit approval. Destructive or outward actions use a confirm step that names what will happen.
4. **Keyboard-first for power users, obvious for new ones.** Every primary action has a shortcut and a visible button.
5. **Motion explains, never decorates.** Animation shows state change, flow of data or causality. Nothing loops without meaning.

## 2. Visual system

- **Tokens only.** Colour, spacing, radius, elevation, typography and motion come from `src/web/src/theme/tokens.ts`. No hex values or pixel spacing in components.
- **Spacing:** 4 px grid; page gutter 24 px; card padding 16 px; dense tables 36 px rows, comfortable 48 px.
- **Type scale:** Inter (or system UI), with tabular numbers for metrics, costs and IDs. Monospace (JetBrains Mono / Cascadia) for code, diffs, prompts and JSON.
- **Colour semantics** (same meaning everywhere, both themes, WCAG AA contrast):
  - primary: interactive
  - info: running or in progress
  - success: done, approved, passed
  - warning: needs review, low confidence, budget near limit
  - error: failed, rejected, blocked
  - a distinct accent per agent, used on its node, chips and timeline lane
- **Surfaces:** app background, raised cards (elevation 1), overlays (elevation 8). Dark theme is a first-class design, not an inversion.
- **Icons:** filled MUI icons only, 20 px in dense UI, 24 px in navigation.

## 3. Layout and navigation

- App bar: product name, global search (`/` or `Ctrl+K`), team switcher, live-runs indicator (count with a pulse when anything runs), theme toggle, user menu.
- Left nav: collapsible to icons; the active item is highlighted; badges show counts (inbox items awaiting review).
- **Command palette** (`Ctrl+K`): jump to any ticket, run, release or page, and run actions ("Analyse 1234", "Open inbox").
- List → detail uses a split view on wide screens (list stays visible), a full page on narrow ones. Breadcrumbs on every detail page.
- Responsive down to 1280 px fully; usable read-only down to tablet width.

## 4. Interaction rules

- **States everywhere:** skeleton loaders shaped like the content (no bare spinners on page loads), error states with a retry and a support detail ID, empty states that say what to do next.
- **Optimistic UI** for approve, reject and edit, with rollback and a toast if the server refuses.
- **Toasts** for completion of background work, with an action link ("View finding").
- **Tables:** sortable, filterable, column chooser, sticky header, saved views per user, row selection with bulk actions, keyboard row navigation (`j`/`k`, `Enter` to open).
- **Forms:** inline validation on blur, a summary at the top on submit, unsaved-changes guard.
- **Undo** for reversible actions within 5 s instead of confirm dialogs.
- **Shortcuts:** `?` shows the shortcut sheet. Inbox: `a` approve, `e` edit, `r` reject, `n`/`p` next/previous.

## 5. Agent Run View (the showcase)

The screen where a run is watched live and replayed afterwards. It must be compelling to watch and precise to debug.

### 5.1 Layout

```
┌──────────────────────────────────────────────────────────────────────────┐
│ Run header: ticket link · status pill · elapsed timer · tokens · $ cost │
├───────────────────────────────────────────────┬──────────────────────────┤
│                                               │ Inspector (selected node)│
│            Live flow graph                    │  Overview | Input |      │
│   (pan, zoom, minimap, fit-to-view)           │  Tool calls | Output |   │
│                                               │  Evidence | Cost         │
├───────────────────────────────────────────────┴──────────────────────────┤
│ Timeline: one lane per agent, bars per step, scrubber + play/pause/speed │
└──────────────────────────────────────────────────────────────────────────┘
```

### 5.2 Flow graph

- Built with **React Flow (`@xyflow/react`)** and auto-layout (**elkjs**, left-to-right). The graph grows as events arrive: nodes animate in and the layout re-flows smoothly (spring, ~300 ms), and the camera follows the active node unless the user has panned.
- **Node types:** Trigger (webhook / Analyse now), Orchestrator, Agent (classifier, specialists, lead), Tool call (`ado.get_pr_diff`, `kb.search`…), Validation (schema check, repair attempt), Artifact (the draft), Gate (human approval), Write step.
- **Node card:** agent icon and accent, name, model chip, live status, elapsed time, token counter, and a one-line "what it is doing now" (from the latest event). Tool nodes show the call target (e.g. `PR 501 · 3 files`).
- **Node states and their motion:**

  | State | Visual |
  |---|---|
  | queued | muted outline, dashed border |
  | running | accent border with a slow glow pulse; animated progress shimmer; elapsed timer ticking |
  | streaming | as running, plus a live token count and a typing caret on the "now" line |
  | waiting on tool | child tool node appears, connecting edge animates |
  | validating | checklist icon, schema fields tick in as they pass |
  | repairing | warning colour, a "repair 1/1" badge |
  | done | success check that draws in, glow fades; final duration and cost frozen |
  | failed | error colour, a gentle shake once, reason on the card, "Open log" action |
  | skipped | greyed out, struck-through label |

- **Edges:** animated flowing dashes in the source agent's accent while data moves; solid once complete; red on the path of a failure. Hovering an edge shows what passed along it (e.g. "3 files, 412 lines of diff").
- **Parallel specialists** fan out from the orchestrator and visibly run side by side, then converge on the lead.
- **Evidence trails:** selecting a claim in the artifact highlights the tool nodes and edges it came from and dims the rest.
- Minimap, zoom controls, "fit", "follow live" toggle, and full-screen mode.

### 5.3 Inspector

- **Overview:** status, timings, model, effort, tokens in/out, cost (`decimal`, shown with currency).
- **Input:** the prompt with untrusted sections visibly fenced; PII shown redacted by default, revealable only for permitted roles.
- **Tool calls:** each call with arguments, duration, result preview, and a link to the ADO item.
- **Output:** the JSON pretty-printed, with schema validation results inline; for artifacts, the rendered view next to the raw view.
- **Evidence** and **Cost** tabs.

### 5.4 Timeline and replay

- One lane per agent; bars grow in real time; tool calls are ticks on the lane.
- A scrubber replays any finished run at 1×, 2× or 4× from stored events; the graph and inspector follow the playhead. A run replays the same way it ran live (same event stream, same components).
- A deep link includes the run and the playhead position, for sharing in a ticket comment.

### 5.5 Live feel without noise

- An activity ticker in the run header streams the latest event in plain words ("Code agent read PR 501 diff · 3 files").
- Totals (tokens, cost, elapsed) count up smoothly.
- When the run completes: a short completion state on the header, a toast with "Review finding", and the Artifact node pulses once to draw the eye.
- `prefers-reduced-motion`: no glow, shimmer or shake; state still shown by colour, icon and text. Animations never block interaction.
- Performance: 60 fps with 50 nodes; events are batched per animation frame; React Flow nodes are memoised.

### 5.6 Event contract (SSE)

The view is driven only by events, so live and replay are identical. Minimum event types: `run.started`, `node.added`, `node.started`, `node.progress` (message, tokens so far), `tool.called`, `tool.completed`, `node.validating`, `node.repairing`, `node.completed` (duration, tokens, cost), `node.failed` (safe reason), `edge.active`, `artifact.created`, `run.completed`, `run.failed`. Each event has `runId`, `nodeId`, `seq`, `ts`. The client reorders by `seq` and resumes with `Last-Event-ID`.

## 6. Other key screens

- **Review inbox:** split view; claims grouped with label chips and confidence bars; evidence opens in a side sheet; the dry-run write plan is shown as a step list before Approve; "Watch run" opens the Agent Run View at the end of the run.
- **Ticket page:** header with ADO state and links, a vertical activity timeline (events, runs, artifacts, writes), and a "runs" strip of mini run graphs that open the full view.
- **Dashboards:** stat tiles with trend sparklines, every chart with "show query", consistent chart colours from tokens.

## 7. Accessibility

WCAG 2.2 AA: full keyboard use including the graph (arrow keys move between nodes, `Enter` opens the inspector), visible focus rings, ARIA live region announcing run status changes (polite), colour never the only signal, text contrast ≥ 4.5:1.

## 8. How it is checked

- Vitest + Testing Library for behaviour (states, shortcuts, a11y roles); `vitest-axe` on every page for accessibility.
- Storybook stories for each node state and each key component, used for visual review.
- Playwright smoke run against the dev server for the Agent Run View with a recorded event stream (no real services).
