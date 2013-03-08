using System.Collections.Generic;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.Managers;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Utils.VertexFormats;
using MinerWars.AppCode.Game.Textures;
using System;
using MinerWars.AppCode.Game.Effects;
using MinerWars.AppCode.Game.Render;

//using MinerWarsMath.Graphics;
using SharpDX;
using SharpDX.Direct3D9;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;



//  This class mainstains collection of model/texture decal triangleVertexes buffers.

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


    class MyDecalsForPhysObjects
    {
        enum MyDecalForModelsState : byte
        {
            READY,
            FADING_OUT
        }

        struct MyDecalsForModelsDictionaryKey
        {
            public MyEntity PhysObject;
            public MyDecalTexturesEnum DecalTexture;

            public MyDecalsForModelsDictionaryKey(MyEntity physObject, MyDecalTexturesEnum decalTexture)
            {
                PhysObject = physObject;
                DecalTexture = decalTexture;
            }
        }

        MyDecalForModelsState m_status;
        int m_fadingOutStartTime;
        int m_capacity;
        int m_fadingOutStartLimit;
        int m_fadingOutBuffersCount;
        MyDecalsForPhysObjectsTriangleBuffer[] m_triangleBuffers;
        Dictionary<MyDecalsForModelsDictionaryKey, MyDecalsForPhysObjectsTriangleBuffer> m_triangleBuffersByKey;
        Stack<MyDecalsForPhysObjectsTriangleBuffer> m_freeTriangleBuffers;
        List<MyDecalsForPhysObjectsTriangleBuffer> m_usedTriangleBuffers;
        List<MyDecalsForPhysObjectsTriangleBuffer> m_sortTriangleBuffersByTexture;


        public MyDecalsForPhysObjects(int capacity)
        {
            m_status = MyDecalForModelsState.READY;
            m_capacity = capacity;
            m_fadingOutStartLimit = (int)(m_capacity * MyDecalsConstants.TEXTURE_LARGE_FADING_OUT_START_LIMIT_PERCENT);
            m_fadingOutBuffersCount = (int)(m_capacity * MyDecalsConstants.TEXTURE_LARGE_FADING_OUT_MINIMAL_TRIANGLE_COUNT_PERCENT);

            m_sortTriangleBuffersByTexture = new List<MyDecalsForPhysObjectsTriangleBuffer>(m_capacity);
            m_triangleBuffersByKey = new Dictionary<MyDecalsForModelsDictionaryKey, MyDecalsForPhysObjectsTriangleBuffer>(m_capacity);
            m_freeTriangleBuffers = new Stack<MyDecalsForPhysObjectsTriangleBuffer>(m_capacity);
            m_usedTriangleBuffers = new List<MyDecalsForPhysObjectsTriangleBuffer>(m_capacity);

            m_triangleBuffers = new MyDecalsForPhysObjectsTriangleBuffer[m_capacity];
            for (int i = 0; i < m_capacity; i++)
            {
                m_triangleBuffers[i] = new MyDecalsForPhysObjectsTriangleBuffer(MyDecalsConstants.MAX_DECAL_TRIANGLES_IN_BUFFER);
                m_freeTriangleBuffers.Push(m_triangleBuffers[i]);
            }
        }

        public MyDecalsForPhysObjectsTriangleBuffer GetTrianglesBuffer(MyEntity physObject, MyDecalTexturesEnum decalTexture)
        {
            MyDecalsForModelsDictionaryKey key = new MyDecalsForModelsDictionaryKey(physObject, decalTexture);

            MyDecalsForPhysObjectsTriangleBuffer outValue;
            if (m_triangleBuffersByKey.TryGetValue(key, out outValue))
            {
                //  Combination of model/texture was found in dictionary, so we can return in right now
                return outValue;
            }
            else
            {
                if (m_triangleBuffersByKey.Count >= m_capacity)
                {
                    //  We are full, can't place decal on a new model/texture. Need to wait for next CheckBufferFull.
                    return null;
                }
                else
                {
                    //  This is first time we want to place decal on this model/texture, so here we allocate and initialize buffer
                    MyDecalsForPhysObjectsTriangleBuffer newBuffer = m_freeTriangleBuffers.Pop();
                    m_triangleBuffersByKey.Add(key, newBuffer);
                    m_usedTriangleBuffers.Add(newBuffer);
                    newBuffer.Start(physObject, decalTexture);
                    return newBuffer;
                }
            }
        }

        public void ReturnTrianglesBuffer( MyEntity physObject )
        {
            foreach (byte value in Enum.GetValues(typeof(MyDecalTexturesEnum)))
            {
                var key = new MyDecalsForModelsDictionaryKey(physObject, (MyDecalTexturesEnum)value);

                MyDecalsForPhysObjectsTriangleBuffer outValue;
                if (m_triangleBuffersByKey.TryGetValue(key, out outValue))
                {
                    MyDecalsForPhysObjectsTriangleBuffer usedBuffer = outValue;

                    m_triangleBuffersByKey.Remove(key);
                    usedBuffer.Clear();
                    m_usedTriangleBuffers.Remove(usedBuffer);
                    m_freeTriangleBuffers.Push(usedBuffer);
                }
            }
        }

        public void Clear()
        {
            for (int i = 0; i < m_capacity; i++)
            {
                m_triangleBuffers[i].Clear(true);
            }
        }

        public void CheckIfBufferIsFull()
        {
            if (m_status == MyDecalForModelsState.FADING_OUT)
            {
                if ((MyMinerGame.TotalGamePlayTimeInMilliseconds - m_fadingOutStartTime) > MyDecalsConstants.DECALS_FADE_OUT_INTERVAL_MILISECONDS)
                {
                    //  If fading-out phase finished, we change state and remove faded-out buffers
                    for (int i = 0; i < m_fadingOutBuffersCount; i++)
                    {
                        if (m_usedTriangleBuffers.Count > 0)
                        {
                            MyDecalsForPhysObjectsTriangleBuffer releasedBuffer = m_usedTriangleBuffers[0];
                            m_usedTriangleBuffers.RemoveAt(0);
                            releasedBuffer.Clear();
                            m_freeTriangleBuffers.Push(releasedBuffer);
                            m_triangleBuffersByKey.Remove(new MyDecalsForModelsDictionaryKey(releasedBuffer.Entity, releasedBuffer.DecalTexture));
                        }
                    }

                    m_status = MyDecalForModelsState.READY;
                }
            }
            else
            {
                if (m_triangleBuffersByKey.Count >= m_fadingOutStartLimit)
                {
                    int i = 0;
                    foreach (MyDecalsForPhysObjectsTriangleBuffer buffer in m_usedTriangleBuffers)
                    {
                        if (i < m_fadingOutBuffersCount)
                        {
                            buffer.FadeOutAll();
                        }
                        i++;
                    }

                    m_status = MyDecalForModelsState.FADING_OUT;
                    m_fadingOutStartTime = MyMinerGame.TotalGamePlayTimeInMilliseconds;
                }
            }
        }

        public void Draw(MyVertexFormatDecal[] vertices, MyEffectDecals effect, MyTexture2D[] texturesDiffuse, MyTexture2D[] texturesNormalMap)
        {
            CheckIfBufferIsFull();

            //  SortForSAP buffers by texture
            m_sortTriangleBuffersByTexture.Clear();
            foreach (MyDecalsForPhysObjectsTriangleBuffer buffer in m_usedTriangleBuffers)
            {
                if (buffer.Entity.IsVisible() == true)
                {
                    if ((buffer.Entity == MyGuiScreenGamePlay.Static.ControlledEntity
                        || buffer.Entity.Parent == MyGuiScreenGamePlay.Static.ControlledEntity) && 
                        MyGuiScreenGamePlay.Static.IsFirstPersonView)
                    {
                        //  Don't draw decals if they are on an entity in which the camera is
                        continue;
                    }

                    // Decal with "ExplosionSmut" texture is much larger, so it must be drawed to larger distance.
                    float fadeoutDistance = MyDecals.GetMaxDistanceForDrawingDecals();
                    if (buffer.DecalTexture == MyDecalTexturesEnum.ExplosionSmut)
                        fadeoutDistance *= MyDecalsConstants.DISTANCE_MULTIPLIER_FOR_LARGE_DECALS;

                    //if (Vector3.Distance(MyCamera.m_initialSunWindPosition, buffer.PhysObject.GetPosition()) >= (MyDecals.GetMaxDistanceForDrawingDecals()))
                    //if (buffer.PhysObject.GetDistanceBetweenCameraAndBoundingSphere() >= MyDecals.GetMaxDistanceForDrawingDecals())
                    if (buffer.Entity.GetDistanceBetweenCameraAndBoundingSphere() >= fadeoutDistance)
                    {
                        continue;
                    }

                    m_sortTriangleBuffersByTexture.Add(buffer);
                }
            }            
            m_sortTriangleBuffersByTexture.Sort();
            
            //  Draw decals - sorted by texture
            MyDecalTexturesEnum? lastDecalTexture = null;
            for (int i = 0; i < m_sortTriangleBuffersByTexture.Count; i++)
            {
                MyDecalsForPhysObjectsTriangleBuffer buffer = m_sortTriangleBuffersByTexture[i];

                int trianglesCount = buffer.CopyDecalsToVertices(vertices);

                if (trianglesCount <= 0) continue;

                //  Switch texture only if different than previous one
                if ((lastDecalTexture == null) || (lastDecalTexture != buffer.DecalTexture))
                {
                    int textureIndex = (int)buffer.DecalTexture;
                    effect.SetDecalDiffuseTexture(texturesDiffuse[textureIndex]);
                    effect.SetDecalNormalMapTexture(texturesNormalMap[textureIndex]);
                    lastDecalTexture = buffer.DecalTexture;
                }

                //effect.SetWorldMatrix(buffer.Entity.WorldMatrix * Matrix.CreateTranslation(-MyCamera.Position));
                effect.SetWorldMatrix(buffer.Entity.GetWorldMatrixForDraw());
                effect.SetViewProjectionMatrix(MyCamera.ViewProjectionMatrixAtZero);

                // set FadeoutDistance
                float fadeoutDistance = MyDecals.GetMaxDistanceForDrawingDecals();
                if (buffer.DecalTexture == MyDecalTexturesEnum.ExplosionSmut)
                    fadeoutDistance *= MyDecalsConstants.DISTANCE_MULTIPLIER_FOR_LARGE_DECALS;

                effect.SetFadeoutDistance(fadeoutDistance);

                if (!MyRenderConstants.RenderQualityProfile.ForwardRender)
                effect.SetTechnique(MyEffectDecals.Technique.Model);
                else
                    effect.SetTechnique(MyEffectDecals.Technique.ModelForward);

                MyMinerGame.Static.GraphicsDevice.VertexDeclaration = MyVertexFormatDecal.VertexDeclaration;

                effect.Begin();
                MyMinerGame.Static.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, 0, trianglesCount, vertices);
                effect.End();

                MyPerformanceCounter.PerCameraDraw.DecalsForEntitiesInFrustum += trianglesCount;
            }
        }
    }
}