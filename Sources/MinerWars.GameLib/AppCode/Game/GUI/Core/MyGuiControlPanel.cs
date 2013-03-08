using System.Text;
using MinerWarsMath;
using MinerWarsMath.Graphics;
using MinerWars.AppCode.Game.Textures;

namespace MinerWars.AppCode.Game.GUI.Core
{
    class MyGuiControlPanel : MyGuiControlBase
    {
        private int m_borderSize;
        private Vector4? m_borderColor;

        public MyGuiControlPanel(IMyGuiControlsParent parent, Vector2 position, Vector2? size, Vector4 backgroundColor, int borderSize, Vector4 borderColor)
            : this(parent, position, size, backgroundColor, null, null, null, null, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER)
        {
            m_borderSize = borderSize;
            m_borderColor = borderColor;
        }

        public MyGuiControlPanel(IMyGuiControlsParent parent, Vector2 position, Vector2? size, Vector4 backgroundColor, 
            MyTexture2D texture, MyTexture2D hoverTexture, MyTexture2D pressedTexture, MyTexture2D shadowTexture, 
            MyGuiDrawAlignEnum align)
            : base(parent, position, size, backgroundColor, new StringBuilder(), 
                    texture, hoverTexture, pressedTexture, false)
        {
            Visible = true;            
        }

        public MyGuiControlPanel(IMyGuiControlsParent parent, Vector2 position, Vector2? size, Vector4 backgroundColor, MyTexture2D texture, 
                                 MyTexture2D hoverTexture, MyTexture2D pressedTexture, int borderSize, Vector4 borderColor, StringBuilder toolTip)
            : base(parent, position, size, backgroundColor, toolTip, texture, hoverTexture, pressedTexture, false)
        {
            Visible = true;            
            m_borderSize = borderSize;
            m_borderColor = borderColor;
        }

        public override bool HandleInput(MyGuiInput input, bool hasKeyboardActiveControl, bool hasKeyboardActiveControlPrevious, bool receivedFocusInThisUpdate)
        {
            return Visible && base.HandleInput(input, false, false, receivedFocusInThisUpdate);
        }

        public override void Draw()
        {
            if (Visible)
            {
                base.Draw();
                if(m_borderSize > 0)
                {
                    DrawBorders(m_parent.GetPositionAbsolute() + m_position - new Vector2(m_size.Value.X / 2f, m_size.Value.Y / 2f), m_size.Value, new Color(m_borderColor.Value), m_borderSize);
                }
            }
        }        

        public void SetSize(Vector2 size)
        {
            m_size = size;
        }

        protected override bool CheckMouseOver()
        {
            return base.CheckMouseOver();
        }

        public void SetBackgroundTexture(MyTexture2D texture) 
        {
            m_controlTexture = texture;
        }
    }
}
