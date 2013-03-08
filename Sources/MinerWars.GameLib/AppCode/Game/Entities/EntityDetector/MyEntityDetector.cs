using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.HUD;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Physics;
using MinerWars.AppCode.Game.Physics;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.CommonLIB.AppCode.Utils;
using SysUtils.Utils;
using MinerWars.AppCode.Game.Render;
using MinerWars.AppCode.Physics.Collisions;

namespace MinerWars.AppCode.Game.Entities.EntityDetector
{
    delegate void OnEntityEnter(MyEntityDetector sender, MyEntity entity, int meetCriterias);
    delegate void OnEntityLeave(MyEntityDetector sender, MyEntity entity);
    delegate void OnEntityPositionChange(MyEntityDetector sender, MyEntity entity, Vector3 newPosition);
    delegate void OnNearestEntityChange(MyEntityDetector sender, MyEntity oldNearestEntity, MyEntity newNearestEntity);
    delegate void OnNearestEntityCriteriasChange(MyEntityDetector sender, MyEntity entity, int meetCriterias);

    enum MyEntityDetectorType
    {
        Sphere,
        Box,
    }

    class MyEntityDetector : MyEntity, IMySensorEventHandler
    {
        #region constants

        #endregion

        #region members

        private bool m_isOn;
        private MySensor m_sensor;        
        private Dictionary<MyEntity, int> m_detectedEntities;
        private List<MyEntity> m_observableEntities;
        private MyEntity m_nearestEntity;
        private int m_nearestEntityCriterias;
        private MyEntity m_previousNearestEntity;
        private MyEntity m_tempNearestEntity;
        private int m_tempNearestEntityCriterias;
        private List<IMyEntityDetectorCriterium> m_detectionCriterias;        

        private int m_detectedEntitiesActualIndex;
        private int m_detectedEntitiesCount;
        private int m_detectedEntitiesCountPerFrame;

        private int m_observableEntitiesActualIndex;
        private int m_observableEntitiesCount;
        private int m_observableEntitiesCountPerFrame;

        private bool m_containsCriteriumToReCheck;

        private float m_radius;
        private Vector3 m_extent;
        private Vector3 m_size;

        private bool m_isClosed;

        private Action<MyEntity> m_onEntityMarkForClose;
        private Action<MyEntity> m_onEntityClose;
        private EventHandler m_onEntityPositionChange;

        private bool m_isSlowDetector;

        #endregion

        #region ctors

        public MyEntityDetector(bool isSlowDetector = false)
            : base()
        {                                    
            m_detectedEntities = new Dictionary<MyEntity, int>();
            m_observableEntities = new List<MyEntity>();
            m_sensor = new MySensor();
            m_detectionCriterias = new List<IMyEntityDetectorCriterium>();

            m_onEntityMarkForClose = new Action<MyEntity>(OnMarkForCloseHandler);
            m_onEntityClose = new Action<MyEntity>(OnCloseHandler);
            m_onEntityPositionChange = new EventHandler(OnDetectedEntityPositionChange);
            m_isSlowDetector = isSlowDetector;
            //InitCriterias(detectionCriterias);            
        }        

        #endregion

        #region events

        public event OnEntityEnter OnEntityEnter;

        public event OnEntityLeave OnEntityLeave;

        public event OnEntityPositionChange OnEntityPositionChange;

        public event OnNearestEntityChange OnNearestEntityChange;

        public event OnNearestEntityCriteriasChange OnNearestEntityCriteriasChange;

        #endregion

        #region properties

        public MyEntityDetectorType EntityDetectorType { get; private set; }

        public Vector3 Size 
        {
            get 
            {
                return m_size;
            }
            set 
            {
                m_size = value;

                bool isOn = m_isOn;
                if (isOn)
                {
                    Off();
                }

                MySensorElement sensorElement = m_sensor.GetElement();
                sensorElement.SetSize(m_size);

                if (isOn)
                {
                    On();
                }
            }
        }

