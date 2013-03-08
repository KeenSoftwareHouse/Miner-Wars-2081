using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Entities.SubObjects;
using MinerWarsMath;
using MinerWars.AppCode.Game.Lights;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Localization;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.Prefabs;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Editor;

namespace MinerWars.AppCode.Game.GUI.Helpers
{
    class MyGuiControlEditLights : MyGuiControlBase
    {
        class MyLightProperties
        {
            public Vector4 Color;
            public Vector3 SpecularColor;
            public float FallOff;
            public float Range;
            public float Intensity;
            public float PointLightOffSet;
            public Vector4 ReflectorColor;
            public float ReflectorFallOff;
            public float ReflectorRange;
            public float ReflectorIntensity;
            public float ReflectorConeDegrees;
            public MyLight.LightTypeEnum LightType;
            public MyPrefabLight PrefabLight;
            public float FlashOffset;

            public MyLightProperties(Vector4 color, Vector3 specularColor, float fallOff, float range, float intensity, float pointLightOffSet, Vector4 reflectorColor,
                float reflectorFallOff, float reflectorRange, float reflectorIntensity, float reflectorConeDegrees, MyLight.LightTypeEnum lightType, MyPrefabLight prefabLight, float flashOffset)
            {
                Color = color;
                SpecularColor = specularColor;
                FallOff = fallOff;
                Range = range;
                Intensity = intensity;
                PointLightOffSet = pointLightOffSet;
                ReflectorColor = reflectorColor;
                ReflectorFallOff = reflectorFallOff;
                ReflectorRange = reflectorRange;
                ReflectorIntensity = reflectorIntensity;
                ReflectorConeDegrees = reflectorConeDegrees;
                LightType = lightType;
                PrefabLight = prefabLight;
                FlashOffset = flashOffset;
            }
        }

        MyGuiControlTabControl m_tabControl;

        MyGuiControlCheckbox m_pointLightCheckbox;
        MyGuiControlCheckbox m_hemisphereLightCheckbox;
        MyGuiControlSlider[] m_pointNormalLightColorSlider;
        MyGuiControlSlider[] m_specularLightColorSlider;
        MyGuiControlSlider m_pointFallOffSlider;
        MyGuiControlSlider m_pointRangeSlider;
        MyGuiControlSlider m_pointIntensitySlider;
        MyGuiControlSlider m_pointOffsetSlider;
        MyGuiControlSlider m_flashOffsetSlider;
        MyGuiControlLabel m_flashOffsetLabel;

        MyGuiControlCheckbox m_spotLightCheckbox;
        MyGuiControlSlider[] m_spotNormalLightColorSlider;
        MyGuiControlSlider m_spotAngleSlider;
        MyGuiControlSlider m_spotIntensitySlider;
        MyGuiControlSlider m_spotFallOffSlider;
        MyGuiControlSlider m_spotRangeSlider;
        MyGuiControlSlider m_spotLightShadowDistance;

        MyGuiControlCombobox m_effectComboBox;

        List<MyPrefabLight> m_prefabLights;
        List<MyLightProperties> m_originalValues;

        List<MyGuiControlBase> m_controls;


        public MyGuiControlEditLights(IMyGuiControlsParent parent, Vector2 position, Vector2 size, Vector4 bgColor)
            : base(parent, position, size, bgColor, null)
        {
            m_controls = new List<MyGuiControlBase>();
            m_prefabLights = new List<MyPrefabLight>();
            m_pointLightControls = new List<MyGuiControlBase>();
            m_spotLightControls = new List<MyGuiControlBase>();

            Init();

            MyEntities.OnEntityRemove += MyEntities_OnEntityRemove;
        }

        public void SetLights(List<MyPrefabLight> lights)
        {
            m_prefabLights = new List<MyPrefabLight>();
            
            if (lights.Count > 0)
            {
                MyPrefabLight myLight = lights[0] as MyPrefabLight;
                InitializeValues(myLight);
            }
            
            m_prefabLights = lights;
            SaveOriginalValues();
        }

        private void SaveOriginalValues()
        {
            m_originalValues = new List<MyLightProperties>(m_prefabLights.Count);
            foreach (MyPrefabLight prefabLight in m_prefabLights)
            {
                MyLight light = prefabLight.GetLight();
                m_originalValues.Add(new MyLightProperties(light.Color, light.SpecularColor, light.Falloff, light.Range, light.Intensity, light.PointLightOffset,
                    light.ReflectorColor, light.ReflectorFalloff, light.ReflectorRange, light.ReflectorIntensity, light.ReflectorConeDegrees, light.LightType, prefabLight, prefabLight.FlashOffset));
            }
        }

        public void RestoreOriginalValues()
        {
            if (m_originalValues != null)
            {
                for (int i = 0; i < m_originalValues.Count; i++)
                {
                    MyLightProperties originalValue = m_originalValues[i];
                    MyLight light = m_prefabLights[i].GetLight();
                    light.Color = originalValue.Color;
                    light.SpecularColor = originalValue.SpecularColor;
                    light.Falloff = originalValue.FallOff;
                    light.Range = originalValue.Range;
                    light.Intensity = originalValue.Intensity;
                    light.PointLightOffset = originalValue.PointLightOffSet;
                    light.ReflectorColor = originalValue.ReflectorColor;
                    light.ReflectorFalloff = originalValue.ReflectorFallOff;
                    light.ReflectorRange = originalValue.ReflectorRange;
                    light.ReflectorIntensity = originalValue.ReflectorIntensity;
                    light.ReflectorConeDegrees = originalValue.ReflectorConeDegrees;
                    light.LightType = originalValue.LightType;

                    m_prefabLights[i].FlashOffset = originalValue.FlashOffset;

                    originalValue.PrefabLight.OnWorldPositionChanged(this);
                }
            }
        }

