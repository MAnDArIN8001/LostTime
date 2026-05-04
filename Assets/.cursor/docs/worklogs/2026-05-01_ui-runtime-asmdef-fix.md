# UI Runtime Asmdef Fix

## summary
- Introduced dedicated `UI.Runtime` assembly definition for runtime UI module.
- Rewired editor tests assembly to reference `UI.Runtime` instead of `Assembly-CSharp`.
- Moved bridge scripts dependent on `MainInput` out of runtime assembly scope to avoid circular dependency with predefined assembly.
- Rolled back asmdef approach by team decision to keep project complexity lower at current stage.

## files changed
- `Assets/Scripts/UI/Runtime/UI.Runtime.asmdef` - new runtime assembly definition.
- `Assets/Tests/Editor/UIRuntime/UIRuntime.Editor.Tests.asmdef` - reference updated to `UI.Runtime`.
- `Assets/Scripts/UI/RuntimeBridge/MainInputUIGameplayGate.cs` - moved from runtime folder.
- `Assets/Scripts/UI/RuntimeBridge/UIShortcutCloseAllBridge.cs` - moved from runtime folder.
- rollback: removed both asmdef files and moved bridge scripts back to `Assets/Scripts/UI/Runtime`.

## scene/inspector
- No scene changes.
- No inspector changes required.

## validation
- Static fix applied to resolve missing namespace/type references in test assembly compile.
- Unity reimport/compile validation pending in editor.

## decision log
- Avoided introducing hard dependency from `UI.Runtime` asmdef to `Assembly-CSharp` by relocating `MainInput`-dependent bridge classes outside asmdef folder.
- Final decision: avoid asmdef setup for now in non-production scope to reduce maintenance overhead.

## follow-ups
- Let Unity regenerate `.meta`/assembly import state and verify no compile errors.
- Run EditMode tests from `UIRuntime.Editor.Tests`.

## commit
- hash: pending (awaiting review approval)
- message: pending
