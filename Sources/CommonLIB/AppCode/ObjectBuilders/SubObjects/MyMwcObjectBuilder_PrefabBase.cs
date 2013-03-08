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
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.Prefabs;

namespace MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects
{
    /// <summary>
    /// The available materials (styles) for prefabs based on the major factions.
    /// </summary>
    public enum MyMwcObjectBuilder_Prefab_AppearanceEnum : ushort
    {
        None = 0,
        FourthReich = 1,
        Chinese = 2,
        Euroamerican = 3,
        Omnicorp = 4,
        Russian = 5,
        Church = 6,
        Saudi = 7,
    }

    public abstract class MyMwcObjectBuilder_PrefabBase : MyMwcObjectBuilder_SubObjectBase
    {        
        public MyMwcObjectBuilder_Prefab_AppearanceEnum FactionAppearance;
        public MyMwcVector3Short PositionInContainer;
        //Important - values in this vector contain 3 angles in radians
        public Vector3 AnglesInContainer;
        public float? PrefabMaxHealth;
        public float PrefabHealthRatio;
        public string DisplayName;
        public float ElectricCapacity;
        protected int PrefabObjectBuilderId;
        public MyMwcObjectBuilder_EntityUseProperties UseProperties;
        public bool? CausesAlarm;
        public bool? RequiresEnergy;
        public int AIPriority;

        protected MyMwcObjectBuilder_PrefabBase() : base()
        {
            PersistentFlags |= MyPersistentEntityFlags.Enabled;
        }

        protected MyMwcObjectBuilder_PrefabBase(int prefabObjectBuilderId, MyMwcObjectBuilder_Prefab_AppearanceEnum appearance,
            MyMwcVector3Short position, Vector3 anglesInContainer, float? prefabMaxHealth, float prefabHealthRatio, string displayName,
            float electricCapacity, bool? causesAlarm, int aiPriority, bool requiresEnergy = false)
            : this()
        {
            SetObjectBuilderId(prefabObjectBuilderId);
            //PrefabObjectBuilderId = prefabObjectBuilderId;
            FactionAppearance = appearance;
            PositionInContainer = position;
            AnglesInContainer = anglesInContainer;
            PrefabMaxHealth = prefabMaxHealth;
            PrefabHealthRatio = prefabHealthRatio;
            DisplayName = displayName;
            ElectricCapacity = electricCapacity;
            CausesAlarm = causesAlarm;
            RequiresEnergy = requiresEnergy;            
            AIPriority = aiPriority;
        }

        public override MyMwcObjectBuilderTypeEnum GetObjectBuilderType()
        {
            return MyMwcObjectBuilderTypeEnum.Prefab;
        }

        public override int? GetObjectBuilderId()
        {
            return PrefabObjectBuilderId;
        }

        public override void SetDefaultProperties()
        {
            base.SetDefaultProperties();
            this.PrefabHealthRatio = 1f; // MyGameplayConstants.MAX_HEALTH_MAX;
        }

        internal override void SetObjectBuilderIdInternal(int? objectBuilderId)
        {
            PrefabObjectBuilderId = objectBuilderId.Value;
        }

