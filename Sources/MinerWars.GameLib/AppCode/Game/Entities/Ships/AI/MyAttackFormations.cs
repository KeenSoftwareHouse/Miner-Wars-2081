using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWarsMath;

namespace MinerWars.AppCode.Game.Entities.Ships.AI
{
    internal class MyAttackFormations
    {
        const float FORMATION_LEADER_RANGE_SQR = 500 * 500;
        const float FORMATION_FORWARD_PROJECTION = 400;

        public class MyAttackFormation
        {
            public MyEntity Target { get; set; }
            public MyEntity Leader { get; set; }
            public List<MyEntity> Followers { get; set; }

            public MyAttackFormation()
            {
                Followers = new List<MyEntity>();
            }

            public void Add(MyEntity entity)
            {
                Followers.Add(entity);
            }

            public Vector3 GetFormationPosition(MyEntity entity)
            {
                if (entity == Leader)
                {
                    return Target.GetPosition();
                }

                int index = Followers.IndexOf(entity);
                if (index >= 0)
                {
                    float FORMATION_SPACING = 100;

                    Vector3 leaderToTarget = Target.GetPosition() - Leader.GetPosition();
                    Matrix matrix = Matrix.CreateWorld(Leader.GetPosition(), leaderToTarget, Leader.WorldMatrix.Up);

                    int sideIndex = index / 2 + 1;
                    int sideSign = index % 2 * 2 - 1;   // just -1 is left and +1 right

                    leaderToTarget.Normalize();

                    return Leader.GetPosition() + leaderToTarget * FORMATION_FORWARD_PROJECTION + matrix.Left * sideIndex * sideSign * FORMATION_SPACING;
                }

                return Vector3.Zero;
            }
        }

        public static MyAttackFormations Instance = new MyAttackFormations();
        public List<MyAttackFormation> Formations { get; set; }

        public MyAttackFormations()
        {
            Formations = new List<MyAttackFormation>();
        }

        public Vector3 GetFormationPosition(MyEntity entity, MyEntity target)
        {
            MyAttackFormation entityFormation = null;
            
            // Try find existing formation (if entity is already in some)
            foreach (var formation in Formations)
            {
                if (formation.Target == target)
                {
                    // Already  in formation as leader or follower
                    if (formation.Leader == entity || formation.Followers.Contains(entity))
                    {
                        entityFormation = formation;
                        break;
                    }
                }
            }

            // Try enter existing formation (if it is close enough)
            if (entityFormation == null)
            {
                foreach (var formation in Formations)
                {
                    if (formation.Target == target)
                    {
                        if ((formation.Leader.GetPosition() - entity.GetPosition()).LengthSquared() <= FORMATION_LEADER_RANGE_SQR)
                        {
                            formation.Add(entity);
                            entityFormation = formation;
                            break;
                        }
                    }
                }
            }

            // All failed, create new formation
            if (entityFormation == null)
            {
                entityFormation = new MyAttackFormation();
                entityFormation.Target = target;
                entityFormation.Leader = entity;
                Formations.Add(entityFormation);
            }

            return entityFormation.GetFormationPosition(entity);
        }

        public void RemoveEntity(MyEntity entity)
        {
            foreach (MyAttackFormation formation in Formations.ToArray())
            {
                if (formation.Leader == entity)
                {
                    Formations.Remove(formation);
                }

                if (formation.Followers.Remove(entity))
                {
                    break;
                }
            }
        }

        public void Update()
        {
            //Debug.WriteLine("Formations Count: " + Formations.Count);
            // TODO: sort formations, every follower should travel only shortest route to formation position
        }
    }
}
