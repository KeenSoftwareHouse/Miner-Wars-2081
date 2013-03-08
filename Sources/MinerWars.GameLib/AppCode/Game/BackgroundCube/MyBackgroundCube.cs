using System.IO;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Managers;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Utils.VertexFormats;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.CommonLIB.AppCode.Utils;
using SysUtils;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.Render;
using MinerWars.AppCode.Game.Effects;
using MinerWars.AppCode.Game.World;

using SharpDX.Direct3D9;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

namespace MinerWars.AppCode.Game.BackgroundCube
{
    using Vector2 = MinerWarsMath.Vector2;
    using Vector3 = MinerWarsMath.Vector3;
    using Vector4 = MinerWarsMath.Vector4;
    using Rectangle = MinerWarsMath.Rectangle;
    using Matrix = MinerWarsMath.Matrix;
    using Quaternion = MinerWarsMath.Quaternion;
    using Color = MinerWarsMath.Color;
    using BoundingBox = MinerWarsMath.BoundingBox;
    using BoundingSphere = MinerWarsMath.BoundingSphere;
    using BoundingFrustum = MinerWarsMath.BoundingFrustum;

    static class MyBackgroundCube
    {
        static MyTextureCube m_textureCube;        
        static VertexBuffer m_boxVertexBuffer;
        static bool m_loaded = false;
        static MyMwcSectorIdentifier m_sectorIdentifier;
        const int BOX_TRIANGLES_COUNT = 12;
        static Matrix m_backgroundProjectionMatrix;

        static MyBackgroundCube()
        {
            Render.MyRender.RegisterRenderModule(MyRenderModuleEnum.BackgroundCube, "Background cube", Draw, Render.MyRenderStage.Background, 1, true);
        }

        public static void LoadContent(MyMwcSectorIdentifier sectorIdentifier)
        {
            MyMwcLog.WriteLine("MyBackgroundCube.LoadContent() - START");
            MyMwcLog.IncreaseIndent();
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyBackgroundCube");

            m_sectorIdentifier = sectorIdentifier;

            UpdateTexture();
         
            m_loaded = false;

            //  Projection matrix according to zoom level
            m_backgroundProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(1.0f, MyCamera.ForwardAspectRatio,
                5,
                100000);

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyBackgroundCube.LoadContent() - END");
        }

        static string GetFilename()
        {
            return MySector.BackgroundTexture;
        }

        public static void ReloadContent()
        {
            UpdateTexture();
        }

        static void UpdateTexture()
        {     
            //  This texture should be in DDS file extension and must be DXT1 compressed (use Photoshop and DDS tool from NVIDIA)
            //  We don't use for it dxt compression from XNA's content processor because we don't want huge (over 100 Mb) files in SVN.
            m_textureCube = MyTextureManager.GetTexture<MyTextureCube>("Textures\\BackgroundCube\\Final\\" + GetFilename(), null, LoadingMode.Immediate);
        }

        public static void UnloadContent()
        {
            MyMwcLog.WriteLine("MyBackgroundCube.UnloadContent - START");
            MyMwcLog.IncreaseIndent();

            if (m_boxVertexBuffer != null)
            {
                m_boxVertexBuffer.Dispose();
                m_boxVertexBuffer = null;
            }

            if (m_textureCube != null)
            {
                MyTextureManager.UnloadTexture(m_textureCube);
                m_textureCube = null;
            }
            m_textureCube = null;
            m_loaded = false;

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyBackgroundCube.UnloadContent - END");
        }