        public float Radius
        {
            get
            {
                Debug.Assert(
                    ((MyMwcObjectBuilder_EntityDetector) GetObjectBuilder(true)).EntityDetectorType ==
                    MyMwcObjectBuilder_EntityDetector_TypesEnum.Sphere);
                return 0.5f * Size.X;
            }
            set
            {
                if (m_sensor != null && m_sensor.GetElement() is MySphereSensorElement)
                {
                    var sphereSensorElement = (MySphereSensorElement)m_sensor.GetElement();
                    sphereSensorElement.Radius = value;
                }
				m_radius = value;
            }
        }

        public int UpdateCounter
        {
            get;
            set;
        }

        #endregion

        #region initialization

        public void Init(string hudLabelText, MyMwcObjectBuilder_EntityDetector objectBuilder, MyEntity parent, Matrix matrix, IEnumerable<IMyEntityDetectorCriterium> detectionCriterias)
        {
            InitCriterias(detectionCriterias);
            InitSensor(objectBuilder.EntityDetectorType, objectBuilder.Size);

            StringBuilder hudLabelTextSb = (hudLabelText == null) ? null : new StringBuilder(hudLabelText);

            Init(hudLabelTextSb, null, null, parent, null, objectBuilder);

            SetWorldMatrix(matrix);                        

            Save = false;
            m_isClosed = false;
            Visible = false;
            //NeedsUpdate = true;
        }

        private void InitSensor(MyMwcObjectBuilder_EntityDetector_TypesEnum type, Vector3 size)
        {
            MySensorElement sensorElement;

            if (type == MyMwcObjectBuilder_EntityDetector_TypesEnum.Sphere)
            {
                MySphereSensorElement sphereSensorElement = new MySphereSensorElement();
                sphereSensorElement.Radius = size.Length() / 2f;
                sensorElement = sphereSensorElement;
                EntityDetectorType = MyEntityDetectorType.Sphere;
                m_radius = sphereSensorElement.Radius;
            }
            else if (type == MyMwcObjectBuilder_EntityDetector_TypesEnum.Box)
            {
                MyBoxSensorElement boxSensorElement = new MyBoxSensorElement();
                boxSensorElement.Size = size;
                sensorElement = boxSensorElement;
                EntityDetectorType = MyEntityDetectorType.Box;
                m_extent = size / 2f;
            }
            else
            {
                throw new MyMwcExceptionApplicationShouldNotGetHere();
            }

            sensorElement.LocalPosition = Vector3.Zero;
            MySensorDesc senDesc = new MySensorDesc();
            senDesc.m_Element = sensorElement;
            senDesc.m_Matrix = Matrix.Identity;
            senDesc.m_SensorEventHandler = this;
            m_sensor.Inserted = false;
            m_sensor.LoadFromDesc(senDesc);
            m_sensor.Active = true;
            m_size = size;           
        }

        protected override void SetHudMarker()
        {            
        }

        private void InitCriterias(IEnumerable<IMyEntityDetectorCriterium> detectionCriterias)
        {
            Debug.Assert(detectionCriterias != null);

            m_detectionCriterias.AddRange(detectionCriterias);
            m_containsCriteriumToReCheck = false;

            foreach (IMyEntityDetectorCriterium criterium in m_detectionCriterias)
            {                
                if (criterium.ReCheckCriterium)
                {
                    m_containsCriteriumToReCheck = true;                    
                }
            }            
        }

        #endregion

        #region public methods

        public void On()
        {
            //Debug.Assert(!m_isOn);
            if (!m_isOn)
            {
                m_isOn = true;
                AddSensor();
                NeedsUpdate = true;
                if (m_isSlowDetector) 
                {
                    MyEntityDetectorsManager.RegisterSlowDetectorForUpdate(this);
                }
            }
        }

        public void Off()
        {
            //Debug.Assert(m_isOn);
            SetOff(true);
        }

        public bool IsOn()
        {
            return m_isOn;
        }

        public void Reset()
        {
            if (m_isOn)
            {
                Off();
                On();
            }
        }

