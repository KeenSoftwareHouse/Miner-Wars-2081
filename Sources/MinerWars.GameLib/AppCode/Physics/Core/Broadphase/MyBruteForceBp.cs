#region Using Statements

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MinerWars.CommonLIB.AppCode.Utils;
using SysUtils.Utils;

#endregion

namespace MinerWars.AppCode.Physics
{    
    //////////////////////////////////////////////////////////////////////////

    class MyBruteForceBroadphase: MyBroadphase
    {
        public MyBruteForceBroadphase()
        {
            m_Elements = new List<MyRBElement>(256);
            m_ActiveElements = new List<MyRBElement>(256);
            m_InteractionList   = new List<MyRBElementInteraction>(512);
        }
        public override void CreateVolume(MyElement element)
        { 
#if PHYSICS_CHECK
            for (int i = 0; i < m_Elements.Count; i++)
            {
                MyRBElement el = m_Elements[i];
                if (el == element)
                {
                    // inserting twice!!!
                    CommonDebugUtils.AssertRelease(false);
                    return;
                }
            }
#endif
            if ((element.Flags & MyElementFlag.EF_SENSOR_ELEMENT) > 0)
            {
                return;
            }
            MyRBElement elm = (MyRBElement)element;
            m_Elements.Add(elm);
        }
        public override void DestroyVolume(MyElement element)
        {
            if ((element.Flags & MyElementFlag.EF_SENSOR_ELEMENT) > 0)
            {
                return;
            }
            MyRBElement elm = (MyRBElement)element;

            elm.GetRBElementInteractions().Clear();
            m_Elements.Remove(elm);
        }

        public override void DoWork()
        {
            // brute force
            MyRBInteractionModule module = MyPhysics.physicsSystem.GetRBInteractionModule();
            List <MyRigidBody> activeRigids = MyPhysics.physicsSystem.GetRigidBodyModule().GetActiveRigids();

            m_ActiveElements.Clear();

            for (int i = 0; i < activeRigids.Count; i++)
            {
                MyRigidBody rbo = activeRigids[i];
                for (int j = 0; j < rbo.GetRBElementList().Count; j++)
                {
                    MyRBElement el = rbo.GetRBElementList()[j];
                    el.UpdateAABB();
                    m_ActiveElements.Add(el);
                }
            }

            // parse the elements
            BoundingBox bbox;
            MyRBElementInteraction interaction = null;
            m_InteractionList.Clear();
            for (int i = 0; i < m_ActiveElements.Count; i++)
            {
                MyRBElement testEl = m_ActiveElements[i];
                BoundingBox testAABB = testEl.GetWorldSpaceAABB();                
                for (int j = 0; j < m_Elements.Count;j++)
                {
                    MyRBElement el = m_Elements[j];
                    interaction = null;
                    if (el != testEl)
                    {
                        if(el.GetRigidBody().IsStatic() && testEl.GetRigidBody().IsStatic())
                            continue;

                        if (el.GetRigidBody().IsKinematic() && testEl.GetRigidBody().IsKinematic())
                            continue;

                        if(el.GetRigidBody() == testEl.GetRigidBody())
                            continue;

                        bbox = el.GetWorldSpaceAABB();
                        if (bbox.Intersects(testAABB))
                        {      
                            interaction = module.FindRBElementInteraction(el, testEl);
                            if (interaction == null)
                            {
                                interaction = module.AddRBElementInteraction(el, testEl);
                            }
                        }
                        else
                        {
                            interaction = module.FindRBElementInteraction(el, testEl);
                            if (interaction != null)
                            {
                                interaction = null;
                                module.RemoveRBElementInteraction(el,testEl);
                            }
                        }

                        if (interaction != null)
                        {
                            bool iinserted = false;
                            for (int t = 0; t < m_InteractionList.Count; t++)
                            {
                                if (m_InteractionList[t] == interaction)
                                {
                                    iinserted = true;
                                    break;
                                }
                            }
                            if(!iinserted)
                                m_InteractionList.Add(interaction);
                        }
                    }
                }
            }
        }

        public override List<MyRBElementInteraction> GetRBElementInteractionList() { return m_InteractionList; }

        private List <MyRBElement>          m_Elements;
        private List<MyRBElement>           m_ActiveElements;
        private List<MyRBElementInteraction>    m_InteractionList;

    }
}