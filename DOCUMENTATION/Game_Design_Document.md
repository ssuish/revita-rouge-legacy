# Game Design Document - Revita Rogue

## Game Overview

**Title**: Revita Rogue  
**Genre**: 2D Survival Action-Adventure  
**Platform**: Mobile (Unity)  
**Target Audience**: Players who enjoy survival games with exploration and resource management  

## Core Game Loop

1. **Explore** the post-apocalyptic world
2. **Gather** resources (dates, legumes, salvage materials)
3. **Craft** essential survival items and tools
4. **Complete** quests to progress the story
5. **Battle** enemies and bosses for rewards
6. **Upgrade** your automata character
7. **Survive** by managing health and stamina

## Game Mechanics

### Survival Systems

#### Health System
- **Heart-based display**: 8 maximum hearts
- **Damage sources**: Enemy attacks, environmental hazards
- **Healing**: Consumable food items restore health
- **Death penalty**: Complete inventory loss and respawn at base

#### Stamina/Hunger System
- **Hunger bars**: 8 maximum hunger points
- **Depletion**: Sprinting consumes hunger over time
- **Recovery**: Food items restore stamina
- **Sprint limitation**: Cannot sprint when hunger is depleted

### Resource Management

#### Gathering Mechanics
- **Hold-to-gather**: Press and hold to collect resources
- **Timed collection**: 3-second gathering time
- **Experience reward**: 5-10 XP per item collected
- **Resource types**:
  - **Dates**: Found on trees, used for health recovery
  - **Legumes**: Ground-based plants, used for stamina recovery
  - **Salvage**: Found in dumps, used for crafting materials

#### Inventory System
- **Grid-based slots**: Fixed number of inventory spaces
- **Item stacking**: Multiple quantities per slot
- **Persistent storage**: Saves between game sessions
- **Strategic management**: Limited space requires planning

### Combat System

#### Player Combat
- **Projectile-based**: Launch snares at enemies
- **Directional aiming**: Attacks follow movement direction
- **Ammunition**: Requires snare items from inventory
- **Animation timing**: Attack has startup and recovery frames

#### Enemy Behavior
- **Patrol patterns**: Enemies follow predetermined paths
- **Detection system**: Chase player when within range
- **Attack patterns**: Melee damage with cooldown periods
- **Health system**: Multiple hits required to defeat

#### Boss Encounters
- **Boss 2 - Seitune**: Mid-game challenge
- **Boss 3 - Hara**: End-game encounter
- **Unique mechanics**: Each boss has special attack patterns
- **Rewards**: Significant experience and story progression

### Progression Systems

#### Experience & Leveling
- **Level cap**: Maximum level 20
- **XP sources**: Gathering resources, defeating enemies
- **Experience values**: 5-10 XP per gathered item
- **Progression tracking**: Persistent across sessions

#### Quest System
- **Story quests**: Main narrative progression
- **Daily quests**: Recurring objectives for rewards
- **Quest types**: Gathering, combat, exploration
- **State management**: Tracks completion and progress

#### Automata Upgrades
- **Visual progression**: Character sprite improvements
- **Upgrade points**: Earned through gameplay achievements
- **Enhancement system**: Unlocks new abilities and appearance

### Crafting System

#### Recipe Mechanics
- **Pattern matching**: Specific item combinations create recipes
- **Resource consumption**: Uses materials from inventory
- **Crafting slots**: Dedicated UI for recipe assembly
- **Success validation**: Checks recipe patterns before crafting

#### Craftable Items
- **Tools**: Enhanced gathering and combat equipment
- **Consumables**: Health and stamina restoration items
- **Equipment**: Improved survival gear
- **Special items**: Story-specific crafted objects

## World Design

### Environment Systems

#### Day/Night Cycle
- **Real-time progression**: Time advances continuously
- **Dynamic lighting**: Environment lighting changes with time
- **Streetlights**: Automatic illumination during night hours
- **Player flashlight**: Manual light source for dark areas

#### Scene Structure
- **Connected areas**: Multiple explorable zones
- **Waypoint system**: Travel between different locations
- **Scene persistence**: World state saves between sessions
- **Progressive unlocking**: New areas unlock through story progression

### Interactive Elements

#### Resource Points
- **Renewable sources**: Resources respawn over time
- **Visual indicators**: Clear markers for gatherable items
- **Depletion mechanics**: Some sources have limited yields
- **Strategic placement**: Resources distributed across world

#### NPCs & Dialogue
- **Quest givers**: Characters who provide objectives
- **Dialogue system**: Text-based conversations
- **Story delivery**: Narrative progression through interactions
- **Dynamic responses**: Dialogue changes based on progress

#### Environmental Objects
- **Chests**: Contain valuable items and resources
- **Switches**: Control gates and access to new areas
- **Interactive elements**: Objects that respond to player input
- **Destructible items**: Objects that can be damaged or destroyed

## Technical Systems

### Save System
- **JSON serialization**: Structured data storage
- **Persistent data**: Inventory, progress, and world state
- **Automatic saving**: Progress saved at key checkpoints
- **Manual save points**: Player-initiated save locations

### Input System
- **Multi-platform support**: Keyboard, gamepad, and touch
- **Customizable controls**: Players can remap inputs
- **Context-sensitive**: Different controls for different game states
- **Accessibility features**: Support for various input methods

### Audio Design
- **Environmental audio**: Contextual sound effects
- **Player feedback**: Audio cues for actions and events
- **Music system**: Dynamic soundtrack based on game state
- **3D audio**: Spatial sound for immersive experience

## User Interface

### HUD Elements
- **Health display**: Visual heart-based health indicator
- **Stamina bars**: Hunger/stamina visualization
- **Experience tracker**: Current level and XP progress
- **Inventory preview**: Quick access to carried items

### Menu Systems
- **Main menu**: Game start, settings, and exit options
- **Pause menu**: In-game pause with save and quit options
- **Settings menu**: Audio, controls, and graphics options
- **Quest log**: Objective tracking and progress display

### Mobile Adaptations
- **Touch controls**: Virtual joystick and action buttons
- **UI scaling**: Interface adapts to different screen sizes
- **Gesture support**: Touch-specific interaction methods
- **Responsive design**: Optimized for mobile gameplay

## Narrative Design

### Setting & Atmosphere
- **Post-apocalyptic world**: Desolate wasteland environment
- **Survival theme**: Emphasis on resource scarcity and danger
- **Hope through progress**: Character improvement and world restoration
- **Mystery elements**: Discovering the world's history through exploration

### Character Development
- **Player avatar**: Customizable automata character
- **Progression arc**: From survivor to capable explorer
- **Skill development**: Growing abilities through experience
- **Visual evolution**: Character appearance improves with upgrades

### Story Structure
- **Introduction phase**: Tutorial and basic survival
- **Exploration phase**: World discovery and resource gathering
- **Challenge phase**: Boss encounters and difficult quests
- **Resolution phase**: Story conclusion and character mastery

## Balancing & Difficulty

### Progression Pacing
- **Gradual difficulty increase**: Challenges scale with player ability
- **Resource availability**: Balanced scarcity without frustration
- **Quest complexity**: Objectives become more involved over time
- **Skill gate progression**: New abilities unlock access to content

### Player Support Systems
- **Death recovery**: Meaningful but not punishing death penalty
- **Resource regeneration**: Renewable sources prevent permanent failure
- **Multiple solutions**: Various approaches to challenges
- **Optional content**: Side activities for struggling players

This design document serves as a comprehensive guide to Revita Rogue's gameplay systems, mechanics, and intended player experience.
