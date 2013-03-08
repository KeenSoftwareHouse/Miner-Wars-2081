using MinerWars.AppCode.Game.Managers.EntityManager.Entities;
using MinerWars.AppCode.Game.Voxels;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.AppCode.Game.GUI;
using System.Threading;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.GUI.Core;

namespace MinerWars.AppCode.Game.Editor
{
    class MyEditorActionRandomSector : MyEditorActionWithObjectBuildersBase
    {
        public MyEditorActionRandomSector()
            : base()
        {
        }

        public override bool Perform()
        {
            MyGuiManager.AddScreen(new MyGuiScreenEditorProgress(MyTextsWrapperEnum.LoadingPleaseWait, false));
            MyEditorGizmo.ClearSelection();
            MyEditor.Static.StartBackgroundThread(new Thread(new ThreadStart(MyGuiScreenGamePlay.Static.CreateRandomWorld)));
            return true;
        }

        public override bool Rollback()
        {
            //TODO
            AddOrCreateInBackgroundThread();
            return true;
        }
    }
}
