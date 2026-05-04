# Worklog Template

Use this template for every agent task that changes project files.

Path convention:

- `Assets/.cursor/docs/worklogs/YYYY-MM-DD_short-task-slug.md`

## Template

```md
# {Task Title}

## Summary

Short description of what changed and why.

## Context

- task goal
- current stage or planning context
- technical constraints that mattered

## Files Changed

- `path/to/file`
- `path/to/file`

## Scene/Inspector

1. exact Unity editor steps, or `none`
2. exact serialized references, or `none`

## Validation

1. what was checked
2. what still needs checking
3. explicit gaps if Unity runtime validation was not performed

## Decision Log

- chose: {approach}
- avoided: {alternative}
- why: {reason}

## Follow-Ups

- next logical task
- cleanup or refactor ideas
- open risks

## Commit

- prefix used: `fix|feat|update|refactor`
- subject: `prefix: short subject`
- status: `planned` or `created`
```
