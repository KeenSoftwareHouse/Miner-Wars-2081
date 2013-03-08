#region Using Statements

using MinerWars.CommonLIB.AppCode.Generics;
using SysUtils.Utils;
#endregion

namespace MinerWars.AppCode.Physics
{
    // for object pools and generation
    //////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Objects class as a container for preallocated physics objects - rigids, descs, elements etc
    /// Base class for creation of rigids and elements. 
    /// </summary>
    class MyPhysicsObjects
    {
        public MyPhysicsObjects()
        {
            m_RigidBodyDesc = new MyRigidBodyDesc();
            m_SensorDesc = new MySensorDesc();
            m_RBSphereElementDesc = new MyRBSphereElementDesc();
            m_RBBoxElementDesc = new MyRBBoxElementDesc();
            m_RBCapsuleElementDesc = new MyRBCapsuleElementDesc();
            m_RBTriangleMeshDesc = new MyRBTriangleMeshElementDesc();
            m_RBVoxelElementDesc = new MyRBVoxelElementDesc();

            m_SensorsPool = new MyObjectsPool<MySensor>(128);
            m_SphereSensorElementPool = new MyObjectsPool<MySphereSensorElement>(128);

            m_RigidsPool = new MyObjectsPool<MyRigidBody>(24576);
            m_RBSphereElementPool = new MyObjectsPool<MyRBSphereElement>(16384);
            m_RBBoxElementPool = new MyObjectsPool<MyRBBoxElement>(16384);
            m_RBCapsuleElementPool = new MyObjectsPool<MyRBCapsuleElement>(16);
            m_RBTriangleMeshElementPool = new MyObjectsPool<MyRBTriangleMeshElement>(16384);//value from Ales - 64
            m_RBVoxelElementPool = new MyObjectsPool<MyRBVoxelElement>(512);
        }

        public MyRigidBodyDesc GetRigidBodyDesc() { return m_RigidBodyDesc; }
        public MyRBSphereElementDesc GetRBSphereElementDesc() { return m_RBSphereElementDesc; }
        public MyRBBoxElementDesc GetRBBoxElementDesc() { return m_RBBoxElementDesc; }
        public MyRBCapsuleElementDesc GetRBCapsuleElementDesc() { return m_RBCapsuleElementDesc; }
        public MyRBTriangleMeshElementDesc GetRBTriangleMeshElementDesc() { return m_RBTriangleMeshDesc; }
        public MyRBVoxelElementDesc GetRBVoxelElementDesc() { return m_RBVoxelElementDesc; }        

        public MyRigidBody CreateRigidBody(MyRigidBodyDesc desc)
        {
            if (!desc.IsValid())
            {
                // invalid desc
                MyCommonDebugUtils.AssertDebug(false);
                return null;
            }

            MyRigidBody rbo = m_RigidsPool.Allocate();

            MyCommonDebugUtils.AssertDebug(rbo != null);

            rbo.LoadFromDesc(desc);

            return rbo;
        }

        public MySensor CreateSensor(MySensorDesc desc)
        {
            if (!desc.IsValid())
            {
                // invalid desc
                MyCommonDebugUtils.AssertDebug(false);
                return null;
            }

            MySensor sensor = m_SensorsPool.Allocate();

            MyCommonDebugUtils.AssertDebug(sensor != null);

            sensor.LoadFromDesc(desc);

            return sensor;
        }

        public void DestroyRigidBody(MyRigidBody rbo)
        {
            if ((rbo.ReadFlag(RigidBodyFlag.RBF_INSERTED)))
            {
                MyPhysics.physicsSystem.GetRigidBodyModule().Remove(rbo);
            }

            m_RigidsPool.Deallocate(rbo);
        }

        public void DestroySensor(MySensor sensor)
        {
            if(sensor.Inserted)
            {
                MyPhysics.physicsSystem.GetSensorModule().RemoveSensor(sensor);
            }

            m_SensorsPool.Deallocate(sensor);
        }

        public MySensorElement CreateSensorElement(MySensorElementDesc desc)
        {
            switch(desc.GetElementType())
            {
                case MySensorElementType.ET_SPHERE:
                    {
                        MySphereSensorElement element = m_SphereSensorElementPool.Allocate();

                        MyCommonDebugUtils.AssertDebug(element != null);

                        if (element.LoadFromDesc(desc))
                            return element;
                        else
                        {
                            m_SphereSensorElementPool.Deallocate(element);
                            return null;
                        }
                    }
                    break;
                default:
                    return null;
                    break;
            }
        }

