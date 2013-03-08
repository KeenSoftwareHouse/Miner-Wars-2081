using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Utils;
using Microsoft.Xna.Framework;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Managers.Graphics.Textures;
using MinerWars.AppCode.App;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using System.Diagnostics;
using MinerWars.AppCode.Networking.SectorService;
using System.ServiceModel;
using MinerWars.CommonLIB.AppCode.Networking.Services;
using MinerWars.AppCode.Game.Managers.Session;

namespace MinerWars.AppCode.Game.GUI
{
    class MyGuiScreenLoadCheckpoint : MyGuiScreenBase
    {
        private MyGuiControlListbox m_listbox;

        public MyGuiScreenLoadCheckpoint()
            : base(new Vector2(0.5f, 0.5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, null)
        {
            m_enableBackgroundFade = true;
            m_size = new Vector2(0.59f, 0.68544f);

            m_backgroundTexture = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\BackgroundScreen\\VideoBackground", flags: TextureFlags.IgnoreQuality);
            AddCaption(MyTextsWrapperEnum.LoadCheckpoint);

            Vector2 controlsOriginLeft = new Vector2(-m_size.Value.X / 2.0f + 0.05f, -m_size.Value.Y / 2.0f + 0.145f);
            Vector2 controlsOriginRight = new Vector2(-m_size.Value.X / 2.0f + 0.185f, -m_size.Value.Y / 2.0f + 0.145f);
            controlsOriginRight.X += 0.04f;

            /*
            // Checkpoint list
            m_listbox = new MyGuiControlListbox(this, new Vector2(-MyGuiConstants.LISTBOX_SCROLLBAR_WIDTH/2, -0.015f),
                MyGuiConstants.LISTBOX_LONGMEDIUM_SIZE, Vector4.Zero, null, 0.5f, 1, 18, 1, false, true, false);
            */

            m_listbox = new MyGuiControlListbox(this,
                                      new Vector2(0.0055f, -0.0322f),
                                      new Vector2(0.45f, 50 / 1200f),
                                      Vector4.One*0.5f,
                                      null,
                                      0.8f,
                                      1, 10, 1, false, true, false,
                                      null, null, MyGuiManager.GetScrollbarSlider(), null,
                                      1, 1, Vector4.One*0.1f, 0, 0, 0, 0, 0, 0, -0.007f);



            m_listbox.ItemDoubleClick += OnListboxItemDoubleClick;
            Controls.Add(m_listbox);


            Vector2 buttonSize = 0.75f*MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE;
            // Buttons (Load, Rename, Delete, Back)
            Vector2 buttonOrigin = new Vector2(-buttonSize.X * 1.5f - 0.001f, m_size.Value.Y / 2.0f - MyGuiConstants.MESSAGE_BOX_BORDER_AREA_Y - buttonSize.Y / 2.0f - 0.025f);
            Vector2 buttonDelta = new Vector2(buttonSize.X + 0.001f, 0);
            Controls.Add(new MyGuiControlButton(this, buttonOrigin + buttonDelta * 0,buttonSize, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.Load, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE*0.8f, OnLoadClick, MyGuiControlButtonTextAlignment.CENTERED, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));
            Controls.Add(new MyGuiControlButton(this, buttonOrigin + buttonDelta * 1, buttonSize, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.Rename, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE * 0.8f, OnRenameClick, MyGuiControlButtonTextAlignment.CENTERED, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));
            Controls.Add(new MyGuiControlButton(this, buttonOrigin + buttonDelta * 2, buttonSize, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.Delete, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE * 0.8f, OnDeleteClick, MyGuiControlButtonTextAlignment.CENTERED, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));
            Controls.Add(new MyGuiControlButton(this, buttonOrigin + buttonDelta * 3, buttonSize, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.Back, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE * 0.8f, OnBackClick, MyGuiControlButtonTextAlignment.CENTERED, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));

            FillList();
        }

        void FillList()
        {
            Func<IAsyncResult> begin = () =>
            {
                var client = MySectorServiceClient.GetCheckedInstance();
                return client.BeginLoadTemplateCheckpointNames(null, client);
            };

            Action<IAsyncResult, MyGuiScreenProgressAsync> end = (result, screen) =>
            {
                try
                {
                    var client = (MySectorServiceClient)result.AsyncState;
                    var info = client.EndLoadTemplateCheckpointNames(result);
                    InsertItems(info);
                }
                catch (Exception)
                {
                    MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, MyTextsWrapperEnum.PleaseTryAgain, MyTextsWrapperEnum.MessageBoxNetworkErrorCaption, MyTextsWrapperEnum.Ok, null));
                }
                screen.CloseScreenNow();
            };

            var progress = new MyGuiScreenProgressAsync(MyTextsWrapperEnum.LoadSectorIdentifiersPleaseWait, false, begin, end);
            MyGuiManager.AddScreen(progress);
        }

