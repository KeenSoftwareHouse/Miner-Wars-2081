using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Lidgren.Network;
using KeenSoftwareHouse.Library.Trace;
using KeenSoftwareHouse.Library.IO;
using System.Net;
using SysUtils.Utils;
using System.Diagnostics;
using MinerWars.CommonLIB.AppCode.Utils;

namespace MinerWars.CommonLIB.AppCode.Networking
{
    public enum MyEventEnum : byte
    {
        // Relay
        RELAY = 0,

        // Client-server
        LOGIN = 1,
        LOGIN_RESPONSE = 2,
        CREATE_GAME = 3,
        CRAETE_GAME_RESPONSE = 4,
        GET_GAMES = 5,
        GET_GAMES_RESPONSE = 6,
        JOIN_GAME = 7,
        JOIN_GAME_RESPONSE = 8,
        PLAYER_STATE_CHANGED = 9,
        REQUEST_INTRODUCTION = 10,
        UPDATE_GAME = 11,
        DIRECT_INTRODUCTION_RESPONSE = 12,
        DISCONNECT = 13,
        LOGIN_STEAM = 14,
        VALIDATE_USER = 15,
        VALIDATE_USER_RESPONSE = 16,

        // Game logic
        NEW_PLAYER = 33,
        CHECKPOINT = 34,
        SET_FACTION = 35,
        RESPAWN = 36,
        STATS_UPDATE = 37,
        DIE = 38,
        ENTER_GAME = 39,
        ENTER_GAME_RESPONSE = 40,
        GET_PLAYER_LIST = 41,
        GET_PLAYER_LIST_RESPONSE = 42,
        CHOOSE_FACTION = 43,
        CHOOSE_FACTION_RESPONSE = 44,
        SPAWN_BOT = 45,
        PILOT_DIE = 46,
        MISSION_PROGRESS = 47,
        NOTIFICATION = 48,

        // Game updates
        SHOOT = 101,
        AMMO_EXPLOSION = 102,
        PROJECTILE_HIT = 103,
        DO_DAMAGE = 104,
        UPDATE_POSITION = 105,
        AMMO_UPDATE = 106,
        WEAPON_EVENT = 107,
        CHAT = 108,
        SHIP_CONFIG_UPDATE = 109,
        INVENTORY_UPDATE = 110,
        LOCK = 111,
        LOCK_RESULT = 112,
        ENTITY_RESET = 113,
        HEALTH_UPDATE = 114,
        AFTERBURNER = 115,
        CUT_OUT = 116,
        FLAGS = 117,
        NEW_ENTITY = 118,
        MISSION_UPDATE_VARS = 119,
        ADD_EXPLOSION = 120,
        DUMMY_FLAGS = 121,
        ADD_VOXEL_HAND = 122,
        UPDATE_POSITION_FAST = 123,
        PLAY_DIALOGUE = 124,
        PLAY_SOUND = 125,
        HEADSHAKE = 126,
        SET_ACTOR_FACTION = 127,
        SET_FACTION_RELATION = 128,
        COUNTDOWN = 129,
        UPDATE_ROTATION_FAST = 130,
        FRIENDLY_FIRE = 131,
        SET_ENTITY_FACTION = 132,
        MUSIC_TRANSITION = 133,
        EVENT = 134,
        GLOBAL_FLAGS = 135,
        SAVE_PLAYER = 136,
    }

    public enum MyLoggingTypeEnum
    {
        NONE,
        NAME,
        FULL,
    }

    public delegate void EventCallback<T>(ref T obj);
    public delegate bool MessageFilterHandler(NetConnection connection, IPEndPoint remoteEndpoint, MyEventEnum eventEnum);

    public class MyLidgrenPeer : NetPeer
    {
        public const int DEFAULT_MAX_SIZE = 512;

        static readonly HashSet<byte> m_allowedMessageTypes;
        readonly HashSet<byte> m_bufferedEvents = new HashSet<byte>();
        readonly Dictionary<byte, Action<NetIncomingMessage, MyEventEnum, IPEndPoint>> m_actions = new Dictionary<byte, Action<NetIncomingMessage, MyEventEnum, IPEndPoint>>();

        readonly List<NetIncomingMessage> m_messageBuffer = new List<NetIncomingMessage>(256);

