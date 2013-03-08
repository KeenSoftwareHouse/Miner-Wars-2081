using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.App;
using SysUtils.Utils;
using MinerWars.AppCode.Game.Inventory;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;

namespace MinerWars.AppCode.Game.Entities.Tools
{
    public enum MyMedicineType
    {
        PERFORMANCE_ENHANCING_MEDICINE,
        HEALTH_ENHANCING_MEDICINE,
        ANTIRADIATION_MEDICINE,
        MEDIKIT,
    }

    class MyMedicine
    {
        private MyMedicineType m_type;
        private float m_activationTime;
        private float m_lastTriggerTime;

        MyMedicine(MyMedicineType type)
        {
            m_type = type;
            Deactivate();
        }

        public static MyMedicine[] GetArrayOfAllMedicines()
        {
            return new MyMedicine[] {
                new MyMedicine(MyMedicineType.PERFORMANCE_ENHANCING_MEDICINE),
                new MyMedicine(MyMedicineType.HEALTH_ENHANCING_MEDICINE),
                new MyMedicine(MyMedicineType.ANTIRADIATION_MEDICINE),
                new MyMedicine(MyMedicineType.MEDIKIT),
            };
        }

        #region Commands

        public void Activate()
        {
            m_activationTime = m_lastTriggerTime = MyMinerGame.TotalGamePlayTimeInMilliseconds;
        }

        public void Deactivate()
        {
            m_activationTime = m_lastTriggerTime = float.NegativeInfinity;
        }

        public void Trigger()
        {
            m_lastTriggerTime = MyMinerGame.TotalGamePlayTimeInMilliseconds;
        }

        public void ActivateIfInInventory(MyInventory inventory)
        {
            MyInventoryItem item;
            switch (Type())
            {
                case MyMedicineType.MEDIKIT:
                    item = inventory.GetInventoryItem(MyMwcObjectBuilderTypeEnum.SmallShip_Tool, (int?)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.MEDIKIT);
                    if (item != null)
                    {
                        Activate();
                        inventory.RemoveInventoryItemAmount(ref item, 1);
                    }
                    break;
                case MyMedicineType.ANTIRADIATION_MEDICINE:
                    item = inventory.GetInventoryItem(MyMwcObjectBuilderTypeEnum.SmallShip_Tool, (int?)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.ANTIRADIATION_MEDICINE);
                    if (item != null)
                    {
                        Activate();
                        inventory.RemoveInventoryItemAmount(ref item, 1);
                    }
                    break;
                case MyMedicineType.HEALTH_ENHANCING_MEDICINE:
                    item = inventory.GetInventoryItem(MyMwcObjectBuilderTypeEnum.SmallShip_Tool, (int?)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.HEALTH_ENHANCING_MEDICINE);
                    if (item != null)
                    {
                        Activate();
                        inventory.RemoveInventoryItem(item, true);
                    }
                    break;
                case MyMedicineType.PERFORMANCE_ENHANCING_MEDICINE:
                    item = inventory.GetInventoryItem(MyMwcObjectBuilderTypeEnum.SmallShip_Tool, (int?)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.PERFORMANCE_ENHANCING_MEDICINE);
                    if (item != null)
                    {
                        Activate();
                        inventory.RemoveInventoryItem(item, true);
                    }
                    break;
            }
        }

        #endregion

        #region Queries

        public MyMedicineType Type()
        {
            return m_type;
        }

        public float Duration()
        {
            switch (m_type)
            {
                case MyMedicineType.MEDIKIT: return MyMedicineConstants.MEDIKIT_DURATION;
                case MyMedicineType.ANTIRADIATION_MEDICINE: return MyMedicineConstants.ANTIRADIATION_MEDICINE_DURATION;
                case MyMedicineType.HEALTH_ENHANCING_MEDICINE: return MyMedicineConstants.HEALTH_ENHANCING_MEDICINE_DURATION;
                case MyMedicineType.PERFORMANCE_ENHANCING_MEDICINE: return MyMedicineConstants.PERFORMANCE_ENHANCING_MEDICINE_DURATION;
                default: MyCommonDebugUtils.AssertDebug(false, "Unknown medicine type."); return 0;
            }
        }

        public float TimeSinceActivation()
        {
            return MyMinerGame.TotalGamePlayTimeInMilliseconds - m_activationTime;
        }

        public bool IsActive()
        {
            return TimeSinceActivation() >= 0 && TimeSinceActivation() < Duration();
        }

        public float ActiveTimeSinceLastTriggered()
        {
            return MyMinerGame.TotalGamePlayTimeInMilliseconds - m_lastTriggerTime - Math.Max(0, TimeSinceActivation() - Duration());
        }

        public bool WasActiveSinceLastTriggered()
        {
            return ActiveTimeSinceLastTriggered() > 0;
        }

        #endregion
    }
}
