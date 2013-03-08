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
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.World;

namespace MinerWars.AppCode.Game.GUI
{
    class MyGuiScreenDebugColors : MyGuiScreenDebugBase
    {
        public static bool EnableRenderLights = true;

        public MyGuiScreenDebugColors()
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

            

            m_scale = 0.62f;

            AddCaption(new System.Text.StringBuilder("Colors settings"), Color.Yellow.ToVector4());

            MyGuiControlLabel label = new MyGuiControlLabel(this, new Vector2(0.01f, -m_size.Value.Y / 2.0f + 0.07f), null, new System.Text.StringBuilder("(press ALT to share focus)"), Color.Yellow.ToVector4(), MyGuiConstants.LABEL_TEXT_SCALE * 0.7f,
                               MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_TOP);
            Controls.Add(label);

            m_currentPosition = -m_size.Value / 2.0f + new Vector2(0.02f, 0.10f);

            m_currentPosition.Y += 0.01f;
            AddLabel(new StringBuilder("Sun"), Color.Yellow.ToVector4(), 1.2f);

            var sunObj = MySector.SunProperties;

            MyEffectShadowMap shadowMapEffect = MyRender.GetEffect(MyEffects.ShadowMap) as MyEffectShadowMap;
            if (shadowMapEffect != null)
            {
                m_currentPosition.Y += 0.01f;
                AddSlider(new StringBuilder("Intensity"), 0, 10.0f, sunObj, MemberHelper.GetMember(() => MySector.SunProperties.SunIntensity));

                m_currentPosition.Y += 0.01f;
                AddColor(new StringBuilder("Sun color"), sunObj, MemberHelper.GetMember(() => MySector.SunProperties.SunDiffuse));

                m_currentPosition.Y += 0.02f;
                AddColor(new StringBuilder("Sun specular"), sunObj, MemberHelper.GetMember(() => MySector.SunProperties.SunSpecular));

                m_currentPosition.Y += 0.02f;
                AddSlider(new StringBuilder("Back sun intensity"), 0, 5.0f, sunObj, MemberHelper.GetMember(() => MySector.SunProperties.BackSunIntensity));

                m_currentPosition.Y += 0.01f;
                AddColor(new StringBuilder("Back sun color"), sunObj, MemberHelper.GetMember(() => MySector.SunProperties.BackSunDiffuse));

                m_currentPosition.Y += 0.02f;
                AddColor(new StringBuilder("Ambient color"), sunObj, MemberHelper.GetMember(() => MySector.SunProperties.AmbientColor));

                m_currentPosition.Y += 0.02f;
                AddSlider(new StringBuilder("Ambient multiplier"), 0, 5.0f, sunObj, MemberHelper.GetMember(() => MySector.SunProperties.AmbientMultiplier));

                AddSlider(new StringBuilder("Env. ambient intensity"), 0, 5.0f, sunObj, MemberHelper.GetMember(() => MySector.SunProperties.EnvironmentAmbientIntensity));
            }

            /*
            //m_currentPosition.Y += 0.01f;
            AddLabel(new StringBuilder("Player ship"), Color.Yellow.ToVector4(), 1.2f);

            //m_currentPosition.Y += 0.01f;
            AddSlider(new StringBuilder("Light range multiplier"), 0, 10, null, MemberHelper.GetMember(() => MySmallShip.LightRangeMultiplier));
            AddSlider(new StringBuilder("Light intensity multiplier"), 0, 10, null, MemberHelper.GetMember(() => MySmallShip.LightIntensityMultiplier));
            AddSlider(new StringBuilder("Reflector intensity multiplier"), 0, 10, null, MemberHelper.GetMember(() => MySmallShip.ReflectorIntensityMultiplier));
            */
            //m_currentPosition.Y += 0.01f;
            AddLabel(new StringBuilder("Post process"), Color.Yellow.ToVector4(), 1.2f);
            //m_currentPosition.Y += 0.01f;
            MyPostProcessContrast contrastPP = MyRender.GetPostProcess(MyPostProcessEnum.Contrast) as MyPostProcessContrast;
            if (contrastPP != null)
            {
                AddSlider(new StringBuilder("Contrast"), -10, 10, contrastPP, MemberHelper.GetMember(() => contrastPP.Contrast));
                AddSlider(new StringBuilder("Hue"), -10, 10, contrastPP, MemberHelper.GetMember(() => contrastPP.Hue));
                AddSlider(new StringBuilder("Saturation"), -10, 10, contrastPP, MemberHelper.GetMember(() => contrastPP.Saturation));
            }

    
            m_currentPosition.Y += 0.01f;
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenDebugRenderColors";
        }

    }
}
