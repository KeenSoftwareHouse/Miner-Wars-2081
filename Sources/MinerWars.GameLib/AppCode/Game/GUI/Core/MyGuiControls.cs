using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using KeenSoftwareHouse.Library.Collections;
using MinerWarsMath;
using MinerWars.AppCode.Toolkit.Input;
using MinerWars.AppCode.Game.Utils;

namespace MinerWars.AppCode.Game.GUI.Core
{
    class MyGuiControls
    {
        private ObservableCollection<MyGuiControlBase> m_allControls;
        private List<MyGuiControlBase> m_visibleControls;
        private bool m_refreshVisibleControls;

        public event EventHandler CollectionChanged;

        public MyGuiControls()
        {            
            m_allControls = new ObservableCollection<MyGuiControlBase>();
            m_visibleControls = new List<MyGuiControlBase>();
            m_allControls.CollectionChanged += CollectionChangedPrivate;
            m_refreshVisibleControls = true;
        }

        private void CollectionChangedPrivate(object sender, NotifyCollectionChangedEventArgs e)
        {
            NotifyCollectionChanged();                        
        }

        private void ChildCollectionChanged(object sender, EventArgs args) 
        {
            NotifyCollectionChanged();
        }

        private void NotifyCollectionChanged()
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, EventArgs.Empty);
            }
        }

        private void OnVisibilityChanged(MyGuiControlBase control, bool isVisible)
        {
            NotifyCollectionChanged();
            m_refreshVisibleControls = true;
        }

        private void RefreshVisibleControls()
        {                   
            foreach (MyGuiControlBase control in m_allControls)
            {
                if (control.Visible && !m_visibleControls.Contains(control))
                {
                    m_visibleControls.Add(control);
                }
                else if(!control.Visible && m_visibleControls.Contains(control))
                {
                    m_visibleControls.Remove(control);
                }
            }
        }

        public List<MyGuiControlBase> GetVisibleControls()
        {
            if (m_refreshVisibleControls)
            {
                RefreshVisibleControls();
                m_refreshVisibleControls = false;
            }
            return m_visibleControls;
        }

        
        public void Add(MyGuiControlBase item)
        {
            item.OnVisibilityChanged -= OnVisibilityChanged;
            item.OnVisibilityChanged += OnVisibilityChanged;

            TrySetCollecionChangeEvent(item, true);

            m_allControls.Add(item);

            if (item.Visible)
            {
                m_visibleControls.Add(item);
            }
        }

        public void Clear()
        {
            foreach (MyGuiControlBase control in m_allControls)
            {
                control.OnVisibilityChanged -= OnVisibilityChanged;
                TrySetCollecionChangeEvent(control, false);                
            }

            m_allControls.Clear();
            m_visibleControls.Clear();
        }

        public bool Contains(MyGuiControlBase item)
        {
            return m_allControls.Contains(item);
        }

        public void CopyTo(MyGuiControlBase[] array, int arrayIndex)
        {
            m_allControls.CopyTo(array, arrayIndex);
        }

        public bool Remove(MyGuiControlBase item)
        {
            bool itemRemove = m_allControls.Remove(item);

            if (itemRemove)
            {
                m_visibleControls.Remove(item);
                item.OnVisibilityChanged -= OnVisibilityChanged;
                TrySetCollecionChangeEvent(item, false);
            }

            return itemRemove;
        }

        public int Count { get { return m_allControls.Count; } }
        public bool IsReadOnly { get { return false; } }
        public int IndexOf(MyGuiControlBase item)
        {
            return m_allControls.IndexOf(item);
        }

        public void Insert(int index, MyGuiControlBase item)
        {
            item.OnVisibilityChanged -= OnVisibilityChanged;
            item.OnVisibilityChanged += OnVisibilityChanged;
            TrySetCollecionChangeEvent(item, true);
            m_allControls.Insert(index, item);

            m_refreshVisibleControls = true;
        }

        public void RemoveAt(int index)
        {
            MyGuiControlBase removedControl = m_allControls[index];
            removedControl.OnVisibilityChanged -= OnVisibilityChanged;
            TrySetCollecionChangeEvent(m_allControls[index], false);

            m_allControls.RemoveAt(index);
            m_visibleControls.Remove(removedControl);
        }

        public MyGuiControlBase this[int index]
        {
            get
            {
                return m_allControls[index];
            }
            set
            {
                MyGuiControlBase oldItem = m_allControls[index];
                if (oldItem != null)
                {
                    oldItem.OnVisibilityChanged -= OnVisibilityChanged;
                    m_visibleControls.Remove(oldItem);
                    TrySetCollecionChangeEvent(oldItem, false);
                }

                if (value != null)
                {
                    MyGuiControlBase newItem = value;
                    newItem.OnVisibilityChanged -= OnVisibilityChanged;
                    newItem.OnVisibilityChanged += OnVisibilityChanged;
                    TrySetCollecionChangeEvent(newItem, true);
                    m_allControls[index] = newItem;

                    m_refreshVisibleControls = true;
                }
            }
        }

        private void TrySetCollecionChangeEvent(MyGuiControlBase control, bool addDelegate) 
        {
            if (control is IMyGuiControlsParent) 
            {
                SetParentCollectionChangeEvent(control as IMyGuiControlsParent, addDelegate);
            }
        }

        private void SetParentCollectionChangeEvent(IMyGuiControlsParent parentControl, bool addDelegate) 
        {
            parentControl.Controls.CollectionChanged -= ChildCollectionChanged;
            if (addDelegate)
            {
                parentControl.Controls.CollectionChanged += ChildCollectionChanged;
            }
        }

        public IList<MyGuiControlBase> GetList()
        {
            return m_allControls;
        }
    }
}
