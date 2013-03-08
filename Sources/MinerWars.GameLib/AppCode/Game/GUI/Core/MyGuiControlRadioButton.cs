using System;
using System.Collections.Generic;
using MinerWarsMath;
using MinerWars.AppCode.Toolkit.Input;
using MinerWars.AppCode.Game.Managers;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Audio;

namespace MinerWars.AppCode.Game.GUI.Core
{
    delegate void OnRadioButtonGroupSelectedChanged(MyGuiControlRadioButtonGroup sender);
    class MyGuiControlRadioButtonGroup
    {
        private List<MyGuiControlRadioButton> m_radioButtons;
        private MyGuiControlRadioButton m_selected;

        public MyGuiControlRadioButtonGroup()
        {
            m_radioButtons = new List<MyGuiControlRadioButton>();
            m_selected = null;
        }

        public void Add(MyGuiControlRadioButton radioButton)
        {
            m_radioButtons.Add(radioButton);
            radioButton.OnSelect += OnRadioButtonSelect;
        }

        public void Remove(MyGuiControlRadioButton radioButton)
        {
            radioButton.OnSelect -= OnRadioButtonSelect;
            m_radioButtons.Remove(radioButton);
        }

        public void Clear()
        {
            foreach (MyGuiControlRadioButton radioButton in m_radioButtons)
            {
                radioButton.OnSelect -= OnRadioButtonSelect;
            }
            m_radioButtons.Clear();
        }

        private void OnRadioButtonSelect(MyGuiControlRadioButton sender)
        {
            SetSelected(sender.Key);
        }

        public MyGuiControlRadioButton GetSelected()
        {
            return m_selected;
        }

        public int? GetSelectedKey() 
        {            
            MyGuiControlRadioButton selectedRadioButton = GetSelected();
            if (selectedRadioButton != null)
            {
                return m_selected.Key;
            }
            else 
            {
                return null;
            }
        }

        private void UnselectSelected() 
        {
            foreach (MyGuiControlRadioButton radioButton in m_radioButtons)
            {
                if (radioButton.Selected)
                {
                    radioButton.Selected = false;
                }
            }
            m_selected = null;
        }

        public void SetSelected(int key) 
        {            
            MyGuiControlRadioButton radioButtonByKey = null;
            foreach (MyGuiControlRadioButton radioButton in m_radioButtons) 
            {
                if (radioButton.Key == key) 
                {
                    radioButtonByKey = radioButton;
                    break;
                }
            }

            if (radioButtonByKey != null) 
            {
                UnselectSelected();

                radioButtonByKey.OnSelect -= OnRadioButtonSelect;
                radioButtonByKey.Selected = true;
                radioButtonByKey.OnSelect += OnRadioButtonSelect;

                m_selected = radioButtonByKey;

                if(OnSelectedChanged != null)
                {
                    OnSelectedChanged(this);
                }
            }
        }

        public event OnRadioButtonGroupSelectedChanged OnSelectedChanged;
    }

    delegate void OnRadioButtonSelect(MyGuiControlRadioButton sender);
    class MyGuiControlRadioButton : MyGuiControlBase
    {        
        public event OnRadioButtonSelect OnSelect;

        bool m_selected;
        int m_key;

        public MyGuiControlRadioButton(IMyGuiControlsParent parent, Vector2 position, Vector2 size, int key, Vector4 color)
            : base(parent, position, size, color, null, MyGuiManager.GetCheckboxOffTexture(), null, null, true)
        {
            m_canHandleKeyboardActiveControl = true;
            m_selected = false;
            m_backgroundColor = color;
            m_key = key;            
        }

        public MyGuiControlRadioButton(IMyGuiControlsParent parent, Vector2 position, int key, Vector4 color)
            : this(parent, position, MyGuiConstants.RADIOBUTTON_SIZE, key, color)
        {
        }

        public int Key 
        {
            get 
            {
                return m_key;
            }
        }

        public bool Selected 
        {
            set 
            {
                bool valueChangedToSelected = m_selected != value && value == true;
                
                m_selected = value;

                if (valueChangedToSelected && OnSelect != null) 
                        OnSelect(this);                                    
            }
            get 
            {
                return m_selected;
            }
        }

        //  Method returns true if input was captured by control, so no other controls, nor screen can use input in this update
        public override bool HandleInput(MyGuiInput input, bool hasKeyboardActiveControl, bool hasKeyboardActiveControlPrevious, bool isThisFirstHandleInput)
        {
            bool ret = base.HandleInput(input, hasKeyboardActiveControl, hasKeyboardActiveControlPrevious, isThisFirstHandleInput);

            if (ret == false)
            {
                if (((IsMouseOver() == true) && (input.IsNewLeftMousePressed() == true)) ||
                    ((hasKeyboardActiveControl == true) && ((input.IsNewKeyPress(Keys.Enter) || (input.IsNewKeyPress(Keys.Space))))))
                {
                    if (!Selected) 
                    {
                        MyAudio.AddCue2D(MySoundCuesEnum.GuiMouseClick);
                        Selected = true;
                        ret = true;
                    }                    
                }
            }

            return ret;
        }

        public override void Draw()
        {
            base.Draw();

            if (m_selected == true)
            {
                Vector4 selectedColor = (IsMouseOverOrKeyboardActive() == false) ? 
                    m_backgroundColor.Value : m_backgroundColor.Value * MyGuiConstants.CONTROL_MOUSE_OVER_BACKGROUND_COLOR_MULTIPLIER;
                // because checkbox texture is not aligned to center, then we must add offset to selected texture position
                Vector2 selectedTextureOffset = new Vector2(m_size.Value.X/40f, 0f);

                //  Selected sprite
                MyGuiManager.DrawSpriteBatch(MyGuiManager.GetBlankTexture(), m_parent.GetPositionAbsolute() + m_position + selectedTextureOffset, m_size.Value * new Vector2(0.5f, 0.5f),
                    GetColorAfterTransitionAlpha(selectedColor), MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER);
            }
        }
    }
}
