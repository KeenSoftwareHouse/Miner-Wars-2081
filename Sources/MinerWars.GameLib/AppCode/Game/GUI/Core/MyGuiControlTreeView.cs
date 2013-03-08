using System;
using System.Collections.Generic;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Toolkit.Input;
using MinerWars.AppCode.Game.Utils;
using MinerWarsMath.Graphics;
using System.Diagnostics;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.Audio;
using MinerWars.AppCode.Game.GUI.Core.TreeView;

namespace MinerWars.AppCode.Game.GUI.Core
{
    class MyGuiControlTreeView : MyGuiControlBase, ITreeView
    {
        //public bool Visible;
        public bool WholeRowHighlight;

        private Vector4 m_treeBackgroundColor;
        private MyTreeView m_treeView;

        public MyGuiControlTreeView(
            IMyGuiControlsParent parent,
            Vector2 position,
            Vector2 size,
            Vector4 backgroundColor,
            bool canHandleKeyboardActiveControl)
            : base(parent, position, size, null, null)
        {
            Visible = true;

            m_treeBackgroundColor = backgroundColor;
            m_canHandleKeyboardActiveControl = canHandleKeyboardActiveControl;

            m_treeView = new MyTreeView(this, m_parent.GetPositionAbsolute() + m_position - m_size.Value / 2, m_size.Value);
        }

        public MyTreeViewItem AddItem(StringBuilder text, MyTexture2D icon, Vector2 iconSize, MyTexture2D expandIcon, MyTexture2D collapseIcon, Vector2 expandIconSize)
        {
            return m_treeView.AddItem(text, icon, iconSize, expandIcon, collapseIcon, expandIconSize);
        }

        public void DeleteItem(MyTreeViewItem item)
        {
            m_treeView.DeleteItem(item);
        }

        public MyTreeViewItem GetFocusedItem()
        {
            return m_treeView.FocusedItem;
        }

        public void ClearItems()
        {
            m_treeView.ClearItems();
        }

        public override void Draw()
        {
            if (Visible)
            {
                //MyGUIHelper.FillRectangle(m_parent.GetPositionAbsolute() + m_position - m_size.Value / 2, m_size.Value, GetColorAfterTransitionAlpha(m_treeBackgroundColor));

                m_treeView.Layout();
                m_treeView.Draw();
            }
            else
            {
                ShowToolTip(null);
            }

            base.Draw();
        }

        public override bool HandleInput(MyGuiInput input, bool hasKeyboardActiveControl, bool hasKeyboardActiveControlPrevious, bool receivedFocusInThisUpdate)
        {
            bool captureInput = base.HandleInput(input, hasKeyboardActiveControl, hasKeyboardActiveControlPrevious, receivedFocusInThisUpdate);

            if (!captureInput && Visible)
            {
                captureInput = m_treeView.HandleInput(input, hasKeyboardActiveControl, hasKeyboardActiveControlPrevious,
                                       receivedFocusInThisUpdate);

                m_hasKeyboardActiveControl = captureInput;
            }

            return captureInput;
        }

        public void ShowToolTip(MyToolTips tooltip)
        {
            m_showToolTip = false;
            m_toolTip = tooltip;
            ShowToolTip();
        }

        public MyTreeViewItem GetItem(int index)
        {
            return m_treeView.GetItem(index);
        }

        public MyTreeViewItem GetItem(StringBuilder name)
        {
            return m_treeView.GetItem(name);
        }

        public int GetItemCount()
        {
            return m_treeView.GetItemCount();
        }

        public override void SetPosition(Vector2 position)
        {
            m_position = position;
            m_treeView.SetPosition(m_parent.GetPositionAbsolute() + m_position - m_size.Value / 2);
        }

        public void SetSize(Vector2 size)
        {
            m_size = size;
            m_treeView.SetPosition(m_parent.GetPositionAbsolute() + m_position - m_size.Value / 2);
            m_treeView.SetSize(size);
        }

        public override bool ContainsMouse()
        {
            return Visible && base.ContainsMouse();
        }

        public void FilterTree(Predicate<MyTreeViewItem> itemFilter)
        {
            MyTreeView.FilterTree(this, itemFilter);
        }
    }

    interface ITreeView
    {
        int GetItemCount();
        MyTreeViewItem GetItem(int index);
    }

    class MyGUIHelper
    {
        public static bool Contains(Vector2 position, Vector2 size, float x, float y)
        {
            return x >= position.X && y >= position.Y &&
                    x <= position.X + size.X && y <= position.Y + size.Y;
        }

        public static bool Intersects(Vector2 aPosition, Vector2 aSize, Vector2 bPosition, Vector2 bSize)
        {
            return
                !((aPosition.X > bPosition.X && aPosition.X > bPosition.X + bSize.X) ||
                  (aPosition.X + aSize.X < bPosition.X && aPosition.X + aSize.X < bPosition.X + bSize.X) ||
                  (aPosition.Y > bPosition.Y && aPosition.Y > bPosition.Y + bSize.Y) ||
                  (aPosition.Y + aSize.Y < bPosition.Y && aPosition.Y + aSize.Y < bPosition.Y + bSize.Y));
        }

