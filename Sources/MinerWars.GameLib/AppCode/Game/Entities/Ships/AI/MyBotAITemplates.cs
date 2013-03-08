using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using System.Diagnostics;
using MinerWars.AppCode.Game.Managers.Session;

namespace MinerWars.AppCode.Game.Entities.Ships.AI
{
    public class MyBotAITemplate
    {
        public MyAITemplateEnum TemplateId { get; set; }

        static int DESIRE_TYPE_COUNT = Enum.GetValues(typeof(BotDesireType)).Length;

        private BotBehaviorType[] m_selectedBehaviors;
        private int[] m_priorities;

        public MyBotAITemplate(MyAITemplateEnum templateId, BotBehaviorType idle, BotBehaviorType seeEnemy, BotBehaviorType atackedByEnemy, BotBehaviorType lowHealth, BotBehaviorType noAmmo, BotBehaviorType curious,
                             int idlePriority, int seeEnemyPriority, int atackedByEnemyPriority, int lowHealthPriority, int noAmmoPriority, int curiousPriority)
        {
            TemplateId = templateId;

            m_selectedBehaviors = new BotBehaviorType[DESIRE_TYPE_COUNT];

            m_selectedBehaviors[(int)BotDesireType.IDLE] = idle;
            m_selectedBehaviors[(int)BotDesireType.SEE_ENEMY] = seeEnemy;
            m_selectedBehaviors[(int)BotDesireType.ATACKED_BY_ENEMY] = atackedByEnemy;
            m_selectedBehaviors[(int)BotDesireType.NO_AMMO] = noAmmo;
            m_selectedBehaviors[(int)BotDesireType.LOW_HEALTH] = lowHealth;
            m_selectedBehaviors[(int)BotDesireType.CURIOUS] = curious;
            m_selectedBehaviors[(int)BotDesireType.KILL] = BotBehaviorType.ATTACK;
            m_selectedBehaviors[(int)BotDesireType.FLASHED] = BotBehaviorType.PANIC;

            m_priorities = new int[DESIRE_TYPE_COUNT];

            m_priorities[(int)BotDesireType.IDLE] = idlePriority;
            m_priorities[(int)BotDesireType.SEE_ENEMY] = seeEnemyPriority;
            m_priorities[(int)BotDesireType.ATACKED_BY_ENEMY] = atackedByEnemyPriority;
            m_priorities[(int)BotDesireType.NO_AMMO] = noAmmoPriority;
            m_priorities[(int)BotDesireType.LOW_HEALTH] = lowHealthPriority;
            m_priorities[(int)BotDesireType.CURIOUS] = curiousPriority;
            m_priorities[(int)BotDesireType.KILL] = 1000;
            m_priorities[(int)BotDesireType.FLASHED] = 2000;

            // IMPORTANT: Don't forget to modify GetCopy() method
        }

        internal int CompareDesires(MyBotDesire a, MyBotDesire b)
        {
            if (a.DesireType == b.DesireType)
            {
                var ta = a.GetEnemy();
                var tb = b.GetEnemy();

                int priorityA = ta != null ? ta.AIPriority : -1;
                int priorityB = tb != null ? tb.AIPriority : -1;

                return priorityA.CompareTo(priorityB);
            }
            
            return m_priorities[(int)a.DesireType] - m_priorities[(int)b.DesireType];
        }

        public BotBehaviorType GetBehaviorType(BotDesireType desire)
        {
            return m_selectedBehaviors[(int)desire];
        }

        public MyBotBehaviorBase GetBehavior(BotDesireType desire)
        {
            BotBehaviorType behaviorType = m_selectedBehaviors[(int)desire];
            switch (behaviorType)
            {
                case BotBehaviorType.IGNORE:
                    return null;
                case BotBehaviorType.PATROL:
                    return new MyBotBehaviorPatrol();
                case BotBehaviorType.FOLLOW:
                    return new MyBotBehaviorFollow();
                case BotBehaviorType.ATTACK:
                    return new MyBotBehaviorAttack();
                case BotBehaviorType.RUN_AWAY:
                    return new MyBotBehaviorRunAway();
                case BotBehaviorType.PANIC:
                    return new MyBotBehaviorPanic();
                case BotBehaviorType.KAMIKADZE:
                    return new MyBotBehaviorKamikadze();
                case BotBehaviorType.IDLE:
                    return new MyBotBehaviorIdle();
                case BotBehaviorType.CURIOUS:
                    return new MyBotBehaviorCurious();
            }
            System.Diagnostics.Debug.Fail("Unexpected behavior: " + behaviorType);
            return null;
        }

        public bool IsIgnored(BotDesireType desire)
        {
            return m_selectedBehaviors[(int)desire] == BotBehaviorType.IGNORE;
        }

