using MinerWars.AppCode.Game.Entities;
using System.Collections.Generic;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.AppCode.Game.Prefabs;
using MinerWars.AppCode.Game.Entities.WayPoints;

namespace MinerWars.AppCode.Game.Editor
{
    /// <summary>
    /// Copy entity editor action
    /// </summary>
    class MyEditorActionEntityCopy : MyEditorActionWithObjectBuildersBase
    {
        public List<MyEntity> SourceEntities;
        public IMyEntityIdRemapContext RemapContext;

        public MyEditorActionEntityCopy(MyEntity actionEntity)
            : base(actionEntity, true)
        {
            ActionEntities.Clear();
            SourceEntities = new List<MyEntity>();
            SourceEntities.Add(actionEntity);
            AddChildren();
            RemapEntityIdsOnInit();
        }

        public MyEditorActionEntityCopy(List<MyEntity> actionEntities)
            : base(actionEntities, true)
        {
            ActionEntities.Clear();
            SourceEntities = new List<MyEntity>();
            SourceEntities.AddRange(actionEntities);
            AddChildren();
            RemapEntityIdsOnInit();
        }

        /// <summary>
        /// Some copied entities need their children copied, too.
        /// </summary>
        private void AddChildren()
        {
            int oldCount = SourceEntities.Count;
            for (int i = 0; i < oldCount; i++)
            {
                if (SourceEntities[i] is MyPrefabBase)
                {
                    foreach (var child in SourceEntities[i].Children)
                        if (child is MyWayPoint)
                        {
                            SourceEntities.Add(child);
                            ActionObjectBuilders.Add(new ObjectBuilderCreate(child, true));
                        }
                }
                else if (SourceEntities[i] is MyPrefabContainer)
                {
                    foreach (var prefab in (SourceEntities[i] as MyPrefabContainer).GetPrefabs())
                        foreach (var child in prefab.Children)
                            if (child is MyWayPoint)
                            {
                                SourceEntities.Add(child);
                                ActionObjectBuilders.Add(new ObjectBuilderCreate(child, true));
                            }
                }
            }
        }

        private void RemapEntityIdsOnInit()
        {
            RemapContext = new MyEntityIdRemapContext();
            foreach (var ob in ActionObjectBuilders)
            {
                ob.ObjectBuilder.RemapEntityIds(RemapContext);

                if (!string.IsNullOrEmpty(ob.ObjectBuilder.Name))
                {
                    string baseName = ob.ObjectBuilder.Name;
                    int index = 0;
                    if (ob.ObjectBuilder.Name.Length > 2)
                    {
                        string indexString = ob.ObjectBuilder.Name.Substring(ob.ObjectBuilder.Name.Length - 2);
                        try
                        {
                            index = System.Convert.ToInt32(indexString);
                            baseName = ob.ObjectBuilder.Name.Substring(0, ob.ObjectBuilder.Name.Length - 3);
                            index++;
                        }
                        catch
                        {
                        }
                    }

                    string proposedName = ""; 
                    do
                    {
                        proposedName = baseName + "_" + index.ToString("##00");
                        index++;
                    }
                    while (MyEntities.IsNameExists(null, proposedName) || ActionObjectBuilders.Exists(aob => aob.ObjectBuilder.Name == proposedName));

                    ob.ObjectBuilder.Name = proposedName;
                }
            }
        }

        public override bool Perform()
        {
            AddOrCreateInBackgroundThread();
            return true;
        }

        public override bool Rollback()
        {
            RemoveInBackgroundThread();            
            return true;
        }
    }
}