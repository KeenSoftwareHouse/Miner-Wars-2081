using MinerWarsMath;
using MinerWars.CommonLIB.AppCode.Utils;

namespace MinerWars.AppCode.Game.Utils
{
    static class MyMissileHelperUtil
    {
        public static Vector4 GetMissileLightColor()
        {
            float rnd = MyMwcUtils.GetRandomFloat(-0.1f, +0.1f);
            return new Vector4(MyMissileConstants.MISSILE_LIGHT_COLOR.X + rnd, MyMissileConstants.MISSILE_LIGHT_COLOR.Y + rnd, MyMissileConstants.MISSILE_LIGHT_COLOR.Z + rnd, MyMissileConstants.MISSILE_LIGHT_COLOR.W);
        }

        public static Vector4 GetCannonShotLightColor()
        {
            float rnd = MyMwcUtils.GetRandomFloat(-0.1f, +0.1f);
            return new Vector4(MyCannonShotConstants.LIGHT_COLOR.X + rnd, MyCannonShotConstants.LIGHT_COLOR.Y + rnd, MyCannonShotConstants.LIGHT_COLOR.Z + rnd, MyCannonShotConstants.LIGHT_COLOR.W);
        }
    }
}
