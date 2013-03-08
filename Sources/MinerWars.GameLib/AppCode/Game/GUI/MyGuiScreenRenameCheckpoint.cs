using MinerWarsMath;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.GUI.ScreenEditor;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Utils;
using System;

namespace MinerWars.AppCode.Game.GUI
{
    delegate void RenameHandler(string oldText, string newText, MyGuiScreenBase renameScreen);

    class MyGuiScreenRenameCheckpoint : MyGuiScreenEditorDialogBase
    {
        private static readonly int MAXIMUM_CHECKPOINT_NAME_LENGTH = 128;

        MyGuiControlTextbox m_nameTextbox;
        string m_oldName;

        RenameHandler m_renameHandler;

        public override string GetFriendlyName()
        {
            return "MyGuiScreenRenameCheckpoint";
        }

        public MyGuiScreenRenameCheckpoint(string changeThis, RenameHandler renameHandler)
            : base(new Vector2(0.5f, 0.5f), new Vector2(0.33f, 0.85f))
        {
            m_oldName = changeThis;
            m_renameHandler = renameHandler;
            m_size = MyGuiConstants.TEXTBOX_MEDIUM_SIZE +
                           new Vector2(MyGuiConstants.MESSAGE_BOX_BORDER_AREA_X * 2,
                                       0.1f + MyGuiConstants.MESSAGE_BOX_BORDER_AREA_Y * 2 +
                                       MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE.Y);

            AddCaption(MyTextsWrapperEnum.RenameCheckpoint, MyGuiConstants.LABEL_TEXT_COLOR);

            m_nameTextbox = new MyGuiControlTextbox(this,
                new Vector2(0, 0.1f + MyGuiConstants.TEXTBOX_MEDIUM_SIZE.Y / 2 - m_size.Value.Y / 2),
                MyGuiControlPreDefinedSize.MEDIUM,
                changeThis, MAXIMUM_CHECKPOINT_NAME_LENGTH,
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
            if (!string.IsNullOrEmpty(m_nameTextbox.Text) /* all OK */)
            {
                m_renameHandler(m_oldName, m_nameTextbox.Text, this);
                base.OnOkClick(sender);
            }
            else
            {
                // TODO: change texts
                MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, MyTextsWrapperEnum.MessageBoxCaptionError, MyTextsWrapperEnum.MessageBoxCaptionError, MyTextsWrapperEnum.Ok, null));
            }
        }
    }
}
