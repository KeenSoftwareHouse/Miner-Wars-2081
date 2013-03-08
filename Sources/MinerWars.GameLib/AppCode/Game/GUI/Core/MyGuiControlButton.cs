using System.Text;
using MinerWarsMath;
using MinerWarsMath.Graphics;
using MinerWars.AppCode.Toolkit.Input;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Audio;
using MinerWars.AppCode.App;
using SysUtils.Utils;
using MinerWars.AppCode.Game.Textures;

namespace MinerWars.AppCode.Game.GUI.Core
{
    enum MyGuiControlButtonTextAlignment
    {
        CENTERED,       //  Text is in the button's center
        LEFT            //  Text is moved to the left side
    }

    class MyGuiControlButton : MyGuiControlBase
    {        
        public delegate void OnButtonClick(MyGuiControlButton sender);
        public delegate void OnIndexedButtonClick(int buttonIndex);

        enum ScaleAnimationState
        {
            None,
            ScaleIn,
            ScaleOut,
            Scaled
        }

        private StringBuilder m_text;
        public Vector2 TextOffset;
        Vector4 m_textColor;
        float m_textScale = 1.0f;
        OnButtonClick m_onButtonClick;
        OnIndexedButtonClick m_onIndexedButtonClick;
        int m_buttonIndex;
        MyGuiControlButtonTextAlignment m_textAlignment;
        bool m_canHandleKeyboardInput;
        bool m_implementedFeature;
        MyTextsWrapperEnum? m_accessForbiddenReason;

        public MyTextsWrapperEnum? AccessForbiddenReason
        {
            get { return m_accessForbiddenReason; }
            set { m_accessForbiddenReason = value; }
        }

        //MyTexture2D m_hoverButtonTexture;
        MyTexture2D m_pressedButtonTexture;
        //MyTexture2D m_shadowTexture;

        ScaleAnimationState m_ScaleAnimationState = ScaleAnimationState.None;
        int m_ScaleAnimationCurrentTime = 0;
        int m_LastTime;

        private bool m_useBackground = true;
        public bool UseBorderBackground
        {
            get { return m_useBackground; }
            set { m_useBackground = value; }
        }

        public bool DrawCrossTextureWhenDisabled = true;

        private bool m_drawRedTextureWhenDisabled = true;
        public bool DrawRedTextureWhenDisabled
        {
            get { return m_drawRedTextureWhenDisabled; }
            set
            {
                if (value)
                {
                    m_drawRedTextureWhenDisabled = true;
                }
                else
                {
                    m_drawRedTextureWhenDisabled = false;
                    DrawCrossTextureWhenDisabled = false;
                }
            }
        }


        // if using 'Switch Mode' then
        // 1) highlight doesn't work
        // 2) disabled button looks like enabled and opposite
        // 3) disabled button can be clicked
        public bool UseSwitchMode = false;

        

        public MyGuiControlButton(IMyGuiControlsParent parent, Vector2 position, Vector2? size, Vector4 backgroundColor, MyTextsWrapperEnum textEnum,
            Vector4 textColor, float textScale, OnButtonClick onButtonClick, MyGuiControlButtonTextAlignment textAlignment, bool canHandleKeyboardInput,
            MyGuiDrawAlignEnum align, bool implementedFeature, MyTextsWrapperEnum? accessForbiddenReason)
            : this(parent, position, size, backgroundColor, MyTextsWrapper.Get(textEnum), null, textColor, textScale, onButtonClick, textAlignment,
            canHandleKeyboardInput, align, implementedFeature)
        {
            m_accessForbiddenReason = accessForbiddenReason;
            m_canHandleKeyboardActiveControl &= m_accessForbiddenReason == null;
        }

