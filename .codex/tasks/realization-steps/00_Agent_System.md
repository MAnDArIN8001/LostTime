# Stage Agent System

## Goal

Shared meta-rules for every stage agent prompt in `realization-steps`.

Use this file as the mandatory base contract. Each stage file adds only stage mission, stage scope, stage outputs, and stage-specific checks.

If a stage file conflicts with `Assets/.cursor/context/context.md`, the context file wins.

## Mandatory Inputs

Every stage agent must read these docs before planning or implementing:

- `Assets/.cursor/context/context.md`
- `Assets/.cursor/tasks/game-plan/01_MVP_Concept.md`
- `Assets/.cursor/tasks/game-plan/CoreLoop.md`
- `Assets/.cursor/tasks/game-plan/02_Implementation_Map.md`
- `Assets/.cursor/tasks/game-plan/03_Realization_Steps.md`
- `Assets/.cursor/docs/WORKLOG_TEMPLATE.md`

## Architecture Rules

- Stay inside the current stage mission.
- Do not expand scope early.
- Reuse existing project seams before adding systems.
- Keep gameplay orchestration event-driven where decoupling matters.
- Keep tunables and content data-driven through `ScriptableObject` assets.
- Keep execution gates inside the current FSM / HFSM stack.
- Do not create a parallel architecture if an existing seam already solves the job.
- The current design target is `action-adventure with environmental magical puzzles`, not combat-first slice expansion.
- Every architecture choice must include:
  - why it was chosen
  - what coupling it reduces
  - what editor or data workflow it enables

## Project Fit Anchors

Stage agents should treat these seams as defaults:

- `Character` modules are the player execution base.
- `MovementModule` plus movement states are the locomotion seam.
- `FSM` and `HFSM` are the action-gating seam.
- `MainInput` is the player-input seam.
- `EventBus` is the gameplay-signal seam when decoupling is useful.
- `ItemSetup` and `ItemsDatabase` are the content-data seams.
- `DirectionalRaycaster`, filters, and markable contracts are the interaction seam.

Stage agents should assume the core mechanic language is:

- `push`
- `pull`
- `press`

## Documentation Flow

Every stage prompt must require documentation before or alongside implementation.

Required sections:

- `Intent`: what the stage solves for the MVP
- `Architecture`: chosen approach and why it fits current seams
- `Data`: `ScriptableObject` assets, prefabs, configs, serialized refs
- `Signals`: emitted and listened events
- `Scene/Inspector`: exact editor wiring steps
- `Validation`: playable checks and test route
- `Decision Log`: why this path, why not a simpler or alternative path

Every implementation task must also produce or update a worklog in:

- `Assets/.cursor/docs/worklogs/`

Default naming:

- `YYYY-MM-DD_short-task-slug.md`

Required worklog sections:

- `Summary`
- `Files Changed`
- `Scene/Inspector`
- `Validation`
- `Decision Log`
- `Follow-Ups`
- `Commit`

## Delivery Rules

- Keep prompts stage-based, not system-based.
- Keep wording concrete, action-oriented, and short.
- State exact non-scope to prevent feature sprawl.
- Require a playable scene state, not just code-complete status.
- Require editor steps even when the answer is `none`.
- Require a worklog path in the final handoff.
- Require one repository commit per scoped agent task unless the user explicitly forbids commits.
- Allowed commit prefixes:
  - `fix:`
  - `feat:`
  - `update:`
  - `refactor:`
- End each implementation handoff with:
  - a step-by-step requirements list
  - explicit inspector or editor setup steps
  - unresolved questions, if any

## Acceptance Gate

A stage prompt is only valid if it:

- names the mission clearly
- defines exact scope and non-scope
- lists required input docs
- lists target outputs and deliverables
- embeds architecture-fit rules tied to current seams
- embeds the full documentation flow
- includes acceptance checks for playable validation
- includes editor configuration steps or `none`
- includes a worklog deliverable
- includes commit policy

## Shared Template

```md
# {Stage File Title}

## Mission

{One short paragraph. State what this stage agent must achieve.}

## Scope

In:
- {feature / doc / setup inside this stage}
- {feature / doc / setup inside this stage}

Out:
- {explicit non-scope}
- {explicit non-scope}

## Required Inputs

- `Assets/.cursor/context/context.md`
- `Assets/.cursor/tasks/game-plan/01_MVP_Concept.md`
- `Assets/.cursor/tasks/game-plan/CoreLoop.md`
- `Assets/.cursor/tasks/game-plan/02_Implementation_Map.md`
- `Assets/.cursor/tasks/game-plan/03_Realization_Steps.md`
- {extra stage-specific docs or code paths}

## Target Outputs

- {playable result}
- {docs to produce or update}
- {worklog path to create or update}
- {assets / prefabs / scene wiring / data created}

## Architecture Fit Rules

- Reuse these seams:
  - {existing seam}
  - {existing seam}
- Keep this work event-driven where needed:
  - emit: {signal}
  - listen: {signal}
- Keep data-driven through:
  - {ScriptableObject / config / prefab}
- Keep FSM-driven through:
  - {state / gate / transition area}
- Rationale:
  - chosen because {reason}
  - reduces coupling by {reason}
  - enables editor/data workflow by {reason}

## Documentation Duties

### Intent

{What player or MVP problem this stage solves.}

### Architecture

{Chosen approach. Why this seam. Why not a parallel system.}

### Data

- {asset / prefab / config}
- {asset / prefab / config}

### Signals

- emit: {signal}
- listen: {signal}
- `none` if not needed

### Scene/Inspector

1. {exact editor wiring step}
2. {exact editor wiring step}
3. `none` if no setup is needed

### Validation

1. {playable validation step}
2. {playable validation step}
3. {failure or regression check}

### Decision Log

- chose: {approach}
- avoided: {alternative}
- why: {short rationale}

## Acceptance Checks

- {clear done condition}
- {clear done condition}
- {playable scene state reached}
- {docs updated}
- {worklog updated}
- {commit created with approved prefix}

## Requirements

1. {step-by-step delivery requirement}
2. {step-by-step delivery requirement}
3. {step-by-step delivery requirement}

## Editor Configuration

1. {editor / inspector step}
2. {editor / inspector step}
3. `none` if no setup is needed

## Unresolved Questions

- {question}
- `none` if fully resolved
```

## Editor Configuration

1. none

## Unresolved Questions

- none
