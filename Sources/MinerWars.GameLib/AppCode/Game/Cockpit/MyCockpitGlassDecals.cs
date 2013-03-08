using System;
using System.Collections.Generic;
using MinerWarsMath;
using MinerWarsMath.Graphics;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.Managers;
using MinerWars.AppCode.Game.Lights;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Utils.VertexFormats;
using MinerWars.AppCode.Game.Render;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.Effects;
using MinerWars.AppCode.Game.Textures;
//  Holds and draws cockpit decals (bullet holes, dirt, etc)

namespace MinerWars.AppCode.Game.Cockpit
{
    //  IMPORTANT: If you add new texture, don't forget create for it a buffer few lines below
    enum MyCockpitGlassDecalTexturesEnum : byte
    {
        DirtOnGlass,
        BulletHoleOnGlass,
        BulletHoleSmallOnGlass
    }

    static class MyCockpitGlassDecals
    {
        static MyVertexFormatGlassDecal[] m_vertices;
        static MyTexture2D[] m_texturesDiffuse;
        static MyCockpitGlassDecalsBuffer[] m_buffers;
        static List<MyTriangle_Vertex_Normals> m_neighbourTriangles;

        public static void LoadData()
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyCockpitGlassDecals.LoadData");

            //  Must be smaller or equal, because if not, no neighbour triangleVertexes will fit in the buffer
            MyCommonDebugUtils.AssertDebug(MyCockpitGlassDecalsConstants.MAX_NEIGHBOUR_TRIANGLES <= MyCockpitGlassDecalsConstants.MAX_DECAL_TRIANGLES_IN_BUFFER);

            m_vertices = new MyVertexFormatGlassDecal[MyCockpitGlassDecalsConstants.MAX_DECAL_TRIANGLES_IN_BUFFER * MyCockpitGlassDecalsConstants.VERTEXES_PER_DECAL];
            m_neighbourTriangles = new List<MyTriangle_Vertex_Normals>(MyCockpitGlassDecalsConstants.MAX_NEIGHBOUR_TRIANGLES);

            //  Decal buffers
            m_buffers = new MyCockpitGlassDecalsBuffer[Enum.GetValues(typeof(MyCockpitGlassDecalTexturesEnum)).Length];
            m_buffers[(int)MyCockpitGlassDecalTexturesEnum.DirtOnGlass] = new MyCockpitGlassDecalsBuffer(3000, 500, 100, 2 * 1000, 1000);
            m_buffers[(int)MyCockpitGlassDecalTexturesEnum.BulletHoleOnGlass] = new MyCockpitGlassDecalsBuffer(1000, 500, 10, 15 * 1000, 300);
            m_buffers[(int)MyCockpitGlassDecalTexturesEnum.BulletHoleSmallOnGlass] = new MyCockpitGlassDecalsBuffer(1000, 500, 100, 15 * 1000, 300);

