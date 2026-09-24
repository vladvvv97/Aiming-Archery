# ARCHITECTURE.md

## Folder layout

```
Assets/
  Scripts/
    Player/          Player, Wood_Bow
    Combat/          Arrow, Wood_Arrow, Explosion
    Enemies/         Enemy*, HP_Bar, HitZones/
    Economy/         Coin* (legacy — remove for soft launch)
    Tutorial/        TrainingManager (legacy — replace with Level 1)
    Camera/          MainCamera, ZoomPan
    Level/           DynamicStartZoneLimiter, (add LevelController, LevelConfig usage)
    Environment/     Clouds*
    Audio/           AudioManager*
    UI/              Load/Option/Restart/Exit (+ add arrow slots, stars, map)
  Prefabs/           Player, Combat, Enemies, Collectibles, Level, Environment, Systems
  Scenes/Menus/      Scene_Game_Start
  Scenes/Levels/     per-level scenes
  ThirdParty/        Joystick Pack
  Art/Source/        raw PSD etc.
```

Vendor code stays under `ThirdParty/`. Game code under `Scripts/` by domain.

## Runtime loop

```
Level start
  → read LevelConfig (arrow budgets, star thresholds, enemy count win check)
  → spawn/enable player in start zone
Aim/draw (joystick or mouse drag) → spawn Arrow subclass / configured Arrow
  → gravity + rotate to velocity
  → hit: materials / shield / body zones / barrels
Win: 0 enemies alive → stars from remaining budget → save → ads?/map
Lose: budgets empty → wait projectile+physics settle → rewarded continue or restart
```

## Core types (target shape)

| Piece | Role |
|-------|------|
| `LevelConfig` (SO) | Per-level arrow counts by type, ★ thresholds, unlock flags, scene id |
| `ArrowTypeId` / `ArrowTypeConfig` (SO) | Damage, AOE, pierce, ignite, freeze params + prefab |
| `Arrow` | Shared projectile lifecycle; type mods via config or subclass |
| `LevelController` | Budgets, win/lose settle, star calc, hooks to Yandex ads/save |
| `ArrowLoadoutUI` | Slots + remaining counts |
| `StartZoneBarrier` | Evolve `DynamicStartZoneLimiter` (VFX wall, not flag) |
| `YandexServices` | SDK, cloud save, i18n lang, interstitial/rewarded |
| `I2n` / `LocaleTable` | RU/EN/TR strings |

Keep hit-zone markers (`EnemyHead` / `Chest` / `Legs` / `Shield`) — `Arrow` routes damage through them.

## Scene flow (target)

```
Menus/Scene_Game_Start
  → Level Map UI
  → Levels/Level_01 (teach: move, jump, shoot, first kill)
  → Level_02 … Level_N
```

Build settings must list menu + all shipped levels. Old `TrainingManager` path and coin-gated return to menu go away.

## Systems to touch by feature

| Feature | Primary places |
|---------|----------------|
| New arrow type | `ArrowTypeConfig` + prefab + Combat scripts; budget field on `LevelConfig` |
| New level | New scene under `Scenes/Levels/` + `LevelConfig` + map entry |
| Shield/fire/pierce | `EnemyShield` + `Arrow` hit routing + material tags on walls |
| Ads / save / lang | `YandexServices` + `LevelController` end-flow |
| Input desktop | `Wood_Bow` / new aim module alongside joystick path |
| Remove coins | `Economy/*`, HUD, `Training*`, PlayerPrefs coin keys |

## Physics / tags

Reuse tags: `Enemy`, `Ground`, `Stone`, `Wood`, etc. Ground impact audio also treats the `Ground` layer as ground, because authored tiles are often untagged. Add clear layers/tags for: pierceable wall, flammable, dynamic prop. Piercing passes the first solid contact (damage, no stick) and embeds in the second. `gravityScale` on `ArrowTypeConfig` flattens that shot.

## Persistence

Save: level unlocked index, stars per level, arrow types unlocked. Prefer Yandex cloud; local cache OK offline. No coin balance in soft launch.
