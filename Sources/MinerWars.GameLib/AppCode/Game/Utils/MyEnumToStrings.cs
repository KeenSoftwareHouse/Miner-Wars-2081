using System;
using System.Collections.Generic;
using System.Diagnostics;
using MinerWarsMath;
using MinerWars.AppCode.Game.Audio;
using MinerWars.AppCode.Game.Cockpit;
using MinerWars.AppCode.Game.Decals;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.HUD;
using MinerWars.AppCode.Game.TransparentGeometry;
using MinerWars.AppCode.Game.VideoMode;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.CommonLIB.AppCode.Utils;
using SysUtils.Utils;
using MinerWars.AppCode.Game.GUI.Core;
using SysUtils;
using MinerWars.AppCode.Game.Localization;

namespace MinerWars.AppCode.Game.Utils
{
    //  I have made this mapping array because when using obfuscator, enum names change and when converted to string, they no more corresponds to file names
    static partial class MyEnumsToStrings
    {
        public static string[] CameraDirection = new string[] { "FORWARD", "BACKWARD", "SHIP_CUSTOMIZATION_SCREEN" };

        public static string[] Particles = new string[] { 
            "Explosion.dds", 
            "ExplosionSmokeDebrisLine.dds", 
            "Smoke.dds", 
            "Test.dds", 
            "EngineThrustMiddle.dds", 
            "ReflectorCone.dds", 
            "ReflectorGlareAdditive.dds", 
            "ReflectorGlareAlphaBlended.dds", 
            "MuzzleFlashMachineGunFront.dds",
            "MuzzleFlashMachineGunSide.dds", 
            "ProjectileTrailLine.dds", 
            "ContainerBorder.dds",
            "Dust.dds",
            "Crosshair.dds",
            "Sun.dds",
            "LightRay.dds",
            "LightGlare.dds",
            "SolarMapOrbitLine.dds",
            "SolarMapSun.dds",
            "SolarMapAsteroidField.dds",
            "SolarMapFactionMap.dds",
            "SolarMapAsteroid.dds",
            "SolarMapZeroPlaneLine.dds",
            "SolarMapSmallShip.dds",
            "SolarMapLargeShip.dds",
            "SolarMapOutpost.dds",

            "Grid.dds",
            "ContainerBorderSelected.dds",

            // Factions
            "FactionRussia.dds",
            "FactionChina.dds",
            "FactionJapan.dds",
            "FactionUnitedKorea.dds",
            "FactionFreeAsia.dds",
            "FactionSaudi.dds",
            "FactionEAC.dds",
            "FactionCSR.dds",
            "FactionIndia.dds",
            "FactionChurch.dds",
            "FactionOmnicorp.dds",
            "FactionFourthReich.dds",
            "FactionSlavers.dds",

            "Smoke_b.dds",
            "Smoke_c.dds",

            "Sparks_a.dds",
            "Sparks_b.dds",
            "particle_stone.dds",
            "Stardust.dds",
            "particle_trash_a.dds",
            "particle_trash_b.dds",
            "particle_glare.dds",
            "smoke_field.dds",
            "Explosion_pieces.dds",
            "particle_laser.dds",
            "particle_nuclear.dds",
            "Explosion_line.dds",
            "particle_flash_a.dds",
            "particle_flash_b.dds",
            "particle_flash_c.dds",
            "snap_point.dds",

            "SolarMapNavigationMark.dds",

            "Impostor_StaticAsteroid20m_A.dds",
            "Impostor_StaticAsteroid20m_C.dds",
            "Impostor_StaticAsteroid50m_D.dds",
            "Impostor_StaticAsteroid50m_E.dds",

            "GPS.dds",
            "GPSBack.dds",

            "ShotgunParticle.dds",

            "ObjectiveDummyFace.dds",
            "ObjectiveDummyLine.dds",

            "SunDisk.dds",

            "scanner_01.dds",
            "Smoke_square.dds",
            "Smoke_lit.dds",

            "SolarMapSideMission.dds",
            "SolarMapStoryMission.dds",
            "SolarMapTemplateMission.dds",
            "SolarMapPlayer.dds",
        };