        //  Write this object into message-out
        internal override void Write(BinaryWriter binaryWriter)
        {
            base.Write(binaryWriter);

            // PrefabType
            MyMwcLog.IfNetVerbose_AddToLog("PrefabObjectBuilderId: " + PrefabObjectBuilderId);
            MyMwcMessageOut.WriteInt32(PrefabObjectBuilderId, binaryWriter);

            // PositionInContainer
            MyMwcLog.IfNetVerbose_AddToLog("PositionInContainer: " + PositionInContainer);
            MyMwcMessageOut.WriteVector3Short(PositionInContainer, binaryWriter);

            // AnglesInContainer
            MyMwcLog.IfNetVerbose_AddToLog("AnglesInContainer: " + AnglesInContainer.ToString());
            MyMwcMessageOut.WriteVector3(AnglesInContainer, binaryWriter);

            bool hasMaxHealth = PrefabMaxHealth != null;
            MyMwcMessageOut.WriteBool(hasMaxHealth, binaryWriter);

            if (hasMaxHealth)
            {
                if (float.IsNaN(PrefabMaxHealth.Value) || float.IsInfinity(PrefabMaxHealth.Value))
                {
                    System.Diagnostics.Debug.Fail("PrefabMaxHealth is: " + PrefabMaxHealth.Value);
                    PrefabMaxHealth = null; // MyGameplayConstants.MAX_HEALTH_MAX;
            }
 
                MyMwcLog.IfNetVerbose_AddToLog("PrefabMaxHealth: " + PrefabMaxHealth.Value.ToString());
                binaryWriter.Write(PrefabMaxHealth.Value);
            }

            //health ratio cannot be 0, prefab is dead 
            System.Diagnostics.Debug.Assert(PrefabHealthRatio != 0, "Prefab health ratio is 0");
            System.Diagnostics.Debug.Assert(PrefabHealthRatio <= 1f, "Prefab health ratio is greater than 1f");
            System.Diagnostics.Debug.Assert(PrefabHealthRatio >= 0f, "Prefab health ratio is lesser than 0f");

            // PrefabHealthRatio
            MyMwcLog.IfNetVerbose_AddToLog("PrefabHealthRatio: " + PrefabHealthRatio.ToString());
            binaryWriter.Write(PrefabHealthRatio);

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

            // faction appearance (texture set)
            MyMwcLog.IfNetVerbose_AddToLog("FactionAppearance: " + FactionAppearance);
            MyMwcMessageOut.WriteInt16((short)FactionAppearance, binaryWriter);

            //  Use Properties
            bool hasUseProperties = UseProperties != null;
            MyMwcMessageOut.WriteBool(hasUseProperties, binaryWriter);
            if (hasUseProperties) UseProperties.Write(binaryWriter);

            //  Causes Alarm
            bool hasCausesAlarm = CausesAlarm.HasValue;
            MyMwcMessageOut.WriteBool(hasCausesAlarm, binaryWriter);
            if (hasCausesAlarm) MyMwcMessageOut.WriteBool(CausesAlarm.Value, binaryWriter);

            // Requires Energy
            bool hasRequiresEnergy = RequiresEnergy.HasValue;
            MyMwcMessageOut.WriteBool(hasRequiresEnergy, binaryWriter);
            if (hasRequiresEnergy) MyMwcMessageOut.WriteBool(RequiresEnergy.Value, binaryWriter);

            //MyMwcLog.IfNetVerbose_AddToLog("ElectricCapacity: " + ElectricCapacity.ToString());
            //binaryWriter.Write(ElectricCapacity);

            // AI Priority
            MyMwcLog.IfNetVerbose_AddToLog("AIPriority: " + AIPriority);
            MyMwcMessageOut.WriteInt32(AIPriority, binaryWriter);
        }

