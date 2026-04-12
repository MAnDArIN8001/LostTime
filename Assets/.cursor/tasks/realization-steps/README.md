# Realization Steps Usage

## Purpose

Use these prompt files to run stage-scoped implementation or planning work without drifting outside the current MVP frame.

Current truth priority:

1. `Assets/.cursor/context/context.md`
2. current `game-plan` docs
3. stage prompt files

If a stage prompt still reflects the older combat-first slice and conflicts with the context file, the context file wins.

## Files

- `00_Agent_System.md`: shared rules, required docs, architecture constraints, documentation flow
- `01_Stage1_Lock_MVP_Frame.md`: scope lock and architecture baseline
- `02_Stage2_Vertical_Slice.md`: first playable slice
- `03_Stage3_Full_Main_Loop.md`: main loop expansion
- `04_Stage4_Boss_And_Ending.md`: climax, boss, ending
- `05_Stage5_Polish.md`: readability, balance, presentation

## How To Use

1. Pick the current stage file.
2. Read `00_Agent_System.md` first.
3. Read the chosen stage file second.
4. Also read the required project docs listed in those files.
5. Execute only the mission and scope of that stage.
6. Create or update a worklog under `Assets/.cursor/docs/worklogs/`.

## Prompting Flow

When using a stage prompt in Cursor:

1. Paste or attach `00_Agent_System.md` with the chosen stage file.
2. Ask the agent to stay inside stage scope.
3. Ask it to document:
   - `Intent`
   - `Architecture`
   - `Data`
   - `Signals`
   - `Scene/Inspector`
   - `Validation`
   - `Decision Log`
   - `Worklog Path`
4. Ask it to finish with:
   - step-by-step requirements
   - exact editor or inspector setup steps
   - commit subject with approved prefix
   - unresolved questions

## Selection Rule

- Need to lock scope or architecture only: use Stage 1
- Need the first playable courtyard verb slice: use Stage 2
- Need the complete courtyard main loop: use Stage 3
- Need the finale and ending path: use Stage 4
- Need demo polish and readability: use Stage 5

## Editor Configuration

1. none

## Unresolved Questions

- none
