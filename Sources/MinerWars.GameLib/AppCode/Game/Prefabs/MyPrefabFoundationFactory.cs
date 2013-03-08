using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.HUD;
using MinerWars.AppCode.Game.Inventory;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Entities.FoundationFactory;
using MinerWars.AppCode.Game.Entities.Tools;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Models;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.World;
using MinerWars.AppCode.Physics;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.Prefabs;

namespace MinerWars.AppCode.Game.Prefabs
{
    internal delegate void OnBuildingComplete(MyPrefabFoundationFactory sender, MyObjectToBuild buildedObject);

    class MyPrefabFoundationFactory : MyPrefabBase
    {
        #region Fields
        private int m_buildingTimeFromStart;          // elapsed time from when building started
        private bool m_isBuilding;                      // is building now                
        #endregion                        

        #region Properties
        /// <summary>
        /// Owner of foundation factory
        /// </summary>
        public MyPlayer Player { get; private set; }

        /// <summary>
        /// Build objects collection
        /// </summary>
        public List<MyObjectToBuild> BuildObjects { get; private set; }

        /// <summary>
        /// Building queue
        /// </summary>
        public List<MyObjectToBuild> BuildingQueue { get; private set; }

        /// <summary>
        /// Actual building object
        /// </summary>
        public MyObjectToBuild BuildingObject { get; private set; }

        /// <summary>
        /// Prefab container of foundation factory
        /// </summary>
        public MyPrefabContainer PrefabContainer
        {
            get
            {
                return m_owner;
            }
        }

        /// <summary>
        /// Is building enabled
        /// </summary>
        public bool IsBuilding
        {
            get { return m_isBuilding; }
            private set
            {
                bool changed = m_isBuilding != value;
                m_isBuilding = value;
                if (changed) 
                {
                    RecheckNeedsUpdate();
                }
            }
        }

