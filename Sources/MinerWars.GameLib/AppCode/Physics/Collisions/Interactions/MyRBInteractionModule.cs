#region Using Statements

using System.Collections.Generic;
using SysUtils.Utils;

#endregion

namespace MinerWars.AppCode.Physics
{
    //////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Interaction module that hadnles possible element type interactions. You register new interaction if you need to, to have pool for it
    /// </summary>
    class MyRBInteractionModule
    {
        private const int m_preAllocCount = 64;

        static MyRBInteractionModule()
        {
            for (int i = 0; i < (int)MyRBElementType.ET_LAST; i++)
                for (int j = 0; j < (int)MyRBElementType.ET_LAST; j++)
                    if (i <= j)
                        m_IslandsPool[i, j] = new List<MyRBElementInteraction>(m_preAllocCount);
        }
        
        public MyRBInteractionModule()
        {
            Clear();
        }

        public void Init()
        {
            // register all interactions!

            // sphere
            MyRBSphereElementSphereElementInteraction sphsph = new MyRBSphereElementSphereElementInteraction();
            RegisterRBElementInteraction(MyRBElementType.ET_SPHERE, MyRBElementType.ET_SPHERE, sphsph);

            MyRBSphereElementBoxElementInteraction sphbox = new MyRBSphereElementBoxElementInteraction();
            RegisterRBElementInteraction(MyRBElementType.ET_SPHERE, MyRBElementType.ET_BOX, sphbox);

            MyRBSphereElementCapsuleElementInteraction sphcap = new MyRBSphereElementCapsuleElementInteraction();
            RegisterRBElementInteraction(MyRBElementType.ET_SPHERE, MyRBElementType.ET_CAPSULE, sphcap);

            MyRBSphereElementTriangleMeshElementInteraction sphtri = new MyRBSphereElementTriangleMeshElementInteraction();
            RegisterRBElementInteraction(MyRBElementType.ET_SPHERE, MyRBElementType.ET_TRIANGLEMESH, sphtri);

            MyRBSphereElementVoxelElementInteraction sphvox = new MyRBSphereElementVoxelElementInteraction();
            RegisterRBElementInteraction(MyRBElementType.ET_SPHERE, MyRBElementType.ET_VOXEL, sphvox);

            // box
            MyRBBoxElementBoxElementInteraction boxbox = new MyRBBoxElementBoxElementInteraction();
            RegisterRBElementInteraction(MyRBElementType.ET_BOX, MyRBElementType.ET_BOX, boxbox);

            MyRBBoxElementCapsuleElementInteraction boxcap = new MyRBBoxElementCapsuleElementInteraction();
            RegisterRBElementInteraction(MyRBElementType.ET_BOX, MyRBElementType.ET_CAPSULE, boxcap);

            MyRBBoxElementTriangleMeshElementInteraction boxtri = new MyRBBoxElementTriangleMeshElementInteraction();
            RegisterRBElementInteraction(MyRBElementType.ET_BOX, MyRBElementType.ET_TRIANGLEMESH, boxtri);

            MyRBBoxElementVoxelElementInteraction boxvox = new MyRBBoxElementVoxelElementInteraction();
            RegisterRBElementInteraction(MyRBElementType.ET_BOX, MyRBElementType.ET_VOXEL, boxvox);

            // capsule
            MyRBCapsuleElementCapsuleElementInteraction capcap = new MyRBCapsuleElementCapsuleElementInteraction();
            RegisterRBElementInteraction(MyRBElementType.ET_CAPSULE, MyRBElementType.ET_CAPSULE, capcap);

            MyRBCapsuleElementTriangleMeshElementInteraction captri = new MyRBCapsuleElementTriangleMeshElementInteraction();
            RegisterRBElementInteraction(MyRBElementType.ET_CAPSULE, MyRBElementType.ET_TRIANGLEMESH, captri);

            MyRBCapsuleElementVoxelElementInteraction capvox = new MyRBCapsuleElementVoxelElementInteraction();
            RegisterRBElementInteraction(MyRBElementType.ET_CAPSULE, MyRBElementType.ET_VOXEL, capvox);
        }

        public void Destroy()
        {
            Clear();
        }

        private void Clear()
        {
            for (int i = 0; i < (int)MyRBElementType.ET_LAST; i++)
                for (int j = 0; j < (int)MyRBElementType.ET_LAST; j++)
                {
                    if (i <= j)
                        m_IslandsPool[i, j].Clear();
                    m_IslandsPoolStatic[i, j] = null;
                }
        }

        /// <summary>
        /// find interaction method from pool
        /// </summary>

        public MyRBElementInteraction FindRBElementInteractionForStaticTesting(MyRBElementType type1, MyRBElementType type2)
        {
            int t1 = (int)type1;
            int t2 = (int)type2;

            MyRBElementInteraction intrList = null;
            if (t1 < t2)
                intrList = m_IslandsPoolStatic[t1, t2];
            else
                intrList = m_IslandsPoolStatic[t2, t1];

            return intrList;
        }


