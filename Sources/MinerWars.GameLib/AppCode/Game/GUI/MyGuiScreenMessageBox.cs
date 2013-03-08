using System;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWarsMath.Graphics;
using SysUtils.Utils;
using MinerWars.AppCode.Game.Textures;

namespace MinerWars.AppCode.Game.GUI
{
    enum MyGuiScreenMessageBoxCallbackEnum
    {
        YES,        //  YES or OK
        NO          //  NO or CANCEL or ESC
    }

    enum MyMessageBoxType 
    {
        MESSAGE,        
        ERROR,
        NULL
    }

    //  Type of message box, that means what buttons do we display (only OK, YES and NO, or something else)
    enum MyMessageBoxButtonsType
    {
        NONE,                   //  No buttons
        OK,                     //  Just OK button
        YES_NO,                 //  YES and NO buttons
        YES_NO_TIMEOUT,         //  YES and NO buttons; And Timeout so if no pressed YES in selected time, message box ends as if NO was pressed        
    }

    class MyGuiScreenMessageBox : MyGuiScreenBase
    {
        class MyMessageBoxConfiguration 
        {
            public MyTexture2D Texture { get; set; }
            public Vector4 TextColor { get; set; }
            public Vector4 BackgroundColor { get; set; }
            public Vector4 ButtonColor { get; set; }
            public Vector4 RotatingWheelColor { get; set; }
            public Vector4 InterferenceVideoColor { get; set; }
            public MyGuiFont Font { get; set; }
            public MyTexture2D ButtonTexture { get; set; }

            public MyMessageBoxConfiguration(MyTexture2D texture, Vector4 textColor, Vector4 backgroundColor, Vector4 buttonColor, Vector4 rotatingWheelColor, Vector4 interferenceVideoColor, MyGuiFont font, MyTexture2D buttonTexture ) 
            {
                Texture = texture;
                TextColor = textColor;
                BackgroundColor = backgroundColor;
                ButtonColor = buttonColor;
                RotatingWheelColor = rotatingWheelColor;
                InterferenceVideoColor = interferenceVideoColor;
                Font = font;
                ButtonTexture = buttonTexture;
            }
        }

        static readonly MyMessageBoxConfiguration[] m_typesConfiguration;

        static MyGuiScreenMessageBox() 
        {
            m_typesConfiguration = new MyMessageBoxConfiguration[MyMwcUtils.GetMaxValueFromEnum<MyMessageBoxType>() + 1];
            m_typesConfiguration[(int)MyMessageBoxType.MESSAGE] = new MyMessageBoxConfiguration(
                MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\MessageBackground_blue", flags: TextureFlags.IgnoreQuality),
                MyGuiConstants.MESSAGE_BOX_MESSAGE_TEXT_COLOR,
                MyGuiConstants.MESSAGE_BOX_MESSAGE_BACKGROUND_COLOR,
                MyGuiConstants.MESSAGE_BOX_MESSAGE_BUTTON_BACKGROUND_COLOR,
                MyGuiConstants.MESSAGE_BOX_MESSAGE_ROTATING_WHEEL_COLOR,
                MyGuiConstants.MESSAGE_BOX_MESSAGE_BACKGROUND_INTERFERENCE_VIDEO_COLOR,
                MyGuiManager.GetFontMinerWarsBlue(),
                MyGuiManager.GetConfirmButton());
            m_typesConfiguration[(int)MyMessageBoxType.ERROR] = new MyMessageBoxConfiguration(
                MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\MessageBackground_red", flags: TextureFlags.IgnoreQuality),
                MyGuiConstants.MESSAGE_BOX_ERROR_TEXT_COLOR,
                MyGuiConstants.MESSAGE_BOX_ERROR_BACKGROUND_COLOR,
                MyGuiConstants.MESSAGE_BOX_ERROR_BUTTON_BACKGROUND_COLOR,
                MyGuiConstants.MESSAGE_BOX_ERROR_ROTATING_WHEEL_COLOR,
                MyGuiConstants.MESSAGE_BOX_ERROR_BACKGROUND_INTERFERENCE_VIDEO_COLOR,
                MyGuiManager.GetFontMinerWarsWhite(),
                MyGuiManager.GetMessageBoxButton());
            m_typesConfiguration[(int)MyMessageBoxType.NULL] = new MyMessageBoxConfiguration(
                MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\MessageBackground_blue", flags: TextureFlags.IgnoreQuality),
                MyGuiConstants.MESSAGE_BOX_NULL_TEXT_COLOR,
                MyGuiConstants.MESSAGE_BOX_NULL_BACKGROUND_COLOR,
                MyGuiConstants.MESSAGE_BOX_NULL_BUTTON_BACKGROUND_COLOR,
                MyGuiConstants.MESSAGE_BOX_NULL_ROTATING_WHEEL_COLOR,
                MyGuiConstants.MESSAGE_BOX_NULL_BACKGROUND_INTERFERENCE_VIDEO_COLOR,
                MyGuiManager.GetFontMinerWarsBlue(),
                MyGuiManager.GetConfirmButton());

        }

