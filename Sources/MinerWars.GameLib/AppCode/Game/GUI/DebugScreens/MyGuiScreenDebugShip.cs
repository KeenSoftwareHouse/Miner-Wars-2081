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
#endregion

namespace MinerWars.AppCode.Game.GUI
{
    class MyGuiScreenDebugShip : MyGuiScreenDebugBase
    {
        public MyGuiScreenDebugShip()
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

            AddCaption(new System.Text.StringBuilder("Player ship debug"), Color.Yellow.ToVector4());

            MyGuiControlLabel label = new MyGuiControlLabel(this, new Vector2(0.01f, -m_size.Value.Y / 2.0f + 0.07f), null, new System.Text.StringBuilder("(press ALT to share focus)"), Color.Yellow.ToVector4(), MyGuiConstants.LABEL_TEXT_SCALE * 0.7f,
                               MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_TOP);
            Controls.Add(label);

            m_scale = 0.6f;
            m_currentPosition = -m_size.Value / 2.0f + new Vector2(0.02f, 0.10f);

            if (MyGuiScreenGamePlay.Static != null && MySession.PlayerShip != null)
            {
                AddSlider(new StringBuilder("Mass"), 1, 30000, MySession.PlayerShip.Physics, MemberHelper.GetMember(() => MySession.PlayerShip.Physics.Mass));
                AddSlider(new StringBuilder("Max speed"), 1, 500, MySession.PlayerShip.Physics, MemberHelper.GetMember(() => MySession.PlayerShip.Physics.MaxLinearVelocity));
                AddSlider(new StringBuilder("Max angular velocity"), 0, 30, MySession.PlayerShip.Physics, MemberHelper.GetMember(() => MySession.PlayerShip.Physics.MaxAngularVelocity));
                AddSlider(new StringBuilder("Multiplier forward & backward"), 0, 300*7, MySession.PlayerShip.ShipTypeProperties.Physics, MemberHelper.GetMember(() => MySession.PlayerShip.ShipTypeProperties.Physics.MultiplierForwardBackward));
                AddSlider(new StringBuilder("Multiplier strafe"), 0, 300*7, MySession.PlayerShip.ShipTypeProperties.Physics, MemberHelper.GetMember(() => MySession.PlayerShip.ShipTypeProperties.Physics.MultiplierStrafe));
                AddSlider(new StringBuilder("Multiplier strafe rotation"), 0, 100, MySession.PlayerShip.ShipTypeProperties.Physics, MemberHelper.GetMember(() => MySession.PlayerShip.ShipTypeProperties.Physics.MultiplierStrafeRotation));
                AddSlider(new StringBuilder("Multiplier up & down"), 0, 300*7, MySession.PlayerShip.ShipTypeProperties.Physics, MemberHelper.GetMember(() => MySession.PlayerShip.ShipTypeProperties.Physics.MultiplierUpDown));
                AddSlider(new StringBuilder("Multiplier roll"), 0, 30, MySession.PlayerShip.ShipTypeProperties.Physics, MemberHelper.GetMember(() => MySession.PlayerShip.ShipTypeProperties.Physics.MultiplierRoll));
                AddSlider(new StringBuilder("Multiplier rotation"), 0, 20, MySession.PlayerShip.ShipTypeProperties.Physics, MemberHelper.GetMember(() => MySession.PlayerShip.ShipTypeProperties.Physics.MultiplierRotation));
                AddSlider(new StringBuilder("Multiplier rotation effect"), 0, 2f, MySession.PlayerShip.ShipTypeProperties.Physics, MemberHelper.GetMember(() => MySession.PlayerShip.ShipTypeProperties.Physics.MultiplierRotationEffect));
                AddSlider(new StringBuilder("Multiplier rotation decelerate"), 0, 150, MySession.PlayerShip.ShipTypeProperties.Physics, MemberHelper.GetMember(() => MySession.PlayerShip.ShipTypeProperties.Physics.MultiplierRotationDecelerate));
                AddSlider(new StringBuilder("Multiplier movement"), 0.2f, 5f, MySession.PlayerShip.ShipTypeProperties.Physics, MemberHelper.GetMember(() => MySession.PlayerShip.ShipTypeProperties.Physics.MultiplierMovement));
                AddSlider(new StringBuilder("Multiplier afterburner"), 0, 10, MySession.PlayerShip.EnginePropertiesForDebug, MemberHelper.GetMember(() => MySession.PlayerShip.EnginePropertiesForDebug.AfterburnerSpeedMultiplier));
                AddSlider(new StringBuilder("Multiplier horizontal angle stabilization"), 0, 1000, MySession.PlayerShip.ShipTypeProperties.Physics, MemberHelper.GetMember(() => MySession.PlayerShip.ShipTypeProperties.Physics.MultiplierHorizontalAngleStabilization));
                AddCheckBox(new StringBuilder("Show secondary camera bounding frustum"), MySession.PlayerShip, MemberHelper<MySmallShip>.GetMember((smallShip) => smallShip.DebugDrawSecondaryCameraBoundingFrustum));
                AddSlider(new StringBuilder("Max zoom sensitivity modifier"), 0, 1, null, MemberHelper.GetMember(() => MySmallShip.SENSITIVITY_MODIFIER_FOR_MAX_ZOOM));
                AddSlider(new StringBuilder("Rotation sensitivity modifier"), 0.5f, 1.3f, null, MemberHelper.GetMember(() => MySmallShip.SHIP_ROTATION_SENSITIVITY_MODIFIER));                
            }

            m_currentPosition.Y += 0.01f;
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenDebugShip";
        }
    }
}
