# Review Gate Workflow Update

## summary
- Added explicit review-gate workflow to agent instructions and ui-manager plan.
- Enforced rule: no commit before reviewer explicit approval.

## files changed
- `AGENTS.md` - added mandatory review gate before commit.
- `.codex/tasks/ui-manager/ui-manager-plan.json` - added execution policy and commit-after-review constraints.

## scene/inspector
- No scene changes.
- No inspector changes.

## validation
- Verified updated instruction text and plan keys are present.

## decision log
- Review-first pipeline made explicit in both global agent instructions and task-local plan to ensure cross-agent consistency.

## follow-ups
- Apply same execution_policy block to other active plans if needed.

## commit
- hash: pending (awaiting review approval)
- message: pending