        //	Special method that loads data into GPU, and can be called only from Draw method, never from LoadContent or from background thread.
        //	Because that would lead to empty vertex/index buffers if they are filled/created while game is minimized (remember the issue - alt-tab during loading screen)
        static void LoadInDraw()
        {
            if (m_loaded) return;

            //  In fact it doesn't matter how large is cube, it will always look same as we are always in its middle
            //  I changed it from 1.0 to 100.0 only because will small length I had problems with near frustum plane and crazy aspect ratios.
            const float CUBE_LENGTH_HALF = 100;

            Vector3 shapeSize = Vector3.One * CUBE_LENGTH_HALF;
            Vector3 shapePosition = Vector3.Zero;

            MyVertexFormatPositionTexture3[] boxVertices = new MyVertexFormatPositionTexture3[36];

            Vector3 topLeftFront = shapePosition + new Vector3(-1.0f, 1.0f, -1.0f) * shapeSize;
            Vector3 bottomLeftFront = shapePosition + new Vector3(-1.0f, -1.0f, -1.0f) * shapeSize;
            Vector3 topRightFront = shapePosition + new Vector3(1.0f, 1.0f, -1.0f) * shapeSize;
            Vector3 bottomRightFront = shapePosition + new Vector3(1.0f, -1.0f, -1.0f) * shapeSize;
            Vector3 topLeftBack = shapePosition + new Vector3(-1.0f, 1.0f, 1.0f) * shapeSize;
            Vector3 topRightBack = shapePosition + new Vector3(1.0f, 1.0f, 1.0f) * shapeSize;
            Vector3 bottomLeftBack = shapePosition + new Vector3(-1.0f, -1.0f, 1.0f) * shapeSize;
            Vector3 bottomRightBack = shapePosition + new Vector3(1.0f, -1.0f, 1.0f) * shapeSize;

            Vector3 textureTopLeftFront = MyMwcUtils.Normalize(topLeftFront);
            Vector3 textureBottomLeftFront = MyMwcUtils.Normalize(bottomLeftFront);
            Vector3 textureTopRightFront = MyMwcUtils.Normalize(topRightFront);
            Vector3 textureBottomRightFront = MyMwcUtils.Normalize(bottomRightFront);
            Vector3 textureTopLeftBack = MyMwcUtils.Normalize(topLeftBack);
            Vector3 textureTopRightBack = MyMwcUtils.Normalize(topRightBack);
            Vector3 textureBottomLeftBack = MyMwcUtils.Normalize(bottomLeftBack);
            Vector3 textureBottomRightBack = MyMwcUtils.Normalize(bottomRightBack);
            textureTopLeftFront.Z *= -1;
            textureBottomLeftFront.Z *= -1;
            textureTopRightFront.Z *= -1;
            textureBottomRightFront.Z *= -1;
            textureTopLeftBack.Z *= -1;
            textureTopRightBack.Z *= -1;
            textureBottomLeftBack.Z *= -1;
            textureBottomRightBack.Z *= -1;

            // Front face.
            boxVertices[0] = new MyVertexFormatPositionTexture3(topLeftFront, textureTopLeftFront);
            boxVertices[1] = new MyVertexFormatPositionTexture3(bottomLeftFront, textureBottomLeftFront);
            boxVertices[2] = new MyVertexFormatPositionTexture3(topRightFront, textureTopRightFront);
            boxVertices[3] = new MyVertexFormatPositionTexture3(bottomLeftFront, textureBottomLeftFront);
            boxVertices[4] = new MyVertexFormatPositionTexture3(bottomRightFront, textureBottomRightFront);
            boxVertices[5] = new MyVertexFormatPositionTexture3(topRightFront, textureTopRightFront);

            // Back face.
            boxVertices[6] = new MyVertexFormatPositionTexture3(topLeftBack, textureTopLeftBack);
            boxVertices[7] = new MyVertexFormatPositionTexture3(topRightBack, textureTopRightBack);
            boxVertices[8] = new MyVertexFormatPositionTexture3(bottomLeftBack, textureBottomLeftBack);
            boxVertices[9] = new MyVertexFormatPositionTexture3(bottomLeftBack, textureBottomLeftBack);
            boxVertices[10] = new MyVertexFormatPositionTexture3(topRightBack, textureTopRightBack);
            boxVertices[11] = new MyVertexFormatPositionTexture3(bottomRightBack, textureBottomRightBack);

            // Top face.
            boxVertices[12] = new MyVertexFormatPositionTexture3(topLeftFront, textureTopLeftFront);
            boxVertices[13] = new MyVertexFormatPositionTexture3(topRightBack, textureTopRightBack);
            boxVertices[14] = new MyVertexFormatPositionTexture3(topLeftBack, textureTopLeftBack);
            boxVertices[15] = new MyVertexFormatPositionTexture3(topLeftFront, textureTopLeftFront);
            boxVertices[16] = new MyVertexFormatPositionTexture3(topRightFront, textureTopRightFront);
            boxVertices[17] = new MyVertexFormatPositionTexture3(topRightBack, textureTopRightBack);

            // Bottom face.
            boxVertices[18] = new MyVertexFormatPositionTexture3(bottomLeftFront, textureBottomLeftFront);
            boxVertices[19] = new MyVertexFormatPositionTexture3(bottomLeftBack, textureBottomLeftBack);
            boxVertices[20] = new MyVertexFormatPositionTexture3(bottomRightBack, textureBottomRightBack);
            boxVertices[21] = new MyVertexFormatPositionTexture3(bottomLeftFront, textureBottomLeftFront);
            boxVertices[22] = new MyVertexFormatPositionTexture3(bottomRightBack, textureBottomRightBack);
            boxVertices[23] = new MyVertexFormatPositionTexture3(bottomRightFront, textureBottomRightFront);

            // Left face.
            boxVertices[24] = new MyVertexFormatPositionTexture3(topLeftFront, textureTopLeftFront);
            boxVertices[25] = new MyVertexFormatPositionTexture3(bottomLeftBack, textureBottomLeftBack);
            boxVertices[26] = new MyVertexFormatPositionTexture3(bottomLeftFront, textureBottomLeftFront);
            boxVertices[27] = new MyVertexFormatPositionTexture3(topLeftBack, textureTopLeftBack);
            boxVertices[28] = new MyVertexFormatPositionTexture3(bottomLeftBack, textureBottomLeftBack);
            boxVertices[29] = new MyVertexFormatPositionTexture3(topLeftFront, textureTopLeftFront);

            // Right face.
            boxVertices[30] = new MyVertexFormatPositionTexture3(topRightFront, textureTopRightFront);
            boxVertices[31] = new MyVertexFormatPositionTexture3(bottomRightFront, textureBottomRightFront);
            boxVertices[32] = new MyVertexFormatPositionTexture3(bottomRightBack, textureBottomRightBack);
            boxVertices[33] = new MyVertexFormatPositionTexture3(topRightBack, textureTopRightBack);
            boxVertices[34] = new MyVertexFormatPositionTexture3(topRightFront, textureTopRightFront);
            boxVertices[35] = new MyVertexFormatPositionTexture3(bottomRightBack, textureBottomRightBack);
            
            // if we've loaded the cube from DDS, orient it towards the sun
            var sun = MyGuiScreenGamePlay.Static.GetDirectionToSunNormalized();
            var toSun = new Quaternion(Vector3.Cross(Vector3.UnitX, sun), Vector3.Dot(Vector3.UnitX, sun));  // default orientation is +x
            toSun.Normalize();
            for (int i = 0; i < boxVertices.Length; i++)
            {
                boxVertices[i].Position = Vector3.Transform(boxVertices[i].Position, toSun);
            }
        
            m_boxVertexBuffer = new VertexBuffer(MyMinerGame.Static.GraphicsDevice, MyVertexFormatPositionTexture3.Stride * boxVertices.Length, Usage.WriteOnly, VertexFormat.None, Pool.Default);
            m_boxVertexBuffer.Lock(0, 0, LockFlags.None).WriteRange(boxVertices);
            m_boxVertexBuffer.Unlock();
            m_boxVertexBuffer.DebugName = "BackgroundCube";

            m_loaded = true;
        }

