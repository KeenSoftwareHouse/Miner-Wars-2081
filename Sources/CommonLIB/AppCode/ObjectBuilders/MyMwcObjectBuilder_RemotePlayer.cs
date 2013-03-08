using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.CommonLIB.AppCode.Networking;
using System.Net;

namespace MinerWars.CommonLIB.AppCode.ObjectBuilders
{
    public class MyMwcObjectBuilder_RemotePlayer: MyMwcObjectBuilder_Base
    {
        public EndPoint Endpoint;

        public byte GameUserId;
        public int UserId;
        public uint SmallShipEntityId;

        public override MyMwcObjectBuilderTypeEnum GetObjectBuilderType()
        {
            return MyMwcObjectBuilderTypeEnum.RemotePlayer;
        }

        internal override bool Read(System.IO.BinaryReader binaryReader, System.Net.EndPoint senderEndPoint, int gameVersion)
        {
            if (!base.Read(binaryReader, senderEndPoint, gameVersion)) return NetworkError();

            var endpoint = MyMwcMessageIn.ReadEndpointEx(binaryReader, senderEndPoint);
            if (endpoint == null) return NetworkError();
            Endpoint = endpoint;

            var gameUserId = MyMwcMessageIn.ReadByteEx(binaryReader, senderEndPoint);
            if (!gameUserId.HasValue) return NetworkError();
            GameUserId = gameUserId.Value;

            var userId = MyMwcMessageIn.ReadInt32Ex(binaryReader, senderEndPoint);
            if (!userId.HasValue) return NetworkError();
            UserId = userId.Value;

            var smallShipEntityId = MyMwcMessageIn.ReadUInt32Ex(binaryReader, senderEndPoint);
            if (!smallShipEntityId.HasValue) return NetworkError();
            SmallShipEntityId = smallShipEntityId.Value;

            return true;
        }

        internal override void Write(System.IO.BinaryWriter binaryWriter)
        {
            base.Write(binaryWriter);

            MyMwcMessageOut.WriteEndpoint(Endpoint, binaryWriter);
            MyMwcMessageOut.WriteByte(GameUserId, binaryWriter);
            MyMwcMessageOut.WriteInt32(UserId, binaryWriter);
            MyMwcMessageOut.WriteUInt32(SmallShipEntityId, binaryWriter);
        }
    }
}
