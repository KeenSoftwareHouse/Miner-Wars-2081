using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Entities.SubObjects;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWarsMath;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Sessions;
using System.Diagnostics;

namespace MinerWars.AppCode.Game.GUI.Prefabs
{
    class MyGuiControlPrefabLargeWeaponUse : MyGuiControlPrefabUse
    {
        MyGuiControlButton m_acquireControl;

        public MyPrefabLargeWeapon PrefabLargeWeapon
        {
            get { return (MyPrefabLargeWeapon)m_entity; }
        }

        public MyGuiControlPrefabLargeWeaponUse(IMyGuiControlsParent parent, MyPrefabLargeWeapon prefab)
            : base(parent, prefab) 
        {
            
        }

        public MyGuiControlPrefabLargeWeaponUse(IMyGuiControlsParent parent, MyPrefabLargeWeapon prefab, MyTexture2D texture)
            : base(parent, prefab,texture)
        {

        }


        protected override void LoadControls()
        {
            base.LoadControls();

            var pos = GetNextControlPosition() - MyGuiConstants.CONTROLS_DELTA + new Vector2(0.07f,-0.02f);

            /*
            Controls.Add(m_acquireControl = new MyGuiControlButton(this, pos , new Vector2(MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE.X * 2f, MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE.Y), MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.AcquireControl, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnAcquireControlClick, MyGuiControlButtonTextAlignment.CENTERED, true, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP, true));
            */

            m_acquireControl = new MyGuiControlButton(this, pos, new Vector2(0.14f, 0.051f),
                       MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                       MyGuiManager.GetConfirmButton(), null, null, MyTextsWrapperEnum.AcquireControl,
                       MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE_SMALLER, MyGuiControlButtonTextAlignment.CENTERED, OnAcquireControlClick,
                       true, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP, true, true);
            m_acquireControl.Enabled = m_entity.Enabled;
            Controls.Add(m_acquireControl);
            m_acquireControl.DrawRedTextureWhenDisabled = false;

            UpdateAcquireState();
        }

        public override void Draw()
        {
            UpdateAcquireState();
            base.Draw();
        }

        private void UpdateAcquireState() 
        {
            bool canBeControlled = PrefabLargeWeapon.IsWorking() && !IsControlledByOtherPlayer();

            m_acquireControl.Visible = m_acquireControl.Enabled = canBeControlled;
        }

        protected override void OnOnOffClick(MyGuiControlButton sender)
        {
            base.OnOnOffClick(sender);
            UpdateAcquireState();
        }

        void OnAcquireControlClick(MyGuiControlButton sender)
        {
            Debug.Assert(PrefabLargeWeapon.EntityId.HasValue, "EntityID cannot be null");

            if (MyMultiplayerGameplay.IsRunning)
            {
                MyMultiplayerGameplay.Static.LockReponse = (e, success) =>
                {
                    MyMultiplayerGameplay.Static.LockReponse = null;
                    if (PrefabLargeWeapon != e)
                    {
                        Debug.Fail("Something went wrong, locked different entity");
                        MyMultiplayerGameplay.Static.Lock(e, false);
                        return;
                    }

                    if (success)
                    {
                        ParentScreen.Closed += ParentScreenClosed;

                        MyGuiScreenGamePlay.Static.TakeControlOfLargeWeapon(PrefabLargeWeapon);
                        HideHUBScreen();
                    }
                };
                MyMultiplayerGameplay.Static.Lock(PrefabLargeWeapon, true);
            }
            else
            {
                MyGuiScreenGamePlay.Static.TakeControlOfLargeWeapon(PrefabLargeWeapon);
                HideHUBScreen();
            }
        }

        void ParentScreenClosed(MyGuiScreenBase screen) 
        {
            MyMultiplayerGameplay.Static.Lock(PrefabLargeWeapon, false);
        }

        public override void ClearAfterRemove()
        {
            base.ClearAfterRemove();
            ParentScreen.Closed -= ParentScreenClosed;
        }
    }
}
