# Implementation Map

## Goal

Map MVP mechanics onto current project seams so later tasks plug into existing movement, FSM, input, and loot code instead of adding parallel systems.

## Existing seams

- `Character.Character` is current composition root for camera look, movement update loop, and movement FSM.
- `Character.Modules.Movement.MovementModule` + `CharacterMovementModule` already solve locomotion execution through `CharacterController`.
- `FSM.StateMachine` + `HFSM.HierarchicalState` already solve root-state and child-state transitions.
- `MainInput` + `DI.InputInstaller` already solve shared input creation, enable, and injection.
- `Loot.Systems.ITakable` / `IMarkable` + `LootableItem` / `MarkableItem` are a partial base for manual pickups and highlights, but they need rework before becoming the main interaction standard.
- `Utils.Physics.Raycaster` + `Utils.Filters.RaycastFilter` already solve forward hit detection and filtered interaction targeting.
- `Loot.Data.ItemSetup` + `ItemsDatabase` + `Loot.Inventory.InventoryService` already solve tunable item data and runtime counts.

## Mechanic map

### Movement

- `walk / run / camera / target facing`
- keep in current character stack:
- `Character.Character` reads camera input and updates root FSM
- `CharacterMovementState` reads `MainInput.Character.Movement`
- `CharacterRunState` toggles run animation
- `CharacterSetup` stays source of walk/run speeds
- `MovementModule` variants stay source of actual displacement

Implementation note:
- extend movement by adding child states under `StateType.Movement`, not by bypassing the FSM with direct `MonoBehaviour` logic

### Combat

- `magic projectile / mana cost / cooldown / hit reaction`
- use FSM as action gate:
- `StateType.Attack` already exists for cast execution
- `StateType.Aim` can hold pre-cast or lock-on behaviour if needed
- `StateType.Attacking` can become parent scope if combat grows beyond one cast state
- keep cast enter/exit animation writes behind `IAnimationFacade`
- keep spell stats in ScriptableObjects under `Assets/Setups`

Input fit:
- add new actions in `MainInput.inputactions` for cast / aim / target
- consume those actions inside new combat states, same pattern as movement states

Loot fit:
- coins / keys can stay `ItemSetup`-driven manual pickups
- mana orb / heal flask can use trigger-based pickup flow instead of the current looting interaction flow
- manual pickup objects can implement `ITakable`
- highlight should stay optional through `IMarkable`, not mandatory for all pickups
- pickup result should update `InventoryService` for stored items, or directly apply effect through a consumable service for instant pickups

### Interaction

- `NPC talk / pickup / activate seal`
- use one forward interaction flow:
- ray source: `DirectionalRaycaster`
- target filtering: `RaycastFilter`
- highlight: `IMarkable`
- action trigger: `MainInput.Character.CommunicationAction`

Mapping by target:
- NPC talk: same interaction ray + mark flow, then transition into `StateType.Communication`
- manual pickup: same ray + mark flow, then call `ITakable.Take()`
- trigger pickup: collide with player trigger, apply effect, then destroy object
- seal activate: same ray + mark flow, then dispatch seal-specific logic from an activator component

Implementation note:
- current loot contracts are not enough to cover all interaction cases cleanly
- seals and NPCs do not fit `ITakable`; they should reuse `IMarkable` + raycast targeting, then expose their own interaction interface or handler
- consumables like mana and heal should not be forced through the same manual loot path if trigger pickup is simpler

### Quest flow

- `mentor intro / 3 seals / boss unlock / return to mentor`
- quest progression should sit above movement/FSM/input/loot, not inside them
- use existing systems as triggers:
- mentor interaction via `CommunicationAction`
- seal completion via interaction handler
- key or reward pickup via `ITakable` or trigger collector, depending on pickup type
- movement/combat states remain execution layer only

### Consumables

- `mana orb / heal flask / coins`
- author each as `ItemSetup`
- register persistent items in `ItemsDatabase`
- `coins`: manual or auto pickup, update `InventoryService`
- `mana orb` / `heal flask`: prefer trigger pickup with immediate effect apply
- use separate consumable effect logic for `heal` and `mana`; inventory stays storage, not effect logic

## Suggested architecture by mechanic

1. `Movement`: reuse as-is; only add transitions if combat or interaction must temporarily lock locomotion.
2. `Combat`: add new states first, then input actions, then spell data assets.
3. `Interaction`: build one interaction controller over raycaster + markable + communication input.
4. `Quest`: build quest state/progression as orchestration layer listening to interactions and pickups.
5. `Loot`: keep world items data-driven through `ItemSetup`; support both manual loot and trigger consumables.

## Integration order

1. Add combat input actions in `MainInput.inputactions`.
2. Add combat states around `StateType.Aim` / `Attack`.
3. Add shared interaction controller using raycast + filter + `IMarkable`.
4. Split pickups into manual interaction pickups and trigger consumables.
5. Hook stored pickups through `ITakable` into `InventoryService`.
6. Add quest progression that reacts to mentor talk, seal activation, boss clear, and return talk.

## Requirements

1. Keep character movement inside `MovementModule` + movement FSM.
2. Put new player actions behind `MainInput`, not raw Unity input calls.
3. Use `StateType` and FSM states for cast, aim, looting, and communication gating.
4. Reuse `ITakable` only for manual pickups; allow trigger-based consumables for mana and heal.
5. Keep item content in `ItemSetup` / `ItemsDatabase`; keep runtime counts in `InventoryService`.
6. Inspector work later:
7. assign `MainInput.inputactions` bindings for combat actions
8. add raycaster component on player interaction root
9. assign mark visuals on NPC, seal, and manual pickup prefabs
10. create `ItemSetup` assets for mana, heal, coins, keys if used
11. add trigger colliders on mana/heal pickup prefabs if using auto-collect
12. extend character prefab references only through existing serialized module slots or dedicated new components

## Unresolved questions

- should `Attack` live under global root or inside a new `Attacking` parent state
- should seal / NPC interaction share one new `IInteractable` contract, or stay handler-specific
- should coins also become auto-pickup, or stay manual for stronger interaction feedback
