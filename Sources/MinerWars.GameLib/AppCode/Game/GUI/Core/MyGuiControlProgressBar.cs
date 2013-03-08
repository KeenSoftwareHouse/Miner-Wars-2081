using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWarsMath;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.Utils;
using KeenSoftwareHouse.Library.Extensions;

namespace MinerWars.AppCode.Game.GUI.Core
{
    class MyGuiControlProgressBar : MyGuiControlBase
    {
        private Vector4 m_progressColor;
        private Vector4 m_textColor;
        private float m_textScale;        
        private float m_value;
        private StringBuilder m_progressValueText;
        private int m_decimals;

        private const char PERCENTAGE = '%';

        public MyGuiControlProgressBar(IMyGuiControlsParent parent, Vector2 position, Vector2? size, Vector4 backgroundColor, Vector4 progressColor, 
            Vector4 textColor, float textScale, float value, int decimals)
            : base(parent, position, size, backgroundColor, null)
        {
            m_progressColor = progressColor;
            m_textColor = textColor;
            m_textScale = textScale;            
            m_value = value;
            m_progressValueText = new StringBuilder();
            m_decimals = decimals;
        }

        public float Value 
        {
            get { return m_value; }
            set { m_value = value; }
        }

        public override void Draw()
        {
            base.Draw();
            MyGuiManager.EndSpriteBatch();
            Vector2 normalizeSize = new Vector2(483 / 1600f, 112 / 1200f);//MyGuiManager.GetNormalizedCoordsAndPreserveOriginalSize(483, 112); 559?
            Vector2 stencilPos = GetParent().GetPositionAbsolute() - m_size.Value / 2f;
            Vector2 senctilSize = new Vector2(normalizeSize.X * m_value, normalizeSize.Y) + new Vector2(38 / 1600f, 0);//MyGuiManager.GetNormalizedCoordsAndPreserveOriginalSize(38, 0);            
            Utils.MyRectangle2D stencilRect = new Utils.MyRectangle2D(stencilPos, senctilSize);            
            MyGuiManager.DrawStencilMaskRectangle(stencilRect, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP);
            MyGuiManager.BeginSpriteBatch_StencilMask();
            // draw progress bar            
            MyGuiManager.DrawSpriteBatch(MyGuiManager.GetProgressBarTexture(), GetParent().GetPositionAbsolute(), m_size.Value, new Color(m_progressColor), MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER);
            MyGuiManager.EndSpriteBatch_StencilMask();
            MyGuiManager.BeginSpriteBatch();

            // draw progress value
            m_progressValueText.Clear();
            float percentage = m_value * 100f;
            if (m_decimals > 0)
            {
                m_progressValueText.AppendDecimal(percentage, m_decimals);
            }
            else 
            {
                m_progressValueText.AppendInt32((int)percentage);
            }
            m_progressValueText.Append(PERCENTAGE);
            MyGuiManager.DrawString(MyGuiManager.GetFontMinerWarsWhite(), m_progressValueText, GetParent().GetPositionAbsolute() - new Vector2(0f, m_size.Value.Y / 2f), m_textScale, new Color(m_textColor), MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER);
        }
    }
}
