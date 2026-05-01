# UI Input Blocking Per Panel

## summary
- Changed UI input-gating policy from "any open panel blocks input" to "only panels marked as blocking input".
- Added explicit panel config flag `BlocksGameplayInput`.
- Wired service gate sync to respect this flag.

## files changed
- `Assets/Scripts/UI/Runtime/UIPanelConfig.cs` - added `BlocksGameplayInput` in config contract.
- `Assets/Scripts/UI/Runtime/AbstractUIPanel.cs` - added serialized `_blocksGameplayInput` panel setting.
- `Assets/Scripts/UI/Runtime/UIService.cs` - gate now blocks only if any open panel has `Config.BlocksGameplayInput == true`.

## scene/inspector
- For each panel prefab with `AbstractUIPanel` set:
- `Blocks Gameplay Input = true` only for blocking windows (e.g. modal dialogs).
- `Blocks Gameplay Input = false` for non-blocking overlays/hints.

## validation
- Static code validation in terminal.
- Runtime validation pending in Unity play mode.

## decision log
- Input control should be panel-specific, not globally tied to "panel opened" state.
- Service computes block condition from open panel configs, preserving idempotent restore flow.

## follow-ups
- Verify migrated panels defaults in prefabs (`GuideStory`, `Win`, `InteractionHint`).
- Add test coverage for mixed blocking/non-blocking open panels.

## commit
- hash: pending (awaiting review approval)
- message: pending
