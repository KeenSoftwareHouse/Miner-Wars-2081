using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Render;
using MinerWars.AppCode.Game.Managers;

namespace MinerWars.AppCode.Game.Entities.Ships.SubObjects
{
    class MyCockpit: MyEntity
    {
        public MyCockpit()
        {
            // Cockpit is near
            NearFlag = true;
            Save = false;
        }

        public MinerWars.AppCode.Game.HUD.MyHudTexturesEnum Crosshair;

        public override void Init(StringBuilder displayName, Models.MyModelsEnum? modelLod0Enum, Models.MyModelsEnum? modelLod1Enum, MyEntity parentObject, float? scale, CommonLIB.AppCode.ObjectBuilders.MyMwcObjectBuilder_Base objectBuilder, Models.MyModelsEnum? modelCollision = null, Models.MyModelsEnum? modelLod2 = null)
        {
            base.Init(displayName, modelLod0Enum, modelLod1Enum, parentObject, scale, objectBuilder, modelCollision, modelLod2);

            switch (modelLod0Enum)
            {
                case Models.MyModelsEnum.Cockpit_SS_04:
                    Crosshair = HUD.MyHudTexturesEnum.crosshair_nazzi; 
                    break;
                case Models.MyModelsEnum.Cockpit_Razorclaw:
                    Crosshair = HUD.MyHudTexturesEnum.crosshair_templary;
                    break;
                case Models.MyModelsEnum.Cockpit_CN_03:
                    Crosshair = HUD.MyHudTexturesEnum.crosshair_russian;
                    break;
                case Models.MyModelsEnum.OmniCorp_EAC01_Cockpit:
                case Models.MyModelsEnum.OmniCorp01_Cockpit:
                case Models.MyModelsEnum.OmniCorp04_Cockpit:
                case Models.MyModelsEnum.OmniCorp03_Cockpit:
                    Crosshair = HUD.MyHudTexturesEnum.crosshair_omnicorp;
                    break;
                case Models.MyModelsEnum.EAC02_Cockpit:
                case Models.MyModelsEnum.EAC03_Cockpit:
                case Models.MyModelsEnum.EAC04_Cockpit:
                case Models.MyModelsEnum.EAC05_Cockpit:
                default:
                    Crosshair = HUD.MyHudTexturesEnum.Crosshair01;
                    break;
            }
            m_modelLod0.PreloadTextures(LoadingMode.Immediate, MaterialIndex);
        }

        public override Matrix GetWorldMatrixForDraw()
        {
            return MySession.PlayerShip.PlayerHeadForCockpitInteriorWorldMatrix;
        }

        public override bool Draw(MyRenderObject renderObject = null)
        {
            return base.Draw(renderObject);
        }

        public override void UpdateBeforeSimulation()
        {
            base.UpdateBeforeSimulation();

            this.Visible = Cockpit.MyCockpitGlass.CanDrawCockpit();
        }
    }
}
