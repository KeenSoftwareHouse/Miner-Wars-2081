using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.Networking;
using System.IO;
using System.Net;
using System.Data.SqlClient;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;

namespace MinerWars.CommonLIB.AppCode.ObjectBuilders
{
    public class MyMwcObjectBuilder_ObjectGroup : MyMwcObjectBuilder_Base
    {
        public List<uint> ObjectList { get; set; }

        internal MyMwcObjectBuilder_ObjectGroup()
            : base()
        {
        }

        public MyMwcObjectBuilder_ObjectGroup(List<uint> objectList, String name)
        {
            ObjectList = objectList;
            Name = name;
        }

        public override void RemapEntityIds(IMyEntityIdRemapContext remapContext)
        {
            base.RemapEntityIds(remapContext);
            var newList = new List<uint>();
            foreach (var obj in ObjectList)
            {
                newList.Add(remapContext.RemapEntityId(obj).Value);
            }
            ObjectList = newList;
        }

        public IEnumerable<MyMwcObjectBuilder_Base> GetRootBuilders(IEnumerable<MyMwcObjectBuilder_Base> entities)
        {
            return this.ObjectList.ConvertAll(entityId => entities.FirstOrDefault(entity => entity.EntityId == entityId) as MyMwcObjectBuilder_Object3dBase).Where(s => s != null);
        }

        public IEnumerable<MyMwcObjectBuilder_PrefabBase> GetPrefabBuilders(IEnumerable<MyMwcObjectBuilder_Base> entities)
        {
            var allPrefabs = entities.OfType<MyMwcObjectBuilder_PrefabContainer>().SelectMany(container => container.Prefabs);
            return this.ObjectList.ConvertAll(a => allPrefabs.FirstOrDefault(b => b.EntityId == a)).Where(s => s != null);
        }

        internal override void Write(BinaryWriter binaryWriter)
        {
            base.Write(binaryWriter);

            //  ObjectList
            int countObjectList = ObjectList == null ? 0 : ObjectList.Count;
            MyMwcLog.IfNetVerbose_AddToLog("countObjectList: " + countObjectList);
            MyMwcMessageOut.WriteInt32(countObjectList, binaryWriter);
            for (int i = 0; i < countObjectList; i++)
            {
                MyMwcMessageOut.WriteInt32((int)ObjectList[i], binaryWriter);
            }
        }

        internal override bool Read(BinaryReader binaryReader, EndPoint senderEndPoint, int gameVersion)
        {
            if (!base.Read(binaryReader, senderEndPoint, gameVersion)) return NetworkError();

            // ObjectList
            int? countObjectList = MyMwcMessageIn.ReadInt32Ex(binaryReader, senderEndPoint);
            if (countObjectList == null) return NetworkError();
            MyMwcLog.IfNetVerbose_AddToLog("countObjectList: " + countObjectList);
            ObjectList = new List<uint>(countObjectList.Value);
            for (int i = 0; i < countObjectList; i++)
            {
                int? objectId = MyMwcMessageIn.ReadInt32Ex(binaryReader, senderEndPoint);
                if (objectId == null) return NetworkError();
                ObjectList.Add((uint)objectId.Value);
                MyMwcLog.IfNetVerbose_AddToLog(string.Format("ObjectList[{0}]: {1}", i, objectId));
            }

            return true;
        }

        public override MyMwcObjectBuilderTypeEnum GetObjectBuilderType()
        {
            return MyMwcObjectBuilderTypeEnum.ObjectGroup;
        }
    }
}
