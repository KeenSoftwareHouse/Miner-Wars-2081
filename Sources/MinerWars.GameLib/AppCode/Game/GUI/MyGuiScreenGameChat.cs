using System;
using System.Text;
using System.Collections.Generic;
using MinerWarsMath;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.SolarSystem;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.World;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.AppCode.Game.Sessions;
using MinerWars.CommonLIB.AppCode.Networking.Multiplayer;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.World.Global;
using MinerWars.AppCode.Toolkit.Input;

namespace MinerWars.AppCode.Game.GUI
{

    class MyGuiScreenGameChat : MyGuiScreenBase
    {
        MyGuiControlTextbox m_textbox;
        bool m_teamOnly;
        MyTexture2D m_panelTexture;
        StringBuilder m_stringBuilderForText;
        Vector2 m_positionOffset;

        public MyGuiScreenGameChat(bool sendToTeam)
            : base(Vector2.Zero, null, null)
        {
            m_closeOnEsc = true;
            m_enableBackgroundFade = true;
            m_teamOnly = sendToTeam;

            m_positionOffset = new Vector2(MyGuiManager.GetNormalizedCoordinateFromScreenCoordinate_FULLSCREEN(new Vector2(0, 0)).X - 920/1920f/2 - 0.04f, -0.2f);

            m_panelTexture = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\BackgroundScreen\\ChatBackground", flags: TextureFlags.IgnoreQuality);

            MyGuiControlPanel panel = new MyGuiControlPanel(this, new Vector2(0.5f, 0.85f) + m_positionOffset, new Vector2(920 / 1920f, 388 / 1200f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, m_panelTexture, null, null, null, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_TOP);
            Controls.Add(panel);

            Vector2 menuPositionOrigin = new Vector2(0.423f, 0.939f) + m_positionOffset;
            Vector2 delta = new Vector2(0.17f, 0f);
            const MyGuiControlButtonTextAlignment menuButtonTextAlignement = MyGuiControlButtonTextAlignment.CENTERED;

            m_textbox = new MyGuiControlTextbox(this, menuPositionOrigin, MyGuiControlPreDefinedSize.MEDIUM, "", MyGuiConstants.CHAT_WINDOW_MAX_MESSAGE_LENGTH, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE * 0.75f, MyGuiControlTextboxType.NORMAL, true, false);
            Controls.Add(m_textbox);

            Controls.Add(new MyGuiControlButton(this, menuPositionOrigin + delta,
                MyGuiConstants.MAIN_MENU_BUTTON_SIZE * new Vector2(0.41f, 0.78f), MyGuiConstants.BUTTON_BACKGROUND_COLOR, MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\ButtonChatEnter", flags: TextureFlags.IgnoreQuality), null, null,
                sendToTeam ? MyTextsWrapperEnum.SendToTeam : MyTextsWrapperEnum.SendToAll, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE * 0.65f,
                menuButtonTextAlignement, OnSendClick, true, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER,
                true, true, MyGuiControlHighlightType.WHEN_ACTIVE));
        }



        public override string GetFriendlyName()
        {
            return "MyGuiScreenGameChat";
        }

        public void OnSendClick(MyGuiControlButton sender)
        {
            if (m_textbox.Text != "")
            {
                if (MyMultiplayerGameplay.IsRunning)
                {
                    if (m_teamOnly)
                    {
                        m_textbox.Text = MyTextsWrapper.GetFormatString(MyTextsWrapperEnum.MPChatTeamMessagePrefix) + m_textbox.Text;
                        MyMultiplayerGameplay.Static.SendChatMessageToTeam(m_textbox.Text, MySession.Static.Player.Faction);
                    }
                    else
                    {
                        MyMultiplayerGameplay.Static.SendChatMessage(m_textbox.Text);
                    }
                }

                MyGuiScreenGamePlay.Static.AddChatMessage(MyClientServer.LoggedPlayer.GetUserId(), m_textbox.Text);
            }
            this.CloseScreen();
        }

        public override void HandleInput(MyGuiInput input, bool receivedFocusInThisUpdate)
        {
            base.HandleInput(input, receivedFocusInThisUpdate);

            if (input.IsNewKeyPress(Keys.Enter))
            {
                OnSendClick(null);
            }
        }

        public override bool Draw(float backgroundFadeAlpha)
        {
            bool retval = base.Draw(backgroundFadeAlpha);
            DrawChetArea();
            return retval;
        }

        private void DrawChetArea()
        {
            List<MyChatMessage> messages = MyGuiScreenGamePlay.Static.GetChatMessages(8);
            if (messages == null) return;

            if (m_stringBuilderForText == null)
            {
                m_stringBuilderForText = new StringBuilder(MyGuiConstants.CHAT_WINDOW_MAX_MESSAGE_LENGTH);
            }
            else { m_stringBuilderForText.Clear(); }

            int visibleCount = messages.Count;

            if (visibleCount == 0) return;

            // row size
            float rowSize = 0;
            m_stringBuilderForText.Append(messages[0].SenderName.ToString());
            m_stringBuilderForText.Append(": ");
            m_stringBuilderForText.Append(messages[0].Message);
            Vector2 textSize = MyGuiManager.GetNormalizedSize(GetSenderFont(messages[0].SenderRelation), m_stringBuilderForText, MyGuiConstants.CHAT_WINDOW_MESSAGE_SCALE);
            rowSize = textSize.Y;
            m_stringBuilderForText.Clear();

            MyGuiManager.BeginSpriteBatch();
            var offset = new Vector2(VideoMode.MyVideoModeManager.IsTripleHead() ? -1 : 0, 0);
            {
                // Draw texts
                Vector2 notificationPosition = new Vector2(0.305f, 0.756f) + m_positionOffset;
                for (int i = 0; i < visibleCount; i++)
                {
                    m_stringBuilderForText.Append(messages[i].SenderName.ToString());
                    m_stringBuilderForText.Append(": ");
                    MyRectangle2D size = MyGuiManager.DrawString(GetSenderFont(messages[i].SenderRelation), m_stringBuilderForText, notificationPosition + offset,
                        MyGuiConstants.CHAT_WINDOW_MESSAGE_SCALE, MyGuiConstants.CHAT_WINDOW_TEXT_COLOR, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);

                    notificationPosition.X += size.Size.X;
                    MyGuiManager.DrawString(GetSenderFont(messages[i].SenderRelation), messages[i].Message, notificationPosition + offset,
                        MyGuiConstants.CHAT_WINDOW_MESSAGE_SCALE, MyGuiConstants.CHAT_WINDOW_TEXT_COLOR, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);
                    notificationPosition.Y += rowSize;
                    notificationPosition.X -= size.Size.X;
                    m_stringBuilderForText.Clear();
                }
            }
            MyGuiManager.EndSpriteBatch();
        }

        private MyGuiFont GetSenderFont(MyFactionRelationEnum senderRelation)
        {
            return MyGuiScreenGamePlay.Static.GetSenderFont(senderRelation);
        }
    }
}