using System;
using MinerWarsMath;


using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.World.Global;
using MinerWars.CommonLIB.AppCode.Utils;
using SysUtils.Utils;
using MinerWars.AppCode.Game.Missions;
using MinerWars.AppCode.Game.Managers.Session;

namespace MinerWars.AppCode.Game.Audio.Dialogues
{
    internal delegate void MyDialogueDelegate(MyDialogueEnum dialogue, bool interrupted);
    internal delegate void MySentenceDelegate(MyDialogueEnum dialogue, MyDialoguesWrapperEnum sentence);

    static class MyDialogues
    {
        public static MyDialogueDelegate OnDialogueFinished;
        public static MySentenceDelegate OnSentenceStarted;

        public static MyDialogue CurrentDialogue { get { return MyDialogueConstants.GetDialogue(m_currentDialogueId); } }
        public static MyDialogueSentence CurrentSentence
        { 
            get
            {
                var d = CurrentDialogue;
                if (d == null || m_currentSentenceIndex >= d.Sentences.Length)
                    return null;
                else
                    return d.Sentences[m_currentSentenceIndex];
            }
        }
        public static float CurrentSentenceStarted_ms { get; private set; }

        private static MySoundCue? m_currentCue;

        public static void Play(MyDialogueEnum id)
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyDialogues::Play");

            if (MinerWars.AppCode.Game.Managers.Session.MySession.PlayerShip != null && MinerWars.AppCode.Game.Managers.Session.MySession.PlayerShip.IsDead())
            {
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
                return;
            }

            MyDialogue newDialogue = MyDialogueConstants.GetDialogue(id);
            if (newDialogue != null && (CurrentDialogue == null || CurrentSentence == null || newDialogue.Priority >= CurrentDialogue.Priority))
                SwitchTo(id);

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        public static void Stop()
        {
            SwitchTo(MyDialogueEnum.NONE);
        }

        public static void LoadData()
        {
        }

        public static void UnloadData()
        {
            Stop();
        }

        public static void Update()
        {
            if (CurrentSentence == null) return;

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyDialogues.Update");

            if ((m_currentCue == null && MyMinerGame.TotalGamePlayTimeInMilliseconds - CurrentSentenceStarted_ms > CurrentSentence.SentenceTime_ms) ||
                (m_currentCue != null && (!m_currentCue.Value.IsValid || m_currentCue.Value.IsStopped))
                )
            {
                // advance to next sentence or unload
                m_currentSentenceIndex++;
                if (m_currentDialogueId != MyDialogueEnum.NONE && m_currentSentenceIndex == CurrentDialogue.Sentences.Length)
                    if (OnDialogueFinished != null)
                        OnDialogueFinished(m_currentDialogueId, false);

                PlayCurrentSentence();
            }

            if (m_currentCue != null && CurrentSentence.PauseBefore_ms != 0 && m_currentCue.Value.IsValid && m_currentCue.Value.IsPaused && MyMinerGame.TotalGamePlayTimeInMilliseconds - CurrentSentenceStarted_ms > CurrentSentence.PauseBefore_ms && !MyMinerGame.IsPaused())
                m_currentCue.Value.Resume();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        public static bool IsPlaying()
        {
            if (CurrentSentence == null || m_currentDialogueId == MyDialogueEnum.NONE || m_currentSentenceIndex >= CurrentDialogue.Sentences.Length) return false;
            else return true;
        }

        #region Implementation

        static MyDialogues()
        {
            MyMwcLog.WriteLine("MyDialogues()");
        }

        static MyDialogueEnum m_currentDialogueId = MyDialogueEnum.NONE;
        static int m_currentSentenceIndex = 0;
        static float m_volume = 1;

        static void SwitchTo(MyDialogueEnum id)
        {
            if (id != MyDialogueEnum.NONE)
            {
                foreach (var myDialogueSentence in CurrentDialogue.Sentences)
                {
                    if (myDialogueSentence.Listener != null)
                    {
                        var speaker = MyScriptWrapper.TryGetEntity(MyActorConstants.GetActorName(myDialogueSentence.Actor)) as MySmallShipBot;
                        if (speaker != null)
                        {
                            speaker.LookTarget = null;
                        }
                    }
                }
            }
            if (m_currentDialogueId == MyDialogueEnum.NONE || CurrentSentence == null || OnDialogueFinished == null)
            {
                m_currentDialogueId = id;
                m_currentSentenceIndex = 0;
            }
            else
            {
                var oldDialogueId = m_currentDialogueId;

                m_currentDialogueId = id;
                m_currentSentenceIndex = 0;

                OnDialogueFinished(oldDialogueId, true);
            }

            PlayCurrentSentence();
        }

        static void StopSound()
        {
            if (m_currentCue != null)
                m_currentCue.Value.Stop(SharpDX.XACT3.StopFlags.Release);
        }

        static void PlayCurrentSentence()
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("PlayCurrentSentence");

            StopSound();
            CurrentSentenceStarted_ms = MyMinerGame.TotalGamePlayTimeInMilliseconds;
            if (CurrentSentence == null || CurrentSentence.Cue == null)
            {
                m_currentCue = null;
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
                return;
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("LoadSentence");

            if (CurrentSentence.Listener != null)
            {
                var speaker = MyScriptWrapper.TryGetEntity(MyActorConstants.GetActorName(CurrentSentence.Actor)) as MySmallShipBot;
                MySmallShip listener = null;
                if (CurrentSentence.Listener.Value == MyActorEnum.APOLLO)
                {
                    listener = MySession.Static.Player.Ship;
                } 
                else
                {
                    listener = MyScriptWrapper.TryGetEntity(MyActorConstants.GetActorName(CurrentSentence.Listener.Value)) as MySmallShip;
                }
                if (speaker != null && listener != null)
                {
                    speaker.LookTarget = listener;
                }
            }
            if (CurrentSentence.Cue != null)
            {
                m_currentCue = MyAudio.AddCue2D(CurrentSentence.Cue.Value, m_volume);

                if (OnSentenceStarted != null)
                    OnSentenceStarted(m_currentDialogueId, CurrentSentence.Text);

                if (m_currentCue != null && CurrentSentence.PauseBefore_ms != 0)
                    m_currentCue.Value.Pause();
            }
            else
            {
                m_currentCue = null;
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        public static bool IsCurrentCuePausedAndHidden()
        {
            return (m_currentCue != null && m_currentCue.Value.IsValid && m_currentCue.Value.IsPaused) ||
                   (m_currentCue == null && MyMinerGame.TotalGamePlayTimeInMilliseconds - CurrentSentenceStarted_ms < CurrentSentence.PauseBefore_ms);
        }

        #endregion
    }
}
