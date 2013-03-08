using System;
using MinerWarsMath.Graphics;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.CommonLIB.AppCode.Utils;
using SysUtils.Utils;

using SysUtils;
using KeenSoftwareHouse.Library.Extensions;
using System.Collections.Generic;
using System.IO;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.Import;

namespace MinerWars.AppCode.Game.Models
{
    public enum MyModelsEnum : int
    {
        MissileLauncher,
        Autocannon_Barrel,
        Autocannon_Base,
        Liberator,
        Enforcer,
        Kammler,
        Gettysburg,
        Virginia,
        Baer,
        Hewer,
        Razorclaw,
        Greiser,
        Tracer,
        EAC02_Cockpit,
        EAC02_Cockpit_glass,
        EAC03_Cockpit,
        EAC03_Cockpit_glass,
        EAC04_Cockpit,
        EAC04_Cockpit_glass,
        EAC05_Cockpit,
        EAC05_Cockpit_glass,
        OmniCorp01_Cockpit,
        OmniCorp01_Cockpit_glass,
        OmniCorp03_Cockpit,
        OmniCorp03_Cockpit_glass,
        OmniCorp04_Cockpit,
        OmniCorp04_Cockpit_glass,
        OmniCorp_EAC01_Cockpit,
        OmniCorp_EAC01_Cockpit_glass,
        Cockpit_CN_03,
        Cockpit_CN_03_glass,
        Cockpit_SS_04,
        Cockpit_SS_04_glass,
        Cockpit_Razorclaw,
        Cockpit_Razorclaw_glass,
        //AnnaV,
        Kai,
        Kai_COL,
        Kai_LOD1,
        Missile,
        ExplosionDebrisVoxel,
        HarvestingTube,
        Debris1, //Steel scrap
        Debris2, //Cylindrical tripod
        Debris3, //Wing scrap
        Debris4, //steel gun scrap
        Debris5, //ship wing ventilation scrap
        Debris6, //wired parts
        Debris7, //steel solid part
        Debris8, //fat wires part
        Debris9, //box cover 
        Debris10,//square steel plate
        Debris11,//small tripod
        Debris12,//exploded barrel
        Debris13,//exploded tube
        Debris14,//steel turbine or what
        Debris15,//steel ribs
        Debris16,//satelitte
        Debris17,//chamfer cover box
        Debris18,//Exploded pile
        Debris19,//reflector or what
        Debris20,//steel arm
        Debris21,//old box
        Debris22,//box with cables
        Debris23,//lw debris
        Debris24,//steel corner part
        Debris25,//exploded dose
        Debris26,//exploded tube
        Debris27,//wired connected parts
        Debris28,//rocket abandoned
        Debris29,//circular tripod
        Debris30,//christmas tree tripod
        Debris31,//hook part
        Debris32_pilot,
        cistern,
        pipe_bundle,
        Standard_Container_1,
        Standard_Container_2,
        Standard_Container_3,
        Standard_Container_4,
        UtilityVehicle_1,
        DebrisField,
        HarvestingHead,
        Drill_Base,
        Drill_Gear1,
        Drill_Gear2,
        Drill_Gear3,
        LaserDrill,
        NuclearDrill,
        NuclearDrillHead,
        PressureDrill,
        SawDrill,
        ThermalDrill,
        ThermalDrillHead,
        MotherShipSaya,
        MotherShipSaya_COL,
        MotherShipSaya_LOD1,

        //Small ships
        SmallShip_Doon,
        SmallShip_Doon_LOD1,
        SmallShip_Jacknife,
        SmallShip_Jacknife_LOD1,
        SmallShip_Hammer,
        SmallShip_Hammer_LOD1,
        SmallShip_ORG,
        SmallShip_ORG_LOD1,
        SmallShip_ORG_NoPaint,
        SmallShip_ORG_NoPaint_LOD1,
        SmallShip_YG_Closed,
        SmallShip_YG_Closed_LOD1,
        SmallShip_Hawk,
        SmallShip_Hawk_LOD1,
        SmallShip_Phoenix,
        SmallShip_Phoenix_LOD1,
        SmallShip_Leviathan,
        SmallShip_Leviathan_LOD1,
        SmallShip_Rockheater,
        SmallShip_Rockheater_LOD1,
        SmallShip_SteelHead,
        SmallShip_SteelHead_LOD1,
        SmallShip_Talon,
        SmallShip_Talon_LOD1,
        SmallShip_Stanislav,
        SmallShip_Stanislav_LOD1,
        Liberator_LOD1,
        Enforcer_LOD1,
        Kammler_LOD1,
        Gettysburg_LOD1,
        Virginia_LOD1,
        Baer_LOD1,
        Hewer_LOD1,
        Razorclaw_LOD1,
        Greiser_LOD1,
        Tracer_LOD1,

        // Drones
        DroneCN,
        DroneSS,
        DroneUS,

        // Largeship weapons
        LargeShipMachineGunBarrel,
        LargeShipMachineGunBase,
        LargeShipMachineGunBase_COL,
        LargeShipMissileLauncher9Barrel,
        LargeShipMissileLauncher9Base,
        LargeShipMissileLauncher9Base_COL,
        LargeShipMissileLauncher4Barrel,
        LargeShipMissileLauncher4Base,
        LargeShipMissileLauncher4Base_COL,
        LargeShipMissileLauncher6Barrel,
        LargeShipMissileLauncher6Base,
        LargeShipMissileLauncher6Base_COL,
        LargeShipCiwsBarrel,
        LargeShipCiwsBase,
        LargeShipCiwsBase_COL,
        LargeShipAutocannonBarrel,
        LargeShipAutocannonBase,
        LargeShipAutocannonBase_COL,

        StaticAsteroid10m_A_LOD0,
        StaticAsteroid10m_A_LOD1,
        StaticAsteroid10m_A_LOD2,
        StaticAsteroid20m_A_LOD0,
        StaticAsteroid20m_A_LOD1,
        StaticAsteroid20m_A_LOD2,
        StaticAsteroid30m_A_LOD0,
        StaticAsteroid30m_A_LOD1,
        StaticAsteroid30m_A_LOD2,
        StaticAsteroid50m_A_LOD0,
        StaticAsteroid50m_A_LOD1,
        StaticAsteroid50m_A_LOD2,
        StaticAsteroid100m_A_LOD0,
        StaticAsteroid100m_A_LOD1,
        StaticAsteroid100m_A_LOD2,
        StaticAsteroid300m_A_LOD0,
        StaticAsteroid300m_A_LOD1,
        StaticAsteroid300m_A_LOD2,
        StaticAsteroid500m_A_LOD0,
        StaticAsteroid500m_A_LOD1,
        StaticAsteroid500m_A_LOD2,
        StaticAsteroid1000m_A_LOD0,
        StaticAsteroid1000m_A_LOD1,
        StaticAsteroid1000m_A_LOD2,
        StaticAsteroid2000m_A_LOD0,
        StaticAsteroid2000m_A_LOD1,
        StaticAsteroid2000m_A_LOD2,
        StaticAsteroid5000m_A_LOD0,
        StaticAsteroid5000m_A_LOD1,
        StaticAsteroid5000m_A_LOD2,
        StaticAsteroid10000m_A_LOD0,
        StaticAsteroid10000m_A_LOD1,
        StaticAsteroid10000m_A_LOD2,
        /*  Removed
        StaticAsteroid40000m_A_LOD0,
        StaticAsteroid40000m_A_LOD1,
        StaticAsteroid40000m_A_LOD2,
          */
        StaticAsteroid10m_B_LOD0,
        StaticAsteroid10m_B_LOD1,
        StaticAsteroid10m_B_LOD2,
        StaticAsteroid20m_B_LOD0,
        StaticAsteroid20m_B_LOD1,
        StaticAsteroid20m_B_LOD2,
        StaticAsteroid30m_B_LOD0,
        StaticAsteroid30m_B_LOD1,
        StaticAsteroid30m_B_LOD2,
        StaticAsteroid50m_B_LOD0,
        StaticAsteroid50m_B_LOD1,
        StaticAsteroid50m_B_LOD2,
        StaticAsteroid100m_B_LOD0,
        StaticAsteroid100m_B_LOD1,
        StaticAsteroid100m_B_LOD2,
        StaticAsteroid300m_B_LOD0,
        StaticAsteroid300m_B_LOD1,
        StaticAsteroid300m_B_LOD2,
        StaticAsteroid500m_B_LOD0,
        StaticAsteroid500m_B_LOD1,
        StaticAsteroid500m_B_LOD2,
        StaticAsteroid1000m_B_LOD0,
        StaticAsteroid1000m_B_LOD1,
        StaticAsteroid1000m_B_LOD2,
        StaticAsteroid2000m_B_LOD0,
        StaticAsteroid2000m_B_LOD1,
        StaticAsteroid2000m_B_LOD2,
        StaticAsteroid5000m_B_LOD0,
        StaticAsteroid5000m_B_LOD1,
        StaticAsteroid5000m_B_LOD2,
        StaticAsteroid10000m_B_LOD0,
        StaticAsteroid10000m_B_LOD1,
        StaticAsteroid10000m_B_LOD2,
                 /*    Removed
        StaticAsteroid10m_C_LOD0,
        StaticAsteroid10m_C_LOD1,
        StaticAsteroid20m_C_LOD0,
        StaticAsteroid20m_C_LOD1,
        StaticAsteroid30m_C_LOD0,
        StaticAsteroid30m_C_LOD1,
        StaticAsteroid50m_C_LOD0,
        StaticAsteroid50m_C_LOD1,
        StaticAsteroid100m_C_LOD0,
        StaticAsteroid100m_C_LOD1,
        StaticAsteroid300m_C_LOD0,
        StaticAsteroid300m_C_LOD1,
        StaticAsteroid500m_C_LOD0,
        StaticAsteroid500m_C_LOD1,
        StaticAsteroid1000m_C_LOD0,
        StaticAsteroid1000m_C_LOD1,
        StaticAsteroid2000m_C_LOD0,
        StaticAsteroid2000m_C_LOD1,
        StaticAsteroid5000m_C_LOD0,
        StaticAsteroid5000m_C_LOD1,
        StaticAsteroid10000m_C_LOD0,
        StaticAsteroid10000m_C_LOD1,

        StaticAsteroid10m_D_LOD0,
        StaticAsteroid10m_D_LOD1,
        StaticAsteroid20m_D_LOD0,
        StaticAsteroid20m_D_LOD1,
        StaticAsteroid30m_D_LOD0,
        StaticAsteroid30m_D_LOD1,
        StaticAsteroid50m_D_LOD0,
        StaticAsteroid50m_D_LOD1,
        StaticAsteroid100m_D_LOD0,
        StaticAsteroid100m_D_LOD1,
        StaticAsteroid300m_D_LOD0,
        StaticAsteroid300m_D_LOD1,
        StaticAsteroid500m_D_LOD0,
        StaticAsteroid500m_D_LOD1,
        StaticAsteroid1000m_D_LOD0,
        StaticAsteroid1000m_D_LOD1,
        StaticAsteroid2000m_D_LOD0,
        StaticAsteroid2000m_D_LOD1,
        StaticAsteroid5000m_D_LOD0,
        StaticAsteroid5000m_D_LOD1,
        StaticAsteroid10000m_D_LOD0,
        StaticAsteroid10000m_D_LOD1,

        StaticAsteroid10m_E_LOD0,
        StaticAsteroid10m_E_LOD1,
        StaticAsteroid20m_E_LOD0,
        StaticAsteroid20m_E_LOD1,
        StaticAsteroid30m_E_LOD0,
        StaticAsteroid30m_E_LOD1,
        StaticAsteroid50m_E_LOD0,
        StaticAsteroid50m_E_LOD1,
        StaticAsteroid100m_E_LOD0,
        StaticAsteroid100m_E_LOD1,
        StaticAsteroid300m_E_LOD0,
        StaticAsteroid300m_E_LOD1,
        StaticAsteroid500m_E_LOD0,
        StaticAsteroid500m_E_LOD1,
        StaticAsteroid1000m_E_LOD0,
        StaticAsteroid1000m_E_LOD1,
        StaticAsteroid2000m_E_LOD0,
        StaticAsteroid2000m_E_LOD1,
        StaticAsteroid5000m_E_LOD0,
        StaticAsteroid5000m_E_LOD1,
        StaticAsteroid10000m_E_LOD0,
        StaticAsteroid10000m_E_LOD1,
             */
        UniversalLauncher,
        MineBasic,
        MineSmart,
        FlashBomb,
        IlluminatingShell,
        DecoyFlare,
        SphereExplosive,
        SmokeBomb,
        AsteroidKiller,
        DirectionalExplosive,
        TimeBomb,
        RemoteBomb,
        GravityBomb,
        Hologram,
        RemoteCamera,
        Rifle,
        Sniper,
        MachineGun,
        Shotgun,
        MinerShip_Generic_CockpitInterior,
        MinerShip_Generic_CockpitGlass,
        Ardant,
        Ardant_COL,
        Ardant_LOD1,
        //PREFAB MODULES
        GizmoTranslation,
        GizmoRotation,
        p430_a01_passage_10m,
        p430_a01_passage_10m_LOD1,
        p430_a01_passage_10m_COL,
        p430_a02_passage_40m,
        p430_a02_passage_40m_LOD1,
        p430_a02_passage_40m_COL,
        p424_a01_pipe_base,
        p424_a01_pipe_base_COL,
        p423_a01_pipe_junction,
        p422_a01_pipe_turn_90,
        p421_a01_pipe_straight_80m,
        p421_a02_pipe_straight_40m,
        p421_a03_pipe_straight_10m,
        p413_g01_junction_6axes,
        p413_g01_junction_6axes_COL,
        p414_g02_entrance_60m,
        p414_g02_entrance_60m_LOD1,
        p414_g02_entrance_60m_COL,
        p410_g01_turn_90_right_0m,
        p410_g01_turn_90_right_0m_LOD1,
        p410_g01_turn_90_right_0m_COL,
        p411_g01_straight_1,
        p411_g02_straight_2,
        p411_g03_straight_3,
        p411_g04_straight_4,
        //p414_f02_entrance_60m,
        p412_f21_turn_s_up,
        p412_f21_turn_s_up_COL,
        p412_f22_turn_s_left,
        p412_f22_turn_s_left_COL,
        p412_f23_turn_s_right,
        p412_f23_turn_s_right_COL,
        p412_f24_turn_s_down,
        p412_f24_turn_s_down_COL,
        p412_f01_turn_90_up_230m,
        p412_f02_turn_90_left_230m,
        p412_f02_turn_90_left_230m_COL,
        p412_f02_turn_90_left_230m_LOD1,
        p412_f03_turn_90_right_230m,
        p412_f03_turn_90_right_230m_LOD1,
        p412_f03_turn_90_right_230m_COL,
        p412_f04_turn_90_down_230m,
        p412_f04_turn_90_down_230m_COL,
        p411_f01_straight_1,
        p411_f02_straight_2,
        p411_f03_straight_3,
        p414_e01_entrance_60m,
        p414_e01_entrance_60m_LOD1,
        p414_e01_entrance_60m_COL,
        p411_e01_straight_1,
        p411_e02_straight_2,
        p411_e03_straight_3,
        p411_e04_straight_4,
        p411_e05_straight_5,
        p411_e01_straight_1_COL,
        p411_e02_straight_2_COL,
        p411_e03_straight_3_COL,
        p411_e04_straight_4_COL,
        p411_e05_straight_5_COL,
        p415_d01_doorcase,
        p415_d01_doorcase_COL,
        p415_d02_door1,
        p415_d03_door2_a,
        p415_d03_door2_a_COL,
        p415_d03_door2_b,
        p415_d03_door2_b_COL,
        p413_d01_junction_t_horizontal,
        p413_d03_junction_x_horizontal,
        p413_d03_junction_x_horizontal_COL,
        p414_d01_entrance_60m,
        p414_d01_entrance_60m_COL,
        p411_d01_straight_10m,
        p411_d01_straight_10m_LOD1,
        p411_d01_straight_10m_COL,
        p411_d02_straight_40m_with_hole,
        p411_d03_straight_60m,
        p411_d04_straight_120m,
        p411_d05_straight_180m,
        p415_c01_doorcase,
        p415_c02_door1_right,
        p415_c02_door1_right_COL,
        p415_c02_door1_left,
        p415_c02_door1_left_COL,
        p415_c03_door2_a_left,
        p415_c03_door2_a_right,
        p415_c03_door2_b_left,
        p415_c03_door2_b_right,
        p415_c02_door1,
        p415_c03_door2_a,
        p415_c03_door2_b,
        p415_c04_door3,
        p413_c01_junction_t_horizontal,
        p413_c01_junction_t_horizontal_LOD1,
        p413_c01_junction_x_horizontal,
        p413_c01_junction_x_horizontal_COL,
        p413_c01_junction_x_horizontal_LOD1,
        p414_c01_entrance_60m,
        p414_c01_entrance_60m_COL,
        p411_c01_straight_10m,
        p411_c01_straight_10m_COL,
        p411_c02_straight_40m_with_hole,
        p411_c02_straight_40m_with_hole_COL,
        p411_c02_straight_40m_with_hole_LOD1,
        p411_c03_straight_60m,
        p411_c03_straight_60m_LOD1,
        p411_c03_straight_60m_COL,
        p411_c04_straight_120m,
        p411_c04_straight_120m_LOD1,
        p411_c04_straight_120m_COL,
        p411_c05_straight_180m,
        p415_b01_doorcase,
        p415_b01_doorcase_LOD1,
        p415_b01_doorcase_COL,
        //p415_b02_door,
        p413_b01_junction_t_horizontal,
        p413_b01_junction_t_horizontal_LOD1,
        p413_b01_junction_t_horizontal_COL,
        p413_b02_junction_t_vertical,
        p413_b02_junction_t_vertical_LOD1,
        p413_b02_junction_t_vertical_COL,
        p414_b02_entrance_60m,
        p412_b21_turn_s_up,
        p412_b21_turn_s_up_COL,
        p412_b22_turn_s_left,
        p412_b23_turn_s_right,
        p412_b23_turn_s_right_COL,
        p412_b24_turn_s_down,
        p412_b11_turn_90_up_160m,
        p412_b12_turn_90_left_160m,
        p412_b13_turn_90_right_160m,
        p412_b13_turn_90_right_160m_COL,
        p412_b14_turn_90_down_160m,
        p412_b14_turn_90_down_160m_COL,
        p412_b01_turn_90_up_80m,
        p412_b01_turn_90_up_80m_COL,
        p412_b02_turn_90_left_80m,
        p412_b02_turn_90_left_80m_COL,
        p412_b03_turn_90_right_80m,
        p412_b03_turn_90_right_80m_COL,
        p412_b04_turn_90_down_80m,
        p412_b04_turn_90_down_80m_COL,
        p411_b01_straight_10m,
        p411_b01_straight_10m_LOD1,
        p411_b01_straight_10m_COL,
        p411_b02_straight_30m_yellow,
        p411_b02_straight_30m_yellow_LOD1,
        p411_b02_straight_30m_yellow_COL,
        p411_b03_straight_320m,
        p411_b03_straight_320m_LOD1,
        p411_b03_straight_320m_COL,
        p411_b04_straight_80m_with_side_grates,
        p411_b04_straight_80m_with_side_grates_LOD1,
        p411_b04_straight_80m_with_side_grates_COL,
        p411_b05_straight_80m_with_side_open,
        p411_b05_straight_80m_with_side_open_LOD1,
        p411_b05_straight_80m_with_side_open_COL,
        p411_b06_straight_180m_concrete,
        p411_b06_straight_180m_concrete_LOD1,
        p411_b06_straight_180m_concrete_COL,
        p411_b06_straight_200m,
        p411_b06_straight_200m_LOD1,
        p411_b06_straight_200m_COL,
        p411_b07_straight_180m_blue,
        p411_b07_straight_180m_blue_LOD1,
        p411_b07_straight_180m_blue_COL,
        p411_b09_straight_30m_gray,
        p411_b09_straight_30m_gray_LOD1,
        p411_b09_straight_30m_gray_COL,
        p411_b11_straight_220m,
        p411_b11_straight_220m_LOD1,
        p411_b11_straight_220m_COL,
        p411_b12_straight_160m_dark_metal,
        p411_b12_straight_160m_dark_metal_LOD1,
        p411_b12_straight_160m_dark_metal_COL,
        p411_b13_straight_100m_tube_inside,
        p411_b13_straight_100m_tube_inside_LOD1,
        p411_b13_straight_100m_tube_inside_COL,
        p415_a01_doorcase,
        p415_a01_doorcase_LOD1,
        p415_a01_doorcase_COL,
        p415_a02_door,
        p415_a02_door_left,
        p415_a02_door_left_COL,
        p415_a02_door_right,
        p415_a02_door_right_COL,
        p413_a01_junction_t_horizontal,
        p413_a01_junction_t_horizontal_LOD1,
        p413_a01_junction_t_horizontal_COL,
        p413_a02_junction_t_vertical,
        p413_a02_junction_t_vertical_LOD1,
        p413_a02_junction_t_vertical_COL,
        //p414_a01_entrance_30m,
        p414_a02_entrance_60m,
        p414_a02_entrance_60m_LOD1,
        p414_a02_entrance_60m_COL,
        p412_a21_turn_s_up,
        p412_a21_turn_s_up_LOD1,
        p412_a21_turn_s_up_COL,
        p412_a22_turn_s_left,
        p412_a22_turn_s_left_LOD1,
        p412_a22_turn_s_left_COL,
        p412_a23_turn_s_right,
        p412_a23_turn_s_right_LOD1,
        p412_a23_turn_s_right_COL,
        p412_a24_turn_s_down,
        p412_a24_turn_s_down_LOD1,
        p412_a24_turn_s_down_COL,
        p412_a11_turn_90_up_160m,
        p412_a11_turn_90_up_160m_LOD1,
        p412_a11_turn_90_up_160m_COL,
        p412_a12_turn_90_left_160m,
        p412_a12_turn_90_left_160m_LOD1,
        p412_a12_turn_90_left_160m_COL,
        p412_a13_turn_90_right_160m,
        p412_a13_turn_90_right_160m_LOD1,
        p412_a13_turn_90_right_160m_COL,
        p412_a14_turn_90_down_160m,
        p412_a14_turn_90_down_160m_LOD1,
        p412_a14_turn_90_down_160m_COL,
        p412_a01_turn_90_up_80m,
        p412_a01_turn_90_up_80m_LOD1,
        p412_a01_turn_90_up_80m_COL,
        p412_a02_turn_90_left_80m,
        p412_a02_turn_90_left_80m_LOD1,
        p412_a02_turn_90_left_80m_COL,
        p412_a03_turn_90_right_80m,
        p412_a03_turn_90_right_80m_LOD1,
        p412_a03_turn_90_right_80m_COL,
        p412_a04_turn_90_down_80m,
        p412_a04_turn_90_down_80m_LOD1,
        p412_a04_turn_90_down_80m_COL,
        p411_a01_straight_10m,
        p411_a01_straight_10m_LOD1,
        p411_a01_straight_10m_COL,
        p411_a02_straight_60m_with_hole,
        p411_a02_straight_60m_with_hole_LOD1,
        p411_a02_straight_60m_with_hole_COL,
        p411_a03_straight_120m,
        p411_a03_straight_120m_LOD1,
        p411_a03_straight_120m_COL,
        p411_a04_straight_80m,
        p411_a04_straight_80m_LOD1,
        p411_a04_straight_80m_COL,
        p411_a05_straight_80m_with_extension,
        p411_a05_straight_80m_with_extension_LOD1,
        p411_a05_straight_80m_with_extension_COL,
        p382_e01_bridge5,
        p382_e01_bridge5_LOD1,
        p382_d01_bridge4,
        p382_d01_bridge4_COL,
        p382_c01_bridge3,
        p382_c01_bridge3_COL,
        p382_b01_bridge2,
        p382_a01_bridge1,
        p382_a01_bridge1_COL,
        p382_a01_bridge1_LOD1,
        p381_c01_building3,
        p381_c01_building3_COL,
        p381_b01_building2,
        p381_b01_building2_COL,
        p381_a01_building1,
        p381_a01_building1_COL,
        //p361_a01_small_hangar,
        //p361_a01_small_hangar_COL,
        p362_a01_short_distance_antenna,
        p362_a01_short_distance_antenna_COL,
        p361_a01_long_distance_antenna,
        p361_a01_long_distance_antenna_COL,
        p351_a01_weapon_mount,
        p351_a01_weapon_mount_COL,
        p345_a01_refinery,
        p345_a01_refinery_COL,
        p344_a01_container_arm_filled,
        p344_a01_container_arm_filled_LOD1,
        p344_a01_container_arm_filled_COL,
        p344_a02_container_arm_empty,
        p344_a02_container_arm_empty_COL,
        p344_a02_container_arm_empty_LOD1,
        p343_a01_ore_storage,
        p343_a01_ore_storage_LOD1,
        p343_a01_ore_storage_COL,
        p342_a01_loading_bay,
        p342_a01_loading_bay_LOD1,
        p342_a01_loading_bay_COL,
        p341_b01_open_dock_variation1,
        p341_b01_open_dock_variation1_LOD1,
        p341_b01_open_dock_variation1_COL,
        p341_b02_open_dock_variation2,
        p341_b02_open_dock_variation2_LOD1,
        p341_b02_open_dock_variation2_COL,
        p341_a01_open_dock_variation1,
        p341_a01_open_dock_variation1_LOD1,
        p341_a01_open_dock_variation1_COL,
        p341_a01_open_dock_variation1_doorleft,
        p341_a01_open_dock_variation1_doorright,
        p341_a02_open_dock_variation2,
        p341_a02_open_dock_variation2_LOD1,
        p341_a02_open_dock_variation2_COL,
        p333_a01_hydroponic_building,
        p333_a01_hydroponic_building_COL,
        p332_a01_oxygen_storage,
        p332_a01_oxygen_storage_LOD1,
        p332_a01_oxygen_storage_COL,
        p331_a01_oxygen_generator,
        p331_a01_oxygen_generator_COL,
        p324b01_fuel_storage_b,
        p324a01_fuel_storage_a,
        p323a01_fuel_generator,
        p323a01_fuel_generator_COL,
        p322a01_battery,
        p322a01_battery_COL,
        p321c01_inertia_generator,
        p321c01_inertia_generator_COL,
        p321c01_inertia_generator_center,
        p321c01_inertia_generator_center_COL,
        p321c01_inertia_generator_center_LOD1,
        p321c01_inertia_generator_LOD1,
        p321b01_nuclear_reactor,
        //p321a01_solar_panel,
        //p321a01_solar_panel_LOD1,
        //p321a01_solar_panel_COL,
        p312a01_short_term_thruster_latitude,
        p312a01_short_term_thruster_latitude_COL,
        p312a02_short_term_thruster_lateral,
        p312a02_short_term_thruster_lateral_COL,
        p311a01_long_term_thruster,
        p231a01_armor,
        p231a02_armor,
        p231a03_armor,
        p231a04_armor,
        p231a05_armor,
        p231a06_armor,
        p231a07_armor,
        p231a08_armor,
        p231a09_armor,
        p231a10_armor,
        p231a11_armor,
        p231a12_armor,
        p231a13_armor,
        p231a14_armor,
        p231a15_armor,
        p231a16_armor,
        p231a17_armor,
        p231a18_armor,

        p221a01_chamber_v1,
        p221a01_chamber_v1_LOD1,
        p221a01_chamber_v1_COL,

        p221b01_chamber_v1,
        p221b01_chamber_v1_LOD1,

        p221c01_chamber_v1,
        p221c01_chamber_v1_LOD1,
        p221c01_chamber_v1_COL,

        p221d01_chamber_v1,
        p221d01_chamber_v1_LOD1,
        p221d01_chamber_v1_COL,

        p221e01_chamber_v1,
        p221e01_chamber_v1_LOD1,
        p221e01_chamber_v1_COL,

        p221f01_chamber_v1,
        p221f01_chamber_v1_LOD1,
        p221f01_chamber_v1_COL,

        p221g01_chamber_v1,
        p221g01_chamber_v1_LOD1,
        p221g01_chamber_v1_COL,

        p221h01_chamber_v1,
        p221h01_chamber_v1_LOD1,
        p221h01_chamber_v1_COL,

        p221j01_chamber_v1,
        p221j01_chamber_v1_LOD1,
        p221j01_chamber_v1_COL,

        p221k01_chamber_v1,
        p221k01_chamber_v1_LOD1,




        p211h01_panel_535mx130m,
        p211h01_panel_535mx130m_COL,
        p211h01_panel_535mx130m_LOD1,
        p211g01_panel_120mx60m,
        p211g02_panel_60mx60m,
        p211g03_panel_60mx30m,
        p211g03_panel_60mx30m_LOD1,
        p211g03_panel_60mx30m_COL,
        //p211g03_panel_60mx30m_COL,
        p211f01_panel_120mx60m,
        p211f01_panel_120mx60m_LOD1,
        p211f01_panel_120mx60m_COL,
        p211f02_panel_60mx60m,
        p211f02_panel_60mx60m_LOD1,
        p211f02_panel_60mx60m_COL,
        p211f03_panel_60mx30m,
        p211f03_panel_60mx30m_COL,
        p211e01_panel_120mx60m,
        p211e01_panel_120mx60m_LOD1,
        p211e01_panel_120mx60m_COL,
        p211e02_panel_60mx60m,
        p211e02_panel_60mx60m_LOD1,
        p211e02_panel_60mx60m_COL,
        p211e03_panel_60mx30m,
        p211e03_panel_60mx30m_LOD1,
        p211e03_panel_60mx30m_COL,
        p211d01_panel_120mx60m,
        p211d01_panel_120mx60m_LOD1,
        p211d01_panel_120mx60m_COL,
        p211d02_panel_60mx60m,
        p211d03_panel_60mx30m,
        p211d03_panel_60mx30m_LOD1,
        p211d03_panel_60mx30m_COL,
        p211c01_panel_120mx60m,
        p211c02_panel_60mx60m,
        p211c02_panel_60mx60m_COL,
        p211c03_panel_60mx30m,
        p211b01_panel_120mx60m,
        p211b01_panel_120mx60m_COL,
        p211b02_panel_60mx60m,
        p211b02_panel_60mx60m_COL,
        p211b03_panel_60mx30m,
        p211b03_panel_60mx30m_COL,
        p211a01_panel_120mx60m,
        p211a01_panel_120mx60m_COL,
        p211a02_panel_60mx60m,
        p211a02_panel_60mx60m_COL,
        p211a03_panel_60mx30m,
        p211a03_panel_60mx30m_COL,
        p142b01_cage_empty,
        p142b02_cage_halfcut,
        p142b03_cage_with_corners,
        p142b03_cage_with_corners_LOD1,
        p142b03_cage_with_corners_COL,
        p142b11_cage_pillar,
        p142b11_cage_pillar_COL,
        p142b12_cage_edge,
        p142a01_cage_empty,
        p142a01_cage_empty_COL,
        p142a02_cage_halfcut,
        p142a02_cage_halfcut_COL,
        p142a03_cage_with_corners,
        p142a03_cage_with_corners_COL,
        p142a11_cage_pillar,
        p142a12_cage_edge,
        p141b01_thick_frame_straight_10m,
        p141b02_thick_frame_straight_60m,
        p141b02_thick_frame_straight_60m_COL,
        p141b11_thick_frame_edge,
        p141b12_thick_frame_corner,
        p141b31_thick_frame_joint,
        p141b31_thick_frame_joint_COL,
        p141a01_thick_frame_straight_10m,
        p141a01_thick_frame_straight_10m_COL,
        p141a02_thick_frame_straight_60m,
        p141a02_thick_frame_straight_60m_COL,
        p141a11_thick_frame_edge,
        p141a11_thick_frame_edge_COL,
        p141a12_thick_frame_corner,
        p141a31_thick_frame_joint,
        p141a31_thick_frame_joint_COL,
        p130j01_j_straight_30m,
        p130j01_j_straight_30m_COL,
        p130j02_j_straight_10m,
        p130j02_j_straight_10m_COL,
        p130i01_i_straight_30m,
        p130i02_i_straight_10m,
        p130i02_i_straight_10m_COL,
        p130h01_h_straight_30m,
        p130h01_h_straight_30m_COL,
        p130h02_h_straight_10m,
        p130h02_h_straight_10m_COL,
        //p130g01_g_straight_30m,
        //p130g02_g_straight_10m,
        //p130f01_f_straight_30m,
        //p130f02_f_straight_10m,
        //p130e01_e_straight_30m,
        //p130e02_e_straight_10m,
        //p130d01_d_straight_30m,
        ////p130d02_d_straight_10m,
        //p130c01_c_straight_30m,
        //p130c02_c_straight_10m,
        //p130b01_b_straight_30m,
        //p130b02_b_straight_10m,
        //p130a01_a_straight_30m,
        //p130a02_a_straight_10m,
        p120d01_d_straight_10m,
        p120d01_d_straight_10m_COL,
        p120d02_d_straight_40m,
        p120d02_d_straight_40m_COL,
        p120c01_c_straight_10m,
        p120c01_c_straight_10m_COL,
        p120c02_c_straight_40m,
        p120c02_c_straight_40m_COL,
        p120b01_b_straight_10m,
        p120b01_b_straight_10m_COL,
        p120b02_b_straight_40m,
        p120b02_b_straight_40m_COL,
        p120a01_strong_lattice_straight_10m,
        p120a01_strong_lattice_straight_10m_COL,
        p120a02_strong_lattice_straight_60m,
        p120a03_strong_lattice_straight_120m,
        p120a21_strong_lattice_junction_t_strong,
        p120a21_strong_lattice_junction_t_strong_COL,
        p120a22_strong_lattice_junction_t_weak,
        p120a23_strong_lattice_junction_t_rotated,
        p120a23_strong_lattice_junction_t_rotated_COL,
        p120a51_strong_to_weak_lattice_2to1,
        p120a51_strong_to_weak_lattice_2to1_COL,
        p120a52_strong_to_weak_lattice_1to2,
        p120a61_weak_lattice_junction_t_rotated,
        p120a61_weak_lattice_junction_t_rotated_COL,
        p110b01_lattice_beam_straight_10m,
        p110b02_lattice_beam_straight_30m,
        p110b02_lattice_beam_straight_30m_COL,
        p110b03_lattice_beam_straight_60m,
        p110b04_lattice_beam_straight_60m_with_panels,
        p110b21_lattice_beam_junction_t_strong,
        p110b22_lattice_beam_junction_t_weak,
        p110b31_lattice_beam_joint_horizontal,
        p110b31_lattice_beam_joint_horizontal_COL,
        p110b32_lattice_beam_joint_vertical,
        p110b32_lattice_beam_joint_vertical_COL,
        p110a01_solid_beam_straight_10m,
        p110a01_solid_beam_straight_10m_COL,
        p110a02_solid_beam_straight_20m,
        p110a02_solid_beam_straight_20m_COL,
        p110a03_solid_beam_straight_40m_with_hole,
        p110a03_solid_beam_straight_40m_with_hole_COL,
        p110a04_solid_beam_straight_40m_lattice,
        p110a04_solid_beam_straight_40m_lattice_LOD1,
        p110a04_solid_beam_straight_40m_lattice_COL,
        p110a05_solid_beam_straight_80m,
        p110a11_solid_beam_junction_x_strong,
        p110a11_solid_beam_junction_x_strong_LOD1,
        p110a11_solid_beam_junction_x_strong_COL,
        p110a12_solid_beam_junction_x_weak,
        p110a13_solid_beam_junction_x_rotated,
        p110a13_solid_beam_junction_x_rotated_LOD1,
        p110a13_solid_beam_junction_x_rotated_COL,
        p110a21_solid_beam_junction_t_strong,
        p110a21_solid_beam_junction_t_strong_COL,
        p110a22_solid_beam_junction_t_weak,
        p110a23_solid_beam_junction_t_rotated,
        p110a23_solid_beam_junction_t_rotated_COL,
        p110a31_solid_beam_joint_horizontal,
        p110a31_solid_beam_joint_horizontal_COL,
        p110a32_solid_beam_joint_vertical,
        p110a32_solid_beam_joint_vertical_COL,
        p110a33_solid_beam_joint_longitudinal,
        p110a33_solid_beam_joint_longitudinal_COL,
        p110a41_solid_beam_superjoint,
        p110a41_solid_beam_superjoint_COL,

        //Debug
        plane_10_50,
        plane_50_20,
        plane_100_10,
        plane_100_50,
        plane_128_70,
        plane_150_50,
        plane_255_70,
        plane_300_70,
        plane_800_10,
        plane_800_70,
        sphere_smooth,

        //Simple object draw
        BoxLowRes,
        BoxHiRes,
        Sphere,
        Sphere_low,
        Cone,
        Hemisphere,
        Hemisphere_low,
        Capsule,

        //Lights
        default_light_0,
        default_light_0_COL,
        p521_a01_light1,
        p521_a01_light1_COL,
        p521_a02_light2,
        p521_a02_light2_COL,
        p521_a03_light3,
        p521_a03_light3_COL,
        p521_a04_light4,
        p521_a04_light4_COL,

        //particles prefab
        default_particlesprefab_0,
        p551_a01_particles,
        p551_b01_particles,
        p551_c01_particles,
        p551_d01_particles,

        //sound prefab
        default_soundprefab_0,
        p561_a01_sound,
        p561_b01_sound,
        p561_c01_sound,
        p561_d01_sound,

        // billboard prefab:
        p511_a01_billboard,
        p511_a01_billboard_COL,
        p511_a01_billboard_LOD1,
        p511_a02_billboard,
        p511_a02_billboard_COL,
        p511_a02_billboard_LOD1,
        p511_a03_billboard,
        p511_a03_billboard_COL,
        p511_a03_billboard_LOD1,
        p511_a04_billboard,
        p511_a04_billboard_COL,
        p511_a04_billboard_LOD1,
        p511_a05_billboard,
        p511_a05_billboard_COL,
        p511_a05_billboard_LOD1,
        p511_a06_billboard,
        p511_a06_billboard_COL,
        p511_a06_billboard_LOD1,
        p511_a07_billboard,
        p511_a07_billboard_COL,
        p511_a07_billboard_LOD1,
        p511_a08_billboard,
        p511_a08_billboard_COL,
        p511_a08_billboard_LOD1,
        p511_a09_billboard,
        p511_a09_billboard_COL,
        p511_a09_billboard_LOD1,
        p511_a10_billboard,
        p511_a10_billboard_COL,
        p511_a10_billboard_LOD1,
        p511_a11_billboard,
        p511_a11_billboard_COL,
        p511_a11_billboard_LOD1,
        p511_a12_billboard,
        p511_a12_billboard_COL,
        p511_a12_billboard_LOD1,        
        p511_a14_billboard,
        //p511_a14_billboard_COL,
        p511_a14_billboard_LOD1,
        p511_a15_billboard,
        p511_a15_billboard_COL,
        p511_a15_billboard_LOD1,
        p511_a16_billboard,
        p511_a16_billboard_COL,
        p511_a16_billboard_LOD1,

