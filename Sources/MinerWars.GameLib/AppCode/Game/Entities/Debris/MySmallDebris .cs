using MinerWars.AppCode.Physics;
using MinerWars.AppCode.Game.Physics;

namespace MinerWars.AppCode.Game.Entities
{
    using System;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Text;
    using CommonLIB.AppCode.ObjectBuilders.Object3D;
    using MinerWarsMath;
    using Models;
    using SysUtils.Utils;
    using Utils;

    class MySmallDebris : MyEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MyEntity"/> class.
        /// </summary>
        public MySmallDebris()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MyEntity"/> class.
        /// </summary>
        public MySmallDebris(string name)
        {
            this.Name = name;

            InitDebris(null, MyModelsEnum.Debris1, null, MyMaterialType.METAL, null, Matrix.Identity, 10000);
            Save = false;
        }

        public static MyModelsEnum GetModelForType(MyMwcObjectBuilder_SmallDebris_TypesEnum smallDebrisEnum)
        {
            switch (smallDebrisEnum)
            {
                case MyMwcObjectBuilder_SmallDebris_TypesEnum.Cistern:
                    return MyModelsEnum.cistern;
                    break;
                case MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris1:
                    return MyModelsEnum.Debris1;
                    break;
                case MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris2:
                    return MyModelsEnum.Debris2;
                    break;
                case MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris3:
                    return MyModelsEnum.Debris3;
                    break;
                case MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris4:
                    return MyModelsEnum.Debris4;
                    break;
                case MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris5:
                    return MyModelsEnum.Debris5;
                    break;
                case MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris6:
                    return MyModelsEnum.Debris6;
                    break;
                case MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris7:
                    return MyModelsEnum.Debris7;
                    break;
                case MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris8:
                    return MyModelsEnum.Debris8;
                    break;
                case MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris9:
                    return MyModelsEnum.Debris9;
                    break;
                case MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris10:
                    return MyModelsEnum.Debris10;
                    break;
                case MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris11:
                    return MyModelsEnum.Debris11;
                    break;
                case MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris12:
                    return MyModelsEnum.Debris12;
                    break;
                case MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris13:
                    return MyModelsEnum.Debris13;
                    break;
                case MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris14:
                    return MyModelsEnum.Debris14;
                    break;
                case MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris15:
                    return MyModelsEnum.Debris15;
                    break;
                case MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris16:
                    return MyModelsEnum.Debris16;
                    break;
                case MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris17:
                    return MyModelsEnum.Debris17;
                    break;
                case MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris18:
                    return MyModelsEnum.Debris18;
                    break;
                case MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris19:
                    return MyModelsEnum.Debris19;
                    break;
                case MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris20:
                    return MyModelsEnum.Debris20;
                    break;
                case MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris21:
                    return MyModelsEnum.Debris21;
                    break;
                case MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris22:
                    return MyModelsEnum.Debris22;
                    break;
                case MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris23:
                    return MyModelsEnum.Debris23;
                    break;
                case MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris24:
                    return MyModelsEnum.Debris24;
                    break;
                case MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris25:
                    return MyModelsEnum.Debris25;
                    break;
                case MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris26:
                    return MyModelsEnum.Debris26;
                    break;
                case MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris27:
                    return MyModelsEnum.Debris27;
                    break;
                case MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris28:
                    return MyModelsEnum.Debris28;
                    break;
                case MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris29:
                    return MyModelsEnum.Debris29;
                    break;
                case MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris30:
                    return MyModelsEnum.Debris30;
                    break;
                case MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris31:
                    return MyModelsEnum.Debris31;
                    break;
                case MyMwcObjectBuilder_SmallDebris_TypesEnum.pipe_bundle:
                    return MyModelsEnum.pipe_bundle;
                    break;
                case MyMwcObjectBuilder_SmallDebris_TypesEnum.UtilityVehicle_1:
                    return MyModelsEnum.UtilityVehicle_1;
                    break;
                case MyMwcObjectBuilder_SmallDebris_TypesEnum.Standard_Container_1:
                    return MyModelsEnum.Standard_Container_1;
                    break;
                case MyMwcObjectBuilder_SmallDebris_TypesEnum.Standard_Container_2:
                    return MyModelsEnum.Standard_Container_2;
                    break;
                case MyMwcObjectBuilder_SmallDebris_TypesEnum.Standard_Container_3:
                    return MyModelsEnum.Standard_Container_3;
                    break;
                case MyMwcObjectBuilder_SmallDebris_TypesEnum.Standard_Container_4:
                    return MyModelsEnum.Standard_Container_4;
                    break;
                case MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris32_pilot:
                    return MyModelsEnum.Debris32_pilot;
                    break;
                default:
                    throw new MyMwcExceptionApplicationShouldNotGetHere();
            }

        }

        public virtual void Init(string hudLabelText, MyMwcObjectBuilder_SmallDebris objectBuilder, Matrix matrix)
        {
            InitDebris(hudLabelText, GetModelForType(objectBuilder.DebrisType), null, MyMaterialType.METAL, objectBuilder, matrix, objectBuilder.Mass);
        }

        void InitDebris(string hudLabelText, MyModelsEnum modelLod0Enum, MyModelsEnum? modelLod1Enum, MyMaterialType materialType,
            MyMwcObjectBuilder_SmallDebris objectBuilder, Matrix matrix, float mass)
        {
            StringBuilder hudLabelTextSb = (hudLabelText == null) ? null : new StringBuilder(hudLabelText);

            base.Init(hudLabelTextSb, modelLod0Enum, modelLod1Enum, null, null, objectBuilder);

            if (objectBuilder != null)
            {
                SetWorldMatrix(matrix);
            }

            if (mass > 0)
            {
                base.InitBoxPhysics(materialType, this.ModelLod0, mass, 0, MyConstants.COLLISION_LAYER_DEFAULT, RigidBodyFlag.RBF_DEFAULT);
                this.Physics.PlayCollisionCueEnabled = true;
            }
        }

        public override void Close()
        {
            base.Close();

            if (this.Physics != null)
                this.Physics.RemoveAllElements();
        }

        public override bool IsSelectable()
        {
            return false;
        }

        public override bool IsSelectableAsChild()
        {
            return false;
        }
    }
}
