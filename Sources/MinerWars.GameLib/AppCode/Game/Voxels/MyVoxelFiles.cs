using MinerWars.AppCode.App;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.CommonLIB.AppCode.Utils;
using SysUtils.Utils;

namespace MinerWars.AppCode.Game.Voxels
{
    class MyVoxelFile
    {
        public MyMwcVoxelFilesEnum VoxelFileEnum;
        public MyMwcVector3Int SizeInVoxels;
        public string VoxelName;

        private MyVoxelFile() { }
        
        public MyVoxelFile(MyMwcVoxelFilesEnum voxelFileEnum, MyMwcVector3Int sizeInVoxels, string voxelName)
        {
            VoxelFileEnum = voxelFileEnum;
            SizeInVoxels = sizeInVoxels;
            VoxelName = voxelName;
        }

        public int GetLargestSizeInVoxels()
        {
            int max = SizeInVoxels.X;
            if (SizeInVoxels.Y > max) max = SizeInVoxels.Y;
            if (SizeInVoxels.Z > max) max = SizeInVoxels.Z;
            return max;
        }

        //  Full or relative path to VOX file
        public string GetVoxFilePath()
        {
            return MyMinerGame.Static.RootDirectory + "\\VoxelMaps\\" + VoxelName + ".vox";
        }

        public string GetIconFilePath()
        {
            return "Textures\\GUI\\GuiHelpers\\" + VoxelName;
        }
    }

    static class MyVoxelFiles
    {
        public static readonly string ExportFile = "VoxelImporterTest";

        public static MyVoxelFile[] DefaultVoxelFiles = new MyVoxelFile[MyMwcUtils.GetMaxValueFromEnum<MyMwcVoxelFilesEnum>() + 1];

