# Realization Steps

## Stage 1. Lock MVP frame

Goal: freeze scope before feature work.

Steps:

1. keep one region, one hero, one mentor, one main quest
2. lock the main interaction language to `push / pull / press`
3. keep progression based on courtyard world-state changes
4. treat combat-heavy content as optional or legacy

Output:

- concept approved
- core loop approved
- mechanic map approved

Inspector work:

1. none

## Stage 2. Vertical slice

Goal: one fully playable short path.

Steps:

1. add or adapt one direct interaction path for `press`
2. create one movable object path for `push` or `pull`
3. build one small mechanism chain
4. create one mentor or starting focal interaction
5. add basic HUD: objective and interact hint minimum
6. document scene wiring and validation path

Output:

- player can enter the courtyard, learn one verb, solve one chained mechanism, and reach one clear progression beat

Inspector work:

1. bind interaction actions in `Assets/InputSystem_Actions.inputactions` if needed
2. assign interact targets and mark visuals on world mechanisms
3. wire objective and hint references in scene canvas
4. assign serialized links between mechanism components

## Stage 3. Full main loop

Goal: complete full start-to-finish story path.

Steps:

1. expand the courtyard into several connected puzzle beats
2. chain `push / pull / press` across multiple sectors
3. add timed states and one hazard pattern
4. add final ritual objective
5. add return or completion step
6. keep world-state readability high

Output:

- full quest chain works from intro to ending

Inspector work:

1. place mechanism chains across the courtyard
2. wire gates, barriers, plates, sliders, and final ritual objects
3. configure hazard zones and timings
4. wire quest progression references

## Stage 4. Finale and pressure

Goal: add climax and clear payoff.

Steps:

1. build final combined interaction challenge
2. add pressure through hazards, timing, or light disruption
3. open ending state after ritual stabilization
4. add final mentor handoff and win screen if used

Output:

- game has clear climax and resolution

Inspector work:

1. place final ritual objects and pressure sources
2. wire completion conditions to quest progression
3. assign ending UI panel and completion presentation

## Stage 5. Polish

Goal: make MVP readable and presentable for diploma.

Steps:

1. add interaction, mechanism, hazard, and UI sounds
2. add VFX for magic state changes and ritual beats
3. improve readability of object states and affordances
4. improve quest text clarity
5. improve onboarding in first minute

Output:

- stable demo with readable feedback

Inspector work:

1. assign audio clips on interaction, UI, and hazard objects
2. assign VFX prefabs on mechanisms, barriers, and ritual states
3. tune values in ScriptableObjects and serialized fields

## Delivery order

1. Stage 1 docs
2. Stage 2 vertical slice
3. Stage 3 full loop
4. Stage 4 boss ending
5. Stage 5 polish

## Requirements

1. Never expand scope before Stage 4 works.
2. Ship playable verb slice before adding wider content.
3. Prefer ScriptableObjects for tunable mechanism and timing data.
4. Prefer existing FSM / input / interaction seams over parallel systems.
5. Every stage must end in playable scene state.

## Unresolved Questions

- should ranged magic be mandatory in the MVP or optional support
- should finale pressure come only from hazards, or also from a simple hostile source
