using System;
using System.Collections.Generic;
using System.Linq;
using MinerWarsMath;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Utils.VertexFormats;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.Lights;


//  Buffer of voxel triangles for specified render cell and texture. It behaves similar to model/texture, except
//  when explosion near this cell, we change alpha of affected triangles.

namespace MinerWars.AppCode.Game.Decals
{
    class MyDecalsForVoxelsTriangleBuffer : IComparable
    {

        enum MyDecalsBufferState : byte
        {
            READY,
            FADING_OUT_ONLY_BEGINNING,
            FADING_OUT_ALL
        }

        //  These parameters are set when this buffer is allocated for new cell/texture
        //public int VoxelMapId;
        public MyVoxelMap VoxelMap;
        public MyMwcVector3Int RenderCellCoord;
        public MyDecalTexturesEnum DecalTexture;
        public BoundingBox RenderCellBoundingBox;

        public int MaxNeighbourTriangles;

        //  These parameters are preallocates 
        Queue<MyDecalTriangle> m_trianglesQueue;
        Stack<MyDecalTriangle> m_freeTriangles;
        MyDecalsBufferState m_status;
        int m_capacity;
        int m_capacityAfterStart;                   // This must be less than m_capacity, because buffer is initialized only once. This capacity is only work bounding number of decals for this type of texture
        int m_fadingOutStartLimit;                  //  Start fading out if this percent of buffer 'fillness' is achieved
        int m_fadingOutMinimalTriangleCount;        //  Minimal number of triangles we fade-out from the beggining of the queue
        int m_fadingOutStartTime;                   //  When last fading-out started
        int m_fadingOutRealTriangleCount;           //  When in fade-out phase, this is the real number of triangles we will fade-out. Always equal or more than 'm_fadingOutMinimalTriangleCount'


        public MyDecalsForVoxelsTriangleBuffer(int capacity)
        {
            m_capacity = capacity;
            m_trianglesQueue = new Queue<MyDecalTriangle>(m_capacity);

            //  Preallocate triangles
            m_freeTriangles = new Stack<MyDecalTriangle>(m_capacity);
            for (int i = 0; i < m_capacity; i++)
            {
                m_freeTriangles.Push(new MyDecalTriangle());
            }
        }

        //  Because this class is reused in buffers, it isn't really initialized by constructor. We make real initialization here.
        public void Start(MyVoxelMap voxelMap, ref MyMwcVector3Int renderCellCoord, MyDecalTexturesEnum decalTexture, ref BoundingBox renderCellBoundingBox)
        {
            VoxelMap = voxelMap;
            RenderCellCoord = renderCellCoord;
            DecalTexture = decalTexture;
            m_status = MyDecalsBufferState.READY;
            m_fadingOutStartTime = 0;
            RenderCellBoundingBox = renderCellBoundingBox;
            
            if (MyDecals.IsLargeTexture(decalTexture) == true)
            {
                m_capacityAfterStart = MyDecalsConstants.MAX_DECAL_TRIANGLES_IN_BUFFER_LARGE;
                m_fadingOutStartLimit = (int)(m_capacityAfterStart * MyDecalsConstants.TEXTURE_LARGE_FADING_OUT_START_LIMIT_PERCENT);
                m_fadingOutMinimalTriangleCount = (int)(m_capacityAfterStart * MyDecalsConstants.TEXTURE_LARGE_FADING_OUT_MINIMAL_TRIANGLE_COUNT_PERCENT);
                MaxNeighbourTriangles = MyDecalsConstants.TEXTURE_LARGE_MAX_NEIGHBOUR_TRIANGLES;
            }
            else
            {
                m_capacityAfterStart = MyDecalsConstants.MAX_DECAL_TRIANGLES_IN_BUFFER_SMALL;
                m_fadingOutStartLimit = (int)(m_capacityAfterStart * MyDecalsConstants.TEXTURE_SMALL_FADING_OUT_START_LIMIT_PERCENT);
                m_fadingOutMinimalTriangleCount = (int)(m_capacityAfterStart * MyDecalsConstants.TEXTURE_SMALL_FADING_OUT_MINIMAL_TRIANGLE_COUNT_PERCENT);
                MaxNeighbourTriangles = MyDecalsConstants.TEXTURE_SMALL_MAX_NEIGHBOUR_TRIANGLES;
            }
        }

