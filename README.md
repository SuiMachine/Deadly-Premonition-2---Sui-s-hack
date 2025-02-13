# Deadly Premonition 2 - Sui's hack
A hack/mod that utilizes ''BepInEx'' and ''HarmonyX'' to make up for some of the ports shortcomings.

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
* Enforce LOD models to cast their own shadows.
* Basic Keyboard and Mouse support* (see notes).

# Experimental features
* Hook for SteamInput to read Keyboard and mouse input instead.

# Requirements
* Original copy of the game.
* BepInEx (bundled in the release).

# Installation (Windows)
* Download the [Sui's Hack](https://github.com/SuiMachine/Deadly-Premonition-2---Sui-s-hack/releases) and extract it to game directory.
* Launch the game.
* Press F11 to open configuration GUI or close the game and edit "SuisHack.cfg" inside the BepInEx/config folder.

# Installation (SteamDeck)
* Switch to desktop mode.
* Download [Sui's Hack](https://github.com/SuiMachine/Deadly-Premonition-2---Sui-s-hack/releases) and extract it.
* Open Steam (in desktop mode) and find Deadly Premonition 2 on the list of games in your library.
* Right click on it and select `Properties`.
* Go to `Installed files` tab and select `Browse...` - this will open a new Dolphin file explorer window.
* Copy extracted files to that directory.
* Now return to Deadly Premonition 2's property window in Steam and go to `General` tab.
* Under launch options paste in the following `WINEDLLOVERRIDES="winhttp.dll=n,b" %command%`
* Launch the game.

# Bugs
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
