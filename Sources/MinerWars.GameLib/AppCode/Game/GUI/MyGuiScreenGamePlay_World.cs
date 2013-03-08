#region Using

using System;
using System.Collections.Generic;
using MinerWarsMath;
using MinerWars.AppCode.Game.GUI.Helpers;
using MinerWars.AppCode.Game.Entities.Prefabs;
using MinerWars.AppCode.Game.Missions;
using MinerWars.AppCode.Game.Models;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Trailer;
using MinerWars.AppCode.Game.TransparentGeometry;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Voxels;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Voxels;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.AppCode.Game.Editor;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.InfluenceSpheres;
using SysUtils;
using System.Linq;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Managers;
using MinerWars.AppCode.Game.Entities.InfluenceSpheres;
using MinerWars.AppCode.Game.World;
using MinerWars.AppCode.Game.SolarSystem;

using System.IO;
using MinerWars.AppCode.Game.Inventory;
using MinerWars.CommonLIB.AppCode.Generics;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Gameplay;
using MinerWars.AppCode.Game.Entities.WayPoints;
using MinerWars.AppCode.Networking.SectorService;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Localization;
using System.Diagnostics;
using MinerWars.AppCode.Game.BackgroundCube;
using MinerWars.AppCode.Game.Missions.SideMissions;
using MinerWars.AppCode.Game.Entities.Ships.AI;
using MinerWars.AppCode.Game.World.Global;
using MinerWars.AppCode.Game.Sessions;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.Prefabs;
using MinerWars.AppCode.Physics;

#endregion

namespace MinerWars.AppCode.Game.GUI
{
    partial class MyGuiScreenGamePlay
    {
        public const int UniverseGeneratorSeed = 0;

        public override string GetFriendlyName()
        {
            return "MyGuiScreenGameBase";
        }

