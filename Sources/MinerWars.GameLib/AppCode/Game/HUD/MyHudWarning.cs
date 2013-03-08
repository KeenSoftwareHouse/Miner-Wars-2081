using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.Audio;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Audio.Dialogues;

namespace MinerWars.AppCode.Game.HUD {
    /// <summary>
    /// This class represents sound warning in HUD
    /// </summary>
    class MyHudSoundWarning
    {
        /// <summary>
        /// Sound to play
        /// </summary>
        public MySoundCuesEnum Sound { get; set; }

        /// <summary>
        /// Interval (in ms) when sound will be repeated (0... no repeat)
        /// </summary>
        public int RepeatInterval { get; set; }

        private int m_msSinceLastStateChange;
        private int m_initialDelay;
        private enum WarningState { NOT_STARTED, STARTED, PLAYED };
        private WarningState m_warningState;
        private bool m_updatedForCurrentWarning;
        private MySoundCue? m_soundCue;
        private bool m_playOverDialogues;

        /// <summary>
        /// Creates new instance of MyHudSoundWarning, without repeating
        /// </summary>
        /// <param name="sound">Sound to play</param>
        public MyHudSoundWarning(MySoundCuesEnum sound)
            : this(sound, 0)
        {                        
        }

        /// <summary>
        /// Creates new instance of MyHudSoundWaring, with repeating
        /// </summary>
        /// <param name="sound">Sound to play</param>
        /// <param name="repeatInverval">Repeat sound in interval (in ms) ... for non repeat set to 0</param>
        /// <param name="initialDelay">Waiting time (in ms) before the sound plays when the condition is encountered</param>
        public MyHudSoundWarning(MySoundCuesEnum sound, int repeatInverval, int initialDelay = 0, bool playOverDialogues = true)
        {
            Sound = sound;
            RepeatInterval = repeatInverval;
            m_msSinceLastStateChange = 0;
            m_warningState = WarningState.NOT_STARTED;
            m_initialDelay = initialDelay;
            m_playOverDialogues = playOverDialogues;
        }

        private void PlaySound()
        {
            if (m_playOverDialogues || !MyDialogues.IsPlaying())
                if (m_soundCue == null || !m_soundCue.Value.IsPlaying)
                    m_soundCue = MyAudio.AddCue2D(Sound);
        }

        /// <summary>
        /// Call it in each update.
        /// </summary>
        /// <param name="isWarningDetected">Indicates if warning is detected</param>
        public void Update(bool isWarningDetected)
        {
            if (isWarningDetected)
            {
                switch (m_warningState)
                {
                    case WarningState.NOT_STARTED:
                        m_msSinceLastStateChange = 0;
                        m_warningState = WarningState.STARTED;
                        break;
                    case WarningState.STARTED:
                        m_msSinceLastStateChange += MyConstants.PHYSICS_STEP_SIZE_IN_MILLISECONDS;
                        if (m_msSinceLastStateChange >= m_initialDelay)
                        {
                            PlaySound();
                            m_warningState = WarningState.PLAYED;
                            m_msSinceLastStateChange -= m_initialDelay;
                        }
                        break;
                    case WarningState.PLAYED:
                        if (RepeatInterval > 0)
                        {
                            m_msSinceLastStateChange += MyConstants.PHYSICS_STEP_SIZE_IN_MILLISECONDS;
                            if (m_msSinceLastStateChange >= RepeatInterval)
                            {
                                PlaySound();
                                m_msSinceLastStateChange -= RepeatInterval;
                            }
                        }
                        break;
                }
            }
            else
            {
                if (m_soundCue != null && m_soundCue.Value.IsPlaying)
                {
                    m_soundCue.Value.Stop(SharpDX.XACT3.StopFlags.Release);
                }
                m_warningState = WarningState.NOT_STARTED;
            }
        }
    }

