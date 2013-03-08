using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.Editor;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Entities.VoxelHandShapes;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.AppCode.Game.Entities;

namespace MinerWars.AppCode.Game.GUI.Helpers
{
    class MyGuiControlEditVoxelHand : MyGuiControlBase
    {        
        private MyGuiControlRadioButtonGroup m_voxelShapeTypeRadioButtonGroup;                
        private MyGuiControlRadioButtonGroup m_voxelShapeModeRadioButtonGroup;        
        
        /*
        private MyGuiControlSlider m_voxelShapeSizeSlider;
        private MyGuiControlSlider m_voxelShapeSize2Slider;
        private MyGuiControlSlider m_voxelShapeSize3Slider;
          */

        private MyGuiControlSlider m_voxelShapeDistanceSlider;
        private MyGuiControlCombobox m_voxelShapeMaterialCombobox;
        private MyGuiControlCheckbox m_isProjectedCheckbox;
        private MyGuiControlCheckbox m_isProjectedOnWaypointCheckbox;
        private MyGuiControlLabel m_voxelShapeDistanceLabel;
        private MyGuiControlLabel m_voxelShapeMaterialLabel;
        private MyGuiControlCheckbox m_attachDetachVoxelHandCheckbox;
        private MyGuiControlLabel m_detachLabel;
        
        private List<MyGuiControlBase> m_controls;
        private List<MyGuiControlBase> m_sortedControlsByPriority;
        private List<MyGuiControlBase> m_dynamicControls = new List<MyGuiControlBase>();

        private bool m_sizeChangedFromEditor = false;
        private bool m_distanceChangedFromEditor = false;
        private float m_offsetForSizeProperties = 0;
        bool m_canSelect = false;

        public MyGuiControlEditVoxelHand(IMyGuiControlsParent parent, Vector2 position, Vector2 size, Vector4 backgroundColor)
            : base(parent, position, size, backgroundColor, null)
        {                     
            LoadControls();

            
            if (MyEditorVoxelHand.VoxelHandShape.Material != null)
            {
                m_voxelShapeMaterialCombobox.SelectItemByKey((int)MyEditorVoxelHand.VoxelHandShape.Material);
            }
            else 
            {
                m_voxelShapeMaterialCombobox.SelectItemByKey(0);
            }

            m_canSelect = true;
            //MyEditorVoxelHand.OnVoxelShapeSize += OnVoxelHandSizeChanged;
            //MyEditorVoxelHand.OnVoxelShapeDistance += OnVoxelHandDistanceChanged;
        }

