//#define DEBUG_AUDIO

using System;
using System.Collections.Generic;
using MinerWarsMath;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.TransparentGeometry;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Utils;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Physics;
using MinerWars.AppCode.Game.Physics;
using SysUtils;
using SharpDX.XACT3;
using SharpDX.X3DAudio;

//  IMPORTANT: You can load sounds only in main thread (not background thread). It will not throw exception, but sounds loaded on background thread
//  will be silent when played on main thread (on XP, on Vista it's OK).

//  Use this class to play one-time or looping sounds.
//  It's based on 3D features of XACT. I did tests, and it looks that XATC doesn't create unvanted garbage, even if new sounds are created by m_soundBank.GetCue(...)
//  How I tested: in MyGuiScreenGameBase.Static.Update method, I started explosion sound every 100 milisecons, at random location. I let it runing few minutes and watched output of
//  how many GC collections were run. No one was there, so I assume, XACT is allocating cues from its own preallocated buffer.
//
//  By the way, this test tells me, I can run 10 sounds per second without any performance (speed or memory) problems. Probably we can run more, but I don't need it.

namespace MinerWars.AppCode.Game.Audio
{
    using System.IO;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Threading;
    using KeenSoftwareHouse.Library.Trace;
    using MinerWars.AppCode.Game.Managers.Session;
    using System.Diagnostics;
    using System.Text;
    using KeenSoftwareHouse.Library.Extensions;
    using SharpDX.Toolkit;

    enum MyMusicState
    {
        Stopped,
        Playing,
        Transition,
    }

    static partial class MyAudio
    {
        //  XACT objects
        static AudioEngine m_audioEngine;
        static MyX3DAudio m_x3dAudio;
        static List<WaveBank> m_waveBanks;
        static SoundBank m_sfxSoundBank;
        static SoundBank m_musicSoundBank;
        static SoundBank m_dialogueSoundBank;
        static SoundBank m_voiceSoundBank;
        //static SoundBank m_voiceSoundBank;
        static Listener m_listener;
        static Emitter m_helperEmitter;          // The emitter describes an entity which is making a 3D sound.
        static AudioCategory m_cockpitCategory;
        static AudioCategory m_defaultCategory;
        static AudioCategory m_musicCategory;
        static AudioCategory m_guiCategory;
        static AudioCategory m_wep2DCategory;
        static AudioCategory m_doorCategory;
        static AudioCategory m_enginesCategory;
        static AudioCategory m_dialogueCategory;
        static AudioCategory m_shoutsCategory;
        static AudioCategory m_hudCategory;
        static AudioCategory m_ambCategory;
        static AudioCategory m_drillsCategory;
        static AudioCategory m_impCategory;
        static AudioCategory m_importantsCategory;
        static AudioCategory m_sfxCategory;
        static AudioCategory m_voiceCategory;
        static AudioCategory m_wep3DCategory;
        static AudioCategory m_xCategory;
        static AudioCategory m_welcomeCategory;
        static List<AudioCategory> m_gameCategories;
        static float m_volumeGui;
        static float m_volumeDefault;
        static float m_volumeMusic;
        static float m_reverbControl;
        static bool m_canPlay;
        static bool m_gameSoundsOn;
        static bool m_musicOn;
        static bool m_musicAllowed;
        static bool m_loopMusic = true;

        static MyCuePool m_cuePool;

        static MyCueInfo[] m_cueInfos;

        static HashSet<WeakReference>[] m_nonLoopableCuesLimit;    //  Here we will remember every non-loopable cue we are playing. It will serve for limiting max count of same cues played too.

        //  Numbers must correspond to real number of wave files for corresponding cue
        const int MUS_MAIN_MENU_WAVES_COUNT = 7;
        const int MUS_STORY_AMBIENT_WAVES_COUNT = 23;
        const string WAVEBANK_SUFFIX = ".xwb";

        static Dictionary<uint, MySoundCue> m_collisionDictionary = new Dictionary<uint, MySoundCue>(MyAudioConstants.MAX_COLLISION_SOUNDS);
        static Dictionary<int, List<int>> m_collisionSoundsDictionary = new Dictionary<int, List<int>>();
        static List<uint> m_collisionDictionaryRemovals = new List<uint>(MyAudioConstants.MAX_COLLISION_SOUNDS);

        //  Music cues
        struct MyMusicTransition
        {
            public int Priority;
            public MyMusicTransitionEnum TransitionEnum;
            public string Category;

            public MyMusicTransition(int priority, MyMusicTransitionEnum transitionEnum, string category)
            {
                Priority = priority;
                TransitionEnum = transitionEnum;
                Category = category;
            }
        }

        static MySoundCue? m_musicCue = null;
        static MyMusicState m_musicState = MyMusicState.Stopped;
        static bool m_transitionForward = false;
        static SortedList<int, MyMusicTransition> m_nextTransitions = new SortedList<int, MyMusicTransition>();
        static MyMusicTransition? m_currentTransition;
        static int m_timeFromTransitionStart = 0;                   // in ms
        const int TRANSITION_TIME = 2000;                           // in ms

        //  Number of sound instances (cue) created/added from the application start
        static int m_soundInstancesTotal2D = 0;
        static int m_soundInstancesTotal3D = 0;

        //  Events
        public delegate void EventHandler();
        public delegate void VolumeChangeHandler(float newVolume);
        public static event EventHandler OnStop, OnPause, OnResume;
        public static event VolumeChangeHandler OnSetVolumeGui, OnSetVolumeGame, OnSetVolumeMusic;

        public static MyX3DAudio X3DAudio { get { return m_x3dAudio; } }

#if DEBUG_AUDIO

        static List<Matrix> m_soundCollisions = new List<Matrix>();

        static MyAudio()
        {

            Render.MyRender.RegisterRenderModule("Audio", DebugDraw, Render.MyRenderStage.DebugDraw, 300, true);

        }

#endif