        // sign prefabs:
        p531_a01_sign1,
        p531_a02_sign2,
        p531_a03_sign3,
        p531_a04_sign4,
        p531_a05_sign5,
        p531_a06_sign6,
        p531_a07_sign7,
        p531_a08_sign8,
        p531_a09_sign9,
        p531_a10_sign10,
        p531_a11_sign11,
        p531_a12_sign12,

        p531_c_administrative_area,
        p531_c_armory,
        p531_c_arrow_L,
        p531_c_arrow_R,
        p531_c_arrow_str,
        p531_c_cargo_bay,
        p531_c_command_center,
        p531_c_commercial_area,
        p531_c_communications,
        p531_c_defenses,
        p531_c_docks,
        p531_c_docks_COL,
        p531_c_emergency_exit,
        p531_c_engineering_area,
        p531_c_engineering_area_COL,
        p531_c_exit,
        p531_c_experimental_labs,
        p531_c_foundry,
        p531_c_habitats,
        p531_c_habitats_COL,
        p531_c_hangars,
        p531_c_hangars_COL,
        p531_c_industrial_area,
        p531_c_landing_bay,
        p531_c_maintenance,
        p531_c_maintenance_COL,
        p531_c_military_area,
        p531_c_mines,
        p531_c_ore_processing,
        p531_c_outer_area,
        p531_c_prison,
        p531_c_public_area,
        p531_c_reactor,
        p531_c_reactor_COL,
        p531_c_research,
        p531_c_restricted_area,
        p531_c_security,
        p531_c_sign,
        p531_c_storage,
        p531_c_storage_COL,
        p531_c_technical_area,
        p531_c_trade_port,

        p221a02_chamber_v2,
        p221a02_chamber_v2_LOD1,
        p221a02_chamber_v2_COL,
        p221b02_chamber_v2,
        p221c02_chamber_v2,
        p221c02_chamber_v2_LOD1,
        p221c02_chamber_v2_COL,
        p221d02_chamber_v2,
        p221d02_chamber_v2_LOD1,
        p221d02_chamber_v2_COL,
        p221e02_chamber_v2,

        p221b02_chamber_v2_LOD1,
        p221e02_chamber_v2_LOD1,

        // Prefabs Beams small:
        p130a01_a_straight_10m,
        p130a01_a_straight_10m_COL,
        p130a02_a_straight_30m,
        p130a02_a_straight_30m_LOD1,
        p130a02_a_straight_30m_COL,
        p130b01_b_straight_10m,
        p130b01_b_straight_10m_COL,
        p130b02_b_straight_30m,
        p130b02_b_straight_30m_LOD1,
        p130b02_b_straight_30m_COL,
        p130c01_c_straight_10m,
        p130c01_c_straight_10m_COL,
        p130c02_c_straight_30m,
        p130c02_c_straight_30m_LOD1,
        p130c02_c_straight_30m_COL,
        p130d01_d_straight_10m,
        p130d01_d_straight_10m_COL,
        p130d02_d_straight_30m,
        p130d02_d_straight_30m_LOD1,
        p130d02_d_straight_30m_COL,
        p130e01_e_straight_10m,
        p130e01_e_straight_10m_COL,
        p130e02_e_straight_30m,
        p130e02_e_straight_30m_LOD1,
        p130e02_e_straight_30m_COL,

        // Solar panels:
        p321b01_solar_panel,
        p321b01_solar_panel_LOD1,
        p321b01_solar_panel_COL,
        p321c01_solar_panel,
        p321c01_solar_panel_COL,
        p321c01_solar_panel_LOD1,

        p413_d02_junction_t_vertical,
        p413_d02_junction_t_vertical_LOD1,
        p413_d02_junction_t_vertical_COL,
        p413_d04_junction_x_vertical,
        p413_d04_junction_x_vertical_LOD1,
        p413_d04_junction_x_vertical_COL,

        p412_f11_turn_90_up_230m,
        p412_f11_turn_90_up_230m_COL,
        p412_f11_turn_90_up_230m_LOD1,
        p412_f12_turn_90_left_230m,
        p412_f12_turn_90_left_230m_LOD1,
        p412_f12_turn_90_left_230m_COL,
        p412_f13_turn_90_right_230m,
        p412_f13_turn_90_right_230m_LOD1,
        p412_f13_turn_90_right_230m_COL,
        p412_f14_turn_90_down_230m,
        p412_f14_turn_90_down_230m_LOD1,
        p412_f14_turn_90_down_230m_COL,

        p411_f04_straight_4,
        p411_f05_straight_5,
        p411_g05_straight_5,
        p414_f01_entrance_60m,
        p414_f01_entrance_60m_LOD1,
        p414_f01_entrance_60m_COL,
        p414_g01_entrance_60m,
        p414_g01_entrance_60m_LOD1,
        p414_g01_entrance_60m_COL,

        p571_a01_traffic_sign,
        p571_a01_traffic_sign_COL,
        p571_b01_traffic_sign,
        p571_box01_traffic_sign,
        p571_box01_traffic_sign_COL,
        p571_box02_traffic_sign,
        p571_c01_traffic_sign,
        p571_c01_traffic_sign_COL,
        p571_d01_traffic_sign,
        p571_e01_traffic_sign,
        p571_f01_traffic_sign,
        p571_f01_traffic_sign_COL,
        p571_g01_traffic_sign,
        p571_h01_traffic_sign,
        p571_i01_traffic_sign,
        p571_i01_traffic_sign_COL,
        p571_j01_traffic_sign,
        p571_j01_traffic_sign_COL,
        p571_k01_traffic_sign,
        p571_k01_traffic_sign_COL,
        p571_l01_traffic_sign,
        p571_l01_traffic_sign_COL,

        //Simple object to be replace by custom import
        SimpleObject,
        //AsteroidPrefabTest,
        FoundationFactory,
        FoundationFactory_COL,

        // new prefabs
        p385_a01_temple_900m,
        p385_a01_temple_900m_COL,
        p383_a01_church,
        p383_a01_church_LOD1,
        p383_a01_church_COL,
        p334_a01_food_grow,
        p334_a01_food_grow_COL,
        p345_a01_bio_exp,
        p345_a01_bio_mach_exp,
        p345_a01_bio_mach_exp_COL,
        p345_a01_recycle,
        p345_a01_recycle_COL,
        p345_a01_recycle_LOD1,

        p541_escape_pod,
        p541_escape_pod_LOD1,
        p541_escape_pod_COL,
        p541_escape_pod_base,
        p541_escape_pod_base_LOD1,
        p541_escape_pod_base_COL,
        p541_ventilator_body,
        p541_ventilator_body_LOD1,
        p541_ventilator_body_COL,
        p541_ventilator_propeller,
        p541_ventilator_propeller_COL,

        p349_a_tower,
        p349_a_tower_LOD1,
        p349_a_tower_COL,
        p349_b_tower,
        p349_b_tower_LOD1,
        p349_b_tower_COL,
        p349_c_tower,
        p349_c_tower_LOD1,
        p349_c_tower_COL,

        //p361_a02_hangar_panel,

        p531_b_faction,
        p531_b_faction_COL,
        p531_b_faction_holo,
        p531_b_faction_holo_COL,
        armor_hull,
        armor_hull_LOD1,
        armor_hull_COL,

        FourthReichMothership,
        FourthReichMothership_B,
        FourthReichMothership_B_COL,
        FourthReichMothership_LOD1,
        FourthReichMothership_B_LOD1,
        RusMothership,
        RusMothership_COL,
        RusMothership_LOD1,
        Russian_Mothership_Hummer,
        Russian_Mothership_Hummer_LOD1,
        Russian_Mothership_Hummer_COL,

        p231a01_armor_LOD01,
        p231a02_armor_LOD01,
        p231a03_armor_LOD01,
        p231a04_armor_LOD01,
        p231a05_armor_LOD01,
        p231a06_armor_LOD01,
        p231a07_armor_LOD01,
        p231a08_armor_LOD01,
        p231a09_armor_LOD01,
        p231a10_armor_LOD01,
        p231a11_armor_LOD01,
        p231a12_armor_LOD01,
        p231a13_armor_LOD01,
        p231a14_armor_LOD01,
        p231a15_armor_LOD01,
        p231a16_armor_LOD01,
        p231a17_armor_LOD01,
        p231a18_armor_LOD01,

        p311a01_long_term_thruster_LOD1,
        p312a01_short_term_thruster_latitude_LOD1,
        p312a02_short_term_thruster_lateral_LOD1,

        p321d01_big_solar_panel,
        p321d01_big_solar_panel_LOD1,
        p321d01_big_solar_panel_COL,

        p321b01_nuclear_reactor_LOD1,
        //p321c01_inertia_generator_LOD1,
        p322a01_battery_LOD1,
        p323a01_fuel_generator_LOD1,
        p324a01_fuel_storage_a_LOD1,
        p324a01_fuel_storage_a_COL,
        //p324b01_fuel_storage_b_LOD01,

        p331_a01_oxygen_generator_LOD1,
        p333_a01_hydroponic_building_LOD1,
        p334_a01_food_grow_LOD01,

        //p342_a01_loading_bay_LOD1,
        p345_a01_refinery_LOD1,
        p345_a01_bio_exp_LOD1,
        p345_a01_bio_exp_COL,
        p345_a01_bio_mach_exp_LOD1,
        //p349_a_tower_LOD01,
        //p349_b_tower_LOD01,
        //p349_c_tower_LOD01,

        p381_a01_building1_LOD1,
        p381_b01_building2_LOD1,

        p385_a01_temple_900m_LOD1,

        p212a01_panel_large,
        p212a01_panel_large_LOD1,
        p212a01_panel_large_COL,
        p212a01_panel_medium,
        p212a01_panel_medium_COL,
        p212a01_panel_medium_LOD1,
        p212a01_panel_small,
        p212a01_panel_small_COL,
        p212b02_panel_medium,
        p212b02_panel_medium_COL,
        p212b02_panel_medium_LOD1,
        p212b02_panel_small,
        p212b02_panel_small_COL,
        p212c03_panel_medium,
        p212c03_panel_medium_LOD1,
        p212c03_panel_medium_COL,
        p212c03_panel_small,
        p212c03_panel_small_COL,
        p212d04_panel_medium,
        p212d04_panel_medium_COL,
        p212d04_panel_small,
        p212d04_panel_small_COL,
        p212e05_panel_medium,
        p212e05_panel_medium_COL,
        p212e05_panel_small,
        p212e05_panel_small_COL,

        p341_c01_closed_dock_v1,
        p341_c01_closed_dock_v1_COL,
        p341_c01_closed_dock_v1_LOD1,

        MysteriousBox_matt_5m,
        MysteriousBox_spec_5m,
        MysteriousBox_mid_5m,

        p212b02_panel_large,
        p212b02_panel_large_COL,
        p212b02_panel_large_LOD1,
        p212c03_panel_large,
        p212c03_panel_large_LOD1,
        p212c03_panel_large_COL,
        p212d04_panel_large,
        p212d04_panel_large_LOD1,
        p212d04_panel_large_COL,
        p212e05_panel_large,
        p212e05_panel_large_LOD1,
        p212e05_panel_large_COL,
        p212f01_panel_large,
        p212f01_panel_large_COL,

        Alien_gate,
        Alien_gate_LOD1,

        mship_body,
        mship_body_LOD1,
        mship_engine,
        mship_engine_LOD1,
        mship_shield_back_large_left,
        mship_shield_back_large_left_LOD1,
        mship_shield_back_large_right,
        mship_shield_back_large_right_LOD1,
        mship_shield_back_small_left,
        mship_shield_back_small_left_LOD1,
        mship_shield_back_small_right,
        mship_shield_back_small_right_LOD1,
        mship_shield_front_large_left,
        mship_shield_front_large_left_LOD1,
        mship_shield_front_large_right,
        mship_shield_front_large_right_LOD1,
        mship_shield_front_small_left,
        mship_shield_front_small_left_LOD1,
        mship_shield_front_small_right,
        mship_shield_front_small_right_LOD1,
        mship_shield_front_small02_left,
        mship_shield_front_small02_left_LOD1,
        mship_shield_front_small02_right,
        mship_shield_front_small02_right_LOD1,

        p411_d02_straight_40m_with_hole_COL,
        p411_d03_straight_60m_COL,
        p411_d04_straight_120m_COL,
        p411_d05_straight_180m_COL,
        p411_d02_straight_40m_with_hole_LOD1,
        p411_d03_straight_60m_LOD1,
        p411_d04_straight_120m_LOD1,
        p411_d05_straight_180m_LOD1,
        
        p381_d03_hospital,
        p381_d03_hospital_LOD1,
        p381_d03_hospital_COL,  
        p381_d05_food_grow,
        p381_d05_food_grow_LOD1,                
        p381_c01_building3_LOD1,
        p381_c01_building4,
        p381_c01_building4_COL,
        p381_c01_building4_LOD1,

        Cable_corner_25m,
        Cable_corner_25m_COL,
        Cable_S_45m,
        Cable_S_45m_COL,
        Cable_straight_180,
        Cable_straight_180_COL,
        Cable_straight_45,
        Cable_straight_45_COL,
        Cable_straight_90,
        Cable_straight_90_COL,
        Connection_box,
        Connection_box_COL,

        //p411_d02_straight_40m_with_hole_COL,
        //p411_d02_straight_40m_with_hole_LOD1,
        //p411_d03_straight_60m_COL,
        //p411_d03_straight_60m_LOD1,
        //p411_d04_straight_120m_COL,
        //p411_d04_straight_120m_LOD1,
        //p411_d05_straight_180m_COL,
        //p411_d05_straight_180m_LOD1,

        p411_f01_straight_1_COL,
        p411_f02_straight_2_COL,
        p411_f03_straight_3_COL,
        p411_f04_straight_4_COL,
        p411_f05_straight_5_COL,

        p411_g02_straight_2_COL,
        p411_g03_straight_3_COL,
        p411_g04_straight_4_COL,
        p411_g05_straight_5_COL,

        cargo_box_1,
        cargo_box_2,
        cargo_box_3,
        cargo_box_4,
        cargo_box_5,
        cargo_box_6,
        cargo_box_7,
        cargo_box_8,
        cargo_box_9,
        cargo_box_10,
        cargo_box_small,
        p541_security_hub,
        p541_security_hub_LOD1,
        p541_security_hub_COL,
        Alarm,
        Bank_node,
        Bank_node_COL,
        Cam,
        Alarm_off,

        p361_b01_long_distance_antenna_big,
        p361_b01_long_distance_antenna_big_LOD1,
        p361_b01_long_distance_antenna_big_COL,
        p361_b01_long_distance_antenna_dish,
        p361_b01_long_distance_antenna_dish_COL,
        p361_b01_long_distance_antenna_dish_LOD1,
        fourth_reich_wreck,
        fourth_reich_wreck_LOD1,
        fourth_reich_wreck_COL,
        p344_a03_container,
        p231b01_armor,
        p231b01_armor_LOD1,
        p231b01_armor_COL,
        p231b01_armor_corner,
        p231b01_armor_corner_LOD1,
        p231b01_armor_corner_COL,
        p231b01_armor_edge,
        p231b01_armor_edge_COL,
        p231b01_armor_edge_LOD1,
        p231b01_armor_hole,
        p231b01_armor_hole_LOD1,
        p231b01_armor_hole_COL,

        p150a03_shelf_1,
        p150a03_shelf_1_LOD1,
        p150a03_shelf_1_COL,
        p150a02_shelf_1X2,
        p150a02_shelf_1X2_LOD1,
        p150a02_shelf_1X2_COL,
        p150a01_shelf_1X3,
        p150a01_shelf_1X3_LOD1,
        p150a01_shelf_1X3_COL,
        //LargeShipMachineGunBarrel_LOD1,
        //LargeShipMachineGunBase_LOD1,
        p221b01_chamber_v1_COL,
        p221b02_chamber_v2_COL,
        p221e02_chamber_v2_COL,
        p382_d01_bridge4_LOD1,
        p382_c01_bridge3_LOD1,
        p382_b01_bridge2_LOD1,
        p362_a01_short_distance_antenna_LOD1,
        p361_a01_long_distance_antenna_LOD1,
        p351_a01_weapon_mount_LOD1,
        p411_e01_straight_1_LOD1,
        p411_e02_straight_2_LOD1,
        p411_e03_straight_3_LOD1,
        p411_e04_straight_4_LOD1,
        p411_e05_straight_5_LOD1,
        p413_d01_junction_t_horizontal_LOD1,
        p413_d03_junction_x_horizontal_LOD1,
        p412_b21_turn_s_up_LOD1,
        p412_b22_turn_s_left_LOD1,
        p412_b23_turn_s_right_LOD1,
        p412_b24_turn_s_down_LOD1,
        p412_b11_turn_90_up_160m_LOD1,
        p412_b12_turn_90_left_160m_LOD1,
        p412_b13_turn_90_right_160m_LOD1,
        p412_b14_turn_90_down_160m_LOD1,
        p412_b01_turn_90_up_80m_LOD1,
        p412_b02_turn_90_left_80m_LOD1,
        p412_b03_turn_90_right_80m_LOD1,
        p412_b04_turn_90_down_80m_LOD1,
        p413_c01_junction_t_horizontal_COL,
        p411_c05_straight_180m_COL,
        p411_c05_straight_180m_LOD1,
        p414_b02_entrance_60m_LOD1,

        p611_asteroid_part_A,
        p611_asteroid_part_A_LOD1,
        p611_asteroid_part_B,
        p611_asteroid_part_B_LOD1,
        p363_a01_big_antenna_300m,
        p363_a01_big_antenna_300m_LOD1,
        p363_a01_big_antenna_300m_COL,
        p130d02_d_straight_300m,
        p130d02_d_straight_300m_COL,
        p130d02_d_straight_300m_LOD1,
        p130j01_j_straight_300m,
        p130j01_j_straight_300m_LOD1,
        p130j01_j_straight_300m_COL,
        p120c02_c_straight_400m,
        p120c02_c_straight_400m_LOD1,
        p120c02_c_straight_400m_COL,
        p110b03_lattice_beam_straight_420m,
        p110b03_lattice_beam_straight_420m_COL,
        p110b03_lattice_beam_straight_420m_LOD1,
        p110b04_lattice_beam_straight_420m_with_panels,
        p110b04_lattice_beam_straight_420m_with_panels_LOD1,
        p110b04_lattice_beam_straight_420m_with_panels_COL,
        p345_a01_bio_exp_center,
        p345_a01_bio_exp_center_LOD1,
        p345_a01_bio_exp_center_COL,
        p345_a01_bio_exp_tanks,
        p345_a01_bio_exp_tanks_LOD1,
        p345_a01_bio_exp_tanks_COL,
        p541_ventilator_body_standalone,
        p541_ventilator_body_standalone_LOD1,
        p541_ventilator_body_standalone_COL,
        p541_ventilator_propeller_standalone,
        p541_ventilator_propeller_standalone_COL,
        p541_ventilator_propeller_standalone_LOD1,

        p321c03_centrifuge,
        p321c03_centrifuge_centre,
        p321c03_centrifuge_centre_LOD1,
        p321c03_centrifuge_centre_COL,
        p321c03_centrifuge_COL,
        p321c03_centrifuge_LOD1,
        p321c04_box_generator,
        p321c04_box_generator_COL,
        p321c04_box_generator_LOD1,
        p321c04_two_big_inertia,
        p321c04_two_big_inertia_LOD1,
        p321c05_centrifuge_big,
        p321c05_centrifuge_big_COL,
        p321c05_centrifuge_big_LOD1,
        p321c05_centrifuge_centre_big,
        p321c05_centrifuge_centre_big_LOD1,
        p363_a01_big_antenna_1500m,
        p363_a01_big_antenna_1500m_COL,
        p363_a01_big_antenna_1500m_LOD1,
        p541_ventilator_propeller_big,
        p541_ventilator_propeller_big_COL,
        p541_ventilator_propeller_big_LOD1,
        p321c02_generator_wall_big,
        p321c02_generator_wall_big_COL,
        p321c02_generator_wall_big_LOD1,
        p321c02_inertia_generator_center_big,
        p321c02_inertia_generator_center_big_LOD1,
        p321c02_inertia_generator_center_big_COL,
        p321c02_inertia_generator_center_vert,
        p321c02_inertia_generator_center_vert_LOD1,

        p321c06_inertia_B_centre,
        p321c06_inertia_B_centre_LOD1,
        p321c06_inertia_B_centre_COL,
        p321c06_inertia_generator_B,
        p321c06_inertia_generator_B_COL,
        p321c06_inertia_generator_B_LOD1,
        p4221_a01_cooling_device_ventilator_1,
        p4221_a01_cooling_device_ventilator_2,
        p4221_a01_cooling_device_wall_340x400,
        p4221_a01_cooling_device_wall_340x400_COL,
        p4221_a01_cooling_device_wall_340x400_LOD1,
        p4222_a01_pipes_connector,
        p4222_a01_pipes_connector_center,
        p4222_a01_pipes_connector_center_LOD1,
        p4222_a01_pipes_connector_COL,
        p4222_a01_pipes_connector_LOD1,
        p4223_a01_open_pipe,
        p4223_a01_open_pipe_COL,
        p4223_a01_open_pipe_LOD1,
        p311b01_long_term_thruster,
        p311b01_long_term_thruster_LOD1,
        p321c07_generator,
        p321c07_generator_center,
        p321c07_generator_center_LOD1,
        p321c07_generator_COL,
        p321c07_generator_LOD1,
        p321c07_generator_propeller_1,
        p321c07_generator_propeller_1_COL,
        p321c07_generator_propeller_2,
        p321c07_generator_propeller_2_COL,
        p221k01_chamber_v2,
        p221k01_chamber_v2_LOD1,

        p581_a01_barrel_biohazard,
        p581_a01_barrel_biohazard_2,
        p581_a01_nuke_barrel,
        p581_a01_red_barrel,
        p581_a01_simple_barrel,
        p581_a01_simple_barrel_2,
        cargo_box_11,
        cargo_box_12,
        Barrel_prop_A,
        Barrel_prop_B,
        Barrel_prop_C,
        //Barrel_prop_D,
        //Barrel_prop_E,
        CargoBox_prop_A,
        CargoBox_prop_B,
        CargoBox_prop_C,
        CannonBall_Capsule_1,
        CannonBall_Capsule_1_COL,
        Missile_pack01,
        Missile_pack01_COL,
        Missile_pack02,
        Missile_pack02_COL,
        Missile_plazma01,
        Missile_plazma01_COL,
        Missile_stack_biochem01,
        Missile_stack_biochem01_COL,

        p231a01_armor_COL,
        p231a02_armor_COL,
        p231a03_armor_COL,
        p231a04_armor_COL,
        p231a05_armor_COL,
        p231a06_armor_COL,
        p231a07_armor_COL,
        p231a08_armor_COL,
        p231a09_armor_COL,
        p231a10_armor_COL,
        p231a11_armor_COL,
        p231a12_armor_COL,
        p231a13_armor_COL,
        p231a14_armor_COL,
        p231a15_armor_COL,
        p231a16_armor_COL,
        p231a17_armor_COL,
        p231a18_armor_COL,
        p110a12_solid_beam_junction_x_weak_COL,
        p110a12_solid_beam_junction_x_weak_LOD1,
        p110a22_solid_beam_junction_t_weak_COL,
        p110a22_solid_beam_junction_t_weak_LOD1,
        p110a05_solid_beam_straight_80m_COL,
        p110a05_solid_beam_straight_80m_LOD1,
        p110b01_lattice_beam_straight_10m_COL,
        p110b01_lattice_beam_straight_10m_LOD1,
        p110b04_lattice_beam_straight_60m_with_panels_COL,
        p110b04_lattice_beam_straight_60m_with_panels_LOD1,
        p110b21_lattice_beam_junction_t_strong_COL,
        p110b21_lattice_beam_junction_t_strong_LOD1,
        p110b03_lattice_beam_straight_60m_COL,
        p110b03_lattice_beam_straight_60m_LOD1,
        p110b22_lattice_beam_junction_t_weak_COL,
        p110b22_lattice_beam_junction_t_weak_LOD1,
        p142a11_cage_pillar_COL,
        p142a11_cage_pillar_LOD1,
        p211d02_panel_60mx60m_COL,
        p211d02_panel_60mx60m_LOD1,
        p211g01_panel_120mx60m_COL,
        p211g01_panel_120mx60m_LOD1,
        p211g02_panel_60mx60m_COL,
        p211g02_panel_60mx60m_LOD1,
        p344_a03_container_COL,
        p344_a03_container_LOD1,
        p415_c03_door2_a_left_COL,
        p415_c03_door2_a_left_LOD1,
        p415_c03_door2_a_right_COL,
        p415_c03_door2_a_right_LOD1,
        p415_c03_door2_b_right_COL,
        p415_c03_door2_b_right_LOD1,
        p415_c03_door2_b_left_COL,
        p415_c03_door2_b_left_LOD1,
        p415_c01_doorcase_COL,
        p415_c01_doorcase_LOD1,
        p421_a02_pipe_straight_40m_COL,
        p421_a02_pipe_straight_40m_LOD1,
        p421_a03_pipe_straight_10m_COL,
        p421_a03_pipe_straight_10m_LOD1,
        p421_a01_pipe_straight_80m_COL,
        p421_a01_pipe_straight_80m_LOD1,
        p423_a01_pipe_junction_COL,
        p423_a01_pipe_junction_LOD1,
        p422_a01_pipe_turn_90_COL,
        p422_a01_pipe_turn_90_LOD1,
        p120a22_strong_lattice_junction_t_weak_COL,
        p120a22_strong_lattice_junction_t_weak_LOD1,
        p120a02_strong_lattice_straight_60m_COL,
        p120a02_strong_lattice_straight_60m_LOD1,
        p120a03_strong_lattice_straight_120m_COL,
        p120a03_strong_lattice_straight_120m_LOD1,
        p411_g01_straight_1_COL,
        p411_g01_straight_1_LOD1,
        p324b01_fuel_storage_b_COL,
        p324b01_fuel_storage_b_LOD1,
        p581_a01_o2_barrel,
        p581_a01_o2_barrel_COL,

        p211a01_panel_120mx60m_LOD1,
        p211a02_panel_60mx60m_LOD1,
        p211a03_panel_60mx30m_LOD1,
        p211c01_panel_120mx60m_LOD1,
        p211c02_panel_60mx60m_LOD1,
        p211c03_panel_60mx30m_LOD1,
        p211b01_panel_120mx60m_LOD1,
        p211b02_panel_60mx60m_LOD1,
        p211b03_panel_60mx30m_LOD1,

        Nuclear_Warhead_closed,
        Nuclear_Warhead_closed_COL,
        Nuclear_Warhead_open,
        Nuclear_Warhead_open_COL,
        CargoBox_prop_D,
        p581_a01_nuke_barrel_1,
        p581_a01_simple_barrel_3,
        p221L01_chamber_v1,
        p221L01_chamber_v1_LOD1,
        p221L01_chamber_v1_COL,


        p221m01_chamber_bottom_v1,
        p221m01_chamber_bottom_v1_LOD1,
        p221m01_chamber_bottom_v1_COL,
        p221m01_chamber_center_v1,
        p221m01_chamber_center_v1_LOD1,
        p221m01_chamber_center_v1_COL,
        p221m01_chamber_top_v1,
        p221m01_chamber_top_v1_LOD1,
        p221m01_chamber_top_v1_COL,
        p321e01_solar_panel,
        p321e01_solar_panel_LOD1,
        p321e01_solar_panel_COL,
        p110a06_solid_beam_straight_420m,

        gattling_ammo_belt,
        gattling_ammo_belt_COL,

        p311b01_cut_thruster,
        p311b01_cut_thruster_LOD1,
        p311b01_cut_thruster_COL,
        p312b01_cut_thruster_lateral,
        p312b01_cut_thruster_lateral_LOD1,
        p312b01_cut_thruster_lateral_COL,
        p312b02_cut_thruster_latitude,
        p312b02_cut_thruster_latitude_LOD1,
        p312b02_cut_thruster_latitude_COL,
        alien_detector_unit,
        alien_detector_unit_COL,
        p381_c01_building5,
        p381_c01_building5_LOD1,
        p381_c01_building5_COL,
        p381_c01_building6,
        p381_c01_building6_LOD1,
        p381_c01_building6_COL,
        p381_c01_building7,
        p381_c01_building7_LOD1,
        p381_c01_building7_COL,
        p221n01_chamber_v1,
        p221n01_chamber_v1_LOD1,
        p221n01_chamber_v1_COL,
        p531_d_medic_cross,
        p531_d_medic_cross_COL,

        p541_screen_A,
        p541_screen_A_LOD1,
        p541_screen_A_COL,
        p541_screen_B,
        p541_screen_B_COL,
        p541_screen_B_LOD1,
        p541_terminal_A,
        p541_terminal_A_COL,
        p541_terminal_A_LOD1,

        alien_artefact,
        alien_artefact_COL,

        bomb,
        bomb_COL,

        p581_a01_universal_barrel_COL,
        rail_gun,
        rail_gun_COL,
        rail_gun_LOD1,
        p345_a01_recycle_sphere,
        p345_a01_recycle_sphere_LOD1,
        p345_a01_recycle_sphere_COL,
        prison,
        prison_COL,
        vendor,
        vendor_COL,
        hangar_screen,
        hangar_screen_COL,

        ScannerPlane,        
        ScannerRays,
        Nuclear_Warhead_open_LOD1,
        Nuclear_Warhead_closed_LOD1,

        p511_a17_billboard_portrait_1,
        p511_a17_billboard_portrait_1_COL,
        p412_b24_turn_s_down_COL,
        p414_b02_entrance_60m_COL,
        p415_d01_doorcase_LOD1,
        p411_g02_straight_2_LOD1,
        p411_g03_straight_3_LOD1,
        p411_g04_straight_4_LOD1,
        p411_g05_straight_5_LOD1,
        p511_a14_billboard_COL,
        p413_g01_junction_6axes_LOD1,

        p511_a18_billboard,
        p511_a18_billboard_COL,
        p511_a18_billboard_LOD1,

        p511_a19_billboard,
        p511_a19_billboard_COL,
        p511_a19_billboard_LOD1,

        p511_a17_billboard_portrait_2,
    }

    public enum MyModelTexturesEnum
    {
    };