        // with delegate for indexed onButtonClick
        public MyGuiControlButton(IMyGuiControlsParent parent, Vector2 position, Vector2? size, Vector4 backgroundColor, MyTextsWrapperEnum textEnum,
            Vector4 textColor, float textScale, OnIndexedButtonClick onIndexedButtonClick, int buttonIndex, MyGuiControlButtonTextAlignment textAlignement, bool canHandleKeyboardInput,
            MyGuiDrawAlignEnum align, bool implementedFeature, MyTextsWrapperEnum? accessForbiddenReason)
            : this(parent, position, size, backgroundColor, MyTextsWrapper.Get(textEnum), textColor, textScale, onIndexedButtonClick, buttonIndex,
            textAlignement, canHandleKeyboardInput, align, null, implementedFeature)
        {
            m_accessForbiddenReason = accessForbiddenReason;
            m_canHandleKeyboardActiveControl &= m_accessForbiddenReason == null;
        }

        public MyGuiControlButton(IMyGuiControlsParent parent, Vector2 position, Vector2? size, Vector4 backgroundColor, MyTextsWrapperEnum textEnum,
            Vector4 textColor, float textScale, OnButtonClick onButtonClick, MyGuiControlButtonTextAlignment textAlignement, bool canHandleKeyboardInput,
            MyGuiDrawAlignEnum align, bool implementedFeature)
            : this(parent, position, size, backgroundColor, MyTextsWrapper.Get(textEnum), null, textColor, textScale, onButtonClick, textAlignement,
            canHandleKeyboardInput, align, implementedFeature)
        {
        }

        public MyGuiControlButton(IMyGuiControlsParent parent, Vector2 position, Vector2? size, Vector4 backgroundColor, MyTextsWrapperEnum textEnum, MyTextsWrapperEnum tooltip,
            Vector4 textColor, float textScale, OnButtonClick onButtonClick, MyGuiControlButtonTextAlignment textAlignement, bool canHandleKeyboardInput,
            MyGuiDrawAlignEnum align, bool implementedFeature)
            : this(parent, position, size, backgroundColor, MyTextsWrapper.Get(textEnum), MyTextsWrapper.Get(tooltip), textColor, textScale, onButtonClick, textAlignement,
            canHandleKeyboardInput, align, implementedFeature)
        {
        }

        // with delegate for indexed onButtonClick
        public MyGuiControlButton(IMyGuiControlsParent parent, Vector2 position, Vector2? size, Vector4 backgroundColor, MyTextsWrapperEnum textEnum,
            Vector4 textColor, float textScale, OnIndexedButtonClick onIndexedButtonClick, int buttonIndex, MyGuiControlButtonTextAlignment textAlignment, bool canHandleKeyboardInput,
            MyGuiDrawAlignEnum align, bool implementedFeature)
            : this(parent, position, size, backgroundColor, MyTextsWrapper.Get(textEnum), textColor, textScale, onIndexedButtonClick, buttonIndex,
            textAlignment, canHandleKeyboardInput, align, null, implementedFeature)
        {
        }

        //  Text constructor
        public MyGuiControlButton(IMyGuiControlsParent parent, Vector2 position, Vector2? size, Vector4 backgroundColor, StringBuilder text, StringBuilder tooltip,
            Vector4 textColor, float textScale, OnButtonClick onButtonClick, MyGuiControlButtonTextAlignment textAlignment, bool canHandleKeyboardInput,
            MyGuiDrawAlignEnum align, bool implementedFeature)
            : this(parent, position, size, backgroundColor, null, null, null, onButtonClick, canHandleKeyboardInput, align, tooltip, implementedFeature, true, MyGuiControlHighlightType.WHEN_ACTIVE, null, null/*, null, null*/)
        {
            m_textColor = textColor;
            Text = text;
            m_textScale = textScale;
            m_textAlignment = textAlignment;
        }

        //  Text constructor with delegate for indexed onButtonClick
        public MyGuiControlButton(IMyGuiControlsParent parent, Vector2 position, Vector2? size, Vector4 backgroundColor, StringBuilder text,
            Vector4 textColor, float textScale, OnIndexedButtonClick onIndexedButtonClick, int buttonIndex, MyGuiControlButtonTextAlignment textAlignement, bool canHandleKeyboardInput,
            MyGuiDrawAlignEnum align, StringBuilder tooltip, bool implementedFeature)
            : this(parent, position, size, backgroundColor, onIndexedButtonClick, buttonIndex, canHandleKeyboardInput, align, tooltip, implementedFeature, true)
        {
            m_textColor = textColor;
            Text = text;
            m_textScale = textScale;
            m_textAlignment = textAlignement;
        }

