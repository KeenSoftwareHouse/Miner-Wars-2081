using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.Managers;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Toolkit.Input;

namespace MinerWars.AppCode.Game.GUI.Core
{
    class MyGuiScreenLogo: MyGuiScreenBase
    {
        private int? m_startTime;
        private string m_textureName;
        private MyTexture2D m_texture;

        private int m_fadeIn, m_fadeOut, m_openTime;
        private float m_scale;

        /// <summary>
        /// Time in ms
        /// </summary>
        public MyGuiScreenLogo(string texture, float scale = 0.66f, int fadeIn = 300, int fadeOut = 300, int openTime = 300)
            : base(new Vector2(0.5f, 0.5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, Vector2.One)
        {
            this.m_scale = scale;
            this.m_fadeIn = fadeIn;
            this.m_fadeOut = fadeOut;
            this.m_openTime = openTime;
            DrawMouseCursor = false;
            m_textureName = texture;
            m_closeOnEsc = true;
        }

        public override bool CanHandleInputDuringTransition()
        {
            return true;
        }

        public override void LoadContent()
        {
            m_texture = MyTextureManager.GetTexture<MyTexture2D>(m_textureName, flags: TextureFlags.IgnoreQuality);
            base.LoadContent();
        }

        public override void UnloadContent()
        {
            if (m_texture != null)
            {
                MyTextureManager.UnloadTexture(m_texture);
            }
            base.UnloadContent();
        }

        protected override void Canceling()
        {
            this.m_fadeOut = 0;
            base.Canceling();
        }

        public override void HandleInput(MyGuiInput input, bool receivedFocusInThisUpdate)
        {
            base.HandleInput(input, receivedFocusInThisUpdate);

            if (input.IsNewLeftMousePressed() || input.IsNewRightMousePressed() || input.IsNewKeyPress(Keys.Space) || input.IsNewKeyPress(Keys.Enter))
                Canceling();
        }

        public override string GetFriendlyName()
        {
            return "Logo screen";
        }

        public override int GetTransitionOpeningTime()
        {
            return m_fadeIn;
        }

        public override int GetTransitionClosingTime()
        {
            return m_fadeOut;
        }

        public override bool Draw(float backgroundFadeAlpha)
        {
            Rectangle backgroundRectangle;
            MyGuiManager.GetSafeAspectRatioFullScreenPictureSize(MyGuiConstants.LOADING_BACKGROUND_TEXTURE_REAL_SIZE, out backgroundRectangle);

            backgroundRectangle.Inflate(-(int)(backgroundRectangle.Width * (1-m_scale) / 2), -((int)(backgroundRectangle.Height * (1-m_scale) / 2)));
            MyGuiManager.DrawSpriteBatch(m_texture, backgroundRectangle, new Color(new Vector4(0.95f, 0.95f, 0.95f, m_transitionAlpha)));
            return true;
        }

        public override bool Update(bool hasFocus)
        {
            if (base.Update(hasFocus) == false) return false;

            if(GetState() == MyGuiScreenState.OPENED && !m_startTime.HasValue)
                m_startTime = MyMinerGame.TotalGamePlayTimeInMilliseconds;

            if (m_startTime.HasValue && MyMinerGame.TotalGamePlayTimeInMilliseconds > (m_startTime + m_openTime))
                this.CloseScreen();

            return true;
        }
    }
}
