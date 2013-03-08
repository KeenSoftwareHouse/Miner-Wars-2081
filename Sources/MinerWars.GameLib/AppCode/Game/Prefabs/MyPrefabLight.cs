using System;
using System.Collections.Generic;
using MinerWarsMath;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Lights;
using MinerWars.AppCode.Game.Models;
using MinerWars.AppCode.Game.Prefabs;
using MinerWars.AppCode.Game.TransparentGeometry;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.Prefabs;
using MinerWars.CommonLIB.AppCode.Utils;

using MinerWars.AppCode.Game.Localization;
using System.Text;
using System.Diagnostics;
using MinerWars.AppCode.Game.Render;
using MinerWars.CommonLIB.AppCode.Import;

namespace MinerWars.AppCode.Game.Entities.SubObjects
{
    class MyPrefabLight : MyPrefabBase
    {
        MyLight m_light; //here beter to use base light class then fixed light type... when we add more light types
        Matrix m_pointLocalMatrix;
        //public MyLightPrefabTypeEnum m_Type = MyLightPrefabTypeEnum.NOT_ASSIGNED;

        // Offsets flashing of light
        public float FlashOffset;

        MyLightEffectTypeEnum m_effect;
        public MyLightEffectTypeEnum Effect
        {
            get { return m_effect; }
            set
            {
                m_effect = value;
                UpdateEffect();
                RecheckNeedsUpdate();
            }
        }




