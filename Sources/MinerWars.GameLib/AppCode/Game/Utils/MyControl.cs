using System;
using System.Text;
using MinerWars.AppCode.Toolkit.Input;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Localization;
using SysUtils.Utils;

namespace MinerWars.AppCode.Game.Utils
{
    public enum MyGuiGameControlType
    {
        GAME,
        EDITOR
    }

    enum MyGuiControlTypeEnum : byte
    {
        General = 0,
        Navigation = 1,
        Communications = 2,
        Weapons = 3,
        SpecialWeapons = 4,
        Systems1 = 5,
        Systems2 = 6,
        Editor = 7,
        Deleted = 8 //assign this type to removed controls
    }

    enum MyGuiLightPrefabTypeEnum : byte
    {
        PointLight = 0,
        SpotLight = 1
    }

    //  IMPORTANT: If you change order or names in this enum, update it also in MyEnumsToStrings
    public enum MyGuiInputDeviceEnum : byte
    {
        None = 0,
        Keyboard = 1,
        Mouse = 2,
        Joystick = 3,
        JoystickAxis = 4,
    }

    //  IMPORTANT: If you change order or names in this enum, update it also in MyEnumsToStrings
    enum MyMouseButtonsEnum : byte
    {
        None = 0,
        Left = 1,
        Middle = 2,
        Right = 3,
        XButton1 = 4,
        XButton2 = 5
    }

    //  IMPORTANT: If you change order or names in this enum, update it also in MyEnumsToStrings
    enum MyJoystickButtonsEnum : byte
    {
        None = 0,
        JDLeft = 1,  // Directional pad buttons
        JDRight = 2,
        JDUp = 3,
        JDDown = 4,
        J01 = 5,  // Regular buttons (up to 16)
        J02 = 6,
        J03 = 7,
        J04 = 8,
        J05 = 9,
        J06 = 10,
        J07 = 11,
        J08 = 12,
        J09 = 13,
        J10 = 14,
        J11 = 15,
        J12 = 16,
        J13 = 17,
        J14 = 18,
        J15 = 19,
        J16 = 20
    }

    //  IMPORTANT: If you change order or names in this enum, update it also in MyEnumsToStrings
    // If you add any axes, change also IsJoystickAxisSupported, GetJoystickAxisStateRaw, InitializeJoystickIfPossible, GetJoystickAxisStateForGameplay...
    enum MyJoystickAxesEnum : byte
    {
        None = 0,
        Xpos = 1,
        Xneg = 2,
        Ypos = 3,
        Yneg = 4,
        Zpos = 5,
        Zneg = 6,
        RotationXpos = 7,
        RotationXneg = 8,
        RotationYpos = 9,
        RotationYneg = 10,
        RotationZpos = 11,
        RotationZneg = 12,
        Slider1pos = 13,
        Slider1neg = 14,
        Slider2pos = 15,
        Slider2neg = 16,
    }

    class MyControl : ICloneable
    {
        //  Enum values
        public static Array MyJoystickButtonsEnumValues { get; private set; }
        public static Array MyJoystickAxesEnumValues { get; private set; }
        static MyControl()
        {
            MyJoystickButtonsEnumValues = Enum.GetValues(typeof(MyJoystickButtonsEnum));
            MyJoystickAxesEnumValues = Enum.GetValues(typeof(MyJoystickAxesEnum));
        }

        MyGuiGameControlType m_gameControlType;
        MyGameControlEnums? m_gameControl;
        MyEditorControlEnums? m_editorControl;
        MyTextsWrapperEnum m_text;
        MyGuiControlTypeEnum m_controlType;
        Keys m_keyboardKey = Keys.None;
        Keys m_keyboardKey2 = Keys.None;
        MyMouseButtonsEnum m_mouseButton = MyMouseButtonsEnum.None;
        MyJoystickButtonsEnum m_joystickButton = MyJoystickButtonsEnum.None;
        MyJoystickAxesEnum m_joystickAxis = MyJoystickAxesEnum.None;

