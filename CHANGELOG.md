# Changelog

## [2.0.0] - 2026-07-09

### Added
- better travel:
    - a new travel mode for NavMeshAgents
    - the old travel mode is now referred to as the "simple" travel mode
    - the simple travel mode now rotates the traveler towards the current target point
- improved waypoint editor:
    - added button handles to change the selected waypoint from within the scene view

### Changes
- renamed member variables with a prefix to distinguish them from parameters and local variables
- waypoints converted from List to Array since they are not meant to be modified at runtime
- improved waypoint labels for better legibility

## [1.2.0] - 2026-07-04

### Added
- custom inspector
    - works similar to the default list inspector
        - reordering
        - adding
        - removing
        - selecting (no multiselect)
    - custom add logic
        - the new point is inserted after the selected point or at the beginning if no point is selected
        - if the new point is inserted between existing points it will be placed halfway between them
        - if the new point is inserted at the end the direction and distance of the two previous points will be maintained
- new option to only show the handle of the selected point
- the handles now respect the editors tool handle rotation (local or global)

## [1.1.0] - 2026-06-30

### Added
- basic Traveler

## [1.0.1] - 2026-06-27

### Added
- Changelog

## [1.0.0] - 2026-06-27

Initial Release

### Added
- a list of waypoints
    - the number of waypoints, their order and positions can be changed from the inspector
    - the points and their connections are visualized with gizmos
- Custom editor
        each point can be moved individually from the scene view
