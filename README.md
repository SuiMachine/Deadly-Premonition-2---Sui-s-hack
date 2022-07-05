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
   * Shadow Mask Mode
   * Shadow Projection Mode
   * Shadow cascade amount (and its distances)
   * LOD Bias
   * Pixel Light Count
   * Texture Resolution
   * Far Plane Distance
* Option to skip splash screens / intros
* Runtime button prompt replacement with buttons prepared for Xbox and Playstation controllers (make sure to correct bindings in Steam Input).
* Bugfix: Corrected Electrical Wires bounds to be rendered correctly (Fix geometry).
* Movement interpolation.
* A basic GUI available for configuring the mod available under F11.

# Experimental features
* Hook for SteamInput to read Keyboard and mouse input instead (set Input type to "KeyboardAndMouse" in config file).

# Requirements
* Original copy of the game
* Melon Loader 0.5+

# Intallation
* Download [MelonLoader](https://github.com/LavaGang/MelonLoader/releases) and install it (prefably using installer).
* Download the [Sui's Hack](https://github.com/SuiMachine/Deadly-Premonition-2---Sui-s-hack/releases) and extract it to game directory (do not change the file structure SuisHack.dll goes into "Mods", "Prompts" directory to a game's "StreamingAssets" folder etc.)
* Launch the game.
* Press F11 to open configuration GUI or close the game and edit "MelonPreferences.cfg" inside the UserData folder.

# Bugs
* (BUG) Resolution is not applied to main window on startup, if focus is on console window.
* TAA causes some issues with shader of phantoms in the otherworld, so it has Jitter set to 0 to avoid that - because of that it doesn't produce great results.

# Notes
* Replacing gamepad prompts requires generally requires modifying Steam Input profile, you generally want:
   * Xbox's A / DualShock's Cross to be the game's (Switch) B.
   * Xbox's B / DualShock's Circle to be the game's (Switch) A.
   * Xbox's X / DualShock's Square to be the game's (Switch) Y.
   * Xbox's Y / DualShock's Triangle to be the game's (Switch) X.
* Due to hooking SteamInput to provide Keyboard and Mouse input, a controller that SteamInput detects has to be present for keyboard and mouse to work. This can be a virtual controller (for example [Virtual Controller / vJoy](https://sourceforge.net/projects/vjoy-controller/) seems to be working just fine when using vXbox).