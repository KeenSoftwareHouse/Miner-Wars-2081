using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Managers;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.Utils;

using MinerWars.AppCode.Game.Utils.VertexFormats;
using MinerWars.AppCode.Game.Render;
using MinerWars.AppCode.Game.Effects;

using SharpDX.Direct3D9;
using SharpDX.Toolkit.Graphics;


//  Use this class when you want draw many 3D lines at once and using DrawLine3D() will be extremely slow

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

    static class MyDebugDrawCachedLines
    {
        static MyVertexFormatPositionColor[] m_verticesLine = null;
        static int m_linesCount;

        public static void LoadContent()
        {
            MyMwcLog.WriteLine("MyDebugDrawCachedLines.LoadContent() - START");
            MyMwcLog.IncreaseIndent();

            Clear();

            //  Line
            m_verticesLine = new MyVertexFormatPositionColor[MyDebugDrawCachedLinesConstants.MAX_LINES_IN_CACHE * 2];

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyDebugDrawCachedLines.LoadContent() - END");
        }

        public static void UnloadContent()
        {
        }

        //  Clear line cache
        public static void Clear()
        {
            m_linesCount = 0;
        }

        //  Returns true if cache is full and no more lines can be added. Every consequent AddLine throws an exception.
        //  So lines must be drawn and cache cleared
        //  "reserveDelta" must be negative number and serves as a reserve when we are adding more triangles at once and need to know if we can add them all, or none
        public static bool IsFull(int reserveDelta)
        {
            return m_linesCount >= (MyDebugDrawCachedLinesConstants.MAX_LINES_IN_CACHE + reserveDelta);
        }

        //  Add 3d line into cache, so then we can draw many lines at once - by calling DrawLinesFromCache()
        public static void AddLine(Vector3 pointFrom, Vector3 pointTo, Color colorFrom, Color colorTo)
        {
            MyCommonDebugUtils.AssertDebug(IsFull(0) == false);

            if (m_linesCount + 2 >= MyDebugDrawCachedLinesConstants.MAX_LINES_IN_CACHE)
                return;

            m_verticesLine[m_linesCount * 2 + 0].Position = pointFrom;
            m_verticesLine[m_linesCount * 2 + 0].Color = colorFrom.ToVector4();
            m_verticesLine[m_linesCount * 2 + 1].Position = pointTo;
            m_verticesLine[m_linesCount * 2 + 1].Color = colorTo.ToVector4();
            m_linesCount++;
        }

        public static void AddAABB(BoundingBox aabb, Color color)
        {
            Vector3 min = aabb.Min;
            Vector3 max = aabb.Max;

            // bottom
            AddLine(min, new Vector3(max.X, min.Y, min.Z), color, color);
            AddLine(new Vector3(max.X, max.Y, min.Z), new Vector3(max.X, min.Y, min.Z), color, color);

            AddLine(min, new Vector3(min.X, max.Y, min.Z), color, color);
            AddLine(new Vector3(max.X, max.Y, min.Z), new Vector3(min.X, max.Y, min.Z), color, color);

            // top
            AddLine(new Vector3(min.X, min.Y, max.Z), new Vector3(max.X, min.Y, max.Z), color, color);
            AddLine(new Vector3(max.X, max.Y, max.Z), new Vector3(max.X, min.Y, max.Z), color, color);

            AddLine(new Vector3(min.X, min.Y, max.Z), new Vector3(min.X, max.Y, max.Z), color, color);
            AddLine(new Vector3(max.X, max.Y, max.Z), new Vector3(min.X, max.Y, max.Z), color, color);

            // vertical lines
            AddLine(new Vector3(min.X, min.Y, min.Z), new Vector3(min.X, min.Y, max.Z), color, color);
            AddLine(new Vector3(max.X, min.Y, min.Z), new Vector3(max.X, min.Y, max.Z), color, color);

            AddLine(new Vector3(min.X, max.Y, min.Z), new Vector3(min.X, max.Y, max.Z), color, color);
            AddLine(new Vector3(max.X, max.Y, min.Z), new Vector3(max.X, max.Y, max.Z), color, color);
        }

        //  Draws cached lines (added by AddLineToCache)
        public static void DrawLines()
        {                           
            if (m_linesCount <= 0) return;

            Device graphicsDevice = MyMinerGame.Static.GraphicsDevice;

            RasterizerState prev = RasterizerState.Current;
            RasterizerState.CullNone.Apply();

            var effect = (MyEffectModelsDiffuse)MyRender.GetEffect(MyEffects.ModelDiffuse);

            effect.SetProjectionMatrix(MyCamera.ProjectionMatrix);
            effect.SetViewMatrix(MyCamera.ViewMatrix);
            effect.SetWorldMatrix(Matrix.Identity);
            effect.SetDiffuseColor(Vector3.One);
            effect.SetTechnique(MyEffectModelsDiffuse.Technique.PositionColor);
            graphicsDevice.VertexDeclaration = MyVertexFormatPositionColor.VertexDeclaration;
            
            //  Draw the line                     
            effect.Begin();
            graphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, 0, m_linesCount, m_verticesLine);
            effect.End();

            prev.Apply();
        }
    }
}
