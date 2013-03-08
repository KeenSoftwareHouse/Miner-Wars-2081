using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWarsMath;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Localization;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.Editor;
using MinerWars.AppCode.Game.GUI.Helpers;

namespace MinerWars.AppCode.Game.GUI.ScreenEditor
{
    class MyGuiScreenSaveSector : MyGuiScreenEditorDialogBase
    {
        private static readonly int MAXIMUM_SECTOR_NAME_LENGTH = 32;
        private MyGuiControlTextbox m_nameTextbox;

        public override string GetFriendlyName()
        {
            return "MyGuiScreenEditorSaveSector";
        }

        public MyGuiScreenSaveSector()
            : base(new Vector2(0.5f, 0.5f), new Vector2(0.33f, 0.85f))
        {
            m_size = MyGuiConstants.TEXTBOX_MEDIUM_SIZE +
                           new Vector2(MyGuiConstants.MESSAGE_BOX_BORDER_AREA_X * 2,
                                       0.1f + MyGuiConstants.MESSAGE_BOX_BORDER_AREA_Y * 2 +
                                       MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE.Y);

            AddCaption(MyTextsWrapperEnum.SaveSector, MyGuiConstants.LABEL_TEXT_COLOR);

            string sectorName = MyGuiScreenGamePlay.Static.GetSectorIdentifier().SectorName;
            sectorName = sectorName == null ? string.Empty : sectorName;

            m_nameTextbox = new MyGuiControlTextbox(this,
                new Vector2(0, 0.1f + MyGuiConstants.TEXTBOX_MEDIUM_SIZE.Y / 2 - m_size.Value.Y / 2),
                MyGuiControlPreDefinedSize.MEDIUM,
                sectorName, MAXIMUM_SECTOR_NAME_LENGTH,
                MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiControlTextboxType.NORMAL);

            Controls.Add(m_nameTextbox);
            AddOkAndCancelButtonControls();
        }

        public override void HandleInput(MyGuiInput input, bool receivedFocusInThisUpdate)
        {
            base.HandleInput(input, receivedFocusInThisUpdate);
        }

        protected override void OnOkClick(MyGuiControlButton sender)
        {
            base.OnOkClick(sender);
            
            MyGuiScreenGamePlay.Static.SetSectorName(m_nameTextbox.Text);
            MyEditor.Static.PerformContextMenuAction(MyGuiContextMenuItemActionType.SAVE_SECTOR);            
        }


    }
}
