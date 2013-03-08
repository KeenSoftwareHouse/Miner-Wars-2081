using System;
using System.Collections.Generic;
using MinerWars.AppCode.Physics;
using MinerWars.AppCode.Game.Audio;
using MinerWars.AppCode.Game.Explosions;
using MinerWars.AppCode.Game.Decals;

namespace MinerWars.AppCode.Game
{
    //  Material type of a physical object. This value determine sound of collision, decal type, explosion type, etc.
    public enum MyMaterialType
    {
        ROCK,
        METAL,
        GLASS,
        SHIP,
        PLAYERSHIP,
        AMMO,
    }

    namespace Utils
    {
        internal struct MyMaterialTypeProperties
        {
            public MyExplosionParticlesTypeEnum ExplosionParticles; //Particles which should be made after explosion of this material
            public float BulletHoleSizeMin, BulletHoleSizeMax; //Size of bullet hole
            public MyDecalTexturesEnum BulletHoleDecal; //Texture of basic bullet hole 
            public MySoundCuesEnum BulletHitCue; //Sound of hit by basic bullet
            public MySoundCuesEnum ExpBulletHitCue; //Sound of hit by explosive bullet

            public MyRBMaterial PhysicsMaterial; //Physics material
            public bool DoSparksOnCollision;
            public bool DoSmokeOnCollision;
        }

        static class MyMaterialsConstants
        {
            public enum MyMaterialCollisionType
            {
                Start,
                Touch,
                End
            }

            static int MyMaterialTypeLength = Enum.GetValues(typeof(MyMaterialType)).Length;
            static int MyMaterialCollisionTypeLength = Enum.GetValues(typeof(MyMaterialCollisionType)).Length;
            static MySoundCuesEnum?[,,] CollisionCues = new MySoundCuesEnum?[MyMaterialCollisionTypeLength, MyMaterialTypeLength, MyMaterialTypeLength];
            static Dictionary<int, MyMaterialTypeProperties> MaterialProperties = new Dictionary<int, MyMaterialTypeProperties>();

