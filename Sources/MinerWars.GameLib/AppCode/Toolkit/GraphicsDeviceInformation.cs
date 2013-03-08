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

using SharpDX.Direct3D;
using SharpDX.Direct3D9;
using SharpDX.Toolkit.Graphics;

namespace SharpDX.Toolkit
{
    public class  GraphicsDeviceInformation
    {
        #region Fields

        private GraphicsAdapter adapter;

        private FeatureLevel graphicsProfile;

        private PresentParameters presentationParameters;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphicsDeviceInformation" /> class.
        /// </summary>
        public GraphicsDeviceInformation()
        {
            Adapter = GraphicsAdapter.Default;
            PresentParameters p = new PresentParameters();
            p.InitDefaults();
            PresentationParameters = p;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the adapter.
        /// </summary>
        /// <value>The adapter.</value>
        /// <exception cref="System.ArgumentNullException">if value is null</exception>
        public GraphicsAdapter Adapter
        {
            get
            {
                return adapter;
            }

            set
            {
                adapter = value;
            }
        }

        /// <summary>
        /// Gets or sets the graphics profile.
        /// </summary>
        /// <value>The graphics profile.</value>
        /// <exception cref="System.ArgumentNullException">if value is null</exception>
        public FeatureLevel GraphicsProfile
        {
            get
            {
                return graphicsProfile;
            }

            set
            {
                graphicsProfile = value;
            }
        }

        /// <summary>
        /// Gets or sets the presentation parameters.
        /// </summary>
        /// <value>The presentation parameters.</value>
        /// <exception cref="System.ArgumentNullException">if value is null</exception>
        public PresentParameters PresentationParameters
        {
            get
            {
                return presentationParameters;
            }

            set
            {
                presentationParameters = value;
            }
        }

        /// <summary>
        /// Gets or sets the creation flags.
        /// </summary>
        /// <value>The creation flags.</value>
        public CreateFlags DeviceCreationFlags { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>Returns a value that indicates whether the current instance is equal to a specified object.</summary>
        /// <param name="obj">The Object to compare with the current GraphicsDeviceInformation.</param>
        public override bool Equals(object obj)
        {
            var information = obj as GraphicsDeviceInformation;
            if (information == null)
            {
                return false;
            }

            if (!Equals(information.adapter, adapter))
            {
                return false;
            }

            if (information.graphicsProfile != this.graphicsProfile)
            {
                return false;
            }

            return information.PresentationParameters.BackBufferWidth == this.PresentationParameters.BackBufferWidth 
                && information.PresentationParameters.BackBufferHeight == this.PresentationParameters.BackBufferHeight
                && information.PresentationParameters.BackBufferFormat == this.PresentationParameters.BackBufferFormat
                && information.PresentationParameters.AutoDepthStencilFormat == this.PresentationParameters.AutoDepthStencilFormat
                && information.PresentationParameters.MultiSampleQuality == this.PresentationParameters.MultiSampleQuality
                && information.PresentationParameters.FullScreenRefreshRateInHz == this.PresentationParameters.FullScreenRefreshRateInHz
                && information.PresentationParameters.PresentationInterval == this.PresentationParameters.PresentationInterval
                && information.PresentationParameters.DeviceWindowHandle == this.PresentationParameters.DeviceWindowHandle
                && information.PresentationParameters.Windowed == this.PresentationParameters.Windowed;
        }

        /// <summary>Gets the hash code for this object.</summary>
        public override int GetHashCode()
        {
            return graphicsProfile.GetHashCode()
                   ^ (adapter == null ? 0 : adapter.GetHashCode())
                   ^ presentationParameters.BackBufferWidth.GetHashCode()
                   ^ presentationParameters.BackBufferHeight.GetHashCode()
                   ^ presentationParameters.BackBufferFormat.GetHashCode()
                   ^ presentationParameters.AutoDepthStencilFormat.GetHashCode()
                   ^ presentationParameters.MultiSampleQuality.GetHashCode()
                   ^ presentationParameters.PresentationInterval.GetHashCode()
                   ^ presentationParameters.FullScreenRefreshRateInHz.GetHashCode()
                   ^ presentationParameters.DeviceWindowHandle.GetHashCode()
                   ^ presentationParameters.Windowed.GetHashCode();
        }


        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>A new copy-instance of this GraphicsDeviceInformation.</returns>
        public GraphicsDeviceInformation Clone()
        {
            return new GraphicsDeviceInformation 
            {
                Adapter = Adapter, 
                GraphicsProfile = GraphicsProfile, 
                PresentationParameters = this.PresentationParameters
            };
        }

        #endregion
    }
}