            int texturesCount = MyEnumsToStrings.CockpitGlassDecals.Length;
            m_texturesDiffuse = new MyTexture2D[texturesCount];

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        public static void LoadContent()
        {
            MyMwcLog.WriteLine("MyCockpitGlassDecals.LoadContent() - START");
            MyMwcLog.IncreaseIndent();
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyCockpitGlassDecals::LoadContent");
            
            int texturesCount = MyEnumsToStrings.CockpitGlassDecals.Length;
            for (int i = 0; i < texturesCount; i++)
            {
                m_texturesDiffuse[i] =
                    MyTextureManager.GetTexture<MyTexture2D>("Textures2\\Models\\Ships\\Cockpits\\Decals\\" + MyEnumsToStrings.CockpitGlassDecals[i] + "_Diffuse", t => MyUtils.AssertTexture((MyTexture2D)t));
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyCockpitGlassDecals.LoadContent() - END");
        }

        public static void UnloadContent()
        {
            MyMwcLog.WriteLine("MyCockpitGlassDecals.UnloadContent - START");
            MyMwcLog.IncreaseIndent();

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyCockpitGlassDecals.UnloadContent - END");
        }

        static MyCockpitGlassDecalsBuffer GetBuffer(MyCockpitGlassDecalTexturesEnum decalTexture)
        {
            return m_buffers[(int)decalTexture];
        }

        //  Add cockpit decal and all surounding triangles. Method needs intersection, but result of the intersection must be with ideal glass, not any other part of a miner ship.
        public static void Add(MyCockpitGlassDecalTexturesEnum decalTexture, float decalSize, float angle, float alpha,
            ref MyIntersectionResultLineTriangleEx idealIntersection, bool alphaBlendByAngle)
        {
            MyCockpitGlassDecalsBuffer buffer = GetBuffer(decalTexture);

            //	Polomer decalu a scale faktor pre vypocet textury.
            //  Decal size is something as radius of a decal, so when converting from real metres to texture space, we need to divide by 2.0
            float decalScale = 1.0f / decalSize / 2.0f;

            Vector3 rightVector = MyMwcUtils.Normalize(idealIntersection.Triangle.InputTriangle.Vertex0 - idealIntersection.IntersectionPointInObjectSpace);
            Vector3 upVector = MyMwcUtils.Normalize(Vector3.Cross(rightVector, idealIntersection.NormalInObjectSpace));

            //  We create world matrix for the decal and then rotate the matrix, so we can extract rotated right/up vectors/planes for texture coord0 calculations
            Matrix decalMatrix = Matrix.CreateRotationZ(angle) * Matrix.CreateWorld(idealIntersection.IntersectionPointInObjectSpace, idealIntersection.NormalInObjectSpace, upVector);

            //	Right plane
            MyPlane rightPlane;
            rightPlane.Point = idealIntersection.IntersectionPointInObjectSpace;
            rightPlane.Normal = MyUtils.GetTransformNormalNormalized(Vector3.Right, ref decalMatrix);

            //	Up plane
            MyPlane upPlane;
            upPlane.Point = idealIntersection.IntersectionPointInObjectSpace;
            upPlane.Normal = MyUtils.GetTransformNormalNormalized(Vector3.Up, ref decalMatrix);

            float? maxAngle = null;
            if (alphaBlendByAngle == false) maxAngle = MyCockpitGlassDecalsConstants.MAX_NEIGHBOUR_ANGLE;

            BoundingSphere decalSphere = new BoundingSphere(idealIntersection.IntersectionPointInObjectSpace, decalSize);
            m_neighbourTriangles.Clear();
            //idealIntersection.PhysObject.GetTrianglesIntersectingSphere(ref decalSphere, idealIntersection.NormalInObjectSpace, maxAngle, idealIntersection.TriangleHelperIndex, m_neighbourTriangles, buffer.MaxNeighbourTriangles);
            idealIntersection.Entity.GetTrianglesIntersectingSphere(ref decalSphere, idealIntersection.NormalInObjectSpace, maxAngle, m_neighbourTriangles, buffer.MaxNeighbourTriangles);

            int trianglesToAdd = m_neighbourTriangles.Count;// +1;
            if (buffer.CanAddTriangles(trianglesToAdd) == true)
            {
                //  Decal on triangleVertexes we hit
//                buffer.Add(idealIntersection.Triangle.InputTriangle, idealIntersection.NormalInObjectSpace, ref rightPlane, ref upPlane, decalScale, color, alphaBlendByAngle, ref decalSphere);

                //  Create decal for every neighbour triangleVertexes
                for (int i = 0; i < m_neighbourTriangles.Count; i++)
                {
                    buffer.Add(m_neighbourTriangles[i].Vertexes, idealIntersection.NormalInObjectSpace, ref rightPlane, ref upPlane, decalScale, alpha, alphaBlendByAngle, ref decalSphere);
                }
            }
        }

        public static void Clear()
        {
            foreach (var b in m_buffers)
            {
                b.Clear();
            }
        }

        public static void Draw(MyEffectCockpitGlass effect)
        {           
            for (int i = 0; i < m_buffers.Length; i++)
            {
                if (m_buffers[i].GetTrianglesCount() <= 0) 
                    continue;

                int trianglesToDraw = m_buffers[i].CopyDecalsToVertices(m_vertices);
                if (trianglesToDraw <= 0) continue;

                effect.SetCockpitGlassTexture(m_texturesDiffuse[i]);
                effect.SetGlassDirtLevelAlpha(Vector4.UnitW);
                
                effect.Begin();
                MyMinerGame.Static.GraphicsDevice.DrawUserPrimitives(SharpDX.Direct3D9.PrimitiveType.TriangleList, 0, trianglesToDraw, m_vertices);
                effect.End();

                MyPerformanceCounter.PerCameraDraw.DecalsForCockipGlassInFrustum += trianglesToDraw;
            }         
        }
    }
}
