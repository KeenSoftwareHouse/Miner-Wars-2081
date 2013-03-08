#region Using Statements
using System.Collections.Generic;
using MinerWarsMath;
#endregion

namespace MinerWars.AppCode.Physics
{
    //////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Rigid body island is a group of rigid bodies that are bind together through the constraints. The island can be then solved on a separate solver. The deactivation
    /// is then handled on the whole island. The island is simulated by the larges num iteraction steps set on inserted rigid.
    /// </summary>
    class MyRigidBodyIsland
    {
        public MyRigidBodyIsland()
        {
            m_Rigids = new List <MyRigidBody>(8);
        }
        public void AddRigidBody(MyRigidBody rbo)
        { 
            // make the add unique
            for (int i = 0; i < m_Rigids.Count; i++)
            {
                if(m_Rigids[i] == rbo)
                    return;
            }

            if (rbo.IterationCount > m_IterationCount)
                m_IterationCount = rbo.IterationCount;
            m_Rigids.Add(rbo); 
        }
        public void RemoveRigidBody(MyRigidBody rbo) { m_Rigids.Remove(rbo); }

        public void Update(float timeStep)
        {
        }

        public uint IterationCount { get { return m_IterationCount; } set { m_IterationCount = value; }} 

        public List <MyRigidBody> GetRigids()
        {
            return m_Rigids;
        }

        public void Clear()
        {
            m_Rigids.Clear();

        }

        private List <MyRigidBody>  m_Rigids;        
        private uint m_IterationCount;
    }

}