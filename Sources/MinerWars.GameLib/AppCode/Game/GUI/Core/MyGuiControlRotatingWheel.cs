using MinerWarsMath;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Managers;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.Utils;

namespace MinerWars.AppCode.Game.GUI.Core
{
    class MyGuiControlRotatingWheel : MyGuiControlBase
    {
        float m_rotatingAngle;
        Vector4 m_color;
        float m_wheelScale;
        MyGuiDrawAlignEnum m_align;
        private MyTexture2D m_texture;

        public MyGuiControlRotatingWheel(IMyGuiControlsParent parent, Vector2 position, Vector4 color, float scale, MyGuiDrawAlignEnum align, MyTexture2D texture)
            : base(parent, position, null, null, null, false)
        {
            m_rotatingAngle = MyMwcUtils.GetRandomRadian();
            m_color = color;
            m_wheelScale = scale;
            //m_scale = 4;
            m_align = align;
            m_texture = texture;
        }

        public override bool HandleInput(MyGuiInput input, bool hasKeyboardActiveControl, bool hasKeyboardActiveControlPrevious, bool receivedFocusInThisUpdate)
        {
            return base.HandleInput(input, false, false, false);
        }
        
        public override void Draw()
        {
            base.Draw();

            //1.5 radian per second - not using MinerWars.GameTime, because its nort updated sometimes
            m_rotatingAngle = (System.Environment.TickCount / 1000f) * 1.5f;


            Vector2 rotatingSize = MyGuiManager.GetNormalizedSize(m_texture, m_wheelScale);
            Vector2 rotatingPosition = m_parent.GetPositionAbsolute() + m_position + new Vector2(rotatingSize.X / 2.0f, rotatingSize.Y / 2.0f);
            
            //  Large wheel - shadow only
            MyGuiManager.DrawSpriteBatch(m_texture, rotatingPosition + MyGuiConstants.SHADOW_OFFSET, m_wheelScale,
                new Color(m_parent.GetTransitionAlpha() * (new Color(0, 0, 0, 80)).ToVector4()),
                MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, m_rotatingAngle, new Vector2(0.5f, 0.5f));

            //  Large wheel - wheel
            MyGuiManager.DrawSpriteBatch(m_texture, rotatingPosition, m_wheelScale, new Color(m_color * m_parent.GetTransitionAlpha()),
                MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, m_rotatingAngle, new Vector2(0.5f, 0.5f));

            //  Small wheel - without shadow
            const float SMALL_SCALE = 0.6f;
            Vector2 smallRotatingPosition = rotatingPosition - (rotatingSize * ((1 - SMALL_SCALE) / 2.0f));
            MyGuiManager.DrawSpriteBatch(m_texture, smallRotatingPosition,
                SMALL_SCALE * m_wheelScale, new Color(m_color * m_parent.GetTransitionAlpha()),
                MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, -m_rotatingAngle * 1.1f, new Vector2(0.5f, 0.5f));

            //  Mini wheel - without shadow
            const float MINI_SCALE = SMALL_SCALE * 0.6f;
            MyGuiManager.DrawSpriteBatch(m_texture, rotatingPosition - (rotatingSize * ((1 - MINI_SCALE) / 2.0f)),
                MINI_SCALE * m_wheelScale, new Color(m_color * m_parent.GetTransitionAlpha()),
                MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, m_rotatingAngle * 1.2f, new Vector2(0.5f, 0.5f));
        }

     
    }
}
