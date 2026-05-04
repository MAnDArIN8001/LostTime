# 2026-05-03_interaction-hover-outline-layer

## summary
Доработано поведение ховера: слой `Outline` ставится на `GraphicsTarget` коммуникабельного (`InteractionTarget`) и все его дочерние объекты; при потере/смене фокуса исходные слои каждого объекта восстанавливаются.

## files changed
- Assets/Scripts/Loot/Systems/InteractionController.cs
- Assets/Scripts/Loot/Systems/InteractionTarget.cs

## scene/inspector
- Убедиться, что в проекте существует слой `Outline` (`Project Settings > Tags and Layers`).
- На каждом `InteractionTarget` при необходимости назначить поле `GraphicsTarget` (если не задано, используется `transform` самого объекта).
- Для сложных иерархий визуала указывать корневой `GraphicsTarget`, чтобы подсветка покрывала всю нужную ветку children.

## validation
- При наведении на коммуникабельный объект слой `Outline` применяется к `GraphicsTarget` и всем его children.
- При переключении фокуса на другой объект слои предыдущего объекта полностью восстанавливаются.
- При уходе с объекта все слои возвращаются к исходным значениям.
- При пустом `GraphicsTarget` используется `transform` объекта и его children.

## decision log
- `GraphicsTarget` добавлен прямо в `InteractionTarget`, так как это целевой коммуникабельный контракт текущего запроса.
- В `InteractionController` хранится снимок исходных слоёв по каждому объекту подсвеченной ветки для корректного rollback.
- Смена слоя сделана итеративным обходом children без рекурсии.

## follow-ups
- Если аналогичное поведение нужно для `IPressable`/`IControlable`, стоит вынести `GraphicsTarget` в общий интерфейс-провайдер.

## commit
pending review
