using MinerWars.AppCode.Game.Entities;
using System.Collections.Generic;

namespace MinerWars.AppCode.Game.Editor
{
    /// <summary>
    /// Delete entity editor action
    /// </summary>
    class MyEditorActionEntityDelete : MyEditorActionWithObjectBuildersBase
    {

        public MyEditorActionEntityDelete(MyEntity actionPhysObject)
            : base(actionPhysObject)
        {
        }

        public MyEditorActionEntityDelete(List<MyEntity> actionPhysObjects)
            : base(actionPhysObjects)
        {
        }

        public override bool Perform()
        {
            RemoveInBackgroundThread();            
            return true;
        }

        public override bool Rollback()
        {
            AddOrCreateInBackgroundThread();            
            return true;
        }
    }
}