        private void InsertItems(List<MyCheckpointInfo> info)
        {
            m_listbox.DeselectAll();
            m_listbox.RemoveAllRows();
            m_listbox.RemoveAllItems();
            int index = 0;
            for (int ind = 0; ind < info.Count; ind++)
            {
                var i = info[ind];
                if (i.CheckpointName != null)
                {
                    m_listbox.AddItem(index, new StringBuilder(i.CheckpointName));
                    index++;
                }
            }
        }
        public override string GetFriendlyName()
        {
            return "MyGuiScreenLoadCheckpoint";
        }

        public void OnLoadClick(MyGuiControlButton sender)
        {
            LoadCheckpoint();
        }

        public void OnRenameClick(MyGuiControlButton sender)
        {
            var item = m_listbox.GetSelectedItem();
            if (item != null)
            {
                MyGuiManager.AddScreen(new MyGuiScreenRenameCheckpoint(GetCheckpointNameFromItem(item), RenameTemplate));
            }
        }

        void RenameTemplate(string oldName, string newName, MyGuiScreenBase renameScreen)
        {
            renameScreen.CloseScreenNow();

            Func<IAsyncResult> begin = () =>
            {
                var client = MySectorServiceClient.GetCheckedInstance();
                return client.BeginRenameTemplateCheckpoint(oldName, newName, null, client);
            };

            Action<IAsyncResult, MyGuiScreenProgressAsync> end = (result, screen) =>
            {
                try
                {
                    var client = (MySectorServiceClient)result.AsyncState;
                    client.EndRenameTemplateCheckpoint(result);
                }
                catch (FaultException<MyCheckpointRenameFault> e)
                {
                    switch (e.Detail.Reason)
                    {
                        case MyCheckpointRenameFaultReason.CheckpointNotFound:
                            MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, MyTextsWrapperEnum.SectorNotFound, MyTextsWrapperEnum.MessageBoxTimeoutCaption, MyTextsWrapperEnum.Ok, null));
                            break;

                        case MyCheckpointRenameFaultReason.NameAlreadyExists:
                            MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, MyTextsWrapperEnum.CantSaveSectorNameExists, MyTextsWrapperEnum.MessageBoxTimeoutCaption, MyTextsWrapperEnum.Ok, null));
                            break;
                    }
                }
                catch (Exception)
                {
                    MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, MyTextsWrapperEnum.PleaseTryAgain, MyTextsWrapperEnum.MessageBoxNetworkErrorCaption, MyTextsWrapperEnum.Ok, null));
                }
                screen.CloseScreenNow();
                FillList();
            };

            var progress = new MyGuiScreenProgressAsync(MyTextsWrapperEnum.LoadSectorIdentifiersPleaseWait, false, begin, end);
            MyGuiManager.AddScreen(progress);            
        }

        public void OnDeleteClick(MyGuiControlButton sender)
        {
            var item = m_listbox.GetSelectedItem();
            if (item != null)
            {
                string name = GetCheckpointNameFromItem(item);
                DeleteTemplate(name);
            }
        }

        private void DeleteTemplate(string name)
        {
            Func<IAsyncResult> begin = () =>
            {
                var client = MySectorServiceClient.GetCheckedInstance();
                return client.BeginDeleteTemplateCheckpoint(name, null, client);
            };

            Action<IAsyncResult, MyGuiScreenProgressAsync> end = (result, screen) =>
            {
                try
                {
                    var client = (MySectorServiceClient)result.AsyncState;
                    client.EndDeleteTemplateCheckpoint(result);
                }
                catch (Exception)
                {
                    MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, MyTextsWrapperEnum.PleaseTryAgain, MyTextsWrapperEnum.MessageBoxNetworkErrorCaption, MyTextsWrapperEnum.Ok, null));
                }
                screen.CloseScreenNow();
                FillList();
            };

            var progress = new MyGuiScreenProgressAsync(MyTextsWrapperEnum.LoadSectorIdentifiersPleaseWait, false, begin, end);
            MyGuiManager.AddScreen(progress);
        }

        public void OnBackClick(MyGuiControlButton sender)
        {
            CloseScreen();
        }

        void OnListboxItemDoubleClick(object sender, MyGuiControlListboxItemEventArgs eventArgs)
        {
            LoadCheckpoint();
        }

        string GetCheckpointNameFromItem(MyGuiControlListboxItem item)
        {
            return item.Value.ToString();
        }

        private void LoadCheckpoint()
        {
            var item = m_listbox.GetSelectedItem();
            if (item != null)
            {
                MyGuiManager.CloseAllScreensExcept(MyGuiScreenGamePlay.Static);

                MySession.StartTemplateCheckpoint(GetCheckpointNameFromItem(item));
            }
        }
    }
}
