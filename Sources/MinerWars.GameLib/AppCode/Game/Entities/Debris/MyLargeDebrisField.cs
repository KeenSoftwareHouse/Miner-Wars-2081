using MinerWars.AppCode.Physics;
using MinerWars.AppCode.Game.Physics;

namespace MinerWars.AppCode.Game.Entities
{
    using System.Text;
    using CommonLIB.AppCode.ObjectBuilders.Object3D;
    using MinerWarsMath;
    using Models;
    using SysUtils.Utils;
    using Utils;

    class MyLargeDebrisField : MyEntity
    {
        public virtual void Init(string hudLabelText, MyMwcObjectBuilder_LargeDebrisField objectBuilder, Matrix matrix)
        {
            switch (objectBuilder.DebrisType)
            {
                case MyMwcObjectBuilder_LargeDebrisField_TypesEnum.Debris84:
                    InitDebris(hudLabelText, MyModelsEnum.DebrisField, null, MyMaterialType.METAL, objectBuilder, matrix);
                    break;
                default:
                    throw new MyMwcExceptionApplicationShouldNotGetHere();
            }
            Save = false;
        }

        void InitDebris(string hudLabelText, MyModelsEnum modelLod0Enum, MyModelsEnum? modelLod1Enum, MyMaterialType materialType, MyMwcObjectBuilder_LargeDebrisField objectBuilder, Matrix matrix)
        {
            StringBuilder hudLabelTextSb = (hudLabelText == null) ? null : new StringBuilder(hudLabelText);
            base.Init(hudLabelTextSb, modelLod0Enum, modelLod1Enum, null, null, objectBuilder);

            MyModel model = MyModels.GetModelOnlyData(modelLod0Enum);

            InitTrianglePhysics(materialType, 1.0f, model, null, enable: false);
			SetWorldMatrix(matrix);            
        }
    }
}


