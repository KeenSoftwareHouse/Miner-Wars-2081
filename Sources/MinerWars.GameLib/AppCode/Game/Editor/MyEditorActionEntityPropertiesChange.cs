using System.Collections.Generic;
using MinerWars.AppCode.Game.Entities;

namespace MinerWars.AppCode.Game.Editor
{
    class MyEditorActionEntityPropertiesChange : MyEditorActionBase
    {
        public MyEditorActionEntityPropertiesChange(MyEntity actionPhysObject)
            : base(actionPhysObject)
        {
        }

        public MyEditorActionEntityPropertiesChange(List<MyEntity> actionPhysObjects)
            : base(actionPhysObjects)
        {
        }

        public override bool Perform()
        {
            return true;
        }

        public override bool Rollback()
        {
            return true;
        }
    }
}
