#region Using
using System;
using System.Reflection;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Render;
using MinerWars.AppCode.Game.Effects;
using MinerWars.CommonLIB.AppCode.Utils;
using SysUtils;
using System.Collections.Generic;
#endregion

//  Abstract class (screen) for all debug / developer screens

namespace MinerWars.AppCode.Game.GUI
{
    abstract class MyGuiScreenDebugBase : MyGuiScreenBase
    {
        static Vector4 m_defaultColor = new Vector4(1f, 1f, 0f, 1f);

        protected Vector2 m_currentPosition;
        protected float m_checkBoxOffset = 0.015f;
        protected float m_scale = 1.0f;
        protected float m_buttonXOffset = 0;

        float m_maxWidth = 0;

        protected MyGuiScreenDebugBase(Vector4? backgroundColor, bool isTopMostScreen) :
            this(new Vector2( MyGuiManager.GetMaxMouseCoord().X - 0.16f, 0.5f), new Vector2(0.32f, 1.0f), backgroundColor, isTopMostScreen)
        {
        }

        protected MyGuiScreenDebugBase(Vector2 position, Vector2? size, Vector4? backgroundColor, bool isTopMostScreen) :
            base(position, backgroundColor, size, isTopMostScreen, null)
        {
            m_screenCanHide = false;
            m_canCloseInCloseAllScreenCalls = false;
            m_canShareInput = true;
        }


        #region CheckBox

        protected MyGuiControlCheckbox addCheckBox(StringBuilder text, bool enabled = true, List<MyGuiControlBase> controlGroup = null, Vector4? color = null)
        {
            MyGuiControlLabel label = new MyGuiControlLabel(this, m_currentPosition, null, text, color ?? m_defaultColor, MyGuiConstants.LABEL_TEXT_SCALE * MyGuiConstants.DEBUG_LABEL_TEXT_SCALE * m_scale,
                                                            MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);

            float labelWidth = label.GetTextSize().Size.X + 0.02f;
            m_maxWidth = Math.Max(m_maxWidth, labelWidth);
            label.Enabled = enabled;
            Controls.Add(label);


            Vector2? m = this.GetSize();

            Vector2 checkBoxSize = MyGuiConstants.CHECKBOX_SIZE * m_scale;
            MyGuiControlCheckbox checkBox = new MyGuiControlCheckbox(this, m_currentPosition + new Vector2(m.Value.X - 2 * checkBoxSize.X - m_checkBoxOffset, 0),
                checkBoxSize, false, 0.8f * (color ?? m_defaultColor));
            checkBox.Enabled = enabled;

            Controls.Add(checkBox);

            m_currentPosition.Y += 0.04f * m_scale;

            if (controlGroup != null)
            {
                controlGroup.Add(label);
                controlGroup.Add(checkBox);
            }

            return checkBox;
        }

        protected MyGuiControlCheckbox AddCheckBox(StringBuilder text, MyGuiScreenDebugBase screen, List<MyGuiControlBase> controlGroup = null, Vector4? color = null)
        {
            MyGuiControlCheckbox checkBox = addCheckBox(text, true, controlGroup, color);

            checkBox.Checked = screen.GetState() == MyGuiScreenState.OPENED;
            checkBox.UserData = screen;

            checkBox.OnCheck = delegate(MyGuiControlCheckbox sender)
            {
                MyGuiScreenDebugBase screenSender = sender.UserData as MyGuiScreenDebugBase;
                if (sender.Checked)
                {
                    MyGuiManager.AddScreen(screenSender);
                    screenSender.SetState(MyGuiScreenState.OPENING);
                    screenSender.LoadContent();
                    screenSender.RecreateControls(false);
                }
                else
                {
                    screenSender.CloseScreen();
                }
            };

            return checkBox;
        }

        protected MyGuiControlCheckbox AddCheckBox(MyTextsWrapperEnum textEnum, bool checkedState, MyGuiControlCheckbox.OnCheckBoxCheckCallback checkBoxChange, bool enabled = true, List<MyGuiControlBase> controlGroup = null, Vector4? color = null)
        {
            return AddCheckBox(MyTextsWrapper.Get(textEnum), checkedState, checkBoxChange, enabled, controlGroup, color);
        }

