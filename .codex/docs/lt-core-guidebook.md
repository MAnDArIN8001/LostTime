# LT-Core Guidebook

## Purpose

This document explains what was implemented for the `LT-Core` branch of the `push / pull / press` interaction system and how to configure it in Unity.

This guide is based on:

- `Assets/.aiagents/vertical-slice-mvp-plan.json`
- `Assets/Scripts/Gameplay/Interaction/Core/*`
- `Assets/Scripts/Gameplay/Interaction/Character/*`
- `Assets/Scripts/Gameplay/Interaction/World/*`
- `Assets/Scripts/Gameplay/Interaction/Authoring/*`
- `Assets/Scripts/Loot/Systems/InteractionController.cs`
- `Assets/Scripts/Loot/Systems/Character.cs`

## What LT-Core Includes

`LT-Core` is the reusable interaction branch for three verbs:

- `Press`
- `Push`
- `Pull`

It is intentionally separated from:

- quest logic
- HUD logic
- scene-specific puzzle logic

The current implementation covers:

1. core contracts and runtime primitives
2. character-side interaction driver
3. world interaction adapters
4. authoring guards
5. runtime diagnostics and gizmos

## Architecture Overview

### Core contracts

Folder:

- `Assets/Scripts/Gameplay/Interaction/Core`

Main types:

- `InteractionIntent`
- `ControlMode`
- `PointerTargetContext`
- `InteractionFocusContext`
- `IPressable`
- `IControlable`
- `ControlSession`
- `ControlSessionSnapshot`

Purpose:

- define common interaction vocabulary
- separate one-shot `Press` from sustained `Push` / `Pull`
- provide a generic pointer/focus context
- provide runtime session primitives for control interactions

### Focus bridge

Files:

- `Assets/Scripts/Loot/Systems/InteractionController.cs`
- `Assets/Scripts/Loot/Systems/LootInteractionFocus.cs`
- `Assets/Scripts/Loot/Systems/LootInteractionFocusDiscovery.cs`

Purpose:

- keep existing interaction flow alive
- bridge the old `IMarkable` / `IInteractable` / `ITakable` system into the new focus model
- expose `CurrentFocusContext` for the new interaction feature

### Character interaction feature

Folder:

- `Assets/Scripts/Gameplay/Interaction/Character`

Main types:

- `CharacterInteractionDriver`
- `PointerDrivenInteractionIntentResolver`
- `InteractionIntentResolution`
- `ICharacterControlSessionPolicy`
- `DefaultCharacterControlSessionPolicy`
- `CharacterInteractionFrameInput`

Purpose:

- remove direct interaction logic from `Character.Update()`
- resolve `Press` / `Push` / `Pull` from runtime context
- manage and maintain `ControlSession`
- define rules for when movement and control session may coexist

Current policy:

- `Aim` and `Cast` block or stop control session
- movement is allowed in parallel with active control session
- simple `interact / take` remains the fallback path

### World adapters

Folder:

- `Assets/Scripts/Gameplay/Interaction/World`

Main components:

- `PressableWorldObject`
- `PushControllableWorldObject`
- `PullControllableWorldObject`

Purpose:

- provide reusable scene components for the three LT-Core verbs
- keep world object logic out of quest and HUD
- let the same contracts work across different puzzle objects

### Authoring and diagnostics

Folders:

- `Assets/Scripts/Gameplay/Interaction/Authoring`
- `Assets/Scripts/Gameplay/Interaction/Character`

Main types:

- `InteractionAuthoringGuards`
- `InteractionRuntimeDiagnosticsGizmo`
- `CharacterInteractionDiagnostics`
- `CharacterInteractionDiagnosticsSnapshot`

Purpose:

- normalize broken inspector data
- warn about missing colliders for pointer-driven interaction
- expose current focus, pointer hit, intent and control session
- draw lightweight gizmos for runtime debugging

## Existing Player Wiring

The current player integration already exists in code.

Relevant files:

- `Assets/Scripts/Loot/Systems/Character.cs`
- `Assets/Scripts/Loot/Systems/InteractionController.cs`

Important behavior:

1. `Character` updates movement and combat as before.
2. `CharacterInteractionDriver` runs after movement state machine update.
3. `InteractionController` still supports current `IInteractable` and `ITakable` flow.
4. LT-Core now adds the ability to resolve control intent and maintain control sessions.

This means you do not need to rewrite the player controller to use LT-Core.

## Unity Setup

## Step 1. Verify player-side references

Select the player object and confirm:

- `Character` exists
- `InteractionController` exists
- the serialized `_interactionController` field on `Character` is assigned
- `InteractionController._directionalRaycaster` is assigned

Without this, neither legacy interactables nor LT-Core focus resolution will work.

## Step 2. Verify pointer targetability

Any object intended for `Press`, `Push`, or `Pull` should have:

- a `Collider` on the object or in children
- a transform positioned so the player ray can hit it

If `Require Pointer Target` is enabled on an LT-Core world object and no collider exists, authoring guards will warn in the editor.

## Step 3. Configure a press object

Create or select a scene object and add:

- `PressableWorldObject`

Configure:

