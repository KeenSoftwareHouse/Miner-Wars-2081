using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Toolkit.Input;
using MinerWars.AppCode.Game.Managers;
using MinerWarsMath.Graphics;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Audio;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Localization;

namespace MinerWars.AppCode.Game.GUI.Core
{
    class MyGuiControlWheelControl : MyGuiControlBase
    {
        //bool m_visible;
        int m_selected;
        int m_first;
        Vector2 m_mousePositionLast;
        int? m_mouseWheelValueLast = null;
        const int ITEM_COUNT = 13;
        const float ITEM_HEIGHT = 0.044f;
        const float BUTTON_HEIGHT = 0.025f;
        const float BUTTON_VERTICAL_DIFF = 0.015f;
        const float SLIDER_WIDTH = 0.0f; // <-make it zero size, else 0.0125f;
        const float TOTAL_WIDTH = 0.25f;
        const float FOG_TOTAL_WIDTH = TOTAL_WIDTH * 0.9f;
        const float TEXT_SCALE = 0.83f;
        const float BACKGROUND_FOG_HEIGHT_MULTILPIER = 1.66999936f;
        const float BACKGROUND_FOG_FADE_HEIGHT_MULTILPIER = 3.0f;
        MyPhysObjectSmallShipConfigItem[] m_displayedItems = new MyPhysObjectSmallShipConfigItem[ITEM_COUNT];

        private static readonly Vector2 itemSize = new Vector2(530 / 1600f, 35 / 1200f);

        int m_itemsCount;
        bool m_isMouseOnItem;
        bool m_isMouseOnUpButton;
        bool m_isMouseOnDownButton;
        float? m_mouseScrollbarDragStartY = null;
        
        //public bool IsVisible { get { return m_visible; } }
        public bool IsEnable { get; set; }


        private MyTexture2D m_backgroundtexture;
        //private MyTexture2D m_smallBg;
        private MyTexture2D m_smallBgHover;


        public MyGuiControlWheelControl(IMyGuiControlsParent parent)
            : base(parent, new Vector2(0.5f, 0.5f), new Vector2(1,1), null, null)
        {
            Visible = false;
            m_selected = 0;
            m_first = 0;
            m_size = new Vector2(673/1600f, 910/1200f);
        }


        public override bool Visible
        {
            get
            {
                return base.Visible;
            }
            set
            {
                base.Visible = value;
                if (value)
                {
                    LoadTextures();
                }
                else
                {
                    UnloadTextures();
                }
            }
        }


        private void LoadTextures()
        {
            m_backgroundtexture = MyGuiManager.GetConfigWheelBackground();
            m_smallBgHover = MyGuiManager.GetConfigWheelHover();
            //m_smallBg = MyGuiManager.GetConfigWheelBackgroundSmall();
        }

        private void UnloadTextures()
        {
            MyTextureManager.UnloadTexture(m_backgroundtexture);
            MyTextureManager.UnloadTexture(m_smallBgHover);
        }

        public override bool HandleInput(MyGuiInput input, bool hasKeyboardActiveControl, bool hasKeyboardActiveControlPrevious, bool receivedFocusInThisUpdate)
        {
            //If this control is not enable, do nothing
            if (!IsEnable) return false;

            if (Visible)
            {
                if (((input.IsKeyPress(Keys.LeftShift) || input.IsKeyPress(Keys.RightShift)) && input.IsNewKeyPress(Keys.Tab)) || input.IsNewKeyPress(Keys.Up) || input.IsNewGamepadKeyUpPressed())
                    SelectPrev();
                if ((!(input.IsKeyPress(Keys.LeftShift) || input.IsKeyPress(Keys.RightShift)) && input.IsNewKeyPress(Keys.Tab)) || input.IsNewKeyPress(Keys.Down) || input.IsNewGamepadKeyDownPressed())
                    SelectNext();
                if (input.IsNewKeyPress(Keys.Escape) || input.IsJoystickButtonNewPressed(MyJoystickButtonsEnum.J02))
                    HideScreenIfPossible();

                if (m_displayedItems[m_selected] != null)
                {
                    if (input.IsNewKeyPress(Keys.Left) || input.IsNewGamepadKeyLeftPressed())
                        m_displayedItems[m_selected].ChangeValueDown();
                    if (input.IsNewKeyPress(Keys.Right) || input.IsNewGamepadKeyRightPressed())
                        m_displayedItems[m_selected].ChangeValueUp();
                    if (input.IsNewKeyPress(Keys.Enter) || input.IsNewKeyPress(Keys.Space) || input.IsJoystickButtonNewPressed(MyJoystickButtonsEnum.J01))
                        m_displayedItems[m_selected].ChangeValueUp();
                }
                
                HandleMouse(input);
            }

            if (input.IsNewGameControlPressed(MyGameControlEnums.WHEEL_CONTROL))
            {
                m_first = 0;
                Visible = !Visible;
                if (Visible == false)
                {
                    MyAudio.AddCue2D(MySoundCuesEnum.GuiWheelControlClose);
                }
                else
                {
                    MyAudio.AddCue2D(MySoundCuesEnum.GuiWheelControlOpen);
                }
            }

            return base.HandleInput(input, hasKeyboardActiveControl, hasKeyboardActiveControlPrevious, receivedFocusInThisUpdate);
        }


