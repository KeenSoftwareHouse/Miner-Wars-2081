using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Entities.Prefabs;
using MinerWars.AppCode.Game.Entities;
using MinerWarsMath;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.Prefabs;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.GUI.Prefabs;
using MinerWars.AppCode.Game.World.Global;
using MinerWars.AppCode.Game.Lights;
using MinerWars.AppCode.Game.Models;

using System.Diagnostics;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.TransparentGeometry;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.Render;

namespace MinerWars.AppCode.Game.Prefabs
{
    class MyPrefabAlarm : MyPrefabBase
    {
        protected override void WorkingChanged()
        {
            base.WorkingChanged();
            if (IsWorking())
            {
                m_modelLod0 = m_modelLod0On;
                m_modelLod1 = m_modelLod1On;
            }
            else 
            {
                m_modelLod0 = m_modelLod0Off;
                m_modelLod1 = m_modelLod1Off;
            }
            if (m_pointLight != null)
            {
                m_pointLight.LightOn = IsWorking();
            }
        }

        private MyModel m_modelLod0On;        
        private MyModel m_modelLod1On;
        private MyModel m_modelLod0Off;
        private MyModel m_modelLod1Off;

        private MyLight m_pointLight;
        private Matrix m_pointLocalMatrix;
        private float m_intensityMax;
        private int m_lastBlickChange;

        public MyPrefabAlarm(MyPrefabContainer owner) 
            : base(owner)
        {            
            
        }

        protected override void InitPhysicsInternal()
        {
            InitBoxPhysics(MyMaterialType.METAL, ModelLod0, 1, 0, collisionLayer: (ushort)MyConstants.COLLISION_LAYER_PREFAB_ALARM, rbFlag: AppCode.Physics.RigidBodyFlag.RBF_RBO_STATIC);
            Physics.Enabled = Activated;
        }

        protected override void InitPrefab(string displayName, Vector3 relativePosition, Matrix localOrientation, MyMwcObjectBuilder_PrefabBase objectBuilder, MyPrefabConfiguration prefabConfig)
        {
            MyMwcObjectBuilder_PrefabAlarm objectBuilderAlarm = objectBuilder as MyMwcObjectBuilder_PrefabAlarm;
            MyPrefabConfigurationAlarm alarmConfig = prefabConfig as MyPrefabConfigurationAlarm;

            m_modelLod0On = MyModels.GetModelOnlyData(alarmConfig.ModelLod0EnumOn);
            if (alarmConfig.ModelLod1EnumOn != null)
            {
                m_modelLod1On = MyModels.GetModelOnlyData(alarmConfig.ModelLod1EnumOn.Value);
            }
            m_modelLod0Off = m_modelLod0;
            m_modelLod1Off = m_modelLod1;

            Flags |= EntityFlags.EditableInEditor;
            InitLight();
        }

        private void InitLight()
        {
            m_pointLight = MyLights.AddLight();

            m_pointLight.LightType = MyLight.LightTypeEnum.PointLight;

            m_pointLight.Start(m_pointLight.LightType, 1);
            m_pointLight.UseInForwardRender = true;


            //grab first dummy and set it as point source
            //since we dont support yet more lights in one prefab. add just the first one
            MyModel model = MyModels.GetModelOnlyDummies(ModelLod0.ModelEnum);
            m_pointLocalMatrix = Matrix.CreateTranslation(model.BoundingSphere.Center);            
            
            m_pointLight.Color = new Vector4(1f, 0f, 0f, 1f);
            m_pointLight.ReflectorColor = Vector4.Zero;
            m_pointLight.Falloff = 0.1f;
            m_pointLight.ReflectorFalloff = 0.1f;
            m_intensityMax = m_pointLight.Intensity = 10f;            
            m_pointLight.ReflectorIntensity = 1f;            
            m_pointLight.ReflectorRange = 1f;
            m_pointLight.Range = 40f;
            m_pointLight.PointLightOffset = 0f;

            //to add reflector range to builders
            m_pointLight.SpecularColor = new Vector3(1f, 0f, 0f);
            m_pointLight.ReflectorConeDegrees = 0.1f;
            m_lastBlickChange = MyMinerGame.TotalGamePlayTimeInMilliseconds;

            // here set the properties of glare for the prefab light            
            m_pointLight.GlareOn = true;
            m_pointLight.Glare.Type = MyLightGlare.GlareTypeEnum.Distant;            
            m_pointLight.Glare.QuerySize = .8f;
            UpdateLightWorldMatrix();
        }

        private void UpdateLightWorldMatrix()
        {
            if (m_pointLight != null)
            {
                Matrix newMat = m_pointLocalMatrix * base.WorldMatrix;

                m_pointLight.SetPosition(newMat.Translation);
                m_pointLight.ReflectorDirection = newMat.Down;
                m_pointLight.ReflectorDirection = MyMwcUtils.Normalize(m_pointLight.ReflectorDirection);
                m_pointLight.ReflectorUp = newMat.Right;
                m_pointLight.ReflectorUp = MyMwcUtils.Normalize(m_pointLight.ReflectorUp);

                // move the light outwards in the direction of the lamp, for purposes of glare
                m_pointLight.SetPosition(m_pointLight.Position + 0.75f * m_pointLight.ReflectorDirection);
            }
        }

        public override void OnWorldPositionChanged(object source)
        {
            base.OnWorldPositionChanged(source);

            UpdateLightWorldMatrix();
        }

        protected override void UpdatePrefabBeforeSimulation()
        {
            base.UpdatePrefabBeforeSimulation();

            m_pointLight.Intensity = 1.5f*m_intensityMax * ((1.0f +  (float)Math.Sin(2.60f*(float)Math.PI*(MyMinerGame.TotalGamePlayTimeInMilliseconds / 1000.0f)))*0.5f-0.3f);
            //Debug.WriteLine(m_pointLight.Intensity);
        }        

        protected override MyMwcObjectBuilder_Base GetObjectBuilderInternal(bool getExactCopy)
        {
            MyMwcObjectBuilder_PrefabAlarm objectBuilder = base.GetObjectBuilderInternal(getExactCopy) as MyMwcObjectBuilder_PrefabAlarm;                     

            return objectBuilder;
        }

        public override bool Draw(MyRenderObject renderObject)
        {
            return base.Draw(renderObject);
        }

        public MyMwcObjectBuilder_PrefabAlarm_TypesEnum PrefabAlarmType
        {
            get
            {
                return (MyMwcObjectBuilder_PrefabAlarm_TypesEnum)m_prefabId;
            }
            set
            {
                m_prefabId = (int)value;
            }
        }

        public override string GetFriendlyName()
        {
            return "MyPrefabAlarm";
        }

        public override void Close()
        {
            MyLights.RemoveLight(m_pointLight);
            m_pointLight = null;
            base.Close();            
        }
    }
}
