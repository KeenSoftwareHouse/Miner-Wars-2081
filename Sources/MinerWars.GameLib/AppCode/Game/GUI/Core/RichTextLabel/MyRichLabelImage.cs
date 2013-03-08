using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.Textures;

namespace MinerWars.AppCode.Game.GUI.Core
{
    class MyRichLabelImage : MyRichLabelPart
    {
        private MyTexture2D m_texture;
        private Vector4 m_color;
        private Vector2 m_size;

        public MyRichLabelImage(MyTexture2D texture, Vector2 size, Vector4 color)
        {
            m_texture = texture;
            m_size = size;
            m_color = color;
        }

        public MyTexture2D Texture
        {
            get { return m_texture; }
            set { m_texture = value; }
        }

        public Vector4 Color
        {
            get { return m_color; }
            set { m_color = value; }
        }

        public Vector2 Size
        {
            get { return m_size; }
            set { m_size = value; }
        }

        public override Vector2 GetSize()
        {
            return m_size;
        }

        /// <summary>
        /// Draws image
        /// </summary>
        /// <param name="position">Top-left position</param>
        /// <returns></returns>
        public override bool Draw(Vector2 position)
        {
            MyGuiManager.DrawSpriteBatch(m_texture, position, m_size, new Color(m_color), MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP);
            return true;
        }
    }
}
