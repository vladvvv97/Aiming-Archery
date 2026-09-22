# AGENTS.md

## Project

2D WebGL archery siege for **Yandex Games** (Angry Birds–like). Hero: elf archer. Ballistic arrows, gravity, destroy enemies/structures. Soft launch target.

Unity **6000.6**, landscape. Evolve existing prototype — do not rewrite from scratch.

## Agent rules

- Unity Editor/project work: **Unity CLI only** (see `.cursor/rules/unity-cli-only.mdc`). Prefer `unity status` / `unity command` / `unity command eval`.
- Prefer driving a live Editor over hand-editing `.unity` / `.prefab` YAML.
- Keep changes scoped; match existing code style; no drive-by refactors.
- After gameplay/script edits: recompile via CLI and check console (0 errors).

## Docs map

| File | Read when |
|------|-----------|
| `DESIGN.md` | Mechanics, economy, Yandex, i18n, soft-launch scope |
| `ARCHITECTURE.md` | Folders, systems, data flow, what to touch |
| `PLAN.md` | Phased delivery order, exit criteria, risks |
| `SPEC.md` | Data model, APIs, save schema, acceptance tests |
| This file | How to work in the repo |

## Soft-launch constraints (do not violate)

- **No coins / shop meta** in soft launch (remove or gut coin economy).
- Arrow unlocks = **campaign progress only**.
- Tutorial = **campaign Level 1**, not a separate `TrainingManager` flow.
- Languages: **RU / EN / TR** via string keys + Yandex `i18n.lang`.
- Win: all enemies dead. Lose: out of arrows after last shot settles + physics calm.
- Ads: rewarded continue (+1 arrow of an available type, once per attempt); interstitial on restart / between levels.

## Current prototype debt (expected cleanup)

- Flat leftovers: `TrainingManager`, `TrainingCoin`, `TrainingOrc`, coin UI — replace with Level-1 teach + stars/progress.
- Only `Wood_Arrow` exists; introduce data-driven arrow types.
- `Scene_Game_Level_1` unfinished / not in build — fold into campaign levels.
- Start-zone “flag” → non-flag barrier/VFX (`DynamicStartZoneLimiter` evolve).

## Workflow hints

```bash
unity status
unity command console --level Warning --tail 50
unity command recompile
```

Authored levels: one scene per level + `LevelConfig` asset (arrow budgets, star thresholds, unlocks).
