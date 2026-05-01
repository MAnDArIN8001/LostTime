# UI Runtime Step 004

## summary
- Implemented resource loader backends for UI runtime.
- Added `Resources` backend and real `Addressables` backend.
- Added release flow for loaded prefab handles/assets.

## files changed
- `Assets/Scripts/UI/Runtime/ResourcesPanelLoader.cs` - resources-based panel prefab loader and release path.
- `Assets/Scripts/UI/Runtime/AddressablesPanelLoader.cs` - addressables-based panel prefab loader and handle release path.

## scene/inspector
- No scene changes.
- No inspector changes required in this step.

## validation
- Static terminal review only.
- Runtime validation in Unity pending (load/release paths).

## decision log
- Chose real Addressables implementation because `com.unity.addressables` is present in `Packages/manifest.json`.
- Kept loader contract synchronous to match current `IResourceLoader` API.
- Removed internal Addressables cache by `PanelId` after review: loader now stays thin and stateless, responsibility kept to load/release only.

## follow-ups
- Add registry/factory to provide `assetPath` and backend selection.
- Add tests for repeated load/release and duplicate load path.
- Revisit async loader API migration (`UniTask`) if service pipeline requires it.

## commit
- hash: pending (awaiting review approval)
- message: pending