        public static void Draw()
        {      
            //  We can fill vertex buffer only when in Draw
            LoadInDraw();

            RasterizerState.CullClockwise.Apply();
            DepthStencilState.None.Apply();
            BlendState.Opaque.Apply();

            if (MyRender.CurrentRenderSetup.BackgroundColor != null)
            {
                MyMinerGame.Static.GraphicsDevice.Clear(ClearFlags.Target, SharpDXHelper.ToSharpDX(MyRender.CurrentRenderSetup.BackgroundColor.Value), 1, 0);
            }
            else
            {
                MyEffectBackgroundCube effect = MyRender.GetEffect(MyEffects.BackgroundCube) as MyEffectBackgroundCube;
                effect.SetViewProjectionMatrix(MyCamera.ViewMatrixAtZero * m_backgroundProjectionMatrix);
                effect.SetBackgroundTexture(m_textureCube);
                effect.SetBackgroundColor(MySector.SunProperties.BackgroundColor);
                MyMinerGame.Static.GraphicsDevice.VertexDeclaration = MyVertexFormatPositionTexture3.VertexDeclaration;
                MyMinerGame.Static.GraphicsDevice.SetStreamSource(0, m_boxVertexBuffer, 0, MyVertexFormatPositionTexture3.Stride);
                
                effect.Begin();

                MyMinerGame.Static.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, BOX_TRIANGLES_COUNT);

                effect.End();

                MyPerformanceCounter.PerCameraDraw.TotalDrawCalls++;
            }                
        }
    }
}
