using System;
using MinerWarsMath;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.Utils;

namespace MinerWars.AppCode.Game.Entities.Weapons
{

    class MyLargeShipWeaponPrediction
    {
        private class MyPredictionRecord
        {
            private Curve m_curveX = null;
            private Curve m_curveY = null;
            private Curve m_curveZ = null;
            private int m_maxRecords = 1;
            private const float PREDICTION_MINIMAL_EPSILON = 1.0e-5f;

            public MyPredictionRecord(int MaxRecords, CurveLoopType PreLoopType = CurveLoopType.Linear, CurveLoopType PostLoopType = CurveLoopType.Linear)
            {
                m_curveX = new Curve();
                m_curveX.PreLoop = PreLoopType;
                m_curveX.PostLoop = PostLoopType;
                m_curveY = new Curve();
                m_curveY.PreLoop = PreLoopType;
                m_curveY.PostLoop = PostLoopType;
                m_curveZ = new Curve();
                m_curveZ.PreLoop = PreLoopType;
                m_curveZ.PostLoop = PostLoopType;
                m_maxRecords = MaxRecords;
            }

            public int GetRecordCount()
            {
                return m_curveX.Keys.Count;
            }

            public void AddRecord(Vector3 Point, float Time, Vector3 TangentIn, Vector3 TangentOut)
            {
                if (m_curveX.Keys.Count >= m_maxRecords)
                {
                    m_curveX.Keys.RemoveAt(0);
                    m_curveY.Keys.RemoveAt(0);
                    m_curveZ.Keys.RemoveAt(0);
                }

                m_curveX.Keys.Add(new CurveKey(Time, Point.X, TangentIn.X, TangentOut.X));
                m_curveY.Keys.Add(new CurveKey(Time, Point.Y, TangentIn.Y, TangentOut.Y));
                m_curveZ.Keys.Add(new CurveKey(Time, Point.Z, TangentIn.Z, TangentOut.Z));
            }

            public void AddRecord(Vector3 Point, float Time)
            {
                AddRecord(Point, Time, Vector3.Zero, Vector3.Zero);
            }

            private static void SetCurveKeyTangent(ref CurveKey Prev, ref CurveKey Current, ref CurveKey Next)
            {
                float dt = Next.Position - Prev.Position;
                float dv = Next.Value - Prev.Value;
                if (Math.Abs(dv) < float.Epsilon)
                {
                    Current.TangentIn = 0.0f;
                    Current.TangentOut = 0.0f;
                }
                else
                {
                    Current.TangentIn = dv * (Current.Position - Prev.Position) / dt;
                    Current.TangentOut = dv * (Current.Position - Prev.Position) / dt;
                }
            }


            public bool HasRecord()
            {
                return GetRecordCount() != 0;
            }

            public float GetStartTime()
            {
                if (GetRecordCount() != 0)
                    return m_curveX.Keys[0].Position;
                return 0.0f;
            }

            private float GetPrewTime()
            {
                if (GetRecordCount() > 1)
                {
                    return m_curveX.Keys[m_curveX.Keys.Count - 2].Position;
                }
                return 0.0f;
            }

            public float GetLastTime()
            {
                if (GetRecordCount() != 0)
                    return m_curveX.Keys[m_curveX.Keys.Count - 1].Position;
                return 0.0f;
            }

            public float GetRecordTime()
            {
                if (GetRecordCount() != 0)
                    return GetLastTime() - GetStartTime();
                return 0.0f;
            }

            public void Clear()
            {
                m_curveX.Keys.Clear();
                m_curveY.Keys.Clear();
                m_curveZ.Keys.Clear();
            }

            public bool GetRecordPoint(int index, ref Vector3 Point)
            {
                if (index < GetRecordCount())
                {
                    Point.X = m_curveX.Keys[index].Value;
                    Point.Y = m_curveY.Keys[index].Value;
                    Point.Z = m_curveZ.Keys[index].Value;
                    return true;
                }
                return false;
            }

