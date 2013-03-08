using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.Utils;

namespace MinerWars.AppCode.Game.GUI.Core
{    
    enum MyScrollDirection
    {
        Vertical,
        Horizontal
    }
    class MyGuiControlListboxScrollBar
    {
        #region fields
        private MyScrollDirection m_scrollDirection;
        private float m_scrollValueMin;
        private float m_scrollValueMax;
        private float m_scrollValue;

        private Vector2 m_scrollSliderSize;
        private Vector2 m_scrollSliderSizeMax;
        private Vector2 m_scrollSliderSizeMin;

        private Vector2 m_scrollSliderPosition;
        private Vector2 m_scrollSliderPositionMin;
        private Vector2 m_scrollSliderPositionMax;

        private Vector2? m_mouseLastPosition;
        private bool m_isScrollSliderDragging;

        private MyTexture2D m_texture;

        private Vector2 m_position;         // topleft position
        private Vector2 m_size;
        private IMyGuiControlsParent m_parent;
        #endregion

        #region constructors
        public MyGuiControlListboxScrollBar(IMyGuiControlsParent parent, Vector2 position, Vector2 size, Vector4? backgroundColor, Color sliderColor, MyScrollDirection scrollDirection, MyTexture2D texture)
        {
            m_parent = parent;
            m_position = position;
            m_size = size;
            m_scrollDirection = scrollDirection;
            m_texture = texture;
            m_isScrollSliderDragging = false;
            InitializeScrollBar(1.0f);
            SetScrollValue(1.0f);
        }
        #endregion

        #region events and delegates
        public event EventHandler ScrollValueChanged;
        #endregion

        #region public methods
        /// <summary>
        /// Returns scrollbar's size
        /// </summary>
        /// <returns></returns>
        public Vector2 GetSize()
        {
            return m_size;
        }

        /// <summary>
        /// Sets new scrollbar's size
        /// </summary>
        /// <param name="newSize"></param>
        public void SetNewSize(Vector2 newSize)
        {
            m_size = newSize;
            RecalculateScrollSliderMinAndMaxSize();
            RecalculateScrollSliderSize();
            RecalculateScrollSliderMinAndMaxPosition();
            RecalculateSrollSliderPosition();
        }

        /// <summary>
        /// Sets new scrollbar's position
        /// </summary>
        /// <param name="newPosition"></param>
        public void SetNewPosition(Vector2 newPosition)
        {
            m_position = newPosition;
            RecalculateScrollSliderMinAndMaxPosition();
            RecalculateSrollSliderPosition();
        }

        /// <summary>
        /// Initialize scrollbar
        /// </summary>
        /// <param name="maxValue">Max value of scrolling (must be >= 1.0f)</param>
        public void InitializeScrollBar(float maxValue)
        {
            m_scrollValueMin = 1.0f;
            m_scrollValueMax = Math.Max(maxValue, 1.0f);
            RecalculateScrollSliderMinAndMaxSize();
            RecalculateScrollSliderSize();
            RecalculateScrollSliderMinAndMaxPosition();
            RecalculateSrollSliderPosition();
        }

        /// <summary>
        /// Sets current scrolling value
        /// </summary>
        /// <param name="value">Scrolling value</param>
        public void SetScrollValue(float value)
        {
            if (value <= m_scrollValueMin)
            {
                m_scrollValue = m_scrollValueMin;
            }
            else if (value >= m_scrollValueMax)
            {
                m_scrollValue = m_scrollValueMax;
            }
            else
            {
                m_scrollValue = value;
            }
            RecalculateSrollSliderPosition();
        }

        /// <summary>
        /// Returns current scrolling value
        /// </summary>
        /// <returns></returns>
        public float GetScrollValue()
        {
            return m_scrollValue;
        }

        /// <summary>
        /// Returns scrolling ratio 0 - 1 ... (0% - 100%)
        /// </summary>
        /// <returns></returns>
        public float GetScrollRatio()
        {
            if (m_scrollValueMax > m_scrollValueMin)
            {
                return (m_scrollValue - m_scrollValueMin) / (m_scrollValueMax - m_scrollValueMin);
            }
            else
            {
                return 0.0f;
            }
        }