            static MyMaterialsConstants()
            {
                //Rock material
                MaterialProperties.Add((int)MyMaterialType.ROCK, new MyMaterialTypeProperties()
                {
                    ExplosionParticles = MyExplosionParticlesTypeEnum.EXPLOSIVE_AND_DIRTY,
                    BulletHoleSizeMin = 0.5f,
                    BulletHoleSizeMax = 1.0f,
                    BulletHoleDecal = MyDecalTexturesEnum.BulletHoleOnRock,
                    BulletHitCue = MySoundCuesEnum.ImpBulletHitRock,
                    ExpBulletHitCue = MySoundCuesEnum.ImpExpHitRock,
                    PhysicsMaterial = new MyRBMaterial(MyConstants.PHYSICS_STANDARD_STATIC_FRICTION, MyConstants.PHYSICS_STANDARD_DYNAMIC_FRICTION,
                                    MyConstants.PHYSICS_STANDARD_RESTITUTION, 0),
                    DoSparksOnCollision = false,
                    DoSmokeOnCollision = true,
                });

                //Metal material
                MaterialProperties.Add((int)MyMaterialType.METAL, new MyMaterialTypeProperties()
                {
                    ExplosionParticles = MyExplosionParticlesTypeEnum.EXPLOSIVE_ONLY,
                    BulletHoleSizeMin = 0.33f,
                    BulletHoleSizeMax = 0.49f,
                    BulletHoleDecal = MyDecalTexturesEnum.BulletHoleOnMetal,
                    BulletHitCue = MySoundCuesEnum.ImpBulletHitMetal,
                    ExpBulletHitCue = MySoundCuesEnum.ImpExpHitMetal,
                    PhysicsMaterial = new MyRBMaterial(MyConstants.PHYSICS_STANDARD_STATIC_FRICTION, MyConstants.PHYSICS_STANDARD_DYNAMIC_FRICTION,
                                    MyConstants.PHYSICS_STANDARD_RESTITUTION, 1),
                    DoSparksOnCollision = true,
                    DoSmokeOnCollision = false,
                });

                //Glass material
                MaterialProperties.Add((int)MyMaterialType.GLASS, new MyMaterialTypeProperties()
                {
                    ExplosionParticles = MyExplosionParticlesTypeEnum.EXPLOSIVE_ONLY,
                    BulletHoleSizeMin = 0.5f,
                    BulletHoleSizeMax = 1.0f,
                    BulletHoleDecal = MyDecalTexturesEnum.BulletHoleOnMetal,
                    BulletHitCue = MySoundCuesEnum.ImpBulletHitGlass,
                    ExpBulletHitCue = MySoundCuesEnum.ImpExpHitGlass,
                    PhysicsMaterial = new MyRBMaterial(MyConstants.PHYSICS_STANDARD_STATIC_FRICTION, MyConstants.PHYSICS_STANDARD_DYNAMIC_FRICTION,
                                    MyConstants.PHYSICS_STANDARD_RESTITUTION, 2),
                    DoSparksOnCollision = false,
                    DoSmokeOnCollision = false,
                });

                //Ship material
                MaterialProperties.Add((int)MyMaterialType.SHIP, new MyMaterialTypeProperties()
                {
                    ExplosionParticles = MyExplosionParticlesTypeEnum.EXPLOSIVE_ONLY,
                    BulletHoleSizeMin = 0.165f,
                    BulletHoleSizeMax = 0.33f,
                    BulletHoleDecal = MyDecalTexturesEnum.BulletHoleOnMetal,
                    BulletHitCue = MySoundCuesEnum.ImpBulletHitMetal,
                    ExpBulletHitCue = MySoundCuesEnum.ImpExpHitMetal,
                    PhysicsMaterial = new MyRBMaterial(MyConstants.PHYSICS_STANDARD_STATIC_FRICTION, MyConstants.PHYSICS_STANDARD_DYNAMIC_FRICTION,
                                    MyConstants.PHYSICS_BOT_RESTITUTION, 3),
                    DoSparksOnCollision = true,
                    DoSmokeOnCollision = false,
                });

                //Player ship material (handled in MyAudio)
                MaterialProperties.Add((int)MyMaterialType.PLAYERSHIP, new MyMaterialTypeProperties()
                {
                    ExplosionParticles = MyExplosionParticlesTypeEnum.EXPLOSIVE_ONLY,
                    BulletHoleSizeMin = 0.1f,
                    BulletHoleSizeMax = 0.2f,
                    BulletHoleDecal = MyDecalTexturesEnum.BulletHoleOnMetal,
                    BulletHitCue = MySoundCuesEnum.ImpBulletHitShip,
                    ExpBulletHitCue = MySoundCuesEnum.ImpExpHitShip,
                    PhysicsMaterial = new MyRBMaterial(0.05f, 0.01f, 0.001f, 4),
                    DoSparksOnCollision = true,
                    DoSmokeOnCollision = false,
                });

                //Metal material
                MaterialProperties.Add((int)MyMaterialType.AMMO, new MyMaterialTypeProperties()
                {
                    ExplosionParticles = MyExplosionParticlesTypeEnum.EXPLOSIVE_ONLY,
                    BulletHoleSizeMin = 0.33f,
                    BulletHoleSizeMax = 0.49f,
                    BulletHoleDecal = MyDecalTexturesEnum.BulletHoleOnMetal,
                    BulletHitCue = MySoundCuesEnum.ImpBulletHitMetal,
                    ExpBulletHitCue = MySoundCuesEnum.ImpExpHitMetal,
                    PhysicsMaterial = new MyRBMaterial(MyConstants.PHYSICS_STANDARD_STATIC_FRICTION, MyConstants.PHYSICS_STANDARD_DYNAMIC_FRICTION,
                                    MyConstants.PHYSICS_AMMO_RESTITUTION, 1),
                    DoSparksOnCollision = true,
                    DoSmokeOnCollision = false,
                });

                //Default collision sounds
                for (int t = 0; t < MyMaterialCollisionTypeLength; t++)
                    for (int i =0; i < MyMaterialTypeLength; i++)
                        for (int j = 0; j < MyMaterialTypeLength; j++)
                        {
                            AddCollisionCue((MyMaterialCollisionType)t, (MyMaterialType)i, (MyMaterialType)j, MySoundCuesEnum.ImpRockCollideMetal);
                        }

                //Collision sounds
                AddCollisionCue(MyMaterialCollisionType.Start, MyMaterialType.SHIP, MyMaterialType.METAL, MySoundCuesEnum.ImpShipCollideMetal);
                AddCollisionCue(MyMaterialCollisionType.Start, MyMaterialType.SHIP, MyMaterialType.AMMO, MySoundCuesEnum.ImpShipCollideMetal);
                AddCollisionCue(MyMaterialCollisionType.Start, MyMaterialType.SHIP, MyMaterialType.ROCK, MySoundCuesEnum.ImpShipCollideRock);
                AddCollisionCue(MyMaterialCollisionType.Start, MyMaterialType.ROCK, MyMaterialType.METAL, MySoundCuesEnum.ImpRockCollideMetal);
                AddCollisionCue(MyMaterialCollisionType.Start, MyMaterialType.ROCK, MyMaterialType.AMMO, MySoundCuesEnum.ImpRockCollideMetal);
                AddCollisionCue(MyMaterialCollisionType.Start, MyMaterialType.ROCK, MyMaterialType.ROCK, MySoundCuesEnum.ImpRockCollideRock);
                AddCollisionCue(MyMaterialCollisionType.Start, MyMaterialType.PLAYERSHIP, MyMaterialType.METAL, MySoundCuesEnum.ImpPlayerShipCollideMetal);
                AddCollisionCue(MyMaterialCollisionType.Start, MyMaterialType.PLAYERSHIP, MyMaterialType.AMMO, MySoundCuesEnum.ImpPlayerShipCollideMetal);
                AddCollisionCue(MyMaterialCollisionType.Start, MyMaterialType.PLAYERSHIP, MyMaterialType.ROCK, MySoundCuesEnum.ImpPlayerShipCollideRock);
                AddCollisionCue(MyMaterialCollisionType.Start, MyMaterialType.PLAYERSHIP, MyMaterialType.SHIP, MySoundCuesEnum.ImpPlayerShipCollideShip);
                
                //Scrape sounds
                AddCollisionCue(MyMaterialCollisionType.Touch, MyMaterialType.PLAYERSHIP, MyMaterialType.METAL, MySoundCuesEnum.ImpPlayerShipScrapeShipLoop);
                AddCollisionCue(MyMaterialCollisionType.Touch, MyMaterialType.PLAYERSHIP, MyMaterialType.AMMO, MySoundCuesEnum.ImpPlayerShipScrapeShipLoop);
                AddCollisionCue(MyMaterialCollisionType.Touch, MyMaterialType.PLAYERSHIP, MyMaterialType.ROCK, MySoundCuesEnum.ImpPlayerShipScrapeShipLoop);
                AddCollisionCue(MyMaterialCollisionType.Touch, MyMaterialType.PLAYERSHIP, MyMaterialType.SHIP, MySoundCuesEnum.ImpPlayerShipScrapeShipLoop);

                AddCollisionCue(MyMaterialCollisionType.End, MyMaterialType.PLAYERSHIP, MyMaterialType.SHIP, null);
                AddCollisionCue(MyMaterialCollisionType.End, MyMaterialType.PLAYERSHIP, MyMaterialType.METAL, null);
                AddCollisionCue(MyMaterialCollisionType.End, MyMaterialType.PLAYERSHIP, MyMaterialType.AMMO, null);
                AddCollisionCue(MyMaterialCollisionType.End, MyMaterialType.PLAYERSHIP, MyMaterialType.ROCK, null);
            }

            public static MyMaterialTypeProperties GetMaterialProperties(MyMaterialType materialType)
            {
                return MaterialProperties[(int)materialType];
            }

            public static void AddCollisionCue(MyMaterialCollisionType type,MyMaterialType materialType1, MyMaterialType materialType2, MySoundCuesEnum? cue)
            {
                CollisionCues[(int)type, (int)materialType1, (int)materialType2] = cue;
                CollisionCues[(int)type, (int)materialType2, (int)materialType1] = cue;
            }

            public static MySoundCuesEnum? GetCollisionCue(MyMaterialCollisionType type, MyMaterialType materialType1, MyMaterialType materialType2)
            {
                return CollisionCues[(int)type, (int)materialType1, (int)materialType2];
            }
        }
    }
}