using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Data.SqlClient;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using System;
using MinerWars.CommonLIB.AppCode.Networking;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.Prefabs;

namespace MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D
{
    public enum MyMwcObjectBuilder_PrefabContainer_TypesEnum : ushort
    {
        TEMPLATE = 1,
        INSTANCE = 2
    }

    //This class represents instance of prefab container
    public class MyMwcObjectBuilder_PrefabContainer : MyMwcObjectBuilder_Object3dBase
    {
        /** Reference to template, container was created from - in case it was not modified, all prefabs will be loaded from template.
         *  When creating new container and we create from template or copy from instance, that has template set, we copy this ID to new instance.
         *  In future, we must be able to mark the template as deleted(player will have templates, that bothers him) - that will result in action,
         *  when we will mark the template as deleted(bool objectProperty) and when loading templates, marked wont be included. It will be then usefull,
         *  if there is some procedure running once in a time, which scans, if there are some templates, that are no longer referenced and thus can be deleted
         *  physically and permanently.
         */
        public int? TemplateId;
        public MyMwcObjectBuilder_PrefabContainer_TypesEnum ContainerType;
        public int UserOwnerID;
        public MyMwcObjectBuilder_FactionEnum Faction;
        public MyMwcObjectBuilder_Inventory Inventory;
        public string DisplayName;
        public MyMwcObjectBuilder_EntityUseProperties UseProperties;
        public bool AlarmOn;
        public int? RefillTime;

        /** When we create instance from template, we can watch if any modification has been done to it. In case it wasnt,
         *  we wont fill this list - but 
         */
        public List<MyMwcObjectBuilder_PrefabBase> Prefabs;

        internal MyMwcObjectBuilder_PrefabContainer()
            : base()
        {
            Inventory = new MyMwcObjectBuilder_Inventory();
        }

        public MyMwcObjectBuilder_PrefabContainer(int? templateId, MyMwcObjectBuilder_PrefabContainer_TypesEnum containerType,
            List<MyMwcObjectBuilder_PrefabBase> prefabs, int userOwnerID, MyMwcObjectBuilder_FactionEnum faction, MyMwcObjectBuilder_Inventory inventory)
        {
            TemplateId = templateId;
            ContainerType = containerType;
            Prefabs = prefabs;
            UserOwnerID = userOwnerID;
            Faction = faction;
            Inventory = inventory;
        }

        public override void RemapEntityIds(IMyEntityIdRemapContext remapContext)
        {
            base.RemapEntityIds(remapContext);
            foreach (var prefab in Prefabs)
            {
                prefab.RemapEntityIds(remapContext);
            }
            Inventory.RemapEntityIds(remapContext);
        }

        public override MyMwcObjectBuilderTypeEnum GetObjectBuilderType()
        {
            return MyMwcObjectBuilderTypeEnum.PrefabContainer;
        }

        public override int? GetObjectBuilderId()
        {
            return (int)ContainerType;
        }

        public override void ClearEntityId()
        {
            base.ClearEntityId();
            foreach (var p in Prefabs)
            {
                p.ClearEntityId();
            }
        }

        internal override void SetObjectBuilderIdInternal(int? objectBuilderId)
        {
            ContainerType = (MyMwcObjectBuilder_PrefabContainer_TypesEnum)Convert.ToUInt16(objectBuilderId);
        }

        //  Write this object into message-out
        internal override void Write(BinaryWriter binaryWriter)
        {
            base.Write(binaryWriter);

            //  Template ID
            MyMwcMessageOut.WriteBool(TemplateId.HasValue, binaryWriter);
            if (TemplateId.HasValue)
            {
                MyMwcLog.IfNetVerbose_AddToLog("TemplateId: " + TemplateId);
                MyMwcMessageOut.WriteInt32(TemplateId.Value, binaryWriter);
            }            

            //  Container Type
            MyMwcLog.IfNetVerbose_AddToLog("ContainerType: " + (int)ContainerType);
            MyMwcMessageOut.WriteObjectBuilderPrefabContainerTypesEnum(ContainerType, binaryWriter);

            // Faction must be defined
            System.Diagnostics.Debug.Assert(Enum.IsDefined(typeof(MyMwcObjectBuilder_FactionEnum), Faction));
            // Faction            
            MyMwcLog.IfNetVerbose_AddToLog("Faction: " + (int)Faction);
            MyMwcMessageOut.WriteInt32((int)Faction, binaryWriter);

            // Prefabs
            int countPrefabs = Prefabs == null ? 0 : Prefabs.Count;
            MyMwcLog.IfNetVerbose_AddToLog("countPrefabs: " + countPrefabs);
            MyMwcMessageOut.WriteInt32(countPrefabs, binaryWriter);
            for (int i = 0; i < countPrefabs; i++)
            {
                Prefabs[i].Write(binaryWriter);
            }

            // Inventory            
            bool isInventory = Inventory != null;
            MyMwcMessageOut.WriteBool(isInventory, binaryWriter);
            if (isInventory) Inventory.Write(binaryWriter);

            if (DisplayName != null)
            {
                MyMwcMessageOut.WriteBool(true, binaryWriter);
                MyMwcLog.IfNetVerbose_AddToLog("DisplayName: " + this.DisplayName);
                MyMwcMessageOut.WriteString(DisplayName, binaryWriter);
            }
            else
            {
                MyMwcLog.IfNetVerbose_AddToLog("DisplayName: " + "null");
                MyMwcMessageOut.WriteBool(false, binaryWriter);
            }

            //  Use Properties
            bool hasUseProperties = UseProperties != null;
            MyMwcMessageOut.WriteBool(hasUseProperties, binaryWriter);
            if (hasUseProperties) UseProperties.Write(binaryWriter);

            //  Alarm On
            MyMwcLog.IfNetVerbose_AddToLog("AlarmOn: " + AlarmOn);
            MyMwcMessageOut.WriteBool(AlarmOn, binaryWriter);

            //  Refill time
            MyMwcMessageOut.WriteBool(RefillTime.HasValue, binaryWriter);
            if (RefillTime.HasValue)
            {
                MyMwcLog.IfNetVerbose_AddToLog("RefillTime: " + RefillTime);
                MyMwcMessageOut.WriteInt32(RefillTime.Value, binaryWriter);
            }
        }

