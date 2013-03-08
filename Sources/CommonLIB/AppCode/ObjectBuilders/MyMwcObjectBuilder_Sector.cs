using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using KeenSoftwareHouse.Library.Trace;
using MinerWarsMath;
using MinerWarsMath.Graphics;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using MinerWars.CommonLIB.AppCode.Utils;
using SysUtils.Utils;

namespace MinerWars.CommonLIB.AppCode.ObjectBuilders
{
    public class MyMwcObjectBuilder_Sector : MyMwcObjectBuilder_Base
    {
        // Area template, designer can choose sector area template
        // When null, area is taken from universe generator
        public MySolarSystemAreaEnum? AreaTemplate { get; set; }

        // Number from <0;1>, interpolation between default area and template
        // This number is used only for generator, it's never saved to database
        public float AreaMultiplier { get; set; }

        public MyMwcVector3Int Position;
        public bool FromGenerator { get; set; }
        public int Version { get; set; }

        public List<MyMwcObjectBuilder_Base> SectorObjects { get; set; }
        public List<MyMwcObjectBuilder_ObjectGroup> ObjectGroups { get; set; }
        public List<MyMwcObjectBuilder_SnapPointLink> SnapPointLinks { get; set; }

        public Dictionary<string, string> Dictionary { get; set; }

        public MyMwcObjectBuilder_Sector()
            : base()
        {
            Dictionary = new Dictionary<string, string>();
        }

        public override void RemapEntityIds(IMyEntityIdRemapContext remapContext)
        {
            base.RemapEntityIds(remapContext);
            foreach (var sectorObj in SectorObjects)
            {
                sectorObj.RemapEntityIds(remapContext);
            }

            foreach (var link in SnapPointLinks)
            {
                link.RemapEntityIds(remapContext);
            }
        }

        public static MyMwcObjectBuilder_Sector UseGenerator()
        {
            var sectorBuilder = MyMwcObjectBuilder_Base.CreateNewObject(MyMwcObjectBuilderTypeEnum.Sector, null) as MyMwcObjectBuilder_Sector;
            sectorBuilder.FromGenerator = true;
            sectorBuilder.Version = 0;
            return sectorBuilder;
        }

