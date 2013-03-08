using System;
using System.Collections.Generic;
using MinerWarsMath.Graphics;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Localization;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.AppCode.Game.Voxels;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.AppCode.Game.Textures;

namespace MinerWars.AppCode.Game.GUI.Helpers
{
    using System.Linq;
    using SysUtils.Utils;

    public enum MyGuiAsteroidTypesEnum
    {
        VOXEL,
        STATIC
    }

    static class MyGuiAsteroidHelpers
    {
        //Dictionaries for helpers
        static Dictionary<MyMwcVoxelFilesEnum, MyGuiAsteroidHelper> m_voxelFilesHelpers = new Dictionary<MyMwcVoxelFilesEnum, MyGuiAsteroidHelper>();
        static Dictionary<MyMwcVoxelMaterialsEnum, MyGuiVoxelMaterialHelper> m_voxelMaterialHelpers = new Dictionary<MyMwcVoxelMaterialsEnum, MyGuiVoxelMaterialHelper>();
        static Dictionary<MyGuiAsteroidTypesEnum, MyGuiAsteroidHelper> m_guiAsteroidTypeHelpers = new Dictionary<MyGuiAsteroidTypesEnum, MyGuiAsteroidHelper>();
        static Dictionary<MyMwcObjectBuilder_StaticAsteroid_TypesEnum, MyGuiAsteroidHelper> m_staticAsteroidHelpers = new Dictionary<MyMwcObjectBuilder_StaticAsteroid_TypesEnum, MyGuiAsteroidHelper>();

        //Arrays of enums values
        public static List<MyMwcVoxelFilesEnum> MyMwcVoxelFilesEnumValues { get; private set; }
        public static List<MyMwcVoxelMaterialsEnum> MyMwcVoxelMaterialsEnumValues { get; private set; }
        public static List<MyGuiAsteroidTypesEnum> MyGuiAsteroidTypeEnumValues { get; private set; }
        public static List<MyMwcObjectBuilder_StaticAsteroid_TypesEnum> MyMwcStaticAsteroidTypesEnumValues { get; private set; }
        public static List<MyMwcObjectBuilder_Ore_TypesEnum> MyMwcOreTypesEnumValues { get; private set; }

        static MyGuiAsteroidHelpers()
        {
            MyMwcLog.WriteLine("MyGuiAsteroidHelpers()");

            MyMwcVoxelFilesEnumValues = Enum.GetValues(typeof(MyMwcVoxelFilesEnum)).Cast<MyMwcVoxelFilesEnum>().ToList();
            MyMwcVoxelMaterialsEnumValues = Enum.GetValues(typeof(MyMwcVoxelMaterialsEnum)).Cast<MyMwcVoxelMaterialsEnum>().ToList();
            MyGuiAsteroidTypeEnumValues = Enum.GetValues(typeof(MyGuiAsteroidTypesEnum)).Cast<MyGuiAsteroidTypesEnum>().ToList();
            MyMwcStaticAsteroidTypesEnumValues = Enum.GetValues(typeof(MyMwcObjectBuilder_StaticAsteroid_TypesEnum)).Cast<MyMwcObjectBuilder_StaticAsteroid_TypesEnum>().ToList();
            MyMwcOreTypesEnumValues = Enum.GetValues(typeof(MyMwcObjectBuilder_Ore_TypesEnum)).Cast<MyMwcObjectBuilder_Ore_TypesEnum>().ToList();
        }

        public static MyGuiAsteroidHelper GetMyGuiVoxelFileHelper(MyMwcVoxelFilesEnum voxelFile)
        {
            MyGuiAsteroidHelper ret;
            if (m_voxelFilesHelpers.TryGetValue(voxelFile, out ret))
                return ret;
            else
                return null;
        }

        public static MyGuiVoxelMaterialHelper GetMyGuiVoxelMaterialHelper(MyMwcVoxelMaterialsEnum voxelMaterial)
        {
            MyGuiVoxelMaterialHelper ret;
            if (m_voxelMaterialHelpers.TryGetValue(voxelMaterial, out ret))
                return ret;
            else
                return null;
        }

        public static MyGuiAsteroidHelper GetMyGuiAsteroidTypeHelper(MyGuiAsteroidTypesEnum asteroidType)
        {
            MyGuiAsteroidHelper ret;
            if (m_guiAsteroidTypeHelpers.TryGetValue(asteroidType, out ret))
                return ret;
            else
                return null;
        }

        public static MyGuiAsteroidHelper GetStaticAsteroidTypeHelper(MyMwcObjectBuilder_StaticAsteroid_TypesEnum staticAsteroidType)
        {
            MyGuiAsteroidHelper ret;
            if (m_staticAsteroidHelpers.TryGetValue(staticAsteroidType, out ret))
                return ret;
            else
                return null;
        }

        public static void UnloadContent()
        {
            m_voxelFilesHelpers.Clear();
            m_voxelMaterialHelpers.Clear();
            m_guiAsteroidTypeHelpers.Clear();
            m_staticAsteroidHelpers.Clear();
        }