        public MyControl(MyGameControlEnums control, MyTextsWrapperEnum text, MyGuiControlTypeEnum controlType, MyJoystickAxesEnum defaultControl)
            : this(control, text, controlType, null, null, null, defaultControl)
        {
        }

        public MyControl(MyGameControlEnums control, MyTextsWrapperEnum text, MyGuiControlTypeEnum controlType, MyJoystickButtonsEnum defaultControl)
            : this(control, text, controlType, null, null, defaultControl, null)
        {
        }

        public MyControl(MyGameControlEnums control, MyTextsWrapperEnum text, MyGuiControlTypeEnum controlType, Keys defaultControl)
            : this(control, text, controlType, null, defaultControl, null, null)
        {
        }

        public MyControl(MyGameControlEnums control, MyTextsWrapperEnum text, MyGuiControlTypeEnum controlType, Keys defaultControl, Keys defaultSecondControl)
            : this(control, text, controlType, null, defaultControl, null, null)
        {
            m_keyboardKey2 = defaultSecondControl;
        }

        public MyControl(MyGameControlEnums control, MyTextsWrapperEnum text, MyGuiControlTypeEnum controlType, MyMouseButtonsEnum defaultControl)
            : this(control, text, controlType, defaultControl, null, null, null)
        {
        }

        public MyControl(MyEditorControlEnums control, MyTextsWrapperEnum text, MyGuiControlTypeEnum controlType, MyJoystickAxesEnum defaultControl)
            : this(control, text, controlType, null, null, null, defaultControl)
        {
        }

        public MyControl(MyEditorControlEnums control, MyTextsWrapperEnum text, MyGuiControlTypeEnum controlType, MyJoystickButtonsEnum defaultControl)
            : this(control, text, controlType, null, null, defaultControl, null)
        {
        }

        public MyControl(MyEditorControlEnums control, MyTextsWrapperEnum text, MyGuiControlTypeEnum controlType, Keys defaultControl)
            : this(control, text, controlType, null, defaultControl, null, null)
        {
        }

        public MyControl(MyEditorControlEnums control, MyTextsWrapperEnum text, MyGuiControlTypeEnum controlType, MyMouseButtonsEnum defaultControl)
            : this(control, text, controlType, defaultControl, null, null, null)
        {
        }

        public MyControl(MyGameControlEnums control, MyTextsWrapperEnum text, MyGuiControlTypeEnum controlType, MyMouseButtonsEnum? defaultControlMouse,
            Keys? defaultControlKey, MyJoystickButtonsEnum? defaultControlJoy, MyJoystickAxesEnum? defaultControlJoyAxis)
            : this(control, text, controlType)
        {
            m_mouseButton = defaultControlMouse.HasValue ? defaultControlMouse.Value : MyMouseButtonsEnum.None;
            m_keyboardKey = defaultControlKey.HasValue ? defaultControlKey.Value : Keys.None;
            m_joystickButton = defaultControlJoy.HasValue ? defaultControlJoy.Value : MyJoystickButtonsEnum.None;
            m_joystickAxis = defaultControlJoyAxis.HasValue ? defaultControlJoyAxis.Value : MyJoystickAxesEnum.None;
        }

        public MyControl(MyEditorControlEnums control, MyTextsWrapperEnum text, MyGuiControlTypeEnum controlType, MyMouseButtonsEnum? defaultControlMouse,
            Keys? defaultControlKey, MyJoystickButtonsEnum? defaultControlJoy, MyJoystickAxesEnum? defaultControlJoyAxis)
            : this(control, text, controlType)
        {
            m_mouseButton = defaultControlMouse.HasValue ? defaultControlMouse.Value : MyMouseButtonsEnum.None;
            m_keyboardKey = defaultControlKey.HasValue ? defaultControlKey.Value : Keys.None;
            m_joystickButton = defaultControlJoy.HasValue ? defaultControlJoy.Value : MyJoystickButtonsEnum.None;
            m_joystickAxis = defaultControlJoyAxis.HasValue ? defaultControlJoyAxis.Value : MyJoystickAxesEnum.None;
        }

