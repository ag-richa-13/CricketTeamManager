# Cricket Team Management Screen

## Overview

This project is an interactive Cricket Team Management Screen developed in Unity using C# and Unity UI (uGUI).

The screen allows users to manage a Playing XI by selecting players from an owned-player collection, removing selected players, assigning a captain, monitoring team composition, and viewing the team's overall rating.

The implementation focuses on clear UI presentation, data-driven player cards, gesture handling, team validation, and separation of player data, team logic, and UI presentation.

## Features

### User Interface

- Landscape-oriented layout with the Playing XI on the left and owned player cards on the right.
- Team rating and player composition counts.
- Scrollable Playing XI and owned-player collection.
- Player cards display name, role, overall rating, batting rating, and bowling rating.
- Playing XI entries display player information and captain status.
- Selected-player and captain indicators.
- Empty-state messages when the relevant collection has no visible players.
- Team status text that updates as the Playing XI changes.

### Player Data

- Player information is stored using ScriptableObjects.
- Each player has a unique ID, name, role, overall rating, batting rating, bowling rating, country flag, and role icon.
- Ratings use integer values from 0 to 100.
- Player cards and Playing XI entries are generated from the player database.
- The player dataset can be edited independently of the UI.

### Player Selection

- Long-press an owned player card to add that player to the Playing XI.
- Long-press a Playing XI entry to remove that player.
- Players remain in the owned collection after selection or removal.
- Duplicate selections are prevented.
- The Playing XI is limited to a maximum of 11 players.
- Selected-player indicators update with team changes.

### Captain Selection

- Double-tap a Playing XI entry to assign that player as captain.
- Only one captain can be selected at a time.
- Selecting a different captain replaces the previous captain.
- Double-tapping the current captain leaves the selection unchanged.
- Removing the captain clears the captain selection without automatically assigning another player.
- Captain status is displayed in the Playing XI and on the corresponding owned-player card.

### Team Validation

A valid Playing XI must satisfy all of the following conditions:

- Exactly 11 distinct players.
- At least one wicketkeeper.
- At least four specialist bowlers.
- Exactly one selected captain.

Only players with the `Bowler` role count toward the specialist-bowler requirement. All-rounders do not count toward this minimum.

The team can be edited while incomplete or invalid. The UI displays live player and role counts and updates the team status as the selection changes.

### Team Rating

The team rating is the arithmetic mean of the selected players' overall ratings, rounded to the nearest integer, with `.5` rounded up.

- Only overall ratings contribute to the team rating.
- The divisor is the current number of selected players.
- An empty Playing XI has a rating of 0.
- Adding or removing a player recalculates the rating.
- Captain selection does not affect the rating.

### Additional UI

The screen includes Gold, Silver, and Bronze player-rating categories with filtering. These are additional features and do not affect team validation or team rating.

## Requirements

- Unity Editor: Unity 6.5
- Language: C#
- UI framework: Unity UI (uGUI)
- Text rendering: TextMeshPro
- Input: Mouse input in the Unity Editor and touch input, subject to the project's configured input handling.

## Dependencies

| Dependency              | Purpose                                          |
| ----------------------- | ------------------------------------------------ |
| Unity UI (uGUI)         | Screen layout, buttons, images, and Scroll Views |
| TextMeshPro             | UI text rendering                                |
| Unity ScriptableObjects | Editable player data and player database         |
| Unity EventSystem       | UI pointer and interaction events                |

No paid assets, backend services, or external team-management packages are required.

If the project uses additional packages, list their exact names and versions here.

## Project Structure

The project is organized into the following main areas:

- `Assets/Scripts/` — Player data, team-management logic, UI binding, and gesture handling.
- `Assets/Prefab/` — Reusable player card and Playing XI entry prefabs.
- `Assets/Sprites/` — Country flags and role icons.
- `Assets/Data/` — Player database and generated player ScriptableObject assets.
- `Assets/Scenes/` — Project scenes, including the entry scene.

Folder names may vary slightly depending on the final project structure.

## Setup Instructions

1. Install the Unity Editor version specified above.
2. Open the project through Unity Hub.
3. Allow Unity to import the project assets and packages.
4. Open the entry scene: TeamManagementScene in Assets/Scenes.
5. Confirm that the player database, UI references, prefabs, and sprites are assigned in the Inspector.
6. Enter Play Mode to run the Cricket Team Management Screen.

## Controls

| Input                           | Action                                      |
| ------------------------------- | ------------------------------------------- |
| Long-press an owned player card | Add the player to the Playing XI            |
| Long-press a Playing XI entry   | Remove the player from the Playing XI       |
| Double-tap a Playing XI entry   | Select the player as captain                |
| Scroll over either collection   | Browse the available players or selected XI |

Gesture thresholds are configurable through the gesture handler Inspector fields. Record the final configured values before submission.

## Verification Results

Complete these checks in the final Unity build before submission.

- [x] Owned-player collection displays all database players.
- [x] At least 20 distinct players are available.
- [x] At least two wicketkeepers and six specialist bowlers are available.
- [x] Adding a player updates the Playing XI, counts, and rating.
- [x] Duplicate additions are prevented.
- [x] The Playing XI cannot exceed 11 players.
- [x] Removing a player keeps that player in the owned collection.
- [x] Captain selection and replacement work correctly.
- [x] Removing the captain clears captain selection.
- [x] A valid team requires 11 players, one or more wicketkeepers, four or more specialist bowlers, and exactly one captain.
- [x] All-rounders do not count toward the specialist-bowler minimum.
- [x] Invalid teams are not displayed as ready.
- [x] Team rating uses the correct average and rounding rules.
- [x] Mouse and touch gesture behavior has been tested.
- [x] Scrolling does not accidentally add, remove, or assign a captain.
- [x] The UI has been checked at 16:9 and a wider landscape resolution.


## Known Limitations

- Team selections are not persisted between application sessions.
- No backend or online synchronization is implemented.
- The player dataset and ratings are illustrative game data.
- The project is designed around the assignment's specified team-management behavior.

Add any remaining known issues here. If no known issues remain after testing, state that instead.

## Development Time

Approximate development time: 2 days

The development period included UI construction, player data setup, team-management logic, gesture handling, validation, testing, and documentation.

## AI Assistance

AI tools were used for occasional technical guidance, code-related suggestions, and documentation support during development. The project was implemented, integrated, and tested in Unity.

## Submission Contents

The project is submitted as a repository or ZIP archive containing:

- `Assets/`
- `Packages/`
- `ProjectSettings/`
- `README.md`

Generated folders such as `Library/`, `Temp/`, and `Logs/` are excluded from the submission.

