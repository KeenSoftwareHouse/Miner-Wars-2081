using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Localization;

namespace MinerWars.AppCode.Game.GUI
{
    // This screen displays when we are loading something in editor
    class MyGuiScreenEditorProgress : MyGuiScreenProgressBase
    {
        public static MyGuiScreenEditorProgress CurrentScreen = null;    //  This is always filled with reference to actual instance of this scree. If there isn't, it's null.

        public MyGuiScreenEditorProgress(MyTextsWrapperEnum progressText, bool enableCancel) :
            base(progressText, enableCancel)
        {
            CurrentScreen = this;
        }

        protected override void ProgressStart()
        {
            // Nothing to do
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenEditorProgress";
        }

        public override bool CloseScreen()
        {
            bool ret = base.CloseScreen();

            if (ret == true)
            {
                CurrentScreen = null;
            }

            return ret;
        }
    }
}