            public Vector3 GetPredictedRecord(float Time)
            {
                float lastTime = GetLastTime();

                if (Time < lastTime)
                {
                    return new Vector3(m_curveX.Evaluate(Time), m_curveY.Evaluate(Time), m_curveZ.Evaluate(Time));
                }
                else
                {
                    // compute predicate point:
                    int count = GetRecordCount();
                    if (count > 1)
                    {
                        Vector3 Last = Vector3.Zero;
                        Vector3 Previous = Vector3.Zero;
                        GetRecordPoint(count - 1, ref Last);
                        GetRecordPoint(count - 2, ref Previous);

                        if (Last == Previous)
                        {
                            return Last;
                        }

                        Vector3 interpolated = LinearVector(ref Last, ref Previous, GetLastTime() - GetPrewTime(), Time - GetLastTime());

                        if (interpolated.Length() != 0.0f)
                            return Last + interpolated;

                        return Last;
                    }
                    else if (count == 1)
                    {
                        return new Vector3(m_curveX.Keys[0].Value, m_curveZ.Keys[0].Value, m_curveY.Keys[0].Value);
                    }
                }
                return Vector3.Zero;
            }

            public static Vector3 LinearVector(ref Vector3 B, ref Vector3 A, float LastTime, float t)
            {
                Vector3 diffVector = A - B;
                float length = diffVector.Length();
                if (diffVector.Length() < PREDICTION_MINIMAL_EPSILON)
                {
                    return A;
                }
                float lastSpeed = length / LastTime;
                float newLength = lastSpeed * t;

                diffVector = MyMwcUtils.Normalize(diffVector);
                diffVector = Vector3.Negate(diffVector);
                return Vector3.Multiply(diffVector, newLength);
            }

            public void DebugDraw_LinearValues()
            {
                Color color = Color.Yellow;
                if (GetRecordCount() > 1)
                    for (int i = 0; i < GetRecordCount() - 1; ++i)
                    {
                        Vector3 pointFrom = new Vector3();
                        Vector3 pointTo = new Vector3();
                        GetRecordPoint(i, ref pointFrom);
                        GetRecordPoint(i + 1, ref pointTo);
                        MyDebugDraw.DrawLine3D(pointFrom, pointTo, color, color);
                    }
            }

            public void DebugDraw_FromVectorValues(Vector3 Point, Color endColor)
            {
                Color color = Color.Violet;
                for (int i = 0; i < GetRecordCount(); ++i)
                {
                    Vector3 pointTo = new Vector3();
                    GetRecordPoint(i, ref pointTo);
                    MyDebugDraw.DrawLine3D(Point, pointTo, color, endColor);
                }
            }

            public float GetLastRecordSpeed()
            {
                int count = GetRecordCount();
                if (count > 1)
                {
                    float dt = GetLastTime() - m_curveX.Keys[m_curveX.Keys.Count - 2].Position;
                    Vector3 last = new Vector3();
                    Vector3 prew = new Vector3();
                    GetRecordPoint(count - 2, ref prew);
                    GetRecordPoint(count - 1, ref last);
                    float length = (last - prew).Length();
                    return length / dt;
                }
                return 0.0f;
            }

        }


        private MyPredictionRecord m_record = null;

        public MyLargeShipWeaponPrediction()
        {
            m_record = new MyPredictionRecord(/*10*/2);
        }

        public int GetRecordCount()
        {
            return m_record.GetRecordCount();
        }

        public void AddRecord(Vector3 Point, float dt)
        {
            m_record.AddRecord(Point, dt);
        }

        public Vector3 GetPredictedPosition(float Time)
        {
            return m_record.GetPredictedRecord(Time);
        }

        public bool HasPrediction()
        {
            return m_record.HasRecord();
        }

        public void Clear()
        {
            m_record.Clear();
        }

        public float GetLastRecordSpeed()
        {
            return m_record.GetLastRecordSpeed();
        }

        public void DebugDraw(Vector3 PointFrom, Color EndColor)
        {
            m_record.DebugDraw_LinearValues();
            m_record.DebugDraw_FromVectorValues(PointFrom, EndColor);
        }
    }
}
