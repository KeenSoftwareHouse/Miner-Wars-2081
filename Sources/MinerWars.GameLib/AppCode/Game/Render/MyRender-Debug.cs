#region Using

using System.Collections.Generic;
using SysUtils.Utils;

using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Textures;
using System.Linq;

using SharpDX;
using SharpDX.Direct3D9;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

#endregion

namespace MinerWars.AppCode.Game.Render
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

    static partial class MyRender
    {
        #region Debug render

        public static void EntitiesDebugDraw()
        {
            //if (MyMwcFinalBuildConstants.DrawHelperPrimitives)
            {
                foreach (MyEntity entity in m_entitiesToDebugDraw)
                {
                    entity.DebugDraw();
                }
            }
        }

        internal static void DrawDebugEnvironmentRenderTargets()
        {
            BlendState.Opaque.Apply();

            int cubeSize = GetRenderTargetCube(MyRenderTargets.EnvironmentCube).GetLevelDescription(0).Width;
            cubeSize = 128;

            MyMwcVector2Int delta = new MyMwcVector2Int((int)(MyCamera.Viewport.Height * 0.07f), (int)(MyCamera.Viewport.Height * 0.015f));
            MyMwcVector2Int size = new MyMwcVector2Int(cubeSize, cubeSize);

            int heightOffset = size.Y + delta.Y;

            for (int i = 0; i < 6; i++)
            {
                //var back = MyTextureManager.GetTexture<MyTextureCube>("Textures\\BackgroundCube\\Final\\TestCube", null, MinerWars.AppCode.Game.Managers.LoadingMode.Immediate);
                MyGuiManager.DrawSpriteFast(GetRenderTargetCube(MyRenderTargets.EnvironmentCube), (CubeMapFace)i, delta.X + size.X * i, delta.Y, size.X, size.Y, Color.White);
                MyGuiManager.DrawSpriteFast(GetRenderTargetCube(MyRenderTargets.EnvironmentCubeAux), (CubeMapFace)i, delta.X + size.X * i, delta.Y + heightOffset, size.X, size.Y, Color.White);
                MyGuiManager.DrawSpriteFast(GetRenderTargetCube(MyRenderTargets.AmbientCube), (CubeMapFace)i, delta.X + size.X * i, delta.Y + heightOffset * 2, size.X, size.Y, Color.White);
                MyGuiManager.DrawSpriteFast(GetRenderTargetCube(MyRenderTargets.AmbientCubeAux), (CubeMapFace)i, delta.X + size.X * i, delta.Y + heightOffset * 3, size.X, size.Y, Color.White);
            }
        }

        internal static void DrawDebugBlendedRenderTargets()
        {
            BlendState.Opaque.Apply();

            //  All RT should be of same size, so for size we can use any of them we just pick up depthRT
            float renderTargetAspectRatio = (float)MyCamera.Viewport.Width / (float)MyCamera.Viewport.Height;

            float normalizedSizeY = 0.40f;
            //float normalizedSizeY = MyCamera.Viewport.Height / 1920f;
            float normalizedSizeX = normalizedSizeY * renderTargetAspectRatio;

            MyMwcVector2Int delta = new MyMwcVector2Int((int)(MyCamera.Viewport.Height * 0.015f), (int)(MyCamera.Viewport.Height * 0.015f));
            MyMwcVector2Int size = new MyMwcVector2Int((int)(MyCamera.Viewport.Height * normalizedSizeX), (int)(MyCamera.Viewport.Height * normalizedSizeY));

            MyGuiManager.DrawSpriteFast(MyRender.GetRenderTarget(MyRenderTargets.Diffuse), delta.X, delta.Y, size.X, size.Y, Color.White);
            MyGuiManager.DrawSpriteFast(MyRender.GetRenderTarget(MyRenderTargets.Normals), delta.X + size.X + delta.X, delta.Y, size.X, size.Y, Color.White);
        }

        internal static void DrawDebugHDRRenderTargets()
        {
            BlendState.Opaque.Apply();

            //  All RT should be of same size, so for size we can use any of them we just pick up depthRT
            float renderTargetAspectRatio = (float)MyCamera.Viewport.Width / (float)MyCamera.Viewport.Height;

            float normalizedSizeY = 0.40f;
            float normalizedSizeX = normalizedSizeY * renderTargetAspectRatio;

            MyMwcVector2Int delta = new MyMwcVector2Int((int)(MyCamera.Viewport.Height * 0.015f), (int)(MyCamera.Viewport.Height * 0.015f));
            MyMwcVector2Int size = new MyMwcVector2Int((int)(MyCamera.Viewport.Height * normalizedSizeX), (int)(MyCamera.Viewport.Height * normalizedSizeY));

            //MyGuiManager.DrawSpriteFast(MyRender.GetRenderTarget(MyRenderTargets.Diffuse), delta.X, delta.Y, size.X, size.Y, Color.White);
            //MyGuiManager.DrawSpriteFast(MyRender.GetRenderTarget(MyRenderTargets.Normals), delta.X + size.X + delta.X, delta.Y, size.X, size.Y, Color.White);
        }

        internal static void DrawDebug()
        {
            m_renderProfiler.StartProfilingBlock("Draw entity debug");

            RasterizerState.CullNone.Apply();
            DepthStencilState.Default.Apply();
            //DepthStencilState.None.Apply();
            BlendState.Opaque.Apply();

            EntitiesDebugDraw();

            GetShadowRenderer().DebugDraw();

            if (ShowEnhancedRenderStatsEnabled)
                ShowEnhancedRenderStats();
            //if (ShowResourcesStatsEnabled)
              //  MyMinerGame.GraphicsDeviceManager.DebugDrawStatistics();
            //if (ShowTexturesStatsEnabled)
              //  MyTextureManager.DebugDrawStatistics();

            m_renderProfiler.EndProfilingBlock();
        }

        class MyTypeStats
        {
            public int Count;
            public int Tris;
            public object UserData;
            public string UserString;
        }

        static Dictionary<string, MyTypeStats> m_prefabStats = new Dictionary<string, MyTypeStats>();
        static Dictionary<string, MyTypeStats> m_typesStats = new Dictionary<string, MyTypeStats>();

        public static void ClearEnhancedStats()
        {
            m_typesStats.Clear();
            m_prefabStats.Clear();
        }

        static private void ShowEnhancedRenderStats()
        {
            ClearEnhancedStats();

            //m_renderObjectListForDraw.Clear();
            //m_shadowPrunningStructure.OverlapAllFrustum(ref m_cameraFrustum, m_renderObjectListForDraw);
            //m_cameraFrustumBox = new BoundingBox(new Vector3(float.NegativeInfinity), new Vector3(float.PositiveInfinity));
            //m_shadowPrunningStructure.OverlapAllBoundingBox(ref m_cameraFrustumBox, m_renderObjectListForDraw);

            foreach (MyRenderObject ro in m_renderObjectListForDraw)
            {
                string ts = ro.Entity.GetType().Name.ToString();
                if (!m_typesStats.ContainsKey(ts))
                    m_typesStats.Add(ts, new MyTypeStats());
                m_typesStats[ts].Count++;
            }
            

            float topOffset = 100;
            Vector2 offset = new Vector2(100, topOffset);
            MyDebugDraw.DrawText(offset, new System.Text.StringBuilder("Detailed render statistics"), Color.Yellow, 2);

            float scale = 0.7f;
            offset.Y += 50;
            MyDebugDraw.DrawText(offset, new System.Text.StringBuilder("Prepared entities for draw:"), Color.Yellow, scale);
            offset.Y += 30;
            foreach (var pair in SortByCount(m_typesStats))
            {
                MyDebugDraw.DrawText(offset, new System.Text.StringBuilder(pair.Key + ": " + pair.Value.Count.ToString() + "x"), Color.Yellow, scale);
                offset.Y += 20;
            }

            offset = new Vector2(400, topOffset + 50);
            scale = 0.6f;
            MyDebugDraw.DrawText(offset, new System.Text.StringBuilder("Prepared prefabs for draw:"), Color.Yellow, 0.7f);
            offset.Y += 30;
            foreach (var pair in SortByCount(m_prefabStats))
            {
                MyDebugDraw.DrawText(offset, new System.Text.StringBuilder(pair.Key + ": " + pair.Value.Count.ToString() + "x"), Color.Yellow, scale);
                offset.Y += 14;
            }


            ClearEnhancedStats();
            foreach (MyRenderObject ro in m_debugRenderObjectListForDrawLOD0)
            {
                string pt = ro.Entity.GetType().Name.ToString();
                if (!m_prefabStats.ContainsKey(pt))
                    m_prefabStats.Add(pt, new MyTypeStats());

                m_prefabStats[pt].Count++;
                m_prefabStats[pt].Tris += ro.Entity.ModelLod0.GetTrianglesCount();
            }

            offset = new Vector2(800, topOffset + 50);
            scale = 0.6f;
            MyDebugDraw.DrawText(offset, new System.Text.StringBuilder("Prepared entities for LOD0:"), Color.Yellow, 0.7f);
            offset.Y += 30;
            foreach (var pair in SortByCount(m_prefabStats))
            {
                MyDebugDraw.DrawText(offset, new System.Text.StringBuilder(pair.Key + ": " + pair.Value.Count.ToString() + "x [" + pair.Value.Tris.ToString() + " tris]"), Color.Yellow, scale);
                offset.Y += 14;
            }

            ClearEnhancedStats();
            foreach (MyRenderObject ro in m_debugRenderObjectListForDrawLOD1)
            {
                string pt = ro.Entity.GetType().Name.ToString();
                if (!m_prefabStats.ContainsKey(pt))
                    m_prefabStats.Add(pt, new MyTypeStats());

                m_prefabStats[pt].Count++;
                if (ro.Entity.ModelLod1 != null)
                {
                    m_prefabStats[pt].Tris += ro.Entity.ModelLod1.GetTrianglesCount();
                }
            }

            offset = new Vector2(1200, topOffset + 50);
            scale = 0.6f;
            MyDebugDraw.DrawText(offset, new System.Text.StringBuilder("Prepared entities for LOD1:"), Color.Yellow, 0.7f);
            offset.Y += 30;
            foreach (var pair in SortByCount(m_prefabStats))
            {
                MyDebugDraw.DrawText(offset, new System.Text.StringBuilder(pair.Key + ": " + pair.Value.Count.ToString() + "x [" + pair.Value.Tris.ToString() + " tris]"), Color.Yellow, scale);
                offset.Y += 14;
            }

        }

        static public void DumpAllEntities()
        {
            m_typesStats.Clear();

            MyMwcLog.WriteLine("Dump of all loaded prefabs");
            MyMwcLog.WriteLine("");

            foreach (MyEntity entity in MyEntities.GetEntities())
            {
                MyPrefabContainer container = entity as MyPrefabContainer;

                if (container != null)
                {
                    foreach (var prefab in container.GetPrefabs())
                    {
                        string ts = prefab.ModelLod0.AssetName.ToString();
                        if (!m_typesStats.ContainsKey(ts))
                            m_typesStats.Add(ts, new MyTypeStats());
                        m_typesStats[ts].Count++;
                        m_typesStats[ts].Tris += prefab.ModelLod0.Triangles.Length;
                        m_typesStats[ts].UserData = prefab;
                        m_typesStats[ts].UserString += prefab.EntityId.Value.NumericValue + " ";
                    }
                }
            }


            foreach (var po in SortByCount(m_typesStats))
            {
                var prefab = ((MinerWars.AppCode.Game.Prefabs.MyPrefabBase)po.Value.UserData);
                int verticesSize = prefab.ModelLod0.GetVBSize + prefab.ModelLod0.GetIBSize;

                MyMwcLog.WriteLine(po.Key + "," + po.Value.Count + "," + po.Value.Tris + "," + verticesSize + "," + prefab.ModelLod0.GetBVHSize() + "," + po.Value.UserString);
            }
        }

        static public List<KeyValuePair<string, int>> SortByValue(Dictionary<string, int> stats)
        {
            List<KeyValuePair<string, int>> statsList = stats.ToList();
            statsList.Sort(
                delegate(KeyValuePair<string, int> firstPair,
                KeyValuePair<string, int> nextPair)
                {
                    return nextPair.Value.CompareTo(firstPair.Value);
                }
            );

            return statsList;
        }

        static List<KeyValuePair<string, MyTypeStats>> SortByCount(Dictionary<string, MyTypeStats> stats)
        {
            List<KeyValuePair<string, MyTypeStats>> statsList = stats.ToList();
            statsList.Sort(
                delegate(KeyValuePair<string, MyTypeStats> firstPair,
                KeyValuePair<string, MyTypeStats> nextPair)
                {
                    return nextPair.Value.Count.CompareTo(firstPair.Value.Count);
                }
            );

            return statsList;
        }


        #endregion
    }
}