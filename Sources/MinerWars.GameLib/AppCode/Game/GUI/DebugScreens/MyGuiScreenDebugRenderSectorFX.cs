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

    class MyGuiScreenDebugRenderSectorFX : MyGuiScreenDebugBase
    {
        public MyGuiScreenDebugRenderSectorFX()
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

            m_scale = 0.5f;

            AddCaption(new System.Text.StringBuilder("Render sector FX"), Color.Yellow.ToVector4());

            MyGuiControlLabel label = new MyGuiControlLabel(this, new Vector2(0.01f, -m_size.Value.Y / 2.0f + 0.07f), null, new System.Text.StringBuilder("(press ALT to share focus)"), Color.Yellow.ToVector4(), MyGuiConstants.LABEL_TEXT_SCALE * 0.7f,
                               MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_TOP);
            Controls.Add(label);

            m_currentPosition = -m_size.Value / 2.0f + new Vector2(0.02f, 0.10f);

            m_currentPosition.Y += 0.01f * m_scale;
            AddCheckBox(new StringBuilder("Enable dust"), MySector.ParticleDustProperties, MemberHelper.GetMember(() => MySector.ParticleDustProperties.Enabled));
                                       
            m_currentPosition.Y += 0.01f * m_scale;
            AddLabel(new StringBuilder("Nebula"), Color.Yellow.ToVector4(), 1.2f);

            MyImpostorProperties nebulaObj = null;
            foreach (MyImpostorProperties nebulaObjIt in MySector.ImpostorProperties)
            {
                if (nebulaObjIt.ImpostorType == MyVoxelMapImpostors.MyImpostorType.Nebula)
                {
                    nebulaObj = nebulaObjIt;
                    break;
                }
            }

            if (nebulaObj != null)
            {
                AddCheckBox(new StringBuilder("Enable"), nebulaObj, MemberHelper.GetMember(() => nebulaObj.Enabled));
                AddColor(new StringBuilder("Color"), nebulaObj, MemberHelper.GetMember(() => nebulaObj.Color));
                AddSlider(new StringBuilder("Contrast"), 0, 20, nebulaObj, MemberHelper.GetMember(() => nebulaObj.Contrast));
                AddSlider(new StringBuilder("Intensity"), 0, 20, nebulaObj, MemberHelper.GetMember(() => nebulaObj.Intensity));
                AddSlider(new StringBuilder("Radius"), 0, 10, nebulaObj, MemberHelper.GetMember(() => nebulaObj.Radius));
                AddSlider(new StringBuilder("Anim1"), -0.1f, 0.1f, nebulaObj, MemberHelper.GetMember(() => nebulaObj.Anim1));
                AddSlider(new StringBuilder("Anim2"), -0.1f, 0.1f, nebulaObj, MemberHelper.GetMember(() => nebulaObj.Anim2));
                AddSlider(new StringBuilder("Anim3"), -0.1f, 0.1f, nebulaObj, MemberHelper.GetMember(() => nebulaObj.Anim3));
            }

                                     
            m_currentPosition.Y += 0.01f * m_scale;
            AddLabel(new StringBuilder("God rays"), Color.Yellow.ToVector4(), 1.2f);

            AddCheckBox(new StringBuilder("Enable"), MySector.GodRaysProperties, MemberHelper.GetMember(() => MySector.GodRaysProperties.Enabled));
            AddSlider(new StringBuilder("Density"), 0, 2, MySector.GodRaysProperties, MemberHelper.GetMember(() => MySector.GodRaysProperties.Density));
            AddSlider(new StringBuilder("Weight"), 0, 2, MySector.GodRaysProperties, MemberHelper.GetMember(() => MySector.GodRaysProperties.Weight));
            AddSlider(new StringBuilder("Decay"), 0, 2, MySector.GodRaysProperties, MemberHelper.GetMember(() => MySector.GodRaysProperties.Decay));
            AddSlider(new StringBuilder("Exposition"), 0, 2, MySector.GodRaysProperties, MemberHelper.GetMember(() => MySector.GodRaysProperties.Exposition));

            m_currentPosition.Y += 0.01f * m_scale;
            AddLabel(new StringBuilder("Particle dust"), Color.Yellow.ToVector4(), 1.2f);

            AddSlider(new StringBuilder("Dust radius"), 0.01f, 200, MySector.ParticleDustProperties , MemberHelper.GetMember(() => MySector.ParticleDustProperties.DustBillboardRadius));
            AddSlider(new StringBuilder("Dust count in dir half"), 0.01f, 20, MySector.ParticleDustProperties , MemberHelper.GetMember(() => MySector.ParticleDustProperties.DustFieldCountInDirectionHalf));
            AddSlider(new StringBuilder("Distance between"), 1f, 500, MySector.ParticleDustProperties , MemberHelper.GetMember(() => MySector.ParticleDustProperties.DistanceBetween));
            AddSlider(new StringBuilder("Anim speed"), 0.0f, 0.1f, MySector.ParticleDustProperties, MemberHelper.GetMember(() => MySector.ParticleDustProperties.AnimSpeed));
            AddColor(new StringBuilder("Color"), MySector.ParticleDustProperties, MemberHelper.GetMember(() => MySector.ParticleDustProperties.Color));

        }

        private bool nebula_selector(MyImpostorProperties properties)
        {
            return properties.ImpostorType == MyVoxelMapImpostors.MyImpostorType.Nebula;
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenDebugRenderSectorFX";
        }

    }
}
