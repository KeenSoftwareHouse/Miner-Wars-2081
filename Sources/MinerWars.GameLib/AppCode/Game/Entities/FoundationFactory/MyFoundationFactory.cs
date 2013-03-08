//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Text;
//using MinerWarsMath;
//using MinerWars.AppCode.App;
//using MinerWars.AppCode.Game.Gameplay;
//using MinerWars.AppCode.Game.HUD;
//using MinerWars.AppCode.Game.Inventory;
//using MinerWars.AppCode.Game.Localization;
//using MinerWars.AppCode.Game.Entities.Prefabs;
//using MinerWars.AppCode.Game.Entities.Tools;
//using MinerWars.AppCode.Game.Managers.Session;
//using MinerWars.AppCode.Game.Models;
//using MinerWars.AppCode.Game.Utils;
//using MinerWars.AppCode.Game.World;
//using MinerWars.AppCode.Physics;
//using MinerWars.CommonLIB.AppCode.ObjectBuilders;
//using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
//using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;

//namespace MinerWars.AppCode.Game.Entities.FoundationFactory
//{
//    internal delegate void OnBuildingComplete(MyFoundationFactory sender, MyObjectToBuild buildedObject);

//    /// <summary>
//    /// Foundation factory entity
//    /// </summary>
//    class MyFoundationFactory : MyEntity//, IMyInventory
//    {
//        #region Fields
//        private int m_buildingTimeFromStart;          // elapsed time from when building started
//        private bool m_isBuilding;                      // is building now
//        //private BoundingSphere m_boundingShere;         // helper bounding sphere to detecting
//        private MyNanoRepairToolFoundationFactory m_nanoRepairTool;
//        #endregion

//        #region Properties
//        ///// <summary>
//        ///// Inventory of foundation factory
//        ///// </summary>
//        //public MyInventory Inventory { get { return PrefabContainer.Inventory; } }

//        /// <summary>
//        /// Build objects collection
//        /// </summary>
//        public List<MyObjectToBuild> BuildObjects { get; private set; }

//        /// <summary>
//        /// Building queue
//        /// </summary>
//        public List<MyObjectToBuild> BuildingQueue { get; private set; }

//        /// <summary>
//        /// Actual building object
//        /// </summary>
//        public MyObjectToBuild BuildingObject { get; private set; }

//        /// <summary>
//        /// Prefab container of foundation factory
//        /// </summary>
//        public MyPrefabContainer PrefabContainer { get; private set; }

//        /// <summary>
//        /// Owner of foundation factory
//        /// </summary>
//        public MyPlayer Player { get; private set; }        

//        /// <summary>
//        /// Is building enabled
//        /// </summary>
//        public bool IsBuilding
//        {
//            get { return m_isBuilding; }
//        }

//        /// <summary>
//        /// Time left to complete building
//        /// </summary>
//        public int TimeToCompleteBuldingLeft
//        {
//            get
//            {
//                if(IsBuilding)
//                {
//                    return BuildingObject.BuildingSpecification.BuildingTime - m_buildingTimeFromStart;
//                } 
//                else
//                {
//                    return 0;
//                }
//            }
//        }

//        /// <summary>
//        /// How much is building complete
//        /// </summary>
//        public float BuildingPercentageComplete
//        {
//            get
//            {
//                if(IsBuilding)
//                {
//                    if (BuildingObject.BuildingSpecification.BuildingTime > 0)
//                    {
//                        return (float)m_buildingTimeFromStart/BuildingObject.BuildingSpecification.BuildingTime;
//                    }
//                    else
//                    {
//                        return 1f;
//                    }
//                }
//                else
//                {
//                    return 0f;
//                }
//            }
//        }
//        #endregion

//        #region Events
//        /// <summary>
//        /// Call when building complete
//        /// </summary>
//        public event OnBuildingComplete BuildingComplete;        
//        #endregion

//        #region Ctors
//        /// <summary>
//        /// Creates new instance of foundation factory
//        /// </summary>
//        /// <param name="player">Owner of foundation factory</param>
//        public MyFoundationFactory(MyPlayer player)
//        {                        
//            BuildObjects = new List<MyObjectToBuild>();
//            BuildingQueue = new List<MyObjectToBuild>();
//            BuildingObject = null;

//            PrefabContainer = null;

//            Player = player;

//            m_buildingTimeFromStart = 0;
//            m_isBuilding = false;                        
//        }
//        #endregion

//        #region Methods
//        /// <summary>
//        /// Starts building
//        /// </summary>
//        public void StartBuilding()
//        {
//            if (IsBuilding)
//            {
//                throw new Exception("Building is already started");
//            }

//            if (BuildingObject == null)
//            {
//                if (BuildingQueue.Count == 0)
//                {
//                    throw new Exception("There are nothing to build");
//                }
                
