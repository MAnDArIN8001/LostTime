# Quest System Architecture

## Purpose

This document is the technical reference for the new quest system.

It describes the runtime shape of the current implementation in:

- `Assets/.aiagents/quest-system-tutorial-plan.json`
- `Assets/.cursor/docs/quest-setup-guide.md`
- `Assets/.cursor/docs/WORKLOG_TEMPLATE.md`

The system is implemented as a data-driven, event-driven quest progression layer.
It is not a hardcoded storyline controller.

## System Shape

The quest system is a list of ordered steps.

Each step waits for a specific event, applies configurable filters, and advances progress when the matching event arrives.

Current implementation classes:

- `Quest.Runtime.EventBusQuestRunner`
- `Quest.Runtime.QuestSession`
- `Quest.Authoring.QuestDefinitionAuthoring`
- `Quest.Tutorial.TutorialQuestDefinitionAuthoring`
- `Quest.Integration.PushQuestEventPublisher`
- `Quest.Integration.PullQuestEventPublisher`
- `Quest.Integration.CharacterSpellCastQuestEventPublisher`
- `Quest.Integration.SpellHitQuestEventPublisher`
- `UI.QuestEventBusPresenter`

Core properties of the model:

- steps are sequential by default
- each step can require a configurable count
- each step can filter by source, target, payload, context, or any future metadata
- the same runtime can support push, pull, spell cast, target hit, and future event types
- the UI reads service events, not hidden runtime state

## EventBus Wiring

Scene-level communication must go through the existing `Utils.Events.EventBus` and `SceneEventBusProvider`.

Current flow:

1. `SceneEventBusProvider` resolves the shared bus for the scene.
2. Gameplay adapters publish `Quest.Core.QuestEventData` to that bus.
3. `QuestSession` subscribes to `QuestEventData` on the same bus.
4. `QuestSession` publishes `QuestStateChangedEvent`, `QuestStepProgressChangedEvent`, and `QuestCompletedEvent`.
5. `QuestEventBusPresenter` subscribes only to those service events.

This keeps the system decoupled:

- gameplay does not know quest internals
- quest runtime does not depend on specific MonoBehaviours
- UI does not read quest state directly

## Runtime Flow

The current runtime loop is:

1. Scene bootstrap resolves the bus, quest definition, runtime service, and UI presenter.
2. Runtime loads the current quest definition.
3. Runtime activates the first step.
4. Gameplay publishes a matching event.
5. Runtime checks the event against the active step filters.
6. Runtime increments the active step progress if the event matches.
7. When the required count is reached, runtime publishes step-completed service info.
8. Runtime advances to the next step.
9. After the final step completes, runtime publishes quest-completed service info.

Important behavior:

- non-matching events are ignored
- repeated matching events keep incrementing until the threshold is reached
- progress should be stable under noisy gameplay event streams
- the same runtime should support any number of quest definitions

## Step Model

Each quest step should remain fully configurable.

Recommended fields:

- `stepId`
- `title`
- `description`
- `expectedEventType`
- `requiredCount`
- `currentCount`
- `sourceFilter`
- `targetFilter`
- `payloadFilter`
- `contextFilter`
- `displayTemplate`
- `visibleInHud`
- `completionBehavior`

Design rule:

- the runtime consumes the step as data
- the UI renders from service payloads and templates
- gameplay adapters only emit domain events

## Tutorial Quest Shape

The first tutorial quest is a four-step learning chain:

1. push an object 3 times
2. pull an object 3 times
3. cast a spell 3 times
4. hit a target 3 times

This should be authored as data, not hardcoded progression logic.

Each tutorial step should expose:

- configurable displayed text
- configurable required count
- configurable event type
- configurable matching filters
- optional payload metadata for UI text

Suggested UI text examples:

- `Push objects: 1/3`
- `Pull objects: 2/3`
- `Cast spells: 3/3`
- `Hit targets: 1/3`

## Authoring Model

Quest authoring is currently done through serialized authoring components.

The authoring layer should allow designers to:

- create quest definitions with `QuestDefinitionAuthoring`
- reorder steps
- edit counts and labels
- bind event types
- configure filters
- mark tutorial quests through `TutorialQuestDefinitionAuthoring`

Validation rules should catch:

- empty event ids
- zero or negative counts
- broken step references
- missing display templates where the UI expects one
- invalid tutorial setup references

## UI Hookup

The quest UI should be a dedicated presenter component.

Responsibilities:

- subscribe to quest service events through `EventBus`
- build the current HUD text from payload data
- show active objective text and progress
- react to step transitions and quest completion
- hide itself when no quest is active, if required by the scene

The UI must not:

- poll the runtime directly
- depend on a specific gameplay object
- hardcode tutorial text

The UI should work from service payloads so the same presenter can display any quest definition.

## Extension Points

The architecture should support later expansion without rewriting the core loop.

Planned extension points:

- new event types
- new matching filters
- optional multi-condition steps
- localized display templates
- optional quest chains beyond the tutorial
- alternate HUD presentations

## Scene Responsibilities

The scene should provide only wiring, not quest logic.

Scene bootstrap is responsible for:

- resolving the shared event bus
- instantiating or referencing the quest runtime service
- attaching gameplay adapters
- attaching the UI presenter
- binding the tutorial definition

The scene should not contain quest-specific branching logic outside the bootstrap layer.
