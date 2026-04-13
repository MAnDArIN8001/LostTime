# Quest System Docs

## Summary

Updated the documentation layer for the new quest system. Added a technical architecture reference and rewrote the setup guide so it matches the current event-driven quest flow and the implemented runtime classes.

## Context

- task goal: document the new quest system in `.cursor/docs`
- current architecture direction: ordered quest steps, event-driven progression, UI driven by service payloads
- constraint: documentation only, no code changes

## Files Changed

- `.cursor/docs/quest-system-architecture.md`
- `.cursor/docs/quest-setup-guide.md`
- `.cursor/docs/worklogs/2026-04-13_quest-system-docs.md`

## Scene/Inspector

1. none

## Validation

1. reviewed the current docs in `.cursor/docs`
2. reviewed `Assets/.aiagents/quest-system-tutorial-plan.json`
3. did not run Unity because this task only changed documentation

## Decision Log

- chose: split the content into an architecture doc and a setup guide
- avoided: leaving the old combat-first guide in place as the main reference
- why: the new quest flow needs a direct current-source doc set, not a legacy note

- chose: keep the docs aligned around `EventBus` and `SceneEventBusProvider`
- avoided: describing a separate quest-specific transport layer
- why: the plan already standardizes scene communication on the existing bus

## Follow-Ups

- recheck docs if runtime contracts change again during scene wiring
- add a short troubleshooting note if the scene wiring grows more complex

## Commit

- prefix used: `update`
- subject: `update: document quest system architecture and setup`
- status: not created
