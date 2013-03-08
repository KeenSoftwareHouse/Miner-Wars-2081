using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.Entities;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.AppCode.App;

namespace MinerWars.AppCode.Game.Renders
{
    static class MyIceComet
    {
        public static bool IsActive;
        private static int m_maxTime = (int)TimeSpan.FromSeconds(60).TotalMilliseconds;
        private static int m_startTime;

        private static List<MyMwcVoxelMaterialsEnum> m_iceMeteorMaterials = new List<MyMwcVoxelMaterialsEnum>
        {
            MyMwcVoxelMaterialsEnum.Snow_01,
        };

        private const int m_frontDistance = 1000;
        private const int m_sideDistance = 5000;

        private const int m_sizeMin = 800;
        private const int m_sizeMax = 900;


        internal static void Start()
        {
            IsActive = true;
            m_startTime = MyMinerGame.TotalGamePlayTimeInMilliseconds;

            const int meteorsCount = 3;

            const int minSpeed = 1200;
            const int maxSpeed = 1400;

            Vector3 sphereCenter = MyCamera.Position + MyCamera.ForwardVector * m_frontDistance - MyCamera.LeftVector * m_sideDistance;
            Vector3 windForwardDirection = MyCamera.LeftVector;

            for (int i = 0; i < meteorsCount; i++)
            {

                Vector3 meteorDirection = (windForwardDirection + (MyMwcUtils.GetRandomVector3Normalized() * 0.001f)) * MyMwcUtils.GetRandomInt(minSpeed, maxSpeed);
                float distance = MyMwcUtils.GetRandomFloat(1000, 1500);

                CreateComet(sphereCenter + MyMwcUtils.GetRandomVector3Normalized() * new Vector3(distance, distance, distance), meteorDirection);
            }
        }

        //private static void Clear()
        //{
        //    for (int i = 0; i < m_comets.Count; i++)
        //    {
        //        if (m_comets[i] != null)
        //        {
        //            if (m_comets[i].Physics != null)
        //            {
        //                if (m_comets[i].Physics.Enabled) // Editor can disable physics
        //                {
        //                    m_comets[i].Physics.Enabled = false;
        //                }
        //                m_comets[i].Physics.RemoveAllElements();
        //            }
        //            m_comets[i].MarkForClose();
        //        }
        //    }

        //    m_comets.Clear();
        //}

        public static void Update()
        {
            if (IsActive == false)
                return;

        }

        public static MyMeteor CreateComet(Vector3 position, Vector3 direction)
        {
            float size = MyMwcUtils.GetRandomInt(m_sizeMin, m_sizeMax);

            Matrix worldMatrix = Matrix.CreateFromAxisAngle(MyMwcUtils.GetRandomVector3Normalized(), MyMwcUtils.GetRandomFloat(0, MathHelper.Pi));
            worldMatrix.Translation = position;

            MyMeteor meteor = MyMeteor.GenerateMeteor(size, worldMatrix, position, m_iceMeteorMaterials[MyMwcUtils.GetRandomInt(0, m_iceMeteorMaterials.Count)]);
            meteor.Start(direction, 953);

            return meteor;
        }
    }
}
