using System.Collections.Generic;
using System.Text;
using MinerWarsMath;
using MinerWarsMath.Graphics;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.Utils;
using SysUtils.Utils;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.Sessions.Multiplayer;
using MinerWars.AppCode.Toolkit.Input;

namespace MinerWars.AppCode.Game.GUI
{
    class MyGuiScreenHelp : MyGuiScreenBase
    {
        struct ControlWithDescription
        {
            public StringBuilder Control;
            public StringBuilder Description;

            public ControlWithDescription(StringBuilder control, StringBuilder description)
            {
                Control = control;
                Description = description;
            }
            public ControlWithDescription(string control, string description) : this(new StringBuilder(control), new StringBuilder(description)) { }
            public ControlWithDescription(StringBuilder controlString, MyGameControlEnums control) : this(controlString, MyTextsWrapper.Get(MyGuiManager.GetInput().GetGameControl(control).GetControlName())) { }
            public ControlWithDescription(MyGameControlEnums control, StringBuilder descriptionString) : this(new StringBuilder(MyGuiManager.GetInput().GetGameControlTextEnum(control)), descriptionString) { }
            public ControlWithDescription(MyGameControlEnums control)
            {
                MyControl c = MyGuiManager.GetInput().GetGameControl(control);
                Control = c.GetControlButtonStringBuilder(MyGuiInputDeviceEnum.Keyboard);
                Description = MyTextsWrapper.Get(c.GetControlName());
            }
            public ControlWithDescription(MyGameControlEnums control1, MyGameControlEnums control2)
            {
                MyControl c1 = MyGuiManager.GetInput().GetGameControl(control1);
                MyControl c2 = MyGuiManager.GetInput().GetGameControl(control2);
                Control = new StringBuilder().Append(c1.GetControlButtonStringBuilder(MyGuiInputDeviceEnum.Keyboard)).Append(", ").Append(c2.GetControlButtonStringBuilder(MyGuiInputDeviceEnum.Keyboard));
                Description = new StringBuilder().Append(MyTextsWrapper.Get(c1.GetControlName())).Append(" / ").Append(MyTextsWrapper.Get(c2.GetControlName()));
            }
        }
        
        static MyGameControlEnums[] specFront = new MyGameControlEnums[]
        {
            MyGameControlEnums.FIRE_HOLOGRAM_FRONT,
            MyGameControlEnums.FIRE_BASIC_MINE_FRONT,
            MyGameControlEnums.FIRE_SMART_MINE_FRONT,
            MyGameControlEnums.FIRE_FLASH_BOMB_FRONT,
            MyGameControlEnums.FIRE_DECOY_FLARE_FRONT,
            MyGameControlEnums.FIRE_SMOKE_BOMB_FRONT
        };
        static MyGameControlEnums[] specBack = new MyGameControlEnums[]
        {
            MyGameControlEnums.FIRE_HOLOGRAM_BACK,
            MyGameControlEnums.FIRE_BASIC_MINE_BACK,
            MyGameControlEnums.FIRE_SMART_MINE_BACK,
            MyGameControlEnums.FIRE_FLASH_BOMB_BACK,
            MyGameControlEnums.FIRE_DECOY_FLARE_BACK,
            MyGameControlEnums.FIRE_SMOKE_BOMB_BACK
        };
        static MyTextsWrapperEnum[] specTexts = new MyTextsWrapperEnum[]
        {
            MyTextsWrapperEnum.FireHologram,
            MyTextsWrapperEnum.FireBasicMine,
            MyTextsWrapperEnum.FireSmartMine,
            MyTextsWrapperEnum.FireFlashBomb,
            MyTextsWrapperEnum.FireDecoyFlare,
            MyTextsWrapperEnum.FireSmokeBomb
        };

        bool m_wasPause = false;

