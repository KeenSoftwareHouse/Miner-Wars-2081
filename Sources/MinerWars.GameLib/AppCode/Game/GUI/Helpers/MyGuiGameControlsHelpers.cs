using System;
using System.Collections.Generic;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Utils;
using SysUtils.Utils;

namespace MinerWars.AppCode.Game.GUI.Helpers
{
    static class MyGuiGameControlsHelpers
    {
        static Dictionary<MyGameControlEnums, MyGuiHelperBase> m_gameControlHelpers = new Dictionary<MyGameControlEnums, MyGuiHelperBase>();
        static Dictionary<MyEditorControlEnums, MyGuiHelperBase> m_editorControlHelpers = new Dictionary<MyEditorControlEnums, MyGuiHelperBase>();

        //Arrays of enums values
        public static Array MyGameControllerHelperTypesEnumValues { get; private set; }
        public static Array MyControlHelperTypesEnumValues { get; private set; }
        public static Array MyEditorControlHelperTypesEnumValues { get; private set; }

        static MyGuiGameControlsHelpers()
        {
            MyMwcLog.WriteLine("MyGuiGameControlsHelpers()");

            MyControlHelperTypesEnumValues = Enum.GetValues(typeof(MyGameControlEnums));
            MyEditorControlHelperTypesEnumValues = Enum.GetValues(typeof(MyEditorControlEnums));
        }

        public static MyGuiHelperBase GetGameControlHelper(MyGameControlEnums controlHelper)
        {
            MyGuiHelperBase ret;
            if (m_gameControlHelpers.TryGetValue(controlHelper, out ret))
                return ret;
            else
                return null;
        }

        public static MyGuiHelperBase GetEditorControlHelper(MyEditorControlEnums controlHelper)
        {
            MyGuiHelperBase ret;
            if (m_editorControlHelpers.TryGetValue(controlHelper, out ret))
                return ret;
            else
                return null;
        }

        public static void UnloadContent()
        {
            m_gameControlHelpers.Clear();
            m_editorControlHelpers.Clear();
        }