        /// <summary>
        /// Returns true if is scrolling now
        /// </summary>
        /// <returns></returns>
        public bool IsScrolling()
        {
            return m_isScrollSliderDragging;
        }

        /// <summary>
        /// Handling scrollbar's input
        /// </summary>
        /// <param name="input"></param>
        /// <param name="hasKeyboardActiveControl"></param>
        /// <param name="hasKeyboardActiveControlPrevious"></param>
        /// <param name="receivedFocusInThisUpdate"></param>
        /// <returns></returns>
        public bool HandleInput(MyGuiInput input, bool hasKeyboardActiveControl, bool hasKeyboardActiveControlPrevious, bool receivedFocusInThisUpdate)
        {
            bool captureInput = false;
            // still scrolling
            if (IsScrolling() && input.IsLeftMousePressed())
            {
                if (m_mouseLastPosition != null)
                {
                    Vector2 distance = new Vector2(0.0f, 0.0f);
                    Vector2 sliderDistanceFromLastUpdate = MyGuiManager.MouseCursorPosition - m_mouseLastPosition.Value;
                    switch (m_scrollDirection)
                    {
                        case MyScrollDirection.Horizontal:
                            distance.X = sliderDistanceFromLastUpdate.X;
                            break;
                        case MyScrollDirection.Vertical:
                            distance.Y = sliderDistanceFromLastUpdate.Y;
                            break;
                    }
                    if (distance.Length() > 0.00001f)
                    {
                        Vector2 newScrollSliderPosition = m_scrollSliderPosition + distance;
                        FixScrollSliderPosition(ref newScrollSliderPosition);

                        if (newScrollSliderPosition != m_scrollSliderPosition)
                        {                            
                            SetScrollSliderPosition(newScrollSliderPosition);
                            m_mouseLastPosition = MyGuiManager.MouseCursorPosition;
                        }

                    }
                    captureInput = true;
                }
            }
            // start scrolling
            else if (!IsScrolling() && input.IsNewLeftMousePressed() && IsMouseOverScrollSlider())
            {
                m_isScrollSliderDragging = true;
                m_mouseLastPosition = MyGuiManager.MouseCursorPosition;
                captureInput = true;
            }
            // no scrolling
            else
            {
                m_mouseLastPosition = null;
                m_isScrollSliderDragging = false;
            }

            return captureInput;
        }

        /// <summary>
        /// Draw's scrollbar
        /// </summary>
        public void Draw()
        {
            Vector4 vctColor = Color.White.ToVector4();
            vctColor.W *= m_parent.GetTransitionAlpha();
            MyGuiManager.DrawSpriteBatch(m_texture, m_parent.GetPositionAbsolute() + m_scrollSliderPosition, m_scrollSliderSize, new Color(vctColor), MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP);
        }
        #endregion

        #region private methods
        private void SetScrollSliderPosition(Vector2 position)
        {
            m_scrollSliderPosition = position;
            RecalculateScrollValueByScrollSliderPosition();
        }

        private bool IsMouseOverScrollSlider()
        {
            Vector2 normalPosition = m_parent.GetPositionAbsolute() + m_scrollSliderPosition;
            Vector2 minPosition = normalPosition;
            Vector2 maxPosition = normalPosition + (m_scrollSliderSize);

            return MyGuiManager.MouseCursorPosition.X >= minPosition.X && MyGuiManager.MouseCursorPosition.X <= maxPosition.X &&
               MyGuiManager.MouseCursorPosition.Y >= minPosition.Y && MyGuiManager.MouseCursorPosition.Y <= maxPosition.Y;
        }

