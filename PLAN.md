# PLAN.md

Implementation roadmap for soft launch. Design source: `DESIGN.md`. Code map: `ARCHITECTURE.md`. Spec details: `SPEC.md`.

## Principles

1. Evolve the prototype — ship vertical slices, not a big-bang rewrite.
2. Each phase ends in a **playable build** (Editor / WebGL locally).
3. Delete legacy (coins, `TrainingManager`) only after the replacement path works.
4. Yandex SDK can start behind interfaces (`IProgressSave`, `IAds`, `ILocale`) so gameplay doesn’t block on WebGL.

## Phase overview

| Phase | Goal | Exit criteria |
|-------|------|----------------|
| **0** | Baseline hygiene | Docs + folders stable; 0 console errors; build settings sane |
| **1** | Level shell | `LevelConfig` + `LevelController` win/lose/stars on 1 level (Normal only) |
| **2** | Combat v2 | Arrow types data + shield durability + materials tags; Normal + Piercing + Magic |
| **3** | Fire / Ice | Remaining types + flammable / freeze |
| **4** | Input & zone | Desktop mouse-drag aim; barrier VFX (no flag) |
| **5** | Campaign UX | Level map, unlocks, Level 1 teach; gut coins/training |
| **6** | Yandex + i18n | SDK, cloud save, ads, RU/EN/TR |
| **7** | Content pack | 8–15 levels, balance, WebGL soft-launch candidate |

Phases 2–3 can overlap with 4 if staffing allows; **1 before 2**, **5 after 1**, **6 can start stubs in 1**.

---

## Phase 0 — Baseline (done / verify)

- [x] Folder restructure (`Scripts` / `Prefabs` / `Scenes` / `ThirdParty`)
- [x] `AGENTS.md`, `DESIGN.md`, `ARCHITECTURE.md`
- [ ] Confirm build: `Scene_Game_Start` + at least one playable level scene
- [ ] Rename level scenes toward `Level_01`… when convenient (not blocking)

**Owner focus:** agent + human smoke-test Play Mode.

---

## Phase 1 — Level shell (first vertical slice)

**Why first:** every later system hangs on budgets / win / lose / stars.

### Work

1. Add `ArrowTypeId` enum + `LevelConfig` ScriptableObject (see SPEC).
2. Add `LevelController` in level scenes: budgets, enemy registry, settle timer, win/lose UI hooks.
3. Wire `Wood_Bow` to spend from active type budget (Normal only); block shot if 0.
4. Stub `ArrowLoadoutUI` (one slot OK).
5. Star calculation from remaining Normal count (thresholds on config).
6. Keep existing bow feel; no new arrow VFX yet.

### Exit

- Play Level_01 (or current Level_0): limited Normal arrows, win on all kills, lose after settle, 1–3★ shown.
- No dependency on coins/training for this loop.

---

## Phase 2 — Combat data + Piercing + Magic

1. `ArrowTypeConfig` SO + prefabs per type (start: Normal, Piercing, Magic).
2. Refactor `Arrow` to read config (damage, pierce charges, AOE radius).
3. `EnemyShield`: durability field; hit routing per DESIGN.
4. Tags/components: `Pierceable`, keep Wood/Stone; Magic AOE overlap query.
5. LevelConfig budgets for multiple types; UI slots for types present in level **and** unlocked.

### Exit

- Level with Normal + Piercing + Magic budgets solvable as a small puzzle.
- Shielded orc behaves per DESIGN.
- Piercing flight is near-straight and sticks only on the second contact. Magic shows a radius-matched burst. HUD slots are icon buttons and auto-advance when a type runs out.

---

## Phase 3 — Fire + Ice

1. Fire: ignite component on enemies/props/shield (burn duration → destroy shield / damage).
2. Ice: freeze RB2D / enemy AI-less stop for `freezeDuration`.
3. Flammable marker on wood props; barrel stays explosion path.
4. Tune via SO fields only where possible.

### Exit

- All 5 types functional in a sandbox level (unlock gate can still be cheat/debug).

---

## Phase 4 — Input & start zone

1. Desktop: mouse drag aim/power path beside joystick (`AimInput` abstraction).
2. Mobile unchanged (dual sticks).
3. Replace flag visual with barrier VFX; keep clamp logic from `DynamicStartZoneLimiter`.
4. Optional: hide on-screen sticks on desktop.

### Exit

- Same level completable on mouse+WASD and on touch joysticks.

---

## Phase 5 — Campaign, tutorial Level 1, kill legacy

1. Main menu → **linear level map** (slots, stars, locked).
2. `ProgressService` local (PlayerPrefs/JSON) matching save schema; swap to Yandex later.
3. Arrow unlock table: e.g. Magic @ clear Level 3, Fire @ 5, Ice @ 7, Piercing @ 9 (tune in SPEC/data).
4. Rebuild **Level 1** as teach level (move → jump → shoot → kill); remove `TrainingManager` / training coins / coin HUD.
5. Delete or disable `Economy/*` coin flow; strip Start menu coin refs.
6. Put all shipped levels in Build Settings.

### Exit

- Fresh profile: only Level 1 + Normal → finish → map unlocks Level 2; no coin UI.

---

## Phase 6 — Yandex Games + i18n

1. Integrate Yandex Games SDK (WebGL template / plugin as chosen).
2. Implement `YandexServices`: init, `GetLang`, cloud get/set for progress, interstitial, rewarded.
3. Rewarded: after lose, once per attempt, +1 to selected available type.
4. Interstitial: on restart and when leaving level to map / next.
5. Locale tables RU/EN/TR; bind HUD/map/buttons; fallback rules from DESIGN.
6. Safe area / canvas scaler pass for iframe.

### Exit

- WebGL build runs in Yandex debug / draft: lang switches, progress persists, ads callbacks fire (test stubs OK in Editor).

---

## Phase 7 — Content & soft-launch polish

1. Author **8–15** levels (scene + `LevelConfig` each); enemy variants (shield / thick).
2. Balance arrow budgets and ★ thresholds.
3. Audio pass, death/win UX, restart flow.
4. Performance: WebGL (physics sleep, limit debris).
5. Store draft: RU/EN + TR texts, icon/cover, age rating.
6. Soft-launch checklist (SPEC § Acceptance).

### Exit

- Candidate build uploaded to Yandex draft; internal playtest of full campaign path.

---

## Suggested order of PRs / agent tasks

```
P1 LevelConfig + LevelController + budget on Wood_Bow
P2 ArrowTypeConfig + pierce/magic + shield durability
P3 Fire/Ice
P4 AimInput desktop + barrier VFX
P5 Map + Progress + gut coins/training + Level_01 teach
P6 Yandex SDK + i18n + ads
P7 Levels 02–N + balance + WebGL ship
```

## Risks

| Risk | Mitigation |
|------|------------|
| Bow script is monolithic | Extract `AimInput` / spawn only; don’t rewrite animation in P1 |
| Physics settle false lose/win | Tunable settle window + “any Enemy alive” recheck |
| SDK WebGL-only | Interfaces + Editor mocks from P1 |
| Scope creep (new races, shop) | Refuse unless DESIGN updated |
| Per-type budgets hard to tutorial | Level 1 = Normal only; introduce types one level at a time |

## Tracking

Update checkboxes in this file as phases complete. If DESIGN changes, update SPEC first, then PLAN dates/order.
