using MinerWarsMath;
using MinerWarsMath.Graphics;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Textures;

namespace MinerWars.AppCode.Game.GUI.Core
{
    using App;
    using System;
    using MinerWars.AppCode.Networking.SectorService;
    using SysUtils.Utils;

    abstract class MyGuiScreenProgressBase : MyGuiScreenBase
    {
        bool m_controlsCreated = false;
        bool m_enableCancel = false;
        bool m_loaded = false;
        MyTextsWrapperEnum m_progressText;

        MyTexture2D m_wheelTexture;
        public MyGuiScreenProgressBase(MyTextsWrapperEnum progressText, bool enableCancel) :
            base(new Vector2(0.5f, 0.5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, null)
        {
            m_progressText = progressText;
            m_enableCancel = enableCancel;

            m_enableBackgroundFade = true;
            DrawMouseCursor = enableCancel;
            m_closeOnEsc = enableCancel;
           
            m_drawEvenWithoutFocus = true;
            
            // There is no reason for hiding progress screens!
            m_screenCanHide = false;
        }

        protected bool ReturnToMainMenuOnError = false;

        protected virtual void OnCancelClick(MyGuiControlButton sender)
        {
            Canceling();
        }

        void LoadControls()
        {
            // Background texture unloaded in base
            m_backgroundTexture = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\BackgroundScreen\\ProgressBackground", flags: TextureFlags.IgnoreQuality);
            m_wheelTexture = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Loading", flags: TextureFlags.IgnoreQuality);

            m_size = new Vector2(598 / 1600f, 368 / 1200f);
            Controls.Add(new MyGuiControlLabel(this, new Vector2(0.0f, -0.05f), null, m_progressText, MyGuiConstants.LABEL_TEXT_COLOR,
                MyGuiConstants.LABEL_TEXT_SCALE * 0.86f, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER));

            float deltaX = (m_enableCancel) ? 0.08f : 0.0f;
            float deltaY = 0.035f;

            Controls.Add(new MyGuiControlRotatingWheel(this, new Vector2(-deltaX, deltaY), MyGuiConstants.ROTATING_WHEEL_COLOR, MyGuiConstants.ROTATING_WHEEL_DEFAULT_SCALE,
                MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, m_wheelTexture));

            //  Sometimes we don't want to allow user to cancel pending progress screen
            if (m_enableCancel)
            {
                var cancelButton = new MyGuiControlButton(this, new Vector2(deltaX, deltaY), MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE_SMALL,
                        Vector4.One,
                        MyGuiManager.GetConfirmButton(), null, null, MyTextsWrapperEnum.Cancel,
                        MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, MyGuiControlButtonTextAlignment.CENTERED, OnCancelClick,
                        true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true, true);
                Controls.Add(cancelButton);
            }
            m_controlsCreated = true;
        }

        #region Overrides of MyGuiScreenBase

        public override bool Draw(float backgroundFadeAlpha)
        {
            // Load in draw, because sometimes screen is invisible and saving is on background
            if (!m_controlsCreated)
            {
                LoadControls();
            }

            return base.Draw(backgroundFadeAlpha);
        }

        public override void LoadContent()
        {
            if (!m_loaded)
            {
                m_loaded = true;
                ProgressStart();
            }
        }

        public override void UnloadContent()
        {
            m_loaded = false;

            base.UnloadContent();
            if (m_controlsCreated)
            {
                MyTextureManager.UnloadTexture(m_wheelTexture);
            }
        }

        protected abstract void ProgressStart();

        #endregion
    }
}