        public MyControl(MyGameControlEnums control, MyTextsWrapperEnum text, MyGuiControlTypeEnum controlType)
        {
            m_gameControl = control;
            m_gameControlType = MyGuiGameControlType.GAME;
            m_text = text;
            m_controlType = controlType;
            m_joystickButton = MyJoystickButtonsEnum.None;
            m_joystickAxis = MyJoystickAxesEnum.None;
            m_mouseButton = MyMouseButtonsEnum.None;
            m_keyboardKey = Keys.None;
        }

        public MyControl(MyEditorControlEnums control, MyTextsWrapperEnum text, MyGuiControlTypeEnum controlType)
        {
            m_editorControl = control;
            m_gameControlType = MyGuiGameControlType.EDITOR;
            m_text = text;
            m_controlType = controlType;
            m_joystickButton = MyJoystickButtonsEnum.None;
            m_joystickAxis = MyJoystickAxesEnum.None;
            m_mouseButton = MyMouseButtonsEnum.None;
            m_keyboardKey = Keys.None;
        }

        public void SetControl(Keys key)
        {
            m_keyboardKey = key;
        }

        public void SetSecondControl(Keys key)
        {
            m_keyboardKey2 = key;
        }

        public void SetControl(MyMouseButtonsEnum mouseButton)
        {
            m_mouseButton = mouseButton;
        }

        public void SetControl(MyJoystickButtonsEnum joyButton)
        {
            m_joystickButton = joyButton;
        }

        public void SetControl(MyJoystickAxesEnum joyAxis)
        {
            m_joystickAxis = joyAxis;
        }

        public void SetNoControl()
        {
            m_joystickAxis = MyJoystickAxesEnum.None;
            m_joystickButton = MyJoystickButtonsEnum.None;
            m_mouseButton = MyMouseButtonsEnum.None;
            m_keyboardKey = Keys.None;
            m_keyboardKey2 = Keys.None;
        }

        public Keys GetKeyboardControl()
        {
            return m_keyboardKey;
        }

        public Keys GetSecondKeyboardControl()
        {
            return m_keyboardKey2;
        }

        public MyMouseButtonsEnum GetMouseControl()
        {
            return m_mouseButton;
        }

        public MyJoystickButtonsEnum GetJoystickControl()
        {
            return m_joystickButton;
        }

        public MyJoystickAxesEnum GetJoystickAxisControl()
        {
            return m_joystickAxis;
        }

        public bool IsPressed()
        {
            bool pressed = false;

            if (m_keyboardKey != Keys.None)
            {
                pressed = MyGuiManager.GetInput().IsKeyPress(m_keyboardKey);
                if (pressed == false)
                {
                    pressed = MyGuiManager.GetInput().IsKeyPress(m_keyboardKey2);
                }
            }

            if (m_mouseButton != MyMouseButtonsEnum.None && pressed == false)
            {
                switch (m_mouseButton)
                {
                    case MyMouseButtonsEnum.Left:
                        pressed = MyGuiManager.GetInput().IsLeftMousePressed();
                        break;
                    case MyMouseButtonsEnum.Middle:
                        pressed = MyGuiManager.GetInput().IsMiddleMousePressed();
                        break;
                    case MyMouseButtonsEnum.Right:
                        pressed = MyGuiManager.GetInput().IsRightMousePressed();
                        break;
                    case MyMouseButtonsEnum.XButton1:
                        pressed = MyGuiManager.GetInput().IsXButton1MousePressed();
                        break;
                    case MyMouseButtonsEnum.XButton2:
                        pressed = MyGuiManager.GetInput().IsXButton2MousePressed();
                        break;
                }
            }

            if (m_joystickButton != MyJoystickButtonsEnum.None && pressed == false)
            {
                pressed = MyGuiManager.GetInput().IsJoystickButtonPressed(m_joystickButton);
            }

            if (m_joystickAxis != MyJoystickAxesEnum.None && pressed == false)
            {
                pressed = MyGuiManager.GetInput().IsJoystickAxisPressed(m_joystickAxis);
            }

            return pressed;
        }

