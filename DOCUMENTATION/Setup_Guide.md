# Setup Guide - Revita Rogue

This guide will help you set up and run the Revita Rogue project.

## Prerequisites

### Unity Requirements

- **Unity Editor**: 2021.3 LTS or later
- **Unity Hub**: For managing Unity versions
- **Platform modules**: Windows, Mac, or Linux build support

### Development Tools

- **Visual Studio** or **Visual Studio Code** with C# extensions
- **Git** for version control
- **Text editor** for documentation editing

### Hardware Requirements

- **OS**: Windows 10+, macOS 10.15+, or Ubuntu 18.04+
- **Memory**: 4GB RAM minimum (8GB recommended)
- **Storage**: 5GB free space for project files
- **Graphics**: DirectX 11 compatible graphics card

## Project Setup

### 1. Download the Project

```bash
# Clone the repository
git clone https://github.com/your-repo/revita-rouge.git
cd revita-rouge
```

### 2. Unity Setup

1. Open Unity Hub
2. Click "Add" and select the project folder
3. Ensure Unity 2021.3 LTS is installed
4. Open the project (initial import may take several minutes)

### 3. Package Dependencies

The project uses these Unity packages:

- **Input System**: For player controls
- **2D Animation**: For character animations
- **2D Tilemap**: For world building
- **Audio**: For sound management

Unity should automatically resolve these dependencies when opening the project.

### 4. Build Settings

1. Go to **File > Build Settings**
2. Select your target platform
3. Click "Switch Platform" if needed
4. Add scenes from the Scenes folder

## Project Structure

### Core Directories

```
revita-rouge/
├── Assets/                 # Unity project assets
├── Code/                   # C# scripts organized by system
├── DOCUMENTATION/          # Project documentation
├── LICENSE                 # License information
└── README.md              # Main project overview
```

### Code Organization

```
Code/
├── Boss3Behaviour/         # Boss AI and behaviors
├── ButtonsInventoryHandler/ # UI button management
├── CanvasButtons/          # Canvas and menu systems
├── Crafting/               # Item crafting system
├── DatesandLegumesSpawner/ # Resource spawning
├── Enemy/                  # Enemy AI systems
├── Events/                 # Event management
├── Health/                 # Health system
├── InputActions/           # Input handling
├── Inventory/              # Inventory management
├── QuestSystem/            # Quest progression
├── Worldtime and lights/   # Time and lighting
└── ...                     # Additional systems
```

## Running the Game

### In Unity Editor

1. Open the main scene (usually in Assets/Scenes/)
2. Click the **Play** button in the Unity Editor
3. Use keyboard controls or connect a gamepad
4. Test all systems and features

### Building for Distribution

1. **File > Build Settings**
2. Select scenes to include
3. Choose target platform
4. Configure player settings
5. Click **Build** or **Build and Run**

### Testing Builds

1. Test on target platform
2. Verify save/load functionality
3. Check performance and stability
4. Test input methods (keyboard, gamepad, touch)

## Development Workflow

### Code Editing

1. Open scripts in your preferred IDE
2. Make changes and save
3. Return to Unity (auto-compilation)
4. Test changes in Play mode

### Version Control

```bash
# Before making changes
git pull origin main

# After making changes
git add .
git commit -m "Descriptive commit message"
git push origin main
```

### Debugging

- Use Unity Console for error messages
- Add Debug.Log() statements for troubleshooting
- Use Unity Profiler for performance analysis
- Test builds regularly to catch platform-specific issues

## Common Issues & Solutions

### Unity Won't Open Project

- Ensure Unity version compatibility
- Check for corrupted project files
- Try opening with Unity Hub instead of direct file association

### Scripts Won't Compile

- Check for syntax errors in the Console
- Verify all required packages are installed
- Clear Library folder and reimport if needed

### Build Errors

- Check Player Settings for target platform
- Ensure all scenes are added to Build Settings
- Verify platform-specific settings are correct

### Performance Issues

- Use Unity Profiler to identify bottlenecks
- Optimize texture sizes and compression
- Reduce draw calls with sprite atlasing
- Profile on target hardware

## Configuration Files

### Input Actions

- Located in `Code/InputActions/`
- Modify `Revita Rogue Final.inputactions` for control changes
- Regenerate input classes after modifications

### Save Data

- Save files stored in persistent data path
- Location varies by platform:
  - **Windows**: `%USERPROFILE%/AppData/LocalLow/CompanyName/GameName/`
  - **Mac**: `~/Library/Application Support/CompanyName/GameName/`
  - **Linux**: `~/.config/unity3d/CompanyName/GameName/`

### Game Settings

- Modify variables in manager scripts
- Use Unity Inspector for runtime adjustments
- Consider ScriptableObjects for data-driven design

## Customization

### Adding New Items

1. Create item data in ItemDatabase
2. Add crafting recipes if needed
3. Update inventory display logic
4. Test item functionality

### Creating New Quests

1. Create QuestInfoSO ScriptableObject
2. Define quest steps and requirements
3. Add to quest manager system
4. Implement quest-specific logic

### Modifying Player Stats

1. Adjust values in Player.cs
2. Update UI displays accordingly
3. Balance with game difficulty
4. Test across all game systems

## Testing Checklist

### Functional Testing

- [ ] Player movement in all directions
- [ ] Health and stamina systems
- [ ] Inventory management
- [ ] Crafting system
- [ ] Quest progression
- [ ] Save/load functionality
- [ ] All input methods

### Performance Testing

- [ ] Frame rate stability
- [ ] Memory usage within limits
- [ ] Loading times acceptable
- [ ] No memory leaks
- [ ] Smooth animations

### Platform Testing

- [ ] Keyboard controls
- [ ] Gamepad support
- [ ] Touch controls (if applicable)
- [ ] Different screen resolutions
- [ ] Various hardware configurations

## Deployment

### Preparing for Release

1. Finalize all art assets
2. Complete localization if applicable
3. Optimize performance
4. Test thoroughly on target platforms
5. Prepare marketing materials

### Distribution Platforms

- **Steam**: Use Unity Cloud Build or manual upload
- **Itch.io**: Direct file upload
- **Mobile**: Platform-specific stores
- **Console**: Platform certification required

This setup guide should get you up and running with the Revita Rogue project. Refer to the other documentation files for detailed information about specific systems and APIs.
