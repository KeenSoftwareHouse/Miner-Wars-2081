using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using KeenSoftwareHouse.Library.Extensions;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.TransparentGeometry;
using MinerWars.CommonLIB.AppCode.Utils;
using SysUtils.Utils;

//using MinerWarsMath;
//using MinerWarsMath.Graphics;

using SharpDX;
using SharpDX.Direct3D9;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

namespace MinerWars.AppCode.Game.Utils
{
    using Vector2 = MinerWarsMath.Vector2;
    using Vector3 = MinerWarsMath.Vector3;
    using Vector4 = MinerWarsMath.Vector4;
    using Rectangle = MinerWarsMath.Rectangle;
    using Matrix = MinerWarsMath.Matrix;
    using Color = MinerWarsMath.Color;
    using BoundingBox = MinerWarsMath.BoundingBox;
    using BoundingSphere = MinerWarsMath.BoundingSphere;
    using BoundingFrustum = MinerWarsMath.BoundingFrustum;
    using MathHelper = MinerWarsMath.MathHelper;

    static class MyUtils
    {
        [DllImport("kernel32")]
        public static extern bool SetProcessWorkingSetSize(IntPtr handle, int minSize, int maxSize);

        public static readonly Matrix ZeroMatrix = new Matrix(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);

        //  Clamps velocity (vector3) so its length is never longer than maxSpeed
        public static void GetClampVelocity(ref Vector3 velocity, float maxSpeed)
        {
            if (velocity.Length() > maxSpeed)
            {
                velocity = MyMwcUtils.Normalize(velocity);
                velocity.X *= maxSpeed;
                velocity.Y *= maxSpeed;
                velocity.Z *= maxSpeed;
            }
        }

        //moved from VF_Packer Class (originally in MyVertexFormats.cs, now in MyVertexFormatPacker.cs)
        static public Vector3 RepackVoxelPosition(ref Vector3 position)
        {
            MyShort4 voxelPos;
            voxelPos.X = (short)(MyVertexCompression.VOXEL_MULTIPLIER * position.X - MyVertexCompression.VOXEL_OFFSET + MyVertexCompression.VOXEL_COORD_EPSILON);
            voxelPos.Y = (short)(MyVertexCompression.VOXEL_MULTIPLIER * position.Y - MyVertexCompression.VOXEL_OFFSET + MyVertexCompression.VOXEL_COORD_EPSILON);
            voxelPos.Z = (short)(MyVertexCompression.VOXEL_MULTIPLIER * position.Z - MyVertexCompression.VOXEL_OFFSET + MyVertexCompression.VOXEL_COORD_EPSILON);

            return MyVertexCompression.INV_VOXEL_MULTIPLIER * new Vector3(voxelPos.X + MyVertexCompression.VOXEL_OFFSET,
                                                                            voxelPos.Y + MyVertexCompression.VOXEL_OFFSET,
                                                                            voxelPos.Z + MyVertexCompression.VOXEL_OFFSET);
        }

        //  Clamps pitch to interval <0..2>. This are allowed values. Zero means no sound, one means default pitch, two means maximum pitch.
        public static float GetClampVolume(float volume)
        {
            return MathHelper.Clamp(volume, 0, 2);
        }

        //  Clamps pitch to interval <-1..+1>. This are allowed values. Zero means default pitch, negative is low and positive is high sound.
        public static float GetClampPitch(float pitch)
        {
            return MathHelper.Clamp(pitch, -1, +1);
        }

        public static string GetFormatedVector2(Vector2 vec, int decimalDigits)
        {
            return
                "{X: " + MyValueFormatter.GetFormatedDouble(vec.X, decimalDigits) +
                " Y: " + MyValueFormatter.GetFormatedDouble(vec.Y, decimalDigits) + "}";
        }

        public static StringBuilder GetFormatedVector3(this StringBuilder sb, string before, Vector3 value, string after = "")
        {
            sb.Clear();
            sb.Append(before);
            sb.Append("{");
            sb.ConcatFormat("{0: #,000} ", value.X);
            sb.ConcatFormat("{0: #,000} ", value.Y);
            sb.ConcatFormat("{0: #,000} ", value.Z);
            sb.Append("}");
            sb.Append(after);
            return sb;
        }

        public static string GetFormatedVector3(Vector3 vec, int decimalDigits)
        {
            return
                "{X: " + MyValueFormatter.GetFormatedDouble(vec.X, decimalDigits) +
                " Y: " + MyValueFormatter.GetFormatedDouble(vec.Y, decimalDigits) +
                " Z: " + MyValueFormatter.GetFormatedDouble(vec.Z, decimalDigits) + "}";
        }


        public static string GetFormatedVector4(Vector4 vec, int decimalDigits)
        {
            return
                "{X: " + MyValueFormatter.GetFormatedDouble(vec.X, decimalDigits) +
                " Y: " + MyValueFormatter.GetFormatedDouble(vec.Y, decimalDigits) +
                " Z: " + MyValueFormatter.GetFormatedDouble(vec.Z, decimalDigits) +
                " W: " + MyValueFormatter.GetFormatedDouble(vec.W, decimalDigits) + "}";
        }

        public static string GetFormatedVector2Int(MyMwcVector2Int vec)
        {
            return
                "{X: " + MyValueFormatter.GetFormatedInt(vec.X) +
                " Y: " + MyValueFormatter.GetFormatedInt(vec.Y) + "}";
        }

        public static string GetFormatedVector3Int(MyMwcVector3Int vec)
        {
            return
                "{X: " + MyValueFormatter.GetFormatedInt(vec.X) +
                " Y: " + MyValueFormatter.GetFormatedInt(vec.Y) +
                " Z: " + MyValueFormatter.GetFormatedInt(vec.Z) + "}";
        }

        public static string GetFormatedBoundingBox(BoundingBox boundingBox, int decimalDigits)
        {
            return "Min: " + MyUtils.GetFormatedVector3(boundingBox.Min, decimalDigits) + ", Max: " + MyUtils.GetFormatedVector3(boundingBox.Max, decimalDigits);
        }

        public static BoundingBox GetNewBoundingBox(Vector3 position, Vector3 sizeInMetres)
        {
            return new BoundingBox(position, position + sizeInMetres);
        }

        public static string GetFormatedMatrix(Matrix matrix, int decimalDigits)
        {
            return
                "{Translation.X: " + MyValueFormatter.GetFormatedDouble(matrix.Translation.X, decimalDigits) +
                " Translation.Y: " + MyValueFormatter.GetFormatedDouble(matrix.Translation.Y, decimalDigits) +
                " Translation.Z: " + MyValueFormatter.GetFormatedDouble(matrix.Translation.Z, decimalDigits) +
                " Forward.X: " + MyValueFormatter.GetFormatedDouble(matrix.Forward.X, decimalDigits) +
                " Forward.Y: " + MyValueFormatter.GetFormatedDouble(matrix.Forward.Y, decimalDigits) +
                " Forward.Z: " + MyValueFormatter.GetFormatedDouble(matrix.Forward.Z, decimalDigits) + "}";
        }

        // Get the yaw,pitch and roll from a rotation matrix.
        public static void RotationMatrixToYawPitchRoll(ref Matrix mx, out float yaw, out float pitch, out float roll)
        {
            float clamped = mx.M32;
            if (clamped > 1) clamped = 1;
            else if (clamped < -1) clamped = -1;
            pitch = (float)Math.Asin(-clamped);
            float threshold = 0.001f;
            float test = (float)Math.Cos(pitch);
            if (test > threshold)
            {
                roll = (float)Math.Atan2(mx.M12, mx.M22);
                yaw = (float)Math.Atan2(mx.M31, mx.M33);
            }
            else
            {
                roll = (float)Math.Atan2(-mx.M21, mx.M11);
                yaw = 0.0f;
            }
        }

        public static void AssertTexture(MyTexture2D texture)
        {
            AssertTextureDxtCompress(texture);
            AssertTextureMipMapped(texture);
        }

        static void AssertTextureDxtCompress(MyTexture2D texture)
        {
            MyCommonDebugUtils.AssertRelease(IsTextureDxtCompressed(texture));
        }

        static void AssertTextureMipMapped(MyTexture2D texture)
        {
            MyCommonDebugUtils.AssertRelease(IsTextureMipMapped(texture));
        }

        public static bool IsTextureDxtCompressed(MyTexture2D texture)
        {
            return texture.Format == Format.Dxt1 || texture.Format == Format.Dxt3 || texture.Format == Format.Dxt5;
        }

        public static bool IsTextureMipMapped(MyTexture2D texture)
        {
            return texture.Width == 4 || texture.Height == 4 || texture.LevelCount > 1;
        }

        //  IMPORTANT: Don't use Debug.Assert() that has two arguments (condition and message). Use only one that has condition.
        //  IMPORTANT: It's because obfuscator encodes all string and if we run this assert offten, we always need to decode that string even if condition is OK.
        //  IMPORTANT: Write assert details as comments where you call the assert. It's safer and faster.
        //  Checks if Vector3 is correct (not NaN, etc)
        //public static void AssertVector3(Vector3 vec)
        //{
        //    //  Check if 'One component of a vector is NaN'
        //    System.Diagnostics.Debug.Assert((!float.IsNaN(vec.X)) && (!float.IsNaN(vec.Y)) && (!float.IsNaN(vec.Z)));
        //}

        //  Save 'Vector3' using binary writer
        public static void BinaryWrite(BinaryWriter bw, ref Vector3 vector)
        {
            bw.Write(vector.X);
            bw.Write(vector.Y);
            bw.Write(vector.Z);
        }

        //  Save 'Matrix' using binary writer
        public static void BinaryWrite(BinaryWriter bw, ref Matrix matrix)
        {
            bw.Write(matrix.M11);
            bw.Write(matrix.M12);
            bw.Write(matrix.M13);
            bw.Write(matrix.M14);
            bw.Write(matrix.M21);
            bw.Write(matrix.M22);
            bw.Write(matrix.M23);
            bw.Write(matrix.M24);
            bw.Write(matrix.M31);
            bw.Write(matrix.M32);
            bw.Write(matrix.M33);
            bw.Write(matrix.M34);
            bw.Write(matrix.M41);
            bw.Write(matrix.M42);
            bw.Write(matrix.M43);
            bw.Write(matrix.M44);
        }

        //  Read 'Vector3' using binary reader
        public static void BinaryRead(BinaryReader br, ref Vector3 vector)
        {
            vector.X = br.ReadSingle();
            vector.Y = br.ReadSingle();
            vector.Z = br.ReadSingle();
        }

        //  Read 'Matrix' using binary reader
        public static void BinaryRead(BinaryReader br, ref Matrix matrix)
        {
            matrix.M11 = br.ReadSingle();
            matrix.M12 = br.ReadSingle();
            matrix.M13 = br.ReadSingle();
            matrix.M14 = br.ReadSingle();
            matrix.M21 = br.ReadSingle();
            matrix.M22 = br.ReadSingle();
            matrix.M23 = br.ReadSingle();
            matrix.M24 = br.ReadSingle();
            matrix.M31 = br.ReadSingle();
            matrix.M32 = br.ReadSingle();
            matrix.M33 = br.ReadSingle();
            matrix.M34 = br.ReadSingle();
            matrix.M41 = br.ReadSingle();
            matrix.M42 = br.ReadSingle();
            matrix.M43 = br.ReadSingle();
            matrix.M44 = br.ReadSingle();
        }

