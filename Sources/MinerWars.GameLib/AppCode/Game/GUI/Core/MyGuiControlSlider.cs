using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Toolkit.Input;
using MinerWars.AppCode.Game.Managers;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.Utils;
using SysUtils.Utils;

namespace MinerWars.AppCode.Game.GUI.Core
{
    class MyGuiControlSlider : MyGuiControlBase
    {
        public delegate void OnSliderChangeCallback(MyGuiControlSlider sender);
        public OnSliderChangeCallback OnChange;

        public int LabelDecimalPlaces { get; set; }

        float m_valueNormalized;        //  This is values selected on slider normalized to <0..1>
        float? m_value;                  //  This is values selected on slider in original units, e.g. meters, so it can be for example 1000 meters
        float m_minValue;               //  This is min value that can be selected on slider in original units, e.g. meters, so it can be for example 100 meters
        float m_maxValue;               //  This is max value that can be selected on slider in original units, e.g. meters, so it can be for example 10,000 meters
        string m_labelText;       //  Label on right side
        float m_labelWidth;
        float m_labelScale;
        Vector4 m_color;
        int m_blinkerTimer;
        bool m_controlCaptured = false;

        StringBuilder m_tempForDraw;        //  Only for drawing

        public MyGuiControlSlider(IMyGuiControlsParent parent, Vector2 position, float width, float minValue, float maxValue,
            Vector4 color, StringBuilder labelText, float labelWidth, int labelDecimalPlaces, float labelScale)
            : this(parent, position, width, minValue, maxValue, color, labelText, labelWidth, labelDecimalPlaces, labelScale, 1.0f)
        {
        }

        public MyGuiControlSlider(IMyGuiControlsParent parent, Vector2 position, float width, float minValue, float maxValue,
            Vector4 color, StringBuilder labelText, float labelWidth, int labelDecimalPlaces, float labelScale, float scale)
            : base(parent, position, new Vector2(width, MyGuiConstants.SLIDER_HEIGHT), MyGuiConstants.SLIDER_BACKGROUND_COLOR, null, MyGuiManager.GetSliderControlTexture(), null, null, true)
        {
            m_canHandleKeyboardActiveControl = true;
            m_color = color;
            m_minValue = minValue;
            m_maxValue = maxValue;
            m_blinkerTimer = 0;
            m_value = minValue;
            m_scale = scale;

            //m_valuePrevious = 0;

            MyCommonDebugUtils.AssertDebug(m_maxValue > m_minValue && m_maxValue != m_minValue);

            m_tempForDraw = new StringBuilder(labelText.Length);

            m_labelText = labelText.ToString();
            m_labelWidth = labelWidth;
            LabelDecimalPlaces = labelDecimalPlaces;
            m_labelScale = labelScale;
        }

        public void SetBounds(float minValue, float maxValue)
        {
            m_minValue = minValue;
            m_maxValue = maxValue;
            SetValue(GetValue());
        }

        public void SetValue(float value)
        {
            value = MathHelper.Clamp(value, m_minValue, m_maxValue);

            if ((m_value.HasValue == false) || (m_value.Value != value))
            {
                m_value = value;
                UpdateNormalizedValue();

                //  Change callback
                if (OnChange != null) OnChange(this);
            }
        }


        public void SetNormalizedValue(float value)
        {
            m_valueNormalized = MathHelper.Clamp(value, 0, 1);
            m_value = m_valueNormalized * (m_maxValue - m_minValue) + m_minValue;
        }

        public float GetValue()
        {
            return m_value.Value;
        }

        public float GetNormalizedValue()
        {
            return (m_value.Value - m_minValue) / (m_maxValue - m_minValue);
        }

        void UpdateNormalizedValue()
        {
            MyCommonDebugUtils.AssertDebug(m_minValue < m_maxValue);
            MyCommonDebugUtils.AssertDebug(m_value >= m_minValue);
            MyCommonDebugUtils.AssertDebug(m_value <= m_maxValue);
            m_valueNormalized = MathHelper.Clamp((m_value.Value - m_minValue) / (m_maxValue - m_minValue), 0, 1);
        }

