using System;
using System.Collections.Generic;
using MinerWarsMath;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Physics;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.Explosions;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Entities.Weapons;
using MinerWars.AppCode.Game.TransparentGeometry.Particles;

namespace MinerWars.AppCode.Game.TransparentGeometry
{
    static class MyParticleEffects
    {
        static Dictionary<MyEntity, Dictionary<int, MyParticleEffect>> m_hitParticles = new Dictionary<MyEntity, Dictionary<int, MyParticleEffect>>(64);


        public static readonly MyCustomHitParticlesMethod DelegateForCreateExplosiveHitParticles = CreateExplosiveHitParticles;
        public static readonly MyCustomHitParticlesMethod DelegateForCreateBiochemHitParticles = CreateBiochemHitParticles;
        public static readonly MyCustomHitParticlesMethod DelegateForCreateEMPHitParticles = CreateEMPHitParticles;
        public static readonly MyCustomHitParticlesMethod DelegateForCreateBasicHitParticles = CreateBasicHitParticles;
        public static readonly MyCustomHitParticlesMethod DelegateForCreatePiercingHitParticles = CreatePiercingHitParticles;

        public static readonly MyCustomHitParticlesMethod DelegateForCreateAutocannonBasicHitParticles = CreateBasicHitAutocannonParticles;
        public static readonly MyCustomHitParticlesMethod DelegateForCreateAutocannonBiochemHitParticles = CreateChemicalHitAutocannonParticles;
        public static readonly MyCustomHitParticlesMethod DelegateForCreateAutocannonEMPHitParticles = CreateEMPHitAutocannonParticles;
        public static readonly MyCustomHitParticlesMethod DelegateForCreateAutocannonExplosiveHitParticles = CreateExplosiveHitAutocannonParticles;
        public static readonly MyCustomHitParticlesMethod DelegateForCreateAutocannonPiercingHitParticles = CreatePiercingHitAutocannonParticles;
        public static readonly MyCustomHitParticlesMethod DelegateForCreateAutocannonHighSpeedHitParticles = CreateHighSpeedHitAutocannonParticles;

        public static readonly MyCustomHitMaterialMethod DelegateForCreateHitMaterialParticles = CreateHitMaterialParticles;
        public static readonly MyCustomHitMaterialMethod DelegateForCreateAutocannonHitMaterialParticles = CreateAutocannonHitMaterialParticles;

        public static void GenerateMuzzleFlash(Vector3 position, Vector3 dir, float radius, float length, bool near = false)
        {
            float angle = MyMwcUtils.GetRandomFloat(0, MathHelper.PiOver2);

            float colorComponent = 1.3f;
            Vector4 color = new Vector4(colorComponent, colorComponent, colorComponent, 1);

            MyTransparentGeometry.AddLineBillboard(MyTransparentMaterialEnum.MuzzleFlashMachineGunSide, color, position,
                dir, length * 2, 0.3f, 0, near);
            MyTransparentGeometry.AddPointBillboard(MyTransparentMaterialEnum.MuzzleFlashMachineGunFront, color, position, radius, angle, 0, false, near);
        }

        //  Create smoke and debris particle at the place of voxel/model hit
        public static void CreateCollisionParticles(Vector3 hitPoint, Vector3 direction, bool doSmoke, bool doSparks)
        {
            Matrix dirMatrix = MyMath.MatrixFromDir(direction);
            if (doSmoke)
            {
                MyParticleEffect effect = MyParticlesManager.CreateParticleEffect((int)MyParticleEffectsIDEnum.Collision_Smoke);
                effect.WorldMatrix = Matrix.CreateWorld(hitPoint, dirMatrix.Forward, dirMatrix.Up);
            }
            if (doSparks)
            {
                MyParticleEffect effect = MyParticlesManager.CreateParticleEffect((int)MyParticleEffectsIDEnum.Collision_Sparks);
                effect.WorldMatrix = Matrix.CreateWorld(hitPoint, dirMatrix.Forward, dirMatrix.Up);
            }
        }

        //  Create smoke and debris particle at the place of voxel/model hit
        static void CreateBasicHitParticles(ref Vector3 hitPoint, ref Vector3 normal, ref Vector3 direction, MyEntity physObject, MyEntity weapon, MyEntity ownerEntity = null)
        {
            Vector3 reflectedDirection = Vector3.Reflect(direction, normal);
            MyUtilRandomVector3ByDeviatingVector randomVector = new MyUtilRandomVector3ByDeviatingVector(reflectedDirection);

            if (MyCamera.GetDistanceWithFOV(hitPoint) < 200)
            {
                MyParticleEffect effect = MyParticlesManager.CreateParticleEffect((int)MyParticleEffectsIDEnum.Hit_BasicAmmo);
                Matrix dirMatrix = MyMath.MatrixFromDir(reflectedDirection);
                effect.WorldMatrix = Matrix.CreateWorld(hitPoint, dirMatrix.Forward, dirMatrix.Up);
            }
        }

