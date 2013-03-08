using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.Audio;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Toolkit.Input;

namespace MinerWars.AppCode.Game.GUI.Core.TreeView
{
    class MyTreeView
    {
        private MyGuiControlTreeView m_control;

        private Vector2 m_position;
        private Vector2 m_size;

        private MyTreeViewBody m_body;
        private MyHScrollbar m_hScrollbar;
        private MyVScrollbar m_vScrollbar;
        private Vector2 m_scrollbarSize;

        public MyTreeViewItem FocusedItem;
        public MyTreeViewItem HooveredItem;

        public MyTreeView(MyGuiControlTreeView control, Vector2 position, Vector2 size)
        {
            m_control = control;

            m_position = position;
            m_size = size;

            m_body = new MyTreeViewBody(this, position, size);
            m_vScrollbar = new MyVScrollbar(control);
            m_hScrollbar = new MyHScrollbar(control);
            m_scrollbarSize = new Vector2(MyGuiConstants.TREEVIEW_VSCROLLBAR_SIZE.X, MyGuiConstants.TREEVIEW_HSCROLLBAR_SIZE.Y);

            m_vScrollbar.TopBorder = m_vScrollbar.RightBorder = false;
            m_hScrollbar.LeftBorder = m_hScrollbar.BottomBorder = false;
        }

        public void Layout()
        {
            m_body.Layout(Vector2.Zero);

            Vector2 realSize = m_body.GetRealSize();

            bool scrollbarsVisible = m_size.Y - m_scrollbarSize.Y < realSize.Y && m_size.X - m_scrollbarSize.X < realSize.X;
            bool vScrollbarVisible = scrollbarsVisible || m_size.Y < realSize.Y;
            bool hScrollbarVisible = scrollbarsVisible || m_size.X < realSize.X;

            m_vScrollbar.BottomBorder = scrollbarsVisible;
            m_hScrollbar.RightBorder = scrollbarsVisible;

            Vector2 bodySize = new Vector2(vScrollbarVisible ? m_size.X - m_scrollbarSize.X : m_size.X, hScrollbarVisible ? m_size.Y - m_scrollbarSize.Y : m_size.Y);

            m_vScrollbar.Visible = vScrollbarVisible;
            m_vScrollbar.Init(realSize.Y, bodySize.Y);
            m_vScrollbar.Layout(m_body.GetPosition() + new Vector2(m_scrollbarSize.X / 4f - 0.0024f, 0), m_body.GetSize(), new Vector2(m_scrollbarSize.X / 2f, m_scrollbarSize.Y), hScrollbarVisible);
            m_vScrollbar.BorderNormalizedOffset = new Vector2(-m_scrollbarSize.X/2 - 0.001f, 0);

            m_hScrollbar.Visible = hScrollbarVisible;
            m_hScrollbar.Init(realSize.X, bodySize.X);
            m_hScrollbar.Layout(m_body.GetPosition(), m_body.GetSize(), m_scrollbarSize, vScrollbarVisible);

            m_body.SetSize(bodySize);
            m_body.Layout(new Vector2(m_hScrollbar.GetValue(), m_vScrollbar.GetValue()));
        }

        private void TraverseVisible(ITreeView iTreeView, Action<MyTreeViewItem> action)
        {
            for (int i = 0; i < iTreeView.GetItemCount(); i++)
            {
                var item = iTreeView.GetItem(i);

                if (item.Visible)
                {
                    action(item);
                    if (item.IsExpanded)
                    {
                        TraverseVisible(item, action);
                    }
                }
            }
        }

        private MyTreeViewItem NextVisible(ITreeView iTreeView, MyTreeViewItem focused)
        {
            bool found = false;
            TraverseVisible(m_body, a =>
            {
                if (a == focused)
                {
                    found = true;
                }
                else if (found)
                {
                    focused = a;
                    found = false;
                }
            }
            );
            return focused;
        }

        private MyTreeViewItem PrevVisible(ITreeView iTreeView, MyTreeViewItem focused)
        {
            MyTreeViewItem pred = focused;
            TraverseVisible(m_body, a =>
            {
                if (a == focused)
                {
                    focused = pred;
                }
                else
                {
                    pred = a;
                }
            }
            );
            return focused;
        }