        private static MyTextureAtlas LoadTextureAtlas(string textureDir, string atlasFile)
        {
            MyTextureAtlas atlas = new MyTextureAtlas(64);
            using (StreamReader sr = new StreamReader(atlasFile))
            {
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();

                    if (line.StartsWith("#"))
                        continue;
                    if (line.Trim(' ').Length == 0)
                        continue;

                    string[] parts = line.Split(new char[] {' ', '\t', ','}, StringSplitOptions.RemoveEmptyEntries );

                    string name = parts[0];

                    string atlasName = parts[1].Replace(".dds","");

                    Vector4 uv = new Vector4(
                        Convert.ToSingle(parts[4], System.Globalization.CultureInfo.InvariantCulture),
                        Convert.ToSingle(parts[5], System.Globalization.CultureInfo.InvariantCulture),
                        Convert.ToSingle(parts[7], System.Globalization.CultureInfo.InvariantCulture),
                        Convert.ToSingle(parts[8], System.Globalization.CultureInfo.InvariantCulture));

                    MyTexture2D atlasTexture = MyTextureManager.GetTexture<MyTexture2D>(textureDir + atlasName);
                    MyTextureAtlasItem item = new MyTextureAtlasItem(atlasTexture, uv);
                    atlas.Add(name, item);
                }
            }

            return atlas;
        }


        public static void LoadTextureAtlas(string[] enumsToStrings, string textureDir, string atlasFile, out MyTexture2D texture, out MyAtlasTextureCoordinate[] textureCoords)
        {
            //MyTextureAtlas atlas = contentManager.Load<MyTextureAtlas>(atlasFile);

            MyTextureAtlas atlas = LoadTextureAtlas(textureDir, atlasFile);

            //  Here we define particle texture coordinates inside of texture atlas
            textureCoords = new MyAtlasTextureCoordinate[enumsToStrings.Length];

            texture = null;

            for (int i = 0; i < enumsToStrings.Length; i++)
            {
                MyTextureAtlasItem textureAtlasItem = atlas[enumsToStrings[i]];

                textureCoords[i] = new MyAtlasTextureCoordinate(new Vector2(textureAtlasItem.UVOffsets.X, textureAtlasItem.UVOffsets.Y), new Vector2(textureAtlasItem.UVOffsets.Z, textureAtlasItem.UVOffsets.W));

                //  Texture atlas content processor support having more DDS files for one atlas, but we don't want it (because we want to have all particles in one texture, so we can draw fast).
                //  So here we just take first and only texture.
                if (texture == null)
                {
                    texture = textureAtlasItem.AtlasTexture;
                }
            }
        }

        //  Used by Bresenham's rasterization
        public static int GetBresenhamSgn(int a)
        {
            return a < 0 ? -1 : 1;
        }

        //  Scales 'originalVector' to length 'newLength'
        //  IMPORTANT: This method should be used only on not-normalized vectores, because if vector is normalized, multiplication by new length is faster!!!
        public static Vector3 GetVector3Scaled(Vector3 originalVector, float newLength)
        {
            if (newLength == 0.0f)
            {
                //	V pripade ze chceme aby mal vektor nulovu dlzku, tak komponenty vektora zmenime
                //	na nulu rucne, lebo delenie nulou by sposobilo vznik neplatnych floatov a problemy!
                return Vector3.Zero;
            }
            else
            {
                float originalLength = originalVector.Length();

                //	Ak je dlzka povodneho vektora nulova, nema zmysel scalovat jeho dlzku
                if (originalLength == 0.0f)
                {
                    return Vector3.Zero;
                }

                //  Return scaled vector
                float mul = newLength / originalLength;
                return new Vector3(originalVector.X * mul, originalVector.Y * mul, originalVector.Z * mul);
            }
        }

        //  Converts spherical coordinates (horizontal and vertical angle) to cartesian coordinates (relative to sphere centre).
        //  Use radius to specify sphere's radius (set to 1 if unit sphere).
        //  Angles are in radians.
        //  Input spherical coordinate system: horisontal is angle on XZ plane starting at -Z direction, vertical is angle on YZ plan, starting at -Z direction.
        //  Output cartesian coordinate system: forward is -Z, up is +Y, right is +X
        //  Formulas for conversion from/to spherical/cartezian are from: http://en.wikipedia.org/wiki/Spherical_coordinates
        //  IMPORTANT: I don't know why I did "angleVertical = MathHelper.PiOver2 - angleVertical;", it isn't in wiki's formulas. Probably it has something
        //  to di with background-cube billboards I use this method for.
        //  IMPORTANT: This isn't correct version, because orientation of horizontal/vertical angles seems to be wrong. Correct is GetCartesianCoordinatesFromSpherical()
        public static Vector3 GetCartesianCoordinatesFromSpherical_Weird(float angleHorizontal, float angleVertical, float radius)
        {
            angleVertical = MathHelper.PiOver2 - angleVertical;
            return new Vector3(
                (float)(radius * Math.Sin(angleVertical) * Math.Sin(angleHorizontal)),
                (float)(radius * Math.Cos(angleVertical)),
                (float)(radius * Math.Sin(angleVertical) * Math.Cos(angleHorizontal)));
        }

        //  Converts spherical coordinates (horizontal and vertical angle) to cartesian coordinates (relative to sphere centre).
        //  Use radius to specify sphere's radius (set to 1 if unit sphere).
        //  Angles are in radians.
        //  Input spherical coordinate system: horisontal is angle on XZ plane starting at -Z direction, vertical is angle on YZ plan, starting at -Z direction.
        //  Output cartesian coordinate system: forward is -Z, up is +Y, right is +X
        //  Formulas for conversion from/to spherical/cartezian are from: http://en.wikipedia.org/wiki/Spherical_coordinates
        //  IMPORTANT: This should be RIGHT version of this method, instead of GetCartesianCoordinatesFromSpherical_Weird()
        public static Vector3 GetCartesianCoordinatesFromSpherical(float angleHorizontal, float angleVertical, float radius)
        {
            angleVertical = MathHelper.PiOver2 - angleVertical;
            angleHorizontal = MathHelper.Pi - angleHorizontal;

            return new Vector3(
                (float)(radius * Math.Sin(angleVertical) * Math.Sin(angleHorizontal)),
                (float)(radius * Math.Cos(angleVertical)),
                (float)(radius * Math.Sin(angleVertical) * Math.Cos(angleHorizontal)));
        }

        //  Get spherical coordinates (angles) from normalized vector coordinates
        //  This angles will be in right orientation, that is horizontal will begin at zero from FRONT direction in right side and same for vertical.
        //  Use it together with GetCartesianCoordinatesFromSpherical()
        public static Vector2 GetSphericalCoordinatesFromCartesian(ref Vector3 vector)
        {
            return new Vector2(
                -(float)Math.Atan2(-vector.X, -vector.Z),
                (float)Math.Atan2(vector.Y, Math.Sqrt(vector.Z * vector.Z + vector.X * vector.X)));
        }

        //  Calculating the Angle between two Vectors (return in radians)
        //  Input vectors need to be normalized before calling this method.
        public static float GetAngleBetweenVectors(Vector3 vectorA, Vector3 vectorB)
        {
            //  Calculate the cosine of the angle using the dot product
            //  Use normalised vectors to simplify the formula
            float cosAngle = Vector3.Dot(vectorA, vectorB);

            //  Result from Vector3.Dot are sometime not accurate due to rounding errors, so sometime 0 degree angle is
            //  calculated not as 1.0 but 1.00000001. We need to fix this, because ACOS accepts only number in range <-1, +1>
            if ((cosAngle > 1.0f) && (cosAngle <= 1.0001f))
            {
                cosAngle = 1.0f;
            }
            if ((cosAngle < -1.0f) && (cosAngle >= -1.0001f))
            {
                cosAngle = -1.0f;
            }

            //  Calculate the angle in radians
            return (float)Math.Acos(cosAngle);
        }

        public static float GetAngleBetweenVectorsAndNormalise(Vector3 vectorA, Vector3 vectorB)
        {
            return GetAngleBetweenVectors(Vector3.Normalize(vectorA), Vector3.Normalize(vectorB));
        }

        // rotuje souradnicouvou soustavu 2 vektoru a updatuje jejich pozici, vektory musi byt na sebe kolme
        public static void VectorPlaneRotation(Vector3 xVector, Vector3 yVector, out Vector3 xOut, out Vector3 yOut, float angle)
        {
            Vector3 newX = xVector * (float)Math.Cos(angle) + yVector * (float)Math.Sin(angle);
            Vector3 newY = xVector * (float)Math.Cos(angle + Math.PI / 2.0f) + yVector * (float)Math.Sin(angle + Math.PI / 2.0f);
            xOut = newX;
            yOut = newY;
        }

        //  Return quad whos face is always looking to the camera. 
        //  IMPORTANT: This bilboard looks same as point vertexes (point sprites) - horizontal and vertical axes of billboard are always parallel to screen
        //  That means, if billboard is in the left-up corner of screen, it won't be distorted by perspective. If will look as 2D quad on screen. As I said, it's same as GPU points.
        public static void GetBillboardQuadRotated(MyBillboard billboard, ref Vector3 position, float radius, float angle)
        {
            float angleCos = radius * (float)Math.Cos(angle);
            float angleSin = radius * (float)Math.Sin(angle);

            //	Two main vectors of a billboard rotated around the view axis/vector
            Vector3 billboardAxisX = new Vector3();
            billboardAxisX.X = angleCos * MyCamera.LeftVector.X + angleSin * MyCamera.UpVector.X;
            billboardAxisX.Y = angleCos * MyCamera.LeftVector.Y + angleSin * MyCamera.UpVector.Y;
            billboardAxisX.Z = angleCos * MyCamera.LeftVector.Z + angleSin * MyCamera.UpVector.Z;

            Vector3 billboardAxisY = new Vector3();
            billboardAxisY.X = -angleSin * MyCamera.LeftVector.X + angleCos * MyCamera.UpVector.X;
            billboardAxisY.Y = -angleSin * MyCamera.LeftVector.Y + angleCos * MyCamera.UpVector.Y;
            billboardAxisY.Z = -angleSin * MyCamera.LeftVector.Z + angleCos * MyCamera.UpVector.Z;

            //	Coordinates of four points of a billboard's quad
            billboard.Position0.X = position.X + billboardAxisX.X + billboardAxisY.X;
            billboard.Position0.Y = position.Y + billboardAxisX.Y + billboardAxisY.Y;
            billboard.Position0.Z = position.Z + billboardAxisX.Z + billboardAxisY.Z;

            billboard.Position1.X = position.X - billboardAxisX.X + billboardAxisY.X;
            billboard.Position1.Y = position.Y - billboardAxisX.Y + billboardAxisY.Y;
            billboard.Position1.Z = position.Z - billboardAxisX.Z + billboardAxisY.Z;

            billboard.Position2.X = position.X - billboardAxisX.X - billboardAxisY.X;
            billboard.Position2.Y = position.Y - billboardAxisX.Y - billboardAxisY.Y;
            billboard.Position2.Z = position.Z - billboardAxisX.Z - billboardAxisY.Z;

            billboard.Position3.X = position.X + billboardAxisX.X - billboardAxisY.X;
            billboard.Position3.Y = position.Y + billboardAxisX.Y - billboardAxisY.Y;
            billboard.Position3.Z = position.Z + billboardAxisX.Z - billboardAxisY.Z;
        }