        static MyParticleEffect GetEffectForWeapon(MyEntity weapon, int effectID)
        {
            Dictionary<int, MyParticleEffect> effects;
            m_hitParticles.TryGetValue(weapon, out effects);
            if (effects == null)
            {
                effects = new Dictionary<int, MyParticleEffect>();
                m_hitParticles.Add(weapon, effects);
            }

            MyParticleEffect effect;
            effects.TryGetValue(effectID, out effect);

            if (effect == null)
            {
                effect = MyParticlesManager.CreateParticleEffect(effectID);
                effects.Add(effectID, effect);
                effect.Tag = weapon;
                effect.OnDelete += new EventHandler(effect_OnDelete);
            }
            else
            {
                effect.Restart();
            }

            return effect;
        }

        static void effect_OnDelete(object sender, EventArgs e)
        {
            MyParticleEffect effect = (MyParticleEffect)sender;
            MyEntity weapon = (MyEntity)effect.Tag;
            Dictionary<int, MyParticleEffect> effects = m_hitParticles[weapon];
            effects.Remove(effect.GetID());
            if (effects.Count == 0)
                m_hitParticles.Remove(weapon);
        }

        //  Create smoke and debris particle at the place of voxel/model hit
        static void CreateBasicHitAutocannonParticles(ref Vector3 hitPoint, ref Vector3 normal, ref Vector3 direction, MyEntity physObject, MyEntity weapon, MyEntity ownerEntity = null)
        {
            MyParticleEffect effect = GetEffectForWeapon(weapon, (int)MyParticleEffectsIDEnum.Hit_AutocannonBasicAmmo);

            Vector3 reflectedDirection = Vector3.Reflect(direction, normal);
            MyUtilRandomVector3ByDeviatingVector randomVector = new MyUtilRandomVector3ByDeviatingVector(reflectedDirection);
            Matrix dirMatrix = MyMath.MatrixFromDir(reflectedDirection);
            effect.WorldMatrix = Matrix.CreateWorld(hitPoint, dirMatrix.Forward, dirMatrix.Up);
        }       

        //  Create smoke and debris particle at the place of voxel/model hit
        static void CreateHighSpeedHitAutocannonParticles(ref Vector3 hitPoint, ref Vector3 normal, ref Vector3 direction, MyEntity physObject, MyEntity weapon, MyEntity ownerEntity = null)
        {
            Vector3 reflectedDirection = Vector3.Reflect(direction, normal);
            MyUtilRandomVector3ByDeviatingVector randomVector = new MyUtilRandomVector3ByDeviatingVector(reflectedDirection);

            MyParticleEffect effect = GetEffectForWeapon(weapon, (int)MyParticleEffectsIDEnum.Hit_AutocannonHighSpeedAmmo);

            Matrix dirMatrix = MyMath.MatrixFromDir(reflectedDirection);
            effect.WorldMatrix = Matrix.CreateWorld(hitPoint, dirMatrix.Forward, dirMatrix.Up);
        }

        //  Create smoke and debris particle at the place of voxel/model hit
        static void CreateExplosiveHitAutocannonParticles(ref Vector3 hitPoint, ref Vector3 normal, ref Vector3 direction, MyEntity physObject, MyEntity weapon, MyEntity ownerEntity = null)
        {
            Vector3 reflectedDirection = Vector3.Reflect(direction, normal);
            MyUtilRandomVector3ByDeviatingVector randomVector = new MyUtilRandomVector3ByDeviatingVector(reflectedDirection);

            MyParticleEffect effect = GetEffectForWeapon(weapon, (int)MyParticleEffectsIDEnum.Hit_AutocannonExplosiveAmmo);

            Matrix dirMatrix = MyMath.MatrixFromDir(reflectedDirection);
            effect.WorldMatrix = Matrix.CreateWorld(hitPoint, dirMatrix.Forward, dirMatrix.Up);
        }