        readonly Dictionary<IPEndPoint, MyRelayedConnection> m_relayedConnections = new Dictionary<IPEndPoint, MyRelayedConnection>();

        ResetableMemoryStream m_stream;
        BinaryReader m_reader;
        BinaryWriter m_writer;

        // 10 elements should be sufficient
        private List<MyMessageQueueItem> m_messageQueue = new List<MyMessageQueueItem>(10);

        public MessageFilterHandler MessageFilter;

        Dictionary<int, MyLoggingTypeEnum> m_loggedMessages = new Dictionary<int, MyLoggingTypeEnum>();

        public event Action<NetConnection> PeerConnected;
        public event Action<NetConnection> PeerDisconnected;
        public Action<IPEndPoint, string> NatIntroductionSuccess;

        public NetConnection RelayServerConnection;
        public bool IsRelayServer;
        public long RelayedBytes = 0;

        static MyLidgrenPeer()
        {
            m_allowedMessageTypes = new HashSet<byte>(Enum.GetValues(typeof(MyEventEnum)).Cast<byte>());
        }

        public MyLidgrenPeer(NetPeerConfiguration configuration)
            : base(configuration)
        {
            m_stream = new ResetableMemoryStream();
            m_reader = new BinaryReader(m_stream);
            m_writer = new BinaryWriter(m_stream);
        }

        public MyRelayedConnection GetRelayedConnection(IPEndPoint targetEp)
        {
            MyRelayedConnection connection;
            if (!m_relayedConnections.TryGetValue(targetEp, out connection))
            {
                connection = new MyRelayedConnection(this) { RelayTargetEp = targetEp };
                m_relayedConnections[targetEp] = connection;
            }
            return connection;
        }

        public void LogAll()
        {
            foreach (var val in MyMwcEnums.GetAllowedValues<MyEventEnum>())
            {
                m_loggedMessages.Add(val, MyLoggingTypeEnum.NAME);
            }
        }

        public void LogNone()
        {
            m_loggedMessages.Clear();
        }

        public void Log(MyEventEnum eventType, MyLoggingTypeEnum logType = MyLoggingTypeEnum.NAME)
        {
            if (logType != MyLoggingTypeEnum.NONE)
            {
                m_loggedMessages[(int)eventType] = logType;
            }
            else
            {
                m_loggedMessages.Remove((int)eventType);
            }
        }

        public NetConnection Connect<T>(IPEndPoint endpoint, ref T hailEvent, int maxSize = DEFAULT_MAX_SIZE)
            where T : struct, IMyEvent
        {
            var msg = CreateMessage(maxSize);
            Write(msg, hailEvent);
            try
            {
                return Connect(endpoint, msg);
            }
            catch (Exception e)
            {
                MyMwcLog.WriteLine("Error connecting to MP server or peer: ");
                MyMwcLog.WriteLine(e);
                return null;
            }
        }

        public NetConnection Connect<T>(string host, int port, ref T hailEvent, int maxSize = DEFAULT_MAX_SIZE)
            where T : struct, IMyEvent
        {
            var msg = CreateMessage(maxSize);
            Write(msg, hailEvent);
            try
            {
                return Connect(host, port, msg);
            }
            catch (Exception e)
            {
                MyMwcLog.WriteLine("Error connecting to MP server or peer: ");
                MyMwcLog.WriteLine(e);
                return null;
            }
        }

        public void RegisterCallback<T>(EventCallback<T> callback)
            where T : struct, IMyEvent
        {
            byte eventId = (byte)GetEventType<T>();
            m_bufferedEvents.Remove(eventId);
            m_actions[eventId] = (msg, eventType, endpoint) => ProcessCallbackMsg(msg, eventType, endpoint, callback);
        }

        public void RegisterBuffering<T>()
            where T : struct, IMyEvent
        {
            m_bufferedEvents.Add((byte)GetEventType<T>());
        }

        public void ProcessBuffered()
        {
            foreach (var msg in m_messageBuffer)
            {
                ProcessMessage(msg);
                Recycle(msg);
            }
            m_messageBuffer.Clear();
        }

        public void ClearBuffering()
        {
            foreach (var msg in m_messageBuffer)
            {
                Recycle(msg);
            }
            m_messageBuffer.Clear();
            m_bufferedEvents.Clear();
        }

