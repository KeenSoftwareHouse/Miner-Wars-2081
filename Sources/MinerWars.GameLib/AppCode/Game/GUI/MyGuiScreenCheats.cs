using System;
using System.Collections.Generic;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Render;
using MinerWars.CommonLIB.AppCode.Utils;
using SysUtils;
using KeenSoftwareHouse.Library.Extensions;

namespace MinerWars.AppCode.Game.GUI
{
    using System.Diagnostics;
    using MinerWars.AppCode.Game.GUI.DebugScreens;
    using MinerWars.AppCode.Game.Gameplay;

    class MyGuiScreenCheats : MyGuiScreenDebugBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MyGuiScreenDebugDeveloper"/> class.
        /// </summary>
        public MyGuiScreenCheats()
            : base(new Vector2(.5f, .5f), new Vector2(0.35f, 0.92f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, true)
        {
            AddCaption(MyTextsWrapperEnum.CheatsCaption, new Vector2(0f, 0.04f), captionScale:1.35f);

            m_closeOnEsc = true;
            m_checkBoxOffset = 0.015f;

            m_currentPosition = -m_size.Value / 2.0f + new Vector2(0.03f, /*0.15f*//*0.05f*/0.15f);

            if (!MyGuiScreenGamePlay.Static.CheatsEnabled())
            {
                m_currentPosition.Y -= 0.025f;
                AddLabel(MyTextsWrapper.Get(MyTextsWrapperEnum.PurchaseCheats), Vector4.One, 0.725f, font:MyGuiManager.GetFontMinerWarsRed());
                m_currentPosition.Y -= 0.005f;
            }

            foreach (MyGameplayCheat cheat in MyGameplayCheats.AllCheats)
            {
                AddCheat(cheat);
            }

            m_currentPosition.Y += 0.01f;

            m_enableBackgroundFade = true;
            m_backgroundFadeColor = new Vector4(0.0f,0.0f,0.0f,0.5f);

            // resize for drawing
            m_size = new Vector2(0.425f, 0.92f);
        }

        bool m_lastWasButton;

        void AddCheat(MyGameplayCheat cheat)
        {
            bool enabled = MyGuiScreenGamePlay.Static.CheatsEnabled() && cheat.IsImplemented;
            MyGuiControlBase control;
            if (cheat.IsButton)
            {
                if (!m_lastWasButton) m_currentPosition.Y += 0.005f * m_scale;
                m_lastWasButton = true;
                control = AddButton(MyTextsWrapper.Get(cheat.CheatName), onCheatButtonClick, textColor: Vector4.One, size: new Vector2(0.20f, 0.04f));
            }
            else
            {
                if (m_lastWasButton) m_currentPosition.Y += 0.005f * m_scale;
                m_lastWasButton = false;
                control = AddCheckBox(cheat.CheatName, MyGameplayCheats.IsCheatEnabled(cheat.CheatEnum), OnCheatCheckedChanged, enabled, color: Vector4.One);
            }

            control.UserData = cheat;

            control.Enabled = enabled;
        }

        void OnCheatCheckedChanged(MyGuiControlCheckbox sender)
        {
            MyGameplayCheat cheat = (MyGameplayCheat)sender.UserData;
            MyGameplayCheats.EnableCheat(cheat.CheatEnum, sender.Checked);
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenCheats";
        }

        void onCheatButtonClick(MyGuiControlButton sender)
        {
            MyGameplayCheat cheat = (MyGameplayCheat)sender.UserData;
            MyGameplayCheats.EnableCheat(cheat.CheatEnum, true);
        }
  
    }
}