        /// <summary>
        /// Time left to complete building
        /// </summary>
        public int TimeToCompleteBuldingLeft
        {
            get
            {
                if(IsBuilding)
                {
                    return BuildingObject.BuildingSpecification.BuildingTime - m_buildingTimeFromStart;
                } 
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// How much is building complete
        /// </summary>
        public float BuildingPercentageComplete
        {
            get
            {
                if(IsBuilding)
                {
                    if (BuildingObject.BuildingSpecification.BuildingTime > 0)
                    {
                        return (float)m_buildingTimeFromStart/BuildingObject.BuildingSpecification.BuildingTime;
                    }
                    else
                    {
                        return 1f;
                    }
                }
                else
                {
                    return 0f;
                }
            }
        }

        public MyMwcObjectBuilder_PrefabFoundationFactory_TypesEnum FoundationFactoryType
        {
            get
            {
                return (MyMwcObjectBuilder_PrefabFoundationFactory_TypesEnum)m_prefabId;
            }
            set
            {
                m_prefabId = (int)value;
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Call when building complete
        /// </summary>
        public event OnBuildingComplete BuildingComplete;        
        #endregion

        #region Ctors
        /// <summary>
        /// Creates new instance of foundation factory prefab
        /// </summary>
        /// <param name="owner">Owner</param>
        public MyPrefabFoundationFactory(MyPrefabContainer owner)
            : base(owner)
        {
            Player = MySession.Static.Player;
            BuildObjects = new List<MyObjectToBuild>();
            BuildingQueue = new List<MyObjectToBuild>();
            BuildingObject = null;            

            m_buildingTimeFromStart = 0;
            IsBuilding = false;
        }        
        #endregion

        #region Methods
        /// <summary>
        /// Starts building
        /// </summary>
        public void StartBuilding()
        {
            if (IsBuilding)
            {
                throw new Exception("Building is already started");
            }

            if (BuildingObject == null)
            {
                if (BuildingQueue.Count == 0)
                {
                    throw new Exception("There are nothing to build");
                }
                
                SetBuildingNextObject();
                m_buildingTimeFromStart = 0;
            }

            IsBuilding = true;
        }

        /// <summary>
        /// Stops building
        /// </summary>
        public void StopBuilding()
        {
            if (!IsBuilding)
            {
                throw new Exception("Building is not running");
            }

            IsBuilding = false;
        }

        /// <summary>
        /// Cancels actual building object
        /// </summary>
        public void CancelBuilding()
        {
            if (!IsBuilding)
            {
                throw new Exception("Building is not running");
            }

            ChangeInventoryItemsAmount(false, MyFoundationFactoryConstants.RETURN_AMOUNT_RATIO * m_buildingTimeFromStart / BuildingObject.BuildingSpecification.BuildingTime, BuildingObject);
            if (BuildingQueue.Count > 0)
            {
                SetBuildingNextObject();
            } 
            else
            {
                BuildingObject = null;
                IsBuilding = false;
            }

            m_buildingTimeFromStart = 0;            
        }

        /// <summary>
        /// Adds object to building queue
        /// </summary>
        /// <param name="objectToBuild">Object to build</param>
        public void AddToBuildingQueue(MyObjectToBuild objectToBuild)
        {            
            if(!objectToBuild.BuildingSpecification.CanBuild(this))
            {
                throw new Exception("You can't build this!");
            }

            BuildingQueue.Add(objectToBuild);
            ChangeInventoryItemsAmount(true, 1f, objectToBuild);            

            if(!IsBuilding)
            {
                StartBuilding();
            }            
        }

        /// <summary>
        /// Removes object from building queue
        /// </summary>
        /// <param name="objectToBuild">Object to build</param>
        public void RemoveFromBuildingQueue(MyObjectToBuild objectToBuild)
        {
            int indexOfLastObjectToBuild = BuildingQueue.FindLastIndex(x => x == objectToBuild);
            if(indexOfLastObjectToBuild >= 0)
            {
                BuildingQueue.RemoveAt(indexOfLastObjectToBuild);
            }            
            ChangeInventoryItemsAmount(false, MyFoundationFactoryConstants.RETURN_AMOUNT_RATIO, objectToBuild);            
        }

        /// <summary>
        /// Removes build object from build objects collection
        /// </summary>
        /// <param name="objectToBuild">Build object</param>
        public void RemoveFromBuildObjects(MyObjectToBuild objectToBuild)
        {
            BuildObjects.Remove(objectToBuild);
        }                        

        /// <summary>
        /// Sets next object from building queue to building
        /// </summary>
        private void SetBuildingNextObject()
        {
            if(BuildingQueue.Count == 0)
            {
                throw new Exception("There is no object to build in building queue");
            }
            
            BuildingObject = BuildingQueue[0];
            BuildingQueue.RemoveAt(0);            
        }

        /// <summary>
        /// Changes inventory item's amount. When add/remove object to/from building queue
        /// </summary>
        /// <param name="remove">Remove = true, Add = false</param>
        /// <param name="ratio">Ratio of item's amount</param>
        /// <param name="objectToBuild">Object to build</param>
        private void ChangeInventoryItemsAmount(bool remove, float ratio, MyObjectToBuild objectToBuild)
        {
            foreach (IMyBuildingRequirement buildingRequirement in objectToBuild.BuildingSpecification.BuildingRequirements)
            {
                MyBuildingRequirementInventoryItem buildingRequirementInventoryItem = buildingRequirement as MyBuildingRequirementInventoryItem;
                if (buildingRequirementInventoryItem != null)
                {
                    if (buildingRequirementInventoryItem.RemoveAfterBuild)
                    {
                        if (remove)
                        {                            
                            Player.Ship.Inventory.RemoveInventoryItemAmount(buildingRequirementInventoryItem.ObjectBuilderType,
                                                            buildingRequirementInventoryItem.ObjectBuilderId,
                                                            buildingRequirementInventoryItem.Amount * ratio);
                        } 
                        else
                        {
                            Player.Ship.Inventory.AddInventoryItem(buildingRequirementInventoryItem.ObjectBuilderType,
                                                            buildingRequirementInventoryItem.ObjectBuilderId,
                                                            buildingRequirementInventoryItem.Amount * ratio, false);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Completes actual building
        /// </summary>
        private void CompleteBuilding()
        {
            MyObjectToBuild buildObject = BuildingObject;
            PrefabContainer.Inventory.AddInventoryItem(buildObject.ObjectBuilder, buildObject.Amount, false, true);            

            if(BuildingQueue.Count != 0)
            {
                SetBuildingNextObject();
            } 
            else
            {
                BuildingObject = null;
                IsBuilding = false;                
            }

            m_buildingTimeFromStart = 0;

            if (BuildingComplete != null)
            {
                BuildingComplete(this, buildObject);
            }
        }

        /// <summary>
        /// Creates new foundation factory from player's inventory. When player has no foundation factory in inventory, then returns result false.
        /// </summary>        
        /// <param name="result">Result of foundation factory creation</param>
        /// <returns>Instance of new foundation factory</returns>
        public static MyPrefabFoundationFactory TryCreateFoundationFactory(MyPlayer player, out bool result)
        {
            MyPrefabFoundationFactory foundationFactory = null;            
            MyInventoryItem foundationFactoryItem = player.Ship.Inventory.GetInventoryItem(MyMwcObjectBuilderTypeEnum.PrefabFoundationFactory, (int)MyMwcObjectBuilder_PrefabFoundationFactory_TypesEnum.DEFAULT);
            if (foundationFactoryItem == null) 
            {
                foundationFactoryItem = player.Ship.Inventory.GetInventoryItem(MyMwcObjectBuilderTypeEnum.FoundationFactory, null);
            }
            if (foundationFactoryItem == null)
            {
                result = false;                
            }
            else
            {
                MyMwcObjectBuilder_PrefabFoundationFactory foundationFactoryBuilder =
                    MyMwcObjectBuilder_Base.CreateNewObject(MyMwcObjectBuilderTypeEnum.PrefabFoundationFactory, (int)MyMwcObjectBuilder_PrefabFoundationFactory_TypesEnum.DEFAULT) as MyMwcObjectBuilder_PrefabFoundationFactory;
                MyMwcObjectBuilder_PrefabContainer prefabContainerBuilder = new MyMwcObjectBuilder_PrefabContainer(
                    null, MyMwcObjectBuilder_PrefabContainer_TypesEnum.INSTANCE, new List<MyMwcObjectBuilder_PrefabBase>(), MyClientServer.LoggedPlayer.GetUserId(),
                    player.Faction, new MyMwcObjectBuilder_Inventory(new List<MyMwcObjectBuilder_InventoryItem>(), 1000));

                Matrix ffWorld = Matrix.Identity;
                ffWorld.Translation = player.Ship.WorldMatrix.Translation + player.Ship.WorldMatrix.Forward * 20;

                MyPrefabContainer prefabContainer = new MyPrefabContainer();
                prefabContainer.Init(null, prefabContainerBuilder, ffWorld);
                MyEntities.Add(prefabContainer);

                MyPrefabConfiguration ffConfiguration = MyPrefabConstants.GetPrefabConfiguration(MyMwcObjectBuilderTypeEnum.PrefabFoundationFactory, (int)MyMwcObjectBuilder_PrefabFoundationFactory_TypesEnum.DEFAULT);
                
                foundationFactory = new MyPrefabFoundationFactory(prefabContainer);
                foundationFactory.Init(null, new Vector3(0f, 0f, 0f), Matrix.Identity, foundationFactoryBuilder, ffConfiguration);                
                prefabContainer.AddPrefab(foundationFactory);
                
                player.Ship.Inventory.RemoveInventoryItemAmount(ref foundationFactoryItem, 1f);
                                
                result = true;
            }
            return foundationFactory;
        }                        

        #endregion

        #region Overriden methods        
        protected override void InitPrefab(string displayName, Vector3 relativePosition, Matrix localOrientation, MyMwcObjectBuilder_PrefabBase objectBuilder, MyPrefabConfiguration prefabConfig)
        {
            MyMwcObjectBuilder_PrefabFoundationFactory objectBuilderFF = objectBuilder as MyMwcObjectBuilder_PrefabFoundationFactory;                        

            IsBuilding = objectBuilderFF.IsBuilding;
            m_buildingTimeFromStart = objectBuilderFF.BuildingTimeFromStart;
            if (objectBuilderFF.BuildingObject != null)
            {
                BuildingObject = MyObjectToBuild.CreateFromObjectBuilder(objectBuilderFF.BuildingObject.ObjectBuilder, objectBuilderFF.BuildingObject.Amount);
            }
            if (objectBuilderFF.BuildingQueue != null)
            {
                foreach (MyMwcObjectBuilder_ObjectToBuild buildingQueue in objectBuilderFF.BuildingQueue)
                {
                    BuildingQueue.Add(MyObjectToBuild.CreateFromObjectBuilder(buildingQueue.ObjectBuilder, buildingQueue.Amount));
                }
            }                                    
        }

        protected override void SetHudMarker()
        {
            MyHud.ChangeText(this, new StringBuilder(DisplayName), MyGuitargetMode.Neutral, 0, MyHudIndicatorFlagsEnum.SHOW_TEXT | MyHudIndicatorFlagsEnum.SHOW_BORDER_INDICATORS | MyHudIndicatorFlagsEnum.SHOW_ONLY_IF_DETECTED_BY_RADAR | MyHudIndicatorFlagsEnum.SHOW_HEALTH_BARS | MyHudIndicatorFlagsEnum.SHOW_DISTANCE | MyHudIndicatorFlagsEnum.ALPHA_CORRECTION_BY_DISTANCE | MyHudIndicatorFlagsEnum.SHOW_MISSION_MARKER);
        }

        protected override StringBuilder GetDisplayNameSb(string displayName)
        {
            StringBuilder displayNameSb;
            if (!string.IsNullOrEmpty(displayName))
            {
                displayNameSb = base.GetDisplayNameSb(displayName);
            }
            else 
            {
                if (!string.IsNullOrEmpty(m_owner.DisplayName))
                {
                    displayNameSb = new StringBuilder(m_owner.DisplayName);
                    displayNameSb.Append(" (");
                    displayNameSb.Append(MyTextsWrapper.Get(MyTextsWrapperEnum.FoundationFactory));
                    displayNameSb.Append(")");
                }
                else
                {
                    displayNameSb = MyTextsWrapper.Get(MyTextsWrapperEnum.FoundationFactory);
                }
            }
            return displayNameSb;
        }

        protected override void UpdatePrefabBeforeSimulation()
        {
            base.UpdatePrefabBeforeSimulation();

            if (IsBuilding)
            {
                m_buildingTimeFromStart += MyConstants.PHYSICS_STEP_SIZE_IN_MILLISECONDS;
                if (m_buildingTimeFromStart >= BuildingObject.BuildingSpecification.BuildingTime)
                {
                    CompleteBuilding();
                }
            }            
        }

        protected override MyMwcObjectBuilder_Base GetObjectBuilderInternal(bool getExactCopy)
        {            
            MyMwcObjectBuilder_PrefabFoundationFactory ffBuilder = base.GetObjectBuilderInternal(getExactCopy) as MyMwcObjectBuilder_PrefabFoundationFactory;            
            ffBuilder.BuildingObject = BuildingObject != null ? new MyMwcObjectBuilder_ObjectToBuild(BuildingObject.ObjectBuilder, BuildingObject.Amount) : null;
            ffBuilder.IsBuilding = IsBuilding;
            ffBuilder.BuildingTimeFromStart = m_buildingTimeFromStart;                
            ffBuilder.BuildingQueue = new List<MyMwcObjectBuilder_ObjectToBuild>();                
            foreach (MyObjectToBuild objectToBuild in BuildingQueue)
            {
                ffBuilder.BuildingQueue.Add(new MyMwcObjectBuilder_ObjectToBuild(objectToBuild.ObjectBuilder, objectToBuild.Amount));
            }                                            
            return ffBuilder;
        }

        public override string GetFriendlyName()
        {
            return "MyPrefabFoundationFactory";
        }

        protected override bool PrefabNeedsUpdateNow
        {
            get
            {
                return base.PrefabNeedsUpdateNow && IsBuilding;
            }
        }

        #endregion
    }
}