        public bool IsPatroling()
        {
            return m_selectedBehaviors[(int)BotDesireType.IDLE] == BotBehaviorType.PATROL;
        }

        public MyBotAITemplate GetCopy()
        {
            return new MyBotAITemplate(TemplateId,

                m_selectedBehaviors[(int)BotDesireType.IDLE],
                m_selectedBehaviors[(int)BotDesireType.SEE_ENEMY],
                m_selectedBehaviors[(int)BotDesireType.ATACKED_BY_ENEMY],
                m_selectedBehaviors[(int)BotDesireType.LOW_HEALTH],
                m_selectedBehaviors[(int)BotDesireType.NO_AMMO],
                m_selectedBehaviors[(int)BotDesireType.CURIOUS],

                m_priorities[(int)BotDesireType.IDLE],
                m_priorities[(int)BotDesireType.SEE_ENEMY],
                m_priorities[(int)BotDesireType.ATACKED_BY_ENEMY],
                m_priorities[(int)BotDesireType.LOW_HEALTH],
                m_priorities[(int)BotDesireType.NO_AMMO],
                m_priorities[(int)BotDesireType.CURIOUS]);
        }

        public void SetIdleBehavior(BotBehaviorType idleBehavior)
        {
            m_selectedBehaviors[(int)BotDesireType.IDLE] = idleBehavior;
        }

        public BotBehaviorType GetIdleBehavior()
        {
            return m_selectedBehaviors[(int)BotDesireType.IDLE];
        }
    }
    
    public class MyBotAITemplates
    {
        public static List<MyBotAITemplate> Templates;
        static MyBotAITemplates()
        {
            Templates = new List<MyBotAITemplate>();

            Templates.Add(new MyBotAITemplate(MyAITemplateEnum.DEFAULT, BotBehaviorType.IDLE, BotBehaviorType.ATTACK, BotBehaviorType.ATTACK, BotBehaviorType.IGNORE, BotBehaviorType.IGNORE, BotBehaviorType.IGNORE,
                                            0, 2, 3, 4, 5, 1));
            Templates.Add(new MyBotAITemplate(MyAITemplateEnum.AGGRESIVE, BotBehaviorType.IDLE, BotBehaviorType.ATTACK, BotBehaviorType.ATTACK, BotBehaviorType.IGNORE, BotBehaviorType.IGNORE, BotBehaviorType.CURIOUS,
                                            0, 2, 3, 4, 5, 1));
            Templates.Add(new MyBotAITemplate(MyAITemplateEnum.DEFENSIVE, BotBehaviorType.IDLE, BotBehaviorType.IGNORE, BotBehaviorType.ATTACK, BotBehaviorType.IGNORE, BotBehaviorType.IGNORE, BotBehaviorType.IGNORE,
                                            0, 2, 3, 4, 5, 1));
            Templates.Add(new MyBotAITemplate(MyAITemplateEnum.FLEE, BotBehaviorType.IDLE, BotBehaviorType.RUN_AWAY, BotBehaviorType.RUN_AWAY, BotBehaviorType.IGNORE, BotBehaviorType.IGNORE, BotBehaviorType.IGNORE,
                                            0, 2, 3, 4, 5, 1));
            Templates.Add(new MyBotAITemplate(MyAITemplateEnum.CRAZY, BotBehaviorType.PANIC, BotBehaviorType.KAMIKADZE, BotBehaviorType.KAMIKADZE, BotBehaviorType.PANIC, BotBehaviorType.IGNORE, BotBehaviorType.IGNORE,
                                            0, 2, 3, 4, 5, 1));
            Templates.Add(new MyBotAITemplate(MyAITemplateEnum.DRONE, BotBehaviorType.FOLLOW, BotBehaviorType.IGNORE, BotBehaviorType.IGNORE, BotBehaviorType.IGNORE, BotBehaviorType.IGNORE, BotBehaviorType.IGNORE,
                                            5, 3, 2, 1, 0, 4));
            Templates.Add(new MyBotAITemplate(MyAITemplateEnum.PASSIVE, BotBehaviorType.IDLE, BotBehaviorType.IGNORE, BotBehaviorType.IGNORE, BotBehaviorType.IGNORE, BotBehaviorType.IGNORE, BotBehaviorType.IGNORE,
                                            0, 2, 3, 4, 5, 1));
        }

        public static MyBotAITemplate GetTemplate(MyAITemplateEnum templateEnum)
        {
            foreach (var template in Templates)
            {
                if (template.TemplateId == templateEnum)
                {
                    return template.GetCopy();
                }
            }

            Debug.Fail("AI template not found!");
            return null;
        }
    }
}
