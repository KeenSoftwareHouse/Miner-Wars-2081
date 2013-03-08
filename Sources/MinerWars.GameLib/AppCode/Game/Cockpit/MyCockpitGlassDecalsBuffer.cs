using System.Collections.Generic;
using MinerWarsMath;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Utils.VertexFormats;
using MinerWars.CommonLIB.AppCode.Generics;
using SysUtils.Utils;

//  Buffer for specified type of glass decal texture (e.g. bullet hole, or dirt, ...)
//  This buffer works as some sort of FIFO (but not exactly... because we are adding decals but if some max number reached, we need to remove all and only then add new ones)

namespace MinerWars.AppCode.Game.Cockpit
{
    class MyCockpitGlassDecalsBuffer
    {
        class MyCockpitGlassDecalTriangle
        {
            public Vector3 Position0;
            public Vector3 Position1;
            public Vector3 Position2;
            public Vector2 TexCoord0;
            public Vector2 TexCoord1;
            public Vector2 TexCoord2;
            public Vector3 Normal0;
            public Vector3 Normal1;
            public Vector3 Normal2;
            public Vector3 Alpha012;
            public int CreatedTime;
        }

        public int MaxNeighbourTriangles;
        int m_fadeInPhaseInMiliseconds;
        int m_fadeOutStartInMiliseconds;
        int m_fadeOutLengthInMiliseconds;

        MyObjectsPool<MyCockpitGlassDecalTriangle> m_triangles;

        public MyCockpitGlassDecalsBuffer(int capacity, int maxNeighbourTriangles, int fadeInPhaseInMiliseconds, int fadeOutStartInMiliseconds, int fadeOutLengthInMiliseconds)
        {
            //  Fade in (initial phase) can't interfere with fadeout phase
            MyCommonDebugUtils.AssertRelease(fadeInPhaseInMiliseconds < fadeOutStartInMiliseconds);

            //  Buffer can't store more decal triangles than vertex buffer used for drawing them!
            MyCommonDebugUtils.AssertRelease(capacity <= MyCockpitGlassDecalsConstants.MAX_DECAL_TRIANGLES_IN_BUFFER);

            MyCommonDebugUtils.AssertRelease(maxNeighbourTriangles <= MyCockpitGlassDecalsConstants.MAX_NEIGHBOUR_TRIANGLES);

            //  If there will be more neighbour triangles, we won't be able to add them into buffer in one call!
            MyCommonDebugUtils.AssertRelease(maxNeighbourTriangles < capacity);

            m_triangles = new MyObjectsPool<MyCockpitGlassDecalTriangle>(capacity);
            MaxNeighbourTriangles = maxNeighbourTriangles;
            m_fadeInPhaseInMiliseconds = fadeInPhaseInMiliseconds;
            m_fadeOutStartInMiliseconds = fadeOutStartInMiliseconds;
            m_fadeOutLengthInMiliseconds = fadeOutLengthInMiliseconds;
        }

        //  Checks if buffer has enough free triangles for adding new decal. If not, we can't add triangles
        //  of the decal (because we add all decal triangles or none)
        public bool CanAddTriangles(int newTrianglesCount)
        {
            //  If buffer is full, we can't add new triangles
            return ((m_triangles.GetActiveCount() + newTrianglesCount) < m_triangles.GetCapacity());
        }

        public int GetTrianglesCount()
        {
            return m_triangles.GetActiveCount();
        }

        public void Add(MyTriangle_Vertexes triangle, Vector3 normal, ref MyPlane rightPlane, ref MyPlane upPlane, float decalScale, float alpha, bool alphaBlendByAngle, ref BoundingSphere decalSphere)
        {
            float alpha0 = alpha;
            float alpha1 = alpha;
            float alpha2 = alpha;

            if (alphaBlendByAngle == true)
            {
                alpha0 *= GetAlphaByDistance(ref triangle.Vertex0, ref decalSphere);
                alpha1 *= GetAlphaByDistance(ref triangle.Vertex1, ref decalSphere);
                alpha2 *= GetAlphaByDistance(ref triangle.Vertex2, ref decalSphere);
            }

            if ((alpha0 <= 0.0f) && (alpha1 <= 0.0f) && (alpha2 <= 0.0f))
            {
                //  Decal would be totaly transparent so it doesn't make sense to draw it
                return;
            }

            MyCockpitGlassDecalTriangle decalTriangle = m_triangles.Allocate();
            if (decalTriangle == null) 
                return;

            decalTriangle.Position0 = triangle.Vertex0;
            decalTriangle.Position1 = triangle.Vertex1;
            decalTriangle.Position2 = triangle.Vertex2;

            //  Texture coords
            decalTriangle.TexCoord0 = new Vector2(
                0.5f + decalScale * MyUtils.GetDistanceFromPointToPlane(ref decalTriangle.Position0, ref rightPlane),
                0.5f + decalScale * MyUtils.GetDistanceFromPointToPlane(ref decalTriangle.Position0, ref upPlane));
            decalTriangle.TexCoord1 = new Vector2(
                0.5f + decalScale * MyUtils.GetDistanceFromPointToPlane(ref decalTriangle.Position1, ref rightPlane),
                0.5f + decalScale * MyUtils.GetDistanceFromPointToPlane(ref decalTriangle.Position1, ref upPlane));
            decalTriangle.TexCoord2 = new Vector2(
                0.5f + decalScale * MyUtils.GetDistanceFromPointToPlane(ref decalTriangle.Position2, ref rightPlane),
                0.5f + decalScale * MyUtils.GetDistanceFromPointToPlane(ref decalTriangle.Position2, ref upPlane));

            //  Normal
            normal = MinerWars.CommonLIB.AppCode.Utils.MyMwcUtils.Normalize(normal);
            decalTriangle.Normal0 = normal;
            decalTriangle.Normal1 = normal;
            decalTriangle.Normal2 = normal;

            decalTriangle.Alpha012.X = alpha0;
            decalTriangle.Alpha012.Y = alpha1;
            decalTriangle.Alpha012.Z = alpha2;

            //  Time created
            decalTriangle.CreatedTime = MyMinerGame.TotalGamePlayTimeInMilliseconds;
        }