        //  Method returns true if input was captured by control, so no other controls, nor screen can use input in this update
        public override bool HandleInput(MyGuiInput input, bool hasKeyboardActiveControl, bool hasKeyboardActiveControlPrevious, bool isThisFirstHandleInput)
        {
            bool ret = base.HandleInput(input, hasKeyboardActiveControl, hasKeyboardActiveControlPrevious, isThisFirstHandleInput);

            if (ret == false)
            {
                //float valuePrevious = m_value;

                if ((IsMouseOver() == true) && (input.IsNewLeftMousePressed() == true))
                {
                    m_controlCaptured = true;
                }

                if (input.IsLeftMouseReleased())
                {
                    m_controlCaptured = false;
                }

                if ((IsMouseOver() == true) && m_controlCaptured)
                {
                    float lineHorizontalPositionStart = GetStart();
                    float lineHorizontalPositionEnd = GetEnd();

                    SetValue(((MyGuiManager.MouseCursorPosition.X - lineHorizontalPositionStart) / (lineHorizontalPositionEnd - lineHorizontalPositionStart)) * (m_maxValue - m_minValue) + m_minValue);
                    //KeenSoftwareHouse.Library.Trace.Trace.SendMsgLastCall("t");
                    ret = true;
                }

                if (hasKeyboardActiveControl == true)
                {
                    const float MOVEMENT_DELTA_NORMALIZED = 0.001f;

                    if (input.IsKeyPress(Keys.Left) || input.IsGamepadKeyLeftPressed())
                    {
                        MoveForward(-MOVEMENT_DELTA_NORMALIZED);
                        ret = true;
                    }

                    if (input.IsKeyPress(Keys.Right) || input.IsGamepadKeyRightPressed())
                    {
                        MoveForward(+MOVEMENT_DELTA_NORMALIZED);
                        ret = true;
                    }

                    if (ret == true) m_blinkerTimer = 0;
                }
            }

            return ret;
        }

        void MoveForward(float movementDelta)
        {
            SetValue(m_value.Value + (m_maxValue - m_minValue) * movementDelta);
        }

        public override void Update()
        {
            base.Update();

            UpdateNormalizedValue();
        }

        Vector2 GetLeftTopCorner()
        {
            return m_parent.GetPositionAbsolute() + m_position - (m_size.Value / 2.0f) * m_scale;
        }

        float GetStart()
        {
            return GetLeftTopCorner().X + MyGuiConstants.SLIDER_INSIDE_OFFSET_X;
        }

        float GetEnd()
        {
            return GetLeftTopCorner().X + (m_size.Value.X - MyGuiConstants.SLIDER_INSIDE_OFFSET_X) * m_scale;
        }

