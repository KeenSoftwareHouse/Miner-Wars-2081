using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Localization;

namespace MinerWars.AppCode.Game.GUI.ScreenEditor
{
    // Helper class with base dialog fuctions
    abstract class MyGuiScreenEditorDialogBase : MyGuiScreenBase
    {
        protected static readonly float CAPTION_OFFSET_Y = 0.1f;

        public MyGuiScreenEditorDialogBase(Vector2 position, Vector2 size)
            : base(position, MyGuiConstants.SCREEN_BACKGROUND_COLOR, size)
        {
            this.OnEnterCallback = OnEnter;
        }

        protected void AddOkAndCancelButtonControls()
        {
            AddOkAndCancelButtonControls(Vector2.Zero);
        }

        protected void AddOkAndCancelButtonControls(Vector2 offset)
        {
            //  Buttons OK and CANCEL
            Vector2 buttonDelta = new Vector2(0.1f, m_size.Value.Y / 2.0f - MyGuiConstants.MESSAGE_BOX_BORDER_AREA_Y - MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE.Y / 2.0f - 0.01f) + offset;
            Controls.Add(new MyGuiControlButton(this, new Vector2(-buttonDelta.X, buttonDelta.Y), MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.Ok, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnOkClick, MyGuiControlButtonTextAlignment.CENTERED, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));
            Controls.Add(new MyGuiControlButton(this, new Vector2(+buttonDelta.X, buttonDelta.Y), MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.Cancel, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnCancelClick, MyGuiControlButtonTextAlignment.CENTERED, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));
        }

        protected void AddBackButtonControl()
        {
            AddBackButtonControl(Vector2.Zero);
        }

        protected void AddBackButtonControl(Vector2 offset)
        {
            Vector2 buttonDelta = new Vector2(0.05f, m_size.Value.Y / 2.0f - MyGuiConstants.MESSAGE_BOX_BORDER_AREA_Y - MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE.Y / 2.0f - 0.01f) + offset;
            Controls.Add(new MyGuiControlButton(this, new Vector2(0.0f, m_size.Value.Y / 2.0f - MyGuiConstants.MESSAGE_BOX_BORDER_AREA_Y - MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE.Y / 2.0f) + offset, MyGuiConstants.BACK_BUTTON_SIZE, MyGuiConstants.BACK_BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.Back, MyGuiConstants.BACK_BUTTON_TEXT_COLOR, MyGuiConstants.BACK_BUTTON_TEXT_SCALE, OnBackClick, MyGuiControlButtonTextAlignment.CENTERED, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));
        }

        protected virtual void OnOkClick(MyGuiControlButton sender)
        {
            CloseScreen();
        }

        protected virtual void OnCancelClick(MyGuiControlButton sender)
        {
            CloseScreen();
        }

        protected virtual void OnBackClick(MyGuiControlButton sender)
        {
            CloseScreen();
        }

        protected virtual void OnEnter()
        {
            OnOkClick(null);
        }
    }
}
