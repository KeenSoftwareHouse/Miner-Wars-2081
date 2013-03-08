using MinerWarsMath;
using MinerWarsMath.Graphics;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Managers;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.Utils;

namespace MinerWars.AppCode.Game.GUI
{
    class MyGuiScreenRegister : MyGuiScreenBase
    {
        MyGuiControlTextbox m_usernameTextbox;
        MyGuiControlTextbox m_passwordTextbox;
        MyGuiControlTextbox m_retypePasswordTextbox;
        MyGuiControlTextbox m_emailTextbox;        
        MyGuiControlCheckbox m_sendNewslettersCheckbox;
        MyGuiControlCheckbox m_rememberCheckbox;
        MyGuiScreenBase m_openAfterSuccesfullRegistration;


        public MyGuiScreenRegister(MyGuiScreenBase openAfterSuccesfullRegistration, string login, string password)
            : base(new Vector2(0.5f, 0.5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, null)
        {
            m_enableBackgroundFade = true;
            m_backgroundTexture = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\BackgroundScreen\\VideoBackground", flags: TextureFlags.IgnoreQuality);

            m_size = new Vector2(1030f / 1600f, 897f / 1200f);

            m_openAfterSuccesfullRegistration = openAfterSuccesfullRegistration;
            //  If user presses enter in any control, we call this method. It's default ENTER.
            this.OnEnterCallback = delegate { OnOkClick(null); };

            Vector2 controlsOriginLeft = new Vector2(-m_size.Value.X / 2.0f + 0.07f, -m_size.Value.Y / 2.0f + 0.125f + 0.018f);
            Vector2 controlsOriginRight = new Vector2(-m_size.Value.X / 2.0f + 0.32f, -m_size.Value.Y / 2.0f + 0.125f + 0.018f );
            Vector2 checkboxControlsOriginRight = new Vector2(-m_size.Value.X / 2.0f + 0.35f, -m_size.Value.Y / 2.0f + 0.125f);

            AddCaption(MyTextsWrapperEnum.Registration, new Vector2(0, 0.005f));

            // Choose a username
            Controls.Add(new MyGuiControlLabel(this, controlsOriginLeft + 0.5f * MyGuiConstants.CONTROLS_DELTA, null, MyTextsWrapperEnum.ChooseUsername, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            m_usernameTextbox = new MyGuiControlTextbox(this, controlsOriginRight + 0.5f * MyGuiConstants.CONTROLS_DELTA + new Vector2(MyGuiConstants.TEXTBOX_MEDIUM_SIZE.X / 2.0f, 0), MyGuiControlPreDefinedSize.MEDIUM, "", MyMwcValidationConstants.USERNAME_LENGTH_MAX, MyGuiConstants.TEXTBOX_BACKGROUND_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiControlTextboxType.NORMAL);
            Controls.Add(m_usernameTextbox);            

            // Choose a password
            Controls.Add(new MyGuiControlLabel(this, controlsOriginLeft + 1.5f * MyGuiConstants.CONTROLS_DELTA, null, MyTextsWrapperEnum.ChoosePassword, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            m_passwordTextbox = new MyGuiControlTextbox(this, controlsOriginRight + 1.5f * MyGuiConstants.CONTROLS_DELTA + new Vector2(MyGuiConstants.TEXTBOX_MEDIUM_SIZE.X / 2.0f, 0), MyGuiControlPreDefinedSize.MEDIUM, "", MyMwcValidationConstants.PASSWORD_LENGTH_MAX, MyGuiConstants.TEXTBOX_BACKGROUND_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiControlTextboxType.PASSWORD);
            Controls.Add(m_passwordTextbox);

            // Retype password
            Controls.Add(new MyGuiControlLabel(this, controlsOriginLeft + 2.5f * MyGuiConstants.CONTROLS_DELTA, null, MyTextsWrapperEnum.RetypePassword, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            m_retypePasswordTextbox = new MyGuiControlTextbox(this, controlsOriginRight + 2.5f * MyGuiConstants.CONTROLS_DELTA + new Vector2(MyGuiConstants.TEXTBOX_MEDIUM_SIZE.X / 2.0f, 0), MyGuiControlPreDefinedSize.MEDIUM, "", MyMwcValidationConstants.PASSWORD_LENGTH_MAX, MyGuiConstants.TEXTBOX_BACKGROUND_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiControlTextboxType.PASSWORD);
            Controls.Add(m_retypePasswordTextbox);

            // Email address
            Controls.Add(new MyGuiControlLabel(this, controlsOriginLeft + 3.5f * MyGuiConstants.CONTROLS_DELTA, null, MyTextsWrapperEnum.EmailAddress, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            m_emailTextbox = new MyGuiControlTextbox(this, controlsOriginRight + 3.5f * MyGuiConstants.CONTROLS_DELTA + new Vector2(MyGuiConstants.TEXTBOX_MEDIUM_SIZE.X / 2.0f, 0), MyGuiControlPreDefinedSize.MEDIUM, "", MyMwcValidationConstants.EMAIL_LENGTH_MAX, MyGuiConstants.TEXTBOX_BACKGROUND_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiControlTextboxType.NORMAL);
            Controls.Add(m_emailTextbox);            

            // Send me newsletters
            Controls.Add(new MyGuiControlLabel(this, (new Vector2(0.05f,0) + controlsOriginLeft) + 5.5f * MyGuiConstants.CONTROLS_DELTA, null, MyTextsWrapperEnum.SendNewsletters, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            m_sendNewslettersCheckbox = new MyGuiControlCheckbox(this,
             controlsOriginLeft + 5.5f * MyGuiConstants.CONTROLS_DELTA + new Vector2(MyGuiConstants.CHECKBOX_WITH_GLOW_SIZE.X / 2.0f, 0),
             MyGuiConstants.CHECKBOX_WITH_GLOW_SIZE,
             MyGuiManager.GetCheckboxOffTexture(), MyGuiManager.GetCheckboxOnTexture(), null,
             true, MyGuiConstants.CHECKBOX_BACKGROUND_COLOR, true, null);


            //m_sendNewslettersCheckbox = new MyGuiControlCheckbox(this, controlsOriginLeft + 6f * MyGuiConstants.CONTROLS_DELTA + new Vector2(MyGuiConstants.CHECKBOX_SIZE.X / 2.0f, 0), true, MyGuiConstants.CHECKBOX_BACKGROUND_COLOR);
            Controls.Add(m_sendNewslettersCheckbox);

            bool remember = MyConfig.RememberUsernameAndPassword;

            // Remember me
            Controls.Add(new MyGuiControlLabel(this, (new Vector2(0.05f, 0) + controlsOriginLeft) + 6.5f * MyGuiConstants.CONTROLS_DELTA, null, MyTextsWrapperEnum.RememberMe, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            m_rememberCheckbox = new MyGuiControlCheckbox(this,
                 controlsOriginLeft + 6.5f * MyGuiConstants.CONTROLS_DELTA + new Vector2(MyGuiConstants.CHECKBOX_WITH_GLOW_SIZE.X / 2.0f, 0),
                 MyGuiConstants.CHECKBOX_WITH_GLOW_SIZE,
                 MyGuiManager.GetCheckboxOffTexture(), MyGuiManager.GetCheckboxOnTexture(), null,
                 remember, MyGuiConstants.CHECKBOX_BACKGROUND_COLOR, true, null);
            Controls.Add(m_rememberCheckbox);
            /*
            // Buttons APPLY and BACK
            Vector2 buttonDelta = new Vector2(0.05f, m_size.Value.Y / 2.0f - MyGuiConstants.MESSAGE_BOX_BORDER_AREA_Y - MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE.Y / 2.0f);
            Controls.Add(new MyGuiControlButton(this, new Vector2(-buttonDelta.X, buttonDelta.Y), MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.Ok, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnOkClick, MyGuiControlButtonTextAlignment.CENTERED, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));
            Controls.Add(new MyGuiControlButton(this, new Vector2(+buttonDelta.X, buttonDelta.Y), MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.Cancel, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnCancelClick, MyGuiControlButtonTextAlignment.CENTERED, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));
            */

            var okButton = new MyGuiControlButton(this, new Vector2(-0.1379f, 0.2558f), MyGuiConstants.OK_BUTTON_SIZE,
                       MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                       MyGuiManager.GetInventoryScreenButtonTexture(), null, null, MyTextsWrapperEnum.Ok,
                       MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, MyGuiControlButtonTextAlignment.CENTERED, OnOkClick,
                       true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true, true);

            Controls.Add(okButton);

            var cancelButton = new MyGuiControlButton(this, new Vector2(0.1418f, 0.2558f), MyGuiConstants.OK_BUTTON_SIZE,
           MyGuiConstants.BUTTON_BACKGROUND_COLOR,
           MyGuiManager.GetInventoryScreenButtonTexture(), null, null, MyTextsWrapperEnum.Cancel,
           MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, MyGuiControlButtonTextAlignment.CENTERED, OnCancelClick,
           true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true, true);

            Controls.Add(cancelButton);


            if (login != null) 
            {
                m_usernameTextbox.Text = login;
            }
            if (password != null) 
            {
                m_passwordTextbox.Text = password;
                m_retypePasswordTextbox.Text = password;
            }
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenRegister";
        }

        public void OnOkClick(MyGuiControlButton sender)
        {
            MyTextsWrapperEnum? errorMessage = ValidateInput();
            if (errorMessage.HasValue)
            {
                MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, errorMessage.Value,
                    MyTextsWrapperEnum.MessageBoxCaptionError, MyTextsWrapperEnum.Ok, null));
            }
            else
            {
                MyGuiScreenRegisterProgress registerProgressScreen = new MyGuiScreenRegisterProgress(m_usernameTextbox.Text, m_passwordTextbox.Text, m_emailTextbox.Text, m_sendNewslettersCheckbox.Checked, m_openAfterSuccesfullRegistration, this);
                MyGuiManager.AddScreen(registerProgressScreen);
            }
        }

        public void OnCancelClick(MyGuiControlButton sender)
        {
            //  Just close the screen, ignore any change
            CloseScreen();
        }

        private MyTextsWrapperEnum? ValidateInput()
        {
            MyTextsWrapperEnum? errorMessage = null;

            if (MyMwcUtils.IsValidUsernameFormat(m_usernameTextbox.Text) == false)
            {
                errorMessage = MyTextsWrapperEnum.ValidationUsername;
            }
            else if (MyMwcUtils.IsValidPasswordFormat(m_passwordTextbox.Text) == false)
            {
                errorMessage = MyTextsWrapperEnum.ValidationPasswordWrong;
            }
            else if (MyMwcUtils.IsValidEmailAddress(m_emailTextbox.Text) == false)
            {
                errorMessage = MyTextsWrapperEnum.ValidationEmailWrong;
            }
            else if (m_passwordTextbox.Text != m_retypePasswordTextbox.Text)
            {
                errorMessage = MyTextsWrapperEnum.ValidationPasswordsDoNotMatch;
            }            
            return errorMessage;
        }

        public override bool Draw(float backgroundFadeAlpha)
        {
            if (base.Draw(backgroundFadeAlpha) == false) return false;

            Color textColor = new Color(MyGuiConstants.LABEL_TEXT_COLOR);
            MyGuiManager.DrawString(MyGuiManager.GetFontMinerWarsBlue(), MyTextsWrapper.Get(MyTextsWrapperEnum.RegistrationInfo), new Vector2(0.50f, 0.25f), 0.75f * 0.8f,
                textColor, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_BOTTOM);
            MyGuiManager.DrawString(MyGuiManager.GetFontMinerWarsBlue(), MyTextsWrapper.Get(MyTextsWrapperEnum.FormAgreement1), new Vector2(0.50f, 0.67f), 0.75f * 0.8f,
                textColor, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_BOTTOM);
            MyGuiManager.DrawString(MyGuiManager.GetFontMinerWarsBlue(), MyTextsWrapper.Get(MyTextsWrapperEnum.FormAgreement2), new Vector2(0.50f, 0.69f), 0.75f * 0.8f,
                textColor, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_BOTTOM);
            
            return true;
        }
    }
}
