using System;
using System.Globalization;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.Editor;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.GUI.Helpers;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Entities.InfluenceSpheres;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.InfluenceSpheres;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.Audio;

namespace MinerWars.AppCode.Game.GUI.ScreenEditor.Object3D
{
    class MyGuiScreenEditorInfluenceSphere : MyGuiScreenEditorObject3DBase
    {
        private const float MAX_RADIOACTIVITY_MAGNITUDE = 50;
        private float m_controlsAdded;
        private MyGuiControlCheckbox m_enabledCheckBox;
        private MyGuiControlTextbox m_radiusMinTextBox;
        private MyGuiControlTextbox m_radiusMaxTextBox;
        private Color m_sphereDustColor;
        private MyGuiControlSlider m_redColorSlider;
        private MyGuiControlSlider m_greenColorSlider;
        private MyGuiControlSlider m_blueColorSlider;
        private MyGuiControlSlider m_opacitySlider;
        private StringBuilder m_opacityValueText;
        private MyGuiControlSlider m_strengthSlider;
        private MyGuiControlCombobox m_selectSoundCombobox;
        private MyGuiControlCheckbox m_dustCheckbox;
        private MyGuiControlCheckbox m_radioactivityCheckbox;
        private MyGuiControlCheckbox m_soundCheckbox;

        public override string GetFriendlyName()
        {
            return "MyGuiScreenEditorInfluenceSphere";
        }

