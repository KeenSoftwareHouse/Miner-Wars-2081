using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Managers.EntityManager.Entities;
using MinerWars.AppCode.Game.HUD;
using MinerWars.AppCode.Game.Explosions;
using Microsoft.Xna.Framework;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.Models;
using MinerWars.AppCode.Game.GUI;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.AppCode.Game.TransparentGeometry;
using MinerWars.AppCode.Game.Lights;
using MinerWars.AppCode.Game.Render;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Managers.EntityManager.Entities.InfluenceSpheres;
using MinerWars.AppCode.Game.Managers.Session;

namespace MinerWars.AppCode.Game.Utils
{
    static class MyTestMission
    {
        static bool m_outpostReached = false;
        static int m_remainingPrimaryTargetsCounter = 4;
        static bool m_primaryTargetTextAlreadyAdded = false;
        //static bool m_russianOutpostTextAlreadyAdded = false;
        static MyLight m_russianDropZoneLight = null;
        public static readonly Vector3 RUSSIAN_DROP_ZONE_POSITION = new Vector3(-9664.0f, 8818.0f, 53268.0f);
        const float MAX_ENEMY_HUD_VISIBILITY_DISTANCE = 3000f;
        const float REMOVE_FROM_HUD_POSSIBLE_OUTPOST_DISTANCE = 3000f;
        static Vector4 m_dropZoneLightColor = new Vector4(1,0.92f,0.62f, 1);

        static MyTestMission()
        {
             MyRender.RegisterRenderModule("Test mission 1", Draw, MyRenderStage.PrepareForDraw);
        }


        public static bool UpdateSmallShipBotScript(MySmallShipBot smallShipBot)
        {

            // Make all small ship enemy bots fly towards Russian Outpost
            if (m_remainingPrimaryTargetsCounter == 0)
            {
				//TODO: not working with new bot version
                //smallShipBot.Decision.TargetPosition = RUSSIAN_DROP_ZONE_POSITION;
                //smallShipBot.SetBehavior<MyBotBehaviorFollow>();
            }

            return true;
        }

        public static bool UpdateSmallDebrisScript(MySmallDebris smallDebris)
        {
            if (smallDebris.ModelLod0.ModelEnum == MyModelsEnum.cistern)
            {
                if (m_outpostReached == true && m_primaryTargetTextAlreadyAdded == false)
                {
                    bool containsHud = MyHud.ContainsTextForEntity(smallDebris);
                    if (containsHud == false) MyHud.AddText(smallDebris, new StringBuilder("Primary target"), Color.Green);
                }

                if (smallDebris.IsDestroyed())
                {
                    MyHud.RemoveText(smallDebris);
                    //  small ship explosion!
                    MyExplosion explosion = MyExplosions.AddExplosion();
                    Vector3 explosionPosition = smallDebris.GetPosition();
                    if (explosion != null)
                    {
                        explosion.Start(
                            MyExplosionTypeEnum.SMALL_SHIP_EXPLOSION, new BoundingSphere(explosionPosition,
                            MyMwcUtils.GetRandomFloat(MyExplosionsConstants.EXPLOSION_RANDOM_RADIUS_MIN, MyExplosionsConstants.EXPLOSION_RANDOM_RADIUS_MAX)),
                            MyExplosionsConstants.EXPLOSION_LIFESPAN);
                    }

                    smallDebris.Close();
                    m_remainingPrimaryTargetsCounter--;
                    return false;
                }
            }

            return true;
        }

        public static bool UpdateStaticAsteroidScript(MyStaticAsteroid staticAsteroid)
        {
            if (MyHud.ContainsTextForEntity(staticAsteroid))
            {
                if (m_outpostReached == false)
                {
                    if (Vector3.Distance(MySession.PlayerShip.GetPosition(), staticAsteroid.GetPosition()) < REMOVE_FROM_HUD_POSSIBLE_OUTPOST_DISTANCE)
                    {
                        MyHud.RemoveText(staticAsteroid);
                    }
                }
                else
                {
                    MyHud.RemoveText(staticAsteroid);
                }
            }

            return true;
        }

        public static bool UpdateVoxelMapScript(MyVoxelMap voxelMap)
        {
            if (MyHud.ContainsTextForEntity(voxelMap))
            {
                if (m_outpostReached == false)
                {
                    if (Vector3.Distance(MySession.PlayerShip.GetPosition(), voxelMap.GetPosition()) < REMOVE_FROM_HUD_POSSIBLE_OUTPOST_DISTANCE)
                    {
                        MyHud.RemoveText(voxelMap);
                    }
                }
                else
                {
                    MyHud.RemoveText(voxelMap);
                }
            }

            return true;
        }

        public static bool UpdateLargeShipScript(MyLargeShip largeShip)
        {
            /*if (m_outpostReached == false &&
                    largeShip.ModelLod0.GetModelEnum() == MyModelsEnum.Test_JeromieInteriorLevel_LOD0 &&
                    Vector3.Distance(MySession.PlayerShip.GetPosition(), largeShip.GetPosition()) < 3000.0f)
            {
                if (m_russianOutpostTextAlreadyAdded == false)
                {
                    MyHud.AddText(largeShip, new StringBuilder("Russian Outpost"), Color.Red);
                    m_outpostReached = true;
                    m_russianOutpostTextAlreadyAdded = true;
                }

            }*/

            return true;
        }

        public static bool UpdateInfluenceSphereSoundScript(MyInfluenceSphereSound influenceSphereSound)
        {
            if ((m_remainingPrimaryTargetsCounter == 0) && (MyHud.ContainsTextForEntity(influenceSphereSound) == false))
            {
                MyHud.AddText(influenceSphereSound, new StringBuilder("Russian Drop Zone"), Color.Green);
            }

            return true;
        }

        public static void UpdateEntityHealth(MyEntity entity, float healthDamage)
        {
            if (entity != null && entity is MySmallDebris)
            {
                ((MySmallDebris)entity).AddHealth(healthDamage);
            }
        }

        public static void Draw()
        {
            if (MyFakes.TEST_MISSION_1_ENABLED)
            {

                if (m_remainingPrimaryTargetsCounter == 0)
                {
                    if (m_russianDropZoneLight != null)
                    {
                        Vector3 dir = MyMwcUtils.Normalize(MyCamera.Position - RUSSIAN_DROP_ZONE_POSITION);

                        float timeBlic = MyMinerGame.TotalGamePlayTimeInMilliseconds % 980;
                        if (timeBlic > 250) timeBlic = 980 - timeBlic;
                        timeBlic = MathHelper.Clamp(1 - timeBlic / 250, 0, 1);

                        float alpha = MathHelper.Lerp(0.1f, 0.6f, timeBlic);

                        m_dropZoneLightColor.W = alpha;
                        m_russianDropZoneLight.Start(MyLight.LightTypeEnum.PointLight, RUSSIAN_DROP_ZONE_POSITION, m_dropZoneLightColor, 1, 200);

                        float radius = MathHelper.Lerp(0.1f, 150f, timeBlic);

                        MyTransparentGeometry.AddPointBillboard(MyTransparentMaterialEnum.ReflectorGlareAlphaBlended, m_dropZoneLightColor,
                                    RUSSIAN_DROP_ZONE_POSITION + dir * 5, radius, 0);
                    }
                    else
                    {
                        m_russianDropZoneLight = MyLights.AddLight();
                        if (m_russianDropZoneLight != null)
                        {
                            m_russianDropZoneLight.Start(MyLight.LightTypeEnum.PointLight, 1);
                        }
                    }
                }
            }
        }
    }
}
