using System;
using System.Collections.Generic;

using SharpDX.Direct3D;
using SharpDX.Direct3D9;

namespace SharpDX.Toolkit.Graphics
{
    /// <summary>
    /// Provides methods to retrieve and manipulate graphics adapters. This is the equivalent to <see cref="Adapter1"/>.
    /// </summary>
    /// <msdn-id>ff471329</msdn-id>	
    /// <unmanaged>IDXGIAdapter1</unmanaged>	
    /// <unmanaged-short>IDXGIAdapter1</unmanaged-short>	
    public class GraphicsAdapter : Component
    {
        private static DisposeCollector staticCollector;
        private readonly int adapterOrdinal;
        public int AdapterOrdinal { get { return adapterOrdinal; } }

        public static SharpDX.Direct3D9.Direct3D D3D;

        /// <summary>
        /// Default PixelFormat used.
        /// </summary>
        public SharpDX.Direct3D9.Format DefaultFormat = SharpDX.Direct3D9.Format.A8R8G8B8;


        AdapterDetails m_adapterDetails;
        public AdapterDetails AdapterDetails { get { return m_adapterDetails; } }

        /// <summary>
        /// Initializes static members of the <see cref="GraphicsAdapter" /> class.
        /// </summary>
        static GraphicsAdapter()
        {
#if DIRECTX11_1
            using (var factory = new Factory1())
                Initialize(factory.QueryInterface<Factory2>());
#else
            Initialize();
#endif
        }

        /// <summary>
        /// Initializes all adapters with the specified factory.
        /// </summary>
        /// <param name="factory1">The factory1.</param>
        internal static void Initialize()
        {
            if (staticCollector != null)
            {
                staticCollector.Dispose();
            }
            else
                staticCollector = new DisposeCollector();

            if (D3D != null)
            {
                D3D.Dispose();
                D3D = null;
            }

            D3D = new Direct3D9.Direct3D();

            var adapters = new List<GraphicsAdapter>();
            for (int i = 0; i < D3D.AdapterCount; i++)
            {
                var adapter = new GraphicsAdapter(i);
                staticCollector.Collect(adapter);
                adapters.Add(adapter);
            }

            Default = adapters[0];
            Adapters = adapters.ToArray();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphicsAdapter" /> class.
        /// </summary>
        /// <param name="adapterOrdinal">The adapter ordinal.</param>
        private GraphicsAdapter(int adapterOrdinal)
        {
            this.adapterOrdinal = adapterOrdinal;

            SupportedDisplayModes = GetSupportedDisplayModes();
                               
            CurrentDisplayMode = D3D.GetAdapterDisplayMode(adapterOrdinal);

            m_adapterDetails = D3D.GetAdapterIdentifier(adapterOrdinal);
            this.Name = m_adapterDetails.Description + " (" + m_adapterDetails.DeviceName.Replace("\\","").Replace(".","") + ")";
        }

        /// <summary>
        /// Return the description of this adapter
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return base.ToString();
        }

        /// <summary>
        /// Tests to see if the adapter supports the requested profile.
        /// </summary>
        /// <param name="featureLevel">The graphics profile.</param>
        /// <returns>true if the profile is supported</returns>
        public bool IsProfileSupported(FeatureLevel featureLevel)
        {
            return featureLevel == FeatureLevel.Level_9_3;
        }

        /// <summary>
        /// Gets the current display mode.
        /// </summary>
        /// <value>The current display mode.</value>
        public DisplayMode CurrentDisplayMode { get; private set; }

        /// <summary>
        /// Collection of available adapters on the system.
        /// </summary>
        public static GraphicsAdapter[] Adapters { get; private set;}

        /// <summary>
        /// Gets the default adapter.
        /// </summary>
        public static GraphicsAdapter Default { get; private set; }

        /// <summary>
        /// Returns a collection of supported display modes for the current adapter.
        /// </summary>
        public DisplayMode[] SupportedDisplayModes { get; private set; }

        /// <summary>
        /// Retrieves bounds of the desktop coordinates.
        /// </summary>
        /// <msdn-id>bb173068</msdn-id>	
        /// <unmanaged>RECT DesktopCoordinates</unmanaged>	
        /// <unmanaged-short>RECT DesktopCoordinates</unmanaged-short>	
        public Rectangle DesktopBounds { get { return new Rectangle(0, 0, CurrentDisplayMode.Width, CurrentDisplayMode.Height); } }

        /// <summary>
        /// Determines if this instance of GraphicsAdapter is the default adapter.
        /// </summary>
        public bool IsDefaultAdapter { get { return adapterOrdinal == 0; } }

        /// <summary>
        /// Retrieves the handle of the monitor associated with the Microsoft Direct3D object.
        /// </summary>
        /// <msdn-id>bb173068</msdn-id>	
        /// <unmanaged>HMONITOR Monitor</unmanaged>	
        /// <unmanaged-short>HMONITOR Monitor</unmanaged-short>	
        public IntPtr MonitorHandle { get { return Adapters[adapterOrdinal].MonitorHandle; } }

        /// <summary>
        /// Disposes of all objects
        /// </summary>
        internal static void DisposeStatic()
        {
            ((IDisposable)staticCollector).Dispose();

            if (D3D != null)
            {
                D3D.Dispose();
                D3D = null;
            }
        }

        /// <summary>
        /// Returns a collection of supported display modes for a particular Format.
        /// </summary>
        /// <returns>a read-only collection of display modes</returns>
        private DisplayMode[] GetSupportedDisplayModes()
        {
            var modeAvailable = new List<DisplayMode>();
            var modeMap = new Dictionary<string, DisplayMode>();

            try
            {
                Direct3D9.Format format = Format.X8R8G8B8;
                
                int modeCount = format == Format.Unknown ? 0 : D3D.GetAdapterModeCount(adapterOrdinal, format);
                for (int modeIndex = 0; modeIndex < modeCount; modeIndex++)
                {
                    DisplayMode mode = D3D.EnumAdapterModes(adapterOrdinal, (Direct3D9.Format)format, modeIndex);

                    string key = format + ";" + mode.Width + ";" + mode.Height + ";" + mode.RefreshRate;

                    DisplayMode oldMode;
                    if (!modeMap.TryGetValue(key, out oldMode))
                    {
                        var displayMode = new DisplayMode()
                        {
                            Format = mode.Format,
                            Width = mode.Width,
                            Height = mode.Height,
                            RefreshRate = mode.RefreshRate
                        };

                        modeMap.Add(key, displayMode);
                        modeAvailable.Add(displayMode);
                    }
                }                
            }
            catch (SharpDXException dxgiException)
            {
                if (dxgiException.ResultCode != ResultCode.NotAvailable)
                {
                    throw;
                }
            }
            return modeAvailable.ToArray();
        }
    }
}