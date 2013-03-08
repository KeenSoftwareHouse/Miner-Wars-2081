using MinerWarsMath;
using System;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.TransparentGeometry;
using MinerWars.AppCode.Game.Render;
using MinerWars.AppCode.Physics;

namespace MinerWars.AppCode.Game.Lights
{

    class MyLight
    {
        //  IMPORTANT: This class isn't realy inicialized by constructor, but by Start()
        //  So don't initialize members here, do it in Start()

        /// <summary>
        /// Light type, flags, could be combined
        /// </summary>
        [Flags]
        public enum LightTypeEnum
        {
            None,
            PointLight = 1 << 0,
            Spotlight = 1 << 1,
            Hemisphere = 1 << 2,
        }

        public enum LightOwnerEnum
        {
            None,
            SmallShip,
            LargeShip,
            Missile
        }

        public int ProxyId = MyDynamicAABBTree.NullNode;

        readonly MyLightGlare m_glare;
        public bool GlareOn;

        public Vector3 Position;
        public Vector3 PositionWithOffset;

        Vector4 m_color;
        Vector3 m_specularColor = Vector3.One;
        float m_falloff;
        float m_range;
        float m_intensity;
        bool m_lightOn;        //  If true, we use the light in lighting calculation. Otherwise it's like turned off, but still in the buffer.
        public bool UseInForwardRender = false;

        LightTypeEnum m_lightType = LightTypeEnum.PointLight; // Type of the light
        LightOwnerEnum m_lightOwner = LightOwnerEnum.None;

        // Reflector parameters are also parameters for spot light
        float m_reflectorIntensity;
        bool m_reflectorOn = false;
        Vector3 m_reflectorDirection;
        Vector3 m_reflectorUp;
        float m_reflectorConeMaxAngleCos;
        Vector4 m_reflectorColor;
        float m_reflectorRange;
        float m_reflectorFalloff;
        MyTexture2D m_reflectorTexture;
        float m_pointLightOffset;

        // Value from 0 to 1 indication light offset in direction of reflector
        // 0 means zero offset, 1 means radius offset
        public float PointLightOffset 
        {
            get 
            { 
                return m_pointLightOffset; 
            }
            set 
            {
                m_pointLightOffset = value;
                UpdatePositionWithOffset();
                m_pointBoundingSphere.Center = PositionWithOffset;
            }
        }

        private bool m_spotParamsDirty = true;
        private BoundingBox m_spotBoundingBox;
        private Matrix m_spotWorld;

        private BoundingSphere m_pointBoundingSphere;

        public float ShadowDistanceSquared = MyLightsConstants.MAX_SPOTLIGHT_SHADOW_RANGE_SQUARED;

        public float SapSortValue;

        private float m_shadowDistance;
        // Should be updated only from inside
        public BoundingSphere PointBoundingSphere { get { return m_pointBoundingSphere; } }

        public BoundingBox SpotBoundingBox
        {
            get
            {
                if (m_spotParamsDirty)
                {
                    UpdateSpotParams();
                }
                return m_spotBoundingBox;
            }
        }

        public Matrix SpotWorld
        {
            get
            {
                if (m_spotParamsDirty)
                {
                    UpdateSpotParams();
                }
                return m_spotWorld;
            }
        }

        public static float ConeRadiansToConeMaxAngleCos(float value)
        {
            return 1 - (float)Math.Cos(value / 2);
        }

        public static float ConeDegreesToConeMaxAngleCos(float value)
        {
            return ConeRadiansToConeMaxAngleCos(MathHelper.ToRadians(value));
        }

        public static float ConeMaxAngleCosToRadians(float reflectorConeMaxAngleCos)
        {
            return (float)Math.Acos(1 - reflectorConeMaxAngleCos) * 2;
        }

        public static float ConeMaxAngleCosToDegrees(float reflectorConeMaxAngleCos)
        {
            return MathHelper.ToDegrees(ConeMaxAngleCosToRadians(reflectorConeMaxAngleCos));
        }


        void UpdatePositionWithOffset()
        {
            PositionWithOffset = Position + ReflectorDirection * m_range * PointLightOffset;
        }


        /// <summary>
        /// Sets reflector cone angle in degrees, minimum is 0, teoretical maximum is PI
        /// </summary>
        public float ReflectorConeRadians
        {
            get
            {
                return ConeMaxAngleCosToRadians(ReflectorConeMaxAngleCos);
            }
            set
            {
                ReflectorConeMaxAngleCos = ConeRadiansToConeMaxAngleCos(value);
            }
        }