        //  Return quad whos face is always looking to the camera. 
        //  IMPORTANT: This bilboard looks same as point vertexes (point sprites) - horizontal and vertical axes of billboard are always parallel to screen
        //  That means, if billboard is in the left-up corner of screen, it won't be distorted by perspective. If will look as 2D quad on screen. As I said, it's same as GPU points.
        public static void GetBillboardQuadRotated(out MyQuad quad, Vector3 position, float radius, float angle,
            Vector3 cameraLeftVector, Vector3 cameraUpVector)
        {
            float angleCos = (float)Math.Cos(angle);
            float angleSin = (float)Math.Sin(angle);

            //	Two main vectors of a billboard rotated around the view axis/vector
            Vector3 billboardAxisX = (radius * angleCos) * cameraLeftVector + (radius * angleSin) * cameraUpVector;
            Vector3 billboardAxisY = (-radius * angleSin) * cameraLeftVector + (radius * angleCos) * cameraUpVector;

            //	Coordinates of four points of a billboard's quad
            quad.Point0 = position + billboardAxisX + billboardAxisY;
            quad.Point1 = position - billboardAxisX + billboardAxisY;
            quad.Point2 = position - billboardAxisX - billboardAxisY;
            quad.Point3 = position + billboardAxisX - billboardAxisY;
        }

        //  Same as GetBillboardQuadRotated(), but without rotation.
        public static void GetBillboardQuad(out MyQuad quad, Vector3 position, float radius, Vector3 cameraLeftVector, Vector3 cameraUpVector)
        {
            Vector3 billboardAxisX = cameraLeftVector * radius;
            Vector3 billboardAxisY = cameraUpVector * radius;

            //	Coordinates of four points of a billboard's quad
            quad.Point0 = position + billboardAxisX + billboardAxisY;
            quad.Point1 = position - billboardAxisX + billboardAxisY;
            quad.Point2 = position - billboardAxisX - billboardAxisY;
            quad.Point3 = position + billboardAxisX - billboardAxisY;
        }

        //  This billboard isn't facing the camera. It's always oriented in specified direction. May be used as thrusts, or inner light of reflector.
        public static void GetBillboardQuadOriented(out MyQuad quad, ref Vector3 position, float radius, ref Vector3 leftVector, ref Vector3 upVector)
        {
            Vector3 billboardAxisX = leftVector * radius;
            Vector3 billboardAxisY = upVector * radius;

            //	Coordinates of four points of a billboard's quad
            quad.Point0 = position + billboardAxisX + billboardAxisY;
            quad.Point1 = position + billboardAxisX - billboardAxisY;
            quad.Point2 = position - billboardAxisX - billboardAxisY;
            quad.Point3 = position - billboardAxisX + billboardAxisY;
        }

        //  This billboard isn't facing the camera. It's always oriented in specified direction. May be used as thrusts, or inner light of reflector.
        public static void GetBillboardQuadOriented(out MyQuad quad, ref Vector3 position, float width, float height, ref Vector3 leftVector, ref Vector3 upVector)
        {
            Vector3 billboardAxisX = leftVector * width;
            Vector3 billboardAxisY = upVector * height;

            //	Coordinates of four points of a billboard's quad
            quad.Point0 = position + billboardAxisX + billboardAxisY;
            quad.Point1 = position + billboardAxisX - billboardAxisY;
            quad.Point2 = position - billboardAxisX - billboardAxisY;
            quad.Point3 = position - billboardAxisX + billboardAxisY;
        }


        /// <summary>
        /// Generate oriented quad by matrix
        /// </summary>
        /// <param name="quad"></param>
        /// <param name="position"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="matrix"></param>
        public static void GenerateQuad(out MyQuad quad, ref Vector3 position, float width, float height, ref Matrix matrix)
        {
            Vector3 billboardAxisX = matrix.Left * width;
            Vector3 billboardAxisY = matrix.Up * height;

            //	Coordinates of four points of a billboard's quad
            quad.Point0 = position + billboardAxisX + billboardAxisY;
            quad.Point1 = position + billboardAxisX - billboardAxisY;
            quad.Point2 = position - billboardAxisX - billboardAxisY;
            quad.Point3 = position - billboardAxisX + billboardAxisY;
        }

        //  Return quad whos face is always looking to the camera. This is the real billboard, with perspective distortions!
        //  Billboard's orientation is calculated by projecting unit sphere's north pole to billboard's plane. From this we can get up/right vectors.
        //  Idea is this: if camera is in the middle of unit sphere and billboard is touching this sphere at some place (still unit sphere), we
        //  know billboard's plan is perpendicular to the sphere, so if we want billboard's orientation to be like earth's latitude and Colatitude,
        //  we need to find billboard's up vector in the direction. This is when projection cames into place.
        //  Notice that we don't need camera left/up vector. We only need camera position. Because it's about the sphere around the player. Not camera orientation.
        //  IMPORTANT: One problem of this approach is that if billboard is right above the player, its orientation will swing. Thats because we are projecting
        //  Rotation is around vector pointing from camera position to center of the billboard.
        //  the point, but it ends right in the billboard's centre.
        //  So we not use this for particles. We use it only for background sphere (starts, galaxies) prerender.
        //  Return false if billboard wasn't for any reason created (e.g. too close to the camera)
        public static bool GetBillboardQuadAdvancedRotated(out MyQuad quad, Vector3 position, float radiusX, float radiusY, float angle, Vector3 cameraPosition)
        {
            //  Optimized: Vector3 dirVector = MyMwcUtils.Normalize(position - cameraPosition);
            Vector3 dirVector;
            dirVector.X = position.X - cameraPosition.X;
            dirVector.Y = position.Y - cameraPosition.Y;
            dirVector.Z = position.Z - cameraPosition.Z;

            // If distance to camera is really small don't draw it.
            if (dirVector.LengthSquared() <= MyMwcMathConstants.EPSILON)
            {
                //  Some empty quad
                quad = new MyQuad();
                return false;
            }

            dirVector = MyMwcUtils.Normalize(dirVector);

            Vector3 projectedPoint;
            ProjectPointOnPlane(ref MyConstants.VECTOR3_UP, ref dirVector, out projectedPoint);

            Vector3 upVector;
            if (projectedPoint.LengthSquared() <= MyMwcMathConstants.EPSILON_SQUARED)
            {
                //  If projected point equals to zero, we know billboard is exactly above or bottom of camera 
                //  and we can't calculate proper orientation. So we just select some direction. Good thing we
                //  know is that billboard's plan ix XY, so we can choose any point on this plane
                upVector = Vector3.Forward;
            }
            else
            {
                //  Optimized: upVector = MyMwcUtils.Normalize(projectedPoint);
                MyMwcUtils.Normalize(ref projectedPoint, out upVector);
            }

            //  Optimized: Vector3 leftVector = MyMwcUtils.Normalize(Vector3.Cross(upVector, dirVector));
            Vector3 leftVector;
            Vector3.Cross(ref upVector, ref dirVector, out leftVector);
            leftVector = MyMwcUtils.Normalize(leftVector);

            //	Two main vectors of a billboard rotated around the view axis/vector
            float angleCos = MyMath.FastCos(angle);
            float angleSin = MyMath.FastSin(angle);

            Vector3 billboardAxisX;
            billboardAxisX.X = (radiusX * angleCos) * leftVector.X + (radiusY * angleSin) * upVector.X;
            billboardAxisX.Y = (radiusX * angleCos) * leftVector.Y + (radiusY * angleSin) * upVector.Y;
            billboardAxisX.Z = (radiusX * angleCos) * leftVector.Z + (radiusY * angleSin) * upVector.Z;

            Vector3 billboardAxisY;
            billboardAxisY.X = (-radiusX * angleSin) * leftVector.X + (radiusY * angleCos) * upVector.X;
            billboardAxisY.Y = (-radiusX * angleSin) * leftVector.Y + (radiusY * angleCos) * upVector.Y;
            billboardAxisY.Z = (-radiusX * angleSin) * leftVector.Z + (radiusY * angleCos) * upVector.Z;

            //	Coordinates of four points of a billboard's quad
            quad.Point0.X = position.X + billboardAxisX.X + billboardAxisY.X;
            quad.Point0.Y = position.Y + billboardAxisX.Y + billboardAxisY.Y;
            quad.Point0.Z = position.Z + billboardAxisX.Z + billboardAxisY.Z;

            quad.Point1.X = position.X - billboardAxisX.X + billboardAxisY.X;
            quad.Point1.Y = position.Y - billboardAxisX.Y + billboardAxisY.Y;
            quad.Point1.Z = position.Z - billboardAxisX.Z + billboardAxisY.Z;

            quad.Point2.X = position.X - billboardAxisX.X - billboardAxisY.X;
            quad.Point2.Y = position.Y - billboardAxisX.Y - billboardAxisY.Y;
            quad.Point2.Z = position.Z - billboardAxisX.Z - billboardAxisY.Z;

            quad.Point3.X = position.X + billboardAxisX.X - billboardAxisY.X;
            quad.Point3.Y = position.Y + billboardAxisX.Y - billboardAxisY.Y;
            quad.Point3.Z = position.Z + billboardAxisX.Z - billboardAxisY.Z;

            return true;
        }

        public static bool GetBillboardQuadAdvancedRotated(out MyQuad quad, Vector3 position, float radius, float angle, Vector3 cameraPosition)
        {
            return GetBillboardQuadAdvancedRotated(out quad, position, radius, radius, angle, cameraPosition);
        }

        //  Calculates coordinates for quad that lies on line defined by two points and is always facing the camera. It thickness is defined in metres.
        //  It is used for drawing bullet lines, debris flying from explosions, anything that isn't quad but is line.
        //  IMPORTANT: Parameter 'polyLine' is refed only for performance. Don't change it inside the method.
        public static void GetPolyLineQuad(out MyQuad retQuad, ref MyPolyLine polyLine)
        {
            Vector3 toCamera = MyCamera.Position - polyLine.Point0;
            Vector3 cameraToPoint;
            if (!MyMwcUtils.HasValidLength(toCamera))
            {
                // When camera at point, choose random direction
                cameraToPoint = Vector3.Forward;
            }
            else
            {
                cameraToPoint = MyMwcUtils.Normalize(toCamera);
            }
            Vector3 sideVector = GetVector3Scaled(Vector3.Cross(polyLine.LineDirectionNormalized, cameraToPoint), polyLine.Thickness);

            retQuad.Point0 = polyLine.Point0 - sideVector;
            retQuad.Point1 = polyLine.Point1 - sideVector;
            retQuad.Point2 = polyLine.Point1 + sideVector;
            retQuad.Point3 = polyLine.Point0 + sideVector;
        }

