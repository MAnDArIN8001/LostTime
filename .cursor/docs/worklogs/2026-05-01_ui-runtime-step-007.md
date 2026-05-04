# UI Runtime Step 007

## summary
- Integrated UI service lifecycle with gameplay input gate abstraction.
- Added idempotent gameplay input block/restore flow.
- Wired `Esc`/`Gamepad Start` close-all shortcut through `IUIService` entrypoint.

## files changed
- `Assets/Scripts/UI/Runtime/IUIService.cs` - added `HandleCloseAllShortcut` entrypoint.
- `Assets/Scripts/UI/Runtime/UIService.cs` - integrated `IUIInputGate` and input restore safety.
- `Assets/Scripts/UI/Runtime/IUIInputGate.cs` - gameplay input gate contract.
- `Assets/Scripts/UI/Runtime/MainInputUIGameplayGate.cs` - concrete gate for `MainInput` + close-all shortcut action.
- `Assets/Scripts/UI/Runtime/UIShortcutCloseAllBridge.cs` - bridge from gate shortcut event to UI service.

## scene/inspector
- No scene changes.
- No inspector changes required in this step.

## validation
- Static terminal-level validation only.
- Runtime validation pending in Unity play mode.

## decision log
- Kept close-all shortcut outside `MainInput.Character` map to avoid deadlock when gameplay map is disabled.
- Service owns open-panel based gate state and restore path (`Dispose`, `CloseAll`) to avoid stuck blocked input states.
- Shortcut handling routed via explicit `IUIService.HandleCloseAllShortcut` entrypoint.

## follow-ups
- Bind gate/service/bridge in Zenject installer step.
- Verify Escape close-all and input restore in mixed modal/non-modal panel flow.

## commit
- hash: pending (awaiting review approval)
- message: pending
