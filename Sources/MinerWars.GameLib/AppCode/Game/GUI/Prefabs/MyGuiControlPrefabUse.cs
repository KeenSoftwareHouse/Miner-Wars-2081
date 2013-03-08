using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWarsMath;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.Prefabs;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Sessions;
using MinerWars.CommonLIB.AppCode.Networking.Multiplayer;

namespace MinerWars.AppCode.Game.GUI.Prefabs
{
    class MyGuiControlPrefabUse : MyGuiControlEntityUse
    {
        protected MyGuiControlLabel m_unpoweredLabel;
        protected MyGuiControlLabel m_remoteControlledLabel;
        protected MyGuiControlButton m_onButton;
        protected MyGuiControlButton m_offButon;
        private bool m_isPrefabElectrified;


        public MyGuiControlPrefabUse(IMyGuiControlsParent parent, MyPrefabBase prefab, MyTexture2D texture)
            : base(parent, new Vector2(0.452f, 0.127f), prefab, texture)
        {

        }


        public MyGuiControlPrefabUse(IMyGuiControlsParent parent, MyPrefabBase prefab)
            : base(parent, new Vector2(0.452f, 0.127f), prefab)
        {
        }

        public MyPrefabBase Prefab
        {
            get { return (MyPrefabBase)m_entity; }
        }


        public override void Draw()
        {
            UpdateVisibility();
            UpdateEnabledState(); // Because of MP
            base.Draw();
        }

        protected override void LoadControls()
        {
            Vector2 position = GetNextControlPosition() + new Vector2(0.38f, 0);

            m_onButton = new MyGuiControlButton(this, position, new Vector2(0.051f, 0.061f), Vector4.One, MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\ButtonOnOff", flags: TextureFlags.IgnoreQuality), null, null,
                MyTextsWrapperEnum.On, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE_SMALLER,
                MyGuiControlButtonTextAlignment.CENTERED, OnOnOffClick, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true, true, MyGuiControlHighlightType.WHEN_ACTIVE);
            Controls.Add(m_onButton);
            m_onButton.DrawRedTextureWhenDisabled = false;
            m_onButton.UseSwitchMode = true;

            position.X -= 0.04f;

            m_offButon = new MyGuiControlButton(this, position, new Vector2(0.051f, 0.061f), Vector4.One, MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\ButtonOnOff", flags: TextureFlags.IgnoreQuality), null, null,
                MyTextsWrapperEnum.Off, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE_SMALLER,
                MyGuiControlButtonTextAlignment.CENTERED, OnOnOffClick, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true, true, MyGuiControlHighlightType.WHEN_ACTIVE);
            Controls.Add(m_offButon);
            m_offButon.DrawRedTextureWhenDisabled = false;
            m_offButon.UseSwitchMode = true;

            position.X -= 0.11f;

            m_unpoweredLabel = new MyGuiControlLabel(this, position, null, MyTextsWrapperEnum.Unpowered, MyGuiConstants.LABEL_TEXT_COLOR, 1.5f, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, MyGuiManager.GetFontMinerWarsBlue());
            m_remoteControlledLabel = new MyGuiControlLabel(this, position, null, MyTextsWrapperEnum.MP_RemoteControlled, MyGuiConstants.LABEL_TEXT_COLOR, 1.5f, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, MyGuiManager.GetFontMinerWarsBlue());
            Controls.Add(m_unpoweredLabel);
            Controls.Add(m_remoteControlledLabel);

            m_isPrefabElectrified = !Prefab.IsElectrified();
            UpdateVisibility();
            UpdateEnabledState();
        }

        private void UpdateVisibility()
        {
            m_isPrefabElectrified = Prefab.IsElectrified();

            if (IsControlledByOtherPlayer())
            {
                m_remoteControlledLabel.Visible = true;
                m_unpoweredLabel.Visible = false;
                m_onButton.Visible = false;
                m_offButon.Visible = false;
            }
            else if (m_isPrefabElectrified)
            {
                m_remoteControlledLabel.Visible = false;
                m_unpoweredLabel.Visible = false;
                m_onButton.Visible = true;
                m_offButon.Visible = true;
            }
            else
            {
                m_remoteControlledLabel.Visible = false;
                m_unpoweredLabel.Visible = true;
                m_onButton.Visible = false;
                m_offButon.Visible = false;
            }
        }

        private void UpdateEnabledState()
        {
            if (Prefab.Enabled)
            {
                m_onButton.Enabled = false;
                m_offButon.Enabled = true;
            }
            else
            {
                m_onButton.Enabled = true;
                m_offButon.Enabled = false;
            }
        }

        protected virtual void OnOnOffClick(MyGuiControlButton sender)
        {
            Prefab.Enabled = !Prefab.Enabled;
            UpdateEnabledState();

            if (MyMultiplayerGameplay.IsRunning)
            {
                MyMultiplayerGameplay.Static.UpdateFlags(Prefab, Prefab.Enabled ? MyFlagsEnum.ENABLE : MyFlagsEnum.DISABLE);
            }
        }
    }
}
