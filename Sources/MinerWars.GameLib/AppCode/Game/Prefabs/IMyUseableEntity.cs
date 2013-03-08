using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.AppCode.Game.GUI.Prefabs;
using MinerWars.AppCode.Game.GUI.Core;
using System.Diagnostics;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.Prefabs;
using MinerWars.AppCode.Game.Localization;

namespace MinerWars.AppCode.Game.Entities.Prefabs
{
    [Flags]
    enum MyUseType 
    {
        None = 0,
        Solo = 1 << 0,
        FromHUB = 1 << 1,        
    }

    interface IMyUseableEntity 
    {
        MyUseProperties UseProperties { get; set; }
        bool CanBeUsed(MySmallShip usedBy);
        bool CanBeHacked(MySmallShip hackedBy);
        void Use(MySmallShip useBy);
        void UseFromHackingTool(MySmallShip useBy, int hackingLevelDifference);        
    }

    interface IMyHasGuiControl
    {
        MyGuiControlEntityUse GetGuiControl(IMyGuiControlsParent parent);
        MyEntity GetEntity();
    }

    class MyUseProperties 
    {
        private MyUseType m_useType;
        public MyUseType UseType 
        {
            get { return m_useType; }
            set 
            {
                m_useType = value & UseMask;
            }
        }
        private MyUseType m_hackType;
        public MyUseType HackType 
        {
            get { return m_hackType; }
            set 
            {
                m_hackType = value & HackMask;
            }
        }

        public int HackingLevel { get; set; }
        public int HackingTime { get; set; }
        public bool IsHacked { get; set; }

        public MyUseType UseMask { get; private set; }
        public MyUseType HackMask { get; private set; }

        public MyTextsWrapperEnum? UseText { get; set; }

        public MyUseProperties(MyUseType useMask, MyUseType hackMask, MyTextsWrapperEnum? useText = null) 
        {
            UseMask = useMask;
            HackMask = hackMask;
            UseText = useText;
        }        

        public void Init(MyUseType useType, MyUseType hackType, int hackingLevel, int hackingTime, bool isHacked) 
        {
            UseType = useType;
            HackType = hackType;
            HackingLevel = hackingLevel;
            HackingTime = hackingTime;
            IsHacked = isHacked;            
        }

        public void Init(MyMwcObjectBuilder_EntityUseProperties builder) 
        {
            Init((MyUseType)builder.UseType, (MyUseType)builder.HackType, builder.HackingLevel, builder.HackingTime, builder.IsHacked);
        }

        public MyMwcObjectBuilder_EntityUseProperties GetObjectBuilder() 
        {
            MyMwcObjectBuilder_EntityUseProperties builder = MyMwcObjectBuilder_Base.CreateNewObject(MyMwcObjectBuilderTypeEnum.EntityUseProperties, null) as MyMwcObjectBuilder_EntityUseProperties;

            builder.UseType = (int)UseType;
            builder.HackType = (int)HackType;
            builder.HackingLevel = HackingLevel;
            builder.HackingTime = HackingTime;
            builder.IsHacked = IsHacked;

            return builder;
        }
    }
}
