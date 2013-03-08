using System.Collections.Generic;
using MinerWars.AppCode.Game.Audio;
using MinerWars.AppCode.Game.Utils;

namespace MinerWars.AppCode.Game.Entities
{
    static class MyDummyEffectHelpers
    {
        static readonly Dictionary<MyParticleEffectsIDEnum, MyDummyEffectHelper> m_dummyEffectHelpers =
            new Dictionary<MyParticleEffectsIDEnum, MyDummyEffectHelper>();

        static MyDummyEffectHelpers()
        {
            m_dummyEffectHelpers.Add(
                MyParticleEffectsIDEnum.Prefab_LeakingFire,
                new MyDummyEffectHelper { DamageStrength = 10, SoundCueEnum = MySoundCuesEnum.Amb3D_PrefabFire });
            m_dummyEffectHelpers.Add(
                MyParticleEffectsIDEnum.Prefab_LeakingFire_x2,
                new MyDummyEffectHelper { DamageStrength = 10, SoundCueEnum = MySoundCuesEnum.Amb3D_PrefabFire });
            m_dummyEffectHelpers.Add(
                MyParticleEffectsIDEnum.EngineThrust,
                new MyDummyEffectHelper { DamageStrength = 10, SoundCueEnum = MySoundCuesEnum.Amb3D_EngineThrust });

            m_dummyEffectHelpers.Add(
                MyParticleEffectsIDEnum.Prefab_LeakingSteamBlack,
                new MyDummyEffectHelper { DirectionalPushStrength = 1, SoundCueEnum = MySoundCuesEnum.Amb3D_SteamLoop01 });
            m_dummyEffectHelpers.Add(
                MyParticleEffectsIDEnum.Prefab_LeakingSteamGrey,
                new MyDummyEffectHelper { DirectionalPushStrength = 1, SoundCueEnum = MySoundCuesEnum.Amb3D_SteamLoop02 });
            m_dummyEffectHelpers.Add(
                MyParticleEffectsIDEnum.Prefab_LeakingSteamWhite,
                new MyDummyEffectHelper { DirectionalPushStrength = 1, SoundCueEnum = MySoundCuesEnum.Amb3D_SteamLoop03 });
        }

        public static MyDummyEffectHelper Get(MyParticleEffectsIDEnum particleEffectType)
        {
            MyDummyEffectHelper ret;
            if (m_dummyEffectHelpers.TryGetValue(particleEffectType, out ret))
            {
                return ret;
            }

            return null;
        }
    }
}