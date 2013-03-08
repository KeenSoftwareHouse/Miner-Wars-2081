using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWarsMath;
using MinerWars.AppCode.Game.Entities.Prefabs;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Localization;

namespace MinerWars.AppCode.Game.GUI.ScreenEditor
{
    class MyGuiControlUseProperties : MyGuiControlParent
    {
        private MyGuiControlCheckbox m_useSoloCheckbox;
        private MyGuiControlCheckbox m_useHUBCheckbox;
        private MyGuiControlCheckbox m_hackSoloCheckbox;
        private MyGuiControlCheckbox m_hackHUBCheckbox;
        private MyGuiControlCheckbox m_isHackedCheckbox;
        private MyGuiControlTextbox m_hackingTimeTextbox;
        private MyGuiControlCombobox m_hackingLevelCombobox;
        private Vector4 m_labelColor;

        private MyUseProperties m_useProperties;

        public MyGuiControlUseProperties(IMyGuiControlsParent parent, Vector2 position, MyUseProperties useProperties, Vector4 labelColor)
            : base(parent, position, new Vector2(0.7f, 0.35f), Vector4.Zero, null, MyGuiManager.GetBlankTexture()) 
        {
            m_labelColor = labelColor;
            m_useProperties = useProperties;            
            InitControls();
        }

