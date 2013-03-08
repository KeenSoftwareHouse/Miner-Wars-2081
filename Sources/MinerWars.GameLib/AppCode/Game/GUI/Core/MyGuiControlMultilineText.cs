using System.Collections.Generic;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.GUI.Core.TreeView;

namespace MinerWars.AppCode.Game.GUI.Core
{
    class MyGuiControlMultilineText : MyGuiControlBase
    {
        #region Fields

        private readonly MyGuiFont m_font;
        private readonly float m_textScale;
        private readonly MyGuiDrawAlignEnum m_textAlign;

        private readonly MyVScrollbar m_scrollbar;
        private Vector2 m_scrollbarSize;
        private MyRichLabel m_label;

        private bool m_drawBorders;
        private bool m_drawScrollbar;
        private float m_scrollbarOffset;

        #endregion

        /// <summary>
        /// Gets or sets the color of the text.
        /// </summary>
        public Color TextColor { get; set; }

        public float ScrollbarOffset
        {
            get { return m_scrollbarOffset; }
            set { m_scrollbarOffset = value; RecalculateScrollBar();}
        }

        public MyGuiControlMultilineText(
            IMyGuiControlsParent parent, Vector2 position,
            Vector2 size, Vector4? backgroundColor,
            MyGuiFont font, float textScale, MyGuiDrawAlignEnum textAlign, StringBuilder contents, bool drawBorders = true, bool drawScrollbar = true)
            : base(parent, position, size, backgroundColor, null)
        {
            m_font = font;
            m_textScale = textScale;
            m_textAlign = textAlign;
            m_drawBorders = drawBorders;
            m_drawScrollbar = drawScrollbar;
            TextColor = new Color(MyGuiConstants.LABEL_TEXT_COLOR);

            m_scrollbar = new MyVScrollbar(this);
            m_scrollbar.TopBorder = m_scrollbar.RightBorder = m_scrollbar.BottomBorder = false;
            m_scrollbar.LeftBorder = drawBorders;
            m_scrollbarSize = new Vector2(0.0334f, MyGuiConstants.COMBOBOX_VSCROLLBAR_SIZE.Y);
            m_scrollbarSize = MyGuiConstants.COMBOBOX_VSCROLLBAR_SIZE;
            float minLineHeight = MyGuiManager.MeasureString(m_font,
                                                      MyTextsWrapper.Get(MyTextsWrapperEnum.ServerShutdownNotificationCaption),
                                                      m_parent.GetPositionAbsolute() + m_position, m_textScale,
                                                      m_textAlign).Size.Y;
            m_label = new MyRichLabel(size.X, minLineHeight);
            if (contents != null && contents.Length > 0)
            {
                SetText(contents);
            }
        }                

        /// <summary>
        /// Sets the text to the given StringBuilder value.
        /// Layouts the controls.
        /// </summary>
        /// <param name="value"></param>
        public void SetText(StringBuilder value)
        {
            m_label.Clear();
            AppendText(value);
        }
        
        public void AppendText(StringBuilder text)
        {
            AppendText(text, m_font, m_textScale, TextColor.ToVector4());
            RecalculateScrollBar();
        }

        public void AppendText(StringBuilder text, MyGuiFont font, float scale, Vector4 color)
        {
            m_label.Append(text, font, scale, color);
            RecalculateScrollBar();
        }

        public void AppendImage(MyTexture2D texture, Vector2 size, Vector4 color)
        {
            m_label.Append(texture, size, color);
            RecalculateScrollBar();
        }

        public void AppendLine()
        {
            m_label.AppendLine();
            RecalculateScrollBar();
        }

        public void Clear()
        {
            m_label.Clear();
            RecalculateScrollBar();
        }        

        private void RecalculateScrollBar()
        {
            float realHeight = m_label.GetSize().Y;

            bool vScrollbarVisible = m_size.Value.Y < realHeight;

            m_scrollbar.Visible = vScrollbarVisible;
            m_scrollbar.Init(realHeight, (m_size.Value - 2f * MyGuiConstants.MULTILINE_LABEL_BORDER).Y);
            m_scrollbar.Layout(m_position + m_parent.GetPositionAbsolute() - m_size.Value / 2 + new Vector2(ScrollbarOffset, 0), m_size.Value, m_scrollbarSize, false);
        }

        public override void Draw()
        {
            base.Draw();

            if (m_drawBorders)
                DrawBorders(m_parent.GetPositionAbsolute() + m_position - .5f * m_size.Value + new Vector2(ScrollbarOffset,0), m_size.Value, new Color(MyGuiConstants.BUTTON_TEXT_COLOR), 2);
            DrawText(m_scrollbar.GetValue());
            if (m_drawScrollbar)
                m_scrollbar.Draw();
        }

        public override bool HandleInput(MyGuiInput input, bool hasKeyboardActiveControl, bool hasKeyboardActiveControlPrevious, bool receivedFocusInThisUpdate)
        {
            bool baseResult = base.HandleInput(input, hasKeyboardActiveControl, hasKeyboardActiveControlPrevious, receivedFocusInThisUpdate);

            bool captured = false;
            var deltaWheel = input.DeltaMouseScrollWheelValue();
            if (IsMouseOver() && deltaWheel != 0)
            {
                m_scrollbar.ChangeValue(-0.0005f * deltaWheel);
                captured = true;
            }

            bool capturedScrollbar = m_scrollbar.HandleInput(input, hasKeyboardActiveControl, hasKeyboardActiveControlPrevious, receivedFocusInThisUpdate);

            return baseResult || capturedScrollbar || captured;
        }

        /// <summary>
        /// Draws the text with the offset given by the scrollbar.
        /// </summary>
        /// <param name="offset">Indicates how low is the scrollbar (and how many beginning lines are skipped)</param>
        private void DrawText(float offset)
        {
            Vector2 position = m_parent.GetPositionAbsolute() + m_position - 0.5f * m_size.Value + MyGuiConstants.MULTILINE_LABEL_BORDER;
            Vector2 drawSizeMax = m_size.Value - 2f * MyGuiConstants.MULTILINE_LABEL_BORDER;
            m_label.Draw(position, offset, drawSizeMax);
        }
    }
}
