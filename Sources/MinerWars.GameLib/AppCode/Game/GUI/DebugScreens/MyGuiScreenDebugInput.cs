using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Managers;
using KeenSoftwareHouse.Library.Extensions;

namespace MinerWars.AppCode.Game.GUI.DebugScreens
{
    class MyGuiScreenDebugInput : MyGuiScreenDebugBase
    {
        static StringBuilder m_debugText = new StringBuilder(1000);

        public MyGuiScreenDebugInput()
            : base(new Vector2(0.5f, 0.5f), new Vector2(), null, true)
        {
            m_isTopMostScreen = true;
            m_drawEvenWithoutFocus = true;
            m_canHaveFocus = false;
        }

        public override string GetFriendlyName()
        {
            return "DebugInputScreen";
        }

        public Vector2 GetScreenLeftTopPosition()
        {
            float deltaPixels = 25 * MyGuiManager.GetSafeScreenScale();
            Rectangle fullscreenRectangle = MyGuiManager.GetSafeFullscreenRectangle();
            return MyGuiManager.GetNormalizedCoordinateFromScreenCoordinate_FULLSCREEN(new Vector2(deltaPixels, deltaPixels));
        }

        public void SetTexts()
        {
            MyMwcUtils.ClearStringBuilder(m_debugText);

            var joy = MyGuiManager.GetInput().GetActualJoystickState();
            if (joy == null)
            {
                m_debugText.Append("No joystick detected.");
                return;
            }

            m_debugText.Append("Supported axes: ");
            if (MyGuiManager.GetInput().IsJoystickAxisSupported(MyJoystickAxesEnum.Xpos)) m_debugText.Append("X ");
            if (MyGuiManager.GetInput().IsJoystickAxisSupported(MyJoystickAxesEnum.Ypos)) m_debugText.Append("Y ");
            if (MyGuiManager.GetInput().IsJoystickAxisSupported(MyJoystickAxesEnum.Zpos)) m_debugText.Append("Z ");
            if (MyGuiManager.GetInput().IsJoystickAxisSupported(MyJoystickAxesEnum.RotationXpos)) m_debugText.Append("Rx ");
            if (MyGuiManager.GetInput().IsJoystickAxisSupported(MyJoystickAxesEnum.RotationYpos)) m_debugText.Append("Ry ");
            if (MyGuiManager.GetInput().IsJoystickAxisSupported(MyJoystickAxesEnum.RotationZpos)) m_debugText.Append("Rz ");
            if (MyGuiManager.GetInput().IsJoystickAxisSupported(MyJoystickAxesEnum.Slider1pos)) m_debugText.Append("S1 ");
            if (MyGuiManager.GetInput().IsJoystickAxisSupported(MyJoystickAxesEnum.Slider2pos)) m_debugText.Append("S2 ");
            m_debugText.AppendLine();

            m_debugText.Append("accX: "); m_debugText.AppendInt32(joy.AccelerationX); m_debugText.AppendLine();
            m_debugText.Append("accY: "); m_debugText.AppendInt32(joy.AccelerationY); m_debugText.AppendLine();
            m_debugText.Append("accZ: "); m_debugText.AppendInt32(joy.AccelerationZ); m_debugText.AppendLine();
            m_debugText.Append("angAccX: "); m_debugText.AppendInt32(joy.AngularAccelerationX); m_debugText.AppendLine();
            m_debugText.Append("angAccY: "); m_debugText.AppendInt32(joy.AngularAccelerationY); m_debugText.AppendLine();
            m_debugText.Append("angAccZ: "); m_debugText.AppendInt32(joy.AngularAccelerationZ); m_debugText.AppendLine();
            m_debugText.Append("angVelX: "); m_debugText.AppendInt32(joy.AngularVelocityX); m_debugText.AppendLine();
            m_debugText.Append("angVelY: "); m_debugText.AppendInt32(joy.AngularVelocityY); m_debugText.AppendLine();
            m_debugText.Append("angVelZ: "); m_debugText.AppendInt32(joy.AngularVelocityZ); m_debugText.AppendLine();
            m_debugText.Append("forX: "); m_debugText.AppendInt32(joy.ForceX); m_debugText.AppendLine();
            m_debugText.Append("forY: "); m_debugText.AppendInt32(joy.ForceY); m_debugText.AppendLine();
            m_debugText.Append("forZ: "); m_debugText.AppendInt32(joy.ForceZ); m_debugText.AppendLine();
            m_debugText.Append("rotX: "); m_debugText.AppendInt32(joy.RotationX); m_debugText.AppendLine();
            m_debugText.Append("rotY: "); m_debugText.AppendInt32(joy.RotationY); m_debugText.AppendLine();
            m_debugText.Append("rotZ: "); m_debugText.AppendInt32(joy.RotationZ); m_debugText.AppendLine();
            m_debugText.Append("torqX: "); m_debugText.AppendInt32(joy.TorqueX); m_debugText.AppendLine();
            m_debugText.Append("torqY: "); m_debugText.AppendInt32(joy.TorqueY); m_debugText.AppendLine();
            m_debugText.Append("torqZ: "); m_debugText.AppendInt32(joy.TorqueZ); m_debugText.AppendLine();
            m_debugText.Append("velX: "); m_debugText.AppendInt32(joy.VelocityX); m_debugText.AppendLine();
            m_debugText.Append("velY: "); m_debugText.AppendInt32(joy.VelocityY); m_debugText.AppendLine();
            m_debugText.Append("velZ: "); m_debugText.AppendInt32(joy.VelocityZ); m_debugText.AppendLine();
            m_debugText.Append("X: "); m_debugText.AppendInt32(joy.X); m_debugText.AppendLine();
            m_debugText.Append("Y: "); m_debugText.AppendInt32(joy.Y); m_debugText.AppendLine();
            m_debugText.Append("Z: "); m_debugText.AppendInt32(joy.Z); m_debugText.AppendLine();
            m_debugText.AppendLine();
            m_debugText.Append("AccSliders: "); foreach (var i in joy.AccelerationSliders) { m_debugText.AppendInt32(i); m_debugText.Append(" "); } m_debugText.AppendLine();
            m_debugText.Append("Buttons: "); foreach (var i in joy.Buttons) { m_debugText.Append(i ? "#" : "_"); m_debugText.Append(" "); } m_debugText.AppendLine();
            m_debugText.Append("ForSliders: "); foreach (var i in joy.ForceSliders) { m_debugText.AppendInt32(i); m_debugText.Append(" "); } m_debugText.AppendLine();
            m_debugText.Append("POVControllers: "); foreach (var i in joy.PointOfViewControllers) { m_debugText.AppendInt32(i); m_debugText.Append(" "); } m_debugText.AppendLine();
            m_debugText.Append("Sliders: "); foreach (var i in joy.Sliders) { m_debugText.AppendInt32(i); m_debugText.Append(" "); } m_debugText.AppendLine();
            m_debugText.Append("VelocitySliders: "); foreach (var i in joy.VelocitySliders) { m_debugText.AppendInt32(i); m_debugText.Append(" "); } m_debugText.AppendLine();
        }

        public override bool Draw(float backgroundFadeAlpha)
        {
            if (base.Draw(backgroundFadeAlpha) == false) return false;

            SetTexts();
            float textScale = MyGuiConstants.DEBUG_STATISTICS_TEXT_SCALE;

            Vector2 origin = GetScreenLeftTopPosition();

            MyGuiManager.DrawString(MyGuiManager.GetFontMinerWarsWhite(), m_debugText, origin, textScale,
                    Color.Yellow, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP);

            return true;
        }
    }
}
