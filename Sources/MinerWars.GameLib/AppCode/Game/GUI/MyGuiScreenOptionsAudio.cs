using System.Text;
using MinerWarsMath;
using MinerWarsMath.Graphics;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Audio;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Textures;

namespace MinerWars.AppCode.Game.GUI
{
    class MyGuiScreenOptionsAudio : MyGuiScreenBase
    {
        class MyGuiScreenOptionsAudioSettings
        {
            public float GameVolume;
            public float MusicVolume;
        }

        MyGuiControlSlider m_gameVolumeSlider;
        MyGuiControlSlider m_musicVolumeSlider;
        MyGuiScreenOptionsAudioSettings m_settingsOld = new MyGuiScreenOptionsAudioSettings();
        MyGuiScreenOptionsAudioSettings m_settingsNew = new MyGuiScreenOptionsAudioSettings();

        
        public MyGuiScreenOptionsAudio()
            : base(new Vector2(0.5f, 0.5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, null)
        {
            m_enableBackgroundFade = true;
            m_size = new Vector2(1030f / 1600f, 572f / 1200f);

            m_backgroundTexture = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\BackgroundScreen\\AudioBackground", flags: TextureFlags.IgnoreQuality);

            AddCaption(MyTextsWrapperEnum.AudioOptions, new Vector2(0, 0.005f));

            Vector2 controlsOriginLeft = new Vector2(-m_size.Value.X / 2.0f + 0.07f, -m_size.Value.Y / 2.0f + 0.149f);
            Vector2 controlsOriginRight = new Vector2(-m_size.Value.X / 2.0f + 0.325f, -m_size.Value.Y / 2.0f + 0.149f);
            Vector2 controlsDelta = new Vector2(0, 0.0525f);

            //  Game Volume
            Controls.Add(new MyGuiControlLabel(this, controlsOriginLeft + 0 * controlsDelta, null, MyTextsWrapperEnum.GameVolume, MyGuiConstants.LABEL_TEXT_COLOR, 
                MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            m_gameVolumeSlider = new MyGuiControlSlider(this, controlsOriginRight + 0 * controlsDelta + new Vector2(MyGuiConstants.SLIDER_WIDTH / 2.0f, 0), MyGuiConstants.SLIDER_WIDTH,
                MyAudioConstants.GAME_MASTER_VOLUME_MIN, MyAudioConstants.GAME_MASTER_VOLUME_MAX, MyGuiConstants.SLIDER_BACKGROUND_COLOR,
                new StringBuilder(), MyGuiConstants.SLIDER_WIDTH_LABEL, 0, MyGuiConstants.LABEL_TEXT_SCALE * 0.85f);
            m_gameVolumeSlider.OnChange = OnGameVolumeChange;
            Controls.Add(m_gameVolumeSlider);

            //  Music Volume
            Controls.Add(new MyGuiControlLabel(this, controlsOriginLeft + 1 * controlsDelta, null, MyTextsWrapperEnum.MusicVolume, MyGuiConstants.LABEL_TEXT_COLOR, 
                MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            m_musicVolumeSlider = new MyGuiControlSlider(this, controlsOriginRight + 1 * controlsDelta + new Vector2(MyGuiConstants.SLIDER_WIDTH / 2.0f, 0), MyGuiConstants.SLIDER_WIDTH,
                MyAudioConstants.MUSIC_MASTER_VOLUME_MIN, MyAudioConstants.MUSIC_MASTER_VOLUME_MAX, MyGuiConstants.SLIDER_BACKGROUND_COLOR,
                new StringBuilder(), MyGuiConstants.SLIDER_WIDTH_LABEL, 0, MyGuiConstants.LABEL_TEXT_SCALE * 0.85f);
            m_musicVolumeSlider.OnChange = OnMusicVolumeChange;
            Controls.Add(m_musicVolumeSlider);

            /*
            //  Buttons OK and CANCEL
            Vector2 buttonDelta = new Vector2(0.05f, m_size.Value.Y / 2.0f - MyGuiConstants.MESSAGE_BOX_BORDER_AREA_Y - MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE.Y / 2.0f);
            Controls.Add(new MyGuiControlButton(this, new Vector2(-buttonDelta.X, buttonDelta.Y), MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.Ok, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnOkClick, MyGuiControlButtonTextAlignment.CENTERED, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));
            Controls.Add(new MyGuiControlButton(this, new Vector2(+buttonDelta.X, buttonDelta.Y), MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.Cancel, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnCancelClick, MyGuiControlButtonTextAlignment.CENTERED, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));

            */

            var m_okButton = new MyGuiControlButton(this, new Vector2(-0.1379f, 0.1041f), MyGuiConstants.OK_BUTTON_SIZE,
                                   MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                                   MyGuiManager.GetInventoryScreenButtonTexture(), null, null, MyTextsWrapperEnum.Ok,
                                   MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, MyGuiControlButtonTextAlignment.CENTERED, OnOkClick,
                                   true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true, true);
            Controls.Add(m_okButton);

            var m_cancelButton = new MyGuiControlButton(this, new Vector2(0.141f, 0.1041f), MyGuiConstants.OK_BUTTON_SIZE,
                       MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                       MyGuiManager.GetInventoryScreenButtonTexture(), null, null, MyTextsWrapperEnum.Cancel,
                       MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, MyGuiControlButtonTextAlignment.CENTERED, OnCancelClick,
                       true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true, true);
            Controls.Add(m_cancelButton);


            //  Update controls with values from config file
            UpdateFromConfig(m_settingsOld);
            UpdateFromConfig(m_settingsNew);
            UpdateControls(m_settingsOld);
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenOptionsAudio";
        }

        void UpdateFromConfig(MyGuiScreenOptionsAudioSettings settings)
        {
            m_settingsOld.GameVolume = MyConfig.GameVolume;
            m_settingsOld.MusicVolume = MyConfig.MusicVolume;
        }

        //void UpdateSettings(MyGuiScreenOptionsVideoSettings settings)
        //{
        //    settings.AspectRatio = (MyAspectRatioEnum)m_aspectRationCombobox.GetSelectedKey();
        //}

        void UpdateControls(MyGuiScreenOptionsAudioSettings settings)
        {
            m_gameVolumeSlider.SetValue(settings.GameVolume);
            m_musicVolumeSlider.SetValue(settings.MusicVolume);
        }

        void Save()
        {
            MyConfig.GameVolume = m_gameVolumeSlider.GetValue();
            MyConfig.MusicVolume = m_musicVolumeSlider.GetValue();
            MyConfig.Save();
        }

        static void UpdateValues(MyGuiScreenOptionsAudioSettings settings)
        {
            MyAudio.VolumeMusic = settings.MusicVolume;
            MyAudio.VolumeGame = settings.GameVolume;
            MyAudio.VolumeGui = settings.GameVolume;
        }

        public void OnOkClick(MyGuiControlButton sender)
        {
            //  Save values
            Save();

            CloseScreen();
        }

        public void OnCancelClick(MyGuiControlButton sender)
        {
            //  Revert to OLD values
            UpdateValues(m_settingsOld);
            
            CloseScreen();
        }

        void OnGameVolumeChange(MyGuiControlSlider sender)
        {
            m_settingsNew.GameVolume = m_gameVolumeSlider.GetValue();
            UpdateValues(m_settingsNew);
        }

        void OnMusicVolumeChange(MyGuiControlSlider sender)
        {
            m_settingsNew.MusicVolume = m_musicVolumeSlider.GetValue();
            UpdateValues(m_settingsNew);
        }
    }
}
