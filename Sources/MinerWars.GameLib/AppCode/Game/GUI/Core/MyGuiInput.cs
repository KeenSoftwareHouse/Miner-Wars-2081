using System;
using System.Collections.Generic;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.VideoMode;
using SysUtils.Utils;
using System.Text;
using SharpDX.DirectInput;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.GUI.Helpers;
using System.Runtime.InteropServices;
using MinerWars.AppCode.Toolkit.Input;

namespace MinerWars.AppCode.Game.GUI
{
    using System.Reflection;
    using System.Management;
    using KeenSoftwareHouse.Library.Trace;
    using SysUtils;
    using MinerWars.AppCode.Game.World;
    using MinerWarsMath;

    delegate void OnControlsSaved(MyGuiInput sender);

    static class JoystickExtensions
    {
        public static bool IsPressed(this JoystickState state, int button)
        {
            return state.Buttons[button];
        }

        public static bool IsReleased(this JoystickState state, int button)
        {
            return !IsPressed(state, button);
        }
    }

    class MyGuiInput:IDisposable
    {
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        // An umanaged function that retrieves the states of each key
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        public static extern short GetKeyState(int keyCode);

        public bool IsCapsLock = (((ushort)GetKeyState(0x14)) & 0xffff) != 0;
        public bool IsNumLock = (((ushort)GetKeyState(0x90)) & 0xffff) != 0;
        public bool IsScrollLock = (((ushort)GetKeyState(0x91)) & 0xffff) != 0;
 

        //  State Variables
        MyMouseState m_previousMouseState;
        JoystickState m_previousJoystickState;
        MyGuiLocalizedKeyboardState m_keyboardState;
        MyMouseState m_actualMouseState;
        JoystickState m_actualJoystickState;
        bool m_joystickXAxisSupported;
        bool m_joystickYAxisSupported;
        bool m_joystickZAxisSupported;
        bool m_joystickRotationXAxisSupported;
        bool m_joystickRotationYAxisSupported;
        bool m_joystickRotationZAxisSupported;
        bool m_joystickSlider1AxisSupported;
        bool m_joystickSlider2AxisSupported;

        // Double Click Variables
        readonly int DOUBLE_CLICK_DELAY = 500;
        readonly int DOUBLE_CLICK_MAXIMUM_DISTANCE_SQUARED = 100;
        MinerWarsMath.Vector2 m_lastLeftButtonMousePosition;
        int m_lastLeftButtonClickTime;
        bool m_leftButtonDoubleClick;

        //  Control properties
        bool m_mouseXIsInverted;
        bool m_mouseYIsInverted;
        float m_mouseSensitivity;
        private string m_joystickInstanceName;
        public string JoystickInstanceName
        {
            get { return m_joystickInstanceName; }
            set
            {
                if (m_joystickInstanceName != value)
                {
                    m_joystickInstanceName = value;
                    InitializeJoystickIfPossible();
                }
            }
        }
        float m_joystickSensitivity;
        float m_joystickDeadzone;
        float m_joystickExponent;
        public bool IsMouseXInvertedDefault { get { return false; } }
        public bool IsMouseYInvertedDefault { get { return false; } }
        public float MouseSensitivityDefault { get { return 1.0f; } }
        public string JoystickInstanceNameDefault { get { return null; } }
        public float JoystickSensitivityDefault { get { return 2.0f; } }
        public float JoystickExponentDefault { get { return 2.0f; } }
        public float JoystickDeadzoneDefault { get { return 0.2f; } }

        string m_joystickInstanceNameSnapshot;

        //  Control lists
        MyControl[] m_defaultGameControlsList = new MyControl[Enum.GetValues(typeof(MyGameControlEnums)).Length];
        MyControl[] m_defaultEditorControlsList = new MyControl[Enum.GetValues(typeof(MyEditorControlEnums)).Length];
        MyControl[] m_gameControlsList = new MyControl[Enum.GetValues(typeof(MyGameControlEnums)).Length];
        MyControl[] m_editorControlsList = new MyControl[Enum.GetValues(typeof(MyEditorControlEnums)).Length];
        MyControl[] m_controlsSnapshot = new MyControl[Enum.GetValues(typeof(MyGameControlEnums)).Length];
        MyControl[] m_gameControlsSnapshot = new MyControl[Enum.GetValues(typeof(MyGameControlEnums)).Length];
        MyControl[] m_editorControlsSnapshot = new MyControl[Enum.GetValues(typeof(MyEditorControlEnums)).Length];
        
        //  Lists of valid keys and buttons
        static List<Keys> m_validKeyboardKeys = new List<Keys>();
        static List<MyJoystickButtonsEnum> m_validJoystickButtons = new List<MyJoystickButtonsEnum>();
        static List<MyJoystickAxesEnum> m_validJoystickAxes = new List<MyJoystickAxesEnum>();
        static List<MyMouseButtonsEnum> m_validMouseButtons = new List<MyMouseButtonsEnum>();
        static List<MyGameControlEnums> m_deprecatedGameControls = new List<MyGameControlEnums>();
        //static ManagementEventWatcher m_mewWatcher;

        List<Keys> m_digitKeys = new List<Keys>();

        //  Joystick variables
        Device m_joystick = null;
        DeviceType? m_joystickType = null;
        bool m_joystickConnected = false;

        bool m_previousUpdateWasLostFocus = true;

        //Declare the hook handle as an int.
        static int m_hHook = 0;
        static MyWindowsAPIWrapper.HookProc m_hookHandler;

