# LostTime Project Context

## Project Description
`LostTime` is a Unity gameplay prototype built around a controllable character, modular movement/animation systems, and an in-progress loot or interaction layer. The current codebase looks like a third-person character sandbox with walking, running, camera look, item setup data, and world interaction primitives.

## Unity / Rendering
- Engine version: `6000.3.9f1`
- Unity revision: `6000.3.9f1 (7a9955a4f2fa)`
- Render pipeline: `Universal Render Pipeline`
- Main render pipeline asset: `Assets/Settings/PC_RPAsset.asset`
- Default renderer asset: `Assets/Settings/PC_Renderer.asset`
- URP global settings asset: `Assets/Settings/UniversalRenderPipelineGlobalSettings.asset`
- Main scene currently present: `Assets/Scenes/SampleScene.unity`

## High-Level Structure
- `Assets/Scripts/Character`: character setup, modules, and movement states
- `Assets/Scripts/FSM`: custom finite state machine and hierarchical state infrastructure
- `Assets/Scripts/Loot`: item data, loot systems, and inventory-related code
- `Assets/Scripts/Input`: Unity Input System asset and generated wrapper
- `Assets/Scripts/DI`: Zenject installers
- `Assets/Scripts/Utils`: event bus, raycast filters, and directional raycasters
- `Assets/Scripts/CodeGeneration`: editor-side constant key generation
- `Assets/Setups`: ScriptableObject assets for character, animation, and items
- `Assets/Animations`: animator controller and movement clips
- `Assets/Generated`: generated constants such as animation keys
- `Assets/Settings`: URP assets, renderer assets, and rendering profiles

## Core Code Features
- Character controller built from serialized modules instead of one monolithic behaviour
- Custom movement flow using a hierarchical FSM with global and nested states
- Walk and run locomotion driven by `CharacterSetup` ScriptableObject values
- Camera rotation read from input every frame
- Animation signalling routed through an internal `EventBus` plus animation facade
- Loot system based on `ItemSetup`, `ItemsDatabase`, and world item interfaces like `ITakable` and `IMarkable`
- Interaction helpers built around directional raycasters and raycast filters
- Editor code generation for strongly named constant keys under `Assets/Generated`

## Project Patterns
### 1. Modular Character Composition
The main character behaviour wires together independent modules:
- `MovementModule`
- `RotationModule`
- `AnimationModule`

Concrete gameplay behaviour is implemented through variants such as `CharacterMovementModule`, while the root character MonoBehaviour coordinates them.

### 2. Custom FSM / HFSM
Movement is not handled by Animator transitions alone. The project has its own:
- `State`
- `StateMachine`
- `StateTransition`
- `HierarchicalState`

Current active movement states include `Idle`, `Walk`, and `Run`, with placeholders in `StateType` for future behaviours like `Jump`, `Aim`, `Attack`, `Looting`, and `Communication`.

### 3. ScriptableObject-Driven Configuration
The project stores gameplay configuration in assets rather than hardcoding data:
- `CharacterSetup` for movement speeds
- `AnimationParamsDataBase` for animation parameter metadata
- `ItemSetup` and `ItemsDatabase` for item content

This is a strong project convention and should be preserved when adding new configurable gameplay features.

### 4. Input System + DI
Input is handled with Unity's new Input System through:
- `Assets/Scripts/Input/MainInput.inputactions`
- generated wrapper `MainInput.cs`

Bindings are installed through Zenject in `InputInstaller`, where a shared `MainInput` instance is created, enabled, bound, and disposed.

### 5. Event-Based Animation Communication
Animation communication uses a lightweight in-project pub/sub bus:
- `Utils.Events.EventBus`
- animation facade classes under `Character.Modules.Animation.Facade`

This reduces direct coupling between gameplay states and animator parameter writes.

### 6. Editor-Time Code Generation
The project uses editor tooling to generate constant key classes. `ConstKeysGenerator` writes generated files into `Assets/Generated`. Treat this folder as generated output, not hand-authored gameplay logic.

## Project-Specific Libraries / Packages
### In-project / embedded libraries
- `Zenject` for dependency injection
- `UniTask` plugin present under `Assets/Plugins`

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

## Practical Notes For Future Work
- Prefer extending existing module abstractions before adding one-off logic to the root character behaviour.
- Prefer new gameplay data as ScriptableObjects when designers may need to tune it.
- If adding locomotion or interaction states, integrate them through the existing FSM or HFSM structure.
- Do not manually edit generated files under `Assets/Generated` unless the generation flow is intentionally being changed.
- Input changes should usually start from `MainInput.inputactions`, then propagate through the generated wrapper and Zenject wiring.
