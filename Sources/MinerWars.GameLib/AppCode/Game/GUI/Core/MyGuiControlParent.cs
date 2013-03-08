using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Toolkit.Input;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.Utils;

namespace MinerWars.AppCode.Game.GUI.Core
{
    class MyGuiControlParent : MyGuiControlBase, IMyGuiControlsParent
    {
        private MyGuiControls m_controls;

        public MyGuiControlParent(IMyGuiControlsParent parent, Vector2 position, Vector2 size, Vector4? backgroundColor, StringBuilder toolTip, MyTexture2D controlTexture)
            : base(parent, position, size, backgroundColor, toolTip, controlTexture, null, null, true)
        {
            m_controls = new MyGuiControls();
        }

        public MyGuiControls Controls
        {
            get { return m_controls; }
        }

        public Vector2 GetPositionAbsolute()
        {
            return m_parent.GetPositionAbsolute() + m_position;
        }

        public float GetTransitionAlpha()
        {
            return m_parent.GetTransitionAlpha();
        }

        #region Overriden methods
        public override void ShowToolTip()
        {
            foreach (MyGuiControlBase control in Controls.GetVisibleControls())
            {
                if (control.IsMouseOver()) return;
            }
            base.ShowToolTip();
        }

        public override void Draw()
        {
            base.Draw();
            foreach (MyGuiControlBase control in Controls.GetList())
            {
                if (control is MyGuiControlListboxDragAndDrop)
                {
                    MyGuiControlListboxDragAndDrop tempDragAndDrop = (MyGuiControlListboxDragAndDrop)control;
                    if (tempDragAndDrop.IsActive())
                    {
                        continue;
                    }
                }
                if (control is MyGuiControlCombobox)
                {
                    MyGuiControlCombobox tempCombobox = (MyGuiControlCombobox)control;
                    if (tempCombobox.IsHandlingInputNow() == true)
                    {
                        continue;
                    }
                }
                if(control.Visible)
                    control.Draw();
            }
        }

        public override bool HandleInput(MyGuiInput input, bool hasKeyboardActiveControl, bool hasKeyboardActiveControlPrevious, bool receivedFocusInThisUpdate)
        {
            bool captured = base.HandleInput(input, hasKeyboardActiveControl, hasKeyboardActiveControlPrevious, receivedFocusInThisUpdate);
            return captured;
        }

        public override void Update()
        {
            base.Update();
        }

        public override void HideToolTip()
        {
            base.HideToolTip();
        }

        public override bool Visible
        {
            get
            {
                return base.Visible;
            }
            set
            {
                if (Controls != null)
                {
                    foreach (MyGuiControlBase control in Controls.GetList())
                    {
                        control.Visible = value;
                    }
                }
                base.Visible = value;
            }
        }
        #endregion        
    }
}
