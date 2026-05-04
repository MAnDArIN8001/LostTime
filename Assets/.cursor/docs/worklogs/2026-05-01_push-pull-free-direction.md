# Push Pull Free Direction

## summary
- Removed serialized movement axis limits from push and pull controllable world objects.
- Push now moves the object away from the interactor position.
- Pull now moves the object toward the interactor position.
- Added serialized x/y/z movement ignore mask for push and pull direction projection.
- Existing collider cast blocker check remains the only movement reachability gate.

## files changed
- `Assets/Scripts/Gameplay/Interaction/World/PushControllableWorldObject.cs` - direction resolved from object position minus interactor position; axis/range clamp removed.
- `Assets/Scripts/Gameplay/Interaction/World/PullControllableWorldObject.cs` - direction resolved from interactor position minus object position; axis/range clamp removed.
- `Assets/Scripts/Gameplay/Interaction/World/MovementAxisIgnoreMask.cs` - serializable x/y/z mask that removes selected direction components.
- `Assets/Scripts/Gameplay/Interaction/World/MovementAxisIgnoreMask.cs.meta` - Unity meta for new runtime script.
- `Assets/.cursor/docs/worklogs/2026-05-01_push-pull-free-direction.md` - worklog for this change.

## scene/inspector
- No scene files edited.
- Push/Pull components no longer require Movement Space, Push/Pull Axis, or Max Push/Pull Distance setup.
- Optional Ignored Movement Axes field: enable x, y, or z to remove movement on that world axis.
- Existing scene YAML may still contain old serialized fields until Unity reserializes the scene.

## validation
- Searched target scripts for removed axis/range members and helpers; no remaining references found.
- Static code review of x/y/z mask use in both direction resolvers.
- Unity editor compile/play validation pending.

## decision log
- Treated missing `Assets/.codex/context/context.md` as unavailable; no replacement source of truth was found in the workspace.
- Treated missing `Assets/.cursor/docs/WORKLOG_TEMPLATE.md` as unavailable and followed the existing worklog shape in `Assets/.cursor/docs/worklogs/`.
- Kept blocker detection through existing collider casts so movement is rejected when the requested endpoint/path would enter another collider.
- Applied ignored axes before normalization so speed/step distance stay consistent after projection.
- Did not edit `Assets/Scenes/SampleScene.unity` because it was already modified before this task and scene changes were not needed.

## follow-ups
- Open Unity and confirm affected push/pull objects compile and move relative to the player in Play Mode.
- Verify ignored axes in inspector: for ground-only movement, enable y.
- Optionally resave scenes/prefabs to drop old serialized axis/range fields from YAML.

## commit
- hash: pending (awaiting review approval)
- message: pending