        //  Calculates coordinates for quad that lies on line defined by two points and is always facing the camera. It thickness is defined in metres.
        //  It is used for drawing bullet lines, debris flying from explosions, anything that isn't quad but is line.
        //  IMPORTANT: Parameter 'polyLine' is refed only for performance. Don't change it inside the method.
        public static void GetPolyLineQuad(out MyQuad retQuad, ref MyPolyLine polyLine, Vector3 cameraPosition)
        {
            Vector3 cameraToPoint = MyMwcUtils.Normalize(cameraPosition - polyLine.Point0);
            Vector3 sideVector = GetVector3Scaled(Vector3.Cross(polyLine.LineDirectionNormalized, cameraToPoint), polyLine.Thickness);

            retQuad.Point0 = polyLine.Point0 - sideVector;
            retQuad.Point1 = polyLine.Point1 - sideVector;
            retQuad.Point2 = polyLine.Point1 + sideVector;
            retQuad.Point3 = polyLine.Point0 + sideVector;
        }

        //  Simple overload for cases when it doesn't make sense to use 'ref' on one of the input parameters
        public static void ProjectPointOnPlane(
            ref Vector3 p,				//	Bod ktory chceme projektovat na rovinu
            Vector3 normal,			//	Normala roviny
            out Vector3 ret
            )
        {
            ProjectPointOnPlane(ref p, ref normal, out ret);
        }

        //	Projekcia bodu na rovinu. Pozor! Tuto funkciu sme zatial nepouzili, otestovana je iba ciastocne.
        //	Je ukradnuta z Wolf zdrojakov. Trosku zvlastne je, ze na definovanie roviny staci iba jej normala a ziaden
        //	origin, alebo distance k bodu nula. Preto asi treba k vyslednemu bodu "dst" pripocitat origin roviny.
        //	Ale riadne to otestuj, alebo ukradni tuto funkciu od inakadial.
        public static void ProjectPointOnPlane(
            ref Vector3 p,				//	Bod ktory chceme projektovat na rovinu
            ref Vector3 normal,			//	Normala roviny
            out Vector3 ret
            )
        {
            //  Optimized: float inv_denom = 1.0f / Vector3.Dot(normal, normal);
            float invDenom;
            Vector3.Dot(ref normal, ref normal, out invDenom);
            invDenom = 1.0f / invDenom;

            //  Optimized: float d = Vector3.Dot(normal, p) * inv_denom;
            float d;
            Vector3.Dot(ref normal, ref p, out d);
            d = d * invDenom;

            //  Optimized: Vector3 n = normal * inv_denom;
            Vector3 n;
            n.X = normal.X * invDenom;
            n.Y = normal.Y * invDenom;
            n.Z = normal.Z * invDenom;

            //  Optimized: return p - d * n;
            ret.X = p.X - d * n.X;
            ret.Y = p.Y - d * n.Y;
            ret.Z = p.Z - d * n.Z;
        }

        //	This method calculated vector perpendicular to vector "src". Because this problem has infinite numner
        //  of solutions, this method returns only one of them, which lucky is very good.
        //  Returned vector is normalized. I guess source vector must be normalized before passed to this method.
        public static void GetPerpendicularVector(ref Vector3 src, out Vector3 ret)
        {
            float absX = Math.Abs(src.X);
            float absY = Math.Abs(src.Y);
            float absZ = Math.Abs(src.Z);

            const int SMALLEST_COMPONENT_X = 0;
            const int SMALLEST_COMPONENT_Y = 1;
            const int SMALLEST_COMPONENT_Z = 2;

            int smallestComponent = SMALLEST_COMPONENT_X;
            float minelem = absX;

            //  Find the smallest magnitude axially aligned vector
            if (absY < minelem)
            {
                smallestComponent = SMALLEST_COMPONENT_Y;
                minelem = absY;
            }
            if (absZ < minelem)
            {
                smallestComponent = SMALLEST_COMPONENT_Z;
            }

            Vector3 tempvec = Vector3.Zero;
            if (smallestComponent == SMALLEST_COMPONENT_X)
            {
                tempvec.X = 1.0f;
            }
            else if (smallestComponent == SMALLEST_COMPONENT_Y)
            {
                tempvec.Y = 1.0f;
            }
            else if (smallestComponent == SMALLEST_COMPONENT_Z)
            {
                tempvec.Z = 1.0f;
            }

            //  Project the point onto the plane defined by src and normalize the result
            ProjectPointOnPlane(ref tempvec, ref src, out ret);
            ret = MyMwcUtils.Normalize(ret);
        }

        //  This is just a wrapper for Vector3.Transform
        public static Vector3 GetTransform(Vector3 vec, ref Matrix matrix)
        {
            Vector3 ret;
            Vector3.Transform(ref vec, ref matrix, out ret);
            return ret;
        }

        //  This is just a wrapper for Vector3.Transform
        public static Vector3 GetTransform(ref Vector3 vec, ref Matrix matrix)
        {
            Vector3 ret;
            Vector3.Transform(ref vec, ref matrix, out ret);
            return ret;
        }

        //  This is just a wrapper for Vector3.TransformNormal
        public static Vector3 GetTransformNormal(Vector3 vec, ref Matrix matrix)
        {
            Vector3 ret;
            Vector3.TransformNormal(ref vec, ref matrix, out ret);
            return ret;
        }

        //  This is just a wrapper for Vector3.TransformNormal + result is normalized
        public static Vector3 GetTransformNormalNormalized(Vector3 vec, ref Matrix matrix)
        {
            Vector3 ret;
            Vector3.TransformNormal(ref vec, ref matrix, out ret);
            ret = MyMwcUtils.Normalize(ret);
            return ret;
        }

        //	Vrati najkratsiu vzdialenost medzi bodom a rovinou (definovanou normalou a lubovolnym bodom na rovine).
        //	Moze vratit aj zapornu vzdialenost, pokial sa bod nachadza na opacnej strane roviny nez ukazuje normalovy vektor.
        //	Predpokladame ze, normalovy vektor je normalizovany.
        public static float GetDistanceFromPointToPlane(Vector3 point, ref MyPlane plane)
        {
            return
                 plane.Normal.X * (point.X - plane.Point.X) +
                 plane.Normal.Y * (point.Y - plane.Point.Y) +
                 plane.Normal.Z * (point.Z - plane.Point.Z);
        }

        //	Vrati najkratsiu vzdialenost medzi bodom a rovinou (definovanou normalou a lubovolnym bodom na rovine).
        //	Moze vratit aj zapornu vzdialenost, pokial sa bod nachadza na opacnej strane roviny nez ukazuje normalovy vektor.
        //	Predpokladame ze, normalovy vektor je normalizovany.
        public static float GetDistanceFromPointToPlane(ref Vector3 point, ref MyPlane plane)
        {
            return
                 plane.Normal.X * (point.X - plane.Point.X) +
                 plane.Normal.Y * (point.Y - plane.Point.Y) +
                 plane.Normal.Z * (point.Z - plane.Point.Z);
        }

        //  Calculates distance from point 'from' to boundary of 'sphere'. If point is inside the sphere, distance will be negative.
        public static float GetSmallestDistanceToSphere(ref Vector3 from, ref BoundingSphere sphere)
        {
            return Vector3.Distance(from, sphere.Center) - sphere.Radius;
        }

        //  Calculates distance from point 'from' to boundary of 'sphere'. If point is inside the sphere, distance will be zero (this is the diference against GetSmallestDistanceToSphere)
        public static float GetSmallestDistanceToSphereAlwaysPositive(ref Vector3 from, ref BoundingSphere sphere)
        {
            float distance = GetSmallestDistanceToSphere(ref from, ref sphere);
            if (distance < 0) distance = 0;
            return distance;
        }

        //  Distance between "from" and opposite side of the "sphere". Always positive.
        public static float GetLargestDistanceToSphere(ref Vector3 from, ref BoundingSphere sphere)
        {
            return Vector3.Distance(from, sphere.Center) + sphere.Radius;
        }

        //  Calculates distance from boundary of one sphere to boundary of another sphere. If spheres intersects, distance will be negative.
        public static float GetDistanceBetweenSpheres(ref BoundingSphere sphere1, ref BoundingSphere sphere2)
        {
            return (Vector3.Distance(sphere1.Center, sphere2.Center) - (sphere1.Radius + sphere2.Radius));
        }

        //  Calculates distance from boundary of one sphere to boundary of another sphere. If spheres intersects, distance will be zero.
        public static float GetDistanceBetweenSpheresAbsolute(ref BoundingSphere sphere1, ref BoundingSphere sphere2)
        {
            float distance = GetDistanceBetweenSpheres(ref sphere1, ref sphere2);
            if (distance < 0) distance = 0;
            return distance;
        }

        //	This tells if a sphere is BEHIND, in FRONT, or INTERSECTS a plane, also it's distance
        public static MySpherePlaneIntersectionEnum GetSpherePlaneIntersection(ref BoundingSphere sphere, ref MyPlane plane, out float distanceFromPlaneToSphere)
        {
            //  First we need to find the distance our polygon plane is from the origin.
            float planeDistance = plane.GetPlaneDistance();

            //  Here we use the famous distance formula to find the distance the center point
            //  of the sphere is from the polygon's plane.  
            distanceFromPlaneToSphere = (plane.Normal.X * sphere.Center.X + plane.Normal.Y * sphere.Center.Y + plane.Normal.Z * sphere.Center.Z + planeDistance);

            //  If the absolute value of the distance we just found is less than the radius, 
            //  the sphere intersected the plane.
            if (Math.Abs(distanceFromPlaneToSphere) < sphere.Radius)
            {
                return MySpherePlaneIntersectionEnum.INTERSECTS;
            }
            else if (distanceFromPlaneToSphere >= sphere.Radius)
            {
                //  Else, if the distance is greater than or equal to the radius, the sphere is
                //  completely in FRONT of the plane.
                return MySpherePlaneIntersectionEnum.FRONT;
            }

            //  If the sphere isn't intersecting or in FRONT of the plane, it must be BEHIND
            return MySpherePlaneIntersectionEnum.BEHIND;
        }