        public void ClearCallbacks()
        {
            m_actions.Clear();
            NatIntroductionSuccess = null;
        }

        public void RemoveCallback<T>()
            where T : struct, IMyEvent
        {
            m_actions.Remove((byte)GetEventType<T>());
        }

        public void Receive()
        {
            NetIncomingMessage msg;
            while ((msg = this.ReadMessage()) != null)
            {
                if (msg.MessageType == NetIncomingMessageType.StatusChanged)
                {
                    var status = (NetConnectionStatus)msg.PeekByte();
                    if (status == NetConnectionStatus.Connected)
                    {
                        RaisePeerConnected(msg.SenderConnection);
                        if (msg.SenderConnection.RemoteHailMessage != null)
                        {
                            var hailMsg = msg.SenderConnection.RemoteHailMessage;
                            hailMsg.m_senderConnection = msg.SenderConnection;
                            hailMsg.m_senderEndpoint = msg.SenderEndpoint;
                            // Recycle hail msg inside
                            OnMessageReceived(hailMsg);
                        }
                        // Process send queue
                        ProcessQueue();
                    }
                    else if (status == NetConnectionStatus.Disconnected)
                    {
                        RaisePeerDisconnected(msg.SenderConnection);
                    }
                    Recycle(msg);
                }
                else if (msg.MessageType == NetIncomingMessageType.NatIntroductionSuccess)
                {
                    RaiseNatIntroductionSuccess(msg.SenderEndpoint, msg.ReadString());
                    Recycle(msg);
                }
                else if (msg.MessageType == NetIncomingMessageType.Data)
                {
                    // Recycle inside
                    OnMessageReceived(msg);
                }
                else if (
                    //msg.MessageType == NetIncomingMessageType.DebugMessage ||
                    msg.MessageType == NetIncomingMessageType.WarningMessage ||
                    msg.MessageType == NetIncomingMessageType.ErrorMessage)
                {
                    var text = msg.ReadString();
                    MyTrace.Send(TraceWindow.MultiplayerAlerts, text);
                    Recycle(msg);
                }
                else
                {
                    Recycle(msg);
                }
            }
        }

        public void OnMessageReceived(NetIncomingMessage msg)
        {
            if (m_bufferedEvents.Contains(msg.PeekByte()))
            {
                m_messageBuffer.Add(msg);
            }
            else
            {
                ProcessMessage(msg);
                Recycle(msg);
            }
        }

        public NetSendResult Send<T>(ref T evnt, NetConnection connection, NetDeliveryMethod deliveryMethod, int sequenceChannel, int maxSize = DEFAULT_MAX_SIZE)
            where T : struct, IMyEvent
        {
            NetOutgoingMessage msg;
            MyRelayedConnection relayedConnection = connection as MyRelayedConnection;

            if (relayedConnection != null)
            {
                if (RelayServerConnection == null)
                {
                    //RaisePeerDisconnected(connection);
                    return NetSendResult.FailedNotConnected;
                }

                maxSize += sizeof(byte); // Event type enum
                maxSize += 16; // 16B for IPv6 addr
                maxSize += sizeof(int); // Port

                msg = CreateMessage(maxSize);

                // Write relay header
                msg.Write((byte)MyEventEnum.RELAY);
                msg.Write(relayedConnection.RelayTargetEp);

                connection = RelayServerConnection;
            }
            else
            {
                msg = CreateMessage(maxSize);
            }

            Write(msg, evnt);
            if (CanEnqueue(connection.Status))
            {
                m_messageQueue.Add(new MyMessageQueueItem() { Message = msg, DeliveryMethod = deliveryMethod, SequenceChannel = sequenceChannel, EndPoint = connection.RemoteEndpoint });
                return NetSendResult.Queued;
            }
            else
            {
                return connection.SendMessage(msg, deliveryMethod, sequenceChannel);
            }
        }

        public void SendToAll<T>(ref T evnt, List<NetConnection> connections, NetDeliveryMethod deliveryMethod, int sequenceChannel, int maxSize = DEFAULT_MAX_SIZE)
            where T : struct, IMyEvent
        {
            CheckConnectionsAreNotRelayed(connections);

            var msg = CreateMessage(maxSize);
            Write(msg, evnt);
            SendMessage(msg, connections, deliveryMethod, sequenceChannel);
        }

