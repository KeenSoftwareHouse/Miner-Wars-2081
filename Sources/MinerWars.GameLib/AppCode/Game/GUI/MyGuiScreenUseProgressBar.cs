using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Audio;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.GUI.Core;

namespace MinerWars.AppCode.Game.GUI
{
    class MyGuiScreenUseProgressBar : MyGuiScreenSimpleProgressBar
    {
        private MyGameControlEnums m_gameControl;  // practically always MyGameControlEnums.USE
        private int m_closeTime;
        //private int m_startedTime;
        private int m_successTime;
        private int m_time;        

        public event EventHandler OnCanceled;
        public event EventHandler OnSuccess;

        public MyGuiScreenUseProgressBar(MyTextsWrapperEnum caption, StringBuilder text, float value, MySoundCuesEnum? progressCueEnum, MySoundCuesEnum? cancelCueEnum, MyGameControlEnums gameControl, int closeTime, int successTime, int decimals, MySoundCuesEnum? successCueEnum = null)
            : base(caption, text, value, progressCueEnum, cancelCueEnum, decimals, successCueEnum)
        {
            DrawMouseCursor = false;

            m_gameControl = gameControl;
            m_closeTime = closeTime;
            m_successTime = successTime;            
            //m_startedTime = MyMinerGame.TotalGamePlayTimeInMilliseconds;
        }        

        public void Reset() 
        {
            m_time = 0;
            m_progressBar.Value =0;
        }

        public MyGuiScreenUseProgressBar(MyTextsWrapperEnum caption, MyTextsWrapperEnum text, float value, MySoundCuesEnum? progressCueEnum, MySoundCuesEnum? cancelCueEnum, MyGameControlEnums gameControl, int closeTime, int successTime, int decimals, MySoundCuesEnum? successCueEnum = null)
            : this(caption, MyTextsWrapper.Get(text), value, progressCueEnum, cancelCueEnum, gameControl, closeTime, successTime, decimals, successCueEnum) 
        {            
        }        

        public override void HandleInput(MyGuiInput input, bool receivedFocusInThisUpdate)
        {
            base.HandleInput(input, receivedFocusInThisUpdate);
            if (!input.IsGameControlPressed(m_gameControl) && m_time >= m_closeTime)
            {
                if (OnCanceled != null)
                {
                    OnCanceled(this, EventArgs.Empty);
                }
                CloseScreenNow();
            }            
        }

        public override bool Update(bool hasFocus)
        {
            if (!base.Update(hasFocus)) return false;

            m_time += MyConstants.PHYSICS_STEP_SIZE_IN_MILLISECONDS;

            if (GetValue() >= SUCCESS_VALUE)
            {
                if (OnSuccess != null) 
                {
                    OnSuccess(this, EventArgs.Empty);
                }
                CloseScreenNow();
            }
            else 
            {
                float newValue = (float)Math.Round((double)m_time / (double)m_successTime, 2);
                UpdateValue(newValue);
            }

            return true;
        }
    }
}
