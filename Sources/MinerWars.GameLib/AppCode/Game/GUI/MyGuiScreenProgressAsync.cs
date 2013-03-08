using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Localization;

namespace MinerWars.AppCode.Game.GUI
{
    class MyGuiScreenProgressAsync: MyGuiScreenProgressBase
    {
        public string FriendlyName { get; set; }

        private Func<IAsyncResult> m_beginAction;
        private Action<IAsyncResult, MyGuiScreenProgressAsync> m_endAction;
        private IAsyncResult m_asyncResult;

        public MyGuiScreenProgressAsync(MyTextsWrapperEnum text, bool enableCancel, Func<IAsyncResult> beginAction, Action<IAsyncResult, MyGuiScreenProgressAsync> endAction)
            : base(text, enableCancel)
        {
            FriendlyName = "MyGuiScreenProgressAsync";
            m_beginAction = beginAction;
            m_endAction = endAction;
        }

        protected override void ProgressStart()
        {
            m_asyncResult = m_beginAction();
        }

        public override string GetFriendlyName()
        {
            return FriendlyName;
        }

        public override bool Update(bool hasFocus)
        {
            if (base.Update(hasFocus) == false) return false;

            //  Only continue if this screen is really open (not closing or closed)
            if (GetState() != MyGuiScreenState.OPENED) return false;

            if (m_asyncResult.IsCompleted)
            {
                m_endAction(m_asyncResult, this);
            }

            return true;
        }
    }
}
