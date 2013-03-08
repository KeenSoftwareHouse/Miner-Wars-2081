using MinerWarsMath;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.Utils;
using System;

namespace MinerWars.AppCode.Game.GUI
{
    class MyGuiScreenLogin : MyGuiScreenBase
    {
        MyGuiControlTextbox m_usernameTextbox;
        MyGuiControlTextbox m_passwordTextbox;
        MyGuiControlCheckbox m_rememberCheckbox;
        MyGuiControlCheckbox m_autologinCheckbox;
        MyGuiControlCheckbox m_hidePasswordCheckbox;
        MyGuiScreenBase m_openAfterSuccessfulLogin;
        Action m_callAfterSuccessfulLogin = null;

        bool m_simple;

        public bool CanBeCanceled = true;

        public MyGuiScreenLogin(string username, string password, Action callAfterSuccessfulLogin, bool simple = false)
            : this(username, password, (MyGuiScreenBase)null, simple)
        {
            m_callAfterSuccessfulLogin = callAfterSuccessfulLogin;
        }

        public MyGuiScreenLogin(string username, string password, MyGuiScreenBase openAfterSuccessfulLogin, bool simple = false)
            : base(new Vector2(0.5f, 0.5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, null)
        {
            m_simple = simple;
            m_enableBackgroundFade = true;
            m_openAfterSuccessfulLogin = openAfterSuccessfulLogin;
            m_size = new Vector2(1030f / 1600f, 674 / 1200f);
            m_backgroundTexture = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\BackgroundScreen\\LoginBackground", flags: TextureFlags.IgnoreQuality);
            //  If user presses enter in any control, we call this method. It's default ENTER.
            this.OnEnterCallback = delegate { OnOkClick(null); };

            Vector2 controlsOriginLeft = new Vector2(-m_size.Value.X / 2.0f + 0.07f, -m_size.Value.Y / 2.0f + 0.149f);
            Vector2 controlsOriginRight = new Vector2(-m_size.Value.X / 2.0f + 0.25f, -m_size.Value.Y / 2.0f + 0.149f);

            AddCaption(MyTextsWrapperEnum.Login);

            //  Player name
            Controls.Add(new MyGuiControlLabel(this, controlsOriginLeft + 0 * MyGuiConstants.CONTROLS_DELTA, null, MyTextsWrapperEnum.PlayerName, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            m_usernameTextbox = new MyGuiControlTextbox(this, controlsOriginRight + 0 * MyGuiConstants.CONTROLS_DELTA + new Vector2(MyGuiConstants.TEXTBOX_MEDIUM_SIZE.X / 2.0f, 0), MyGuiControlPreDefinedSize.MEDIUM, username, MyMwcValidationConstants.USERNAME_LENGTH_MAX, MyGuiConstants.DEFAULT_CONTROL_NONACTIVE_COLOR/*TEXTBOX_BACKGROUND_COLOR*/, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiControlTextboxType.NORMAL);
            Controls.Add(m_usernameTextbox);

            //  Password
            Controls.Add(new MyGuiControlLabel(this, controlsOriginLeft + 1 * MyGuiConstants.CONTROLS_DELTA, null, MyTextsWrapperEnum.Password, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            //m_passwordTextbox = new MyGuiControlTextbox(this, controlsOriginRight + 1 * MyGuiConstants.CONTROLS_DELTA + new Vector2(MyGuiConstants.TEXTBOX_WIDTH / 2.0f, 0), true, MyGuiConstants.LABEL_TEXT_COLOR);
            m_passwordTextbox = new MyGuiControlTextbox(this, controlsOriginRight + 1 * MyGuiConstants.CONTROLS_DELTA + new Vector2(MyGuiConstants.TEXTBOX_MEDIUM_SIZE.X / 2.0f, 0), MyGuiControlPreDefinedSize.MEDIUM, password, MyMwcValidationConstants.PASSWORD_LENGTH_MAX, MyGuiConstants.DEFAULT_CONTROL_NONACTIVE_COLOR/*TEXTBOX_BACKGROUND_COLOR*/, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiControlTextboxType.NORMAL);
            Controls.Add(m_passwordTextbox);

            bool remember = MyConfig.RememberUsernameAndPassword;
            bool autologin = MyConfig.Autologin;


            m_hidePasswordCheckbox = new MyGuiControlCheckbox(this,
                  new Vector2(0.1950f,-0.0793f), 
                  MyGuiConstants.CHECKBOX_WITH_GLOW_SIZE,
                 MyGuiManager.GetCheckboxOffTexture(), MyGuiManager.GetCheckboxOnTexture(), null,
                 true, MyGuiConstants.CHECKBOX_BACKGROUND_COLOR, true, null);
            //m_hidePasswordCheckbox = new MyGuiControlCheckbox(this, controlsOriginRight - 0.025f * Vector2.UnitX + MyGuiConstants.CONTROLS_DELTA + new Vector2(MyGuiConstants.CHECKBOX_SIZE.X / 2.0f, 0), true, MyGuiConstants.CHECKBOX_BACKGROUND_COLOR);
            Controls.Add(m_hidePasswordCheckbox);
            m_hidePasswordCheckbox.OnCheck = OnChangePasswordType;
            OnChangePasswordType(m_hidePasswordCheckbox);



            //  Remember
            m_rememberCheckbox = new MyGuiControlCheckbox(this,
                     controlsOriginRight + 2.25f * MyGuiConstants.CONTROLS_DELTA + new Vector2(MyGuiConstants.CHECKBOX_WITH_GLOW_SIZE.X / 2.0f, 0),
                     MyGuiConstants.CHECKBOX_WITH_GLOW_SIZE,
                     MyGuiManager.GetCheckboxOffTexture(), MyGuiManager.GetCheckboxOnTexture(), null,
                     remember, MyGuiConstants.CHECKBOX_BACKGROUND_COLOR, true, null);

            if (!m_simple)
            {
                Controls.Add(new MyGuiControlLabel(this, controlsOriginLeft + 2.25f * MyGuiConstants.CONTROLS_DELTA, null, MyTextsWrapperEnum.RememberMe, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
                Controls.Add(m_rememberCheckbox);
            }

            //  Auto-login
            m_autologinCheckbox = new MyGuiControlCheckbox(this,
                     controlsOriginRight + 3.25f * MyGuiConstants.CONTROLS_DELTA + new Vector2(MyGuiConstants.CHECKBOX_WITH_GLOW_SIZE.X / 2.0f, 0),
                     MyGuiConstants.CHECKBOX_WITH_GLOW_SIZE,
                     MyGuiManager.GetCheckboxOffTexture(), MyGuiManager.GetCheckboxOnTexture(), null,
                     autologin, MyGuiConstants.CHECKBOX_BACKGROUND_COLOR, true, null);
            
            if (!m_simple)
            {
                Controls.Add(new MyGuiControlLabel(this, controlsOriginLeft + 3.25f * MyGuiConstants.CONTROLS_DELTA, null, MyTextsWrapperEnum.Autologin, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
                Controls.Add(m_autologinCheckbox);
            }


            //  Buttons APPLY and BACK
            Vector2 buttonDelta = new Vector2(0.1f, m_size.Value.Y / 2.0f - MyGuiConstants.MESSAGE_BOX_BORDER_AREA_Y - MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE.Y / 2.0f);


            var okButton = new MyGuiControlButton(this, new Vector2(-0.1861f, 0.1611f), MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE_SMALL,
                Vector4.One,
                MyGuiManager.GetConfirmButton(), null, null, MyTextsWrapperEnum.Ok,
                MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, MyGuiControlButtonTextAlignment.CENTERED, OnOkClick,
                true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true, true);
            var cancelButton = new MyGuiControlButton(this, new Vector2(-0.0461f, 0.1611f), MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE_SMALL,
                Vector4.One,
                MyGuiManager.GetConfirmButton(), null, null, MyTextsWrapperEnum.Cancel,
                MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, MyGuiControlButtonTextAlignment.CENTERED, OnCancelClick,
                true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true, true);

            var registerButton = new MyGuiControlButton(this, new Vector2(0.1924f, 0.1611f), MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE_SMALL,
                Vector4.One,
                MyGuiManager.GetConfirmButton(), null, null, MyTextsWrapperEnum.Register,
                MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, MyGuiControlButtonTextAlignment.CENTERED, OnRegisterClick,
                true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true, true);
            Controls.Add(okButton);
            Controls.Add(cancelButton);

            if (!m_simple)
            {
                Controls.Add(registerButton);
            }

            //  Hide password
            Controls.Add(new MyGuiControlLabel(this, new Vector2(0.2164f,-0.0793f),  null, MyTextsWrapperEnum.HidePassword, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));


        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenLogin";
        }

        public void OnOkClick(MyGuiControlButton sender)
        {
            //  Trim username and password because some users mistakenly write a space at the start/end
            //  and then wonder why they can't log in
            m_usernameTextbox.Text = m_usernameTextbox.Text.Trim();
            m_passwordTextbox.Text = m_passwordTextbox.Text.Trim();

            SaveToConfig();
            MyGuiScreenLoginProgress loginProgressScreen;
            if (m_openAfterSuccessfulLogin == null && m_callAfterSuccessfulLogin != null)
            {
                loginProgressScreen = new MyGuiScreenLoginProgress(m_usernameTextbox.Text, m_passwordTextbox.Text, m_callAfterSuccessfulLogin, this);
            }
            else
            {
                loginProgressScreen = new MyGuiScreenLoginProgress(m_usernameTextbox.Text, m_passwordTextbox.Text, m_openAfterSuccessfulLogin, this);
            }
            MyGuiManager.AddScreen(loginProgressScreen);

            //CloseScreen();
        }

        public void SetUsernameAndPassword(string username, string password)
        {
            m_usernameTextbox.Text = username;
            m_passwordTextbox.Text = password;
        }

        //  change is password is normal text or asterixs
        public void OnChangePasswordType(MyGuiControlCheckbox sender)
        {
            if (m_hidePasswordCheckbox.Checked)
                m_passwordTextbox.ChangeMyType(MyGuiControlTextboxType.PASSWORD);
            else m_passwordTextbox.ChangeMyType(MyGuiControlTextboxType.NORMAL);
        }

        void SaveToConfig()
        {
            MyConfig.RememberUsernameAndPassword = m_rememberCheckbox.Checked;
            MyConfig.Autologin = m_autologinCheckbox.Checked;

            string username = "";
            string password = "";
            if (m_rememberCheckbox.Checked)
            {
                username = m_usernameTextbox.Text;
                password = m_passwordTextbox.Text;
            }
            MyConfig.Username = username;
            MyConfig.Password = password;
            MyConfig.Save();
        }

        public override void HandleInput(MyGuiInput input, bool receivedFocusInThisUpdate)
        {
            base.HandleInput(input, receivedFocusInThisUpdate);

            //  If autologin is true then we always must remember the login and password, otherwise autologin won't work
            if (m_autologinCheckbox.Checked) m_rememberCheckbox.Checked = true;
        }

        public void OnCancelClick(MyGuiControlButton sender)
        {
            if (!MyGuiManager.IsMainMenuScreenOpened())
            {
                MyGuiScreenMainMenu.SkipAutologin();
                MyGuiManager.BackToMainMenu();
            }

            MyGuiManager.UpdateDemoAccessScreen();

            //  Just close the screen, ignore any change
            CloseScreen();
        }

        public void OnRegisterClick(MyGuiControlButton sender)
        {
            MyGuiManager.AddScreen(new MyGuiScreenRegister(this, m_usernameTextbox.Text, m_passwordTextbox.Text));
        }

        protected override void Canceling()
        {
            MyGuiManager.UpdateDemoAccessScreen();

            if(CanBeCanceled)
                base.Canceling();
        }
    }
}
