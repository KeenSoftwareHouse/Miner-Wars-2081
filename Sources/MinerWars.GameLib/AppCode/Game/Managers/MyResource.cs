// -----------------------------------------------------------------------
//  <copyright file="MyResource.cs" company="Keen Software House">
//      Copyright (c) Keen Software House 2011. All rights reserved.
//  </copyright>
//  <author>Ondřej Štorek</author>
// -----------------------------------------------------------------------

namespace MinerWars.AppCode.Game.Managers
{
    using System.Collections.Generic;
    using KeenSoftwareHouse.Library.Memory;
    using System.Reflection;
    using KeenSoftwareHouse.Library.Extensions;

    /// <summary>
    /// Defines object that supports collecting of sub-resources.
    /// </summary>
    internal interface IResourceCollector
    {
        /// <summary>
        /// Collects all sub-resources of this object.
        /// </summary>
        /// <param name="collectedResources">The collected resources.</param>
        /// <param name="hierarchically"></param>
        void CollectResources(ICollection<MyResource> collectedResources, bool hierarchically = true);
    }

    /// <summary>
    /// Abstract engine resource base of all engine data.
    /// </summary>
    internal abstract class MyResource: IResourceCollector
    {
        #region Constants

        /// <summary>
        /// Can read values.
        /// </summary>
        public const byte User = 1 << 0;

        /// <summary>
        /// Can change values and execute standard logic.
        /// </summary>
        public const byte Controller = 1 << 3;

        /// <summary>
        /// Can change values execute protected logic and  serialize/deserialize resource.
        /// </summary>
        public const byte Owner = 1 << 7;

        #endregion

        #region Properties

        string m_name;

        /// <summary>
        /// Gets or sets the name of resource used for debug/develop purpose.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name
        {
            get
            {
                return m_name;
            }
            protected set
            {
                m_name = value;
            }
        }

        /// <summary>
        /// Gets or sets the manager.
        /// </summary>
        /// <value>
        /// The manager associtated with this resource.
        /// </value>
        //public MyManager Manager { get; internal set; }

        // removed, it's "Storkovina"
        ///// <summary>
        ///// Gets a value indicating whether this instance is activated.
        ///// </summary>
        ///// <value>
        ///// 	<c>true</c> if this instance is activated; otherwise, <c>false</c>.
        ///// </value>
        //public bool IsActivated { get; private set; }

        #endregion
        
        #region Methods

        /// <summary>
        /// Called when resource is [activated] for its context.
        /// </summary>
        /// <param name="source">The source of activation.</param>
        protected virtual void OnActivated(object source) {}

        /// <summary>
        /// Called when resource is [deactivated] for its context.
        /// </summary>
        /// <param name="source">The source of deactivation.</param>
        protected virtual void OnDeactivated(object source) { }

        #endregion

        #region Notifications methods

        /// <summary>
        /// Notifies that resource was activated.
        /// <remarks>
        /// This function should call direct only owner/manager of this resource.
        /// </remarks>
        /// </summary>
        /// <param name="source">The source of activation.</param>
        public virtual void NotifyActivated(object source)
        {
            // removed, it's "Storkovina"
            //this.IsActivated = true;

            OnActivated(source);
            
            using (var ob = PoolClass<HashSet<MyResource>>.Acquire())
            {
                HashSet<MyResource> collectedResources = ob;
                collectedResources.Clear();
                
                CollectResources(collectedResources, false);
             
                foreach (var collectedResource in collectedResources)
                {
                    collectedResource.NotifyActivated(this);
                }
            }
        }

        /// <summary>
        /// Notifies that resource was deactivated.
        /// </summary>
        /// <remarks>
        /// This function should call direct only owner/manager of this resource.
        /// </remarks>
        /// <param name="source">The source of deactivation.</param>
        public virtual void NotifyDeactivated(object source)
        {
            using (var ob = PoolClass<HashSet<MyResource>>.Acquire())
            {
                HashSet<MyResource> collectedResources = ob;
                collectedResources.Clear();

                CollectResources(collectedResources, false);

                foreach (var collectedResource in collectedResources)
                {
                    collectedResource.NotifyDeactivated(this);
                }
            }

            // removed, it's "Storkovina"
            //this.IsActivated = false;

            OnDeactivated(source);
        }

        #endregion
        
        #region Implementation of IResourceCollector

        /// <summary>
        /// Collects all sub-resources of this object.
        /// </summary>
        /// <param name="collectedResources">The collected resources.</param>
        /// <param name="hierarchically">[true] collect all resources hierarchically, [false] only children.</param>
        public virtual void CollectResources(ICollection<MyResource> collectedResources, bool hierarchically = true) { }

        #endregion
    }
}