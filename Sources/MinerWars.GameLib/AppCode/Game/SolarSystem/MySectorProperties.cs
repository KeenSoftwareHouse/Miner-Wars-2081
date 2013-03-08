using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.TransparentGeometry;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.CommonLIB.AppCode.Networking;

namespace MinerWars.AppCode.Game.SolarSystem
{
    class MySunProperties
    {
        // Sun & ambient
        public float SunIntensity;
        public Vector3 SunDiffuse;
        public Vector3 SunSpecular;

        public float BackSunIntensity;
        public Vector3 BackSunDiffuse;

        public Vector3 AmbientColor;
        public float AmbientMultiplier;
        public float EnvironmentAmbientIntensity;
        public float SunSizeMultiplier = 1;

        public Vector3 BackgroundColor = Vector3.One;
        public float SunRadiationDamagePerSecond = 0;


        /// <param name="interpolator">0 - use this object, 1 - use other object</param>
        public MySunProperties InterpolateWith(MySunProperties otherProperties, float interpolator)
        {
            var result = new MySunProperties();

            result.SunDiffuse = Vector3.Lerp(SunDiffuse, otherProperties.SunDiffuse, interpolator);
            result.SunIntensity = MathHelper.Lerp(SunIntensity, otherProperties.SunIntensity, interpolator);
            result.SunSpecular = Vector3.Lerp(SunSpecular, otherProperties.SunSpecular, interpolator);

            result.BackSunIntensity = MathHelper.Lerp(BackSunIntensity, otherProperties.BackSunIntensity, interpolator);
            result.BackSunDiffuse = Vector3.Lerp(BackSunDiffuse, otherProperties.BackSunDiffuse, interpolator);

            result.AmbientColor = Vector3.Lerp(AmbientColor, otherProperties.AmbientColor, interpolator);
            result.AmbientMultiplier = MathHelper.Lerp(AmbientMultiplier, otherProperties.AmbientMultiplier, interpolator);
            result.EnvironmentAmbientIntensity = MathHelper.Lerp(EnvironmentAmbientIntensity, otherProperties.EnvironmentAmbientIntensity, interpolator);
            result.SunSizeMultiplier = MathHelper.Lerp(SunSizeMultiplier, otherProperties.SunSizeMultiplier, interpolator);

            result.BackgroundColor = Vector3.Lerp(BackgroundColor, otherProperties.BackgroundColor, interpolator);
            result.SunRadiationDamagePerSecond = MathHelper.Lerp(SunRadiationDamagePerSecond, otherProperties.SunRadiationDamagePerSecond, interpolator);
 
            return result;
        }
    }

    class MyFogProperties
    {
        // Fog
        public float FogNear;
        public float FogFar;
        public float FogMultiplier;
        public float FogBacklightMultiplier;
        public Vector3 FogColor;

        /// <param name="interpolator">0 - use this object, 1 - use other object</param>
        public MyFogProperties InterpolateWith(MyFogProperties otherProperties, float interpolator)
        {
            var result = new MyFogProperties();
            result.FogNear = MathHelper.Lerp(FogNear, otherProperties.FogNear, interpolator);
            result.FogFar = MathHelper.Lerp(FogFar, otherProperties.FogFar, interpolator);
            result.FogMultiplier = MathHelper.Lerp(FogMultiplier, otherProperties.FogMultiplier, interpolator);
            result.FogBacklightMultiplier = MathHelper.Lerp(FogBacklightMultiplier, otherProperties.FogBacklightMultiplier, interpolator);
            result.FogColor = Vector3.Lerp(FogColor, otherProperties.FogColor, interpolator);
            return result;
        }
    }

    class MyImpostorProperties
    {
        public bool Enabled = true;
        public MinerWars.AppCode.Game.BackgroundCube.MyVoxelMapImpostors.MyImpostorType ImpostorType;
        public MyTransparentMaterialEnum? Material;
        public int ImpostorsCount;
        public float MinDistance;
        public float MaxDistance;
        public float MinRadius;
        public float MaxRadius;
        public Vector4 AnimationSpeed;
        public Vector3 Color;
        public float Intensity;
        public float Contrast;

        // Gets or sets both MinRadius and MaxRadius
        // Always returns MaxRadius
        public float Radius
        {
            get
            {
                return MaxRadius;
            }
            set
            {
                MinRadius = value;
                MaxRadius = value;
            }
        }

        public float Anim1 { get { return AnimationSpeed.X; } set { AnimationSpeed.X = value; } }
        public float Anim2 { get { return AnimationSpeed.Y; } set { AnimationSpeed.Y = value; } }
        public float Anim3 { get { return AnimationSpeed.Z; } set { AnimationSpeed.Z = value; } }
        public float Anim4 { get { return AnimationSpeed.W; } set { AnimationSpeed.W = value; } }
    }

