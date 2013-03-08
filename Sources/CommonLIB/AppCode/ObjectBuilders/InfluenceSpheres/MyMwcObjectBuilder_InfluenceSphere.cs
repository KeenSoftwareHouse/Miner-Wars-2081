using System;
using System.IO;
using System.Net;
using MinerWarsMath;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.CommonLIB.AppCode.Networking;
using SysUtils.Utils;

namespace MinerWars.CommonLIB.AppCode.ObjectBuilders.InfluenceSpheres
{
    // Do not change numbers, these are saved in DB
    [Flags]
    public enum MyInfluenceFlags
    {
        None = 0,
        Dust = 1 << 0,
        Sound = 1 << 1,
        Radioactivity = 1 << 2,
    }

    //public enum MyInfluenceSphereSoundsEnum
    //{
    //    MUS_STORY_AMBIENT = 0
    //}

    public class MyMwcObjectBuilder_InfluenceSphere : MyMwcObjectBuilder_Object3dBase
    {
        public MyInfluenceFlags InfluenceFlags;

        public float RadiusMin;
        public float RadiusMax;
        public Color DustColor;
        public float Magnitude;
        //public MyInfluenceSphereSoundsEnum SoundType;
        public short SoundCueId;

        #region Influence types

        public bool IsDust
        {
            get
            {
                return (InfluenceFlags & MyInfluenceFlags.Dust) != 0;
            }
            set
            {
                if (value)
                {
                    InfluenceFlags |= MyInfluenceFlags.Dust;
                }
                else
                {
                    InfluenceFlags &= (~MyInfluenceFlags.Dust);
                }
            }
        }

        public bool IsRadioactivity
        {
            get
            {
                return (InfluenceFlags & MyInfluenceFlags.Radioactivity) != 0;
            }
            set
            {
                if (value)
                {
                    InfluenceFlags |= MyInfluenceFlags.Radioactivity;
                }
                else
                {
                    InfluenceFlags &= (~MyInfluenceFlags.Radioactivity);
                }
            }
        }

        public bool IsSound
        {
            get
            {
                return (InfluenceFlags & MyInfluenceFlags.Sound) != 0;
            }
            set
            {
                if (value)
                {
                    InfluenceFlags |= MyInfluenceFlags.Sound;
                }
                else
                {
                    InfluenceFlags &= (~MyInfluenceFlags.Sound);
                }
            }
        }

        #endregion

        //  Write this object into message-out
        public override MyMwcObjectBuilderTypeEnum GetObjectBuilderType()
        {
            return MyMwcObjectBuilderTypeEnum.InfluenceSphere;
        }

        internal override void Write(BinaryWriter binaryWriter)
        {
            base.Write(binaryWriter);

            // Type
            MyMwcLog.IfNetVerbose_AddToLog("InfluenceFlags: " + InfluenceFlags);
            MyMwcMessageOut.WriteInt16((short) InfluenceFlags, binaryWriter);

            // RadiusMin
            MyMwcLog.IfNetVerbose_AddToLog("RadiusMin: " + RadiusMin);
            MyMwcMessageOut.WriteFloat(RadiusMin, binaryWriter);

            // RadiusMax
            MyMwcLog.IfNetVerbose_AddToLog("RadiusMax: " + RadiusMax);
            MyMwcMessageOut.WriteFloat(RadiusMax, binaryWriter);

            // Dust Color
            MyMwcLog.IfNetVerbose_AddToLog("DustColor: " + DustColor.ToString());
            MyMwcMessageOut.WriteColor(DustColor, binaryWriter);

            // Magnitude
            MyMwcLog.IfNetVerbose_AddToLog("Magnitude: " + Magnitude);
            MyMwcMessageOut.WriteFloat(Magnitude, binaryWriter);

            //// Sound Type
            //MyMwcLog.IfNetVerbose_AddToLog("SoundType: " + SoundType);
            //MyMwcMessageOut.WriteInt16((short) SoundType, binaryWriter);

            // Sound Cue Id
            MyMwcLog.IfNetVerbose_AddToLog("SoundCueId: " + SoundCueId);
            MyMwcMessageOut.WriteInt16((short)SoundCueId, binaryWriter);
        }

        //  Read this object from message-in
        internal override bool Read(BinaryReader binaryReader, EndPoint senderEndPoint, int gameVersion)
        {
            if (base.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();

            // Type
            short? flags = MyMwcMessageIn.ReadInt16Ex(binaryReader, senderEndPoint);
            if (flags == null) return NetworkError();
            InfluenceFlags = (MyInfluenceFlags) flags.Value;
            MyMwcLog.IfNetVerbose_AddToLog("InfluenceFlags: " + InfluenceFlags);

            // RadiusMin
            float radiusMin = MyMwcMessageIn.ReadFloat(binaryReader);
            RadiusMin = radiusMin;
            MyMwcLog.IfNetVerbose_AddToLog("RadiusMin: " + RadiusMin);

            // RadiusMax
            float radiusMax = MyMwcMessageIn.ReadFloat(binaryReader);
            RadiusMax = radiusMax;
            MyMwcLog.IfNetVerbose_AddToLog("RadiusMax: " + RadiusMax);

            // Dust color
            Color? dustColor = MyMwcMessageIn.ReadColorEx(binaryReader, senderEndPoint);
            if (dustColor == null) return NetworkError();
            MyMwcLog.IfNetVerbose_AddToLog("dustColor: " + dustColor.ToString());
            DustColor = dustColor.Value;

            // Magnitude
            float magnitude = MyMwcMessageIn.ReadFloat(binaryReader);
            MyMwcLog.IfNetVerbose_AddToLog("Magnitude: " + magnitude);
            Magnitude = magnitude;

            //// Sound
            //short? soundType = MyMwcMessageIn.ReadInt16Ex(binaryReader, senderEndPoint);
            //if (soundType == null) return NetworkError();
            //SoundType = (MyInfluenceSphereSoundsEnum) soundType.Value;
            //MyMwcLog.IfNetVerbose_AddToLog("SoundType: " + SoundType);

            // Sound Cue Id
            short? soundCueId = MyMwcMessageIn.ReadInt16Ex(binaryReader, senderEndPoint);
            if (soundCueId == null) return NetworkError();
            SoundCueId = soundCueId.Value;
            MyMwcLog.IfNetVerbose_AddToLog("SoundCueId: " + SoundCueId);

            return true;
        }
    }
}