        protected MyGuiControlCheckbox AddCheckBox(StringBuilder text, bool checkedState, MyGuiControlCheckbox.OnCheckBoxCheckCallback checkBoxChange, bool enabled = true, List<MyGuiControlBase> controlGroup = null, Vector4? color = null)
        {
            MyGuiControlCheckbox checkBox = addCheckBox(text, enabled, controlGroup, color);
            checkBox.Checked = checkedState;
            checkBox.OnCheck = checkBoxChange;
            return checkBox;
        }

        protected MyGuiControlCheckbox AddCheckBox(MyTextsWrapperEnum textEnum, object instance, MemberInfo memberInfo, bool enabled = true, List<MyGuiControlBase> controlGroup = null, Vector4? color = null)
        {
            return AddCheckBox(MyTextsWrapper.Get(textEnum), instance, memberInfo, enabled, controlGroup, color);
        }

        protected MyGuiControlCheckbox AddCheckBox(StringBuilder text, object instance, MemberInfo memberInfo, bool enabled = true, List<MyGuiControlBase> controlGroup = null, Vector4? color = null)
        {
            MyGuiControlCheckbox checkBox = addCheckBox(text, enabled, controlGroup, color);

            if (memberInfo is PropertyInfo)
            {
                PropertyInfo property = (PropertyInfo)memberInfo;
                checkBox.Checked = (bool)property.GetValue(instance, new object[0]);
                checkBox.UserData = new Tuple<object, PropertyInfo>(instance, property);
                checkBox.OnCheck = delegate(MyGuiControlCheckbox sender)
                {
                    Tuple<object, PropertyInfo> tuple = sender.UserData as Tuple<object, PropertyInfo>;
                    tuple.Item2.SetValue(tuple.Item1, sender.Checked, new object[0]);
                };
            }
            else
                if (memberInfo is FieldInfo)
                {
                    FieldInfo field = (FieldInfo)memberInfo;
                    checkBox.Checked = (bool)field.GetValue(instance);
                    checkBox.UserData = new Tuple<object, FieldInfo>(instance, field);
                    checkBox.OnCheck = delegate(MyGuiControlCheckbox sender)
                    {
                        Tuple<object, FieldInfo> tuple = sender.UserData as Tuple<object, FieldInfo>;
                        tuple.Item2.SetValue(tuple.Item1, sender.Checked);
                    };
                }
                else
                {
                    System.Diagnostics.Debug.Assert(false, "Unknown type of memberInfo");
                }

            return checkBox;
        }

        #endregion

        #region Slider

        protected MyGuiControlSlider addSlider(StringBuilder text, float valueMin, float valueMax, Vector4? color = null)
        {
            MyGuiControlLabel label = new MyGuiControlLabel(this, m_currentPosition, null, text, color ?? m_defaultColor, MyGuiConstants.LABEL_TEXT_SCALE * 0.8f * m_scale,
                                                            MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);
            float labelWidth = label.GetTextSize().Size.X + 0.02f;
            m_maxWidth = Math.Max(m_maxWidth, labelWidth);
            Controls.Add(label);

            m_currentPosition.Y += 0.04f * m_scale;

            MyGuiControlSlider slider = new MyGuiControlSlider(this, m_currentPosition + new Vector2(0.1f, 0) * m_scale, 0.2f, valueMin, valueMax, color ?? m_defaultColor,
                new System.Text.StringBuilder(" {0}"), 0.1f, 3, 0.65f * m_scale, m_scale);

            Controls.Add(slider);
            m_currentPosition.Y += 0.05f * m_scale;

            return slider;
        }

        protected void AddSlider(StringBuilder text, float value, float valueMin, float valueMax, MyGuiControlSlider.OnSliderChangeCallback valueChange, Vector4? color = null)
        {
            MyGuiControlSlider slider = addSlider(text, valueMin, valueMax, color);
            slider.SetValue(value);
            slider.OnChange = valueChange;
        }