    /// <summary>
    /// This class represents text warning in HUD
    /// </summary>
    class MyHudTextWarning
    {
        /// <summary>
        /// Text which will be displayed when warning detected
        /// </summary>
        public StringBuilder Text { get; set; }

        /// <summary>
        /// Text's font
        /// </summary>
        public MyGuiFont TextFont { get; set; }

        /// <summary>
        /// Creates new instance with default text's color (RED)
        /// </summary>
        /// <param name="text">Warning text</param>
        public MyHudTextWarning(MyTextsWrapperEnum text)
            : this(text, MyHudConstants.ENEMY_FONT)
        {
        }

        /// <summary>
        /// Creates new instance with default text's color (RED)
        /// </summary>
        /// <param name="text">Warning text</param>
        public MyHudTextWarning(StringBuilder text)
            : this(text, MyHudConstants.ENEMY_FONT)
        {
        }

        /// <summary>
        /// Creates new instance with text's font
        /// </summary>
        /// <param name="text">Warning text</param>
        /// <param name="textFont">Text's font</param>
        public MyHudTextWarning(MyTextsWrapperEnum text, MyGuiFont textFont)
            : this(MyTextsWrapper.Get(text), textFont)
        {            
        }

        public MyHudTextWarning(MyTextsWrapperEnum text, params object[] args)
            : this(new StringBuilder(MyTextsWrapper.GetFormatString(text, args)))
        {
        }

        /// <summary>
        /// Creates new instance with text's color
        /// </summary>
        /// <param name="text">Warning text</param>
        /// <param name="textFont">Text's font</param>
        public MyHudTextWarning(StringBuilder text, MyGuiFont textFont)
        {
            Text = text;
            TextFont = textFont;
        }

        /// <summary>
        /// Draws warning text
        /// </summary>
        /// <param name="position">Position to draw</param>
        /// <param name="highlight">Highlight multiplicator of text's color</param>
        /// <returns>Drawed rectangle</returns>
        public MyRectangle2D Draw(Vector2 position, float highlight)
        {
            return MyGuiManager.DrawString(TextFont, Text, position, 1f,
                                    Color.White * highlight, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER);
        }
    }

    /// <summary>
    /// Delegate of warning detection method
    /// </summary>
    /// <returns></returns>
    delegate bool MyWarningDetectionMethod();

    /// <summary>
    /// This class represents HUD warning
    /// </summary>
    class MyHudWarning
    {
        /// <summary>
        /// Warning's priority
        /// </summary>
        public int Priority { get; private set; }

        private MyWarningDetectionMethod m_warningDetectionMethod;
        private MyHudSoundWarning m_soundWarning;
        private MyHudTextWarning m_textWarning;

        private bool m_warningDetected;

        /// <summary>
        /// Creates new instance of HUD warning
        /// </summary>
        /// <param name="detectionMethod">Warning's detection method</param>
        /// <param name="soundWarning">Sound warning</param>
        /// <param name="textWarning">Text warning</param>
        /// <param name="priority">Warning's priority</param>
        public MyHudWarning(MyWarningDetectionMethod detectionMethod, MyHudSoundWarning soundWarning, MyHudTextWarning textWarning, int priority)
        {            
            m_warningDetectionMethod = detectionMethod;
            m_soundWarning = soundWarning;
            m_textWarning = textWarning;
            Priority = priority;
            m_warningDetected = false;            
        }

        /// <summary>
        /// Call it in each update
        /// </summary>
        /// <param name="isWarnedHigherPriority">Indicated if warning with greater priority was signalized</param>
        /// <returns>Returns true if warning detected. Else returns false</returns>
        public bool Update(bool isWarnedHigherPriority)
        {            
            m_warningDetected = false;
            if (!isWarnedHigherPriority)
            {
                m_warningDetected = m_warningDetectionMethod();
            }

            if (m_soundWarning != null)
            {
                m_soundWarning.Update(m_warningDetected);
            }
            
            return m_warningDetected;
        }