        public static void LoadData()
        {
            Add(MyMwcVoxelFilesEnum.PerfectSphereSplitted_512x512x512, new MyMwcVector3Int(512, 512, 512), "PerfectSphereSplitted_512x512x512");
            Add(MyMwcVoxelFilesEnum.PerfectSphereWithFewTunnels_512x512x512, new MyMwcVector3Int(512, 512, 512), "PerfectSphereWithFewTunnels_512x512x512");
            Add(MyMwcVoxelFilesEnum.PerfectSphereWithMassiveTunnels_512x512x512, new MyMwcVector3Int(512, 512, 512), "PerfectSphereWithMassiveTunnels_512x512x512");
            Add(MyMwcVoxelFilesEnum.PerfectSphereWithMassiveTunnels2_512x512x512, new MyMwcVector3Int(512, 512, 512), "PerfectSphereWithMassiveTunnels2_512x512x512");
            Add(MyMwcVoxelFilesEnum.PerfectSphereWithRaceTunnel_512x512x512, new MyMwcVector3Int(512, 512, 512), "PerfectSphereWithRaceTunnel_512x512x512");
            Add(MyMwcVoxelFilesEnum.PerfectSphereWithRaceTunnel2_512x512x512, new MyMwcVector3Int(512, 512, 512), "PerfectSphereWithRaceTunnel2_512x512x512");
            Add(MyMwcVoxelFilesEnum.SphereWithLargeCutOut_128x128x128, new MyMwcVector3Int(128, 128, 128), "SphereWithLargeCutOut_128x128x128");
            Add(MyMwcVoxelFilesEnum.TorusWithManyTunnels_256x128x256, new MyMwcVector3Int(256, 128, 256), "TorusWithManyTunnels_256x128x256");
            Add(MyMwcVoxelFilesEnum.TorusWithManyTunnels_2_256x128x256, new MyMwcVector3Int(256, 128, 256), "TorusWithManyTunnels_2_256x128x256");
            Add(MyMwcVoxelFilesEnum.TorusWithSmallTunnel_256x128x256, new MyMwcVector3Int(256, 128, 256), "TorusWithSmallTunnel_256x128x256");
            Add(MyMwcVoxelFilesEnum.VerticalIsland_128x128x128, new MyMwcVector3Int(128, 128, 128), "VerticalIsland_128x128x128");
            Add(MyMwcVoxelFilesEnum.VerticalIsland_128x256x128, new MyMwcVector3Int(128, 256, 128), "VerticalIsland_128x256x128");
            Add(MyMwcVoxelFilesEnum.TorusStorySector_256x128x256, new MyMwcVector3Int(256, 128, 256), "TorusStorySector_256x128x256");
            Add(MyMwcVoxelFilesEnum.VerticalIslandStorySector_128x256x128, new MyMwcVector3Int(128, 256, 128), "VerticalIslandStorySector_128x256x128");
            Add(MyMwcVoxelFilesEnum.DeformedSphere1_64x64x64, new MyMwcVector3Int(64, 64, 64), "DeformedSphere1_64x64x64");
            Add(MyMwcVoxelFilesEnum.DeformedSphere2_64x64x64, new MyMwcVector3Int(64, 64, 64), "DeformedSphere2_64x64x64");
            Add(MyMwcVoxelFilesEnum.DeformedSphereWithCorridor_128x64x64, new MyMwcVector3Int(128, 64, 64), "DeformedSphereWithCorridor_128x64x64");
            Add(MyMwcVoxelFilesEnum.DeformedSphereWithCorridor_256x256x256, new MyMwcVector3Int(256, 256, 256), "DeformedSphereWithCorridor_256x256x256");
            Add(MyMwcVoxelFilesEnum.DeformedSphereWithCraters_128x128x128, new MyMwcVector3Int(128, 128, 128), "DeformedSphereWithCraters_128x128x128");
            Add(MyMwcVoxelFilesEnum.ScratchedBoulder_128x128x128, new MyMwcVector3Int(128, 128, 128), "ScratchedBoulder_128x128x128");
            Add(MyMwcVoxelFilesEnum.DeformedSphereWithHoles_64x128x64, new MyMwcVector3Int(64, 128, 64), "DeformedSphereWithHoles_64x128x64");
            Add(MyMwcVoxelFilesEnum.VoxelImporterTest, new MyMwcVector3Int(128, 128, 128), ExportFile);
            Add(MyMwcVoxelFilesEnum.Fortress, new MyMwcVector3Int(128, 128, 128), "Fortress");
            Add(MyMwcVoxelFilesEnum.AsteroidWithSpaceStationStartStorySector, new MyMwcVector3Int(128, 128, 128), "AsteroidWithSpaceStationStartStorySector");

            Add(MyMwcVoxelFilesEnum.Cube_128x128x128, new MyMwcVector3Int(128, 128, 128), "Cube_128x128x128");
            Add(MyMwcVoxelFilesEnum.Cube_128x128x256, new MyMwcVector3Int(128, 128, 256), "Cube_128x128x256");
            Add(MyMwcVoxelFilesEnum.Cube_128x128x64, new MyMwcVector3Int(128, 128, 64), "Cube_128x128x64");
            Add(MyMwcVoxelFilesEnum.Cube_128x256x128, new MyMwcVector3Int(128, 256, 128), "Cube_128x256x128");
            Add(MyMwcVoxelFilesEnum.Cube_128x256x256, new MyMwcVector3Int(128, 256, 256), "Cube_128x256x256");
            Add(MyMwcVoxelFilesEnum.Cube_128x256x64, new MyMwcVector3Int(128, 256, 64), "Cube_128x256x64");
            Add(MyMwcVoxelFilesEnum.Cube_128x64x128, new MyMwcVector3Int(128, 64, 128), "Cube_128x64x128");
            Add(MyMwcVoxelFilesEnum.Cube_128x64x256, new MyMwcVector3Int(128, 64, 256), "Cube_128x64x256");
            Add(MyMwcVoxelFilesEnum.Cube_128x64x64, new MyMwcVector3Int(128, 64, 64), "Cube_128x64x64");
            Add(MyMwcVoxelFilesEnum.Cube_256x128x128, new MyMwcVector3Int(256, 128, 128), "Cube_256x128x128");
            Add(MyMwcVoxelFilesEnum.Cube_256x128x256, new MyMwcVector3Int(256, 128, 256), "Cube_256x128x256");
            Add(MyMwcVoxelFilesEnum.Cube_256x128x512, new MyMwcVector3Int(256, 128, 512), "Cube_256x128x512");
            Add(MyMwcVoxelFilesEnum.Cube_256x256x128, new MyMwcVector3Int(256, 256, 128), "Cube_256x256x128");
            Add(MyMwcVoxelFilesEnum.Cube_256x256x256, new MyMwcVector3Int(256, 256, 256), "Cube_256x256x256");
            Add(MyMwcVoxelFilesEnum.Cube_256x256x512, new MyMwcVector3Int(256, 256, 512), "Cube_256x256x512");
            Add(MyMwcVoxelFilesEnum.Cube_256x512x128, new MyMwcVector3Int(256, 512, 128), "Cube_256x512x128");
            Add(MyMwcVoxelFilesEnum.Cube_256x512x256, new MyMwcVector3Int(256, 512, 256), "Cube_256x512x256");
            Add(MyMwcVoxelFilesEnum.Cube_256x512x512, new MyMwcVector3Int(256, 512, 512), "Cube_256x512x512");
            Add(MyMwcVoxelFilesEnum.Cube_512x256x256, new MyMwcVector3Int(512, 256, 256), "Cube_512x256x256");
            Add(MyMwcVoxelFilesEnum.Cube_512x256x512, new MyMwcVector3Int(512, 256, 512), "Cube_512x256x512");
            Add(MyMwcVoxelFilesEnum.Cube_512x512x256, new MyMwcVector3Int(512, 512, 256), "Cube_512x512x256");
            Add(MyMwcVoxelFilesEnum.Cube_512x512x512, new MyMwcVector3Int(512, 512, 512), "Cube_512x512x512");
            Add(MyMwcVoxelFilesEnum.Cube_64x128x128, new MyMwcVector3Int(64, 128, 128), "Cube_64x128x128");
            Add(MyMwcVoxelFilesEnum.Cube_64x128x64, new MyMwcVector3Int(64, 128, 64), "Cube_64x128x64");
            Add(MyMwcVoxelFilesEnum.Cube_64x64x128, new MyMwcVector3Int(64, 64, 128), "Cube_64x64x128");
            Add(MyMwcVoxelFilesEnum.Cube_64x64x64, new MyMwcVector3Int(64, 64, 64), "Cube_64x64x64");

            Add(MyMwcVoxelFilesEnum.Story02, new MyMwcVector3Int(256, 256, 256), "Story02");
            Add(MyMwcVoxelFilesEnum.Mission01_01, new MyMwcVector3Int(512, 256, 256), "Mission01_01");
            Add(MyMwcVoxelFilesEnum.Mission01_02, new MyMwcVector3Int(256, 256, 256), "Mission01_02");
            Add(MyMwcVoxelFilesEnum.Mission07_01, new MyMwcVector3Int(128, 256, 128), "Mission07_01");
            Add(MyMwcVoxelFilesEnum.Mission01_asteroid_mine, new MyMwcVector3Int(256, 256, 256), "Mission01_asteroid_mine");
            Add(MyMwcVoxelFilesEnum.Mission01_asteroid_big, new MyMwcVector3Int(512, 256, 256), "Mission01_asteroid_big");

            Add(MyMwcVoxelFilesEnum.SphereFull_64, new MyMwcVector3Int(64, 64, 64), "SphereFull_64");
            Add(MyMwcVoxelFilesEnum.SphereFull_128, new MyMwcVector3Int(128, 128, 128), "SphereFull_128");
            Add(MyMwcVoxelFilesEnum.SphereFull_256, new MyMwcVector3Int(256, 256, 256), "SphereFull_256");
            Add(MyMwcVoxelFilesEnum.SphereFull_512, new MyMwcVector3Int(512, 512, 512), "SphereFull_512");
            Add(MyMwcVoxelFilesEnum.SphereFull_1024, new MyMwcVector3Int(1024, 1024, 1024), "SphereFull_1024");

            Add(MyMwcVoxelFilesEnum.EacPrisonAsteroid, new MyMwcVector3Int(512, 512, 512), "EacPrisonAsteroid");
            Add(MyMwcVoxelFilesEnum.piratebase_export, new MyMwcVector3Int(512, 512, 512), "piratebase_export");

            Add(MyMwcVoxelFilesEnum.Chinese_Mines_CenterAsteroid, new MyMwcVector3Int(256, 256, 256), "Chinese_Mines_CenterAsteroid");
            Add(MyMwcVoxelFilesEnum.Chinese_Mines_FrontAsteroid, new MyMwcVector3Int(256, 256, 256), "Chinese_Mines_FrontAsteroid");
            Add(MyMwcVoxelFilesEnum.Chinese_Mines_FrontRightAsteroid, new MyMwcVector3Int(256, 256, 256), "Chinese_Mines_FrontRightAsteroid");
            Add(MyMwcVoxelFilesEnum.Chinese_Mines_LeftAsteroid, new MyMwcVector3Int(256, 256, 256), "Chinese_Mines_LeftAsteroid");
            Add(MyMwcVoxelFilesEnum.Chinese_Mines_MainAsteroid, new MyMwcVector3Int(256, 256, 256), "Chinese_Mines_MainAsteroid");
            Add(MyMwcVoxelFilesEnum.Chinese_Mines_RightAsteroid, new MyMwcVector3Int(256, 256, 256), "Chinese_Mines_RightAsteroid");

            Add(MyMwcVoxelFilesEnum.TowerWithConcreteBlock1, new MyMwcVector3Int(256, 512, 128), "TowerWithConcreteBlock1");
            Add(MyMwcVoxelFilesEnum.TowerWithConcreteBlock2, new MyMwcVector3Int(256, 512, 128), "TowerWithConcreteBlock2");

            Add(MyMwcVoxelFilesEnum.Warehouse, new MyMwcVector3Int(256, 256, 256), "Warehouse");
            
            Add(MyMwcVoxelFilesEnum.Barths_moon_base, new MyMwcVector3Int(256, 256, 256), "Barths_moon_base");
            Add(MyMwcVoxelFilesEnum.Barths_moon_satelite, new MyMwcVector3Int(128, 128, 128), "Barths_moon_satelite");
            Add(MyMwcVoxelFilesEnum.Fort_valiant_base, new MyMwcVector3Int(512, 256, 256), "Fort_valiant_base");
            Add(MyMwcVoxelFilesEnum.Fort_valiant_dungeon, new MyMwcVector3Int(512, 512, 512), "Fort_valiant_dungeon");

            Add(MyMwcVoxelFilesEnum.JunkYardInhabited_256x128x256, new MyMwcVector3Int(256, 128, 256), "JunkYardInhabited_256x128x256");
            Add(MyMwcVoxelFilesEnum.JunkYardToxic_128x128x128, new MyMwcVector3Int(128, 128, 128), "JunkYardToxic_128x128x128");

            Add(MyMwcVoxelFilesEnum.Empty_512x512x512, new MyMwcVector3Int(512, 512, 512), "Empty_512x512x512");

            Add(MyMwcVoxelFilesEnum.JunkYardForge_256x256x256, new MyMwcVector3Int(256, 256, 256), "JunkYardForge_256x256x256");

            Add(MyMwcVoxelFilesEnum.Barths_moon_camp, new MyMwcVector3Int(256, 256, 256), "Barths_moon_camp");

            Add(MyMwcVoxelFilesEnum.rift_base_bigger, new MyMwcVector3Int(128, 256, 256), "Rift_base_bigger");
            Add(MyMwcVoxelFilesEnum.rift_base_smaller, new MyMwcVector3Int(64, 128, 64), "Rift_base_smaller");
            Add(MyMwcVoxelFilesEnum.rift, new MyMwcVector3Int(512, 512, 256), "rift");
            Add(MyMwcVoxelFilesEnum.rift_small_1, new MyMwcVector3Int(64, 64, 64), "rift_small_1");
            Add(MyMwcVoxelFilesEnum.rift_small_2, new MyMwcVector3Int(64, 64, 64), "rift_small_2");

            Add(MyMwcVoxelFilesEnum.barths_moon_lab1, new MyMwcVector3Int(256, 256, 256), "barths_moon_lab1");
            Add(MyMwcVoxelFilesEnum.barths_moon_lab2, new MyMwcVector3Int(256, 256, 256), "barths_moon_lab2");

            Add(MyMwcVoxelFilesEnum.fort_val_box_128, new MyMwcVector3Int(128, 128, 128), "fort_val_box_128");


            Add(MyMwcVoxelFilesEnum.Junkyard_Race_256x256x256, new MyMwcVector3Int(256, 256, 256), "Junkyard_Race_256x256x256");
            Add(MyMwcVoxelFilesEnum.Junkyard_RaceAsteroid_256x256x256, new MyMwcVector3Int(256, 256, 256), "Junkyard_RaceAsteroid_256x256x256");

            Add(MyMwcVoxelFilesEnum.ChineseRefinery_First_128x128x128, new MyMwcVector3Int(256, 256, 256), "ChineseRefinery_First_128x128x128");
            Add(MyMwcVoxelFilesEnum.ChineseRefinery_Second_128x128x128, new MyMwcVector3Int(256, 256, 256), "ChineseRefinery_Second_128x128x128");
            Add(MyMwcVoxelFilesEnum.ChineseRefinery_Third_128x256x128, new MyMwcVector3Int(256, 256, 256), "ChineseRefinery_Third_128x256x128");

            Add(MyMwcVoxelFilesEnum.Chinese_Corridor_Last_126x126x126, new MyMwcVector3Int(256, 256, 256), "Chinese_Corridor_Last_126x126x126");
            Add(MyMwcVoxelFilesEnum.Chinese_Corridor_Tunnel_256x256x256, new MyMwcVector3Int(256, 256, 256), "Chinese_Corridor_Tunnel_256x256x256");

            Add(MyMwcVoxelFilesEnum.Bioresearch, new MyMwcVector3Int(256, 256, 256), "Bioresearch");

            Add(MyMwcVoxelFilesEnum.small2_asteroids, new MyMwcVector3Int(128, 128, 128), "small2_asteroids");
            Add(MyMwcVoxelFilesEnum.small3_asteroids, new MyMwcVector3Int(128, 128, 128), "small3_asteroids");
            Add(MyMwcVoxelFilesEnum.many_medium_asteroids, new MyMwcVector3Int(128, 128, 128), "many_medium_asteroids");
            Add(MyMwcVoxelFilesEnum.many_small_asteroids, new MyMwcVector3Int(128, 128, 128), "many_small_asteroids");
            Add(MyMwcVoxelFilesEnum.many2_small_asteroids, new MyMwcVector3Int(128, 128, 128), "many2_small_asteroids");
            Add(MyMwcVoxelFilesEnum.rus_attack, new MyMwcVector3Int(128, 128, 128), "rus_attack");

            Add(MyMwcVoxelFilesEnum.RussianWarehouse, new MyMwcVector3Int(256, 256, 256), "RussianWarehouse");
            Add(MyMwcVoxelFilesEnum.MothershipFacility, new MyMwcVector3Int(256, 256, 256), "mothership_facility");
            Add(MyMwcVoxelFilesEnum.SlaverBase, new MyMwcVector3Int(256, 256, 256), "slaver_base");
            Add(MyMwcVoxelFilesEnum.ResearchVessel, new MyMwcVector3Int(256, 256, 256), "research_vessel");
            Add(MyMwcVoxelFilesEnum.BilitaryBase, new MyMwcVector3Int(256, 256, 256), "military_base");

            Add(MyMwcVoxelFilesEnum.reef_ast, new MyMwcVector3Int(64, 64, 64), "reef_ast");

            Add(MyMwcVoxelFilesEnum.hopebase512, new MyMwcVector3Int(512, 512, 512), "hopebase512");
            Add(MyMwcVoxelFilesEnum.hopefood128, new MyMwcVector3Int(128, 128, 128), "hopefood128");
            Add(MyMwcVoxelFilesEnum.hopevault128, new MyMwcVector3Int(128, 128, 128), "hopevault128");
            Add(MyMwcVoxelFilesEnum.New_Jerusalem_Asteroid, new MyMwcVector3Int(128, 128, 128), "New_Jerusalem_Asteroid");
            Add(MyMwcVoxelFilesEnum.Chinese_Transmitter_Asteroid, new MyMwcVector3Int(128, 128, 128), "Chinese_Transmitter_Asteroid");
            Add(MyMwcVoxelFilesEnum.Slaver_Base_2_Asteroid, new MyMwcVector3Int(128, 128, 128), "Slaver_Base_2_Asteroid");
            Add(MyMwcVoxelFilesEnum.Small_Pirate_Base_Asteroid, new MyMwcVector3Int(128, 128, 128), "Small_Pirate_Base_Asteroid");
            Add(MyMwcVoxelFilesEnum.Small_Pirate_Base_2_Asteroid, new MyMwcVector3Int(128, 128, 128), "Small_Pirate_Base_2_Asteroid");
            Add(MyMwcVoxelFilesEnum.Solar_Factory_EAC_Asteroid, new MyMwcVector3Int(128, 128, 128), "Solar_Factory_EAC_Asteroid");
            Add(MyMwcVoxelFilesEnum.Mines_Asteroid, new MyMwcVector3Int(128, 128, 128), "Mines_Asteroid");
            Add(MyMwcVoxelFilesEnum.Small_Pirate_Base_3_1, new MyMwcVector3Int(64, 64, 64), "Small_Pirate_Base_3_1");
            Add(MyMwcVoxelFilesEnum.Small_Pirate_Base_3_2, new MyMwcVector3Int(64, 64, 64), "Small_Pirate_Base_3_2");
            Add(MyMwcVoxelFilesEnum.Small_Pirate_Base_3_3, new MyMwcVector3Int(64, 64, 64), "Small_Pirate_Base_3_3");
            Add(MyMwcVoxelFilesEnum.Voxel_Arena_Tunnels, new MyMwcVector3Int(256, 256, 256), "Voxel_Arena_Tunnels");
            Add(MyMwcVoxelFilesEnum.EACSurvaySmaller_256_256_256, new MyMwcVector3Int(256, 256, 256), "EACSurvaySmaller_256_256_256");
            Add(MyMwcVoxelFilesEnum.EACSurveyBigger_512_256_256, new MyMwcVector3Int(512, 256, 256), "EACSurveyBigger_512_256_256");
            Add(MyMwcVoxelFilesEnum.Laika1_128_128_128, new MyMwcVector3Int(512, 256, 256), "Laika1_128_128_128");
            Add(MyMwcVoxelFilesEnum.Laika2_64_64_64, new MyMwcVector3Int(512, 256, 256), "Laika2_64_64_64");
            Add(MyMwcVoxelFilesEnum.Laika3_64_64_64, new MyMwcVector3Int(512, 256, 256), "Laika3_64_64_64");
            Add(MyMwcVoxelFilesEnum.Laika4_256_128_128, new MyMwcVector3Int(512, 256, 256), "Laika4_256_128_128");
            Add(MyMwcVoxelFilesEnum.Laika5_128_128_128, new MyMwcVector3Int(512, 256, 256), "Laika5_128_128_128");
            Add(MyMwcVoxelFilesEnum.Laika6_64_64_64, new MyMwcVector3Int(512, 256, 256), "Laika6_64_64_64");
            Add(MyMwcVoxelFilesEnum.Laika7_64_64_64, new MyMwcVector3Int(512, 256, 256), "Laika7_64_64_64");
            Add(MyMwcVoxelFilesEnum.Laika8_128_128_128, new MyMwcVector3Int(512, 256, 256), "Laika8_128_128_128");
            Add(MyMwcVoxelFilesEnum.Laika9_128_128_128, new MyMwcVector3Int(512, 256, 256), "Laika9_128_128_128");
            Add(MyMwcVoxelFilesEnum.Novaja_Zemlja, new MyMwcVector3Int(256, 256, 256), "Novaja_Zemlja");

            Add(MyMwcVoxelFilesEnum.Arabian_Border_1, new MyMwcVector3Int(64, 64, 64), "Arabian_Border_1");
            Add(MyMwcVoxelFilesEnum.Arabian_Border_2, new MyMwcVector3Int(64, 64, 64), "Arabian_Border_2");
            Add(MyMwcVoxelFilesEnum.Arabian_Border_3, new MyMwcVector3Int(64, 64, 64), "Arabian_Border_3");
            Add(MyMwcVoxelFilesEnum.Arabian_Border_4, new MyMwcVector3Int(64, 64, 64), "Arabian_Border_4");
            Add(MyMwcVoxelFilesEnum.Arabian_Border_5, new MyMwcVector3Int(64, 64, 64), "Arabian_Border_5");
            Add(MyMwcVoxelFilesEnum.Arabian_Border_6, new MyMwcVector3Int(64, 64, 64), "Arabian_Border_6");
            Add(MyMwcVoxelFilesEnum.Arabian_Border_7, new MyMwcVector3Int(64, 64, 64), "Arabian_Border_7");
            Add(MyMwcVoxelFilesEnum.Arabian_Border_Arabian, new MyMwcVector3Int(128, 128, 128), "Arabian_Border_Arabian");
            Add(MyMwcVoxelFilesEnum.Arabian_Border_EAC, new MyMwcVector3Int(128, 128, 128), "Arabian_Border_EAC");

            Add(MyMwcVoxelFilesEnum.Chinese_Corridor_1, new MyMwcVector3Int(128, 128, 128), "Chinese_Corridor_1");

            Add(MyMwcVoxelFilesEnum.Grave_Skull, new MyMwcVector3Int(256, 256, 256), "Grave_Skull");

            Add(MyMwcVoxelFilesEnum.Pirate_Base_1, new MyMwcVector3Int(64, 64, 64), "Pirate_Base_1");
            Add(MyMwcVoxelFilesEnum.Pirate_Base_2, new MyMwcVector3Int(128, 128, 128), "Pirate_Base_2");
            Add(MyMwcVoxelFilesEnum.Pirate_Base_3, new MyMwcVector3Int(64, 128, 64), "Pirate_Base_3");

            Add(MyMwcVoxelFilesEnum.Plutonium_Mines, new MyMwcVector3Int(256, 256, 256), "PlutoniumMines");

            Add(MyMwcVoxelFilesEnum.Fragile_Sector, new MyMwcVector3Int(256, 128, 256), "Fragile_Sector");

            Add(MyMwcVoxelFilesEnum.Chinese_Solar_Array_Bottom, new MyMwcVector3Int(64, 64, 64), "Chinese_Solar_Array_Bottom");
            Add(MyMwcVoxelFilesEnum.Chinese_Solar_Array_Main, new MyMwcVector3Int(256, 128, 256), "Chinese_Solar_Array_Main");

            Add(MyMwcVoxelFilesEnum.Chinese_Mines_Small, new MyMwcVector3Int(64, 64, 64), "Chinese_Mines_Small");
            Add(MyMwcVoxelFilesEnum.Chinese_Mines_Side, new MyMwcVector3Int(128, 128, 128), "Chinese_Mines_Side");

            Add(MyMwcVoxelFilesEnum.Hippie_Outpost_Base, new MyMwcVector3Int(128, 128, 128), "Hippie_Outpost_Base");
            Add(MyMwcVoxelFilesEnum.Hippie_Outpost_Tree, new MyMwcVector3Int(512, 512, 512), "Hippie_Outpost_Tree");

            Add(MyMwcVoxelFilesEnum.Laika_1, new MyMwcVector3Int(64, 64, 64), "Laika_1");
            Add(MyMwcVoxelFilesEnum.Laika_2, new MyMwcVector3Int(256, 128, 256), "Laika_2");
            Add(MyMwcVoxelFilesEnum.Laika_3, new MyMwcVector3Int(64, 64, 64), "Laika_3");

            Add(MyMwcVoxelFilesEnum.Fortress_Sanc_1, new MyMwcVector3Int(128, 256, 128), "Fortress_Sanc_1");
            Add(MyMwcVoxelFilesEnum.Fortress_Sanc_2, new MyMwcVector3Int(256, 128, 256), "Fortress_Sanc_2");
            Add(MyMwcVoxelFilesEnum.Fortress_Sanc_3, new MyMwcVector3Int(128, 128, 128), "Fortress_Sanc_3");
            Add(MyMwcVoxelFilesEnum.Fortress_Sanc_4, new MyMwcVector3Int(128, 128, 128), "Fortress_Sanc_4");
            Add(MyMwcVoxelFilesEnum.Fortress_Sanc_5, new MyMwcVector3Int(256, 256, 256), "Fortress_Sanc_5");
            Add(MyMwcVoxelFilesEnum.Fortress_Sanc_6, new MyMwcVector3Int(256, 128, 256), "Fortress_Sanc_6");
            Add(MyMwcVoxelFilesEnum.Fortress_Sanc_7, new MyMwcVector3Int(128, 256, 128), "Fortress_Sanc_7");
            Add(MyMwcVoxelFilesEnum.Fortress_Sanc_8, new MyMwcVector3Int(128, 256, 128), "Fortress_Sanc_8");
            Add(MyMwcVoxelFilesEnum.Fortress_Sanc_9, new MyMwcVector3Int(128, 128, 128), "Fortress_Sanc_9");
            Add(MyMwcVoxelFilesEnum.Fortress_Sanc_10, new MyMwcVector3Int(128, 128, 128), "Fortress_Sanc_10");
            Add(MyMwcVoxelFilesEnum.Fortress_Sanc_11, new MyMwcVector3Int(128, 128, 128), "Fortress_Sanc_11");

            Add(MyMwcVoxelFilesEnum.Russian_Transmitter_1, new MyMwcVector3Int(128, 128, 128), "Russian_Transmitter_1");
            Add(MyMwcVoxelFilesEnum.Russian_Transmitter_2, new MyMwcVector3Int(256, 128, 256), "Russian_Transmitter_2");
            Add(MyMwcVoxelFilesEnum.Russian_Transmitter_3, new MyMwcVector3Int(256, 128, 256), "Russian_Transmitter_3");
            Add(MyMwcVoxelFilesEnum.Russian_Transmitter_Main, new MyMwcVector3Int(256, 256, 256), "Russian_Transmitter_Main");

            Add(MyMwcVoxelFilesEnum.Russian_Warehouse, new MyMwcVector3Int(64, 64, 64), "Russian_Warehouse");

            Add(MyMwcVoxelFilesEnum.ReichStag_1, new MyMwcVector3Int(128, 128, 128), "ReichStag_1");
            Add(MyMwcVoxelFilesEnum.ReichStag_2, new MyMwcVector3Int(256, 128, 256), "ReichStag_2");

            Add(MyMwcVoxelFilesEnum.New_Singapore, new MyMwcVector3Int(512, 512, 256), "New_Singapore");

            Add(MyMwcVoxelFilesEnum.HallOfFame, new MyMwcVector3Int(256, 128, 256), "HallOfFame");

            Add(MyMwcVoxelFilesEnum.IceCaveDeathmatch, new MyMwcVector3Int(256, 128, 256), "IceCaveDeathmatch");

            Add(MyMwcVoxelFilesEnum.WarehouseDeathmatch, new MyMwcVector3Int(256, 128, 256), "WarehouseDeathmatch");

            Add(MyMwcVoxelFilesEnum.Nearby_Station_1, new MyMwcVector3Int(128, 128, 128), "Nearby_Station_1");
            Add(MyMwcVoxelFilesEnum.Nearby_Station_2, new MyMwcVector3Int(64, 128, 64), "Nearby_Station_2");
            Add(MyMwcVoxelFilesEnum.Nearby_Station_3, new MyMwcVector3Int(64, 64, 128), "Nearby_Station_3");
            Add(MyMwcVoxelFilesEnum.Nearby_Station_4, new MyMwcVector3Int(64, 128, 64), "Nearby_Station_4");
            Add(MyMwcVoxelFilesEnum.Nearby_Station_5, new MyMwcVector3Int(128, 128, 128), "Nearby_Station_5");
            Add(MyMwcVoxelFilesEnum.Nearby_Station_6, new MyMwcVector3Int(128, 128, 128), "Nearby_Station_6");
            Add(MyMwcVoxelFilesEnum.Nearby_Station_7, new MyMwcVector3Int(64, 128, 64), "Nearby_Station_7");
            Add(MyMwcVoxelFilesEnum.Nearby_Station_8, new MyMwcVector3Int(64, 64, 64), "Nearby_Station_8");
            Add(MyMwcVoxelFilesEnum.Nearby_Station_9, new MyMwcVector3Int(128, 128, 128), "Nearby_Station_9");
            Add(MyMwcVoxelFilesEnum.Nearby_Station_10, new MyMwcVector3Int(64, 64, 64), "Nearby_Station_10");
            Add(MyMwcVoxelFilesEnum.Nearby_Station_11, new MyMwcVector3Int(128, 64, 64), "Nearby_Station_11");
            Add(MyMwcVoxelFilesEnum.Nearby_Station_12, new MyMwcVector3Int(256, 128, 256), "Nearby_Station_12");

            Add(MyMwcVoxelFilesEnum.VoxelArenaDeathmatch, new MyMwcVector3Int(256, 256, 256), "VoxelArenaDeathmatch");

            Add(MyMwcVoxelFilesEnum.RiftStationSmaller, new MyMwcVector3Int(64, 128, 64), "RiftStationSmaller");

            Add(MyMwcVoxelFilesEnum.Flat_256x64x256, new MyMwcVector3Int(256, 64, 256), "Flat_256x64x256");
            Add(MyMwcVoxelFilesEnum.Flat_128x64x128, new MyMwcVector3Int(128, 64, 128), "Flat_128x64x128");
            Add(MyMwcVoxelFilesEnum.Flat_512x64x512, new MyMwcVector3Int(512, 64, 512), "Flat_512x64x512");

            Add(MyMwcVoxelFilesEnum.PirateBaseStaticAsteroid_A_1000m, new MyMwcVector3Int(128, 128, 128), "PirateBaseStaticAsteroid_A_1000m");
            Add(MyMwcVoxelFilesEnum.PirateBaseStaticAsteroid_A_5000m_1, new MyMwcVector3Int(384, 384, 384), "PirateBaseStaticAsteroid_A_5000m_1");
            Add(MyMwcVoxelFilesEnum.PirateBaseStaticAsteroid_A_5000m_2, new MyMwcVector3Int(384, 384, 384), "PirateBaseStaticAsteroid_A_5000m_2");

            Add(MyMwcVoxelFilesEnum.d25asteroid_field, new MyMwcVector3Int(128, 64, 128), "d25asteroid_field");
            Add(MyMwcVoxelFilesEnum.d25city_fight, new MyMwcVector3Int(128, 64, 128), "d25city_fight");
            Add(MyMwcVoxelFilesEnum.d25gates_ofhell, new MyMwcVector3Int(128, 64, 128), "d25gates_ofhell");
            Add(MyMwcVoxelFilesEnum.d25junkyard, new MyMwcVector3Int(128, 64, 128), "d25junkyard");
            Add(MyMwcVoxelFilesEnum.d25military_area, new MyMwcVector3Int(128, 64, 128), "d25military_area");
            Add(MyMwcVoxelFilesEnum.d25miner_outpost, new MyMwcVector3Int(128, 64, 128), "d25miner_outpost");
            Add(MyMwcVoxelFilesEnum.d25plain, new MyMwcVector3Int(128, 64, 128), "d25plain");
            Add(MyMwcVoxelFilesEnum.d25radioactive, new MyMwcVector3Int(128, 64, 128), "d25radioactive");





            

            //  Assert whether we didn't forget on some voxelfile
            for (int i = 0; i < DefaultVoxelFiles.Length; i++)
            {
                MyCommonDebugUtils.AssertDebug(DefaultVoxelFiles[i] != null);
            }
        }

        static void Add(MyMwcVoxelFilesEnum voxelFileEnum, MyMwcVector3Int sizeInVoxels, string filename)
        {
            DefaultVoxelFiles[(int)voxelFileEnum] = new MyVoxelFile(voxelFileEnum, sizeInVoxels, filename);
        }

        public static MyVoxelFile Get(MyMwcVoxelFilesEnum voxelFileEnum)
        {
            return DefaultVoxelFiles[(int)voxelFileEnum];
        }
    }
}
