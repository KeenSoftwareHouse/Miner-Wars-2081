using System;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Editor;
using MinerWarsMath;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Entities.InfluenceSpheres;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.InfluenceSpheres;
using MinerWars.CommonLIB.AppCode.Utils;
using System.Text;
using MinerWars.AppCode.Game.Entities;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.AppCode.Game.TransparentGeometry.Particles;
using MinerWars.AppCode.Game.GUI.RichControls;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.AppCode.Game.GUI.Helpers;
using MinerWars.AppCode.Game.World.Global;

namespace MinerWars.AppCode.Game.GUI.ScreenEditor.Object3D
{
    // This screen is used for adding new DUST influence sphere into world - you choose color and opacity and how big it should be. It consists of 2 spheres - INNER, where the color
    // remains same as selected on this screen and - OUTER, where color is lowering based on the distance from INNER sphere using interpolation.
    // Dust spheres are interpolating their colors between themselves
    class MyGuiScreenEditorDummyPoint : MyGuiScreenEditorObject3DBase
    {
        MyGuiControlSize m_widthSize;
        MyGuiControlSize m_heightSize;
        MyGuiControlSize m_depthSize;

        MyGuiControlCombobox m_typeComboBox;

        MyGuiControlCheckbox m_colorArea;
        MyGuiControlCheckbox m_playerStart;
        MyGuiControlCheckbox m_mothershipStart;
        MyGuiControlCheckbox m_detector;
        MyGuiControlCheckbox m_sideMission;
        MyGuiControlCheckbox m_safeArea;
        MyGuiControlCheckbox m_particleEffect;
        MyGuiControlCheckbox m_survivePrefabDestruction;
        MyGuiControlCheckbox m_respawnPoint;
        MyGuiControlCheckbox m_enabled;
        MyGuiControlCheckbox m_textureQuad;
        MyGuiControlCheckbox m_note;

        MyGuiControlLabel m_colorAreaLabel;
        MyGuiControlLabel m_playerStartLabel;
        MyGuiControlLabel m_mothershipStartLabel;
        MyGuiControlLabel m_detectorLabel;
        MyGuiControlLabel m_sideMissionLabel;
        MyGuiControlLabel m_safeAreaLabel;
        MyGuiControlLabel m_particleEffectLabel;
        MyGuiControlLabel m_survivePrefabDestructionLabel;
        MyGuiControlLabel m_respawnPointLabel;
        MyGuiControlLabel m_enabledLabel;
        MyGuiControlLabel m_textureQuadLabel;
        MyGuiControlLabel m_noteLabel;

        MyGuiControlSlider m_redSlider;
        MyGuiControlSlider m_blueSlider;
        MyGuiControlSlider m_greenSlider;
        MyGuiControlSlider m_alphaSlider;

        MyGuiControlLabel m_redSliderLabel;
        MyGuiControlLabel m_blueSliderLabel;
        MyGuiControlLabel m_greenSliderLabel;
        MyGuiControlLabel m_alphaSliderLabel;
        MyGuiControlLabel m_alphaSliderValueLabel;

        MyGuiControlLabel m_userScaleSliderLabel;
        MyGuiControlSlider m_userScaleSlider;
        MyGuiControlLabel m_userScaleLabel;

        MyGuiControlCombobox m_particleCombo;
        MyGuiControlCombobox m_respawnPointCombo;
        MyGuiControlCombobox m_secretCombo;

        MyGuiControlLabel m_nameLabel;
        MyGuiControlTextbox m_nameTextBox;

        MyDummyPoint DummyPoint { get { return (MyDummyPoint)m_entity; } }

        bool m_canUpdateValues = true;

