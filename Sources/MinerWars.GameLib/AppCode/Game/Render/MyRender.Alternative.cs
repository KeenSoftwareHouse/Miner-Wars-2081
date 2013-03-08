using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX.Toolkit.Graphics;
using MinerWars.AppCode.Game.Managers;
using SharpDX.Direct3D9;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.Import;
using MinerWars.AppCode.Game.Effects;
using SysUtils.Utils;
using MinerWars.AppCode.Game.Voxels;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Models;
using MinerWarsMath;
using MinerWars.CommonLIB.AppCode.Networking;

namespace MinerWars.AppCode.Game.Render
{
    static partial class MyRender
    {
        private static void DrawRenderElementsAlternative(MyLodTypeEnum lod, bool applyStencil, out int ibChangesStats)
        {
            m_currentLodDrawPass = lod;

            ibChangesStats = 0;

            BlendState.Opaque.Apply(); //set by default, blend elements are at the end. 
            DrawVoxels(lod, MyVoxelCacheCellRenderBatchType.SINGLE_MATERIAL, ref ibChangesStats);
            DrawVoxels(lod, MyVoxelCacheCellRenderBatchType.MULTI_MATERIAL, ref ibChangesStats);

            BlendState.Opaque.Apply(); //set by default, blend elements are at the end. 
            DrawModels(lod, ref ibChangesStats);
        }

        private static void DrawVoxels(MyLodTypeEnum lod, MyVoxelCacheCellRenderBatchType batchType, ref int ibChangesStats)
        {
            int index = m_sortedElements.GetVoxelIndex(lod, batchType);
            var matDict = m_sortedElements.Voxels[index];

            if (matDict.RenderElementCount == 0)
                return;

            // Technique start
            var shader = GetShader(MyMeshDrawTechnique.VOXEL_MAP);
            SetupShaderPerDraw(shader, MyMeshDrawTechnique.VOXEL_MAP);
            BeginShaderAlternative(shader, MyMeshDrawTechnique.VOXEL_MAP, batchType);
            shader.Begin();

            MyPerformanceCounter.PerCameraDraw.TechniqueChanges[(int)lod]++;

            foreach (var mat in matDict.Voxels)
            {
                var firstElement = mat.Value.FirstOrDefault();
                if (firstElement == null)
                    continue;

                // Setup material
                SetupShaderForMaterialAlternative(shader, batchType, firstElement.VoxelBatch.Material0, firstElement.VoxelBatch.Material1, firstElement.VoxelBatch.Material2);
                MyPerformanceCounter.PerCameraDraw.MaterialChanges[(int)lod]++;
                
                MyEntity lastEntity = null;
                VertexBuffer lastVertexBuffer = null;

                foreach (var renderElement in mat.Value)
                {
                    if (!object.ReferenceEquals(lastVertexBuffer, renderElement.VertexBuffer))
                    {
                        lastVertexBuffer = renderElement.VertexBuffer;
                        m_device.Indices = renderElement.IndexBuffer;
                        m_device.SetStreamSource(0, renderElement.VertexBuffer, 0, renderElement.VertexStride);
                        m_device.VertexDeclaration = renderElement.VertexDeclaration;
                        MyPerformanceCounter.PerCameraDraw.VertexBufferChanges[(int)lod]++;
                        ibChangesStats++;
                    }

                    if (lastEntity != renderElement.Entity)
                    {
                        lastEntity = renderElement.Entity;
                        MyPerformanceCounter.PerCameraDraw.EntityChanges[(int)lod]++;
                        SetupShaderForEntity(shader, renderElement);
                        shader.D3DEffect.CommitChanges();
                    }

                    m_renderProfiler.StartProfilingBlock("DrawIndexedPrimitives");
                    m_device.DrawIndexedPrimitive(PrimitiveType.TriangleList, 0, 0, renderElement.VertexCount, renderElement.IndexStart, renderElement.TriCount);
                    MyPerformanceCounter.PerCameraDraw.TotalDrawCalls++;
                    m_renderProfiler.EndProfilingBlock();
                }
            }

            shader.End();
            // Technique End
        }