//                SetBuildingNextObject();
//                m_buildingTimeFromStart = 0;
//            }

//            m_isBuilding = true;
//        }

//        /// <summary>
//        /// Stops building
//        /// </summary>
//        public void StopBuilding()
//        {
//            if (!IsBuilding)
//            {
//                throw new Exception("Building is not running");
//            }

//            m_isBuilding = false;
//        }

//        /// <summary>
//        /// Cancels actual building object
//        /// </summary>
//        public void CancelBuilding()
//        {
//            if (!IsBuilding)
//            {
//                throw new Exception("Building is not running");
//            }

//            ChangeInventoryItemsAmount(false, MyFoundationFactoryConstants.RETURN_AMOUNT_RATIO * m_buildingTimeFromStart / BuildingObject.BuildingSpecification.BuildingTime, BuildingObject);
//            if (BuildingQueue.Count > 0)
//            {
//                SetBuildingNextObject();
//            } 
//            else
//            {
//                BuildingObject = null;
//                m_isBuilding = false;
//            }

//            m_buildingTimeFromStart = 0;            
//        }

//        /// <summary>
//        /// Adds object to building queue
//        /// </summary>
//        /// <param name="objectToBuild">Object to build</param>
//        public void AddToBuildingQueue(MyObjectToBuild objectToBuild)
//        {            
//            if(!objectToBuild.BuildingSpecification.CanBuild(this))
//            {
//                throw new Exception("You can't build this!");
//            }

//            BuildingQueue.Add(objectToBuild);
//            ChangeInventoryItemsAmount(true, 1f, objectToBuild);            

//            if(!IsBuilding)
//            {
//                StartBuilding();
//            }            
//        }

//        /// <summary>
//        /// Removes object from building queue
//        /// </summary>
//        /// <param name="objectToBuild">Object to build</param>
//        public void RemoveFromBuildingQueue(MyObjectToBuild objectToBuild)
//        {
//            int indexOfLastObjectToBuild = BuildingQueue.FindLastIndex(x => x == objectToBuild);
//            if(indexOfLastObjectToBuild >= 0)
//            {
//                BuildingQueue.RemoveAt(indexOfLastObjectToBuild);
//            }            
//            ChangeInventoryItemsAmount(false, MyFoundationFactoryConstants.RETURN_AMOUNT_RATIO, objectToBuild);            
//        }

//        /// <summary>
//        /// Removes build object from build objects collection
//        /// </summary>
//        /// <param name="objectToBuild">Build object</param>
//        public void RemoveFromBuildObjects(MyObjectToBuild objectToBuild)
//        {
//            BuildObjects.Remove(objectToBuild);
//        }                        

//        /// <summary>
//        /// Sets next object from building queue to building
//        /// </summary>
//        private void SetBuildingNextObject()
//        {
//            if(BuildingQueue.Count == 0)
//            {
//                throw new Exception("There is no object to build in building queue");
//            }
            
//            BuildingObject = BuildingQueue[0];
//            BuildingQueue.RemoveAt(0);            
//        }

//        /// <summary>
//        /// Changes inventory item's amount. When add/remove object to/from building queue
//        /// </summary>
//        /// <param name="remove">Remove = true, Add = false</param>
//        /// <param name="ratio">Ratio of item's amount</param>
//        /// <param name="objectToBuild">Object to build</param>
//        private void ChangeInventoryItemsAmount(bool remove, float ratio, MyObjectToBuild objectToBuild)
//        {
//            foreach (IMyBuildingRequirement buildingRequirement in objectToBuild.BuildingSpecification.BuildingRequirements)
//            {
//                MyBuildingRequirementInventoryItem buildingRequirementInventoryItem = buildingRequirement as MyBuildingRequirementInventoryItem;
//                if (buildingRequirementInventoryItem != null)
//                {
//                    if (buildingRequirementInventoryItem.RemoveAfterBuild)
//                    {
//                        if (remove)
//                        {
//                            Player.Ship.Inventory.RemoveInventoryItemAmount(buildingRequirementInventoryItem.ObjectBuilderType,
//                                                            buildingRequirementInventoryItem.ObjectBuilderId,
//                                                            buildingRequirementInventoryItem.Amount * ratio);
//                        } 
//                        else
//                        {
//                            Player.Ship.Inventory.AddInventoryItem(buildingRequirementInventoryItem.ObjectBuilderType,
//                                                            buildingRequirementInventoryItem.ObjectBuilderId,
//                                                            buildingRequirementInventoryItem.Amount * ratio, false);
//                        }
//                    }
//                }
//            }
//        }

