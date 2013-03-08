using System;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Render;
using MinerWars.AppCode.Game.Effects;
using MinerWars.AppCode.Game.Voxels;
using MinerWars.CommonLIB.AppCode.Utils;
using SysUtils;
using KeenSoftwareHouse.Library.Extensions;
using MinerWars.AppCode.Game.Managers;
using MinerWars.AppCode.Game.Entities;

namespace MinerWars.AppCode.Game.GUI
{
    //Prepared to be render debug screen

    class MyGuiScreenDebugRenderOptimizations : MyGuiScreenDebugBase
    {
        public MyGuiScreenDebugRenderOptimizations()
            : base(0.35f * Color.Yellow.ToVector4(), false)
        {
            m_closeOnEsc = true;
            m_drawEvenWithoutFocus = true;
            m_isTopMostScreen = false;
            m_canHaveFocus = false;

            RecreateControls(true);
        }

        public override void RecreateControls(bool contructor)
        {
            Controls.Clear();

            m_scale = 0.7f;

            AddCaption(new System.Text.StringBuilder("Render optimizations"), Color.Yellow.ToVector4());

            MyGuiControlLabel label = new MyGuiControlLabel(this, new Vector2(0.01f, -m_size.Value.Y / 2.0f + 0.07f), null, new System.Text.StringBuilder("(press ALT to share focus)"), Color.Yellow.ToVector4(), MyGuiConstants.LABEL_TEXT_SCALE * 0.7f,
                               MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_TOP);
            Controls.Add(label);

            m_currentPosition = -m_size.Value / 2.0f + new Vector2(0.02f, 0.10f);

            m_currentPosition.Y += 0.01f;
            AddLabel(new StringBuilder("Optimizations"), Color.Yellow.ToVector4(), 1.2f);
            AddCheckBox(new StringBuilder("Show particles overdraw"), null, MemberHelper.GetMember(() => TransparentGeometry.MyTransparentGeometry.VisualiseOverdraw));
            AddCheckBox(new StringBuilder("Enable stencil optimization"), null, MemberHelper.GetMember(() => MyRender.EnableStencilOptimization));
            //AddCheckBox(new StringBuilder("Enable LOD blending"), null, MemberHelper.GetMember(() => MyRender.EnableLODBlending));
            //AddCheckBox(new StringBuilder("Enable stencil optimization LOD1"), null, MemberHelper.GetMember(() => MyRender.EnableStencilOptimizationLOD1));
            //AddCheckBox(new StringBuilder("Show stencil optimization"), null, MemberHelper.GetMember(() => MyRender.ShowStencilOptimization));
            AddCheckBox(new StringBuilder("Respect cast shadows flag"), null, MemberHelper.GetMember(() => MyShadowRenderer.RespectCastShadowsFlags));
            AddCheckBox(new StringBuilder("Multithreaded shadows"), MyRender.GetShadowRenderer(), MemberHelper.GetMember(() => MyRender.GetShadowRenderer().MultiThreaded));
            AddCheckBox(new StringBuilder("Multithreaded entities prepare"), null, MemberHelper.GetMember(() => MyRender.EnableEntitiesPrepareInBackground));
            

            m_currentPosition.Y += 0.01f;
            AddLabel(new StringBuilder("HW Occ queries"), Color.Yellow.ToVector4(), 1.2f);
            AddCheckBox(new StringBuilder("Enable"), null, MemberHelper.GetMember(() => MyRender.EnableHWOcclusionQueries));
            AddCheckBox(new StringBuilder("Enable shadow occ.q."), null, MemberHelper.GetMember(() => MyRender.EnableHWOcclusionQueriesForShadows));
            AddCheckBox(new StringBuilder("Show occ queries"), null, MemberHelper.GetMember(() => MyRender.ShowHWOcclusionQueries));

            m_currentPosition.Y += 0.01f;
            AddLabel(new StringBuilder("Rendering"), Color.Yellow.ToVector4(), 1.2f);
            AddCheckBox(new StringBuilder("Alternative sort"), null, MemberHelper.GetMember(() => MyRender.AlternativeSort));
            AddCheckBox(new StringBuilder("Skip LOD NEAR"), null, MemberHelper.GetMember(() => MyRender.SkipLOD_NEAR));
            AddCheckBox(new StringBuilder("Skip LOD0"), null, MemberHelper.GetMember(() => MyRender.SkipLOD_0));
            AddCheckBox(new StringBuilder("Skip LOD1"), null, MemberHelper.GetMember(() => MyRender.SkipLOD_1));
            AddCheckBox(new StringBuilder("Skip Voxels"), null, MemberHelper.GetMember(() => MyRender.SkipVoxels));
            AddCheckBox(new StringBuilder("Show render stats"), null, MemberHelper.GetMember(() => MyRender.ShowEnhancedRenderStatsEnabled));
            AddCheckBox(new StringBuilder("Show resources stats"), null, MemberHelper.GetMember(() => MyRender.ShowResourcesStatsEnabled));
            AddCheckBox(new StringBuilder("Show textures stats"), null, MemberHelper.GetMember(() => MyRender.ShowTexturesStatsEnabled));
            AddCheckBox(new StringBuilder("Show entities stats"), null, MemberHelper.GetMember(() => MyEntities.ShowDebugDrawStatistics));

            m_currentPosition.Y += 0.01f;
            AddLabel(new StringBuilder("Voxel Mesh reduction"), Color.Yellow.ToVector4(), 1.2f);
            var checkBox = AddCheckBox(new StringBuilder("Enable decimation"), null, MemberHelper.GetMember(() => MyFakes.SIMPLIFY_VOXEL_MESH));
            checkBox.OnCheck += delegate(MyGuiControlCheckbox sender)
            {
                // just to trigger recalculation of voxel map
                MyMeshSimplifier.MinEdgeLength = MyMeshSimplifier.MinEdgeLength;
                RecreateControls(false);
            };
            var slider = AddSlider(new StringBuilder("Min Edge Length"), 0, 30, null, MemberHelper.GetMember(() => MyMeshSimplifier.MinEdgeLength));
            AddLabel(new StringBuilder("Voxel precalc time: " + MyMeshSimplifier.VoxelRecalcTime + " ms"), Color.Red.ToVector4(), 1.15f);
            slider.OnChange += delegate(MyGuiControlSlider sender)
            {
                RecreateControls(false);
            };

            AddButton(new StringBuilder("Textures to Log"), delegate { MyTextureManager.DbgDumpLoadedTexturesBetter(); });
            AddButton(new StringBuilder("Wrong entities to Log"), delegate { MyEntities.DumpWrongEntities(); });
            AddButton(new StringBuilder("Dump all entities to Log"), delegate { MyRender.DumpAllEntities(); });
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenDebugRenderOpts";
        }

    }
}