        //  This check if sphere and triangleVertexes are in intersection - but this version isn't correct as it check only vertexes and not edges.
        //  Thus if sphere doesn't intersect any vertex, but does intersect one edge, this method will not detect it. But is fast.
        public static bool GetSphereTriangleIntersectionFast(ref BoundingSphere sphere, ref MyTriangle_Vertexes triangle)
        {
            float dist0;
            Vector3.Distance(ref triangle.Vertex0, ref sphere.Center, out dist0);
            if (dist0 <= sphere.Radius) return true;

            float dist1;
            Vector3.Distance(ref triangle.Vertex1, ref sphere.Center, out dist1);
            if (dist1 <= sphere.Radius) return true;

            float dist2;
            Vector3.Distance(ref triangle.Vertex2, ref sphere.Center, out dist2);
            if (dist2 <= sphere.Radius) return true;

            return false;
        }

        //  Method returns intersection point between sphere and triangle (which is defined by vertexes and plane).
        //  If no intersection found, method returns null.
        //  See below how intersection point can be calculated, because it's not so easy - for example sphere vs. triangle will 
        //  hardly generate just intersection point... more like intersection area or something.
        public static Vector3? GetSphereTriangleIntersection(ref BoundingSphere sphere, ref MyPlane trianglePlane, ref MyTriangle_Vertexes triangle)
        {
            //	Vzdialenost gule od roviny trojuholnika
            float distance;

            //	Zistim, ci sa gula nachadza pred alebo za rovinou trojuholnika, alebo ju presekava
            MySpherePlaneIntersectionEnum spherePlaneIntersection = GetSpherePlaneIntersection(ref sphere, ref trianglePlane, out distance);

            //	Ak gula presekava rovinu, tak hladam pseudo-priesecnik
            if (spherePlaneIntersection == MySpherePlaneIntersectionEnum.INTERSECTS)
            {
                //	Offset ktory pomoze vypocitat suradnicu stredu gule premietaneho na rovinu trojuholnika
                Vector3 offset = trianglePlane.Normal * distance;

                //	Priesecnik na rovine trojuholnika, je to premietnuty stred gule na rovine trojuholnika
                Vector3 intersectionPoint;
                intersectionPoint.X = sphere.Center.X - offset.X;
                intersectionPoint.Y = sphere.Center.Y - offset.Y;
                intersectionPoint.Z = sphere.Center.Z - offset.Z;

                if (GetInsidePolygonForSphereCollision(ref intersectionPoint, ref triangle))		//	Ak priesecnik nachadza v trojuholniku
                {
                    //	Toto je pripad, ked sa podarilo premietnut stred gule na rovinu trojuholnika a tento priesecnik sa
                    //	nachadza vnutri trojuholnika (tzn. sedia uhly)
                    return intersectionPoint;
                }
                else													//	Ak sa priesecnik nenachadza v trojuholniku, este stale sa moze nachadzat na hrane trojuholnika
                {
                    Vector3? edgeIntersection = GetEdgeSphereCollision(ref sphere.Center, sphere.Radius / 1.0f, ref triangle);
                    if (edgeIntersection != null)
                    {
                        //	Toto je pripad, ked sa priemietnuty stred gule nachadza mimo trojuholnika, ale intersection gule a trojuholnika tam
                        //	je, pretoze gula presekava jednu z hran trojuholnika. Takze vratim suradnice priesecnika na jednej z hran.
                        return edgeIntersection.Value;
                    }
                }
            }

            //	Sphere doesn't collide with any triangle
            return null;
        }

        //	Return true if point is inside the triangle.
        public static bool GetInsidePolygonForSphereCollision(ref Vector3 point, ref MyTriangle_Vertexes triangle)
        {
            const float MATCH_FACTOR = 0.99f;		// Used to cover up the error in floating point
            float angle = 0.0f;						// Initialize the angle

            //	Spocitame uhol medzi bodmi trojuholnika a intersection bodu (ale na vypocet uhlov pouzivame funkciu ktora je
            //	bezpecna aj pre sphere coldet, problem so SafeACos())
            angle += GetAngleBetweenVectorsForSphereCollision(triangle.Vertex0 - point, triangle.Vertex1 - point);	// Find the angle between the 2 vectors and add them all up as we go along
            angle += GetAngleBetweenVectorsForSphereCollision(triangle.Vertex1 - point, triangle.Vertex2 - point);	// Find the angle between the 2 vectors and add them all up as we go along
            angle += GetAngleBetweenVectorsForSphereCollision(triangle.Vertex2 - point, triangle.Vertex0 - point);	// Find the angle between the 2 vectors and add them all up as we go along

            if (angle >= (MATCH_FACTOR * (2.0 * MathHelper.Pi)))	// If the angle is greater than 2 PI, (360 degrees)
            {
                return true;							// The point is inside of the polygon
            }

            return false;								// If you get here, it obviously wasn't inside the polygon, so Return FALSE
        }

        //	Vrati uhol medzi dvoma vektormi. Tuto funkciu pouzivam cisto na vypocet kolizii medzi sphere a 
        //	trojuholnikmy. Dovod je zvysena bezpecnost, aby sa nestalo ze sa player zasekne v trojuholniku. Suvisi
        //	to s vypoctom acos(). Funkcia M_SafeACos() je sice vhodnejsia na line-triangleVertexes intersekcie, ale menej
        //	vhodna na sphere-triangleVertexes.
        public static float GetAngleBetweenVectorsForSphereCollision(Vector3 vector1, Vector3 vector2)
        {
            //	Get the dot product of the vectors
            float dotProduct = Vector3.Dot(vector1, vector2);

            //	Get the product of both of the vectors magnitudes
            float vectorsMagnitude = vector1.Length() * vector2.Length();

            float angle = (float)Math.Acos(dotProduct / vectorsMagnitude);

            //	Ak bol parameter pre acos() nie v ramci intervalo -1 az +1, tak to je zle, a funkcia musi vratit 0
            if (float.IsNaN(angle) == true)
            {
                return 0.0f;
            }

            //	Vysledny uhol
            return angle;
        }

        //	Returns intersection point between sphere and its edges. But only if there is intersection between sphere and one of the edges.
        //  If sphere intersects somewhere inside the triangle, this method will not detect it.
        public static Vector3? GetEdgeSphereCollision(ref Vector3 sphereCenter, float sphereRadius, ref MyTriangle_Vertexes triangle)
        {
            Vector3 intersectionPoint;

            // This returns the closest point on the current edge to the center of the sphere.
            intersectionPoint = GetClosestPointOnLine(ref triangle.Vertex0, ref triangle.Vertex1, ref sphereCenter);

            // Now, we want to calculate the distance between the closest point and the center
            float distance1 = Vector3.Distance(intersectionPoint, sphereCenter);

            // If the distance is less than the radius, there must be a collision so return true
            if (distance1 < sphereRadius)
            {
                return intersectionPoint;
            }

            // This returns the closest point on the current edge to the center of the sphere.
            intersectionPoint = GetClosestPointOnLine(ref triangle.Vertex1, ref triangle.Vertex2, ref sphereCenter);

            // Now, we want to calculate the distance between the closest point and the center
            float distance2 = Vector3.Distance(intersectionPoint, sphereCenter);

            // If the distance is less than the radius, there must be a collision so return true
            if (distance2 < sphereRadius)
            {
                return intersectionPoint;
            }

            // This returns the closest point on the current edge to the center of the sphere.
            intersectionPoint = GetClosestPointOnLine(ref triangle.Vertex2, ref triangle.Vertex0, ref sphereCenter);

            // Now, we want to calculate the distance between the closest point and the center
            float distance3 = Vector3.Distance(intersectionPoint, sphereCenter);

            // If the distance is less than the radius, there must be a collision so return true
            if (distance3 < sphereRadius)
            {
                return intersectionPoint;
            }

            // The was no intersection of the sphere and the edges of the polygon
            return null;
        }

        public static float GetPointLineDistance(ref MyLine line, ref Vector3 point)
        {
            return GetPointLineDistance(ref line.From, ref line.To, ref point);
        }

        public static float GetPointLineDistance(ref Vector3 linePointA, ref Vector3 linePointB, ref Vector3 point)
        {
            Vector3 line = linePointB - linePointA;
            return Vector3.Cross(line, point - linePointA).Length() / line.Length();
        }

        public static Vector3 GetClosestPointOnLine(ref MyLine line, ref Vector3 point, out float dist)
        {
            return GetClosestPointOnLine(ref line.From, ref line.To, ref point, out dist);
        }

           //	Vypocita bod na ciare o_LinePointA-o_LinePointB ktory je najblizsie k bodu o_Point.
        public static Vector3 GetClosestPointOnLine(
            ref Vector3 linePointA,		//	Zaciatok usecky
            ref Vector3 linePointB,		//	Koniec usecky
            ref Vector3 point				//	Bod
            )
        {
            float dist = 0;
            return GetClosestPointOnLine(ref linePointA, ref linePointB, ref point, out dist);
        }

        //	Vypocita bod na ciare o_LinePointA-o_LinePointB ktory je najblizsie k bodu o_Point.
        public static Vector3 GetClosestPointOnLine(
            ref Vector3 linePointA,		//	Zaciatok usecky
            ref Vector3 linePointB,		//	Koniec usecky
            ref Vector3 point,				//	Bod
            out float dist             
            )
        {
            //	Create the vector from end point vA to our point vPoint.
            Vector3 vector1 = point - linePointA;

            //	Create a normalized direction vector from end point vA to end point vB
            Vector3 vector2 = MyMwcUtils.Normalize(linePointB - linePointA);

            //	Use the distance formula to find the distance of the line segment (or magnitude)
            float d = Vector3.Distance(linePointA, linePointB);

            //	Using the dot product, we project the vVector1 onto the vector vVector2.
            //	This essentially gives us the distance from our projected vector from vA.
            float t = Vector3.Dot(vector2, vector1);

            dist = t;

            //	If our projected distance from vA, "t", is less than or equal to 0, it must
            //	be closest to the end point vA.  We want to return this end point.
            if (t <= 0) return linePointA;

            //	If our projected distance from vA, "t", is greater than or equal to the magnitude
            //	or distance of the line segment, it must be closest to the end point vB.  So, return vB.
            if (t >= d) return linePointB;

            //	Here we create a vector that is of length t and in the direction of vVector2
            Vector3 vector3 = vector2 * t;

            //	To find the closest point on the line segment, we just add vVector3 to the original
            //	end point vA. 
            //	Return the closest point on the line segment
            return linePointA + vector3;
        }

        public static Vector3 GetNormalVectorFromTriangle(ref MyTriangle_Vertexes inputTriangle)
        {
            return MyMwcUtils.Normalize(Vector3.Cross(inputTriangle.Vertex2 - inputTriangle.Vertex0, inputTriangle.Vertex1 - inputTriangle.Vertex0));
        }

        //  Converts bounding box into bounding sphere
        public static BoundingSphere GetBoundingSphereFromBoundingBox(ref BoundingBox box)
        {
            BoundingSphere ret;
            ret.Center = (box.Max + box.Min) / 2.0f;
            ret.Radius = Vector3.Distance(ret.Center, box.Max);
            return ret;
        }

