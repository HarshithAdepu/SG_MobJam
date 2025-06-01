# Unity Puzzle Game README

## Overview

This is a Unity-based puzzle game where players slide colored blocks to designated exits to spawn NPCs, who then attack guards and a tower to complete the level. The game features a grid-based system with click-and-drag mechanics, AI pathfinding for NPCs, and a modular level design using ScriptableObjects.

## Technical Details

- **Unity Version**: 6000.0.30f1
- **Build**: Available in the releases section
- **Source**: Clone the project repository
- **Starting Scene**: Open the "MainMenu" scene
- **Play**: Enter Play Mode in Unity to start the game

## Features

- **Click-and-Drag Block Movement**: Move blocks by clicking and dragging them across the grid.
- **Exit Gates for NPC Spawning**: Blocks exiting through designated gates spawn NPCs based on their value.
- **AI Pathfinding for NPCs**: NPCs use Unity’s NavMesh to pathfind towards guards and the tower.
- **ScriptableObjects for Levels**: Levels are stored in ScriptableObjects, allowing easy tweaking and potential JSON-based level loading in the future without rebuilding.
- **Custom Level Storage Tool**: Uses a custom tool (which was partially built by me) for level storage.

## Code Structure

- **GameManager**: Central controller for the scene, managing data communication between systems.
- **TowerManager**: Sets up the tower and its guards.
- **GridManager**: Spawns and manages the playable grid, including blocks and exits.
- **NPCManager**: Handles spawning and behavior of friendly NPCs that attack guards.
- **GameUIManager**: Manages the in-game UI elements.

## Short-Term Improvements

- Replace the current UI with a better asset pack.
- Add comments to code blocks for clarity.
- Improve camera positioning logic for better gameplay visibility.

## Long-Term Improvements

- Fix edge cases in exit logic (currently mitigated through level design).
- Address edge cases in exit gate spawning (also mitigated through level design).
- Rework guard selection logic to enhance gameplay appeal.
- Enable multiplication gates for exit mechanics (not supported in the current implementation).

## Getting Started

1. Clone the project or download the build from releases.
2. Open the project in Unity 6000.0.30f1.
3. Load the "MainMenu" scene.
4. Enter Play Mode to start playing.
