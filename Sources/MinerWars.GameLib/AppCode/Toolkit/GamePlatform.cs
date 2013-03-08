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
using SharpDX.Windows;

namespace SharpDX.Toolkit
{
    public abstract class GamePlatform
    {
        protected GamePlatform()
        {
        }

        public static GamePlatform Create()
        {
#if WIN8METRO
            return new GamePlatformWinRT(services);
#elif WP8
            return new GamePlatformWP8(services);
#else
            return new GamePlatformDesktop();
#endif
        }

        public abstract string DefaultAppDirectory { get; }

        public object WindowContext { get; set; }

        public event EventHandler<EventArgs> Activated;

        public event EventHandler<EventArgs> Deactivated;

        public event EventHandler<EventArgs> Exiting;

        public event EventHandler<EventArgs> Idle;

        public event EventHandler<EventArgs> Resume;

        public event EventHandler<EventArgs> Suspend;

        public abstract GameWindow MainWindow { get; }

        public abstract GameWindow CreateWindow(object windowContext = null, int width = 0, int height = 0);

        public bool IsBlockingRun { get; protected set; }

        public abstract void Run(object windowContext, VoidAction initCallback, VoidAction tickCallback);

        public virtual void Exit()
        {
            Activated = null;
            Deactivated = null;
            Exiting = null;
            Idle = null;
            Resume = null;
            Suspend = null;
        }
        
        protected void OnActivated(object source, EventArgs e)
        {
            EventHandler<EventArgs> handler = Activated;
            if (handler != null) handler(this, e);
        }

        protected void OnDeactivated(object source, EventArgs e)
        {
            EventHandler<EventArgs> handler = Deactivated;
            if (handler != null) handler(this, e);
        }

        protected void OnExiting(object source, EventArgs e)
        {
            EventHandler<EventArgs> handler = Exiting;
            if (handler != null) handler(this, e);
        }

        protected void OnIdle(object source, EventArgs e)
        {
            EventHandler<EventArgs> handler = Idle;
            if (handler != null) handler(this, e);
        }

        protected void OnResume(object source, EventArgs e)
        {
            EventHandler<EventArgs> handler = Resume;
            if (handler != null) handler(this, e);
        }

        protected void OnSuspend(object source, EventArgs e)
        {
            EventHandler<EventArgs> handler = Suspend;
            if (handler != null) handler(this, e);
        }

        protected void AddDevice(GraphicsAdapter graphicsAdapter, DisplayMode mode,  GraphicsDeviceInformation deviceBaseInfo, GameGraphicsParameters prefferedParameters, List<GraphicsDeviceInformation> graphicsDeviceInfos)
        {
            var deviceInfo = deviceBaseInfo.Clone();

            PresentParameters p = new PresentParameters();
            p.InitDefaults();

            p.FullScreenRefreshRateInHz = mode.RefreshRate;

            if (prefferedParameters.IsFullScreen)
            {
                p.BackBufferWidth = prefferedParameters.PreferredBackBufferWidth;
                p.BackBufferHeight = prefferedParameters.PreferredBackBufferHeight;
                p.Windowed = false;
            }
            else
            {
                p.BackBufferWidth = prefferedParameters.PreferredBackBufferWidth;
                p.BackBufferHeight = prefferedParameters.PreferredBackBufferHeight;
                p.Windowed = true;
                p.FullScreenRefreshRateInHz = 0;
            }

            p.DeviceWindowHandle = MainWindow.NativeWindow.Handle;
            p.AutoDepthStencilFormat = prefferedParameters.PreferredDepthStencilFormat;
            p.MultiSampleQuality = 0;
            p.PresentationInterval = prefferedParameters.SynchronizeWithVerticalRetrace ? PresentInterval.One : PresentInterval.Immediate;
            p.SwapEffect = SwapEffect.Discard;
            p.PresentFlags = PresentFlags.Video;            

            deviceInfo.PresentationParameters = p;
            deviceInfo.Adapter = GraphicsAdapter.Adapters[prefferedParameters.PreferredVideoAdapter];

            if (!graphicsDeviceInfos.Contains(deviceInfo))
            {
                graphicsDeviceInfos.Add(deviceInfo);
            }
        }

        public virtual List<GraphicsDeviceInformation> FindBestDevices(GameGraphicsParameters prefferedParameters)
        {
            var graphicsDeviceInfos = new List<GraphicsDeviceInformation>();

            // Iterate on each adapter
            foreach (var graphicsAdapter in GraphicsAdapter.Adapters)
            {
                // Iterate on each preferred graphics profile
                foreach (var featureLevel in prefferedParameters.PreferredGraphicsProfile)
                {
                    // Check if this profile is supported.
                    if (graphicsAdapter.IsProfileSupported(featureLevel))
                    {
                        var deviceInfo = new GraphicsDeviceInformation
                        {
                            Adapter = graphicsAdapter,
                            GraphicsProfile = featureLevel,
                            PresentationParameters = new PresentParameters()
                            {
                                MultiSampleQuality = 0,
                                Windowed = !prefferedParameters.IsFullScreen,
                                PresentationInterval = prefferedParameters.SynchronizeWithVerticalRetrace ? PresentInterval.One : PresentInterval.Immediate,
                                DeviceWindowHandle = MainWindow.NativeWindow.Handle,
                            }
                        };

                        if (graphicsAdapter.CurrentDisplayMode.Format != Format.Unknown)
                        {
                            AddDevice(graphicsAdapter, graphicsAdapter.CurrentDisplayMode, deviceInfo, prefferedParameters, graphicsDeviceInfos);
                        }

                        if (prefferedParameters.IsFullScreen)
                        {
                            // Get display mode for the particular width, height, pixelformat
                            foreach (var displayMode in graphicsAdapter.SupportedDisplayModes)
                            {
                                AddDevice(graphicsAdapter, displayMode, deviceInfo, prefferedParameters, graphicsDeviceInfos);
                            }
                        }

                        // If the profile is supported, we are just using the first best one
                        break;
                    }
                }
            }

            return graphicsDeviceInfos;
        }

        public virtual SharpDX.Direct3D9.Device CreateDevice(GraphicsDeviceInformation deviceInformation)
        {
            //var device = 
            
                //GraphicsDevice.New(deviceInformation.Adapter, deviceInformation.DeviceCreationFlags);

            // Create Device
            //SharpDX.Direct3D9.Direct3D direct3D = new SharpDX.Direct3D9.Direct3D();

            PresentParameters p = deviceInformation.PresentationParameters;
            p.DeviceWindowHandle = MainWindow.NativeWindow.Handle;

            deviceInformation.PresentationParameters = p;

            SharpDX.Direct3D9.Device device = null;
            //try
            {
                device = new SharpDX.Direct3D9.Device(GraphicsAdapter.D3D, deviceInformation.Adapter.AdapterOrdinal, DeviceType.Hardware, p.DeviceWindowHandle, CreateFlags.HardwareVertexProcessing | CreateFlags.Multithreaded, deviceInformation.PresentationParameters);
            }
            /*catch
            {
            } */

            //device.Presenter = new SwapChainGraphicsPresenter(device, deviceInformation.PresentationParameters);
              
            return device;
        }
    }
}