        public MyGuiControlButton(IMyGuiControlsParent parent, Vector2 position, Vector2? size, Vector4 backgroundColor, MyTexture2D buttonTexture, MyTexture2D hoverButtonTexture, MyTexture2D pressedButtonTexture,
            MyTextsWrapperEnum textEnum, Vector4 textColor, float textScale, MyGuiControlButtonTextAlignment textAlignment,
            OnButtonClick onButtonClick, bool canHandleKeyboardInput, MyGuiDrawAlignEnum align, bool implementedFeature, bool canHandleKeyboardActiveControl)
            : this(parent, position, size, backgroundColor, buttonTexture, hoverButtonTexture, pressedButtonTexture, textEnum, textColor, textScale, textAlignment, onButtonClick,
            canHandleKeyboardInput, align, implementedFeature, canHandleKeyboardActiveControl,MyGuiControlHighlightType.WHEN_ACTIVE)
        {
            
        }
        
        public MyGuiControlButton(IMyGuiControlsParent parent, Vector2 position, Vector2? size, Vector4 backgroundColor, MyTexture2D buttonTexture, MyTexture2D hoverButtonTexture, MyTexture2D pressedButtonTexture,
    MyTextsWrapperEnum textEnum, Vector4 textColor, float textScale, MyGuiControlButtonTextAlignment textAlignment,
    OnButtonClick onButtonClick, bool canHandleKeyboardInput, MyGuiDrawAlignEnum align, bool implementedFeature, bool canHandleKeyboardActiveControl, bool useBackground, bool drawCrossTextureWhenDisabled = true)
            : this(parent, position, size, backgroundColor, buttonTexture, hoverButtonTexture, pressedButtonTexture, textEnum, textColor, textScale, textAlignment, onButtonClick,
            canHandleKeyboardInput, align, implementedFeature, canHandleKeyboardActiveControl, MyGuiControlHighlightType.WHEN_ACTIVE)
        {
            m_useBackground = useBackground;       
        }



        // Image without text constructor
        public MyGuiControlButton(IMyGuiControlsParent parent, Vector2 position, Vector2? size, Vector4 backgroundColor, 
            MyTexture2D buttonTexture, MyTexture2D hoverButtonTexture, MyTexture2D pressedButtonTexture, StringBuilder tooltip,
            OnButtonClick onButtonClick, bool canHandleKeyboardInput, MyGuiDrawAlignEnum align, bool implementedFeature, bool canHandleKeyboardActiveControl, MyGuiControlHighlightType highlightType)
            : this(parent, position,
            size, backgroundColor, buttonTexture, hoverButtonTexture, pressedButtonTexture, tooltip, onButtonClick, canHandleKeyboardInput, align, implementedFeature, 
            canHandleKeyboardActiveControl, highlightType, null, null/*, null, null*/)
        {
            
        }

        // Image without text constructor and with custom mouse cursor textures
        public MyGuiControlButton(IMyGuiControlsParent parent, Vector2 position, Vector2? size, Vector4 backgroundColor,
            MyTexture2D buttonTexture, MyTexture2D hoverButtonTexture, MyTexture2D pressedButtonTexture, StringBuilder tooltip,
            OnButtonClick onButtonClick, bool canHandleKeyboardInput, MyGuiDrawAlignEnum align, bool implementedFeature, bool canHandleKeyboardActiveControl, 
            MyGuiControlHighlightType highlightType, MyTexture2D mouseCursorHoverTexture, MyTexture2D mouseCursorPressedTexture/*,
            System.Drawing.Bitmap mouseCursorHoverBitmap, System.Drawing.Bitmap mouseCursorPressedBitmap*/)
            : this(parent, position, size, backgroundColor, buttonTexture, hoverButtonTexture, pressedButtonTexture, onButtonClick, canHandleKeyboardInput, align, tooltip, implementedFeature,
            canHandleKeyboardActiveControl, highlightType, mouseCursorHoverTexture, mouseCursorPressedTexture/*, mouseCursorHoverBitmap, mouseCursorPressedBitmap*/)
        {

        }

