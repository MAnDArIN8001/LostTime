# Context Reset And Agent Rules

## Summary

Updated the project source-of-truth documents to match the current pivot toward an open-courtyard action-adventure built around environmental magical interactions. Also tightened agent process rules so future agent tasks must read the technical context, create worklogs, and use approved commit prefixes.

## Context

- task goal: reset project context before implementation work continues
- current design direction: `push / pull / press` in an open inner castle courtyard
- key requirement: stop future agents from following outdated combat-first planning docs

## Files Changed

- `Assets/.cursor/context/context.md`
- `Assets/.cursor/rules/AGENTS.mdc`
- `Assets/.cursor/tasks/realization-steps/00_Agent_System.md`
- `Assets/.cursor/tasks/realization-steps/02_Stage2_Vertical_Slice.md`
- `Assets/.cursor/tasks/realization-steps/02_Stage2_Vertical_Slice_Handoff.md`
- `Assets/.cursor/tasks/realization-steps/03_Stage3_Full_Main_Loop.md`
- `Assets/.cursor/tasks/realization-steps/04_Stage4_Boss_And_Ending.md`
- `Assets/.cursor/tasks/realization-steps/05_Stage5_Polish.md`
- `Assets/.cursor/tasks/game-plan/01_MVP_Concept.md`
- `Assets/.cursor/tasks/game-plan/CoreLoop.md`
- `Assets/.cursor/tasks/game-plan/02_Implementation_Map.md`
- `Assets/.cursor/tasks/game-plan/03_Realization_Steps.md`
- `Assets/.cursor/tasks/realization-steps/README.md`
- `Assets/.cursor/docs/WORKLOG_TEMPLATE.md`
- `Assets/.cursor/docs/quest-setup-guide.md`

## Scene/Inspector

1. none

## Validation

1. reviewed the existing `.cursor` context, rules, and planning files before editing
2. updated the main source-of-truth and planning docs to align on the same direction
3. Unity scene/runtime validation was not performed because this task was documentation and process only

## Decision Log

- chose: make `Assets/.cursor/context/context.md` the explicit technical source of truth
- avoided: creating a second competing "master doc"
- why: future agents need one stable reference, not multiple overlapping project summaries

- chose: require worklogs for every file-changing agent task
- avoided: leaving documentation as optional handoff prose only
- why: this creates a traceable implementation history for the diploma project

- chose: enforce commit prefixes in rules
- avoided: informal commit naming
- why: predictable commit naming will make agent-created history easier to read

## Follow-Ups

- update stage-specific prompt files if they will be reused directly for implementation
- decide whether current quest and combat docs should be fully migrated or kept as clearly marked legacy references
- create the first implementation-stage prompt for the `push / pull / press` courtyard slice

## Commit

- prefix used: `update`
- subject: `update: refresh project truth and agent workflow rules`
- status: planned
