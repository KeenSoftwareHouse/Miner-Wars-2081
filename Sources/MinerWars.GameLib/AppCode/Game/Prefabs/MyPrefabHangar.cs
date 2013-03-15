using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.HUD;
using MinerWars.AppCode.Game.Inventory;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Entities.Prefabs;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Missions;
using MinerWars.AppCode.Game.Prefabs;
using MinerWars.AppCode.Game.Render;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.World.Global;
using MinerWars.AppCode.Physics;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.Prefabs;
using SysUtils.Utils;
using MinerWars.AppCode.Game.TransparentGeometry;

namespace MinerWars.AppCode.Game.Entities.Prefabs
{
    class MyPrefabHangar : MyPrefabBase, IMyInventory, IMyUseableEntity
    {        
        //private BoundingBox m_localBoundingBox;

        public MyPrefabHangar(MyPrefabContainer owner) 
            : base(owner)
        {                        
        }                

        protected override void InitPrefab(string displayName, Vector3 relativePosition, Matrix localOrientation, MyMwcObjectBuilder_PrefabBase objectBuilder, MyPrefabConfiguration prefabConfig)
        {
            //Physics.RemoveAllElements();

            //float raidus = ModelLod0.BoundingSphere.Radius;
            //Vector3 min = new Vector3(-raidus, -raidus, -raidus);
            //Vector3 max = new Vector3(raidus, raidus, raidus);
            //m_localBoundingBox = new BoundingBox(min, max);            

            UseProperties = new MyUseProperties(MyUseType.None, MyUseType.Solo);
            if (objectBuilder.UseProperties == null)
            {
                UseProperties.Init(MyUseType.None, MyUseType.None, 3, 8000, false);
            }
            else
            {
                UseProperties.Init(objectBuilder.UseProperties);
            }
        }

        public override string GetCorrectDisplayName()
        {
            string displayName = DisplayName;

            if (DisplayName == "Mixed Merchant")
            {
                displayName = MyTextsWrapper.Get(MyTextsWrapperEnum.MerchantMixed).ToString();
            }


            return displayName;
        }

        protected override void SetHudMarker()
        {
            MyHudIndicatorFlagsEnum hudFlags = MyHudIndicatorFlagsEnum.SHOW_TEXT | MyHudIndicatorFlagsEnum.SHOW_BORDER_INDICATORS | MyHudIndicatorFlagsEnum.SHOW_HEALTH_BARS | MyHudIndicatorFlagsEnum.SHOW_DISTANCE | MyHudIndicatorFlagsEnum.SHOW_MISSION_MARKER;
            MyGuitargetMode? guiTargetMode = MyGuitargetMode.Neutral;
            if(PrefabHangarType == MyMwcObjectBuilder_PrefabHangar_TypesEnum.HANGAR && 
               MyFactions.GetFactionsRelation(this.m_owner, MySession.PlayerShip) == MyFactionRelationEnum.Friend)
            {
                guiTargetMode = MyGuitargetMode.Friend;
            }
            else
            {
                hudFlags |= MyHudIndicatorFlagsEnum.SHOW_ONLY_IF_DETECTED_BY_RADAR | MyHudIndicatorFlagsEnum.ALPHA_CORRECTION_BY_DISTANCE;
            }


            MyHud.ChangeText(this, new StringBuilder(GetCorrectDisplayName()), guiTargetMode, 0, hudFlags); 
        }

        protected override StringBuilder GetDisplayNameSb(string displayName)
        {
            StringBuilder displayNameSb = null;
            if (!string.IsNullOrEmpty(displayName))
            {
                displayNameSb = base.GetDisplayNameSb(displayName);
            }
            else 
            {
                if (!string.IsNullOrEmpty(m_owner.DisplayName))
                {
                    displayNameSb = new StringBuilder(m_owner.DisplayName);
                    //displayNameSb.Append(" (");
                    //displayNameSb.Append(MyTextsWrapper.Get(MyTextsWrapperEnum.vendor));
                    //displayNameSb.Append(")");
                }
                else
                {
                    if (PrefabHangarType == MyMwcObjectBuilder_PrefabHangar_TypesEnum.HANGAR)
                    {
                        displayNameSb = MyTextsWrapper.Get(MyTextsWrapperEnum.Hangar);
                    }
                    else if(PrefabHangarType == MyMwcObjectBuilder_PrefabHangar_TypesEnum.VENDOR)
                    {
                        displayNameSb = MyTextsWrapper.Get(MyTextsWrapperEnum.Merchant);
                    }                    
                }
            }
            return displayNameSb;
        }

