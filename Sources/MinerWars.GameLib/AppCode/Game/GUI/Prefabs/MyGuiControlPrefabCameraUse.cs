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
using MinerWars.AppCode.Game.Sessions;
using System.Diagnostics;

namespace MinerWars.AppCode.Game.GUI.Prefabs
{
    class MyGuiControlPrefabCameraUse : MyGuiControlPrefabUse
    {
        MyGuiControlButton m_takeControl;

        public MyPrefabCamera PrefabCamera
        {
            get { return (MyPrefabCamera)m_entity; }
        }

        public MyGuiControlPrefabCameraUse(IMyGuiControlsParent parent, MyPrefabCamera prefabCamera)
            : base(parent, prefabCamera) 
        {
            
        }

        public MyGuiControlPrefabCameraUse(IMyGuiControlsParent parent, MyPrefabCamera prefabCamera, MyTexture2D texture)
            : base(parent, prefabCamera, texture)
        {

        }


        protected override void LoadControls()
        {
            base.LoadControls();
            var pos = GetNextControlPosition() - MyGuiConstants.CONTROLS_DELTA + new Vector2(0.07f, -0.02f);


           m_takeControl = new MyGuiControlButton(this, pos, new Vector2(0.14f, 0.051f),
           Vector4.One,
           MyGuiManager.GetConfirmButton(), null, null, MyTextsWrapperEnum.AcquireControl,
           MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE_SMALLER, MyGuiControlButtonTextAlignment.CENTERED, OnControlClick,
           true, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP, true, true, true, drawCrossTextureWhenDisabled : false);
           Controls.Add(m_takeControl);
           m_takeControl.DrawRedTextureWhenDisabled = false;

           UpdateTakeControlState();
        }

        private void UpdateTakeControlState() 
        {
            bool canBeControlled = PrefabCamera.IsWorking() && !IsControlledByOtherPlayer();

            m_takeControl.Enabled = m_takeControl.Visible = canBeControlled;
        }

        protected override void OnOnOffClick(MyGuiControlButton sender)
        {
            base.OnOnOffClick(sender);
            UpdateTakeControlState();
        }

        void OnControlClick(MyGuiControlButton sender)
        {
            Debug.Assert(PrefabCamera.EntityId.HasValue, "EntityID cannot be null");

            if (MyMultiplayerGameplay.IsRunning)
            {
                MyMultiplayerGameplay.Static.LockReponse = (e, success) =>
                {
                    MyMultiplayerGameplay.Static.LockReponse = null;
                    if (PrefabCamera != e)
                    {
                        Debug.Fail("Something went wrong, locked different entity");
                        MyMultiplayerGameplay.Static.Lock(e, false);
                        return;
                    }

                    if (success)
                    {
                        ParentScreen.Closed += ParentScreenClosed;

                        MyGuiScreenGamePlay.Static.TakeControlOfCamera(PrefabCamera);
                        HideHUBScreen();
                    }
                };
                MyMultiplayerGameplay.Static.Lock(PrefabCamera, true);
            }
            else
            {
                MyGuiScreenGamePlay.Static.TakeControlOfCamera(PrefabCamera);
                HideHUBScreen();
            }
        }

        public override void Draw()
        {
            UpdateTakeControlState();
            base.Draw();
        }

        private void ParentScreenClosed(MyGuiScreenBase screen) 
        {
            MyMultiplayerGameplay.Static.Lock(PrefabCamera, false);
        }

        public override void ClearAfterRemove()
        {
            ParentScreen.Closed -= ParentScreenClosed;
        }
    }
}