        public void HideScreenIfPossible()
        {
            m_first = 0;
            if (Visible)
                MyAudio.AddCue2D(MySoundCuesEnum.GuiWheelControlClose);
            Visible = false;
        }


        private void SelectNext()
        {
            if (m_selected >= m_displayedItems.Length - 1 && m_first + m_displayedItems.Length >= m_itemsCount) return;
            if (m_selected >= m_displayedItems.Length - 1)
                m_first++;
            else
                m_selected++;

            if (m_displayedItems[m_selected] == null)
            {
                SelectNext();
            }
        }

        private void SelectPrev()
        {
            if (m_selected <= 0 && m_first <= 0) return;
            if (m_selected <= 0)
                m_first--;
            else
                m_selected--;

            if (m_displayedItems[m_selected] == null)
            {
                SelectPrev();
            }
        }

        //private bool MoveNext()
        //{
        //    if (m_first + ITEM_COUNT >= m_itemsCount) 
        //        return false;
        //    m_first++;
        //    if (m_selected > 0) m_selected--;
        //    return true;
        //}

        //private bool MovePrev()
        //{
        //    if (m_first <= 0) 
        //        return false;
        //    m_first--;
        //    if (m_selected < ITEM_COUNT - 1) m_selected++;
        //    return true;
        //}

        public override void 
            Draw()
        {
            if (!Visible || MySession.PlayerShip == null) return;

            MySession.PlayerShip.Config.Items(ref m_displayedItems);

            MyGuiManager.BeginSpriteBatch();

            var offset = new Vector2(VideoMode.MyVideoModeManager.IsTripleHead() ? -1 : 0, 0);

            //Draw background
            MyGuiManager.DrawSpriteBatch(m_backgroundtexture,
                new Vector2(0.5f,0.5f) + offset,
                m_size.Value, Color.White,
                MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER);

            //Draw items
            for (int i = 0, j = 0; i < m_displayedItems.Length; i++)
            {
                if (m_displayedItems[i] == null) continue;
                if (m_selected == i)
                {
                    var pos = new Vector2(0.5f , m_position.Y - (ITEM_HEIGHT * m_displayedItems.Length / 2) + (ITEM_HEIGHT * j) - 0.003f);
                    MyGuiManager.DrawSpriteBatch(m_smallBgHover, pos + offset, itemSize,
                            Color.White, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_TOP);
                }


                //Draw item name
                var nameWithKey = new StringBuilder().Append(m_displayedItems[i].Name);
                MyGameControlEnums? control = m_displayedItems[i].AssociatedControl;
                if (control != null)
                {
                    string key = MyGuiManager.GetInput().GetGameControlTextEnum(control.Value);
                    if (key != MyTextsWrapper.Get(MyTextsWrapperEnum.UnknownControl).ToString())
                    {
                        nameWithKey.Append(" (").Append(key).Append(")");
                    }
                }

                MyGuiManager.DrawString(MyGuiManager.GetFontMinerWarsBlue(), nameWithKey,
                    new Vector2(m_position.X - (TOTAL_WIDTH / 2) - 0.02f, m_position.Y - (ITEM_HEIGHT * m_displayedItems.Length / 2) + (ITEM_HEIGHT * j)) + offset,
                    TEXT_SCALE, Color.White, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP);


                //Draw item value bg 
                var smallBgSize = MyGuiManager.GetNormalizedSizeFromScreenSize(MyGuiManager.GetFontMinerWarsGreen().MeasureString(m_displayedItems[i].CurrentValueText, TEXT_SCALE));
                if (smallBgSize.LengthSquared() > 0.00f)
                {
                    var bgPos = new Vector2(m_position.X + (TOTAL_WIDTH / 2) - SLIDER_WIDTH + 0.01f - smallBgSize.X / 2 , m_position.Y - (ITEM_HEIGHT * m_displayedItems.Length / 2) + (ITEM_HEIGHT * j) - 0.003f);
                    smallBgSize.Y = itemSize.Y;
                    smallBgSize.X *= 1.6f;
                    MyGuiManager.DrawSpriteBatch(m_smallBgHover, bgPos + offset, smallBgSize, new Color(255, 255, 255, 200), MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_TOP);
                }

                //draw item
                MyGuiManager.DrawString(MyGuiManager.GetFontMinerWarsGreen(), m_displayedItems[i].CurrentValueText,
                    new Vector2(m_position.X + (TOTAL_WIDTH / 2) - SLIDER_WIDTH + 0.01f, m_position.Y - (ITEM_HEIGHT * m_displayedItems.Length / 2) + (ITEM_HEIGHT * j)) + offset,
                    TEXT_SCALE, Color.White, MyGuiDrawAlignEnum.HORISONTAL_RIGHT_AND_VERTICAL_TOP);

                j++;
            }

            /*
            //Draw top button
            if (IsUpButtonVisible)
            {
                MyGuiManager.DrawSpriteBatch(MyGuiManager.GetConfigWheelArrowTextureUp(),
                    new Vector2(m_position.X - (TOTAL_WIDTH / 2), m_position.Y - (ITEM_HEIGHT * ITEM_COUNT / 2) - BUTTON_HEIGHT - BUTTON_VERTICAL_DIFF) + offset,
                    new Vector2(TOTAL_WIDTH, BUTTON_HEIGHT), Color.White, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP);
            }

             * */

            //Draw bottom button
            //if (IsDownButtonVisible)
            //{   
            //    MyGuiManager.DrawSpriteBatch(MyGuiManager.GetConfigWheelArrowTextureDown(),
            //        new Vector2(m_position.X - (TOTAL_WIDTH / 2), m_position.Y + (ITEM_HEIGHT * m_displayedItems.Length / 2) + BUTTON_VERTICAL_DIFF) + offset,
            //        new Vector2(TOTAL_WIDTH, BUTTON_HEIGHT), Color.White, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP);
            //}

            //Draw scrollbar
            //if (IsScrollBarVisible)
            //{
            //    MyGuiManager.DrawSpriteBatch(MyGuiManager.GetBlankTexture(),
            //        new Vector2(m_position.X + (TOTAL_WIDTH / 2) - SLIDER_WIDTH, m_position.Y - (ITEM_HEIGHT * m_displayedItems.Length / 2)) + offset,
            //        new Vector2(SLIDER_WIDTH, m_displayedItems.Length * ITEM_HEIGHT), new Color(Color.RoyalBlue.R, Color.RoyalBlue.G, Color.RoyalBlue.B, (byte)(255 * 0.3f)),
            //        MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP);
            //    MyGuiManager.DrawSpriteBatch(MyGuiManager.GetScrollbarSlider(),
            //        SliderNormalizedCoord, SliderNormalizedSize, Color.White,
            //        MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP);
            //}

            MyGuiManager.EndSpriteBatch();
        }

