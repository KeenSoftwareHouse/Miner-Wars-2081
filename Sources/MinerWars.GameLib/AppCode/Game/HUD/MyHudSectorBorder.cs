using System.IO;
using System.Linq;
using MinerWarsMath;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Managers;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Utils.VertexFormats;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.Render;

using SharpDX.Direct3D9;

namespace MinerWars.AppCode.Game.HUD
{
    
    static class MyHudSectorBorder
    {
        static MyTexture2D m_texture;
        static VertexBuffer m_boxVertexBuffer;
        static bool m_loaded = false;
        static Matrix m_ProjectionMatrix;
        const int BOX_TRIANGLES_COUNT = 12;
        
        public static string SectorBorderRenderingModuleName = "Draw sector bbox";

        static bool m_canDraw;

        static Vector4 m_sectorBorderAditionalColor;
        static bool m_enabled;
        public static bool Enabled { get { return m_enabled; } }

        static MyHudSectorBorder()
        {
            Render.MyRender.RegisterRenderModule(MyRenderModuleEnum.SectorBorder, "Sector border", Draw, Render.MyRenderStage.AlphaBlend);
            Render.MyRender.RegisterRenderModule(MyRenderModuleEnum.DrawSectorBBox, "Draw sector bbox", MyUtils.DrawSectorBoundingBox, Render.MyRenderStage.DebugDraw, false);
        }

        public static void LoadData()
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyHudSectorBorder.LoadData");
            m_enabled = false;
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        public static void LoadContent()
        {
            MyMwcLog.WriteLine("MyHudSectorBorder.LoadContent() - START");
            MyMwcLog.IncreaseIndent();
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyHudSectorBorder.LoadContent()");

            m_loaded = false;

            m_texture = MyTextureManager.GetTexture<MyTexture2D>("Textures\\HUD\\SectorBorder", flags: TextureFlags.IgnoreQuality);

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyHudSectorBorder.LoadContent() - END");
        }

        public static void SwitchToDraw()
        {
            float dist = MyGuiScreenGamePlay.Static.GetDistanceToSectorBoundaries();
            if (dist <= MyHudConstants.DISTANCE_FOR_SECTOR_BORDER_DRAW || m_enabled)
            {
                m_canDraw = true;
            }
            else
            {
                m_canDraw = false;
            }

            m_sectorBorderAditionalColor = new Vector4(1, 1, 1, m_enabled ? 0.2f : 0);

            if (dist <= MyHudConstants.DISTANCE_FOR_SECTOR_BORDER_DRAW && !m_enabled)
            {
                m_sectorBorderAditionalColor = new Vector4(0.7f, 0.078f, 0.157f, 0.5f);
            }
        }

        public static void SwitchSectorBorderVisibility()
        {
            m_enabled = !m_enabled;
            MyConfig.EditorEnableGrid = m_enabled;
            MyConfig.Save();
        }

        public static void UnloadContent()
        {
            MyMwcLog.WriteLine("MyHudSectorBorder.UnloadContent - START");
            MyMwcLog.IncreaseIndent();

            if (m_boxVertexBuffer != null)
            {
                m_boxVertexBuffer.Dispose();
                m_boxVertexBuffer = null;
            }

            m_loaded = false;

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyHudSectorBorder.UnloadContent - END");
        }