        public MyGuiScreenEditorDummyPoint(Vector2? screenPosition)
            : base(new Vector2(0.5f, 0.5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, null, MyTextsWrapperEnum.DustInfluenceSphereType, screenPosition)
        {
            Init();
        }

        public MyGuiScreenEditorDummyPoint(MyDummyPoint dummyPoint)
            : base(dummyPoint, new Vector2(0.5f, 0.5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, null, MyTextsWrapperEnum.DummyPoint)
        {
            Init();
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenEditorDummyPoint";
        }

        void Init()
        {
            m_enableBackgroundFade = true;
            m_size = new Vector2(0.8f, 0.2f + (12 * MyGuiConstants.MENU_BUTTONS_POSITION_DELTA.Y));
            Vector2 controlsOriginLeft = GetControlsOriginLeftFromScreenSize() + new Vector2(0.05f, 2 * CONTROLS_DELTA.Y + 0.03f);
            Vector2 sliderOffset = new Vector2(0.25f, 0f);

            // First create controls
            CreateControls(controlsOriginLeft, sliderOffset);

            //m_size = new Vector2(0.7f, 0.15f + (11 * MyGuiConstants.MENU_BUTTONS_POSITION_DELTA.Y));

            // Add screen title
            AddCaption(new Vector2(0, 0.007f));
            AddOkButtonControl(new Vector2(0, -0.02f));
        }

        void CreateControls(Vector2 controlsOrigin, Vector2 sliderOffset)
        {            
            m_nameLabel = new MyGuiControlLabel(this, controlsOrigin - new Vector2(0, 2 * CONTROLS_DELTA.Y), null, MyTextsWrapperEnum.Name, 
                MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);

            m_nameTextBox = new MyGuiControlTextbox(this, controlsOrigin - new Vector2(-0.2f, 2 * CONTROLS_DELTA.Y), MyGuiControlPreDefinedSize.MEDIUM,
                DummyPoint.Name ?? String.Empty, 512, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiControlTextboxType.NORMAL);
            m_nameTextBox.TextChanged = OnTextChange;

            Controls.Add(m_nameLabel);
            Controls.Add(m_nameTextBox);

            m_typeComboBox = new MyGuiControlCombobox(this, controlsOrigin - new Vector2(-0.25f, CONTROLS_DELTA.Y), MyGuiControlPreDefinedSize.MEDIUM, MyGuiConstants.COMBOBOX_BACKGROUND_COLOR, MyGuiConstants.COMBOBOX_TEXT_SCALE);
            m_typeComboBox.AddItem(0, new StringBuilder("Box"));
            m_typeComboBox.AddItem(1, new StringBuilder("Sphere"));
            m_typeComboBox.OnSelect += new MyGuiControlCombobox.OnComboBoxSelectCallback(m_typeComboBox_OnSelect);
            Controls.Add(m_typeComboBox);
            MyGuiControlLabel typeLabel = new MyGuiControlLabel(this, controlsOrigin - new Vector2(-0.0f, CONTROLS_DELTA.Y), null, MyTextsWrapperEnum.Width, MyGuiConstants.LABEL_TEXT_COLOR,
                MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);
            typeLabel.UpdateText("Type");
            Controls.Add(typeLabel);

            float checkBoxOffset = 0.507f;
            //Enabled
            Controls.Add(new MyGuiControlLabel(this, controlsOrigin - new Vector2(-checkBoxOffset, 2 * CONTROLS_DELTA.Y), null, MyTextsWrapperEnum.Enabled, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            m_enabled = new MyGuiControlCheckbox(this, controlsOrigin - new Vector2(-checkBoxOffset, 2 * CONTROLS_DELTA.Y) + new Vector2(0.1f, 0f), false, MyGuiConstants.CHECKBOX_BACKGROUND_COLOR);
            m_enabled.OnCheck += OnEnabledChange;
            Controls.Add(m_enabled);

            // Active
            AddActivatedCheckbox(controlsOrigin - new Vector2(-checkBoxOffset, CONTROLS_DELTA.Y), DummyPoint.Activated);

            float sliderMax = 50000;

            //Width slider            
            m_widthSize = new MyGuiControlSize(this, controlsOrigin + new Vector2(0.35f, 0f), new Vector2(0.7f, MyGuiConstants.SLIDER_HEIGHT), Vector4.Zero, null, 0f, 0.1f, sliderMax, MyTextsWrapper.Get(MyTextsWrapperEnum.Width), MyGuiSizeEnumFlags.All, sliderOffset.X);
            m_widthSize.OnValueChange += OnWidthChange;
            Controls.Add(m_widthSize);

            //Height slider            
            m_heightSize = new MyGuiControlSize(this, controlsOrigin + new Vector2(0.35f, 0f) + 1 * CONTROLS_DELTA, new Vector2(0.7f, MyGuiConstants.SLIDER_HEIGHT), Vector4.Zero, null, 0f, 0.1f, sliderMax, MyTextsWrapper.Get(MyTextsWrapperEnum.Height), MyGuiSizeEnumFlags.All, sliderOffset.X);
            m_heightSize.OnValueChange += OnHeightChange;
            Controls.Add(m_heightSize);

            //Depth slider            
            m_depthSize = new MyGuiControlSize(this, controlsOrigin + new Vector2(0.35f, 0f) + 2 * CONTROLS_DELTA, new Vector2(0.7f, MyGuiConstants.SLIDER_HEIGHT), Vector4.Zero, null, 0f, 0.1f, sliderMax, MyTextsWrapper.Get(MyTextsWrapperEnum.Depth), MyGuiSizeEnumFlags.All, sliderOffset.X);
            m_depthSize.OnValueChange += OnDepthChange;
            Controls.Add(m_depthSize);

            MyGuiControlLabel idLabel = new MyGuiControlLabel(this, controlsOrigin + 3 * CONTROLS_DELTA, null, MyTextsWrapperEnum.Depth, MyGuiConstants.LABEL_TEXT_COLOR,
                    MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);
            Controls.Add(idLabel);
            idLabel.UpdateText("ID: " + m_entity.EntityId.ToString());

            int controlsDelta = 4;

            // Flags
            m_colorArea = new MyGuiControlCheckbox(this, controlsOrigin + controlsDelta++ * CONTROLS_DELTA, false, MyGuiConstants.RADIOBUTTON_BACKGROUND_COLOR);
            m_colorArea.OnCheck += OnFlagChange;
            m_colorAreaLabel = new MyGuiControlLabel(this, controlsOrigin + new Vector2(MyGuiConstants.RADIOBUTTON_SIZE.X, 0.0f) + 4 * CONTROLS_DELTA, null, MyTextsWrapperEnum.ColorArea,
                MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);
            Controls.Add(m_colorArea);
            Controls.Add(m_colorAreaLabel);

            m_playerStart = new MyGuiControlCheckbox(this, controlsOrigin + controlsDelta * CONTROLS_DELTA, false, MyGuiConstants.RADIOBUTTON_BACKGROUND_COLOR);
            m_playerStart.OnCheck += OnPlayerStartFlagChange;
            m_playerStartLabel = new MyGuiControlLabel(this, controlsOrigin + new Vector2(MyGuiConstants.RADIOBUTTON_SIZE.X, 0.0f) + controlsDelta++ * CONTROLS_DELTA, null, MyTextsWrapperEnum.PlayerStart,
                MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);
            Controls.Add(m_playerStart);
            Controls.Add(m_playerStartLabel);

            m_mothershipStart = new MyGuiControlCheckbox(this, controlsOrigin + controlsDelta * CONTROLS_DELTA, false, MyGuiConstants.RADIOBUTTON_BACKGROUND_COLOR);
            m_mothershipStart.OnCheck += OnMothershipStartFlagChange;
            m_mothershipStartLabel = new MyGuiControlLabel(this, controlsOrigin + new Vector2(MyGuiConstants.RADIOBUTTON_SIZE.X, 0.0f) + controlsDelta++ * CONTROLS_DELTA, null, MyTextsWrapperEnum.MothershipStart,
                MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);
            Controls.Add(m_mothershipStart);
            Controls.Add(m_mothershipStartLabel);

            m_detector = new MyGuiControlCheckbox(this, controlsOrigin + controlsDelta * CONTROLS_DELTA, false, MyGuiConstants.RADIOBUTTON_BACKGROUND_COLOR);
            m_detector.OnCheck += OnFlagChange;
            m_detectorLabel = new MyGuiControlLabel(this, controlsOrigin + new Vector2(MyGuiConstants.RADIOBUTTON_SIZE.X, 0.0f) + controlsDelta++ * CONTROLS_DELTA, null, MyTextsWrapperEnum.Detector,
                MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);
            Controls.Add(m_detector);
            Controls.Add(m_detectorLabel);

            m_sideMission = new MyGuiControlCheckbox(this, controlsOrigin + controlsDelta * CONTROLS_DELTA, false, MyGuiConstants.RADIOBUTTON_BACKGROUND_COLOR);
            m_sideMission.OnCheck += OnFlagChange;
            m_sideMissionLabel = new MyGuiControlLabel(this, controlsOrigin + new Vector2(MyGuiConstants.RADIOBUTTON_SIZE.X, 0.0f) + controlsDelta++ * CONTROLS_DELTA, null, MyTextsWrapperEnum.SideMission,
                MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);
            Controls.Add(m_sideMission);
            Controls.Add(m_sideMissionLabel);

            m_particleEffect = new MyGuiControlCheckbox(this, controlsOrigin + controlsDelta * CONTROLS_DELTA, false, MyGuiConstants.RADIOBUTTON_BACKGROUND_COLOR);
            m_particleEffect.OnCheck += OnFlagChange;
            m_particleEffectLabel = new MyGuiControlLabel(this, controlsOrigin + new Vector2(MyGuiConstants.RADIOBUTTON_SIZE.X, 0.0f) + controlsDelta++ * CONTROLS_DELTA, null, MyTextsWrapperEnum.ParticleEffect,
                MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);
            Controls.Add(m_particleEffectLabel);
            Controls.Add(m_particleEffect);

            m_respawnPoint = new MyGuiControlCheckbox(this, controlsOrigin + controlsDelta * CONTROLS_DELTA, false, MyGuiConstants.RADIOBUTTON_BACKGROUND_COLOR);
            m_respawnPoint.OnCheck += OnFlagChange;
            m_respawnPointLabel = new MyGuiControlLabel(this, controlsOrigin + new Vector2(MyGuiConstants.RADIOBUTTON_SIZE.X, 0.0f) + controlsDelta++ * CONTROLS_DELTA, null, MyTextsWrapperEnum.RespawnPoint,
                MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);
            Controls.Add(m_respawnPointLabel);
            Controls.Add(m_respawnPoint);

            m_safeArea = new MyGuiControlCheckbox(this, controlsOrigin + controlsDelta * CONTROLS_DELTA, false, MyGuiConstants.RADIOBUTTON_BACKGROUND_COLOR);
            m_safeArea.OnCheck += OnFlagChange;
            m_safeAreaLabel = new MyGuiControlLabel(this, controlsOrigin + new Vector2(MyGuiConstants.RADIOBUTTON_SIZE.X, 0.0f) + controlsDelta++ * CONTROLS_DELTA, null, MyTextsWrapperEnum.SafeArea,
                MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);
            Controls.Add(m_safeArea);
            Controls.Add(m_safeAreaLabel);

            m_survivePrefabDestruction = new MyGuiControlCheckbox(this, controlsOrigin + controlsDelta * CONTROLS_DELTA, false, MyGuiConstants.RADIOBUTTON_BACKGROUND_COLOR);
            m_survivePrefabDestruction.OnCheck += OnFlagChange;
            m_survivePrefabDestructionLabel = new MyGuiControlLabel(this, controlsOrigin + new Vector2(MyGuiConstants.RADIOBUTTON_SIZE.X, 0.0f) + controlsDelta++ * CONTROLS_DELTA, null, MyTextsWrapperEnum.SurvivePrefabDestruction,
                MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);
            Controls.Add(m_survivePrefabDestructionLabel);
            Controls.Add(m_survivePrefabDestruction);

            m_textureQuad = new MyGuiControlCheckbox(this, controlsOrigin + controlsDelta * CONTROLS_DELTA, false, MyGuiConstants.RADIOBUTTON_BACKGROUND_COLOR);
            m_textureQuad.OnCheck += OnFlagChange;
            m_textureQuadLabel = new MyGuiControlLabel(this, controlsOrigin + new Vector2(MyGuiConstants.RADIOBUTTON_SIZE.X, 0.0f) + controlsDelta++ * CONTROLS_DELTA, null, MyTextsWrapperEnum.TextureQuad,
                MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);
            Controls.Add(m_textureQuad);
            Controls.Add(m_textureQuadLabel);

            m_note = new MyGuiControlCheckbox(this, controlsOrigin + controlsDelta * CONTROLS_DELTA, false, MyGuiConstants.RADIOBUTTON_BACKGROUND_COLOR);
            m_note.OnCheck += OnFlagChange;
            m_noteLabel = new MyGuiControlLabel(this, controlsOrigin + new Vector2(MyGuiConstants.RADIOBUTTON_SIZE.X, 0.0f) + controlsDelta++ * CONTROLS_DELTA, null, MyTextsWrapperEnum.Note,
                MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);
            Controls.Add(m_note);
            Controls.Add(m_noteLabel);

            Vector2 columnOffset = new Vector2(0.15f, 0);
            Vector2 labelColumnOffset = new Vector2(0.18f, 0);
            // Red slider
            m_redSliderLabel = new MyGuiControlLabel(this, labelColumnOffset + controlsOrigin + 4 * CONTROLS_DELTA, null, MyTextsWrapperEnum.Red, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);
            m_redSlider = new MyGuiControlSlider(this, columnOffset + controlsOrigin + sliderOffset + 4 * CONTROLS_DELTA, MyGuiConstants.SLIDER_WIDTH,
               MyEditorConstants.COLOR_COMPONENT_MIN_VALUE, MyEditorConstants.COLOR_COMPONENT_MAX_VALUE, MyGuiConstants.SLIDER_BACKGROUND_COLOR,
                new StringBuilder(), MyGuiConstants.SLIDER_WIDTH_LABEL, 0, MyGuiConstants.LABEL_TEXT_SCALE * 0.85f);
            m_redSlider.OnChange = OnComponentChange;
            Controls.Add(m_redSliderLabel);
            Controls.Add(m_redSlider);

            // Green slider
            m_greenSliderLabel = new MyGuiControlLabel(this, labelColumnOffset + controlsOrigin + 5 * CONTROLS_DELTA, null, MyTextsWrapperEnum.Green, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);
            m_greenSlider = new MyGuiControlSlider(this, columnOffset + controlsOrigin + sliderOffset + 5 * CONTROLS_DELTA, MyGuiConstants.SLIDER_WIDTH,
               MyEditorConstants.COLOR_COMPONENT_MIN_VALUE, MyEditorConstants.COLOR_COMPONENT_MAX_VALUE, MyGuiConstants.SLIDER_BACKGROUND_COLOR,
                new StringBuilder(), MyGuiConstants.SLIDER_WIDTH_LABEL, 0, MyGuiConstants.LABEL_TEXT_SCALE * 0.85f);
            m_greenSlider.OnChange = OnComponentChange;
            Controls.Add(m_greenSliderLabel);
            Controls.Add(m_greenSlider);

            // Blue slider
            m_blueSliderLabel = new MyGuiControlLabel(this, labelColumnOffset + controlsOrigin + 6 * CONTROLS_DELTA, null, MyTextsWrapperEnum.Blue, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);
            m_blueSlider = new MyGuiControlSlider(this, columnOffset + controlsOrigin + sliderOffset + 6 * CONTROLS_DELTA, MyGuiConstants.SLIDER_WIDTH,
               MyEditorConstants.COLOR_COMPONENT_MIN_VALUE, MyEditorConstants.COLOR_COMPONENT_MAX_VALUE, MyGuiConstants.SLIDER_BACKGROUND_COLOR,
                new StringBuilder(), MyGuiConstants.SLIDER_WIDTH_LABEL, 0, MyGuiConstants.LABEL_TEXT_SCALE * 0.85f);
            m_blueSlider.OnChange = OnComponentChange;
            Controls.Add(m_blueSliderLabel);
            Controls.Add(m_blueSlider);

            // Alpha slider
            m_alphaSliderLabel = new MyGuiControlLabel(this, labelColumnOffset + controlsOrigin + 7 * CONTROLS_DELTA, null, MyTextsWrapperEnum.Alpha, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);
            m_alphaSlider = new MyGuiControlSlider(this, columnOffset + controlsOrigin + sliderOffset + 7 * CONTROLS_DELTA, MyGuiConstants.SLIDER_WIDTH,
               MyEditorConstants.COLOR_COMPONENT_MIN_VALUE, MyEditorConstants.COLOR_COMPONENT_MAX_VALUE, MyGuiConstants.SLIDER_BACKGROUND_COLOR,
                new StringBuilder(), MyGuiConstants.SLIDER_WIDTH_LABEL, 0, MyGuiConstants.LABEL_TEXT_SCALE * 0.85f);
            m_alphaSlider.OnChange = OnComponentChange;
            m_alphaSliderValueLabel = new MyGuiControlLabel(this, columnOffset + controlsOrigin + 7 * CONTROLS_DELTA + new Vector2(0.4f, 0), null, new StringBuilder("0"), MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);

            // scale slider
            m_userScaleSliderLabel = new MyGuiControlLabel(this, labelColumnOffset + controlsOrigin + 8 * CONTROLS_DELTA, null, MyTextsWrapperEnum.UserScale, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);
            m_userScaleSlider = new MyGuiControlSlider(this, columnOffset + controlsOrigin + sliderOffset + 8 * CONTROLS_DELTA, MyGuiConstants.SLIDER_WIDTH,
               0.01f, 3.9f, MyGuiConstants.SLIDER_BACKGROUND_COLOR,
                new StringBuilder(), MyGuiConstants.SLIDER_WIDTH_LABEL, 0, MyGuiConstants.LABEL_TEXT_SCALE * 0.85f);
            m_userScaleSlider.OnChange = OnComponentChange;
            m_userScaleLabel = new MyGuiControlLabel(this, columnOffset + controlsOrigin + 8 * CONTROLS_DELTA + new Vector2(0.4f, 0), null, new StringBuilder("0"), MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);

            Controls.Add(m_alphaSliderLabel);
            Controls.Add(m_alphaSliderValueLabel);
            Controls.Add(m_alphaSlider);

            Controls.Add(m_userScaleSliderLabel);
            Controls.Add(m_userScaleSlider);
            Controls.Add(m_userScaleLabel);

            // Particle effect
            //m_particleNameLabel = new MyGuiControlLabel(this, columnOffset + controlsOrigin + 4 * CONTROLS_DELTA, null, MyTextsWrapperEnum.ParticleEffect, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);
            m_particleCombo = new MyGuiControlCombobox(this, columnOffset + controlsOrigin + 9 * CONTROLS_DELTA + sliderOffset, MyGuiControlPreDefinedSize.MEDIUM,
                MyGuiConstants.COMBOBOX_BACKGROUND_COLOR, MyGuiConstants.COMBOBOX_TEXT_SCALE, 5, false, false, false);
            foreach (var p in MyParticlesLibrary.GetParticleEffects())
            {
                m_particleCombo.AddItem(p.GetID(), new StringBuilder(p.Name));
                m_particleCombo.SortItemsByValueText();
            }
            m_particleCombo.OnSelect += new MyGuiControlCombobox.OnComboBoxSelectCallback(m_particleCombo_OnSelect);
            //Controls.Add(m_particleNameLabel);
            Controls.Add(m_particleCombo);

            m_respawnPointCombo = new MyGuiControlCombobox(this, columnOffset + controlsOrigin + 10 * CONTROLS_DELTA + sliderOffset,
                MyGuiControlPreDefinedSize.MEDIUM, MyGuiConstants.COMBOBOX_BACKGROUND_COLOR, MyGuiConstants.COMBOBOX_TEXT_SCALE, 4);
            foreach (MyMwcObjectBuilder_FactionEnum enumValue in MyGuiSmallShipHelpers.MyMwcObjectBuilder_SmallShip_ShipFactionNationalityEnumValues)
            {
                MyGuiHelperBase factionNationalityHelper = MyGuiSmallShipHelpers.GetMyGuiSmallShipFactionNationality(enumValue);
                m_respawnPointCombo.AddItem((int)enumValue, null, factionNationalityHelper.Description);
            }
            m_respawnPointCombo.OnSelect += new MyGuiControlCombobox.OnComboBoxSelectCallback(m_repawnPointCombo_OnSelect);
            Controls.Add(m_respawnPointCombo);

            m_secretCombo = new MyGuiControlCombobox(this, columnOffset + controlsOrigin + 7 * CONTROLS_DELTA + sliderOffset,
                MyGuiControlPreDefinedSize.MEDIUM, MyGuiConstants.COMBOBOX_BACKGROUND_COLOR, MyGuiConstants.COMBOBOX_TEXT_SCALE, 4);

            m_secretCombo.AddItem(0, null, new StringBuilder("No secret"));
            foreach (var room in MySecretRooms.SecretRooms)
            {
                m_secretCombo.AddItem(room.Key, null, new StringBuilder(room.Value));
            }
            m_secretCombo.OnSelect += new MyGuiControlCombobox.OnComboBoxSelectCallback(m_secretCombo_OnSelect);
            Controls.Add(m_secretCombo);

            UpdateValues();
        }

        void m_secretCombo_OnSelect()
        {
            if (DummyPoint.DummyFlags == MyDummyPointFlags.DETECTOR)
            {
                DummyPoint.Argument = m_secretCombo.GetSelectedKey();
            }
        }

        void m_particleCombo_OnSelect()
        {
            DummyPoint.ParticleID = m_particleCombo.GetSelectedKey();
            UpdateValues();
        }

        void m_repawnPointCombo_OnSelect()
        {
            DummyPoint.RespawnPointFaction = (MyMwcObjectBuilder_FactionEnum)m_respawnPointCombo.GetSelectedKey();
        }

        void OnTextChange(object sender, EventArgs args)
        {
            DummyPoint.SetName(m_nameTextBox.Text);
            UpdateValues();
        }

        void OnEnabledChange(MyGuiControlCheckbox sender)
        {
            DummyPoint.Enabled = sender.Checked;
        }

        void OnFlagChange(MyGuiControlCheckbox sender)
        {
            if (!m_canUpdateValues)
                return;

            if (m_colorArea.Checked) DummyPoint.DummyFlags |= MyDummyPointFlags.COLOR_AREA;
            else DummyPoint.DummyFlags &= ~MyDummyPointFlags.COLOR_AREA;

            if (m_detector.Checked) DummyPoint.DummyFlags |= MyDummyPointFlags.DETECTOR;
            else DummyPoint.DummyFlags &= ~MyDummyPointFlags.DETECTOR;

            if (m_playerStart.Checked) DummyPoint.DummyFlags |= MyDummyPointFlags.PLAYER_START;
            else DummyPoint.DummyFlags &= ~MyDummyPointFlags.PLAYER_START;

            if (m_mothershipStart.Checked) DummyPoint.DummyFlags |= MyDummyPointFlags.MOTHERSHIP_START;
            else DummyPoint.DummyFlags &= ~MyDummyPointFlags.MOTHERSHIP_START;

            if (m_sideMission.Checked) DummyPoint.DummyFlags |= MyDummyPointFlags.SIDE_MISSION;
            else DummyPoint.DummyFlags &= ~MyDummyPointFlags.SIDE_MISSION;

            if (m_safeArea.Checked)
            {
                DummyPoint.DummyFlags |= MyDummyPointFlags.SAFE_AREA;
            }
            else DummyPoint.DummyFlags &= ~MyDummyPointFlags.SAFE_AREA;

            if (m_particleEffect.Checked)
            {
                DummyPoint.DummyFlags |= MyDummyPointFlags.PARTICLE;
            }
            else
            {
                DummyPoint.DisableParticleEffect();
            }

            if (m_survivePrefabDestruction.Checked) DummyPoint.DummyFlags |= MyDummyPointFlags.SURIVE_PREFAB_DESTRUCTION;
            else DummyPoint.DummyFlags &= ~MyDummyPointFlags.SURIVE_PREFAB_DESTRUCTION;

            if (m_respawnPoint.Checked) DummyPoint.DummyFlags |= MyDummyPointFlags.RESPAWN_POINT;
            else DummyPoint.DummyFlags &= ~MyDummyPointFlags.RESPAWN_POINT;

            if (m_textureQuad.Checked) DummyPoint.DummyFlags |= MyDummyPointFlags.TEXTURE_QUAD;
            else DummyPoint.DummyFlags &= ~MyDummyPointFlags.TEXTURE_QUAD;

            if (m_note.Checked) DummyPoint.DummyFlags |= MyDummyPointFlags.NOTE;
            else DummyPoint.DummyFlags &= ~MyDummyPointFlags.NOTE;

            if (DummyPoint.DummyFlags == MyDummyPointFlags.DETECTOR)
            {
                DummyPoint.Argument = m_secretCombo.GetSelectedKey();
            }

            UpdateValues();

            if (m_safeArea.Checked)
            {
                m_typeComboBox.SelectItemByKey(1);//select Sphere
            }
        }

        void OnPlayerStartFlagChange(MyGuiControlCheckbox sender)
        {
            if (!m_canUpdateValues)
                return;

            // Make sure there's only one start location per sector
            if (m_playerStart.Checked && (DummyPoint.DummyFlags & MyDummyPointFlags.PLAYER_START) == 0)
            {
                foreach (var entity in MyEntities.GetEntities())
                {
                    var dummyPoint = entity as MyDummyPoint;
                    if (dummyPoint != null && (dummyPoint.DummyFlags & MyDummyPointFlags.PLAYER_START) > 0)
                    {
                        MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, MyTextsWrapperEnum.EntryPointAlreadyDefined, MyTextsWrapperEnum.MessageBoxCaptionError,
                            MyTextsWrapperEnum.Yes, MyTextsWrapperEnum.Cancel, mbReturn =>
                            {
                                if (mbReturn == MyGuiScreenMessageBoxCallbackEnum.YES)
                                {
                                    MyEditorGizmo.ClearSelection();
                                    MyEditorGizmo.AddEntityToSelection(dummyPoint);
                                    CloseScreen();
                                    return;
                                }
                                else
                                {
                                    // We want to enable multiple start dummies
                                    //m_playerStart.Checked = false;
                                }
                            }));
                    }
                }
            }

            if (m_playerStart.Checked)
            {
                DummyPoint.DummyFlags |= MyDummyPointFlags.PLAYER_START;
            }
            else
            {
                DummyPoint.DummyFlags &= ~MyDummyPointFlags.PLAYER_START;
            }

            if (m_playerStart.Checked)
            {
                DummyPoint.DummyFlags |= MyDummyPointFlags.PLAYER_START;
            }
            else
            {
                DummyPoint.DummyFlags &= ~MyDummyPointFlags.PLAYER_START;
            }

            UpdateValues();
        }


        void OnMothershipStartFlagChange(MyGuiControlCheckbox sender)
        {
            if (!m_canUpdateValues)
                return;

            // Make sure there's only one start location per sector
            if (m_mothershipStart.Checked && (DummyPoint.DummyFlags & MyDummyPointFlags.MOTHERSHIP_START) == 0)
            {
                foreach (var entity in MyEntities.GetEntities())
                {
                    var dummyPoint = entity as MyDummyPoint;
                    if (dummyPoint != null && (dummyPoint.DummyFlags & MyDummyPointFlags.MOTHERSHIP_START) > 0)
                    {
                        MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, MyTextsWrapperEnum.EntryPointAlreadyDefined, MyTextsWrapperEnum.MessageBoxCaptionError,
                            MyTextsWrapperEnum.Yes, MyTextsWrapperEnum.Cancel, mbReturn =>
                            {
                                if (mbReturn == MyGuiScreenMessageBoxCallbackEnum.YES)
                                {
                                    MyEditorGizmo.ClearSelection();
                                    MyEditorGizmo.AddEntityToSelection(dummyPoint);
                                    CloseScreen();
                                }
                                else
                                {
                                    m_mothershipStart.Checked = false;
                                }
                            }));
                        return;
                    }
                }
            }

            if (m_mothershipStart.Checked)
            {
                DummyPoint.DummyFlags |= MyDummyPointFlags.MOTHERSHIP_START;
            }
            else
            {
                DummyPoint.DummyFlags &= ~MyDummyPointFlags.MOTHERSHIP_START;
            }

            UpdateValues();
        }

