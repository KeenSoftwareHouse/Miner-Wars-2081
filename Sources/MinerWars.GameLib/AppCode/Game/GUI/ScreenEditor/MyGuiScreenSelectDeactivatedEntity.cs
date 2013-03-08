using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Editor;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Prefabs;
using MinerWars.AppCode.Game.Utils;

namespace MinerWars.AppCode.Game.GUI.ScreenEditor
{
    class MyGuiScreenSelectDeactivatedEntity : MyGuiScreenEditorDialogBase
    {
        private MyGuiControlListbox m_listbox;

        public MyGuiScreenSelectDeactivatedEntity()
            : base(new Vector2(0.5f), new Vector2(0.9f, 0.85f))
        {
            m_enableBackgroundFade = true;
            m_size = new Vector2(0.8f, 0.9f);

            AddCaption(MyTextsWrapperEnum.SelectDeactivatedEntity, new Vector2(0,0.035f));
            
            AddBackButtonControl(new Vector2(0, -0.02f));

            Init();
        }

        private void Init() 
        {
            m_listbox = new MyGuiControlListbox(this,
                new Vector2(0f, -0.02f),
                MyGuiConstants.LISTBOX_LONGMEDIUM_SIZE,
                MyGuiConstants.DEFAULT_CONTROL_BACKGROUND_COLOR,
                null, .6f, 1, 16, 1, false, true, false,
                null, null, MyGuiManager.GetScrollbarSlider(), MyGuiManager.GetHorizontalScrollbarSlider(), 2, 1, MyGuiConstants.LISTBOX_BACKGROUND_COLOR_BLUE, 0f, 0f, 0f, 0f, 0, 0, -0.01f, -0.01f, -0.02f, 0.02f);
            Controls.Add(m_listbox);

            m_listbox.ItemDoubleClick += new OnListboxItemDoubleClick(m_listbox_ItemDoubleClick);
            FillListbox();
        }

        private void FillListbox() 
        {
            List<MyEntity> deactivatedEntities = new List<MyEntity>();
            foreach (MyEntity entity in MyEntities.GetEntities()) 
            {
                if (!entity.Activated) 
                {
                    deactivatedEntities.Add(entity);                    
                }
                else if (entity is MyPrefabContainer) 
                {
                    foreach (MyPrefabBase deactivatedPrefab in ((MyPrefabContainer)entity).GetDeactivatedPrefabs()) 
                    {
                        deactivatedEntities.Add(deactivatedPrefab);
                    }
                }
            }

            foreach (MyEntity deactivatedEntity in deactivatedEntities) 
            {
                if (deactivatedEntity.EntityId.HasValue) 
                {                    
                    string displayInfo = string.Format("[{0}] ({1}) {2} : {3}", 
                        deactivatedEntity.EntityId.Value.NumericValue, 
                        deactivatedEntity.GetFriendlyName(),
                        string.IsNullOrEmpty(deactivatedEntity.Name) ? "NONE" : deactivatedEntity.Name,
                        string.IsNullOrEmpty(deactivatedEntity.DisplayName) ? "NONE" : deactivatedEntity.DisplayName);
                    m_listbox.AddItem((int)deactivatedEntity.EntityId.Value.NumericValue, new StringBuilder(displayInfo));
                }
            }
        }

        private void m_listbox_ItemDoubleClick(object sender, MyGuiControlListboxItemEventArgs eventArgs)
        {
            MyEditorGizmo.ClearSelection();
            MyEntity entityToSelect = MyEntities.GetEntityById(new MyEntityIdentifier((uint)eventArgs.Key));
            MyEditorGizmo.AddEntityToSelection(entityToSelect);
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenSelectDeactivatedEntity";
        }
    }
}
