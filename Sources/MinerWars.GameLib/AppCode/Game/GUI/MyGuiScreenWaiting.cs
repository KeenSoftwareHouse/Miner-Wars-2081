using System;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Localization;

namespace MinerWars.AppCode.Game.GUI
{
    enum CancelReasonEnum
    {
        UserCancel,
        Timeout,
    }

    class WaitingCanceledArgs : EventArgs
    {
        public CancelReasonEnum CancelReason { get; private set; }

        public WaitingCanceledArgs(CancelReasonEnum cancelReason)
        {
            CancelReason = cancelReason;
        }
    }

    class MyGuiScreenWaiting : MyGuiScreenProgressBaseAsync<object>
    {
        readonly EventHandler<WaitingCanceledArgs> m_cancelOrTimeoutHandler;

        readonly TimeSpan? m_timeout;
        TimeSpan m_timeoutStartedTimeInMiliseconds;

        public MyGuiScreenWaiting(MyTextsWrapperEnum caption, EventHandler<WaitingCanceledArgs> cancelOrTimeoutHandler, TimeSpan? timeout = null)
            : base(caption, true)
        {
            m_cancelOrTimeoutHandler = cancelOrTimeoutHandler;
            m_timeout = timeout;
            m_timeoutStartedTimeInMiliseconds = TimeSpan.FromMilliseconds(MyMinerGame.TotalTimeInMilliseconds);
            m_closeOnEsc = false;
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenWaiting";
        }

        public override bool Update(bool hasFocus)
        {
            if (base.Update(hasFocus) == false) return false;

            //  Only continue if this screen is really open (not closing or closed)
            if (GetState() != MyGuiScreenState.OPENED) return false;

            if (m_timeout.HasValue)
            {
                var deltaTime = TimeSpan.FromMilliseconds(MyMinerGame.TotalTimeInMilliseconds) - m_timeoutStartedTimeInMiliseconds;
                if (deltaTime >= m_timeout)
                {
                    OnTimeout();
                }
            }

            return true;
        }

        protected virtual void OnTimeout()
        {
            CancelAll();
            CloseScreen();

            var handler = m_cancelOrTimeoutHandler;
            if (handler != null)
            {
                handler(this, new WaitingCanceledArgs(CancelReasonEnum.Timeout));
            }
        }

        protected override void OnCancelClick(MyGuiControlButton sender)
        {
            base.OnCancelClick(sender);

            var handler = m_cancelOrTimeoutHandler;
            if (handler != null)
            {
                handler(this, new WaitingCanceledArgs(CancelReasonEnum.UserCancel));
            }
        }

        protected override void ProgressStart()
        {
            
        }
    }
}