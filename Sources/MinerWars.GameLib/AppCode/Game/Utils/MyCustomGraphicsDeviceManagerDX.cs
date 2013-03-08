using System;
using System.Collections.Generic;
using System.Reflection;
//using MinerWarsMath;
//using MinerWarsMath.Graphics;
using MinerWars.AppCode.App;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using KeenSoftwareHouse.Library.Trace;


using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

namespace MinerWars.AppCode.Game.Utils
{
    public class MyCustomGraphicsDeviceManagerDX : GraphicsDeviceManager
    {
        /*
        private struct ResourceInfo
        {
            /// <summary>
            /// Gets or sets the resource.
            /// </summary>
            /// <value>
            /// The resource.
            /// </value>
            public GraphicsResource Resource { get; set; }

            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            /// <value>
            /// The name.
            /// </value>
            public string AllocatedAt { get; set; }

            /// <summary>
            /// Gets or sets the size.
            /// </summary>
            /// <value>
            /// The size.
            /// </value>
            public float Size { get; set; }
        }

        /// <summary>
        /// 
        /// </summary>
        private List<ResourceInfo> m_loadedResources = null;

        /// <summary>
        /// 
        /// </summary>
        private static uint m_loadedResourceCounter;

        /// <summary>
        /// 
        /// </summary>
        private readonly Timer m_sendTimer = new Timer(1000);
                */
        /// <summary>
        /// 
        /// </summary>
        private MyGraphicTest m_DXTest = null;

        /// <summary>
        /// Gets the size of the max texture.
        /// </summary>
        /// <value>
        /// The size of the max texture.
        /// </value>
        public int MaxTextureSize { get; private set; }

