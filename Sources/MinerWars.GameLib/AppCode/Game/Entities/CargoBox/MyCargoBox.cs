using System;
using System.Text;
using MinerWars.AppCode.Game.Inventory;
using MinerWarsMath;
using MinerWars.AppCode.Game.TransparentGeometry.Particles;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.AppCode.Game.Models;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Physics;
using MinerWars.AppCode.Game.HUD;
using MinerWars.AppCode.Game.Audio;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.AppCode.Game.Sessions;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.Localization;

namespace MinerWars.AppCode.Game.Entities.CargoBox
{
    class MyCargoBox : MyEntity, IMyInventory, IResetable
    {
        public MyInventory Inventory { get; set; }

        private float m_elapsedTime;
        public TimeSpan? RespawnTime;
        private bool m_shouldRespawn;

        /// <summary>
        /// Indicates whether this cargo box should disappear after all items have been taken from it.
        /// </summary>
        public bool CloseAfterEmptied
        {
            get { return m_cargoBoxType == MyMwcObjectBuilder_CargoBox_TypesEnum.DroppedItems; }
        }

        private MyMwcObjectBuilder_CargoBox_TypesEnum m_cargoBoxType;
        public MyMwcObjectBuilder_CargoBox_TypesEnum CargoBoxType
        {
            get { return m_cargoBoxType; }
            set
            {
                m_cargoBoxType = value;
                ChangeModelLod0(value);
            }
        }

        public MyCargoBox()
            : base(true)
        {
            Inventory = new MyInventory();
            Inventory.OnInventoryContentChange += Inventory_OnInventoryContentChange;
        }

        void Inventory_OnInventoryContentChange(MyInventory sender)
        {
            UpdateState();

            if (RespawnTime.HasValue && MyMultiplayerGameplay.IsRunning && !MyGuiScreenGamePlay.Static.IsGameStoryActive())
            {
                if (Inventory.GetInventoryItems().Count < m_inventoryTemplate.InventoryItems.Count)
                {
                    m_shouldRespawn = NeedsUpdate = true;
                }
            }

            if (sender.IsInventoryEmpty() && CloseAfterEmptied)
            {
                MarkForClose();
            }
        }

        private void UpdateState()
        {
            if (IsNotEmpty())
            {
                m_diffuseColor = Vector3.One * 2f;
                m_enableEmissivity = true;
            }
            else
            {
                m_diffuseColor = Vector3.One;
                m_enableEmissivity = false;
            }
            SetHudMarker();
        }

        public override void UpdateBeforeSimulation()
        {
            base.UpdateBeforeSimulation();

            if (m_shouldRespawn)
            {
                m_elapsedTime += MyConstants.PHYSICS_STEP_SIZE_IN_MILLISECONDS;
                if (m_elapsedTime > RespawnTime.Value.TotalMilliseconds)
                {
                    Respawn();
                }
            }
        }

        public bool IsNotEmpty()
        {
            return Inventory != null && Inventory.GetInventoryItems().Count > 0;
        }

        private MyMwcObjectBuilder_Inventory m_inventoryTemplate;

        public void Init(string hudLabelText, MyMwcObjectBuilder_CargoBox objectBuilder, Matrix matrix)
        {
            Flags |= EntityFlags.EditableInEditor;

            StringBuilder hudLabelTextSb = (hudLabelText == null) ? MyTextsWrapper.Get(MyTextsWrapperEnum.CargoBox) : new StringBuilder(hudLabelText);

            // We want to make cargo box "lazy" to prevent rapid movement
            const float cargoBoxMass = 5000.0f;
            const float cargoAngularDamping = 0.5f;

            var modelLod0Enum = GetModelLod0EnumFromType(objectBuilder.CargoBoxType);

            base.Init(hudLabelTextSb, modelLod0Enum, null, null, null, objectBuilder);
            base.InitBoxPhysics(MyMaterialType.METAL, ModelLod0, cargoBoxMass, cargoAngularDamping, MyConstants.COLLISION_LAYER_DEFAULT, RigidBodyFlag.RBF_RBO_STATIC);
            this.Physics.LinearDamping = 0.7f;
            this.Physics.MaxLinearVelocity = 350;
            this.Physics.MaxAngularVelocity = 5;

            m_inventoryTemplate = objectBuilder.Inventory;
            if (m_inventoryTemplate != null)
            {
                Inventory.Init(m_inventoryTemplate);
            }
            CargoBoxType = objectBuilder.CargoBoxType;

            SetWorldMatrix(matrix);

            Save = true;
            this.Physics.Enabled = true;
            UpdateState();
        }

