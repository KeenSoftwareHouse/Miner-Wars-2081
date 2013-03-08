using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Utils;
using MinerWarsMath;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Localization;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.Audio;
using MinerWars.AppCode.Toolkit.Input;
using MinerWars.AppCode.Game.Editor;
using System;

namespace MinerWars.AppCode.Game.GUI.ScreenEditor.Object3D
{
    abstract class MyGuiScreenEditorObject3DBase : MyGuiScreenBase
    {
        public static readonly Vector2 CONTROLS_DELTA = new Vector2(0, 0.0525f);
        protected Vector3 m_newObjectPosition;
        protected MyEntity m_entity;
        protected MyTextsWrapperEnum m_caption;
        protected MyGuiControlTextbox m_positionXTextBox;
        protected MyGuiControlTextbox m_positionYTextBox;
        protected MyGuiControlTextbox m_positionZTextBox;
        protected MyGuiControlCheckbox m_activatedCheckbox;
        protected MyGuiControlTextbox m_idTextbox;
        
        protected Vector2? m_screenPosition;

        // Basic constructor
        public MyGuiScreenEditorObject3DBase(MyTextsWrapperEnum caption)
            : this(null, new Vector2(0.5f, 0.5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, null, caption)
        { }

        public MyGuiScreenEditorObject3DBase(Vector2 position, Vector4 backgroundColor, Vector2? size, MyTextsWrapperEnum caption, Vector2? screnPosition = null)
            : this(null, position, backgroundColor, size, caption, screnPosition)
        { }

        public MyGuiScreenEditorObject3DBase(MyEntity entity, MyTextsWrapperEnum caption)
            : this(caption)
        {
            m_entity = entity;
        }

        public MyGuiScreenEditorObject3DBase(MyEntity physObject, Vector2 position, Vector4 backgroundColor, Vector2? size, MyTextsWrapperEnum caption, Vector2? screenPosition = null)
            : base(position, backgroundColor, size)
        {
            m_caption = caption;
            m_newObjectPosition = MySpectator.Target + 2000 * MySpectator.Orientation;
            m_entity = physObject;
            m_backgroundFadeColor = MyGuiConstants.SCREEN_BACKGROUND_FADE_BLANK_DARK;
            m_backgroundFadeColor.W = 0.75f;
            m_screenPosition = screenPosition;
            this.OnEnterCallback = OnEnterClick;
        }

        protected virtual Vector2 GetControlsOriginLeftFromScreenSize()
        {
            return new Vector2(-m_size.Value.X / 2.0f + 0.03f, -m_size.Value.Y / 2.0f + 0.09f);
        }

        protected virtual Vector2 GetControlsOriginRightFromScreenSize()
        {
            return new Vector2(-m_size.Value.X / 2.0f + 0.25f, -m_size.Value.Y / 2.0f + 0.09f);
        }

        protected bool HasEntity()
        {
            return m_entity != null;
        }

        protected void AddCaption(Vector2 offset)
        {
            AddCaption(m_caption, new Vector2(0, 0.01f) + offset);
        }

        protected void AddCaption()
        {
            AddCaption(Vector2.Zero);
        }

        protected void AddEditPositionControls()
        {
            if (HasEntity())
            {
                Controls.Add(new MyGuiControlLabel(this, GetControlsOriginLeftFromScreenSize() + 0 * CONTROLS_DELTA, null, MyTextsWrapperEnum.Position, MyGuiConstants.LABEL_TEXT_COLOR,
                    MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));

                //TODO add support for textbox with numbers with floating point
                Controls.Add(new MyGuiControlLabel(this, GetControlsOriginLeftFromScreenSize() + 1 * CONTROLS_DELTA, null, MyTextsWrapperEnum.X, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
                m_positionXTextBox = new MyGuiControlTextbox(this, (new Vector2(0.05f, 0) + GetControlsOriginLeftFromScreenSize()) + 1 * CONTROLS_DELTA + new Vector2(MyGuiConstants.TEXTBOX_MEDIUM_SIZE.X / 2.0f, 0), MyGuiControlPreDefinedSize.MEDIUM, "0", MyMwcValidationConstants.POSITION_X_MAX, MyGuiConstants.TEXTBOX_BACKGROUND_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiControlTextboxType.DIGITS_ONLY);
                Controls.Add(m_positionXTextBox);
                m_positionXTextBox.Text = Convert.ToInt32(m_entity.GetPosition().X).ToString();

                Controls.Add(new MyGuiControlLabel(this, GetControlsOriginLeftFromScreenSize() + 2 * CONTROLS_DELTA, null, MyTextsWrapperEnum.Y, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
                m_positionYTextBox = new MyGuiControlTextbox(this, (new Vector2(0.05f, 0) + GetControlsOriginLeftFromScreenSize()) + 2 * CONTROLS_DELTA + new Vector2(MyGuiConstants.TEXTBOX_MEDIUM_SIZE.X / 2.0f, 0), MyGuiControlPreDefinedSize.MEDIUM, "0", MyMwcValidationConstants.POSITION_X_MAX, MyGuiConstants.TEXTBOX_BACKGROUND_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiControlTextboxType.DIGITS_ONLY);
                Controls.Add(m_positionYTextBox);
                m_positionYTextBox.Text = Convert.ToInt32(m_entity.GetPosition().Y).ToString();

                Controls.Add(new MyGuiControlLabel(this, GetControlsOriginLeftFromScreenSize() + 3 * CONTROLS_DELTA, null, MyTextsWrapperEnum.Z, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
                m_positionZTextBox = new MyGuiControlTextbox(this, (new Vector2(0.05f, 0) + GetControlsOriginLeftFromScreenSize()) + 3 * CONTROLS_DELTA + new Vector2(MyGuiConstants.TEXTBOX_MEDIUM_SIZE.X / 2.0f, 0), MyGuiControlPreDefinedSize.MEDIUM, "0", MyMwcValidationConstants.POSITION_X_MAX, MyGuiConstants.TEXTBOX_BACKGROUND_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiControlTextboxType.DIGITS_ONLY);
                Controls.Add(m_positionZTextBox);
                m_positionZTextBox.Text = Convert.ToInt32(m_entity.GetPosition().Z).ToString();
            }
        }

        protected void AddActivatedCheckbox(Vector2 position, bool value) 
        {
            Controls.Add(new MyGuiControlLabel(this, position, null, MyTextsWrapperEnum.Active, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            m_activatedCheckbox = new MyGuiControlCheckbox(this, position + new Vector2(0.1f, 0f), value, MyGuiConstants.CHECKBOX_BACKGROUND_COLOR);
            Controls.Add(m_activatedCheckbox);
        }

        protected void AddIdTextBox(Vector2 position, uint value)
        {
            if (HasEntity() && MyFakes.ENABLE_ENTITY_ID_CHANGE)
            {
                Controls.Add(new MyGuiControlLabel(this, position, null, MyTextsWrapperEnum.EntityId, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
                m_idTextbox = new MyGuiControlTextbox(this, position + new Vector2(0.2f, 0f), MyGuiControlPreDefinedSize.MEDIUM, Convert.ToString(value), 10, MyGuiConstants.TEXTBOX_BACKGROUND_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiControlTextboxType.DIGITS_ONLY);
                Controls.Add(m_idTextbox);
            }
        }

        protected void AddOkButtonControl()
        {
            AddOkButtonControl(Vector2.Zero);
        }

        protected void AddOkButtonControl(Vector2 offset)
        {
            //  Buttons OK
            Vector2 buttonDelta = new Vector2(0.05f, m_size.Value.Y / 2.0f - MyGuiConstants.MESSAGE_BOX_BORDER_AREA_Y - MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE.Y / 2.0f - 0.01f) + offset;
            Controls.Add(new MyGuiControlButton(this, new Vector2(0, buttonDelta.Y), MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.Ok, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnOkClick, MyGuiControlButtonTextAlignment.CENTERED, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));
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

        void OnEnterClick()
        {
            HandleKeyboardActiveIndex(true);
        }

        public virtual void OnOkClick(MyGuiControlButton sender)
        {
            if (HasEntity())
            {
                if (m_idTextbox != null)
                {
                    if (m_entity.EntityId != null)
                    {
                        var newId = new MyEntityIdentifier(Convert.ToUInt32(m_idTextbox.Text));

                        if (newId.NumericValue != m_entity.EntityId.Value.NumericValue && !MyEntityIdentifier.ExistsById(newId))
                        {
                            MyEntityIdentifier.RemoveEntity(m_entity.EntityId.Value);
                            m_entity.EntityId = newId;
                            MyEntityIdentifier.AddEntityWithId(m_entity);
                        }
                    }
                }

                if (IsPositionInput())
                {
                    Vector3 position = GetNewPosition();
                    if (IsValidPositionInput(position))
                    {
                        MyEditorGizmo.MoveAndRotateObject(position, m_entity.GetWorldRotation(), m_entity);
                    }
                }
            }            
        }        

        public virtual void OnCancelClick(MyGuiControlButton sender)
        {
            CloseScreen();
        }

        // Create Vector3 for position from GUI input for x,y,z coordinates
        public Vector3 GetNewPosition()
        {
            int x = Convert.ToInt32(m_positionXTextBox.Text);
            int y = Convert.ToInt32(m_positionYTextBox.Text);
            int z = Convert.ToInt32(m_positionZTextBox.Text);

            return new Vector3(x, y, z);
        }

        bool IsPositionInput()
        {
            return (m_positionXTextBox != null) && (m_positionYTextBox != null) && (m_positionZTextBox != null);
        }

        // 
        public bool IsValidPositionInput(Vector3 position)
        {
            if (position != MyEditorGizmo.GetSelectedObjectsCenter())
            {
                return true;
            }
            else
            {
                MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, MyTextsWrapperEnum.InvalidPositionCoordinates, MyTextsWrapperEnum.MessageBoxCaptionError, MyTextsWrapperEnum.Ok, null));
                return false;
            }
        }

        public override void HandleInput(MyGuiInput input, bool receivedFocusInThisUpdate)
        {
            base.HandleInput(input, receivedFocusInThisUpdate);

            if (input.IsNewKeyPress(Keys.Escape))
            {
                //CloseScreen();
                OnCancelClick(null);
            }
        }
    }
}
