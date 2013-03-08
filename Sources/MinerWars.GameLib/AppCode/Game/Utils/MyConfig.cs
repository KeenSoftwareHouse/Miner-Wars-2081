using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Render;
using MinerWars.AppCode.Game.VideoMode;
using MinerWars.CommonLIB.AppCode.Utils;
using SysUtils;
using SysUtils.Utils;
using MinerWars.AppCode.App;

//  This class encapsulated read/write access to our config file - MinerWars.cfg - stored in user's local files
//  It assumes that config file may be non existing, or that some values may be missing or in wrong format - this class can handle it
//  and in such case will offer default values -> BUT YOU HAVE TO HELP IT... HOW? -> when writing getter from a new property,
//  you have to return default value in case it's null or empty or invalid!!
//  IMPORTANT: Never call get/set on this class properties from real-time code (during gameplay), e.g. don't do AddCue2D(cueEnum, MyConfig.VolumeMusic)
//  IMPORTANT: Only from loading and initialization methods.

namespace MinerWars.AppCode.Game.Utils
{
    static class MyConfig
    {
        //  Here we store parameter name (dictionary key) in its value (dictionary value)
        static readonly Dictionary<string, string> m_values = new Dictionary<string, string>();

        //  Constants for mapping between our get/set properties and parameters inside the config file
        static readonly string USERNAME = "Username";
        static readonly string PASSWORD = "Password";
        static readonly string LAST_LOGIN_WAS_SUCCESSFUL = "LastLoginWasSuccessful";
        static readonly string REMEBER_USERNAME_AND_PASSWORD = "RememberUsernameAndPassword";
        static readonly string AUTOLOGIN = "Autologin";
        static readonly string RENDER_QUALITY = "RenderQuality";
        static readonly string FIELD_OF_VIEW = "FieldOfView";
        static readonly string FIELD_OF_VIEW2 = "FieldOfView2";
        static readonly string SCREEN_WIDTH = "ScreenWidth";
        static readonly string SCREEN_HEIGHT = "ScreenHeight";
        static readonly string FULL_SCREEN = "FullScreen";
        static readonly string VIDEO_ADAPTER = "VideoAdapter";
        static readonly string VERTICAL_SYNC = "VerticalSync";
        static readonly string HARDWARE_CURSOR = "HardwareCursor";
        static readonly string GAME_VOLUME = "GameVolume";
        static readonly string MUSIC_VOLUME = "MusicVolume";
        static readonly string LANGUAGE = "Language";
        static readonly string SUBTITLES = "Subtitles";
        static readonly string NOTIFICATIONS = "Notifications";
        static readonly string CONTROLS_GENERAL = "ControlsGeneral";
        static readonly string CONTROLS_BUTTONS = "ControlsButtons";
        static readonly string EDITOR_CONTROLS_BUTTONS = "EditorControlsButtons";
        static readonly string DISPLAY_UNSELECTED_BOUNDING = "DisplayUnselectedBounding";
        static readonly string USE_CAMERA_CROSSHAIR = "UseCameraCrosshair";
        //static readonly string DISPLAY_PREFAB_CONTAINER_BOUNDING = "DisplayPrefabContainerBounding";
        static readonly string LOCKED_PREFAB_90_DEGREES_ROTATION = "LockedPrefab90DegreesRotation";
        static readonly string SCREENSHOT_SIZE_MULTIPLIER = "ScreenshotSizeMultiplier";
        static readonly string LAST_FRIEND_NAME = "LastFriendSectorName";
        static readonly string LAST_FRIEND_SECTOR_USER_ID = "LastFriendSectorUserId";
        static readonly string LAST_FRIEND_SECTOR_POSITION = "LastFriendSectorPosition";
        static readonly string LAST_MY_SANDBOX_SECTOR = "LastMySandboxSector";
        static readonly string NEED_SHOW_HELPSCREEN = "NeedShowHelpScreen";
        //static readonly string NEED_SHOW_PERFWARNING = "NeedShowPerfWarning";