        public MyGuiScreenEditorInfluenceSphere(Vector2? screenPosition)
            : base(new Vector2(0.5f, 0.5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, new Vector2(0.7f, 0.9f), MyTextsWrapperEnum.InfluenceSphereContextMenuCaption, screenPosition)
        {
            Init();
        }

        public MyGuiScreenEditorInfluenceSphere(MyInfluenceSphere influenceSphere)
            : base(influenceSphere, new Vector2(0.5f, 0.5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, new Vector2(0.7f, 0.9f), MyTextsWrapperEnum.InfluenceSphereContextMenuCaption)
        {
            Init();
        }

        #region Init

        private void Init()
        {
            m_size = new Vector2(0.7f, 1);

            m_enableBackgroundFade = true;

            AddCaption(new Vector2(0, 0.003f));
            AddOkAndCancelButtonControls(new Vector2(0.03f, -0.02f));

            var controlsOrigin = GetControlsOriginLeftFromScreenSize() - new Vector2(-0.018f, 0.025f);            
            m_controlsAdded = 1;

            CreateControlsBase(controlsOrigin);

            m_activatedCheckbox.OnCheck += OnActivatedCheckedChanged;

            CreateControlsDust(controlsOrigin);

            CreateControlsSound(controlsOrigin);

            CreateControlsRadioactivity(controlsOrigin);

            if (HasEntity())
            {
                var influenceSphere = (MyInfluenceSphere) m_entity;
                m_radiusMinTextBox.Text = influenceSphere.RadiusMin.ToString(CultureInfo.InvariantCulture);
                m_radiusMaxTextBox.Text = influenceSphere.RadiusMax.ToString(CultureInfo.InvariantCulture);

                m_enabledCheckBox.Checked = influenceSphere.Enabled;
                m_activatedCheckbox.Checked = influenceSphere.Activated;

                m_redColorSlider.SetValue(influenceSphere.DustColor.R);
                m_greenColorSlider.SetValue(influenceSphere.DustColor.G);
                m_blueColorSlider.SetValue(influenceSphere.DustColor.B);
                m_opacitySlider.SetValue(influenceSphere.DustColor.A);

                m_strengthSlider.SetValue(influenceSphere.Magnitude);

                var objectBuilder = (MyMwcObjectBuilder_InfluenceSphere) influenceSphere.GetObjectBuilder(false);
                m_selectSoundCombobox.SelectItemByKey(objectBuilder.SoundCueId);

                m_dustCheckbox.Checked = influenceSphere.IsDust;
                m_radioactivityCheckbox.Checked = influenceSphere.IsRadioactivity;
                m_soundCheckbox.Checked = influenceSphere.IsSound;
            }
            else
            {
                m_opacitySlider.SetValue(255);
            }
        }

        void OnActivatedCheckedChanged(MyGuiControlCheckbox sender)
        {
            if (m_activatedCheckbox.Checked)
            {
                m_enabledCheckBox.Enabled = true;
            }
            else
            {
                m_enabledCheckBox.Checked = false;
                m_enabledCheckBox.Enabled = false;
            }
        }

        private void CreateControlsBase(Vector2 controlsOrigin)
        {
            AddActivatedCheckbox(controlsOrigin + m_controlsAdded * CONTROLS_DELTA + new Vector2(0.0f, 0), true);

            Controls.Add(
                new MyGuiControlLabel(
                    this,
                    controlsOrigin + m_controlsAdded * CONTROLS_DELTA + new Vector2(0.2f, 0),
                    null,
                    MyTextsWrapperEnum.Enabled,
                    MyGuiConstants.LABEL_TEXT_COLOR,
                    MyGuiConstants.LABEL_TEXT_SCALE,
                    MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));

            m_enabledCheckBox = new MyGuiControlCheckbox(
                this,
                controlsOrigin + m_controlsAdded++ * CONTROLS_DELTA + new Vector2(0.3f, 0),
                false,
                MyGuiConstants.CHECKBOX_BACKGROUND_COLOR);

            Controls.Add(m_enabledCheckBox);            

            // Set Radius Min
            Controls.Add(new MyGuiControlLabel(this, controlsOrigin + m_controlsAdded * CONTROLS_DELTA, null,
                                               MyTextsWrapperEnum.RadiusMin,
                                               MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE,
                                               MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            m_radiusMinTextBox = new MyGuiControlTextbox(this,
                                                         (new Vector2(0.17f, 0) + controlsOrigin) +
                                                         m_controlsAdded++ * CONTROLS_DELTA +
                                                         new Vector2(0.093f, 0), MyGuiControlPreDefinedSize.MEDIUM, "50",
                                                         MyMwcValidationConstants.POSITION_X_MAX,
                                                         MyGuiConstants.TEXTBOX_BACKGROUND_COLOR,
                                                         MyGuiConstants.LABEL_TEXT_SCALE,
                                                         MyGuiControlTextboxType.DIGITS_ONLY);
            Controls.Add(m_radiusMinTextBox);

            // Set Radius Max
            Controls.Add(new MyGuiControlLabel(this, controlsOrigin + m_controlsAdded * CONTROLS_DELTA, null,
                                               MyTextsWrapperEnum.RadiusMax,
                                               MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE,
                                               MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            m_radiusMaxTextBox = new MyGuiControlTextbox(this,
                                                         (new Vector2(0.17f, 0) + controlsOrigin) +
                                                         m_controlsAdded++ * CONTROLS_DELTA +
                                                         new Vector2(0.093f, 0), MyGuiControlPreDefinedSize.MEDIUM,
                                                         "100",
                                                         MyMwcValidationConstants.POSITION_X_MAX,
                                                         MyGuiConstants.TEXTBOX_BACKGROUND_COLOR,
                                                         MyGuiConstants.LABEL_TEXT_SCALE,
                                                         MyGuiControlTextboxType.DIGITS_ONLY);
            Controls.Add(m_radiusMaxTextBox);
        }

        private void CreateControlsDust(Vector2 controlsOrigin)
        {
            AddSeparator(controlsOrigin);

            Controls.Add(new MyGuiControlLabel(this, controlsOrigin + m_controlsAdded * CONTROLS_DELTA + new Vector2(0.05f, 0), null, MyTextsWrapperEnum.DustInfluenceSphereType, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            m_dustCheckbox = new MyGuiControlCheckbox(this, controlsOrigin + m_controlsAdded++ * CONTROLS_DELTA + new Vector2(0.02f, 0), false, MyGuiConstants.CHECKBOX_BACKGROUND_COLOR);
            Controls.Add(m_dustCheckbox);

            //Red slider
            Controls.Add(new MyGuiControlLabel(this, controlsOrigin + m_controlsAdded * CONTROLS_DELTA, null,
                                               MyTextsWrapperEnum.Red, MyGuiConstants.LABEL_TEXT_COLOR,
                                               MyGuiConstants.LABEL_TEXT_SCALE,
                                               MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            m_redColorSlider = new MyGuiControlSlider(this,
                                                      (new Vector2(0.22f, 0) + controlsOrigin) +
                                                      m_controlsAdded++ * CONTROLS_DELTA, MyGuiConstants.SLIDER_WIDTH,
                                                      MyEditorConstants.COLOR_COMPONENT_MIN_VALUE,
                                                      MyEditorConstants.COLOR_COMPONENT_MAX_VALUE,
                                                      MyGuiConstants.SLIDER_BACKGROUND_COLOR,
                                                      new StringBuilder(), MyGuiConstants.SLIDER_WIDTH_LABEL, 0,
                                                      MyGuiConstants.LABEL_TEXT_SCALE * 0.85f);
            m_redColorSlider.OnChange = OnRedColorChange;
            Controls.Add(m_redColorSlider);

            //Green slider
            Controls.Add(new MyGuiControlLabel(this, controlsOrigin + m_controlsAdded * CONTROLS_DELTA, null,
                                               MyTextsWrapperEnum.Green, MyGuiConstants.LABEL_TEXT_COLOR,
                                               MyGuiConstants.LABEL_TEXT_SCALE,
                                               MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            m_greenColorSlider = new MyGuiControlSlider(this,
                                                        (new Vector2(0.22f, 0) + controlsOrigin) +
                                                        m_controlsAdded++ * CONTROLS_DELTA, MyGuiConstants.SLIDER_WIDTH,
                                                        MyEditorConstants.COLOR_COMPONENT_MIN_VALUE,
                                                        MyEditorConstants.COLOR_COMPONENT_MAX_VALUE,
                                                        MyGuiConstants.SLIDER_BACKGROUND_COLOR,
                                                        new StringBuilder(), MyGuiConstants.SLIDER_WIDTH_LABEL, 0,
                                                        MyGuiConstants.LABEL_TEXT_SCALE * 0.85f);
            m_greenColorSlider.OnChange = OnGreenColorChange;
            Controls.Add(m_greenColorSlider);
            // Final color display

            //Blue slider
            Controls.Add(new MyGuiControlLabel(this, controlsOrigin + m_controlsAdded * CONTROLS_DELTA, null,
                                               MyTextsWrapperEnum.Blue, MyGuiConstants.LABEL_TEXT_COLOR,
                                               MyGuiConstants.LABEL_TEXT_SCALE,
                                               MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            m_blueColorSlider = new MyGuiControlSlider(this,
                                                       (new Vector2(0.22f, 0) + controlsOrigin) +
                                                       m_controlsAdded++ * CONTROLS_DELTA, MyGuiConstants.SLIDER_WIDTH,
                                                       MyEditorConstants.COLOR_COMPONENT_MIN_VALUE,
                                                       MyEditorConstants.COLOR_COMPONENT_MAX_VALUE,
                                                       MyGuiConstants.SLIDER_BACKGROUND_COLOR,
                                                       new StringBuilder(), MyGuiConstants.SLIDER_WIDTH_LABEL, 0,
                                                       MyGuiConstants.LABEL_TEXT_SCALE * 0.85f);
            m_blueColorSlider.OnChange = OnBlueColorChange;
            Controls.Add(m_blueColorSlider);

            //Opacity slider
            Controls.Add(new MyGuiControlLabel(this, controlsOrigin + m_controlsAdded * CONTROLS_DELTA, null,
                                               MyTextsWrapperEnum.Opacity, MyGuiConstants.LABEL_TEXT_COLOR,
                                               MyGuiConstants.LABEL_TEXT_SCALE,
                                               MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            m_opacitySlider = new MyGuiControlSlider(this,
                                                     (new Vector2(0.22f, 0) + controlsOrigin) +
                                                     m_controlsAdded++ * CONTROLS_DELTA, MyGuiConstants.SLIDER_WIDTH,
                                                     MyEditorConstants.COLOR_COMPONENT_MIN_VALUE,
                                                     MyEditorConstants.COLOR_COMPONENT_MAX_VALUE,
                                                     MyGuiConstants.SLIDER_BACKGROUND_COLOR,
                                                     new StringBuilder(), MyGuiConstants.SLIDER_WIDTH_LABEL, 0,
                                                     MyGuiConstants.LABEL_TEXT_SCALE * 0.85f);
            m_opacitySlider.OnChange = OnOpacityChange;
            Controls.Add(m_opacitySlider);
            m_opacityValueText = new StringBuilder();
        }

        private void CreateControlsRadioactivity(Vector2 controlsOrigin)
        {
            AddSeparator(controlsOrigin);

            Controls.Add(new MyGuiControlLabel(this, controlsOrigin + m_controlsAdded * CONTROLS_DELTA + new Vector2(0.05f, 0), null, MyTextsWrapperEnum.RadioactivityInfluenceSphereType, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            m_radioactivityCheckbox = new MyGuiControlCheckbox(this, controlsOrigin + m_controlsAdded++ * CONTROLS_DELTA + new Vector2(0.02f, 0), false, MyGuiConstants.CHECKBOX_BACKGROUND_COLOR);
            Controls.Add(m_radioactivityCheckbox);

            //choose influence sphere sound
            Controls.Add(new MyGuiControlLabel(this, controlsOrigin + m_controlsAdded * CONTROLS_DELTA, null,
                                               MyTextsWrapperEnum.Magnitude, MyGuiConstants.LABEL_TEXT_COLOR,
                                               MyGuiConstants.LABEL_TEXT_SCALE,
                                               MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));

            // slider - magnitude
            m_strengthSlider = new MyGuiControlSlider(this,
                                                      controlsOrigin + m_controlsAdded++ * CONTROLS_DELTA +
                                                      new Vector2(0.25f, 0), MyGuiConstants.SLIDER_WIDTH,
                                                      0, MAX_RADIOACTIVITY_MAGNITUDE,
                                                      MyGuiConstants.SLIDER_BACKGROUND_COLOR, new StringBuilder("{0}"),
                                                      0.03f, 1, MyGuiConstants.LABEL_TEXT_SCALE * 0.85f);

            m_strengthSlider.SetValue(1);

            Controls.Add(m_strengthSlider);
        }

        private void CreateControlsSound(Vector2 controlsOrigin)
        {
            AddSeparator(controlsOrigin);

            Controls.Add(new MyGuiControlLabel(this, controlsOrigin + m_controlsAdded * CONTROLS_DELTA + new Vector2(0.05f, 0), null, MyTextsWrapperEnum.SoundInfluenceSphereType, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            m_soundCheckbox = new MyGuiControlCheckbox(this, controlsOrigin + m_controlsAdded++ * CONTROLS_DELTA + new Vector2(0.02f, 0), false, MyGuiConstants.CHECKBOX_BACKGROUND_COLOR);
            Controls.Add(m_soundCheckbox);

            //m_controlsAdded++;

            //COMBOBOX - sounds
            //m_selectSoundCombobox = new MyGuiControlCombobox(this,
            //                                                 controlsOrigin + m_controlsAdded++ * CONTROLS_DELTA +
            //                                                 new Vector2(MyGuiConstants.COMBOBOX_LARGE_SIZE.X / 2.0f, 0),
            //                                                 MyGuiControlPreDefinedSize.LARGE,
            //                                                 MyGuiConstants.COMBOBOX_BACKGROUND_COLOR,
            //                                                 MyGuiConstants.COMBOBOX_TEXT_SCALE, 3, true, false, true);
            m_selectSoundCombobox = new MyGuiControlCombobox(this,
                                                             controlsOrigin + m_controlsAdded++ * CONTROLS_DELTA +
                                                             new Vector2(MyGuiConstants.COMBOBOX_LONGMEDIUM_SIZE.X / 2.0f, 0),
                                                             MyGuiControlPreDefinedSize.MEDIUM,
                                                             MyGuiConstants.COMBOBOX_BACKGROUND_COLOR,
                                                             MyGuiConstants.COMBOBOX_TEXT_SCALE, 6, false, false, false);

            foreach (MySoundCuesEnum enumValue in MyGuiInfluenceSphereHelpers.MyInfluenceSphereSoundHelperTypesEnumValues)
            {
                MyGuiInfluenceSphereHelper musicHelper = MyGuiInfluenceSphereHelpers.GetInfluenceSphereSoundHelper(enumValue);
                if (musicHelper != null && musicHelper.MultiLineDescription.ToString().ToLower().StartsWith("amb2d_"))
                {
                    m_selectSoundCombobox.AddItem((int)enumValue, musicHelper.Icon, musicHelper.MultiLineDescription);
                }
            }

            m_selectSoundCombobox.SelectItemByKey(0);
            //m_selectSoundCombobox.OnSelectItemDoubleClick = OnOkClick;
            Controls.Add(m_selectSoundCombobox);
        }

        private void AddSeparator(Vector2 controlsOrigin)
        {
            var pos = controlsOrigin + m_controlsAdded * CONTROLS_DELTA + new Vector2(0.3f, -0.01f);

            Controls.Add(new MyGuiControlPanel(this, pos,
                                               new Vector2(0.6f, 0.005f), Vector4.Zero, 2,
                                               MyGuiConstants.DEFAULT_CONTROL_NONACTIVE_COLOR));

            m_controlsAdded += 0.5f;
        }

        #endregion

        public override void OnOkClick(MyGuiControlButton sender)
        {
            float radiusMin;
            float radiusMax;

            if (!float.TryParse(m_radiusMinTextBox.Text, out radiusMin))
            {
                MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, MyTextsWrapperEnum.YouHaveToSelect, MyTextsWrapperEnum.MessageBoxCaptionError, MyTextsWrapperEnum.Ok, null));
                return;
            }
            
            if (!float.TryParse(m_radiusMaxTextBox.Text, out radiusMax))
            {
                MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, MyTextsWrapperEnum.YouHaveToSelect, MyTextsWrapperEnum.MessageBoxCaptionError, MyTextsWrapperEnum.Ok, null));
                return;
            }

            // Disallow to enter lower MAX radius than MIN radius
            if (radiusMax < radiusMin)
            {
                MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, MyTextsWrapperEnum.RadiusMaxLowerThanRadiusMin, MyTextsWrapperEnum.MessageBoxCaptionError, MyTextsWrapperEnum.Ok, null));
                return;
            }
            
            if (!m_dustCheckbox.Checked && !m_radioactivityCheckbox.Checked && !m_soundCheckbox.Checked)
            {
                MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, MyTextsWrapperEnum.YouHaveToSelect, MyTextsWrapperEnum.MessageBoxCaptionError, MyTextsWrapperEnum.Ok, null));
                return;
            }
            
            if (m_soundCheckbox.Checked && m_selectSoundCombobox.GetSelectedKey() == -1)
            {
                MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, MyTextsWrapperEnum.YouHaveToSelect, MyTextsWrapperEnum.MessageBoxCaptionError, MyTextsWrapperEnum.Ok, null));
                return;
            }
            
            {
                base.OnOkClick(sender);

                float magnitude = m_strengthSlider.GetValue();

                var soundType = (MySoundCuesEnum)m_selectSoundCombobox.GetSelectedKey();

                if (HasEntity())
                {
                    // Update currently selected influence sphere properties
                    var influenceSphere = (MyInfluenceSphere)m_entity;

                    influenceSphere.Enabled = m_enabledCheckBox.Checked;

                    if (radiusMin > influenceSphere.RadiusMax)
                    {
                        influenceSphere.RadiusMax = radiusMax;
                        influenceSphere.RadiusMin = radiusMin;
                    }
                    else
                    {
                        influenceSphere.RadiusMin = radiusMin;
                        influenceSphere.RadiusMax = radiusMax;
                    }

                    var r = (byte)m_redColorSlider.GetValue();
                    var g = (byte)m_greenColorSlider.GetValue();
                    var b = (byte)m_blueColorSlider.GetValue();
                    var a = (byte)m_opacitySlider.GetValue();
                    influenceSphere.DustColor = new Color(r, g, b, a);

                    influenceSphere.Magnitude = magnitude;

                    influenceSphere.ChangeCueEnum(soundType);

                    influenceSphere.IsDust = m_dustCheckbox.Checked;
                    influenceSphere.IsRadioactivity = m_radioactivityCheckbox.Checked;
                    influenceSphere.IsSound = m_soundCheckbox.Checked;
                    influenceSphere.Activate(m_activatedCheckbox.Checked, false);
                }
                else
                {
                    // Create new influence sphere dust
                    var builder = new MyMwcObjectBuilder_InfluenceSphere();
                    builder.InfluenceFlags |= MyInfluenceFlags.Dust;
                    builder.RadiusMin = radiusMin;
                    builder.RadiusMax = radiusMax;
                    builder.Enabled = m_enabledCheckBox.Checked;

                    var r = (byte)m_redColorSlider.GetValue();
                    var g = (byte)m_greenColorSlider.GetValue();
                    var b = (byte)m_blueColorSlider.GetValue();
                    var a = (byte)m_opacitySlider.GetValue();
                    builder.DustColor = new Color(r, g, b, a);

                    builder.Magnitude = magnitude;

                    builder.SoundCueId = (short)soundType;

                    builder.IsDust = m_dustCheckbox.Checked;
                    builder.IsRadioactivity = m_radioactivityCheckbox.Checked;
                    builder.IsSound = m_soundCheckbox.Checked;
                    if(!m_activatedCheckbox.Checked)
                    {
                        builder.PersistentFlags |= CommonLIB.AppCode.ObjectBuilders.MyPersistentEntityFlags.Deactivated;
                    }
                    MyEditor.Static.CreateFromObjectBuilder(builder, Matrix.Identity, m_screenPosition);
                }

                this.CloseScreen();
            }
        }

        #region Controls' events

        private void OnRedColorChange(MyGuiControlSlider sender)
        {
            m_sphereDustColor.R = (byte) m_redColorSlider.GetValue();
        }

        private void OnGreenColorChange(MyGuiControlSlider sender)
        {
            m_sphereDustColor.G = (byte) m_greenColorSlider.GetValue();
        }

        private void OnBlueColorChange(MyGuiControlSlider sender)
        {
            m_sphereDustColor.B = (byte) m_blueColorSlider.GetValue();
        }

        private void OnOpacityChange(MyGuiControlSlider sender)
        {
            m_sphereDustColor.A = (byte) m_opacitySlider.GetValue();
            m_opacityValueText.Clear();
            m_opacityValueText.Append(m_sphereDustColor.A);
        }

        #endregion

        public override bool Draw(float backgroundFadeAlpha)
        {
            if (base.Draw(backgroundFadeAlpha) == false)
                return false;

            Vector2 opacityPosition = m_position + m_opacitySlider.GetPosition() +
                                      new Vector2(0.13f, 0);

            Vector2 finalColorPosition = m_position + m_redColorSlider.GetPosition() +
                                         new Vector2(0.18f, -MyGuiConstants.SLIDER_HEIGHT / 2);
            Vector2 finalColorScreenCoord = MyGuiManager.GetScreenCoordinateFromNormalizedCoordinate(finalColorPosition);

            Vector2 finalColorSize = MyGuiManager.GetScreenSizeFromNormalizedSize(new Vector2(3 * MyGuiConstants.SLIDER_HEIGHT, 3 * MyGuiConstants.SLIDER_HEIGHT) + new Vector2(0, 0.024f));

            // Draws one big final color rectangle to see result of each RGBA component in one
            MyGuiManager.DrawString(MyGuiManager.GetFontMinerWarsWhite(), m_opacityValueText, opacityPosition, MyGuiConstants.LABEL_TEXT_SCALE, new Color(MyGuiConstants.LABEL_TEXT_COLOR), MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);
            MyGuiManager.DrawSpriteBatch(MyGuiManager.GetBlankTexture(), (int)finalColorScreenCoord.X, (int)finalColorScreenCoord.Y, (int)finalColorSize.X, (int)finalColorSize.Y, m_sphereDustColor);

            return true;
        }
    }
}