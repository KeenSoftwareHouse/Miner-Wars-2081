using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Audio.Dialogues;
using MinerWars.AppCode.Game.GUI.Helpers;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.World.Global;

namespace MinerWars.AppCode.Game.Missions
{
    class MyObjectiveEnterInventory : MyObjective
    {

        public MyObjectiveEnterInventory(MyTextsWrapperEnum Name, MyMissionID ID, MyTextsWrapperEnum Description, MyTexture2D Icon, MyMission ParentMission, MyMissionID[] RequiredMissions, uint ownerId, MyDialogueEnum? successDialogId = null, MyDialogueEnum? startDialogId = null, MyActorEnum[] requiredActors = null, float? radiusOverride = null)
            : base(Name, ID, Description, Icon, ParentMission, RequiredMissions, null, null, successDialogId, startDialogId, requiredActors, radiusOverride)
        {
            MissionEntityIDs = new List<uint> { ownerId };
        }

        private bool m_isSuccess;
        public override void Load()
        {
            base.Load();
            
            MyGuiScreenInventoryManagerForGame.OpeningInventoryScreen += MyGuiScreenInventoryManagerForGameOnOpeningInventoryScreen;
        }

        private void MyGuiScreenInventoryManagerForGameOnOpeningInventoryScreen(MyEntity entity, MySmallShipInteractionActionEnum interactionAction)
        {
            if (MissionEntityIDs.Contains(entity.EntityId.Value.NumericValue))
            {
                MissionEntityIDs.Remove(entity.EntityId.Value.NumericValue);
                if (0 == MissionEntityIDs.Count)
                {
                    m_isSuccess = true;
                }
            }
        }

        public override bool IsSuccess()
        {
            return m_isSuccess;
        }
        public override void Unload()
        {
            base.Unload();
            MyGuiScreenInventoryManagerForGame.OpeningInventoryScreen -= MyGuiScreenInventoryManagerForGameOnOpeningInventoryScreen;
        }
    }
}