        //  Create smoke and debris particle at the place of voxel/model hit
        static void CreateChemicalHitAutocannonParticles(ref Vector3 hitPoint, ref Vector3 normal, ref Vector3 direction, MyEntity physObject, MyEntity weapon, MyEntity ownerEntity = null)
        {
            Vector3 reflectedDirection = Vector3.Reflect(direction, normal);
            MyUtilRandomVector3ByDeviatingVector randomVector = new MyUtilRandomVector3ByDeviatingVector(reflectedDirection);

            MyParticleEffect effect = GetEffectForWeapon(weapon, (int)MyParticleEffectsIDEnum.Hit_AutocannonChemicalAmmo);
            Matrix dirMatrix = MyMath.MatrixFromDir(reflectedDirection);
            effect.WorldMatrix = Matrix.CreateWorld(hitPoint, dirMatrix.Forward, dirMatrix.Up);
        }

        //  Create smoke and debris particle at the place of voxel/model hit
        static void CreateEMPHitAutocannonParticles(ref Vector3 hitPoint, ref Vector3 normal, ref Vector3 direction, MyEntity physObject, MyEntity weapon, MyEntity ownerEntity = null)
        {
            Vector3 reflectedDirection = Vector3.Reflect(direction, normal);
            MyUtilRandomVector3ByDeviatingVector randomVector = new MyUtilRandomVector3ByDeviatingVector(reflectedDirection);

            MyParticleEffect effect = GetEffectForWeapon(weapon, (int)MyParticleEffectsIDEnum.Hit_AutocannonEMPAmmo);
            Matrix dirMatrix = MyMath.MatrixFromDir(reflectedDirection);
            effect.WorldMatrix = Matrix.CreateWorld(hitPoint, dirMatrix.Forward, dirMatrix.Up);
        }

        //  Create smoke and debris particle at the place of voxel/model hit
        static void CreatePiercingHitAutocannonParticles(ref Vector3 hitPoint, ref Vector3 normal, ref Vector3 direction, MyEntity physObject, MyEntity weapon, MyEntity ownerEntity = null)
        {
            Vector3 reflectedDirection = Vector3.Reflect(direction, normal);
            MyUtilRandomVector3ByDeviatingVector randomVector = new MyUtilRandomVector3ByDeviatingVector(reflectedDirection);

            MyParticleEffect effect = GetEffectForWeapon(weapon, (int)MyParticleEffectsIDEnum.Hit_AutocannonPiercingAmmo);
            Matrix dirMatrix = MyMath.MatrixFromDir(reflectedDirection);
            effect.WorldMatrix = Matrix.CreateWorld(hitPoint, dirMatrix.Forward, dirMatrix.Up);
        }

        //  Create smoke and debris particle at the place of voxel/model hit
        static void CreatePiercingHitParticles(ref Vector3 hitPoint, ref Vector3 normal, ref Vector3 direction, MyEntity physObject, MyEntity weapon, MyEntity ownerEntity = null)
        {
            Vector3 reflectedDirection = Vector3.Reflect(direction, normal);
            MyUtilRandomVector3ByDeviatingVector randomVector = new MyUtilRandomVector3ByDeviatingVector(reflectedDirection);

            MyParticleEffect effect = GetEffectForWeapon(weapon, (int)MyParticleEffectsIDEnum.Hit_PiercingAmmo);
            Matrix dirMatrix = MyMath.MatrixFromDir(reflectedDirection);
            effect.WorldMatrix = Matrix.CreateWorld(hitPoint, dirMatrix.Forward, dirMatrix.Up);
        }

        //  Create smoke and debris particle at the place of voxel/model hit from explosive projectil
        static void CreateExplosiveHitParticles(ref Vector3 hitPoint, ref Vector3 normal, ref Vector3 direction, MyEntity physObject, MyEntity weapon, MyEntity ownerEntity = null)
        {
            if (MyMwcUtils.GetRandomFloat(0.0f, 1.0f) > MyShotgunConstants.EXPLOSIVE_PROJECTILE_RELIABILITY)
            { //this projectile failed to explode

                Vector3 reflectedDirection = Vector3.Reflect(direction, normal);
                MyUtilRandomVector3ByDeviatingVector randomVector = new MyUtilRandomVector3ByDeviatingVector(reflectedDirection);

                MyParticleEffect effect = GetEffectForWeapon(weapon, (int)MyParticleEffectsIDEnum.Hit_ExplosiveAmmo);
                Matrix dirMatrix = MyMath.MatrixFromDir(reflectedDirection);
                effect.WorldMatrix = Matrix.CreateWorld(hitPoint, dirMatrix.Forward, dirMatrix.Up);
            }
            else
            {  //this projectile exploded
                MyExplosion newExplosion = MyExplosions.AddExplosion();
                if (newExplosion != null)
                {
                    float radius = MyMwcUtils.GetRandomFloat(5, 20);
                    newExplosion.Start(0, 0, 0, MyExplosionTypeEnum.AMMO_EXPLOSION, new BoundingSphere(hitPoint, radius), 1, MyExplosionForceDirection.EXPLOSION, MyGroupMask.Empty, false,ownerEntity: ownerEntity, hitEntity: physObject);
                }
            }
        }

