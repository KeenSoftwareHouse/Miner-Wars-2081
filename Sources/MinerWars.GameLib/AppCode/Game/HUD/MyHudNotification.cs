using System;
using System.Text;
using System.Collections.Generic;
using MinerWarsMath;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.App;
using KeenSoftwareHouse.Library.Extensions;
using System.Diagnostics;
using MinerWars.CommonLIB.AppCode.Generics;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.VideoMode;

namespace MinerWars.AppCode.Game.HUD
{
    internal static class MyHudNotification
    {
        // notification structure:
        public class MyNotification
        {
            private MyNotification(int notificationTextId, string notificationString, MyGuiScreenGamePlayType LifeArea, float scale,
                                  MyGuiFont font,
                                  MyGuiDrawAlignEnum textAlign, int disapearTimeMs, MyEntity owner, bool showConfirmMessage = false,
                                  object[] textFormatArguments = null)
            {
                Owner = owner;
                m_originalText = notificationString;
                m_notificationText = notificationString;
                m_notificationTextID = (int)notificationTextId;
                m_isTextDirty = false;

                // always false:
                m_isDisappeared = false;

                m_actualScale = scale;                
                m_actualFont = font;
                m_actualTextAlign = textAlign;

                m_textFormatArguments = textFormatArguments;

                // timing:
                m_disappearTimeMs = disapearTimeMs;
                m_aliveTime = 0;

                // life space;
                m_lifeSpace = LifeArea;

                // show standart message?
                m_showConfirmMessage = showConfirmMessage;
            }

            // construction from: precopyed MyTextWrapperEnum in normalized position with scale and textColor, with textAlign
            // and asociated user function or delegate returns approvement of the event with added textFormatArguments for MyTextWrapperEnum
            // and also settings for the time disapeartin of the notification
            public MyNotification(MyTextsWrapperEnum notificationText, MyGuiScreenGamePlayType LifeArea, float scale,
                                  MyGuiFont font,
                                  MyGuiDrawAlignEnum textAlign, int disapearTimeMs, MyEntity owner = null, bool showConfirmMessage = false,
                                  object[] textFormatArguments = null)
                : this((int)notificationText, null, LifeArea, scale, font, textAlign, disapearTimeMs, owner, showConfirmMessage, textFormatArguments)
            {                
            }

            public MyNotification(String notificationText, MyGuiScreenGamePlayType LifeArea, float scale,
                                  MyGuiFont font,
                                  MyGuiDrawAlignEnum textAlign, int disapearTimeMs, MyEntity owner = null, bool showConfirmMessage = false,
                                  object[] textFormatArguments = null)
                : this(0, notificationText, LifeArea, scale, font, textAlign, disapearTimeMs, owner, showConfirmMessage, textFormatArguments)
            {
            }

            public MyNotification(string notificationText, int disapearTimeMs, MyEntity owner = null,
                                  object[] textFormatArguments = null)
                : this(notificationText, GetCurrentScreen(), 1.0f, MyGuiConstants.DEFAULT_CONTROL_FONT,
                       MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, disapearTimeMs, owner, false,
                       textFormatArguments)
            {
                
            }

            public MyNotification(MyTextsWrapperEnum notificationText, int disapearTimeMs, MyEntity owner = null,
                                  object[] textFormatArguments = null)
                : this(notificationText, GetCurrentScreen(), 1.0f, MyGuiConstants.DEFAULT_CONTROL_FONT,
                       MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, disapearTimeMs, owner, false,
                       textFormatArguments)
            {
            }

            public MyNotification(MyTextsWrapperEnum notificationText, MyEntity owner = null, bool showConfirmMessage = false,
                                  object[] textFormatArguments = null)
                : this(notificationText, DONT_DISAPEAR, owner, textFormatArguments)
            {
                m_showConfirmMessage = showConfirmMessage;
            }

            public MyNotification(MyTextsWrapperEnum notificationText, MyGuiFont font, MyEntity owner = null)
                : this(notificationText, GetCurrentScreen(), 1.0f, font, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, DONT_DISAPEAR, owner)
            {
            }

            public MyNotification(MyTextsWrapperEnum notificationText, MyGuiFont font, int disapearTimeMs, MyEntity owner = null, object[] textFormatArguments = null)
                : this(notificationText, GetCurrentScreen(), 1.0f, font, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, disapearTimeMs, owner, false, textFormatArguments)
            {
            }

            public MyNotification(string notificationText, MyGuiFont font, MyEntity owner = null, bool showConfirmMessage = false)
                : this(notificationText, GetCurrentScreen(), 1.0f, font, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, DONT_DISAPEAR, owner)
            {
                m_showConfirmMessage = showConfirmMessage;
            }

