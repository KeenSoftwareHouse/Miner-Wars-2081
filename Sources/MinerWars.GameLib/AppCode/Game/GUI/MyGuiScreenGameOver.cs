using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Audio;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Utils;
using MinerWarsMath;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.App;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.Utils;
using System.Diagnostics;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.Sessions;

namespace MinerWars.AppCode.Game.GUI
{
    enum MyGameOverTypeEnum
    {
        GameOver,
        MissionFail,        
    }

    class MyGuiScreenGameOver : MyGuiScreenBase
    {
        private float m_fadeTime;
        private float m_time;
        private MyTextsWrapperEnum? m_customMessage;

        public MyGuiScreenGameOver(float fadeTime, MyTextsWrapperEnum? customMessage = null)
            : base(new Vector2(0.5f, 0.5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, null)
        {
            m_fadeTime = fadeTime;
            m_customMessage = customMessage;
            m_size = MyGuiManager.GetNormalizedSizeFromScreenSize(new Vector2(MyGuiManager.GetSafeFullscreenRectangle().Width, MyGuiManager.GetSafeFullscreenRectangle().Height));
            m_backgroundTexture = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\BackgroundScreen\\MissionFailed", flags: TextureFlags.IgnoreQuality);

            if (!MyMultiplayerGameplay.IsRunning || MyMultiplayerGameplay.Static.IsHost)
            {
                OnEnterCallback += OnEnter;
            }

            // we wan't clear all hud cues when is game over
            MyHudAudio.Reset();
            // 0004862: game over zvuk - vypnut / zadisablovat
            //MyAudio.AddCue2D(MySoundCuesEnum.SfxGameOver);
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenGameOver";
        }

        public override bool Update(bool hasFocus)
        {
            base.Update(hasFocus);

            m_time += MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS;
            m_backgroundColor = Vector4.One * MathHelper.Clamp(m_time / m_fadeTime, 0, 1);

            return true;
        }

        public override bool Draw(float backgroundFadeAlpha)
        {
            if (base.Draw(backgroundFadeAlpha))
            {
                if (m_customMessage.HasValue)
                {
                    MyGuiManager.DrawString(MyGuiManager.GetFontMinerWarsRed(),
                            MyTextsWrapper.Get(m_customMessage.Value),
                            new Vector2(0.5f, 680 / 1200f), 1f,
                            MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER);
                }

                if (MyMultiplayerGameplay.IsRunning && !MyMultiplayerGameplay.Static.IsHost)
                {
                    MyGuiManager.DrawString(MyGuiManager.GetFontMinerWarsRed(),
                        MyTextsWrapper.Get(MyTextsWrapperEnum.GameOverInstructionsPart2MP),
                        new Vector2(0.5f, 870 / 1200f), 1f,
                        MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER);
                }
                else
                {
                    MyGuiManager.DrawString(MyGuiManager.GetFontMinerWarsRed(),
                        MyTextsWrapper.Get(MyTextsWrapperEnum.GameOverInstructionsPart2),
                        new Vector2(0.5f, 870 / 1200f), 1f,
                        MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER);
                }
                MyGuiManager.DrawString(MyGuiManager.GetFontMinerWarsRed(),
                        MyTextsWrapper.Get(MyTextsWrapperEnum.GameOverInstructionsPart1),
                        new Vector2(0.5f, 930 / 1200f), 1f,
                        MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER);
            }
            return true;
        }

        protected override void Canceling()
        {
            if (MySession.Static != null && MySession.Static.Player != null)
            {
                MySession.Static.Player.RestoreHealth();
            }

            MyGuiScreenMainMenu.UnloadAndExitToMenu();
        }

        private void OnEnter()
        {
            var gameplayType = MyGuiScreenGamePlay.Static.GetGameType();
            Debug.Assert(gameplayType == MyGuiScreenGamePlayType.GAME_STORY || gameplayType == MyGuiScreenGamePlayType.GAME_SANDBOX);
            MyGuiScreenGamePlay.Static.Restart();
        }
    }
}
