# Battery Socket Trigger Charge Flow

## Summary

Implemented a trigger-driven battery socket component that detects `IBattery`, disables push/pull control components on attach, tweens battery to socket root, reparents battery to socket, then invokes charge events.

## Context

- task goal: socket should auto-capture battery on trigger enter and emit charge event after attach animation
- current stage or planning context: environmental interaction extension in world interaction layer
- technical constraints that mattered: use existing DOTween dependency and keep inspector-driven wiring

## Files Changed

- `Assets/Scripts/Gameplay/Interaction/Core/IBattery.cs`
- `Assets/Scripts/Gameplay/Interaction/World/BatterySocketTrigger.cs`

## Scene/Inspector

1. Add `BatterySocketTrigger` to socket GameObject with a trigger collider.
2. Assign `_socketRoot` (or leave empty to use current transform).
3. Set tween params: `_attachDuration`, `_attachEase`, `_matchSocketRotation`.
4. Optionally assign `_componentsToDisable` for explicit control scripts.
5. Ensure battery prefab has at least one MonoBehaviour implementing `IBattery`.
6. Wire `_onCharged` UnityEvent listeners in inspector.

## Validation

1. static check: class resolves `IBattery` from entering collider hierarchy and blocks duplicate attach while tween runs
2. static check: after tween complete battery is parented to socket root and event raised in completion path
3. gap: Unity Play Mode runtime validation was not executed in this environment

## Decision Log

- chose: marker interface `IBattery` + component lookup via `GetComponentsInParent<MonoBehaviour>`
- avoided: hard dependency on specific battery MonoBehaviour type
- why: keeps socket reusable for any battery implementation

## Follow-Ups

- add optional occupancy reset/eject flow if puzzle needs re-usable socket
- decide whether colliders/rigidbody on battery should be toggled after docking
- add PlayMode test coverage for attach completion + event dispatch sequence

## Commit

- prefix used: `feat`
- subject: `feat: add battery socket trigger charge flow`
- status: `planned`