    class MyDebrisProperties
    {
        // Debris
        public bool Enabled = true;
        public float DistanceBetween = 1;
        public float CountInDirectionHalf = 0;
        public float MaxDistance = 1;
        public float FullScaleDistance = 1;
        public Array DebrisEnumValues = new MyMwcObjectBuilder_SmallDebris_TypesEnum[]
            {
                MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris1,
                MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris2,
                MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris3,
                MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris4,
                MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris5,
                MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris6,
                MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris7,
                MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris8,
                MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris9,
                MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris10,
                MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris11,
                MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris12,
                MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris13,
                MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris14,
                MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris15,
                MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris16,
                MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris17,
                MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris18,
                MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris19,
                MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris20,
                MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris21,
                MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris22,
                MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris23,
                MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris24,
                MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris25,
                MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris26,
                MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris27,
                MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris28,
                MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris29,
                MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris30,
                MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris31,
            };

        public Array DebrisVoxelMaterials = new MyMwcVoxelMaterialsEnum[] { MyMwcVoxelMaterialsEnum.Indestructible_01 };

        /// <param name="interpolator">0 - use this object, 1 - use other object</param>
        public MyDebrisProperties InterpolateWith(MyDebrisProperties otherProperties, float interpolator)
        {
            var result = new MyDebrisProperties();
            result.DistanceBetween = MathHelper.Lerp(DistanceBetween, otherProperties.DistanceBetween, interpolator);
            result.CountInDirectionHalf = MathHelper.Lerp(CountInDirectionHalf, otherProperties.CountInDirectionHalf, interpolator);
            result.MaxDistance = MathHelper.Lerp(MaxDistance, otherProperties.MaxDistance, interpolator);
            result.FullScaleDistance = MathHelper.Lerp(FullScaleDistance, otherProperties.FullScaleDistance, interpolator);
            result.DebrisEnumValues = DebrisEnumValues;
            result.DebrisVoxelMaterials = DebrisVoxelMaterials;
            return result;
        }
    }

    class MyParticleDustProperties
    {
        public bool Enabled = false;
        public float DustBillboardRadius = 3;
        public float DustFieldCountInDirectionHalf = 5;
        public float DistanceBetween = 180;
        public float AnimSpeed = 0.004f;
        public Color Color = Color.White;
        public MyTransparentMaterialEnum Texture = MyTransparentMaterialEnum.Dust;

        /// <param name="interpolator">0 - use this object, 1 - use other object</param>
        public MyParticleDustProperties InterpolateWith(MyParticleDustProperties otherProperties, float interpolator)
        {
            var result = new MyParticleDustProperties();
            result.DustFieldCountInDirectionHalf = MathHelper.Lerp(DustFieldCountInDirectionHalf, otherProperties.DustFieldCountInDirectionHalf, interpolator);
            result.DistanceBetween = MathHelper.Lerp(DistanceBetween, otherProperties.DistanceBetween, interpolator);
            result.AnimSpeed = MathHelper.Lerp(AnimSpeed, otherProperties.AnimSpeed, interpolator);
            result.Color = Color.Lerp(Color, otherProperties.Color, interpolator);
            result.Enabled = MathHelper.Lerp(Enabled ? 1 : 0, otherProperties.Enabled ? 1 : 0, interpolator) > 0.5f;
            result.DustBillboardRadius = interpolator <= 0.5f ? DustBillboardRadius : otherProperties.DustBillboardRadius;
            result.Texture = interpolator <= 0.5f ? Texture : otherProperties.Texture;
            return result;
        }
    }

    class MyGodRaysProperties
    {
        public bool Enabled = false;
        public float Density = 0.34f;
        public float Weight = 1.27f;
        public float Decay = 0.97f;
        public float Exposition = 0.077f;

        /// <param name="interpolator">0 - use this object, 1 - use other object</param>
        public MyGodRaysProperties InterpolateWith(MyGodRaysProperties otherProperties, float interpolator)
        {
            var result = new MyGodRaysProperties();
            result.Density = MathHelper.Lerp(Density, otherProperties.Density, interpolator);
            result.Weight = MathHelper.Lerp(Weight, otherProperties.Weight, interpolator);
            result.Decay = MathHelper.Lerp(Decay, otherProperties.Decay, interpolator);
            result.Exposition = MathHelper.Lerp(Exposition, otherProperties.Exposition, interpolator);
            result.Enabled = MathHelper.Lerp(Enabled ? 1 : 0, otherProperties.Enabled ? 1 : 0, interpolator) > 0.5f;
            return result;
        }
    }
}