        private void InitControls() 
        {
            Vector2 controlsLeftPosition = -m_size.Value / 2f;
            Vector2 controlsRightPosition = controlsLeftPosition + new Vector2(m_size.Value.X / 2f, 0f);
            Vector2 labelOffset = new Vector2(0.25f, 0f);

            // use solo
            Controls.Add(new MyGuiControlLabel(this, controlsLeftPosition, null, MyTextsWrapperEnum.UseSolo, m_labelColor, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            m_useSoloCheckbox = new MyGuiControlCheckbox(this, controlsLeftPosition + labelOffset, MyGuiConstants.CHECKBOX_SIZE, (m_useProperties.UseType & MyUseType.Solo) != 0, MyGuiConstants.CHECKBOX_BACKGROUND_COLOR);
            Controls.Add(m_useSoloCheckbox);                        
            controlsLeftPosition += MyGuiConstants.CONTROLS_DELTA/* + new Vector2(m_useSoloCheckbox.GetSize().Value.Y)*/;
            if ((m_useProperties.UseMask & MyUseType.Solo) == 0) 
            {
                m_useSoloCheckbox.Enabled = false;
                m_useSoloCheckbox.Checked = false;
            }

            // hack solo
            Controls.Add(new MyGuiControlLabel(this, controlsRightPosition, null, MyTextsWrapperEnum.HackSolo, m_labelColor, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            m_hackSoloCheckbox = new MyGuiControlCheckbox(this, controlsRightPosition + labelOffset, MyGuiConstants.CHECKBOX_SIZE, (m_useProperties.HackType & MyUseType.Solo) != 0, MyGuiConstants.CHECKBOX_BACKGROUND_COLOR);
            Controls.Add(m_hackSoloCheckbox);                        
            controlsRightPosition += MyGuiConstants.CONTROLS_DELTA/* + new Vector2(m_hackSoloCheckbox.GetSize().Value.Y)*/;
            if ((m_useProperties.HackMask & MyUseType.Solo) == 0)
            {
                m_hackSoloCheckbox.Enabled = false;
                m_hackSoloCheckbox.Checked = false;
            }
            
            // use from HUB
            Controls.Add(new MyGuiControlLabel(this, controlsLeftPosition, null, MyTextsWrapperEnum.UseFromHUB, m_labelColor, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            m_useHUBCheckbox = new MyGuiControlCheckbox(this, controlsLeftPosition + labelOffset, MyGuiConstants.CHECKBOX_SIZE, (m_useProperties.UseType & MyUseType.FromHUB) != 0, MyGuiConstants.CHECKBOX_BACKGROUND_COLOR);
            Controls.Add(m_useHUBCheckbox);                                
            controlsLeftPosition += MyGuiConstants.CONTROLS_DELTA;
            if ((m_useProperties.UseMask & MyUseType.FromHUB) == 0)
            {
                m_useHUBCheckbox.Enabled = false;
                m_useHUBCheckbox.Checked = false;
            }

            // hack from HUB
            Controls.Add(new MyGuiControlLabel(this, controlsRightPosition, null, MyTextsWrapperEnum.HackFromHUB, m_labelColor, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            m_hackHUBCheckbox = new MyGuiControlCheckbox(this, controlsRightPosition + labelOffset, MyGuiConstants.CHECKBOX_SIZE, (m_useProperties.HackType & MyUseType.FromHUB) != 0, MyGuiConstants.CHECKBOX_BACKGROUND_COLOR);
            Controls.Add(m_hackHUBCheckbox);                                
            controlsRightPosition += MyGuiConstants.CONTROLS_DELTA;
            if ((m_useProperties.HackMask & MyUseType.FromHUB) == 0)
            {
                m_hackHUBCheckbox.Enabled = false;
                m_hackHUBCheckbox.Checked = false;
            }

            // is hacked
            Controls.Add(new MyGuiControlLabel(this, controlsLeftPosition, null, MyTextsWrapperEnum.IsHacked, m_labelColor, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            m_isHackedCheckbox = new MyGuiControlCheckbox(this, controlsLeftPosition + labelOffset, MyGuiConstants.CHECKBOX_SIZE, m_useProperties.IsHacked, MyGuiConstants.CHECKBOX_BACKGROUND_COLOR);
            Controls.Add(m_isHackedCheckbox);                        
            controlsLeftPosition += MyGuiConstants.CONTROLS_DELTA;

            // move the two last controls to the right a bit.
            labelOffset.X = labelOffset.X + 0.075f;

            // hacking time
            Controls.Add(new MyGuiControlLabel(this, controlsLeftPosition, null, MyTextsWrapperEnum.HackingTime, m_labelColor, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            m_hackingTimeTextbox = new MyGuiControlTextbox(this, controlsLeftPosition + labelOffset, MyGuiControlPreDefinedSize.MEDIUM, m_useProperties.HackingTime.ToString(), 6, MyGuiConstants.DEFAULT_CONTROL_NONACTIVE_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiControlTextboxType.DIGITS_ONLY);
            Controls.Add(m_hackingTimeTextbox);            
            controlsLeftPosition += MyGuiConstants.CONTROLS_DELTA;

            // hacking level
            Controls.Add(new MyGuiControlLabel(this, controlsLeftPosition, null, MyTextsWrapperEnum.HackingLevel, m_labelColor, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            m_hackingLevelCombobox = new MyGuiControlCombobox(this, controlsLeftPosition + labelOffset, MyGuiControlPreDefinedSize.MEDIUM, MyGuiConstants.COMBOBOX_BACKGROUND_COLOR, MyGuiConstants.COMBOBOX_TEXT_SCALE);
            Controls.Add(m_hackingLevelCombobox);
            for (int i = 1; i <= 5; i++) 
            {
                m_hackingLevelCombobox.AddItem(i, new StringBuilder(i.ToString()));
            }
            m_hackingLevelCombobox.SelectItemByKey(m_useProperties.HackingLevel);
        }

        public bool Validate(ref List<StringBuilder> errorMessages) 
        {
            bool result = true;
            if (string.IsNullOrEmpty(m_hackingTimeTextbox.Text)) 
            {
                errorMessages.Add(MyTextsWrapper.Get(MyTextsWrapperEnum.MessageYouMustSetHackingTime));
                result = false;
            }

            return result;
        }

        public void SaveTo(MyUseProperties useProperties) 
        {
            useProperties.UseType = MyUseType.None;
            if (m_useSoloCheckbox.Checked) 
            {
                useProperties.UseType |= MyUseType.Solo;
            }
            if (m_useHUBCheckbox != null && m_useHUBCheckbox.Checked)
            {
                useProperties.UseType |= MyUseType.FromHUB;
            }

            useProperties.HackType = MyUseType.None;
            if (m_hackSoloCheckbox.Checked)
            {
                useProperties.HackType |= MyUseType.Solo;
            }
            if (m_hackHUBCheckbox != null && m_hackHUBCheckbox.Checked)
            {
                useProperties.HackType |= MyUseType.FromHUB;
            }

            useProperties.HackingTime = (int)double.Parse(m_hackingTimeTextbox.Text.Trim());
            useProperties.HackingLevel = m_hackingLevelCombobox.GetSelectedKey();
            useProperties.IsHacked = m_isHackedCheckbox.Checked;
        }
    }
}