            public MyNotification(string notificationText, MyGuiFont font, int disapearTimeMs, MyEntity owner = null, object[] textFormatArguments = null)
                : this(notificationText, GetCurrentScreen(), 1.0f, font, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, disapearTimeMs, owner, false, textFormatArguments)
            {
            }

            // Wait with notification until notification is proved:
            public bool m_isDisappeared;
            // Actual settings:
            private int m_notificationTextID;
            private object[] m_textFormatArguments;
            private float m_actualScale;
            private MyGuiFont m_actualFont;
            private MyGuiDrawAlignEnum m_actualTextAlign;
            private int m_aliveTime;
            private int m_disappearTimeMs;
            private MyGuiScreenGamePlayType m_lifeSpace;
            private bool m_showConfirmMessage;
            private string m_notificationText;
            private string m_originalText;
            private bool m_isTextDirty;

            public event Action Confirmed;
            public MyEntity Owner { get; private set; }

            /// <summary>
            /// Important notifications are added to hud even when it's full (other notifications are removed)
            /// </summary>
            public bool IsImportant { get; set; }

            // Test if event delegate is present, if is present than call it,
            // and also test if this notification doesnt have to disapear after the setting of the disapearing time
            public bool IsDisappeared()
            {
                if (m_isDisappeared)
                    return true;

                bool isDisappeared = false;
                if (m_showConfirmMessage)
                {
                    isDisappeared = CheckStandardDisapearKey();
                    if (isDisappeared)
                        if (Confirmed != null)
                            Confirmed();
                }
                else
                {
                    isDisappeared = CheckTimeDisapear();
                }

                return isDisappeared;
            }

            // Test current time with disapear time:
            public bool CheckTimeDisapear()
            {
                return m_disappearTimeMs == DONT_DISAPEAR ? false : (m_aliveTime >= m_disappearTimeMs);
            }

            public bool CheckStandardDisapearKey()
            {
                if (MyGuiManager.GetInput().IsGameControlPressed(MyGameControlEnums.NOTIFICATION_CONFIRMATION))
                {
                    return true;
                }
                return false;
            }

            public void SetTextDirty()
            {
                m_isTextDirty = true;
            }

            public MyGuiFont GetFont() 
            {
                return m_actualFont;
            }

            public MyTextsWrapperEnum GetTextEnum()
            {
                return (MyTextsWrapperEnum) m_notificationTextID;
            }

            public string GetText() 
            {
                if (string.IsNullOrEmpty(m_notificationText) || m_isTextDirty) 
                {
                    if (m_textFormatArguments != null && m_textFormatArguments.Length > 0)
                    {
                        if (m_notificationTextID == 0)
                            m_notificationText = String.Format(m_originalText, m_textFormatArguments);
                        else
                            m_notificationText = MyTextsWrapper.GetFormatString(GetTextEnum(), m_textFormatArguments);
                    }
                    else
                    {
                        if (m_notificationTextID == 0)
                            m_notificationText = m_originalText;
                        else
                            m_notificationText = MyTextsWrapper.GetFormatString(GetTextEnum());
                    }

                    m_isTextDirty = false;
                }
                return m_notificationText;
            }

            public float GetScale()
            {
                return m_actualScale;
            }

            public object[] GetTextFormatArguments()
            {
                return m_textFormatArguments;
            }

            public void SetTextFormatArguments(object[] arguments)
            {
                m_textFormatArguments = arguments;
                m_notificationText = null;
                GetText();
            }

            public void AddAliveTime(int timeStep)
            {
                m_aliveTime += timeStep;
            }

            public void ResetAliveTime()
            {
                m_aliveTime = 0;
                m_isDisappeared = false;
            }

            public MyGuiScreenGamePlayType GetNotificationScreenSpace()
            {
                return m_lifeSpace;
            }

            public bool HasDefaultDisappearMessage()
            {
                return m_showConfirmMessage;
            }

            public event Action Disappeared;
            public void Disappear()
            {
                m_isDisappeared = true;
                if (Disappeared != null)
                    Disappeared();
            }

            public void Appear()
            {
                m_isDisappeared = false;
            }
        }


        // Constants for notification disapear:
        public const int DONT_DISAPEAR = 0;

        private static string m_defaultNotificationDisapearMessage = MyTextsWrapper.GetFormatString(MyTextsWrapperEnum.DefaultNotificationDisapearMessage);
        private static MyObjectsPool<StringBuilder> m_textsPool = new MyObjectsPool<StringBuilder>(MyNotificationConstants.MAX_DISPLAYED_NOTIFICATIONS_COUNT);
        private static MyScreenNotifications[] m_screenNotifications;
        private static MyGuiFont m_usedFont;
        private static MyGuiScreenGamePlayType m_currentScreen;

