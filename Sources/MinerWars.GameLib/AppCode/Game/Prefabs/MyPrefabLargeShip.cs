using System;
using System.Collections.Generic;
using MinerWarsMath;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Lights;
using MinerWars.AppCode.Game.Entities.Prefabs;
using MinerWars.AppCode.Game.Entities.Tools;
using MinerWars.AppCode.Game.Models;
using MinerWars.AppCode.Game.Prefabs;
using MinerWars.AppCode.Game.TransparentGeometry;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.Prefabs;
using MinerWars.CommonLIB.AppCode.Utils;

using MinerWars.AppCode.Game.Gameplay;
using MinerWars.AppCode.Game.Localization;
using System.Text;
using MinerWars.AppCode.Game.Inventory;
using MinerWars.AppCode.Game.HUD;
using MinerWars.AppCode.Game.Render;
using MinerWars.AppCode.Game.Audio;

namespace MinerWars.AppCode.Game.Entities.Prefabs
{
    class MyPrefabLargeShip : MyPrefabBase
    {
        private MyNanoRepairToolEntity m_nanoRepairTool;

        public MyPrefabLargeShip(MyPrefabContainer owner) : 
            base(owner) { }
        
        public override void Init(string hudLabelText, Vector3 relativePosition, Matrix localOrientation, MyMwcObjectBuilder_PrefabBase objectBuilder, MyPrefabConfiguration prefabConfig)
        {
            MyMwcObjectBuilder_PrefabLargeShip objectBuilderLargeShip = objectBuilder as MyMwcObjectBuilder_PrefabLargeShip;            
            
            m_owner.Inventory.OnInventoryContentChange += OnInventoryContentChanged;

            Name = objectBuilderLargeShip.Name;

            base.Init(Name, relativePosition, localOrientation, objectBuilder, prefabConfig);
        }

        protected override void InitPrefab(string displayName, Vector3 relativePosition, Matrix localOrientation, MyMwcObjectBuilder_PrefabBase objectBuilder, MyPrefabConfiguration prefabConfig)
        {
        }

        public override void Close()
        {
            base.Close();
            m_owner.Inventory.OnInventoryContentChange -= OnInventoryContentChanged;
        }

        protected override MyMwcObjectBuilder_Base GetObjectBuilderInternal(bool getExactCopy)
        {
            MyMwcObjectBuilder_PrefabLargeShip objectBuilder = base.GetObjectBuilderInternal(getExactCopy) as MyMwcObjectBuilder_PrefabLargeShip;
            
            objectBuilder.Name = Name;

            return objectBuilder;
        }

        public override bool Draw(MyRenderObject renderObject)
        {
            if (!base.Draw(renderObject))
                return false;            

            return true;
        }

        private void OnInventoryContentChanged(MyInventory sender)
        {
            if (sender.Contains(MyMwcObjectBuilderTypeEnum.SmallShip_Tool, (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.NANO_REPAIR_TOOL))
            {
                if (m_nanoRepairTool == null)
                {
                    m_nanoRepairTool = new MyNanoRepairToolEntity(this);                    
                }
            }
            else
            {
                m_nanoRepairTool = null;                
            }
            RecheckNeedsUpdate();
        }

        protected override void UpdatePrefabBeforeSimulation()
        {
            base.UpdatePrefabBeforeSimulation();

            if (m_nanoRepairTool != null)
            {
                m_nanoRepairTool.Use();
            }
        }

        public override void UpdateAfterSimulation()
        {
            base.UpdateAfterSimulation();
        }

        public MyMwcObjectBuilder_PrefabLargeShip_TypesEnum PrefabLargeShipType
        {
            get
            {
                return (MyMwcObjectBuilder_PrefabLargeShip_TypesEnum)m_prefabId;
            }
            set
            {
                m_prefabId = (int)value;
            }
        }

        public override string GetFriendlyName()
        {
            return "MyPrefabLargeShip";
        }
    }
}