        public void UpdateEffect()
        {
            switch (m_effect)
            {
                case MyLightEffectTypeEnum.NORMAL:
                    m_light.Intensity = m_IntensityMax;
                    m_light.Glare.Intensity = null;
                    m_light.Glare.Type = MyLightGlare.GlareTypeEnum.Normal;
                    m_light.ReflectorIntensity = ReflectorIntensityMax;
                    break;
                case MyLightEffectTypeEnum.CONSTANT_FLASHING:
                case MyLightEffectTypeEnum.RANDOM_FLASHING:                    
                    m_light.Glare.Type = MyLightGlare.GlareTypeEnum.Normal;
                    break;
                case MyLightEffectTypeEnum.DISTANT_GLARE:
                    m_light.Glare.Intensity = 1;                 
                    m_light.GlareOn = true;
                    m_light.Glare.Type = MyLightGlare.GlareTypeEnum.Distant;

                    break;
                case MyLightEffectTypeEnum.DISTANT_GLARE_FLASHING:
                case MyLightEffectTypeEnum.DISTANT_GLARE_RANDOM_FLASHING:                    
                    m_light.GlareOn = true;
                    m_light.Glare.Type = MyLightGlare.GlareTypeEnum.Distant;

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        public float m_IntensityMax; //fore effects
        public float ReflectorIntensityMax;
        int m_lastBlickChange;
        int m_lastSpotBlickChange;
        MyPrefabConfigurationLight m_prefabLightConfig;

        private const float WITHOUT_ELECTRICITY_INTENSITY_MULTIPLICATOR = 0.1f;

        public MyPrefabLight(MyPrefabContainer owner): base(owner) { }

        public MyLight GetLight()
        {
            return m_light;
        }

        protected override void WorkingChanged()
        {
            base.WorkingChanged();
            if (m_light != null)
            {
                //m_light.LightOn = IsWorking();
                m_light.LightOn = Enabled;
                if (IsElectrified())
                {
                    m_light.Intensity = m_IntensityMax;
                    //m_light.ReflectorIntensity = ReflectorIntensityMax;
                    //m_light.Glare.Intensity = m_IntensityMax;
                }
                else 
                {
                    m_light.Intensity = m_IntensityMax * WITHOUT_ELECTRICITY_INTENSITY_MULTIPLICATOR;
                    //m_light.ReflectorIntensity = ReflectorIntensityMax * WITHOUT_ELECTRICITY_INTENSITY_MULTIPLICATOR;
                    //m_light.Glare.Intensity = m_IntensityMax * WITHOUT_ELECTRICITY_INTENSITY_MULTIPLICATOR;
                }
            }
        }

        public static StringBuilder GetStringFromMyLightEffectTypeEnum(MyLightEffectTypeEnum t)
        {
            switch (t)
            {
                case MyLightEffectTypeEnum.NORMAL:
                    return MyTextsWrapper.Get(MyTextsWrapperEnum.Normal);
                case MyLightEffectTypeEnum.CONSTANT_FLASHING:
                    return MyTextsWrapper.Get(MyTextsWrapperEnum.ConstantFlashing);
                case MyLightEffectTypeEnum.RANDOM_FLASHING:
                    return MyTextsWrapper.Get(MyTextsWrapperEnum.RandomFlashing);
                case MyLightEffectTypeEnum.DISTANT_GLARE:
                    return MyTextsWrapper.Get(MyTextsWrapperEnum.DistantGlare);
                case MyLightEffectTypeEnum.DISTANT_GLARE_FLASHING:
                    return MyTextsWrapper.Get(MyTextsWrapperEnum.DistantGlareFlashing);
                case MyLightEffectTypeEnum.DISTANT_GLARE_RANDOM_FLASHING:
                    return MyTextsWrapper.Get(MyTextsWrapperEnum.DistantGlareRandomFlashing);
                default:
                    throw new IndexOutOfRangeException();
            }
        }


        public static StringBuilder GetStringFromMyLightPrefabTypeEnum(MyLightPrefabTypeEnum t)
        {
            switch (t)
            {
                case MyLightPrefabTypeEnum.NOT_ASSIGNED:
                    return MyTextsWrapper.Get(MyTextsWrapperEnum.None);
                case MyLightPrefabTypeEnum.POINT_LIGHT:
                    return MyTextsWrapper.Get(MyTextsWrapperEnum.PointLight);
                case MyLightPrefabTypeEnum.SPOT_LIGHT:
                    return MyTextsWrapper.Get(MyTextsWrapperEnum.SpotLight);
                case MyLightPrefabTypeEnum.HEMISPHERIC_LIGHT:
                    return MyTextsWrapper.Get(MyTextsWrapperEnum.HemisphericLight);
                default:
                    System.Diagnostics.Debug.Assert(false);
                    return new StringBuilder();
            }
        }        

        protected override void InitPrefab(string displayName, Vector3 relativePosition, Matrix localOrientation, MyMwcObjectBuilder_PrefabBase objectBuilder, MyPrefabConfiguration prefabConfig)
        {

            m_prefabLightConfig = prefabConfig as MyPrefabConfigurationLight;
            MyMwcObjectBuilder_PrefabLight objectBuilderLight = objectBuilder as MyMwcObjectBuilder_PrefabLight;            

            m_light = MyLights.AddLight();

            m_light.LightType = (MyLight.LightTypeEnum)objectBuilderLight.LightType;

            m_light.Start(m_light.LightType, 1);
            m_light.UseInForwardRender = true;


            //grab first dummy and set it as point source
            //since we dont support yet more lights in one prefab. add just the first one
            MyModel model = MyModels.GetModelOnlyDummies(m_prefabLightConfig.ModelLod0Enum);

            m_pointLocalMatrix = Matrix.Identity;
            bool dummyFound = false;
            foreach (KeyValuePair<string, MyModelDummy> pair in model.Dummies)
            {
                if (pair.Key.StartsWith("Dummy", StringComparison.InvariantCultureIgnoreCase))
                {
                    m_pointLocalMatrix = pair.Value.Matrix;
                    dummyFound = true;
                    break;
                }
            }
            Debug.Assert(dummyFound, "Dummy 'POINT_LIGHT_POS' not found in light prefab model: " + model.AssetName);

            m_light.Color = objectBuilderLight.PointColor;
            m_light.ReflectorColor = objectBuilderLight.SpotColor;
            m_light.Falloff = objectBuilderLight.PointFalloff;
            m_light.ReflectorFalloff = objectBuilderLight.SpotFalloff;
            m_IntensityMax = m_light.Intensity = objectBuilderLight.PointIntensity;
            m_light.ReflectorIntensity = ReflectorIntensityMax = objectBuilderLight.SpotIntensity;
            m_light.ReflectorRange = MathHelper.Clamp(objectBuilderLight.SpotRange, 1, MyLightsConstants.MAX_SPOTLIGHT_RANGE);
            m_light.Range = MathHelper.Clamp(objectBuilderLight.PointRange, 1, MyLightsConstants.MAX_POINTLIGHT_RADIUS);
            m_light.PointLightOffset = objectBuilderLight.PointOffset;
            this.FlashOffset = objectBuilderLight.FlashOffset;

            //to add reflector range to builders
            m_light.SpecularColor = objectBuilderLight.PointSpecular;
            m_light.ReflectorConeDegrees = objectBuilderLight.SpotAgle;
            m_effect = objectBuilderLight.Effect;
            //m_light.LightOn = true;
            m_lastBlickChange = MyMinerGame.TotalGamePlayTimeInMilliseconds;

            // here set the properties of glare for the prefab light
            m_light.GlareOn = true;
            m_light.Glare.Type = MyLightGlare.GlareTypeEnum.Normal;
            m_light.Glare.QuerySize = .8f;
            m_light.ShadowDistance = objectBuilderLight.ShadowsDistance;
            m_light.Glare.Intensity = m_light.Intensity;
            UpdateEffect();

            CastShadows = false;
            UpdateLightWorldMatrix();            
        }


        protected override void InitPhysicsInternal()
        {
            InitBoxPhysics(MyMaterialType.METAL, ModelLod0, 1, 0, collisionLayer: (ushort)MyConstants.COLLISION_LAYER_LIGHT, rbFlag: AppCode.Physics.RigidBodyFlag.RBF_RBO_STATIC);
            Physics.Enabled = Activated;
        }

        protected override MyMwcObjectBuilder_Base GetObjectBuilderInternal(bool getExactCopy)
        {
            MyMwcObjectBuilder_PrefabLight objectBuilder = base.GetObjectBuilderInternal(getExactCopy) as MyMwcObjectBuilder_PrefabLight;

            objectBuilder.PointColor = m_light.Color;
            objectBuilder.PointFalloff = m_light.Falloff;
            objectBuilder.PointIntensity = m_IntensityMax;//we use this local value because intenisty in mpoint light may change due to effects
            objectBuilder.PointRange = m_light.Range;
            objectBuilder.PointOffset = m_light.PointLightOffset;
            objectBuilder.SpotAgle = m_light.ReflectorConeDegrees;
            objectBuilder.SpotSpecular = objectBuilder.PointSpecular = m_light.SpecularColor;
            objectBuilder.PointEnabled = m_light.PointOn;
            objectBuilder.Effect = m_effect;
            objectBuilder.LightType = (MyLightPrefabTypeEnum) m_light.LightType;
            objectBuilder.FlashOffset = this.FlashOffset;

            objectBuilder.SpotColor = m_light.ReflectorColor;
            objectBuilder.SpotFalloff = m_light.ReflectorFalloff;
            objectBuilder.SpotIntensity = ReflectorIntensityMax;//we use this local value because intenisty in mpoint light may change due to effects
            objectBuilder.SpotRange = m_light.ReflectorRange;
            objectBuilder.SpotEnabled = m_light.ReflectorOn;
            objectBuilder.ShadowsDistance = m_light.ShadowDistance;

            return objectBuilder;
        }


        //
        public override void OnWorldPositionChanged(object source)
        {

            base.OnWorldPositionChanged(source);

            UpdateLightWorldMatrix();
        }

        private void UpdateLightWorldMatrix() 
        {
            if (m_light != null)
            {
                Matrix newMat = m_pointLocalMatrix * base.WorldMatrix;

                m_light.SetPosition(newMat.Translation);
                m_light.ReflectorDirection = newMat.Down;
                m_light.ReflectorDirection = MyMwcUtils.Normalize(m_light.ReflectorDirection);
                m_light.ReflectorUp = newMat.Right;
                m_light.ReflectorUp = MyMwcUtils.Normalize(m_light.ReflectorUp);

                // move the light outwards in the direction of the lamp, for purposes of glare
                m_light.SetPosition(m_light.Position + 0.75f * m_light.ReflectorDirection);
            }
        }

        /// <summary>
        /// Every object must have this method, but not every phys object must necessarily have something to cleanup
        /// </summary>
        public override void Close()
        {
            MyLights.RemoveLight(m_light);
            m_light = null;
            base.Close();
        }

        /// <summary>
        /// udpate flashing
        /// </summary>
        protected override void UpdatePrefabBeforeSimulation()
        {
            base.UpdatePrefabBeforeSimulation();

            switch (m_effect)
            {
                case MyLightEffectTypeEnum.CONSTANT_FLASHING:
                case MyLightEffectTypeEnum.DISTANT_GLARE_FLASHING:
                    {
                        m_light.Intensity = m_IntensityMax * MathHelper.Clamp(1.5f + 1.5f * (float)Math.Sin(6.0f * (float)MyMinerGame.TotalGamePlayTimeInMilliseconds / 1000.0f + MathHelper.TwoPi * this.FlashOffset), 0.0f, 1.0f);
                        m_light.ReflectorIntensity = ReflectorIntensityMax * MathHelper.Clamp(1.5f + 1.5f * (float)Math.Sin(6.0f * (float)MyMinerGame.TotalGamePlayTimeInMilliseconds / 1000.0f + MathHelper.TwoPi * this.FlashOffset), 0.0f, 1.0f);

                        m_light.Glare.Intensity = MathHelper.Max(m_light.Intensity / m_IntensityMax,
                                                                 m_light.ReflectorIntensity / ReflectorIntensityMax);
                        break;
                    }
                case MyLightEffectTypeEnum.RANDOM_FLASHING:
                case MyLightEffectTypeEnum.DISTANT_GLARE_RANDOM_FLASHING:
                    {
                        int period = 100;//in ms
                        if (MyMinerGame.TotalGamePlayTimeInMilliseconds - m_lastBlickChange > period)
                        {
                            m_lastBlickChange = MyMinerGame.TotalGamePlayTimeInMilliseconds;

                            float size = MyMwcUtils.GetRandomFloat(-1.00f, 1.00f);

                            m_light.Intensity = MathHelper.Clamp(m_light.Intensity +  size * m_IntensityMax/ 2.0f, 0, m_IntensityMax);
                            m_light.ReflectorIntensity = MathHelper.Clamp(m_light.ReflectorIntensity + size*ReflectorIntensityMax / 2.0f, 0, ReflectorIntensityMax);

                            m_light.Glare.Intensity = MathHelper.Max(m_light.Intensity / m_IntensityMax,
                                                                     m_light.ReflectorIntensity / ReflectorIntensityMax);
                        }

                        break;
                    }
                case MyLightEffectTypeEnum.NORMAL:
                case MyLightEffectTypeEnum.DISTANT_GLARE:
                    {
                        m_light.Intensity = m_IntensityMax;
                        m_light.ReflectorIntensity = ReflectorIntensityMax;

                        //m_light.Glare.Intensity = m_light.Intensity;
                        m_light.Glare.Intensity = 1f;
                        break;
                    }
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool Draw(MyRenderObject renderObject)
        {
            if (base.Draw(renderObject) == false)
                return false;

            return true;
        }

        public override bool DebugDraw()
        {
            //MyDebugDraw.DrawLine3D(m_light.Position, m_light.Position + m_light.Range * m_light.ReflectorDirection, new Color(1, 0, 1, .5f), new Color(1, 0, 1, .5f));

            return base.DebugDraw();
        }

        public void SetAllColors(Color newColor)
        {
            m_light.Color = newColor.ToVector4();
            m_light.ReflectorColor = newColor.ToVector4();
            m_light.SpecularColor = newColor.ToVector3();
        }

        public MyMwcObjectBuilder_PrefabLight_TypesEnum PrefabLightType
        {
            get
            {
                return (MyMwcObjectBuilder_PrefabLight_TypesEnum)m_prefabId;
            }
            set
            {
                m_prefabId = (int)value;
            }
        }

        public override string GetFriendlyName()
        {
            return "MyPrefabLight";
        }

        protected override bool PrefabNeedsUpdateNow
        {
            get
            {
                return base.PrefabNeedsUpdateNow && 
                       (m_effect == MyLightEffectTypeEnum.CONSTANT_FLASHING ||
                       m_effect == MyLightEffectTypeEnum.DISTANT_GLARE_FLASHING ||
                       m_effect == MyLightEffectTypeEnum.DISTANT_GLARE_RANDOM_FLASHING ||
                       m_effect == MyLightEffectTypeEnum.RANDOM_FLASHING);
            }
        }
    }
}
