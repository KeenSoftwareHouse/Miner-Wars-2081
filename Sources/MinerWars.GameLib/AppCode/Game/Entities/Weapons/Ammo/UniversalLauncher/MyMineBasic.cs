using MinerWarsMath;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Utils;

namespace MinerWars.AppCode.Game.Entities.Weapons.UniversalLauncher
{
    using Models;
    using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;

    class MyMineBasic : MyMineBase
    {
        public override void Init()
        {
            Init(MyModelsEnum.MineBasic, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Mine_Basic, MyUniversalLauncherConstants.USE_SPHERE_PHYSICS);
        }

        public override void Start(Vector3 position, Vector3 initialVelocity, Vector3 direction, float impulseMultiplier, MyEntity owner)
        {
            base.Start(position, initialVelocity, direction, impulseMultiplier, owner, MyTextsWrapper.Get(MyTextsWrapperEnum.MineBasicHud));
        }
    }
}
