using Microsoft.Xna.Framework;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.Renders;
using MinerWars.AppCode.Game.HUD;

//  This class holds and sets global constants for effects.

namespace MinerWars.AppCode.Game.Utils
{
    static class MyEffectValuesManager
    {
        /*
         * 
         * 
         * TODO_NEW_RENDER 
         * 
         * 
        static float m_reflectorRange;
        static Vector4 m_fogColor;
        static Vector4 m_fogColorForBackground;
        static float m_fogDistanceFar;
        static float m_fogDistanceNear;
        static Vector3 m_reflectorPosition;
        static Vector3 m_reflectorPositionZero;
        static float m_nearLightRange;
        static Vector4 m_nearLightColor;
        static Vector4 m_sunColor;
        static float m_reflectorConeMaxAngleCos;
        static Vector3 m_reflectorDirection;


        //  Update specified effect with specular parameters
        public static void UpdateSpecularParameters(MyEffect effect, float specularShininess, float specularPower)
        {
            effect.Shininess.SetValue(specularShininess);
            effect.SpecularPower.SetValue(specularPower);
        }

        //  Update specified effect with common parameters but also change sun direction - needed for ship customization screen
        public static void UpdateEffect(MyEffect effect)
        {
            effect.EyePosition.SetValue(MyCamera.PositionZero);
            effect.ReflectorPosition.SetValue(m_reflectorPositionZero);

            effect.ViewProjectionMatrix.SetValue(MyCamera.ViewProjectionMatrixAtZero);

            if (effect.FogColor != null)
            {
                effect.FogColor.SetValue(m_fogColor);
                effect.FogDistanceFar.SetValue(m_fogDistanceFar);
                effect.FogDistanceNear.SetValue(m_fogDistanceNear);
            }


            if (effect.FogColorForBackground != null)
            {
                effect.FogColorForBackground.SetValue(m_fogColorForBackground);
            }

            if (effect.ReflectorRange != null)
            {
                effect.ReflectorRange.SetValue(m_reflectorRange);
                effect.NearLightRange.SetValue(m_nearLightRange);

                if (MyGuiScreenGameBase.Static.CameraAttachedTo == MyCameraAttachedToEnum.PlayerMinerShip)
                {
                    effect.ReflectorColor.SetValue(MyGuiScreenGameBase.Static.PlayerShip.Config.ReflectorLight.Level *
                        MyGuiScreenGameBase.Static.PlayerShip.GetReflectorProperties().CurrentReflectorLightColor);
                    effect.NearLightColor.SetValue(m_nearLightColor);
                }
                else
                {
                    effect.ReflectorColor.SetValue(MySpectator.ReflectorOn == true ?
                        MyGuiScreenGameBase.Static.PlayerShip.GetReflectorProperties().CurrentReflectorLightColor : new Vector4(0, 0, 0, 0));
                    effect.NearLightColor.SetValue(MySpectator.ReflectorOn == true ? m_nearLightColor : new Vector4(0, 0, 0, 0));
                }

                effect.ReflectorDirection.SetValue(m_reflectorDirection);
                effect.ReflectorConeMaxAngleCos.SetValue(m_reflectorConeMaxAngleCos);
            }

            if (effect.SunColor != null)
            {
                effect.SunColor.SetValue(m_sunColor);
                effect.DirectionToSun.SetValue(MyGuiScreenGameBase.Static.GetSectorGroup().GetDirectionToSunNormalized());
            }
        }

        //  Cal this to update members of this class before every Draw
        //  If forwardCamera = true, then this update was called for forward camera. If false, then for backward/mirror camera.
        public static void Update()
        {
            if (MySunWind.IsActive == true)
            {
                //  Sun color altered by sun
                m_sunColor = MySunWind.GetSunColor();
            }
            else
            {
                //  Default sector sun color
                m_sunColor = MyGuiScreenGameBase.Static.GetSectorGroup().GetSunColor();
            }

            //  Fog for all objects and then fog for background cube
            Vector4 sectorDustColor = MyGuiScreenGameBase.Static.SectorDustColor;
            m_fogColorForBackground = new Vector4(sectorDustColor.X * sectorDustColor.W, sectorDustColor.Y * sectorDustColor.W, sectorDustColor.Z * sectorDustColor.W, 1);
            m_fogColor = new Vector4(m_fogColorForBackground.X * 0.2f, m_fogColorForBackground.Y * 0.2f, m_fogColorForBackground.Z * 0.2f, m_fogColorForBackground.W);

            //  Sun color contains a bit of fog/dust too
            m_sunColor.X = MathHelper.Lerp(m_sunColor.X, m_fogColor.X, 0.1f);
            m_sunColor.Y = MathHelper.Lerp(m_sunColor.Y, m_fogColor.Y, 0.1f);
            m_sunColor.Z = MathHelper.Lerp(m_sunColor.Z, m_fogColor.Z, 0.1f);
            m_sunColor.W = MathHelper.Lerp(m_sunColor.W, m_fogColor.W, 0.1f);

            //  Fog is used to hide switching LOD on voxels, but ofcourse it has impact on models too
            m_fogDistanceFar = 2000;
            m_fogDistanceNear = 1500;

            m_reflectorPositionZero = MyCamera.PositionZero + MyConstants.REFLECTOR_POSITION_DELTA.Y * MyCamera.UpVector + MyConstants.REFLECTOR_POSITION_DELTA.Z * MyCamera.ForwardVector;
            m_reflectorPosition = m_reflectorPositionZero + MyCamera.Position;
            
            m_reflectorDirection = MyCamera.ForwardVector;

            //  Every camera has reflector + near light. Reflector is spot light, near light is point light. They add together.
            m_nearLightColor = MyGuiScreenGameBase.Static.PlayerShip.Light.Color;

            //  Every camera has reflector + near light. Reflector is spot light, near light is point light. They add together.
            //  This range is used formula where we use normalized distance. Because that one is calculated from FogEnd-FogStart, we need to set here value that is fraction of that.
            m_nearLightRange = MyMinerShipConstants.MINER_SHIP_NEAR_LIGHT_RANGE;

            //  This is max angle the reflector can light. It's in inverse cosinus (not radians, not degrees)!!!
            //ReflectorConeMaxAngleCos = 1.0f - (float)Math.Cos(3.14f / 5.0f);
            if (MyCamera.ActualCameraDirection == MyCameraDirection.FORWARD)
            {
                //  Reflector for forward camera
                m_reflectorRange = MyGuiScreenGameBase.Static.PlayerShip.GetReflectorProperties().CurrentReflectorRangeForward;
                m_reflectorConeMaxAngleCos = MyGuiScreenGameBase.Static.PlayerShip.GetReflectorProperties().CurrentReflectorConeAngleForward;
            }
            else
            {
                //  Reflector for backward camera
                m_reflectorRange = MyGuiScreenGameBase.Static.PlayerShip.GetReflectorProperties().CurrentReflectorRangeBackward;
                m_reflectorConeMaxAngleCos = MyGuiScreenGameBase.Static.PlayerShip.GetReflectorProperties().CurrentReflectorConeAngleBackward;
            }
        }

        public static Vector4 GetSunColor()
        {
            return m_sunColor;
        }

        public static Vector4 GetNearLightColor()
        {
            return m_nearLightColor;
        }

        public static float GetReflectorConeMaxAngleCos()
        {
            return m_reflectorConeMaxAngleCos;
        }

        public static float GetReflectorRange()
        {
            return m_reflectorRange;
        }*/
    }
}
