using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.App;

namespace MinerWars.AppCode.Game.GUI.Core
{
    class MyToolTips
    {
        private List<MyColoredText> m_toolTips;
        private Vector2 m_size;

        /// <summary>
        /// Creates new instance with empty tooltips
        /// </summary>
        public MyToolTips()
        {
            m_toolTips = new List<MyColoredText>();
        }

        /// <summary>
        /// Creates new instance with one default tooltip
        /// </summary>
        /// <param name="toolTip">Tooltip's text</param>
        public MyToolTips(StringBuilder toolTip)
            : this()
        {
            AddToolTip(toolTip);
        }

        /// <summary>
        /// Removes all tooltips
        /// </summary>
        public void ClearToolTips()
        {
            m_toolTips.Clear();
            RecalculateSize();
        }

        /// <summary>
        /// Removes a range of tooltips
        /// </summary>
        /// <param name="index">From index</param>
        /// <param name="count">Count of removing tooltips</param>
        public void RemoveRange(int index, int count)
        {
            m_toolTips.RemoveRange(index, count);
            RecalculateSize();
        }

        /// <summary>
        /// Adds new default tooltip
        /// </summary>
        /// <param name="toolTip">Tooltip's text</param>
        public void AddToolTip(StringBuilder toolTip)
        {
            AddToolTip(toolTip, MyGuiConstants.TOOL_TIP_TEXT_COLOR);
        }

        /// <summary>
        /// Adds new tooltip with specific color
        /// </summary>
        /// <param name="toolTip">Tooltip's text</param>
        /// <param name="color">Tooltip's color</param>
        public void AddToolTip(StringBuilder toolTip, Color color)
        {
            AddToolTip(toolTip, color, MyGuiConstants.TOOL_TIP_TEXT_SCALE);            
        }

        /// <summary>
        /// Adds new tooltip with specific color and scale
        /// </summary>
        /// <param name="toolTip">Tooltip's text</param>
        /// <param name="color">Tooltip's color</param>
        /// <param name="textScale">Tooltip's scale</param>
        public void AddToolTip(StringBuilder toolTip, Color color, float textScale)
        {
            if (toolTip != null)
            {
                m_toolTips.Add(new MyColoredText(toolTip, color, MyGuiManager.GetFontMinerWarsBlue(), textScale));
                RecalculateSize();
            }
        }

        /// <summary>
        /// Returns all tooltips
        /// </summary>
        /// <returns></returns>
        public List<MyColoredText> GetToolTips()
        {
            return m_toolTips;
        }

        /// <summary>
        /// Return size of tooltips
        /// </summary>
        /// <returns></returns>
        public Vector2 GetSize()
        {
            return m_size;
        }

        /// <summary>
        /// Recalculates size of tooltips
        /// </summary>
        public void RecalculateSize()
        {
            m_size = new Vector2(0f, 0f);
            bool isEmptyToolTip = true;
            for (int i = 0; i < m_toolTips.Count; i++)
            {
                if (m_toolTips[i].Text.Length > 0) isEmptyToolTip = false;
                Vector2 actualToolTipSize = MyGuiManager.GetNormalizedSize(MyGuiManager.GetFontMinerWarsBlue(), m_toolTips[i].Text, m_toolTips[i].Scale);
                m_size.X = Math.Max(m_size.X, actualToolTipSize.X);
                m_size.Y += actualToolTipSize.Y;
            }
            
            if (isEmptyToolTip)m_size = new Vector2(-1f,-1f);
        }

        public void Draw(Vector2 normalizedPosition)
        {
            if (GetSize().X > -1f)
            {
                Vector2 innerBorder = new Vector2(0.005f, 0.002f);
                Vector2 bgSize = GetSize() + 2 * innerBorder;
                Vector2 bgPosition = normalizedPosition - new Vector2(innerBorder.X, bgSize.Y / 2);

                var screenRectangle = MyGuiManager.FullscreenHudEnabled ? MyGuiManager.GetFullscreenRectangle() : MyGuiManager.GetSafeFullscreenRectangle();
                var topleft = MyGuiManager.GetNormalizedCoordinateFromScreenCoordinate(new Vector2(screenRectangle.Left, screenRectangle.Top)) + new Vector2(MyGuiConstants.TOOLTIP_DISTANCE_FROM_BORDER);
                var rightbottom = MyGuiManager.GetNormalizedCoordinateFromScreenCoordinate(new Vector2(screenRectangle.Right, screenRectangle.Bottom)) - new Vector2(MyGuiConstants.TOOLTIP_DISTANCE_FROM_BORDER);

                if (bgPosition.X + bgSize.X > rightbottom.X) bgPosition.X = rightbottom.X - bgSize.X;
                if (bgPosition.Y + bgSize.Y > rightbottom.Y) bgPosition.Y = rightbottom.Y - bgSize.Y;
                if (bgPosition.X < topleft.X) bgPosition.X = topleft.X;
                if (bgPosition.Y < topleft.Y) bgPosition.Y = topleft.Y;

                MyGUIHelper.FillRectangle(bgPosition, bgSize, MyGuiConstants.TOOL_TIP_BACKGROUND_COLOR);
                MyGUIHelper.OutsideBorder(bgPosition, bgSize, 1, MyGuiConstants.TOOL_TIP_BORDER_COLOR);

                Vector2 toolTipPosition = bgPosition + new Vector2(innerBorder.X, bgSize.Y / 2 - GetSize().Y / 2f);

                foreach (MyColoredText toolTip in GetToolTips())
                {
                    toolTipPosition.Y += toolTip.Draw(toolTipPosition, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP, 1f, false).Size.Y;
                }
            }
        }
    }
}
