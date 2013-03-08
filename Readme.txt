======================
Miner Wars 2081 MOD KIT
======================

You need to own Steam version of Miner Wars 2081 to create or play MODS!
MinerWars.exe and Original Content (textures, models, audio) is not part of MOD KIT (3rd party resources can be provided)

Official modding website: http://www.MinerWars.com/SourceCode.aspx
Official modding forum: http://www.minerwars.com/ForumCategory.aspx?id=13
Source code: [TODO: Add Url]
30-seconds video tutorial: [TODO: Add Url]

When we refer to MinerWars folder, we have MinerWars folder in Steam client on our mind. 
Usually C:\Program Files\Steam\steamapps\common\MinerWars\ or C:\Program Files (x86)\Steam\steamapps\common\MinerWars\

Make your MOD
=============
1) Copy "steam_appid.txt" to Steam folder ("c:\Program Files (x86)\Steam\steamapps\common\MinerWars\")
2) Download and install free Visual C# 2010 Express: http://www.microsoft.com/visualstudio/eng#downloads+d-2010-express
3) Download zip with Miner Wars 2081 source code from github and unpack [TODO: Add URL]
4) Make sure Steam is running and you own MinerWars 2081
5) Double-click MinerWars2081.sln
6) Set your MinerWars.exe location
   a) Right-click MinerWars.GameLib (in solution explorer)
   b) Select properties
   c) Select Debug tab
   d) Set "Start external program:" to MinerWars.exe ([Steam folder]\steamapps\common\MinerWars\MinerWars.exe)
7) Do some code changes (e.g. change ammo damage contant)
8) Click Debug in Visual studio menu, click "Start debugging"
9) When you're done with your MOD and want to distribute it -> test it first without Visual Studio (see Distribute your MOD and Use your MOD sections)

Distribute your MOD
===================
1) Open file MinerWarsCommonLIB\AppCode\Utils\MyMwcSecrets.cs
   a) Fill MOD_NAME
   b) Fill GAME_HASH - you can generate new GUID (Main menu, Tools, Create GUID) and put it here in format: "F80AE059ABF7401893E3724AE573FA1E" (dashes removed)
2) Select "Release" in Visual C# (Debug/Release combo box)
3) Click Build/Rebuild solution
4) Rename "Release" folder in "Sources\MinerWars.GameLib\bin\x86\" to "MyMod" (use name of your mod)
5) Zip "MyMod" folder
6) Distribute zipped folder

Use your MOD
============
1) Unzip MOD into [MinerWars folder]\Mods (eg. C:\Program Files\Steam\steamapps\common\MinerWars\Mods\MyMod should contain MinerWars.GameLib.dll)
2) Create shortcut to MinerWars.exe
3) Edit shortcut (right-click, properties)
   a) Write after quotes space and this: -FromLauncher -mod MyMod
   b) It should look like: "C:\....\MinerWars.exe" -FromLauncher -mod MyMod
   - replace MyMod with mod name
4) Run shortcut


Tips when developing your own mod
=================================
- When you see "Please run this game from Steam", make sure you have:
  a) Steam client running, user logged in, owns Miner Wars 2081
  b) File "steam_appid.txt" exists in MinerWars Steam folder (See step 1 in Make your MOD) and contains: 223430
- When MinerWars is started, it checks for permissions, when it detects admin permissions, it restarts as "Basic User".
  This check is disabled when running mod from Visual Studio.
  Make sure you don't write to C:\ or other protected locations, becase it won't work when you distribute your MOD.
- Content (textures, models, audio) can be modified too, check Readme_Content.txt
- To quickly find classes and members, use file search tool, "DPack" (Visual studio addin) is great! Also CTRL+comma works quite good.
- You should not replace original files in Content folder, instead place your files to Mods/YourMod folder and change source codes to use files in this directory.
- Almost all classes starts with "My" prefix
- You can distribute only your MOD dir, original Content and EXE distribution is prohibited (see License.txt for details)
- When you're not sure how something works, just put breakpoint (press F9) to some method and see values when breakpoint is hit (Yes, you can DEBUG actual game!)
- You can't add new steam achievements!
- When you write something which makes development of other mods easy, you can submit the code and we add it to official MOD branch (on GitHub), so other developers can use it too
  (e.g. support for new audio files, better texture loading for mods, some plugin API and so on)