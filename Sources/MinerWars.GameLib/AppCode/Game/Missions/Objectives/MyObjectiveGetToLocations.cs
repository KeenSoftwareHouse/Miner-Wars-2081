#region Using

using System;
using System.Collections.Generic;
using System.Text;
using MinerWarsMath.Graphics;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Audio;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.HUD;
using MinerWars.AppCode.Game.Journal;
using MinerWars.AppCode.Game.Localization;
using MinerWarsMath;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.GUI.Helpers;
using MinerWars.CommonLIB.AppCode.Generics;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Prefabs;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.AppCode.Game.Missions.Utils;
using MinerWars.CommonLIB.AppCode.Utils;
using System.Diagnostics;

#endregion


namespace MinerWars.AppCode.Game.Missions
{
     class MyObjectiveGetToLocations : MyMultipleObjectives
     {
        private List<MyMissionLocation> m_dummiesToVisit;
        private List<MyMissionLocation> m_dummiesVisited;

        public MyObjectiveGetToLocations(StringBuilder Name, MyMissionID ID, StringBuilder Description, MyTexture2D Icon, MyMission ParentMission, MyMissionID[] RequiredMissions, List<MyMissionLocation> DummiesToVisit, Audio.Dialogues.MyDialogueEnum? successDialogId = null, Audio.Dialogues.MyDialogueEnum? startDialogId = null, bool displayObjectivesCount = true) :
            base(Name, ID, Description, Icon, ParentMission, RequiredMissions, null, null, successDialogId, startDialogId, displayObjectivesCount)
        {
            Debug.Assert(DummiesToVisit != null);
            Debug.Assert(DummiesToVisit.Count > 0);
            m_dummiesToVisit = DummiesToVisit;
            m_dummiesVisited = new List<MyMissionLocation>();
        }

        public override void Load()
        {
            foreach (MyMissionLocation locationToVisit in m_dummiesToVisit)
            {
                locationToVisit.Entity = MyScriptWrapper.GetEntity(locationToVisit.LocationEntityIdentifier);
                SetLocationVisibility(true, locationToVisit.Entity, MyGuitargetMode.Objective);
            }
            base.Load();
        }

        public override bool IsMissionEntity(MyEntity target)
        {
            if (base.IsMissionEntity(target)) return true;
            foreach (MyMissionLocation locationToVisit in m_dummiesToVisit)
            {
                if (target == locationToVisit.Entity && !m_dummiesVisited.Contains(locationToVisit))
                {
                    return true;
                }
            }
            return false;
        }

        public override void Update()
        {
            base.Update();
            var boundingSphere = MySession.PlayerShip.WorldVolume;
            foreach (MyMissionLocation locationToVisit in m_dummiesToVisit)
            {
                if (!m_dummiesVisited.Contains(locationToVisit) && locationToVisit.Entity.GetIntersectionWithSphere(ref boundingSphere))
                {
                    SetLocationVisibility(false, locationToVisit.Entity, MyGuitargetMode.Objective);
                    m_dummiesVisited.Add(locationToVisit);
                }
            }
        }

        public override void Unload()
        {
            base.Unload();
            foreach (MyMissionLocation locationToVisit in m_dummiesToVisit)
            {
                SetLocationVisibility(false, locationToVisit.Entity, MyGuitargetMode.Objective);
            }
            m_dummiesVisited.Clear();
        }

        public override bool IsSuccess()
        {
            return m_dummiesVisited.Count >= m_dummiesToVisit.Count;
        }

        protected override int GetObjectivesCompletedCount()
        {
            return m_dummiesVisited.Count;
        }

        protected override int GetObjectivesTotalCount()
        {
            return m_dummiesToVisit.Count;
        }
    }

}
