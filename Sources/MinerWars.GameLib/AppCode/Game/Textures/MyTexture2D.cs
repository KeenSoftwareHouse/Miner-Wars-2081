using System;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Managers;
using MinerWars.AppCode.Game.Utils;
using SysUtils.Utils;

using MinerWarsMath;
//
//using MinerWarsMath.Graphics;

using SharpDX.Direct3D9;

namespace MinerWars.AppCode.Game.Textures
{
    /// <summary>
    /// 
    /// </summary>
    internal class MyTexture2D : MyTexture
    {
        #region Properties

        /// <summary>
        /// Gets the bounds.
        /// </summary>
        public Rectangle Bounds
        {
            get
            {
                return new Rectangle(0,0, Width, Height);
            }
        }


        /// <summary>
        /// Gets the size of texture in MB.
        /// </summary>
        public override float Memory
        {
            get
            {
                if (this.LoadState == LoadState.Loaded)
                {
                    return (float)MyUtils.GetTextureSizeInMb(this);
                }

                return 0f;
            }
        }

        #endregion

        
        /// <summary>
        /// Initializes a new instance of the <see cref="MyTexture2D"/> class.
        /// </summary>
        private MyTexture2D(Texture right)
            : base(string.Empty, LoadMethod.Lazy, TextureFlags.None)
        {
            this.texture = right;
            this.LoadState = LoadState.Loaded;
        } 

        /// <summary>
        /// Initializes a new instance of the <see cref="MyTexture2D"/> class.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="manager">The manager.</param>
        /// <param name="loadMethod">if set to <c>true</c> [external load].</param>
        /// <param name="flags">The flags.</param>
        public MyTexture2D(string path, LoadMethod loadMethod, TextureFlags flags)
            : base(path, loadMethod, flags)
        {
        }

        protected override BaseTexture LoadPNGTexture(string fileName)
        {
            return Texture.FromFile(MyMinerGame.Static.GraphicsDevice, fileName);
        }

        /// <summary>
        /// Loads the DDS texture.
        /// </summary>
        /// <param name="fileName">The name.</param>
        /// <param name="quality"></param>
        /// <returns></returns>
        protected override BaseTexture LoadDDSTexture(string fileName, TextureQuality quality)
        {
            var device = MyMinerGame.Static.GraphicsDevice;
            if (device == null || device.IsDisposed)
            {
                return null;
            }

            //cannot use profiler because of multithreading
            //int loadDDSTextureBlock = -1;
            //MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyTexture2D.LoadDDSTexture", ref loadDDSTextureBlock);

            MyMwcLog.WriteLine(string.Format("Loading DDS texture {0} ...", fileName), SysUtils.LoggingOptions.LOADING_TEXTURES);

            Texture loadedTexture = null;

            if (this.flags.HasFlag(TextureFlags.IgnoreQuality))
            {
                quality = TextureQuality.Full;
            }

            MyDDSFile.DDSFromFile(fileName, device, true, (int)quality, out loadedTexture);
            loadedTexture.Tag = this;

            if (!MyUtils.IsPowerOfTwo(loadedTexture.GetLevelDescription(0).Width) || !MyUtils.IsPowerOfTwo(loadedTexture.GetLevelDescription(0).Height))
            {
                throw new FormatException("Size must be power of two!");
            }

            //cannot use profiler because of multithreading
            //MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock(loadDDSTextureBlock);
            return loadedTexture;
        }


        /// <summary>
        /// Called when [loaded].
        /// </summary>
        protected override void OnLoaded()
        {
            base.OnLoaded();
            if (MyUtils.IsTextureMipMapped(this) == false)
            {
                //MyMwcLog.IncreaseIndent();
                MyMwcLog.WriteLine("TextureNotMipMapped " + this.Name.ToString());
                //MyMwcLog.DecreaseIndent();
            }
            if (MyUtils.IsTextureDxtCompressed(this) == false)
            {
                MyPerformanceCounter.PerAppLifetime.NonDxtCompressedTexturesCount++;
                //MyMwcLog.IncreaseIndent();
                MyMwcLog.WriteLine("TextureNotCompressed " + this.Name.ToString());
                //MyMwcLog.DecreaseIndent();
            }
        }

        protected override void OnUnloading()
        {
            base.OnUnloading();
        }

        #region Operators
                
        public static implicit operator MyTexture2D(Texture right)
        {
            if (right == null)
            {
                return null;
            }

            return new MyTexture2D(right);
        }     

        /// <summary>
        /// Performs an implicit conversion from <see cref="MinerWars.AppCode.Game.Textures.MyTexture2D"/> to <see cref="MinerWarsMath.Graphics.Texture2D"/>.
        /// </summary>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator SharpDX.Direct3D9.Texture(MyTexture2D right)
        {
            if (right == null || !right.RequestAccess())
            {
                return null;
            }

            return (SharpDX.Direct3D9.Texture)right.texture;
        }

        #endregion
    }
}