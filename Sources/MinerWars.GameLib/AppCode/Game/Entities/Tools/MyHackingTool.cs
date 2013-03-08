using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Entities.Prefabs;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.Audio;
using MinerWarsMath;
using MinerWars.AppCode.Game.Missions;
using System.Diagnostics;
using MinerWars.AppCode.Game.HUD;

namespace MinerWars.AppCode.Game.Entities.Tools
{
    enum MyHackingToolStateEnum
    {
        Idle,
        Hacking,
    }

    enum MyHackingResultEnum
    {
        Success,        
        CanNotBeHacked,
        NotEnoughLevel,
        Canceled,
        NoHackingTool,
    }

    class MyHackingEventArgs : EventArgs 
    {
        public MyHackingResultEnum HackingResult { get; private set; }
        public IMyUseableEntity HackingEntity { get; private set; }

        public MyHackingEventArgs(MyHackingResultEnum hackingResult, IMyUseableEntity hackingEntity) 
        {
            HackingResult = hackingResult;
            HackingEntity = hackingEntity;
        }
    }

    delegate void OnMyHackingFinish(object sender, MyHackingEventArgs hackingEventArgs);
    delegate void OnMyHackingStart(object sender, IMyUseableEntity hackingEntity);

    class MyHackingTool
    {
        private delegate void HackingResultAction();        
        private delegate MyGuiFont GetFontDelegate();

        class MyHackingResultMessage
        {
            public MyTextsWrapperEnum? Text { get; set; }
            public GetFontDelegate GetFontDelegate { get; set; }
            public MySoundCuesEnum? SoundCue { get; set; }

            public MyHackingResultMessage(MyTextsWrapperEnum? text, GetFontDelegate getFontDelegate, MySoundCuesEnum? soundCue)
            {
                Text = text;
                GetFontDelegate = getFontDelegate;
                SoundCue = soundCue;
            }

            public void Display(object[] parameters)
            {
                if (Text != null)
                {
                    MyGuiFont font = GetFontDelegate();
                    MyHudNotification.MyNotification notification = new MyHudNotification.MyNotification(Text.Value, font, 5000, null, parameters);
                    MyHudNotification.AddNotification(notification);
                }
                if (SoundCue != null) 
                {
                    MyAudio.AddCue2D(SoundCue.Value);
                }
            }
        }

        private static readonly MyHackingResultMessage[] m_hackingResultMessages;
        static MyHackingTool() 
        {
            m_hackingResultMessages = new MyHackingResultMessage[MyMwcUtils.GetMaxValueFromEnum<MyHackingResultEnum>() + 1];
            m_hackingResultMessages[(int)MyHackingResultEnum.Success] = null;
            m_hackingResultMessages[(int)MyHackingResultEnum.NotEnoughLevel] = new MyHackingResultMessage(MyTextsWrapperEnum.HackingResult_NotEnoughLevel, delegate() { return MyGuiManager.GetFontMinerWarsRed(); }, MySoundCuesEnum.SfxCancelHack);
            m_hackingResultMessages[(int)MyHackingResultEnum.CanNotBeHacked] = new MyHackingResultMessage(MyTextsWrapperEnum.HackingResult_CanNotBeHacked, delegate() { return MyGuiManager.GetFontMinerWarsRed(); }, MySoundCuesEnum.SfxCancelHack);
            m_hackingResultMessages[(int)MyHackingResultEnum.Canceled] = null;
            m_hackingResultMessages[(int)MyHackingResultEnum.NoHackingTool] = new MyHackingResultMessage(MyTextsWrapperEnum.HackingResult_NoHackingTool, delegate() { return MyGuiManager.GetFontMinerWarsRed(); }, MySoundCuesEnum.SfxCancelHack);
        }

        private MyHackingToolStateEnum m_state;
        private IMyUseableEntity m_acutalHackingEntity;
        //private int m_hackingTimeLeft;
        //private int m_hackingTimeTotal;
        private OnMyHackingFinish m_hackingFinishCallback;
        private MyGuiScreenUseProgressBar m_hackingProgressScreen;
        private EventHandler m_hackingProgressScreenSuccess;
        private EventHandler m_hackingProgressScreenCanceled;
        private DieHandler m_ownerDie;
        private Action<MyEntity> m_ownerClose;
        private MySmallShip m_owner;

        public int HackingLevel { get; set; }

        public MyHackingToolStateEnum ActualState { get { return m_state; } }        

        public MyHackingTool(MySmallShip owner, int hackingLevel) 
        {
            m_owner = owner;
            HackingLevel = hackingLevel;
            m_ownerDie = new DieHandler(OnOwnerDie);
            m_ownerClose = OnOwnerClose;
            m_owner.OnClose += m_ownerClose;
            m_hackingProgressScreenSuccess = new EventHandler(m_hackingProgressScreen_OnSuccess);
            m_hackingProgressScreenCanceled = new EventHandler(m_hackingProgressScreen_OnCanceled);
        }        

        public void Update() 
        {
            //if (m_state == MyHackingToolStateEnum.Hacking) 
            //{
            //    m_hackingTimeLeft -= MyConstants.PHYSICS_STEP_SIZE_IN_MILLISECONDS;
            //    float newValue = (float)Math.Round((double)(m_hackingTimeTotal - m_hackingTimeLeft) / (double)m_hackingTimeTotal, 2);
            //    newValue = MathHelper.Clamp(newValue, 0f, 1f);
            //    m_hackingProgressScreen.UpdateValue(newValue);
            //    if (m_hackingTimeLeft <= 0)
            //    {
            //        StopHacking(MyHackingResultEnum.Success, HackingSuccessAction);
            //    }
            //}
        }

