using System;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWarsMath;
using System.IO;
using System.Net;
using System.Data.SqlClient;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.Networking;

namespace MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.Prefabs
{
    //  IMPORTANT: Never change numeric values and never delete any enum item. These numbers are referenced e.g. from database.
    public enum MyMwcObjectBuilder_PrefabKinematic_TypesEnum : ushort
    {
        P415_C01_DOOR1 = 45,
        P415_C01_DOOR2 = 46,
        P415_C01_DOOR3 = 47,
        P415_C01_DOOR4 = 48,
        P341_A01_OPEN_DOCK_VARIATION1 = 122,
        P415_A01_DOORCASE = 79,
    }

    public class MyMwcObjectBuilder_PrefabKinematic : /*MyMwcObjectBuilder_Prefab*/ MyMwcObjectBuilder_PrefabBase
    {
        public const int MAX_KINEMATIC_PARTS = 10;

        // Kinematic parts healths        
        public float?[] KinematicPartsHealth;
        public float?[] KinematicPartsMaxHealth;
        public uint?[] KinematicPartsEntityId;

        internal MyMwcObjectBuilder_PrefabKinematic()
            : base()
        {
            KinematicPartsHealth = new float?[MAX_KINEMATIC_PARTS];
            KinematicPartsMaxHealth = new float?[MAX_KINEMATIC_PARTS];
            KinematicPartsEntityId = new uint?[MAX_KINEMATIC_PARTS];
        }

        /// <summary>
        /// c-tor
        /// </summary>
        /// <param name="prefabType"></param>
        /// <param name="position"></param>
        /// <param name="anglesInContainer"></param>
        public MyMwcObjectBuilder_PrefabKinematic(MyMwcObjectBuilder_PrefabKinematic_TypesEnum prefabType, MyMwcObjectBuilder_Prefab_AppearanceEnum appearance, MyMwcVector3Short position, Vector3 anglesInContainer, float? prefabMaxHealth, float prefabHealthRatio, string displayName, float electricCapacity, bool? causesAlarm, int aiPriority)
            : base((int)prefabType, appearance, position, anglesInContainer, prefabMaxHealth, prefabHealthRatio, displayName, electricCapacity, causesAlarm, aiPriority)
        {

        }

        /// <summary>
        /// GetObjectBuilderType
        /// </summary>
        /// <returns></returns>
        public override MyMwcObjectBuilderTypeEnum GetObjectBuilderType()
        {
            return MyMwcObjectBuilderTypeEnum.PrefabKinematic;
        }

        public override void ClearEntityId()
        {
            base.ClearEntityId();
            for (int i = 0; i < MAX_KINEMATIC_PARTS; i++)
            {
                KinematicPartsEntityId[i] = null;
            }
        }

        public override void RemapEntityIds(IMyEntityIdRemapContext remapContext)
        {
            base.RemapEntityIds(remapContext);
            for (int i = 0; i < MAX_KINEMATIC_PARTS; i++)
            {
                KinematicPartsEntityId[i] = remapContext.RemapEntityId(KinematicPartsEntityId[i]);
            }
        }

