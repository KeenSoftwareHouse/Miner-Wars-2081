using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Inventory;
using MinerWarsMath;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.AppCode.Game.Models;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Physics;

namespace MinerWars.AppCode.Game.Entities
{
    class MyMysteriousCube : MyEntity
    {
        public MyMysteriousCube() 
            : base(true)
        {
        }

        public void Init(string hudLabelText, MyMwcObjectBuilder_MysteriousCube objectBuilder, Matrix matrix) 
        {
            Flags |= EntityFlags.EditableInEditor;

            StringBuilder hudLabelTextSb = (hudLabelText == null) ? null : new StringBuilder(hudLabelText);

            

            switch (objectBuilder.MysteriousCubeType)
            {
                case MyMwcObjectBuilder_MysteriousCube_TypesEnum.Type1:
                    base.Init(hudLabelTextSb, MyModelsEnum.MysteriousBox_matt_5m, null, null, null, objectBuilder);
                    m_diffuseColor = new Vector3(0, 0, 0);
                    break;

                case MyMwcObjectBuilder_MysteriousCube_TypesEnum.Type2:
                    base.Init(hudLabelTextSb, MyModelsEnum.MysteriousBox_spec_5m, null, null, null, objectBuilder);
                    m_diffuseColor = new Vector3(0, 0, 0);
                    break;

                case MyMwcObjectBuilder_MysteriousCube_TypesEnum.Type3:
                    base.Init(hudLabelTextSb, MyModelsEnum.MysteriousBox_mid_5m, null, null, null, objectBuilder);
                    m_diffuseColor = new Vector3(0, 0, 0);
                    break;

            }

            base.InitBoxPhysics(MyMaterialType.METAL, ModelLod0, 1f, 0f, MyConstants.COLLISION_LAYER_DEFAULT, RigidBodyFlag.RBF_RBO_STATIC);
            SetWorldMatrix(matrix);
            Save = true;
            this.Physics.Enabled = true;

           // NeedsUpdate = true;

        }

        protected override MyMwcObjectBuilder_Base GetObjectBuilderInternal(bool getExactCopy)
        {
            MyMwcObjectBuilder_MysteriousCube builder = base.GetObjectBuilderInternal(getExactCopy) as MyMwcObjectBuilder_MysteriousCube;

            return builder;
        }


        public override void UpdateBeforeSimulation()
        {
            base.UpdateBeforeSimulation();
                                             
            //MyMesh mesh = m_modelLod0.GetMeshList()[0];
            //mesh.Materials[0].SpecularIntensity = 0.5f;
            //mesh.Materials[0].SpecularPower = 3.5f;
            //mesh.Materials[0].SpecularColor = Vector3.Zero;
 
        }

        public override void Close()
        {
            base.Close();
        }
    }
}
