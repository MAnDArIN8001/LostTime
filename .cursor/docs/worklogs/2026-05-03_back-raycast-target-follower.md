# Back Raycast Target Follower

## summary
- Reworked the component to a standard camera-collision model: raycast from target to desired camera position and place camera at hit point with a safety offset.
- Exposed inspector-driven references and movement/raycast settings for scene-level setup.
- Logged the `AGENTS.md` path mismatch as legacy and used the real `.codex`/`.cursor` files present in repo root.

## files changed
- `Assets/Scripts/Utils/Followers/BackRaycastTargetFollower.cs` - new runtime component for backward-hit driven smooth following.
- `.cursor/docs/worklogs/2026-05-03_back-raycast-target-follower.md` - implementation worklog for this task.

## scene/inspector
- No scene file edited in this step.
- Add `BackRaycastTargetFollower` to the moving object or a controller object.
- Assign `_movable`, `_targetTransform`, and `_defaultTransform`.
- Set `_ignoreHierarchyRoot` to the moving object's root if it has colliders that should be ignored by the ray.
- Tune `_positionSmoothTime`, `_collisionOffset`, `_layerMask`, and optional `_defaultOffset`.

## validation
- Code reviewed against existing utility component patterns in `Assets/Scripts/Utils` and existing SmoothDamp usage.
- Fixed namespace collision inside `Utils.*` scope by qualifying `UnityEngine.Physics` explicitly.
- Removed stateful blocked/default switching and switched to deterministic target-to-desired ray path evaluation each frame.
- Unity compilation/runtime not executed in this environment, so inspector wiring and script compile still need in-editor verification.

## decision log
- Implemented as a generic utility component instead of binding it to IK or interaction systems because request is scene-driven and reusable.
- Used `Vector3.SmoothDamp` for soft movement to match an existing project pattern from `CameraPositionComposer`.
- Added self-hierarchy ignore support because backward rays from the same object can easily hit local colliders and break the intended toggle.
- Removed debug gizmo support after follow-up request to keep the component minimal.
- Replaced hysteresis state machine with direct geometric solve: `target -> desired` ray defines obstruction, and resolved position no longer feeds back into collision query.
- Treated `Assets/.codex/context/context.md` and `Assets/.cursor/docs/WORKLOG_TEMPLATE.md` references from `AGENTS.md` as legacy path typos; actual repo files are `.codex/context/context.md` and `.cursor/docs/WORKLOG_TEMPLATE.md`.

## follow-ups
- Verify script compiles in Unity and confirm no `.meta` handling is needed in current workflow.
- Optional: extend component with rotation follow if design needs it later.
- Optional: rename component to camera-specific wording if this utility will be used only for camera collision.
- Await reviewer approval before any commit.

## commit
- hash: pending (awaiting review approval)
- message: pending