        //  Check intersection between line and bounding box
        public static bool IsLineIntersectingBoundingBox(ref MyLine line, ref BoundingBox boundingBox)
        {
            //  Create temporary ray and do intersection. But we can't rely only on it, because ray doesn't have end, yet our line does, so we 
            //  need to check if ray-bounding_box intersection lies in the range of our line
            MinerWarsMath.Ray ray = new MinerWarsMath.Ray(line.From, line.Direction);
            float? intersectionDistance = boundingBox.Intersects(ray);
            if (intersectionDistance.HasValue == false)
            {
                //  No intersection between ray/line and bounding box
                return false;
            }
            else
            {
                if (intersectionDistance.Value <= line.Length)
                {
                    //  Intersection between ray/line and bounding box IS withing the range of the line
                    return true;
                }
                else
                {
                    //  Intersection between ray/line and bounding box IS NOT withing the range of the line
                    return false;
                }
            }
        }

        //  Calculates intersection between line and bounding box and if found, distance is returned. Otherwise null is returned.
        public static float? GetLineBoundingBoxIntersection(ref MyLine line, ref BoundingBox boundingBox)
        {
            //  Create temporary ray and do intersection. But we can't rely only on it, because ray doesn't have end, yet our line does, so we 
            //  need to check if ray-bounding_box intersection lies in the range of our line
            MinerWarsMath.Ray ray = new MinerWarsMath.Ray(line.From, line.Direction);
            float? intersectionDistance = boundingBox.Intersects(ray);
            if (intersectionDistance.HasValue == false)
            {
                //  No intersection between ray/line and bounding box
                return null;
            }
            else
            {
                if (intersectionDistance.Value <= line.Length)
                {
                    //  Intersection between ray/line and bounding box IS withing the range of the line
                    return intersectionDistance.Value;
                }
                else
                {
                    //  Intersection between ray/line and bounding box IS NOT withing the range of the line
                    return null;
                }
            }
        }

        //  Check intersection between box and sphere
        public static bool IsBoxIntersectingSphere(ref BoundingBox box, ref BoundingSphere sphere)
        {
            bool ret;
            box.Intersects(ref sphere, out ret);
            return ret;
        }

        //  Check intersection between box and sphere
        public static bool IsBoxIntersectingSphere(BoundingBox box, ref BoundingSphere sphere)
        {
            bool ret;
            box.Intersects(ref sphere, out ret);
            return ret;
        }

        //  Check intersection between box and box
        public static bool IsBoxIntersectingBox(ref BoundingBox box0, ref BoundingBox box1)
        {
            bool ret;
            box0.Intersects(ref box1, out ret);
            return ret;
        }

        //  Check intersection between box and box
        public static bool IsBoxIntersectingBox(BoundingBox box0, ref BoundingBox box1)
        {
            bool ret;
            box0.Intersects(ref box1, out ret);
            return ret;
        }

        //  Check intersection between line and bounding sphere
        //  We don't use BoundingSphere.Contains(Ray ...) because ray doesn't have an end, but line does, so we need
        //  to check if line really intersects the sphere.
        public static bool IsLineIntersectingBoundingSphere(ref MyLine line, ref BoundingSphere boundingSphere)
        {
            //  Create temporary ray and do intersection. But we can't rely only on it, because ray doesn't have end, yet our line does, so we 
            //  need to check if ray-bounding_sphere intersection lies in the range of our line
            MinerWarsMath.Ray ray = new MinerWarsMath.Ray(line.From, line.Direction);
            float? intersectionDistance = boundingSphere.Intersects(ray);
            if (intersectionDistance.HasValue == false)
            {
                //  No intersection between ray/line and bounding sphere
                return false;
            }
            else
            {
                if (intersectionDistance.Value <= line.Length)
                {
                    //  Intersection between ray/line and bounding sphere IS withing the range of the line
                    return true;
                }
                else
                {
                    //  Intersection between ray/line and bounding sphere IS NOT withing the range of the line
                    return false;
                }
            }
        }

        //  Checks whether a ray intersects a triangleVertexes. This uses the algorithm
        //  developed by Tomas Moller and Ben Trumbore, which was published in the
        //  Journal of Graphics Tools, pitch 2, "Fast, Minimum Storage Ray-Triangle
        //  Intersection".
        //
        //  This method is implemented using the pass-by-reference versions of the
        //  XNA math functions. Using these overloads is generally not recommended,
        //  because they make the code less readable than the normal pass-by-value
        //  versions. This method can be called very frequently in a tight inner loop,
        //  however, so in this particular case the performance benefits from passing
        //  everything by reference outweigh the loss of readability.
        public static float? GetLineTriangleIntersection(ref MyLine line, ref MyTriangle_Vertexes triangle)
        {
            // Compute vectors along two edges of the triangleVertexes.
            Vector3 edge1, edge2;

            Vector3.Subtract(ref triangle.Vertex1, ref triangle.Vertex0, out edge1);
            Vector3.Subtract(ref triangle.Vertex2, ref triangle.Vertex0, out edge2);

            // Compute the determinant.
            Vector3 directionCrossEdge2;
            Vector3.Cross(ref line.Direction, ref edge2, out directionCrossEdge2);

            float determinant;
            Vector3.Dot(ref edge1, ref directionCrossEdge2, out determinant);

            // If the ray is parallel to the triangleVertexes plane, there is no collision.
            if (determinant > -float.Epsilon && determinant < float.Epsilon)
            {
                return null;
            }

            float inverseDeterminant = 1.0f / determinant;

            // Calculate the U parameter of the intersection point.
            Vector3 distanceVector;
            Vector3.Subtract(ref line.From, ref triangle.Vertex0, out distanceVector);

            float triangleU;
            Vector3.Dot(ref distanceVector, ref directionCrossEdge2, out triangleU);
            triangleU *= inverseDeterminant;

            // Make sure it is inside the triangleVertexes.
            if (triangleU < 0 || triangleU > 1)
            {
                return null;
            }

            // Calculate the V parameter of the intersection point.
            Vector3 distanceCrossEdge1;
            Vector3.Cross(ref distanceVector, ref edge1, out distanceCrossEdge1);

            float triangleV;
            Vector3.Dot(ref line.Direction, ref distanceCrossEdge1, out triangleV);
            triangleV *= inverseDeterminant;

            // Make sure it is inside the triangleVertexes.
            if (triangleV < 0 || triangleU + triangleV > 1)
            {
                return null;
            }

            // Compute the distance along the ray to the triangleVertexes.
            float rayDistance;
            Vector3.Dot(ref edge2, ref distanceCrossEdge1, out rayDistance);
            rayDistance *= inverseDeterminant;

            // Is the triangleVertexes behind the ray origin?
            if (rayDistance < 0)
            {
                return null;
            }

            //  Does the intersection point lie on the line (ray hasn't end, but line does)
            if (rayDistance > line.Length) return null;

            return rayDistance;
        }

        //  Swap two variables
        //  IMPORTANT: I have moved this method back here (from common lib) because obfuscator had problems with this generics and I don't want to solve it...
        public static void Swap<T>(ref T lhs, ref T rhs)
        {
            T temp;
            temp = lhs;
            lhs = rhs;
            rhs = temp;
        }

        //  Finst closes distance between point and axis aligned bounding box (AABB), therefore one if its planes.
        //  It doesn't matter if point is inside or outside of the AABB
        public static float GetClosestDistanceFromPointToAxisAlignedBoundingBox(Vector3 point, BoundingBox axisAlignedBoundingBox)
        {
            float distToMinX = Math.Abs(axisAlignedBoundingBox.Min.X - point.X);
            float distToMaxX = Math.Abs(axisAlignedBoundingBox.Max.X - point.X);
            float distToMinY = Math.Abs(axisAlignedBoundingBox.Min.Y - point.Y);
            float distToMaxY = Math.Abs(axisAlignedBoundingBox.Max.Y - point.Y);
            float distToMinZ = Math.Abs(axisAlignedBoundingBox.Min.Z - point.Z);
            float distToMaxZ = Math.Abs(axisAlignedBoundingBox.Max.Z - point.Z);

            float min = float.MaxValue;
            if (distToMinX < min) min = distToMinX;
            if (distToMaxX < min) min = distToMaxX;
            if (distToMinY < min) min = distToMinY;
            if (distToMaxY < min) min = distToMaxY;
            if (distToMinZ < min) min = distToMinZ;
            if (distToMaxZ < min) min = distToMaxZ;
            return min;
        }

        //  Calculate halfpixel for texture coordinate fix when we copy texture to render target and want
        //  pixels and texels to match precisely.
        //  IMPORTANT: Sometimes half-pixel depends on screen resolution, but sometimes you need to read from
        //  low resolution render targets, and then you need to supply size of that render target (not screen size)
        public static Vector2 GetHalfPixel(int screenSizeX, int screenSizeY)
        {
            return new Vector2(0.5f / (float)screenSizeX, 0.5f / (float)screenSizeY);
        }

        public static BoundingFrustum UnprojectRectangle(MinerWarsMath.Rectangle source, Viewport viewport, Matrix viewMatrix, Matrix projectionMatrix)
        {
            // Point in screen space of the center of the region selected
            Vector2 regionCenterScreen = new Vector2(source.Center.X, source.Center.Y);
            // Generate the projection matrix for the screen region
            Matrix regionProjMatrix = projectionMatrix;
            // Calculate the region dimensions in the projection matrix. M11 is inverse of width, M22 is inverse of height.
            regionProjMatrix.M11 /= ((float)source.Width / (float)viewport.Width);
            regionProjMatrix.M22 /= ((float)source.Height / (float)viewport.Height);
            // Calculate the region center in the projection matrix. M31 is horizonatal center.
            regionProjMatrix.M31 = (regionCenterScreen.X - (viewport.Width / 2f)) / ((float)source.Width / 2f);

            // M32 is vertical center. Notice that the screen has low Y on top, projection has low Y on bottom.
            regionProjMatrix.M32 = -(regionCenterScreen.Y - (viewport.Height / 2f)) / ((float)source.Height / 2f);

            return new BoundingFrustum(viewMatrix * regionProjMatrix);
        }

        public static Vector2 Project3dCoordinateTo2dCoordinate(Vector3 coordinate, Matrix world)
        {
            Vector3 proj = SharpDXHelper.ToXNA(MyMinerGame.Static.GraphicsDevice.Viewport.Project(SharpDXHelper.ToSharpDX(coordinate), SharpDXHelper.ToSharpDX(MyCamera.ProjectionMatrix), SharpDXHelper.ToSharpDX(MyCamera.ViewMatrix), SharpDXHelper.ToSharpDX(world)));
            Vector3 projZero = SharpDXHelper.ToXNA(MyMinerGame.Static.GraphicsDevice.Viewport.Project(SharpDXHelper.ToSharpDX(new Vector3(0, 0, 0)), SharpDXHelper.ToSharpDX(MyCamera.ProjectionMatrix), SharpDXHelper.ToSharpDX(MyCamera.ViewMatrix), SharpDXHelper.ToSharpDX(world)));
            return Vector2.Normalize((new Vector2(projZero.X, projZero.Y) - new Vector2(proj.X, proj.Y)));
        }

