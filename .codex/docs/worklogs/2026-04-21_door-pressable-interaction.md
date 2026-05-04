# Door Pressable Interaction

## Summary

Added a reusable door interaction component driven by the existing `press` interaction path. The door opens away from the player, supports serialized lock state, and now broadcasts state changes through regular C# events instead of UnityEvents.

## Context

- task goal: add door opening interaction compatible with current `push / pull / press` interaction language
- current stage or planning context: extend existing interaction seams without adding a parallel interaction flow
- technical constraints that mattered: keep interaction independent from character FSM state, avoid UnityEvents, preserve current field styling, do not touch unrelated dirty scene files

## Files Changed

- `Assets/Scripts/Gameplay/Interaction/World/DoorPressableWorldObject.cs`
- `Assets/Scripts/Gameplay/Interaction/World/DoorPressableWorldObject.cs.meta`

## Scene/Inspector

1. Add `DoorPressableWorldObject` to the door GameObject or door root.
2. Ensure the door object or a child object has a `Collider` so pointer targeting works.
3. Assign `Rotation Pivot` to the hinge transform used as the rotation origin.
4. Set `Rotation Axis` to the hinge rotation axis, usually `0, 1, 0`.
5. Set `Door Normal Axis` to the forward-facing normal of the closed door leaf.
6. Tune `Open Angle`, `Open Duration`, and `Open Ease`.
7. Toggle `Is Locked` if the door must reject opening.
8. If needed, subscribe from code to `Opened`, `Closed`, `LockedPressed`, or `Pressed`.

## Validation

1. Verified the component follows the existing `IPressable` interaction seam and compiles structurally against current interfaces.
2. Verified UnityEvents were removed and replaced by standard C# events for broadcast/control flow.
3. Unity runtime validation in-editor was not performed from this environment, so hinge axis and away-from-player rotation should be checked on the target prefab in scene.

## Decision Log

- chose: implement the door as `IPressable`
- avoided: adding a new door-specific interaction controller path
- why: press already matches the requested one-button interaction and remains independent from character movement state
- chose: compute signed open angle from player position relative to hinge and door normal
- avoided: fixed open direction
- why: requirement says the door must always open away from the player
- chose: use plain C# events for opened/closed/locked notifications
- avoided: `UnityEvent`
- why: current project flow keeps configuration and subscription ownership in code

## Follow-Ups

- validate the component on the actual door prefab and tune `Door Normal Axis` if the mesh local forward differs
- consider adding a small audio or FX listener via code subscriptions if door feedback is needed
- confirm whether locked doors should still expose `Locked` as HUD hint or use a different prompt string per level

## Commit

- prefix used: `feat`
- subject: `feat: add door pressable interaction`
- status: `planned`
