using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWarsMath;
using System.IO;
using System.Net;
using MinerWars.CommonLIB.AppCode.Networking;
using SysUtils.Utils;


namespace MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects
{
    public enum MyDummyPointType
    {
        Sphere,
        Box
    }

    // Do not change numbers, these are saved in DB
    [Flags]
    public enum MyDummyPointFlags
    {
        NONE =                      0,
        COLOR_AREA =                1 << 0,
        PLAYER_START =              1 << 1,
        DETECTOR =                  1 << 2,
        SIDE_MISSION =              1 << 3,
        SAFE_AREA =                 1 << 4,
        PARTICLE =                  1 << 5,
        MOTHERSHIP_START =          1 << 6,
        SURIVE_PREFAB_DESTRUCTION = 1 << 7,
        RESPAWN_POINT =             1 << 8,
        TEXTURE_QUAD =              1 << 9,
        NOTE =                      1 << 10,
        VOXEL_HAND =                1 << 11,
    }

    public class MyMwcObjectBuilder_DummyPoint : MyMwcObjectBuilder_Object3dBase
    {
        public MyDummyPointType Type { get; set; }
        public Vector3 Size { get; set; }

        public MyDummyPointFlags DummyFlags { get; set; }

        public Color Color { get; set; }

        public float Argument { get; set; }

        public MyMwcObjectBuilder_FactionEnum RespawnPointFaction { get; set; }

        internal MyMwcObjectBuilder_DummyPoint()
            : base()
        {
            Size = Vector3.One;            
        }

        public override int? GetObjectBuilderId()
        {
            //only one main type
            return null;
        }

        public override MyMwcObjectBuilderTypeEnum GetObjectBuilderType()
        {
            return MyMwcObjectBuilderTypeEnum.DummyPoint;
        }

        public override void SetDefaultProperties()
        {
            base.SetDefaultProperties();

            RespawnPointFaction = MyMwcObjectBuilder_FactionEnum.None;
        }

        //  Read this object from message-in
        internal override bool Read(BinaryReader binaryReader, EndPoint senderEndPoint, int gameVersion)
        {
            if (base.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();

            // Type
            ushort? type = MyMwcMessageIn.ReadUInt16Ex(binaryReader, senderEndPoint);
            if (type == null) return NetworkError();
            Type = (MyDummyPointType)type.Value;
            MyMwcLog.IfNetVerbose_AddToLog("Type: " + Type);

            // Size
            Vector3? size = MyMwcMessageIn.ReadVector3FloatEx(binaryReader, senderEndPoint);
            if (size == null) return NetworkError();
            Size = size.Value;
            MyMwcLog.IfNetVerbose_AddToLog("Size: " + Size.ToString());

            // Dummy flags
            int? flagsResult = MyMwcMessageIn.ReadInt32Ex(binaryReader, senderEndPoint);
            if (!flagsResult.HasValue) return NetworkError();

            this.DummyFlags = (MyDummyPointFlags)flagsResult.Value;
            MyMwcLog.IfNetVerbose_AddToLog("DummyFlags: " + this.DummyFlags);

            // Dummy flags
            Color? color = MyMwcMessageIn.ReadColorEx(binaryReader, senderEndPoint);
            if (!color.HasValue) return NetworkError();

            this.Color = color.Value;
            MyMwcLog.IfNetVerbose_AddToLog("DummyColor: " + this.Color);

            // Argument
            Argument = MyMwcMessageIn.ReadFloat(binaryReader);

            // RespawnPointFaction
            int? respawnPointFaction = MyMwcMessageIn.ReadInt32Ex(binaryReader, senderEndPoint);
            if (!respawnPointFaction.HasValue) return NetworkError();
            this.RespawnPointFaction = (MyMwcObjectBuilder_FactionEnum)respawnPointFaction.Value;
            MyMwcLog.IfNetVerbose_AddToLog("RespawnPointFaction: " + this.RespawnPointFaction);

            return true;
        }

        //  Write this object into message-out
        internal override void Write(BinaryWriter binaryWriter)
        {
            base.Write(binaryWriter);

            // Type
            MyMwcLog.IfNetVerbose_AddToLog("Type: " + Type);
            binaryWriter.Write((ushort)Type);

            // Size
            MyMwcLog.IfNetVerbose_AddToLog("Size: " + Size.ToString());
            MyMwcMessageOut.WriteVector3(Size, binaryWriter);

            // Dummy flags
            MyMwcLog.IfNetVerbose_AddToLog("DummyFlags: " + this.DummyFlags);
            MyMwcMessageOut.WriteInt32((int)this.DummyFlags, binaryWriter);

            // Dummy color
            MyMwcLog.IfNetVerbose_AddToLog("DummyColor: " + this.Color);
            MyMwcMessageOut.WriteColor(this.Color, binaryWriter);

            // Argument
            MyMwcLog.IfNetVerbose_AddToLog("Argument: " + this.Argument);
            MyMwcMessageOut.WriteFloat(this.Argument, binaryWriter);

            // RespawnPointFaction
            MyMwcLog.IfNetVerbose_AddToLog("RespawnPointFaction: " + this.RespawnPointFaction);
            MyMwcMessageOut.WriteInt32((int)this.RespawnPointFaction, binaryWriter);
        }
    }
}