        public static void LoadData()
        {
            MyMwcLog.WriteLine("MyAudio.LoadData - START");
            MyMwcLog.IncreaseIndent();

            m_canPlay = true;
            try
            {
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("new AudioEngine");
                //TimeSpan timeSpan = TimeSpan.FromMilliseconds(250);
                //m_audioEngine = new AudioEngine(MyPlugins.GetAudioFolder() + "Audio.xgs", timeSpan, "");
                using (var file = File.OpenRead(MyPlugins.GetAudioFolder() + "Audio.xgs"))
                {
                    m_audioEngine = new AudioEngine(file);
                    m_cuePool = new MyCuePool(m_audioEngine);
                }

                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("new MyX3DAudio");

                m_x3dAudio = new MyX3DAudio(m_audioEngine);

                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
            }
            catch (Exception ex)
            {
                MyMwcLog.WriteLine("Exception during loading audio engine. Game continues, but without sound. Details: " + ex.ToString(), LoggingOptions.AUDIO);

                //  This exception is the only way I can know if we can play sound (e.g. if computer doesn't have sound card).
                //  I didn't find other ways of checking it.
                m_canPlay = false;
            }

            if (m_canPlay)
            {
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyXactVariables.LoadData");
                MyXactVariables.LoadData();

                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("AddWaveBanks");
                // IMPORTANT!!! Currently, there is problem with multiple XACT projects, that when anything changes in Music.xap or Voice.xap,
                // it is necessary to open and save Sounds.xap also, in order to run the game properly(otherwise it has problem to load the SoundBank.xsb)
                // this will be solved later in future
                AddWaveBanks();

                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("Adding sound banks");

                MyMwcLog.WriteLine("Adding sound banks", LoggingOptions.AUDIO);
                using (var file = File.OpenRead(MyPlugins.GetAudioFolder() + "Music.xsb"))
                {
                    m_musicSoundBank = new SoundBank(m_audioEngine, file);
                }
                using (var file = File.OpenRead(MyPlugins.GetAudioFolder() + "Sounds.xsb"))
                {
                    m_sfxSoundBank = new SoundBank(m_audioEngine, file);
                }
                using (var file = File.OpenRead(MyPlugins.GetAudioFolder() + "Dialogues.xsb"))
                {
                    m_dialogueSoundBank = new SoundBank(m_audioEngine, file);
                }
                using (var file = File.OpenRead(MyPlugins.GetAudioFolder() + "Voice.xsb"))
                {
                    m_voiceSoundBank = new SoundBank(m_audioEngine, file);
                }

                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("Categories");

                //m_voiceSoundBank = new SoundBank(m_audioEngine, MyPlugins.GetAudioFolder() + "Voice.xsb");
                m_listener = new Listener();
                m_listener.SetDefaultValues();
                m_helperEmitter = new Emitter();
                m_helperEmitter.SetDefaultValues();
                m_defaultCategory = m_audioEngine.GetCategoryInstance("Default");
                m_wep2DCategory = m_audioEngine.GetCategoryInstance("wep2d");
                m_cockpitCategory = m_audioEngine.GetCategoryInstance("Cockpit");
                m_musicCategory = m_audioEngine.GetCategoryInstance("Music");
                m_guiCategory = m_audioEngine.GetCategoryInstance("Gui");
                m_doorCategory = m_audioEngine.GetCategoryInstance("Door");
                m_enginesCategory = m_audioEngine.GetCategoryInstance("Engines");
                m_dialogueCategory = m_audioEngine.GetCategoryInstance("Dialogues");
                m_shoutsCategory = m_audioEngine.GetCategoryInstance("Shouts");
                m_hudCategory = m_audioEngine.GetCategoryInstance("VocHud");
                m_ambCategory = m_audioEngine.GetCategoryInstance("Amb");
                m_drillsCategory = m_audioEngine.GetCategoryInstance("Drills");
                m_impCategory = m_audioEngine.GetCategoryInstance("Imp");
                m_importantsCategory = m_audioEngine.GetCategoryInstance("IMPORTANTS");
                m_sfxCategory = m_audioEngine.GetCategoryInstance("Sfx");
                m_voiceCategory = m_audioEngine.GetCategoryInstance("Voice");
                m_welcomeCategory = m_audioEngine.GetCategoryInstance("Welcomes");
                m_wep3DCategory = m_audioEngine.GetCategoryInstance("Wep3D");
                m_xCategory = m_audioEngine.GetCategoryInstance("X");

                m_gameCategories = new List<AudioCategory>();
                m_gameCategories.Add(m_defaultCategory);
                m_gameCategories.Add(m_wep2DCategory);
                m_gameCategories.Add(m_cockpitCategory);
                m_gameCategories.Add(m_doorCategory);
                m_gameCategories.Add(m_enginesCategory);
                m_gameCategories.Add(m_dialogueCategory);
                m_gameCategories.Add(m_shoutsCategory);
                m_gameCategories.Add(m_hudCategory);
                m_gameCategories.Add(m_ambCategory);
                m_gameCategories.Add(m_drillsCategory);
                m_gameCategories.Add(m_impCategory);
                m_gameCategories.Add(m_importantsCategory);
                m_gameCategories.Add(m_sfxCategory);
                m_gameCategories.Add(m_voiceCategory);
                m_gameCategories.Add(m_welcomeCategory);
                m_gameCategories.Add(m_wep3DCategory);
                m_gameCategories.Add(m_xCategory);

                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("WaitForWaveBanks");

                //  AudioEngine.Update needs to be called at least once before a streaming wave bank is ready
                MyMwcLog.WriteLine("Updating audio engine...", LoggingOptions.AUDIO);
                WaitForWaveBanks();

                //  This is reverb turned to off, so we hear sounds as they are defined in wav files
                ReverbControl = 100;

                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("PreloadCueInfo");

                PreloadCueInfo(); // Takes about 26 ms

                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("ValidateCues");

#if DEBUG
                ThreadPool.QueueUserWorkItem(ValidateCue_Thread); // Takes 4000 ms on background thread
#endif

                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("InitCueParameters");

                InitCueParameters();

                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("InitNonLoopableCuesLimitRemoveHelper");

                InitNonLoopableCuesLimitRemoveHelper();

                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("MyHudAudio.LoadData");

                MyHudAudio.LoadData();

                //  Volume from config
                m_musicOn = true;
                m_gameSoundsOn = true;
                m_musicAllowed = true;
                VolumeMusic = MyConfig.MusicVolume;
                VolumeGame = MyConfig.GameVolume;
                VolumeGui = MyConfig.GameVolume;
                MyConfig.MusicVolume = VolumeMusic;
                MyConfig.GameVolume = VolumeGame;

                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("MyConfig.Save");

                MyConfig.Save();

                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
            }

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyAudio.LoadData - END");
        }

        private static void ValidateCue_Thread(object state)
        {
            ValidateCues(); // Takes 4000 ms
        }

        private static void WaitForWaveBanks()
        {
            foreach (var bank in m_waveBanks)
            {
                while (!bank.IsPrepared())
                {
                    m_audioEngine.DoWork();
                }
            }
        }

        private static void PreloadCueInfo()
        {
            int cueCount = MyMwcUtils.GetMaxValueFromEnum<MySoundCuesEnum>() + 1;
            m_cueInfos = new MyCueInfo[cueCount];
            for (int i = 0; i < cueCount; i++)
            {
                string cueName = MyEnumsToStrings.Sounds[i];
                SoundBank soundBank = GetSoundBank(cueName);
                short cueIndex = soundBank.GetCueIndex(cueName);
                m_cueInfos[i] = new MyCueInfo() { SoundBank = soundBank, CueIndex = cueIndex };
            }
        }

        //  USE THIS METHOD TO ADD MORE WAVEBANKS!!!
        public static void UnloadData()
        {
            MyMwcLog.WriteLine("MyAudio.UnloadData - START");

            MyHudAudio.UnloadData();

            //  This will quiet down the sounds and music immediatelly so we won't hear squeaking when starting background unload thread
            if (m_audioEngine != null)
            {
                VolumeGame = 0;
                VolumeMusic = 0;
                m_audioEngine.DoWork();
            }

            //  We need to dispose sounds because when unloading whole game-play screen, we need to turn-off any currently played sounds
            if (m_waveBanks != null)
            {
                foreach (WaveBank waveBank in m_waveBanks)
                {
                    if (!waveBank.IsDisposed)
                        waveBank.Dispose();
                }
                m_waveBanks.Clear();
                m_waveBanks = null;
            }

            if (m_sfxSoundBank != null) m_sfxSoundBank.Dispose();
            if (m_musicSoundBank != null) m_musicSoundBank.Dispose();
            if (m_dialogueSoundBank != null) m_dialogueSoundBank.Dispose();
            if (m_voiceSoundBank != null) m_voiceSoundBank.Dispose();
            //if (m_voiceSoundBank != null) m_voiceSoundBank.Dispose();

            m_x3dAudio = null;

            m_cuePool.Dispose();

            if (m_audioEngine != null) m_audioEngine.Dispose();

            MyXactVariables.UnloadData();

            MyMwcLog.WriteLine("MyAudio.UnloadData - END");
        }

        public static void UnloadContent()
        {
            MyMwcLog.WriteLine("MyAudio.UnloadContent - START");
            MyMwcLog.IncreaseIndent();

            ClearMusicTransitions();
            foreach (var item in m_collisionSoundsDictionary.Values)
            {
                item.Clear();
            }

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyAudio.UnloadContent - END");
        }

