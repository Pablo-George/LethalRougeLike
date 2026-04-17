# Tasks: Hunger System

**Feature**: Hunger System  
**Spec**: [spec.md](spec.md)  
**Plan**: [plan.md](plan.md)

## Summary

Implement hunger system for Lethal Company - a survival mechanic where players manage hunger by buying/eating food.

- **Total Tasks**: 15
- **Phases**: 7

## Phase 1: Setup

Project initialization and directory setup.

- [ ] T001 Create src/Hunger/ directory structure per plan.md

## Phase 2: Foundational

Core data components that block all user stories.

- [ ] T002 [P] Create HungerData component class in LethalCompanyMod/src/Hunger/HungerData.cs
- [ ] T003 Implement HungerConfig for tunable parameters in LethalCompanyMod/src/Config/ModConfig.cs

## Phase 3: User Story 1 - View Hunger Level (P1)

Hunger bar UI in top-right corner.

- [ ] T004 [P] [US1] Create HungerUI bar component in LethalCompanyMod/src/Hunger/HungerUI.cs
- [ ] T005 [US1] Implement Update() to sync hunger value to UI bar width in LethalCompanyMod/src/Hunger/HungerUI.cs
- [ ] T006 [US1] Add hunger bar to HUD canvas at start of game in LethalCompanyMod/src/Hunger/HungerHooks.cs

## Phase 4: User Story 3 - Hunger Decreases Over Time (P1)

Hunger depletes during gameplay.

- [ ] T007 [P] [US3] Implement hunger depletion in Update() loop in LethalCompanyMod/src/Hunger/HungerManager.cs
- [ ] T008 [US3] Integrate HungerManager with player spawn in LethalCompanyMod/src/Hunger/HungerHooks.cs

## Phase 5: User Story 2 - Eat Food (P2)

Consume food from inventory to restore hunger.

- [ ] T009 [P] [US2] Create FoodItem class with hunger restore value in LethalCompanyMod/src/Hunger/FoodItem.cs
- [ ] T010 [US2] Implement ConsumeFood() method to restore hunger in LethalCompanyMod/src/Hunger/HungerManager.cs
- [ ] T011 [US2] Add food consumption keybind/button handling in LethalCompanyMod/src/Hunger/HungerHooks.cs

## Phase 6: User Story 4 - Starvation at Zero Hunger (P1)

Take damage when hunger reaches 0%.

- [ ] T012 [P] [US4] Implement starvation damage dealing when hunger is 0 in HungerManager.Update() in LethalCompanyMod/src/Hunger/HungerManager.cs
- [ ] T013 [US4] Stop starvation damage immediately when food is consumed in LethalCompanyMod/src/Hunger/HungerManager.cs

## Phase 7: User Story 5 - Buy Food from Store (P2)

Purchase food items from in-game store.

- [ ] T014 [P] [US5] Add food items to store inventory in LethalCompanyMod/src/Hunger/StoreHooks.cs
- [ ] T015 [US5] Handle purchase transaction to add food to player inventory in LethalCompanyMod/src/Hunger/StoreHooks.cs

## Phase 8: Polish

Cross-cutting concerns and refinement.

- [ ] T016 Add config options for hunger depletion rate in LethalCompanyMod/src/Config/ModConfig.cs
- [ ] T017 Add multiplayer sync for hunger value in LethalCompanyMod/src/Hunger/HungerHooks.cs

---

## Dependencies

| From | To | Type |
|------|-----|------|
| T001 (Setup) | All | Blocks |
| T002 (Foundation) | US1, US3, US2, US4 | Blocks |
| US1 (View Hunger) | T006 | Requires |
| US3 (Hunger Decrease) | T008 | Requires |
| US2 (Eat Food) | T010-T011 | Requires food (T009) |
| US4 (Starvation) | T012-T013 | Requires hunger 0 logic |
| US5 (Buy Food) | T014-T015 | Can be parallel with others |

## Parallel Opportunities

- T002, T003 can run in parallel (different files)
- T004-T006 (US1) can parallel with T007-T008 (US3) after T002 done
- T009-T011 (US2) and T012-T013 (US4) are independent after foundational
- US5 (T014-T015) is independent and can run in parallel with US2/US4

## Independent Test Criteria

| US | Test Criteria |
|----|---------------|
| US1 | Spawn player, verify hunger bar visible in top-right at 100% |
| US2 | Have food, consume, verify hunger increases |
| US3 | Wait 60s, verify hunger decreases |
| US4 | Reduce to 0 hunger, verify taking damage |
| US5 | Buy food from store, verify in inventory |

## MVP Scope

User Story 1 (View Hunger Level) + User Story 3 (Hunger Decreases) = Core hunger loop with visible UI. This is the minimum viable feature that demonstrates hunger system works.