        public bool TrySetStatus(bool setOn) 
        {
            bool changed = false;
            if (setOn && !m_isOn) 
            {
                On();
                changed = true;
            }
            else if (!setOn && m_isOn) 
            {
                Off();
                changed = true;
            }
            return changed;
        }

        public void ActivateSensor() 
        {
            if (!m_sensor.Active)
            {
                m_sensor.Active = true;
            }
        }

        public MyEntity GetNearestEntity()
        {
            return m_nearestEntity;
        }

        public bool IsEntityDetected(MyEntity entity, out int? meetCriterias)
        {            
            meetCriterias = GetCriteriasForDetectedEntity(entity);
            return meetCriterias != null;
        }

        public int? GetCriteriasForDetectedEntity(MyEntity entity)
        {
            Debug.Assert(entity != null);

            if(m_detectedEntities.Count == 0)
            {
                return null;
            }

            int meetCriterias;
            m_detectedEntities.TryGetValue(entity, out meetCriterias);
            return meetCriterias;
        }

        public int? GetNearestEntityCriterias()
        {
            if(m_nearestEntity == null)
            {
                return null;
            }
            else
            {
                return m_nearestEntityCriterias;
            }
        }

        public Dictionary<MyEntity, int> GetDetectedEntities()
        {
            return m_detectedEntities;
        }

        public void SetSensorDetectRigidBodyTypes(uint? rigidBodyType) 
        {
            if (m_sensor != null && m_sensor.GetElement() != null) 
            {
                m_sensor.GetElement().DetectRigidBodyTypes = rigidBodyType;
            }
        }
        #endregion

        #region private methods

        private void SetOff(bool callEvents) 
        {
            if (m_isOn)
            {
                m_isOn = false;
                RemoveSensor();
                ClearDetectedEntities(callEvents);
                NeedsUpdate = false;
                if (m_isSlowDetector)
                {
                    MyEntityDetectorsManager.UnregisterSlowDetectorForUpdate(this);
                }
            }
        }

        private void OnDetectedEntityPositionChange(object sender, EventArgs args)
        {            
            CallOnEntityPositionChange(sender as MyEntity);
        }

        private void OnMarkForCloseHandler(MyEntity entity)
        {
            RemoveEntityFromDetectedAndObservable(entity);
        }

        private void OnCloseHandler(MyEntity entity)
        {
            RemoveEntityFromDetectedAndObservable(entity);
        }

        private void RemoveEntityFromDetectedAndObservable(MyEntity entity) 
        {
            if (m_containsCriteriumToReCheck)
            {
                m_observableEntities.Remove(entity);
            }
            RemoveDetectedEntity(entity);
            UnregisterOnCloseHandlers(entity);
        }

        private void RegisterOnCloseHandlers(MyEntity entity) 
        {
            entity.OnMarkForClose += m_onEntityMarkForClose;
            entity.OnClose += m_onEntityClose;
        }

        private void UnregisterOnCloseHandlers(MyEntity entity) 
        {
            entity.OnMarkForClose -= m_onEntityMarkForClose;
            entity.OnClose -= m_onEntityClose;
        }

        private void AddDetectedEntity(MyEntity entity, int criterias)
        {
            Debug.Assert(!m_detectedEntities.ContainsKey(entity), "This should not happen!");
            if (!m_detectedEntities.ContainsKey(entity))
            {
                entity.OnPositionChanged += m_onEntityPositionChange;                
                m_detectedEntities.Add(entity, criterias);

                CallOnEntityEnter(entity, criterias);
            }
        }

        private void RemoveDetectedEntity(MyEntity entity, bool callEvents = true)
        {            
            bool removed = m_detectedEntities.Remove(entity);

            if (removed)
            {
                entity.OnPositionChanged -= m_onEntityPositionChange;
                if (callEvents)
                {
                    CallOnEntityLeave(entity);
                }
            }
        }

