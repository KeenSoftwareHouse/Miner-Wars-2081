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
using MinerWars.AppCode.Game.Managers.Session;


namespace MinerWars.AppCode.Game.GUI.DebugScreens
{
    class MyGuiScreenDebugPlayerFriends: MyGuiScreenDebugBase
    {
        static StringBuilder m_debugText = new StringBuilder(5000);

        public MyGuiScreenDebugPlayerFriends()
            : base(new Vector2(0.5f, 0.5f), new Vector2(), null, true)
        {
            m_isTopMostScreen = true;
            m_drawEvenWithoutFocus = true;
            m_canHaveFocus = false;
        }

        public override string GetFriendlyName()
        {
            return "DebugScreenPlayerFriends";
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

            m_debugText.AppendLine("Player friends debug info");

            if (MySession.Static == null || MySession.PlayerFriends == null)
            {
                m_debugText.AppendLine("Player friends does not exists");
                return;
            }

            foreach (var friend in MySession.PlayerFriends.GetDebug())
            {
                m_debugText.Append(friend.DisplayName);
                m_debugText.Append(": ");
                if(friend.EntityId.HasValue)
                {
                    m_debugText.AppendInt32((int)friend.EntityId.Value.NumericValue);
                    m_debugText.Append(" (");
                    m_debugText.AppendInt32(friend.EntityId.Value.PlayerId);
                    m_debugText.Append(")");
                    m_debugText.AppendLine();
                    m_debugText.Append("    Position: ");
                    m_debugText.AppendDecimal(friend.GetPosition().X, 1);
                    m_debugText.Append("; ");
                    m_debugText.AppendDecimal(friend.GetPosition().Y, 1);
                    m_debugText.Append("; ");
                    m_debugText.AppendDecimal(friend.GetPosition().Z, 1);
                    m_debugText.AppendLine();
                }
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
