using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Managers;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.Utils;
using SysUtils.Utils;

//  Label is defined by string builder or by text enum. Only one of them at a time. It's good to use enum whenever 
//  possible, as it easily supports changing languages. Use string builder only if the text isn't and can't be defined 
//  in text resources.
//
//  If enum version is used, then text won't be stored in string builder until you use UpdateParams


namespace MinerWars.AppCode.Game.GUI.Core
{
    class MyGuiControlColor : MyGuiControlBase
    {
        public delegate void OnColorChangeCallback(MyGuiControlColor sender);
        public OnColorChangeCallback OnChange = null;

        Color m_color;
        MyGuiControlSlider m_RSlider;
        MyGuiControlSlider m_GSlider;
        MyGuiControlSlider m_BSlider;
        MyGuiControlLabel m_RLabel;
        MyGuiControlLabel m_GLabel;
        MyGuiControlLabel m_BLabel;

        bool m_canChangeColor = true;

        public MyGuiControlColor(IMyGuiControlsParent parent, StringBuilder text, Vector2 position, Vector2? size, float scale, Color color)
            : base(parent, position, size, null, null, false)
        {

            m_color = color;


            float localScale = 0.5f;

            MyGuiControlLabel label = new MyGuiControlLabel(parent, position, null, text, Color.Yellow.ToVector4(), MyGuiConstants.LABEL_TEXT_SCALE * 0.8f * m_scale,
                                            MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);

            parent.Controls.Add(label);
            position.Y += 0.04f * m_scale;

            m_RSlider = new MyGuiControlSlider(parent, position + new Vector2(0.04f, 0) * m_scale, 0.2f, 0, 255, new Vector4(1, 1, 0, 1),
                new StringBuilder(), 0.1f, 3, 0.65f * m_scale * localScale, m_scale * localScale);
            parent.Controls.Add(m_RSlider);
            m_RSlider.SetValue(m_color.R);
            m_RSlider.OnChange += delegate(MyGuiControlSlider sender)
            {
                if (m_canChangeColor)
                {
                    m_color.R = (byte)sender.GetValue();
                    UpdateTexts();
                    if (OnChange != null)
                        OnChange(this);
                }
            };
            m_RLabel = new MyGuiControlLabel(parent, position, null, new StringBuilder(), Color.Yellow.ToVector4(), MyGuiConstants.LABEL_TEXT_SCALE * 0.8f * m_scale, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP);
            parent.Controls.Add(m_RLabel);

            m_GSlider = new MyGuiControlSlider(parent, position + new Vector2(0.04f + 0.1f, 0) * m_scale, 0.2f, 0, 255, new Vector4(1, 1, 0, 1),
    new StringBuilder(), 0.1f, 3, 0.65f * m_scale * localScale, m_scale * localScale);
            parent.Controls.Add(m_GSlider);
            m_GSlider.SetValue(m_color.G);
            m_GSlider.OnChange += delegate(MyGuiControlSlider sender)
            {
                if (m_canChangeColor)
                {
                    m_color.G = (byte)sender.GetValue();
                    UpdateTexts();
                    if (OnChange != null)
                        OnChange(this);
                }
            };
            m_GLabel = new MyGuiControlLabel(parent, position + new Vector2(0.04f + 0.1f, 0) * m_scale, null, new StringBuilder(), Color.Yellow.ToVector4(), MyGuiConstants.LABEL_TEXT_SCALE * 0.8f * m_scale, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP);
            parent.Controls.Add(m_GLabel);

            m_BSlider = new MyGuiControlSlider(parent, position + new Vector2(0.04f + 2 * 0.1f, 0) * m_scale, 0.2f, 0, 255, new Vector4(1, 1, 0, 1),
new StringBuilder(), 0.1f, 3, 0.65f * m_scale * localScale, m_scale * localScale);
            parent.Controls.Add(m_BSlider);
            m_BSlider.SetValue(m_color.B);
            m_BSlider.OnChange += delegate(MyGuiControlSlider sender)
            {
                if (m_canChangeColor)
                {
                    m_color.B = (byte)sender.GetValue();
                    UpdateTexts();
                    if (OnChange != null)
                        OnChange(this);
                }
            };
            m_BLabel = new MyGuiControlLabel(parent, position + new Vector2(0.04f + 2 * 0.1f, 0) * m_scale, null, new StringBuilder(), Color.Yellow.ToVector4(), MyGuiConstants.LABEL_TEXT_SCALE * 0.8f * m_scale, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP);
            parent.Controls.Add(m_BLabel);


            
        }

        public override void Draw()
        {
            base.Draw();

            MyGuiManager.DrawSpriteBatch(MyGuiManager.GetBlankTexture(), m_parent.GetPositionAbsolute() + m_position + new Vector2(0.19f, 0.005f), new Vector2(0.1f, 0.03f), m_color, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);
        }

        private void UpdateSliders()
        {
            m_canChangeColor = false;
            m_RSlider.SetValue((byte)m_color.R);
            m_GSlider.SetValue((byte)m_color.G);
            m_BSlider.SetValue((byte)m_color.B);

            UpdateTexts();

            m_canChangeColor = true;
        }

        private void UpdateTexts()
        {
            m_RLabel.UpdateText(((byte)m_color.R).ToString());
            m_GLabel.UpdateText(((byte)m_color.G).ToString());
            m_BLabel.UpdateText(((byte)m_color.B).ToString());
        }

        public void SetColor(Vector3 color)
        {
            SetColor(new Color(color));
        }

        public void SetColor(Vector4 color)
        {
            SetColor(new Color(color));
        }

        public void SetColor(Color color)
        {
            m_color = color;
            
            UpdateSliders();
            if (OnChange != null)
                OnChange(this);
        }

        public Color GetColor()
        {
            return m_color;
        }
    }
}
