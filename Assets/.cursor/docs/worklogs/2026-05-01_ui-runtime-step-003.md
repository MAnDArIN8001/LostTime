# UI Runtime Step 003

## summary
- Implemented `AbstractUIPanel` base class for UI runtime.
- Added generated `PanelId` from runtime panel type.
- Added base config wiring (modal + cache policy) and deterministic show/hide/close flow.

## files changed
- `Assets/Scripts/UI/Runtime/AbstractUIPanel.cs` - abstract panel runtime with lifecycle hooks and visibility state.

## scene/inspector
- New optional inspector fields on inheritors:
- `_isModal`
- `_cachePolicy`
- `_hideOnEnable`
- `_panelRoot` (optional, defaults to current object)

## validation
- Static review only in terminal.
- Unity compile/runtime validation pending in Editor.

## decision log
- `PanelId` generated in `Awake` from panel runtime type (`PanelId.From(GetType())`).
- Lifecycle extension points exposed via protected virtual hooks instead of service/resource coupling.
- `Close` delegates visibility transition to `Hide`, then triggers close hook with reason.

## follow-ups
- Implement loader abstraction backends (resources/addressables path) in next step.
- Add registry/factory and service stack orchestration.
- Validate inherited panel behavior in migrated presenters.

## commit
- hash: pending
- message: pending
