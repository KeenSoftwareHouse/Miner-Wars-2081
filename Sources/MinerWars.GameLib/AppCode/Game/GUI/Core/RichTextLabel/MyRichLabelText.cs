using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWarsMath;

namespace MinerWars.AppCode.Game.GUI.Core
{
    class MyRichLabelText : MyRichLabelPart
    {
        private StringBuilder m_text;
        private MyGuiFont m_font;
        private Vector4 m_color;
        private float m_scale;

        private Vector2 m_size;

        public MyRichLabelText(StringBuilder text, MyGuiFont font, float scale, Vector4 color)
        {
            m_text = text;
            m_font = font;
            m_scale = scale;
            m_color = color;
            RecalculateSize();
        }

        public MyRichLabelText()
        {
            m_text = new StringBuilder(512);
            m_font = null;
            m_scale = 0;
            m_color = Vector4.Zero;
        }

        public void Init(string text, MyGuiFont font, float scale, Vector4 color)
        {
            m_text.Append(text);
            m_font = font;
            m_scale = scale;
            m_color = color;
            RecalculateSize();
        }

        public StringBuilder Text
        {
            get
            {
                return m_text;
            }
            set
            {
                m_text = value;
                RecalculateSize();
            }
        }

        public void Append(string text)
        {
            m_text.Append(text);
            RecalculateSize();
        }

        public float Scale
        {
            get
            {
                return m_scale;
            }
            set
            {
                m_scale = value;
                RecalculateSize();
            }
        }

        public MyGuiFont Font
        {
            get
            {
                return m_font;
            }
            set
            {
                m_font = value;
                RecalculateSize();
            }
        }

        public Vector4 Color
        {
            get { return m_color; }
            set { m_color = value; }
        }

        private void RecalculateSize()
        {
            m_size = MyGuiManager.GetNormalizedSize(m_font, m_text, m_scale);
        }

        public override Vector2 GetSize()
        {
            return m_size;
        }

        /// <summary>
        /// Draws text
        /// </summary>
        /// <param name="position">Top-left position</param>
        /// <returns></returns>
        public override bool Draw(Vector2 position)
        {
            MyGuiManager.DrawString(m_font, m_text, position, m_scale, new Color(m_color), MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP);
            return true;
        }
    }
}