        public bool IsNewPressed()
        {
            bool pressed = false;

            if (m_keyboardKey != Keys.None)
            {
                pressed = MyGuiManager.GetInput().IsNewKeyPress(m_keyboardKey);
                if (pressed == false)
                {
                    pressed = MyGuiManager.GetInput().IsNewKeyPress(m_keyboardKey2);
                }
            }

            if (m_mouseButton != MyMouseButtonsEnum.None && pressed == false)
            {
                switch (m_mouseButton)
                {
                    case MyMouseButtonsEnum.Left:
                        pressed = MyGuiManager.GetInput().IsNewLeftMousePressed();
                        break;
                    case MyMouseButtonsEnum.Middle:
                        pressed = MyGuiManager.GetInput().IsNewMiddleMousePressed();
                        break;
                    case MyMouseButtonsEnum.Right:
                        pressed = MyGuiManager.GetInput().IsNewRightMousePressed();
                        break;
                    case MyMouseButtonsEnum.XButton1:
                        pressed = MyGuiManager.GetInput().IsNewXButton1MousePressed();
                        break;
                    case MyMouseButtonsEnum.XButton2:
                        pressed = MyGuiManager.GetInput().IsNewXButton2MousePressed();
                        break;
                }
            }

            if (m_joystickButton != MyJoystickButtonsEnum.None && pressed == false)
            {
                pressed = MyGuiManager.GetInput().IsJoystickButtonNewPressed(m_joystickButton);
            }

            if (m_joystickAxis != MyJoystickAxesEnum.None && pressed == false)
            {
                pressed = MyGuiManager.GetInput().IsJoystickAxisNewPressed(m_joystickAxis);
            }
            return pressed;
        }

        public bool WasPressed()
        {
            bool pressed = false;

            if (m_keyboardKey != Keys.None)
            {
                pressed = MyGuiManager.GetInput().WasKeyPressed(m_keyboardKey);
                if (pressed == false)
                {
                    pressed = MyGuiManager.GetInput().WasKeyPressed(m_keyboardKey2);
                }
            }

            if (m_mouseButton != MyMouseButtonsEnum.None && pressed == false)
            {
                switch (m_mouseButton)
                {
                    case MyMouseButtonsEnum.Left:
                        pressed = MyGuiManager.GetInput().WasLeftMousePressed();
                        break;
                    case MyMouseButtonsEnum.Middle:
                        pressed = MyGuiManager.GetInput().WasMiddleMousePressed();
                        break;
                    case MyMouseButtonsEnum.Right:
                        pressed = MyGuiManager.GetInput().WasRightMousePressed();
                        break;
                    case MyMouseButtonsEnum.XButton1:
                        pressed = MyGuiManager.GetInput().WasXButton1MousePressed();
                        break;
                    case MyMouseButtonsEnum.XButton2:
                        pressed = MyGuiManager.GetInput().WasXButton2MousePressed();
                        break;
                }
            }

            if (m_joystickButton != MyJoystickButtonsEnum.None && pressed == false)
            {
                pressed = MyGuiManager.GetInput().WasJoystickButtonPressed(m_joystickButton);
            }

            if (m_joystickAxis != MyJoystickAxesEnum.None && pressed == false)
            {
                pressed = MyGuiManager.GetInput().WasJoystickAxisPressed(m_joystickAxis);
            }
            return pressed;
        }