        public MyGuiInput()
        {
            MyWindowsMouse.SetWindow(MyMinerGame.Static.Window.NativeWindow.Handle);

            InitDevicePluginHandlerCallBack();

            m_mouseXIsInverted = IsMouseXInvertedDefault;
            m_mouseYIsInverted = IsMouseYInvertedDefault;
            m_mouseSensitivity = MouseSensitivityDefault;
            m_joystickInstanceName = JoystickInstanceNameDefault;
            m_joystickSensitivity = JoystickSensitivityDefault;
            m_joystickDeadzone = JoystickDeadzoneDefault;
            m_joystickExponent = JoystickExponentDefault;

            // Fill list of deprecated controls
            //m_deprecatedGameControls.Add(MyGameControlEnums.COMM_AVATAR_SELECT);

            #region Digit keys only
            
            m_digitKeys.Add(Keys.D0);
            m_digitKeys.Add(Keys.D1);
            m_digitKeys.Add(Keys.D2);
            m_digitKeys.Add(Keys.D3);
            m_digitKeys.Add(Keys.D4);
            m_digitKeys.Add(Keys.D5);
            m_digitKeys.Add(Keys.D6);
            m_digitKeys.Add(Keys.D7);
            m_digitKeys.Add(Keys.D8);
            m_digitKeys.Add(Keys.D9);
            m_digitKeys.Add(Keys.NumPad0);
            m_digitKeys.Add(Keys.NumPad1);
            m_digitKeys.Add(Keys.NumPad2);
            m_digitKeys.Add(Keys.NumPad3);
            m_digitKeys.Add(Keys.NumPad4);
            m_digitKeys.Add(Keys.NumPad5);
            m_digitKeys.Add(Keys.NumPad6);
            m_digitKeys.Add(Keys.NumPad7);
            m_digitKeys.Add(Keys.NumPad8);
            m_digitKeys.Add(Keys.NumPad9);

            #endregion

            #region Lists of assignable keys and buttons

            //  List of assignable keyboard keys
            m_validKeyboardKeys.Add(Keys.A);
            m_validKeyboardKeys.Add(Keys.Add);
            m_validKeyboardKeys.Add(Keys.B);
            m_validKeyboardKeys.Add(Keys.Back);
            m_validKeyboardKeys.Add(Keys.C);
            m_validKeyboardKeys.Add(Keys.CapsLock);
            m_validKeyboardKeys.Add(Keys.D);
            m_validKeyboardKeys.Add(Keys.D0);
            m_validKeyboardKeys.Add(Keys.D1);
            m_validKeyboardKeys.Add(Keys.D2);
            m_validKeyboardKeys.Add(Keys.D3);
            m_validKeyboardKeys.Add(Keys.D4);
            m_validKeyboardKeys.Add(Keys.D5);
            m_validKeyboardKeys.Add(Keys.D6);
            m_validKeyboardKeys.Add(Keys.D7);
            m_validKeyboardKeys.Add(Keys.D8);
            m_validKeyboardKeys.Add(Keys.D9);
            m_validKeyboardKeys.Add(Keys.Decimal);
            m_validKeyboardKeys.Add(Keys.Delete);
            m_validKeyboardKeys.Add(Keys.Divide);
            m_validKeyboardKeys.Add(Keys.Down);
            m_validKeyboardKeys.Add(Keys.E);
            m_validKeyboardKeys.Add(Keys.End);
            m_validKeyboardKeys.Add(Keys.Enter);
            m_validKeyboardKeys.Add(Keys.F);
            m_validKeyboardKeys.Add(Keys.G);
            m_validKeyboardKeys.Add(Keys.H);
            m_validKeyboardKeys.Add(Keys.Home);
            m_validKeyboardKeys.Add(Keys.I);
            m_validKeyboardKeys.Add(Keys.Insert);
            m_validKeyboardKeys.Add(Keys.J);
            m_validKeyboardKeys.Add(Keys.K);
            m_validKeyboardKeys.Add(Keys.L);
            m_validKeyboardKeys.Add(Keys.Left);
            m_validKeyboardKeys.Add(Keys.LeftAlt);
            m_validKeyboardKeys.Add(Keys.LeftControl);
            m_validKeyboardKeys.Add(Keys.LeftShift);
            m_validKeyboardKeys.Add(Keys.M);
            m_validKeyboardKeys.Add(Keys.Multiply);
            m_validKeyboardKeys.Add(Keys.N);
            m_validKeyboardKeys.Add(Keys.None);
            m_validKeyboardKeys.Add(Keys.NumPad0);
            m_validKeyboardKeys.Add(Keys.NumPad1);
            m_validKeyboardKeys.Add(Keys.NumPad2);
            m_validKeyboardKeys.Add(Keys.NumPad3);
            m_validKeyboardKeys.Add(Keys.NumPad4);
            m_validKeyboardKeys.Add(Keys.NumPad5);
            m_validKeyboardKeys.Add(Keys.NumPad6);
            m_validKeyboardKeys.Add(Keys.NumPad7);
            m_validKeyboardKeys.Add(Keys.NumPad8);
            m_validKeyboardKeys.Add(Keys.NumPad9);
            m_validKeyboardKeys.Add(Keys.O);
            m_validKeyboardKeys.Add(Keys.OemCloseBrackets);
            m_validKeyboardKeys.Add(Keys.OemComma);
            m_validKeyboardKeys.Add(Keys.OemMinus);
            m_validKeyboardKeys.Add(Keys.OemOpenBrackets);
            m_validKeyboardKeys.Add(Keys.OemPeriod);
            m_validKeyboardKeys.Add(Keys.OemPipe);
            m_validKeyboardKeys.Add(Keys.OemPlus);
            m_validKeyboardKeys.Add(Keys.OemQuestion);
            m_validKeyboardKeys.Add(Keys.OemQuotes);
            m_validKeyboardKeys.Add(Keys.OemSemicolon);
            m_validKeyboardKeys.Add(Keys.OemTilde);
            m_validKeyboardKeys.Add(Keys.OemBackslash);
            m_validKeyboardKeys.Add(Keys.P);
            m_validKeyboardKeys.Add(Keys.PageDown);
            m_validKeyboardKeys.Add(Keys.PageUp);
            m_validKeyboardKeys.Add(Keys.Pause);
            m_validKeyboardKeys.Add(Keys.Q);
            m_validKeyboardKeys.Add(Keys.R);
            m_validKeyboardKeys.Add(Keys.Right);
            m_validKeyboardKeys.Add(Keys.RightAlt);
            m_validKeyboardKeys.Add(Keys.RightControl);
            m_validKeyboardKeys.Add(Keys.RightShift);
            m_validKeyboardKeys.Add(Keys.S);
            m_validKeyboardKeys.Add(Keys.Space);
            m_validKeyboardKeys.Add(Keys.Subtract);
            m_validKeyboardKeys.Add(Keys.T);
            m_validKeyboardKeys.Add(Keys.Tab);
            m_validKeyboardKeys.Add(Keys.U);
            m_validKeyboardKeys.Add(Keys.Up);
            m_validKeyboardKeys.Add(Keys.V);
            m_validKeyboardKeys.Add(Keys.W);
            m_validKeyboardKeys.Add(Keys.X);
            m_validKeyboardKeys.Add(Keys.Y);
            m_validKeyboardKeys.Add(Keys.Z);

            //  List of assignable mouse buttons
            m_validMouseButtons.Add(MyMouseButtonsEnum.Left);
            m_validMouseButtons.Add(MyMouseButtonsEnum.Middle);
            m_validMouseButtons.Add(MyMouseButtonsEnum.Right);
            m_validMouseButtons.Add(MyMouseButtonsEnum.XButton1);
            m_validMouseButtons.Add(MyMouseButtonsEnum.XButton2);
            m_validMouseButtons.Add(MyMouseButtonsEnum.None);

            //  List of assignable joystick buttons
            m_validJoystickButtons.Add(MyJoystickButtonsEnum.J01);
            m_validJoystickButtons.Add(MyJoystickButtonsEnum.J02);
            m_validJoystickButtons.Add(MyJoystickButtonsEnum.J03);
            m_validJoystickButtons.Add(MyJoystickButtonsEnum.J04);
            m_validJoystickButtons.Add(MyJoystickButtonsEnum.J05);
            m_validJoystickButtons.Add(MyJoystickButtonsEnum.J06);
            m_validJoystickButtons.Add(MyJoystickButtonsEnum.J07);
            m_validJoystickButtons.Add(MyJoystickButtonsEnum.J08);
            m_validJoystickButtons.Add(MyJoystickButtonsEnum.J09);
            m_validJoystickButtons.Add(MyJoystickButtonsEnum.J10);
            m_validJoystickButtons.Add(MyJoystickButtonsEnum.J11);
            m_validJoystickButtons.Add(MyJoystickButtonsEnum.J12);
            m_validJoystickButtons.Add(MyJoystickButtonsEnum.J13);
            m_validJoystickButtons.Add(MyJoystickButtonsEnum.J14);
            m_validJoystickButtons.Add(MyJoystickButtonsEnum.J15);
            m_validJoystickButtons.Add(MyJoystickButtonsEnum.J16);
            m_validJoystickButtons.Add(MyJoystickButtonsEnum.JDLeft);
            m_validJoystickButtons.Add(MyJoystickButtonsEnum.JDRight);
            m_validJoystickButtons.Add(MyJoystickButtonsEnum.JDUp);
            m_validJoystickButtons.Add(MyJoystickButtonsEnum.JDDown);
            m_validJoystickButtons.Add(MyJoystickButtonsEnum.None);

            //  List of assignable joystick axes
            m_validJoystickAxes.Add(MyJoystickAxesEnum.Xpos);
            m_validJoystickAxes.Add(MyJoystickAxesEnum.Xneg);
            m_validJoystickAxes.Add(MyJoystickAxesEnum.Ypos);
            m_validJoystickAxes.Add(MyJoystickAxesEnum.Yneg);
            m_validJoystickAxes.Add(MyJoystickAxesEnum.Zpos);
            m_validJoystickAxes.Add(MyJoystickAxesEnum.Zneg);
            m_validJoystickAxes.Add(MyJoystickAxesEnum.RotationXpos);
            m_validJoystickAxes.Add(MyJoystickAxesEnum.RotationXneg);
            m_validJoystickAxes.Add(MyJoystickAxesEnum.RotationYpos);
            m_validJoystickAxes.Add(MyJoystickAxesEnum.RotationYneg);
            m_validJoystickAxes.Add(MyJoystickAxesEnum.RotationZpos);
            m_validJoystickAxes.Add(MyJoystickAxesEnum.RotationZneg);
            m_validJoystickAxes.Add(MyJoystickAxesEnum.Slider1pos);
            m_validJoystickAxes.Add(MyJoystickAxesEnum.Slider1neg);
            m_validJoystickAxes.Add(MyJoystickAxesEnum.Slider2pos);
            m_validJoystickAxes.Add(MyJoystickAxesEnum.Slider2neg);
            m_validJoystickAxes.Add(MyJoystickAxesEnum.None);

            #endregion

            //  IMPORTANT! If you add/remove a control you MUST add/remove default control list! 
            //  Default Controls list
            m_defaultGameControlsList[(int)MyGameControlEnums.FIRE_PRIMARY] = new MyControl(MyGameControlEnums.FIRE_PRIMARY, MyGuiGameControlsHelpers.GetGameControlHelper(MyGameControlEnums.FIRE_PRIMARY).DescriptionEnum, MyGuiControlTypeEnum.Weapons, MyMouseButtonsEnum.Left, Keys.LeftControl, MyJoystickButtonsEnum.J01, null);
            m_defaultGameControlsList[(int)MyGameControlEnums.FIRE_SECONDARY] = new MyControl(MyGameControlEnums.FIRE_SECONDARY, MyGuiGameControlsHelpers.GetGameControlHelper(MyGameControlEnums.FIRE_SECONDARY).DescriptionEnum, MyGuiControlTypeEnum.Weapons, MyMouseButtonsEnum.Right, MyFakes.ALT_AS_DEBUG_KEY ? Keys.None : Keys.LeftAlt, MyJoystickButtonsEnum.J02, null);
            m_defaultGameControlsList[(int)MyGameControlEnums.FIRE_THIRD] = new MyControl(MyGameControlEnums.FIRE_THIRD, MyGuiGameControlsHelpers.GetGameControlHelper(MyGameControlEnums.FIRE_THIRD).DescriptionEnum, MyGuiControlTypeEnum.Weapons, Keys.None);
            m_defaultGameControlsList[(int)MyGameControlEnums.FIRE_FOURTH] = new MyControl(MyGameControlEnums.FIRE_FOURTH, MyGuiGameControlsHelpers.GetGameControlHelper(MyGameControlEnums.FIRE_FOURTH).DescriptionEnum, MyGuiControlTypeEnum.Weapons, Keys.None);
            m_defaultGameControlsList[(int)MyGameControlEnums.FIRE_FIFTH] = new MyControl(MyGameControlEnums.FIRE_FIFTH, MyGuiGameControlsHelpers.GetGameControlHelper(MyGameControlEnums.FIRE_FIFTH).DescriptionEnum, MyGuiControlTypeEnum.Weapons, Keys.None);
            m_defaultGameControlsList[(int)MyGameControlEnums.FORWARD] = new MyControl(MyGameControlEnums.FORWARD, MyGuiGameControlsHelpers.GetGameControlHelper(MyGameControlEnums.FORWARD).DescriptionEnum, MyGuiControlTypeEnum.Navigation, Keys.W);
            m_defaultGameControlsList[(int)MyGameControlEnums.REVERSE] = new MyControl(MyGameControlEnums.REVERSE, MyGuiGameControlsHelpers.GetGameControlHelper(MyGameControlEnums.REVERSE).DescriptionEnum, MyGuiControlTypeEnum.Navigation, Keys.S);
            m_defaultGameControlsList[(int)MyGameControlEnums.UP_THRUST] = new MyControl(MyGameControlEnums.UP_THRUST, MyGuiGameControlsHelpers.GetGameControlHelper(MyGameControlEnums.UP_THRUST).DescriptionEnum, MyGuiControlTypeEnum.Navigation, null, Keys.F, MyJoystickButtonsEnum.JDUp, null);
            m_defaultGameControlsList[(int)MyGameControlEnums.DOWN_THRUST] = new MyControl(MyGameControlEnums.DOWN_THRUST, MyGuiGameControlsHelpers.GetGameControlHelper(MyGameControlEnums.DOWN_THRUST).DescriptionEnum, MyGuiControlTypeEnum.Navigation, null, Keys.C, MyJoystickButtonsEnum.JDDown, null);
            m_defaultGameControlsList[(int)MyGameControlEnums.STRAFE_LEFT] = new MyControl(MyGameControlEnums.STRAFE_LEFT, MyGuiGameControlsHelpers.GetGameControlHelper(MyGameControlEnums.STRAFE_LEFT).DescriptionEnum, MyGuiControlTypeEnum.Navigation, null, Keys.A, MyJoystickButtonsEnum.JDLeft, null);
            m_defaultGameControlsList[(int)MyGameControlEnums.STRAFE_RIGHT] = new MyControl(MyGameControlEnums.STRAFE_RIGHT, MyGuiGameControlsHelpers.GetGameControlHelper(MyGameControlEnums.STRAFE_RIGHT).DescriptionEnum, MyGuiControlTypeEnum.Navigation, null, Keys.D, MyJoystickButtonsEnum.JDRight, null);
            m_defaultGameControlsList[(int)MyGameControlEnums.ROLL_LEFT] = new MyControl(MyGameControlEnums.ROLL_LEFT, MyGuiGameControlsHelpers.GetGameControlHelper(MyGameControlEnums.ROLL_LEFT).DescriptionEnum, MyGuiControlTypeEnum.Navigation, Keys.Q);
            m_defaultGameControlsList[(int)MyGameControlEnums.ROLL_RIGHT] = new MyControl(MyGameControlEnums.ROLL_RIGHT, MyGuiGameControlsHelpers.GetGameControlHelper(MyGameControlEnums.ROLL_RIGHT).DescriptionEnum, MyGuiControlTypeEnum.Navigation, Keys.E);
            m_defaultGameControlsList[(int)MyGameControlEnums.ROTATION_LEFT] = new MyControl(MyGameControlEnums.ROTATION_LEFT, MyGuiGameControlsHelpers.GetGameControlHelper(MyGameControlEnums.ROTATION_LEFT).DescriptionEnum, MyGuiControlTypeEnum.Navigation, null, Keys.Left, null, MyJoystickAxesEnum.Xneg);
            m_defaultGameControlsList[(int)MyGameControlEnums.ROTATION_RIGHT] = new MyControl(MyGameControlEnums.ROTATION_RIGHT, MyGuiGameControlsHelpers.GetGameControlHelper(MyGameControlEnums.ROTATION_RIGHT).DescriptionEnum, MyGuiControlTypeEnum.Navigation, null, Keys.Right, null, MyJoystickAxesEnum.Xpos);
            m_defaultGameControlsList[(int)MyGameControlEnums.ROTATION_UP] = new MyControl(MyGameControlEnums.ROTATION_UP, MyGuiGameControlsHelpers.GetGameControlHelper(MyGameControlEnums.ROTATION_UP).DescriptionEnum, MyGuiControlTypeEnum.Navigation, null, Keys.Up, null, MyJoystickAxesEnum.Yneg);
            m_defaultGameControlsList[(int)MyGameControlEnums.ROTATION_DOWN] = new MyControl(MyGameControlEnums.ROTATION_DOWN, MyGuiGameControlsHelpers.GetGameControlHelper(MyGameControlEnums.ROTATION_DOWN).DescriptionEnum, MyGuiControlTypeEnum.Navigation, null, Keys.Down, null, MyJoystickAxesEnum.Ypos);
            m_defaultGameControlsList[(int)MyGameControlEnums.HEADLIGHTS] = new MyControl(MyGameControlEnums.HEADLIGHTS, MyGuiGameControlsHelpers.GetGameControlHelper(MyGameControlEnums.HEADLIGHTS).DescriptionEnum, MyGuiControlTypeEnum.Systems2, Keys.H);
            //m_defaultGameControlsList[(int)MyGameControlEnums.ENGINE_SHUTDOWN] = new MyControl(MyGameControlEnums.ENGINE_SHUTDOWN, MyGuiGameControlsHelpers.GetGameControlHelper(MyGameControlEnums.ENGINE_SHUTDOWN).DescriptionEnum, MyGuiControlTypeEnum.Systems1, Keys.T);
            m_defaultGameControlsList[(int)MyGameControlEnums.HARVEST] = new MyControl(MyGameControlEnums.HARVEST, MyGuiGameControlsHelpers.GetGameControlHelper(MyGameControlEnums.HARVEST).DescriptionEnum, MyGuiControlTypeEnum.Systems2, Keys.B);
            m_defaultGameControlsList[(int)MyGameControlEnums.ZOOM_IN] = new MyControl(MyGameControlEnums.ZOOM_IN, MyGuiGameControlsHelpers.GetGameControlHelper(MyGameControlEnums.ZOOM_IN).DescriptionEnum, MyGuiControlTypeEnum.Systems1, Keys.R);
            m_defaultGameControlsList[(int)MyGameControlEnums.ZOOM_OUT] = new MyControl(MyGameControlEnums.ZOOM_OUT, MyGuiGameControlsHelpers.GetGameControlHelper(MyGameControlEnums.ZOOM_OUT).DescriptionEnum, MyGuiControlTypeEnum.Systems1, Keys.V);
            m_defaultGameControlsList[(int)MyGameControlEnums.QUICK_ZOOM] = new MyControl(MyGameControlEnums.QUICK_ZOOM, MyGuiGameControlsHelpers.GetGameControlHelper(MyGameControlEnums.QUICK_ZOOM).DescriptionEnum, MyGuiControlTypeEnum.Systems1, Keys.G);
            m_defaultGameControlsList[(int)MyGameControlEnums.TRAVEL] = new MyControl(MyGameControlEnums.TRAVEL, MyGuiGameControlsHelpers.GetGameControlHelper(MyGameControlEnums.TRAVEL).DescriptionEnum, MyGuiControlTypeEnum.Systems1, Keys.Home);
            m_defaultGameControlsList[(int)MyGameControlEnums.SELECT_AMMO_BULLET] = new MyControl(MyGameControlEnums.SELECT_AMMO_BULLET, MyGuiGameControlsHelpers.GetGameControlHelper(MyGameControlEnums.SELECT_AMMO_BULLET).DescriptionEnum, MyGuiControlTypeEnum.Weapons, Keys.D1);
            m_defaultGameControlsList[(int)MyGameControlEnums.SELECT_AMMO_MISSILE] = new MyControl(MyGameControlEnums.SELECT_AMMO_MISSILE, MyGuiGameControlsHelpers.GetGameControlHelper(MyGameControlEnums.SELECT_AMMO_MISSILE).DescriptionEnum, MyGuiControlTypeEnum.Weapons, Keys.D2);
            m_defaultGameControlsList[(int)MyGameControlEnums.SELECT_AMMO_CANNON] = new MyControl(MyGameControlEnums.SELECT_AMMO_CANNON, MyGuiGameControlsHelpers.GetGameControlHelper(MyGameControlEnums.SELECT_AMMO_CANNON).DescriptionEnum, MyGuiControlTypeEnum.Weapons, Keys.D3);
            m_defaultGameControlsList[(int)MyGameControlEnums.SELECT_AMMO_UNIVERSAL_LAUNCHER_FRONT] = new MyControl(MyGameControlEnums.SELECT_AMMO_UNIVERSAL_LAUNCHER_FRONT, MyGuiGameControlsHelpers.GetGameControlHelper(MyGameControlEnums.SELECT_AMMO_UNIVERSAL_LAUNCHER_FRONT).DescriptionEnum, MyGuiControlTypeEnum.Weapons, Keys.D4);
            m_defaultGameControlsList[(int)MyGameControlEnums.AUTO_LEVEL] = new MyControl(MyGameControlEnums.AUTO_LEVEL, MyGuiGameControlsHelpers.GetGameControlHelper(MyGameControlEnums.AUTO_LEVEL).DescriptionEnum, MyGuiControlTypeEnum.Systems2, Keys.N);
            m_defaultGameControlsList[(int)MyGameControlEnums.DRILL] = new MyControl(MyGameControlEnums.DRILL, MyGuiGameControlsHelpers.GetGameControlHelper(MyGameControlEnums.DRILL).DescriptionEnum, MyGuiControlTypeEnum.Systems2, Keys.M);
            m_defaultGameControlsList[(int)MyGameControlEnums.REAR_CAM] = new MyControl(MyGameControlEnums.REAR_CAM, MyGuiGameControlsHelpers.GetGameControlHelper(MyGameControlEnums.REAR_CAM).DescriptionEnum, MyGuiControlTypeEnum.Systems1, Keys.K);
            m_defaultGameControlsList[(int)MyGameControlEnums.SELECT_AMMO_UNIVERSAL_LAUNCHER_BACK] = new MyControl(MyGameControlEnums.SELECT_AMMO_UNIVERSAL_LAUNCHER_BACK, MyGuiGameControlsHelpers.GetGameControlHelper(MyGameControlEnums.SELECT_AMMO_UNIVERSAL_LAUNCHER_BACK).DescriptionEnum, MyGuiControlTypeEnum.Weapons, Keys.D5);
            //m_defaultGameControlsList[(int)MyGameControlEnums.SWITCH_RADAR_MODE] = new MyControl(MyGameControlEnums.SWITCH_RADAR_MODE, MyGuiGameControlsHelpers.GetGameControlHelper(MyGameControlEnums.SWITCH_RADAR_MODE).DescriptionEnum, MyGuiControlTypeEnum.Systems2, Keys.OemTilde);
            m_defaultGameControlsList[(int)MyGameControlEnums.AFTERBURNER] = new MyControl(MyGameControlEnums.AFTERBURNER, MyGuiGameControlsHelpers.GetGameControlHelper(MyGameControlEnums.AFTERBURNER).DescriptionEnum, MyGuiControlTypeEnum.Navigation, Keys.LeftShift);
            //m_defaultGameControlsList[(int)MyGameControlEnums.FULLSCREEN_RADAR] = new MyControl(MyGameControlEnums.FULLSCREEN_RADAR, MyGuiGameControlsHelpers.GetGameControlHelper(MyGameControlEnums.FULLSCREEN_RADAR).DescriptionEnum, MyGuiControlTypeEnum.Systems2, Keys.Tab);
            m_defaultGameControlsList[(int)MyGameControlEnums.CHANGE_DRONE_MODE] = new MyControl(MyGameControlEnums.CHANGE_DRONE_MODE, MyGuiGameControlsHelpers.GetGameControlHelper(MyGameControlEnums.CHANGE_DRONE_MODE).DescriptionEnum, MyGuiControlTypeEnum.Systems2, Keys.End);
            //m_defaultGameControlsList[(int)MyGameControlEnums.RADAR_ZOOM_IN] = new MyControl(MyGameControlEnums.RADAR_ZOOM_IN, MyGuiGameControlsHelpers.GetGameControlHelper(MyGameControlEnums.RADAR_ZOOM_IN).DescriptionEnum, MyGuiControlTypeEnum.Systems2, Keys.OemPlus, Keys.Add);
            //m_defaultGameControlsList[(int)MyGameControlEnums.RADAR_ZOOM_OUT] = new MyControl(MyGameControlEnums.RADAR_ZOOM_OUT, MyGuiGameControlsHelpers.GetGameControlHelper(MyGameControlEnums.RADAR_ZOOM_OUT).DescriptionEnum, MyGuiControlTypeEnum.Systems2, Keys.OemMinus, Keys.Subtract);
            m_defaultGameControlsList[(int)MyGameControlEnums.MOVEMENT_SLOWDOWN] = new MyControl(MyGameControlEnums.MOVEMENT_SLOWDOWN, MyGuiGameControlsHelpers.GetGameControlHelper(MyGameControlEnums.MOVEMENT_SLOWDOWN).DescriptionEnum, MyGuiControlTypeEnum.Systems2, Keys.X);
            m_defaultGameControlsList[(int)MyGameControlEnums.VIEW_MODE] = new MyControl(MyGameControlEnums.VIEW_MODE, MyGuiGameControlsHelpers.GetGameControlHelper(MyGameControlEnums.VIEW_MODE).DescriptionEnum, MyGuiControlTypeEnum.Systems2, Keys.O);
            m_defaultGameControlsList[(int)MyGameControlEnums.WEAPON_SPECIAL] = new MyControl(MyGameControlEnums.WEAPON_SPECIAL, MyGuiGameControlsHelpers.GetGameControlHelper(MyGameControlEnums.WEAPON_SPECIAL).DescriptionEnum, MyGuiControlTypeEnum.Weapons, Keys.Z);
            m_defaultGameControlsList[(int)MyGameControlEnums.WHEEL_CONTROL] = new MyControl(MyGameControlEnums.WHEEL_CONTROL, MyGuiGameControlsHelpers.GetGameControlHelper(MyGameControlEnums.WHEEL_CONTROL).DescriptionEnum, MyGuiControlTypeEnum.Systems2, MyMouseButtonsEnum.Middle, Keys.Back, null, null);
            m_defaultGameControlsList[(int)MyGameControlEnums.MISSION_DIALOG] = new MyControl(MyGameControlEnums.MISSION_DIALOG, MyGuiGameControlsHelpers.GetGameControlHelper(MyGameControlEnums.MISSION_DIALOG).DescriptionEnum, MyGuiControlTypeEnum.Deleted, Keys.J);
            m_defaultGameControlsList[(int)MyGameControlEnums.HEADLIGTHS_DISTANCE] = new MyControl(MyGameControlEnums.HEADLIGTHS_DISTANCE, MyGuiGameControlsHelpers.GetGameControlHelper(MyGameControlEnums.HEADLIGTHS_DISTANCE).DescriptionEnum, MyGuiControlTypeEnum.Systems2, Keys.L);
            m_defaultGameControlsList[(int)MyGameControlEnums.GPS] = new MyControl(MyGameControlEnums.GPS, MyGuiGameControlsHelpers.GetGameControlHelper(MyGameControlEnums.GPS).DescriptionEnum, MyGuiControlTypeEnum.Systems1, Keys.T);

            m_defaultGameControlsList[(int)MyGameControlEnums.NOTIFICATION_CONFIRMATION] = new MyControl(MyGameControlEnums.NOTIFICATION_CONFIRMATION, MyGuiGameControlsHelpers.GetGameControlHelper(MyGameControlEnums.NOTIFICATION_CONFIRMATION).DescriptionEnum, MyGuiControlTypeEnum.Systems1, Keys.Y);
            m_defaultGameControlsList[(int)MyGameControlEnums.INVENTORY] = new MyControl(MyGameControlEnums.INVENTORY, MyGuiGameControlsHelpers.GetGameControlHelper(MyGameControlEnums.INVENTORY).DescriptionEnum, MyGuiControlTypeEnum.Systems1, Keys.I);

            //m_defaultGameControlsList[(int)MyGameControlEnums.COMM_AVATAR_SELECT] = new MyControl(MyGameControlEnums.COMM_AVATAR_SELECT, MyGuiGameControlsHelpers.GetGameControlHelper(MyGameControlEnums.COMM_AVATAR_SELECT).DescriptionEnum, MyGuiControlTypeEnum.Communications, Keys.None);

            m_defaultGameControlsList[(int)MyGameControlEnums.PREVIOUS_CAMERA] = new MyControl(MyGameControlEnums.PREVIOUS_CAMERA, MyGuiGameControlsHelpers.GetGameControlHelper(MyGameControlEnums.PREVIOUS_CAMERA).DescriptionEnum, MyGuiControlTypeEnum.Systems1, Keys.OemOpenBrackets);
            m_defaultGameControlsList[(int)MyGameControlEnums.NEXT_CAMERA] = new MyControl(MyGameControlEnums.NEXT_CAMERA, MyGuiGameControlsHelpers.GetGameControlHelper(MyGameControlEnums.NEXT_CAMERA).DescriptionEnum, MyGuiControlTypeEnum.Systems1, Keys.OemCloseBrackets);
            m_defaultGameControlsList[(int)MyGameControlEnums.USE] = new MyControl(MyGameControlEnums.USE, MyGuiGameControlsHelpers.GetGameControlHelper(MyGameControlEnums.USE).DescriptionEnum, MyGuiControlTypeEnum.Systems1, Keys.Space);

            m_defaultGameControlsList[(int)MyGameControlEnums.FIRE_HOLOGRAM_FRONT   ] = new MyControl(MyGameControlEnums.FIRE_HOLOGRAM_FRONT   , MyGuiGameControlsHelpers.GetGameControlHelper(MyGameControlEnums.FIRE_HOLOGRAM_FRONT   ).DescriptionEnum, MyGuiControlTypeEnum.SpecialWeapons);
            m_defaultGameControlsList[(int)MyGameControlEnums.FIRE_HOLOGRAM_BACK    ] = new MyControl(MyGameControlEnums.FIRE_HOLOGRAM_BACK    , MyGuiGameControlsHelpers.GetGameControlHelper(MyGameControlEnums.FIRE_HOLOGRAM_BACK    ).DescriptionEnum, MyGuiControlTypeEnum.SpecialWeapons, Keys.NumPad1);
            m_defaultGameControlsList[(int)MyGameControlEnums.FIRE_BASIC_MINE_FRONT ] = new MyControl(MyGameControlEnums.FIRE_BASIC_MINE_FRONT , MyGuiGameControlsHelpers.GetGameControlHelper(MyGameControlEnums.FIRE_BASIC_MINE_FRONT ).DescriptionEnum, MyGuiControlTypeEnum.SpecialWeapons);
            m_defaultGameControlsList[(int)MyGameControlEnums.FIRE_BASIC_MINE_BACK  ] = new MyControl(MyGameControlEnums.FIRE_BASIC_MINE_BACK  , MyGuiGameControlsHelpers.GetGameControlHelper(MyGameControlEnums.FIRE_BASIC_MINE_BACK  ).DescriptionEnum, MyGuiControlTypeEnum.SpecialWeapons, Keys.NumPad2);
            m_defaultGameControlsList[(int)MyGameControlEnums.FIRE_SMART_MINE_FRONT ] = new MyControl(MyGameControlEnums.FIRE_SMART_MINE_FRONT , MyGuiGameControlsHelpers.GetGameControlHelper(MyGameControlEnums.FIRE_SMART_MINE_FRONT ).DescriptionEnum, MyGuiControlTypeEnum.SpecialWeapons);
            m_defaultGameControlsList[(int)MyGameControlEnums.FIRE_SMART_MINE_BACK  ] = new MyControl(MyGameControlEnums.FIRE_SMART_MINE_BACK  , MyGuiGameControlsHelpers.GetGameControlHelper(MyGameControlEnums.FIRE_SMART_MINE_BACK  ).DescriptionEnum, MyGuiControlTypeEnum.SpecialWeapons, Keys.NumPad3);
            m_defaultGameControlsList[(int)MyGameControlEnums.FIRE_FLASH_BOMB_FRONT ] = new MyControl(MyGameControlEnums.FIRE_FLASH_BOMB_FRONT , MyGuiGameControlsHelpers.GetGameControlHelper(MyGameControlEnums.FIRE_FLASH_BOMB_FRONT ).DescriptionEnum, MyGuiControlTypeEnum.SpecialWeapons);
            m_defaultGameControlsList[(int)MyGameControlEnums.FIRE_FLASH_BOMB_BACK  ] = new MyControl(MyGameControlEnums.FIRE_FLASH_BOMB_BACK  , MyGuiGameControlsHelpers.GetGameControlHelper(MyGameControlEnums.FIRE_FLASH_BOMB_BACK  ).DescriptionEnum, MyGuiControlTypeEnum.SpecialWeapons, Keys.NumPad4);
            m_defaultGameControlsList[(int)MyGameControlEnums.FIRE_DECOY_FLARE_FRONT] = new MyControl(MyGameControlEnums.FIRE_DECOY_FLARE_FRONT, MyGuiGameControlsHelpers.GetGameControlHelper(MyGameControlEnums.FIRE_DECOY_FLARE_FRONT).DescriptionEnum, MyGuiControlTypeEnum.SpecialWeapons);
            m_defaultGameControlsList[(int)MyGameControlEnums.FIRE_DECOY_FLARE_BACK ] = new MyControl(MyGameControlEnums.FIRE_DECOY_FLARE_BACK , MyGuiGameControlsHelpers.GetGameControlHelper(MyGameControlEnums.FIRE_DECOY_FLARE_BACK ).DescriptionEnum, MyGuiControlTypeEnum.SpecialWeapons, Keys.NumPad5);
            m_defaultGameControlsList[(int)MyGameControlEnums.FIRE_SMOKE_BOMB_FRONT ] = new MyControl(MyGameControlEnums.FIRE_SMOKE_BOMB_FRONT , MyGuiGameControlsHelpers.GetGameControlHelper(MyGameControlEnums.FIRE_SMOKE_BOMB_FRONT ).DescriptionEnum, MyGuiControlTypeEnum.SpecialWeapons, Keys.NumPad6);
            m_defaultGameControlsList[(int)MyGameControlEnums.FIRE_SMOKE_BOMB_BACK  ] = new MyControl(MyGameControlEnums.FIRE_SMOKE_BOMB_BACK  , MyGuiGameControlsHelpers.GetGameControlHelper(MyGameControlEnums.FIRE_SMOKE_BOMB_BACK  ).DescriptionEnum, MyGuiControlTypeEnum.SpecialWeapons);

            m_defaultGameControlsList[(int)MyGameControlEnums.DRONE_DEPLOY] = new MyControl(MyGameControlEnums.DRONE_DEPLOY, MyGuiGameControlsHelpers.GetGameControlHelper(MyGameControlEnums.DRONE_DEPLOY).DescriptionEnum, MyGuiControlTypeEnum.Systems2, Keys.Insert);
            m_defaultGameControlsList[(int)MyGameControlEnums.DRONE_CONTROL] = new MyControl(MyGameControlEnums.DRONE_CONTROL, MyGuiGameControlsHelpers.GetGameControlHelper(MyGameControlEnums.DRONE_CONTROL).DescriptionEnum, MyGuiControlTypeEnum.Systems2, Keys.Delete);
            m_defaultGameControlsList[(int)MyGameControlEnums.CONTROL_SECONDARY_CAMERA] = new MyControl(MyGameControlEnums.CONTROL_SECONDARY_CAMERA, MyGuiGameControlsHelpers.GetGameControlHelper(MyGameControlEnums.CONTROL_SECONDARY_CAMERA).DescriptionEnum, MyGuiControlTypeEnum.Systems1, Keys.OemTilde);

            m_defaultGameControlsList[(int)MyGameControlEnums.PREV_TARGET] = new MyControl(MyGameControlEnums.PREV_TARGET, MyGuiGameControlsHelpers.GetGameControlHelper(MyGameControlEnums.PREV_TARGET).DescriptionEnum, MyGuiControlTypeEnum.Weapons, Keys.OemComma);
            m_defaultGameControlsList[(int)MyGameControlEnums.NEXT_TARGET] = new MyControl(MyGameControlEnums.NEXT_TARGET, MyGuiGameControlsHelpers.GetGameControlHelper(MyGameControlEnums.NEXT_TARGET).DescriptionEnum, MyGuiControlTypeEnum.Weapons, Keys.OemPeriod);

            m_defaultGameControlsList[(int)MyGameControlEnums.CHAT] = new MyControl(MyGameControlEnums.CHAT, MyGuiGameControlsHelpers.GetGameControlHelper(MyGameControlEnums.CHAT).DescriptionEnum, MyGuiControlTypeEnum.Systems2, Keys.Enter);
            m_defaultGameControlsList[(int)MyGameControlEnums.SCORE] = new MyControl(MyGameControlEnums.SCORE, MyGuiGameControlsHelpers.GetGameControlHelper(MyGameControlEnums.SCORE).DescriptionEnum, MyGuiControlTypeEnum.Systems2, Keys.Tab);

            // Editor default controls
            m_defaultEditorControlsList[(int)MyEditorControlEnums.DECREASE_GRID_SCALE] = new MyControl(MyEditorControlEnums.DECREASE_GRID_SCALE, MyGuiGameControlsHelpers.GetEditorControlHelper(MyEditorControlEnums.DECREASE_GRID_SCALE).DescriptionEnum, MyGuiControlTypeEnum.Editor, Keys.OemMinus);
            m_defaultEditorControlsList[(int)MyEditorControlEnums.INCREASE_GRID_SCALE] = new MyControl(MyEditorControlEnums.INCREASE_GRID_SCALE, MyGuiGameControlsHelpers.GetEditorControlHelper(MyEditorControlEnums.INCREASE_GRID_SCALE).DescriptionEnum, MyGuiControlTypeEnum.Editor, Keys.OemPlus);
            m_defaultEditorControlsList[(int)MyEditorControlEnums.PRIMARY_ACTION_KEY] = new MyControl(MyEditorControlEnums.PRIMARY_ACTION_KEY, MyGuiGameControlsHelpers.GetEditorControlHelper(MyEditorControlEnums.PRIMARY_ACTION_KEY).DescriptionEnum, MyGuiControlTypeEnum.Editor, MyMouseButtonsEnum.Left);
            m_defaultEditorControlsList[(int)MyEditorControlEnums.SECONDARY_ACTION_KEY] = new MyControl(MyEditorControlEnums.SECONDARY_ACTION_KEY, MyGuiGameControlsHelpers.GetEditorControlHelper(MyEditorControlEnums.SECONDARY_ACTION_KEY).DescriptionEnum, MyGuiControlTypeEnum.Editor, MyMouseButtonsEnum.Right);
            m_defaultEditorControlsList[(int)MyEditorControlEnums.SWITCH_GIZMO_MODE] = new MyControl(MyEditorControlEnums.SWITCH_GIZMO_MODE, MyGuiGameControlsHelpers.GetEditorControlHelper(MyEditorControlEnums.SWITCH_GIZMO_MODE).DescriptionEnum, MyGuiControlTypeEnum.Editor, Keys.T);
            m_defaultEditorControlsList[(int)MyEditorControlEnums.SWITCH_GIZMO_SPACE] = new MyControl(MyEditorControlEnums.SWITCH_GIZMO_SPACE, MyGuiGameControlsHelpers.GetEditorControlHelper(MyEditorControlEnums.SWITCH_GIZMO_SPACE).DescriptionEnum, MyGuiControlTypeEnum.Editor, Keys.Space);
            m_defaultEditorControlsList[(int)MyEditorControlEnums.VOXEL_HAND] = new MyControl(MyEditorControlEnums.VOXEL_HAND, MyGuiGameControlsHelpers.GetEditorControlHelper(MyEditorControlEnums.VOXEL_HAND).DescriptionEnum, MyGuiControlTypeEnum.Editor, Keys.V);

            CheckValidControls(m_defaultGameControlsList);
            CheckValidControls(m_defaultEditorControlsList);

            CloneControls(m_defaultGameControlsList, m_gameControlsList);
            CloneControls(m_defaultEditorControlsList, m_editorControlsList);

            LoadControls();
            TakeSnapshot();

            //  Make sure that DirectInput has been initialized
            InitializeJoystickIfPossible();

            m_keyboardState = new MyGuiLocalizedKeyboardState();
            m_actualMouseState = MinerWars.AppCode.Toolkit.Input.MyWindowsMouse.GetCurrentState();

            if (MyVideoModeManager.IsHardwareCursorUsed())
            {
                /*
                if (MyMinerGame.Static.Window.Handle == GetForegroundWindow() && MyGuiManager.GetScreenWithFocus() != null && MyGuiManager.GetScreenWithFocus().GetDrawMouseCursor() == false)
                    SetMouseToScreenCenter();
                */
            }else
                SetMouseToScreenCenter();

            UpdateStates();
        }

