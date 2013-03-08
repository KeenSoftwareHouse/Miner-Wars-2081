using System.Collections.Generic;
using MinerWars.AppCode.Game.Effects;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Render;
using MinerWars.AppCode.Game.TransparentGeometry;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.World;
using MinerWars.AppCode.Physics;
using MinerWars.CommonLIB.AppCode.Generics;
using SysUtils.Utils;

using MinerWarsMath;


//  This class is responsible for holding list of dynamic lights, adding, removing and finally drawing on voxels or other models.

namespace MinerWars.AppCode.Game.Lights
{
    static class MyLights
    {
        public static MyDynamicAABBTree m_tree = new MyDynamicAABBTree(MyRender.PrunningExtension);

        static MyObjectsPool<MyLight> m_preallocatedLights = null;
        static List<MyLight> m_sortedLights;
        static MySortLightsByDistanceComparer m_sortLightsComparer = new MySortLightsByDistanceComparer();

        static int m_lightsCount;
        static BoundingSphere m_lastBoundingSphere;

        //  Used to sort lights by their distance to influence bounding sphere
        class MySortLightsByDistanceComparer : IComparer<MyLight>
        {
            public BoundingSphere BoundingSphere;

            public int Compare(MyLight x, MyLight y)
            {
                float xDist, yDist;
                Vector3 xPos = x.Position;
                Vector3 yPos = y.Position;
                Vector3.Distance(ref BoundingSphere.Center, ref xPos, out xDist);
                Vector3.Distance(ref BoundingSphere.Center, ref yPos, out yDist);
                return xDist.CompareTo(yDist);
            }
        }

        //  For SAP comparisions
        class MySortLightsSapComparer : IComparer<MyLight>
        {
            public int Compare(MyLight x, MyLight y)
            {
                return ((int)(x.SapSortValue - y.SapSortValue));
            }
        }

        static MyLights()
        {
            MyRender.RegisterRenderModule(MyRenderModuleEnum.Lights, "Lights", DebugDraw, MyRenderStage.DebugDraw, false);
        }

        public static void LoadData()
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyLights.LoadData");
            MyMwcLog.WriteLine("MyLights.LoadData() - START");
            MyMwcLog.IncreaseIndent();

            if (m_preallocatedLights == null)
            {
                m_preallocatedLights = new MyObjectsPool<MyLight>(MyLightsConstants.MAX_LIGHTS_COUNT);
            }
            m_sortedLights = new List<MyLight>(MyLightsConstants.MAX_LIGHTS_COUNT_WHEN_DRAWING);

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyLights.LoadData() - END");
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        public static void UnloadData()
        {
            // all lights should be deallocated at this point
            MyCommonDebugUtils.AssertDebug(m_preallocatedLights.GetActiveCount() == 0, "MyLights.UnloadData: preallocated lights not emptied!");
            m_preallocatedLights.DeallocateAll();
            if (m_sortedLights != null)
            {
                m_sortedLights.Clear();
                m_sortedLights = null;
            }

            m_tree.Clear();
        }

        //  Add new light to the list, but caller needs to start it using Start() method
        public static MyLight AddLight()
        {
            var result = m_preallocatedLights.Allocate();
            result.ProxyId = MyDynamicAABBTree.NullNode;
            return result;
        }

        public static void RemoveLight(MyLight light)
        {
            if (light.ProxyId != MyDynamicAABBTree.NullNode) // Has been added
            {
                m_tree.RemoveProxy(light.ProxyId);
            }
            light.Clear();
            m_preallocatedLights.Deallocate(light);
        }

