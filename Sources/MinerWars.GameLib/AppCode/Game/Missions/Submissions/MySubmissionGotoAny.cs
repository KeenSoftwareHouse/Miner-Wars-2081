using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.Managers.Session;
using System.Diagnostics;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.Entities;

namespace MinerWars.AppCode.Game.Missions.Submissions
{
    class MySubmissionFlyToAny : MyObjective
    {
        readonly List<MyMissionLocation> m_locations;

        public MySubmissionFlyToAny(StringBuilder Name, MyMissionID ID, StringBuilder Description, MyTexture2D Icon, MyMission ParentMission, MyMissionID[] RequiredMissions, List<MyMissionLocation> Locations, List<uint> MissionEntityIDs = null, Audio.Dialogues.MyDialogueEnum? successDialogId = null)
            : base(Name, ID, Description, Icon, ParentMission, RequiredMissions, null, MissionEntityIDs, successDialogId)
        {
            m_locations = Locations;
        }

        [Conditional("DEBUG")]
        private void TestLocations()
        {
            foreach (var loc in m_locations)
            {
                Debug.Assert(loc != null && loc.Entity != null, "All locations must be valid!");
            }
        }

        private bool TestLocation(MyMissionLocation location)
        {
            var boundingSphere = MySession.PlayerShip.WorldVolume;
            return MyGuiScreenGamePlay.Static.IsCurrentSector(location.Sector) && location.Entity.GetIntersectionWithSphere(ref boundingSphere);
        }

        public override void Unload()
        {
            base.Unload();

            SetLocationVisibility(false);
        }

        public override void Load()
        {
            base.Load();

            foreach (var loc in m_locations)
            {
                loc.Entity = MyEntities.GetEntityByMissionLocationIdentifier(loc.LocationEntityIdentifier) as MyDummyPoint;
                Debug.Assert(loc.Entity != null, "Cannot find location dummypoint, IDENTIFIER: " + loc.LocationEntityIdentifier.ToString());                

                if (!MissionEntityIDs.Contains(loc.Entity.EntityId.Value.NumericValue))
                {
                    MissionEntityIDs.Add(loc.Entity.EntityId.Value.NumericValue);
                }
            }
        }

        public override bool IsSuccess()
        {
            SetLocationVisibility(true);
            foreach (var loc in m_locations)
            {
                if (TestLocation(loc))
                {
                    return true;
                }
            }
            return false;
        }

        public override void SetLocationVisibility(bool visible)
        {
            foreach (var loc in m_locations)
            {
                SetLocationVisibility(visible, loc.Entity, GuiTargetMode);
            }
        }
    }
}
