# Stage 4 Agent Prompt: Boss And Ending

> Legacy note:
> This prompt may contain boss-first assumptions from the older combat slice.
> Read `Assets/.cursor/context/context.md` first.
> If any section conflicts with the current courtyard `push / pull / press` direction, the context file wins.

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
  - emit: boss encounter started, pattern switched, boss defeated, ending unlocked, trial completed
  - listen: arena entered, all seals restored, boss death confirmed, mentor interaction completed
- Keep data-driven through:
  - boss stats, pattern durations, telegraph/cooldowns, arena refs, ending text, UI refs, and reward copy in assets or serialized refs
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

- emit: boss started, time-window pattern switched, boss defeated, ending shown, trial completed
- listen: arena entered, attack trigger, pattern timer elapsed, boss death confirmed

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

- chose: time-based pattern alternation with serialized durations/cooldowns and a death-gated quest step before mentor return
- avoided: HP-threshold phase transitions, summons/add waves, and direct boss death to quest completion
- why: keeps pattern readability consistent, keeps ending gated by real boss kill plus final mentor interaction, and reuses stable quest/interaction seams

## Acceptance Checks

- guardian fight is playable and beatable
- boss has at least 2 distinct readable attack patterns
- boss pattern switching is timer-driven (not HP-driven)
- boss defeat unlocks return-to-mentor only once, then completion only after mentor interaction
- final mentor payoff and win state are reachable in one run
- docs cover boss data, signals, scene wiring, and playable validation

## Requirements

1. create one guardian boss with minimum 2 attack patterns.
2. schedule pattern alternation by elapsed time (serialized durations/cooldowns), not HP thresholds.
3. route boss flow through current combat and quest seams.
4. wire boss death to unlock return-to-mentor, then mentor payoff to complete the trial.
5. end with one complete playable victory path.

## Editor Configuration

1. place boss arena trigger and boss prefab.
2. assign guardian setup asset fields for health, pattern telegraph/active durations, and cooldowns.
3. wire boss death event to quest progression (unlock return-to-mentor step only).
4. assign ending UI panel and mentor final dialog.
5. verify arena gate, intro trigger, and win-state refs.

## Unresolved Questions

- none