        /// <summary>
        /// Draws warning's text if any warning detected
        /// </summary>
        /// <param name="position">Position to draw</param>
        /// <param name="highlight">Highlight multiplicator of text's color</param>
        /// <param name="drawedRectangle">Drawed rectangle</param>
        /// <returns>Returns true if warning's text drawed. Else returns false</returns>
        public bool Draw(Vector2 position, float highlight, out MyRectangle2D? drawedRectangle)
        {
            drawedRectangle = null;
            if (m_warningDetected && m_textWarning != null)
            {
                drawedRectangle = m_textWarning.Draw(position, highlight);
            }
            return drawedRectangle != null;
        }
    }    

    /// <summary>
    /// This class represents HUD warning group. Only 1 warning can be signalized, from this group.
    /// </summary>
    class MyHudWarningGroup 
    {
        private List<MyHudWarning> m_hudWarnings;
        private bool m_canBeTurnedOff;

        /// <summary>
        /// Creates new instance of HUD warning group
        /// </summary>
        /// <param name="hudWarnings"></param>
        public MyHudWarningGroup(List<MyHudWarning> hudWarnings, bool canBeTurnedOff)
        {            
            m_hudWarnings = new List<MyHudWarning>(hudWarnings);
            SortByPriority();
            m_canBeTurnedOff = canBeTurnedOff;
        }

        /// <summary>
        /// Call it in each update.
        /// </summary>
        public void Update()
        {
            if (m_canBeTurnedOff && MyConfig.Notifications == false)
                return;
            
            bool isWarnedHigherPriority = false;
            foreach (MyHudWarning hudWarning in m_hudWarnings)
            {
                if (hudWarning.Update(isWarnedHigherPriority))
                {
                    isWarnedHigherPriority = true;
                }
            }
        }