        private void RecalculateScrollValueByScrollSliderPosition()
        {
            float oldScrollValue = m_scrollValue;            
            float sliderMaxAndMinPositionDelta = (m_scrollSliderPositionMax - m_scrollSliderPositionMin).Length();
            float scrollRatio;
            if (sliderMaxAndMinPositionDelta != 0f)
            {
                scrollRatio = (m_scrollSliderPosition - m_scrollSliderPositionMin).Length() / sliderMaxAndMinPositionDelta;
            }
            else
            {
                scrollRatio = 0.0f;
            }
            m_scrollValue = scrollRatio * (m_scrollValueMax - m_scrollValueMin) + m_scrollValueMin;
            if (oldScrollValue != m_scrollValue && ScrollValueChanged != null)
            {
                ScrollValueChanged(this, EventArgs.Empty);
            }
        }

        private void RecalculateSrollSliderPosition()
        {
            float scrollRatio = GetScrollRatio();
            Vector2 scrollSliderOffset = (m_scrollSliderPositionMax - m_scrollSliderPositionMin) * new Vector2(scrollRatio, scrollRatio);
            m_scrollSliderPosition = m_scrollSliderPositionMin + scrollSliderOffset;
        }

        private void RecalculateScrollSliderMinAndMaxPosition()
        {
            Vector2 topLeftPosition = m_position;
            switch (m_scrollDirection)
            {
                case MyScrollDirection.Horizontal:
                    m_scrollSliderPositionMin = topLeftPosition;
                    m_scrollSliderPositionMax = topLeftPosition + new Vector2(m_size.X - m_scrollSliderSize.X, 0.0f);
                    break;
                case MyScrollDirection.Vertical:
                    m_scrollSliderPositionMin = topLeftPosition;
                    m_scrollSliderPositionMax = topLeftPosition + new Vector2(0.0f, m_size.Y - m_scrollSliderSize.Y);
                    break;
            }
        }

        private void RecalculateScrollSliderMinAndMaxSize()
        {
            switch (m_scrollDirection)
            {
                case MyScrollDirection.Horizontal:
                    m_scrollSliderSizeMin = new Vector2(MyGuiConstants.LISTBOX_SCROLLBAR_MIN_SIZE, m_size.Y);
                    m_scrollSliderSizeMax = new Vector2(MyGuiConstants.LISTBOX_SCROLLBAR_MAX_SIZE, m_size.Y);
                    break;
                case MyScrollDirection.Vertical:
                    m_scrollSliderSizeMin = new Vector2(m_size.X, MyGuiConstants.LISTBOX_SCROLLBAR_MIN_SIZE);
                    m_scrollSliderSizeMax = new Vector2(m_size.X, MyGuiConstants.LISTBOX_SCROLLBAR_MAX_SIZE);
                    break;
            }
        }

        private void RecalculateScrollSliderSize()
        {
            Vector2 newSliderSize = m_scrollSliderSizeMax;
            switch (m_scrollDirection)
            {
                case MyScrollDirection.Horizontal:
                    newSliderSize.X = newSliderSize.X / m_scrollValueMax;
                    break;
                case MyScrollDirection.Vertical:
                    newSliderSize.Y = newSliderSize.Y / m_scrollValueMax;
                    break;
            }
            if (newSliderSize.Length() > m_scrollSliderSizeMin.Length())
            {
                m_scrollSliderSize = newSliderSize;
            }
            else
            {
                m_scrollSliderSize = m_scrollSliderSizeMin;
            }
            //m_scrollSliderSize = m_scrollSliderSizeMax;
        }

        private void FixScrollSliderPosition(ref Vector2 scrollSliderPosition)
        {
            if (scrollSliderPosition.Y < m_scrollSliderPositionMin.Y) scrollSliderPosition.Y = m_scrollSliderPositionMin.Y;
            if (scrollSliderPosition.X < m_scrollSliderPositionMin.X) scrollSliderPosition.X = m_scrollSliderPositionMin.X;
            if (scrollSliderPosition.Y > m_scrollSliderPositionMax.Y) scrollSliderPosition.Y = m_scrollSliderPositionMax.Y;
            if (scrollSliderPosition.X > m_scrollSliderPositionMax.X) scrollSliderPosition.X = m_scrollSliderPositionMax.X;
        }

        #endregion
    }
}
