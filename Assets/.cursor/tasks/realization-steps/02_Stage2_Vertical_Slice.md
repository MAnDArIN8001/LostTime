# Stage 2 Agent Prompt: Vertical Slice

## Mission

Build the first fully playable short courtyard path. Player must enter the scene, understand one of the core interaction verbs, solve one chained environmental mechanism, and receive readable objective and interaction feedback.

## Scope

In:
- one direct interaction path for `press`
- one environmental interaction path for `push` or `pull`
- one small chained mechanism using current project seams
- one mentor or starting focal interaction
- minimal HUD: objective and interact hint
- scene wiring and validation docs for the slice

Out:
- expanded multi-sector courtyard loop
- boss or combat-heavy escalation
- inventory expansion, crafting, branching dialog
- large enemy roster or guardian fight

## Required Inputs

- `Assets/.cursor/tasks/realization-steps/00_Agent_System.md`
- `Assets/.cursor/context/context.md`
- `Assets/.cursor/tasks/game-plan/01_MVP_Concept.md`
- `Assets/.cursor/tasks/game-plan/CoreLoop.md`
- `Assets/.cursor/tasks/game-plan/02_Implementation_Map.md`
- `Assets/.cursor/tasks/game-plan/03_Realization_Steps.md`
- `Assets/InputSystem_Actions.inputactions`
- `Assets/Scripts/FSM`
- `Assets/Scripts/Character`
- `Assets/Scripts/Loot`
- `Assets/Scripts/Quest`
- `Assets/Scripts/Utils`

## Target Outputs

- one playable slice in `Assets/Scenes/SampleScene.unity`
- one readable courtyard mechanism chain wired through current seams
- objective and interaction feedback wired for the slice
- docs for intent, architecture, data, signals, scene wiring, validation
- a worklog under `Assets/.cursor/docs/worklogs/`

## Architecture Fit Rules

- Reuse these seams:
  - `MainInput` for interaction actions
  - `FSM` / `HFSM` plus `StateType` when action gating or temporary control locks are needed
  - `DirectionalRaycaster` / filters / `IMarkable` for target selection
  - current quest and UI seams for objective feedback
  - `ScriptableObject` assets or serialized refs for tunable timings if needed
- Keep this work event-driven where needed:
  - emit: mechanism activated, world state changed, objective advanced
  - listen: interaction accepted, mechanism completed, quest step changed
- Keep data-driven through:
  - timings, interaction config, quest text, and mechanism refs in assets or serialized refs
- Keep FSM-driven through:
  - interaction lock, movement lock, or temporary action gating only when needed
- Rationale:
  - chosen because the current project already solves movement, input, interaction targeting, and state gating
  - reduces coupling by keeping environmental logic out of the root character loop
  - enables editor/data workflow by keeping the slice mostly inspector-wired

## Documentation Duties

### Intent

State how this slice proves the current `push / pull / press` action-adventure direction in the smallest playable route.

### Architecture

Explain why the slice was built through existing movement, interaction, quest, and UI seams instead of a new standalone puzzle framework.

### Data

- mechanism config or serialized refs
- mentor or starting focal scene object
- gate, plate, slider, or movable object refs
- HUD references and text sources

### Signals

- emit: mechanism activated, state changed, objective advanced
- listen: interact input, mechanism completion, quest step changed

### Scene/Inspector

1. bind interaction input in `Assets/InputSystem_Actions.inputactions` if needed
2. assign interaction target and mark visuals on slice objects
3. wire serialized links between mechanism objects
4. wire HUD references in the scene canvas

### Validation

1. start scene and receive or discover the slice objective
2. perform one direct interaction successfully
3. complete one chained mechanism using the intended verb flow
4. verify HUD updates and interaction hints stay readable

### Decision Log

- chose: one complete short mechanism route before wider courtyard expansion
- avoided: adding combat escalation or multiple puzzle families early
- why: proves architecture and player fantasy with minimum surface area

## Acceptance Checks

- player can understand and use one core interaction verb
- the slice contains one readable chained mechanism
- interactions use the shared raycast / mark path or a justified seam-compatible variant
- slice is playable in-scene, not code-complete only
- docs include architecture, data, signals, scene wiring, and validation
- worklog is created or updated
- commit uses approved prefix

## Requirements

1. add interaction input through `MainInput`, not direct Unity polling.
2. keep slice logic inside existing interaction, world-state, and quest seams.
3. keep timings and scene data externalized in assets or serialized refs where useful.
4. end with one playable start -> interact -> mechanism -> progression route in scene.

## Editor Configuration

1. bind new actions in `Assets/InputSystem_Actions.inputactions` if needed.
2. assign mark and interact targets on slice objects.
3. wire mechanism references in scene.
4. wire HUD references in the scene canvas.

## Unresolved Questions

- should the first slice teach `press` only, or `press + push` in one route
- should remote magic interaction be deferred until the second slice
