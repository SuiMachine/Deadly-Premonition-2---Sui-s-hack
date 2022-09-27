# Deadly Premonition 2 - Sui's hack
A hack/mod that utilizes ''MelonLoader'' and ''HarmonyX'' to make up for some of the ports shortcomings.

# List of features
* Resolution override (the game should no longer start in 1920x1080 maximized window).
* Exposed display settings:
   * Resolution
   * V-sync
   * Refresh Rate (or FPS cap)
   * Display Mode (Windowed, Fullscreen, Borderless etc.).
* Exposed quality settings:
   * Antialiasing (Post Process due to deferred path)
   * Anisotropic filtering (and its level)
   * Camera far plane distance
   * Shadow Distance
   * Shadow Quality
   * Shadow Resolution
   * Shadow cascade amount (and its distances)
   * LOD Bias
   * Pixel Light Count
   * Texture Resolution
   * Far Plane Distance
* Option to skip splash screens / intros
* Runtime button prompt replacement with buttons prepared for Xbox and Playstation controllers (make sure to correct bindings in Steam Input).
* Bugfix: Corrected Electrical Wires bounds to be rendered correctly (Fix geometry).
* Bugfix: Corrected terrain meshes for 210 terrain pieces (Fix geometry).
* Movement interpolation.
* A basic GUI available for configuring the mod available under F11.
* Option to disable Edge detection (Outline) filter.
* Options to enable and configure screen-space reflection.
* An option to add shadows to lights used byt the game.
* Options to configure HBAO.
* Option to modify planar reflections resolution.
* Rumble for controller when shooting
* Basic Keyboard and Mouse support* (see notes)

# Experimental features
* Hook for SteamInput to read Keyboard and mouse input instead.

# Requirements
* Original copy of the game
* Melon Loader 0.5+

# Installation (Windows)
* Download [MelonLoader](https://github.com/LavaGang/MelonLoader/releases) and install it (prefably using installer).
* Download the [Sui's Hack](https://github.com/SuiMachine/Deadly-Premonition-2---Sui-s-hack/releases) and extract it to game directory (do not change the file structure SuisHack.dll goes into "Mods", "Prompts" directory to a game's "StreamingAssets" folder etc.)
* Launch the game.
* Press F11 to open configuration GUI or close the game and edit "MelonPreferences.cfg" inside the UserData folder.

# Installation (SteamDeck)
These instructions are written with path assuming you install it on build in drive. They may need to be modified, if installing on SD card.
* Switch to desktop mode.
* Download [MelonLoader](https://github.com/LavaGang/MelonLoader/releases).
* Launch the installer using Protontricks Launcher (if it is missing, install it using Discovery).
* When the screen of protontricks launcher pops up, selected Deadly Premonition 2.
* Select brows and navigate to: `/home/deck/`
* In text field type in `.local` and press enter (it's a hidden folder).
* Further navigate down to: `/home/deck/.local/share/Steam/common/Deadly Premonition 2` (or wherever the games install on SD cards)
* Select `DeadlyPremonition2.exe` and install it.
* Once it is installed download the [Sui's Hack](https://github.com/SuiMachine/Deadly-Premonition-2---Sui-s-hack/releases).
* Navigate to `/home/deck/.local/share/Steam/common/Deadly Premonition 2` (or wherever the games install on SD cards)
* Extract the files from the newly downloaded archive file to that folder.
* Finally right click on the game in Steam libary and choose `Properties`.
* Under launch options paste in the following `WINEDLLOVERRIDES="version.dll=n,b" %command%`
* Launch the game. If the MelonLoader appears you should be all set.


# Bugs
* Resolution is not applied to main window on startup, if focus is on console window.
* Final boss doesn't seem to display controller prompts correctly.
* Epilepsy warning! TAA causes issues with shader of phantoms in the otherworld, so if you are prone to suffering epileptic seizures - use SMAA instead.
* Occasionally the protagonist may fall through the terrain when using interpolation. Sorry :(


# Notes
* Replacing gamepad prompts requires generally requires modifying Steam Input profile, you generally want:
   * Xbox's A / DualShock's Cross to be the game's (Switch) B.
   * Xbox's B / DualShock's Circle to be the game's (Switch) A.
   * Xbox's X / DualShock's Square to be the game's (Switch) Y.
   * Xbox's Y / DualShock's Triangle to be the game's (Switch) X.
* Due to hooking SteamInput to provide Keyboard and Mouse input, a controller that SteamInput detects has to be present for keyboard and mouse to work. This can be a virtual controller (for example [Virtual Controller / vJoy](https://sourceforge.net/projects/vjoy-controller/) seems to be working just fine when using vXbox).
* If you extend draw distance usinf far plane value and notice outlines bugging out when up close, make sure to adjust the outline distance value.
