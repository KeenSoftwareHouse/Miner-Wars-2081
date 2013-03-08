using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Utils;
using System.Diagnostics;

namespace MinerWars.AppCode.Game.Missions.Components
{
    class MyMovingEntity : MyMissionComponent
    {

        private readonly uint? m_shipId;
        private readonly string m_shipName;
        private readonly uint m_targetId;
        private readonly int m_time;
        private readonly bool m_isShip;

        private float m_progress;

        private MyEntity m_ship;
        private MyLine m_trajectory;
        private bool m_shipMoving = false;

        public MyMovingEntity(uint shipId, uint targetId, int timeMilliseconds, bool isShip = true)
        {
            m_shipId = shipId;
            m_targetId = targetId;
            m_time = timeMilliseconds;
            m_isShip = isShip;
        }


        public MyMovingEntity(string shipName, uint targetId, int timeMilliseconds, bool isShip = true)
        {
            m_shipName = shipName;
            m_targetId = targetId;
            m_time = timeMilliseconds;
            m_isShip = isShip;
        }


        public override void Load(MyMissionBase sender)
        {
            base.Load(sender);
            m_ship = m_shipId.HasValue ? MyScriptWrapper.GetEntity(m_shipId.Value) : MyScriptWrapper.GetEntity(m_shipName);
            m_trajectory = new MyLine(m_ship.GetPosition(), MyScriptWrapper.GetEntity(m_targetId).GetPosition());
            
            if (m_isShip)
            {
                MyScriptWrapper.PrepareMotherShipForMove(m_ship);
            }
            m_shipMoving = true;
        }



        public override void Update(MyMissionBase sender)
        {
            base.Update(sender);
            var progress = sender.MissionTimer.ElapsedTime / (float)m_time;
            if (progress < 1.0f && m_shipMoving)
            {
                var position = Vector3.SmoothStep(m_trajectory.From, m_trajectory.To, progress);
                MyScriptWrapper.Move(m_ship, position);
            }
            else
            {
                StopShip();
            }
        }

        public void StopShip()
        {
            if (m_shipMoving)
            {
                m_shipMoving = false;
                if (m_isShip)
                {
                    MyScriptWrapper.ReturnMotherShipFromMove(m_ship);
                }
            }
        }

        public override void Unload(MyMissionBase sender)
        {
            base.Unload(sender);

            //Ensure that entity is on the end of trajectory
            if (m_shipMoving)
            {
                MyScriptWrapper.Move(m_ship, m_trajectory.To);

                if (!MyFakes.TEST_MISSION_GAMEPLAY)
                {
                    Debug.Assert(!sender.IsCompleted() || Vector3.Distance(m_ship.GetPosition(), m_trajectory.To) < 0.1);
                } 
            }

            StopShip();
        }

    }
}