        public override MyMwcObjectBuilderTypeEnum GetObjectBuilderType()
        {
            return MyMwcObjectBuilderTypeEnum.Sector;
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public void WriteTrace()
        {
            var smallShips = SectorObjects.OfType<MyMwcObjectBuilder_SmallShip>();
            MyTrace.Indent(TraceWindow.Saving, "SmallShips");
            foreach (var smallShip in smallShips)
            {
                MyTrace.Send(TraceWindow.Saving, smallShip.DisplayName);
            }
            MyTrace.UnIndent(TraceWindow.Saving);
        }

        //  Write this object into message-out
        internal override void Write(BinaryWriter binaryWriter)
        {
            base.Write(binaryWriter);

            // TODO: remove
            MyMwcLog.IfNetVerbose_AddToLog("DustColor: " + Color.White.ToString());
            MyMwcMessageOut.WriteColor(Color.White, binaryWriter);

            List<MyMwcObjectBuilder_Base> objectToWrite = SectorObjects;

            int countSectorObjects = objectToWrite == null ? 0 : objectToWrite.Count;
            MyMwcLog.IfNetVerbose_AddToLog("Sector object count: " + countSectorObjects);
            MyMwcMessageOut.WriteInt32(countSectorObjects, binaryWriter);

            for (int i = 0; i < countSectorObjects; i++)
            {
                objectToWrite[i].Write(binaryWriter);
            }
            MyMwcLog.IfNetVerbose_AddToLog("Sector objects written");

            // Write object groups
            int countObjectGroups = 0;
            if (ObjectGroups != null)
            {
                countObjectGroups = ObjectGroups.Count;
            }
            MyMwcLog.IfNetVerbose_AddToLog("Object groups count: " + countObjectGroups);
            MyMwcMessageOut.WriteInt32(countObjectGroups, binaryWriter);

            for (int i = 0; i < countObjectGroups; i++)
            {
                ObjectGroups[i].Write(binaryWriter);
            }
            MyMwcLog.IfNetVerbose_AddToLog("Object groups written");

            // Write snap point links
            int countSnapPointLinks = 0;
            if (SnapPointLinks != null)
            {
                countSnapPointLinks = SnapPointLinks.Count;
            }
            MyMwcLog.IfNetVerbose_AddToLog("Snap point links count: " + countSnapPointLinks);
            MyMwcMessageOut.WriteInt32(countSnapPointLinks, binaryWriter);

            for (int i = 0; i < countSnapPointLinks; i++)
            {
                SnapPointLinks[i].Write(binaryWriter);
            }
            MyMwcLog.IfNetVerbose_AddToLog("Snap point links written");

            MyMwcMessageOut.WriteBool(FromGenerator, binaryWriter);
            MyMwcLog.IfNetVerbose_AddToLog("From generator: " + FromGenerator);

            // Version
            MyMwcMessageOut.WriteInt32(Version, binaryWriter);
            MyMwcLog.IfNetVerbose_AddToLog("Current sector version: " + Version.ToString());

            MyMwcMessageOut.WriteVector3Int(Position, binaryWriter);
            MyMwcLog.IfNetVerbose_AddToLog("Position written: " + Position);

            MyMessageHelper.WriteStringDictionary(Dictionary, binaryWriter);
        }

        //  Read this object from message-in
        internal override bool Read(BinaryReader binaryReader, EndPoint senderEndPoint, int gameVersion)
        {
            if (base.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();

            // TODO: remove
            Color? dustColor = MyMwcMessageIn.ReadColorEx(binaryReader, senderEndPoint);
            if (dustColor == null) return NetworkError();
            MyMwcLog.IfNetVerbose_AddToLog("dustColor: " + dustColor.ToString());

            int? countSectorObjects = MyMwcMessageIn.ReadInt32Ex(binaryReader, senderEndPoint);
            if (countSectorObjects == null) return NetworkError();
            MyMwcLog.IfNetVerbose_AddToLog("Sector object count: " + countSectorObjects);
            SectorObjects = new List<MyMwcObjectBuilder_Base>(countSectorObjects.Value);
            for (int i = 0; i < countSectorObjects; i++)
            {
                MyMwcObjectBuilder_Base sectorObject = MyMwcObjectBuilder_Base.ReadAndCreateNewObject(binaryReader, senderEndPoint);
                if (sectorObject == null) return NetworkError();
                if (sectorObject.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();
                SectorObjects.Add(sectorObject);
            }
            MyMwcLog.IfNetVerbose_AddToLog("Sector objects read");

            // Read object groups
            int? countObjectGroups = MyMwcMessageIn.ReadInt32Ex(binaryReader, senderEndPoint);
            if (countObjectGroups == null) return NetworkError();
            MyMwcLog.IfNetVerbose_AddToLog("Object groups count: " + countObjectGroups);
            ObjectGroups = new List<MyMwcObjectBuilder_ObjectGroup>();
            for (int i = 0; i < countObjectGroups; i++)
            {
                MyMwcObjectBuilder_ObjectGroup objectGroup = MyMwcObjectBuilder_ObjectGroup.ReadAndCreateNewObject(binaryReader, senderEndPoint) as MyMwcObjectBuilder_ObjectGroup;
                if (objectGroup == null) return NetworkError();
                if (objectGroup.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();
                ObjectGroups.Add(objectGroup);
            }
            MyMwcLog.IfNetVerbose_AddToLog("Object groups read");

            // Read snap point links
            int? countSnapPointLinks = MyMwcMessageIn.ReadInt32Ex(binaryReader, senderEndPoint);
            if (countSnapPointLinks == null) return NetworkError();
            MyMwcLog.IfNetVerbose_AddToLog("Snap point links count: " + countSnapPointLinks);
            SnapPointLinks = new List<MyMwcObjectBuilder_SnapPointLink>();
            for (int i = 0; i < countSnapPointLinks; i++)
            {
                MyMwcObjectBuilder_SnapPointLink snapPointLink = MyMwcObjectBuilder_SnapPointLink.ReadAndCreateNewObject(binaryReader, senderEndPoint) as MyMwcObjectBuilder_SnapPointLink;
                if (snapPointLink == null) return NetworkError();
                if (snapPointLink.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();
                SnapPointLinks.Add(snapPointLink);
            }
            MyMwcLog.IfNetVerbose_AddToLog("Snap point links read");

            bool? fromGenerator = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
            if (!fromGenerator.HasValue) return NetworkError();
            FromGenerator = fromGenerator.Value;
            MyMwcLog.IfNetVerbose_AddToLog("From generator read: " + FromGenerator);

            // Version
            int? version = MyMwcMessageIn.ReadInt32Ex(binaryReader, senderEndPoint);
            if (!version.HasValue) return NetworkError();
            this.Version = version.Value;

            var position = MyMwcMessageIn.ReadVector3IntEx(binaryReader, senderEndPoint);
            if (!position.HasValue) return NetworkError();
            Position = position.Value;
            MyMwcLog.IfNetVerbose_AddToLog("Position: " + Position);

            Dictionary = MyMessageHelper.ReadStringDictionary(binaryReader, senderEndPoint);
            if (Dictionary == null) return NetworkError();

            return true;
        }
    }
}