        void OnComponentChange(MyGuiControlSlider sender)
        {
            if (!m_canUpdateValues)
                return;

            DummyPoint.Color = new Vector4(
                m_redSlider.GetValue() / MyEditorConstants.COLOR_COMPONENT_MAX_VALUE,
                m_greenSlider.GetValue() / MyEditorConstants.COLOR_COMPONENT_MAX_VALUE,
                m_blueSlider.GetValue() / MyEditorConstants.COLOR_COMPONENT_MAX_VALUE,
                m_alphaSlider.GetValue() / MyEditorConstants.COLOR_COMPONENT_MAX_VALUE);

            DummyPoint.UserScale = m_userScaleSlider.GetValue();

            UpdateValues();
        }

        void m_typeComboBox_OnSelect()
        {
            if (!m_canUpdateValues)
                return;

            if (m_typeComboBox.GetSelectedKey() == 0)
            {
                DummyPoint.Type = MyDummyPointType.Box;
                DummyPoint.DummyFlags &= ~MyDummyPointFlags.SAFE_AREA;
            }
            else
                DummyPoint.Type = MyDummyPointType.Sphere;

            UpdateValues();
        }

        void OnWidthChange(MyGuiControlBase sender)
        {
            float width = ((MyGuiControlSize)sender).GetValue();
            if (DummyPoint.Type == MyDummyPointType.Box)
            {
                Vector3 size = DummyPoint.Size;
                DummyPoint.Size = new Vector3(width, size.Y, size.Z);
            }
            else
            {
                DummyPoint.Radius = width / 2.0f;
            }
        }

