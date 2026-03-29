# Realization Steps

## Stage 1. Lock MVP frame

Goal: freeze scope before feature work.

Steps:

1. keep one region, one hero, one mentor, one main quest
2. keep combat to one primary magic shot
3. keep progression to 3 seals -> boss -> return
4. keep content target to 3 enemy types + 1 boss

Output:

- concept approved
- core loop approved
- mechanic map approved

Inspector work:

1. none

## Stage 2. Vertical slice

Goal: one fully playable short path.

Steps:

1. add combat input actions: cast, aim if needed
2. add first combat state using existing FSM seams
3. create one magic projectile with mana cost and cooldown
4. create one enemy: melee beast
5. create one mentor NPC interaction
6. create one seal interaction
7. add basic HUD: hp, mana, quest text, interact hint

Output:

- player can talk, cast, kill one enemy, restore one seal

Inspector work:

1. bind new actions in `Assets/Scripts/Input/MainInput.inputactions`
2. assign projectile prefab and spell data on player/combat component
3. add mark / interact targets on mentor and seal prefabs
4. wire HUD references in scene canvas

## Stage 3. Full main loop

Goal: complete full start-to-finish story path.

Steps:

1. duplicate seal flow to 3 anomaly points
2. add ranged cultist
3. add heavy golem
4. add mana, heal, coin pickups
5. add final arena unlock after 3 seals
6. add return-to-mentor completion step

Output:

- full quest chain works from intro to ending

Inspector work:

1. place 3 seal objects and connect completion handlers
2. place enemy spawners or encounter prefabs per zone
3. create `ItemSetup` assets for mana, heal, coins
4. assign pickup prefabs and colliders

## Stage 4. Boss and ending

Goal: add climax and clear graduation payoff.

Steps:

1. create trial guardian boss
2. give boss 2 attack patterns minimum
3. open ending state after boss death
4. add final mentor dialog and win screen

Output:

- game has clear climax and resolution

Inspector work:

1. place boss arena trigger and boss prefab
2. wire boss death event to quest progression
3. assign ending UI panel and mentor final dialog

## Stage 5. Polish

Goal: make MVP readable and presentable for diploma.

Steps:

1. add cast, hit, pickup, UI sounds
2. add simple spell and seal VFX
3. balance enemy hp, damage, mana economy
4. improve quest text clarity
5. improve onboarding in first minute

Output:

- stable demo with readable feedback

Inspector work:

1. assign audio clips on combat, UI, pickup objects
2. assign VFX prefabs on spell, seal, boss attacks
3. tune values in ScriptableObjects and serialized fields

## Delivery order

1. Stage 1 docs
2. Stage 2 vertical slice
3. Stage 3 full loop
4. Stage 4 boss ending
5. Stage 5 polish

## Requirements

1. Never expand scope before Stage 4 works.
2. Ship playable slice before adding second and third enemy.
3. Prefer ScriptableObjects for tunable combat and item data.
4. Prefer existing FSM / input / loot seams over parallel systems.
5. Every stage must end in playable scene state.

## Unresolved Questions

- should coins be pure score, gate currency, or optional reward only
- should aim be free-fire only or soft lock-on
