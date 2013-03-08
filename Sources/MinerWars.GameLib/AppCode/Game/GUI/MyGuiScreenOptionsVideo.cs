using System.Text;
using MinerWarsMath;
using MinerWarsMath.Graphics;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.VideoMode;
using MinerWars.AppCode.Game.Voxels;
using MinerWars.AppCode.Game.Render;
using SysUtils.Utils;
using MinerWars.AppCode.Game.Textures;
//  Controls dependency:
//      - anti-aliasing combobox depends on full-screen checkbox

namespace MinerWars.AppCode.Game.GUI
{
    using System;
    using App;
    using SharpDX.Toolkit.Graphics;

    class MyGuiScreenOptionsVideo : MyGuiScreenBase
    {
        internal class MyGuiScreenOptionsVideoSettings
        {
            public int VideoAdapter;
            public bool FullScreen;
            public bool VerticalSync;
            public bool HardwareCursor;
            public MyRenderQualityEnum RenderQuality;
            public MyVideoModeEx VideoMode;
            public float FieldOfView;
        }

        readonly MyGuiControlCombobox m_videoAdapterCombobox;
        readonly MyGuiControlCombobox m_videoModeCombobox;
        readonly MyGuiControlCheckbox m_verticalSyncCheckbox;
        readonly MyGuiControlCheckbox m_fullscreenCheckbox;
        readonly MyGuiControlCheckbox m_hardwareCursorCheckbox;
        readonly MyGuiControlCombobox m_renderQualityCombobox;
        readonly MyGuiControlLabel m_recommendAspectRatioLabel;
        readonly MyGuiControlSlider m_fieldOfViewSlider;
        readonly MyGuiControlLabel m_fieldOfViewLabel;
        readonly MyGuiControlLabel m_fieldOfViewDefaultLabel;

        readonly MyGuiScreenOptionsVideoSettings m_settingsOld = new MyGuiScreenOptionsVideoSettings();
        readonly MyGuiScreenOptionsVideoSettings m_settingsNew = new MyGuiScreenOptionsVideoSettings();

        bool m_waitingForConfirmation = false;

        bool m_doRevert = false;


