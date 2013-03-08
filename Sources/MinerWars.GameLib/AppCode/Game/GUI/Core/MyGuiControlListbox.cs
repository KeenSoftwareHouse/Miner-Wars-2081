using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWarsMath.Graphics;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.Localization;
using MinerWarsMath;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Audio;
using MinerWars.AppCode.Toolkit.Input;
using SysUtils.Utils;

namespace MinerWars.AppCode.Game.GUI.Core
{    
    delegate void OnListboxItemSelect(object sender, MyGuiControlListboxItemEventArgs eventArgs);
    delegate void OnListboxItemDoubleClick(object sender, MyGuiControlListboxItemEventArgs eventArgs);
    delegate void OnListboxItemDrag(object sender, MyGuiControlListboxItemEventArgs eventArgs);
    class MyGuiControlListbox : MyGuiControlBase
    {
        #region fields
        public Vector2 IconScale = Vector2.One;

        private readonly int m_columns;
        private int m_itemsCount;
        private Vector2 m_itemSize;
        private MyTexture2D m_itemTexture;
        private Vector4 m_itemColor;

        private List<MyGuiControlListboxRow> m_rows;
        private int? m_selectedRowIndex;
        private int? m_selectedItemIndex;
        private int? m_selectedKey;

        private int? m_mousePreselectedRowIndex;
        private int? m_mousePreselectedItemIndex;
        private int? m_preselectedRowIndex;
        private int? m_preselectedItemIndex;
        private int m_rowsOffset;
        private int m_columnsOffset;
        private bool m_isListboxItemDragging;
        private Vector2? m_mouseLastPosition;
        private int? m_previousMousePreselectedRowIndex;
        private int? m_previousMousePreselectedItemIndex;
        private double m_doubleClickTimer;
        private const double DOUBLE_CLICK_DELAY = 500;

        private int m_displayRowsCount;
        private int m_displayColumnsCount;

        private Vector2 m_topLeftPosition;
        private float m_columnWidth;
        private float m_rowHeight;
        private float m_textScale;
        private bool m_supportIcon;

        private int m_borderSize = 4;
        private int m_itemBorderSize = 2;
        private float m_borderSizeListBox;
        private float m_borderSizeItem;
        private float m_paddingTop;
        private float m_paddingRight;
        private float m_paddingBottom;
        private float m_paddingLeft;
        private float m_sliderOffset;
        private float m_sliderOffsetTop;
        private float m_sizeXincrease;
        private float m_sliderLengthIncrease;
        private bool m_useFullSizeicon;
        private Vector2 m_textOffset = MyGuiConstants.LISTBOX_TEXT_OFFSET;
        private MyGuiControlListboxScrollBar m_verticalScrollBar;
        private MyGuiControlListboxScrollBar m_horizontalScrollBar;
        private Vector2 m_innerItemSize;
        private bool m_highlightAllRow;
        private bool m_dontHighlightHeadlineRow;

        private List<float> m_customCollumsWidths = null;

        private List<Point> m_selectedItemIndices;
        #endregion

        #region properties
        public bool MultipleSelection { get; set; }
        public bool DisplayHighlight { get; set; }
        public bool UseFullItemSizeIcon { get; set; }

        public Vector2 TextOffset
        {
            get { return m_textOffset; }
            set { m_textOffset = value; }
        }

        public Vector2 InnerItemSize
        {
            get { return m_innerItemSize; }
            set { m_innerItemSize = value; }
        }

        public MyTexture2D ItemMouseOverTexture { get; set; }

        public bool HighlightHeadline { get; set; }

        public bool ShowScrollBarOnlyWhenNeeded { get; set; }

        #endregion

        #region constructors
        public MyGuiControlListbox(IMyGuiControlsParent parent, Vector2 position, Vector2 size, Vector4 backgroundColor, StringBuilder toolTip, float textScale,
            int columns, int displayRowsCount, int displayColumnsCount, bool supportPicture, bool verticalScrolling, bool horizontalScrolling,
            MyTexture2D backgroundTexture, MyTexture2D itemBackgroundTexture, MyTexture2D verticalScrollBarTexture, MyTexture2D horizontalScrollBarTexture,
            int borderSize, int itemBorderSize, Vector4 itemColor, float paddingTop, float paddingRight, float paddingBottom, float paddingLeft, float itemPaddingRight, float itemPaddingBottom, float sliderOffset, float sliderOffsetTop = 0, float sizeXincrease = 0, float sliderLengthIncrease = 0)
            : base(parent, position, size, backgroundColor, toolTip, backgroundTexture, null, null, true)
        {
            System.Diagnostics.Debug.Assert(columns > 0);
            System.Diagnostics.Debug.Assert(displayRowsCount > 0);
            System.Diagnostics.Debug.Assert(displayColumnsCount > 0);
            m_itemsCount = 0;
            m_doubleClickTimer = 0;
            m_canHandleKeyboardActiveControl = true;
            m_rows = new List<MyGuiControlListboxRow>();
            m_columns = columns;
            m_textScale = textScale;
            m_displayRowsCount = displayRowsCount;
            m_displayColumnsCount = displayColumnsCount;
            m_supportIcon = supportPicture;
            m_itemSize = size;
            m_innerItemSize = size;
            m_topLeftPosition = m_position - m_itemSize / 2.0f;
            m_rowHeight = m_itemSize.Y + itemPaddingBottom;
            m_columnWidth = m_itemSize.X + itemPaddingRight;
            m_borderSize = borderSize;
            m_itemBorderSize = itemBorderSize;
            m_itemTexture = itemBackgroundTexture;
            m_itemColor = itemColor;
            m_paddingTop = paddingTop;
            m_paddingRight = paddingRight;
            m_paddingBottom = paddingBottom;
            m_paddingLeft = paddingLeft;
            m_sliderOffset = sliderOffset;
            m_sliderOffsetTop = sliderOffsetTop;
            m_borderSizeListBox = MyGuiManager.GetNormalizedSizeFromScreenSize(new Vector2(m_borderSize, m_borderSize)).X;
            m_borderSizeItem = MyGuiManager.GetNormalizedSizeFromScreenSize(new Vector2(m_itemBorderSize, m_itemBorderSize)).X;
            m_sizeXincrease = sizeXincrease;
            m_sliderLengthIncrease = sliderLengthIncrease;

            if (verticalScrolling)
            {
                Vector2 scrollBarSize = MyGuiConstants.COMBOBOX_VSCROLLBAR_SIZE;
                Vector2 scrollBarPosition =     m_topLeftPosition
                                            + new Vector2(m_borderSizeListBox, m_borderSizeListBox) +
                                            new Vector2(m_paddingLeft, m_paddingTop) + new Vector2(m_displayColumnsCount * m_columnWidth, 0f) + new Vector2(m_sliderOffset, m_sliderOffsetTop);
                m_verticalScrollBar = new MyGuiControlListboxScrollBar(m_parent, scrollBarPosition, scrollBarSize, m_backgroundColor, MyGuiConstants.LISTBOX_SCROLLBAR_COLOR, MyScrollDirection.Vertical, verticalScrollBarTexture);
                m_verticalScrollBar.ScrollValueChanged += OnScrollValueChanged;
            }

            if (horizontalScrolling)
            {
                Vector2 scrollBarSize = new Vector2(m_displayColumnsCount * m_columnWidth, MyGuiConstants.LISTBOX_SCROLLBAR_WIDTH);
                Vector2 scrollBarPosition = m_topLeftPosition
                    + new Vector2(m_borderSizeListBox, m_borderSizeListBox) + new Vector2(m_paddingLeft, m_paddingTop)
                    + new Vector2(0f, m_rows.Count * m_rowHeight);
                float scrollBarMaxValue = Math.Max((float)m_columns / (float)m_displayColumnsCount, 1.0f);
                m_horizontalScrollBar = new MyGuiControlListboxScrollBar(m_parent, scrollBarPosition, scrollBarSize, m_backgroundColor, MyGuiConstants.LISTBOX_SCROLLBAR_COLOR, MyScrollDirection.Horizontal, horizontalScrollBarTexture);
                m_horizontalScrollBar.ScrollValueChanged += OnScrollValueChanged;
                m_horizontalScrollBar.InitializeScrollBar(scrollBarMaxValue);
            }

            RecalculateRealSize();
            RecalculateRealPosition();

            m_selectedItemIndices = new List<Point>();
            DisplayHighlight = true;
            ShowScrollBarOnlyWhenNeeded = false;
        }

