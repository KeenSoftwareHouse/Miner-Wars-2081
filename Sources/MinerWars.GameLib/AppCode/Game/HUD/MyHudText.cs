using System.Text;
using MinerWarsMath;
using MinerWarsMath.Graphics;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.CommonLIB.AppCode.Utils;
using KeenSoftwareHouse.Library.Extensions;

namespace MinerWars.AppCode.Game.HUD
{
    class MyHudText
    {
        public MyGuiFont Font;
        public Vector2 Position;                                //  Normalized position in HUD fullscreen (height isn't 1.0)
        public Color Color;
        public float Scale;
        public MyGuiDrawAlignEnum Alignement;
        readonly StringBuilder m_text;


        //  IMPORTANT: This class isn't initialized by constructor, but by Start() because it's supposed to be used in memory pool
        public MyHudText()
        {
            //  Must be preallocated because during game-play we will just use this string object for storing hud texts
            m_text = new StringBuilder(256);
        }

        //  IMPORTANT: This class isn't initialized by constructor, but by Start() because it's supposed to be used in memory pool
        public void Start(MyGuiFont font, Vector2 position, Color color, float scale, MyGuiDrawAlignEnum alignement)
        {
            Font = font;
            Position = position;            
            Color = color;
            Scale = scale;
            Alignement = alignement;

            //  Clear current text
            MyMwcUtils.ClearStringBuilder(m_text);
        }

        public void Append(StringBuilder sb)
        {
            MyMwcUtils.AppendStringBuilder(m_text, sb);
        }

        public void Append(string text)
        {
            m_text.Append(text);
        }

        public void AppendInt32(int number)
        {
            m_text.AppendInt32(number);
        }

        public void AppendLine()
        {
            m_text.AppendLine();
        }

        public StringBuilder GetStringBuilder()
        {
            return m_text;
        }
    }
}
