# UI Runtime Step 009

## summary
- Migrated target windows flow to `IUIService`: GuideStory, WinScreen, InteractionHint.
- Added dedicated panel classes for each migrated window.
- Updated event-driven presenters to open/close/update panels through UI service.

## files changed
- `Assets/Scripts/UI/GuideStoryPanel.cs` - runtime panel for story content and close action.
- `Assets/Scripts/UI/InteractionHintPanel.cs` - runtime panel for hint text rendering with input labels.
- `Assets/Scripts/UI/VerticalSliceWinPanel.cs` - runtime win panel.
- `Assets/Scripts/UI/GuideStoryPanelEventBusPresenter.cs` - now opens `GuideStoryPanel` via `IUIService`.
- `Assets/Scripts/UI/InteractionHintEventBusPresenter.cs` - now opens/closes `InteractionHintPanel` via `IUIService`.
- `Assets/Scripts/UI/VerticalSliceWinScreen.cs` - now opens `VerticalSliceWinPanel` via `IUIService`.
- `Assets/Scripts/UI/Runtime/UIPanelFactory.cs` - switched instantiation to Zenject container (`InstantiatePrefab`) for injected panel dependencies.

## scene/inspector
- Create 3 panel prefabs (or reuse existing roots as prefabs) with components:
- `UI.GuideStoryPanel`
- `UI.InteractionHintPanel`
- `UI.VerticalSliceWinPanel`
- In `UIRuntimeConfig` add panel entries:
- `panelTypeName`: `UI.GuideStoryPanel`, `UI.InteractionHintPanel`, `UI.VerticalSliceWinPanel`
- `assetPath`: path/key for each prefab according to selected backend.
- Keep scene presenters:
- `GuideStoryPanelEventBusPresenter`
- `InteractionHintEventBusPresenter`
- `VerticalSliceWinScreen`

## validation
- `dotnet build Assembly-CSharp.csproj -nologo` executed, returned non-zero without diagnostics in this environment.
- Runtime validation pending in Unity play mode.

## decision log
- Preserved existing scene entrypoints (presenter scripts) to minimize scene wiring breakage.
- Moved visual behavior into panel components while presenters remain event orchestration layer.
- Factory instantiation moved to Zenject container so panel dependencies (e.g. optional `IUIService`) are injectable.

## follow-ups
- Add/verify `UIRuntimeConfig` entries for migrated panels.
- Validate close behavior and stack ordering in play mode with real prefab registrations.
- Add tests for migration flow and duplicate open path.

## commit
- hash: pending (awaiting review approval)
- message: pending
