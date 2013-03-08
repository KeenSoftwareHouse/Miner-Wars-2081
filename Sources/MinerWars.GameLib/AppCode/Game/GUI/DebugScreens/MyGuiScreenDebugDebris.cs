#region Using
using System;
using System.Collections.Generic;
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
using MinerWars.AppCode.Game.Models;
using MinerWars.AppCode.Game;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Physics;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.TransparentGeometry;
using MinerWars.AppCode.Game.World;
#endregion

namespace MinerWars.AppCode.Game.GUI
{
    class MyGuiScreenDebugDebris : MyGuiScreenDebugBase
    {
        public MyGuiScreenDebugDebris()
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

            AddCaption(new System.Text.StringBuilder("Debris field"), Color.Yellow.ToVector4());

            MyGuiControlLabel label = new MyGuiControlLabel(this, new Vector2(0.01f, -m_size.Value.Y / 2.0f + 0.07f), null, new System.Text.StringBuilder("(press ALT to share focus)"), Color.Yellow.ToVector4(), MyGuiConstants.LABEL_TEXT_SCALE * 0.7f,
                               MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_TOP);
            Controls.Add(label);

            m_scale = 0.7f;
            m_currentPosition = -m_size.Value / 2.0f + new Vector2(0.02f, 0.10f);

            var debrisObj = MySector.DebrisProperties;
            AddLabel(new StringBuilder("Debris field"), Vector4.One, 1.0f);
            AddCheckBox(new StringBuilder("Enabled"), debrisObj, MemberHelper.GetMember(() => MySector.DebrisProperties.Enabled));
            AddSlider(new StringBuilder("Distance between"), 1, 500, debrisObj, MemberHelper.GetMember(() => MySector.DebrisProperties.DistanceBetween));
            AddSlider(new StringBuilder("Half count in direction"), 1, 50, debrisObj, MemberHelper.GetMember(() => MySector.DebrisProperties.CountInDirectionHalf));
            AddSlider(new StringBuilder("Max distance"), 1, 1000, debrisObj, MemberHelper.GetMember(() => MySector.DebrisProperties.MaxDistance));
            AddSlider(new StringBuilder("Full scale distance"), 1, 1000, debrisObj, MemberHelper.GetMember(() => MySector.DebrisProperties.FullScaleDistance));

            m_currentPosition.Y += 0.01f;

            AddLabel(new StringBuilder("Voxel debris"), Vector4.One, 1.0f);
            var low = AddSlider(new StringBuilder("Lower size"), 0.0005f, 0.05f, null, MemberHelper.GetMember(() => MyExplosionDebrisVoxel.DebrisScaleLower));
            var high = AddSlider(new StringBuilder("Upper size"), 0.0005f, 0.05f, null, MemberHelper.GetMember(() => MyExplosionDebrisVoxel.DebrisScaleUpper));
            AddSlider(new StringBuilder("Clamp size"), 0.1f, 2.0f, null, MemberHelper.GetMember(() => MyExplosionDebrisVoxel.DebrisScaleClamp));

            low.LabelDecimalPlaces = 4;
            high.LabelDecimalPlaces = 4;
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenDebugDebris";
        }
    }
}