        public event OnMyHackingFinish HackingFinish;
        public event OnMyHackingStart HackingStart;

        public void Hack(IMyUseableEntity entityToHack) 
        {
            if (entityToHack == null) 
            {
                throw new ArgumentNullException("entityToHack");
            }
            if (!entityToHack.CanBeHacked(m_owner)) 
            {
                StopHacking(MyHackingResultEnum.CanNotBeHacked);
                return;
            }
            if (HackingLevel < 1) 
            {
                StopHacking(MyHackingResultEnum.NoHackingTool);
                return;
            }
            if (entityToHack.UseProperties.HackingLevel > HackingLevel) 
            {
                StopHacking(MyHackingResultEnum.NotEnoughLevel, null, new object[]{ entityToHack.UseProperties.HackingLevel, HackingLevel });
                return;
            }

            m_state = MyHackingToolStateEnum.Hacking;
            m_acutalHackingEntity = entityToHack;
            //m_hackingTimeTotal = (int)((float)entityToHack.UseProperties.HackingTime * (1f - (HackingLevel - entityToHack.UseProperties.HackingLevel) * 0.25f));
            //m_hackingTimeLeft = m_hackingTimeTotal;
            NotifyHackingStart();
            int hackingTimeTotal = (int)((float)entityToHack.UseProperties.HackingTime * (1f - (HackingLevel - entityToHack.UseProperties.HackingLevel) * 0.25f));
            m_hackingProgressScreen = new MyGuiScreenUseProgressBar(MyTextsWrapperEnum.Hacking, MyTextsWrapperEnum.HackingProgress, 0f, MySoundCuesEnum.SfxProgressHack, MySoundCuesEnum.SfxCancelHack, MyGameControlEnums.USE, 50, hackingTimeTotal, 0);
            m_hackingProgressScreen.OnCanceled += m_hackingProgressScreenCanceled;
            m_hackingProgressScreen.OnSuccess += m_hackingProgressScreenSuccess;
            m_owner.OnDie += m_ownerDie;
            MyGuiManager.AddScreen(m_hackingProgressScreen);
        }

        public IMyUseableEntity GetActualHackingEntity()
        {
            return m_acutalHackingEntity;
        }

        void m_hackingProgressScreen_OnSuccess(object sender, EventArgs e)
        {
            StopHacking(MyHackingResultEnum.Success, HackingSuccessAction);
        }

        void m_hackingProgressScreen_OnCanceled(object sender, EventArgs e)
        {
            StopHacking(MyHackingResultEnum.Canceled);
        }

        void OnOwnerDie(MyEntity entity, MyEntity killer)
        {
            Close();
        }

        void OnOwnerClose(MyEntity entity) 
        {
            Close();
        }

        private void HackingSuccessAction() 
        {
            m_acutalHackingEntity.UseProperties.IsHacked = true;
            m_acutalHackingEntity.UseFromHackingTool(m_owner, HackingLevel - m_acutalHackingEntity.UseProperties.HackingLevel);
        }

        private void StopHacking(MyHackingResultEnum result, HackingResultAction action = null, object[] parameters = null) 
        {                        
            MyHackingResultMessage resultMessage = m_hackingResultMessages[(int)result];
            if (resultMessage != null)
            {
                //MyGuiManager.AddScreen(new MyGuiScreenMessageBox(resultConfig.MessageBoxType, resultConfig.Text, MyTextsWrapperEnum.HackingResult, MyTextsWrapperEnum.Ok, null));
                resultMessage.Display(parameters);
            }

            if (m_hackingProgressScreen != null)
            {
                if (m_hackingProgressScreen.GetState() != MyGuiScreenState.CLOSED)
                {
                    m_hackingProgressScreen.CloseScreenNow();
                }
                m_hackingProgressScreen.OnCanceled -= m_hackingProgressScreenCanceled;
                m_hackingProgressScreen.OnSuccess -= m_hackingProgressScreenSuccess;
                m_hackingProgressScreen = null;
            }
            m_owner.OnDie -= m_ownerDie;
            if (action != null) 
            {
                action();
            }
            NotifyHackingFinish(result);
            m_acutalHackingEntity = null;
            //m_hackingTimeLeft = 0;
            m_state = MyHackingToolStateEnum.Idle;
        }

        private void NotifyHackingFinish(MyHackingResultEnum result) 
        {
            if (HackingFinish != null) 
            {
                HackingFinish(this, new MyHackingEventArgs(result, m_acutalHackingEntity));
            }
            if (result == MyHackingResultEnum.Success) 
            {
                MyScriptWrapper.OnEntityHacked(m_acutalHackingEntity as MyEntity);
            }
        }

        private void NotifyHackingStart() 
        {
            if (HackingStart != null) 
            {
                HackingStart(this, m_acutalHackingEntity);
            }
        }

        public void Close() 
        {
            StopHacking(MyHackingResultEnum.Canceled);
            m_owner.OnDie -= m_ownerDie;
            m_owner.OnClose -= m_ownerClose;
            m_owner = null;
            m_hackingProgressScreenSuccess = null;
            m_hackingProgressScreenCanceled = null;
        }
    }
}
