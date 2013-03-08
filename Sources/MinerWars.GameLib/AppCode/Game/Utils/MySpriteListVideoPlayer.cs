using System.Collections.Generic;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.App;
using SysUtils.Utils;
using MinerWars.AppCode.Game.Effects;
using MinerWars.AppCode.Game.Render;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Textures;

using SharpDX.Direct3D9;
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

    class MySpriteListVideoPlayer
    {
        #region Private Fields
        private List<MyTexture2D> m_frames;
        private string m_fileName;        // folder to play
        Device m_graphicsDevice;
        private int m_realFramesPerSecond;
        private long m_initTime;//
        private SpriteBatch m_spriteBatch;
        int m_frameFirst;
        int m_frameLast;
        #endregion

        public MySpriteListVideoPlayer(string fileName, Device graphicsDevice, int fps, int frameFirst, int frameLast)
        {
            m_fileName = fileName;
            m_graphicsDevice = graphicsDevice;
            m_frames = new List<MyTexture2D>();
            m_initTime = MyMinerGame.TotalTimeInMilliseconds;
            m_realFramesPerSecond = fps;
            m_frameFirst = frameFirst;
            m_frameLast = frameLast;
        }
            
        //make list of textures here
        public void LoadContent()
        {
            MyMwcLog.WriteLine("MySpriteListVideoPlayer.LoadContent - START");
            MyMwcLog.IncreaseIndent();

            m_spriteBatch = new SpriteBatch(MyMinerGame.Static.GraphicsDevice, "SpriteListVideoPlayer");

            //  We must iterate over defined number of frames. We can't relly on reading files from a folder because in final
            //  build that folder will contain "hash" files and it may contain some other random files too, so then iteration won't work correctly.
            for (int frame = m_frameFirst; frame <= m_frameLast; frame++)
            {
                string toput = "Videos\\" + m_fileName + "\\" + MyStringUtils.AlignIntToRight(frame, 4, '0');
                MyMwcLog.WriteLine("Loading file: " + toput, SysUtils.LoggingOptions.LOADING_SPRITE_VIDEO);
                MyTexture2D tx = MyTextureManager.GetTexture<MyTexture2D>(toput);
                if (tx != null)
                    m_frames.Add(tx);
            }

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MySpriteListVideoPlayer.LoadContent - END");
        }

        public void UnloadContent()
        {
            m_spriteBatch.Dispose();
        }

        //
        public MyTexture2D GetCurrentFrame()
        {
            long deltaTime = MyMinerGame.TotalTimeInMilliseconds - m_initTime;//in ms
            long sequenceTime = (m_frames.Count) / m_realFramesPerSecond;// in seconds
            long realPosOfFrame = deltaTime % (sequenceTime * 1000); // gets [0 - sequenceTime * 1000 ] ( 0 - 1 of file scale) 
            long indexOfFrame = (m_frames.Count * realPosOfFrame) / (sequenceTime*1000);
            return m_frames[(int)indexOfFrame];
        }

        //
        public void DrawInterferencedBackGround(MyTexture2D overlappingTexture, Vector2 normalizedCoord, Vector2 normalizedSize, Vector2 overlappingTextureTiling, Color color, MyGuiDrawAlignEnum drawAlign)
        {
            MyEffectSpriteBatchShader spriteEffect = MyRender.GetEffect(MyEffects.VideoSpriteEffects) as MyEffectSpriteBatchShader ;
            
            spriteEffect.SetDiffuseTexture2(overlappingTexture);
            spriteEffect.SetTexture2Tiling(overlappingTextureTiling);
            
            m_spriteBatch.Begin(0, BlendState.AlphaBlend, null, null, null, spriteEffect.GetEffect());

            Vector2 screenCoord = MyGuiManager.GetScreenCoordinateFromNormalizedCoordinate(normalizedCoord);
            Vector2 screenSize = MyGuiManager.GetScreenSizeFromNormalizedSize(normalizedSize);
            screenCoord = MyGuiManager.GetAlignedCoordinate(screenCoord, screenSize, drawAlign);


            m_spriteBatch.Draw(GetCurrentFrame(), new SharpDX.DrawingRectangle((int)screenCoord.X, (int)screenCoord.Y, (int)screenSize.X, (int)screenSize.Y), SharpDX.Toolkit.SharpDXHelper.ToSharpDX(color));

            m_spriteBatch.End();
        }
    }
}
