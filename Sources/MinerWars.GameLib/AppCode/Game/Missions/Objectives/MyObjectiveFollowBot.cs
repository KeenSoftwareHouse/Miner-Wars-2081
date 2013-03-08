#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Audio.Dialogues;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.HUD;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.World.Global;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.Entities.EntityDetector;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Entities;
using System.Diagnostics;
using MinerWarsMath;
using MinerWars.AppCode.Game.Entities.WayPoints;
using MinerWars.AppCode.Game.Audio;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using MinerWars.AppCode.Game.Utils;

#endregion


namespace MinerWars.AppCode.Game.Missions.SinglePlayer
{

    class MyObjectiveFollowBot : MyObjective
    {
        private uint? m_targetId;
        private MySmallShipBot m_target;

        public int FailDistanceTooShort = 150;
        public int FailDistanceTooFar = 800;
        public int WarningDelta = 150;

        private MyEntity m_dummy;
        private uint m_dummyId;
        private bool m_invalid = false;

        private MyHudNotification.MyNotification m_notificationWarningHurry;
        private MyHudNotification.MyNotification m_notificationWarningSlowDown;

        public MyDialogueEnum FarDialog;
        public MyDialogueEnum ShortDialog;

        public uint TargetId
        {
            set
            {
                m_targetId = value;
                if (m_targetId != null)
                {
                    MissionEntityIDs.Add(value);
                    m_target = MyScriptWrapper.GetEntity(m_targetId.Value) as MySmallShipBot;
                }
            }
        }



        public MyObjectiveFollowBot(MyTextsWrapperEnum Name, MyMissionID ID, MyTextsWrapperEnum Description, MyTexture2D Icon, MyMission ParentMission, MyMissionID[] RequiredMissions, uint? target, uint dummy, Audio.Dialogues.MyDialogueEnum? startDialogId = null)
            : base(Name, ID, Description, Icon, ParentMission, RequiredMissions, null, null, null, startDialogId)
        {
            //MissionEntityIDs.Add(target);
            m_targetId = target;
            m_dummyId = dummy;
            MakeEntityIndestructible = false;
        }



        public override void Update()
        {
            if (m_dummy == null) return;
            
            base.Update();

            if (m_invalid) return;

            if ((MySession.PlayerShip.GetPosition() - m_target.GetPosition()).Length() < FailDistanceTooShort)
            {
                MyScriptWrapper.PlayDialogue(ShortDialog);
                m_target.SpeedModifier = 2.00f;
                m_dummy = null;
            }

            if ((MySession.PlayerShip.GetPosition() - m_target.GetPosition()).Length() < FailDistanceTooShort + WarningDelta)
            {
                MyScriptWrapper.AddNotification(m_notificationWarningSlowDown);
                m_notificationWarningSlowDown.Appear();
            }
            else
            {
                m_notificationWarningSlowDown.Disappear();
            }


            if ((MySession.PlayerShip.GetPosition() - m_target.GetPosition()).Length() > FailDistanceTooFar)
            {
                MyScriptWrapper.PlayDialogue(FarDialog);
                m_dummy = null;
            }

            if ((MySession.PlayerShip.GetPosition() - m_target.GetPosition()).Length() > FailDistanceTooFar - WarningDelta)
            {
                m_notificationWarningHurry.Appear();
                MyScriptWrapper.AddNotification(m_notificationWarningHurry);
            }
            else
            {
                m_notificationWarningHurry.Disappear();
            }
        }

        public override bool IsSuccess()
        {
            if (m_dummy == null) return false;
            if (m_target == null) return false;

            if (!m_dummy.Enabled) return false;

            var playerBoundingSphere = MySession.PlayerShip.WorldVolume;
            var targetBoundingSphere = m_target.WorldVolume;
            var result = base.IsSuccess() || (m_dummy.GetIntersectionWithSphere(ref playerBoundingSphere) && m_dummy.GetIntersectionWithSphere(ref targetBoundingSphere));
            return result;
        }

        public override void Load()
        {
            base.Load();
            m_dummy = MyScriptWrapper.GetEntity(m_dummyId);
            MyScriptWrapper.OnDialogueFinished += MyScriptWrapperOnDialogueFinished;
            m_notificationWarningHurry = MyScriptWrapper.CreateNotification(MyTextsWrapperEnum.HurryUp,
                                                                            MyGuiManager.GetFontMinerWarsRed());
            m_notificationWarningSlowDown = MyScriptWrapper.CreateNotification(MyTextsWrapperEnum.SlowDown,
                                                                               MyGuiManager.GetFontMinerWarsRed());

            MyScriptWrapper.AddNotification(m_notificationWarningHurry);
            m_notificationWarningHurry.Disappear();
            MyScriptWrapper.AddNotification(m_notificationWarningSlowDown);
            m_notificationWarningSlowDown.Disappear();

            m_invalid = false;

            //We prevent mission fail by this when skipping with ctrl+del
            if ((MySession.PlayerShip.GetPosition() - m_target.GetPosition()).Length() < FailDistanceTooShort)
            {
                m_invalid = true;
            }
            //We prevent mission fail by this when skipping with ctrl+del
            if ((MySession.PlayerShip.GetPosition() - m_target.GetPosition()).Length() > FailDistanceTooFar)
            {
                m_invalid = true;
            }
        }

        private void MyScriptWrapperOnDialogueFinished(MyDialogueEnum dialogue, bool interrupted)
        {
            if (dialogue == FarDialog)
            {
                Fail(MyTextsWrapperEnum.Fail_LostTarget);
            }
            if (dialogue == ShortDialog)
            {
                Fail(MyTextsWrapperEnum.Fail_SeenByTarget);
            }
        }


        public override void Unload()
        {
            base.Unload();
            m_targetId = null;
            m_dummy = null;
            MyScriptWrapper.OnDialogueFinished -= MyScriptWrapperOnDialogueFinished;
            m_notificationWarningHurry.Disappear();
            m_notificationWarningSlowDown.Disappear();
        }
    }
}
