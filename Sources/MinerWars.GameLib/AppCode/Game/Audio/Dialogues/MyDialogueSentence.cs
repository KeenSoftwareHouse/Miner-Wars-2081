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
    class MyDialogueSentence
    {
        static readonly int MIN_SENTENCE_TIME = 1500;
        static readonly int MAX_SENTENCE_TIME = 6000;

        public MyActorEnum Actor { get; private set; }
        public MyActorEnum? Listener { get; private set; }
        public MySoundCuesEnum? Cue { get; private set; }
        public MyDialoguesWrapperEnum Text { get; private set; }

        public float SentenceTime_ms { get; private set; }
        public float PauseBefore_ms { get; private set; }

        public float Noise { get; private set; }
        public MyGuiFont Font
        {
            get
            {
                var faction = MySession.PlayerShip == null ? MinerWars.CommonLIB.AppCode.ObjectBuilders.MyMwcObjectBuilder_FactionEnum.Rainiers : MySession.PlayerShip.Faction;
                switch (MyFactions.GetFactionsRelation(faction, MyActorConstants.GetActorProperties(Actor).Faction))
                {
                    case MyFactionRelationEnum.Enemy: return MyHudConstants.ENEMY_FONT;
                    case MyFactionRelationEnum.Friend: return MyHudConstants.FRIEND_FONT;
                    default: return MyHudConstants.NEUTRAL_FONT;
                }
            }
        }
        public MyTexture2D BackgroundTexture
        {
            get
            {
                var faction = MySession.PlayerShip == null ? MinerWars.CommonLIB.AppCode.ObjectBuilders.MyMwcObjectBuilder_FactionEnum.Rainiers : MySession.PlayerShip.Faction;
                switch (MyFactions.GetFactionsRelation(faction, MyActorConstants.GetActorProperties(Actor).Faction))
                {
                    case MyFactionRelationEnum.Enemy: return MyGuiManager.GetDialogueEnemyBackgroundTexture();
                    case MyFactionRelationEnum.Friend: return MyGuiManager.GetDialogueFriendBackgroundTexture();
                    default: return MyGuiManager.GetDialogueNeutralBackgroundTexture();
                }
            }
        }


        public MyDialogueSentence(MyActorEnum speaker, MySoundCuesEnum? cue, MyDialoguesWrapperEnum text, float noise = 0.0f, float pauseBefore_ms = 0.0f, MyActorEnum? listener = null)
        {
            Actor = speaker;
            Listener = listener;
            Cue = cue;
            Text = text;
            PauseBefore_ms = pauseBefore_ms;

            SentenceTime_ms = MathHelper.Clamp(MyDialoguesWrapper.Get(Text).Length * 66, MIN_SENTENCE_TIME, MAX_SENTENCE_TIME) + PauseBefore_ms;
            MyCommonDebugUtils.AssertDebug(noise >= 0 && noise <= 1, "Bad dialogue sentence noise value!");
            Noise = noise;
            
            if (MyActorConstants.IsNoiseActor(speaker))
            {
                Noise = 1f;
            }
        }
    }
}