//        /// <summary>
//        /// Completes actual building
//        /// </summary>
//        private void CompleteBuilding()
//        {
//            MyObjectToBuild buildObject = BuildingObject;
//            PrefabContainer.Inventory.AddInventoryItem(buildObject.ObjectBuilder, buildObject.Amount, false);
//            //BuildObjects.Add(buildObject);            );

//            if(BuildingQueue.Count != 0)
//            {
//                SetBuildingNextObject();
//            } 
//            else
//            {
//                BuildingObject = null;
//                m_isBuilding = false;                
//            }

//            m_buildingTimeFromStart = 0;

//            if (BuildingComplete != null)
//            {
//                BuildingComplete(this, buildObject);
//            }
//        }

//        /// <summary>
//        /// Creates new foundation factory from player's inventory. When player has no foundation factory in inventory, then returns result false.
//        /// </summary>
//        /// <param name="player">Owner of foundation factory</param>
//        /// <param name="result">Result of foundation factory creation</param>
//        /// <returns>Instance of new foundation factory</returns>
//        public static MyFoundationFactory TryCreateFoundationFactory(MyPlayer player, out bool result)
//        {
//            MyFoundationFactory foundationFactory = null;
//            List<MyInventoryItem> foundationFactories = player.Ship.Inventory.GetInventoryItems(MyMwcObjectBuilderTypeEnum.FoundationFactory, null);
//            if (foundationFactories.Count == 0)
//            {
//                result = false;                
//            }
//            else
//            {
//                MyMwcObjectBuilder_FoundationFactory foundationFactoryBuilder =
//                    MyMwcObjectBuilder_Base.CreateNewObject(MyMwcObjectBuilderTypeEnum.FoundationFactory, null) as MyMwcObjectBuilder_FoundationFactory;
//                foundationFactoryBuilder.PrefabContainer = new MyMwcObjectBuilder_PrefabContainer(
//                    null, MyMwcObjectBuilder_PrefabContainer_TypesEnum.INSTANCE, new List<MyMwcObjectBuilder_PrefabBase>(), MyClientServer.LoggedPlayer.GetUserId(),
//                    player.Faction, new MyMwcObjectBuilder_Inventory(new List<MyMwcObjectBuilder_InventoryItem>(), 1000));

//                Matrix ffWorld = Matrix.Identity;
//                ffWorld.Translation = player.Ship.WorldMatrix.Translation + player.Ship.WorldMatrix.Forward * 20;
//                //Matrix ffWorld = Matrix.CreateWorld(player.Ship.WorldMatrix.Translation + player.Ship.WorldMatrix.Forward * 20,
//                //                                    player.Ship.WorldMatrix.Forward, player.Ship.WorldMatrix.Up);                
//                foundationFactory = new MyFoundationFactory(player);
//                foundationFactory.Init(null, foundationFactoryBuilder, ffWorld);
//                MyEntities.Add(foundationFactory);

//                MyInventoryItem foundationFactoryItem = foundationFactories[0];
//                foundationFactories.RemoveAt(0);
//                player.Ship.Inventory.RemoveInventoryItemAmount(ref foundationFactoryItem, 1f);
                                
//                result = true;
//            }
//            return foundationFactory;
//        }

//        /// <summary>
//        /// Inits foundation factory 
//        /// </summary>
//        /// <param name="hudLabelText">HUD label text</param>
//        /// <param name="objectBuilder">Object builder of foundation factory</param>
//        /// <param name="matrix">World's matrix</param>
//        public virtual void Init(string hudLabelText, MyMwcObjectBuilder_FoundationFactory objectBuilder, Matrix matrix)
//        {
//            InitFoundationFactory(hudLabelText, objectBuilder, matrix);

//            Debug.Assert(objectBuilder.PrefabContainer != null);

//            Matrix pcWorld = WorldMatrix;
//            pcWorld.Translation = WorldVolume.Center;
//            PrefabContainer = new MyPrefabContainer();
//            PrefabContainer.Init(null, objectBuilder.PrefabContainer, pcWorld);
//            PrefabContainer.AssignToFoundationFactory(this);
          
//            PrefabContainer.Inventory.OnInventoryContentChange += OnInventoryContentChanged;            
            
//        }

//        /// <summary>
//        /// Inits foundation factory and assign existing prefab container
//        /// </summary>
//        /// <param name="hudLabelText">HUD label text</param>
//        /// <param name="objectBuilder">Object builder of foundation factory</param>
//        /// <param name="assignedPrefabContainer">Assigned prefab container</param>
//        public virtual void Init(string hudLabelText, MyMwcObjectBuilder_FoundationFactory objectBuilder, MyPrefabContainer assignedPrefabContainer)
//        {                        
//            InitFoundationFactory(hudLabelText, objectBuilder, assignedPrefabContainer.WorldMatrix);

