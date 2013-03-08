using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Editor;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Entities.WayPoints;
using MinerWars.AppCode.Game.Prefabs;
using MinerWars.AppCode.Game.Entities.InfluenceSpheres;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;

namespace MinerWars.AppCode.Game.GUI.ScreenEditor
{
    class MyGuiScreenEditorSelectAndHide : MyGuiScreenBase
    {
        // if you add a type here, add it to MyEditorActionWithObjectBuildersBase.AddToScene()!
        const int TYPE_COUNT = 6;
        MyTextsWrapperEnum[] rowLabels = { MyTextsWrapperEnum.Waypoints, MyTextsWrapperEnum.Voxels, MyTextsWrapperEnum.Prefabs, MyTextsWrapperEnum.SpawnPoints, MyTextsWrapperEnum.InfluenceSpheres, MyTextsWrapperEnum.Dummies };
        Type[] types = { typeof(MyWayPoint), typeof(MyVoxelMap), typeof(MyPrefabBase), typeof(MySpawnPoint), typeof(MyInfluenceSphere), typeof(MyDummyPoint) };

        MyGuiControlCheckbox[] m_checkboxVisible = new MyGuiControlCheckbox[TYPE_COUNT];
        MyGuiControlCheckbox[] m_checkboxSelectable = new MyGuiControlCheckbox[TYPE_COUNT];
        MyGuiControlCheckbox m_checkboxSnapPointsVisible;
        MyGuiControlCheckbox m_checkboxWaypointIgnoreDepth;
        MyGuiControlCheckbox m_checkboxGeneratorsRangeVisible;
        MyGuiControlCheckbox m_checkboxLargeWeaponsRangeVisible;
        MyGuiControlCheckbox m_checkboxSafeAreasVisible;
        MyGuiControlCheckbox m_checkboxSafeAreasSelectable;
        MyGuiControlCheckbox m_checkboxDetectorsVisible;
        MyGuiControlCheckbox m_checkboxDetectorsSelectable;
        MyGuiControlCheckbox m_checkboxParticleEffectsVisible;
        MyGuiControlCheckbox m_checkboxParticleEffectsSelectable;
        MyGuiControlCheckbox m_checkboxDisplayDeactivatedEntities;

        public override string GetFriendlyName()
        {
            return "MyGuiScreenEditorSelectAndHide";
        }