        public MyGuiControlListbox(IMyGuiControlsParent parent, Vector2 position, Vector2 size, Vector4 backgroundColor, StringBuilder toolTip, float textScale, int columns, int displayRowsCount, int displayColumnsCount, bool supportPicture, bool verticalScrolling, bool horizontalScrolling)
            : this(parent, position, size, backgroundColor, toolTip, textScale, columns, displayRowsCount, displayColumnsCount, supportPicture, verticalScrolling, horizontalScrolling,
            null, null, MyGuiManager.GetScrollbarSlider(), MyGuiManager.GetHorizontalScrollbarSlider(), 4, 2, MyGuiConstants.LISTBOX_ITEM_COLOR, 0f, 0f, 0f, 0f, 0, 0, 0)
        {

        }
        #endregion

        #region events and delegates
        public event OnListboxItemSelect ItemSelect;
        public event OnListboxItemDrag ItemDrag;
        public event OnListboxItemDoubleClick ItemDoubleClick;
        #endregion

        #region public methods

        public void SetCustomCollumnsWidths(List<float> widths)
        {
            m_customCollumsWidths = widths;
            RecalculateAfterRowsCountChanged();
        }

        public void EnableAllRowHighlightWhileMouseOver(bool enable, bool exceptForHeadline = false)
        {
            m_highlightAllRow = enable;
            m_dontHighlightHeadlineRow = exceptForHeadline;
        }

        public void ResetScrollbarPosition()
        {

            if (m_verticalScrollBar!=null)
            {
                var verticalScrollposition = m_topLeftPosition
                                             + new Vector2(m_borderSizeListBox, m_borderSizeListBox) +
                                             new Vector2(m_paddingLeft, m_paddingTop) +
                                             new Vector2(m_displayColumnsCount*m_columnWidth, 0f) +
                                             new Vector2(m_sliderOffset, m_sliderOffsetTop);
                m_verticalScrollBar.SetNewPosition(verticalScrollposition);
                m_verticalScrollBar.SetScrollValue(0);
                m_rowsOffset = 0;
            }
            RecalculateRealPosition();
            RecalculateRealSize();

        }

        /// <summary>
        /// Adds new row to the end and returs index of added row
        /// </summary>        
        /// <returns>index of new row</returns>
        public int AddRow(Vector4? color = null)
        {
            return AddRow(m_rows.Count, color);
        }

        /// <summary>
        /// Adds new row to specific index
        /// </summary>
        /// <param name="index">index of added row</param>
        /// <returns>index of new row</returns>
        public int AddRow(int index, Vector4? color)
        {
            m_rows.Insert(index, new MyGuiControlListboxRow(m_columns) { Color = color } );
            RecalculateAfterRowsCountChanged();
            return index;
        }

        /// <summary>
        /// Adds new rows to listbox
        /// </summary>
        /// <param name="count">Count</param>
        public void AddRows(int count)
        {
            for (int i = 0; i < count; i++)
            {
                AddRow();
            }
        }

        /// <summary>
        /// Removes row at index
        /// </summary>
        /// <param name="index">row's index</param>
        public void RemoveRow(int index)
        {
            int removedItems = m_rows[index].ItemsCount();
            m_itemsCount -= removedItems;
            m_rows.RemoveAt(index);

            RecalculateAfterRowsCountChanged();
            FixSelectedIndexes();
        }

        public void RemoveAllRows()
        {
            m_rows.Clear();
            m_itemsCount = 0;

            RecalculateAfterRowsCountChanged();
            FixSelectedIndexes();
        }

        /// <summary>
        /// Adds new item to first free row
        /// </summary>
        /// <param name="key">item's key</param>
        /// <param name="value">item's value</param>
        public MyGuiControlListboxItem AddItem(int key, MyTextsWrapperEnum value)
        {
            return AddItem(key, MyTextsWrapper.Get(value));
        }

        /// <summary>
        /// Adds new item to first free row
        /// </summary>
        /// <param name="key">item's key</param>
        /// <param name="value">item's value</param>
        public MyGuiControlListboxItem AddItem(int key, StringBuilder value)
        {
            return AddItem(key, value, null);
        }

        /// <summary>
        /// Adds new item to first free row
        /// </summary>
        /// <param name="key">item's key</param>
        /// <param name="value">item's value</param>
        /// <param name="icon">item's icon</param>
        public MyGuiControlListboxItem AddItem(int key, StringBuilder value, MyTexture2D icon)
        {
            MyGuiControlListboxItem newItem = CreateListboxItem(key, value, icon);
            AddItem(newItem);
            return newItem;
        }

        /// <summary>
        /// Adds new item to specific row and item's index
        /// </summary>
        /// <param name="key">item's key</param>
        /// <param name="value">item's value</param>
        /// <param name="icon">item's icon</param>
        /// <param name="rowIndex">row's index</param>
        /// <param name="itemIndex">item's index</param>        
        public MyGuiControlListboxItem AddItem(int key, StringBuilder value, MyTexture2D icon, int rowIndex, int itemIndex)
        {
            MyGuiControlListboxItem newItem = CreateListboxItem(key, value, icon);
            AddItem(newItem, rowIndex, itemIndex);
            return newItem;
        }

        /// <summary>
        /// Adds new item to first free row
        /// </summary>
        /// <param name="item">item</param>
        public void AddItem(MyGuiControlListboxItem item)
        {
            // if there are no rows, then add new one and add item there
            if (m_rows.Count == 0)
            {
                AddRow();
                AddItem(item, 0, 0);
            }
            else
            {
                int? emptyRowIndex = null;
                int? emptyItemIndex = null;

                // try find first empty row
                for (int rowIndex = 0; rowIndex < m_rows.Count; rowIndex++)
                {
                    emptyItemIndex = m_rows[rowIndex].GetFirstEmptyItemSlot();
                    if (emptyItemIndex != null)
                    {
                        emptyRowIndex = rowIndex;
                        break;
                    }
                }

                // if not founded, then add new row
                if (emptyRowIndex == null || emptyItemIndex == null)
                {
                    emptyRowIndex = this.AddRow();
                    emptyItemIndex = 0;
                }

                // add item at first empty position
                AddItem(item, emptyRowIndex.Value, emptyItemIndex.Value);
            }
        }

        /// <summary>
        /// Adds new item to specific row and item's index
        /// </summary>
        /// <param name="item">item</param>
        /// <param name="rowIndex">row's index</param>
        /// <param name="itemIndex">item's index</param>
        public void AddItem(MyGuiControlListboxItem item, int rowIndex, int itemIndex)
        {
            MyGuiControlListboxRow row = m_rows[rowIndex];
            row.Items[itemIndex] = item;

            if (item != null)
            {
                m_itemsCount++;
            }
        }

