#region Using Statements

using System.Collections.Generic;
using MinerWarsMath;
using MinerWars.AppCode.Physics;
using MinerWars.AppCode.Game.Entities;
using MinerWarsMath.Graphics;
using System;
using ParallelTasks;
using MinerWars.CommonLIB.AppCode.Utils;

#endregion

namespace MinerWars.AppCode.Game.Render
{
    /// <summary>
    /// Sensor element used for sensors
    /// </summary>
    class MyRenderObject : MyElement
    {

        #region Enums

        /// <summary>
        /// Entity flags.
        /// </summary>
        [Flags]
        public enum RenderFlags
        {
            /// <summary>
            /// Skip the object in render if detected that it is too small
            /// </summary>
            SkipIfTooSmall = 1 << 0,

            /// <summary>
            /// Needs resolve cast shadows flag (done by parallel raycast to sun)
            /// </summary>
            NeedsResolveCastShadow = 1 << 1,

            /// <summary>
            /// Tells if this object is visible from sun. If not, it does not casts shadows
            /// </summary>
            CastShadow = 1 << 2,

            /// <summary>
            /// Casts only one raycast to determine shadow casting
            /// </summary>
            FastCastShadowResolve = 1 << 3,
        }

        #endregion

        RenderFlags m_renderFlags;
        
        public int RenderCounter;
        public int ShadowCastUpdateInterval; //frames count to refresh visibility from sun
        public MyCullableRenderObject CullObject;
        public MyEntity Entity;
        public Task CastShadowTask;
        public MyCastShadowJob CastShadowJob;
        public float Distance; //if object was in frustum, distance is uptodate
        public MyMwcVector3Int? RenderCellCoord; //Render cell coordinate if voxel
 


        public MyRenderObject(MyEntity entity, MyMwcVector3Int? renderCellCoord)
        {
            Entity = entity;
            Flags = MyElementFlag.EF_AABB_DIRTY;
            m_renderFlags = RenderFlags.SkipIfTooSmall | RenderFlags.NeedsResolveCastShadow;
            RenderCellCoord = renderCellCoord;
        }

        public void SetDirty()
        {
            Flags |= MyElementFlag.EF_AABB_DIRTY;
        }

        public override void UpdateAABB()
        {
            if (RenderCellCoord != null)
            {
                MyMwcVector3Int renderCellCoord = RenderCellCoord.Value;
                ((MyVoxelMap)Entity).GetRenderCellBoundingBox(ref renderCellCoord, out m_AABB);

                //m_AABB.Max += MinerWars.AppCode.Game.Utils.MyVoxelConstants.VOXEL_DATA_CELL_SIZE_HALF_VECTOR_IN_METRES;
                //m_AABB.Min -= MinerWars.AppCode.Game.Utils.MyVoxelConstants.VOXEL_DATA_CELL_SIZE_HALF_VECTOR_IN_METRES;
            }
            else
            {
                m_AABB = Entity.WorldAABB;
            }
            base.UpdateAABB();
        }

        public bool SkipIfTooSmall
        {
            get { return (m_renderFlags & RenderFlags.SkipIfTooSmall) > 0; }
            set 
            { 
                if (value)
                    m_renderFlags |= RenderFlags.SkipIfTooSmall; 
                else
                    m_renderFlags &= ~RenderFlags.SkipIfTooSmall; 
            }
        }

        public bool NeedsResolveCastShadow
        {
            get { return (m_renderFlags & RenderFlags.NeedsResolveCastShadow) > 0; }
            set
            {
                if (value)
                    m_renderFlags |= RenderFlags.NeedsResolveCastShadow;
                else
                    m_renderFlags &= ~RenderFlags.NeedsResolveCastShadow;
            }
        }

        public bool FastCastShadowResolve
        {
            get { return (m_renderFlags & RenderFlags.FastCastShadowResolve) > 0; }
            set
            {
                if (value)
                    m_renderFlags |= RenderFlags.FastCastShadowResolve;
                else
                    m_renderFlags &= ~RenderFlags.FastCastShadowResolve;
            }
        }

        public bool CastShadow
        {
            get { return (m_renderFlags & RenderFlags.CastShadow) > 0; }
            set
            {
                if (value)
                    m_renderFlags |= RenderFlags.CastShadow;
                else
                    m_renderFlags &= ~RenderFlags.CastShadow;
            }
        }

    }
}
