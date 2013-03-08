using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Networking.SectorService;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.GUI.Core;
using SysUtils.Utils;
using MinerWarsMath;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.HUD;
using System.Diagnostics;

namespace MinerWars.AppCode.Game
{
    class MyServerAction : MyGuiScreenSectorServiceCallProgress
    {
        public MyServerAction(MyTextsWrapperEnum progressText)
            : base(progressText, false)
        {
        }

        public MyServerAction(MyTextsWrapperEnum progressText, TimeSpan operationTimeout)
            : base(progressText, false, operationTimeout)
        {
        }

        public MyTextsWrapperEnum RetryCaptionText;
        public MyTextsWrapperEnum RetryMessageText;
        public MyTextsWrapperEnum RetryButtonText = MyTextsWrapperEnum.ButtonRetry;
        public MyTextsWrapperEnum RetryCancelText = MyTextsWrapperEnum.Cancel;

        public MyTextsWrapperEnum ErrorCaptionText = MyTextsWrapperEnum.MessageBoxNetworkErrorCaption;
        public MyTextsWrapperEnum ErrorMessageText = MyTextsWrapperEnum.PleaseTryAgain;

        public Func<MySectorServiceClient, IAsyncResult> BeginAction;
        public Action<MySectorServiceClient, IAsyncResult> EndAction;
        public Func<MySectorServiceClient, IAsyncResult, MyServerAction> EndActionWait;

        public MyTextsWrapperEnum? BackgroundNotification;

        public bool Background = false;

        public bool ShowErrorMessage = true;
        public bool OnErrorReturnToMainMenu = true;
        public bool EnableRetry = false;
        public TimeSpan Timeout { get; set; }

        public event Action<Exception> ActionFailed;
        public event Action ActionSuccess;

        private MyGuiScreenBase m_retryScreen;

        public void Start()
        {
            MyGuiManager.AddScreen(this);

            if (Background && BackgroundNotification.HasValue)
            {
                var notification = new MyHudNotification.MyNotification(BackgroundNotification.Value);
                MyHudNotification.AddNotification(notification);
                this.Closed += (screen) => notification.Disappear();
            }
        }

        protected override void ServiceProgressStart(MySectorServiceClient client)
        {
            AddAction(BeginAction(client));
        }

        protected override void OnActionCompleted(IAsyncResult asyncResult, MySectorServiceClient client)
        {
            Debug.Assert((EndAction != null && EndActionWait == null) || (EndAction == null && EndActionWait != null), "Set only one of EndAction, EndActionWait");

            if (EndActionWait != null)
            {
                var innerAction = EndActionWait(client, asyncResult);
                if (innerAction != null)
                {
                    innerAction.ActionSuccess += RaiseActionSuccess;
                    innerAction.ActionFailed += RaiseActionFailed;
                }
                else
                {
                    RaiseActionSuccess();
                }
            }
            else if (EndAction != null)
            {
                // When error occures, OnError is called
                EndAction(client, asyncResult);
                RaiseActionSuccess();
            }
            CloseScreen();
        }

        private void RaiseActionSuccess()
        {
            var handler = ActionSuccess;
            if (handler != null)
            {
                handler();
            }
        }

        protected override void OnError(Exception exception, MySectorServiceClient client)
        {
            MyMwcLog.WriteLine(exception); // log exception

            if (EnableRetry)
            {
                ShowRetryDialog(exception);
            }
            else
            {
                RaiseActionFailed(exception);
                if (ShowErrorMessage)
                {
                    MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, ErrorMessageText, ErrorCaptionText, MyTextsWrapperEnum.Ok, null));
                }
                CloseScreen();
                if (OnErrorReturnToMainMenu)
                {
                    MyGuiManager.BackToMainMenu();
                }
            }
        }

        public override string GetFriendlyName()
        {
            return "MyServerAction";
        }

        private void ShowRetryDialog(Exception exception)
        {
            var messageBoxCaption = MyTextsWrapper.Get(RetryCaptionText);
            var messageBoxMessage = MyTextsWrapper.Get(RetryMessageText);

            Vector2 buttonSize = new Vector2(MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE.X * 2.4f, MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE.Y);
            m_retryScreen = new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, MyMessageBoxButtonsType.YES_NO, messageBoxMessage, messageBoxCaption, null, RetryButtonText,
                    RetryCancelText, (callbackReturn) => MessageBoxCallback(exception, callbackReturn), false, buttonSize);

            MyGuiManager.AddScreen(m_retryScreen);
        }

        private void MessageBoxCallback(Exception exception, MyGuiScreenMessageBoxCallbackEnum callbackReturn)
        {
            m_retryScreen.CloseScreenNow();
            m_retryScreen = null;

            // retry
            if (callbackReturn == MyGuiScreenMessageBoxCallbackEnum.YES)
            {
                Retry();
            }
            // continue without saving
            else if (callbackReturn == MyGuiScreenMessageBoxCallbackEnum.NO)
            {
                RaiseActionFailed(exception);
                CloseScreen();
            }
        }

        private void RaiseActionFailed(Exception exception)
        {
            var handler = ActionFailed;
            if (handler != null)
            {
                handler(exception);
            }
        }

        public override bool Update(bool hasFocus)
        {
            if (base.Update(hasFocus) == false) return false;

            if (Background && m_state != MyGuiScreenState.HIDING && m_state != MyGuiScreenState.HIDDEN)
            {
                HideScreen();
            }

            return GetState() == MyGuiScreenState.OPENED;
        }

        public override bool Draw(float backgroundFadeAlpha)
        {
            if (Background)
                return true;

            return base.Draw(backgroundFadeAlpha);
        }
    }
}
