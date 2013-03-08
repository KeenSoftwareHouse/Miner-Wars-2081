using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Utils;

namespace MinerWars.AppCode.Game.GUI
{
    class MyGuiScreenFake : MyGuiScreenBase
    {
        public MyGuiScreenFake()
            :base(new Vector2(0.5f, 0.5f), new Vector4(0f, 1f, 0f, 0.5f), new Vector2(1f, 1f))
        {
            MyGuiControlList controlList = new MyGuiControlList(this, new Vector2(0f, 0f), new Vector2(0.8f, 0.5f), MyGuiConstants.DEFAULT_CONTROL_BACKGROUND_COLOR, new StringBuilder("ToolTip"), MyGuiManager.GetBlankTexture());
            Controls.Add(controlList);
            List<MyGuiControlBase> fakeControls = new List<MyGuiControlBase>();
            MyGuiControlParent parent1 = new MyGuiControlParent(this, Vector2.Zero, new Vector2(0.4f, 0.2f), new Vector4(1f, 0f, 0f, 1f), new StringBuilder("Parent1"), null);
            parent1.Controls.Add(new MyGuiControlTextbox(parent1, new Vector2(0f, 0f), MyGuiControlPreDefinedSize.LARGE, "AAA", 50, MyGuiConstants.TEXTBOX_BACKGROUND_COLOR, 0.7f, MyGuiControlTextboxType.NORMAL));
            parent1.Controls.Add(new MyGuiControlCheckbox(parent1, new Vector2(0f, 0.07f), MyGuiConstants.CHECKBOX_SIZE, true, MyGuiConstants.CHECKBOX_BACKGROUND_COLOR));            
            fakeControls.Add(parent1);
            MyGuiControlParent parent2 = new MyGuiControlParent(this, Vector2.Zero, new Vector2(0.4f, 0.2f), new Vector4(0f, 1f, 0f, 1f), new StringBuilder("Parent2"), null);
            parent2.Controls.Add(new MyGuiControlTextbox(parent2, new Vector2(0f, 0f), MyGuiControlPreDefinedSize.LARGE, "CCC", 50, MyGuiConstants.TEXTBOX_BACKGROUND_COLOR, 0.7f, MyGuiControlTextboxType.NORMAL));
            MyGuiControlCombobox combobox1 = new MyGuiControlCombobox(parent2, new Vector2(0f, 0.07f), MyGuiControlPreDefinedSize.MEDIUM, MyGuiConstants.COMBOBOX_BACKGROUND_COLOR, MyGuiConstants.COMBOBOX_TEXT_SCALE, 10, false, true, false);
            combobox1.AddItem(0, new StringBuilder("Item1 Item1 Item1 Item1 Item1 Item1 Item1 "));
            combobox1.AddItem(1, new StringBuilder("Item2 Item2 Item2 Item2 Item2 Item2 Item2 "));
            combobox1.AddItem(2, new StringBuilder("Item3 Item3 Item3 Item3 Item3 Item3 Item3 "));
            combobox1.AddItem(3, new StringBuilder("Item4"));
            combobox1.AddItem(4, new StringBuilder("Item5"));
            combobox1.AddItem(5, new StringBuilder("Item6"));
            combobox1.AddItem(6, new StringBuilder("Item7"));
            combobox1.AddItem(7, new StringBuilder("Item8"));
            combobox1.AddItem(8, new StringBuilder("Item9"));
            combobox1.AddItem(9, new StringBuilder("Item10"));
            combobox1.AddItem(10, new StringBuilder("Item11"));
            combobox1.AddItem(11, new StringBuilder("Item12"));
            combobox1.AddItem(12, new StringBuilder("Item13"));
            parent2.Controls.Add(combobox1);
            fakeControls.Add(parent2);
            MyGuiControlParent parent3 = new MyGuiControlParent(this, Vector2.Zero, new Vector2(0.4f, 0.2f), new Vector4(0f, 0f, 1f, 1f), new StringBuilder("Parent3"), null);
            parent3.Controls.Add(new MyGuiControlTextbox(parent3, new Vector2(0f, 0f), MyGuiControlPreDefinedSize.LARGE, "EEE", 50, MyGuiConstants.TEXTBOX_BACKGROUND_COLOR, 0.7f, MyGuiControlTextboxType.NORMAL));
            MyGuiControlCombobox combobox2 = new MyGuiControlCombobox(parent3, new Vector2(0f, 0.07f), MyGuiControlPreDefinedSize.MEDIUM, MyGuiConstants.COMBOBOX_BACKGROUND_COLOR, MyGuiConstants.COMBOBOX_TEXT_SCALE, 10, false, true, false);
            combobox2.AddItem(0, new StringBuilder("Item1 Item1 Item1 Item1 Item1 Item1 Item1 "));
            combobox2.AddItem(1, new StringBuilder("Item2 Item2 Item2 Item2 Item2 Item2 Item2 "));
            combobox2.AddItem(2, new StringBuilder("Item3 Item3 Item3 Item3 Item3 Item3 Item3 "));
            combobox2.AddItem(3, new StringBuilder("Item4"));
            combobox2.AddItem(4, new StringBuilder("Item5"));
            combobox2.AddItem(5, new StringBuilder("Item6"));
            combobox2.AddItem(6, new StringBuilder("Item7"));
            combobox2.AddItem(7, new StringBuilder("Item8"));
            combobox2.AddItem(8, new StringBuilder("Item9"));
            combobox2.AddItem(9, new StringBuilder("Item10"));
            combobox2.AddItem(10, new StringBuilder("Item11"));
            combobox2.AddItem(11, new StringBuilder("Item12"));
            combobox2.AddItem(12, new StringBuilder("Item13"));
            parent3.Controls.Add(combobox2);
            fakeControls.Add(parent3);
            MyGuiControlParent parent4 = new MyGuiControlParent(this, Vector2.Zero, new Vector2(0.4f, 0.2f), new Vector4(1f, 0f, 1f, 1f), new StringBuilder("Parent4"), null);
            parent4.Controls.Add(new MyGuiControlTextbox(parent4, new Vector2(0f, 0f), MyGuiControlPreDefinedSize.LARGE, "GGG", 50, MyGuiConstants.TEXTBOX_BACKGROUND_COLOR, 0.7f, MyGuiControlTextboxType.NORMAL));
            parent4.Controls.Add(new MyGuiControlTextbox(parent4, new Vector2(0f, 0.07f), MyGuiControlPreDefinedSize.LARGE, "HHH", 50, MyGuiConstants.TEXTBOX_BACKGROUND_COLOR, 0.7f, MyGuiControlTextboxType.NORMAL));
            fakeControls.Add(parent4);
            MyGuiControlParent parent5 = new MyGuiControlParent(this, Vector2.Zero, new Vector2(0.4f, 0.2f), new Vector4(1f, 1f, 0f, 1f), new StringBuilder("Parent5"), null);
            parent5.Controls.Add(new MyGuiControlTextbox(parent5, new Vector2(0f, 0f), MyGuiControlPreDefinedSize.LARGE, "III", 50, MyGuiConstants.TEXTBOX_BACKGROUND_COLOR, 0.7f, MyGuiControlTextboxType.NORMAL));
            parent5.Controls.Add(new MyGuiControlTextbox(parent5, new Vector2(0f, 0.07f), MyGuiControlPreDefinedSize.LARGE, "JJJ", 50, MyGuiConstants.TEXTBOX_BACKGROUND_COLOR, 0.7f, MyGuiControlTextboxType.NORMAL));
            fakeControls.Add(parent5);
            controlList.InitControls(fakeControls);
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenFake";
        }
    }
}
