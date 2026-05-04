# Implementation Map

## Goal

Map the current MVP mechanics onto existing project seams so later tasks plug into movement, FSM, input, interaction, and data-driven workflows instead of adding parallel systems.

## Existing seams

- `Character.Character` is current composition root for camera look, movement update loop, and movement FSM.
- `Character.Modules.Movement.MovementModule` + `CharacterMovementModule` already solve locomotion execution through `CharacterController`.
- `FSM.StateMachine` + `HFSM.HierarchicalState` already solve root-state and child-state transitions.
- `MainInput` + `DI.InputInstaller` already solve shared input creation, enable, and injection.
- `Loot.Systems.ITakable` / `IMarkable` + `LootableItem` / `MarkableItem` are a partial base for highlighted world targets and movable or collectable objects.
- `Utils.Physics.Raycaster` + `Utils.Filters.RaycastFilter` already solve forward hit detection and filtered interaction targeting.
- `Loot.Data.ItemSetup` + `ItemsDatabase` + `Loot.Inventory.InventoryService` already solve tunable item data and runtime counts.
- `Combat.CharacterSpellCaster` + `Combat.SpellProjectile` already solve one possible route for ranged magical interaction if that supports environmental mechanics.

## Mechanic map

### Movement

- `walk / run / camera / target facing / interaction positioning`
- keep in current character stack:
- `Character.Character` reads camera input and updates root FSM
- `CharacterMovementState` reads `MainInput.Character.Movement`
- `CharacterRunState` toggles run animation
- `CharacterSetup` stays source of walk/run speeds
- `MovementModule` variants stay source of actual displacement

Implementation note:
- extend movement by adding compatible gating states or interaction locks, not by bypassing the FSM with ad-hoc `MonoBehaviour` booleans

### Interaction

- `push / pull / press / talk / activate`
- use one forward interaction flow:
- ray source: `DirectionalRaycaster`
- target filtering: `RaycastFilter`
- highlight: `IMarkable`
- action trigger: `MainInput.Character.CommunicationAction`

Mapping by target:
- NPC talk: same interaction ray + mark flow, then transition into `StateType.Communication`
- button / rune / activator: same ray + mark flow, then dispatch direct world-state change
- movable object: may use focus targeting plus a dedicated movable contract instead of item pickup semantics
- pressure plate: usually passive world listener, activated by object weight or state
- slider / chain / mechanism: direct interaction target with stateful response
- manual pickup if still needed: same ray + mark flow, then call `ITakable.Take()`

Implementation note:
- current loot contracts are not enough to express all environmental interactions cleanly
- do not force `push / pull / press` through `ITakable`
- prefer dedicated environmental interfaces or components for:
  - movable objects
  - activators
  - reactive mechanisms
  - pressure listeners

### Environmental mechanisms

- `block / plate / gate / slider / chain / barrier / bridge`
- keep these as composable world objects
- prefer small purpose-built components over one giant puzzle manager

Recommended split:

- `MovableObject`: owns movement rules and allowed interaction verbs
- `WorldActivator`: changes state on press or signal
- `PressurePlate`: listens for object or weight presence
- `GateController` / `BridgeController`: owns visible world state
- `TimedStateController`: returns state after delay when needed
- `SignalRelay` or event-based link when decoupling matters

Implementation note:

- local courtyard setups may use serialized references directly
- reusable mechanism families should stay component-driven and inspector-wired

### Magic support

- `magic as environmental tool, not combat pillar`
- if spell casting is used, it should support:
  - remote activation
  - pulling or pushing magical targets
  - triggering distant mechanisms
- mana and cooldown are optional balancing tools, not the identity of the loop

Implementation note:

- existing projectile code can be reused when distance interaction adds clarity
- do not introduce multiple spell schools for MVP unless explicitly approved

### Quest flow

- `mentor intro / mechanism chain / final ritual / completion`
- quest progression should sit above movement, FSM, interaction, and environmental systems, not inside them
- use existing systems as triggers:
- mentor interaction via `CommunicationAction`
- mechanism completion via activators and listeners
- final ritual completion via combined world-state checks
- movement and action states remain execution layer only

### Data

- use `ScriptableObject` assets where tuning is useful:
  - interaction strength
  - slider timings
  - gate timings
  - hazard cadence
  - quest text
- use serialized scene references where the setup is location-specific and not worth abstracting into content databases

## Suggested architecture by mechanic

1. `Movement`: reuse as-is; only add state gates when an interaction needs temporary lock or alignment.
2. `Interaction`: keep one forward interaction controller over raycaster + markable + input.
3. `Environmental mechanisms`: build composable activators, listeners, and state controllers.
4. `Quest`: keep as orchestration over mechanism completion and world-state milestones.
5. `Magic support`: reuse spell/runtime pieces only when they strengthen environmental interaction.
6. `Loot`: keep optional and secondary to the main loop.

## Integration order

1. Lock the `push / pull / press` interaction grammar.
2. Add or adapt interaction targets and environmental components.
3. Add the first full courtyard mechanism chain.
4. Add timed state changes and hazard pressure.
5. Add quest progression around mechanism milestones and final ritual.
6. Add optional remote magic interaction if it improves readability or pacing.

## Requirements

1. Keep character movement inside `MovementModule` + movement FSM.
2. Put new player actions behind `MainInput`, not raw Unity input calls.
3. Use FSM or lightweight compatible gates when interactions must lock or redirect player action.
4. Reuse the existing raycast interaction path before inventing a second targeting system.
5. Keep puzzle-critical world logic out of inventory abstractions unless the object is truly a pickup.
6. Prefer inspector-wired environmental components over a monolithic level script.
7. Extend character prefab references only through existing serialized module slots or dedicated new components.

## Unresolved questions

- should `pull` and `push` be direct object verbs, or modes of one generic interaction component
- should distance magic be part of the MVP critical path or secondary enrichment
- should hazards be purely environmental, or should one simple hostile source also be included
