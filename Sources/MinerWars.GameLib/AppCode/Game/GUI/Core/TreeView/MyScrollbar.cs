using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.Utils;

namespace MinerWars.AppCode.Game.GUI.Core.TreeView
{
    abstract class MyScrollbar
    {
        public bool TopBorder = true;
        public bool BottomBorder = true;
        public bool LeftBorder = true;
        public bool RightBorder = true;

        public Vector2? BorderNormalizedOffset = null;

        //protected readonly float MIN_CARRET_SIZE = 0.05f;

        protected enum State
        {
            READY,
            DRAG
        }

        protected MyGuiControlBase m_control;

        protected Vector2 m_position;
        protected Vector2 m_size;
        protected Vector2 m_caretSize;

        protected float m_max;
        protected float m_page;
        protected float m_value;

        protected State m_state;

        protected MyScrollbar(MyGuiControlBase control)
        {
            m_control = control;
        }

        public bool Visible;

        public event EventHandler OnScrollValueChanged;

        protected bool CanScroll()
        {
            return m_max > 0 && m_max > m_page;
        }

        public void Init(float max, float page)
        {
            m_max = max;
            m_page = page;

            ChangeValue(0);
        }

        public float GetValue()
        {
            return m_value;
        }


        public void SetValue(float value)
        {
            float oldValue = m_value;
            m_value = value;
            m_value = MathHelper.Clamp(m_value, 0, m_max - m_page);
            if(oldValue != m_value && OnScrollValueChanged != null)
            {
                OnScrollValueChanged(this, EventArgs.Empty);
            }
        }

        public void ChangeValue(float amount)
        {
            SetValue(m_value + amount);
        }

        public void PageDown()
        {
            ChangeValue(m_page);
        }

        public void PageUp()
        {
            ChangeValue(-m_page);
        }

        public abstract void Layout(Vector2 position, Vector2 size, Vector2 scrollSize, bool trim);
        public abstract void Draw();
        public abstract bool HandleInput(MyGuiInput input, bool hasKeyboardActiveControl, bool hasKeyboardActiveControlPrevious, bool receivedFocusInThisUpdate);
    }

    class MyVScrollbar : MyScrollbar
    {
        private Vector2 m_dragClick;

        public MyVScrollbar(MyGuiControlBase control)
            : base(control)
        {

        }

        private Vector2 GetCarretPosition()
        {
            return new Vector2(0, m_value * (m_size.Y - m_caretSize.Y) / (m_max - m_page));
        }

        public override void Layout(Vector2 position, Vector2 size, Vector2 scrollSize, bool trim)
        {
            m_position = position + new Vector2(size.X - scrollSize.X, 0);
            m_size = new Vector2(scrollSize.X, size.Y - (trim ? scrollSize.Y : 0.0f));

            if (CanScroll())
            {
                //m_caretSize = new Vector2(scrollSize.X, Math.Max(m_page / m_max * m_size.Y, MIN_CARRET_SIZE));
                if (m_caretSize!= Vector2.Zero)
                {
                    m_caretSize = MyGuiConstants.COMBOBOX_VSCROLLBAR_SIZE;
                }
                else
                {
                    m_caretSize = scrollSize;
                }
            }
        }

        public override void Draw()
        {
            if (!Visible)
            {
                return;
            }

            if (CanScroll())
            {
                Vector2 carretPosition = GetCarretPosition();
                //GUIHelper.FillRectangle(m_position + carretPosition, m_caretSize, new Color(0.5f, 0.5f, 0, 0.5f));
                MyGuiManager.DrawSpriteBatch(MyGuiManager.GetScrollbarSlider(), m_position + carretPosition, m_caretSize, Color.White, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP);
            }

            Color borderColor = m_control.GetColorAfterTransitionAlpha(MyGuiConstants.TREEVIEW_VERTICAL_LINE_COLOR);
            MyGUIHelper.Border(m_position, m_size, 1, borderColor, TopBorder, BottomBorder, LeftBorder, RightBorder, BorderNormalizedOffset);
        }

