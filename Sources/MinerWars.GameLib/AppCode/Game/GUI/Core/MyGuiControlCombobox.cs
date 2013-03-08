using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using MinerWarsMath;
using MinerWarsMath.Graphics;
using MinerWars.AppCode.Toolkit.Input;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Audio;
using SysUtils.Utils;
using MinerWars.AppCode.Game.Textures;

namespace MinerWars.AppCode.Game.GUI.Core
{
    class MyGuiControlCombobox : MyGuiControlBase
    {
        public class MyGuiControlComboboxItem : IComparable
        {
            public int Key;
            public MyTexture2D Icon;
            public StringBuilder Value;
            public int SortOrder;
            public MyToolTips ToolTip;

            public MyGuiControlComboboxItem(int key, MyTexture2D icon, StringBuilder value, int sortOrder)
            {
                Key = key;
                Icon = icon;
                Value = value;
                SortOrder = sortOrder;
                if (value != null)
                {
                    ToolTip = new MyToolTips(value);
                }                
            }

            //  Sorts from small to large, e.g. 0, 1, 2, 3, ...
            public int CompareTo(object compareToObject)
            {
                MyGuiControlComboboxItem compareToItem = (MyGuiControlComboboxItem)compareToObject;
                return this.SortOrder.CompareTo(compareToItem.SortOrder);
            }
        }

        public delegate void OnComboBoxSelectCallback();
        public event OnComboBoxSelectCallback OnSelect;

        public delegate void OnComboBoxItemDoubleClick();
        public event OnComboBoxItemDoubleClick OnSelectItemDoubleClick = null;

        Vector4 m_textColor;
        float m_textScale;
        Vector2 m_textOffset;
        bool m_isOpen;
        bool m_scrollBarDragging = false;
        List<MyGuiControlComboboxItem> m_items;
        MyGuiControlComboboxItem m_selected;                            //  Item that is selected in the combobox, that is displayed in the main rectangle
        MyGuiControlComboboxItem m_preselectedMouseOver;                //  Item that is under mouse and may be selected if user clicks on it
        MyGuiControlComboboxItem m_preselectedMouseOverPrevious;        //  Same as m_preselectedMouseOver, but in previous update
        int? m_preselectedKeyboardIndex = null;                                 //  Same as m_preselectedMouseOver, but for keyboard. By default no item is selected.
        int? m_preselectedKeyboardIndexPrevious = null;                         //  Same as m_preselectedMouseOverPrevious, but for keyboard
        int? m_mouseWheelValueLast = null;

        //  Scroll Bar logic code
        int m_openAreaItemsCount;
        int m_middleIndex;
        bool m_showScrollBar;
        float? m_scrollBarCurrentPosition;
        float m_scrollBarCurrentNonadjustedPosition;
        float m_mouseOldPosition;
        bool m_mousePositionReinit;
        float m_maxScrollBarPosition;
        float m_scrollBarEndPositionRelative;
        int m_displayItemsStartIndex;
        int m_displayItemsEndIndex;
        int m_scrollBarItemOffSet;
        float m_scrollBarHeight;
        float m_scrollBarWidth; // not the texture width, but the clickable area width
        float m_comboboxItemDeltaHeight;
        float m_scrollRatio;
        bool m_supportIcon;
        bool m_supportSmoothScroll;
        bool m_supportListBoxMode;
        Vector2 m_iconPadding;
        Vector2 m_iconSize;
        Vector2 m_iconTextOffset;
        private Vector2 m_dropDownItemSize;
        double m_doubleClickTimer;
        const double DOUBLE_CLICK_DELAY = 500;

        private const float ITEM_DRAW_DELTA = 0.0001f;

        bool m_useScrollBarOffset = false;

        public MyGuiControlCombobox(IMyGuiControlsParent parent, Vector2 position, MyGuiControlPreDefinedSize predefinedSize, Vector2 iconSize, Vector2 textOffset, Vector4 backgroundColor, float textScale, int openAreaItemsCount, bool supportPicture, bool supportSmoothScroll, bool supportListBoxMode, bool useScrollBarOffset = false)
            : base(parent, position, predefinedSize, backgroundColor, null, MyGuiManager.GetComboboxTexture(predefinedSize), null, null, true)
        {
            m_highlightType = MyGuiControlHighlightType.WHEN_CURSOR_OVER;
            m_canHandleKeyboardActiveControl = true;
            m_items = new List<MyGuiControlComboboxItem>();
            m_isOpen = supportListBoxMode;
            m_textColor = MyGuiConstants.COMBOBOX_TEXT_COLOR;
            m_textScale = textScale;
            m_openAreaItemsCount = openAreaItemsCount;
            m_middleIndex = Math.Max(m_openAreaItemsCount / 2 - 1, 0);
            m_supportIcon = supportPicture;
            m_supportSmoothScroll = supportSmoothScroll;
            m_supportListBoxMode = supportListBoxMode;
            m_textOffset = textOffset;
            m_size = GetPredefinedControlSize();
            if(m_supportIcon)
            {         
                m_size = new Vector2(m_size.Value.X, iconSize.Y);                
            }
            m_dropDownItemSize = GetItemSize();
            m_comboboxItemDeltaHeight = m_dropDownItemSize.Y; // 
            m_doubleClickTimer = 0;
            m_mousePositionReinit = true;
            InitializeScrollBarParameters();
            m_showToolTip = true;
            m_useScrollBarOffset = useScrollBarOffset;
        }





        //  Base Constructor
        public MyGuiControlCombobox(IMyGuiControlsParent parent, Vector2 position, MyGuiControlPreDefinedSize predefinedSize, Vector4 backgroundColor, float textScale, int openAreaItemsCount, bool supportPicture, bool supportSmoothScroll, bool supportListBoxMode, bool useScrollbarOffset = false)
            : this(parent, position, predefinedSize, MyGuiConstants.COMBOBOX_ICON_SIZE, MyGuiConstants.COMBOBOX_TEXT_OFFSET, backgroundColor, textScale, openAreaItemsCount, supportPicture, supportSmoothScroll, supportListBoxMode, useScrollbarOffset)
        { }

        //

        //  Original MyGuiControlCombobox constructor with same signatures
        public MyGuiControlCombobox(IMyGuiControlsParent parent, Vector2 position, MyGuiControlPreDefinedSize predefinedSize, Vector4 backgroundColor, float textScale)
            : this(parent, position, predefinedSize, backgroundColor, textScale, 10, false, false, false)
        { }

        public MyGuiControlCombobox(IMyGuiControlsParent parent, Vector2 position, MyGuiControlPreDefinedSize predefinedSize, Vector4 backgroundColor, float textScale, int openAreaItemsCount)
            : this(parent, position, predefinedSize, backgroundColor, textScale, openAreaItemsCount, false, false, false)
        { }

