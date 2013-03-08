using System.Collections.Generic;
using MinerWarsMath;
using MinerWars.AppCode.Game.Editor;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.World;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.AppCode.Game.Managers.Session;
using System.IO;
using SysUtils.Utils;
using MinerWars.AppCode.Networking.SectorService;
using System;
using System.ServiceModel;
using System.Text;
using SysUtils;
using MinerWars.CommonLIB.AppCode.Networking.Services;
using MinerWars.CommonLIB.AppCode.Utils;
using KeenSoftwareHouse.Library.Trace;
using System.Linq;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.AppCode.Networking;

//  IMPORTANT: THIS SCREEN CAN'T BE CANCELED BY ESC OR CANCEL BUTTON BECAUSE THAT WOULD INTERFERE THE PROCESS ON SERVER

namespace MinerWars.AppCode.Game.GUI
{
    class MyGuiScreenEditorSaveProgress : MyGuiScreenSectorServiceCallProgress
    {
        MyMwcSectorIdentifier m_sectorIdentifier;
        MyMwcObjectBuilder_Checkpoint m_checkpoint;
        bool m_savePlayer;
        bool m_forceHide;
        bool m_pause;

        public MyGuiScreenEditorSaveProgress(MyMwcSectorIdentifier sectorIdentifier, MyMwcObjectBuilder_Checkpoint checkpoint, bool savePlayer, bool visibleSave = true, bool pause = false)
            : base(MyTextsWrapperEnum.SaveSectorInProgressPleaseWait, false, TimeSpan.FromSeconds(120)) // Enought time to save sector - full of asteroids, etc
        {
            m_checkpoint = checkpoint;
            m_sectorIdentifier = sectorIdentifier;
            m_savePlayer = savePlayer;
            m_forceHide = !visibleSave;
            m_pause = pause;
        }

        public event ScreenHandler Saved;
        public event ScreenHandler SaveFailed;

        public override string GetFriendlyName()
        {
            return "MyGuiScreenEditorSaveProgress";
        }

        protected override void ServiceProgressStart(MySectorServiceClient client)
        {
            if (!MySession.PlayerShip.IsDead())
            {
                SaveSector(client);
            }
            else
            {
                this.CloseScreen();
            }
        }

        private void SaveSector(MySectorServiceClient client)
        {
            if (m_pause)
            {
                if (!MinerWars.AppCode.App.MyMinerGame.IsPaused())
                {
                    MinerWars.AppCode.App.MyMinerGame.SwitchPause();
                }
            }

            m_checkpoint.CurrentSector = m_sectorIdentifier;

            //if (MyLocalCache.CanBeSaved(m_checkpoint))
            //{
            //    MyLocalCache.Save(m_checkpoint);
            //    m_checkpoint.SectorObjectBuilder = null;
            //}

            AddAction(client.BeginSaveCheckpoint(m_checkpoint.ToBytes(), m_savePlayer, null, client));
        }

        protected override void OnActionCompleted(IAsyncResult asyncResult, MySectorServiceClient client)
        {
            try
            {                
                client.EndSaveCheckpoint(asyncResult);
                MyEditorActions.ResetActionHistory();

                var handler = Saved;
                if (handler != null)
                {
                    handler(this);
                }
            }
            catch (FaultException<MyCustomFault> faultException)
            {
                if (faultException.Detail.ErrorCode == MyCustomFaultCode.SectorNameExists)
                {
                    MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, MyTextsWrapperEnum.CantSaveSectorNameExists, MyTextsWrapperEnum.MessageBoxCaptionError, MyTextsWrapperEnum.Ok, null));
                }
                else
                {
                    throw faultException; // Causes parent to call OnError
                }
            }

            CloseScreen();
        }

        // Override default behaviour - default behaviour logs exception, aborts client and closes screen
        protected override void OnError(Exception exception, MySectorServiceClient client)
        {
            MySectorServiceClient.SafeClose();
            MyMwcLog.WriteLine(exception); // log exception

            ShowRetryDialog();
            CloseScreen();
        }

        private void ShowRetryDialog(string message = null, string caption = null)
        {
            var messageBoxMessage = MyTextsWrapper.Get(MyTextsWrapperEnum.CantSaveSectorFailed);
            var messageBoxCaption = MyTextsWrapper.Get(MyTextsWrapperEnum.MessageBoxCaptionError);

            if (m_pause)
            {
                if (!MinerWars.AppCode.App.MyMinerGame.IsPaused())
                {
                    MinerWars.AppCode.App.MyMinerGame.SwitchPause();
                }
            }

            Vector2 buttonSize = new Vector2(MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE.X * 2.4f, MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE.Y);
            var messageBoxScreen = new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, MyMessageBoxButtonsType.YES_NO, messageBoxMessage, messageBoxCaption, null, MyTextsWrapperEnum.ButtonRetry,
                    MyTextsWrapperEnum.ButtonContinueWithoutSaving, MessageBoxCallback, false, buttonSize);
            
            MyGuiManager.AddScreen(messageBoxScreen);
        }

        private void MessageBoxCallback(MyGuiScreenMessageBoxCallbackEnum callbackReturn)
        {
            // retry
            if (callbackReturn == MyGuiScreenMessageBoxCallbackEnum.YES)
            {
                var newSaveScreen = new MyGuiScreenEditorSaveProgress(m_sectorIdentifier, m_checkpoint, m_savePlayer, true, true);
                newSaveScreen.Saved = this.Saved;
                newSaveScreen.SaveFailed = this.SaveFailed;
                MyGuiManager.AddScreen(newSaveScreen);
            }
            // continue without saving
            else if (callbackReturn == MyGuiScreenMessageBoxCallbackEnum.NO)
            {
                var handler = SaveFailed;
                if (handler != null)
                {
                    handler(this);
                }
            }
        }

        public override bool Update(bool hasFocus)
        {
            if (base.Update(hasFocus) == false) return false;

            if (m_forceHide && m_state != MyGuiScreenState.HIDING && m_state != MyGuiScreenState.HIDDEN)
            {
                HideScreen();
            }

            return GetState() == MyGuiScreenState.OPENED;
        }

        public override bool CloseScreen()
        {
            if (m_pause)
            {
                if (MinerWars.AppCode.App.MyMinerGame.IsPaused())
                {
                    MinerWars.AppCode.App.MyMinerGame.SwitchPause();
                }
            }
            return base.CloseScreen();
        }

        public override int GetTransitionOpeningTime()
        {
            return 0;
        }

        public override int GetTransitionClosingTime()
        {
            return 0;
        }

        public override bool HideScreen()
        {
            var hidden = base.HideScreen();
            m_forceHide = true;
            return hidden;
        }

        public override bool Draw(float backgroundFadeAlpha)
        {
            if (m_forceHide)
                return true;

            return base.Draw(backgroundFadeAlpha);
        }
    }
}
