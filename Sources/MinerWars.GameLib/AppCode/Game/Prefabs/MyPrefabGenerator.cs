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
using MinerWars.AppCode.Game.GUI.Prefabs;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Editor;
using MinerWars.AppCode.Game.World.Global;

namespace MinerWars.AppCode.Game.Prefabs
{
    class MyPrefabGenerator : MyPrefabBase, IMyUseableEntity, IMyHasGuiControl
    {
        public MyPrefabGenerator(MyPrefabContainer owner)
            : base(owner)
        {
        }        

        protected override void InitPrefab(string displayName, Vector3 relativePosition, Matrix localOrientation, MyMwcObjectBuilder_PrefabBase objectBuilder, MyPrefabConfiguration prefabConfig)
        {
            MyMwcObjectBuilder_PrefabGenerator objectBuilderGenerator = objectBuilder as MyMwcObjectBuilder_PrefabGenerator;
            UseProperties = new MyUseProperties(MyUseType.FromHUB | MyUseType.Solo, MyUseType.FromHUB | MyUseType.Solo);
            if (objectBuilder.UseProperties == null)
            {
                UseProperties.Init(MyUseType.FromHUB | MyUseType.Solo, MyUseType.FromHUB | MyUseType.Solo, 1, 4000, false);                
            }
            else
            {
                UseProperties.Init(objectBuilder.UseProperties);
            }
        }

        protected override void SetHudMarker()
        {
            MyHud.ChangeText(this, new StringBuilder(DisplayName), MyGuitargetMode.Neutral, 0, MyHudIndicatorFlagsEnum.SHOW_TEXT | MyHudIndicatorFlagsEnum.SHOW_BORDER_INDICATORS | MyHudIndicatorFlagsEnum.SHOW_HEALTH_BARS | MyHudIndicatorFlagsEnum.SHOW_DISTANCE | MyHudIndicatorFlagsEnum.ALPHA_CORRECTION_BY_DISTANCE | MyHudIndicatorFlagsEnum.SHOW_MISSION_MARKER);
        }

        protected override void WorkingChanged()
        {
            base.WorkingChanged();
            m_owner.UpdateGenerators();
        }

        protected override MyMwcObjectBuilder_Base GetObjectBuilderInternal(bool getExactCopy)
        {
            MyMwcObjectBuilder_PrefabGenerator objectBuilder = base.GetObjectBuilderInternal(getExactCopy) as MyMwcObjectBuilder_PrefabGenerator;
            objectBuilder.UseProperties = UseProperties.GetObjectBuilder();
            return objectBuilder;
        }

        public override string GetFriendlyName()
        {
            return "MyPrefabGenerator";
        }

        public float GetRange()
        {
            var config = m_config as MyPrefabConfigurationGenerator;
            return config != null ? config.Range : 0;
        }

        public override bool MoveAndRotate(Vector3 moveIndicator, Matrix orientation)
        {
            if (m_owner != null)
            {
                m_owner.UpdateGenerators();
            }

            return base.MoveAndRotate(moveIndicator, orientation);
        }

        public override void Close()
        {
            base.Close();

            if (m_owner != null)
            {
                m_owner.UpdateGenerators();
            }

        }        

        public void DebugDrawRange()
        {
            Matrix world = Matrix.CreateWorld(WorldMatrix.Translation, WorldMatrix.Forward, WorldMatrix.Up);
            Vector4 color = Color.Blue.ToVector4();
            color.W *= 0.1f;
            MySimpleObjectDraw.DrawTransparentSphere(ref world, GetRange(), ref color, true, 24);         
        }

        public MyGuiControlEntityUse GetGuiControl(IMyGuiControlsParent parent)
        {
            return new MyGuiControlPrefabUse(parent, this);
        }

        public MyEntity GetEntity()
        {
            return this;
        }

        public MyUseProperties UseProperties { get; set; }

        public bool CanBeUsed(MySmallShip usedBy)
        {
            return MyFactions.GetFactionsRelation(usedBy, this) == MyFactionRelationEnum.Friend || UseProperties.IsHacked;
        }

        public bool CanBeHacked(MySmallShip hackedBy)
        {
            return MyFactions.GetFactionsRelation(hackedBy, this) == MyFactionRelationEnum.Neutral ||
                MyFactions.GetFactionsRelation(hackedBy, this) == MyFactionRelationEnum.Enemy;
        }

        public void Use(MySmallShip useBy)
        {
            MyGuiManager.AddScreen(new MyGuiScreenEntityUseSolo(this));
        }

        public void UseFromHackingTool(MySmallShip useBy, int hackingLevelDifference)
        {
            Use(useBy);
        }
    }
}