        private static void DrawModels(MyLodTypeEnum lod, ref int ibChangesStats)
        {
            for (int i = 0; i < MySortedElements.DrawTechniqueCount; i++)
            {
                var technique = (MyMeshDrawTechnique)i;
                int index = m_sortedElements.GetModelIndex(lod, technique);
                var matDict = m_sortedElements.Models[index];

                if (matDict.RenderElementCount == 0)
                    continue;

                // Technique start
                var shader = GetShader(technique);
                SetupShaderPerDraw(shader, technique);
                BeginShaderAlternative(shader, technique, MyVoxelCacheCellRenderBatchType.SINGLE_MATERIAL /* doesn't care at all */);
                shader.Begin();

                MyPerformanceCounter.PerCameraDraw.TechniqueChanges[(int)lod]++;

                foreach (var mat in matDict.Models)
                {
                    if (mat.Value.RenderElementCount == 0)
                        continue;

                    // Setup material
                    SetupShaderForMaterialAlternative(shader, mat.Key);
                    MyPerformanceCounter.PerCameraDraw.MaterialChanges[(int)lod]++;

                    foreach (var vb in mat.Value.Models)
                    {
                        // Set vb
                        var firstElement = vb.Value.FirstOrDefault();
                        if (firstElement == null)
                            continue;

                        m_device.Indices = firstElement.IndexBuffer;
                        m_device.SetStreamSource(0, firstElement.VertexBuffer, 0, firstElement.VertexStride);
                        m_device.VertexDeclaration = firstElement.VertexDeclaration;
                        MyPerformanceCounter.PerCameraDraw.VertexBufferChanges[(int)lod]++;
                        ibChangesStats++;

                        MyEntity lastEntity = null;

                        foreach (var renderElement in vb.Value)
                        {
                            if (lastEntity != renderElement.Entity)
                            {
                                lastEntity = renderElement.Entity;
                                MyPerformanceCounter.PerCameraDraw.EntityChanges[(int)lod]++;
                                SetupShaderForEntity(shader, renderElement);
                                shader.D3DEffect.CommitChanges();
                            }

                            m_renderProfiler.StartProfilingBlock("DrawIndexedPrimitives");
                            m_device.DrawIndexedPrimitive(PrimitiveType.TriangleList, 0, 0, renderElement.VertexCount, renderElement.IndexStart, renderElement.TriCount);
                            MyPerformanceCounter.PerCameraDraw.TotalDrawCalls++;
                            m_renderProfiler.EndProfilingBlock();
                        }
                    }
                }

                shader.End();
                // Technique End
            }
        }

        static MyEffectBase GetShader(MyMeshDrawTechnique technique)
        {
            switch (technique)
            {
                case MyMeshDrawTechnique.MESH:
                case MyMeshDrawTechnique.DECAL:
                case MyMeshDrawTechnique.HOLO:
                case MyMeshDrawTechnique.ALPHA_MASKED:
                    return GetEffect(MyEffects.ModelDNS) as MyEffectModelsDNS;

                case MyMeshDrawTechnique.VOXELS_DEBRIS:
                    return GetEffect(MyEffects.VoxelDebrisMRT) as MyEffectVoxelsDebris;

                case MyMeshDrawTechnique.VOXEL_MAP:
                    return MyRender.GetEffect(MyEffects.VoxelsMRT) as MyEffectVoxels;

                case MyMeshDrawTechnique.VOXELS_STATIC_ASTEROID:
                    return GetEffect(MyEffects.VoxelStaticAsteroidMRT) as MyEffectVoxelsStaticAsteroid;

                default:
                    throw new MyMwcExceptionApplicationShouldNotGetHere();
            }
        }

        static void SetupShaderForMaterialAlternative(MyEffectBase shader, MyVoxelCacheCellRenderBatchType batchType, MyMwcVoxelMaterialsEnum m0, MyMwcVoxelMaterialsEnum? m1, MyMwcVoxelMaterialsEnum? m2)
        {
            MyEffectVoxels effectVoxels = shader as MyEffectVoxels;

            if (batchType == MyVoxelCacheCellRenderBatchType.SINGLE_MATERIAL)
            {
                effectVoxels.UpdateVoxelTextures(OverrideVoxelMaterial ?? m0);
            }
            else if (batchType == MyVoxelCacheCellRenderBatchType.MULTI_MATERIAL)
            {
                effectVoxels.UpdateVoxelMultiTextures(OverrideVoxelMaterial ?? m0, OverrideVoxelMaterial ?? m1, OverrideVoxelMaterial ?? m2);
            }
        }

