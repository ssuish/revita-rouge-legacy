# Documentation Index - Revita Rogue

Welcome to the complete documentation for Revita Rogue, a 2D post-apocalyptic survival game built with Unity.

## Quick Start

1. **New to the project?** Start with the [README.md](../README.md) for a complete overview
2. **Setting up for development?** Follow the [Setup Guide](Setup_Guide.md)
3. **Need technical details?** Check the [Technical Documentation](Technical_Documentation.md)
4. **Working with the code?** Reference the [API Documentation](API_Reference.md)

## Documentation Structure

### Core Documentation

#### [README.md](../README.md)

**Complete project overview with features and gameplay mechanics**

- Game features and mechanics
- System architecture overview
- Installation and setup instructions
- Controls and gameplay systems
- Development information

#### [Setup Guide](Setup_Guide.md)

**Step-by-step setup instructions for developers**

- Prerequisites and requirements
- Project setup process
- Development workflow
- Common issues and solutions
- Testing checklist

#### [Technical Documentation](Technical_Documentation.md)

**In-depth technical implementation details**

- System architecture
- Performance optimization
- Platform considerations
- Security and debugging
- Build pipeline

#### [API Reference](API_Reference.md)

**Comprehensive code reference and class documentation**

- Core class descriptions
- Method signatures and usage
- Event system overview
- Architecture patterns
- Performance considerations

### Additional Resources

#### [Game Design Document](Game_Design_Document.md)

**Complete game design specification**

- Game mechanics and systems
- User interface design
- Narrative and progression
- Balancing and difficulty

#### [CHANGELOG.md](../CHANGELOG.md)

**Complete development history and version information**

- Version history
- Feature additions
- Technical improvements
- Development milestones

## Game Systems Overview

### Core Systems

- **Player Controller**: Movement, combat, health, and interaction management
- **Inventory System**: Item storage, management, and persistence
- **Crafting System**: Recipe-based item creation and validation
- **Quest System**: Story progression and objective management
- **Combat System**: Projectile-based fighting with enemy AI

### Supporting Systems

- **Input Management**: Multi-platform input handling
- **Save System**: JSON-based data persistence
- **Audio System**: Environmental and feedback audio
- **UI System**: Interface management and user interaction
- **World Systems**: Time, lighting, and scene management

## Development Resources

### Code Organization

```
Code/
├── Boss3Behaviour/         # Boss AI systems
├── Crafting/              # Item crafting logic
├── Enemy/                 # Enemy behaviors
├── Events/                # Event management
├── Inventory/             # Inventory systems
├── QuestSystem/           # Quest management
├── Worldtime and lights/  # Time and lighting
└── ...                    # Additional systems
```

### Key Classes

- `Player.cs` - Main player controller
- `InventoryController.cs` - Inventory management
- `CraftingManager.cs` - Crafting system
- `Quest.cs` - Quest instances
- `AIchase.cs` - Enemy AI behavior

### Architecture Patterns

- **Singleton**: Core managers (InventoryController, CraftingManager)
- **Observer**: Event-driven system communication
- **Component**: Modular entity design

## Getting Help

### Common Tasks

#### Adding New Items

1. Define item in ItemDatabase
2. Add crafting recipes if needed
3. Update inventory display logic
4. Test functionality

#### Creating New Quests

1. Create QuestInfoSO ScriptableObject
2. Define quest steps and requirements
3. Add to quest manager system
4. Implement quest-specific logic

#### Modifying Player Stats

1. Adjust values in Player.cs
2. Update UI displays
3. Balance with game difficulty
4. Test across all systems

### Debugging

- Use Unity Console for error messages
- Add Debug.Log() statements for troubleshooting
- Use Unity Profiler for performance analysis
- Test builds regularly for platform-specific issues

### Performance

- Profile using Unity Profiler
- Monitor memory usage
- Optimize texture sizes
- Use object pooling for frequently created objects

## Contributing

### Code Style

- Follow Unity C# coding conventions
- Use meaningful variable and method names
- Comment complex logic sections
- Maintain consistent indentation

### Testing

- Test all functionality after changes
- Verify save/load operations
- Check performance impact
- Test on target platforms

### Documentation

- Update relevant documentation for changes
- Add comments for new public methods
- Update API reference for new classes
- Maintain changelog entries

## Project Status

**Current Version**: 1.0.0  
**Unity Version**: 2021.3 LTS  
**Platform Support**: Windows, macOS, Linux  
**Development Status**: Feature Complete

## License

This project is licensed under a custom license. See [LICENSE](../LICENSE) for details.

---

This documentation represents the complete Revita Rogue project as of August 2025. For the most current information, always refer to the latest version of these documents and the project's source code.
