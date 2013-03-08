using System;
using MinerWarsMath;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.BackgroundCube;
using MinerWars.AppCode.Game.HUD;
using MinerWars.AppCode.Game.Managers;
using MinerWars.AppCode.Game.Models;
using MinerWars.AppCode.Game.TransparentGeometry;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Utils.VertexFormats;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;

namespace MinerWars.AppCode.Game.GUI.Core
{
    //  IMPORTATNT: This class must UnloadContent in the end of lifecycle
    //class MyGuiControlShipPreview : MyGuiControlBase
    //{
        //const float CAMERA_REPOSITION_FOR_SHIP_MENU_DRAWING = 42f;

        //MySmallShip m_minerShip;

        //MyStencilOpaqueTexture m_backCameraStencilOpaque;
        //MyCustomContentManager m_localContentManager;
        //MyEffectHud m_effect;
        //Texture2D m_borderTexture;

        //Vector3 m_positionForNewShip;
        //Matrix m_oldWorldMatrix;

        //MyMwcPositionAndOrientation m_previewPosition;

        //static VertexBuffer m_vertexBuffer;
        //static VertexPositionColorTexture[] m_vertexes;
        //static Matrix m_orthographicProjectionMatrix;

        //public MyGuiControlShipPreview(MyGuiScreenBase parentScreen, MyMwcObjectBuilder_SmallShip smallShipObjectBuilder)
        //    : base(parentScreen, Vector2.Zero, null, null)
        //{
        //    Vector3 oldShipPosition = MyGuiScreenGameBase.Static.PlayerShip.GetPosition();
        //    m_positionForNewShip = oldShipPosition + MyGuiScreenGameBase.Static.PlayerShip.WorldMatrix.Backward * 50f;
        //    m_oldWorldMatrix = MyGuiScreenGameBase.Static.PlayerShip.WorldMatrix;

        //    m_previewPosition = new MyMwcPositionAndOrientation(m_positionForNewShip, m_oldWorldMatrix.Forward, m_oldWorldMatrix.Up);

        //    CreateSmallShipForPreview(smallShipObjectBuilder);

        //    //MyFakes.DRAW_PLAYER_MINER_SHIP = false;
        //    m_localContentManager = new MyCustomContentManager(MyMinerGame.Static.Services, MyMinerGame.Static.Content.RootDirectory);
        //    m_effect = new MyEffectHud(m_localContentManager, "Effects\\HUD\\MyHudEffect");
        //    m_backCameraStencilOpaque = new MyStencilOpaqueTexture("ShipCustomizationStencil", m_localContentManager);
    //    m_borderTexture = m_localContentManager.Load<Texture2D>("Textures\\HUD\\ShipCustomizationBorder", flags: TextureFlags.IgnoreQuality);
        //    m_vertexes = new VertexPositionColorTexture[MyHudConstants.VERTEXES_PER_HUD_QUAD];
        //    m_vertexBuffer = new VertexBuffer(MyMinerGame.Static.GraphicsDevice, typeof(VertexPositionColorTexture), m_vertexes.Length, BufferUsage.None);
        //    int width = 1;
        //    float height = width / MyCamera.MenuViewPort.AspectRatio;
        //    m_orthographicProjectionMatrix = Matrix.CreateOrthographicOffCenter(0.0f, width, height, 0.0f, 0.0f, 1000);
        //    AddShipCustomizationBorders();
        //}

        //public void CreateSmallShipForPreview(MyMwcObjectBuilder_SmallShip smallShipObjectBuilder)
        //{
        //    MyMwcPositionAndOrientation newPosition;
        //    //Destroy old ship if there is any
        //    if (m_minerShip != null)
        //    {
        //        newPosition = ((MyMwcObjectBuilder_SmallShip)m_minerShip.ObjectBuilder).PositionAndOrientation;
        //        m_minerShip.Close();
        //        m_minerShip = null;
        //    }
        //    else
        //    {
        //        newPosition = m_previewPosition;
        //    }
        //    //Create new ship (need diferent position, but don't change original object builder)
        //    MyMwcPositionAndOrientation originalPosition = smallShipObjectBuilder.PositionAndOrientation;
        //    smallShipObjectBuilder.PositionAndOrientation = newPosition;
        //    m_minerShip = (MySmallShip)MyEntities.CreateFromObjectBuilder(null, smallShipObjectBuilder);
        //    smallShipObjectBuilder.PositionAndOrientation = originalPosition;
        //}

        //public override void Update()
        //{
        //    base.Update();

        //    if (m_backCameraStencilOpaque != null) m_backCameraStencilOpaque.UpdateScreenSize(MyCamera.MenuViewPort);

        //    if (m_minerShip != null)
        //    {
        //        m_minerShip.MoveAndRotate(Vector3.Zero, new Vector2(0, -4), 0, false);
        //        m_minerShip.UpdateBeforeIntegration();
        //        m_minerShip.UpdateAfterIntegration();
        //    }
        //}