        void DrawSlider(Vector4 color)
        {
            Vector2 leftTopCorner = GetLeftTopCorner();

            //  Beging and end of horizontal line
            float lineVerticalPosition = leftTopCorner.Y + m_size.Value.Y / 2.0f * m_scale;
            float lineHorizontalPositionStart = GetStart();
            float lineHorizontalPositionEnd = GetEnd();

            //  Horizontal position of silder's marker/selector
            float lineHorizontalPositionSlider = MathHelper.Lerp(lineHorizontalPositionStart, lineHorizontalPositionEnd, m_valueNormalized);

            //  Right part of horizontal line - it start at Start and ends at End
            //MyGuiManager.DrawSpriteBatch(MyGuiManager.GradientTexture, new Vector2(lineHorizontalPositionStart, lineVerticalPosition),
            //    lineHorizontalPositionEnd - lineHorizontalPositionStart, 2, GetColorAfterTransitionAlpha(tempColor * 0.7f), MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);
            //MyGuiManager.DrawSpriteBatch(MyGuiManager.GetButtonTexture(), new Vector2(lineHorizontalPositionStart, lineVerticalPosition),
               // new Vector2(lineHorizontalPositionEnd - lineHorizontalPositionStart, 0.004f * m_scale), GetColorAfterTransitionAlpha(color * 0.8f), MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);

            //  Left part of horizontal line
            //MyGuiManager.DrawSpriteBatch(MyGuiManager.GradientTexture, new Vector2(lineHorizontalPositionStart, lineVerticalPosition),
            //    lineHorizontalPositionSlider - lineHorizontalPositionStart, 4, GetColorAfterTransitionAlpha(color * 0.9f), MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);
           // MyGuiManager.DrawSpriteBatch(MyGuiManager.GetButtonTexture(), new Vector2(lineHorizontalPositionStart, lineVerticalPosition),
             //   new Vector2(lineHorizontalPositionSlider - lineHorizontalPositionStart, 0.006f * m_scale), GetColorAfterTransitionAlpha(color * 0.95f), MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);

            Vector4 startEndColor = color * 0.5f;
            startEndColor.W = 1;

            if (IsMouseOverOrKeyboardActive())
            {
                startEndColor = MyGuiConstants.DEFAULT_CONTROL_FOREGROUND_COLOR.ToVector4() * 0.8f;
            }
            else
            {
                startEndColor = MyGuiConstants.DEFAULT_CONTROL_FOREGROUND_COLOR.ToVector4() * 0.5f;
            }

            //  Small start vertical line
            //MyGuiManager.DrawSpriteBatch(MyGuiManager.GetBlankTexture(), new Vector2(lineHorizontalPositionStart, lineVerticalPosition),
            //    2, 0.02f, GetColorAfterTransitionAlpha(color * 0.7f), MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER);
            //MyGuiManager.DrawSpriteBatch(MyGuiManager.GetBlankTexture(), new Vector2(lineHorizontalPositionStart, lineVerticalPosition),
               // new Vector2(0.0025f, 0.01f) * m_scale, GetColorAfterTransitionAlpha(startEndColor), MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER);

            //  Small end vertical line
            //MyGuiManager.DrawSpriteBatch(MyGuiManager.GetBlankTexture(), new Vector2(lineHorizontalPositionEnd, lineVerticalPosition),
               // new Vector2(0.0025f, 0.01f) * m_scale, GetColorAfterTransitionAlpha(startEndColor), MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER);

            //  Draw carriage line
            //  Carriage blinker time is solved here in Draw because I want to be sure it will be drawn even in low FPS
            bool drawSlider = false;
            if (m_hasKeyboardActiveControl == true)
            {
                //  This condition controls "blinking", so most often is carrier visible and blinks very fast
                //  It also depends on FPS, but as we have max FPS set to 60, it won't go faster, nor will it omit a "blink".
                int carriageInterval = m_blinkerTimer % 20;
                if ((carriageInterval >= 0) && (carriageInterval <= 15))
                {
                    drawSlider = true;
                }
                m_blinkerTimer++;
            }
            else
            {
                drawSlider = true;
            }


            //  Moving Slider
            if (drawSlider == true)
            {
                //MyGuiManager.DrawSpriteBatch(MyGuiManager.GetBlankTexture(), new Vector2(lineHorizontalPositionSlider, lineVerticalPosition),
                //    new Vector2(0.01f, 0.0125f), GetColorAfterTransitionAlpha(color), MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER);
                MyGuiManager.DrawSpriteBatch(MyGuiManager.GetSliderTexture(), new Vector2(lineHorizontalPositionSlider, lineVerticalPosition),
                    new Vector2(0.017f, 0.0205f) * m_scale, GetColorAfterTransitionAlpha(new Vector4(color.X, color.Y, color.Z, 1)), MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER);
            }
        }

        void DrawLabel(Vector4 color)
        {
            MyMwcUtils.ClearStringBuilder(m_tempForDraw);
            m_tempForDraw.AppendFormat(m_labelText, MyValueFormatter.GetFormatedFloat(m_value.Value, LabelDecimalPlaces));

            MyGuiManager.DrawString(MyGuiManager.GetFontMinerWarsBlue(), m_tempForDraw, m_parent.GetPositionAbsolute() + m_position + new Vector2(m_size.Value.X / 2.0f + m_labelWidth, 0), m_labelScale,
                GetColorAfterTransitionAlpha(color), MyGuiDrawAlignEnum.HORISONTAL_RIGHT_AND_VERTICAL_CENTER);
        }

        public override void Draw()
        {
            base.Draw();

            Vector4 tempColor = (IsMouseOverOrKeyboardActive() == true) ? m_color * MyGuiConstants.CONTROL_MOUSE_OVER_BACKGROUND_COLOR_MULTIPLIER : m_color;
            DrawSlider(tempColor);
            DrawLabel(tempColor);
        }
    }
}