        public static string[] HudTextures = new string[] { "Line.tga", "DirectionIndicator_blue.png", "DirectionIndicator_green.png", "DirectionIndicator_red.png", "DirectionIndicator_white.png",
            "Target.png", "Crosshair_locked.png", "Crosshair_side_locked.png", "Crosshair01.png", "HorizontalLineLeft.png", "HorizontalLineRight.png", "damage_direction.png", "BackCameraOverlay.png",/* "RadarOverlay.png",*/ "Rectangle.png", 
            "hudStatusArmor_blue.png", "hudStatusArmor_red.png", "hudStatusFuel_blue.png", "hudStatusFuel_red.png", 
            "hudStatusOxygen_blue.png", "hudStatusOxygen_red.png", "hudStatusPlayerHealth_blue.png", "hudStatusPlayerHealth_red.png", "hudStatusShipDamage_blue.png", "hudStatusShipDamage_red.png", 
            "hudStatusSpeed.png", "hudStatusBar_blue.png", "hudStatusBar_red.png", "hudUnderbarSmall.png", "hudUnderbarBig.png","BlueTarget.png","GreenTarget.png","RedTarget.png", "CargoBoxIndicator.png",
            "Unpowered_blue.png", "Unpowered_green.png", "Unpowered_red.png", "Unpowered_white.png", "HudOre.png", "crosshair_nazzi.png", "crosshair_omnicorp.png", "crosshair_russian.png", "crosshair_templary.png", "Sun.tga",
            "crosshair_nazzi_red.png", "crosshair_omnicorp_red.png", "crosshair_russian_red.png", "crosshair_templary_red.png", "Crosshair01_red.png",
        };

        public static string[] HudRadarTextures = new string[] {"Arrow.png", "ImportantObject.tga", "LargeShip.tga",
            "Line.tga", "RadarBackground.tga", "RadarPlane.tga", "SectorBorder.tga",
            "SmallShip.tga", "Sphere.png", "SphereGrid.tga", "Sun.tga" , "OreDeposit_Treasure.png", "OreDeposit_Helium.png",
            "OreDeposit_Ice.png", "OreDeposit_Iron.png", "OreDeposit_Lava.png", "OreDeposit_Gold.png", "OreDeposit_Platinum.png", 
            "OreDeposit_Silver.png", "OreDeposit_Silicon.png", "OreDeposit_Organic.png", "OreDeposit_Nickel.png", "OreDeposit_Magnesium.png", 
            "OreDeposit_Uranite.png", "OreDeposit_Cobalt.png", "OreDeposit_Snow.png" };

        public static string[] Decals = new string[] { "ExplosionSmut", "BulletHoleOnMetal", "BulletHoleOnRock" };

        public static string[] CockpitGlassDecals = new string[] { "DirtOnGlass", "BulletHoleOnGlass", "BulletHoleSmallOnGlass" };

        public static string[] CameraAttachedTo = new string[] { "PlayerShip", "BotMinerShip", "PlayerMinerShip_ThirdPersonStatic", 
            "PlayerMinerShip_ThirdPersonFollowing", "Spectator", "PlayerMinerShip_ThirdPersonDynamic", "Drone", "Camera", "LargeWeapon" };

        //public static string[] Sounds = new string[] { "GuiMouseClick", "GuiMouseOver",
        //    "GuiEditorFlyOutsideBorder", "GuiEditorObjectAttach", "GuiEditorObjectDelete", "GuiEditorObjectDetach",
        //    "GuiEditorObjectMoveInvalid", "GuiEditorObjectMoveStep", "GuiEditorObjectRotateStep", "GuiEditorObjectSelect",
        //    "GuiEditorPrefabCommit", "GuiEditorPrefabEnter", "GuiEditorPrefabExit",
        //    "GuiEditorVoxelHandAdd", "GuiEditorVoxelHandRemove", "GuiEditorVoxelHandSoften","GuiEditorVoxelHandMaterial","GuiEditorVoxelHandSwitch",
        //    "GuiWheelControlOpen", "GuiWheelControlClose",
        //    "ImpRockCollideMetal", "ImpRockCollideRock", "ImpBulletHitRock", "ImpBulletHitMetal", "ImpBulletHitGlass", "ImpBulletHitShip", 
        //    "ImpExpHitGlass","ImpExpHitMetal","ImpExpHitShip","ImpExpHitRock",
        //    "ImpShipCollideMetal", "ImpShipCollideRock", "ImpPlayerShipCollideMetal", "ImpPlayerShipCollideRock", "ImpPlayerShipCollideShip", "ImpPlayerShipScrapeShipLoop", "ImpPlayerShipScrapeShipRelease",
        //    "VehShipaEngineIdle2d", "VehShipaEngineIdle3d", "VehShipaEngineHigh2d", "VehShipaEngineHigh3d", "VehShipaThrust2d", "VehShipaThrust3d", "VehShipaLightsOn", "VehShipaLightsOff", 
        //    "VehShipaEngineOn", "VehShipaEngineOff", 

