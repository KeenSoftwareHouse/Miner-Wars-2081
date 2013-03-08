using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Inventory;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.Audio.Dialogues;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;

namespace MinerWars.AppCode.Game.Missions.Submissions
{
    class MyHarvestOreSubmission : MyObjective
    {
        bool m_oreObtained = false;
        float m_amount = 100;
        MyMwcObjectBuilder_Ore_TypesEnum m_ore;

        public MyHarvestOreSubmission(MyTextsWrapperEnum name, MyMissionID id, MyTextsWrapperEnum description, MyTexture2D icon, MyMission parentMission, MyMissionID[] requiredMissions, MyMissionLocation location, MyMwcObjectBuilder_Ore_TypesEnum ore, float amount, MyDialogueEnum successDialogId)
            : base(name, id, description, icon, parentMission, requiredMissions, null, successDialogId: successDialogId)
        {
            Location = location;
            m_amount = amount;
            m_ore = ore;
        }

        public override void Load()
        {
            base.Load();

            CheckOre();

            MyScriptWrapper.EntityInventoryItemAmountChanged += EntityInventoryItemAmountChanged;
        }

        public override void Unload()
        {
            base.Unload();
            MyScriptWrapper.EntityInventoryItemAmountChanged -= EntityInventoryItemAmountChanged;
        }


        public override bool IsSuccess()
        {
            return m_oreObtained;
        }

        public override void Success()
        {
            base.Success();
        }

        void EntityInventoryItemAmountChanged(MyEntity entity, MyInventory inventory, MyInventoryItem item, float number)
        {
            if (MyScriptWrapper.IsPlayerShip(entity))
            {
                CheckOre();
            }
        }

        private void CheckOre()
        {
            m_oreObtained = MyScriptWrapper.GetInventoryItemAmount(MyScriptWrapper.GetPlayerInventory(), MyMwcObjectBuilderTypeEnum.Ore, (int)m_ore) > m_amount;
        }
    }
}
