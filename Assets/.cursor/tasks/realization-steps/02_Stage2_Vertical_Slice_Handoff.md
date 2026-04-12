# Stage 2 Vertical Slice Handoff

> Legacy note:
> This handoff may contain combat-first assumptions from the older vertical slice.
> Read `Assets/.cursor/context/context.md` first.
> If any section conflicts with the current courtyard `push / pull / press` direction, the context file wins.

## Intent

Ship one short playable route in `Assets/Scenes/SampleScene.unity`:
mentor talk with minimal dialog UI -> explicit aim + cast spell -> defeat one melee beast activated after mentor talk -> restore one seal -> keep HUD readable through the full route.

This stage proves the core MVP fantasy using the smallest end-to-end playable path, not isolated feature demos.

## Architecture

- Reuse existing player orchestration in `Assets/Scripts/Loot/Systems/Character.cs`: keep cast gated by FSM state, keep interaction on `CommunicationAction` through `_interactionController.TryInteract(...)`.
- Keep combat implementation in `Assets/Scripts/Combat/CharacterSpellCaster.cs`: mana cost, cooldown gating, projectile spawn, and combat signals (`SpellCast`, `ManaSpent`).
- Keep interaction resolution in `Assets/Scripts/Loot/Systems/InteractionController.cs` with shared targeting path (`DirectionalRaycaster` + markable/interactable contracts).
- Keep quest progression in `Assets/Scripts/Quest/VerticalSliceQuestProgression.cs` with fixed sequence `TalkToMentor -> DefeatBeast -> RestoreSeal -> Completed`.
- Keep HUD projection in `Assets/Scripts/UI/VerticalSliceHudPresenter.cs`, binding vitals, mana, objective text, and interaction hint.

Why this fit:
- Core seams already exist in repo.
- Reduces coupling by avoiding new parallel systems for combat/dialog/interaction.
- Preserves data-driven workflow through scene refs and `ScriptableObject` tuning.

## Data

Create and tune:

1. Spell setup asset from `Assets/Scripts/Combat/Data/ProjectileSpellSetup.cs`.
2. Melee beast setup asset from `Assets/Scripts/Enemy/Data/MeleeBeastSetup.cs`.
3. Projectile prefab with trigger collider + `SpellProjectile`.
4. Beast prefab or scene object with collider + active `IDamageable` hit path.
5. Minimal mentor dialog text source + dialog UI refs.
6. Mentor and seal scene objects with `InteractionTarget` + mark visuals.
7. HUD canvas refs for HP, MP, objective, and interaction hint.

## Signals

Emit / reuse:

- `CharacterSpellCaster.SpellCast`
- `CharacterSpellCaster.ManaSpent`
- `MeleeBeastEncounter.Died`
- `InteractionTarget.Interacted`
- `VerticalSliceQuestProgression.ObjectiveChanged`
- `VerticalSliceQuestProgression.Completed`
- `InteractionController.FocusHintChanged`
- mentor dialog open/close signal (or serialized UnityEvent hook) used to trigger beast activation after mentor handoff

Listen:

- cast input via `MainInput` action map (including explicit aim + cast)
- interaction input via `MainInput.Character.CommunicationAction`
- quest object listens to mentor/beast/seal progression events
- HUD listens to vitals/mana/quest/focus-hint and optional dialog visibility state

## Scene Wiring

Required in `Assets/Scenes/SampleScene.unity`:

1. Player:
   - `Character` wired with `_interactionController`.
   - `CharacterSpellCaster` wired with spell setup, mana, and cast origin.
   - explicit aim + cast bindings active through `MainInput`.
2. Interaction root:
   - `InteractionController` wired with `DirectionalRaycaster` and proper layer mask/filter setup.
3. Mentor:
   - collider + `InteractionTarget` + markable visual + minimal dialog presenter refs.
   - mentor interaction completion path advances quest and activates/spawns beast.
4. Beast:
   - starts disabled or absent at scene start.
   - activates/spawns only after mentor completion.
   - has collider/trigger and `MeleeBeastEncounter` wired to setup asset.
5. Seal:
   - collider + `InteractionTarget` + markable visual; single-use completion behavior.
6. Quest:
   - `VerticalSliceQuestProgression` references wired to mentor target, beast encounter, and seal target in this exact order.
7. HUD:
   - `VerticalSliceHudPresenter` wired to TMP labels and data refs for HP/MP/objective/hint.
   - dialog UI visibility remains readable during mentor handoff.

## Validation

Play Mode checklist:

1. Start `SampleScene`; objective shows "Talk to the mentor".
2. Look at mentor; mark + interaction hint appear.
3. Interact with mentor; minimal dialog opens, closes cleanly, beast activates/spawns, objective advances to beast step.
4. Use explicit aim, then cast spell; mana decreases and cooldown prevents spam.
5. Projectile hits beast; beast dies; objective advances to seal step.
6. Look at seal; mark + interaction hint appear; interact completes route.
7. HUD remains readable for HP, MP, objective, hint, and mentor dialog handoff through full route.

Failure checks:

- wrong layers or missing colliders/triggers prevent interaction/combat hits
- missing serialized refs break quest or HUD updates
- quest resets unexpectedly on re-enable
- beast does not activate after mentor completion
- aim gate blocks movement/state exit incorrectly

## Requirements

1. Keep combat input behind `MainInput`; no direct/raw polling path.
2. Keep explicit aim and cast gated by `Character` FSM state; no free-fire `Update` loop.
3. Keep spell/enemy tuning in `ScriptableObject` assets or serialized refs.
4. Keep mentor and seal on shared interaction path (raycast + markable + `InteractionTarget`).
5. Keep mentor dialog minimal (single short handoff path, no branching system).
6. Activate or spawn beast only after mentor interaction completion.
7. Deliver one playable in-scene route, not scripts only.
8. End with documented editor setup + unresolved questions.

## Editor Setup

1. Open `Assets/Scenes/SampleScene.unity`.
2. Add/bind explicit aim and cast actions in `Assets/Scripts/Input/MainInput.inputactions`.
3. Regenerate `Assets/Scripts/Input/MainInput.cs` if bindings changed.
4. On player, assign `CharacterSpellCaster`, spell setup asset, cast origin, mana, and vitals refs.
5. On interaction root, assign `DirectionalRaycaster`, hit layers, and `InteractionController` refs.
6. On mentor and seal objects, add colliders, `InteractionTarget`, and mark visuals.
7. Wire mentor dialog UI refs and short mentor text content.
8. Ensure projectile/beast colliders and trigger setup reliably route `SpellProjectile` hits into `IDamageable`.
9. Wire beast disabled-start or spawn reference so mentor completion activates encounter.
10. Wire `VerticalSliceQuestProgression` refs and `VerticalSliceHudPresenter` TMP refs in scene canvas.
11. Run validation checklist end-to-end in Play Mode.

## Unresolved Questions

- none
