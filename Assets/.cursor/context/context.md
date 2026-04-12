# LostTime Technical Context

## Single Source Of Truth

This file is the primary technical source of truth for the current project state.

If any other planning file, prompt file, or old documentation conflicts with this file:

1. `Assets/.cursor/context/context.md` wins.
2. The newer design direction wins over legacy combat-first docs.
3. Old combat-oriented vertical-slice docs must be treated as legacy unless they are explicitly updated to match this file.

## Current Project Direction

`LostTime` is currently a short third-person action-adventure diploma project set in an open inner courtyard of a magical medieval castle.

The project is no longer targeting a combat-heavy vertical slice as its main identity.

The current gameplay focus is:

- exploration inside one open-space courtyard map
- environmental interaction through simple magical verbs
- short quest progression for a 20-30 minute play session
- readable, chained spatial puzzles
- light action pressure from environment, timing, and moving world state

The main interaction language is:

- `push`
- `pull`
- `press`

Those verbs should drive most puzzle, traversal, and world-state interactions.

## Genre Framing

Target framing:

- `action-adventure with environmental magical puzzles`

Not target framing:

- full combat game
- deep RPG
- inventory-heavy adventure
- physics sandbox

Action in this project should mainly come from:

- timing windows
- moving gates and bridges
- hazardous zones
- world pressure during puzzle execution
- fast chained interaction sequences

## World / Level Context

- Main playable space is an open inner castle courtyard.
- The space can include walls, gates, towers, exterior stairs, raised platforms, altars, ruins, arches, and outer walkways.
- The space should not be designed around indoor room-to-room progression.
- Progression should come from courtyard sectors, vertical routes, gate states, and world-state changes.

## MVP Fantasy

Player fantasy:

"I am a young mage using simple but powerful magic to manipulate an ancient courtyard, overcome dangerous magical mechanisms, and complete a real trial."

The hero should feel active and capable, but the game does not need complex combat depth to support that fantasy.

## Primary Design Pillars

### 1. One Clear Interaction Language

Most core gameplay should be expressible through:

- pulling objects or mechanisms
- pushing objects or mechanisms
- pressing buttons, runes, plates, or activators

### 2. Spatial Cause And Effect

The player should read the courtyard, understand how one object affects another, and solve problems by changing world state step by step.

### 3. Short Chained Puzzles

The strongest puzzle format for this project is:

- action A unlocks object B
- object B enables action C
- action C opens path D

Simple verbs, deeper sequencing.

### 4. Action Pressure Without Full Combat Dependence

The project may include combat or hostile entities, but they are secondary.

Primary pressure should come from:

- temporary openings
- dangerous floor patterns
- unstable magical energy
- moving gates
- forced repositioning

## Technical Project Snapshot

## Unity / Rendering

- Engine version: `6000.3.9f1`
- Unity revision: `6000.3.9f1 (7a9955a4f2fa)`
- Render pipeline: `Universal Render Pipeline`
- Main render pipeline asset: `Assets/Settings/PC_RPAsset.asset`
- Default renderer asset: `Assets/Settings/PC_Renderer.asset`
- URP global settings asset: `Assets/Settings/UniversalRenderPipelineGlobalSettings.asset`
- Main scene currently present: `Assets/Scenes/SampleScene.unity`

## High-Level Structure

- `Assets/Scripts/Character`: player setup, modules, and state-driven behaviour
- `Assets/Scripts/FSM`: custom FSM and HFSM infrastructure
- `Assets/Scripts/Loot`: interaction-adjacent data and interfaces, pickups, inventory pieces
- `Assets/Scripts/Quest`: current quest orchestration scripts
- `Assets/Scripts/Combat`: projectile, mana, vitals, and combat-adjacent runtime pieces
- `Assets/Scripts/Enemy`: legacy enemy encounter slice pieces
- `Assets/Scripts/Input`: Input System asset and generated wrapper
- `Assets/Scripts/DI`: Zenject installers
- `Assets/Scripts/Utils`: event bus, raycast filters, and raycasters
- `Assets/Scripts/CodeGeneration`: editor-side constant generation
- `Assets/Generated`: generated constants and generated outputs
- `Assets/Scenes/SampleScene.unity`: current active working scene

