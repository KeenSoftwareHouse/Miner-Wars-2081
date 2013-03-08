using System.Diagnostics;
using MinerWars.AppCode.Game.HUD;
using MinerWars.AppCode.Game.Inventory;
using MinerWars.AppCode.Game.Entities.FoundationFactory;
using MinerWars.AppCode.Physics;
using MinerWars.AppCode.Game.Physics;

namespace MinerWars.AppCode.Game.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using CommonLIB.AppCode.ObjectBuilders;
    using CommonLIB.AppCode.ObjectBuilders.Object3D;
    using CommonLIB.AppCode.ObjectBuilders.SubObjects;
    using CommonLIB.AppCode.Utils;
    using Editor;
    using MinerWarsMath;
    using TransparentGeometry;
    using Prefabs;
    using SubObjects;
    using Utils;
    using MinerWars.AppCode.Game.Prefabs;
    using MinerWars.AppCode.Game.GUI.Core;
    using MinerWars.AppCode.App;
    using MinerWars.AppCode.Game.Managers.Session;
    using MinerWars.AppCode.Game.Audio;
    using MinerWars.AppCode.Game.GUI.Prefabs;
    using MinerWars.AppCode.Game.World.Global;
    using MinerWars.AppCode.Game.Missions;
    using MinerWars.AppCode.Game.Entities.Ships.AI;
    using MinerWars.AppCode.Game.Render;
    using MinerWars.AppCode.Game.GUI.Helpers;
    using MinerWars.AppCode.Physics.Collisions;

    delegate void OnPrefabContainerInitialized(MyPrefabContainer sender);
    delegate void OnAlarmLaunchned(object sender, MyEntity entity);

    /// <summary>
    /// This phys object represents Prefab Container(it is like one big object created from many prefabs(lego components).
    /// When interacting with prefab container in gameplay, it behaves as one object, but due to the fact, that its sub-components(prefabs)
    /// can have some additional functionality(for example some prefabs are destructible, thus they can be destroyed during gameplay, or
    /// some prefabs might be moving/closing/opening), it is necessary to keep track of all prefabs during gameplay. For that reason, container
    /// keeps 2 lists of prefabs, one for commited prefabs, and second for working prefabs. Working prefabs are available when editing container and
    /// are not visible to the outside-of-container world. Commited prefabs are visible to the outside-of-container worlds, 
    /// </summary>
    class MyPrefabContainer : MyEntity, IMyInventory, IMyUseableEntity, IMyHasGuiControl
    {
        //List<MyEntity> m_commitedPrefabs;
        //List<MyPrefab> m_workingPrefabs;
        List<MyPrefabBase> m_prefabs;
        List<MyPrefabBase> m_deactivatedPrefabs;

        private bool m_editingActive; // indicates wheater we are inside editing container, or not

        private int m_userID;

        private BoundingBox m_selectionBox;  // this is a small box inside container, that is used as a selection control from editor
        //MyRBBoxElement m_selectionBox;
        public bool HighlightSelectionBox;
        int m_lastCommitInMillis;

        public MyRigidBody DynamicPrefabsRigidBody;

        bool m_needUpdateAABB = false;

        //@ key - entity, value - modification type //TODO not integrated listen on prefab moving/modifying
        private Dictionary<MyPrefabBase, short> m_Modifications = new Dictionary<MyPrefabBase, short>();

        public void SetEditState(bool editState)
        {
            m_editingActive = editState;
        }
        public bool HasEditState() { return m_editingActive; }

        public event OnPrefabContainerInitialized OnPrefabContainerInitialized;

        /// <summary>
        /// Prefab container's inventory
        /// </summary>
        public MyInventory Inventory { get; set; }

        private PrefabTypesFlagEnum m_prefabTypesFlag;
        private int[] m_prefabTypeSingleFlagsCount;

        private List<MyPrefabBase>[] m_prefabsByCategory;

        private bool m_intializing;

        public const int DEFAULT_REFILL_TIME_IN_SEC = 5 * 60;
        private int m_lastRefillTime;
        private int? m_refillTime;
        public int? RefillTime 
        {
            get { return m_refillTime; }
            set 
            {
                m_refillTime = value;
                RecheckNeedsUpdateState();
            }
        }

        // initialize container
        public MyPrefabContainer()
        {
            m_prefabs = new List<MyPrefabBase>(MyPrefabContainerConstants.MAX_PREFABS_IN_CONTAINER+1);
            m_deactivatedPrefabs = new List<MyPrefabBase>(MyPrefabContainerConstants.MAX_PREFABS_IN_CONTAINER + 1);

            Inventory = new MyInventory(1000);
            
            m_prefabTypeSingleFlagsCount = new int[MyMwcUtils.GetMaxValueFromEnum<PrefabTypesFlagEnum>() + 1];

            m_prefabsByCategory = new List<MyPrefabBase>[MyMwcUtils.GetMaxValueFromEnum<CategoryTypesEnum>() + 1];
            foreach (ushort categoryTypeId in Enum.GetValues(typeof(CategoryTypesEnum)))
            {
                m_prefabsByCategory[categoryTypeId] = new List<MyPrefabBase>();
            }
        }

        public virtual void Init(string hudLabelText, MyMwcObjectBuilder_PrefabContainer objectBuilder, Matrix matrix)
        {
            m_intializing = true;
            StringBuilder hudLabelTextSb = (string.IsNullOrEmpty(hudLabelText) ? null : new StringBuilder(hudLabelText));

            base.Init(hudLabelTextSb, null, null, null, null, objectBuilder);

            DisplayName = objectBuilder.DisplayName;            

            SetWorldMatrix(matrix);            

            Flags |= EntityFlags.EditableInEditor;

            this.Faction = objectBuilder.Faction;

            //during container initialization, it is not necessary to check if prefab is outside boundaries, because it cannot be saved that way
            foreach (MyMwcObjectBuilder_PrefabBase prefabBuilder in objectBuilder.Prefabs)
            {
                CreateAndAddPrefab(null, prefabBuilder);
            }

            // we must initialize inventory after prefabs, because some prefabs are registered on OnInventoryContentChanged event
            if (objectBuilder.Inventory != null)
            {
                Inventory.Init(objectBuilder.Inventory, MyMwcUtils.GetRandomFloat(1.1f, 2f));                
            }

            //Commit();

            UpdateAABBHr();

            // ----- THIS PHYSICS IS NEEDED BECAUSE ENTITY DETECTOR -----                        
            this.Physics = new MyPhysicsBody(this, 1.0f, UseKinematicPhysics ? RigidBodyFlag.RBF_KINEMATIC : RigidBodyFlag.RBF_RBO_STATIC) { MaterialType = MyMaterialType.METAL };
            MyPhysicsObjects physobj = MyPhysics.physicsSystem.GetPhysicsObjects();
            MyRBBoxElementDesc boxDesc = physobj.GetRBBoxElementDesc();
            boxDesc.SetToDefault();
            boxDesc.m_RBMaterial = MyMaterialsConstants.GetMaterialProperties(MyMaterialType.METAL).PhysicsMaterial;
            boxDesc.m_Size = UseKinematicPhysics ? WorldAABBHr.Size() : Vector3.One;
            boxDesc.m_CollisionLayer = UseKinematicPhysics ? MyConstants.COLLISION_LAYER_DEFAULT : MyConstants.COLLISION_LAYER_UNCOLLIDABLE;
            MyRBBoxElement boxEl = (MyRBBoxElement)physobj.CreateRBElement(boxDesc);
            this.Physics.AddElement(boxEl, true);
            this.Physics.Enabled = true;
            this.Physics.RigidBody.KinematicLinear = false;
            // ----- THIS PHYSICS IS NEEDED BECAUSE ENTITY DETECTOR -----

                /*
            MyRBBoxElementDesc boxDesc = physobj.GetRBBoxElementDesc();
            MyMaterialType materialType = MyMaterialType.METAL;
            boxDesc.SetToDefault();
            boxDesc.m_Size = Vector3.One;
            boxDesc.m_CollisionLayer = MyConstants.COLLISION_LAYER_UNCOLLIDABLE;
            boxDesc.m_RBMaterial = MyMaterialsConstants.GetMaterialProperties(materialType).PhysicsMaterial;
              */
            //m_selectionBox = (MyRBBoxElement)physobj.CreateRBElement(boxDesc);
            m_selectionBox = new BoundingBox(-Vector3.One, Vector3.One);

            // to be selectable in editor
            //this.Physics.AddElement(m_selectionBox, true);

            m_userID = objectBuilder.UserOwnerID;
            m_faction = objectBuilder.Faction;

            VisibleInGame = false;
            // set here later on user id when firstly created object inserted into scene

            //m_selectionBox = new BoundingBox(m_worldAABB.Min, m_worldAABB.Max);

            if (OnPrefabContainerInitialized != null)
            {
                OnPrefabContainerInitialized(this);
            }
            
            //StringBuilder displayName;
            //if (!string.IsNullOrEmpty(DisplayName))
            //{
            //    MyHud.ChangeText(this, new StringBuilder(DisplayName), null, 0, MyHudIndicatorFlagsEnum.SHOW_TEXT | MyHudIndicatorFlagsEnum.SHOW_BORDER_INDICATORS | MyHudIndicatorFlagsEnum.SHOW_ONLY_IF_DETECTED_BY_RADAR);
            //}
            m_intializing = false;

            UpdateGenerators();

            UseProperties = new MyUseProperties(MyUseType.FromHUB, MyUseType.FromHUB);
            if (objectBuilder.UseProperties == null)
            {
                UseProperties.Init(MyUseType.FromHUB, MyUseType.FromHUB, 3, 4000, false);
            }
            else
            {                
                UseProperties.Init(objectBuilder.UseProperties);
            }

            AlarmOn = objectBuilder.AlarmOn;
            RefillTime = objectBuilder.RefillTime;

            // check possible values
            if (Inventory.TemplateType != null && RefillTime == null) 
            {
                RefillTime = DEFAULT_REFILL_TIME_IN_SEC;
            }
            else if (Inventory.TemplateType == null && RefillTime != null) 
            {
                RefillTime = null;
            }
        }

        protected override void SetHudMarker()
        {
        }

        //public void AssignToFoundationFactory(MyFoundationFactory foundationFactory)
        //{
        //    Debug.Assert(m_prefabTypeSingleFlagsCount[(int) PrefabTypesFlagEnum.FoundationFactory] == 0);

        //    m_prefabTypesFlag = m_prefabTypesFlag | PrefabTypesFlagEnum.FoundationFactory;
        //    m_prefabTypeSingleFlagsCount[(int)PrefabTypesFlagEnum.FoundationFactory]++;
        //    if (MyEntities.Exist(this)) 
        //    {
        //        MyEntities.Remove(this);
        //    }

        //    foundationFactory.AddChild(this, true);                        
        //}

        //  This method is responsible for adding new prefab modules into container
        public MyEntity CreateAndAddPrefab(string hudLabelText, MyMwcObjectBuilder_PrefabBase prefabBuilder)
        {
            if (m_prefabs.Count + 1 >= m_prefabs.Capacity)
                return null;

            MyPrefabBase prefab = MyPrefabFactory.GetInstance().CreatePrefab(hudLabelText, this, prefabBuilder);

            this.AddPrefab(prefab);
            return (MyEntity) prefab;
        }


        // Removes prefab from this container working prefabs list
        public void RemovePrefab(MyPrefabBase prefab)
        {
            this.Children.Remove(prefab);
            m_prefabs.Remove(prefab);
            UpdateAABB();

            // update prefab type flags
            foreach (PrefabTypesFlagEnum prefabTypeSingleFlag in MyPrefabTypesFlagHelper.ParseToSingleFlags(prefab.PrefabTypeFlag))
            {
                // if we remove last prefab of this type, then we update prefab types flag in prefab container
                m_prefabTypeSingleFlagsCount[(int)prefabTypeSingleFlag]--;
                if (m_prefabTypeSingleFlagsCount[(int)prefabTypeSingleFlag] == 0)
                {
                    m_prefabTypesFlag = m_prefabTypesFlag & (~prefabTypeSingleFlag);
                }                
            }

            // remove prefab from categories dictionary
            m_prefabsByCategory[(ushort)prefab.PrefabCategory].Remove(prefab);

            m_deactivatedPrefabs.Remove(prefab);
        }

        private bool NeedsUpdateNow
        {
            get 
            {
                return NeedsUpdateAlarm || m_needUpdateAABB || RefillTime != null;
            }
        }

        private void RecheckNeedsUpdateState() 
        {
            NeedsUpdate = NeedsUpdateNow;
        }

        private bool NeedsUpdateAlarm 
        {
            get 
            {
                return MyPrefabContainerConstants.MAX_ALARM_PLAYING_TIME_IN_MS > -1 &&
                       m_alarmCue != null &&
                       m_alarmCue.Value.IsPlaying;
            }
        }

        public override void UpdateBeforeSimulation()
        {
            //Dont call this on children, set them NeedsUpdate personally
            //base.UpdateBeforeSimulation();
        }


        public override void UpdateAfterSimulation()
        {
            //Dont call this on children, set them NeedsUpdate personally
            //base.UpdateAfterSimulation();

            if (m_needUpdateAABB)
            {
                UpdateAABBHr();
                m_needUpdateAABB = false;
                //NeedsUpdate = false;
            }
            if (NeedsUpdateAlarm) 
            {
                HandleAlarm();
            }
            if (RefillTime != null) 
            {
                HandleRefillInventory();
            }
            RecheckNeedsUpdateState();
            //for (int i = 0; i < Children.Count; i++)
            //{
            //    var child = Children[i];
            //    child.UpdateAfterSimulation();
            //}
        }

        private void HandleRefillInventory() 
        {
            if (MyMinerGame.TotalGamePlayTimeInMilliseconds - m_lastRefillTime > RefillTime.Value * 1000)
            {
                RefillInventory();
            }
        }

        private void HandleAlarm() 
        {
            if (MyMinerGame.TotalGamePlayTimeInMilliseconds - m_alarmCueStartPlaying > MyPrefabContainerConstants.MAX_ALARM_PLAYING_TIME_IN_MS)
            {
                StopAlarmCue();
            }
        }

        public override bool Draw(MyRenderObject renderObject)
        {
            bool retval = true;
            retval = base.Draw(renderObject);
                 
            return retval;
        }


        /// <summary>
        /// DebugDrawBox
        /// </summary>        
        public override bool DebugDraw()
        {
            base.DebugDraw(); 
            
            //DebugDrawAABBHr();
            foreach (MyPrefabBase prefab in m_prefabs)
            {
                //This is not always called from Render because prefab does not need to have physics
                prefab.DebugDraw();
            }                
            return true;
        }

        /// <summary>
        /// DebugDrawBox
        /// </summary>
        /// <param name="color"></param>
        public void DrawSelectionBox(Vector4 color)
        {
            //@ Draw Sphere in the center
            Matrix worldMat = this.WorldMatrix;
            MySimpleObjectDraw.DrawTransparentSphere(ref worldMat, 2f, ref color, true, 12);

            Matrix mat = Matrix.Identity;// .CreateTranslation(this.LocalVolumeOffset) * this.WorldMatrix;
            BoundingBox bbox = WorldAABBHr;
            MySimpleObjectDraw.DrawWireFramedBox(ref mat, ref bbox, ref color, 0.01f, 1);
        }

        public void DrawSelectionBoxAndBounding(bool selected, bool entered)
        {
            if (MyEditor.Static.IsEditingPrefabContainer() && m_editingActive == false) return;
            //if (m_selectionBox == null) return;

            // Calculate selection box size relatively to position of camera
            Vector3 vLength = MySpectator.Position - GetPosition();
            float screenScale = vLength.Length() / 30;
            float maxDistanceFromCenter = MyPrefabContainerConstants.MAX_DISTANCE_FROM_CONTAINER_CENTER / 4;
            float size = MathHelper.Clamp(screenScale, 1, maxDistanceFromCenter);
            Vector3 selectionBox = new Vector3(size, size, size);

            Vector4 defaultColor = new Vector4(0f, 0.95f, 0f, 0.1f);
            Vector4 highLight = Vector4.Zero;
            Vector4 selectColor = Vector4.Zero; 
            if (entered)
                defaultColor = new Vector4(0.99f, 0.4f, 0f, 0.4f);

            if (HighlightSelectionBox || selected)
            {
                float timeBlic = MyMinerGame.TotalGamePlayTimeInMilliseconds % 400;
                if (timeBlic > 250)
                    timeBlic = 400 - timeBlic;
                timeBlic = MathHelper.Clamp(1 - timeBlic / 250, 0, 1);

                if (selected)
                {
                    defaultColor = new Vector4(0.5f, 0.5f, 0.0f, 0.2f);
                }

                float colorR = MathHelper.Lerp(0f, /*0.8f*/defaultColor.X, timeBlic);
                float colorG = MathHelper.Lerp(/*0.75f*/defaultColor.Y, 0f, timeBlic);
                float colorB = MathHelper.Lerp(0f, /*0.8f*/defaultColor.Z, timeBlic);

                highLight = new Vector4(colorR, colorG, colorB, 0.1f);
            }

            if (MyEditor.DisplayPrefabContainerAxis)
            {
                AddBoundingSelectionBillboard(entered, Vector3.Left, Vector3.Forward, Vector3.Up, new Vector3(-selectionBox.X, 0, 0), selectionBox.X, defaultColor + highLight + selectColor);
                AddBoundingSelectionBillboard(entered, Vector3.Left, Vector3.Forward, Vector3.Up, new Vector3(selectionBox.X, 0, 0), selectionBox.X, defaultColor + highLight + selectColor);
                AddBoundingSelectionBillboard(entered, Vector3.Left, Vector3.Forward, Vector3.Right, new Vector3(0, selectionBox.X, 0), selectionBox.X, defaultColor + highLight + selectColor);
                AddBoundingSelectionBillboard(entered, Vector3.Left, Vector3.Forward, Vector3.Right, new Vector3(0, -selectionBox.X, 0), selectionBox.X, defaultColor + highLight + selectColor);
                AddBoundingSelectionBillboard(entered, Vector3.Left, Vector3.Up, Vector3.Right, new Vector3(0, 0, selectionBox.X), selectionBox.X, defaultColor + highLight + selectColor);
                AddBoundingSelectionBillboard(entered, Vector3.Left, Vector3.Up, Vector3.Right, new Vector3(0, 0, -selectionBox.X), selectionBox.X, defaultColor + highLight + selectColor);

                AddSelectionBoxLineBillboard(entered, AxisTypesEnum.X, defaultColor + selectColor, screenScale);
                AddSelectionBoxLineBillboard(entered, AxisTypesEnum.Y, defaultColor + selectColor, screenScale);
                AddSelectionBoxLineBillboard(entered, AxisTypesEnum.Z, defaultColor + selectColor, screenScale);
            }

            if (MyEditor.DisplayPrefabContainerBounding == true)
            {
                //change texture here if we r in container. . . 
                float distanceFromCenter = MyPrefabContainerConstants.MAX_DISTANCE_FROM_CONTAINER_CENTER;
                AddBoundingBillboard(entered, Vector3.Left, Vector3.Forward, Vector3.Up, new Vector3(-distanceFromCenter, 0, 0), distanceFromCenter, defaultColor + selectColor);
                AddBoundingBillboard(entered, Vector3.Left, Vector3.Forward, Vector3.Up, new Vector3(distanceFromCenter, 0, 0), distanceFromCenter, defaultColor + selectColor);
                AddBoundingBillboard(entered, Vector3.Left, Vector3.Forward, Vector3.Right, new Vector3(0, distanceFromCenter, 0), distanceFromCenter, defaultColor + selectColor);
                AddBoundingBillboard(entered, Vector3.Left, Vector3.Forward, Vector3.Right, new Vector3(0, -distanceFromCenter, 0), distanceFromCenter, defaultColor + selectColor);
                AddBoundingBillboard(entered, Vector3.Left, Vector3.Up, Vector3.Right, new Vector3(0, 0, distanceFromCenter), distanceFromCenter, defaultColor + selectColor);
                AddBoundingBillboard(entered, Vector3.Left, Vector3.Up, Vector3.Right, new Vector3(0, 0, -distanceFromCenter), distanceFromCenter, defaultColor + selectColor);
            }
        }

        void AddSelectionBoxLineBillboard(bool entered, AxisTypesEnum axis, Vector4 color, float scale)
        {
            int lineLength = 222;
            int lineThickness = (int) MathHelper.Clamp(scale / 20, 3, 20);
            
            // Get position to start generating from
            Vector3 startingPosition = Vector3.Zero;
            if (axis == AxisTypesEnum.X)
            {
                startingPosition = this.GetPosition() - new Vector3(-MyPrefabContainerConstants.MAX_DISTANCE_FROM_CONTAINER_CENTER, 0, 0);
            }
            else if (axis == AxisTypesEnum.Y)
            {
                startingPosition = this.GetPosition() - new Vector3(0, MyPrefabContainerConstants.MAX_DISTANCE_FROM_CONTAINER_CENTER, 0);
            }
            else if (axis == AxisTypesEnum.Z)
            {
                startingPosition = this.GetPosition() - new Vector3(0, 0, -MyPrefabContainerConstants.MAX_DISTANCE_FROM_CONTAINER_CENTER);
            }

            for (int i = 0; i < MyPrefabContainerConstants.MAX_DISTANCE_FROM_CONTAINER_CENTER * 2; i += 200)
            {
                float delta = i;
                if (i >= MyPrefabContainerConstants.MAX_DISTANCE_FROM_CONTAINER_CENTER * 2)
                {
                    break;
                }

                Matrix matrix = Matrix.Identity;

                // Choose axis in which to generate selection box lines
                Vector3 position = startingPosition;
                Vector3 direction = Vector3.Zero;
                if (axis == AxisTypesEnum.X)
                {
                    position.X -= delta;
                    direction = this.WorldMatrix.Left;
                }
                else if (axis == AxisTypesEnum.Y)
                {
                    position.Y += delta;
                    direction = this.WorldMatrix.Up;
                }
                else if (axis == AxisTypesEnum.Z)
                {
                    position.Z -= delta;
                    direction = this.WorldMatrix.Forward;
                }

                Matrix localRot = Matrix.Identity;
                matrix.Translation = Vector3.Transform(position - this.GetPosition(), this.WorldMatrix);
                if(entered)
                    MyTransparentGeometry.AddLineBillboard(MyTransparentMaterialEnum.DebrisTrailLine, color, matrix.Translation,
                        direction, lineLength, lineThickness*2.5f);
                else
                    MyTransparentGeometry.AddLineBillboard(MyTransparentMaterialEnum.ProjectileTrailLine, color, matrix.Translation,
                        direction, lineLength, lineThickness);
            }
        }

        public override void DrawMouseOver(ref Vector3 highlightColor)
        {
            // When mouse over selection box, disallow to highlight any other components inside prefab container
            if (HighlightSelectionBox == false)
            {
                base.DrawMouseOver(ref highlightColor);
            }
        }

        private void AddBoundingSelectionBillboard(bool entered, Vector3 localForward, Vector3 localUp, Vector3 localRight, Vector3 localTranslation, float radius, Vector4 color)
        {

            Matrix localRot = Matrix.Identity;
            localRot.Forward = localForward;
            localRot.Up = localUp;
            localRot.Right = localRight;
            localRot.Translation = localTranslation;
            Matrix matrix = localRot * this.GetOrientation();
            matrix.Translation += this.GetPosition();


            MyTransparentGeometry.AddBillboardOriented(
                MyTransparentMaterialEnum.ContainerBorder, color,
                    matrix.Translation, matrix.Left, matrix.Up, radius);
        }

        private void AddBoundingBillboard(bool entered, Vector3 localForward, Vector3 localUp, Vector3 localRight, Vector3 localTranslation, float radius, Vector4 color)
        {

            Matrix localRot = Matrix.Identity;
            localRot.Forward = localForward;
            localRot.Up = localUp;
            localRot.Right = localRight;
            localRot.Translation = localTranslation;
            Matrix matrix = localRot * this.GetOrientation();
            matrix.Translation += this.GetPosition();
            
            
            MyTransparentGeometry.AddBillboardOriented(
                MyTransparentMaterialEnum.ContainerBorderSelected, color,
                    matrix.Translation, matrix.Left, matrix.Up, radius);
        }

        //return ID of user/palyer of this container
        public int GetUserID()
        {
            return m_userID;
        }

        // Method that handles all changes required when moving from commited-2-editing or editing-2-commited state.
        public void SwitchEditMode()
        {
            m_editingActive = !m_editingActive;
        }

        //  Helper method to retrieve center position of multiple phys objects
        Vector3 GetPrefabsCenter(IEnumerable<MyEntity> prefabs)
        {
            Vector3 center = Vector3.Zero;
            int i = 0;
            foreach (MyEntity entity in prefabs)
            {
                center = center + entity.GetPosition();
                i++;
            }

            return Vector3.Divide(center, i);
        }

        public bool GetIntersectionWithMaximumBoundingBox(ref BoundingSphere sphere)
        {
            Vector3 min = new Vector3(-MyPrefabContainerConstants.MAX_DISTANCE_FROM_CONTAINER_CENTER, -MyPrefabContainerConstants.MAX_DISTANCE_FROM_CONTAINER_CENTER, -MyPrefabContainerConstants.MAX_DISTANCE_FROM_CONTAINER_CENTER);
            Vector3 max = new Vector3(MyPrefabContainerConstants.MAX_DISTANCE_FROM_CONTAINER_CENTER, MyPrefabContainerConstants.MAX_DISTANCE_FROM_CONTAINER_CENTER, MyPrefabContainerConstants.MAX_DISTANCE_FROM_CONTAINER_CENTER);

            BoundingSphere newSphere = new BoundingSphere(this.GetPosition(), MyPrefabContainerConstants.MAX_DISTANCE_FROM_CONTAINER_CENTER);
            BoundingBox bb = BoundingBox.CreateFromSphere(newSphere);
            return bb.Intersects(sphere);
        }

        public override bool GetIntersectionWithSphere(ref BoundingSphere sphere)
        {
            return this.WorldAABB.Intersects(sphere);
        }

        public override bool GetIntersectionWithLine(ref MyLine line, out MyIntersectionResultLineTriangleEx? t, IntersectionFlags flags = IntersectionFlags.ALL_TRIANGLES)
        {
            t = null;
            foreach (MyPrefabBase prefab in m_prefabs)
            {
                MyEntity entity = prefab as MyEntity;
                MyIntersectionResultLineTriangleEx? prefabIntersectionResult;
                entity.GetIntersectionWithLine(ref line, out prefabIntersectionResult);
                t = MyIntersectionResultLineTriangleEx.GetCloserIntersection(ref t, ref prefabIntersectionResult);
            }

            return t != null;
        }

        public override bool GetIntersectionWithLine(ref MyLine line, out Vector3? v, bool useCollisionModel = true, IntersectionFlags flags = IntersectionFlags.ALL_TRIANGLES)
        {
            v = null;
            BoundingBox box = BoundingBox.CreateFromSphere(new BoundingSphere(GetPosition(), m_selectionBox.Size().X));
            float? dt = MyUtils.GetLineBoundingBoxIntersection(ref line, ref box);

            if (dt == null)
                return false;

            v = line.From + line.Direction * dt;
            return true;
        }

        public int GetWorkingPrefabsCount()
        {
            return m_prefabs.Count;
        }


        protected override MyMwcObjectBuilder_Base GetObjectBuilderInternal(bool getExactCopy)
        {
            List<MyMwcObjectBuilder_PrefabBase> prefabs = new List<MyMwcObjectBuilder_PrefabBase>(m_prefabs.Count);
            MyMwcObjectBuilder_Inventory inventory = null;            
            if (Inventory != null)
            {
                inventory = Inventory.GetObjectBuilder(getExactCopy);
            }            
            foreach (MyPrefabBase prefab in m_prefabs)
            {
                MyEntity entity = prefab;
                if (MyEntities.IsMarkedForClose(entity))
                    continue;

                MyMwcObjectBuilder_PrefabBase prefabOld = entity.GetObjectBuilder(getExactCopy) as MyMwcObjectBuilder_PrefabBase;
                prefabs.Add(prefabOld);
            }
            
            MyMwcObjectBuilder_PrefabContainer objectBuilder 
                = new MyMwcObjectBuilder_PrefabContainer(null, MyMwcObjectBuilder_PrefabContainer_TypesEnum.INSTANCE, prefabs,
                                                         GetUserID(), Faction, inventory);
            objectBuilder.PositionAndOrientation.Position = this.GetPosition();
            objectBuilder.PositionAndOrientation.Forward = this.GetOrientation().Forward;
            objectBuilder.PositionAndOrientation.Up = this.GetOrientation().Up;
            objectBuilder.DisplayName = DisplayName;
            objectBuilder.UseProperties = UseProperties.GetObjectBuilder();
            objectBuilder.AlarmOn = AlarmOn;
            objectBuilder.RefillTime = RefillTime;
            return objectBuilder;
        }

        public static MyMwcVector3Short GetRelativePositionInContainerCoords(Vector3 position)
        {
            return new MyMwcVector3Short(
                (short)(position.X * MyPrefabContainerConstants.CONTAINER_CONVERSION_UNIT),
                (short)(position.Y * MyPrefabContainerConstants.CONTAINER_CONVERSION_UNIT),
                (short)(position.Z * MyPrefabContainerConstants.CONTAINER_CONVERSION_UNIT));
        }

        public static Vector3 GetRelativePositionInAbsoluteCoords(MyMwcVector3Short position)
        {
            return new Vector3(
                (float)position.X / MyPrefabContainerConstants.CONTAINER_CONVERSION_UNIT,
                (float)position.Y / MyPrefabContainerConstants.CONTAINER_CONVERSION_UNIT,
                (float)position.Z / MyPrefabContainerConstants.CONTAINER_CONVERSION_UNIT);
        }

        // Each container has limited size which prefab positions cannot exceed, validate it in this method
        // IMPORTANT! - it requires that prefab's relative position in container is provided(not world position)
        public static bool IsPrefabOutOfContainerBounds(Vector3 prefabRelativePosition)
        {
            float x = prefabRelativePosition.X;
            float y = prefabRelativePosition.Y;
            float z = prefabRelativePosition.Z;

            float maxDistance = MyPrefabContainerConstants.MAX_DISTANCE_FROM_CONTAINER_CENTER;

            if (Math.Abs(x) < maxDistance && Math.Abs(y) < maxDistance && Math.Abs(z) < maxDistance)
            {
                return false;
            }

            return true;
        }

        //  Allows you to iterate through all working prefab objects
        public IEnumerable<MyEntity> GetPrefabs()
        {
            foreach (var item in m_prefabs)
            {
                yield return (MyEntity) item;
            }
        }

        public List<MyEntity> GetPrefabsInFrustum(ref BoundingFrustum boundingFrustum)
        {
            List<MyEntity> retList = null;
            foreach (MyEntity entity in m_prefabs)
            {
                if (entity.GetIntersectionWithBoundingFrustum(ref boundingFrustum) == true)
                {
                    if (retList == null)
                        retList = new List<MyEntity>();
                    retList.Add(entity);
                }
            }

            return retList;
        }

        
        public bool IsEditingActive()
        {
            return m_editingActive;
        }
        

        // close container
        public override void Close()
        {            
            MyPrefabContainerManager.GetInstance().RemoveContainer(this);
            Inventory.Close();
            if (m_alarmCue != null && m_alarmCue.Value.IsPlaying) 
            {
                m_alarmCue.Value.Stop(SharpDX.XACT3.StopFlags.Immediate);
            }
            
            base.Close();
        }

        public override bool MoveAndRotate(Vector3 newPosition, Matrix newOrientation)
        {
            base.MoveAndRotate(newPosition, newOrientation);
            UpdateAABB();
            return true;
        }

        /// <summary>
        /// UpdateAABB
        /// </summary>
        public void UpdateAABB()
        {
            m_needUpdateAABB = true;
            NeedsUpdate = true;
        }

        /// <summary>
        /// UpdateEditedNode
        /// </summary>
        /// <param name="entity"></param>
        public void UpdateEditedNode(MyEntity entity)
        {
            if (!(entity is MyPrefabBase))
            {
                //MyCommonDebugUtils.AssertDebug(false);
                return;
            }
        }

        /// <summary>
        /// IsSelectabale
        /// </summary>
        /// <returns></returns>
        public override bool IsSelectable()
        {
            return MyEntities.IsSelectable(this);
        }

        /// <summary>
        /// Add prefab into container - reparenting,...
        /// </summary>
        /// <param name="prefab"></param>
        public void AddPrefab(MyPrefabBase prefab)
        {
            //@ Add prefab
            //if (this.Children.Contains(prefab))
            //{
            //    prefab.SetVisible = true;
            //}
            //else
            //{
            //    this.Children.Add(prefab);
            //    m_prefabs.Add(prefab);
            //}

            Debug.Assert(prefab.Parent == this);
            if (!m_prefabs.Contains(prefab))
            {
                m_prefabs.Add(prefab);
            }
            else 
            {
                prefab.Visible = true;
            }
            UpdateAABB();

            // update prefab type flags
            foreach (PrefabTypesFlagEnum prefabTypeSingleFlag in MyPrefabTypesFlagHelper.ParseToSingleFlags(prefab.PrefabTypeFlag))
            {
                // if we added first prefab of this type, then we update prefab types flag in prefab container
                if (m_prefabTypeSingleFlagsCount[(int)prefabTypeSingleFlag] == 0)
                {
                    m_prefabTypesFlag = m_prefabTypesFlag | prefabTypeSingleFlag;
                }
                m_prefabTypeSingleFlagsCount[(int)prefabTypeSingleFlag]++;

                if (prefabTypeSingleFlag == PrefabTypesFlagEnum.Vendor) 
                {
                    Debug.Assert(m_prefabTypeSingleFlagsCount[(int)prefabTypeSingleFlag] <= 1, "You can have max 1 vendor in prefab container!");
                }
            }          
  
            // add prefab to categories dictionary
            m_prefabsByCategory[(ushort)prefab.PrefabCategory].Add(prefab);

            if (!m_intializing)
            {
                if (prefab is MyPrefabGenerator)
                {
                    UpdateGenerators();
                }
                else
                {
                    FindGeneratorsFor(prefab);
                }
            }
        }        

        public override void OnWorldPositionChanged(object source)
        {
            base.OnWorldPositionChanged(source);
            UpdateAABB();
        }

        /// <summary>
        /// IsEntityFromContainer
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool IsEntityFromContainer(MyEntity entity)
        {
            MyEntity tmpEntity = entity;
            while (tmpEntity != null)
            {
                if (tmpEntity == this)
                    return true;

                tmpEntity = tmpEntity.Parent;
            }

            return false;
        }

        public override bool IsSelectableParentOnly()
        {
            return true;
        }

        /// <summary>
        /// Called when [activated] which for entity means that was added to scene.
        /// </summary>
        /// <param name="source"></param>
        protected override void OnActivated(object source)
        {
            m_editingActive = false;
            base.OnActivated(source);

            MyPrefabContainerManager.GetInstance().AddContainer(this);
        }

        /// <summary>
        /// Called when [deactivated] which for entity means that was removed from scene.
        /// </summary>
        /// <param name="source"></param>
        protected override void OnDeactivated(object source)
        {
            m_editingActive = !m_editingActive;
            
            MyPrefabContainerManager.GetInstance().RemoveContainer(this);

            base.OnDeactivated(source);
        }

        public override MyMwcObjectBuilder_FactionEnum Faction
        {
            get
            {
                //Foundation factory
                if (Parent != null)
                    return Parent.Faction;

                return base.Faction;
            }
        }        

        public bool ContainsPrefab(PrefabTypesFlagEnum prefabTypeFlag)
        {
            return (m_prefabTypesFlag & prefabTypeFlag) != 0;
        }

        public List<MyPrefabBase> GetPrefabs(CategoryTypesEnum prefabCategory)
        {
            return m_prefabsByCategory[(int) prefabCategory];
        }

        public override string GetFriendlyName()
        {
            return "MyPrefabContainer";
        }

        /// <summary>
        /// Update eletricity supply for each prefab
        /// </summary>
        public void UpdateGenerators()
        {
            foreach (var prefab in m_prefabs)
            {
                FindGeneratorsFor(prefab);
            }

            if (CanBeAlarmSignalized() && !m_alarmSignalized && AlarmOn) 
            {
                StartAlarm();
            }
            else if (!CanBeAlarmSignalized() && m_alarmSignalized) 
            {
                StopAlarm();
            }
        }

        /// <summary>
        /// Try find generator for prefab which will supply electricity
        /// </summary>
        public void FindGeneratorsFor(MyPrefabBase prefab)
        {
            int generatorsCount = 0;
            foreach (var item in m_prefabs)
            {
                MyPrefabGenerator generator = item as MyPrefabGenerator;
                if (generator != null && generator.IsWorking())
                {
                    float distanceSqr = generator.GetRange() + WorldVolume.Radius;
                    distanceSqr *= distanceSqr;

                    if(Vector3.DistanceSquared(prefab.GetPosition(), generator.GetPosition()) <= distanceSqr)
                    {
                        //prefab.Generator = generator;
                        //return;
                        generatorsCount++;
                    }
                }
            }

            //prefab.Generator = null;
            prefab.GeneratorsCount = generatorsCount;
        }

        public event OnAlarmLaunchned OnAlarmLaunchned;

        private MySoundCue? m_alarmCue;
        private int m_alarmCueStartPlaying;

        private bool m_alarmOn;
        public bool AlarmOn 
        {
            get 
            {
                return m_alarmOn;
            }
            set 
            {
                //bool changed = value != m_alarmOn;
                m_alarmOn = value;                
                //if (changed) 
                //{
                //    NeedsUpdate = m_alarmOn;
                //}
                if (m_alarmOn && CanBeAlarmSignalized())
                {
                    StartAlarm();
                }
                else 
                {
                    StopAlarm();
                }
            }
        }

        private bool m_alarmSignalized;

        private bool CanBeAlarmSignalized()
        {
            // we can launch alarm, when prefab auto charging enabled
            if (MyFakes.ENABLE_PREFABS_AUTO_CHARGING) 
            {
                return true;
            }

            // we can launch alarm, when prefab container contains working generator
            foreach (MyPrefabBase prefabGenerator in GetPrefabs(CategoryTypesEnum.GENERATOR)) 
            {
                MyPrefabGenerator generator = prefabGenerator as MyPrefabGenerator;
                if (generator.IsWorking()) 
                {
                    return true;
                }
            }
            // we can launch alarm, when prefab container contains alarm, which doesn't require energy
            foreach (MyPrefabBase prefabAlarm in GetPrefabs(CategoryTypesEnum.ALARM)) 
            {
                MyPrefabAlarm alarm = prefabAlarm as MyPrefabAlarm;
                if (alarm.RequiresEnergy != null && !alarm.RequiresEnergy.Value) 
                {
                    return true;
                }
            }
            return false;
        }

        private void SetAlarmPrefabs(bool enabled) 
        {
            foreach (MyPrefabBase prefab in GetPrefabs(CategoryTypesEnum.ALARM))
            {
                MyPrefabAlarm alarmPrefab = prefab as MyPrefabAlarm;
                Debug.Assert(alarmPrefab != null);
                alarmPrefab.Enabled = enabled;
            }
        }

        private void StartAlarm() 
        {
            SetAlarmPrefabs(true);
            PlayAlarmCue();
            m_alarmSignalized = true;
        }

        private void StopAlarm() 
        {
            SetAlarmPrefabs(false);
            StopAlarmCue();
            m_alarmSignalized = false;
        }

        public void LaunchAlarm(object sender, MyEntity entity) 
        {
            if (!AlarmOn)
            {
                if (OnAlarmLaunchned != null)
                {
                    OnAlarmLaunchned(sender, entity);
                }
                MyScriptWrapper.OnAlarmLaunched(this, entity);
                AlarmOn = true;

                AggroBots(entity);
            }
        }        

        private void PlayAlarmCue() 
        {
            if ((m_alarmCue == null) || !m_alarmCue.Value.IsPlaying)
            {
                m_alarmCue = MyAudio.AddCue2D(MySoundCuesEnum.SfxClaxonAlert);
                m_alarmCueStartPlaying = MyMinerGame.TotalGamePlayTimeInMilliseconds;
            }
            RecheckNeedsUpdateState();
        }

        private void StopAlarmCue() 
        {
            if ((m_alarmCue != null) && m_alarmCue.Value.IsPlaying)
            {
                m_alarmCue.Value.Stop(SharpDX.XACT3.StopFlags.Release);
                m_alarmCue = null;
            }
        }

        private void AggroBots(MyEntity entity)
        {
            MySmallShip smallShip = entity as MySmallShip;
            if (smallShip != null)
            {
                // Untested stealth mechanics, uncomment when needed

                //UpdateAABBHr();
                //BoundingBox aggroBox = new BoundingBox(WorldAABBHr.Min - Vector3.One*1000, WorldAABBHr.Max + Vector3.One*1000);
                //MyDangerZones.Instance.Aggro(aggroBox, smallShip);
            }
        }

        public override void DebugDrawDeactivated()
        {
            foreach (MyPrefabBase prefab in m_deactivatedPrefabs) 
            {
                prefab.DebugDrawDeactivated();
            }

            if (!Activated && Visible) 
            {
                DrawSelectionBoxAndBounding(false, true);
            }
        }

        public void DeactivatePrefab(MyPrefabBase prefab) 
        {
            if(!m_deactivatedPrefabs.Contains(prefab) && prefab.Parent == this)
            {
                m_deactivatedPrefabs.Add(prefab);
            }
        }

        public void ActivatePrefab(MyPrefabBase prefab) 
        {            
            m_deactivatedPrefabs.Remove(prefab);
        }

        public IEnumerable<MyPrefabBase> GetDeactivatedPrefabs() 
        {
            return m_deactivatedPrefabs;
        }

        public void RefillInventory() 
        {
            Debug.Assert(Inventory.TemplateType != null);
            if (!MyGuiScreenInventoryManagerForGame.IsOtherSideInventory(Inventory))
            {
                Inventory.ClearInventoryItems(true);
                MyInventoryTemplates.RefillInventory(Inventory);
                m_lastRefillTime = MyMinerGame.TotalGamePlayTimeInMilliseconds;
            }
        }

        #region IMyUseablePrefab
        public MyGuiControlEntityUse GetGuiControl(IMyGuiControlsParent parent)
        {
            return new MyGuiControlPrefabContainerUse(parent, this);
        }

        public MyEntity GetEntity()
        {
            return this;
        }

        public void Use(MySmallShip useBy)
        {
            MyGuiManager.AddScreen(new MyGuiScreenEntityUseSolo(this));
        }

        public void UseFromHackingTool(MySmallShip useBy, int hackingLevelDifference)
        {
            Use(useBy);
        }

        public bool CanBeUsed(MySmallShip usedBy)
        {
            return MyFactions.GetFactionsRelation(usedBy, this) == MyFactionRelationEnum.Friend;
        }

        public bool CanBeHacked(MySmallShip hackedBy)
        {
            return (MyFactions.GetFactionsRelation(hackedBy, this) == MyFactionRelationEnum.Neutral ||
                MyFactions.GetFactionsRelation(hackedBy, this) == MyFactionRelationEnum.Enemy);
        }

        public MyUseProperties UseProperties
        {
            get;
            set;
        }
        #endregion
    }
}