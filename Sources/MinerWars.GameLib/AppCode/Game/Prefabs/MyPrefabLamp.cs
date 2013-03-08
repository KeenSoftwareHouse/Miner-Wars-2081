using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Lights;
using MinerWars.AppCode.Game.TransparentGeometry;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.Utils;

namespace MinerWars.AppCode.Game.Prefabs
{
    class MyPrefabLamp
    {
        const float BLIC_DURATON_IN_MILISECONDS = 30.0f;

        public Vector3 Position;
        public readonly float RadiusMin;
        public readonly float RadiusMax;
        public readonly int TimerForBlic;
        public readonly MyLight Light;

        public MyPrefabLamp(Vector3 position, float radiusMin, float radiusMax, int timerForBlic)
        {
            Position = position;
            RadiusMin = radiusMin;
            RadiusMax = radiusMax;
            TimerForBlic = timerForBlic;
            Light = MyLights.AddLight();
            Light.Start(MyLight.LightTypeEnum.PointLight, position, Vector4.One, 1, radiusMin);
            Light.Intensity = 1;
            Light.LightOn = true;
        }

        public void Draw()
        {
            Vector3 dir = MyMwcUtils.Normalize(MyCamera.Position - Position);

            float timeBlic = MyMinerGame.TotalGamePlayTimeInMilliseconds % TimerForBlic;
            if (timeBlic > BLIC_DURATON_IN_MILISECONDS) timeBlic = TimerForBlic - timeBlic;
            timeBlic = MathHelper.Clamp(1 - timeBlic / BLIC_DURATON_IN_MILISECONDS, 0, 1);

            float radius = MathHelper.Lerp(RadiusMin, RadiusMax, timeBlic);

            MyTransparentGeometry.AddPointBillboard(MyTransparentMaterialEnum.ReflectorGlareAlphaBlended, Vector4.One, Position + dir, radius, 0);
            Light.Range = radius * 4;
        }

        public void Close()
        {
            MyLights.RemoveLight(Light);
        }

        internal void MoveTo(Vector3 position)
        {
            Position = position;
            Light.SetPosition(position);
        }
    }
}