        //    "VehHarvesterTubeRelease2d","VehHarvesterTubeRelease3d","VehHarvesterTubeMovingLoop2d","VehHarvesterTubeMovingLoop3d",
        //    "VehHarvesterTubeColliding2d","VehHarvesterTubeColliding3d","VehHarvesterTubeCollision2d","VehHarvesterTubeCollision3d","VehHarvesterTubeImplode2d",
        //    "VehHarvesterTubeImplode3d","VehToolCrusherDrillLoop2d","VehToolCrusherDrillLoop3d","VehToolCrusherDrillRelease2d","VehToolCrusherDrillRelease3d",
        //    "VehToolCrusherDrillColliding2d","VehToolCrusherDrillColliding3d", "VehToolCrusherDrillCollidingRelease2d","VehToolCrusherDrillCollidingRelease3d",

        //    "VehToolThermalDrillLoop2d", "VehToolThermalDrillLoop3d","VehToolThermalDrillRelease2d","VehToolThermalDrillRelease3d",
        //    "VehToolSawCut2d", "VehToolSawCut3d", "VehToolSawLoop2d", "VehToolSawLoop3d", "VehToolSawRelease2d", "VehToolSawRelease3d",
        //    "VehToolLaserDrillLoop2d", "VehToolLaserDrillLoop3d","VehToolLaserDrillRelease2d","VehToolLaserDrillRelease3d",
        //    "WepMissileLock", "WepMissileFly", "WepMissileExplosion", "WepMissileLaunch2d", "WepMissileLaunch3d", "WepAutocanonFire2d", "WepAutocanonFire3d", "WepAutocanonRel2d", "WepAutocanonRel3d",
        //    "WepSniperScopeZoomRel", "WepSniperScopeZoomALoop", "WepSniperNormFire2d","WepSniperHighFire2d", "WepSniperNormFire3d","WepSniperHighFire3d", "WepMineMoveALoop","WepUnivLaunch2d","WepUnivLaunch3d","WepArsHighShot2d","WepArsHighShot3d","WepArsNormShot2d", "WepArsNormShot3d",
        //    "WepBombExplosion",
        //    "WepLargeShipAutocannonRotate",
        //    "WepMachineGunHighFire2d","WepMachineGunHighFire3d","WepMachineGunHighRel2d","WepMachineGunHighRel3d","WepMachineGunNormFire2d","WepMachineGunNormFire3d","WepMachineGunNormRel2d","WepMachineGunNormRel3d",
        //    "WepShotgunNormShot2d", "WepShotgunNormShot3d", "WepShotgunHighShot2d", "WepShotgunHighShot3d",
        //    "WepRailNormShot3d","WepRailNormShot2d","WepRailHighShot3d","WepRailHighShot2d",
        //    "WepBombSmartTimer","WepBombSmartSmoke","WepBombSmartPlant","WepBombSmartDrone","WepBombGravSuck","WepBombFlash", 
        //    "SfxSolarWind", "SfxHudBackcameraOn","SfxHudBackcameraOff", "SfxPlayerBreath", "SfxShipSmallExplosion", "SfxHudReflectorRange", "SfxHudSlowMovementOff", "SfxHudSlowMovementOn", "SfxHudAutolevelingOn", "SfxHudAutolevelingOff", 
        //    "SfxHudWeaponScroll", "SfxHudWeaponSelect", "SfxHudRadarMode","SfxHudCockpitOn", "SfxHudCockpitOff", "SfxSpark",
        //    "MusMainMenu", "MusStoryAmbient", "MusStoryMission01", 
        //    "MovDoor1AClose","MovDoor1AOpen", 
        //    "MovDoor1BClose","MovDoor1BOpen", 
        //    "MovDoor2AClose","MovDoor2AOpen", 
        //    "MovDoor2BClose","MovDoor2BOpen", 
        //    "MovDoor3AClose","MovDoor3AOpen", 
        //    "HudFuelLowWarning", "HudFuelCriticalWarning",
        //    "HudOxygenLowWarning", "HudOxygenCriticalWarning", "HudLowBatteryWarning", "HudNoBatteryWarning", "HudDamageAlertWarning", "HudDamageCriticalWarning",
        //    "HudAmmoCriticalWarning", "HudAmmoLowWarning", "HudArmorCriticalWarning", "HudArmorLowWarning", "HudDestinationReached", "HudRadarJammedWarning",
        //    "SfxTargetDestroyed", "HudRadiationWarning","SfxGeigerCounterHeavyLoop", "HudEnemyAlertWarning", "HudFriendlyFireWarning", "HudHarvestingComplete", "HudIndestructDrillWarning",
        //    "HudSolarFlareWarning", "HudIndestructHarvest", "HudInventoryComplete", "HudInventoryFullWarning", "HudInventoryTransfer", "HudObjectiveComplete", "HudGameOver",
        //    "HudHealthLowWarning", "HudHealthCriticalWarning", "HudMissionComplete", "SfxClaxonAlert",
        //    "VehToolCrusherDrillIdle2d", "VehToolCrusherDrillIdle3d", "VehToolLaserDrillIdle2d", "VehToolLaserDrillIdle3d", "VehToolSawIdle2d", "VehToolSawIdle3d", "VehToolThermalDrillIdle2d", "VehToolThermalDrillIdle3d", "SfxGps", "SfxGpsFail", 
        //    "MusHorrorOrMystery", "MusLightFight", "MusTensionBeforeAnAction", "MusCalmAtmosphere", "MusHeavyFight", "MusVictory", "MusSadnessOrDesperation", "MusStressOrTimeRush", "MusStealthAction", "MusDesperateWithStress", "MenuWelcome",

