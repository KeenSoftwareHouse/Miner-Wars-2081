using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWarsMath;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Prefabs;
using MinerWars.AppCode.Game.Entities;
using System.Diagnostics;
using MinerWars.AppCode.Game.Entities.Prefabs;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Managers.Session;

namespace MinerWars.AppCode.Game.GUI.Prefabs
{
    class MyGuiScreenEntityUseSolo : MyGuiSceenEntityUseBase
    {
        private MyGuiControlEntityUse m_entityUseControl;
        private IMyHasGuiControl m_entity;

        public MyGuiScreenEntityUseSolo(IMyHasGuiControl entity)
            : base(new Vector2(0.505f, 0.5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, new Vector2(0.57f, 0.91f), true, MyGuiManager.GetHubBackground()) 
        {
            m_entity = entity;            
            RecreateControls(true);
            MyGuiScreenGamePlay.Static.ReleasedControlOfEntity += OnGameReleasedControlOfEntity;
            MySession.PlayerShip.OnDie += OnPlayerShipDie;
            entity.GetEntity().OnMarkForClose += OnEntityMarkForClose;
        }

        void OnEntityMarkForClose(MyEntity obj)
        {
            CloseScreenNow();
        }        

        public override void RecreateControls(bool contructor)
        {
            base.RecreateControls(contructor);
            Controls.Clear();

            if (!contructor) 
            {
                m_entityUseControl.ClearAfterRemove();
            }
            m_entityUseControl = m_entity.GetGuiControl(this);
            m_entityUseControl.SetPosition(new Vector2(-0.013f, -0.27f));
            m_entityUseControl.ParentScreen = this;

            Controls.Add(m_entityUseControl);

            var exitButton = new MyGuiControlButton(this, new Vector2(0f, 0.3490f), new Vector2(0.161f, 0.0637f),
                                                    Vector4.One,
                                                    MyGuiManager.GetConfirmButton(), null, null, MyTextsWrapperEnum.Exit,
                                                    MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, MyGuiControlButtonTextAlignment.CENTERED, OnExitClick,
                                                    true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true, true);
            Controls.Add(exitButton);            
        }

        void OnGameReleasedControlOfEntity(MyEntity entity)
        {
            if (m_entityUseControl.Entity == entity) 
            {
                Debug.Assert(m_state == MyGuiScreenState.HIDDEN || m_state == MyGuiScreenState.HIDING);

                RecreateControls(false);
                CanBeUnhidden = true;

                this.UnhideScreen();
            }            
        }

        private void OnExitClick(MyGuiControlButton sender)
        {
            CloseScreen();
        }

        protected override void OnClosed()
        {
            if (MyGuiScreenGamePlay.Static != null)
            {
                MyGuiScreenGamePlay.Static.ReleasedControlOfEntity -= OnGameReleasedControlOfEntity;
            }
            if (MySession.PlayerShip != null)
            {
                MySession.PlayerShip.OnDie -= OnPlayerShipDie;
            }

            base.OnClosed();
        }

        void OnPlayerShipDie(MyEntity entity, MyEntity killer)
        {
            CloseScreenNow();
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenPrefabUseSolo";
        }
    }
}
