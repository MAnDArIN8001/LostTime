# UI Runtime Step 005

## summary
- Implemented panel registry and panel factory for type-based registration and runtime instantiation.
- Added `PanelId`-based O(1) lookup for registration and created instances.
- Added duplicate active-instance prevention in factory create path.

## files changed
- `Assets/Scripts/UI/Runtime/UIPanelRegistration.cs` - registration DTO (PanelId, panel type, asset path).
- `Assets/Scripts/UI/Runtime/IUIPanelRegistry.cs` - registry contract.
- `Assets/Scripts/UI/Runtime/UIPanelRegistry.cs` - dictionary-based registry implementation.
- `Assets/Scripts/UI/Runtime/IUIPanelFactory.cs` - factory contract.
- `Assets/Scripts/UI/Runtime/UIPanelFactory.cs` - prefab load/instantiate/release implementation.

## scene/inspector
- No scene changes.
- No inspector changes required in this step.

## validation
- Static terminal-level validation only.
- Unity compile and runtime path validation pending.

## decision log
- Registry uses dictionary by `PanelId` for O(1) lookup.
- Factory returns existing created panel for the same `PanelId` to prevent duplicate active instances.
- Factory owns prefab release call and created panel object lifecycle.

## follow-ups
- Wire registry/factory to `UIService`.
- Add backend switch binding in installer.
- Add tests for duplicate open path and release-all cleanup.

## commit
- hash: pending (awaiting review approval)
- message: pending