        public MyRBElement CreateRBElement(MyRBElementDesc desc)
        {
            switch (desc.GetElementType())
            {
                case MyRBElementType.ET_SPHERE:
                    {
                        MyRBSphereElement element = m_RBSphereElementPool.Allocate();

                        MyCommonDebugUtils.AssertDebug(element != null);

                        if (element.LoadFromDesc(desc))
                            return element;
                        else
                        {
                            m_RBSphereElementPool.Deallocate(element);
                            return null;
                        }
                    }
                    break;
                case MyRBElementType.ET_BOX:
                    {
                        MyRBBoxElement element = m_RBBoxElementPool.Allocate();

                        MyCommonDebugUtils.AssertDebug(element != null);

                        if (element.LoadFromDesc(desc))
                            return element;
                        else
                        {
                            m_RBBoxElementPool.Deallocate(element);
                            return null;
                        }
                    }
                    break;
                case MyRBElementType.ET_CAPSULE:
                    {
                        MyRBCapsuleElement element = m_RBCapsuleElementPool.Allocate();

                        MyCommonDebugUtils.AssertDebug(element != null);

                        if (element.LoadFromDesc(desc))
                            return element;
                        else
                        {
                            m_RBCapsuleElementPool.Deallocate(element);
                            return null;
                        }
                    }
                    break;
                case MyRBElementType.ET_TRIANGLEMESH:
                    {
                        MyRBTriangleMeshElement element = m_RBTriangleMeshElementPool.Allocate();

                        MyCommonDebugUtils.AssertDebug(element != null);

                        if (element.LoadFromDesc(desc))
                            return element;
                        else
                        {
                            m_RBTriangleMeshElementPool.Deallocate(element);
                            return null;
                        }
                    }
                    break;
                case MyRBElementType.ET_VOXEL:
                    {
                        MyRBVoxelElement element = m_RBVoxelElementPool.Allocate();

                        MyCommonDebugUtils.AssertDebug(element != null);

                        if (element.LoadFromDesc(desc))
                            return element;
                        else
                        {
                            m_RBVoxelElementPool.Deallocate(element);
                            return null;
                        }
                    }
                    break;
                default:
                    // unknown element type
                    MyCommonDebugUtils.AssertDebug(false);
                    break;
            }
            return null;
        }


        public void RemoveRBElement(MyRBElement element)
        {
            switch (element.GetElementType())
            {
                case MyRBElementType.ET_SPHERE:
                    {
                        m_RBSphereElementPool.Deallocate((MyRBSphereElement)element);
                    }
                    break;
                case MyRBElementType.ET_BOX:
                    {
                        m_RBBoxElementPool.Deallocate((MyRBBoxElement)element);
                    }
                    break;
                case MyRBElementType.ET_CAPSULE:
                    {
                        m_RBCapsuleElementPool.Deallocate((MyRBCapsuleElement)element);
                    }
                    break;
                case MyRBElementType.ET_TRIANGLEMESH:
                    {
                        m_RBTriangleMeshElementPool.Deallocate((MyRBTriangleMeshElement)element);
                    }
                    break;
                case MyRBElementType.ET_VOXEL:
                    {
                        m_RBVoxelElementPool.Deallocate((MyRBVoxelElement)element);
                    }
                    break;
                default:
                    // unknown element type
                    MyCommonDebugUtils.AssertDebug(false);
                    break;
            }
        }

        private MyRigidBodyDesc m_RigidBodyDesc;
        private MyRBSphereElementDesc m_RBSphereElementDesc;
        private MyRBBoxElementDesc m_RBBoxElementDesc;
        private MyRBCapsuleElementDesc m_RBCapsuleElementDesc;
        private MyRBTriangleMeshElementDesc m_RBTriangleMeshDesc;
        private MyRBVoxelElementDesc m_RBVoxelElementDesc;
        private MySensorDesc m_SensorDesc;

        private MyObjectsPool<MySensor> m_SensorsPool;
        private MyObjectsPool<MySphereSensorElement> m_SphereSensorElementPool;

        private MyObjectsPool<MyRigidBody> m_RigidsPool;
        private MyObjectsPool<MyRBSphereElement> m_RBSphereElementPool;
        private MyObjectsPool<MyRBBoxElement> m_RBBoxElementPool;
        private MyObjectsPool<MyRBCapsuleElement> m_RBCapsuleElementPool;
        private MyObjectsPool<MyRBTriangleMeshElement> m_RBTriangleMeshElementPool;
        private MyObjectsPool<MyRBVoxelElement> m_RBVoxelElementPool;
    }
}