        //  We can't just erase decal triangles. We need to push them back into 'free triangles stack'. So we do it here.
        public void Clear(bool destroy = false)
        {
            while (m_trianglesQueue.Count > 0)
            {
                MyDecalTriangle fadedoutTriangle = m_trianglesQueue.Dequeue();
                fadedoutTriangle.Close();
                m_freeTriangles.Push(fadedoutTriangle);
            }

            if (destroy)
                VoxelMap = null;
        }

        public void FadeOutAll()
        {
            if (m_status != MyDecalsBufferState.FADING_OUT_ONLY_BEGINNING)
            {
                m_fadingOutStartTime = MyMinerGame.TotalGamePlayTimeInMilliseconds;
            }

            m_status = MyDecalsBufferState.FADING_OUT_ALL;
        }

        //  Checks if buffer has enough free triangles for adding new decal. If not, we can't add triangles
        //  of the decal (because we add all decal triangles or none)
        public bool CanAddTriangles(int newTrianglesCount)
        {
            //  If whole buffer is fading out or if buffer is full, we can't add new triangles
            return (m_status != MyDecalsBufferState.FADING_OUT_ALL) && ((m_trianglesQueue.Count + newTrianglesCount) < m_capacityAfterStart);
        }

        public void Add(MyTriangle_Vertex_Normals triangle, Vector3 normal, ref MyPlane rightPlane, 
            ref MyPlane upPlane, float decalScale, int remainingTrianglesOfThisDecal, Vector4 color, bool alphaBlendByAngle, float lightSize, Vector3 position, float emissivity)
        {
            MyDecalTriangle decalTriangle = m_freeTriangles.Pop();
            
            decalTriangle.Start(lightSize);
            decalTriangle.Emissivity = emissivity;
            //decalTriangle.RandomOffset = MyMwcUtils.GetRandomFloat(0.0f, MathHelper.Pi);
            decalTriangle.RandomOffset = MyMinerGame.TotalGamePlayTimeInMilliseconds;
            decalTriangle.Position = position;

            // We must repack vertex positions before copying to new triange, to avoid Z-fight.
            decalTriangle.Position0 = MyUtils.RepackVoxelPosition(ref triangle.Vertexes.Vertex0);
            decalTriangle.Position1 = MyUtils.RepackVoxelPosition(ref triangle.Vertexes.Vertex1);
            decalTriangle.Position2 = MyUtils.RepackVoxelPosition(ref triangle.Vertexes.Vertex2);
   
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


            //  Alpha
            decalTriangle.Color0 = color;
            decalTriangle.Color1 = color;
            decalTriangle.Color2 = color;
            if (alphaBlendByAngle)
            {
                decalTriangle.Color0.W = GetAlphaByAngleDiff(ref normal, ref triangle.Normals.Normal0);
                decalTriangle.Color1.W = GetAlphaByAngleDiff(ref normal, ref triangle.Normals.Normal1);
                decalTriangle.Color2.W = GetAlphaByAngleDiff(ref normal, ref triangle.Normals.Normal2);
            }

            //  Bump mapping
            decalTriangle.Normal0 = triangle.Normals.Normal0;
            decalTriangle.Normal1 = triangle.Normals.Normal1;
            decalTriangle.Normal2 = triangle.Normals.Normal2;

            decalTriangle.Binormal0 = triangle.Binormals.Normal0;
            decalTriangle.Binormal1 = triangle.Binormals.Normal1;
            decalTriangle.Binormal2 = triangle.Binormals.Normal2;

            decalTriangle.Tangent0 = triangle.Tangents.Normal0;
            decalTriangle.Tangent1 = triangle.Tangents.Normal1;
            decalTriangle.Tangent2 = triangle.Tangents.Normal2;

            decalTriangle.Draw = true;
            decalTriangle.RemainingTrianglesOfThisDecal = remainingTrianglesOfThisDecal;

            m_trianglesQueue.Enqueue(decalTriangle);
        }