        //  Read this object from message-in
        internal override bool Read(BinaryReader binaryReader, EndPoint senderEndPoint, int gameVersion)
        {
            if (base.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();

            //  TemplateId
            bool? isTemplateId = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
            if (isTemplateId == null) return NetworkError();
            if (isTemplateId.Value)
            {
                TemplateId = MyMwcMessageIn.ReadInt32Ex(binaryReader, senderEndPoint);
            }
            else
            {
                TemplateId = null;
            }
            
            //  Container Type
            MyMwcObjectBuilder_PrefabContainer_TypesEnum? containerType = MyMwcMessageIn.ReadObjectBuilderPrefabContainerTypesEnumEx(binaryReader, senderEndPoint);
            if (containerType == null) return NetworkError();
            ContainerType = containerType.Value;
            MyMwcLog.IfNetVerbose_AddToLog("ContainerType: " + ContainerType);

            // Faction
            Faction = (MyMwcObjectBuilder_FactionEnum)MyMwcMessageIn.ReadInt32Ex(binaryReader, senderEndPoint);
            // Faction must be defined
            System.Diagnostics.Debug.Assert(Enum.IsDefined(typeof(MyMwcObjectBuilder_FactionEnum), Faction));

            //  Prefabs
            int? countPrefabs = MyMwcMessageIn.ReadInt32Ex(binaryReader, senderEndPoint);
            if (countPrefabs == null) return NetworkError();
            MyMwcLog.IfNetVerbose_AddToLog("countPrefabs: " + countPrefabs);
            Prefabs = new List<MyMwcObjectBuilder_PrefabBase>(countPrefabs.Value);
            for (int i = 0; i < countPrefabs; i++)
            {
                MyMwcObjectBuilder_PrefabBase prefab = MyMwcObjectBuilder_Base.ReadAndCreateNewObject(binaryReader, senderEndPoint) as MyMwcObjectBuilder_PrefabBase;
                if (prefab == null) return NetworkError();
                if (prefab.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();
                Prefabs.Add(prefab);
            }

            //  Inventory
            bool? isInventory = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
            if (isInventory == null) return NetworkError();
            if (isInventory.Value)
            {
                Inventory = MyMwcObjectBuilder_Base.ReadAndCreateNewObject(binaryReader, senderEndPoint) as MyMwcObjectBuilder_Inventory;
                if (Inventory == null) return NetworkError();
                if (Inventory.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();
            }
            else
            {
                Inventory = null;
            }


            bool? hasDisplayName = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
            if (!hasDisplayName.HasValue) return NetworkError();
            if (hasDisplayName.Value)
            {
                string displayName = MyMwcMessageIn.ReadStringEx(binaryReader, senderEndPoint);
                if (displayName == null) return NetworkError();
                MyMwcLog.IfNetVerbose_AddToLog("DisplayName: " + displayName);
                DisplayName = displayName;
            }
            else
            {
                MyMwcLog.IfNetVerbose_AddToLog("DisplayName: " + "null");
                DisplayName = null;
            }

            //  Use Properties
            bool? hasUseProperties = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
            if (hasUseProperties == null) return NetworkError();
            if (hasUseProperties.Value)
            {
                UseProperties = MyMwcObjectBuilder_Base.ReadAndCreateNewObject(binaryReader, senderEndPoint) as MyMwcObjectBuilder_EntityUseProperties;
                if (UseProperties == null) return NetworkError();
                if (UseProperties.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();
            }
            else
            {
                UseProperties = null;
            }

            //  Alarm On
            bool? alarmOn = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
            if (alarmOn == null) return NetworkError();
            AlarmOn = alarmOn.Value;
            MyMwcLog.IfNetVerbose_AddToLog("AlarmOn: " + AlarmOn);

            //  Refill time
            bool? refillTime = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
            if (refillTime == null) return NetworkError();
            if (refillTime.Value)
            {
                RefillTime = MyMwcMessageIn.ReadInt32Ex(binaryReader, senderEndPoint);
            }
            else
            {
                RefillTime = null;
            }

            return true;
        }
    }
}
