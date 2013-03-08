using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Audio.Dialogues;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Entities.EntityDetector;

namespace MinerWars.AppCode.Game.Missions.Components
{
    class MyDetectorDialogue : MyMissionComponent
    {
        public event Action OnDialogStarted;
        public event Action OnDialogFinished;

        private readonly uint m_detectorId;
        private readonly MyDialogueEnum m_dialogueId;
        private MyEntityDetector m_detector;
        
        public MyDetectorDialogue(uint detectorId, MyDialogueEnum dialogueId)
        {
            m_detectorId = detectorId;
            m_dialogueId = dialogueId;
        }

        public override void Load(MyMissionBase sender)
        {
            m_detector = MyScriptWrapper.GetDetector(MyScriptWrapper.GetEntity(m_detectorId));
            m_detector.On();
            m_detector.OnEntityEnter += DetectorOnOnEntityEnter;
            MyScriptWrapper.OnDialogueFinished += MyScriptWrapperOnOnDialogueFinished;
            base.Load(sender);
        }

        private void MyScriptWrapperOnOnDialogueFinished(MyDialogueEnum dialogue, bool interrupted)
        {
            if (dialogue == m_dialogueId)
            {
                if (OnDialogFinished != null) OnDialogFinished();
            }
        }

        private void DetectorOnOnEntityEnter(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            var isEnteredWithDrone = MyGuiScreenGamePlay.Static.IsControlledDrone &&
                                    entity == MyGuiScreenGamePlay.Static.ControlledDrone;

            if(MyScriptWrapper.IsPlayerShip(entity) || isEnteredWithDrone)
            {
                m_detector.Off();
                MyScriptWrapper.PlayDialogue(m_dialogueId);
                if (OnDialogStarted != null) OnDialogStarted();
            }
        }

        public override void Unload(MyMissionBase sender)
        {
            base.Unload(sender);
            m_detector.OnEntityEnter -= DetectorOnOnEntityEnter;
            MyScriptWrapper.OnDialogueFinished -= MyScriptWrapperOnOnDialogueFinished;
            m_detector.Off();

        }
    }
}
