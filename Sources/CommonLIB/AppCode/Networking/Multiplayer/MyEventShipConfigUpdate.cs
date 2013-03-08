using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using System.Net;
using Lidgren.Network;

namespace MinerWars.CommonLIB.AppCode.Networking.Multiplayer
{
    public struct MyEventShipConfigUpdate : IMyEvent
    {
        [Flags]
        enum ConfigValue : int
        {
            NONE = 0,
            ENGINE = 1 << 0,
            REFLECTOR_ON = 1 << 1,
            REFLECTOR_LONG_RANGE = 1 << 2,
            AUTOLEVELING = 1 << 3,
            SLOWDOWN = 1 << 4,
            JAMMER = 1 << 5,
        }

        ConfigValue m_config
        {
            get
            {
                ConfigValue result = ConfigValue.NONE;
                if (EngineOn) result |= ConfigValue.ENGINE;
                if (ReflectorOn) result |= ConfigValue.REFLECTOR_ON;
                if (ReflectorLongRange) result |= ConfigValue.REFLECTOR_LONG_RANGE;
                if (Autoleveling) result |= ConfigValue.AUTOLEVELING;
                if (Slowdown) result |= ConfigValue.SLOWDOWN;
                if (RadarJammer) result |= ConfigValue.JAMMER;
                return result;
            }
            set
            {
                EngineOn = (value & ConfigValue.ENGINE) != 0;
                ReflectorOn = (value & ConfigValue.REFLECTOR_ON) != 0;
                ReflectorLongRange = (value & ConfigValue.REFLECTOR_LONG_RANGE) != 0;
                Autoleveling = (value & ConfigValue.AUTOLEVELING) != 0;
                Slowdown = (value & ConfigValue.SLOWDOWN) != 0;
                RadarJammer = (value & ConfigValue.JAMMER) != 0;
            }
        }

        public bool EngineOn;
        public bool ReflectorOn;
        public bool ReflectorLongRange;
        public bool Autoleveling;
        public bool Slowdown;
        public bool RadarJammer;

        public int TimeBombTimer;
        public uint ShipId;

        public bool Read(MyMessageReader msg)
        {
            int configValues = 0;

            var result = msg.ReadInt32(ref configValues)
                && msg.ReadInt32(ref TimeBombTimer)
                && msg.ReadUInt32(ref ShipId);

            m_config = (ConfigValue)configValues;
            return result;
        }

        public void Write(MyMessageWriter msg)
        {
            msg.WriteInt32((int)m_config);
            msg.WriteInt32(TimeBombTimer);
            msg.WriteUInt32(ShipId);
        }

        public NetConnection SenderConnection { get; set; }
        public IPEndPoint SenderEndpoint { get; set; }
        public MyEventEnum EventType { get { return MyEventEnum.SHIP_CONFIG_UPDATE; } }
    }
}