        public delegate void MessageBoxCallback(MyGuiScreenMessageBoxCallbackEnum callbackReturn);
        public bool CloseBeforeCallback { get; set; }
        public bool InstantClose { get; set; }

        MyTextsWrapperEnum? m_yesButtonText;
        MyTextsWrapperEnum? m_noButtonText;
        MyTextsWrapperEnum? m_okButtonText;
        MessageBoxCallback m_callback;
        MyMessageBoxButtonsType m_buttonType;
        MyMessageBoxType m_type;
        int m_timeoutInMiliseconds;
        int m_timeoutStartedTimeInMiliseconds;
        MyGuiControlLabel m_messageBoxText;
        MyGuiControlCheckbox m_showAgainCheckBox;

        Vector2 m_buttonSize;
        Vector4 m_textColor;
        protected Vector4 m_interferenceVideoColor;

        //  Constructor for no buttons message boxes
        public MyGuiScreenMessageBox(MyMessageBoxType type, MyTextsWrapperEnum messageText, MyTextsWrapperEnum messageCaption, MessageBoxCallback callback) :
            this(type, MyMessageBoxButtonsType.NONE, MyTextsWrapper.Get(messageText), MyTextsWrapper.Get(messageCaption), null, null, null, callback, false, MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE)
        {
        }

        //  Constructor for OK message boxes
        public MyGuiScreenMessageBox(MyMessageBoxType type, MyTextsWrapperEnum messageText, MyTextsWrapperEnum messageCaption, MyTextsWrapperEnum? okButtonText, MessageBoxCallback callback) :
            this(type, MyMessageBoxButtonsType.OK, MyTextsWrapper.Get(messageText), MyTextsWrapper.Get(messageCaption), okButtonText, null, null, callback, false)
        {
        }

        //  Constructor for OK message boxes - and text and caption defined by StringBuilder instead of enum
        public MyGuiScreenMessageBox(MyMessageBoxType type, StringBuilder messageText, StringBuilder messageCaption, MyTextsWrapperEnum? okButtonText, MessageBoxCallback callback) :
            this(type, MyMessageBoxButtonsType.OK, messageText, messageCaption, okButtonText, null, null, callback, false)
        {
        }

        //  Constructor for YES/NO message boxes
        public MyGuiScreenMessageBox(MyMessageBoxType type, MyTextsWrapperEnum messageText, MyTextsWrapperEnum messageCaption, MyTextsWrapperEnum? yesButtonText, MyTextsWrapperEnum? noButtonText, MessageBoxCallback callback) :
            this(type, MyMessageBoxButtonsType.YES_NO, MyTextsWrapper.Get(messageText), MyTextsWrapper.Get(messageCaption), null, yesButtonText, noButtonText, callback, false)
        {
        }

        //  Constructor for YES/NO message boxes - and text and caption defined by StringBuilder instead of enum
        public MyGuiScreenMessageBox(MyMessageBoxType type, StringBuilder messageText, StringBuilder messageCaption, MyTextsWrapperEnum? yesButtonText, MyTextsWrapperEnum? noButtonText, MessageBoxCallback callback) :
            this(type, MyMessageBoxButtonsType.YES_NO, messageText, messageCaption, null, yesButtonText, noButtonText, callback, false)
        {
        }