        /// <summary>
        /// Sets reflector cone angle in degrees, minimum is 0, teoretical maximum is 180
        /// </summary>
        public float ReflectorConeDegrees
        {
            get
            {
                return ConeMaxAngleCosToDegrees(ReflectorConeMaxAngleCos);
            }
            set
            {
                ReflectorConeMaxAngleCos = ConeDegreesToConeMaxAngleCos(value);
            }
        }

        //  IMPORTANT: This class isn't realy inicialized by constructor, but by Start()
        //  So don't initialize members here, do it in Start()
        public MyLight()
        {
            m_glare = new MyLightGlare(this);
        }

        //  IMPORTANT: This class isn't realy inicialized by constructor, but by Start()
        //  Copies values from 'sourceLight' into this instance
        public void Start(MyLight sourceLight)
        {
            //SysUtils.Utils.MyCommonDebugUtils.AssertDebug((sourceLight.LightType & LightTypeEnum.PointLight) != 0 && ReflectorTexture == null, "ReflectorTexture is null!");
            Start(sourceLight.LightType, sourceLight.Position, sourceLight.Color, sourceLight.Falloff, sourceLight.Range);
        }

        //  IMPORTANT: This class isn't realy inicialized by constructor, but by Start()
        private void Start()
        {
            m_glare.Start();
            m_reflectorRange = 1;
            m_reflectorUp = Vector3.Up;
            m_reflectorDirection = Vector3.Forward;
            LightOn = true;
            Intensity = 1.0f;
            UseInForwardRender = false;
            m_spotParamsDirty = true;
            GlareOn = false;
            PointLightOffset = 0;
          
            m_shadowDistance = MyLightsConstants.MAX_SPOTLIGHT_SHADOW_RANGE;

            UpdateLight();
        }

        private void UpdateLight()
        {
            MyLights.UpdateLightProxy(this);
        }

        public void Start(LightTypeEnum lightType, float falloff)
        {
            Start();
            LightType = lightType;

            if ((lightType & MyLight.LightTypeEnum.PointLight) != 0)
            {
                Falloff = falloff;
                PointOn = true;
            }

            if ((lightType & MyLight.LightTypeEnum.Hemisphere) != 0)
            {
                Falloff = falloff;
                PointOn = true;
            }

            if ((lightType & MyLight.LightTypeEnum.Spotlight) != 0)
            {
                ReflectorFalloff = falloff;
                ReflectorOn = true;
            }

            LightOwner = LightOwnerEnum.None;
        }

        // Can be called only from MyLights.RemoveLight
        public void Clear()
        {
            m_glare.Clear();
            ProxyId = MyDynamicAABBTree.NullNode;
        }

        public bool IsPointLightInFrustum()
        {
            return MyCamera.IsInFrustum(ref m_pointBoundingSphere);
        }

        public bool IsSpotLightInFrustum()
        {
            if (m_spotParamsDirty)
            {
                UpdateSpotParams();
            }
            return MyCamera.IsInFrustum(ref m_spotBoundingBox);
        }

        public void Start(LightTypeEnum lightType, Vector4 color, float falloff, float range)
        {
            Start(lightType, falloff);
            Color = color;
            Range = range;
        }

        //  IMPORTANT: This class isn't realy inicialized by constructor, but by Start()
        public void Start(LightTypeEnum lightType, Vector3 position, Vector4 color, float falloff, float range)
        {
            Start(lightType, color, falloff, range);
            SetPosition(position);
        }

        /// <summary>
        /// When setting Reflector properties, use this function to test whether properties are in bounds and light AABB is not too large.
        /// Properties which affects calculations are ReflectorRange and ReflectorConeMaxAngleCos (ReflectorConeDegrees, ReflectorConeRadians)
        /// </summary>
        /// <returns></returns>
        public bool SpotlightNotTooLarge(float reflectorConeMaxAngleCos, float reflectorRange)
        {
            return reflectorConeMaxAngleCos <= MyLightsConstants.MAX_SPOTLIGHT_ANGLE_COS && reflectorRange <= MyLightsConstants.MAX_SPOTLIGHT_RANGE;

            //BoundingBox bbox = new BoundingBox();
            //float scaleXY, scaleZ;

            //// Up and direction are both used worst possible
            //Vector3 direction = MyMwcUtils.Normalize(new Vector3(1, 1, 1));
            //Vector3 help = new Vector3(1, 0, 0);
            //Vector3 up = MyMwcUtils.Normalize(Vector3.Cross(direction, help));

            //CalculateAABB(ref bbox, out scaleXY, out scaleZ, direction, up, reflectorConeMaxAngleCos, reflectorRange);
            //return TestAABB(ref bbox);
        }

