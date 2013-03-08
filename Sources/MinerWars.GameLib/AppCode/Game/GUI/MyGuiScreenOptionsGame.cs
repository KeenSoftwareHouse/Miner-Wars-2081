using MinerWarsMath;
using MinerWarsMath.Graphics;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.HUD;
using MinerWars.AppCode.Game.Missions;
using MinerWars.AppCode.Game.GUI.Helpers;

namespace MinerWars.AppCode.Game.GUI
{
    class MyGuiScreenOptionsGame : MyGuiScreenBase
    {
        MyGuiControlCombobox m_languageCombobox;
        MyGuiControlCheckbox m_subtitlesCheckbox;
        MyGuiControlCheckbox m_notificationsCheckbox;
        MyGuiControlCheckbox m_disableQuickZoomCheckbox;
        MyGuiControlCheckbox m_disableZoomCheckbox;

        public MyGuiScreenOptionsGame()
            : base(new Vector2(0.5f, 0.5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, null)
        {
            m_enableBackgroundFade = true;
            m_size = new Vector2(0.51f, 0.525f);
            //m_backgroundTexture = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GameOptionsBackground", flags: TextureFlags.IgnoreQuality);

            AddCaption(MyTextsWrapperEnum.GameOptions, new Vector2(0, 0.005f));

            Vector2 controlsOriginLeft = new Vector2(-m_size.Value.X / 2.0f + 0.05f, -m_size.Value.Y / 2.0f + 0.125f);
            Vector2 controlsOriginRight = new Vector2(-m_size.Value.X / 2.0f + 0.175f, -m_size.Value.Y / 2.0f + 0.125f);
            Vector2 controlsDelta = new Vector2(0, 0.0525f);

            //  Language
            Controls.Add(new MyGuiControlLabel(this, controlsOriginLeft + 0 * controlsDelta, null, MyTextsWrapperEnum.Language, MyGuiConstants.LABEL_TEXT_COLOR, 
                MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            m_languageCombobox = new MyGuiControlCombobox(this, controlsOriginRight + 0 * controlsDelta + new Vector2(MyGuiConstants.COMBOBOX_MEDIUM_SIZE.Y / 2.0f + 0.1f, 0), MyGuiControlPreDefinedSize.MEDIUM, MyGuiConstants.COMBOBOX_BACKGROUND_COLOR, MyGuiConstants.COMBOBOX_TEXT_SCALE);
            m_languageCombobox.AddItem((int)MyLanguagesEnum.English, MyTextsWrapperEnum.LanguageEnglish);
            m_languageCombobox.AddItem((int)MyLanguagesEnum.Cesky, MyTextsWrapperEnum.LanguageCesky);
            Controls.Add(m_languageCombobox);

            //  Subtitles
            Controls.Add(new MyGuiControlLabel(this, controlsOriginLeft + 1 * controlsDelta, null, MyTextsWrapperEnum.Subtitles, MyGuiConstants.LABEL_TEXT_COLOR, 
                MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            m_subtitlesCheckbox = new MyGuiControlCheckbox(this, controlsOriginRight + 1 * controlsDelta + new Vector2(MyGuiConstants.CHECKBOX_SIZE.X / 2.0f + 0.1f, 0), true, MyGuiConstants.CHECKBOX_BACKGROUND_COLOR);
            Controls.Add(m_subtitlesCheckbox);

            //  Notifications
            Controls.Add(new MyGuiControlLabel(this, controlsOriginLeft + 2 * controlsDelta, null, MyTextsWrapperEnum.Notifications, MyGuiConstants.LABEL_TEXT_COLOR,
                MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            m_notificationsCheckbox = new MyGuiControlCheckbox(this, controlsOriginRight + 2 * controlsDelta + new Vector2(MyGuiConstants.CHECKBOX_SIZE.X / 2.0f + 0.1f, 0), true, MyGuiConstants.CHECKBOX_BACKGROUND_COLOR);
            Controls.Add(m_notificationsCheckbox);
            
            //  Buttons OK and CANCEL
            Vector2 buttonDelta = new Vector2(0.1f, m_size.Value.Y / 2.0f - MyGuiConstants.MESSAGE_BOX_BORDER_AREA_Y - MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE.Y / 2.0f - 0.03f);
            Controls.Add(new MyGuiControlButton(this, new Vector2(-buttonDelta.X, buttonDelta.Y), MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.Ok, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnOkClick, MyGuiControlButtonTextAlignment.CENTERED, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));
            Controls.Add(new MyGuiControlButton(this, new Vector2(+buttonDelta.X, buttonDelta.Y), MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.Cancel, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnCancelClick, MyGuiControlButtonTextAlignment.CENTERED, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));


            //  Update controls with values from config file
            UpdateControls();
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenOptionsGame";
        }

        void UpdateControls()
        {
            m_languageCombobox.SelectItemByKey((int)MyConfig.Language);
            m_subtitlesCheckbox.Checked = MyConfig.Subtitles;
            m_notificationsCheckbox.Checked = MyConfig.Notifications;
        }

        void DoChanges()
        {
            MyTextsWrapper.ActualLanguage = (MyLanguagesEnum)m_languageCombobox.GetSelectedKey();
            MyDialoguesWrapper.ActualLanguage = (MyLanguagesEnum)m_languageCombobox.GetSelectedKey();
            MyMissions.ReloadTexts();
            MyGuiObjectBuilderHelpers.ReloadTexts();
            MyGuiManager.RecreateControls();
            MyHudNotification.ReloadTexts();
            foreach (var entity in MinerWars.AppCode.Game.Entities.MyEntities.GetEntities())
            {
                entity.UpdateHudMarker(true);
                var cont = entity as MinerWars.AppCode.Game.Entities.MyPrefabContainer;
                if (cont != null)
                {
                    foreach (var prefab in cont.GetPrefabs())
                    {
                        prefab.UpdateHudMarker(true);
                    }
                }
            }

            MySubtitles.Enabled = m_subtitlesCheckbox.Checked;
            MyConfig.Notifications = m_notificationsCheckbox.Checked;
            MyConfig.Save();
        }

        public void OnCancelClick(MyGuiControlButton sender)
        {
            //  Just close the screen, ignore any change
            CloseScreen();
        }

        public void OnOkClick(MyGuiControlButton sender)
        {
            //  Save/update and then close screen
            DoChanges();
            CloseScreen();
        }
    }
}