        [Conditional("DEBUG")]
        private void CheckConnectionsAreNotRelayed(List<NetConnection> connections)
        {
            foreach (var c in connections)
            {
                Debug.Assert(!(c is MyRelayedConnection), "Networking error, call OndrejPetrzilka");
            }
        }

        private MyEventEnum GetEventType<T>()
            where T : struct, IMyEvent
        {
            return new T().EventType;
        }

        private void ProcessQueue()
        {
            int i = 0;
            // Normally there would be zero messages
            while (i < m_messageQueue.Count)
            {
                var connection = GetConnection(m_messageQueue[i].EndPoint);
                if (connection != null)
                {
                    if (CanEnqueue(connection.Status))
                    {
                        i++;
                    }
                    else if (connection.Status == NetConnectionStatus.Connected)
                    {
                        connection.SendMessage(m_messageQueue[i].Message, m_messageQueue[i].DeliveryMethod, m_messageQueue[i].SequenceChannel);
                        m_messageQueue.RemoveAt(i);
                    }
                    else
                    {
                        m_messageQueue.RemoveAt(i);
                    }
                }
                else
                {
                    m_messageQueue.RemoveAt(i);
                }
            }
        }

        private bool CanEnqueue(NetConnectionStatus status)
        {
            return status == NetConnectionStatus.None
                || status == NetConnectionStatus.InitiatedConnect
                || status == NetConnectionStatus.RespondedAwaitingApproval
                || status == NetConnectionStatus.RespondedConnect;
        }

        private void RaisePeerConnected(NetConnection connection)
        {
            var handler = PeerConnected;
            if (handler != null)
            {
                handler(connection);
            }
        }

        private void RaisePeerDisconnected(NetConnection connection)
        {
            var handler = PeerDisconnected;
            if (handler != null && connection != null)
            {
                handler(connection);
            }
        }

        private void RaiseNatIntroductionSuccess(IPEndPoint endpoint, string token)
        {
            var handler = NatIntroductionSuccess;
            if (handler != null)
            {
                handler(endpoint, token);
            }
        }

        private void Write<T>(NetOutgoingMessage msg, T multiplayerEvent)
            where T : struct, IMyEvent
        {
            m_stream.Reset(msg.Data);
            m_stream.Position = msg.LengthBytes;
            m_writer.Write((byte)multiplayerEvent.EventType);
            try
            {
                multiplayerEvent.Write(new MyMessageWriter(m_writer));
                msg.LengthBytes = (int)m_writer.BaseStream.Position;
            }
            catch (EndOfStreamException)
            {
                Debug.Fail("Message size is too small for this type of message!");
                MyMwcLog.WriteLine("ERROR: message size is too small for this type of message!");

                // This could happen sometimes, for example for extremely large inventories...
                msg.EnsureBufferSize(msg.LengthBits * 2);
                Write(msg, multiplayerEvent);
            }
        }

        private void ProcessCallbackMsg<T>(NetIncomingMessage msg, MyEventEnum eventType, IPEndPoint endpoint, EventCallback<T> callback)
            where T : struct, IMyEvent
        {
            if (callback == null) return;

            T inst = new T();
            inst.SenderEndpoint = endpoint;
            inst.SenderConnection = msg.SenderConnection;
            if (ReadMessage(msg, ref inst))
            {
                LogMessage<T>(eventType, inst);
                callback(ref inst);
            }
            else
            {
                MyMwcCheaterAlert.AddAlert(MyMwcCheaterAlertType.EXCEPTION_DURING_READING_MESSAGE, msg.SenderEndpoint, "Invalid message, message type: " + (int)inst.EventType);
            }
        }

        [Conditional("DEBUG")]
        private void LogMessage<T>(MyEventEnum eventType, T inst)
            where T : struct, IMyEvent
        {
            MyLoggingTypeEnum loggingType;
            if (m_loggedMessages.TryGetValue((int)eventType, out loggingType))
            {
                switch (loggingType)
                {
                    case MyLoggingTypeEnum.NONE:
                        break;
                    case MyLoggingTypeEnum.NAME:
                        MyTrace.Send(TraceWindow.Multiplayer, "Received event " + eventType.ToString() + ", from " + (inst.SenderEndpoint != null ? inst.SenderEndpoint.ToString() : "null"));
                        break;
                    case MyLoggingTypeEnum.FULL:
                        MyTrace.Send(TraceWindow.Multiplayer, inst.ToString());
                        break;
                    default:
                        break;
                }
            }
        }

