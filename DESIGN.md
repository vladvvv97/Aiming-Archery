# DESIGN.md

Decisions from project grill (soft launch). Change here if design changes.

## Pitch

Elf with bow destroys enemies on physics levels. Shots affected by gravity. Yandex Games (WebGL), touch + desktop.

## Soft-launch scope

- ~**8–15** levels + Level 1 as tutorial-in-campaign
- **3–8** enemies per level
- **5** arrow types (unlock over campaign; start with Normal only)
- Landscape; safe area for Yandex iframe
- Evolve current feel (move/jump in start zone, dual-stick on mobile)

## Win / lose / stars

| Event | Rule |
|-------|------|
| Win | All enemies dead |
| Lose | All arrow budgets spent; wait until last arrow finishes + physics settles (structures may still kill) |
| Stars | 1–3 from **remaining arrows** (and/or unused rarer types) — tune thresholds in `LevelConfig` |

Rewarded ad after lose: **+1 arrow** of a chosen *already available* type, **once per attempt**, or continue. Interstitial: restart / between levels.

## Player

- Move + jump inside **start zone**; cannot cross a **barrier effect** (not a flag prop).
- Shoot from anywhere inside the zone.
- **Mobile:** dual joysticks (move + aim/draw).
- **Desktop:** WASD/arrows move; **mouse drag** aim/power (same mental model as draw).

## Arrows

Per-level **budget by type** (e.g. 3 Normal + 1 Fire). UI: **slot bar** with icon + remaining count; tap/click selects active type.

| Type | Effect |
|------|--------|
| Normal | Damage. Standard gravity arc. |
| Magic | AOE large enough to clip a neighboring enemy. Blue flame on the projectile and a burst VFX whose ring matches `aoeRadius`. |
| Fire | Ignite target / flammable props; **ignites shield → burns away** |
| Ice | Brief freeze/stop on enemy **or** dynamic prop |
| Piercing | Almost no gravity (near-straight flight). Passes through the **first** solid hit (wall, orc, or shield): that hit is damaged and the arrow keeps flying. Sticks in the **second** object (wall, orc, ground, …). |

**Unlock:** campaign milestones only (no coin shop). Soft launch starts with Normal; Magic / Fire / Ice / Piercing unlock later.

HUD slots are **buttons**: the arrow’s icon plus the remaining count. The icon is the only type label. If the selected type hits 0, the bow switches to the next type in the level that still has ammo.

### Shield rules

- Shield has durability.
- Non-piercing (except fire): hit shield → **−1 durability**, no body damage while durability > 0.
- Fire: ignites shield until destroyed.
- Magic AOE may damage body if blast hits around the shield, and can reach a neighbor inside `aoeRadius`.
- Piercing: the shield counts as the first obstacle (−1 durability, arrow does not stick). The next contact sticks and, if that is the body, deals zone damage.

## Environment

Mixed: static / dynamic / flammable wood / single-layer pierceable walls / explosive barrels. Enough for arrow fantasy without full soft-body.

## Enemies

Orc variants: normal / shield / thick. Hit zones kept: head×2, chest×1, legs×0.5. One extra enemy type later in soft launch if needed.

## Campaign / meta

- Linear level map with star slots; clear level N → unlock N+1 (and possibly an arrow type).
- **No coins** in soft launch.
- Progress (levels, stars, unlocks) via **Yandex cloud save** + SDK.

## Yandex

- SDK init, cloud progress, interstitial + rewarded as above.
- i18n: **RU / EN / TR**, keys + tables; language from SDK; fallback `ru` for be/kk/uk/uz else `en` ([Yandex languages](https://yandex.ru/dev/games/doc/ru/concepts/languages-and-domains)).

## Explicitly out of soft launch

Coin economy, cosmetic shop, deep skill trees, portrait mode, full multi-race enemy roster, data-only level shell (we use scene + config).
