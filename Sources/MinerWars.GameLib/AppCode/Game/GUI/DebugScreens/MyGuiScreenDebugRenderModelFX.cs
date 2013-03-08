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
using MinerWars.AppCode.Game.Render.EnvironmentMap;
using MinerWars.AppCode.Game.Voxels;
using MinerWars.CommonLIB.AppCode.Networking;

namespace MinerWars.AppCode.Game.GUI
{
    //Prepared to be render debug screen

    class MyGuiScreenDebugRenderModelFX : MyGuiScreenDebugBase
    {
        public static bool EnableRenderLights = true;

        public MyGuiScreenDebugRenderModelFX()
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

            AddCaption(new System.Text.StringBuilder("Render Model FX"), Color.Yellow.ToVector4());

            MyGuiControlLabel label = new MyGuiControlLabel(this, new Vector2(0.01f, -m_size.Value.Y / 2.0f + 0.07f), null, new System.Text.StringBuilder("(press ALT to share focus)"), Color.Yellow.ToVector4(), MyGuiConstants.LABEL_TEXT_SCALE * 0.7f,
                               MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_TOP);
            Controls.Add(label);

            m_currentPosition = -m_size.Value / 2.0f + new Vector2(0.02f, 0.10f);


            m_currentPosition.Y += 0.01f;
            /*
            AddLabel(new StringBuilder("Channels"), Color.Yellow.ToVector4(), 1.2f);
            AddSlider(new StringBuilder("Channel0"), 0, 1.0f, null, MemberHelper.GetMember(() => MyRender.Channel0Intensity));
            AddSlider(new StringBuilder("Channel1"), 0, 1.0f, null, MemberHelper.GetMember(() => MyRender.Channel1Intensity));
              */
            AddLabel(new StringBuilder("Environmental maps"), Color.Yellow.ToVector4(), 1.2f);
            AddSlider(new StringBuilder("LOD0 max distance"), 50.0f, 1000.0f, null, MemberHelper.GetMember(() => MyEnvironmentMap.NearDistance));
            AddSlider(new StringBuilder("LOD1 max distance"), 50.0f, 1000.0f, null, MemberHelper.GetMember(() => MyEnvironmentMap.FarDistance));
            AddButton(new StringBuilder("Rebuild"), delegate { MyEnvironmentMap.Reset(); });

            var listbox = AddListbox();
            listbox.ItemSelect += new OnListboxItemSelect(listbox_ItemSelect);
            listbox.AddItem(-1, new StringBuilder("None"));
            for (int i = 0; i < MyVoxelMaterials.GetMaterialsCount(); i++)
            {
                MyMwcVoxelMaterialsEnum mat = (MyMwcVoxelMaterialsEnum)i;
                listbox.AddItem((int)mat, new StringBuilder(mat.ToString()));
            }
            listbox.SetSelectedItem(-1);
        }

        void listbox_ItemSelect(object sender, MyGuiControlListboxItemEventArgs eventArgs)
        {
            var key = eventArgs.Key;
            if (key < 0)
            {
                MyRender.OverrideVoxelMaterial = null;
            }
            else
            {
                MyRender.OverrideVoxelMaterial = (MyMwcVoxelMaterialsEnum)key;
            }
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenDebugRenderModelFX";
        }

    }
}
