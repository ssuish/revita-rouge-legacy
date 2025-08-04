# Revita Rogue - A 2D Post-Apocalyptic Survival Game

## Table of Contents

- [Overview](#overview)
- [Game Features](#game-features)
- [Gameplay Mechanics](#gameplay-mechanics)
- [System Architecture](#system-architecture)
- [Installation & Setup](#installation--setup)
- [Controls](#controls)
- [Game Systems](#game-systems)
- [Development Information](#development-information)
- [License](#license)

## Overview

Revita Rogue is a 2D post-apocalyptic survival game built with Unity. Players navigate a dangerous wasteland, collecting resources, crafting items, completing quests, and battling enemies while managing health, stamina, and hunger systems. The game features day/night cycles, an inventory system, quest mechanics, and boss battles.

You can play this game at <a href="https://kofeejan.itch.io/revita-rogue-capstone-project-showcase"><img src="https://img.shields.io/badge/Play_on_Itch-F85A5A"></a></a>.

Watch Revita Rogue's short gameplay at: <a href="https://drive.google.com/file/d/1Zdr1VPkYVe_WQVV2_kFwQjeICcE6mVRa/view?usp=sharing"><img src="https://img.shields.io/badge/YouTube-F85A5A"></a></a>.

## Game Features

### Core Gameplay

- **Post-Apocalyptic Setting**: Explore a dangerous wasteland filled with resources and enemies
- **Survival Mechanics**: Manage health, stamina/hunger, and inventory
- **Resource Gathering**: Collect dates, legumes, and salvage materials from dumps
- **Crafting System**: Create tools and items using collected resources
- **Quest System**: Complete objectives and progress through the story
- **Combat System**: Battle enemies and bosses using projectile weapons

### World Systems

- **Day/Night Cycle**: Dynamic lighting system with streetlights and player flashlight
- **Real-Time Clock**: In-game time tracking with save/load functionality
- **Scene Management**: Multiple areas connected through waypoints and triggers
- **Automata Upgrade System**: Enhance your character's abilities

## Gameplay Mechanics

### Player Systems

- **Movement**: 8-directional movement with sprinting capability
- **Health System**: Heart-based health with consumable healing items
- **Stamina/Hunger**: Hunger bars that deplete during sprinting
- **Inventory Management**: Grid-based inventory with item stacking
- **Flashlight**: Toggle-able light source for dark areas
- **Gathering**: Hold-to-gather system for collecting resources

### Combat

- **Projectile-Based**: Launch snares and other projectiles at enemies
- **Enemy AI**: Intelligent enemies with patrol patterns and chase behavior
- **Boss Battles**: Multiple boss encounters with unique behaviors
- **Damage System**: Health-based combat with death/respawn mechanics

### Progression

- **Experience System**: Gain XP from gathering and combat (up to level 20)
- **Quest Completion**: Story and daily quest systems
- **Item Progression**: Craft better tools and consumables
- **Automata Upgrades**: Enhance character sprites and abilities

## System Architecture

### Core Systems

#### Player Controller (`Player.cs`)

- Central player management class handling movement, combat, health, and interactions
- Integrates with input system, inventory, and quest systems
- Manages animations, audio, and UI interactions

#### Inventory System

- **InventoryController**: Manages item storage, saving/loading, and item usage
- **Item Database**: Centralized item definitions and properties
- **Crafting Manager**: Handles recipe validation and item creation
- **Pickup System**: Automatic item collection with gathering mechanics

#### Quest System

- **Quest Manager**: Handles quest progression and state management
- **Quest Data**: Scriptable objects defining quest parameters
- **Daily Quests**: Time-based recurring objectives
- **Quest UI**: Log and progression tracking interfaces

#### World Systems

- **WorldTime**: Real-time clock with day/night cycles
- **Lighting System**: Dynamic light management for time of day
- **Scene Management**: Level transitions and save state persistence
- **Audio Management**: Context-aware sound effects and music

### Input System

- Unity's New Input System with customizable controls
- Support for keyboard, gamepad, and touch inputs
- Action mapping for movement, combat, interaction, and UI

## Installation & Setup

### Prerequisites

- Unity 2021.3 LTS or later
- Unity Input System package
- Visual Studio or preferred C# IDE

### Setup Instructions

1. Clone or download the repository
2. Open the project in Unity
3. Ensure all required packages are installed
4. Build and run the project

### Project Structure

```
Code/
├── Boss3Behaviour/          # Boss AI and behavior
├── ButtonsInventoryHandler/  # UI button management
├── CanvasButtons/           # Canvas and menu systems
├── Crafting/                # Item crafting system
├── Enemy/                   # Enemy AI and behaviors
├── Events/                  # Event management system
├── Health/                  # Health system
├── InputActions/            # Input handling
├── Inventory/               # Inventory management
├── Misc/                    # Utility classes
├── QuestSystem/             # Quest management
├── Worldtime and lights/    # Time and lighting systems
└── ...
```

## Controls

### Default Controls

- **Movement**: WASD / Arrow Keys / Left Stick
- **Sprint**: Hold Shift / Sprint Button / Right Trigger
- **Gather**: Hold Space / Gather Button
- **Attack**: E Key / Attack Button
- **Flashlight**: F Key / Flashlight Button
- **Use Health Item**: H Key / Health Button
- **Use Stamina Item**: T Key / Stamina Button
- **Pause**: Escape / Pause Button
- **Quest Log**: Q Key / Quest Button
- **Inventory**: I Key / Inventory Button

### Mobile Controls

- Virtual joystick for movement
- On-screen buttons for actions
- Touch-friendly UI elements

## Game Systems

### Resource Management

- **Health**: Managed through consumable items and combat damage
- **Stamina/Hunger**: Depletes during sprinting, restored with food items
- **Inventory Space**: Limited slots requiring strategic item management

### Progression Systems

- **Experience Points**: Gained from gathering (5-10 XP per item)
- **Level Cap**: Maximum level 20
- **Death Penalty**: Inventory clearing and respawn at base
- **Save System**: Persistent progress across play sessions

### World Interaction

- **Gathering Points**: Dates, legumes, and dump sites
- **NPCs**: Dialogue and quest interactions
- **Environmental Objects**: Chests, switches, and interactive elements
- **Scene Transitions**: Waypoint-based area traversal

### Enemy Types

- **Basic Enemies**: Patrol-based AI with chase behavior
- **Boss Enemies**: Seitune (Boss 2) and Hara (Boss 3)
- **Combat Mechanics**: Projectile-based combat system
- **Experience Rewards**: XP gained from defeating enemies

## Development Information

### Technical Details

- **Engine**: Unity 2021.3 LTS
- **Language**: C#
- **Input System**: Unity Input System
- **Save System**: JSON-based persistent data
- **Audio**: Unity Audio Source with spatial sound
- **Graphics**: 2D sprite-based rendering

### Code Architecture

- **Singleton Patterns**: Used for managers (InventoryController, etc.)
- **Event System**: Centralized event management through GameEventsManagerSO
- **Scriptable Objects**: Data-driven design for items, quests, and configuration
- **Component-Based**: Modular systems with clear separation of concerns

### Performance Considerations

- Object pooling for projectiles and effects
- Efficient inventory management with lazy loading
- Optimized lighting system for day/night cycles
- Memory-conscious save/load operations

### Future Development

The codebase is structured to support:

- Additional boss encounters
- Extended crafting recipes
- New resource types and gathering mechanics
- Enhanced quest system with branching narratives
- Multiplayer functionality (with architectural modifications)

## License

Copyright (c) 2025 [Revita Rogue]. All rights reserved.

This code is provided for educational and reference purposes only. See the LICENSE file for full terms and conditions.

---

_This documentation reflects the current state of the Revita Rogue codebase as of August 2025._