        //  Load ships, large-ships, enemies, etc
        void LoadObjects()
        {
            MyMwcLog.WriteLine("MyGuiScreenGameBase.LoadObjects() - START");
            MyMwcLog.IncreaseIndent();
            int blockId = -1;
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("LoadObjects", ref blockId);

            if (MyMwcFinalBuildConstants.ENABLE_TRAILER_SAVE)
            {
                MyTrailerSave.LoadContent();
            }

            if ((GetGameType() == MyGuiScreenGamePlayType.PURE_FLY_THROUGH ||
                 GetGameType() == MyGuiScreenGamePlayType.MAIN_MENU ||
                 GetGameType() == MyGuiScreenGamePlayType.CREDITS))
            {
                if (MyTrailerLoad.TrailerAnimation != null)
                {
                    MyMwcLog.WriteLine("Loading world for trailer animation: " + MyTrailerLoad.TrailerAnimation.Name);
                    CreateTrailerWorld();
                }
            }
            else
            {
                MyMwcLog.WriteLine("Loading world from data received from server");
                CreateWorldFromDataReceivedFromServer();

                // we need to save it because of player position uptodate
                // Don't save on NEW_STORY, it's saved by mission.accept
                if (!MyFakes.DISABLE_AUTO_SAVE && (m_sessionType.Value == MyMwcStartSessionRequestTypeEnum.LOAD_CHECKPOINT))
                {
                    // When loading checkpoint or template, make full save
                    bool fullSave = m_sessionType.Value == MyMwcStartSessionRequestTypeEnum.LOAD_CHECKPOINT || m_checkpoint.CheckpointName != null;

                    MyMwcLog.WriteLine("Saving world");
                    MySession.Static.SaveLastCheckpoint(); // When new game or travel, don't save checkpoint
                    m_sessionType = MyMwcStartSessionRequestTypeEnum.LOAD_CHECKPOINT;
                }
                if (MyMwcFinalBuildConstants.ENABLE_TRAILER_SAVE)
                {
                    MyTrailerSave.AttachPhysObject("PlayerShip", MySession.PlayerShip);
                }
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock(blockId);
            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyGuiScreenGameBase.LoadObjects() - END");
        }

        private void CreateTrailerWorld()
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("CreateTrailerWorld");

            MyTrailerXmlAnimation animation = MyTrailerLoad.TrailerAnimation;
            if (animation.Name == MyTrailerConstants.FIGHT_ANIMATION)
            {
                CreateFakeWorld1();
            }
            else if (animation.Name == MyTrailerConstants.SHIP_ATTACK_ANIMATION)
            {
                CreateFakeWorld5();
            }
            else if (animation.Name == MyTrailerConstants.ICEFIGHT_ANIMATION)
            {
                CreateFakeWorld2();
            }
            else if (animation.Name == MyTrailerConstants.RACE_ANIMATION)
            {
                CreateFakeWorld3();
            }
            else if (animation.Name == MyTrailerConstants.MENU_ANIMATION)
            {
                CreateFakeWorld4();
            }
            else
            {
                //there is animation, that does not have its world, create default one
                CreateFakeWorld1();
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        private MyMwcObjectBuilder_SmallShip_Player CreatePlayerShip()
        {
            List<MyMwcObjectBuilder_SmallShip_Weapon> weapons = new List<MyMwcObjectBuilder_SmallShip_Weapon>();
            List<MyMwcObjectBuilder_SmallShip_Ammo> ammo = new List<MyMwcObjectBuilder_SmallShip_Ammo>();
            List<MyMwcObjectBuilder_AssignmentOfAmmo> assignments = new List<MyMwcObjectBuilder_AssignmentOfAmmo>();

            weapons.Add(new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Autocanon));
            weapons.Add(new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Autocanon));
            weapons.Add(new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Missile_Launcher));
            weapons.Add(new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Cannon));
            weapons.Add(new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Shotgun));
            weapons.Add(new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Crusher));
            weapons.Add(new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Universal_Launcher_Front));
            weapons.Add(new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Harvesting_Device));
            ammo.Add(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_Basic));
            ammo.Add(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_BioChem));
            ammo.Add(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Automatic_Rifle_With_Silencer_High_Speed));
            ammo.Add(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_High_Speed));
            ammo.Add(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_Basic));
            ammo.Add(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_Tunnel_Buster));
            ammo.Add(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Missile_Basic));
            ammo.Add(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Guided_Missile_Engine_Detection));
            ammo.Add(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Guided_Missile_Visual_Detection));
            ammo.Add(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Shotgun_Basic));
            ammo.Add(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Hologram));
            ammo.Add(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Mine_Basic));
            ammo.Add(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Mine_Smart));
            assignments.Add(new MyMwcObjectBuilder_AssignmentOfAmmo(MyMwcObjectBuilder_FireKeyEnum.Primary, MyMwcObjectBuilder_AmmoGroupEnum.Bullet, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_Basic));
            assignments.Add(new MyMwcObjectBuilder_AssignmentOfAmmo(MyMwcObjectBuilder_FireKeyEnum.Secondary, MyMwcObjectBuilder_AmmoGroupEnum.Missile, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Missile_Basic));
            assignments.Add(new MyMwcObjectBuilder_AssignmentOfAmmo(MyMwcObjectBuilder_FireKeyEnum.Third, MyMwcObjectBuilder_AmmoGroupEnum.UniversalLauncherFront, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Mine_Basic));
            assignments.Add(new MyMwcObjectBuilder_AssignmentOfAmmo(MyMwcObjectBuilder_FireKeyEnum.Fourth, MyMwcObjectBuilder_AmmoGroupEnum.UniversalLauncherBack, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Mine_Smart));
            assignments.Add(new MyMwcObjectBuilder_AssignmentOfAmmo(MyMwcObjectBuilder_FireKeyEnum.Fifth, MyMwcObjectBuilder_AmmoGroupEnum.Bullet, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_Basic));

            MyMwcObjectBuilder_Inventory inventory = new MyMwcObjectBuilder_Inventory(new List<MyMwcObjectBuilder_InventoryItem>(), 1000);
            foreach (MyMwcObjectBuilder_SmallShip_Ammo ammoItem in ammo)
            {
                inventory.InventoryItems.Add(new MyMwcObjectBuilder_InventoryItem(ammoItem, 1000));
            }
            //inventory.InventoryItems.Add(new MyMwcObjectBuilder_InventoryItem(new MyMwcObjectBuilder_FoundationFactory(), 1));            
            inventory.InventoryItems.Add(new MyMwcObjectBuilder_InventoryItem(new MyMwcObjectBuilder_SmallShip_Radar(MyMwcObjectBuilder_SmallShip_Radar_TypesEnum.Radar_1), 1));

            //inventory.InventoryItems.Add(new MyMwcObjectBuilder_InventoryItem(new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Harvesting_Device), 1));

            //return new MyMwcObjectBuilder_SmallShip_Player(
            //    MyMwcObjectBuilder_SmallShip_TypesEnum.GETTYSBURG, 
            //    weapons,
            //    new MyMwcObjectBuilder_SmallShip_Engine(MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.Chemical_1), 
            //    ammo, 
            //    null,
            //    assignments);
            return new MyMwcObjectBuilder_SmallShip_Player(
                MyMwcObjectBuilder_SmallShip_TypesEnum.GETTYSBURG,
                inventory,
                weapons,
                new MyMwcObjectBuilder_SmallShip_Engine(MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.PowerCells_1),
                assignments,
                null,
                new MyMwcObjectBuilder_SmallShip_Radar(MyMwcObjectBuilder_SmallShip_Radar_TypesEnum.Radar_1),
                null,
                MyGameplayConstants.HEALTH_RATIO_MAX,
                100f,
                float.MaxValue,
                float.MaxValue,
                float.MaxValue,
                true, false, MyLightsConstants.MAX_SPOTLIGHT_SHADOW_RANGE, 0);
        }

        /// <summary>
        /// Creates default sector with those 3 voxel asteroid, KAI, few prefabs inside asteroid
        /// </summary>
        /// <param name="objectBuilders"></param>
        static void CreateDefaultSector(List<MyMwcObjectBuilder_Base> objectBuilders)
        {
            objectBuilders.Add(new MyMwcObjectBuilder_Sector() { AreaTemplate = MySolarSystemAreaEnum.Earth });
            objectBuilders.Add(new MyMwcObjectBuilder_VoxelMap(new Vector3(-67, -33, 0) * MyVoxelConstants.VOXEL_SIZE_IN_METRES, MyMwcVoxelFilesEnum.TorusStorySector_256x128x256, MyMwcVoxelMaterialsEnum.Stone_10));
            objectBuilders.Add(new MyMwcObjectBuilder_VoxelMap(new Vector3(-133, 64, -316) * MyVoxelConstants.VOXEL_SIZE_IN_METRES, MyMwcVoxelFilesEnum.VerticalIsland_128x128x128, MyMwcVoxelMaterialsEnum.Stone_05));
            objectBuilders.Add(new MyMwcObjectBuilder_VoxelMap(new Vector3(-20, -452, 15) * MyVoxelConstants.VOXEL_SIZE_IN_METRES, MyMwcVoxelFilesEnum.VerticalIslandStorySector_128x256x128, MyMwcVoxelMaterialsEnum.Indestructible_05_Craters_01));

            /*
            List<MyMwcObjectBuilder_Prefab> prefabsA = new List<MyMwcObjectBuilder_Prefab>();
            prefabsA.Add(new MyMwcObjectBuilder_Prefab(MyMwcObjectBuilder_Prefab_TypesEnum.P381_B01_BUILDING2, new MyMwcVector3Short(1126, -225, 1021), new Vector3(-1.658715f, -0.1217058f, 0.01070096f)));
            prefabsA.Add(new MyMwcObjectBuilder_Prefab(MyMwcObjectBuilder_Prefab_TypesEnum.P381_B01_BUILDING2, new MyMwcVector3Short(1424, -260, 1045), new Vector3(-1.658715f, -0.1217058f, 0.01070098f)));
            prefabsA.Add(new MyMwcObjectBuilder_Prefab(MyMwcObjectBuilder_Prefab_TypesEnum.P381_B01_BUILDING2, new MyMwcVector3Short(1719, -295, 1069), new Vector3(-1.658715f, -0.1217058f, 0.01070098f)));
            prefabsA.Add(new MyMwcObjectBuilder_Prefab(MyMwcObjectBuilder_Prefab_TypesEnum.P381_B01_BUILDING2, new MyMwcVector3Short(2017, -331, 1094), new Vector3(-1.658715f, -0.1217058f, 0.01070098f)));
            prefabsA.Add(new MyMwcObjectBuilder_Prefab(MyMwcObjectBuilder_Prefab_TypesEnum.P381_B01_BUILDING2, new MyMwcVector3Short(2315, -371, 1126), new Vector3(-1.658715f, -0.1217058f, 0.01070098f)));
            prefabsA.Add(new MyMwcObjectBuilder_Prefab(MyMwcObjectBuilder_Prefab_TypesEnum.P381_B01_BUILDING2, new MyMwcVector3Short(2613, -407, 1151), new Vector3(-1.658715f, -0.1217058f, 0.01070098f)));
            prefabsA.Add(new MyMwcObjectBuilder_Prefab(MyMwcObjectBuilder_Prefab_TypesEnum.P381_B01_BUILDING2, new MyMwcVector3Short(2908, -442, 1175), new Vector3(-1.658715f, -0.1217058f, 0.01070098f)));
            prefabsA.Add(new MyMwcObjectBuilder_Prefab(MyMwcObjectBuilder_Prefab_TypesEnum.P381_B01_BUILDING2, new MyMwcVector3Short(3206, -477, 1199), new Vector3(-1.658715f, -0.1217058f, 0.01070098f)));
            prefabsA.Add(new MyMwcObjectBuilder_Prefab(MyMwcObjectBuilder_Prefab_TypesEnum.P211B02_PANEL_60MX60M, new MyMwcVector3Short(5885, -1124, 3545), new Vector3(0, 1.57079637f, -1.57079637f)));
            prefabsA.Add(new MyMwcObjectBuilder_Prefab(MyMwcObjectBuilder_Prefab_TypesEnum.P211B02_PANEL_60MX60M, new MyMwcVector3Short(5885, -1124, 4145), new Vector3(0, 1.57079637f, -1.57079637f)));
            prefabsA.Add(new MyMwcObjectBuilder_Prefab(MyMwcObjectBuilder_Prefab_TypesEnum.P211B02_PANEL_60MX60M, new MyMwcVector3Short(5885, -1744, 2945), new Vector3(-1.57079637f, 0, 1.57079637f)));
            prefabsA.Add(new MyMwcObjectBuilder_Prefab(MyMwcObjectBuilder_Prefab_TypesEnum.P211B02_PANEL_60MX60M, new MyMwcVector3Short(4685, -1124, 4145), new Vector3(0, 1.57079637f, -1.57079637f)));
            prefabsA.Add(new MyMwcObjectBuilder_Prefab(MyMwcObjectBuilder_Prefab_TypesEnum.P211B02_PANEL_60MX60M, new MyMwcVector3Short(4685, -1124, 3545), new Vector3(0, 1.57079637f, -1.57079637f)));
            MyMwcObjectBuilder_PrefabContainer prefabContainerA = new MyMwcObjectBuilder_PrefabContainer(null, MyMwcObjectBuilder_PrefabContainer_TypesEnum.INSTANCE, prefabsA, 0);
            prefabContainerA.PositionAndOrientation.Position = new Vector3(-268.4516f, 122.3787f, 2375.529f);
            prefabContainerA.PositionAndOrientation.Forward = new Vector3(0, 0, -1);
            prefabContainerA.PositionAndOrientation.Up = new Vector3(0, 1, 0);
            objectBuilders.Add(prefabContainerA);

            List<MyMwcObjectBuilder_Prefab> prefabsB = new List<MyMwcObjectBuilder_Prefab>();
            prefabsB.Add(new MyMwcObjectBuilder_Prefab(MyMwcObjectBuilder_Prefab_TypesEnum.P211G03_PANEL_60MX30M, new MyMwcVector3Short(0, 1800, 0), new Vector3(0, 0, 0)));
            MyMwcObjectBuilder_PrefabContainer prefabContainerB = new MyMwcObjectBuilder_PrefabContainer(null, MyMwcObjectBuilder_PrefabContainer_TypesEnum.INSTANCE, prefabsB, 0);
            prefabContainerB.PositionAndOrientation.Position = new Vector3(-1296.336f, 2857.169f, -3711.143f);
            prefabContainerB.PositionAndOrientation.Forward = new Vector3(0, 0, -1);
            prefabContainerB.PositionAndOrientation.Up = new Vector3(0, 1, 0);
            objectBuilders.Add(prefabContainerB);

            var largeShip = new MyMwcObjectBuilder_LargeShip(MyMwcObjectBuilder_LargeShip_TypesEnum.KAI);
            largeShip.PositionAndOrientation.Position = new Vector3(200, 0, 0);
            largeShip.PositionAndOrientation.Forward = Vector3.Forward;
            largeShip.PositionAndOrientation.Up = Vector3.Up;
            objectBuilders.Add(largeShip);

            List<MyMwcObjectBuilder_SmallDebris> debrisList = new List<MyMwcObjectBuilder_SmallDebris>();
            AddDebrisToList(debrisList, MyMwcObjectBuilder_SmallDebris_TypesEnum.Standard_Container_2, new Vector3(200, 0, 20));
            AddDebrisToList(debrisList, MyMwcObjectBuilder_SmallDebris_TypesEnum.Standard_Container_2, new Vector3(200, 130, 0));
            AddDebrisToList(debrisList, MyMwcObjectBuilder_SmallDebris_TypesEnum.Standard_Container_2, new Vector3(200, 160, 0));
            objectBuilders.AddRange(debrisList);
                  */
            //CreateFakeSmallDebris(null, MyMwcObjectBuilder_SmallDebris_TypesEnum.sat_01, new Vector3(500, 0, 0), false);
            //CreateFakeSmallDebris(null, MyMwcObjectBuilder_SmallDebris_TypesEnum.sat_01, new Vector3(-500, 0, 0), false);

            //CreateFakeSmallDebris(null, MyMwcObjectBuilder_SmallDebris_TypesEnum.Standard_Container_2, new Vector3(200, 0, 20), false);
            //CreateFakeSmallDebris(null, MyMwcObjectBuilder_SmallDebris_TypesEnum.Standard_Container_2, new Vector3(200, 130, 0), false);
            //CreateFakeSmallDebris(null, MyMwcObjectBuilder_SmallDebris_TypesEnum.Standard_Container_2, new Vector3(200, 160, 0), false);
        }

        private static void AddDebrisToList(List<MyMwcObjectBuilder_SmallDebris> list, MyMwcObjectBuilder_SmallDebris_TypesEnum debrisType, Vector3 position)
        {
            var debris = new MyMwcObjectBuilder_SmallDebris(debrisType, false, 10000);
            debris.PositionAndOrientation.Forward = Vector3.Forward;
            debris.PositionAndOrientation.Up = Vector3.Up;
            debris.PositionAndOrientation.Position = position;
            list.Add(debris);
        }

        private void CreateWorldFromDataReceivedFromServer_LoadCheckpoint()
        {
            MyMwcLog.WriteLine("CreateWorldFromDataReceivedFromServer_LoadCheckpoint - START");

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("CreateWorldFromDataReceivedFromServer_LoadCheckpoint");

            MyEditor.Static.Init(m_checkpoint);

            MySession.Static.GameDateTime = m_checkpoint.GameTime;

            MyMwcLog.WriteLine("MySession.Static.EventLog.Init");
            MySession.Static.EventLog.Init(m_checkpoint);

            if (m_checkpoint.InventoryObjectBuilder == null)
            {
                m_checkpoint.InventoryObjectBuilder = new MyMwcObjectBuilder_Inventory(new List<MyMwcObjectBuilder_InventoryItem>(), 200);                
            }
            m_checkpoint.InventoryObjectBuilder.UnlimitedCapacity = true;

            // Add razer claw for people with contributions > 30 USD
            if (GetSessionType().HasValue && GetSessionType().Value == MyMwcStartSessionRequestTypeEnum.NEW_STORY &&  MyClientServer.LoggedPlayer.AdditionalInfo.Contributions >= 30)
            {
                m_checkpoint.InventoryObjectBuilder.InsertBonusShipRazerClaw();
            }

            MyMwcLog.WriteLine("MySession.Static.Inventory");
            if (MySession.Static.CanSaveAndLoadSessionInventory)
            {
                MySession.Static.Inventory.Init(m_checkpoint.InventoryObjectBuilder);
            }
            else 
            {
                MySession.Static.Inventory.ClearInventoryItems(true);
            }

            if (m_checkpoint.PlayerObjectBuilder == null)
            {
                m_checkpoint.PlayerObjectBuilder = new MyMwcObjectBuilder_Player(100f, 10000f, 0f, null, null, null);
            }

            if (m_checkpoint.PlayerObjectBuilder.ShipObjectBuilder == null)
            {
                MyMwcObjectBuilder_SmallShip_Player playerShipObjectBuilder = CreatePlayerShip();
                playerShipObjectBuilder.PositionAndOrientation.Position = Vector3.Zero;
                playerShipObjectBuilder.PositionAndOrientation.Up = Vector3.Up;
                playerShipObjectBuilder.PositionAndOrientation.Forward = Vector3.Forward;
                m_checkpoint.PlayerObjectBuilder.ShipObjectBuilder = playerShipObjectBuilder;
            }

            // Invalid faction, for compatibility reason
            if (m_checkpoint.PlayerObjectBuilder.ShipObjectBuilder.Faction == MyMwcObjectBuilder_FactionEnum.None || m_checkpoint.PlayerObjectBuilder.ShipObjectBuilder.Faction == 0)
            {
                m_checkpoint.PlayerObjectBuilder.ShipObjectBuilder.Faction = MyMwcObjectBuilder_FactionEnum.Euroamerican;
            }

            MyMwcLog.WriteLine("MySession.Static.Player.Init");
            MySession.Static.Player.Init(m_checkpoint.PlayerObjectBuilder);

            if (MyFakes.ADD_DRONES_TO_INVENTORY)
            {
                var droneObjectBuilder = (MyMwcObjectBuilder_Drone)MyMwcObjectBuilder_Base.CreateNewObject(MyMwcObjectBuilderTypeEnum.Drone, (int)MyMwcObjectBuilder_Drone_TypesEnum.DroneUS);
                droneObjectBuilder.Faction = MyMwcObjectBuilder_FactionEnum.None;

                MySession.PlayerShip.Inventory.AddInventoryItem(droneObjectBuilder, 1, true);

                droneObjectBuilder = (MyMwcObjectBuilder_Drone)MyMwcObjectBuilder_Base.CreateNewObject(MyMwcObjectBuilderTypeEnum.Drone, (int)MyMwcObjectBuilder_Drone_TypesEnum.DroneCN);
                droneObjectBuilder.Faction = MyMwcObjectBuilder_FactionEnum.None;

                MySession.PlayerShip.Inventory.AddInventoryItem(droneObjectBuilder, 1, true);

                droneObjectBuilder = (MyMwcObjectBuilder_Drone)MyMwcObjectBuilder_Base.CreateNewObject(MyMwcObjectBuilderTypeEnum.Drone, (int)MyMwcObjectBuilder_Drone_TypesEnum.DroneSS);
                droneObjectBuilder.Faction = MyMwcObjectBuilder_FactionEnum.None;

                MySession.PlayerShip.Inventory.AddInventoryItem(droneObjectBuilder, 1, true);
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            MyMwcLog.WriteLine("CreateWorldFromDataReceivedFromServer_LoadCheckpoint - END");
        }

        //  Create world using data received from server
        public void CreateWorldFromDataReceivedFromServer()
        {
            MyMwcLog.WriteLine("CreateWorldFromDataReceivedFromServer - START");

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("CreateWorldFromDataReceivedFromServer");

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MySession.BeforeLoad");
            MySession.Static.BeforeLoad(m_checkpoint);
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            CreateWorldFromDataReceivedFromServer_LoadCheckpoint();
            
            bool canUseGenerator = m_sessionType.Value != MyMwcStartSessionRequestTypeEnum.EDITOR_SANDBOX && m_sessionType.Value != MyMwcStartSessionRequestTypeEnum.SANDBOX_OWN && MyFakes.ENABLE_GENERATED_ASTEROIDS;
            if (m_checkpoint.SectorObjectBuilder.FromGenerator && canUseGenerator) // When no sector on server, use procedural generator
            {
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("ProceduralGenerator.Player");
                if (m_checkpoint.PlayerObjectBuilder == null)
                {
                    MyMwcObjectBuilder_SmallShip_Player playerShipObjectBuilder = CreatePlayerShip();
                    playerShipObjectBuilder.PositionAndOrientation.Position = Vector3.Zero;
                    playerShipObjectBuilder.PositionAndOrientation.Up = Vector3.Up;
                    playerShipObjectBuilder.PositionAndOrientation.Forward = Vector3.Forward;
                    m_checkpoint.PlayerObjectBuilder = new MyMwcObjectBuilder_Player(100f, 10000f, 0f, null, playerShipObjectBuilder, null);
                }
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
            }

            UpdateSmallshipsOfferedByVendors(m_checkpoint);

            if (((m_checkpoint.SectorObjectBuilder.FromGenerator && MyFakes.ENABLE_GENERATED_ASTEROIDS) || canUseGenerator)) // When no sector on server, use procedural generator
            {
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Generating sector");

                //Use objects from server and generate new items into them
                //m_checkpoint.SectorObjectBuilder.SectorObjects = new List<MyMwcObjectBuilder_Base>();

                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Safe areas");

                if (m_checkpoint.SectorObjectBuilder.SectorObjects == null)
                {
                    m_checkpoint.SectorObjectBuilder.SectorObjects = new List<MyMwcObjectBuilder_Base>();
                }

                MyMwcLog.WriteLine("Adding safe areas");

                List<BoundingSphere> safeAreas = new List<BoundingSphere>();
                foreach (MyMwcObjectBuilder_Base objectBuilder in m_checkpoint.SectorObjectBuilder.SectorObjects)
                {
                    MyMwcObjectBuilder_DummyPoint dummyObjectBuilder = objectBuilder as MyMwcObjectBuilder_DummyPoint;
                    if (dummyObjectBuilder != null && dummyObjectBuilder.DummyFlags.HasFlag(MyDummyPointFlags.SAFE_AREA))
                    {
                        safeAreas.Add(new BoundingSphere(dummyObjectBuilder.PositionAndOrientation.Position, dummyObjectBuilder.Size.Length() / 2.0f));
                    }

                    MyMwcObjectBuilder_VoxelMap voxelObjectBuilder = objectBuilder as MyMwcObjectBuilder_VoxelMap;
                    if (voxelObjectBuilder != null)
                    {
                        MyMwcVector3Int sizeInVoxels = MyVoxelFiles.Get(voxelObjectBuilder.VoxelFile).SizeInVoxels;
                        Vector3 sizeInMeters = new Vector3(MyVoxelConstants.VOXEL_SIZE_IN_METRES * sizeInVoxels.X, MyVoxelConstants.VOXEL_SIZE_IN_METRES * sizeInVoxels.Y, MyVoxelConstants.VOXEL_SIZE_IN_METRES * sizeInVoxels.Z);
                        safeAreas.Add(new BoundingSphere(voxelObjectBuilder.PositionAndOrientation.Position, sizeInMeters.Length() / 2.0f));
                    }
                }
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

                MyMwcLog.WriteLine("GenerateSector");
                MySolarSystemMapSectorData sectorData = GenerateSector(m_sectorIdentifier.Position, m_checkpoint.SectorObjectBuilder.SectorObjects, safeAreas, m_checkpoint.SectorObjectBuilder.SectorObjects.Count > 0);

                //MySolarSystemMapSectorData sectorData = GenerateSectorData();
                m_checkpoint.SectorObjectBuilder.AreaTemplate = sectorData.Area;
                m_checkpoint.SectorObjectBuilder.AreaMultiplier = sectorData.AreaInfluenceMultiplier;

                MySector.UseGenerator = true;

                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
            }
            else if (!m_checkpoint.SectorObjectBuilder.AreaTemplate.HasValue)
            {
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Generating sector area data");
                MySolarSystemMapSectorData sectorData = GenerateSectorData();
                m_checkpoint.SectorObjectBuilder.AreaTemplate = sectorData.Area;
                m_checkpoint.SectorObjectBuilder.AreaMultiplier = sectorData.AreaInfluenceMultiplier;
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
            }

            MyMwcLog.WriteLine("CreateFromSectorObjectBuilder");
            CreateFromSectorObjectBuilder(m_checkpoint.SectorObjectBuilder);


            MyTrailerLoad.TrailerAnimation = null;

            //  If null, then loop above didn't contain player's ship
            MyCommonDebugUtils.AssertRelease(MySession.PlayerShip != null);

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Preload cockpit models");

            MySmallShip playerShip = MySession.PlayerShip;
            //  Pre-load player's cockpit interior models
            //  IMPORTANT: Only data and no vertex buffers, because this method is called from Update and not from Draw
            MyModels.GetModelOnlyData(MySession.PlayerShip.CockpitInteriorModelEnum);
            MyModels.GetModelOnlyData(MySession.PlayerShip.CockpitGlassModelEnum);
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            //0002325: AI - Add friend bot to story and sandbox, he will be there by default.
            if (!IsEditorActive() && MyFakes.SPAWN_FRIENDS == true)
            {
                MySession.PlayerFriends.Clear();// when saving bots will be enabled please remove this
                if (MySession.PlayerFriends.Count == 0)
                {
                    MySmallShipBot bot = CreateFakeBot("Friend", MyMwcObjectBuilder_SmallShip_TypesEnum.GREISER, MySession.PlayerShip.WorldMatrix.Translation + new Vector3(0, 30, 30), 1f, MyMwcObjectBuilder_FactionEnum.Euroamerican);
                    MySession.PlayerFriends.Add(bot);
                    bot.Follow(MySession.PlayerShip);
                    bot.Save = false;

                    bot = CreateFakeBot("Friend", MyMwcObjectBuilder_SmallShip_TypesEnum.GREISER, MySession.PlayerShip.WorldMatrix.Translation + new Vector3(0, -30, 30), 1f, MyMwcObjectBuilder_FactionEnum.Euroamerican);
                    MySession.PlayerFriends.Add(bot);
                    bot.Follow(MySession.PlayerShip);
                    bot.Save = false;
                }
            }

            MyMwcLog.WriteLine("MySession.Static.AfterLoad");
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("AfterLoad");
            MySession.Static.AfterLoad(m_checkpoint);
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();


            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Side missions");
            // Side missions
            foreach (var dummy in MyEntities.GetEntities().OfType<MyDummyPoint>())
            {
                if ((dummy.DummyFlags & MyDummyPointFlags.SIDE_MISSION) != 0)
                {
                    MyMwcLog.WriteLine("Side missions");
                    // TODO: Choose random submission
                    var sideMission = new MySideMissionAssassination(new MyMissionBase.MyMissionLocation(m_checkpoint.CurrentSector.Position, dummy.EntityId.Value.NumericValue));
                    if (MyMissions.GetMissionByID(sideMission.ID) == null)
                    {
                        MyMissions.AddMission(sideMission);
                    }
                }
            }
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            MyMwcLog.WriteLine("MySession.Static.Player.TravelEnter");
            //Unpacks player friends needed for mission script
            MySession.Static.Player.TravelEnter();

            if (MySession.Is25DSector)
                MyGlobalEvents.DisableAllGlobalEvents();
            else
                MyGlobalEvents.EnableAllGlobalEvents();

            // Init active mission
            System.Diagnostics.Debug.Assert(MyMissions.ActiveMission == null);

            MyMwcLog.WriteLine("Active mission load - START");
            if (GetGameType() == MyGuiScreenGamePlayType.GAME_STORY && (MyMultiplayerGameplay.Static == null || MyMultiplayerGameplay.Static.IsHost))
            {
                MyMwcLog.WriteLine("Starting story mission");

                if (m_checkpoint.ActiveMissionID != -1)
                {
                    MyMwcLog.WriteLine("ActiveMissionID: " + m_checkpoint.ActiveMissionID);

                    MyMission mission = MyMissions.GetMissionByID((MyMissionID)m_checkpoint.ActiveMissionID) as MyMission;
                    MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Load active mission");
                    if (mission.IsMainSector)
                    {
                        mission.InsertRequiredActors();
                    }
                    mission.Load();
                    MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
                }
                else
                {
                    MyMwcLog.WriteLine("ActiveMissionID: none");

                    if (m_missionToStart.HasValue)
                    {
                        MyMission mission = MyMissions.GetMissionByID(m_missionToStart.Value) as MyMission;
                        MyScriptWrapper.DebugSetFactions(mission);
                        mission.Accept();

                        m_missionToStart = null;
                    }
                }
                MyMissions.RefreshAvailableMissions();

                if (m_travelReason == MyMwcTravelTypeEnum.SOLAR)
                {
                    if (MyMissions.ActiveMission == null || MyMissions.ActiveMission.MovePlayerToMadelynHangar)
                    {
                        if (MyMissionBase.IsPlayerShipNearMadelyn())
                        {
                            if (MyMissions.ActiveMission != null)
                                MyMissionBase.MovePlayerAndFriendsToHangar(MyMissions.ActiveMission.RequiredActors);
                            else
                                MyMissionBase.MovePlayerAndFriendsToHangar(new MyActorEnum[] { MyActorEnum.MARCUS, MyActorEnum.TARJA, MyActorEnum.VALENTIN });
                        }
                    }
                }


            }
            else if (GetGameType() == MyGuiScreenGamePlayType.GAME_SANDBOX)
            {
                MyMwcLog.WriteLine("Starting sandbox mission");
                MyMissions.StartSandboxMission(m_checkpoint.CurrentSector.Position);
            }
            MyMwcLog.WriteLine("Active mission load - END");

            MyEntities.UpdateAfterSimulation(); //Updates AABBs of objects

            if (MyMultiplayerGameplay.IsStory())
            {
                MyMultiplayerGameplay.MakeInventoryItemsTradeable(MySession.PlayerShip);
            }


            if (MyFakes.MWBUILDER)
            {
                MyVoxelMap voxelMap = null;

                if (MyVoxelMaps.GetVoxelMapsCount() == 0)
                {
                   // MyMwcObjectBuilder_VoxelMap voxelMapOb = new MyMwcObjectBuilder_VoxelMap(Vector3.Zero, MyMwcVoxelFilesEnum.Cube_512x512x512, MyMwcVoxelMaterialsEnum.Stone_01);
                   // voxelMap = (MyVoxelMap)MyEntities.CreateFromObjectBuilderAndAdd(null, voxelMapOb, Matrix.Identity);
                    voxelMap = new MyVoxelMap();
                    voxelMap.Init(Vector3.Zero, new MyMwcVector3Int(1024, 1024, 1024), MyMwcVoxelMaterialsEnum.Stone_01);

                    //MyVoxelImport.Fill(voxelMap);
                    MyVoxelImport.FillEmpty(voxelMap);

                    MyEntities.Add(voxelMap);

                    voxelMap.SetPosition(Vector3.Zero);
                 

                    MyVoxelGenerator.CreateSphere(voxelMap, new BoundingSphere(Vector3.Zero, 1024));
                    voxelMap.InvalidateCache(new MyMwcVector3Int(0, 0, 0), new MyMwcVector3Int(1024, 1024, 1024));
                    

                    MyVoxelMaps.RecalcVoxelMaps();

                    voxelMap.UpdateAABBHr();


                    voxelMap.SaveVoxelContents(Path.Combine(MyMinerGame.Static.RootDirectory, "VoxelMaps", MyVoxelFiles.ExportFile + ".vox"));

                    //MyVoxelGenerator.CutOutBoxRelative(voxelMap, new Vector3(0, 0.5f, 0), new Vector3(1, 1, 1));
                    /*
  MyVoxelGenerator.ChangeMaterialInBoxRelative(voxelMap, new Vector3(0, 0.49f, 0), new Vector3(1, 0.5f, 1), MyMwcVoxelMaterialsEnum.Uranite_01);
  MyVoxelGenerator.ChangeMaterialInBoxRelative(voxelMap, new Vector3(0, 0.2f, 0), new Vector3(1, 0.49f, 1), MyMwcVoxelMaterialsEnum.Nickel_01);
  MyVoxelGenerator.ChangeMaterialInBoxRelative(voxelMap, new Vector3(0, 0.0f, 0), new Vector3(1, 0.1f, 1), MyMwcVoxelMaterialsEnum.Lava_01);
                     */
                }
                else
                {
                    voxelMap = MyVoxelMaps.GetVoxelMaps()[0];

                    MyPhysics.physicsSystem.GravitationPoints.Clear();
                    MyPhysics.physicsSystem.GravitationPoints.Add(new Tuple<BoundingSphere,float>(new BoundingSphere(voxelMap.WorldAABB.GetCenter(), voxelMap.WorldAABB.Size().Length()), 2000));

                    
              //      voxelMap.Init(Vector3.Zero, new MyMwcVector3Int(1024, 1024, 1024), MyMwcVoxelMaterialsEnum.Stone_01);
              //      MyVoxelImport.Fill(voxelMap);
              //      MyVoxelGenerator.CutOutBoxRelative(voxelMap, new Vector3(0, 0.3f, 0), new Vector3(1, 1, 1));

              //      string name = Path.Combine(MyFileSystemUtils.GetApplicationUserDataFolder(), "VoxelMap_0_mikrogen.vox");
              //      voxelMap.SaveVoxelContents(name);
                      
              //      voxelMap.SetName("VoxelMap_0_mikrogen.vox");
              //      voxelMap.Init(null, Vector3.Zero, null);
                }
                    
                MySpectator.Position = voxelMap.WorldAABB.GetCenter();

                MyCamera.SetViewMatrix(MySpectator.GetViewMatrix());
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            MyMwcLog.WriteLine("CreateWorldFromDataReceivedFromServer - END");
        }

        /// <summary>
        /// Generates area data, sun color, dust color, etc.
        /// </summary>
        /// <returns></returns>
        private MySolarSystemMapSectorData GenerateSectorData()
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("GenerateSectorData");

            MySolarSystemGenerator solarSystemGenerator = new MySolarSystemGenerator(MyGuiScreenSolarSystemMap.UNIVERSE_SEED);
            solarSystemGenerator.Generate(1);

            MySectorGenerator sectorGenerator = new MySectorGenerator(MyGuiScreenSolarSystemMap.UNIVERSE_SEED);

            MyMwcVector3Int sectorToGenerate = m_sectorIdentifier.Position;
            MySolarSystemMapSectorData sectorData = sectorGenerator.GenerateSectorEntities(solarSystemGenerator.SolarSystemData, sectorToGenerate, 0, int.MaxValue, true);

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
            return sectorData;
        }

        public MySolarSystemMapSectorData GenerateSector(MyMwcVector3Int sectorToGenerate, List<MyMwcObjectBuilder_Base> objectBuilders, List<BoundingSphere> safeAreas, bool onlyStaticAsteroids)
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Solar generator");

            MySolarSystemGenerator solarSystemGenerator = new MySolarSystemGenerator(MyGuiScreenSolarSystemMap.UNIVERSE_SEED);
            solarSystemGenerator.Generate(1024);
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            BoundingSphere playerSafeArea = new BoundingSphere(MySession.PlayerShip.GetPosition(), 150); //smallship + mothership
            safeAreas.Add(playerSafeArea);

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Sector generator");
            MySectorGenerator sectorGenerator = new MySectorGenerator(MyGuiScreenSolarSystemMap.UNIVERSE_SEED ^ sectorToGenerate.GetHashCode(), safeAreas);
            MySolarSystemMapSectorData sectorData = sectorGenerator.GenerateSectorObjectBuilders(sectorToGenerate, solarSystemGenerator.SolarSystemData, objectBuilders, onlyStaticAsteroids);
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            MySector.UseGenerator = true;

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Sector data");
            MySolarSectorData defaultSectorData = MySolarSystemConstants.GetDefaultArea().SectorData;
            MySolarSectorData currentSectorData = defaultSectorData;
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            if (sectorData.Area.HasValue)
            {
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Sector data interpolation");
                var sectorData2 = MySolarSystemConstants.Areas[sectorData.Area.Value].SectorData;
                currentSectorData = sectorData2.InterpolateWith(defaultSectorData, 1 - sectorData.AreaInfluenceMultiplier);
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyMwcObjectBuilder_Sector");

            SetSectorProperties(currentSectorData);

            MyBackgroundCube.ReloadContent();
            MyDistantImpostors.ReloadContent();
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            return sectorData;
        }

        private static void UpdateBuildersForVoxelImport(MyMwcObjectBuilder_Sector sectorBuilder)
        {
            sectorBuilder.SectorObjects.RemoveAll(x => x.GetObjectBuilderType() != MyMwcObjectBuilderTypeEnum.SmallShip_Player && x.GetObjectBuilderType() != MyMwcObjectBuilderTypeEnum.Sector);
            sectorBuilder.SectorObjects.Add(new MyMwcObjectBuilder_VoxelMap(new Vector3(-160, -100, 3000),
                MyMwcVoxelFilesEnum.VoxelImporterTest, MyMwcVoxelMaterialsEnum.Cobalt_01));
        }


        //this method creates world objects from object builders received from server
        public static void CreateFromObjectBuilders(List<MyMwcObjectBuilder_Base> objectBuilders)
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("CreateFromObjectBuilders");

            MyVoxelMaps.AutoRecalculateVoxelMaps = false;

            //  Objects received from server
            foreach (MyMwcObjectBuilder_Base objectBuilder in objectBuilders)
            {

                if (objectBuilder.GetObjectBuilderType() == MyMwcObjectBuilderTypeEnum.VoxelMap)
                {
                    MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("CreateFromObjectBuilderAndAdd Voxel");
                    MyMwcObjectBuilder_VoxelMap voxelMapObjectBuilder = objectBuilder as MyMwcObjectBuilder_VoxelMap;
                    MyEntities.CreateFromObjectBuilderAndAdd(null, voxelMapObjectBuilder, Matrix.Identity);
                    MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
                }
                else if (objectBuilder.GetObjectBuilderType() == MyMwcObjectBuilderTypeEnum.VoxelMap_Neighbour)
                {
                    //  Voxel map neighbours are handled in its static classe, so ignore it here
                }
                else
                {
                    if (objectBuilder is MyMwcObjectBuilder_Object3dBase || objectBuilder is MyMwcObjectBuilder_InfluenceSphere)
                    {
                        MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("CreateFromObjectBuilderAndAdd 3D object");

                        MyEntity temporaryEntity = null;
                        Matrix matrix = Matrix.Identity;

                        if (objectBuilder is MyMwcObjectBuilder_Object3dBase)
                        {
                            var object3d = objectBuilder as MyMwcObjectBuilder_Object3dBase;

                            matrix = Matrix.CreateWorld(object3d.PositionAndOrientation.Position, object3d.PositionAndOrientation.Forward, object3d.PositionAndOrientation.Up);
                            MyUtils.AssertIsValid(matrix);
                        }

                        temporaryEntity = MyEntities.CreateFromObjectBuilderAndAdd(null, objectBuilder, matrix);

                        MyEntities.TestEntityAfterInsertionForCollision(temporaryEntity);

                        MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
                    }
                }
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        //this method creates world objects from object builders received from server
        public static void CreateFromSectorObjectBuilder(MyMwcObjectBuilder_Sector sectorBuilder)
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("CreateFromSectorObjectBuilder");

            if (MyFakes.VOXEL_IMPORT)
            {
                UpdateBuildersForVoxelImport(sectorBuilder);
            }

            CreateFromObjectBuilders(sectorBuilder.SectorObjects);

            MySolarSectorData defaultSectorData = MySolarSystemConstants.GetDefaultArea().SectorData;
            MySolarSectorData currentSectorData = defaultSectorData;

            if (sectorBuilder.AreaTemplate.HasValue)
            {
                var sectorData = MySolarSystemConstants.Areas[sectorBuilder.AreaTemplate.Value].SectorData;
                currentSectorData = sectorData.InterpolateWith(defaultSectorData, 1 - sectorBuilder.AreaMultiplier);
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyMwcObjectBuilder_Sector");

            SetSectorProperties(currentSectorData);

            MyBackgroundCube.ReloadContent();
            MyDistantImpostors.ReloadContent();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            MyVoxelMaps.AutoRecalculateVoxelMaps = true;

#if PROFILING
            Console.WriteLine("Total voxel load: {0}", MyPerformanceTimer.VoxelLoad.GetTotalTimeSpent());
            Console.WriteLine("Total voxel content merge: {0}", MyPerformanceTimer.VoxelContentMerge.GetTotalTimeSpent());
            Console.WriteLine("Total voxel material merge: {0}", MyPerformanceTimer.VoxelMaterialMerge.GetTotalTimeSpent());
            Console.WriteLine("Total voxel hand: {0}", MyPerformanceTimer.VoxelHandLoad.GetTotalTimeSpent());
#endif //PROFILING

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        static void SetSectorProperties(MySolarSectorData currentSectorData)
        {
            MySector.DebrisProperties = currentSectorData.DebrisProperties;
            MySector.FogProperties = currentSectorData.FogProperties;
            MySector.ImpostorProperties = currentSectorData.ImpostorProperties;
            MySector.SunProperties = currentSectorData.SunProperties;
            MySector.ParticleDustProperties = currentSectorData.ParticleDustProperties;
            MySector.GodRaysProperties = currentSectorData.GodRaysProperties;
            MySector.BackgroundTexture = currentSectorData.BackgroundTexture;
            MySector.PrimaryMaterials = currentSectorData.PrimaryAsteroidMaterials.Keys.Select(e=>(int)e).ToList();
            MySector.SecondaryMaterials = currentSectorData.SecondaryAsteroidMaterials.Keys.Select(e => (int)e).ToList();
            MySector.AllowedMaterials = currentSectorData.AllowedAsteroidMaterials.Select(e => (int)e).ToList();
        }

    
        //  Trailer animation fake world - world for fight animation
        void CreateFakeWorld1()
        {
            Vector3 miningShipsOrigin = new Vector3(0, 500, 500);
            MySession.PlayerShip = CreateFakeMinerShip(null, MyMwcObjectBuilder_SmallShip_TypesEnum.GETTYSBURG, miningShipsOrigin, true, 1.0f);

            MyEntities.CreateFromObjectBuilderAndAdd("Kai", new MyMwcObjectBuilder_LargeShip(MyMwcObjectBuilder_LargeShip_TypesEnum.KAI), Matrix.CreateWorld(new Vector3(-1000, -500, -500), Vector3.Forward, Vector3.Up));

            SectorDustColor = new Vector4(1, 1, 1, 0.5f);
            MySector.SunProperties.SunDiffuse = new Vector3(1, 1, 1);

            LoadTrailerContent();

            MyMwcObjectBuilder_VoxelMap objectBuilder = MyMwcObjectBuilder_Base.CreateNewObject(MyMwcObjectBuilderTypeEnum.VoxelMap, (int?)null) as MyMwcObjectBuilder_VoxelMap;
            objectBuilder.VoxelFile = MyMwcVoxelFilesEnum.PerfectSphereWithMassiveTunnels_512x512x512;
            objectBuilder.VoxelMaterial = MyMwcVoxelMaterialsEnum.Stone_06;
            //MyVoxelMap newVoxelMap0 = (MyVoxelMap)MyEntities.CreateFromObjectBuilderAndAdd(null, objectBuilder, Matrix.CreateTranslation(3900, 960, 1920));
            objectBuilder.PositionAndOrientation.Position = new Vector3(2000, 0, 0);
            MyVoxelMap newVoxelMap0 = (MyVoxelMap)MyEntities.CreateFromObjectBuilderAndAdd(null, objectBuilder, Matrix.Identity);

            //newVoxelMap0.MergeVoxelContents(MyMwcVoxelFilesEnum.SphereWithLargeCutOut_128x128x128, new MyMwcVector3Short(0, 0, 0), MyMwcVoxelMapMergeTypeEnum.INVERSE_AND_SUBTRACT);
            //newVoxelMap0.MergeVoxelMaterials(MyMwcVoxelFilesEnum.SphereWithLargeCutOut_128x128x128, new MyMwcVector3Short(128, 64, 0), MyMwcVoxelMaterialsEnum.Gold_01);
            //newVoxelMap0.MergeVoxelMaterials(MyMwcVoxelFilesEnum.PerfectSphereWithMassiveTunnels_512x512x512, new MyMwcVector3Short(-64, 0, 0), MyMwcVoxelMaterialsEnum.Stone_01);
            newVoxelMap0.WriteDebugInfo();

            objectBuilder.VoxelFile = MyMwcVoxelFilesEnum.TorusWithSmallTunnel_256x128x256;
            objectBuilder.VoxelMaterial = MyMwcVoxelMaterialsEnum.Cobalt_01;
            //MyVoxelMap newVoxelMap1 = (MyVoxelMap)MyEntities.CreateFromObjectBuilderAndAdd(null, objectBuilder, Matrix.CreateTranslation(-1000, -100, -8000));
            objectBuilder.PositionAndOrientation.Position = new Vector3(-1000, -100, -8000);
            MyVoxelMap newVoxelMap1 = (MyVoxelMap)MyEntities.CreateFromObjectBuilderAndAdd(null, objectBuilder, Matrix.Identity);

            //newVoxelMap1.MergeVoxelMaterials(MyMwcVoxelFilesEnum.SphereWithLargeCutOut_128x128x128, new MyMwcVector3Short(64, 0, 0), MyMwcVoxelMaterialsEnum.Platinum_01);
            //newVoxelMap1.MergeVoxelMaterials(MyMwcVoxelFilesEnum.SphereWithLargeCutOut_128x128x128, new MyMwcVector3Short(64, 64, 64), MyMwcVoxelMaterialsEnum.Silicon_01);
            newVoxelMap1.WriteDebugInfo();

            objectBuilder.VoxelFile = MyMwcVoxelFilesEnum.VerticalIsland_128x256x128;
            objectBuilder.VoxelMaterial = MyMwcVoxelMaterialsEnum.Iron_02;
            //MyVoxelMap newVoxelMap2 = (MyVoxelMap)MyEntities.CreateFromObjectBuilderAndAdd(null, objectBuilder, Matrix.CreateTranslation(-5000, -500, 0));
            objectBuilder.PositionAndOrientation.Position = new Vector3(-5000, -500, 0);
            MyVoxelMap newVoxelMap2 = (MyVoxelMap)MyEntities.CreateFromObjectBuilderAndAdd(null, objectBuilder, Matrix.Identity);
            newVoxelMap2.WriteDebugInfo();
        }

        //  Trailer animation fake world - world for ice fight animation
        void CreateFakeWorld2()
        {
            Vector3 miningShipsOrigin = new Vector3(0, 500, 500);

            MySession.PlayerShip = CreateFakeMinerShip(null, MyMwcObjectBuilder_SmallShip_TypesEnum.GETTYSBURG, miningShipsOrigin + new Vector3(7175, 486, 5540), true, 1.0f);

            miningShipsOrigin.X -= 1000;

            SectorDustColor = new Vector4(1, 1, 1, 0.5f);
            MySector.SunProperties.SunDiffuse = new Vector3(1, 1, 1);

            LoadTrailerContent();

            MyMwcObjectBuilder_VoxelMap objectBuilder = MyMwcObjectBuilder_Base.CreateNewObject(MyMwcObjectBuilderTypeEnum.VoxelMap, (int?)null) as MyMwcObjectBuilder_VoxelMap;
            objectBuilder.VoxelFile = MyMwcVoxelFilesEnum.PerfectSphereWithMassiveTunnels2_512x512x512;
            objectBuilder.VoxelMaterial = MyMwcVoxelMaterialsEnum.Organic_01;
            //MyVoxelMap newVoxelMap0 = (MyVoxelMap)MyEntities.CreateFromObjectBuilderAndAdd(null, objectBuilder, Matrix.CreateTranslation(3900, 960, 1920));
            objectBuilder.PositionAndOrientation.Position = new Vector3(2000, 0, 0);
            MyVoxelMap newVoxelMap0 = (MyVoxelMap)MyEntities.CreateFromObjectBuilderAndAdd(null, objectBuilder, Matrix.Identity);

            //newVoxelMap0.MergeVoxelMaterials(MyMwcVoxelFilesEnum.PerfectSphereWithMassiveTunnels2_512x512x512, new MyMwcVector3Short(128, 128, 128), MyMwcVoxelMaterialsEnum.Helium3_01);
            //newVoxelMap0.MergeVoxelMaterials(MyMwcVoxelFilesEnum.PerfectSphereWithMassiveTunnels2_512x512x512, new MyMwcVector3Short(64, 64, 64), MyMwcVoxelMaterialsEnum.Ice_01);
            newVoxelMap0.WriteDebugInfo();
        }

        //  Trailer animation fake world - world for race in the tunnels animation
        void CreateFakeWorld3()
        {
            Vector3 miningShipsOrigin = new Vector3(0, 500, 500);

            MySession.PlayerShip = CreateFakeMinerShip(null, MyMwcObjectBuilder_SmallShip_TypesEnum.GETTYSBURG, miningShipsOrigin + new Vector3(3057, 629, 4537), true, 1.0f);

            miningShipsOrigin.X -= 1000;

            CreateFakeMinerShip(null, MyMwcObjectBuilder_SmallShip_TypesEnum.GETTYSBURG, miningShipsOrigin + new Vector3(3000, 629, 4537), false, 1.0f);

            MyEntities.CreateFromObjectBuilderAndAdd("Kai", new MyMwcObjectBuilder_LargeShip(MyMwcObjectBuilder_LargeShip_TypesEnum.KAI), Matrix.CreateWorld(new Vector3(3000, 500, 5000), Vector3.Forward, Vector3.Up));

            MyEntities.CreateFromObjectBuilderAndAdd("Kai", new MyMwcObjectBuilder_LargeShip(MyMwcObjectBuilder_LargeShip_TypesEnum.KAI), Matrix.CreateWorld(new Vector3(9800, 5393, 3922), Vector3.Forward, Vector3.Up));

            SectorDustColor = new Vector4(1, 1, 1, 0.5f);
            MySector.SunProperties.SunDiffuse = new Vector3(1, 1, 1);

            LoadTrailerContent();

            MyMwcObjectBuilder_VoxelMap objectBuilder = MyMwcObjectBuilder_Base.CreateNewObject(MyMwcObjectBuilderTypeEnum.VoxelMap, (int?)null) as MyMwcObjectBuilder_VoxelMap;
            objectBuilder.VoxelFile = MyMwcVoxelFilesEnum.PerfectSphereWithRaceTunnel2_512x512x512;
            objectBuilder.VoxelMaterial = MyMwcVoxelMaterialsEnum.Stone_01;
            //MyVoxelMap newVoxelMap0 = (MyVoxelMap)MyEntities.CreateFromObjectBuilderAndAdd(null, objectBuilder, Matrix.CreateTranslation(3900, 960, 1920));
            objectBuilder.PositionAndOrientation.Position = new Vector3(2000, 0, 0);
            MyVoxelMap newVoxelMap0 = (MyVoxelMap)MyEntities.CreateFromObjectBuilderAndAdd(null, objectBuilder, Matrix.Identity);
            //newVoxelMap0.MergeVoxelMaterials(MyMwcVoxelFilesEnum.PerfectSphereWithRaceTunnel2_512x512x512, new MyMwcVector3Short(64, 64, 64), MyMwcVoxelMaterialsEnum.Gold_01);
            //newVoxelMap0.MergeVoxelMaterials(MyMwcVoxelFilesEnum.PerfectSphereWithRaceTunnel2_512x512x512, new MyMwcVector3Short(0, 128, 64), MyMwcVoxelMaterialsEnum.Stone_12_Craters_01);
            newVoxelMap0.WriteDebugInfo();
        }

        //  Trailer animation fake world - world for main menu animation
        void CreateFakeWorld4()
        {
            // Player ship. 
            Vector3 miningShipsOrigin = new Vector3(0, 500, 500);
            MySession.PlayerShip = CreateFakeMinerShip(null, MyMwcObjectBuilder_SmallShip_TypesEnum.GETTYSBURG, miningShipsOrigin + new Vector3(3769, 1859, 2930), true, 1.0f);

            SectorDustColor = new Vector4(1, 1, 1, 0.5f);
            MySector.SunProperties.SunDiffuse = new Vector3(1, 1, 1);

            LoadTrailerContent();

            MyMwcObjectBuilder_VoxelMap objectBuilder = MyMwcObjectBuilder_Base.CreateNewObject(MyMwcObjectBuilderTypeEnum.VoxelMap, null) as MyMwcObjectBuilder_VoxelMap;
            objectBuilder.VoxelFile = MyMwcVoxelFilesEnum.TorusWithManyTunnels_256x128x256;
            objectBuilder.VoxelMaterial = MyMwcVoxelMaterialsEnum.Ice_01;

            objectBuilder.PositionAndOrientation.Position = new Vector3(2000, 0, 0);
            MyVoxelMap newVoxelMap0 = (MyVoxelMap)MyEntities.CreateFromObjectBuilderAndAdd(null, objectBuilder, Matrix.Identity);
            //MyVoxelMap newVoxelMap0 = (MyVoxelMap)MyEntities.CreateFromObjectBuilderAndAdd(null, objectBuilder, Matrix.CreateTranslation(3900, 960, 1920));
            newVoxelMap0.WriteDebugInfo();
        }

        //  Trailer animation fake world - world for ship attack animation
        void CreateFakeWorld5()
        {
            Vector3 miningShipsOrigin = new Vector3(0, 500, 500);
            MySession.PlayerShip = CreateFakeMinerShip(null, MyMwcObjectBuilder_SmallShip_TypesEnum.GETTYSBURG, miningShipsOrigin + new Vector3(-2300, 486, 5515), true, 1.0f);

            miningShipsOrigin.X -= 1000;

            MyEntities.CreateFromObjectBuilderAndAdd("Kai", new MyMwcObjectBuilder_LargeShip(
                MyMwcObjectBuilder_LargeShip_TypesEnum.KAI/*, null, null*/), Matrix.CreateWorld(new Vector3(-300, 550, 0), Vector3.Forward, Vector3.Up));

            SectorDustColor = new Vector4(1, 1, 1, 0.5f);
            MySector.SunProperties.SunDiffuse = new Vector3(1, 1, 1);

            LoadTrailerContent();
        }

        static MyMwcVoxelFilesEnum GetRandomVoxelFileNameForSandboxRandom()
        {
            switch (MyMwcUtils.GetRandomInt(0, 13))
            {
                case 0:
                    return MyMwcVoxelFilesEnum.SphereWithLargeCutOut_128x128x128;
                case 1:
                    return MyMwcVoxelFilesEnum.TorusWithManyTunnels_256x128x256;
                case 2:
                    return MyMwcVoxelFilesEnum.TorusWithSmallTunnel_256x128x256;
                case 3:
                    return MyMwcVoxelFilesEnum.VerticalIsland_128x128x128;
                case 4:
                    return MyMwcVoxelFilesEnum.VerticalIsland_128x256x128;
                case 5:
                    return MyMwcVoxelFilesEnum.VerticalIslandStorySector_128x256x128;
                case 6:
                    return MyMwcVoxelFilesEnum.DeformedSphere1_64x64x64;
                case 7:
                    return MyMwcVoxelFilesEnum.DeformedSphere2_64x64x64;
                case 8:
                    return MyMwcVoxelFilesEnum.DeformedSphereWithCorridor_128x64x64;
                case 9:
                    return MyMwcVoxelFilesEnum.DeformedSphereWithCorridor_256x256x256;
                case 10:
                    return MyMwcVoxelFilesEnum.DeformedSphereWithCraters_128x128x128;
                case 11:
                    return MyMwcVoxelFilesEnum.ScratchedBoulder_128x128x128;
                case 12:
                    return MyMwcVoxelFilesEnum.DeformedSphereWithHoles_64x128x64;
                default:
                    return MyMwcVoxelFilesEnum.TorusStorySector_256x128x256;
            }
        }

        static MyMwcVoxelFilesEnum GetRandomSmallVoxelFileNameForSandboxRandom()
        {
            switch (MyMwcUtils.GetRandomInt(0, 6))
            {
                case 0:
                    return MyMwcVoxelFilesEnum.DeformedSphere1_64x64x64;
                case 1:
                    return MyMwcVoxelFilesEnum.DeformedSphere2_64x64x64;
                case 2:
                    return MyMwcVoxelFilesEnum.DeformedSphereWithCorridor_128x64x64;
                case 3:
                    return MyMwcVoxelFilesEnum.ScratchedBoulder_128x128x128;
                case 4:
                    return MyMwcVoxelFilesEnum.DeformedSphereWithCraters_128x128x128;
                case 5:
                    return MyMwcVoxelFilesEnum.DeformedSphereWithHoles_64x128x64;
                default:
                    return MyMwcVoxelFilesEnum.DeformedSphere1_64x64x64;
            }
        }

        static MyMwcObjectBuilder_LargeShip_TypesEnum GetRandomLargeShipType()
        {
            switch (MyMwcUtils.GetRandomInt(0, 4))
            {
                case 0:
                    return MyMwcObjectBuilder_LargeShip_TypesEnum.KAI;
                case 1:
                    return MyMwcObjectBuilder_LargeShip_TypesEnum.KAI;
                case 2:
                    return MyMwcObjectBuilder_LargeShip_TypesEnum.MOTHERSHIP_SAYA;
                case 3:
                    return MyMwcObjectBuilder_LargeShip_TypesEnum.MOTHERSHIP_SAYA;
                default:
                    return MyMwcObjectBuilder_LargeShip_TypesEnum.MOTHERSHIP_SAYA;
            }
        }

        void LoadTrailerContent()
        {
            //if (MyMwcFinalBuildConstants.ENABLE_TRAILER_SAVE)
            //{
            //    MyTrailerSave.LoadContent();
            //    MyTrailerSave.AttachPhysObject("PlayerShip", MySession.PlayerShip);
            //}

            MySector.SunProperties.SunIntensity = MySolarSystemConstants.SunIntensityFromPosition(MyBgrCubeConsts.EARTH_POSITION);
            MyTrailerLoad.LoadContent();
        }

        //  Creates debris field
        public MyLargeDebrisField CreateFakeLargeDebrisField(string hudLabelText, MyMwcObjectBuilder_LargeDebrisField_TypesEnum debrisType,
            Vector3 position)
        {
            MyLargeDebrisField debris = (MyLargeDebrisField)MyEntities.CreateFromObjectBuilderAndAdd(hudLabelText,
                new MyMwcObjectBuilder_LargeDebrisField(debrisType), Matrix.CreateWorld(position, Vector3.Forward, Vector3.Up));
            MyEntities.TestEntityAfterInsertionForCollision(debris);
            return debris;
        }

        //  Use only for quick test or trailer animations
        public MySmallDebris CreateFakeSmallDebris(string hudLabelText, MyMwcObjectBuilder_SmallDebris_TypesEnum debrisType,
            Vector3 position, bool immovable)
        {
            MySmallDebris debris = (MySmallDebris)MyEntities.CreateFromObjectBuilderAndAdd(hudLabelText,
                    new MyMwcObjectBuilder_SmallDebris(debrisType, immovable, 10000), Matrix.CreateWorld(position, Vector3.Forward, Vector3.Up));
            MyEntities.TestEntityAfterInsertionForCollision(debris);
            return debris;
        }

        public MyPrefabContainer CreateFakePrefabContainer(string hudLabelText, Vector3 position)
        {

            List<MyMwcObjectBuilder_PrefabBase> prefabBuilders = new List<MyMwcObjectBuilder_PrefabBase>();
            int i = 0;
            int x = 0;
            foreach (MyMwcObjectBuilderTypeEnum prefabType in MyGuiPrefabHelpers.MyMwcPrefabTypesEnumValues)
            {
                foreach (int prefabId in MyMwcObjectBuilder_Base.GetObjectBuilderIDs(prefabType))
                {
                    MyMwcObjectBuilder_PrefabBase prefabBuilder = MyPrefabFactory.GetInstance().CreatePrefabObjectBuilder(prefabType, prefabId, MyMwcObjectBuilder_Prefab_AppearanceEnum.None);

                    prefabBuilders.Add(prefabBuilder);
                    i = i + 1000;
                    x = x + 500;
                }
            }

            MyMwcObjectBuilder_PrefabContainer prefabContainerBuilder = new MyMwcObjectBuilder_PrefabContainer(
                null, MyMwcObjectBuilder_PrefabContainer_TypesEnum.INSTANCE, prefabBuilders, MyClientServer.LoggedPlayer.GetUserId(),
                MyMwcObjectBuilder_FactionEnum.Euroamerican, new MyMwcObjectBuilder_Inventory(new List<MyMwcObjectBuilder_InventoryItem>(), MyInventory.DEFAULT_MAX_ITEMS));
            MyPrefabContainer container = (MyPrefabContainer)MyEntities.CreateFromObjectBuilderAndAdd(hudLabelText, prefabContainerBuilder, Matrix.CreateWorld(position, Vector3.Forward, Vector3.Up));
            MyEntities.TestEntityAfterInsertionForCollision(container);
            return container;
        }

        //  Use only for quick test, random sandbox or trailer animations
        //  Never initialize as player ship! We can have only 1 player ship in the game
        public MySmallShip CreateFakeMinerShip(string hudLabelText, MyMwcObjectBuilder_SmallShip_TypesEnum shipType, Vector3 position, bool isPlayerShip, float health)
        {
            List<MyMwcObjectBuilder_SmallShip_Weapon> weapons = new List<MyMwcObjectBuilder_SmallShip_Weapon>();
            weapons.Add(new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Autocanon));
            weapons.Add(new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Autocanon));
            weapons.Add(new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Missile_Launcher));
            //weapons.Add(new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Missile_Launcher));
            weapons.Add(new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Harvesting_Device));
            weapons.Add(new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Crusher));
            weapons.Add(new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Universal_Launcher_Front));

            List<MyMwcObjectBuilder_AssignmentOfAmmo> assignmentOfAmmo = new List<MyMwcObjectBuilder_AssignmentOfAmmo>();
            assignmentOfAmmo.Add(new MyMwcObjectBuilder_AssignmentOfAmmo(MyMwcObjectBuilder_FireKeyEnum.Primary, MyMwcObjectBuilder_AmmoGroupEnum.Bullet, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_Basic));
            assignmentOfAmmo.Add(new MyMwcObjectBuilder_AssignmentOfAmmo(MyMwcObjectBuilder_FireKeyEnum.Secondary, MyMwcObjectBuilder_AmmoGroupEnum.Missile, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Missile_Basic));

            var inventory = new MyMwcObjectBuilder_Inventory(new List<MyMwcObjectBuilder_InventoryItem>()
            {
                new MyMwcObjectBuilder_InventoryItem(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_Basic), 10000),
                new MyMwcObjectBuilder_InventoryItem(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Missile_Basic), 10000),
            }, 1000);

            MySmallShip smallShipEntity;
            if (!isPlayerShip)
            {
                var shipBuilder = new MyMwcObjectBuilder_SmallShip(shipType,
                        inventory,
                        weapons,
                        new MyMwcObjectBuilder_SmallShip_Engine(MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.PowerCells_1),
                        assignmentOfAmmo,
                        new MyMwcObjectBuilder_SmallShip_Armor(MyMwcObjectBuilder_SmallShip_Armor_TypesEnum.Basic),
                        null,
                        MyGameplayConstants.MAXHEALTH_SMALLSHIP,
                        1f,
                        100f,
                        float.MaxValue,
                        float.MaxValue,
                        true,
                        false,
                        MyLightsConstants.MAX_SPOTLIGHT_SHADOW_RANGE, 0);

                shipBuilder.Faction = MyMwcObjectBuilder_FactionEnum.Euroamerican;

                smallShipEntity = (MySmallShip)MyEntities.CreateFromObjectBuilderAndAdd(hudLabelText,
                    //new MyMwcObjectBuilder_SmallShip(shipType, 
                    //    weapons,
                    //    new MyMwcObjectBuilder_SmallShip_Engine(MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.Chemical_1), 
                    //    ammo, 
                    //    assignmentOfAmmo,
                    //    null),
                    shipBuilder,
                    Matrix.CreateWorld(position, Vector3.Forward, Vector3.Up));
            }
            else
            {
                Vector3 forward = new Vector3(-0.664f, 0.556f, -0.499f);
                Vector3 up = new Vector3(0.437f, 0.856f, 0.340f);

                smallShipEntity = (MySmallShip)MyEntities.CreateFromObjectBuilderAndAdd(hudLabelText,
                    //new MyMwcObjectBuilder_SmallShip_Player(
                    //    shipType, 
                    //    weapons,
                    //    new MyMwcObjectBuilder_SmallShip_Engine(MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.Chemical_1), 
                    //    ammo, 
                    //    null, 
                    //    assignmentOfAmmo),
                    new MyMwcObjectBuilder_SmallShip_Player(
                        shipType,
                        null,
                        weapons,
                        new MyMwcObjectBuilder_SmallShip_Engine(MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.Chemical_1),
                        assignmentOfAmmo,
                        null,
                        null,
                        null,
                        MyGameplayConstants.HEALTH_RATIO_MAX,
                        100f,
                        float.MaxValue,
                        float.MaxValue,
                        float.MaxValue,
                        true, false, MyLightsConstants.MAX_SPOTLIGHT_SHADOW_RANGE, 0),
                    Matrix.CreateWorld(position, forward, up));
            }

            smallShipEntity.Inventory.AddInventoryItem(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Automatic_Rifle_With_Silencer_High_Speed), 10, true);
            smallShipEntity.Inventory.AddInventoryItem(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_Basic), 10000, true);
            smallShipEntity.Inventory.AddInventoryItem(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_Armor_Piercing_Incendiary), 10000, true);
            smallShipEntity.Inventory.AddInventoryItem(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Missile_Basic), 500, true);
            smallShipEntity.Inventory.AddInventoryItem(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_Basic), 500, true);
            smallShipEntity.Inventory.AddInventoryItem(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_Tunnel_Buster), 1000, true);
            smallShipEntity.Inventory.AddInventoryItem(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Guided_Missile_Engine_Detection), 5, true);
            smallShipEntity.Inventory.AddInventoryItem(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Guided_Missile_Visual_Detection), 5, true);
            smallShipEntity.Inventory.AddInventoryItem(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Remote_Camera), 5, true);
            smallShipEntity.Inventory.AddInventoryItem(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Hologram), 5, true);
            smallShipEntity.Inventory.AddInventoryItem(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Mine_Basic), 15, true);
            smallShipEntity.Inventory.AddInventoryItem(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Mine_Smart), 15, true);

            return smallShipEntity;
        }

        public MySmallShipBot CreateFakeBot(string hudLabelText, MyMwcObjectBuilder_SmallShip_TypesEnum shipType, Vector3 position, float health, MyMwcObjectBuilder_FactionEnum faction)
        {
            MySmallShipBot bot = CreateFakeBot(hudLabelText, shipType, position, health, null, faction);
            MyEntities.TestEntityAfterInsertionForCollision(bot);
            return bot;
        }

        //  Create a bot ship, which is a special computer-controlled type of miner ship.
        public MySmallShipBot CreateFakeBot(string hudLabelText, MyMwcObjectBuilder_SmallShip_TypesEnum shipType, Vector3 position, float health, Vector3? diffuseColor, MyMwcObjectBuilder_FactionEnum faction)
        {
            List<MyMwcObjectBuilder_SmallShip_Weapon> weapons = new List<MyMwcObjectBuilder_SmallShip_Weapon>();
            List<MyMwcObjectBuilder_AssignmentOfAmmo> assignments = new List<MyMwcObjectBuilder_AssignmentOfAmmo>();

            weapons.Add(new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Autocanon));
            weapons.Add(new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Autocanon));
            weapons.Add(new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Cannon));
            weapons.Add(new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Cannon));
            weapons.Add(new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Shotgun));
            weapons.Add(new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Crusher));
            weapons.Add(new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Universal_Launcher_Front));
            //ammo.Add(new MyMwcObjectBuilder_SmallShip_Ammo(10000, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_Basic));
            //ammo.Add(new MyMwcObjectBuilder_SmallShip_Ammo(500, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_BioChem));
            //ammo.Add(new MyMwcObjectBuilder_SmallShip_Ammo(1000, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_Tunnel_Buster));
            //ammo.Add(new MyMwcObjectBuilder_SmallShip_Ammo(10, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Automatic_Rifle_With_Silencer_High_Velocity));
            //ammo.Add(new MyMwcObjectBuilder_SmallShip_Ammo(10000, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_High_Velocity));
            //ammo.Add(new MyMwcObjectBuilder_SmallShip_Ammo(500000, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_Basic));
            //ammo.Add(new MyMwcObjectBuilder_SmallShip_Ammo(100000, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_Tunnel_Buster));
            //ammo.Add(new MyMwcObjectBuilder_SmallShip_Ammo(5, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Guided_Missile_Engine_Detection));
            //ammo.Add(new MyMwcObjectBuilder_SmallShip_Ammo(5, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Guided_Missile_Visual_Detection));
            //ammo.Add(new MyMwcObjectBuilder_SmallShip_Ammo(50, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Shotgun_Basic));
            //ammo.Add(new MyMwcObjectBuilder_SmallShip_Ammo(5, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Hologram));
            //ammo.Add(new MyMwcObjectBuilder_SmallShip_Ammo(15, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Mine_Basic));
            //ammo.Add(new MyMwcObjectBuilder_SmallShip_Ammo(15, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Mine_Smart));
            assignments.Add(new MyMwcObjectBuilder_AssignmentOfAmmo(MyMwcObjectBuilder_FireKeyEnum.Primary, MyMwcObjectBuilder_AmmoGroupEnum.Cannon, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_Basic));
            assignments.Add(new MyMwcObjectBuilder_AssignmentOfAmmo(MyMwcObjectBuilder_FireKeyEnum.Secondary, MyMwcObjectBuilder_AmmoGroupEnum.Cannon, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_Basic));
            assignments.Add(new MyMwcObjectBuilder_AssignmentOfAmmo(MyMwcObjectBuilder_FireKeyEnum.Third, MyMwcObjectBuilder_AmmoGroupEnum.Bullet, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_Basic));
            assignments.Add(new MyMwcObjectBuilder_AssignmentOfAmmo(MyMwcObjectBuilder_FireKeyEnum.Fourth, MyMwcObjectBuilder_AmmoGroupEnum.Bullet, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Automatic_Rifle_With_Silencer_BioChem));
            assignments.Add(new MyMwcObjectBuilder_AssignmentOfAmmo(MyMwcObjectBuilder_FireKeyEnum.Fifth, MyMwcObjectBuilder_AmmoGroupEnum.Bullet, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_Basic));

            List<MyMwcObjectBuilder_InventoryItem> inventoryItems = new List<MyMwcObjectBuilder_InventoryItem>();
            MyMwcObjectBuilder_Inventory inventory = new MyMwcObjectBuilder_Inventory(inventoryItems, 1024);

            inventory.InventoryItems.Add(new MyMwcObjectBuilder_InventoryItem(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Automatic_Rifle_With_Silencer_High_Speed), 1000));
            inventory.InventoryItems.Add(new MyMwcObjectBuilder_InventoryItem(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_Basic), 1000));
            inventory.InventoryItems.Add(new MyMwcObjectBuilder_InventoryItem(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_Basic), 30));
            inventory.InventoryItems.Add(new MyMwcObjectBuilder_InventoryItem(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_Tunnel_Buster), 10));
            inventory.InventoryItems.Add(new MyMwcObjectBuilder_InventoryItem(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Guided_Missile_Engine_Detection), 10));
            inventory.InventoryItems.Add(new MyMwcObjectBuilder_InventoryItem(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Guided_Missile_Visual_Detection), 10));


            MySmallShipBot bot = (MySmallShipBot)MyEntities.CreateFromObjectBuilderAndAdd(hudLabelText,
                //new MyMwcObjectBuilder_SmallShip_Bot(shipType, 
                //    weapons,
                //    new MyMwcObjectBuilder_SmallShip_Engine(MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.Chemical_1), 
                //    ammo, 
                //    assignments,
                //    null,
                //    faction
                //    ),
                new MyMwcObjectBuilder_SmallShip_Bot(shipType,
                    inventory,
                    weapons,
                    new MyMwcObjectBuilder_SmallShip_Engine(MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.Chemical_1),
                    assignments,
                    null,
                    null,
                    null,
                    health,
                    1000f,
                    float.MaxValue,
                    float.MaxValue,
                    true,
                    false,
                    faction,
                    MyAITemplateEnum.AGGRESIVE,
                    0,
                    1000,
                    1000,
                    MyPatrolMode.CYCLE,
                    null,
                    BotBehaviorType.IDLE,
                    MyLightsConstants.MAX_SPOTLIGHT_SHADOW_RANGE,
                    0, false, true),
                    Matrix.CreateWorld(position, Vector3.Backward, Vector3.Up)
               );

            /*
            bot.Inventory.AddInventoryItem(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Remote_Camera), 50, true);
            bot.Inventory.AddInventoryItem(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Hologram), 50, true);
            bot.Inventory.AddInventoryItem(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Mine_Basic), 1500, true);
            bot.Inventory.AddInventoryItem(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Mine_Smart), 1500, true);
                */
            //MyEntities.TestEntityAfterInsertionForCollision(bot);

            //System.Diagnostics.Debug.Assert(bot.MaxHealth > health);
            //bot.DoDamage(0, bot.MaxHealth - health, 0, MyDamageType.Unknown, MyAmmoType.Unknown, null);
            bot.Faction = faction;

            return bot;
        }

        public MySmallShipBot CreateFriend(string name, float? maxHealth = null, float healthPercentage = 1.0f, MyMwcObjectBuilder_SmallShip_TypesEnum shipType = MyMwcObjectBuilder_SmallShip_TypesEnum.GETTYSBURG)
        {
            var bot = MyGuiScreenGamePlay.Static.CreateFakeBot(
               name,
               shipType,
               MySession.PlayerShip.GetFormationPosition(MySession.PlayerShip.Followers.Count),
               1f,
               MySession.PlayerShip.Faction);

            bot.MaxHealth = maxHealth.HasValue ? maxHealth.Value : bot.GetDefaultMaxHealth();
            bot.Health = bot.MaxHealth * healthPercentage;

            bot.Follow(MySession.PlayerShip);

            return bot;
        }

        void UpdateSmallshipsOfferedByVendors(MyMwcObjectBuilder_Checkpoint checkpoint)
        {
            if (m_checkpoint.SectorObjectBuilder.SectorObjects == null)
                return;

            MyMwcLog.WriteLine("UpdateSmallshipsOfferedByVendors - START");

            foreach (var ob in m_checkpoint.SectorObjectBuilder.SectorObjects)
            {
                MyMwcObjectBuilder_PrefabContainer prefabContainer = ob as MyMwcObjectBuilder_PrefabContainer;
                if (prefabContainer != null)
                {
                    foreach (var prefab in prefabContainer.Prefabs)
                    {
                        MyMwcObjectBuilder_PrefabHangar hangar = prefab as MyMwcObjectBuilder_PrefabHangar;
                        if (hangar != null)
                        { //check smallships contained in this vendor
                            if (prefabContainer.Inventory != null)
                            {
                                foreach (var inventoryItem in prefabContainer.Inventory.InventoryItems)
                                {
                                    MyMwcObjectBuilder_SmallShip_Player playerShip = inventoryItem.ItemObjectBuilder as MyMwcObjectBuilder_SmallShip_Player;

                                    if (playerShip != null)
                                    {
                                        var shipType = playerShip.ShipType;
                                        inventoryItem.ItemObjectBuilder = MyMwcObjectBuilder_SmallShip_Player.CreateDefaultShip(shipType, playerShip.Faction, MyShipTypeConstants.GetShipTypeProperties(shipType).GamePlay.CargoCapacity);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            MyMwcLog.WriteLine("UpdateSmallshipsOfferedByVendors - END");
        }
    }
}