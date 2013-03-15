using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.GUI.Core.TreeView;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.Utils;

namespace MinerWars.AppCode.Game.GUI.Core
{
    class MyGuiControlList : MyGuiControlParent
    {
        private MyScrollbar m_verticalScrollbar;
        private Vector2 m_realSize;
        private Vector2 m_controlsPadding = new Vector2(0.025f, 0.025f);

        public MyGuiControlList(IMyGuiControlsParent parent, Vector2 position, Vector2 size, Vector4 backgroundColor, StringBuilder toolTip, MyTexture2D controlTexture) 
            : base(parent, position, size, backgroundColor, toolTip, controlTexture)
        {
            m_realSize = size;

            m_verticalScrollbar = new MyVScrollbar(this);
            m_verticalScrollbar.TopBorder = m_verticalScrollbar.RightBorder = m_verticalScrollbar.BottomBorder = false;
            m_verticalScrollbar.LeftBorder = false;
            m_verticalScrollbar.OnScrollValueChanged += OnScrollValueChanged;

            RecalculateScrollbar();

            Controls.CollectionChanged += ControlsCollectionChanged;            
        }

        public MyGuiControlList(IMyGuiControlsParent parent, Vector2 position, Vector2 size, Vector4 backgroundColor, StringBuilder toolTip, MyTexture2D controlTexture, Vector2 padding)
            :this(parent,position,size,backgroundColor,toolTip,controlTexture)
        {
            m_controlsPadding = padding;
        }

        private void ControlsCollectionChanged(object sender, EventArgs e)
        {
            Recalculate();
        }        

        public void InitControls(IEnumerable<MyGuiControlBase> controls)
        {
            Controls.CollectionChanged -= ControlsCollectionChanged;
            Controls.Clear();
            foreach (MyGuiControlBase control in controls)
            {
                Controls.Add(control);
            }
            Controls.CollectionChanged += ControlsCollectionChanged;
            Recalculate();
        }

        public override void Draw()
        {
            //  End our standard sprite batch
            MyGuiManager.EndSpriteBatch();
            //  Draw the rectangle(basically the opened area) to stencil buffer to be used for clipping partial item
            if (m_controlsPadding.Y > 0)
                MyGuiManager.DrawStencilMaskRectangle(new MyRectangle2D(GetPositionAbsolute(), m_size.Value), MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER);
            else 
                MyGuiManager.DrawStencilMaskRectangle(new MyRectangle2D(GetPositionAbsolute(), m_size.Value - 2*m_controlsPadding), MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER);  
            
            //  Set up the stencil operation and parameters
            MyGuiManager.BeginSpriteBatch_StencilMask();
            
            base.Draw();            
            //  End stencil-mask batch, and restart the standard sprite batch
            MyGuiManager.EndSpriteBatch_StencilMask();
            //MyGuiManager.EndSpriteBatch();
            MyGuiManager.BeginSpriteBatch();

            m_verticalScrollbar.Draw();
        }

        public override bool HandleInput(MyGuiInput input, bool hasKeyboardActiveControl, bool hasKeyboardActiveControlPrevious, bool receivedFocusInThisUpdate)
        {
            bool baseResult = base.HandleInput(input, hasKeyboardActiveControl, hasKeyboardActiveControlPrevious, receivedFocusInThisUpdate);

            bool captured = false;
            var deltaWheel = input.DeltaMouseScrollWheelValue();
            if (IsMouseOver() && deltaWheel != 0)
            {
                m_verticalScrollbar.ChangeValue(-0.0005f * deltaWheel);
                captured = true;
            }

            bool capturedScrollbar = m_verticalScrollbar.HandleInput(input, hasKeyboardActiveControl, hasKeyboardActiveControlPrevious, receivedFocusInThisUpdate);

            return baseResult || capturedScrollbar || captured;
        }

        public override void SetPosition(Vector2 position)
        {
            base.SetPosition(position);
            RecalculateScrollbar();
            CalculateNewPositionsForControls(m_verticalScrollbar.GetValue());
        }

        private void Recalculate()
        {
            Vector2 oldRealSize = m_realSize;
            CalculateRealSize();
            if (oldRealSize != m_realSize)
            {
                RecalculateScrollbar();
            }
            CalculateNewPositionsForControls(m_verticalScrollbar.GetValue());
        }

        private void RecalculateScrollbar()
        {
            bool vScrollbarVisible = m_size.Value.Y < m_realSize.Y;
            
           
            m_verticalScrollbar.Visible = vScrollbarVisible;
            m_verticalScrollbar.Init(m_realSize.Y, m_size.Value.Y);
            m_verticalScrollbar.Layout(m_position + m_parent.GetPositionAbsolute() - m_size.Value / 2, m_size.Value, MyGuiConstants.COMBOBOX_VSCROLLBAR_SIZE, false);
        }

        private void OnScrollValueChanged(object sender, EventArgs args)
        {
            CalculateNewPositionsForControls(m_verticalScrollbar.GetValue());
        }

        private void CalculateNewPositionsForControls(float offset)
        {
            Vector2 positionTopLeft = new Vector2(-m_size.Value.X / 2f, -m_size.Value.Y / 2f) - new Vector2(0f, offset) + m_controlsPadding;

            foreach (MyGuiControlBase control in Controls.GetVisibleControls())
            {
                Debug.Assert(control.GetSize() != null);
                Vector2 controlSize = control.GetSize().Value;
                control.SetPosition(positionTopLeft + controlSize / 2f);
                positionTopLeft.Y += controlSize.Y + m_controlsPadding.Y;
            }
        }

        private void CalculateRealSize()
        {
            Vector2 newSize = Vector2.Zero;
            newSize.Y += m_controlsPadding.Y;
            foreach (MyGuiControlBase control in Controls.GetVisibleControls())
            {
                Debug.Assert(control.GetSize() != null);
                Vector2 controlSize = control.GetSize().Value;
                newSize.Y += controlSize.Y + m_controlsPadding.Y;
                newSize.X = Math.Max(newSize.X, controlSize.X);                
            }

            m_realSize.X = Math.Max(m_size.Value.X, newSize.X);
            m_realSize.Y = Math.Max(m_size.Value.Y, newSize.Y);
        }
    }
}
