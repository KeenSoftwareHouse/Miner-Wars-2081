using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.Utils;

namespace MinerWars.AppCode.Game.GUI.Core
{
    class MyGuiControlListboxItemEventArgs
    {
        public int RowIndex { get; set; }
        public int ItemIndex { get; set; }
        public int Key { get; set; }

        public MyGuiControlListboxItemEventArgs(int rowIndex, int itemIndex, int key)
        {
            RowIndex = rowIndex;
            ItemIndex = itemIndex;
            Key = key;
        }
    }

    class MyGuiControlListboxItem
    {
        public int Key { get; private set; }
        public MyTexture2D Icon { get; private set; }
        public MyColoredText ColoredText { get; set; }
        public MyIconTexts IconTexts { get; set; }
        public MyToolTips ToolTip { get; set; }

        public Vector4 BackgroundColor { get; set; }
        public bool Enabled { get; set; }

        public StringBuilder Value
        {
            get
            {
                if (ColoredText != null)
                {
                    return ColoredText.Text;
                }
                else
                {
                    return null;
                }
            }
        }

        public MyGuiControlListboxItem(int key, StringBuilder value, MyTexture2D icon, float scale)
            : this(key, value, icon, null, scale)
        {
        }

        public MyGuiControlListboxItem(int key, StringBuilder value, MyTexture2D icon, MyToolTips tooltip, float scale)
        {
            Key = key;
            Icon = icon;
            ToolTip = tooltip;

            if (value != null)
            {
                ColoredText = new MyColoredText(value, new Color(MyGuiConstants.LISTBOX_TEXT_COLOR),
                                  new Color(MyGuiConstants.LISTBOX_TEXT_COLOR * MyGuiConstants.CONTROL_MOUSE_OVER_BACKGROUND_COLOR_MULTIPLIER),
                                  MyGuiManager.GetFontMinerWarsBlue(),
                                  scale, Vector2.Zero);

                if (ToolTip == null)
                {
                    ToolTip = new MyToolTips(value);
                }
            }
            BackgroundColor = Vector4.One;
            Enabled = true;
        }

        public MyGuiControlListboxItem(int key, MyColoredText coloredText, MyTexture2D icon, MyToolTips toolTip)
        {
            Key = key;
            Icon = icon;
            ColoredText = coloredText;
            ToolTip = toolTip;
            BackgroundColor = Vector4.One;
            Enabled = true;
        }
    }
}
