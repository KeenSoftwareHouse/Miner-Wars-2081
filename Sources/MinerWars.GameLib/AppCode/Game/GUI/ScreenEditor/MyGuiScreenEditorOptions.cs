
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Editor;
using MinerWarsMath;
using System.Text;
using MinerWars.AppCode.Game.Entities.WayPoints;

namespace MinerWars.AppCode.Game.GUI.ScreenEditor
{
    class MyGuiScreenEditorOptions : MyGuiScreenBase
    {
        class MyGuiScreenOptionsEditorSettings
        {
            public bool DisplayUnselectedBounding;
            public bool DisplayPrefabContainerBounding;
            public bool DisplayPrefabContainerAxis;
            public bool UseCameraCrosshair;
            public bool LockedPrefab90DegreesRotation;
            public bool EnableLightsInEditor;
            public bool EnableSnapPointFilter;
            public bool FixedSizeSnapPoints;
            public bool SavePlayerShip;
            public bool EnableObjectPivot;
            public bool DisplayVoxelBounding;
            public bool EnableTextsDrawing;
        }

        readonly MyGuiScreenOptionsEditorSettings m_settingsOld = new MyGuiScreenOptionsEditorSettings();
        readonly MyGuiScreenOptionsEditorSettings m_settingsNew = new MyGuiScreenOptionsEditorSettings();

        //MyGuiControlCheckbox m_displayUnselectedCheckbox;
        MyGuiControlCheckbox m_displayPrefabContainerBounding;
        MyGuiControlCheckbox m_displayPrefabContainerAxis;
        MyGuiControlCheckbox m_useCameraCrosshairCheckbox;
        MyGuiControlCheckbox m_enableLightsInEditorCheckbox;
        MyGuiControlCheckbox m_enableSnapPointFilter;
        MyGuiControlCheckbox m_fixedSnapPointSize;
        MyGuiControlCheckbox m_savePlayerShip;
        MyGuiControlCheckbox m_enableObjectPivot;
        MyGuiControlCheckbox m_displayVoxelBounding;
        MyGuiControlCheckbox m_enableTextsDrawing;        
        

        bool m_displaySavePlayerShipOption;

