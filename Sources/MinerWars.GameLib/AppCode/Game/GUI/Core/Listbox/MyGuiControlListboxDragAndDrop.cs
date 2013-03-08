using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.Audio;
using MinerWars.AppCode.Game.Utils;

namespace MinerWars.AppCode.Game.GUI.Core
{
    class MyDragAndDropInfo
    {
        public MyGuiControlListbox Listbox { get; set; }
        public int RowIndex { get; set; }
        public int ItemIndex { get; set; }
    }

    class MyDragAndDropRestriction
    {
        // main types enums can be dropped, if empty, all object builders can be dropped
        public List<ushort> ObjectBuilders { get; private set; }
        // sub types enums can be dropped, if empty, all object builder types can be dropped
        public List<ushort> ObjectBuilderTypes { get; private set; }

        public MyDragAndDropRestriction()
        {
            ObjectBuilders = new List<ushort>();
            ObjectBuilderTypes = new List<ushort>();
        }
    }

    enum MyDropHandleType
    {
        /// <summary>
        /// Drop released on mouse left click
        /// </summary>
        LeftMouseClick,
        /// <summary>
        /// Drop released on stop pressed left mouse 
        /// </summary>
        LeftMousePressed
    }

    class MyDragAndDropEventArgs
    {
        public MyDragAndDropInfo DragFrom { get; set; }
        public MyDragAndDropInfo DropTo { get; set; }
        public MyGuiControlListboxItem ListboxItem { get; set; }
    }
    delegate void OnListboxItemDropped(object sender, MyDragAndDropEventArgs eventArgs);
    class MyGuiControlListboxDragAndDrop : MyGuiControlBase
    {
        #region fields
        private MyGuiControlListboxItem m_draggingListboxItem;
        private List<MyGuiControlListbox> m_listboxesToDrop;
        private MyDragAndDropInfo m_draggingFrom;
        private Vector4 m_textColor;
        private float m_textScale;
        private Vector2 m_textOffset;
        private bool m_supportIcon;
        private MyDropHandleType? m_currentDropHandleType;
        #endregion

        #region constructors
        public MyGuiControlListboxDragAndDrop(IMyGuiControlsParent parent, List<MyGuiControlListbox> listboxesToDrop, MyGuiControlPreDefinedSize predefinedSize, Vector4 backgroundColor, Vector4 textColor, float textScale, Vector2 textOffset, bool supportIcon)
            : base(parent, new Vector2(0.0f, 0.0f), predefinedSize, backgroundColor, null, true)
        {
            m_listboxesToDrop = listboxesToDrop;
            m_textColor = textColor;
            m_textScale = textScale;
            m_textOffset = textOffset;
            m_supportIcon = supportIcon;
            m_size = GetPredefinedControlSize();
            DrawBackgroundTexture = true;
        }
        #endregion

        #region events and delegates
        public event OnListboxItemDropped ListboxItemDropped;
        #endregion

        #region overriden methods
        protected override bool CheckMouseOver()
        {
            return IsActive();
        }

        public override void Draw()
        {
            //base.Draw();

            if (IsActive())
            {
                // draw item's background
                if (m_backgroundColor != null && DrawBackgroundTexture)
                {
                    MyGuiManager.DrawSpriteBatch(MyGuiManager.GetBlankTexture(), MyGuiManager.MouseCursorPosition, m_size.Value, /*new Color(m_backgroundColor.Value)*/ GetColorAfterTransitionAlpha(m_backgroundColor.Value * (new Color(50, 66, 70, 255).ToVector4())), MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER);
                }

                Vector2 itemPosition = MyGuiManager.MouseCursorPosition - m_size.Value / 2.0f;
                Vector2 textPosition = itemPosition + m_textOffset;
                textPosition.Y += (m_size.Value.Y / 2.0f);
                // draw item's icon
                if (m_supportIcon == true && m_draggingListboxItem.Icon != null)
                {
                    MyGuiManager.DrawSpriteBatch(m_draggingListboxItem.Icon, itemPosition, m_size.Value, GetColorAfterTransitionAlpha(m_backgroundColor.Value), MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP);
                }
                // draw item's text
                else if (m_draggingListboxItem.Value != null)
                {
                    //  End our standard sprite batch
                    MyGuiManager.EndSpriteBatch();

                    //  Draw the rectangle(basically the opened area) to stencil buffer to be used for clipping partial item
                    MyGuiManager.DrawStencilMaskRectangle(new MyRectangle2D(itemPosition, m_size.Value), MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP);

                    //  Set up the stencil operation and parameters
                    MyGuiManager.BeginSpriteBatch_StencilMask();

                    m_draggingListboxItem.ColoredText.Draw(textPosition, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER, m_parent.GetTransitionAlpha(), true);

                    //  End stencil-mask batch, and restart the standard sprite batch
                    //MyGuiManager.EndSpriteBatch();
                    MyGuiManager.EndSpriteBatch_StencilMask();
                    MyGuiManager.BeginSpriteBatch();
                }

                ShowToolTip();
            }
        }

