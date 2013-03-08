#region Using Statements

using System;
using System.Collections.Generic;
using System.Threading;
using MinerWarsMath;
using MinerWars.CommonLIB.AppCode.Utils;

#endregion

namespace MinerWars.AppCode.Physics
{

    //////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Stores informations about contacts and holds support containers. This class is responsible for the contact callbacks.
    /// </summary>
    class MyContactConstraintModule
    {
        static MyContactConstraintModule()
        {
            while (m_FreeCc.Count < m_InitialCollisionInfoStack)
            {
                m_FreeCc.Push(new MyRBContactConstraint());
            }
            while (m_FreePtInfos.Count < m_InitialCollisionPointInfoStack)
            {
                m_FreePtInfos.Push(new MyCollPointInfo());
            }
        }

        public MyContactConstraintModule()
        {
            m_ActiveContactConstrains.Clear();
            m_ContactEventCache.Clear();
            m_StartContactEvents.Clear();
            m_EndContactEvents.Clear();
            m_TouchContactEvents.Clear();

            while (m_FreeCc.Count < m_InitialCollisionInfoStack)
            {
                m_FreeCc.Push(new MyRBContactConstraint());
            }
            while (m_FreePtInfos.Count < m_InitialCollisionPointInfoStack)
            {
                m_FreePtInfos.Push(new MyCollPointInfo());
            }

            m_ContactInfoCache = new MyContactInfoCache();
            m_TriangleCache = new MyTriangleCache();            
        }
        

        public void AddContactConstraint(MyRBElementInteraction interaction, MySmallCollPointInfo[] pointInfos, int numCollPts)
        {
            lock (m_Locker)
            {             
                if (m_FreeCc.Count == 0)
                {
                    m_FreeCc.Push(new MyRBContactConstraint());
                }
                MyRBContactConstraint cc = m_FreeCc.Pop();
                cc.Init(interaction, pointInfos, numCollPts);
                m_ActiveContactConstrains.Add(cc);             
            }
        }

        public MyCollPointInfo PopCollPointInfo()
        {
            MyCollPointInfo retVal = null;
            lock (m_Locker)
            {
                if (m_FreePtInfos.Count == 0)
                {
                    m_FreePtInfos.Push(new MyCollPointInfo());
                }
                retVal = m_FreePtInfos.Pop();
            }            
            return retVal;
        }

        public void PushCollPointInfo(MyCollPointInfo info)
        {
            lock (m_Locker)
            {
                m_FreePtInfos.Push(info);
            }
        }

        private void FreeRBContactConstraint(MyRBContactConstraint info)
        {
            info.Destroy();
            m_FreeCc.Push(info);
        }

        /// <summary>
        /// deletes all contact info and checks it with last one in order to generate contact notifications
        /// A.B. this is using list - will be slow in case of lots of objects
        /// </summary>
        public void Flush()
        {
            m_StartContactEvents.Clear();
            m_TouchContactEvents.Clear();
            m_EndContactEvents.Clear();
            for (int i = 0; i < m_ActiveContactConstrains.Count; i++)
            {
                if (m_ActiveContactConstrains[i].GetRBElementInteraction().RBElement1 == null || m_ActiveContactConstrains[i].GetRBElementInteraction().RBElement2 == null)
                    continue;

                if (m_ActiveContactConstrains[i].GetRBElementInteraction().GetRigidBody1().NotifyContactHandler == null && m_ActiveContactConstrains[i].GetRBElementInteraction().GetRigidBody2().NotifyContactHandler == null)
                {
                    continue;
                }

                uint guid = m_ActiveContactConstrains[i].GUID;

                MyContactEventInfo cei = null;

                if (m_ContactEventCache.TryGetValue(guid,out cei))
                {
                    // contact found should we make touch?
                    if ((uint)MyPhysics.physicsSystem.GetRigidBodyModule().GetRigidBodyContactEventTypeMask(cei.m_RigidBody1.Type, cei.m_RigidBody2.Type) > ((uint)MyContactEventType.CET_START_AND_END))
                    {
                        m_TouchContactEvents.Add(cei);
                    }
                    else
                    {
                        m_FreeCei.Push(cei);
                    }

                    m_ContactEventCache.Remove(guid);
                }
                else
                {
                    // contact not found new contact
                    if (m_FreeCei.Count == 0)
                    {
                        m_FreeCei.Push(new MyContactEventInfo());
                    }

                    cei = m_FreeCei.Pop();
                    cei.Fill(m_ActiveContactConstrains[i]);
                    m_StartContactEvents.Add(cei);
                }
            }

            foreach (KeyValuePair<uint, MyContactEventInfo> kvp in m_ContactEventCache)
            {
                m_EndContactEvents.Add(kvp.Value);
            }

            m_ContactEventCache.Clear();

            FireContactCallbacks();

            for (int i = 0; i< m_ActiveContactConstrains.Count; i++)
            {
                FreeRBContactConstraint(m_ActiveContactConstrains[i]);

                if (m_ActiveContactConstrains[i].GetRBElementInteraction().RBElement1 == null || m_ActiveContactConstrains[i].GetRBElementInteraction().RBElement2 == null)
                    continue;

                if (m_ActiveContactConstrains[i].GetRBElementInteraction().GetRigidBody1().NotifyContactHandler == null && m_ActiveContactConstrains[i].GetRBElementInteraction().GetRigidBody2().NotifyContactHandler == null)
                {
                    continue;
                }

                // else fill new contact event cache                
                if (m_FreeCei.Count == 0)
                {
                    m_FreeCei.Push(new MyContactEventInfo());
                }
                MyContactEventInfo cei = m_FreeCei.Pop();
                cei.Fill(m_ActiveContactConstrains[i]);
                if (!m_ContactEventCache.ContainsKey(m_ActiveContactConstrains[i].GUID))
                {
                    m_ContactEventCache.Add(m_ActiveContactConstrains[i].GUID, cei);
                }
            }

            m_ActiveContactConstrains.Clear();
        }

