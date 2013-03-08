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
    //Prepared to be render debug screen

    class MyGuiScreenDebugRender : MyGuiScreenDebugBase
    {
        public MyGuiScreenDebugRender()
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

            AddCaption(new System.Text.StringBuilder("Render debug"), Color.Yellow.ToVector4());

            MyGuiControlLabel label = new MyGuiControlLabel(this, new Vector2(0.01f, -m_size.Value.Y / 2.0f + 0.07f), null, new System.Text.StringBuilder("(press ALT to share focus)"), Color.Yellow.ToVector4(), MyGuiConstants.LABEL_TEXT_SCALE * 0.7f,
                               MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_TOP);
            Controls.Add(label);

            m_currentPosition = -m_size.Value / 2.0f + new Vector2(0.02f, 0.10f);

            m_currentPosition.Y += 0.01f;
            AddLabel(new StringBuilder("LODs"), Color.Yellow.ToVector4(), 1.2f);

            var profile = MyRenderConstants.RenderQualityProfile; // Obfuscated MemberHelper can't access property, so store it to field

            //AddCheckBox(new StringBuilder("Show LOD screens"), null, MemberHelper.GetMember(() => MyRender.ShowLODScreens));
            AddCheckBox(new StringBuilder("Show blended screens"), null, MemberHelper.GetMember(() => MyRender.ShowBlendedScreens));
            AddCheckBox(new StringBuilder("Show LOD1 red overlay"), null, MemberHelper.GetMember(() => MyRender.ShowLod1WithRedOverlay));
            AddCheckBox(new StringBuilder("Show green background"), null, MemberHelper.GetMember(() => MyRender.ShowGreenBackground));
            AddCheckBox(new StringBuilder("Show environment screens"), null, MemberHelper.GetMember(() => MyRender.ShowEnvironmentScreens));
            AddSlider(new StringBuilder("Lod near distance"), 0.0f, 20000.0f, profile, MemberHelper.GetMember(() => profile.LodTransitionDistanceNear));
            AddSlider(new StringBuilder("Lod far distance"), 0.0f, 20000.0f, profile, MemberHelper.GetMember(() => profile.LodTransitionDistanceFar));
            AddSlider(new StringBuilder("Lod background start"), 0.0f, 50000.0f, profile, MemberHelper.GetMember(() => profile.LodTransitionDistanceBackgroundStart));
            AddSlider(new StringBuilder("Lod background end"), MyCamera.NEAR_PLANE_DISTANCE + 1, 50000.0f, profile, MemberHelper.GetMember(() => profile.LodTransitionDistanceBackgroundEnd));


            m_currentPosition.Y += 0.01f;
            AddLabel(new StringBuilder("Textures"), Color.Yellow.ToVector4(), 1.2f);
            
            AddCheckBox(new StringBuilder("Check diffuse textures"), null, MemberHelper.GetMember(() => MyRender.CheckDiffuseTextures));
            AddCheckBox(new StringBuilder("Check normals textures"), null, MemberHelper.GetMember(() => MyRender.CheckNormalTextures));

            AddCheckBox(new StringBuilder("Debug diffuse texture"), null, MemberHelper.GetMember(() => MyRender.DebugDiffuseTexture));
            AddCheckBox(new StringBuilder("Debug normal texture"), null, MemberHelper.GetMember(() => MyRender.DebugNormalTexture));

            m_currentPosition.Y += 0.01f;
            AddLabel(new StringBuilder("Clip planes"), Color.Yellow.ToVector4(), 1.2f);
            AddSlider(new StringBuilder("Near clip"), 0.05f, 10.0f, null, MemberHelper.GetMember(() => MyCamera.NEAR_PLANE_DISTANCE));
            AddSlider(new StringBuilder("Far clip"), 10000.0f, 100000.0f, null, MemberHelper.GetMember(() => MyCamera.FAR_PLANE_DISTANCE));

            AddSlider(new StringBuilder("FOV"), MyCamera.FieldOfViewAngle, 1.00f, 90.0f, new MyGuiControlSlider.OnSliderChangeCallback(OnFovSlider));
        }

        void OnFovSlider(MyGuiControlSlider slider)
        {
            MyCamera.FieldOfViewAngle = slider.GetValue();
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenDebugRender";
        }

    }
}