        public static void UpdateLightProxy(MyLight light)
        {
            if ((!light.LightOn || light.LightType == MyLight.LightTypeEnum.None) && light.ProxyId != MyDynamicAABBTree.NullNode)
            {
                m_tree.RemoveProxy(light.ProxyId);
                light.ProxyId = MyDynamicAABBTree.NullNode;
            }

            BoundingBox bbox = BoundingBoxHelper.InitialBox;

            if (light.IsTypePoint || light.IsTypeHemisphere)
            {
                bbox = BoundingBox.CreateFromSphere(light.PointBoundingSphere);
            }
            if (light.IsTypeSpot)
            {
                var box = light.SpotBoundingBox;
                BoundingBoxHelper.AddBBox(box, ref bbox);
            }

            if (light.ProxyId == MyDynamicAABBTree.NullNode)
            {
                light.ProxyId = m_tree.AddProxy(ref bbox, light, 0);
            }
            else
            {
                m_tree.MoveProxy(light.ProxyId, ref bbox, Vector3.Zero);
            }
        }

        public static void FrustumQuery(ref BoundingFrustum frustum, List<MyLight> lights)
        {
            m_tree.OverlapAllFrustumAny<MyLight>(ref frustum, lights, false);
        }

        public static void GetLights(IList<MyLight> list)
        {
            foreach (LinkedListNode<MyLight> light in m_preallocatedLights)
            {
                list.Add(light.Value);
            }
        }

        public static List<MyLight> GetSortedLights()
        {
            return m_sortedLights;
        }

        public static int GetActiveLights()
        {
            return m_preallocatedLights.GetActiveCount();
        }

        //  This method must be called on every update. It will sort lights using SAP, so drawing will be faster.
        public static void Update()
        {
            // Tree is updated on change
        }

        public static void UpdateSortedLights(ref BoundingFrustum boundingFrustum)
        {
            m_sortedLights.Clear();
            FrustumQuery(ref boundingFrustum, m_sortedLights);
        }

        public static void UpdateSortedLights(ref BoundingSphere boundingSphere, bool useMaxCount)
        {
            m_tree.OverlapAllBoundingSphere(ref boundingSphere, m_sortedLights, true);
        }

        //  This method adds lights information into specified effect.
        //  Method gets lights that could have influence on bounding sphere (it is assumed this will be bounding sphere of a phys object or voxel render cell).
        //  Lights that are far from bounding sphere are ignored. But near lights are taken to second step, where we sort them by distance and priority
        //  and set them to the effect.
        //  We assume RemoveKilled() was called before this method, so here we don't check if light isn't killed.
        public static void UpdateEffect(MyEffectDynamicLightingBase effect, ref BoundingSphere boundingSphere, bool subtractCameraPosition)
        {
            MyUtils.AssertIsValid(boundingSphere.Center);
            MyUtils.AssertIsValid(boundingSphere.Radius);

            // Reason to remove this condition: when updating effect with same bounding sphere, it could return different result when some light died (effect ended) before second request
            //if (m_lastBoundingSphere != boundingSphere)
            {
                UpdateSortedLights(ref boundingSphere, true);
                m_lastBoundingSphere = boundingSphere;

                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //  Now in 'm_sortedLights' we have only lights that intersects bounding sphere in SAP list
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                m_lightsCount = m_sortedLights.Count;
                int maxLightsForEffect = MyLightsConstants.MAX_LIGHTS_FOR_EFFECT;

                //  If number of lights with influence is more than max number of lights allowed in the effect, we sort them by distance (or we can do it by some priority)
                if (m_sortedLights.Count > maxLightsForEffect)
                {
                    m_sortLightsComparer.BoundingSphere = boundingSphere;
                    m_sortedLights.Sort(m_sortLightsComparer);
                    m_lightsCount = maxLightsForEffect;
                }
            }

            //  Set lights to effect, but not more than effect can handle
            for (int i = 0; i < m_lightsCount; i++)
            {
                SetLightToEffect(effect, i, m_sortedLights[i], subtractCameraPosition);
            }

            effect.SetDynamicLightsCount(m_lightsCount);


            Vector4 sunColor = MySunWind.GetSunColor();
            effect.SetSunColor(new Vector3(sunColor.X, sunColor.Y, sunColor.Z));
            effect.SetDirectionToSun(MyGuiScreenGamePlay.Static.GetDirectionToSunNormalized());
            effect.SetSunIntensity(MySector.SunProperties.SunIntensity);

            Vector3 ambientColor = MyRender.AmbientColor * MyRender.AmbientMultiplier;
            effect.SetAmbientColor(ambientColor * (MyRenderConstants.RenderQualityProfile.ForwardRender ? 6.5f : 1.0f));

        }