        public bool IsNewReleased()
        {
            bool released = false;

            if (m_keyboardKey != Keys.None)
            {
                released = MyGuiManager.GetInput().IsNewKeyReleased(m_keyboardKey);
                if (released == false)
                {
                    released = MyGuiManager.GetInput().IsNewKeyReleased(m_keyboardKey2);
                }
            }

            if (m_mouseButton != MyMouseButtonsEnum.None && released == false)
            {
                switch (m_mouseButton)
                {
                    case MyMouseButtonsEnum.Left:
                        released = MyGuiManager.GetInput().IsNewLeftMouseReleased();
                        break;
                    case MyMouseButtonsEnum.Middle:
                        released = MyGuiManager.GetInput().IsNewMiddleMouseReleased();
                        break;
                    case MyMouseButtonsEnum.Right:
                        released = MyGuiManager.GetInput().IsNewRightMouseReleased();
                        break;
                    case MyMouseButtonsEnum.XButton1:
                        released = MyGuiManager.GetInput().IsNewXButton1MouseReleased();
                        break;
                    case MyMouseButtonsEnum.XButton2:
                        released = MyGuiManager.GetInput().IsNewXButton2MouseReleased();
                        break;
                }
            }

            if (m_joystickButton != MyJoystickButtonsEnum.None && released == false)
            {
                released = MyGuiManager.GetInput().IsNewJoystickButtonReleased(m_joystickButton);
            }
            if (m_joystickAxis != MyJoystickAxesEnum.None && released == false)
            {
                released = MyGuiManager.GetInput().IsNewJoystickAxisReleased(m_joystickAxis);
            }

            return released;
        }

        public bool WasReleased()
        {
            bool wasReleased = false;

            if (m_keyboardKey != Keys.None)
            {
                wasReleased = MyGuiManager.GetInput().WasKeyReleased(m_keyboardKey);
                if (wasReleased == false)
                {
                    wasReleased = MyGuiManager.GetInput().WasKeyReleased(m_keyboardKey2);
                }
            }

            if (m_mouseButton != MyMouseButtonsEnum.None && wasReleased == false)
            {
                switch (m_mouseButton)
                {
                    case MyMouseButtonsEnum.Left:
                        wasReleased = MyGuiManager.GetInput().WasLeftMouseReleased();
                        break;
                    case MyMouseButtonsEnum.Middle:
                        wasReleased = MyGuiManager.GetInput().WasMiddleMouseReleased();
                        break;
                    case MyMouseButtonsEnum.Right:
                        wasReleased = MyGuiManager.GetInput().WasRightMouseReleased();
                        break;
                    case MyMouseButtonsEnum.XButton1:
                        wasReleased = MyGuiManager.GetInput().WasNewXButton1MouseReleased();
                        break;
                    case MyMouseButtonsEnum.XButton2:
                        wasReleased = MyGuiManager.GetInput().WasNewXButton2MouseReleased();
                        break;
                }
            }

            if (m_joystickButton != MyJoystickButtonsEnum.None && wasReleased == false)
            {
                wasReleased = MyGuiManager.GetInput().WasJoystickButtonReleased(m_joystickButton);
            }
            if (m_joystickAxis != MyJoystickAxesEnum.None && wasReleased == false)
            {
                wasReleased = MyGuiManager.GetInput().WasJoystickAxisReleased(m_joystickAxis);
            }

            return wasReleased;
        }


        /// <summary>
        /// Return the analog state between 0 (not pressed at all) and 1 (fully pressed).
        /// If a digital button is mapped to an analog control, it can return only 0 or 1.
        /// </summary>
        public float GetAnalogState()
        {
            bool pressed = false;

            if (m_keyboardKey != Keys.None)
            {
                pressed = MyGuiManager.GetInput().IsKeyPress(m_keyboardKey);
                if (pressed == false)
                {
                    pressed = MyGuiManager.GetInput().IsKeyPress(m_keyboardKey2);
                }
            }

            if (m_mouseButton != MyMouseButtonsEnum.None && pressed == false)
            {
                switch (m_mouseButton)
                {
                    case MyMouseButtonsEnum.Left:
                        pressed = MyGuiManager.GetInput().IsLeftMousePressed();
                        break;
                    case MyMouseButtonsEnum.Middle:
                        pressed = MyGuiManager.GetInput().IsMiddleMousePressed();
                        break;
                    case MyMouseButtonsEnum.Right:
                        pressed = MyGuiManager.GetInput().IsRightMousePressed();
                        break;
                    case MyMouseButtonsEnum.XButton1:
                        pressed = MyGuiManager.GetInput().IsXButton1MousePressed();
                        break;
                    case MyMouseButtonsEnum.XButton2:
                        pressed = MyGuiManager.GetInput().IsXButton2MousePressed();
                        break;
                }
            }

            if (m_joystickButton != MyJoystickButtonsEnum.None && pressed == false)
            {
                pressed = MyGuiManager.GetInput().IsJoystickButtonPressed(m_joystickButton);
            }

            if (pressed) return 1;
            
            if (m_joystickAxis != MyJoystickAxesEnum.None)
            {
                return MyGuiManager.GetInput().GetJoystickAxisStateForGameplay(m_joystickAxis);
            }
            return 0;
        }


