using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.Utils;

namespace MinerWars.AppCode.Game.Entities.EntityDetector
{
    internal delegate bool MyEntityDetectorCriteriumCondition<T>(T entity, params object[] args);

    interface IMyEntityDetectorCriterium
    {
        int Key { get; }
        bool ReCheckCriterium { get; }

        bool IsEntityInRightType(MyEntity entity);
        bool Check(MyEntity entity);
    }

    class MyEntityDetectorCriterium<T> : IMyEntityDetectorCriterium
        where T : MyEntity
    {
        public int Key { get; private set; }
        public MyEntityDetectorCriteriumCondition<T> Condition { get; set; }        
        public bool ReCheckCriterium { get; set; }
        public object[] Args { get; set; }

        public MyEntityDetectorCriterium(int key, MyEntityDetectorCriteriumCondition<T> condition, bool reCheckCriterium, params object[] args)
        {
            if (!MyUtils.IsPowerOfTwo(key))
            {
                throw new Exception("Criterium key must by power of 2");
            }
            Key = key;
            Condition = condition;
            ReCheckCriterium = reCheckCriterium;
            Args = args;
        }        

        public MyEntityDetectorCriterium(int key)
            : this(key, null, false, null)
        {
        }

        public MyEntityDetectorCriterium(int key, MyEntityDetectorCriteriumCondition<T> condition, params object[] args)
            : this(key, condition, false, args)
        {
        }              

        public bool IsEntityInRightType(MyEntity entity)
        {
            return entity is T;
        }

        public bool Check(MyEntity entity)
        {
            if (!IsEntityInRightType(entity))
            {
                return false;
            }

            if(Condition != null)
            {
                if (!Condition(entity as T, Args))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
