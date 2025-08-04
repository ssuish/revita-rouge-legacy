# API Reference - Revita Rogue

This document provides a comprehensive reference for the key classes and systems in Revita Rogue.

## Core Classes

### Player (`Player.cs`)

The main player controller that handles all player-related functionality.

#### Key Properties
- `Health`: Current player health (0-8)
- `maxHealth`: Maximum health value
- `currentHunger`: Current stamina/hunger level
- `maxHunger`: Maximum stamina value
- `isGathering`: Whether player is currently gathering resources
- `isSprinting`: Whether player is currently sprinting
- `enemiesKilled`: Number of enemies defeated
- `deathCount`: Number of times player has died

#### Key Methods

**Movement & Actions**
- `Movement()`: Handles player movement input and animations
- `Sprinting()`: Manages sprint functionality and hunger depletion
- `Gathering()`: Processes resource gathering from pickup items
- `Attack()`: Initiates projectile attack sequence

**Health & Survival**
- `TakeDamage(int damage)`: Reduces player health and handles death
- `RecoverHealth(int healthAmount)`: Restores health using consumables
- `UseHealthItem()`: Consumes health items from inventory
- `UseHungerItem()`: Consumes stamina items from inventory

**UI & Interaction**
- `ToggleFlashlight()`: Toggles flashlight on/off
- `OnGatherButtonDown()`: Starts gathering process
- `OnSprintButtonDown()`: Begins sprinting

### InventoryController (`InventoryController.cs`)

Singleton class managing all inventory operations and item storage.

#### Key Properties
- `slots`: Array of inventory slot GameObjects
- `isfull`: Boolean array tracking slot occupancy
- `itemDatabase`: List of all available items in the game

#### Key Methods

**Core Inventory**
- `SaveInventory()`: Persists inventory data to file
- `LoadInventory()`: Restores inventory from saved data
- `CheckInventoryItems()`: Returns dictionary of current items
- `ClearInventory()`: Removes all items from inventory

**Item Management**
- `AddItem(string itemName, int amount)`: Adds items to first available slot
- `RemoveItem(string itemName, int amount)`: Removes specified items
- `HasItem(string itemName)`: Checks if item exists in inventory
- `GetItemCount(string itemName)`: Returns quantity of specific item

**Consumables**
- `HasHealthItem()`: Checks for any health-restoring items
- `HasHungerItem()`: Checks for any stamina-restoring items
- `RemoveHealthItem(int healthValue)`: Removes health item by value
- `RemoveHungerItem(int hungerValue)`: Removes stamina item by value

### CraftingManager (`CraftingManager.cs`)

Singleton managing the crafting system and recipe validation.

#### Key Properties
- `craftSlots`: Array of crafting slot components
- `itemlist`: List of all craftable items
- `recipes`: String array of crafting recipe patterns
- `recipeResults`: Array of items produced by recipes

#### Key Methods
- `CraftItem()`: Validates recipe and creates item if valid
- `ValidateRecipe()`: Checks if current slots match any recipe
- `ClearCraftSlots()`: Empties all crafting slots

### Quest (`Quest.cs`)

Represents individual quest instances with state management.

#### Key Properties
- `info`: ScriptableObject containing quest data
- `state`: Current quest state (enum)
- `currentQuestStepIndex`: Index of current quest step
- `timeRemaining`: Time left for timed quests

#### Key Methods
- `MovetoNextStep()`: Advances quest to next step
- `CurrentStepExists()`: Checks if more steps remain
- `CanBeFinished()`: Determines if quest can be completed

### AIchase (`AIchase.cs`)

Base enemy AI controller with patrol and chase behaviors.

#### Key Properties
- `Health`: Enemy health points
- `moveSpeed`: Movement speed value
- `detectionRange`: Distance at which enemy detects player
- `patrolPoints`: Array of patrol waypoints
- `damage`: Damage dealt to player on attack

#### Key Methods
- `ChasePlayer()`: Moves enemy toward player position
- `PatrolBehavior()`: Handles waypoint-based patrolling
- `AttackPlayer()`: Initiates attack sequence
- `TakeDamage(int damage)`: Processes incoming damage

## Event System

### GameEventsManagerSO

Central event management system using ScriptableObject architecture.

#### Event Categories
- `playerEvents`: Player-related events (death, level up, etc.)
- `questEvents`: Quest progression and completion events
- `itemEvents`: Item pickup, crafting, and usage events
- `inputEvents`: UI and input-related events
- `miscEvents`: General game events and save operations

## Input System

### PlayerActionMap

Unity Input System action map defining all player controls.

#### Action Categories
- **Player Actions**: Movement, attack, gather, sprint
- **UI Actions**: Inventory, quest log, pause menu
- **Utility Actions**: Flashlight, item usage, automata upgrade

## Save System

### SaveLoadManager (`SaveLoadManager.cs`)

Handles game state persistence using JSON serialization.

#### Saved Data
- Elapsed game time
- Last save timestamp
- Inventory contents
- Quest progress
- Player statistics

## Architecture Patterns

### Singleton Pattern
Used by core managers to ensure single instances:
- `InventoryController`
- `CraftingManager`
- `GameEventsManagerSO`

### Observer Pattern
Event system allows loose coupling between systems:
- Quest progress updates
- Player stat changes
- Item interactions

### Component-Based Design
Modular components enable flexible entity composition:
- Separate health, movement, and interaction components
- Reusable scripts across different entity types
- Easy testing and maintenance

## Performance Considerations

### Object Pooling
- Projectiles use pooling to reduce garbage collection
- Audio sources are reused for sound effects

### Efficient Updates
- Movement calculations in FixedUpdate for physics consistency
- UI updates only when values change
- Lazy loading for large data sets

### Memory Management
- Proper disposal of file streams
- Avoiding unnecessary object allocations in Update loops
- Strategic use of coroutines for time-based operations
