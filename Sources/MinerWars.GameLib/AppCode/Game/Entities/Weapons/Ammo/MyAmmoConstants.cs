using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.Entities.Weapons;
using MinerWarsMath;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.AppCode.Game.TransparentGeometry;
using MinerWars.AppCode.Game.Audio;
using MinerWars.AppCode.Game.Utils;
using System;
using SysUtils.Utils;
using System.IO;
using System.Text;

namespace MinerWars.AppCode.Game.Gameplay
{
    internal class MyAmmoProperties
    {
        public readonly float InitialSpeed;     // In metres/second
        public readonly float DesiredSpeed;     // In metres/second
        public readonly float SpeedVar;         // speed *= MyMwcUtils.GetRandomFloat(1.0f - SpeedVar, 1.0f + SpeedVar)
        public readonly float MaxTrajectory;    // How far can projectile fly before we kill it (it's like distance timeout)
        public readonly float HealthDamage;     // Damage for living entities in hit entity
        public readonly float ShipDamage;       // Damage for hit entity
        public readonly float EMPDamage;        // EMP damage for hit entity
        public readonly float ExplosionRadius;  // Radius to create voxel damage
        public readonly float DeviateAngle;     // Deviating angle in radians
        public readonly Vector3 TrailColor;     // Color of ammo trail, if any
        public readonly MySoundCuesEnum ShotSound; // Sound when ammo is shot
        public readonly bool IsExplosive;       // Ammo explodes with some probability
        public readonly bool AllowAimCorrection;// Ammo is corrected by autoaiming
        public readonly float DecalEmissivity;    // Decals shines in dark
        public readonly int ProjectileGroupSize; // Group size, for example for shotgun

        public readonly MyDamageType DamageType; //Type of damage caused by this ammo
        public readonly MyAmmoType AmmoType; //Type of ammo

        public readonly MyCustomHitParticlesMethod OnHitParticles;  // Delegate to method to create hit particles
        public readonly MyCustomHitMaterialMethod OnHitMaterialSpecificParticles; // Delegate to method to create material specific particles

        public MyAmmoProperties(float InitialSpeed,
                                float DesiredSpeed,
                                float SpeedVar,
                                float MaxTrajectory,
                                float HealthDamage,
                                float ShipDamage,
                                float EMPDamage,
                                float ExplosionRadius,
                                float DeviateAngle,
                                Vector3 TrailColor,
                                MySoundCuesEnum ShotSound,
                                bool IsExplosive,
                                bool AllowAimCorrection,
                                float DecalEmissivity,
                                int ProjectileGroupSize,
                                MyDamageType DamageType,
                                MyAmmoType AmmoType,

                                MyCustomHitParticlesMethod OnHitParticles,
                                MyCustomHitMaterialMethod OnHitMaterialSpecificParticles)
        {
            this.InitialSpeed = InitialSpeed;
            this.DesiredSpeed = DesiredSpeed;
            this.SpeedVar = SpeedVar;
            this.MaxTrajectory = MaxTrajectory;
            this.HealthDamage = HealthDamage;
            this.ShipDamage = ShipDamage;
            this.EMPDamage = EMPDamage;
            this.ExplosionRadius = ExplosionRadius;
            this.DeviateAngle = DeviateAngle;
            this.TrailColor = TrailColor;
            this.ShotSound = ShotSound;
            this.IsExplosive = IsExplosive;
            this.AllowAimCorrection = AllowAimCorrection;
            this.DecalEmissivity = DecalEmissivity;
            this.ProjectileGroupSize = ProjectileGroupSize;

            this.DamageType = DamageType;
            this.AmmoType = AmmoType;

            this.OnHitParticles = OnHitParticles;
            this.OnHitMaterialSpecificParticles = OnHitMaterialSpecificParticles;
        }
    }

    enum MyAmmoPropertiesEnum
    {
        Shrapnel = MyAmmoConstants.AMMO_ENUM_START
    }


    static class MyAmmoConstants
    {
        public const int AMMO_ENUM_START = 100;
        public const float ArmorEffectivityVsPiercingAmmo = 0.5f;

        static readonly MyAmmoProperties[] m_ammoProperties = new MyAmmoProperties[MyMwcUtils.GetMaxValueFromEnum<MyAmmoPropertiesEnum>() + 1];

        // HACK: HUGE MULTIPLAYER HACK
        [Obsolete]
        internal static MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum FindAmmo(MyAmmoProperties ammoProperties)
        {
            for (int i = 0; i < m_ammoProperties.Length; i++)
            {
                if (m_ammoProperties[i] == ammoProperties)
                {
                    return (MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum)i;
                }
            }
            return MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_Basic;
        }

