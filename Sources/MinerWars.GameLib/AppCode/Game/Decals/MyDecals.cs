using System.Collections.Generic;
using MinerWarsMath;
using MinerWarsMath.Graphics;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Managers;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Utils.VertexFormats;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.Render;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.Lights;
using System;

//  Decals manager. It holds lists of decal triangles, draws them, removes decals after explosion, etc.
//  I can't use texture atlas for holding all decal textures, because I need clamping, and if using atlas, 
//  texture sampler will get neighbour textures too.
//
//  We have two decal buffers. One for model instances, the other for voxels. Each one manages separate 
//  triangleVertexes buffers. One triangleVertexes buffer for one model/texture or voxel render cell and texture.

namespace MinerWars.AppCode.Game.Decals
{
    enum MyDecalTexturesEnum : byte
    {
        ExplosionSmut,
        BulletHoleOnMetal,
        BulletHoleOnRock
    }

    //  IMPORTANT: This is class, not struct!!!
    //  Reason is, we need to be able to overwrite values inside even if stored in queue, stack or list. If it
    //  was struct, we won't be able to modify it without enque/deque... and that's bad.
    class MyDecalTriangle
    {
        public Vector3 Position0;
        public Vector3 Position1;
        public Vector3 Position2;
        public Vector2 TexCoord0;
        public Vector2 TexCoord1;
        public Vector2 TexCoord2;
        public Vector4 Color0;
        public Vector4 Color1;
        public Vector4 Color2;
        public Vector3 Normal0;
        public Vector3 Normal1;
        public Vector3 Normal2;
        public Vector3 Binormal0;
        public Vector3 Binormal1;
        public Vector3 Binormal2;
        public Vector3 Tangent0;
        public Vector3 Tangent1;
        public Vector3 Tangent2;
        public bool Draw;
        public int RemainingTrianglesOfThisDecal;       //  This number tells us how many triangles of one decal are in the buffer after this triangleVertexes. If zero, this is the last triangleVertexes of a decal.
        public float RandomOffset;

        public MyLight Light;
        public float Emissivity;
        public Vector3 Position;

        public void Start(float lightSize)
        {
            if (lightSize > 0)
            {
                Light = MyLights.AddLight();
                Light.Color = Vector4.One;
                Light.Start(MyLight.LightTypeEnum.PointLight, 1.0f);
                Light.LightOn = lightSize > 0;
                Light.Intensity = 10;
            }
        }

        public void Close()
        {
            if (Light != null)
            {
                MyLights.RemoveLight(Light);
                Light = null;
            }
        }
    }

    static class MyDecals
    {
        static MyVertexFormatDecal[] m_vertices;
        static MyTexture2D[] m_texturesDiffuse;
        static MyTexture2D[] m_texturesNormalMap;
        static List<MyTriangle_Vertex_Normals> m_neighbourTriangles;
        static MyDecalsForPhysObjects m_decalsForModels;
        static MyDecalsForVoxels m_decalsForVoxels;

        static MyDecals()
        {
            MyRender.RegisterRenderModule(MyRenderModuleEnum.Decals, "Decals", Draw, MyRenderStage.LODDrawEnd);
        }