        //  Image constructor
        public MyGuiControlButton(IMyGuiControlsParent parent, Vector2 position, Vector2? size, Vector4 backgroundColor, MyTexture2D buttonTexture, MyTexture2D hoverButtonTexture, MyTexture2D pressedButtonTexture, 
            MyTextsWrapperEnum textEnum, Vector4 textColor, float textScale, MyGuiControlButtonTextAlignment textAlignment,
            OnButtonClick onButtonClick, bool canHandleKeyboardInput, MyGuiDrawAlignEnum align, bool implementedFeature, bool canHandleKeyboardActiveControl, MyGuiControlHighlightType highlightType)
            : this(parent, position, size, backgroundColor, buttonTexture, hoverButtonTexture, pressedButtonTexture, onButtonClick, canHandleKeyboardInput, align, null, implementedFeature, canHandleKeyboardActiveControl, highlightType, null, null/*, null, null*/)
        {
            Text = MyTextsWrapper.Get(textEnum);
            m_textColor = textColor;
            m_textAlignment = textAlignment;
            m_textScale = textScale;
        }        

        //  Base constructor
        private MyGuiControlButton(IMyGuiControlsParent parent, Vector2 position, Vector2? size, Vector4 backgroundColor, MyTexture2D buttonTexture, MyTexture2D hoverButtonTexture, MyTexture2D pressedButtonTexture,
            OnButtonClick onButtonClick, bool canHandleKeyboardInput, MyGuiDrawAlignEnum align, StringBuilder tooltip, bool implementedFeature, bool canHandleKeyboardActiveControl, MyGuiControlHighlightType highlightType,
            MyTexture2D mouseCursorHoverTexture, MyTexture2D mouseCursorPressedTexture/*,
            System.Drawing.Bitmap mouseCursorHoverBitmap, System.Drawing.Bitmap mouseCursorPressedBitmap*/)
            : base(parent, MyGuiManager.GetAlignedCoordinate(position, size.Value, align) + new Vector2(size.Value.X / 2.0f, size.Value.Y / 2.0f), size, backgroundColor, tooltip,
             buttonTexture, hoverButtonTexture, pressedButtonTexture, true, highlightType, mouseCursorHoverTexture, mouseCursorPressedTexture/*, mouseCursorHoverBitmap, mouseCursorPressedBitmap*/)
        {
            m_canHandleKeyboardActiveControl = canHandleKeyboardActiveControl && implementedFeature;
            m_onButtonClick = onButtonClick;
            m_canHandleKeyboardInput = canHandleKeyboardInput && implementedFeature;
            m_implementedFeature = implementedFeature;
        }

        //  Base constructor with delegate for indexed onButtonClick
        private MyGuiControlButton(IMyGuiControlsParent parent, Vector2 position, Vector2? size, Vector4 backgroundColor,
             MyTexture2D buttonTexture, MyTexture2D hoverButtonTexture, MyTexture2D pressedButtonTexture,
            OnIndexedButtonClick onIndexedButtonClick, int buttonIndex, bool canHandleKeyboardInput, MyGuiDrawAlignEnum align, bool implementedFeature, bool canHandleKeyboardActiveControl)
            : base(parent, MyGuiManager.GetAlignedCoordinate(position, size.Value, align) + new Vector2(size.Value.X / 2.0f, size.Value.Y / 2.0f), size, backgroundColor, null,
             buttonTexture, hoverButtonTexture, pressedButtonTexture, true)
        {
            m_canHandleKeyboardActiveControl = canHandleKeyboardActiveControl;
            m_onIndexedButtonClick = onIndexedButtonClick;
            m_buttonIndex = buttonIndex;
            m_canHandleKeyboardInput = canHandleKeyboardInput;
            m_implementedFeature = implementedFeature;
        }