        public static Vector3 Unproject2dCoordinateTo3dCoordinate(Vector2 coordinate)
        {
            return MyMwcUtils.Normalize(
                SharpDXHelper.ToXNA(MyMinerGame.Static.GraphicsDevice.Viewport.Unproject(SharpDXHelper.ToSharpDX(new Vector3(coordinate.X, coordinate.Y, 1f)),
                    SharpDXHelper.ToSharpDX(MyCamera.ProjectionMatrix), SharpDXHelper.ToSharpDX(MyCamera.ViewMatrix), SharpDXHelper.ToSharpDX(Matrix.Identity))) 
                    -
                SharpDXHelper.ToXNA(MyMinerGame.Static.GraphicsDevice.Viewport.Unproject(SharpDXHelper.ToSharpDX(new Vector3(coordinate.X, coordinate.Y, 0f)),
                    SharpDXHelper.ToSharpDX(MyCamera.ProjectionMatrix), SharpDXHelper.ToSharpDX(MyCamera.ViewMatrix), SharpDXHelper.ToSharpDX(Matrix.Identity)))
                    );
        }

        /// <summary>
        /// Converts the 2D mouse position to a 3D ray for collision tests.
        /// </summary>
        public static MinerWarsMath.Ray ConvertMouseToRay(Vector2 screenMousePosition)
        {
            Vector3 nearPoint = new Vector3(screenMousePosition, 0f);
            Vector3 farPoint = new Vector3(screenMousePosition, 1f);

            nearPoint = SharpDXHelper.ToXNA(MyMinerGame.Static.GraphicsDevice.Viewport.Unproject(SharpDXHelper.ToSharpDX(nearPoint),
                SharpDXHelper.ToSharpDX(MyCamera.ProjectionMatrix),
                SharpDXHelper.ToSharpDX(MyCamera.ViewMatrix),
                SharpDXHelper.ToSharpDX(Matrix.Identity)));
            farPoint = SharpDXHelper.ToXNA(MyMinerGame.Static.GraphicsDevice.Viewport.Unproject(SharpDXHelper.ToSharpDX(farPoint),
                SharpDXHelper.ToSharpDX(MyCamera.ProjectionMatrix),
                SharpDXHelper.ToSharpDX(MyCamera.ViewMatrix),
                SharpDXHelper.ToSharpDX(Matrix.Identity)));

            Vector3 direction = farPoint - nearPoint;
            direction = MyMwcUtils.Normalize(direction);

            return new MinerWarsMath.Ray(nearPoint, direction);
        }

        /// <summary>
        /// Converts the 2D mouse position to a 3D ray for collision tests.
        /// </summary>
        public static MinerWarsMath.Ray ConvertMouseToRay()
        {
            return ConvertMouseToRay(MyGuiManager.GetScreenCoordinateFromNormalizedCoordinate(MyGuiManager.MouseCursorPosition));
        }

        /// <summary>
        /// Converts the 2D mouse position to a MyLine for collision tests.
        /// </summary>
        public static MyLine ConvertMouseToLine()
        {
            Vector3 mouseCursorDirection = MyGuiManager.GetMouseCursorDirection();

            Vector3 nearPoint = MyCamera.Position;
            Vector3 farPoint = nearPoint + mouseCursorDirection * 100000;

            return new MyLine(nearPoint, farPoint, true);
        }

        //  This method returns next or previous element in enumeration relatively to currentEnumValue
        public static T GetNextOrPreviousEnumValue<T>(T currentEnumValue, bool nextElement)
            where T : struct
        {
            int elementIndex = GetEnumMemberIndex(currentEnumValue);
            Array enumValues = Enum.GetValues(typeof(T));
            int enumValuesCount = enumValues.Length;
            if (nextElement) // get next element from enum
            {
                if (elementIndex != enumValuesCount - 1)
                {
                    elementIndex++;
                }
                else
                {
                    elementIndex = 0;
                }
            }
            else // get previous element from enum
            {
                if (elementIndex != 0)
                {
                    elementIndex--;
                }
                else
                {
                    elementIndex = enumValuesCount - 1;
                }
            }

            foreach (T enumValue in enumValues)
            {
                if (Array.IndexOf(enumValues, enumValue) == elementIndex)
                {
                    return enumValue;
                }
            }

            return currentEnumValue;
        }


        //  This method return index of passed element in given enumeration
        public static int GetEnumMemberIndex<T>(T element)
            where T : struct
        {
            T[] values = (T[])Enum.GetValues(typeof(T));
            return Array.IndexOf(values, element);
        }

        /// <summary>
        /// This method rounds each vector component floating point value to specified number of fractional digits
        /// </summary>
        public static Vector3 GetRoundedVector3(Vector3 vec, int precisionDigits)
        {
            return new Vector3((float)Math.Round(vec.X, precisionDigits), (float)Math.Round(vec.Y, precisionDigits), (float)Math.Round(vec.Z, precisionDigits));
        }


