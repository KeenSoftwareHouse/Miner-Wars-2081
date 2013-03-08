using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWarsMath;

namespace MinerWars.AppCode.Game.GUI.Core
{    
    class MyGuiControlListboxRow
    {
        #region fields
        #endregion

        #region properties
        public MyGuiControlListboxItem[] Items { get; private set; }
        public Vector4? Color;
        #endregion

        #region constructors
        public MyGuiControlListboxRow(int columns)
        {
            Items = new MyGuiControlListboxItem[columns];
        }
        #endregion

        #region public methods
        /// <summary>
        /// Returns free empty slot in this row, if not founded then return null
        /// </summary>
        /// <returns>Index of first empty slot</returns>
        public int? GetFirstEmptyItemSlot()
        {
            for (int i = 0; i < Items.Length; i++)
            {
                if (Items[i] == null)
                {
                    return i;
                }
            }
            return null;
        }

        /// <summary>
        /// Returns if row is empty
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty()
        {
            for (int i = 0; i < Items.Length; i++)
            {
                if (Items[i] != null)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Returns item's count
        /// </summary>
        /// <returns></returns>
        public int ItemsCount()
        {
            int itemsCount = 0;
            for (int i = 0; i < Items.Length; i++)
            {
                if (Items[i] != null)
                {
                    itemsCount++;
                }
            }
            return itemsCount;
        }
        #endregion

        #region private methods
        #endregion

    }    
}