        //  Base constructor with delegate for indexed onButtonClick and tooltip
        private MyGuiControlButton(IMyGuiControlsParent parent, Vector2 position, Vector2? size, Vector4 backgroundColor,
            OnIndexedButtonClick onIndexedButtonClick, int buttonIndex, bool canHandleKeyboardInput, MyGuiDrawAlignEnum align, StringBuilder toolTip, bool implementedFeature, bool canHandleKeyboardActiveControl)
            : base(parent, MyGuiManager.GetAlignedCoordinate(position, size.Value, align) + new Vector2(size.Value.X / 2.0f, size.Value.Y / 2.0f), size, backgroundColor, toolTip)
        {
            m_canHandleKeyboardActiveControl = canHandleKeyboardActiveControl;
            m_onIndexedButtonClick = onIndexedButtonClick;
            m_buttonIndex = buttonIndex;
            m_canHandleKeyboardInput = canHandleKeyboardInput;
            m_implementedFeature = implementedFeature;
        }

        public StringBuilder Text
        {
            get { return m_text; }
            set { m_text = value; }
        }




        //  Method returns true if input was captured by control, so no other controls, nor screen can use input in this update
        public override bool HandleInput(MyGuiInput input, bool hasKeyboardActiveControl, bool hasKeyboardActiveControlPrevious, bool receivedFocusInThisUpdate)
        {
            bool captureInput = base.HandleInput(input, hasKeyboardActiveControl, hasKeyboardActiveControlPrevious, receivedFocusInThisUpdate);

            if (captureInput == false)
            {
                if (((IsMouseOver() == true) && ((input.IsNewLeftMouseReleased() == true))) ||
                    ((hasKeyboardActiveControl == true) && (m_canHandleKeyboardInput == true) && ((input.IsJoystickButtonNewPressed(MyJoystickButtonsEnum.J01)) || (input.IsNewKeyPress(Keys.Enter)) || (input.IsNewKeyPress(Keys.Space)))))
                {
                    if (m_implementedFeature == false)
                    {
                        MyAudio.AddCue2D(MySoundCuesEnum.GuiMouseClick);
                        MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, MyTextsWrapperEnum.FeatureNotYetImplemented,
                            MyTextsWrapperEnum.MessageBoxCaptionFeatureDisabled, MyTextsWrapperEnum.Ok, null));
                    }
                    else if (m_accessForbiddenReason != null)
                    {
                        MyAudio.AddCue2D(MySoundCuesEnum.GuiMouseClick);
                        MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, m_accessForbiddenReason.Value,
                            MyTextsWrapperEnum.MessageBoxCaptionFeatureDisabled, MyTextsWrapperEnum.Ok, null));
                    }
                    else if (Enabled || UseSwitchMode)
                    {
                        MyAudio.AddCue2D(MySoundCuesEnum.GuiMouseClick);
                        if (m_onButtonClick != null)
                        {
                            m_onButtonClick(this);
                        }
                        else if (m_onIndexedButtonClick != null)
                        {
                            m_onIndexedButtonClick(m_buttonIndex);
                        }
                    }

