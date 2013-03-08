using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Entities.VoxelHandShapes;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Voxels;
using MinerWars.AppCode.Game.Voxels;
using SysUtils.Utils;

namespace MinerWars.AppCode.Game.Editor
{
    /// <summary>
    /// Voxel hand undo/redo action
    /// </summary>
    class MyEditorActionVoxelHand : MyEditorActionBase
    {
        private MyVoxelHandShape m_voxelHandShape;
        private List<MyVoxelHandShape> m_voxelShapes;

        public MyEditorActionVoxelHand(MyVoxelMap voxelMap, MyVoxelHandShape voxelHandShape)
            : base(voxelMap)
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyEditorActionVoxelHand::ctor");
            m_voxelHandShape = voxelHandShape;
            m_voxelShapes = voxelMap.GetVoxelHandShapes();
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        public override bool Perform()
        {
            foreach (MyEntity entity in ActionEntities)
            {
                MyVoxelMap voxelMap = (MyVoxelMap)entity;
                voxelMap.AddVoxelHandShape(m_voxelHandShape, true);
            }
            return true;
        }

        public override bool Rollback()
        {
            MyCommonDebugUtils.AssertDebug(false, "Rollback called, but voxel hands are not undoable!");
            return true;
        }
    }
}