        //  Clears/removes all items
        public void ClearItems()
        {
            m_items.Clear();
            m_selected = null;
            m_preselectedKeyboardIndex = null;
            m_preselectedKeyboardIndexPrevious = null;
            m_preselectedMouseOver = null;
            m_preselectedMouseOverPrevious = null;
            InitializeScrollBarParameters();
        }

        //  Same as other AddItem, but this one auto-assign sort order
        public void AddItem(int key, MyTextsWrapperEnum value)
        {
            AddItem(key, value, m_items.Count);
        }

        //  Add new item
        public void AddItem(int key, MyTextsWrapperEnum value, int sortOrder)
        {
            AddItem(key, MyTextsWrapper.Get(value), sortOrder);
        }

        //  Add new item
        public void AddItem(int key, StringBuilder value)
        {
            AddItem(key, value, m_items.Count);
        }

        // Add new item with icon texture
        public void AddItem(int key, MyTexture2D icon, StringBuilder value)
        {
            AddItem(key, icon, value, m_items.Count);
        }

        //  Add new item
        public void AddItem(int key, StringBuilder value, int sortOrder)
        {
            System.Diagnostics.Debug.Assert(value != null);

            //  Create new item
            MyGuiControlComboboxItem newItem = new MyGuiControlComboboxItem(key, null, value, sortOrder);            
            //  Add to list
            m_items.Add(newItem);

            //  Reorder the list
            m_items.Sort();

            //  scroll bar parameters need to be recalculated when new item is added
            AdjustScrollBarParameters();
        }

        //  Add new item with icon
        public void AddItem(int key, MyTexture2D icon, StringBuilder value, int sortOrder)
        {
            System.Diagnostics.Debug.Assert(value != null);

            //  Create new item
            MyGuiControlComboboxItem newItem = new MyGuiControlComboboxItem(key, icon, value, sortOrder);            
            //  Add to list
            m_items.Add(newItem);

            //  Reorder the list
            m_items.Sort();

            //  scroll bar parameters need to be recalculated when new item is added
            AdjustScrollBarParameters();
        }

        public void RemoveItem(int key) 
        {
            MyGuiControlComboboxItem removedItem = m_items.Find(x => x.Key == key);
            RemoveItem(removedItem);
        }

        public void RemoveItemByIndex(int index) 
        {
            if (index < 0 || index >= m_items.Count) 
            {
                throw new ArgumentOutOfRangeException("index");
            }

            RemoveItem(m_items[index]);
        }

        private void RemoveItem(MyGuiControlComboboxItem item) 
        {
            if (item == null) 
            {
                throw new ArgumentNullException("item");
            }

            m_items.Remove(item);

            // if we remove selected item (clear selection)
            if (m_selected == item)
            {
                m_selected = null;
            }
        }

        public int GetItemsCount()
        {
            return m_items.Count;
        }

        public void SortItemsByValueText()
        {
            if (m_items != null)
            {
                m_items.Sort(delegate(MyGuiControlComboboxItem item1, MyGuiControlComboboxItem item2)
                {
                    return item1.Value.ToString().CompareTo(item2.Value.ToString());
                });
            }
        }

        public void CustomSortItems(Comparison<MyGuiControlComboboxItem> comparison)
        {
            if (m_items != null)
            {
                m_items.Sort(comparison);
            }
        }

        public bool IsHandlingInputNow()
        {
            bool handlingInput = false;
            if (m_supportListBoxMode)
            {
                if (IsMouseOver() || m_hasKeyboardActiveControl)
                {
                    handlingInput = true;
                }
            }
            else
            {
                handlingInput = m_isOpen;
            }

            return handlingInput;
        }
        //  Selects item by index, so when you want to make first item as selected call SelectItemByIndex(0)
        public void SelectItemByIndex(int index)
        {
            m_selected = m_items[index];
            SetScrollBarPositionByIndex(index);
        }
        public void SetKeyboardActiveControl(bool value)
        {
            m_hasKeyboardActiveControl = value;
        }
        //  Selects item by key
        public void SelectItemByKey(int key)
        {
            for (int i = 0; i < m_items.Count; i++)
            {
                MyGuiControlComboboxItem item = m_items[i];

                if (item.Key.Equals(key))
                {
                    m_selected = item;
                    m_preselectedKeyboardIndex = i;
                    SetScrollBarPositionByIndex(i);
                    //if (m_showScrollBar == true && m_supportSmoothScroll == false) ScrollToPreSelectedItem();
                    if (OnSelect != null) OnSelect();
                    return;
                }
            }
        }

        //  Return key of selected item
        public int GetSelectedKey()
        {
            if (m_selected == null)
                return -1;
            return m_selected.Key;
        }

        public int GetSelectedIndex() 
        {
            if (m_selected == null)
                return -1;
            return m_items.IndexOf(m_selected);
        }

        //  Return value of selected item
        public StringBuilder GetSelectedValue()
        {
            return m_selected.Value;
        }

        void Assert()
        {
            //  If you forget to set default or pre-selected item, you must do it! It won't be assigned automaticaly!
            MyCommonDebugUtils.AssertDebug(m_selected != null);

            //  Combobox can't be empty!
            MyCommonDebugUtils.AssertDebug(m_items.Count > 0);
        }

        private void SwitchComboboxMode()
        {
            //disallow closing of combobox when we r in listbox mode flag on
            if (m_isOpen && m_supportListBoxMode)
                return;

            if (m_scrollBarDragging == false)
            {
                m_isOpen = !m_isOpen;
            }
        }

