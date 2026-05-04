# 2026-05-03_dialogue-system-plan

## summary
Подготовлен подробный markdown-план архитектуры диалоговой системы и добавлен `.json` implementation plan в формате существующего task plan: runtime lifecycle, SO schema, conditions, global history, orchestrator, MVP UI, integration points, testing, scene/inspector impact, execution graph.

## files changed
- Assets/.cursor/docs/dialogue-system-plan.md
- .codex/tasks/dialogue-system/dialogue-system-plan.json
- Assets/.cursor/docs/worklogs/2026-05-03_dialogue-system-plan.md

## scene/inspector
- Изменений в сценах и инспекторе пока нет.
- Планом зафиксированы будущие inspector requirements:
  - `interactionPrompt`
  - `graphicsTarget`
  - `cameraPivot`
  - `dialogueDefinition`
  - animation config key `Talking`

## validation
- План согласован с текущими кодовыми точками интеграции:
  - `IInteractable`
  - `InteractionTarget`
  - `InteractionController`
  - `Character`
  - `StateType.Communication`
- Отдельно отмечены отсутствующие артефакты:
  - `Assets/.codex/context/context.md`
  - `Assets/.cursor/docs/WORKLOG_TEMPLATE.md`

## decision log
- План оформлен как отдельный md-документ для дальнейшего ревью и конвертации в `.json` implementation plan.
- В качестве фактического source of truth использован текущий код, так как контекстный файл из инструкции не найден в репозитории.
- Структура ворклога взята из существующих файлов в `Assets/.cursor/docs/worklogs/`, так как шаблон ворклога отсутствует.

## follow-ups
- После одобрения `.json` плана переходить к реализации по execution graph.
- При необходимости отдельно уточнить restart policy completed dialogue и UX для unavailable options.

## commit
pending review
