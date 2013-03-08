using System;
using System.Text;
using MinerWars.AppCode.Game.GUI.Helpers;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.Gameplay;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.CommonLIB.AppCode.Utils;

namespace MinerWars.AppCode.Game.Inventory
{
    delegate void OnAmountChange(MyInventoryItem sender, float amountChanged);

    class MyInventoryItem
    {
        #region Fields
        float m_amount;
        StringBuilder m_multiLineDescription;
        #endregion

        #region Ctors
        public MyInventoryItem()
        {
            m_multiLineDescription = new StringBuilder();
        }
        #endregion

        #region Methods
        public void Start(MyGuiHelperBase guiHelper, MyGameplayProperties itemProperties, MyMwcObjectBuilder_Base objectBuilder)
        {
            Start(guiHelper, itemProperties, objectBuilder, 1f);
        }

        public void Start(MyGuiHelperBase guiHelper, MyGameplayProperties itemProperties, MyMwcObjectBuilder_Base objectBuilder, float amount) 
        {
            GuiHelper = guiHelper;
            ItemProperties = itemProperties;
            ObjectBuilder = objectBuilder;

            Amount = Math.Min(amount, MaxAmount);
            TemporaryFlags = MyInventoryItemTemporaryFlags.NONE;                        
        }        

        public override string ToString()
        {
            return GuiHelper.Description + " Amount:" + Amount.ToString() + " Price:" + Price.ToString() + " Weight:" + Weight.ToString();
        }

        #endregion

        #region Events
        public OnAmountChange OnAmountChange;

        #endregion

        #region Properties
        public MyInventoryItemTemporaryFlags TemporaryFlags { get; set; }

        public object Owner { get; set; }

        public MyMwcObjectBuilder_Base ObjectBuilder { get; set; }        

        public float Amount 
        {
            get 
            { 
                return m_amount; 
            }
            set 
            {
                if (value <= ItemProperties.MaxAmount)
                {
                    float amountChanged = value - m_amount;
                    m_amount = value;

                    if (amountChanged != 0 && OnAmountChange != null) 
                    {
                        OnAmountChange(this, amountChanged);
                    }
                }
                else 
                {
                    throw new Exception("Value is greater then max amount of item");
                }
            }
        }        

        public MyGuiHelperBase GuiHelper { get; set; }

        public MyGameplayProperties ItemProperties { get; set; }

        public float MaxAmount 
        {
            get { return ItemProperties.MaxAmount; }
        }

        public float Price
        {
            get { return Amount * ItemProperties.PricePerUnit; }
        }

        public float PricePerUnit
        {
            get { return ItemProperties.PricePerUnit; }
        }

        public float Weight
        {
            get { return Amount * ItemProperties.WeightPerUnit; }
        }

        public float WeightPerUnit
        {
            get { return ItemProperties.WeightPerUnit; }
        }

        public StringBuilder Description
        {
            get { return GuiHelper.Description; }
        }

        public StringBuilder MultiLineDescription 
        {
            get { return GuiHelper.MultiLineDescription; }
        }

        public MyTexture2D Icon
        {
            get { return GuiHelper.Icon; }
        }

        public MyMwcObjectBuilder_InventoryItem GetObjectBuilder() 
        {
            MyMwcObjectBuilder_InventoryItem builder = new MyMwcObjectBuilder_InventoryItem(ObjectBuilder, Amount);
            builder.TemporaryFlags = TemporaryFlags;
            return builder;
        }

        public MyMwcObjectBuilder_Base GetInventoryItemObjectBuilder(bool getExactCopy) 
        {
            if (getExactCopy)
            {
                return ObjectBuilder.Clone();
            }
            else
            {
                return ObjectBuilder;
            }
        }

        public MyMwcObjectBuilderTypeEnum ObjectBuilderType 
        {
            get { return ObjectBuilder.GetObjectBuilderType(); }
        }

        public int? ObjectBuilderId 
        {
            get { return ObjectBuilder.GetObjectBuilderId(); }
        }

        public MyInventoryAmountTextAlign AmountTextAlign
        {
            get { return GuiHelper.InventoryTextAlign; }
        }

        public bool CanBeTraded
        {
            get { return (ObjectBuilder.PersistentFlags & MyPersistentEntityFlags.NotTradeable) == 0; }
        }

        public bool CanBeMoved 
        {
            get { return (TemporaryFlags & MyInventoryItemTemporaryFlags.CANT_BE_MOVED) == 0; }
            set
            {
                if (value)
                {
                    TemporaryFlags &= ~MyInventoryItemTemporaryFlags.CANT_BE_MOVED;
                }
                else 
                {
                    TemporaryFlags |= MyInventoryItemTemporaryFlags.CANT_BE_MOVED;
                }
            }
        }

        public bool IsTemporaryItem
        {
            get { return (TemporaryFlags & MyInventoryItemTemporaryFlags.TEMPORARY_ITEM) != 0; }
            set
            {
                if (value)
                {
                    TemporaryFlags |= MyInventoryItemTemporaryFlags.TEMPORARY_ITEM;
                }
                else
                {
                    TemporaryFlags &= ~MyInventoryItemTemporaryFlags.TEMPORARY_ITEM;
                }
            }
        }

        public bool NotEnoughMoney
        {
            get { return (TemporaryFlags & MyInventoryItemTemporaryFlags.NOT_ENOUGH_MONEY) != 0; }
            set
            {
                if (value)
                {
                    TemporaryFlags |= MyInventoryItemTemporaryFlags.NOT_ENOUGH_MONEY;
                }
                else
                {
                    TemporaryFlags &= ~MyInventoryItemTemporaryFlags.NOT_ENOUGH_MONEY;
                }
            }
        }

        #endregion        
    }
}
