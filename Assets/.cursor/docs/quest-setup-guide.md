# Quest Setup Guide

> Legacy note:
> This guide documents the older combat-first seal and guardian setup.
> It is not the current project source of truth.
> Before using this guide, read `Assets/.cursor/context/context.md`.
> If this guide conflicts with the current courtyard `push / pull / press` direction, treat this file as legacy reference only.

## Purpose

This document explains how to configure the quest part of the current MVP scene in Unity based on the implemented flow and the current project architecture.

The guide is based on:

- `Assets/Scripts/Quest/VerticalSliceQuestProgression.cs`
- `Assets/Scripts/UI/VerticalSliceHudPresenter.cs`
- `Assets/Scripts/UI/VerticalSliceWinScreen.cs`
- `Assets/Scripts/Loot/Systems/InteractionController.cs`
- `Assets/.cursor/tasks/game-plan/03_Realization_Steps.md`
- `Assets/.cursor/tasks/realization-steps/03_Stage3_Full_Main_Loop.md`

## Current Quest Flow

The current quest implementation is no longer the old short Stage 2 chain.

The actual flow in code is:

1. `TalkToMentor`
2. `RestoreSeals`
3. `UnlockArena`
4. `DefeatGuardian`
5. `ReturnToMentor`
6. `Completed`

That means the scene should be configured around this route:

1. Player talks to mentor.
2. Seal encounters become active.
3. Player defeats enemies tied to seals.
4. Player restores all seals.
5. Arena opens.
6. Player defeats guardian.
7. Player returns to mentor.
8. Quest completes.

## Important Architecture Note

Quest logic in this project is an orchestration layer.

It should not be embedded inside:

- movement logic
- input logic
- combat logic
- loot logic

Instead, `VerticalSliceQuestProgression` listens to events coming from:

- `InteractionTarget` for mentor and seals
- `IEncounterEnemy.Died` for encounter enemies and guardian

This means correct scene wiring is more important than adding extra quest scripts.

## Main Components Involved

### 1. Quest runtime

- `Assets/Scripts/Quest/VerticalSliceQuestProgression.cs`

This is the main quest controller for the scene.

It stores:

- mentor reference
- seal objective list
- arena unlock targets
- guardian reference
- current quest step

### 2. Interaction system

- `Assets/Scripts/Loot/Systems/InteractionController.cs`

This is responsible for:

- detecting interactable targets through raycast
- showing interaction hints
- calling interact on focused objects

Without it, mentor and seal interactions will not work.

### 3. HUD

- `Assets/Scripts/UI/VerticalSliceHudPresenter.cs`

This displays:

- HP
- MP
- current objective
- interaction hint

### 4. Win screen

- `Assets/Scripts/UI/VerticalSliceWinScreen.cs`

This listens for quest completion and shows a final panel.

## What Must Exist In The Scene

To make the quest work correctly, the scene must contain:

1. Player with interaction and combat wiring.
2. Mentor object with `InteractionTarget`.
3. One or more seal objects with `InteractionTarget`.
4. One encounter enemy per seal, or a valid encounter root.
5. Arena unlock objects that are hidden until seals are restored.
6. Guardian boss implementing `IEncounterEnemy`.
7. Quest object with `VerticalSliceQuestProgression`.
8. HUD object with `VerticalSliceHudPresenter`.
9. Optional win screen object with `VerticalSliceWinScreen`.

## Step-By-Step Inspector Setup

## Step 1. Create or select the quest object

In the scene, create a dedicated object such as:

- `QuestController`

Add component:

- `VerticalSliceQuestProgression`

This object will own the full quest state.

## Step 2. Assign the mentor

On `VerticalSliceQuestProgression`, assign:

- `_mentorTarget`

Expected target:

- a scene object representing the mentor
- containing `InteractionTarget`
- containing a collider so the interaction ray can hit it

What this does:

- first mentor interaction starts the quest
- final mentor interaction completes the quest

## Step 3. Configure seals in `_seals`

This is the most important part of the setup.

In `VerticalSliceQuestProgression`:

1. Set `_seals` array size to the number of seal objectives you want.
2. For MVP full loop, set it to `3`.

For each seal entry, configure:

- `Name`
- `SealTarget`
- `EncounterBehaviour`
- `EncounterRoot`

### Meaning of each field

`Name`

- editor-only readable label for the seal objective

`SealTarget`

- the seal object in the scene
- must have `InteractionTarget`
- must have collider

`EncounterBehaviour`

- component implementing `IEncounterEnemy`
- usually the enemy controller or encounter script linked to that seal

`EncounterRoot`

- root object that should be activated when the mentor starts the trial
- use this if the whole encounter should be enabled as one root

Important behavior:

- on quest start, seal encounters are disabled until mentor interaction
- after talking to mentor, quest activates encounter roots
- player can restore a seal only if its encounter is dead, unless no encounter is assigned

## Step 4. Configure arena unlock

In `VerticalSliceQuestProgression`, assign:

- `_arenaUnlockTargets`
- `_arenaUnlockDelay`

`_arenaUnlockTargets` should contain objects that must appear or activate after all seals are restored, for example:

- arena gate open object
- portal
- arena blocker removal object
- boss entrance trigger

Important behavior:

- these objects are disabled in `OnEnable`
- after all seals are restored, they are enabled after `_arenaUnlockDelay`
- quest then advances to guardian fight

Recommended:

- keep these objects disabled by quest orchestration only
- do not manually enable them elsewhere unless intentionally overriding flow

## Step 5. Assign guardian boss