        public MyGuiScreenHelp()
            : base(new Vector2(0.5f, 0.5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, new Vector2(1f, 0.98f))
        {
            m_enableBackgroundFade = true;

            AddCaption(MyTextsWrapperEnum.HelpCaption, captionScale:1.35f);

            if (MyGuiScreenGamePlay.Static != null)
                MyGuiScreenGamePlay.Static.DrawHud = false;

            List<ControlWithDescription> left = new List<ControlWithDescription>();
            List<ControlWithDescription> right = new List<ControlWithDescription>();

//            left.Add(new ControlWithDescription(MyGameControlEnums.FORWARD, MyGameControlEnums.REVERSE));
//            left.Add(new ControlWithDescription(MyGameControlEnums.STRAFE_LEFT, MyGameControlEnums.STRAFE_RIGHT));
//            left.Add(new ControlWithDescription(MyGameControlEnums.UP_THRUST, MyGameControlEnums.DOWN_THRUST));
//            left.Add(new ControlWithDescription(MyGameControlEnums.ROLL_LEFT, MyGameControlEnums.ROLL_RIGHT));
            left.Add(new ControlWithDescription(MyGameControlEnums.FORWARD));
            left.Add(new ControlWithDescription(MyGameControlEnums.REVERSE));
            left.Add(new ControlWithDescription(MyGameControlEnums.STRAFE_LEFT));
            left.Add(new ControlWithDescription(MyGameControlEnums.STRAFE_RIGHT));
            left.Add(new ControlWithDescription(MyGameControlEnums.UP_THRUST));
            left.Add(new ControlWithDescription(MyGameControlEnums.DOWN_THRUST));
            left.Add(new ControlWithDescription(MyGameControlEnums.ROLL_LEFT));
            left.Add(new ControlWithDescription(MyGameControlEnums.ROLL_RIGHT));
            left.Add(new ControlWithDescription(MyGameControlEnums.AFTERBURNER));
            left.Add(new ControlWithDescription(MyGameControlEnums.MOVEMENT_SLOWDOWN));
            left.Add(new ControlWithDescription(MyGameControlEnums.AUTO_LEVEL));
            
            left.Add(new ControlWithDescription("",""));
            
            left.Add(new ControlWithDescription(
                new StringBuilder().
                    Append(MyGuiManager.GetInput().GetGameControlTextEnum(MyGameControlEnums.FIRE_PRIMARY)).
                    Append(", ").
                    Append(MyGuiManager.GetInput().GetGameControl(MyGameControlEnums.FIRE_PRIMARY).GetControlButtonName(MyGuiInputDeviceEnum.Mouse)),
                MyGameControlEnums.FIRE_PRIMARY
            ));
            left.Add(new ControlWithDescription(
                new StringBuilder().
                    Append(MyGuiManager.GetInput().GetGameControlTextEnum(MyGameControlEnums.FIRE_SECONDARY)).
                    Append(", ").
                    Append(MyGuiManager.GetInput().GetGameControl(MyGameControlEnums.FIRE_SECONDARY).GetControlButtonName(MyGuiInputDeviceEnum.Mouse)),
                MyGameControlEnums.FIRE_SECONDARY
            ));
            left.Add(new ControlWithDescription(
                new StringBuilder().
                    Append(MyGuiManager.GetInput().GetGameControlTextEnum(MyGameControlEnums.SELECT_AMMO_BULLET)).
                    Append(",").
                    Append(MyGuiManager.GetInput().GetGameControlTextEnum(MyGameControlEnums.SELECT_AMMO_MISSILE)).
                    Append(",").
                    Append(MyGuiManager.GetInput().GetGameControlTextEnum(MyGameControlEnums.SELECT_AMMO_CANNON)).
                    Append(",").
                    Append(MyGuiManager.GetInput().GetGameControlTextEnum(MyGameControlEnums.SELECT_AMMO_UNIVERSAL_LAUNCHER_FRONT)).
                    Append(",").
                    Append(MyGuiManager.GetInput().GetGameControlTextEnum(MyGameControlEnums.SELECT_AMMO_UNIVERSAL_LAUNCHER_BACK)),
                MyTextsWrapper.Get(MyTextsWrapperEnum.SelectAmmo)
            ));
            left.Add(new ControlWithDescription(MyGameControlEnums.WEAPON_SPECIAL));
            left.Add(new ControlWithDescription(MyGameControlEnums.PREV_TARGET));
            left.Add(new ControlWithDescription(MyGameControlEnums.NEXT_TARGET));
            
            for (int i = 0; i < specTexts.Length; i++)
            {
                StringBuilder specControl = new StringBuilder();
                if (MyGuiManager.GetInput().GetGameControl(specFront[i]).IsControlAssigned(MyGuiInputDeviceEnum.Keyboard))
                {
                    specControl.Append(MyGuiManager.GetInput().GetGameControlTextEnum(specFront[i]));
                }
                if (MyGuiManager.GetInput().GetGameControl(specBack[i]).IsControlAssigned(MyGuiInputDeviceEnum.Keyboard))
                {
                    if (specControl.Length != 0) specControl.Append(", ");
                    specControl.Append(MyGuiManager.GetInput().GetGameControlTextEnum(specBack[i]));
                }
                left.Add(new ControlWithDescription(specControl, MyTextsWrapper.Get(specTexts[i])));
            }

            left.Add(new ControlWithDescription("", ""));

            left.Add(new ControlWithDescription(MyGameControlEnums.USE));
            left.Add(new ControlWithDescription(MyGameControlEnums.GPS));
            //Journal removed
            //left.Add(new ControlWithDescription(MyGameControlEnums.MISSION_DIALOG));
            left.Add(new ControlWithDescription(MyGameControlEnums.INVENTORY));
            left.Add(new ControlWithDescription(MyGameControlEnums.TRAVEL));

            right.Add(new ControlWithDescription(MyGameControlEnums.DRILL));
            right.Add(new ControlWithDescription(MyGameControlEnums.HARVEST));
            right.Add(new ControlWithDescription(MyGameControlEnums.DRONE_DEPLOY));
            right.Add(new ControlWithDescription(MyGameControlEnums.DRONE_CONTROL));
            right.Add(new ControlWithDescription(MyGameControlEnums.CHANGE_DRONE_MODE));
            right.Add(new ControlWithDescription(MyTextsWrapper.Get(MyTextsWrapperEnum.LeftMouseButton), MyTextsWrapper.Get(MyTextsWrapperEnum.DetonateDrone)));
            right.Add(new ControlWithDescription(
                new StringBuilder().
                    Append(MyGuiManager.GetInput().GetGameControlTextEnum(MyGameControlEnums.PREVIOUS_CAMERA)).
                    Append(", ").
                    Append(MyGuiManager.GetInput().GetGameControlTextEnum(MyGameControlEnums.NEXT_CAMERA)),
                MyTextsWrapper.Get(MyTextsWrapperEnum.CycleSecondaryCamera)
            ));
            right.Add(new ControlWithDescription(MyGameControlEnums.CONTROL_SECONDARY_CAMERA));
            
            right.Add(new ControlWithDescription("", ""));
            
            right.Add(new ControlWithDescription(MyGameControlEnums.QUICK_ZOOM));
            right.Add(new ControlWithDescription(MyGameControlEnums.ZOOM_IN, MyGameControlEnums.ZOOM_OUT));
            right.Add(new ControlWithDescription(MyGameControlEnums.REAR_CAM));
            right.Add(new ControlWithDescription(MyGameControlEnums.HEADLIGHTS));
            right.Add(new ControlWithDescription(MyGameControlEnums.HEADLIGTHS_DISTANCE));
            right.Add(new ControlWithDescription(MyGameControlEnums.VIEW_MODE));
            right.Add(new ControlWithDescription(MyGameControlEnums.WHEEL_CONTROL));
            right.Add(new ControlWithDescription(MyGameControlEnums.CHAT));
            right.Add(new ControlWithDescription(MyGameControlEnums.SCORE));

            right.Add(new ControlWithDescription("", ""));

            right.Add(new ControlWithDescription(new StringBuilder("F1"), MyTextsWrapper.Get(MyTextsWrapperEnum.OpenHelpScreen)));
            right.Add(new ControlWithDescription(
                new StringBuilder().
                    Append(MyTextsWrapper.Get(MyTextsWrapperEnum.Ctrl)).
                    Append("+F1"),
                MyTextsWrapper.Get(MyTextsWrapperEnum.OpenCheats)
            ));
            right.Add(new ControlWithDescription(new StringBuilder("F4"), MyTextsWrapper.Get(MyTextsWrapperEnum.SaveScreenshotToUserFolder)));
            
            //These keys are to be used just for developers or testing
            if (!SysUtils.MyMwcFinalBuildConstants.IS_PUBLIC)
            {
                right.Add(new ControlWithDescription("", ""));
                //Programmers
                right.Add(new ControlWithDescription("F6", "Switch to player"));
                right.Add(new ControlWithDescription("F7", "Switch to following 3rd person"));
                right.Add(new ControlWithDescription("F9", "Switch to static 3rd person"));
                right.Add(new ControlWithDescription("F8, Ctrl+F8", "Switch to / reset spectator"));
                right.Add(new ControlWithDescription("Ctrl+Space", "Move ship to spectator"));
                right.Add(new ControlWithDescription("F11, Shift+F11", "Game statistics / FPS"));
                right.Add(new ControlWithDescription("F12", "Debug screen (jump to mission)"));
                right.Add(new ControlWithDescription("Ctrl+Del", "Skip current objective"));
                right.Add(new ControlWithDescription("Shift+\\", "Teleport to next obstacle"));
            }

            /*
            right.Add(new ControlWithDescription("F5","Ship customization screen"));
            right.Add(new ControlWithDescription("F9","restart current game and reload sounds"));
            right.Add(new ControlWithDescription("F3","start sun wind"));
            right.Add(new ControlWithDescription("F8","save all voxel maps into USER FOLDER - only for programmers"));
            right.Add(new ControlWithDescription("F10","switch camera - only for programmers"));
            right.Add(new ControlWithDescription("F11","save screenshot(s) into USER FOLDER"));
            right.Add(new ControlWithDescription("F12","cycle through debug screens - only for programmers"));
            */

#if RENDER_PROFILING
            right.Add(new ControlWithDescription("Alt + Num0", "Enable/Disable render profiler or leave current child node."));
            right.Add(new ControlWithDescription("Alt + Num1-Num9", "Enter child node in render profiler"));
            right.Add(new ControlWithDescription("Alt + Enter", "Pause/Unpause profiler"));
#endif //RENDER_PROFILER

            Vector2 controlPosition = -m_size.Value / 2.0f + new Vector2(0.06f, 0.125f);
            Vector2 descriptionPosition = -m_size.Value / 2.0f + new Vector2(0.18f, 0.125f);
            foreach (var line in left)
            {
                Controls.Add(new MyGuiControlLabel(this, controlPosition, null, line.Control, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER, MyGuiManager.GetFontMinerWarsRed()));
                Controls.Add(new MyGuiControlLabel(this, descriptionPosition, null, line.Description, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
                controlPosition.Y += 0.023f;
                descriptionPosition.Y += 0.023f;
            }

            controlPosition = new Vector2(0.03f, 0.125f - m_size.Value.Y / 2.0f);
            descriptionPosition = new Vector2(0.15f, 0.125f - m_size.Value.Y / 2.0f);

            foreach (var line in right)
            {
                Controls.Add(new MyGuiControlLabel(this, controlPosition, null, line.Control, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER, MyGuiManager.GetFontMinerWarsRed()));
                Controls.Add(new MyGuiControlLabel(this, descriptionPosition, null, line.Description, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
                controlPosition.Y += 0.023f;
                descriptionPosition.Y += 0.023f;
            }

            Controls.Add(new MyGuiControlLabel(this, new Vector2(0.06f - m_size.Value.X / 2.0f, -0.085f + m_size.Value.Y / 2.0f), null, new StringBuilder().Append(MyTextsWrapper.Get(MyTextsWrapperEnum.UserFolder)).Append(": ").Append(MyFileSystemUtils.GetApplicationUserDataFolder()), MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));

            Controls.Add(new MyGuiControlButton(this, 0.5f * m_size.Value + new Vector2(-0.14f, -0.085f), MyGuiConstants.BACK_BUTTON_SIZE, 
                MyGuiConstants.BACK_BUTTON_BACKGROUND_COLOR, MyTextsWrapperEnum.Back, MyGuiConstants.BACK_BUTTON_TEXT_COLOR, 
                MyGuiConstants.BACK_BUTTON_TEXT_SCALE, OnBackClick, MyGuiControlButtonTextAlignment.CENTERED, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));

            if (MyMultiplayerPeers.Static.Players.Count == 0)
            {
                m_wasPause = MyMinerGame.IsPaused();
                MyMinerGame.SetPause(true);
            }
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenHelp";
        }

        public override void LoadContent()
        {
            base.LoadContent();

            m_backgroundColor = Vector4.Zero;

            MyConfig.NeedShowHelpScreen = false;
            MyConfig.Save();
        }

        //  Just close the screen
        public void OnBackClick(MyGuiControlButton sender)
        {
            CloseScreen();
        }

        protected override void OnClosed()
        {
            if (MyGuiScreenGamePlay.Static != null)
                MyGuiScreenGamePlay.Static.DrawHud = true;

            MyMinerGame.SetPause(m_wasPause);

            base.OnClosed();
        }

        public override void HandleInput(MyGuiInput input, bool receivedFocusInThisUpdate)
        {
            base.HandleInput(input, receivedFocusInThisUpdate);

            if (input.IsNewKeyPress(Keys.F1))
            {
                OnBackClick(null);
            }
        }
    }
}
