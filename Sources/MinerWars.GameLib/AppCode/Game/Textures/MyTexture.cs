using System;
using System.Diagnostics;
using System.IO;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Managers;
using SysUtils.Utils;
using KeenSoftwareHouse.Library.Parallelization.Threading;

//using MinerWarsMath.Graphics;
using SharpDX.Direct3D9;

namespace MinerWars.AppCode.Game.Textures
{
    /// <summary>
    /// Loading flags
    /// </summary>
    [Flags]
    public enum TextureFlags
    {
        /// <summary>
        /// No flags
        /// </summary>
        None = 1 << 0,

        /// <summary>
        /// Texture will ignore any quality override and always will use TextureQuality.Full 
        /// </summary>
        IgnoreQuality = 1 << 1,
    }

    /// <summary>
    /// The load method for a texture.
    /// </summary>
    internal enum LoadMethod
    {
        /// <summary>
        /// Someone else loads me.
        /// </summary>
        External,

        /// <summary>
        /// I load myself synchronously when needed for the first time.
        /// </summary>
        Lazy,

        /// <summary>
        /// I start loading myself asynchronously when needed for the first time.
        /// </summary>
        LazyBackground,
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="texture">The texture.</param>
    internal delegate void TextureLoadedHandler(MyTexture texture);

    /// <summary>
    /// 
    /// </summary>
    internal abstract class MyTexture : MyResource
    {
        #region Fields

        /// <summary>
        /// XNA internl texture.
        /// </summary>
        protected SharpDX.Direct3D9.BaseTexture texture;

        /// <summary>
        /// Texture flags setting
        /// </summary>
        protected readonly TextureFlags flags;

        /// <summary>
        /// State of loading.
        /// </summary>
        private volatile LoadState loadState;

        #endregion

        #region Events

        /// <summary>
        /// Occurs when [texture loaded].
        /// </summary>
        public event TextureLoadedHandler TextureLoaded;

        #endregion

        #region Properties

        public int Width { get; private set; }
        public int Height { get; private set; }
        public Format Format { get; private set; }

        /// <summary>
        /// Gets the level count.
        /// </summary>
        public int LevelCount
        {
            get
            {
                if (RequestAccess())
                {
                    return this.texture.LevelCount;
                }

                return 0;
            }
        }


        /// <summary>
        /// Gets a value indicating whether this instance is valid.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </value>
        public bool IsValid
        {
            get
            {                                                           //alocation!
                return this.texture != null && !this.texture.IsDisposed /*&& !this.texture.Device.IsDisposed*/;
            }
        }

        /// <summary>
        /// Gets or sets the state of the load.
        /// </summary>
        /// <value>
        /// The state of the load.
        /// </value>
        public LoadState LoadState
        {
            get
            {
                return this.loadState;
            }
            internal set
            {
                this.loadState = value;
            }
        }

        protected void UpdateProperties(BaseTexture texture)
        {
            SurfaceDescription desc;
            Texture tex = texture as Texture;
            if (tex != null)
                desc = tex.GetLevelDescription(0);
            else
            {
                CubeTexture ctex = texture as CubeTexture;
                desc = ctex.GetLevelDescription(0);
            }

            Width = desc.Width;
            Height = desc.Height;
            Format = desc.Format;
        }

