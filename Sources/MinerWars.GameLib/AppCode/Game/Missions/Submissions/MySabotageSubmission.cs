using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.Entities.EntityDetector;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.Audio;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using System.Diagnostics;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.HUD;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Utils;

namespace MinerWars.AppCode.Game.Missions.SinglePlayer
{
    class MySabotageSubmission : MyObjective
    {
        private MyTextsWrapperEnum m_notificationText;
        private MyHudNotification.MyNotification m_notification;
        private bool m_isNearLocation;
        public MySabotageSubmission(
            StringBuilder name,
            MyMissionID id,
            StringBuilder description,
            MyTexture2D icon,
            MyMission parentMission,
            MyMissionID[] requiredMissions,
            MyMissionLocation location)
            : this(name, id, description, icon, parentMission, requiredMissions, location, MyTextsWrapperEnum.NotificationSabotageSubmission)
        {
        
        }

        public MySabotageSubmission(
            StringBuilder name, 
            MyMissionID id, 
            StringBuilder description, 
            MyTexture2D icon, 
            MyMission parentMission, 
            MyMissionID[] requiredMissions, 
            MyMissionLocation location, 
            MyTextsWrapperEnum notificationText)
            : base(name, id, description, icon, parentMission, requiredMissions, location, null)
        {
            m_notificationText = notificationText;
        }

        public override void Load()
        {
            base.Load();

            m_notification = new MyHudNotification.MyNotification(m_notificationText, MyHudNotification.DONT_DISAPEAR, null, new object[] { "" });
            //MyHudNotification.AddNotification(m_countdownNotification);

            MyScriptWrapper.OnUseKeyPress += OnUseKeyPress;
        }

        public override void Unload()
        {
            base.Unload();

            if (m_notification != null)
            {
                m_notification.Disappear();
                m_notification = null;
            }
            
            MyScriptWrapper.OnUseKeyPress -= OnUseKeyPress;
        }

        public override bool IsSuccess()
        {
            m_isNearLocation = base.IsSuccess();

            if (m_isNearLocation)
            {
                m_notification.SetTextFormatArguments(new object[] { MyGuiManager.GetInput().GetGameControlTextEnum(MyGameControlEnums.USE) });
                m_notification.Appear();
                MyHudNotification.AddNotification(m_notification);
            }
            else
            {
                m_notification.Disappear();
            }

            return false;
        }

        
        void OnUseKeyPress()
        {
            if (m_isNearLocation)
            {
                Success();
            }
        }
    }


}
