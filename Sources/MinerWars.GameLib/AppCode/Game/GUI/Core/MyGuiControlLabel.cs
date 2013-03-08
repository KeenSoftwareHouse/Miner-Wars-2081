using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Managers;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.Utils;
using SysUtils.Utils;
using System;
using MinerWars.AppCode.Toolkit.Input;

//  Label is defined by string builder or by text enum. Only one of them at a time. It's good to use enum whenever 
//  possible, as it easily supports changing languages. Use string builder only if the text isn't and can't be defined 
//  in text resources.
//
//  If enum version is used, then text won't be stored in string builder until you use UpdateParams


namespace MinerWars.AppCode.Game.GUI.Core
{
    class MyGuiControlLabel : MyGuiControlBase
    {
        enum MyGuiControlLabelType
        {
            DEFINED_BY_STRING_BUILDER,
            DEFINED_BY_TEXT_WRAPPER_ENUM,
        }

        Vector4 m_textColor;
        float m_textScale;
        MyGuiDrawAlignEnum m_textAlign;

        MyGuiControlLabelType m_type;

        //  For DEFINED_BY_TEXT_WRAPPER_ENUM
        MyTextsWrapperEnum m_textEnum;

        public MyTextsWrapperEnum Text
        {
            get { return m_textEnum; }
            set { m_textEnum = value; }
        }
        public Vector4 TextColor
        {
            get { return m_textColor; }
            set { m_textColor  = value; }
        }

        //  For DEFINED_BY_STRING_BUILDER
        StringBuilder m_textDefinition;
        StringBuilder m_textDraw;       //  This can be also used by enum type

        public bool lastIsMouseOver;
        public EventHandler MouseLeave;
        public EventHandler MouseEnter;
        public EventHandler Click;

        public MyGuiFont Font;

        public MyGuiControlLabel(IMyGuiControlsParent parent, Vector2 position, Vector2? size, MyTextsWrapperEnum textEnum, Vector4 textColor, float textScale, MyGuiDrawAlignEnum textAlign, MyGuiFont font)
            : base(parent, position, size, null, null, false)
        {
            m_type = MyGuiControlLabelType.DEFINED_BY_TEXT_WRAPPER_ENUM;

            m_textEnum = textEnum;
            Font = font;
            Init(textColor, textScale, textAlign);
        }

        public MyGuiControlLabel(IMyGuiControlsParent parent, Vector2 position, Vector2? size, MyTextsWrapperEnum textEnum, Vector4 textColor, float textScale, MyGuiDrawAlignEnum textAlign)
            : base(parent, position, size, null, null, false)
        {
            m_type = MyGuiControlLabelType.DEFINED_BY_TEXT_WRAPPER_ENUM;

            m_textEnum = textEnum;

            Init(textColor, textScale, textAlign);
        }

        public MyGuiControlLabel(IMyGuiControlsParent parent, Vector2 position, Vector2? size, StringBuilder text, Vector4 textColor, float textScale, MyGuiDrawAlignEnum textAlign)
            : base(parent, position, size, null, null, false)
        {
            m_type = MyGuiControlLabelType.DEFINED_BY_STRING_BUILDER;

            if (text != null)
            {
                //  Create COPY of the text (Don't just point to one string builder!!! This was my original mistake!)
                m_textDefinition = new StringBuilder(text.ToString());
                m_textDraw = new StringBuilder(text.ToString());
            }

            Init(textColor, textScale, textAlign);
        }

        public MyGuiControlLabel(IMyGuiControlsParent parent, Vector2 position, Vector2? size, StringBuilder text, Vector4 textColor, float textScale, MyGuiDrawAlignEnum textAlign,MyGuiFont font)
            : base(parent, position, size, null, null, false)
        {
            m_type = MyGuiControlLabelType.DEFINED_BY_STRING_BUILDER;
            Font = font;
            if (text != null)
            {
                //  Create COPY of the text (Don't just point to one string builder!!! This was my original mistake!)
                m_textDefinition = new StringBuilder(text.ToString());
                m_textDraw = new StringBuilder(text.ToString());
            }

            Init(textColor, textScale, textAlign);
        }

        void Init(Vector4 textColor, float textScale, MyGuiDrawAlignEnum textAlign)
        {
            m_textColor = textColor;            
            m_textAlign = textAlign;
            SetTextScale(textScale);            
        }

        public void SetTextScale(float textScale) 
        {
            m_textScale = textScale;
            RecalculateSize();
        }

