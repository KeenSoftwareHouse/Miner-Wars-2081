using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using SysUtils.Utils;

namespace MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D
{    
    public class MyMwcObjectBuilder_FoundationFactory : MyMwcObjectBuilder_Object3dBase
    {
        public MyMwcObjectBuilder_PrefabContainer PrefabContainer { get; set; }
        public bool IsBuilding { get; set; }
        public int BuildingTimeFromStart { get; set; }
        public MyMwcObjectBuilder_ObjectToBuild BuildingObject { get; set; }
        public List<MyMwcObjectBuilder_ObjectToBuild> BuildingQueue { get; set; }

        internal MyMwcObjectBuilder_FoundationFactory()
            : base()
        {
            BuildingQueue = new List<MyMwcObjectBuilder_ObjectToBuild>();            
        }

        public MyMwcObjectBuilder_FoundationFactory(MyMwcObjectBuilder_PrefabContainer prefabContainer, bool isBuilding, int buildingTimeFromStart, 
            MyMwcObjectBuilder_ObjectToBuild buildingObject, List<MyMwcObjectBuilder_ObjectToBuild> buildingQueue)
        {
            PrefabContainer = prefabContainer;
            IsBuilding = isBuilding;
            BuildingTimeFromStart = buildingTimeFromStart;
            BuildingObject = buildingObject;
            BuildingQueue = buildingQueue;            
        }

        public override void RemapEntityIds(IMyEntityIdRemapContext remapContext)
        {
            base.RemapEntityIds(remapContext);
            PrefabContainer.RemapEntityIds(remapContext);
            BuildingObject.RemapEntityIds(remapContext);
            foreach (var obj in BuildingQueue)
            {
                obj.RemapEntityIds(remapContext);
            }
        }

        public override MyMwcObjectBuilderTypeEnum GetObjectBuilderType()
        {
            return MyMwcObjectBuilderTypeEnum.FoundationFactory;
        }

        //  Write this object into message-out
        internal override void Write(BinaryWriter binaryWriter)
        {
            base.Write(binaryWriter);

            //  Is building
            MyMwcLog.IfNetVerbose_AddToLog("IsBuilding: " + IsBuilding);
            MyMwcMessageOut.WriteBool(IsBuilding, binaryWriter);

            //  Building time from start
            MyMwcLog.IfNetVerbose_AddToLog("BuildingTimeFromStart: " + BuildingTimeFromStart);
            MyMwcMessageOut.WriteInt32(BuildingTimeFromStart, binaryWriter);

            //  Prefab container
            bool isPrefabContainerObjectBuilder = PrefabContainer != null;
            MyMwcMessageOut.WriteBool(isPrefabContainerObjectBuilder, binaryWriter);
            if (isPrefabContainerObjectBuilder) PrefabContainer.Write(binaryWriter);

            //  Building queue
            int countBuildingQueueObjects = BuildingQueue == null ? 0 : BuildingQueue.Count;
            MyMwcLog.IfNetVerbose_AddToLog("CountBuildingQueueObjects: " + countBuildingQueueObjects);
            MyMwcMessageOut.WriteInt32(countBuildingQueueObjects, binaryWriter);
            for (int i = 0; i < countBuildingQueueObjects; i++)
            {
                BuildingQueue[i].Write(binaryWriter);
            }

            //  Building object
            bool isBuildingObjectObjectBuilder = BuildingObject != null;
            MyMwcMessageOut.WriteBool(isBuildingObjectObjectBuilder, binaryWriter);
            if (isBuildingObjectObjectBuilder) BuildingObject.Write(binaryWriter);            
        }

        //  Read this object from message-in
        internal override bool Read(BinaryReader binaryReader, EndPoint senderEndPoint, int gameVersion)
        {
            if (base.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();

            //  Is building
            bool? isBuilding = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
            if (isBuilding == null) return NetworkError();
            IsBuilding = isBuilding.Value;
            MyMwcLog.IfNetVerbose_AddToLog("IsBuilding: " + IsBuilding);

            //  Building time from start
            int? buildingTimeFromStart = MyMwcMessageIn.ReadInt32Ex(binaryReader, senderEndPoint);
            if (buildingTimeFromStart == null) return NetworkError();
            BuildingTimeFromStart = buildingTimeFromStart.Value;
            MyMwcLog.IfNetVerbose_AddToLog("BuildingTimeFromStart: " + BuildingTimeFromStart);

            //  Prefab container
            bool? isPrefabContainer = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
            if (isPrefabContainer == null) return NetworkError();
            if (isPrefabContainer.Value)
            {
                PrefabContainer = MyMwcObjectBuilder_Base.ReadAndCreateNewObject(binaryReader, senderEndPoint) as MyMwcObjectBuilder_PrefabContainer;
                if (PrefabContainer == null) return NetworkError();
                if (PrefabContainer.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();
            }
            else
            {
                PrefabContainer = null;
            }

            //  Building queue
            int? countBuildingQueueObjects = MyMwcMessageIn.ReadInt32Ex(binaryReader, senderEndPoint);
            if (countBuildingQueueObjects == null) return NetworkError();
            MyMwcLog.IfNetVerbose_AddToLog("CountBuildingQueueObjects: " + countBuildingQueueObjects);
            BuildingQueue = new List<MyMwcObjectBuilder_ObjectToBuild>(countBuildingQueueObjects.Value);
            for (int i = 0; i < countBuildingQueueObjects; i++)
            {
                MyMwcObjectBuilder_ObjectToBuild objectToBuild = MyMwcObjectBuilder_Base.ReadAndCreateNewObject(binaryReader, senderEndPoint) as MyMwcObjectBuilder_ObjectToBuild;
                if (objectToBuild == null) return NetworkError();
                if (objectToBuild.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();
                BuildingQueue.Add(objectToBuild);
            }

            //  Building object
            bool? isBuildingObject = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
            if (isBuildingObject == null) return NetworkError();
            if (isBuildingObject.Value)
            {
                BuildingObject = MyMwcObjectBuilder_Base.ReadAndCreateNewObject(binaryReader, senderEndPoint) as MyMwcObjectBuilder_ObjectToBuild;
                if (BuildingObject == null) return NetworkError();
                if (BuildingObject.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();
            }
            else
            {
                BuildingObject = null;
            }            

            return true;
        }
    }
}
