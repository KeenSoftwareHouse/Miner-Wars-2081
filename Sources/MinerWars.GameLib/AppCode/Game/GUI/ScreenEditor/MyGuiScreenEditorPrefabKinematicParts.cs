using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Localization;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.Utils;

namespace MinerWars.AppCode.Game.GUI.ScreenEditor
{
    class MyGuiScreenEditorPrefabKinematicParts : MyGuiScreenEditorDialogBase
    {
        private MyGuiControlTextbox[] m_healthTextboxes;
        private MyGuiControlTextbox[] m_maxHealthTextboxes;
        private MyGuiScreenEditorEditProperties.MyHealthAndMaxHealth[] m_healthsAndMaxhealths;

        private List<StringBuilder> m_errorMessages;
        private StringBuilder m_errorMessage;

        public MyGuiScreenEditorPrefabKinematicParts(MyGuiScreenEditorEditProperties.MyHealthAndMaxHealth[] healthsAndMaxhealths)
            : base(new Vector2(0.5f, 0.5f), new Vector2(0.8f, 0.5f)) 
        {            
            AddCaption(MyTextsWrapperEnum.EditPrefabKinematicPartsHealthAndMaxHealth, MyGuiConstants.LABEL_TEXT_COLOR);
            m_healthsAndMaxhealths = healthsAndMaxhealths;
            m_healthTextboxes = new MyGuiControlTextbox[m_healthsAndMaxhealths.Length];
            m_maxHealthTextboxes = new MyGuiControlTextbox[m_healthsAndMaxhealths.Length];

            Init();

            m_errorMessages = new List<StringBuilder>();
            m_errorMessage = new StringBuilder();
        }

        private void Init() 
        {
            Vector2 topLeftPosition = new Vector2(-m_size.Value.X / 2f + 0.05f, -m_size.Value.Y / 2f + 0.15f);
            Vector2 labelHealthPosition = topLeftPosition;
            Vector2 textboxHealthPosition = labelHealthPosition + new Vector2(0.2f, 0f);
            Vector2 labelMaxHealthPosition = textboxHealthPosition + new Vector2(0.13f, 0f);
            Vector2 textboxMaxHealthPosition = labelMaxHealthPosition + new Vector2(0.22f, 0f);
            for (int i = 0; i < m_healthsAndMaxhealths.Length; i++)
            {
                if (m_healthsAndMaxhealths[i] != null)
                {
                    MyGuiControlLabel labelHealth = new MyGuiControlLabel(this, labelHealthPosition, null, MyTextsWrapperEnum.Health, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);
                    Controls.Add(labelHealth);

                    m_healthTextboxes[i] = new MyGuiControlTextbox(this, textboxHealthPosition, MyGuiControlPreDefinedSize.MEDIUM, MyValueFormatter.GetFormatedFloat(m_healthsAndMaxhealths[i].Health, 0, string.Empty), 6, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiControlTextboxType.DIGITS_ONLY);
                    Controls.Add(m_healthTextboxes[i]);

                    MyGuiControlLabel labelMaxHealth = new MyGuiControlLabel(this, labelMaxHealthPosition, null, MyTextsWrapperEnum.MaxHealth, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);
                    Controls.Add(labelMaxHealth);

                    m_maxHealthTextboxes[i] = new MyGuiControlTextbox(this, textboxMaxHealthPosition, MyGuiControlPreDefinedSize.MEDIUM, MyValueFormatter.GetFormatedFloat(m_healthsAndMaxhealths[i].MaxHealth, 0, string.Empty), 6, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiControlTextboxType.DIGITS_ONLY);
                    Controls.Add(m_maxHealthTextboxes[i]);
                }
                else 
                {
                    MyGuiControlLabel labelDestroyed = new MyGuiControlLabel(this, labelHealthPosition, null, MyTextsWrapperEnum.Destroyed, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);
                    Controls.Add(labelDestroyed);
                }
                labelHealthPosition += MyGuiConstants.CONTROLS_DELTA;
                textboxHealthPosition += MyGuiConstants.CONTROLS_DELTA;
                labelMaxHealthPosition += MyGuiConstants.CONTROLS_DELTA;
                textboxMaxHealthPosition += MyGuiConstants.CONTROLS_DELTA;
            }

            AddOkAndCancelButtonControls();
        }

        private bool Validate(ref List<StringBuilder> errorMessages)
        {
            // check healt and max health            
            for (int i = 0; i < m_healthsAndMaxhealths.Length; i++) 
            {
                if (m_healthTextboxes[i] != null && m_maxHealthTextboxes[i] != null) 
                {
                    float? health = !String.IsNullOrEmpty(m_healthTextboxes[i].Text) ? float.Parse(m_healthTextboxes[i].Text) : (float?)null;
                    float? maxHealth = !String.IsNullOrEmpty(m_maxHealthTextboxes[i].Text) ? float.Parse(m_maxHealthTextboxes[i].Text) : (float?)null;

                    if (health == null)
                    {
                        errorMessages.Add(MyTextsWrapper.Get(MyTextsWrapperEnum.MessageYouMustSetHealth));
                    }
                    if (maxHealth == null)
                    {
                        errorMessages.Add(MyTextsWrapper.Get(MyTextsWrapperEnum.MessageYouMustSetMaxHealth));
                    }
                    if (health != null && health <= 0f)
                    {
                        errorMessages.Add(MyTextsWrapper.Get(MyTextsWrapperEnum.MessageHealthCantBeLessOrEqualZero));
                    }
                    if (maxHealth != null && maxHealth <= 0f)
                    {
                        errorMessages.Add(MyTextsWrapper.Get(MyTextsWrapperEnum.MessageMaxHealthCantBeLessOrEqualZero));
                    }
                    if (health != null && maxHealth != null && health > maxHealth)
                    {
                        errorMessages.Add(MyTextsWrapper.Get(MyTextsWrapperEnum.MessageHealthMustBeLesserOrEqualMaxHealth));
                    }
                }
            }            
            return errorMessages.Count == 0;
        }

        protected override void OnOkClick(MyGuiControlButton sender)
        {
            m_errorMessages.Clear();
            m_errorMessage.Clear();
            if (!Validate(ref m_errorMessages))
            {
                DisplayErrorMessage();
                return;
            }

            Save();
            
            base.OnOkClick(sender);
        }

        private void DisplayErrorMessage() 
        {
            foreach (StringBuilder errorMessage in m_errorMessages)
            {
                MyMwcUtils.AppendStringBuilder(m_errorMessage, errorMessage);
                m_errorMessage.AppendLine();
            }
            StringBuilder caption = MyTextsWrapper.Get(m_errorMessages.Count > 1 ? MyTextsWrapperEnum.CaptionPropertiesAreNotValid : MyTextsWrapperEnum.CaptionPropertyIsNotValid);
            MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, m_errorMessage, caption, MyTextsWrapperEnum.Ok, null));
        }

        private void Save() 
        {
            for (int i = 0; i < m_healthsAndMaxhealths.Length; i++)
            {
                if (m_healthsAndMaxhealths != null)
                {
                    m_healthsAndMaxhealths[i].Health = float.Parse(m_healthTextboxes[i].Text);
                    m_healthsAndMaxhealths[i].MaxHealth = float.Parse(m_maxHealthTextboxes[i].Text);
                }
            }
        }

        protected override void OnCancelClick(MyGuiControlButton sender)
        {
            base.OnCancelClick(sender);
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenEditorPrefabKinematicParts";
        }
    }
}