        //  Read this object from message-in
        internal override bool Read(BinaryReader binaryReader, EndPoint senderEndPoint, int gameVersion)
        {
            if (base.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();

            if (gameVersion > 01085000)
            {
                return Read_Current(binaryReader, senderEndPoint, gameVersion);
            }
            else
            {
                return Read_01085000(binaryReader, senderEndPoint, gameVersion);   
            }
        }

        private bool Read_Current(BinaryReader binaryReader, EndPoint senderEndPoint, int gameVersion)
        {
            // PrefabType
            int? prefabObjectBuilderId = MyMwcMessageIn.ReadInt32Ex(binaryReader, senderEndPoint);
            if (prefabObjectBuilderId == null) return NetworkError();
            PrefabObjectBuilderId = prefabObjectBuilderId.Value;
            MyMwcLog.IfNetVerbose_AddToLog("PrefabObjectBuilderId: " + PrefabObjectBuilderId);

            // PositionInContainer
            MyMwcVector3Short? positionInContainer = MyMwcMessageIn.ReadVector3ShortEx(binaryReader, senderEndPoint);
            if (positionInContainer == null) return NetworkError();
            PositionInContainer = positionInContainer.Value;
            MyMwcLog.IfNetVerbose_AddToLog("PositionInContainer: " + PositionInContainer);

            // AnglesInContainer
            Vector3? anglesInContainer = MyMwcMessageIn.ReadVector3FloatEx(binaryReader, senderEndPoint);
            if (anglesInContainer == null) return NetworkError();
            AnglesInContainer = anglesInContainer.Value;
            MyMwcLog.IfNetVerbose_AddToLog("AnglesInContainer: " + AnglesInContainer.ToString());

            bool? hasMaxHealth = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
            if (!hasMaxHealth.HasValue) return NetworkError();
            if (hasMaxHealth.Value)
            {
                // PrefabMaxHealth
                float? prefabMaxHealth = MyMwcMessageIn.ReadFloat(binaryReader);
                if (prefabMaxHealth == null) return NetworkError();
                PrefabMaxHealth = prefabMaxHealth.Value;
                MyMwcLog.IfNetVerbose_AddToLog("PrefabMaxHealth: " + PrefabMaxHealth.Value.ToString());
                if (float.IsNaN(PrefabMaxHealth.Value) || float.IsInfinity(PrefabMaxHealth.Value))
                {
                    System.Diagnostics.Debug.Fail("PrefabMaxHealth is: " + PrefabMaxHealth.Value);
                    PrefabMaxHealth = null; // MyGameplayConstants.MAX_HEALTH_MAX;
                }
            }
            else
            {
                MyMwcLog.IfNetVerbose_AddToLog("PrefabMaxHealth is: null");
                PrefabMaxHealth = null;
            }

            // PrefabHealthRatio
            float? prefabHealthRatio = MyMwcMessageIn.ReadFloat(binaryReader);
            if (prefabHealthRatio == null) return NetworkError();
            PrefabHealthRatio = prefabHealthRatio.Value;
            MyMwcLog.IfNetVerbose_AddToLog("PrefabHealth: " + PrefabHealthRatio.ToString());

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

            // faction appearance (texture set)
            short? factionAppearance = MyMwcMessageIn.ReadInt16Ex(binaryReader, senderEndPoint);
            if (!factionAppearance.HasValue) return NetworkError();
            FactionAppearance = (MyMwcObjectBuilder_Prefab_AppearanceEnum)factionAppearance.Value;
            MyMwcLog.IfNetVerbose_AddToLog("FactionAppearance: " + FactionAppearance);

            //  Use Properties
            bool? hasUseProperties = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
            if (hasUseProperties == null) return NetworkError();
            if (hasUseProperties.Value)
            {
                UseProperties =
                    MyMwcObjectBuilder_Base.ReadAndCreateNewObject(binaryReader, senderEndPoint) as
                    MyMwcObjectBuilder_EntityUseProperties;
                if (UseProperties == null) return NetworkError();
                if (UseProperties.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();
            }
            else
            {
                UseProperties = null;
            }

            // Causes Alarm
            bool? hasCausesAlarm = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
            if (!hasCausesAlarm.HasValue) return NetworkError();
            if (hasCausesAlarm.Value)
            {
                bool? causesAlarm = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
                if (!causesAlarm.HasValue) return NetworkError();
                MyMwcLog.IfNetVerbose_AddToLog("CausesAlarm: " + CausesAlarm);
                CausesAlarm = causesAlarm.Value;
            }
            else
            {
                MyMwcLog.IfNetVerbose_AddToLog("CausesAlarm: " + "null");
                CausesAlarm = null;
            }

            // Requires Energy
            bool? hasRequiresEnergy = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
            if (!hasRequiresEnergy.HasValue) return NetworkError();
            if (hasRequiresEnergy.Value)
            {
                bool? requiresEnergy = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
                if (!requiresEnergy.HasValue) return NetworkError();
                MyMwcLog.IfNetVerbose_AddToLog("RequiresEnergy: " + RequiresEnergy);
                RequiresEnergy = requiresEnergy.Value;
            }
            else
            {
                MyMwcLog.IfNetVerbose_AddToLog("RequiresEnergy: " + "null");
                RequiresEnergy = null;
            }

            // AI Priority
            int? aiPriority = MyMwcMessageIn.ReadInt32Ex(binaryReader, senderEndPoint);
            if (aiPriority == null) return NetworkError();
            AIPriority = aiPriority.Value;
            MyMwcLog.IfNetVerbose_AddToLog("AIPriority: " + AIPriority);

            return true;
        }

        private bool Read_01085000(BinaryReader binaryReader, EndPoint senderEndPoint, int gameVersion)
        {
// PrefabType
            int? prefabObjectBuilderId = MyMwcMessageIn.ReadInt32Ex(binaryReader, senderEndPoint);
            if (prefabObjectBuilderId == null) return NetworkError();
            PrefabObjectBuilderId = prefabObjectBuilderId.Value;
            MyMwcLog.IfNetVerbose_AddToLog("PrefabObjectBuilderId: " + PrefabObjectBuilderId);

            // PositionInContainer
            MyMwcVector3Short? positionInContainer = MyMwcMessageIn.ReadVector3ShortEx(binaryReader, senderEndPoint);
            if (positionInContainer == null) return NetworkError();
            PositionInContainer = positionInContainer.Value;
            MyMwcLog.IfNetVerbose_AddToLog("PositionInContainer: " + PositionInContainer);

            // AnglesInContainer
            Vector3? anglesInContainer = MyMwcMessageIn.ReadVector3FloatEx(binaryReader, senderEndPoint);
            if (anglesInContainer == null) return NetworkError();
            AnglesInContainer = anglesInContainer.Value;
            MyMwcLog.IfNetVerbose_AddToLog("AnglesInContainer: " + AnglesInContainer.ToString());

            bool? hasMaxHealth = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
            if (!hasMaxHealth.HasValue) return NetworkError();
            if (hasMaxHealth.Value)
            {
                // PrefabMaxHealth
                float? prefabMaxHealth = MyMwcMessageIn.ReadFloat(binaryReader);
                if (prefabMaxHealth == null) return NetworkError();
                PrefabMaxHealth = prefabMaxHealth.Value;
                MyMwcLog.IfNetVerbose_AddToLog("PrefabMaxHealth: " + PrefabMaxHealth.Value.ToString());
                if (float.IsNaN(PrefabMaxHealth.Value) || float.IsInfinity(PrefabMaxHealth.Value))
                {
                    System.Diagnostics.Debug.Fail("PrefabMaxHealth is: " + PrefabMaxHealth.Value);
                    PrefabMaxHealth = null; // MyGameplayConstants.MAX_HEALTH_MAX;
                }
            }
            else
            {
                MyMwcLog.IfNetVerbose_AddToLog("PrefabMaxHealth is: null");
                PrefabMaxHealth = null;
            }

            // PrefabHealthRatio
            float? prefabHealthRatio = MyMwcMessageIn.ReadFloat(binaryReader);
            if (prefabHealthRatio == null) return NetworkError();
            PrefabHealthRatio = prefabHealthRatio.Value;
            MyMwcLog.IfNetVerbose_AddToLog("PrefabHealth: " + PrefabHealthRatio.ToString());

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

            // faction appearance (texture set)
            short? factionAppearance = MyMwcMessageIn.ReadInt16Ex(binaryReader, senderEndPoint);
            if (!factionAppearance.HasValue) return NetworkError();
            FactionAppearance = (MyMwcObjectBuilder_Prefab_AppearanceEnum) factionAppearance.Value;
            MyMwcLog.IfNetVerbose_AddToLog("FactionAppearance: " + FactionAppearance);

            //  Use Properties
            bool? hasUseProperties = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
            if (hasUseProperties == null) return NetworkError();
            if (hasUseProperties.Value)
            {
                UseProperties =
                    MyMwcObjectBuilder_Base.ReadAndCreateNewObject(binaryReader, senderEndPoint) as
                    MyMwcObjectBuilder_EntityUseProperties;
                if (UseProperties == null) return NetworkError();
                if (UseProperties.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();
            }
            else
            {
                UseProperties = null;
            }

            // Causes Alarm
            bool? hasCausesAlarm = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
            if (!hasCausesAlarm.HasValue) return NetworkError();
            if (hasCausesAlarm.Value)
            {
                bool? causesAlarm = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
                if (!causesAlarm.HasValue) return NetworkError();
                MyMwcLog.IfNetVerbose_AddToLog("CausesAlarm: " + CausesAlarm);
                CausesAlarm = causesAlarm.Value;
            }
            else
            {
                MyMwcLog.IfNetVerbose_AddToLog("CausesAlarm: " + "null");
                CausesAlarm = null;
            }

            // Requires Energy
            bool? hasRequiresEnergy = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
            if (!hasRequiresEnergy.HasValue) return NetworkError();
            if (hasRequiresEnergy.Value)
            {
                bool? requiresEnergy = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
                if (!requiresEnergy.HasValue) return NetworkError();
                MyMwcLog.IfNetVerbose_AddToLog("RequiresEnergy: " + RequiresEnergy);
                RequiresEnergy = requiresEnergy.Value;
            }
            else
            {
                MyMwcLog.IfNetVerbose_AddToLog("RequiresEnergy: " + "null");
                RequiresEnergy = null;
            }

            // Display HUD
            bool? hasDisplayHud = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
            if (!hasDisplayHud.HasValue) return NetworkError();
            if (hasDisplayHud.Value)
            {
                bool? displayHud = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
                if (!displayHud.HasValue) return NetworkError();
                if (displayHud.Value)
                    PersistentFlags |= MyPersistentEntityFlags.DisplayOnHud;
                else
                    PersistentFlags &= ~MyPersistentEntityFlags.DisplayOnHud;
                MyMwcLog.IfNetVerbose_AddToLog("DisplayHud: " + displayHud.Value);
            }
            else
            {
                MyMwcLog.IfNetVerbose_AddToLog("DisplayHud: " + "null");
            }

            // AI Priority
            int? aiPriority = MyMwcMessageIn.ReadInt32Ex(binaryReader, senderEndPoint);
            if (aiPriority == null) return NetworkError();
            AIPriority = aiPriority.Value;
            MyMwcLog.IfNetVerbose_AddToLog("AIPriority: " + AIPriority);

            return true;
        }
    }
}
