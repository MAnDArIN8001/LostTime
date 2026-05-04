# Dialogue System Plan

## status
- Draft for review.
- `Assets/.codex/context/context.md` not found in repo. Source of truth fallback: current codebase.
- `Assets/.cursor/docs/WORKLOG_TEMPLATE.md` not found in repo. Worklog structure mirrored from existing worklogs.

## goals
- Add modular event-driven dialogue system.
- Reuse current interaction flow and highlight/prompt patterns.
- Freeze player during dialogue.
- Enter `Communication` sub-state and trigger animation key `Talking`.
- Move camera to dialogue pivot on start, restore on complete/break.
- Support mouse and gamepad for option selection.
- Support `Continue` and `Choice` steps.
- Persist dialogue choices globally.

## non-goals v1
- No parallel dialogues.
- No voiced lines.
- No portraits required.
- No generic condition language editor.
- No save migration tooling beyond stable ids.

## current project anchors
- Interaction contract exists: [IInteractable.cs](</C:/Unity/LostTime/Assets/Scripts/Loot/Systems/IInteractable.cs>).
- Reusable prompt/highlight pattern exists: [InteractionTarget.cs](</C:/Unity/LostTime/Assets/Scripts/Loot/Systems/InteractionTarget.cs>).
- Focus/interaction orchestration exists: [InteractionController.cs](</C:/Unity/LostTime/Assets/Scripts/Loot/Systems/InteractionController.cs>).
- Character input already has `CommunicationAction`: [Character.cs](</C:/Unity/LostTime/Assets/Scripts/Character/Character.cs>).
- FSM already reserves `StateType.Communication`: [StateType.cs](</C:/Unity/LostTime/Assets/Scripts/FSM/StateType.cs>).

## target architecture
- `Authoring`
  - ScriptableObject dialogue definitions.
  - Inspector-visible ids.
  - Camera pivot + prompt + highlight target on interactable zone.
- `Domain`
  - Immutable dialogue definitions consumed by runtime.
  - Condition contracts.
  - Session/history contracts.
- `Runtime`
  - Dialogue session lifecycle.
  - Step iteration.
  - Choice evaluation.
  - Events.
  - Integration with camera/FSM/input freeze/save.
- `Presentation`
  - MVP UI only.
  - Presenter maps runtime step to view model.
  - View emits input intents only.

## main runtime flow
1. Player raycasts dialogue zone through existing interaction system.
2. Prompt shown from dialogue interactable.
3. Player presses `CommunicationAction`.
4. Dialogue interactable requests dialogue start from orchestrator.
5. Orchestrator validates start conditions.
6. Orchestrator creates `DialogueSession`.
7. Orchestrator switches camera to serialized dialogue pivot.
8. Orchestrator enters character `Communication` sub-state.
9. Orchestrator opens dialogue UI through presenter.
10. Presenter renders current node.
11. Player uses mouse or gamepad to continue/select option.
12. Session records choice if step is `Choice`.
13. Session resolves next node.
14. On end node or no next node, session completes.
15. On cancel/forced close, session breaks.
16. Orchestrator closes UI, restores camera, exits communication state.

## core modules

### 1. DialogueInteractableZone
- Purpose: world entry point for dialogue.
- Shape: `MonoBehaviour, IInteractable`.
- Serialized fields:
  - `interactionPrompt`
  - `graphicsTarget`
  - `cameraPivot`
  - `dialogueDefinition`
  - `singleUse` optional
  - `consumeOnCompleteOnly` optional
- Responsibilities:
  - Expose prompt/highlight target.
  - Receive `Interact`.
  - Delegate start request to dialogue orchestrator.
- Notes:
  - Better separate than inheriting current `InteractionTarget`.
  - Keep world trigger thin. No session logic here.

### 2. DialogueDefinition authoring
- Root SO:
  - `dialogueId`
  - `displayName` optional
  - `startNodeId`
  - `nodes`
- Node:
  - `nodeId`
  - `speakerId` optional
  - `speakerName`
  - `text`
  - `nodeType`
  - `options`
  - `entryConditions`
  - `autoGenerateId` editor-only support
- Option:
  - `optionId`
  - `text`
  - `nextNodeId`
  - `conditions`
- Node types:
  - `Continue`
  - `Choice`
  - `End`