        private void ProcessMessage(NetIncomingMessage msg)
        {
            byte msgType;
            if (!msg.ReadByte(out msgType))
            {
                MyMwcCheaterAlert.AddAlert(MyMwcCheaterAlertType.EXCEPTION_DURING_READING_MESSAGE, msg.SenderEndpoint, "Cannot read message type");
                return;
            }

            if (msgType == (byte)MyEventEnum.RELAY)
            {
                ProcessRelayedMessage(msg);
                return;
            }

            ProcessMessageBody(msg, msgType);
        }

        private void ProcessMessageBody(NetIncomingMessage msg, byte msgType)
        {
            if (!m_allowedMessageTypes.Contains(msgType))
            {
                MyMwcCheaterAlert.AddAlert(MyMwcCheaterAlertType.EXCEPTION_DURING_READING_MESSAGE, msg.SenderEndpoint, "Unknown message type: " + msgType);
                return;
            }

            var handler = MessageFilter;
            if (handler != null && !handler(msg.SenderConnection, msg.SenderEndpoint, (MyEventEnum)msgType))
            {
                return;
            }

            Action<NetIncomingMessage, MyEventEnum, IPEndPoint> callback;
            if (m_actions.TryGetValue(msgType, out callback))
            {
                callback(msg, (MyEventEnum)msgType, msg.SenderEndpoint);
            }
        }

        private void ProcessRelayedMessage(NetIncomingMessage msg)
        {
            var ep = msg.ReadIPEndpoint();

            if (IsRelayServer)
            {
                RelayMessage(msg, ep);
            }
            else
            {
                byte innerMsgType;
                if (!msg.ReadByte(out innerMsgType))
                {
                    MyMwcCheaterAlert.AddAlert(MyMwcCheaterAlertType.EXCEPTION_DURING_READING_MESSAGE, msg.SenderEndpoint, "Cannot read message type of RELAYED message");
                    return;
                }

                msg.m_senderEndpoint = ep;
                msg.m_senderConnection = GetRelayedConnection(ep);

                if (innerMsgType == (byte)MyEventEnum.DISCONNECT)
                {
                    // TODO: msg.SenderConnection.Tag is not player, fix it
                    this.RaisePeerDisconnected(msg.SenderConnection);
                    return;
                }

                ProcessMessageBody(msg, innerMsgType);
            }
        }

        private void RelayMessage(NetIncomingMessage msg, IPEndPoint targetEndpoint)
        {
            RelayedBytes += msg.LengthBytes;

            NetConnection target = GetConnection(targetEndpoint);
            if (target != null && target.Status != NetConnectionStatus.Disconnected && target.Status != NetConnectionStatus.Disconnecting)
            {
                // Rewrite header (header now contains sender EP)
                NetOutgoingMessage outMsg = CreateMessage(msg.LengthBytes);
                outMsg.Write((byte)MyEventEnum.RELAY);
                outMsg.Write(msg.SenderEndpoint);

                int bytesLeft = (msg.LengthBytes - msg.PositionInBytes);
                outMsg.Write(msg.ReadBytes(bytesLeft));
                SendMessage(outMsg, target, msg.DeliveryMethod, msg.SequenceChannel);
            }
            else
            {
                // Send disconnect back to sender
                var response = CreateMessage();
                response.Write((byte)MyEventEnum.RELAY);
                response.Write(targetEndpoint);

                response.Write((byte)MyEventEnum.DISCONNECT);
                SendMessage(response, msg.SenderConnection, NetDeliveryMethod.ReliableOrdered, 0);
            }
        }

        private bool ReadMessage<T>(NetIncomingMessage msg, ref T multiplayerEvent)
            where T : struct, IMyEvent
        {
            m_stream.Reset(msg.Data);
            m_stream.Position = msg.PositionInBytes;
            return multiplayerEvent.Read(new MyMessageReader(m_reader, msg.SenderEndpoint));
        }

        public void Close()
        {
            ClearBuffering();
            ClearCallbacks();
            PeerConnected = null;
            PeerDisconnected = null;
        }
    }
}