        void OnHeightChange(MyGuiControlBase sender)
        {
            float height = ((MyGuiControlSize)sender).GetValue();
            Vector3 size = DummyPoint.Size;
            DummyPoint.Size = new Vector3(size.X, height, size.Z);
        }

        void OnDepthChange(MyGuiControlBase sender)
        {
            float depth = ((MyGuiControlSize)sender).GetValue();
            Vector3 size = DummyPoint.Size;
            DummyPoint.Size = new Vector3(size.X, size.Y, depth);
        }        

        void UpdateValues()
        {
            if (!m_canUpdateValues)
                return;

            m_canUpdateValues = false;

            m_nameTextBox.Text = DummyPoint.Name ?? String.Empty;

            if (DummyPoint.Type == MyDummyPointType.Box)
            {
                m_heightSize.Visible = true;
                m_depthSize.Visible = true;                

                m_widthSize.SetValue(DummyPoint.Size.X);
                m_heightSize.SetValue(DummyPoint.Size.Y);
                m_depthSize.SetValue(DummyPoint.Size.Z);

                m_widthSize.UpdateDescription("Width");                
            }
            else
            {
                m_heightSize.Visible = false;
                m_depthSize.Visible = false;

                m_widthSize.UpdateDescription("Diameter");

                m_widthSize.SetValue(DummyPoint.Radius * 2);
            }

            if (DummyPoint.Type == MyDummyPointType.Box)
                m_typeComboBox.SelectItemByIndex(0);
            else
                m_typeComboBox.SelectItemByIndex(1);

            m_colorArea.Checked = (DummyPoint.DummyFlags & MyDummyPointFlags.COLOR_AREA) > 0;
            m_playerStart.Checked = (DummyPoint.DummyFlags & MyDummyPointFlags.PLAYER_START) > 0;
            m_mothershipStart.Checked = (DummyPoint.DummyFlags & MyDummyPointFlags.MOTHERSHIP_START) > 0;
            m_detector.Checked = (DummyPoint.DummyFlags & MyDummyPointFlags.DETECTOR) > 0;
            m_sideMission.Checked = (DummyPoint.DummyFlags & MyDummyPointFlags.SIDE_MISSION) > 0;
            m_safeArea.Checked = (DummyPoint.DummyFlags & MyDummyPointFlags.SAFE_AREA) > 0;
            m_particleEffect.Checked = (DummyPoint.DummyFlags & MyDummyPointFlags.PARTICLE) > 0;
            m_survivePrefabDestruction.Checked = (DummyPoint.DummyFlags & MyDummyPointFlags.SURIVE_PREFAB_DESTRUCTION) > 0;
            m_respawnPoint.Checked = (DummyPoint.DummyFlags & MyDummyPointFlags.RESPAWN_POINT) > 0;
            m_enabled.Checked = (DummyPoint.PersistentFlags & CommonLIB.AppCode.ObjectBuilders.MyPersistentEntityFlags.Enabled) > 0;
            m_textureQuad.Checked = (DummyPoint.DummyFlags & MyDummyPointFlags.TEXTURE_QUAD) > 0;
            m_note.Checked = (DummyPoint.DummyFlags & MyDummyPointFlags.NOTE) > 0;

            m_redSlider.Visible = m_colorArea.Checked || m_particleEffect.Checked;
            m_redSliderLabel.Visible = m_colorArea.Checked || m_particleEffect.Checked;
            m_greenSlider.Visible = m_colorArea.Checked || m_particleEffect.Checked;
            m_greenSliderLabel.Visible = m_colorArea.Checked || m_particleEffect.Checked;
            m_blueSlider.Visible = m_colorArea.Checked || m_particleEffect.Checked;
            m_blueSliderLabel.Visible = m_colorArea.Checked || m_particleEffect.Checked;
            m_alphaSlider.Visible = m_colorArea.Checked || m_particleEffect.Checked;
            m_alphaSliderLabel.Visible = m_colorArea.Checked || m_particleEffect.Checked;
            m_alphaSliderValueLabel.Visible = m_colorArea.Checked || m_particleEffect.Checked;
            m_userScaleSliderLabel.Visible = m_particleEffect.Checked;
            m_userScaleSlider.Visible = m_particleEffect.Checked;
            m_userScaleLabel.Visible = m_particleEffect.Checked;

            m_redSlider.SetValue(DummyPoint.Color.X * MyEditorConstants.COLOR_COMPONENT_MAX_VALUE);
            m_greenSlider.SetValue(DummyPoint.Color.Y * MyEditorConstants.COLOR_COMPONENT_MAX_VALUE);
            m_blueSlider.SetValue(DummyPoint.Color.Z * MyEditorConstants.COLOR_COMPONENT_MAX_VALUE);
            m_alphaSlider.SetValue(DummyPoint.Color.W * MyEditorConstants.COLOR_COMPONENT_MAX_VALUE);
            m_alphaSliderValueLabel.UpdateText(new Color(DummyPoint.Color).ToVector4().W.ToString("#,###0.000", System.Globalization.CultureInfo.InvariantCulture));
            m_userScaleSlider.SetValue(DummyPoint.UserScale);
            m_userScaleLabel.UpdateText(DummyPoint.UserScale.ToString("#,###0.000", System.Globalization.CultureInfo.InvariantCulture));

            //m_particleCombo.Visible = m_particleEffect.Checked;
            // m_particleNameLabel.Visible = m_particleEffect.Checked;
            m_particleCombo.SelectItemByKey((int)DummyPoint.ParticleID);
            m_respawnPointCombo.SelectItemByKey((int)DummyPoint.RespawnPointFaction);//hopefuly china

            if (DummyPoint.DummyFlags == MyDummyPointFlags.DETECTOR)
            {
                m_secretCombo.Visible = true;
                m_secretCombo.SelectItemByKey((int)DummyPoint.Argument);
            }
            else
            {
                m_secretCombo.Visible = false;
            }

            m_canUpdateValues = true;
        }