    static partial class MyModels
    {
        static void InitModels()
        {
            MyMwcLog.WriteLine("MyModels.InitModels - START");
            MyMwcLog.IncreaseIndent();

            if (m_models != null)
                return; //models already inited

            m_models = new MyModel[Enum.GetValues(typeof(MyModelsEnum)).Length];
            m_modelsByAssertName = new Dictionary<string, MyModel>();

           
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //  Static asteroids
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Indestructible_01: 10-20m
            // Indestructible_02: 30-100m
            // Indestructible_03: 300-5000m
            // Indestructible_04: 10000m

            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\10m\\StaticAsteroid10m_A_LOD0", MyMeshDrawTechnique.MESH, MyModelsEnum.StaticAsteroid10m_A_LOD0, MyFakes.UNLOAD_OPTIMIZATION_KEEP_STATIC_ASTEROIDS));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\10m\\StaticAsteroid10m_A_LOD1", MyMeshDrawTechnique.MESH, MyModelsEnum.StaticAsteroid10m_A_LOD1, MyFakes.UNLOAD_OPTIMIZATION_KEEP_STATIC_ASTEROIDS));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\10m\\StaticAsteroid10m_A_LOD2", MyMeshDrawTechnique.MESH, MyModelsEnum.StaticAsteroid10m_A_LOD2, MyFakes.UNLOAD_OPTIMIZATION_KEEP_STATIC_ASTEROIDS));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\20m\\StaticAsteroid20m_A_LOD0", MyMeshDrawTechnique.MESH, MyModelsEnum.StaticAsteroid20m_A_LOD0, MyFakes.UNLOAD_OPTIMIZATION_KEEP_STATIC_ASTEROIDS));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\20m\\StaticAsteroid20m_A_LOD1", MyMeshDrawTechnique.MESH, MyModelsEnum.StaticAsteroid20m_A_LOD1, MyFakes.UNLOAD_OPTIMIZATION_KEEP_STATIC_ASTEROIDS));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\20m\\StaticAsteroid20m_A_LOD2", MyMeshDrawTechnique.MESH, MyModelsEnum.StaticAsteroid20m_A_LOD2, MyFakes.UNLOAD_OPTIMIZATION_KEEP_STATIC_ASTEROIDS));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\30m\\StaticAsteroid30m_A_LOD0", MyMeshDrawTechnique.MESH, MyModelsEnum.StaticAsteroid30m_A_LOD0, MyFakes.UNLOAD_OPTIMIZATION_KEEP_STATIC_ASTEROIDS));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\30m\\StaticAsteroid30m_A_LOD1", MyMeshDrawTechnique.MESH, MyModelsEnum.StaticAsteroid30m_A_LOD1, MyFakes.UNLOAD_OPTIMIZATION_KEEP_STATIC_ASTEROIDS));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\30m\\StaticAsteroid30m_A_LOD2", MyMeshDrawTechnique.MESH, MyModelsEnum.StaticAsteroid30m_A_LOD2, MyFakes.UNLOAD_OPTIMIZATION_KEEP_STATIC_ASTEROIDS));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\50m\\StaticAsteroid50m_A_LOD0", MyMeshDrawTechnique.MESH, MyModelsEnum.StaticAsteroid50m_A_LOD0, MyFakes.UNLOAD_OPTIMIZATION_KEEP_STATIC_ASTEROIDS));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\50m\\StaticAsteroid50m_A_LOD1", MyMeshDrawTechnique.MESH, MyModelsEnum.StaticAsteroid50m_A_LOD1, MyFakes.UNLOAD_OPTIMIZATION_KEEP_STATIC_ASTEROIDS));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\50m\\StaticAsteroid50m_A_LOD2", MyMeshDrawTechnique.MESH, MyModelsEnum.StaticAsteroid50m_A_LOD2, MyFakes.UNLOAD_OPTIMIZATION_KEEP_STATIC_ASTEROIDS));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\100m\\StaticAsteroid100m_A_LOD0", MyMeshDrawTechnique.MESH, MyModelsEnum.StaticAsteroid100m_A_LOD0, MyFakes.UNLOAD_OPTIMIZATION_KEEP_STATIC_ASTEROIDS));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\100m\\StaticAsteroid100m_A_LOD1", MyMeshDrawTechnique.MESH, MyModelsEnum.StaticAsteroid100m_A_LOD1, MyFakes.UNLOAD_OPTIMIZATION_KEEP_STATIC_ASTEROIDS));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\100m\\StaticAsteroid100m_A_LOD2", MyMeshDrawTechnique.MESH, MyModelsEnum.StaticAsteroid100m_A_LOD2, MyFakes.UNLOAD_OPTIMIZATION_KEEP_STATIC_ASTEROIDS));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\300m\\StaticAsteroid300m_A_LOD0", MyMeshDrawTechnique.MESH, MyModelsEnum.StaticAsteroid300m_A_LOD0, MyFakes.UNLOAD_OPTIMIZATION_KEEP_STATIC_ASTEROIDS));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\300m\\StaticAsteroid300m_A_LOD1", MyMeshDrawTechnique.MESH, MyModelsEnum.StaticAsteroid300m_A_LOD1, MyFakes.UNLOAD_OPTIMIZATION_KEEP_STATIC_ASTEROIDS));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\300m\\StaticAsteroid300m_A_LOD2", MyMeshDrawTechnique.MESH, MyModelsEnum.StaticAsteroid300m_A_LOD2, MyFakes.UNLOAD_OPTIMIZATION_KEEP_STATIC_ASTEROIDS));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\500m\\StaticAsteroid500m_A_LOD0", MyMeshDrawTechnique.MESH, MyModelsEnum.StaticAsteroid500m_A_LOD0, MyFakes.UNLOAD_OPTIMIZATION_KEEP_STATIC_ASTEROIDS));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\500m\\StaticAsteroid500m_A_LOD1", MyMeshDrawTechnique.MESH, MyModelsEnum.StaticAsteroid500m_A_LOD1, MyFakes.UNLOAD_OPTIMIZATION_KEEP_STATIC_ASTEROIDS));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\500m\\StaticAsteroid500m_A_LOD2", MyMeshDrawTechnique.MESH, MyModelsEnum.StaticAsteroid500m_A_LOD2, MyFakes.UNLOAD_OPTIMIZATION_KEEP_STATIC_ASTEROIDS));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\1000m\\StaticAsteroid1000m_A_LOD0", MyMeshDrawTechnique.MESH, MyModelsEnum.StaticAsteroid1000m_A_LOD0, MyFakes.UNLOAD_OPTIMIZATION_KEEP_STATIC_ASTEROIDS));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\1000m\\StaticAsteroid1000m_A_LOD1", MyMeshDrawTechnique.MESH, MyModelsEnum.StaticAsteroid1000m_A_LOD1, MyFakes.UNLOAD_OPTIMIZATION_KEEP_STATIC_ASTEROIDS));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\1000m\\StaticAsteroid1000m_A_LOD2", MyMeshDrawTechnique.MESH, MyModelsEnum.StaticAsteroid1000m_A_LOD2, MyFakes.UNLOAD_OPTIMIZATION_KEEP_STATIC_ASTEROIDS));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\2000m\\StaticAsteroid2000m_A_LOD0", MyMeshDrawTechnique.MESH, MyModelsEnum.StaticAsteroid2000m_A_LOD0, MyFakes.UNLOAD_OPTIMIZATION_KEEP_STATIC_ASTEROIDS));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\2000m\\StaticAsteroid2000m_A_LOD1", MyMeshDrawTechnique.MESH, MyModelsEnum.StaticAsteroid2000m_A_LOD1, MyFakes.UNLOAD_OPTIMIZATION_KEEP_STATIC_ASTEROIDS));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\2000m\\StaticAsteroid2000m_A_LOD2", MyMeshDrawTechnique.MESH, MyModelsEnum.StaticAsteroid2000m_A_LOD2, MyFakes.UNLOAD_OPTIMIZATION_KEEP_STATIC_ASTEROIDS));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\5000m\\StaticAsteroid5000m_A_LOD0", MyMeshDrawTechnique.MESH, MyModelsEnum.StaticAsteroid5000m_A_LOD0, MyFakes.UNLOAD_OPTIMIZATION_KEEP_STATIC_ASTEROIDS));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\5000m\\StaticAsteroid5000m_A_LOD1", MyMeshDrawTechnique.MESH, MyModelsEnum.StaticAsteroid5000m_A_LOD1, MyFakes.UNLOAD_OPTIMIZATION_KEEP_STATIC_ASTEROIDS));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\5000m\\StaticAsteroid5000m_A_LOD2", MyMeshDrawTechnique.MESH, MyModelsEnum.StaticAsteroid5000m_A_LOD2, MyFakes.UNLOAD_OPTIMIZATION_KEEP_STATIC_ASTEROIDS));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\10000m\\StaticAsteroid10000m_A_LOD0", MyMeshDrawTechnique.MESH, MyModelsEnum.StaticAsteroid10000m_A_LOD0, MyFakes.UNLOAD_OPTIMIZATION_KEEP_STATIC_ASTEROIDS));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\10000m\\StaticAsteroid10000m_A_LOD1", MyMeshDrawTechnique.MESH, MyModelsEnum.StaticAsteroid10000m_A_LOD1, MyFakes.UNLOAD_OPTIMIZATION_KEEP_STATIC_ASTEROIDS));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\10000m\\StaticAsteroid10000m_A_LOD2", MyMeshDrawTechnique.MESH, MyModelsEnum.StaticAsteroid10000m_A_LOD2, MyFakes.UNLOAD_OPTIMIZATION_KEEP_STATIC_ASTEROIDS));
            //AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\40000m\\StaticAsteroid40000m_A_LOD0", MyMeshDrawTechnique.MESH, MyModelsEnum.StaticAsteroid40000m_A_LOD0, MyFakes.UNLOAD_OPTIMIZATION_STATIC_ASTEROIDS));
            //AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\40000m\\StaticAsteroid40000m_A_LOD1", MyMeshDrawTechnique.MESH, MyModelsEnum.StaticAsteroid40000m_A_LOD1, MyFakes.UNLOAD_OPTIMIZATION_STATIC_ASTEROIDS));
            //AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\40000m\\StaticAsteroid40000m_A_LOD2", MyMeshDrawTechnique.MESH, MyModelsEnum.StaticAsteroid40000m_A_LOD2, MyFakes.UNLOAD_OPTIMIZATION_STATIC_ASTEROIDS));

            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\10m\\StaticAsteroid10m_B_LOD0", MyMeshDrawTechnique.MESH, MyModelsEnum.StaticAsteroid10m_B_LOD0, MyFakes.UNLOAD_OPTIMIZATION_KEEP_STATIC_ASTEROIDS));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\10m\\StaticAsteroid10m_B_LOD1", MyMeshDrawTechnique.MESH, MyModelsEnum.StaticAsteroid10m_B_LOD1, MyFakes.UNLOAD_OPTIMIZATION_KEEP_STATIC_ASTEROIDS));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\10m\\StaticAsteroid10m_B_LOD2", MyMeshDrawTechnique.MESH, MyModelsEnum.StaticAsteroid10m_B_LOD2, MyFakes.UNLOAD_OPTIMIZATION_KEEP_STATIC_ASTEROIDS));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\20m\\StaticAsteroid20m_B_LOD0", MyMeshDrawTechnique.MESH, MyModelsEnum.StaticAsteroid20m_B_LOD0, MyFakes.UNLOAD_OPTIMIZATION_KEEP_STATIC_ASTEROIDS));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\20m\\StaticAsteroid20m_B_LOD1", MyMeshDrawTechnique.MESH, MyModelsEnum.StaticAsteroid20m_B_LOD1, MyFakes.UNLOAD_OPTIMIZATION_KEEP_STATIC_ASTEROIDS));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\20m\\StaticAsteroid20m_B_LOD2", MyMeshDrawTechnique.MESH, MyModelsEnum.StaticAsteroid20m_B_LOD2, MyFakes.UNLOAD_OPTIMIZATION_KEEP_STATIC_ASTEROIDS));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\30m\\StaticAsteroid30m_B_LOD0", MyMeshDrawTechnique.MESH, MyModelsEnum.StaticAsteroid30m_B_LOD0, MyFakes.UNLOAD_OPTIMIZATION_KEEP_STATIC_ASTEROIDS));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\30m\\StaticAsteroid30m_B_LOD1", MyMeshDrawTechnique.MESH, MyModelsEnum.StaticAsteroid30m_B_LOD1, MyFakes.UNLOAD_OPTIMIZATION_KEEP_STATIC_ASTEROIDS));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\30m\\StaticAsteroid30m_B_LOD2", MyMeshDrawTechnique.MESH, MyModelsEnum.StaticAsteroid30m_B_LOD2, MyFakes.UNLOAD_OPTIMIZATION_KEEP_STATIC_ASTEROIDS));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\50m\\StaticAsteroid50m_B_LOD0", MyMeshDrawTechnique.MESH, MyModelsEnum.StaticAsteroid50m_B_LOD0, MyFakes.UNLOAD_OPTIMIZATION_KEEP_STATIC_ASTEROIDS));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\50m\\StaticAsteroid50m_B_LOD1", MyMeshDrawTechnique.MESH, MyModelsEnum.StaticAsteroid50m_B_LOD1, MyFakes.UNLOAD_OPTIMIZATION_KEEP_STATIC_ASTEROIDS));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\50m\\StaticAsteroid50m_B_LOD2", MyMeshDrawTechnique.MESH, MyModelsEnum.StaticAsteroid50m_B_LOD2, MyFakes.UNLOAD_OPTIMIZATION_KEEP_STATIC_ASTEROIDS));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\100m\\StaticAsteroid100m_B_LOD0", MyMeshDrawTechnique.MESH, MyModelsEnum.StaticAsteroid100m_B_LOD0, MyFakes.UNLOAD_OPTIMIZATION_KEEP_STATIC_ASTEROIDS));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\100m\\StaticAsteroid100m_B_LOD1", MyMeshDrawTechnique.MESH, MyModelsEnum.StaticAsteroid100m_B_LOD1, MyFakes.UNLOAD_OPTIMIZATION_KEEP_STATIC_ASTEROIDS));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\100m\\StaticAsteroid100m_B_LOD2", MyMeshDrawTechnique.MESH, MyModelsEnum.StaticAsteroid100m_B_LOD2, MyFakes.UNLOAD_OPTIMIZATION_KEEP_STATIC_ASTEROIDS));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\300m\\StaticAsteroid300m_B_LOD0", MyMeshDrawTechnique.MESH, MyModelsEnum.StaticAsteroid300m_B_LOD0, MyFakes.UNLOAD_OPTIMIZATION_KEEP_STATIC_ASTEROIDS));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\300m\\StaticAsteroid300m_B_LOD1", MyMeshDrawTechnique.MESH, MyModelsEnum.StaticAsteroid300m_B_LOD1, MyFakes.UNLOAD_OPTIMIZATION_KEEP_STATIC_ASTEROIDS));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\300m\\StaticAsteroid300m_B_LOD2", MyMeshDrawTechnique.MESH, MyModelsEnum.StaticAsteroid300m_B_LOD2, MyFakes.UNLOAD_OPTIMIZATION_KEEP_STATIC_ASTEROIDS));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\500m\\StaticAsteroid500m_B_LOD0", MyMeshDrawTechnique.MESH, MyModelsEnum.StaticAsteroid500m_B_LOD0, MyFakes.UNLOAD_OPTIMIZATION_KEEP_STATIC_ASTEROIDS));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\500m\\StaticAsteroid500m_B_LOD1", MyMeshDrawTechnique.MESH, MyModelsEnum.StaticAsteroid500m_B_LOD1, MyFakes.UNLOAD_OPTIMIZATION_KEEP_STATIC_ASTEROIDS));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\500m\\StaticAsteroid500m_B_LOD2", MyMeshDrawTechnique.MESH, MyModelsEnum.StaticAsteroid500m_B_LOD2, MyFakes.UNLOAD_OPTIMIZATION_KEEP_STATIC_ASTEROIDS));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\1000m\\StaticAsteroid1000m_B_LOD0", MyMeshDrawTechnique.MESH, MyModelsEnum.StaticAsteroid1000m_B_LOD0, MyFakes.UNLOAD_OPTIMIZATION_KEEP_STATIC_ASTEROIDS));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\1000m\\StaticAsteroid1000m_B_LOD1", MyMeshDrawTechnique.MESH, MyModelsEnum.StaticAsteroid1000m_B_LOD1, MyFakes.UNLOAD_OPTIMIZATION_KEEP_STATIC_ASTEROIDS));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\1000m\\StaticAsteroid1000m_B_LOD2", MyMeshDrawTechnique.MESH, MyModelsEnum.StaticAsteroid1000m_B_LOD2, MyFakes.UNLOAD_OPTIMIZATION_KEEP_STATIC_ASTEROIDS));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\2000m\\StaticAsteroid2000m_B_LOD0", MyMeshDrawTechnique.MESH, MyModelsEnum.StaticAsteroid2000m_B_LOD0, MyFakes.UNLOAD_OPTIMIZATION_KEEP_STATIC_ASTEROIDS));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\2000m\\StaticAsteroid2000m_B_LOD1", MyMeshDrawTechnique.MESH, MyModelsEnum.StaticAsteroid2000m_B_LOD1, MyFakes.UNLOAD_OPTIMIZATION_KEEP_STATIC_ASTEROIDS));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\2000m\\StaticAsteroid2000m_B_LOD2", MyMeshDrawTechnique.MESH, MyModelsEnum.StaticAsteroid2000m_B_LOD2, MyFakes.UNLOAD_OPTIMIZATION_KEEP_STATIC_ASTEROIDS));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\5000m\\StaticAsteroid5000m_B_LOD0", MyMeshDrawTechnique.MESH, MyModelsEnum.StaticAsteroid5000m_B_LOD0, MyFakes.UNLOAD_OPTIMIZATION_KEEP_STATIC_ASTEROIDS));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\5000m\\StaticAsteroid5000m_B_LOD1", MyMeshDrawTechnique.MESH, MyModelsEnum.StaticAsteroid5000m_B_LOD1, MyFakes.UNLOAD_OPTIMIZATION_KEEP_STATIC_ASTEROIDS));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\5000m\\StaticAsteroid5000m_B_LOD2", MyMeshDrawTechnique.MESH, MyModelsEnum.StaticAsteroid5000m_B_LOD2, MyFakes.UNLOAD_OPTIMIZATION_KEEP_STATIC_ASTEROIDS));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\10000m\\StaticAsteroid10000m_B_LOD0", MyMeshDrawTechnique.MESH, MyModelsEnum.StaticAsteroid10000m_B_LOD0, MyFakes.UNLOAD_OPTIMIZATION_KEEP_STATIC_ASTEROIDS));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\10000m\\StaticAsteroid10000m_B_LOD1", MyMeshDrawTechnique.MESH, MyModelsEnum.StaticAsteroid10000m_B_LOD1, MyFakes.UNLOAD_OPTIMIZATION_KEEP_STATIC_ASTEROIDS));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\10000m\\StaticAsteroid10000m_B_LOD2", MyMeshDrawTechnique.MESH, MyModelsEnum.StaticAsteroid10000m_B_LOD2, MyFakes.UNLOAD_OPTIMIZATION_KEEP_STATIC_ASTEROIDS));
                                     /* Removed
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\10m\\StaticAsteroid10m_C_LOD0", MyMeshDrawTechnique.VOXELS_STATIC_ASTEROID, MyModelsEnum.StaticAsteroid10m_C_LOD0));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\10m\\StaticAsteroid10m_C_LOD1", MyMeshDrawTechnique.VOXELS_STATIC_ASTEROID, MyModelsEnum.StaticAsteroid10m_C_LOD1));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\20m\\StaticAsteroid20m_C_LOD0", MyMeshDrawTechnique.VOXELS_STATIC_ASTEROID, MyModelsEnum.StaticAsteroid20m_C_LOD0));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\20m\\StaticAsteroid20m_C_LOD1", MyMeshDrawTechnique.VOXELS_STATIC_ASTEROID, MyModelsEnum.StaticAsteroid20m_C_LOD1));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\30m\\StaticAsteroid30m_C_LOD0", MyMeshDrawTechnique.VOXELS_STATIC_ASTEROID, MyModelsEnum.StaticAsteroid30m_C_LOD0));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\30m\\StaticAsteroid30m_C_LOD1", MyMeshDrawTechnique.VOXELS_STATIC_ASTEROID, MyModelsEnum.StaticAsteroid30m_C_LOD1));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\50m\\StaticAsteroid50m_C_LOD0", MyMeshDrawTechnique.VOXELS_STATIC_ASTEROID, MyModelsEnum.StaticAsteroid50m_C_LOD0));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\50m\\StaticAsteroid50m_C_LOD1", MyMeshDrawTechnique.VOXELS_STATIC_ASTEROID, MyModelsEnum.StaticAsteroid50m_C_LOD1));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\100m\\StaticAsteroid100m_C_LOD0", MyMeshDrawTechnique.VOXELS_STATIC_ASTEROID, MyModelsEnum.StaticAsteroid100m_C_LOD0));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\100m\\StaticAsteroid100m_C_LOD1", MyMeshDrawTechnique.VOXELS_STATIC_ASTEROID, MyModelsEnum.StaticAsteroid100m_C_LOD1));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\300m\\StaticAsteroid300m_C_LOD0", MyMeshDrawTechnique.VOXELS_STATIC_ASTEROID, MyModelsEnum.StaticAsteroid300m_C_LOD0));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\300m\\StaticAsteroid300m_C_LOD1", MyMeshDrawTechnique.VOXELS_STATIC_ASTEROID, MyModelsEnum.StaticAsteroid300m_C_LOD1));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\500m\\StaticAsteroid500m_C_LOD0", MyMeshDrawTechnique.VOXELS_STATIC_ASTEROID, MyModelsEnum.StaticAsteroid500m_C_LOD0));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\500m\\StaticAsteroid500m_C_LOD1", MyMeshDrawTechnique.VOXELS_STATIC_ASTEROID, MyModelsEnum.StaticAsteroid500m_C_LOD1));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\1000m\\StaticAsteroid1000m_C_LOD0", MyMeshDrawTechnique.VOXELS_STATIC_ASTEROID, MyModelsEnum.StaticAsteroid1000m_C_LOD0));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\1000m\\StaticAsteroid1000m_C_LOD1", MyMeshDrawTechnique.VOXELS_STATIC_ASTEROID, MyModelsEnum.StaticAsteroid1000m_C_LOD1));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\2000m\\StaticAsteroid2000m_C_LOD0", MyMeshDrawTechnique.VOXELS_STATIC_ASTEROID, MyModelsEnum.StaticAsteroid2000m_C_LOD0));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\2000m\\StaticAsteroid2000m_C_LOD1", MyMeshDrawTechnique.VOXELS_STATIC_ASTEROID, MyModelsEnum.StaticAsteroid2000m_C_LOD1));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\5000m\\StaticAsteroid5000m_C_LOD0", MyMeshDrawTechnique.VOXELS_STATIC_ASTEROID, MyModelsEnum.StaticAsteroid5000m_C_LOD0));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\5000m\\StaticAsteroid5000m_C_LOD1", MyMeshDrawTechnique.VOXELS_STATIC_ASTEROID, MyModelsEnum.StaticAsteroid5000m_C_LOD1));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\10000m\\StaticAsteroid10000m_C_LOD0", MyMeshDrawTechnique.VOXELS_STATIC_ASTEROID, MyModelsEnum.StaticAsteroid10000m_C_LOD0));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\10000m\\StaticAsteroid10000m_C_LOD1", MyMeshDrawTechnique.VOXELS_STATIC_ASTEROID, MyModelsEnum.StaticAsteroid10000m_C_LOD1));

            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\10m\\StaticAsteroid10m_D_LOD0", MyMeshDrawTechnique.VOXELS_STATIC_ASTEROID, MyModelsEnum.StaticAsteroid10m_D_LOD0));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\10m\\StaticAsteroid10m_D_LOD1", MyMeshDrawTechnique.VOXELS_STATIC_ASTEROID, MyModelsEnum.StaticAsteroid10m_D_LOD1));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\20m\\StaticAsteroid20m_D_LOD0", MyMeshDrawTechnique.VOXELS_STATIC_ASTEROID, MyModelsEnum.StaticAsteroid20m_D_LOD0));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\20m\\StaticAsteroid20m_D_LOD1", MyMeshDrawTechnique.VOXELS_STATIC_ASTEROID, MyModelsEnum.StaticAsteroid20m_D_LOD1));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\30m\\StaticAsteroid30m_D_LOD0", MyMeshDrawTechnique.VOXELS_STATIC_ASTEROID, MyModelsEnum.StaticAsteroid30m_D_LOD0));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\30m\\StaticAsteroid30m_D_LOD1", MyMeshDrawTechnique.VOXELS_STATIC_ASTEROID, MyModelsEnum.StaticAsteroid30m_D_LOD1));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\50m\\StaticAsteroid50m_D_LOD0", MyMeshDrawTechnique.VOXELS_STATIC_ASTEROID, MyModelsEnum.StaticAsteroid50m_D_LOD0));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\50m\\StaticAsteroid50m_D_LOD1", MyMeshDrawTechnique.VOXELS_STATIC_ASTEROID, MyModelsEnum.StaticAsteroid50m_D_LOD1));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\100m\\StaticAsteroid100m_D_LOD0", MyMeshDrawTechnique.VOXELS_STATIC_ASTEROID, MyModelsEnum.StaticAsteroid100m_D_LOD0));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\100m\\StaticAsteroid100m_D_LOD1", MyMeshDrawTechnique.VOXELS_STATIC_ASTEROID, MyModelsEnum.StaticAsteroid100m_D_LOD1));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\300m\\StaticAsteroid300m_D_LOD0", MyMeshDrawTechnique.VOXELS_STATIC_ASTEROID, MyModelsEnum.StaticAsteroid300m_D_LOD0));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\300m\\StaticAsteroid300m_D_LOD1", MyMeshDrawTechnique.VOXELS_STATIC_ASTEROID, MyModelsEnum.StaticAsteroid300m_D_LOD1));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\500m\\StaticAsteroid500m_D_LOD0", MyMeshDrawTechnique.VOXELS_STATIC_ASTEROID, MyModelsEnum.StaticAsteroid500m_D_LOD0));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\500m\\StaticAsteroid500m_D_LOD1", MyMeshDrawTechnique.VOXELS_STATIC_ASTEROID, MyModelsEnum.StaticAsteroid500m_D_LOD1));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\1000m\\StaticAsteroid1000m_D_LOD0", MyMeshDrawTechnique.VOXELS_STATIC_ASTEROID, MyModelsEnum.StaticAsteroid1000m_D_LOD0));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\1000m\\StaticAsteroid1000m_D_LOD1", MyMeshDrawTechnique.VOXELS_STATIC_ASTEROID, MyModelsEnum.StaticAsteroid1000m_D_LOD1));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\2000m\\StaticAsteroid2000m_D_LOD0", MyMeshDrawTechnique.VOXELS_STATIC_ASTEROID, MyModelsEnum.StaticAsteroid2000m_D_LOD0));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\2000m\\StaticAsteroid2000m_D_LOD1", MyMeshDrawTechnique.VOXELS_STATIC_ASTEROID, MyModelsEnum.StaticAsteroid2000m_D_LOD1));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\5000m\\StaticAsteroid5000m_D_LOD0", MyMeshDrawTechnique.VOXELS_STATIC_ASTEROID, MyModelsEnum.StaticAsteroid5000m_D_LOD0));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\5000m\\StaticAsteroid5000m_D_LOD1", MyMeshDrawTechnique.VOXELS_STATIC_ASTEROID, MyModelsEnum.StaticAsteroid5000m_D_LOD1));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\10000m\\StaticAsteroid10000m_D_LOD0", MyMeshDrawTechnique.VOXELS_STATIC_ASTEROID, MyModelsEnum.StaticAsteroid10000m_D_LOD0));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\10000m\\StaticAsteroid10000m_D_LOD1", MyMeshDrawTechnique.VOXELS_STATIC_ASTEROID, MyModelsEnum.StaticAsteroid10000m_D_LOD1));

            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\10m\\StaticAsteroid10m_E_LOD0", MyMeshDrawTechnique.VOXELS_STATIC_ASTEROID, MyModelsEnum.StaticAsteroid10m_E_LOD0));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\10m\\StaticAsteroid10m_E_LOD1", MyMeshDrawTechnique.VOXELS_STATIC_ASTEROID, MyModelsEnum.StaticAsteroid10m_E_LOD1));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\20m\\StaticAsteroid20m_E_LOD0", MyMeshDrawTechnique.VOXELS_STATIC_ASTEROID, MyModelsEnum.StaticAsteroid20m_E_LOD0));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\20m\\StaticAsteroid20m_E_LOD1", MyMeshDrawTechnique.VOXELS_STATIC_ASTEROID, MyModelsEnum.StaticAsteroid20m_E_LOD1));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\30m\\StaticAsteroid30m_E_LOD0", MyMeshDrawTechnique.VOXELS_STATIC_ASTEROID, MyModelsEnum.StaticAsteroid30m_E_LOD0));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\30m\\StaticAsteroid30m_E_LOD1", MyMeshDrawTechnique.VOXELS_STATIC_ASTEROID, MyModelsEnum.StaticAsteroid30m_E_LOD1));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\50m\\StaticAsteroid50m_E_LOD0", MyMeshDrawTechnique.VOXELS_STATIC_ASTEROID, MyModelsEnum.StaticAsteroid50m_E_LOD0));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\50m\\StaticAsteroid50m_E_LOD1", MyMeshDrawTechnique.VOXELS_STATIC_ASTEROID, MyModelsEnum.StaticAsteroid50m_E_LOD1));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\100m\\StaticAsteroid100m_E_LOD0", MyMeshDrawTechnique.VOXELS_STATIC_ASTEROID, MyModelsEnum.StaticAsteroid100m_E_LOD0));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\100m\\StaticAsteroid100m_E_LOD1", MyMeshDrawTechnique.VOXELS_STATIC_ASTEROID, MyModelsEnum.StaticAsteroid100m_E_LOD1));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\300m\\StaticAsteroid300m_E_LOD0", MyMeshDrawTechnique.VOXELS_STATIC_ASTEROID, MyModelsEnum.StaticAsteroid300m_E_LOD0));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\300m\\StaticAsteroid300m_E_LOD1", MyMeshDrawTechnique.VOXELS_STATIC_ASTEROID, MyModelsEnum.StaticAsteroid300m_E_LOD1));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\500m\\StaticAsteroid500m_E_LOD0", MyMeshDrawTechnique.VOXELS_STATIC_ASTEROID, MyModelsEnum.StaticAsteroid500m_E_LOD0));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\500m\\StaticAsteroid500m_E_LOD1", MyMeshDrawTechnique.VOXELS_STATIC_ASTEROID, MyModelsEnum.StaticAsteroid500m_E_LOD1));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\1000m\\StaticAsteroid1000m_E_LOD0", MyMeshDrawTechnique.VOXELS_STATIC_ASTEROID, MyModelsEnum.StaticAsteroid1000m_E_LOD0));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\1000m\\StaticAsteroid1000m_E_LOD1", MyMeshDrawTechnique.VOXELS_STATIC_ASTEROID, MyModelsEnum.StaticAsteroid1000m_E_LOD1));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\2000m\\StaticAsteroid2000m_E_LOD0", MyMeshDrawTechnique.VOXELS_STATIC_ASTEROID, MyModelsEnum.StaticAsteroid2000m_E_LOD0));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\2000m\\StaticAsteroid2000m_E_LOD1", MyMeshDrawTechnique.VOXELS_STATIC_ASTEROID, MyModelsEnum.StaticAsteroid2000m_E_LOD1));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\5000m\\StaticAsteroid5000m_E_LOD0", MyMeshDrawTechnique.VOXELS_STATIC_ASTEROID, MyModelsEnum.StaticAsteroid5000m_E_LOD0));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\5000m\\StaticAsteroid5000m_E_LOD1", MyMeshDrawTechnique.VOXELS_STATIC_ASTEROID, MyModelsEnum.StaticAsteroid5000m_E_LOD1));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\10000m\\StaticAsteroid10000m_E_LOD0", MyMeshDrawTechnique.VOXELS_STATIC_ASTEROID, MyModelsEnum.StaticAsteroid10000m_E_LOD0));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Asteroids\\10000m\\StaticAsteroid10000m_E_LOD1", MyMeshDrawTechnique.VOXELS_STATIC_ASTEROID, MyModelsEnum.StaticAsteroid10000m_E_LOD1));
                               */
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //  Large ships
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            AddEntityModels("Models2\\ObjectsStatic\\LargeShips\\LargeShip_Kai", MyModelsEnum.Kai, MyModelsEnum.Kai_LOD1, MyModelsEnum.Kai_COL);

            AddEntityModels("Models2\\ObjectsStatic\\LargeShips\\LargeShip_Saya", MyModelsEnum.MotherShipSaya, MyModelsEnum.MotherShipSaya_LOD1, MyModelsEnum.MotherShipSaya_COL);

            AddEntityModels("Models2\\ObjectsStatic\\LargeShips\\LargeShip_Ardant", MyModelsEnum.Ardant, MyModelsEnum.Ardant_LOD1, MyModelsEnum.Ardant_COL);

            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //  Guns for mining ships
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //  Large debris fields
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //  Editor
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////
            //*
            //////////////////////////////////////////////////////////////////////////
            AddEntityModels("Models2\\Prefabs\\04_Connections\\03_Passage\\01\\01_Straight\\p430_a01_passage_10m", MyModelsEnum.p430_a01_passage_10m, MyModelsEnum.p430_a01_passage_10m_LOD1, MyModelsEnum.p430_a01_passage_10m_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\03_Passage\\01\\01_Straight\\p430_a02_passage_40m", MyModelsEnum.p430_a02_passage_40m, MyModelsEnum.p430_a02_passage_40m_LOD1, MyModelsEnum.p430_a02_passage_40m_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\02_Pipe\\01\\04_Base\\p424_a01_pipe_base", MyModelsEnum.p424_a01_pipe_base, null, MyModelsEnum.p424_a01_pipe_base_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\02_Pipe\\01\\03_Junction\\p423_a01_pipe_junction", MyModelsEnum.p423_a01_pipe_junction, MyModelsEnum.p423_a01_pipe_junction_LOD1, MyModelsEnum.p423_a01_pipe_junction_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\02_Pipe\\01\\02_Turn\\p422_a01_pipe_turn_90", MyModelsEnum.p422_a01_pipe_turn_90, MyModelsEnum.p422_a01_pipe_turn_90_LOD1, MyModelsEnum.p422_a01_pipe_turn_90_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\02_Pipe\\01\\01_Straight\\p421_a01_pipe_straight_80m", MyModelsEnum.p421_a01_pipe_straight_80m, MyModelsEnum.p421_a01_pipe_straight_80m_LOD1, MyModelsEnum.p421_a01_pipe_straight_80m_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\02_Pipe\\01\\01_Straight\\p421_a02_pipe_straight_40m", MyModelsEnum.p421_a02_pipe_straight_40m, MyModelsEnum.p421_a02_pipe_straight_40m_LOD1, MyModelsEnum.p421_a02_pipe_straight_40m_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\02_Pipe\\01\\01_Straight\\p421_a03_pipe_straight_10m", MyModelsEnum.p421_a03_pipe_straight_10m, MyModelsEnum.p421_a03_pipe_straight_10m_LOD1, MyModelsEnum.p421_a03_pipe_straight_10m_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\07\\05_Junction\\p413_g01_junction_6axes", MyModelsEnum.p413_g01_junction_6axes, MyModelsEnum.p413_g01_junction_6axes_LOD1, MyModelsEnum.p413_g01_junction_6axes_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\07\\04_Entrance\\p414_g02_entrance_60m", MyModelsEnum.p414_g02_entrance_60m, MyModelsEnum.p414_g02_entrance_60m_LOD1, MyModelsEnum.p414_g02_entrance_60m_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\07\\02_Turn90\\00_0m\\p410_g01_turn_90_right_0m", MyModelsEnum.p410_g01_turn_90_right_0m, MyModelsEnum.p410_g01_turn_90_right_0m_LOD1, MyModelsEnum.p410_g01_turn_90_right_0m_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\07\\01_Straight\\p411_g01_straight_1", MyModelsEnum.p411_g01_straight_1, MyModelsEnum.p411_g01_straight_1_LOD1, MyModelsEnum.p411_g01_straight_1_COL);
            //AddModel(new MyModel("Models2\\Prefabs\\04_Connections\\01_Tunnel\\07\\01_Straight\\p411_g02_straight_2", MyMeshDrawTechnique.MESH, MyModelsEnum.p411_g02_straight_2));
            //AddModel(new MyModel("Models2\\Prefabs\\04_Connections\\01_Tunnel\\07\\01_Straight\\p411_g03_straight_3", MyMeshDrawTechnique.MESH, MyModelsEnum.p411_g03_straight_3));
            //AddModel(new MyModel("Models2\\Prefabs\\04_Connections\\01_Tunnel\\07\\01_Straight\\p411_g04_straight_4", MyMeshDrawTechnique.MESH, MyModelsEnum.p411_g04_straight_4));
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\07\\01_Straight\\p411_g02_straight_2", MyModelsEnum.p411_g02_straight_2, MyModelsEnum.p411_g02_straight_2_LOD1, MyModelsEnum.p411_g02_straight_2_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\07\\01_Straight\\p411_g03_straight_3", MyModelsEnum.p411_g03_straight_3, MyModelsEnum.p411_g03_straight_3_LOD1, MyModelsEnum.p411_g03_straight_3_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\07\\01_Straight\\p411_g04_straight_4", MyModelsEnum.p411_g04_straight_4, MyModelsEnum.p411_g04_straight_4_LOD1, MyModelsEnum.p411_g04_straight_4_COL);
            //AddModel(new MyModel("Models2\\Prefabs\\04_Connections\\01_Tunnel\\06\\04_Entrance\\p414_f02_entrance_60m", MyMeshDrawTechnique.MESH, MyModelsEnum.p414_f02_entrance_60m));
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\06\\03_TurnS\\p412_f21_turn_s_up", MyModelsEnum.p412_f21_turn_s_up, null, MyModelsEnum.p412_f21_turn_s_up_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\06\\03_TurnS\\p412_f22_turn_s_left", MyModelsEnum.p412_f22_turn_s_left, null, MyModelsEnum.p412_f22_turn_s_left_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\06\\03_TurnS\\p412_f23_turn_s_right", MyModelsEnum.p412_f23_turn_s_right, null, MyModelsEnum.p412_f23_turn_s_right_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\06\\03_TurnS\\p412_f24_turn_s_down", MyModelsEnum.p412_f24_turn_s_down, null, MyModelsEnum.p412_f24_turn_s_down_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\06\\02_Turn90\\01_230m\\p412_f01_turn_90_up_230m", MyModelsEnum.p412_f01_turn_90_up_230m, null, null);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\06\\02_Turn90\\01_230m\\p412_f02_turn_90_left_230m", MyModelsEnum.p412_f02_turn_90_left_230m, MyModelsEnum.p412_f02_turn_90_left_230m_LOD1, MyModelsEnum.p412_f02_turn_90_left_230m_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\06\\02_Turn90\\01_230m\\p412_f03_turn_90_right_230m", MyModelsEnum.p412_f03_turn_90_right_230m, MyModelsEnum.p412_f03_turn_90_right_230m_LOD1, MyModelsEnum.p412_f03_turn_90_right_230m_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\06\\02_Turn90\\01_230m\\p412_f04_turn_90_down_230m", MyModelsEnum.p412_f04_turn_90_down_230m, null, MyModelsEnum.p412_f04_turn_90_down_230m_COL);
            //AddModel(new MyModel("Models2\\Prefabs\\04_Connections\\01_Tunnel\\06\\01_Straight\\p411_f01_straight_1", MyMeshDrawTechnique.MESH, MyModelsEnum.p411_f01_straight_1));
            //AddModel(new MyModel("Models2\\Prefabs\\04_Connections\\01_Tunnel\\06\\01_Straight\\p411_f02_straight_2", MyMeshDrawTechnique.MESH, MyModelsEnum.p411_f02_straight_2));
            //AddModel(new MyModel("Models2\\Prefabs\\04_Connections\\01_Tunnel\\06\\01_Straight\\p411_f03_straight_3", MyMeshDrawTechnique.MESH, MyModelsEnum.p411_f03_straight_3));
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\06\\01_Straight\\p411_f01_straight_1", MyModelsEnum.p411_f01_straight_1, null, MyModelsEnum.p411_f01_straight_1_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\06\\01_Straight\\p411_f02_straight_2", MyModelsEnum.p411_f02_straight_2, null, MyModelsEnum.p411_f02_straight_2_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\06\\01_Straight\\p411_f03_straight_3", MyModelsEnum.p411_f03_straight_3, null, MyModelsEnum.p411_f03_straight_3_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\05\\04_Entrance\\p414_e01_entrance_60m", MyModelsEnum.p414_e01_entrance_60m, MyModelsEnum.p414_e01_entrance_60m_LOD1, MyModelsEnum.p414_e01_entrance_60m_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\05\\01_Straight\\p411_e01_straight_1", MyModelsEnum.p411_e01_straight_1, MyModelsEnum.p411_e01_straight_1_LOD1, MyModelsEnum.p411_e01_straight_1_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\05\\01_Straight\\p411_e02_straight_2", MyModelsEnum.p411_e02_straight_2, MyModelsEnum.p411_e02_straight_2_LOD1, MyModelsEnum.p411_e02_straight_2_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\05\\01_Straight\\p411_e03_straight_3", MyModelsEnum.p411_e03_straight_3, MyModelsEnum.p411_e03_straight_3_LOD1, MyModelsEnum.p411_e03_straight_3_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\05\\01_Straight\\p411_e04_straight_4", MyModelsEnum.p411_e04_straight_4, MyModelsEnum.p411_e04_straight_4_LOD1, MyModelsEnum.p411_e04_straight_4_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\05\\01_Straight\\p411_e05_straight_5", MyModelsEnum.p411_e05_straight_5, MyModelsEnum.p411_e05_straight_5_LOD1, MyModelsEnum.p411_e05_straight_5_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\04\\06_Other\\01_Door\\p415_d01_doorcase", MyModelsEnum.p415_d01_doorcase, MyModelsEnum.p415_d01_doorcase_LOD1, MyModelsEnum.p415_d01_doorcase_COL);
            AddModel(new MyModel("Models2\\Prefabs\\04_Connections\\01_Tunnel\\04\\06_Other\\01_Door\\p415_d02_door1", MyMeshDrawTechnique.MESH, MyModelsEnum.p415_d02_door1));
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\04\\06_Other\\01_Door\\p415_d03_door2_a", MyModelsEnum.p415_d03_door2_a, null, MyModelsEnum.p415_d03_door2_a_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\04\\06_Other\\01_Door\\p415_d03_door2_b", MyModelsEnum.p415_d03_door2_b, null, MyModelsEnum.p415_d03_door2_b_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\04\\05_Junction\\p413_d01_junction_t_horizontal", MyModelsEnum.p413_d01_junction_t_horizontal, MyModelsEnum.p413_d01_junction_t_horizontal_LOD1, null);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\04\\05_Junction\\p413_d03_junction_x_horizontal", MyModelsEnum.p413_d03_junction_x_horizontal, MyModelsEnum.p413_d03_junction_x_horizontal_LOD1, MyModelsEnum.p413_d03_junction_x_horizontal_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\04\\04_Entrance\\p414_d01_entrance_60m", MyModelsEnum.p414_d01_entrance_60m, null, MyModelsEnum.p414_d01_entrance_60m_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\04\\01_Straight\\p411_d01_straight_10m", MyModelsEnum.p411_d01_straight_10m, MyModelsEnum.p411_d01_straight_10m_LOD1, MyModelsEnum.p411_d01_straight_10m_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\04\\01_Straight\\p411_d02_straight_40m_with_hole", MyModelsEnum.p411_d02_straight_40m_with_hole, MyModelsEnum.p411_d02_straight_40m_with_hole_LOD1, MyModelsEnum.p411_d02_straight_40m_with_hole_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\04\\01_Straight\\p411_d03_straight_60m", MyModelsEnum.p411_d03_straight_60m, MyModelsEnum.p411_d03_straight_60m_LOD1, MyModelsEnum.p411_d03_straight_60m_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\04\\01_Straight\\p411_d04_straight_120m", MyModelsEnum.p411_d04_straight_120m, MyModelsEnum.p411_d04_straight_120m_LOD1, MyModelsEnum.p411_d04_straight_120m_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\04\\01_Straight\\p411_d05_straight_180m", MyModelsEnum.p411_d05_straight_180m, MyModelsEnum.p411_d05_straight_180m_LOD1, MyModelsEnum.p411_d05_straight_180m_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\03\\06_Other\\01_Door\\p415_c01_doorcase", MyModelsEnum.p415_c01_doorcase, MyModelsEnum.p415_c01_doorcase_LOD1, MyModelsEnum.p415_c01_doorcase_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\03\\06_Other\\01_Door\\p415_c02_door1_right", MyModelsEnum.p415_c02_door1_right, null, MyModelsEnum.p415_c02_door1_right_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\03\\06_Other\\01_Door\\p415_c02_door1_left", MyModelsEnum.p415_c02_door1_left, null, MyModelsEnum.p415_c02_door1_left_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\03\\06_Other\\01_Door\\p415_c03_door2_a_left", MyModelsEnum.p415_c03_door2_a_left, MyModelsEnum.p415_c03_door2_a_left_LOD1, MyModelsEnum.p415_c03_door2_a_left_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\03\\06_Other\\01_Door\\p415_c03_door2_a_right", MyModelsEnum.p415_c03_door2_a_right, MyModelsEnum.p415_c03_door2_a_right_LOD1, MyModelsEnum.p415_c03_door2_a_right_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\03\\06_Other\\01_Door\\p415_c03_door2_b_left", MyModelsEnum.p415_c03_door2_b_left, MyModelsEnum.p415_c03_door2_b_left_LOD1, MyModelsEnum.p415_c03_door2_b_left_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\03\\06_Other\\01_Door\\p415_c03_door2_b_right", MyModelsEnum.p415_c03_door2_b_right, MyModelsEnum.p415_c03_door2_b_right_LOD1, MyModelsEnum.p415_c03_door2_b_right_COL);
            AddModel(new MyModel("Models2\\Prefabs\\04_Connections\\01_Tunnel\\03\\06_Other\\01_Door\\p415_c02_door1", MyMeshDrawTechnique.MESH, MyModelsEnum.p415_c02_door1));
            AddModel(new MyModel("Models2\\Prefabs\\04_Connections\\01_Tunnel\\03\\06_Other\\01_Door\\p415_c03_door2_a", MyMeshDrawTechnique.MESH, MyModelsEnum.p415_c03_door2_a));
            AddModel(new MyModel("Models2\\Prefabs\\04_Connections\\01_Tunnel\\03\\06_Other\\01_Door\\p415_c03_door2_b", MyMeshDrawTechnique.MESH, MyModelsEnum.p415_c03_door2_b));
            AddModel(new MyModel("Models2\\Prefabs\\04_Connections\\01_Tunnel\\03\\06_Other\\01_Door\\p415_c04_door3", MyMeshDrawTechnique.MESH, MyModelsEnum.p415_c04_door3));
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\03\\05_Junction\\p413_c01_junction_t_horizontal", MyModelsEnum.p413_c01_junction_t_horizontal, MyModelsEnum.p413_c01_junction_t_horizontal_LOD1, MyModelsEnum.p413_c01_junction_t_horizontal_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\03\\05_Junction\\p413_c01_junction_x_horizontal", MyModelsEnum.p413_c01_junction_x_horizontal, MyModelsEnum.p413_c01_junction_x_horizontal_LOD1, MyModelsEnum.p413_c01_junction_x_horizontal_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\03\\04_Entrance\\p414_c01_entrance_60m", MyModelsEnum.p414_c01_entrance_60m, null, MyModelsEnum.p414_c01_entrance_60m_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\03\\01_Straight\\p411_c01_straight_10m", MyModelsEnum.p411_c01_straight_10m, null, MyModelsEnum.p411_c01_straight_10m_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\03\\01_Straight\\p411_c02_straight_40m_with_hole", MyModelsEnum.p411_c02_straight_40m_with_hole, MyModelsEnum.p411_c02_straight_40m_with_hole_LOD1, MyModelsEnum.p411_c02_straight_40m_with_hole_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\03\\01_Straight\\p411_c03_straight_60m", MyModelsEnum.p411_c03_straight_60m, MyModelsEnum.p411_c03_straight_60m_LOD1, MyModelsEnum.p411_c03_straight_60m_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\03\\01_Straight\\p411_c04_straight_120m", MyModelsEnum.p411_c04_straight_120m, MyModelsEnum.p411_c04_straight_120m_LOD1, MyModelsEnum.p411_c04_straight_120m_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\03\\01_Straight\\p411_c05_straight_180m", MyModelsEnum.p411_c05_straight_180m, MyModelsEnum.p411_c05_straight_180m_LOD1, MyModelsEnum.p411_c05_straight_180m_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\02\\06_Other\\01_Door\\p415_b01_doorcase", MyModelsEnum.p415_b01_doorcase, MyModelsEnum.p415_b01_doorcase_LOD1, MyModelsEnum.p415_b01_doorcase_COL);
            //AddModel(new MyModel("Models2\\Prefabs\\04_Connections\\01_Tunnel\\02\\06_Other\\01_Door\\p415_b02_door", MyMeshDrawTechnique.MESH, MyModelsEnum.p415_b02_door));
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\02\\05_Junction\\p413_b01_junction_t_horizontal", MyModelsEnum.p413_b01_junction_t_horizontal, MyModelsEnum.p413_b01_junction_t_horizontal_LOD1, MyModelsEnum.p413_b01_junction_t_horizontal_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\02\\05_Junction\\p413_b02_junction_t_vertical", MyModelsEnum.p413_b02_junction_t_vertical, MyModelsEnum.p413_b02_junction_t_vertical_LOD1, MyModelsEnum.p413_b02_junction_t_vertical_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\02\\04_Entrance\\p414_b02_entrance_60m", MyModelsEnum.p414_b02_entrance_60m, MyModelsEnum.p414_b02_entrance_60m_LOD1, MyModelsEnum.p414_b02_entrance_60m_COL);
            //AddModel(new MyModel("Models2\\Prefabs\\04_Connections\\01_Tunnel\\02\\04_Entrance\\p414_b02_entrance_60m_LOD1", MyMeshDrawTechnique.MESH, MyModelsEnum.p414_b02_entrance_60m_LOD1));
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\02\\03_TurnS\\p412_b21_turn_s_up", MyModelsEnum.p412_b21_turn_s_up, MyModelsEnum.p412_b21_turn_s_up_LOD1, MyModelsEnum.p412_b21_turn_s_up_COL);
            AddModel(new MyModel("Models2\\Prefabs\\04_Connections\\01_Tunnel\\02\\03_TurnS\\p412_b22_turn_s_left", MyMeshDrawTechnique.MESH, MyModelsEnum.p412_b22_turn_s_left));
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\02\\03_TurnS\\p412_b23_turn_s_right", MyModelsEnum.p412_b23_turn_s_right, MyModelsEnum.p412_b23_turn_s_right_LOD1, MyModelsEnum.p412_b23_turn_s_right_COL);
            //AddModel(new MyModel("Models2\\Prefabs\\04_Connections\\01_Tunnel\\02\\03_TurnS\\p412_b24_turn_s_down", MyMeshDrawTechnique.MESH, MyModelsEnum.p412_b24_turn_s_down));
            AddModel(new MyModel("Models2\\Prefabs\\04_Connections\\01_Tunnel\\02\\03_TurnS\\p412_b22_turn_s_left_LOD1", MyMeshDrawTechnique.MESH, MyModelsEnum.p412_b22_turn_s_left_LOD1));
            //AddModel(new MyModel("Models2\\Prefabs\\04_Connections\\01_Tunnel\\02\\03_TurnS\\p412_b24_turn_s_down_LOD1", MyMeshDrawTechnique.MESH, MyModelsEnum.p412_b24_turn_s_down_LOD1));
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\02\\03_TurnS\\p412_b24_turn_s_down", MyModelsEnum.p412_b24_turn_s_down, MyModelsEnum.p412_b24_turn_s_down_LOD1, MyModelsEnum.p412_b24_turn_s_down_COL);

            AddModel(new MyModel("Models2\\Prefabs\\04_Connections\\01_Tunnel\\02\\02_Turn90\\02_160m\\p412_b11_turn_90_up_160m", MyMeshDrawTechnique.MESH, MyModelsEnum.p412_b11_turn_90_up_160m));
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\02\\02_Turn90\\02_160m\\p412_b12_turn_90_left_160m", MyModelsEnum.p412_b12_turn_90_left_160m, MyModelsEnum.p412_b12_turn_90_left_160m_LOD1, null);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\02\\02_Turn90\\02_160m\\p412_b13_turn_90_right_160m", MyModelsEnum.p412_b13_turn_90_right_160m, MyModelsEnum.p412_b13_turn_90_right_160m_LOD1, MyModelsEnum.p412_b13_turn_90_right_160m_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\02\\02_Turn90\\02_160m\\p412_b14_turn_90_down_160m", MyModelsEnum.p412_b14_turn_90_down_160m, MyModelsEnum.p412_b14_turn_90_down_160m_LOD1, MyModelsEnum.p412_b14_turn_90_down_160m_COL);
            AddModel(new MyModel("Models2\\Prefabs\\04_Connections\\01_Tunnel\\02\\02_Turn90\\02_160m\\p412_b11_turn_90_up_160m_LOD1", MyMeshDrawTechnique.MESH, MyModelsEnum.p412_b11_turn_90_up_160m_LOD1));
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\02\\02_Turn90\\01_80m\\p412_b01_turn_90_up_80m", MyModelsEnum.p412_b01_turn_90_up_80m, MyModelsEnum.p412_b01_turn_90_up_80m_LOD1, MyModelsEnum.p412_b01_turn_90_up_80m_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\02\\02_Turn90\\01_80m\\p412_b02_turn_90_left_80m", MyModelsEnum.p412_b02_turn_90_left_80m, MyModelsEnum.p412_b02_turn_90_left_80m_LOD1, MyModelsEnum.p412_b02_turn_90_left_80m_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\02\\02_Turn90\\01_80m\\p412_b03_turn_90_right_80m", MyModelsEnum.p412_b03_turn_90_right_80m, MyModelsEnum.p412_b03_turn_90_right_80m_LOD1, MyModelsEnum.p412_b03_turn_90_right_80m_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\02\\02_Turn90\\01_80m\\p412_b04_turn_90_down_80m", MyModelsEnum.p412_b04_turn_90_down_80m, MyModelsEnum.p412_b04_turn_90_down_80m_LOD1, MyModelsEnum.p412_b04_turn_90_down_80m_COL);

            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\02\\01_Straight\\p411_b01_straight_10m", MyModelsEnum.p411_b01_straight_10m, MyModelsEnum.p411_b01_straight_10m_LOD1, MyModelsEnum.p411_b01_straight_10m_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\02\\01_Straight\\p411_b02_straight_30m_yellow", MyModelsEnum.p411_b02_straight_30m_yellow, MyModelsEnum.p411_b02_straight_30m_yellow_LOD1, MyModelsEnum.p411_b02_straight_30m_yellow_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\02\\01_Straight\\p411_b03_straight_320m", MyModelsEnum.p411_b03_straight_320m, MyModelsEnum.p411_b03_straight_320m_LOD1, MyModelsEnum.p411_b03_straight_320m_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\02\\01_Straight\\p411_b04_straight_80m_with_side_grates", MyModelsEnum.p411_b04_straight_80m_with_side_grates, MyModelsEnum.p411_b04_straight_80m_with_side_grates_LOD1, MyModelsEnum.p411_b04_straight_80m_with_side_grates_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\02\\01_Straight\\p411_b05_straight_80m_with_side_open", MyModelsEnum.p411_b05_straight_80m_with_side_open, MyModelsEnum.p411_b05_straight_80m_with_side_open_LOD1, MyModelsEnum.p411_b05_straight_80m_with_side_open_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\02\\01_Straight\\p411_b06_straight_180m_concrete", MyModelsEnum.p411_b06_straight_180m_concrete, MyModelsEnum.p411_b06_straight_180m_concrete_LOD1, MyModelsEnum.p411_b06_straight_180m_concrete_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\02\\01_Straight\\p411_b06_straight_200m", MyModelsEnum.p411_b06_straight_200m, MyModelsEnum.p411_b06_straight_200m_LOD1, MyModelsEnum.p411_b06_straight_200m_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\02\\01_Straight\\p411_b07_straight_180m_blue", MyModelsEnum.p411_b07_straight_180m_blue, MyModelsEnum.p411_b07_straight_180m_blue_LOD1, MyModelsEnum.p411_b07_straight_180m_blue_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\02\\01_Straight\\p411_b09_straight_30m_gray", MyModelsEnum.p411_b09_straight_30m_gray, MyModelsEnum.p411_b09_straight_30m_gray_LOD1, MyModelsEnum.p411_b09_straight_30m_gray_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\02\\01_Straight\\p411_b11_straight_220m", MyModelsEnum.p411_b11_straight_220m, MyModelsEnum.p411_b11_straight_220m_LOD1, MyModelsEnum.p411_b11_straight_220m_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\02\\01_Straight\\p411_b12_straight_160m_dark_metal", MyModelsEnum.p411_b12_straight_160m_dark_metal, MyModelsEnum.p411_b12_straight_160m_dark_metal_LOD1, MyModelsEnum.p411_b12_straight_160m_dark_metal_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\02\\01_Straight\\p411_b13_straight_100m_tube_inside", MyModelsEnum.p411_b13_straight_100m_tube_inside, MyModelsEnum.p411_b13_straight_100m_tube_inside_LOD1, MyModelsEnum.p411_b13_straight_100m_tube_inside_COL);

            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\01\\06_Other\\01_Door\\p415_a01_doorcase", MyModelsEnum.p415_a01_doorcase, MyModelsEnum.p415_a01_doorcase_LOD1, MyModelsEnum.p415_a01_doorcase_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\01\\06_Other\\01_Door\\p415_a02_door_left", MyModelsEnum.p415_a02_door_left, null, MyModelsEnum.p415_a02_door_left_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\01\\06_Other\\01_Door\\p415_a02_door_right", MyModelsEnum.p415_a02_door_right, null, MyModelsEnum.p415_a02_door_right_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\01\\06_Other\\01_Door\\p415_a02_door", MyModelsEnum.p415_a02_door, null, null);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\01\\05_Junction\\p413_a01_junction_t_horizontal", MyModelsEnum.p413_a01_junction_t_horizontal, MyModelsEnum.p413_a01_junction_t_horizontal_LOD1, MyModelsEnum.p413_a01_junction_t_horizontal_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\01\\05_Junction\\p413_a02_junction_t_vertical", MyModelsEnum.p413_a02_junction_t_vertical, MyModelsEnum.p413_a02_junction_t_vertical_LOD1, MyModelsEnum.p413_a02_junction_t_vertical_COL);
            //AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\01\\04_Entrance\\p414_a01_entrance_30m", MyModelsEnum.p414_a01_entrance_30m, null, null);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\01\\04_Entrance\\p414_a02_entrance_60m", MyModelsEnum.p414_a02_entrance_60m, MyModelsEnum.p414_a02_entrance_60m_LOD1, MyModelsEnum.p414_a02_entrance_60m_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\01\\03_TurnS\\p412_a21_turn_s_up", MyModelsEnum.p412_a21_turn_s_up, MyModelsEnum.p412_a21_turn_s_up_LOD1, MyModelsEnum.p412_a21_turn_s_up_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\01\\03_TurnS\\p412_a22_turn_s_left", MyModelsEnum.p412_a22_turn_s_left, MyModelsEnum.p412_a22_turn_s_left_LOD1, MyModelsEnum.p412_a22_turn_s_left_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\01\\03_TurnS\\p412_a23_turn_s_right", MyModelsEnum.p412_a23_turn_s_right, MyModelsEnum.p412_a23_turn_s_right_LOD1, MyModelsEnum.p412_a23_turn_s_right_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\01\\03_TurnS\\p412_a24_turn_s_down", MyModelsEnum.p412_a24_turn_s_down, MyModelsEnum.p412_a24_turn_s_down_LOD1, MyModelsEnum.p412_a24_turn_s_down_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\01\\02_Turn90\\02_160m\\p412_a11_turn_90_up_160m", MyModelsEnum.p412_a11_turn_90_up_160m, MyModelsEnum.p412_a11_turn_90_up_160m_LOD1, MyModelsEnum.p412_a11_turn_90_up_160m_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\01\\02_Turn90\\02_160m\\p412_a12_turn_90_left_160m", MyModelsEnum.p412_a12_turn_90_left_160m, MyModelsEnum.p412_a12_turn_90_left_160m_LOD1, MyModelsEnum.p412_a12_turn_90_left_160m_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\01\\02_Turn90\\02_160m\\p412_a13_turn_90_right_160m", MyModelsEnum.p412_a13_turn_90_right_160m, MyModelsEnum.p412_a13_turn_90_right_160m_LOD1, MyModelsEnum.p412_a13_turn_90_right_160m_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\01\\02_Turn90\\02_160m\\p412_a14_turn_90_down_160m", MyModelsEnum.p412_a14_turn_90_down_160m, MyModelsEnum.p412_a14_turn_90_down_160m_LOD1, MyModelsEnum.p412_a14_turn_90_down_160m_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\01\\02_Turn90\\01_80m\\p412_a01_turn_90_up_80m", MyModelsEnum.p412_a01_turn_90_up_80m, MyModelsEnum.p412_a01_turn_90_up_80m_LOD1, MyModelsEnum.p412_a01_turn_90_up_80m_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\01\\02_Turn90\\01_80m\\p412_a02_turn_90_left_80m", MyModelsEnum.p412_a02_turn_90_left_80m, MyModelsEnum.p412_a02_turn_90_left_80m_LOD1, MyModelsEnum.p412_a02_turn_90_left_80m_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\01\\02_Turn90\\01_80m\\p412_a03_turn_90_right_80m", MyModelsEnum.p412_a03_turn_90_right_80m, MyModelsEnum.p412_a03_turn_90_right_80m_LOD1, MyModelsEnum.p412_a03_turn_90_right_80m_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\01\\02_Turn90\\01_80m\\p412_a04_turn_90_down_80m",  MyModelsEnum.p412_a04_turn_90_down_80m, MyModelsEnum.p412_a04_turn_90_down_80m_LOD1, MyModelsEnum.p412_a04_turn_90_down_80m_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\01\\01_Straight\\p411_a01_straight_10m", MyModelsEnum.p411_a01_straight_10m, MyModelsEnum.p411_a01_straight_10m_LOD1, MyModelsEnum.p411_a01_straight_10m_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\01\\01_Straight\\p411_a02_straight_60m_with_hole", MyModelsEnum.p411_a02_straight_60m_with_hole, MyModelsEnum.p411_a02_straight_60m_with_hole_LOD1, MyModelsEnum.p411_a02_straight_60m_with_hole_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\01\\01_Straight\\p411_a03_straight_120m", MyModelsEnum.p411_a03_straight_120m, MyModelsEnum.p411_a03_straight_120m_LOD1,MyModelsEnum.p411_a03_straight_120m_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\01\\01_Straight\\p411_a04_straight_80m", MyModelsEnum.p411_a04_straight_80m, MyModelsEnum.p411_a04_straight_80m_LOD1, MyModelsEnum.p411_a04_straight_80m_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\01\\01_Straight\\p411_a05_straight_80m_with_extension", MyModelsEnum.p411_a05_straight_80m_with_extension, MyModelsEnum.p411_a05_straight_80m_with_extension_LOD1, MyModelsEnum.p411_a05_straight_80m_with_extension_COL);
            
            AddModel(new MyModel("Models2\\Prefabs\\03_Modules\\08_MannedObjects\\02_Bridge\\05\\p382_e01_bridge5", MyMeshDrawTechnique.MESH, MyModelsEnum.p382_e01_bridge5));
            AddModel(new MyModel("Models2\\Prefabs\\03_Modules\\08_MannedObjects\\02_Bridge\\05\\p382_e01_bridge5_LOD1", MyMeshDrawTechnique.MESH, MyModelsEnum.p382_e01_bridge5_LOD1));
            AddEntityModels("Models2\\Prefabs\\03_Modules\\08_MannedObjects\\02_Bridge\\04\\p382_d01_bridge4", MyModelsEnum.p382_d01_bridge4, MyModelsEnum.p382_d01_bridge4_LOD1, MyModelsEnum.p382_d01_bridge4_COL);
            AddEntityModels("Models2\\Prefabs\\03_Modules\\08_MannedObjects\\02_Bridge\\03\\p382_c01_bridge3", MyModelsEnum.p382_c01_bridge3, MyModelsEnum.p382_c01_bridge3_LOD1, MyModelsEnum.p382_c01_bridge3_COL);
            AddModel(new MyModel("Models2\\Prefabs\\03_Modules\\08_MannedObjects\\02_Bridge\\02\\p382_b01_bridge2", MyMeshDrawTechnique.MESH, MyModelsEnum.p382_b01_bridge2));
            AddModel(new MyModel("Models2\\Prefabs\\03_Modules\\08_MannedObjects\\02_Bridge\\02\\p382_b01_bridge2_LOD1", MyMeshDrawTechnique.MESH, MyModelsEnum.p382_b01_bridge2_LOD1));
            AddEntityModels("Models2\\Prefabs\\03_Modules\\08_MannedObjects\\02_Bridge\\01\\p382_a01_bridge1", MyModelsEnum.p382_a01_bridge1, MyModelsEnum.p382_a01_bridge1_LOD1, MyModelsEnum.p382_a01_bridge1_COL);
            AddEntityModels("Models2\\Prefabs\\03_Modules\\08_MannedObjects\\01_LivingQuarters\\03\\p381_c01_building4", MyModelsEnum.p381_c01_building4, MyModelsEnum.p381_c01_building4_LOD1, MyModelsEnum.p381_c01_building4_COL);
            AddEntityModels("Models2\\Prefabs\\03_Modules\\08_MannedObjects\\01_LivingQuarters\\03\\p381_c01_building3", MyModelsEnum.p381_c01_building3, MyModelsEnum.p381_c01_building3_LOD1, MyModelsEnum.p381_c01_building3_COL);
            AddEntityModels("Models2\\Prefabs\\03_Modules\\08_MannedObjects\\01_LivingQuarters\\02\\p381_b01_building2", MyModelsEnum.p381_b01_building2, MyModelsEnum.p381_b01_building2_LOD1, MyModelsEnum.p381_b01_building2_COL);
            AddEntityModels("Models2\\Prefabs\\03_Modules\\08_MannedObjects\\01_LivingQuarters\\01\\p381_a01_building1", MyModelsEnum.p381_a01_building1, MyModelsEnum.p381_a01_building1_LOD1, MyModelsEnum.p381_a01_building1_COL);
            //AddEntityModels("Models2\\Prefabs\\03_Modules\\07_Hangars\\01_Small\\01\\p361_a01_small_hangar", MyModelsEnum.p361_a01_small_hangar, null, MyModelsEnum.p361_a01_small_hangar_COL);
            AddEntityModels("Models2\\Prefabs\\03_Modules\\06_Communications\\02_ShortDistanceAntenna\\p362_a01_short_distance_antenna", MyModelsEnum.p362_a01_short_distance_antenna, MyModelsEnum.p362_a01_short_distance_antenna_LOD1, MyModelsEnum.p362_a01_short_distance_antenna_COL);
            AddEntityModels("Models2\\Prefabs\\03_Modules\\06_Communications\\01_LongDistanceAntenna\\01\\p361_a01_long_distance_antenna", MyModelsEnum.p361_a01_long_distance_antenna, MyModelsEnum.p361_a01_long_distance_antenna_LOD1, MyModelsEnum.p361_a01_long_distance_antenna_COL);
            AddEntityModels("Models2\\Prefabs\\03_Modules\\05_Weaponry\\01_WeaponMount\\01\\p351_a01_weapon_mount", MyModelsEnum.p351_a01_weapon_mount, MyModelsEnum.p351_a01_weapon_mount_LOD1, MyModelsEnum.p351_a01_weapon_mount_COL);
            AddEntityModels("Models2\\Prefabs\\03_Modules\\04_Industry\\05_Refinery\\01\\p345_a01_refinery", MyModelsEnum.p345_a01_refinery, MyModelsEnum.p345_a01_refinery_LOD1, MyModelsEnum.p345_a01_refinery_COL);
            AddEntityModels("Models2\\Prefabs\\03_Modules\\04_Industry\\04_CargoStorage\\01\\p344_a01_container_arm_filled", MyModelsEnum.p344_a01_container_arm_filled, MyModelsEnum.p344_a01_container_arm_filled_LOD1, MyModelsEnum.p344_a01_container_arm_filled_COL);
            AddEntityModels("Models2\\Prefabs\\03_Modules\\04_Industry\\04_CargoStorage\\01\\p344_a02_container_arm_empty", MyModelsEnum.p344_a02_container_arm_empty, MyModelsEnum.p344_a02_container_arm_empty_LOD1, MyModelsEnum.p344_a02_container_arm_empty_COL);
            //AddModel(new MyModel("Models2\\Prefabs\\03_Modules\\04_Industry\\03_OreStorage\\01\\p343_a01_ore_storage", MyMeshDrawTechnique.MESH, MyModelsEnum.p343_a01_ore_storage));
            AddEntityModels("Models2\\Prefabs\\03_Modules\\04_Industry\\03_OreStorage\\01\\p343_a01_ore_storage", MyModelsEnum.p343_a01_ore_storage, null, MyModelsEnum.p343_a01_ore_storage_COL);
            //AddModel(new MyModel("Models2\\Prefabs\\03_Modules\\04_Industry\\02_LoadingBay\\01\\p342_a01_loading_bay", MyMeshDrawTechnique.MESH, MyModelsEnum.p342_a01_loading_bay));
            AddEntityModels("Models2\\Prefabs\\03_Modules\\04_Industry\\02_LoadingBay\\01\\p342_a01_loading_bay", MyModelsEnum.p342_a01_loading_bay, null, MyModelsEnum.p342_a01_loading_bay_COL);
            AddEntityModels("Models2\\Prefabs\\03_Modules\\04_Industry\\01_Docks\\02\\p341_b01_open_dock_variation1", MyModelsEnum.p341_b01_open_dock_variation1, MyModelsEnum.p341_b01_open_dock_variation1_LOD1, MyModelsEnum.p341_b01_open_dock_variation1_COL);
            AddEntityModels("Models2\\Prefabs\\03_Modules\\04_Industry\\01_Docks\\02\\p341_b02_open_dock_variation2", MyModelsEnum.p341_b02_open_dock_variation2, MyModelsEnum.p341_b02_open_dock_variation2_LOD1, MyModelsEnum.p341_b02_open_dock_variation2_COL);
            AddEntityModels("Models2\\Prefabs\\03_Modules\\04_Industry\\01_Docks\\01\\p341_a01_open_dock_variation1", MyModelsEnum.p341_a01_open_dock_variation1, MyModelsEnum.p341_a01_open_dock_variation1_LOD1, MyModelsEnum.p341_a01_open_dock_variation1_COL);
            AddModel(new MyModel("Models2\\Prefabs\\03_Modules\\04_Industry\\01_Docks\\01\\p341_a01_open_dock_variation1_doorleft", MyMeshDrawTechnique.MESH, MyModelsEnum.p341_a01_open_dock_variation1_doorleft));
            AddModel(new MyModel("Models2\\Prefabs\\03_Modules\\04_Industry\\01_Docks\\01\\p341_a01_open_dock_variation1_doorright", MyMeshDrawTechnique.MESH, MyModelsEnum.p341_a01_open_dock_variation1_doorright));
            AddEntityModels("Models2\\Prefabs\\03_Modules\\04_Industry\\01_Docks\\01\\p341_a02_open_dock_variation2", MyModelsEnum.p341_a02_open_dock_variation2, MyModelsEnum.p341_a02_open_dock_variation2_LOD1, MyModelsEnum.p341_a02_open_dock_variation2_COL);
            AddEntityModels("Models2\\Prefabs\\03_Modules\\03_LifeSupport\\03_HydroponicFarm\\01\\p333_a01_hydroponic_building", MyModelsEnum.p333_a01_hydroponic_building, MyModelsEnum.p333_a01_hydroponic_building_LOD1, MyModelsEnum.p333_a01_hydroponic_building_COL);
            AddEntityModels("Models2\\Prefabs\\03_Modules\\03_LifeSupport\\02_OxygenStorage\\01\\p332_a01_oxygen_storage", MyModelsEnum.p332_a01_oxygen_storage, MyModelsEnum.p332_a01_oxygen_storage_LOD1, MyModelsEnum.p332_a01_oxygen_storage_COL);
            AddEntityModels("Models2\\Prefabs\\03_Modules\\03_LifeSupport\\01_OxygenGeneration\\01\\p331_a01_oxygen_generator", MyModelsEnum.p331_a01_oxygen_generator, MyModelsEnum.p331_a01_oxygen_generator_LOD1, MyModelsEnum.p331_a01_oxygen_generator_COL);
            AddEntityModels("Models2\\Prefabs\\03_Modules\\02_Supply\\04_FuelStorage\\02\\p324b01_fuel_storage_b", MyModelsEnum.p324b01_fuel_storage_b, MyModelsEnum.p324b01_fuel_storage_b_LOD1, MyModelsEnum.p324b01_fuel_storage_b_COL);
            AddEntityModels("Models2\\Prefabs\\03_Modules\\02_Supply\\04_FuelStorage\\01\\p324a01_fuel_storage_a", MyModelsEnum.p324a01_fuel_storage_a, MyModelsEnum.p324a01_fuel_storage_a_LOD1, MyModelsEnum.p324a01_fuel_storage_a_COL);
            AddEntityModels("Models2\\Prefabs\\03_Modules\\02_Supply\\03_FuelGeneration\\01\\p323a01_fuel_generator", MyModelsEnum.p323a01_fuel_generator, MyModelsEnum.p323a01_fuel_generator_LOD1, MyModelsEnum.p323a01_fuel_generator_COL);
            AddEntityModels("Models2\\Prefabs\\03_Modules\\02_Supply\\02_PowerStorage\\01\\p322a01_battery", MyModelsEnum.p322a01_battery, MyModelsEnum.p322a01_battery_LOD1, MyModelsEnum.p322a01_battery_COL);
            AddEntityModels("Models2\\Prefabs\\03_Modules\\02_Supply\\01_PowerGeneration\\03\\p321c01_inertia_generator", MyModelsEnum.p321c01_inertia_generator, MyModelsEnum.p321c01_inertia_generator_LOD1, MyModelsEnum.p321c01_inertia_generator_COL);
            AddEntityModels("Models2\\Prefabs\\03_Modules\\02_Supply\\01_PowerGeneration\\03\\p321c01_inertia_generator_center", MyModelsEnum.p321c01_inertia_generator_center, MyModelsEnum.p321c01_inertia_generator_center_LOD1, MyModelsEnum.p321c01_inertia_generator_center_COL);
            AddModel(new MyModel("Models2\\Prefabs\\03_Modules\\02_Supply\\01_PowerGeneration\\02\\p321b01_nuclear_reactor", MyMeshDrawTechnique.MESH, MyModelsEnum.p321b01_nuclear_reactor));
            //AddEntityModels("Models2\\Prefabs\\03_Modules\\02_Supply\\01_PowerGeneration\\01\\p321a01_solar_panel", MyModelsEnum.p321a01_solar_panel, MyModelsEnum.p321a01_solar_panel_LOD1, MyModelsEnum.p321a01_solar_panel_COL);
            AddEntityModels("Models2\\Prefabs\\03_Modules\\01_Flight\\02_ShortTermThruster\\01\\p312a01_short_term_thruster_latitude", MyModelsEnum.p312a01_short_term_thruster_latitude, MyModelsEnum.p312a01_short_term_thruster_latitude_LOD1, MyModelsEnum.p312a01_short_term_thruster_latitude_COL);
            AddEntityModels("Models2\\Prefabs\\03_Modules\\01_Flight\\02_ShortTermThruster\\01\\p312a02_short_term_thruster_lateral", MyModelsEnum.p312a02_short_term_thruster_lateral, MyModelsEnum.p312a02_short_term_thruster_lateral_LOD1, MyModelsEnum.p312a02_short_term_thruster_lateral_COL);
            AddModel(new MyModel("Models2\\Prefabs\\03_Modules\\01_Flight\\01_LongTermThruster\\01\\p311a01_long_term_thruster", MyMeshDrawTechnique.MESH, MyModelsEnum.p311a01_long_term_thruster));            
            AddEntityModels("Models2\\Prefabs\\02_Shells\\03_Armor\\01\\p231a01_armor", MyModelsEnum.p231a01_armor, MyModelsEnum.p231a01_armor_LOD01, MyModelsEnum.p231a01_armor_COL);
            AddEntityModels("Models2\\Prefabs\\02_Shells\\03_Armor\\01\\p231a02_armor", MyModelsEnum.p231a02_armor, MyModelsEnum.p231a02_armor_LOD01, MyModelsEnum.p231a02_armor_COL);
            AddEntityModels("Models2\\Prefabs\\02_Shells\\03_Armor\\01\\p231a03_armor", MyModelsEnum.p231a03_armor, MyModelsEnum.p231a03_armor_LOD01, MyModelsEnum.p231a03_armor_COL);
            AddEntityModels("Models2\\Prefabs\\02_Shells\\03_Armor\\01\\p231a04_armor", MyModelsEnum.p231a04_armor, MyModelsEnum.p231a04_armor_LOD01, MyModelsEnum.p231a04_armor_COL);
            AddEntityModels("Models2\\Prefabs\\02_Shells\\03_Armor\\01\\p231a05_armor", MyModelsEnum.p231a05_armor, MyModelsEnum.p231a05_armor_LOD01, MyModelsEnum.p231a05_armor_COL);
            AddEntityModels("Models2\\Prefabs\\02_Shells\\03_Armor\\01\\p231a06_armor", MyModelsEnum.p231a06_armor, MyModelsEnum.p231a06_armor_LOD01, MyModelsEnum.p231a06_armor_COL);
            AddEntityModels("Models2\\Prefabs\\02_Shells\\03_Armor\\01\\p231a07_armor", MyModelsEnum.p231a07_armor, MyModelsEnum.p231a07_armor_LOD01, MyModelsEnum.p231a07_armor_COL);
            AddEntityModels("Models2\\Prefabs\\02_Shells\\03_Armor\\01\\p231a08_armor", MyModelsEnum.p231a08_armor, MyModelsEnum.p231a08_armor_LOD01, MyModelsEnum.p231a08_armor_COL);
            AddEntityModels("Models2\\Prefabs\\02_Shells\\03_Armor\\01\\p231a09_armor", MyModelsEnum.p231a09_armor, MyModelsEnum.p231a09_armor_LOD01, MyModelsEnum.p231a09_armor_COL);
            AddEntityModels("Models2\\Prefabs\\02_Shells\\03_Armor\\01\\p231a10_armor", MyModelsEnum.p231a10_armor, MyModelsEnum.p231a10_armor_LOD01, MyModelsEnum.p231a10_armor_COL);
            AddEntityModels("Models2\\Prefabs\\02_Shells\\03_Armor\\01\\p231a11_armor", MyModelsEnum.p231a11_armor, MyModelsEnum.p231a11_armor_LOD01, MyModelsEnum.p231a11_armor_COL);
            AddEntityModels("Models2\\Prefabs\\02_Shells\\03_Armor\\01\\p231a12_armor", MyModelsEnum.p231a12_armor, MyModelsEnum.p231a12_armor_LOD01, MyModelsEnum.p231a12_armor_COL);
            AddEntityModels("Models2\\Prefabs\\02_Shells\\03_Armor\\01\\p231a13_armor", MyModelsEnum.p231a13_armor, MyModelsEnum.p231a13_armor_LOD01, MyModelsEnum.p231a13_armor_COL);
            AddEntityModels("Models2\\Prefabs\\02_Shells\\03_Armor\\01\\p231a14_armor", MyModelsEnum.p231a14_armor, MyModelsEnum.p231a14_armor_LOD01, MyModelsEnum.p231a14_armor_COL);
            AddEntityModels("Models2\\Prefabs\\02_Shells\\03_Armor\\01\\p231a15_armor", MyModelsEnum.p231a15_armor, MyModelsEnum.p231a15_armor_LOD01, MyModelsEnum.p231a15_armor_COL);
            AddEntityModels("Models2\\Prefabs\\02_Shells\\03_Armor\\01\\p231a16_armor", MyModelsEnum.p231a16_armor, MyModelsEnum.p231a16_armor_LOD01, MyModelsEnum.p231a16_armor_COL);
            AddEntityModels("Models2\\Prefabs\\02_Shells\\03_Armor\\01\\p231a17_armor", MyModelsEnum.p231a17_armor, MyModelsEnum.p231a17_armor_LOD01, MyModelsEnum.p231a17_armor_COL);
            AddEntityModels("Models2\\Prefabs\\02_Shells\\03_Armor\\01\\p231a18_armor", MyModelsEnum.p231a18_armor, MyModelsEnum.p231a18_armor_LOD01, MyModelsEnum.p231a18_armor_COL);
            
            AddEntityModels("Models2\\Prefabs\\02_Shells\\02_Chambers\\01\\p221a01_chamber_v1", MyModelsEnum.p221a01_chamber_v1, MyModelsEnum.p221a01_chamber_v1_LOD1, MyModelsEnum.p221a01_chamber_v1_COL);
            AddEntityModels("Models2\\Prefabs\\02_Shells\\02_Chambers\\02\\p221b01_chamber_v1", MyModelsEnum.p221b01_chamber_v1, MyModelsEnum.p221b01_chamber_v1_LOD1, MyModelsEnum.p221b01_chamber_v1_COL);
            AddEntityModels("Models2\\Prefabs\\02_Shells\\02_Chambers\\03\\p221c01_chamber_v1", MyModelsEnum.p221c01_chamber_v1, MyModelsEnum.p221c01_chamber_v1_LOD1, MyModelsEnum.p221c01_chamber_v1_COL);
            AddEntityModels("Models2\\Prefabs\\02_Shells\\02_Chambers\\04\\p221d01_chamber_v1", MyModelsEnum.p221d01_chamber_v1, MyModelsEnum.p221d01_chamber_v1_LOD1, MyModelsEnum.p221d01_chamber_v1_COL);
            AddEntityModels("Models2\\Prefabs\\02_Shells\\02_Chambers\\05\\p221e01_chamber_v1", MyModelsEnum.p221e01_chamber_v1, MyModelsEnum.p221e01_chamber_v1_LOD1, MyModelsEnum.p221e01_chamber_v1_COL);
            AddEntityModels("Models2\\Prefabs\\02_Shells\\02_Chambers\\06\\p221f01_chamber_v1", MyModelsEnum.p221f01_chamber_v1, MyModelsEnum.p221f01_chamber_v1_LOD1, MyModelsEnum.p221f01_chamber_v1_COL);
            AddEntityModels("Models2\\Prefabs\\02_Shells\\02_Chambers\\07\\p221g01_chamber_v1", MyModelsEnum.p221g01_chamber_v1, MyModelsEnum.p221g01_chamber_v1_LOD1, MyModelsEnum.p221g01_chamber_v1_COL);
            AddEntityModels("Models2\\Prefabs\\02_Shells\\02_Chambers\\08\\p221h01_chamber_v1", MyModelsEnum.p221h01_chamber_v1, MyModelsEnum.p221h01_chamber_v1_LOD1, MyModelsEnum.p221h01_chamber_v1_COL);
            AddEntityModels("Models2\\Prefabs\\02_Shells\\02_Chambers\\09\\p221j01_chamber_v1", MyModelsEnum.p221j01_chamber_v1, MyModelsEnum.p221j01_chamber_v1_LOD1, MyModelsEnum.p221j01_chamber_v1_COL);
            AddEntityModels("Models2\\Prefabs\\02_Shells\\02_Chambers\\10\\p221k01_chamber_v1", MyModelsEnum.p221k01_chamber_v1, MyModelsEnum.p221k01_chamber_v1_LOD1, null);

            AddEntityModels("Models2\\Prefabs\\02_Shells\\01_CagePanels\\08\\p211h01_panel_535mx130m", MyModelsEnum.p211h01_panel_535mx130m, MyModelsEnum.p211h01_panel_535mx130m_LOD1, MyModelsEnum.p211h01_panel_535mx130m_COL);

            AddEntityModels("Models2\\Prefabs\\02_Shells\\01_CagePanels\\07\\p211g01_panel_120mx60m", MyModelsEnum.p211g01_panel_120mx60m, MyModelsEnum.p211g01_panel_120mx60m_LOD1, MyModelsEnum.p211g01_panel_120mx60m_COL);
            AddEntityModels("Models2\\Prefabs\\02_Shells\\01_CagePanels\\07\\p211g02_panel_60mx60m", MyModelsEnum.p211g02_panel_60mx60m, MyModelsEnum.p211g02_panel_60mx60m_LOD1, MyModelsEnum.p211g02_panel_60mx60m_COL);
            AddEntityModels("Models2\\Prefabs\\02_Shells\\01_CagePanels\\07\\p211g03_panel_60mx30m", MyModelsEnum.p211g03_panel_60mx30m, MyModelsEnum.p211g03_panel_60mx30m_LOD1, MyModelsEnum.p211g03_panel_60mx30m_COL);
            AddEntityModels("Models2\\Prefabs\\02_Shells\\01_CagePanels\\06\\p211f01_panel_120mx60m", MyModelsEnum.p211f01_panel_120mx60m, MyModelsEnum.p211f01_panel_120mx60m_LOD1, MyModelsEnum.p211f01_panel_120mx60m_COL);
            AddEntityModels("Models2\\Prefabs\\02_Shells\\01_CagePanels\\06\\p211f02_panel_60mx60m", MyModelsEnum.p211f02_panel_60mx60m, MyModelsEnum.p211f02_panel_60mx60m_LOD1, MyModelsEnum.p211f02_panel_60mx60m_COL);
            AddEntityModels("Models2\\Prefabs\\02_Shells\\01_CagePanels\\06\\p211f03_panel_60mx30m", MyModelsEnum.p211f03_panel_60mx30m, null, MyModelsEnum.p211f03_panel_60mx30m_COL);
            AddEntityModels("Models2\\Prefabs\\02_Shells\\01_CagePanels\\05\\p211e01_panel_120mx60m", MyModelsEnum.p211e01_panel_120mx60m, MyModelsEnum.p211e01_panel_120mx60m_LOD1, MyModelsEnum.p211e01_panel_120mx60m_COL);
            AddEntityModels("Models2\\Prefabs\\02_Shells\\01_CagePanels\\05\\p211e02_panel_60mx60m", MyModelsEnum.p211e02_panel_60mx60m, MyModelsEnum.p211e02_panel_60mx60m_LOD1, MyModelsEnum.p211e02_panel_60mx60m_COL);
            AddEntityModels("Models2\\Prefabs\\02_Shells\\01_CagePanels\\05\\p211e03_panel_60mx30m", MyModelsEnum.p211e03_panel_60mx30m, MyModelsEnum.p211e03_panel_60mx30m_LOD1, MyModelsEnum.p211e03_panel_60mx30m_COL);
            AddEntityModels("Models2\\Prefabs\\02_Shells\\01_CagePanels\\04\\p211d01_panel_120mx60m", MyModelsEnum.p211d01_panel_120mx60m, MyModelsEnum.p211d01_panel_120mx60m_LOD1, MyModelsEnum.p211d01_panel_120mx60m_COL);
            AddEntityModels("Models2\\Prefabs\\02_Shells\\01_CagePanels\\04\\p211d02_panel_60mx60m", MyModelsEnum.p211d02_panel_60mx60m, MyModelsEnum.p211d02_panel_60mx60m_LOD1, MyModelsEnum.p211d02_panel_60mx60m_COL);
            AddEntityModels("Models2\\Prefabs\\02_Shells\\01_CagePanels\\04\\p211d03_panel_60mx30m", MyModelsEnum.p211d03_panel_60mx30m, MyModelsEnum.p211d03_panel_60mx30m_LOD1, MyModelsEnum.p211d03_panel_60mx30m_COL);
            AddEntityModels("Models2\\Prefabs\\02_Shells\\01_CagePanels\\03\\p211c01_panel_120mx60m", MyModelsEnum.p211c01_panel_120mx60m, MyModelsEnum.p211c01_panel_120mx60m_LOD1, null);
            AddEntityModels("Models2\\Prefabs\\02_Shells\\01_CagePanels\\03\\p211c02_panel_60mx60m", MyModelsEnum.p211c02_panel_60mx60m, MyModelsEnum.p211c02_panel_60mx60m_LOD1, MyModelsEnum.p211c02_panel_60mx60m_COL);
            AddEntityModels("Models2\\Prefabs\\02_Shells\\01_CagePanels\\03\\p211c03_panel_60mx30m", MyModelsEnum.p211c03_panel_60mx30m, MyModelsEnum.p211c03_panel_60mx30m_LOD1, null);
            AddEntityModels("Models2\\Prefabs\\02_Shells\\01_CagePanels\\02\\p211b01_panel_120mx60m", MyModelsEnum.p211b01_panel_120mx60m, MyModelsEnum.p211b01_panel_120mx60m_LOD1, MyModelsEnum.p211b01_panel_120mx60m_COL);
            AddEntityModels("Models2\\Prefabs\\02_Shells\\01_CagePanels\\02\\p211b02_panel_60mx60m", MyModelsEnum.p211b02_panel_60mx60m, MyModelsEnum.p211b02_panel_60mx60m_LOD1, MyModelsEnum.p211b02_panel_60mx60m_COL);
            AddEntityModels("Models2\\Prefabs\\02_Shells\\01_CagePanels\\02\\p211b03_panel_60mx30m", MyModelsEnum.p211b03_panel_60mx30m, MyModelsEnum.p211b03_panel_60mx30m_LOD1, MyModelsEnum.p211b03_panel_60mx30m_COL);
            AddEntityModels("Models2\\Prefabs\\02_Shells\\01_CagePanels\\01\\p211a01_panel_120mx60m", MyModelsEnum.p211a01_panel_120mx60m, MyModelsEnum.p211a01_panel_120mx60m_LOD1, MyModelsEnum.p211a01_panel_120mx60m_COL);
            AddEntityModels("Models2\\Prefabs\\02_Shells\\01_CagePanels\\01\\p211a02_panel_60mx60m", MyModelsEnum.p211a02_panel_60mx60m, MyModelsEnum.p211a02_panel_60mx60m_LOD1, MyModelsEnum.p211a02_panel_60mx60m_COL);
            AddEntityModels("Models2\\Prefabs\\02_Shells\\01_CagePanels\\01\\p211a03_panel_60mx30m", MyModelsEnum.p211a03_panel_60mx30m, MyModelsEnum.p211a03_panel_60mx30m_LOD1, MyModelsEnum.p211a03_panel_60mx30m_COL);
            AddModel(new MyModel("Models2\\Prefabs\\01_Beams\\04_Frame\\02_Cage\\02\\p142b01_cage_empty", MyMeshDrawTechnique.MESH, MyModelsEnum.p142b01_cage_empty));
            AddModel(new MyModel("Models2\\Prefabs\\01_Beams\\04_Frame\\02_Cage\\02\\p142b02_cage_halfcut", MyMeshDrawTechnique.MESH, MyModelsEnum.p142b02_cage_halfcut));
            AddEntityModels("Models2\\Prefabs\\01_Beams\\04_Frame\\02_Cage\\02\\p142b03_cage_with_corners", MyModelsEnum.p142b03_cage_with_corners, MyModelsEnum.p142b03_cage_with_corners_LOD1, MyModelsEnum.p142b03_cage_with_corners_COL);
            AddEntityModels("Models2\\Prefabs\\01_Beams\\04_Frame\\02_Cage\\02\\p142b11_cage_pillar", MyModelsEnum.p142b11_cage_pillar, null, MyModelsEnum.p142b11_cage_pillar_COL);
            AddModel(new MyModel("Models2\\Prefabs\\01_Beams\\04_Frame\\02_Cage\\02\\p142b12_cage_edge", MyMeshDrawTechnique.MESH, MyModelsEnum.p142b12_cage_edge));
            AddEntityModels("Models2\\Prefabs\\01_Beams\\04_Frame\\02_Cage\\01\\p142a01_cage_empty", MyModelsEnum.p142a01_cage_empty, null, MyModelsEnum.p142a01_cage_empty_COL);
            AddEntityModels("Models2\\Prefabs\\01_Beams\\04_Frame\\02_Cage\\01\\p142a02_cage_halfcut", MyModelsEnum.p142a02_cage_halfcut, null, MyModelsEnum.p142a02_cage_halfcut_COL);
            AddEntityModels("Models2\\Prefabs\\01_Beams\\04_Frame\\02_Cage\\01\\p142a03_cage_with_corners", MyModelsEnum.p142a03_cage_with_corners, null, MyModelsEnum.p142a03_cage_with_corners_COL);
            AddEntityModels("Models2\\Prefabs\\01_Beams\\04_Frame\\02_Cage\\01\\p142a11_cage_pillar", MyModelsEnum.p142a11_cage_pillar, MyModelsEnum.p142a11_cage_pillar_LOD1, MyModelsEnum.p142a11_cage_pillar_COL);
            AddModel(new MyModel("Models2\\Prefabs\\01_Beams\\04_Frame\\02_Cage\\01\\p142a12_cage_edge", MyMeshDrawTechnique.MESH, MyModelsEnum.p142a12_cage_edge));
            AddModel(new MyModel("Models2\\Prefabs\\01_Beams\\04_Frame\\01_Frame\\02\\p141b01_thick_frame_straight_10m", MyMeshDrawTechnique.MESH, MyModelsEnum.p141b01_thick_frame_straight_10m));
            AddEntityModels("Models2\\Prefabs\\01_Beams\\04_Frame\\01_Frame\\02\\p141b02_thick_frame_straight_60m", MyModelsEnum.p141b02_thick_frame_straight_60m, null, MyModelsEnum.p141b02_thick_frame_straight_60m_COL);
            AddModel(new MyModel("Models2\\Prefabs\\01_Beams\\04_Frame\\01_Frame\\02\\p141b11_thick_frame_edge", MyMeshDrawTechnique.MESH, MyModelsEnum.p141b11_thick_frame_edge));
            AddModel(new MyModel("Models2\\Prefabs\\01_Beams\\04_Frame\\01_Frame\\02\\p141b12_thick_frame_corner", MyMeshDrawTechnique.MESH, MyModelsEnum.p141b12_thick_frame_corner));
            AddEntityModels("Models2\\Prefabs\\01_Beams\\04_Frame\\01_Frame\\02\\p141b31_thick_frame_joint", MyModelsEnum.p141b31_thick_frame_joint, null, MyModelsEnum.p141b31_thick_frame_joint_COL);
            AddEntityModels("Models2\\Prefabs\\01_Beams\\04_Frame\\01_Frame\\01\\p141a01_thick_frame_straight_10m", MyModelsEnum.p141a01_thick_frame_straight_10m, null, MyModelsEnum.p141a01_thick_frame_straight_10m_COL);
            AddEntityModels("Models2\\Prefabs\\01_Beams\\04_Frame\\01_Frame\\01\\p141a02_thick_frame_straight_60m", MyModelsEnum.p141a02_thick_frame_straight_60m, null, MyModelsEnum.p141a02_thick_frame_straight_60m_COL);
            AddEntityModels("Models2\\Prefabs\\01_Beams\\04_Frame\\01_Frame\\01\\p141a11_thick_frame_edge", MyModelsEnum.p141a11_thick_frame_edge, null, MyModelsEnum.p141a11_thick_frame_edge_COL);
            AddModel(new MyModel("Models2\\Prefabs\\01_Beams\\04_Frame\\01_Frame\\01\\p141a12_thick_frame_corner", MyMeshDrawTechnique.MESH, MyModelsEnum.p141a12_thick_frame_corner));
            AddEntityModels("Models2\\Prefabs\\01_Beams\\04_Frame\\01_Frame\\01\\p141a31_thick_frame_joint", MyModelsEnum.p141a31_thick_frame_joint, null, MyModelsEnum.p141a31_thick_frame_joint_COL);
            AddEntityModels("Models2\\Prefabs\\01_Beams\\03_Small\\10\\p130j01_j_straight_30m", MyModelsEnum.p130j01_j_straight_30m, null, MyModelsEnum.p130j01_j_straight_30m_COL);
            AddEntityModels("Models2\\Prefabs\\01_Beams\\03_Small\\10\\p130j02_j_straight_10m", MyModelsEnum.p130j02_j_straight_10m, null, MyModelsEnum.p130j02_j_straight_10m_COL);
            AddEntityModels("Models2\\Prefabs\\01_Beams\\03_Small\\09\\p130i01_i_straight_30m", MyModelsEnum.p130i01_i_straight_30m, null, null);
            AddEntityModels("Models2\\Prefabs\\01_Beams\\03_Small\\09\\p130i02_i_straight_10m", MyModelsEnum.p130i02_i_straight_10m, null, MyModelsEnum.p130i02_i_straight_10m_COL);
            AddEntityModels("Models2\\Prefabs\\01_Beams\\03_Small\\08\\p130h01_h_straight_30m", MyModelsEnum.p130h01_h_straight_30m, null, MyModelsEnum.p130h01_h_straight_30m_COL);
            AddEntityModels("Models2\\Prefabs\\01_Beams\\03_Small\\08\\p130h02_h_straight_10m", MyModelsEnum.p130h02_h_straight_10m, null, MyModelsEnum.p130h02_h_straight_10m_COL);
            //AddModel(new MyModel("Models2\\Prefabs\\01_Beams\\03_Small\\07\\p130g01_g_straight_30m", MyMeshDrawTechnique.MESH, MyModelsEnum.p130g01_g_straight_30m));
            //AddModel(new MyModel("Models2\\Prefabs\\01_Beams\\03_Small\\07\\p130g02_g_straight_10m", MyMeshDrawTechnique.MESH, MyModelsEnum.p130g02_g_straight_10m));
            //AddModel(new MyModel("Models2\\Prefabs\\01_Beams\\03_Small\\06\\p130f01_f_straight_30m", MyMeshDrawTechnique.MESH, MyModelsEnum.p130f01_f_straight_30m));
            //AddModel(new MyModel("Models2\\Prefabs\\01_Beams\\03_Small\\06\\p130f02_f_straight_10m", MyMeshDrawTechnique.MESH, MyModelsEnum.p130f02_f_straight_10m));
            //AddModel(new MyModel("Models2\\Prefabs\\01_Beams\\03_Small\\05\\p130e01_e_straight_30m", MyMeshDrawTechnique.MESH, MyModelsEnum.p130e01_e_straight_30m));
            //AddModel(new MyModel("Models2\\Prefabs\\01_Beams\\03_Small\\05\\p130e02_e_straight_10m", MyMeshDrawTechnique.MESH, MyModelsEnum.p130e02_e_straight_10m));
            //AddModel(new MyModel("Models2\\Prefabs\\01_Beams\\03_Small\\04\\p130d01_d_straight_30m", MyMeshDrawTechnique.MESH, MyModelsEnum.p130d01_d_straight_30m));
            //AddModel(new MyModel("Models2\\Prefabs\\01_Beams\\03_Small\\04\\p130d02_d_straight_10m", MyMeshDrawTechnique.MESH, MyModelsEnum.p130d02_d_straight_10m));
            //AddModel(new MyModel("Models2\\Prefabs\\01_Beams\\03_Small\\03\\p130c01_c_straight_30m", MyMeshDrawTechnique.MESH, MyModelsEnum.p130c01_c_straight_30m));
            //AddModel(new MyModel("Models2\\Prefabs\\01_Beams\\03_Small\\03\\p130c02_c_straight_10m", MyMeshDrawTechnique.MESH, MyModelsEnum.p130c02_c_straight_10m));
            //AddModel(new MyModel("Models2\\Prefabs\\01_Beams\\03_Small\\02\\p130b01_b_straight_30m", MyMeshDrawTechnique.MESH, MyModelsEnum.p130b01_b_straight_30m));
            //AddModel(new MyModel("Models2\\Prefabs\\01_Beams\\03_Small\\02\\p130b02_b_straight_10m", MyMeshDrawTechnique.MESH, MyModelsEnum.p130b02_b_straight_10m));
            //AddModel(new MyModel("Models2\\Prefabs\\01_Beams\\03_Small\\01\\p130a01_a_straight_30m", MyMeshDrawTechnique.MESH, MyModelsEnum.p130a01_a_straight_30m));
            //AddModel(new MyModel("Models2\\Prefabs\\01_Beams\\03_Small\\01\\p130a02_a_straight_10m", MyMeshDrawTechnique.MESH, MyModelsEnum.p130a02_a_straight_10m));
            AddEntityModels("Models2\\Prefabs\\01_Beams\\02_Medium\\04\\p120d01_d_straight_10m", MyModelsEnum.p120d01_d_straight_10m, null, MyModelsEnum.p120d01_d_straight_10m_COL);
            AddEntityModels("Models2\\Prefabs\\01_Beams\\02_Medium\\04\\p120d02_d_straight_40m", MyModelsEnum.p120d02_d_straight_40m, null, MyModelsEnum.p120d02_d_straight_40m_COL);
            AddEntityModels("Models2\\Prefabs\\01_Beams\\02_Medium\\03\\p120c01_c_straight_10m", MyModelsEnum.p120c01_c_straight_10m, null, MyModelsEnum.p120c01_c_straight_10m_COL);
            AddEntityModels("Models2\\Prefabs\\01_Beams\\02_Medium\\03\\p120c02_c_straight_40m", MyModelsEnum.p120c02_c_straight_40m, null, MyModelsEnum.p120c02_c_straight_40m_COL);
            AddEntityModels("Models2\\Prefabs\\01_Beams\\02_Medium\\02\\p120b01_b_straight_10m", MyModelsEnum.p120b01_b_straight_10m, null, MyModelsEnum.p120b01_b_straight_10m_COL);
            AddEntityModels("Models2\\Prefabs\\01_Beams\\02_Medium\\02\\p120b02_b_straight_40m", MyModelsEnum.p120b02_b_straight_40m, null, MyModelsEnum.p120b02_b_straight_40m_COL);
            AddEntityModels("Models2\\Prefabs\\01_Beams\\02_Medium\\01\\p120a01_strong_lattice_straight_10m", MyModelsEnum.p120a01_strong_lattice_straight_10m, null, MyModelsEnum.p120a01_strong_lattice_straight_10m_COL);
            AddEntityModels("Models2\\Prefabs\\01_Beams\\02_Medium\\01\\p120a02_strong_lattice_straight_60m", MyModelsEnum.p120a02_strong_lattice_straight_60m, MyModelsEnum.p120a02_strong_lattice_straight_60m_LOD1, MyModelsEnum.p120a02_strong_lattice_straight_60m_COL);
            AddEntityModels("Models2\\Prefabs\\01_Beams\\02_Medium\\01\\p120a03_strong_lattice_straight_120m", MyModelsEnum.p120a03_strong_lattice_straight_120m, MyModelsEnum.p120a03_strong_lattice_straight_120m_LOD1, MyModelsEnum.p120a03_strong_lattice_straight_120m_COL);
            AddEntityModels("Models2\\Prefabs\\01_Beams\\02_Medium\\01\\p120a21_strong_lattice_junction_t_strong", MyModelsEnum.p120a21_strong_lattice_junction_t_strong, null, MyModelsEnum.p120a21_strong_lattice_junction_t_strong_COL);
            AddEntityModels("Models2\\Prefabs\\01_Beams\\02_Medium\\01\\p120a22_strong_lattice_junction_t_weak", MyModelsEnum.p120a22_strong_lattice_junction_t_weak, MyModelsEnum.p120a22_strong_lattice_junction_t_weak_LOD1, MyModelsEnum.p120a22_strong_lattice_junction_t_weak_COL);
            AddEntityModels("Models2\\Prefabs\\01_Beams\\02_Medium\\01\\p120a23_strong_lattice_junction_t_rotated", MyModelsEnum.p120a23_strong_lattice_junction_t_rotated, null, MyModelsEnum.p120a23_strong_lattice_junction_t_rotated_COL);
            AddEntityModels("Models2\\Prefabs\\01_Beams\\02_Medium\\01\\p120a51_strong_to_weak_lattice_2to1", MyModelsEnum.p120a51_strong_to_weak_lattice_2to1, null, MyModelsEnum.p120a51_strong_to_weak_lattice_2to1_COL);
            AddEntityModels("Models2\\Prefabs\\01_Beams\\02_Medium\\01\\p120a52_strong_to_weak_lattice_1to2", MyModelsEnum.p120a52_strong_to_weak_lattice_1to2, null, null);
            AddEntityModels("Models2\\Prefabs\\01_Beams\\02_Medium\\01\\p120a61_weak_lattice_junction_t_rotated", MyModelsEnum.p120a61_weak_lattice_junction_t_rotated, null, MyModelsEnum.p120a61_weak_lattice_junction_t_rotated_COL);
            AddEntityModels("Models2\\Prefabs\\01_Beams\\01_Large\\02\\p110b01_lattice_beam_straight_10m", MyModelsEnum.p110b01_lattice_beam_straight_10m, MyModelsEnum.p110b01_lattice_beam_straight_10m_LOD1, MyModelsEnum.p110b01_lattice_beam_straight_10m_COL);
            AddEntityModels("Models2\\Prefabs\\01_Beams\\01_Large\\02\\p110b02_lattice_beam_straight_30m", MyModelsEnum.p110b02_lattice_beam_straight_30m, null, MyModelsEnum.p110b02_lattice_beam_straight_30m_COL);
            AddEntityModels("Models2\\Prefabs\\01_Beams\\01_Large\\02\\p110b03_lattice_beam_straight_60m", MyModelsEnum.p110b03_lattice_beam_straight_60m, MyModelsEnum.p110b03_lattice_beam_straight_60m_LOD1, MyModelsEnum.p110b03_lattice_beam_straight_60m_COL);
            AddEntityModels("Models2\\Prefabs\\01_Beams\\01_Large\\02\\p110b04_lattice_beam_straight_60m_with_panels", MyModelsEnum.p110b04_lattice_beam_straight_60m_with_panels, MyModelsEnum.p110b04_lattice_beam_straight_60m_with_panels_LOD1, MyModelsEnum.p110b04_lattice_beam_straight_60m_with_panels_COL);
            AddEntityModels("Models2\\Prefabs\\01_Beams\\01_Large\\02\\p110b21_lattice_beam_junction_t_strong", MyModelsEnum.p110b21_lattice_beam_junction_t_strong, MyModelsEnum.p110b21_lattice_beam_junction_t_strong_LOD1, MyModelsEnum.p110b21_lattice_beam_junction_t_strong_COL);
            AddEntityModels("Models2\\Prefabs\\01_Beams\\01_Large\\02\\p110b22_lattice_beam_junction_t_weak", MyModelsEnum.p110b22_lattice_beam_junction_t_weak, MyModelsEnum.p110b22_lattice_beam_junction_t_weak_LOD1, MyModelsEnum.p110b22_lattice_beam_junction_t_weak_COL);
            AddEntityModels("Models2\\Prefabs\\01_Beams\\01_Large\\02\\p110b31_lattice_beam_joint_horizontal", MyModelsEnum.p110b31_lattice_beam_joint_horizontal, null, MyModelsEnum.p110b31_lattice_beam_joint_horizontal_COL);
            AddEntityModels("Models2\\Prefabs\\01_Beams\\01_Large\\02\\p110b32_lattice_beam_joint_vertical", MyModelsEnum.p110b32_lattice_beam_joint_vertical, null, MyModelsEnum.p110b32_lattice_beam_joint_vertical_COL);
            AddEntityModels("Models2\\Prefabs\\01_Beams\\01_Large\\01\\p110a01_solid_beam_straight_10m", MyModelsEnum.p110a01_solid_beam_straight_10m, null, MyModelsEnum.p110a01_solid_beam_straight_10m_COL);
            AddEntityModels("Models2\\Prefabs\\01_Beams\\01_Large\\01\\p110a02_solid_beam_straight_20m", MyModelsEnum.p110a02_solid_beam_straight_20m, null, MyModelsEnum.p110a02_solid_beam_straight_20m_COL);
            AddEntityModels("Models2\\Prefabs\\01_Beams\\01_Large\\01\\p110a03_solid_beam_straight_40m_with_hole", MyModelsEnum.p110a03_solid_beam_straight_40m_with_hole, null, MyModelsEnum.p110a03_solid_beam_straight_40m_with_hole_COL);
            AddEntityModels("Models2\\Prefabs\\01_Beams\\01_Large\\01\\p110a04_solid_beam_straight_40m_lattice", MyModelsEnum.p110a04_solid_beam_straight_40m_lattice, MyModelsEnum.p110a04_solid_beam_straight_40m_lattice_LOD1, MyModelsEnum.p110a04_solid_beam_straight_40m_lattice_COL);
            AddEntityModels("Models2\\Prefabs\\01_Beams\\01_Large\\01\\p110a05_solid_beam_straight_80m", MyModelsEnum.p110a05_solid_beam_straight_80m, MyModelsEnum.p110a05_solid_beam_straight_80m_LOD1, MyModelsEnum.p110a05_solid_beam_straight_80m_COL);
            AddEntityModels("Models2\\Prefabs\\01_Beams\\01_Large\\01\\p110a11_solid_beam_junction_x_strong", MyModelsEnum.p110a11_solid_beam_junction_x_strong, MyModelsEnum.p110a11_solid_beam_junction_x_strong_LOD1, MyModelsEnum.p110a11_solid_beam_junction_x_strong_COL);
            AddEntityModels("Models2\\Prefabs\\01_Beams\\01_Large\\01\\p110a12_solid_beam_junction_x_weak", MyModelsEnum.p110a12_solid_beam_junction_x_weak, MyModelsEnum.p110a12_solid_beam_junction_x_weak_LOD1, MyModelsEnum.p110a12_solid_beam_junction_x_weak_COL);
            AddEntityModels("Models2\\Prefabs\\01_Beams\\01_Large\\01\\p110a13_solid_beam_junction_x_rotated", MyModelsEnum.p110a13_solid_beam_junction_x_rotated, MyModelsEnum.p110a13_solid_beam_junction_x_rotated_LOD1, MyModelsEnum.p110a13_solid_beam_junction_x_rotated_COL);
            AddEntityModels("Models2\\Prefabs\\01_Beams\\01_Large\\01\\p110a21_solid_beam_junction_t_strong", MyModelsEnum.p110a21_solid_beam_junction_t_strong, null, MyModelsEnum.p110a21_solid_beam_junction_t_strong_COL);
            AddEntityModels("Models2\\Prefabs\\01_Beams\\01_Large\\01\\p110a22_solid_beam_junction_t_weak", MyModelsEnum.p110a22_solid_beam_junction_t_weak, MyModelsEnum.p110a22_solid_beam_junction_t_weak_LOD1, MyModelsEnum.p110a22_solid_beam_junction_t_weak_COL);
            AddEntityModels("Models2\\Prefabs\\01_Beams\\01_Large\\01\\p110a23_solid_beam_junction_t_rotated", MyModelsEnum.p110a23_solid_beam_junction_t_rotated, null, MyModelsEnum.p110a23_solid_beam_junction_t_rotated_COL);
            AddEntityModels("Models2\\Prefabs\\01_Beams\\01_Large\\01\\p110a31_solid_beam_joint_horizontal", MyModelsEnum.p110a31_solid_beam_joint_horizontal, null, MyModelsEnum.p110a31_solid_beam_joint_horizontal_COL);
            AddEntityModels("Models2\\Prefabs\\01_Beams\\01_Large\\01\\p110a32_solid_beam_joint_vertical", MyModelsEnum.p110a32_solid_beam_joint_vertical, null, MyModelsEnum.p110a32_solid_beam_joint_vertical_COL);
            AddEntityModels("Models2\\Prefabs\\01_Beams\\01_Large\\01\\p110a33_solid_beam_joint_longitudinal", MyModelsEnum.p110a33_solid_beam_joint_longitudinal, null, MyModelsEnum.p110a33_solid_beam_joint_longitudinal_COL);
            AddEntityModels("Models2\\Prefabs\\01_Beams\\01_Large\\01\\p110a41_solid_beam_superjoint", MyModelsEnum.p110a41_solid_beam_superjoint, null, MyModelsEnum.p110a41_solid_beam_superjoint_COL);

            //////////////////////////////////////////////////////////////////////////
            AddModel(new MyModel("Models2\\Weapons\\WeaponsSmall\\WeaponSmall_Autocannon_barrel", MyMeshDrawTechnique.MESH, MyModelsEnum.Autocannon_Barrel, MyFakes.UNLOAD_OPTIMIZATION_KEEP_COMMON_MODELS));
            AddModel(new MyModel("Models2\\Weapons\\WeaponsSmall\\WeaponSmall_Autocannon_base", MyMeshDrawTechnique.MESH, MyModelsEnum.Autocannon_Base, MyFakes.UNLOAD_OPTIMIZATION_KEEP_COMMON_MODELS));
            AddModel(new MyModel("Models2\\Weapons\\WeaponsSmall\\WeaponSmall_Rifle", MyMeshDrawTechnique.MESH, MyModelsEnum.Rifle, MyFakes.UNLOAD_OPTIMIZATION_KEEP_COMMON_MODELS));
            AddModel(new MyModel("Models2\\Weapons\\WeaponsSmall\\WeaponSmall_Shotgun", MyMeshDrawTechnique.MESH, MyModelsEnum.Shotgun, MyFakes.UNLOAD_OPTIMIZATION_KEEP_COMMON_MODELS));
            AddModel(new MyModel("Models2\\Weapons\\WeaponsSmall\\WeaponSmall_Sniper", MyMeshDrawTechnique.MESH, MyModelsEnum.Sniper, MyFakes.UNLOAD_OPTIMIZATION_KEEP_COMMON_MODELS));
            AddModel(new MyModel("Models2\\Weapons\\WeaponsSmall\\WeaponSmall_UniversalLauncher", MyMeshDrawTechnique.MESH, MyModelsEnum.UniversalLauncher, MyFakes.UNLOAD_OPTIMIZATION_KEEP_COMMON_MODELS));
            AddModel(new MyModel("Models2\\Weapons\\WeaponsSmall\\WeaponSmall_MissileLauncher", MyMeshDrawTechnique.MESH, MyModelsEnum.MissileLauncher, MyFakes.UNLOAD_OPTIMIZATION_KEEP_COMMON_MODELS));
            AddModel(new MyModel("Models2\\Weapons\\WeaponsSmall\\WeaponSmall_MachineGun", MyMeshDrawTechnique.MESH, MyModelsEnum.MachineGun, MyFakes.UNLOAD_OPTIMIZATION_KEEP_COMMON_MODELS));

            AddModel(new MyModel("Models2\\Weapons\\WeaponsLarge\\WeaponLarge_MachineGun_barrel", MyMeshDrawTechnique.MESH, MyModelsEnum.LargeShipMachineGunBarrel, MyFakes.UNLOAD_OPTIMIZATION_KEEP_COMMON_MODELS));
            AddEntityModels("Models2\\Weapons\\WeaponsLarge\\WeaponLarge_MachineGun_base", MyModelsEnum.LargeShipMachineGunBase, null, MyModelsEnum.LargeShipMachineGunBase_COL, true);
            AddModel(new MyModel("Models2\\Weapons\\WeaponsLarge\\WeaponLarge_MissileLauncher_9_barrel", MyMeshDrawTechnique.MESH, MyModelsEnum.LargeShipMissileLauncher9Barrel, MyFakes.UNLOAD_OPTIMIZATION_KEEP_COMMON_MODELS));
            AddEntityModels("Models2\\Weapons\\WeaponsLarge\\WeaponLarge_MissileLauncher_9_base", MyModelsEnum.LargeShipMissileLauncher9Base, null, MyModelsEnum.LargeShipMissileLauncher9Base_COL, true);
            AddModel(new MyModel("Models2\\Weapons\\WeaponsLarge\\WeaponLarge_MissileLauncher_4_barrel", MyMeshDrawTechnique.MESH, MyModelsEnum.LargeShipMissileLauncher4Barrel, MyFakes.UNLOAD_OPTIMIZATION_KEEP_COMMON_MODELS));
            AddEntityModels("Models2\\Weapons\\WeaponsLarge\\WeaponLarge_MissileLauncher_4_base", MyModelsEnum.LargeShipMissileLauncher4Base, null, MyModelsEnum.LargeShipMissileLauncher4Base_COL, true);
            AddModel(new MyModel("Models2\\Weapons\\WeaponsLarge\\WeaponLarge_MissileLauncher_6_barrel", MyMeshDrawTechnique.MESH, MyModelsEnum.LargeShipMissileLauncher6Barrel, MyFakes.UNLOAD_OPTIMIZATION_KEEP_COMMON_MODELS));
            AddEntityModels("Models2\\Weapons\\WeaponsLarge\\WeaponLarge_MissileLauncher_6_base", MyModelsEnum.LargeShipMissileLauncher6Base, null, MyModelsEnum.LargeShipMissileLauncher6Base_COL, true);
            AddModel(new MyModel("Models2\\Weapons\\WeaponsLarge\\WeaponLarge_Ciws_barrel", MyMeshDrawTechnique.MESH, MyModelsEnum.LargeShipCiwsBarrel, MyFakes.UNLOAD_OPTIMIZATION_KEEP_COMMON_MODELS));
            AddEntityModels("Models2\\Weapons\\WeaponsLarge\\WeaponLarge_Ciws_base", MyModelsEnum.LargeShipCiwsBase, null, MyModelsEnum.LargeShipCiwsBase_COL, true);
            AddModel(new MyModel("Models2\\Weapons\\WeaponsLarge\\WeaponLarge_Autocannon_barrel", MyMeshDrawTechnique.MESH, MyModelsEnum.LargeShipAutocannonBarrel, MyFakes.UNLOAD_OPTIMIZATION_KEEP_COMMON_MODELS));
            AddEntityModels("Models2\\Weapons\\WeaponsLarge\\WeaponLarge_Autocannon_base", MyModelsEnum.LargeShipAutocannonBase, null, MyModelsEnum.LargeShipAutocannonBase_COL, true);

            AddModel(new MyModel("Models2\\Weapons\\Projectiles\\Projectile_AsteroidKiller", MyMeshDrawTechnique.MESH, MyModelsEnum.AsteroidKiller, MyFakes.UNLOAD_OPTIMIZATION_KEEP_COMMON_MODELS));
            AddModel(new MyModel("Models2\\Weapons\\Projectiles\\Projectile_DecoyFlare", MyMeshDrawTechnique.MESH, MyModelsEnum.DecoyFlare, MyFakes.UNLOAD_OPTIMIZATION_KEEP_COMMON_MODELS));
            AddModel(new MyModel("Models2\\Weapons\\Projectiles\\Projectile_DirectionalExplosive", MyMeshDrawTechnique.MESH, MyModelsEnum.DirectionalExplosive, MyFakes.UNLOAD_OPTIMIZATION_KEEP_COMMON_MODELS));
            AddModel(new MyModel("Models2\\Weapons\\Projectiles\\Projectile_FlashBomb", MyMeshDrawTechnique.MESH, MyModelsEnum.FlashBomb, MyFakes.UNLOAD_OPTIMIZATION_KEEP_COMMON_MODELS));
            AddModel(new MyModel("Models2\\Weapons\\Projectiles\\Projectile_GravityBomb", MyMeshDrawTechnique.MESH, MyModelsEnum.GravityBomb, MyFakes.UNLOAD_OPTIMIZATION_KEEP_COMMON_MODELS));
            AddModel(new MyModel("Models2\\Weapons\\Projectiles\\Projectile_Hologram", MyMeshDrawTechnique.MESH, MyModelsEnum.Hologram, MyFakes.UNLOAD_OPTIMIZATION_KEEP_COMMON_MODELS));
            AddModel(new MyModel("Models2\\Weapons\\Projectiles\\Projectile_IlluminatingShell", MyMeshDrawTechnique.MESH, MyModelsEnum.IlluminatingShell, MyFakes.UNLOAD_OPTIMIZATION_KEEP_COMMON_MODELS));
            AddModel(new MyModel("Models2\\Weapons\\Projectiles\\Mine", MyMeshDrawTechnique.MESH, MyModelsEnum.MineBasic, MyFakes.UNLOAD_OPTIMIZATION_KEEP_COMMON_MODELS));
            AddModel(new MyModel("Models2\\Weapons\\Projectiles\\Mine_Homing", MyMeshDrawTechnique.MESH, MyModelsEnum.MineSmart, MyFakes.UNLOAD_OPTIMIZATION_KEEP_COMMON_MODELS));
            AddModel(new MyModel("Models2\\Weapons\\Projectiles\\Projectile_Missile01", MyMeshDrawTechnique.MESH, MyModelsEnum.Missile, MyFakes.UNLOAD_OPTIMIZATION_KEEP_COMMON_MODELS));
            AddModel(new MyModel("Models2\\Weapons\\Projectiles\\Projectile_RemoteBomb", MyMeshDrawTechnique.MESH, MyModelsEnum.RemoteBomb, MyFakes.UNLOAD_OPTIMIZATION_KEEP_COMMON_MODELS));
            AddModel(new MyModel("Models2\\Weapons\\Projectiles\\Projectile_RemoteCamera", MyMeshDrawTechnique.MESH, MyModelsEnum.RemoteCamera, MyFakes.UNLOAD_OPTIMIZATION_KEEP_COMMON_MODELS));
            AddModel(new MyModel("Models2\\Weapons\\Projectiles\\Projectile_SmokeBomb", MyMeshDrawTechnique.MESH, MyModelsEnum.SmokeBomb, MyFakes.UNLOAD_OPTIMIZATION_KEEP_COMMON_MODELS));
            AddModel(new MyModel("Models2\\Weapons\\Projectiles\\Projectile_SphereExplosive", MyMeshDrawTechnique.MESH, MyModelsEnum.SphereExplosive, MyFakes.UNLOAD_OPTIMIZATION_KEEP_COMMON_MODELS));
            AddModel(new MyModel("Models2\\Weapons\\Projectiles\\Projectile_TimeBomb", MyMeshDrawTechnique.MESH, MyModelsEnum.TimeBomb, MyFakes.UNLOAD_OPTIMIZATION_KEEP_COMMON_MODELS));
            AddModel(new MyModel("Models2\\Weapons\\Equipment\\DrillCrusher_base", MyMeshDrawTechnique.MESH, MyModelsEnum.Drill_Base, MyFakes.UNLOAD_OPTIMIZATION_KEEP_COMMON_MODELS));
            AddModel(new MyModel("Models2\\Weapons\\Equipment\\DrillCrusher_gear1", MyMeshDrawTechnique.MESH, MyModelsEnum.Drill_Gear1, MyFakes.UNLOAD_OPTIMIZATION_KEEP_COMMON_MODELS));
            AddModel(new MyModel("Models2\\Weapons\\Equipment\\DrillCrusher_gear2", MyMeshDrawTechnique.MESH, MyModelsEnum.Drill_Gear2, MyFakes.UNLOAD_OPTIMIZATION_KEEP_COMMON_MODELS));
            AddModel(new MyModel("Models2\\Weapons\\Equipment\\DrillCrusher_gear3", MyMeshDrawTechnique.MESH, MyModelsEnum.Drill_Gear3, MyFakes.UNLOAD_OPTIMIZATION_KEEP_COMMON_MODELS));
            AddModel(new MyModel("Models2\\Weapons\\Equipment\\DrillLaser", MyMeshDrawTechnique.MESH, MyModelsEnum.LaserDrill));
            AddModel(new MyModel("Models2\\Weapons\\Equipment\\DrillNuclear", MyMeshDrawTechnique.MESH, MyModelsEnum.NuclearDrill));
            AddModel(new MyModel("Models2\\Weapons\\Equipment\\DrillNuclearHead", MyMeshDrawTechnique.MESH, MyModelsEnum.NuclearDrillHead));
            AddModel(new MyModel("Models2\\Weapons\\Equipment\\DrillPressure", MyMeshDrawTechnique.MESH, MyModelsEnum.PressureDrill));
            AddModel(new MyModel("Models2\\Weapons\\Equipment\\DrillSaw", MyMeshDrawTechnique.MESH, MyModelsEnum.SawDrill));
            AddModel(new MyModel("Models2\\Weapons\\Equipment\\DrillThermal", MyMeshDrawTechnique.MESH, MyModelsEnum.ThermalDrill));
            AddModel(new MyModel("Models2\\Weapons\\Equipment\\DrillThermalHead", MyMeshDrawTechnique.MESH, MyModelsEnum.ThermalDrillHead));
            AddModel(new MyModel("Models2\\Weapons\\Equipment\\HarvestingDevice_nozzle", MyMeshDrawTechnique.MESH, MyModelsEnum.HarvestingHead, MyFakes.UNLOAD_OPTIMIZATION_KEEP_COMMON_MODELS));
            AddModel(new MyModel("Models2\\Weapons\\Equipment\\HarvestingDevice_tube", MyMeshDrawTechnique.MESH, MyModelsEnum.HarvestingTube, MyFakes.UNLOAD_OPTIMIZATION_KEEP_COMMON_MODELS));

            AddModel(new MyModel("Models2\\Ships\\Ships\\SmallShip_Jacknife", MyMeshDrawTechnique.MESH, MyModelsEnum.SmallShip_Jacknife));
            //AddModel(new MyModel("Models2\\Ships\\Ships\\SmallShip_Jacknife_LOD1",  MyMeshDrawTechnique.MESH, MyModelsEnum.SmallShip_Jacknife_LOD1));
            AddModel(new MyModel("Models2\\Ships\\Ships\\SmallShip_Doon", MyMeshDrawTechnique.MESH, MyModelsEnum.SmallShip_Doon));
            //AddModel(new MyModel("Models2\\Ships\\Ships\\SmallShip_Doon_LOD1",  MyMeshDrawTechnique.MESH, MyModelsEnum.SmallShip_Doon_LOD1));
            AddModel(new MyModel("Models2\\Ships\\Ships\\SmallShip_Hammer", MyMeshDrawTechnique.MESH, MyModelsEnum.SmallShip_Hammer));
            //AddModel(new MyModel("Models2\\Ships\\Ships\\SmallShip_Hammer_LOD1",  MyMeshDrawTechnique.MESH, MyModelsEnum.SmallShip_Hammer_LOD1));
            AddModel(new MyModel("Models2\\Ships\\Ships\\SmallShip_ORG", MyMeshDrawTechnique.MESH, MyModelsEnum.SmallShip_ORG));
            //AddModel(new MyModel("Models2\\Ships\\Ships\\SmallShip_ORG_LOD1",  MyMeshDrawTechnique.MESH, MyModelsEnum.SmallShip_ORG_LOD1));
            AddModel(new MyModel("Models2\\Ships\\Ships\\SmallShip_ORG_NoPaint", MyMeshDrawTechnique.MESH, MyModelsEnum.SmallShip_ORG_NoPaint));
            //AddModel(new MyModel("Models2\\Ships\\Ships\\SmallShip_ORG_NoPaint_LOD1",  MyMeshDrawTechnique.MESH, MyModelsEnum.SmallShip_ORG_NoPaint_LOD1));
            AddModel(new MyModel("Models2\\Ships\\Ships\\SmallShip_YG_Closed", MyMeshDrawTechnique.MESH, MyModelsEnum.SmallShip_YG_Closed));
            //AddModel(new MyModel("Models2\\Ships\\Ships\\SmallShip_YG_Closed_LOD1",  MyMeshDrawTechnique.MESH, MyModelsEnum.SmallShip_YG_Closed_LOD1));

            AddModel(new MyModel("Models2\\Ships\\Ships\\SmallShip_MinerShip02", MyMeshDrawTechnique.MESH, MyModelsEnum.Enforcer));
            AddModel(new MyModel("Models2\\Ships\\Ships\\SmallShip_MinerShip03", MyMeshDrawTechnique.MESH, MyModelsEnum.Kammler));
            AddModel(new MyModel("Models2\\Ships\\Ships\\SmallShip_MinerShip04", MyMeshDrawTechnique.MESH, MyModelsEnum.Gettysburg));
            AddModel(new MyModel("Models2\\Ships\\Ships\\SmallShip_MinerShip10", MyMeshDrawTechnique.MESH, MyModelsEnum.Tracer));
            AddModel(new MyModel("Models2\\Ships\\Cockpits\\Cockpit_Default", MyMeshDrawTechnique.MESH, MyModelsEnum.MinerShip_Generic_CockpitInterior));
            AddModel(new MyModel("Models2\\Ships\\Cockpits\\Cockpit_Default_glass", MyMeshDrawTechnique.MESH, MyModelsEnum.MinerShip_Generic_CockpitGlass));
            AddModel(new MyModel("Models2\\ObjectsStatic\\Debris\\DebrisField01", MyMeshDrawTechnique.MESH, MyModelsEnum.DebrisField));
            AddModel(new MyModel("Models2\\ObjectsFloating\\Container01a_Red", MyMeshDrawTechnique.MESH, MyModelsEnum.Standard_Container_1, MyFakes.UNLOAD_OPTIMIZATION_KEEP_COMMON_MODELS));
            AddModel(new MyModel("Models2\\ObjectsFloating\\Container01b_Blue", MyMeshDrawTechnique.MESH, MyModelsEnum.Standard_Container_2, MyFakes.UNLOAD_OPTIMIZATION_KEEP_COMMON_MODELS));
            AddModel(new MyModel("Models2\\ObjectsFloating\\Container01c_Grey", MyMeshDrawTechnique.MESH, MyModelsEnum.Standard_Container_3, MyFakes.UNLOAD_OPTIMIZATION_KEEP_COMMON_MODELS));
            AddModel(new MyModel("Models2\\ObjectsFloating\\Container01d_White", MyMeshDrawTechnique.MESH, MyModelsEnum.Standard_Container_4, MyFakes.UNLOAD_OPTIMIZATION_KEEP_COMMON_MODELS));
            AddModel(new MyModel("Models2\\ObjectsFloating\\RockDebris01", MyMeshDrawTechnique.MESH, MyModelsEnum.ExplosionDebrisVoxel, MyFakes.UNLOAD_OPTIMIZATION_KEEP_COMMON_MODELS));
            AddModel(new MyModel("Models2\\ObjectsFloating\\Tank01", MyMeshDrawTechnique.MESH, MyModelsEnum.cistern, MyFakes.UNLOAD_OPTIMIZATION_KEEP_COMMON_MODELS));
            AddModel(new MyModel("Models2\\ObjectsFloating\\TubeBundle01", MyMeshDrawTechnique.MESH, MyModelsEnum.pipe_bundle, MyFakes.UNLOAD_OPTIMIZATION_KEEP_COMMON_MODELS));
            AddModel(new MyModel("Models2\\ObjectsFloating\\UtilityVehicle01", MyMeshDrawTechnique.MESH, MyModelsEnum.UtilityVehicle_1, MyFakes.UNLOAD_OPTIMIZATION_KEEP_COMMON_MODELS));
            AddModel(new MyModel("Models2\\ObjectsFloating\\Debris\\Debris01", MyMeshDrawTechnique.MESH, MyModelsEnum.Debris1, MyFakes.UNLOAD_OPTIMIZATION_KEEP_COMMON_MODELS));
            AddModel(new MyModel("Models2\\ObjectsFloating\\Debris\\Debris02", MyMeshDrawTechnique.MESH, MyModelsEnum.Debris2, MyFakes.UNLOAD_OPTIMIZATION_KEEP_COMMON_MODELS));
            AddModel(new MyModel("Models2\\ObjectsFloating\\Debris\\Debris03", MyMeshDrawTechnique.MESH, MyModelsEnum.Debris3, MyFakes.UNLOAD_OPTIMIZATION_KEEP_COMMON_MODELS));
            AddModel(new MyModel("Models2\\ObjectsFloating\\Debris\\Debris04", MyMeshDrawTechnique.MESH, MyModelsEnum.Debris4, MyFakes.UNLOAD_OPTIMIZATION_KEEP_COMMON_MODELS));
            AddModel(new MyModel("Models2\\ObjectsFloating\\Debris\\Debris05", MyMeshDrawTechnique.MESH, MyModelsEnum.Debris5, MyFakes.UNLOAD_OPTIMIZATION_KEEP_COMMON_MODELS));
            AddModel(new MyModel("Models2\\ObjectsFloating\\Debris\\Debris06", MyMeshDrawTechnique.MESH, MyModelsEnum.Debris6, MyFakes.UNLOAD_OPTIMIZATION_KEEP_COMMON_MODELS));
            AddModel(new MyModel("Models2\\ObjectsFloating\\Debris\\Debris07", MyMeshDrawTechnique.MESH, MyModelsEnum.Debris7, MyFakes.UNLOAD_OPTIMIZATION_KEEP_COMMON_MODELS));
            AddModel(new MyModel("Models2\\ObjectsFloating\\Debris\\Debris08", MyMeshDrawTechnique.MESH, MyModelsEnum.Debris8, MyFakes.UNLOAD_OPTIMIZATION_KEEP_COMMON_MODELS));
            AddModel(new MyModel("Models2\\ObjectsFloating\\Debris\\Debris09", MyMeshDrawTechnique.MESH, MyModelsEnum.Debris9, MyFakes.UNLOAD_OPTIMIZATION_KEEP_COMMON_MODELS));
            AddModel(new MyModel("Models2\\ObjectsFloating\\Debris\\Debris10", MyMeshDrawTechnique.MESH, MyModelsEnum.Debris10, MyFakes.UNLOAD_OPTIMIZATION_KEEP_COMMON_MODELS));
            AddModel(new MyModel("Models2\\ObjectsFloating\\Debris\\Debris11", MyMeshDrawTechnique.MESH, MyModelsEnum.Debris11, MyFakes.UNLOAD_OPTIMIZATION_KEEP_COMMON_MODELS));
            AddModel(new MyModel("Models2\\ObjectsFloating\\Debris\\Debris12", MyMeshDrawTechnique.MESH, MyModelsEnum.Debris12, MyFakes.UNLOAD_OPTIMIZATION_KEEP_COMMON_MODELS));
            AddModel(new MyModel("Models2\\ObjectsFloating\\Debris\\Debris13", MyMeshDrawTechnique.MESH, MyModelsEnum.Debris13, MyFakes.UNLOAD_OPTIMIZATION_KEEP_COMMON_MODELS));
            AddModel(new MyModel("Models2\\ObjectsFloating\\Debris\\Debris14", MyMeshDrawTechnique.MESH, MyModelsEnum.Debris14, MyFakes.UNLOAD_OPTIMIZATION_KEEP_COMMON_MODELS));
            AddModel(new MyModel("Models2\\ObjectsFloating\\Debris\\Debris15", MyMeshDrawTechnique.MESH, MyModelsEnum.Debris15, MyFakes.UNLOAD_OPTIMIZATION_KEEP_COMMON_MODELS));
            AddModel(new MyModel("Models2\\ObjectsFloating\\Debris\\Debris16", MyMeshDrawTechnique.MESH, MyModelsEnum.Debris16, MyFakes.UNLOAD_OPTIMIZATION_KEEP_COMMON_MODELS));
            AddModel(new MyModel("Models2\\ObjectsFloating\\Debris\\Debris17", MyMeshDrawTechnique.MESH, MyModelsEnum.Debris17, MyFakes.UNLOAD_OPTIMIZATION_KEEP_COMMON_MODELS));
            AddModel(new MyModel("Models2\\ObjectsFloating\\Debris\\Debris18", MyMeshDrawTechnique.MESH, MyModelsEnum.Debris18, MyFakes.UNLOAD_OPTIMIZATION_KEEP_COMMON_MODELS));
            AddModel(new MyModel("Models2\\ObjectsFloating\\Debris\\Debris19", MyMeshDrawTechnique.MESH, MyModelsEnum.Debris19, MyFakes.UNLOAD_OPTIMIZATION_KEEP_COMMON_MODELS));
            AddModel(new MyModel("Models2\\ObjectsFloating\\Debris\\Debris20", MyMeshDrawTechnique.MESH, MyModelsEnum.Debris20, MyFakes.UNLOAD_OPTIMIZATION_KEEP_COMMON_MODELS));
            AddModel(new MyModel("Models2\\ObjectsFloating\\Debris\\Debris21", MyMeshDrawTechnique.MESH, MyModelsEnum.Debris21, MyFakes.UNLOAD_OPTIMIZATION_KEEP_COMMON_MODELS));
            AddModel(new MyModel("Models2\\ObjectsFloating\\Debris\\Debris22", MyMeshDrawTechnique.MESH, MyModelsEnum.Debris22, MyFakes.UNLOAD_OPTIMIZATION_KEEP_COMMON_MODELS));
            AddModel(new MyModel("Models2\\ObjectsFloating\\Debris\\Debris23", MyMeshDrawTechnique.MESH, MyModelsEnum.Debris23, MyFakes.UNLOAD_OPTIMIZATION_KEEP_COMMON_MODELS));
            AddModel(new MyModel("Models2\\ObjectsFloating\\Debris\\Debris24", MyMeshDrawTechnique.MESH, MyModelsEnum.Debris24, MyFakes.UNLOAD_OPTIMIZATION_KEEP_COMMON_MODELS));
            AddModel(new MyModel("Models2\\ObjectsFloating\\Debris\\Debris25", MyMeshDrawTechnique.MESH, MyModelsEnum.Debris25, MyFakes.UNLOAD_OPTIMIZATION_KEEP_COMMON_MODELS));
            AddModel(new MyModel("Models2\\ObjectsFloating\\Debris\\Debris26", MyMeshDrawTechnique.MESH, MyModelsEnum.Debris26, MyFakes.UNLOAD_OPTIMIZATION_KEEP_COMMON_MODELS));
            AddModel(new MyModel("Models2\\ObjectsFloating\\Debris\\Debris27", MyMeshDrawTechnique.MESH, MyModelsEnum.Debris27, MyFakes.UNLOAD_OPTIMIZATION_KEEP_COMMON_MODELS));
            AddModel(new MyModel("Models2\\ObjectsFloating\\Debris\\Debris28", MyMeshDrawTechnique.MESH, MyModelsEnum.Debris28, MyFakes.UNLOAD_OPTIMIZATION_KEEP_COMMON_MODELS));
            AddModel(new MyModel("Models2\\ObjectsFloating\\Debris\\Debris29", MyMeshDrawTechnique.MESH, MyModelsEnum.Debris29, MyFakes.UNLOAD_OPTIMIZATION_KEEP_COMMON_MODELS));
            AddModel(new MyModel("Models2\\ObjectsFloating\\Debris\\Debris30", MyMeshDrawTechnique.MESH, MyModelsEnum.Debris30, MyFakes.UNLOAD_OPTIMIZATION_KEEP_COMMON_MODELS));
            AddModel(new MyModel("Models2\\ObjectsFloating\\Debris\\Debris31", MyMeshDrawTechnique.MESH, MyModelsEnum.Debris31, MyFakes.UNLOAD_OPTIMIZATION_KEEP_COMMON_MODELS));
            AddModel(new MyModel("Models2\\ObjectsFloating\\Debris\\Debris32_Pilot", MyMeshDrawTechnique.MESH, MyModelsEnum.Debris32_pilot, MyFakes.UNLOAD_OPTIMIZATION_KEEP_COMMON_MODELS));
            AddModel(new MyModel("Models2\\Editor\\EditorGizmo_Rotation", MyMeshDrawTechnique.MESH, MyModelsEnum.GizmoRotation, MyFakes.UNLOAD_OPTIMIZATION_KEEP_COMMON_MODELS));
            AddModel(new MyModel("Models2\\Editor\\EditorGizmo_Translation", MyMeshDrawTechnique.MESH, MyModelsEnum.GizmoTranslation, MyFakes.UNLOAD_OPTIMIZATION_KEEP_COMMON_MODELS));

            AddModel(new MyModel("Models2\\Ships\\Cockpits\\Cockpit_EAC_02", MyMeshDrawTechnique.MESH, MyModelsEnum.EAC02_Cockpit));
            AddModel(new MyModel("Models2\\Ships\\Cockpits\\Cockpit_EAC_02_glass", MyMeshDrawTechnique.MESH, MyModelsEnum.EAC02_Cockpit_glass));
            AddModel(new MyModel("Models2\\Ships\\Cockpits\\Cockpit_EAC_03", MyMeshDrawTechnique.MESH, MyModelsEnum.EAC03_Cockpit));
            AddModel(new MyModel("Models2\\Ships\\Cockpits\\Cockpit_EAC_03_glass", MyMeshDrawTechnique.MESH, MyModelsEnum.EAC03_Cockpit_glass));
            AddModel(new MyModel("Models2\\Ships\\Cockpits\\Cockpit_EAC_04", MyMeshDrawTechnique.MESH, MyModelsEnum.EAC04_Cockpit));
            AddModel(new MyModel("Models2\\Ships\\Cockpits\\Cockpit_EAC_04_glass", MyMeshDrawTechnique.MESH, MyModelsEnum.EAC04_Cockpit_glass));
            AddModel(new MyModel("Models2\\Ships\\Cockpits\\Cockpit_EAC_05", MyMeshDrawTechnique.MESH, MyModelsEnum.EAC05_Cockpit));
            AddModel(new MyModel("Models2\\Ships\\Cockpits\\Cockpit_EAC_05_glass", MyMeshDrawTechnique.MESH, MyModelsEnum.EAC05_Cockpit_glass));

            AddModel(new MyModel("Models2\\Ships\\Cockpits\\Cockpit_OmniCorp_01", MyMeshDrawTechnique.MESH, MyModelsEnum.OmniCorp01_Cockpit));
            AddModel(new MyModel("Models2\\Ships\\Cockpits\\Cockpit_OmniCorp_01_glass", MyMeshDrawTechnique.MESH, MyModelsEnum.OmniCorp01_Cockpit_glass));
            AddModel(new MyModel("Models2\\Ships\\Cockpits\\Cockpit_OmniCorp_03", MyMeshDrawTechnique.MESH, MyModelsEnum.OmniCorp03_Cockpit));
            AddModel(new MyModel("Models2\\Ships\\Cockpits\\Cockpit_OmniCorp_03_glass", MyMeshDrawTechnique.MESH, MyModelsEnum.OmniCorp03_Cockpit_glass));
            AddModel(new MyModel("Models2\\Ships\\Cockpits\\Cockpit_OmniCorp_04", MyMeshDrawTechnique.MESH, MyModelsEnum.OmniCorp04_Cockpit));
            AddModel(new MyModel("Models2\\Ships\\Cockpits\\Cockpit_OmniCorp_04_glass", MyMeshDrawTechnique.MESH, MyModelsEnum.OmniCorp04_Cockpit_glass));
            AddModel(new MyModel("Models2\\Ships\\Cockpits\\Cockpit_OmniCorp_EAC_01", MyMeshDrawTechnique.MESH, MyModelsEnum.OmniCorp_EAC01_Cockpit));
            AddModel(new MyModel("Models2\\Ships\\Cockpits\\Cockpit_OmniCorp_EAC_glass_01", MyMeshDrawTechnique.MESH, MyModelsEnum.OmniCorp_EAC01_Cockpit_glass));
        
            AddModel(new MyModel("Models2\\Ships\\Cockpits\\Cockpit_CN_03", MyMeshDrawTechnique.MESH, MyModelsEnum.Cockpit_CN_03));
            AddModel(new MyModel("Models2\\Ships\\Cockpits\\Cockpit_CN_03_glass", MyMeshDrawTechnique.MESH, MyModelsEnum.Cockpit_CN_03_glass));

            AddModel(new MyModel("Models2\\Ships\\Cockpits\\Cockpit_SS_04", MyMeshDrawTechnique.MESH, MyModelsEnum.Cockpit_SS_04));
            AddModel(new MyModel("Models2\\Ships\\Cockpits\\Cockpit_SS_04_glass", MyMeshDrawTechnique.MESH, MyModelsEnum.Cockpit_SS_04_glass));
            AddModel(new MyModel("Models2\\Ships\\Cockpits\\Cockpit_Razorclaw", MyMeshDrawTechnique.MESH, MyModelsEnum.Cockpit_Razorclaw));
            AddModel(new MyModel("Models2\\Ships\\Cockpits\\Cockpit_Razorclaw_glass", MyMeshDrawTechnique.MESH, MyModelsEnum.Cockpit_Razorclaw_glass));

            AddModel(new MyModel("Models2\\Ships\\Ships\\SmallShip_MinerShip01", MyMeshDrawTechnique.MESH, MyModelsEnum.Liberator));
            AddModel(new MyModel("Models2\\Ships\\Ships\\SmallShip_MinerShip05", MyMeshDrawTechnique.MESH, MyModelsEnum.Virginia));
            AddModel(new MyModel("Models2\\Ships\\Ships\\SmallShip_MinerShip06", MyMeshDrawTechnique.MESH, MyModelsEnum.Baer));
            AddModel(new MyModel("Models2\\Ships\\Ships\\SmallShip_MinerShip07", MyMeshDrawTechnique.MESH, MyModelsEnum.Hewer));
            AddModel(new MyModel("Models2\\Ships\\Ships\\SmallShip_MinerShip08", MyMeshDrawTechnique.MESH, MyModelsEnum.Razorclaw));
            AddModel(new MyModel("Models2\\Ships\\Ships\\SmallShip_MinerShip09", MyMeshDrawTechnique.MESH, MyModelsEnum.Greiser));
            AddModel(new MyModel("Models2\\Ships\\Ships\\SmallShip_Hawk", MyMeshDrawTechnique.MESH, MyModelsEnum.SmallShip_Hawk));
            AddModel(new MyModel("Models2\\Ships\\Ships\\SmallShip_Phoenix", MyMeshDrawTechnique.MESH, MyModelsEnum.SmallShip_Phoenix));
            AddModel(new MyModel("Models2\\Ships\\Ships\\SmallShip_Leviathan", MyMeshDrawTechnique.MESH, MyModelsEnum.SmallShip_Leviathan));
            AddModel(new MyModel("Models2\\Ships\\Ships\\SmallShip_Rockheater", MyMeshDrawTechnique.MESH, MyModelsEnum.SmallShip_Rockheater));
            AddModel(new MyModel("Models2\\Ships\\Ships\\SmallShip_SteelHead", MyMeshDrawTechnique.MESH, MyModelsEnum.SmallShip_SteelHead));
            AddModel(new MyModel("Models2\\Ships\\Ships\\SmallShip_Talon", MyMeshDrawTechnique.MESH, MyModelsEnum.SmallShip_Talon));
            AddModel(new MyModel("Models2\\Ships\\Ships\\SmallShip_Stanislav", MyMeshDrawTechnique.MESH, MyModelsEnum.SmallShip_Stanislav));

            //Debug models
            AddModel(new MyModel("Models2\\Debug\\plane_10_50", MyMeshDrawTechnique.MESH, MyModelsEnum.plane_10_50));
            AddModel(new MyModel("Models2\\Debug\\plane_50_20", MyMeshDrawTechnique.MESH, MyModelsEnum.plane_50_20));
            AddModel(new MyModel("Models2\\Debug\\plane_100_10", MyMeshDrawTechnique.MESH, MyModelsEnum.plane_100_10));
            AddModel(new MyModel("Models2\\Debug\\plane_128_70", MyMeshDrawTechnique.MESH, MyModelsEnum.plane_128_70));
            AddModel(new MyModel("Models2\\Debug\\plane_100_50", MyMeshDrawTechnique.MESH, MyModelsEnum.plane_100_50));
            AddModel(new MyModel("Models2\\Debug\\plane_150_50", MyMeshDrawTechnique.MESH, MyModelsEnum.plane_150_50));
            AddModel(new MyModel("Models2\\Debug\\plane_255_70", MyMeshDrawTechnique.MESH, MyModelsEnum.plane_255_70));
            AddModel(new MyModel("Models2\\Debug\\plane_300_70", MyMeshDrawTechnique.MESH, MyModelsEnum.plane_300_70));
            AddModel(new MyModel("Models2\\Debug\\plane_800_10", MyMeshDrawTechnique.MESH, MyModelsEnum.plane_800_10));
            AddModel(new MyModel("Models2\\Debug\\plane_800_70", MyMeshDrawTechnique.MESH, MyModelsEnum.plane_800_70));
            AddModel(new MyModel("Models2\\Debug\\Sphere_smooth", MyMeshDrawTechnique.MESH, MyModelsEnum.sphere_smooth));

            AddModel(new MyModel("Models2\\Debug\\BoxHiRes", MyMeshDrawTechnique.MESH, MyModelsEnum.BoxHiRes));
            AddModel(new MyModel("Models2\\Debug\\BoxLowRes", MyMeshDrawTechnique.MESH, MyModelsEnum.BoxLowRes));
            AddModel(new MyModel("Models2\\Debug\\Sphere", MyMeshDrawTechnique.MESH, MyModelsEnum.Sphere));
            AddModel(new MyModel("Models2\\Debug\\Sphere_low", MyMeshDrawTechnique.MESH, MyModelsEnum.Sphere_low));
            AddModel(new MyModel("Models2\\Debug\\Cone", MyMeshDrawTechnique.MESH, MyModelsEnum.Cone));
            AddModel(new MyModel("Models2\\Debug\\Hemisphere", MyMeshDrawTechnique.MESH, MyModelsEnum.Hemisphere));
            AddModel(new MyModel("Models2\\Debug\\Hemisphere_low", MyMeshDrawTechnique.MESH, MyModelsEnum.Hemisphere_low));
            AddModel(new MyModel("Models2\\Debug\\Capsule", MyMeshDrawTechnique.MESH, MyModelsEnum.Capsule));


            AddEntityModels("Models2\\Prefabs\\05_Details\\02_Lights\\default_light_0", MyModelsEnum.default_light_0, null, MyModelsEnum.default_light_0_COL);
            AddEntityModels("Models2\\Prefabs\\05_Details\\02_Lights\\p521_a01_light1", MyModelsEnum.p521_a01_light1, null, MyModelsEnum.p521_a01_light1_COL);
            AddEntityModels("Models2\\Prefabs\\05_Details\\02_Lights\\p521_a02_light2", MyModelsEnum.p521_a02_light2, null, MyModelsEnum.p521_a02_light2_COL);
            AddEntityModels("Models2\\Prefabs\\05_Details\\02_Lights\\p521_a03_light3", MyModelsEnum.p521_a03_light3, null, MyModelsEnum.p521_a03_light3_COL);
            AddEntityModels("Models2\\Prefabs\\05_Details\\02_Lights\\p521_a04_light4", MyModelsEnum.p521_a04_light4, null, MyModelsEnum.p521_a04_light4_COL);

            AddModel(new MyModel("Models2\\Prefabs\\05_Details\\05_Particles\\default_particles_0", MyMeshDrawTechnique.MESH, MyModelsEnum.default_particlesprefab_0));
            AddModel(new MyModel("Models2\\Prefabs\\05_Details\\05_Particles\\p551_a01_particles", MyMeshDrawTechnique.MESH, MyModelsEnum.p551_a01_particles));
            AddModel(new MyModel("Models2\\Prefabs\\05_Details\\05_Particles\\p551_b01_particles", MyMeshDrawTechnique.MESH, MyModelsEnum.p551_b01_particles));
            AddModel(new MyModel("Models2\\Prefabs\\05_Details\\05_Particles\\p551_c01_particles", MyMeshDrawTechnique.MESH, MyModelsEnum.p551_c01_particles));
            AddModel(new MyModel("Models2\\Prefabs\\05_Details\\05_Particles\\p551_d01_particles", MyMeshDrawTechnique.MESH, MyModelsEnum.p551_d01_particles));

            AddModel(new MyModel("Models2\\Prefabs\\05_Details\\06_Sounds\\default_sound_0", MyMeshDrawTechnique.MESH, MyModelsEnum.default_soundprefab_0));
            AddModel(new MyModel("Models2\\Prefabs\\05_Details\\06_Sounds\\p561_a01_sound", MyMeshDrawTechnique.MESH, MyModelsEnum.p561_a01_sound));
            AddModel(new MyModel("Models2\\Prefabs\\05_Details\\06_Sounds\\p561_b01_sound", MyMeshDrawTechnique.MESH, MyModelsEnum.p561_b01_sound));
            AddModel(new MyModel("Models2\\Prefabs\\05_Details\\06_Sounds\\p561_c01_sound", MyMeshDrawTechnique.MESH, MyModelsEnum.p561_c01_sound));
            AddModel(new MyModel("Models2\\Prefabs\\05_Details\\06_Sounds\\p561_d01_sound", MyMeshDrawTechnique.MESH, MyModelsEnum.p561_d01_sound));

            AddEntityModels("Models2\\Prefabs\\05_Details\\01_Billboards\\511_a01_billboard", MyModelsEnum.p511_a01_billboard, MyModelsEnum.p511_a01_billboard_LOD1, MyModelsEnum.p511_a01_billboard_COL);
            AddEntityModels("Models2\\Prefabs\\05_Details\\01_Billboards\\511_a02_billboard", MyModelsEnum.p511_a02_billboard, MyModelsEnum.p511_a02_billboard_LOD1, MyModelsEnum.p511_a02_billboard_COL);
            AddEntityModels("Models2\\Prefabs\\05_Details\\01_Billboards\\511_a03_billboard", MyModelsEnum.p511_a03_billboard, MyModelsEnum.p511_a03_billboard_LOD1, MyModelsEnum.p511_a03_billboard_COL);
            AddEntityModels("Models2\\Prefabs\\05_Details\\01_Billboards\\511_a04_billboard", MyModelsEnum.p511_a04_billboard, MyModelsEnum.p511_a04_billboard_LOD1, MyModelsEnum.p511_a04_billboard_COL);
            AddEntityModels("Models2\\Prefabs\\05_Details\\01_Billboards\\511_a05_billboard", MyModelsEnum.p511_a05_billboard, MyModelsEnum.p511_a05_billboard_LOD1, MyModelsEnum.p511_a05_billboard_COL);
            AddEntityModels("Models2\\Prefabs\\05_Details\\01_Billboards\\511_a06_billboard", MyModelsEnum.p511_a06_billboard, MyModelsEnum.p511_a06_billboard_LOD1, MyModelsEnum.p511_a06_billboard_COL);
            AddEntityModels("Models2\\Prefabs\\05_Details\\01_Billboards\\511_a07_billboard", MyModelsEnum.p511_a07_billboard, MyModelsEnum.p511_a07_billboard_LOD1, MyModelsEnum.p511_a07_billboard_COL);
            AddEntityModels("Models2\\Prefabs\\05_Details\\01_Billboards\\511_a08_billboard", MyModelsEnum.p511_a08_billboard, MyModelsEnum.p511_a08_billboard_LOD1, MyModelsEnum.p511_a08_billboard_COL);
            AddEntityModels("Models2\\Prefabs\\05_Details\\01_Billboards\\511_a09_billboard", MyModelsEnum.p511_a09_billboard, MyModelsEnum.p511_a09_billboard_LOD1, MyModelsEnum.p511_a09_billboard_COL);
            AddEntityModels("Models2\\Prefabs\\05_Details\\01_Billboards\\511_a10_billboard", MyModelsEnum.p511_a10_billboard, MyModelsEnum.p511_a10_billboard_LOD1, MyModelsEnum.p511_a10_billboard_COL);
            AddEntityModels("Models2\\Prefabs\\05_Details\\01_Billboards\\511_a11_billboard", MyModelsEnum.p511_a11_billboard, MyModelsEnum.p511_a11_billboard_LOD1, MyModelsEnum.p511_a11_billboard_COL);
            AddEntityModels("Models2\\Prefabs\\05_Details\\01_Billboards\\511_a12_billboard", MyModelsEnum.p511_a12_billboard, MyModelsEnum.p511_a12_billboard_LOD1, MyModelsEnum.p511_a12_billboard_COL);
            AddEntityModels("Models2\\Prefabs\\05_Details\\01_Billboards\\511_a14_billboard", MyModelsEnum.p511_a14_billboard, MyModelsEnum.p511_a14_billboard_LOD1, MyModelsEnum.p511_a14_billboard_COL);
            AddEntityModels("Models2\\Prefabs\\05_Details\\01_Billboards\\511_a15_billboard", MyModelsEnum.p511_a15_billboard, MyModelsEnum.p511_a15_billboard_LOD1, MyModelsEnum.p511_a15_billboard_COL);
            AddEntityModels("Models2\\Prefabs\\05_Details\\01_Billboards\\511_a16_billboard", MyModelsEnum.p511_a16_billboard, MyModelsEnum.p511_a16_billboard_LOD1, MyModelsEnum.p511_a16_billboard_COL);

            AddModel(new MyModel("Models2\\Prefabs\\05_Details\\03_Signs\\531_a01_sign1", MyMeshDrawTechnique.MESH, MyModelsEnum.p531_a01_sign1));
            AddModel(new MyModel("Models2\\Prefabs\\05_Details\\03_Signs\\531_a02_sign2", MyMeshDrawTechnique.MESH, MyModelsEnum.p531_a02_sign2));
            AddModel(new MyModel("Models2\\Prefabs\\05_Details\\03_Signs\\531_a03_sign3", MyMeshDrawTechnique.MESH, MyModelsEnum.p531_a03_sign3));
            AddModel(new MyModel("Models2\\Prefabs\\05_Details\\03_Signs\\531_a04_sign4", MyMeshDrawTechnique.MESH, MyModelsEnum.p531_a04_sign4));
            AddModel(new MyModel("Models2\\Prefabs\\05_Details\\03_Signs\\531_a05_sign5", MyMeshDrawTechnique.MESH, MyModelsEnum.p531_a05_sign5));
            AddModel(new MyModel("Models2\\Prefabs\\05_Details\\03_Signs\\531_a06_sign6", MyMeshDrawTechnique.MESH, MyModelsEnum.p531_a06_sign6));
            AddModel(new MyModel("Models2\\Prefabs\\05_Details\\03_Signs\\531_a07_sign7", MyMeshDrawTechnique.MESH, MyModelsEnum.p531_a07_sign7));
            AddModel(new MyModel("Models2\\Prefabs\\05_Details\\03_Signs\\531_a08_sign8", MyMeshDrawTechnique.MESH, MyModelsEnum.p531_a08_sign8));
            AddModel(new MyModel("Models2\\Prefabs\\05_Details\\03_Signs\\531_a09_sign9", MyMeshDrawTechnique.MESH, MyModelsEnum.p531_a09_sign9));
            AddModel(new MyModel("Models2\\Prefabs\\05_Details\\03_Signs\\531_a10_sign10", MyMeshDrawTechnique.MESH, MyModelsEnum.p531_a10_sign10));
            AddModel(new MyModel("Models2\\Prefabs\\05_Details\\03_Signs\\531_a11_sign11", MyMeshDrawTechnique.MESH, MyModelsEnum.p531_a11_sign11));
            AddModel(new MyModel("Models2\\Prefabs\\05_Details\\03_Signs\\531_a12_sign12", MyMeshDrawTechnique.MESH, MyModelsEnum.p531_a12_sign12));

            AddModel(new MyModel("Models2\\Prefabs\\05_Details\\03_Signs\\531_c_administrative_area", MyMeshDrawTechnique.MESH, MyModelsEnum.p531_c_administrative_area));
            AddModel(new MyModel("Models2\\Prefabs\\05_Details\\03_Signs\\531_c_armory", MyMeshDrawTechnique.MESH, MyModelsEnum.p531_c_armory));
            AddModel(new MyModel("Models2\\Prefabs\\05_Details\\03_Signs\\531_c_arrow_L", MyMeshDrawTechnique.MESH, MyModelsEnum.p531_c_arrow_L));
            AddModel(new MyModel("Models2\\Prefabs\\05_Details\\03_Signs\\531_c_arrow_R", MyMeshDrawTechnique.MESH, MyModelsEnum.p531_c_arrow_R));
            AddModel(new MyModel("Models2\\Prefabs\\05_Details\\03_Signs\\531_c_arrow_str", MyMeshDrawTechnique.MESH, MyModelsEnum.p531_c_arrow_str));
            AddModel(new MyModel("Models2\\Prefabs\\05_Details\\03_Signs\\531_c_cargo_bay", MyMeshDrawTechnique.MESH, MyModelsEnum.p531_c_cargo_bay));
            AddModel(new MyModel("Models2\\Prefabs\\05_Details\\03_Signs\\531_c_command_center", MyMeshDrawTechnique.MESH, MyModelsEnum.p531_c_command_center));
            AddModel(new MyModel("Models2\\Prefabs\\05_Details\\03_Signs\\531_c_commercial_area", MyMeshDrawTechnique.MESH, MyModelsEnum.p531_c_commercial_area));
            AddModel(new MyModel("Models2\\Prefabs\\05_Details\\03_Signs\\531_c_communications", MyMeshDrawTechnique.MESH, MyModelsEnum.p531_c_communications));
            AddModel(new MyModel("Models2\\Prefabs\\05_Details\\03_Signs\\531_c_defenses", MyMeshDrawTechnique.MESH, MyModelsEnum.p531_c_defenses));
            AddEntityModels("Models2\\Prefabs\\05_Details\\03_Signs\\531_c_docks", MyModelsEnum.p531_c_docks, null, MyModelsEnum.p531_c_docks_COL);
            AddModel(new MyModel("Models2\\Prefabs\\05_Details\\03_Signs\\531_c_emergency_exit", MyMeshDrawTechnique.MESH, MyModelsEnum.p531_c_emergency_exit));
            AddEntityModels("Models2\\Prefabs\\05_Details\\03_Signs\\531_c_engineering_area", MyModelsEnum.p531_c_engineering_area, null, MyModelsEnum.p531_c_engineering_area_COL);
            AddModel(new MyModel("Models2\\Prefabs\\05_Details\\03_Signs\\531_c_exit", MyMeshDrawTechnique.MESH, MyModelsEnum.p531_c_exit));
            AddModel(new MyModel("Models2\\Prefabs\\05_Details\\03_Signs\\531_c_experimental_labs", MyMeshDrawTechnique.MESH, MyModelsEnum.p531_c_experimental_labs));
            AddModel(new MyModel("Models2\\Prefabs\\05_Details\\03_Signs\\531_c_foundry", MyMeshDrawTechnique.MESH, MyModelsEnum.p531_c_foundry));
            AddEntityModels("Models2\\Prefabs\\05_Details\\03_Signs\\531_c_habitats", MyModelsEnum.p531_c_habitats, null, MyModelsEnum.p531_c_habitats_COL);
            AddEntityModels("Models2\\Prefabs\\05_Details\\03_Signs\\531_c_hangars", MyModelsEnum.p531_c_hangars, null, MyModelsEnum.p531_c_hangars_COL);
            AddModel(new MyModel("Models2\\Prefabs\\05_Details\\03_Signs\\531_c_industrial_area", MyMeshDrawTechnique.MESH, MyModelsEnum.p531_c_industrial_area));
            AddModel(new MyModel("Models2\\Prefabs\\05_Details\\03_Signs\\531_c_landing_bay", MyMeshDrawTechnique.MESH, MyModelsEnum.p531_c_landing_bay));
            AddEntityModels("Models2\\Prefabs\\05_Details\\03_Signs\\531_c_maintenance", MyModelsEnum.p531_c_maintenance, null, MyModelsEnum.p531_c_maintenance_COL);
            AddModel(new MyModel("Models2\\Prefabs\\05_Details\\03_Signs\\531_c_military_area", MyMeshDrawTechnique.MESH, MyModelsEnum.p531_c_military_area));
            AddModel(new MyModel("Models2\\Prefabs\\05_Details\\03_Signs\\531_c_mines", MyMeshDrawTechnique.MESH, MyModelsEnum.p531_c_mines));
            AddModel(new MyModel("Models2\\Prefabs\\05_Details\\03_Signs\\531_c_ore_processing", MyMeshDrawTechnique.MESH, MyModelsEnum.p531_c_ore_processing));
            AddModel(new MyModel("Models2\\Prefabs\\05_Details\\03_Signs\\531_c_outer_area", MyMeshDrawTechnique.MESH, MyModelsEnum.p531_c_outer_area));
            AddModel(new MyModel("Models2\\Prefabs\\05_Details\\03_Signs\\531_c_prison", MyMeshDrawTechnique.MESH, MyModelsEnum.p531_c_prison));
            AddModel(new MyModel("Models2\\Prefabs\\05_Details\\03_Signs\\531_c_public_area", MyMeshDrawTechnique.MESH, MyModelsEnum.p531_c_public_area));
            AddEntityModels("Models2\\Prefabs\\05_Details\\03_Signs\\531_c_reactor", MyModelsEnum.p531_c_reactor, null, MyModelsEnum.p531_c_reactor_COL);
            AddModel(new MyModel("Models2\\Prefabs\\05_Details\\03_Signs\\531_c_research", MyMeshDrawTechnique.MESH, MyModelsEnum.p531_c_research));
            AddModel(new MyModel("Models2\\Prefabs\\05_Details\\03_Signs\\531_c_restricted_area", MyMeshDrawTechnique.MESH, MyModelsEnum.p531_c_restricted_area));
            AddModel(new MyModel("Models2\\Prefabs\\05_Details\\03_Signs\\531_c_security", MyMeshDrawTechnique.MESH, MyModelsEnum.p531_c_security));
            AddModel(new MyModel("Models2\\Prefabs\\05_Details\\03_Signs\\531_c_sign", MyMeshDrawTechnique.MESH, MyModelsEnum.p531_c_sign));
            AddEntityModels("Models2\\Prefabs\\05_Details\\03_Signs\\531_c_storage", MyModelsEnum.p531_c_storage, null, MyModelsEnum.p531_c_storage_COL);
            AddModel(new MyModel("Models2\\Prefabs\\05_Details\\03_Signs\\531_c_technical_area", MyMeshDrawTechnique.MESH, MyModelsEnum.p531_c_technical_area));
            AddModel(new MyModel("Models2\\Prefabs\\05_Details\\03_Signs\\531_c_trade_port", MyMeshDrawTechnique.MESH, MyModelsEnum.p531_c_trade_port));


            //AddModel(new MyModel("Models2\\Prefabs\\02_Shells\\02_Chambers\\01\\p221a02_chamber_v2", MyMeshDrawTechnique.MESH, MyModelsEnum.p221a02_chamber_v2));
            AddEntityModels("Models2\\Prefabs\\02_Shells\\02_Chambers\\01\\p221a02_chamber_v2", MyModelsEnum.p221a02_chamber_v2, null, MyModelsEnum.p221a02_chamber_v2_COL);
            AddModel(new MyModel("Models2\\Prefabs\\02_Shells\\02_Chambers\\02\\p221b02_chamber_v2", MyMeshDrawTechnique.MESH, MyModelsEnum.p221b02_chamber_v2));
            AddModel(new MyModel("Models2\\Prefabs\\02_Shells\\02_Chambers\\02\\p221b02_chamber_v2_COL", MyMeshDrawTechnique.MESH, MyModelsEnum.p221b02_chamber_v2_COL));
            
            //Used col model from Arena prefab because of bad collision model of this one
            //AddEntityModels("Models2\\Prefabs\\02_Shells\\02_Chambers\\03\\p221c02_chamber_v2", MyModelsEnum.p221c02_chamber_v2, MyModelsEnum.p221c02_chamber_v2_LOD1, MyModelsEnum.p221c01_chamber_v1_COL);
            AddModel(new MyModel("Models2\\Prefabs\\02_Shells\\02_Chambers\\03\\p221c02_chamber_v2", MyMeshDrawTechnique.MESH, MyModelsEnum.p221c02_chamber_v2));
            AddModel(new MyModel("Models2\\Prefabs\\02_Shells\\02_Chambers\\03\\p221c02_chamber_v2_LOD1", MyMeshDrawTechnique.MESH, MyModelsEnum.p221c02_chamber_v2_LOD1));
            m_models[(int)MyModelsEnum.p221c02_chamber_v2_COL] = m_models[(int)MyModelsEnum.p221c01_chamber_v1_COL]; 
            
            AddEntityModels("Models2\\Prefabs\\02_Shells\\02_Chambers\\04\\p221d02_chamber_v2", MyModelsEnum.p221d02_chamber_v2, MyModelsEnum.p221d02_chamber_v2_LOD1, MyModelsEnum.p221d02_chamber_v2_COL);
            AddModel(new MyModel("Models2\\Prefabs\\02_Shells\\02_Chambers\\05\\p221e02_chamber_v2", MyMeshDrawTechnique.MESH, MyModelsEnum.p221e02_chamber_v2));
            AddModel(new MyModel("Models2\\Prefabs\\02_Shells\\02_Chambers\\05\\p221e02_chamber_v2_COL", MyMeshDrawTechnique.MESH, MyModelsEnum.p221e02_chamber_v2_COL));

            //AddModel(new MyModel("Models2\\Prefabs\\02_Shells\\02_Chambers\\01\\p221a01_chamber_v1_LOD1",  MyMeshDrawTechnique.MESH, MyModelsEnum.p221a01_chamber_v1_LOD1));
            //AddModel(new MyModel("Models2\\Prefabs\\02_Shells\\02_Chambers\\02\\p221b01_chamber_v1_LOD1", MyMeshDrawTechnique.MESH, MyModelsEnum.p221b01_chamber_v1_LOD1));
            //AddModel(new MyModel("Models2\\Prefabs\\02_Shells\\02_Chambers\\03\\p221c01_chamber_v1_LOD1", MyMeshDrawTechnique.MESH, MyModelsEnum.p221c01_chamber_v1_LOD1));
            //AddModel(new MyModel("Models2\\Prefabs\\02_Shells\\02_Chambers\\04\\p221d01_chamber_v1_LOD1", MyMeshDrawTechnique.MESH, MyModelsEnum.p221d01_chamber_v1_LOD1));
            //AddModel(new MyModel("Models2\\Prefabs\\02_Shells\\02_Chambers\\05\\p221e01_chamber_v1_LOD1", MyMeshDrawTechnique.MESH, MyModelsEnum.p221e01_chamber_v1_LOD1));

            AddModel(new MyModel("Models2\\Prefabs\\02_Shells\\02_Chambers\\01\\p221a02_chamber_v2_LOD1", MyMeshDrawTechnique.MESH, MyModelsEnum.p221a02_chamber_v2_LOD1));
            AddModel(new MyModel("Models2\\Prefabs\\02_Shells\\02_Chambers\\02\\p221b02_chamber_v2_LOD1", MyMeshDrawTechnique.MESH, MyModelsEnum.p221b02_chamber_v2_LOD1));
            //AddModel(new MyModel("Models2\\Prefabs\\02_Shells\\02_Chambers\\03\\p221c02_chamber_v2_LOD1", MyMeshDrawTechnique.MESH, MyModelsEnum.p221c02_chamber_v2_LOD1));
            //AddModel(new MyModel("Models2\\Prefabs\\02_Shells\\02_Chambers\\04\\p221d02_chamber_v2_LOD1", MyMeshDrawTechnique.MESH, MyModelsEnum.p221d02_chamber_v2_LOD1));
            AddModel(new MyModel("Models2\\Prefabs\\02_Shells\\02_Chambers\\05\\p221e02_chamber_v2_LOD1", MyMeshDrawTechnique.MESH, MyModelsEnum.p221e02_chamber_v2_LOD1));

            AddEntityModels("Models2\\Prefabs\\01_Beams\\03_Small\\01\\p130a01_a_straight_10m", MyModelsEnum.p130a01_a_straight_10m, null, MyModelsEnum.p130a01_a_straight_10m_COL);
            AddEntityModels("Models2\\Prefabs\\01_Beams\\03_Small\\01\\p130a02_a_straight_30m", MyModelsEnum.p130a02_a_straight_30m, MyModelsEnum.p130a02_a_straight_30m_LOD1, MyModelsEnum.p130a02_a_straight_30m_COL);
            AddEntityModels("Models2\\Prefabs\\01_Beams\\03_Small\\02\\p130b01_b_straight_10m", MyModelsEnum.p130b01_b_straight_10m, null, MyModelsEnum.p130b01_b_straight_10m_COL);
            AddEntityModels("Models2\\Prefabs\\01_Beams\\03_Small\\02\\p130b02_b_straight_30m", MyModelsEnum.p130b02_b_straight_30m, MyModelsEnum.p130b02_b_straight_30m_LOD1, MyModelsEnum.p130b02_b_straight_30m_COL);
            AddEntityModels("Models2\\Prefabs\\01_Beams\\03_Small\\03\\p130c01_c_straight_10m", MyModelsEnum.p130c01_c_straight_10m, null, MyModelsEnum.p130c01_c_straight_10m_COL);
            AddEntityModels("Models2\\Prefabs\\01_Beams\\03_Small\\03\\p130c02_c_straight_30m", MyModelsEnum.p130c02_c_straight_30m, MyModelsEnum.p130c02_c_straight_30m_LOD1, MyModelsEnum.p130c02_c_straight_30m_COL);
            AddEntityModels("Models2\\Prefabs\\01_Beams\\03_Small\\04\\p130d01_d_straight_10m", MyModelsEnum.p130d01_d_straight_10m, null, MyModelsEnum.p130d01_d_straight_10m_COL);
            AddEntityModels("Models2\\Prefabs\\01_Beams\\03_Small\\04\\p130d02_d_straight_30m", MyModelsEnum.p130d02_d_straight_30m, MyModelsEnum.p130d02_d_straight_30m_LOD1, MyModelsEnum.p130d02_d_straight_30m_COL);
            AddEntityModels("Models2\\Prefabs\\01_Beams\\03_Small\\05\\p130e01_e_straight_10m", MyModelsEnum.p130e01_e_straight_10m, null, MyModelsEnum.p130e01_e_straight_10m_COL);
            AddEntityModels("Models2\\Prefabs\\01_Beams\\03_Small\\05\\p130e02_e_straight_30m", MyModelsEnum.p130e02_e_straight_30m, MyModelsEnum.p130e02_e_straight_30m_LOD1, MyModelsEnum.p130e02_e_straight_30m_COL);

            AddEntityModels("Models2\\Prefabs\\03_Modules\\02_Supply\\01_PowerGeneration\\04\\p321d01_big_solar_panel", MyModelsEnum.p321d01_big_solar_panel, MyModelsEnum.p321d01_big_solar_panel_LOD1, MyModelsEnum.p321d01_big_solar_panel_COL);
            //AddModel(new MyModel("Models2\\Prefabs\\03_Modules\\02_Supply\\01_PowerGeneration\\04\\p321d01_big_solar_panel", MyMeshDrawTechnique.MESH, MyModelsEnum.p321d01_big_solar_panel));

            AddEntityModels("Models2\\Prefabs\\03_Modules\\02_Supply\\01_PowerGeneration\\01\\p321b01_solar_panel", MyModelsEnum.p321b01_solar_panel, MyModelsEnum.p321b01_solar_panel_LOD1, MyModelsEnum.p321b01_solar_panel_COL);
            AddEntityModels("Models2\\Prefabs\\03_Modules\\02_Supply\\01_PowerGeneration\\01\\p321c01_solar_panel", MyModelsEnum.p321c01_solar_panel, MyModelsEnum.p321c01_solar_panel_LOD1, MyModelsEnum.p321c01_solar_panel_COL);

            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\04\\05_Junction\\p413_d02_junction_t_vertical", MyModelsEnum.p413_d02_junction_t_vertical, MyModelsEnum.p413_d02_junction_t_vertical_LOD1, MyModelsEnum.p413_d02_junction_t_vertical_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\04\\05_Junction\\p413_d04_junction_x_vertical", MyModelsEnum.p413_d04_junction_x_vertical, MyModelsEnum.p413_d04_junction_x_vertical_LOD1, MyModelsEnum.p413_d04_junction_x_vertical_COL);

            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\06\\02_Turn90\\01_230m\\p412_f11_turn_90_up_230m", MyModelsEnum.p412_f11_turn_90_up_230m, MyModelsEnum.p412_f11_turn_90_up_230m_LOD1, MyModelsEnum.p412_f11_turn_90_up_230m_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\06\\02_Turn90\\01_230m\\p412_f12_turn_90_left_230m", MyModelsEnum.p412_f12_turn_90_left_230m, MyModelsEnum.p412_f12_turn_90_left_230m_LOD1, MyModelsEnum.p412_f12_turn_90_left_230m_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\06\\02_Turn90\\01_230m\\p412_f13_turn_90_right_230m", MyModelsEnum.p412_f13_turn_90_right_230m, MyModelsEnum.p412_f13_turn_90_right_230m_LOD1, MyModelsEnum.p412_f13_turn_90_right_230m_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\06\\02_Turn90\\01_230m\\p412_f14_turn_90_down_230m", MyModelsEnum.p412_f14_turn_90_down_230m, MyModelsEnum.p412_f14_turn_90_down_230m_LOD1, MyModelsEnum.p412_f14_turn_90_down_230m_COL);

            //AddModel(new MyModel("Models2\\Prefabs\\04_Connections\\01_Tunnel\\06\\01_Straight\\p411_f04_straight_4", MyMeshDrawTechnique.MESH, MyModelsEnum.p411_f04_straight_4));
            //AddModel(new MyModel("Models2\\Prefabs\\04_Connections\\01_Tunnel\\06\\01_Straight\\p411_f05_straight_5", MyMeshDrawTechnique.MESH, MyModelsEnum.p411_f05_straight_5));
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\06\\01_Straight\\p411_f04_straight_4", MyModelsEnum.p411_f04_straight_4, null, MyModelsEnum.p411_f04_straight_4_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\06\\01_Straight\\p411_f05_straight_5", MyModelsEnum.p411_f05_straight_5, null, MyModelsEnum.p411_f05_straight_5_COL);

            //AddModel(new MyModel("Models2\\Prefabs\\04_Connections\\01_Tunnel\\07\\01_Straight\\p411_g05_straight_5", MyMeshDrawTechnique.MESH, MyModelsEnum.p411_g05_straight_5));
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\07\\01_Straight\\p411_g05_straight_5", MyModelsEnum.p411_g05_straight_5, MyModelsEnum.p411_g05_straight_5_LOD1, MyModelsEnum.p411_g05_straight_5_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\06\\04_Entrance\\p414_f01_entrance_60m", MyModelsEnum.p414_f01_entrance_60m, MyModelsEnum.p414_f01_entrance_60m_LOD1, MyModelsEnum.p414_f01_entrance_60m_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\01_Tunnel\\07\\04_Entrance\\p414_g01_entrance_60m", MyModelsEnum.p414_g01_entrance_60m, MyModelsEnum.p414_g01_entrance_60m_LOD1, MyModelsEnum.p414_g01_entrance_60m_COL);

            AddEntityModels("Models2\\Prefabs\\05_Details\\07_traffic_signs\\p571_a01_traffic_sign", MyModelsEnum.p571_a01_traffic_sign, null, MyModelsEnum.p571_a01_traffic_sign_COL);
            AddModel(new MyModel("Models2\\Prefabs\\05_Details\\07_traffic_signs\\p571_b01_traffic_sign", MyMeshDrawTechnique.MESH, MyModelsEnum.p571_b01_traffic_sign));
            AddEntityModels("Models2\\Prefabs\\05_Details\\07_traffic_signs\\p571_box01_traffic_sign", MyModelsEnum.p571_box01_traffic_sign, null, MyModelsEnum.p571_box01_traffic_sign_COL);
            AddModel(new MyModel("Models2\\Prefabs\\05_Details\\07_traffic_signs\\p571_box02_traffic_sign", MyMeshDrawTechnique.MESH, MyModelsEnum.p571_box02_traffic_sign));
            AddEntityModels("Models2\\Prefabs\\05_Details\\07_traffic_signs\\p571_c01_traffic_sign", MyModelsEnum.p571_c01_traffic_sign, null, MyModelsEnum.p571_c01_traffic_sign_COL);
            AddModel(new MyModel("Models2\\Prefabs\\05_Details\\07_traffic_signs\\p571_d01_traffic_sign", MyMeshDrawTechnique.MESH, MyModelsEnum.p571_d01_traffic_sign));
            AddModel(new MyModel("Models2\\Prefabs\\05_Details\\07_traffic_signs\\p571_e01_traffic_sign", MyMeshDrawTechnique.MESH, MyModelsEnum.p571_e01_traffic_sign));
            AddEntityModels("Models2\\Prefabs\\05_Details\\07_traffic_signs\\p571_f01_traffic_sign", MyModelsEnum.p571_f01_traffic_sign, null, MyModelsEnum.p571_f01_traffic_sign_COL);
            AddModel(new MyModel("Models2\\Prefabs\\05_Details\\07_traffic_signs\\p571_g01_traffic_sign", MyMeshDrawTechnique.MESH, MyModelsEnum.p571_g01_traffic_sign));
            AddModel(new MyModel("Models2\\Prefabs\\05_Details\\07_traffic_signs\\p571_h01_traffic_sign", MyMeshDrawTechnique.MESH, MyModelsEnum.p571_h01_traffic_sign));
            AddEntityModels("Models2\\Prefabs\\05_Details\\07_traffic_signs\\p571_i01_traffic_sign", MyModelsEnum.p571_i01_traffic_sign, null, MyModelsEnum.p571_i01_traffic_sign_COL);
            AddEntityModels("Models2\\Prefabs\\05_Details\\07_traffic_signs\\p571_j01_traffic_sign", MyModelsEnum.p571_j01_traffic_sign, null, MyModelsEnum.p571_j01_traffic_sign_COL);
            AddEntityModels("Models2\\Prefabs\\05_Details\\07_traffic_signs\\p571_k01_traffic_sign", MyModelsEnum.p571_k01_traffic_sign, null, MyModelsEnum.p571_k01_traffic_sign_COL);
            AddEntityModels("Models2\\Prefabs\\05_Details\\07_traffic_signs\\p571_l01_traffic_sign", MyModelsEnum.p571_l01_traffic_sign, null, MyModelsEnum.p571_l01_traffic_sign_COL);

            AddEntityModels("Models2\\Prefabs\\03_Modules\\08_MannedObjects\\05_Temple\\01\\p385_a01_temple_900m", MyModelsEnum.p385_a01_temple_900m, MyModelsEnum.p385_a01_temple_900m_LOD1, MyModelsEnum.p385_a01_temple_900m_COL);
            AddEntityModels("Models2\\Prefabs\\03_Modules\\08_MannedObjects\\03_Church\\01\\p383_a01_church", MyModelsEnum.p383_a01_church, MyModelsEnum.p383_a01_church_LOD1, MyModelsEnum.p383_a01_church_COL);
            AddEntityModels("Models2\\Prefabs\\03_Modules\\03_LifeSupport\\04_FoodGrowing\\01\\p334_a01_food_grow", MyModelsEnum.p334_a01_food_grow, MyModelsEnum.p334_a01_food_grow_LOD01, MyModelsEnum.p334_a01_food_grow_COL);
            AddEntityModels("Models2\\Prefabs\\03_Modules\\04_Industry\\06_BioExperiment\\01\\p345_a01_bio_exp",  MyModelsEnum.p345_a01_bio_exp, MyModelsEnum.p345_a01_bio_exp_LOD1, MyModelsEnum.p345_a01_bio_exp_COL);
            AddEntityModels("Models2\\Prefabs\\03_Modules\\04_Industry\\06_BioExperiment\\01\\p345_a01_bio_exp_center", MyModelsEnum.p345_a01_bio_exp_center, MyModelsEnum.p345_a01_bio_exp_center_LOD1, MyModelsEnum.p345_a01_bio_exp_center_COL);
            AddEntityModels("Models2\\Prefabs\\03_Modules\\04_Industry\\06_BioExperiment\\01\\p345_a01_bio_exp_tanks", MyModelsEnum.p345_a01_bio_exp_tanks, MyModelsEnum.p345_a01_bio_exp_tanks_LOD1, MyModelsEnum.p345_a01_bio_exp_tanks_COL);
            AddEntityModels("Models2\\Prefabs\\03_Modules\\04_Industry\\07_BioMachine\\01\\p345_a01_bio_mach_exp", MyModelsEnum.p345_a01_bio_mach_exp, MyModelsEnum.p345_a01_bio_mach_exp_LOD1, MyModelsEnum.p345_a01_bio_mach_exp_COL);
            AddEntityModels("Models2\\Prefabs\\03_Modules\\04_Industry\\08_Recycle\\01\\p345_a01_recycle", MyModelsEnum.p345_a01_recycle, MyModelsEnum.p345_a01_recycle_LOD1, MyModelsEnum.p345_a01_recycle_COL);

            //AddModel(new MyModel("Models2\\Prefabs\\05_Details\\04_Other\\541_escape_pod", MyMeshDrawTechnique.MESH, MyModelsEnum.p541_escape_pod));
            AddEntityModels("Models2\\Prefabs\\05_Details\\04_Other\\541_escape_pod", MyModelsEnum.p541_escape_pod, MyModelsEnum.p541_escape_pod_LOD1, MyModelsEnum.p541_escape_pod_COL);

            AddEntityModels("Models2\\Prefabs\\05_Details\\04_Other\\541_escape_pod_base", MyModelsEnum.p541_escape_pod_base, MyModelsEnum.p541_escape_pod_base_LOD1, MyModelsEnum.p541_escape_pod_base_COL);


            //AddModel(new MyModel("Models2\\Prefabs\\05_Details\\04_Other\\541_ventilator_body", MyMeshDrawTechnique.MESH, MyModelsEnum.p541_ventilator_body));
            AddEntityModels("Models2\\Prefabs\\05_Details\\04_Other\\541_ventilator_body", MyModelsEnum.p541_ventilator_body, MyModelsEnum.p541_ventilator_body_LOD1, MyModelsEnum.p541_ventilator_body_COL);

            AddEntityModels("Models2\\Prefabs\\05_Details\\04_Other\\541_ventilator_propeller", MyModelsEnum.p541_ventilator_propeller, null, MyModelsEnum.p541_ventilator_propeller_COL);
            AddEntityModels("Models2\\Prefabs\\05_Details\\04_Other\\541_ventilator_propeller_big", MyModelsEnum.p541_ventilator_propeller_big, MyModelsEnum.p541_ventilator_propeller_big_LOD1, MyModelsEnum.p541_ventilator_propeller_big_COL);            

            AddModel(new MyModel("Models2\\Prefabs\\06_Custom\\SimpleObject", MyMeshDrawTechnique.MESH, MyModelsEnum.SimpleObject));
            //AddModel(new MyModel("Models2\\Prefabs\\06_Custom\\asteroid_test", MyMeshDrawTechnique.MESH, MyModelsEnum.AsteroidPrefabTest));            
            AddEntityModels("Models2\\Prefabs\\FoundationFactory", MyModelsEnum.FoundationFactory, null, MyModelsEnum.FoundationFactory_COL);

            AddEntityModels("Models2\\Prefabs\\03_Modules\\04_Industry\\09_Tower\\p349_a_tower", MyModelsEnum.p349_a_tower, MyModelsEnum.p349_a_tower_LOD1, MyModelsEnum.p349_a_tower_COL);
            AddEntityModels("Models2\\Prefabs\\03_Modules\\04_Industry\\09_Tower\\p349_b_tower", MyModelsEnum.p349_b_tower, MyModelsEnum.p349_b_tower_LOD1, MyModelsEnum.p349_b_tower_COL);
            AddEntityModels("Models2\\Prefabs\\03_Modules\\04_Industry\\09_Tower\\p349_c_tower", MyModelsEnum.p349_c_tower, MyModelsEnum.p349_c_tower_LOD1, MyModelsEnum.p349_c_tower_COL);

            //AddModel(new MyModel("Models2\\Prefabs\\03_Modules\\07_Hangars\\01_Small\\01\\p361_a02_hangar_panel", MyMeshDrawTechnique.MESH, MyModelsEnum.p361_a02_hangar_panel));

            AddEntityModels("Models2\\Prefabs\\05_Details\\03_Signs\\p531_b_faction", MyModelsEnum.p531_b_faction, null, MyModelsEnum.p531_b_faction_COL);
            AddEntityModels("Models2\\Prefabs\\05_Details\\03_Signs\\p531_b_faction_holo", MyModelsEnum.p531_b_faction_holo, null, MyModelsEnum.p531_b_faction_holo_COL);
            AddEntityModels("Models2\\Prefabs\\02_Shells\\03_Armor\\01\\armor_hull", MyModelsEnum.armor_hull, MyModelsEnum.armor_hull_LOD1, MyModelsEnum.armor_hull_COL);

            AddModel(new MyModel("Models2\\Prefabs\\02_Shells\\03_Armor\\01\\Fourth_Reich_mothership", MyMeshDrawTechnique.MESH, MyModelsEnum.FourthReichMothership));
            AddModel(new MyModel("Models2\\Prefabs\\02_Shells\\03_Armor\\01\\Fourth_Reich_mothership_LOD1", MyMeshDrawTechnique.MESH, MyModelsEnum.FourthReichMothership_LOD1));
            AddEntityModels("Models2\\Prefabs\\02_Shells\\03_Armor\\01\\Fourth_Reich_mothership_B", MyModelsEnum.FourthReichMothership_B, MyModelsEnum.FourthReichMothership_B_LOD1, MyModelsEnum.FourthReichMothership_B_COL);

            AddEntityModels("Models2\\Prefabs\\02_Shells\\03_Armor\\01\\Rus_mothership", MyModelsEnum.RusMothership, MyModelsEnum.RusMothership_LOD1, MyModelsEnum.RusMothership_COL);
            AddEntityModels("Models2\\Prefabs\\02_Shells\\03_Armor\\01\\Rus_Mothership_Russian_Hummer", MyModelsEnum.Russian_Mothership_Hummer, MyModelsEnum.Russian_Mothership_Hummer_LOD1, MyModelsEnum.Russian_Mothership_Hummer_COL);            


            AddModel(new MyModel("Models2\\Prefabs\\03_Modules\\01_Flight\\01_LongTermThruster\\01\\p311a01_long_term_thruster_LOD1", MyMeshDrawTechnique.MESH, MyModelsEnum.p311a01_long_term_thruster_LOD1));

            AddModel(new MyModel("Models2\\Prefabs\\03_Modules\\02_Supply\\01_PowerGeneration\\02\\p321b01_nuclear_reactor_LOD1", MyMeshDrawTechnique.MESH, MyModelsEnum.p321b01_nuclear_reactor_LOD1));
            AddModel(new MyModel("Models2\\Prefabs\\03_Modules\\04_Industry\\02_LoadingBay\\01\\p342_a01_loading_bay_LOD1", MyMeshDrawTechnique.MESH, MyModelsEnum.p342_a01_loading_bay_LOD1));
            AddModel(new MyModel("Models2\\Prefabs\\03_Modules\\04_Industry\\03_OreStorage\\01\\p343_a01_ore_storage_LOD1", MyMeshDrawTechnique.MESH, MyModelsEnum.p343_a01_ore_storage_LOD1));

            AddModel(new MyModel("Models2\\Ships\\Ships\\SmallShip_Doon_LOD1", MyMeshDrawTechnique.MESH, MyModelsEnum.SmallShip_Doon_LOD1));
            AddModel(new MyModel("Models2\\Ships\\Ships\\SmallShip_Jacknife_LOD1", MyMeshDrawTechnique.MESH, MyModelsEnum.SmallShip_Jacknife_LOD1));
            AddModel(new MyModel("Models2\\Ships\\Ships\\SmallShip_Hammer_LOD1", MyMeshDrawTechnique.MESH, MyModelsEnum.SmallShip_Hammer_LOD1));
            AddModel(new MyModel("Models2\\Ships\\Ships\\SmallShip_ORG_LOD1", MyMeshDrawTechnique.MESH, MyModelsEnum.SmallShip_ORG_LOD1));
            AddModel(new MyModel("Models2\\Ships\\Ships\\SmallShip_ORG_NoPaint_LOD1", MyMeshDrawTechnique.MESH, MyModelsEnum.SmallShip_ORG_NoPaint_LOD1));
            AddModel(new MyModel("Models2\\Ships\\Ships\\SmallShip_YG_Closed_LOD1", MyMeshDrawTechnique.MESH, MyModelsEnum.SmallShip_YG_Closed_LOD1));
            AddModel(new MyModel("Models2\\Ships\\Ships\\SmallShip_Hawk_LOD1", MyMeshDrawTechnique.MESH, MyModelsEnum.SmallShip_Hawk_LOD1));
            AddModel(new MyModel("Models2\\Ships\\Ships\\SmallShip_Phoenix_LOD1", MyMeshDrawTechnique.MESH, MyModelsEnum.SmallShip_Phoenix_LOD1));
            AddModel(new MyModel("Models2\\Ships\\Ships\\SmallShip_Leviathan_LOD1", MyMeshDrawTechnique.MESH, MyModelsEnum.SmallShip_Leviathan_LOD1));
            AddModel(new MyModel("Models2\\Ships\\Ships\\SmallShip_Rockheater_LOD1", MyMeshDrawTechnique.MESH, MyModelsEnum.SmallShip_Rockheater_LOD1));
            AddModel(new MyModel("Models2\\Ships\\Ships\\SmallShip_SteelHead_LOD1", MyMeshDrawTechnique.MESH, MyModelsEnum.SmallShip_SteelHead_LOD1));
            AddModel(new MyModel("Models2\\Ships\\Ships\\SmallShip_Talon_LOD1", MyMeshDrawTechnique.MESH, MyModelsEnum.SmallShip_Talon_LOD1));
            AddModel(new MyModel("Models2\\Ships\\Ships\\SmallShip_Stanislav_LOD1", MyMeshDrawTechnique.MESH, MyModelsEnum.SmallShip_Stanislav_LOD1));
            AddModel(new MyModel("Models2\\Ships\\Ships\\SmallShip_MinerShip01_LOD1", MyMeshDrawTechnique.MESH, MyModelsEnum.Liberator_LOD1));
            AddModel(new MyModel("Models2\\Ships\\Ships\\SmallShip_MinerShip02_LOD1", MyMeshDrawTechnique.MESH, MyModelsEnum.Enforcer_LOD1));
            AddModel(new MyModel("Models2\\Ships\\Ships\\SmallShip_MinerShip03_LOD1", MyMeshDrawTechnique.MESH, MyModelsEnum.Kammler_LOD1));
            AddModel(new MyModel("Models2\\Ships\\Ships\\SmallShip_MinerShip04_LOD1", MyMeshDrawTechnique.MESH, MyModelsEnum.Gettysburg_LOD1));
            AddModel(new MyModel("Models2\\Ships\\Ships\\SmallShip_MinerShip05_LOD1", MyMeshDrawTechnique.MESH, MyModelsEnum.Virginia_LOD1));
            AddModel(new MyModel("Models2\\Ships\\Ships\\SmallShip_MinerShip06_LOD1", MyMeshDrawTechnique.MESH, MyModelsEnum.Baer_LOD1));
            AddModel(new MyModel("Models2\\Ships\\Ships\\SmallShip_MinerShip07_LOD1", MyMeshDrawTechnique.MESH, MyModelsEnum.Hewer_LOD1));
            AddModel(new MyModel("Models2\\Ships\\Ships\\SmallShip_MinerShip08_LOD1", MyMeshDrawTechnique.MESH, MyModelsEnum.Razorclaw_LOD1));
            AddModel(new MyModel("Models2\\Ships\\Ships\\SmallShip_MinerShip09_LOD1", MyMeshDrawTechnique.MESH, MyModelsEnum.Greiser_LOD1));
            AddModel(new MyModel("Models2\\Ships\\Ships\\SmallShip_MinerShip10_LOD1", MyMeshDrawTechnique.MESH, MyModelsEnum.Tracer_LOD1));

            // Drones
            AddModel(new MyModel("Models2\\Ships\\Drones\\drone_CN", MyMeshDrawTechnique.MESH, MyModelsEnum.DroneCN));
            AddModel(new MyModel("Models2\\Ships\\Drones\\drone_SS", MyMeshDrawTechnique.MESH, MyModelsEnum.DroneSS));
            AddModel(new MyModel("Models2\\Ships\\Drones\\drone_US", MyMeshDrawTechnique.MESH, MyModelsEnum.DroneUS));

            AddEntityModels("Models2\\Prefabs\\02_Shells\\01_CagePanels\\08_bariers\\p212a01_panel_large", MyModelsEnum.p212a01_panel_large, MyModelsEnum.p212a01_panel_large_LOD1, MyModelsEnum.p212a01_panel_large_COL);
            AddEntityModels("Models2\\Prefabs\\02_Shells\\01_CagePanels\\08_bariers\\p212a01_panel_medium", MyModelsEnum.p212a01_panel_medium, MyModelsEnum.p212a01_panel_medium_LOD1, MyModelsEnum.p212a01_panel_medium_COL);
            AddEntityModels("Models2\\Prefabs\\02_Shells\\01_CagePanels\\08_bariers\\p212b02_panel_medium", MyModelsEnum.p212b02_panel_medium, MyModelsEnum.p212b02_panel_medium_LOD1, MyModelsEnum.p212b02_panel_medium_COL);
            AddEntityModels("Models2\\Prefabs\\02_Shells\\01_CagePanels\\08_bariers\\p212c03_panel_medium", MyModelsEnum.p212c03_panel_medium, MyModelsEnum.p212c03_panel_medium_LOD1, MyModelsEnum.p212c03_panel_medium_COL);
            AddEntityModels("Models2\\Prefabs\\02_Shells\\01_CagePanels\\08_bariers\\p212d04_panel_medium", MyModelsEnum.p212d04_panel_medium, null, MyModelsEnum.p212d04_panel_medium_COL);
            AddEntityModels("Models2\\Prefabs\\02_Shells\\01_CagePanels\\08_bariers\\p212e05_panel_medium", MyModelsEnum.p212e05_panel_medium, null, MyModelsEnum.p212e05_panel_medium_COL);

            AddEntityModels("Models2\\Prefabs\\02_Shells\\01_CagePanels\\08_bariers\\p212a01_panel_small", MyModelsEnum.p212a01_panel_small, null, MyModelsEnum.p212a01_panel_small_COL);
            AddEntityModels("Models2\\Prefabs\\02_Shells\\01_CagePanels\\08_bariers\\p212b02_panel_small", MyModelsEnum.p212b02_panel_small, null, MyModelsEnum.p212b02_panel_small_COL);
            AddEntityModels("Models2\\Prefabs\\02_Shells\\01_CagePanels\\08_bariers\\p212c03_panel_small", MyModelsEnum.p212c03_panel_small, null, MyModelsEnum.p212c03_panel_small_COL);
            AddEntityModels("Models2\\Prefabs\\02_Shells\\01_CagePanels\\08_bariers\\p212d04_panel_small", MyModelsEnum.p212d04_panel_small, null, MyModelsEnum.p212d04_panel_small_COL);
            AddEntityModels("Models2\\Prefabs\\02_Shells\\01_CagePanels\\08_bariers\\p212e05_panel_small", MyModelsEnum.p212e05_panel_small, null, MyModelsEnum.p212e05_panel_small_COL);

            AddModel(new MyModel("Models2\\ObjectsFloating\\Box_matt_5m", MyMeshDrawTechnique.MESH, MyModelsEnum.MysteriousBox_matt_5m));
            AddModel(new MyModel("Models2\\ObjectsFloating\\Box_spec_5m", MyMeshDrawTechnique.MESH, MyModelsEnum.MysteriousBox_spec_5m));
            AddModel(new MyModel("Models2\\ObjectsFloating\\Box_mid_5m", MyMeshDrawTechnique.MESH, MyModelsEnum.MysteriousBox_mid_5m));


            AddEntityModels("Models2\\Prefabs\\03_Modules\\04_Industry\\01_Docks\\03\\p341_c01_closed_dock_v1", MyModelsEnum.p341_c01_closed_dock_v1, MyModelsEnum.p341_c01_closed_dock_v1_LOD1, MyModelsEnum.p341_c01_closed_dock_v1_COL);

            AddEntityModels("Models2\\Prefabs\\02_Shells\\01_CagePanels\\08_bariers\\p212b02_panel_large", MyModelsEnum.p212b02_panel_large, MyModelsEnum.p212b02_panel_large_LOD1, MyModelsEnum.p212b02_panel_large_COL);
            AddEntityModels("Models2\\Prefabs\\02_Shells\\01_CagePanels\\08_bariers\\p212c03_panel_large", MyModelsEnum.p212c03_panel_large, MyModelsEnum.p212c03_panel_large_LOD1, MyModelsEnum.p212c03_panel_large_COL);
            AddEntityModels("Models2\\Prefabs\\02_Shells\\01_CagePanels\\08_bariers\\p212d04_panel_large", MyModelsEnum.p212d04_panel_large, MyModelsEnum.p212d04_panel_large_LOD1, MyModelsEnum.p212d04_panel_large_COL);
            AddEntityModels("Models2\\Prefabs\\02_Shells\\01_CagePanels\\08_bariers\\p212e05_panel_large", MyModelsEnum.p212e05_panel_large, MyModelsEnum.p212e05_panel_large_LOD1, MyModelsEnum.p212e05_panel_large_COL);
            AddEntityModels("Models2\\Prefabs\\02_Shells\\01_CagePanels\\08_bariers\\p212f01_panel_large", MyModelsEnum.p212f01_panel_large, null, MyModelsEnum.p212f01_panel_large_COL);

            AddEntityModels("Models2\\Prefabs\\03_Modules\\09_Alien\\01_Alien_Gate\\p391_Alien_Gate", MyModelsEnum.Alien_gate, MyModelsEnum.Alien_gate_LOD1, null);

            AddEntityModels("Models2\\ObjectsStatic\\LargeShips\\mship\\mship_body", MyModelsEnum.mship_body, MyModelsEnum.mship_body_LOD1, null);
            AddEntityModels("Models2\\ObjectsStatic\\LargeShips\\mship\\mship_engine", MyModelsEnum.mship_engine, MyModelsEnum.mship_engine_LOD1, null);
            AddEntityModels("Models2\\ObjectsStatic\\LargeShips\\mship\\mship_shield_back_large_left", MyModelsEnum.mship_shield_back_large_left, MyModelsEnum.mship_shield_back_large_left_LOD1, null);
            AddEntityModels("Models2\\ObjectsStatic\\LargeShips\\mship\\mship_shield_back_large_right", MyModelsEnum.mship_shield_back_large_right, MyModelsEnum.mship_shield_back_large_right_LOD1, null);
            AddEntityModels("Models2\\ObjectsStatic\\LargeShips\\mship\\mship_shield_back_small_left", MyModelsEnum.mship_shield_back_small_left, MyModelsEnum.mship_shield_back_small_left_LOD1, null);
            AddEntityModels("Models2\\ObjectsStatic\\LargeShips\\mship\\mship_shield_back_small_right", MyModelsEnum.mship_shield_back_small_right, MyModelsEnum.mship_shield_back_small_right_LOD1, null);
            AddEntityModels("Models2\\ObjectsStatic\\LargeShips\\mship\\mship_shield_front_large_left", MyModelsEnum.mship_shield_front_large_left, MyModelsEnum.mship_shield_front_large_left_LOD1, null);
            AddEntityModels("Models2\\ObjectsStatic\\LargeShips\\mship\\mship_shield_front_large_right", MyModelsEnum.mship_shield_front_large_right, MyModelsEnum.mship_shield_front_large_right_LOD1, null);
            AddEntityModels("Models2\\ObjectsStatic\\LargeShips\\mship\\mship_shield_front_small_left", MyModelsEnum.mship_shield_front_small_left, MyModelsEnum.mship_shield_front_small_left_LOD1, null);
            AddEntityModels("Models2\\ObjectsStatic\\LargeShips\\mship\\mship_shield_front_small_right", MyModelsEnum.mship_shield_front_small_right, MyModelsEnum.mship_shield_front_small_right_LOD1, null);
            AddEntityModels("Models2\\ObjectsStatic\\LargeShips\\mship\\mship_shield_front_small02_left", MyModelsEnum.mship_shield_front_small02_left, MyModelsEnum.mship_shield_front_small02_left_LOD1, null);
            AddEntityModels("Models2\\ObjectsStatic\\LargeShips\\mship\\mship_shield_front_small02_right", MyModelsEnum.mship_shield_front_small02_right, MyModelsEnum.mship_shield_front_small02_right_LOD1, null);

            AddEntityModels("Models2\\Prefabs\\03_Modules\\08_MannedObjects\\01_LivingQuarters\\04\\p381_d03_hospital", MyModelsEnum.p381_d03_hospital, MyModelsEnum.p381_d03_hospital_LOD1, MyModelsEnum.p381_d03_hospital_COL);            
            AddEntityModels("Models2\\Prefabs\\03_Modules\\08_MannedObjects\\01_LivingQuarters\\04\\p381_d05_food_grow", MyModelsEnum.p381_d05_food_grow, MyModelsEnum.p381_d05_food_grow_LOD1, null);            

            AddEntityModels("Models2\\Prefabs\\04_Connections\\04_Cables\\Cable_corner_25m", MyModelsEnum.Cable_corner_25m, null, MyModelsEnum.Cable_corner_25m_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\04_Cables\\Cable_S_45m", MyModelsEnum.Cable_S_45m, null, MyModelsEnum.Cable_S_45m_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\04_Cables\\Cable_straight_180", MyModelsEnum.Cable_straight_180, null, MyModelsEnum.Cable_straight_180_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\04_Cables\\Cable_straight_45", MyModelsEnum.Cable_straight_45, null, MyModelsEnum.Cable_straight_45_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\04_Cables\\Cable_straight_90", MyModelsEnum.Cable_straight_90, null, MyModelsEnum.Cable_straight_90_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\04_Cables\\Connection_box", MyModelsEnum.Connection_box, null, MyModelsEnum.Connection_box_COL);

            AddModel(new MyModel("Models2\\ObjectsFloating\\cargo_box", MyMeshDrawTechnique.MESH, MyModelsEnum.cargo_box_1));
            AddModel(new MyModel("Models2\\ObjectsFloating\\cargo_box_2", MyMeshDrawTechnique.MESH, MyModelsEnum.cargo_box_2));
            AddModel(new MyModel("Models2\\ObjectsFloating\\cargo_box_3", MyMeshDrawTechnique.MESH, MyModelsEnum.cargo_box_3));
            AddModel(new MyModel("Models2\\ObjectsFloating\\cargo_box_4", MyMeshDrawTechnique.MESH, MyModelsEnum.cargo_box_4));
            AddModel(new MyModel("Models2\\ObjectsFloating\\cargo_box_5", MyMeshDrawTechnique.MESH, MyModelsEnum.cargo_box_5));
            AddModel(new MyModel("Models2\\ObjectsFloating\\cargo_box_6", MyMeshDrawTechnique.MESH, MyModelsEnum.cargo_box_6));
            AddModel(new MyModel("Models2\\ObjectsFloating\\cargo_box_7", MyMeshDrawTechnique.MESH, MyModelsEnum.cargo_box_7));
            AddModel(new MyModel("Models2\\ObjectsFloating\\cargo_box_8", MyMeshDrawTechnique.MESH, MyModelsEnum.cargo_box_8));
            AddModel(new MyModel("Models2\\ObjectsFloating\\cargo_box_9", MyMeshDrawTechnique.MESH, MyModelsEnum.cargo_box_9));
            AddModel(new MyModel("Models2\\ObjectsFloating\\cargo_box_10", MyMeshDrawTechnique.MESH, MyModelsEnum.cargo_box_10));
            AddModel(new MyModel("Models2\\ObjectsFloating\\cargo_box_small", MyMeshDrawTechnique.MESH, MyModelsEnum.cargo_box_small));
            AddEntityModels("Models2\\Prefabs\\05_Details\\04_Other\\541_security_hub", MyModelsEnum.p541_security_hub, MyModelsEnum.p541_security_hub_LOD1, MyModelsEnum.p541_security_hub_COL);
            AddModel(new MyModel("Models2\\ObjectsFloating\\Alarm", MyMeshDrawTechnique.MESH, MyModelsEnum.Alarm));
            AddModel(new MyModel("Models2\\ObjectsFloating\\Alarm_off", MyMeshDrawTechnique.MESH, MyModelsEnum.Alarm_off));
            AddEntityModels("Models2\\ObjectsFloating\\Bank_node", MyModelsEnum.Bank_node, null, MyModelsEnum.Bank_node_COL);
            //AddEntityModels("Models2\\ObjectsFloating\\Bank_node", MyModelsEnum.Bank_node, null, null);
            AddModel(new MyModel("Models2\\ObjectsFloating\\Cam", MyMeshDrawTechnique.MESH, MyModelsEnum.Cam));

            AddEntityModels("Models2\\Prefabs\\03_Modules\\06_Communications\\01_LongDistanceAntenna\\01\\p361_b01_long_distance_antenna_big", MyModelsEnum.p361_b01_long_distance_antenna_big, MyModelsEnum.p361_b01_long_distance_antenna_big_LOD1, MyModelsEnum.p361_b01_long_distance_antenna_big_COL);
            AddEntityModels("Models2\\Prefabs\\03_Modules\\06_Communications\\01_LongDistanceAntenna\\01\\p361_b01_long_distance_antenna_dish", MyModelsEnum.p361_b01_long_distance_antenna_dish, MyModelsEnum.p361_b01_long_distance_antenna_dish_LOD1, MyModelsEnum.p361_b01_long_distance_antenna_dish_COL);
            AddEntityModels("Models2\\Prefabs\\05_Details\\04_Other\\4th_reich_wreck", MyModelsEnum.fourth_reich_wreck, MyModelsEnum.fourth_reich_wreck_LOD1, MyModelsEnum.fourth_reich_wreck_COL);
            AddEntityModels("Models2\\Prefabs\\03_Modules\\04_Industry\\04_CargoStorage\\01\\p344_a03_container", MyModelsEnum.p344_a03_container, MyModelsEnum.p344_a03_container_LOD1, MyModelsEnum.p344_a03_container_COL);
            AddEntityModels("Models2\\Prefabs\\02_Shells\\03_Armor\\01\\p231b01_armor", MyModelsEnum.p231b01_armor, MyModelsEnum.p231b01_armor_LOD1, MyModelsEnum.p231b01_armor_COL);
            AddEntityModels("Models2\\Prefabs\\02_Shells\\03_Armor\\01\\p231b01_armor_corner", MyModelsEnum.p231b01_armor_corner, MyModelsEnum.p231b01_armor_corner_LOD1, MyModelsEnum.p231b01_armor_corner_COL);
            AddEntityModels("Models2\\Prefabs\\02_Shells\\03_Armor\\01\\p231b01_armor_edge", MyModelsEnum.p231b01_armor_edge, MyModelsEnum.p231b01_armor_edge_LOD1, MyModelsEnum.p231b01_armor_edge_COL);
            AddEntityModels("Models2\\Prefabs\\02_Shells\\03_Armor\\01\\p231b01_armor_hole", MyModelsEnum.p231b01_armor_hole, MyModelsEnum.p231b01_armor_hole_LOD1, MyModelsEnum.p231b01_armor_hole_COL);

            AddEntityModels("Models2\\Prefabs\\01_Beams\\05_Shelf\\p150a03_shelf_1", MyModelsEnum.p150a03_shelf_1, MyModelsEnum.p150a03_shelf_1_LOD1, MyModelsEnum.p150a03_shelf_1_COL);
            AddEntityModels("Models2\\Prefabs\\01_Beams\\05_Shelf\\p150a02_shelf_1X2", MyModelsEnum.p150a02_shelf_1X2, MyModelsEnum.p150a02_shelf_1X2_LOD1, MyModelsEnum.p150a02_shelf_1X2_COL);
            AddEntityModels("Models2\\Prefabs\\01_Beams\\05_Shelf\\p150a01_shelf_1X3", MyModelsEnum.p150a01_shelf_1X3, MyModelsEnum.p150a01_shelf_1X3_LOD1, MyModelsEnum.p150a01_shelf_1X3_COL);

            AddEntityModels("Models2\\Prefabs\\01_Beams\\03_Small\\04\\p130d02_d_straight_300m", MyModelsEnum.p130d02_d_straight_300m, MyModelsEnum.p130d02_d_straight_300m_LOD1, MyModelsEnum.p130d02_d_straight_300m_COL);
            AddEntityModels("Models2\\Prefabs\\01_Beams\\03_Small\\10\\p130j01_j_straight_300m", MyModelsEnum.p130j01_j_straight_300m, MyModelsEnum.p130j01_j_straight_300m_LOD1, MyModelsEnum.p130j01_j_straight_300m_COL);
            AddEntityModels("Models2\\Prefabs\\01_Beams\\02_Medium\\03\\p120c02_c_straight_400m", MyModelsEnum.p120c02_c_straight_400m, MyModelsEnum.p120c02_c_straight_400m_LOD1, MyModelsEnum.p120c02_c_straight_400m_COL);
            AddEntityModels("Models2\\Prefabs\\01_Beams\\01_Large\\02\\p110b03_lattice_beam_straight_420m", MyModelsEnum.p110b03_lattice_beam_straight_420m, MyModelsEnum.p110b03_lattice_beam_straight_420m_LOD1, MyModelsEnum.p110b03_lattice_beam_straight_420m_COL);
            AddEntityModels("Models2\\Prefabs\\01_Beams\\01_Large\\02\\p110b04_lattice_beam_straight_420m_with_panels", MyModelsEnum.p110b04_lattice_beam_straight_420m_with_panels, MyModelsEnum.p110b04_lattice_beam_straight_420m_with_panels_LOD1, MyModelsEnum.p110b04_lattice_beam_straight_420m_with_panels_COL);
            AddEntityModels("Models2\\Prefabs\\03_Modules\\06_Communications\\03_BigAntenna\\p363_a01_big_antenna_300m", MyModelsEnum.p363_a01_big_antenna_300m, MyModelsEnum.p363_a01_big_antenna_300m_LOD1, MyModelsEnum.p363_a01_big_antenna_300m_COL);
            AddEntityModels("Models2\\Prefabs\\06_Custom\\01_Asteroid_Prefabs\\p611_asteroid_part_A", MyModelsEnum.p611_asteroid_part_A, MyModelsEnum.p611_asteroid_part_A_LOD1, null);
            AddEntityModels("Models2\\Prefabs\\06_Custom\\01_Asteroid_Prefabs\\p611_asteroid_part_B", MyModelsEnum.p611_asteroid_part_B, MyModelsEnum.p611_asteroid_part_B_LOD1, null);

            //AddModel(new MyModel("Models2\\Prefabs\\05_Details\\04_Other\\541_ventilator_body_standalone", MyMeshDrawTechnique.MESH, MyModelsEnum.p541_ventilator_body_standalone));
            AddEntityModels("Models2\\Prefabs\\05_Details\\04_Other\\541_ventilator_body_standalone", MyModelsEnum.p541_ventilator_body_standalone, MyModelsEnum.p541_ventilator_body_standalone_LOD1, MyModelsEnum.p541_ventilator_body_standalone_COL);
            AddEntityModels("Models2\\Prefabs\\05_Details\\04_Other\\541_ventilator_propeller_standalone", MyModelsEnum.p541_ventilator_propeller_standalone, MyModelsEnum.p541_ventilator_propeller_standalone_LOD1, MyModelsEnum.p541_ventilator_propeller_standalone_COL);            
                            
            AddEntityModels("Models2\\Prefabs\\03_Modules\\02_Supply\\01_PowerGeneration\\03\\p321c03_centrifuge", MyModelsEnum.p321c03_centrifuge, MyModelsEnum.p321c03_centrifuge_LOD1, MyModelsEnum.p321c03_centrifuge_COL);
            AddEntityModels("Models2\\Prefabs\\03_Modules\\02_Supply\\01_PowerGeneration\\03\\p321c04_box_generator", MyModelsEnum.p321c04_box_generator, MyModelsEnum.p321c04_box_generator_LOD1, MyModelsEnum.p321c04_box_generator_COL);
            AddEntityModels("Models2\\Prefabs\\03_Modules\\02_Supply\\01_PowerGeneration\\03\\p321c05_centrifuge_big", MyModelsEnum.p321c05_centrifuge_big, MyModelsEnum.p321c05_centrifuge_big_LOD1, MyModelsEnum.p321c05_centrifuge_big_COL);
            AddEntityModels("Models2\\Prefabs\\03_Modules\\06_Communications\\03_BigAntenna\\p363_a01_big_antenna_1500m", MyModelsEnum.p363_a01_big_antenna_1500m, MyModelsEnum.p363_a01_big_antenna_1500m_LOD1, MyModelsEnum.p363_a01_big_antenna_1500m_COL);
            AddEntityModels("Models2\\Prefabs\\03_Modules\\02_Supply\\01_PowerGeneration\\03\\\\p321c02_generator_wall_big", MyModelsEnum.p321c02_generator_wall_big, MyModelsEnum.p321c02_generator_wall_big_LOD1, MyModelsEnum.p321c02_generator_wall_big_COL);
            AddEntityModels("Models2\\Prefabs\\03_Modules\\02_Supply\\01_PowerGeneration\\03\\p321c02_inertia_generator_center_big", MyModelsEnum.p321c02_inertia_generator_center_big, MyModelsEnum.p321c02_inertia_generator_center_big_LOD1, MyModelsEnum.p321c02_inertia_generator_center_big_COL);
            AddEntityModels("Models2\\Prefabs\\03_Modules\\02_Supply\\01_PowerGeneration\\03\\p321c02_inertia_generator_center_vert", MyModelsEnum.p321c02_inertia_generator_center_vert, MyModelsEnum.p321c02_inertia_generator_center_vert_LOD1, null);
            AddEntityModels("Models2\\Prefabs\\03_Modules\\02_Supply\\01_PowerGeneration\\03\\p321c03_centrifuge_centre", MyModelsEnum.p321c03_centrifuge_centre, MyModelsEnum.p321c03_centrifuge_centre_LOD1, MyModelsEnum.p321c03_centrifuge_centre_COL);
            AddEntityModels("Models2\\Prefabs\\03_Modules\\02_Supply\\01_PowerGeneration\\03\\p321c04_two_big_inertia", MyModelsEnum.p321c04_two_big_inertia, MyModelsEnum.p321c04_two_big_inertia_LOD1, null);
            AddEntityModels("Models2\\Prefabs\\03_Modules\\02_Supply\\01_PowerGeneration\\03\\p321c05_centrifuge_centre_big", MyModelsEnum.p321c05_centrifuge_centre_big, MyModelsEnum.p321c05_centrifuge_centre_big_LOD1, null);

            AddEntityModels("Models2\\Prefabs\\03_Modules\\02_Supply\\01_PowerGeneration\\03\\p321c06_inertia_B_centre", MyModelsEnum.p321c06_inertia_B_centre, MyModelsEnum.p321c06_inertia_B_centre_LOD1, MyModelsEnum.p321c06_inertia_B_centre_COL);
            AddEntityModels("Models2\\Prefabs\\03_Modules\\02_Supply\\01_PowerGeneration\\03\\p321c06_inertia_generator_B", MyModelsEnum.p321c06_inertia_generator_B, MyModelsEnum.p321c06_inertia_generator_B_LOD1, MyModelsEnum.p321c06_inertia_generator_B_COL);
            AddModel(new MyModel("Models2\\Prefabs\\04_Connections\\02_Pipe\\02\\01_Cooling_Device_Wall\\p4221_a01_cooling_device_ventilator_1", MyMeshDrawTechnique.MESH, MyModelsEnum.p4221_a01_cooling_device_ventilator_1));
            AddModel(new MyModel("Models2\\Prefabs\\04_Connections\\02_Pipe\\02\\01_Cooling_Device_Wall\\p4221_a01_cooling_device_ventilator_2", MyMeshDrawTechnique.MESH, MyModelsEnum.p4221_a01_cooling_device_ventilator_2));
            AddEntityModels("Models2\\Prefabs\\04_Connections\\02_Pipe\\02\\01_Cooling_Device_Wall\\p4221_a01_cooling_device_wall_340x400", MyModelsEnum.p4221_a01_cooling_device_wall_340x400, MyModelsEnum.p4221_a01_cooling_device_wall_340x400_LOD1, MyModelsEnum.p4221_a01_cooling_device_wall_340x400_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\02_Pipe\\02\\02_Pipes_Connector\\p4222_a01_pipes_connector_center", MyModelsEnum.p4222_a01_pipes_connector_center, MyModelsEnum.p4222_a01_pipes_connector_center_LOD1, null);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\02_Pipe\\02\\02_Pipes_Connector\\p4222_a01_pipes_connector", MyModelsEnum.p4222_a01_pipes_connector, MyModelsEnum.p4222_a01_pipes_connector_LOD1, MyModelsEnum.p4222_a01_pipes_connector_COL);
            AddEntityModels("Models2\\Prefabs\\04_Connections\\02_Pipe\\02\\03_Open_Pipe\\p4223_a01_open_pipe", MyModelsEnum.p4223_a01_open_pipe, MyModelsEnum.p4223_a01_open_pipe_LOD1, MyModelsEnum.p4223_a01_open_pipe_COL);
            AddEntityModels("Models2\\Prefabs\\03_Modules\\01_Flight\\01_LongTermThruster\\01\\p311b01_long_term_thruster", MyModelsEnum.p311b01_long_term_thruster, MyModelsEnum.p311b01_long_term_thruster_LOD1, null);
            AddEntityModels("Models2\\Prefabs\\03_Modules\\02_Supply\\01_PowerGeneration\\03\\p321c07_generator_center", MyModelsEnum.p321c07_generator_center, MyModelsEnum.p321c07_generator_center_LOD1, null);
            AddEntityModels("Models2\\Prefabs\\03_Modules\\02_Supply\\01_PowerGeneration\\03\\p321c07_generator_propeller_1", MyModelsEnum.p321c07_generator_propeller_1, null, MyModelsEnum.p321c07_generator_propeller_1_COL);
            AddEntityModels("Models2\\Prefabs\\03_Modules\\02_Supply\\01_PowerGeneration\\03\\p321c07_generator_propeller_2", MyModelsEnum.p321c07_generator_propeller_2, null, MyModelsEnum.p321c07_generator_propeller_2_COL);
            AddEntityModels("Models2\\Prefabs\\03_Modules\\02_Supply\\01_PowerGeneration\\03\\p321c07_generator", MyModelsEnum.p321c07_generator, MyModelsEnum.p321c07_generator_LOD1, MyModelsEnum.p321c07_generator_COL);
            AddEntityModels("Models2\\Prefabs\\02_Shells\\02_Chambers\\10\\p221k01_chamber_v2", MyModelsEnum.p221k01_chamber_v2, MyModelsEnum.p221k01_chamber_v2_LOD1, null);
            

            AddModel(new MyModel("Models2\\Prefabs\\05_Details\\08_Barrels\\p581_a01_barrel_biohazard", MyMeshDrawTechnique.MESH, MyModelsEnum.p581_a01_barrel_biohazard));
            AddModel(new MyModel("Models2\\Prefabs\\05_Details\\08_Barrels\\p581_a01_barrel_biohazard_2", MyMeshDrawTechnique.MESH, MyModelsEnum.p581_a01_barrel_biohazard_2));
            AddModel(new MyModel("Models2\\Prefabs\\05_Details\\08_Barrels\\p581_a01_nuke_barrel", MyMeshDrawTechnique.MESH, MyModelsEnum.p581_a01_nuke_barrel));
            AddModel(new MyModel("Models2\\Prefabs\\05_Details\\08_Barrels\\p581_a01_red_barrel", MyMeshDrawTechnique.MESH, MyModelsEnum.p581_a01_red_barrel));
            AddModel(new MyModel("Models2\\Prefabs\\05_Details\\08_Barrels\\p581_a01_simple_barrel", MyMeshDrawTechnique.MESH, MyModelsEnum.p581_a01_simple_barrel));
            AddModel(new MyModel("Models2\\Prefabs\\05_Details\\08_Barrels\\p581_a01_simple_barrel_2", MyMeshDrawTechnique.MESH, MyModelsEnum.p581_a01_simple_barrel_2));
            AddModel(new MyModel("Models2\\ObjectsFloating\\cargo_box_11", MyMeshDrawTechnique.MESH, MyModelsEnum.cargo_box_11));
            AddModel(new MyModel("Models2\\ObjectsFloating\\cargo_box_12", MyMeshDrawTechnique.MESH, MyModelsEnum.cargo_box_12));
            AddModel(new MyModel("Models2\\Prefabs\\05_Details\\08_Barrels\\Barrel_prop_A", MyMeshDrawTechnique.MESH, MyModelsEnum.Barrel_prop_A));
            AddModel(new MyModel("Models2\\Prefabs\\05_Details\\08_Barrels\\Barrel_prop_B", MyMeshDrawTechnique.MESH, MyModelsEnum.Barrel_prop_B));
            AddModel(new MyModel("Models2\\Prefabs\\05_Details\\08_Barrels\\Barrel_prop_C", MyMeshDrawTechnique.MESH, MyModelsEnum.Barrel_prop_C));
            //AddModel(new MyModel("Models2\\Prefabs\\05_Details\\08_Barrels\\Barrel_prop_D", MyMeshDrawTechnique.MESH, MyModelsEnum.Barrel_prop_D));
            //AddModel(new MyModel("Models2\\Prefabs\\05_Details\\08_Barrels\\Barrel_prop_E", MyMeshDrawTechnique.MESH, MyModelsEnum.Barrel_prop_E));
            AddModel(new MyModel("Models2\\Prefabs\\05_Details\\08_Barrels\\p581_a01_universal_barrel_COL", MyMeshDrawTechnique.MESH, MyModelsEnum.p581_a01_universal_barrel_COL));
            AddModel(new MyModel("Models2\\ObjectsFloating\\CargoBox_prop_A", MyMeshDrawTechnique.MESH, MyModelsEnum.CargoBox_prop_A));
            AddModel(new MyModel("Models2\\ObjectsFloating\\CargoBox_prop_B", MyMeshDrawTechnique.MESH, MyModelsEnum.CargoBox_prop_B));
            AddModel(new MyModel("Models2\\ObjectsFloating\\CargoBox_prop_C", MyMeshDrawTechnique.MESH, MyModelsEnum.CargoBox_prop_C));
            AddEntityModels("Models2\\Prefabs\\05_Details\\09_Ammo\\CannonBall_Capsule_1", MyModelsEnum.CannonBall_Capsule_1, null, MyModelsEnum.CannonBall_Capsule_1_COL);
            AddEntityModels("Models2\\Prefabs\\05_Details\\09_Ammo\\Gattling_ammo_belt", MyModelsEnum.gattling_ammo_belt, null, MyModelsEnum.gattling_ammo_belt_COL);
            AddEntityModels("Models2\\Prefabs\\05_Details\\09_Ammo\\Missile_pack01", MyModelsEnum.Missile_pack01, null, MyModelsEnum.Missile_pack01_COL);
            AddEntityModels("Models2\\Prefabs\\05_Details\\09_Ammo\\Missile_pack02", MyModelsEnum.Missile_pack02, null, MyModelsEnum.Missile_pack02_COL);
            AddEntityModels("Models2\\Prefabs\\05_Details\\09_Ammo\\Missile_plazma01", MyModelsEnum.Missile_plazma01, null, MyModelsEnum.Missile_plazma01_COL);
            AddEntityModels("Models2\\Prefabs\\05_Details\\09_Ammo\\Missile_stack_biochem01", MyModelsEnum.Missile_stack_biochem01, null, MyModelsEnum.Missile_stack_biochem01_COL);
            AddEntityModels("Models2\\Prefabs\\05_Details\\08_Barrels\\p581_a01_o2_barrel", MyModelsEnum.p581_a01_o2_barrel, null, MyModelsEnum.p581_a01_o2_barrel_COL);

            //AddModel(new MyModel("Models2\\Prefabs\\05_Details\\09_Ammo\\Nuclear_Warhead_closed", MyMeshDrawTechnique.MESH, MyModelsEnum.Nuclear_Warhead_closed));
            AddEntityModels("Models2\\Prefabs\\05_Details\\09_Ammo\\Nuclear_Warhead_closed", MyModelsEnum.Nuclear_Warhead_closed, MyModelsEnum.Nuclear_Warhead_closed_LOD1, MyModelsEnum.Nuclear_Warhead_closed_COL);
            //AddModel(new MyModel("Models2\\Prefabs\\05_Details\\09_Ammo\\Nuclear_Warhead_open", MyMeshDrawTechnique.MESH, MyModelsEnum.Nuclear_Warhead_open));
            AddEntityModels("Models2\\Prefabs\\05_Details\\09_Ammo\\Nuclear_Warhead_open", MyModelsEnum.Nuclear_Warhead_open, MyModelsEnum.Nuclear_Warhead_open_LOD1, MyModelsEnum.Nuclear_Warhead_open_COL);
            AddModel(new MyModel("Models2\\ObjectsFloating\\CargoBox_prop_D", MyMeshDrawTechnique.MESH, MyModelsEnum.CargoBox_prop_D));
            AddModel(new MyModel("Models2\\Prefabs\\05_Details\\08_Barrels\\p581_a01_nuke_barrel_1", MyMeshDrawTechnique.MESH, MyModelsEnum.p581_a01_nuke_barrel_1));
            AddModel(new MyModel("Models2\\Prefabs\\05_Details\\08_Barrels\\p581_a01_simple_barrel_3", MyMeshDrawTechnique.MESH, MyModelsEnum.p581_a01_simple_barrel_3));
            AddEntityModels("Models2\\Prefabs\\02_Shells\\02_Chambers\\11\\p221L01_chamber_v1", MyModelsEnum.p221L01_chamber_v1, MyModelsEnum.p221L01_chamber_v1_LOD1, MyModelsEnum.p221L01_chamber_v1_COL);

            AddEntityModels("Models2\\Prefabs\\02_Shells\\02_Chambers\\12\\p221M01_chamber_bottom_v1", MyModelsEnum.p221m01_chamber_bottom_v1, MyModelsEnum.p221m01_chamber_bottom_v1_LOD1, MyModelsEnum.p221m01_chamber_bottom_v1_COL);
            AddEntityModels("Models2\\Prefabs\\02_Shells\\02_Chambers\\12\\p221M01_chamber_center_v1", MyModelsEnum.p221m01_chamber_center_v1, MyModelsEnum.p221m01_chamber_center_v1_LOD1, MyModelsEnum.p221m01_chamber_center_v1_COL);
            AddEntityModels("Models2\\Prefabs\\02_Shells\\02_Chambers\\12\\p221M01_chamber_top_v1", MyModelsEnum.p221m01_chamber_top_v1, MyModelsEnum.p221m01_chamber_top_v1_LOD1, MyModelsEnum.p221m01_chamber_top_v1_COL);
            AddEntityModels("Models2\\Prefabs\\03_Modules\\02_Supply\\01_PowerGeneration\\04\\p321e01_solar_panel", MyModelsEnum.p321e01_solar_panel, MyModelsEnum.p321e01_solar_panel_LOD1, MyModelsEnum.p321e01_solar_panel_COL);
            AddModel(new MyModel("Models2\\Prefabs\\01_Beams\\01_Large\\01\\p110a06_solid_beam_straight_420m", MyMeshDrawTechnique.MESH, MyModelsEnum.p110a06_solid_beam_straight_420m));

            AddEntityModels("Models2\\Prefabs\\03_Modules\\01_Flight\\01_LongTermThruster\\02\\p311b01_cut_thruster", MyModelsEnum.p311b01_cut_thruster, MyModelsEnum.p311b01_cut_thruster_LOD1, MyModelsEnum.p311b01_cut_thruster_COL);
            AddEntityModels("Models2\\Prefabs\\03_Modules\\01_Flight\\02_ShortTermThruster\\02\\p312b01_cut_thruster_lateral", MyModelsEnum.p312b01_cut_thruster_lateral, MyModelsEnum.p312b01_cut_thruster_lateral_LOD1, MyModelsEnum.p312b01_cut_thruster_lateral_COL);
            AddEntityModels("Models2\\Prefabs\\03_Modules\\01_Flight\\02_ShortTermThruster\\02\\p312b02_cut_thruster_latitude", MyModelsEnum.p312b02_cut_thruster_latitude, MyModelsEnum.p312b02_cut_thruster_latitude_LOD1, MyModelsEnum.p312b02_cut_thruster_latitude_COL);
            AddEntityModels("Models2\\Prefabs\\05_Details\\04_Other\\alien_detector_unit", MyModelsEnum.alien_detector_unit, null, MyModelsEnum.alien_detector_unit_COL);
            AddEntityModels("Models2\\Prefabs\\03_Modules\\08_MannedObjects\\01_LivingQuarters\\03\\p381_c01_building5", MyModelsEnum.p381_c01_building5, MyModelsEnum.p381_c01_building5_LOD1, MyModelsEnum.p381_c01_building5_COL);
            AddEntityModels("Models2\\Prefabs\\03_Modules\\08_MannedObjects\\01_LivingQuarters\\03\\p381_c01_building6", MyModelsEnum.p381_c01_building6, MyModelsEnum.p381_c01_building6_LOD1, MyModelsEnum.p381_c01_building6_COL);
            AddEntityModels("Models2\\Prefabs\\03_Modules\\08_MannedObjects\\01_LivingQuarters\\03\\p381_c01_building7", MyModelsEnum.p381_c01_building7, MyModelsEnum.p381_c01_building7_LOD1, MyModelsEnum.p381_c01_building7_COL);
            AddEntityModels("Models2\\Prefabs\\02_Shells\\02_Chambers\\13\\p221n01_chamber_v1", MyModelsEnum.p221n01_chamber_v1, MyModelsEnum.p221n01_chamber_v1_LOD1, MyModelsEnum.p221n01_chamber_v1_COL);
            AddEntityModels("Models2\\Prefabs\\05_Details\\03_Signs\\531_d_medic_cross", MyModelsEnum.p531_d_medic_cross, null, MyModelsEnum.p531_d_medic_cross_COL);

            AddEntityModels("Models2\\Prefabs\\05_Details\\04_Other\\541_screen_A", MyModelsEnum.p541_screen_A, MyModelsEnum.p541_screen_A_LOD1, MyModelsEnum.p541_screen_A_COL);
            AddEntityModels("Models2\\Prefabs\\05_Details\\04_Other\\541_screen_B", MyModelsEnum.p541_screen_B, MyModelsEnum.p541_screen_B_LOD1, MyModelsEnum.p541_screen_B_COL);
            AddEntityModels("Models2\\Prefabs\\05_Details\\04_Other\\541_terminal_A", MyModelsEnum.p541_terminal_A, MyModelsEnum.p541_terminal_A_LOD1, MyModelsEnum.p541_terminal_A_COL);

            AddEntityModels("Models2\\Prefabs\\05_Details\\04_Other\\alien_artefact", MyModelsEnum.alien_artefact, null, MyModelsEnum.alien_artefact_COL);

            AddEntityModels("Models2\\Prefabs\\05_Details\\04_Other\\bomb", MyModelsEnum.bomb, null, MyModelsEnum.bomb_COL);

            AddEntityModels("Models2\\Prefabs\\02_Shells\\02_Chambers\\14\\rail_gun", MyModelsEnum.rail_gun, MyModelsEnum.rail_gun_LOD1, MyModelsEnum.rail_gun_COL);
            AddEntityModels("Models2\\Prefabs\\03_Modules\\04_Industry\\08_Recycle\\01\\p345_a01_recycle_sphere", MyModelsEnum.p345_a01_recycle_sphere, MyModelsEnum.p345_a01_recycle_sphere_LOD1, MyModelsEnum.p345_a01_recycle_sphere_COL);

            AddEntityModels("Models2\\Prefabs\\02_Shells\\02_Chambers\\15\\prison", MyModelsEnum.prison, null, MyModelsEnum.prison_COL);
            AddEntityModels("Models2\\Prefabs\\05_Details\\04_Other\\vendor", MyModelsEnum.vendor, null, MyModelsEnum.vendor_COL);

            AddEntityModels("Models2\\Prefabs\\05_Details\\04_Other\\hangar_screen", MyModelsEnum.hangar_screen, null, MyModelsEnum.hangar_screen_COL);            

            AddEntityModels("Models2\\Prefabs\\ScannerPlane", MyModelsEnum.ScannerPlane, null, null);
            AddEntityModels("Models2\\Prefabs\\ScannerRays", MyModelsEnum.ScannerRays, null, null);

            AddEntityModels("Models2\\Prefabs\\05_Details\\01_Billboards\\511_a17_billboard_portrait_1", MyModelsEnum.p511_a17_billboard_portrait_1, null, MyModelsEnum.p511_a17_billboard_portrait_1_COL);
            AddEntityModels("Models2\\Prefabs\\05_Details\\01_Billboards\\511_a17_billboard_portrait_2", MyModelsEnum.p511_a17_billboard_portrait_2, null, null);

            AddEntityModels("Models2\\Prefabs\\05_Details\\01_Billboards\\511_a18_billboard", MyModelsEnum.p511_a18_billboard, MyModelsEnum.p511_a18_billboard_LOD1, MyModelsEnum.p511_a18_billboard_COL);
            AddEntityModels("Models2\\Prefabs\\05_Details\\01_Billboards\\511_a19_billboard", MyModelsEnum.p511_a19_billboard, MyModelsEnum.p511_a19_billboard_LOD1, MyModelsEnum.p511_a19_billboard_COL);


            //  Check if we didn't forget to load some model
            foreach (int i in Enum.GetValues(typeof(MyModelsEnum)))
            {
                MyModel myModel = m_models[i];
                MyModelsEnum modelEnum = (MyModelsEnum)i;
                MyMwcLog.WriteLine("Checking model enum id [" + i + "] " + modelEnum, LoggingOptions.ENUM_CHECKING);   //  Here we show model enum as a string. It won't work after obfuscation, but we need it only in debug (before obfuscation), so it's OK.
                MyCommonDebugUtils.AssertRelease(myModel != null);
            }

            //FindMissingAndUnusedModels();

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyModels.InitModels - END");
        }

        /// <summary>
        /// Used to find models that does not have LOD1 or COL set, and models that can have those models set
        /// </summary>
        private static void FindMissingAndUnusedModels()
        {
            string contentDirectory = @"C:\KeenSWH\MinerWars\Sources\MinerWars\Content";

            using (StreamWriter missing = new StreamWriter(File.OpenWrite("models-missing.csv")), notused = new StreamWriter(File.OpenWrite("models-notused.csv")))
            {
                missing.WriteLine("Model,Triangles,Volume,Missing");
                notused.WriteLine("Model,NotUsed");
                foreach (int i in Enum.GetValues(typeof(MyModelsEnum)))
                {
                    MyModel myModel = m_models[i];
                    string firstAssetName = NormalizeAssetName(myModel.AssetName);

                    if (!firstAssetName.EndsWith("LOD0", StringComparison.InvariantCultureIgnoreCase))
                        continue;

                    bool hasLod1 = false;
                    bool hasCol = false;
                    foreach (int j in Enum.GetValues(typeof(MyModelsEnum)))
                    {
                        if (i == j)
                            continue;

                        MyModel secondModel = m_models[j];
                        string assetName = NormalizeAssetName(secondModel.AssetName);

                        if (RemoveModifier(firstAssetName) != RemoveModifier(assetName))
                        {
                            continue;
                        }

                        if (secondModel.AssetName.EndsWith("COL", StringComparison.InvariantCultureIgnoreCase))
                            hasCol = true;
                        if (secondModel.AssetName.EndsWith("LOD1", StringComparison.InvariantCultureIgnoreCase))
                            hasLod1 = true;

                        if (hasCol && hasLod1)
                            break;
                    }

                    if (!hasLod1)
                    {
                        bool lod1Exists = File.Exists(Path.Combine(contentDirectory, RemoveModifier(myModel.AssetName) + "_LOD1.fbx"));
                        if (lod1Exists)
                        {
                            notused.WriteLine("{0},LOD1", myModel.AssetName);
                        }
                        myModel.LoadData();
                        missing.WriteLine("{0},{1},{2},LOD1", myModel.AssetName, myModel.GetTrianglesCount(), myModel.BoundingBox.Volume());
                        myModel.UnloadData();
                    }
                    if (!hasCol)
                    {
                        bool colExists = File.Exists(Path.Combine(contentDirectory, RemoveModifier(myModel.AssetName) + "_COL.fbx"));
                        if (colExists)
                        {
                            notused.WriteLine("{0},COL", myModel.AssetName);
                        }
                        myModel.LoadData();
                        missing.WriteLine("{0},{1},{2},COL", myModel.AssetName, myModel.GetTrianglesCount(), myModel.BoundingBox.Volume());
                        myModel.UnloadData();
                    }
                }
                missing.Flush();
                missing.Close();
                notused.Flush();
                notused.Close();
            }
        }

        private static string NormalizeAssetName(string assetName)
        {
            if (!assetName.EndsWith("LOD1", StringComparison.InvariantCultureIgnoreCase) && !assetName.EndsWith("COL", StringComparison.InvariantCultureIgnoreCase))
            {
                if (!assetName.EndsWith("LOD0", StringComparison.InvariantCultureIgnoreCase))
                {
                    assetName = assetName + "_LOD0";
                }
            }
            return assetName;
        }

        private static string RemoveModifier(string assetName)
        {
            return assetName.Replace("_LOD0", "").Replace("_LOD1", "").Replace("_COL", "");
        }


        static void AddModel(MyModel model)
        {
            //  If we already have an object in the array with this model id, then it means we are trying to load it twice (duplicity)
            //  The idea is: every model is instantiated only once, and is also only once in m_models. No duplicities.
            MyCommonDebugUtils.AssertRelease(m_models[(int)model.ModelEnum] == null);

            m_models[(int)model.ModelEnum] = model;
            m_modelsByAssertName.Add(model.AssetName, model);
        }

        static void AddEntityModels(string assetName, MyModelsEnum lod0, MyModelsEnum? lod1, MyModelsEnum? collision, bool keepInMemory = false)
        {
            AddModel(new MyModel(assetName, MyMeshDrawTechnique.MESH, lod0, keepInMemory));

            if (lod1.HasValue)
                AddModel(new MyModel(assetName + "_LOD1", MyMeshDrawTechnique.MESH, lod1.Value, keepInMemory));
            if (collision.HasValue)
                AddModel(new MyModel(assetName + "_COL", MyMeshDrawTechnique.MESH, collision.Value, keepInMemory));
        }
    }
}
