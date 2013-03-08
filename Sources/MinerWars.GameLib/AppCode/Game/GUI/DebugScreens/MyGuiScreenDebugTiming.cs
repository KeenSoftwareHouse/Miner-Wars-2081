using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Managers;
using KeenSoftwareHouse.Library.Extensions;
using SysUtils;


namespace MinerWars.AppCode.Game.GUI.DebugScreens
{
    class MyGuiScreenDebugTiming: MyGuiScreenDebugBase
    {
        static StringBuilder m_debugText = new StringBuilder(1000);
        
        public MyGuiScreenDebugTiming()
            : base(new Vector2(0.5f, 0.5f), new Vector2(), null, true)
        {
            m_isTopMostScreen = true;
            m_drawEvenWithoutFocus = true;
            m_canHaveFocus = false;
        }

        public override string GetFriendlyName()
        {
            return "DebugTimingScreen";
        }

        public Vector2 GetScreenLeftTopPosition()
        {
            float deltaPixels = 25 * MyGuiManager.GetSafeScreenScale();
            Rectangle fullscreenRectangle = MyGuiManager.GetSafeFullscreenRectangle();
            return MyGuiManager.GetNormalizedCoordinateFromScreenCoordinate_FULLSCREEN(new Vector2(deltaPixels, deltaPixels));
        }

        public void SetTexts()
        {
            MyMwcUtils.ClearStringBuilder(m_debugText);

            m_debugText.Append("FPS: ");
            m_debugText.AppendInt32(MyFpsManager.GetFps());
            m_debugText.AppendLine();

            m_debugText.Append("Frame time: ");
            m_debugText.AppendDecimal(MyFpsManager.FrameTime, 1);
            m_debugText.Append(" ms");
            m_debugText.AppendLine();

            m_debugText.Append("Frame avg time: ");
            m_debugText.AppendDecimal(MyFpsManager.FrameTimeAvg, 1);
            m_debugText.Append(" ms");
            m_debugText.AppendLine();

            m_debugText.Append("Frame min time: ");
            m_debugText.AppendDecimal(MyFpsManager.FrameTimeMin, 1);
            m_debugText.Append(" ms");
            m_debugText.AppendLine();

            m_debugText.Append("Frame max time: ");
            m_debugText.AppendDecimal(MyFpsManager.FrameTimeMax, 1);
            m_debugText.Append(" ms");
            m_debugText.AppendLine();

            m_debugText.Append("Environment map update time: ");
            m_debugText.AppendDecimal(MinerWars.AppCode.Game.Render.EnvironmentMap.MyEnvironmentMap.LastUpdateTime, 1);
            m_debugText.Append(" ms");
            m_debugText.AppendLine();

            if (MyMwcFinalBuildConstants.IS_DEVELOP)
            {
                m_debugText.AppendLine();
                MyPerformanceCounter.PerCameraDraw.AppendCustomCounters(m_debugText);
                m_debugText.AppendLine();
                MyPerformanceCounter.PerCameraDraw.AppendCustomTimers(m_debugText);
            }
        }

        public override bool Draw(float backgroundFadeAlpha)
        {
            if (base.Draw(backgroundFadeAlpha) == false) return false;

            SetTexts();
            float textScale = MyGuiConstants.DEBUG_STATISTICS_TEXT_SCALE;

            Vector2 origin = GetScreenLeftTopPosition();

            MyGuiManager.DrawString(MyGuiManager.GetFontMinerWarsWhite(), m_debugText, origin, textScale,
                    Color.Yellow, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP);

            return true;
        }
    }
}
