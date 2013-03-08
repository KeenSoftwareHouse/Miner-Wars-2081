using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Prefabs;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Localization;
using MinerWarsMath;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.Prefabs;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.GUI.RichControls;

namespace MinerWars.AppCode.Game.GUI.ScreenEditor.Object3D
{
    class MyGuiScreenEditorScanner : MyGuiScreenEditorObject3DBase
    {
        MyGuiControlSize m_widthSize;
        MyGuiControlSize m_heightSize;
        MyGuiControlSize m_depthSize;
        MyGuiControlSize m_scanningSpeedSize;        
        MyGuiControlCheckbox m_onCheckbox;                

        MyGuiControlSlider[] m_colorSlider;
        Vector2 m_colorDrawPosition;        

        // original values
        Vector3 m_originalSize;
        float m_originalScanningSpeed;
        Color m_originalColor;
        bool m_originalOn;

        private MyPrefabScanner Scanner { get { return (MyPrefabScanner)m_entity; } }

        public MyGuiScreenEditorScanner(MyPrefabScanner scanner)
            : base(scanner, new Vector2(0.5f, 0.5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, null, MyTextsWrapperEnum.EditScanner)
        {
            m_originalSize = Scanner.Size;
            m_originalColor = Scanner.Color;
            m_originalOn = Scanner.Enabled;
            m_originalScanningSpeed = Scanner.ScanningSpeed;
            Init();
            Scanner.Enabled = false;
        }        

        private void Init()
        {
            m_enableBackgroundFade = true;
            m_size = new Vector2(0.9f, 0.85f);

            Vector2 controlsOriginLeft = GetControlsOriginLeftFromScreenSize() + new Vector2(0.06f, 0.06f);
            Vector2 sliderOffset = new Vector2(0.3f, 0f);

            // First create controls
            CreateControls(controlsOriginLeft, sliderOffset);

            // Add screen title
            AddCaption(new Vector2(0, 0.025f));
            AddOkAndCancelButtonControls(new Vector2(0.01f, -0.02f));            

            m_colorSlider[0].SetValue(Scanner.Color.R);
            m_colorSlider[1].SetValue(Scanner.Color.G);
            m_colorSlider[2].SetValue(Scanner.Color.B);            
            m_widthSize.SetValue(Scanner.Size.X);
            m_heightSize.SetValue(Scanner.Size.Y);
            m_depthSize.SetValue(Scanner.Size.Z);
            m_scanningSpeedSize.SetValue(Scanner.ScanningSpeed);
            m_onCheckbox.Checked = Scanner.Enabled;

            m_colorSlider[0].OnChange = OnColorChange;
            m_colorSlider[1].OnChange = OnColorChange;
            m_colorSlider[2].OnChange = OnColorChange;            
            m_widthSize.OnValueChange += OnWidthChange;
            m_heightSize.OnValueChange += OnHeightChange;
            m_depthSize.OnValueChange += OnDepthChange;
            m_scanningSpeedSize.OnValueChange += OnScanningSpeedChange;                        
        }

        public void CreateControls(Vector2 controlsOrigin, Vector2 sliderOffset)
        {                        
            float sliderMax = 1000f;

            AddActivatedCheckbox(controlsOrigin, Scanner.Activated);

            AddIdTextBox(new Vector2(-0.17f, controlsOrigin.Y), Scanner.EntityId.Value.NumericValue);

            //Width slider            
            m_widthSize = new MyGuiControlSize(this, controlsOrigin + new Vector2(0.4f, 0f) + 1 * CONTROLS_DELTA, new Vector2(0.8f, MyGuiConstants.SLIDER_HEIGHT), Vector4.Zero, null, 0f, 0.1f, sliderMax, MyTextsWrapper.Get(MyTextsWrapperEnum.Width), MyGuiSizeEnumFlags.All, sliderOffset.X);            
            Controls.Add(m_widthSize);

            //Height slider            
            m_heightSize = new MyGuiControlSize(this, controlsOrigin + new Vector2(0.4f, 0f) + 2 * CONTROLS_DELTA, new Vector2(0.8f, MyGuiConstants.SLIDER_HEIGHT), Vector4.Zero, null, 0f, 0.1f, sliderMax, MyTextsWrapper.Get(MyTextsWrapperEnum.Height), MyGuiSizeEnumFlags.All, sliderOffset.X);
            Controls.Add(m_heightSize);

            //Depth slider            
            m_depthSize = new MyGuiControlSize(this, controlsOrigin + new Vector2(0.4f, 0f) + 3 * CONTROLS_DELTA, new Vector2(0.8f, MyGuiConstants.SLIDER_HEIGHT), Vector4.Zero, null, 0f, 0.1f, sliderMax, MyTextsWrapper.Get(MyTextsWrapperEnum.Depth), MyGuiSizeEnumFlags.All, sliderOffset.X);
            Controls.Add(m_depthSize);

            //Scanning speed slider            
            m_scanningSpeedSize = new MyGuiControlSize(this, controlsOrigin + new Vector2(0.4f, 0f) + 4 * CONTROLS_DELTA, new Vector2(0.8f, MyGuiConstants.SLIDER_HEIGHT), Vector4.Zero, null, 0f, 0.1f, sliderMax, MyTextsWrapper.Get(MyTextsWrapperEnum.ScanningSpeed), MyGuiSizeEnumFlags.All, 0.3f);
            Controls.Add(m_scanningSpeedSize);

            // Color
            Vector2 colorPosition = controlsOrigin + 5 * CONTROLS_DELTA;
            m_colorDrawPosition = colorPosition + sliderOffset;
            Controls.Add(new MyGuiControlLabel(this, colorPosition, null, MyTextsWrapperEnum.Color, MyGuiConstants.LABEL_TEXT_COLOR,
                MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));

            MyTextsWrapperEnum[] colorNames = { MyTextsWrapperEnum.Red, MyTextsWrapperEnum.Green, MyTextsWrapperEnum.Blue };
            m_colorSlider = new MyGuiControlSlider[3];
            //color sliders
            for (int i = 0; i < 3; i++)
            {
                Controls.Add(new MyGuiControlLabel(this, controlsOrigin + (6 + i) * CONTROLS_DELTA, null, colorNames[i], MyGuiConstants.LABEL_TEXT_COLOR,
                    MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
                m_colorSlider[i] = new MyGuiControlSlider(this, (sliderOffset + controlsOrigin) + (6 + i) * CONTROLS_DELTA, MyGuiConstants.SLIDER_WIDTH,
                    MyEditorConstants.COLOR_COMPONENT_MIN_VALUE, MyEditorConstants.COLOR_COMPONENT_MAX_VALUE, MyGuiConstants.SLIDER_BACKGROUND_COLOR,
                    new StringBuilder(), MyGuiConstants.SLIDER_WIDTH_LABEL, 0, MyGuiConstants.LABEL_TEXT_SCALE * 0.85f * 0.75f);                
                Controls.Add(m_colorSlider[i]);
            }

            Controls.Add(new MyGuiControlLabel(this, controlsOrigin + 9 * CONTROLS_DELTA, null, MyTextsWrapperEnum.On, MyGuiConstants.LABEL_TEXT_COLOR,
                    MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            m_onCheckbox = new MyGuiControlCheckbox(this, controlsOrigin + sliderOffset + 9 * CONTROLS_DELTA, Scanner.Enabled, MyGuiConstants.CHECKBOX_BACKGROUND_COLOR); 
            Controls.Add(m_onCheckbox);                                
        }        

        public override string GetFriendlyName()
        {
            return "MyGuiScreenEditorScanner";
        }

        void OnWidthChange(MyGuiControlBase sender)
        {
            float width = ((MyGuiControlSize)sender).GetValue();
            Vector3 size = Scanner.Size;
            Scanner.Size = new Vector3(width, size.Y, size.Z);            
        }

        void OnHeightChange(MyGuiControlBase sender)
        {
            float height = ((MyGuiControlSize)sender).GetValue();
            Vector3 size = Scanner.Size;
            Scanner.Size = new Vector3(size.X, height, size.Z);
        }

        void OnDepthChange(MyGuiControlBase sender)
        {
            float depth = ((MyGuiControlSize)sender).GetValue();
            Vector3 size = Scanner.Size;
            Scanner.Size = new Vector3(size.X, size.Y, depth);
        }

        void OnScanningSpeedChange(MyGuiControlBase sender) 
        {
            float scanningSpeed = ((MyGuiControlSize)sender).GetValue();
            Scanner.ScanningSpeed = scanningSpeed;            
        }

        void OnColorChange(MyGuiControlSlider sender) 
        {
            Color color = new Color();
            color.R = (byte)m_colorSlider[0].GetValue();
            color.G = (byte)m_colorSlider[1].GetValue();
            color.B = (byte)m_colorSlider[2].GetValue();
            color.A = 255;
            Scanner.Color = color;
        }        

        public override bool Draw(float backgroundFadeAlpha)
        {
            if (!base.Draw(backgroundFadeAlpha))
                return false;
                        
            MyGuiManager.DrawSpriteBatch(MyGuiManager.GetBlankTexture(), this.GetPositionAbsolute() + m_colorDrawPosition, new Vector2(0.04f, 0.04f), Scanner.Color, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER);

            return true;
        }

        public override void OnOkClick(MyGuiControlButton sender)
        {
            base.OnOkClick(sender);
            
            Scanner.ReinitScanningPart();
            Scanner.Enabled = m_onCheckbox.Checked;
            Scanner.Activate(m_activatedCheckbox.Checked, false);

            // close all opened screens except gameplay
            MyGuiManager.CloseAllScreensExcept(MyGuiScreenGamePlay.Static);
        }

        public override void OnCancelClick(MyGuiControlButton sender)
        {
            base.OnCancelClick(sender);

            Scanner.Size = m_originalSize;
            Scanner.Color = m_originalColor;
            Scanner.Enabled = m_originalOn;
            Scanner.ScanningSpeed = m_originalScanningSpeed;
        }
    }
}