        //  Constructor for YES/NO message boxes and checkBox
        public MyGuiScreenMessageBox(MyMessageBoxType type, MyTextsWrapperEnum messageText, MyTextsWrapperEnum messageCaption, MyTextsWrapperEnum checkBoxMessage, MyTextsWrapperEnum? okButtonText, MyTextsWrapperEnum? yesButtonText, MyTextsWrapperEnum? noButtonText, MessageBoxCallback callback, out MyGuiControlCheckbox showAgainCheckBox) :
            this(type, MyMessageBoxButtonsType.YES_NO, MyTextsWrapper.Get(messageText), MyTextsWrapper.Get(messageCaption), null, yesButtonText, noButtonText, callback, true)
        {
            showAgainCheckBox = m_showAgainCheckBox;
        }

        //  Constructor for YES/NO message boxes and TIMEOUT
        public MyGuiScreenMessageBox(MyMessageBoxType type, MyTextsWrapperEnum messageText, MyTextsWrapperEnum messageCaption, MyTextsWrapperEnum? yesButtonText, MyTextsWrapperEnum? noButtonText, MessageBoxCallback callback, int timeoutInMiliseconds) :
            this(type, MyMessageBoxButtonsType.YES_NO_TIMEOUT, MyTextsWrapper.Get(messageText), MyTextsWrapper.Get(messageCaption), null, yesButtonText, noButtonText, callback, false)
        {
            m_timeoutStartedTimeInMiliseconds = MyMinerGame.TotalTimeInMilliseconds;
            m_timeoutInMiliseconds = timeoutInMiliseconds;
        }

        private MyGuiScreenMessageBox(MyMessageBoxType type, MyMessageBoxButtonsType buttonType, StringBuilder messageText, StringBuilder messageCaption, MyTextsWrapperEnum? okButtonText, MyTextsWrapperEnum? yesButtonText, MyTextsWrapperEnum? noButtonText, MessageBoxCallback callback, bool enableCheckBox)
            : this(type, buttonType, messageText, messageCaption, okButtonText, yesButtonText, noButtonText, callback, enableCheckBox, MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE)
        {
        }