        //    "MovDock1Start", "MovDock1Loop", "MovDock1End", "MovDock2Start", "MovDock2Loop", "MovDock2End","MovDock3Start", "MovDock3Loop", "MovDock3End", 
        //    "VehCH1EngineHigh2d", "VehCH1EngineHigh3d", "VehCH1EngineIdle2d", "VehCH1EngineIdle3d", "VehEL1EngineHigh2d", "VehEL1EngineHigh3d", "VehEL1EngineIdle2d", "VehEL1EngineIdle3d",
        //    "VehEL1EngineOff", "VehEL1EngineOn", "VehNU1EngineHigh2d", "VehNU1EngineHigh3d", "VehNU1EngineIdle2d", "VehNU1EngineIdle3d", "ImpShipQuake",

        //    "Amb2D_RoomLarge01", "Amb2D_RoomLarge02", "Amb2D_RoomLarge03", "Amb2D_RoomLarge04", "Amb2D_RoomLarge05", "Amb2D_RoomMed01", "Amb2D_RoomMed02", "Amb2D_RoomMed03", "Amb2D_RoomMed04", "Amb2D_RoomMed05",
        //    "Amb2D_RoomSmall01", "Amb2D_RoomSmall02", "Amb2D_RoomSmall03", "Amb2D_RoomSmall04", "Amb2D_StressLoop", "Amb2D_TunnelLarge01", "Amb2D_TunnelMedium01", "Amb2D_TunnelSmall01", "Amb3D_Electrical01", "Amb3D_Electrical02", 
        //    "Amb3D_FanLargeDamaged", "Amb3D_FanLargeDestroyed", "Amb3D_FanLargeNormal", "Amb3D_FanMediumDamaged", "Amb3D_FanMediumDestroyed", "Amb3D_FanMediumNormal", "Amb3D_FanSmallDamaged", "Amb3D_FanSmallDestroyed", "Amb3D_FanSmallNormal",
        //    "Amb3D_GenLargeDamaged", "Amb3D_GenLargeDestroyed", "Amb3D_GenLargeNormal", "Amb3D_GenMediumDamaged", "Amb3D_GenMediumDestroyed", "Amb3D_GenMediumNormal", "Amb3D_GenSmallDamaged", "Amb3D_GenSmallDestroyed", "Amb3D_GenSmallNormal",
        //    "Amb3D_PipeFlow01", "Amb3D_RadioChatterAllied01-04", "Amb3D_RadioChatterAllied05-08", "Amb3D_RadioChatterAllied09-12", "Amb3D_RadioChatterAllied13-16", "Amb3D_RadioChatterChinese01-04", "Amb3D_RadioChatterChinese05-08", 
        //    "Amb3D_RadioChatterChinese09-12", "Amb3D_RadioChatterChinese13-16", "Amb3D_RadioChatterRussian01-04", "Amb3D_RadioChatterRussian05-08", "Amb3D_RadioChatterRussian09-12", "Amb3D_RadioChatterRussian13-16", "Amb3D_Spark01",
        //    "Amb3D_SteamDischarge01", "Amb3D_SteamDischarge02", "Amb3D_SteamDischarge03", "Amb3D_SteamDischarge04", "Amb3D_SteamLoop01", "Amb3D_SteamLoop02", "Amb3D_SteamLoop03", "Amb3D_SteamLoop04", "Amb3D_SteamLoop05", "Amb3D_ThunderClapLarge",
        //    "Amb3D_ThunderClapMed", "Amb3D_ThunderClapSmall", "Amb3D_Welding01",

        //    "VehToolLaserDrillColliding2d", "VehToolLaserDrillColliding3d", "VehToolLaserDrillCollidingRelease2d", "VehToolLaserDrillCollidingRelease3d", 
        //    "VehToolThermalDrillColliding2d", "VehToolThermalDrillColliding3d", "VehToolThermalDrillCollidingRelease2d", "VehToolThermalDrillCollidingRelease3d",

