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
    class MyStealthPlayground : MyMissionSandboxBase
    {
        private enum EntityID : uint
        {
            StartLocation = 23619,
            Detector0 = 62667,
            Detector1 = 62669,
            Detector2 = 62670,
            Scanner0 = 39389,
            Scanner1 = 54904,
            Scanner2 = 54910,
            SecurityHub = 70447,
            PatrolSpawnPoint = 62685,
        }
       
        public MyStealthPlayground()
        {
            ID = MyMissionID.STEALTH_PLAYGROUND; /* ID must be added to MyMissions.cs */
            DebugName = new StringBuilder("Stealth playground");
            Name = Localization.MyTextsWrapperEnum.EmptyDescription;
            Description = Localization.MyTextsWrapperEnum.EmptyDescription;

            MyMwcVector3Int baseSector = new MyMwcVector3Int(-2, 0, 0);

            Location = new MyMissionLocation(baseSector, (uint)EntityID.StartLocation);

            RequiredMissions = new MyMissionID[] { MyMissionID.STEALTH_PLAYGROUND };
            RequiredActors = new MyActorEnum[] { MyActorEnum.MARCUS, MyActorEnum.MADELYN };

            m_objectives = new List<MyObjective>();

            //var playgroundSubmission = new MySubmission(
            //    new StringBuilder("Move like a ninja!"),
            //    MyMissionID.PLAYGROUND_SUBMISSION_01,
            //    new StringBuilder(""),
            //    null,
            //    this,
            //    new MyMissionID[] { },
            //    new MyMissionLocation(baseSector, 831)
            //);

            //playgroundSubmission.OnMissionSuccess += PlaygroundSubmissionSubmissionSuccess;
            //m_submissions.Add(playgroundSubmission);

        }

        public override void Load()
        {
            base.Load();
            var entities = MyEntities.GetEntities();

            var detectorsIds = new[] { EntityID.Detector0, EntityID.Detector1, EntityID.Detector2 };
            var scannerIds = new[] { EntityID.Scanner0, EntityID.Scanner1, EntityID.Scanner2 };
            for (int i = 0; i < detectorsIds.Length; i++)
			{
                MyDummyPoint dummy = (MyDummyPoint)MyScriptWrapper.GetEntity((uint)detectorsIds[i]);
                MyPrefabScanner scanner = (MyPrefabScanner)MyScriptWrapper.GetEntity((uint)scannerIds[i]);
                dummy.Tag = scanner.EntityId.Value.NumericValue;

                var detector = dummy.GetDetector();
                detector.OnEntityEnter += new OnEntityEnter(detector_OnEntityEnter);
                detector.OnEntityLeave += new OnEntityLeave(detector_OnEntityLeave);
                detector.On();
	        }

        }

        void detector_OnEntityLeave(MyEntityDetector sender, MyEntity entity)
        {
            //MySpawnPoint spawnPoint = (MySpawnPoint)MyScriptWrapper.GetEntity((uint)EntityID.PatrolSpawnPoint);

            MyDummyPoint dummy = (MyDummyPoint)sender.Parent;


            if (MyFactions.GetFactionsRelation(dummy, entity) == MyFactionRelationEnum.Friend)
            {

            }


            uint scannerId = (uint)dummy.Tag;
            MyPrefabScanner scanner = (MyPrefabScanner)MyScriptWrapper.GetEntity(scannerId);
            scanner.Enabled = true;
        }

        void detector_OnEntityEnter(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            MyDummyPoint dummy = (MyDummyPoint)sender.Parent;
            uint scannerId = (uint)dummy.Tag;
            MyPrefabScanner scanner = (MyPrefabScanner)MyScriptWrapper.GetEntity(scannerId);
            scanner.Enabled = false;
        }

      
        private void PlaygroundSubmissionSubmissionSuccess(MyMissionBase sender)
        {
            
        }
    }
}
