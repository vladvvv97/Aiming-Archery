# SPEC.md

Technical specification for soft launch. Binding product rules: `DESIGN.md`. Phased delivery: `PLAN.md`.

## 1. Goals & non-goals

**Goals:** playable Yandex WebGL campaign (8–15 levels), 5 arrow types with per-type budgets, stars, cloud progress, RU/EN/TR, rewarded continue + interstitial.

**Non-goals (soft launch):** coins/shop, cosmetics, portrait, multi-branch map, pure data-driven spawner without scenes.

---

## 2. Data model

### 2.1 `ArrowTypeId`

```csharp
public enum ArrowTypeId
{
    Normal = 0,
    Magic = 1,
    Fire = 2,
    Ice = 3,
    Piercing = 4,
}
```

### 2.2 `ArrowTypeConfig` (ScriptableObject)

| Field | Type | Notes |
|-------|------|--------|
| `id` | `ArrowTypeId` | Unique |
| `displayNameKey` | string | i18n key |
| `icon` | Sprite | HUD button only |
| `prefab` | `Arrow` | Projectile |
| `baseDamage` | float | Fed into body multipliers |
| `aoeRadius` | float | Magic; 0 = none. Large enough to reach a neighbor |
| `aoeDamageFalloff` | optional | Default flat AOE dmg = base |
| `pierceCharges` | int | Piercing = 1. First solid hit is a pass-through; the next sticks |
| `gravityScale` | float | 1 = normal arc. Piercing ≈ 0.12 (near-straight) |
| `ignite` | bool | Fire |
| `igniteDuration` | float | Shield/prop burn time |
| `igniteDps` | float | Optional DoT on flesh |
| `freezeDuration` | float | Ice; 0 = none |
| `trailColor` / SFX keys | optional | Polish |

Assets path suggestion: `Assets/Data/Arrows/`.

### 2.3 `LevelConfig` (ScriptableObject)

| Field | Type | Notes |
|-------|------|--------|
| `levelId` | int | 1…N; map order |
| `sceneName` | string | Build Settings scene name |
| `arrowBudgets` | `(ArrowTypeId type, int count)[]` | Only types with count > 0 |
| `stars` | `StarThresholds` | See below |
| `unlockArrowOnClear` | `ArrowTypeId?` | Optional grant on first clear |
| `teachNotes` | optional | Editor-only / Level 1 tips keys |

**StarThresholds** (remaining arrows after win — exact formula):

```
score = sum(remaining counts)
// optional weight: rarer types count × weight in LevelConfig
3★ if score >= threeStarMin
2★ if score >= twoStarMin
1★ otherwise (on win)
```

Defaults per level authored in asset; no global hardcode.

Assets: `Assets/Data/Levels/Level_XX.asset`.

### 2.4 Progress save blob

```json
{
  "v": 1,
  "unlockedLevel": 1,
  "stars": { "1": 3, "2": 1 },
  "unlockedArrows": ["Normal"],
  "flags": {}
}
```

- `unlockedLevel`: highest playable index (1-based).
- `stars[levelId]`: best 1–3 (never decrease).
- `unlockedArrows`: list of `ArrowTypeId` names; always includes `Normal`.
- No coin field.

Storage: local cache + Yandex cloud player data. Merge policy: **max** stars per level, **max** unlockedLevel, **union** arrows.

---

## 3. Runtime systems

### 3.1 `LevelController`

Singleton-per-scene (or scene-scoped service).

**Responsibilities:**

- Load `LevelConfig` (ref on scene bootstrap).
- Filter budgets to types in config ∩ `Progress.unlockedArrows`.
- Track remaining counts; expose `CanShoot(type)`, `Spend(type)`, `AddBonusArrow(type)` (ads).
- When `Spend` empties the active type, select the next budget entry that still has ammo (wraps). Empty slots are not selectable.
- Register enemies (`Enemy` OnEnable/OnDestroy or explicit list).
- Observe shots in flight (`Arrow` register/unregister).
- **Win:** enemies alive == 0 → compute stars → save → show win UI → interstitial policy → map/next.
- **Lose check:** when all budgets 0 and no arrows in flight and physics settled → lose UI (rewarded once / restart).

**Physics settled (tunable):**

- No tracked arrows alive; and
- For ~`settleSeconds` (default 1.0–1.5s): max `|linearVelocity|` among dynamic RBs in level bounds < `sleepVelocity`; or Unity sleep on those bodies.

Pseudo:

```
OnArrowSpent:
  if AnyBudgetLeft: return
  StartCoroutine(WaitSettleThenLoseUnlessWin)
```

Win always pre-empts lose if enemies hit 0 during settle.

### 3.2 Bow / spawn

- `Wood_Bow` (or extracted `BowController`) asks `LevelController` for active type + remaining.
- Spawns prefab from `ArrowTypeConfig`; applies impulse as today. Aim preview uses that type's `gravityScale`.
- If remaining == 0 for active type, refuse release (feedback SFX/UI).

### 3.3 `Arrow` hit pipeline

Order on contact:

1. If Piercing and `pierceChargesLeft > 0`: decrement, damage this contact, `IgnoreCollision` on it, keep velocity, do not stick.
   - Shield: durability−1 only (body is a later contact).
   - Body zone: that zone’s damage, then ignore the rest of this enemy.
   - Anything else (wall, ground, …): ignore that body’s colliders.
2. Otherwise stick. If `EnemyShield` and durability > 0:
   - Fire: start ignite on shield; do not body-hit unless shield already 0.
   - Else: durability−1, stop arrow (stick/block) — no body damage.