        //private bool TestAABB(ref BoundingBox bbox)
        //{
        //    return (bbox.Max - bbox.Min).Length() < MyLightsConstants.MAX_SPOTLIGHT_AABB_DIAGONAL;
        //}

        private static void CalculateAABB(ref BoundingBox bbox, out float scaleZ, out float scaleXY, ref Vector3 position, ref Vector3 direction, ref Vector3 up, float reflectorConeMaxAngleCos, float reflectorRange)
        {
            float cosAngle = 1 - reflectorConeMaxAngleCos;
            scaleZ = reflectorRange;
            // Calculate cone side (hypotenuse of triangle)
            float side = reflectorRange / cosAngle;
            // Calculate cone bottom scale (Pythagoras theorem)
            scaleXY = (float)System.Math.Sqrt(side * side - reflectorRange * reflectorRange) * 2;

            up = MyMwcUtils.Normalize(up);
            Vector3 coneSideDirection = Vector3.Cross(up, direction);
            coneSideDirection = MyMwcUtils.Normalize(coneSideDirection);
            Vector3 coneCenter = position + direction * scaleZ;
            Vector3 pt1 = coneCenter + coneSideDirection * scaleXY / 2 + up * scaleXY / 2;
            Vector3 pt2 = coneCenter - coneSideDirection * scaleXY / 2 + up * scaleXY / 2;
            Vector3 pt3 = coneCenter + coneSideDirection * scaleXY / 2 - up * scaleXY / 2;
            Vector3 pt4 = coneCenter - coneSideDirection * scaleXY / 2 - up * scaleXY / 2;

            bbox = bbox.CreateInvalid();
            bbox = bbox.Include(ref position);
            //bbox = bbox.Include(ref coneCenter);
            bbox = bbox.Include(ref pt1);
            bbox = bbox.Include(ref pt2);
            bbox = bbox.Include(ref pt3);
            bbox = bbox.Include(ref pt4);
        }

        private void UpdateSpotParams()
        {
            float scaleZ, scaleXY;
            CalculateAABB(ref m_spotBoundingBox, out scaleZ, out scaleXY, ref Position, ref m_reflectorDirection, ref m_reflectorUp, ReflectorConeMaxAngleCos, ReflectorRange);
            m_spotWorld = Matrix.CreateScale(scaleXY, scaleXY, scaleZ) * Matrix.CreateWorld(Position, ReflectorDirection, ReflectorUp);
            m_spotParamsDirty = false;
        }

        /// <summary>
        /// Use when setting both values and previous state of both value is undefined
        /// </summary>
        /// <param name="reflectorConeMaxAngleCos"></param>
        /// <param name="range"></param>
        public void UpdateReflectorRangeAndAngle(float reflectorConeMaxAngleCos, float reflectorRange)
        {
            SpotlightNotTooLarge(reflectorConeMaxAngleCos, reflectorRange);
            m_reflectorRange = reflectorRange;
            m_reflectorConeMaxAngleCos = reflectorConeMaxAngleCos;
        }

        /// <summary>
        /// Draws things associated with this light, for example glare.
        /// </summary>
        public void Draw()
        {
            if (GlareOn)
            {
                Glare.Draw();
            }
        }

        #region Properties

        public bool IsTypePoint
        {
            get
            {
                return (LightType & MyLight.LightTypeEnum.PointLight) != 0 && !IsTypeHemisphere;
            }
        }

        public bool IsTypeHemisphere
        {
            get
            {
                return (LightType & MyLight.LightTypeEnum.Hemisphere) != 0;
            }
        }

        public bool IsTypeSpot
        {
            get
            {
                return (LightType & MyLight.LightTypeEnum.Spotlight) != 0;
            }
        }

        /*
        public Vector3 Position
        {
            get
            {
                return Position;
            }
            
            set
            {
                if (Position != value)
                {
                    MinerWars.AppCode.Game.Utils.MyUtils.AssertIsValid(value);
                    Position = value;
                    m_spotParamsDirty = true;
                    m_pointBoundingSphere.Center = PositionWithOffset;
                    SapSortValue = value.X - m_range;

                    UpdateLight();
                }
            } 
        }
              */
        public void SetPosition(Vector3 position)
        {
            if (Vector3.DistanceSquared(Position, position) > 0.0001f)
            {
                MinerWars.AppCode.Game.Utils.MyUtils.AssertIsValid(position);
                Position = position;
                m_spotParamsDirty = true;
                UpdatePositionWithOffset();
                m_pointBoundingSphere.Center = PositionWithOffset;
                SapSortValue = position.X - m_range;

                UpdateLight();
            }
        }