        public MyGuiScreenOptionsVideo()
            : base(new Vector2(0.5f, 0.5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, null)
        {
            MyMwcLog.WriteLine("MyGuiScreenOptionsVideo.ctor START");

            m_enableBackgroundFade = true;
            m_size = new Vector2(0.59f, 0.68544f);
            m_size *= 1.1f;

            m_backgroundTexture = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\BackgroundScreen\\VideoBackground", flags: TextureFlags.IgnoreQuality);
            
            AddCaption(MyTextsWrapperEnum.VideoOptions, new Vector2(0, 0.005f));
            //Controls.Add(new MyGuiControlLabel(this, -m_size.Value / 2.0f + MyGuiConstants.SCREEN_CAPTION_DELTA_Y, null, MyTextsWrapperEnum.VideoOptions, MyGuiConstants.SCREEN_CAPTION_TEXT_COLOR, MyGuiConstants.SCREEN_CAPTION_TEXT_SCALE));

            Vector2 controlsOriginLeft = new Vector2(-m_size.Value.X / 2.0f + 0.05f, -m_size.Value.Y / 2.0f + 0.145f) + new Vector2(0.02f,0f);
            Vector2 controlsOriginRight = new Vector2(-m_size.Value.X / 2.0f + 0.185f, -m_size.Value.Y / 2.0f + 0.145f) + new Vector2(0.043f,0f);
            controlsOriginRight.X += 0.04f;


            int controlIndex = 0;

            // Adapter

            Controls.Add(new MyGuiControlLabel(this, controlsOriginLeft + controlIndex * MyGuiConstants.CONTROLS_DELTA, null, MyTextsWrapperEnum.VideoAdapter, MyGuiConstants.LABEL_TEXT_COLOR,
             MyGuiConstants.LABEL_TEXT_SCALE_OPTIONS, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER, MyGuiManager.GetFontMinerWarsBlue()));

            m_videoAdapterCombobox = new MyGuiControlCombobox(this, controlsOriginRight + controlIndex++ * MyGuiConstants.CONTROLS_DELTA + new Vector2(MyGuiConstants.COMBOBOX_MEDIUM_SIZE.X / 2.0f, 0),
                MyGuiControlPreDefinedSize.MEDIUM, MyGuiConstants.COMBOBOX_BACKGROUND_COLOR, MyGuiConstants.COMBOBOX_TEXT_SCALE);
            m_videoAdapterCombobox.OnSelect += OnVideoAdapterSelected;
            AddAdaptersToComboBox();
            Controls.Add(m_videoAdapterCombobox);

            //  Video Mode
            Controls.Add(new MyGuiControlLabel(this, controlsOriginLeft + controlIndex * MyGuiConstants.CONTROLS_DELTA, null, MyTextsWrapperEnum.VideoMode, MyGuiConstants.LABEL_TEXT_COLOR,
                MyGuiConstants.LABEL_TEXT_SCALE_OPTIONS, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER, MyGuiManager.GetFontMinerWarsBlue()));

            m_videoModeCombobox = new MyGuiControlCombobox(this, controlsOriginRight + controlIndex++ * MyGuiConstants.CONTROLS_DELTA + new Vector2(MyGuiConstants.COMBOBOX_MEDIUM_SIZE.X / 2.0f, 0),
                MyGuiControlPreDefinedSize.MEDIUM, MyGuiConstants.COMBOBOX_BACKGROUND_COLOR, MyGuiConstants.COMBOBOX_TEXT_SCALE);
            //  Populating Video Mode with supported DisplayMode reported by XNA framework
            m_videoModeCombobox.OnSelect += OnVideoModeSelected;
            //Added on UpdateSettings
            //AddDisplayModesToComboBox(MyMinerGame.GraphicsDeviceManager.GraphicsAdapter.AdapterOrdinal);
            Controls.Add(m_videoModeCombobox);

            //  Recommended aspect ratio
            m_recommendAspectRatioLabel = new MyGuiControlLabel(this, new Vector2(controlsOriginRight.X + MyGuiConstants.COMBOBOX_TEXT_OFFSET.X, controlsOriginRight.Y + controlIndex++ * MyGuiConstants.CONTROLS_DELTA.Y - 0.015f), null,
                    new StringBuilder(), MyGuiConstants.LABEL_TEXT_COLOR * 0.9f, MyGuiConstants.LABEL_TEXT_SCALE_OPTIONS * 0.85f, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER, MyGuiManager.GetFontMinerWarsBlue());
            //Added on UpdateSettings
            //UpdateRecommendecAspectRatioLabel(MyMinerGame.GraphicsDeviceManager.GraphicsAdapter.AdapterOrdinal);
            Controls.Add(m_recommendAspectRatioLabel);

            //  reduce the spacing between "Recommended aspect ratio" and items below it
            Vector2 offSet = new Vector2(0, MyGuiConstants.CONTROLS_DELTA.Y / 2);
            controlsOriginLeft -= offSet;
            controlsOriginRight -= offSet;

            //  Fullscreen
            Controls.Add(new MyGuiControlLabel(this, controlsOriginLeft + controlIndex * MyGuiConstants.CONTROLS_DELTA, null, MyTextsWrapperEnum.Fullscreen, MyGuiConstants.LABEL_TEXT_COLOR,
                MyGuiConstants.LABEL_TEXT_SCALE_OPTIONS, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER, MyGuiManager.GetFontMinerWarsBlue()));

            m_fullscreenCheckbox = new MyGuiControlCheckbox(this,
                                 controlsOriginRight + controlIndex++ * MyGuiConstants.CONTROLS_DELTA +
                                 new Vector2(MyGuiConstants.CHECKBOX_WITH_GLOW_SIZE.X / 2.0f, 0), MyGuiConstants.CHECKBOX_WITH_GLOW_SIZE,
                                 MyGuiManager.GetCheckboxOffTexture(), MyGuiManager.GetCheckboxOnTexture(), null,
                                 true, MyGuiConstants.CHECKBOX_BACKGROUND_COLOR, true, null);
            //m_fullscreenCheckbox.OnCheck = OnFullScreenCheck;
            Controls.Add(m_fullscreenCheckbox);

            //  Vertical Sync
            Controls.Add(new MyGuiControlLabel(this, controlsOriginLeft + controlIndex * MyGuiConstants.CONTROLS_DELTA, null, MyTextsWrapperEnum.VerticalSync, MyGuiConstants.LABEL_TEXT_COLOR,
                MyGuiConstants.LABEL_TEXT_SCALE_OPTIONS, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER, MyGuiManager.GetFontMinerWarsBlue()));
            //m_verticalSyncCheckbox = new MyGuiControlCheckbox(this, controlsOriginRight + 3 * MyGuiConstants.CONTROLS_DELTA + new Vector2(MyGuiConstants.CHECKBOX_SIZE.X / 2.0f, 0), true, MyGuiConstants.CHECKBOX_BACKGROUND_COLOR);
            

            m_verticalSyncCheckbox = new MyGuiControlCheckbox(this,
                                 controlsOriginRight + controlIndex++ * MyGuiConstants.CONTROLS_DELTA +
                                 new Vector2(MyGuiConstants.CHECKBOX_WITH_GLOW_SIZE.X / 2.0f, 0), MyGuiConstants.CHECKBOX_WITH_GLOW_SIZE,
                                 MyGuiManager.GetCheckboxOffTexture(), MyGuiManager.GetCheckboxOnTexture(), null,
                                 false, MyGuiConstants.CHECKBOX_BACKGROUND_COLOR, true, null);
            Controls.Add(m_verticalSyncCheckbox);


            //  Hardware Cursor
            Controls.Add(new MyGuiControlLabel(this, controlsOriginLeft + controlIndex * MyGuiConstants.CONTROLS_DELTA, null, MyTextsWrapperEnum.HardwareCursor, MyGuiConstants.LABEL_TEXT_COLOR,
                MyGuiConstants.LABEL_TEXT_SCALE_OPTIONS, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER, MyGuiManager.GetFontMinerWarsBlue()));

            m_hardwareCursorCheckbox = new MyGuiControlCheckbox(this,
                                 controlsOriginRight + controlIndex++ * MyGuiConstants.CONTROLS_DELTA +
                                 new Vector2(MyGuiConstants.CHECKBOX_WITH_GLOW_SIZE.X / 2.0f, 0), MyGuiConstants.CHECKBOX_WITH_GLOW_SIZE,
                                 MyGuiManager.GetCheckboxOffTexture(), MyGuiManager.GetCheckboxOnTexture(), null,
                                 false, MyGuiConstants.CHECKBOX_BACKGROUND_COLOR, true, null);
            Controls.Add(m_hardwareCursorCheckbox);

            //  Render Quality            
            Controls.Add(new MyGuiControlLabel(this, controlsOriginLeft + controlIndex * MyGuiConstants.CONTROLS_DELTA, null, MyTextsWrapperEnum.RenderQuality, MyGuiConstants.LABEL_TEXT_COLOR,
                MyGuiConstants.LABEL_TEXT_SCALE_OPTIONS, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER, MyGuiManager.GetFontMinerWarsBlue()));
            m_renderQualityCombobox = new MyGuiControlCombobox(this, controlsOriginRight + controlIndex++ * MyGuiConstants.CONTROLS_DELTA + new Vector2(MyGuiConstants.COMBOBOX_MEDIUM_SIZE.X / 2.0f, 0),
                MyGuiControlPreDefinedSize.MEDIUM, MyGuiConstants.COMBOBOX_BACKGROUND_COLOR, MyGuiConstants.COMBOBOX_TEXT_SCALE);
            m_renderQualityCombobox.AddItem((int)MyRenderQualityEnum.LOW, MyTextsWrapperEnum.RenderQualityLow);
            m_renderQualityCombobox.AddItem((int)MyRenderQualityEnum.NORMAL, MyTextsWrapperEnum.RenderQualityNormal);
            m_renderQualityCombobox.AddItem((int)MyRenderQualityEnum.HIGH, MyTextsWrapperEnum.RenderQualityHigh);
            m_renderQualityCombobox.AddItem((int)MyRenderQualityEnum.EXTREME, MyTextsWrapperEnum.RenderQualityExtreme);
            Controls.Add(m_renderQualityCombobox);

            // Field of View
            Controls.Add(new MyGuiControlLabel(this, controlsOriginLeft + controlIndex * MyGuiConstants.CONTROLS_DELTA, null, MyTextsWrapperEnum.FieldOfView, MyGuiConstants.LABEL_TEXT_COLOR,
                MyGuiConstants.LABEL_TEXT_SCALE_OPTIONS, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER, MyGuiManager.GetFontMinerWarsBlue()));

            const float fovSliderSize = MyGuiConstants.SLIDER_WIDTH * 1.1f;
            m_fieldOfViewSlider = new MyGuiControlSlider(this, controlsOriginRight + controlIndex * MyGuiConstants.CONTROLS_DELTA + new Vector2(fovSliderSize / 2.0f, 0),
                fovSliderSize, MyConstants.FIELD_OF_VIEW_CONFIG_MIN, MyConstants.FIELD_OF_VIEW_CONFIG_MAX, MyGuiConstants.SLIDER_BACKGROUND_COLOR,
                new StringBuilder(), MyGuiConstants.SLIDER_WIDTH_LABEL, 0, MyGuiConstants.LABEL_TEXT_SCALE_OPTIONS * 0.85f);
            m_fieldOfViewSlider.OnChange = OnFovChanged;
            Controls.Add(m_fieldOfViewSlider);

            m_fieldOfViewLabel = new MyGuiControlLabel(this, controlsOriginRight + controlIndex++ * MyGuiConstants.CONTROLS_DELTA + new Vector2(fovSliderSize * 1.1f, 0),
                null, new StringBuilder("{0:F1}"), MyGuiConstants.LABEL_TEXT_COLOR,
                MyGuiConstants.LABEL_TEXT_SCALE_OPTIONS, MyGuiDrawAlignEnum.HORISONTAL_RIGHT_AND_VERTICAL_CENTER, MyGuiManager.GetFontMinerWarsBlue());
            Controls.Add(m_fieldOfViewLabel);

            m_fieldOfViewDefaultLabel = new MyGuiControlLabel(this, controlsOriginRight + controlIndex++ * MyGuiConstants.CONTROLS_DELTA + new Vector2(0.009f, 0),
                null, MyTextsWrapper.Get(MyTextsWrapperEnum.DefaultFOV), MyGuiConstants.LABEL_TEXT_COLOR,
                MyGuiConstants.LABEL_TEXT_SCALE_OPTIONS, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_BOTTOM, MyGuiManager.GetFontMinerWarsBlue());
            m_fieldOfViewDefaultLabel.UpdateParams(MathHelper.ToDegrees(MyConstants.FIELD_OF_VIEW_CONFIG_DEFAULT));
            Controls.Add(m_fieldOfViewDefaultLabel);

            //MyGuiManager.GetFontMinerWarsBlue()
            //  Brightness
            //Controls.Add(new MyGuiControlLabel(this, controlsOriginLeft + 8 * MyGuiConstants.CONTROLS_DELTA, null, MyTextsWrapperEnum.Brightness, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE_OPTIONS, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));

            //  Contrast
            //Controls.Add(new MyGuiControlLabel(this, controlsOriginLeft + 9 * MyGuiConstants.CONTROLS_DELTA, null, MyTextsWrapperEnum.Contrast, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE_OPTIONS, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            //m_contrastTextbox = new MyGuiControlTextbox(this, controlsOriginRight + 9 * MyGuiConstants.CONTROLS_DELTA + new Vector2(MyGuiConstants.TEXTBOX_WIDTH / 2.0f, 0), MyGuiConstants.TEXTBOX_WIDTH, "Opicka©123456", 20, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE_OPTIONS, MyGuiControlTextboxType.NORMAL);
            //Controls.Add(m_contrastTextbox);

            //  Buttons APPLY and BACK


            var m_okButton = new MyGuiControlButton(this, new Vector2(-0.1379f, 0.2269f), MyGuiConstants.OK_BUTTON_SIZE,
                                   MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                                   MyGuiManager.GetInventoryScreenButtonTexture(), null, null, MyTextsWrapperEnum.Ok,
                                   MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, MyGuiControlButtonTextAlignment.CENTERED, OnApplyClick,
                                   true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true, true);
            Controls.Add(m_okButton);
            //Controls.Add(new MyGuiControlButton(this, new Vector2(-buttonDelta.X, buttonDelta.Y), MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
             //   MyTextsWrapperEnum.Apply, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnApplyClick, MyGuiControlButtonTextAlignment.CENTERED, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));
            //Controls.Add(new MyGuiControlButton(this, new Vector2(+buttonDelta.X, buttonDelta.Y), MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
             //   MyTextsWrapperEnum.Back, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnBackClick, MyGuiControlButtonTextAlignment.CENTERED, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));

            var m_cancelButton = new MyGuiControlButton(this, new Vector2(0.1428f, 0.2269f), MyGuiConstants.OK_BUTTON_SIZE,
                       MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                       MyGuiManager.GetInventoryScreenButtonTexture(), null, null, MyTextsWrapperEnum.Back,
                       MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, MyGuiControlButtonTextAlignment.CENTERED, OnBackClick,
                       true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true, true);
            Controls.Add(m_cancelButton);

            //  Update controls with values from config file
            UpdateFromConfig();
            UpdateControls(m_settingsOld);

            //  Update OLD settings
            UpdateSettings(m_settingsOld);
            UpdateSettings(m_settingsNew);

            MyMwcLog.WriteLine("MyGuiScreenOptionsVideo.ctor END");
        }

        private void UpdateRecommendecAspectRatioLabel(int adapterIndex)
        {
            m_recommendAspectRatioLabel.UpdateText(GetRecommendedAspectRatio(adapterIndex) + " ***");
        }

        void OnVideoAdapterSelected()
        {
            int adapterIndex = m_videoAdapterCombobox.GetSelectedKey();
            AddDisplayModesToComboBox(adapterIndex);
            UpdateRecommendecAspectRatioLabel(adapterIndex);

            m_videoModeCombobox.SelectItemByKey(MyVideoModeManager.GetVideoModeIndexByWidthAndHeight(adapterIndex, m_settingsOld.VideoMode.Width, m_settingsOld.VideoMode.Height));
        }

        void OnVideoModeSelected()
        {
            MyVideoModeEx mode = MyVideoModeManager.GetVideoModeByIndex(m_videoAdapterCombobox.GetSelectedKey(), m_videoModeCombobox.GetSelectedKey());
            m_fieldOfViewSlider.SetValue(MyConstants.FIELD_OF_VIEW_CONFIG_DEFAULT);
            if (mode.AspectRatio >= (12.0 / 3.0))
            {
                m_fieldOfViewSlider.SetBounds(MyConstants.FIELD_OF_VIEW_CONFIG_MIN, MyConstants.FIELD_OF_VIEW_CONFIG_MAX_TRIPLE_HEAD);
            }
            else if (mode.AspectRatio >= (8.0 / 3.0))
            {
                m_fieldOfViewSlider.SetBounds(MyConstants.FIELD_OF_VIEW_CONFIG_MIN, MyConstants.FIELD_OF_VIEW_CONFIG_MAX_DUAL_HEAD);
            }
            else
            {
                m_fieldOfViewSlider.SetBounds(MyConstants.FIELD_OF_VIEW_CONFIG_MIN, MyConstants.FIELD_OF_VIEW_CONFIG_MAX);
            }
        }

        void OnFovChanged(MyGuiControlSlider sender)
        {
            m_fieldOfViewLabel.UpdateParams(MathHelper.ToDegrees(m_fieldOfViewSlider.GetValue()));
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenOptionsVideo";
        }

        void UpdateFromConfig()
        {
            MyMwcLog.WriteLine("MyGuiScreenOptionsVideo.UpdateFromConfig START");

            m_settingsOld.VideoAdapter = MyConfig.VideoAdapter;
            m_settingsOld.VideoMode = MyConfig.VideoMode;
            m_settingsOld.FullScreen = MyConfig.FullScreen;
            m_settingsOld.VerticalSync = MyConfig.VerticalSync;
            m_settingsOld.HardwareCursor = MyConfig.HardwareCursor;
            m_settingsOld.RenderQuality = MyConfig.RenderQuality;
            m_settingsOld.FieldOfView = MyConfig.FieldOfView;

            MyMwcLog.WriteLine("MyGuiScreenOptionsVideo.UpdateFromConfig END");
        }

        bool UpdateSettings(MyGuiScreenOptionsVideoSettings settings)
        {
            bool retval = settings.VideoMode != MyVideoModeManager.GetVideoModeByIndex(m_videoAdapterCombobox.GetSelectedKey(), m_videoModeCombobox.GetSelectedKey()) ||
                settings.VideoAdapter != m_videoAdapterCombobox.GetSelectedKey() ||
                settings.FullScreen != m_fullscreenCheckbox.Checked ||
                settings.VerticalSync != m_verticalSyncCheckbox.Checked ||
                settings.HardwareCursor != m_hardwareCursorCheckbox.Checked ||
                settings.RenderQuality != (MyRenderQualityEnum)m_renderQualityCombobox.GetSelectedKey() ||
                settings.FieldOfView != m_fieldOfViewSlider.GetValue();
            
            settings.VideoAdapter = m_videoAdapterCombobox.GetSelectedKey();
            settings.VideoMode = MyVideoModeManager.GetVideoModeByIndex(settings.VideoAdapter, m_videoModeCombobox.GetSelectedKey());
            settings.FullScreen = m_fullscreenCheckbox.Checked;
            settings.VerticalSync = m_verticalSyncCheckbox.Checked;
            settings.HardwareCursor = m_hardwareCursorCheckbox.Checked;
            settings.RenderQuality = (MyRenderQualityEnum)m_renderQualityCombobox.GetSelectedKey();
            settings.FieldOfView = m_fieldOfViewSlider.GetValue();

            return retval;
        }

        void UpdateControls(MyGuiScreenOptionsVideoSettings settings)
        {
            m_videoAdapterCombobox.SelectItemByKey(settings.VideoAdapter);
            m_videoModeCombobox.SelectItemByKey(MyVideoModeManager.GetVideoModeIndexByWidthAndHeight(settings.VideoAdapter, settings.VideoMode.Width, settings.VideoMode.Height));
            m_fullscreenCheckbox.Checked = settings.FullScreen;
            m_verticalSyncCheckbox.Checked = settings.VerticalSync;
            m_hardwareCursorCheckbox.Checked = settings.HardwareCursor;
            m_renderQualityCombobox.SelectItemByKey((int)settings.RenderQuality);
            m_fieldOfViewSlider.SetValue(settings.FieldOfView);
            OnFovChanged(m_fieldOfViewSlider);
        }

        void SaveSettings()
        {
            MyConfig.VideoAdapter = m_settingsNew.VideoAdapter;
            MyConfig.VideoMode = m_settingsNew.VideoMode;
            MyConfig.FullScreen = m_settingsNew.FullScreen;
            MyConfig.VerticalSync = m_settingsNew.VerticalSync;
            MyConfig.HardwareCursor = m_settingsNew.HardwareCursor;
            MyConfig.RenderQuality = m_settingsNew.RenderQuality;
            MyConfig.FieldOfView = m_settingsNew.FieldOfView;
        }

        public void OnBackClick(MyGuiControlButton sender)
        {
            //  Just close the screen, ignore any change
            CloseScreen();
        }

        public void OnApplyClick(MyGuiControlButton sender)
        {
            //  Update NEW settings
            bool somethingChanged = UpdateSettings(m_settingsNew);

            //  Change video mode to new one
            if (somethingChanged)
            {
                MyVideoModeManager.BeginChangeVideoMode(true, m_settingsNew.VideoAdapter, m_settingsNew.VideoMode, m_settingsNew.FullScreen, m_settingsNew.VerticalSync, m_settingsNew.HardwareCursor, m_settingsNew.RenderQuality, m_settingsNew.FieldOfView, false, OnVideoModeChangedAndComfirm);
            }
            else
            {
                CloseScreen();
            }
        }

        private void OnVideoModeChangedAndComfirm(MinerWars.AppCode.Game.VideoMode.MyVideoModeManager.MyVideoModeChangeOperation result)
        {
            bool changed = MyVideoModeManager.EndChangeVideoMode(result);

            if (changed)
            {
                if (MyVideoModeManager.HasAnythingChanged(result))
                {
                    //udpate screen here
                    m_waitingForConfirmation = true;
                    MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.MESSAGE, MyTextsWrapperEnum.DoYouWantToKeepTheseSettingsXSecondsRemaining,
                        MyTextsWrapperEnum.MessageBoxCaptionPleaseConfirm, MyTextsWrapperEnum.Yes, MyTextsWrapperEnum.No,
                        OnMessageBoxCallback, MyGuiConstants.VIDEO_OPTIONS_CONFIRMATION_TIMEOUT_IN_MILISECONDS));
                }
            }
            else
            {
                MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, MyTextsWrapperEnum.SorryButSelectedSettingsAreNotSupportedByYourHardware,
                    MyTextsWrapperEnum.MessageBoxCaptionError, MyTextsWrapperEnum.Ok, null));