        public static void GenerateDebugAmmoTypeInfo()
        {
            string format = "{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}";
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format(format, "ID", "Ship damage", "Player damage", "EMP damage", "Desired speed", "Explosion radius", "Range"));
            foreach (MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum ammoType in Enum.GetValues(typeof(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum)))
            {
                if (MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.MyMwcObjectBuilder_InventoryItem.IsDisabled(MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)ammoType)) 
                {
                    continue;
                }
                var ammoProp = GetAmmoProperties(ammoType);
                sb.AppendLine(string.Format(format, (int)ammoType, ammoProp.ShipDamage, ammoProp.HealthDamage, ammoProp.EMPDamage, ammoProp.DesiredSpeed > 0f ? ammoProp.DesiredSpeed : ammoProp.InitialSpeed, ammoProp.ExplosionRadius, ammoProp.MaxTrajectory));
            }
            string directory = @"C:\Temp";
            string fileName = directory + @"\AmmoInfo.txt";

            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            using (var sw = File.CreateText(fileName))
            {
                sw.Write(sb.ToString());
                sw.Close();
            }
        }

        internal static void LoadData()
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyAmmoConstants.LoadData");

            System.Diagnostics.Debug.Assert(MyMwcUtils.GetMaxValueFromEnum<MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum>() < AMMO_ENUM_START);

            m_ammoProperties[(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_Basic] = 
                new MyAmmoProperties(
                    InitialSpeed: 0,
                    DesiredSpeed: 2000,
                    SpeedVar: 0,
                    MaxTrajectory: 2800,
                    HealthDamage: 0.0f,
                    //ShipDamage: 3.0f, // default 6
                    ShipDamage: 3.9f, // default 6
                    EMPDamage: 0.0f,
                    ExplosionRadius: 0,
                    DeviateAngle: MathHelper.ToRadians(0.17f),
                    TrailColor: Vector3.One,
                    ShotSound: MySoundCuesEnum.WepAutocanonFire3d,
                    IsExplosive: false,
                    AllowAimCorrection: true,
                    DecalEmissivity: 0.5f,
                    ProjectileGroupSize: 1,

                    DamageType: MyDamageType.Bullet,
                    AmmoType: MyAmmoType.Basic,

                    OnHitParticles: MyParticleEffects.DelegateForCreateAutocannonBasicHitParticles,
                    OnHitMaterialSpecificParticles: MyParticleEffects.DelegateForCreateAutocannonHitMaterialParticles
                   );
            
            m_ammoProperties[(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_High_Speed] =
               new MyAmmoProperties(
                   InitialSpeed: 0,
                   DesiredSpeed: 3000,
                   SpeedVar: 0,
                   MaxTrajectory: 3000,
                   HealthDamage: 0.0f,
                   //ShipDamage: 4.5f,
                   ShipDamage: 5.85f,
                   EMPDamage: 0.0f,
                   ExplosionRadius: 0,
                   DeviateAngle: MathHelper.ToRadians(0.17f),
                   TrailColor: MyProjectilesConstants.HIGH_SPEED_PROJECTILE_TRAIL_COLOR,
                   ShotSound: MySoundCuesEnum.WepAutocanonFire3d,
                   IsExplosive: false,
                   AllowAimCorrection: true,
                   DecalEmissivity: 1.0f,
                   ProjectileGroupSize: 1,

                   DamageType: MyDamageType.Bullet,
                   AmmoType: MyAmmoType.HighSpeed,

                   OnHitParticles: MyParticleEffects.DelegateForCreateAutocannonHighSpeedHitParticles,
                   OnHitMaterialSpecificParticles: MyParticleEffects.DelegateForCreateAutocannonHitMaterialParticles
                  );

            m_ammoProperties[(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_Armor_Piercing_Incendiary] =
               new MyAmmoProperties(
                   InitialSpeed: 0,
                   DesiredSpeed: 2000,
                   SpeedVar: 0,
                   MaxTrajectory: 2500,
                   HealthDamage: 0.0f,
                   //ShipDamage: 6f,
                   ShipDamage: 7.8f,
                   EMPDamage: 0.0f,
                   ExplosionRadius: 0,
                   DeviateAngle: MathHelper.ToRadians(0.17f),
                   TrailColor: MyProjectilesConstants.PIERCING_PROJECTILE_TRAIL_COLOR,
                   ShotSound: MySoundCuesEnum.WepAutocanonFire3d,
                   IsExplosive: false,
                   AllowAimCorrection: true,
                   DecalEmissivity: 1.0f,
                   ProjectileGroupSize: 1,

                   DamageType: MyDamageType.Bullet,
                   AmmoType: MyAmmoType.Piercing,

                   OnHitParticles: MyParticleEffects.DelegateForCreateAutocannonPiercingHitParticles,
                   OnHitMaterialSpecificParticles: MyParticleEffects.DelegateForCreateAutocannonHitMaterialParticles
                  );

            m_ammoProperties[(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_BioChem] =
               new MyAmmoProperties(
                   InitialSpeed: 0,
                   DesiredSpeed: 1860,
                   SpeedVar: 0,
                   MaxTrajectory: 1800,
                   HealthDamage: 10f,
                   ShipDamage: 2f,
                   EMPDamage: 0.0f,
                   ExplosionRadius: 0,
                   DeviateAngle: MathHelper.ToRadians(0.17f),
                   TrailColor: MyProjectilesConstants.BIOCHEM_PROJECTILE_TRAIL_COLOR,
                   ShotSound: MySoundCuesEnum.WepAutocanonFire3d,
                   IsExplosive: false,
                   AllowAimCorrection: true,
                   DecalEmissivity: 0.5f,
                   ProjectileGroupSize: 1,

                   DamageType: MyDamageType.Bullet,
                   AmmoType: MyAmmoType.Biochem,

                   OnHitParticles: MyParticleEffects.DelegateForCreateAutocannonBiochemHitParticles,
                   OnHitMaterialSpecificParticles: MyParticleEffects.DelegateForCreateAutocannonHitMaterialParticles
                  );

            m_ammoProperties[(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_SAPHEI] =
               new MyAmmoProperties(
                   InitialSpeed: 0,
                   DesiredSpeed: 2000,
                   SpeedVar: 0,
                   MaxTrajectory: 2200,
                   HealthDamage: 0.0f,
                   //ShipDamage: 7.5f,
                   ShipDamage: 9.75f,
                   EMPDamage: 0.0f,
                   ExplosionRadius: 0,
                   DeviateAngle: MathHelper.ToRadians(0.17f),
                   TrailColor: MyProjectilesConstants.EXPLOSIVE_PROJECTILE_TRAIL_COLOR,
                   ShotSound: MySoundCuesEnum.WepAutocanonFire3d,
                   IsExplosive: false,
                   AllowAimCorrection: true,
                   DecalEmissivity: 0.5f,
                   ProjectileGroupSize: 1,

                   DamageType: MyDamageType.Bullet,
                   AmmoType: MyAmmoType.Explosive,

                   OnHitParticles: MyParticleEffects.DelegateForCreateAutocannonExplosiveHitParticles,
                   OnHitMaterialSpecificParticles: MyParticleEffects.DelegateForCreateAutocannonHitMaterialParticles
                  );

            m_ammoProperties[(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Machine_Gun_Basic] =
               new MyAmmoProperties(
                   InitialSpeed: 0,
                   DesiredSpeed: 1700,
                   SpeedVar: 0,
                   MaxTrajectory: 2300,
                   HealthDamage: 0.0f,
                   ShipDamage: 6f,
                   EMPDamage: 0.0f,
                   ExplosionRadius: 0,
                   DeviateAngle: MathHelper.ToRadians(0.17f),
                   TrailColor: Vector3.One,
                   ShotSound: MySoundCuesEnum.WepMachineGunNormFire3d,
                   IsExplosive: false,
                   AllowAimCorrection: true,
                   DecalEmissivity: 0.5f,
                   ProjectileGroupSize: 1,

                   DamageType: MyDamageType.Bullet,
                   AmmoType: MyAmmoType.Basic,

                   OnHitParticles: MyParticleEffects.DelegateForCreateBasicHitParticles,
                   OnHitMaterialSpecificParticles: MyParticleEffects.DelegateForCreateHitMaterialParticles
                  );

            m_ammoProperties[(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Machine_Gun_High_Speed] =
               new MyAmmoProperties(
                   InitialSpeed: 0,
                   DesiredSpeed: 2200,
                   SpeedVar: 0,
                   MaxTrajectory: 2500,
                   HealthDamage: 0.0f,
                   ShipDamage: 9f,
                   EMPDamage: 0.0f,
                   ExplosionRadius: 0,
                   DeviateAngle: MathHelper.ToRadians(0.17f),
                   TrailColor: MyProjectilesConstants.HIGH_SPEED_PROJECTILE_TRAIL_COLOR,
                   ShotSound: MySoundCuesEnum.WepMachineGunHighFire3d,
                   IsExplosive: false,
                   AllowAimCorrection: true,
                   DecalEmissivity: 1.0f,
                   ProjectileGroupSize: 1,

                   DamageType: MyDamageType.Bullet,
                   AmmoType: MyAmmoType.HighSpeed,

                   OnHitParticles: MyParticleEffects.DelegateForCreateBasicHitParticles,
                   OnHitMaterialSpecificParticles: MyParticleEffects.DelegateForCreateHitMaterialParticles
                  );

            m_ammoProperties[(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Machine_Gun_Armor_Piercing_Incendiary] =
               new MyAmmoProperties(
                   InitialSpeed: 0,
                   DesiredSpeed: 1900,
                   SpeedVar: 0,
                   MaxTrajectory: 2100,
                   HealthDamage: 0.0f,
                   ShipDamage: 12f,
                   EMPDamage: 0.0f,
                   ExplosionRadius: 0,
                   DeviateAngle: MathHelper.ToRadians(0.17f),
                   TrailColor: MyProjectilesConstants.PIERCING_PROJECTILE_TRAIL_COLOR,
                   ShotSound: MySoundCuesEnum.WepMachineGunNormFire3d,
                   IsExplosive: false,
                   AllowAimCorrection: true,
                   DecalEmissivity: 1.0f,
                   ProjectileGroupSize: 1,

                   DamageType: MyDamageType.Bullet,
                   AmmoType: MyAmmoType.Piercing,

                   OnHitParticles: MyParticleEffects.DelegateForCreatePiercingHitParticles,
                   OnHitMaterialSpecificParticles: MyParticleEffects.DelegateForCreateHitMaterialParticles
                  );

            m_ammoProperties[(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Machine_Gun_BioChem] =
               new MyAmmoProperties(
                   InitialSpeed: 0,
                   DesiredSpeed: 900,
                   SpeedVar: 0,
                   MaxTrajectory: 1900,
                   HealthDamage: 30f,
                   ShipDamage: 3f,
                   EMPDamage: 0.0f,
                   ExplosionRadius: 0,
                   DeviateAngle: MathHelper.ToRadians(0.17f),
                   TrailColor: MyProjectilesConstants.BIOCHEM_PROJECTILE_TRAIL_COLOR,
                   ShotSound: MySoundCuesEnum.WepMachineGunNormFire3d,
                   IsExplosive: false,
                   AllowAimCorrection: true,
                   DecalEmissivity: 0.5f,
                   ProjectileGroupSize: 1,

                   DamageType: MyDamageType.Bullet,
                   AmmoType: MyAmmoType.Biochem,

                   OnHitParticles: MyParticleEffects.DelegateForCreateBiochemHitParticles,
                   OnHitMaterialSpecificParticles: MyParticleEffects.DelegateForCreateHitMaterialParticles
                  );

            m_ammoProperties[(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Machine_Gun_SAPHEI] =
               new MyAmmoProperties(
                   InitialSpeed: 0,
                   DesiredSpeed: 1900,
                   SpeedVar: 0,
                   MaxTrajectory: 1600,
                   HealthDamage: 0.0f,
                   ShipDamage: 15f,
                   EMPDamage: 0.0f,
                   ExplosionRadius: 0,
                   DeviateAngle: MathHelper.ToRadians(0.17f),
                   TrailColor: MyProjectilesConstants.EXPLOSIVE_PROJECTILE_TRAIL_COLOR,
                   ShotSound: MySoundCuesEnum.WepMachineGunNormFire3d,
                   IsExplosive: true,
                   AllowAimCorrection: true,
                   DecalEmissivity: 0.5f,
                   ProjectileGroupSize: 1,

                   DamageType: MyDamageType.Bullet,
                   AmmoType: MyAmmoType.Explosive,

                   OnHitParticles: MyParticleEffects.DelegateForCreateExplosiveHitParticles,
                   OnHitMaterialSpecificParticles: MyParticleEffects.DelegateForCreateHitMaterialParticles
                  );

            m_ammoProperties[(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Automatic_Rifle_With_Silencer_High_Speed] =
                new MyAmmoProperties(
                    InitialSpeed: 0,
                    DesiredSpeed: 1000,
                    SpeedVar: 0,
                    MaxTrajectory: 2000,
                    HealthDamage: 0.0f,
                    ShipDamage: 100.0f,
                    EMPDamage: 0.0f,
                    ExplosionRadius: 0,
                    DeviateAngle: MathHelper.ToRadians(0.17f),
                    TrailColor: MyProjectilesConstants.HIGH_SPEED_PROJECTILE_TRAIL_COLOR,
                    ShotSound: MySoundCuesEnum.WepArsHighShot3d,
                    IsExplosive: false,
                    AllowAimCorrection: true,
                    DecalEmissivity: 1.0f,
                    ProjectileGroupSize: 1,

                    DamageType: MyDamageType.Bullet,
                    AmmoType: MyAmmoType.HighSpeed,

                    OnHitParticles: MyParticleEffects.DelegateForCreateBasicHitParticles,
                    OnHitMaterialSpecificParticles: MyParticleEffects.DelegateForCreateHitMaterialParticles
                   );

            m_ammoProperties[(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Automatic_Rifle_With_Silencer_BioChem] =
                new MyAmmoProperties(
                    InitialSpeed: 0,
                    DesiredSpeed: 900,
                    SpeedVar: 0,
                    MaxTrajectory: 1500,
                    HealthDamage: 1000.0f,
                    ShipDamage: 50f,
                    EMPDamage: 0.0f,
                    ExplosionRadius: 0,
                    DeviateAngle: MathHelper.ToRadians(0.17f),
                    TrailColor: MyProjectilesConstants.BIOCHEM_PROJECTILE_TRAIL_COLOR,
                    ShotSound: MySoundCuesEnum.WepArsNormShot3d,
                    IsExplosive: false,
                    AllowAimCorrection: true,
                    DecalEmissivity: 0.5f,
                    ProjectileGroupSize: 1,

                    DamageType: MyDamageType.Bullet,
                    AmmoType: MyAmmoType.Biochem,

                    OnHitParticles: MyParticleEffects.DelegateForCreateBiochemHitParticles,
                    OnHitMaterialSpecificParticles: MyParticleEffects.DelegateForCreateHitMaterialParticles
                   );

            m_ammoProperties[(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Automatic_Rifle_With_Silencer_SAPHEI] =
                new MyAmmoProperties(
                    InitialSpeed: 0,
                    DesiredSpeed: 900,
                    SpeedVar: 0,
                    MaxTrajectory: 1800,
                    HealthDamage: 0.0f,
                    ShipDamage: 200f,
                    EMPDamage: 0.0f,
                    ExplosionRadius: 0,
                    DeviateAngle: MathHelper.ToRadians(0.17f),
                    TrailColor: MyProjectilesConstants.EXPLOSIVE_PROJECTILE_TRAIL_COLOR,
                    ShotSound: MySoundCuesEnum.WepArsNormShot3d,
                    IsExplosive: true,
                    AllowAimCorrection: true,
                    DecalEmissivity: 0.5f,
                    ProjectileGroupSize: 1,

                    DamageType: MyDamageType.Bullet,
                    AmmoType: MyAmmoType.Explosive,

                    OnHitParticles: MyParticleEffects.DelegateForCreateExplosiveHitParticles,
                    OnHitMaterialSpecificParticles: MyParticleEffects.DelegateForCreateHitMaterialParticles
                   );

            m_ammoProperties[(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Shotgun_Basic] =
                new MyAmmoProperties(
                    InitialSpeed: 0,
                    DesiredSpeed: 600,
                    SpeedVar: 0.2f,
                    MaxTrajectory: 300,
                    HealthDamage: 0.0f,
                    ShipDamage: 5f,
                    EMPDamage: 0.0f,
                    ExplosionRadius: 0,
                    DeviateAngle: MathHelper.ToRadians(5f),
                    TrailColor: Vector3.One,
                    ShotSound: MySoundCuesEnum.WepShotgunNormShot3d,
                    IsExplosive: false,
                    AllowAimCorrection: false,
                    DecalEmissivity: 0.5f,
                    ProjectileGroupSize: 20,

                    DamageType: MyDamageType.Bullet,
                    AmmoType: MyAmmoType.Basic,

                    OnHitParticles: MyParticleEffects.DelegateForCreateBasicHitParticles,
                    OnHitMaterialSpecificParticles: MyParticleEffects.DelegateForCreateHitMaterialParticles
                   );

            m_ammoProperties[(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Shotgun_High_Speed] =
                new MyAmmoProperties(
                    InitialSpeed: 0,
                    DesiredSpeed: 1200,
                    SpeedVar: 0.2f,
                    MaxTrajectory: 400,
                    HealthDamage: 0.0f,
                    ShipDamage: 8f,
                    EMPDamage: 0.0f,
                    ExplosionRadius: 0,
                    DeviateAngle: MathHelper.ToRadians(5f),
                    TrailColor: Vector3.One,
                    ShotSound: MySoundCuesEnum.WepShotgunHighShot3d,
                    IsExplosive: false,
                    AllowAimCorrection: false,
                    DecalEmissivity: 1.0f,
                    ProjectileGroupSize: 18,

                    DamageType: MyDamageType.Bullet,
                    AmmoType: MyAmmoType.HighSpeed,

                    OnHitParticles: MyParticleEffects.DelegateForCreateBasicHitParticles,
                    OnHitMaterialSpecificParticles: MyParticleEffects.DelegateForCreateHitMaterialParticles
                   );

            m_ammoProperties[(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Shotgun_Armor_Piercing] =
                new MyAmmoProperties(
                    InitialSpeed: 0,
                    DesiredSpeed: 800,
                    SpeedVar: 0.2f,
                    MaxTrajectory: 300,
                    HealthDamage: 0.0f,
                    ShipDamage: 10f,
                    EMPDamage: 0.0f,
                    ExplosionRadius: 0,
                    DeviateAngle: MathHelper.ToRadians(5f),
                    TrailColor: MyProjectilesConstants.PIERCING_PROJECTILE_TRAIL_COLOR,
                    ShotSound: MySoundCuesEnum.WepShotgunNormShot3d,
                    IsExplosive: false,
                    AllowAimCorrection: false,
                    DecalEmissivity: 1.0f,
                    ProjectileGroupSize: 18,

                    DamageType: MyDamageType.Bullet,
                    AmmoType: MyAmmoType.Piercing,

                    OnHitParticles: MyParticleEffects.DelegateForCreatePiercingHitParticles,
                    OnHitMaterialSpecificParticles: MyParticleEffects.DelegateForCreateHitMaterialParticles
                   );

            m_ammoProperties[(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Shotgun_Explosive] =
                new MyAmmoProperties(
                    InitialSpeed: 0,
                    DesiredSpeed: 700,
                    SpeedVar: 0.2f,
                    MaxTrajectory: 300,
                    HealthDamage: 0.0f,
                    ShipDamage: 10f,
                    EMPDamage: 0.0f,
                    ExplosionRadius: 0,
                    DeviateAngle: MathHelper.ToRadians(7f),
                    TrailColor: Vector3.One,
                    ShotSound: MySoundCuesEnum.WepShotgunNormShot3d,
                    IsExplosive: false,
                    AllowAimCorrection: false,
                    DecalEmissivity: 0.5f,
                    ProjectileGroupSize: 16,

                    DamageType: MyDamageType.Bullet,
                    AmmoType: MyAmmoType.Explosive,

                    OnHitParticles: MyParticleEffects.DelegateForCreateExplosiveHitParticles,
                    OnHitMaterialSpecificParticles: MyParticleEffects.DelegateForCreateHitMaterialParticles
                   );

            m_ammoProperties[(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Sniper_High_Speed] =
                new MyAmmoProperties(
                    InitialSpeed: 0,
                    DesiredSpeed: 15000,
                    SpeedVar: 0.0f,
                    MaxTrajectory: 12000,
                    HealthDamage: 0.0f,
                    ShipDamage: 60.0f,
                    EMPDamage: 0.0f,
                    ExplosionRadius: 0,
                    DeviateAngle: MathHelper.ToRadians(0.0f),
                    TrailColor: MyProjectilesConstants.HIGH_SPEED_PROJECTILE_TRAIL_COLOR,
                    ShotSound: MySoundCuesEnum.WepSniperHighFire3d,
                    IsExplosive: false,
                    AllowAimCorrection: true,
                    DecalEmissivity: 1.0f,
                    ProjectileGroupSize: 14,

                    DamageType: MyDamageType.Bullet,
                    AmmoType: MyAmmoType.HighSpeed,

                    OnHitParticles: MyParticleEffects.DelegateForCreateBasicHitParticles,
                    OnHitMaterialSpecificParticles: MyParticleEffects.DelegateForCreateHitMaterialParticles
                   );

            m_ammoProperties[(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Sniper_BioChem] =
                new MyAmmoProperties(
                    InitialSpeed: 0,
                    DesiredSpeed: 9000,
                    SpeedVar: 0.0f,
                    MaxTrajectory: 9600,
                    HealthDamage: 50.0f,
                    ShipDamage: 10.0f,
                    EMPDamage: 0.0f,
                    ExplosionRadius: 0,
                    DeviateAngle: MathHelper.ToRadians(0.01f),
                    TrailColor: MyProjectilesConstants.BIOCHEM_PROJECTILE_TRAIL_COLOR,
                    ShotSound: MySoundCuesEnum.WepSniperNormFire3d,
                    IsExplosive: false,
                    AllowAimCorrection: true,
                    DecalEmissivity: 0.5f,
                    ProjectileGroupSize: 1,

                    DamageType: MyDamageType.Bullet,
                    AmmoType: MyAmmoType.Biochem,

                    OnHitParticles: MyParticleEffects.DelegateForCreateBiochemHitParticles,
                    OnHitMaterialSpecificParticles: MyParticleEffects.DelegateForCreateHitMaterialParticles
                   );

            m_ammoProperties[(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Sniper_SAPHEI] =
                new MyAmmoProperties(
                    InitialSpeed: 0,
                    DesiredSpeed: 8000,
                    SpeedVar: 0.0f,
                    MaxTrajectory: 11400,
                    HealthDamage: 0.0f,
                    ShipDamage: 105.0f,
                    EMPDamage: 0.0f,
                    ExplosionRadius: 0,
                    DeviateAngle: MathHelper.ToRadians(0.01f),
                    TrailColor: MyProjectilesConstants.EXPLOSIVE_PROJECTILE_TRAIL_COLOR,
                    ShotSound: MySoundCuesEnum.WepSniperNormFire3d,
                    IsExplosive: true,
                    AllowAimCorrection: true,
                    DecalEmissivity: 0.5f,
                    ProjectileGroupSize: 1,

                    DamageType: MyDamageType.Bullet,
                    AmmoType: MyAmmoType.Explosive,

                    OnHitParticles: MyParticleEffects.DelegateForCreateExplosiveHitParticles,
                    OnHitMaterialSpecificParticles: MyParticleEffects.DelegateForCreateHitMaterialParticles
                   );

            m_ammoProperties[(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Missile_Basic] =
                new MyAmmoProperties(
                    InitialSpeed: 1.0f,
                    DesiredSpeed: 700.0f,
                    SpeedVar: 0.0f,
                    MaxTrajectory: 10000,
                    HealthDamage: 0.0f,
                    ShipDamage: 70.0f,
                    EMPDamage: 0.0f,
                    ExplosionRadius: 30.0f,
                    DeviateAngle: MathHelper.ToRadians(0.01f),
                    TrailColor: Vector3.One,
                    ShotSound: MySoundCuesEnum.WepMissileFly,
                    IsExplosive: true,
                    AllowAimCorrection: true,
                    DecalEmissivity: 0.0f,
                    ProjectileGroupSize: 1,

                    DamageType: MyDamageType.Rocket,
                    AmmoType: MyAmmoType.Basic,

                    OnHitParticles: MyParticleEffects.DelegateForCreateExplosiveHitParticles,
                    OnHitMaterialSpecificParticles: MyParticleEffects.DelegateForCreateHitMaterialParticles
                   );

            m_ammoProperties[(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Guided_Missile_Engine_Detection] =
                new MyAmmoProperties(
                    InitialSpeed: 1.0f,
                    DesiredSpeed: 400.0f,
                    SpeedVar: 0.0f,
                    MaxTrajectory: 15000,
                    HealthDamage: 0.0f,
                    ShipDamage: 20.0f,
                    EMPDamage: 0.0f,
                    ExplosionRadius: 30,
                    DeviateAngle: MathHelper.ToRadians(0.01f),
                    TrailColor: Vector3.One,
                    ShotSound: MySoundCuesEnum.WepMissileFly,
                    IsExplosive: true,
                    AllowAimCorrection: false,
                    DecalEmissivity: 0.0f,
                    ProjectileGroupSize: 1,

                    DamageType: MyDamageType.Rocket,
                    AmmoType: MyAmmoType.Basic,

                    OnHitParticles: MyParticleEffects.DelegateForCreateExplosiveHitParticles,
                    OnHitMaterialSpecificParticles: MyParticleEffects.DelegateForCreateHitMaterialParticles
                   );

                m_ammoProperties[(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Guided_Missile_Visual_Detection] =
                new MyAmmoProperties(
                    InitialSpeed: 1.0f,
                    DesiredSpeed: 400.0f,
                    SpeedVar: 0.0f,
                    MaxTrajectory: 15000,
                    HealthDamage: 0.0f,
                    ShipDamage: 20.0f,
                    EMPDamage: 0.0f,
                    ExplosionRadius: 30,
                    DeviateAngle: MathHelper.ToRadians(0.01f),
                    TrailColor: Vector3.One,
                    ShotSound: MySoundCuesEnum.WepMissileFly,
                    IsExplosive: true,
                    AllowAimCorrection: false,
                    DecalEmissivity: 0.0f,
                    ProjectileGroupSize: 1,

                    DamageType: MyDamageType.Rocket,
                    AmmoType: MyAmmoType.Basic,

                    OnHitParticles: MyParticleEffects.DelegateForCreateExplosiveHitParticles,
                    OnHitMaterialSpecificParticles: MyParticleEffects.DelegateForCreateHitMaterialParticles
                   );


               m_ammoProperties[(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Guided_Missile_Radar_Detection] =
                new MyAmmoProperties(
                    InitialSpeed: 1.0f,
                    DesiredSpeed: 400.0f,
                    SpeedVar: 0.0f,
                    MaxTrajectory: 15000,
                    HealthDamage: 0.0f,
                    ShipDamage: 20.0f,
                    EMPDamage: 0.0f,
                    ExplosionRadius: 30,
                    DeviateAngle: MathHelper.ToRadians(0.01f),
                    TrailColor: Vector3.One,
                    ShotSound: MySoundCuesEnum.WepMissileFly,
                    IsExplosive: true,
                    AllowAimCorrection: false,
                    DecalEmissivity: 0.0f,
                    ProjectileGroupSize: 1,

                    DamageType: MyDamageType.Rocket,
                    AmmoType: MyAmmoType.Basic,

                    OnHitParticles: MyParticleEffects.DelegateForCreateExplosiveHitParticles,
                    OnHitMaterialSpecificParticles: MyParticleEffects.DelegateForCreateHitMaterialParticles
                   );

            m_ammoProperties[(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_Basic] =
                new MyAmmoProperties(
                    InitialSpeed: 0.0f,
                    DesiredSpeed: 1000.0f,
                    SpeedVar: 0.0f,
                    MaxTrajectory: 14000,
                    HealthDamage: 0.0f,
                    ShipDamage: 30.0f,
                    EMPDamage: 0.0f,
                    ExplosionRadius: 22.0f,
                    DeviateAngle: MathHelper.ToRadians(0.01f),
                    TrailColor: Vector3.One,
                    ShotSound: MySoundCuesEnum.WepMissileFly,
                    IsExplosive: true,
                    AllowAimCorrection: true,
                    DecalEmissivity: 0.0f,
                    ProjectileGroupSize: 1,

                    DamageType: MyDamageType.Rocket,
                    AmmoType: MyAmmoType.Basic,

                    OnHitParticles: MyParticleEffects.DelegateForCreateExplosiveHitParticles,
                    OnHitMaterialSpecificParticles: MyParticleEffects.DelegateForCreateHitMaterialParticles
                   );

            m_ammoProperties[(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_High_Speed] =
                new MyAmmoProperties(
                    InitialSpeed: 0.0f,
                    DesiredSpeed: 1500.0f,
                    SpeedVar: 0.0f,
                    MaxTrajectory: 15000,
                    HealthDamage: 0.0f,
                    ShipDamage: 40.0f,
                    EMPDamage: 0.0f,
                    ExplosionRadius: 25.0f,
                    DeviateAngle: MathHelper.ToRadians(0.01f),
                    TrailColor: MyProjectilesConstants.HIGH_SPEED_PROJECTILE_TRAIL_COLOR,
                    ShotSound: MySoundCuesEnum.WepMissileFly,
                    IsExplosive: true,
                    AllowAimCorrection: true,
                    DecalEmissivity: 0.0f,
                    ProjectileGroupSize: 1,

                    DamageType: MyDamageType.Rocket,
                    AmmoType: MyAmmoType.HighSpeed,

                    OnHitParticles: MyParticleEffects.DelegateForCreateExplosiveHitParticles,
                    OnHitMaterialSpecificParticles: MyParticleEffects.DelegateForCreateHitMaterialParticles
                   );

            m_ammoProperties[(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_Armor_Piercing_Incendiary] =
                new MyAmmoProperties(
                    InitialSpeed: 0.0f,
                    DesiredSpeed: 1100.0f,
                    SpeedVar: 0.0f,
                    MaxTrajectory: 15000,
                    HealthDamage: 0.0f,
                    ShipDamage: 50.0f,
                    EMPDamage: 0.0f,
                    ExplosionRadius: 25.0f,
                    DeviateAngle: MathHelper.ToRadians(0.01f),
                    TrailColor: MyProjectilesConstants.PIERCING_PROJECTILE_TRAIL_COLOR,
                    ShotSound: MySoundCuesEnum.WepMissileFly,
                    IsExplosive: true,
                    AllowAimCorrection: true,
                    DecalEmissivity: 0.0f,
                    ProjectileGroupSize: 1,

                    DamageType: MyDamageType.Rocket,
                    AmmoType: MyAmmoType.Piercing,

                    OnHitParticles: MyParticleEffects.DelegateForCreateExplosiveHitParticles,
                    OnHitMaterialSpecificParticles: MyParticleEffects.DelegateForCreateHitMaterialParticles
                   );

            m_ammoProperties[(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_SAPHEI] =
                new MyAmmoProperties(
                    InitialSpeed: 0.0f,
                    DesiredSpeed: 1100.0f,
                    SpeedVar: 0.0f,
                    MaxTrajectory: 15000,
                    HealthDamage: 0.0f,
                    ShipDamage: 80.0f,
                    EMPDamage: 0.0f,
                    ExplosionRadius: 30.0f,
                    DeviateAngle: MathHelper.ToRadians(0.01f),
                    TrailColor: MyProjectilesConstants.EXPLOSIVE_PROJECTILE_TRAIL_COLOR,
                    ShotSound: MySoundCuesEnum.WepMissileFly,
                    IsExplosive: true,
                    AllowAimCorrection: true,
                    DecalEmissivity: 0.0f,
                    ProjectileGroupSize: 1,

                    DamageType: MyDamageType.Rocket,
                    AmmoType: MyAmmoType.Explosive,

                    OnHitParticles: MyParticleEffects.DelegateForCreateExplosiveHitParticles,
                    OnHitMaterialSpecificParticles: MyParticleEffects.DelegateForCreateHitMaterialParticles
                   );

            m_ammoProperties[(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_Proximity_Explosive] =
                new MyAmmoProperties(
                    InitialSpeed: 0.0f,
                    DesiredSpeed: 950.0f,
                    SpeedVar: 0.0f,
                    MaxTrajectory: 12000,
                    HealthDamage: 0.0f,
                    ShipDamage: 50.0f,
                    EMPDamage: 0.0f,
                    ExplosionRadius: 32.0f,
                    DeviateAngle: MathHelper.ToRadians(0.01f),
                    TrailColor: MyProjectilesConstants.EXPLOSIVE_PROJECTILE_TRAIL_COLOR,
                    ShotSound: MySoundCuesEnum.WepMissileFly,
                    IsExplosive: true,
                    AllowAimCorrection: true,
                    DecalEmissivity: 0.0f,
                    ProjectileGroupSize: 1,

                    DamageType: MyDamageType.Rocket,
                    AmmoType: MyAmmoType.Explosive,

                    OnHitParticles: MyParticleEffects.DelegateForCreateExplosiveHitParticles,
                    OnHitMaterialSpecificParticles: MyParticleEffects.DelegateForCreateHitMaterialParticles
                   );

            m_ammoProperties[(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_Tunnel_Buster] =
                new MyAmmoProperties(
                    InitialSpeed: 0.0f,
                    DesiredSpeed: 300.0f,
                    SpeedVar: 0.0f,
                    MaxTrajectory: 10000,
                    HealthDamage: 0.0f,
                    ShipDamage: 60.0f,
                    EMPDamage: 0.0f,
                    ExplosionRadius: 60.0f,
                    DeviateAngle: MathHelper.ToRadians(0.01f),
                    TrailColor: Vector3.One,
                    ShotSound: MySoundCuesEnum.WepMissileFly,
                    IsExplosive: true,
                    AllowAimCorrection: true,
                    DecalEmissivity: 0.0f,
                    ProjectileGroupSize: 1,

                    DamageType: MyDamageType.Rocket,
                    AmmoType: MyAmmoType.Explosive,

                    OnHitParticles: MyParticleEffects.DelegateForCreateExplosiveHitParticles,
                    OnHitMaterialSpecificParticles: MyParticleEffects.DelegateForCreateHitMaterialParticles
                   );

            m_ammoProperties[(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Sphere_Explosive] =
                new MyAmmoProperties(
                    InitialSpeed: 5.0f,
                    DesiredSpeed: 0.0f,
                    SpeedVar: 0.0f,
                    MaxTrajectory: 10000,
                    HealthDamage: 50.0f,
                    ShipDamage: 120.0f,
                    EMPDamage: 0.0f,
                    ExplosionRadius: 30.0f,
                    DeviateAngle: MathHelper.ToRadians(0.01f),
                    TrailColor: Vector3.One,
                    ShotSound: MySoundCuesEnum.WepMissileFly,
                    IsExplosive: true,
                    AllowAimCorrection: false,
                    DecalEmissivity: 0.0f,
                    ProjectileGroupSize: 1,

                    DamageType: MyDamageType.Mine,
                    AmmoType: MyAmmoType.Explosive,

                    OnHitParticles: MyParticleEffects.DelegateForCreateExplosiveHitParticles,
                    OnHitMaterialSpecificParticles: MyParticleEffects.DelegateForCreateHitMaterialParticles
                   );

            m_ammoProperties[(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Directional_Explosive] =
                new MyAmmoProperties(
                    InitialSpeed: 5.0f,
                    DesiredSpeed: 0.0f,
                    SpeedVar: 0.0f,
                    MaxTrajectory: 10000,
                    HealthDamage: 60.0f,
                    ShipDamage: 90.0f,
                    EMPDamage: 0.0f,
                    ExplosionRadius: 35.0f,
                    DeviateAngle: MathHelper.ToRadians(0.01f),
                    TrailColor: Vector3.One,
                    ShotSound: MySoundCuesEnum.WepMissileFly,
                    IsExplosive: true,
                    AllowAimCorrection: false,
                    DecalEmissivity: 0.0f,
                    ProjectileGroupSize: 1,

                    DamageType: MyDamageType.Mine,
                    AmmoType: MyAmmoType.Explosive,

                    OnHitParticles: MyParticleEffects.DelegateForCreateExplosiveHitParticles,
                    OnHitMaterialSpecificParticles: MyParticleEffects.DelegateForCreateHitMaterialParticles
                   );

            m_ammoProperties[(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Time_Bomb] =
                new MyAmmoProperties(
                    InitialSpeed: 5.0f,
                    DesiredSpeed: 0.0f,
                    SpeedVar: 0.0f,
                    MaxTrajectory: 10000,
                    HealthDamage: 0.0f,
                    ShipDamage: 100.0f,
                    EMPDamage: 0.0f,
                    ExplosionRadius: 30.0f,
                    DeviateAngle: MathHelper.ToRadians(0.01f),
                    TrailColor: Vector3.One,
                    ShotSound: MySoundCuesEnum.WepMissileFly,
                    IsExplosive: true,
                    AllowAimCorrection: true,
                    DecalEmissivity: 0.0f,
                    ProjectileGroupSize: 1,

                    DamageType: MyDamageType.Mine,
                    AmmoType: MyAmmoType.Explosive,

                    OnHitParticles: MyParticleEffects.DelegateForCreateExplosiveHitParticles,
                    OnHitMaterialSpecificParticles: MyParticleEffects.DelegateForCreateHitMaterialParticles
                   );

            m_ammoProperties[(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Smoke_Bomb] =
                new MyAmmoProperties(
                    InitialSpeed: 5.0f,
                    DesiredSpeed: 0.0f,
                    SpeedVar: 0.0f,
                    MaxTrajectory: 10000,
                    HealthDamage: 0.0f,
                    ShipDamage: 0.0f,
                    EMPDamage: 0.0f,
                    ExplosionRadius: 40.0f,
                    DeviateAngle: MathHelper.ToRadians(0.01f),
                    TrailColor: Vector3.One,
                    ShotSound: MySoundCuesEnum.WepMissileFly,
                    IsExplosive: true,
                    AllowAimCorrection: true,
                    DecalEmissivity: 0.0f,
                    ProjectileGroupSize: 1,

                    DamageType: MyDamageType.Mine,
                    AmmoType: MyAmmoType.Explosive,

                    OnHitParticles: MyParticleEffects.DelegateForCreateExplosiveHitParticles,
                    OnHitMaterialSpecificParticles: MyParticleEffects.DelegateForCreateHitMaterialParticles
                   );

             m_ammoProperties[(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Remote_Bomb] =
                new MyAmmoProperties(
                    InitialSpeed: 5.0f,
                    DesiredSpeed: 0.0f,
                    SpeedVar: 0.0f,
                    MaxTrajectory: 10000,
                    HealthDamage: 60.0f,
                    ShipDamage: 120.0f,
                    EMPDamage: 0.0f,
                    ExplosionRadius: 40.0f,
                    DeviateAngle: MathHelper.ToRadians(0.01f),
                    TrailColor: Vector3.One,
                    ShotSound: MySoundCuesEnum.WepMissileFly,
                    IsExplosive: true,
                    AllowAimCorrection: false,
                    DecalEmissivity: 0.0f,
                    ProjectileGroupSize: 1,

                    DamageType: MyDamageType.Mine,
                    AmmoType: MyAmmoType.Explosive,

                    OnHitParticles: MyParticleEffects.DelegateForCreateExplosiveHitParticles,
                    OnHitMaterialSpecificParticles: MyParticleEffects.DelegateForCreateHitMaterialParticles
                   );

            m_ammoProperties[(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Asteroid_Killer] =
                new MyAmmoProperties(
                    InitialSpeed: 5.0f,
                    DesiredSpeed: 0.0f,
                    SpeedVar: 0.0f,
                    MaxTrajectory: 10000,
                    HealthDamage: 100.0f,
                    ShipDamage: 150.0f,
                    EMPDamage: 0.0f,
                    ExplosionRadius: 90.0f,
                    DeviateAngle: MathHelper.ToRadians(0.01f),
                    TrailColor: Vector3.One,
                    ShotSound: MySoundCuesEnum.WepMissileFly,
                    IsExplosive: true,
                    AllowAimCorrection: true,
                    DecalEmissivity: 0.0f,
                    ProjectileGroupSize: 1,

                    DamageType: MyDamageType.Mine,
                    AmmoType: MyAmmoType.Explosive,

                    OnHitParticles: MyParticleEffects.DelegateForCreateExplosiveHitParticles,
                    OnHitMaterialSpecificParticles: MyParticleEffects.DelegateForCreateHitMaterialParticles
                   );

            m_ammoProperties[(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Mine_Smart] =
                new MyAmmoProperties(
                    InitialSpeed: 5.0f,
                    DesiredSpeed: 5.0f,
                    SpeedVar: 0.0f,
                    MaxTrajectory: 300,
                    HealthDamage: 0.0f,
                    ShipDamage: 60.0f,
                    EMPDamage: 0.0f,
                    ExplosionRadius: 50.0f,
                    DeviateAngle: MathHelper.ToRadians(0.01f),
                    TrailColor: Vector3.One,
                    ShotSound: MySoundCuesEnum.WepMissileFly,
                    IsExplosive: true,
                    AllowAimCorrection: false,
                    DecalEmissivity: 0.0f,
                    ProjectileGroupSize: 1,

                    DamageType: MyDamageType.Mine,
                    AmmoType: MyAmmoType.Explosive,

                    OnHitParticles: MyParticleEffects.DelegateForCreateExplosiveHitParticles,
                    OnHitMaterialSpecificParticles: MyParticleEffects.DelegateForCreateHitMaterialParticles
                   );
 
            m_ammoProperties[(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Mine_Basic] =
                new MyAmmoProperties(
                    InitialSpeed: 5.0f,
                    DesiredSpeed: 0.0f,
                    SpeedVar: 0.0f,
                    MaxTrajectory: 100,
                    HealthDamage: 0.0f,
                    ShipDamage: 80.0f,
                    EMPDamage: 0.0f,
                    ExplosionRadius: 30.0f,
                    DeviateAngle: MathHelper.ToRadians(0.01f),
                    TrailColor: Vector3.One,
                    ShotSound: MySoundCuesEnum.WepMissileFly,
                    IsExplosive: true,
                    AllowAimCorrection: true,
                    DecalEmissivity: 0.0f,
                    ProjectileGroupSize: 1,

                    DamageType: MyDamageType.Mine,
                    AmmoType: MyAmmoType.Explosive,

                    OnHitParticles: MyParticleEffects.DelegateForCreateExplosiveHitParticles,
                    OnHitMaterialSpecificParticles: MyParticleEffects.DelegateForCreateHitMaterialParticles
                   );
         
            m_ammoProperties[(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Gravity_Bomb] =
                new MyAmmoProperties(
                    InitialSpeed: 5.0f,
                    DesiredSpeed: 0.0f,
                    SpeedVar: 0.0f,
                    MaxTrajectory: 100,
                    HealthDamage: 0.0f,
                    ShipDamage: 40.0f,
                    EMPDamage: 0.0f,
                    ExplosionRadius: 40.0f,
                    DeviateAngle: MathHelper.ToRadians(0.01f),
                    TrailColor: Vector3.One,
                    ShotSound: MySoundCuesEnum.WepMissileFly,
                    IsExplosive: true,
                    AllowAimCorrection: true,
                    DecalEmissivity: 0.0f,
                    ProjectileGroupSize: 1,

                    DamageType: MyDamageType.Mine,
                    AmmoType: MyAmmoType.Explosive,

                    OnHitParticles: MyParticleEffects.DelegateForCreateExplosiveHitParticles,
                    OnHitMaterialSpecificParticles: MyParticleEffects.DelegateForCreateHitMaterialParticles
                   );

            m_ammoProperties[(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Hologram] =
                new MyAmmoProperties(
                    InitialSpeed: 2.0f,
                    DesiredSpeed: 0.0f,
                    SpeedVar: 0.0f,
                    MaxTrajectory: 100,
                    HealthDamage: 0.0f,
                    ShipDamage: 40.0f,
                    EMPDamage: 0.0f,
                    ExplosionRadius: 40.0f,
                    DeviateAngle: MathHelper.ToRadians(0.01f),
                    TrailColor: Vector3.One,
                    ShotSound: MySoundCuesEnum.WepMissileFly,
                    IsExplosive: true,
                    AllowAimCorrection: true,
                    DecalEmissivity: 0.0f,
                    ProjectileGroupSize: 1,

                    DamageType: MyDamageType.Mine,
                    AmmoType: MyAmmoType.Explosive,

                    OnHitParticles: MyParticleEffects.DelegateForCreateExplosiveHitParticles,
                    OnHitMaterialSpecificParticles: MyParticleEffects.DelegateForCreateHitMaterialParticles
                   );

            m_ammoProperties[(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Decoy_Flare] =
                new MyAmmoProperties(
                    InitialSpeed: 100.0f,
                    DesiredSpeed: 0.0f,
                    SpeedVar: 0.0f,
                    MaxTrajectory: 100,
                    HealthDamage: 0.0f,
                    ShipDamage: 40.0f,
                    EMPDamage: 0.0f,
                    ExplosionRadius: 40.0f,
                    DeviateAngle: MathHelper.ToRadians(0.01f),
                    TrailColor: Vector3.One,
                    ShotSound: MySoundCuesEnum.WepMissileFly,
                    IsExplosive: true,
                    AllowAimCorrection: true,
                    DecalEmissivity: 0.0f,
                    ProjectileGroupSize: 1,

                    DamageType: MyDamageType.Mine,
                    AmmoType: MyAmmoType.Explosive,

                    OnHitParticles: MyParticleEffects.DelegateForCreateExplosiveHitParticles,
                    OnHitMaterialSpecificParticles: MyParticleEffects.DelegateForCreateHitMaterialParticles
                   );

            m_ammoProperties[(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Flash_Bomb] =
                new MyAmmoProperties(
                    InitialSpeed: 2.0f,
                    DesiredSpeed: 0.0f,
                    SpeedVar: 0.0f,
                    MaxTrajectory: 100,
                    HealthDamage: 0.0f,
                    ShipDamage: 1.0f,
                    EMPDamage: 0.0f,
                    ExplosionRadius: 1.0f,
                    DeviateAngle: MathHelper.ToRadians(0.01f),
                    TrailColor: Vector3.One,
                    ShotSound: MySoundCuesEnum.WepMissileFly,
                    IsExplosive: true,
                    AllowAimCorrection: true,
                    DecalEmissivity: 0.0f,
                    ProjectileGroupSize: 1,

                    DamageType: MyDamageType.Mine,
                    AmmoType: MyAmmoType.Explosive,

                    OnHitParticles: MyParticleEffects.DelegateForCreateExplosiveHitParticles,
                    OnHitMaterialSpecificParticles: MyParticleEffects.DelegateForCreateHitMaterialParticles
                   );

            m_ammoProperties[(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Illuminating_Shell] =
                new MyAmmoProperties(
                    InitialSpeed: 2.0f,
                    DesiredSpeed: 0.0f,
                    SpeedVar: 0.0f,
                    MaxTrajectory: 100,
                    HealthDamage: 0.0f,
                    ShipDamage: 1.0f,
                    EMPDamage: 0.0f,
                    ExplosionRadius: 0.0f,
                    DeviateAngle: MathHelper.ToRadians(0.01f),
                    TrailColor: Vector3.One,
                    ShotSound: MySoundCuesEnum.WepMissileFly,
                    IsExplosive: true,
                    AllowAimCorrection: true,
                    DecalEmissivity: 0.0f,
                    ProjectileGroupSize: 1,

                    DamageType: MyDamageType.Mine,
                    AmmoType: MyAmmoType.Explosive,

                    OnHitParticles: MyParticleEffects.DelegateForCreateExplosiveHitParticles,
                    OnHitMaterialSpecificParticles: MyParticleEffects.DelegateForCreateHitMaterialParticles
                   );

            m_ammoProperties[(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Remote_Camera] =
                new MyAmmoProperties(
                    InitialSpeed: 2.0f,
                    DesiredSpeed: 0.0f,
                    SpeedVar: 0.0f,
                    MaxTrajectory: 100,
                    HealthDamage: 0.0f,
                    ShipDamage: 0.0f,
                    EMPDamage: 0.0f,
                    ExplosionRadius: 0.0f,
                    DeviateAngle: MathHelper.ToRadians(0.01f),
                    TrailColor: Vector3.One,
                    ShotSound: MySoundCuesEnum.WepMissileFly,
                    IsExplosive: true,
                    AllowAimCorrection: true,
                    DecalEmissivity: 0.0f,
                    ProjectileGroupSize: 1,

                    DamageType: MyDamageType.Mine,
                    AmmoType: MyAmmoType.Explosive,

                    OnHitParticles: MyParticleEffects.DelegateForCreateExplosiveHitParticles,
                    OnHitMaterialSpecificParticles: MyParticleEffects.DelegateForCreateHitMaterialParticles
                   );
   

            m_ammoProperties[(int)MyAmmoPropertiesEnum.Shrapnel] =
               new MyAmmoProperties(
                   InitialSpeed: 0,
                   DesiredSpeed: 800,
                   SpeedVar: 0.2f,
                   MaxTrajectory: 500,
                   HealthDamage: 0.0f,
                   ShipDamage: 5.0f,
                   EMPDamage: 0.0f,
                   ExplosionRadius: 0,
                   DeviateAngle: MathHelper.ToRadians(MathHelper.TwoPi),
                   TrailColor: MyProjectilesConstants.EXPLOSIVE_PROJECTILE_TRAIL_COLOR,
                   ShotSound: MySoundCuesEnum.WepMachineGunNormFire3d,
                   IsExplosive: false,
                   AllowAimCorrection: true,
                   DecalEmissivity: 0.0f,
                   ProjectileGroupSize: 1,

                   DamageType: MyDamageType.Bullet,
                   AmmoType: MyAmmoType.Basic,

                   OnHitParticles: MyParticleEffects.DelegateForCreateBasicHitParticles,
                   OnHitMaterialSpecificParticles: MyParticleEffects.DelegateForCreateHitMaterialParticles
                  );

            m_ammoProperties[(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Mine_BioChem] =
                new MyAmmoProperties(
                    InitialSpeed: 5.0f,
                    DesiredSpeed: 0.0f,
                    SpeedVar: 0.0f,
                    MaxTrajectory: 10000,
                    HealthDamage: 60.0f,
                    ShipDamage: 6.0f,
                    EMPDamage: 0.0f,
                    ExplosionRadius: 20.0f,
                    DeviateAngle: MathHelper.ToRadians(0.01f),
                    TrailColor: Vector3.One,
                    ShotSound: MySoundCuesEnum.WepMissileFly,
                    IsExplosive: true,
                    AllowAimCorrection: false,
                    DecalEmissivity: 0.0f,
                    ProjectileGroupSize: 1,

                    DamageType: MyDamageType.Mine,
                    AmmoType: MyAmmoType.Biochem,

                    OnHitParticles: MyParticleEffects.DelegateForCreateBiochemHitParticles,
                    OnHitMaterialSpecificParticles: MyParticleEffects.DelegateForCreateHitMaterialParticles
                   );

            m_ammoProperties[(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Missile_BioChem] =
                new MyAmmoProperties(
                    InitialSpeed: 1.0f,
                    DesiredSpeed: 500.0f,
                    SpeedVar: 0.0f,
                    MaxTrajectory: 10000,
                    HealthDamage: 40.0f,
                    ShipDamage: 2.0f,
                    EMPDamage: 0.0f,
                    ExplosionRadius: 40.0f,
                    DeviateAngle: MathHelper.ToRadians(0.01f),
                    TrailColor: Vector3.One,
                    ShotSound: MySoundCuesEnum.WepMissileFly,
                    IsExplosive: true,
                    AllowAimCorrection: true,
                    DecalEmissivity: 0.0f,
                    ProjectileGroupSize: 1,

                    DamageType: MyDamageType.Rocket,
                    AmmoType: MyAmmoType.Biochem,

                    OnHitParticles: MyParticleEffects.DelegateForCreateBiochemHitParticles,
                    OnHitMaterialSpecificParticles: MyParticleEffects.DelegateForCreateHitMaterialParticles
                   );

            m_ammoProperties[(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_BioChem] =
                new MyAmmoProperties(
                    InitialSpeed: 0.0f,
                    DesiredSpeed: 2000.0f,
                    SpeedVar: 0.0f,
                    MaxTrajectory: 14000,
                    HealthDamage: 30.0f,
                    ShipDamage: 3.0f,
                    EMPDamage: 0.0f,
                    ExplosionRadius: 15.0f,
                    DeviateAngle: MathHelper.ToRadians(0.01f),
                    TrailColor: Vector3.One,
                    ShotSound: MySoundCuesEnum.WepMissileFly,
                    IsExplosive: true,
                    AllowAimCorrection: true,
                    DecalEmissivity: 0.0f,
                    ProjectileGroupSize: 1,

                    DamageType: MyDamageType.Rocket,
                    AmmoType: MyAmmoType.Biochem,

                    OnHitParticles: MyParticleEffects.DelegateForCreateExplosiveHitParticles,
                    OnHitMaterialSpecificParticles: MyParticleEffects.DelegateForCreateHitMaterialParticles
                   );

            m_ammoProperties[(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_EMP] =
               new MyAmmoProperties(
                   InitialSpeed: 0,
                   DesiredSpeed: 1000,
                   SpeedVar: 0,
                   MaxTrajectory: 1800,
                   HealthDamage: 0.0f,
                   ShipDamage: 0.5f,
                   EMPDamage: 5.0f,
                   ExplosionRadius: 0,
                   DeviateAngle: MathHelper.ToRadians(0.17f),
                   TrailColor: MyProjectilesConstants.EMP_PROJECTILE_TRAIL_COLOR,
                   ShotSound: MySoundCuesEnum.WepAutocanonFire3d,
                   IsExplosive: false,
                   AllowAimCorrection: true,
                   DecalEmissivity: 0.5f,
                   ProjectileGroupSize: 1,

                   DamageType: MyDamageType.Bullet,
                   AmmoType: MyAmmoType.EMP,

                   OnHitParticles: MyParticleEffects.DelegateForCreateAutocannonEMPHitParticles,
                   OnHitMaterialSpecificParticles: MyParticleEffects.DelegateForCreateAutocannonHitMaterialParticles
                  );

            m_ammoProperties[(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Sniper_EMP] =
                new MyAmmoProperties(
                    InitialSpeed: 0,
                    DesiredSpeed: 8000,
                    SpeedVar: 0.0f,
                    MaxTrajectory: 9600,
                    HealthDamage: 0.0f,
                    ShipDamage: 15.0f,
                    EMPDamage: 45.0f,
                    ExplosionRadius: 0,
                    DeviateAngle: MathHelper.ToRadians(0.01f),
                    TrailColor: MyProjectilesConstants.EMP_PROJECTILE_TRAIL_COLOR,
                    ShotSound: MySoundCuesEnum.WepSniperNormFire3d,
                    IsExplosive: false,
                    AllowAimCorrection: true,
                    DecalEmissivity: 0.5f,
                    ProjectileGroupSize: 1,

                    DamageType: MyDamageType.Bullet,
                    AmmoType: MyAmmoType.Biochem,

                    OnHitParticles: MyParticleEffects.DelegateForCreateEMPHitParticles,
                    OnHitMaterialSpecificParticles: MyParticleEffects.DelegateForCreateHitMaterialParticles
                   );

            m_ammoProperties[(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Machine_Gun_EMP] =
               new MyAmmoProperties(
                   InitialSpeed: 0,
                   DesiredSpeed: 900,
                   SpeedVar: 0,
                   MaxTrajectory: 1900,
                   HealthDamage: 0.0f,
                   ShipDamage: 0.5f,
                   EMPDamage: 5.0f,
                   ExplosionRadius: 0,
                   DeviateAngle: MathHelper.ToRadians(0.17f),
                   TrailColor: MyProjectilesConstants.EMP_PROJECTILE_TRAIL_COLOR,
                   ShotSound: MySoundCuesEnum.WepMachineGunNormFire3d,
                   IsExplosive: false,
                   AllowAimCorrection: true,
                   DecalEmissivity: 0.5f,
                   ProjectileGroupSize: 1,

                   DamageType: MyDamageType.Bullet,
                   AmmoType: MyAmmoType.EMP,

                   OnHitParticles: MyParticleEffects.DelegateForCreateEMPHitParticles,
                   OnHitMaterialSpecificParticles: MyParticleEffects.DelegateForCreateHitMaterialParticles
                  );

            m_ammoProperties[(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Missile_EMP] =
                new MyAmmoProperties(
                    InitialSpeed: 1.0f,
                    DesiredSpeed: 500.0f,
                    SpeedVar: 0.0f,
                    MaxTrajectory: 10000,
                    HealthDamage: 0.0f,
                    ShipDamage: 2.0f,
                    EMPDamage: 20.0f,
                    ExplosionRadius: 40.0f,
                    DeviateAngle: MathHelper.ToRadians(0.01f),
                    TrailColor: Vector3.One,
                    ShotSound: MySoundCuesEnum.WepMissileFly,
                    IsExplosive: true,
                    AllowAimCorrection: true,
                    DecalEmissivity: 0.0f,
                    ProjectileGroupSize: 1,

                    DamageType: MyDamageType.Rocket,
                    AmmoType: MyAmmoType.EMP,

                    OnHitParticles: MyParticleEffects.DelegateForCreateEMPHitParticles,
                    OnHitMaterialSpecificParticles: MyParticleEffects.DelegateForCreateHitMaterialParticles
                   );

            m_ammoProperties[(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_EMP] =
                new MyAmmoProperties(
                    InitialSpeed: 0.0f,
                    DesiredSpeed: 2000.0f,
                    SpeedVar: 0.0f,
                    MaxTrajectory: 14000,
                    HealthDamage: 0.0f,
                    ShipDamage: 3.0f,
                    EMPDamage: 30.0f,
                    ExplosionRadius: 15.0f,
                    DeviateAngle: MathHelper.ToRadians(0.01f),
                    TrailColor: Vector3.One,
                    ShotSound: MySoundCuesEnum.WepMissileFly,
                    IsExplosive: true,
                    AllowAimCorrection: true,
                    DecalEmissivity: 0.0f,
                    ProjectileGroupSize: 1,

                    DamageType: MyDamageType.Rocket,
                    AmmoType: MyAmmoType.EMP,

                    OnHitParticles: MyParticleEffects.DelegateForCreateEMPHitParticles,
                    OnHitMaterialSpecificParticles: MyParticleEffects.DelegateForCreateHitMaterialParticles
                   );

            m_ammoProperties[(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_EMP_Bomb] =
                new MyAmmoProperties(
                    InitialSpeed: 5.0f,
                    DesiredSpeed: 0.0f,
                    SpeedVar: 0.0f,
                    MaxTrajectory: 10000,
                    HealthDamage: 0.0f,
                    ShipDamage: 20.0f,
                    EMPDamage: 150.0f,
                    ExplosionRadius: 100.0f,
                    DeviateAngle: MathHelper.ToRadians(0.01f),
                    TrailColor: Vector3.One,
                    ShotSound: MySoundCuesEnum.WepMissileFly,
                    IsExplosive: true,
                    AllowAimCorrection: false,
                    DecalEmissivity: 0.0f,
                    ProjectileGroupSize: 1,

                    DamageType: MyDamageType.Mine,
                    AmmoType: MyAmmoType.EMP,

                    OnHitParticles: MyParticleEffects.DelegateForCreateEMPHitParticles,
                    OnHitMaterialSpecificParticles: MyParticleEffects.DelegateForCreateHitMaterialParticles
                   );

            float minimalExplosionRadius = (float)Math.Sqrt(3) * MyVoxelConstants.VOXEL_SIZE_IN_METRES * 0.5f;

            foreach (MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum ammo in Enum.GetValues(typeof(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum)))
            {
                System.Diagnostics.Debug.Assert(MyAmmoConstants.GetAmmoProperties(ammo) != null);
                if (MyAmmoConstants.GetAmmoProperties(ammo).ExplosionRadius > 1)
                    System.Diagnostics.Debug.Assert(MyAmmoConstants.GetAmmoProperties(ammo).ExplosionRadius > minimalExplosionRadius);
            }

            foreach (MyAmmoPropertiesEnum ammo in Enum.GetValues(typeof(MyAmmoPropertiesEnum)))
            {
                System.Diagnostics.Debug.Assert(MyAmmoConstants.GetAmmoProperties(ammo) != null);
                if (MyAmmoConstants.GetAmmoProperties(ammo).ExplosionRadius > 1)
                    System.Diagnostics.Debug.Assert(MyAmmoConstants.GetAmmoProperties(ammo).ExplosionRadius > minimalExplosionRadius);
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        public static MyAmmoProperties GetAmmoProperties(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum ammoType)
        {
            return m_ammoProperties[(int)ammoType];
        }

        public static MyAmmoProperties GetAmmoProperties(MyAmmoPropertiesEnum ammoType)
        {
            return m_ammoProperties[(int)ammoType];
        }
    }
}
