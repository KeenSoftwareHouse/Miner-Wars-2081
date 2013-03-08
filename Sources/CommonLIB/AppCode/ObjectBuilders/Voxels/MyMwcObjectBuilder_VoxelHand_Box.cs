using System.IO;
using System.Net;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.Networking;
using System.Data.SqlClient;
using System;
using MinerWarsMath;

namespace MinerWars.CommonLIB.AppCode.ObjectBuilders.Voxels
{
    public class MyMwcObjectBuilder_VoxelHand_Box : MyMwcObjectBuilder_VoxelHand_Shape
    {
        public float Size;
        public float Size2;
        public float Size3;

        internal MyMwcObjectBuilder_VoxelHand_Box()
            : base()
        {
        }

        public MyMwcObjectBuilder_VoxelHand_Box(MyMwcPositionAndOrientation positionAndOrientation, float size, MyMwcVoxelHandModeTypeEnum voxelHandModeType)
            : base(positionAndOrientation, voxelHandModeType)
        {
            Size = size;
            Size3 = Size2 = size; //not fully supported yet
        }

        public override MyMwcObjectBuilderTypeEnum GetObjectBuilderType()
        {
            return MyMwcObjectBuilderTypeEnum.VoxelHand_Box;
        }

        //  Write this object into message-out
        internal override void Write(BinaryWriter binaryWriter)
        {
            base.Write(binaryWriter);
                 
            MyMwcLog.IfNetVerbose_AddToLog("Size: " + Size.ToString());
            MyMwcMessageOut.WriteFloat(Size, binaryWriter);

            MyMwcLog.IfNetVerbose_AddToLog("Size2: " + Size2.ToString());
            MyMwcMessageOut.WriteFloat(Size2, binaryWriter);

            MyMwcLog.IfNetVerbose_AddToLog("Size3: " + Size3.ToString());
            MyMwcMessageOut.WriteFloat(Size3, binaryWriter);                        
        }

        //  Read this object from message-in
        internal override bool Read(BinaryReader binaryReader, EndPoint senderEndPoint, int gameVersion)
        {
            if (gameVersion < 01087004) 
            {
                return ReadLesserThan01087004(binaryReader, senderEndPoint, gameVersion);
            }
            else if (gameVersion < 01087005)
            {
                return ReadLesserThan01087005(binaryReader, senderEndPoint, gameVersion);
            }
            else 
            {
                return ReadCurrent(binaryReader, senderEndPoint, gameVersion);
            }
        }

        private bool ReadLesserThan01087004(BinaryReader binaryReader, EndPoint senderEndPoint, int gameVersion)
        {
            if (base.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();

            float? size = MyMwcMessageIn.ReadFloat(binaryReader);
            if (size == null) return NetworkError();
            Size = size.Value;
            MyMwcLog.IfNetVerbose_AddToLog("Size: " + Size.ToString());

            return true;
        }

        private bool ReadLesserThan01087005(BinaryReader binaryReader, EndPoint senderEndPoint, int gameVersion) 
        {
            if (base.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();

            return true;
        }

        private bool ReadCurrent(BinaryReader binaryReader, EndPoint senderEndPoint, int gameVersion) 
        {
            if (base.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();

            float? size = MyMwcMessageIn.ReadFloat(binaryReader);
            if (size == null) return NetworkError();
            Size = size.Value;
            MyMwcLog.IfNetVerbose_AddToLog("Size: " + Size.ToString());

            float? size2 = MyMwcMessageIn.ReadFloat(binaryReader);
            if (size2 == null) return NetworkError();
            Size2 = size2.Value;
            MyMwcLog.IfNetVerbose_AddToLog("Size2: " + Size2.ToString());

            float? size3 = MyMwcMessageIn.ReadFloat(binaryReader);
            if (size3 == null) return NetworkError();
            Size3 = size3.Value;
            MyMwcLog.IfNetVerbose_AddToLog("Size3: " + Size3.ToString());

            return true;  
        }
    } 
}