                m_doRevert = true;
            }
        }

        private void OnVideoModeChanged(MinerWars.AppCode.Game.VideoMode.MyVideoModeManager.MyVideoModeChangeOperation result)
        {
            UpdateControls(m_settingsOld);
            UpdateSettings(m_settingsNew);
        }

        public void OnMessageBoxCallback(MyGuiScreenMessageBoxCallbackEnum callbackReturn)
        {
            if (callbackReturn == MyGuiScreenMessageBoxCallbackEnum.YES)
            {
                //  Save current video mode settings
                SaveSettings();
                MyConfig.Save();

                //  These are now OLD settings
                UpdateSettings(m_settingsOld);

                this.CloseScreenNow();
            }
            else
            {
                m_doRevert = true;
                //RevertChanges();
            }

            m_waitingForConfirmation = false;
        }

        //  Revert changes - setting new video resolution must be done from Draw call, because when called
        //  from Update while game isn't active (alt-tabed or minimized) it will fail on weird XNA exceptions
        void RevertChanges()
        {
            //  Revert and change video mode to OLD settings
            MyVideoModeManager.BeginChangeVideoMode(true,
                m_settingsOld.VideoAdapter,
                m_settingsOld.VideoMode,
                m_settingsOld.FullScreen, m_settingsOld.VerticalSync, m_settingsOld.HardwareCursor, m_settingsOld.RenderQuality, m_settingsOld.FieldOfView, false, OnVideoModeChanged);
        }

        public override bool CloseScreen()
        {
            bool ret = base.CloseScreen();

            //  If the screen was closed for whatever reason during we waited for 15secs acknowledgement of changes, we need to revert them (because YES wasn't pressed)
            if ((ret == true) && (m_waitingForConfirmation == true))
            {
                //RevertChanges();
            }

            return ret;
        }

        void AddAdaptersToComboBox()
        {
            int counter = 0;
            foreach (GraphicsAdapter adapter in GraphicsAdapter.Adapters)
            {
                m_videoAdapterCombobox.AddItem(counter++, new StringBuilder(adapter.Name));
            }
        }

        void AddDisplayModesToComboBox(int adapterIndex)
        {
            m_videoModeCombobox.ClearItems();

            int counter = 0;
            foreach (MyVideoModeEx videoMode in MyVideoModeManager.GetAllSupportedVideoModes(adapterIndex))
            {
                m_videoModeCombobox.AddItem(counter++, new StringBuilder(
                    videoMode.Width + " × " + videoMode.Height + string.Format(videoMode.IsRecommended ? " – {0} ***" : " – {0}", MyTextsWrapper.Get(MyAspectRatioExList.Get(videoMode.AspectRatioEnum).TextShort))
                ));
            }
        }

        StringBuilder GetRecommendedAspectRatio(int adapterIndex)
        {
            MyAspectRatioEx recommendedAspectRatio = MyAspectRatioExList.Get(MyAspectRatioExList.GetWindowsDesktopClosestAspectRatio(adapterIndex));

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(MyTextsWrapper.Get(MyTextsWrapperEnum.RecommendedAspectRatio).ToString(), MyTextsWrapper.Get(recommendedAspectRatio.TextShort));
            return sb;
        }

        public override bool Draw(float backgroundFadeAlpha)
        {
            if (base.Draw(backgroundFadeAlpha) == false) return false;

            if (m_doRevert)
            {
                //  Revert changes - setting new video resolution must be done from Draw call, because when called
                //  from Update while game isn't active (alt-tabed or minimized) it will fail on weird XNA exceptions
                RevertChanges();
                m_doRevert = false;
            }

            return true;
        }
    }
}