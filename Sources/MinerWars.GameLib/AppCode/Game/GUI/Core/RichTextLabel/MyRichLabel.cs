using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.Textures;

namespace MinerWars.AppCode.Game.GUI.Core
{
    class MyRichLabel
    {
        private static readonly string[] m_lineSeparators = new[] { "\n", "\r\n" };
        private const char m_wordSeparator = ' ';

        private float m_maxLineWidth;
        private float m_minLineHeight;

        private List<MyRichLabelLine> m_lines;

        private MyRichLabelLine m_currentLine;
        private float m_currentLineRestFreeSpace;
        private StringBuilder m_helperSb;

        private MyRichLabelLine m_emptyLine;
        private int m_linesCount;
        private int m_linesCountMax;

        private List<MyRichLabelText> m_richTextsPool;
        private int m_richTextsOffset;
        private int m_richTextsMax;


        public MyRichLabel(float maxLineWidth, float minLineHeight)
        {
            m_maxLineWidth = maxLineWidth;
            m_minLineHeight = minLineHeight;
            m_helperSb = new StringBuilder(256);

            Init();
        }

        private void Init()
        {
            m_helperSb.Clear();

            m_linesCount = 0;
            m_linesCountMax = 64;
            m_richTextsOffset = 0;
            m_richTextsMax = 64;
            m_currentLineRestFreeSpace = m_maxLineWidth;

            m_lines = new List<MyRichLabelLine>(m_linesCountMax);
            for (int i = 0; i < m_linesCountMax; i++)
            {
                m_lines.Add(new MyRichLabelLine(m_minLineHeight));
            }

            m_richTextsPool = new List<MyRichLabelText>(m_richTextsMax);
            for (int i = 0; i < m_richTextsMax; i++)
            {
                m_richTextsPool.Add(new MyRichLabelText());
            }
        }

        private void RealocateLines()
        {
            if ((m_linesCount + 1) >= m_linesCountMax)
            {
                m_linesCountMax *= 2;
                m_lines.Capacity = m_linesCountMax;
                for (int i = m_linesCount + 1; i < m_linesCountMax; i++)
                {
                    m_lines.Add(new MyRichLabelLine(m_minLineHeight));
                }
            }
        }

        private void RealocateRichTexts()
        {
            if ((m_richTextsOffset + 1) >= m_richTextsMax)
            {
                m_richTextsMax *= 2;
                m_richTextsPool.Capacity = m_richTextsMax;
                for (int i = m_richTextsOffset + 1; i < m_richTextsMax; i++)
                {
                    m_richTextsPool.Add(new MyRichLabelText());
                }
            }
        }

        public void Append(StringBuilder text, MyGuiFont font, float scale, Vector4 color)
        {
            Append(text.ToString(), font, scale, color);
        }

        public void Append(string text, MyGuiFont font, float scale, Vector4 color)
        {
            string[] paragraphs = text.Split(m_lineSeparators, StringSplitOptions.None);

            for (int i = 0; i < paragraphs.Length; i++)
            {
                AppendParagraph(paragraphs[i], font, scale, color);
                if (i < paragraphs.Length - 1)
                {
                    AppendLine();
                }
            }
        }

        public void Append(MyTexture2D texture, Vector2 size, Vector4 color)
        {
            MyRichLabelImage image = new MyRichLabelImage(texture, size, color);
            if (image.GetSize().X > m_currentLineRestFreeSpace)
            {
                AppendLine();
            }
            AppendPart(new MyRichLabelImage(texture, size, color));
        }

        public void AppendLine()
        {
            RealocateLines();
            m_currentLine = m_lines[++m_linesCount];
            m_currentLineRestFreeSpace = m_maxLineWidth;
            RealocateRichTexts();

        }

        public void AppendLine(StringBuilder text, MyGuiFont font, float scale, Vector4 color)
        {
            Append(text, font, scale, color);
            AppendLine();
        }

        public void AppendLine(MyTexture2D texture, Vector2 size, Vector4 color)
        {
            Append(texture, size, color);
            AppendLine();
        }

