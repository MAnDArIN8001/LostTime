# Stage 2 Agent Prompt: Vertical Slice

## Mission

Build the first fully playable short path. Player must talk to the mentor, cast one spell, defeat one enemy, restore one seal, and read basic HUD feedback in one scene flow.

## Scope

In:
- combat input for cast and optional aim
- first combat state on top of current FSM seams
- one projectile spell with mana cost and cooldown
- one enemy archetype: melee beast
- one mentor interaction
- one seal interaction
- minimal HUD: hp, mana, quest text, interact hint

Out:
- second and third enemy archetypes
- all 3 seals
- boss, ending, polish pass
- inventory expansion, crafting, branching dialog

## Required Inputs

- `Assets/.cursor/tasks/realization-steps/00_Agent_System.md`
- `Assets/.cursor/context/context.md`
- `Assets/.cursor/tasks/game-plan/01_MVP_Concept.md`
- `Assets/.cursor/tasks/game-plan/CoreLoop.md`
- `Assets/.cursor/tasks/game-plan/02_Implementation_Map.md`
- `Assets/.cursor/tasks/game-plan/03_Realization_Steps.md`
- `Assets/Scripts/Input/MainInput.inputactions`
- `Assets/Scripts/FSM`
- `Assets/Scripts/Character`
- `Assets/Scripts/Loot`
- `Assets/Scripts/Utils`

## Target Outputs

- one playable slice in `Assets/Scenes/SampleScene.unity`
- combat, mentor, seal, and HUD wired through current seams
- first pass docs for intent, architecture, data, signals, scene wiring, validation
- required assets, prefabs, and serialized references for the slice

## Architecture Fit Rules

- Reuse these seams:
  - `MainInput` for cast / aim / interaction actions
  - `FSM` / `HFSM` plus `StateType` for combat and communication gates
  - `IAnimationFacade` or current animation facade path for cast animation writes
  - `DirectionalRaycaster` / filters / `IMarkable` for mentor and seal targeting
  - `ItemSetup` or other `ScriptableObject` assets for tunable spell data
- Keep this work event-driven where needed:
  - emit: spell fired, enemy defeated, seal restored, mentor dialog completed
  - listen: mana spent, quest step changed, interaction accepted
- Keep data-driven through:
  - spell stats, enemy stats, quest text, and interaction config in assets or serialized refs
- Keep FSM-driven through:
  - cast entry / exit, movement lock, interaction lock, and communication gate
- Rationale:
  - chosen because the current project already solves movement, input, interaction targeting, and state gating
  - reduces coupling by keeping combat and interaction logic out of the root character loop
  - enables editor/data workflow by letting designers tune slice data without code branches

## Documentation Duties

### Intent

State how this slice proves the full MVP fantasy in the smallest playable route.

### Architecture

Explain why combat, mentor talk, and seal activation were added through existing FSM, input, interaction, and data seams.

### Data

- spell data asset or config
- melee beast data or prefab refs
- mentor and seal prefabs or scene objects
- HUD references and text sources

### Signals

- emit: spell cast, mana spent, enemy killed, seal restored, dialog closed
- listen: cast input, interact input, quest step changed, cooldown or mana block

### Scene/Inspector

1. bind cast and optional aim in `Assets/Scripts/Input/MainInput.inputactions`
2. assign projectile prefab and spell data on player combat entry point
3. add interaction target / mark visuals on mentor and seal objects
4. wire HUD references in the scene canvas

### Validation

1. start scene, talk to mentor, receive slice objective
2. cast spell, spend mana, defeat melee beast
3. interact with one seal and complete the slice objective
4. verify HUD updates and interaction hints stay readable

### Decision Log

- chose: one complete short path before wider content expansion
- avoided: adding more enemies, pickups, or boss logic early
- why: proves architecture and fantasy with minimum surface area

## Acceptance Checks

- player can talk, cast, kill one enemy, and restore one seal
- combat uses `MainInput` plus FSM gating, not raw update logic
- mentor and seal interactions use the shared raycast / mark path or a justified seam-compatible variant
- slice is playable in-scene, not code-complete only
- docs include architecture, data, signals, scene wiring, and validation

## Requirements

1. add combat input through `MainInput`, not direct Unity polling.
2. add cast logic through FSM state gates and current animation path.
3. keep spell, enemy, and HUD tuning externalized in assets or serialized refs.
4. end with one playable mentor -> fight -> seal route in scene.

## Editor Configuration

1. bind new actions in `Assets/Scripts/Input/MainInput.inputactions`.
2. assign projectile prefab and spell data on player combat component.
3. add mark and interact targets on mentor and seal prefabs.
4. wire HUD references in the scene canvas.

## Unresolved Questions

- should aim exist now or stay deferred until ranged enemy pressure appears
- should the first spell use free-fire only or soft target assist
