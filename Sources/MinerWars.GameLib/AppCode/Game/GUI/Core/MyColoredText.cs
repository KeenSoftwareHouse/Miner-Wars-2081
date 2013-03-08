using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.Utils;

namespace MinerWars.AppCode.Game.GUI.Core
{
    class MyColoredText
    {        
        #region Properties
        public StringBuilder Text { get; set; }
        public Color NormalColor { get; set; }
        public Color HighlightColor { get; set; }
        public MyGuiFont Font { get; set; }
        public float Scale { get; set; }
        public Vector2 Offset { get; set; }
        #endregion

        #region Ctors
        public MyColoredText(StringBuilder text)
            : this(text, MyGuiConstants.COLORED_TEXT_DEFAULT_COLOR)
        {
        }

        public MyColoredText(StringBuilder text, Color color)
            : this(text, color, MyGuiConstants.COLORED_TEXT_DEFAULT_HIGHLIGHT_COLOR)
        {
        }

        public MyColoredText(StringBuilder text, Color color, float textScale)
            : this(text, color, MyGuiManager.GetFontMinerWarsWhite(), textScale)
        {
        }

        public MyColoredText(StringBuilder text, Color color, MyGuiFont font, float textScale)
            : this(text, color, MyGuiConstants.COLORED_TEXT_DEFAULT_HIGHLIGHT_COLOR, font, textScale, Vector2.Zero)
        {
        }

        public MyColoredText(StringBuilder text, Color normalColor, Color highlightColor)
            : this(text, normalColor, highlightColor, MyGuiManager.GetFontMinerWarsWhite(), MyGuiConstants.COLORED_TEXT_DEFAULT_TEXT_SCALE, Vector2.Zero)
        {
        }

        public MyColoredText(StringBuilder text, Color normalColor, Color highlightColor, MyGuiFont font, float textScale, Vector2 offset)
        {
            Text = text;
            NormalColor = normalColor;
            HighlightColor = highlightColor;
            Font = font;
            Scale = textScale;
            Offset = offset;
        }
        #endregion

        #region Methods
        public MyRectangle2D Draw(Vector2 normalizedPosition, MyGuiDrawAlignEnum drawAlign, float backgroundAlphaFade, bool isHighlight, float colorMultiplicator = 1f)
        {
            Color drawColor = isHighlight ? HighlightColor : NormalColor;
            Vector4 vctColor = drawColor.ToVector4();
            vctColor.W *= backgroundAlphaFade;
            vctColor *= colorMultiplicator;

            return MyGuiManager.DrawString(Font, Text, normalizedPosition + Offset, Scale, new Color(vctColor), drawAlign);
        }

        public MyRectangle2D Draw(Vector2 normalizedPosition, MyGuiDrawAlignEnum drawAlign, float backgroundAlphaFade, float colorMultiplicator = 1f)
        {
            return Draw(normalizedPosition, drawAlign, backgroundAlphaFade, false, colorMultiplicator);
        }

        #endregion
    }
}