        //    "SfxProgressHack", "SfxCancelHack", "SfxProgressRepair", "SfxProgressBuild", "SfxCancelRepair", "SfxCancelBuild", "Amb3D_GenXstart", "Amb3D_GenXloop", "SfxAlertVoc", "Amb3D_GenXend",
        //    "Amb3D_Temple1", "Amb3D_Temple2", "Amb3D_Temple3", "SfxShipLargeExplosion",

        //    "MusCalmAtmosphere_MM01", "MusCalmAtmosphere_MM02", "MusDesperateWithStress_MM01", "MusDesperateWithStress_KA01", "MusCalmAtmosphere_KA01",
        //    "MusHeavyFight_MM01", "MusHeavyFight_MM02", "MusCalmAtmosphere_KA02", "MusLightFight_MM01", "MusLightFight_MM02", "MusLightFight_KA01",
        //    "MusSadnessOrDesperation_MM01", "MusSadnessOrDesperation_MM02", "MusSadnessOrDesperation_KA02", "MusSadnessOrDesperation_KA01", 
        //    "MusStealthAction_MM01", "MusStealthAction_MM02", "MusStealthAction_KA01", "MusStressOrTimeRush_MM01", "MusStressOrTimeRush_MM02",
        //    "MusStressOrTimeRush_KA01", "MusStressOrTimeRush_KA02", "MusTensionBeforeAnAction_MM01", "MusTensionBeforeAnAction_MM02", 
        //    "MusTensionBeforeAnAction_KA01", "MusTensionBeforeAnAction_KA02", "MusSpecial_MM01", "MusVictory_MM01", "MusVictory_MM02",
        //    "MusVictory_KA01", "MusVictory_KA02", "MusHeavyFight_KA01", "MusHeavyFight_KA02", "MusCalmAtmosphere_KA03", "MusDesperateWithStress_KA02",
        //    "MusDesperateWithStress_KA03", "MusDesperateWithStress_KA04", "MusMainMenu_KA+MM", "MusHorror_KA01", "MusHorror_MM01",
        //    "MusMystery_MM01", "MusMystery_KA02", "MusMystery_KA01", "MusMystery_MM02", "MusHorror_KA02", "MusHorror_MM02",
        //    "SfxHudAlarmIncoming", "SfxHudAlarmDamageA", "SfxHudAlarmDamageB", "SfxHudAlarmDamageC", "SfxHudAlarmDamageD", "SfxHudAlarmDamageE", "WepNoAmmo", "WepCannon2d", "WepCannon3d", "VehCH1EngineOff", "VehCH1EngineOn", "VehNU1EngineOff", "VehNU1EngineOn",
        //    "VehToolPressureDrillBlast2d","VehToolPressureDrillIdle2d", "VehToolPressureDrillRecharge2d", "VehToolPressureDrillBlastRock2d",
        //    "Amb3D_PrefabFire", "SfxProgressActivation", "SfxCancelActivation",