        static readonly string EDITOR_HIDDEN_WAYPOINT = "EditorHiddenWayPoint";
        static readonly string EDITOR_SELECTABLE_WAYPOINT = "EditorSelectableWayPoint";
        static readonly string EDITOR_HIDDEN_PREFABBASE = "EditorHiddenPrefabBase";
        static readonly string EDITOR_SELECTABLE_PREFABBASE = "EditorSelectablePrefabBase";
        static readonly string EDITOR_HIDDEN_DUMMYPOINT = "EditorHiddenDummyPoint";
        static readonly string EDITOR_SELECTABLE_DUMMYPOINT = "EditorSelectableDummyPoint";
        static readonly string EDITOR_HIDDEN_VOXELMAP = "EditorHiddenVoxelMap";
        static readonly string EDITOR_SELECTABLE_VOXELMAP = "EditorSelectableVoxelMap";
        static readonly string EDITOR_WAYPOINTS_IGNORE_DEPTH = "EditorWaypointsIgnoreDepth";
        static readonly string EDITOR_HIDDEN_SPAWNPOINT = "EditorHiddenSpawnPoint";
        static readonly string EDITOR_SELECTABLE_SPAWNPOINT = "EditorSelectableSpawnPoint";
        static readonly string EDITOR_HIDDEN_INFLUENCESPHERE = "EditorHiddenInfluenceSphere";
        static readonly string EDITOR_SELECTABLE_INFLUENCESPHERE = "EditorSelectableInfluenceSphere";

        static readonly string EDITOR_SHOW_SAFE_AREAS = "EditorShowSafeAreas";
        static readonly string EDITOR_SELECTABLE_SAFE_AREAS = "EditorSelectableSafeAreas";
        static readonly string EDITOR_SHOW_DETECTORS = "EditorShowDetectors";
        static readonly string EDITOR_SELECTABLE_DETECTORS = "EditorSelectableDetectors";
        static readonly string EDITOR_SHOW_PARTICLE_EFFECTS = "EditorShowParticleEffects";
        static readonly string EDITOR_SELECTABLE_PARTICLE_EFFECTS = "EditorSelectableParticleEffects";

        static readonly string EDITOR_ENABLE_LIGHTS_IN_EDITOR = "EditorEnableLightsInEditor";
        static readonly string EDITOR_DISPLAY_PREFAB_CONTAINER_BOUNDING = "EditorDisplayPrefabContainerBounding";
        static readonly string EDITOR_DISPLAY_PREFAB_CONTAINER_AXIS = "EditorDisplayPrefabContainerAxis";
        static readonly string EDITOR_EDITOR_SNAP_POINT_FILTER = "EditorSnapPointFilter";
        static readonly string EDITOR_SAVE_PLAYER_SHIP = "EditorSavePlayerShip";
        static readonly string EDITOR_FIXED_SIZE_SNAP_POINTS = "EditorFixedSizeSnapPoints";
        static readonly string EDITOR_ENABLE_OBJECT_PIVOT = "EditorEnableObjectPivot";
        static readonly string EDITOR_SHOW_SNAP_POINTS = "EditorShowSnapPoints";
        static readonly string EDITOR_DISPLAY_VOXEL_BOUNDING = "EditorDisplayVoxelBounding";
        static readonly string EDITOR_SHOW_GENERATORS_RANGE = "EditorShowGeneratorsRange";
        static readonly string EDITOR_SHOW_LARGE_WEAPONS_RANGE = "EditorShowLargeWeaponsRange";
        static readonly string EDITOR_SHOW_DEACTIVATED_ENTITIES = "EditorShowDeactivatedEntities";
        static readonly string EDITOR_ENABLE_TEXTS_DRAWING = "EditorEnableTextsDrawing";

        static readonly string EDITOR_ENABLE_GRID = "EditorEnableGrid";

        static MyConfig()
        {
        }

        public static bool EditorShowDeactivatedEntities
        {
            get
            {
                return MyMwcUtils.GetBoolFromString(GetParameterValue(EDITOR_SHOW_DEACTIVATED_ENTITIES), true);
            }

            set
            {
                SetParameterValue(EDITOR_SHOW_DEACTIVATED_ENTITIES, value);
            }
        }
        public static bool EditorShowLargeWeaponsRange
        {
            get
            {
                return MyMwcUtils.GetBoolFromString(GetParameterValue(EDITOR_SHOW_LARGE_WEAPONS_RANGE), true);
            }

            set
            {
                SetParameterValue(EDITOR_SHOW_LARGE_WEAPONS_RANGE, value);
            }
        }
        public static bool EditorShowGeneratorsRange 
        {
            get
            {
                return MyMwcUtils.GetBoolFromString(GetParameterValue(EDITOR_SHOW_GENERATORS_RANGE), true);
            }

            set
            {
                SetParameterValue(EDITOR_SHOW_GENERATORS_RANGE, value);
            }
        }
        public static bool EditorShowSnapPoints
        {
            get
            {
                return MyMwcUtils.GetBoolFromString(GetParameterValue(EDITOR_SHOW_SNAP_POINTS), true);
            }

            set
            {
                SetParameterValue(EDITOR_SHOW_SNAP_POINTS, value);
            }
        }
        public static bool EditorEnableTextsDrawing
        {
            get
            {
                return MyMwcUtils.GetBoolFromString(GetParameterValue(EDITOR_ENABLE_TEXTS_DRAWING), true);
            }

            set
            {
                SetParameterValue(EDITOR_ENABLE_TEXTS_DRAWING, value);
            }
        }
        public static bool EditorShowSafeAreas
        {
            get
            {
                return MyMwcUtils.GetBoolFromString(GetParameterValue(EDITOR_SHOW_SAFE_AREAS), true);
            }

            set
            {
                SetParameterValue(EDITOR_SHOW_SAFE_AREAS, value);
            }
        }
        public static bool EditorSelectableSafeAreas
        {
            get
            {
                return MyMwcUtils.GetBoolFromString(GetParameterValue(EDITOR_SELECTABLE_SAFE_AREAS), true);
            }

            set
            {
                SetParameterValue(EDITOR_SELECTABLE_SAFE_AREAS, value);
            }
        }

