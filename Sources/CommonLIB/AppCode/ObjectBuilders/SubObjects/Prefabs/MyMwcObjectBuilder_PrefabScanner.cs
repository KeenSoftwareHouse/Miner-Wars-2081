using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWarsMath;
using System.IO;
using System.Net;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.Networking;

namespace MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.Prefabs
{
    //  IMPORTANT: Never change numeric values and never delete any enum item. These numbers are referenced e.g. from database.
    public enum MyMwcObjectBuilder_PrefabScanner_TypesEnum : ushort
    {
        Plane = 1,
        Rays = 2,
    }

    public class MyMwcObjectBuilder_PrefabScanner : MyMwcObjectBuilder_PrefabBase
    {
        public Vector3 Size { get; set; }
        public Color Color { get; set; }
        public float ScanningSpeed { get; set; }        

        internal MyMwcObjectBuilder_PrefabScanner()
            : base()
        {
            Size = new Vector3(20f, 20f, 20f);
            Color = new Color(255, 0, 0, 255);
            ScanningSpeed = 10f;            
        }

        public MyMwcObjectBuilder_PrefabScanner(MyMwcObjectBuilder_PrefabScanner_TypesEnum prefabType, MyMwcObjectBuilder_Prefab_AppearanceEnum appearance, MyMwcVector3Short position, Vector3 anglesInContainer, float? prefabMaxHealth, float prefabHealthRatio, string displayName, float electricCapacity, bool? causesAlarm, int aiPriority, Vector3 size, Color color, float scanningSpeed/*, bool on*/)
            : base((int)prefabType, appearance, position, anglesInContainer, prefabMaxHealth, prefabHealthRatio, displayName, electricCapacity, causesAlarm, aiPriority)
        {
            Size = size;
            Color = color;
            ScanningSpeed = scanningSpeed;            
        }

        /// <summary>
        /// GetObjectBuilderType
        /// </summary>
        /// <returns></returns>
        public override MyMwcObjectBuilderTypeEnum GetObjectBuilderType()
        {
            return MyMwcObjectBuilderTypeEnum.PrefabScanner;
        }

        public MyMwcObjectBuilder_PrefabScanner_TypesEnum PrefabScannerType
        {
            get
            {
                return (MyMwcObjectBuilder_PrefabScanner_TypesEnum)GetObjectBuilderId().Value;
            }
            set
            {
                SetObjectBuilderId((int)value);
            }
        }

        //  Write this object into message-out
        internal override void Write(BinaryWriter binaryWriter)
        {
            base.Write(binaryWriter);

            //  Color
            MyMwcLog.IfNetVerbose_AddToLog("Color: " + Color);
            MyMwcMessageOut.WriteColor(Color, binaryWriter);

            //  Size
            MyMwcLog.IfNetVerbose_AddToLog("Size: " + Size);
            MyMwcMessageOut.WriteVector3(Size, binaryWriter);

            //  Scanning speed
            MyMwcLog.IfNetVerbose_AddToLog("ScanningSpeed: " + ScanningSpeed);
            MyMwcMessageOut.WriteFloat(ScanningSpeed, binaryWriter);

            ////  On
            //MyMwcLog.IfNetVerbose_AddToLog("On: " + On);
            //MyMwcMessageOut.WriteBool(On, binaryWriter);
        }

        //  Read this object from message-in
        internal override bool Read(BinaryReader binaryReader, EndPoint senderEndPoint, int gameVersion)
        {
            if (base.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();

            //  Color
            Color? color = MyMwcMessageIn.ReadColorEx(binaryReader, senderEndPoint);
            if (color == null) return NetworkError();
            Color = color.Value;
            MyMwcLog.IfNetVerbose_AddToLog("Color: " + Color);

            //  Size
            Vector3? size = MyMwcMessageIn.ReadVector3FloatEx(binaryReader, senderEndPoint);
            if (size == null) return NetworkError();
            Size = size.Value;
            MyMwcLog.IfNetVerbose_AddToLog("Size: " + Size);

            //  Scanning speed
            float? scanningSpeed = MyMwcMessageIn.ReadFloat(binaryReader);
            if (scanningSpeed == null) return NetworkError();
            ScanningSpeed = scanningSpeed.Value;
            MyMwcLog.IfNetVerbose_AddToLog("ScanningSpeed: " + ScanningSpeed);

            ////  On
            //bool? on = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
            //if (on == null) return NetworkError();
            //On = on.Value;
            //MyMwcLog.IfNetVerbose_AddToLog("On: " + On);

            return true;
        }
    }
}