//            Matrix newWorldMatrix = WorldMatrix;
//            newWorldMatrix.Translation += (WorldMatrix.Translation - WorldVolume.Center);
//            SetWorldMatrix(newWorldMatrix);

//            PrefabContainer = assignedPrefabContainer;
//            PrefabContainer.AssignToFoundationFactory(this);            

//            PrefabContainer.Inventory.OnInventoryContentChange += OnInventoryContentChanged;            
//        }

//        private void InitFoundationFactory(string hudLabelText, MyMwcObjectBuilder_FoundationFactory objectBuilder, Matrix matrix)
//        {
//            Debug.Assert(objectBuilder != null);            

//            StringBuilder hudLabelTextSb = string.IsNullOrEmpty(hudLabelText) ? MyTextsWrapper.Get(MyTextsWrapperEnum.FoundationFactory) : new StringBuilder(hudLabelText);

//            base.Init(hudLabelTextSb, MyModelsEnum.FoundationFactory, null, null, null, objectBuilder);

//            SetWorldMatrix(matrix);

//            m_isBuilding = objectBuilder.IsBuilding;
//            m_buildingTimeFromStart = objectBuilder.BuildingTimeFromStart;
//            if (objectBuilder.BuildingObject != null)
//            {
//                BuildingObject = MyObjectToBuild.CreateFromObjectBuilder(objectBuilder.BuildingObject.ObjectBuilder, objectBuilder.BuildingObject.Amount);
//            }
//            if (objectBuilder.BuildingQueue != null)
//            {
//                foreach (MyMwcObjectBuilder_ObjectToBuild buildingQueue in objectBuilder.BuildingQueue)
//                {
//                    BuildingQueue.Add(MyObjectToBuild.CreateFromObjectBuilder(buildingQueue.ObjectBuilder, buildingQueue.Amount));
//                }
//            }

//            base.InitBoxPhysics(MyMaterialType.METAL, ModelLod0, 1f, 0f, MyConstants.COLLISION_LAYER_ALL, RigidBodyFlag.RBF_RBO_STATIC);

//            MyHud.RemoveText(this);            
//        }

//        private void OnInventoryContentChanged(MyInventory sender)
//        {
//            if (sender.GetInventoryItems(MyMwcObjectBuilderTypeEnum.SmallShip_Tool, (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.NANO_REPAIR_TOOL).Count > 0)
//            {
//                if (m_nanoRepairTool == null)
//                {
//                    m_nanoRepairTool = new MyNanoRepairToolFoundationFactory(this);
//                }
//            }
//            else
//            {
//                m_nanoRepairTool = null;                
//            }
//        }

//        #endregion

//        #region Overriden methods
//        public override void Update()
//        {
//            base.Update();

//            if (IsBuilding)
//            {
//                m_buildingTimeFromStart += MyConstants.PHYSICS_STEP_SIZE_IN_MILLISECONDS;
//                if (m_buildingTimeFromStart >= BuildingObject.BuildingSpecification.BuildingTime)
//                {
//                    CompleteBuilding();
//                }
//            }

//            if (m_nanoRepairTool != null)
//            {
//                m_nanoRepairTool.Use();
//            }
//        }        

//        protected override MyMwcObjectBuilder_Base  GetObjectBuilderInternal(bool getExactCopy)
//        {
//            MyMwcObjectBuilder_Base objectBuilderBase = base.GetObjectBuilderInternal(getExactCopy);
//            MyMwcObjectBuilder_FoundationFactory ffBuilder = objectBuilderBase as MyMwcObjectBuilder_FoundationFactory;
//            if (ffBuilder != null)
//            {
//                ffBuilder.BuildingObject = BuildingObject != null ? new MyMwcObjectBuilder_ObjectToBuild(BuildingObject.ObjectBuilder, BuildingObject.Amount) : null;
//                ffBuilder.IsBuilding = m_isBuilding;
//                ffBuilder.BuildingTimeFromStart = m_buildingTimeFromStart;
//                ffBuilder.PrefabContainer = PrefabContainer.GetObjectBuilder(getExactCopy) as MyMwcObjectBuilder_PrefabContainer;
//                ffBuilder.BuildingQueue = new List<MyMwcObjectBuilder_ObjectToBuild>();                
//                foreach (MyObjectToBuild objectToBuild in BuildingQueue)
//                {
//                    ffBuilder.BuildingQueue.Add(new MyMwcObjectBuilder_ObjectToBuild(objectToBuild.ObjectBuilder, objectToBuild.Amount));
//                }                
//                //objectBuilderBase = ffBuilder;
//            }
//            return objectBuilderBase;
//        }

//        public override MyMwcObjectBuilder_FactionEnum Faction
//        {
//            get
//            {
//                return Player.Faction;
//            }
//        }

//        #endregion
//    }
//}