        public bool HandleInput(MyGuiInput input, bool hasKeyboardActiveControl, bool hasKeyboardActiveControlPrevious, bool receivedFocusInThisUpdate)
        {
            var oldHooveredItem = HooveredItem;
            HooveredItem = null;

            bool captured =
                m_body.HandleInput(input, hasKeyboardActiveControl, hasKeyboardActiveControlPrevious, receivedFocusInThisUpdate) ||
                m_vScrollbar.HandleInput(input, hasKeyboardActiveControl, hasKeyboardActiveControlPrevious, receivedFocusInThisUpdate) ||
                m_hScrollbar.HandleInput(input, hasKeyboardActiveControl, hasKeyboardActiveControlPrevious, receivedFocusInThisUpdate);

            if (hasKeyboardActiveControl)
            {
                if (FocusedItem == null &&
                    m_body.GetItemCount() > 0 &&
                    (input.IsNewKeyPress(Keys.Up) ||
                     input.IsNewKeyPress(Keys.Down) ||
                     input.IsNewKeyPress(Keys.Left) ||
                     input.IsNewKeyPress(Keys.Right) ||
                     input.DeltaMouseScrollWheelValue() != 0))
                {
                    FocusItem(m_body[0]);
                }
                else if (FocusedItem != null)
                {
                    if (input.IsNewKeyPress(Keys.Down) || (input.DeltaMouseScrollWheelValue() < 0 && Contains(MyGuiManager.MouseCursorPosition.X, MyGuiManager.MouseCursorPosition.Y)))
                    {
                        FocusItem(NextVisible(m_body, FocusedItem));
                    }

                    if (input.IsNewKeyPress(Keys.Up) || (input.DeltaMouseScrollWheelValue() > 0 && Contains(MyGuiManager.MouseCursorPosition.X, MyGuiManager.MouseCursorPosition.Y)))
                    {
                        FocusItem(PrevVisible(m_body, FocusedItem));
                    }

                    if (input.IsNewKeyPress(Keys.Right))
                    {
                        if (FocusedItem.GetItemCount() > 0)
                        {
                            if (!FocusedItem.IsExpanded)
                            {
                                FocusedItem.IsExpanded = true;
                            }
                            else
                            {
                                var next = NextVisible(FocusedItem, FocusedItem);
                                FocusItem(next);
                            }
                        }
                    }

                    if (input.IsNewKeyPress(Keys.Left))
                    {
                        if (FocusedItem.GetItemCount() > 0 && FocusedItem.IsExpanded)
                        {
                            FocusedItem.IsExpanded = false;
                        }
                        else if (FocusedItem.Parent is MyTreeViewItem)
                        {
                            FocusItem(FocusedItem.Parent as MyTreeViewItem);
                        }
                    }

                    if (FocusedItem.GetItemCount() > 0)
                    {
                        if (input.IsNewKeyPress(Keys.Add))
                        {
                            FocusedItem.IsExpanded = true;
                        }

                        if (input.IsNewKeyPress(Keys.Subtract))
                        {
                            FocusedItem.IsExpanded = false;
                        }
                    }
                }

                if (input.IsNewKeyPress(Keys.PageDown))
                {
                    m_vScrollbar.PageDown();
                }

                if (input.IsNewKeyPress(Keys.PageUp))
                {
                    m_vScrollbar.PageUp();
                }

                captured = captured ||
                           input.IsNewKeyPress(Keys.PageDown) ||
                           input.IsNewKeyPress(Keys.PageUp) ||
                           input.IsNewKeyPress(Keys.Down) ||
                           input.IsNewKeyPress(Keys.Up) ||
                           input.IsNewKeyPress(Keys.Left) ||
                           input.IsNewKeyPress(Keys.Right) ||
                           input.IsNewKeyPress(Keys.Add) ||
                           input.IsNewKeyPress(Keys.Subtract) ||
                           input.DeltaMouseScrollWheelValue() != 0;
            }

            // Hoovered item changed
            if (HooveredItem != oldHooveredItem)
            {
                m_control.ShowToolTip(HooveredItem == null ? null : HooveredItem.ToolTip);
                MyAudio.AddCue2D(MySoundCuesEnum.GuiMouseOver);
            }

            return captured;
        }

