## Tournament Leaderboard System

A dynamic tournament leaderboard system built in Unity for mobile games, featuring smooth animations, object pooling, and optimized performance for displaying 1000+ players.

### 🎮 Project Overview

This Unity project implements a high-performance tournament leaderboard designed specifically for mobile games. The system displays player rankings without using Unity's Canvas system, relying instead on SpriteRenderer components and TextMeshPro for optimal performance.

### ✨ Key Features

### Core Functionality

- **Dynamic Leaderboard**: Displays 1000 players with real-time ranking
- **Data-Driven**: Player data loaded from JSON files
- **Player Highlighting**: "Me" player is visually highlighted and centered
- **Smooth Animations**: Fluid transitions when rankings change
- **Update System**: Random score updates with animated rank changes

### Performance Optimizations

- **Object Pooling**: Efficient memory management for UI elements
- **Viewport Culling**: Only visible entries are rendered
- **Mobile Optimized**: Designed for mid-to-low-end mobile devices
- **Portrait Mode**: Optimized for 2436x1125 resolution

### Visual Design

- **Rank-Based Coloring\*\***: Gold, silver, bronze, and default rank colors
- **Smooth Transitions\*\***: DOTween-powered animations
- **Responsive Layout\*\***: Adapts to different screen sizes

### 🛠️ Technical Requirements

### Dependencies

- **Unity 2021.3+**
- **TextMeshPro (Unity Package)**
- **DOTween (Asset Store - Free)**

### Target Platform

- **Mobile Devices: iOS/Android**
- **Screen Resolution: 2436x1125 (Portrait)**
- **Performance: Optimized for mid-to-low-end devices**

### Data Generation

To generate new tournament data:

Select the JSONGenerator object in the scene
Right-click and choose "Generate JSON File" from the context menu
New data will be generated in StreamingAssets/tournament_data.json

### 🎯 Usage

### Basic Operations

- **Startup**: Leaderboard displays automatically with "Me" player centered
- **Update**: Click the UPDATE button to randomly update scores
- **Animation**: Watch as the "Me" player animates to their new rank position

### Customization

- **Player Count**: Modify totalPlayers in JSONGenerator
- **Visible Entries**: Adjust visibleEntries in LeaderboardView
- **Animation Speed**: Change animationDuration in LeaderboardView
- **Colors**: Customize colors in LeaderboardEntry component