        public MyTextsWrapperEnum GetControlName()
        {
            return m_text;
        }

        public MyGuiControlTypeEnum GetControlTypeEnum()
        {
            return m_controlType;
        }

        public MyGameControlEnums? GetGameControlEnum()
        {
            return m_gameControl;
        }

        public MyEditorControlEnums? GetEditorControlEnum()
        {
            return m_editorControl;
        }

        public MyGuiGameControlType GetGameControlTypeEnum()
        {
            return m_gameControlType;
        }

        public String GetControlButtonName(MyGuiInputDeviceEnum deviceType)
        {
            return GetControlButtonStringBuilder(deviceType).ToString();
        }

        public StringBuilder GetControlButtonStringBuilder(MyGuiInputDeviceEnum deviceType)
        {            
            switch (deviceType)
            {
                case MyGuiInputDeviceEnum.Keyboard:
                    if (m_keyboardKey != Keys.None)
                        return new StringBuilder(MyKeysToString.GetKeyName(m_keyboardKey));
                    break;
                case MyGuiInputDeviceEnum.Mouse:
                    switch(m_mouseButton)
                    {
                        case MyMouseButtonsEnum.Left: return MyTextsWrapper.Get(MyTextsWrapperEnum.LeftMouseButton);
                        case MyMouseButtonsEnum.Middle: return MyTextsWrapper.Get(MyTextsWrapperEnum.MiddleMouseButton);
                        case MyMouseButtonsEnum.Right: return MyTextsWrapper.Get(MyTextsWrapperEnum.RightMouseButton);
                        case MyMouseButtonsEnum.XButton1: return MyTextsWrapper.Get(MyTextsWrapperEnum.MouseXButton1);
                        case MyMouseButtonsEnum.XButton2: return MyTextsWrapper.Get(MyTextsWrapperEnum.MouseXButton2);
                    }
                    break;
                case MyGuiInputDeviceEnum.Joystick:
                    if (m_joystickButton != MyJoystickButtonsEnum.None)
                    {
                        switch (m_joystickButton)
                        {
                            case MyJoystickButtonsEnum.JDLeft: return MyTextsWrapper.Get(MyTextsWrapperEnum.JoystickButtonLeft);
                            case MyJoystickButtonsEnum.JDRight: return MyTextsWrapper.Get(MyTextsWrapperEnum.JoystickButtonRight);
                            case MyJoystickButtonsEnum.JDUp: return MyTextsWrapper.Get(MyTextsWrapperEnum.JoystickButtonUp);
                            case MyJoystickButtonsEnum.JDDown: return MyTextsWrapper.Get(MyTextsWrapperEnum.JoystickButtonDown);
                            default: 
                                StringBuilder buttonName = new StringBuilder(MyTextsWrapper.Get(MyTextsWrapperEnum.JoystickButton).ToString());
                                return buttonName.Append(((int)m_joystickButton - 4).ToString());
                        }
                    }
                    break;
                case MyGuiInputDeviceEnum.JoystickAxis:
                    if (m_joystickAxis != MyJoystickAxesEnum.None)
                    {
                        switch (m_joystickAxis)
                        {
                            case MyJoystickAxesEnum.Xpos: return MyTextsWrapper.Get(MyTextsWrapperEnum.JoystickAxisXpos);
                            case MyJoystickAxesEnum.Xneg: return MyTextsWrapper.Get(MyTextsWrapperEnum.JoystickAxisXneg);
                            case MyJoystickAxesEnum.Ypos: return MyTextsWrapper.Get(MyTextsWrapperEnum.JoystickAxisYpos);
                            case MyJoystickAxesEnum.Yneg: return MyTextsWrapper.Get(MyTextsWrapperEnum.JoystickAxisYneg);
                            case MyJoystickAxesEnum.Zpos: return MyTextsWrapper.Get(MyTextsWrapperEnum.JoystickAxisZpos);
                            case MyJoystickAxesEnum.Zneg: return MyTextsWrapper.Get(MyTextsWrapperEnum.JoystickAxisZneg);
                            case MyJoystickAxesEnum.RotationXpos: return MyTextsWrapper.Get(MyTextsWrapperEnum.JoystickRotationXpos);
                            case MyJoystickAxesEnum.RotationXneg: return MyTextsWrapper.Get(MyTextsWrapperEnum.JoystickRotationXneg);
                            case MyJoystickAxesEnum.RotationYpos: return MyTextsWrapper.Get(MyTextsWrapperEnum.JoystickRotationYpos);
                            case MyJoystickAxesEnum.RotationYneg: return MyTextsWrapper.Get(MyTextsWrapperEnum.JoystickRotationYneg);
                            case MyJoystickAxesEnum.RotationZpos: return MyTextsWrapper.Get(MyTextsWrapperEnum.JoystickRotationZpos);
                            case MyJoystickAxesEnum.RotationZneg: return MyTextsWrapper.Get(MyTextsWrapperEnum.JoystickRotationZneg);
                            case MyJoystickAxesEnum.Slider1pos: return MyTextsWrapper.Get(MyTextsWrapperEnum.JoystickSlider1pos);
                            case MyJoystickAxesEnum.Slider1neg: return MyTextsWrapper.Get(MyTextsWrapperEnum.JoystickSlider1neg);
                            case MyJoystickAxesEnum.Slider2pos: return MyTextsWrapper.Get(MyTextsWrapperEnum.JoystickSlider2pos);
                            case MyJoystickAxesEnum.Slider2neg: return MyTextsWrapper.Get(MyTextsWrapperEnum.JoystickSlider2neg);
                        }
                    }
                    break;
            }
            return MyTextsWrapper.Get(MyTextsWrapperEnum.UnknownControl);
        }