- Validation rules:
  - `dialogueId` required, stable.
  - `startNodeId` must exist.
  - Each `nodeId` unique inside dialogue.
  - Each `optionId` unique inside node.
  - `Continue` node: no required choice list, one continue action allowed.
  - `Choice` node: options count >= 1.
  - `End` node: no outbound transition required.
  - All `nextNodeId` must resolve or explicitly terminate.

### 3. Conditions
- Minimal v1 condition types:
  - `AlwaysTrue`
  - `PreviousChoiceIs`
  - `QuestCompleted`
- Evaluation target:
  - Node `entryConditions`
  - Option `conditions`
- Evaluator inputs:
  - Current dialogue id.
  - Current node id.
  - Session local history.
  - Global dialogue history.
  - Quest completion query service.
- Rules:
  - All conditions true => allowed.
  - Hidden option/node if false by default.
- Extension path:
  - Add `VisitedNode`, `DialogueCompleted`, `FlagEquals` later without schema rewrite.

### 4. DialogueSession
- Purpose: runtime owner of active dialogue state.
- Fields:
  - `DialogueDefinition`
  - `CurrentNodeId`
  - `Status`
  - `LocalChoiceHistory`
  - `StartedAt` optional
- Responsibilities:
  - Resolve first node.
  - Expose current node.
  - Handle continue.
  - Handle option selection.
  - Record local history.
  - Publish step/choice/completion events.
- Session statuses:
  - `Idle`
  - `Running`
  - `Completed`
  - `Broken`
- Completion rules:
  - `End` node confirmed => `Completed`
  - Cancel/force-close => `Broken`

### 5. DialogueOrchestrator
- Purpose: application service around session.
- Responsibilities:
  - Ensure one active dialogue at a time.
  - Start session from interactable.
  - Bind presenter/view.
  - Switch camera.
  - Enter/exit communication state.
  - Freeze/unfreeze player controls.
  - Persist global history on choice select.
  - Handle complete/break cleanup.
- Dependencies:
  - Camera controller adapter.
  - Character state adapter.
  - Dialogue presenter.
  - Dialogue history repository.
  - Quest state query service.

### 6. Dialogue UI MVP
- `DialogueView`
  - Modal root.
  - Speaker label.
  - Body text.
  - Continue button or options container.
  - Gamepad focus anchor.
  - Emits:
    - `ContinueRequested`
    - `OptionRequested(index or optionId)`
    - `CancelRequested`
- `DialoguePresenter`
  - Subscribes to session.
  - Builds view model.
  - Controls visible controls by node type.
  - Applies initial selected option for gamepad.
  - Forwards UI intents to orchestrator/session.
- `DialogueViewModel`
  - `speakerName`
  - `text`
  - `nodeType`
  - `options`
  - `isCancelable`

### 7. Global dialogue history
- Store globally, not only per session.
- Proposed shape:

```json
{
  "dialogues": [
    {
      "dialogueId": "mentor_intro",
      "entries": [
        {
          "nodeId": "node_010",
          "choiceId": "choice_010",
          "choiceVariantId": "option_b"
        }
      ]
    }
  ]
}
```

- Why this shape:
  - stable dialogue grouping
  - preserves node context
  - supports repeated runs later
  - makes conditions easier than flat list
- Persist timing:
  - On each successful option selection.
  - `Break` does not mark dialogue completed.
- Optional separate completion log:
  - `completedDialogueIds`

## event contract
- `OnDialogueStarted(DialogueStartedEvent)`
- `OnDialogueStepShown(DialogueStepShownEvent)`
- `OnDialogueOptionSelected(DialogueOptionSelectedEvent)`
- `OnDialogueStepCompleted(DialogueStepCompletedEvent)`
- `OnDialogueCompleted(DialogueCompletedEvent)`
- `OnDialogueBreak(DialogueBreakEvent)`

## event semantics
- `Started`
  - fired after session created and first node resolved
- `StepShown`
  - fired when presenter/runtime moves to a node ready for UI
- `OptionSelected`
  - fired immediately after valid option picked
- `StepCompleted`
  - fired after:
    - continue confirmed on `Continue`
    - option picked on `Choice`
    - final acknowledgement on `End`
- `Completed`
  - fired on successful natural end only
- `Break`
  - fired on cancel/forced close only

