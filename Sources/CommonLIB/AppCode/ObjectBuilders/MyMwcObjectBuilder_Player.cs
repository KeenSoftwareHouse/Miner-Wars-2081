using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using SysUtils.Utils;

namespace MinerWars.CommonLIB.AppCode.ObjectBuilders
{
    public class MyMwcObjectBuilder_Player : MyMwcObjectBuilder_Base
    {
        public float Health { get; set; }
        public float Money { get; set; }
        public MyMwcObjectBuilder_PlayerStatistics PlayerStatisticsObjectBuilder { get; set; }
        public MyMwcObjectBuilder_SmallShip ShipObjectBuilder { get; set; }
        public MyMwcObjectBuilder_ShipConfig ShipConfigObjectBuilder { get; set; }
        public float WithoutOxygen { get; set; }

        internal MyMwcObjectBuilder_Player()
            : base()
        {
        }

        public MyMwcObjectBuilder_Player(float health, float money, float withoutOxygen,
            MyMwcObjectBuilder_PlayerStatistics playerStatisticsObjectBuilder, MyMwcObjectBuilder_SmallShip shipObjectBuilder, MyMwcObjectBuilder_ShipConfig shipConfigObjectBuilder)
        {
            Health = health;
            Money = money;
            WithoutOxygen = withoutOxygen;
            PlayerStatisticsObjectBuilder = playerStatisticsObjectBuilder;
            ShipObjectBuilder = shipObjectBuilder;
            ShipConfigObjectBuilder = shipConfigObjectBuilder;
        }

        public override void RemapEntityIds(IMyEntityIdRemapContext remapContext)
        {
            base.RemapEntityIds(remapContext);
            PlayerStatisticsObjectBuilder.RemapEntityIds(remapContext);
            ShipConfigObjectBuilder.RemapEntityIds(remapContext);
            ShipObjectBuilder.RemapEntityIds(remapContext);
        }

        public override MyMwcObjectBuilderTypeEnum GetObjectBuilderType()
        {
            return MyMwcObjectBuilderTypeEnum.Player;
        }

        internal override void Write(System.IO.BinaryWriter binaryWriter)
        {
            base.Write(binaryWriter);

            // Health
            MyMwcLog.IfNetVerbose_AddToLog("Health: " + Health);
            MyMwcMessageOut.WriteFloat(Health, binaryWriter);

            // Money
            MyMwcLog.IfNetVerbose_AddToLog("Money: " + Money);
            MyMwcMessageOut.WriteFloat(Money, binaryWriter);

            // Without oxygen
            MyMwcLog.IfNetVerbose_AddToLog("WithoutOxygen: " + WithoutOxygen);
            MyMwcMessageOut.WriteFloat(WithoutOxygen, binaryWriter);

            // Player statistic's objectbuilder            
            bool isPlayerStatisticsObjectBuilder = PlayerStatisticsObjectBuilder != null;
            MyMwcMessageOut.WriteBool(isPlayerStatisticsObjectBuilder, binaryWriter);
            if (isPlayerStatisticsObjectBuilder) PlayerStatisticsObjectBuilder.Write(binaryWriter);

            // Ship's objectbuilder            
            bool isShipObjectBuilder = ShipObjectBuilder != null;
            MyMwcMessageOut.WriteBool(isShipObjectBuilder, binaryWriter);
            if (isShipObjectBuilder) ShipObjectBuilder.Write(binaryWriter);

            // ShipConfig's objectbuilder            
            bool isShipConfigObjectBuilder = ShipConfigObjectBuilder != null;
            MyMwcMessageOut.WriteBool(isShipConfigObjectBuilder, binaryWriter);
            if (isShipConfigObjectBuilder) ShipConfigObjectBuilder.Write(binaryWriter);
        }

        internal override bool Read(System.IO.BinaryReader binaryReader, System.Net.EndPoint senderEndPoint, int gameVersion)
        {
            if (!base.Read(binaryReader, senderEndPoint, gameVersion)) return NetworkError();

            // Health
            float? health = MyMwcMessageIn.ReadFloat(binaryReader);
            if (health == null) return NetworkError();
            Health = health.Value;
            MyMwcLog.IfNetVerbose_AddToLog("Health: " + Health);

            // Money
            float? money = MyMwcMessageIn.ReadFloat(binaryReader);
            if (money == null) return NetworkError();
            Money = money.Value;
            MyMwcLog.IfNetVerbose_AddToLog("Money: " + Money);

            // Without oxygen
            float? withoutOxygen = MyMwcMessageIn.ReadFloat(binaryReader);
            if (withoutOxygen == null) return NetworkError();
            WithoutOxygen = withoutOxygen.Value;
            MyMwcLog.IfNetVerbose_AddToLog("WithoutOxygen: " + WithoutOxygen);

            //  Player statistic's objectbuilder
            bool? isPlayerStatisticsObjectBuilder = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
            if (isPlayerStatisticsObjectBuilder == null) return NetworkError();
            if (isPlayerStatisticsObjectBuilder.Value)
            {
                PlayerStatisticsObjectBuilder = MyMwcObjectBuilder_Base.ReadAndCreateNewObject(binaryReader, senderEndPoint) as MyMwcObjectBuilder_PlayerStatistics;
                if (PlayerStatisticsObjectBuilder == null) return NetworkError();
                if (PlayerStatisticsObjectBuilder.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();
            }
            else
            {
                PlayerStatisticsObjectBuilder = null;
            }

            //  Ship's objectbuilder
            bool? isShipObjectBuilder = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
            if (isShipObjectBuilder == null) return NetworkError();
            if (isShipObjectBuilder.Value)
            {
                ShipObjectBuilder = MyMwcObjectBuilder_Base.ReadAndCreateNewObject(binaryReader, senderEndPoint) as MyMwcObjectBuilder_SmallShip;
                if (ShipObjectBuilder == null) return NetworkError();
                if (ShipObjectBuilder.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();
            }
            else
            {
                ShipObjectBuilder = null;
            }

            //  ShipConfig's objectbuilder
            bool? isShipConfigObjectBuilder = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
            if (isShipConfigObjectBuilder == null) return NetworkError();
            if (isShipConfigObjectBuilder.Value)
            {
                ShipConfigObjectBuilder = MyMwcObjectBuilder_Base.ReadAndCreateNewObject(binaryReader, senderEndPoint) as MyMwcObjectBuilder_ShipConfig;
                if (ShipConfigObjectBuilder == null) return NetworkError();
                if (ShipConfigObjectBuilder.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();
            }
            else
            {
                ShipConfigObjectBuilder = null;
            }

            return true;
        }
    }
}
