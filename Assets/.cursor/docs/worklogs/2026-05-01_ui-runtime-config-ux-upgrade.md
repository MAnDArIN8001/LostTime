# UI Runtime Config UX Upgrade

## summary
- Reworked UI config pipeline to ScriptableObject-per-panel definitions.
- Removed strict manual type-string entry from main runtime config flow.
- Added custom inspector validation button for GD-friendly setup checks.

## files changed
- `Assets/Scripts/UI/Runtime/UIPanelDefinition.cs` - panel-level definition asset (panel script sync, prefab/key references).
- `Assets/Scripts/UI/Runtime/UIRuntimeConfig.cs` - now stores list of `UIPanelDefinition` assets.
- `Assets/Scripts/DI/UIInstaller.cs` - bootstrap now registers panels from definitions.
- `Assets/Scripts/UI/Runtime/UIPanelRegistration.cs` - supports direct prefab in registration.
- `Assets/Scripts/UI/Runtime/UIPanelFactory.cs` - supports prefab-first instantiation and loader release only for loader-owned prefabs.
- `Assets/Scripts/UI/Runtime/Editor/UIRuntimeConfigEditor.cs` - inspector validation tooling.

## scene/inspector
- Create `UIPanelDefinition` asset per panel (`Create -> LostTime -> UI -> UI Panel Definition`).
- In each definition assign:
- panel script (editor field, auto-fills `PanelTypeName`)
- panel prefab (preferred) or asset path/key fallback.
- In `UIRuntimeConfig` assign definitions list.
- In `UIInstaller` assign `UIRuntimeConfig`.

## validation
- Static code-level validation complete.
- Runtime validation pending in Unity editor.

## decision log
- Chose panel-definition assets to remove fragile central list editing and improve content-team workflow.
- Kept fallback path/key support for loader backends while enabling direct prefab references for simpler scene setup.

## follow-ups
- Validate existing three migrated panels through new definition assets.
- Optionally add auto-create definitions tool for selected prefabs.

## commit
- hash: pending (awaiting review approval)
- message: pending
