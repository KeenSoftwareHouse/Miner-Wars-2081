using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.GUI.Core;

namespace MinerWars.AppCode.Game.GUI
{
    class MyGuiScreenWaitingOnHost: MyGuiScreenWaiting
    {
        bool canceled = false;

        public MyGuiScreenWaitingOnHost(MyTextsWrapperEnum caption)
            :base(caption, null, null)
        {
        }
        
        protected override void Canceling()
        {
            if (!canceled)
            {
                canceled = true;
                MyGuiManager.AddModalScreen(new MyGuiScreenMessageBox(MyMessageBoxType.MESSAGE, MyTextsWrapperEnum.DoYouWantToDisconnect, MyTextsWrapperEnum.DisconnectQuestion, MyTextsWrapperEnum.Yes, MyTextsWrapperEnum.No, new MyGuiScreenMessageBox.MessageBoxCallback(DisconnectQuestion)), null);
            }
        }

        void DisconnectQuestion(MyGuiScreenMessageBoxCallbackEnum resultEnum)
        {
            if (resultEnum == MyGuiScreenMessageBoxCallbackEnum.YES)
            {
                OnCancelClick(null);
                MyGuiManager.BackToMainMenu();
            }
        }
    }
}