        float GetAlphaByAngleDiff(ref Vector3 referenceNormal, ref Vector3 vertexNormal)
        {
            //return (float)Math.Pow(Vector3.Dot(referenceNormal, vertexNormal), 5);
            float dot = Vector3.Dot(referenceNormal, vertexNormal);
            if (dot < MyMwcMathConstants.EPSILON)
                return 0;
            float result = (float)Math.Pow(dot, 1f);
            return MathHelper.Clamp(result, 0, 1);
        }

        //  Blends-out triangles affected by explosion (radius + some safe delta). Triangles there have zero alpha are flaged to not-draw at all.
        public void HideTrianglesAfterExplosion(ref BoundingSphere explosionSphere)
        {
            //  Make safe-delta (don't forget explosions have random radius - due to voxel stuff, so safe-radius should have highest possible value)
            float safeRadius = explosionSphere.Radius * 2.5f;

            foreach (MyDecalTriangle decalTriangle in m_trianglesQueue)
            {
                float distance0 = Vector3.Distance(explosionSphere.Center, decalTriangle.Position0 + VoxelMap.PositionLeftBottomCorner);
                float distance1 = Vector3.Distance(explosionSphere.Center, decalTriangle.Position1 + VoxelMap.PositionLeftBottomCorner);
                float distance2 = Vector3.Distance(explosionSphere.Center, decalTriangle.Position2 + VoxelMap.PositionLeftBottomCorner);
                if (distance0 <= safeRadius) decalTriangle.Color0.W = 0;
                if (distance1 <= safeRadius) decalTriangle.Color1.W = 0;
                if (distance2 <= safeRadius) decalTriangle.Color2.W = 0;

                if ((decalTriangle.Color0.W <= 0.0f) && (decalTriangle.Color1.W <= 0.0f) && (decalTriangle.Color2.W <= 0.0f))
                {
                    decalTriangle.Draw = false;
                    decalTriangle.Close();
                }
            }
        }

        //  Checks if buffer isn't full at 80% (or something like that). If is, we start fadeing-out first 20% triangles (or something like that). But always all triangles of a decal.
        void CheckIfBufferIsFull()
        {
            if (m_status == MyDecalsBufferState.FADING_OUT_ALL)
            {
                return;
            } 
            else if (m_status == MyDecalsBufferState.FADING_OUT_ONLY_BEGINNING)
            {
                if ((MyMinerGame.TotalGamePlayTimeInMilliseconds - m_fadingOutStartTime) > MyDecalsConstants.DECALS_FADE_OUT_INTERVAL_MILISECONDS)
                {
                    //  If fading-out phase finished, we change state and remove faded-out triangles
                    for (int i = 0; i < m_fadingOutRealTriangleCount; i++)
                    {
                        MyDecalTriangle fadedoutTriangle = m_trianglesQueue.Dequeue();
                        fadedoutTriangle.Close();
                        m_freeTriangles.Push(fadedoutTriangle);
                    }

                    m_status = MyDecalsBufferState.READY;
                }
            }
            else
            {
                if (m_trianglesQueue.Count >= m_fadingOutStartLimit) 
                {
                    //  If we get here, buffer is close to be full, so we start fade-out phase
                    m_fadingOutRealTriangleCount = GetFadingOutRealTriangleCount();
                    m_status = MyDecalsBufferState.FADING_OUT_ONLY_BEGINNING;
                    m_fadingOutStartTime = MyMinerGame.TotalGamePlayTimeInMilliseconds;
                }
            }
        }

        int GetFadingOutRealTriangleCount()
        {
            int result = 1;
            foreach (MyDecalTriangle decalTriangle in m_trianglesQueue)
            {
                if ((result >= m_fadingOutMinimalTriangleCount) && (decalTriangle.RemainingTrianglesOfThisDecal == 0))
                {
                    break;
                }

                result++;
            }

            return result;
        }

        //  For sorting buffers by texture
        public int CompareTo(object compareToObject)
        {
            MyDecalsForVoxelsTriangleBuffer compareToBuffer = (MyDecalsForVoxelsTriangleBuffer)compareToObject;
            return ((int)compareToBuffer.DecalTexture).CompareTo((int)this.DecalTexture);
        }

