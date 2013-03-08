using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Toolkit.Input;
using MinerWars.AppCode.Game.Managers;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Audio;

namespace MinerWars.AppCode.Game.GUI.Core
{
    class MyGuiControlCheckbox : MyGuiControlBase
    {
        public delegate void OnCheckBoxCheckCallback(MyGuiControlCheckbox sender);
        public OnCheckBoxCheckCallback OnCheck;

        bool m_checked;
        bool m_highlightWhenChecked;
        MyTexture2D m_checkedTexture;
        MyGuiControlLabel m_label;
        bool m_highlight;

        private Vector2? m_innerSize; // if we have bigger texture than control
        //public MyGuiControlCheckbox(MyGuiScreenBase parentScreen, Vector2 position, Vector2 size, bool checkedVal, Vector4 color)
        //    : base(parentScreen, position, size, color, null, MyGuiManager.GetCheckboxTexture(), null, null, true)
        //{
        //    m_canHandleKeyboardActiveControl = true;
        //    m_checked = checkedVal;
        //    m_backgroundColor = color;
        //}


        public MyGuiControlCheckbox(IMyGuiControlsParent parent, Vector2 position, bool checkedVal, Vector4 color, MyGuiControlLabel label)
            : this(parent, position, MyGuiConstants.CHECKBOX_WITH_GLOW_SIZE, MyGuiManager.GetCheckboxOffTexture(), MyGuiManager.GetCheckboxOnTexture(), null, checkedVal, color, false, label)
        {
        }


        public MyGuiControlCheckbox(IMyGuiControlsParent parent, Vector2 position, Vector2 size, bool checkedVal, Vector4 color)
            : this(parent, position, size, MyGuiManager.GetCheckboxOffTexture(), MyGuiManager.GetCheckboxOnTexture(), null, checkedVal, color, false, null)
        {            
        }

        public MyGuiControlCheckbox(IMyGuiControlsParent parent, Vector2 position, bool checkedVal, Vector4 color)
            : this(parent, position, MyGuiConstants.CHECKBOX_WITH_GLOW_SIZE, checkedVal, color)
        {
        }


        public MyGuiControlCheckbox(IMyGuiControlsParent parent, Vector2 position, Vector2 size, MyTexture2D texture, MyTexture2D checkedTexture,
    StringBuilder toolTip, bool checkedVal, Vector4 color, bool highlightWhenChecked, bool canHandeKeyboard )
            : this(parent, position, size, texture, checkedTexture, toolTip, checkedVal, color, highlightWhenChecked, null)
        {
            m_canHandleKeyboardActiveControl = canHandeKeyboard;
        }

        public MyGuiControlCheckbox(IMyGuiControlsParent parent, Vector2 position, Vector2 size, MyTexture2D texture, MyTexture2D checkedTexture,
StringBuilder toolTip, bool checkedVal, Vector4 color, bool highlightWhenChecked, bool canHandeKeyboard, Vector2 innerSize)
            : this(parent, position, size, texture, checkedTexture, toolTip, checkedVal, color, highlightWhenChecked, null, innerSize)
        {
            m_canHandleKeyboardActiveControl = canHandeKeyboard;
        }


        public MyGuiControlCheckbox(IMyGuiControlsParent parent, Vector2 position, Vector2 size, MyTexture2D texture, MyTexture2D checkedTexture,
            StringBuilder toolTip, bool checkedVal, Vector4 color, bool highlightWhenChecked)
            : this(parent, position, size, texture, checkedTexture, toolTip, checkedVal, color, highlightWhenChecked, null)
        {   
        }

        public MyGuiControlCheckbox(IMyGuiControlsParent parent, Vector2 position, Vector2 size, MyTexture2D texture, MyTexture2D checkedTexture,
            StringBuilder toolTip, bool checkedVal, Vector4 color, bool highlightWhenChecked, MyGuiControlLabel label, Vector2? innerSize = null)
            : base(parent, position, size, color, toolTip, texture, null, null, true)
        {
            m_canHandleKeyboardActiveControl = true;
            m_checked = checkedVal;
            m_highlightWhenChecked = false; // highlightWhenChecked; this feature is depracted
            m_checkedTexture = checkedTexture;
            m_label = label;
            if (m_label != null) {
                m_label.MouseEnter += delegate
                {
                    m_highlight = true;
                };
                m_label.MouseLeave += delegate
                {
                    m_highlight = false;
                };
                m_label.Click += delegate
                {
                    UserCheck();
                };
            }
            if (innerSize == null) m_innerSize = size;
            else m_innerSize = innerSize;
        }

        //  Checks if mouse cursor is over control
        protected override bool CheckMouseOver()
        {
            //  If size isn't specified, this test can't be done and that was probably intend
            if (m_innerSize.HasValue == false) return false;

            return CheckMouseOver(m_innerSize.Value * m_scale, m_parent.GetPositionAbsolute());
        }


        public void SetSize(Vector2 size)
        {
            m_size = size;
        }

        public bool Checked
        {
            set
            {
                bool fireEvent = m_checked != value;
                m_checked = value;

                if (fireEvent && OnCheck != null) 
                    OnCheck(this);
            }

            get
            {
                return m_checked;
            }
        }

        private void UserCheck()
        {
            MyAudio.AddCue2D(MySoundCuesEnum.GuiMouseClick);
            Checked = !Checked;
        }


        //this twho methods doesnt fire event
        public void Check()
        {
            m_checked = true;
        }

        public void UnCheck()
        {
            m_checked = false;
        }

        //  Method returns true if input was captured by control, so no other controls, nor screen can use input in this update
        public override bool HandleInput(MyGuiInput input, bool hasKeyboardActiveControl, bool hasKeyboardActiveControlPrevious, bool isThisFirstHandleInput)
        {
            if (!Enabled)
                return false;

            bool ret = base.HandleInput(input, hasKeyboardActiveControl, hasKeyboardActiveControlPrevious, isThisFirstHandleInput);

            if (ret == false)
            {
                if (((IsMouseOver() == true) && input.IsNewLeftMousePressed() ) ||
                    ((hasKeyboardActiveControl == true) && ((input.IsNewKeyPress(Keys.Enter) || (input.IsNewKeyPress(Keys.Space)) || (input.IsNewGameControlPressed(MyGameControlEnums.FIRE_PRIMARY) && !input.IsNewLeftMousePressed())))))
                {
                    UserCheck();

                    ret = true;
                }
            }

            return ret;
        }


    
        public override void Draw()
        {
            Vector4? oldBackgroundColor = m_backgroundColor;

            if (m_highlight == true)
            {
                if (m_backgroundColor != null) {
                    m_backgroundColor = m_backgroundColor.Value * MyGuiConstants.CONTROL_MOUSE_OVER_BACKGROUND_COLOR_MULTIPLIER;
                }
            }
            
            base.Draw();

            m_backgroundColor = oldBackgroundColor;

            if (m_checked == true)
            {
                Vector4 checkedColor = !Enabled ? MyGuiConstants.DISABLED_BUTTON_COLOR_VECTOR : (IsMouseOverOrKeyboardActive() || m_highlightWhenChecked) ?
                    m_backgroundColor.Value * MyGuiConstants.CONTROL_MOUSE_OVER_BACKGROUND_COLOR_MULTIPLIER : m_backgroundColor.Value;

                //  Checked sprite (fajka)
                MyGuiManager.DrawSpriteBatch(m_checkedTexture, m_parent.GetPositionAbsolute() + m_position, m_size.Value,
                    GetColorAfterTransitionAlpha(checkedColor), MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER);
            }
        }
    }
}
