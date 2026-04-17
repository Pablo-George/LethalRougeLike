# Quickstart: Hunger System

## Test Scenarios

### Scenario 1: New Game Shows Hunger Bar

**Steps**:
1. Start a new game with the mod enabled
2. Spawn in the game world

**Expected**: Hunger bar visible in top-right corner showing 100%

---

### Scenario 2: Hunger Decreases Over Time

**Steps**:
1. Start a new game
2. Wait 60 seconds without eating

**Expected**: Hunger bar decreases from 100% to ~90%

---

### Scenario 3: Eat Food Restores Hunger

**Steps**:
1. Start a new game
2. Wait for hunger to decrease to ~50%
3. Use a food item from inventory

**Expected**: Hunger increases by food's restore value

---

### Scenario 4: Starvation at Zero

**Steps**:
1. Start a new game  
2. Wait for hunger to reach 0%
3. Continue playing without eating

**Expected**: Player takes periodic damage (starvation)

---

### Scenario 5: Eat Food Stops Starvation

**Steps**:
1. Let hunger reach 0%
2. Player is taking starvation damage
3. Consume a food item

**Expected**: Starvation damage stops immediately, hunger increases

---

### Scenario 6: Buy Food from Store

**Steps**:
1. Start a new game
2. Have sufficient credits
3. Purchase food from the store

**Expected**: Food added to inventory, credits deducted

---

## Debug Commands

| Action | Command/Key | Expected Result |
|--------|------------|----------------|
| Check hunger value | Inspect player component | Shows 0-100 |
| Force hunger to 0 | Debug menu / mod config | Starvation begins |
| Add food to inventory | Debug menu / console | Food available |
| Toggle UI | Mod config | Show/hide hunger bar |

## Common Issues

| Issue | Likely Cause | Solution |
|-------|--------------|----------|
| Hunger bar not showing | UI not initialized | Check Canvas exists |
| Hunger not decreasing | Update loop not running | Check Update() hook |
| Food not working | Wrong item type | Verify food item parsing |
| Starvation damage not applying | Health hook not patched | Check Harmony patch |
| Multiplayer not syncing | Network sync missing | Add SyncObject |