        //  Copy triangles to array of vertexes (which is then copyed to vertex buffer) and return count of triangles to draw
        public int CopyDecalsToVertexBuffer(MyVertexFormatDecal[] vertices)
        {
            CheckIfBufferIsFull();            

            float fadingOutAlpha = 1;
            if ((m_status == MyDecalsBufferState.FADING_OUT_ONLY_BEGINNING) || (m_status == MyDecalsBufferState.FADING_OUT_ALL))
            {
                fadingOutAlpha = 1 - MathHelper.Clamp((float)(MyMinerGame.TotalGamePlayTimeInMilliseconds - m_fadingOutStartTime) / (float)MyDecalsConstants.DECALS_FADE_OUT_INTERVAL_MILISECONDS, 0, 1);
            }

            int trianglesToDraw = 0;
            int i = 0;
            foreach (MyDecalTriangle decalTriangle in m_trianglesQueue)
            {
                if (decalTriangle.Draw == true)
                {
                    float alpha = 1;

                    //  If fading-out, we blend first 'm_fadingOutRealTriangleCount' triangles
                    if (m_status == MyDecalsBufferState.FADING_OUT_ALL)
                    {
                        alpha = fadingOutAlpha;
                    }
                    else if ((m_status == MyDecalsBufferState.FADING_OUT_ONLY_BEGINNING) && (i < m_fadingOutRealTriangleCount))
                    {
                        alpha = fadingOutAlpha;
                    }

                    int vertexIndexStart = trianglesToDraw * MyDecalsConstants.VERTEXES_PER_DECAL;

                    vertices[vertexIndexStart + 0].Position = decalTriangle.Position0;
                    vertices[vertexIndexStart + 1].Position = decalTriangle.Position1;
                    vertices[vertexIndexStart + 2].Position = decalTriangle.Position2;

                    vertices[vertexIndexStart + 0].TexCoord = decalTriangle.TexCoord0;
                    vertices[vertexIndexStart + 1].TexCoord = decalTriangle.TexCoord1;
                    vertices[vertexIndexStart + 2].TexCoord = decalTriangle.TexCoord2;

                    Vector4 color0 = decalTriangle.Color0;
                    Vector4 color1 = decalTriangle.Color1;
                    Vector4 color2 = decalTriangle.Color2;

                    //color0.W = 1;
                    //color1.W = 1;
                    //color2.W = 1;

                    color0 *= alpha;
                    color1 *= alpha;
                    color2 *= alpha;

                    vertices[vertexIndexStart + 0].Color = color0;
                    vertices[vertexIndexStart + 1].Color = color1;
                    vertices[vertexIndexStart + 2].Color = color2;

                    vertices[vertexIndexStart + 0].Normal = decalTriangle.Normal0;
                    vertices[vertexIndexStart + 1].Normal = decalTriangle.Normal1;
                    vertices[vertexIndexStart + 2].Normal = decalTriangle.Normal2;

                    vertices[vertexIndexStart + 0].Binormal = decalTriangle.Binormal0;
                    vertices[vertexIndexStart + 1].Binormal = decalTriangle.Binormal1;
                    vertices[vertexIndexStart + 2].Binormal = decalTriangle.Binormal2;

                    vertices[vertexIndexStart + 0].Tangent = decalTriangle.Tangent0;
                    vertices[vertexIndexStart + 1].Tangent = decalTriangle.Tangent1;
                    vertices[vertexIndexStart + 2].Tangent = decalTriangle.Tangent2;

                    float emisivity = MyDecals.UpdateDecalEmissivity(decalTriangle, alpha, null);

                    vertices[vertexIndexStart + 0].EmissiveRatio = emisivity;
                    vertices[vertexIndexStart + 1].EmissiveRatio = emisivity;
                    vertices[vertexIndexStart + 2].EmissiveRatio = emisivity;

                    trianglesToDraw++;
                }

                i++;
            }

            return trianglesToDraw;
        }
    }
}