- `_pressPrompt`
- `_singleUse`
- `_consumeOnPress`
- `_requirePointerTarget`
- `_onPressed`

Recommended:

- keep `_requirePointerTarget = true` for scene buttons, levers, or explicit click targets
- use `_onPressed` to trigger scene responses

Use cases:

- button
- lever
- trigger plate proxy
- one-shot scene switch

## Step 4. Configure a push object

Create or select a scene object and add:

- `PushControllableWorldObject`

Configure:

- `_controlPrompt`
- `_requirePointerTarget`
- `_movementSpace`
- `_pushAxis`
- `_pushSpeed`
- `_maxPushDistance`
- `_snapBackOnControlEnd`
- `_onControlStarted`
- `_onControlUpdated`
- `_onControlEnded`

How it works:

- object supports only `ControlMode.Push`
- direction is resolved from pointer hit normal first
- if that is not useful, fallback uses interactor and hit position
- movement is clamped relative to the original position

Recommended defaults:

- `_movementSpace = World`
- `_pushAxis = (0, 0, 1)` or the dominant local forward axis
- `_pushSpeed > 0`
- `_maxPushDistance > 0`

Use cases:

- sliding block
- pushable altar
- forward-only puzzle pillar

## Step 5. Configure a pull object

Create or select a scene object and add:

- `PullControllableWorldObject`

Configure:

- `_controlPrompt`
- `_requirePointerTarget`
- `_movementSpace`
- `_pullAxis`
- `_pullSpeed`
- `_maxPullDistance`
- `_snapBackOnControlEnd`
- `_onControlStarted`
- `_onControlUpdated`
- `_onControlEnded`

How it works:

- object supports only `ControlMode.Pull`
- direction is resolved from pointer hit normal first
- fallback uses interactor and hit position
- movement is clamped relative to the original position

Use cases:

- chain or handle
- retractable block
- pullable ritual mechanism

## Step 6. Enable diagnostics in scene

To debug LT-Core runtime state, create an object such as:

- `LTCoreDiagnostics`

Add:

- `InteractionRuntimeDiagnosticsGizmo`

What it gives you:

- inspector summary of current interaction runtime state
- focus target visualization
- pointer hit point and normal visualization
- active control session visualization

Recommended:

- keep this object in test scenes and vertical-slice scenes
- disable or remove it only if scene clutter becomes a problem

## Step 7. Play mode validation

Use this order:

1. Look at an LT-Core object and confirm the object is hittable by the interaction ray.
2. Verify focus changes on the object.
3. For `PressableWorldObject`, trigger the interaction button and confirm `_onPressed` fires.
4. For `PushControllableWorldObject`, aim at the object, trigger interaction, then use mouse scroll to drive push intent.
5. For `PullControllableWorldObject`, do the same for pull direction.
6. Confirm movement still works while a control session is active.
7. Confirm aim or cast interrupts control session.
8. Confirm diagnostics gizmo updates during focus and control changes.

## Important Notes About Input

Current behavior:

- `CommunicationAction` remains the main interaction trigger
- pointer-driven control intent is inferred from runtime pointer signal
- mouse scroll is currently used as the control-axis signal source

This means:

- basic interact/take behavior still works as before
- `Push` and `Pull` are staged on top of the same interaction button
- no input asset rewrite was required for LT-Core

## Authoring Guards

The following safeguards are already in place:

- prompts are trimmed and defaulted if empty
- zero axes are normalized to safe fallback axes
- negative speed and distance values are clamped
- invalid `Space` values are normalized
- missing colliders produce warnings when pointer target is required
- meaningless `PressableWorldObject` flag combinations are corrected
- zero-speed control objects produce warnings

This means scene setup errors should usually surface early in the Inspector.

## Current Limitations

The LT-Core branch is implemented, but scene orchestration on top of it is not part of this guide.

Not covered here:

- quest bridge
- HUD prompts bridge
- full scene smoke-path assembly

Those belong to the later `LT-VS-*` tasks.

Also note:

- CLI `dotnet build` could not fully validate this Unity project in the current environment
- final validation should be done in Unity Editor play mode

## Recommended Scene Assembly Pattern

For a first LT-Core test scene, use:

1. player with `Character` and `InteractionController`
2. one `PressableWorldObject`
3. one `PushControllableWorldObject`
4. one `PullControllableWorldObject`
5. one `InteractionRuntimeDiagnosticsGizmo`

This gives you a compact smoke test before bridging into quest and HUD.

## Quick Checklist

- Player has `Character` and assigned `InteractionController`
- `InteractionController` has a working `DirectionalRaycaster`
- Every LT-Core world object has a collider
- Prompts are readable
- Push/Pull axes match intended scene direction
- Push/Pull speed is non-zero
- Max distance is non-zero if clamp is desired
- Diagnostics gizmo exists in the test scene
- Play mode confirms focus, press, push, pull, and control session behavior

## Summary

`LT-Core` is now a separate reusable interaction branch with:

- shared contracts
- character-side runtime handling
- world-side reusable adapters
- authoring validation
- runtime diagnostics

To use it in Unity, wire the player correctly, add the relevant world component for each object, ensure colliders are present, and validate the result in play mode with the diagnostics gizmo enabled.
