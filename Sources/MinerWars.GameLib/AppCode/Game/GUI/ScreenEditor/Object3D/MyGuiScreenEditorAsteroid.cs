using System;
using MinerWars.AppCode.Game.Utils;
using MinerWarsMath;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.GUI.Helpers;
using MinerWars.AppCode.Game.Editor;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.AppCode.Game.Entities;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Voxels;
using MinerWars.CommonLIB.AppCode.Utils;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace MinerWars.AppCode.Game.GUI.ScreenEditor.Object3D
{
    class MyGuiScreenEditorAsteroid : MyGuiScreenEditorObject3DBase
    {
        MyGuiControlLabel m_selectAsteroidLabel;
        MyGuiControlLabel m_selectMaterialLabel;
        MyGuiControlCombobox m_selectAsteroidTypeCombobox;
        MyGuiControlCombobox m_selectVoxelMapCombobox;
        MyGuiControlCombobox m_selectVoxelMapMaterialCombobox;
        MyGuiControlTextbox m_positionX;
        MyGuiControlTextbox m_positionY;
        MyGuiControlTextbox m_positionZ;
        MyGuiControlCheckbox m_changeMaterial;

        public MyGuiScreenEditorAsteroid(Vector2? screenPosition)
            : base(new Vector2(0.5f, 0.5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, null, MyTextsWrapperEnum.CreateAsteroid, screenPosition)
        {
            Init();
        }

        /// <summary>
        /// asteroid has to be either MyVoxelMap or MyStaticAsteroid.
        /// </summary>
        /// <param name="asteroid">Has to be either MyVoxelMap or MyStaticAsteroid!</param>
        public MyGuiScreenEditorAsteroid(MyEntity asteroid)
            : base(asteroid, new Vector2(0.5f, 0.5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, null, MyTextsWrapperEnum.EditAsteroid)
        {
            Init();
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenEditorAsteroid";
        }

        void Init()
        {
            m_enableBackgroundFade = true;
            m_size = new Vector2(0.92f, 0.95f);

            // Add screen title
            AddCaption();

            Vector2 originDelta = new Vector2(0.02f, 0);
            Vector2 controlsOriginLeft = GetControlsOriginLeftFromScreenSize() + originDelta;
            Vector2 controlsOriginRight = GetControlsOriginRightFromScreenSize() + originDelta;

            // Decide if screen is for editing existing phys object or adding new
            if (HasEntity())
            {
                //controlsOriginLeft.X += 0.5f * MyGuiConstants.COMBOBOX_LONGMEDIUM_SIZE.X;
                controlsOriginLeft.Y += 0.075f;

                //AddEditPositionControls();                
                AddVoxelMaterialCombobox(
                    controlsOriginLeft + new Vector2(0.1f,-0.02f),
                    controlsOriginLeft + new Vector2(MyGuiConstants.COMBOBOX_LONGMEDIUM_SIZE.X / 2.0f, 0) + 2 * CONTROLS_DELTA + new Vector2(0.1f,-0.065f),
                    m_entity.VoxelMaterial);

                if (this.m_entity is MyVoxelMap)
                {
                    Vector2 buttonDelta = new Vector2(-0.15f, m_size.Value.Y / 2.0f - MyGuiConstants.MESSAGE_BOX_BORDER_AREA_Y - MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE.Y / 2.0f - 0.085f);
                    Controls.Add(new MyGuiControlButton(this, new Vector2(buttonDelta.X, buttonDelta.Y), new Vector2(0.25f, 0.0475f), MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                        MyTextsWrapperEnum.RemoveVoxels, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnClearClick, MyGuiControlButtonTextAlignment.CENTERED, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));

                    buttonDelta.X = -buttonDelta.X;
                    Controls.Add(new MyGuiControlButton(this, new Vector2(buttonDelta.X, buttonDelta.Y), new Vector2(0.25f, 0.0475f), MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                        MyTextsWrapperEnum.RemoveAllVoxelHands, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnClearVoxelHands, MyGuiControlButtonTextAlignment.CENTERED, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));
                }

                Vector2 controlsColumn2OriginLabel = new Vector2(controlsOriginLeft.X + MyGuiConstants.COMBOBOX_LONGMEDIUM_SIZE.X / 2 + MyGuiConstants.TEXTBOX_MEDIUM_SIZE.X + 0.03f, -m_size.Value.Y / 2.0f + 0.04f);
                Vector2 controlsColumn2Origin = new Vector2(controlsColumn2OriginLabel.X + 0.02f, -m_size.Value.Y / 2.0f + 0.04f);

                m_positionX = new MyGuiControlTextbox(this, controlsColumn2Origin + 3 * CONTROLS_DELTA + new Vector2(MyGuiConstants.TEXTBOX_MEDIUM_SIZE.X / 2.0f, 0), MyGuiControlPreDefinedSize.MEDIUM, "", 20, MyGuiConstants.TEXTBOX_BACKGROUND_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiControlTextboxType.NORMAL);
                m_positionY = new MyGuiControlTextbox(this, controlsColumn2Origin + 4 * CONTROLS_DELTA + new Vector2(MyGuiConstants.TEXTBOX_MEDIUM_SIZE.X / 2.0f, 0), MyGuiControlPreDefinedSize.MEDIUM, "", 20, MyGuiConstants.TEXTBOX_BACKGROUND_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiControlTextboxType.NORMAL);
                m_positionZ = new MyGuiControlTextbox(this, controlsColumn2Origin + 5 * CONTROLS_DELTA + new Vector2(MyGuiConstants.TEXTBOX_MEDIUM_SIZE.X / 2.0f, 0), MyGuiControlPreDefinedSize.MEDIUM, "", 20, MyGuiConstants.TEXTBOX_BACKGROUND_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiControlTextboxType.NORMAL);

                Controls.Add(new MyGuiControlLabel(this, controlsColumn2OriginLabel + 3 * CONTROLS_DELTA, null, MyTextsWrapperEnum.X, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
                Controls.Add(new MyGuiControlLabel(this, controlsColumn2OriginLabel + 4 * CONTROLS_DELTA, null, MyTextsWrapperEnum.Y, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
                Controls.Add(new MyGuiControlLabel(this, controlsColumn2OriginLabel + 5 * CONTROLS_DELTA, null, MyTextsWrapperEnum.Z, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));

                Controls.Add(m_positionX);
                Controls.Add(m_positionY);
                Controls.Add(m_positionZ);

                m_positionX.Text = m_entity.WorldMatrix.Translation.X.ToString();
                m_positionY.Text = m_entity.WorldMatrix.Translation.Y.ToString();
                m_positionZ.Text = m_entity.WorldMatrix.Translation.Z.ToString();

                m_changeMaterial = new MyGuiControlCheckbox(this, controlsColumn2OriginLabel + 6 * CONTROLS_DELTA + new Vector2(0.16f, 0), true, MyGuiConstants.CHECKBOX_BACKGROUND_COLOR);
                Controls.Add(new MyGuiControlLabel(this, controlsColumn2OriginLabel + 6 * CONTROLS_DELTA, null, MyTextsWrapperEnum.ChangeMaterial, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
                Controls.Add(m_changeMaterial);

                AddOkAndCancelButtonControls();
            }
            else
            {
                #region Asteroid Type
                //choose asteroid type label
                //Controls.Add(new MyGuiControlLabel(this, controlsOriginLeft + 1 * CONTROLS_DELTA + new Vector2(0, -0.0f), null, MyTextsWrapperEnum.AsteroidType, MyGuiConstants.LABEL_TEXT_COLOR,
                //    MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));

                //COMBOBOX - asteroid type
                m_selectAsteroidTypeCombobox = new MyGuiControlCombobox(this, controlsOriginRight + 1 * CONTROLS_DELTA + new Vector2(- controlsOriginRight.X, -0.045f),
                    MyGuiControlPreDefinedSize.MEDIUM, MyGuiConstants.COMBOBOX_BACKGROUND_COLOR, MyGuiConstants.COMBOBOX_TEXT_SCALE, 2);

                foreach (MyGuiAsteroidTypesEnum enumValue in MyGuiAsteroidHelpers.MyGuiAsteroidTypeEnumValues)
                {
                    MyGuiAsteroidHelper asteroidTypeHelper = MyGuiAsteroidHelpers.GetMyGuiAsteroidTypeHelper(enumValue);
                    m_selectAsteroidTypeCombobox.AddItem((int)enumValue, null, asteroidTypeHelper.Description);
                }

                m_selectAsteroidTypeCombobox.SelectItemByKey(0);
                Controls.Add(m_selectAsteroidTypeCombobox);
                m_selectAsteroidTypeCombobox.OnSelect += OnComboboxAsteroidTypeSelect;
                #endregion

                ReloadControls();

                AddOkAndCancelButtonControls();
            }
        }

        private void OnClearVoxelHands(MyGuiControlButton btn)
        {
            MyGuiManager.AddScreen(
                new MyGuiScreenMessageBox(
                    MyMessageBoxType.MESSAGE,
                    MyTextsWrapperEnum.ClearVoxelHandsWarning,
                    MyTextsWrapperEnum.Warning,
                    MyTextsWrapperEnum.Ok,
                    MyTextsWrapperEnum.Cancel,
                    ClearVoxelHands));
        }

        private void OnClearClick(MyGuiControlButton btn)
        {
            MyGuiManager.AddScreen(
                new MyGuiScreenMessageBox(
                    MyMessageBoxType.MESSAGE,
                    MyTextsWrapperEnum.ClearAsteroidWarning,
                    MyTextsWrapperEnum.Warning,
                    MyTextsWrapperEnum.Ok,
                    MyTextsWrapperEnum.Cancel,
                    ClearAsteroid));
        }

        private void ClearVoxelHands(MyGuiScreenMessageBoxCallbackEnum callbackReturn)
        {
            if (callbackReturn == MyGuiScreenMessageBoxCallbackEnum.YES)
            {
                var voxelMap = m_entity as MyVoxelMap;
                voxelMap.ClearVoxelHands();
                OnOkClick(null);
            }
        }

        private void ClearAsteroid(MyGuiScreenMessageBoxCallbackEnum callbackReturn)
        {
            if (callbackReturn == MyGuiScreenMessageBoxCallbackEnum.YES)
            {
                var voxelMap = m_entity as MyVoxelMap;
                MyMwcVector3Int voxelCoordMin = new MyMwcVector3Int(0, 0, 0);
                MyMwcVector3Int voxelCoordMax = voxelMap.Size;

                //  Fix min and max cell coordinates so they don't overlap the voxelmap
                voxelMap.FixVoxelCoord(ref voxelCoordMin);
                voxelMap.FixVoxelCoord(ref voxelCoordMax);

                MyMwcVector3Int voxelCoord;
                for (voxelCoord.X = voxelCoordMin.X; voxelCoord.X <= voxelCoordMax.X; voxelCoord.X++)
                {
                    for (voxelCoord.Y = voxelCoordMin.Y; voxelCoord.Y <= voxelCoordMax.Y; voxelCoord.Y++)
                    {
                        for (voxelCoord.Z = voxelCoordMin.Z; voxelCoord.Z <= voxelCoordMax.Z; voxelCoord.Z++)
                        {
                            voxelMap.SetVoxelContent(0, ref voxelCoord);
                        }
                    }
                }
                voxelMap.InvalidateCache(voxelCoordMin, voxelCoordMax);
                OnOkClick(null);
            }
        }

        private void OnDoubleClick()
        {
            OnOkClick(null);
        }

        public void ReloadControls()
        {
            Vector2 originDelta = new Vector2(0.02f, 0);
            Vector2 controlsOriginLeft = GetControlsOriginLeftFromScreenSize() + originDelta;

            if (GetAsteroidType() == MyGuiAsteroidTypesEnum.VOXEL)
            {
                #region Voxel File
                //choose asteroid label
                m_selectAsteroidLabel = new MyGuiControlLabel(this, controlsOriginLeft + 2.5f * CONTROLS_DELTA + new Vector2(0.02f, -0.02f), null, MyTextsWrapperEnum.AsteroidName, MyGuiConstants.LABEL_TEXT_COLOR,
                    MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);
                Controls.Add(m_selectAsteroidLabel);

                //COMBOBOX - voxel files
                m_selectVoxelMapCombobox = new MyGuiControlCombobox(this, controlsOriginLeft + 4.5f * CONTROLS_DELTA + new Vector2(MyGuiConstants.COMBOBOX_LONGMEDIUM_SIZE.X / 2.0f + 0.02f, -0.065f), MyGuiControlPreDefinedSize.LONGMEDIUM,
                    MyGuiConstants.COMBOBOX_BACKGROUND_COLOR, MyGuiConstants.COMBOBOX_TEXT_SCALE, 11, true, false, true);

                foreach (MyMwcVoxelFilesEnum enumValue in MyGuiAsteroidHelpers.MyMwcVoxelFilesEnumValues)
                {
                    MyGuiAsteroidHelper voxelFileHelper = MyGuiAsteroidHelpers.GetMyGuiVoxelFileHelper(enumValue);
                    Debug.Assert(voxelFileHelper.Description != null);
                    m_selectVoxelMapCombobox.AddItem((int)enumValue, voxelFileHelper.Icon, voxelFileHelper.Description);
                }

                m_selectVoxelMapCombobox.SortItemsByValueText();
                m_selectVoxelMapCombobox.SelectItemByKey(1);
                m_selectVoxelMapCombobox.OnSelectItemDoubleClick += OnDoubleClick;
                Controls.Add(m_selectVoxelMapCombobox);
                #endregion

                #region Voxel Material
                Vector2 delta = new Vector2(m_selectVoxelMapCombobox.GetSize().Value.X + 0.02f, 0);
                AddVoxelMaterialCombobox(
                    controlsOriginLeft + 2.5f * CONTROLS_DELTA + delta + new Vector2(0.14f, -0.02f),
                    controlsOriginLeft + 4.5f * CONTROLS_DELTA + new Vector2(MyGuiConstants.COMBOBOX_LONGMEDIUM_SIZE.X / 2.0f, 0) + delta + new Vector2(0.14f, -0.065f),
                    null);
                #endregion
            }
            else if (GetAsteroidType() == MyGuiAsteroidTypesEnum.STATIC)
            {

                #region Static Asteroid
                m_selectAsteroidLabel = new MyGuiControlLabel(this, controlsOriginLeft + 2.5f * CONTROLS_DELTA + new Vector2(0.02f,-0.02f), null, MyTextsWrapperEnum.AsteroidName, MyGuiConstants.LABEL_TEXT_COLOR,
                    MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);
                Controls.Add(m_selectAsteroidLabel);

                //COMBOBOX - static asteroids
                m_selectVoxelMapCombobox = new MyGuiControlCombobox(this, controlsOriginLeft + 4.5f * CONTROLS_DELTA + new Vector2(MyGuiConstants.COMBOBOX_LONGMEDIUM_SIZE.X / 2.0f + 0.02f, -0.065f), MyGuiControlPreDefinedSize.LONGMEDIUM,
                    MyGuiConstants.COMBOBOX_BACKGROUND_COLOR, MyGuiConstants.COMBOBOX_TEXT_SCALE, 11, true, false, true);

                m_selectVoxelMapCombobox.OnSelectItemDoubleClick += OnDoubleClick;

                foreach (MyMwcObjectBuilder_StaticAsteroid_TypesEnum enumValue in MyGuiAsteroidHelpers.MyMwcStaticAsteroidTypesEnumValues)
                {
                    MyGuiAsteroidHelper staticAsteroidHelper = MyGuiAsteroidHelpers.GetStaticAsteroidTypeHelper(enumValue);

                    if (staticAsteroidHelper != null)
                    {
                        m_selectVoxelMapCombobox.AddItem((int)enumValue, staticAsteroidHelper.Icon, staticAsteroidHelper.Description);
                    }
                }

                m_selectVoxelMapCombobox.SelectItemByIndex(0);
                Controls.Add(m_selectVoxelMapCombobox);

                Vector2 delta = new Vector2(m_selectVoxelMapCombobox.GetSize().Value.X + 0.02f, 0);
                AddVoxelMaterialCombobox(
                    controlsOriginLeft + 2.5f * CONTROLS_DELTA + delta + new Vector2(0.14f, -0.02f),
                    controlsOriginLeft + 4.5f * CONTROLS_DELTA + new Vector2(MyGuiConstants.COMBOBOX_LONGMEDIUM_SIZE.X / 2.0f, 0) + delta + new Vector2(0.14f,-0.065f),
                    null);

                m_selectVoxelMapMaterialCombobox.SelectItemByKey(0); // Select default material

                #endregion
            }
        }

        void AddVoxelMaterialCombobox(Vector2 labelPosition, Vector2 comboboxPosition, MyMwcVoxelMaterialsEnum? selectedMaterial)
        {
            //choose material label            
            m_selectMaterialLabel = new MyGuiControlLabel(this, labelPosition, null, MyTextsWrapperEnum.AsteroidMaterial, MyGuiConstants.LABEL_TEXT_COLOR,
                MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);
            Controls.Add(m_selectMaterialLabel);

            //COMBOBOX - voxel materials enum
            m_selectVoxelMapMaterialCombobox = new MyGuiControlCombobox(this, comboboxPosition, MyGuiControlPreDefinedSize.LONGMEDIUM,
                MyGuiConstants.COMBOBOX_BACKGROUND_COLOR, MyGuiConstants.COMBOBOX_TEXT_SCALE, 11, true, false, true);

            foreach (MyMwcVoxelMaterialsEnum enumValue in MyGuiAsteroidHelpers.MyMwcVoxelMaterialsEnumValues)
            {
                MyGuiVoxelMaterialHelper voxelMaterialHelper = MyGuiAsteroidHelpers.GetMyGuiVoxelMaterialHelper(enumValue);

                if (voxelMaterialHelper != null)
                    m_selectVoxelMapMaterialCombobox.AddItem((int)enumValue, voxelMaterialHelper.Icon, voxelMaterialHelper.Description);
            }
            int selectedIndex;
            if (selectedMaterial == null)
            {
                selectedIndex = 0;
            }
            else
            {
                selectedIndex = (int)selectedMaterial.Value;
            }
            m_selectVoxelMapMaterialCombobox.SelectItemByKey(selectedIndex);
            m_selectVoxelMapMaterialCombobox.OnSelectItemDoubleClick += OnDoubleClick;
            Controls.Add(m_selectVoxelMapMaterialCombobox);
        }

        void OnComboboxAsteroidTypeSelect()
        {
            Controls.Remove(m_selectAsteroidLabel);
            Controls.Remove(m_selectMaterialLabel);
            Controls.Remove(m_selectVoxelMapCombobox);
            Controls.Remove(m_selectVoxelMapMaterialCombobox);
            ReloadControls();
        }

        MyGuiAsteroidTypesEnum GetAsteroidType()
        {
            return (MyGuiAsteroidTypesEnum)Enum.ToObject(typeof(MyGuiAsteroidTypesEnum), m_selectAsteroidTypeCombobox.GetSelectedKey());
        }

        public override void OnOkClick(MyGuiControlButton sender)
        {
            base.OnOkClick(sender);

            if (HasEntity())
            {
                if (m_changeMaterial.Checked)
                {
                    m_entity.VoxelMaterial = ((MyMwcVoxelMaterialsEnum)m_selectVoxelMapMaterialCombobox.GetSelectedKey());
                }
                
                float x,y,z;
                if (float.TryParse(m_positionX.Text, out x) && float.TryParse(m_positionY.Text, out y) && float.TryParse(m_positionZ.Text, out z))
	            {
                    m_entity.SetPosition(new Vector3(x, y, z));
	            }
            }
            else
            {
                if (GetAsteroidType() == MyGuiAsteroidTypesEnum.VOXEL)
                {
                    MyMwcObjectBuilder_SmallShip_TypesEnum shipType = (MyMwcObjectBuilder_SmallShip_TypesEnum)
                        Enum.ToObject(typeof(MyMwcObjectBuilder_SmallShip_TypesEnum), m_selectVoxelMapCombobox.GetSelectedKey());
                    MyMwcVoxelFilesEnum voxelFileEnum = (MyMwcVoxelFilesEnum)
                        Enum.ToObject(typeof(MyMwcVoxelFilesEnum), m_selectVoxelMapCombobox.GetSelectedKey());
                    MyMwcVoxelMaterialsEnum materialEnum = (MyMwcVoxelMaterialsEnum)
                        Enum.ToObject(typeof(MyMwcVoxelMaterialsEnum), m_selectVoxelMapMaterialCombobox.GetSelectedKey());

                    MyMwcObjectBuilder_VoxelMap voxelMapBuilder = MyMwcObjectBuilder_Base.CreateNewObject(MyMwcObjectBuilderTypeEnum.VoxelMap, null) as MyMwcObjectBuilder_VoxelMap;
                    voxelMapBuilder.VoxelMaterial = materialEnum;
                    voxelMapBuilder.VoxelFile = voxelFileEnum;
                    MyEditor.Static.CreateFromObjectBuilder(voxelMapBuilder, Matrix.CreateWorld(m_newObjectPosition, Vector3.Forward, Vector3.Up), m_screenPosition);
                }
                else if (GetAsteroidType() == MyGuiAsteroidTypesEnum.STATIC)
                {
                    MyMwcObjectBuilder_StaticAsteroid_TypesEnum staticAsteroidType = (MyMwcObjectBuilder_StaticAsteroid_TypesEnum)
                        Enum.ToObject(typeof(MyMwcObjectBuilder_StaticAsteroid_TypesEnum), m_selectVoxelMapCombobox.GetSelectedKey());

                    MyMwcVoxelMaterialsEnum? materialEnum = null;
                    int materialKey = m_selectVoxelMapMaterialCombobox.GetSelectedKey();
                    if (materialKey != -1)
                    {
                        materialEnum = (MyMwcVoxelMaterialsEnum)Enum.ToObject(typeof(MyMwcVoxelMaterialsEnum), materialKey);
                    }

                    MyMwcObjectBuilder_StaticAsteroid staticAsteroidBuilder = new MyMwcObjectBuilder_StaticAsteroid(staticAsteroidType, materialEnum);
                    MyEditor.Static.CreateFromObjectBuilder(staticAsteroidBuilder, Matrix.CreateWorld(m_newObjectPosition, Vector3.Forward, Vector3.Up), m_screenPosition);
                }
            }

            MyGuiManager.CloseAllScreensExcept(MyGuiScreenGamePlay.Static);

        }

    }
}
