using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Entities.Ships.AI;
using MinerWars.AppCode.Game.GUI.Helpers;
using MinerWars.AppCode.Game.World.Global;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Sessions;
using MinerWars.CommonLIB.AppCode.Networking.Multiplayer;
using MinerWars.AppCode.Game.Audio.Dialogues;
using MinerWars.AppCode.Game.Audio;
using MinerWars.AppCode.Game.Entities;

namespace MinerWars.AppCode.Game.Missions
{
    static class MyFriendlyFire
    {
        private static float? m_gameOverTimer;

        public static void Load()
        {
            m_gameOverTimer = null;
        }

        public static void Unload()
        {
            m_gameOverTimer = null;
        }

        public static void MakeEnemy(MyMwcObjectBuilder_FactionEnum shipFaction)
        {
            // Order bots to kill player
            foreach (var bot in MyBotCoordinator.GetBots())
            {
                var relation = MyFactions.GetFactionsRelation(shipFaction, bot.Faction);
                if (relation == MyFactionRelationEnum.Friend || relation == MyFactionRelationEnum.Neutral)
                {
                    bot.Attack(MySession.PlayerShip);
                }
            }

            MyScriptWrapper.SetPlayerFaction(MyMwcObjectBuilder_FactionEnum.Freelancers);


            MyMissions.DisableSaveObjectives();
        }

        public static void StartGameoverTimer()
        {
            if (m_gameOverTimer == null)
            {
                MyAudio.AddCue2D(MySoundCuesEnum.SfxClaxonAlert); 
                MySmallShipBot.PlayFriendlyFireCue();
                MyDialogues.Stop();
                m_gameOverTimer = 10.0f; // 10s
            }
        }

        public static void Fail()
        {
            MySession.Static.GameOver(MyTextsWrapperEnum.CriticalFriendlyFireDetected);
            m_gameOverTimer = null;
        }

        public static void Update()
        {
            // Handle friendly fire aggro
            if (m_gameOverTimer != null)
            {
                m_gameOverTimer -= MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS;
                if (m_gameOverTimer.Value < 0 && MyGuiScreenGamePlay.Static.IsGameStoryActive())
                {
                    Fail();

                    if (MyMultiplayerGameplay.IsRunning)
                    {
                        MyMultiplayerGameplay.Static.FriendlyFire(MyFriendlyFireEnum.GAME_FAILED);
                    }
                }
            }
        }
    }
}
