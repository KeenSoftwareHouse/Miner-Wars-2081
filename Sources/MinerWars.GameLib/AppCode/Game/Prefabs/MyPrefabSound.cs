using MinerWarsMath;
using MinerWars.AppCode.Game.Audio;
using MinerWars.AppCode.Game.Entities.SubObjects;
using MinerWars.AppCode.Game.Prefabs;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.Prefabs;
using MinerWars.AppCode.Game.Models;
using System.Collections.Generic;

using System.Text;
using MinerWars.AppCode.Physics;
using System;
using MinerWars.CommonLIB.AppCode.Import;


namespace MinerWars.AppCode.Game.Entities.Prefabs
{
    class MyPrefabSound : MyPrefabBase
    {
        MySoundCuesEnum m_cueEnum;
        MySoundCue? m_cue;
        Matrix m_pointLocalMatrix;
        MyPrefabConfigurationSound m_prefabSoundConfig;

        protected override void WorkingChanged()
        {
            base.WorkingChanged();
            if (IsWorking())
            {
                PlaySound();
            }
            else 
            {
                StopSound();
            }
        }

        public MyPrefabSound(MyPrefabContainer owner) 
            : base(owner)
        {            
        }        

        protected override void InitPrefab(string displayName, Vector3 relativePosition, Matrix localOrientation, MyMwcObjectBuilder_PrefabBase objectBuilder, MyPrefabConfiguration prefabConfig)
        {
            m_prefabSoundConfig = prefabConfig as MyPrefabConfigurationSound;            

            MyModel model = MyModels.GetModelOnlyDummies(m_prefabSoundConfig.ModelLod0Enum);
            m_pointLocalMatrix = Matrix.Identity;
            m_cueEnum = m_prefabSoundConfig.Sound;
            foreach (KeyValuePair<string, MyModelDummy> pair in model.Dummies)
            {
                m_pointLocalMatrix = pair.Value.Matrix;
            }

            if (IsWorking())
            {
                PlaySound();
            }
        }

        public override void UpdateBeforeSimulation()
        {
            base.UpdateBeforeSimulation();

            if (Parent == null) return;

            if (m_cue == null || !m_cue.Value.IsPlaying) return;

            MyAudio.UpdateCuePosition(m_cue, Vector3.Transform(m_pointLocalMatrix.Translation, base.WorldMatrix), WorldMatrix.Forward, WorldMatrix.Up, Parent.Physics.LinearVelocity);
        }

        private void PlaySound() 
        {
            if (m_cue == null || !m_cue.Value.IsPlaying)
            {
                m_cue = MyAudio.AddCue3D(m_cueEnum, Vector3.Transform(m_pointLocalMatrix.Translation, base.WorldMatrix), WorldMatrix.Forward, WorldMatrix.Up, Physics.LinearVelocity);
            }
        }

        private void StopSound() 
        {
            if (m_cue != null && m_cue.Value.IsPlaying)
            {
                m_cue.Value.Stop(SharpDX.XACT3.StopFlags.Immediate);
            }
            m_cue = null;
        }

        protected override MyMwcObjectBuilder_Base GetObjectBuilderInternal(bool getExactCopy)
        {
            MyMwcObjectBuilder_PrefabSound objectBuilder = base.GetObjectBuilderInternal(getExactCopy) as MyMwcObjectBuilder_PrefabSound;
            
            return objectBuilder;
        }

        protected override void UpdatePrefabAfterSimulation()
        {
            base.UpdatePrefabAfterSimulation();
            /*
            if (m_cue != null && m_cue.Value.IsPlaying)
            {
                MyAudio.CalculateOcclusion(m_cue.Value, m_cue.Value.Center);
            }
            */            

            PlaySound();
            if (m_cue != null && m_cue.Value.IsPlaying)
            {
                MyAudio.UpdateCuePosition(m_cue, Vector3.Transform(m_pointLocalMatrix.Translation, base.WorldMatrix), WorldMatrix.Forward, WorldMatrix.Up, Physics.LinearVelocity);
            }
        }

        public override void Close()
        {
            base.Close();
            StopSound();            
        }

        public MyMwcObjectBuilder_PrefabSound_TypesEnum PrefabSoundType
        {
            get
            {
                return (MyMwcObjectBuilder_PrefabSound_TypesEnum)m_prefabId;
            }
            set
            {
                m_prefabId = (int)value;
            }
        }

        public override string GetFriendlyName()
        {
            return "MyPrefabSound";
        }        
    }
}