        static void AddWaveBanks()
        {
            //wavebanks issue - http://social.msdn.microsoft.com/forums/en-US/xnaframework/thread/85594964-8220-483c-a27f-8bc10f112a42/

            // Important - count is used, when there are multiple wavebanks having same name, but they have number suffix(MusMainMenu01, MusMainMenu02 etc.)
            //AddWaveBanksForName("Voice", 1, ref m_waveBanks, 16);
            AddWaveBanksForName("SfxGeneral", 1, ref m_waveBanks, null);
            AddWaveBanksForName("SfxAmbient", 1, ref m_waveBanks, 16);
            //AddWaveBanksForName("MusMainMenu", MUS_MAIN_MENU_WAVES_COUNT, ref m_waveBanks, 16);
            //AddWaveBanksForName("MusStoryAmbient", MUS_STORY_AMBIENT_WAVES_COUNT, ref m_waveBanks, 16);
            //AddWaveBanksForName("Music", 1, ref m_waveBanks, 16);
            AddWaveBanksForName("MusicCalm", 1, ref m_waveBanks, 16);
            AddWaveBanksForName("MusicDesperate", 1, ref m_waveBanks, 16);
            AddWaveBanksForName("MusicHeavyFight", 1, ref m_waveBanks, 16);
            AddWaveBanksForName("MusicHorror", 1, ref m_waveBanks, 16);
            AddWaveBanksForName("MusicLightFight", 1, ref m_waveBanks, 16);
            AddWaveBanksForName("MusicMystery", 1, ref m_waveBanks, 16);
            AddWaveBanksForName("MusicSadness", 1, ref m_waveBanks, 16);
            AddWaveBanksForName("MusicSpecial", 1, ref m_waveBanks, 16);
            AddWaveBanksForName("MusicStealth", 1, ref m_waveBanks, 16);
            AddWaveBanksForName("MusicStress", 1, ref m_waveBanks, 16);
            AddWaveBanksForName("MusicTension", 1, ref m_waveBanks, 16);
            AddWaveBanksForName("MusicTrailer", 1, ref m_waveBanks, 16);
            AddWaveBanksForName("MusicVictory", 1, ref m_waveBanks, 16);
            AddWaveBanksForName("Welcomes", 1, ref m_waveBanks, 16);
            AddWaveBanksForName("Dialogues", 1, ref m_waveBanks, 16);
            AddWaveBanksForName("Dialogues2", 1, ref m_waveBanks, 16);
            AddWaveBanksForName("Dialogues3", 1, ref m_waveBanks, 16);
            AddWaveBanksForName("Dialogues4", 1, ref m_waveBanks, 16);
            AddWaveBanksForName("Dialogues5", 1, ref m_waveBanks, 16);
            AddWaveBanksForName("Dialogues6_1", 1, ref m_waveBanks, 16);
            AddWaveBanksForName("Dialogues6_2", 1, ref m_waveBanks, 16);
            AddWaveBanksForName("Dialogues7", 1, ref m_waveBanks, 16);
            AddWaveBanksForName("Dialogues8a", 1, ref m_waveBanks, 16);
            AddWaveBanksForName("Dialogues8b", 1, ref m_waveBanks, 16);
            AddWaveBanksForName("Dialogues8c", 1, ref m_waveBanks, 16);
            AddWaveBanksForName("Dialogues8d", 1, ref m_waveBanks, 16);
            AddWaveBanksForName("Dialogues8e", 1, ref m_waveBanks, 16);
            AddWaveBanksForName("Dialogues9a", 1, ref m_waveBanks, 16);
            AddWaveBanksForName("Dialogues9b", 1, ref m_waveBanks, 16);
            AddWaveBanksForName("Dialogues9c", 1, ref m_waveBanks, 16);
            AddWaveBanksForName("Dialogues9d", 1, ref m_waveBanks, 16);
            AddWaveBanksForName("Dialogues9e", 1, ref m_waveBanks, 16);
            AddWaveBanksForName("Dialogues10", 1, ref m_waveBanks, 16);
            AddWaveBanksForName("Dialogues12", 1, ref m_waveBanks, 16);
            AddWaveBanksForName("Dialogues13", 1, ref m_waveBanks, 16);
            AddWaveBanksForName("Dialogues14", 1, ref m_waveBanks, 16);
            AddWaveBanksForName("Dialogues15", 1, ref m_waveBanks, 16);
            AddWaveBanksForName("Dialogues16", 1, ref m_waveBanks, 16);
            AddWaveBanksForName("Dialogues17", 1, ref m_waveBanks, 16);
            AddWaveBanksForName("Dialogues18a", 1, ref m_waveBanks, 16);
            AddWaveBanksForName("Dialogues18b", 1, ref m_waveBanks, 16);
            AddWaveBanksForName("Dialogues18c", 1, ref m_waveBanks, 16);
            AddWaveBanksForName("Dialogues19", 1, ref m_waveBanks, 16);
            AddWaveBanksForName("Dialogues20", 1, ref m_waveBanks, 16);
            AddWaveBanksForName("Dialogues21", 1, ref m_waveBanks, 16);
            AddWaveBanksForName("Dialogues22", 1, ref m_waveBanks, 16);
            AddWaveBanksForName("Voice", 1, ref m_waveBanks, 16);
        }

        static void AddWaveBanksForName(string name, int count, ref List<WaveBank> addToList, short? packetSize)
        {
            MyMwcLog.WriteLine("AddWaveBanksForName for: " + name, LoggingOptions.AUDIO);

            if (addToList == null) addToList = new List<WaveBank>();
            string waveBankPath = MyPlugins.GetAudioFolder() + name;
            if (count == 1)
            {
                addToList.Add(CreateWaveBank(waveBankPath + WAVEBANK_SUFFIX, packetSize));
            }
            else
            {
                for (int i = 1; i <= count + 1; i++)
                {
                    string counter = i < 10 ? "0" + i : i.ToString();
                    string waveBankCompletePath = waveBankPath + counter + WAVEBANK_SUFFIX;
                    addToList.Add(CreateWaveBank(waveBankCompletePath, packetSize));
                }
            }
        }

        static WaveBank CreateWaveBank(string bankFileName, short? packetSize)
        {
            try
            {
                if (packetSize.HasValue)
                {
                    return new WaveBank(m_audioEngine, bankFileName, 0, packetSize.Value);
                }
                else
                {
                    return MyInMemoryWaveBank.Create(m_audioEngine, bankFileName);
                }
            }
            catch (OutOfMemoryException)
            {
                string error = string.Format("Out of memory when creating new wave bank.\nDebug data:\nbankFileName {0}\npacketSize {1}", bankFileName, packetSize);
                MyCommonDebugUtils.AssertDebug(false, error);
                MyMwcLog.WriteLine(error);
                throw;
            }
        }

        public static bool Mute = false;

        //  Updates the state of the 3D audio system.
        public static void Update(bool ignoreGameReadyStatus = false)
        {
            if (!MyMinerGame.IsGameReady && !ignoreGameReadyStatus)
                return;

            //Moved m_cuePool.Update before Mute test, otherwise cues werent destroyed
            if (m_cuePool != null)
                m_cuePool.Update();

            if (Mute) return;

            if (MyMinerGame.IsPaused() == false && m_canPlay)
            {
                UpdateCollisionCues();
            }

            if (m_canPlay)
            {
                // Tell the AudioManager about the new camera position.
                m_listener.Position = SharpDXHelper.ToSharpDX(MyCamera.Position);
                m_listener.OrientFront = -SharpDXHelper.ToSharpDX(MyCamera.ForwardVector);
                m_listener.OrientTop = SharpDXHelper.ToSharpDX(MyCamera.UpVector);
                m_listener.Velocity = SharpDXHelper.ToSharpDX(MyCamera.Velocity);
                //  Update the XACT engine.
                if (!MySystemTimer.IsSetMinResolution())
                {
                    Render.MyRender.GetRenderProfiler().StartProfilingBlock("audioEngine.Update");
                    //TODO: 0.5ms 
                    m_audioEngine.DoWork();
                    Render.MyRender.GetRenderProfiler().EndProfilingBlock();
                }
                Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyAudio.UpdateHudCues");
                //TODO
                //MyHudAudio.Update();
                UpdateMusic();
              
                Render.MyRender.GetRenderProfiler().EndProfilingBlock();
            }
        }

