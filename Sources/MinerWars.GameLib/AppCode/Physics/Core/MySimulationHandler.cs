#region Using Statements

using System.Collections.Generic;
using MinerWarsMath;

#endregion

namespace MinerWars.AppCode.Physics
{
    //////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Simulation handlers can be registered in order to receive simulation notfications
    /// </summary>
    abstract class MyPhysSimulationHandler
    {
        public virtual void BeforeSimulation(float timeStep) { }
        public virtual void AfterSimulation(float timeStep) { }
    }

}