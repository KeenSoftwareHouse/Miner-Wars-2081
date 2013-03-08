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
    class MyGuiScreenDebugSystem : MyGuiScreenDebugBase
    {
        public MyGuiScreenDebugSystem()
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

            AddCaption(new System.Text.StringBuilder("System debug"), Color.Yellow.ToVector4());

            
            MyGuiControlLabel label = new MyGuiControlLabel(this, new Vector2(0.01f, -m_size.Value.Y / 2.0f + 0.07f), null, new System.Text.StringBuilder("(press ALT to share focus)"), Color.Yellow.ToVector4(), MyGuiConstants.LABEL_TEXT_SCALE * 0.7f,
                               MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_TOP);
            Controls.Add(label);


            m_currentPosition = -m_size.Value / 2.0f + new Vector2(0.02f, 0.10f);

            AddLabel(new StringBuilder("System"), Color.Yellow.ToVector4(), 1.2f);

            //System debugging
            AddCheckBox(MyTextsWrapperEnum.DrawCollisionSpotsInHud, null, MemberHelper.GetMember(() => MyMwcFinalBuildConstants.DrawCollisionSpotsInHud));
            AddCheckBox(MyTextsWrapperEnum.DrawVoxelContentAsBillboards, null, MemberHelper.GetMember(() => MyMwcFinalBuildConstants.DrawVoxelContentAsBillboards));
            AddCheckBox(MyTextsWrapperEnum.ShowVertexNormals, null, MemberHelper.GetMember(() => MyMwcFinalBuildConstants.DrawNormalVectors));
            AddCheckBox(new StringBuilder("Disable bots"), null, MemberHelper.GetMember(() => MyMwcFinalBuildConstants.DisableEnemyBots));
            AddCheckBox(new StringBuilder("Show waypoints"), null, MemberHelper.GetMember(() => MyMwcFinalBuildConstants.BOT_DEBUG_MODE));
            AddCheckBox(MyTextsWrapperEnum.LoggingInDrawUpdate, null, MemberHelper.GetMember(() => MyMwcFinalBuildConstants.EnableLoggingInDrawAndUpdateAndGuiLoops));
            AddCheckBox(MyTextsWrapperEnum.SimulateSlowUpdate, null, MemberHelper.GetMember(() => MyMwcFinalBuildConstants.SimulateSlowUpdate));
            AddCheckBox(MyTextsWrapperEnum.SimulateSlowDraw, null, MemberHelper.GetMember(() => MyMwcFinalBuildConstants.SimulateSlowDraw));
            AddCheckBox(MyTextsWrapperEnum.LogGarbageCollection, null, MemberHelper.GetMember(() => MyMwcFinalBuildConstants.EnableLoggingGarbageCollectionCalls));
            AddCheckBox(MyTextsWrapperEnum.SimulateLostMessages, MyMwcFinalBuildConstants.SIMULATE_LOST_MESSAGES_SENT_OUT > 0, SimulateLostMessagesChange);
            AddCheckBox(new StringBuilder("Must look at target in interactions"), null, MemberHelper.GetMember(() => MySmallShipInteraction.MUST_LOOK_AT_TARGET));

            m_currentPosition.Y += 0.01f;
            
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenDebugGame";
        }

        private void SimulateLostMessagesChange(MyGuiControlCheckbox sender)
        {
            MyMwcFinalBuildConstants.SIMULATE_LOST_MESSAGES_SENT_OUT = sender.Checked ? (int?)4 : null;
        }
    }
}