In `VerticalSliceQuestProgression`, assign:

- `_guardianEncounter`

Expected target:

- a component implementing `IEncounterEnemy`

What this does:

- when the guardian dies during `DefeatGuardian`, quest advances to `ReturnToMentor`

If this reference is missing:

- the quest will not advance properly after arena unlock

## Step 6. Configure mentor object

The mentor object should have:

1. collider
2. `InteractionTarget`
3. optional mark/highlight visual
4. optional dialog presenter or UnityEvent-based dialog handoff

Requirements:

- object must be hittable by the player's interaction ray
- interaction prompt should be readable
- if mentor becomes unavailable after first interaction, final quest completion can break

The quest code tries to recover final mentor interaction by resetting interaction state when needed, but proper mentor setup is still required.

## Step 7. Configure seal objects

Each seal object should have:

1. collider
2. `InteractionTarget`
3. optional mark/highlight visual
4. single-use interaction behavior if desired

Requirements:

- the player must be able to focus the seal with the interaction ray
- the seal must stay assigned to the matching `_seals` element

Important behavior:

- if a seal has no assigned encounter, it is considered immediately restorable
- that is useful for tests, but can accidentally skip intended combat gating

## Step 8. Configure encounter enemies

Each encounter used by the quest should:

1. implement `IEncounterEnemy`
2. fire its death flow correctly
3. be referenced in the matching seal objective

You can set either:

- `EncounterBehaviour` only
- `EncounterRoot` only
- both, when behavior lives under a root object

Recommended setup:

- `EncounterBehaviour` points to the component reporting `Died` and `IsDead`
- `EncounterRoot` points to the root object that should be enabled or disabled

## Step 9. Configure player interaction

The player side must include:

- `InteractionController`
- `DirectionalRaycaster`

`InteractionController` should be correctly wired so that it can:

- detect mentor
- detect seals
- show focus mark
- show interaction hint
- call interaction on the focused target

If this is broken, the quest code may be correct but progression will feel non-functional.

Check:

1. ray origin and direction
2. layer filtering
3. collider placement
4. target visibility and reachable range

## Step 10. Configure HUD

Add or select a HUD object with:

- `VerticalSliceHudPresenter`

Assign:

- `_characterVitals`
- `_characterMana`
- `_questProgression`
- `_interactionController`
- `_healthLabel`
- `_manaLabel`
- `_objectiveLabel`
- `_interactionHintLabel`

This allows the HUD to react to:

- health changes
- mana changes
- objective changes
- interaction hint changes

Without `_questProgression`, objective text will not update.

Without `_interactionController`, interaction hint text will not update.

## Step 11. Configure win screen

If you want final completion feedback, add or configure:

- `VerticalSliceWinScreen`

Assign:

- `_questProgression`
- `_winPanelRoot`

What this does:

- when the quest reaches `Completed`, the win panel is shown

## Practical Wiring Order

Use this order in Unity to avoid missing references:

1. Set up mentor object.
2. Set up all seal objects.
3. Set up all encounter enemies.
4. Set up guardian.
5. Create or select quest object.
6. Fill `_mentorTarget`.
7. Fill `_seals`.
8. Fill `_arenaUnlockTargets`.
9. Fill `_guardianEncounter`.
10. Connect player `InteractionController`.
11. Connect HUD.
12. Connect win screen.

## Play Mode Validation Checklist

Run the scene and verify the following in order:

1. Scene starts with objective text asking the player to talk to the mentor.
2. Looking at mentor shows highlight and interaction hint.
3. Interacting with mentor advances the quest and activates seal encounters.
4. Seal encounters are not active before mentor talk.
5. Defeating an encounter allows the connected seal to be restored.
6. Restoring one seal updates objective progress count.
7. Restoring all seals activates arena unlock targets after delay.
8. Objective changes to guardian fight.
9. Killing guardian changes objective to return to mentor.
10. Talking to mentor again completes the quest.
11. Win panel appears if `VerticalSliceWinScreen` is wired.

## Common Failure Cases

### Mentor or seal cannot be interacted with

Possible causes:

- missing collider
- missing `InteractionTarget`
- wrong layer mask
- raycaster not aimed correctly
- object not under the expected interaction path

### Seal restores before enemy is defeated

Possible cause:

- no valid `EncounterBehaviour` assigned for that seal

### Seal never becomes restorable

Possible causes:

- assigned encounter does not implement `IEncounterEnemy`
- death event is not firing
- wrong encounter linked to the seal

### Arena never opens

Possible causes:

- not all seals are counted as restored
- `_arenaUnlockTargets` not assigned
- quest flow blocked before `UnlockArena`

### Guardian death does not advance quest

Possible causes:

- `_guardianEncounter` missing
- guardian does not implement `IEncounterEnemy`
- guardian death callback is not firing

### Objective text does not update

Possible causes:

- `VerticalSliceHudPresenter._questProgression` missing
- objective TMP label missing

## Recommendation

For new scene setup, prefer the current `_seals` array workflow.

Do not rely on legacy fields:

- `_sealTarget`
- `_beastEncounter`
- `_beastRoot`

Those exist only for backward compatibility with an older Stage 2 scene layout.

## Summary

To configure the quest correctly, you do not need a separate quest framework.

You need to correctly wire scene references around `VerticalSliceQuestProgression`:

1. mentor
2. seals
3. encounters
4. arena unlock objects
5. guardian
6. interaction controller
7. HUD
8. win screen

If all of these references are correct, the quest flow should work as designed from start to finish.
