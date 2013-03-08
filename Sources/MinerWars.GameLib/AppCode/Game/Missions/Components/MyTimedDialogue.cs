using System;
using MinerWars.AppCode.Game.Audio.Dialogues;

namespace MinerWars.AppCode.Game.Missions.Components
{
    class MyTimedDialogue : MyMissionComponent
    {
        readonly TimeSpan m_startTime;
        readonly MyDialogueEnum m_dialogue;

        public MyTimedDialogue(TimeSpan startTime, MyDialogueEnum dialogue)
        {
            m_startTime = startTime;
            m_dialogue = dialogue;
        }

        public override void Load(MyMissionBase sender)
        {
            base.Load(sender);

            sender.MissionTimer.RegisterTimerAction(m_startTime, OnAllTimeElapsed);
        }

        void OnAllTimeElapsed()
        {
            MyScriptWrapper.PlayDialogue(m_dialogue);
        }
    }
}