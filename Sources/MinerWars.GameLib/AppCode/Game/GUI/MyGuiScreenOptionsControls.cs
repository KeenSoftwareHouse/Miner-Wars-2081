using System.Collections.Generic;
using System.Text;
using System.Threading;
using MinerWarsMath;
using MinerWarsMath.Graphics;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Utils;
using System;
using MinerWars.AppCode.Toolkit.Input;
using MinerWars.AppCode.Game.Audio;
using MinerWars.AppCode.Game.GUI.Helpers;
using MinerWars.AppCode.Game.Textures;

namespace MinerWars.AppCode.Game.GUI
{
    class MyGuiScreenOptionsControls : MyGuiScreenBase
    {
        MyGuiControlTypeEnum m_currentControlType;
        MyGuiControlCombobox m_controlTypeList;

        readonly Vector2 CONTROL_BUTTON_SIZE = new Vector2(163 / 1600f, 66 / 1200f);

        //  All controls in this screen
        Dictionary<MyGuiControlTypeEnum, List<MyGuiControlBase>> m_allControls = new Dictionary<MyGuiControlTypeEnum, List<MyGuiControlBase>>();

        //  Dictionary for getting button by control
        Dictionary<MyControl, MyGuiControlButton> m_keyButtonsDictionary;
        Dictionary<MyControl, MyGuiControlButton> m_mouseButtonsDictionary;
        Dictionary<MyControl, MyGuiControlButton> m_joystickButtonsDictionary;
        Dictionary<MyControl, MyGuiControlButton> m_joystickAxesDictionary;

        //  List for getting button by input type
        List<MyGuiControlButton> m_keyButtons;
        List<MyGuiControlButton> m_mouseButtons;
        List<MyGuiControlButton> m_joystickButtons;
        List<MyGuiControlButton> m_joystickAxes;

        //  I need these checkboxes here so I can check their value if the user clicks 'OK'
        MyGuiControlCheckbox m_invertMouseXCheckbox;
        MyGuiControlCheckbox m_invertMouseYCheckbox;
        MyGuiControlSlider m_mouseSensitivitySlider;
        MyGuiControlSlider m_joystickSensitivitySlider;
        MyGuiControlSlider m_joystickDeadzoneSlider;
        MyGuiControlSlider m_joystickExponentSlider;
        MyGuiControlCombobox m_joystickCombobox;

        Vector2 m_controlsOriginLeft;
        Vector2 m_controlsOriginRight;

