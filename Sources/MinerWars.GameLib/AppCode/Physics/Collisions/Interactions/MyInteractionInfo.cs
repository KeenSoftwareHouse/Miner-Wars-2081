#region Using Statements

using System.Collections.Generic;
using MinerWarsMath;

#endregion

namespace MinerWars.AppCode.Physics
{
    /// <summary>
    /// Support classes for collision detection
    /// </summary>
    struct MySmallCollPointInfo
    {
        /// <summary>
        /// Estimated Penetration before the objects collide (can be -ve)
        /// </summary>
        public float m_InitialPenetration;

        /// <summary>
        /// Positions relative to body 0 (in world space)
        /// </summary>
        public Vector3 m_R0;

        /// <summary>
        /// positions relative to body 1 (if there is a body1)
        /// </summary>
        public Vector3 m_R1;

        public Vector3 m_Normal;

        public Vector3 m_V0;
        public Vector3 m_V1;


        /// <summary>
        /// Position of intersection in world space
        /// </summary>
        public Vector3 m_WorldPosition;

        public MySmallCollPointInfo(ref Vector3 R0, ref Vector3 R1, ref Vector3 V0, ref Vector3 V1, Vector3 normal, float initialPenetration, Vector3 worldPosition)
        {
            this.m_R0 = R0;
            this.m_R1 = R1;
            this.m_V0 = V0;
            this.m_V1 = V1;
            this.m_Normal = normal;
            this.m_InitialPenetration = initialPenetration;
            this.m_WorldPosition = worldPosition;
        }

        public MySmallCollPointInfo(Vector3 R0, Vector3 R1, Vector3 V0, Vector3 V1, Vector3 normal, float initialPenetration, Vector3 worldPosition)
        {
            this.m_R0 = R0;
            this.m_R1 = R1;
            this.m_V0 = V0;
            this.m_V1 = V1;
            this.m_Normal = normal;
            this.m_InitialPenetration = initialPenetration;
            this.m_WorldPosition = worldPosition;
        }
    }

    /// <summary>
    /// Contact cache
    /// </summary>
    class MyContactInfoCache
    {
        static MyContactInfoCache()
        {
            m_cache = new Stack<MySmallCollPointInfo[]>(CACHE_SIZE);
            while (m_cache.Count < CACHE_SIZE)
            {
                m_cache.Push(new MySmallCollPointInfo[MaxLocalStackSCPI]);
            }
        }

        public MyContactInfoCache()
        {
            while (m_cache.Count < CACHE_SIZE)
            {
                m_cache.Push(new MySmallCollPointInfo[MaxLocalStackSCPI]);
            }
        }

        public static MySmallCollPointInfo[] SCPIStackAlloc()
        {
            MySmallCollPointInfo[] retVal = null;
            lock (m_Locker)
            {
                if (m_cache.Count == 0)
                {
                    m_cache.Push(new MySmallCollPointInfo[MaxLocalStackSCPI]);
                }
                retVal = m_cache.Pop();
            }
            return retVal;
        }

        public static void FreeStackAlloc(MySmallCollPointInfo[] alloced)
        {
            lock (m_Locker)
            {
                m_cache.Push(alloced);
            }
        }

        public const int MaxLocalStackSCPI = MyPhysicsConfig.MaxContactPoints;
        private const int CACHE_SIZE = 64;
        private static Stack<MySmallCollPointInfo[]> m_cache;
        private static readonly object m_Locker = new object();
    }

    //////////////////////////////////////////////////////////////////////////

    class MyCollPointInfo
    {
        public MySmallCollPointInfo m_Info;

        /// <summary>
        /// Estimated Penetration before the objects collide (can be -ve)
        /// </summary>
        public float InitialPenetration
        {
            get
            {
                return m_Info.m_InitialPenetration;
            }
        }

        /// <summary>
        /// Positions relative to body 0 (in world space)
        /// </summary>
        public Vector3 R0
        {
            get
            {
                return m_Info.m_R0;
            }
        }

        /// <summary>
        /// positions relative to body 1 (if there is a body1)
        /// </summary>
        public Vector3 R1
        {
            get
            {
                return m_Info.m_R1;
            }
        }

        /// <summary>
        /// Used by physics to cache desired minimum separation velocity
        /// in the normal direction
        /// </summary>
        public float m_MinSeparationVel;

        /// <summary>
        /// Used by physics to cache value used in calculating impulse
        /// </summary>
        public float m_Denominator;

        /// <summary>
        /// Used by physics to accumulated the normal impulse
        /// </summary>
        public float m_AccumulatedNormalImpulse;

        /// <summary>
        /// Used by physics to accumulated the normal impulse
        /// </summary>
        public Vector3 m_AccumulatedFrictionImpulse;

        /// <summary>
        /// Used by physics to accumulated the normal impulse
        /// </summary>
        public float m_AccumulatedNormalImpulseAux;

        /// <summary>
        /// Used by physics to cache the world position (not really
        /// needed? pretty useful in debugging!)
        /// </summary>
        public Vector3 m_Position;


        public void Init(ref MySmallCollPointInfo m_Info)
        {
            this.m_Info = m_Info;
            this.m_Denominator = 0.0f;
            this.m_AccumulatedNormalImpulse = 0.0f;
            this.m_AccumulatedNormalImpulseAux = 0.0f;
            this.m_AccumulatedFrictionImpulse = Vector3.Zero;
            this.m_Position = Vector3.Zero;
            this.m_MinSeparationVel = 0.0f;
        }
    }
}