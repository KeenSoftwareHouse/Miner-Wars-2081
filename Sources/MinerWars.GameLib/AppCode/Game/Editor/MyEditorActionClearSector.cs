
namespace MinerWars.AppCode.Game.Editor
{
    /// <summary>
    /// Action used for clearing whole sector
    /// </summary>
    class MyEditorActionClearSector : MyEditorActionWithObjectBuildersBase
    {
        public MyEditorActionClearSector()
            : base()
        {
        }

        public override bool Perform()
        {
            RemoveAllInBackgroundThread();            
            return true;
        }

        public override bool Rollback()
        {
            AddOrCreateInBackgroundThread();            
            return true;
        }
    }
}
