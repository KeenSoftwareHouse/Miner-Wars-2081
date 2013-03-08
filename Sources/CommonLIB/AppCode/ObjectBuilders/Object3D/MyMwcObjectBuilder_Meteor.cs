using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWarsMath;
using SysUtils.Utils;

namespace MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D
{
    public enum MyMwcObjectBuilder_Meteor_TypesEnum
    {
        DEFAULT = 0,
    }
    public class MyMwcObjectBuilder_Meteor : MyMwcObjectBuilder_StaticAsteroid
    {
        public Vector3 Direction;
        public int? EffectID;

        public override MyMwcObjectBuilderTypeEnum GetObjectBuilderType()
        {
            return MyMwcObjectBuilderTypeEnum.Meteor;
        }
        public override int? GetObjectBuilderId()
        {
            return (int)MyMwcObjectBuilder_Meteor_TypesEnum.DEFAULT;
        }

        public MyMwcObjectBuilder_Meteor()
        {
        }

        public MyMwcObjectBuilder_Meteor(Vector3 direction, int? effectID)
        {
            Direction = direction;
            EffectID = effectID;
        }
        public MyMwcObjectBuilder_Meteor(MyMwcObjectBuilder_StaticAsteroid_TypesEnum asteroidType, MyMwcVoxelMaterialsEnum material)
            : base(asteroidType, material)
        {
        }

        internal override void Write(System.IO.BinaryWriter binaryWriter)
        {
            base.Write(binaryWriter);

            // Direction
            MyMwcLog.IfNetVerbose_AddToLog("Direction: " + Direction);
            MyMwcMessageOut.WriteVector3(Direction, binaryWriter);

            // EffectID
            MyMwcLog.IfNetVerbose_AddToLog("MyMwcObjectBuilder_Meteor.EffectID.HasValue: " + this.EffectID.HasValue);
            MyMwcMessageOut.WriteBool(this.EffectID.HasValue, binaryWriter);
            if (this.EffectID.HasValue)
            {
                MyMwcLog.IfNetVerbose_AddToLog("MyMwcObjectBuilder_Meteor.EffectID.Value: " + this.EffectID.Value);
                MyMwcMessageOut.WriteInt16((byte)this.EffectID.Value, binaryWriter);
            }
        }

        internal override bool Read(System.IO.BinaryReader binaryReader, System.Net.EndPoint senderEndPoint, int gameVersion)
        {
            if (base.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();

            // Direction
            Vector3? direction = MyMwcMessageIn.ReadVector3FloatEx(binaryReader, senderEndPoint);
            if (direction == null) return NetworkError();
            Direction = direction.Value;
            MyMwcLog.IfNetVerbose_AddToLog("Direction: " + Direction);

            // Effect ID
            bool? hasId = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
            if (!hasId.HasValue) return NetworkError(); // Cannot read bool 
            MyMwcLog.IfNetVerbose_AddToLog("MyMwcObjectBuilder_Meteor.EffectID.HasValue: " + hasId.Value);

            // Testing whether Effect ID is null
            if (hasId.Value)
            {
                // effect id has value - read the value
                EffectID = MyMwcMessageIn.ReadInt16(binaryReader);

                MyMwcLog.IfNetVerbose_AddToLog("MyMwcObjectBuilder_Meteor.EffectID.Value: " + EffectID.Value);
            }
            else
            {
                this.EffectID = null;
            }

            return true;
        }
    }
}
