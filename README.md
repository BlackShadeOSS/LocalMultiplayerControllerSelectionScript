# LocalMultiplayerControllerSelectionScript

## About the script
LMCSS is a script that dynamicly updates a list of toggles so user can select a input device. It also makes sure that only one player at the time uses a specific device.
I tried to write this thing as best as I could but C# is not a language that I am used to. **This was made in Unity 2022.3.13f**

This repo contains a example scene that uses this script. So it should be a good starting point.
You can see the example scene bellow.
![image](https://github.com/user-attachments/assets/bcc2cdb5-63b2-4a94-b0e6-6a5b0a0bd6cc)

## How to use
It is quite simple to use, just add the LocalMultiplayerControllerSelection.cs to canvas object and fill the required things
![image](https://github.com/user-attachments/assets/429d4d8d-f2bc-4afb-a7c8-da97d2d80df6)

### Thing to fill and what are they
- **Example Toggle** - here you put a game object of a toggle, all created toggles will be looking like this one
- **Player Toggle Groups** - here you put a empty game object that will a group for toggles, so you need 1 per player
- **Start Button** - here you put a game object of a button that will be activated when all players select a device

### Thing to keep in mind

1. Start button needs to have Interactable set to false
2. This script uses a normal text for toggle not TextMesh Pro text, but it should be easy to change