        private void CallOnEntityEnter(MyEntity entity, int criterias)
        {
            if(OnEntityEnter != null)
            {
                OnEntityEnter(this, entity, criterias);
            }
        }

        private void CallOnEntityLeave(MyEntity entity)
        {
            if(OnEntityLeave != null)
            {
                OnEntityLeave(this, entity);
            }
        }

        private void CallOnNearestEntityChange(MyEntity oldEntity, MyEntity newEntity)
        {
            if(OnNearestEntityChange != null)
            {
                OnNearestEntityChange(this, oldEntity, newEntity);
            }
        }

        private void CallOnNearestEntityCriteriasChange(MyEntity entity, int meetCriterias)         
        {
            if (OnNearestEntityCriteriasChange != null) 
            {
                OnNearestEntityCriteriasChange(this, entity, meetCriterias);
            }
        }

        private void CallOnEntityPositionChange(MyEntity entity)
        {
            if(OnEntityPositionChange != null)
            {
                OnEntityPositionChange(this, entity, entity.GetPosition());
            }
        }

        private bool IsEntityMeetCritarias(MyEntity entity, ref int meetCriterias, bool reCheck = false)
        {            
            if(m_detectionCriterias.Count == 0)
            {
                return true;
            }

            foreach(IMyEntityDetectorCriterium criterium in m_detectionCriterias)
            {
                if (reCheck && !criterium.ReCheckCriterium)
                {
                    continue;                    
                }

                if (criterium.Check(entity))
                {
                    if ((meetCriterias & criterium.Key) == 0)
                    {
                        meetCriterias |= criterium.Key;
                    }
                }
                else
                {
                    if ((meetCriterias & criterium.Key) != 0)
                    {
                        meetCriterias &= ~criterium.Key;
                    }
                }
            }
            return meetCriterias > 0;
        }        

        private bool CanBeEntityObserved(MyEntity entity)
        {
            if (m_detectionCriterias.Count == 0)
            {
                return true;
            }

            foreach (IMyEntityDetectorCriterium criterium in m_detectionCriterias)
            {
                if (criterium.IsEntityInRightType(entity))
                {
                    return true;
                }
            }
            return false;
        }

        private float GetEntityDistance(MyEntity entity)
        {
            return Vector3.Distance(GetPosition(), entity.GetPosition());
        }

        private void UpdateDetectedEntities()
        {
            m_detectedEntitiesCount = m_detectedEntities.Count;
            m_detectedEntitiesCountPerFrame = (int)Math.Ceiling(m_detectedEntitiesCount / MyConstants.PHYSICS_STEPS_PER_SECOND);

            if (m_detectedEntitiesActualIndex == 0)
            {
                m_tempNearestEntity = null;
                m_previousNearestEntity = m_nearestEntity;
            }

            int recaltulateTo = Math.Min(m_detectedEntitiesActualIndex + m_detectedEntitiesCountPerFrame, m_detectedEntitiesCount);
            while (m_detectedEntitiesActualIndex < recaltulateTo)
            {
                KeyValuePair<MyEntity, int> entityKvp = m_detectedEntities.ElementAt(m_detectedEntitiesActualIndex);
                MyEntity entity = entityKvp.Key;
                int meetCriterias = entityKvp.Value;

                if(m_containsCriteriumToReCheck)
                {                    
                    if(!IsEntityMeetCritarias(entity, ref meetCriterias, true))
                    {
                        RemoveDetectedEntity(entity);
                        m_observableEntities.Add(entity);
                        m_detectedEntitiesCount = m_detectedEntities.Count;
                        recaltulateTo = Math.Min(recaltulateTo, m_detectedEntitiesCount);
                        continue;
                    }
                }

                if (m_tempNearestEntity == null || GetEntityDistance(entity) < GetEntityDistance(m_tempNearestEntity))
                {
                    m_tempNearestEntity = entity;
                    m_tempNearestEntityCriterias = meetCriterias;
                }
                m_detectedEntitiesActualIndex++;
            }

            if (m_detectedEntitiesActualIndex >= m_detectedEntitiesCount)
            {
                m_nearestEntity = m_tempNearestEntity;
                int previousNearestEntityCriterias = m_nearestEntityCriterias;
                m_nearestEntityCriterias = m_tempNearestEntityCriterias;

                if (m_nearestEntity != m_previousNearestEntity)
                {
                    CallOnNearestEntityChange(m_previousNearestEntity, m_nearestEntity);
                }
                else if (m_nearestEntityCriterias != previousNearestEntityCriterias) 
                {
                    CallOnNearestEntityCriteriasChange(m_nearestEntity, m_nearestEntityCriterias);
                }

                m_detectedEntitiesActualIndex = 0;
            }
        }

