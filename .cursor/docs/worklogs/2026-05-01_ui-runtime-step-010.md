# UI Runtime Step 010

## summary
- Added editor test assembly for UI runtime unit tests.
- Added `UIService` tests for stack order, close-all, keepalive reuse, release flow, and dispose cleanup.
- Implemented test doubles for panel factory and panels to isolate service behavior.

## files changed
- `Assets/Tests/Editor/UIRuntime/UIRuntime.Editor.Tests.asmdef` - editor test assembly definition.
- `Assets/Tests/Editor/UIRuntime/UIServiceTests.cs` - unit tests for `UIService` core invariants.

## scene/inspector
- No scene changes.
- No inspector changes required.

## validation
- Tests added in Unity editor test assembly; run validation pending in Unity Test Runner.

## decision log
- Chose service-level unit tests with fakes to avoid scene dependencies and reduce flakiness.
- Covered required invariants from step plan: stack ordering, close-all, keepalive reuse, release flow, dispose cleanup.

## follow-ups
- Run tests in Unity Test Runner.
- Add integration tests after installer wiring is finalized in active scene.

## commit
- hash: pending (awaiting review approval)
- message: pending