        /// <summary>
        /// Draws warning's text if any warning detected
        /// </summary>
        /// <param name="position">Position to draw</param>
        /// <param name="highlight">Highlight multiplicator of text's color</param>
        /// <param name="drawedRectangle">Drawed rectangle</param>
        /// <returns>Returns true if warning's text drawed. Else returns false</returns>/// <returns></returns>
        public bool Draw(Vector2 position, float highlight, out MyRectangle2D? drawedRectangle)
        {
            drawedRectangle = null;

            if (m_canBeTurnedOff && MyConfig.Notifications == false)
                return false;

            foreach (MyHudWarning hudWarning in m_hudWarnings)
            {
                if (hudWarning.Draw(position, highlight, out drawedRectangle))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Adds new HUD warning to this group
        /// </summary>
        /// <param name="hudWarning">HUD warning to add</param>
        public void Add(MyHudWarning hudWarning)
        {
            m_hudWarnings.Add(hudWarning);
            SortByPriority();
        }

        /// <summary>
        /// Removes HUD warning from this group
        /// </summary>
        /// <param name="hudWarning">HUD warning to remove</param>
        public void Remove(MyHudWarning hudWarning)
        {
            m_hudWarnings.Remove(hudWarning);            
        }

        /// <summary>
        /// Removes all HUD warnings from this group
        /// </summary>        
        public void Clear()
        {
            m_hudWarnings.Clear();
        }

        private void SortByPriority()
        {
            m_hudWarnings.Sort((x, y) => x.Priority.CompareTo(y.Priority));
        }
    }    

    /// <summary>
    /// This class represents HUD warnings for entities
    /// </summary>
    static class MyHudWarnings 
    {
        private const float m_highlightMin = 0.6f;
        private const float m_highlightMax = 1.0f;
        private const float m_highlightStep = 0.02f;
        private static readonly Vector2 m_drawPosition = new Vector2(0.5f, 0.35f);

        private static Dictionary<WeakReference, List<MyHudWarningGroup>> m_hudWarnings = new Dictionary<WeakReference, List<MyHudWarningGroup>>();        
        private static bool m_highlightIncreasing = false;
        private static float m_actualHighlight = m_highlightMax;

        /// <summary>
        /// Register new HUD warning group for entity
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="hudWarningGroup">HUD warning group</param>
        public static void Add(object entity, MyHudWarningGroup hudWarningGroup)
        {
            foreach (KeyValuePair<WeakReference, List<MyHudWarningGroup>> keyValuePair in m_hudWarnings)
            {
                if (keyValuePair.Key.Target == entity)
                {
                    keyValuePair.Value.Add(hudWarningGroup);
                    return;
                }
            }
            WeakReference weakReference = new WeakReference(entity);
            m_hudWarnings[weakReference] = new List<MyHudWarningGroup>();
            m_hudWarnings[weakReference].Add(hudWarningGroup);            
        }

        /// <summary>
        /// Unregister HUD warning group for entity
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="hudWarningGroup">HUD warning group</param>
        public static void Remove(object entity, MyHudWarningGroup hudWarningGroup)
        {
            foreach (KeyValuePair<WeakReference, List<MyHudWarningGroup>> keyValuePair in m_hudWarnings)
            {
                if (keyValuePair.Key.Target == entity)
                {
                    keyValuePair.Value.Remove(hudWarningGroup);
                }
            }
        }

        /// <summary>
        /// Unregister all HUD warning groups for entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public static void Remove(object entity)
        {
            WeakReference weakReferenceToRemove = null;
            foreach (KeyValuePair<WeakReference, List<MyHudWarningGroup>> keyValuePair in m_hudWarnings)
            {
                if (keyValuePair.Key.Target == entity)
                {
                    weakReferenceToRemove = keyValuePair.Key;
                    break;
                }
            }
            if (weakReferenceToRemove != null)
            {
                m_hudWarnings.Remove(weakReferenceToRemove);
            }
        }

        /// <summary>
        /// Unregister all HUD warning groups for all entities
        /// </summary>
        public static void UnloadData()
        {
            m_hudWarnings.Clear();
        }

        /// <summary>
        /// Call it in each update
        /// </summary>
        public static void Update()
        {            
            UpdateHighlight();

            foreach (KeyValuePair<WeakReference, List<MyHudWarningGroup>> keyValuePair in m_hudWarnings)
            {
                MyEntity entity = keyValuePair.Key.Target as MyEntity;
                if (entity != null && entity.Activated)
                {
                    foreach (MyHudWarningGroup hudWarningGroup in keyValuePair.Value)
                    {
                        hudWarningGroup.Update();
                    }
                }
            }
        }

        private static void UpdateHighlight()
        {
            if (m_highlightIncreasing)
            {
                m_actualHighlight += m_highlightStep;
            }
            else
            {
                m_actualHighlight -= m_highlightStep;
            }

            if (m_actualHighlight > m_highlightMax)
            {
                m_highlightIncreasing = false;
            }
            else if (m_actualHighlight < m_highlightMin)
            {
                m_highlightIncreasing = true;
            }
        }

        /// <summary>
        /// Draws HUD warning's texts
        /// </summary>
        public static void Draw()
        {
            Vector2 drawPosition = m_drawPosition;
            MyRectangle2D? drawedRectangle = null;
            foreach (KeyValuePair<WeakReference, List<MyHudWarningGroup>> keyValuePair in m_hudWarnings)
            {
                MyEntity entity = keyValuePair.Key.Target as MyEntity;
                if (entity != null && entity.Activated)
                {
                    foreach (MyHudWarningGroup hudWarningGroup in keyValuePair.Value)
                    {
                        if (hudWarningGroup.Draw(drawPosition, m_actualHighlight, out drawedRectangle))
                        {
                            drawPosition.Y += drawedRectangle.Value.Size.Y;
                        }
                    }
                }
            }            
        }
    }
}
