using System;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Effects;
using MinerWars.AppCode.Game.Utils.VertexFormats;
using MinerWars.AppCode.Game.Managers;

using SharpDX;
using SharpDX.Direct3D9;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;


namespace MinerWars.AppCode.Game.Utils
{
    using Vector2 = MinerWarsMath.Vector2;
    using Vector3 = MinerWarsMath.Vector3;
    using Vector4 = MinerWarsMath.Vector4;
    using Rectangle = MinerWarsMath.Rectangle;
    using Matrix = MinerWarsMath.Matrix;
    using Color = MinerWarsMath.Color;
    using BoundingBox = MinerWarsMath.BoundingBox;
    using BoundingSphere = MinerWarsMath.BoundingSphere;
    using BoundingFrustum = MinerWarsMath.BoundingFrustum;

    public class MyFullScreenQuad
    {
        VertexBuffer m_vertexBuffer;

        /// <summary>
        /// Gets the quad's vertex buffer
        /// </summary>
        public VertexBuffer VertexBuffer
        {
            get { return m_vertexBuffer; }
        }

        public void Dispose()
        {
            if (m_vertexBuffer != null) m_vertexBuffer.Dispose();
        }


        /// <summary>
        /// Creates an instance of FullScreenQuad
        /// </summary>
        /// <param name="graphicsDevice">The GraphicsDevice to use for creating resources</param>
        public MyFullScreenQuad()
        {
            CreateFullScreenQuad(MyMinerGame.Static.GraphicsDevice);
        }

        /// <summary>
        /// Draws the full screen quad
        /// </summary>
        /// <param name="graphicsDevice">The GraphicsDevice to use for rendering</param>
        public void Draw(MyEffectBase effect)
        {
            

            // Set the vertex buffer and declaration
            MyMinerGame.Static.GraphicsDevice.VertexDeclaration = MyVertexFormatFullScreenQuad.VertexDeclaration;
            MyMinerGame.Static.GraphicsDevice.SetStreamSource(0, m_vertexBuffer, 0, MyVertexFormatFullScreenQuad.Stride);

            effect.Begin();
            // Draw primitives
            MyMinerGame.Static.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);

            effect.End();

            MyPerformanceCounter.PerCameraDraw.TotalDrawCalls++;
        }


        /// <summary>
        /// Creates the VertexBuffer for the quad
        /// </summary>
        /// <param name="graphicsDevice">The GraphicsDevice to use</param>
        public void CreateFullScreenQuad(Device graphicsDevice)
        {
            // Create a vertex buffer for the quad, and fill it in
            m_vertexBuffer = new VertexBuffer(graphicsDevice, MyVertexFormatFullScreenQuad.Stride * 4, Usage.WriteOnly, VertexFormat.None, Pool.Default);
            m_vertexBuffer.DebugName = "FullScreenQuad";
            MyVertexFormatFullScreenQuad[] vbData = new MyVertexFormatFullScreenQuad[4];

            // Upper right
            vbData[0].Position = new Vector3(1, 1, 1);
            vbData[0].TexCoordAndCornerIndex = new Vector3(1, 0, 1);

            // Lower right
            vbData[1].Position = new Vector3(1, -1, 1);
            vbData[1].TexCoordAndCornerIndex = new Vector3(1, 1, 2);

            // Upper left
            vbData[2].Position = new Vector3(-1, 1, 1);
            vbData[2].TexCoordAndCornerIndex = new Vector3(0, 0, 0);

            // Lower left
            vbData[3].Position = new Vector3(-1, -1, 1);
            vbData[3].TexCoordAndCornerIndex = new Vector3(0, 1, 3);


            m_vertexBuffer.Lock(0, 0, LockFlags.None).WriteRange(vbData);
            m_vertexBuffer.Unlock();
        }

    }
}