        //  Method returns true if input was captured by control, so no other controls, nor screen can use input in this update
        public override bool HandleInput(MyGuiInput input, bool hasKeyboardActiveControl, bool hasKeyboardActiveControlPrevious, bool receivedFocusInThisUpdate)
        {
            bool captureInput = base.HandleInput(input, hasKeyboardActiveControl, hasKeyboardActiveControlPrevious, receivedFocusInThisUpdate);

            //Assert();

            if (captureInput == false)
            {
                if ((IsMouseOver() == true) && (input.IsNewLeftMousePressed() == true) && (m_supportListBoxMode == false && !m_isOpen) && m_scrollBarDragging == false)
                    return true;
                // Make sure here, that whenever not in listbox mode, switch opened/closed state when clicked on combobobox
                //if ((IsMouseOver() == true) && (input.IsNewLeftMouseReleased() == true) && (m_supportListBoxMode == false) && m_scrollBarDragging == false)
                if (input.IsNewLeftMouseReleased() && !m_supportListBoxMode && !m_scrollBarDragging) 
                {
                    if (IsMouseOver() && !m_isOpen || IsMouseOverSelectedItem() && m_isOpen) 
                    {
                        MyAudio.AddCue2D(MySoundCuesEnum.GuiMouseClick);
                        SwitchComboboxMode();
                        captureInput = true;
                    }
                }

                if ((hasKeyboardActiveControl == true) && ((input.IsNewKeyPress(Keys.Enter)) || (input.IsNewKeyPress(Keys.Space) || (input.IsNewGameControlPressed(MyGameControlEnums.FIRE_PRIMARY) && !input.IsNewLeftMousePressed()))))
                {
                    MyAudio.AddCue2D(MySoundCuesEnum.GuiMouseClick);

                    if ((m_preselectedKeyboardIndex.HasValue) && (m_preselectedKeyboardIndex.Value < m_items.Count))
                    {
                        if (m_isOpen == false)
                        {
                            SetScrollBarPositionByIndex(m_selected.Key);
                        }
                        else
                        {
                            SelectItemByKey(m_items[m_preselectedKeyboardIndex.Value].Key);
                        }
                    }

                    //  Close but capture focus for this update so parent screen don't receive this ENTER
                    SwitchComboboxMode();
                    captureInput = true;
                }

                //  In listbox mode, the list is always in opened state
                if (m_isOpen == true)
                {                   
                    #region Handle mouse and scrollbar interaction
                    if (m_showScrollBar == true && input.IsLeftMousePressed() == true)
                    {
                        //  Handles mouse input of dragging the scrollbar up or down
                        Vector2 position = GetDrawPosition();
                        MyRectangle2D openedArea = GetOpenedArea();
                        float minX = position.X + m_size.Value.X - m_scrollBarWidth;
                        float maxX = position.X + m_size.Value.X;
                        float minY = m_supportListBoxMode == false ? position.Y + m_size.Value.Y / 2.0f : position.Y - m_size.Value.Y / 2.0f;
                        float maxY = minY + openedArea.Size.Y;

                        // if we are already scrolling, the area used for scrollbar moving will be extended to whole screen
                        if (m_scrollBarDragging)
                        {
                            minX = 0;
                            maxX = 1;
                            minY = 0;
                            maxY = 1;
                        }

                        // In case mouse cursor is intersecting scrollbar area, start scroll bar dragging mode
                        if ((MyGuiManager.MouseCursorPosition.X >= minX) && (MyGuiManager.MouseCursorPosition.X <= maxX)
                            && (MyGuiManager.MouseCursorPosition.Y >= minY) && (MyGuiManager.MouseCursorPosition.Y <= maxY))
                        {                           
                            // Are we over thee scroll bar handle?
                            float P0 = m_scrollBarCurrentPosition.Value + (openedArea.LeftTop.Y);
                            if (MyGuiManager.MouseCursorPosition.Y > P0 && MyGuiManager.MouseCursorPosition.Y < P0 + m_scrollBarHeight)
                            {
                                if (m_mousePositionReinit)
                                {
                                    m_mouseOldPosition = MyGuiManager.MouseCursorPosition.Y;
                                    m_mousePositionReinit = false;
                                }

                                float mdeff = MyGuiManager.MouseCursorPosition.Y - m_mouseOldPosition;
                                if (mdeff > float.Epsilon || mdeff < float.Epsilon)
                                {
                                    SetScrollBarPosition(m_scrollBarCurrentNonadjustedPosition + mdeff);
                                }

                                m_mouseOldPosition = MyGuiManager.MouseCursorPosition.Y;
                            }
                            else
                            {
                                // If we are not over the scrollbar handle -> jump:
                                float scrollPositionY = MyGuiManager.MouseCursorPosition.Y - (openedArea.LeftTop.Y) - m_scrollBarHeight / 2.0f;
                                SetScrollBarPosition(scrollPositionY);
                            }

                            m_scrollBarDragging = true;
                        }
                    }
                    #endregion

                    // Reset mouse parameters after it was released now
                    if (input.IsNewLeftMouseReleased())
                    {
                        m_mouseOldPosition = MyGuiManager.MouseCursorPosition.Y;
                        m_mousePositionReinit = true;
                    }

                    //  Don't try close combobox if listbox mode is suported.
                    if (m_supportListBoxMode == false)
                    {
                        //  If ESC was pressed while combobox has keyboard focus and combobox was opened, then close combobox but don't send this ESC to parent screen
                        //  Or if user clicked outside of the combobox's area
                        if (((hasKeyboardActiveControl == true) && (input.IsNewKeyPress(Keys.Escape) || input.IsJoystickButtonNewPressed(MyJoystickButtonsEnum.J02))) ||
                            ((IsMouseOverOnOpenedArea() != true) && (IsMouseOver() != true) && (input.IsNewLeftMouseReleased() == true)))
                        {
                            MyAudio.AddCue2D(MySoundCuesEnum.GuiMouseClick);
                            m_isOpen = false;
                        }

                        //  Still capture focus, don't allow parent screen to receive this ESCAPE
                        captureInput = true;
                    }

                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    //  Mouse controling items in the combobox
                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    if (m_scrollBarDragging == false)
                    {
                        #region Handle item that is under mouse cursor
                        //  Search for item that is under the mouse cursor
                        m_preselectedMouseOverPrevious = m_preselectedMouseOver;
                        m_preselectedMouseOver = null;

                        //  The following are used for controlling scroll window range
                        int startIndex = 0;
                        int endIndex = m_items.Count;
                        float widthOffSet = 0f;
                        if (m_showScrollBar == true)
                        {
                            if (m_supportSmoothScroll == true)
                            {
                                //  Extend the display items range by 1(top and bottom) in smooth scrolling mode because of the scenario of partial rendering
                                startIndex = Math.Max(0, m_displayItemsStartIndex - 1);
                                endIndex = Math.Min(m_items.Count, endIndex + 1);
                            }
                            else
                            {
                                startIndex = m_displayItemsStartIndex;
                                endIndex = m_displayItemsEndIndex;
                            }
                            widthOffSet = 0.025f;
                        }

                        for (int i = startIndex; i < endIndex; i++)
                        {
                            Vector2 position = GetOpenItemPosition(i - m_displayItemsStartIndex);
                            MyRectangle2D openedArea = GetOpenedArea();
                            Vector2 min = new Vector2(position.X, Math.Max(openedArea.LeftTop.Y, position.Y));
                            Vector2 max = min + new Vector2(m_size.Value.X - widthOffSet, m_comboboxItemDeltaHeight);

                            if ((MyGuiManager.MouseCursorPosition.X >= min.X) && (MyGuiManager.MouseCursorPosition.X <= max.X) && (MyGuiManager.MouseCursorPosition.Y >= min.Y) && (MyGuiManager.MouseCursorPosition.Y <= max.Y))
                            {
                                m_preselectedMouseOver = m_items[i];

                                //  Auto snap(scroll) current pre-selected/selected item to be fully visible 
                                if (m_supportSmoothScroll == true)
                                {
                                    if (position.Y + 0.001f < openedArea.LeftTop.Y)
                                    {
                                        float scrollUpBy = m_scrollBarCurrentPosition.Value;
                                        scrollUpBy -= 0.001f;
                                        SetScrollBarPosition(scrollUpBy);
                                    }
                                    else if (position.Y + m_comboboxItemDeltaHeight - 0.001f > openedArea.LeftTop.Y + openedArea.Size.Y)
                                    {
                                        float scrollDownBy = m_scrollBarCurrentPosition.Value;
                                        scrollDownBy += 0.001f;
                                        SetScrollBarPosition(scrollDownBy);
                                    }
                                }
                            }
                        }

                        if (m_preselectedMouseOver != null && m_preselectedMouseOver != m_preselectedMouseOverPrevious)
                        {
                            MyAudio.AddCue2D(MySoundCuesEnum.GuiMouseOver);
                        }

                        //  To be used to check for mouse double click action
                        m_doubleClickTimer += MyConstants.PHYSICS_STEP_SIZE_IN_MILLISECONDS;

                        #endregion

                        #region Selecting item in opened combobox area
                        //  Select item when user clicks on it
                        if (input.IsNewLeftMouseReleased() == true && m_preselectedMouseOver != null)
                        {                            
                            //  Checks for double click, only listbox mode supports this type of input
                            if (m_supportListBoxMode == true && m_preselectedMouseOver.Key.Equals(m_selected.Key) == true && m_doubleClickTimer < DOUBLE_CLICK_DELAY)
                            {
                                if (OnSelectItemDoubleClick != null)
                                    OnSelectItemDoubleClick();
                            }
                            m_doubleClickTimer = 0;

                            //m_selected = m_preselectedMouseOver;
                            SelectItemByKey(m_preselectedMouseOver.Key);

                            MyAudio.AddCue2D(MySoundCuesEnum.GuiMouseClick);
                            if (m_supportListBoxMode == false) m_isOpen = false;

                            //  Still capture focus, don't allow parent screen to receive this CLICK
                            captureInput = true;                                                   
                        }
                        #endregion

                        #region Keyboard and scrollwheel controlling items in combobox

                        if (hasKeyboardActiveControl == true || IsMouseOverOnOpenedArea())
                        {
                            if (m_mouseWheelValueLast == null) m_mouseWheelValueLast = input.MouseScrollWheelValue();

                            if (input.MouseScrollWheelValue() < m_mouseWheelValueLast)
                            {
                                HandleItemMovement(true);
                                captureInput = true;
                            }
                            else if (input.MouseScrollWheelValue() > m_mouseWheelValueLast)
                            {
                                HandleItemMovement(false);
                                captureInput = true;
                            }

                            //  Keyboard and mouse movement
                            if (input.IsNewKeyPress(Keys.Down) || input.IsNewGamepadKeyDownPressed())
                            {
                                HandleItemMovement(true);
                                captureInput = true;
                            }
                            else if (input.IsNewKeyPress(Keys.Up) || input.IsNewGamepadKeyUpPressed())
                            {
                                HandleItemMovement(false);
                                captureInput = true;
                            }
                            else if (input.IsNewKeyPress(Keys.PageDown))
                            {
                                HandleItemMovement(true, true);
                            }
                            else if (input.IsNewKeyPress(Keys.PageUp))
                            {
                                HandleItemMovement(false, true);
                            }
                            else if (input.IsNewKeyPress(Keys.Home))
                            {
                                HandleItemMovement(true, false, true);
                            }
                            else if (input.IsNewKeyPress(Keys.End))
                            {
                                HandleItemMovement(false, false, true);
                            }
                            else if (input.IsNewKeyPress(Keys.Tab))
                            {
                                //  We want to close the combobox without selecting any item and forward TAB or SHIF+TAB to parent screen so it can navigate to next control
                                if (m_supportListBoxMode == false && m_isOpen) SwitchComboboxMode();
                                captureInput = false;
                            }

                            m_mouseWheelValueLast = input.MouseScrollWheelValue();
                        }
                        #endregion
                    }
                    else
                    {
                        // When finished scrollbar dragging, set it to false and enable input capturing again
                        if (input.IsNewLeftMouseReleased()) m_scrollBarDragging = false;
                        captureInput = true;
                    }
                }
            }

            return captureInput;
        }