        float GetAlphaByDistance(ref Vector3 position, ref BoundingSphere decalSphere)
        {
            float dist;
            Vector3.Distance(ref position, ref decalSphere.Center, out dist);

            //  This sort of alpha blending will cause that inside of sphere is always visible and blended is only what is outside
            return (dist >= decalSphere.Radius) ? 0 : 1;

            //return 1 - (float)Math.Pow(MathHelper.Clamp(dist / decalSphere.Radius, 0, 1), 2);
            //return 1 - MathHelper.Clamp(dist / decalSphere.Radius, 0, 1);
            
            //return (float)Math.Pow(Vector3.Dot(referenceNormal, vertexNormal), 20);
            //return ;
            //return MathHelper.SmoothStep(0, 1, Vector3.Dot(referenceNormal, vertexNormal));
            //return 1;
        }

        //  Copy triangles to array of vertexes (which is then copyed to vertex buffer) and return count of triangles to draw
        public int CopyDecalsToVertices(MyVertexFormatGlassDecal[] vertexes)
        {
            int trianglesToDraw = 0;
            foreach (LinkedListNode<MyCockpitGlassDecalTriangle> item in m_triangles)
            {
                MyCockpitGlassDecalTriangle triangle = item.Value;

                int deltaTime = MyMinerGame.TotalGamePlayTimeInMilliseconds - triangle.CreatedTime;

                //  If triangleVertexes is completely faded-out, we remove it from buffer and don't draw it now
                if (deltaTime >= (m_fadeOutStartInMiliseconds + m_fadeOutLengthInMiliseconds))
                {
                    m_triangles.MarkForDeallocate(item);
                    continue;
                }
                
                int vertexIndexStart = trianglesToDraw * MyCockpitGlassDecalsConstants.VERTEXES_PER_DECAL;

                //  Texture coords
                vertexes[vertexIndexStart + 0].TexCoord = triangle.TexCoord0;
                vertexes[vertexIndexStart + 1].TexCoord = triangle.TexCoord1;
                vertexes[vertexIndexStart + 2].TexCoord = triangle.TexCoord2;

                float alpha = 1.0f;
                if (deltaTime <= m_fadeInPhaseInMiliseconds)
                {
                    //  If we are in fade-in phase (initial phase)
                    alpha = (float)deltaTime / (float)m_fadeInPhaseInMiliseconds;
                }
                else if (deltaTime >= m_fadeOutStartInMiliseconds)
                {
                    //  If fading-out started, change alpha here
                    alpha = 1.0f - ((float)(deltaTime - m_fadeOutStartInMiliseconds) / (float)m_fadeOutLengthInMiliseconds);
                }

                //  Vertex position
                vertexes[vertexIndexStart + 0].SetPositionAndAlpha(ref triangle.Position0, System.Math.Min(alpha, triangle.Alpha012.X));
                vertexes[vertexIndexStart + 1].SetPositionAndAlpha(ref triangle.Position1, System.Math.Min(alpha, triangle.Alpha012.Y));
                vertexes[vertexIndexStart + 2].SetPositionAndAlpha(ref triangle.Position2, System.Math.Min(alpha, triangle.Alpha012.Z));

                //  Normal vectors
                Vector3 normal0 = triangle.Normal0;
                Vector3 normal1 = triangle.Normal1;
                Vector3 normal2 = triangle.Normal2;

                // Normals for glass decals aren't normalized
                normal0.Normalize();
                normal1.Normalize();
                normal2.Normalize();

                vertexes[vertexIndexStart + 0].Normal = normal0;
                vertexes[vertexIndexStart + 1].Normal = normal1;
                vertexes[vertexIndexStart + 2].Normal = normal2;

                trianglesToDraw++;
            }

            m_triangles.DeallocateAllMarked();

            return trianglesToDraw;
        }

        internal void Clear()
        {
            m_triangles.DeallocateAll();
        }
    }
}