        static void SetLightToEffect(MyEffectDynamicLightingBase effect, int index, MyLight light, bool subtractCameraPosition)
        {
            if (subtractCameraPosition == true)
            {
                effect.SetDynamicLightsPosition(index, light.Position - MyCamera.Position);
            }
            else
            {
                effect.SetDynamicLightsPosition(index, light.Position);
            }

            effect.SetDynamicLightsColor(index, light.Color * light.Intensity);
            effect.SetDynamicLightsFalloff(index, light.Falloff);
            effect.SetDynamicLightsRange(index, light.Range);
        }

        public static void UpdateEffectReflector(MyEffectReflectorBase effect, bool subtractCameraPosition)
        {
            Lights.MyLight light = null;
            if (MySession.PlayerShip != null)
                light = MySession.PlayerShip.Light;

            if (light != null && light.ReflectorOn)
            {
                effect.SetReflectorDirection(light.ReflectorDirection);
                effect.SetReflectorConeMaxAngleCos(light.ReflectorConeMaxAngleCos);
                effect.SetReflectorColor(light.ReflectorColor);
                effect.SetReflectorRange(light.ReflectorRange);
            }
            else
            {
                effect.SetReflectorRange(0);
            }

            if (subtractCameraPosition)
                effect.SetCameraPosition(Vector3.Zero);
            else
                effect.SetCameraPosition(MyCamera.Position);
        }

        public static void DebugDraw()
        {
            MyLights.UpdateSortedLights(ref MyCamera.BoundingSphere, false);

            foreach (MyLight light in m_sortedLights)
            {
                //if (light.LightOn && light.Glare.Type == TransparentGeometry.MyLightGlare.GlareTypeEnum.Distant)
                {
                    if ((light.LightType & MyLight.LightTypeEnum.PointLight) != 0)
                    {
                        MyDebugDraw.DrawSphereWireframe(Matrix.CreateScale(light.Range) * Matrix.CreateTranslation(light.PositionWithOffset), new Vector3(1, 0, 0), 1);
                    }
                    if ((light.LightType & MyLight.LightTypeEnum.Hemisphere) != 0)
                    {
                        Matrix rotationHotfix = Matrix.CreateFromAxisAngle(Vector3.UnitX, MathHelper.PiOver2);
                        Matrix world = Matrix.CreateScale(light.Range) * rotationHotfix * Matrix.CreateWorld(light.Position, light.ReflectorDirection, light.ReflectorUp);
                        MyDebugDraw.DrawHemisphereWireframe(world, new Vector3(1, 0, 0), 1);
                    }
                    if ((light.LightType & MyLight.LightTypeEnum.Spotlight) != 0/* && light.ReflectorOn*/)
                    {
                        Vector4 color = Color.Aqua.ToVector4();
                        // MyDebugDraw.DrawAABB(ref bb, ref color, 1.0f);

                        MyDebugDraw.DrawAxis(Matrix.CreateWorld(light.Position, Vector3.Up, Vector3.Forward), 2, 1);
                        MyDebugDraw.DrawSphereWireframe(Matrix.CreateScale(light.Range) * Matrix.CreateTranslation(light.PositionWithOffset), new Vector3(1, 0, 0), 1);

                        // Uncomment to show sphere for spot light
                        //MyDebugDraw.DrawSphereWireframe(Matrix.CreateScale(light.ReflectorRange) * Matrix.CreateTranslation(light.Position), new Vector3(color.X, color.Y, color.Z), 0.25f);
                        //MySimpleObjectDraw.DrawConeForLight();
                        MyStateObjects.WireframeRasterizerState.Apply();
                        SharpDX.Toolkit.Graphics.DepthStencilState.None.Apply();
                        
                        MyDebugDraw.DrawModel(MySimpleObjectDraw.ModelCone, light.SpotWorld, Vector3.One, 1);
                    }
                }
            }
        }
    }
}