        static void SetupShaderForMaterialAlternative(MyEffectBase shader, MyMeshMaterial material)
        {
            switch (material.DrawTechnique)
            {
                case MyMeshDrawTechnique.MESH:
                case MyMeshDrawTechnique.DECAL:
                case MyMeshDrawTechnique.HOLO:
                case MyMeshDrawTechnique.ALPHA_MASKED:
                    {
                        if (material != null)
                        {
                            shader.SetTextureDiffuse(material.DiffuseTexture);
                            shader.SetTextureNormal(material.NormalTexture);

                            //Do we need this? Graphicians dont use this
                            //shader.SetDiffuseColor(material.DiffuseColor);

                            shader.SetSpecularIntensity(material.SpecularIntensity);
                            shader.SetSpecularPower(material.SpecularPower);

                            shader.SetDiffuseUVAnim(material.DiffuseUVAnim);
                            shader.SetEmissivityUVAnim(material.EmissiveUVAnim);

                            shader.SetEmissivityOffset(material.EmissivityOffset);

                            if (material.DrawTechnique == MyMeshDrawTechnique.HOLO)
                            {
                                shader.SetEmissivity(material.HoloEmissivity);
                            }

                            // Commented due 856 - graphicians have to reexport white diffuse colors from MAX
                            //shader.SetDiffuseColor(material.DiffuseColor);
                        }
                        else
                        {
                            shader.SetTextureDiffuse(null);
                            shader.SetTextureNormal(null);

                            shader.SetSpecularPower(1);
                            shader.SetSpecularIntensity(1);

                            //this value is set from object if not from material
                            //shader.SetDiffuseColor(material.DiffuseColor);
                        }

                        if (CheckDiffuseTextures)
                        {
                            if (!shader.IsTextureDiffuseSet())
                            {
                                LazyLoadDebugTextures();

                                shader.SetTextureDiffuse(m_debugTexture);
                                shader.SetDiffuseColor(Vector3.One);
                                shader.SetEmissivity(1);
                            }
                            else
                            {
                                if (material.DrawTechnique != MyMeshDrawTechnique.HOLO)
                                {
                                    shader.SetEmissivity(0);
                                }
                            }
                        }
                        if (CheckNormalTextures)
                        {
                            if (!shader.IsTextureNormalSet())
                            {
                                LazyLoadDebugTextures();

                                shader.SetTextureDiffuse(m_debugTexture);
                                shader.SetEmissivity(1);
                            }
                            else
                            {
                                shader.SetTextureDiffuse(material.NormalTexture);
                                //shader.SetTextureDiffuse(m_debugNormalTexture);
                                shader.SetEmissivity(0);
                            }
                        }

                        if (!shader.IsTextureNormalSet())
                        {
                            LazyLoadDebugTextures();
                            shader.SetTextureNormal(m_debugTexture);
                        }

                    }
                    break;

                case MyMeshDrawTechnique.VOXELS_STATIC_ASTEROID:
                case MyMeshDrawTechnique.VOXELS_DEBRIS:
                    break;

                default:
                    {
                        throw new MyMwcExceptionApplicationShouldNotGetHere();
                    }
            }
        }

        static void BeginShaderAlternative(MyEffectBase shader, MyMeshDrawTechnique technique, MyVoxelCacheCellRenderBatchType batchType)
        {
            switch (technique)
            {
                case MyMeshDrawTechnique.DECAL:
                    {
                        if (shader is MyEffectModelsDNS)
                        {
                            (shader as MyEffectModelsDNS).BeginBlended();
                        }
                    }
                    break;
                case MyMeshDrawTechnique.HOLO:
                    {
                        if (m_currentLodDrawPass != MyLodTypeEnum.LOD_NEAR && !MyRenderConstants.RenderQualityProfile.ForwardRender)
                        {
                            (shader as MyEffectModelsDNS).ApplyHolo(false);
                        }
                        else
                        {
                            (shader as MyEffectModelsDNS).ApplyHolo(true);
                        }
                    }
                    break;
                case MyMeshDrawTechnique.ALPHA_MASKED:
                    {
                        (shader as MyEffectModelsDNS).ApplyMasked();
                    }
                    break;
                case MyMeshDrawTechnique.VOXEL_MAP:
                    {
                        MyEffectVoxels effectVoxels = shader as MyEffectVoxels;
                        if (batchType == MyVoxelCacheCellRenderBatchType.SINGLE_MATERIAL)
                        {
                            effectVoxels.Apply();
                        }
                        else if (batchType == MyVoxelCacheCellRenderBatchType.MULTI_MATERIAL)
                        {
                            effectVoxels.ApplyMultimaterial();
                        }
                        break;
                    }
                case MyMeshDrawTechnique.VOXELS_STATIC_ASTEROID:
                    {
                        ((MyEffectVoxelsStaticAsteroid)shader).Apply();
                    }
                    break;
                case MyMeshDrawTechnique.MESH:
                    {
                        ((MyEffectModelsDNS)shader).SetTechnique(MyRenderConstants.RenderQualityProfile.ModelsRenderTechnique);
                    }
                    break;
                case MyMeshDrawTechnique.VOXELS_DEBRIS:
                    {
                        ((MyEffectVoxelsDebris)shader).SetTechnique(MyRenderConstants.RenderQualityProfile.VoxelsRenderTechnique);
                    }
                    break;
                default:
                    {
                        System.Diagnostics.Debug.Assert(false);
                    }
                    break;
            }
        }
    }
}
