======================================
MinerWars 2081 MOD KIT - Content Tools
======================================

Audio
=====
1) Download complete Audio pack, http://sourceforge.net/projects/minerwars2081/files/
2) Install DirectX SDK, http://www.microsoft.com/en-us/download/details.aspx?id=6812
3) Open Audio.xap in "Microsoft Cross-Platform Audio Creation Tool 3 (XACT3)"
4) Edit sounds
5) Click File/Build...
6) Add Audio content to you mod dir
7) Change sources codes (MyPlugins.cs, GetAudioFolder() method) to use your audio instead of official audio

Textures
========
MinerWars 2081 uses mainly 2 textures:
a) "de" - diffuse, emmissive. Texture contains diffuse color in RGB channel and emissivity in Alpha channel
b) "ns" - normal, specular. Texture contains normal in RGB channel and specular in Alpha channel

All textures should be DXT5 compressed DDS files, but game also handles loading png's.
If you don't know what this means, you can just use classic texture (saved as PNG) as "de" texture, it will work.

1) Add modified textures to you mod dir
2) Change sources codes (MyTextureManager.cs, LoadTexture() method) to use your textures first, then (when texture in your mod dir not found) to use official textures.

Models
======
All models must be FBX files (many editors can export to FBX)

1) Install XNA Game Studio (required to build models)
2) Copy your FBX models to "Utils/MwmBuilder/Content/Custom" (relative to this file)
3) Run "Utils/MwmBuilder/Models.bat"
4) Copy compiled models from "Utils/MwmBuilder/Output/Custom" to you mod dir
5) Change sources codes (MyModels.cs, MyModel constructor and Load methods) to use your models first, then (when model in your mod file not found) to use official models

Feel free to edit Models.bat and experiment with parameters
When your model is deformed, not textured or you have any other issue, take a look at Ship model in "Utils/MwmBuilder/Content/Custom", it could be your starting point

Shaders
=======
All shader source codes are in "Effects" folder

1) It's recommended to install NShader addin to edit .fx files or use shader editor
2) Copy modified ".fx" to your mod dir
3) Change sources codes (MyEffectBase, constructor) to use your shaders first, then (when shader is not found in your mod dir) to use official shader

Shaders are automatically compiled to .fxo files when game detect's that .fx file is newer than .fxo
Make sure game can write to your mod directory (or where you place your .fx files) You can run Visual Studio as admin
You only need to distribute .fxo file, .fx file can be deleted when you're done with it (backup it somewhere)
