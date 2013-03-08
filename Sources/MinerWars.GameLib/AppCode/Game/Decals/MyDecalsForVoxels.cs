using System.Collections.Generic;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Managers;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Utils.VertexFormats;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.Textures;
using System;
using MinerWars.AppCode.Game.Render;

using SharpDX;
using SharpDX.Direct3D9;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

namespace MinerWars.AppCode.Game.Decals
{
    using Byte4 = MinerWarsMath.Graphics.PackedVector.Byte4;
    using HalfVector2 = MinerWarsMath.Graphics.PackedVector.HalfVector2;
    using HalfVector4 = MinerWarsMath.Graphics.PackedVector.HalfVector4;
    using Vector2 = MinerWarsMath.Vector2;
    using Vector3 = MinerWarsMath.Vector3;
    using Vector4 = MinerWarsMath.Vector4;
    using Rectangle = MinerWarsMath.Rectangle;
    using Matrix = MinerWarsMath.Matrix;
    using Color = MinerWarsMath.Color;
    using BoundingBox = MinerWarsMath.BoundingBox;
    using BoundingSphere = MinerWarsMath.BoundingSphere;
    using BoundingFrustum = MinerWarsMath.BoundingFrustum;

    class MyDecalsForVoxels
    {
        enum MyDecalForVoxelsState : byte
        {
            READY,
            FADING_OUT
        }

        struct MyDecalsForVoxelsDictionaryKey : IEqualityComparer<MyDecalsForVoxelsDictionaryKey>, IEquatable<MyDecalsForVoxelsDictionaryKey>
        {
            public readonly int VoxelMapId;
            public readonly MyMwcVector3Int RenderCellCoord;
            public readonly MyDecalTexturesEnum DecalTexture;

            public MyDecalsForVoxelsDictionaryKey(int voxelMapId, ref MyMwcVector3Int renderCellCoord, MyDecalTexturesEnum decalTexture)
            {
                VoxelMapId = voxelMapId;
                RenderCellCoord = renderCellCoord;
                DecalTexture = decalTexture;                
            }

            #region Implementation of IEquatable<MyDecalsForVoxelsDictionaryKey>

            /// <summary>
            /// Equalses the specified other.
            /// </summary>
            /// <param name="other">The other.</param>
            /// <returns></returns>
            public bool Equals(MyDecalsForVoxelsDictionaryKey other)
            {
                return other.VoxelMapId == VoxelMapId && other.RenderCellCoord.Equals(RenderCellCoord) &&
                       other.DecalTexture == DecalTexture;
            }

            /// <summary>
            /// Returns a hash code for this instance.
            /// </summary>
            /// <returns>
            /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
            /// </returns>
            public override int GetHashCode()
            {
                unchecked
                {
                    int result = VoxelMapId;
                    result = (result*397) ^ RenderCellCoord.GetHashCode();
                    result = (result*397) ^ ((int)DecalTexture).GetHashCode();
                    return result;
                }
            }

            #endregion

            #region Implementation of IEqualityComparer<in MyDecalsForVoxelsDictionaryKey>

            /// <summary>
            /// Determines whether the specified objects are equal.
            /// </summary>
            /// <returns>
            /// true if the specified objects are equal; otherwise, false.
            /// </returns>
            /// <param name="x">The first object of type <paramref name="T"/> to compare.</param><param name="y">The second object of type <paramref name="T"/> to compare.</param>
            public bool Equals(MyDecalsForVoxelsDictionaryKey x, MyDecalsForVoxelsDictionaryKey y)
            {
                return x.Equals(y);
            }

            /// <summary>
            /// Returns a hash code for the specified object.
            /// </summary>
            /// <returns>
            /// A hash code for the specified object.
            /// </returns>
            /// <param name="obj">The <see cref="T:System.Object"/> for which a hash code is to be returned.</param><exception cref="T:System.ArgumentNullException">The type of <paramref name="obj"/> is a reference type and <paramref name="obj"/> is null.</exception>
            public int GetHashCode(MyDecalsForVoxelsDictionaryKey obj)
            {
                return obj.GetHashCode();
            }

            #endregion
        }
        
        MyDecalForVoxelsState m_status;
        int m_fadingOutStartTime;
        int m_capacity;
        int m_fadingOutStartLimit;
        int m_fadingOutBuffersCount;
        MyDecalsForVoxelsTriangleBuffer[] m_triangleBuffers;
        Dictionary<MyDecalsForVoxelsDictionaryKey, MyDecalsForVoxelsTriangleBuffer> m_triangleBuffersByKey;
        Stack<MyDecalsForVoxelsTriangleBuffer> m_freeTriangleBuffers;
        Queue<MyDecalsForVoxelsTriangleBuffer> m_usedTriangleBuffers;
        List<MyDecalsForVoxelsTriangleBuffer> m_sortTriangleBuffersByTexture;        