        public void Respawn()
        {
            // Dummy entities won't respawn;
            if (!IsDummy)
            {
                NeedsUpdate = false;
                m_shouldRespawn = false;
                m_elapsedTime = 0;
                if (m_inventoryTemplate != null)
                {
                    Inventory.Init(m_inventoryTemplate);
                }
                if (MyMultiplayerGameplay.IsRunning)
                {
                    MyMultiplayerGameplay.Static.ResetEntity(this);
                }
            }
        }

        public string GetCorrectDisplayName(ref MyHudMaxDistanceMultiplerTypes? maxDistanceMultiplerType)
        {
            string displayName = GetCorrectDisplayName();

            switch (m_cargoBoxType)
            {
                case MyMwcObjectBuilder_CargoBox_TypesEnum.Type7:
                case MyMwcObjectBuilder_CargoBox_TypesEnum.Type3:
                    maxDistanceMultiplerType = MyHudMaxDistanceMultiplerTypes.CargoBoxMedkit;
                    break;
                case MyMwcObjectBuilder_CargoBox_TypesEnum.Type8:
                case MyMwcObjectBuilder_CargoBox_TypesEnum.Type6:
                    maxDistanceMultiplerType = MyHudMaxDistanceMultiplerTypes.CargoBoxAmmo;
                    break;
                case MyMwcObjectBuilder_CargoBox_TypesEnum.TypeProp_A:
                case MyMwcObjectBuilder_CargoBox_TypesEnum.Type9:
                    maxDistanceMultiplerType = MyHudMaxDistanceMultiplerTypes.CargoBoxOxygen;
                    break;
                case MyMwcObjectBuilder_CargoBox_TypesEnum.Type5:
                case MyMwcObjectBuilder_CargoBox_TypesEnum.Type11:
                    maxDistanceMultiplerType = MyHudMaxDistanceMultiplerTypes.CargoBoxFuel;
                    break;
                case MyMwcObjectBuilder_CargoBox_TypesEnum.Type2:
                case MyMwcObjectBuilder_CargoBox_TypesEnum.Type12:
                    maxDistanceMultiplerType = MyHudMaxDistanceMultiplerTypes.CargoBoxRepair;
                    break;
                default:
                    maxDistanceMultiplerType = null;
                    break;
            }

            return displayName;
        }

        public override string GetCorrectDisplayName()
        {
            string displayName = DisplayName;

            switch (m_cargoBoxType)
            {
                case MyMwcObjectBuilder_CargoBox_TypesEnum.Type7:
                case MyMwcObjectBuilder_CargoBox_TypesEnum.Type3:
                    if (DisplayName == "Medikit")
                        displayName = MyTextsWrapper.Get(MyTextsWrapperEnum.CargoBoxMedikit).ToString();
                    break;
                case MyMwcObjectBuilder_CargoBox_TypesEnum.Type8:
                case MyMwcObjectBuilder_CargoBox_TypesEnum.Type6:
                    if (DisplayName == "Ammo")
                        displayName = MyTextsWrapper.Get(MyTextsWrapperEnum.CargoBoxAmmo).ToString();
                    break;
                case MyMwcObjectBuilder_CargoBox_TypesEnum.TypeProp_A:
                case MyMwcObjectBuilder_CargoBox_TypesEnum.Type9:
                    if (DisplayName == "Oxygen")
                        displayName = MyTextsWrapper.Get(MyTextsWrapperEnum.CargoBoxOxygen).ToString();
                    break;
                case MyMwcObjectBuilder_CargoBox_TypesEnum.Type5:
                case MyMwcObjectBuilder_CargoBox_TypesEnum.Type11:
                    if (DisplayName == "Fuel")
                        displayName = MyTextsWrapper.Get(MyTextsWrapperEnum.CargoBoxFuel).ToString();
                    break;
                case MyMwcObjectBuilder_CargoBox_TypesEnum.Type2:
                case MyMwcObjectBuilder_CargoBox_TypesEnum.Type12:
                    if (DisplayName == "Repair")
                        displayName = MyTextsWrapper.Get(MyTextsWrapperEnum.CargoBoxRepair).ToString();
                    break;
                default:
                    
                    break;
            }

            if (DisplayName == "Health")
                displayName = MyTextsWrapper.Get(MyTextsWrapperEnum.Health).ToString();


            return displayName;
        }

        protected override void SetHudMarker()
        {
            if (IsNotEmpty())
            {
                MyHudMaxDistanceMultiplerTypes? maxDistanceMultiplerType = null;

                if (m_cargoBoxType == 0)
                    m_cargoBoxType = GetTypeFromModelLod0Enum(m_modelLod0.ModelEnum);

                string displayName = GetCorrectDisplayName(ref maxDistanceMultiplerType);

                MyHud.ChangeText(this, new StringBuilder(displayName), MyGuitargetMode.CargoBox, 200f, MyHudIndicatorFlagsEnum.SHOW_BORDER_INDICATORS | MyHudIndicatorFlagsEnum.SHOW_DISTANCE | MyHudIndicatorFlagsEnum.SHOW_TEXT, maxDistanceMultiplerType: maxDistanceMultiplerType);
            }
            else
            {
                MyHud.RemoveText(this);
            }
        }

