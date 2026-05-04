# UI Runtime Step 008

## summary
- Added Zenject `UIInstaller` for full UI runtime module bindings.
- Added backend switch binding for resources/addressables loader from ScriptableObject config.
- Moved panel registrations and runtime backend selection to dedicated ScriptableObject (`UIRuntimeConfig`).

## files changed
- `Assets/Scripts/DI/UIInstaller.cs` - installer bindings for service/loaders/registry/factory/input gate/shortcut bridge and panel registration bootstrap.
- `Assets/Scripts/UI/Runtime/UIRuntimeConfig.cs` - runtime config asset and panel entries for data-oriented setup.

## scene/inspector
- Add `UIInstaller` component to scene DI context.
- Create and assign `UIRuntimeConfig` asset.
- Optional: assign `_uiRuntimeRoot`.
- In `UIRuntimeConfig`, configure loader backend (`Resources` or `Addressables`).
- In `UIRuntimeConfig`, fill panel entries:
- `panelTypeName` as full type name (namespace + class).
- `assetPath` as Resources path or Addressables key, based on selected backend.

## validation
- Static terminal-level validation only.
- Unity compile/runtime validation pending.

## decision log
- Preserved current `InputInstaller` behavior; UI module integrates via additional installer, not by changing existing input installer.
- Switched from installer-local serialized list to ScriptableObject-driven runtime data for better extensibility.
- Lifecycle/disposal integration delegated to Zenject via `BindInterfacesAndSelfTo` on disposable services.
- Applied review style pass in `UIRuntimeConfig`: used `[field: SerializeField]` property for backend, exposed entries as `IReadOnlyList`, and aligned member order (serialized fields before public accessors in entry struct).

## follow-ups
- Scene wire `UIInstaller` into active context.
- Validate panel type names and asset paths in play mode.
- Migrate target windows to new UI runtime service in next step.

## commit
- hash: pending (awaiting review approval)
- message: pending