        public MyDecalsForVoxels(int capacity)
        {
            m_status = MyDecalForVoxelsState.READY;
            m_capacity = capacity;
            m_fadingOutStartLimit = (int)(m_capacity * MyDecalsConstants.TEXTURE_LARGE_FADING_OUT_START_LIMIT_PERCENT);
            m_fadingOutBuffersCount = (int)(m_capacity * MyDecalsConstants.TEXTURE_LARGE_FADING_OUT_MINIMAL_TRIANGLE_COUNT_PERCENT);

            m_sortTriangleBuffersByTexture = new List<MyDecalsForVoxelsTriangleBuffer>(m_capacity);
            m_triangleBuffersByKey = new Dictionary<MyDecalsForVoxelsDictionaryKey, MyDecalsForVoxelsTriangleBuffer>(m_capacity);
            m_freeTriangleBuffers = new Stack<MyDecalsForVoxelsTriangleBuffer>(m_capacity);
            m_usedTriangleBuffers = new Queue<MyDecalsForVoxelsTriangleBuffer>(m_capacity);

            m_triangleBuffers = new MyDecalsForVoxelsTriangleBuffer[m_capacity];
            for (int i = 0; i < m_capacity; i++)
            {
                m_triangleBuffers[i] = new MyDecalsForVoxelsTriangleBuffer(MyDecalsConstants.MAX_DECAL_TRIANGLES_IN_BUFFER);
                m_freeTriangleBuffers.Push(m_triangleBuffers[i]);
            }
        }

        public MyDecalsForVoxelsTriangleBuffer GetTrianglesBuffer(MyVoxelMap voxelMap, ref MyMwcVector3Int renderCellCoord, MyDecalTexturesEnum decalTexture, ref BoundingBox renderCellBoundingBox)
        {
            MyDecalsForVoxelsDictionaryKey key = new MyDecalsForVoxelsDictionaryKey(voxelMap.VoxelMapId, ref renderCellCoord, decalTexture);

            MyDecalsForVoxelsTriangleBuffer outValue;
            if (m_triangleBuffersByKey.TryGetValue(key, out outValue) == true)
            {
                //  Combination of cell/texture was found in dictionary, so we can return in right now
                return outValue;
            }
            else
            {
                if (m_triangleBuffersByKey.Count >= m_capacity)
                {
                    //  We are full, can't place decal on a new cell/texture. Need to wait for next CheckBufferFull.
                    return null;
                }
                else
                {
                    //  This is first time we want to place decal to this cell/texture, so here we allocate and initialize buffer
                    MyDecalsForVoxelsTriangleBuffer newBuffer = m_freeTriangleBuffers.Pop();
                    m_triangleBuffersByKey.Add(key, newBuffer);
                    m_usedTriangleBuffers.Enqueue(newBuffer);
                    newBuffer.Start(voxelMap, ref renderCellCoord, decalTexture, ref renderCellBoundingBox);
                    return newBuffer;
                }
            }
        }

        //  Blends-out triangles affected by explosion (radius + some safe delta). Triangles there have zero alpha are flaged to not-draw at all.
        public void HideTrianglesAfterExplosion(int voxelMapId, ref MyMwcVector3Int renderCellCoord, ref BoundingSphere explosionSphere)
        {
            //  Search for all buffers storing this voxelmap and render cell
            foreach (MyDecalsForVoxelsTriangleBuffer buffer in m_usedTriangleBuffers)
            {
                if ((buffer.VoxelMap.VoxelMapId == voxelMapId) && (buffer.RenderCellCoord.Equals(renderCellCoord)))
                {
                    buffer.HideTrianglesAfterExplosion(ref explosionSphere);
                }
            }
        }

        public void CheckIfBufferIsFull()
        {
            if (m_status == MyDecalForVoxelsState.FADING_OUT)
            {
                if ((MyMinerGame.TotalGamePlayTimeInMilliseconds - m_fadingOutStartTime) > MyDecalsConstants.DECALS_FADE_OUT_INTERVAL_MILISECONDS)
                {
                    //  If fading-out phase finished, we change state and remove faded-out buffers
                    for (int i = 0; i < m_fadingOutBuffersCount; i++)
                    {
                        MyDecalsForVoxelsTriangleBuffer releasedBuffer = m_usedTriangleBuffers.Dequeue();
                        releasedBuffer.Clear();
                        m_freeTriangleBuffers.Push(releasedBuffer);
                        m_triangleBuffersByKey.Remove(new MyDecalsForVoxelsDictionaryKey(releasedBuffer.VoxelMap.VoxelMapId, ref releasedBuffer.RenderCellCoord, releasedBuffer.DecalTexture));
                    }

                    m_status = MyDecalForVoxelsState.READY;
                }
            }
            else
            {
                if (m_triangleBuffersByKey.Count >= m_fadingOutStartLimit)
                {
                    int i = 0;
                    foreach (MyDecalsForVoxelsTriangleBuffer buffer in m_usedTriangleBuffers)
                    {
                        if (i < m_fadingOutBuffersCount)
                        {
                            buffer.FadeOutAll();
                        }
                        i++;
                    }

                    m_status = MyDecalForVoxelsState.FADING_OUT;
                    m_fadingOutStartTime = MyMinerGame.TotalGamePlayTimeInMilliseconds;
                }
            }
        }