        private static Color m_fogColor;

        // Set current space - screen:
        public static void SetCurrentScreen(MyGuiScreenGamePlayType currentScreen = MyGuiScreenGamePlayType.GAME_STORY)
        {
            m_currentScreen = currentScreen;
        }

        public static MyGuiScreenGamePlayType GetCurrentScreen()
        {
            return m_currentScreen;
        }

        // initialize:
        public static void LoadContent()
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyHudNotification::LoadContent");

            m_usedFont = MyGuiManager.GetFontMinerWarsWhite();
            if (m_screenNotifications == null)
            {
                int allScreensCount = (int) MyGuiScreenGamePlayType.ALL_SCREEN_COUNT;
                m_screenNotifications = new MyScreenNotifications[allScreensCount];
                for (int i = 0; i < allScreensCount; ++i)
                {
                    m_screenNotifications[i] = new MyScreenNotifications();
                }
            }
            m_fogColor = MyGuiConstants.SELECT_AMMO_TIP_BACKGROUND_FADE_COLOR;
            m_fogColor.A = (byte) (m_fogColor.A * 0.85f);

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        // Add notification to the end of the list:
        public static int AddNotification(MyNotification notification)
        {
            return AddNotification(notification, GetCurrentScreen());
        }

        // Add notification to the end of the list:
        public static int AddNotification(MyNotification notification, MyGuiScreenGamePlayType currentScreen)
        {
            int currentScreenInt = (int)currentScreen;
            notification.ResetAliveTime();
            m_screenNotifications[currentScreenInt].Add(notification);
            return m_screenNotifications[currentScreenInt].Count();
        }

        // Has any notifications on the current screen?
        public static bool HasNotification()
        {
            return (m_screenNotifications[(int) GetCurrentScreen()].Count()) != 0;
        }

        // has notifications for the current screen:
        public static bool HasNotification(MyGuiScreenGamePlayType forScreen)
        {
            return m_screenNotifications[(int) forScreen].Count() > 0;
        }

        // Flush all notifications for the game play type screen:
        public static void ClearNotifications(MyGuiScreenGamePlayType forScreen)
        {
            m_screenNotifications[(int) forScreen].Clear();
        }

        // Removes all notification for owner
        public static void ClearOwnerNotifications(MyEntity owner)
        {
            foreach (var notification in m_screenNotifications)
            {
                notification.RemoveByOwner(owner);
            }
        }

        public static void ReloadTexts()
        {
            if (m_screenNotifications != null)
            {
                foreach (var notification in m_screenNotifications)
                {
                    notification.SetTextDirty();
                }
            }
        }


        // Flush all notifications:
        public static void ClearAllNotifications()
        {
            if (m_screenNotifications != null)
            {
                foreach (MyScreenNotifications notifications in m_screenNotifications)
                {
                    notifications.Clear();
                }
            }
        }

        // Draw current notification - based on current screen:
        public static void Draw()
        {
            m_screenNotifications[(int) GetCurrentScreen()].Draw();
        }

        // Update the current notification test - based on current screen:
        public static void Update()
        {
            m_screenNotifications[(int) GetCurrentScreen()].Update();
        }

        public static bool HandleInput(MyGuiInput input) 
        {
            return m_screenNotifications[(int)GetCurrentScreen()].HandleInput(input);
        }

        public static void SetNotificationsPosition(MyGuiScreenGamePlayType screen, Vector2 position)
        {
            m_screenNotifications[(int) screen].Position = position;
        }

        private class MyScreenNotifications
        {
            private List<MyNotification> m_notifications;                        

            public Vector2 Position { get; set; }

            public MyScreenNotifications()
            {
                m_notifications = new List<MyNotification>();
                Position = MyNotificationConstants.DEFAULT_NOTIFICATION_MESSAGE_NORMALIZED_POSITION;
            }

            List<Vector2> m_textSizes = new List<Vector2>(MyNotificationConstants.MAX_DISPLAYED_NOTIFICATIONS_COUNT);
            List<StringBuilder> m_texts = new List<StringBuilder>(MyNotificationConstants.MAX_DISPLAYED_NOTIFICATIONS_COUNT);

            private void ClearTexts() 
            {
                m_textSizes.Clear();
                foreach (StringBuilder text in m_texts)
                {
                    text.Clear();                                        
                }
                m_textsPool.DeallocateAll();
                m_texts.Clear();
            }

