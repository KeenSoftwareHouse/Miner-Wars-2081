using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Utils;
using MinerWarsMath;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Audio;

namespace MinerWars.AppCode.Game.GUI
{
    class MyGuiScreenSimpleProgressBar : MyGuiScreenBase
    {
        protected const float SUCCESS_VALUE = 1f;
        protected const float START_VALUE = 0f;

        private StringBuilder m_text;
        protected MyGuiControlProgressBar m_progressBar;
        private MyGuiControlLabel m_progressText;

        private MySoundCuesEnum? m_progressCueEnum;
        private MySoundCuesEnum? m_cancelCueEnum;
        private MySoundCuesEnum? m_successCueEnum;

        private MySoundCue? m_progressCue;
        private MySoundCue? m_cancelCue;
        private MySoundCue? m_successCue;

        private int m_decimals;

        public MyGuiScreenSimpleProgressBar(MyTextsWrapperEnum caption, MyTextsWrapperEnum text, float value, MySoundCuesEnum? progressCueEnum, MySoundCuesEnum? cancelCueEnum, int decimals, MySoundCuesEnum? successCueEnum = null)
            : this(caption, MyTextsWrapper.Get(text), value, progressCueEnum, cancelCueEnum, decimals, successCueEnum)
        {
        }

        public MyGuiScreenSimpleProgressBar(MyTextsWrapperEnum caption, StringBuilder text, float value, MySoundCuesEnum? progressCueEnum, MySoundCuesEnum? cancelCueEnum, int decimals, MySoundCuesEnum? successCueEnum = null) 
            : base(new Vector2(0.5f, 0.5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, new Vector2(0.4f, 0.4f))
        {
            m_decimals = decimals;
            m_text = text;
            m_closeOnEsc = false;
            m_isTopMostScreen = true;            
            m_backgroundTexture = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\BackgroundScreen\\ProgressBarBackground", flags: TextureFlags.IgnoreQuality);
            m_size = new Vector2(673/1600f, 363/1200f);// MyGuiManager.GetNormalizedCoordsAndPreserveOriginalSize(673, 363);

            m_progressCueEnum = progressCueEnum;
            m_cancelCueEnum = cancelCueEnum;
            m_successCueEnum = successCueEnum;

            AddCaption(caption, new Vector2(0f, 0.015f));
            Vector2 progressBarSize = new Vector2(559 / 1600f, 112 / 1200f);// MyGuiManager.GetNormalizedCoordsAndPreserveOriginalSize(559, 112);            
            m_progressBar = new MyGuiControlProgressBar(this, Vector2.Zero, progressBarSize, Vector4.Zero, new Vector4(1f, 1f, 1f, 1f), Color.White.ToVector4(), MyGuiConstants.LABEL_TEXT_SCALE, value, m_decimals);
            Controls.Add(m_progressBar);

            m_progressText = new MyGuiControlLabel(this, new Vector2(0f, 0.05f), null, m_text, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER);
            Controls.Add(m_progressText);
        }

        public void UpdateValue(float value) 
        {
            m_progressBar.Value = MathHelper.Clamp(value, START_VALUE, SUCCESS_VALUE);
        }

        public float GetValue() 
        {
            return m_progressBar.Value;
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenSimpleProgressBar";
        }

        public override void CloseScreenNow()
        {
            if (m_progressCue != null && m_progressCue.Value.IsPlaying)
            {
                m_progressCue.Value.Stop(SharpDX.XACT3.StopFlags.Immediate);
            }
            if (GetValue() < SUCCESS_VALUE)
            {
                if (m_cancelCueEnum != null && (m_cancelCue == null || !m_cancelCue.Value.IsPlaying))
                {
                    m_cancelCue = MyAudio.AddCue2D(m_cancelCueEnum.Value);
                }
            }
            else
            {
                if (m_successCueEnum != null && (m_successCue == null || !m_successCue.Value.IsPlaying))
                {
                    m_successCue = MyAudio.AddCue2D(m_successCueEnum.Value);
                }
            }
            base.CloseScreenNow();            
        }

        public override bool Update(bool hasFocus)
        {
            if (m_progressCueEnum != null && (m_progressCue == null || !m_progressCue.Value.IsPlaying) && m_state == MyGuiScreenState.OPENED)
            {
                m_progressCue = MyAudio.AddCue2D(m_progressCueEnum.Value);
            }
            return base.Update(hasFocus);
        }
    }
}
