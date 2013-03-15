using System.Diagnostics;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.World;
using MinerWars.AppCode.Networking;
using System;

namespace MinerWars.AppCode.Game.GUI
{
    class MyGuiScreenUserInfo : MyGuiScreenBase
    {
        private StringBuilder m_dash = new StringBuilder("-");
        public MyGuiScreenUserInfo()
            : base(new Vector2(.5f, .5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, new Vector2(0.6f, 0.8f))
        {
            m_enableBackgroundFade = true;
            m_backgroundTexture = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\BackgroundScreen\\ProfileBackground", flags: TextureFlags.IgnoreQuality);

            // User name
            AddCaption(MyClientServer.LoggedPlayer.UserName, MyGuiConstants.SCREEN_CAPTION_TEXT_COLOR, new Vector2(0, 0.01f));
            AddBackButton();

            // some info in:
            //MyClientServer.LoggedPlayer
            //all other in MyClientServer.LoggedPlayer.AdditionalInfo

            Debug.Assert(m_size != null, "m_size != null");
            var currentLabelPosition = new Vector2(0.0f, -0.35f * m_size.Value.Y);
            var padding = new Vector2(0.005f, 0);

            // Display name
            Controls.Add(new MyGuiControlLabel(this, currentLabelPosition - padding, null, MyTextsWrapperEnum.InfoDisplayName, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_RIGHT_AND_VERTICAL_CENTER));
            Controls.Add(new MyGuiControlLabel(this, currentLabelPosition + padding, null, MyClientServer.LoggedPlayer.GetDisplayName(), MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            
            //currentLabelPosition.Y += 0.03f;

            // E-mail
            //Controls.Add(new MyGuiControlLabel(this, currentLabelPosition - padding, null, MyTextsWrapperEnum.InfoEmail, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_RIGHT_AND_VERTICAL_CENTER));
            //Controls.Add(new MyGuiControlLabel(this, currentLabelPosition + padding, null, MyTextsWrapperEnum.FeatureNotYetImplemented, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));

            currentLabelPosition.Y += 0.03f;

            // Age
            Controls.Add(new MyGuiControlLabel(this, currentLabelPosition - padding, null, MyTextsWrapperEnum.InfoAge, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_RIGHT_AND_VERTICAL_CENTER));
            Controls.Add(new MyGuiControlLabel(this, currentLabelPosition + padding, null, WriteInfo(MyClientServer.LoggedPlayer.AdditionalInfo.Age), MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));

            currentLabelPosition.Y += 0.03f;

            // Gender
            Controls.Add(new MyGuiControlLabel(this, currentLabelPosition - padding, null, MyTextsWrapperEnum.InfoGender, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_RIGHT_AND_VERTICAL_CENTER));
            Controls.Add(new MyGuiControlLabel(this, currentLabelPosition + padding, null, WriteGender(MyClientServer.LoggedPlayer.AdditionalInfo.Gender), MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));

            currentLabelPosition.Y += 0.03f;

            // Registered (register date)
            Controls.Add(new MyGuiControlLabel(this, currentLabelPosition - padding, null, MyTextsWrapperEnum.InfoRegistered, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_RIGHT_AND_VERTICAL_CENTER));
            Controls.Add(new MyGuiControlLabel(this, currentLabelPosition + padding, null, WriteInfo(MyClientServer.LoggedPlayer.AdditionalInfo.Registered), MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));

            currentLabelPosition.Y += 0.03f;

            // Lastweblogin
            Controls.Add(new MyGuiControlLabel(this, currentLabelPosition - padding, null, MyTextsWrapperEnum.InfoWebLogin, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_RIGHT_AND_VERTICAL_CENTER));
            Controls.Add(new MyGuiControlLabel(this, currentLabelPosition + padding, null, WriteInfo(MyClientServer.LoggedPlayer.AdditionalInfo.LastWebsiteLogin), MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));

            currentLabelPosition.Y += 0.03f;

            // Lastgamelogin (previous)
            Controls.Add(new MyGuiControlLabel(this, currentLabelPosition - padding, null, MyTextsWrapperEnum.InfoGameLogin, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_RIGHT_AND_VERTICAL_CENTER));
            Controls.Add(new MyGuiControlLabel(this, currentLabelPosition + padding, null, WriteInfo(MyClientServer.LoggedPlayer.AdditionalInfo.LastGameLogin), MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));

            currentLabelPosition.Y += 0.03f;

            // Forum posts
            Controls.Add(new MyGuiControlLabel(this, currentLabelPosition - padding, null, MyTextsWrapperEnum.InfoForumPosts, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_RIGHT_AND_VERTICAL_CENTER));
            Controls.Add(new MyGuiControlLabel(this, currentLabelPosition + padding, null, WriteInfo(MyClientServer.LoggedPlayer.AdditionalInfo.ForumPosts), MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));

            currentLabelPosition.Y += 0.03f;

            // Affiliate URL Registrations
            Controls.Add(new MyGuiControlLabel(this, currentLabelPosition - padding, null, MyTextsWrapperEnum.InfoAffReg, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_RIGHT_AND_VERTICAL_CENTER));
            Controls.Add(new MyGuiControlLabel(this, currentLabelPosition + padding, null, WriteInfo(MyClientServer.LoggedPlayer.AdditionalInfo.AffiliateUrlRegistrations), MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));

            currentLabelPosition.Y += 0.03f;

            // Affiliate URL Unique Clicks
            Controls.Add(new MyGuiControlLabel(this, currentLabelPosition - padding, null, MyTextsWrapperEnum.InfoAffClick, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_RIGHT_AND_VERTICAL_CENTER));
            Controls.Add(new MyGuiControlLabel(this, currentLabelPosition + padding, null, WriteInfo(MyClientServer.LoggedPlayer.AdditionalInfo.AffiliateUniqueClicks), MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));

            currentLabelPosition.Y += 0.03f;

            // Contributions
            Controls.Add(new MyGuiControlLabel(this, currentLabelPosition - padding, null, MyTextsWrapperEnum.InfoContributions, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_RIGHT_AND_VERTICAL_CENTER));
            Controls.Add(new MyGuiControlLabel(this, currentLabelPosition + padding, null, WriteInfo(MyClientServer.LoggedPlayer.AdditionalInfo.Contributions, "$"), MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));

            currentLabelPosition.Y += 0.03f;

            // Qualification Points
            Controls.Add(new MyGuiControlLabel(this, currentLabelPosition - padding, null, MyTextsWrapperEnum.InfoQualificationPoints, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_RIGHT_AND_VERTICAL_CENTER));
            Controls.Add(new MyGuiControlLabel(this, currentLabelPosition + padding, null, WriteInfo(MyClientServer.LoggedPlayer.AdditionalInfo.QualificationPoinsts), MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));

            currentLabelPosition.Y += 0.03f;

            // Description
            Controls.Add(new MyGuiControlLabel(this, currentLabelPosition - padding, null, MyTextsWrapperEnum.InfoDescription, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_RIGHT_AND_VERTICAL_CENTER));
            Controls.Add(new MyGuiControlLabel(this, currentLabelPosition + padding, null, WriteInfoTrim(MyClientServer.LoggedPlayer.AdditionalInfo.Description), MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));

            currentLabelPosition.Y += 0.03f;

            // Secrets found
            Controls.Add(new MyGuiControlLabel(this, currentLabelPosition - padding, null, MyTextsWrapperEnum.SecretRoomsFound, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_RIGHT_AND_VERTICAL_CENTER));
            Controls.Add(new MyGuiControlLabel(this, currentLabelPosition + padding, null, WriteInfo(MySteamStats.GetStatInt(MySteamStatNames.FoundSecrets)), MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
        }

        private StringBuilder WriteInfoTrim<T>(T info, string pre = "", string post = "")
        {
            if (info == null)
            {
                return m_dash;
            }

            string text = info.ToString();
            var splits = text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            if (splits.Length > 0)
            {
                text = splits[0];
            }
            if (text.Length > 30)
            {
                text = text.Substring(0, 30);
            }

            return new StringBuilder(pre + text + post);
        }

        private StringBuilder WriteInfo<T>(T info, string pre = "", string post = "")
        {
            return info == null ? m_dash : new StringBuilder(pre + info.ToString() + post);
        }

        private StringBuilder WriteGender(int? gender)
        {
            if (gender.HasValue)
            {
                if (gender.Value == 1)
                {
                    return MyTextsWrapper.Get(MyTextsWrapperEnum.Male);
                }
                else if (gender.Value == 2)
                {
                    return MyTextsWrapper.Get(MyTextsWrapperEnum.Female);
                }
            }
            return m_dash;
        }

        void AddBackButton()
        {
            var position = new Vector2(
                0.0f,
                m_size.Value.Y / 2.0f - MyGuiConstants.MESSAGE_BOX_BORDER_AREA_Y -
                MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE.Y / 2.0f - 0.025f);

            Controls.Add(
                new MyGuiControlButton(
                    this,
                    position,
                    MyGuiConstants.BACK_BUTTON_SIZE,
                    MyGuiConstants.BACK_BUTTON_BACKGROUND_COLOR,
                    MyTextsWrapperEnum.Back,
                    MyGuiConstants.BACK_BUTTON_TEXT_COLOR,
                    MyGuiConstants.BACK_BUTTON_TEXT_SCALE,
                    OnBackClick,
                    MyGuiControlButtonTextAlignment.CENTERED,
                    true,
                    MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER,
                    true));
        }

        void OnBackClick(MyGuiControlButton sender)
        {
            this.CloseScreen();
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenUserInfo";
        }
    }
}