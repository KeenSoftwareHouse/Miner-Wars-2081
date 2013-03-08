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

  class MyObjectiveWatchSigns : MyObjective
        {
            private uint m_targetId;
            private MyEntity m_target;

            public int FailDistance = 80;
            public int WarningDistance = 200;
            private MyHudNotification.MyNotification m_notification;
            public event Action DialogeFinished;

            public MyObjectiveWatchSigns(StringBuilder Name, MyMissionID ID, StringBuilder Description, MyTexture2D Icon, MyMission ParentMission, MyMissionID[] RequiredMissions, uint target, Audio.Dialogues.MyDialogueEnum? dialogId = null)
                : base(Name, ID, Description, Icon, ParentMission, RequiredMissions, null, null, null, dialogId)
            {
                m_targetId = target;
            }

            public override bool IsSuccess()
            {
                return false;
            }


            public override void Update()
            {
                base.Update();

                if ((MySession.PlayerShip.GetPosition() - m_target.GetPosition()).Length() < WarningDistance)
                {
                    m_notification.Appear();
                    MyScriptWrapper.AddNotification(m_notification);
                }
                else
                {
                    m_notification.Disappear();
                }

                if ((MySession.PlayerShip.GetPosition() - m_target.GetPosition()).Length() < FailDistance)
                {
                    Fail(MyTextsWrapperEnum.Fail_LostTarget);
                }
            }

            public override void Load()
            {
                base.Load();
                MyScriptWrapper.OnDialogueFinished += MyScriptWrapperOnOnDialogueFinished;
                m_notification = MyScriptWrapper.CreateNotification(MyTextsWrapperEnum.DoNotGoThere,
                                                                    MyGuiManager.GetFontMinerWarsRed());

                m_target = MyScriptWrapper.GetEntity(m_targetId);
                MyScriptWrapper.MarkEntity(m_target, NameTemp.ToString(),
                                           MyHudIndicatorFlagsEnum.SHOW_MISSION_MARKER |
                                           MyHudIndicatorFlagsEnum.SHOW_BORDER_INDICATORS |
                                           MyHudIndicatorFlagsEnum.SHOW_DISTANCE |
                                MyHudIndicatorFlagsEnum.SHOW_TEXT, MyGuitargetMode.Objective);
                MyScriptWrapper.AddNotification(m_notification);
                m_notification.Disappear();
            }

            private void MyScriptWrapperOnOnDialogueFinished(MyDialogueEnum dialogue, bool interrupted)
            {
                if (dialogue == StartDialogId)
                {
                    if (DialogeFinished != null) DialogeFinished();
                }
            }

            public override void Unload()
            {
                base.Unload();
                m_notification.Disappear();
                MyScriptWrapper.OnDialogueFinished -= MyScriptWrapperOnOnDialogueFinished;
                MyScriptWrapper.RemoveEntityMark(m_target);
            }
        }

}