        ~MyGuiInput()
        {
            Dispose();
        }

        void CheckValidControls(MyControl[] controls)
        {
            foreach (MyControl control in controls)
            {                
                MyCommonDebugUtils.AssertDebug(IsKeyValid(control.GetKeyboardControl()));                
                MyCommonDebugUtils.AssertDebug(IsMouseButtonValid(control.GetMouseControl()));
                MyCommonDebugUtils.AssertDebug(IsJoystickButtonValid(control.GetJoystickControl()));                
            }
        }

        public List<string> EnumerateJoystickNames()
        {
            var results = new List<string>();

            DirectInput dinput = new DirectInput();
            var devices = dinput.GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AttachedOnly);
            for (int i = 0; i < devices.Count; i++)
            {
                var device = devices[i];
                results.Add(device.InstanceName);
            }
            return results;
        }

        //call this on call back when something is beeing plugged in or unplugged
        void InitializeJoystickIfPossible()
        {
            // try to dispose of the old joystick
            if (m_joystick != null)
            {
                m_joystick.Dispose();
                m_joystick = null;
                m_joystickConnected = false;
                m_joystickType = null;
            }
            if (m_joystick == null)
            {
                // Joystick disabled?
                if (m_joystickInstanceName == null) return;

                //  Make sure that DirectInput has been initialized
                DirectInput dinput = new DirectInput();

                //  Try to grab the joystick with the correct instance name
                foreach (var device in dinput.GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AttachedOnly))
                {
                    if (device.InstanceName != m_joystickInstanceName)
                        continue;
                    
                    try
                    {
                        //device.Type
                        m_joystick = new Joystick(dinput, device.InstanceGuid);
                        m_joystickType = device.Type;

                        MethodInfo setCooperativeLevel = typeof(Device).GetMethod("SetCooperativeLevel",
                                                                                     new[]
                                                                                         {
                                                                                             typeof (IntPtr),
                                                                                             typeof (CooperativeLevel)
                                                                                         });

                        // Workaround for not need to reference System.Windows.Forms
                        setCooperativeLevel.Invoke(m_joystick,
                                                   new object[]
                                                       {
                                                           MyMinerGame.Static.Window.NativeWindow.Handle,
                                                           CooperativeLevel.Exclusive | CooperativeLevel.Foreground
                                                       });

                        break;
                    }
                    catch (SharpDX.SharpDXException)
                    {
                    }
                }
                
                // load and acquire joystick
                // both joystick and xbox 360 gamepad are treated as joystick device by slimdx
                if (m_joystick != null)
                {
                    int sliderCount = 0;
                    m_joystickXAxisSupported = m_joystickYAxisSupported = m_joystickZAxisSupported = false;
                    m_joystickRotationXAxisSupported = m_joystickRotationYAxisSupported = m_joystickRotationZAxisSupported = false;
                    m_joystickSlider1AxisSupported = m_joystickSlider2AxisSupported = false;
                    foreach (DeviceObjectInstance doi in m_joystick.GetObjects())
                    {
                        if ((doi.ObjectId.Flags & DeviceObjectTypeFlags.Axis) != 0)
                        {
                            // set range 0..65535 for each axis
                            m_joystick.GetObjectPropertiesById(doi.ObjectId).Range = new InputRange(0, 65535);

                            // find out which axes are supported
                            if (doi.ObjectType == ObjectGuid.XAxis) m_joystickXAxisSupported = true;
                            else if (doi.ObjectType == ObjectGuid.YAxis) m_joystickYAxisSupported = true;
                            else if (doi.ObjectType == ObjectGuid.ZAxis) m_joystickZAxisSupported = true;
                            else if (doi.ObjectType == ObjectGuid.RxAxis) m_joystickRotationXAxisSupported = true;
                            else if (doi.ObjectType == ObjectGuid.RyAxis) m_joystickRotationYAxisSupported = true;
                            else if (doi.ObjectType == ObjectGuid.RzAxis) m_joystickRotationZAxisSupported = true;
                            else if (doi.ObjectType == ObjectGuid.Slider)
                            {
                                sliderCount++;
                                if (sliderCount >= 1) m_joystickSlider1AxisSupported = true;
                                if (sliderCount >= 2) m_joystickSlider2AxisSupported = true;
                            }
                        }
                    }

                    // acquire the device
                    m_joystick.Acquire();
                    m_joystickConnected = true;
                }
                dinput.Dispose();
            }
        }

        // hook to wnd proc and catch WM_DEVICECHANGE event
        private unsafe int WndHookProc(int nCode, int wParam, int lParam)
        {
            if (nCode >= 0)
            {
                if (((MyWindowsAPIWrapper.DeviceChangeHookStruct*)lParam)->message == MyWindowsAPIWrapper.WM_DEVICECHANGE)
                {
                    InitializeJoystickIfPossible();
                }
            }
            return MyWindowsAPIWrapper.CallNextHookEx(m_hHook, nCode, wParam, lParam);
        }

        private void InitDevicePluginHandlerCallBack()
        {
            m_hookHandler = new MyWindowsAPIWrapper.HookProc(WndHookProc);
            m_hHook = MyWindowsAPIWrapper.SetWindowsHookEx(MyWindowsAPIWrapper.HookType.WH_CALLWNDPROC, m_hookHandler, (IntPtr)0, (uint) AppDomain.GetCurrentThreadId());
        }

        private void UninitDevicePluginHandlerCallBack()
        {
            if (m_hHook != 0)
            {
                MyWindowsAPIWrapper.UnhookWindowsHookEx(m_hHook);
                m_hHook = 0;
            }
        }
        

        void UpdateStates()
        {
            m_previousMouseState = m_actualMouseState;
            m_keyboardState.UpdateStates();
            m_actualMouseState = MinerWars.AppCode.Toolkit.Input.MyWindowsMouse.GetCurrentState();
            if (IsJoystickConnected())
            {
                //  Try/catch block around the joystick .Poll() function to catch an exception thrown when the device is detached DURING gameplay. 
                try
                {
                    m_joystick.Acquire();
                    m_joystick.Poll();

                    m_joystick.Poll();//grab next joystick state
                    m_previousJoystickState = m_actualJoystickState;
                    m_actualJoystickState = ((Joystick)m_joystick).GetCurrentState();
                }
                catch
                {
                    m_joystickConnected = false;
                }
            }
        }

        //  Update keyboard/mouse input and return true if application has focus (is active). Otherwise false.
        public bool Update()
        {
            bool ret;
            
            if (MyMinerGame.Static.IsActive == true && 
                ((MinerWars.AppCode.ExternalEditor.MyEditorBase.Static == null && MyMinerGame.Static.Window.NativeWindow.Handle == GetForegroundWindow())
                ||
                (MinerWars.AppCode.ExternalEditor.MyEditorBase.Static != null && !MinerWars.AppCode.ExternalEditor.MyEditorBase.IsEditorActive))
                )
            {
                                

                if (m_previousUpdateWasLostFocus == true)
                {
                    // we sets game mouse cursor to position, when was windows mouse cursor
                    MyMouseState windowsMouseState = MinerWars.AppCode.Toolkit.Input.MyWindowsMouse.GetCurrentState();
                    MyGuiManager.MouseCursorPosition = new MinerWarsMath.Vector2((float)windowsMouseState.X / MyMinerGame.ScreenSize.X, (float)windowsMouseState.Y / MyMinerGame.ScreenSize.Y);                    

                    //  When we return from lost focus, we must set mouse to the middle of the screen, otherwise we will see jump as user moved the mouse while he was out-of-focus
                    if (MyVideoModeManager.IsHardwareCursorUsed()) {
                        if(MyGuiManager.GetScreenWithFocus() != null && MyGuiManager.GetScreenWithFocus().GetDrawMouseCursor() == false)    
                            SetMouseToScreenCenter();                                        
                    } else
                        SetMouseToScreenCenter();
                }

                UpdateStates();

                if (IsNewLeftMouseReleased())
                {
                    var screenCoordinate = MyGuiManager.GetScreenCoordinateFromNormalizedCoordinate(MyGuiManager.MouseCursorPosition);
                    
                    m_leftButtonDoubleClick = MyMinerGame.TotalTimeInMilliseconds - m_lastLeftButtonClickTime < DOUBLE_CLICK_DELAY &&
                        (screenCoordinate - m_lastLeftButtonMousePosition).LengthSquared() < DOUBLE_CLICK_MAXIMUM_DISTANCE_SQUARED;

                    m_lastLeftButtonClickTime = m_leftButtonDoubleClick ? 0 : MyMinerGame.TotalTimeInMilliseconds;
                    m_lastLeftButtonMousePosition = screenCoordinate;
                }
                else
                {
                    m_leftButtonDoubleClick = false;
                }

                //  Set mouse position to the middle of the screen. We must do it here, right after we retrieved latest mouse state, because
                //  if rest of this update method takes lonk time and we set it then, all mouse movement between this place and end of this
                //  method will be lost and user will feel like we are out of sync
                if (MyVideoModeManager.IsHardwareCursorUsed())
                {

                    if (MyGuiManager.GetScreenWithFocus() != null && MyGuiManager.GetScreenWithFocus().GetDrawMouseCursor() == false && !MyGuiManager.InputToNonFocusedScreens)
                    {
                        /*
                        Trace.SendMsgLastCall(MyGuiManager.GetScreenWithFocus().GetFriendlyName());
                        Trace.SendMsgLastCall(MyGuiManager.GetScreenWithFocus().GetDrawMouseCursor().ToString());
                        Trace.SendMsgLastCall("tu");
                        SetMouseToScreenCenter();
                        */
                    }
                }
                else
                    SetMouseToScreenCenter();

                ret = true;
            }
            else
            {
                ret = false;
            }

            m_previousUpdateWasLostFocus = !MyMinerGame.Static.IsActive;

            return ret;
        }

        //  Return true if ANY key IS pressed, that means that the key was pressed now. During previous Update it wasn't pressed at all.
        public bool IsAnyKeyPress()
        {
            return (m_keyboardState.IsAnyKeyPressed());
        }

        //  Return true if ANY NEW key IS pressed, that means that the key was pressed now. During previous Update it wasn't pressed at all.
        public bool IsAnyNewKeyPress()
        {
            return (m_keyboardState.IsAnyKeyPressed() && !m_keyboardState.GetPreviousKeyboardState().IsAnyKeyPressed());
        }

        //  Return true if ANY mouse key IS pressed.
        public bool IsAnyMousePress()
        {
            return (m_actualMouseState.LeftButton) || (m_actualMouseState.MiddleButton) || (m_actualMouseState.RightButton);
        }

        //  Check to see if any button is currently pressed on the joystick
        public bool IsAnyJoystickButtonPressed()
        {
            if (m_joystickConnected)
            {
                foreach (bool button in m_actualJoystickState.Buttons)
                {
                    if (button)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public DeviceType ? GetJoystickType(){
            return m_joystickType;
        }

        //True if any SHIFT key is pressed
        public bool IsAnyShiftKeyPressed()
        {
            return IsKeyPress(Keys.LeftShift) || IsKeyPress(Keys.RightShift);
        }

        //True if any ALT key is pressed
        public bool IsAnyAltKeyPressed()
        {
            return IsKeyPress(Keys.LeftAlt) || IsKeyPress(Keys.RightAlt);
        }

        //True if any ALT key was pressed
        public bool WasAnyAltKeyPressed()
        {
            return WasKeyPressed(Keys.LeftAlt) || WasKeyPressed(Keys.RightAlt);
        }

        //True if any CTRL key is pressed
        public bool IsAnyCtrlKeyPressed()
        {
            return IsKeyPress(Keys.LeftControl) || IsKeyPress(Keys.RightControl);
        }

        //  Gets an array of values that correspond to the keyboard keys that are currently
        //  being pressed. Reference page contains links to related code samples.
        public void GetPressedKeys(List<Toolkit.Input.Keys> keys)
        {
            m_keyboardState.GetActualPressedKeys(keys);
        }

        #region Key Button States

        //  Return true if new key pressed right now. Don't care if it was pressed in previous update too.
        public bool IsKeyPress(Keys key)
        {
            return m_keyboardState.IsKeyDown(key);
        }

        //  Return true if new key was pressed, that means this key was pressed now. During previous Update it wasn't pressed at all.
        public bool IsNewKeyPress(Keys key)
        {
            return (m_keyboardState.IsKeyDown(key) && m_keyboardState.IsPreviousKeyUp(key));
        }

        //  Return true if key was pressed in previous update and now it is not.
        public bool IsNewKeyReleased(Keys key)
        {
            return (m_keyboardState.IsKeyUp(key) && m_keyboardState.IsPreviousKeyDown(key));
        }

        public bool WasKeyPressed(Keys key)
        {
            return m_keyboardState.IsPreviousKeyDown(key);
        }

        public bool WasKeyReleased(Keys key)
        {
            return m_keyboardState.IsPreviousKeyUp(key);
        }

        public bool IsAnyControlPress()
        {
            return m_keyboardState.IsKeyDown(Keys.LeftControl) || m_keyboardState.IsKeyDown(Keys.RightControl);
        }

        public bool IsAnyShiftPress()
        {
            return m_keyboardState.IsKeyDown(Keys.LeftShift) || m_keyboardState.IsKeyDown(Keys.RightShift);
        }

        public bool IsAnyAltPress()
        {
            return m_keyboardState.IsKeyDown(Keys.LeftAlt) || m_keyboardState.IsKeyDown(Keys.RightAlt);
        }

        #endregion

        #region Left Mouse Button States

        //  True if LEFT mouse is pressed right now, but previous update wasn't pressed. So this is one-time press.
        public bool IsNewLeftMousePressed()
        {
            return (IsLeftMousePressed() && WasLeftMouseReleased());
        }

        //  True if LEFT mouse is double clicked. This is once per update uvent.
        public bool IsNewLeftMouseDoubleClick()
        {
            return m_leftButtonDoubleClick;
        }

        //  True if LEFT mouse is released right now, but previous update wasn't pressed. So this is one-time release.
        public bool IsNewLeftMouseReleased()
        {
            return (IsLeftMouseReleased() && WasLeftMousePressed());
        }

        //  True if LEFT mouse is pressed right now. Don't care if it was pressed in previous update too.
        public bool IsLeftMousePressed()
        {
            return (m_actualMouseState.LeftButton);
        }

        //  True if LEFT mouse is released (not pressed) right now. Don't care if it was pressed/released in previous update too.
        public bool IsLeftMouseReleased()
        {
            return (m_actualMouseState.LeftButton == false);
        }

        public bool WasLeftMouseReleased()
        {
            return (m_previousMouseState.LeftButton == false);
        }

        //  True if LEFT mouse was pressed in previous update.
        public bool WasLeftMousePressed()
        {
            return (m_previousMouseState.LeftButton);
        }

        #endregion

        #region Right Mouse Button states

        //  True if RIGHT mouse is pressed right now. Don't care if it was pressed in previous update too.
        public bool IsRightMousePressed()
        {
            return (m_actualMouseState.RightButton);
        }

        //  True if RIGHT mouse is released (not pressed) right now. Don't care if it was pressed/released in previous update too.
        public bool IsRightMouseReleased()
        {
            return (m_actualMouseState.RightButton == false);
        }

        //  True if RIGHT mouse is pressed right now, but previous update wasn't pressed. So this is one-time press.
        public bool IsNewRightMousePressed()
        {
            return ((m_actualMouseState.RightButton) && (m_previousMouseState.RightButton == false));
        }

        //  True if RIGHT mouse is released right now, but previous update wasn't pressed. So this is one-time release.
        public bool IsNewRightMouseReleased()
        {
            return ((m_actualMouseState.RightButton == false) && (m_previousMouseState.RightButton));
        }

        public bool WasRightMousePressed()
        {
            return (m_previousMouseState.RightButton);
        }

        public bool WasRightMouseReleased()
        {
            return (m_previousMouseState.RightButton == false);
        }

        #endregion

        #region Middle Mouse Button States

        //  True if MIDDLE mouse is pressed right now. Don't care if it was pressed in previous update too.
        public bool IsMiddleMousePressed()
        {
            return (m_actualMouseState.MiddleButton);
        }

        //  True if MIDDLE mouse is released (not pressed) right now. Don't care if it was pressed/released in previous update too.
        public bool IsMiddleMouseReleased()
        {
            return (m_actualMouseState.MiddleButton == false);
        }

        //  True if MIDDLE mouse is pressed right now, but previous update wasn't pressed. So this is one-time press.
        public bool IsNewMiddleMousePressed()
        {
            return ((m_actualMouseState.MiddleButton) && (m_previousMouseState.MiddleButton == false));
        }

        //  True if MIDDLE mouse is pressed right now, but previous update wasn't pressed. So this is one-time press.
        public bool IsNewMiddleMouseReleased()
        {
            return ((m_actualMouseState.MiddleButton == false) && (m_previousMouseState.MiddleButton));
        }

        public bool WasMiddleMousePressed()
        {
            return (m_previousMouseState.MiddleButton);
        }

        public bool WasMiddleMouseReleased()
        {
            return (m_previousMouseState.MiddleButton == false);
        }

        #endregion

        #region XButton1 Mouse Button States

        //  True if XButton1 mouse is pressed right now. Don't care if it was pressed in previous update too.
        public bool IsXButton1MousePressed()
        {
            return (m_actualMouseState.XButton1);
        }

        //  True if XButton1 mouse is released (not pressed) right now. Don't care if it was pressed/released in previous update too.
        public bool IsXButton1MouseReleased()
        {
            return (m_actualMouseState.XButton1 == false);
        }

        //  True if XButton1 mouse is pressed right now, but previous update wasn't pressed. So this is one-time press.
        public bool IsNewXButton1MousePressed()
        {
            return ((m_actualMouseState.XButton1) && (m_previousMouseState.XButton1 == false));
        }

        public bool IsNewXButton1MouseReleased()
        {
            return ((m_actualMouseState.XButton1 == false) && (m_previousMouseState.XButton1));
        }

        public bool WasXButton1MousePressed()
        {
            return (m_previousMouseState.XButton1);
        }

        public bool WasNewXButton1MouseReleased()
        {
            return (m_previousMouseState.XButton1 == false);
        }

        #endregion

        #region XButton2 Mouse Button States

        //  True if XButton2 mouse is pressed right now. Don't care if it was pressed in previous update too.
        public bool IsXButton2MousePressed()
        {
            return (m_actualMouseState.XButton2);
        }

        //  True if XButton2 mouse is released (not pressed) right now. Don't care if it was pressed/released in previous update too.
        public bool IsXButton2MouseReleased()
        {
            return (m_actualMouseState.XButton2 == false);
        }

        //  True if XButton2 mouse is pressed right now, but previous update wasn't pressed. So this is one-time press.
        public bool IsNewXButton2MousePressed()
        {
            return ((m_actualMouseState.XButton2) && (m_previousMouseState.XButton2 == false));
        }

        public bool IsNewXButton2MouseReleased()
        {
            return ((m_actualMouseState.XButton2 == false) && (m_previousMouseState.XButton2));
        }

        public bool WasXButton2MousePressed()
        {
            return (m_previousMouseState.XButton2);
        }

        public bool WasNewXButton2MouseReleased()
        {
            return (m_previousMouseState.XButton2 == false);
        }

        #endregion

        #region Joystick button States

        //  Check to see if a specific button on the joystick is pressed.
        public bool IsJoystickButtonPressed(MyJoystickButtonsEnum button)
        {
            bool isPressed = false;
            if (m_joystickConnected && button != MyJoystickButtonsEnum.None && m_actualJoystickState != null)
            {
                switch (button)
                {
                    case MyJoystickButtonsEnum.JDLeft: isPressed = IsGamepadKeyLeftPressed(); break;
                    case MyJoystickButtonsEnum.JDRight: isPressed = IsGamepadKeyRightPressed(); break;
                    case MyJoystickButtonsEnum.JDUp: isPressed = IsGamepadKeyUpPressed(); break;
                    case MyJoystickButtonsEnum.JDDown: isPressed = IsGamepadKeyDownPressed(); break;
                    default: isPressed = (m_actualJoystickState.Buttons[(int)button - 5]); break;
                }
            }
            if (!isPressed && button == MyJoystickButtonsEnum.None)
            {
                return true;
            }
            return isPressed;
        }

        //  Check to see if a specific button on the joystick is currently pressed and was not pressed during the last update. 
        public bool IsJoystickButtonNewPressed(MyJoystickButtonsEnum button)
        {
            bool isNewPressed = false;
            if (m_joystickConnected && button != MyJoystickButtonsEnum.None && m_actualJoystickState != null && m_previousJoystickState != null)
            {
                switch (button)
                {
                    case MyJoystickButtonsEnum.JDLeft: isNewPressed = IsNewGamepadKeyLeftPressed(); break;
                    case MyJoystickButtonsEnum.JDRight: isNewPressed = IsNewGamepadKeyRightPressed(); break;
                    case MyJoystickButtonsEnum.JDUp: isNewPressed = IsNewGamepadKeyUpPressed(); break;
                    case MyJoystickButtonsEnum.JDDown: isNewPressed = IsNewGamepadKeyDownPressed(); break;
                    default: isNewPressed = ((m_actualJoystickState.IsPressed((int)button - 5)) && (m_previousJoystickState.IsPressed((int)button - 5) == false)); break;
                }
            }
            if (!isNewPressed && button == MyJoystickButtonsEnum.None)
            {
                return true;
            }
            return isNewPressed;
        }

        public bool IsNewJoystickButtonReleased(MyJoystickButtonsEnum button)
        {
            bool isReleased = false;
            if (m_joystickConnected && button != MyJoystickButtonsEnum.None && m_actualJoystickState != null && m_previousJoystickState != null)
            {
                switch (button)
                {
                    case MyJoystickButtonsEnum.JDLeft: isReleased = IsNewGamepadKeyLeftReleased(); break;
                    case MyJoystickButtonsEnum.JDRight: isReleased = IsNewGamepadKeyRightReleased(); break;
                    case MyJoystickButtonsEnum.JDUp: isReleased = IsNewGamepadKeyUpReleased(); break;
                    case MyJoystickButtonsEnum.JDDown: isReleased = IsNewGamepadKeyDownReleased(); break;
                    default: isReleased = ((m_actualJoystickState.IsReleased((int)button - 5)) && (m_previousJoystickState.IsPressed((int)button - 5))); break;
                }
            }
            if (!isReleased && button == MyJoystickButtonsEnum.None)
            {
                return true;
            }
            return isReleased;
        }

        public bool IsJoystickButtonReleased(MyJoystickButtonsEnum button)
        {
            bool isReleased = false;
            if (m_joystickConnected && button != MyJoystickButtonsEnum.None && m_actualJoystickState != null)
            {
                switch (button)
                {
                    case MyJoystickButtonsEnum.JDLeft: isReleased = !IsGamepadKeyLeftPressed(); break;
                    case MyJoystickButtonsEnum.JDRight: isReleased = !IsGamepadKeyRightPressed(); break;
                    case MyJoystickButtonsEnum.JDUp: isReleased = !IsGamepadKeyUpPressed(); break;
                    case MyJoystickButtonsEnum.JDDown: isReleased = !IsGamepadKeyDownPressed(); break;
                    default: isReleased = m_actualJoystickState.IsReleased((int)button - 5); break;
                }
            }
            if (!isReleased && button == MyJoystickButtonsEnum.None)
            {
                return true;
            }
            return isReleased;
        }

        //  Check to see if a specific button on the joystick was pressed.
        public bool WasJoystickButtonPressed(MyJoystickButtonsEnum button)
        {
            bool wasPressed = false;
            if (m_joystickConnected && button != MyJoystickButtonsEnum.None && m_previousJoystickState != null)
            {
                switch (button)
                {
                    case MyJoystickButtonsEnum.JDLeft: wasPressed = WasGamepadKeyLeftPressed(); break;
                    case MyJoystickButtonsEnum.JDRight: wasPressed = WasGamepadKeyRightPressed(); break;
                    case MyJoystickButtonsEnum.JDUp: wasPressed = WasGamepadKeyUpPressed(); break;
                    case MyJoystickButtonsEnum.JDDown: wasPressed = WasGamepadKeyDownPressed(); break;
                    default: wasPressed = (m_previousJoystickState.Buttons[(int)button - 5]); break;
                }
            }
            if (!wasPressed && button == MyJoystickButtonsEnum.None)
            {
                return true;
            }
            return wasPressed;
        }

        //  Check to see if a specific button on the joystick was released.
        public bool WasJoystickButtonReleased(MyJoystickButtonsEnum button)
        {
            bool wasReleased = false;
            if (m_joystickConnected && button != MyJoystickButtonsEnum.None && m_previousJoystickState != null)
            {
                switch (button)
                {
                    case MyJoystickButtonsEnum.JDLeft: wasReleased = !WasGamepadKeyLeftPressed(); break;
                    case MyJoystickButtonsEnum.JDRight: wasReleased = !WasGamepadKeyRightPressed(); break;
                    case MyJoystickButtonsEnum.JDUp: wasReleased = !WasGamepadKeyUpPressed(); break;
                    case MyJoystickButtonsEnum.JDDown: wasReleased = !WasGamepadKeyDownPressed(); break;
                    default: wasReleased = (m_previousJoystickState.IsReleased((int)button - 5)); break;
                }
            }
            if (!wasReleased && button == MyJoystickButtonsEnum.None)
            {
                return true;
            }
            return wasReleased;
        }

        #endregion

        #region Joystick axis States

        //  Find out how much a specific joystick axis is pressed.
        //  Return a raw number between 0 and 65535. 32768 is the middle value.
        public float GetJoystickAxisStateRaw(MyJoystickAxesEnum axis)
        {
            int value = 32768;
            if (m_joystickConnected && axis != MyJoystickAxesEnum.None && m_actualJoystickState != null && IsJoystickAxisSupported(axis))
            {
                switch (axis)
                {
                    case MyJoystickAxesEnum.RotationXpos: case MyJoystickAxesEnum.RotationXneg: value = m_actualJoystickState.RotationX; break;
                    case MyJoystickAxesEnum.RotationYpos: case MyJoystickAxesEnum.RotationYneg: value = m_actualJoystickState.RotationY; break;
                    case MyJoystickAxesEnum.RotationZpos: case MyJoystickAxesEnum.RotationZneg: value = m_actualJoystickState.RotationZ; break;
                    case MyJoystickAxesEnum.Xpos: case MyJoystickAxesEnum.Xneg: value = m_actualJoystickState.X; break;
                    case MyJoystickAxesEnum.Ypos: case MyJoystickAxesEnum.Yneg: value = m_actualJoystickState.Y; break;
                    case MyJoystickAxesEnum.Zpos: case MyJoystickAxesEnum.Zneg: value = m_actualJoystickState.Z; break;
                    case MyJoystickAxesEnum.Slider1pos: case MyJoystickAxesEnum.Slider1neg:
                        {
                            var array = m_actualJoystickState.Sliders;
                            value = (array.Length < 1) ? 32768 : array[0];
                        }
                        break;
                    case MyJoystickAxesEnum.Slider2pos: case MyJoystickAxesEnum.Slider2neg:
                        {
                            var array = m_actualJoystickState.Sliders;
                            value = (array.Length < 2) ? 32768 : array[1];
                        }
                        break;
                }
            }
            return value;
        }

        //  Find out how much a specific joystick axis was pressed.
        //  Return a raw number between 0 and 65535. 32768 is the middle value.
        public float GetPreviousJoystickAxisStateRaw(MyJoystickAxesEnum axis)
        {
            int value = 32768;
            if (m_joystickConnected && axis != MyJoystickAxesEnum.None && m_previousJoystickState != null && IsJoystickAxisSupported(axis))
            {
                switch (axis)
                {
                    case MyJoystickAxesEnum.RotationXpos: case MyJoystickAxesEnum.RotationXneg: value = m_previousJoystickState.RotationX; break;
                    case MyJoystickAxesEnum.RotationYpos: case MyJoystickAxesEnum.RotationYneg: value = m_previousJoystickState.RotationY; break;
                    case MyJoystickAxesEnum.RotationZpos: case MyJoystickAxesEnum.RotationZneg: value = m_previousJoystickState.RotationZ; break;
                    case MyJoystickAxesEnum.Xpos: case MyJoystickAxesEnum.Xneg: value = m_previousJoystickState.X; break;
                    case MyJoystickAxesEnum.Ypos: case MyJoystickAxesEnum.Yneg: value = m_previousJoystickState.Y; break;
                    case MyJoystickAxesEnum.Zpos: case MyJoystickAxesEnum.Zneg: value = m_previousJoystickState.Z; break;
                    case MyJoystickAxesEnum.Slider1pos: case MyJoystickAxesEnum.Slider1neg:
                        {
                            var array = m_previousJoystickState.Sliders;
                            value = (array.Length < 1) ? 32768 : array[0];
                        }
                        break;
                    case MyJoystickAxesEnum.Slider2pos: case MyJoystickAxesEnum.Slider2neg:
                        {
                            var array = m_previousJoystickState.Sliders;
                            value = (array.Length < 2) ? 32768 : array[1];
                        }
                        break;
                }
            }
            return value;
        }


        public float GetJoystickX()
        {
            return GetJoystickAxisStateRaw(MyJoystickAxesEnum.Xpos);
        }

        public float GetJoystickY()
        {
            return GetJoystickAxisStateRaw(MyJoystickAxesEnum.Ypos);
        }


        //  Find out how much a specific joystick half-axis is pressed.
        //  Return a number between 0 and 1 (taking deadzone, sensitivity and non-linearity into account).
        public float GetJoystickAxisStateForGameplay(MyJoystickAxesEnum axis)
        {
            if (m_joystickConnected && IsJoystickAxisSupported(axis))
            {
                // Input position scaled to (-1..1).
                float position = ((float)GetJoystickAxisStateRaw(axis) - (float)MyJoystickConstants.CENTER_AXIS) / (float)MyJoystickConstants.CENTER_AXIS;

                switch (axis)
                {
                    case MyJoystickAxesEnum.RotationXneg: case MyJoystickAxesEnum.Xneg:
                    case MyJoystickAxesEnum.RotationYneg: case MyJoystickAxesEnum.Yneg:
                    case MyJoystickAxesEnum.RotationZneg: case MyJoystickAxesEnum.Zneg:
                    case MyJoystickAxesEnum.Slider1neg: case MyJoystickAxesEnum.Slider2neg:
                        if (position >= 0) return 0;
                        break;
                    case MyJoystickAxesEnum.RotationXpos: case MyJoystickAxesEnum.Xpos:
                    case MyJoystickAxesEnum.RotationYpos: case MyJoystickAxesEnum.Ypos:
                    case MyJoystickAxesEnum.RotationZpos: case MyJoystickAxesEnum.Zpos:
                    case MyJoystickAxesEnum.Slider1pos: case MyJoystickAxesEnum.Slider2pos:
                        if (position <= 0) return 0;
                        break;
                    default:
                        MyCommonDebugUtils.AssertDebug(false, "Unknown joystick axis!");
                        break;
                }

                float distance = Math.Abs(position);
                if (distance > m_joystickDeadzone)
                {
                    distance = (distance - m_joystickDeadzone) / (1 - m_joystickDeadzone);  // Rescale distance to (0..1) outside the deadzone.
                    return m_joystickSensitivity * (float)Math.Pow(distance, m_joystickExponent);
                }
            }

            return 0;
        }

        //  Find out how much a specific joystick half-axis is pressed.
        //  Return a number between 0 and 100 (taking deadzone, sensitivity and non-linearity into account).
        public float GetPreviousJoystickAxisStateForGameplay(MyJoystickAxesEnum axis)
        {
            if (m_joystickConnected && IsJoystickAxisSupported(axis))
            {
                // Input position scaled to (-1..1).
                float position = ((float)GetPreviousJoystickAxisStateRaw(axis) - (float)MyJoystickConstants.CENTER_AXIS) / (float)MyJoystickConstants.CENTER_AXIS;

                switch (axis)
                {
                    case MyJoystickAxesEnum.RotationXneg: case MyJoystickAxesEnum.Xneg:
                    case MyJoystickAxesEnum.RotationYneg: case MyJoystickAxesEnum.Yneg:
                    case MyJoystickAxesEnum.RotationZneg: case MyJoystickAxesEnum.Zneg:
                    case MyJoystickAxesEnum.Slider1neg: case MyJoystickAxesEnum.Slider2neg:
                        if (position >= 0) return 0;
                        break;
                    case MyJoystickAxesEnum.RotationXpos: case MyJoystickAxesEnum.Xpos:
                    case MyJoystickAxesEnum.RotationYpos: case MyJoystickAxesEnum.Ypos:
                    case MyJoystickAxesEnum.RotationZpos: case MyJoystickAxesEnum.Zpos:
                    case MyJoystickAxesEnum.Slider1pos: case MyJoystickAxesEnum.Slider2pos:
                        if (position <= 0) return 0;
                        break;
                    default:
                        MyCommonDebugUtils.AssertDebug(false, "Unknown joystick axis!");
                        break;
                }

                float distance = Math.Abs(position);
                if (distance > m_joystickDeadzone)
                {
                    distance = (distance - m_joystickDeadzone) / (1 - m_joystickDeadzone);  // Rescale distance to (0..1) outside the deadzone.
                    return 100 * m_joystickSensitivity * (float)Math.Pow(distance, m_joystickExponent);
                }
            }

            return 0;
        }

        #region Joystick analog axes used for digital controls

        public bool IsJoystickAxisPressed(MyJoystickAxesEnum axis)
        {
            bool isPressed = false;
            if (m_joystickConnected && axis != MyJoystickAxesEnum.None && m_actualJoystickState != null)
            {
                isPressed = GetJoystickAxisStateForGameplay(axis) > MyJoystickConstants.ANALOG_PRESSED_THRESHOLD;
            }
            if (!isPressed && axis == MyJoystickAxesEnum.None)
            {
                return true;
            }
            if (!IsJoystickAxisSupported(axis)) return false;
            return isPressed;
        }

        //  Check to see if a specific button on the joystick is currently pressed and was not pressed during the last update. 
        public bool IsJoystickAxisNewPressed(MyJoystickAxesEnum axis)
        {
            bool isNewPressed = false;
            if (m_joystickConnected && axis != MyJoystickAxesEnum.None && m_actualJoystickState != null && m_previousJoystickState != null)
            {
                isNewPressed = GetJoystickAxisStateForGameplay(axis) > MyJoystickConstants.ANALOG_PRESSED_THRESHOLD && GetPreviousJoystickAxisStateForGameplay(axis) <= MyJoystickConstants.ANALOG_PRESSED_THRESHOLD;
            }
            if (!isNewPressed && axis == MyJoystickAxesEnum.None)
            {
                return true;
            }
            if (!IsJoystickAxisSupported(axis)) return false;
            return isNewPressed;
        }

        public bool IsNewJoystickAxisReleased(MyJoystickAxesEnum axis)
        {
            bool isNewPressed = false;
            if (m_joystickConnected && axis != MyJoystickAxesEnum.None && m_actualJoystickState != null && m_previousJoystickState != null)
            {
                isNewPressed = GetJoystickAxisStateForGameplay(axis) <= MyJoystickConstants.ANALOG_PRESSED_THRESHOLD && GetPreviousJoystickAxisStateForGameplay(axis) > MyJoystickConstants.ANALOG_PRESSED_THRESHOLD;
            }
            if (!isNewPressed && axis == MyJoystickAxesEnum.None)
            {
                return true;
            }
            if (!IsJoystickAxisSupported(axis)) return false;
            return isNewPressed;
        }

        public bool IsJoystickAxisReleased(MyJoystickAxesEnum axis)
        {
            bool isPressed = false;
            if (m_joystickConnected && axis != MyJoystickAxesEnum.None && m_actualJoystickState != null)
            {
                isPressed = GetJoystickAxisStateForGameplay(axis) <= MyJoystickConstants.ANALOG_PRESSED_THRESHOLD;
            }
            if (!isPressed && axis == MyJoystickAxesEnum.None)
            {
                return true;
            }
            if (!IsJoystickAxisSupported(axis)) return false;
            return isPressed;
        }

        //  Check to see if a specific button on the joystick was pressed.
        public bool WasJoystickAxisPressed(MyJoystickAxesEnum axis)
        {
            bool isPressed = false;
            if (m_joystickConnected && axis != MyJoystickAxesEnum.None && m_previousJoystickState != null)
            {
                isPressed = GetPreviousJoystickAxisStateForGameplay(axis) > MyJoystickConstants.ANALOG_PRESSED_THRESHOLD;
            }
            if (!isPressed && axis == MyJoystickAxesEnum.None)
            {
                return true;
            }
            if (!IsJoystickAxisSupported(axis)) return false;
            return isPressed;
        }

        //  Check to see if a specific button on the joystick was released.
        public bool WasJoystickAxisReleased(MyJoystickAxesEnum axis)
        {
            bool isPressed = false;
            if (m_joystickConnected && axis != MyJoystickAxesEnum.None && m_previousJoystickState != null)
            {
                isPressed = GetPreviousJoystickAxisStateForGameplay(axis) <= MyJoystickConstants.ANALOG_PRESSED_THRESHOLD;
            }
            if (!isPressed && axis == MyJoystickAxesEnum.None)
            {
                return true;
            }
            if (!IsJoystickAxisSupported(axis)) return false;
            return isPressed;
        }
        
        #endregion

        #region Joystick settings

        public float GetJoystickSensitivity()
        {
            return m_joystickSensitivity;
        }

        public void SetJoystickSensitivity(float newSensitivity)
        {
            m_joystickSensitivity = newSensitivity;
        }

        public float GetJoystickExponent()
        {
            return m_joystickExponent;
        }

        public void SetJoystickExponent(float newExponent)
        {
            m_joystickExponent = newExponent;
        }

        public float GetJoystickDeadzone()
        {
            return m_joystickDeadzone;
        }

        public void SetJoystickDeadzone(float newDeadzone)
        {
            m_joystickDeadzone = newDeadzone;
        }

        #endregion

        #endregion

        //  Current mouse scrollwheel value.
        public int MouseScrollWheelValue()
        {
            return m_actualMouseState.ScrollWheelValue;
        }

        //  Previous mouse scrollwheel value.
        public int PreviousMouseScrollWheelValue()
        {
            return m_previousMouseState.ScrollWheelValue;
        }

        //  Delta mouse scrollwheel value.
        public int DeltaMouseScrollWheelValue()
        {
            return MouseScrollWheelValue() - PreviousMouseScrollWheelValue();
        }

        //  Return actual mouse X position - for drawing cursor
        public int GetMouseX()
        {
            return m_actualMouseState.X;
        }

        //  Return actual mouse Y position - for drawing cursor
        public int GetMouseY()
        {
            return m_actualMouseState.Y;
        }

        public int GetPreviousMouseX()
        {
            return m_previousMouseState.X;
        }

        public int GetPreviousMouseY()
        {
            return m_previousMouseState.Y;
        }

        //  Return actual mouse X position - for gameplay
        public int GetMouseXForGamePlay()
        {
            int inv = m_mouseXIsInverted ? -1 : 1;
            return (int)(MyMinerGame.ScreenSizeHalf.X + (m_mouseSensitivity * (inv * (m_actualMouseState.X - MyMinerGame.ScreenSizeHalf.X))));
        }

        //  Return actual mouse Y position - for gameplay
        public int GetMouseYForGamePlay()
        {
            int inv = m_mouseYIsInverted ? -1 : 1;
            return (int)(MyMinerGame.ScreenSizeHalf.Y + (m_mouseSensitivity * (inv * (m_actualMouseState.Y - MyMinerGame.ScreenSizeHalf.Y))));
        }

        public bool GetMouseXInversion()
        {
            return m_mouseXIsInverted;
        }

        public bool GetMouseYInversion()
        {
            return m_mouseYIsInverted;
        }

        public void SetMouseXInversion(bool inverted)
        {
            m_mouseXIsInverted = inverted;
        }

        public void SetMouseYInversion(bool inverted)
        {
            m_mouseYIsInverted = inverted;
        }

        public float GetMouseSensitivity()
        {
            return m_mouseSensitivity;
        }

        public void SetMouseSensitivity(float sensitivity)
        {
            m_mouseSensitivity = sensitivity;
        }

        /// <summary>
        /// Returns immediatelly current cursor position.
        /// Obtains position on every call, it can get cursor data with higher rate than 60 fps
        /// </summary>
        public static Vector2 GetMousePosition()
        {
            int x, y;
            MyWindowsMouse.GetPosition(out x, out y);
            return new Vector2(x, y);
        }

        public static void SetMousePosition(int x, int y)
        {
            MyWindowsMouse.SetPosition(x, y);
        }

        //  Re-sets the mouse to the center position
        public static void SetMouseToScreenCenter()
        {
            if (MyVideoModeManager.IsHardwareCursorUsed() && ( MyMinerGame.Static.IsMouseVisible || MyMinerGame.Static.Window.NativeWindow.Handle != GetForegroundWindow() ))
                return;
            //KeenSoftwareHouse.Library.Trace.Trace.SendMsgAllCall("center");
            SetMousePosition(MyMinerGame.ScreenSizeHalf.X, MyMinerGame.ScreenSizeHalf.Y);
        }

        // Checks to see if the joystick is connected. This is used so we don't try to poll a joystick that doesn't exist.
        public bool IsJoystickConnected()
        {
            return m_joystickConnected;
        }
        
        //  POVDirection()
        //
        //  @return int[] of size NumJoystickPOVs(). Will contain the raw int
        //  representing the directing the POV is currently pointing. Use the public 
        //  consts below to determine direction:
        //  POV_NEUTRAL 
        //  POV_NORTH 
        //  POV_NORTHEAST
        //  POV_EAST
        //  POV_SOUTHEAST
        //  POV_SOUTH
        //  POV_SOUTHWEST
        //  POV_WEST
        //  POV_NORTHWEST
        //
        //  If no joystick is present return null
        public int[] POVDirection()
        {
            if (m_joystickConnected)
            {
                return m_actualJoystickState.PointOfViewControllers;
            }
            return null;
        }

        /// <summary>
        /// Get the actual and previous gamepad key directions (use the first POV controller).
        /// Returns false if this type of input is not available.
        /// </summary>
        public bool GetGamepadKeyDirections(out int actual, out int previous)
        {
            if (m_joystickConnected && m_actualJoystickState != null && m_previousJoystickState != null)
            {
                int[] actualPOVControllers = m_actualJoystickState.PointOfViewControllers;
                int[] previousPOVControllers = m_previousJoystickState.PointOfViewControllers;
                //Trace.SendMsgLastCall(actualPOVControllers[0].ToString() + previousPOVControllers[0].ToString());

                if (actualPOVControllers != null && previousPOVControllers != null)
                {
                    actual = actualPOVControllers[0];
                    previous = previousPOVControllers[0];
                    return true;
                }

            }
            actual = -1;
            previous = -1;
            return false;
        }

        public bool IsGamepadKeyRightPressed()
        {
            int actual, previous;
            if (GetGamepadKeyDirections(out actual, out previous)) return (actual >= 4500 && actual <= 13500);
            return false;
        }

        public bool IsGamepadKeyLeftPressed()
        {
            int actual, previous;
            if (GetGamepadKeyDirections(out actual, out previous)) return (actual >= 22500 && actual <= 31500);
            return false;
        }

        public bool IsGamepadKeyDownPressed()
        {
            int actual, previous;
            if (GetGamepadKeyDirections(out actual, out previous)) return (actual >= 13500 && actual <= 22500);
            return false;
        }

        public bool IsGamepadKeyUpPressed()
        {
            int actual, previous;
            if (GetGamepadKeyDirections(out actual, out previous)) return (actual >= 0 && actual <= 4500) || (actual >= 31500 && actual <= 36000);
            return false;
        }

        
        public bool WasGamepadKeyRightPressed()
        {
            int actual, previous;
            if (GetGamepadKeyDirections(out actual, out previous)) return (previous >= 4500 && previous <= 13500);
            return false;
        }

        public bool WasGamepadKeyLeftPressed()
        {
            int actual, previous;
            if (GetGamepadKeyDirections(out actual, out previous)) return (previous >= 22500 && previous <= 31500);
            return false;
        }

        public bool WasGamepadKeyDownPressed()
        {
            int actual, previous;
            if (GetGamepadKeyDirections(out actual, out previous)) return (previous >= 13500 && previous <= 22500);
            return false;
        }

        public bool WasGamepadKeyUpPressed()
        {
            int actual, previous;
            if (GetGamepadKeyDirections(out actual, out previous)) return (previous >= 0 && previous <= 4500) || (previous >= 31500 && previous <= 36000);
            return false;
        }


        public bool IsNewGamepadKeyRightPressed() { return !WasGamepadKeyRightPressed() && IsGamepadKeyRightPressed(); }
        public bool IsNewGamepadKeyLeftPressed()  { return !WasGamepadKeyLeftPressed()  && IsGamepadKeyLeftPressed(); }
        public bool IsNewGamepadKeyDownPressed()  { return !WasGamepadKeyDownPressed()  && IsGamepadKeyDownPressed(); }
        public bool IsNewGamepadKeyUpPressed()    { return !WasGamepadKeyUpPressed()    && IsGamepadKeyUpPressed(); }

        public bool IsNewGamepadKeyRightReleased() { return WasGamepadKeyRightPressed() && !IsGamepadKeyRightPressed(); }
        public bool IsNewGamepadKeyLeftReleased()  { return WasGamepadKeyLeftPressed()  && !IsGamepadKeyLeftPressed(); }
        public bool IsNewGamepadKeyDownReleased()  { return WasGamepadKeyDownPressed()  && !IsGamepadKeyDownPressed(); }
        public bool IsNewGamepadKeyUpReleased()    { return WasGamepadKeyUpPressed()    && !IsGamepadKeyUpPressed(); }

        public JoystickState GetActualJoystickState()
        {
            return m_actualJoystickState;
        }

        public JoystickState GetPreviousJoystickState()
        {
            return m_previousJoystickState;
        }

        public bool IsJoystickAxisSupported(MyJoystickAxesEnum axis)
        {
            if (!m_joystickConnected) return false;
            switch (axis)
            {
                case MyJoystickAxesEnum.Xpos: case MyJoystickAxesEnum.Xneg: return m_joystickXAxisSupported;
                case MyJoystickAxesEnum.Ypos: case MyJoystickAxesEnum.Yneg: return m_joystickYAxisSupported;
                case MyJoystickAxesEnum.Zpos: case MyJoystickAxesEnum.Zneg: return m_joystickZAxisSupported;
                case MyJoystickAxesEnum.RotationXpos: case MyJoystickAxesEnum.RotationXneg: return m_joystickRotationXAxisSupported;
                case MyJoystickAxesEnum.RotationYpos: case MyJoystickAxesEnum.RotationYneg: return m_joystickRotationYAxisSupported;
                case MyJoystickAxesEnum.RotationZpos: case MyJoystickAxesEnum.RotationZneg: return m_joystickRotationZAxisSupported;
                case MyJoystickAxesEnum.Slider1pos: case MyJoystickAxesEnum.Slider1neg: return m_joystickSlider1AxisSupported;
                case MyJoystickAxesEnum.Slider2pos: case MyJoystickAxesEnum.Slider2neg: return m_joystickSlider2AxisSupported;
                default: return false;
            }
        }

        //  Check if an assigned control for game is new pressed.
        public bool IsNewGameControlPressed(MyGameControlEnums control)
        {
            //  If you are trying to set a control that does not exist do nothing.
            if ((int)control > m_gameControlsList.Length)
            {
                return false;
            }
            if (m_gameControlsList[(int)control] == null)
            {
                return false;
            }

            return m_gameControlsList[(int)control].IsNewPressed();
        }


        //  Check if an assigned control for game is currently pressed.
        public bool IsGameControlPressed(MyGameControlEnums control)
        {
            //  If you are trying to set a control that does not exist do nothing.
            if ((int)control > m_gameControlsList.Length)
            {
                return false;
            }
            if (m_gameControlsList[(int)control] == null)
            {
                return false;
            }

            return m_gameControlsList[(int)control].IsPressed();
        }

        //  Check if an assigned control for editor is currently pressed.
        public bool IsEditorControlPressed(MyEditorControlEnums control)
        {
            //  If you are trying to set a control that does not exist do nothing.
            if ((int)control > m_editorControlsList.Length)
            {
                return false;
            }
            if (m_editorControlsList[(int)control] == null)
            {
                return false;
            }

            return m_editorControlsList[(int)control].IsPressed();
        }

        //  Check if an assigned control for game is currently pressed.
        public float GetGameControlAnalogState(MyGameControlEnums control)
        {
            //  If you are trying to set a control that does not exist do nothing.
            if ((int)control > m_gameControlsList.Length || m_gameControlsList[(int)control] == null)
            {
                return 0;
            }

            return m_gameControlsList[(int)control].GetAnalogState();
        }

        //  Check if an assigned control for editor is currently pressed.
        public float GetEditorControlAnalogState(MyEditorControlEnums control)
        {
            //  If you are trying to set a control that does not exist do nothing.
            if ((int)control > m_editorControlsList.Length || m_editorControlsList[(int)control] == null)
            {
                return 0;
            }

            return m_editorControlsList[(int)control].GetAnalogState();
        }


        //  Check if an assigned control is currently pressed and was not pressed during the last update.
        /*public bool IsGameControlNewPressed(MyGameControlEnums control)
        {
            //  If you are trying to set a control that does not exist do nothing.
            if ((int)control > m_gameControlsList.Length)
            {
                return false;
            }
            if (m_gameControlsList[(int)control] == null)
            {
                return false;
            }

            return m_gameControlsList[(int)control].IsNewPressed();
        }*/

        //  Check if an assigned control is currently pressed and was not pressed during the last update.
        public bool IsEditorControlNewPressed(MyEditorControlEnums control)
        {
            //  If you are trying to set a control that does not exist do nothing.
            if ((int)control > m_editorControlsList.Length)
            {
                return false;
            }
            if (m_editorControlsList[(int)control] == null)
            {
                return false;
            }

            return m_editorControlsList[(int)control].IsNewPressed();
        }

        //  Check is an assigned control was pressed in previous state
        public bool WasGameControlPressed(MyGameControlEnums control)
        {
            //  If you are trying to set a control that does not exist do nothing.
            if ((int)control > m_gameControlsList.Length)
            {
                return false;
            }
            if (m_gameControlsList[(int)control] == null)
            {
                return false;
            }

            return m_gameControlsList[(int)control].WasPressed();
        }

        //  Check is an assigned control was pressed in previous state
        public bool WasEditorControlPressed(MyEditorControlEnums control)
        {
            //  If you are trying to set a control that does not exist do nothing.
            if ((int)control > m_editorControlsList.Length)
            {
                return false;
            }
            if (m_editorControlsList[(int)control] == null)
            {
                return false;
            }

            return m_editorControlsList[(int)control].WasPressed();
        }

        //  Check is an assigned game control is released
        public bool IsGameControlReleased(MyGameControlEnums control)
        {
            //  If you are trying to set a control that does not exist do nothing.
            if ((int)control > m_gameControlsList.Length)
            {
                return false;
            }
            if (m_gameControlsList[(int)control] == null)
            {
                return false;
            }

            return m_gameControlsList[(int)control].IsNewReleased();
        }

        //  Check is an assigned editor control is new released
        public bool IsEditorControlNewReleased(MyEditorControlEnums control)
        {
            //  If you are trying to set a control that does not exist do nothing.
            if ((int)control > m_editorControlsList.Length)
            {
                return false;
            }
            if (m_editorControlsList[(int)control] == null)
            {
                return false;
            }

            return m_editorControlsList[(int)control].IsNewReleased();
        }

        //  Check is an assigned control was released in previous state
        public bool WasGameControlReleased(MyGameControlEnums control)
        {
            //  If you are trying to set a control that does not exist do nothing.
            if ((int)control > m_gameControlsList.Length)
            {
                return false;
            }
            if (m_gameControlsList[(int)control] == null)
            {
                return false;
            }

            return m_gameControlsList[(int)control].WasReleased();
        }

        //  Check is an assigned control was released in previous state
        public bool WasEditorControlReleased(MyEditorControlEnums control)
        {
            //  If you are trying to set a control that does not exist do nothing.
            if ((int)control > m_editorControlsList.Length)
            {
                return false;
            }
            if (m_editorControlsList[(int)control] == null)
            {
                return false;
            }

            return m_editorControlsList[(int)control].WasReleased();
        }

        //  Return true if key is valid for user controls
        public static bool IsKeyValid(Keys key)
        {
            foreach (var item in m_validKeyboardKeys)
            {
                if (item == key) return true;
            }

            return false;
        }

        public bool IsKeyDigit(Keys key)
        {
            return m_digitKeys.Contains(key);
        }

        //  Return true if mouse button is valid for user controls
        public static bool IsMouseButtonValid(MyMouseButtonsEnum button)
        {
            foreach (var item in m_validMouseButtons)
            {
                if (item == button) return true;
            }
            return false;
        }

        //  Return true if joystick button is valid for user controls
        public static bool IsJoystickButtonValid(MyJoystickButtonsEnum button)
        {
            foreach (var item in m_validJoystickButtons)
            {
                if (item == button) return true;
            }
            return false;
        }

        //  Return true if joystick axis is valid for user controls
        public static bool IsJoystickAxisValid(MyJoystickAxesEnum axis)
        {
            foreach (var item in m_validJoystickAxes)
            {
                if (item == axis) return true;
            }
            return false;
        }

        public MyControl[] GetControlsListByGameControlType(MyGuiGameControlType gameControlType)
        {
            MyControl[] controlsList = null;
            if (gameControlType == MyGuiGameControlType.GAME)
            {
                controlsList = m_gameControlsList;
            }
            else if (gameControlType == MyGuiGameControlType.EDITOR)
            {
                controlsList = m_editorControlsList;
            }

            return controlsList;
        }

        //  Return true if key is used by some user control
        public MyControl GetControl(Keys key, MyGuiGameControlType gameControlType)
        {
            MyControl[] controlsList = GetControlsListByGameControlType(gameControlType);
            foreach (var item in controlsList)
            {
                if (item.GetKeyboardControl() == key) return item;
            }
            return null;
        }

        //  Return true if mouse button is used by some user control
        public MyControl GetControl(MyMouseButtonsEnum button, MyGuiGameControlType gameControlType)
        {
            MyControl[] controlsList = GetControlsListByGameControlType(gameControlType);
            foreach (var item in controlsList)
            {
                if (item.GetMouseControl() == button) return item;
            }
            return null;
        }

        //  Return true if joystick button is used by some user control
        public MyControl GetControl(MyJoystickButtonsEnum button, MyGuiGameControlType gameControlType)
        {
            MyControl[] controlsList = GetControlsListByGameControlType(gameControlType);
            foreach (var item in m_gameControlsList)
            {
                if (item.GetJoystickControl() == button) return item;
            }
            return null;
        }

        //  Return true if joystick axis is used by an user control
        public MyControl GetControl(MyJoystickAxesEnum axis, MyGuiGameControlType gameControlType)
        {
            MyControl[] controlsList = GetControlsListByGameControlType(gameControlType);
            foreach (var item in m_gameControlsList)
            {
                if (item.GetJoystickAxisControl() == axis) return item;
            }
            return null;
        }

        public void GetListOfPressedKeys(List<Toolkit.Input.Keys> keys)
        {
            GetPressedKeys(keys);
        }

        public void GetListOfPressedMouseButtons(List<MyMouseButtonsEnum> result)
        {
            result.Clear();

            if (IsLeftMousePressed()) result.Add(MyMouseButtonsEnum.Left);
            if (IsRightMousePressed()) result.Add(MyMouseButtonsEnum.Right);
            if (IsMiddleMousePressed()) result.Add(MyMouseButtonsEnum.Middle);
            if (IsXButton1MousePressed()) result.Add(MyMouseButtonsEnum.XButton1);
            if (IsXButton2MousePressed()) result.Add(MyMouseButtonsEnum.XButton2);
        }

        public void GetListOfPressedJoystickButtons(List<MyJoystickButtonsEnum> result)
        {
            result.Clear();

            foreach (MyJoystickButtonsEnum item in MyControl.MyJoystickButtonsEnumValues)
            {
                if (item != MyJoystickButtonsEnum.None && IsJoystickButtonPressed(item)) result.Add(item);
            }
        }

        public void GetListOfPressedJoystickAxes(List<MyJoystickAxesEnum> result)
        {
            result.Clear();

            foreach (MyJoystickAxesEnum item in MyControl.MyJoystickButtonsEnumValues)
            {
                if (item != MyJoystickAxesEnum.None && IsJoystickAxisPressed(item)) result.Add(item);
            }
        }

        //  Returns an array MyControl that contains every assigned control for game.
        public MyControl[] GetGameControlsList()
        {
            return m_gameControlsList;
        }

        //  Returns an array MyControl that contains every assigned control for editor.
        public MyControl[] GetEditorControlsList()
        {
            return m_editorControlsList;
        }

        //  IMPORTANT! Use this function before attempting to assign new controls so that the controls can be re-set if the user does not like the changes.
        public void TakeSnapshot()
        {
            //m_controlsSnapshot.Clear();
            m_joystickInstanceNameSnapshot = JoystickInstanceName;
            m_gameControlsSnapshot = CloneControls(m_gameControlsList, m_gameControlsSnapshot);
            m_editorControlsSnapshot = CloneControls(m_editorControlsList, m_editorControlsSnapshot);
        }

        //  IMPORTANT! Only call this method after calling TakeSnapshot() to revert any changes made since TakeSnapshot() was last called. 
        public void RevertChanges()
        {
            //m_controlsList.Clear();
            JoystickInstanceName = m_joystickInstanceNameSnapshot;
            m_gameControlsList = CloneControls(m_gameControlsSnapshot, m_gameControlsList);
            m_editorControlsList = CloneControls(m_editorControlsSnapshot, m_editorControlsList);
        }

        //  Returns a string value of the button or key assigned to a control for game.
        public String GetGameControlTextEnum(MyGameControlEnums control)
        {
            //return m_controlsList[(int)control].GetControlButtonName();
            return m_gameControlsList[(int)control].GetControlButtonName(MyGuiInputDeviceEnum.Keyboard);
        }

        public MyControl GetGameControl(MyGameControlEnums control)
        {
            return m_gameControlsList[(int)control];
        }

        //  Returns a string value of the button or key assigned to a control for editor.
        public String GetEditorControlTextEnum(MyEditorControlEnums control)
        {
            //return m_controlsList[(int)control].GetControlButtonName();
            return m_editorControlsList[(int)control].GetControlButtonName(MyGuiInputDeviceEnum.Keyboard);
        }

        //  This is used to copy the list of controls into backup lists or for reverting changes to the list
        private static MyControl[] CloneControls(MyControl[] original, MyControl[] copy)
        {
            for (int i = 0; i < original.Length; i++)
            {   
                copy[i] = original[i].Clone();
            }
            return copy;
        }

        public void RevertToDefaultControls()
        {
            m_mouseXIsInverted = IsMouseXInvertedDefault;
            m_mouseYIsInverted = IsMouseYInvertedDefault;
            m_mouseSensitivity = MouseSensitivityDefault;
            
            m_joystickSensitivity = JoystickSensitivityDefault;
            m_joystickDeadzone = JoystickDeadzoneDefault;
            m_joystickExponent = JoystickExponentDefault;
            CloneControls(m_defaultGameControlsList, m_gameControlsList);
            CloneControls(m_defaultEditorControlsList, m_editorControlsList);
        }

        //  Save all controls to the Config File.
        public void SaveControls()
        {
            StringBuilder controlsGeneral = new StringBuilder();
            controlsGeneral.Append(m_mouseXIsInverted).Append(";");
            controlsGeneral.Append(m_mouseYIsInverted).Append(";");
            controlsGeneral.Append(m_mouseSensitivity.ToString(System.Globalization.CultureInfo.InvariantCulture)).Append(";");
            controlsGeneral.Append(m_joystickInstanceName == null ? "" : MyStringUtils.EscapeSemicolons(m_joystickInstanceName)).Append(";");
            controlsGeneral.Append(m_joystickSensitivity.ToString(System.Globalization.CultureInfo.InvariantCulture)).Append(";");
            controlsGeneral.Append(m_joystickExponent.ToString(System.Globalization.CultureInfo.InvariantCulture)).Append(";");
            controlsGeneral.Append(m_joystickDeadzone.ToString(System.Globalization.CultureInfo.InvariantCulture));

            MyConfig.ControlsGeneral = controlsGeneral.ToString();
            
            List<string> controlsButtons = new List<string>();
            //BEFORE - FIRE_PRIMARY:Mouse:Left;
            //AFTER - FIRE_PRIMARY:Keyboard:A:Mouse:Left:Joystick:J01:JoystickAxis:JY+
            foreach (MyControl control in m_gameControlsList)
            {
                string retVal = MyEnumsToStrings.GameControlEnums[(int)control.GetGameControlEnum()] + ":";
                retVal += MyEnumsToStrings.GuiInputDeviceEnum[(int)MyGuiInputDeviceEnum.Keyboard] + ":";
                retVal += control.GetKeyboardControl() + ":";
                retVal += MyEnumsToStrings.GuiInputDeviceEnum[(int)MyGuiInputDeviceEnum.Mouse] + ":";
                retVal += MyEnumsToStrings.MouseButtonsEnum[(int)control.GetMouseControl()] + ":";
                retVal += MyEnumsToStrings.GuiInputDeviceEnum[(int)MyGuiInputDeviceEnum.Joystick] + ":";
                retVal += MyEnumsToStrings.JoystickButtonsEnum[(int)control.GetJoystickControl()] + ":";
                retVal += MyEnumsToStrings.GuiInputDeviceEnum[(int)MyGuiInputDeviceEnum.JoystickAxis] + ":";
                retVal += MyEnumsToStrings.JoystickAxesEnum[(int)control.GetJoystickAxisControl()];

                controlsButtons.Add(retVal);
            }

            List<string> editorControlsButtons = new List<string>();
            //BEFORE - FIRE_PRIMARY:Mouse:Left;
            //AFTER - FIRE_PRIMARY:Keyboard:A:Mouse:Left:Joystick:J01
            foreach (MyControl control in m_editorControlsList)
            {
                string retVal = MyEnumsToStrings.EditorControlEnums[(int)control.GetEditorControlEnum()] + ":";
                retVal += MyEnumsToStrings.GuiInputDeviceEnum[(int)MyGuiInputDeviceEnum.Keyboard] + ":";
                retVal += control.GetKeyboardControl() + ":";
                retVal += MyEnumsToStrings.GuiInputDeviceEnum[(int)MyGuiInputDeviceEnum.Mouse] + ":";
                retVal += MyEnumsToStrings.MouseButtonsEnum[(int)control.GetMouseControl()] + ":";
                retVal += MyEnumsToStrings.GuiInputDeviceEnum[(int)MyGuiInputDeviceEnum.Joystick] + ":";
                retVal += MyEnumsToStrings.JoystickButtonsEnum[(int)control.GetJoystickControl()] + ":";
                retVal += MyEnumsToStrings.GuiInputDeviceEnum[(int)MyGuiInputDeviceEnum.JoystickAxis] + ":";
                retVal += MyEnumsToStrings.JoystickAxesEnum[(int)control.GetJoystickAxisControl()];

                editorControlsButtons.Add(retVal);
            }

            MyConfig.ControlsButtons = string.Join(";", controlsButtons.ToArray());
            MyConfig.EditorControlsButtons = string.Join(";", editorControlsButtons.ToArray());
            MyConfig.Save();

            if (ControlsSaved != null) 
            {
                ControlsSaved(this);
            }
        }

        //  Load Controls from the Config file. Function is private to prevent use other than instantiation of the MyGuiInput object
        private void LoadControls()
        {
            MyMwcLog.WriteLine("MyGuiInput.LoadControls() - START");

            //  Load general controls
            string controlsGeneral = MyConfig.ControlsGeneral;

            if (String.IsNullOrEmpty(controlsGeneral) || !LoadControls(controlsGeneral))
            {
                MyMwcLog.WriteLine("    Loading default controls");
                RevertToDefaultControls();
            }
            MyMwcLog.WriteLine("MyGuiInput.LoadControls() - END");
        }

        private bool LoadControls(string controlsGeneral)
        {
            try
            {
                string[] controlGeneral = controlsGeneral.Split(';');
                int index = 0;
                m_mouseXIsInverted = bool.Parse(controlGeneral[index++]);
                m_mouseYIsInverted = bool.Parse(controlGeneral[index++]);
                m_mouseSensitivity = float.Parse(controlGeneral[index++], System.Globalization.CultureInfo.InvariantCulture);

                if (controlGeneral.Length > 6)  // compatibility
                {
                    string joystickInstanceName = MyStringUtils.UnescapeSemicolons(controlGeneral[index++]);
                    JoystickInstanceName = (joystickInstanceName == "" ? null : joystickInstanceName);
                }
                else
                {
                    JoystickInstanceName = null;
                }

                m_joystickSensitivity = float.Parse(controlGeneral[index++], System.Globalization.CultureInfo.InvariantCulture);
                m_joystickExponent = float.Parse(controlGeneral[index++], System.Globalization.CultureInfo.InvariantCulture);
                m_joystickDeadzone = float.Parse(controlGeneral[index++], System.Globalization.CultureInfo.InvariantCulture);

                //  Load buttons and keys
                LoadGameControls();
                LoadEditorControls();
                return true;
            }
            catch (Exception e)
            {
                MyMwcLog.WriteLine("    Error loading controls from config:");
                MyMwcLog.WriteLine(e);
                return false;
            }
        }

        private void LoadGameControls()
        {
            string controlsButtons = MyConfig.ControlsButtons;
            if (controlsButtons.Equals(""))
            {
                throw new Exception("ControlsButtons config parameter is empty.");
            }

            //BEFORE - FIRE_PRIMARY:Mouse:Left;
            //AFTER - FIRE_PRIMARY:Keyboard:A:Mouse:Left:Joystick:None
            string[] controlButtons = controlsButtons.Split(';');
            foreach (var item in m_gameControlsList)
            {
                item.SetNoControl();
            }
            for (int i = 0; i < m_gameControlsList.Length; i++)
            {
                string[] control = controlButtons[i].Split(':');
                bool newVersion = true;
                if (control.Length != 9)
                {
                    //BACKWARDS compatibility with older config version
                    if (control.Length != 3)
                    {
                        throw new Exception("Wrong syntax of saved button assignment string.");
                    }
                    else
                    {
                        newVersion = false;
                    }
                }

                MyGameControlEnums controlType = ParseMyGameControlEnums(control[0]);

                // deprecated controls will remain without any control set to it
                if (m_deprecatedGameControls.Contains(controlType) == false)
                {
                    LoadGameControl(control[2], controlType, ParseMyGuiInputDeviceEnum(control[1]));

                    if (newVersion == true)
                    {
                        LoadGameControl(control[4], controlType, ParseMyGuiInputDeviceEnum(control[3]));
                        LoadGameControl(control[6], controlType, ParseMyGuiInputDeviceEnum(control[5]));
                        LoadGameControl(control[8], controlType, ParseMyGuiInputDeviceEnum(control[7]));
                    }
                }
            }
        }

        private void LoadEditorControls()
        {
            string editorControlsButtons = MyConfig.EditorControlsButtons;

            string[] controlButtons = editorControlsButtons.Split(';');
            if (controlButtons.Length != m_editorControlsList.Length)
            {
                throw new Exception("Size of saved controls buttons list does not match size of default editor controls list.");
            }
            foreach (var item in m_editorControlsList)
            {
                item.SetNoControl();
            }
            for (int i = 0; i < m_editorControlsList.Length; i++)
            {
                string[] control = controlButtons[i].Split(':');
                bool newVersion = true;
                if (control.Length != 9)
                {
                    //BACKWARDS compatibility with older config version
                    if (control.Length != 3)
                    {
                        throw new Exception("Wrong syntax of saved button assignment string.");
                    }
                    else
                    {
                        newVersion = false;
                    }
                }

                MyEditorControlEnums controlType = ParseMyEditorControlEnums(control[0]);
                MyGuiInputDeviceEnum device1 = ParseMyGuiInputDeviceEnum(control[1]);

                LoadEditorControl(control, controlType, device1, 2);

                if (newVersion == true)
                {
                    MyGuiInputDeviceEnum device2 = ParseMyGuiInputDeviceEnum(control[3]);
                    LoadEditorControl(control, controlType, device2, 4);

                    MyGuiInputDeviceEnum device3 = ParseMyGuiInputDeviceEnum(control[5]);
                    LoadEditorControl(control, controlType, device3, 6);

                    MyGuiInputDeviceEnum device4 = ParseMyGuiInputDeviceEnum(control[7]);
                    LoadEditorControl(control, controlType, device4, 8);
                }
            }
        }

        private void LoadGameControl(string controlName, MyGameControlEnums controlType, MyGuiInputDeviceEnum device)
        {
            switch (device)
            {
                case MyGuiInputDeviceEnum.Keyboard:
                    Keys key = (Keys)Enum.Parse(typeof(Keys), controlName);
                    if (!IsKeyValid(key))
                    {
                        throw new Exception("Key \"" + key.ToString() + "\" is already assigned or is not valid.");
                    }
                    FindNotAssignedGameControl(controlType, device).SetControl(key);
                    break;
                case MyGuiInputDeviceEnum.Mouse:
                    MyMouseButtonsEnum mouse = ParseMyMouseButtonsEnum(controlName);
                    if (!IsMouseButtonValid(mouse))
                    {
                        throw new Exception("Mouse button \"" + mouse.ToString() + "\" is already assigned or is not valid.");
                    }
                    FindNotAssignedGameControl(controlType, device).SetControl(mouse);
                    break;
                case MyGuiInputDeviceEnum.Joystick:
                    MyJoystickButtonsEnum joy = ParseMyJoystickButtonsEnum(controlName);
                    if (!IsJoystickButtonValid(joy))
                    {
                        throw new Exception("Joystick button \"" + joy.ToString() + "\" is already assigned or is not valid.");
                    }
                    FindNotAssignedGameControl(controlType, device).SetControl(joy);
                    break;
                case MyGuiInputDeviceEnum.JoystickAxis:
                    MyJoystickAxesEnum joyAxis = ParseMyJoystickAxesEnum(controlName);
                    if (!IsJoystickAxisValid(joyAxis))
                    {
                        throw new Exception("Joystick axis \"" + joyAxis.ToString() + "\" is already assigned or is not valid.");
                    }
                    FindNotAssignedGameControl(controlType, device).SetControl(joyAxis);
                    break;
                case MyGuiInputDeviceEnum.None:
                    break;
                default:
                    break;
            }
        }

        private void LoadEditorControl(string[] control, MyEditorControlEnums controlType, MyGuiInputDeviceEnum device, int controlId)
        {
            switch (device)
            {
                case MyGuiInputDeviceEnum.Keyboard:
                    Keys key = (Keys)Enum.Parse(typeof(Keys), control[controlId]);
                    if (!IsKeyValid(key))
                    {
                        throw new Exception("Key \"" + key.ToString() + "\" is already assigned or is not valid.");
                    }
                    FindNotAssignedEditorControl(controlType, device).SetControl(key);
                    break;
                case MyGuiInputDeviceEnum.Mouse:
                    MyMouseButtonsEnum mouse = ParseMyMouseButtonsEnum(control[controlId]);
                    if (!IsMouseButtonValid(mouse))
                    {
                        throw new Exception("Mouse button \"" + mouse.ToString() + "\" is already assigned or is not valid.");
                    }
                    FindNotAssignedEditorControl(controlType, device).SetControl(mouse);
                    break;
                case MyGuiInputDeviceEnum.Joystick:
                    MyJoystickButtonsEnum joy = ParseMyJoystickButtonsEnum(control[controlId]);
                    if (!IsJoystickButtonValid(joy))
                    {
                        throw new Exception("Joystick button \"" + joy.ToString() + "\" is already assigned or is not valid.");
                    }
                    FindNotAssignedEditorControl(controlType, device).SetControl(joy);
                    break;
                case MyGuiInputDeviceEnum.JoystickAxis:
                    MyJoystickAxesEnum joyAxis = ParseMyJoystickAxesEnum(control[controlId]);
                    if (!IsJoystickAxisValid(joyAxis))
                    {
                        throw new Exception("Joystick axis \"" + joyAxis.ToString() + "\" is already assigned or is not valid.");
                    }
                    FindNotAssignedEditorControl(controlType, device).SetControl(joyAxis);
                    break;
                case MyGuiInputDeviceEnum.None:
                    break;
                default:
                    break;
            }
        }

        public static MyGuiInputDeviceEnum ParseMyGuiInputDeviceEnum(string s)
        {
            for (int i = 0; i < MyEnumsToStrings.GuiInputDeviceEnum.Length; i++)
			{
                if (MyEnumsToStrings.GuiInputDeviceEnum[i] == s) return (MyGuiInputDeviceEnum)i;
			}
            throw new ArgumentException("Value \""+ s +"\" is not from GuiInputDeviceEnum.", "s");
        }

        public static MyJoystickButtonsEnum ParseMyJoystickButtonsEnum(string s)
        {
            for (int i = 0; i < MyEnumsToStrings.JoystickButtonsEnum.Length; i++)
            {
                if (MyEnumsToStrings.JoystickButtonsEnum[i] == s) return (MyJoystickButtonsEnum)i;
            }
            throw new ArgumentException("Value \"" + s + "\" is not from JoystickButtonsEnum.", "s");
        }

        public static MyJoystickAxesEnum ParseMyJoystickAxesEnum(string s)
        {
            for (int i = 0; i < MyEnumsToStrings.JoystickAxesEnum.Length; i++)
            {
                if (MyEnumsToStrings.JoystickAxesEnum[i] == s) return (MyJoystickAxesEnum)i;
            }
            throw new ArgumentException("Value \"" + s + "\" is not from JoystickAxesEnum.", "s");
        }

        public static MyMouseButtonsEnum ParseMyMouseButtonsEnum(string s)
        {
            for (int i = 0; i < MyEnumsToStrings.MouseButtonsEnum.Length; i++)
            {
                if (MyEnumsToStrings.MouseButtonsEnum[i] == s) return (MyMouseButtonsEnum)i;
            }
            throw new ArgumentException("Value \"" + s + "\" is not from MouseButtonsEnum.", "s");
        }

        public static MyGameControlEnums ParseMyGameControlEnums(string s)
        {
            for (int i = 0; i < MyEnumsToStrings.GameControlEnums.Length; i++)
            {
                if (MyEnumsToStrings.GameControlEnums[i] == s) return (MyGameControlEnums)i;
            }
            throw new ArgumentException("Value \"" + s + "\" is not from GameControlEnums.", "s");
        }

        public static MyEditorControlEnums ParseMyEditorControlEnums(string s)
        {
            for (int i = 0; i < MyEnumsToStrings.EditorControlEnums.Length; i++)
            {
                if (MyEnumsToStrings.EditorControlEnums[i] == s) return (MyEditorControlEnums)i;
            }
            throw new ArgumentException("Value \"" + s + "\" is not from EditorControlEnums.", "s");
        }

        public static MyGuiControlTypeEnum ParseMyGuiControlTypeEnum(string s)
        {
            for (int i = 0; i < MyEnumsToStrings.ControlTypeEnum.Length; i++)
            {
                if (MyEnumsToStrings.ControlTypeEnum[i] == s) return (MyGuiControlTypeEnum)i;
            }
            throw new ArgumentException("Value \"" + s + "\" is not from MyGuiInputTypeEnum.", "s");
        }

        private MyControl FindNotAssignedGameControl(MyGameControlEnums type, MyGuiInputDeviceEnum deviceType)
        {
            foreach (var item in m_gameControlsList)
            {
                if (item.GetGameControlEnum().Value == type && item.IsControlAssigned(deviceType) == false) return item;
            }
            throw new Exception("Game control \"" + MyEnumsToStrings.GameControlEnums[(int)type] + "\" not found at control list or control is already assigned.");
        }

        private MyControl FindNotAssignedEditorControl(MyEditorControlEnums type, MyGuiInputDeviceEnum deviceType)
        {
            foreach (var item in m_editorControlsList)
            {
                if (item.GetEditorControlEnum().Value == type && item.IsControlAssigned(deviceType) == false) return item;
            }
            throw new Exception("Editor control \"" + MyEnumsToStrings.GameControlEnums[(int)type] + "\" not found at control list or control is already assigned.");
        }

        public event OnControlsSaved ControlsSaved;

        public void Dispose()
        {
            UninitDevicePluginHandlerCallBack();
            if (ControlsSaved != null) 
            {
                ControlsSaved = null;
            }
        }

        public static bool ENABLE_DEVELOPER_KEYS
        {
            get
            {
                return MyMwcFinalBuildConstants.IS_DEVELOP || ((MyGuiScreenGamePlay.Static != null && MyGuiScreenGamePlay.Static.CheatsEnabled()) || (MyGuiScreenGamePlay.Static == null && MyClientServer.LoggedPlayer!= null && MyClientServer.LoggedPlayer.GetUseCheats()));
            }
        }

        #region Functionality of the old PrimaryController

        public static bool Trichording = false;

        public float GetRoll()
        {
            return GetGameControlAnalogState(MyGameControlEnums.ROLL_RIGHT) - GetGameControlAnalogState(MyGameControlEnums.ROLL_LEFT);
        }

        public Vector3 GetPositionDelta()
        {
            Vector3 moveIndicator = Vector3.Zero;

            // get rotation based on primary controller settings
            moveIndicator.X = GetGameControlAnalogState(MyGameControlEnums.STRAFE_RIGHT) - GetGameControlAnalogState(MyGameControlEnums.STRAFE_LEFT);
            moveIndicator.Y = GetGameControlAnalogState(MyGameControlEnums.UP_THRUST) - GetGameControlAnalogState(MyGameControlEnums.DOWN_THRUST);
            moveIndicator.Z = GetGameControlAnalogState(MyGameControlEnums.REVERSE) - GetGameControlAnalogState(MyGameControlEnums.FORWARD);

            // disallow trichording in game
            if (!MyGuiScreenGamePlay.Static.IsEditorActive())
            {
                if (!Trichording && moveIndicator.LengthSquared() > 1)
                {
                    moveIndicator.Normalize();
                }
            }

            return moveIndicator;
        }

        public Vector2 GetRotation()
        {
            Vector2 rotationIndicator = Vector2.Zero;
            
            // shouldn't we rotate secondary camera instead?
            if (!IsGameControlPressed(MyGameControlEnums.CONTROL_SECONDARY_CAMERA))
            {
                // get rotation from mouse
                rotationIndicator = new Vector2(GetMouseYForGamePlay() - MyMinerGame.ScreenSizeHalf.Y, GetMouseXForGamePlay() - MyMinerGame.ScreenSizeHalf.X) * MyGuiConstants.MOUSE_ROTATION_INDICATOR_MULTIPLIER;

                rotationIndicator.X -= GetGameControlAnalogState(MyGameControlEnums.ROTATION_UP);
                rotationIndicator.X += GetGameControlAnalogState(MyGameControlEnums.ROTATION_DOWN);
                rotationIndicator.Y -= GetGameControlAnalogState(MyGameControlEnums.ROTATION_LEFT);
                rotationIndicator.Y += GetGameControlAnalogState(MyGameControlEnums.ROTATION_RIGHT);
            }

            // Fix rotation to be independent from physics step.
            rotationIndicator *= MyConstants.PHYSICS_STEPS_PER_SECOND * MyGuiConstants.ROTATION_INDICATOR_MULTIPLIER;

            return rotationIndicator;
        }

        public Vector2 GetCursorPosition()
        {
            return new Vector2(GetMouseX(), GetMouseY());
        }

        #endregion
    }
}