        private void Init()
        {
            Vector2 controlsOriginLeft = m_position - m_size.Value*0.5f + new Vector2(0.03f, 0.1f);

            m_pointNormalLightColorSlider = new MyGuiControlSlider[3];
            m_specularLightColorSlider = new MyGuiControlSlider[3];
            m_spotNormalLightColorSlider = new MyGuiControlSlider[3];

            CreateControls(controlsOriginLeft);
            AssignCallbacks();
        }

        private void AssignCallbacks()
        {
            m_pointLightCheckbox.OnCheck = OnMyCheckEnable;
            m_hemisphereLightCheckbox.OnCheck = OnMyCheckEnable;
            for (int i = 0; i < 3; i++)
            {
                m_pointNormalLightColorSlider[i].OnChange = OnComponentChange;
            }
            for (int i = 0; i < 3; i++)
            {
                m_specularLightColorSlider[i].OnChange = OnComponentChange;
            }
            m_pointFallOffSlider.OnChange = OnComponentChange;
            m_pointRangeSlider.OnChange = OnComponentChange;
            m_pointIntensitySlider.OnChange = OnComponentChange;
            m_pointOffsetSlider.OnChange = OnComponentChange;

            m_spotLightCheckbox.OnCheck = OnMyCheckEnable;
            for (int i = 0; i < 3; i++)
            {
                m_spotNormalLightColorSlider[i].OnChange = OnComponentChange;
            }
            m_spotAngleSlider.OnChange = OnComponentChange;
            m_spotFallOffSlider.OnChange = OnComponentChange;
            m_spotRangeSlider.OnChange = OnComponentChange;
            m_spotIntensitySlider.OnChange = OnComponentChange;
            m_spotLightShadowDistance.OnChange = OnComponentChange;
            m_flashOffsetSlider.OnChange = OnComponentChange;
        }


        List<MyGuiControlBase> m_pointLightControls;
        List<MyGuiControlBase> m_spotLightControls;

        private void AddControl(bool point, MyGuiControlBase control)
        {
            if (point)
	        {
                m_pointLightControls.Add(control);
	        }
            else
	        {
                m_spotLightControls.Add(control);
	        }

            m_controls.Add(control);
        }

