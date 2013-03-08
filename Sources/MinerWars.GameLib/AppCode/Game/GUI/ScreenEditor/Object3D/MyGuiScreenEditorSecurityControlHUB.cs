using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Prefabs;
using MinerWarsMath;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.GUI.Core;
using SysUtils.Utils;
using MinerWars.AppCode.Game.Entities.Prefabs;
using MinerWars.AppCode.Game.Entities;
using System.Diagnostics;

namespace MinerWars.AppCode.Game.GUI.ScreenEditor.Object3D
{
    class MyGuiScreenEditorSecurityControlHUB : MyGuiScreenEditorObject3DBase
    {
        private MyGuiControlListbox m_connectedPrefabIdsListbox;

        public MyGuiScreenEditorSecurityControlHUB(MyPrefabSecurityControlHUB prefabSecurityControlHUB)
            : base(prefabSecurityControlHUB, new Vector2(0.5f, 0.5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, null, MyTextsWrapperEnum.SecurityControlHUB)
        {
            Init();
        }

        private MyPrefabSecurityControlHUB PrefabSecurityControlHUB 
        {
            get { return (MyPrefabSecurityControlHUB)m_entity; }
            set { m_entity = value; }
        }

        private void Init() 
        {
            m_windowIsMovable = false;
            m_enableBackgroundFade = true;
            m_size = new Vector2(0.45f, 0.875f);
            AddCaption(new Vector2(0, -0.005f));

            AddActivatedCheckbox(GetControlsOriginLeftFromScreenSize() + new Vector2(0.05f, 0.01f), PrefabSecurityControlHUB.Activated);
            m_activatedCheckbox.OnCheck = OnActivateClick;

            m_connectedPrefabIdsListbox = new MyGuiControlListbox(this, new Vector2(0, -0.02f), new Vector2(0.2f, 0.07f), MyGuiConstants.LISTBOX_BACKGROUND_COLOR,
                MyTextsWrapper.Get(MyTextsWrapperEnum.ConnectedPrefabIds), MyGuiConstants.LABEL_TEXT_SCALE, 1, 7, 1, false, true, false);
            foreach (IMyUseableEntity connectedEntity in PrefabSecurityControlHUB.ConnectedEntities) 
            {
                MyEntity entity = connectedEntity as MyEntity;
                Debug.Assert(entity.EntityId != null);
                uint entityId = entity.EntityId.Value.NumericValue;
                m_connectedPrefabIdsListbox.AddItem((int)entityId, new StringBuilder(entityId.ToString()));
            }

            Controls.Add(m_connectedPrefabIdsListbox);

            Vector2 buttonPosition = m_connectedPrefabIdsListbox.GetPosition() + 
                new Vector2(-m_connectedPrefabIdsListbox.GetSize().Value.X / 2f, m_connectedPrefabIdsListbox.GetSize().Value.Y / 2f) + 
                new Vector2(-0.05f, 0.09f);
            Controls.Add(new MyGuiControlButton(this, buttonPosition, MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.Add, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnAddClick,
                MyGuiControlButtonTextAlignment.CENTERED, true, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER, true));
            buttonPosition += new Vector2(0.18f, 0f);
            Controls.Add(new MyGuiControlButton(this, buttonPosition, MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.Delete, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnDeleteClick,
                MyGuiControlButtonTextAlignment.CENTERED, true, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER, true));            
        }

        private void OnActivateClick(MyGuiControlCheckbox sender) 
        {
            PrefabSecurityControlHUB.Activate(sender.Checked, false);
        }

        private void OnDeleteClick(MyGuiControlButton sender) 
        {
            int? selectedPrefabId = m_connectedPrefabIdsListbox.GetSelectedItemKey();
            if (selectedPrefabId != null) 
            {
                PrefabSecurityControlHUB.DisconnetEntity((uint)selectedPrefabId.Value);
                m_connectedPrefabIdsListbox.RemoveItem(selectedPrefabId.Value);
            }
        }

        private void OnAddClick(MyGuiControlButton sender) 
        {
            MyGuiScreenEditorConnectPrefab connectPrefabScreen = new MyGuiScreenEditorConnectPrefab();
            connectPrefabScreen.OnSubmit += ConnectPrefabScreen_OnSubmit;
            MyGuiManager.AddScreen(connectPrefabScreen);
        }

        private void ConnectPrefabScreen_OnSubmit(object sender, EventArgs e)
        {
            MyGuiScreenEditorConnectPrefab connectPrefabScreen = (MyGuiScreenEditorConnectPrefab)sender;

            uint prefabId = connectPrefabScreen.PrefabId;
            MyConnectEntityOperation result = PrefabSecurityControlHUB.ConnectEntity(prefabId);
            switch (result)
            {
                case MyConnectEntityOperation.AlreadyConnected:
                    DisplayErrorMessage(MyTextsWrapperEnum.EntityIsAlreadyConnected);
                    break;
                case MyConnectEntityOperation.NotExists:
                    DisplayErrorMessage(MyTextsWrapperEnum.EntityIsNotExist);
                    break;
                case MyConnectEntityOperation.NotSupported:
                    DisplayErrorMessage(MyTextsWrapperEnum.EntityIsNotSupported);
                    break;
                case MyConnectEntityOperation.Success:
                    m_connectedPrefabIdsListbox.AddItem((int)prefabId, new StringBuilder(prefabId.ToString()));
                    connectPrefabScreen.CloseScreen();
                    break;
                default:
                    throw new MyMwcExceptionApplicationShouldNotGetHere();
            }
        }        

        private void DisplayErrorMessage(MyTextsWrapperEnum errorText) 
        {
            MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, errorText, MyTextsWrapperEnum.EmptyDescription, MyTextsWrapperEnum.Ok, null));
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenEditorSecurityControlHUB";
        }
    }
    
    class MyGuiScreenEditorConnectPrefab : MyGuiScreenBase 
    {
        private MyGuiControlTextbox m_prefabIdTextbox;

        public MyGuiScreenEditorConnectPrefab()
            : base(new Vector2(0.5f, 0.5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, new Vector2(0.3f, 0.2f)) 
        {
            Vector2 leftTop = -m_size.Value / 2f + new Vector2(0.025f, 0.025f);

            m_size = new Vector2(0.3f, 0.25f);

            MyGuiControlLabel label = new MyGuiControlLabel(this, leftTop, null, MyTextsWrapperEnum.ConnectEntityWithId, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP);
            Controls.Add(label);
            leftTop.Y += label.GetSize().Value.Y + 0.025f;

            m_prefabIdTextbox = new MyGuiControlTextbox(this, leftTop + MyGuiConstants.TEXTBOX_SMALL_SIZE / 2f, MyGuiControlPreDefinedSize.SMALL, string.Empty, 10, MyGuiConstants.LABEL_TEXT_COLOR,
                MyGuiConstants.LABEL_TEXT_SCALE * 0.75f, MyGuiControlTextboxType.DIGITS_ONLY);
            Controls.Add(m_prefabIdTextbox);

            Controls.Add(new MyGuiControlButton(this, new Vector2(0f, 0.05f), MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.Ok, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnOkClick,
                MyGuiControlButtonTextAlignment.CENTERED, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));

            OnEnterCallback += OnOkClickDelegate;
        }

        public event EventHandler OnSubmit;

        private void OnOkClickDelegate()
        {
            OnOkClick(null);
        }

        private void OnOkClick(MyGuiControlButton sender) 
        {
            if (string.IsNullOrEmpty(m_prefabIdTextbox.Text)) 
            {
                CloseScreenNow();
                return;
            }

            if (OnSubmit != null) 
            {
                OnSubmit(this, EventArgs.Empty);
            }
        }

        public uint PrefabId 
        {
            get 
            {
                return uint.Parse(m_prefabIdTextbox.Text);
            }
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenEditorConnectPrefab";
        }
    }
}
