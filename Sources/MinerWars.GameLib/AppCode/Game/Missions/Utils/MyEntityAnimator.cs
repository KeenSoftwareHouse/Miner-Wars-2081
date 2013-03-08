using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Entities;

namespace MinerWars.AppCode.Game.Missions.Utils
{
    class MyEntityAnimator
    {
        public class MyKey
        {
            public float Time { get; set; }
            public Vector3 Position { get; set; }
            public Quaternion Rotation { get; set; }
        }

        public List<MyKey> Keys { get; set; }

        private float m_time;

        public MyEntityAnimator()
        {
            Keys = new List<MyKey>();
        }

        private int CompareKeysByTime(MyKey a, MyKey b)
        {
            return a.Time.CompareTo(b.Time);
        }

        private void GetValues(float time, out Vector3 position, out Quaternion rotation)
        {
            int index = -1;
            foreach (var key in Keys)
            {
                if (key.Time < time)
                {
                    ++index;
                }
                else
                {
                    break;
                }
            }

            MyKey key0 = Keys[(int)MathHelper.Clamp(index - 1, 0, Keys.Count - 1)];
            MyKey key1 = Keys[(int)MathHelper.Clamp(index, 0, Keys.Count - 1)];
            MyKey key2 = Keys[(int)MathHelper.Clamp(index + 1, 0, Keys.Count - 1)];
            MyKey key3 = Keys[(int)MathHelper.Clamp(index + 2, 0, Keys.Count - 1)];

            float deltaTime = key2.Time - key1.Time;
            float amount = deltaTime > 0 ? (time - key1.Time) / deltaTime : 1;

            position = new Vector3(
                MathHelper.CatmullRom(key0.Position.X, key1.Position.X, key2.Position.X, key3.Position.X, amount),
                MathHelper.CatmullRom(key0.Position.Y, key1.Position.Y, key2.Position.Y, key3.Position.Y, amount),
                MathHelper.CatmullRom(key0.Position.Z, key1.Position.Z, key2.Position.Z, key3.Position.Z, amount));

            rotation = Quaternion.Slerp(key1.Rotation, key2.Rotation, amount);
        }

        public void AddKey(float time, Vector3 position, Quaternion rotation)
        {
            Keys.Add(new MyKey
            {
                Time = time,
                Position = position,
                Rotation = rotation
            });
            Keys.Sort(CompareKeysByTime);
        }

        public void UpdateEntity(MyEntity entity)
        {
            Vector3 position;
            Quaternion rotation;
            GetValues(m_time, out position, out rotation);
            entity.MoveAndRotate(position, Matrix.CreateFromQuaternion(rotation));
        }

        public void UpdateTangent(MyEntity entity, Vector3 up)
        {
            Vector3 position0, position1;
            Quaternion rotation0, rotation1;

            GetValues(m_time - 0.05f, out position0, out rotation0);
            GetValues(m_time + 0.05f, out position1, out rotation1);

            Vector3 forward = position1 - position0;
            if (!MyUtils.IsValid(forward))
            {
                forward = entity.WorldMatrix.Forward;
            }

            entity.MoveAndRotate((position0 + position1) / 2, Matrix.CreateWorld(Vector3.Zero, forward, up));
        }

        public float GetAnimationLength()
        {
            return Keys.Count > 0 ? Keys[Keys.Count - 1].Time : 0;
        }

        public void JumpToTime(float time)
        {
            m_time = time;
        }

        public void Update()
        {
            m_time += MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS;
        }

        public void DebugDraw()
        {
            Vector3? prePosition = null;
            for (int i = 0; i < Keys.Count - 1; i++)
            {
                MyKey key0 = Keys[(int)MathHelper.Clamp(i - 1, 0, Keys.Count - 1)];
                MyKey key1 = Keys[(int)MathHelper.Clamp(i, 0, Keys.Count - 1)];
                MyKey key2 = Keys[(int)MathHelper.Clamp(i + 1, 0, Keys.Count - 1)];
                MyKey key3 = Keys[(int)MathHelper.Clamp(i + 2, 0, Keys.Count - 1)];

                MyDebugDraw.DrawAxis(Matrix.CreateTranslation(key1.Position), 10, 1);

                int steps = 50;
                for (int j = 0; j < steps; j++)
                {
                    float amount = 1.0f * j / steps;

                    Vector3 position = new Vector3(
                        MathHelper.CatmullRom(key0.Position.X, key1.Position.X, key2.Position.X, key3.Position.X, amount),
                        MathHelper.CatmullRom(key0.Position.Y, key1.Position.Y, key2.Position.Y, key3.Position.Y, amount),
                        MathHelper.CatmullRom(key0.Position.Z, key1.Position.Z, key2.Position.Z, key3.Position.Z, amount));

                    if (prePosition.HasValue)
                    {
                        MyDebugDraw.DrawLine3D(prePosition.Value, position, Color.Orange, Color.Orange);
                    }

                    prePosition = position;
                }
            }

            MyDebugDraw.DrawAxis(Matrix.CreateTranslation(Keys[Keys.Count - 1].Position), 10, 1);
        }
    }
}