        /// <summary>
        /// Gets the size of texture in MB.
        /// </summary>
        public abstract float Memory { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="MyTexture"/> class.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="manager">The manager.</param>
        /// <param name="loadMethod">The load method. See <code>LoadMethod</code> enum.</param>
        /// <param name="flags">The flags.</param>
        protected MyTexture(string path, LoadMethod loadMethod, TextureFlags flags)
        {
            this.flags = flags;
            this.Name = path;
            //  this.Manager = manager;
            switch (loadMethod)
            {
                case LoadMethod.External:
                    this.LoadState = LoadState.Pending;
                    break;
                case LoadMethod.Lazy:
                    this.LoadState = LoadState.LoadYourself;
                    break;
                case LoadMethod.LazyBackground:
                    this.LoadState = LoadState.LoadYourselfBackground;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("loadMethod");
            }
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="MyTexture"/> is reclaimed by garbage collection.
        /// </summary>
        ~MyTexture()
        {
            //This storkovina crashes with multiple VSync changes
           // MyTextureManager.UnloadTexture(this);
        }

        public bool IgnoreQuality
        {
            get
            {
                return flags.HasFlag(TextureFlags.IgnoreQuality);
            }
        }

        /// <summary>
        /// Reloads this instance.
        /// </summary>
        internal bool Load(TextureQuality quality = 0)
        {
            Debug.Assert(this.LoadState != LoadState.Loaded);

            MyMwcLog.WriteLine(string.Format("Loading texture {0} ...", this.Name), SysUtils.LoggingOptions.LOADING_TEXTURES);

            var ddsTexture = new FileInfo(MyMinerGame.Static.RootDirectory + "\\" + this.Name + ".dds");
           // System.Diagnostics.Debug.Assert(ddsTexture.Exists);
            if (ddsTexture.Exists)
            {
                this.texture = LoadDDSTexture(ddsTexture.FullName, quality);// : LoadXNATexture(this.Name);
            }
            else
            {
                var pngTexture = new FileInfo(MyMinerGame.Static.RootDirectory + "\\" + this.Name + ".png");
                if (pngTexture.Exists)
                {
                    this.texture = LoadPNGTexture(pngTexture.FullName);// : LoadXNATexture(this.Name);
                }
                else
                {
                    this.LoadState = LoadState.Error;
                    string s = "Texture " + this.Name + " is missing.";
                    System.Diagnostics.Debug.Assert(pngTexture.Exists, s);
                    return false;
                }
            }

            if (this.texture == null)
            {
                this.LoadState = LoadState.Error;

                return false;
            }

            this.texture.DebugName = this.Name;
            this.LoadState = LoadState.Loaded;

         //   if (Name.Contains("Textures\\GUI\\Loading"))
          //  {
            //}

            UpdateProperties(texture);

            MyMwcLog.WriteLine(string.Format("Texture {0} loaded", this.Name), SysUtils.LoggingOptions.LOADING_TEXTURES);

            OnLoaded();

            return true;
        }


        /// <summary>
        /// Unloads this instance.
        /// </summary>
        internal void Unload()
        {
            Debug.Assert(this.loadState == LoadState.Loaded);

            OnUnloading();

            this.texture.Dispose();
            this.texture = null;

            this.LoadState = LoadState.Unloaded;

#if DETECT_LEAKS
            var o = SharpDX.Diagnostics.ObjectTracker.FindActiveObjects();
            foreach (var ob in o)
            {
                if (ob.IsAlive)
                {
                    Texture t = ob.Object.Target as Texture;
                    if (t != null)
                    {
                        System.Diagnostics.Debug.Assert(t.DebugName != Name);
                    }
                }
            }
#endif
            //slowdown
            //Debug.WriteLine(string.Format("Texture {0} unloaded.", this.Name));
        }

        /// <summary>
        /// Loads the DDS texture.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="quality"></param>
        /// <returns></returns>
        protected abstract SharpDX.Direct3D9.BaseTexture LoadDDSTexture(string name, TextureQuality quality);

        protected abstract SharpDX.Direct3D9.BaseTexture LoadPNGTexture(string name);

        /// <summary>
        /// Request access to xna texture.
        /// </summary>
        /// <returns>true if data from texture can be readed.</returns>
        protected bool RequestAccess()
        {
            switch (this.LoadState)
            {
                case LoadState.Loaded:
                    if (IsValid)
                    {
                        return true;
                    }
                    else
                    {
                        using (MyTextureManager.TexturesLock.AcquireExclusiveUsing())
                        {
                            Unload();
                            return Load();
                        }
                    }

                case LoadState.Unloaded:
                case LoadState.LoadYourself:
                    using (MyTextureManager.TexturesLock.AcquireExclusiveUsing())
                    {
                        return Load();
                    }

                case LoadState.LoadYourselfBackground:
                    {
                        bool immediate = MyTextureManager.OverrideLoadingMode.HasValue && MyTextureManager.OverrideLoadingMode.Value == LoadingMode.Immediate;

                        if (immediate)
                        {
                            using (MyTextureManager.TexturesLock.AcquireExclusiveUsing())
                            {
                                return Load();
                            }
                        }
                        else
                        {
                            MyTextureManager.LoadTextureInBackground(this);
                            loadState = LoadState.Pending;
                            return false;
                        }
                    }

                case LoadState.Pending:
                case LoadState.Error:
                    return false;

                default:
                    throw new MyMwcExceptionApplicationShouldNotGetHere();
            }

        }

        /// <summary>
        /// Called when [loaded].
        /// </summary>
        protected virtual void OnLoaded()
        {
            if (this.TextureLoaded != null)
            {
                this.TextureLoaded(this);
            }

            //var textureManager = this.Manager as MyTextureManager;

            //TODO: This is incredibly slow, solve better
            //textureManager.DbgUpdateStats();
        }

        /// <summary>
        /// Called when [unloading].
        /// </summary>
        protected virtual void OnUnloading()
        {
            // var textureManager = this.Manager as MyTextureManager;

            //TODO: This is incredibly slow, solve better
            //textureManager.DbgUpdateStats();
        }

        #endregion

        /// <summary>
        /// Performs an implicit conversion from <see cref="MinerWars.AppCode.Game.Managers.Graphics.Buffers.MyVertexBuffer"/> to <see cref="MinerWarsMath.Graphics.VertexBuffer"/>.
        /// </summary>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator SharpDX.Direct3D9.BaseTexture(MyTexture right)
        {
            if (right == null)
            {
                return null;
            }

            right.RequestAccess();

            return right.texture;
        }
    }
}