        //  Create smoke and debris particle at the place of voxel/model hit
        static void CreateBiochemHitParticles(ref Vector3 hitPoint, ref Vector3 normal, ref Vector3 direction, MyEntity physObject, MyEntity weapon, MyEntity ownerEntity = null)
        {
            Vector3 reflectedDirection = Vector3.Reflect(direction, normal);
            MyUtilRandomVector3ByDeviatingVector randomVector = new MyUtilRandomVector3ByDeviatingVector(reflectedDirection);

            MyParticleEffect effect = GetEffectForWeapon(weapon, (int)MyParticleEffectsIDEnum.Hit_ChemicalAmmo);
            Matrix dirMatrix = MyMath.MatrixFromDir(reflectedDirection);
            effect.WorldMatrix = Matrix.CreateWorld(hitPoint, dirMatrix.Forward, dirMatrix.Up);
        }
        
        //  Create smoke and debris particle at the place of voxel/model hit
        static void CreateEMPHitParticles(ref Vector3 hitPoint, ref Vector3 normal, ref Vector3 direction, MyEntity physObject, MyEntity weapon, MyEntity ownerEntity = null)
        {
            Vector3 reflectedDirection = Vector3.Reflect(direction, normal);
            MyUtilRandomVector3ByDeviatingVector randomVector = new MyUtilRandomVector3ByDeviatingVector(reflectedDirection);

            MyParticleEffect effect = GetEffectForWeapon(weapon, (int)MyParticleEffectsIDEnum.Hit_EMPAmmo);
            Matrix dirMatrix = MyMath.MatrixFromDir(reflectedDirection);
            effect.WorldMatrix = Matrix.CreateWorld(hitPoint, dirMatrix.Forward, dirMatrix.Up);
        }

        static void CreateHitMaterialParticles(ref Vector3 hitPoint, ref Vector3 normal, ref Vector3 direction, MyEntity physObject, MySurfaceImpactEnum surfaceImpact, MyEntity weapon)
        {
            Vector3 reflectedDirection = Vector3.Reflect(direction, normal);

            MyParticleEffect effect = null;
            switch (surfaceImpact)
            {
                case MySurfaceImpactEnum.METAL:
                    effect = GetEffectForWeapon(weapon, (int)MyParticleEffectsIDEnum.MaterialHit_Metal);
                    break;
                case MySurfaceImpactEnum.DESTRUCTIBLE:
                    effect = GetEffectForWeapon(weapon, (int)MyParticleEffectsIDEnum.MaterialHit_Destructible);
                    break;
                case MySurfaceImpactEnum.INDESTRUCTIBLE:
                    effect = GetEffectForWeapon(weapon, (int)MyParticleEffectsIDEnum.MaterialHit_Indestructible);
                    break;
                default:
                    System.Diagnostics.Debug.Assert(false);
                    break;
            }
            Matrix dirMatrix = MyMath.MatrixFromDir(reflectedDirection);
            effect.WorldMatrix = Matrix.CreateWorld(hitPoint, dirMatrix.Forward, dirMatrix.Up);
        }

        static void CreateAutocannonHitMaterialParticles(ref Vector3 hitPoint, ref Vector3 normal, ref Vector3 direction, MyEntity physObject, MySurfaceImpactEnum surfaceImpact, MyEntity weapon)
        {
            Vector3 reflectedDirection = Vector3.Reflect(direction, normal);

            MyParticleEffect effect = null;
            switch (surfaceImpact)
            {
                case MySurfaceImpactEnum.METAL:
                    effect = GetEffectForWeapon(weapon, (int)MyParticleEffectsIDEnum.MaterialHit_Autocannon_Metal);
                    break;
                case MySurfaceImpactEnum.DESTRUCTIBLE:
                    effect = GetEffectForWeapon(weapon, (int)MyParticleEffectsIDEnum.MaterialHit_Autocannon_Destructible);
                    break;
                case MySurfaceImpactEnum.INDESTRUCTIBLE:
                    effect = GetEffectForWeapon(weapon, (int)MyParticleEffectsIDEnum.MaterialHit_Autocannon_Indestructible);
                    break;
                default:
                    System.Diagnostics.Debug.Assert(false);
                    break;
            }
            Matrix dirMatrix = MyMath.MatrixFromDir(reflectedDirection);
            effect.WorldMatrix = Matrix.CreateWorld(hitPoint, dirMatrix.Forward, dirMatrix.Up);
        }
    }
}
