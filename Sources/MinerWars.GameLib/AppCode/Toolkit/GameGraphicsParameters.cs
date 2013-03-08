using SharpDX.Direct3D;
using SharpDX.Toolkit.Graphics;
using SharpDX.Direct3D9;

namespace SharpDX.Toolkit
{
    /// <summary>
    ///   Describess how data will be displayed to the screen.
    /// </summary>
    /// <msdn-id>bb173075</msdn-id>
    /// <unmanaged>DXGI_SWAP_CHAIN_DESC</unmanaged>
    /// <unmanaged-short>DXGI_SWAP_CHAIN_DESC</unmanaged-short>
    public class GameGraphicsParameters
    {
        public int PreferredVideoAdapter;

        /// <summary>
        ///   A value that describes the resolution width.
        /// </summary>
        /// <msdn-id>bb173075</msdn-id>
        /// <unmanaged>DXGI_MODE_DESC BufferDesc</unmanaged>
        /// <unmanaged-short>DXGI_MODE_DESC BufferDesc</unmanaged-short>
        public int PreferredBackBufferWidth;

        /// <summary>
        ///   A value that describes the resolution height.
        /// </summary>
        /// <msdn-id>bb173075</msdn-id>
        /// <unmanaged>DXGI_MODE_DESC BufferDesc</unmanaged>
        /// <unmanaged-short>DXGI_MODE_DESC BufferDesc</unmanaged-short>
        public int PreferredBackBufferHeight;

        /// <summary>
        ///   A <strong><see cref="SharpDX.DXGI.Format" /></strong> structure describing the display format.
        /// </summary>
        /// <msdn-id>bb173075</msdn-id>
        /// <unmanaged>DXGI_MODE_DESC BufferDesc</unmanaged>
        /// <unmanaged-short>DXGI_MODE_DESC BufferDesc</unmanaged-short>
        public Format PreferredBackBufferFormat;

        /// <summary>
        /// Gets or sets the depth stencil format
        /// </summary>
        public Format PreferredDepthStencilFormat;

        /// <summary>
        ///   Gets or sets a value indicating whether the application is in full screen mode.
        /// </summary>
        public bool IsFullScreen;

        /// <summary>
        /// Gets or sets the minimum graphics profile.
        /// </summary>
        public FeatureLevel[] PreferredGraphicsProfile;

        /// <summary>
        ///   Gets or sets a value indicating the number of sample locations during multisampling.
        /// </summary>
        public bool PreferMultiSampling;

        /// <summary>
        /// Gets or sets a value indicating whether to synochrnize present with vertical blanking.
        /// </summary>
        public bool SynchronizeWithVerticalRetrace;
    }
}