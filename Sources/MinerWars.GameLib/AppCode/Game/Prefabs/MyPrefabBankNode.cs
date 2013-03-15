using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Entities;
using MinerWarsMath;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.Prefabs;
using MinerWars.AppCode.Game.HUD;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.AppCode.Game.Entities.Prefabs;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Localization;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.GUI.Core;

namespace MinerWars.AppCode.Game.Prefabs
{
    class MyPrefabBankNode : MyPrefabBase, IMyUseableEntity
    {
        public float Cash { get; private set; }

        public MyPrefabBankNode(MyPrefabContainer owner) 
            : base(owner)
        {            
        }                

        protected override void InitPrefab(string displayName, Vector3 relativePosition, Matrix localOrientation, MyMwcObjectBuilder_PrefabBase objectBuilder, MyPrefabConfiguration prefabConfig)
        {
            MyMwcObjectBuilder_PrefabBankNode objectBuilderBankNode = objectBuilder as MyMwcObjectBuilder_PrefabBankNode;
            Cash = objectBuilderBankNode.Cash;
            UseProperties = new MyUseProperties(MyUseType.None, MyUseType.Solo);
            if (objectBuilderBankNode.UseProperties == null)
            {
                UseProperties.Init(MyUseType.None, MyUseType.Solo, 3, 4000, false);                
            }
            else
            {                
                UseProperties.Init(objectBuilderBankNode.UseProperties);
            }
            // some default cash for testing
            if (!UseProperties.IsHacked)
            {
                Cash = 8000f;
            }            
        }

        public override string GetCorrectDisplayName()
        {
            string displayName = DisplayName;

            if (DisplayName == "BankNode")
                displayName = MyTextsWrapper.Get(MyTextsWrapperEnum.BankNode).ToString();

            return displayName;
        }

        protected override void SetHudMarker()
        {
            MyHud.ChangeText(this, new StringBuilder(DisplayName), MyGuitargetMode.Neutral, 0, MyHudIndicatorFlagsEnum.SHOW_TEXT | MyHudIndicatorFlagsEnum.SHOW_BORDER_INDICATORS | MyHudIndicatorFlagsEnum.SHOW_HEALTH_BARS | MyHudIndicatorFlagsEnum.SHOW_DISTANCE | MyHudIndicatorFlagsEnum.ALPHA_CORRECTION_BY_DISTANCE | MyHudIndicatorFlagsEnum.SHOW_MISSION_MARKER);            
        }

        protected override MyMwcObjectBuilder_Base GetObjectBuilderInternal(bool getExactCopy)
        {
            MyMwcObjectBuilder_PrefabBankNode objectBuilder = base.GetObjectBuilderInternal(getExactCopy) as MyMwcObjectBuilder_PrefabBankNode;

            objectBuilder.Cash = Cash;
            objectBuilder.UseProperties = UseProperties.GetObjectBuilder();

            return objectBuilder;
        }

        public MyMwcObjectBuilder_PrefabBankNode_TypesEnum PrefabBankNodeType
        {
            get
            {
                return (MyMwcObjectBuilder_PrefabBankNode_TypesEnum)m_prefabId;
            }
            set
            {
                m_prefabId = (int)value;
            }
        }

        public override string GetFriendlyName()
        {
            return "MyPrefabBankNode";
        }

        public MyUseProperties UseProperties
        {
            get; set;
        }

        public bool CanBeUsed(MySmallShip usedBy)
        {
            return false;
        }

        public bool CanBeHacked(MySmallShip hackedBy)
        {
            return IsWorking();
        }

        public void Use(MySmallShip useBy)
        {
            throw new NotImplementedException();
        }

        public void UseFromHackingTool(MySmallShip useBy, int hackingLevelDifference)
        {
            float cashMoveToPlayer = Cash * (0.5f + hackingLevelDifference * 0.1f);
            MySession.Static.Player.Money += cashMoveToPlayer;
            Cash -= cashMoveToPlayer;
            MyHudNotification.AddNotification(
                new MyHudNotification.MyNotification(MyTextsWrapperEnum.YouObtainNotification, MyGuiManager.GetFontMinerWarsGreen(), 5000, null, new object[] { MyMwcUtils.GetFormatedPriceForGame((decimal)cashMoveToPlayer) }));
        }
    }
}