3. Body zones (`Head`/`Chest`/`Legs`): existing multipliers.
4. Magic: on the sticking hit, `OverlapCircle` damage enemies in `aoeRadius` (neighbors included) and spawn a burst VFX of that radius. Direct hit and AOE on the same enemy do not stack.
5. Ice: apply freeze to `Enemy` or dynamic `Rigidbody2D` with marker.
6. Fire on flammable prop: start burn.
7. Existing barrel `Explosion` path kept.

Ground impact plays `arrow_hit_ground` when the collider’s tag **or** layer is `Ground` (level tiles are often untagged but on the Ground layer).

### 3.4 `EnemyShield`

| Field | Default |
|-------|---------|
| `durability` | 2 |
| `onBurned` | destroy shield visual / set durability 0 |

When durability ≤ 0: disable shield collider / object so further hits go to body.

### 3.5 Input

| Platform | Move | Aim/draw |
|----------|------|----------|
| Touch | Joystick A | Joystick B (current) |
| Desktop | WASD / arrows | Mouse press-drag from bow/anchor; release = shoot; magnitude = power |

Detect: `Application.isMobilePlatform` or touch supported && last input touch; allow override in settings later.

Shared: produce `Vector2 aimDir` + `float draw01` for existing bow tiers.

### 3.6 Start zone

- Evolve `DynamicStartZoneLimiter` → barrier volume + VFX line/wall.
- Player cannot cross; camera/world beyond OK.
- No flag sprite required.

### 3.7 UI

| Screen | Contents |
|--------|----------|
| Main | Play → Map, Options (audio, maybe lang override), Exit |
| Map | Linear slots 1…N; locked/unlocked; stars; arrow unlock toast |
| HUD | Arrow slots (icon+count), pause/restart |
| Win | Stars, next / map |
| Lose | Rewarded continue (type picker among types with config budget > 0 or unlocked types that had budget this level), restart |

Slots: show types with `budget > 0` in this level that are unlocked. Each slot is a **button** with the arrow icon and the count only (no type name). Selected highlight on the button.

### 3.8 Campaign unlock defaults (editable data)

| Cleared level | Unlocks |
|---------------|---------|
| (start) | Level 1, arrow Normal |
| Level 1 | Level 2 |
| Level 3 | Magic |
| Level 5 | Fire |
| Level 7 | Ice |
| Level 9 | Piercing |

Also: `LevelConfig.unlockArrowOnClear` can grant on that level for clarity in content.

Level 1 teach scripted prompts (optional lightweight): move, jump, aim, kill one orc — no coins.

---

## 4. Yandex integration

### 4.1 Boot

1. Init SDK (WebGL).
2. Resolve language → load locale.
3. Load cloud progress → merge local → apply.
4. Open main menu.

Editor: mock SDK (lang `ru`, save to PlayerPrefs, ads auto-success after delay).

### 4.2 Ads

| Placement | Type | Rule |
|-----------|------|------|
| After lose continue | Rewarded | Once per attempt; on success `AddBonusArrow(selected)`; resume play |
| Restart level | Interstitial | After confirm restart |
| Back to map / Next level | Interstitial | Soft policy: skip if just watched rewarded < 30s (optional) |

### 4.3 i18n

- Keys for all player-facing strings.
- Tables: `ru`, `en`, `tr`.
- Lang: SDK `environment.i18n.lang`.
- Fallback: `be|kk|uk|uz` → `ru`; else → `en` if missing; missing key → key name in dev builds.

---

## 5. Scene & build

| Scene | Purpose |
|-------|---------|
| `Scene_Game_Start` | Menu + entry to map |
| `Level_01` … `Level_N` | Gameplay; each refs its `LevelConfig` |

Build Settings: menu first, then levels in order. Loading via scene name from config.

Legacy: `TrainingManager` scenes/objects removed in Phase 5; `Scene_Game_Level_0` either becomes `Level_01` or is replaced.

---

## 6. Interfaces (decouple platform)

```csharp
public interface IProgressSave
{
    ProgressData Load();
    void Save(ProgressData data);
}

public interface IAds
{
    void ShowInterstitial(Action onClosed);
    void ShowRewarded(Action onSuccess, Action onFailOrCancel);
}

public interface ILocale
{
    string Lang { get; }
    string T(string key);
}
```

Gameplay only depends on these; Yandex and Editor provide implementations.

---

## 7. Acceptance criteria (soft launch)

1. Fresh save: Level 1 only, Normal only; completable with teach flow.
2. Win/lose/stars match DESIGN; lose waits for settle.
3. All 5 arrow types work on at least one test level each (gated by unlock in campaign).
4. Shield + fire + piercing rules match DESIGN § Shield.
5. Map unlocks linearly; best stars persist after reload (cloud or local in Editor).
6. RU/EN/TR strings for menu/HUD/win/lose/map.
7. Rewarded continue once; interstitial on restart.
8. Desktop mouse-drag + WASD; mobile dual stick.
9. No coin UI or coin PlayerPrefs usage.
10. WebGL build runs in Yandex draft without console errors on happy path.
11. 8–15 levels in build with distinct budgets.

---

## 8. Test checklist (per phase)

- P1: budget 3 → 3 shots max; kill all with 1 left → ≥2★; miss all → lose after settle.
- P2: piercing passes the first contact and sticks in the second (wall then orc, or shield then body); shield blocks normal until durability 0; magic burst can reach a neighbor.
- P3: fire burns shield; ice stops falling plank briefly.
- P4: complete level mouse-only aim; barrier blocks exit.
- P5: no coin references in scenes; Level 1 has no TrainingManager.
- P6: mock rewarded grants +1; lang switch updates HUD.
- P7: full map clear on soft-launch candidate.

---

## 9. Open tunables (not design locks)

- Exact `settleSeconds`, star mins per level, ignite DPS, freeze duration, AOE radius, shield default durability, interstitial frequency cap.
- Authored in SO / LevelConfig — change without code when possible.