        protected MyGuiControlSlider AddSlider(StringBuilder text, float valueMin, float valueMax, object instance, MemberInfo memberInfo, Vector4? color = null)
        {
            MyGuiControlSlider slider = addSlider(text, valueMin, valueMax, color);
            
            if (memberInfo is PropertyInfo)
            {
                PropertyInfo property = (PropertyInfo)memberInfo;

                slider.SetValue((float)property.GetValue(instance, new object[0]));
                slider.UserData = new Tuple<object, PropertyInfo>(instance, property);
                slider.OnChange = delegate(MyGuiControlSlider sender)
                {
                    Tuple<object, PropertyInfo> tuple = sender.UserData as Tuple<object, PropertyInfo>;
                    tuple.Item2.SetValue(tuple.Item1, sender.GetValue(), new object[0]);
                };
            }
            else
            if (memberInfo is FieldInfo)
            {
                FieldInfo field = (FieldInfo)memberInfo;

                slider.SetValue((float)field.GetValue(instance));
                slider.UserData = new Tuple<object, FieldInfo>(instance, field);
                slider.OnChange = delegate(MyGuiControlSlider sender)
                {
                    Tuple<object, FieldInfo> tuple = sender.UserData as Tuple<object, FieldInfo>;
                    tuple.Item2.SetValue(tuple.Item1, sender.GetValue());
                };
            }
            else
            {
                System.Diagnostics.Debug.Assert(false, "Unknown type of memberInfo");
            }

            return slider;
        }


        #endregion

        #region Label

        protected void AddLabel(StringBuilder text, Vector4 color, float scale, List<MyGuiControlBase> controlGroup = null, MyGuiFont font = null)
        {
            MyGuiControlLabel label = new MyGuiControlLabel(this, m_currentPosition, null, text, color, MyGuiConstants.LABEL_TEXT_SCALE * MyGuiConstants.DEBUG_LABEL_TEXT_SCALE * scale * m_scale,
                                                            MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER, font);
            float labelWidth = label.GetTextSize().Size.X + 0.02f;
            m_maxWidth = Math.Max(m_maxWidth, labelWidth);
            Controls.Add(label);

            m_currentPosition.Y += 0.04f * m_scale;

            if (controlGroup != null)
                controlGroup.Add(label);
        }

        #endregion

        #region Color


        protected MyGuiControlColor addColor(StringBuilder text)
        {
            MyGuiControlColor colorControl = new MyGuiControlColor(this, text,  m_currentPosition, new Vector2(0.3f, 0.1f), 1.0f, Color.White);
            Controls.Add(colorControl);
            return colorControl;
        }

        protected void AddColor(StringBuilder text, object instance, MemberInfo memberInfo)
        {
            MyGuiControlColor colorControl = addColor(text);

            if (memberInfo is PropertyInfo)
            {
                PropertyInfo property = (PropertyInfo)memberInfo;
                var val = property.GetValue(instance, new object[0]);
                if (val is Color)
                    colorControl.SetColor((Color)val);
                else
                    if (val is Vector3)
                        colorControl.SetColor((Vector3)val);
                    else
                        if (val is Vector4)
                            colorControl.SetColor((Vector4)val);

                colorControl.UserData = new Tuple<object, PropertyInfo>(instance, property);
                colorControl.OnChange = delegate(MyGuiControlColor sender)
                {
                    Tuple<object, PropertyInfo> tuple = sender.UserData as Tuple<object, PropertyInfo>;
                    if (tuple.Item2.MemberType.GetType() == typeof(Color))
                    {
                        tuple.Item2.SetValue(tuple.Item1, sender.GetColor(), new object[0]);
                    }
                    else
                        if (tuple.Item2.MemberType.GetType() == typeof(Vector3))
                    {
                        tuple.Item2.SetValue(tuple.Item1, sender.GetColor().ToVector3(), new object[0]);
                    }
                    else
                    if (tuple.Item2.MemberType.GetType() == typeof(Vector4))
                    {
                        tuple.Item2.SetValue(tuple.Item1, sender.GetColor().ToVector4(), new object[0]);
                    }
                };
            }
            else
                if (memberInfo is FieldInfo)
                {
                    FieldInfo field = (FieldInfo)memberInfo;
                    var val = field.GetValue(instance);
                    if (val is Color)
                        colorControl.SetColor((Color)val);
                    else
                        if (val is Vector3)
                            colorControl.SetColor((Vector3)val);
                        else
                            if (val is Vector4)
                                colorControl.SetColor((Vector4)val);

                    colorControl.UserData = new Tuple<object, FieldInfo>(instance, field);
                    colorControl.OnChange = delegate(MyGuiControlColor sender)
                    {
                        Tuple<object, FieldInfo> tuple = sender.UserData as Tuple<object, FieldInfo>;
                        if (tuple.Item2.FieldType == typeof(Color))
                        {
                            tuple.Item2.SetValue(tuple.Item1, sender.GetColor());
                        }
                        else
                            if (tuple.Item2.FieldType == typeof(Vector3))
                            {
                                tuple.Item2.SetValue(tuple.Item1, sender.GetColor().ToVector3());
                            }
                            else
                                if (tuple.Item2.FieldType == typeof(Vector4))
                                {
                                    tuple.Item2.SetValue(tuple.Item1, sender.GetColor().ToVector4());
                                }
                    };
                }
                else
                {
                    System.Diagnostics.Debug.Assert(false, "Unknown type of memberInfo");
                }

            m_currentPosition.Y += 0.08f * m_scale;
        }