        //  Moves keyboard index to the next item, or previous item, or first item in the combobox.
        //  forwardMovement -> set to TRUE when you want forward movement, set to FALSE when you wasnt backward
        void HandleItemMovement(bool forwardMovement, bool page = false, bool list = false)
        {
            m_preselectedKeyboardIndexPrevious = m_preselectedKeyboardIndex;

            int step = 0;
            if (list && forwardMovement) // first item
            {
                m_preselectedKeyboardIndex = 0;
            }
            else if (list && !forwardMovement) // last item
            {
                m_preselectedKeyboardIndex = m_items.Count - 1;
            }
            else if (page && forwardMovement) // step + 1 page
            {
                if (m_openAreaItemsCount > m_items.Count)
                    step = m_items.Count - 1;
                else
                    step = m_openAreaItemsCount - 1;
            }
            else if (page && !forwardMovement) // step - 1 page
            {
                if (m_openAreaItemsCount > m_items.Count)
                    step = -(m_items.Count - 1);
                else
                    step = -m_openAreaItemsCount + 1;
            }
            else if (!page && !list && forwardMovement) // step 1 item
            {
                step = 1;
            }
            else // step -1 item
            {
                step = -1;
            }


            if (m_preselectedKeyboardIndex.HasValue == false)
            {
                //  If this is first keypress in this combobox, we will set keyboard index to begining or end of the list
                m_preselectedKeyboardIndex = (forwardMovement == true) ? 0 : m_items.Count - 1;
            }
            else
            {
                //  Increase or decrease and than check ranges and do sort of overflow
                m_preselectedKeyboardIndex += step;// sign;
                if (m_preselectedKeyboardIndex > (m_items.Count - 1)) m_preselectedKeyboardIndex = (m_items.Count - 1);
                if (m_preselectedKeyboardIndex < 0) m_preselectedKeyboardIndex = 0;
                /*if (m_preselectedKeyboardIndex > (m_items.Count - 1)) m_preselectedKeyboardIndex = 0;
                if (m_preselectedKeyboardIndex < 0) m_preselectedKeyboardIndex = m_items.Count - 1;*/
            }

            if (m_preselectedKeyboardIndex != m_preselectedKeyboardIndexPrevious)
            {
                MyAudio.AddCue2D(MySoundCuesEnum.GuiMouseOver);
            }

            SetScrollBarPositionByIndex(m_preselectedKeyboardIndex.Value);
        }