        private Vector2 SliderNormalizedCoord
        {
            get { return new Vector2(m_position.X + (TOTAL_WIDTH / 2) - SLIDER_WIDTH, m_position.Y - (ITEM_HEIGHT * m_displayedItems.Length / 2) + (ITEM_HEIGHT * m_displayedItems.Length / m_itemsCount) * m_first); }
        }
        private Vector2 SliderNormalizedSize
        {
            get { return new Vector2(SLIDER_WIDTH, m_displayedItems.Length * ITEM_HEIGHT * m_displayedItems.Length / m_itemsCount); }
        }
        private bool IsScrollBarVisible
        {
            get { return false; } // (m_itemsCount > m_displayedItems.Length);
        }
        private bool IsUpButtonVisible
        {
            get { return (m_first > 0); }
        }
        private bool IsDownButtonVisible
        {
            get { return (m_first < m_itemsCount - m_displayedItems.Length); }
        }

        private void HandleMouse(MyGuiInput input)
        {
            //Handle mouse wheel
            if (m_mouseWheelValueLast == null) m_mouseWheelValueLast = input.MouseScrollWheelValue();
            if (input.MouseScrollWheelValue() < m_mouseWheelValueLast)
            {
                //if (!MoveNext())
                    SelectNext();
            }
            if (input.MouseScrollWheelValue() > m_mouseWheelValueLast)
            {
                //if (!MovePrev())
                    SelectPrev();
            }
            m_mouseWheelValueLast = input.MouseScrollWheelValue();

            //Do nothing if mouse don't move.
            if (!input.IsNewLeftMousePressed() && m_mousePositionLast != null && m_mousePositionLast == MyGuiManager.MouseCursorPosition) return;
            m_mousePositionLast = MyGuiManager.MouseCursorPosition;

            if (input.IsNewLeftMousePressed() && m_displayedItems[m_selected] != null)
            {
                m_displayedItems[m_selected].ChangeValueUp();
            }
        }        
    }
}