        private static MyMwcObjectBuilder_CargoBox_TypesEnum GetTypeFromModelLod0Enum(MyModelsEnum model)
        {
            switch (model)
            {
                case MyModelsEnum.cargo_box_1:
                    return MyMwcObjectBuilder_CargoBox_TypesEnum.Type1;
                case MyModelsEnum.cargo_box_2:
                    return MyMwcObjectBuilder_CargoBox_TypesEnum.Type2;
                case MyModelsEnum.cargo_box_3:
                    return MyMwcObjectBuilder_CargoBox_TypesEnum.Type3;
                case MyModelsEnum.cargo_box_4:
                    return MyMwcObjectBuilder_CargoBox_TypesEnum.Type4;
                case MyModelsEnum.cargo_box_5:
                    return MyMwcObjectBuilder_CargoBox_TypesEnum.Type5;
                case MyModelsEnum.cargo_box_6:
                    return MyMwcObjectBuilder_CargoBox_TypesEnum.Type6;
                case MyModelsEnum.cargo_box_7:
                    return MyMwcObjectBuilder_CargoBox_TypesEnum.Type7;
                case MyModelsEnum.cargo_box_8:
                    return MyMwcObjectBuilder_CargoBox_TypesEnum.Type8;
                case MyModelsEnum.cargo_box_9:
                    return MyMwcObjectBuilder_CargoBox_TypesEnum.Type9;
                case MyModelsEnum.cargo_box_10:
                    return MyMwcObjectBuilder_CargoBox_TypesEnum.Type10;
                case MyModelsEnum.cargo_box_11:
                    return MyMwcObjectBuilder_CargoBox_TypesEnum.Type11;
                case MyModelsEnum.cargo_box_12:
                    return MyMwcObjectBuilder_CargoBox_TypesEnum.Type12;
                case MyModelsEnum.CargoBox_prop_A:
                    return MyMwcObjectBuilder_CargoBox_TypesEnum.TypeProp_A;
                case MyModelsEnum.CargoBox_prop_B:
                    return MyMwcObjectBuilder_CargoBox_TypesEnum.TypeProp_B;
                case MyModelsEnum.CargoBox_prop_C:
                    return MyMwcObjectBuilder_CargoBox_TypesEnum.TypeProp_C;
                case MyModelsEnum.CargoBox_prop_D:
                    return MyMwcObjectBuilder_CargoBox_TypesEnum.TypeProp_D;
                case MyModelsEnum.cargo_box_small:
                    return MyMwcObjectBuilder_CargoBox_TypesEnum.DroppedItems;
                default:
                    return (MyMwcObjectBuilder_CargoBox_TypesEnum)0;
            }
        }

        public static MyModelsEnum GetModelLod0EnumFromType(MyMwcObjectBuilder_CargoBox_TypesEnum cargoBoxType)
        {
            switch (cargoBoxType)
            {
                case MyMwcObjectBuilder_CargoBox_TypesEnum.Type1:
                    return MyModelsEnum.cargo_box_1;
                case MyMwcObjectBuilder_CargoBox_TypesEnum.Type2:
                    return MyModelsEnum.cargo_box_2;
                case MyMwcObjectBuilder_CargoBox_TypesEnum.Type3:
                    return MyModelsEnum.cargo_box_3;
                case MyMwcObjectBuilder_CargoBox_TypesEnum.Type4:
                    return MyModelsEnum.cargo_box_4;
                case MyMwcObjectBuilder_CargoBox_TypesEnum.Type5:
                    return MyModelsEnum.cargo_box_5;
                case MyMwcObjectBuilder_CargoBox_TypesEnum.Type6:
                    return MyModelsEnum.cargo_box_6;
                case MyMwcObjectBuilder_CargoBox_TypesEnum.Type7:
                    return MyModelsEnum.cargo_box_7;
                case MyMwcObjectBuilder_CargoBox_TypesEnum.Type8:
                    return MyModelsEnum.cargo_box_8;
                case MyMwcObjectBuilder_CargoBox_TypesEnum.Type9:
                    return MyModelsEnum.cargo_box_9;
                case MyMwcObjectBuilder_CargoBox_TypesEnum.Type10:
                    return MyModelsEnum.cargo_box_10;
                case MyMwcObjectBuilder_CargoBox_TypesEnum.Type11:
                    return MyModelsEnum.cargo_box_11;
                case MyMwcObjectBuilder_CargoBox_TypesEnum.Type12:
                    return MyModelsEnum.cargo_box_12;
                case MyMwcObjectBuilder_CargoBox_TypesEnum.TypeProp_A:
                    return MyModelsEnum.CargoBox_prop_A;
                case MyMwcObjectBuilder_CargoBox_TypesEnum.TypeProp_B:
                    return MyModelsEnum.CargoBox_prop_B;
                case MyMwcObjectBuilder_CargoBox_TypesEnum.TypeProp_C:
                    return MyModelsEnum.CargoBox_prop_C;
                case MyMwcObjectBuilder_CargoBox_TypesEnum.TypeProp_D:
                    return MyModelsEnum.CargoBox_prop_D;
                case MyMwcObjectBuilder_CargoBox_TypesEnum.DroppedItems:
                    return MyModelsEnum.cargo_box_small;
                default:
                    throw new ArgumentOutOfRangeException("cargoBoxType");
            }
        }

