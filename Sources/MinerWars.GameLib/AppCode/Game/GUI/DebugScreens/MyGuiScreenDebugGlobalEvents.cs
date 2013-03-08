#region Using
using System;
using System.Collections.Generic;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.GUI.Helpers;
using MinerWars.AppCode.Game.HUD;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Entities.Ships.SubObjects;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Render;
using MinerWars.AppCode.Game.Effects;
using MinerWars.AppCode.Game.World.Global;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.Utils;
using SysUtils;
using KeenSoftwareHouse.Library.Extensions;
using MinerWars.AppCode.Game.Models;
using MinerWars.AppCode.Game;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Physics;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.AppCode.Game.Entities.Prefabs;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.AppCode.Game.Prefabs;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Gameplay;
using SysUtils.Utils;


#endregion

namespace MinerWars.AppCode.Game.GUI
{
    class MyGuiScreenDebugGlobalEvents: MyGuiScreenDebugBase
    {

        public MyGuiScreenDebugGlobalEvents()
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

            AddCaption(new System.Text.StringBuilder("Global Events Debug"), Color.Yellow.ToVector4());


            MyGuiControlLabel label = new MyGuiControlLabel(this, new Vector2(0.01f, -m_size.Value.Y / 2.0f + 0.07f), null, new System.Text.StringBuilder("(press ALT to share focus)"), Color.Yellow.ToVector4(), MyGuiConstants.LABEL_TEXT_SCALE * 0.7f,
                               MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_TOP);
            Controls.Add(label);


            m_currentPosition = -m_size.Value / 2.0f + new Vector2(0.12f, 0.15f);

            var globalEventEnumValues = Enum.GetValues(typeof (MyGlobalEventEnum));

            foreach (MyGlobalEventEnum globalEventEnum in globalEventEnumValues)
            {

                var globalEvent = MyGlobalEvents.GetGlobalEventByType(globalEventEnum);
                var button = AddButton(MyTextsWrapper.Get(globalEvent.Name), OnGlobalEvent);
                button.UserData = globalEventEnum;

            }


        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenDebugGlobalEvents";
        }


        private void OnGlobalEvent(MyGuiControlButton sender)
        {
            if (MyGuiScreenGamePlay.Static != null)
            {
                var globalEventEnum = (MyGlobalEventEnum)sender.UserData;
                MyGlobalEvents.StartGlobalEvent(globalEventEnum);
            }
        }

    }
}