        private void UpdateObservableEntities()
        {
            m_observableEntitiesCount = m_observableEntities.Count;
            m_observableEntitiesCountPerFrame = (int)Math.Ceiling(m_observableEntitiesCount / MyConstants.PHYSICS_STEPS_PER_SECOND);            

            int recaltulateTo = Math.Min(m_observableEntitiesActualIndex + m_observableEntitiesCountPerFrame, m_observableEntitiesCount);
            while (m_observableEntitiesActualIndex < recaltulateTo)
            {
                MyEntity entity = m_observableEntities[m_observableEntitiesActualIndex];

                int meetCriterias = 0;
                if (IsEntityMeetCritarias(entity, ref meetCriterias, true))
                {
                    m_observableEntities.RemoveAt(m_observableEntitiesActualIndex);
                    AddDetectedEntity(entity, meetCriterias);
                    m_observableEntitiesCount = m_observableEntities.Count;
                    recaltulateTo = Math.Min(recaltulateTo, m_observableEntitiesCount);
                    continue;
                }
                m_observableEntitiesActualIndex++;
            }

            if (m_observableEntitiesActualIndex >= m_observableEntitiesCount)
            {
                m_observableEntitiesActualIndex = 0;
            }
        }

        private void ClearDetectedEntities(bool callEvents)
        {            
            while (m_detectedEntities.Count > 0)
            {
                MyEntity entityToRemove = m_detectedEntities.ElementAt(0).Key;
                RemoveDetectedEntity(entityToRemove, callEvents);
                UnregisterOnCloseHandlers(entityToRemove);
            }
            m_detectedEntities.Clear();
            m_observableEntities.Clear();

            if (callEvents && m_nearestEntity != null)
            {
                CallOnNearestEntityChange(m_nearestEntity, null);
            }            

            m_nearestEntity = null;
            m_tempNearestEntity = null;
            m_previousNearestEntity = null;
            m_detectedEntitiesActualIndex = 0;
            m_observableEntitiesActualIndex = 0;            
        }

        private void AddSensor()
        {
            //m_sensor.GetElement().ProxyData = MyElement.PROXY_UNASSIGNED;
            //m_sensor.m_Interactions.Clear();
            MyPhysics.physicsSystem.GetSensorModule().AddSensor(m_sensor);
        }

        private void RemoveSensor()
        {
            //m_sensor.GetElement().ProxyData = MyElement.PROXY_UNASSIGNED;
            MyPhysics.physicsSystem.GetSensorModule().RemoveSensor(m_sensor);
        }

        #endregion

        #region overriden members

        public override void UpdateBeforeSimulation()
        {

            if (m_isClosed) 
            {
                Debug.Fail("This shoudln't happen!");
                return;
            }
            //base.UpdateBeforeSimulation();            

            if (m_isOn)
            {
                if (m_isSlowDetector && !MyEntityDetectorsManager.CanBeSlowDetectorUpdated(this))
                {
                    return;
                }
                if (m_containsCriteriumToReCheck)
                {
                    UpdateObservableEntities();
                }
                UpdateDetectedEntities();
            }

        }

