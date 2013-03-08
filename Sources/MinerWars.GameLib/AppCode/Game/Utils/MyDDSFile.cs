#define COLOR_SAVE_TO_ARGB

using System;
using System.IO;
using MinerWars.AppCode.App;
using SysUtils.Utils;

//using MinerWarsMath.Graphics;
using SharpDX.Direct3D9;

//  Read/Write dds files from/to files or from streams.
namespace MinerWars.AppCode.Game.Utils
{
    static class MyDDSFile
    {
        public static void DDSFromFile(string fileName, Device device, bool loadMipMap, int offsetMipMaps, out SharpDX.Direct3D9.Texture texture)
        {
            Stream stream = File.OpenRead(fileName);
            SharpDX.Direct3D9.Texture tex;
            InternalDDSFromStream(stream, device, 0, loadMipMap, offsetMipMaps, out tex);
            stream.Close();
            stream.Dispose();

            texture = tex as SharpDX.Direct3D9.Texture;
            if (texture == null)
            {
                throw new Exception("The data in the stream contains a TextureCube not Texture2D");
            }
        }


        public static void DDSFromFile(string fileName, Device device, bool loadMipMap, int offsetMipMaps, out CubeTexture texture)
        {
            Stream stream = File.OpenRead(fileName);
            CubeTexture tex = null;
            InternalDDSFromStream(stream, device, 0, loadMipMap, 0, out tex);
            stream.Close();
            stream.Dispose();

            texture = tex as CubeTexture;
            if (texture == null)
            {
                throw new Exception("Error while loading TextureCube");
            }
        }

      
        //loads the data from a stream in to a texture object.
        private static void InternalDDSFromStream(Stream stream, Device device, int streamOffset, bool loadMipMap, int offsetMipMaps, out SharpDX.Direct3D9.Texture texture)
        {
            stream.Position = 0;
            texture = SharpDX.Direct3D9.Texture.FromStream(device, stream, 0, 0, offsetMipMaps, Usage.None, Format.Unknown, Pool.Default, Filter.None, Filter.None, 0); 
        }

        //loads the data from a stream in to a texture object.
        private static void InternalDDSFromStream(Stream stream, Device device, int streamOffset, bool loadMipMap, int offsetMipMaps, out CubeTexture texture)
        {
            stream.Position = 0;
            texture = SharpDX.Direct3D9.CubeTexture.FromStream(device, stream, 0, 0, offsetMipMaps, Usage.None, Format.Unknown, Pool.Default, Filter.None, Filter.None, 0);
        }


        /// <summary>
        /// Save a texture from memory to a file.
        /// (Supported formats : Dxt1,Dxt3,Dxt5,A8R8G8B8/Color,A4R4G4B4,A1R5G5B5,R5G6B5,A8,
        /// FP32/Single,FP16/HalfSingle,FP32x4/Vector4,FP16x4/HalfVector4,CxV8U8/NormalizedByte2/CxVU,Q8VW8V8U8/NormalizedByte4/8888QWVU
        /// ,HalfVector2/G16R16F/16.16fGR,Vector2/G32R32F,G16R16/RG32/1616GB,A8B8G8R8,A2B10G10R10/Rgba1010102,A16B16G16R16/Rgba64)
        /// </summary>
        /// <param name="fileName">The name of the file where you want to save the texture.</param>
        /// <param name="saveMipMaps">Save the complete mip-map chain ?</param>
        /// <param name="texture">The texture that you want to save.</param>
        /// <param name="throwExceptionIfFileExist">Throw an exception if the file exists ?</param>
        public static void DDSToFile(string fileName, bool saveMipMaps, BaseTexture texture, bool throwExceptionIfFileExist)
        {
            if (throwExceptionIfFileExist && File.Exists(fileName))
            {
                throw new Exception("The file allready exists and \"throwExceptionIfFileExist\" is true");
            }

            Stream fileStream = null;
            try
            {
                fileStream = File.Create(fileName);
                BaseTexture.ToStream(texture, ImageFileFormat.Dds);
                //DDSToStream(fileStream, 0, saveMipMaps, texture);
                // sometimes needed because of out of memory and this helps
                //GC.Collect(2);
            }            
            catch (Exception x)
            {
                throw x;
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                    fileStream = null;
                }
            }

        }
    }
}
