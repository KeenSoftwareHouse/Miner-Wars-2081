using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.Networking;

namespace MinerWars.CommonLIB.AppCode.ObjectBuilders
{
    public class MyMwcObjectBuilder_ShipConfig : MyMwcObjectBuilder_Base
    {
        public bool Engine;
        public byte RadarType;
        public bool AutoLeveling;
        public bool MovementSlowdown;
        public bool BackCamera;
        public byte ViewMode;

        public MyMwcObjectBuilder_ShipConfig()
        {

        }

        public MyMwcObjectBuilder_ShipConfig(bool engine, byte radarType, bool autoLeveling, bool movementSlowdown, bool backCamera, byte viewMode)
        {
            Engine = engine;
            RadarType = radarType;
            AutoLeveling = autoLeveling;
            MovementSlowdown = movementSlowdown;
            BackCamera = backCamera;
            ViewMode = viewMode;
        }

        public override MyMwcObjectBuilderTypeEnum GetObjectBuilderType()
        {
            return MyMwcObjectBuilderTypeEnum.ShipConfig;
        }

        internal override void Write(BinaryWriter binaryWriter)
        {
            base.Write(binaryWriter);

            // Engine
            MyMwcLog.IfNetVerbose_AddToLog("Engine: " + Engine);
            MyMwcMessageOut.WriteBool(Engine, binaryWriter);

            // RadarType
            MyMwcLog.IfNetVerbose_AddToLog("RadarType: " + RadarType);
            MyMwcMessageOut.WriteByte(RadarType, binaryWriter);

            // BackCamera
            MyMwcLog.IfNetVerbose_AddToLog("BackCamera: " + BackCamera);
            MyMwcMessageOut.WriteBool(BackCamera, binaryWriter);

            // MovementSlowdown
            MyMwcLog.IfNetVerbose_AddToLog("MovementSlowdown: " + MovementSlowdown);
            MyMwcMessageOut.WriteBool(MovementSlowdown, binaryWriter);

            // ViewMode
            MyMwcLog.IfNetVerbose_AddToLog("ViewMode: " + ViewMode);
            MyMwcMessageOut.WriteByte(ViewMode, binaryWriter);
        }

        internal override bool Read(BinaryReader binaryReader, System.Net.EndPoint senderEndPoint, int gameVersion)
        {
            if (!base.Read(binaryReader, senderEndPoint, gameVersion)) return NetworkError();

            // Engine
            bool? engine = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
            if (engine == null) return NetworkError();
            Engine = engine.Value;
            MyMwcLog.IfNetVerbose_AddToLog("Engine: " + Engine);

            // RadarType
            byte? radarType = MyMwcMessageIn.ReadByteEx(binaryReader, senderEndPoint);
            if (radarType == null) return NetworkError();
            RadarType = radarType.Value;
            MyMwcLog.IfNetVerbose_AddToLog("RadarType: " + RadarType);

            // BackCamera
            bool? backCamera = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
            if (backCamera == null) return NetworkError();
            BackCamera = backCamera.Value;
            MyMwcLog.IfNetVerbose_AddToLog("BackCamera: " + BackCamera);

            // MovementSlowdown
            bool? movementSlowdown = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
            if (movementSlowdown == null) return NetworkError();
            MovementSlowdown = movementSlowdown.Value;
            MyMwcLog.IfNetVerbose_AddToLog("MovementSlowdown: " + MovementSlowdown);

            // ViewMode
            byte? viewMode = MyMwcMessageIn.ReadByteEx(binaryReader, senderEndPoint);
            if (viewMode == null) return NetworkError();
            ViewMode = viewMode.Value;
            MyMwcLog.IfNetVerbose_AddToLog("ViewMode: " + ViewMode);

            return true;
        }
    }
}