        protected override MyMwcObjectBuilder_Base GetObjectBuilderInternal(bool getExactCopy)
        {
            MyMwcObjectBuilder_PrefabHangar objectBuilder = base.GetObjectBuilderInternal(getExactCopy) as MyMwcObjectBuilder_PrefabHangar;
            objectBuilder.UseProperties = UseProperties.GetObjectBuilder();

            return objectBuilder;
        }

        public override float GetHUDDamageRatio()
        {
            List<MyPrefabBase> largeShips = GetOwner().GetPrefabs(CategoryTypesEnum.LARGE_SHIPS);
            if (largeShips.Count > 0)
            {
                return largeShips[0].GetHUDDamageRatio();
            }
            else 
            {
                return base.GetHUDDamageRatio();
            }            
        }

        public override bool Draw(MyRenderObject renderObject)
        {
            if (!base.Draw(renderObject))
                return false;

            //if (IsWorking())
            /*{
                Matrix world = Matrix.CreateWorld(WorldMatrix.Translation + ModelLod0.BoundingSphere.Center, WorldMatrix.Forward, WorldMatrix.Up);
                MyTransparentMaterialEnum? faceMaterial;
                MyTransparentMaterialEnum? lineMaterial;
                Vector4 color;
                SetTransparentParameters(out color, out faceMaterial, out lineMaterial);
                if (faceMaterial.HasValue)
                {
                    BoundingBox localBoundingBox = m_localBoundingBox;
                    MySimpleObjectDraw.DrawTransparentBox(ref world, ref localBoundingBox, ref color, true, 1, faceMaterial, lineMaterial);
                }
            }*/

            return true;
        }

        private void SetTransparentParameters(out Vector4 color, out MyTransparentMaterialEnum? faceMaterial, out MyTransparentMaterialEnum? lineMaterial)
        {
            if (Game.Editor.MyEditor.Static.IsActive())
            {
                faceMaterial = MyTransparentMaterialEnum.ObjectiveDummyFace;
                lineMaterial = MyTransparentMaterialEnum.ObjectiveDummyLine;
            }
            else 
            {
                faceMaterial = null;
                lineMaterial = null;
            }

            if(MyMissions.IsMissionEntity(this))
            {                
                color = MyHudConstants.MISSION_CUBE_COLOR;
            }
            else
            {
                MyFactionRelationEnum status = MyFactions.GetFactionsRelation(this, MySession.PlayerShip);
                switch(status) {
                    case MyFactionRelationEnum.Friend:
                        color = MyHudConstants.FRIEND_CUBE_COLOR;
                        break;
                    case MyFactionRelationEnum.Enemy:
                        color = MyHudConstants.ENEMY_CUBE_COLOR;
                        break;
                    case MyFactionRelationEnum.Neutral:
                        color = MyHudConstants.NEUTRAL_CUBE_COLOR;
                        break;
                    default:
                        throw new MyMwcExceptionApplicationShouldNotGetHere();
                }                
            }
        }        

        public MyInventory Inventory
        {
            get
            {
                return m_owner.Inventory;
            }            
        }

        public MyMwcObjectBuilder_PrefabHangar_TypesEnum PrefabHangarType
        {
            get
            {
                return (MyMwcObjectBuilder_PrefabHangar_TypesEnum)m_prefabId;
            }
            set
            {
                m_prefabId = (int)value;
            }
        }

        public override string GetFriendlyName()
        {
            return "MyPrefabHangar";
        }

        public bool CanBeHacked(MySmallShip hackedBy)
        {
            return IsWorking() && 
                   PrefabHangarType == MyMwcObjectBuilder_PrefabHangar_TypesEnum.VENDOR &&
                   MyFactions.GetFactionsRelation(hackedBy, this) == MyFactionRelationEnum.Enemy;
        }
        public bool CanBeUsed(MySmallShip usedBy)
        {
            return false;
        }

        public void Use(MySmallShip useBy)
        {
            throw new NotSupportedException();
        }

        public void UseFromHackingTool(MySmallShip useBy, int hackingLevelDifference)
        {
            return;
        }

        public MyUseProperties UseProperties { get; set; }
    }
}