        private void SetScrollBarPositionByIndex(int index)
        {
            //  Programmatically adjust the scroll bar position based on changes in m_preselectedKeyboardIndex
            //  So it handles the scrolling action when users press up and down keys
            if (m_showScrollBar == true)
            {
                m_scrollRatio = 0f; //  Reset to zero, since keyboard navigation always does full item movement
                //  These two conditions handle when either
                //  1. the index is at top of the display index range (so scrolls down)
                //  2. the index is at bottom of the display index range (so scrolls up)
                //  3. if neither, then the index is in between the display range, so no scrolling is needed yet
                if (m_preselectedKeyboardIndex >= m_displayItemsEndIndex)
                {
                    m_displayItemsEndIndex = Math.Max(m_openAreaItemsCount, m_preselectedKeyboardIndex.Value + 1);
                    m_displayItemsStartIndex = Math.Max(0, m_displayItemsEndIndex - m_openAreaItemsCount);
                    SetScrollBarPosition(m_preselectedKeyboardIndex.Value * m_maxScrollBarPosition  / (m_items.Count-1), false);
                }
                else if (m_preselectedKeyboardIndex < m_displayItemsStartIndex)
                {
                    m_displayItemsStartIndex = Math.Max(0, m_preselectedKeyboardIndex.Value);
                    m_displayItemsEndIndex = Math.Max(m_openAreaItemsCount, m_displayItemsStartIndex + m_openAreaItemsCount);
                    SetScrollBarPosition(m_preselectedKeyboardIndex.Value * m_maxScrollBarPosition / (m_items.Count - 1), false);
                }
                else if(m_preselectedKeyboardIndex.HasValue)
                {
                    SetScrollBarPosition(m_preselectedKeyboardIndex.Value * m_maxScrollBarPosition / (m_items.Count - 1), false);
                }
            }
        }

        //  Checks if mouse cursor is over opened combobox area
        bool IsMouseOverOnOpenedArea()
        {
            MyRectangle2D openedArea = GetOpenedArea();
            openedArea.Size.Y += m_dropDownItemSize.Y;

            Vector2 min = openedArea.LeftTop;
            Vector2 max = openedArea.LeftTop + openedArea.Size;

            return ((MyGuiManager.MouseCursorPosition.X >= min.X) && (MyGuiManager.MouseCursorPosition.X <= max.X) && (MyGuiManager.MouseCursorPosition.Y >= min.Y) && (MyGuiManager.MouseCursorPosition.Y <= max.Y));
        }

        MyRectangle2D GetOpenedArea()
        {
            MyRectangle2D ret;
            if (m_supportListBoxMode == false)
                ret.LeftTop = m_parent.GetPositionAbsolute() + m_position + new Vector2(-m_dropDownItemSize.X / 2.0f, m_dropDownItemSize.Y / 2.0f);
            else
                ret.LeftTop = m_parent.GetPositionAbsolute() + m_position + new Vector2(-m_dropDownItemSize.X / 2.0f, -m_dropDownItemSize.Y / 2.0f);

            // Adjust the open area to be as big as the scroll bar MAX_VISIBLE_ITEMS_COUNT when scrollbar is on
            if (m_showScrollBar == true || (m_supportListBoxMode == true && m_supportIcon == false))
                ret.Size = new Vector2(m_dropDownItemSize.X, m_openAreaItemsCount * m_comboboxItemDeltaHeight);
            else
                ret.Size = new Vector2(m_dropDownItemSize.X, m_items.Count * m_comboboxItemDeltaHeight);

            return ret;
        }

        //  Returns position of item in open list
        Vector2 GetOpenItemPosition(int index)
        {
            float yOffSet = m_supportListBoxMode == false ? m_dropDownItemSize.Y / 2.0f : -m_dropDownItemSize.Y / 2.0f;

            if (m_supportSmoothScroll == true && m_scrollRatio != 0.0f)
                return GetDrawPosition() + new Vector2(0, yOffSet + index * m_comboboxItemDeltaHeight - (m_scrollRatio - m_displayItemsStartIndex) * m_comboboxItemDeltaHeight) + new Vector2(0, GetTopOffsetSize());
            else
                return GetDrawPosition() + new Vector2(0, yOffSet + index * m_comboboxItemDeltaHeight)  + new Vector2(0, GetTopOffsetSize()); 
        }

        Vector2 GetDrawPosition()
        {
            return m_parent.GetPositionAbsolute() + m_position + new Vector2(-m_dropDownItemSize.X / 2.0f, 0);
        }