        //    "Dlg_EACSurveySite_0001","Dlg_EACSurveySite_0002","Dlg_EACSurveySite_0003","Dlg_EACSurveySite_0004","Dlg_EACSurveySite_0005","Dlg_EACSurveySite_0006","Dlg_EACSurveySite_0007","Dlg_EACSurveySite_0008",
        //    "Dlg_EACSurveySite_0009","Dlg_EACSurveySite_0010","Dlg_EACSurveySite_0011","Dlg_EACSurveySite_0012_01","Dlg_EACSurveySite_0012_02","Dlg_EACSurveySite_0013","Dlg_EACSurveySite_0014_01",
        //    "Dlg_EACSurveySite_0014_02","Dlg_EACSurveySite_0015","Dlg_EACSurveySite_0016","Dlg_EACSurveySite_0017_01","Dlg_EACSurveySite_0017_02","Dlg_EACSurveySite_0018_01","Dlg_EACSurveySite_0018_02",
        //    "Dlg_EACSurveySite_0018_03","Dlg_EACSurveySite_0019","Dlg_EACSurveySite_0020","Dlg_EACSurveySite_0021","Dlg_EACSurveySite_0022","Dlg_EACSurveySite_0023","Dlg_EACSurveySite_0024_01",
        //    "Dlg_EACSurveySite_0024_02","Dlg_EACSurveySite_0025_01","Dlg_EACSurveySite_0025_02","Dlg_EACSurveySite_0026","Dlg_EACSurveySite_0027_01","Dlg_EACSurveySite_0027_02","Dlg_EACSurveySite_0028",
        //    "Dlg_EACSurveySite_0029","Dlg_EACSurveySite_0030","Dlg_EACSurveySite_0031","Dlg_EACSurveySite_0032_01","Dlg_EACSurveySite_0032_02","Dlg_EACSurveySite_0033","Dlg_EACSurveySite_0034",
        //    "Dlg_EACSurveySite_0035","Dlg_EACSurveySite_0036","Dlg_EACSurveySite_0037","Dlg_EACSurveySite_0038","Dlg_EACSurveySite_0039","Dlg_EACSurveySite_0040_01","Dlg_EACSurveySite_0040_02",
        //    "Dlg_EACSurveySite_0041","Dlg_EACSurveySite_0042","Dlg_EACSurveySite_0043","Dlg_EACSurveySite_0044","Dlg_EACSurveySite_0045","Dlg_EACSurveySite_0046","Dlg_EACSurveySite_0047",
        //    "Dlg_EACSurveySite_0048","Dlg_EACSurveySite_0049_01","Dlg_EACSurveySite_0049_02","Dlg_EACSurveySite_0050","Dlg_EACSurveySite_0051","Dlg_EACSurveySite_0052","Dlg_EACSurveySite_0053_01",
        //    "Dlg_EACSurveySite_0053_02","Dlg_EACSurveySite_0053_03","Dlg_EACSurveySite_0054","Dlg_EACSurveySite_0055_01","Dlg_EACSurveySite_0055_02","Dlg_EACSurveySite_0056","Dlg_EACSurveySite_0057",
        //    "Dlg_EACSurveySite_0058","Dlg_EACSurveySite_0059","Dlg_EACSurveySite_0060_01","Dlg_EACSurveySite_0060_02","Dlg_EACSurveySite_0061","Dlg_EACSurveySite_0062","Dlg_EACSurveySite_0063",
        //    "Dlg_EACSurveySite_0064","Dlg_EACSurveySite_0065","Dlg_EACSurveySite_0066","Dlg_EACSurveySite_0067","Dlg_EACSurveySite_0068","Dlg_EACSurveySite_0069","Dlg_EACSurveySite_0070","Dlg_EACSurveySite_0071",
        //    "Dlg_EACSurveySite_0072","Dlg_EACSurveySite_0073","Dlg_EACSurveySite_0074","Dlg_EACSurveySite_0075","Dlg_EACSurveySite_0076","Dlg_EACSurveySite_0077","Dlg_EACSurveySite_0078",

        //     "SfxTakeAll", "SfxAcquireWeaponOn", "SfxAcquireWeaponOff", "SfxAcquireDroneOn", "SfxAcquireDroneOff", "SfxAcquireCameraOn", "SfxAcquireCameraOff", "SfxTakeAllUniversal", "SfxTakeAllAmmo", "SfxTakeAllEnergy", "SfxTakeAllFuel", "SfxTakeAllMedkit", "SfxTakeAllOxygen", "SfxTakeAllRepair", "HudMissionFailed",

        //     "MusLightFight_KA03", "MusLightFight_KA04", "MusLightFight_KA05", "MusLightFight_KA07", "MusLightFight_KA08", "MusLightFight_KA10", "MusLightFight_KA11", "MusSpecial_MM02", "MusSpecial_MM03", "MusSpecial_KA01", "MusHeavyFight_KA03", 
        //     "SfxProgressTake", "SfxCancelTake", "SfxProgressPut", "SfxCancelPut",

        //     "Amb2D_City", "Amb2D_ComputerRoom", "Amb2D_Factory", "Amb2D_LostPlace", "Amb2D_RedHeat", "Amb2D_War", "HudAmmoNoWarning", "HudArmorNoWarning",
             
        //      "VehToolCrusherDrillCollidingOtherRelease2d",
        //      "VehToolCrusherDrillCollidingOtherRelease3d",
        //      "VehToolLaserCollidingOther2d",
        //      "VehToolLaserCollidingOther3d",
        //      "VehToolLaserCollidingOtherRelease2d",
        //      "VehToolLaserCollidingOtherRelease3d",
        //      "VehToolPressureDrillBlastOther2d",
        //      "VehToolPressureDrillBlastOther3d",
        //      "VehToolSawCutOther2d",
        //      "VehToolSawCutOther3d",
        //      "VehToolSawCutOtherRelease2d",
        //      "VehToolSawCutOtherRelease3d",
        //      "VehToolSawCutRelease2d",
        //      "VehToolSawCutRelease3d",
        //      "VehToolThermalDrillCollidingOther2d",
        //      "VehToolThermalDrillCollidingOther3d",
        //      "VehToolThermalDrillCollidingOtherRelease2d",
        //      "VehToolThermalDrillCollidingOtherRelease3d",
        //      "VehToolCrusherDrillCollidingOther2d",
        //      "VehToolCrusherDrillCollidingOther3d",