        public override void OnOkClick(MyGuiControlButton sender)
        {
            if (m_respawnPoint.Checked &&  m_respawnPointCombo.GetSelectedKey() == (int)MyMwcObjectBuilder_FactionEnum.None)
            {
                MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, MyTextsWrapperEnum.PleaseSelectValidFaction, MyTextsWrapperEnum.InvalidFaction, MyTextsWrapperEnum.Ok, null));
                return;
            }

            base.OnOkClick(sender);
            DummyPoint.Activate(m_activatedCheckbox.Checked, false);
            // close all opened screens except gameplay
            MyGuiManager.CloseAllScreensExcept(MyGuiScreenGamePlay.Static);
        }

        public override bool Draw(float backgroundFadeAlpha)
        {
            if (base.Draw(backgroundFadeAlpha) == false) return false;

            if (m_colorArea.Checked || m_particleEffect.Checked)
            {
                MyGuiManager.DrawSpriteBatch(MyGuiManager.GetBlankTexture(), GetPosition() + new Vector2(MyGuiConstants.SLIDER_WIDTH / 2 + 0.02f, 0) + m_redSlider.GetPosition(), MyGuiConstants.CHECKBOX_SIZE,
                    new Color(m_redSlider.GetValue() / MyEditorConstants.COLOR_COMPONENT_MAX_VALUE, 0, 0), MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER);
                MyGuiManager.DrawSpriteBatch(MyGuiManager.GetBlankTexture(), GetPosition() + new Vector2(MyGuiConstants.SLIDER_WIDTH / 2 + 0.02f, 0) + m_greenSlider.GetPosition(), MyGuiConstants.CHECKBOX_SIZE,
                    new Color(0, m_greenSlider.GetValue() / MyEditorConstants.COLOR_COMPONENT_MAX_VALUE, 0), MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER);
                MyGuiManager.DrawSpriteBatch(MyGuiManager.GetBlankTexture(), GetPosition() + new Vector2(MyGuiConstants.SLIDER_WIDTH / 2 + 0.02f, 0) + m_blueSlider.GetPosition(), MyGuiConstants.CHECKBOX_SIZE,
                    new Color(0, 0, m_blueSlider.GetValue() / MyEditorConstants.COLOR_COMPONENT_MAX_VALUE), MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER);

                float height = MyGuiConstants.CHECKBOX_SIZE.Y + CONTROLS_DELTA.Y * 2;
                float width = MyGuiConstants.CHECKBOX_SIZE.X / MyGuiConstants.CHECKBOX_SIZE.Y * height;

                MyGuiManager.DrawSpriteBatch(MyGuiManager.GetBlankTexture(),
                    GetPosition() + new Vector2(MyGuiConstants.SLIDER_WIDTH / 2 + 0.04f + width / 2, 0) + m_greenSlider.GetPosition(),
                    new Vector2(width, height),
                    new Color(m_redSlider.GetValue() / 255, m_greenSlider.GetValue() / 255, m_blueSlider.GetValue() / 255),
                    MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER);
            }

            return true;
        }
    }
}