        public static bool EditorShowDetectors
        {
            get
            {
                return MyMwcUtils.GetBoolFromString(GetParameterValue(EDITOR_SHOW_DETECTORS), true);
            }

            set
            {
                SetParameterValue(EDITOR_SHOW_DETECTORS, value);
            }
        }
        public static bool EditorSelectableDetectors
        {
            get
            {
                return MyMwcUtils.GetBoolFromString(GetParameterValue(EDITOR_SELECTABLE_DETECTORS), true);
            }

            set
            {
                SetParameterValue(EDITOR_SELECTABLE_DETECTORS, value);
            }
        }
        public static bool EditorShowParticleEffects
        {
            get
            {
                return MyMwcUtils.GetBoolFromString(GetParameterValue(EDITOR_SHOW_PARTICLE_EFFECTS), true);
            }

            set
            {
                SetParameterValue(EDITOR_SHOW_PARTICLE_EFFECTS, value);
            }
        }
        public static bool EditorSelectableParticleEffects
        {
            get
            {
                return MyMwcUtils.GetBoolFromString(GetParameterValue(EDITOR_SELECTABLE_PARTICLE_EFFECTS), true);
            }

            set
            {
                SetParameterValue(EDITOR_SELECTABLE_PARTICLE_EFFECTS, value);
            }
        }

        public static bool EditorEnableGrid
        {
            get
            {
                return MyMwcUtils.GetBoolFromString(GetParameterValue(EDITOR_ENABLE_GRID), true);
            }

            set
            {
                SetParameterValue(EDITOR_ENABLE_GRID, value);
            }
        }
        
        public static bool EditorEnableLightsInEditor
        {
            get
            {
                return MyMwcUtils.GetBoolFromString(GetParameterValue(EDITOR_ENABLE_LIGHTS_IN_EDITOR), true);
            }

            set
            {
                SetParameterValue(EDITOR_ENABLE_LIGHTS_IN_EDITOR, value);
            }
        }
        public static bool EditorDisplayPrefabContainerBounding
        {
            get
            {
                return MyMwcUtils.GetBoolFromString(GetParameterValue(EDITOR_DISPLAY_PREFAB_CONTAINER_BOUNDING), true);
            }

            set
            {
                SetParameterValue(EDITOR_DISPLAY_PREFAB_CONTAINER_BOUNDING, value);
            }
        }
        public static bool EditorDisplayPrefabContainerAxis
        {
            get
            {
                return MyMwcUtils.GetBoolFromString(GetParameterValue(EDITOR_DISPLAY_PREFAB_CONTAINER_AXIS), true);
            }

            set
            {
                SetParameterValue(EDITOR_DISPLAY_PREFAB_CONTAINER_AXIS, value);
            }
        }        
        public static bool EditorDisplayVoxelBounding
        {
            get
            {
                return MyMwcUtils.GetBoolFromString(GetParameterValue(EDITOR_DISPLAY_VOXEL_BOUNDING), true);
            }

            set
            {
                SetParameterValue(EDITOR_DISPLAY_VOXEL_BOUNDING, value);
            }
        }
        public static bool EditorSnapPointFilter
        {
            get
            {
                return MyMwcUtils.GetBoolFromString(GetParameterValue(EDITOR_EDITOR_SNAP_POINT_FILTER), true);
            }

            set
            {
                SetParameterValue(EDITOR_EDITOR_SNAP_POINT_FILTER, value);
            }
        }
        public static bool EditorFixedSizeSnapPoints
        {
            get
            {
                return MyMwcUtils.GetBoolFromString(GetParameterValue(EDITOR_FIXED_SIZE_SNAP_POINTS), true);
            }

            set
            {
                SetParameterValue(EDITOR_FIXED_SIZE_SNAP_POINTS, value);
            }
        }
        public static bool EditorEnableObjectPivot
        {
            get
            {
                return MyMwcUtils.GetBoolFromString(GetParameterValue(EDITOR_ENABLE_OBJECT_PIVOT), true);
            }

            set
            {
                SetParameterValue(EDITOR_ENABLE_OBJECT_PIVOT, value);
            }
        }

