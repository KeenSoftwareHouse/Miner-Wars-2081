using System.Diagnostics;
using MinerWars.AppCode.Game.Utils;

namespace MinerWars.AppCode.Game.Entities
{
    using System.Text;
    using CommonLIB.AppCode.ObjectBuilders;
    using MinerWarsMath;
    using Models;
    using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
    using Inventory;
    using World.Global;
    using System.Reflection;
    using KeenSoftwareHouse.Library.Extensions;
    using MinerWars.AppCode.Game.Missions;
using System;

    /// <summary>
    /// Represent base class for all ships.
    /// </summary>
    internal abstract class MyShip: MyEntity, IMyInventory
    {
        private MyInventory m_inventory;

        /// <summary>
        /// Gets the inventory.
        /// </summary>
        public MyInventory Inventory 
        {
            get 
            {
                return m_inventory;
            }
            private set 
            {
                m_inventory = value;
            }
        }

        public event Action<MyShip> InventoryChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="MyEntity"/> class.
        /// </summary>
        protected MyShip()
        {
            this.Faction = MyMwcObjectBuilder_FactionEnum.Euroamerican;
            Inventory = new MyInventory();
            Inventory.OnInventoryContentChange += OnInventoryContentChange;
            Inventory.OnInventoryItemAmountChange += OnInventoryItemAmountChange;
            Inventory.OnInventoryContentChange += new AppCode.Game.Inventory.OnInventoryContentChange(Inventory_OnInventoryContentChange);
        }

        void Inventory_OnInventoryContentChange(MyInventory sender)
        {
            RaiseInventoryChanged();
        }

        void RaiseInventoryChanged()
        {
            var handler = InventoryChanged;
            if (handler != null)
            {
                handler(this);
            }
        }

        /// <summary>
        /// Inits the specified hud label text.
        /// </summary>
        /// <param name="hudLabelText">The hud label text.</param>
        /// <param name="modelLod0Enum">The model lod0 enum.</param>
        /// <param name="modelLod1Enum">The model lod1 enum.</param>
        /// <param name="parentObject">The parent object.</param>
        /// <param name="scale">The scale.</param>
        /// <param name="objectBuilder">The object builder.</param>
        public override void Init(StringBuilder hudLabelText, MyModelsEnum? modelLod0Enum, MyModelsEnum? modelLod1Enum, MyEntity parentObject, float? scale, MyMwcObjectBuilder_Base objectBuilder, MyModelsEnum? modelCollision = null, Models.MyModelsEnum? modelLod2 = null)
        {
            Flags |= EntityFlags.EditableInEditor;
            NeedsUpdate = true;

            base.Init(hudLabelText, modelLod0Enum, modelLod1Enum, null, scale, objectBuilder, modelCollision, modelLod2);
        }

        public virtual void Init(string hudLabelText, MyMwcObjectBuilder_Ship shipObjectBuilder)
        {
            Debug.Assert(shipObjectBuilder != null);

            this.Faction = shipObjectBuilder.Faction;

            if (shipObjectBuilder.Inventory != null)
            {
                Inventory.Init(shipObjectBuilder.Inventory);
            }
            else 
            {
                OnInventoryContentChange(Inventory);
            }
        }

        protected override MyMwcObjectBuilder_Base GetObjectBuilderInternal(bool getExactCopy)
        {
            MyMwcObjectBuilder_Base baseBuilder = base.GetObjectBuilderInternal(getExactCopy);
            var builder = baseBuilder as MyMwcObjectBuilder_Ship;
            if(builder != null)
            {                
                builder.Inventory = Inventory.GetObjectBuilder(getExactCopy);
                builder.Faction = Faction;
                return builder;
            }
            return baseBuilder;
        }

        protected virtual void OnInventoryContentChange(MyInventory sender)
        {
            MyScriptWrapper.OnEntityInventoryContentChange(this, sender);
        }

        protected virtual void OnInventoryItemAmountChange(MyInventory sender, MyInventoryItem inventoryItem, float amountChanged)
        {
            MyScriptWrapper.OnEntityInventoryAmountChange(this, sender, inventoryItem, amountChanged);
        }

        /// <summary>
        /// Every object must have this method, but not every phys object must necessarily have something to cleanup
        /// </summary>
        public override void Close()
        {
            Inventory.OnInventoryContentChange -= OnInventoryContentChange;
            Inventory.OnInventoryItemAmountChange -= OnInventoryItemAmountChange;
            Inventory.Close();            

            base.Close();

            //TODO this is now causing crash when destroying bot, will solve later
            //if (this.Physics != null)
            //    this.Physics.RemoveAllElements();
        }        
    }
}