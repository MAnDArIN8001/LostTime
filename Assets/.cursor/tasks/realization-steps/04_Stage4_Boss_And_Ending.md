# Stage 4 Agent Prompt: Boss And Ending

## Mission

Add the climax and resolution. Deliver the trial guardian fight, boss-to-ending quest wiring, final mentor payoff, and clear win state so the MVP ends as a full wizard trial.

## Scope

In:
- one trial guardian boss
- minimum 2 boss attack patterns
- boss arena entry and combat flow
- boss death to ending quest progression
- final mentor dialog and win screen

Out:
- extra bosses, post-game, alternate endings
- broad polish and rebalance outside boss readability needs
- new side systems not required for boss flow

## Required Inputs

- `Assets/.cursor/tasks/realization-steps/00_Agent_System.md`
- `Assets/.cursor/context/context.md`
- `Assets/.cursor/tasks/game-plan/01_MVP_Concept.md`
- `Assets/.cursor/tasks/game-plan/CoreLoop.md`
- `Assets/.cursor/tasks/game-plan/02_Implementation_Map.md`
- `Assets/.cursor/tasks/game-plan/03_Realization_Steps.md`
- `Assets/.cursor/tasks/realization-steps/03_Stage3_Full_Main_Loop.md`
- `Assets/Scripts/Character`
- `Assets/Scripts/FSM`
- `Assets/Scripts/Loot`
- `Assets/Scripts/Utils`

## Target Outputs

- playable guardian boss encounter with at least 2 readable patterns
- boss death connected to ending progression and final mentor payoff
- win screen or equivalent clear completion feedback
- docs updated for architecture, data, signals, scene wiring, validation, and decision log

## Architecture Fit Rules

- Reuse these seams:
  - combat actions through existing FSM and input paths
  - quest progression above movement/combat systems
  - interaction flow for final mentor talk through the same mark / ray / communication path
  - boss tuning in assets or serialized data, not hardcoded branches
- Keep this work event-driven where needed:
  - emit: boss phase changed, boss defeated, ending unlocked, trial completed
  - listen: arena entered, all seals restored, boss hp threshold reached, mentor interaction completed
- Keep data-driven through:
  - boss stats, attack timings, arena refs, ending text, UI refs, and reward copy in assets or serialized refs
- Keep FSM-driven through:
  - cast windows, boss attack windows, player action locks, and communication gates
- Rationale:
  - chosen because climax should attach to proven combat and quest seams, not replace them
  - reduces coupling by decoupling boss encounter results from direct scene object chains
  - enables editor/data workflow by allowing boss tuning and ending presentation to be adjusted in data and inspector

## Documentation Duties

### Intent

State how the boss and ending complete the MVP fantasy and give the player a graduation payoff.

### Architecture

Explain how boss combat, boss death routing, ending unlock, and final mentor talk fit current combat, quest, and interaction seams.

### Data

- guardian prefab and tuning assets
- boss attack configs or serialized timings
- arena trigger and gate refs
- ending UI refs and final mentor content refs

### Signals

- emit: boss started, boss phase changed, boss defeated, ending shown, trial completed
- listen: arena entered, attack trigger, hp threshold crossed, boss death confirmed

### Scene/Inspector

1. place boss arena trigger and boss prefab
2. wire boss death event or callback to quest progression
3. assign ending UI panel and final mentor dialog refs
4. verify arena lock / unlock refs and boss intro setup

### Validation

1. reach the arena through normal quest flow
2. fight the guardian and observe at least 2 readable attack patterns
3. defeat the boss and verify ending progression fires once
4. talk to mentor and reach win screen or clear completion state

### Decision Log

- chose: add climax only after the main loop is stable
- avoided: building ending screens before boss outcome exists
- why: keeps ending logic tied to real gameplay completion

## Acceptance Checks

- guardian fight is playable and beatable
- boss has at least 2 distinct readable attack patterns
- boss defeat unlocks the ending only once and through stable progression flow
- final mentor payoff and win state are reachable in one run
- docs cover boss data, signals, scene wiring, and playable validation

## Requirements

1. create one guardian boss with minimum 2 attack patterns.
2. route boss flow through current combat and quest seams.
3. wire boss death into ending progression and mentor payoff.
4. end with one complete playable victory path.

## Editor Configuration

1. place boss arena trigger and boss prefab.
2. wire boss death event to quest progression.
3. assign ending UI panel and mentor final dialog.
4. verify arena gate, intro trigger, and win-state refs.

## Unresolved Questions

- should the boss use summons or stay pure pattern pressure only
- should the ending go straight to win UI or require explicit mentor return
