using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Toolkit.Input;
using MinerWars.AppCode.Game.Managers;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Audio;
using System.Collections.Generic;

namespace MinerWars.AppCode.Game.GUI.Core
{
    class MyGuiControlSimpleTabControl : MyGuiControlParent
    {

        private string m_caption;

        public MyGuiControlSimpleTabControl(IMyGuiControlsParent parent, Vector2 position, Vector2 size, Vector4 color)
            : base(parent, position, size, color, null, null)
        {
            m_caption = "";
            m_backgroundColor = null;
        }

        public void SetCatption(StringBuilder str)
        {
            m_caption = str.ToString();
        }

        public string GetCaption()
        {
            return m_caption; 
        }

        /*
        public MyGuiControlSimpleTabControl(MyGuiScreenBase parentScreen, Vector2 position, Vector2 size, Vector4 color)
            : base(position, color, size)
        {
            //m_parentScreen = parentScreen;
        }
        */        

        /*
        public override string GetFriendlyName()
        {
            return "TabControl";
        }
        */
        public override bool HandleInput(MyGuiInput input, bool hasKeyboardActiveControl, bool hasKeyboardActiveControlPrevious, bool receivedFocusInThisUpdate)
        {
            
            bool ret = base.HandleInput(input, hasKeyboardActiveControl, hasKeyboardActiveControlPrevious, receivedFocusInThisUpdate);
            /*
            if (ret == false)
            {
                if (((IsMouseOver() == true) && input.IsNewLeftMousePressed()) ||
                    ((hasKeyboardActiveControl == true) && ((input.IsNewKeyPress(Keys.Enter) || (input.IsNewKeyPress(Keys.Space)) || (input.IsNewGameControlPressed(MyGameControlEnums.FIRE_PRIMARY) && !input.IsNewLeftMousePressed())))))
                {
                    MyAudio.AddCue2D(MySoundCuesEnum.GuiClick);
                    //Checked = !Checked;
                    //ret = true;
                }
            }
            */

            return ret;
        }

        public override void Draw()
        {
            base.Draw();
        }

    }

    class MyGuiControlTabControl : /*MyGuiControlBase*/MyGuiControlParent
    {
        
        Dictionary<int, MyGuiControlSimpleTabControl> m_tabs = new Dictionary<int, MyGuiControlSimpleTabControl>();

        int m_selectedTab = 0;
        Vector2 m_headSize;
        Vector4 m_color;        
        MyTexture2D m_selectedTexture;
        MyTexture2D m_unSelectedTexture;

        public MyGuiControlTabControl(IMyGuiControlsParent parent, Vector2 position, Vector2 size, Vector2 headSize, Vector4 color)
            : base(parent, position, size, color, null, null)
            //: base(parent, position, size, color, null, null, null, null, true)
        {
            m_headSize = headSize;
            m_color = color;
        }

        
        /*
        public void SelectTab(int key)
        {
            if (m_selectedTab != key)
            {
                DeactivateControls(m_selectedTab);
                ActivateControls(key);
            }
            m_selectedTab = key;
        }
        */

        public MyGuiControlSimpleTabControl GetTabSubControl(int key)
        {
            if (!m_tabs.ContainsKey(key))
            {
                //m_tabs[key] = new MyGuiControlSimpleTabControl(m_parent, new Vector2(0, m_headSize.Y), new Vector2(1, 1 - m_headSize.Y), m_color);
                m_tabs[key] = new MyGuiControlSimpleTabControl(this, new Vector2(0, m_headSize.Y), new Vector2(1, 1 - m_headSize.Y), m_color);
                Controls.Add(m_tabs[key]);
            }
            return m_tabs[key];
        }


        public void ActivateTab(int key)
        {
            foreach (int i in m_tabs.Keys)
            {
                if (i == key)
                {
                    m_tabs[i].Visible = true;
                }
                else 
                {
                    m_tabs[i].Visible = false;
                }
            }
            m_selectedTab = key;
        }



        /*
        private void DeactivateControls(int type)
        {
            foreach (var item in m_allControls[type])
            {
                m_parentScreen.Controls.Remove(item);
            }
        }

        private void ActivateControls(int type)
        {
            foreach (var item in m_allControls[type])
            {
                m_parentScreen.Controls.Add(item);
            }
        }
        */

        /*
        public void AddControl(int key, MyGuiControlBase screen)
        {
            
            if (!m_allScreens.ContainsKey(key))
            {
                //m_allScreens[key] = new MyGuiScreenBase();
            }

            
            if (!m_allControls[key].Contains(screen))
            {
                //assert
                return;
            }
            
            //check if we have control screen
            //
            m_allScreens[key].Controls.Add(screen);
        }

        public void RemoveControl(int key, MyGuiControlBase screen)
        {

            if (!m_allScreens.ContainsKey(key))
            {
                //assert
                return;
            }

            m_allScreens[key].Controls.Remove(screen);
        }
        */

        //returns mouse over tab head otherwise -1
        private int GetMouseOverTab()
        {
            int count = m_tabs.Keys.Count;
            int pos = 0;

            foreach (int i in m_tabs.Keys)
            {
                Vector2 parentPos = m_parent.GetPositionAbsolute();
                Vector2? parentSize = m_parent.GetSize();
                parentPos -= parentSize.Value / 2;
                Vector2 offsetPos = parentPos + pos * new Vector2(m_headSize.X * parentSize.Value.X / count, 0);
                Vector2 tabHeadSize = new Vector2((m_headSize.X / (count)) * parentSize.Value.X, (m_headSize.Y) * parentSize.Value.Y);

                Vector2 min = offsetPos;
                Vector2 max = offsetPos + tabHeadSize;

                if(((MyGuiManager.MouseCursorPosition.X >= min.X) && (MyGuiManager.MouseCursorPosition.X <= max.X) && (MyGuiManager.MouseCursorPosition.Y >= min.Y) && (MyGuiManager.MouseCursorPosition.Y <= max.Y)))
                    return i;

                pos++;
            }

            return -1;
        }

