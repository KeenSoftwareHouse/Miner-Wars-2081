using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using MinerWarsMath;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.CommonLIB.AppCode.Utils;
using SysUtils.Utils;

namespace MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.Prefabs
{
    public enum MyMwcObjectBuilder_PrefabSecurityControlHUB_TypesEnum : ushort
    {
        DEFAULT = 1,
        P541_SCREEN_A = 2,
        P541_SCREEN_B = 3,
        P541_TERMINAL_A = 4,
    }

    public class MyMwcObjectBuilder_PrefabSecurityControlHUB : MyMwcObjectBuilder_PrefabBase
    {
        public List<uint> ConnectedEntities { get; set; }

        internal MyMwcObjectBuilder_PrefabSecurityControlHUB()
            : base()
        {
            ConnectedEntities = new List<uint>();
        }

        public MyMwcObjectBuilder_PrefabSecurityControlHUB(MyMwcObjectBuilder_PrefabHangar_TypesEnum prefabType, MyMwcObjectBuilder_Prefab_AppearanceEnum appearance, MyMwcVector3Short position, Vector3 anglesInContainer, float? prefabMaxHealth, float prefabHealthRatio, string displayName, float electricCapacity, bool? causesAlarm, List<uint> connectedEntities, int aiPriority)
            : base((int)prefabType, appearance, position, anglesInContainer, prefabMaxHealth, prefabHealthRatio, displayName, electricCapacity, causesAlarm, aiPriority, true)
        {
            ConnectedEntities = connectedEntities;
        }

        public override void RemapEntityIds(IMyEntityIdRemapContext remapContext)
        {
            base.RemapEntityIds(remapContext);
            for (int i = 0; i < ConnectedEntities.Count; i++)
            {
                ConnectedEntities[i] = remapContext.RemapEntityId(ConnectedEntities[i]).Value;
            }
        }

        /// <summary>
        /// GetObjectBuilderType
        /// </summary>
        /// <returns></returns>
        public override MyMwcObjectBuilderTypeEnum GetObjectBuilderType()
        {
            return MyMwcObjectBuilderTypeEnum.PrefabSecurityControlHUB;
        }

        public MyMwcObjectBuilder_PrefabSecurityControlHUB_TypesEnum PrefabSecurityControlHUBType
        {
            get
            {
                return (MyMwcObjectBuilder_PrefabSecurityControlHUB_TypesEnum)GetObjectBuilderId().Value;
            }
            set
            {
                SetObjectBuilderId((int)value);
            }
        }

        //  Write this object into message-out
        internal override void Write(BinaryWriter binaryWriter)
        {
            base.Write(binaryWriter);

            //  Connected entities
            int countConnectedEntitiesObjects = ConnectedEntities == null ? 0 : ConnectedEntities.Count;
            MyMwcLog.IfNetVerbose_AddToLog("CountConnecteEntities: " + countConnectedEntitiesObjects);
            MyMwcMessageOut.WriteInt32(countConnectedEntitiesObjects, binaryWriter);
            for (int i = 0; i < countConnectedEntitiesObjects; i++)
            {
                MyMwcMessageOut.WriteUInt32(ConnectedEntities[i], binaryWriter);
            }
        }

        //  Read this object from message-in
        internal override bool Read(BinaryReader binaryReader, EndPoint senderEndPoint, int gameVersion)
        {
            if (base.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();

            //  Connected entities
            int? countConnectedEntitiesObjects = MyMwcMessageIn.ReadInt32Ex(binaryReader, senderEndPoint);
            if (countConnectedEntitiesObjects == null) return NetworkError();
            MyMwcLog.IfNetVerbose_AddToLog("CountConnectedEntities: " + countConnectedEntitiesObjects);
            ConnectedEntities = new List<uint>(countConnectedEntitiesObjects.Value);
            for (int i = 0; i < countConnectedEntitiesObjects; i++)
            {
                // conected entity id id has value - read the value
                uint? conecteEntityId = MyMwcMessageIn.ReadUInt32Ex(binaryReader, senderEndPoint);
                if (!conecteEntityId.HasValue) return NetworkError();
                ConnectedEntities.Add(conecteEntityId.Value);
            }

            return true;
        }
    }
}