        public static void LoadContent()
        {
            #region Create voxel asteroid helpers

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.PerfectSphereSplitted_512x512x512,
                new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(MyVoxelFiles.Get(MyMwcVoxelFilesEnum.PerfectSphereSplitted_512x512x512).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.PerfectSphereSplitted_512x512x512));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.PerfectSphereWithFewTunnels_512x512x512,
                new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(MyVoxelFiles.Get(MyMwcVoxelFilesEnum.PerfectSphereWithFewTunnels_512x512x512).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.PerfectSphereWithFewTunnels_512x512x512));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.PerfectSphereWithMassiveTunnels_512x512x512,
                new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(MyVoxelFiles.Get(MyMwcVoxelFilesEnum.PerfectSphereWithMassiveTunnels_512x512x512).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.PerfectSphereWithMassiveTunnels_512x512x512));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.PerfectSphereWithMassiveTunnels2_512x512x512,
                new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(MyVoxelFiles.Get(MyMwcVoxelFilesEnum.PerfectSphereWithMassiveTunnels2_512x512x512).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.PerfectSphereWithMassiveTunnels2_512x512x512));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.PerfectSphereWithRaceTunnel_512x512x512,
                new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(MyVoxelFiles.Get(MyMwcVoxelFilesEnum.PerfectSphereWithRaceTunnel_512x512x512).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.PerfectSphereWithRaceTunnel_512x512x512));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.PerfectSphereWithRaceTunnel2_512x512x512,
                new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(MyVoxelFiles.Get(MyMwcVoxelFilesEnum.PerfectSphereWithRaceTunnel2_512x512x512).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.PerfectSphereWithRaceTunnel2_512x512x512));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.SphereWithLargeCutOut_128x128x128,
                new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(MyVoxelFiles.Get(MyMwcVoxelFilesEnum.SphereWithLargeCutOut_128x128x128).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.SphereWithLargeCutOut_128x128x128));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.TorusStorySector_256x128x256,
                new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(MyVoxelFiles.Get(MyMwcVoxelFilesEnum.TorusStorySector_256x128x256).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.TorusStorySector_256x128x256));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.TorusWithManyTunnels_256x128x256,
                new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(MyVoxelFiles.Get(MyMwcVoxelFilesEnum.TorusWithManyTunnels_256x128x256).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.TorusWithManyTunnels_256x128x256));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.TorusWithSmallTunnel_256x128x256,
                new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(MyVoxelFiles.Get(MyMwcVoxelFilesEnum.TorusWithSmallTunnel_256x128x256).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.TorusWithSmallTunnel_256x128x256));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.VerticalIsland_128x128x128,
                new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(MyVoxelFiles.Get(MyMwcVoxelFilesEnum.VerticalIsland_128x128x128).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.VerticalIsland_128x128x128));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.VerticalIsland_128x256x128,
                new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(MyVoxelFiles.Get(MyMwcVoxelFilesEnum.VerticalIsland_128x256x128).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.VerticalIsland_128x256x128));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.VerticalIslandStorySector_128x256x128,
                new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(MyVoxelFiles.Get(MyMwcVoxelFilesEnum.VerticalIslandStorySector_128x256x128).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.VerticalIslandStorySector_128x256x128));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.DeformedSphere1_64x64x64,
                new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(MyVoxelFiles.Get(MyMwcVoxelFilesEnum.DeformedSphere1_64x64x64).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.DeformedSphere1_64x64x64));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.DeformedSphere2_64x64x64,
                new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(MyVoxelFiles.Get(MyMwcVoxelFilesEnum.DeformedSphere2_64x64x64).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.DeformedSphere2_64x64x64));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.DeformedSphereWithCorridor_128x64x64,
                new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(MyVoxelFiles.Get(MyMwcVoxelFilesEnum.DeformedSphereWithCorridor_128x64x64).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.DeformedSphereWithCorridor_128x64x64));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.DeformedSphereWithCorridor_256x256x256,
                new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(MyVoxelFiles.Get(MyMwcVoxelFilesEnum.DeformedSphereWithCorridor_256x256x256).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.DeformedSphereWithCorridor_256x256x256));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.DeformedSphereWithCraters_128x128x128,
                new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(MyVoxelFiles.Get(MyMwcVoxelFilesEnum.DeformedSphereWithCraters_128x128x128).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.DeformedSphereWithCraters_128x128x128));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.ScratchedBoulder_128x128x128,
                new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(MyVoxelFiles.Get(MyMwcVoxelFilesEnum.ScratchedBoulder_128x128x128).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.ScratchedBoulder_128x128x128));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.DeformedSphereWithHoles_64x128x64,
                new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(MyVoxelFiles.Get(MyMwcVoxelFilesEnum.DeformedSphereWithHoles_64x128x64).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.DeformedSphereWithHoles_64x128x64));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.TorusWithManyTunnels_2_256x128x256,
                new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(MyVoxelFiles.Get(MyMwcVoxelFilesEnum.TorusWithManyTunnels_2_256x128x256).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.TorusWithManyTunnels_2_256x128x256));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.VoxelImporterTest,
                new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(MyVoxelFiles.Get(MyMwcVoxelFilesEnum.VoxelImporterTest).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.VoxelImportTest));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Fortress,
                new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Fortress).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.Fortress));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.AsteroidWithSpaceStationStartStorySector, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
                MyVoxelFiles.Get(MyMwcVoxelFilesEnum.AsteroidWithSpaceStationStartStorySector).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
                MyTextsWrapperEnum.AsteroidWithSpaceStationStartStorySector));


            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.RussianWarehouse,
                                    new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
                                        MyVoxelFiles.Get(MyMwcVoxelFilesEnum.RussianWarehouse).GetIconFilePath(),
                                        flags: TextureFlags.IgnoreQuality),
                                                            MyTextsWrapperEnum.RussianWarehouseAsteroid));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.MothershipFacility,
                                    new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
                                        MyVoxelFiles.Get(MyMwcVoxelFilesEnum.MothershipFacility).GetIconFilePath(),
                                        flags: TextureFlags.IgnoreQuality),
                                                            MyTextsWrapperEnum.mothership_facilityAsteroid));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.SlaverBase,
                                    new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
                                        MyVoxelFiles.Get(MyMwcVoxelFilesEnum.SlaverBase).GetIconFilePath(),
                                        flags: TextureFlags.IgnoreQuality),
                                                            MyTextsWrapperEnum.slaver_baseAsteroid));


            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.ResearchVessel,
                                    new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
                                        MyVoxelFiles.Get(MyMwcVoxelFilesEnum.ResearchVessel).GetIconFilePath(),
                                        flags: TextureFlags.IgnoreQuality),
                                                            MyTextsWrapperEnum.research_vesselAsteroid));


            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.BilitaryBase,
                                    new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
                                        MyVoxelFiles.Get(MyMwcVoxelFilesEnum.BilitaryBase).GetIconFilePath(),
                                        flags: TextureFlags.IgnoreQuality),
                                                            MyTextsWrapperEnum.military_baseAsteroid));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.reef_ast,
                                    new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
                                        MyVoxelFiles.Get(MyMwcVoxelFilesEnum.reef_ast).GetIconFilePath(),
                                        flags: TextureFlags.IgnoreQuality),
                                                            MyTextsWrapperEnum.reef_ast));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Flat_256x64x256,
                                    new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
                                        MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Flat_256x64x256).GetIconFilePath(),
                                        flags: TextureFlags.IgnoreQuality),
                                                            MyTextsWrapperEnum.Flat_256x64x256));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Flat_128x64x128,
                        new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
                            MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Flat_128x64x128).GetIconFilePath(),
                            flags: TextureFlags.IgnoreQuality),
                                                MyTextsWrapperEnum.Flat_128x64x128));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Flat_512x64x512,
                        new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
                            MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Flat_512x64x512).GetIconFilePath(),
                            flags: TextureFlags.IgnoreQuality),
                                                MyTextsWrapperEnum.Flat_512x64x512));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.PirateBaseStaticAsteroid_A_1000m,
                        new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
                            MyVoxelFiles.Get(MyMwcVoxelFilesEnum.PirateBaseStaticAsteroid_A_1000m).GetIconFilePath(),
                            flags: TextureFlags.IgnoreQuality),
                                                MyTextsWrapperEnum.PirateBaseStaticAsteroid_A_1000m));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.PirateBaseStaticAsteroid_A_5000m_1,
                        new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
                            MyVoxelFiles.Get(MyMwcVoxelFilesEnum.PirateBaseStaticAsteroid_A_5000m_1).GetIconFilePath(),
                            flags: TextureFlags.IgnoreQuality),
                                                MyTextsWrapperEnum.PirateBaseStaticAsteroid_A_5000m_1));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.PirateBaseStaticAsteroid_A_5000m_2,
                        new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
                            MyVoxelFiles.Get(MyMwcVoxelFilesEnum.PirateBaseStaticAsteroid_A_5000m_2).GetIconFilePath(),
                            flags: TextureFlags.IgnoreQuality),
                                                MyTextsWrapperEnum.PirateBaseStaticAsteroid_A_5000m_2));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.d25asteroid_field,
                        new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
                            MyVoxelFiles.Get(MyMwcVoxelFilesEnum.d25asteroid_field).GetIconFilePath(),
                            flags: TextureFlags.IgnoreQuality),
                                                MyTextsWrapperEnum.d25asteroid_field));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.d25city_fight,
                        new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
                            MyVoxelFiles.Get(MyMwcVoxelFilesEnum.d25city_fight).GetIconFilePath(),
                            flags: TextureFlags.IgnoreQuality),
                                                MyTextsWrapperEnum.d25city_fight));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.d25gates_ofhell,
                        new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
                            MyVoxelFiles.Get(MyMwcVoxelFilesEnum.d25gates_ofhell).GetIconFilePath(),
                            flags: TextureFlags.IgnoreQuality),
                                                MyTextsWrapperEnum.d25gates_ofhell));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.d25junkyard,
                        new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
                            MyVoxelFiles.Get(MyMwcVoxelFilesEnum.d25junkyard).GetIconFilePath(),
                            flags: TextureFlags.IgnoreQuality),
                                                MyTextsWrapperEnum.d25junkyard));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.d25military_area,
                        new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
                            MyVoxelFiles.Get(MyMwcVoxelFilesEnum.d25military_area).GetIconFilePath(),
                            flags: TextureFlags.IgnoreQuality),
                                                MyTextsWrapperEnum.d25military_area));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.d25miner_outpost,
                        new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
                            MyVoxelFiles.Get(MyMwcVoxelFilesEnum.d25miner_outpost).GetIconFilePath(),
                            flags: TextureFlags.IgnoreQuality),
                                                MyTextsWrapperEnum.d25miner_outpost));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.d25plain,
                        new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
                            MyVoxelFiles.Get(MyMwcVoxelFilesEnum.d25plain).GetIconFilePath(),
                            flags: TextureFlags.IgnoreQuality),
                                                MyTextsWrapperEnum.d25plain));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.d25radioactive,
                        new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
                            MyVoxelFiles.Get(MyMwcVoxelFilesEnum.d25radioactive).GetIconFilePath(),
                            flags: TextureFlags.IgnoreQuality),
                                                MyTextsWrapperEnum.d25radioactive));
            

            // Cubes
            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Cube_128x128x128, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Cube_128x128x128).GetIconFilePath(), flags: TextureFlags.IgnoreQuality), MyTextsWrapperEnum.Cube_128x128x128));
            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Cube_128x128x256, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Cube_128x128x256).GetIconFilePath(), flags: TextureFlags.IgnoreQuality), MyTextsWrapperEnum.Cube_128x128x256));
            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Cube_128x128x64, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Cube_128x128x64).GetIconFilePath(), flags: TextureFlags.IgnoreQuality), MyTextsWrapperEnum.Cube_128x128x64));
            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Cube_128x256x128, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Cube_128x256x128).GetIconFilePath(), flags: TextureFlags.IgnoreQuality), MyTextsWrapperEnum.Cube_128x256x128));
            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Cube_128x256x256, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Cube_128x256x256).GetIconFilePath(), flags: TextureFlags.IgnoreQuality), MyTextsWrapperEnum.Cube_128x256x256));
            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Cube_128x256x64, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Cube_128x256x64).GetIconFilePath(), flags: TextureFlags.IgnoreQuality), MyTextsWrapperEnum.Cube_128x256x64));
            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Cube_128x64x128, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Cube_128x64x128).GetIconFilePath(), flags: TextureFlags.IgnoreQuality), MyTextsWrapperEnum.Cube_128x64x128));
            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Cube_128x64x256, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Cube_128x64x256).GetIconFilePath(), flags: TextureFlags.IgnoreQuality), MyTextsWrapperEnum.Cube_128x64x256));
            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Cube_128x64x64, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Cube_128x64x64).GetIconFilePath(), flags: TextureFlags.IgnoreQuality), MyTextsWrapperEnum.Cube_128x64x64));
            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Cube_256x128x128, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Cube_256x128x128).GetIconFilePath(), flags: TextureFlags.IgnoreQuality), MyTextsWrapperEnum.Cube_256x128x128));
            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Cube_256x128x256, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Cube_256x128x256).GetIconFilePath(), flags: TextureFlags.IgnoreQuality), MyTextsWrapperEnum.Cube_256x128x256));
            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Cube_256x128x512, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Cube_256x128x512).GetIconFilePath(), flags: TextureFlags.IgnoreQuality), MyTextsWrapperEnum.Cube_256x128x512));
            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Cube_256x256x128, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Cube_256x256x128).GetIconFilePath(), flags: TextureFlags.IgnoreQuality), MyTextsWrapperEnum.Cube_256x256x128));
            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Cube_256x256x256, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Cube_256x256x256).GetIconFilePath(), flags: TextureFlags.IgnoreQuality), MyTextsWrapperEnum.Cube_256x256x256));
            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Cube_256x256x512, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Cube_256x256x512).GetIconFilePath(), flags: TextureFlags.IgnoreQuality), MyTextsWrapperEnum.Cube_256x256x512));
            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Cube_256x512x128, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Cube_256x512x128).GetIconFilePath(), flags: TextureFlags.IgnoreQuality), MyTextsWrapperEnum.Cube_256x512x128));
            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Cube_256x512x256, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Cube_256x512x256).GetIconFilePath(), flags: TextureFlags.IgnoreQuality), MyTextsWrapperEnum.Cube_256x512x256));
            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Cube_256x512x512, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Cube_256x512x512).GetIconFilePath(), flags: TextureFlags.IgnoreQuality), MyTextsWrapperEnum.Cube_256x512x512));
            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Cube_512x256x256, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Cube_512x256x256).GetIconFilePath(), flags: TextureFlags.IgnoreQuality), MyTextsWrapperEnum.Cube_512x256x256));
            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Cube_512x256x512, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Cube_512x256x512).GetIconFilePath(), flags: TextureFlags.IgnoreQuality), MyTextsWrapperEnum.Cube_512x256x512));
            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Cube_512x512x256, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Cube_512x512x256).GetIconFilePath(), flags: TextureFlags.IgnoreQuality), MyTextsWrapperEnum.Cube_512x512x256));
            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Cube_512x512x512, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Cube_512x512x512).GetIconFilePath(), flags: TextureFlags.IgnoreQuality), MyTextsWrapperEnum.Cube_512x512x512));
            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Cube_64x128x128, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Cube_64x128x128).GetIconFilePath(), flags: TextureFlags.IgnoreQuality), MyTextsWrapperEnum.Cube_64x128x128));
            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Cube_64x128x64, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Cube_64x128x64).GetIconFilePath(), flags: TextureFlags.IgnoreQuality), MyTextsWrapperEnum.Cube_64x128x64));
            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Cube_64x64x128, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Cube_64x64x128).GetIconFilePath(), flags: TextureFlags.IgnoreQuality), MyTextsWrapperEnum.Cube_64x64x128));
            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Cube_64x64x64, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Cube_64x64x64).GetIconFilePath(), flags: TextureFlags.IgnoreQuality), MyTextsWrapperEnum.Cube_64x64x64));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Story02, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
                MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Story02).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
                MyTextsWrapperEnum.VoxelStory02));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Mission01_01, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
                MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Mission01_01).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
                MyTextsWrapperEnum.VoxelMission01_01));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Mission01_02, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
                MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Mission01_02).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
                MyTextsWrapperEnum.VoxelMission01_02));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Mission07_01, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
                MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Mission07_01).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
                MyTextsWrapperEnum.VoxelMission07_01));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Mission01_asteroid_big, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
                MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Mission01_asteroid_big).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
                MyTextsWrapperEnum.VoxelMission01_asteroid_big));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Mission01_asteroid_mine, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
                MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Mission01_asteroid_mine).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
                MyTextsWrapperEnum.VoxelMission01_asteroid_mine));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.SphereFull_64, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
                MyVoxelFiles.Get(MyMwcVoxelFilesEnum.SphereFull_64).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
                MyTextsWrapperEnum.SphereFull_64));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.SphereFull_128, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
                MyVoxelFiles.Get(MyMwcVoxelFilesEnum.SphereFull_128).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
                MyTextsWrapperEnum.SphereFull_128));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.SphereFull_256, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
                MyVoxelFiles.Get(MyMwcVoxelFilesEnum.SphereFull_256).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
                MyTextsWrapperEnum.SphereFull_256));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.SphereFull_512, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
                MyVoxelFiles.Get(MyMwcVoxelFilesEnum.SphereFull_512).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
                MyTextsWrapperEnum.SphereFull_512));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.SphereFull_1024, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
                MyVoxelFiles.Get(MyMwcVoxelFilesEnum.SphereFull_512).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
                MyTextsWrapperEnum.SphereFull_1024));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.EacPrisonAsteroid, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
                MyVoxelFiles.Get(MyMwcVoxelFilesEnum.EacPrisonAsteroid).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
                MyTextsWrapperEnum.EacPrisonAsteroid));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.piratebase_export, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
                MyVoxelFiles.Get(MyMwcVoxelFilesEnum.piratebase_export).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
                MyTextsWrapperEnum.piratebase_export));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Chinese_Mines_CenterAsteroid, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
    MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Chinese_Mines_CenterAsteroid).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
    MyTextsWrapperEnum.Chinese_Mines_CenterAsteroid));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Chinese_Mines_FrontAsteroid, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
    MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Chinese_Mines_FrontAsteroid).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
    MyTextsWrapperEnum.Chinese_Mines_FrontAsteroid));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Chinese_Mines_FrontRightAsteroid, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
    MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Chinese_Mines_FrontRightAsteroid).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
    MyTextsWrapperEnum.Chinese_Mines_FrontRightAsteroid));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Chinese_Mines_LeftAsteroid, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
    MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Chinese_Mines_LeftAsteroid).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
    MyTextsWrapperEnum.Chinese_Mines_LeftAsteroid));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Chinese_Mines_MainAsteroid, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
    MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Chinese_Mines_MainAsteroid).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
    MyTextsWrapperEnum.Chinese_Mines_MainAsteroid));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Chinese_Mines_RightAsteroid, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
    MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Chinese_Mines_RightAsteroid).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
    MyTextsWrapperEnum.Chinese_Mines_RightAsteroid));


            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.TowerWithConcreteBlock1, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
    MyVoxelFiles.Get(MyMwcVoxelFilesEnum.TowerWithConcreteBlock1).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
    MyTextsWrapperEnum.TowerWithConcreteBlock1));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.TowerWithConcreteBlock2, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
    MyVoxelFiles.Get(MyMwcVoxelFilesEnum.TowerWithConcreteBlock2).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
    MyTextsWrapperEnum.TowerWithConcreteBlock2));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Warehouse, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
                MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Warehouse).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
                MyTextsWrapperEnum.Warehouse));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Barths_moon_base, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
           MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Barths_moon_base).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
           MyTextsWrapperEnum.Barths_moon_base));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Barths_moon_satelite, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
          MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Barths_moon_satelite).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
          MyTextsWrapperEnum.Barths_moon_satelite));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Fort_valiant_base, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
          MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Fort_valiant_base).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
          MyTextsWrapperEnum.Fort_valiant_base));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Fort_valiant_dungeon, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
          MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Fort_valiant_dungeon).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
          MyTextsWrapperEnum.Fort_valiant_dungeon));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.JunkYardInhabited_256x128x256, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
      MyVoxelFiles.Get(MyMwcVoxelFilesEnum.JunkYardInhabited_256x128x256).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
      MyTextsWrapperEnum.JunkYardInhabited_256x128x256));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.JunkYardToxic_128x128x128, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
      MyVoxelFiles.Get(MyMwcVoxelFilesEnum.JunkYardToxic_128x128x128).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
      MyTextsWrapperEnum.JunkYardToxic_128x128x128));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Empty_512x512x512, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
      MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Empty_512x512x512).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
      MyTextsWrapperEnum.Empty512x512x512));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.JunkYardForge_256x256x256, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
      MyVoxelFiles.Get(MyMwcVoxelFilesEnum.JunkYardForge_256x256x256).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
      MyTextsWrapperEnum.JunkYardForge_256x256x256));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Barths_moon_camp, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
       MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Barths_moon_camp).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
       MyTextsWrapperEnum.Barths_moon_camp));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.rift_base_bigger, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
       MyVoxelFiles.Get(MyMwcVoxelFilesEnum.rift_base_bigger).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
       MyTextsWrapperEnum.Asteroid_Rift_base_bigger));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.rift_base_smaller, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
       MyVoxelFiles.Get(MyMwcVoxelFilesEnum.rift_base_smaller).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
       MyTextsWrapperEnum.Asteroid_Rift_base_smaller));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.barths_moon_lab1, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
     MyVoxelFiles.Get(MyMwcVoxelFilesEnum.barths_moon_lab1).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
     MyTextsWrapperEnum.barths_moon_lab1));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.barths_moon_lab2, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
     MyVoxelFiles.Get(MyMwcVoxelFilesEnum.barths_moon_lab2).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
     MyTextsWrapperEnum.barths_moon_lab2));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.fort_val_box_128, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
     MyVoxelFiles.Get(MyMwcVoxelFilesEnum.fort_val_box_128).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
     MyTextsWrapperEnum.fort_val_box_128));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.rift, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
       MyVoxelFiles.Get(MyMwcVoxelFilesEnum.rift).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
       MyTextsWrapperEnum.Asteroid_Rift));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.rift_small_1, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
       MyVoxelFiles.Get(MyMwcVoxelFilesEnum.rift_base_smaller).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
       MyTextsWrapperEnum.Asteroid_Rift_small_1));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.rift_small_2, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
       MyVoxelFiles.Get(MyMwcVoxelFilesEnum.rift_base_smaller).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
       MyTextsWrapperEnum.Asteroid_Rift_small_2));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Junkyard_Race_256x256x256, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
     MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Junkyard_Race_256x256x256).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
     MyTextsWrapperEnum.Junkyard_Race_256x256x256));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Junkyard_RaceAsteroid_256x256x256, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
     MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Junkyard_RaceAsteroid_256x256x256).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
     MyTextsWrapperEnum.Junkyard_RaceAsteroid_256x256x256));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.ChineseRefinery_First_128x128x128, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
