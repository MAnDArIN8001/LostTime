# Stage 1 Agent Prompt: Lock MVP Frame

## Mission

Lock the MVP frame before feature work. Align concept, core loop, implementation map, and realization order into one approved documentation baseline so later stages build the same small complete wizard-trial arc without scope drift.

## Scope

In:
- lock one hero, one region, one mentor, one main quest, one short complete wizard-trial arc
- lock one primary magic shot, `3 seals -> boss -> return`, pickups `mana / heal / coins`, and `3 enemy types + 1 boss`
- lock the architecture baseline around `Character` modules, `MovementModule` + movement states, `FSM` / `HFSM`, `MainInput`, `EventBus`, `DirectionalRaycaster`, raycast filters, markable contracts, and `ItemSetup` / `ItemsDatabase`
- align Stage 1 wording, outputs, requirements, and validation with the approved MVP docs only
- keep only deferred architecture questions that can affect later stage implementation

Out:
- feature implementation
- playable scene gates, combat wiring, quest wiring, or prefab setup
- extra biomes, spells, quests, systems, enemies, pickups, or progression layers beyond the locked MVP
- polish, balancing, presentation pass

## Required Inputs

- `Assets/.cursor/tasks/realization-steps/00_Agent_System.md`
- `Assets/.cursor/context/context.md`
- `Assets/.cursor/tasks/game-plan/01_MVP_Concept.md`
- `Assets/.cursor/tasks/game-plan/CoreLoop.md`
- `Assets/.cursor/tasks/game-plan/02_Implementation_Map.md`
- `Assets/.cursor/tasks/game-plan/03_Realization_Steps.md`

## Target Outputs

- approved Stage 1 scope-freeze prompt for later stage agents
- approved alignment across concept, core loop, implementation map, and realization steps docs
- explicit architecture baseline tied to current project seams and anti-parallel-system rules
- explicit doc-only validation gate, acceptance checks, requirements list, and editor setup `none`
- deferred architecture questions only where later stages still need a decision

## Architecture Fit Rules

- Reuse these seams:
  - `Character` modules for player execution baseline
  - `MovementModule` + movement states for locomotion baseline
  - `FSM` / `HFSM` for all future action gating
  - `MainInput` for all future player actions
  - `EventBus` for gameplay signals where decoupling matters
  - `ItemSetup` / `ItemsDatabase` for future tunables and content
  - `DirectionalRaycaster` / filters / markables for future interaction
- Keep this work event-driven where needed:
  - define signal boundaries only; do not implement them yet
  - note likely signal routes for combat, seal clear, boss unlock, quest progress
- Keep data-driven through:
  - `ScriptableObject` assets as the default for tunable gameplay data
- Keep FSM-driven through:
  - movement, combat, looting, and communication must stay in the state stack
- Rationale:
  - chosen because current seams already cover execution, input, interaction, and tunable data
  - reduces coupling by preventing one-off stage-specific systems
  - enables editor/data workflow by keeping content authoring in assets and inspector wiring

## Documentation Duties

### Intent

State the exact MVP promise: one short wizard trial with clear start, escalation, climax, and return.

### Architecture

Explain how future work must enter through `Character` modules, keep locomotion in `MovementModule` plus movement states, keep action gating in `FSM` / `HFSM`, keep player actions behind `MainInput`, route interaction targeting through `DirectionalRaycaster` plus raycast filters plus markable contracts, use `EventBus` where decoupling matters, and keep tunables in `ScriptableObject` assets. Reject parallel systems and stage-specific side paths.

### Data

- approved docs lock later data families around spell, enemy, seal, pickup, quest, and UI text
- later tunables stay asset-driven through `ScriptableObject` content, not hardcoded branches
- Stage 1 creates no new assets and changes no inspector wiring

### Signals

- emit: `none`
- listen: `none`
- document planned signal ownership only

### Scene/Inspector

1. none

### Validation

1. verify concept, core loop, implementation map, and realization steps all describe one hero, one region, one mentor, one main quest, and one short complete wizard-trial arc
2. verify the locked loop is `3 seals -> boss -> return`, combat stays one primary magic shot, pickups stay `mana / heal / coins`, and content stays `3 enemy types + 1 boss`
3. verify the seam baseline explicitly points later stages to `Character` modules, `MovementModule` + movement states, `FSM` / `HFSM`, `MainInput`, `EventBus`, `DirectionalRaycaster`, raycast filters, markable contracts, and `ItemSetup` / `ItemsDatabase`
4. verify Stage 1 stays documentation-only, editor setup stays `none`, and playable scene validation starts in later stages
5. verify unresolved questions, if any, are written as deferred architecture decisions and do not replace the locked baseline for later stages

### Decision Log

- chose: freeze scope before implementation
- avoided: building slice features while concept boundaries stay loose
- why: protects later stages from rework and architecture drift

## Acceptance Checks

- concept, core loop, implementation map, and realization steps are aligned to one locked MVP baseline
- scope and non-scope explicitly prevent early feature growth
- architecture baseline references current seams, including `Character` modules, `MovementModule` + movement states, and interaction filters, and rejects parallel systems
- Stage 1 is defined as documentation-only and does not require a playable scene state
- editor setup is declared as `none`
- unresolved choices, if any, are deferred architecture calls only

## Requirements

1. Align mission, scope, outputs, validation, and requirements to the approved MVP docs only.
2. Lock one hero, one region, one mentor, one main quest, one primary magic shot, `3 seals -> boss -> return`, pickups `mana / heal / coins`, and `3 enemy types + 1 boss`.
3. Lock existing seams as the mandatory integration path for later stages: `Character` modules, `MovementModule` + movement states, `FSM` / `HFSM`, `MainInput`, `EventBus`, `DirectionalRaycaster`, raycast filters, markables, and `ItemSetup` / `ItemsDatabase`.
4. State that Stage 1 is documentation-only, editor setup is `none`, and playable scene gates begin in later stages.
5. Keep unresolved questions only if they can change later architecture decisions, and phrase them as deferred choices that preserve the locked baseline until a later stage resolves them.

## Editor Configuration

1. none

## Unresolved Questions

- defer whether combat growth later justifies an `Attacking` parent state; until then, later stages stay on the current `FSM` / `HFSM` baseline with `Attack` as the immediate action gate
- defer whether seals and NPCs later share one interaction contract or stay handler-specific; until then, later stages stay on `DirectionalRaycaster` + raycast filters + `IMarkable` as the interaction baseline
- defer whether coins stay score only or become a light gate later; until then, later stages keep the locked pickup baseline of `mana / heal / coins` without adding new progression systems