        public MyGuiScreenOptionsControls()
            : base(new Vector2(0.5f, 0.5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, null)
        {
            m_size = new Vector2(1110f / 1600f, 1127f / 1200f);
            m_backgroundTexture = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\BackgroundScreen\\ControlsBackground", flags: TextureFlags.IgnoreQuality);

            m_enableBackgroundFade = true;
            AddCaption(MyTextsWrapperEnum.Controls, new Vector2(0, 0.005f));

            MyGuiManager.GetInput().TakeSnapshot();

            m_controlsOriginLeft = new Vector2(-m_size.Value.X / 2.0f + 0.06f, -m_size.Value.Y / 2.0f + 0.102f);
            m_controlsOriginRight = new Vector2(-m_size.Value.X / 2.0f + 0.333f, -m_size.Value.Y / 2.0f + 0.102f);

            #region Add Revert, OK, Cancel and selection combobox
            /*
            //  Buttons Revert, OK and CANCEL
            float buttonsY = m_size.Value.Y / 2.0f - MyGuiConstants.MESSAGE_BOX_BORDER_AREA_Y - MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE.Y / 2.0f;
            Controls.Add(new MyGuiControlButton(this,
                new Vector2((MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE.X + 0.015f) * -1, buttonsY),
                MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,

                MyTextsWrapperEnum.Ok, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnOkClick,
                MyGuiControlButtonTextAlignment.CENTERED, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));
            Controls.Add(new MyGuiControlButton(this,
                new Vector2(0, buttonsY),
                MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.Cancel, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnCancelClick,
                MyGuiControlButtonTextAlignment.CENTERED, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));
            Controls.Add(new MyGuiControlButton(this,
                new Vector2(m_controlsOriginRight.X + MyGuiConstants.TEXTBOX_MEDIUM_SIZE.X, buttonsY),
                new Vector2(MyGuiConstants.BUTTON_DEFAULTS_WIDTH, MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE.Y), MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.Revert, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnResetDefaultsClick,
                MyGuiControlButtonTextAlignment.CENTERED, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));
            */


            var okButton = new MyGuiControlButton(this, new Vector2(-0.1942f, 0.3435f), MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE_SMALL,
                MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyGuiManager.GetConfirmButton(), null, null, MyTextsWrapperEnum.Ok,
                MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, MyGuiControlButtonTextAlignment.CENTERED, OnOkClick,
                true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true, true);
            var cancelButton = new MyGuiControlButton(this, new Vector2(-0.0597f, 0.3435f), MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE_SMALL,
                MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyGuiManager.GetConfirmButton(), null, null, MyTextsWrapperEnum.Cancel,
                MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, MyGuiControlButtonTextAlignment.CENTERED, OnCancelClick,
                true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true, true);
            var resetButton = new MyGuiControlButton(this, new Vector2(0.1942f, 0.3435f), MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE_SMALL,
                MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyGuiManager.GetConfirmButton(), null, null, MyTextsWrapperEnum.Revert,
                MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, MyGuiControlButtonTextAlignment.CENTERED, OnResetDefaultsClick,
                true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true, true);
            Controls.Add(okButton);
            Controls.Add(cancelButton);
            Controls.Add(resetButton);




            //  Page selection combobox
            m_currentControlType = MyGuiControlTypeEnum.General;
            var cBoxPosition = m_controlsOriginRight + 0.5f * MyGuiConstants.CONTROLS_DELTA +
                               new Vector2(MyGuiConstants.COMBOBOX_MEDIUM_SIZE.X / 2.0f, 0) - new Vector2(0.065f, 0);
            m_controlTypeList = new MyGuiControlCombobox(this, cBoxPosition, MyGuiControlPreDefinedSize.MEDIUM, MyGuiConstants.COMBOBOX_BACKGROUND_COLOR, MyGuiConstants.COMBOBOX_TEXT_SCALE);
            m_controlTypeList.AddItem((int)MyGuiControlTypeEnum.General, MyTextsWrapperEnum.General);
            m_controlTypeList.AddItem((int)MyGuiControlTypeEnum.Navigation, MyTextsWrapperEnum.Navigation);
            // There are no controls for this category now, so hide it completely and uncomment, when we have new comms controls
            //m_controlTypeList.AddItem((int)MyGuiInputTypeEnum.Communications, MyTextsWrapperEnum.Comms);
            m_controlTypeList.AddItem((int)MyGuiControlTypeEnum.Weapons, MyTextsWrapperEnum.Weapons);
            m_controlTypeList.AddItem((int)MyGuiControlTypeEnum.SpecialWeapons, MyTextsWrapperEnum.SpecialWeapons);
            m_controlTypeList.AddItem((int)MyGuiControlTypeEnum.Systems1, MyTextsWrapperEnum.Systems1);
            m_controlTypeList.AddItem((int)MyGuiControlTypeEnum.Systems2, MyTextsWrapperEnum.Systems2);
            m_controlTypeList.AddItem((int)MyGuiControlTypeEnum.Editor, MyTextsWrapperEnum.Editor);
            m_controlTypeList.SelectItemByKey((int)m_currentControlType);
            Controls.Add(m_controlTypeList);

            #endregion

            AddControls();

            ActivateControls(m_currentControlType);
        }

        private void AddControls()
        {
            m_keyButtonsDictionary = new Dictionary<MyControl, MyGuiControlButton>();
            m_mouseButtonsDictionary = new Dictionary<MyControl, MyGuiControlButton>();
            m_joystickButtonsDictionary = new Dictionary<MyControl, MyGuiControlButton>();
            m_joystickAxesDictionary = new Dictionary<MyControl, MyGuiControlButton>();
            m_keyButtons = new List<MyGuiControlButton>();
            m_mouseButtons = new List<MyGuiControlButton>();
            m_joystickButtons = new List<MyGuiControlButton>();
            m_joystickAxes = new List<MyGuiControlButton>();

            //  "General" page is little bit too complex, so I need to create it separately.
            #region AddControlsByType(MyGuiInputTypeEnum.GENERAL_CONTROL);

            m_allControls[MyGuiControlTypeEnum.General] = new List<MyGuiControlBase>();

            m_allControls[MyGuiControlTypeEnum.General].Add(new MyGuiControlLabel(this, m_controlsOriginLeft + 2 * MyGuiConstants.CONTROLS_DELTA, null, MyTextsWrapperEnum.InvertMouseX, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            //m_invertMouseXCheckbox = new MyGuiControlCheckbox(this, m_controlsOriginRight + 2 * MyGuiConstants.CONTROLS_DELTA + new Vector2(MyGuiConstants.CHECKBOX_SIZE.X / 2.0f, 0), MyGuiManager.GetInput().GetMouseXInversion(), MyGuiConstants.CHECKBOX_BACKGROUND_COLOR);
            m_invertMouseXCheckbox = new MyGuiControlCheckbox(this,
                     m_controlsOriginRight + 2 * MyGuiConstants.CONTROLS_DELTA + new Vector2(MyGuiConstants.CHECKBOX_WITH_GLOW_SIZE.X / 2.0f, 0),
                     MyGuiConstants.CHECKBOX_WITH_GLOW_SIZE,
                     MyGuiManager.GetCheckboxOffTexture(), MyGuiManager.GetCheckboxOnTexture(), null,
                     MyGuiManager.GetInput().GetMouseXInversion(), MyGuiConstants.CHECKBOX_BACKGROUND_COLOR, true, null);

            m_allControls[MyGuiControlTypeEnum.General].Add(m_invertMouseXCheckbox);

            m_allControls[MyGuiControlTypeEnum.General].Add(new MyGuiControlLabel(this, m_controlsOriginLeft + 3 * MyGuiConstants.CONTROLS_DELTA, null, MyTextsWrapperEnum.InvertMouseY, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            //m_invertMouseYCheckbox = new MyGuiControlCheckbox(this, m_controlsOriginRight + 3 * MyGuiConstants.CONTROLS_DELTA + new Vector2(MyGuiConstants.CHECKBOX_SIZE.X / 2.0f, 0), MyGuiManager.GetInput().GetMouseYInversion(), MyGuiConstants.CHECKBOX_BACKGROUND_COLOR);
            m_invertMouseYCheckbox = new MyGuiControlCheckbox(this,
                 m_controlsOriginRight + 3 * MyGuiConstants.CONTROLS_DELTA + new Vector2(MyGuiConstants.CHECKBOX_WITH_GLOW_SIZE.X / 2.0f, 0),
                 MyGuiConstants.CHECKBOX_WITH_GLOW_SIZE,
                 MyGuiManager.GetCheckboxOffTexture(), MyGuiManager.GetCheckboxOnTexture(), null,
                 MyGuiManager.GetInput().GetMouseYInversion(), MyGuiConstants.CHECKBOX_BACKGROUND_COLOR, true, null);

            m_allControls[MyGuiControlTypeEnum.General].Add(m_invertMouseYCheckbox);

            m_allControls[MyGuiControlTypeEnum.General].Add(new MyGuiControlLabel(this, m_controlsOriginLeft + 4 * MyGuiConstants.CONTROLS_DELTA, null, MyTextsWrapperEnum.MouseSensitivity, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            m_mouseSensitivitySlider = new MyGuiControlSlider(this, m_controlsOriginRight + 4 * MyGuiConstants.CONTROLS_DELTA + new Vector2(MyGuiConstants.COMBOBOX_MEDIUM_SIZE.X / 2.0f, 0), MyGuiConstants.COMBOBOX_MEDIUM_SIZE.X, 0.0f, 3.0f, MyGuiConstants.SLIDER_BACKGROUND_COLOR, new StringBuilder(), MyGuiConstants.TEXTBOX_MEDIUM_SIZE.X, 0, MyGuiConstants.LABEL_TEXT_SCALE);
            m_mouseSensitivitySlider.SetValue(MyGuiManager.GetInput().GetMouseSensitivity());
            m_allControls[MyGuiControlTypeEnum.General].Add(m_mouseSensitivitySlider);

            m_allControls[MyGuiControlTypeEnum.General].Add(new MyGuiControlLabel(this, m_controlsOriginLeft + 8.5f * MyGuiConstants.CONTROLS_DELTA, null, MyTextsWrapperEnum.Joystick, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            m_joystickCombobox = new MyGuiControlCombobox(this, m_controlsOriginRight + 8.5f * MyGuiConstants.CONTROLS_DELTA + new Vector2(MyGuiConstants.COMBOBOX_MEDIUM_SIZE.X / 2.0f, 0), MyGuiControlPreDefinedSize.MEDIUM, MyGuiConstants.COMBOBOX_BACKGROUND_COLOR, MyGuiConstants.COMBOBOX_TEXT_SCALE);
            m_joystickCombobox.OnSelect += OnSelectJoystick;
            AddJoysticksToComboBox();
            m_allControls[MyGuiControlTypeEnum.General].Add(m_joystickCombobox);

            m_allControls[MyGuiControlTypeEnum.General].Add(new MyGuiControlLabel(this, m_controlsOriginLeft + 10 * MyGuiConstants.CONTROLS_DELTA, null, MyTextsWrapperEnum.JoystickSensitivity, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            m_joystickSensitivitySlider = new MyGuiControlSlider(this, m_controlsOriginRight + 10 * MyGuiConstants.CONTROLS_DELTA + new Vector2(MyGuiConstants.COMBOBOX_MEDIUM_SIZE.X / 2.0f, 0), MyGuiConstants.COMBOBOX_MEDIUM_SIZE.X, 0.1f, 6.0f, MyGuiConstants.SLIDER_BACKGROUND_COLOR, new StringBuilder(), MyGuiConstants.TEXTBOX_MEDIUM_SIZE.X, 0, MyGuiConstants.LABEL_TEXT_SCALE);
            m_joystickSensitivitySlider.SetValue(MyGuiManager.GetInput().GetJoystickSensitivity());
            m_allControls[MyGuiControlTypeEnum.General].Add(m_joystickSensitivitySlider);

            m_allControls[MyGuiControlTypeEnum.General].Add(new MyGuiControlLabel(this, m_controlsOriginLeft + 11 * MyGuiConstants.CONTROLS_DELTA, null, MyTextsWrapperEnum.JoystickExponent, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            m_joystickExponentSlider = new MyGuiControlSlider(this, m_controlsOriginRight + 11 * MyGuiConstants.CONTROLS_DELTA + new Vector2(MyGuiConstants.COMBOBOX_MEDIUM_SIZE.X / 2.0f, 0), MyGuiConstants.COMBOBOX_MEDIUM_SIZE.X, 1.0f, 8.0f, MyGuiConstants.SLIDER_BACKGROUND_COLOR, new StringBuilder(), MyGuiConstants.TEXTBOX_MEDIUM_SIZE.X, 0, MyGuiConstants.LABEL_TEXT_SCALE);
            m_joystickExponentSlider.SetValue(MyGuiManager.GetInput().GetJoystickExponent());
            m_allControls[MyGuiControlTypeEnum.General].Add(m_joystickExponentSlider);

            m_allControls[MyGuiControlTypeEnum.General].Add(new MyGuiControlLabel(this, m_controlsOriginLeft + 12 * MyGuiConstants.CONTROLS_DELTA, null, MyTextsWrapperEnum.JoystickDeadzone, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            m_joystickDeadzoneSlider = new MyGuiControlSlider(this, m_controlsOriginRight + 12 * MyGuiConstants.CONTROLS_DELTA + new Vector2(MyGuiConstants.COMBOBOX_MEDIUM_SIZE.X / 2.0f, 0), MyGuiConstants.COMBOBOX_MEDIUM_SIZE.X, 0.0f, 0.5f, MyGuiConstants.SLIDER_BACKGROUND_COLOR, new StringBuilder(), MyGuiConstants.TEXTBOX_MEDIUM_SIZE.X, 0, MyGuiConstants.LABEL_TEXT_SCALE);
            m_joystickDeadzoneSlider.SetValue(MyGuiManager.GetInput().GetJoystickDeadzone());
            m_allControls[MyGuiControlTypeEnum.General].Add(m_joystickDeadzoneSlider);




            #endregion
            AddControlsByType(MyGuiControlTypeEnum.Navigation);
            AddControlsByType(MyGuiControlTypeEnum.Systems1);
            AddControlsByType(MyGuiControlTypeEnum.Systems2);
            AddControlsByType(MyGuiControlTypeEnum.Weapons);
            AddControlsByType(MyGuiControlTypeEnum.SpecialWeapons);
            AddControlsByType(MyGuiControlTypeEnum.Editor);

            //There are no controls for this category now, so hide it completely and uncomment, when we have new comms controls
            //AddControlsByType(MyGuiInputTypeEnum.Communications);
            RefreshJoystickControlEnabling();
        }

        private void AddControlsByType(MyGuiControlTypeEnum type)
        {
            Vector2 controlsOriginRight = m_controlsOriginRight;
            controlsOriginRight.X -= 0.047f;
            m_allControls[type] = new List<MyGuiControlBase>();
            float i = 2;
            float buttonScale = 0.85f;

            MyControl[] controls = MyGuiManager.GetInput().GetGameControlsList();
            if (type == MyGuiControlTypeEnum.Editor) controls = MyGuiManager.GetInput().GetEditorControlsList();

            m_allControls[type].Add(new MyGuiControlLabel(this, controlsOriginRight + (i - 1) * MyGuiConstants.CONTROLS_DELTA + 0.0f * buttonScale * new Vector2(MyGuiConstants.TEXTBOX_MEDIUM_SIZE.X - CONTROL_BUTTON_SIZE.X / 2.0f, 0) + new Vector2(CONTROL_BUTTON_SIZE.X / 2.0f, 0) * buttonScale, null, MyTextsWrapperEnum.Keyboard, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_TOP));
            m_allControls[type].Add(new MyGuiControlLabel(this, controlsOriginRight + (i - 1) * MyGuiConstants.CONTROLS_DELTA + 0.5f * buttonScale * new Vector2(MyGuiConstants.TEXTBOX_MEDIUM_SIZE.X - CONTROL_BUTTON_SIZE.X / 2.0f, 0) + new Vector2(CONTROL_BUTTON_SIZE.X / 2.0f, 0) * buttonScale, null, MyTextsWrapperEnum.Mouse, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_TOP));
            m_allControls[type].Add(new MyGuiControlLabel(this, controlsOriginRight + (i - 1) * MyGuiConstants.CONTROLS_DELTA + 1.0f * buttonScale * new Vector2(MyGuiConstants.TEXTBOX_MEDIUM_SIZE.X - CONTROL_BUTTON_SIZE.X / 2.0f, 0) + new Vector2(CONTROL_BUTTON_SIZE.X / 2.0f, 0) * buttonScale, null, MyTextsWrapperEnum.Gamepad, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_TOP));
            m_allControls[type].Add(new MyGuiControlLabel(this, controlsOriginRight + (i - 1) * MyGuiConstants.CONTROLS_DELTA + 1.5f * buttonScale * new Vector2(MyGuiConstants.TEXTBOX_MEDIUM_SIZE.X - CONTROL_BUTTON_SIZE.X / 2.0f, 0) + new Vector2(CONTROL_BUTTON_SIZE.X / 2.0f, 0) * buttonScale, null, MyTextsWrapperEnum.AnalogAxes, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_TOP));

            controlsOriginRight.X += 0.047f;
            foreach (MyControl control in controls)
            {
                if (control.GetControlTypeEnum() == type)
                {

                    m_allControls[type].Add(new MyGuiControlLabel(this, m_controlsOriginLeft + i * MyGuiConstants.CONTROLS_DELTA, null, control.GetControlName(), MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));

                    // This is column for keyboard
                    MyGuiControlButtonParameters param = new MyGuiControlButtonParameters();
                    MyGuiControlButton keyButton = new MyGuiControlButton(this, controlsOriginRight + i * MyGuiConstants.CONTROLS_DELTA + 0 * new Vector2(MyGuiConstants.TEXTBOX_MEDIUM_SIZE.X - CONTROL_BUTTON_SIZE.X / 2.0f, 0) * buttonScale,
                        CONTROL_BUTTON_SIZE * buttonScale, MyGuiConstants.BUTTON_BACKGROUND_COLOR, new StringBuilder(control.GetControlButtonName(MyGuiInputDeviceEnum.Keyboard)), null, MyGuiConstants.BUTTON_TEXT_COLOR,
                        MyGuiConstants.BUTTON_TEXT_SCALE, delegate(MyGuiControlButton sender) { OnControlClick(param, m_keyButtonsDictionary, MyGuiInputDeviceEnum.Keyboard); }, MyGuiControlButtonTextAlignment.CENTERED, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true);
                    param.Control = control;
                    m_allControls[type].Add(keyButton);
                    m_keyButtons.Add(keyButton);

                    // This is column for mouse
                    MyGuiControlButton mouseButton = new MyGuiControlButton(this, controlsOriginRight + i * MyGuiConstants.CONTROLS_DELTA + 0.5f * new Vector2(MyGuiConstants.TEXTBOX_MEDIUM_SIZE.X - CONTROL_BUTTON_SIZE.X / 2.0f, 0) * buttonScale,
                        CONTROL_BUTTON_SIZE * buttonScale, MyGuiConstants.BUTTON_BACKGROUND_COLOR, new StringBuilder(control.GetControlButtonName(MyGuiInputDeviceEnum.Mouse)), null, MyGuiConstants.BUTTON_TEXT_COLOR,
                        MyGuiConstants.BUTTON_TEXT_SCALE, delegate(MyGuiControlButton sender) { OnControlClick(param, m_mouseButtonsDictionary, MyGuiInputDeviceEnum.Mouse); }, MyGuiControlButtonTextAlignment.CENTERED, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true);
                    param.Control = control;
                    m_allControls[type].Add(mouseButton);
                    m_mouseButtons.Add(mouseButton);

                    // This is column for joystick
                    MyGuiControlButton joyButton = new MyGuiControlButton(this, controlsOriginRight + i * MyGuiConstants.CONTROLS_DELTA + 1 * new Vector2(MyGuiConstants.TEXTBOX_MEDIUM_SIZE.X - CONTROL_BUTTON_SIZE.X / 2.0f, 0) * buttonScale,
                        CONTROL_BUTTON_SIZE * buttonScale, MyGuiConstants.BUTTON_BACKGROUND_COLOR, new StringBuilder(control.GetControlButtonName(MyGuiInputDeviceEnum.Joystick)), null, MyGuiConstants.BUTTON_TEXT_COLOR,
                        MyGuiConstants.BUTTON_TEXT_SCALE, delegate(MyGuiControlButton sender) { OnControlClick(param, m_joystickButtonsDictionary, MyGuiInputDeviceEnum.Joystick); }, MyGuiControlButtonTextAlignment.CENTERED, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true);
                    param.Control = control;
                    m_allControls[type].Add(joyButton);
                    m_joystickButtons.Add(joyButton);

                    // This is column for joystick axes
                    MyGuiControlButton joyAxis = new MyGuiControlButton(this, controlsOriginRight + i * MyGuiConstants.CONTROLS_DELTA + 1.5f * new Vector2(MyGuiConstants.TEXTBOX_MEDIUM_SIZE.X - CONTROL_BUTTON_SIZE.X / 2.0f, 0) * buttonScale,
                        CONTROL_BUTTON_SIZE * buttonScale, MyGuiConstants.BUTTON_BACKGROUND_COLOR, new StringBuilder(control.GetControlButtonName(MyGuiInputDeviceEnum.JoystickAxis)), null, MyGuiConstants.BUTTON_TEXT_COLOR,
                        MyGuiConstants.BUTTON_TEXT_SCALE, delegate(MyGuiControlButton sender) { OnControlClick(param, m_joystickAxesDictionary, MyGuiInputDeviceEnum.JoystickAxis); }, MyGuiControlButtonTextAlignment.CENTERED, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true);
                    param.Control = control;
                    m_allControls[type].Add(joyAxis);
                    m_joystickAxes.Add(joyAxis);

                    m_keyButtonsDictionary[control] = keyButton;
                    m_mouseButtonsDictionary[control] = mouseButton;
                    m_joystickButtonsDictionary[control] = joyButton;
                    m_joystickAxesDictionary[control] = joyAxis;

                    i += buttonScale;
                }
            }
        }

        private void DeactivateControls(MyGuiControlTypeEnum type)
        {
            foreach (var item in m_allControls[type])
            {
                Controls.Remove(item);
            }
        }

        private void ActivateControls(MyGuiControlTypeEnum type)
        {
            foreach (var item in m_allControls[type])
            {
                Controls.Add(item);
            }
        }

        private void AddJoysticksToComboBox()
        {
            int counter = 0;
            m_joystickCombobox.AddItem(counter++, MyTextsWrapper.Get(MyTextsWrapperEnum.Disabled));
            m_joystickCombobox.SelectItemByIndex(0);

            foreach (string joystickName in MyGuiManager.GetInput().EnumerateJoystickNames())
            {
                m_joystickCombobox.AddItem(counter, new StringBuilder(joystickName));
                if (MyGuiManager.GetInput().JoystickInstanceName == joystickName)
                {
                    m_joystickCombobox.SelectItemByIndex(counter);
                }
                counter++;
            }
        }

        private void OnSelectJoystick()
        {
            MyGuiManager.GetInput().JoystickInstanceName = m_joystickCombobox.GetSelectedIndex() == 0 ? null : m_joystickCombobox.GetSelectedValue().ToString();
            RefreshJoystickControlEnabling();
        }

        private void RefreshJoystickControlEnabling()
        {
            foreach (var button in m_joystickButtons)
                button.Enabled = (m_joystickCombobox.GetSelectedIndex() != 0);
            foreach (var axis in m_joystickAxes)
                axis.Enabled = (m_joystickCombobox.GetSelectedIndex() != 0);
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenOptionsControls";
        }

        public override bool Update(bool hasFocus)
        {
            if (m_controlTypeList.GetSelectedKey() != (int)m_currentControlType)
            {
                DeactivateControls(m_currentControlType);
                m_currentControlType = (MyGuiControlTypeEnum)m_controlTypeList.GetSelectedKey();
                ActivateControls(m_currentControlType);
            }

            if (base.Update(hasFocus) == false) return false;
            return true;
        }

        private void OnControlClick(MyGuiControlButtonParameters param, Dictionary<MyControl, MyGuiControlButton> buttonsDictionary, MyGuiInputDeviceEnum deviceType)
        {
            MyTextsWrapperEnum messageText = MyTextsWrapperEnum.AssignControlKeyboard;
            if (deviceType == MyGuiInputDeviceEnum.Mouse)
            {
                messageText = MyTextsWrapperEnum.AssignControlMouse;
            }
            else if (deviceType == MyGuiInputDeviceEnum.Joystick)
            {
                messageText = MyTextsWrapperEnum.AssignControlJoystick;
            }
            else if (deviceType == MyGuiInputDeviceEnum.JoystickAxis)
            {
                messageText = MyTextsWrapperEnum.AssignControlJoystickAxis;
            }

            MyGuiManager.AddScreen(new MyGuiControlAssignKeyMessageBox(buttonsDictionary, deviceType, param.Control, messageText));
        }

        private void OnResetDefaultsClick(MyGuiControlButton sender)
        {
            //  revert to controls when the screen was first opened and then close.
            MyGuiManager.GetInput().RevertToDefaultControls();
            //  I need refresh text on buttons. Create them again is the easiest way.
            DeactivateControls(m_currentControlType);
            AddControls();
            ActivateControls(m_currentControlType);
        }

        private void OnCancelClick(MyGuiControlButton sender)
        {
            //  revert to controls when the screen was first opened and then close.
            MyGuiManager.GetInput().RevertChanges();
            CloseScreen();
        }

        private void OnOkClick(MyGuiControlButton sender)
        {
            CloseScreenAndSave();
        }

        private void CloseScreenAndSave()
        {
            MyGuiManager.GetInput().JoystickInstanceName = m_joystickCombobox.GetSelectedIndex() == 0 ? null : m_joystickCombobox.GetSelectedValue().ToString();
            MyGuiManager.GetInput().SetMouseXInversion(m_invertMouseXCheckbox.Checked);
            MyGuiManager.GetInput().SetMouseYInversion(m_invertMouseYCheckbox.Checked);
            MyGuiManager.GetInput().SetMouseSensitivity(m_mouseSensitivitySlider.GetValue());
            MyGuiManager.GetInput().SetJoystickSensitivity(m_joystickSensitivitySlider.GetValue());
            MyGuiManager.GetInput().SetJoystickExponent(m_joystickExponentSlider.GetValue());
            MyGuiManager.GetInput().SetJoystickDeadzone(m_joystickDeadzoneSlider.GetValue());
            MyGuiManager.GetInput().SaveControls();

            //MyGuiScreenGamePlay.Static.SetControlsChange(true);

            CloseScreen();
        }

        private class MyGuiControlAssignKeyMessageBox : MyGuiScreenMessageBox
        {
            MyControl m_control;
            Dictionary<MyControl, MyGuiControlButton> m_buttonsDictionary;
            List<Toolkit.Input.Keys> m_oldPressedKeys = new List<Keys>();
            List<MyMouseButtonsEnum> m_oldPressedMouseButtons = new List<MyMouseButtonsEnum>();
            List<MyJoystickButtonsEnum> m_oldPressedJoystickButtons = new List<MyJoystickButtonsEnum>();
            List<MyJoystickAxesEnum> m_oldPressedJoystickAxes = new List<MyJoystickAxesEnum>();
            MyGuiInputDeviceEnum m_deviceType;

            public MyGuiControlAssignKeyMessageBox(Dictionary<MyControl, MyGuiControlButton> buttonsDictionary, MyGuiInputDeviceEnum deviceType, MyControl control, MyTextsWrapperEnum messageText)
                : base(MyMessageBoxType.NULL, messageText, MyTextsWrapperEnum.SelectControl, null)
            {
                DrawMouseCursor = false;
                m_isTopMostScreen = false;
                m_backgroundTexture = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\BackgroundScreen\\ProgressBackground", flags: TextureFlags.IgnoreQuality);
                m_size = new Vector2(598 / 1600f, 368 / 1200f);
                m_control = control;
                m_buttonsDictionary = buttonsDictionary;
                m_deviceType = deviceType;

                MyGuiManager.GetInput().GetListOfPressedKeys(m_oldPressedKeys);
                MyGuiManager.GetInput().GetListOfPressedMouseButtons(m_oldPressedMouseButtons);
                MyGuiManager.GetInput().GetListOfPressedJoystickButtons(m_oldPressedJoystickButtons);
                MyGuiManager.GetInput().GetListOfPressedJoystickAxes(m_oldPressedJoystickAxes);
                m_interferenceVideoColor = Vector4.One;
                m_closeOnEsc = false;
                m_screenCanHide = true;
                //Controls.Add(new MyGuiControlRotatingWheel(this, new Vector2(0, 0.05f), Vector4.One, MyGuiConstants.ROTATING_WHEEL_DEFAULT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER));
            }

            public override void HandleInput(MyGuiInput input, bool receivedFocusInThisUpdate)
            {
                base.HandleInput(input, receivedFocusInThisUpdate);
                if (input.IsNewKeyPress(Keys.Escape))
                {
                    Canceling();
                }

                //  Do nothing if base.HandleInput closing this screen right now
                if (m_state == MyGuiScreenState.CLOSING || m_state == MyGuiScreenState.HIDING) return;

                if (m_deviceType == MyGuiInputDeviceEnum.Keyboard)
                {
                    List<Toolkit.Input.Keys> pressedKeys = new List<Toolkit.Input.Keys>();
                    input.GetListOfPressedKeys(pressedKeys);

                    //  don't assign keys that were pressed when we arrived in the menu
                    foreach (var key in pressedKeys)
                    {
                        if (!m_oldPressedKeys.Contains(key))
                        {
                            if (key == Keys.Control) continue; //there is always LeftControl or RightControl pressed with this
                            if (key == Keys.Shift) continue; //there is always LeftShift or RightShift pressed with this

                            MyAudio.AddCue2D(MySoundCuesEnum.GuiMouseClick);

                            if (!MyGuiInput.IsKeyValid((Keys)key))
                            {
                                ShowControlIsNotValidMessageBox(); break;
                            }

                            MyControl ctrl = input.GetControl((Keys)key, m_control.GetGameControlTypeEnum());
                            if (ctrl != null)
                            {
                                if (ctrl.Equals(m_control)) { CloseScreen(); return; }
                                ShowControlIsAlreadyAssigned(ctrl); break;
                            }
                            m_control.SetControl((Keys)key);
                            m_buttonsDictionary[m_control].SetText(new StringBuilder(m_control.GetControlButtonName(MyGuiInputDeviceEnum.Keyboard)));
                            CloseScreen(); return;
                        }
                    }
                    m_oldPressedKeys = pressedKeys;
                }
                else if (m_deviceType == MyGuiInputDeviceEnum.Mouse)
                {
                    var pressedMouseButtons = new List<MyMouseButtonsEnum>();
                    input.GetListOfPressedMouseButtons(pressedMouseButtons);

                    //  don't assign buttons that were pressed when we arrived in the menu
                    foreach (var button in pressedMouseButtons)
                    {
                        if (!m_oldPressedMouseButtons.Contains(button))
                        {
                            MyAudio.AddCue2D(MySoundCuesEnum.GuiMouseClick);
                            if (!MyGuiInput.IsMouseButtonValid(button)) { ShowControlIsNotValidMessageBox(); break; }

                            MyControl ctrl = input.GetControl(button, m_control.GetGameControlTypeEnum());
                            if (ctrl != null)
                            {
                                if (ctrl.Equals(m_control)) { CloseScreen(); return; }
                                ShowControlIsAlreadyAssigned(ctrl); break;
                            }
                            m_control.SetControl(button);
                            m_buttonsDictionary[m_control].SetText(new StringBuilder(m_control.GetControlButtonName(MyGuiInputDeviceEnum.Mouse)));
                            CloseScreen(); return;
                        }
                    }
                    m_oldPressedMouseButtons = pressedMouseButtons;
                }
                else if (m_deviceType == MyGuiInputDeviceEnum.Joystick)
                {
                    var pressedJoystickButtons = new List<MyJoystickButtonsEnum>();
                    input.GetListOfPressedJoystickButtons(pressedJoystickButtons);

                    //  don't assign buttons that were pressed when we arrived in the menu
                    foreach (var button in pressedJoystickButtons)
                    {
                        if (!m_oldPressedJoystickButtons.Contains(button))
                        {
                            MyAudio.AddCue2D(MySoundCuesEnum.GuiMouseClick);
                            if (!MyGuiInput.IsJoystickButtonValid(button)) { ShowControlIsNotValidMessageBox(); break; }

                            MyControl ctrl = input.GetControl(button, m_control.GetGameControlTypeEnum());
                            if (ctrl != null)
                            {
                                if (ctrl.Equals(m_control)) { CloseScreen(); return; }
                                ShowControlIsAlreadyAssigned(ctrl); break;
                            }
                            m_control.SetControl(button);
                            m_buttonsDictionary[m_control].SetText(new StringBuilder(m_control.GetControlButtonName(MyGuiInputDeviceEnum.Joystick)));
                            CloseScreen(); return;
                        }
                    }
                    m_oldPressedJoystickButtons = pressedJoystickButtons;
                }
                else if (m_deviceType == MyGuiInputDeviceEnum.JoystickAxis)
                {
                    var pressedJoystickAxes = new List<MyJoystickAxesEnum>();
                    input.GetListOfPressedJoystickAxes(pressedJoystickAxes);

                    //  don't assign axes that were pressed when we arrived in the menu
                    foreach (var axis in pressedJoystickAxes)
                    {
                        if (!m_oldPressedJoystickAxes.Contains(axis))
                        {
                            MyAudio.AddCue2D(MySoundCuesEnum.GuiMouseClick);
                            if (!MyGuiInput.IsJoystickAxisValid(axis)) { ShowControlIsNotValidMessageBox(); break; }

                            MyControl ctrl = input.GetControl(axis, m_control.GetGameControlTypeEnum());
                            if (ctrl != null)
                            {
                                if (ctrl.Equals(m_control)) { CloseScreen(); return; }
                                ShowControlIsAlreadyAssigned(ctrl); break;
                            }
                            m_control.SetControl(axis);
                            m_buttonsDictionary[m_control].SetText(new StringBuilder(m_control.GetControlButtonName(MyGuiInputDeviceEnum.JoystickAxis)));
                            CloseScreen(); return;
                        }
                    }
                    m_oldPressedJoystickAxes = pressedJoystickAxes;
                }
            }

            private void ShowControlIsNotValidMessageBox()
            {
                MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, MyTextsWrapperEnum.ControlIsNotValid, MyTextsWrapperEnum.CanNotAssignControl, MyTextsWrapperEnum.Ok, null));
            }

            private void ShowControlIsAlreadyAssigned(MyControl control)
            {
                MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR,
                    new StringBuilder(string.Format(MyTextsWrapper.Get(MyTextsWrapperEnum.ControlAlreadyAssigned).ToString(), control.GetControlButtonName(m_deviceType), MyTextsWrapper.Get(control.GetControlName()).ToString())),
                    MyTextsWrapper.Get(MyTextsWrapperEnum.CanNotAssignControl), MyTextsWrapperEnum.Yes, MyTextsWrapperEnum.No,
                    delegate(MyGuiScreenMessageBoxCallbackEnum r)
                    {
                        AssignAlreadyAssignedCommand(r, control);
                    }));
            }

            private void AssignAlreadyAssignedCommand(MyGuiScreenMessageBoxCallbackEnum r, MyControl control)
            {
                if (r == MyGuiScreenMessageBoxCallbackEnum.YES)
                {
                    switch (m_deviceType)
                    {
                        case MyGuiInputDeviceEnum.Keyboard:
                            m_control.SetControl(control.GetKeyboardControl());
                            control.SetControl(Keys.None);
                            break;
                        case MyGuiInputDeviceEnum.Mouse:
                            m_control.SetControl(control.GetMouseControl());
                            control.SetControl(MyMouseButtonsEnum.None);
                            break;
                        case MyGuiInputDeviceEnum.Joystick:
                            m_control.SetControl(control.GetJoystickControl());
                            control.SetControl(MyJoystickButtonsEnum.None);
                            break;
                        case MyGuiInputDeviceEnum.JoystickAxis:
                            m_control.SetControl(control.GetJoystickAxisControl());
                            control.SetControl(MyJoystickAxesEnum.None);
                            break;
                    }
                    m_buttonsDictionary[m_control].SetText(new StringBuilder(m_control.GetControlButtonName(m_deviceType)));
                    if (m_buttonsDictionary.ContainsKey(control))
                    {
                        m_buttonsDictionary[control].SetText(new StringBuilder(control.GetControlButtonName(m_deviceType)));
                    }
                    CloseScreen();
                }
                else
                {
                    MyGuiManager.GetInput().GetListOfPressedKeys(m_oldPressedKeys);
                    MyGuiManager.GetInput().GetListOfPressedMouseButtons(m_oldPressedMouseButtons);
                    MyGuiManager.GetInput().GetListOfPressedJoystickButtons(m_oldPressedJoystickButtons);
                    MyGuiManager.GetInput().GetListOfPressedJoystickAxes(m_oldPressedJoystickAxes);
                }
            }

            public override bool CloseScreen()
            {
                DrawMouseCursor = true;
                return base.CloseScreen();
            }
        }

        private class MyGuiControlButtonParameters
        {
            public MyControl Control { get; set; }
        }
    }
}