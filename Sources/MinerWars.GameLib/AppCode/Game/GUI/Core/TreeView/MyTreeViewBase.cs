using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.Textures;

namespace MinerWars.AppCode.Game.GUI.Core.TreeView
{
    class MyTreeViewBase : ITreeView
    {
        private List<MyTreeViewItem> m_items;
        public MyTreeView TreeView;

        public MyTreeViewItem this[int i]
        {
            get
            {
                System.Diagnostics.Debug.Assert(i < m_items.Count);
                return m_items[i];
            }
        }

        public MyTreeViewBase()
        {
            m_items = new List<MyTreeViewItem>();
        }

        public MyTreeViewItem AddItem(StringBuilder text, MyTexture2D icon, Vector2 iconSize, MyTexture2D expandIcon, MyTexture2D collapseIcon, Vector2 expandIconSize)
        {
            //System.Diagnostics.Trace.Assert(m_items.TrueForAll(a => a.GetIconSize() == iconSize));

            var item = new MyTreeViewItem(text, icon, iconSize, expandIcon, collapseIcon, expandIconSize);
            item.TreeView = TreeView;
            m_items.Add(item);
            item.Parent = this;
            return item;
        }

        public void DeleteItem(MyTreeViewItem item)
        {
            if (m_items.Remove(item))
            {
                item.TreeView = null;
                item.ClearItems();
            }
        }

        public void ClearItems()
        {
            foreach (var item in m_items)
            {
                item.TreeView = null;
                item.ClearItems();
            }
            m_items.Clear();
        }

        public Vector2 LayoutItems(Vector2 origin)
        {
            float width = 0;
            float height = 0;

            Vector2 currentOrigin = origin;
            foreach (var treeViewItem in m_items)
            {
                Vector2 itemSize = treeViewItem.LayoutItem(origin + new Vector2(0, height));

                width = Math.Max(width, itemSize.X);
                height += itemSize.Y;
            }
            return new Vector2(width, height);
        }

        public void DrawItems()
        {
            foreach (var treeViewItem in m_items)
            {
                treeViewItem.Draw();

                if (treeViewItem.IsExpanded)
                {
                    treeViewItem.DrawItems();
                }
            }
        }

        public bool HandleInput(MyGuiInput input, bool hasKeyboardActiveControl, bool hasKeyboardActiveControlPrevious, bool receivedFocusInThisUpdate)
        {
            bool captured = false;
            foreach (var treeViewItem in m_items)
            {
                captured = captured || treeViewItem.HandleInputEx(input, hasKeyboardActiveControl, hasKeyboardActiveControlPrevious, receivedFocusInThisUpdate);
                if (treeViewItem.IsExpanded)
                {
                    captured = captured || treeViewItem.HandleInput(input, hasKeyboardActiveControl, hasKeyboardActiveControlPrevious, receivedFocusInThisUpdate);
                }
            }
            return captured;
        }

        public int GetItemCount()
        {
            return m_items.Count;
        }

        public MyTreeViewItem GetItem(int index)
        {
            return m_items[index];
        }

        public int GetIndex(MyTreeViewItem item)
        {
            return m_items.IndexOf(item);
        }

        public MyTreeViewItem GetItem(StringBuilder name)
        {
            return m_items.Find(a => a.Text == name);
        }
    }
}
