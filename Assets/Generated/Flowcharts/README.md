# Блок-схемы игровых механик

В папке находятся две блок-схемы, оформленные по мотивам ГОСТ 19.701-90 (ИСО 5807-85):

- `flowchart_item_pickup.svg` - механика подбора расходуемого предмета.
- `flowchart_firebolt_cast.svg` - механика каста и полета заклинания Firebolt.

Использованные обозначения:

- овал - начало/конец алгоритма;
- прямоугольник - процесс;
- ромб - решение;
- параллелограмм - ввод/вывод или событие.

Схемы основаны на текущей логике проекта:

- `Scripts/Loot/Items/ConsumablePickupItem.cs`
- `Scripts/Combat/CharacterSpellCaster.cs`
- `Scripts/Combat/SpellProjectile.cs`
