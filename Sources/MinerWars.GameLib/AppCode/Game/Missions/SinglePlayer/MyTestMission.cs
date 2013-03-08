using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Entities.EntityDetector;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.AppCode.Game.HUD;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Missions;
using MinerWars.AppCode.Game.Audio;
using MinerWars.AppCode.Game.Explosions;
using MinerWarsMath;
using MinerWars.AppCode.Game.Entities.VoxelHandShapes;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Voxels;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.AppCode.Game.Prefabs;
using MinerWars.AppCode.Game.World.Global;

namespace MinerWars.AppCode.Game.Missions.SinglePlayer
{
    class MyTestMission : MyMission
    {
        private enum EntityID : uint
        {
            StartLocation = 64,
        }

        public MyTestMission()
        {
            ID = MyMissionID.TEST_MISSION; /* ID must be added to MyMissions.cs */
            DebugName = new StringBuilder("Test mission");
            Name = Localization.MyTextsWrapperEnum.EmptyDescription;
            Description = Localization.MyTextsWrapperEnum.EmptyDescription;

            MyMwcVector3Int baseSector = new MyMwcVector3Int(-2, 0, 2);

            Location = new MyMissionLocation(baseSector, (uint)EntityID.StartLocation);

            RequiredMissions = new MyMissionID[] { MyMissionID.TEST_MISSION };
            RequiredActors = new MyActorEnum[] { MyActorEnum.MADELYN };

            m_objectives = new List<MyObjective>();

            var objective1 = new MyObjective(
                new StringBuilder("Objective1"),
                MyMissionID.TEST_MISSION_OBJECTIVE1,
                new StringBuilder(""),
                null,
                this,
                new MyMissionID[] { },
                null
            );
            objective1.SaveOnSuccess = true;
            m_objectives.Add(objective1);

            var objective2 = new MyObjective(
                new StringBuilder("Objective2"),
                MyMissionID.TEST_MISSION_OBJECTIVE2,
                new StringBuilder(""),
                null,
                this,
                new MyMissionID[] { MyMissionID.TEST_MISSION_OBJECTIVE1 },
                null
            );
            objective2.SaveOnSuccess = true;
            m_objectives.Add(objective2);


        }

        public override void Load()
        {
            base.Load();   
        }

    }
}