        void CreateControls(Vector2 controlsOrigin)
        {
            var panel = new MyGuiControlPanel(m_parent, m_position, m_size, m_backgroundColor.Value, 2, m_backgroundColor.Value * 2f);
            m_controls.Add(panel);

            float dPos = -1;
            float dPosFalloff;
            MyTextsWrapperEnum[] colorNames = { MyTextsWrapperEnum.Red, MyTextsWrapperEnum.Green, MyTextsWrapperEnum.Blue };

            Vector2 sliderOffset = new Vector2(0.21f, 0f);

            float buttonsY = m_size.Value.Y / 2.0f - MyGuiConstants.MESSAGE_BOX_BORDER_AREA_Y - MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE.Y / 2.0f;

            AddControl(true, new MyGuiControlButton(m_parent, new Vector2(m_position.X, controlsOrigin.Y) + dPos * MyGuiConstants.CONTROLS_DELTA, MyGuiConstants.MAIN_MENU_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.SwitchToSpotLight, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnSpotClick,
                MyGuiControlButtonTextAlignment.CENTERED, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));
            ++dPos;

            m_controls.Add(new MyGuiControlLabel(m_parent, controlsOrigin + dPos * MyGuiConstants.CONTROLS_DELTA, null, MyTextsWrapperEnum.EachlightHasInternallyTwoLightsPointAndSpot, MyGuiConstants.LABEL_TEXT_COLOR,
                MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));

            /*
             * 
             * Point light
             * 
             * 
             */
            // Checkboxes for Disabled/Point/Hemisphere

            Vector2 checkBoxOffset = new Vector2(0.14f, 0);
            dPos++;
            AddControl(true, new MyGuiControlLabel(m_parent, controlsOrigin + dPos * MyGuiConstants.CONTROLS_DELTA, null, new StringBuilder("Point enabled"), MyGuiConstants.LABEL_TEXT_COLOR,
                MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            m_pointLightCheckbox = new MyGuiControlCheckbox(m_parent, (checkBoxOffset + controlsOrigin) + dPos * MyGuiConstants.CONTROLS_DELTA, false, MyGuiConstants.CHECKBOX_BACKGROUND_COLOR);
            AddControl(true, m_pointLightCheckbox);

            //dPos++;
            AddControl(true, new MyGuiControlLabel(m_parent, controlsOrigin + dPos * MyGuiConstants.CONTROLS_DELTA + new Vector2(0.2f, 0), null, new StringBuilder("Hemispheric enabled"), MyGuiConstants.LABEL_TEXT_COLOR,
                MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            m_hemisphereLightCheckbox = new MyGuiControlCheckbox(m_parent, (checkBoxOffset + controlsOrigin) + dPos * MyGuiConstants.CONTROLS_DELTA + new Vector2(0.26f, 0), false, MyGuiConstants.CHECKBOX_BACKGROUND_COLOR);
            AddControl(true, m_hemisphereLightCheckbox);

            //text
            dPos += 0.9f;
            AddControl(true, new MyGuiControlLabel(m_parent, controlsOrigin + dPos * MyGuiConstants.CONTROLS_DELTA, null, MyTextsWrapperEnum.DiffuseColor, MyGuiConstants.LABEL_TEXT_COLOR,
                MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));

            Vector2 buttonsColumn = new Vector2(0.165f, 0.0025f);
            Vector2 copyPasteButtonScale = MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE * new Vector2(0.7f, 0.7f);
            float copyPasteButtonFontScale = MyGuiConstants.BUTTON_TEXT_SCALE * 0.75f;
            Vector2 pasteButtonShift = new Vector2(0.115f, 0f);            

            AddControl(true, new MyGuiControlButton(m_parent, controlsOrigin + dPos * MyGuiConstants.CONTROLS_DELTA + buttonsColumn, copyPasteButtonScale, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.Copy, MyGuiConstants.BUTTON_TEXT_COLOR, copyPasteButtonFontScale, OnCopyPointDiffuseColor,
                MyGuiControlButtonTextAlignment.CENTERED, true, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER, true));

            AddControl(true, new MyGuiControlButton(m_parent, controlsOrigin + dPos * MyGuiConstants.CONTROLS_DELTA + buttonsColumn + pasteButtonShift, copyPasteButtonScale, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.Paste, MyGuiConstants.BUTTON_TEXT_COLOR, copyPasteButtonFontScale, OnPastePointDiffuseColor,
                MyGuiControlButtonTextAlignment.CENTERED, true, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER, true));
            dPos += 0.1f;

            //point color sliders
            for (int i = 0; i < 3; i++)
            {
                dPos += 0.75f;
                AddControl(true, new MyGuiControlLabel(m_parent, controlsOrigin + dPos * MyGuiConstants.CONTROLS_DELTA, null, colorNames[i], MyGuiConstants.LABEL_TEXT_COLOR,
                    MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
                m_pointNormalLightColorSlider[i] = new MyGuiControlSlider(m_parent, (sliderOffset + controlsOrigin) + dPos * MyGuiConstants.CONTROLS_DELTA, MyGuiConstants.SLIDER_WIDTH,
                    MyEditorConstants.COLOR_COMPONENT_MIN_VALUE, MyEditorConstants.COLOR_COMPONENT_MAX_VALUE, MyGuiConstants.SLIDER_BACKGROUND_COLOR,
                    new StringBuilder(), MyGuiConstants.SLIDER_WIDTH_LABEL, 0, MyGuiConstants.LABEL_TEXT_SCALE * 0.85f * 0.75f);
                AddControl(true, m_pointNormalLightColorSlider[i]);
            }

            //text
            dPos += 0.9f;
            AddControl(true, new MyGuiControlLabel(m_parent, controlsOrigin + dPos * MyGuiConstants.CONTROLS_DELTA, null, MyTextsWrapperEnum.SpecularColor, MyGuiConstants.LABEL_TEXT_COLOR,
                MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));

            AddControl(true, new MyGuiControlButton(m_parent, controlsOrigin + dPos * MyGuiConstants.CONTROLS_DELTA + buttonsColumn, copyPasteButtonScale, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.Copy, MyGuiConstants.BUTTON_TEXT_COLOR, copyPasteButtonFontScale, OnCopyPointSpecularColor,
                MyGuiControlButtonTextAlignment.CENTERED, true, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER, true));

            AddControl(true, new MyGuiControlButton(m_parent, controlsOrigin + dPos * MyGuiConstants.CONTROLS_DELTA + buttonsColumn + pasteButtonShift, copyPasteButtonScale, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.Paste, MyGuiConstants.BUTTON_TEXT_COLOR, copyPasteButtonFontScale, OnPastePointSpecularColor,
                MyGuiControlButtonTextAlignment.CENTERED, true, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER, true));
            dPos += 0.1f;

            // specular color sliders
            for (int i = 0; i < 3; i++)
            {
                dPos += 0.75f;
                AddControl(true, new MyGuiControlLabel(m_parent, controlsOrigin + dPos * MyGuiConstants.CONTROLS_DELTA, null, colorNames[i], MyGuiConstants.LABEL_TEXT_COLOR,
                    MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
                m_specularLightColorSlider[i] = new MyGuiControlSlider(m_parent, (sliderOffset + controlsOrigin) + dPos * MyGuiConstants.CONTROLS_DELTA, MyGuiConstants.SLIDER_WIDTH,
                    MyEditorConstants.COLOR_COMPONENT_MIN_VALUE, MyEditorConstants.COLOR_COMPONENT_MAX_VALUE, MyGuiConstants.SLIDER_BACKGROUND_COLOR,
                    new StringBuilder(), MyGuiConstants.SLIDER_WIDTH_LABEL, 0, MyGuiConstants.LABEL_TEXT_SCALE * 0.85f * 0.75f);
                AddControl(true, m_specularLightColorSlider[i]);
            }

            /*
             * Effect 
             */

            //effect
            dPos += 1.25f;
            m_controls.Add( new MyGuiControlLabel(m_parent, controlsOrigin + dPos * MyGuiConstants.CONTROLS_DELTA, null, MyTextsWrapperEnum.Options, MyGuiConstants.LABEL_TEXT_COLOR,
                MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            m_effectComboBox = new MyGuiControlCombobox(m_parent, controlsOrigin + dPos * MyGuiConstants.CONTROLS_DELTA + sliderOffset + new Vector2(0.03f, 0f),
                MyGuiControlPreDefinedSize.MEDIUM, MyGuiConstants.COMBOBOX_BACKGROUND_COLOR, MyGuiConstants.COMBOBOX_TEXT_SCALE);

            foreach (MyLightEffectTypeEnum enumValue in Enum.GetValues(typeof(MyLightEffectTypeEnum)))
            {
                m_effectComboBox.AddItem((int)enumValue, MyPrefabLight.GetStringFromMyLightEffectTypeEnum(enumValue));
            }

            m_effectComboBox.SelectItemByKey(0);
            m_effectComboBox.OnSelect += OnComboBoxChange;
            //m_controls.Add(m_effectComboBox);
            m_controls.Add(m_effectComboBox);

            //falloff slider
            dPos += 1.25f;
            dPosFalloff = dPos;

            AddControl(true, new MyGuiControlLabel(m_parent, controlsOrigin + dPos * MyGuiConstants.CONTROLS_DELTA, null, new StringBuilder("Falloff"), MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            m_pointFallOffSlider = new MyGuiControlSlider(m_parent, (sliderOffset + controlsOrigin) + dPos * MyGuiConstants.CONTROLS_DELTA, MyGuiConstants.SLIDER_WIDTH,
                0.1f, 5.0f, MyGuiConstants.SLIDER_BACKGROUND_COLOR,
                new System.Text.StringBuilder(" {0}"), MyGuiConstants.SLIDER_WIDTH_LABEL + 0.05f, 3, MyGuiConstants.LABEL_TEXT_SCALE);
            AddControl(true, m_pointFallOffSlider);

            //range slider
            dPos++;
            AddControl(true, new MyGuiControlLabel(m_parent, controlsOrigin + dPos * MyGuiConstants.CONTROLS_DELTA, null, new StringBuilder("Range"), MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            m_pointRangeSlider = new MyGuiControlSlider(m_parent, (sliderOffset + controlsOrigin) + dPos * MyGuiConstants.CONTROLS_DELTA, MyGuiConstants.SLIDER_WIDTH,
                0.1f, MyLightsConstants.MAX_POINTLIGHT_RADIUS, MyGuiConstants.SLIDER_BACKGROUND_COLOR,
                new System.Text.StringBuilder(" {0}"), MyGuiConstants.SLIDER_WIDTH_LABEL + 0.05f, 3, MyGuiConstants.LABEL_TEXT_SCALE);
            AddControl(true, m_pointRangeSlider);

            //intensity slider
            dPos++;
            AddControl(true, new MyGuiControlLabel(m_parent, controlsOrigin + dPos * MyGuiConstants.CONTROLS_DELTA, null, new StringBuilder("Intensity"), MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));

            m_pointIntensitySlider = new MyGuiControlSlider(m_parent, (sliderOffset + controlsOrigin) + dPos * MyGuiConstants.CONTROLS_DELTA, MyGuiConstants.SLIDER_WIDTH,
                0.1f, 10.0f, MyGuiConstants.SLIDER_BACKGROUND_COLOR,
                new System.Text.StringBuilder(" {0}"), MyGuiConstants.SLIDER_WIDTH_LABEL + 0.05f, 3, MyGuiConstants.LABEL_TEXT_SCALE);

            AddControl(true, m_pointIntensitySlider);

            // Offset slider
            dPos++;
            AddControl(true, new MyGuiControlLabel(m_parent, controlsOrigin + dPos * MyGuiConstants.CONTROLS_DELTA, null, new StringBuilder("Offset"), MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));

            m_pointOffsetSlider = new MyGuiControlSlider(m_parent, (sliderOffset + controlsOrigin) + dPos * MyGuiConstants.CONTROLS_DELTA, MyGuiConstants.SLIDER_WIDTH,
                -0.5f, 1.5f, MyGuiConstants.SLIDER_BACKGROUND_COLOR,
                new System.Text.StringBuilder(" {0}"), MyGuiConstants.SLIDER_WIDTH_LABEL + 0.05f, 3, MyGuiConstants.LABEL_TEXT_SCALE);

            AddControl(true, m_pointOffsetSlider);


            dPos++;
            AddControl(true, m_flashOffsetLabel = new MyGuiControlLabel(m_parent, controlsOrigin + dPos * MyGuiConstants.CONTROLS_DELTA, null, new StringBuilder("Flash Ofst."), MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));

            m_flashOffsetSlider = new MyGuiControlSlider(m_parent, (sliderOffset + controlsOrigin) + dPos * MyGuiConstants.CONTROLS_DELTA, MyGuiConstants.SLIDER_WIDTH,
                0f, 1f, MyGuiConstants.SLIDER_BACKGROUND_COLOR,
                new System.Text.StringBuilder(" {0}"), MyGuiConstants.SLIDER_WIDTH_LABEL + 0.05f, 3, MyGuiConstants.LABEL_TEXT_SCALE);

            AddControl(true, m_flashOffsetSlider);

            /*
             * 
             * Reflector spot light
             * 
             * 
             */




            dPos = -1;



            AddControl(false, new MyGuiControlButton(m_parent, new Vector2(m_position.X, controlsOrigin.Y) + dPos * MyGuiConstants.CONTROLS_DELTA, MyGuiConstants.MAIN_MENU_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.SwitchToPointLight, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnPointClick,
                MyGuiControlButtonTextAlignment.CENTERED, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));


            dPos = 1;

            AddControl(false, new MyGuiControlLabel(m_parent, controlsOrigin + dPos * MyGuiConstants.CONTROLS_DELTA, null, MyTextsWrapperEnum.Enabled, MyGuiConstants.LABEL_TEXT_COLOR,
    MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            m_spotLightCheckbox = new MyGuiControlCheckbox(m_parent, (checkBoxOffset + controlsOrigin) + dPos * MyGuiConstants.CONTROLS_DELTA, false, MyGuiConstants.CHECKBOX_BACKGROUND_COLOR);
            AddControl(false, m_spotLightCheckbox);





            //text
            dPos += 0.9f;
            AddControl(false, new MyGuiControlLabel(m_parent, controlsOrigin + dPos * MyGuiConstants.CONTROLS_DELTA, null, MyTextsWrapperEnum.DiffuseColor, MyGuiConstants.LABEL_TEXT_COLOR,
                MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));

            AddControl(false, new MyGuiControlButton(m_parent, controlsOrigin + dPos * MyGuiConstants.CONTROLS_DELTA + buttonsColumn, copyPasteButtonScale, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.Copy, MyGuiConstants.BUTTON_TEXT_COLOR, copyPasteButtonFontScale, OnCopySpotDiffuseColor,
                MyGuiControlButtonTextAlignment.CENTERED, true, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER, true));

            AddControl(false, new MyGuiControlButton(m_parent, controlsOrigin + dPos * MyGuiConstants.CONTROLS_DELTA + buttonsColumn + pasteButtonShift, copyPasteButtonScale, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.Paste, MyGuiConstants.BUTTON_TEXT_COLOR, copyPasteButtonFontScale, OnPasteSpotDiffuseColor,
                MyGuiControlButtonTextAlignment.CENTERED, true, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER, true));
            dPos += 0.1f;

            //color sliders
            for (int i = 0; i < 3; i++)
            {
                dPos += 0.75f;
                AddControl(false, new MyGuiControlLabel(m_parent, controlsOrigin + dPos * MyGuiConstants.CONTROLS_DELTA, null, colorNames[i], MyGuiConstants.LABEL_TEXT_COLOR,
                    MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
                m_spotNormalLightColorSlider[i] = new MyGuiControlSlider(m_parent, (sliderOffset + controlsOrigin) + dPos * MyGuiConstants.CONTROLS_DELTA, MyGuiConstants.SLIDER_WIDTH,
                    MyEditorConstants.COLOR_COMPONENT_MIN_VALUE, MyEditorConstants.COLOR_COMPONENT_MAX_VALUE, MyGuiConstants.SLIDER_BACKGROUND_COLOR,
                    new StringBuilder(), MyGuiConstants.SLIDER_WIDTH_LABEL, 0, MyGuiConstants.LABEL_TEXT_SCALE * 0.85f * 0.75f);
                AddControl(false, m_spotNormalLightColorSlider[i]);
            }


            dPos++;


            AddControl(false, new MyGuiControlLabel(m_parent, controlsOrigin + dPos * MyGuiConstants.CONTROLS_DELTA, null, new StringBuilder("Shadows Distance"), MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            dPos++;
            m_spotLightShadowDistance = new MyGuiControlSlider(m_parent, (sliderOffset + controlsOrigin) + dPos * MyGuiConstants.CONTROLS_DELTA, MyGuiConstants.SLIDER_WIDTH,
                0.0f, MyLightsConstants.MAX_SPOTLIGHT_SHADOW_RANGE, MyGuiConstants.SLIDER_BACKGROUND_COLOR,
                new System.Text.StringBuilder(" {0}"), MyGuiConstants.SLIDER_WIDTH_LABEL + 0.05f, 3, MyGuiConstants.LABEL_TEXT_SCALE);
            AddControl(false, m_spotLightShadowDistance);


            //angle slider
            dPos = dPosFalloff;
            AddControl(false, new MyGuiControlLabel(m_parent, controlsOrigin + dPos * MyGuiConstants.CONTROLS_DELTA, null, new StringBuilder("Angle"), MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            m_spotAngleSlider = new MyGuiControlSlider(m_parent, (sliderOffset + controlsOrigin) + dPos * MyGuiConstants.CONTROLS_DELTA, MyGuiConstants.SLIDER_WIDTH,
                0.1f, MyLightsConstants.MAX_SPOTLIGHT_ANGLE, MyGuiConstants.SLIDER_BACKGROUND_COLOR,
                new System.Text.StringBuilder(" {0}"), MyGuiConstants.SLIDER_WIDTH_LABEL + 0.05f, 3, MyGuiConstants.LABEL_TEXT_SCALE);
            AddControl(false, m_spotAngleSlider);

            //falloff slider
            dPos++;
            AddControl(false, new MyGuiControlLabel(m_parent, controlsOrigin + dPos * MyGuiConstants.CONTROLS_DELTA, null, new StringBuilder("Falloff"), MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            m_spotFallOffSlider = new MyGuiControlSlider(m_parent, (sliderOffset + controlsOrigin) + dPos * MyGuiConstants.CONTROLS_DELTA, MyGuiConstants.SLIDER_WIDTH,
                0.1f, 5.0f, MyGuiConstants.SLIDER_BACKGROUND_COLOR,
                new System.Text.StringBuilder(" {0}"), MyGuiConstants.SLIDER_WIDTH_LABEL + 0.05f, 3, MyGuiConstants.LABEL_TEXT_SCALE);
            AddControl(false, m_spotFallOffSlider);

            //range slider
            dPos++;
            AddControl(false, new MyGuiControlLabel(m_parent, controlsOrigin + dPos * MyGuiConstants.CONTROLS_DELTA, null, new StringBuilder("Range"), MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            m_spotRangeSlider = new MyGuiControlSlider(m_parent, (sliderOffset + controlsOrigin) + dPos * MyGuiConstants.CONTROLS_DELTA, MyGuiConstants.SLIDER_WIDTH,
                 0.1f, MyLightsConstants.MAX_SPOTLIGHT_RANGE, MyGuiConstants.SLIDER_BACKGROUND_COLOR,
                new System.Text.StringBuilder(" {0}"), MyGuiConstants.SLIDER_WIDTH_LABEL + 0.05f, 3, MyGuiConstants.LABEL_TEXT_SCALE);
            AddControl(false, m_spotRangeSlider);

            //intensity slider
            dPos++;
            AddControl(false, new MyGuiControlLabel(m_parent, controlsOrigin + dPos * MyGuiConstants.CONTROLS_DELTA, null, new StringBuilder("Intensity"), MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            m_spotIntensitySlider = new MyGuiControlSlider(m_parent, (sliderOffset + controlsOrigin) + dPos * MyGuiConstants.CONTROLS_DELTA, MyGuiConstants.SLIDER_WIDTH,
                0.1f, 10.0f, MyGuiConstants.SLIDER_BACKGROUND_COLOR,
                new System.Text.StringBuilder(" {0}"), MyGuiConstants.SLIDER_WIDTH_LABEL + 0.05f, 3, MyGuiConstants.LABEL_TEXT_SCALE);
            AddControl(false, m_spotIntensitySlider);

            dPos = 15;
            m_controls.Add(new MyGuiControlButton(m_parent, new Vector2(m_position.X - 0.05f, controlsOrigin.Y) + dPos * MyGuiConstants.CONTROLS_DELTA, MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.Ok, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnOkClick,
                MyGuiControlButtonTextAlignment.CENTERED, true, MyGuiDrawAlignEnum.HORISONTAL_RIGHT_AND_VERTICAL_CENTER, true));

            m_controls.Add(new MyGuiControlButton(m_parent, new Vector2(m_position.X + 0.05f, controlsOrigin.Y) + dPos * MyGuiConstants.CONTROLS_DELTA, MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.Cancel, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnCancelClick,
                MyGuiControlButtonTextAlignment.CENTERED, true, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER, true));



            ActivateLightType(true);
            //m_tabControl.ActivateTab((int)MyGuiLightPrefabTypeEnum.PointLight);
        }



        private void OnCopyPointDiffuseColor(MyGuiControlButton sender)
        {
            var prefabLight = m_prefabLights.FirstOrDefault();
            if (prefabLight != null)
            {
                MyLight light = prefabLight.GetLight();
                MyEditor.SetClipboard(light.Color);
            }
        }

        private void OnPastePointDiffuseColor(MyGuiControlButton sender)
        {
            Color color;
            if (MyEditor.GetClipboard<Color>(out color))
            {
                foreach (var prefabLight in m_prefabLights)
                {
                    MyLight light = prefabLight.GetLight();
                    light.Color = color.ToVector4();
                    InitializeValues(prefabLight);
                }
            }
        }

        private void OnCopyPointSpecularColor(MyGuiControlButton sender)
        {
            var prefabLight = m_prefabLights.FirstOrDefault();
            if (prefabLight != null)
            {
                MyLight light = prefabLight.GetLight();
                MyEditor.SetClipboard(light.SpecularColor);
            }
        }

        private void OnPastePointSpecularColor(MyGuiControlButton sender)
        {
            Color color;
            if (MyEditor.GetClipboard<Color>(out color))
            {
                foreach (var prefabLight in m_prefabLights)
                {
                    MyLight light = prefabLight.GetLight();
                    light.SpecularColor = color.ToVector3();
                    InitializeValues(prefabLight);
                }
            }
        }

        private void OnCopySpotDiffuseColor(MyGuiControlButton sender)
        {
            var prefabLight = m_prefabLights.FirstOrDefault();
            if (prefabLight != null)
            {
                MyLight light = prefabLight.GetLight();
                MyEditor.SetClipboard(light.ReflectorColor);
            }
        }

        private void OnPasteSpotDiffuseColor(MyGuiControlButton sender)
        {
            Color color;
            if (MyEditor.GetClipboard<Color>(out color))
            {
                foreach (var prefabLight in m_prefabLights)
                {
                    MyLight light = prefabLight.GetLight();
                    light.ReflectorColor = color.ToVector4();
                    InitializeValues(prefabLight);
                }
            }
        }

        private void InitializeValues(MyPrefabLight prefabLight)
        {
            MyLight light = prefabLight.GetLight();

            m_pointNormalLightColorSlider[0].SetNormalizedValue(light.Color.X);
            m_pointNormalLightColorSlider[1].SetNormalizedValue(light.Color.Y);
            m_pointNormalLightColorSlider[2].SetNormalizedValue(light.Color.Z);
            m_specularLightColorSlider[0].SetNormalizedValue(light.SpecularColor.X);
            m_specularLightColorSlider[1].SetNormalizedValue(light.SpecularColor.Y);
            m_specularLightColorSlider[2].SetNormalizedValue(light.SpecularColor.Z);
            m_pointFallOffSlider.SetValue(light.Falloff);
            m_pointRangeSlider.SetValue(light.Range);
            m_pointIntensitySlider.SetValue(light.Intensity);
            m_pointOffsetSlider.SetValue(light.PointLightOffset);

            m_spotNormalLightColorSlider[0].SetNormalizedValue(light.ReflectorColor.X);
            m_spotNormalLightColorSlider[1].SetNormalizedValue(light.ReflectorColor.Y);
            m_spotNormalLightColorSlider[2].SetNormalizedValue(light.ReflectorColor.Z);
            m_spotFallOffSlider.SetValue(light.ReflectorFalloff);
            m_spotRangeSlider.SetValue(light.ReflectorRange);
            m_spotIntensitySlider.SetValue(light.ReflectorIntensity);
            m_spotAngleSlider.SetValue(light.ReflectorConeDegrees);

            m_pointLightCheckbox.Checked = (light.LightType & MyLight.LightTypeEnum.PointLight) != 0;
            m_hemisphereLightCheckbox.Checked = (light.LightType & MyLight.LightTypeEnum.Hemisphere) != 0;
            m_spotLightCheckbox.Checked = (light.LightType & MyLight.LightTypeEnum.Spotlight) != 0;
            m_spotLightShadowDistance.SetValue(light.ShadowDistance); 

            m_flashOffsetSlider.SetValue(prefabLight.FlashOffset);

            m_effectComboBox.SelectItemByIndex((int) prefabLight.Effect);

            SetComponentsVisibility();
        }


        public void OnMyCheckEnable(MyGuiControlCheckbox sender)
        {
            foreach (MyPrefabLight prefabLight in m_prefabLights)
            {
                MyLight myLight = prefabLight.GetLight();
                float oldIntensity = myLight.Intensity; // Keep old intensity, because Start() will reset it to 1

                MyGuiControlCheckbox other;
                MyLight.LightTypeEnum lightType;
                
                if (sender == m_pointLightCheckbox)
                {
                    other = m_hemisphereLightCheckbox;
                    lightType = MyLight.LightTypeEnum.PointLight;
                }
                else if (sender == m_hemisphereLightCheckbox)
                {
                    other = m_pointLightCheckbox;
                    lightType = MyLight.LightTypeEnum.Hemisphere;
                }
                else  // (sender == m_spotLightCheckbox)
                {
                    other = null;
                    lightType = MyLight.LightTypeEnum.Spotlight;
                }

                if (sender.Checked)
                {
                    myLight.Start(lightType | myLight.LightType, myLight.Falloff);
                    myLight.GlareOn = true;
                    if (other != null) other.Checked = false;  // only one of Point and Hemisphere can be checked
                }
                else
                {
                    myLight.Start((~lightType & myLight.LightType), myLight.Falloff);
                    myLight.GlareOn = true;
                }



                myLight.Intensity = oldIntensity; // Set previous intensity

                prefabLight.OnWorldPositionChanged(this);
                prefabLight.UpdateEffect();
            }
            OnComponentChange(null);
        }

        /*
        public void OnMyShadowCheckEnable(MyGuiControlCheckbox sender)
        {
            foreach (MyPrefabLight prefabLight in m_prefabLights)
            {
                MyLight myLight = prefabLight.GetLight();

                if (sender == m_spotLightShadowCheckbox)
                {
                    myLight.ShadowsEnabled = sender.Checked;
                }

                prefabLight.OnWorldPositionChanged(this);
                prefabLight.UpdateEffect();
            }
            OnComponentChange(null);
        }
        */
        void OnComponentChange(MyGuiControlSlider sender)
        {
            foreach (MyPrefabLight prefabLight in m_prefabLights)
            {
                MyLight light = prefabLight.GetLight();
                Color tmpColor = new Color();
                tmpColor.R = (byte)m_pointNormalLightColorSlider[0].GetValue();
                tmpColor.G = (byte)m_pointNormalLightColorSlider[1].GetValue();
                tmpColor.B = (byte)m_pointNormalLightColorSlider[2].GetValue();
                light.Color = tmpColor.ToVector4();

                tmpColor.R = (byte)m_specularLightColorSlider[0].GetValue();
                tmpColor.G = (byte)m_specularLightColorSlider[1].GetValue();
                tmpColor.B = (byte)m_specularLightColorSlider[2].GetValue();
                light.SpecularColor = tmpColor.ToVector3();
                light.Falloff = m_pointFallOffSlider.GetValue(); // allowed values 0.1f-5.0f
                light.Range = m_pointRangeSlider.GetValue(); // allowed values 0.0f-MyLightsConstants.MAX_POINTLIGHT_RADIUS
                light.Intensity = prefabLight.m_IntensityMax = m_pointIntensitySlider.GetValue(); // allowed values 0.0f-10.0f
                light.PointLightOffset = m_pointOffsetSlider.GetValue();

                tmpColor.R = (byte)m_spotNormalLightColorSlider[0].GetValue();
                tmpColor.G = (byte)m_spotNormalLightColorSlider[1].GetValue();
                tmpColor.B = (byte)m_spotNormalLightColorSlider[2].GetValue();
                light.ReflectorColor = tmpColor.ToVector4();

                light.ReflectorFalloff = m_spotFallOffSlider.GetValue(); // allowed values 0.1f-5.0f
                light.ReflectorRange = m_spotRangeSlider.GetValue(); // allowed values 0.0f-MyLightsConstants.MAX_SPOTLIGHT_RANGE
                light.ReflectorIntensity = prefabLight.ReflectorIntensityMax = m_spotIntensitySlider.GetValue(); // allowed values 0.0f-10.0f

                light.ReflectorConeDegrees = m_spotAngleSlider.GetValue(); // allowed values 0.0f-MyLightsConstants.MAX_SPOTLIGHT_ANGLE
                light.ShadowDistance = m_spotLightShadowDistance.GetValue();
                prefabLight.FlashOffset = m_flashOffsetSlider.GetValue();
            }
        }

        void SetComponentsVisibility()
        {
            var effect = (MyLightEffectTypeEnum)m_effectComboBox.GetSelectedKey();

            switch (effect)
            {
                case MyLightEffectTypeEnum.CONSTANT_FLASHING:
                case MyLightEffectTypeEnum.DISTANT_GLARE_FLASHING:
                    m_flashOffsetSlider.Visible = m_flashOffsetLabel.Visible = true;
                    break;
                default:
                    m_flashOffsetSlider.Visible = m_flashOffsetLabel.Visible = false;
                    break;
            }
        }

        void OnComboBoxChange()
        {
            foreach (MyPrefabLight prefabLight in m_prefabLights)
            {
                prefabLight.Effect = (MyLightEffectTypeEnum)m_effectComboBox.GetSelectedKey();
            }

            SetComponentsVisibility();
        }




        public void OnSpotClick(MyGuiControlButton sender)
        {
            ActivateLightType(false);
        }

        public void OnPointClick(MyGuiControlButton sender)
        {
            ActivateLightType(true);
        }

        public void OnOkClick(MyGuiControlButton sender)
        {
            Visible = false;
        }

        public void OnCancelClick(MyGuiControlButton sender)
        {
            Visible = false;
            RestoreOriginalValues();
        }

        //public override bool Draw(float backgroundFadeAlpha)
        //{
        //    if (base.Draw(backgroundFadeAlpha) == false) return false;

        //    MyPrefabLight myLight = m_prefabLights[0] as MyPrefabLight;

        //    MyGuiControlSlider rS = m_spotNormalLightColorSlider[0];
        //    Color c = new Color(myLight.GetLight().ReflectorColor);
        //    c.A = 255;

        //    if (m_tabControl.GetSelectedTab() == (int)MyGuiLightPrefabTypeEnum.PointLight)
        //    {
        //        rS = m_pointNormalLightColorSlider[0];
        //        c = new Color(myLight.GetLight().Color);
        //        c.A = 255;
        //    }

        //    Vector2 pos = MyGuiManager.GetScreenCoordinateFromNormalizedCoordinate(m_position + rS.GetPosition());
        //    Vector2 size = MyGuiManager.GetScreenSizeFromNormalizedSize(new Vector2(0.5f * MyGuiConstants.SLIDER_WIDTH, 0.75f * MyGuiConstants.SLIDER_HEIGHT));

        //    // Draw one big final color rectangle to see result of each RGBA component in one
        //    MyGuiManager.DrawSpriteBatch(MyGuiManager.GetBlankTexture(), (int)pos.X, (int)pos.Y, (int)size.X, (int)size.Y, c);


        //    if (m_tabControl.GetSelectedTab() == (int)MyGuiLightPrefabTypeEnum.PointLight)
        //    {
        //        Vector2 pos2 = MyGuiManager.GetScreenCoordinateFromNormalizedCoordinate(m_position + m_specularLightColorSlider[0].GetPosition());

        //        Color c2 = new Color(myLight.GetLight().SpecularColor);
        //        c2.A = 255;

        //        // Draw color rectangles next to each color slider and then draw one big final color rectangle to see result of each RGBA component in one
        //        MyGuiManager.DrawSpriteBatch(MyGuiManager.GetBlankTexture(), (int)pos2.X, (int)pos2.Y, (int)size.X, (int)size.Y, c2);
        //    }

        //    return true;
        //}

        public override void Draw()
        {
            if (Visible)
            {
                foreach (MyGuiControlBase control in m_controls)
                {
                    if (control.Visible && control is MyGuiControlCombobox == false)
                    {
                        control.Draw();
                    }
                }

                foreach (MyGuiControlBase control in m_controls)
                {
                    if (control.Visible && control is MyGuiControlCombobox)
                    {
                        control.Draw();
                    }
                }

                DrawColors();
            }
        }

        private void DrawColors()
        {
            if (m_prefabLights.Count > 0)
            {
                MyPrefabLight myLight = m_prefabLights[0] as MyPrefabLight;

                MyGuiControlSlider rS = m_spotNormalLightColorSlider[0];
                Color color = new Color(myLight.GetLight().ReflectorColor);
                color.A = 255;

                if (m_pointLightCheckbox.Visible)
                {
                    rS = m_pointNormalLightColorSlider[0];
                    color = new Color(myLight.GetLight().Color);
                    color.A = 255;
                }

                Vector2 posOffset = new Vector2(-0.08f, -0.005f);
                Vector2 pos = MyGuiManager.GetScreenCoordinateFromNormalizedCoordinate(rS.GetPosition() - MyGuiConstants.CONTROLS_DELTA + posOffset);
                Vector2 colorLabelSize = MyGuiManager.GetScreenSizeFromNormalizedSize(new Vector2(0.5f * MyGuiConstants.SLIDER_HEIGHT, 0.5f * MyGuiConstants.SLIDER_HEIGHT));

                // Draw one big final color rectangle to see result of each RGBA component in one
                MyGuiManager.DrawSpriteBatch(MyGuiManager.GetBlankTexture(), (int)pos.X, (int)pos.Y, (int)colorLabelSize.X, (int)colorLabelSize.Y, color);

                if (m_pointLightCheckbox.Visible)
                {
                    Vector2 pos2 = MyGuiManager.GetScreenCoordinateFromNormalizedCoordinate(m_specularLightColorSlider[0].GetPosition() - MyGuiConstants.CONTROLS_DELTA + posOffset);

                    Color color2 = new Color(myLight.GetLight().SpecularColor);
                    color2.A = 255;

                    // Draw color rectangles next to each color slider and then draw one big final color rectangle to see result of each RGBA component in one
                    MyGuiManager.DrawSpriteBatch(MyGuiManager.GetBlankTexture(), (int)pos2.X, (int)pos2.Y, (int)colorLabelSize.X, (int)colorLabelSize.Y, color2);
                }
            }
        }

        public bool SelectionChanged()
        {
            if (MyEditorGizmo.SelectedEntities.Count == m_prefabLights.Count)
            {
                foreach (var item in MyEditorGizmo.SelectedEntities)
                {
                    if (!m_prefabLights.Contains(item))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public override void Update()
        {
            if (Visible && SelectionChanged())
            {
                RestoreOriginalValues();
                Visible = false;
            }

            foreach (MyGuiControlBase control in m_controls)
            {
                control.Update();
            }
        }

        public override bool HandleInput(MyGuiInput input, bool hasKeyboardActiveControl, bool hasKeyboardActiveControlPrevious, bool receivedFocusInThisUpdate)
        {
            bool inputCaptured = base.HandleInput(input, hasKeyboardActiveControl, hasKeyboardActiveControlPrevious, receivedFocusInThisUpdate);
            if (Visible)
            {
                foreach (MyGuiControlBase control in m_controls)
                {
                    if (control.Visible && control is MyGuiControlCombobox && control.HandleInput(input, false, false, receivedFocusInThisUpdate))
                    {
                        return true;
                    }
                }

                foreach (MyGuiControlBase control in m_controls)
                {
                    if (control.Visible && control is MyGuiControlCombobox == false && control.HandleInput(input, false, false, receivedFocusInThisUpdate))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public override bool ContainsMouse()
        {
            foreach (MyGuiControlBase myGuiControlBase in m_controls)
            {
                if (myGuiControlBase.ContainsMouse())
                {
                    return true;
                }
            }
            return false;
        }

        protected override bool CheckMouseOver()
        {
            foreach (MyGuiControlBase myGuiControlBase in m_controls)
            {
                if (myGuiControlBase.IsMouseOver())
                {
                    return true;
                }
            }
            return false;
        }
        
        private void ActivateLightType(bool point)
        {
            foreach (var item in m_pointLightControls) item.Visible = point;
            foreach (var item in m_spotLightControls) item.Visible = !point;
        }

        void MyEntities_OnEntityRemove(MyEntity entity)
        {
            MyPrefabLight light = entity as MyPrefabLight;
            if (light != null)
            {
                m_prefabLights.Remove(light);
                if (m_prefabLights.Count == 0 && Visible)
                {
                    Visible = false;
                }
            }
        }
    }
}
