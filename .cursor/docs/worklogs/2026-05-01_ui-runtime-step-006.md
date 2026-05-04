# UI Runtime Step 006

## summary
- Implemented `UIService` with deterministic stack-based panel orchestration.
- Added keepalive cache behavior (close -> hide/cache, reopen -> reuse).
- Added `IDisposable` cleanup flow for open and cached panels.

## files changed
- `Assets/Scripts/UI/Runtime/IUIService.cs` - extended service contract with `IDisposable`.
- `Assets/Scripts/UI/Runtime/UIService.cs` - core service implementation (`Open`, `Close`, `CloseTop`, `CloseAll`, `IsOpen`, `TryGet`, `Dispose`).

## scene/inspector
- No scene changes.
- No inspector changes required in this step.

## validation
- Terminal-level static validation only.
- Unity compile/runtime validation pending.

## decision log
- Single stack (`List<PanelId>`) used for deterministic top-close behavior.
- `KeepAlive` panels are not destroyed on close; moved to internal cache and reused on next open.
- Dispose path closes all open panels and releases cached panels via factory.

## follow-ups
- Integrate input gating / esc flow in next step.
- Add installer bindings for service lifecycle ownership.
- Add tests for stack order, keepalive reuse, and dispose cleanup.

## commit
- hash: pending (awaiting review approval)
- message: pending
