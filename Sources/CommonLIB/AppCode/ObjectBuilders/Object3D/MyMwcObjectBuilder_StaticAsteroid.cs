using System;
using System.IO;
using System.Net;
using MinerWars.CommonLIB.AppCode.Networking;
using SysUtils.Utils;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D
{
    //  IMPORTANT: Never change numeric values and never delete any enum item. These numbers are referenced e.g. from database.
    public enum MyMwcObjectBuilder_StaticAsteroid_TypesEnum : ushort
    {
        //JEROMIE = 1,        //  Jeromie is obsolete, don't use it
        StaticAsteroid10m_A = 2,
        StaticAsteroid20m_A = 3,
        StaticAsteroid30m_A = 4,
        StaticAsteroid50m_A = 5,
        StaticAsteroid100m_A = 6,
        StaticAsteroid300m_A = 7,
        StaticAsteroid500m_A = 8,
        StaticAsteroid1000m_A = 9,
        StaticAsteroid2000m_A = 10,
        StaticAsteroid5000m_A = 11,
        StaticAsteroid10000m_A = 12,

        StaticAsteroid10m_B = 13,
        StaticAsteroid20m_B = 14,
        StaticAsteroid30m_B = 15,
        StaticAsteroid50m_B = 16,
        StaticAsteroid100m_B = 17,
        StaticAsteroid300m_B = 18,
        StaticAsteroid500m_B = 19,
        StaticAsteroid1000m_B = 20,
        StaticAsteroid2000m_B = 21,
        StaticAsteroid5000m_B = 22,
        StaticAsteroid10000m_B = 23,

        //Removed support 
        StaticAsteroid10m_C = 24,
        StaticAsteroid20m_C = 25,
        StaticAsteroid30m_C = 26,
        StaticAsteroid50m_C = 27,
        StaticAsteroid100m_C = 28,
        StaticAsteroid300m_C = 29,
        StaticAsteroid500m_C = 30,
        StaticAsteroid1000m_C = 31,
        StaticAsteroid2000m_C = 32,
        StaticAsteroid5000m_C = 33,
        StaticAsteroid10000m_C = 34,

        //Removed support 
        StaticAsteroid10m_D = 35,
        StaticAsteroid20m_D = 36,
        StaticAsteroid30m_D = 37,
        StaticAsteroid50m_D = 38,
        StaticAsteroid100m_D = 39,
        StaticAsteroid300m_D = 40,
        StaticAsteroid500m_D = 41,
        StaticAsteroid1000m_D = 42,
        StaticAsteroid2000m_D = 43,
        StaticAsteroid5000m_D = 44,
        StaticAsteroid10000m_D = 45,

        //Removed support 
        StaticAsteroid10m_E = 46,
        StaticAsteroid20m_E = 47,
        StaticAsteroid30m_E = 48,
        StaticAsteroid50m_E = 49,
        StaticAsteroid100m_E = 50,
        StaticAsteroid300m_E = 51,
        StaticAsteroid500m_E = 52,
        StaticAsteroid1000m_E = 53,
        StaticAsteroid2000m_E = 54,
        StaticAsteroid5000m_E = 55,
        StaticAsteroid10000m_E = 56,

        //We support asteroids below
        StaticAsteroid40000m_A = 57,
    }

    [Flags]
    public enum MyStaticAsteroidTypeSetEnum
    {
        A = 0x1,
        B = 0x2,

        All = A | B,
    }

    public class MyMwcObjectBuilder_StaticAsteroid : MyMwcObjectBuilder_Object3dBase
    {
        public MyMwcObjectBuilder_StaticAsteroid_TypesEnum AsteroidType;
        public MyMwcVoxelMaterialsEnum? AsteroidMaterial;
        public MyMwcVoxelMaterialsEnum? AsteroidMaterial1;
        public bool UseModelTechnique;
        //These sizes are diameters
        public static readonly List<int> AsteroidSizes = new List<int>()
        {
            10,
            20,
            30,
            50,
            100,
            300,
            500,
            1000,
            2000,
            5000,
            10000,
        };
        public MinerWarsMath.Vector3? FieldDir;
            
        static MyMwcObjectBuilder_StaticAsteroid()
        {
        }

        internal MyMwcObjectBuilder_StaticAsteroid()
            : base()
        {
            UseModelTechnique = false;
        }

        public MyMwcObjectBuilder_StaticAsteroid(MyMwcObjectBuilder_StaticAsteroid_TypesEnum asteroidType, MyMwcVoxelMaterialsEnum? material, MyMwcVoxelMaterialsEnum? material1 = null)
        {
            AsteroidType = asteroidType;
            AsteroidMaterial = material;
            AsteroidMaterial1 = material1;
        }

        public static void GetAsteroids(int sizeInMeters, MyStaticAsteroidTypeSetEnum material, List<MyMwcObjectBuilder_StaticAsteroid_TypesEnum> toList)
        {
            switch(sizeInMeters)
            {
                case 10:
                    switch (material)
                    {
                        case MyStaticAsteroidTypeSetEnum.B:
                            toList.Add(MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid10m_B);
                            break;
                        default:
                            toList.Add(MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid10m_A);
                            break;
                    }
/*
                    toList.Add(MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid10m_C);
                    toList.Add(MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid10m_D);
                    toList.Add(MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid10m_E);
 * */
                    break;

                case 20:
                    switch (material)
                    {
                        case MyStaticAsteroidTypeSetEnum.B:
                            toList.Add(MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid20m_B);
                            break;
                        default:
                            toList.Add(MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid20m_A);
                            break;
                    }
                    /*
                                        toList.Add(MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid20m_C);
                                        toList.Add(MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid20m_D);
                                        toList.Add(MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid20m_E);
                     */ 
                    break;

                case 30:
                    switch (material)
                    {
                        case MyStaticAsteroidTypeSetEnum.B:
                            toList.Add(MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid30m_B);
                            break;
                        default:
                            toList.Add(MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid30m_A);
                            break;
                    }

/*
                    toList.Add(MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid30m_B);
                    toList.Add(MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid30m_C);
                    toList.Add(MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid30m_D);
                    toList.Add(MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid30m_E);
 **/
                    break;

                case 50:
                    switch (material)
                    {
                        case MyStaticAsteroidTypeSetEnum.B:
                            toList.Add(MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid50m_B);
                            break;
                        default:
                            toList.Add(MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid50m_A);
                            break;
                    }

/*
                    toList.Add(MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid50m_B);
                    toList.Add(MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid50m_C);
                    toList.Add(MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid50m_D);
                    toList.Add(MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid50m_E);
 **/
                    break;

                case 100:
                    switch (material)
                    {
                        case MyStaticAsteroidTypeSetEnum.B:
                            toList.Add(MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid100m_B);
                            break;
                        default:
                            toList.Add(MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid100m_A);
                            break;
                    }

/*
                    toList.Add(MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid100m_B);
                    toList.Add(MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid100m_C);
                    toList.Add(MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid100m_D);
                    toList.Add(MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid100m_E);
 */ 
                    break;

                case 300:
                    switch (material)
                    {
                        case MyStaticAsteroidTypeSetEnum.B:
                            toList.Add(MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid300m_B);
                            break;
                        default:
                            toList.Add(MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid300m_A);
                            break;
                    }
/*
                    toList.Add(MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid300m_B);
                    toList.Add(MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid300m_C);
                    toList.Add(MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid300m_D);
                    toList.Add(MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid300m_E);
 */ 
                    break;

                case 500:
                    switch (material)
                    {
                        case MyStaticAsteroidTypeSetEnum.B:
                            toList.Add(MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid500m_B);
                            break;
                        default:
                            toList.Add(MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid500m_A);
                            break;
                    }
/*
                    toList.Add(MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid500m_B);
                    toList.Add(MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid500m_C);
                    toList.Add(MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid500m_D);
                    toList.Add(MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid500m_E);
 */ 
                    break;

                case 1000:
                    // Commented unoptimized asteroids (high poly count)
                    switch (material)
                    {
                        case MyStaticAsteroidTypeSetEnum.B:
                            toList.Add(MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid1000m_B);
                            break;
                        default:
                            toList.Add(MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid1000m_A);
                            break;
                    }
                    //toList.Add(MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid1000m_B);
                    //toList.Add(MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid1000m_C);
                    //toList.Add(MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid1000m_D);
                    //toList.Add(MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid1000m_E);
                    break;

                case 2000:
                    // Commented unoptimized asteroids (high poly count)
                    switch (material)
                    {
                        case MyStaticAsteroidTypeSetEnum.B:
                            toList.Add(MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid2000m_B);
                            break;
                        default:                                                                  
                            toList.Add(MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid2000m_A);
                            break;
                    }
                    //toList.Add(MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid2000m_B);
                    //toList.Add(MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid2000m_C);
                    //toList.Add(MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid2000m_D);
                    //toList.Add(MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid2000m_E);
                    break;

                case 5000:
                    switch (material)
                    {
                        case MyStaticAsteroidTypeSetEnum.B:
                            toList.Add(MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid5000m_B);
                            break;
                        default:
                            toList.Add(MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid5000m_A);
                            break;
                    }
/*
                    toList.Add(MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid5000m_B);
                    toList.Add(MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid5000m_C);
                    toList.Add(MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid5000m_D);
                    toList.Add(MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid5000m_E);
 */ 
                    break;

                case 10000:
                    switch (material)
                    {
                        case MyStaticAsteroidTypeSetEnum.B:
                            toList.Add(MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid10000m_B);
                            break;
                        default:
                            toList.Add(MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid10000m_A);
                            break;
                    }
/*
                    toList.Add(MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid10000m_B);
                    toList.Add(MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid10000m_C);
                    toList.Add(MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid10000m_D);
                    toList.Add(MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid10000m_E);
 */ 
                    break;

                default:
                    System.Diagnostics.Debug.Assert(true, "Asteroid of this size does not exists");
                    break;
            }
        }

        public override int? GetObjectBuilderId()
        {
            return (int)AsteroidType;
        }

        internal override void SetObjectBuilderIdInternal(int? objectBuilderId)
        {
            AsteroidType = (MyMwcObjectBuilder_StaticAsteroid_TypesEnum)Convert.ToUInt16(objectBuilderId);
        }

        public override MyMwcObjectBuilderTypeEnum GetObjectBuilderType()
        {
            return MyMwcObjectBuilderTypeEnum.StaticAsteroid;
        }

        //  Write this object into message-out
        internal override void Write(BinaryWriter binaryWriter)
        {
            base.Write(binaryWriter);

            // Asteroid Type
            MyMwcLog.IfNetVerbose_AddToLog("AsteroidType: " + AsteroidType);
            MyMwcMessageOut.WriteObjectBuilderStaticAsteroidTypesEnum(AsteroidType, binaryWriter);

            // Asteroid Material
            MyMwcLog.IfNetVerbose_AddToLog("MyMwcObjectBuilder_StaticAsteroid.AsteroidMaterial.HasValue: " + this.AsteroidMaterial.HasValue);
            MyMwcMessageOut.WriteBool(this.AsteroidMaterial.HasValue, binaryWriter);
            if (this.AsteroidMaterial.HasValue)
            {
                MyMwcLog.IfNetVerbose_AddToLog("MyMwcObjectBuilder_StaticAsteroid.AsteroidMaterial.Value: " + this.AsteroidMaterial.Value);
                MyMwcMessageOut.WriteByte((byte)this.AsteroidMaterial.Value, binaryWriter);
            }
        }

        //  Read this object from message-in
        internal override bool Read(BinaryReader binaryReader, EndPoint senderEndPoint, int gameVersion)
        {
            if (base.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();

            // Asteroid Type
            MyMwcObjectBuilder_StaticAsteroid_TypesEnum? asteroidType = MyMwcMessageIn.ReadObjectBuilderStaticAsteroidTypesEnumEx(binaryReader, senderEndPoint);
            if (asteroidType == null) return NetworkError();
            AsteroidType = asteroidType.Value;
            MyMwcLog.IfNetVerbose_AddToLog("AsteroidType: " + AsteroidType);

            // Asteroid Material
            bool? hasId = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
            if (!hasId.HasValue) return NetworkError(); // Cannot read bool - whether owner asteroid material is null or not
            MyMwcLog.IfNetVerbose_AddToLog("MyMwcObjectBuilder_StaticAsteroid.AsteroidMaterial.HasValue: " + hasId.Value);

            // Testing whether Asteroid Material is null
            if (hasId.Value)
            {
                // asteroid material has value - read the value
                byte? asteroidMaterial = MyMwcMessageIn.ReadByteEx(binaryReader, senderEndPoint);
                if (!asteroidMaterial.HasValue) return NetworkError(); // Cannot read asteroid material

                MyMwcLog.IfNetVerbose_AddToLog("MyMwcObjectBuilder_StaticAsteroid.AsteroidMaterial.Value: " + asteroidMaterial.Value);
                this.AsteroidMaterial = (MyMwcVoxelMaterialsEnum)asteroidMaterial.Value;
            }
            else
            {
                this.AsteroidMaterial = null;
            }

            return true;
        }
    }
}