        public override bool HandleInput(MyGuiInput input, bool hasKeyboardActiveControl, bool hasKeyboardActiveControlPrevious, bool receivedFocusInThisUpdate)
        {
            bool captured = false;
            if (!CanScroll())
            {
                return false;
            }

            switch (m_state)
            {
                case State.READY:
                    if (input.IsNewLeftMousePressed() && MyGUIHelper.Contains(m_position + GetCarretPosition(), m_caretSize, MyGuiManager.MouseCursorPosition.X, MyGuiManager.MouseCursorPosition.Y))
                    {
                        captured = true;
                        m_state = State.DRAG;
                        m_dragClick = MyGuiManager.MouseCursorPosition;
                    }
                    break;
                case State.DRAG:
                    if (!input.IsLeftMousePressed())
                    {
                        m_state = State.READY;
                    }
                    else
                    {
                        ChangeValue((MyGuiManager.MouseCursorPosition.Y - m_dragClick.Y) * (m_max - m_page) / (m_size.Y - m_caretSize.Y));
                        m_dragClick = MyGuiManager.MouseCursorPosition;
                    }
                    captured = true;
                    break;
            }

            return captured;
        }
    }

    class MyHScrollbar : MyScrollbar
    {
        private Vector2 m_dragClick;

        public MyHScrollbar(MyGuiControlBase control)
            : base(control)
        {

        }

        private Vector2 GetCarretPosition()
        {
            return new Vector2(m_value * (m_size.X - m_caretSize.X) / (m_max - m_page), 0);
        }

        public override void Layout(Vector2 position, Vector2 size, Vector2 scrollSize, bool trim)
        {
            m_position = position + new Vector2(0, size.Y - scrollSize.Y);
            m_size = new Vector2(size.X - (trim ? scrollSize.X : 0.0f), scrollSize.Y);

            if (CanScroll())
            {
                //m_caretSize = new Vector2(Math.Max(m_page / m_max * m_size.X, MIN_CARRET_SIZE), scrollSize.Y);
                m_caretSize = MyGuiConstants.COMBOBOX_HSCROLLBAR_SIZE;
            }
        }

        public override void Draw()
        {
            if (!Visible)
            {
                return;
            }

            if (CanScroll())
            {
                Vector2 carretPosition = GetCarretPosition();
                //GUIHelper.FillRectangle(m_position + carretPosition, m_caretSize, new Color(0.5f, 0.5f, 0, 0.5f));
                MyGuiManager.DrawSpriteBatch(MyGuiManager.GetHorizontalScrollbarSlider(), m_position + carretPosition, m_caretSize, Color.White, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP);
            }

            Color borderColor = m_control.GetColorAfterTransitionAlpha(MyGuiConstants.TREEVIEW_VERTICAL_LINE_COLOR);
            MyGUIHelper.Border(m_position, m_size, 1, borderColor, TopBorder, BottomBorder, LeftBorder, RightBorder);
        }

        public override bool HandleInput(MyGuiInput input, bool hasKeyboardActiveControl, bool hasKeyboardActiveControlPrevious, bool receivedFocusInThisUpdate)
        {
            bool captured = false;
            if (!CanScroll())
            {
                return false;
            }

            switch (m_state)
            {
                case State.READY:
                    if (input.IsNewLeftMousePressed() && MyGUIHelper.Contains(m_position + GetCarretPosition(), m_caretSize, MyGuiManager.MouseCursorPosition.X, MyGuiManager.MouseCursorPosition.Y))
                    {
                        captured = true;
                        m_state = State.DRAG;
                        m_dragClick = MyGuiManager.MouseCursorPosition;
                    }
                    break;
                case State.DRAG:
                    if (!input.IsLeftMousePressed())
                    {
                        m_state = State.READY;
                    }
                    else
                    {
                        ChangeValue((MyGuiManager.MouseCursorPosition.X - m_dragClick.X) * (m_max - m_page) / (m_size.X - m_caretSize.X));
                        m_dragClick = MyGuiManager.MouseCursorPosition;
                    }
                    captured = true;
                    break;
            }
            return captured;
        }
    }
}