        /// <summary>
        /// Do static Test of intersection
        /// </summary>
        public bool DoStaticTestInteraction(MyRBElement el1, MyRBElement el2)
        {

            MyRBElementInteraction myElemInteraction = FindRBElementInteractionForStaticTesting(el1.GetElementType(), el2.GetElementType());
            if (myElemInteraction != null)
            {
                myElemInteraction.RBElement1 = el1;
                myElemInteraction.RBElement2 = el2;
                return myElemInteraction.DoStaticInitialTest();
            }
            return false;
        }

        /// <summary>
        /// Registering the interation between 2 rbelement types
        /// </summary>
        public void RegisterRBElementInteraction(MyRBElementType type1, MyRBElementType type2, MyRBElementInteraction intr)
        {
            int t1 = (int)type1;
            int t2 = (int)type2;

            List<MyRBElementInteraction> intrList = null;
            if (t1 < t2)
            {
                intrList = m_IslandsPool[t1, t2];
                m_IslandsPoolStatic[t1, t2] = intr.CreateNewInstance();
            }
            else
            {
                intrList = m_IslandsPool[t2, t1];
                m_IslandsPoolStatic[t2, t1] = intr.CreateNewInstance(); 
            }

            intrList.Capacity = m_preAllocCount;
            intrList.Add(intr);

            for (int i = 1; i < m_preAllocCount; i++)
            {
                MyRBElementInteraction ins = intr.CreateNewInstance();
                intrList.Add(ins);
            }

        }

        /// <summary>
        /// Looks if interaction between those elements already exist
        /// </summary>
        public MyRBElementInteraction FindRBElementInteraction(MyRBElement el1, MyRBElement el2)
        {
            // look for interaction on element
            for (int i = 0; i < el1.GetRBElementInteractions().Count; i++)
            {
                MyRBElementInteraction intr = el1.GetRBElementInteractions()[i];
                if (intr.RBElement1 == el2 || intr.RBElement2 == el2)
                    return intr;
            }

            return null;
        }


        /// <summary>
        /// Adds interaction between 2 given elements
        /// </summary>
        public MyRBElementInteraction AddRBElementInteraction(MyRBElement el1, MyRBElement el2)
        {
            // get it
            int t1 = (int)el1.GetElementType();
            int t2 = (int)el2.GetElementType();
            List<MyRBElementInteraction> intrList = null;
            if (t1 < t2)
                intrList = m_IslandsPool[t1, t2];
            else
                intrList = m_IslandsPool[t2, t1];

            //pada to jinak
            if (intrList.Count == 0)
                return null; 

            MyCommonDebugUtils.AssertDebug(intrList.Count != 0);

            if (intrList.Count == 1)
            {
                MyRBElementInteraction ins = intrList[0].CreateNewInstance();
                intrList.Add(ins);
            }

            MyRBElementInteraction intr = intrList[intrList.Count - 1];
            intrList.RemoveAt(intrList.Count - 1);

            intr.RBElement1 = el1;
            intr.RBElement2 = el2;

            el1.GetRBElementInteractions().Add(intr);
            el2.GetRBElementInteractions().Add(intr);

            return intr;
        }

        /// <summary>
        /// Removes interaction between these 2 elements
        /// </summary>
        public void RemoveRBElementInteraction(MyRBElement el1, MyRBElement el2)
        {
            if (el1 != null)
            {
                // look for interaction on element
                for (int i = 0; i < el1.GetRBElementInteractions().Count; i++)
                {
                    MyRBElementInteraction intr = el1.GetRBElementInteractions()[i];
                    if ((intr.RBElement1 == el1 && intr.RBElement2 == el2) || (intr.RBElement1 == el2 && intr.RBElement2 == el1))
                    {
                        // add it back
                        int t1 = (int)el1.GetElementType();
                        int t2 = (int)el2.GetElementType();
                        List<MyRBElementInteraction> intrList = null;
                        if (t1 < t2)
                            intrList = m_IslandsPool[t1, t2];
                        else
                            intrList = m_IslandsPool[t2, t1];
                        intrList.Add(intr);

                        el1.GetRBElementInteractions().Remove(intr);
                        break;
                    }
                }
            }

            if (el2 != null)
            {
                for (int i = 0; i < el2.GetRBElementInteractions().Count; i++)
                {
                    MyRBElementInteraction intr = el2.GetRBElementInteractions()[i];
                    if ((intr.RBElement1 == el1 && intr.RBElement2 == el2) || (intr.RBElement1 == el2 && intr.RBElement2 == el1))
                    {
                        intr.RBElement1 = null;
                        intr.RBElement2 = null;

                        el2.GetRBElementInteractions().Remove(intr);
                        break;
                    }
                }
            }
        }

        private static List<MyRBElementInteraction>[,] m_IslandsPool = new List<MyRBElementInteraction>[(int)MyRBElementType.ET_LAST, (int)MyRBElementType.ET_LAST];
        private static MyRBElementInteraction[,] m_IslandsPoolStatic = new MyRBElementInteraction[(int)MyRBElementType.ET_LAST, (int)MyRBElementType.ET_LAST];
    }

}