        public Vector4 Color
        {
            get { return m_color; }
            set { m_color = value; }
        }

        public Vector3 SpecularColor
        {
            get { return m_specularColor; }
            set { m_specularColor = value; }
        }

        /// <summary>
        /// Exponential falloff (1 = linear, 2 = quadratic, etc)
        /// </summary>
        public float Falloff
        {
            get { return m_falloff; }
            set { m_falloff = value; }
        }

        public float Range
        {
            get
            {
                System.Diagnostics.Debug.Assert(m_range > 0, "Point light range not set");
                return m_range;
            }
            set
            {
                if (m_range != value)
                {
                    System.Diagnostics.Debug.Assert(value > 0, "Cannot set zero point light range");
                    System.Diagnostics.Debug.Assert(value <= MyLightsConstants.MAX_POINTLIGHT_RADIUS, "Cannot set point light range bigger than MyLightsConstants.MAX_POINTLIGHT_RADIUS");
                    m_range = value;
                    m_pointBoundingSphere.Radius = value;
                    SapSortValue = Position.X - value;

                    UpdateLight();
                }
            }
        }

        public float Intensity
        {
            get { return m_intensity; }
            set { m_intensity = value; }
        }

        public bool LightOn        //  If true, we use the light in lighting calculation. Otherwise it's like turned off, but still in the buffer.
        {
            get { return m_lightOn; }
            set { if (m_lightOn != value) { m_lightOn = value; UpdateLight(); } }
        }

        public bool PointOn { get; set; }

        public LightTypeEnum LightType
        {
            get { return m_lightType; }
            set { m_lightType = value; }
        }

        public LightOwnerEnum LightOwner
        {
            get { return m_lightOwner; }
            set { m_lightOwner = value; }
        }

        // Reflector parameters are also parameters for spot light
        public float ReflectorIntensity
        {
            get { return m_reflectorIntensity; }
            set { m_reflectorIntensity = value; }
        }

        public bool ReflectorOn
        {
            get { return m_reflectorOn; }
            set { m_reflectorOn = value; }
        }

        public Vector3 ReflectorDirection
        {
            get { return m_reflectorDirection; }
            set {
                if (Vector3.DistanceSquared(m_reflectorDirection, value) > 0.00001f)
                {
                    m_reflectorDirection = value;
                    m_spotParamsDirty = true;
                }
            }
        }

        public Vector3 ReflectorUp
        {
            get { return m_reflectorUp; }
            set
            {
                if (Vector3.DistanceSquared(m_reflectorUp, value) > 0.00001f)
                {
                    m_reflectorUp = value; 
                    m_spotParamsDirty = true;
                }
            }
        }

        public float ReflectorConeMaxAngleCos
        {
            get { return m_reflectorConeMaxAngleCos; }
            set
            {
                System.Diagnostics.Debug.Assert(SpotlightNotTooLarge(value, m_reflectorRange), "Spot light is too large, reduce range or cone angle, AABB diagonal size must be smaller than 2500m");
                m_reflectorConeMaxAngleCos = value;
                m_spotParamsDirty = true;
            }
        }

        public Vector4 ReflectorColor
        {
            get { return m_reflectorColor; }
            set { m_reflectorColor = value; }
        }

        public float ReflectorRange
        {
            get
            {
                System.Diagnostics.Debug.Assert(m_reflectorRange > 0, "Spot light range not set");
                return m_reflectorRange;
            }
            set
            {
                System.Diagnostics.Debug.Assert(value > 0, "Cannot set spot light range to zero");
                System.Diagnostics.Debug.Assert(SpotlightNotTooLarge(m_reflectorConeMaxAngleCos, value), "Spot light is too large, reduce range or cone angle, AABB diagonal size must be smaller than 2500m");
                m_reflectorRange = value;
                m_spotParamsDirty = true;

                UpdateLight();
            }
        }

        public float ReflectorFalloff
        {
            get
            {
                return m_reflectorFalloff;
            }
            set
            {
                m_reflectorFalloff = value;
            }
        }

        public MyTexture2D ReflectorTexture
        {
            get { return m_reflectorTexture; }
            set { m_reflectorTexture = value; }
        }

        public MyLightGlare Glare { get { return m_glare; } }


        public float ShadowDistance
        {
            get { return m_shadowDistance; }
            set{ m_shadowDistance = value; }
        }

        #endregion
    }
}
