using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.Audio;
using MinerWars.AppCode.Game.Explosions;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Entities.EntityDetector;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Utils;

namespace MinerWars.AppCode.Game.Missions.Components
{
    class MyMinesField : MyMissionComponent
    {
        private uint[,] m_mines;
        private float m_mineDamage;
        private float m_mineExplosionRadius;
        private float m_mineStartRadius;
        MySoundCue? m_beepCue = null;

        public MyMinesField(uint[,] mines, float mineDamage = 25,float mineExplosionRadius = 25,float mineStartRadius = 10)
        {
            m_mines = mines;
            m_mineDamage = mineDamage;
            m_mineExplosionRadius = mineExplosionRadius;
            m_mineStartRadius = mineStartRadius;
        }

        public override void Load(MyMissionBase sender)
        {
            base.Load(sender);

            for (int i = 0; i < m_mines.GetLength(0); i++)
            {
                MyEntityDetector mineDetector = MyScriptWrapper.GetDetector(m_mines[i, 1]);
                mineDetector.OnEntityPositionChange +=mineDetector_OnEntityPositionChange;
                mineDetector.On();
            }
        }

        private void mineDetector_OnEntityPositionChange(MyEntityDetector sender, MyEntity entity, Vector3 newposition)
        {
            if (sender.Closed)
                return;

            if (entity == MySession.PlayerShip)
            {
                if (m_beepCue == null || !m_beepCue.Value.IsPlaying)
                {
                    m_beepCue = MyAudio.AddCue2D(MySoundCuesEnum.SfxHudAlarmDamageA);
                }

                float distance = (entity.GetPosition() - sender.GetPosition()).Length();

                if (distance < m_mineStartRadius)
                {
                    uint mineId = 0;
                    for (int i = 0; i < m_mines.GetLength(0); i++)
                    {
                        if (m_mines[i, 1] == sender.Parent.EntityId.Value.NumericValue)
                        {
                            mineId = m_mines[i, 0];
                        }
                    }
                    ExplodeMine(mineId);
                    sender.Off();
                    sender.Parent.MarkForClose();
                }

            }
        }

        public void ExplodeMine(uint entityId)
        {
            MyEntity mine = MyScriptWrapper.GetEntity(entityId);
            MyExplosion newExplosion = MyExplosions.AddExplosion();
            if (newExplosion != null)
            {
                newExplosion.Start(0, m_mineDamage, 0, MyExplosionTypeEnum.BOMB_EXPLOSION, new BoundingSphere(mine.GetPosition(), m_mineExplosionRadius), MyExplosionsConstants.EXPLOSION_LIFESPAN, 1, ownerEntity: mine);
            }
            mine.MarkForClose();
        }


        public override void Update(MyMissionBase sender)
        {
            base.Update(sender);
        }

        public override void Success(MyMissionBase sender)
        {
            base.Success(sender);
        }

        public override void Unload(MyMissionBase sender)
        {
            base.Unload(sender);

            for (int i = 0; i < m_mines.GetLength(0); i++)
            {
                MyEntityDetector mineDetector = MyScriptWrapper.GetDetector(m_mines[i, 1]);
               //mineDetector.OnEntityPositionChange -= mineDetector_OnEntityPositionChange;
                //mineDetector.On();
            }
        }

    }
}
