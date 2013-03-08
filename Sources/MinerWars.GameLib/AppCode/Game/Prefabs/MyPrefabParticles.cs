using MinerWarsMath;
using MinerWars.AppCode.Game.TransparentGeometry;
using MinerWars.AppCode.Game.Prefabs;
using MinerWars.AppCode.Game.TransparentGeometry.Particles;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.Prefabs;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Models;
using System.Collections.Generic;
using MinerWars.AppCode.Game.Entities.Prefabs;
using System.Text;
using System;
using MinerWars.AppCode.Physics;

using MinerWars.AppCode.Game.Render;
using MinerWars.CommonLIB.AppCode.Import;

namespace MinerWars.AppCode.Game.Entities.SubObjects
{
    class MyPrefabParticles : MyPrefabBase
    {
        //MyParticle m_particle;
        int m_lastTimeParticle;
        Matrix m_pointLocalMatrix;
        MyParticleEffect m_particleEffect;

        public MyPrefabParticles(MyPrefabContainer owner) : base(owner) { ; }        

        protected override void InitPrefab(string displayName, Vector3 relativePosition, Matrix localOrientation, MyMwcObjectBuilder_PrefabBase objectBuilder, MyPrefabConfiguration prefabConfig)
        {            
            MyPrefabConfigurationParticles prefabParticleConfig = prefabConfig as MyPrefabConfigurationParticles;
            
            m_lastTimeParticle = MyMinerGame.TotalGamePlayTimeInMilliseconds;
            MyModel model = MyModels.GetModelOnlyDummies(prefabParticleConfig.ModelLod0Enum);
            m_pointLocalMatrix = Matrix.Identity;
            foreach (KeyValuePair<string, MyModelDummy> pair in model.Dummies)
            {
                m_pointLocalMatrix = pair.Value.Matrix;
            }

            m_particleEffect = MyParticlesManager.CreateParticleEffect((int)prefabParticleConfig.EffectID, true);
            m_particleEffect.AutoDelete = false;
            m_particleEffect.WorldMatrix = WorldMatrix;
        }

        protected override MyMwcObjectBuilder_Base GetObjectBuilderInternal(bool getExactCopy)
        {
            MyMwcObjectBuilder_PrefabParticles objectBuilder = base.GetObjectBuilderInternal(getExactCopy) as MyMwcObjectBuilder_PrefabParticles;

            //TODO fill up parameters
            return objectBuilder;
        }

        //
        //
        //
        public override void OnWorldPositionChanged(object source)
        {
            base.OnWorldPositionChanged(source);
            if (m_particleEffect != null)
                m_particleEffect.WorldMatrix = WorldMatrix;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool Draw(MyRenderObject renderObject)
        {

            bool drawOk = base.Draw(renderObject);
            
            if (drawOk)
            {
                if (m_particleEffect != null)
                    MyParticlesManager.CustomDraw(m_particleEffect);
            }
            
            return drawOk;
        }

        public override void Close()
        {
            base.Close();

            if (m_particleEffect != null)
            {
                MyParticlesManager.RemoveParticleEffect(m_particleEffect);
                m_particleEffect = null;
            }
        }

        public MyMwcObjectBuilder_PrefabParticles_TypesEnum PrefabParticleType
        {
            get
            {
                return (MyMwcObjectBuilder_PrefabParticles_TypesEnum)m_prefabId;
            }
            set
            {
                m_prefabId = (int)value;
            }
        }

        public override string GetFriendlyName()
        {
            return "MyPrefabParticles";
        }
    }
}