            public void Draw()
            {
                ClearTexts();
                int visibleCount = Math.Min(m_notifications.Count, MyNotificationConstants.MAX_DISPLAYED_NOTIFICATIONS_COUNT);

                for (int i = 0; i < visibleCount; i++)
                {
                    MyNotification actualNotification = m_notifications[i];                    
                    StringBuilder messageStringBuilder = m_textsPool.Allocate();
                    Debug.Assert(actualNotification != null);
                    Debug.Assert(messageStringBuilder != null);

                    bool hasConfirmation = actualNotification.HasDefaultDisappearMessage();
                    
                    messageStringBuilder.Append(actualNotification.GetText());                    
                    
                    if (hasConfirmation)
                    {                        
                        messageStringBuilder.AppendLine();
                        messageStringBuilder.ConcatFormat(m_defaultNotificationDisapearMessage, MyGuiManager.GetInput().GetGameControlTextEnum(MyGameControlEnums.NOTIFICATION_CONFIRMATION));                            
                    }

                    // draw background:
                    Vector2 textSize = MyGuiManager.GetNormalizedSize(m_usedFont, messageStringBuilder, actualNotification.GetScale());
                    
                    m_textSizes.Add(textSize);
                    m_texts.Add(messageStringBuilder);
                }

                MyGuiManager.BeginSpriteBatch();
                var offset = new Vector2(VideoMode.MyVideoModeManager.IsTripleHead() ? -1 : 0, 0);

                
                // Draw fog
                Vector2 notificationPosition = Position;

                for (int i = 0; i < visibleCount; i++)
                {
                    Vector2 fogFadeSize = m_textSizes[i] * new Vector2(1.6f, 8.0f);

                        MyGuiManager.DrawSpriteBatch(MyGuiManager.GetFogSmallTexture(), notificationPosition + offset, fogFadeSize,
                                m_fogColor,
                                MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, MyVideoModeManager.IsTripleHead());
                    
                    notificationPosition.Y += m_textSizes[i].Y;
                }

                // Draw texts
                notificationPosition = Position;
                for (int i = 0; i < visibleCount; i++)
                {
                    MyNotification actualNotification = m_notifications[i];

                    MyGuiManager.DrawString(actualNotification.GetFont(), m_texts[i], notificationPosition + offset,
                                            actualNotification.GetScale(), Color.White,
                                            MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, MyVideoModeManager.IsTripleHead());
                    notificationPosition.Y += m_textSizes[i].Y;
                }
                MyGuiManager.EndSpriteBatch();
            }

            public void Update()
            {
                for (int i = 0; i < Math.Min(m_notifications.Count, MyNotificationConstants.MAX_DISPLAYED_NOTIFICATIONS_COUNT); i++)
                {
                    m_notifications[i].AddAliveTime(MyConstants.PHYSICS_STEP_SIZE_IN_MILLISECONDS);
                }

                for (int i = m_notifications.Count-1; i >= 0; i--)
                {
                    var notification = m_notifications[i];
                    if (notification.IsDisappeared())
                    {
                        m_notifications.Remove(notification);
                    }
                }
            }

            public bool HandleInput(MyGuiInput input) 
            {
                bool result = false;
                foreach (MyHudNotification.MyNotification notification in m_notifications) 
                {
                    if(notification.HasDefaultDisappearMessage() && input.IsGameControlPressed(MyGameControlEnums.USE))
                    {
                        notification.Disappear();
                        result = true;
                    }
                }
                return result;
            }

            public void Add(MyNotification notification)
            {
                Debug.Assert(notification != null, "you cannot add null notification");
                if (!m_notifications.Contains(notification))
                {
                    if (m_notifications.Count >= MyNotificationConstants.MAX_DISPLAYED_NOTIFICATIONS_COUNT)
                    {
                        if (!notification.IsImportant)
                            return;

                        int index = m_notifications.FindIndex(IsNotImportant);
                        if (index != -1)
                        {
                            m_notifications.RemoveAt(index);
                        }
                    }

                    m_notifications.Add(notification);
                }
            }

            bool IsNotImportant(MyNotification notification)
            {
                return !notification.IsImportant;
            }

            public void Remove(MyNotification notification)
            {
                m_notifications.Remove(notification);
            }

            public void RemoveByOwner(MyEntity owner)
            {
                for (int i = 0; i < m_notifications.Count; )
                {
                    if (m_notifications[i].Owner == owner)
                    {
                        m_notifications.RemoveAt(i);
                    }
                    else
                    {
                        i++;
                    }
                }
            }

            public void Clear()
            {
                m_notifications.Clear();
            }

            public void SetTextDirty()
            {
                foreach (var notification in m_notifications)
                {
                    notification.SetTextDirty();
                }
            }

            public int Count()
            {
                return m_notifications.Count;
            }
        }
    }
}