        public void LoadControls()
        {            
            m_controls = new List<MyGuiControlBase>();
            Vector2 labelOffset = new Vector2(-0.015f, 0f);
            Vector2 controlsOriginLeft = m_position + new Vector2(-m_size.Value.X / 2.0f + 0.025f, -m_size.Value.Y / 2.0f + 0.025f);
            Vector2 controlsOriginRight = m_position + new Vector2(m_size.Value.X / 2.0f - 0.025f, -m_size.Value.Y / 2.0f + 0.025f);
            Vector2 controlsDelta = MyGuiConstants.CONTROLS_DELTA * 0.6f;            

            // add panel
            //m_controls.Add(new MyGuiControlPanel(m_parentScreen, m_position, m_size, m_backgroundColor.Value, null, null, null, null, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER));
            m_controls.Add(new MyGuiControlPanel(m_parent, m_position, m_size, m_backgroundColor.Value, 2, MyGuiConstants.TREEVIEW_VERTICAL_LINE_COLOR));

            #region shape types
            m_controls.Add(new MyGuiControlLabel(m_parent, controlsOriginLeft + labelOffset, null, MyTextsWrapperEnum.EditVoxelHandShapeType, MyGuiConstants.LABEL_TEXT_COLOR,
                MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));            
            m_voxelShapeTypeRadioButtonGroup = new MyGuiControlRadioButtonGroup();

            foreach (MyVoxelHandShapeType voxelShapeType in MyGuiEditorVoxelHandHelpers.MyEditorVoxelHandShapeHelperTypesEnumValues)
            {
                controlsOriginLeft += controlsDelta;
                MyGuiEditorVoxelHandHelper voxelHandHelper = MyGuiEditorVoxelHandHelpers.GetEditorVoxelHandShapeHelper(voxelShapeType);

                MyGuiControlRadioButton voxelShapeTypeRadioButton = new MyGuiControlRadioButton(m_parent, controlsOriginLeft, (int)voxelShapeType,
                    MyGuiConstants.RADIOBUTTON_BACKGROUND_COLOR);
                MyGuiControlLabel voxelShapeTypeDescription = new MyGuiControlLabel(m_parent, controlsOriginLeft + new Vector2(voxelShapeTypeRadioButton.GetSize().Value.X, 0.0f), null, voxelHandHelper.Description, MyGuiConstants.LABEL_TEXT_COLOR,
                    MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);

                m_controls.Add(voxelShapeTypeRadioButton);
                m_controls.Add(voxelShapeTypeDescription);

                m_voxelShapeTypeRadioButtonGroup.Add(voxelShapeTypeRadioButton);                
            }
            #endregion

            controlsOriginLeft += MyGuiConstants.CONTROLS_DELTA;

            #region voxel mode types
            m_controls.Add(new MyGuiControlLabel(m_parent, controlsOriginLeft + labelOffset, null, MyTextsWrapperEnum.EditVoxelHandShapeMode, MyGuiConstants.LABEL_TEXT_COLOR,
                MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));            
            m_voxelShapeModeRadioButtonGroup = new MyGuiControlRadioButtonGroup();
            foreach (MyMwcVoxelHandModeTypeEnum voxelModeType in MyGuiEditorVoxelHandHelpers.MyEditorVoxelHandModeHelperTypesEnumValues)
            {
                controlsOriginLeft += controlsDelta;
                MyGuiEditorVoxelHandHelper voxelHandHelper = MyGuiEditorVoxelHandHelpers.GetEditorVoxelHandModeHelper(voxelModeType);

                MyGuiControlRadioButton voxelShapeModeRadioButton = new MyGuiControlRadioButton(m_parent, controlsOriginLeft, (int)voxelModeType,
                    MyGuiConstants.RADIOBUTTON_BACKGROUND_COLOR);
                MyGuiControlLabel voxelShapeModeDescription = new MyGuiControlLabel(m_parent, controlsOriginLeft + new Vector2(voxelShapeModeRadioButton.GetSize().Value.X, 0.0f), null, voxelHandHelper.Description, MyGuiConstants.LABEL_TEXT_COLOR,
                    MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);

                m_controls.Add(voxelShapeModeRadioButton);
                m_controls.Add(voxelShapeModeDescription);

                m_voxelShapeModeRadioButtonGroup.Add(voxelShapeModeRadioButton);                
            }
            #endregion

            controlsOriginLeft += MyGuiConstants.CONTROLS_DELTA;

            #region is projected

            m_controls.Add(new MyGuiControlLabel(m_parent, controlsOriginLeft + labelOffset, null, MyTextsWrapperEnum.EditVoxelHandIsProjected, MyGuiConstants.LABEL_TEXT_COLOR,
                MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            m_controls.Add(m_isProjectedCheckbox = new MyGuiControlCheckbox(m_parent, new Vector2(controlsOriginRight.X - MyGuiConstants.CHECKBOX_SIZE.X, controlsOriginLeft.Y), false, MyGuiConstants.CHECKBOX_BACKGROUND_COLOR));

            m_isProjectedCheckbox.Checked = MyEditorVoxelHand.IsProjected;
            m_isProjectedCheckbox.OnCheck = OnCheckboxChecked;

            controlsOriginLeft += MyGuiConstants.CONTROLS_DELTA;
            m_controls.Add(new MyGuiControlLabel(m_parent, controlsOriginLeft + labelOffset, null, MyTextsWrapperEnum.EditVoxelHandIsProjectedToWaypoints, MyGuiConstants.LABEL_TEXT_COLOR,
                MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            m_controls.Add(m_isProjectedOnWaypointCheckbox = new MyGuiControlCheckbox(m_parent, new Vector2(controlsOriginRight.X - MyGuiConstants.CHECKBOX_SIZE.X, controlsOriginLeft.Y), false, MyGuiConstants.CHECKBOX_BACKGROUND_COLOR));

            m_isProjectedOnWaypointCheckbox.Checked = MyEditorVoxelHand.IsProjectedToWaypoints;
            m_isProjectedOnWaypointCheckbox.OnCheck = OnCheckboxChecked;

            #endregion

            controlsOriginLeft += new Vector2(0, MyGuiConstants.SLIDER_HEIGHT);
            m_offsetForSizeProperties = controlsOriginLeft.Y;

            RecreateDynamicProperties();

            m_voxelShapeTypeRadioButtonGroup.SetSelected((int)MyEditorVoxelHand.VoxelHandShape.GetShapeType());
            m_voxelShapeTypeRadioButtonGroup.OnSelectedChanged += OnRadioButtonSelectedChange;

            m_voxelShapeModeRadioButtonGroup.SetSelected((int)MyEditorVoxelHand.VoxelHandShape.ModeType);
            m_voxelShapeModeRadioButtonGroup.OnSelectedChanged += OnRadioButtonSelectedChange;
        }

        private void RecreateDynamicProperties()
        {
            Vector2 controlsOriginLeft = m_position + new Vector2(-m_size.Value.X / 2.0f + 0.025f, -m_size.Value.Y / 2.0f + 0.025f);
            controlsOriginLeft.Y = m_offsetForSizeProperties;
            Vector2 labelOffset = new Vector2(-0.015f, 0f);
            Vector2 controlsDelta = MyGuiConstants.CONTROLS_DELTA * 0.6f;            

            #region shape size slidebar

            foreach (MyGuiControlBase oldControl in m_dynamicControls)
            {
                m_controls.Remove(oldControl);
            }
            m_dynamicControls.Clear();


            float propScale = 0.65f;
            float sliderWidth = MyGuiConstants.SLIDER_WIDTH * 1.2f;
            
            for (int i = 0; i < MyEditorVoxelHand.VoxelHandShape.GetPropertiesCount(); i++)
            {
                MyGuiControlLabel label = new MyGuiControlLabel(m_parent, controlsOriginLeft + labelOffset, null, MyEditorVoxelHand.VoxelHandShape.GetPropertyName(i), MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE * propScale, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);
                MyGuiControlSlider voxelShapeSizeSlider = new MyGuiControlSlider(m_parent, controlsOriginLeft + new Vector2(0.145f, 0), sliderWidth,
                    MyVoxelConstants.MIN_VOXEL_HAND_SIZE, MyVoxelConstants.MAX_VOXEL_HAND_SIZE * MyVoxelConstants.VOXEL_SIZE_IN_METRES, MyGuiConstants.SLIDER_BACKGROUND_COLOR,
                    new StringBuilder("{0}"), 0.05f, 0, MyGuiConstants.LABEL_TEXT_SCALE * 0.85f, propScale);
                m_dynamicControls.Add(label);
                m_dynamicControls.Add(voxelShapeSizeSlider);
                m_controls.Add(label);
                m_controls.Add(voxelShapeSizeSlider);

                voxelShapeSizeSlider.SetValue(MyEditorVoxelHand.VoxelHandShape.GetPropertyValue(i));
                voxelShapeSizeSlider.UserData = i;
                voxelShapeSizeSlider.OnChange += OnSizeSliderChange;

                controlsOriginLeft += new Vector2(0, MyGuiConstants.SLIDER_HEIGHT * propScale * 0.75f);
            }

            controlsOriginLeft += new Vector2(0, MyGuiConstants.SLIDER_HEIGHT * propScale);

            #endregion

            #region shape distance slidebar

            if (m_voxelShapeDistanceLabel != null)
            {
                m_voxelShapeDistanceLabel.SetPosition(controlsOriginLeft + labelOffset);
                m_voxelShapeDistanceSlider.SetPosition(controlsOriginLeft + new Vector2(0.145f, 0));
            }
            else
            {
                m_controls.Add(m_voxelShapeDistanceLabel = new MyGuiControlLabel(m_parent, controlsOriginLeft + labelOffset, null, MyTextsWrapperEnum.EditVoxelHandShapeDistance, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE * propScale, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
                m_voxelShapeDistanceSlider = new MyGuiControlSlider(m_parent, controlsOriginLeft + new Vector2(0.145f, 0), sliderWidth,
                    MyVoxelConstants.MIN_VOXEL_HAND_DISTANCE, MyVoxelConstants.MAX_VOXEL_HAND_DISTANCE, MyGuiConstants.SLIDER_BACKGROUND_COLOR,
                    new StringBuilder("{0:0.00}"), 0.05f, 2, MyGuiConstants.LABEL_TEXT_SCALE * 0.85f, propScale);
                m_controls.Add(m_voxelShapeDistanceSlider);

                m_voxelShapeDistanceSlider.SetValue(MyEditorVoxelHand.GetShapeDistance());
                m_voxelShapeDistanceSlider.OnChange += OnDistanceSliderChange;
            }

            #endregion


            controlsOriginLeft += MyGuiConstants.CONTROLS_DELTA;

            #region shape material
            Vector2 iconSize = new Vector2(0.095f, 0.095f);

            if (m_voxelShapeMaterialLabel != null)
            {
                m_voxelShapeMaterialLabel.SetPosition(controlsOriginLeft + labelOffset);
                controlsOriginLeft += controlsDelta;
                m_voxelShapeMaterialCombobox.SetPosition(new Vector2(controlsOriginLeft.X - 0.015f + MyGuiConstants.COMBOBOX_MEDIUM_SIZE.X / 2.0f, controlsOriginLeft.Y));
            }
            else
            {
                m_controls.Add(m_voxelShapeMaterialLabel = new MyGuiControlLabel(m_parent, controlsOriginLeft + labelOffset, null, MyTextsWrapperEnum.EditVoxelHandShapeMaterial, MyGuiConstants.LABEL_TEXT_COLOR,
                    MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
                controlsOriginLeft += controlsDelta;
                m_voxelShapeMaterialCombobox = new MyGuiControlCombobox(m_parent, new Vector2(controlsOriginLeft.X - 0.015f, controlsOriginLeft.Y) + new Vector2(MyGuiConstants.COMBOBOX_MEDIUM_SIZE.X / 2.0f, iconSize.Y / 2.5f), MyGuiControlPreDefinedSize.MEDIUM,
                    iconSize, new Vector2(0.015f, 0f), MyGuiConstants.COMBOBOX_BACKGROUND_COLOR * 0.7f, MyGuiConstants.COMBOBOX_TEXT_SCALE, 4, true, false, true);

                foreach (MyMwcVoxelMaterialsEnum voxelMaterial in MyGuiAsteroidHelpers.MyMwcVoxelMaterialsEnumValues)
                {
                    MyGuiVoxelMaterialHelper voxelMaterialHelper = MyGuiAsteroidHelpers.GetMyGuiVoxelMaterialHelper(voxelMaterial);
                    m_voxelShapeMaterialCombobox.AddItem((int)voxelMaterial, voxelMaterialHelper.Icon, voxelMaterialHelper.Description);
                }
                m_controls.Add(m_voxelShapeMaterialCombobox);
                m_voxelShapeMaterialCombobox.OnSelect += OnComboboxItemSelect;
            }

            #endregion

            controlsOriginLeft += 3.4f * MyGuiConstants.CONTROLS_DELTA;
            if (m_attachDetachVoxelHandCheckbox != null)
                m_controls.Remove(m_attachDetachVoxelHandCheckbox);
            if (m_detachLabel != null)
                m_controls.Remove(m_detachLabel);

            MyTextsWrapperEnum text = MyEditorVoxelHand.DetachedVoxelHand == null ? MyTextsWrapperEnum.DetachVoxelHand : MyTextsWrapperEnum.AttachVoxelHand;

            m_detachLabel = new MyGuiControlLabel(m_parent, controlsOriginLeft + labelOffset, null, text, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE * propScale, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);
            m_controls.Add(m_detachLabel);

            m_attachDetachVoxelHandCheckbox = new MyGuiControlCheckbox(m_parent, new Vector2(controlsOriginLeft.X + 5*MyGuiConstants.CHECKBOX_SIZE.X, controlsOriginLeft.Y), false, MyGuiConstants.CHECKBOX_BACKGROUND_COLOR, m_detachLabel);
            m_attachDetachVoxelHandCheckbox.OnCheck += OnAttachClick;

            m_controls.Add(m_attachDetachVoxelHandCheckbox);
                 

            LoadSortedControlsByPriority();
        }

        private void OnAttachClick(MyGuiControlBase button)
        {
            if (MyEditorVoxelHand.DetachedVoxelHand == null)
            {
                foreach (MyEntity e in MyEntities.GetEntities())
                {
                    if (e is MyDummyPoint && ((int)(((MyDummyPoint)e).DummyFlags & CommonLIB.AppCode.ObjectBuilders.SubObjects.MyDummyPointFlags.VOXEL_HAND)) > 0)
                    {
                        e.WorldMatrix = MyEditorVoxelHand.UpdateShapePosition();
                        MyEditorVoxelHand.DetachedVoxelHand = (MyDummyPoint)e;                        
                    }
                }

                if (MyEditorVoxelHand.DetachedVoxelHand == null)
                {
                    var ob = MinerWars.CommonLIB.AppCode.ObjectBuilders.MyMwcObjectBuilder_Base.CreateNewObject(MinerWars.CommonLIB.AppCode.ObjectBuilders.MyMwcObjectBuilderTypeEnum.DummyPoint, null) as MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.MyMwcObjectBuilder_DummyPoint;
                    MyDummyPoint voxelHand = new MyDummyPoint();
                    voxelHand.Init(null, ob, Matrix.Identity);
                    voxelHand.DummyFlags |= CommonLIB.AppCode.ObjectBuilders.SubObjects.MyDummyPointFlags.VOXEL_HAND;
                    voxelHand.Size = new Vector3(20, 20, 20);
                    voxelHand.Save = false;
                    voxelHand.WorldMatrix = MyEditorVoxelHand.UpdateShapePosition();
                    MyEntities.Add(voxelHand);
                    MyEditorVoxelHand.DetachedVoxelHand = voxelHand;
                }

                
                MyEditorVoxelHand.DetachedVoxelHand.Enabled = true;
                MyEditorGizmo.SelectedEntities.Clear();
                MyEditorGizmo.SelectedEntities.Add(MyEditorVoxelHand.DetachedVoxelHand);

                m_detachLabel.Text = MyTextsWrapperEnum.AttachVoxelHand;
            }
            else
            {
                if (MyEditorVoxelHand.DetachedVoxelHand != null)
                    MyEditorVoxelHand.DetachedVoxelHand.MarkForClose();
                MyEditorVoxelHand.DetachedVoxelHand = null;
                MyEditorGizmo.SelectedEntities.Clear();
                m_detachLabel.Text = MyTextsWrapperEnum.DetachVoxelHand;
            }
        }


        private void LoadSortedControlsByPriority()
        {
            m_sortedControlsByPriority  = new List<MyGuiControlBase>();
            m_sortedControlsByPriority.AddRange(m_controls.Where(x => x is MyGuiControlCombobox));
            m_sortedControlsByPriority.AddRange(m_controls.Where(x => x is MyGuiControlRadioButton));
            m_sortedControlsByPriority.AddRange(m_controls.Where(x => x is MyGuiControlSlider));
            m_sortedControlsByPriority.AddRange(m_controls.Where(x => x is MyGuiControlCheckbox));
        }
               /*
        private void OnVoxelHandSizeChanged(float newSize)
        {
            m_sizeChangedFromEditor = true;
            m_voxelShapeSizeSlider.SetValue(newSize);
        }        */

        private void OnVoxelHandDistanceChanged(float newDistance)
        {
            m_distanceChangedFromEditor = true;
            m_voxelShapeDistanceSlider.SetValue(newDistance);
        }

        private void OnSizeSliderChange(MyGuiControlSlider sliderSender) 
        {
            int index = (int)sliderSender.UserData;
            MyEditorVoxelHand.VoxelHandShape.SetPropertyValue(index, sliderSender.GetValue());

            if (m_sizeChangedFromEditor)
                m_sizeChangedFromEditor = false;
            else
                UpdateVoxelHandProperties();
        }

        private void OnDistanceSliderChange(MyGuiControlSlider sliderSender)
        {
            if (sliderSender == m_voxelShapeDistanceSlider)
            {
                if (m_distanceChangedFromEditor)
                    m_distanceChangedFromEditor = false;
                else
                    UpdateVoxelHandProperties();
            }
        }

        private void OnRadioButtonSelectedChange(MyGuiControlRadioButtonGroup sender)
        {
            UpdateVoxelHandProperties();
            RecreateDynamicProperties();
        }

        private void OnComboboxItemSelect()
        {
            if (m_canSelect)
            {
                UpdateVoxelHandProperties();
            }
        }

        private void OnCheckboxChecked(MyGuiControlCheckbox sender)
        {
            if (sender.Checked)
            {
                // projected
                m_voxelShapeDistanceSlider.SetBounds(MyVoxelConstants.MIN_PROJECTED_VOXEL_HAND_OFFSET, MyVoxelConstants.MAX_PROJECTED_VOXEL_HAND_OFFSET);
                OnVoxelHandDistanceChanged(MyVoxelConstants.DEFAULT_PROJECTED_VOXEL_HAND_OFFSET);
                m_voxelShapeDistanceLabel.UpdateText(MyTextsWrapper.Get(MyTextsWrapperEnum.EditVoxelHandShapeOffset).ToString());
            }
            else
            {
                // not projected
                m_voxelShapeDistanceSlider.SetBounds(MyVoxelConstants.MIN_VOXEL_HAND_DISTANCE, MyVoxelConstants.MAX_VOXEL_HAND_DISTANCE);
                OnVoxelHandDistanceChanged(MyVoxelConstants.DEFAULT_VOXEL_HAND_DISTANCE);
                m_voxelShapeDistanceLabel.UpdateText(MyTextsWrapper.Get(MyTextsWrapperEnum.EditVoxelHandShapeDistance).ToString());
            }

            UpdateVoxelHandProperties();
        }

        private void UpdateVoxelHandProperties()
        {
            MyVoxelHandShapeType voxelHandShapeType = (MyVoxelHandShapeType)m_voxelShapeTypeRadioButtonGroup.GetSelectedKey().Value;
            MyMwcVoxelHandModeTypeEnum modeType = (MyMwcVoxelHandModeTypeEnum)m_voxelShapeModeRadioButtonGroup.GetSelectedKey().Value;
            MyMwcVoxelMaterialsEnum? materialEnum = (MyMwcVoxelMaterialsEnum)m_voxelShapeMaterialCombobox.GetSelectedKey();
            MyEditorVoxelHand.SetVoxelProperties(voxelHandShapeType, m_voxelShapeDistanceSlider.GetValue(), modeType, materialEnum, m_isProjectedCheckbox.Checked, m_isProjectedOnWaypointCheckbox.Checked);
        }        

        #region Overriden methods
        public override void Draw()
        {
           // LoadControls();
            if (Visible)
            {
                foreach (MyGuiControlBase control in m_controls)
                {
                    control.Draw();
                }
            }
        }

        public override void Update()
        {
            foreach (MyGuiControlBase control in m_controls)
            {
                control.Update();
            }
        }

        public override bool HandleInput(MyGuiInput input, bool hasKeyboardActiveControl, bool hasKeyboardActiveControlPrevious, bool receivedFocusInThisUpdate)
        {
            bool inputCaptured = base.HandleInput(input, hasKeyboardActiveControl, hasKeyboardActiveControlPrevious, receivedFocusInThisUpdate);
            if (Visible /*&& !inputCaptured && CheckMouseOver()*/)
            {                                
                foreach (MyGuiControlBase control in m_sortedControlsByPriority)
                {
                    if (control.HandleInput(input, false, false, receivedFocusInThisUpdate))
                    {
                        inputCaptured = true;
                        break;
                    }
                } 
                //Always steal input if clicked on voxel hand control area
                return IsMouseOver() && input.IsAnyMousePress();
            }
            return false;
        }
                     /*
        public override bool ContainsMouse()
        {
            foreach (MyGuiControlBase myGuiControlBase in m_sortedControlsByPriority)
                if(myGuiControlBase.ContainsMouse())
                    return true;
            return false;
        }

        protected override bool CheckMouseOver()
        {
            foreach (MyGuiControlBase myGuiControlBase in m_sortedControlsByPriority)
                if (myGuiControlBase.IsMouseOver())
                    return true;
            return false;
        }              */

        public override void SetPosition(Vector2 position)
        {
            Vector2 d = position - m_position;
            foreach (var control in m_controls)
                control.SetPosition(control.GetPosition() + d);
            m_position = position;
        }
        #endregion
    }
}