        public MyGuiScreenMessageBox(MyMessageBoxType type, MyMessageBoxButtonsType buttonType, StringBuilder messageText, StringBuilder messageCaption, MyTextsWrapperEnum? okButtonText, MyTextsWrapperEnum? yesButtonText, MyTextsWrapperEnum? noButtonText, MessageBoxCallback callback, bool enableCheckBox, Vector2 buttonSize)            
            : base(new Vector2(0.5f, 0.5f), null, null, true, null)
        {
            InstantClose = true;

            MyMessageBoxConfiguration config = m_typesConfiguration[(int)type];
            m_backgroundColor = config.BackgroundColor;
            m_backgroundTexture = config.Texture;
            m_textColor = config.TextColor;
            m_interferenceVideoColor = config.InterferenceVideoColor;

            m_enableBackgroundFade = true;

            m_buttonType = buttonType;
            m_okButtonText = okButtonText;
            m_yesButtonText = yesButtonText;
            m_noButtonText = noButtonText;
            m_callback = callback;            
            m_drawEvenWithoutFocus = true;
            m_screenCanHide = false;
            m_buttonSize = buttonSize;                    

            //  Recalculate heigh of message box screen, so it will auto-adapt to message size and maybe make split it on more lines
            Vector2 textSize = MyGuiManager.GetNormalizedSize(config.Font, messageText, MyGuiConstants.MESSAGE_BOX_TEXT_SCALE);
            Vector2 captionSize = MyGuiManager.GetNormalizedSize(config.Font, messageCaption, MyGuiConstants.MESSAGE_BOX_TEXT_SCALE);
            m_size = new Vector2(Math.Max(2f * m_buttonSize.X + 0.1f, textSize.X) + MyGuiConstants.MESSAGE_BOX_BORDER_AREA_X, 
                                3 * MyGuiConstants.MESSAGE_BOX_BORDER_AREA_Y + captionSize.Y + textSize.Y + MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE.Y)+ new Vector2();

            //if (type == MyMessageBoxType.MESSAGE) m_size = m_size + new Vector2(0.1f, 0.1f);
            //m_size. =m_size.Value.Y * 1.1f;
            if (enableCheckBox)
            {
                m_size = new Vector2(m_size.Value.X, m_size.Value.Y + 0.05f);
            }

            //  Message box caption
            MyGuiControlLabel captionLabel = new MyGuiControlLabel(this, new Vector2(0, -m_size.Value.Y / 2.0f + captionSize.Y / 2.0f + MyGuiConstants.MESSAGE_BOX_BORDER_AREA_Y),
                null, messageCaption, m_textColor, MyGuiConstants.MESSAGE_BOX_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER,config.Font);
            Controls.Add(captionLabel);

            //  Message box text
            m_messageBoxText = new MyGuiControlLabel(this, new Vector2(0f, captionLabel.GetPosition().Y + textSize.Y / 2.0f + 1.0f * MyGuiConstants.MESSAGE_BOX_BORDER_AREA_Y), null, messageText,
                m_textColor, MyGuiConstants.MESSAGE_BOX_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, config.Font);
            Controls.Add(m_messageBoxText);

            float deltaY = 0;

            if (enableCheckBox)
            {
                const float CHECKBOX_DELTA_Y = 0.01f;

                // CheckBox to not show again this message box
                m_showAgainCheckBox = new MyGuiControlCheckbox(this, new Vector2(-0.02f,
                    m_messageBoxText.GetPosition().Y + CHECKBOX_DELTA_Y + textSize.Y / 2.0f + 2 * MyGuiConstants.MESSAGE_BOX_BORDER_AREA_Y),
                    true, config.ButtonColor);
                Controls.Add(m_showAgainCheckBox);
                Controls.Add(new MyGuiControlLabel(this, new Vector2(0f, m_messageBoxText.GetPosition().Y + CHECKBOX_DELTA_Y + textSize.Y / 2.0f +
                    2 * MyGuiConstants.MESSAGE_BOX_BORDER_AREA_Y) + 0 * MyGuiConstants.CONTROLS_DELTA, null,
                    MyTextsWrapper.Get(MyTextsWrapperEnum.DecreaseVideoSettingsCheckBox), m_textColor,
                    MyGuiConstants.LABEL_TEXT_SCALE * 0.75f, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER, config.Font));
            }

            //  Buttons
            Vector2 buttonDelta = new Vector2(0.05f, m_size.Value.Y / 2.0f - MyGuiConstants.MESSAGE_BOX_BORDER_AREA_Y - m_buttonSize.Y / 2.0f);
            if (m_buttonType == MyMessageBoxButtonsType.OK)
            {

                    //Controls.Add(new MyGuiControlButton(this, new Vector2(0, deltaY + buttonDelta.Y), m_buttonSize, config.ButtonColor,
                    //null,null,null,
                    //m_okButtonText.Value, m_textColor, MyGuiConstants.BUTTON_TEXT_SCALE, MyGuiControlButtonTextAlignment.CENTERED, OnYesClick,
                    //true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true, true));

                var okButton = new MyGuiControlButton(this, new Vector2(0, deltaY + buttonDelta.Y), MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE_SMALL,
                    config.ButtonColor,
                    config.ButtonTexture, null, null, m_okButtonText.Value,
                    m_textColor, MyGuiConstants.BUTTON_TEXT_SCALE, MyGuiControlButtonTextAlignment.CENTERED, OnYesClick,
                    true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true, true);
                Controls.Add(okButton);


            }
            else if ((m_buttonType == MyMessageBoxButtonsType.YES_NO) || (m_buttonType == MyMessageBoxButtonsType.YES_NO_TIMEOUT))
            {

                var okButton = new MyGuiControlButton(this, new Vector2(-(buttonDelta.X + m_buttonSize.X / 2f), deltaY + buttonDelta.Y), MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE_SMALL,
                        config.ButtonColor,
                        config.ButtonTexture, null, null, m_yesButtonText.Value,
                        m_textColor, MyGuiConstants.BUTTON_TEXT_SCALE, MyGuiControlButtonTextAlignment.CENTERED, OnYesClick,
                        true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true, true);
                Controls.Add(okButton);

                var noButton = new MyGuiControlButton(this, new Vector2(+buttonDelta.X + m_buttonSize.X / 2f, deltaY + buttonDelta.Y), MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE_SMALL,
                    config.ButtonColor,
                    config.ButtonTexture, null, null, m_noButtonText.Value,
                    m_textColor, MyGuiConstants.BUTTON_TEXT_SCALE, MyGuiControlButtonTextAlignment.CENTERED, OnNoClick,
                    true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true, true);
                Controls.Add(noButton);


                //Controls.Add(new MyGuiControlButton(this, new Vector2(-(buttonDelta.X + m_buttonSize.X / 2f), deltaY + buttonDelta.Y), m_buttonSize, config.ButtonColor,
                //    null,null,null,
                //    m_yesButtonText.Value, m_textColor, MyGuiConstants.BUTTON_TEXT_SCALE, MyGuiControlButtonTextAlignment.CENTERED, OnYesClick, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true, true));

                //Controls.Add(new MyGuiControlButton(this, new Vector2(+buttonDelta.X + m_buttonSize.X / 2f, deltaY + buttonDelta.Y), m_buttonSize, config.ButtonColor,
                //    null,null,null,
                //    m_noButtonText.Value, m_textColor, MyGuiConstants.BUTTON_TEXT_SCALE, MyGuiControlButtonTextAlignment.CENTERED, OnNoClick, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true, true));
            }
            else if (m_buttonType == MyMessageBoxButtonsType.NONE)
            {
            }
            else
            {
                throw new MyMwcExceptionApplicationShouldNotGetHere();
            }
        }                

