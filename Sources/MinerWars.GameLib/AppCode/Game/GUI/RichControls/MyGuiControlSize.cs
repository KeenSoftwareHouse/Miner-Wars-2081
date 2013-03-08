using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWarsMath;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Localization;
using SysUtils.Utils;

namespace MinerWars.AppCode.Game.GUI.RichControls
{
    [Flags]
    enum MyGuiSizeEnumFlags 
    {
        Description = 1 << 0,
        Slider = 1 << 1,
        TextBox = 1 << 2,
        
        All = Description | Slider | TextBox,
    }

    delegate void OnValueChange(MyGuiControlBase sender);

    class MyGuiControlSize : MyGuiControlParent
    {
        private MyGuiControlLabel m_descriptionLabel;
        private MyGuiControlLabel m_valueLabel;
        private MyGuiControlSlider m_valueSlider;
        private MyGuiControlTextbox m_valueTextBox;

        private float m_value;
        private float m_minValue;
        private float m_maxValue;
        private MyGuiSizeEnumFlags m_flags;
        private StringBuilder m_description;
        private float m_offset;

        public MyGuiControlSize(IMyGuiControlsParent parent, Vector2 position, Vector2 size, Vector4 backgroundColor, StringBuilder toolTip, float value, float minValue, float maxValue, StringBuilder description, MyGuiSizeEnumFlags flags, float offset)
            : base(parent, position, size, backgroundColor, toolTip, null) 
        {
            m_minValue = minValue;
            m_maxValue = maxValue;
            m_flags = flags;
            m_description = description;
            m_offset = offset;
            InitControls();
            SetValue(value);
        }

        public event OnValueChange OnValueChange;

        private void InitControls() 
        {
            Vector2 position = new Vector2(-m_size.Value.X / 2f, 0f);
            Vector2 controlsDelta = new Vector2(m_offset, 0f);

            // create description label
            if ((m_flags & MyGuiSizeEnumFlags.Description) > 0) 
            {
                m_descriptionLabel = new MyGuiControlLabel(this, position, null, m_description, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);
                Controls.Add(m_descriptionLabel);
                position += controlsDelta;
            }

            // create value slider
            if ((m_flags & MyGuiSizeEnumFlags.Slider) > 0)
            {
                // slider
                m_valueSlider = new MyGuiControlSlider(this, position, MyGuiConstants.SLIDER_WIDTH, m_minValue, m_maxValue, MyGuiConstants.SLIDER_BACKGROUND_COLOR,
                    new StringBuilder(), MyGuiConstants.SLIDER_WIDTH_LABEL, 0, MyGuiConstants.LABEL_TEXT_SCALE * 0.85f);
                m_valueSlider.OnChange = OnSliderChange;
                Controls.Add(m_valueSlider);
                position += controlsDelta;                                
            }

            // create value textbox
            if ((m_flags & MyGuiSizeEnumFlags.TextBox) > 0)
            {
                m_valueTextBox = new MyGuiControlTextbox(this, position, MyGuiControlPreDefinedSize.MEDIUM, string.Empty, 9, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiControlTextboxType.DIGITS_ONLY);
                m_valueTextBox.TextChanged = OnTextBoxChange;
                Controls.Add(m_valueTextBox);
            }
            else if((m_flags & MyGuiSizeEnumFlags.Slider) > 0)
            {
                // slider value label
                m_valueLabel = new MyGuiControlLabel(this, position, null, MyTextsWrapperEnum.None, MyGuiConstants.LABEL_TEXT_COLOR,
                    MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);
                Controls.Add(m_valueLabel);
            }
        }

        private void OnSliderChange(MyGuiControlSlider sender) 
        {
            SetValue(sender, sender.GetValue());
        }        

        private void SetValue(object sender, float newValue) 
        {
            m_value = newValue;
            //string valueString = m_value.ToString("#,###0.000", System.Globalization.CultureInfo.InvariantCulture);
            //string valueString = MyValueFormatter.GetFormatedDecimal(Convert.ToDecimal(m_value), 1);
            string valueString = MyValueFormatter.GetFormatedFloat(m_value, 1, "");
            if (m_valueSlider != null && m_valueSlider != sender) 
            {
                m_valueSlider.OnChange = null;
                m_valueSlider.SetValue(m_value);
                m_valueSlider.OnChange = OnSliderChange;                
            }            
            if (m_valueTextBox != null && m_valueTextBox != sender) 
            {
                m_valueTextBox.TextChanged = null;
                m_valueTextBox.Text = valueString;
                m_valueTextBox.TextChanged = OnTextBoxChange;
            }
            if (m_valueLabel != null) 
            {
                m_valueLabel.UpdateText(valueString);
            }
            if (OnValueChange != null) 
            {
                OnValueChange(this);
            }
        }

        public void SetValue(float value) 
        {
            SetValue(null, value);
        }

        public float GetValue() 
        {
            return m_value;
        }

        public void UpdateDescription(string newText) 
        {
            m_descriptionLabel.UpdateText(newText);
        }

        private void OnTextBoxChange(object sender, EventArgs args) 
        {
            string textValue = m_valueTextBox.Text;
            if (!string.IsNullOrEmpty(textValue))
            {
                float newValue = (float)(MyValueFormatter.GetDecimalFromString(textValue, 1));
                if (newValue >= m_minValue && newValue <= m_maxValue)
                {
                    SetValue(m_valueTextBox, newValue);
                }
                else 
                {
                    SetValue(m_valueSlider, m_value);
                }
            }
        }
    }
}