        //  Draw bounding box around whole sector - but around its "safe border" - therefore extended "safe zone"
        public static void DrawSectorBoundingBox()
        {
            float size = MyMwcSectorConstants.SAFE_SECTOR_SIZE;
            float sizeHalf = MyMwcSectorConstants.SAFE_SECTOR_SIZE_HALF;

            //  Two passes - because we want to render current sector in second pass so its lines will over-draw other sector lines and will be always visible
            for (int pass = 0; pass <= 1; pass++)
            {
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        for (int z = -1; z <= 1; z++)
                        {
                            bool isCurrentSector = ((x == 0) && (y == 0) && (z == 0));
                            if (((isCurrentSector) && (pass == 1)) || ((isCurrentSector == false) && (pass == 0)))
                            {
                                //  Sector left-bottom corner (not its center)
                                Vector3 origin = new Vector3(x * size, y * size, z * size) + new Vector3(-sizeHalf, -sizeHalf, -sizeHalf);

                                Color color = isCurrentSector ? Color.White : Color.Blue;

                                MyDebugDraw.DrawLine3D(origin, origin + new Vector3(size, 0, 0), color, color);
                                MyDebugDraw.DrawLine3D(origin, origin + new Vector3(0, size, 0), color, color);
                                MyDebugDraw.DrawLine3D(origin, origin + new Vector3(0, 0, size), color, color);
                                MyDebugDraw.DrawLine3D(origin + new Vector3(0, size, 0), origin + new Vector3(size, size, 0), color, color);
                                MyDebugDraw.DrawLine3D(origin + new Vector3(size, size, 0), origin + new Vector3(size, size, size), color, color);
                                MyDebugDraw.DrawLine3D(origin + new Vector3(size, 0, 0), origin + new Vector3(size, 0, size), color, color);
                                MyDebugDraw.DrawLine3D(origin + new Vector3(0, size, 0), origin + new Vector3(0, size, size), color, color);
                                MyDebugDraw.DrawLine3D(origin + new Vector3(0, 0, size), origin + new Vector3(0, size, size), color, color);
                                MyDebugDraw.DrawLine3D(origin + new Vector3(size, 0, size), origin + new Vector3(size, size, size), color, color);
                                MyDebugDraw.DrawLine3D(origin + new Vector3(size, 0, 0), origin + new Vector3(size, size, 0), color, color);
                                MyDebugDraw.DrawLine3D(origin + new Vector3(0, size, size), origin + new Vector3(size, size, size), color, color);
                                MyDebugDraw.DrawLine3D(origin + new Vector3(0, 0, size), origin + new Vector3(size, 0, size), color, color);
                            }
                        }
                    }
                }
            }
        }

        // Get brightnest from the current color (using 0.299 - 0.114 - 0.587 values):
        public static float GetYFromRGB(Color color)
        {
            /* Wr = 0.299;
             * Wb = 0.114;
             * Wg = 1 - r - b = 0.587; */
            return color.R * 0.299f + color.G * 0.587f + color.B * 0.114f;
        }

        // Get U color difference:
        public static float GetUFromRGB(Color color)
        {
            /* Umax = 0.436;
             * 1 - Wb = 0.886; */
            return 0.436f * ((color.B - GetYFromRGB(color)) / 0.886f);
        }

        // Get V color difference:
        public static float GetVFromRGB(Color color)
        {
            /* Vmax = 0.615;
             * 1 - Wr = 0.701; */
            return 0.615f * ((color.R - GetYFromRGB(color)) / 0.701f);
        }

        // Mix RGB color from YUV components:
        public static Color GetRGBFromYUV(float Y, float U, float V)
        {
            /* Umax = 0.436;
             * Vmax = 0.615;
             * 1 - Wr = 0.701;
             * 1 - Wb = 0.886; 
             * Wr = 0.299;
             * Wb = 0.114;
             * Wg = 1 - r - b = 0.587 
             * see: http://en.wikipedia.org/wiki/YUV for formulas and constants
             */
            float R = Y + V * 1.139837f;
            float G = Y - U * 0.032930f - V * 0.580599f;
            float B = Y + U * 2.032110f;
            return new Color(R, G, B);
        }

        // Set brightnest to the current color (using 0.299 - 0.114 - 0.587 values)
        // Y in interval 0.0f - 1.0f
        public static Color SetYtoRGB(Color color, float Y)
        {
            float U, V;
            U = GetUFromRGB(color);
            V = GetVFromRGB(color);
            byte A = color.A;
            color = GetRGBFromYUV(Y, U, V);
            color.A = A;
            return color;
        }

        /// <summary>
        /// Returns true if Vector3 is valid
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public static bool IsValid(Vector3 vec)
        {
            return MyCommonDebugUtils.IsValid(vec.X) && MyCommonDebugUtils.IsValid(vec.Y) && MyCommonDebugUtils.IsValid(vec.Z);
        }

        public static bool IsValidNormal(Vector3 vec)
        {
            const float epsilon = 0.001f;
            var length = vec.LengthSquared();
            return IsValid(vec) && length > 1 - epsilon && length < 1 + epsilon;
        }

        /// <summary>
        /// Returns true if Vector2 is valid
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public static bool IsValid(Vector2 vec)
        {
            return MyCommonDebugUtils.IsValid(vec.X) && MyCommonDebugUtils.IsValid(vec.Y);
        }

        /// <summary>
        /// Returns true if float is valid
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static bool IsValid(float f)
        {
            return MyCommonDebugUtils.IsValid(f);
        }

        /// <summary>
        /// Returns true if Vector3 is valid
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public static bool IsValid(Vector3? vec)
        {
            return vec == null ? true : MyCommonDebugUtils.IsValid(vec.Value.X) && MyCommonDebugUtils.IsValid(vec.Value.Y) && MyCommonDebugUtils.IsValid(vec.Value.Z);
        }

        public static bool IsValid(Matrix matrix)
        {
            return IsValid(matrix.Up) && IsValid(matrix.Left) && IsValid(matrix.Forward) && IsValid(matrix.Translation) && (matrix != ZeroMatrix);
        }

        /// <summary>
        /// Returns true if Vector3 is valid
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public static void AssertIsValid(Vector3 vec)
        {
            System.Diagnostics.Debug.Assert(IsValid(vec));
        }

        /// <summary>
        /// Returns true if Vector3 is valid
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public static void AssertIsValid(Vector3? vec)
        {
            System.Diagnostics.Debug.Assert(IsValid(vec));
        }

        /// <summary>
        /// Returns true if Vector2 is valid
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public static void AssertIsValid(Vector2 vec)
        {
            System.Diagnostics.Debug.Assert(IsValid(vec));
        }

        /// <summary>
        /// Returns true if float is valid
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static void AssertIsValid(float f)
        {
            System.Diagnostics.Debug.Assert(IsValid(f));
        }

        public static void AssertIsValid(Matrix matrix)
        {
        //    System.Diagnostics.Debug.Assert(IsValid(matrix));
        }

        /// <summary>
        /// Returns true if value is power of two
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static bool IsPowerOfTwo(int x)
        {
            return ((x > 0) && ((x & (x - 1)) == 0));
        }


        /// <summary>
        /// Returns true if values are considered as same
        /// </summary>
        public static bool IsEqual(float value1, float value2)
        {
            return MyMwcUtils.IsZero(value1 - value2);
        }

        /// <summary>
        /// Returns true if values are considered as same
        /// </summary>
        public static bool IsEqual(Vector2 value1, Vector2 value2)
        {
            return MyMwcUtils.IsZero(value1.X - value2.X) && MyMwcUtils.IsZero(value1.Y - value2.Y);
        }

        /// <summary>
        /// Returns true if values are considered as same
        /// </summary>
        public static bool IsEqual(Vector3 value1, Vector3 value2)
        {
            return MyMwcUtils.IsZero(value1.X - value2.X) && MyMwcUtils.IsZero(value1.Y - value2.Y) && MyMwcUtils.IsZero(value1.Z - value2.Z);
        }

        /// <summary>
        /// Returns true if values are considered as same
        /// </summary>
        public static bool IsEqual(Matrix value1, Matrix value2)
        {
            return MyMwcUtils.IsZero(value1.Left - value2.Left)
                && MyMwcUtils.IsZero(value1.Up - value2.Up)
                && MyMwcUtils.IsZero(value1.Forward - value2.Forward)
                && MyMwcUtils.IsZero(value1.Translation - value2.Translation);
        }

        public static decimal GetTextureSizeInMb(MyTexture2D texture)
        {
            return CalculateTextureSizeInMb(texture.Format, texture.Width, texture.Height, texture.LevelCount);
        }

        public static decimal GetTextureSizeInMb(MyTextureCube texture)
        {
            return 6 * CalculateTextureSizeInMb(texture.Format, texture.Size, texture.Size, texture.LevelCount);
        }

        //  Calculate texture size, based on width * resolution, texture format (number of bytes per one pixel) and number of mip-map levels
        //  Result is in mega bytes (not bytes)
        //  IMPORTANT: I am not sure if I am doing this correctly for non-uniform mip maps (e.g. 512x128) because each dimension should
        //  have different mipmap level count, but there's only one. In forst case, result will be wrong just by few bytes.
        public static decimal CalculateTextureSizeInMb(Format inputFormat, int inputWidth, int inputHeight, int inputLevelCount)
        {
            int sizeInBytes = 0;

            int width = inputWidth;
            int height = inputHeight;

            for (int level = 0; level < inputLevelCount; level++)
            {
                int sizeOfOneLevel;

                MyCommonDebugUtils.AssertRelease(width >= 1);
                MyCommonDebugUtils.AssertRelease(height >= 1);

                switch (inputFormat)
                {
                    case Format.A8:
                    case Format.L8:
                        sizeOfOneLevel = width * height;
                        break;

                    case Format.A8R8G8B8:
                    case Format.A8B8G8R8:
                    case Format.D24S8:
                    case Format.A2R10G10B10:
                    case Format.Q8W8V8U8:
                        sizeOfOneLevel = width * height * 4;
                        break;

                    case Format.Dxt1:
                        sizeOfOneLevel = (width * height * 3) / 8;
                        break;

                    case Format.Dxt3:
                    case Format.Dxt5:
                        sizeOfOneLevel = (width * height * 4) / 4;
                        break;
                    case Format.R32F:
                        sizeOfOneLevel = (width * height * 4);
                        break;
                    case Format.A16B16G16R16:
                    case Format.A16B16G16R16F:
                        sizeOfOneLevel = (width * height * 4) * 2;
                        break;
                    default:
                        throw new Exception("You are trying to calculate 'texture size in Mb' on a texture whose format is not yet supported by this method. You should extend this method!");
                        break;
                }

                sizeInBytes += sizeOfOneLevel;

                if (width > 1) width /= 2;
                if (height > 1) height /= 2;
            }

            return sizeInBytes / 1024m / 1024m;
        }


        public static void SerializeValue(XmlWriter writer, Vector3 v)
        {
            writer.WriteValue(v.X.ToString(CultureInfo.InvariantCulture) + " " + v.Y.ToString(CultureInfo.InvariantCulture) + " " + v.Z.ToString(CultureInfo.InvariantCulture));
        }

        public static void SerializeValue(XmlWriter writer, Vector4 v)
        {
            writer.WriteValue(v.X.ToString(CultureInfo.InvariantCulture) + " " + v.Y.ToString(CultureInfo.InvariantCulture) + " " + v.Z.ToString(CultureInfo.InvariantCulture) + " " + v.W.ToString(CultureInfo.InvariantCulture));
        }

        public static void DeserializeValue(XmlReader reader, out Vector3 value)
        {
            object val = reader.Value;
            reader.Read();

            string[] parts = ((string)val).Split(' ');
            Vector3 v = new Vector3(Convert.ToSingle(parts[0], CultureInfo.InvariantCulture), Convert.ToSingle(parts[1], CultureInfo.InvariantCulture), Convert.ToSingle(parts[2], CultureInfo.InvariantCulture));
            value = v;
        }

        public static void DeserializeValue(XmlReader reader, out Vector4 value)
        {
            object val = reader.Value;
            reader.Read();

            string[] parts = ((string)val).Split(' ');
            Vector4 v = new Vector4(Convert.ToSingle(parts[0], CultureInfo.InvariantCulture), Convert.ToSingle(parts[1], CultureInfo.InvariantCulture), Convert.ToSingle(parts[2], CultureInfo.InvariantCulture), Convert.ToSingle(parts[3], CultureInfo.InvariantCulture));
            value = v;
        }

        public static float ReadSingleSafe(string text)
        {
            text = text.Replace(',', '.');
            return Convert.ToSingle(text, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Get the first element from the HashSet.
        /// </summary>
        public static T FirstElement<T>(this HashSet<T> set)
        {
            var enumerator = set.GetEnumerator();  // This doesn't do allocations, Petr.
            enumerator.MoveNext();
            return enumerator.Current;
        }

        public static bool IsPointInTriangle(ref Vector3 point, ref MyTriangle_Vertexes triangle)
        {

 

            //Pretend each side of the polygon is a plane, check which side
            //of each plane the point lies (<0 is outside)

            Vector3 vert1 = triangle.Vertex0;
            Vector3 vert2 = triangle.Vertex1;
            Vector3 vert3 = triangle.Vertex2;
            Vector3 normal = GetNormalVectorFromTriangle(ref triangle);

            //First side
            Vector3 v3 = vert2 - vert1;

            //work out the normal to our new imaginary plane
            Vector3 v32 = Vector3.Cross(v3, normal);

            //work out the last value in the plane equation
            double ld = Vector3.Dot(v32, vert1);

            //use the plane equation on our point and check its side

            ld = Vector3.Dot(v32, point) - ld;
            if (ld < -0.0001)
                return false;

            //Second side
            v3 = vert3 - vert2;
            v32 = Vector3.Cross(v3, normal);
            ld = Vector3.Dot(v32, vert2);

            ld = Vector3.Dot(v32, point) - ld;
            if (ld < -0.0001)
                return false;

            //Third side
            v3 = vert1 - vert3;
            v32 = Vector3.Cross(v3, normal);
            ld = Vector3.Dot(v32, vert3);

            ld = Vector3.Dot(v32, point) - ld;

            if (ld < -0.0001)
                return false;

            return true;
        }

        public static float GetDistancePointToQuad(ref Vector3 point, ref MyQuad quad)
        {
            float minDistance = float.MaxValue;

            MyTriangle_Vertexes tri1 = new MyTriangle_Vertexes();
            tri1.Vertex0 = quad.Point0;
            tri1.Vertex1 = quad.Point1;
            tri1.Vertex2 = quad.Point2;

            MyTriangle_Vertexes tri2 = new MyTriangle_Vertexes();
            tri2.Vertex0 = quad.Point2;
            tri2.Vertex1 = quad.Point1;
            tri2.Vertex2 = quad.Point3;

            if (IsPointInTriangle(ref point, ref tri1) || IsPointInTriangle(ref point, ref tri2))
            {
                MyPlane plane = new MyPlane(ref tri1);
                return System.Math.Abs(GetDistanceFromPointToPlane(ref point, ref plane));
            }

            MyLine line1 = new MyLine(quad.Point0, quad.Point1);
            MyLine line2 = new MyLine(quad.Point1, quad.Point3);
            MyLine line3 = new MyLine(quad.Point3, quad.Point2);
            MyLine line4 = new MyLine(quad.Point2, quad.Point0);

            float distance = 0;
            Vector3 closestPoint = GetClosestPointOnLine(ref line1, ref point, out distance);
            distance = Vector3.Distance(point, closestPoint);
            if (distance < minDistance)
                minDistance = distance;

            closestPoint = GetClosestPointOnLine(ref line2, ref point, out distance);
            distance = Vector3.Distance(point, closestPoint);
            if (distance < minDistance)
                minDistance = distance;

            closestPoint = GetClosestPointOnLine(ref line3, ref point, out distance);
            distance = Vector3.Distance(point, closestPoint);
            if (distance < minDistance)
                minDistance = distance;

            closestPoint = GetClosestPointOnLine(ref line4, ref point, out distance);
            distance = Vector3.Distance(point, closestPoint);
            if (distance < minDistance)
                minDistance = distance;

            return minDistance;
        }

        public static string GetDatetimeAsSpentTime(DateTime dt)
        {
            TimeSpan diff = DateTime.Now - dt;
            if (diff.TotalMinutes < 60)
            {
                return String.Format(MyTextsWrapper.Get(MyTextsWrapperEnum.DateMinutesAgo).ToString(), (int)diff.TotalMinutes);
            }
            else if (diff.TotalHours < 24)
            {
                return String.Format(MyTextsWrapper.Get(MyTextsWrapperEnum.DateHoursAgo).ToString(), (int)diff.TotalHours);
            }
            else if (diff.TotalDays < 31)
            {
                return String.Format(MyTextsWrapper.Get(MyTextsWrapperEnum.DateDaysAgo).ToString(), (int)diff.TotalDays);
            }
            else
            {
                return String.Format(MyTextsWrapper.Get(MyTextsWrapperEnum.DateMonthsAgo).ToString(), (int)(diff.TotalDays / 30));
            }
        }
    }
}