using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinerWars.AppCode.Toolkit.Input
{
    struct MyMouseState
    {
        public readonly int X;
        public readonly int Y;
        public readonly int ScrollWheelValue;

        public readonly bool LeftButton;
        public readonly bool RightButton;
        public readonly bool MiddleButton;
        public readonly bool XButton1;
        public readonly bool XButton2;

        public MyMouseState(int x, int y, int scrollWheel, bool leftButton, bool middleButton, bool rightButton, bool xButton1, bool xButton2)
        {
            X = x;
            Y = y;
            ScrollWheelValue = scrollWheel;
            LeftButton = leftButton;
            MiddleButton = middleButton;
            RightButton = rightButton;
            XButton1 = xButton1;
            XButton2 = xButton2;
        }
    }
}
