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
using MinerWars.AppCode.Game.BackgroundCube;
using MinerWars.AppCode.Game.World;
using System.Linq;
using MinerWars.AppCode.Game.SolarSystem;
using MinerWars.AppCode.Game.TransparentGeometry;

namespace MinerWars.AppCode.Game.GUI
{
    //Prepared to be render debug screen

    class MyGuiScreenDebugRenderGlobalFX : MyGuiScreenDebugBase
    {
        public MyGuiScreenDebugRenderGlobalFX()
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

            m_scale = .9f;

            AddCaption(new System.Text.StringBuilder("Render Global FX"), Color.Yellow.ToVector4());

            MyGuiControlLabel label = new MyGuiControlLabel(this, new Vector2(0.01f, -m_size.Value.Y / 2.0f + 0.07f), null, new System.Text.StringBuilder("(press ALT to share focus)"), Color.Yellow.ToVector4(), MyGuiConstants.LABEL_TEXT_SCALE * 0.7f,
                               MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_TOP);
            Controls.Add(label);

            m_currentPosition = -m_size.Value / 2.0f + new Vector2(0.02f, 0.10f);

            m_currentPosition.Y += 0.01f * m_scale;
            AddLabel(new StringBuilder("FXAA"), Color.Yellow.ToVector4(), 1.2f);

            AddCheckBox(new StringBuilder("Enable FXAA"), null, MemberHelper.GetMember(() => MyMwcFinalBuildConstants.EnableFxaa));

            m_currentPosition.Y += 0.01f * m_scale;
            AddLabel(new StringBuilder("Fog"), Color.Yellow.ToVector4(), 1.2f);

            var fogObj = MySector.FogProperties;

            AddCheckBox(new StringBuilder("Enable fog"), null, MemberHelper.GetMember(() => MyRender.EnableFog));
            AddSlider(new StringBuilder("Fog near distance"), 1.0f, MyCamera.FAR_PLANE_DISTANCE, fogObj, MemberHelper.GetMember(() => MySector.FogProperties.FogNear));
            AddSlider(new StringBuilder("Fog far distance"), 1.0f, MyCamera.FAR_PLANE_DISTANCE, fogObj, MemberHelper.GetMember(() => MySector.FogProperties.FogFar));
            AddSlider(new StringBuilder("Fog multiplier"), 0.0f, 1.0f, fogObj, MemberHelper.GetMember(() => MySector.FogProperties.FogMultiplier));
            AddSlider(new StringBuilder("Fog backlight multiplier"), 0.0f, 5.0f, fogObj, MemberHelper.GetMember(() => MySector.FogProperties.FogBacklightMultiplier));
            AddColor(new StringBuilder("Fog color"), fogObj, MemberHelper.GetMember(() => MySector.FogProperties.FogColor));
           
        }

        private bool nebula_selector(MyImpostorProperties properties)
        {
            return properties.ImpostorType == MyVoxelMapImpostors.MyImpostorType.Nebula;
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenDebugRenderGlobalFX";
        }

    }
}
