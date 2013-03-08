using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWarsMath;
using MinerWars.AppCode.Game.Entities.SubObjects;
using MinerWars.AppCode.Game.Missions;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Prefabs;
using MinerWars.AppCode.Game.GUI.Prefabs;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Localization;
using System.Diagnostics;
using MinerWars.AppCode.Game.Entities.Prefabs;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Sessions;

namespace MinerWars.AppCode.Game.GUI
{
    class MyGuiScreenSecurityControlHUB : MyGuiSceenEntityUseBase
    {
        private MyPrefabSecurityControlHUB m_prefabSecurityControlHUB;
        private MySmallShip m_useBy;

        private MyGuiControlList m_entitiesGuiList;
        List<MyGuiControlEntityUse> m_entitiesGui;

        public MyGuiScreenSecurityControlHUB(MySmallShip useBy, MyPrefabSecurityControlHUB prefabSecurityControlHUB)
            : base(new Vector2(0.505f, 0.5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, new Vector2(0.57f, 0.96f), true, MyGuiManager.GetHubBackground())
        {
            AddCaption(MyTextsWrapperEnum.SecurityControlHUB, new Vector2(0, 0.005f));

            m_prefabSecurityControlHUB = prefabSecurityControlHUB;
            m_useBy = useBy;
            m_entitiesGui = new List<MyGuiControlEntityUse>();
            RecreateControls(true);

            /*
            Controls.Add(new MyGuiControlButton(this, new Vector2(0f, 0.45f), MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.Exit, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnExitClick,
                MyGuiControlButtonTextAlignment.CENTERED, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));
             * */

            MyGuiScreenGamePlay.Static.ReleasedControlOfEntity += OnGameReleasedControlOfEntity;
            MyGuiScreenGamePlay.Static.RollLeftPressed += OnSwitchPrevious;
            MyGuiScreenGamePlay.Static.RollRightPressed += OnSwitchNext;
            MySession.PlayerShip.OnDie += OnPlayerShipDie;
            MySession.Static.Player.AliveChanged += Player_AliveChanged;
            m_prefabSecurityControlHUB.OnEntityDisconnected += m_prefabSecurityControlHUB_OnEntityDisconnected;
        }

        void Player_AliveChanged(World.MyPlayer player)
        {
            if (player.IsDead())
            {
                CloseScreenNow();
            }
        }

        void m_prefabSecurityControlHUB_OnEntityDisconnected(IMyUseableEntity obj)
        {
            RecreateControls(false);
        }

        public int GetNumberOfControllableEntities()
        {
            int count = 0;

            foreach (var entityUse in m_entitiesGui)
            {
                var currentEntityControl = entityUse;
                if ((currentEntityControl is MyGuiControlPrefabCameraUse ||
                     currentEntityControl is MyGuiControlPrefabLargeWeaponUse) &&
                    currentEntityControl.Entity.Enabled &&
                    !currentEntityControl.Entity.Closed)
                {
                    count++;
                }
            }

            return count;
        }

        bool IsControlledEntity(MyGuiControlEntityUse entityControl)
        {
            return MyGuiScreenGamePlay.Static.ControlledEntity == entityControl.Entity;
        }

        void OnSwitchPrevious()
        {
            var controlledIndex = m_entitiesGui.FindIndex(IsControlledEntity);

            if (controlledIndex != -1)
            {
                MyEntity oldControlledEntity = m_entitiesGui[controlledIndex].Entity;

                var index = controlledIndex;
                MyGuiControlEntityUse currentEntityControl = null;

                bool foundCandidate;

                do
                {
                    index--;
                    if (index < 0)
                    {
                        index = m_entitiesGui.Count - 1;
                    }
                    if (index == controlledIndex)
                    {
                        break;
                    }
                    currentEntityControl = m_entitiesGui[index];

                    foundCandidate =
                        (currentEntityControl is MyGuiControlPrefabCameraUse && oldControlledEntity is MyPrefabCamera ||
                         currentEntityControl is MyGuiControlPrefabLargeWeaponUse && oldControlledEntity is MyPrefabLargeWeapon
                        ) &&
                        currentEntityControl.Entity.Enabled &&
                        !currentEntityControl.Entity.Closed &&
                        !currentEntityControl.IsControlledByOtherPlayer();

                } while (!foundCandidate);

                if (index != controlledIndex && currentEntityControl != null)
                {
                    SwitchControlToEntity(oldControlledEntity, currentEntityControl.Entity);
                }
            }
        }

        void OnSwitchNext()
        {
            var controlledIndex = m_entitiesGui.FindIndex(IsControlledEntity);

            if (controlledIndex != -1)
            {
                MyEntity oldControlledEntity = m_entitiesGui[controlledIndex].Entity;

                var index = controlledIndex;
                MyGuiControlEntityUse currentEntityControl = null;

                bool foundCandidate;

                do
                {
                    index++;
                    if (index >= m_entitiesGui.Count)
                    {
                        index = 0;
                    }
                    if (index == controlledIndex)
                    {
                        break;
                    }

                    currentEntityControl = m_entitiesGui[index];

                    foundCandidate =
                        (currentEntityControl is MyGuiControlPrefabCameraUse && oldControlledEntity is MyPrefabCamera ||
                         currentEntityControl is MyGuiControlPrefabLargeWeaponUse && oldControlledEntity is MyPrefabLargeWeapon
                        ) &&
                        currentEntityControl.Entity.Enabled &&
                        !currentEntityControl.Entity.Closed &&
                        !currentEntityControl.IsControlledByOtherPlayer();

                } while (!foundCandidate);

                if (index != controlledIndex && currentEntityControl != null)
                {
                    SwitchControlToEntity(oldControlledEntity, currentEntityControl.Entity);
                }
            }
        }

        void SwitchControlToEntity(MyEntity oldControlledEntity, MyEntity entity)
        {
            Debug.Assert(entity.EntityId.HasValue, "EntityID cannot be null");

            if (MyMultiplayerGameplay.IsRunning)
            {
                MyMultiplayerGameplay.Static.LockReponse = (e, success) =>
                {
                    MyMultiplayerGameplay.Static.LockReponse = null;
                    if (entity != e)
                    {
                        Debug.Fail("Something went wrong, locked different entity");
                        MyMultiplayerGameplay.Static.Lock(e, false);
                        return;
                    }

                    if (success)
                    {
                        MyMultiplayerGameplay.Static.Lock(MyGuiScreenGamePlay.Static.ControlledEntity, false);
                        MyMultiplayerGameplay.Static.Lock(oldControlledEntity, false);
                        Closed += (s) =>
                        {
                            if (!entity.Closed)
                            {
                                MyMultiplayerGameplay.Static.Lock(entity, false);
                            }
                        };

                        SwitchControlToEntityInternal(entity);
                    }
                };
                MyMultiplayerGameplay.Static.Lock(entity, true);
            }
            else
            {
                SwitchControlToEntityInternal(entity);
            }
        }

        static void SwitchControlToEntityInternal(MyEntity entity)
        {
            var prefabCamera = entity as MyPrefabCamera;
            if (prefabCamera != null)
            {
                MyGuiScreenGamePlay.Static.SwitchControlOfCamera(prefabCamera);
            }

            var prefabLargeWeapon = entity as MyPrefabLargeWeapon;
            if (prefabLargeWeapon != null)
            {
                MyGuiScreenGamePlay.Static.SwitchControlOfLargeWeapon(prefabLargeWeapon);
            }
        }

        public override void RecreateControls(bool contructor)
        {
            base.RecreateControls(contructor);

            Controls.Clear();

            m_entitiesGuiList = new MyGuiControlList(
                this,
                new Vector2(0.0034f, -0.030f),
                new Vector2(0.489f, 0.65f),
                Vector4.Zero,
                null,//MyTextsWrapper.Get(MyTextsWrapperEnum.SecurityControlHUB),
                MyGuiManager.GetBlankTexture(),
                new Vector2(0.01f, 0.0f));

            if (!contructor) 
            {
                foreach (var entityGui in m_entitiesGui) 
                {
                    entityGui.ClearAfterRemove();
                }
            }
            m_entitiesGui.Clear();
            foreach (IMyUseableEntity connectedEntity in m_prefabSecurityControlHUB.ConnectedEntities)
            {
                MyEntity entity = connectedEntity as MyEntity;

                if (!entity.Visible)
                    continue;
                Debug.Assert((connectedEntity.UseProperties.UseType & MyUseType.FromHUB) != 0);
                IMyHasGuiControl entityWithGuiControl = connectedEntity as IMyHasGuiControl;
                Debug.Assert(entityWithGuiControl != null);
                // if entity is not hacked and could be hacked from HUB, then try hack it
                if (!connectedEntity.UseProperties.IsHacked &&
                    (connectedEntity.UseProperties.HackType & MyUseType.FromHUB) != 0)
                {
                    if (m_useBy.HackingTool != null &&
                        m_useBy.HackingTool.HackingLevel >= connectedEntity.UseProperties.HackingLevel)
                    {
                        connectedEntity.UseProperties.IsHacked = true;
                    }
                }
                MyGuiControlEntityUse entityGui = entityWithGuiControl.GetGuiControl(m_entitiesGuiList);
                entityGui.ParentScreen = this;
                m_entitiesGui.Add(entityGui);
            }
            m_entitiesGuiList.InitControls(m_entitiesGui);
            Controls.Add(m_entitiesGuiList);

            var exitButton = new MyGuiControlButton(
                this,
                new Vector2(0f, 0.3740f),
                new Vector2(0.161f, 0.0637f),
                Vector4.One,
                MyGuiManager.GetConfirmButton(),
                null,
                null,
                MyTextsWrapperEnum.Exit,
                MyGuiConstants.BUTTON_TEXT_COLOR,
                MyGuiConstants.BUTTON_TEXT_SCALE,
                MyGuiControlButtonTextAlignment.CENTERED,
                OnExitClick,
                true,
                MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER,
                true,
                true);
            Controls.Add(exitButton);
        }

        void OnGameReleasedControlOfEntity(MyEntity entity)
        {
            bool isMyEntity = false;
            foreach (var entityUse in m_entitiesGui)
            {
                if (entityUse.Entity == entity)
                {
                    isMyEntity = true;
                }
            }

            if (isMyEntity)
            {
                // Can be closed (or closing) when player is dead
                Debug.Assert(m_state == MyGuiScreenState.HIDDEN || m_state == MyGuiScreenState.HIDING || m_state == MyGuiScreenState.CLOSED || m_state == MyGuiScreenState.CLOSING);

                RecreateControls(false);
                CanBeUnhidden = true;
                this.UnhideScreen();
            }
        }

        protected override void OnClosed()
        {
            if (MyGuiScreenGamePlay.Static != null)
            {
                MyGuiScreenGamePlay.Static.ReleasedControlOfEntity -= OnGameReleasedControlOfEntity;
                MyGuiScreenGamePlay.Static.RollLeftPressed -= OnSwitchPrevious;
                MyGuiScreenGamePlay.Static.RollRightPressed -= OnSwitchNext;
            }
            if (MySession.PlayerShip != null)
            {
                MySession.PlayerShip.OnDie -= OnPlayerShipDie;
            }
            if (MySession.Static.Player != null)
            {
                MySession.Static.Player.AliveChanged -= Player_AliveChanged;
            }
            m_prefabSecurityControlHUB.OnEntityDisconnected -= m_prefabSecurityControlHUB_OnEntityDisconnected;

            if (MyGuiScreenGamePlay.Static != null && MyGuiScreenGamePlay.Static.ActiveSecurityHubScreen == this)
            {
                MyGuiScreenGamePlay.Static.ActiveSecurityHubScreen = null;
            }

            base.OnClosed();
        }

        private void OnExitClick(MyGuiControlButton sender)
        {
            CloseScreen();
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenSecurityControlHUB";
        }

        void OnPlayerShipDie(MyEntity entity, MyEntity killer)
        {
            CloseScreenNow();
        }
    }
}
