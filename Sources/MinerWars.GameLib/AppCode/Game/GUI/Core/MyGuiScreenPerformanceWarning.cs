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
using MinerWars.AppCode.Game.Localization;

namespace MinerWars.AppCode.Game.GUI.Core
{
    class MyGuiScreenPerformanceWarning: MyGuiScreenBase
    {
        private MyTexture2D m_texture;

        MyGuiScreenLoading m_loading;

        MyGuiControlMultilineText m_warningText1;
        MyGuiControlMultilineText m_warningText2;
        MyGuiControlMultilineText m_warningText3;
        MyGuiControlMultilineText m_warningText4;
        MyGuiControlMultilineText m_warningText5;
        MyGuiControlMultilineText m_warningText6;

        MyGuiControlButton m_button;

        float m_scale = 1.0f;

        /// <summary>
        /// Time in ms
        /// </summary>
        public MyGuiScreenPerformanceWarning(MyGuiScreenLoading loading)
            : base(new Vector2(0.5f, 0.5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, Vector2.One)
        {
            DrawMouseCursor = true;
            m_closeOnEsc = false;
            m_loading = loading;
        }

        public MyGuiScreenLoading LoadingScreen
        {
            get { return m_loading; }
        }

        public override bool CanHandleInputDuringTransition()
        {
            return true;
        }

        public override void LoadContent()
        {
            m_texture = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\BackgroundScreen\\PerformanceBackground", flags: TextureFlags.IgnoreQuality);

            float scale = 1.2f;

            m_warningText1 = new MyGuiControlMultilineText(
            this,
            new Vector2(0.245f, -0.09f),
            new Vector2(1.0f, 0.5f),
            Vector4.Zero,
            MyGuiManager.GetFontMinerWarsWhite(), scale,
            MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP,
            MyTextsWrapper.Get(MyTextsWrapperEnum.PerfWarning1_0), false, true);


            m_warningText2 = new MyGuiControlMultilineText(
            this,
            new Vector2(0.215f, -0.04f),
            new Vector2(1.0f, 0.5f),
            Vector4.Zero,
            MyGuiManager.GetFontMinerWarsWhite(), scale,
            MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP,
            new StringBuilder(), false, true);
            m_warningText2.AppendText(MyTextsWrapper.Get(MyTextsWrapperEnum.PerfWarning2_0), MyGuiManager.GetFontMinerWarsBlue(), scale, Vector4.One);



            m_warningText3 = new MyGuiControlMultilineText(
            this,
            new Vector2(0.225f, 0.12f),
            new Vector2(1.0f, 0.5f),
            Vector4.Zero,
            MyGuiManager.GetFontMinerWarsWhite(), scale,
            MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP,
            MyTextsWrapper.Get(MyTextsWrapperEnum.PerfWarning3_0), false, true);

            m_warningText4 = new MyGuiControlMultilineText(
            this,
            new Vector2(0.335f, 0.17f),
            new Vector2(1.0f, 0.5f),
            Vector4.Zero,
            MyGuiManager.GetFontMinerWarsWhite(), scale,
            MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP,
            MyTextsWrapper.Get(MyTextsWrapperEnum.PerfWarning4_0).Append(" "), false, true);
            m_warningText4.AppendText(MyTextsWrapper.Get(MyTextsWrapperEnum.PerfWarning4_1).Append(" "), MyGuiManager.GetFontMinerWarsBlue(), scale, Vector4.One);
            m_warningText4.AppendText(MyTextsWrapper.Get(MyTextsWrapperEnum.PerfWarning4_2).Append(": "), MyGuiManager.GetFontMinerWarsWhite(), scale, Vector4.One);
            m_warningText4.AppendText(new StringBuilder("VRAGE"), MyGuiManager.GetFontMinerWarsBlue(), scale, Vector4.One);



            m_warningText5 = new MyGuiControlMultilineText(
       this,
       new Vector2(0.26f, 0.33f),
       new Vector2(1.0f, 0.5f),
       Vector4.Zero,
       MyGuiManager.GetFontMinerWarsWhite(), scale,
       MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP,
       MyTextsWrapper.Get(MyTextsWrapperEnum.PerfWarning5_0), false, true);

            m_warningText6 = new MyGuiControlMultilineText(
          this,
          new Vector2(0.363f, 0.38f),
          new Vector2(1.0f, 0.5f),
          Vector4.Zero,
          MyGuiManager.GetFontMinerWarsWhite(), scale,
          MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP,
          MyTextsWrapper.Get(MyTextsWrapperEnum.PerfWarning6_0), false, true);



            m_button = new MyGuiControlButton(this,
                new Vector2(0, 0.3f),
                new Vector2(0.25f, 0.09f),
                new Vector4(0.8f, 0.8f, 0.8f, 1),
                new StringBuilder("I UNDERSTAND"),
                null,
                Vector4.One,
                1,
                OnButtonClick,
                MyGuiControlButtonTextAlignment.CENTERED,
                true,
                MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_TOP,
                true);

            Controls.Add(m_button);

            OnEnterCallback += delegate
            {
                CloseScreen();
            };

            base.LoadContent();
        }

        public override void UnloadContent()
        {
            MyTextureManager.UnloadTexture(m_texture);
            
            base.UnloadContent();
        }

        protected override void Canceling()
        {
            base.Canceling();
        }

        public override void HandleInput(MyGuiInput input, bool receivedFocusInThisUpdate)
        {
            base.HandleInput(input, receivedFocusInThisUpdate);

           // if (input.IsNewLeftMousePressed() || input.IsNewRightMousePressed() || input.IsNewKeyPress(Keys.Space) || input.IsNewKeyPress(Keys.Enter))
              //  Canceling();
        }

        public override string GetFriendlyName()
        {
            return "Performance warning screen";
        }

        public override bool Draw(float backgroundFadeAlpha)
        {
            Rectangle backgroundRectangle;
            MyGuiManager.GetSafeAspectRatioFullScreenPictureSize(MyGuiConstants.LOADING_BACKGROUND_TEXTURE_REAL_SIZE, out backgroundRectangle);

            backgroundRectangle.Inflate(-(int)(backgroundRectangle.Width * (1-m_scale) / 2), -((int)(backgroundRectangle.Height * (1-m_scale) / 2)));
            MyGuiManager.DrawSpriteBatch(m_texture, backgroundRectangle, new Color(new Vector4(1, 1, 1, m_transitionAlpha)));

          

            m_warningText1.Draw();
            m_warningText2.Draw();
            m_warningText3.Draw();
            m_warningText4.Draw();
            m_warningText5.Draw();
            m_warningText6.Draw();

            m_button.Draw();

            return true;
        }

        public override bool Update(bool hasFocus)
        {
            if (base.Update(hasFocus) == false) return false;

            return true;
        }

        void OnButtonClick(MyGuiControlButton sender)
        {
            CloseScreen();
        }

        public override bool CloseScreen()
        {
            //MyConfig.NeedShowPerfWarning = false;
            MyConfig.Save();

            return base.CloseScreen();
        }
    }
}