        public override string GetFriendlyName()
        {
            return "MyGuiScreenMessageBox";
        }

        public void OnYesClick(MyGuiControlButton sender)
        {
            OnClick(MyGuiScreenMessageBoxCallbackEnum.YES);
        }
        
        public void OnNoClick(MyGuiControlButton sender)
        {
            OnClick(MyGuiScreenMessageBoxCallbackEnum.NO);
        }

        private void OnClick(MyGuiScreenMessageBoxCallbackEnum result)
        {
            if (CloseBeforeCallback)
            {
                CloseInternal();
                Callback(result);
            }
            else
            {
                Callback(result);
                CloseInternal();
            }
        }

        private void CloseInternal()
        {
            if (InstantClose)
                CloseScreenNow();
            else
                CloseScreen();
        }

        private void CallbackYes()
        {
            if (m_callback != null) m_callback(MyGuiScreenMessageBoxCallbackEnum.YES);
        }

        void Callback(MyGuiScreenMessageBoxCallbackEnum val)
        {
            if (m_callback != null) m_callback(val);
        }

        public override bool Update(bool hasFocus)
        {
            if (base.Update(hasFocus) == false) return false;

            if (m_buttonType == MyMessageBoxButtonsType.YES_NO_TIMEOUT)
            {
                //  If timeout passed out, we need to call NO callback
                int deltaTime = MyMinerGame.TotalTimeInMilliseconds - m_timeoutStartedTimeInMiliseconds;
                if (deltaTime >= m_timeoutInMiliseconds)
                {
                    OnNoClick(null);
                }

                //  Update timeout number in message box label
                int timer = (int)MathHelper.Clamp((m_timeoutInMiliseconds - deltaTime) / 1000, 0, m_timeoutInMiliseconds / 1000);
                m_messageBoxText.UpdateParams(new string[] { timer.ToString() });
            }

            return true;
        }

        public override bool CloseScreen()
        {
            bool ret = base.CloseScreen();
            return ret;
        }

        protected override void Canceling()
        {
            base.Canceling();
            Callback(MyGuiScreenMessageBoxCallbackEnum.NO);
        }

        public static void Show(
            MyTextsWrapperEnum text,
            MyTextsWrapperEnum caption = MyTextsWrapperEnum.Blank,
            MyMessageBoxType type = MyMessageBoxType.MESSAGE)
        {
            MyGuiManager.AddScreen(
                new MyGuiScreenMessageBox(
                    type,
                    text,
                    caption,
                    MyTextsWrapperEnum.Ok,
                    null));
        }
    }
}
