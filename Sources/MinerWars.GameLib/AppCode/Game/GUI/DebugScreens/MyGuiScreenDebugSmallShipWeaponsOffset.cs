using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KeenSoftwareHouse.Library.Extensions;
using MinerWarsMath;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Entities.Weapons;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Entities;

namespace MinerWars.AppCode.Game.GUI.DebugScreens
{
    class MyGuiScreenDebugSmallShipWeaponsOffset : MyGuiScreenDebugBase
    {
        private const float MIN_OFFSET = -3.0f;
        private const float MAX_OFFSET = 3.0f;

        public MyGuiScreenDebugSmallShipWeaponsOffset()
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

            AddCaption(new StringBuilder("Small ship weapons"), Color.Yellow.ToVector4());

            MyGuiControlLabel label = 
                new MyGuiControlLabel(this, new Vector2(0.01f, -m_size.Value.Y / 2.0f + 0.07f), null, new StringBuilder("(press ALT to share focus)"), 
                                      Color.Yellow.ToVector4(), MyGuiConstants.LABEL_TEXT_SCALE * 0.7f, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_TOP);
            Controls.Add(label);

            AddSlider(new StringBuilder("Drill ejecting speed"), 0.10f, 0.50f, null,
                      MemberHelper.GetMember(() => MyDrillDeviceConstants.DRILL_EJECTING_SPEED));

            AddSlider(new StringBuilder("LightOffsetX"), -20, 20, null, MemberHelper.GetMember(() => MySmallShip.LightOffsetX));
            AddSlider(new StringBuilder("LightOffsetY"), -20, 20, null, MemberHelper.GetMember(() => MySmallShip.LightOffsetY));
            AddSlider(new StringBuilder("LightOffsetZ"), -20, 20, null, MemberHelper.GetMember(() => MySmallShip.LightOffsetZ));

            m_scale = 0.7f;
            m_currentPosition = -m_size.Value / 2.0f + new Vector2(0.02f, 0.10f);

            if (MyGuiScreenGamePlay.Static != null && MySession.PlayerShip != null)
            {
                List<MySmallShipGunBase> shipGuns = MySession.PlayerShip.Weapons.GetMountedWeaponsWithHarvesterAndDrill();
                for(int i = 0; i < shipGuns.Count; i++)
                {
                    MySmallShipGunBase shipGun = shipGuns[i];
                    //AddSlider(new StringBuilder(shipGun.WeaponType + "_" + i), MIN_OFFSET, MAX_OFFSET, shipGun, MemberHelper.GetMember(() => shipGun.YOffset));
                    AddSlider(new StringBuilder(shipGun.WeaponType + "_" + i), MIN_OFFSET, MAX_OFFSET, shipGun, MemberHelper.GetMember(() => shipGun.ZOffset));
                }
            }

            m_currentPosition.Y += 0.01f;
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenDebugSmallShipWeaponsOffset";
        }
    }
}
