using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.Prefabs;
using MinerWarsMath;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Entities;

namespace MinerWars.AppCode.Game.GUI.Prefabs
{
    class MyGuiControlPrefabContainerUse : MyGuiControlEntityUse
    {
        protected MyGuiControlButton m_onButon;
        protected MyGuiControlButton m_offButon;

        public MyGuiControlPrefabContainerUse(IMyGuiControlsParent parent, MyPrefabContainer prefabContainer)
            : base(parent, new Vector2(0.452f, 0.127f), prefabContainer) 
        {            
        }

        public MyGuiControlPrefabContainerUse(IMyGuiControlsParent parent, MyPrefabContainer prefabContainer, MyTexture2D texture)
            : base(parent, new Vector2(0.452f, 0.127f), prefabContainer, texture)
        {
        }


        public MyPrefabContainer PrefabContainer
        {
            get { return (MyPrefabContainer)m_entity; }
        }

        protected override void LoadControls()
        {
            Vector2 position = GetNextControlPosition() + new Vector2(0.38f, 0);

            m_onButon = new MyGuiControlButton(this, position, new Vector2(0.051f, 0.061f), Vector4.One, MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\ButtonOnOff", flags: TextureFlags.IgnoreQuality), null, null, 
                MyTextsWrapperEnum.On, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE_SMALLER,
                MyGuiControlButtonTextAlignment.CENTERED, OnOnOffClick, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true, true, MyGuiControlHighlightType.WHEN_ACTIVE);
            Controls.Add(m_onButon);
            m_onButon.DrawRedTextureWhenDisabled = false;
            m_onButon.UseSwitchMode = true;

            position.X -= 0.04f;

            m_offButon = new MyGuiControlButton(this, position, new Vector2(0.051f, 0.061f), Vector4.One, MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\ButtonOnOff", flags: TextureFlags.IgnoreQuality), null, null, 
                MyTextsWrapperEnum.Off, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE_SMALLER,
                MyGuiControlButtonTextAlignment.CENTERED, OnOnOffClick, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true, true, MyGuiControlHighlightType.WHEN_ACTIVE);
            Controls.Add(m_offButon);
            m_offButon.DrawRedTextureWhenDisabled = false;
            m_offButon.UseSwitchMode = true;

            UpdateEnabledState();
        }

        private void UpdateEnabledState()
        {
            if (PrefabContainer.AlarmOn)
            {
                m_onButon.Enabled = false;
                m_offButon.Enabled = true;
            }
            else
            {
                m_onButon.Enabled = true;
                m_offButon.Enabled = false;
            }
        }

        protected virtual void OnOnOffClick(MyGuiControlButton sender)
        {
            PrefabContainer.AlarmOn = !PrefabContainer.AlarmOn;
            UpdateEnabledState();
        }

        protected virtual void OnCheckBoxCheckCallback(MyGuiControlCheckbox sender) 
        {
            PrefabContainer.AlarmOn = sender.Checked;
        }
    }
}
