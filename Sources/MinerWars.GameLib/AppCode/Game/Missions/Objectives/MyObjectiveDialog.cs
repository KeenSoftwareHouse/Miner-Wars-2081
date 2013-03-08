using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Audio.Dialogues;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.Localization;

namespace MinerWars.AppCode.Game.Missions
{
    class MyObjectiveDialog : MyObjective
    {
        public MyObjectiveDialog(MyMissionID id, MyTexture2D icon, MyMission parentMission, MyMissionID[] requiredMissions, Audio.Dialogues.MyDialogueEnum? dialogId = null) 
            : base(MyTextsWrapperEnum.Null, id, MyTextsWrapperEnum.Null, icon, parentMission, requiredMissions, null, null, dialogId, null)
        {
            ShowNotificationOnSuccess = false;
        }

        public MyObjectiveDialog(MyTextsWrapperEnum text, MyMissionID id, MyTextsWrapperEnum description, MyTexture2D icon, MyMission parentMission, MyMissionID[] requiredMissions, Audio.Dialogues.MyDialogueEnum? dialogId = null)
            : base(text, id, description, icon, parentMission, requiredMissions, null, null, dialogId, null)
        {
            ShowNotificationOnSuccess = false;
        }

        [Obsolete]
        public MyObjectiveDialog(StringBuilder text, MyMissionID id, StringBuilder description, MyTexture2D icon, MyMission parentMission, MyMissionID[] requiredMissions, Audio.Dialogues.MyDialogueEnum? dialogId = null)
            : base(text, id, description, icon, parentMission, requiredMissions, null, null, dialogId, null)
        {
            ShowNotificationOnSuccess = false;
        }

        public override bool IsSuccess()
        {
            return true;
        }


        public override void Success()
        {
            base.Success();
        }

        public void SkipDialog()
        {
            MyDialogues.Stop();
            SkipDialogEnabled = true;
            Success();
        }

        public override void Update()
        {
            base.Update();
        }
    }
}