        public override bool HandleInput(MyGuiInput input, bool hasKeyboardActiveControl, bool hasKeyboardActiveControlPrevious, bool receivedFocusInThisUpdate)
        {
            bool ret = GetTabSubControl(m_selectedTab).HandleInput(input,hasKeyboardActiveControl, hasKeyboardActiveControlPrevious, receivedFocusInThisUpdate);
            if (ret)
                return true;
            ret = base.HandleInput(input, hasKeyboardActiveControl, hasKeyboardActiveControlPrevious, receivedFocusInThisUpdate);
            if (ret == false)
            {
                int tab = GetMouseOverTab();
                if (tab != -1 && input.IsNewLeftMousePressed())
                {

                    MyAudio.AddCue2D(MySoundCuesEnum.GuiMouseClick);
                    ActivateTab(tab);
                    //m_selectedTab = tab;
                    ret = true;
                }
            }

            return ret;
        }

        public int GetSelectedTab()
        {
            return m_selectedTab;
        }

        public override void Draw()
        {

            
            /*
            int count = m_allToolTips.Keys.Count;
            for (int c = 0; c < count; c++)
            {
                MyGuiManager.DrawSpriteBatch(m_selectedTexture, m_parentScreen.GetPosition() + m_position, m_size.Value,
                    GetColorAfterTransitionAlpha(m_backgroundColor.Value), MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER);
            }
            */
            //base.Draw();

            //draw buttons on head with texts
            int count = m_tabs.Keys.Count;
            int pos = 0;

            foreach (int i in m_tabs.Keys)
            {
                MyTexture2D buttonTexture = MyGuiManager.GetButtonTexture();
                Vector2 parentPos = m_parent.GetPositionAbsolute();
                Vector2 ?parentSize = m_parent.GetSize();
                parentPos -= parentSize.Value/2;
                Vector2 offsetPos = parentPos + pos * new Vector2(m_headSize.X * parentSize.Value.X / count, 0);
                Vector2 tabHeadSize = new Vector2((m_headSize.X / (count)) * parentSize.Value.X, (m_headSize.Y) * parentSize.Value.Y);

                bool isHighlight = GetMouseOverTab() == i || GetSelectedTab() == i;

                bool isNotImplementedForbidenOrDisabled = /*!m_implementedFeature || m_accessForbiddenReason != null ||*/ !Enabled;
                /*
                bool isHighlighted = IsMouseOverOrKeyboardActive() &&
                    (m_highlightType == MyGuiControlHighlightType.WHEN_ACTIVE || (m_highlightType == MyGuiControlHighlightType.WHEN_CURSOR_OVER && IsMouseOver()));
                */
                Vector4 backgroundColor = isNotImplementedForbidenOrDisabled ? MyGuiConstants.DISABLED_BUTTON_COLOR_VECTOR :
                                         (isHighlight ? m_backgroundColor.Value * MyGuiConstants.CONTROL_MOUSE_OVER_BACKGROUND_COLOR_MULTIPLIER : m_backgroundColor.Value);
                backgroundColor.W = 1.0f;
                // Draw background texture
                MyGuiManager.DrawSpriteBatch(buttonTexture, offsetPos, tabHeadSize * m_scale,
                    new Color(backgroundColor), MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP);

                

                Vector4 textColor = isNotImplementedForbidenOrDisabled ? textColor = MyGuiConstants.DISABLED_BUTTON_TEXT_COLOR :
                                   (( isHighlight ) ? m_color * MyGuiConstants.CONTROL_MOUSE_OVER_BACKGROUND_COLOR_MULTIPLIER : m_color);

                StringBuilder text = new StringBuilder (GetTabSubControl(i).GetCaption());
                if (text != null)
                {
                    Vector2 textPosition;
                    MyGuiDrawAlignEnum textDrawAlign;

                    textPosition = offsetPos + tabHeadSize/2;
                    textDrawAlign = MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER;

                    /*
                    if (m_textAlignment == MyGuiControlButtonTextAlignment.CENTERED)
                    {
                        textPosition = m_parentScreen.GetPosition() + m_position;
                        textDrawAlign = MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER;
                    }
                    else if (m_textAlignment == MyGuiControlButtonTextAlignment.LEFT)
                    {
                        //  This will move text few pixels from button's left border
                        textPosition = m_parentScreen.GetPosition() + m_position - new Vector2(m_size.Value.X / 2.0f, 0) + new Vector2(MyGuiConstants.BUTTON_TEXT_OFFSET.X, 0);
                        textDrawAlign = MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER;
                    }
                    else
                    {
                        throw new MyMwcExceptionApplicationShouldNotGetHere();
                    }
                    */

                    MyGuiManager.DrawString(MyGuiManager.GetFontMinerWarsWhite(), text, textPosition, 1.2f/*m_textScale*/, GetColorAfterTransitionAlpha(textColor), textDrawAlign);
                }
                
                pos++;
            }

            //base.Draw();
            GetTabSubControl(m_selectedTab).Draw();
        }

    }
}