        /// <summary>
        /// Removes item
        /// </summary>
        /// <param name="key">item's key</param>
        public void RemoveItem(int key, bool removeEmptyItems = true)
        {
            foreach (MyGuiControlListboxRow row in m_rows)
            {
                for (int i = 0; i < row.Items.Length; i++)
                {
                    MyGuiControlListboxItem item = row.Items[i];
                    if (item != null && item.Key == key)
                    {
                        row.Items[i] = null;
                        m_itemsCount--;
                        FixAfterRemove(removeEmptyItems);
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Removes item
        /// </summary>
        /// <param name="rowIndex">row's index</param>
        /// <param name="itemIndex">item's index</param>
        public void RemoveItem(int rowIndex, int itemIndex, bool removeEmptyItems = true)
        {
            if (m_rows[rowIndex].Items[itemIndex] != null)
            {
                m_itemsCount--;
            }
            m_rows[rowIndex].Items[itemIndex] = null;
            FixAfterRemove(removeEmptyItems);
        }

        /// <summary>
        /// Removes all items from listbox
        /// </summary>
        public void RemoveAllItems(bool removeEmptyItems = true)
        {
            foreach (MyGuiControlListboxRow row in m_rows)
            {
                for (int i = 0; i < row.Items.Length; i++)
                {
                    row.Items[i] = null;
                }
            }
            m_itemsCount = 0;
            FixAfterRemove(removeEmptyItems);
        }

        /// <summary>
        /// Removes empty items and empty rows
        /// </summary>
        public void RemoveEmptyItems()
        {
            int rowIndex = 0;
            int itemIndex = 0;
            int emptyRowIndex = 0;
            int emptyItemIndex = 0;
            int lastNonEmptyRowIndex = 0;
            int lastNonEmptyItemIndex = 0;
            bool emptyItemFounded = false;

            while (rowIndex < m_rows.Count)
            {
                for (itemIndex = 0; itemIndex < m_columns; itemIndex++)
                {
                    // we looking for empty item
                    if (!emptyItemFounded)
                    {
                        if (m_rows[rowIndex].Items[itemIndex] == null)
                        {
                            emptyRowIndex = rowIndex;
                            emptyItemIndex = itemIndex;
                            emptyItemFounded = true;

                            if (lastNonEmptyRowIndex > emptyRowIndex)
                            {
                                rowIndex = lastNonEmptyRowIndex;
                                itemIndex = lastNonEmptyItemIndex;
                            }
                        }
                    }
                    // we looking for item after empty item
                    else
                    {
                        MyGuiControlListboxItem item = m_rows[rowIndex].Items[itemIndex];
                        if (item != null)
                        {
                            m_rows[emptyRowIndex].Items[emptyItemIndex] = item;
                            m_rows[rowIndex].Items[itemIndex] = null;
                            emptyItemFounded = false;

                            lastNonEmptyRowIndex = rowIndex;
                            lastNonEmptyItemIndex = itemIndex;
                            rowIndex = emptyRowIndex;
                            itemIndex = emptyItemIndex;
                        }
                    }
                }
                rowIndex++;
            }
            RemoveEmptyRows();
        }

        /// <summary>
        /// Removes all empty rows
        /// </summary>
        public void RemoveEmptyRows()
        {
            for (int rowIndex = m_rows.Count - 1; rowIndex >= 0; rowIndex--)
            {
                if (m_rows[rowIndex].IsEmpty())
                {
                    m_rows.RemoveAt(rowIndex);
                }
            }
            RecalculateAfterRowsCountChanged();
            FixSelectedIndexes();
        }

        /// <summary>
        /// Returns item by key
        /// </summary>
        /// <param name="key">item's key</param>
        /// <returns></returns>
        public MyGuiControlListboxItem GetItem(int key)
        {
            foreach (MyGuiControlListboxRow row in m_rows)
            {
                foreach (MyGuiControlListboxItem item in row.Items)
                {
                    if (item != null && item.Key == key)
                    {
                        return item;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Returns item by row's index and item's index
        /// </summary>
        /// <param name="rowIndex">row's index</param>
        /// <param name="itemIndex">imte's index</param>
        /// <returns></returns>
        public MyGuiControlListboxItem GetItem(int rowIndex, int itemIndex)
        {
            if (rowIndex < m_rows.Count && rowIndex >= 0)
            {
                if (itemIndex < m_columns && itemIndex >= 0)
                {
                    return m_rows[rowIndex].Items[itemIndex];
                }
            }
            return null;
        }

        /// <summary>
        /// Returns selected row
        /// </summary>
        /// <returns></returns>
        public MyGuiControlListboxRow GetSelectedRow()
        {
            if (m_selectedRowIndex != null)
            {
                return m_rows[m_selectedRowIndex.Value];
            }
            return null;
        }

        /// <summary>
        /// Returns selected item
        /// </summary>
        /// <returns></returns>
        public MyGuiControlListboxItem GetSelectedItem()
        {
            MyGuiControlListboxRow selectedRow = GetSelectedRow();
            if (selectedRow != null)
            {
                if (m_selectedItemIndex != null)
                {
                    return selectedRow.Items[m_selectedItemIndex.Value];
                }
            }
            return null;
        }

        /// <summary>
        /// Returns selected items, use this method if MultipleSelection is true
        /// </summary>
        public List<MyGuiControlListboxItem> GetSelectedItems()
        {
            List<MyGuiControlListboxItem> items = new List<MyGuiControlListboxItem>();
            foreach (var multiSelectedItem in m_selectedItemIndices)
            {
                if (multiSelectedItem.Y < m_rows.Count)
                {
                    var item = m_rows[multiSelectedItem.Y].Items[multiSelectedItem.X];
                    if (item != null)
                    {
                        items.Add(item);
                    }
                }
            }
            return items;
        }


        public void SelectAll()
        {
            if (MultipleSelection)
            {
                m_selectedItemIndices.Clear();
                for (int i = 0; i < m_columns; i++)
                {
                    for (int j = 0; j < m_rows.Count; j++)
                    {
                        var item = GetItem(j, i);
                        if (item != null)
                        {
                            m_selectedItemIndices.Add(new Point(i, j));
                        }
                    }
                }
            }
        }

        public void SelectRow(int rowToSelect)
        {
            if (MultipleSelection)
            {
                m_selectedItemIndices.Clear();
                for (int i = 0; i < m_columns; i++)
                {
                    var item = GetItem(rowToSelect, i);
                    if (item != null)
                    {
                        m_selectedItemIndices.Add(new Point(i, rowToSelect));
                    }
                }
            }
        }

        public void SelectColumn(int columnToSelect)
        {
            if (MultipleSelection)
            {
                m_selectedItemIndices.Clear();
                for (int j = 0; j < m_rows.Count; j++)
                {
                    var item = GetItem(j, columnToSelect);
                    if (item != null)
                    {
                        m_selectedItemIndices.Add(new Point(columnToSelect, j));
                    }
                }
            }
        }

        public void DeselectAll()
        {
            if (MultipleSelection)
            {
                m_selectedItemIndices.Clear();
                m_preselectedRowIndex = m_selectedRowIndex = null;
                m_preselectedItemIndex = m_selectedItemIndex = null;
            }
        }

        /// <summary>
        /// Returns selected item's key
        /// </summary>
        /// <returns></returns>
        public int? GetSelectedItemKey()
        {
            MyGuiControlListboxRow selectedRow = GetSelectedRow();
            if (selectedRow != null)
            {
                if (m_selectedItemIndex != null)
                {
                    return selectedRow.Items[m_selectedItemIndex.Value].Key;
                }
            }
            return null;
        }

        /// <summary>
        /// Sets selected item
        /// </summary>
        /// <param name="rowIndex">row's index</param>
        /// <param name="itemIndex">item's index</param>
        public void SetSelectedItem(int? rowIndex, int? itemIndex)
        {
            if (rowIndex == null || rowIndex.Value < m_rows.Count && rowIndex.Value > m_rows.Count)
            {
                m_selectedRowIndex = null;
            }
            else
            {
                m_selectedRowIndex = rowIndex;
            }
            m_selectedItemIndex = itemIndex;

            var item = GetSelectedItem();
            if (item != null && item.Enabled)
            {
                m_selectedKey = item.Key;
                if (ItemSelect != null)
                {
                    ItemSelect(this, new MyGuiControlListboxItemEventArgs(m_selectedRowIndex.Value, m_selectedItemIndex.Value, item.Key));
                }
            }
            else
            {
                m_selectedKey = null;
            }
            SetOffsets(m_selectedRowIndex, m_selectedItemIndex, true);
        }

        private bool IsMultipleSelected(int itemIndex, int rowIndex)
        {
            foreach (var item in m_selectedItemIndices)
            {
                if (item.X == itemIndex && item.Y == rowIndex)
                {
                    return true;
                }
            }
            return false;
        }

        private void HandleMultiSelect(bool addSingleItem, bool addMultiple, int? oldSelectedRowIndex, int? oldSelectedItemIndex, int? m_selectedRowIndex, int? m_selectedItemIndex)
        {
            if (!MultipleSelection)
            {
                m_selectedItemIndices.Clear();
                return;
            }

            Point? oldItem = oldSelectedRowIndex.HasValue && oldSelectedItemIndex.HasValue ? new Point(oldSelectedItemIndex.Value, oldSelectedRowIndex.Value) : (Point?)null;
            Point? newItem = m_selectedRowIndex.HasValue && m_selectedItemIndex.HasValue ? new Point(m_selectedItemIndex.Value, m_selectedRowIndex.Value) : (Point?)null;

            if (addSingleItem)
            {
                if (newItem.HasValue)
                {
                    if (m_selectedItemIndices.Contains(newItem.Value))
                    {
                        m_selectedItemIndices.Remove(newItem.Value);

                        SetSelectedItem(null, null);
                    }
                    else
                    {
                        m_selectedItemIndices.Add(newItem.Value);
                    }

                }
            }
            else if (addMultiple)
            {
                if (oldItem.HasValue && newItem.HasValue)
                {
                    Point? start = oldItem;
                    Point? end = newItem;

                    // Switch if selection start after selection end
                    if (oldItem.Value.Y > newItem.Value.Y || (oldItem.Value.Y == newItem.Value.Y && oldItem.Value.X > newItem.Value.X))
                    {
                        start = newItem;
                        end = oldItem;
                    }

                    m_selectedItemIndices.Clear();
                    if (start.Value.Y == end.Value.Y)
                    {
                        // same row
                        for (int i = start.Value.X; i <= end.Value.X; i++)
                        {
                            var item = GetItem(start.Value.Y, i);
                            if (item != null) m_selectedItemIndices.Add(new Point(i, start.Value.Y));
                        }
                    }
                    else
                    {
                        // Start item to end of row
                        for (int i = start.Value.X; i < m_columns; i++)
                        {
                            var item = GetItem(start.Value.Y, i);
                            if (item != null) m_selectedItemIndices.Add(new Point(i, start.Value.Y));
                        }

                        // whole rows
                        for (int i = start.Value.Y + 1; i <= end.Value.Y - 1; i++)
                        {
                            for (int j = 0; j < m_columns; j++)
                            {
                                var item = GetItem(i, j);
                                if (item != null) m_selectedItemIndices.Add(new Point(j, i));
                            }
                        }

                        // Start item to end of row
                        for (int i = 0; i <= end.Value.X; i++)
                        {
                            var item = GetItem(end.Value.Y, i);
                            if (item != null) m_selectedItemIndices.Add(new Point(i, end.Value.Y));
                        }
                    }
                }
            }
            else
            {
                m_selectedItemIndices.Clear();
                if (newItem.HasValue)
                {
                    m_selectedItemIndices.Add(newItem.Value);
                }
            }
        }


        /// <summary>
        /// Sets selected item
        /// </summary>
        /// <param name="key">item's key</param>
        public void SetSelectedItem(int key)
        {
            for (int rowIndex = 0; rowIndex < m_rows.Count; rowIndex++)
            {
                for (int itemIndex = 0; itemIndex < m_columns; itemIndex++)
                {
                    if (m_rows[rowIndex].Items[itemIndex] != null &&
                       m_rows[rowIndex].Items[itemIndex].Key == key)
                    {
                        SetSelectedItem(rowIndex, itemIndex);
                        return;
                    }
                }
            }
            SetSelectedItem(null, null);
        }

        /// <summary>
        /// Returns position (item's index and row's index) under the mouses's cursor
        /// </summary>
        /// <returns></returns>
        public Point? GetMouseOverIndexes()
        {
            Vector2 leftTopPosition = GetLeftTopPosition() + new Vector2(m_borderSizeListBox, m_borderSizeListBox) + new Vector2(m_paddingLeft, m_paddingTop);
            float mouseXPositionRelative = MyGuiManager.MouseCursorPosition.X - leftTopPosition.X;
            float mouseYPositionRelative = MyGuiManager.MouseCursorPosition.Y - leftTopPosition.Y;
            Vector2 itemsAreaSize = GetItemsAreaSizeForMouse();
            float mouseXRelativeRatio = mouseXPositionRelative / itemsAreaSize.X;
            float mouseYRelativeRatio = mouseYPositionRelative / itemsAreaSize.Y;
            int currentMouseRowIndex = (int)Math.Floor(mouseYRelativeRatio * m_displayRowsCount);

            int currentMouseItemIndex = (int)Math.Floor(mouseXRelativeRatio * m_displayColumnsCount);
            if (m_customCollumsWidths != null)
            {
                currentMouseItemIndex = 0;
                float offset = 0;
                for (int i = 0; i <= m_displayColumnsCount; i++)
                {
                    if (offset > mouseXPositionRelative)
                    {
                        currentMouseItemIndex--;
                        break;
                    }
                    else
                    {
                        currentMouseItemIndex++;
                        if(m_customCollumsWidths.Count > i && m_customCollumsWidths[i] > 0){
                            offset+=m_customCollumsWidths[i];
                        }
                        else{
                            offset += m_itemSize.X;
                        }
                    }
                }
            }

            if (currentMouseRowIndex >= 0 && currentMouseRowIndex < m_displayRowsCount &&
               currentMouseItemIndex >= 0 && currentMouseItemIndex < m_displayColumnsCount)
            {
                return new Point(currentMouseItemIndex + m_columnsOffset, currentMouseRowIndex + m_rowsOffset);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Returns row's count
        /// </summary>
        /// <returns></returns>
        public int GetRowsCount()
        {
            return m_rows.Count;
        }

        /// <summary>
        /// Returns column's count
        /// </summary>
        /// <returns></returns>
        public int GetColumsCount()
        {
            return m_columns;
        }

        /// <summary>
        /// Returns all items
        /// </summary>
        /// <returns></returns>
        public List<MyGuiControlListboxItem> GetItems()
        {
            List<MyGuiControlListboxItem> items = new List<MyGuiControlListboxItem>();
            foreach (MyGuiControlListboxRow row in m_rows)
            {
                foreach (MyGuiControlListboxItem item in row.Items)
                {
                    if (item != null)
                    {
                        items.Add(item);
                    }
                }
            }

            return items;
        }

        /// <summary>
        /// Returns all items keys
        /// </summary>
        /// <returns></returns>
        public List<int> GetItemsKeys()
        {
            List<int> keys = new List<int>();
            foreach (MyGuiControlListboxRow row in m_rows)
            {
                foreach (MyGuiControlListboxItem item in row.Items)
                {
                    if (item != null)
                    {
                        keys.Add(item.Key);
                    }
                }
            }

            return keys;
        }

        /// <summary>
        /// Returns item's count
        /// </summary>
        /// <returns></returns>
        public int GetItemsCount()
        {
            //int count = 0;
            //foreach (MyGuiControlListboxRow row in m_rows)
            //{
            //    foreach (MyGuiControlListboxItem item in row.Items)
            //    {
            //        if (item != null)
            //        {
            //            count++;
            //        }
            //    }
            //}

            //return count;
            return m_itemsCount;
        }

        /// <summary>
        /// Returns empty item's count
        /// </summary>
        /// <returns></returns>
        public int GetEmptyItemsCount()
        {
            //int count = 0;
            //foreach (MyGuiControlListboxRow row in m_rows)
            //{
            //    foreach (MyGuiControlListboxItem item in row.Items)
            //    {
            //        if (item == null)
            //        {
            //            count++;
            //        }
            //    }
            //}

            //return count;
            return m_rows.Count * m_columns - m_itemsCount;
        }

        /// <summary>
        /// Returns count of rows without any item
        /// </summary>
        /// <returns></returns>
        public int GetEmptyRowsCount()
        {
            int count = 0;
            foreach (MyGuiControlListboxRow row in m_rows)
            {
                if (row.IsEmpty())
                {
                    count++;
                }
            }

            return count;
        }

        #endregion

        #region overriden methods
        public override void SetPosition(Vector2 position)
        {
            base.SetPosition(position);
            RecalculateRealPosition();
            //base.SetPosition(position);
        }

        public override bool HandleInput(MyGuiInput input, bool hasKeyboardActiveControl, bool hasKeyboardActiveControlPrevious, bool receivedFocusInThisUpdate)
        {
            bool captureInput = base.HandleInput(input, hasKeyboardActiveControl, hasKeyboardActiveControlPrevious, receivedFocusInThisUpdate);

            if (Enabled && !captureInput && m_rows.Count > 0)
            {
                #region handle scrollbar
                if (m_verticalScrollBar != null)
                {
                    if (m_verticalScrollBar.HandleInput(input, hasKeyboardActiveControl, hasKeyboardActiveControlPrevious, receivedFocusInThisUpdate))
                    {
                        captureInput = true;
                    }
                }
                if (m_horizontalScrollBar != null)
                {
                    if (m_horizontalScrollBar.HandleInput(input, hasKeyboardActiveControl, hasKeyboardActiveControlPrevious, receivedFocusInThisUpdate))
                    {
                        captureInput = true;
                    }
                }
                #endregion

                // handle only if mouse is over the control
                if (IsMouseOver() && !captureInput)
                {
                    #region handle mouse
                    HandleCurrentMousePreselected();
                    m_doubleClickTimer += MyConstants.PHYSICS_STEP_SIZE_IN_MILLISECONDS;

                    // selection by mouse
                    var preselectedItem = GetMousePreselectedItem();
                    if (input.IsNewLeftMousePressed())
                    {
                        if (preselectedItem != null && preselectedItem.Enabled)
                        {
                            // double click handle                            
                            if (m_doubleClickTimer <= DOUBLE_CLICK_DELAY && m_mouseLastPosition != null && (m_mouseLastPosition.Value - MyGuiManager.MouseCursorPosition).Length() <= 0.005f)
                            {
                                if (ItemDoubleClick != null)
                                {
                                    ItemDoubleClick(this, new MyGuiControlListboxItemEventArgs(m_mousePreselectedRowIndex.Value, m_mousePreselectedItemIndex.Value, preselectedItem.Key));
                                    MyAudio.AddCue2D(MySoundCuesEnum.GuiMouseClick);
                                    MyAudio.AddCue2D(MySoundCuesEnum.GuiMouseClick);
                                }
                            }
                            // click handle
                            else
                            {
                                int? oldSelectedRowIndex = m_selectedRowIndex;
                                int? oldSelectedItemIndex = m_selectedItemIndex;

                                SetSelectedItem(m_mousePreselectedRowIndex, m_mousePreselectedItemIndex);
                                HandleMultiSelect(input.IsAnyCtrlKeyPressed(), input.IsAnyShiftKeyPressed(),
                                    oldSelectedRowIndex, oldSelectedItemIndex, m_selectedRowIndex, m_selectedItemIndex);

                                MyAudio.AddCue2D(MySoundCuesEnum.GuiMouseClick);
                                ResetPreselected();
                            }

                            m_mouseLastPosition = MyGuiManager.MouseCursorPosition;
                            m_doubleClickTimer = 0;
                            m_isListboxItemDragging = true;
                            captureInput = true;
                        }
                    }
                    // listbox item dragging
                    else if (input.IsLeftMousePressed())
                    {
                        if (m_isListboxItemDragging)
                        {
                            if (m_mouseLastPosition != null &&
                                preselectedItem != null &&
                                preselectedItem.Enabled && 
                                m_mousePreselectedItemIndex == m_previousMousePreselectedItemIndex &&
                                m_mousePreselectedRowIndex == m_previousMousePreselectedRowIndex)
                            {
                                Vector2 mouseDistanceFromLastUpdate = MyGuiManager.MouseCursorPosition - m_mouseLastPosition.Value;
                                if (mouseDistanceFromLastUpdate.Length() != 0.0f)
                                {
                                    if (ItemDrag != null)
                                    {
                                        ItemDrag(this, new MyGuiControlListboxItemEventArgs(m_mousePreselectedRowIndex.Value, m_previousMousePreselectedItemIndex.Value, preselectedItem.Key));
                                    }
                                    m_isListboxItemDragging = false;
                                }

                                captureInput = true;
                            }
                        }
                        m_mouseLastPosition = MyGuiManager.MouseCursorPosition;
                    }
                    else
                    {
                        m_isListboxItemDragging = false;
                    }
                    #endregion

                    #region handle keyboard
                    if (hasKeyboardActiveControl)
                    {
                        // handle selection move
                        if (HandleCurrentPreselected(input))
                        {
                            captureInput = true;
                            preselectedItem = GetPreselectedItem();
                        }

                        // selection by keyboard
                        if (input.IsNewKeyPress(Keys.Enter) || input.IsNewKeyPress(Keys.Space))
                        {                            
                            if (preselectedItem != null && preselectedItem.Enabled)
                            {
                                SetSelectedItem(m_preselectedRowIndex, m_preselectedItemIndex);
                                MyAudio.AddCue2D(MySoundCuesEnum.GuiMouseClick);
                            }
                            captureInput = true;
                        }
                    }
                    else
                    {
                        HandleMouseWheelScroll(input, ref captureInput);
                    }
                    #endregion
                }
                else
                {
                    m_mousePreselectedItemIndex = null;
                    m_mousePreselectedRowIndex = null;
                }
            }

            return captureInput;
        }

        void HandleMouseWheelScroll(MyGuiInput input, ref bool captureInput)
        {
            // MouseWheel movement
            int deltaMouseScrollValue = input.DeltaMouseScrollWheelValue();

            // MouseWheel scroll with scrollbar
            if (m_verticalScrollBar != null)
            {
                bool rowsOffsetChanged = false;
                if (deltaMouseScrollValue < 0)
                {
                    m_rowsOffset = Math.Min(
                        m_rowsOffset + 1, Math.Max(m_rows.Count - m_displayRowsCount, 0));
                    rowsOffsetChanged = true;
                }
                else if (deltaMouseScrollValue > 0)
                {
                    m_rowsOffset = Math.Max(m_rowsOffset - 1, 0);
                    rowsOffsetChanged = true;
                }

                if (rowsOffsetChanged)
                {
                    SetScrollValueByOffset(m_verticalScrollBar, m_rowsOffset, m_displayRowsCount);
                    captureInput = true;
                }
            }
            else // MouseWheel movement
            {
                if (deltaMouseScrollValue < 0)
                {
                    //this.MovePreselected(true, false, true);
                    MovePreselected(true, true);
                    captureInput = true;
                }
                else if (deltaMouseScrollValue > 0)
                {
                    //this.MovePreselected(false, false, true);
                    MovePreselected(false, true);
                    captureInput = true;
                }
            }
        }

        public bool Contains(float x, float y)
        {
            var size = this.GetSize();
            return size != null && MyGUIHelper.Contains(this.GetPosition(), size.Value, x, y);
        }

        public override void Draw()
        {
            //base.Draw();                                    

            Vector4 vectorLineColor = MyGuiConstants.LISTBOX_LINE_COLOR;
            Vector4 backgroundColor = m_backgroundColor.Value;
            if (!Enabled)
            {
                backgroundColor *= MyGuiConstants.LISTBOX_DISABLED_COLOR;
                vectorLineColor *= MyGuiConstants.LISTBOX_DISABLED_COLOR;
            }
            //Vector4 vectorLineColor = MyGuiConstants.LISTBOX_LINE_COLOR;
            if (IsMouseOver())
            {
                vectorLineColor = vectorLineColor * MyGuiConstants.CONTROL_MOUSE_OVER_BACKGROUND_COLOR_MULTIPLIER;
            }

            #region Draw background
            if (m_controlTexture != null)
            {
                MyGuiManager.DrawSpriteBatch(m_controlTexture,
                                             GetLeftTopPosition() +
                                             new Vector2(m_borderSizeListBox, m_borderSizeListBox), m_size.Value,
                                             GetColorAfterTransitionAlpha(backgroundColor *
                                                                          (new Color(50, 66, 70, 255)).ToVector4()),
                                             MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP);
            }
            else
            {   

                //MyGuiManager.DrawBackgroundRectangleForOpaqueControl(
                //    GetLeftTopPosition(), m_size.Value,
                //    GetColorAfterTransitionAlpha(backgroundColor * (new Color(50, 66, 70, 255)).ToVector4()),
                //    MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP);
            }

            #endregion

            #region Draw scrollbar
            if (m_verticalScrollBar != null)
            {
                if (!ShowScrollBarOnlyWhenNeeded || m_rows.Count > m_displayRowsCount)
                {
                    m_verticalScrollBar.Draw();
                }
            }
            if (m_horizontalScrollBar != null)
            {
                if (!ShowScrollBarOnlyWhenNeeded || m_columns > m_displayColumnsCount)
                {
                    m_horizontalScrollBar.Draw();
                }
            }
            #endregion

            #region draw items
            for (int rowIndex = m_rowsOffset; rowIndex < m_rowsOffset + Math.Min(m_displayRowsCount, m_rows.Count); rowIndex++)
            {
                for (int itemIndex = m_columnsOffset; itemIndex < m_columnsOffset + Math.Min(m_displayColumnsCount, m_columns); itemIndex++)
                {
                    DrawItem(rowIndex, itemIndex, MyGuiConstants.LISTBOX_LINE_COLOR);
                }
            }
            #endregion

            #region Draw borders
            if (m_borderSizeListBox > 0.0f)
            {
                Color itemsBordersColor = GetColorAfterTransitionAlpha(vectorLineColor);
                DrawBorders(GetLeftTopPosition(), m_size.Value, itemsBordersColor, m_borderSize);
            }
            #endregion

            #region Draw disabled X
            if (!Enabled)
            {
                MyGuiManager.DrawSpriteBatch(MyGuiManager.GetLockedInventoryItem(),
                                             GetLeftTopPosition() +
                                             new Vector2(m_borderSizeListBox, m_borderSizeListBox) + new Vector2(m_paddingLeft, m_paddingTop),
                                             GetItemsAreaSize(), new Color(255,240,240,255), 
                                             MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP);
            }

            #endregion

            //ShowToolTip();
        }

        public override void ShowToolTip()
        {
            // if listbox supports icons and mouse is over any item, then show item's value in tooltip
            MyToolTips tempToolTip = m_toolTip;
            //if (m_supportIcon)
            //{
            //    MyGuiControlListboxItem mouseOverItem = GetMousePreselectedItem();
            //    if (mouseOverItem != null)
            //    {
            //        m_toolTip = new MyToolTips(mouseOverItem.Value);
            //    }
            //}                
            MyGuiControlListboxItem mouseOverItem = GetMousePreselectedItem();
            if (mouseOverItem != null && mouseOverItem.ToolTip != null && mouseOverItem.ToolTip.GetToolTips().Count > 0)
            {
                m_toolTip = mouseOverItem.ToolTip;
            }
            base.ShowToolTip();
            m_toolTip = tempToolTip;
        }

        protected override bool CheckMouseOver()
        {
            return base.CheckMouseOver();
        }
        #endregion

        #region private methods
        private void FixAfterRemove(bool removeEmptyItems) 
        {
            if (removeEmptyItems)
            {
                RemoveEmptyItems();
            }
            FixSelectedIndexes();
        }

        private MyGuiControlListboxItem CreateListboxItem(int key, StringBuilder value, MyTexture2D icon)
        {
            MyColoredText coloredText = null;
            MyToolTips toolTips = null;
            if (value != null)
            {
                coloredText = new MyColoredText(value, new Color(MyGuiConstants.LISTBOX_TEXT_COLOR),
                                                new Color(MyGuiConstants.LISTBOX_TEXT_COLOR * MyGuiConstants.CONTROL_MOUSE_OVER_BACKGROUND_COLOR_MULTIPLIER),
                                                MyGuiManager.GetFontMinerWarsBlue(), m_textScale, Vector2.Zero);
                toolTips = new MyToolTips(value);
            }
            MyGuiControlListboxItem newItem = new MyGuiControlListboxItem(key, coloredText, icon, toolTips);
            return newItem;
        }

        private void RecalculateAfterRowsCountChanged()
        {
            RecalculateRealSize();
            RecalculateRealPosition();
            int maxOffset = m_rows.Count - m_displayRowsCount;
            m_rowsOffset = Math.Max(Math.Min(m_rowsOffset, maxOffset), 0);
            if (m_verticalScrollBar != null)
            {
                float scrollBarMaxValue = Math.Max((float)m_rows.Count / (float)m_displayRowsCount, 1.0f);
                m_verticalScrollBar.InitializeScrollBar(scrollBarMaxValue);
                SetScrollValueByOffset(m_verticalScrollBar, m_rowsOffset, m_displayRowsCount);
            }
        }

        private void SetOffsets(int? rowIndex, int? itemIndex, bool setMaxOffset = false)
        {
            m_rowsOffset = GetOffsetByIndex(m_rowsOffset, rowIndex, m_displayRowsCount, setMaxOffset);
            m_columnsOffset = GetOffsetByIndex(m_columnsOffset, itemIndex, m_displayColumnsCount, setMaxOffset);
            if (m_verticalScrollBar != null)
            {
                SetScrollValueByOffset(m_verticalScrollBar, m_rowsOffset, m_displayRowsCount);
            }
            if (m_horizontalScrollBar != null)
            {
                SetScrollValueByOffset(m_horizontalScrollBar, m_columnsOffset, m_displayColumnsCount);
            }
        }

        private int GetOffsetByIndex(int currentOffset, int? index, int displayCount, bool setMaxOffset)
        {
            int newOffset;
            if (index == null)
            {
                newOffset = 0;
            }
            else
            {
                // lowest offset, to see index on screen (index will be on last displayed row)
                int minOffset = Math.Max(index.Value - displayCount + 1, 0);
                // highest offset, to see index on screen (index will be on first displayed row)
                int maxOffset = Math.Max(Math.Min(index.Value, m_rows.Count - displayCount), 0);

                if (currentOffset >= minOffset && currentOffset <= maxOffset)
                {
                    newOffset = currentOffset;
                }
                else 
                {
                    if (currentOffset < minOffset && !setMaxOffset)
                    {
                        newOffset = minOffset;
                    }
                    else 
                    {
                        newOffset = maxOffset;
                    }
                }
            }
            return newOffset;
        }

        private void SetScrollValueByOffset(MyGuiControlListboxScrollBar scrollBar, int offset, int displayCount)
        {
            scrollBar.SetScrollValue((float)(offset + displayCount) / (float)displayCount);
        }

        private void SetRowsOffsetByScrollRatio(float scrollRatio)
        {
            int maxOffset = m_rows.Count - m_displayRowsCount;
            m_rowsOffset = Math.Min((int)Math.Floor(scrollRatio * (maxOffset + 1)), maxOffset);
        }

        private void SetColumnsOffsetByScrollRatio(float scrollRatio)
        {
            int maxOffset = m_columns - m_displayColumnsCount;
            m_columnsOffset = Math.Min((int)Math.Floor(scrollRatio * (maxOffset + 1)), maxOffset);
        }

        private void RecalculateRealSize()
        {
            Vector2 newSize = GetItemsAreaSize();
            if (m_verticalScrollBar != null)
            {
                Vector2 scrollBarSize = m_verticalScrollBar.GetSize();
                scrollBarSize.Y = newSize.Y + m_sliderLengthIncrease;
                //scrollBarSize.X /= 2f;
                m_verticalScrollBar.SetNewSize(scrollBarSize);
                newSize.X = newSize.X + m_verticalScrollBar.GetSize().X;
            }
            if (m_horizontalScrollBar != null)
            {
                newSize.Y = newSize.Y + m_horizontalScrollBar.GetSize().Y;
            }
            newSize += new Vector2(m_borderSizeListBox * 2.0f, m_borderSizeListBox * 2.0f);
            newSize += new Vector2(m_paddingLeft + m_paddingRight + m_sizeXincrease, m_paddingTop + m_paddingBottom);

            m_size = newSize;
        }

        private Vector2 GetItemsAreaSize()
        {
            Vector2 itemsAreaSize = Vector2.Zero;
            for (int i = 0; i < m_displayColumnsCount; i++)
            {
                if (m_customCollumsWidths != null && m_customCollumsWidths.Count > i && m_customCollumsWidths[i] > 0)
                {
                    itemsAreaSize.X += m_customCollumsWidths[i];
                }
                else
                {
                    itemsAreaSize.X += m_itemSize.X;
                }
            }
            
            itemsAreaSize.Y = m_itemSize.Y * m_displayRowsCount;


            return itemsAreaSize;
        }

        private Vector2 GetItemsAreaSizeForMouse()
        {
            Vector2 itemsAreaSize = m_itemSize;
            itemsAreaSize.X = m_columnWidth * m_displayColumnsCount;
            itemsAreaSize.Y = m_rowHeight * m_displayRowsCount;
            //itemsAreaSize.X = m_itemSize.X * m_displayColumnsCount;
            //itemsAreaSize.Y = m_itemSize.Y * m_displayRowsCount;
            //Vector2 itemPositionOffset = new Vector2((itemIndex - m_columnsOffset) * m_columnWidth, (rowIndex - m_rowsOffset) * m_rowHeight);
            //Vector2 leftTopItemPosition = GetLeftTopPosition() + itemPositionOffset + new Vector2(m_borderSizeListBox, m_borderSizeListBox) + new Vector2(m_paddingLeft, m_paddingTop);


            return itemsAreaSize;
        }

        private void RecalculateRealPosition()
        {
            //Vector2 newPosition = m_topLeftPosition;
            //newPosition.Y = newPosition.Y + (m_size.Value.Y / 2.0f);
            //newPosition.X = newPosition.X + (m_size.Value.X / 2.0f);

            //m_position = newPosition;

            m_topLeftPosition = new Vector2(m_position.X - (m_size.Value.X / 2.0f), m_position.Y - (m_size.Value.Y / 2.0f));

            if (m_horizontalScrollBar != null)
            {
                m_horizontalScrollBar.SetNewPosition(m_topLeftPosition + new Vector2(0f, GetItemsAreaSize().Y) + new Vector2(m_borderSizeListBox, m_borderSizeListBox) + new Vector2(m_paddingLeft, m_paddingTop));
            }
            if (m_verticalScrollBar != null)
            {
                m_verticalScrollBar.SetNewPosition(m_topLeftPosition + new Vector2(GetItemsAreaSize().X, 0f) + new Vector2(m_borderSizeListBox, m_borderSizeListBox) + new Vector2(m_paddingLeft, m_paddingTop) + new Vector2(m_sliderOffset, m_sliderOffsetTop));
            }
        }

        private Vector2 GetLeftTopPosition()
        {
            return m_parent.GetPositionAbsolute() + m_topLeftPosition;
        }

        private Vector2 GetLeftTopItemPosition(int rowIndex, int itemIndex)
        {
            float Xoffset = 0;
            for (int i = 0; i < itemIndex; i++)
            {
                if (m_customCollumsWidths != null && m_customCollumsWidths.Count > i && m_customCollumsWidths[i] > 0)
                {
                    Xoffset += m_customCollumsWidths[i];
                }
                else
                {
                    Xoffset += m_columnWidth;
                }
            }
            Vector2 itemPositionOffset = new Vector2(Xoffset, (rowIndex - m_rowsOffset) * m_rowHeight);
            Vector2 leftTopItemPosition = GetLeftTopPosition() + itemPositionOffset + new Vector2(m_borderSizeListBox, m_borderSizeListBox) + new Vector2(m_paddingLeft, m_paddingTop);

            return leftTopItemPosition;
        }

        private MyGuiControlListboxItem GetMousePreselectedItem()
        {
            if (m_mousePreselectedItemIndex != null && m_mousePreselectedRowIndex != null &&
                AreIndexesValid(m_mousePreselectedRowIndex.Value, m_mousePreselectedItemIndex.Value))
            {
                return GetItem(m_mousePreselectedRowIndex.Value, m_mousePreselectedItemIndex.Value);
            }
            return null;
        }

        private MyGuiControlListboxItem GetPreselectedItem()
        {
            if (m_preselectedItemIndex != null && m_preselectedRowIndex != null &&
                AreIndexesValid(m_preselectedRowIndex.Value, m_preselectedItemIndex.Value))
            {
                return GetItem(m_preselectedRowIndex.Value, m_preselectedItemIndex.Value);
            }
            return null;
        }

        private bool AreIndexesValid(int rowIndex, int itemIndex)
        {
            return rowIndex >= 0 && rowIndex < m_rows.Count && itemIndex >= 0 && itemIndex < m_columns;
        }

        private void ResetMousePreselected()
        {
            m_mousePreselectedItemIndex = m_selectedItemIndex;
            m_mousePreselectedRowIndex = m_selectedRowIndex;
        }

        private void ResetPreselected()
        {
            m_preselectedItemIndex = m_selectedItemIndex;
            m_preselectedRowIndex = m_selectedRowIndex;
        }

        private void HandleCurrentMousePreselected()
        {
            m_previousMousePreselectedRowIndex = m_mousePreselectedRowIndex;
            m_previousMousePreselectedItemIndex = m_mousePreselectedItemIndex;
            Point? mouseOverIndexes = GetMouseOverIndexes();
            if (mouseOverIndexes != null)
            {
                m_mousePreselectedRowIndex = mouseOverIndexes.Value.Y;
                m_mousePreselectedItemIndex = mouseOverIndexes.Value.X;
            }
            else
            {
                m_mousePreselectedRowIndex = null;
                m_mousePreselectedItemIndex = null;
            }
        }

        private void MovePreselected(bool forward, bool vertical, bool overflow = false, bool page = false, bool list = false)
        {
            if (m_preselectedItemIndex == null && m_preselectedRowIndex == null)
            {
                if (m_selectedItemIndex != null && m_selectedRowIndex != null)
                {
                    m_preselectedItemIndex = m_selectedItemIndex;
                    m_preselectedRowIndex = m_selectedRowIndex;
                }
                else
                {
                    m_preselectedItemIndex = 0;
                    m_preselectedRowIndex = 0;
                    return;     // if not selected or preselected any item, then set preselected first item
                }
            }

            if (page)
            {
                if (forward)
                {
                    m_preselectedRowIndex = Math.Min(m_preselectedRowIndex.Value + m_displayRowsCount, m_rows.Count - 1);
                }
                else
                {
                    m_preselectedRowIndex = Math.Max(m_preselectedRowIndex.Value - m_displayRowsCount, 0);
                }
            }
            else if (list)
            {
                if (forward)
                {
                    m_preselectedRowIndex = m_rows.Count - 1;
                }
                else
                {
                    m_preselectedRowIndex = 0;
                }
            }
            else if (overflow)
            {
                if (forward)
                {
                    if (m_preselectedItemIndex < m_columns - 1)
                    {
                        m_preselectedItemIndex++;
                    }
                    else
                    {
                        if (m_preselectedRowIndex < m_rows.Count - 1)
                        {
                            m_preselectedItemIndex = 0;
                            m_preselectedRowIndex++;
                        }
                    }
                }
                else
                {
                    if (m_preselectedItemIndex > 0)
                    {
                        m_preselectedItemIndex--;
                    }
                    else
                    {
                        if (m_preselectedRowIndex > 0)
                        {
                            m_preselectedItemIndex = m_columns - 1;
                            m_preselectedRowIndex--;
                        }
                    }
                }
            }
            else
            {
                if (vertical)
                {
                    if (forward)
                    {
                        m_preselectedRowIndex = Math.Min(m_preselectedRowIndex.Value + 1, m_rows.Count - 1);
                    }
                    else
                    {
                        m_preselectedRowIndex = Math.Max(m_preselectedRowIndex.Value - 1, 0);
                    }
                }
                else
                {
                    if (forward)
                    {
                        m_preselectedItemIndex = Math.Min(m_preselectedItemIndex.Value + 1, m_columns - 1);
                    }
                    else
                    {
                        m_preselectedItemIndex = Math.Max(m_preselectedItemIndex.Value - 1, 0);
                    }
                }
            }
            SetOffsets(m_preselectedRowIndex, m_preselectedItemIndex);
        }

        private bool HandleCurrentPreselected(MyGuiInput input)
        {
            bool captureInput = false;

            HandleMouseWheelScroll(input, ref captureInput);

            //  Keyboard movement
            if (input.IsNewKeyPress(Keys.Down))
            {
                MovePreselected(true, true);
                captureInput = true;
            }
            else if (input.IsNewKeyPress(Keys.Up))
            {
                MovePreselected(false, true);
                captureInput = true;
            }
            else if (input.IsNewKeyPress(Keys.Right))
            {
                MovePreselected(true, false);
                captureInput = true;
            }
            else if (input.IsNewKeyPress(Keys.Left))
            {
                MovePreselected(false, false);
                captureInput = true;
            }
            else if (input.IsNewKeyPress(Keys.PageDown))
            {
                MovePreselected(true, true, false, true);
                captureInput = true;
            }
            else if (input.IsNewKeyPress(Keys.PageUp))
            {
                MovePreselected(false, true, false, true);
                captureInput = true;
            }
            else if (input.IsNewKeyPress(Keys.Home))
            {
                MovePreselected(false, true, false, false, true);
                captureInput = true;
            }
            else if (input.IsNewKeyPress(Keys.End))
            {
                MovePreselected(true, true, false, false, true);
                captureInput = true;
            }

            return captureInput;
        }

        private void DrawItem(int rowIndex, int itemIndex, Vector4 vectorLineColor)
        {
            var row = m_rows[rowIndex];
            MyGuiControlListboxItem itemToDraw = GetItem(rowIndex, itemIndex);
            Vector2 itemPosition = GetLeftTopItemPosition(rowIndex, itemIndex);
            Vector2 itemSize = m_itemSize;
            if (m_customCollumsWidths != null && m_customCollumsWidths.Count > itemIndex && m_customCollumsWidths[itemIndex] > 0)
            {
                itemSize.X = m_customCollumsWidths[itemIndex];
            }
            Color itemBackgroundColor;
            bool isSelected = false;
            bool isHighlighted = false;
            bool isPreselected = false;
            bool isMultipleSelected = false;

            // there is a item at this position
            if (itemToDraw != null && DisplayHighlight && itemToDraw.Enabled)
            {
                isSelected = m_selectedItemIndex == itemIndex && m_selectedRowIndex == rowIndex;
                isMultipleSelected = IsMultipleSelected(itemIndex, rowIndex);
                if (m_highlightAllRow && m_mousePreselectedRowIndex == rowIndex)
                {
                    if (!m_dontHighlightHeadlineRow || rowIndex > 0)
                    {
                        isHighlighted = true;
                    }
                    else
                    {
                        isHighlighted = m_mousePreselectedItemIndex == itemIndex && m_mousePreselectedRowIndex == rowIndex;
                    }
                }
                else
                {
                    isHighlighted = m_mousePreselectedItemIndex == itemIndex && m_mousePreselectedRowIndex == rowIndex;
                }
                isPreselected = m_preselectedItemIndex == itemIndex && m_preselectedRowIndex == rowIndex || isHighlighted;

                // if an item was selected or preselected
                if ((isSelected || isPreselected || isMultipleSelected) && ItemMouseOverTexture==null)
                {
                    itemBackgroundColor = new Color(m_itemColor * MyGuiConstants.LISTBOX_HIGHLIGHT_MULTIPLIER);    // selected item's background
                }
                else if (isMultipleSelected)
                {
                    itemBackgroundColor = new Color(m_itemColor * MyGuiConstants.LISTBOX_HIGHLIGHT_MULTIPLIER / 2);
                }
                else if (rowIndex == 0 && HighlightHeadline)
                {
                    itemBackgroundColor = new Color(m_itemColor * MyGuiConstants.LISTBOX_HIGHLIGHT_MULTIPLIER / 2);
                }
                else
                {
                    itemBackgroundColor = new Color(m_itemColor);    // item's background
                }
            }
            else
            {
                itemBackgroundColor = new Color(m_itemColor);        // empty slot's background                
            }

            if (itemToDraw != null && !itemToDraw.Enabled) 
            {
                itemBackgroundColor *= 0.5f;
            }

            // draw item's background
            MyTexture2D itemTexture = m_itemTexture != null ? m_itemTexture : MyGuiManager.GetBlankTexture();
            if (row.Color != null)
                itemBackgroundColor = new Color(itemBackgroundColor.ToVector4() + row.Color.Value);
            MyGuiManager.DrawSpriteBatch(itemTexture, itemPosition, itemSize, itemBackgroundColor, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP);

            // draw item if there is anyone
            if (itemToDraw != null)
            {
                bool highLight = (isSelected || isPreselected || isMultipleSelected) && itemToDraw.Enabled;
                Vector2 textPosition = itemPosition + TextOffset;
                textPosition.Y += (m_rowHeight / 2.0f);
                MyTexture2D icon = itemToDraw.Icon;
                float colorMultiplicator = 1f;
                if (!itemToDraw.Enabled)
                {
                    colorMultiplicator = 0.75f;
                }

                if (m_supportIcon == true && icon != null)
                {
                    //Vector2 iconSize = itemSize;


                    Vector2 iconSize = UseFullItemSizeIcon ? m_itemSize : (new Vector2(itemSize.Y * 3 / 4, itemSize.Y));                                        
                    Vector4 backgroundColor = itemToDraw.BackgroundColor * colorMultiplicator;

                    iconSize *= IconScale;
                    // draw icon
                    MyGuiManager.DrawSpriteBatch(
                        icon,
                        itemPosition + new Vector2(m_borderSizeItem, m_borderSizeItem),
                        iconSize - new Vector2(m_borderSizeItem, m_borderSizeItem),
                        GetColorAfterTransitionAlpha(highLight ? MyGuiConstants.CONTROL_MOUSE_OVER_BACKGROUND_COLOR_MULTIPLIER * backgroundColor : backgroundColor),
                        MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP);
                    textPosition.X += itemSize.Y * 3 / 4;


                    // draw icon texts
                    if (itemToDraw.IconTexts != null)
                    {
                        itemToDraw.IconTexts.Draw(itemPosition, iconSize - new Vector2(m_borderSizeItem, m_borderSizeItem), m_parent.GetTransitionAlpha(), highLight, colorMultiplicator);
                    }
                }

                // we draw text only if there is any text value to draw
                if (itemToDraw.Value != null)
                {
                    //  End our standard sprite batch
                    MyGuiManager.EndSpriteBatch();

                    //  Draw the rectangle(basically the opened area) to stencil buffer to be used for clipping partial item
                    MyGuiManager.DrawStencilMaskRectangle(new MyRectangle2D(itemPosition, itemSize), MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP);

                    //  Set up the stencil operation and parameters
                    MyGuiManager.BeginSpriteBatch_StencilMask();

                    
                    itemToDraw.ColoredText.Draw(textPosition, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER, m_parent.GetTransitionAlpha(), isHighlighted, colorMultiplicator);

                    //  End stencil-mask batch, and restart the standard sprite batch
                    //MyGuiManager.EndSpriteBatch();
                    MyGuiManager.EndSpriteBatch_StencilMask();
                    MyGuiManager.BeginSpriteBatch();
                }


                if (ItemMouseOverTexture!=null && isSelected && itemToDraw.Enabled)
                {
                    MyGuiManager.DrawSpriteBatch(ItemMouseOverTexture, itemPosition, itemSize, Color.White, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP);
                }
            }

            // draw borders
            if (m_borderSizeItem > 0.0f)
            {
                Color itemsBordersColor = GetColorAfterTransitionAlpha(vectorLineColor) * 0.8f;
                DrawBorders(itemPosition, itemSize, itemsBordersColor, m_itemBorderSize);
            }
        }

        public void CustomSortRows(Comparison<MyGuiControlListboxItem> comparison, int collumnIndex)
        {
            if (m_rows != null && collumnIndex < m_columns)
            {
                m_rows.Sort((a, b) =>
                    {
                        return comparison(a.Items[collumnIndex], b.Items[collumnIndex]);
                    });
            }
        }

        //private void DrawBorders(Vector2 position, Vector2 size, Color color, int borderSize) 
        //{
        //    Vector2 sizeInPixels = MyGuiManager.GetScreenSizeFromNormalizedSize(size);
        //    sizeInPixels = new Vector2((int)sizeInPixels.X, (int)sizeInPixels.Y);
        //    Vector2 leftTopInPixels = MyGuiManager.GetScreenCoordinateFromNormalizedCoordinate(position);
        //    leftTopInPixels = new Vector2((int)leftTopInPixels.X, (int)leftTopInPixels.Y);
        //    Vector2 rightTopInPixels = leftTopInPixels + new Vector2(sizeInPixels.X, 0);
        //    Vector2 leftBottomInPixels = leftTopInPixels + new Vector2(0, sizeInPixels.Y);
        //    // top
        //    MyGuiManager.DrawSpriteBatch(MyGuiManager.GetBlankTexture(), (int)leftTopInPixels.X, (int)leftTopInPixels.Y, (int)sizeInPixels.X, borderSize, color);
        //    // right
        //    MyGuiManager.DrawSpriteBatch(MyGuiManager.GetBlankTexture(), (int)rightTopInPixels.X - borderSize, (int)rightTopInPixels.Y + borderSize, borderSize, (int)sizeInPixels.Y - borderSize * 2, color);
        //    // bottom
        //    MyGuiManager.DrawSpriteBatch(MyGuiManager.GetBlankTexture(), (int)leftBottomInPixels.X, (int)leftBottomInPixels.Y - borderSize, (int)sizeInPixels.X, borderSize, color);
        //    //left
        //    MyGuiManager.DrawSpriteBatch(MyGuiManager.GetBlankTexture(), (int)leftTopInPixels.X, (int)leftTopInPixels.Y + borderSize, borderSize, (int)sizeInPixels.Y - borderSize * 2, color);            
        //}                

        private void OnScrollValueChanged(object sender, EventArgs args)
        {
            if(sender == m_verticalScrollBar)
            {
                OnVerticalScrollValueChanged();
            }
            else if(sender == m_horizontalScrollBar)
            {
                OnHorizontalScrollValueChanged();
            }
            else
            {
                throw new MyMwcExceptionApplicationShouldNotGetHere();
            }
        }

        private void OnVerticalScrollValueChanged()
        {
            SetRowsOffsetByScrollRatio(m_verticalScrollBar.GetScrollRatio());
        }
        private void OnHorizontalScrollValueChanged()
        {
            SetColumnsOffsetByScrollRatio(m_horizontalScrollBar.GetScrollRatio());
        }
        private void FixSelectedIndexes()
        {
            // mouse preselected
            if (m_mousePreselectedRowIndex != null)
            {
                if (m_mousePreselectedRowIndex.Value >= m_rows.Count)
                {
                    m_mousePreselectedRowIndex = null;
                    m_mousePreselectedItemIndex = null;
                }
            }

            // preselected
            if (m_preselectedRowIndex != null)
            {
                if (m_preselectedRowIndex.Value >= m_rows.Count)
                {
                    m_preselectedRowIndex = null;
                    m_preselectedItemIndex = null;
                }
            }

            // selected            
            if (m_selectedRowIndex != null)
            {
                if (m_selectedRowIndex.Value >= m_rows.Count/* ||
                    m_rows[m_selectedRowIndex.Value].Items[m_selectedItemIndex.Value] == null*/)
                {
                    m_selectedRowIndex = null;
                    m_selectedItemIndex = null;
                }

                SetSelectedItem(m_selectedRowIndex, m_selectedItemIndex);
            }
            //if(m_selectedKey != null)
            //{
            //    SetSelectedItem(m_selectedKey.Value);
            //}            
        }
        #endregion
    }        
}