MyVoxelFiles.Get(MyMwcVoxelFilesEnum.ChineseRefinery_First_128x128x128).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
MyTextsWrapperEnum.ChineseRefinery_First_128x128x128));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.ChineseRefinery_Second_128x128x128, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
MyVoxelFiles.Get(MyMwcVoxelFilesEnum.ChineseRefinery_Second_128x128x128).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
MyTextsWrapperEnum.ChineseRefinery_Second_128x128x128));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.ChineseRefinery_Third_128x256x128, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
MyVoxelFiles.Get(MyMwcVoxelFilesEnum.ChineseRefinery_Third_128x256x128).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
MyTextsWrapperEnum.ChineseRefinery_Third_128x256x128));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Chinese_Corridor_Last_126x126x126, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Chinese_Corridor_Last_126x126x126).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
MyTextsWrapperEnum.Chinese_Corridor_Last_126x126x126));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Chinese_Corridor_Tunnel_256x256x256, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Chinese_Corridor_Tunnel_256x256x256).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
MyTextsWrapperEnum.Chinese_Corridor_Tunnel_256x256x256));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Bioresearch, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
                MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Bioresearch).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
                MyTextsWrapperEnum.Bioresearch));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.small2_asteroids, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
               MyVoxelFiles.Get(MyMwcVoxelFilesEnum.small2_asteroids).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
               MyTextsWrapperEnum.small2_asteroids));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.small3_asteroids, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
               MyVoxelFiles.Get(MyMwcVoxelFilesEnum.small3_asteroids).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
               MyTextsWrapperEnum.small3_asteroids));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.many_medium_asteroids, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
               MyVoxelFiles.Get(MyMwcVoxelFilesEnum.many_medium_asteroids).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
               MyTextsWrapperEnum.many_medium_asteroids));

             m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.many_small_asteroids, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
               MyVoxelFiles.Get(MyMwcVoxelFilesEnum.many_small_asteroids).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
               MyTextsWrapperEnum.many_small_asteroids));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.many2_small_asteroids, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
               MyVoxelFiles.Get(MyMwcVoxelFilesEnum.many2_small_asteroids).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
               MyTextsWrapperEnum.many2_small_asteroids));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.rus_attack, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
               MyVoxelFiles.Get(MyMwcVoxelFilesEnum.rus_attack).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
               MyTextsWrapperEnum.rus_attack));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.hopebase512, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
               MyVoxelFiles.Get(MyMwcVoxelFilesEnum.hopebase512).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
               MyTextsWrapperEnum.hopebase512));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.hopefood128, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
               MyVoxelFiles.Get(MyMwcVoxelFilesEnum.hopefood128).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
               MyTextsWrapperEnum.hopefood128));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.hopevault128, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
              MyVoxelFiles.Get(MyMwcVoxelFilesEnum.hopevault128).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
              MyTextsWrapperEnum.hopevault128));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.New_Jerusalem_Asteroid, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
                MyVoxelFiles.Get(MyMwcVoxelFilesEnum.New_Jerusalem_Asteroid).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
                MyTextsWrapperEnum.New_Jerusalem_Asteroid));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Chinese_Transmitter_Asteroid, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
                MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Chinese_Transmitter_Asteroid).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
                MyTextsWrapperEnum.Chinese_Transmitter_Asteroid));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Slaver_Base_2_Asteroid, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
                MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Slaver_Base_2_Asteroid).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
                MyTextsWrapperEnum.Slaver_Base_2_Asteroid));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Small_Pirate_Base_2_Asteroid, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
                MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Small_Pirate_Base_2_Asteroid).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
                MyTextsWrapperEnum.Small_Pirate_Base_2_Asteroid));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Small_Pirate_Base_Asteroid, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
                MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Small_Pirate_Base_Asteroid).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
                MyTextsWrapperEnum.Small_Pirate_Base_Asteroid));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Solar_Factory_EAC_Asteroid, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
                MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Solar_Factory_EAC_Asteroid).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
                MyTextsWrapperEnum.Solar_Factory_EAC_Asteroid));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Mines_Asteroid, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
                MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Mines_Asteroid).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
                MyTextsWrapperEnum.Mines_Asteroid));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Small_Pirate_Base_3_1, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
               MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Small_Pirate_Base_3_1).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
               MyTextsWrapperEnum.Small_Pirate_Base_3_1));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Small_Pirate_Base_3_2, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
              MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Small_Pirate_Base_3_2).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
              MyTextsWrapperEnum.Small_Pirate_Base_3_2));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Small_Pirate_Base_3_3, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
              MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Small_Pirate_Base_3_3).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
              MyTextsWrapperEnum.Small_Pirate_Base_3_3));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Voxel_Arena_Tunnels, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
              MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Voxel_Arena_Tunnels).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
              MyTextsWrapperEnum.Voxel_Arena_Tunnels));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Novaja_Zemlja, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
              MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Novaja_Zemlja).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
              MyTextsWrapperEnum.Novaja_Zemlja));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.EACSurvaySmaller_256_256_256, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
              MyVoxelFiles.Get(MyMwcVoxelFilesEnum.EACSurvaySmaller_256_256_256).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
              MyTextsWrapperEnum.EACSurvaySmaller_256_256_256));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.EACSurveyBigger_512_256_256, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
              MyVoxelFiles.Get(MyMwcVoxelFilesEnum.EACSurveyBigger_512_256_256).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
              MyTextsWrapperEnum.EACSurveyBigger_512_256_256));



            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Laika1_128_128_128, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
              MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Laika1_128_128_128).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
              MyTextsWrapperEnum.Laika1_128_128_128));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Laika2_64_64_64, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
              MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Laika2_64_64_64).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
              MyTextsWrapperEnum.Laika2_64_64_64));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Laika3_64_64_64, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
              MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Laika3_64_64_64).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
              MyTextsWrapperEnum.Laika3_64_64_64));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Laika4_256_128_128, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
              MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Laika4_256_128_128).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
              MyTextsWrapperEnum.Laika4_256_128_128));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Laika5_128_128_128, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
              MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Laika5_128_128_128).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
              MyTextsWrapperEnum.Laika5_128_128_128));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Laika6_64_64_64, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
              MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Laika6_64_64_64).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
              MyTextsWrapperEnum.Laika6_64_64_64));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Laika7_64_64_64, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
              MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Laika7_64_64_64).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
              MyTextsWrapperEnum.Laika7_64_64_64));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Laika8_128_128_128, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
              MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Laika8_128_128_128).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
              MyTextsWrapperEnum.Laika8_128_128_128));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Laika9_128_128_128, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
              MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Laika9_128_128_128).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
              MyTextsWrapperEnum.Laika9_128_128_128));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Arabian_Border_1, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
             MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Arabian_Border_1).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
             MyTextsWrapperEnum.Arabian_Border_1));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Arabian_Border_2, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
             MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Arabian_Border_2).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
             MyTextsWrapperEnum.Arabian_Border_2));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Arabian_Border_3, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
             MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Arabian_Border_3).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
             MyTextsWrapperEnum.Arabian_Border_3));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Arabian_Border_4, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
             MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Arabian_Border_4).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
             MyTextsWrapperEnum.Arabian_Border_4));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Arabian_Border_5, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
             MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Arabian_Border_5).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
             MyTextsWrapperEnum.Arabian_Border_5));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Arabian_Border_6, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
             MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Arabian_Border_6).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
             MyTextsWrapperEnum.Arabian_Border_6));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Arabian_Border_7, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
             MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Arabian_Border_7).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
             MyTextsWrapperEnum.Arabian_Border_7));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Arabian_Border_Arabian, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
             MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Arabian_Border_Arabian).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
             MyTextsWrapperEnum.Arabian_Border_Arabian));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Arabian_Border_EAC, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
             MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Arabian_Border_EAC).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
             MyTextsWrapperEnum.Arabian_Border_EAC));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Chinese_Corridor_1, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
             MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Chinese_Corridor_1).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
             MyTextsWrapperEnum.Chinese_Corridor_1));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Grave_Skull, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
             MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Grave_Skull).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
             MyTextsWrapperEnum.Grave_Skull));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Pirate_Base_1, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
             MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Pirate_Base_1).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
             MyTextsWrapperEnum.Pirate_Base_Asteroid_1));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Pirate_Base_2, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
             MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Pirate_Base_2).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
             MyTextsWrapperEnum.Pirate_Base_Asteroid_2));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Pirate_Base_3, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
             MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Pirate_Base_3).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
             MyTextsWrapperEnum.Pirate_Base_Asteroid_3));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Plutonium_Mines, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
             MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Plutonium_Mines).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
             MyTextsWrapperEnum.Plutonium_Mines));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Fragile_Sector, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
             MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Fragile_Sector).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
             MyTextsWrapperEnum.Fragile_Sector));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Chinese_Solar_Array_Bottom, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
             MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Chinese_Solar_Array_Bottom).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
             MyTextsWrapperEnum.Chinese_Solar_Array_Bottom));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Chinese_Solar_Array_Main, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
             MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Chinese_Solar_Array_Main).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
             MyTextsWrapperEnum.Chinese_Solar_Array_Main));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Chinese_Mines_Small, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
             MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Chinese_Mines_Small).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
             MyTextsWrapperEnum.Chinese_Mines_Small));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Chinese_Mines_Side, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
             MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Chinese_Mines_Side).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
             MyTextsWrapperEnum.Chinese_Mines_Side));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Hippie_Outpost_Base, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
             MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Hippie_Outpost_Base).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
             MyTextsWrapperEnum.Hippie_Outpost_Base));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Hippie_Outpost_Tree, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
             MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Hippie_Outpost_Tree).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
             MyTextsWrapperEnum.Hippie_Outpost_Tree));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Laika_1, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
             MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Laika_1).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
             MyTextsWrapperEnum.Laika_1));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Laika_2, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
             MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Laika_2).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
             MyTextsWrapperEnum.Laika_2));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Laika_3, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
             MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Laika_3).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
             MyTextsWrapperEnum.Laika_3));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Fortress_Sanc_1, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
             MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Fortress_Sanc_1).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
             MyTextsWrapperEnum.Fortress_Sanc_1));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Fortress_Sanc_2, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
             MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Fortress_Sanc_2).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
             MyTextsWrapperEnum.Fortress_Sanc_2));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Fortress_Sanc_3, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
             MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Fortress_Sanc_3).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
             MyTextsWrapperEnum.Fortress_Sanc_3));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Fortress_Sanc_4, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
             MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Fortress_Sanc_4).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
             MyTextsWrapperEnum.Fortress_Sanc_4));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Fortress_Sanc_5, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
             MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Fortress_Sanc_5).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
             MyTextsWrapperEnum.Fortress_Sanc_5));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Fortress_Sanc_6, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
             MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Fortress_Sanc_6).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
             MyTextsWrapperEnum.Fortress_Sanc_6));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Fortress_Sanc_7, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
             MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Fortress_Sanc_7).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
             MyTextsWrapperEnum.Fortress_Sanc_7));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Fortress_Sanc_8, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
             MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Fortress_Sanc_8).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
             MyTextsWrapperEnum.Fortress_Sanc_8));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Fortress_Sanc_9, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
             MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Fortress_Sanc_9).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
             MyTextsWrapperEnum.Fortress_Sanc_9));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Fortress_Sanc_10, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
             MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Fortress_Sanc_10).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
             MyTextsWrapperEnum.Fortress_Sanc_10));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Fortress_Sanc_11, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
             MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Fortress_Sanc_11).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
             MyTextsWrapperEnum.Fortress_Sanc_11));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Russian_Transmitter_1, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
             MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Russian_Transmitter_1).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
             MyTextsWrapperEnum.Russian_Transmitter_1));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Russian_Transmitter_2, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
             MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Russian_Transmitter_2).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
             MyTextsWrapperEnum.Russian_Transmitter_2));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Russian_Transmitter_3, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
             MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Russian_Transmitter_3).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
             MyTextsWrapperEnum.Russian_Transmitter_3));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Russian_Transmitter_Main, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
            MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Russian_Transmitter_Main).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
            MyTextsWrapperEnum.Russian_Transmitter_Main));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Russian_Warehouse, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
             MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Russian_Warehouse).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
             MyTextsWrapperEnum.Russian_Warehouse_New));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.ReichStag_1, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
             MyVoxelFiles.Get(MyMwcVoxelFilesEnum.ReichStag_1).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
             MyTextsWrapperEnum.ReichStag_1));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.ReichStag_2, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
             MyVoxelFiles.Get(MyMwcVoxelFilesEnum.ReichStag_2).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
             MyTextsWrapperEnum.ReichStag_2));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.New_Singapore, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
             MyVoxelFiles.Get(MyMwcVoxelFilesEnum.New_Singapore).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
             MyTextsWrapperEnum.New_Singapore));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.HallOfFame, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
             MyVoxelFiles.Get(MyMwcVoxelFilesEnum.HallOfFame).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
             MyTextsWrapperEnum.HallOfFame));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.IceCaveDeathmatch, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
             MyVoxelFiles.Get(MyMwcVoxelFilesEnum.IceCaveDeathmatch).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
             MyTextsWrapperEnum.IceCaveDeathmatch));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.WarehouseDeathmatch, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
             MyVoxelFiles.Get(MyMwcVoxelFilesEnum.WarehouseDeathmatch).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
             MyTextsWrapperEnum.WarehouseDeatmatch));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Nearby_Station_1, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
             MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Nearby_Station_1).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
             MyTextsWrapperEnum.Nearby_Station_1));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Nearby_Station_2, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
             MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Nearby_Station_2).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
             MyTextsWrapperEnum.Nearby_Station_2));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Nearby_Station_3, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
             MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Nearby_Station_3).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
             MyTextsWrapperEnum.Nearby_Station_3));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Nearby_Station_4, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
             MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Nearby_Station_4).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
             MyTextsWrapperEnum.Nearby_Station_4));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Nearby_Station_5, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
             MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Nearby_Station_5).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
             MyTextsWrapperEnum.Nearby_Station_5));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Nearby_Station_6, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
             MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Nearby_Station_6).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
             MyTextsWrapperEnum.Nearby_Station_6));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Nearby_Station_7, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
             MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Nearby_Station_7).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
             MyTextsWrapperEnum.Nearby_Station_7));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Nearby_Station_8, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
             MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Nearby_Station_8).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
             MyTextsWrapperEnum.Nearby_Station_8));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Nearby_Station_9, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
             MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Nearby_Station_9).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
             MyTextsWrapperEnum.Nearby_Station_9));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Nearby_Station_10, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
             MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Nearby_Station_10).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
             MyTextsWrapperEnum.Nearby_Station_10));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Nearby_Station_11, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
             MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Nearby_Station_11).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
             MyTextsWrapperEnum.Nearby_Station_11));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.Nearby_Station_12, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
             MyVoxelFiles.Get(MyMwcVoxelFilesEnum.Nearby_Station_12).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
             MyTextsWrapperEnum.Nearby_Station_12));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.VoxelArenaDeathmatch, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
             MyVoxelFiles.Get(MyMwcVoxelFilesEnum.VoxelArenaDeathmatch).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
             MyTextsWrapperEnum.VoxelArenaDeathmatch));

            m_voxelFilesHelpers.Add(MyMwcVoxelFilesEnum.RiftStationSmaller, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>(
             MyVoxelFiles.Get(MyMwcVoxelFilesEnum.RiftStationSmaller).GetIconFilePath(), flags: TextureFlags.IgnoreQuality),
             MyTextsWrapperEnum.RiftStationSmaller));

            #endregion


            #region Create voxel material helpers

            m_voxelMaterialHelpers.Add(MyMwcVoxelMaterialsEnum.Lava_01,
                new MyGuiVoxelMaterialHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconMaterialLava_01", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.Lava_01));

            m_voxelMaterialHelpers.Add(MyMwcVoxelMaterialsEnum.Cobalt_01,
                new MyGuiVoxelMaterialHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconMaterialCobalt_01", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.Cobalt_01));

            m_voxelMaterialHelpers.Add(MyMwcVoxelMaterialsEnum.Gold_01,
                new MyGuiVoxelMaterialHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconMaterialGold_01", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.Gold_01));

            m_voxelMaterialHelpers.Add(MyMwcVoxelMaterialsEnum.Helium3_01,
                new MyGuiVoxelMaterialHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconMaterialHelium3_01", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.Helium3_01));

            m_voxelMaterialHelpers.Add(MyMwcVoxelMaterialsEnum.Helium4_01,
                new MyGuiVoxelMaterialHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconMaterialHelium4_01", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.Helium4_01));

            m_voxelMaterialHelpers.Add(MyMwcVoxelMaterialsEnum.Ice_01,
                new MyGuiVoxelMaterialHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconMaterialIce_01", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.Ice_01));

            m_voxelMaterialHelpers.Add(MyMwcVoxelMaterialsEnum.Indestructible_01,
                new MyGuiVoxelMaterialHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconMaterialIndestructible_01", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.Indestructible_01));

            m_voxelMaterialHelpers.Add(MyMwcVoxelMaterialsEnum.Indestructible_02,
                new MyGuiVoxelMaterialHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconMaterialIndestructible_02", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.Indestructible_02));

            m_voxelMaterialHelpers.Add(MyMwcVoxelMaterialsEnum.Iron_01,
                new MyGuiVoxelMaterialHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconMaterialIron_01", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.Iron_01));

            m_voxelMaterialHelpers.Add(MyMwcVoxelMaterialsEnum.Iron_02,
                new MyGuiVoxelMaterialHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconMaterialIron_02", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.Iron_02));

            m_voxelMaterialHelpers.Add(MyMwcVoxelMaterialsEnum.Magnesium_01,
                new MyGuiVoxelMaterialHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconMaterialMagnesium_01", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.Magnesium_01));

            m_voxelMaterialHelpers.Add(MyMwcVoxelMaterialsEnum.Nickel_01,
                new MyGuiVoxelMaterialHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconMaterialNickel_01", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.Nickel_01));

            m_voxelMaterialHelpers.Add(MyMwcVoxelMaterialsEnum.Organic_01,
                new MyGuiVoxelMaterialHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconMaterialOrganic_01", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.Organic_01));

            m_voxelMaterialHelpers.Add(MyMwcVoxelMaterialsEnum.Platinum_01,
                new MyGuiVoxelMaterialHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconMaterialPlatinum_01", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.Platinum_01));

            m_voxelMaterialHelpers.Add(MyMwcVoxelMaterialsEnum.Silicon_01,
                new MyGuiVoxelMaterialHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconMaterialSillicon_01", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.Silicon_01));

            m_voxelMaterialHelpers.Add(MyMwcVoxelMaterialsEnum.Silver_01,
                new MyGuiVoxelMaterialHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconMaterialSilver_01", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.Silver_01));

            m_voxelMaterialHelpers.Add(MyMwcVoxelMaterialsEnum.Stone_01,
                new MyGuiVoxelMaterialHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconMaterialStone_01", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.Stone_01));

            m_voxelMaterialHelpers.Add(MyMwcVoxelMaterialsEnum.Stone_02,
                new MyGuiVoxelMaterialHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconMaterialStone_02", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.Stone_02));

            m_voxelMaterialHelpers.Add(MyMwcVoxelMaterialsEnum.Stone_03,
                new MyGuiVoxelMaterialHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconMaterialStone_03", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.Stone_03));

            m_voxelMaterialHelpers.Add(MyMwcVoxelMaterialsEnum.Stone_04,
                new MyGuiVoxelMaterialHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconMaterialStone_04", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.Stone_04));

            m_voxelMaterialHelpers.Add(MyMwcVoxelMaterialsEnum.Stone_05,
                new MyGuiVoxelMaterialHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconMaterialStone_05", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.Stone_05));

            m_voxelMaterialHelpers.Add(MyMwcVoxelMaterialsEnum.Stone_06,
                new MyGuiVoxelMaterialHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconMaterialStone_06", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.Stone_06));

            m_voxelMaterialHelpers.Add(MyMwcVoxelMaterialsEnum.Stone_07,
                new MyGuiVoxelMaterialHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconMaterialStone_07", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.Stone_07));

            m_voxelMaterialHelpers.Add(MyMwcVoxelMaterialsEnum.Stone_08,
                new MyGuiVoxelMaterialHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconMaterialStone_08", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.Stone_08));

            m_voxelMaterialHelpers.Add(MyMwcVoxelMaterialsEnum.Indestructible_03,
                new MyGuiVoxelMaterialHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconMaterialStone_09", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.Indestructible_03));

            m_voxelMaterialHelpers.Add(MyMwcVoxelMaterialsEnum.Stone_10,
                new MyGuiVoxelMaterialHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconMaterialStone_10", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.Stone_10));

            m_voxelMaterialHelpers.Add(MyMwcVoxelMaterialsEnum.Indestructible_04,
                new MyGuiVoxelMaterialHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconMaterialStone_11", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.Indestructible_04));

            m_voxelMaterialHelpers.Add(MyMwcVoxelMaterialsEnum.Indestructible_05_Craters_01,
                new MyGuiVoxelMaterialHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconMaterialStone_12", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.Indestructible_05_Craters_01));

            m_voxelMaterialHelpers.Add(MyMwcVoxelMaterialsEnum.Stone_13_Wall_01,
                new MyGuiVoxelMaterialHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconMaterialStone_13", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.Stone_13_Wall_01));

            m_voxelMaterialHelpers.Add(MyMwcVoxelMaterialsEnum.Treasure_01,
                new MyGuiVoxelMaterialHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconMaterialTreasure_01", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.Treasure_01));

            m_voxelMaterialHelpers.Add(MyMwcVoxelMaterialsEnum.Treasure_02,
                new MyGuiVoxelMaterialHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconMaterialTreasure_02", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.Treasure_02));

            m_voxelMaterialHelpers.Add(MyMwcVoxelMaterialsEnum.Uranite_01,
                new MyGuiVoxelMaterialHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconMaterialUranite_01", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.Uranite_01));

            m_voxelMaterialHelpers.Add(MyMwcVoxelMaterialsEnum.Snow_01,
                new MyGuiVoxelMaterialHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconMaterialSnow_01", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.Snow_01));

            m_voxelMaterialHelpers.Add(MyMwcVoxelMaterialsEnum.Concrete_01,
                new MyGuiVoxelMaterialHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconMaterialConcrete_01", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.Concrete_01));

            m_voxelMaterialHelpers.Add(MyMwcVoxelMaterialsEnum.Concrete_02,
                new MyGuiVoxelMaterialHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconMaterialConcrete_02", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.Concrete_02));

            m_voxelMaterialHelpers.Add(MyMwcVoxelMaterialsEnum.Sandstone_01,
                new MyGuiVoxelMaterialHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconMaterialSandstone_01", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.Sandstone_01));

            m_voxelMaterialHelpers.Add(MyMwcVoxelMaterialsEnum.Stone_Red,
                new MyGuiVoxelMaterialHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconMaterialStone_Red", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.Stone_Red));



            #endregion

            #region Create asteroid type helpers

            m_guiAsteroidTypeHelpers.Add(MyGuiAsteroidTypesEnum.VOXEL, new MyGuiAsteroidHelper(null, MyTextsWrapperEnum.Voxel));
            m_guiAsteroidTypeHelpers.Add(MyGuiAsteroidTypesEnum.STATIC, new MyGuiAsteroidHelper(null, MyTextsWrapperEnum.Static));

            #endregion

            #region Create static asteroid type helpers

            /*
            m_staticAsteroidHelpers.Add(MyMwcObjectBuilder_StaticAsteroid_TypesEnum.JEROMIE,
                new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Jeromie", flags: TextureFlags.IgnoreQuality), MyTextsWrapperEnum.JeromieAsteroid));
            */

            /*
            m_staticAsteroidHelpers.Add(MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid10000m_A, new MyGuiAsteroidHelper(
                MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Jeromie", flags: TextureFlags.IgnoreQuality), MyTextsWrapperEnum.StaticAsteroid01));
            */

            /*
            m_staticAsteroidHelpers.Add(MyMwcObjectBuilder_StaticAsteroid_TypesEnum.STATIC_ASTEROID_02, new MyGuiAsteroidHelper(
                MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\DeformedSphereSmall"), MyTextsWrapperEnum.StaticAsteroid02));
            
            m_staticAsteroidHelpers.Add(MyMwcObjectBuilder_StaticAsteroid_TypesEnum.STATIC_ASTEROID_03, new MyGuiAsteroidHelper(
                MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelprs\\DeformedSphereSmallest"), MyTextsWrapperEnum.StaticAsteroid03));*/

            // TODO: Update textures and texts
            var texture = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Jeromie");


            foreach (var enumValue in MyMwcStaticAsteroidTypesEnumValues)
            {
                //Removed support
                if (((int)enumValue >= (int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid10m_C)
                    &&
                    ((int)enumValue <= (int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid40000m_A))
                    continue;

                MyTextsWrapperEnum description;
                string guiHelperTexture;

                switch (enumValue)
                {
                    case MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid10m_A:
                        description = MyTextsWrapperEnum.StaticAsteroid10m;
                        guiHelperTexture = "StaticAsteroid10m_A";
                        break;
                    case MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid20m_A:
                        description = MyTextsWrapperEnum.StaticAsteroid20m;
                        guiHelperTexture = "StaticAsteroid20m_A";
                        break;
                    case MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid30m_A:
                        description = MyTextsWrapperEnum.StaticAsteroid30m;
                        guiHelperTexture = "StaticAsteroid30m_A";
                        break;
                    case MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid50m_A:
                        description = MyTextsWrapperEnum.StaticAsteroid50m;
                        guiHelperTexture = "StaticAsteroid50m_A";
                        break;
                    case MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid100m_A:
                        description = MyTextsWrapperEnum.StaticAsteroid100m;
                        guiHelperTexture = "StaticAsteroid100m_A";
                        break;
                    case MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid300m_A:
                        description = MyTextsWrapperEnum.StaticAsteroid300m;
                        guiHelperTexture = "StaticAsteroid300m_A";
                        break;
                    case MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid500m_A:
                        description = MyTextsWrapperEnum.StaticAsteroid500m;
                        guiHelperTexture = "StaticAsteroid500m_A";
                        break;
                    case MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid1000m_A:
                        description = MyTextsWrapperEnum.StaticAsteroid1000m;
                        guiHelperTexture = "StaticAsteroid1000m_A";
                        break;
                    case MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid2000m_A:
                        description = MyTextsWrapperEnum.StaticAsteroid2000m;
                        guiHelperTexture = "StaticAsteroid2000m_A";
                        break;
                    case MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid5000m_A:
                        description = MyTextsWrapperEnum.StaticAsteroid5000m;
                        guiHelperTexture = "StaticAsteroid5000m_A";
                        break;
                    case MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid10000m_A:
                        description = MyTextsWrapperEnum.StaticAsteroid10000m;
                        guiHelperTexture = "StaticAsteroid10000m_A";
                        break;
                    case MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid40000m_A:
                        description = MyTextsWrapperEnum.StaticAsteroid40000m;
                        guiHelperTexture = "StaticAsteroid10000m_A";
                        break;
                    case MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid10m_B:
                        description = MyTextsWrapperEnum.StaticAsteroid10m;
                        guiHelperTexture = "StaticAsteroid10m_B";
                        break;
                    case MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid20m_B:
                        description = MyTextsWrapperEnum.StaticAsteroid20m;
                        guiHelperTexture = "StaticAsteroid20m_B";
                        break;
                    case MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid30m_B:
                        description = MyTextsWrapperEnum.StaticAsteroid30m;
                        guiHelperTexture = "StaticAsteroid30m_B";
                        break;
                    case MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid50m_B:
                        description = MyTextsWrapperEnum.StaticAsteroid50m;
                        guiHelperTexture = "StaticAsteroid50m_B";
                        break;
                    case MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid100m_B:
                        description = MyTextsWrapperEnum.StaticAsteroid100m;
                        guiHelperTexture = "StaticAsteroid100m_B";
                        break;
                    case MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid300m_B:
                        description = MyTextsWrapperEnum.StaticAsteroid300m;
                        guiHelperTexture = "StaticAsteroid300m_B";
                        break;
                    case MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid500m_B:
                        description = MyTextsWrapperEnum.StaticAsteroid500m;
                        guiHelperTexture = "StaticAsteroid500m_B";
                        break;
                    case MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid1000m_B:
                        description = MyTextsWrapperEnum.StaticAsteroid1000m;
                        guiHelperTexture = "StaticAsteroid1000m_B";
                        break;
                    case MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid2000m_B:
                        description = MyTextsWrapperEnum.StaticAsteroid2000m;
                        guiHelperTexture = "StaticAsteroid2000m_B";
                        break;
                    case MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid5000m_B:
                        description = MyTextsWrapperEnum.StaticAsteroid5000m;
                        guiHelperTexture = "StaticAsteroid5000m_B";
                        break;
                    case MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid10000m_B:
                        description = MyTextsWrapperEnum.StaticAsteroid10000m;
                        guiHelperTexture = "StaticAsteroid10000m_B";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                m_staticAsteroidHelpers.Add(enumValue, new MyGuiAsteroidHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\" + guiHelperTexture, flags: TextureFlags.IgnoreQuality), description));
            }

            #endregion
        }
    }
}