        public MyGuiScreenEditorOptions()
            : base(new Vector2(0.5f, 0.5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, null)
        {
            m_enableBackgroundFade = true;
            m_size = new Vector2(0.49f, 0.75f);
            m_displaySavePlayerShipOption = MyGuiScreenGamePlay.Static.GetGameType() == MyGuiScreenGamePlayType.EDITOR_STORY;

            AddCaption(MyTextsWrapperEnum.EditorOptions, new Vector2(0, 0.02f));

            Vector2 controlsOriginLeft = new Vector2(-m_size.Value.X / 2.0f + 0.05f, -m_size.Value.Y / 2.0f + 0.125f);
            Vector2 controlsOriginRight = new Vector2(-m_size.Value.X / 2.0f + 0.181f, -m_size.Value.Y / 2.0f + 0.125f);
            Vector2 controlsDelta = new Vector2(0, 0.0525f);

            int dPos = 0;

            //  enable/disable all bounding boxes displaying

            MyGuiControlLabel label1 = new MyGuiControlLabel(this, controlsOriginLeft + dPos * controlsDelta, null, MyTextsWrapperEnum.DisplayUnselectedBounding, MyGuiConstants.LABEL_TEXT_COLOR,
                MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);
            //Controls.Add(label1);

            float labelWidth = label1.GetTextSize().Size.X + 0.05f;

            //m_displayUnselectedCheckbox = new MyGuiControlCheckbox(this, controlsOriginLeft + dPos * controlsDelta + new Vector2(labelWidth, 0), false, MyGuiConstants.CHECKBOX_BACKGROUND_COLOR);
            //Controls.Add(m_displayUnselectedCheckbox);

            // display prefab container bounding
            MyGuiControlLabel label2 = new MyGuiControlLabel(this, controlsOriginLeft + dPos * controlsDelta, null, MyTextsWrapperEnum.DisplayPrefabContainerBounding, MyGuiConstants.LABEL_TEXT_COLOR,
                MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);
            Controls.Add(label2);

            m_displayPrefabContainerBounding = new MyGuiControlCheckbox(this, controlsOriginLeft + dPos * controlsDelta + new Vector2(labelWidth, 0), false, MyGuiConstants.CHECKBOX_BACKGROUND_COLOR);
            Controls.Add(m_displayPrefabContainerBounding);
            dPos++;

            // display prefab container axis
            MyGuiControlLabel labelPrefabContainerAxis = new MyGuiControlLabel(this, controlsOriginLeft + dPos * controlsDelta, null, MyTextsWrapperEnum.DisplayPrefabContainerAxis, MyGuiConstants.LABEL_TEXT_COLOR,
                MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);
            Controls.Add(labelPrefabContainerAxis);

            m_displayPrefabContainerAxis = new MyGuiControlCheckbox(this, controlsOriginLeft + dPos * controlsDelta + new Vector2(labelWidth, 0), false, MyGuiConstants.CHECKBOX_BACKGROUND_COLOR);
            Controls.Add(m_displayPrefabContainerAxis);
            dPos++;
            

            // display voxel bounding
            MyGuiControlLabel lblVoxelBounding = new MyGuiControlLabel(this, controlsOriginLeft + dPos * controlsDelta, null, MyTextsWrapperEnum.DisplayVoxelBounding, MyGuiConstants.LABEL_TEXT_COLOR,
            MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);
            Controls.Add(lblVoxelBounding);

            m_displayVoxelBounding = new MyGuiControlCheckbox(this, controlsOriginLeft + dPos * controlsDelta + new Vector2(labelWidth, 0), false, MyGuiConstants.CHECKBOX_BACKGROUND_COLOR);
            Controls.Add(m_displayVoxelBounding);

            dPos++;

            // use camera crosshair option
            MyGuiControlLabel label3 = new MyGuiControlLabel(this, controlsOriginLeft + dPos * controlsDelta, null, MyTextsWrapperEnum.UseCameraCrosshair, MyGuiConstants.LABEL_TEXT_COLOR,
                MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);
            Controls.Add(label3);

            m_useCameraCrosshairCheckbox = new MyGuiControlCheckbox(this, controlsOriginLeft + dPos * controlsDelta + new Vector2(labelWidth, 0), false, MyGuiConstants.CHECKBOX_BACKGROUND_COLOR);
            Controls.Add(m_useCameraCrosshairCheckbox);

            dPos++;


            // enable lightw in editor:
            MyGuiControlLabel label5 = new MyGuiControlLabel(this, controlsOriginLeft + dPos * controlsDelta, null, MyTextsWrapperEnum.EnableLightsInEditor, MyGuiConstants.LABEL_TEXT_COLOR,
                MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);
            Controls.Add(label5);

            m_enableLightsInEditorCheckbox = new MyGuiControlCheckbox(this, controlsOriginLeft + dPos * controlsDelta + new Vector2(labelWidth, 0), true, MyGuiConstants.CHECKBOX_BACKGROUND_COLOR);
            Controls.Add(m_enableLightsInEditorCheckbox);

            dPos++;

            // enable snap point filter
            MyGuiControlLabel label7 = new MyGuiControlLabel(this, controlsOriginLeft + dPos * controlsDelta, null, MyTextsWrapperEnum.EnableSnapPointFilter, MyGuiConstants.LABEL_TEXT_COLOR,
                MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);
            Controls.Add(label7);

            m_enableSnapPointFilter = new MyGuiControlCheckbox(this, controlsOriginLeft + dPos * controlsDelta + new Vector2(labelWidth, 0), true, MyGuiConstants.CHECKBOX_BACKGROUND_COLOR);
            Controls.Add(m_enableSnapPointFilter);

            dPos++;

            // fixed snap point size
            MyGuiControlLabel label8 = new MyGuiControlLabel(this, controlsOriginLeft + dPos * controlsDelta, null, MyTextsWrapperEnum.FixedSnapPointSize, MyGuiConstants.LABEL_TEXT_COLOR,
                MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);
            Controls.Add(label8);

            m_fixedSnapPointSize = new MyGuiControlCheckbox(this, controlsOriginLeft + dPos * controlsDelta + new Vector2(labelWidth, 0), true, MyGuiConstants.CHECKBOX_BACKGROUND_COLOR);
            Controls.Add(m_fixedSnapPointSize);

            dPos++;

            // enable object pivot
            MyGuiControlLabel label10 = new MyGuiControlLabel(this, controlsOriginLeft + dPos * controlsDelta, null, MyTextsWrapperEnum.LabelEnableObjectPivot, MyGuiConstants.LABEL_TEXT_COLOR,
                MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);
            Controls.Add(label10);

            m_enableObjectPivot = new MyGuiControlCheckbox(this, controlsOriginLeft + dPos * controlsDelta + new Vector2(labelWidth, 0), true, MyGuiConstants.CHECKBOX_BACKGROUND_COLOR, label10);
            Controls.Add(m_enableObjectPivot);

            dPos++;

            // enable textsDrawing
            MyGuiControlLabel label11 = new MyGuiControlLabel(this, controlsOriginLeft + dPos * controlsDelta, null, MyTextsWrapperEnum.LabelEnableTextsDrawing, MyGuiConstants.LABEL_TEXT_COLOR,
                MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);
            Controls.Add(label11);

            m_enableTextsDrawing = new MyGuiControlCheckbox(this, controlsOriginLeft + dPos * controlsDelta + new Vector2(labelWidth, 0), true, MyGuiConstants.CHECKBOX_BACKGROUND_COLOR, label10);
            Controls.Add(m_enableTextsDrawing);

            dPos++;

            // save player ship
            MyGuiControlLabel label9 = new MyGuiControlLabel(this, controlsOriginLeft + dPos * controlsDelta, null, MyTextsWrapperEnum.LabelSavePlayerShip, MyGuiConstants.LABEL_TEXT_COLOR,
                MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);
            Controls.Add(label9);

            m_savePlayerShip = new MyGuiControlCheckbox(this, controlsOriginLeft + dPos * controlsDelta + new Vector2(labelWidth, 0), true, MyGuiConstants.CHECKBOX_BACKGROUND_COLOR);
            Controls.Add(m_savePlayerShip);



            //label9.Visible = m_displaySavePlayerShipOption;
            //m_savePlayerShip.Visible = m_displaySavePlayerShipOption;

            //  Buttons OK and CANCEL
            Vector2 buttonDelta = new Vector2(0.1f, m_size.Value.Y / 2.0f - MyGuiConstants.MESSAGE_BOX_BORDER_AREA_Y - MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE.Y / 2.0f - 0.01f);
            Controls.Add(new MyGuiControlButton(this, new Vector2(-buttonDelta.X, buttonDelta.Y), MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.Ok, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnOkClick, MyGuiControlButtonTextAlignment.CENTERED, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));
            Controls.Add(new MyGuiControlButton(this, new Vector2(+buttonDelta.X, buttonDelta.Y), MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.Cancel, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnCancelClick, MyGuiControlButtonTextAlignment.CENTERED, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));


            //  Update controls with values from config file
            UpdateFromConfig();
            UpdateControls(m_settingsOld);
            UpdateSettings(m_settingsNew);
        }        

        public override string GetFriendlyName()
        {
            return "MyGuiScreenEditorOptions";
        }


        void UpdateFromConfig()
        {
            m_settingsOld.DisplayUnselectedBounding = MyConfig.EditorDisplayUnselectedBounding;
            m_settingsOld.UseCameraCrosshair = MyConfig.EditorUseCameraCrosshair;
            m_settingsOld.LockedPrefab90DegreesRotation = MyConfig.EditorLockedPrefab90DegreesRotation;
            m_settingsOld.EnableLightsInEditor = MyEditor.EnableLightsInEditor = MyConfig.EditorEnableLightsInEditor;
            m_settingsOld.DisplayPrefabContainerBounding = MyEditor.DisplayPrefabContainerBounding = MyConfig.EditorDisplayPrefabContainerBounding;
            m_settingsOld.DisplayPrefabContainerAxis = MyEditor.DisplayPrefabContainerAxis = MyConfig.EditorDisplayPrefabContainerAxis;
            m_settingsOld.EnableSnapPointFilter = MyEditor.Static.SnapPointFilter = MyConfig.EditorSnapPointFilter;
            m_settingsOld.FixedSizeSnapPoints = MyEditor.Static.FixedSizeSnapPoints = MyConfig.EditorFixedSizeSnapPoints;
            m_settingsOld.SavePlayerShip = MyEditor.SavePlayerShip;
            m_settingsOld.EnableObjectPivot = MyEditor.EnableObjectPivot = MyConfig.EditorEnableObjectPivot;
            m_settingsOld.EnableTextsDrawing = MyEditor.EnableTextsDrawing = MyConfig.EditorEnableTextsDrawing;            
        }

        void UpdateControls(MyGuiScreenOptionsEditorSettings settings)
        {
            //m_displayUnselectedCheckbox.Checked = settings.DisplayUnselectedBounding;
            m_useCameraCrosshairCheckbox.Checked = settings.UseCameraCrosshair;
            //m_lockedPrefab90DegreesRotationCheckbox.Checked = settings.LockedPrefab90DegreesRotation;
            m_enableLightsInEditorCheckbox.Checked = settings.EnableLightsInEditor;

            m_displayVoxelBounding.Checked = MyEditor.DisplayVoxelBounding;
            m_displayPrefabContainerBounding.Checked = MyEditor.DisplayPrefabContainerBounding;
            m_displayPrefabContainerAxis.Checked = MyEditor.DisplayPrefabContainerAxis;
            m_enableSnapPointFilter.Checked = MyEditor.Static.SnapPointFilter;
            m_fixedSnapPointSize.Checked = MyEditor.Static.FixedSizeSnapPoints;
            m_savePlayerShip.Checked = MyEditor.SavePlayerShip;

            m_enableObjectPivot.Checked = MyEditor.EnableObjectPivot;
            m_enableTextsDrawing.Checked = MyEditor.EnableTextsDrawing;
        }

        void UpdateSettings(MyGuiScreenOptionsEditorSettings settings)
        {
            //settings.DisplayUnselectedBounding = m_displayUnselectedCheckbox.Checked;
            settings.UseCameraCrosshair = m_useCameraCrosshairCheckbox.Checked;
            settings.LockedPrefab90DegreesRotation = false;
            settings.EnableLightsInEditor = m_enableLightsInEditorCheckbox.Checked;

            settings.DisplayVoxelBounding = m_displayVoxelBounding.Checked;
            settings.DisplayPrefabContainerBounding = m_displayPrefabContainerBounding.Checked;
            settings.DisplayPrefabContainerAxis = m_displayPrefabContainerAxis.Checked;
            settings.EnableSnapPointFilter = m_enableSnapPointFilter.Checked;
            settings.FixedSizeSnapPoints = m_fixedSnapPointSize.Checked;
            settings.SavePlayerShip = m_savePlayerShip.Checked;

            settings.EnableObjectPivot = m_enableObjectPivot.Checked;
            settings.EnableTextsDrawing = m_enableTextsDrawing.Checked;
        }

        void Save()
        {
            UpdateSettings(m_settingsNew);
            MyConfig.EditorDisplayUnselectedBounding = m_settingsNew.DisplayUnselectedBounding;
            MyConfig.EditorUseCameraCrosshair = m_settingsNew.UseCameraCrosshair;
            MyConfig.EditorLockedPrefab90DegreesRotation = m_settingsNew.LockedPrefab90DegreesRotation;
            MyConfig.EditorEnableLightsInEditor = MyEditor.EnableLightsInEditor = m_settingsNew.EnableLightsInEditor;
            MyConfig.EditorDisplayVoxelBounding = MyEditor.DisplayVoxelBounding = m_displayVoxelBounding.Checked;
            MyConfig.EditorDisplayPrefabContainerBounding = MyEditor.DisplayPrefabContainerBounding = m_displayPrefabContainerBounding.Checked;
            MyConfig.EditorDisplayPrefabContainerAxis = MyEditor.DisplayPrefabContainerAxis = m_displayPrefabContainerAxis.Checked;
            MyConfig.EditorSnapPointFilter = MyEditor.Static.SnapPointFilter = m_enableSnapPointFilter.Checked;
            MyConfig.EditorFixedSizeSnapPoints = MyEditor.Static.FixedSizeSnapPoints = m_fixedSnapPointSize.Checked;
            MyEditor.SavePlayerShip = m_savePlayerShip.Checked;
            MyConfig.EditorEnableObjectPivot = MyEditor.EnableObjectPivot = m_enableObjectPivot.Checked;
            MyConfig.EditorEnableTextsDrawing = MyEditor.EnableTextsDrawing = m_enableTextsDrawing.Checked;
            MyConfig.Save();
        }

        public void OnOkClick(MyGuiControlButton sender)
        {
            //  Save values
            Save();
            CloseScreen();
        }

        public void OnCancelClick(MyGuiControlButton sender)
        {
            //  Revert to OLD values
            MyConfig.EditorDisplayUnselectedBounding = m_settingsOld.DisplayUnselectedBounding;
            MyConfig.EditorUseCameraCrosshair = m_settingsOld.UseCameraCrosshair;
            MyConfig.EditorLockedPrefab90DegreesRotation = m_settingsOld.LockedPrefab90DegreesRotation;
            MyEditor.EnableLightsInEditor = m_settingsOld.EnableLightsInEditor;
            MyEditor.DisplayPrefabContainerAxis = m_settingsOld.DisplayPrefabContainerAxis;
            MyEditor.DisplayVoxelBounding = m_settingsOld.DisplayVoxelBounding;
            MyEditor.DisplayPrefabContainerBounding = m_settingsOld.DisplayPrefabContainerBounding;
            MyEditor.Static.SnapPointFilter = m_settingsOld.EnableSnapPointFilter;
            MyEditor.Static.FixedSizeSnapPoints = m_settingsOld.FixedSizeSnapPoints;
            MyEditor.SavePlayerShip = m_settingsOld.SavePlayerShip;
            MyEditor.EnableObjectPivot = m_settingsOld.EnableObjectPivot;
            MyEditor.EnableTextsDrawing = m_settingsOld.EnableTextsDrawing;
            CloseScreen();
        }
    }
}
