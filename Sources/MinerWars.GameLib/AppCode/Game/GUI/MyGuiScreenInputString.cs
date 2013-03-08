using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.GUI.ScreenEditor;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Utils;

namespace MinerWars.AppCode.Game.GUI
{
    delegate void InputResultHandler(ScreenResult result, string resultText);

    class MyGuiScreenInputString : MyGuiScreenEditorDialogBase
    {
        readonly MyGuiControlTextbox m_nameTextbox;

        readonly InputResultHandler m_resultHandler;

        public override string GetFriendlyName()
        {
            return "MyGuiScreenInputString";
        }

        public MyGuiScreenInputString(InputResultHandler resultHandler, MyTextsWrapperEnum caption, StringBuilder defaultValue = null, int maxLength = 128)
            : base(new Vector2(0.5f, 0.5f), new Vector2(0.33f, 0.85f))
        {
            m_resultHandler = resultHandler;
            m_size = MyGuiConstants.TEXTBOX_MEDIUM_SIZE +
                     new Vector2(MyGuiConstants.MESSAGE_BOX_BORDER_AREA_X * 2,
                                 0.1f + MyGuiConstants.MESSAGE_BOX_BORDER_AREA_Y * 2 +
                                 MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE.Y);

            AddCaption(caption, MyGuiConstants.LABEL_TEXT_COLOR);

            m_nameTextbox = new MyGuiControlTextbox(this,
                new Vector2(0, 0.1f + MyGuiConstants.TEXTBOX_MEDIUM_SIZE.Y / 2 - m_size.Value.Y / 2),
                MyGuiControlPreDefinedSize.MEDIUM,
                defaultValue != null ? defaultValue.ToString() : "",
                maxLength,
                MyGuiConstants.LABEL_TEXT_COLOR,
                MyGuiConstants.LABEL_TEXT_SCALE,
                MyGuiControlTextboxType.NORMAL);

            Controls.Add(m_nameTextbox);
            AddOkAndCancelButtonControls();
        }

        protected override void OnOkClick(MyGuiControlButton sender)
        {
            base.OnOkClick(sender);
            m_resultHandler(ScreenResult.Ok, m_nameTextbox.Text);
        }

        protected override void OnCancelClick(MyGuiControlButton sender)
        {
            base.OnCancelClick(sender);
            m_resultHandler(ScreenResult.Cancel, m_nameTextbox.Text);
        }
    }
}