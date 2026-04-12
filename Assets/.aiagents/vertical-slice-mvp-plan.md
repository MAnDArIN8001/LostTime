# LostTime Core Mechanic Plan

Цель: обновить таск-флоу проекта так, чтобы первой и обязательной веткой реализации стала core-механика `push / pull / press`, построенная на `IMarkable` и отделенной character feature, которая не ломает текущую state machine и может работать параллельно движению.

## Core Intent

- Core mechanic должна жить как отдельная ветка реализации, а не как побочный слой текущего quest flow.
- Маркировка доступных целей идет через `IMarkable`.
- Коммуникация с целями идет через `IPressable` и `IControlable`.
- Feature персонажа для `push / pull / press` должна висеть отдельно от текущей movement/combat state machine или использовать отдельную локальную state machine.
- Игрок должен иметь возможность одновременно двигаться и удерживать control interaction, если это допускает конкретный объект.
- `push` и `pull` привязаны к pointer мыши, а не только к facing direction персонажа.
- Решение должно быть абстрактным и масштабируемым: новые типы объектов добавляются через интерфейсы и runtime contracts, а не через переписывание character flow.

## Main Architecture Shift

1. Сначала реализуется `Core Interaction Branch`.
2. Потом делается `Character Control Branch`, которая подключает core к персонажу без rewrite текущего movement/combat flow.
3. Потом делается `World Objects Branch` для конкретных press/push/pull объектов.
4. Только после этого идет `Quest / HUD / Scene Branch`.

Это означает, что vertical slice больше не является первой целью. Первая цель теперь: доказать, что core-механика работает как независимый слой.

## Key Runtime Rules

- `IMarkable` отвечает только за mark/highlight/focus state.
- `IPressable` отвечает за одноразовое или дискретное действие `press`.
- `IControlable` отвечает за длительную коммуникацию с объектом: drag, push, pull, hold, steer.
- Character interaction feature принимает решение, с каким объектом идет коммуникация, и держит runtime session отдельно от movement HFSM.
- Pointer мыши определяет target selection и control direction для `push/pull`.
- Core не должен зависеть от конкретного квеста, конкретной сцены или seal-only логики.
- Существующий `InteractionController` можно расширить или использовать как focus-discovery слой, но он не должен остаться единственной точкой логики для всей новой механики.

## Editor Plan

1. Сохранить текущий `InteractionController` как базу для focus detection и `IMarkable`, но не смешивать туда всю новую control-логику.
2. Создать отдельную папку core-механики: `Scripts/Gameplay/Interaction/Core`.
3. Создать отдельную папку character feature: `Scripts/Gameplay/Interaction/Character`.
4. Создать отдельную папку world objects: `Scripts/Gameplay/Interaction/World`.
5. После завершения `LT-CORE-001` и `LT-CORE-002` зафиксировать vocabulary: `IMarkable`, `IPressable`, `IControlable`, interaction session, control mode, pointer target.
6. После `LT-CORE-003` подключить новую feature к персонажу так, чтобы движение и core interaction могли обновляться параллельно.
7. После `LT-CORE-004` сделать минимум один `press`-объект, один `push/pull`-объект и один объект с длительным `control` session.
8. После `LT-CORE-004` проверить, что pointer-driven control читается в сцене без дополнительного туториала.
9. После этого только затем подвязывать quest progression, HUD prompts и финальную orchestration-цепочку через `LT-VS-*`.

## Suggested Runtime Decomposition

- `Focus Discovery`
  Находит `IMarkable`, `IPressable`, `IControlable` под pointer или в зоне доступа.

- `Interaction Intent Resolver`
  Решает, что игрок сейчас хочет сделать: `press`, `start control`, `maintain control`, `stop control`.

- `Character Interaction Feature`
  Живет отдельно от основного movement/combat HFSM и управляет runtime session взаимодействия.

- `Control Session`
  Держит ссылку на текущий `IControlable`, тип взаимодействия, pointer context и условия завершения.

- `World Interaction Adapters`
  Конкретные объекты мира реализуют `IPressable` и/или `IControlable`, не зная деталей character state machine.

- `Quest / HUD Bridges`
  Подписываются на события core-механики, но не определяют ее устройство.

## Acceptance

- В проекте есть выделенная core-ветка механики `push / pull / press`.
- `IMarkable`, `IPressable`, `IControlable` описывают базовый словарь взаимодействия.
- Новая character feature живет отдельно от current state machine либо через отдельную локальную state machine.
- Игрок может двигаться параллельно с активной control-сессией там, где это разрешено дизайном объекта.
- `push` и `pull` реально завязаны на pointer мыши.
- Новые world objects подключаются через интерфейсы и adapters, а не через правки в квестовой логике.
- Quest/HUD/scene flow зависят от core-механики, а не наоборот.

## Execution Note

- Крупные пункты в `json` остаются orchestration-task.
- В работу выдаются только atomic leaf-task.
- Первая обязательная ветка реализации: `LT-CORE-*`.
- Задачи `LT-VS-*` можно брать только после прохождения core-ветки или после явно отмеченных зависимостей.