        //  If label's text contains params, we can update them here. Also don't forget that text is defined two time: one as a definition and one that we draw
        public void UpdateParams(params object[] args)
        {
            if (m_type == MyGuiControlLabelType.DEFINED_BY_TEXT_WRAPPER_ENUM)
            {
                if (m_textDraw == null) m_textDraw = new StringBuilder();
                MyMwcUtils.ClearStringBuilder(m_textDraw);
                m_textDraw.AppendFormat(MyTextsWrapper.Get(m_textEnum).ToString(), args);
                RecalculateSize();
            }
            else if (m_type == MyGuiControlLabelType.DEFINED_BY_STRING_BUILDER)
            {
                MyMwcUtils.ClearStringBuilder(m_textDraw);
                m_textDraw.AppendFormat(m_textDefinition.ToString(), args);
                RecalculateSize();
            }
            else
            {
                throw new MyMwcExceptionApplicationShouldNotGetHere();
            }
        }

        public override bool HandleInput(MyGuiInput input, bool hasKeyboardActiveControl, bool hasKeyboardActiveControlPrevious, bool receivedFocusInThisUpdate)
        {
            bool ret = base.HandleInput(input, hasKeyboardActiveControl, hasKeyboardActiveControlPrevious, receivedFocusInThisUpdate);

            if (ret == false)
            {
                if (MouseEnter != null|| MouseLeave != null)
                {
                    bool isMouseOver = CheckMouseOver();
                    if (isMouseOver != lastIsMouseOver)
                    {
                        if (isMouseOver)
                        {
                            if (MouseEnter != null)
                            {
                                MouseEnter(this, EventArgs.Empty);
                            }
                        }
                        else
                        {
                            if (MouseLeave != null)
                            {
                                MouseLeave(this, EventArgs.Empty);
                            }
                        }
                    }
                    lastIsMouseOver = isMouseOver;
                }

                if (Click != null)
                {
                    if (((CheckMouseOver() == true) && input.IsNewLeftMousePressed()) ||
                        ((hasKeyboardActiveControl == true) && ((input.IsNewKeyPress(Keys.Enter) || (input.IsNewKeyPress(Keys.Space)) || (input.IsNewGameControlPressed(MyGameControlEnums.FIRE_PRIMARY) && !input.IsNewLeftMousePressed())))))
                    {
                        Click(this, EventArgs.Empty);
                    }
                }
            }

            return ret;
        }

        void RecalculateSize()
        {            
            m_size = GetTextSize().Size;
        }

        void DrawText(StringBuilder sb)
        {
            Vector4 textColor = m_textColor;
            if (!Enabled)
                textColor = m_textColor * MyGuiConstants.DISABLED_BUTTON_COLOR_VECTOR;
            if (Font==null) MyGuiManager.DrawString(MyGuiManager.GetFontMinerWarsBlue(), sb, m_parent.GetPositionAbsolute() + m_position, m_textScale, GetColorAfterTransitionAlpha(textColor), m_textAlign);
            else MyGuiManager.DrawString(Font, sb, m_parent.GetPositionAbsolute() + m_position, m_textScale, GetColorAfterTransitionAlpha(textColor), m_textAlign);
        }

        public override void Draw()
        {
            base.Draw();

            if (m_textDraw != null)
            {
                //  This means that text was changed through UpdateParams
                DrawText(m_textDraw);                
            }
            else
            {
                DrawText(MyTextsWrapper.Get(m_textEnum));                
            }
        }

        public void UpdateText(string text)
        {
            //m_textDefinition = new StringBuilder(text);
            //m_textDraw = new StringBuilder(text);
            UpdateText(new StringBuilder(text));
        }

        public void UpdateText(StringBuilder builder)
        {
            m_textDefinition = builder;
            m_textDraw = builder;
            RecalculateSize();
        }


        public StringBuilder GetText() 
        {
            return m_textDefinition;
        }

        public MyRectangle2D GetTextSize()
        {
            if (m_textDraw != null)
            {
                //  This means that text was changed through UpdateParams
                return MyGuiManager.MeasureString(MyGuiManager.GetFontMinerWarsBlue(), m_textDraw, m_parent.GetPositionAbsolute() + m_position, m_textScale, m_textAlign);
            }
            else
            {
                return MyGuiManager.MeasureString(MyGuiManager.GetFontMinerWarsBlue(), MyTextsWrapper.Get(m_textEnum), m_parent.GetPositionAbsolute() + m_position, m_textScale, m_textAlign);
            }
        }
    }
}