        public StringBuilder GetControlButtonStringBuilderCombined(string separator)
        {
            var result = new StringBuilder();

            foreach (MyGuiInputDeviceEnum value in Enum.GetValues(typeof(MyGuiInputDeviceEnum)))
            {
                var name = GetControlButtonStringBuilder(value);
                if (name != MyTextsWrapper.Get(MyTextsWrapperEnum.UnknownControl))
                {
                    if (result.Length != 0) result.Append(separator);
                    result.Append(name);
                }
            }
            if (result.Length == 0) result.Append(MyTextsWrapper.Get(MyTextsWrapperEnum.UnknownControl));
            return result;
        }

            
        public bool IsControlAssigned()
        {
            return (m_keyboardKey != Keys.None) || (m_joystickButton != MyJoystickButtonsEnum.None) || (m_mouseButton != MyMouseButtonsEnum.None) || (m_joystickAxis != MyJoystickAxesEnum.None);
        }

        public bool IsControlAssigned(MyGuiInputDeviceEnum deviceType)
        {
            bool isAssigned = false;
            switch (deviceType)
            {
                case MyGuiInputDeviceEnum.Keyboard:
                    isAssigned = m_keyboardKey != Keys.None;
                    break;
                case MyGuiInputDeviceEnum.Mouse:
                    isAssigned = m_mouseButton != MyMouseButtonsEnum.None;
                    break;
                case MyGuiInputDeviceEnum.Joystick:
                    isAssigned = m_joystickButton != MyJoystickButtonsEnum.None;
                    break;
                case MyGuiInputDeviceEnum.JoystickAxis:
                    isAssigned = m_joystickAxis != MyJoystickAxesEnum.None;
                    break;
            }
            return isAssigned;
        }

        #region ICloneable Members

        public MyControl Clone()
        {
            MyControl copy = (MyControl)this.MemberwiseClone();
            return copy;
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        #endregion
    }
}