## Existing Reusable Technical Seams

### 1. Character Composition

The player runtime already uses modular composition instead of one monolithic controller.

Important seams:

- `MovementModule`
- `RotationModule`
- `AnimationModule`
- root `Character` behaviour

This is the correct place to integrate new interaction-driven behaviour.

### 2. FSM / HFSM

The project already has:

- `State`
- `StateMachine`
- `StateTransition`
- `HierarchicalState`

Use these seams when player control must be gated, locked, or temporarily redirected by interaction logic.

Do not bypass them with ad-hoc state booleans if an existing state seam can solve the problem.

### 3. Input System + Zenject

Input is already centralized through:

- `Assets/InputSystem_Actions.inputactions`
- generated `MainInput.cs`
- `InputInstaller`

All new player actions should start from the Input System asset and flow through the existing generated wrapper and DI path.

### 4. Interaction Targeting

The project already has a forward interaction path built on:

- `DirectionalRaycaster`
- `RaycastFilter`
- `InteractionController`
- `InteractionTarget`
- `IMarkable`

This is the preferred seam for:

- buttons
- runes
- levers
- altar interactions
- direct activators

### 5. World Item / Data Patterns

The project already uses data-driven assets and simple interfaces:

- `ItemSetup`
- `ItemsDatabase`
- `ITakable`
- `IInteractable`

These can be reused where appropriate, but puzzle-critical world logic should not be forced into item semantics if a dedicated environmental interaction component is cleaner.

### 6. Spell / Projectile Runtime

The project already contains:

- `CharacterSpellCaster`
- `SpellProjectile`
- `CharacterMana`
- `CharacterVitals`

These can be reused or repurposed as support systems.

Important: they are no longer the primary identity of the project.

Spell or projectile behaviour should support environmental interaction if helpful, but the MVP must not depend on expanding into a full combat architecture.

## Current Recommended Mechanic Architecture

Best fit for the project:

- a world interaction layer that translates `push / pull / press` into object state changes
- reusable environmental components such as:
  - movable blocks
  - pressure plates
  - buttons
  - pullable sliders
  - chains
  - gates
  - bridges
  - magical barriers
- a lightweight quest or progression layer that tracks completed world states
- optional hazard systems that create timing and action pressure

## Current Scope Boundaries

Include:

- one open courtyard map
- one mentor or framing NPC if needed
- a short guided progression chain
- environmental magical puzzles
- world-state based gate opening
- minimal HUD and interaction readability
- optional light danger or hazard pressure

Avoid unless explicitly re-approved:

- large enemy roster
- boss-first design
- multiple combat styles
- heavy inventory management
- branching quests
- large multi-scene story expansion

## Legacy Systems Status

The repository still contains a combat-first vertical slice:

- seal restoration flow
- enemy encounters
- guardian encounter
- arena unlock logic

These are useful as reference or reusable code, but they are not the current design source of truth.

Agents must not assume:

- three enemy archetypes are required
- a guardian boss is required
- combat is the main progression driver
- indoor room-based structure is still intended

## Project-Specific Libraries / Packages

### In-project / embedded libraries

- `Zenject`
- `UniTask`
- `DOTween`

### Unity packages in use

- `com.unity.render-pipelines.universal`
- `com.unity.inputsystem`
- `com.unity.cinemachine`
- `com.unity.addressables`
- `com.unity.ai.navigation`
- `com.unity.animation.rigging`
- `com.unity.behavior`
- `com.unity.probuilder`
- `com.unity.postprocessing`
- `com.unity.timeline`
- `com.unity.visualeffectgraph`
- `com.unity.visualscripting`

## Implementation Guidance

- Prefer extending existing seams before creating a parallel architecture.
- Prefer `ScriptableObject` data for values designers may tune.
- Prefer world-state orchestration over hardcoded one-off scene hacks.
- Prefer small, composable environmental components over one giant puzzle manager.
- Prefer inspector-driven scene wiring when the interaction is level-specific.
- Do not manually edit generated files under `Assets/Generated` unless intentionally changing generator flow.
- If an old document conflicts with this file, update the old document or mark it as legacy before using it.
