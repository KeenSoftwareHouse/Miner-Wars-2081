using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.World.Global;
using MinerWarsMath;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Textures;
using SysUtils.Utils;
using MinerWars.AppCode.Game.Managers.Session;

namespace MinerWars.AppCode.Game.Audio.Dialogues
{
    enum MyDialoguePriorityEnum : byte
    {
        LOW = 0,
        BASIC = 1,
        TOP = 2
    }

    class MyDialogue
    {
        public MyDialoguePriorityEnum Priority { get; private set; }
        public MyDialogueSentence[] Sentences { get; private set; }

        public MyDialogue(MyDialogueSentence[] sentences, MyDialoguePriorityEnum priority = MyDialoguePriorityEnum.BASIC)
        {
            Sentences = sentences;
            Priority = priority;
        }
    }
}
