# Stage 3 Full Main Loop Handoff

## Intent

Ship one stable pre-boss route in `Assets/Scenes/SampleScene.unity`:
mentor start -> seal route 1/2/3 with encounters and pickups -> arena unlock beat -> return-to-mentor completion.

This stage expands Stage 2 slice into one full MVP quest loop while intentionally deferring boss and ending.

## Architecture

- Keep quest orchestration in `Assets/Scripts/Quest/VerticalSliceQuestProgression.cs`.
- Keep player control in existing character FSM stack and `MainInput` (`CommunicationAction` drives interaction).
- Keep interaction targeting in `Assets/Scripts/Loot/Systems/InteractionController.cs` (`DirectionalRaycaster` + markables/interactables/takables).
- Keep pickups in loot seams (`ITakable`, `ICollectible`, `LootItem`, `ConsumablePickupItem`) with trigger/manual collection flow.
- Keep enemy variety in existing combat/enemy seams (`MeleeBeastEncounter`, ranged cultist, heavy golem prefabs/setups).

Key regression fix:
- `VerticalSliceQuestProgression` now resolves both new `_seals` config and legacy Stage 2 refs (`_sealTarget`, `_beastEncounter`, `_beastRoot`) at runtime, so old scene serialization does not break quest progression.

Why this fit:
- no parallel quest stack added.
- no custom one-off interaction system added.
- scene/asset tuning remains inspector-driven.

## Data

Use and tune:

1. Seal objectives in `VerticalSliceQuestProgression` (`_seals` array; expected 3 authored points).
2. Enemy assets/prefabs:
   - `Assets/Setups/Enemy/Stage3RangedCultistSetup.asset`
   - `Assets/Setups/Enemy/Stage3HeavyGolemSetup.asset`
   - `Assets/Prefabs/Enemy/Stage3RangedCultist.prefab`
   - `Assets/Prefabs/Enemy/Stage3HeavyGolem.prefab`
3. Pickup item assets:
   - `Assets/Setups/Items/ItemsConfiguration/HealItemSetup.asset`
   - `Assets/Setups/Items/ItemsConfiguration/ManaItemSetup.asset`
   - `Assets/Setups/Items/ItemsConfiguration/CoinItemSetup.asset`
4. Arena unlock target refs and delay (`_arenaUnlockTargets`, `_arenaUnlockDelay`).
5. Mentor interaction target ref for start + return completion (`_mentorTarget`).

## Signals

Emit / reuse:

- `VerticalSliceQuestProgression.SealRestored`
- `VerticalSliceQuestProgression.EncounterCleared`
- `VerticalSliceQuestProgression.ArenaUnlocked`
- `VerticalSliceQuestProgression.QuestStepAdvanced`
- `VerticalSliceQuestProgression.QuestCompleted`
- `InteractionController.PickupCollected` (added)
- `InteractionController.FocusHintChanged`
- `InteractionTarget.Interacted`

Listen:

- quest listens to mentor/seal `InteractionTarget.Interacted`.
- quest listens to encounter `IEncounterEnemy.Died`.
- interaction controller listens to raycast hits and routes to interactable/takable/collectible.
- HUD listens to objective/focus/vitals/mana updates.

## Scene/Inspector

1. Open `Assets/Scenes/SampleScene.unity`.
2. On quest object (`VerticalSliceQuestProgression`):
   - set `_mentorTarget`.
   - assign exactly 3 entries in `_seals` (`SealTarget`, encounter behavior/root, name).
   - set `_arenaUnlockTargets` and `_arenaUnlockDelay`.
3. On each seal object:
   - collider.
   - `InteractionTarget` with prompt/consume settings.
   - mark visual (`IMarkable`) if used for focus feedback.
4. On encounter roots:
   - wire encounter scripts implementing `IEncounterEnemy`.
   - ensure inactive-at-start behavior is owned by quest orchestration.
5. On pickup prefabs/instances:
   - `LootItem` with proper `ItemSetup`.
   - `ConsumablePickupItem` payload and `_collectOnTrigger` policy (heal/mana trigger, coin manual or trigger per design).
   - collider/trigger configured for chosen flow.
6. On player:
   - `Character` has `_interactionController` ref.
   - `MainInput` actions include communication/cast/aim as expected.
7. On HUD:
   - `VerticalSliceHudPresenter` refs for vitals, mana, quest progression, interaction controller, TMP labels.

## Validation

Static validation completed:

1. Verified quest orchestration covers full route:
   - `TalkToMentor -> RestoreSeals -> UnlockArena -> ReturnToMentor -> Completed`.
2. Verified seal restore is encounter-gated (`IsSealReadyForRestore`).
3. Verified arena unlock is blocked until all configured seals restored, then delayed unlock coroutine executes.
4. Verified mentor return step resets single-use interaction state before completion handoff.
5. Resolved serialization regression:
   - legacy scene refs now auto-resolve if `_seals` is empty.
6. Added pickup collection signal in interaction flow for route instrumentation and quest-side listeners.

Build/lint checks run:

- `ReadLints`: no diagnostics on touched gameplay files.
- `dotnet build LostTime.sln`: fails due unrelated assembly-generation drift (`Assets/Scripts/Loot/Systems/Character.cs` namespace resolution from stale generated project), not from touched Stage 3 logic.

Manual Play Mode route to run in Unity editor:

1. Talk to mentor -> objective changes to seals.
2. Clear each seal encounter and restore all 3 seals.
3. Confirm arena unlock only after 3/3 and delay beat.
4. Return to mentor -> quest completion event fires.
5. Confirm controls/hints remain responsive after each step.

## Decision Log

- chose: keep Stage 3 as orchestration layer over existing seams.
- chose: add backward-compatible seal config resolution instead of risky manual scene YAML edits.
- chose: add `PickupCollected` event at interaction seam for event-driven observability.
- avoided: introducing a new quest runtime framework or parallel interaction pipeline.
- why: keeps migration safe, preserves inspector workflow, and reduces coupling.

## Requirements (step-by-step)

1. Wire 3 seal objectives in `_seals` with encounter refs.
2. Keep enemy additions on existing encounter/combat seams.
3. Keep pickup content data-driven via `ItemSetup` and loot components.
4. Keep arena unlock dependent on aggregate seal completion + delay.
5. Keep final return-to-mentor completion interaction.
6. Validate one continuous intro->seals->unlock->return loop in Play Mode.
7. Record architecture/data/signals/scene/validation decisions in this handoff.

## Editor Configuration

1. Set `VerticalSliceQuestProgression._seals` size to 3 and assign refs.
2. Assign `VerticalSliceQuestProgression._arenaUnlockTargets` and delay.
3. Place/wire Stage 3 enemy prefabs or spawn roots near each seal route.
4. Assign heal/mana/coin `ItemSetup` assets to pickup objects.
5. Configure pickup colliders/triggers for intended collection mode.
6. Ensure mentor `InteractionTarget` is linked for both start and return completion.
7. Run full validation checklist in Play Mode.

## Unresolved Questions

- none
