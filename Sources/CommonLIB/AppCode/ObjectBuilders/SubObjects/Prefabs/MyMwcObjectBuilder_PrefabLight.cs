using System;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWarsMath;
using System.IO;
using System.Net;
using System.Data.SqlClient;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.Networking;
using System.Text;

namespace MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.Prefabs
{
    //  IMPORTANT: Never change numeric values and never delete any enum item. These numbers are referenced e.g. from database.
    public enum MyMwcObjectBuilder_PrefabLight_TypesEnum : ushort
    {
        DEFAULT_LIGHT_0 = 259,
        P521_A01_LIGHT1 = 260,
        P521_A02_LIGHT2 = 261,
        P521_A03_LIGHT3 = 262,
        P521_A04_LIGHT4 = 263,
    }

    public enum MyLightEffectTypeEnum : short
    {
        NORMAL = 0,//default
        CONSTANT_FLASHING = 1,// sinus curve
        RANDOM_FLASHING = 2,// every 100ms change to on//off
        DISTANT_GLARE = 3, // distant light, that has glare visible from far away
        DISTANT_GLARE_FLASHING = 4, // the same, but regular flashing
        DISTANT_GLARE_RANDOM_FLASHING = 5, // the same, but random flashing
    };

    public enum MyLightPrefabTypeEnum : short
    {
        NOT_ASSIGNED = 0,
        POINT_LIGHT = (1 << 0),
        SPOT_LIGHT = (1 << 1),
        HEMISPHERIC_LIGHT = (1 << 2),
        //addhere more, always add to end in incremental fashion .. dont mess up with prev version saved data
    };
    
    public class MyMwcObjectBuilder_PrefabLight : /*MyMwcObjectBuilder_Prefab*/ MyMwcObjectBuilder_PrefabBase
    {
        //
        public Vector4 PointColor;
        public Vector3 PointSpecular;
        public float PointIntensity;
        public float PointFalloff;
        public float PointRange;
        public float PointOffset;

        public float FlashOffset;

        [Obsolete("Use prefab light type, remove this!")]
        public bool PointEnabled;

        //
        public Vector4 SpotColor;
        public Vector3 SpotSpecular;
        public float SpotIntensity;
        public float SpotFalloff;
        public float SpotRange;


        public float ShadowsDistance;


        [Obsolete("Use prefab light type, remove this!")]
        public bool SpotEnabled;
        public float SpotAgle;//0-150

        //MyLightPrefabTypeEnum m_Type;
        public MyLightEffectTypeEnum Effect;

        public MyLightPrefabTypeEnum LightType = MyLightPrefabTypeEnum.POINT_LIGHT;
        

        internal MyMwcObjectBuilder_PrefabLight()
            : base()
        {
        }

        /// <summary>
        /// c-tor
        /// </summary>
        /// <param name="prefabType"></param>
        /// <param name="position"></param>
        /// <param name="anglesInContainer"></param>
        public MyMwcObjectBuilder_PrefabLight(MyMwcObjectBuilder_PrefabLight_TypesEnum prefabType, MyMwcObjectBuilder_Prefab_AppearanceEnum appearance, MyMwcVector3Short position, Vector3 anglesInContainer, float? prefabMaxHealth, float prefabHealthRatio, string displayName, float electricCapacity, bool? causesAlarm, int aiPriority)
            : base((int)prefabType, appearance, position, anglesInContainer, prefabMaxHealth, prefabHealthRatio, displayName, electricCapacity, causesAlarm, aiPriority)
        {
            PointFalloff = SpotFalloff = 1.0f;
            PointIntensity = SpotIntensity = 1.0f;
            SpotRange = SpotRange = 100.0f;
            PointColor = SpotColor = new Vector4(1, 1, 1, 1);
            PointSpecular = SpotSpecular = new Vector3(0, 0, 0);
            PointOffset = 0;
            Effect = MyLightEffectTypeEnum.NORMAL;
            SpotAgle = 1;
            ShadowsDistance = 200;
        }