        public MyMwcObjectBuilder_PrefabKinematic_TypesEnum PrefabKinematicType
        {
            get
            {
                return (MyMwcObjectBuilder_PrefabKinematic_TypesEnum) GetObjectBuilderId().Value;
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

            // Prefab parts healths and max healths
            for (int i = 0; i < MAX_KINEMATIC_PARTS; i++) 
            {
                float? prefabHealth = KinematicPartsHealth[i];                
                if (prefabHealth != null)
                {
                    MyMwcMessageOut.WriteBool(true, binaryWriter);
                    MyMwcLog.IfNetVerbose_AddToLog("PrefabHealth" + (i + 1) + ": " + prefabHealth);
                    MyMwcMessageOut.WriteFloat(prefabHealth.Value, binaryWriter);
                }
                else
                {
                    MyMwcLog.IfNetVerbose_AddToLog("PrefabHealth" + (i + 1) + ": " + "null");
                    MyMwcMessageOut.WriteBool(false, binaryWriter);
                }

                float? prefabMaxHealth = KinematicPartsMaxHealth[i];
                if (prefabMaxHealth != null)
                {
                    MyMwcMessageOut.WriteBool(true, binaryWriter);
                    MyMwcLog.IfNetVerbose_AddToLog("PrefabMaxHealth" + (i + 1) + ": " + prefabMaxHealth);
                    MyMwcMessageOut.WriteFloat(prefabMaxHealth.Value, binaryWriter);
                }
                else
                {
                    MyMwcLog.IfNetVerbose_AddToLog("PrefabMaxHealth" + (i + 1) + ": " + "null");
                    MyMwcMessageOut.WriteBool(false, binaryWriter);
                }

                uint? entityId = KinematicPartsEntityId[i];
                if (entityId != null)
                {
                    MyMwcMessageOut.WriteBool(true, binaryWriter);
                    MyMwcLog.IfNetVerbose_AddToLog("EntityId" + (i + 1) + ": " + entityId);
                    MyMwcMessageOut.WriteUInt32(entityId.Value, binaryWriter);
                }
                else
                {
                    MyMwcLog.IfNetVerbose_AddToLog("EntityId" + (i + 1) + ": " + "null");
                    MyMwcMessageOut.WriteBool(false, binaryWriter);
                }
            }            
        }

        //  Read this object from message-in
        internal override bool Read(BinaryReader binaryReader, EndPoint senderEndPoint, int gameVersion)
        {
            if (base.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();

            // Prefab parts healths and max healths
            for (int i = 0; i < MAX_KINEMATIC_PARTS; i++) 
            {
                bool? hasPrefabHealth = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
                if (!hasPrefabHealth.HasValue) return NetworkError();
                if (hasPrefabHealth.Value)
                {
                    float prefabHealth = MyMwcMessageIn.ReadFloat(binaryReader);
                    if (prefabHealth == null) return NetworkError();
                    MyMwcLog.IfNetVerbose_AddToLog("PrefabHealth" + (i + 1) + ": " + prefabHealth);
                    KinematicPartsHealth[i] = prefabHealth;
                }
                else
                {
                    MyMwcLog.IfNetVerbose_AddToLog("PrefabHealth" + (i + 1) + ": " + "null");
                    KinematicPartsHealth[i] = null;
                }

                bool? hasPrefabMaxHealth = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
                if (!hasPrefabMaxHealth.HasValue) return NetworkError();
                if (hasPrefabMaxHealth.Value)
                {
                    float prefabMaxHealth = MyMwcMessageIn.ReadFloat(binaryReader);
                    if (prefabMaxHealth == null) return NetworkError();
                    MyMwcLog.IfNetVerbose_AddToLog("PrefabMaxHealth" + (i + 1) + ": " + prefabMaxHealth);
                    KinematicPartsMaxHealth[i] = prefabMaxHealth;
                }
                else
                {
                    MyMwcLog.IfNetVerbose_AddToLog("PrefabMaxHealth" + (i + 1) + ": " + "null");
                    KinematicPartsMaxHealth[i] = null;
                }

                bool? hasEntityId = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
                if (!hasEntityId.HasValue) return NetworkError();
                if (hasEntityId.Value)
                {
                    uint? entityId = MyMwcMessageIn.ReadUInt32Ex(binaryReader, senderEndPoint);
                    if (entityId == null) return NetworkError();
                    MyMwcLog.IfNetVerbose_AddToLog("EntityId" + (i + 1) + ": " + entityId);
                    KinematicPartsEntityId[i] = entityId;
                }
                else
                {
                    MyMwcLog.IfNetVerbose_AddToLog("EntityId" + (i + 1) + ": " + "null");
                    KinematicPartsEntityId[i] = null;
                }
            }

            return true;
        }
    }
}