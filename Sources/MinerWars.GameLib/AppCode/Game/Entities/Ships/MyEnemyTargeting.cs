using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using MinerWarsMath;

using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Audio;
using MinerWars.AppCode.Game.Decals;
using MinerWars.AppCode.Game.Explosions;
using MinerWars.AppCode.Game.Gameplay;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.HUD;
using MinerWars.AppCode.Game.Inventory;
using MinerWars.AppCode.Game.Lights;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Entities.EntityDetector;
using MinerWars.AppCode.Game.Entities.FoundationFactory;
using MinerWars.AppCode.Game.Entities.Prefabs;
using MinerWars.AppCode.Game.Entities.SubObjects;
using MinerWars.AppCode.Game.Entities.Tools;
using MinerWars.AppCode.Game.Entities.Weapons;
using MinerWars.AppCode.Game.Entities.Weapons.UniversalLauncher;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.Physics;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Missions;
using MinerWars.AppCode.Game.Models;
using MinerWars.AppCode.Game.Prefabs;
using MinerWars.AppCode.Game.Radar;
using MinerWars.AppCode.Game.Render.SecondaryCamera;
using MinerWars.AppCode.Game.TransparentGeometry;
using MinerWars.AppCode.Game.TransparentGeometry.Particles;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.World.Global;
using MinerWars.AppCode.Physics;
using MinerWars.CommonLIB.AppCode.Generics;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using MinerWars.CommonLIB.AppCode.Utils;

using SysUtils.Utils;
using MinerWars.AppCode.Game.GUI.Helpers;
using MinerWars.AppCode.Game.Entities.Ships.SubObjects;
using MinerWars.AppCode.Game.Render;
using MinerWars.AppCode.Game.Entities.Weapons.Ammo;
using MinerWars.AppCode.Game.Entities.CargoBox;

namespace MinerWars.AppCode.Game.Entities
{
    static class MyEnemyTargeting
    {
        private const int ACTUALIZE_TIME = 1000;                        // in ms
        private const float ACTUALIZE_ORIENTATION = 0.5f;               // in cos angle
        private const float ACTUALIZE_DISTANCE_SQUARED = 10000f;        // in meters

        private static MyEntity m_owner;
        private static List<MyEntity> m_targets;
        private static int m_currentIndex;

        private static int m_lastTimeTargeting;
        private static Matrix m_lastOwnerWorldMatrix;

        private static int m_lastActualizeTime = 0;
        private static MyEntity m_currentTarget;
        public static MyEntity CurrentTarget
        {
            get { return m_currentTarget; }
        }

        static MyEnemyTargeting()
        {
            m_targets = new List<MyEntity>();
            MyScriptWrapper.EntityClose += new MyScriptWrapper.EntityHandler(MyScriptWrapper_EntityClose);
        }

        static void MyScriptWrapper_EntityClose(MyEntity entity)
        {
            if (entity == CurrentTarget)
            {
                /*
                if (MyHud.GetHudEnemiesOnScreen().Contains(entity))
                    MyHud.GetHudEnemiesOnScreen().Remove(entity);
                SwitchNextTarget(true);
                 */

                m_currentTarget = null; //will invoke next target selection in update
            }

            System.Diagnostics.Debug.Assert(entity != CurrentTarget);
        }

        public static void SwitchOwner(MyEntity owner)
        {
            m_owner = owner;
            m_lastTimeTargeting = 0;
            m_currentIndex = 0;
        }

        public static MyEntity SwitchNextTarget(bool forward)
        {
            MyEntity newTarget = null;
            m_targets.Clear();
            m_targets.AddRange(MyHud.GetHudEnemiesOnScreen());

            bool needActualizeIndex = NeedActualizeIndex();            

            int targetsLeft = m_targets.Count;
            while (targetsLeft > 0)
            {
                if (needActualizeIndex)
                {
                    m_currentIndex = 0;
                    m_lastOwnerWorldMatrix = m_owner.WorldMatrix;
                    needActualizeIndex = false;
                }
                else
                {
                    m_currentIndex += forward ? 1 : -1;
                }

                if (m_currentIndex >= m_targets.Count)
                {
                    m_currentIndex = 0;
                }
                if (m_currentIndex < 0)
                {
                    m_currentIndex = m_targets.Count - 1;
                }
                if (CanSee(m_owner, m_targets[m_currentIndex]) == null)
                {
                    newTarget = m_targets[m_currentIndex];
                    break;
                }
                targetsLeft--;                
            }
            m_lastTimeTargeting = MyMinerGame.TotalTimeInMilliseconds;

            if (m_owner is MySmallShip)
            {
                (m_owner as MySmallShip).TargetEntity = newTarget;
            }

            m_currentTarget = newTarget;
            return m_currentTarget;
        }

        private static bool NeedActualizeIndex()
        {
            int timeDiff = MyMinerGame.TotalTimeInMilliseconds - m_lastTimeTargeting;
            float cosAngle = Vector3.Dot(m_owner.WorldMatrix.Forward, m_lastOwnerWorldMatrix.Forward);
            float distance = Vector3.DistanceSquared(m_owner.WorldMatrix.Translation, m_lastOwnerWorldMatrix.Translation);
            return timeDiff > ACTUALIZE_TIME || cosAngle < ACTUALIZE_ORIENTATION || distance > ACTUALIZE_DISTANCE_SQUARED;
        }

        /// <summary>
        /// Chceck if ship can see target
        /// </summary>
        /// <param name="position">Target position</param>
        /// <param name="target">Target entity.</param>
        /// <returns>True, if bot can see position.</returns>
        public static MyEntity CanSee(MyEntity source, MyEntity target)
        {
            return CanSee(source, target.GetPosition(), target);
        }

        /// <summary>
        /// Chceck if ship can see target
        /// </summary>
        /// <param name="position">Target position</param>
        /// <param name="target">Target entity.</param>
        /// <returns>True, if bot can see position.</returns>
        public static MyEntity CanSee(MyEntity source, Vector3 position, MyEntity ignoreEntity)
        {
            MyIntersectionResultLineTriangleEx? result = null;

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MySmallShip CanSee");
            //There is 100 multiplier because Epsilon can get lost during transformation inside GetAllIntersectionWithLine
            //and normalization assert then appears
            if ((source.GetPosition() - position).Length() > MyMwcMathConstants.EPSILON * 100.0f)
            {
                var line = new MyLine(source.GetPosition(), position, true);
                result = MyEntities.GetIntersectionWithLine(ref line, source, ignoreEntity, ignoreChilds: true);
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            return result.HasValue ? result.Value.Entity : null;
        }


        public static void Update()
        {
            if ((MyMinerGame.TotalTimeInMilliseconds - m_lastActualizeTime) > ACTUALIZE_TIME)
            {
                m_lastActualizeTime = MyMinerGame.TotalTimeInMilliseconds;

                if (MyHud.GetHudEnemiesOnScreen().Count > 0 && CurrentTarget == null)
                {
                    SwitchNextTarget(true);
                    return;
                }

                if (CurrentTarget != null && !MyHud.GetHudEnemiesOnScreen().Contains(CurrentTarget))
                {
                    SwitchNextTarget(true);
                    return;
                }

                MySmallShip smallShipTarget = CurrentTarget as MySmallShip;
                if (smallShipTarget != null)
                {
                    if (smallShipTarget.IsEngineTurnedOff())
                    {
                        SwitchNextTarget(true);
                        return;
                    }
                }
            }
        }

        public static void Unload()
        {
            m_currentTarget = null;
            m_targets.Clear();
            m_owner = null;
        }
    }
}

