//  Object builder is object that defines how to create instance of particular MyPhysObject**
//  MyMwcObjectBuilderBaseSubObject is base for all objects that don't have position/orientation, but are placed on another 3D object (for example weapons)

using System.Data.SqlClient;
using System;
using MinerWars.CommonLIB.AppCode.Networking;
using SysUtils.Utils;

namespace MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects
{
    public abstract class MyMwcObjectBuilder_SubObjectBase : MyMwcObjectBuilder_Base
    {
        protected MyMwcObjectBuilder_SubObjectBase() : base()
        {
        }

        public const string AUTO_MOUNT_DESCRIPTOR = "AUTO";
        public const string LEFT_SIDE_MOUNT_DESCRIPTOR = "AUTO_LEFT";
        public const string RIGHT_SIDE_MOUNT_DESCRIPTOR = "AUTO_RIGHT";

        /// <summary>
        /// Provides the key of the slot in which the item is mounted. If null, the item is not mounted.
        /// </summary>
        public string MountedDescriptor;

        internal override void Write(System.IO.BinaryWriter binaryWriter)
        {
            base.Write(binaryWriter);

            // Mounted descriptor
            MyMwcLog.IfNetVerbose_AddToLog("MountedDescriptor: " + MountedDescriptor);
            MyMwcMessageOut.WriteNullableString(MountedDescriptor, binaryWriter);
        }

        internal override bool Read(System.IO.BinaryReader binaryReader, System.Net.EndPoint senderEndPoint, int gameVersion)
        {
            if (!base.Read(binaryReader, senderEndPoint, gameVersion))
                return false;

            // MountedDescriptor
            if (MyMwcMessageIn.ReadNullableStringEx(binaryReader, senderEndPoint, out MountedDescriptor))
            {
                MyMwcLog.IfNetVerbose_AddToLog("MountedDescriptor: " + MountedDescriptor);
            }
            else
            {
                MyMwcLog.IfNetVerbose_AddToLog("MountedDescriptor: null");
            }

            return true;
        }

        #region Mount methods

        public bool AutoMountLeft
        {
            get { return String.Compare(MountedDescriptor, LEFT_SIDE_MOUNT_DESCRIPTOR, StringComparison.OrdinalIgnoreCase) == 0; }
        }

        public void SetAutoMountLeft()
        {
            MountedDescriptor = LEFT_SIDE_MOUNT_DESCRIPTOR;
        }

        public bool AutoMountRight
        {
            get { return String.Compare(MountedDescriptor, RIGHT_SIDE_MOUNT_DESCRIPTOR, StringComparison.OrdinalIgnoreCase) == 0; }
        }

        public void SetAutoMountRight()
        {
            MountedDescriptor = RIGHT_SIDE_MOUNT_DESCRIPTOR;
        }

        public bool AutoMount
        {
            get { return String.Compare(MountedDescriptor, AUTO_MOUNT_DESCRIPTOR, StringComparison.OrdinalIgnoreCase) == 0; }
        }

        public void SetAutoMount()
        {
            MountedDescriptor = AUTO_MOUNT_DESCRIPTOR;
        }

        #endregion
    }
}
