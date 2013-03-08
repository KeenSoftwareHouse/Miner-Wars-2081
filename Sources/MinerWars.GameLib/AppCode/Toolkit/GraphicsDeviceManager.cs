// Copyright (c) 2010-2012 SharpDX - Alexandre Mutel
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.Generic;

using SharpDX.Direct3D;
using SharpDX.Direct3D9;
using SharpDX.Toolkit.Graphics;

namespace SharpDX.Toolkit
{
    /// <summary>
    /// Manages the <see cref="GraphicsDevice"/> lifecycle.
    /// </summary>
    public class GraphicsDeviceManager : Component
    {
        #region Fields

        /// <summary>
        /// Default width for the back buffer.
        /// </summary>
        public static readonly int DefaultBackBufferWidth = 800;

        /// <summary>
        /// Default height for the back buffer.
        /// </summary>
        public static readonly int DefaultBackBufferHeight = 480;

        private Game game;

        private bool deviceSettingsChanged;

        private FeatureLevel preferredGraphicsProfile;

        private bool isFullScreen;

        private bool preferMultiSampling;

        private Format preferredBackBufferFormat;

        private int preferredBackBufferHeight;

        private int preferredBackBufferWidth;

        private int preferredVideoAdapter;

        private Format preferredDepthStencilFormat;

        private bool synchronizeWithVerticalRetrace;

        private bool isChangingDevice;

        private int resizedBackBufferWidth;

        private int resizedBackBufferHeight;

        private bool isBackBufferToResize = false;

        private bool beginDrawOk;

        private bool isReallyFullScreen;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphicsDeviceManager" /> class.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <exception cref="System.ArgumentNullException">The game instance cannot be null.</exception>
        public GraphicsDeviceManager(Game game)
        {
            this.game = game;
            if (this.game == null)
            {
                throw new ArgumentNullException("game");
            }

            if (game.GraphicsManager != null)
            {
                game.GraphicsManager.Dispose();
            }

            game.GraphicsManager = this;

            // Defines all default values
            SynchronizeWithVerticalRetrace = true;
            PreferredBackBufferFormat = Format.A8R8G8B8;
            PreferredDepthStencilFormat = Format.D24S8;
            preferredBackBufferWidth = DefaultBackBufferWidth;
            preferredBackBufferHeight = DefaultBackBufferHeight;
            PreferMultiSampling = false;
            PreferredGraphicsProfile = new[]
                {
#if DIRECTX11_1
                    FeatureLevel.Level_11_1, 
#endif
                    FeatureLevel.Level_9_3, 
                    FeatureLevel.Level_9_2, 
                    FeatureLevel.Level_9_1, 
                };

            isFullScreen = game.Window.IsFullScreenMandatory;

            game.Window.ClientSizeChanged += Window_ClientSizeChanged;
        }

        #endregion

        #region Public Events

        public event EventHandler<EventArgs> DeviceCreated;

        public event EventHandler<EventArgs> DeviceDisposing;

        public event EventHandler<EventArgs> DeviceReset;

        public event EventHandler<EventArgs> DeviceResetting;

        public event EventHandler<PreparingDeviceSettingsEventArgs> PreparingDeviceSettings;

        #endregion

        #region Public Properties

        public Device GraphicsDevice { get; internal set; }

        public GraphicsAdapter GraphicsAdapter { get; internal set; }

        /// <summary>
        /// Gets or sets the list of graphics profile to select from the best feature to the lower feature. See remarks.
        /// </summary>
        /// <value>The graphics profile.</value>
        /// <remarks>
        /// By default, the PreferredGraphicsProfile is set to { <see cref="FeatureLevel.Level_11_1"/>, 
        /// <see cref="FeatureLevel.Level_11_0"/>,
        /// <see cref="FeatureLevel.Level_10_1"/>,
        /// <see cref="FeatureLevel.Level_10_0"/>,
        /// <see cref="FeatureLevel.Level_9_3"/>,
        /// <see cref="FeatureLevel.Level_9_2"/>,
        /// <see cref="FeatureLevel.Level_9_1"/>}
        /// </remarks>
        public FeatureLevel[] PreferredGraphicsProfile { get; set; }

