using System;

namespace SharpDX.Toolkit
{
    /// <summary>
    /// Describes settings to apply before preparing a device for creation, used by <see cref="GraphicsDeviceManager.OnPreparingDeviceSettings"/>.
    /// </summary>
    public class PreparingDeviceSettingsEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PreparingDeviceSettingsEventArgs" /> class.
        /// </summary>
        /// <param name="graphicsDeviceInformation">The graphics device information.</param>
        public PreparingDeviceSettingsEventArgs(GraphicsDeviceInformation graphicsDeviceInformation)
        {
            GraphicsDeviceInformation = graphicsDeviceInformation;
        }

        /// <summary>
        /// Gets the graphics device information.
        /// </summary>
        /// <value>The graphics device information.</value>
        public GraphicsDeviceInformation GraphicsDeviceInformation { get; private set; }
    }
}