        public static void LoadContent()
        {
            MyMwcLog.WriteLine("MyDecals.LoadContent() - START");
            MyMwcLog.IncreaseIndent();
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyDecals::LoadContent");

            //  Reason is that if count of neighbour triangles is more then decal triangles buffer, we won't be able to add any triangleVertexes to the buffer.
            MyCommonDebugUtils.AssertRelease(MyDecalsConstants.MAX_DECAL_TRIANGLES_IN_BUFFER > MyDecalsConstants.TEXTURE_LARGE_MAX_NEIGHBOUR_TRIANGLES);

            MyCommonDebugUtils.AssertRelease(MyDecalsConstants.MAX_DECAL_TRIANGLES_IN_BUFFER_LARGE <= MyDecalsConstants.MAX_DECAL_TRIANGLES_IN_BUFFER);
            MyCommonDebugUtils.AssertRelease(MyDecalsConstants.MAX_DECAL_TRIANGLES_IN_BUFFER_SMALL <= MyDecalsConstants.MAX_DECAL_TRIANGLES_IN_BUFFER);

            //  Reason is that if count of neighbour triangles is more then decal triangles buffer, we won't be able to add any triangleVertexes to the buffer.
            MyCommonDebugUtils.AssertRelease(MyDecalsConstants.MAX_DECAL_TRIANGLES_IN_BUFFER > MyDecalsConstants.TEXTURE_SMALL_MAX_NEIGHBOUR_TRIANGLES);
            
            //  Reason is that if count of neighbour triangles is more then this fade limit, we won't be able to add decals that lay on more triangles, because buffer will be never released to us.
            MyCommonDebugUtils.AssertRelease(MyDecalsConstants.TEXTURE_LARGE_MAX_NEIGHBOUR_TRIANGLES < (MyDecalsConstants.MAX_DECAL_TRIANGLES_IN_BUFFER * MyDecalsConstants.TEXTURE_LARGE_FADING_OUT_MINIMAL_TRIANGLE_COUNT_PERCENT));

            //  Reason is that if count of neighbour triangles is more then this fade limit, we won't be able to add decals that lay on more triangles, because buffer will be never released to us.
            MyCommonDebugUtils.AssertRelease(MyDecalsConstants.TEXTURE_SMALL_MAX_NEIGHBOUR_TRIANGLES < (MyDecalsConstants.MAX_DECAL_TRIANGLES_IN_BUFFER * MyDecalsConstants.TEXTURE_SMALL_FADING_OUT_MINIMAL_TRIANGLE_COUNT_PERCENT));

            //  Large must be bigger than small
            MyCommonDebugUtils.AssertRelease(MyDecalsConstants.TEXTURE_LARGE_MAX_NEIGHBOUR_TRIANGLES > MyDecalsConstants.TEXTURE_SMALL_MAX_NEIGHBOUR_TRIANGLES);

            m_vertices = new MyVertexFormatDecal[MyDecalsConstants.MAX_DECAL_TRIANGLES_IN_BUFFER * MyDecalsConstants.VERTEXES_PER_DECAL];
            m_neighbourTriangles = new List<MyTriangle_Vertex_Normals>(MyDecalsConstants.TEXTURE_LARGE_MAX_NEIGHBOUR_TRIANGLES);
            
            m_decalsForModels = new MyDecalsForPhysObjects(MyDecalsConstants.DECAL_BUFFERS_COUNT);
            m_decalsForVoxels = new MyDecalsForVoxels(MyDecalsConstants.DECAL_BUFFERS_COUNT);

            //  Decal textures
            int texturesCount = MyEnumsToStrings.Decals.Length;
            m_texturesDiffuse = new MyTexture2D[texturesCount];
            m_texturesNormalMap = new MyTexture2D[texturesCount];
            
            for (int i = 0; i < texturesCount; i++)
            {
                MyMwcLog.WriteLine("textureManager " + i.ToString() + "Textures\\Decals\\" + MyEnumsToStrings.Decals[i] + "_Diffuse", SysUtils.LoggingOptions.MISC_RENDER_ASSETS);
                m_texturesDiffuse[i] = MyTextureManager.GetTexture<MyTexture2D>("Textures\\Decals\\" + MyEnumsToStrings.Decals[i] + "_Diffuse", null, LoadingMode.Immediate);
                MyMwcLog.WriteLine("textureManager " + i.ToString() + "Textures\\Decals\\" + MyEnumsToStrings.Decals[i] + "_NormalMap", SysUtils.LoggingOptions.MISC_RENDER_ASSETS);
                m_texturesNormalMap[i] = MyTextureManager.GetTexture<MyTexture2D>("Textures\\Decals\\" + MyEnumsToStrings.Decals[i] + "_NormalMap", CheckTexture, LoadingMode.Immediate);
                
                MyUtils.AssertTexture(m_texturesNormalMap[i]);
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyDecals.LoadContent() - END");
        }

        /// <summary>
        /// Checks the normal map.
        /// </summary>
        /// <param name="texture">The texture.</param>
        private static void CheckTexture(MyTexture texture)
        {
            MyUtils.AssertTexture((MyTexture2D)texture);

            texture.TextureLoaded -= CheckTexture;
        }

        /// <summary>
        /// Unloads the content.
        /// </summary>
        public static void UnloadContent()
        {
            MyMwcLog.WriteLine("MyDecals.UnloadContent - START");
            MyMwcLog.IncreaseIndent();

            if (m_decalsForModels != null)
                m_decalsForModels.Clear();

            if (m_decalsForVoxels != null)
                m_decalsForVoxels.Clear();

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyDecals.UnloadContent - END");
        }

        //  Add decal and all surounding triangles according to the type of intersection (model or voxel)
        public static void Add(MyDecalTexturesEnum decalTexture, float decalSize, float angle, Vector4 color, bool alphaBlendByAngle,
            ref MyIntersectionResultLineTriangleEx intersection, float lightSize, float emissivity, float decalNormalOffset)
        {
            if (!MyRenderConstants.RenderQualityProfile.EnableDecals)
                return;

            //  Ignore decals too far away
            if (Vector3.Distance(MyCamera.Position, intersection.IntersectionPointInWorldSpace) > (MyDecalsConstants.MAX_DISTANCE_FOR_ADDING_DECALS / MyCamera.Zoom.GetZoomLevel())) 
                return;

            //	Polomer decalu a scale faktor pre vypocet textury.
            //  Decal size is something as radius of a decal, so when converting from real metres to texture space, we need to divide by 2.0
            float decalScale = 1.0f / (2.0f * decalSize);

            // Fix: This is safer way to get right vector.
            Vector3 rightVector;
            MyTriangle_Vertexes triangle = intersection.Triangle.InputTriangle;

            if ((triangle.Vertex0 - intersection.IntersectionPointInObjectSpace).Length() > MyMwcMathConstants.EPSILON)
            {
                rightVector = MyMwcUtils.Normalize(triangle.Vertex0 - intersection.IntersectionPointInObjectSpace);
            }
            else if ((triangle.Vertex1 - intersection.IntersectionPointInObjectSpace).Length() > MyMwcMathConstants.EPSILON)
            {
                rightVector = MyMwcUtils.Normalize(triangle.Vertex1 - intersection.IntersectionPointInObjectSpace);
            }
            else if ((triangle.Vertex2 - intersection.IntersectionPointInObjectSpace).Length() > MyMwcMathConstants.EPSILON)
            {
                rightVector = MyMwcUtils.Normalize(triangle.Vertex2 - intersection.IntersectionPointInObjectSpace);
            }
            else
            {
                System.Diagnostics.Debug.Assert(false, "Normal has zero length! Probably invalid intersection point!");
                return;
            }

            Vector3 upVector = Vector3.Cross(rightVector, intersection.NormalInObjectSpace);

            if (!MyMwcUtils.HasValidLength(upVector))
            {
                //System.Diagnostics.Debug.Assert(false, "Invalid result of cross produt!");
                return;
            }

            upVector = MyMwcUtils.Normalize(upVector);

            //  We create world matrix for the decal and then rotate the matrix, so we can extract rotated right/up vectors/planes for texture coord0 calculations
            Matrix decalMatrix = Matrix.CreateRotationZ(angle) * Matrix.CreateWorld(intersection.IntersectionPointInObjectSpace, intersection.NormalInObjectSpace, upVector);
            
            //	Right plane
            MyPlane rightPlane;
            rightPlane.Point = intersection.IntersectionPointInObjectSpace;
            rightPlane.Normal = MyUtils.GetTransformNormalNormalized(Vector3.Right, ref decalMatrix);

            //	Up plane
            MyPlane upPlane;
            upPlane.Point = intersection.IntersectionPointInObjectSpace;
            upPlane.Normal = MyUtils.GetTransformNormalNormalized(Vector3.Up, ref decalMatrix);

            if (intersection.Entity is MyVoxelMap)
            {
                AddDecalVoxel(decalTexture, decalSize, decalScale, color, alphaBlendByAngle, ref intersection, ref rightPlane, ref upPlane, lightSize, emissivity, decalNormalOffset);
            }
            else
            {
                AddDecalModel(decalTexture, decalSize, decalScale, color, alphaBlendByAngle, ref intersection, ref rightPlane, ref upPlane, lightSize, emissivity, decalNormalOffset);
            }
        }

        //  Add decal and all surounding triangles for voxel intersection
        static void AddDecalVoxel(MyDecalTexturesEnum decalTexture, float decalSize, float decalScale, Vector4 color, bool alphaBlendByAngle,
            ref MyIntersectionResultLineTriangleEx intersection, ref MyPlane rightPlane, ref MyPlane upPlane, float lightSize, float emissivity, float decalNormalOffset)
        {
            MyVoxelMap voxelMap = (MyVoxelMap)intersection.Entity;

            MyMwcVector3Int renderCellCoord = voxelMap.GetVoxelRenderCellCoordinateFromMeters(ref intersection.IntersectionPointInWorldSpace);
            BoundingSphere decalBoundingSphere = new BoundingSphere(intersection.IntersectionPointInWorldSpace, decalSize);

            //  If whole decal can't fit inside of render cell, we won't add any of its triangles. This is because
            //  when hiding/removing triangles after explosion, it is easier to check only one render cell.
            BoundingBox renderCellBoundingBox;
            voxelMap.GetRenderCellBoundingBox(ref renderCellCoord, out renderCellBoundingBox);

            // TODO simon - commented as an experiment. If there are bugs with decals on voxels, remove the comment below
            //if (renderCellBoundingBox.Contains(decalBoundingSphere) != ContainmentType.Contains) return;

            //  If we get null, buffer is full so no new decals can't be placed
            MyDecalsForVoxelsTriangleBuffer decalsBuffer = m_decalsForVoxels.GetTrianglesBuffer(voxelMap, ref renderCellCoord, decalTexture, ref renderCellBoundingBox);
            if (decalsBuffer == null) return;

            //  We need to create decals on neighborhood triangles too, so we check all triangles if they fall in decal's sphere and if yes, we place decal on them.
            //  We check triangles from same voxelmap or model only.

            m_neighbourTriangles.Clear();
            //intersection.VoxelMap.GetTrianglesIntersectingSphere(ref decalBoundingSphere, intersection.TriangleHelperIndex, m_neighbourTriangles, decalsBuffer.MaxNeighbourTriangles);
            voxelMap.GetTrianglesIntersectingSphere(ref decalBoundingSphere, m_neighbourTriangles, decalsBuffer.MaxNeighbourTriangles, false);

            int trianglesToAdd = m_neighbourTriangles.Count;// +1;

            if (trianglesToAdd == 0)
            {
                return;
            }

            if (decalsBuffer.CanAddTriangles(trianglesToAdd) == true)
            {
                var normalSum = CalculateDominantNormal(m_neighbourTriangles);
                normalSum *= decalNormalOffset;

                //  Create decal for every neighbour triangleVertexes
                for (int i = 0; i < m_neighbourTriangles.Count; i++)
                {
                    trianglesToAdd--;

                    var triangle = m_neighbourTriangles[i];
                    triangle.Vertexes.Vertex0 += normalSum;
                    triangle.Vertexes.Vertex1 += normalSum;
                    triangle.Vertexes.Vertex2 += normalSum;
                    m_neighbourTriangles[i] = triangle;

                    decalsBuffer.Add(m_neighbourTriangles[i], intersection.NormalInWorldSpace, ref rightPlane,
                        ref upPlane, decalScale, trianglesToAdd, color, alphaBlendByAngle, lightSize, intersection.IntersectionPointInWorldSpace, emissivity);
                }
            }
        }

        //  Add decal and all surounding triangles for model intersection
        static void AddDecalModel(MyDecalTexturesEnum decalTexture, float decalSize, float decalScale, Vector4 color, bool alphaBlendByAngle, 
            ref MyIntersectionResultLineTriangleEx intersection, ref MyPlane rightPlane, ref MyPlane upPlane, float lightSize, float emissivity, float decalNormalOffset)
        {
            MyDecalsForPhysObjectsTriangleBuffer decalsBuffer = m_decalsForModels.GetTrianglesBuffer(intersection.Entity, decalTexture);

            //  If we get null, buffer is full so no new decals can't be placed
            if (decalsBuffer == null) return;

            //  We need to create decals on neighborhood triangles too, so we check all triangles if they fall in decal's sphere and if yes, we place decal on them.
            //  We check triangles from same voxelmap or model only.

            BoundingSphere decalSphere = new BoundingSphere(intersection.IntersectionPointInObjectSpace, decalSize);
            m_neighbourTriangles.Clear();
            
            intersection.Entity.GetTrianglesIntersectingSphere(ref decalSphere, intersection.NormalInObjectSpace, MyDecalsConstants.MAX_NEIGHBOUR_ANGLE, m_neighbourTriangles, decalsBuffer.MaxNeighbourTriangles);

            int trianglesToAdd = m_neighbourTriangles.Count;

            if (trianglesToAdd == 0)
            {
                return;
            }

            if (decalsBuffer.CanAddTriangles(trianglesToAdd))
            {
                Vector3 normalSum = Vector3.Zero;
                if (MyFakes.USE_DOMINANT_NORMAL_OFFSET_FOR_MODELS)
                {
                    normalSum = CalculateDominantNormal(m_neighbourTriangles);
                    normalSum *= decalNormalOffset;
                }

                //  Create decal for every neighbour triangleVertexes
                for (int i = 0; i < m_neighbourTriangles.Count; i++)
                {
                    trianglesToAdd--;

                    if (MyFakes.USE_DOMINANT_NORMAL_OFFSET_FOR_MODELS)
                    {
                        var triangle = m_neighbourTriangles[i];
                        triangle.Vertexes.Vertex0 += normalSum;
                        triangle.Vertexes.Vertex1 += normalSum;
                        triangle.Vertexes.Vertex2 += normalSum;
                        m_neighbourTriangles[i] = triangle;
                    }

                    decalsBuffer.Add(m_neighbourTriangles[i], intersection.Triangle.InputTriangleNormal,
                        ref rightPlane, ref upPlane, decalScale, decalSize, trianglesToAdd, color, alphaBlendByAngle, lightSize, intersection.IntersectionPointInObjectSpace, emissivity);
                }
            }
        }

        static Vector3 CalculateDominantNormal(List<MyTriangle_Vertex_Normals> triangleVertexNormals)
        {
            Vector3 normalSum = Vector3.Zero;
            for (int i = 0; i < triangleVertexNormals.Count; i++)
            {
                normalSum +=
                    triangleVertexNormals[i].Normals.Normal0 +
                    triangleVertexNormals[i].Normals.Normal1 +
                    triangleVertexNormals[i].Normals.Normal2;
            }

            return MyMwcUtils.Normalize(normalSum);
        }

        //  Blends-out triangles affected by explosion (radius + some safe delta). Triangles there have zero alpha are flaged to not-draw at all.
        public static void HideTrianglesAfterExplosion(MyVoxelMap voxelMap, ref BoundingSphere explosionSphere)
        {
            MyRender.GetRenderProfiler().StartProfilingBlock("MyDecals::HideTrianglesAfterExplosion");
            MyMwcVector3Int renderCellCoord = voxelMap.GetVoxelRenderCellCoordinateFromMeters(ref explosionSphere.Center);
            m_decalsForVoxels.HideTrianglesAfterExplosion(voxelMap.VoxelMapId, ref renderCellCoord, ref explosionSphere);
            MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        /// <summary>
        /// Removes decals from the specified entity (NOT voxel map).
        /// E.g. when the entity is destroyed (destructible prefab).
        /// </summary>
        /// <param name="physObject">The entity from which we want to remove decals. NOT MyVoxelMap!</param>
        public static void RemoveModelDecals(MyEntity physObject)
        {
            m_decalsForModels.ReturnTrianglesBuffer(physObject);
            foreach (var child in physObject.Children)
            {
                RemoveModelDecals(child);
            }
        }

        public static bool IsLargeTexture(MyDecalTexturesEnum decalTexture)
        {
            if (decalTexture == MyDecalTexturesEnum.ExplosionSmut) return true;

            return false;
        }

        public static float GetMaxDistanceForDrawingDecals()
        {
            float zoomLevel = MyCamera.Zoom.GetZoomLevel();
            zoomLevel = System.Math.Max(zoomLevel, MyConstants.FIELD_OF_VIEW_MIN / MyCamera.FieldOfView);
            return zoomLevel > 0 ? MyDecalsConstants.MAX_DISTANCE_FOR_DRAWING_DECALS / zoomLevel : MyDecalsConstants.MAX_DISTANCE_FOR_DRAWING_DECALS;
        }

        public static void Draw()
        {
            if (MyRender.GetCurrentLodDrawPass() == MyLodTypeEnum.LOD0)
            {
                MyStateObjects.DepthStencil_TestFarObject_DepthReadOnly.Apply();
                MyStateObjects.Dynamic_Decals_BlendState.Apply();
                MyStateObjects.BiasedRasterizer_Decals.Apply();

                Effects.MyEffectDecals effect = (Effects.MyEffectDecals)MyRender.GetEffect(MyEffects.Decals);

                //  Draw voxel decals
                m_decalsForVoxels.Draw(m_vertices, effect, m_texturesDiffuse, m_texturesNormalMap);

                //  Draw model decals
                m_decalsForModels.Draw(m_vertices, effect, m_texturesDiffuse, m_texturesNormalMap);
            } 
        }

        public static float UpdateDecalEmissivity(MyDecalTriangle decalTriangle, float alpha, MyEntity entity)
        {
            float emisivity = 0;
            if (decalTriangle.Emissivity > 0)
            {
                //emisivity = (float)(Math.Sin(decalTriangle.RandomOffset + decalTriangle.RandomOffset * MyMinerGame.TotalGamePlayTimeInMilliseconds / 1000.0f)) * 0.4f + 0.7f;

                // 2 seconds default, more emissive lit longer
                float stableLength = 2000 * decalTriangle.Emissivity;
                if ((MyMinerGame.TotalGamePlayTimeInMilliseconds - decalTriangle.RandomOffset) < stableLength)
                    emisivity = 1;
                else
                {
                    emisivity = (float)(500 - (MyMinerGame.TotalGamePlayTimeInMilliseconds - stableLength - decalTriangle.RandomOffset)) / 500.0f;
                    if (emisivity < 0)
                        emisivity = 0;
                }

                emisivity *= decalTriangle.Emissivity;

                if (emisivity > 0)
                {
                    Vector4 color = MyDecalsConstants.PROJECTILE_DECAL_COLOR;

                    Vector3 position;
                    if (entity != null)
                    {
                        position = Vector3.Transform(decalTriangle.Position, entity.WorldMatrix);
                    }
                    else
                    {
                        position = decalTriangle.Position;
                    }

                    MinerWars.AppCode.Game.TransparentGeometry.MyTransparentGeometry.AddPointBillboard(
                            TransparentGeometry.MyTransparentMaterialEnum.DecalGlare,
                            color * alpha,
                           position,
                           1.5f * emisivity,
                           0);                    

                    if (decalTriangle.Light != null)
                    {
                        decalTriangle.Light.Color = color;
                        decalTriangle.Light.SetPosition(position);

                        float range = Math.Max(3 * emisivity * alpha, 0.1f);
                        decalTriangle.Light.Range = range;
                    }
                }
            }

            return emisivity;
        }
    }
}
