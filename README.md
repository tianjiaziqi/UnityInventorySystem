# Unity Inventory System

A Unity inventory system project.

Chinese Version: [README.zh-CN.md](README.zh-CN.md)。

## Features

- Grid-based backpack with multi-size item placement
- Item rotation
- Stackable items
- Unlockable backpack cells
- Quick bar binding and selection
- Mouse wheel quick bar switching
- Basic weight calculation with UI feedback
- Data-driven setup through `ScriptableObject`

## What's Implemented

### Runtime

- `InventoryGrid` handles placement checks, overlap detection, auto placement, moving, and stacking
- `PlayerInventory` wraps backpack data, quick bar state, weight, and player-facing operations
- `InventoryManager` acts as the main runtime entry point for other systems and UI
- `InventoryEventCentre` handles inventory and quick bar related events

### UI

- Backpack panel
- Quick bar panel
- Drag and drop item movement
- Placement preview
- Item rotation during drag
- Drag item to quick bar slot to bind

### Sample

- Demo scene: `Assets/Scenes/SampleScene`
- A simple debug GUI is available in the top-left corner for adding items by ID and count

Sample item IDs:

- `001` BlueGem
- `002` Health
- `003` Monitor
- `004` RAM

## Getting Started

### Environment

- Unity `6000.3.9f1`
- URP
- Input System

### Run the Sample

1. Open the project in Unity
2. Open `Assets/Scenes/SampleScene`
3. Enter Play Mode
4. Use the debug GUI in the top-left corner to add items by ID and count

### Default Controls

- `Tab`: open / close backpack
- `R`: rotate current dragged item
- `1` to `0`: select quick bar slot
- Mouse wheel: switch quick bar selection
- Mouse drag: move item / bind item to quick bar

## Project Structure

```text
Assets
├── Configs                  # Item, backpack, quick bar, and view configs
├── Prefabs/UI               # UI prefabs for backpack and quick bar
├── Scenes                   # Sample scene
└── Scripts
    ├── Runtime
    │   ├── Core             # System entry, events, shared config
    │   ├── Data             # ItemDefinition / ItemDatabase / ItemInstance
    │   ├── Inventory
    │   │   ├── Backpack     # Backpack data and backpack UI
    │   │   ├── Common       # Shared grid and manager logic
    │   │   ├── Player       # Player inventory wrapper
    │   │   └── QuickBar     # Quick bar data and UI
    │   └── UI               # Shared panel base
    └── Sample               # Demo input and sample controller scripts
```

## Design Notes

The current structure follows a fairly straightforward split:

- `ItemDefinition` stores static item data
- `ItemInstance` represents a runtime item instance
- `PlacedItem` stores item position, size, and rotation state inside the grid
- `InventoryGrid` owns the core placement rules
- `PlayerInventory` and `InventoryManager` expose a cleaner API to the UI layer

Right now the focus is on getting the full feature loop working first. The next step is to keep tightening the boundaries and make it easier to extend.

## UML Class Diagram

Plan to add it later once the current structure settles a bit more.

## Roadmap

- Abstracting some essential functions under `InventoryManager` to a Interface
- Implement a class for item instance generation.
- Drop, merge, split operations.
- Saving 
- More specific event types