        #endregion

        #region Button

        protected MyGuiControlButton AddButton(StringBuilder text, MyGuiControlButton.OnButtonClick onClick, List<MyGuiControlBase> controlGroup = null, MyGuiControlButtonTextAlignment textAlign = MyGuiControlButtonTextAlignment.CENTERED, Vector4? textColor = null, Vector2? size = null)
        {
            MyGuiControlButton button = new MyGuiControlButton(this, new Vector2(m_buttonXOffset, m_currentPosition.Y), size ?? new Vector2(0.20f, 0.03f), Vector4.One, text, null, textColor ?? m_defaultColor, MyGuiConstants.LABEL_TEXT_SCALE * MyGuiConstants.DEBUG_BUTTON_TEXT_SCALE * m_scale, onClick, textAlign, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true);

            Controls.Add(button);

            m_currentPosition.Y += 0.04f * m_scale;

            if (controlGroup != null)
                controlGroup.Add(button);

            return button;
        }

        protected MyGuiControlCombobox AddCombo(List<MyGuiControlBase> controlGroup = null, MyGuiControlButtonTextAlignment textAlign = MyGuiControlButtonTextAlignment.CENTERED, Vector4? textColor = null, Vector2? size = null)
        {
            MyGuiControlCombobox combo = new MyGuiControlCombobox(this, new Vector2(m_buttonXOffset, m_currentPosition.Y), MyGuiControlPreDefinedSize.MEDIUM, Vector4.One, 0.7f);
            Controls.Add(combo);

            m_currentPosition.Y += 0.04f * m_scale;

            if (controlGroup != null)
                controlGroup.Add(combo);

            return combo;
        }

        protected MyGuiControlListbox AddListbox(List<MyGuiControlBase> controlGroup = null, Vector2? size = null)
        {
            int itemCount = 15;
            var s = size ?? new Vector2(0.20f, 0.025f);
            var pos = new Vector2(m_buttonXOffset, m_currentPosition.Y);
            pos.Y += s.Y * itemCount / 2;
            MyGuiControlListbox listbox = new MyGuiControlListbox(this, pos, s, Vector4.One, new StringBuilder(), 0.7f, 1, itemCount, 1, false, true, false);
            Controls.Add(listbox);

            m_currentPosition.Y += 0.04f * m_scale;

            if (controlGroup != null)
                controlGroup.Add(listbox);

            return listbox;
        }

        #endregion


        public override bool Draw(float backgroundFadeAlpha)
        {
            if (MyGuiManager.IsDebugScreenEnabled() == false) return false;
            if (base.Draw(backgroundFadeAlpha) == false) return false;
            return true;
        }
    }
}