        public override bool Draw(MyRenderObject renderObject)
        {
        //    return base.Draw();

//            if (IsOn())
            {
                Matrix world = WorldMatrix;
                Vector4 color = new Vector4(0f, 0f, 1f, 0.5f);

                if (EntityDetectorType == MyEntityDetectorType.Sphere)
                {
                    MySimpleObjectDraw.DrawTransparentSphere(ref world, m_radius, ref color, true, 1);
                }
                else if (EntityDetectorType == MyEntityDetectorType.Box)
                {
                    BoundingBox bBox = new BoundingBox(-m_extent, m_extent);
                    MySimpleObjectDraw.DrawTransparentBox(ref world, ref bBox, ref color, true, 1);
                }
            }
            return true;
        }

        public override void Close()
        {
            Debug.Assert(!m_isClosed);
            m_sensor.MarkForClose();
            SetOff(false);
            //ClearDetectedEntities(false);            
            //RemoveSensor();
            //m_isOn = false;
            m_detectionCriterias.Clear();
            m_sensor = null;
            OnEntityEnter = null;
            OnEntityLeave = null;
            OnEntityPositionChange = null;
            base.Close();
            m_isClosed = true;            
        }

        public override void OnWorldPositionChanged(object source)
        {
            base.OnWorldPositionChanged(source);
            m_sensor.Matrix = WorldMatrix;
        }

        public override bool GetIntersectionWithLine(ref MyLine line, out Vector3? v, bool useCollisionModel = true, IntersectionFlags flags = IntersectionFlags.ALL_TRIANGLES)
        {
            v = null;
            Ray ray = new Ray(line.From, line.Direction);
            float? ds = null;
            if (EntityDetectorType == MyEntityDetectorType.Box)
            {
                ds = ray.Intersects(this.WorldAABB);
            }
            else if (EntityDetectorType == MyEntityDetectorType.Sphere)
            {
                ds = ray.Intersects(new BoundingSphere(WorldMatrix.Translation, m_radius));
            }

            if (ds == null)
                return false;
            v = line.From + line.Direction * ds;
            return true;
        }

        public override bool GetIntersectionWithSphere(ref BoundingSphere sphere)
        {
            if (EntityDetectorType == MyEntityDetectorType.Box)
            {
                BoundingBox boundingBox = new BoundingBox(WorldMatrix.Translation - m_extent, WorldMatrix.Translation + m_extent);
                return boundingBox.Intersects(sphere);
            }
            else if (EntityDetectorType == MyEntityDetectorType.Sphere)
            {
                BoundingSphere boundingSphere = new BoundingSphere(WorldMatrix.Translation, m_radius);
                return boundingSphere.Intersects(sphere);
            }

            return false;
        }

        public void OnEnter(MySensor sensor, MyRigidBody rbo, MyRBElement rbElement)
        {
            if(m_isOn && rbo.m_UserData != null)
            {
                MyEntity entity = (rbo.m_UserData as MyPhysicsBody).Entity;
                if(entity != null && (Parent == null || Parent != entity))
                {
                    int meetCriterias = 0;
                    bool canRegisteForClose = false;
                    if (IsEntityMeetCritarias(entity, ref meetCriterias))
                    {
                        AddDetectedEntity(entity, meetCriterias);
                        canRegisteForClose = true;
                    }
                    else
                    {
                        if (m_containsCriteriumToReCheck)
                        {
                            if (CanBeEntityObserved(entity))
                            {
                                m_observableEntities.Add(entity);
                                canRegisteForClose = true;
                            }
                        }
                    }
                    if (canRegisteForClose) 
                    {
                        RegisterOnCloseHandlers(entity);
                    }
                }
            }
        }

        public void OnLeave(MySensor sensor, MyRigidBody rbo, MyRBElement rbElement)
        {
            if (rbo == null)
                return;

            if (m_isOn && rbo.m_UserData != null)
            {
                MyEntity entity = (rbo.m_UserData as MyPhysicsBody).Entity;
                if (entity != null && (Parent == null || Parent != entity))
                {
                    RemoveEntityFromDetectedAndObservable(entity);
                }
            }
        }

        #endregion        
    }
}