        public MyTreeViewItem AddItem(StringBuilder text, MyTexture2D icon, Vector2 iconSize, MyTexture2D expandIcon, MyTexture2D collapseIcon, Vector2 expandIconSize)
        {
            return m_body.AddItem(text, icon, iconSize, expandIcon, collapseIcon, expandIconSize);
        }

        public void DeleteItem(MyTreeViewItem item)
        {
            if (item == FocusedItem)
            {
                int index = item.GetIndex();
                if (index + 1 < GetItemCount())
                {
                    FocusedItem = GetItem(index + 1);
                }
                else if (index - 1 >= 0)
                {
                    FocusedItem = GetItem(index - 1);
                }
                else
                {
                    FocusedItem = FocusedItem.Parent as MyTreeViewItem;
                }
            }

            m_body.DeleteItem(item);
        }

        public void ClearItems()
        {
            m_body.ClearItems();
        }

        public void Draw()
        {
            //  End our standard sprite batch
            MyGuiManager.EndSpriteBatch();

            //  Draw the rectangle(basically the opened area) to stencil buffer to be used for clipping partial item
            MyGuiManager.DrawStencilMaskRectangle(new MyRectangle2D(m_body.GetPosition(), m_body.GetSize()), MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP);

            //  Set up the stencil operation and parameters
            MyGuiManager.BeginSpriteBatch_StencilMask();

            m_body.Draw();

            //  End stencil-mask batch, and restart the standard sprite batch
            //MyGuiManager.EndSpriteBatch();
            MyGuiManager.EndSpriteBatch_StencilMask();
            MyGuiManager.BeginSpriteBatch();

            Color borderColor = m_control.GetColorAfterTransitionAlpha(MyGuiConstants.TREEVIEW_VERTICAL_LINE_COLOR);
            MyGUIHelper.OutsideBorder(m_position, m_size, 2, borderColor);

            m_vScrollbar.Draw();
            m_hScrollbar.Draw();
        }

        public bool Contains(Vector2 position, Vector2 size)
        {
            return MyGUIHelper.Intersects(m_body.GetPosition(), m_body.GetSize(), position, size);
        }

        public bool Contains(float x, float y)
        {
            return MyGUIHelper.Contains(m_body.GetPosition(), m_body.GetSize(), x, y);
        }

        public void FocusItem(MyTreeViewItem item)
        {
            if (item != null)
            {
                Vector2 offset = MyGUIHelper.GetOffset(m_body.GetPosition(), m_body.GetSize(), item.GetPosition(), item.GetSize());

                m_vScrollbar.ChangeValue(-offset.Y);
                m_hScrollbar.ChangeValue(-offset.X);
            }

            FocusedItem = item;
        }

        public Vector2 GetPosition()
        {
            return m_body.GetPosition();
        }

        public Vector2 GetBodySize()
        {
            return m_body.GetSize();
        }

        public Color GetColor(Vector4 color)
        {
            return m_control.GetColorAfterTransitionAlpha(color);
        }

        public float GetTransitionAlpha()
        {
            return m_control.GetParent().GetTransitionAlpha();
        }

        public bool WholeRowHighlight()
        {
            return m_control.WholeRowHighlight;
        }

        public MyTreeViewItem GetItem(int index)
        {
            return m_body[index];
        }

        public MyTreeViewItem GetItem(StringBuilder name)
        {
            return m_body.GetItem(name);
        }

        public int GetItemCount()
        {
            return m_body.GetItemCount();
        }

        public void SetPosition(Vector2 position)
        {
            m_position = position;
            m_body.SetPosition(position);
        }

        public void SetSize(Vector2 size)
        {
            m_size = size;
            m_body.SetSize(size);
        }

        public static bool FilterTree(ITreeView treeView, Predicate<MyTreeViewItem> itemFilter)
        {
            int visibleCount = 0;
            for (int i = 0; i < treeView.GetItemCount(); i++)
            {
                var item = treeView.GetItem(i);

                if (FilterTree(item, itemFilter) || (item.GetItemCount() == 0 && itemFilter(item)))
                {
                    item.Visible = true;
                    ++visibleCount;
                }
                else
                {
                    item.Visible = false;
                }
            }
            return visibleCount > 0;
        }
    }
}
