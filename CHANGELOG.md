# Changelog - Revita Rogue

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

- Comprehensive documentation system
- API reference documentation
- Technical implementation guides
- Setup and deployment instructions

### Changed

- Improved README with detailed feature descriptions
- Enhanced code organization and structure

## [1.0.0] - 2024-08-04

### Added

#### Core Gameplay Systems

- Player movement with 8-directional controls
- Health system with heart-based UI display (8 maximum hearts)
- Stamina/hunger system affecting sprint capabilities
- Experience and leveling system (maximum level 20)
- Death and respawn mechanics with inventory clearing

#### Resource Management

- Gathering system for three resource types:
  - Dates (health restoration items)
  - Legumes (stamina restoration items)
  - Salvage materials (crafting components)
- Hold-to-gather mechanic with 3-second collection time
- Experience rewards for gathering (5-10 XP per item)

#### Inventory System

- Grid-based inventory with limited slots
- Item stacking and quantity management
- Persistent inventory saving/loading
- Health and stamina item consumption
- Item database for centralized item management

#### Crafting System

- Recipe-based crafting with pattern matching
- Crafting slots for item combination
- Recipe validation and success feedback
- Consumable and tool creation

#### Combat System

- Projectile-based combat using snare ammunition
- Directional attack system following player movement
- Enemy AI with patrol and chase behaviors
- Boss encounters (Seitune and Hara)
- Combat experience rewards

#### Quest System

- Story quest progression
- Daily quest system
- Quest state management and saving
- Quest log UI with progress tracking
- Multiple quest types (gathering, combat, exploration)

#### World Systems

- Real-time day/night cycle
- Dynamic lighting system with streetlights
- Player flashlight with directional control
- Multiple connected game areas
- Scene transition system with waypoints
- Save state persistence across scenes

#### User Interface

- Heart-based health display
- Hunger/stamina bar visualization
- Experience and level tracking
- Inventory management interface
- Quest log and progression tracking
- Pause menu with settings and save options
- Mobile-friendly touch controls

#### Audio System

- Environmental audio for different actions
- Walking and sprinting sound effects
- Gathering and combat audio feedback
- Contextual audio management

#### Character Progression

- Automata upgrade system
- Visual character progression
- Sprite enhancement mechanics
- Achievement tracking

### Technical Implementation

#### Input System

- Unity Input System implementation
- Multi-platform input support (keyboard, gamepad, touch)
- Customizable control mapping
- Context-sensitive input handling

#### Save System

- JSON-based data serialization
- Persistent data storage
- Inventory state preservation
- Quest progress tracking
- World state management

#### Architecture

- Singleton pattern for core managers
- Event-driven system architecture
- Component-based entity design
- Modular system organization

#### Performance

- Object pooling for projectiles
- Efficient UI update mechanisms
- Optimized resource loading
- Memory-conscious save operations

### Code Organization

#### Directories Structure

- `Boss3Behaviour/`: Boss AI and combat systems
- `ButtonsInventoryHandler/`: UI button management
- `CanvasButtons/`: Menu and canvas systems
- `Crafting/`: Item crafting implementation
- `DatesandLegumesSpawner/`: Resource spawning systems
- `Enemy/`: Enemy AI and behaviors
- `Events/`: Centralized event management
- `Health/`: Health system components
- `InputActions/`: Input handling and mapping
- `Inventory/`: Inventory management systems
- `QuestSystem/`: Quest progression and management
- `Worldtime and lights/`: Time and lighting systems

#### Key Classes

- `Player.cs`: Main player controller with all player functionality
- `InventoryController.cs`: Singleton inventory management
- `CraftingManager.cs`: Recipe validation and item creation
- `Quest.cs`: Individual quest instance management
- `AIchase.cs`: Base enemy AI behavior
- `GameEventsManagerSO.cs`: Central event distribution

### Platform Support

- Windows PC (primary target)
- macOS compatibility
- Linux support
- Mobile platform preparation

### Dependencies

- Unity 2021.3 LTS
- Unity Input System package
- Unity 2D Animation package
- Unity Audio system

## Development Notes

### Design Philosophy

- **Survival Focus**: Resource scarcity drives gameplay decisions
- **Progressive Difficulty**: Challenges scale with player advancement
- **Exploration Rewards**: Hidden resources and quest opportunities
- **Death Consequences**: Meaningful but not punishing death penalty

### Performance Targets

- 60 FPS on target hardware
- Sub-second loading times
- Minimal memory footprint
- Stable performance across platforms

### Future Considerations

- Multiplayer architecture foundation
- Extensible quest system
- Modular crafting recipes
- Enhanced AI behaviors
- Additional boss encounters

---

**Note**: This project represents a complete 2D survival game implementation with comprehensive systems for player progression, resource management, and world interaction. The codebase is structured for maintainability and future expansion.
