# Skylines-ElevatedTrainStationTrack
Elevated Train Station Track mod for Cities: Skylines

## Local paths
The .csproj file has references to the game files and Unity in your game directory, but it expects them to be in a special Games folder.  To make those references work without changing the csproj file, you can add a symbolic link to make that Games folder just redirect to wherever your game files actually are.
1. Open a command prompt (`cmd.exe`)
2. Go to wherever you cloned this repository (e.g. `cd D:\Source\GitHub` or `cd C:\Users\me\Documents\GitHub`)
3. Run `mklink /D "..\..\..\..\..\Games" "C:\Program Files (x86)"`