        public static MyMwcVector3Int LastSandboxSector
        {
            get
            {
                return GetParameterValueMyMwcVector3Int(LAST_MY_SANDBOX_SECTOR);
            }
            set
            {
                LastFriendSectorUserId = null;
                SetParameterValue(LAST_MY_SANDBOX_SECTOR, value);
            }
        }

        public static string LastFriendName
        {
            get
            {
                return GetParameterValue(LAST_FRIEND_NAME);
            }
            set
            {
                SetParameterValue(LAST_FRIEND_NAME, value);
            }
        }

        public static MyMwcVector3Int LastFriendSectorPosition
        {
            get
            {
                return GetParameterValueMyMwcVector3Int(LAST_FRIEND_SECTOR_POSITION);
            }
            set
            {
                SetParameterValue(LAST_FRIEND_SECTOR_POSITION, value);
            }
        }

        public static int? LastFriendSectorUserId
        {
            get
            {
                int result;
                if(int.TryParse(GetParameterValue(LAST_FRIEND_SECTOR_USER_ID), out result))
                {
                    return result;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                SetParameterValue(LAST_FRIEND_SECTOR_USER_ID, value);
            }
        }

        public static bool NeedShowHelpScreen
        {
            get
            {
                return MyMwcUtils.GetBoolFromString(GetParameterValue(NEED_SHOW_HELPSCREEN), true);
            }

            set
            {
                SetParameterValue(NEED_SHOW_HELPSCREEN, value);
            }
        }

        /*
        public static bool NeedShowPerfWarning
        {
            get
            {
                return MyMwcUtils.GetBoolFromString(GetParameterValue(NEED_SHOW_PERFWARNING), true);
            }

            set
            {
                SetParameterValue(NEED_SHOW_PERFWARNING, value);
            }
        } */

        public static string Username
        {
            get
            {
                return GetParameterValue(USERNAME);
            }

            set
            {
                SetParameterValue(USERNAME, value);
            }
        }

        //  This property accepts password in a decrypted form, but then stores it in memory and file in an encrypted form
        //  It also returns password in a decrypted form.
        public static string Password
        {
            get
            {
                return MyMwcEncryptionSymmetricRijndael.DecryptString(GetParameterValue(PASSWORD), MyConfigConstants.SYMMETRIC_PASSWORD);
            }

            set
            {
                SetParameterValue(PASSWORD, MyMwcEncryptionSymmetricRijndael.EncryptString(value, MyConfigConstants.SYMMETRIC_PASSWORD));
            }
        }

        public static bool LastLoginWasSuccessful
        {
            get
            {
                return MyMwcUtils.GetBoolFromString(GetParameterValue(LAST_LOGIN_WAS_SUCCESSFUL), false);
            }

            set
            {
                SetParameterValue(LAST_LOGIN_WAS_SUCCESSFUL, value);
            }
        }

        public static bool RememberUsernameAndPassword
        {
            get
            {
                return MyMwcUtils.GetBoolFromString(GetParameterValue(REMEBER_USERNAME_AND_PASSWORD), true);
            }

            set
            {
                SetParameterValue(REMEBER_USERNAME_AND_PASSWORD, value);
            }
        }

        public static bool Autologin
        {
            get
            {
                return MyMwcUtils.GetBoolFromString(GetParameterValue(AUTOLOGIN), true);
            }

            set
            {
                SetParameterValue(AUTOLOGIN, value);
            }
        }

        public static MyRenderQualityEnum RenderQuality
        {
            get
            {
                int? retInt = MyMwcUtils.GetIntFromString(GetParameterValue(RENDER_QUALITY));
                if ((retInt.HasValue == false) || (Enum.IsDefined(typeof(MyRenderQualityEnum), retInt) == false))
                {
                    return MyRenderQualityEnum.HIGH;
                }
                else
                {
                    return (MyRenderQualityEnum)retInt.Value;
                }
            }

            set
            {
                SetParameterValue(RENDER_QUALITY, (int)value);
            }
        }

        public static float FieldOfView
        {
            get
            {
                float? ret = MyMwcUtils.GetFloatFromString(GetParameterValue(FIELD_OF_VIEW2));
                if (ret.HasValue)
                {
                    // Loading value - load degrees and convert to radians
                    return MinerWarsMath.MathHelper.ToRadians(ret.Value);
                }
                else
                {
                    // In radians
                    return MyConstants.FIELD_OF_VIEW_CONFIG_DEFAULT;
                }
            }
            set
            {
                // Saving value - save as degrees
                SetParameterValue(FIELD_OF_VIEW2, MinerWarsMath.MathHelper.ToDegrees(value));
            }
        }

        //  Reads height and width from config file, and finds corresponding "video mode object". If not possible, then uses fall-back object/resolution.
        public static MyVideoModeEx VideoMode
        {
            get
            {
                int? width = MyMwcUtils.GetInt32FromString(GetParameterValue(SCREEN_WIDTH));
                int? height = MyMwcUtils.GetInt32FromString(GetParameterValue(SCREEN_HEIGHT));

                MyVideoModeEx ret = null;

                //  If not specified in config, we must find recommended aspect ratio. It will be aspect ration that is closest to actual Windows desktop aspect ratio.
                if ((width == null) || (height == null))
                {
                    MyMwcLog.WriteLine("Resolution not found in config");

                    ret = MyVideoModeManager.GetDefaultVideoModeForEmptyConfig(MyMinerGame.GraphicsDeviceManager.GraphicsAdapter.AdapterOrdinal);
                    if (ret != null)
                    {
                        MyMwcLog.WriteLine("Screen Width and/or Height not found in config, therefore using primary Screen Width and Height based on windows desktop: " + ret.Width + " x " + ret.Height);
                    }
                    else
                    {
                        ret = MyVideoModeManager.GetDefaultVideoModeForEmptyConfigWithClosestAspectRatio(MyMinerGame.GraphicsDeviceManager.GraphicsAdapter.AdapterOrdinal, MyVideoModeManager.GetRecommendedAspectRatio(MyMinerGame.GraphicsDeviceManager.GraphicsAdapter.AdapterOrdinal).AspectRatioNumber);
                        if (ret != null)
                        {
                            MyMwcLog.WriteLine("Screen Width and/or Height not found in config, therefore using recommended Screen Width and Height based on windows desktop: " + ret.Width + " x " + ret.Height);
                        }
                        else
                        {
                            MyMwcLog.WriteLine("Screen Width and/or Height not found in config, cannot determine widows desktop resolution, using 800x600");
                        }
                    }
                }
                else
                {
                    ret = MyVideoModeManager.GetVideoModeByWidthAndHeight(MyMinerGame.GraphicsDeviceManager.GraphicsAdapter.AdapterOrdinal, width.Value, height.Value);
                }

                if ((ret == null) || (MyVideoModeManager.IsSupportedDisplayMode(ret.Width, ret.Height, FullScreen) == false))
                {
                    ret = MyVideoModeManager.DEFAULT_FALL_BACK_4_3_800_600;
                }

                return ret;
            }

            set
            {
                SetParameterValue(SCREEN_WIDTH, value.Width);
                SetParameterValue(SCREEN_HEIGHT, value.Height);
            }
        }

        public static int VideoAdapter
        {
            get
            {
                return MyMwcUtils.GetIntFromString(GetParameterValue(VIDEO_ADAPTER), 0);
            }

            set
            {
                SetParameterValue(VIDEO_ADAPTER, value);
            }
        }

        public static bool FullScreen
        {
            get
            {
                return MyMwcUtils.GetBoolFromString(GetParameterValue(FULL_SCREEN), true);
            }

            set
            {
                SetParameterValue(FULL_SCREEN, value);
            }
        }

        public static bool VerticalSync
        {
            get
            {
                return MyMwcUtils.GetBoolFromString(GetParameterValue(VERTICAL_SYNC), false);
            }

            set
            {
                SetParameterValue(VERTICAL_SYNC, value);
            }
        }

        public static bool HardwareCursor
        {
            get
            {
                // Hardware cursor is always disabled for OnLive
                return MyMwcFinalBuildConstants.IS_CLOUD_GAMING ? false : MyMwcUtils.GetBoolFromString(GetParameterValue(HARDWARE_CURSOR), false);
            }

            set
            {
                SetParameterValue(HARDWARE_CURSOR, value);
            }
        }

        public static float GameVolume
        {
            get
            {
                return MyMwcUtils.GetFloatFromString(GetParameterValue(GAME_VOLUME), MyAudioConstants.GAME_MASTER_VOLUME_MAX);
            }

            set
            {
                SetParameterValue(GAME_VOLUME, value);
            }
        }

        public static float MusicVolume
        {
            get
            {
                return MyMwcUtils.GetFloatFromString(GetParameterValue(MUSIC_VOLUME), MyAudioConstants.MUSIC_MASTER_VOLUME_MAX);
            }
            set
            {
                SetParameterValue(MUSIC_VOLUME, value);
            }
        }

        public static bool Subtitles
        {
            get
            {
                return MyMwcUtils.GetBoolFromString(GetParameterValue(SUBTITLES), true);
            }
            set
            {
                SetParameterValue(SUBTITLES, value);
            }
        }

        public static bool Notifications
        {
            get
            {
                return MyMwcUtils.GetBoolFromString(GetParameterValue(NOTIFICATIONS), true);
            }
            set
            {
                SetParameterValue(NOTIFICATIONS, value);
            }
        }

        public static bool EditorDisplayUnselectedBounding
        {
            get
            {
                return MyMwcUtils.GetBoolFromString(GetParameterValue(DISPLAY_UNSELECTED_BOUNDING), false);
            }

            set
            {
                SetParameterValue(DISPLAY_UNSELECTED_BOUNDING, value);
            }
        }

        public static bool EditorUseCameraCrosshair
        {
            get
            {
                return MyMwcUtils.GetBoolFromString(GetParameterValue(USE_CAMERA_CROSSHAIR), false);
            }

            set
            {
                SetParameterValue(USE_CAMERA_CROSSHAIR, value);
            }
        }

        public static bool EditorLockedPrefab90DegreesRotation
        {
            get
            {
                return MyMwcUtils.GetBoolFromString(GetParameterValue(LOCKED_PREFAB_90_DEGREES_ROTATION), false);
            }

            set
            {
                SetParameterValue(LOCKED_PREFAB_90_DEGREES_ROTATION, value);
            }
        }

        public static float ScreenshotSizeMultiplier
        {
            get
            {
                if (string.IsNullOrEmpty(GetParameterValue(SCREENSHOT_SIZE_MULTIPLIER)))
                {
                    SetParameterValue(SCREENSHOT_SIZE_MULTIPLIER, 1.0f);
                    Save();
                }

                return MyMwcUtils.GetFloatFromString(GetParameterValue(SCREENSHOT_SIZE_MULTIPLIER), 1.0f);
            }

            set
            {
                SetParameterValue(SCREENSHOT_SIZE_MULTIPLIER, value);
            }
        }

        public static MyLanguagesEnum Language
        {
            get
            {
                byte? retByte = MyMwcUtils.GetByteFromString(GetParameterValue(LANGUAGE));

                if ((retByte.HasValue == false) || (Enum.IsDefined(typeof(MyLanguagesEnum), retByte) == false))
                {
                    return MyLanguagesEnum.English;
                }
                else
                {
                    return (MyLanguagesEnum)retByte.Value;
                }
            }

            set
            {
                SetParameterValue(LANGUAGE, (byte)value);
            }
        }

        public static string ControlsGeneral
        {
            get
            {
                return GetParameterValue(CONTROLS_GENERAL);
            }

            set
            {
                SetParameterValue(CONTROLS_GENERAL, value);
            }
        }

        public static string ControlsButtons
        {
            get
            {
                return GetParameterValue(CONTROLS_BUTTONS);
            }

            set
            {
                SetParameterValue(CONTROLS_BUTTONS, value);
            }
        }

        public static string EditorControlsButtons
        {
            get
            {
                return GetParameterValue(EDITOR_CONTROLS_BUTTONS);
            }

            set
            {
                SetParameterValue(EDITOR_CONTROLS_BUTTONS, value);
            }
        }



        public static bool EditorHiddenWayPoint
        {
            get
            {
                return MyMwcUtils.GetBoolFromString(GetParameterValue(EDITOR_HIDDEN_WAYPOINT), false);
            }

            set
            {
                SetParameterValue(EDITOR_HIDDEN_WAYPOINT, value);
            }
        }
        public static bool EditorSelectableWayPoint
        {
            get
            {
                return MyMwcUtils.GetBoolFromString(GetParameterValue(EDITOR_SELECTABLE_WAYPOINT), true);
            }

            set
            {
                SetParameterValue(EDITOR_SELECTABLE_WAYPOINT, value);
            }
        }
        public static bool EditorHiddenVoxelMap
        {
            get
            {
                return MyMwcUtils.GetBoolFromString(GetParameterValue(EDITOR_HIDDEN_VOXELMAP), false);
            }

            set
            {
                SetParameterValue(EDITOR_HIDDEN_VOXELMAP, value);
            }
        }
        public static bool EditorSelectableVoxelMap
        {
            get
            {
                return MyMwcUtils.GetBoolFromString(GetParameterValue(EDITOR_SELECTABLE_VOXELMAP), true);
            }

            set
            {
                SetParameterValue(EDITOR_SELECTABLE_VOXELMAP, value);
            }
        }
        public static bool EditorHiddenDummyPoint
        {
            get
            {
                return MyMwcUtils.GetBoolFromString(GetParameterValue(EDITOR_HIDDEN_DUMMYPOINT), false);
            }

            set
            {
                SetParameterValue(EDITOR_HIDDEN_DUMMYPOINT, value);
            }
        }
        public static bool EditorSelectableDummyPoint        
        {
            get
            {
                return MyMwcUtils.GetBoolFromString(GetParameterValue(EDITOR_SELECTABLE_DUMMYPOINT), true);
            }

            set
            {
                SetParameterValue(EDITOR_SELECTABLE_DUMMYPOINT, value);
            }
        }
        public static bool EditorHiddenPrefabBase
        {
            get
            {
                return MyMwcUtils.GetBoolFromString(GetParameterValue(EDITOR_HIDDEN_PREFABBASE), false);
            }

            set
            {
                SetParameterValue(EDITOR_HIDDEN_PREFABBASE, value);
            }
        }

        public static bool EditorSelectablePrefabBase
        {
            get
            {
                return MyMwcUtils.GetBoolFromString(GetParameterValue(EDITOR_SELECTABLE_PREFABBASE), true);
            }

            set
            {
                SetParameterValue(EDITOR_SELECTABLE_PREFABBASE, value);
            }
        }

        public static bool EditorHiddenSpawnPoint
        {
            get
            {
                return MyMwcUtils.GetBoolFromString(GetParameterValue(EDITOR_HIDDEN_SPAWNPOINT), false);
            }

            set
            {
                SetParameterValue(EDITOR_HIDDEN_SPAWNPOINT, value);
            }
        }

        public static bool EditorSelectableSpawnPoint
        {
            get
            {
                return MyMwcUtils.GetBoolFromString(GetParameterValue(EDITOR_SELECTABLE_SPAWNPOINT), true);
            }

            set
            {
                SetParameterValue(EDITOR_SELECTABLE_SPAWNPOINT, value);
            }
        }

        public static bool EditorHiddenInfluenceSphere
        {
            get
            {
                return MyMwcUtils.GetBoolFromString(GetParameterValue(EDITOR_HIDDEN_INFLUENCESPHERE), false);
            }

            set
            {
                SetParameterValue(EDITOR_HIDDEN_INFLUENCESPHERE, value);
            }
        }

        public static bool EditorSelectableInfluenceSphere
        {
            get
            {
                return MyMwcUtils.GetBoolFromString(GetParameterValue(EDITOR_SELECTABLE_INFLUENCESPHERE), true);
            }

            set
            {
                SetParameterValue(EDITOR_SELECTABLE_INFLUENCESPHERE, value);
            }
        }
        public static bool EditorWaypointsIgnoreDepth
        {
            get
            {
                return MyMwcUtils.GetBoolFromString(GetParameterValue(EDITOR_WAYPOINTS_IGNORE_DEPTH), true);
            }

            set
            {
                SetParameterValue(EDITOR_WAYPOINTS_IGNORE_DEPTH, value);
            }
        }

        //  Return parameter value from memory. If not found, empty string is returned.
        static string GetParameterValue(string parameterName)
        {
            string outValue;
            if (m_values.TryGetValue(parameterName, out outValue) == false)
            {
                outValue = "";
            }
            return outValue;
        }

        static MyMwcVector3Int GetParameterValueMyMwcVector3Int(string parameterName)
        {
            var parts = GetParameterValue(parameterName).Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            int x, y, z;
            if (parts.Length == 3 && int.TryParse(parts[0], out x) && int.TryParse(parts[1], out y) && int.TryParse(parts[2], out z))
            {
                return new MyMwcVector3Int(x, y, z);
            }
            else
            {
                return new MyMwcVector3Int(0, 0, 0);
            }
        }

        //  Change parameter's value in memory. It doesn't matter if this parameter was loaded. If was, it will be overwritten. If wasn't loaded, we will just set it.
        static void SetParameterValue(string parameterName, string value)
        {
            m_values[parameterName] = value;
        }

        //  Change parameter's value in memory. It doesn't matter if this parameter was loaded. If was, it will be overwritten. If wasn't loaded, we will just set it.
        static void SetParameterValue(string parameterName, float value)
        {
            m_values[parameterName] = value.ToString(CultureInfo.InvariantCulture.NumberFormat);
        }

        //  Change parameter's value in memory. It doesn't matter if this parameter was loaded. If was, it will be overwritten. If wasn't loaded, we will just set it.
        static void SetParameterValue(string parameterName, bool value)
        {
            m_values[parameterName] = value.ToString(CultureInfo.InvariantCulture.NumberFormat);
        }

        //  Change parameter's value in memory. It doesn't matter if this parameter was loaded. If was, it will be overwritten. If wasn't loaded, we will just set it.
        static void SetParameterValue(string parameterName, int value)
        {
            m_values[parameterName] = value.ToString(CultureInfo.InvariantCulture.NumberFormat);
        }

        //  Change parameter's value in memory. It doesn't matter if this parameter was loaded. If was, it will be overwritten. If wasn't loaded, we will just set it.
        static void SetParameterValue(string parameterName, int? value)
        {
            m_values[parameterName] = value == null ? "" : value.Value.ToString(CultureInfo.InvariantCulture.NumberFormat);
        }

        static void SetParameterValue(string parameterName, MyMwcVector3Int value)
        {
            SetParameterValue(parameterName, String.Format("{0}, {1}, {2}", value.X, value.Y, value.Z));
        }

        //  Save all values from config file
        public static void Save()
        {
            MyMwcLog.WriteLine("MyConfig.Save() - START");
            MyMwcLog.IncreaseIndent();

            string path = GetFilePath();
            MyMwcLog.WriteLine("Path: " + path, LoggingOptions.CONFIG_ACCESS);

            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Create))
                {
                    XmlWriterSettings settings = new XmlWriterSettings()
                    {
                        Indent = true,
                        NewLineHandling = NewLineHandling.Replace
                    };

                    using (XmlWriter xmlWriter = XmlWriter.Create(fs, settings))
                    {
                        xmlWriter.WriteStartDocument();
                        xmlWriter.WriteStartElement("Parameters");

                        foreach (KeyValuePair<string, string> kvp in m_values)
                        {
                            xmlWriter.WriteStartElement(kvp.Key);
                            xmlWriter.WriteAttributeString("Value", kvp.Value);
                            xmlWriter.WriteEndElement();

                            MyMwcLog.WriteLine(kvp.Key + ": " + kvp.Value, LoggingOptions.CONFIG_ACCESS);
                        }

                        xmlWriter.WriteEndDocument();
                        xmlWriter.Flush();
                    }
                }
            }
            catch (Exception exc)
            {
                //  Write exception to log, but continue as if nothing wrong happened
                MyMwcLog.WriteLine("Exception occured, but application is continuing. Exception: " + exc);
            }

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyConfig.Save() - END");
        }

        //  Loads all values from config file
        //  Result of this method is "m_values" filled with individual values.
        //  If anything fails during loading, we ignore log it, but continue - because damaged cfg file must not stop this game
        public static void Load()
        {
            MyMwcLog.WriteLine("MyConfig.Load() - START");
            MyMwcLog.IncreaseIndent();

            string path = GetFilePath();
            MyMwcLog.WriteLine("Path: " + path, LoggingOptions.CONFIG_ACCESS);

            //  If anything fails during loading, we ignore log it, but continue - because damaged cfg file must not stop this game
            string xmlTextOriginal = "";
            try
            {
                if (MyFileSystemUtils.FileExists(path) == false)
                {
                    MyMwcLog.WriteLine("Config file not found! " + path);
                }
                else
                {
                    xmlTextOriginal = File.ReadAllText(path);
                    string xmlText = xmlTextOriginal.Replace('\0', ' '); // Sometimes there's null chars in config

                    using (var textReader = new StringReader(xmlText))
                    {
                        using (XmlReader xmlReader = XmlReader.Create(textReader))
                        {
                            while (xmlReader.Read())
                            {
                                if (xmlReader.NodeType == XmlNodeType.Element)
                                {
                                    if (xmlReader.HasAttributes)
                                    {
                                        for (int i = 0; i < xmlReader.AttributeCount; i++)
                                        {
                                            m_values[xmlReader.Name] = xmlReader.GetAttribute(i);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                //  Write exception to log, but continue as if nothing wrong happened
                MyMwcLog.WriteLine("Exception occured, but application is continuing. Exception: " + exc);
                MyMwcLog.WriteLine("Config:");
                MyMwcLog.WriteLine(xmlTextOriginal);
            }

            foreach (KeyValuePair<string, string> kvp in m_values)
            {
                MyMwcLog.WriteLine(kvp.Key + ": " + kvp.Value, LoggingOptions.CONFIG_ACCESS);
            }

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyConfig.Load() - END");
        }

        static string GetFilePath()
        {
            string postFix = MyMwcFinalBuildConstants.GetValueForBuildType("", "_TEST_BUILD", "_DEVELOP_BUILD");
            return Path.Combine(MyFileSystemUtils.GetApplicationUserDataFolder(), "MinerWars" + postFix + ".cfg");
        }

        
    }
}