        //  Use this if you want to use default volume level -> this is the prefered way because in 99% cases we don't want to set per-sound volume
        public static MySoundCue? AddCue2dOr3d(MyEntity physObject, MySoundCuesEnum cueEnum,
            Vector3 position, Vector3 forward, Vector3 up, Vector3 velocity)
        {
            return AddCue2dOr3d(physObject, cueEnum, position, forward, up, velocity, 1);
        }

        //  This method decides if player sits inside this object and if yes, play 2D stereo sound.
        //  If not then 3D mono sound.
        public static MySoundCue? AddCue2dOr3d(MyEntity physObject, MySoundCuesEnum cueEnum,
            Vector3 position, Vector3 forward, Vector3 up, Vector3 velocity, float volume)
        {
            if (!MyMinerGame.IsGameReady)
                return null;

            MySoundCue? ret = null;

            if (m_canPlay == false) return null;

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("AddCue2dOr3d");

            bool play3D = true;
            if ((physObject == MySession.PlayerShip && MyGuiScreenGamePlay.Static != null && MyGuiScreenGamePlay.Static.CameraAttachedTo == MyCameraAttachedToEnum.PlayerMinerShip) ||
                (MyGuiScreenGamePlay.Static != null && physObject == MyGuiScreenGamePlay.Static.ControlledEntity && MyGuiScreenGamePlay.Static.CameraAttachedTo != MyCameraAttachedToEnum.Spectator))
            {
                //  Find 2D version for this sound
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("GetVersion2D");
                cueEnum = GetVersion2D(cueEnum);
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
                play3D = false;
            }

            if (play3D)
            {
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("AddCue3d");
                ret = AddCue3D(cueEnum, position, forward, up, velocity, volume);
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
            }
            else
            {
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("AddCue2d");
                ret = AddCue2D(cueEnum, volume);
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            return ret;
        }

        //  Use this if you want to use default volume level -> this is the prefered way because in 99% cases we don't want to set per-sound volume
        public static MySoundCue? AddCue2D(MySoundCuesEnum cueEnum)
        {
            return AddCue2D(cueEnum, 1);
        }

        public static void WriteDebugInfo(StringBuilder builder)
        {
            if(m_cuePool != null)
                m_cuePool.WriteDebugInfo(builder);
        }

        //  Add new cue and starts playing it. This can be used for one-time or for looping cues. It depends how they are set in XACT.
        //  Method returns reference to the cue, so if it's looping cue, we can update its position. Or we can stop playing it.
        //  These are 2D cues played with stereo. Used for in-cockpit sounds.
        //  We don't do any max instance limiting or distance cutting, because it's not needed.
        public static MySoundCue? AddCue2D(MySoundCuesEnum cueEnum, float volume)
        {
            //  If this computer can't play sound, we don't create cues
            if (m_canPlay == false) return null;

            if (!MyMinerGame.IsGameReady)
                return null;

            MySoundCue? result = null;

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyAudio.AddCue2D");

            int cueIndex = (int)cueEnum;
            MyCueParameters cueParameters = m_cueParameters[(int)cueEnum].Value;
            if (cueParameters.IsHudCue)
            {
                MyHudAudio.AddHudCue(cueEnum, volume);
            }
            else
            {
                result = PlayCueNow2D(cueEnum, volume);
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            return result;
        }

        public static MySoundCue PlayCueNow2D(MySoundCuesEnum cueEnum, float volume)
        {
            var cue = GetCue(cueEnum);
            cue.SetVariable(MyCueVariableEnum.Volume, volume);
            cue.Play();

            m_soundInstancesTotal2D++;

            return m_cuePool.CreateCue(cue, cueEnum, false);
        }

        public static AudioCategory GetDefaultCategory()
        {
            return m_defaultCategory;
        }

        public static AudioCategory GetCockpitCategory()
        {
            return m_cockpitCategory;
        }

        //  Use this if you want to use default volume level -> this is the prefered way because in 99% cases we don't want to set per-sound volume
        public static MySoundCue? AddCue3D(MySoundCuesEnum cueEnum, Vector3 position, Vector3 forward, Vector3 up, Vector3 velocity)
        {
            return AddCue3D(cueEnum, position, forward, up, velocity, 1);
        }

        //  Add new cue and starts playing it. This can be used for one-time or for looping cues. It depends how they are set in XACT.
        //  Method returns reference to the cue, so if it's looping cue, we can update its position. Or we can stop playing it.
        //  If we use it for one-time cue, sound is played once at specified position/velocity/etc and then is destroyed.
        //  Volume: look into UpdateCueVolume() for intervals. If pitch not specified, default 'as authored' is used.
        public static MySoundCue? AddCue3D(MySoundCuesEnum cueEnum, Vector3 position, Vector3 forward, Vector3 up, Vector3 velocity, float volume)
        {
            //  If this computer can't play sound, we don't create cues
            if (m_canPlay == false)
                return null;

            if (!MyMinerGame.IsGameReady)
                return null;


            int cueIndex = (int)cueEnum;

            //  If this is one-time cue, we check if it is close enough to hear it and if not, we don't even play - this is for optimization only.
            //  We must add loopable cues always, because if source of cue comes near the camera, we need to update the position, but of course we can do that only if we have reference to it.
            MyCueParameters cueParams = m_cueParameters[cueIndex].Value;
            Debug.Assert(!cueParams.IsHudCue, "Hud cue can't be played as 3d sound!");
            float distanceToSound = Vector3.Distance(MyCamera.Position, position);

            if ((cueParams.Loopable == false) && (distanceToSound > cueParams.MaxDistance))
            {
                return null;
            }

            bool doLimitMaxCuesAndIsNotLoopable = (MyAudioConstants.LIMIT_MAX_SAME_CUES == true) && (cueParams.Loopable == false);

            //  Max numbers of instances for this cues have been reached, must wait until one ends
            if ((doLimitMaxCuesAndIsNotLoopable) && (m_nonLoopableCuesLimit[cueIndex].Count >= MyAudioConstants.MAX_SAME_CUES_PLAYED))
                return null;

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyAudio.AddCue3D");

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("GetCueByIndex");

            Cue cue = GetCue(cueEnum);

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("SetVariableNoGarbage");

            //  Set volume. From XNA 3.1 (XACT 3.0) this seems to be mandatory, so at least set it to 1.0 (default volume)
            cue.SetVariable(MyCueVariableEnum.Volume, volume);

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("UpdateCuePosition");

            if (UseOcclusion(cueEnum))
            {
                cue.SetVariable(MyCueVariableEnum.Occluder, CalculateOcclusion(ref position));
            }

            m_helperEmitter.UpdateValues(ref position, ref forward, ref up, ref velocity);
            cue.Apply3D(m_listener, m_helperEmitter);

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("Play");
            //  Play the cue
            cue.Play();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("MySoundCue");
            MySoundCue soundCue = m_cuePool.CreateCue(cue, cueEnum, true);

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            if (doLimitMaxCuesAndIsNotLoopable)
                m_nonLoopableCuesLimit[cueIndex].Add(new WeakReference(cue));

            m_soundInstancesTotal3D++;
            return soundCue;
        }

        private static void CheckCue(MySoundCue cue)
        {
            Debug.Assert(cue.IsValid, "This method can't be called with this cue, because cue is disposed");
        }

        //  Updates the position and velocity settings of a 3D cue
        public static void UpdateCuePosition(MySoundCue? cue, Vector3 position, Vector3 forward, Vector3 up, Vector3 velocity)
        {
            if (cue.HasValue == false) return;
            UpdateCuePosition(cue.Value, position, forward, up, velocity);
        }

        public static void UpdateCuePosition(MySoundCue cue, Vector3 position, Vector3 forward, Vector3 up, Vector3 velocity)
        {
            if (m_canPlay == false) return;
            if (!cue.IsValid) return;
            if (!cue.Is3D) return;
            CheckCue(cue);

            if (!MyFakes.OPTIMIZATION_FOR_300_SMALLSHIPS)
            {
                CalculateOcclusion(cue, position);
            }

            m_helperEmitter.UpdateValues(ref position, ref forward, ref up, ref velocity);

            cue.Apply3D(m_listener, m_helperEmitter);
        }

        static bool UseOcclusion(MySoundCuesEnum cueEnum)
        {
            return m_cueParameters[(int)cueEnum].Value.UseOcclusion;
        }

        public static float CalculateOcclusion(ref Vector3 position)
        {
            // Occlusions are disabled
            return 0.0f;

            MyLine m_traceLine = new MyLine(MyCamera.Position, position, true);
            MyEntity physObject = MyEntities.TryGetPhysObjectAtPosition(position);

            // Do not set occlusion for cues of playership itself
            if (physObject != MySession.PlayerShip && MyEntities.GetIntersectionWithLine(ref m_traceLine, MySession.PlayerShip, physObject).HasValue)
            {
                return 7.0f;
            }
            return 0.0f;
        }

        public static void CalculateOcclusion(MySoundCue cue, Vector3 position)
        {
            // Occlusions are disabled
            return;

            if (MyFakes.OPTIMIZATION_FOR_300_SMALLSHIPS) return;
            if (m_canPlay == false) return;
            CheckCue(cue);

            MyCueParameters cueParams = m_cueParameters[(int)cue.CueEnum].Value;
            if (!cueParams.UseOcclusion) return;
            if (MyMinerGame.TotalGamePlayTimeInMilliseconds - cueParams.LastUpdate < MyAudioConstants.OCCLUSION_INTERVAL) return;
            cueParams.LastUpdate = MyMinerGame.TotalGamePlayTimeInMilliseconds;

            float distance = Vector3.Distance(MyCamera.Position, position);
            if ((distance <= cueParams.MaxDistance) && (distance > MyMwcMathConstants.EPSILON))
            {
                cue.SetVariable(MyCueVariableEnum.Occluder, CalculateOcclusion(ref position));
            }
        }

        //  Updates pitch of a cue (this is per instance pitch, not all sounds pitch). Volume values are in interval <0..1..2> -> same as for master pitch (see below).
        //  IMPORTANT: Every cue for which we want to set pitch needs to have RPC for it. This can be done in XACT.
        public static void UpdateCueVolume(MySoundCue? cue, float volume)
        {
            if (m_canPlay == false) return;
            if (cue == null) return;
            CheckCue(cue.Value);

            cue.Value.SetVariable(MyCueVariableEnum.Volume, volume);
        }

        //  Updates pitch of a cue (this is per instance pitch, not all sounds pitch). Volume values are in interval <0..1..2> -> same as for master pitch (see below).
        //  IMPORTANT: Every cue for which we want to set pitch needs to have RPC for it. This can be done in XACT.
        public static void UpdateCueAmbVolume(MySoundCue? cue, float volume)
        {
            if (m_canPlay == false) return;
            if (cue == null) return;
            CheckCue(cue.Value);

            cue.Value.SetVariable(MyCueVariableEnum.AmbVolume, volume);
        }

        //  Updates pitch of a cue (this is per instance pitch, not all sounds pitch). Rotation speed values are in interval <0..100> -> it's normalized rotation speed
        //  IMPORTANT: Every cue for which we want to set pitch needs to have RPC for it. This can be done in XACT.
        public static void UpdateCueRotationSpeed(MySoundCue? cue, float rotationSpeed)
        {
            if (m_canPlay == false) return;
            if (cue == null) return;
            CheckCue(cue.Value);

            cue.Value.SetVariable(MyCueVariableEnum.RotatingSpeed, rotationSpeed);
        }

        //  Updates pitch of a cue (this is per instance pitch, not all sounds pitch). Pitch values are in interval <-1..+1>.
        //  IMPORTANT: Every cue for which we want to set pitch needs to have RPC for it. This can be done in XACT.
        public static void UpdateCuePitch(MySoundCue? cue, float pitch)
        {
            if (m_canPlay == false) return;
            if (cue == null) return;
            CheckCue(cue.Value);

            cue.Value.SetVariable(MyCueVariableEnum.Pitch, pitch);
        }

        //  Retrieve cue from corresponding soundbank based on its index(first get name based on index, then get soundbank from name)
        // Cues are not necessary to be destroyed, it gets destroyed in AudioEngine.DoWork as soon as they finish playing, call Destroy to free looping cues or not started cues.
        static Cue GetCue(MySoundCuesEnum cueEnum)
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Cue.Prepare");
            Cue cue = m_cueInfos[(int)cueEnum].Prepare();
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            return cue;
        }

        private static SoundBank GetSoundBank(string cueName)
        {
            SoundBank soundBank = null;
            if (cueName.StartsWith(GROUP_MUS))
            {
                soundBank = m_musicSoundBank;
            }
            else if (cueName.StartsWith(GROUP_DLG) || cueName.StartsWith(GROUP_SHT))
            {
                soundBank = m_dialogueSoundBank;
            }
            else if (cueName.StartsWith(GROUP_HUD) || cueName.StartsWith(GROUP_VOC))
            {
                soundBank = m_voiceSoundBank;
            }
            //else if (cueName.StartsWith(GROUP_VOC))
            //{
            //    soundBank = m_voiceSoundBank;
            //}
            else
            {
                soundBank = m_sfxSoundBank;
            }
            return soundBank;
        }

        // Turn ON/OFF game music - may change in the future(currently setting volume to 0)
        public static bool MusicOn
        {
            get
            {
                return m_musicOn;
            }

            set
            {
                if (value)
                {
                    m_musicOn = value;
                    VolumeMusic = MyConfig.MusicVolume;
                }
                else
                {
                    VolumeMusic = 0;
                    m_musicOn = value;
                }
            }
        }

        // Turn ON/OFF game sounds - may change in the future(currently setting volume to 0)
        // Does not include Gui category sounds, must be enabled in certain situations(credits screen for example),
        // while the in-game sounds must be disabled at the same time.
        public static bool GameSoundsOn
        {
            get
            {
                return m_gameSoundsOn;
            }

            set
            {
                if (value)
                {
                    m_gameSoundsOn = value;
                    VolumeGame = MyConfig.GameVolume;
                }
                else
                {
                    VolumeGame = 0;
                    m_gameSoundsOn = value;
                }
            }
        }

        //  Set/get master volume for all sounds/cues in "Gui" category.
        //  Interval <0..1..2>
        //      0.0f  ->   -96 dB (silence) 
        //      1.0f  ->    +0 dB (full pitch as authored) 
        //      2.0f  ->    +6 dB (6 dB greater than authored) 
        public static float VolumeGui
        {
            get
            {
                if (m_canPlay == false) return 0;

                return m_volumeGui;
            }

            set
            {
                if (m_canPlay == false) return;

                //  We need to clamp the pitch, because app fails if we set it with zero value
                m_volumeGui = MathHelper.Clamp(value, MyAudioConstants.GAME_MASTER_VOLUME_MIN, MyAudioConstants.GAME_MASTER_VOLUME_MAX);
                m_guiCategory.SetVolume(m_volumeGui);
                if (OnSetVolumeGui != null) OnSetVolumeGui(m_volumeDefault);
            }
        }

        //  Set/get master volume for all in-game sounds/cues.
        //  Interval <0..1..2>
        //      0.0f  ->   -96 dB (silence) 
        //      1.0f  ->    +0 dB (full pitch as authored) 
        //      2.0f  ->    +6 dB (6 dB greater than authored) 
        public static float VolumeGame
        {
            get
            {
                if (m_canPlay == false || !m_gameSoundsOn) return 0;

                return m_volumeDefault;
            }

            set
            {
                SetVolumeExceptCategory(value, null);
            }
        }

        public static void SetVolumeExceptDialogues(float volume)
        {
            SetVolumeExceptCategory(volume, m_dialogueCategory);
        }

        private static void SetVolumeExceptCategory(float volume, AudioCategory? ignoreCategory)
        {
            if (m_canPlay == false || !m_gameSoundsOn) return;

            //  We need to clamp the pitch, because app fails if we set it with zero value
            m_volumeDefault = MathHelper.Clamp(volume, MyAudioConstants.GAME_MASTER_VOLUME_MIN, MyAudioConstants.GAME_MASTER_VOLUME_MAX);
            foreach (AudioCategory category in m_gameCategories)
            {
                if (ignoreCategory.HasValue && category == ignoreCategory.Value)
                    continue;

                category.SetVolume(m_volumeDefault);
            }

            if (OnSetVolumeGame != null) OnSetVolumeGame(m_volumeDefault);
        }


        //  Set/get master volume for all sounds/cues for "Music" category.
        //  Interval <0..1..2>
        //      0.0f  ->   -96 dB (silence) 
        //      1.0f  ->    +0 dB (full pitch as authored) 
        //      2.0f  ->    +6 dB (6 dB greater than authored) 
        public static float VolumeMusic
        {
            get
            {
                if (m_canPlay == false || !m_musicOn) return 0;

                return m_volumeMusic;
            }

            set
            {
                if (m_canPlay == false || !m_musicOn) return;

                //  We need to clamp the pitch, because app fails if we set it with zero value
                m_volumeMusic = MathHelper.Clamp(value, MyAudioConstants.MUSIC_MASTER_VOLUME_MIN, MyAudioConstants.MUSIC_MASTER_VOLUME_MAX);
                m_musicCategory.SetVolume(m_volumeMusic);
                if (OnSetVolumeMusic != null) OnSetVolumeMusic(m_volumeDefault);
            }
        }

        //  Use this property to set/get value of reverb. Its interval is <0..100>, where 0 is no-reverb (just default sounds), and 100 is full-reverb (using preset as specified in XACT).
        //  IMPORTANT: Reverb value isn't linear. I think it is something like decibels, so values between 0..90 make almost no reverb, so real reverb start at 90 and best is at 100.
        //  So we can't have many reverb environments, we can have only two, but using this property we can switch between them.
        //  Btw, it's possible to have more types of environment, but we have to make RPC variable for each reverb variable, and it's about 20 variables, so I don't want to do it.
        public static float ReverbControl
        {
            get
            {
                if (m_canPlay == false) return 0;

                return m_reverbControl;
            }
            set
            {
                if (m_canPlay == false) return;

                m_reverbControl = MathHelper.Clamp(value, 0, MyAudioConstants.REVERB_MAX);
                m_audioEngine.SetGlobalVariable(MyGlobalVariableEnum.ReverbControl, m_reverbControl);
            }
        }

        //  This method detects all new collisions/impact and plays cue based on strength of the collision. We check duplicity, so while
        //  collision cue is playing, we don't play it again.
        //  IMPORTANT: This method must be caled only after physics integration (so collisions are calculated)
        static void UpdateCollisionCues()
        {
            if (MyMwcFinalBuildConstants.ENABLE_COLLISION_CUES_AND_PARTICLES == false) return;

            Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyAudio.UpdateCollisionCues");

            FreeCollisionCues();
            FreeLoopableCuesLimit();

            Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        //  Plays one-time cue of impact/collision/hit between two objects. Logic behind will preserve that only one sound is played, even if coldet routine gives us this collision two times.
        //  We also check deceleration needed for playing the sound and if we aren't currently playing sound of collision for this two objects.
        internal static void PlayCollisionCue(MyContactEventInfo contactEventInfo, MyMaterialsConstants.MyMaterialCollisionType type)
        {
            var physicsObject0 = (MyPhysicsBody)contactEventInfo.m_RigidBody1.m_UserData;
            var physicsObject1 = (MyPhysicsBody)contactEventInfo.m_RigidBody2.m_UserData;

            //  We won't add more collision sounds if dictionary is full (we need to wait until some cues are removed)
            if (m_collisionDictionary.Count > MyAudioConstants.MAX_COLLISION_SOUNDS)
            {
                if ((physicsObject0.Entity != MySession.PlayerShip) && (physicsObject1.Entity != MySession.PlayerShip))
                {
                    return;
                }
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("PlayCollisionCue");


            Vector3 ZERO_CUE_VALUES = Vector3.Zero;
            MySoundCue? cue0 = null;
            float volume = 1;// MathHelper.Lerp(MySoundsConstants.DECELERATION_MIN_VOLUME, MySoundsConstants.DECELERATION_MAX_VOLUME, deceleration / MySoundsConstants.MAX_DECELERATION);

            MyMaterialType materialType1 = physicsObject0.MaterialType;
            MyMaterialType materialType2 = physicsObject1.MaterialType;

            if (physicsObject0.Entity == MySession.PlayerShip)
                materialType1 = MyMaterialType.PLAYERSHIP;

            if (physicsObject1.Entity == MySession.PlayerShip)
                materialType2 = MyMaterialType.PLAYERSHIP;


            if (type == MyMaterialsConstants.MyMaterialCollisionType.End && m_collisionDictionary.ContainsKey(contactEventInfo.m_Guid))
            {
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("cue.Stop");

                //We are already playing this collision and we need to end it with specific sound

                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("AddCue3D");

                MySoundCuesEnum? collisionSound = MyMaterialsConstants.GetCollisionCue(MyMaterialsConstants.MyMaterialCollisionType.End, materialType1, materialType2);
                if (collisionSound.HasValue)
                {
                    MySoundCue cue = m_collisionDictionary[contactEventInfo.m_Guid];
                    cue.Stop(StopFlags.Release);

                    cue0 = AddCue3D(collisionSound.Value, contactEventInfo.m_ContactPoint, ZERO_CUE_VALUES,
                            ZERO_CUE_VALUES, ZERO_CUE_VALUES, volume);
                }

                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
            }
            else
            {
                if (type == MyMaterialsConstants.MyMaterialCollisionType.Start)
                {
                    //  If we aren't already playing collision cue for this particular combination of physical objects, now is the time
                    if (!m_collisionDictionary.ContainsKey(contactEventInfo.m_Guid))
                    {
                        MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Start audio");

                        MySoundCuesEnum? collisionSound = MyMaterialsConstants.GetCollisionCue(MyMaterialsConstants.MyMaterialCollisionType.Start, materialType1, materialType2);
                        if (collisionSound.HasValue)
                        {
                            List<int> soundInstances;
                            if (!m_collisionSoundsDictionary.TryGetValue((int)collisionSound.Value, out soundInstances))
                            {
                                m_collisionSoundsDictionary.Add((int)collisionSound, soundInstances = new List<int>());

                            }
                            if (soundInstances.Count < MyAudioConstants.MAX_COLLISION_SOUNDS_PER_SECOND)
                            {
                                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("AddCue3D");
                                cue0 = AddCue3D(collisionSound.Value, contactEventInfo.m_ContactPoint, ZERO_CUE_VALUES,
                                    ZERO_CUE_VALUES, ZERO_CUE_VALUES, volume);
                                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
                                if (cue0.HasValue)
                                {
                                    soundInstances.Add(MyMinerGame.TotalTimeInMilliseconds);
                                }
                            }
                        }

                        MyMaterialTypeProperties materialProps1 = MyMaterialsConstants.GetMaterialProperties(materialType1);
                        MyMaterialTypeProperties materialProps2 = MyMaterialsConstants.GetMaterialProperties(materialType2);

                        MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

                        Vector3 particleDir = contactEventInfo.m_Velocity1.LengthSquared() > contactEventInfo.m_Velocity2.LengthSquared() ? contactEventInfo.m_Velocity1 - contactEventInfo.m_Velocity2 : contactEventInfo.m_Velocity2 - contactEventInfo.m_Velocity1;
                        if (MyMwcUtils.IsZero(particleDir.LengthSquared()))
                        {
                            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
                            return; //it is valid because of collision in rotation
                        }

                        MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Collision particles");

                        particleDir = -MyMwcUtils.Normalize(particleDir);

                        if (Vector3.DistanceSquared(MyCamera.Position, contactEventInfo.m_ContactPoint) < 100 * 100)
                        {
                            //  Create smoke particles at the place of collision
                            bool doSparks = materialProps1.DoSparksOnCollision || materialProps2.DoSparksOnCollision;
                            bool doSmoke = materialProps1.DoSmokeOnCollision || materialProps2.DoSmokeOnCollision;
                            MyParticleEffects.CreateCollisionParticles(contactEventInfo.m_ContactPoint, particleDir, false, doSparks);
                            MyParticleEffects.CreateCollisionParticles(contactEventInfo.m_ContactPoint, contactEventInfo.m_ContactNormal, doSmoke, false);
                        }

                        MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

#if DEBUG_AUDIO
                        Matrix colMatrix = Matrix.CreateWorld(contactEventInfo.m_ContactPoint, particleDir, Vector3.Up);
                        m_soundCollisions.Add(colMatrix);                         
#endif

                    }
                    else if (type == MyMaterialsConstants.MyMaterialCollisionType.Touch)
                    {
                        MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Touch");

                        MySoundCuesEnum? scrapeSound = MyMaterialsConstants.GetCollisionCue(MyMaterialsConstants.MyMaterialCollisionType.Touch, materialType1, materialType2);
                        if (scrapeSound.HasValue)
                        {
                            cue0 = AddCue3D(scrapeSound.Value, contactEventInfo.m_ContactPoint, ZERO_CUE_VALUES,
                                ZERO_CUE_VALUES, ZERO_CUE_VALUES, volume);
                        }

                        MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

                        //  Create smoke particles at the place of collision
                        //MyParticleEffects.CreateCollisionParticles(contactEventInfo.m_ContactPoint, physicsObject1.GetVelocity() - physicsObject0.GetVelocity());
                    }

                    if (cue0.HasValue)
                    {
                        if (!m_collisionDictionary.ContainsKey(contactEventInfo.m_Guid))
                        {
                            m_collisionDictionary.Add(contactEventInfo.m_Guid, cue0.Value);
                        }
                    }
                }
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        //  Remove stoped cues from collisions dictionary. Call it at the end of every update.
        static void FreeCollisionCues()
        {
            //  First fill helper list with keys we need to remove (because we can't remove items while in foreach)
            m_collisionDictionaryRemovals.Clear();
            foreach (KeyValuePair<uint, MySoundCue> kvp in m_collisionDictionary)
            {
                //  If cue stoped playing or if it was never played (in case of collision between two objects of same material), we remove it from collision dictionary,
                //  so cue collision between these same objects can be played again
                if (kvp.Value.IsPlaying || !kvp.Value.IsValid)
                {
                    m_collisionDictionaryRemovals.Add(kvp.Key);
                }
            }

            //  Then remove from dictionary
            foreach (uint key in m_collisionDictionaryRemovals)
            {
                m_collisionDictionary.Remove(key);
            }

            foreach (var item in m_collisionSoundsDictionary.Values)
            {
                int i = 0;
                while (i < item.Count)
                {
                    if (item[i] < MyMinerGame.TotalTimeInMilliseconds - 1000) // TimeSpan.FromSeconds(1).TotalMilliseconds
                    {
                        item.RemoveAt(i);
                        continue;
                    }
                    i++;

                }
            }
        }

        //  Remove stoped cues from this array. This is for limiting max number of same cues played at the same time.
        static List<WeakReference> deleteList = new List<WeakReference>();

        static void FreeLoopableCuesLimit()
        {
            if (MyAudioConstants.LIMIT_MAX_SAME_CUES == true)
            {
                //  Iterate all possible cues
                for (int i = 0; i < m_nonLoopableCuesLimit.Length; i++)
                {
                    if (m_cueParameters[i] == null)
                        continue;
                    if (m_cueParameters[i].Value.Loopable == false)
                    {
                        deleteList.Clear();
                        foreach (var cue in m_nonLoopableCuesLimit[i])
                        {
                            if (cue.IsAlive)
                            {
                                if (IsCueFinished((Cue)cue.Target))
                                    deleteList.Add(cue);
                            }
                            else
                            {
                                deleteList.Add(cue);
                            }
                        }

                        foreach (var cue in deleteList)
                        {
                            m_nonLoopableCuesLimit[i].Remove(cue);
                        }
                    }
                }
            }
        }

        static void ClearMusicTransitions()
        {
            m_musicState = MyMusicState.Stopped;
            m_musicCue = null;
            //m_nextTransition = null;
            m_nextTransitions.Clear();
            m_currentTransition = null;
        }

        static private bool IsCueFinished(Cue cue)
        {
            return cue.IsPlaying() == false;
        }

        public static int GetSoundInstancesTotal2D()
        {
            return m_soundInstancesTotal2D;
        }

        public static int GetSoundInstancesTotal3D()
        {
            return m_soundInstancesTotal3D;
        }

        public static void Stop()
        {
            if (m_canPlay)
            {
                m_musicCategory.Stop(StopFlags.Release);
                //m_guiCategory.Stop(SharpDX.XACT3.StopFlags.Immediate);
                foreach (AudioCategory category in m_gameCategories)
                {
                    category.Stop(StopFlags.Immediate);
                }

                if (OnStop != null) OnStop();

                //makes sure that sounds get back to configured volume
                SetAllVolume(MyConfig.GameVolume, MyConfig.MusicVolume);
                MyHudAudio.Reset();
                ClearMusicTransitions();

                m_cuePool.StopAll(StopFlags.Immediate);
            }
        }

        public static void SetAllVolume(float gameVolume, float musicVolume)
        {
            VolumeMusic = musicVolume;
            VolumeGame = gameVolume;
        }

        static List<AudioCategory> GetCategoriesForPause()
        {
            List<AudioCategory> ret = new List<AudioCategory>();
            ret.Add(m_musicCategory);
            //ret.Add(m_guiCategory);
            ret.AddRange(m_gameCategories);
            return ret;
        }

        //  When game pauses
        public static void Pause()
        {
            if (m_canPlay)
            {
                foreach (AudioCategory audioCategory in GetCategoriesForPause())
                {
                    audioCategory.Pause();
                }
            }
            if (OnPause != null) OnPause();
        }

        //  When game resumes from pause
        public static void Resume()
        {
            if (m_canPlay)
            {
                foreach (AudioCategory audioCategory in GetCategoriesForPause())
                {
                    audioCategory.Resume();
                }
            }
            if (OnResume != null) OnResume();
        }

        public static bool ApplyTransition(MyMusicTransitionEnum transitionEnum, int priority = 0, string category = null, bool loop = true)
        {
            if (!m_canPlay) return false;
            if (!m_musicAllowed) return false;

            Debug.Assert(priority >= 0);
            if (category != null)
            {
                if (!m_musicTransitionCues[(int)transitionEnum].ContainsKey(category))
                {
                    Debug.Assert(false, "This category doesn't exist for this transition!");
                    return false;
                }
            }

            // if we try apply same transition and priority and category
            if (m_currentTransition != null &&
                m_currentTransition.Value.Priority == priority &&
                m_currentTransition.Value.TransitionEnum == transitionEnum &&
                (category == null || m_currentTransition.Value.Category == category))
            {
                if (m_musicState == MyMusicState.Transition && !m_transitionForward)
                {
                    m_musicState = MyMusicState.Playing;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            // if category not set, we take random category from this transition cues
            string transitionCategory = category != null ? category : GetRandomTransitionCategory(transitionEnum);
            // we set this transition as next
            m_nextTransitions[priority] = new MyMusicTransition(priority, transitionEnum, transitionCategory);

            // if new transition has lower priority then current, we don't want apply new transition now
            if (m_currentTransition != null && m_currentTransition.Value.Priority > priority)
            {
                return false;
            }

            m_loopMusic = loop;

            if (m_musicState == MyMusicState.Playing)
            {
                StartTransition(true);
            }
            else if (m_musicState == MyMusicState.Transition)
            {
            }
            else if (m_musicState == MyMusicState.Stopped)
            {
            }
            else
            {
                throw new MyMwcExceptionApplicationShouldNotGetHere();
            }

            return true;
        }

        private static void StartTransition(bool forward)
        {
            m_transitionForward = forward;
            m_musicState = MyMusicState.Transition;
            m_timeFromTransitionStart = 0;
        }

        public static void StopTransition(int priority)
        {
            Debug.Assert(priority >= 0);
            // try removes transition with this priority from next transitions queue
            if (m_nextTransitions.ContainsKey(priority))
            {
                m_nextTransitions.Remove(priority);
            }
            // if we actually play current transition with this priorty, we start transition to next (decreasing volume and switch to another)
            if (m_currentTransition != null && priority == m_currentTransition.Value.Priority)
            {
                if (m_musicState != MyMusicState.Transition)
                {
                    StartTransition(false);
                }
            }
        }

        private static MyMusicTransition? GetNextTransition()
        {
            if (m_nextTransitions.Count > 0)
            {
                return m_nextTransitions[m_nextTransitions.Keys[m_nextTransitions.Keys.Count - 1]];
            }
            else
            {
                return null;
            }
        }

        public static void StopMusic()
        {
            if (m_musicCue.HasValue)
            {
                m_musicCue.Value.Stop(StopFlags.Immediate);
            }
            ClearMusicTransitions();
        }

        public static MySoundCue? GetMusicCue()
        {
            return m_musicCue;
        }

        public static MyMusicState GetMusicState()
        {
            return m_musicState;
        }

        public static bool HasAnyTransition()
        {
            return m_nextTransitions.Count > 0;
        }

        private static void UpdateMusic()
        {
            if (m_musicState == MyMusicState.Transition)
            {
                m_timeFromTransitionStart += MyConstants.PHYSICS_STEP_SIZE_IN_MILLISECONDS;
                // if transition time elapsed, we stop actual playing cue and set music state to stopped
                if (m_timeFromTransitionStart >= TRANSITION_TIME)
                {
                    m_musicState = MyMusicState.Stopped;
                    if (m_musicCue != null && m_musicCue.Value.IsPlaying)
                    {
                        m_musicCue.Value.Stop(StopFlags.Immediate);
                        m_musicCue = null;
                    }
                }
                // we decrease music volume (because transition effect)
                else if (m_musicCue != null && m_musicCue.Value.IsPlaying)
                {
                    //temporary disabled, volume decreasing now works strange (terible noise)
                    if (m_volumeMusic > 0f && m_musicOn)
                    {
                        m_musicCategory.SetVolume((1f - (float)m_timeFromTransitionStart / TRANSITION_TIME) * m_volumeMusic);
                    }
                }
            }

            if (m_musicState == MyMusicState.Stopped)
            {
                MyMusicTransition? nextTransition = GetNextTransition();
                // we save current transition as next transition, if we want apply transition with higher priority, so after new transition stop, then this old transition return back
                if (m_currentTransition != null && m_nextTransitions.Count > 0 && nextTransition != null && nextTransition.Value.Priority > m_currentTransition.Value.Priority)
                {
                    m_nextTransitions[m_currentTransition.Value.Priority] = m_currentTransition.Value;
                }

                m_currentTransition = nextTransition;
                // it there is current transition to play, we play it and set state to playing
                if (m_currentTransition != null)
                {
                    PlayMusicByTransition(m_currentTransition.Value);
                    m_nextTransitions.Remove(m_currentTransition.Value.Priority);
                    m_musicState = MyMusicState.Playing;
                }
            }

            if (m_musicState == MyMusicState.Playing)
            {
                // we play current transition in loop
                //TODO: IsStopped makes peaks

                if (m_musicCue != null && (!m_musicCue.Value.IsValid || m_musicCue.Value.IsStopped))
                {
                    if (m_loopMusic)
                    {
                        Debug.Assert(m_currentTransition != null);
                        PlayMusicByTransition(m_currentTransition.Value);
                    }
                    else
                    {
                        m_loopMusic = true; //default
                        StopTransition(m_currentTransition.Value.Priority);
                    }
                }
            }
        }

        private static void PlayMusicByTransition(MyMusicTransition transition)
        {
            // temporary disabled
            if (m_volumeMusic > 0f && m_musicOn)
            {
                m_musicCategory.SetVolume(m_volumeMusic);
            }
            m_musicCue = AddCue2D(m_musicTransitionCues[(int)transition.TransitionEnum][transition.Category]);
        }

        private static string GetRandomTransitionCategory(MyMusicTransitionEnum transitionEnum)
        {
            int randomIndex = MyMwcUtils.GetRandomInt(m_musicTransitionCues[(int)transitionEnum].Count - 1);
            int currentIndex = 0;
            foreach (var categoryCueKVP in m_musicTransitionCues[(int)transitionEnum])
            {
                if (currentIndex == randomIndex)
                {
                    return categoryCueKVP.Key;
                }
                currentIndex++;
            }
            throw new MyMwcExceptionApplicationShouldNotGetHere();
        }

        private static StringBuilder m_currentTransitionDescription = new StringBuilder();

        public static StringBuilder GetCurrentTransitionForDebug()
        {
            m_currentTransitionDescription.Clear();
            m_currentTransitionDescription.Append("Hud cues: ");
            m_currentTransitionDescription.AppendInt32(MyHudAudio.QueueLength);
            //m_currentTransitionDescription.Append("Current music transition: ");
            //if (m_currentTransition != null)
            //{
            //    MySoundCuesEnum currentCue = m_musicTransitionCues[(int)m_currentTransition.Value.TransitionEnum][m_currentTransition.Value.Category];
            //    //Crashes (not all sounds must be in inflsphere descs)
            //   // MyMwcUtils.AppendStringBuilder(m_currentTransitionDescription, MinerWars.AppCode.Game.GUI.Helpers.MyGuiInfluenceSphereHelpers.GetInfluenceSphereSoundHelper(currentCue).Description);                
            //    m_currentTransitionDescription.Append(" priority:");
            //    m_currentTransitionDescription.AppendInt32(m_currentTransition.Value.Priority);
            //}
            //else 
            //{
            //    m_currentTransitionDescription.Append("NO MUSIC");
            //}
            //m_currentTransitionDescription.Append(" state:");
            //switch(m_musicState)
            //{
            //    case MyMusicState.Playing:
            //        m_currentTransitionDescription.Append("Playing");
            //        break;
            //    case MyMusicState.Transition:
            //        m_currentTransitionDescription.Append("Transition");
            //        break;
            //    case MyMusicState.Stopped:
            //        m_currentTransitionDescription.Append("Stopped");
            //        break;
            //}            
            //m_currentTransitionDescription.Append(" in queue:");
            //m_currentTransitionDescription.AppendInt32(m_nextTransitions.Count);
            return m_currentTransitionDescription;
        }

        public static bool MusicAllowed
        {
            get
            {
                return m_musicAllowed;
            }
            set
            {
                m_musicAllowed = value;
            }
        }

#if DEBUG_AUDIO
        private static void DebugDraw()
        {
            float radius = 0.1f;

            foreach (Matrix colMatrix in m_soundCollisions)
            {
                MyDebugDraw.DrawSphereWireframe(colMatrix.Translation, radius, Vector3.One, 1);
                //MyDebugDraw.DrawAxis(matrix, 5.0f, 1.0f);

                MyDebugDraw.DrawLine3D(colMatrix.Translation, colMatrix.Translation + colMatrix.Forward * 4, Color.Red, Color.Red);
            }
        }

#endif
    }
}
