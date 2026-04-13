# Quest System Setup Guide

## Purpose

This guide explains how to wire the new quest system in Unity.

It covers:

- `EventBus` wiring
- runtime flow
- authoring
- tutorial setup
- UI hookup

This replaces the old combat-first quest guide.

## What The System Does

The quest system is an ordered list of steps.

Each step waits for a gameplay event, tracks progress, and advances when the configured count is reached.

For the first tutorial quest, the expected sequence is:

1. push an object 3 times
2. pull an object 3 times
3. cast a spell 3 times
4. hit a target 3 times

## Before You Start

Make sure the scene has:

- the shared `EventBus`
- a `SceneEventBusProvider`
- quest runtime bootstrap or installer
- gameplay adapters for the events you want to count
- the quest UI presenter

If any of those pieces are missing, the quest can exist in data but never receive progress events.

## Step 1. Create The Quest Definition

Create a serialized authoring component.

Current components:

- `Quest.Authoring.QuestDefinitionAuthoring` for generic ordered step lists
- `Quest.Tutorial.TutorialQuestDefinitionAuthoring` for the first tutorial flow

For each step, configure:

- step title
- user-facing text
- expected event type
- required count
- filters for source or target
- optional template or payload overrides

For the tutorial quest, create four steps with required count set to `3`.

## Step 2. Wire Gameplay Events Into EventBus

Gameplay must publish quest-relevant events to the shared bus.

The quest runtime should not listen to specific scene objects directly.

Use adapters for:

- `PushQuestEventPublisher`
- `PullQuestEventPublisher`
- `CharacterSpellCastQuestEventPublisher`
- `SpellHitQuestEventPublisher`

Keep those adapters thin:

- detect the gameplay action
- build a quest event payload
- publish it to `EventBus`

Do not put quest progress rules inside the gameplay mechanics themselves.

## Step 3. Connect The Runtime

The quest runtime is `Quest.Runtime.EventBusQuestRunner`.

It should:

- read the active quest definition
- subscribe to quest gameplay events
- track the active step only
- publish service events when progress changes
- advance to the next step automatically when the count is met

The runtime flow should be strictly sequential unless a future quest definition says otherwise.

## Step 4. Hook Up Tutorial Content

Author the tutorial as data, not as hardcoded progression logic.

Recommended tutorial setup:

- step 1: push x3
- step 2: pull x3
- step 3: spell cast x3
- step 4: target hit x3

For each step, make sure the following are editable in the asset:

- text shown in the UI
- required count
- event type
- filter values
- completion behavior

If you need to tune the tutorial later, update the asset instead of editing runtime code.

## Step 5. Hook Up The UI

Add `UI.QuestEventBusPresenter` to the HUD.

The presenter should:

- subscribe to service events from the quest system
- build the text from the payload
- show `current / required` progress
- switch text when the active step changes
- hide or clear itself when the quest is finished, depending on scene needs

Example output:

- `Push objects: 1/3`
- `Pull objects: 2/3`
- `Cast spells: 3/3`
- `Hit targets: 1/3`

The presenter should not inspect runtime internals directly.

## Step 6. Bootstrap The Scene

Scene bootstrap should connect the pieces in this order:

1. resolve `SceneEventBusProvider`
2. resolve or create `Quest.Runtime.EventBusQuestRunner`
3. bind the quest definition
4. register gameplay adapters
5. register the UI presenter
6. start the tutorial quest

Avoid hidden singleton-style wiring.
The goal is explicit scene setup that can be debugged from the inspector.

## Validation Checklist

Test the scene with the following checks:

1. pushing an object increases only the push step
2. pulling an object increases only the pull step
3. spell casts do not affect target-hit progress
4. target hits do not affect spell-cast progress
5. the UI shows the active step and current count
6. the quest advances when the required count is reached
7. the UI updates on step transition
8. the final step completes the quest

## Common Mistakes

- publishing quest logic from gameplay objects instead of adapters
- reading quest runtime state directly from UI
- hardcoding tutorial text in the presenter
- using separate buses for gameplay and UI
- making the quest depend on one specific scene object
- forgetting to expose counts and filters as data

## If You Add More Quests Later

Keep using the same model:

- define ordered steps in data
- publish gameplay events through `EventBus`
- let the runtime match and count events
- let the UI render service payloads

That keeps new quests cheap to author and easy to debug.
