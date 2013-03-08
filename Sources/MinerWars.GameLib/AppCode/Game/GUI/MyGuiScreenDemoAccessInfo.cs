using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.World;

namespace MinerWars.AppCode.Game.GUI
{
    class MyGuiScreenDemoAccessInfo : MyGuiScreenBase
    {
        private const int m_blinkingIntervalVisible = 1000;             // in ms
        private const int m_blinkingIntervalInvisible = 300;             // in ms

        private int m_drawingBlinkingTextLastTime;
        private bool m_drawingBlinkingTextStatus;

        private Vector2 m_normalizedCoord = new Vector2(0.5f, 0.27f);

        private StringBuilder m_sbInfo;
        private StringBuilder m_sbPressToHide;
        private const string m_keysToCloseInfo = "Alt + G";

        public MyGuiScreenDemoAccessInfo()
            : base(new Vector2(1.0f, 0.0f), null, null, true, null)
        {
            m_isTopMostScreen = true;
            m_drawEvenWithoutFocus = true;
            m_canHaveFocus = false;
            m_screenCanHide = false;
            m_canCloseInCloseAllScreenCalls = false;

            m_sbInfo = MyTextsWrapper.Get(MyTextsWrapperEnum.MessageYouHaveDemoAccess);
            m_sbPressToHide = new StringBuilder();
            m_sbPressToHide.AppendFormat(MyTextsWrapper.GetFormatString(MyTextsWrapperEnum.MessagePressToHide), m_keysToCloseInfo);
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenDemoAccessInfo";
        }

        public override bool Draw(float backgroundFadeAlpha)
        {
            int deltaTime = MyMinerGame.TotalTimeInMilliseconds - m_drawingBlinkingTextLastTime;

            if (((m_drawingBlinkingTextStatus == true) && (deltaTime > m_blinkingIntervalVisible)) ||
                ((m_drawingBlinkingTextStatus == false) && (deltaTime > m_blinkingIntervalInvisible)))
            {
                m_drawingBlinkingTextStatus = !m_drawingBlinkingTextStatus;
                m_drawingBlinkingTextLastTime = MyMinerGame.TotalTimeInMilliseconds;
            }

            if (m_drawingBlinkingTextStatus)
            {
                MyRectangle2D rec = MyGuiManager.DrawString(MyGuiManager.GetFontMinerWarsRed(), m_sbInfo,
                                                            m_normalizedCoord,
                                                            1f, Color.White,
                                                            MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER);
                MyGuiManager.DrawString(MyGuiManager.GetFontMinerWarsRed(), m_sbPressToHide,
                                        new Vector2(m_normalizedCoord.X, m_normalizedCoord.Y + rec.Size.Y),
                                        0.75f, Color.White, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER);
            }            
            return true;
        }
    }
}
