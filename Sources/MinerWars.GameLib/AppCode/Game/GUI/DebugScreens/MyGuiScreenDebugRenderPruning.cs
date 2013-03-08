using System;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Render;
using MinerWars.AppCode.Game.Effects;
using MinerWars.CommonLIB.AppCode.Utils;
using SysUtils;
using KeenSoftwareHouse.Library.Extensions;

namespace MinerWars.AppCode.Game.GUI
{
    class MyGuiScreenDebugRenderPruning : MyGuiScreenDebugBase
    {
        public MyGuiScreenDebugRenderPruning()
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

            AddCaption(new System.Text.StringBuilder("Pruning and culling"), Color.Yellow.ToVector4());

            MyGuiControlLabel label = new MyGuiControlLabel(this, new Vector2(0.01f, -m_size.Value.Y / 2.0f + 0.07f), null, new System.Text.StringBuilder("(press ALT to share focus)"), Color.Yellow.ToVector4(), MyGuiConstants.LABEL_TEXT_SCALE * 0.7f,
                               MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_TOP);
            Controls.Add(label);

            m_currentPosition = -m_size.Value / 2.0f + new Vector2(0.02f, 0.10f);

            MyRender.MyRenderModuleItem renderModule = MyRender.GetRenderModules(MyRenderStage.DebugDraw).Find((x) => x.Name == MyRenderModuleEnum.PrunningStructure);
            MyRender.MyRenderModuleItem renderModulePhysics = MyRender.GetRenderModules(MyRenderStage.DebugDraw).Find((x) => x.Name == MyRenderModuleEnum.PhysicsPrunningStructure);
            
            AddSlider(new StringBuilder("Worst allowed balance"), 0.01f, 0.5f, null, MemberHelper.GetMember(() => MyRender.CullingStructureWorstAllowedBalance));
            AddSlider(new StringBuilder("Box cut penalty"), 0, 30, null, MemberHelper.GetMember(() => MyRender.CullingStructureCutBadness));
            AddSlider(new StringBuilder("Imbalance penalty"), 0, 5, null, MemberHelper.GetMember(() => MyRender.CullingStructureImbalanceBadness));
            AddSlider(new StringBuilder("Off-center penalty"), 0, 5, null, MemberHelper.GetMember(() => MyRender.CullingStructureOffsetBadness));

            m_currentPosition.Y += 0.01f;
            AddCheckBox(new StringBuilder("Show prunning structure"), renderModule, MemberHelper.GetMember(() => renderModule.Enabled));

            m_currentPosition.Y += 0.01f;
            AddCheckBox(new StringBuilder("Show physics prunning structure"), renderModulePhysics, MemberHelper.GetMember(() => renderModulePhysics.Enabled));

            m_currentPosition.Y += 0.01f;
            AddButton(new StringBuilder("Rebuild now"), delegate { MyRender.RebuildCullingStructure(); });

            m_currentPosition.Y += 0.01f;
            AddButton(new StringBuilder("Rebuild to test lowest triangle count"), delegate { MyRender.RebuildCullingStructureCullEveryPrefab(); });
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenDebugRenderPruning";
        }

    }
}