        //	Special method that loads data into GPU, and can be called only from Draw method, never from LoadContent or from background thread.
        //	Because that would lead to empty vertex/index buffers if they are filled/created while game is minimized (remember the issue - alt-tab during loading screen)
        static void LoadInDraw()
        {
            if (m_loaded) return;
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyHudSectorBorder::LoadInDraw");

            //TODO
                             /*
            VertexPositionColorTexture[] shapeVertices = new VertexPositionColorTexture[36];

            Vector3[] corners = MyMwcSectorConstants.SAFE_SECTOR_SIZE_BOUNDING_BOX_CORNERS;
            Vector3 topLeftFront = corners[4];
            Vector3 bottomLeftFront = corners[7];
            Vector3 topRightFront = corners[5];
            Vector3 bottomRightFront = corners[6];
            Vector3 topLeftBack = corners[0];
            Vector3 topRightBack = corners[1];
            Vector3 bottomLeftBack = corners[3];
            Vector3 bottomRightBack = corners[2];
            Vector2 textureTopLeft = new Vector2(1f, 0.0f);
            Vector2 textureTopRight = new Vector2(0.0f, 0.0f);
            Vector2 textureBottomLeft = new Vector2(1f, 1f);
            Vector2 textureBottomRight = new Vector2(0.0f, 1f);

            Color white = new Color(255, 255, 255, 25);
            Color green = new Color(200, 255, 200, 25);

            //  Front face
            shapeVertices[0] = new VertexPositionColorTexture(
                topLeftFront, white, textureTopLeft);
            shapeVertices[1] = new VertexPositionColorTexture(
                bottomLeftFront, white, textureBottomLeft);
            shapeVertices[2] = new VertexPositionColorTexture(
                topRightFront, white, textureTopRight);
            shapeVertices[3] = new VertexPositionColorTexture(
                bottomLeftFront, white, textureBottomLeft);
            shapeVertices[4] = new VertexPositionColorTexture(
                bottomRightFront, white, textureBottomRight);
            shapeVertices[5] = new VertexPositionColorTexture(
                topRightFront, white, textureTopRight);

            
            // Back face.
            shapeVertices[6] = new VertexPositionColorTexture(
                topLeftBack, white, textureTopRight);
            shapeVertices[7] = new VertexPositionColorTexture(
                topRightBack, white, textureTopLeft);
            shapeVertices[8] = new VertexPositionColorTexture(
                bottomLeftBack, white, textureBottomRight);
            shapeVertices[9] = new VertexPositionColorTexture(
                bottomLeftBack, white, textureBottomRight);
            shapeVertices[10] = new VertexPositionColorTexture(
                topRightBack, white, textureTopLeft);
            shapeVertices[11] = new VertexPositionColorTexture(
                bottomRightBack, white, textureBottomLeft);

            // Top face.
            shapeVertices[12] = new VertexPositionColorTexture(
                topLeftFront, green, textureBottomLeft);
            shapeVertices[13] = new VertexPositionColorTexture(
                topRightBack, green, textureTopRight);
            shapeVertices[14] = new VertexPositionColorTexture(
                topLeftBack, green, textureTopLeft);
            shapeVertices[15] = new VertexPositionColorTexture(
                topLeftFront, green, textureBottomLeft);
            shapeVertices[16] = new VertexPositionColorTexture(
                topRightFront, green, textureBottomRight);
            shapeVertices[17] = new VertexPositionColorTexture(
                topRightBack, green, textureTopRight);

            // Bottom face. 
            shapeVertices[18] = new VertexPositionColorTexture(
                bottomLeftFront, green, textureTopLeft);
            shapeVertices[19] = new VertexPositionColorTexture(
                bottomLeftBack, green, textureBottomLeft);
            shapeVertices[20] = new VertexPositionColorTexture(
                bottomRightBack, green, textureBottomRight);
            shapeVertices[21] = new VertexPositionColorTexture(
                bottomLeftFront, green, textureTopLeft);
            shapeVertices[22] = new VertexPositionColorTexture(
                bottomRightBack, green, textureBottomRight);
            shapeVertices[23] = new VertexPositionColorTexture(
                bottomRightFront, green, textureTopRight);

            // Left face.
            shapeVertices[24] = new VertexPositionColorTexture(
                topLeftFront, white, textureTopRight);
            shapeVertices[25] = new VertexPositionColorTexture(
                bottomLeftBack, white, textureBottomLeft);
            shapeVertices[26] = new VertexPositionColorTexture(
                bottomLeftFront, white, textureBottomRight);
            shapeVertices[27] = new VertexPositionColorTexture(
                topLeftBack, white, textureTopLeft);
            shapeVertices[28] = new VertexPositionColorTexture(
                bottomLeftBack, white, textureBottomLeft);
            shapeVertices[29] = new VertexPositionColorTexture(
                topLeftFront, white, textureTopRight);

            // Right face. 
            shapeVertices[30] = new VertexPositionColorTexture(
                topRightFront, white, textureTopLeft);
            shapeVertices[31] = new VertexPositionColorTexture(
                bottomRightFront, white, textureBottomLeft);
            shapeVertices[32] = new VertexPositionColorTexture(
                bottomRightBack, white, textureBottomRight);
            shapeVertices[33] = new VertexPositionColorTexture(
                topRightBack, white, textureTopRight);
            shapeVertices[34] = new VertexPositionColorTexture(
                topRightFront, white, textureTopLeft);
            shapeVertices[35] = new VertexPositionColorTexture(
                bottomRightBack, white, textureBottomRight);

            m_boxVertexBuffer = new VertexBuffer(MyMinerGame.Static.GraphicsDevice, VertexPositionColorTexture.VertexDeclaration, shapeVertices.Length, BufferUsage.None);
            m_boxVertexBuffer.SetData(shapeVertices);

            m_loaded = true;   */

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        public static void DrawInForeground()
        {
        }

        public static void Draw()
        {
            //  We can fill vertex buffer only when in Draw
            //LoadInDraw();

            if (!m_canDraw) 
                return;
                 /*
            GraphicsDevice device = MyMinerGame.Static.GraphicsDevice;
            device.DepthStencilState = DepthStencilState.None;
            device.RasterizerState = RasterizerState.CullNone;
            device.BlendState = BlendState.NonPremultiplied;

            MyEffectHudSectorBorder effect = MyRender.GetEffect(MyEffects.HudSectorBorder) as MyEffectHudSectorBorder;

            effect.WorldMatrix.SetValue(Matrix.CreateTranslation(-MyCamera.Position));

            effect.SectorBorderWarningDistance.SetValue(MyHudConstants.DISTANCE_FOR_SECTOR_BORDER_DRAW);
            effect.EyePosition.SetValue(Vector3.Zero);
            effect.SectorBorderAditionalColor.SetValue(m_sectorBorderAditionalColor);
            effect.SectorBorderTabPressed.SetValue(m_enabled);

            m_ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MyCamera.FovWithZoom, MyCamera.ForwardAspectRatio,
               1,
               MyMwcSectorConstants.SAFE_SECTOR_SIZE_HALF * 4);

            effect.ViewProjectionMatrix.SetValue(MyCamera.ViewMatrixAtZero * m_ProjectionMatrix);

            effect.GridTexture.SetValue(m_texture);

            device.SetVertexBuffer(m_boxVertexBuffer);

            effect.Apply();

            device.DrawPrimitives(PrimitiveType.TriangleList, 0, BOX_TRIANGLES_COUNT);
            MyPerformanceCounter.PerCameraDraw.TotalDrawCalls++;         */
        }
    }
}