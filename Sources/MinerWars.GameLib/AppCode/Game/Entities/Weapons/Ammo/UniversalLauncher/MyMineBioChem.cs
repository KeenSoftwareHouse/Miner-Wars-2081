using MinerWarsMath;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Utils;

namespace MinerWars.AppCode.Game.Entities.Weapons.UniversalLauncher
{
    using Models;
    using CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;

    class MyMineBioChem : MyMineBase
    {
        public override void Init()
        {
            Init(MyModelsEnum.MineBasic, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Mine_BioChem, MyUniversalLauncherConstants.USE_SPHERE_PHYSICS);
            m_explosionType = Explosions.MyExplosionTypeEnum.BIOCHEM_EXPLOSION;
        }

        public override void Start(Vector3 position, Vector3 initialVelocity, Vector3 direction, float impulseMultiplier, MyEntity owner)
        {
            base.Start(position, initialVelocity, direction, impulseMultiplier, owner, MyTextsWrapper.Get(MyTextsWrapperEnum.MineBioChemHud));
        }
    }
}