        /// <summary>
        /// two phase draw(two SpriteBatch phase):
        /// 1. combobox itself and selected item
        /// 2. opened area and display items draw(if opened area is displayed)
        ///     a. setup up and draw stencil area to stencil buffer for clipping
        ///     b. enable stencil
        ///     c. draw the display items
        ///     d. disable stencil
        /// </summary>
        public override void Draw()
        {
            // In case of listbox mode, before calling parent's draw, reset background color, because it will draw unwanted texture for first item in list(texture, that is used normally for closed combobox)
            Vector4 tempBorderColor = m_backgroundColor.Value;
            if (m_supportListBoxMode == true)
            {
                m_backgroundColor = null;
            }

            base.Draw();
            //Assert();

            m_backgroundColor = tempBorderColor;
            Vector2 position = GetDrawPosition();
            float scrollbarSeparatorPositionX = position.X + m_size.Value.X - m_scrollBarWidth;
            float scrollbarInnerTexturePositionX = position.X + m_size.Value.X - m_scrollBarWidth / 2;
            Vector4 localTextColor = m_textColor;
            Vector4 vectorLineColor = MyGuiConstants.COMBOBOX_VERTICAL_LINE_COLOR;
            Vector4 vectorLineColorWithoutHighlight = MyGuiConstants.COMBOBOX_VERTICAL_LINE_COLOR;

            // Highlighting of combobox parts when mouse over
            if (IsMouseOverOrKeyboardActive())
            {
                localTextColor = localTextColor * MyGuiConstants.CONTROL_MOUSE_OVER_BACKGROUND_COLOR_MULTIPLIER;
                vectorLineColor = vectorLineColor * MyGuiConstants.CONTROL_MOUSE_OVER_BACKGROUND_COLOR_MULTIPLIER;
                tempBorderColor = tempBorderColor * MyGuiConstants.CONTROL_MOUSE_OVER_BACKGROUND_COLOR_MULTIPLIER;
            }
            
            Vector2 textPosition = position + m_textOffset;

            //  The following are used for controlling scroll window range
            int startIndex = 0;
            int endIndex = m_items.Count;
            float widthOffSet = 0f;

            if (m_showScrollBar == true)
            {
                if (m_supportSmoothScroll == true)
                {
                    //  Extend the display items range by 1(top and bottom) in smooth scrolling mode because of the scenario of partial rendering
                    startIndex = Math.Max(0, m_displayItemsStartIndex - 1);
                    endIndex = Math.Min(m_items.Count, endIndex + 1);
                }
                else
                {
                    startIndex = m_displayItemsStartIndex;
                    endIndex = m_displayItemsEndIndex;
                }
                widthOffSet = 0.00f; // scrolbar is now in 
            }
            else
            {
                widthOffSet = 0;
            }

            #region Only non-listbox mode stuff
            if (m_supportListBoxMode == false)
            {
                if (m_selected != null)
                {
                    if (m_supportIcon == true)
                    {
                        //  Selected item's icon
                        if (m_selected.Icon != null)
                            MyGuiManager.DrawSpriteBatchRoundUp(m_selected.Icon, new Vector2(position.X + m_textOffset.X, position.Y),
                                                         m_iconSize, GetColorAfterTransitionAlpha(tempBorderColor),
                                                         MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);

                        textPosition += m_iconTextOffset;
                    }

                    //  End our standard sprite batch
                    MyGuiManager.EndSpriteBatch();

                    //  Draw the rectangle(basically the opened area) to stencil buffer to be used for clipping partial item
                    MyGuiManager.DrawStencilMaskRectangleRoundUp(new MyRectangle2D(position, new Vector2(m_size.Value.X - widthOffSet - (m_showScrollBar == true ? 4f : 2f) * GetGlowSize(), m_comboboxItemDeltaHeight + ITEM_DRAW_DELTA)), MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);

                    //  Set up the stencil operation and parameters
                    MyGuiManager.BeginSpriteBatch_StencilMask();


                    //  Selected item's text
                    MyGuiManager.DrawString(MyGuiManager.GetFontMinerWarsBlue(), m_selected.Value, textPosition,
                                            m_textScale, GetColorAfterTransitionAlpha(localTextColor),
                                            MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);                    

                    //  End stencil-mask batch, and restart the standard sprite batch
                    //MyGuiManager.EndSpriteBatch();
                    MyGuiManager.EndSpriteBatch_StencilMask();
                    MyGuiManager.BeginSpriteBatch();                    
                }
            }
            #endregion

            #region Only when combobox items area is opened
            if (m_isOpen == true)
            {
                //MyGuiManager.ClearStencilBuffer();
                MyRectangle2D openedArea = GetOpenedArea();

                #region Draw Background
                //  Draw background for items - we use DrawBackgroundRectangleForScreen() only because it can draw 
                //  different texture according to aspect ratio of the rectangle
                Vector4 backRectColor = m_backgroundColor.Value * 1.67f;
                backRectColor.W = 1;
                if(m_supportListBoxMode == true) backRectColor.W = 0.7f; // in case of listbox mode, we want area to be bit transparent

                //MyGuiManager.DrawBackgroundRectangleForOpaqueControl(openedArea.LeftTop, openedArea.Size, GetColorAfterTransitionAlpha(backRectColor * (new Color(50, 66, 70, 255)).ToVector4()),
                 //   MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP);

                //  If smooth scrolling is enabled, then stencil buffer is used for clipping
                if (m_supportSmoothScroll == true)
                {
                    //  End our standard sprite batch
                    MyGuiManager.EndSpriteBatch();

                    //  Draw the rectangle(basically the opened area) to stencil buffer to be used for clipping partial item
                    MyGuiManager.DrawStencilMaskRectangle(new MyRectangle2D(openedArea.LeftTop/* + new Vector2(0, 0.01f)*/, openedArea.Size/* - new Vector2(0, 0.02f)*/),
                        MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP);

                    //  Set up the stencil operation and parameters
                    MyGuiManager.BeginSpriteBatch_StencilMask();
                }

                //  To cover up the stencil area's background texture(blank)
                if (m_supportSmoothScroll == true)
                {
                    //MyGuiManager.DrawBackgroundRectangleForScreen(openedArea.LeftTop, openedArea.Size, GetColorAfterTransitionAlpha(m_backgroundColor.Value * 1.5f),
                    //    MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP);
                }
                #endregion                

                #region Draw Combobox Items
                
                //  Draw upper texture of list
                var textureTop = MyGuiManager.GetComboboxTextureTop(m_predefinedSize.Value);
                if (textureTop!=null){
                    MyGuiManager.DrawSpriteBatchRoundUp(textureTop, GetOpenItemPosition(0) - new Vector2(0, GetTopOffsetSize()) - new Vector2(0, 0.02f), new Vector2(m_size.Value.X - widthOffSet, m_comboboxItemDeltaHeight + 0.001f),
                    Color.White, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP);
                }



                for (int i = startIndex; i < endIndex; i++)
                {
                    MyGuiControlComboboxItem item = m_items[i];

                    Vector2 itemPosition = GetOpenItemPosition(i - m_displayItemsStartIndex);
                    float textScale;


                    var texture = MyGuiManager.GetComboboxTextureItem(m_predefinedSize.Value);
                    if (texture != null)
                    {
                        MyGuiManager.DrawSpriteBatchRoundUp(texture, itemPosition, new Vector2(m_size.Value.X - widthOffSet, m_comboboxItemDeltaHeight + ITEM_DRAW_DELTA),
Color.White, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP);
                    }

                    // Draw selected background texture
                    if ((item == m_preselectedMouseOver) || ((m_preselectedKeyboardIndex.HasValue) && (m_preselectedKeyboardIndex == i)))
                    {
                        textScale = m_textScale * MyGuiConstants.BUTTON_MOUSE_OVER_TEXT_SCALE;
                        MyGuiManager.DrawSpriteBatchRoundUp(MyGuiManager.GetBlankTexture(), itemPosition + new Vector2(GetGlowSize(), 0), new Vector2(m_size.Value.X - widthOffSet - (m_showScrollBar == true ? 4f : 2f) * GetGlowSize(), m_comboboxItemDeltaHeight + ITEM_DRAW_DELTA),
                             GetColorAfterTransitionAlpha(MyGuiConstants.COMBOBOX_SELECTED_ITEM_COLOR), MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP);
                    }
                    else
                    {
                        textScale = m_textScale;
                    }

                    textPosition = itemPosition + m_textOffset;

                    if (m_supportIcon == true)
                    {
                        //  Draw combobox item's icon
                        if (item.Icon != null && item.Icon.Height > 0)
                            MyGuiManager.DrawSpriteBatchRoundUp(item.Icon, itemPosition + m_iconPadding, m_iconSize, GetColorAfterTransitionAlpha(m_backgroundColor.Value), MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP);

                        //  Move combobox item's text position to the right of the item's icon
                        textPosition += m_iconTextOffset + new Vector2(0.0f, m_comboboxItemDeltaHeight / 2.0f);
                    }
                    else
                    {
                        textPosition.Y += (m_comboboxItemDeltaHeight / 2.0f);
                    }

                    var itemTextColor = item == m_preselectedMouseOver ? m_textColor * MyGuiConstants.CONTROL_MOUSE_OVER_BACKGROUND_COLOR_MULTIPLIER : m_textColor;
                    
                    //  End our standard sprite batch
                    MyGuiManager.EndSpriteBatch();

                    //  Draw the rectangle(basically the opened area) to stencil buffer to be used for clipping partial item
                    MyGuiManager.DrawStencilMaskRectangle(new MyRectangle2D(itemPosition, new Vector2(m_size.Value.X - widthOffSet - (m_showScrollBar == true ? 4f : 2f) * GetGlowSize(), m_comboboxItemDeltaHeight + ITEM_DRAW_DELTA)), MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP);

                    //  Set up the stencil operation and parameters
                    MyGuiManager.BeginSpriteBatch_StencilMask();

                    //  Draw combobox item's text
                    MyGuiManager.DrawString(MyGuiManager.GetFontMinerWarsBlue(), item.Value, textPosition,
                        textScale, GetColorAfterTransitionAlpha(itemTextColor), MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);
                    
                    //  End stencil-mask batch, and restart the standard sprite batch
                    //MyGuiManager.EndSpriteBatch();
                    MyGuiManager.EndSpriteBatch_StencilMask();
                    MyGuiManager.BeginSpriteBatch();                    
                }

                var textureBottom = MyGuiManager.GetComboboxTextureBottom(m_predefinedSize.Value);
                if (textureBottom != null)
                {
                    MyGuiManager.DrawSpriteBatchRoundUp(textureBottom, GetOpenItemPosition(endIndex - m_displayItemsStartIndex), new Vector2(m_size.Value.X - widthOffSet, m_comboboxItemDeltaHeight),
    Color.White, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP);
                }


                #endregion

                if (m_supportSmoothScroll == true)
                {
                    //  End stencil-mask batch, and restart the standard sprite batch
                    //MyGuiManager.EndSpriteBatch();
                    MyGuiManager.EndSpriteBatch_StencilMask();
                    MyGuiManager.BeginSpriteBatch();
                }

                #region Draw opened combobox bounding lines
               
                /*int linePixelWidth = 3;
                Vector2 screenSize = MyGuiManager.GetScreenSizeFromNormalizedSize(openedArea.Size);
               
                Vector2 screenSizeSlightlyBigger = MyGuiManager.GetScreenSizeFromNormalizedSize(openedArea.Size + new Vector2(0.0021f, 0.0021f));
                Vector2 leftTop = MyGuiManager.GetScreenCoordinateFromNormalizedCoordinate(openedArea.LeftTop);
                Vector2 rightTop = leftTop + new Vector2(screenSize.X, 0);
                Vector2 leftBottom = leftTop + new Vector2(0, screenSize.Y);
                Vector2 rightTopScrollbarOffset = MyGuiManager.GetScreenCoordinateFromNormalizedCoordinate(new Vector2(scrollbarSeparatorPositionX, 0));
                 * */
                Color comboboxBorderColor = GetColorAfterTransitionAlpha(vectorLineColorWithoutHighlight);
                comboboxBorderColor *= 0.6f;
                if (m_supportListBoxMode == false) comboboxBorderColor *=0.8f;
                /*
               // Following sprite drawing is used to draw combobox borders either for listbox or for scrollbar, in case no listbox mode is used
               MyGuiManager.DrawSpriteBatch(MyGuiManager.GetBlankTexture(), (int)leftTop.X, (int)leftTop.Y, (int)screenSize.X + 1, linePixelWidth, comboboxBorderColor);
               MyGuiManager.DrawSpriteBatch(MyGuiManager.GetBlankTexture(), (int)leftBottom.X, (int)leftBottom.Y - linePixelWidth, (int)screenSize.X + 1, linePixelWidth, comboboxBorderColor);
               MyGuiManager.DrawSpriteBatch(MyGuiManager.GetBlankTexture(), (int)leftTop.X, (int)leftTop.Y + linePixelWidth, linePixelWidth, (int)screenSize.Y + 1 - linePixelWidth * 2, comboboxBorderColor);
               MyGuiManager.DrawSpriteBatch(MyGuiManager.GetBlankTexture(), (int)rightTop.X - linePixelWidth, (int)rightTop.Y + linePixelWidth, linePixelWidth, (int)screenSize.Y + 1 - linePixelWidth * 2, comboboxBorderColor);
               */


                if (m_showScrollBar)
                {
                   // MyGuiManager.DrawSpriteBatch(MyGuiManager.GetBlankTexture(), (int)rightTopScrollbarOffset.X - 2, (int)rightTop.Y + linePixelWidth, linePixelWidth, (int)screenSize.Y + 1 - linePixelWidth * 2, comboboxBorderColor);
                }
                #endregion

                #region Draw Scrollbar
                //  Draw the scroll bar
                if (m_showScrollBar == true)
                {
                    MyGuiManager.DrawSpriteBatchRoundUp(MyGuiManager.GetScrollbarSlider(),
                        new Vector2(scrollbarInnerTexturePositionX, openedArea.LeftTop.Y + m_scrollBarCurrentPosition.Value + (m_useScrollBarOffset ? -m_dropDownItemSize.Y / 2.0f : 0)),
                        new Vector2(m_scrollBarWidth, m_scrollBarHeight), MyGuiConstants.COMBOBOX_SCROLLBAR_COLOR, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_TOP);
                }
                #endregion
            }
            #endregion
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //  Scroll Bar logic code
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        void InitializeScrollBarParameters()
        {
            //  Misc
            m_showScrollBar = false;
            m_iconPadding = new Vector2(0.02f, m_comboboxItemDeltaHeight*0.1f);
            m_iconSize = new Vector2(m_comboboxItemDeltaHeight * 0.8f *3f/4f, m_comboboxItemDeltaHeight * 0.8f);

            m_iconTextOffset = new Vector2(m_comboboxItemDeltaHeight, 0f);

            //  Scroll bar size related - this is exact pixel size, so that scrollbar always stays drawn same
            Vector2 scrollbarSize = MyGuiConstants.COMBOBOX_VSCROLLBAR_SIZE;
            m_scrollBarWidth = scrollbarSize.X;
            m_scrollBarHeight = scrollbarSize.Y;

            //  Scroll bar position and range related
            m_scrollBarCurrentPosition = 0;
            m_scrollBarEndPositionRelative = (m_openAreaItemsCount+1) * m_comboboxItemDeltaHeight ;

            //  Display items range index related
            m_displayItemsEndIndex = m_openAreaItemsCount;
        }

        void AdjustScrollBarParameters()
        {
            m_showScrollBar = m_items.Count > m_openAreaItemsCount;
            if (m_showScrollBar == true)
            {
                m_maxScrollBarPosition = m_scrollBarEndPositionRelative - m_scrollBarHeight;
                m_scrollBarItemOffSet = m_items.Count - m_openAreaItemsCount;
            }
        }

        void CalculateStartAndEndDisplayItemsIndex()
        {
            m_scrollRatio = m_scrollBarCurrentPosition.Value == 0 ? 0.0f : m_scrollBarCurrentPosition.Value * m_scrollBarItemOffSet / m_maxScrollBarPosition;
            m_displayItemsStartIndex = Math.Max(0, (int) Math.Floor(m_scrollRatio + 0.5));
            m_displayItemsEndIndex = Math.Min(m_items.Count, m_displayItemsStartIndex + m_openAreaItemsCount);
        }

        public void ScrollToPreSelectedItem()
        {
            if (m_preselectedKeyboardIndex.HasValue == true)
            {
                m_displayItemsStartIndex = m_preselectedKeyboardIndex.Value <= m_middleIndex ?
                    0 : m_preselectedKeyboardIndex.Value - m_middleIndex;

                m_displayItemsEndIndex = m_displayItemsStartIndex + m_openAreaItemsCount;

                if (m_displayItemsEndIndex > m_items.Count)
                {
                    m_displayItemsEndIndex = m_items.Count;
                    m_displayItemsStartIndex = m_displayItemsEndIndex - m_openAreaItemsCount;
                }
                SetScrollBarPosition(m_displayItemsStartIndex * m_maxScrollBarPosition / m_scrollBarItemOffSet);
            }
        }

        void SetScrollBarPosition(float value, bool calculateItemIndexes = true)
        {
            value = MathHelper.Clamp(value, 0, m_maxScrollBarPosition);

            if ((m_scrollBarCurrentPosition.HasValue == false) || (m_scrollBarCurrentPosition.Value != value))
            {
                m_scrollBarCurrentNonadjustedPosition = value;
                m_scrollBarCurrentPosition = value;
                if (calculateItemIndexes)
                {
                    CalculateStartAndEndDisplayItemsIndex();
                }
            }
        }

        protected override Vector2 GetPredefinedControlSize()
        {
            if (m_predefinedSize.HasValue)
            {
                return GetPredefinedControlSize(m_predefinedSize.Value);
            }            

            return Vector2.Zero;
        }

        public static Vector2 GetPredefinedControlSize(MyGuiControlPreDefinedSize preDefinedSize)
        {
            switch (preDefinedSize)
            {
                case MyGuiControlPreDefinedSize.LARGE:
                    return MyGuiConstants.COMBOBOX_LARGE_SIZE;
                case MyGuiControlPreDefinedSize.MEDIUM:
                    return MyGuiConstants.COMBOBOX_MEDIUM_SIZE;
                case MyGuiControlPreDefinedSize.LONGMEDIUM:
                    return MyGuiConstants.COMBOBOX_LONGMEDIUM_SIZE;
                case MyGuiControlPreDefinedSize.SMALL:
                    return MyGuiConstants.COMBOBOX_SMALL_SIZE;
                default:
                    throw new IndexOutOfRangeException("undefined for this size");
            }
        }

        protected float GetGlowSize()
        {
            float size = 0.0f;
            if (m_predefinedSize.HasValue)
            {
                if (m_predefinedSize == MyGuiControlPreDefinedSize.LARGE)
                {
                    size = MyGuiConstants.COMBOBOX_MEDIUM_GLOW_SIZE;
                }
                else if (m_predefinedSize == MyGuiControlPreDefinedSize.MEDIUM)
                {
                    size = MyGuiConstants.COMBOBOX_MEDIUM_GLOW_SIZE;
                }
                else if (m_predefinedSize == MyGuiControlPreDefinedSize.LONGMEDIUM)
                {
                    size = MyGuiConstants.COMBOBOX_MEDIUM_GLOW_SIZE;
                }
                else
                {
                    size = MyGuiConstants.COMBOBOX_MEDIUM_GLOW_SIZE;
                }
            }

            return size;
        }


        protected float GetTopOffsetSize()
        {
            float size = 0.0f;
            if (m_predefinedSize.HasValue)
            {
                if (m_predefinedSize == MyGuiControlPreDefinedSize.LARGE)
                {
                    //size = MyGuiConstants.COMBOBOX_MEDIUM_DROPBOX_TOP_OFFSET;
                }
                else if (m_predefinedSize == MyGuiControlPreDefinedSize.MEDIUM)
                {
                    size = MyGuiConstants.COMBOBOX_MEDIUM_DROPBOX_TOP_OFFSET;
                }
                else if (m_predefinedSize == MyGuiControlPreDefinedSize.LONGMEDIUM)
                {
                    size = MyGuiConstants.COMBOBOX_LONGMEDIUM_DROPBOX_TOP_OFFSET;
                    size = 0.025f;
                }
                else
                {
                    //size = MyGuiConstants.COMBOBOX_MEDIUM_DROPBOX_TOP_OFFSET;
                }
            }

            return size;
        }

        protected Vector2 GetItemSize()
        {
            Vector2 size = Vector2.Zero;
            if (m_predefinedSize.HasValue)
            {
                if (m_predefinedSize == MyGuiControlPreDefinedSize.LARGE)
                {
                    size = MyGuiConstants.COMBOBOX_LARGE_SIZE;
                }
                else if (m_predefinedSize == MyGuiControlPreDefinedSize.MEDIUM)
                {
                    size = MyGuiConstants.COMBOBOX_MEDIUM_ELEMENT_SIZE;
                }
                else if (m_predefinedSize == MyGuiControlPreDefinedSize.LONGMEDIUM)
                {
                    size = MyGuiConstants.COMBOBOX_LONGMEDIUM_ELEMENT_SIZE;
                }
                else
                {
                    size = MyGuiConstants.COMBOBOX_SMALL_SIZE;
                }
            }

            return size;
        }



        protected override bool CheckMouseOver()
        {
            if (m_isOpen == true || m_supportListBoxMode == true)
//          if (m_isOpen == true && m_supportListBoxMode == true)
            {
                float width = m_showScrollBar ? 0 : 0;
                for (int i = 0; i < m_openAreaItemsCount; i++)
                {
                    Vector2 position = GetOpenItemPosition(i);
                    MyRectangle2D openedArea = GetOpenedArea();
                    Vector2 min = new Vector2(position.X, Math.Max(openedArea.LeftTop.Y, position.Y));
                    Vector2 max = min + new Vector2(m_size.Value.X - width, m_comboboxItemDeltaHeight);

                    if (((MyGuiManager.MouseCursorPosition.X >= min.X) && (MyGuiManager.MouseCursorPosition.X <= max.X) && (MyGuiManager.MouseCursorPosition.Y >= min.Y) && (MyGuiManager.MouseCursorPosition.Y <= max.Y)))
                    {
                        return true;
                    }
                }
            }

            if (m_scrollBarDragging) return false;

            return base.CheckMouseOver(m_size.Value, m_parent.GetPositionAbsolute());
        }

        private bool IsMouseOverSelectedItem()
        {
            Vector2 position = GetDrawPosition();
            Vector2 topLeft = position - new Vector2(0f, m_size.Value.Y / 2f);
            Vector2 bottomRight = topLeft + m_size.Value;

            return ((MyGuiManager.MouseCursorPosition.X >= topLeft.X) && (MyGuiManager.MouseCursorPosition.X <= bottomRight.X) && (MyGuiManager.MouseCursorPosition.Y >= topLeft.Y) && (MyGuiManager.MouseCursorPosition.Y <= bottomRight.Y));
        }

        public override void ShowToolTip()
        {
            MyToolTips tempTooltip = m_toolTip;
            if (m_isOpen && IsMouseOverOnOpenedArea() && m_preselectedMouseOver != null)
            {
                m_toolTip = m_preselectedMouseOver.ToolTip;
            }            
            base.ShowToolTip();
            m_toolTip = tempTooltip;
        }
    }
}
