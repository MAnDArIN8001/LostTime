# Stage 5 Agent Prompt: Polish

> Legacy note:
> This prompt may reference polish targets from the older combat-first slice.
> Read `Assets/.cursor/context/context.md` first.
> If any section conflicts with the current courtyard `push / pull / press` direction, the context file wins.

## Mission

Make the MVP readable, stable, and presentable for diploma review. Improve feedback, clarity, balance, onboarding, and demo reliability without reopening scope.

## Scope

In:
- cast, hit, pickup, and UI sounds
- simple spell, seal, and boss feedback VFX
- tuning pass for hp, damage, mana, cooldowns, and pickup economy
- clearer quest text and first-minute onboarding
- demo stability and readability fixes

Out:
- new mechanics, enemies, spells, regions, or quests
- major refactors unless required for demo stability
- content expansion disguised as polish

## Required Inputs

- `Assets/.cursor/tasks/realization-steps/00_Agent_System.md`
- `Assets/.cursor/context/context.md`
- `Assets/.cursor/tasks/game-plan/01_MVP_Concept.md`
- `Assets/.cursor/tasks/game-plan/CoreLoop.md`
- `Assets/.cursor/tasks/game-plan/02_Implementation_Map.md`
- `Assets/.cursor/tasks/game-plan/03_Realization_Steps.md`
- `Assets/.cursor/tasks/realization-steps/04_Stage4_Boss_And_Ending.md`
- current playable scene, prefabs, UI, audio, VFX, and tuning assets

## Target Outputs

- stable demo build path with readable combat and quest feedback
- tuned values for player, enemy, boss, pickups, and UI pacing
- added audio / VFX hookups needed for readability
- docs updated for architecture, data, signals, scene wiring, validation, and decision log

## Architecture Fit Rules

- Reuse these seams:
  - existing combat, interaction, quest, and UI paths only
  - `ScriptableObject` assets and serialized refs for tuning values
  - existing signal routes for readable feedback hooks
- Keep this work event-driven where needed:
  - emit: feedback played, objective updated, boss warning fired
  - listen: cast confirmed, damage dealt, pickup collected, quest step changed, boss attack started
- Keep data-driven through:
  - audio clips, VFX prefabs, text content, stats, cooldowns, and economy values in assets or serialized refs
- Keep FSM-driven through:
  - feedback that depends on action state, hit state, or temporary lock state
- Rationale:
  - chosen because polish should strengthen the current game, not mutate its structure
  - reduces coupling by attaching feedback to existing signals and state transitions
  - enables editor/data workflow by making tuning and presentation mostly asset and inspector work

## Documentation Duties

### Intent

State how this stage improves clarity and presentation while keeping the same MVP scope.

### Architecture

Explain how audio, VFX, text clarity, and balance were attached to existing systems instead of introducing new frameworks.

### Data

- audio clip refs and routing points
- VFX prefab refs for spell, seal, boss, and pickups
- tuned combat and economy assets
- updated quest text and onboarding copy

### Signals

- emit: `none` if existing feedback hooks are enough
- listen: cast, hit, pickup, quest update, boss telegraph, win state
- note any new signal only if existing routes cannot cover readability needs

### Scene/Inspector

1. assign audio clips on combat, UI, and pickup objects
2. assign VFX prefabs on spell, seal, and boss attack hooks
3. tune values in `ScriptableObject` assets and serialized fields
4. verify onboarding prompts and quest text refs in scene UI

### Validation

1. play from intro through ending and check feedback readability in every main beat
2. verify mana, hp, enemy pressure, and pickup economy feel fair for one session MVP
3. verify first-minute onboarding explains cast, interact, and main objective clearly
4. verify no new polish hook breaks the playable path

### Decision Log

- chose: readability and stability pass after full loop completion
- avoided: adding fresh mechanics under the label of polish
- why: keeps diploma demo coherent and finishable

## Acceptance Checks

- combat, pickup, quest, and boss feedback are readable
- tuning pass improves pacing without expanding feature scope
- onboarding and quest text reduce confusion in the first minute
- demo remains stable from intro to ending
- docs record tuned data, inspector wiring, validation path, and unresolved issues

## Requirements

1. add only readability, balance, presentation, and stability work.
2. prefer asset and inspector tuning over structural rewrites.
3. attach sound and VFX through existing hooks or justified minimal extensions.
4. end with a stable, presentable full-run demo path.

## Editor Configuration

1. assign audio clips on combat, UI, and pickup objects.
2. assign VFX prefabs on spell, seal, and boss attacks.
3. tune values in `ScriptableObject` assets and serialized fields.
4. verify onboarding prompts and quest text references.

## Unresolved Questions

- which feedback gaps most affect diploma readability if time is short
- should polish time prioritize balance first or presentation first
