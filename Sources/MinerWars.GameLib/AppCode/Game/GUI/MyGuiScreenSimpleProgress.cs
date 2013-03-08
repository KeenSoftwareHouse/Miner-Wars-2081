using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Localization;
using MinerWarsMath;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Audio;

namespace MinerWars.AppCode.Game.GUI
{
    class MyGuiScreenSimpleProgress : MyGuiScreenBase
    {
        private StringBuilder m_text;
        private MyGuiControlRotatingWheel m_progressWheel;
        private MyGuiControlLabel m_progressText;        

        public MyGuiScreenSimpleProgress(MyTextsWrapperEnum caption, MyTextsWrapperEnum text)
            : this(caption, MyTextsWrapper.Get(text))
        {
        }

        public MyGuiScreenSimpleProgress(MyTextsWrapperEnum caption, StringBuilder text) 
            : base(new Vector2(0.5f, 0.5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, new Vector2(0.4f, 0.4f))
        {
            m_text = text;
            m_closeOnEsc = false;
            m_isTopMostScreen = true;

            AddCaption(caption, new Vector2(0f, 0.021f));

            m_progressWheel = new MyGuiControlRotatingWheel(this, Vector2.Zero, MyGuiConstants.DEFAULT_CONTROL_FOREGROUND_COLOR.ToVector4(), 0.5f, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, MyGuiManager.GetLoadingTexture());
            Controls.Add(m_progressWheel);

            m_progressText = new MyGuiControlLabel(this, new Vector2(0f, 0.12f), null, m_text, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER);
            Controls.Add(m_progressText);
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenSimpleProgress";
        }
    }
}