        public bool DrawBackgroundTexture { get; set; }

        protected override Vector2 GetPredefinedControlSize()
        {
            Vector2 size = Vector2.Zero;
            if (m_predefinedSize.HasValue)
            {
                if (m_predefinedSize == MyGuiControlPreDefinedSize.LARGE)
                {
                    size = MyGuiConstants.DRAG_AND_DROP_LARGE_SIZE;
                }
                else if (m_predefinedSize == MyGuiControlPreDefinedSize.MEDIUM)
                {
                    size = MyGuiConstants.DRAG_AND_DROP_MEDIUM_SIZE;
                }
                else if (m_predefinedSize == MyGuiControlPreDefinedSize.LONGMEDIUM)
                {
                    size = MyGuiConstants.DRAG_AND_DROP_LONGMEDIUM_SIZE;
                }
                else
                {
                    size = MyGuiConstants.DRAG_AND_DROP_SMALL_SIZE;
                }
            }

            if (m_supportIcon == true) size.Y = MyGuiConstants.DRAG_AND_DROP_ICON_SIZE_Y;

            return size;
        }

        public override void ShowToolTip()
        {
            if (IsActive() && m_toolTip != null && m_toolTip.GetToolTips().Count > 0)
            {
                m_toolTipPosition = MyGuiManager.MouseCursorPosition + MyGuiConstants.TOOL_TIP_RELATIVE_DEFAULT_POSITION;
                m_toolTip.Draw(m_toolTipPosition);
            }
        }

        public override bool HandleInput(MyGuiInput input, bool hasKeyboardActiveControl, bool hasKeyboardActiveControlPrevious, bool receivedFocusInThisUpdate)
        {
            bool captureInput = base.HandleInput(input, hasKeyboardActiveControl, hasKeyboardActiveControlPrevious, receivedFocusInThisUpdate);
            if (!captureInput)
            {
                captureInput = HandleInput(input);
            }
            return captureInput;
        }
        #endregion

        #region public methods
        /// <summary>
        /// Starts dragging item
        /// </summary>
        /// <param name="dropHandleType">On which action released drop event</param>
        /// <param name="draggingItem">Item which is dragging</param>
        /// <param name="draggingFrom">Information about item's origin</param>
        public void StartDragging(MyDropHandleType dropHandleType, MyGuiControlListboxItem draggingItem, MyDragAndDropInfo draggingFrom)
        {
            m_currentDropHandleType = dropHandleType;
            m_draggingListboxItem = draggingItem;
            m_draggingFrom = draggingFrom;
            m_toolTip = draggingItem.ToolTip;
        }

        /// <summary>
        /// Stops dragging item
        /// </summary>
        public void Stop()
        {
            m_draggingFrom = null;
            m_draggingListboxItem = null;
            m_currentDropHandleType = null;
        }

        /// <summary>
        /// Returns if dragging is active
        /// </summary>
        /// <returns></returns>
        public bool IsActive()
        {
            return m_draggingListboxItem != null && m_draggingFrom != null && m_currentDropHandleType != null;
        }
        #endregion

        public void Drop()
        {
            if (!IsActive())
                return;

            MyDragAndDropInfo dropTo = null;
            foreach (MyGuiControlListbox listbox in m_listboxesToDrop)
            {
                Point? mouseOverItemIndexes = listbox.GetMouseOverIndexes();
                if (mouseOverItemIndexes != null && listbox.Enabled)
                {
                    dropTo = new MyDragAndDropInfo();
                    dropTo.Listbox = listbox;
                    dropTo.RowIndex = mouseOverItemIndexes.Value.Y;
                    dropTo.ItemIndex = mouseOverItemIndexes.Value.X;
                    break;
                }
            }

            ListboxItemDropped(this, new MyDragAndDropEventArgs() { DragFrom = m_draggingFrom, DropTo = dropTo, ListboxItem = m_draggingListboxItem });
        }


        #region private methods
        private bool HandleInput(MyGuiInput input)
        {
            bool captureInput = false;
            if (IsActive())
            {
                // handling left mouse pressed drag and drop
                if (m_currentDropHandleType.Value == MyDropHandleType.LeftMousePressed)
                {
                    // still dragging
                    if (input.IsLeftMousePressed())
                    {
                        captureInput = true;
                    }
                    // dropping
                    else
                    {
                        HandleDropingItem();
                    }
                }
                // handling left mouse click drag and drop
                else if (m_currentDropHandleType.Value == MyDropHandleType.LeftMouseClick)
                {
                    if (input.IsNewLeftMousePressed())
                    {
                        HandleDropingItem();
                        captureInput = true;
                    }
                }
            }
            return captureInput;
        }

        private void HandleDropingItem()
        {
            if (IsActive())
            {
                Drop();
                
                MyAudio.AddCue2D(MySoundCuesEnum.GuiMouseClick);
            }
        }
        #endregion
    }
}