## FSM and animation integration
- Add dedicated communication runtime path, not UI-driven state toggles.
- Character enters `Communication` sub-state on dialogue start.
- Character exits `Communication` sub-state on complete/break.
- Animation key/config entry to reserve now:
  - `Talking`
- v1 requirement:
  - state freezes move/aim/cast/interact
  - animation trigger/path wired, actual clip can come later

## camera integration
- Each dialogue zone exposes `cameraPivot`.
- Start:
  - cache current/default camera state
  - move to pivot using camera adapter
- End/break:
  - restore cached/default state
- Keep camera logic outside presenter and session.

## input integration
- World start uses existing `CommunicationAction`.
- UI runtime input:
  - Mouse click on buttons.
  - Gamepad/keyboard navigate options.
  - Separate submit action for current focused option.
  - Cancel action mapped to break.
- During active dialogue:
  - world interaction input ignored
  - move/aim/cast blocked

## save/load integration
- Need `IDialogueHistoryRepository`.
- Need `IQuestStateQuery`.
- Persist:
  - per-choice global entries
  - optional completed dialogues list
- Load:
  - available before first dialogue session
- Important:
  - ids must remain stable after content edits

## validation/editor tooling
- Custom validation strongly recommended.
- Checks:
  - duplicate ids
  - missing next node
  - impossible start node
  - `Choice` node with zero options
  - invalid condition targets
  - orphan nodes warning
- Nice-to-have later:
  - auto id generation button
  - graph preview
  - condition summary in inspector

## implementation phases

### Phase 1. Domain + authoring
- Create SO schema.
- Create ids + validation path.
- Create condition contracts.
- Create runtime-friendly read model if needed.

### Phase 2. Runtime session
- Create session state machine.
- Implement node iteration.
- Implement continue/choice handling.
- Emit lifecycle events.

### Phase 3. World integration
- Create `DialogueInteractableZone`.
- Connect to current interaction system.
- Start orchestrator from interact.

### Phase 4. Orchestration
- Add camera adapter.
- Add communication state adapter.
- Add player freeze/unfreeze.
- Add cleanup on complete/break.

### Phase 5. UI MVP
- Build modal view.
- Build presenter.
- Add gamepad focus/select flow.
- Add cancel handling.

### Phase 6. Persistence + quest conditions
- Add history repository.
- Add quest completion condition.
- Add restore/load path.

### Phase 7. Validation + polish
- Add authoring validation.
- Add logs/debug aids.
- Add UX polish for default focus and transitions.

## testing checklist
- Start dialogue from raycasted zone.
- Prompt visible and correct.
- Highlight target uses dialogue zone graphics target.
- Camera moves to pivot on start.
- Character enters `Communication`.
- Movement/aim/cast/interact blocked during dialogue.
- `Continue` node advances correctly.
- `Choice` node shows one or many options.
- Mouse selection works.
- Gamepad selection works.
- Submit picks focused option.
- Cancel triggers `OnDialogueBreak`.
- Natural end triggers `OnDialogueCompleted`.
- Camera restores on complete.
- Camera restores on break.
- Global choice history persists.
- `QuestCompleted` condition filters branch correctly.
- Invalid asset config reports useful validation errors.

## scene/inspector impact
- New dialogue interactable component on dialogue-capable entities.
- Assign:
  - interaction prompt
  - graphics target
  - camera pivot
  - dialogue definition asset
- Character/FSM setup:
  - wire communication sub-state
  - reserve animation config key `Talking`
- UI scene/setup:
  - add dialogue modal view
  - configure selectable buttons for gamepad navigation
- Service setup:
  - bind orchestrator
  - bind history repository
  - bind quest query adapter

## risks
- Missing project context doc can hide local architecture rule not visible in code.
- If ids regenerate after content edits, save/history branches break.
- If communication state is bolted onto UI instead of orchestrator, cleanup bugs likely.
- If gamepad focus is not owned centrally, option navigation will be flaky.
- If conditions live only on nodes and not options, branching will become rigid fast.

## deliverables after approval
- Detailed class/interface blueprint.
- Implementation `.json` plan for agent.

## unresolved questions
- Completed dialogue restart policy: repeatable or single-use by config?
- `Break` from external force-close only log event, or also log partial history marker?
- Hidden unavailable options vs disabled unavailable options: final UX choice?
- Need separate `speakerId` now, or `speakerName` enough for v1?