        private void AppendParagraph(string paragraph, MyGuiFont font, float scale, Vector4 color)
        {
            m_helperSb.Clear();
            m_helperSb.Append(paragraph);
            float textWidth = MyGuiManager.GetNormalizedSize(font, m_helperSb, scale).X;
            // first we try append all paragraph to current line
            if (textWidth < m_currentLineRestFreeSpace)
            {
                RealocateRichTexts();
                m_richTextsPool[++m_richTextsOffset].Init(m_helperSb.ToString(), font, scale, color);
                AppendPart(m_richTextsPool[m_richTextsOffset]);
            }
            // if there is not enough free space in current line for whole paragraph
            else
            {
                RealocateRichTexts();
                m_richTextsPool[++m_richTextsOffset].Init("", font, scale, color);
                string[] words = paragraph.Split(m_wordSeparator);
                int currentWordIndex = 0;
                while (currentWordIndex < words.Length)
                {
                    if (words[currentWordIndex].Trim().Length == 0)
                    {
                        currentWordIndex++;
                        continue;
                    }

                    m_helperSb.Clear();
                    if (m_richTextsPool[m_richTextsOffset].Text.Length > 0)
                    {
                        m_helperSb.Append(m_wordSeparator);
                    }
                    m_helperSb.Append(words[currentWordIndex]);

                    textWidth = MyGuiManager.GetNormalizedSize(font, m_helperSb, scale).X;

                    if (textWidth <= m_currentLineRestFreeSpace - m_richTextsPool[m_richTextsOffset].GetSize().X)
                    {
                        m_richTextsPool[m_richTextsOffset].Append(m_helperSb.ToString());
                        currentWordIndex++;
                    }
                    else
                    {
                        // if this word is wider than line and it will be only one word at line, then we append it to current line
                        if ((m_currentLine == null || m_currentLine.IsEmpty()) && m_richTextsPool[m_richTextsOffset].Text.Length == 0)
                        {
                            m_richTextsPool[m_richTextsOffset].Append(m_helperSb.ToString());
                            currentWordIndex++;
                        }

                        AppendPart(m_richTextsPool[m_richTextsOffset]);
                        RealocateRichTexts();
                        m_richTextsPool[++m_richTextsOffset].Init("", font, scale, color);
                        if (currentWordIndex < words.Length)
                        {
                            AppendLine();
                        }
                    }
                }

                if (m_richTextsPool[m_richTextsOffset].Text.Length > 0)
                {
                    AppendPart(m_richTextsPool[m_richTextsOffset]);
                }
            }
        }

        private void AppendPart(MyRichLabelPart part)
        {
            m_currentLine = m_lines[m_linesCount];
            m_currentLine.AddPart(part);
            m_currentLineRestFreeSpace = m_maxLineWidth - m_currentLine.GetSize().X;
        }

        /// <summary>
        /// Draws label
        /// </summary>
        /// <param name="position">Top-left position</param>
        /// <param name="offset"></param>
        /// <param name="drawSizeMax"></param>
        /// <returns></returns>
        public bool Draw(Vector2 position, float offset, Vector2 drawSizeMax)
        {
            float skippedSizeY = 0f;
            int currentLineIndex = 0;
            int linesPrinted = 0;

            while (offset > skippedSizeY)
            {
                float lineSizeY = m_lines[currentLineIndex].GetSize().Y;
                //if(skippedSizeY + lineSizeY > offset)
                //{
                //    break;
                //}
                skippedSizeY += lineSizeY;
                currentLineIndex++;
            }

            ////  End our standard sprite batch
            //MyGuiManager.EndSpriteBatch();
            ////  Draw the rectangle(basically the opened area) to stencil buffer to be used for clipping partial item
            //MyGuiManager.DrawStencilMaskRectangle(new MyRectangle2D(position, drawSizeMax), MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP);
            ////  Set up the stencil operation and parameters
            //MyGuiManager.BeginSpriteBatch_StencilMask();

            Vector2 drawPosition = Vector2.Zero;
            while (currentLineIndex <= m_linesCount)
            {
                MyRichLabelLine currentLine = m_lines[currentLineIndex];

                if (drawPosition.Y + currentLine.GetSize().Y > drawSizeMax.Y && linesPrinted >= 1)
                {
                    break;
                }

                currentLine.Draw(position + drawPosition);
                drawPosition.Y += currentLine.GetSize().Y;
                currentLineIndex++;
                linesPrinted++;
            }

            ////  End stencil-mask batch, and restart the standard sprite batch
            //MyGuiManager.EndSpriteBatch();
            //MyGuiManager.BeginSpriteBatch();

            return true;
        }

        public Vector2 GetSize()
        {
            Vector2 size = Vector2.Zero;

            for (int i = 0; i < m_linesCount; i++)
            {
                Vector2 lineSize = m_lines[i].GetSize();
                size.Y += lineSize.Y;
                if (size.X < lineSize.X)
                {
                    size.X = lineSize.X;
                }
            }

            return size;
        }

        public void Clear()
        {
            m_lines.Clear();
            m_currentLine = null;
            Init();
        }
    }
}