        public static void FillRectangle(Vector2 position, Vector2 size, Color color)
        {
            Vector2 screenPosition = MyGuiManager.GetScreenCoordinateFromNormalizedCoordinate(position);
            var a = new Point((int)screenPosition.X, (int)screenPosition.Y);

            // for precission
            Vector2 screenPositionDownRight = MyGuiManager.GetScreenCoordinateFromNormalizedCoordinate(position + size);
            var b = new Point((int)screenPositionDownRight.X, (int)screenPositionDownRight.Y);

            MyGuiManager.DrawSpriteBatch(MyGuiManager.GetBlankTexture(), a.X, a.Y, b.X - a.X, b.Y - a.Y, color);
        }

        private static void OffsetInnerBorder(Vector2 normalizedPosition, Vector2 normalizedSize, int pixelWidth, int offset, Color color,
            bool top = true, bool bottom = true, bool left = true, bool right = true, Vector2? normalizedOffset = null)
        {
            Vector2 screenPosition = MyGuiManager.GetScreenCoordinateFromNormalizedCoordinate(normalizedPosition - (normalizedOffset.HasValue ? normalizedOffset.Value : Vector2.Zero));
            var a = new Point((int)screenPosition.X - offset, (int)screenPosition.Y - offset);

            // for precission
            Vector2 screenPositionDownRight = MyGuiManager.GetScreenCoordinateFromNormalizedCoordinate(normalizedPosition + normalizedSize + (normalizedOffset.HasValue ? normalizedOffset.Value : Vector2.Zero));
            var b = new Point((int)screenPositionDownRight.X + offset, (int)screenPositionDownRight.Y + offset);

            if (top)
                MyGuiManager.DrawSpriteBatch(MyGuiManager.GetBlankTexture(),
                    a.X, a.Y, b.X - a.X, pixelWidth, color);                   // Top
            if (bottom)
                MyGuiManager.DrawSpriteBatch(MyGuiManager.GetBlankTexture(),
                    a.X, b.Y - pixelWidth, b.X - a.X, pixelWidth, color);      // Bottom
            if (left)
                MyGuiManager.DrawSpriteBatch(MyGuiManager.GetBlankTexture(),
                    a.X, a.Y + (top ? pixelWidth : 0), pixelWidth, b.Y - a.Y - (bottom ? pixelWidth : 0) - (top ? pixelWidth : 0), color);                 // Left
            if (right)
                MyGuiManager.DrawSpriteBatch(MyGuiManager.GetBlankTexture(),
                    b.X - pixelWidth, a.Y + (top ? pixelWidth : 0), pixelWidth, b.Y - a.Y - (bottom ? pixelWidth : 0) - (top ? pixelWidth : 0), color);     // Right
        }

        public static void OutsideBorder(Vector2 normalizedPosition, Vector2 normalizedSize, int pixelWidth, Color color,
            bool top = true, bool bottom = true, bool left = true, bool right = true)
        {
            OffsetInnerBorder(normalizedPosition, normalizedSize, pixelWidth, pixelWidth, color, top, bottom, left, right);
        }

        public static void InsideBorder(Vector2 normalizedPosition, Vector2 normalizedSize, int pixelWidth, Color color,
            bool top = true, bool bottom = true, bool left = true, bool right = true)
        {
            OffsetInnerBorder(normalizedPosition, normalizedSize, pixelWidth, 0, color, top, bottom, left, right);
        }

        public static void Border(Vector2 normalizedPosition, Vector2 normalizedSize, int pixelWidth, Color color,
            bool top = true, bool bottom = true, bool left = true, bool right = true, Vector2? normalizedOffset = null)
        {
            OffsetInnerBorder(normalizedPosition, normalizedSize, 2 * pixelWidth, pixelWidth, color, top, bottom, left, right, normalizedOffset);
        }

        public static Vector2 GetOffset(Vector2 basePosition, Vector2 baseSize, Vector2 itemPosition, Vector2 itemSize)
        {
            float x = 0;
            float y = 0;

            if (baseSize.X > itemSize.X)
            {
                if (basePosition.X + baseSize.X < itemPosition.X + itemSize.X)
                {
                    x = basePosition.X + baseSize.X - (itemPosition.X + itemSize.X);
                }

                if (basePosition.X > itemPosition.X)
                {
                    x = basePosition.X - itemPosition.X;
                }
            }

            if (baseSize.Y > itemSize.Y)
            {
                if (basePosition.Y + baseSize.Y < itemPosition.Y + itemSize.Y)
                {
                    y = basePosition.Y + baseSize.Y - (itemPosition.Y + itemSize.Y);
                }

                if (basePosition.Y > itemPosition.Y)
                {
                    y = basePosition.Y - itemPosition.Y;
                }
            }

            return new Vector2(x, y);
        }
    }
}