        public void Draw(MyVertexFormatDecal[] vertices, Effects.MyEffectDecals effect, MyTexture2D[] texturesDiffuse, MyTexture2D[] texturesNormalMap)
        {
            CheckIfBufferIsFull();

            //  Sort buffers by texture
            m_sortTriangleBuffersByTexture.Clear();
            foreach (MyDecalsForVoxelsTriangleBuffer buffer in m_usedTriangleBuffers)
            {
                //  Check if render cell is close enought and visible in the view frustum
                if (//(buffer.VoxelMap.GetSmallestDistanceFromCameraToRenderCell(ref buffer.RenderCellCoord) <= (MyDecals.GetMaxDistanceForDrawingDecals())) && 
                    (MyCamera.IsInFrustum(ref buffer.RenderCellBoundingBox) == true))
                {
                    float fadeoutDistance = MyDecals.GetMaxDistanceForDrawingDecals();
                    if (buffer.DecalTexture == MyDecalTexturesEnum.ExplosionSmut)
                        fadeoutDistance *= MyDecalsConstants.DISTANCE_MULTIPLIER_FOR_LARGE_DECALS;

                    if(buffer.VoxelMap.GetSmallestDistanceFromCameraToRenderCell(ref buffer.RenderCellCoord) <= fadeoutDistance)
                        m_sortTriangleBuffersByTexture.Add(buffer);
                }
            }            
            m_sortTriangleBuffersByTexture.Sort();

            if (m_sortTriangleBuffersByTexture.Count <= 0) return;

            //  Draw decals - sorted by texture
            MyDecalTexturesEnum? lastDecalTexture = null;
            for (int i = 0; i < m_sortTriangleBuffersByTexture.Count; i++)
            {
                MyDecalsForVoxelsTriangleBuffer buffer = m_sortTriangleBuffersByTexture[i];

                int trianglesCount = buffer.CopyDecalsToVertexBuffer(vertices);

                if (trianglesCount <= 0) continue;

                //  Switch texture only if different than previous one
                if ((lastDecalTexture == null) || (lastDecalTexture != buffer.DecalTexture))
                {
                    int textureIndex = (int)buffer.DecalTexture;
                    effect.SetDecalDiffuseTexture(texturesDiffuse[textureIndex]);
                    effect.SetDecalNormalMapTexture(texturesNormalMap[textureIndex]);
                    lastDecalTexture = buffer.DecalTexture;
                }

                //  This will move all voxel maps back to position [0,0,0]
                effect.SetVoxelMapPosition(buffer.VoxelMap.PositionLeftBottomCorner - MyCamera.Position);

                effect.SetViewProjectionMatrix(MyCamera.ViewProjectionMatrixAtZero);

                // Set fadeout distance
                float fadeoutDistance = MyDecals.GetMaxDistanceForDrawingDecals();
                if (buffer.DecalTexture == MyDecalTexturesEnum.ExplosionSmut)
                    fadeoutDistance *= MyDecalsConstants.DISTANCE_MULTIPLIER_FOR_LARGE_DECALS;

                effect.SetFadeoutDistance(fadeoutDistance);

                if (!MyRenderConstants.RenderQualityProfile.ForwardRender)
                    effect.SetTechnique(Effects.MyEffectDecals.Technique.Voxels);
                else
                    effect.SetTechnique(Effects.MyEffectDecals.Technique.VoxelsForward);

                MyMinerGame.Static.GraphicsDevice.VertexDeclaration = MyVertexFormatDecal.VertexDeclaration;

                effect.Begin();
                MyMinerGame.Static.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, 0, trianglesCount, vertices);
                effect.End();

                MyPerformanceCounter.PerCameraDraw.DecalsForVoxelsInFrustum += trianglesCount;
            }
        }

        public void Clear()
        {
            for (int i = 0; i < m_capacity; i++)
            {
                m_triangleBuffers[i].Clear(true);
            }
        }
    }
}