        /// <summary>
        /// Gets whether the device supports Rgba1010102 RTs and nonpow2 texture mipmapping.
        /// </summary>
        public bool HDRSupported { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MyCustomGraphicsDeviceManager"/> class.
        /// </summary>
        /// <param name="game">Game the GraphicsDeviceManager should be associated with.</param>
        public MyCustomGraphicsDeviceManagerDX(SharpDX.Toolkit.Game game)
            : base(game)
        {
        }
            /*
        /// <summary>
        /// Handles the ResourceCreated event of the GraphicsDevice control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MinerWarsMath.Graphics.ResourceCreatedEventArgs"/> instance containing the event data.</param>
        private override void OnResourceCreated(object sender, ResourceCreatedEventArgs e)
        {
            var resource = (GraphicsResource)e.Resource;
            float memory = 0;

            do
            {
                var vb = resource as VertexBuffer;
                if (vb != null)
                {
                    memory = ((float)(vb.VertexCount * vb.VertexDeclaration.VertexStride)) / (1000 * 1000);
                    
                    if (vb.BufferUsage == BufferUsage.WriteOnly)
                    {
                        memory *= 2;
                    }
                    
                    break;
                }

                var ib = resource as IndexBuffer;
                if (ib != null)
                {
                    memory += ((float)(ib.IndexCount * (ib.IndexElementSize == IndexElementSize.SixteenBits ? 2 : 4))) / (1000 * 1000);

                    if (ib.BufferUsage == BufferUsage.WriteOnly)
                    {
                        memory *= 2;
                    }

                    break;
                }

                return;

            } while (false);

            StackTrace st = new StackTrace(3);
            string allocatedAt = st.ToString();
            resource.Name = (++m_loadedResourceCounter).ToString();

            if (this.m_loadedResources == null)
                return;

            lock (m_loadedResources)
            {
                m_loadedResources.Add(new ResourceInfo { Resource = resource, AllocatedAt = allocatedAt, Size = memory });
            }

            //DbgSendLoadedResources();
            //DbgSendResourceMemory();
        }


        static private List<ResourceInfo> SortByValue(List<ResourceInfo> stats)
        {
            stats.Sort(
                delegate(ResourceInfo first,
                ResourceInfo next)
                {
                    return next.Size.CompareTo(first.Size);
                }
            );

            return stats;
        }        

        public void DebugDrawStatistics()
        {
            if (m_loadedResources == null)
            {
                MyDebugDraw.DrawText(new Vector2(100, 0), new System.Text.StringBuilder("Not available"), Color.Red, 1);
                return;
            }

            float totalMemory = 0;
            Vector2 offset = new Vector2(100, 0);
            foreach (ResourceInfo info in SortByValue(m_loadedResources))
            {
                totalMemory += info.Size;
            }
            MyDebugDraw.DrawText(offset, new System.Text.StringBuilder("Detailed resources statistics"), Color.Yellow, 1);
            offset.Y += 30;
            MyDebugDraw.DrawText(offset, new System.Text.StringBuilder("Total memory: "+ totalMemory.ToString()), Color.Yellow, 1);

            float scale = 0.7f;
            offset.Y += 30;
            foreach (ResourceInfo info in SortByValue(m_loadedResources))
            {
                MyDebugDraw.DrawText(offset, new System.Text.StringBuilder(info.Resource.ToString() + ": " + info.Size.ToString() + "MB"), Color.Yellow, scale);
                offset.Y += 20;
            }
        }

        /// <summary>
        /// Handles the ResourceDestroyed event of the GraphicsDevice control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MinerWarsMath.Graphics.ResourceDestroyedEventArgs"/> instance containing the event data.</param>
        private void OnResourceDestroyed(object sender, ResourceDestroyedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.Name))
            {
                return;
            }


            if (this.m_loadedResources != null)
            {
                lock (m_loadedResources)
                {
                    int index = m_loadedResources.FindIndex(resPair => resPair.Resource.Name == e.Name);

                    Debug.Assert(index != -1);

                    m_loadedResources.RemoveAt(index);
                }
            }

            DbgSendLoadedResources();
            DbgSendResourceMemory();
        }

        [Conditional("DEBUG")]
        public void DbgDumpLoadedResources(bool orderedBySize = false)
        {
            if (this.m_loadedResources == null)
                return;
            lock (this.m_loadedResources)
            {
                IEnumerable<ResourceInfo> dump;
                if (orderedBySize)
                {
                    dump = m_loadedResources.OrderByDescending(ri => ri.Size);
                }
                else
                {
                    dump = m_loadedResources;
                }

                foreach (var loadedResource in dump)
                {
                    Debug.WriteLine(loadedResource.AllocatedAt, string.Format("{0}: {1} size: {2}MB", loadedResource.Resource.GetType().Name, loadedResource.Resource.Name, loadedResource.Size));
                }
            }
        }

        /// <summary>
        /// DBGs the send loded resources.
        /// </summary>
        [Conditional("DEBUG")]
        private void DbgSendLoadedResources()
        {
            if (this.m_loadedResources == null)
                return;

            lock (this.m_loadedResources)
            {
                this.m_sendTimer.Stop();
                this.m_sendTimer.Start();
            }
        }

        /// <summary>
        /// DBGs the send resource memory.
        /// </summary>
        [Conditional("DEBUG")]
        private void DbgSendResourceMemory()
        {
            if (this.m_loadedResources == null)
                return;

            lock (this.m_loadedResources)
            {
                float totalMemory = m_loadedResources.Sum(loadedResource => loadedResource.Size);

                Watch.Send("Used memory (GPU resources)", totalMemory);
            }
        }  */

        public bool ChangeProfileSupport()
        {
            bool isGraphicsSupported = true;
            m_DXTest = new MyGraphicTest();
            isGraphicsSupported &= m_DXTest.TestDX();

            MaxTextureSize = m_DXTest.MaxTextureSize;
            HDRSupported = m_DXTest.Rgba1010102Supported && m_DXTest.MipmapNonPow2Supported;

            /*
            MethodBase method = typeof(GraphicsAdapter).GetMethod("IsProfileSupported", BindingFlags.Public | BindingFlags.Instance);
            MyReflectionMethod.ReplaceMethod(typeof(MyGraphicsAdapterTest).GetMethod("IsProfileSupported", BindingFlags.Public | BindingFlags.Instance), method);
            MethodBase destination = typeof(GraphicsAdapter).GetMethod("IsProfileSupported", BindingFlags.NonPublic | BindingFlags.Instance);
            MyReflectionMethod.ReplaceMethod(typeof(MyGraphicsAdapterTest).GetMethod("IsProfileSupported", BindingFlags.NonPublic | BindingFlags.Instance), destination);
              */
            return isGraphicsSupported;
        }
    }
}