        public static void LoadContent()
        {
            #region Game Control helpers

            m_gameControlHelpers.Add(MyGameControlEnums.FIRE_PRIMARY, new MyGuiHelperBase(null, MyTextsWrapperEnum.FirePrimaryWeapon));
            m_gameControlHelpers.Add(MyGameControlEnums.FIRE_SECONDARY, new MyGuiHelperBase(null, MyTextsWrapperEnum.FireSecondaryWeapon));
            m_gameControlHelpers.Add(MyGameControlEnums.FIRE_THIRD, new MyGuiHelperBase(null, MyTextsWrapperEnum.FireThirdWeapon));
            m_gameControlHelpers.Add(MyGameControlEnums.FIRE_FOURTH, new MyGuiHelperBase(null, MyTextsWrapperEnum.FireFourthWeapon));
            m_gameControlHelpers.Add(MyGameControlEnums.FIRE_FIFTH, new MyGuiHelperBase(null, MyTextsWrapperEnum.FireFifthWeapon));
            m_gameControlHelpers.Add(MyGameControlEnums.FORWARD, new MyGuiHelperBase(null, MyTextsWrapperEnum.ForwardThrust));
            //m_gameControlHelpers.Add(MyGameControlEnums.FULLSCREEN_RADAR, new MyGuiHelperBase(null, MyTextsWrapperEnum.FullscreenRadar));
            m_gameControlHelpers.Add(MyGameControlEnums.HARVEST, new MyGuiHelperBase(null, MyTextsWrapperEnum.Harvest));
            m_gameControlHelpers.Add(MyGameControlEnums.HEADLIGHTS, new MyGuiHelperBase(null, MyTextsWrapperEnum.ToggleHeadlights));
            m_gameControlHelpers.Add(MyGameControlEnums.MOVEMENT_SLOWDOWN, new MyGuiHelperBase(null, MyTextsWrapperEnum.MovementSlowdown));
            m_gameControlHelpers.Add(MyGameControlEnums.QUICK_ZOOM, new MyGuiHelperBase(null, MyTextsWrapperEnum.QuickZoom));
            //m_gameControlHelpers.Add(MyGameControlEnums.RADAR_ZOOM_IN, new MyGuiHelperBase(null, MyTextsWrapperEnum.RadarZoomIn));
            //m_gameControlHelpers.Add(MyGameControlEnums.RADAR_ZOOM_OUT, new MyGuiHelperBase(null, MyTextsWrapperEnum.RadarZoomOut));
            m_gameControlHelpers.Add(MyGameControlEnums.REAR_CAM, new MyGuiHelperBase(null, MyTextsWrapperEnum.RearCam));
            m_gameControlHelpers.Add(MyGameControlEnums.REVERSE, new MyGuiHelperBase(null, MyTextsWrapperEnum.ReverseThrust));
            m_gameControlHelpers.Add(MyGameControlEnums.ROLL_LEFT, new MyGuiHelperBase(null, MyTextsWrapperEnum.RollLeft));
            m_gameControlHelpers.Add(MyGameControlEnums.ROLL_RIGHT, new MyGuiHelperBase(null, MyTextsWrapperEnum.RollRight));
            m_gameControlHelpers.Add(MyGameControlEnums.SELECT_AMMO_BULLET, new MyGuiHelperBase(null, MyTextsWrapperEnum.SelectBulletAmmo));
            m_gameControlHelpers.Add(MyGameControlEnums.SELECT_AMMO_MISSILE, new MyGuiHelperBase(null, MyTextsWrapperEnum.SelectMissileAmmo));
            m_gameControlHelpers.Add(MyGameControlEnums.SELECT_AMMO_CANNON, new MyGuiHelperBase(null, MyTextsWrapperEnum.SelectCannonAmmo));
            m_gameControlHelpers.Add(MyGameControlEnums.SELECT_AMMO_UNIVERSAL_LAUNCHER_FRONT, new MyGuiHelperBase(null, MyTextsWrapperEnum.SelectFrontLauncherAmmo));
            m_gameControlHelpers.Add(MyGameControlEnums.SELECT_AMMO_UNIVERSAL_LAUNCHER_BACK, new MyGuiHelperBase(null, MyTextsWrapperEnum.SelectBackLauncherAmmo));            
            m_gameControlHelpers.Add(MyGameControlEnums.STRAFE_LEFT, new MyGuiHelperBase(null, MyTextsWrapperEnum.StrafeLeft));
            m_gameControlHelpers.Add(MyGameControlEnums.STRAFE_RIGHT, new MyGuiHelperBase(null, MyTextsWrapperEnum.StrafeRight));
            //m_gameControlHelpers.Add(MyGameControlEnums.SWITCH_RADAR_MODE, new MyGuiHelperBase(null, MyTextsWrapperEnum.SwitchRadarMode));
            m_gameControlHelpers.Add(MyGameControlEnums.UP_THRUST, new MyGuiHelperBase(null, MyTextsWrapperEnum.UpThrust));
            m_gameControlHelpers.Add(MyGameControlEnums.ZOOM_IN, new MyGuiHelperBase(null, MyTextsWrapperEnum.ZoomIn));
            m_gameControlHelpers.Add(MyGameControlEnums.ZOOM_OUT, new MyGuiHelperBase(null, MyTextsWrapperEnum.ZoomOut));
            m_gameControlHelpers.Add(MyGameControlEnums.DOWN_THRUST, new MyGuiHelperBase(null, MyTextsWrapperEnum.DownThrust));
            //m_gameControlHelpers.Add(MyGameControlEnums.ENGINE_SHUTDOWN, new MyGuiHelperBase(null, MyTextsWrapperEnum.EngineShutdown));
            m_gameControlHelpers.Add(MyGameControlEnums.AUTO_LEVEL, new MyGuiHelperBase(null, MyTextsWrapperEnum.AutoLevel));
            m_gameControlHelpers.Add(MyGameControlEnums.DRILL, new MyGuiHelperBase(null, MyTextsWrapperEnum.UseDrill));
            m_gameControlHelpers.Add(MyGameControlEnums.AFTERBURNER, new MyGuiHelperBase(null, MyTextsWrapperEnum.Afterburner));
            m_gameControlHelpers.Add(MyGameControlEnums.VIEW_MODE, new MyGuiHelperBase(null, MyTextsWrapperEnum.ViewMode));
            m_gameControlHelpers.Add(MyGameControlEnums.WEAPON_SPECIAL, new MyGuiHelperBase(null, MyTextsWrapperEnum.WeaponSpecial));
            m_gameControlHelpers.Add(MyGameControlEnums.ROTATION_LEFT, new MyGuiHelperBase(null, MyTextsWrapperEnum.RotationLeft));
            m_gameControlHelpers.Add(MyGameControlEnums.ROTATION_RIGHT, new MyGuiHelperBase(null, MyTextsWrapperEnum.RotationRight));
            m_gameControlHelpers.Add(MyGameControlEnums.ROTATION_UP, new MyGuiHelperBase(null, MyTextsWrapperEnum.RotationUp));
            m_gameControlHelpers.Add(MyGameControlEnums.ROTATION_DOWN, new MyGuiHelperBase(null, MyTextsWrapperEnum.RotationDown));
            //m_gameControlHelpers.Add(MyGameControlEnums.COMM_AVATAR_SELECT, new MyGuiHelperBase(null, MyTextsWrapperEnum.CommAvatarSelect));
            m_gameControlHelpers.Add(MyGameControlEnums.NOTIFICATION_CONFIRMATION, new MyGuiHelperBase(null, MyTextsWrapperEnum.NotificationConfirmation));
            m_gameControlHelpers.Add(MyGameControlEnums.WHEEL_CONTROL, new MyGuiHelperBase(null, MyTextsWrapperEnum.WheelControl));            
            m_gameControlHelpers.Add(MyGameControlEnums.MISSION_DIALOG, new MyGuiHelperBase(null, MyTextsWrapperEnum.MissionDialog));
            m_gameControlHelpers.Add(MyGameControlEnums.HEADLIGTHS_DISTANCE, new MyGuiHelperBase(null, MyTextsWrapperEnum.HeadlightsDistance));
            m_gameControlHelpers.Add(MyGameControlEnums.TRAVEL, new MyGuiHelperBase(null, MyTextsWrapperEnum.Travel));
            m_gameControlHelpers.Add(MyGameControlEnums.PREVIOUS_CAMERA, new MyGuiHelperBase(null, MyTextsWrapperEnum.PreviousCamera));
            m_gameControlHelpers.Add(MyGameControlEnums.NEXT_CAMERA, new MyGuiHelperBase(null, MyTextsWrapperEnum.NextCamera));
            m_gameControlHelpers.Add(MyGameControlEnums.GPS, new MyGuiHelperBase(null, MyTextsWrapperEnum.UseGPS));
            m_gameControlHelpers.Add(MyGameControlEnums.USE, new MyGuiHelperBase(null, MyTextsWrapperEnum.UseHackTake));
            m_gameControlHelpers.Add(MyGameControlEnums.CHANGE_DRONE_MODE, new MyGuiHelperBase(null, MyTextsWrapperEnum.ChangeDroneMode));
            m_gameControlHelpers.Add(MyGameControlEnums.INVENTORY, new MyGuiHelperBase(null, MyTextsWrapperEnum.Inventory));
            m_gameControlHelpers.Add(MyGameControlEnums.FIRE_HOLOGRAM_FRONT   , new MyGuiHelperBase(null, MyTextsWrapperEnum.FireHologramFront  ));
            m_gameControlHelpers.Add(MyGameControlEnums.FIRE_HOLOGRAM_BACK    , new MyGuiHelperBase(null, MyTextsWrapperEnum.FireHologramBack   ));
            m_gameControlHelpers.Add(MyGameControlEnums.FIRE_BASIC_MINE_FRONT , new MyGuiHelperBase(null, MyTextsWrapperEnum.FireBasicMineFront ));
            m_gameControlHelpers.Add(MyGameControlEnums.FIRE_BASIC_MINE_BACK  , new MyGuiHelperBase(null, MyTextsWrapperEnum.FireBasicMineBack  ));
            m_gameControlHelpers.Add(MyGameControlEnums.FIRE_SMART_MINE_FRONT , new MyGuiHelperBase(null, MyTextsWrapperEnum.FireSmartMineFront ));
            m_gameControlHelpers.Add(MyGameControlEnums.FIRE_SMART_MINE_BACK  , new MyGuiHelperBase(null, MyTextsWrapperEnum.FireSmartMineBack  ));
            m_gameControlHelpers.Add(MyGameControlEnums.FIRE_FLASH_BOMB_FRONT , new MyGuiHelperBase(null, MyTextsWrapperEnum.FireFlashBombFront ));
            m_gameControlHelpers.Add(MyGameControlEnums.FIRE_FLASH_BOMB_BACK  , new MyGuiHelperBase(null, MyTextsWrapperEnum.FireFlashBombBack  ));
            m_gameControlHelpers.Add(MyGameControlEnums.FIRE_DECOY_FLARE_FRONT, new MyGuiHelperBase(null, MyTextsWrapperEnum.FireDecoyFlareFront));
            m_gameControlHelpers.Add(MyGameControlEnums.FIRE_DECOY_FLARE_BACK , new MyGuiHelperBase(null, MyTextsWrapperEnum.FireDecoyFlareBack ));
            m_gameControlHelpers.Add(MyGameControlEnums.FIRE_SMOKE_BOMB_FRONT , new MyGuiHelperBase(null, MyTextsWrapperEnum.FireSmokeBombFront ));
            m_gameControlHelpers.Add(MyGameControlEnums.FIRE_SMOKE_BOMB_BACK  , new MyGuiHelperBase(null, MyTextsWrapperEnum.FireSmokeBombBack  ));
            m_gameControlHelpers.Add(MyGameControlEnums.DRONE_DEPLOY, new MyGuiHelperBase(null, MyTextsWrapperEnum.DroneDeploy));
            m_gameControlHelpers.Add(MyGameControlEnums.DRONE_CONTROL, new MyGuiHelperBase(null, MyTextsWrapperEnum.DroneControl));
            m_gameControlHelpers.Add(MyGameControlEnums.CONTROL_SECONDARY_CAMERA, new MyGuiHelperBase(null, MyTextsWrapperEnum.ControlSecondaryCamera));
            m_gameControlHelpers.Add(MyGameControlEnums.PREV_TARGET, new MyGuiHelperBase(null, MyTextsWrapperEnum.PreviousTarget));
            m_gameControlHelpers.Add(MyGameControlEnums.NEXT_TARGET, new MyGuiHelperBase(null, MyTextsWrapperEnum.NextTarget));
            m_gameControlHelpers.Add(MyGameControlEnums.CHAT, new MyGuiHelperBase(null, MyTextsWrapperEnum.OpenChat));
            m_gameControlHelpers.Add(MyGameControlEnums.SCORE, new MyGuiHelperBase(null, MyTextsWrapperEnum.OpenScore));
            #endregion

            #region Editor Control helpers

            m_editorControlHelpers.Add(MyEditorControlEnums.PRIMARY_ACTION_KEY, new MyGuiHelperBase(null, MyTextsWrapperEnum.EditorPrimaryActionKey));
            m_editorControlHelpers.Add(MyEditorControlEnums.SECONDARY_ACTION_KEY, new MyGuiHelperBase(null, MyTextsWrapperEnum.EditorSecondaryActionKey));
            m_editorControlHelpers.Add(MyEditorControlEnums.DECREASE_GRID_SCALE, new MyGuiHelperBase(null, MyTextsWrapperEnum.EditorDecreaseGridScale));
            m_editorControlHelpers.Add(MyEditorControlEnums.INCREASE_GRID_SCALE, new MyGuiHelperBase(null, MyTextsWrapperEnum.EditorIncreaseGridScale));
            m_editorControlHelpers.Add(MyEditorControlEnums.VOXEL_HAND, new MyGuiHelperBase(null, MyTextsWrapperEnum.EditorVoxelHand));
            m_editorControlHelpers.Add(MyEditorControlEnums.SWITCH_GIZMO_MODE, new MyGuiHelperBase(null, MyTextsWrapperEnum.EditorSwitchGizmoMode));
            m_editorControlHelpers.Add(MyEditorControlEnums.SWITCH_GIZMO_SPACE, new MyGuiHelperBase(null, MyTextsWrapperEnum.EditorSwitchGizmoSpace));
            
            #endregion
        }
    }
}
