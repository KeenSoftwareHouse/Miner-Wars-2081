using System;
using MinerWarsMath;

using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Audio;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.Prefabs;
using MinerWars.AppCode.Game.TransparentGeometry;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Utils;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.Managers.Session;
using System.Diagnostics;
using MinerWars.AppCode.Game.Entities.Prefabs;
using MinerWars.AppCode.Game.World;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using System.Collections.Generic;
using MinerWars.AppCode.Game.SolarSystem;
using MinerWars.AppCode.Game.TransparentGeometry.Particles;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.AppCode.Game.Explosions;
using MinerWars.AppCode.Physics;
using MinerWars.AppCode.Game.Models;

namespace MinerWars.AppCode.Game.Renders
{

    static class MyMeteorWind
    {
        public static bool IsActive;
        static MySoundCue? m_burningCue;

        private const int minSize = 50;
        private const int maxSize = 500;

        static MyMeteorWind()
        {
            //Render.MyRender.RegisterRenderModule("Meteor wind", Draw, Render.MyRenderStage.PrepareForDraw);
        }

        public static void LoadInDraw()
        {
            MyMwcLog.WriteLine("MyMeteorWind.LoadInDraw() - START");
            MyMwcLog.IncreaseIndent();
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyMeteorWind::LoadInDraw");

            List<MyMwcObjectBuilder_StaticAsteroid_TypesEnum> asteroids = new List<MyMwcObjectBuilder_StaticAsteroid_TypesEnum>();

            List<int> sizes = new List<int>();
            foreach (int size in MyMwcObjectBuilder_StaticAsteroid.AsteroidSizes)
	        {
                if (size >= minSize && size <= maxSize)
                {
                    sizes.Add(size);
                }
	        }
            foreach (int size in sizes)
            {
                MyMwcObjectBuilder_Meteor.GetAsteroids(size, MyStaticAsteroidTypeSetEnum.A, asteroids);
            }

            foreach (var asteroid in asteroids)
	        {
                MyStaticAsteroid.MyStaticAsteroidModels models = MyMeteor.GetModelsFromType(asteroid);
                PreloadModels(models);
	        }
            

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyMeteorWind.LoadInDraw() - END");
        }

        private static void PreloadModels(MyStaticAsteroid.MyStaticAsteroidModels models)
        {
            PreloadModel(models.LOD0);
            PreloadModel(models.LOD1);
            PreloadModel(models.LOD2);
        }

        private static void PreloadModel(MyModelsEnum? myModelsEnum)
        {
            if (myModelsEnum == null)
                return;

            MyModel model = MyModels.GetModelOnlyData(myModelsEnum.Value);
            model.LoadInDraw();
            model.PreloadTextures(Managers.LoadingMode.Immediate);
        }

        public static List<MyMwcVoxelMaterialsEnum> m_fireMeteorMaterials = new List<MyMwcVoxelMaterialsEnum>
        {
            //MyMwcVoxelMaterialsEnum.Indestructible_01,
            //MyMwcVoxelMaterialsEnum.Indestructible_02,
            //MyMwcVoxelMaterialsEnum.Indestructible_03,
            //MyMwcVoxelMaterialsEnum.Indestructible_04,
                        MyMwcVoxelMaterialsEnum.Lava_01,
        };


        public static void UnloadContent()
        {
            MyMwcLog.WriteLine("MyMeteorWind.UnloadContent - START");
            MyMwcLog.IncreaseIndent();

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyMeteorWind.UnloadContent - END");
            // IsActive = false;
        }

        //  This method will start sun wind. Or if there is one coming, this will reset it so it will start again.
        public static void Start()
        {
            IsActive = true;
            
            //m_burningCue = MyAudio.AddCue3D(MySoundCuesEnum.SfxSolarWind, m_initialSunWindPosition, m_directionFromSunNormalized, Vector3.Up, Vector3.Zero);

            const int meteorsCount = 5;
            //const int frontDistance = 1000;
            //const int sideDistance = 3000;

            const int minSpeed = 1000;
            const int maxSpeed = 4000;

            //Vector3 sphereCenter = MyCamera.Position + MyCamera.ForwardVector * frontDistance - MyCamera.LeftVector * sideDistance;
            Vector3 sphereCenter = MyCamera.Position + MyGuiScreenGamePlay.Static.GetDirectionToSunNormalized() * 10000;
            //Vector3 windForwardDirection = MyCamera.LeftVector ;
            
            int i = 0;
            while (i < meteorsCount)
            {
                //float distance = MyMwcUtils.GetRandomFloat(0, sphereRadius);
                //Vector3 position = sphereCenter + MyMwcUtils.GetRandomVector3Normalized() * new Vector3(distance, distance, distance);
                
                //Vector3 meteorDirection = (windForwardDirection + (MyMwcUtils.GetRandomVector3Normalized() * 0.05f)) * MyMwcUtils.GetRandomInt(minSpeed, maxSpeed);
                //Vector3 meteorDirection = -MyGuiScreenGamePlay.Static.GetDirectionToSunNormalized();// MyMwcUtils.GetRandomVector3HemisphereNormalized(-MyGuiScreenGamePlay.Static.GetDirectionToSunNormalized());
                Vector3 meteorDirection = MyMwcUtils.GetRandomVector3HemisphereNormalized(-MyGuiScreenGamePlay.Static.GetDirectionToSunNormalized());
                Vector3 position = sphereCenter + meteorDirection * MyMwcUtils.GetRandomInt(100, 5000);

                //float normalizedDistance = distance / sphereRadius;

                float size = MyMwcUtils.GetRandomInt(minSize, maxSize);


                MyLine line = new MyLine(position, position + meteorDirection * 100);
                MyIntersectionResultLineBoundingSphere? result = MyEntities.GetIntersectionWithLineAndBoundingSphere(ref line, null, null, 1, null, true);
                if (result != null)
                {   //Do not create meteors colliding with base
                    if (!(result.Value.PhysObject is MyMeteor))
                       continue;
                }

                Matrix worldMatrix = Matrix.CreateFromAxisAngle(MyMwcUtils.GetRandomVector3Normalized(), MyMwcUtils.GetRandomFloat(0, MathHelper.Pi));
                worldMatrix.Translation = position;

                MyMeteor meteor = MyMeteor.GenerateMeteor(size, worldMatrix, position, m_fireMeteorMaterials[MyMwcUtils.GetRandomInt(0, m_fireMeteorMaterials.Count)]);


                float speed = MyMwcUtils.GetRandomInt(minSpeed, maxSpeed);


                meteor.Start(meteorDirection * speed, MyMwcUtils.GetRandomFloat(0, 1) > 0.92f ? 101 : 100);

                i++;
            }
        }



        public static void Update()
        {
            //  Update only if sun wind is active
            if (IsActive == false) return;
        }



    }
}