                    captureInput = true;
                }
            }

            return captureInput;
        }

        public override void Update()
        {
            base.Update();

            if (IsMouseOverOrKeyboardActive())
            {
                if (m_ScaleAnimationState == ScaleAnimationState.None) 
                {
                    m_ScaleAnimationCurrentTime = 0;
                    m_ScaleAnimationState = ScaleAnimationState.ScaleIn;
                }

                if (m_ScaleAnimationState == ScaleAnimationState.ScaleOut)
                {
                    m_ScaleAnimationCurrentTime = MyGuiConstants.BUTTON_HOVER_SCALE_TIME - m_ScaleAnimationCurrentTime;
                    m_ScaleAnimationState = ScaleAnimationState.ScaleIn;
                }
            }
            else
            {
                if (m_ScaleAnimationState == ScaleAnimationState.ScaleIn)
                {
                    m_ScaleAnimationCurrentTime = MyGuiConstants.BUTTON_HOVER_SCALE_TIME - m_ScaleAnimationCurrentTime;
                    m_ScaleAnimationState = ScaleAnimationState.ScaleOut;
                }

                if (m_ScaleAnimationState == ScaleAnimationState.Scaled)
                {
                    m_ScaleAnimationCurrentTime = MyGuiConstants.BUTTON_HOVER_SCALE_TIME;
                    m_ScaleAnimationState = ScaleAnimationState.ScaleOut;
                }
            }
    
            int timeDelta = MyMinerGame.TotalGamePlayTimeInMilliseconds - m_LastTime;

            switch (m_ScaleAnimationState)
            {
                case ScaleAnimationState.ScaleIn:
                    m_ScaleAnimationCurrentTime += timeDelta;
                    if (m_ScaleAnimationCurrentTime >= MyGuiConstants.BUTTON_HOVER_SCALE_TIME)
                    {
                        m_ScaleAnimationCurrentTime = MyGuiConstants.BUTTON_HOVER_SCALE_TIME;
                        m_ScaleAnimationState = ScaleAnimationState.Scaled;
                    }

                    m_scale = 1.0f + (MyGuiConstants.BUTTON_HOVER_SCALE - 1.0f) * m_ScaleAnimationCurrentTime / MyGuiConstants.BUTTON_HOVER_SCALE_TIME;

                    break;

                case ScaleAnimationState.Scaled:
                    m_scale = MyGuiConstants.BUTTON_HOVER_SCALE;

                    if (m_mouseButtonPressed)
                    {
                        m_scale *= MyGuiConstants.BUTTON_PRESSED_SCALE;
                    }
            
                    break;

                case ScaleAnimationState.ScaleOut:
                    m_ScaleAnimationCurrentTime -= timeDelta;
                    if (m_ScaleAnimationCurrentTime <= 0)
                    {
                        m_ScaleAnimationCurrentTime = 0;
                        m_ScaleAnimationState = ScaleAnimationState.None;
                    }

                    m_scale = 1.0f + (MyGuiConstants.BUTTON_HOVER_SCALE - 1.0f) * m_ScaleAnimationCurrentTime / MyGuiConstants.BUTTON_HOVER_SCALE_TIME;

                    break;


                default:
                    break;
            }


            m_LastTime = MyMinerGame.TotalGamePlayTimeInMilliseconds;
        }


        public override void Draw()
        {            
            MyTexture2D buttonTexture = null;
            MyTexture2D backgroundTexture = null;

            if (m_controlTexture == null)
            {
                if (m_size.HasValue && m_size.Value.Y < MyGuiConstants.MAIN_MENU_BUTTON_SIZE.Y)
                {
                    buttonTexture = MyGuiManager.GetConfirmButton();
                }
                else
	            {
                    buttonTexture = MyGuiManager.GetButtonTexture();
	            }
                
            }
            else
	        {
                if (IsMouseOver() && m_mouseButtonPressed && m_pressedTexture != null)
                {
                    buttonTexture = m_pressedTexture;
                }
                else if (IsMouseOver() && m_hoverTexture != null)
                {
                    buttonTexture = m_hoverTexture;
                }
                else
                {
                    buttonTexture = m_controlTexture;
                }
	        }

            backgroundTexture = MyGuiManager.GetButtonTextureBg(buttonTexture);

            bool isNotImplementedForbidenOrDisabled = !m_implementedFeature || m_accessForbiddenReason != null || !Enabled;
            Vector4 backgroundColor, textColor;
            if (!UseSwitchMode)
            {
                bool isHighlighted = IsMouseOverOrKeyboardActive() &&
                    (m_highlightType == MyGuiControlHighlightType.WHEN_ACTIVE || (m_highlightType == MyGuiControlHighlightType.WHEN_CURSOR_OVER && IsMouseOver()));

                backgroundColor = isNotImplementedForbidenOrDisabled ?
                    (DrawRedTextureWhenDisabled ? MyGuiConstants.DISABLED_BUTTON_COLOR_VECTOR : m_backgroundColor.Value * MyGuiConstants.DISABLED_BUTTON_NON_RED_MULTIPLIER) :
                    (isHighlighted ? m_backgroundColor.Value * MyGuiConstants.CONTROL_MOUSE_OVER_BACKGROUND_COLOR_MULTIPLIER : m_backgroundColor.Value);

                textColor = isNotImplementedForbidenOrDisabled ?
                    (DrawRedTextureWhenDisabled ? MyGuiConstants.DISABLED_BUTTON_TEXT_COLOR : m_textColor * MyGuiConstants.DISABLED_BUTTON_NON_RED_MULTIPLIER) :
                    (isHighlighted ? m_textColor * MyGuiConstants.CONTROL_MOUSE_OVER_BACKGROUND_COLOR_MULTIPLIER : m_textColor);
            }
            else
            {
                backgroundColor = isNotImplementedForbidenOrDisabled ?
                    (DrawRedTextureWhenDisabled ? MyGuiConstants.DISABLED_BUTTON_COLOR_VECTOR : m_backgroundColor.Value * MyGuiConstants.DISABLED_BUTTON_NON_RED_MULTIPLIER * 2) :
                    (m_backgroundColor.Value * 0.75f);

                textColor = isNotImplementedForbidenOrDisabled ?
                    (DrawRedTextureWhenDisabled ? MyGuiConstants.DISABLED_BUTTON_TEXT_COLOR : m_textColor * MyGuiConstants.DISABLED_BUTTON_NON_RED_MULTIPLIER * 2) :
                    (m_textColor * 0.75f);
            }

            if (backgroundTexture!=null && m_useBackground)
            {
                // Draw background texture
                MyGuiManager.DrawSpriteBatch(backgroundTexture, m_parent.GetPositionAbsolute() + m_position, m_size.Value * m_scale,
                    Color.White, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER);
            }

            // Draw background texture
            if (buttonTexture != null)
            {
                MyGuiManager.DrawSpriteBatch(buttonTexture, m_parent.GetPositionAbsolute() + m_position, m_size.Value * m_scale,
                    GetColorAfterTransitionAlpha(backgroundColor), MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER);
            }
            
            // Draw cross texture 
            if (isNotImplementedForbidenOrDisabled && DrawCrossTextureWhenDisabled)
            {
                MyGuiManager.DrawSpriteBatch(MyGuiManager.GetLockedButtonTexture(), m_parent.GetPositionAbsolute() + m_position, m_size.Value * MyGuiConstants.LOCKBUTTON_SIZE_MODIFICATION,
                    MyGuiConstants.DISABLED_BUTTON_COLOR, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER);
            }

            if (Text != null)
            {
                Vector2 textPosition;
                MyGuiDrawAlignEnum textDrawAlign;
                if (m_textAlignment == MyGuiControlButtonTextAlignment.CENTERED)
                {
                    textPosition = m_parent.GetPositionAbsolute() + m_position;
                    textDrawAlign = MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER;
                }
                else if (m_textAlignment == MyGuiControlButtonTextAlignment.LEFT)
                {
                    //  This will move text few pixels from button's left border
                    textPosition = m_parent.GetPositionAbsolute() + m_position - new Vector2(m_size.Value.X / 2.0f, 0) + new Vector2(MyGuiConstants.BUTTON_TEXT_OFFSET.X, 0);
                    textDrawAlign = MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER;
                }
                else
                {
                    throw new MyMwcExceptionApplicationShouldNotGetHere();
                }

                textPosition += TextOffset;

                MyGuiManager.DrawString(MyGuiManager.GetFontMinerWarsBlue(), Text, textPosition, m_textScale, GetColorAfterTransitionAlpha(textColor), textDrawAlign);
            }

            //ShowToolTip();
        }

        public void SetTextEnum(MyTextsWrapperEnum textEnum)
        {
            Text = MyTextsWrapper.Get(textEnum);
        }

        public void SetText(StringBuilder text)
        {
            Text = text;
        }

        public void SetTextures(MyTexture2D buttonTexture, MyTexture2D hoverButtonTexture, MyTexture2D pressedButtonTexture)
        {
            m_controlTexture = buttonTexture;
            m_hoverTexture = hoverButtonTexture;
            m_pressedButtonTexture = pressedButtonTexture;
        }        

        public void SetSize(Vector2 size)
        {
            m_size = size;
        }

        public void SetHightlightType(MyGuiControlHighlightType hightlightType)
        {
            m_highlightType = hightlightType;
        }

        public void SetToolTip(StringBuilder tooltip)
        {
            m_toolTip = new MyToolTips(tooltip);
        }

        internal void SetTextScale(float textScale)
        {
            m_textScale = textScale;
        }
    }
}