        //      "SfxFlareLoop01", "SfxFlareDeploy", "Amb3D_EngineThrust", "VehLoopDrone", "VehLoopCamera", "WepEpmExplosion", "HudOxygenLeakingWarning", "VehLoopLargeShip",
        //      "MusLightFight_KA12", "MusHeavyFight_KA05", "MusSpecial_KA02", "MusHeavyFight_KA07", "MusSpecial_MM04", "MusHeavyFight_KA04", "MusSpecial_KA03", 
        //      "SfxMeteorFly", "SfxMeteorExplosion", "WepLargeShipAutocannonRotateRelease", "SfxNuclearExplosion",
        //      "Amb2D_City2", "Amb2D_Factory2", "MusLightFight_KA27", "MusSpecial_KA04", "MusSpecial_KA05", "MusSpecial_MM05",

        //      "VehToolNuclearDrillLoop3d", "VehToolNuclearDrillLoop2d", 
        //      "VehToolNuclearDrillRelease3d", "VehToolNuclearDrillRelease2d", 
        //      "VehToolNuclearDrillColliding3d", "VehToolNuclearDrillColliding2d", 
        //      "VehToolNuclearDrillCollidingOther3d", "VehToolNuclearDrillCollidingOther2d", 
        //      "VehToolNuclearDrillCollidingRelease3d", "VehToolNuclearDrillCollidingRelease2d", 
        //      "VehToolNuclearDrillCollidingOtherRelease3d", "VehToolNuclearDrillCollidingOtherRelease2d", 
        //      "VehToolNuclearDrillIdle3d", "VehToolNuclearDrillIdle2d", 
        //      "VehToolPressureDrillBlast3d", "VehToolPressureDrillBlastRock3d", "VehToolPressureDrillIdle3d", "VehToolPressureDrillRecharge3d", 
        //      "SfxPlayerDeathBeep", "SfxPlayerDeathBreath", "Amb2D_RacingFans",

        //      "Dlg_EACSurveySite_1000", "Dlg_EACSurveySite_1001", "Dlg_EACSurveySite_1002", "Dlg_EACSurveySite_1003", "Amb2D_RedHeat2",
        //      "Sht_Reich_JE_ForTheFuture", "Sht_Reich_JE_Vanquish", "Sht_Reich_JE_WeControl", "Sht_Reich_JE_YouAreTheWeakLink", "Sht_Reich_JE_YourFinalDay", 
        //};

        //  IMPORTANT: These strings are referenced in config file. If you change any, assigned controls will be probably set to defaults.
        public static string[] GuiInputDeviceEnum = new string[] { "None", "Keyboard", "Mouse", "Joystick", "JoystickAxis" };
        public static string[] MouseButtonsEnum = new string[] { "None", "Left", "Middle", "Right", "XButton1", "XButton2" };
        public static string[] JoystickButtonsEnum = new string[] { "None", "JDLeft", "JDRight", "JDUp", "JDDown", "J01", "J02", "J03", "J04", "J05", "J06", "J07", "J08", "J09", "J10", "J11", "J12", "J13", "J14", "J15", "J16" };
        public static string[] JoystickAxesEnum = new string[] { "None", 
            "JXAxis+","JXAxis-", "JYAxis+","JYAxis-", "JZAxis+","JZAxis-",
            "JXRotation+","JXRotation-", "JYRotation+","JYRotation-", "JZRotation+","JZRotation-",
            "JSlider1+","JSlider1-", "JSlider2+","JSlider2-",
        };
        public static string[] GameControlEnums = new string[] {
            "FIRE_PRIMARY", "FIRE_SECONDARY", "FIRE_THIRD", "FIRE_FOURTH", "FIRE_FIFTH",
            "FORWARD", "REVERSE", "STRAFE_LEFT", "STRAFE_RIGHT", "UP_THRUST", "DOWN_THRUST", "ROLL_LEFT", "ROLL_RIGHT",
            "WHEEL_CONTROL", "HEADLIGHTS", "HEADLIGHTS_DISTANCE", "HARVEST", "DRILL",
            "USE", "INVENTORY", "GPS", "MISSION_DIALOG", "TRAVEL", 
            "QUICK_ZOOM", "ZOOM_IN", "ZOOM_OUT", "SELECT_AMMO_BULLET",
            "SELECT_AMMO_MISSILE", "SELECT_AMMO_CANNON", "SELECT_AMMO_UNIVERSAL_LAUNCHER_FRONT", "AUTO_LEVEL",  "REAR_CAM",
            "SELECT_AMMO_UNIVERSAL_LAUNCHER_BACK", "MOVEMENT_SLOWDOWN", "VIEW_MODE",
            "WEAPON_SPECIAL", "CHANGE_DRONE_MODE", "ROTATION_LEFT", "ROTATION_RIGHT", "ROTATION_UP",
            "ROTATION_DOWN", "AFTERBURNER", "PREVIOUS_CAMERA", "NEXT_CAMERA",
            "FIRE_HOLOGRAM_FRONT", "FIRE_HOLOGRAM_BACK",
            "FIRE_BASIC_MINE_FRONT", "FIRE_BASIC_MINE_BACK",
            "FIRE_SMART_MINE_FRONT", "FIRE_SMART_MINE_BACK",
            "FIRE_FLASH_BOMB_FRONT", "FIRE_FLASH_BOMB_BACK",
            "FIRE_DECOY_FLARE_FRONT", "FIRE_DECOY_FLARE_BACK",
            "FIRE_SMOKE_BOMB_FRONT", "FIRE_SMOKE_BOMB_BACK",
            "DRONE_DEPLOY", "DRONE_CONTROL", "CONTROL_SECONDARY_CAMERA", "NOTIFICATION_CONFIRMATION",
            "PREV_TARGET", "NEXT_TARGET",
            "CHAT", "SCORE"
        };