        public MyGuiScreenEditorSelectAndHide()
            : base(new Vector2(0.5f, 0.5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, null)
        {
            m_enableBackgroundFade = true;
            m_size = new Vector2(0.5f, 0.4025f + (TYPE_COUNT + 4) * MyGuiConstants.CONTROLS_DELTA.Y + 1.3f * MyGuiConstants.CONTROLS_DELTA.Y);

            AddCaption(MyTextsWrapperEnum.EditorSelectAndHide, new Vector2(0, 0.01f));

            Vector2 controlsOriginLeft = new Vector2(-m_size.Value.X / 2.0f + 0.08f, -m_size.Value.Y / 2.0f + 0.1f);
            Vector2 controlsOriginRight = new Vector2(-m_size.Value.X / 2.0f + 0.181f, -m_size.Value.Y / 2.0f + 0.1f);

            Vector2 pos = controlsOriginLeft;
            Vector2 tab1 = new Vector2(0.19f, 0);
            Vector2 tab2 = new Vector2(0.29f, 0);

            Controls.Add(new MyGuiControlLabel(this, pos + tab1, null, MyTextsWrapperEnum.Selectable, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER));
            Controls.Add(new MyGuiControlLabel(this, pos + tab2, null, MyTextsWrapperEnum.Visible, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER));

            for (int i = 0; i < m_checkboxVisible.Length; i++)
            {
                pos += MyGuiConstants.CONTROLS_DELTA - new Vector2(0,0.0025f);
                Controls.Add(new MyGuiControlLabel(this, pos, null, rowLabels[i], MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
                Controls.Add(m_checkboxSelectable[i] = new MyGuiControlCheckbox(this, pos + tab1, false, MyGuiConstants.CHECKBOX_BACKGROUND_COLOR));
                Controls.Add(m_checkboxVisible[i] = new MyGuiControlCheckbox(this, pos + tab2, false, MyGuiConstants.CHECKBOX_BACKGROUND_COLOR));

                // uncheck Visible: uncheck its Selectable
                m_checkboxVisible[i].OnCheck = delegate(MyGuiControlCheckbox sender)
                {
                    if (!sender.Checked)
                        for (int j = 0; j < m_checkboxVisible.Length; j++)
                            if (m_checkboxVisible[j] == sender)
                                m_checkboxSelectable[j].Checked = false;
                };
                // check Selectable: check its Visible
                m_checkboxSelectable[i].OnCheck = delegate(MyGuiControlCheckbox sender)
                {
                    if (sender.Checked)
                        for (int j = 0; j < m_checkboxSelectable.Length; j++)
                            if (m_checkboxSelectable[j] == sender)
                                m_checkboxVisible[j].Checked = true;
                };
            }

            //safe areas
            pos += MyGuiConstants.CONTROLS_DELTA;
            Controls.Add(new MyGuiControlLabel(this, pos, null, MyTextsWrapperEnum.SafeAreaDummies, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            Controls.Add(m_checkboxSafeAreasSelectable = new MyGuiControlCheckbox(this, pos + tab1, false, MyGuiConstants.CHECKBOX_BACKGROUND_COLOR));
            Controls.Add(m_checkboxSafeAreasVisible = new MyGuiControlCheckbox(this, pos + tab2, false, MyGuiConstants.CHECKBOX_BACKGROUND_COLOR));

            //detectors
            pos += MyGuiConstants.CONTROLS_DELTA;
            Controls.Add(new MyGuiControlLabel(this, pos, null, MyTextsWrapperEnum.DetectorDummies, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            Controls.Add(m_checkboxDetectorsSelectable = new MyGuiControlCheckbox(this, pos + tab1, false, MyGuiConstants.CHECKBOX_BACKGROUND_COLOR));
            Controls.Add(m_checkboxDetectorsVisible = new MyGuiControlCheckbox(this, pos + tab2, false, MyGuiConstants.CHECKBOX_BACKGROUND_COLOR));

            //particle effects
            pos += MyGuiConstants.CONTROLS_DELTA;
            Controls.Add(new MyGuiControlLabel(this, pos, null, MyTextsWrapperEnum.ParticleDummies, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            Controls.Add(m_checkboxParticleEffectsSelectable = new MyGuiControlCheckbox(this, pos + tab1, false, MyGuiConstants.CHECKBOX_BACKGROUND_COLOR));
            Controls.Add(m_checkboxParticleEffectsVisible = new MyGuiControlCheckbox(this, pos + tab2, false, MyGuiConstants.CHECKBOX_BACKGROUND_COLOR));
            
            // uncheck Visible: uncheck its Selectable
            m_checkboxSafeAreasVisible.OnCheck = delegate(MyGuiControlCheckbox sender) { if (!sender.Checked) m_checkboxSafeAreasSelectable.Checked = false; };
            m_checkboxDetectorsVisible.OnCheck = delegate(MyGuiControlCheckbox sender) { if (!sender.Checked) m_checkboxDetectorsSelectable.Checked = false; };
            m_checkboxParticleEffectsVisible.OnCheck = delegate(MyGuiControlCheckbox sender) { if (!sender.Checked) m_checkboxParticleEffectsSelectable.Checked = false; };
            
            // check Selectable: check its Visible
            m_checkboxSafeAreasSelectable.OnCheck = delegate(MyGuiControlCheckbox sender) { if (sender.Checked) m_checkboxSafeAreasVisible.Checked = true; };
            m_checkboxDetectorsSelectable.OnCheck = delegate(MyGuiControlCheckbox sender) { if (sender.Checked) m_checkboxDetectorsVisible.Checked = true; };
            m_checkboxParticleEffectsSelectable.OnCheck = delegate(MyGuiControlCheckbox sender) { if (sender.Checked) m_checkboxParticleEffectsVisible.Checked = true; };

            //  Are snap points hidden?
            pos += MyGuiConstants.CONTROLS_DELTA;
            Controls.Add(new MyGuiControlLabel(this, pos, null, MyTextsWrapperEnum.SnapPoints, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            Controls.Add(m_checkboxSnapPointsVisible = new MyGuiControlCheckbox(this, pos + tab2, false, MyGuiConstants.CHECKBOX_BACKGROUND_COLOR));

            //  Do waypoints ignore depth?
            pos += MyGuiConstants.CONTROLS_DELTA * 1.2f;
            Controls.Add(new MyGuiControlLabel(this, pos, null, MyTextsWrapperEnum.WaypointsIgnoreDepth, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            Controls.Add(m_checkboxWaypointIgnoreDepth = new MyGuiControlCheckbox(this, pos + tab2, false, MyGuiConstants.CHECKBOX_BACKGROUND_COLOR));

            //  Generators range visible
            pos += MyGuiConstants.CONTROLS_DELTA;
            Controls.Add(new MyGuiControlLabel(this, pos, null, MyTextsWrapperEnum.GeneratorsRange, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            Controls.Add(m_checkboxGeneratorsRangeVisible = new MyGuiControlCheckbox(this, pos + tab2, false, MyGuiConstants.CHECKBOX_BACKGROUND_COLOR));

            //  Large weapons range visible
            pos += MyGuiConstants.CONTROLS_DELTA;
            Controls.Add(new MyGuiControlLabel(this, pos, null, MyTextsWrapperEnum.LargeWeaponsRange, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            Controls.Add(m_checkboxLargeWeaponsRangeVisible = new MyGuiControlCheckbox(this, pos + tab2, false, MyGuiConstants.CHECKBOX_BACKGROUND_COLOR));

            //  Display deactivated entities
            pos += MyGuiConstants.CONTROLS_DELTA;
            Controls.Add(new MyGuiControlLabel(this, pos, null, MyTextsWrapperEnum.DisplayDeactivatedEntities, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            Controls.Add(m_checkboxDisplayDeactivatedEntities = new MyGuiControlCheckbox(this, pos + tab2, false, MyGuiConstants.CHECKBOX_BACKGROUND_COLOR));            

            //  Buttons OK and CANCEL
            Vector2 buttonDelta = new Vector2(0.09f, m_size.Value.Y / 2.0f - MyGuiConstants.MESSAGE_BOX_BORDER_AREA_Y - MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE.Y / 2.0f - 0.025f);
            Controls.Add(new MyGuiControlButton(this, new Vector2(-buttonDelta.X, buttonDelta.Y), MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.Ok, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnOkClick, MyGuiControlButtonTextAlignment.CENTERED, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));
            Controls.Add(new MyGuiControlButton(this, new Vector2(+buttonDelta.X, buttonDelta.Y), MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.Cancel, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnCancelClick, MyGuiControlButtonTextAlignment.CENTERED, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));

            InitControls();
        }

        void InitControls()
        {
            for (int i = 0; i < TYPE_COUNT; i++)
            {
                m_checkboxVisible[i].Checked = !MyEntities.IsTypeHidden(types[i]);
                m_checkboxSelectable[i].Checked = MyEntities.IsTypeSelectable(types[i]);
            }
            m_checkboxSnapPointsVisible.Checked = MyEditor.Static.ShowSnapPoints;
            m_checkboxWaypointIgnoreDepth.Checked = MyWayPointGraph.WaypointsIgnoreDepth;
            m_checkboxGeneratorsRangeVisible.Checked = MyEditor.Static.ShowGeneratorsRange;
            m_checkboxLargeWeaponsRangeVisible.Checked = MyEditor.Static.ShowLargeWeaponsRange;
            m_checkboxSafeAreasVisible.Checked = !MyEntities.SafeAreasHidden;
            m_checkboxDetectorsVisible.Checked = !MyEntities.DetectorsHidden;
            m_checkboxParticleEffectsVisible.Checked = !MyEntities.ParticleEffectsHidden;
            m_checkboxSafeAreasSelectable.Checked = MyEntities.SafeAreasSelectable;
            m_checkboxDetectorsSelectable.Checked = MyEntities.DetectorsSelectable;
            m_checkboxParticleEffectsSelectable.Checked = MyEntities.ParticleEffectsSelectable;
            m_checkboxDisplayDeactivatedEntities.Checked = MyEditor.Static.ShowDeactivatedEntities;
        }


        void SaveSettings()
        {
            MyConfig.EditorHiddenWayPoint = MyEntities.IsTypeHidden(typeof(MyWayPoint));
            MyConfig.EditorSelectableWayPoint = MyEntities.IsTypeSelectable(typeof(MyWayPoint));

            MyConfig.EditorHiddenVoxelMap = MyEntities.IsTypeHidden(typeof(MyVoxelMap));
            MyConfig.EditorSelectableVoxelMap = MyEntities.IsTypeSelectable(typeof(MyVoxelMap));

            MyConfig.EditorHiddenDummyPoint = MyEntities.IsTypeHidden(typeof(MyDummyPoint));
            MyConfig.EditorSelectableDummyPoint = MyEntities.IsTypeSelectable(typeof(MyDummyPoint));

            MyConfig.EditorHiddenPrefabBase = MyEntities.IsTypeHidden(typeof(MyPrefabBase));
            MyConfig.EditorSelectablePrefabBase = MyEntities.IsTypeSelectable(typeof(MyPrefabBase));

            MyConfig.EditorHiddenSpawnPoint = MyEntities.IsTypeHidden(typeof(MySpawnPoint));
            MyConfig.EditorSelectableSpawnPoint = MyEntities.IsTypeSelectable(typeof(MySpawnPoint));

            MyConfig.EditorHiddenInfluenceSphere = MyEntities.IsTypeHidden(typeof(MyInfluenceSphere));
            MyConfig.EditorSelectableInfluenceSphere = MyEntities.IsTypeSelectable(typeof(MyInfluenceSphere));

            MyConfig.EditorShowSnapPoints = MyEditor.Static.ShowSnapPoints;
            MyConfig.EditorShowSafeAreas = !MyEntities.SafeAreasHidden;
            MyConfig.EditorShowDetectors = !MyEntities.DetectorsHidden;
            MyConfig.EditorShowParticleEffects = !MyEntities.ParticleEffectsHidden;

            MyConfig.EditorSelectableSafeAreas = MyEntities.SafeAreasSelectable;
            MyConfig.EditorSelectableDetectors = MyEntities.DetectorsSelectable;
            MyConfig.EditorSelectableParticleEffects = MyEntities.ParticleEffectsSelectable;

            MyConfig.EditorWaypointsIgnoreDepth = MyWayPointGraph.WaypointsIgnoreDepth;

            MyConfig.EditorShowGeneratorsRange = MyEditor.Static.ShowGeneratorsRange;
            MyConfig.EditorShowLargeWeaponsRange = MyEditor.Static.ShowLargeWeaponsRange;
            MyConfig.EditorShowDeactivatedEntities = MyEditor.Static.ShowDeactivatedEntities;

            MyConfig.Save();
        }



        public void OnOkClick(MyGuiControlButton sender)
        {
            // set hidden/selectable flags
            for (int i = 0; i < TYPE_COUNT; i++)
            {
                MyEntities.SetTypeHidden(types[i], !m_checkboxVisible[i].Checked);
                MyEntities.SetTypeSelectable(types[i], m_checkboxSelectable[i].Checked);
            }

            MyEntities.SafeAreasHidden = !m_checkboxSafeAreasVisible.Checked;
            MyEntities.DetectorsHidden = !m_checkboxDetectorsVisible.Checked;
            MyEntities.ParticleEffectsHidden = !m_checkboxParticleEffectsVisible.Checked;

            MyEntities.SafeAreasSelectable = m_checkboxSafeAreasSelectable.Checked;
            MyEntities.DetectorsSelectable = m_checkboxDetectorsSelectable.Checked;
            MyEntities.ParticleEffectsSelectable = m_checkboxParticleEffectsSelectable.Checked;

            MyEditor.Static.ShowSnapPoints = m_checkboxSnapPointsVisible.Checked;
            MyEditor.Static.ShowGeneratorsRange = m_checkboxGeneratorsRangeVisible.Checked;
            MyEditor.Static.ShowLargeWeaponsRange = m_checkboxLargeWeaponsRangeVisible.Checked;
            MyEditor.Static.ShowDeactivatedEntities = m_checkboxDisplayDeactivatedEntities.Checked;

            // if a selected object became unselectable, remove it from selection
            var toRemove = new List<MyEntity>();
            foreach (var selected in MyEditorGizmo.SelectedEntities)
            {
                for (int i = 0; i < TYPE_COUNT; i++)
                    if (!MyEntities.IsTypeSelectable(types[i]) && types[i].IsInstanceOfType(selected))
                        toRemove.Add(selected);
                var dummy = selected as MyDummyPoint;
                if (dummy != null) switch (dummy.DummyFlags)
                {
                    case MyDummyPointFlags.SAFE_AREA: if (!MyEntities.SafeAreasSelectable) toRemove.Add(selected); break;
                    case MyDummyPointFlags.DETECTOR: if (!MyEntities.DetectorsSelectable) toRemove.Add(selected); break;
                    case MyDummyPointFlags.PARTICLE: if (!MyEntities.ParticleEffectsSelectable) toRemove.Add(selected); break;
                    default: break;
                }
            }

            MyEditorGizmo.RemoveEntitiesFromSelection(toRemove);

            MyEditor.Static.RefreshSelectionSettings();

            // waypoints ignore depth?
            MyWayPointGraph.WaypointsIgnoreDepth = m_checkboxWaypointIgnoreDepth.Checked;

            SaveSettings();

            CloseScreen();
        }

        public void OnCancelClick(MyGuiControlButton sender)
        {
            CloseScreen();
        }
    }
}
