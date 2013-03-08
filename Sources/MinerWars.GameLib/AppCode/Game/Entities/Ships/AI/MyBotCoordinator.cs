#region Using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Utils;
using MinerWarsMath;
using MinerWars.AppCode.Game.Render;
#endregion

namespace MinerWars.AppCode.Game.Entities.Ships.AI
{
    static class MyBotCoordinator
    {
        #region Members

        public static readonly int MAX_BOTS_ATTACKING_PLAYER = 4;
        static private List<MySmallShipBot> m_bots;
        private static List<KeyValuePair<float, MySmallShipBot>> m_currentBotsAttackingPlayer = new List<KeyValuePair<float, MySmallShipBot>>();
        private static int m_maxBotsAttackingPlayer = MAX_BOTS_ATTACKING_PLAYER;

        #endregion

        #region Properties



        public static List<KeyValuePair<float, MySmallShipBot>> CurrentBotsAttackingPlayer
        {
            get { return m_currentBotsAttackingPlayer; }
            set { m_currentBotsAttackingPlayer = value; }
        }

        public static int MaxBotsAttackingPlayer
        {
            get { return m_maxBotsAttackingPlayer; }
            set { m_maxBotsAttackingPlayer = value; }
        }

        #endregion

        #region Constructor

        static MyBotCoordinator()
        {
            m_bots = new List<MySmallShipBot>(256);

            MyEntities.OnEntityRemove += MyEntities_OnEntityRemove;

            MyRender.RegisterRenderModule(MyRenderModuleEnum.AttackingBots, "Attacking bots", DebugDraw, MyRenderStage.DebugDraw, false);
        }

        #endregion

        #region Coordinator

        static int Compare(KeyValuePair<float, MySmallShipBot> a, KeyValuePair<float, MySmallShipBot> b)
        {
            return a.Key.CompareTo(b.Key);
        }


        static void MyEntities_OnEntityRemove(MyEntity obj)
        {
            MySmallShipBot smallShipBot = obj as MySmallShipBot;
            if (m_bots.Remove(smallShipBot))
            {
                // Bot removed
            }

            if (smallShipBot != null)
            {
                RemoveAttacker(smallShipBot);
            }
        }

        static public void AddBot(MySmallShipBot bot)
        {
            Debug.Assert(!m_bots.Contains(bot));
            m_bots.Add(bot);
        }

        static public List<MySmallShipBot> GetBots()
        {
            return m_bots;
        }

        #endregion

        #region Attack coordination

        public static bool IsAllowedToAttack(MySmallShipBot attacker, MyEntity target)
        {
            if (MySession.IsPlayerShip(target))
            {
                foreach (var pair in CurrentBotsAttackingPlayer)
                {
                    if (pair.Value == attacker)
                        return true;
                }

                return false;
            }

            return true;
        }

        static List<MySmallShipBot> m_sortHelper = new List<MySmallShipBot>(4);

        public static void AddAttacker(MySmallShipBot attacker, MyEntity target)
        {
            if (MySession.IsPlayerShip(target))
            {
                foreach (var pair in CurrentBotsAttackingPlayer)
                {
                    if (pair.Value == attacker)
                        return;
                }

                float attackerDistance = Vector3.Distance(MyCamera.Position, attacker.GetPosition());

                if (CurrentBotsAttackingPlayer.Count == MaxBotsAttackingPlayer)
                {
                    if (CurrentBotsAttackingPlayer[CurrentBotsAttackingPlayer.Count - 1].Key <= attackerDistance)
                        return;

                    for (int i = 0; i < CurrentBotsAttackingPlayer.Count; i++ )
                    {
                        var pair = CurrentBotsAttackingPlayer[i];

                        if (attackerDistance < pair.Key)
                        {
                            CurrentBotsAttackingPlayer.RemoveAt(i);
                            break;
                        }
                    }
                }

                CurrentBotsAttackingPlayer.Add(new KeyValuePair<float, MySmallShipBot>(attackerDistance, attacker));
                CurrentBotsAttackingPlayer.Sort(Compare);
            }

            System.Diagnostics.Debug.Assert(CurrentBotsAttackingPlayer.Count <= MaxBotsAttackingPlayer);
        }

        public static void RemoveAttacker(MySmallShipBot attacker)
        {
            for (int i = 0; i < CurrentBotsAttackingPlayer.Count; i++)
            {
                var pair = CurrentBotsAttackingPlayer[i];
                if (pair.Value == attacker)
                {
                    CurrentBotsAttackingPlayer.RemoveAt(i);
                    break;
                }
            }
        }

        public static void Update()
        {
            m_sortHelper.Clear();
            foreach (var pair in CurrentBotsAttackingPlayer)
            {
                m_sortHelper.Add(pair.Value);
            }

            CurrentBotsAttackingPlayer.Clear();

            foreach (MySmallShipBot bot in m_sortHelper)
            {
                float distance = Vector3.Distance(MyCamera.Position, bot.GetPosition());
                CurrentBotsAttackingPlayer.Add(new KeyValuePair<float, MySmallShipBot>(distance, bot));
            }

            CurrentBotsAttackingPlayer.Sort(Compare);

            m_sortHelper.Clear();
        }

        #endregion

        #region Follow coordination

        public static void ChangeShip(MySmallShip oldShip, MySmallShip newShip)
        {
            Debug.Assert(oldShip != null && newShip != null);
            foreach (var bot in m_bots)
            {
                if (bot.Leader == oldShip)
                {
                    bot.StopFollow();
                    bot.Follow(newShip);
                }
            }
        }

        #endregion

        public static void DebugDraw()
        {
            foreach (var pair in CurrentBotsAttackingPlayer)
            {
                MySmallShipBot bot = pair.Value;
                MyDebugDraw.DrawSphereWireframe(bot.GetPosition(), bot.WorldVolume.Radius, new Vector3(1, 0, 0), 1);
            }
        }
    }
}