        /// <summary>
        /// GetObjectBuilderType
        /// </summary>
        /// <returns></returns>
        public override MyMwcObjectBuilderTypeEnum GetObjectBuilderType()
        {
            return MyMwcObjectBuilderTypeEnum.PrefabLight;
        }

        public MyMwcObjectBuilder_PrefabLight_TypesEnum PrefaLightType
        {
            get
            {
                return (MyMwcObjectBuilder_PrefabLight_TypesEnum) GetObjectBuilderId().Value;
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
            //
            MyMwcLog.IfNetVerbose_AddToLog("m_PointColor: " + PointColor);
            MyMwcMessageOut.WriteVector4(PointColor, binaryWriter);
            MyMwcLog.IfNetVerbose_AddToLog("m_PointSpecular: " + PointSpecular);
            MyMwcMessageOut.WriteVector3(PointSpecular, binaryWriter);
            MyMwcLog.IfNetVerbose_AddToLog("m_PointIntensity: " + PointIntensity);
            MyMwcMessageOut.WriteFloat(PointIntensity, binaryWriter);
            MyMwcLog.IfNetVerbose_AddToLog("m_PointFalloff: " + PointFalloff);
            MyMwcMessageOut.WriteFloat(PointFalloff, binaryWriter);
            MyMwcLog.IfNetVerbose_AddToLog("m_PointRange: " + PointRange);
            MyMwcMessageOut.WriteFloat(PointRange, binaryWriter);
            MyMwcLog.IfNetVerbose_AddToLog("Point offset: " + PointOffset);
            MyMwcMessageOut.WriteFloat(PointOffset, binaryWriter);

            MyMwcLog.IfNetVerbose_AddToLog("m_SpotColor: " + SpotColor);
            MyMwcMessageOut.WriteVector4(SpotColor, binaryWriter);
            MyMwcLog.IfNetVerbose_AddToLog("m_SpotSpecular: " + SpotSpecular);
            MyMwcMessageOut.WriteVector3(SpotSpecular, binaryWriter);
            MyMwcLog.IfNetVerbose_AddToLog("m_SpotIntensity: " + SpotIntensity);
            MyMwcMessageOut.WriteFloat(SpotIntensity, binaryWriter);
            MyMwcLog.IfNetVerbose_AddToLog("m_SpotFalloff: " + SpotFalloff);
            MyMwcMessageOut.WriteFloat(SpotFalloff, binaryWriter);
            MyMwcLog.IfNetVerbose_AddToLog("m_SpotRange: " + SpotRange);
            MyMwcMessageOut.WriteFloat(SpotRange, binaryWriter);
            MyMwcLog.IfNetVerbose_AddToLog("m_SpotAgle: " + SpotAgle);
            MyMwcMessageOut.WriteFloat(SpotAgle, binaryWriter);

            // m_Effect
            MyMwcLog.IfNetVerbose_AddToLog("Effect: " + Effect);
            MyMwcMessageOut.WriteInt16((short)Effect, binaryWriter);

            MyMwcLog.IfNetVerbose_AddToLog("LightType: " + LightType);
            MyMwcMessageOut.WriteInt16((short)LightType, binaryWriter);

            // FlashOffset
            MyMwcLog.IfNetVerbose_AddToLog("FlashOffset: " + FlashOffset);
            MyMwcMessageOut.WriteFloat(FlashOffset, binaryWriter);    

            
            //shadows
            MyMwcLog.IfNetVerbose_AddToLog("ShadowsDistance: " + ShadowsDistance);
            MyMwcMessageOut.WriteFloat(ShadowsDistance, binaryWriter);   
            
        }

        //  Read this object from message-in
        internal override bool Read(BinaryReader binaryReader, EndPoint senderEndPoint, int gameVersion)
        {
            if (base.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();

            //////////////////////////////////// POINT PROPERTIES ////////////////////////////////////
            Vector4 ?pPC = MyMwcMessageIn.ReadVector4FloatEx(binaryReader, senderEndPoint);
            if (pPC == null) return NetworkError();
            PointColor = pPC.Value;
            MyMwcLog.IfNetVerbose_AddToLog("m_PointColor: " + PointColor);
            //

            Vector3 ? pPS = MyMwcMessageIn.ReadVector3FloatEx(binaryReader, senderEndPoint);
            if (pPS == null) return NetworkError();
            PointSpecular = pPS.Value;
            MyMwcLog.IfNetVerbose_AddToLog("m_PointSpecular: " + PointSpecular);

            //
            float? pPI = MyMwcMessageIn.ReadFloat(binaryReader);
            if (pPI == null) return NetworkError();
            PointIntensity = pPI.Value;
            MyMwcLog.IfNetVerbose_AddToLog("m_PointIntensity: " + PointIntensity);

            //
            float? pPF = MyMwcMessageIn.ReadFloat(binaryReader);
            if (pPF == null) return NetworkError();
            PointFalloff = pPF.Value;
            MyMwcLog.IfNetVerbose_AddToLog("m_PointFalloff: " + PointFalloff);

            //
            float? pPR = MyMwcMessageIn.ReadFloat(binaryReader);
            if (pPR == null) return NetworkError();
            PointRange = pPR.Value;
            MyMwcLog.IfNetVerbose_AddToLog("m_PointRange: " + PointRange);

            // Point offset
            float? offset = MyMwcMessageIn.ReadFloat(binaryReader);
            if (offset == null) return NetworkError();
            PointOffset = offset.Value;
            MyMwcLog.IfNetVerbose_AddToLog("Point offset: " + PointOffset);


            //////////////////////////////////// SPOT PROPERTIES ////////////////////////////////////
            Vector4? pSC = MyMwcMessageIn.ReadVector4FloatEx(binaryReader, senderEndPoint);
            if (pSC == null) return NetworkError();
            SpotColor = pSC.Value;
            MyMwcLog.IfNetVerbose_AddToLog("m_SpotColor: " + SpotColor);
            //

            Vector3? pSS = MyMwcMessageIn.ReadVector3FloatEx(binaryReader, senderEndPoint);
            if (pSS == null) return NetworkError();
            SpotSpecular = pSS.Value;
            MyMwcLog.IfNetVerbose_AddToLog("m_SpotSpecular: " + SpotSpecular);


            float? pSI = MyMwcMessageIn.ReadFloat(binaryReader);
            if (pSI == null) return NetworkError();
            SpotIntensity = pSI.Value;
            MyMwcLog.IfNetVerbose_AddToLog("m_SpotIntensity: " + SpotIntensity);

            //
            float? pSF = MyMwcMessageIn.ReadFloat(binaryReader);
            if (pSF == null) return NetworkError();
            SpotFalloff = pSF.Value;
            MyMwcLog.IfNetVerbose_AddToLog("m_SpotFalloff: " + SpotFalloff);

            //
            float? pSR = MyMwcMessageIn.ReadFloat(binaryReader);
            if (pSR == null) return NetworkError();
            SpotRange = pSR.Value;
            MyMwcLog.IfNetVerbose_AddToLog("m_SpotRange: " + SpotRange);

            //
            float? pSA = MyMwcMessageIn.ReadFloat(binaryReader);
            if (pSA == null) return NetworkError();
            SpotAgle = pSA.Value;
            MyMwcLog.IfNetVerbose_AddToLog("m_SpotAgle: " + SpotAgle);

            // m_Effect
            short? tEffect = MyMwcMessageIn.ReadInt16Ex(binaryReader, senderEndPoint);
            if (tEffect == null) return NetworkError();
            Effect = (MyLightEffectTypeEnum)tEffect.Value;

            // light type
            short? tLightType = MyMwcMessageIn.ReadInt16Ex(binaryReader, senderEndPoint);
            if (tLightType == null) return NetworkError();
            LightType = (MyLightPrefabTypeEnum) tLightType.Value;

            // FlashOffset
            float? flashOffset = MyMwcMessageIn.ReadFloat(binaryReader);
            if (flashOffset == null) return NetworkError();
            FlashOffset = flashOffset.Value;

            
            // ShadowsEnabled
            float? shadowsDistance = MyMwcMessageIn.ReadFloat(binaryReader);
            if (shadowsDistance == null) return NetworkError();
            ShadowsDistance = shadowsDistance.Value;
            

            return true;
        }
    }
}