        public MySoundCuesEnum GetTakeAllSound()
        {
            switch (CargoBoxType)
            {
                case MyMwcObjectBuilder_CargoBox_TypesEnum.Type1:
                case MyMwcObjectBuilder_CargoBox_TypesEnum.TypeProp_B:
                case MyMwcObjectBuilder_CargoBox_TypesEnum.TypeProp_C:
                case MyMwcObjectBuilder_CargoBox_TypesEnum.TypeProp_D:
                case MyMwcObjectBuilder_CargoBox_TypesEnum.Type4:
                case MyMwcObjectBuilder_CargoBox_TypesEnum.Type10:
                case MyMwcObjectBuilder_CargoBox_TypesEnum.DroppedItems:
                    return MySoundCuesEnum.SfxTakeAllUniversal;
                case MyMwcObjectBuilder_CargoBox_TypesEnum.Type6:
                case MyMwcObjectBuilder_CargoBox_TypesEnum.Type8:
                    return MySoundCuesEnum.SfxTakeAllAmmo;
                case MyMwcObjectBuilder_CargoBox_TypesEnum.Type5:
                case MyMwcObjectBuilder_CargoBox_TypesEnum.Type11:
                    return MySoundCuesEnum.SfxTakeAllFuel;
                case MyMwcObjectBuilder_CargoBox_TypesEnum.Type3:
                case MyMwcObjectBuilder_CargoBox_TypesEnum.Type7:
                    return MySoundCuesEnum.SfxTakeAllMedkit;
                case MyMwcObjectBuilder_CargoBox_TypesEnum.Type9:
                case MyMwcObjectBuilder_CargoBox_TypesEnum.TypeProp_A:
                    return MySoundCuesEnum.SfxTakeAllOxygen;
                case MyMwcObjectBuilder_CargoBox_TypesEnum.Type2:
                case MyMwcObjectBuilder_CargoBox_TypesEnum.Type12:
                    return MySoundCuesEnum.SfxTakeAllRepair;
                default:
                    return MySoundCuesEnum.SfxTakeAllUniversal;
            }
        }

        private void ChangeModelLod0(MyMwcObjectBuilder_CargoBox_TypesEnum cargoBoxType)
        {
            var modelLod0Enum = GetModelLod0EnumFromType(cargoBoxType);
            var modelLod0 = MyModels.GetModelForDraw(modelLod0Enum);
            m_modelLod0 = modelLod0;
        }

        protected override MyMwcObjectBuilder_Base GetObjectBuilderInternal(bool getExactCopy)
        {
            var builder = (MyMwcObjectBuilder_CargoBox)base.GetObjectBuilderInternal(getExactCopy);
            builder.Inventory = Inventory.GetObjectBuilder(getExactCopy);
            builder.DisplayName = DisplayName;
            builder.CargoBoxType = CargoBoxType;

            return builder;
        }

        public override void Close()
        {
            Inventory.OnInventoryContentChange -= Inventory_OnInventoryContentChange;
            Inventory.Close();
            base.Close();
        }

        public override string GetFriendlyName()
        {
            return "MyCargoBox";
        }

        public void Reset()
        {
            Respawn();
        }

        protected override void DoDamageInternal(float playerDamage, float damage, float empDamage, MyDamageType damageType, MyAmmoType ammoType, MyEntity damageSource, bool justDeactivate)
        {
            base.DoDamageInternal(playerDamage, damage, empDamage, damageType, ammoType, damageSource, justDeactivate);

            if (IsDead())
            {
                MarkForClose();

                var effect = MyParticlesManager.CreateParticleEffect((int)MyParticleEffectsIDEnum.Explosion_SmallPrefab);
                effect.WorldMatrix = WorldMatrix;
                effect.UserScale = 0.5f;
            }
        }
    }
}