        public static string[] EditorControlEnums = new string[] { "PRIMARY_ACTION_KEY", "SECONDARY_ACTION_KEY",
            "INCREASE_GRID_SCALE", "DECREASE_GRID_SCALE", "VOXEL_HAND", "ENTER_CONTEXT_MENU", "SWITCH_GIZMO_SPACE", "SWITCH_GIZMO_MODE"};

        public static string[] ControlTypeEnum = new string[] { "General", "Navigation", "Communications", "Weapons", "SpecialWeapons", "Systems1", "Systems2", "Editor", "Deleted" };

        public static string[] SessionType = new string[] { "NEW_STORY", "LOAD_CHECKPOINT", "JOIN_FRIEND_STORY", "MMO", "SANDBOX_OWN", "SANDBOX_FRIENDS", "JOIN_SANDBOX_FRIEND", "EDITOR_SANDBOX", "EDITOR_STORY", "EDITOR_MMO", "SANDBOX_RANDOM" };

        public static string[] LanguageEnums = new string[]
        {
            "en","cs","sk","de","ru","es","fr","it"
        };

        static MyEnumsToStrings()
        {
            //  We need to check if programmer who changed/added entries in enum, didn't forget to add it also to these string constants
            //  If he forgot, application will fail here on start, so he can find out quickly
            try
            {
                Validate(typeof(MyGameControlEnums), GameControlEnums);
                Validate(typeof(MyEditorControlEnums), EditorControlEnums);
                Validate(typeof(MyCameraAttachedToEnum), CameraAttachedTo);
                Validate(typeof(MyCameraDirection), CameraDirection);
                Validate(typeof(MyTransparentGeometryTexturesEnum), Particles);
                Validate(typeof(MyHudTexturesEnum), HudTextures);
                Validate(typeof(MyDecalTexturesEnum), Decals);
                Validate(typeof(MyCockpitGlassDecalTexturesEnum), CockpitGlassDecals);
                //Validate(typeof(MySoundCuesEnum), Sounds);
                Validate(typeof(MyGuiInputDeviceEnum), GuiInputDeviceEnum);
                Validate(typeof(MyMouseButtonsEnum), MouseButtonsEnum);
                Validate(typeof(MyJoystickButtonsEnum), JoystickButtonsEnum);
                Validate(typeof(MyJoystickAxesEnum), JoystickAxesEnum);
                Validate(typeof(MyGuiControlTypeEnum), ControlTypeEnum);
                Validate(typeof(MyMwcStartSessionRequestTypeEnum), SessionType);
                Validate(typeof(MyLanguagesEnum), LanguageEnums);
            }
            catch (Exception e)
            {
                Debug.Fail("Validation threw an exception: " + e.Message);
            }
        }

        static void Validate<T>(Type type, T list) where T : IList<string>
        {
            Array values = Enum.GetValues(type);
            Type underlyingType = Enum.GetUnderlyingType(type);
            if (underlyingType == typeof(System.Byte))
            {
                foreach (byte value in values)
                {
                    MyCommonDebugUtils.AssertRelease(list[value] != null);
                }
            }
            else if (underlyingType == typeof(System.Int16))
            {
                foreach (short value in values)
                {
                    MyCommonDebugUtils.AssertRelease(list[value] != null);
                }
            }
            else if (underlyingType == typeof(System.UInt16))
            {
                foreach (ushort value in values)
                {
                    MyCommonDebugUtils.AssertRelease(list[value] != null);
                }
            }
            else if (underlyingType == typeof(System.Int32))
            {
                foreach (int value in values)
                {
                    MyCommonDebugUtils.AssertRelease(list[value] != null);
                }
            }
            else
            {
                //  Unhandled underlying type - probably "long"
                throw new MyMwcExceptionApplicationShouldNotGetHere();
            }            
        }
    }

}