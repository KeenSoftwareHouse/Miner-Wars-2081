using System;
using System.Runtime.InteropServices;

using SharpDX.Direct3D9;

namespace SharpDX.Toolkit.Graphics
{
    /// <summary>
    /// Base class for all <see cref="GraphicsResource"/>.
    /// </summary>
    public abstract class GraphicsResource : Component
    {
        /// <summary>
        /// GraphicsDevice used to create this instance.
        /// </summary>
        public readonly Device GraphicsDevice;

        protected GraphicsResource(Device graphicsDevice)
        {
            if (graphicsDevice == null)
                throw new ArgumentNullException("graphicsDevice");
                 
            GraphicsDevice = graphicsDevice;
        }

        protected GraphicsResource(Device graphicsDevice, string name) : base(name)
        {
            if (graphicsDevice == null)
                throw new ArgumentNullException("graphicsDevice");

            GraphicsDevice = graphicsDevice;
        }

        protected override void Dispose(bool disposeManagedResources)
        {
            base.Dispose(disposeManagedResources);
        }

        /// <summary>
        /// Called when name changed for this component.
        /// </summary>
        protected override void OnNameChanged()
        {
            base.OnNameChanged();
        }

        protected static void UnPin(GCHandle[] handles)
        {
            if (handles != null)
            {
                for (int i = 0; i < handles.Length; i++)
                {
                    if (handles[i].IsAllocated)
                        handles[i].Free();
                }
            }
        }
    }
}