        /// <summary>
        /// calls the user contact callbacks
        /// </summary>
        private void FireContactCallbacks()
        {
            MyRigidBodyModule module = MyPhysics.physicsSystem.GetRigidBodyModule();

            for (int i = 0; i < m_StartContactEvents.Count; i++)
            {
                MyContactEventInfo coi = m_StartContactEvents[i];

                MyContactEventType cet = module.GetRigidBodyContactEventTypeMask(coi.m_RigidBody1.Type, coi.m_RigidBody2.Type);

                if (cet == MyContactEventType.CET_START || cet == MyContactEventType.CET_START_AND_END || cet == MyContactEventType.CET_START_AND_TOUCH || cet == MyContactEventType.CET_START_AND_TOUCH_AND_END)
                {
                    if (coi.m_RigidBody1.NotifyContactHandler != null)
                    {
                        coi.m_RigidBody1.NotifyContactHandler.OnContactStart(coi);
                    }

                    if (coi.m_RigidBody2.NotifyContactHandler != null)
                    {
                        coi.m_RigidBody2.NotifyContactHandler.OnContactStart(coi);
                    }
                }
            }

            for (int i = 0; i < m_TouchContactEvents.Count; i++)
            {
                MyContactEventInfo coi = m_TouchContactEvents[i];

                MyContactEventType cet = module.GetRigidBodyContactEventTypeMask(coi.m_RigidBody1.Type, coi.m_RigidBody2.Type);

                if (cet == MyContactEventType.CET_TOUCH || cet == MyContactEventType.CET_START_AND_TOUCH || cet == MyContactEventType.CET_END_AND_TOUCH || cet == MyContactEventType.CET_START_AND_TOUCH_AND_END)
                {
                    if (coi.m_RigidBody1.NotifyContactHandler != null)
                    {
                        coi.m_RigidBody1.NotifyContactHandler.OnContactTouch(coi);
                    }

                    if (coi.m_RigidBody2.NotifyContactHandler != null)
                    {
                        coi.m_RigidBody2.NotifyContactHandler.OnContactTouch(coi);
                    }
                }

                m_FreeCei.Push(coi);
            }

            for (int i = 0; i < m_EndContactEvents.Count; i++)
            {
                MyContactEventInfo coi = m_EndContactEvents[i];

                MyContactEventType cet = module.GetRigidBodyContactEventTypeMask(coi.m_RigidBody1.Type, coi.m_RigidBody2.Type);

                if (cet == MyContactEventType.CET_END || cet == MyContactEventType.CET_START_AND_END || cet == MyContactEventType.CET_END_AND_TOUCH || cet == MyContactEventType.CET_START_AND_TOUCH_AND_END)
                {
                    if (coi.m_RigidBody1.NotifyContactHandler != null)
                    {
                        coi.m_RigidBody1.NotifyContactHandler.OnContactEnd(coi);
                    }

                    if (coi.m_RigidBody2.NotifyContactHandler != null)
                    {
                        coi.m_RigidBody2.NotifyContactHandler.OnContactEnd(coi);
                    }
                }

                m_FreeCei.Push(coi);
            }
        }

        public MyTriangleCache  GetTriangleCache()
        {
            return m_TriangleCache;
        }

        public List<MyRBContactConstraint> GetActiveRBContactConstraints()
        {
            return m_ActiveContactConstrains;
        }


        public void Destroy()
        {
            m_ContactEventCache.Clear();
            m_StartContactEvents.Clear();
            m_TouchContactEvents.Clear();
            m_EndContactEvents.Clear();
            m_ActiveContactConstrains.Clear();
            m_FreeCc.Clear();
            m_FreeCei.Clear();
            m_FreePtInfos.Clear();
        }

        private static Dictionary<uint, MyContactEventInfo> m_ContactEventCache = new Dictionary<uint, MyContactEventInfo>(512);
        private static List<MyContactEventInfo> m_StartContactEvents = new List<MyContactEventInfo>(512);
        private static List<MyContactEventInfo> m_TouchContactEvents = new List<MyContactEventInfo>(512);
        private static List<MyContactEventInfo> m_EndContactEvents = new List<MyContactEventInfo>(512);
        private static List<MyRBContactConstraint> m_ActiveContactConstrains = new List<MyRBContactConstraint>(512);
        public const int m_InitialCollisionInfoStack = 1024;
        public const int m_InitialCollisionPointInfoStack = 4096;
        private static Stack<MyRBContactConstraint> m_FreeCc = new Stack<MyRBContactConstraint>(m_InitialCollisionInfoStack);
        private static Stack<MyContactEventInfo> m_FreeCei = new Stack<MyContactEventInfo>(m_InitialCollisionInfoStack);
        private static Stack<MyCollPointInfo> m_FreePtInfos = new Stack<MyCollPointInfo>(m_InitialCollisionPointInfoStack);

        private MyContactInfoCache m_ContactInfoCache;
        protected MyTriangleCache m_TriangleCache;

        private readonly object m_Locker = new object();
    }
}