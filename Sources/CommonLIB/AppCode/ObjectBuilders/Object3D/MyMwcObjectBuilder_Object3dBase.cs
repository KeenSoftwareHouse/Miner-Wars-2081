using System.IO;
using System.Net;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.Networking;
using System.Data.SqlClient;
using System;
using MinerWarsMath;
using System.Collections.Generic;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;

//  Object builder is object that defines how to create instance of particular MyPhysObject**
//  MyMwcObjectBuilderBase3D - base for all object builder classes that have 3D position and orientation

namespace MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D
{
    public abstract class MyMwcObjectBuilder_Object3dBase : MyMwcObjectBuilder_Base
    {
        public MyMwcPositionAndOrientation PositionAndOrientation;

        protected MyMwcObjectBuilder_Object3dBase()
            : base()
        {
        }

        protected MyMwcObjectBuilder_Object3dBase(MyMwcPositionAndOrientation positionAndOrientation)
        {
            PositionAndOrientation = positionAndOrientation;
        }

        //  Write this object into message-out
        internal override void Write(BinaryWriter binaryWriter)
        {
            base.Write(binaryWriter);

            MyMwcLog.IfNetVerbose_AddToLog("PositionAndOrientation: " + PositionAndOrientation.ToString());
            MyMwcMessageOut.WritePositionAndOrientation(PositionAndOrientation, binaryWriter);
        }

        //  Read this object from message-in
        internal override bool Read(BinaryReader binaryReader, EndPoint senderEndPoint, int gameVersion)
        {
            if (base.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();

            MyMwcPositionAndOrientation? objectPositionAndOrientation = MyMwcMessageIn.ReadPositionAndOrientationEx(binaryReader, senderEndPoint);
            if (objectPositionAndOrientation == null) return NetworkError();
            MyMwcLog.IfNetVerbose_AddToLog("objectPositionAndOrientation: " + objectPositionAndOrientation.ToString());

            PositionAndOrientation = objectPositionAndOrientation.Value;
            return true;
        }
    }
}