        /// <summary>
        /// Sets the preferred graphics profile.
        /// </summary>
        /// <param name="levels">The levels.</param>
        /// <seealso cref="PreferredGraphicsProfile"/>
        public void SetPreferredGraphicsProfile(params FeatureLevel[] levels)
        {
            PreferredGraphicsProfile = levels;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is full screen.
        /// </summary>
        /// <value><c>true</c> if this instance is full screen; otherwise, <c>false</c>.</value>
        public bool IsFullScreen
        {
            get
            {
                return isFullScreen;
            }

            set
            {
                if (isFullScreen != value && !game.Window.IsFullScreenMandatory)
                {
                    isFullScreen = value;
                    deviceSettingsChanged = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [prefer multi sampling].
        /// </summary>
        /// <value><c>true</c> if [prefer multi sampling]; otherwise, <c>false</c>.</value>
        public bool PreferMultiSampling
        {
            get
            {
                return preferMultiSampling;
            }

            set
            {
                if (preferMultiSampling != value)
                {
                    preferMultiSampling = value;
                    deviceSettingsChanged = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets the device creation flags that will be used to create the <see cref="GraphicsDevice"/>
        /// </summary>
        /// <value>The device creation flags.</value>
        public CreateFlags DeviceCreationFlags { get; set; }


        public int PreferredVideoAdapter
        {
            get
            {
                return preferredVideoAdapter;
            }

            set
            {
                if (preferredVideoAdapter != value)
                {
                    preferredVideoAdapter = value;
                    deviceSettingsChanged = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets the preferred back buffer format.
        /// </summary>
        /// <value>The preferred back buffer format.</value>
        public Format PreferredBackBufferFormat
        {
            get
            {
                return preferredBackBufferFormat;
            }

            set
            {
                if (preferredBackBufferFormat != value)
                {
                    preferredBackBufferFormat = value;
                    deviceSettingsChanged = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets the height of the preferred back buffer.
        /// </summary>
        /// <value>The height of the preferred back buffer.</value>
        public int PreferredBackBufferHeight
        {
            get
            {
                return preferredBackBufferHeight;
            }

            set
            {
                if (preferredBackBufferHeight != value)
                {
                    preferredBackBufferHeight = value;
                    isBackBufferToResize = false;
                    deviceSettingsChanged = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets the width of the preferred back buffer.
        /// </summary>
        /// <value>The width of the preferred back buffer.</value>
        public int PreferredBackBufferWidth
        {
            get
            {
                return preferredBackBufferWidth;
            }

            set
            {
                if (preferredBackBufferWidth != value)
                {
                    preferredBackBufferWidth = value;
                    isBackBufferToResize = false;
                    deviceSettingsChanged = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets the preferred depth stencil format.
        /// </summary>
        /// <value>The preferred depth stencil format.</value>
        public Format PreferredDepthStencilFormat
        {
            get
            {
                return preferredDepthStencilFormat;
            }

            set
            {
                if (preferredDepthStencilFormat != value)
                {
                    preferredDepthStencilFormat = value;
                    deviceSettingsChanged = true;
                }
            }
        }

       

        /// <summary>
        /// Gets or sets a value indicating whether [synchronize with vertical retrace].
        /// </summary>
        /// <value><c>true</c> if [synchronize with vertical retrace]; otherwise, <c>false</c>.</value>
        public bool SynchronizeWithVerticalRetrace
        {
            get
            {
                return synchronizeWithVerticalRetrace;
            }
            set
            {
                if (synchronizeWithVerticalRetrace != value)
                {
                    synchronizeWithVerticalRetrace = value;
                    deviceSettingsChanged = true;
                }
            }
        }

        #endregion


        #region Public Methods and Operators

        /// <summary>
        /// Applies the changes from this instance and change or create the <see cref="GraphicsDevice"/> according to the new values.
        /// </summary>
        public void ApplyChanges()
        {
            if (GraphicsDevice == null || deviceSettingsChanged)
            {
                ChangeOrCreateDevice(false);
            }
        }

        public bool BeginDraw()
        {
            if (GraphicsDevice == null)
            {
                return false;
            }

            Result res = GraphicsDevice.TestCooperativeLevel();
            if (res.Code == ResultCode.DeviceLost.Result.Code)
            {
                Utilities.Sleep(TimeSpan.FromMilliseconds(20));
                return false;
            }

            if (res.Code == ResultCode.DeviceNotReset.Result.Code)
            {
                Utilities.Sleep(TimeSpan.FromMilliseconds(20));

                GraphicsAdapter.Initialize();

                try
                {
                    ChangeOrCreateDevice(false);
                }
                catch
                {
                    try
                    {
                        ChangeOrCreateDevice(true);
                    }
                    catch 
                    {
                        return false;
                    }
                }
            }

            /*
            switch (GraphicsDevice.GraphicsDeviceStatus)
            {
                case GraphicsDeviceStatus.Removed:
                    Utilities.Sleep(TimeSpan.FromMilliseconds(20));
                    return false;
                case GraphicsDeviceStatus.Reset:
                    Utilities.Sleep(TimeSpan.FromMilliseconds(20));
                    try
                    {
                        ChangeOrCreateDevice(false);
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                    catch
                    {
                        ChangeOrCreateDevice(true);
                    }

                    break;
                case GraphicsDeviceStatus.Normal:
                    // By default, we setup the render target to the back buffer, and the viewport as well.
                    if (GraphicsDevice.BackBuffer != null)
                    {
                        GraphicsDevice.SetRenderTargets(GraphicsDevice.DepthStencilBuffer, GraphicsDevice.BackBuffer);
                        GraphicsDevice.SetViewports(0, 0, GraphicsDevice.BackBuffer.Width, GraphicsDevice.BackBuffer.Height);
                    }

                    break;
            }       */

            GraphicsDevice.BeginScene();

            beginDrawOk = true;
            return true;
        }

        public void CreateDevice()
        {
            // Force the creation of the device
            ChangeOrCreateDevice(true);
        }

        public void EndDraw()
        {
            if (beginDrawOk && GraphicsDevice != null)
            {
                try
                {
                    GraphicsDevice.EndScene();
                    GraphicsDevice.Present();
                } 
                //catch (SharpDXException ex)
                catch
                {
                    // If this is not a DeviceRemoved or DeviceReset, than throw an exception
                    //if (ex.ResultCode != ResultCode.DeviceRemoved && ex.ResultCode != ResultCode.DeviceNotReset)
                    {
                      //  throw;
                    }
                }
            }
        }

        #endregion

     

        protected override void Dispose(bool disposeManagedResources)
        {
            if (disposeManagedResources)
            {
                if (game != null)
                {
                    game.Window.ClientSizeChanged -= Window_ClientSizeChanged;
                }

                if (GraphicsDevice != null)
                { 
                    GraphicsDevice.Dispose();
                    GraphicsDevice = null;
                }

                GraphicsAdapter.DisposeStatic();
            }

            base.Dispose(disposeManagedResources);
        }

        /// <summary>
        /// Determines whether this instance is compatible with the the specified new <see cref="GraphicsDeviceInformation"/>.
        /// </summary>
        /// <param name="newDeviceInfo">The new device info.</param>
        /// <returns><c>true</c> if this instance this instance is compatible with the the specified new <see cref="GraphicsDeviceInformation"/>; otherwise, <c>false</c>.</returns>
        protected virtual bool CanResetDevice(GraphicsDeviceInformation newDeviceInfo)
        {
            return GraphicsAdapter.AdapterOrdinal == newDeviceInfo.Adapter.AdapterOrdinal;
            // By default, a reset is compatible when we stay under the same graphics profile.
            //return GraphicsDevice.Features.Level == newDeviceInfo.GraphicsProfile;
            //return true;
        }

        /// <summary>
        /// Finds the best device that is compatible with the preferences defined in this instance.
        /// </summary>
        /// <param name="anySuitableDevice">if set to <c>true</c> a device can be selected from any existing adapters, otherwise, it will select only from default adapter.</param>
        /// <returns>The graphics device information.</returns>
        protected virtual GraphicsDeviceInformation FindBestDevice(bool anySuitableDevice)
        {        
            // Setup preferred parameters before passing them to the factory
            var preferredParameters = new GameGraphicsParameters
                {
                    PreferredVideoAdapter = PreferredVideoAdapter,
                    PreferredBackBufferWidth = PreferredBackBufferWidth,
                    PreferredBackBufferHeight = PreferredBackBufferHeight,
                    PreferredBackBufferFormat = PreferredBackBufferFormat,
                    PreferredDepthStencilFormat = PreferredDepthStencilFormat,
                    IsFullScreen = IsFullScreen,
                    PreferMultiSampling = PreferMultiSampling,
                    SynchronizeWithVerticalRetrace = SynchronizeWithVerticalRetrace,
                    PreferredGraphicsProfile = (FeatureLevel[])PreferredGraphicsProfile.Clone(),
                };

            // Setup resized value if there is a resize pending
            if (!IsFullScreen && isBackBufferToResize)
            {
                preferredParameters.PreferredBackBufferWidth = resizedBackBufferWidth;
                preferredParameters.PreferredBackBufferHeight = resizedBackBufferHeight;
            }

            var devices = game.GamePlatform.FindBestDevices(preferredParameters);
            if (devices.Count == 0)
            {
                throw new InvalidOperationException("No screen modes found");
            }      

            return devices[0];
        }

        /// <summary>
        /// Ranks a list of <see cref="GraphicsDeviceInformation"/> before creating a new device.
        /// </summary>
        /// <param name="foundDevices">The list of devices that can be reorder.</param>
        protected virtual void RankDevices(List<GraphicsDeviceInformation> foundDevices)
        {
            // Don't sort if there is a single device (mostly for XAML/WP8)
            if (foundDevices.Count == 1)
            {
                return;
            }

        }
             /*
        private int CalculateRankForFormat(DXGI.Format format)
        {
            if (format == PreferredBackBufferFormat)
            {
                return 0;
            }

            if (CalculateFormatSize(format) == CalculateFormatSize(PreferredBackBufferFormat))
            {
                return 1;
            }

            return int.MaxValue;
        }
               */
/*        private int CalculateFormatSize(DXGI.Format format)
        {
            switch (format)
            {
                case DXGI.Format.R8G8B8A8_UNorm:
                case DXGI.Format.R8G8B8A8_UNorm_SRgb:
                case DXGI.Format.B8G8R8A8_UNorm:
                case DXGI.Format.B8G8R8A8_UNorm_SRgb:
                case DXGI.Format.R10G10B10A2_UNorm:
                    return 32;

                case DXGI.Format.B5G6R5_UNorm:
                case DXGI.Format.B5G5R5A1_UNorm:
                    return 16;
            }

            return 0;
        }
  */
        protected virtual void OnDeviceCreated(object sender, EventArgs args)
        {
            var handler = DeviceCreated;
            if (handler != null)
            {
                handler(sender, args);
            }
        }

        protected virtual void OnDeviceDisposing(object sender, EventArgs args)
        {
            var handler = DeviceDisposing;
            if (handler != null)
            {
                handler(sender, args);
            }
        }
        
        protected virtual void OnDeviceReset(object sender, EventArgs args)
        {
            var handler = DeviceReset;
            if (handler != null)
            {
                handler(sender, args);
            }
        }
        
        protected virtual void OnDeviceResetting(object sender, EventArgs args)
        {
            var handler = DeviceResetting;
            if (handler != null)
            {
                handler(sender, args);
            }
        }
        
        protected virtual void OnPreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs args)
        {
            var handler = PreparingDeviceSettings;
            if (handler != null)
            {
                handler(sender, args);
            }
        }

        private void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            /*
            if (!isChangingDevice && ((game.Window.ClientBounds.Height != 0) || (game.Window.ClientBounds.Width != 0)))
            {
                resizedBackBufferWidth = game.Window.ClientBounds.Width;
                resizedBackBufferHeight = game.Window.ClientBounds.Height;
                isBackBufferToResize = true;
                ChangeOrCreateDevice(false);
            }*/
        }



        private void CreateDevice(GraphicsDeviceInformation newInfo)
        {
            if (GraphicsDevice != null)
            {
                try
                {
                    GraphicsDevice.Dispose();
                }
                catch
                {
                }
                GraphicsDevice = null;
            }

     //       newInfo.PresentationParameters.Windowed = !isFullScreen;
      //      newInfo.PresentationParameters.PresentationInterval = SynchronizeWithVerticalRetrace ? PresentInterval.One : PresentInterval.Immediate;
            newInfo.DeviceCreationFlags = DeviceCreationFlags;

            OnPreparingDeviceSettings(this, new PreparingDeviceSettingsEventArgs(newInfo));

            // this.ValidateGraphicsDeviceInformation(newInfo);
            GraphicsDevice = game.GamePlatform.CreateDevice(newInfo);

            GraphicsAdapter = newInfo.Adapter;

            GraphicsDevice.Viewport = new Viewport(0, 0, newInfo.PresentationParameters.BackBufferWidth, newInfo.PresentationParameters.BackBufferHeight);

            /*
            GraphicsDevice.DeviceResetting += GraphicsDevice_DeviceResetting;
            GraphicsDevice.DeviceReset += GraphicsDevice_DeviceReset;
            GraphicsDevice.DeviceLost += GraphicsDevice_DeviceLost;   */
            GraphicsDevice.Disposing += GraphicsDevice_Disposing;

            OnDeviceCreated(this, EventArgs.Empty);
        }

        void GraphicsDevice_DeviceResetting(object sender, EventArgs e)
        {
            // TODO what to do?
        }

        void GraphicsDevice_DeviceReset(object sender, EventArgs e)
        {
            // TODO what to do?
        }

        void GraphicsDevice_DeviceLost(object sender, EventArgs e)
        {
            // TODO what to do?
        }

        void GraphicsDevice_Disposing(object sender, EventArgs e)
        {
            OnDeviceDisposing(sender, e);
        }

        private void ChangeOrCreateDevice(bool forceCreate)
        {
            isChangingDevice = true;
            int width = game.Window.ClientBounds.Width;
            int height = game.Window.ClientBounds.Height;

            bool loadContent = false;
            if (game.ContentLoaded)
            {
                game.UnloadContent();
                loadContent = true;
            }
                
            bool isBeginScreenDeviceChange = false;
            try
            {
            
                var graphicsDeviceInformation = FindBestDevice(forceCreate);
                game.Window.BeginScreenDeviceChange(!graphicsDeviceInformation.PresentationParameters.Windowed);              

                isBeginScreenDeviceChange = true;
                bool needToCreateNewDevice = true;

                game.Window.UpdateFullscreen(!graphicsDeviceInformation.PresentationParameters.Windowed);
                game.Window.NativeWindow.ClientSize = new System.Drawing.Size(graphicsDeviceInformation.PresentationParameters.BackBufferWidth, graphicsDeviceInformation.PresentationParameters.BackBufferHeight);

                // If we are not forced to create a new device and this is already an existing GraphicsDevice
                // try to reset and resize it.
                if (!forceCreate && GraphicsDevice != null)
                {
                    OnPreparingDeviceSettings(this, new PreparingDeviceSettingsEventArgs(graphicsDeviceInformation));

                    
                    if (CanResetDevice(graphicsDeviceInformation))
                    {
                        try
                        {
                            var newWidth = graphicsDeviceInformation.PresentationParameters.BackBufferWidth;
                            var newHeight = graphicsDeviceInformation.PresentationParameters.BackBufferHeight;
                            var newFormat = graphicsDeviceInformation.PresentationParameters.BackBufferFormat;

                            //string s = SharpDX.Diagnostics.ObjectTracker.ReportActiveObjects();

                            GraphicsDevice.Reset(graphicsDeviceInformation.PresentationParameters);
                            GraphicsDevice.Viewport = new Viewport(0, 0, newWidth, newHeight);

                            needToCreateNewDevice = false;
                        }
                        catch
                        {
                        }
                    }
                }

                // If we still need to create a device, then we need to create it
                if (needToCreateNewDevice)
                {
                    CreateDevice(graphicsDeviceInformation);
                }

                deviceSettingsChanged = false;
            }
            finally
            {
                if (isBeginScreenDeviceChange)
                {
                    game.Window.EndScreenDeviceChange(width, height);
                }

                if (loadContent)
                    game.LoadContent();
               
                isChangingDevice = false;
            }
        }
    }
}