        //public override void Draw()
        //{
        //    base.Draw();
        //    if (m_minerShip == null) return;
        //    m_minerShip.DynamicLightsOn = true;
        //    MyCameraAttachedToEnum oldCamera = MyGuiScreenGameBase.Static.CameraAttachedTo;
        //    MyGuiScreenGameBase.Static.CameraAttachedTo = MyCameraAttachedToEnum.PlayerMinerShip;
        //    MyPerformanceCounter.PerCameraDraw.Reset();
        //    MyCamera.EnableShipCustomizationScreen(m_minerShip.Volume.Radius);
        //    MyMinerGame.Static.GraphicsDevice.Clear(ClearOptions.Stencil, Color.White, 1, 0);
        //    MyMinerGame.Static.GraphicsDevice.Clear(ClearOptions.DepthBuffer, Color.White, 1, 0);
        //    m_backCameraStencilOpaque.Draw();
        //    m_backCameraStencilOpaque.StencilOpaqueEnable();
        //    DrawMenuScene();
        //    m_backCameraStencilOpaque.StencilOpaqueDisable();
        //    DrawVertexBuffer();
        //    MyCamera.EnableForward();
        //    MyGuiScreenGameBase.Static.CameraAttachedTo = oldCamera;
        //}

        //void DrawMenuScene()
        //{
        //    MyEffectValuesManager.Update();
        //    MyBackgroundCube.Draw();
        //    MyCamera.Position += MyMwcUtils.Normalize(m_positionForNewShip - MyCamera.Position) * CAMERA_REPOSITION_FOR_SHIP_MENU_DRAWING;
        //    m_minerShip.Draw();
        //    MyModels.Draw();
        //    MyParticles.Draw(true, true);
        //}

        //void DrawVertexBuffer()
        //{
        //    MyRenderStatePool.CullMode = CullMode.None;
        //    MyRenderStatePool.AlphaBlendEnable = true;
        //    MyRenderStatePool.BlendFunction = BlendFunction.Add;
        //    MyRenderStatePool.DepthBufferFunction = CompareFunction.Always;
        //    MyRenderStatePool.DepthBufferWriteEnable = false;
        //    MyRenderStatePool.SourceBlend = Blend.SourceAlpha;
        //    MyRenderStatePool.DestinationBlend = Blend.InverseSourceAlpha;
        //    DrawVertexBuffer(m_borderTexture, m_vertexBuffer, m_vertexes, MyHudConstants.VERTEXES_PER_HUD_QUAD, MyHudConstants.TRIANGLES_PER_HUD_QUAD);
        //    MyRenderStatePool.AlphaBlendEnable = false;
        //    MyRenderStatePool.DepthBufferWriteEnable = true;
        //    MyRenderStatePool.DepthBufferFunction = CompareFunction.LessEqual;
        //}

        //void DrawVertexBuffer(Texture2D texture, VertexBuffer vertexBuffer, VertexPositionColorTexture[] vertexes, int vertexCount, int primitivesCount)
        //{
        //    GraphicsDevice device = MyMinerGame.Static.GraphicsDevice;
        //    vertexBuffer.SetData(vertexes, 0, vertexCount);
        //    device.Indices = null;
        //    device.VertexDeclaration = MyVertexFormatManager.Get(MyVertexFormatTypes.POSITION_COLOR_TEXTURE).GetVertexDeclaration();
        //    device.Vertices[0].SetSource(vertexBuffer, 0, VertexPositionColorTexture.SizeInBytes);
        //    m_effect.ProjectionMatrix.SetValue(m_orthographicProjectionMatrix);
        //    m_effect.HudTexture.SetValue(texture);
        //    m_effect.Effect.Begin();
        //    m_effect.Effect.GetTechnique(0].Passes[0].Begin();
        //    device.DrawPrimitives(PrimitiveType.TriangleList, 0, primitivesCount);
        //    m_effect.Effect.GetTechnique(0].Passes[0].End();
        //    m_effect.Effect.End();
        //}

        //void AddShipCustomizationBorders()
        //{
        //    AddTexturedQuad(Color.Azure);
        //}

        //void AddTexturedQuad(Color color)
        //{
        //    m_vertexes[0].Color = color;
        //    m_vertexes[1].Color = color;
        //    m_vertexes[2].Color = color;
        //    m_vertexes[3].Color = color;
        //    m_vertexes[4].Color = color;
        //    m_vertexes[5].Color = color;

        //    int width = 1;
        //    float height = width / MyCamera.MenuViewPort.AspectRatio;

        //    m_vertexes[0].Position = new Vector3(0, 0, 0);
        //    m_vertexes[1].Position = new Vector3(width, 0, 0);
        //    m_vertexes[2].Position = new Vector3(0, height, 0);
        //    m_vertexes[3].Position = new Vector3(width, 0, 0);
        //    m_vertexes[4].Position = new Vector3(width, height, 0);
        //    m_vertexes[5].Position = new Vector3(0, height, 0);

        //    m_vertexes[0].TextureCoordinate = new Vector2(0, 0);
        //    m_vertexes[1].TextureCoordinate = new Vector2(1, 0);
        //    m_vertexes[2].TextureCoordinate = new Vector2(0, 1);
        //    m_vertexes[3].TextureCoordinate = new Vector2(1, 0);
        //    m_vertexes[4].TextureCoordinate = new Vector2(1, 1);
        //    m_vertexes[5].TextureCoordinate = new Vector2(0, 1);

        //}

        //public virtual void UnloadContent()
        //{
        //    if (m_vertexBuffer != null) m_vertexBuffer.Dispose();
        //    if (m_localContentManager != null)
        //    {
        //        m_localContentManager.Unload();
        //    }
        //    if (m_minerShip != null)
        //    {
        //        m_minerShip.Close();
        //        m_minerShip = null;
        //    }
        //}
    //}
}
