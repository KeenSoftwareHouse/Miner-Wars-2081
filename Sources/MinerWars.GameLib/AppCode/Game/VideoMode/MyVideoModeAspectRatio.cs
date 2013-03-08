using System;
using MinerWarsMath.Graphics;
using MinerWars.AppCode.Game.Localization;
using MinerWars.CommonLIB.AppCode.Utils;
using SharpDX.Toolkit.Graphics;

namespace MinerWars.AppCode.Game.VideoMode
{
    //  IMPORTANT: Never change numeric values. When you need new enum item, just use new number!
    enum MyAspectRatioEnum
    {
        NORMAL_4_3 = 0,
        WIDE_16_9 = 1,
        WIDE_16_10 = 2,
        DUAL_HEAD_NORMAL_4_3 = 3,
        DUAL_HEAD_WIDE_16_9 = 4,
        DUAL_HEAD_WIDE_16_10 = 5,
        TRIPLE_HEAD_NORMAL_4_3 = 6,
        TRIPLE_HEAD_WIDE_16_9 = 7,
        TRIPLE_HEAD_WIDE_16_10 = 8
    }

    class MyAspectRatioEx
    {
        public MyAspectRatioEnum AspectRatioEnum;
        public float AspectRatioNumber;
        public MyTextsWrapperEnum TextLong;
        public MyTextsWrapperEnum TextShort;
        public bool IsTripleHead;


        public MyAspectRatioEx(bool isTripleHead, MyAspectRatioEnum aspectRatioEnum, float aspectRatioNumber, MyTextsWrapperEnum textLong, MyTextsWrapperEnum textShort)
        {
            IsTripleHead = isTripleHead;
            AspectRatioEnum = aspectRatioEnum;
            AspectRatioNumber = aspectRatioNumber;
            TextLong = textLong;
            TextShort = textShort;
        }
    }

    static class MyAspectRatioExList
    {
        //  From outside of this class use this list only as read-only!
        public static MyAspectRatioEx[] List;


        static MyAspectRatioExList()
        {
            List = new MyAspectRatioEx[MyMwcUtils.GetMaxValueFromEnum<MyAspectRatioEnum>() + 1];

            Add(false, MyAspectRatioEnum.NORMAL_4_3, 4.0f / 3.0f, MyTextsWrapperEnum.AspectRatioNormal_4_3, MyTextsWrapperEnum.AspectRatio_Short_Normal_4_3);
            Add(false, MyAspectRatioEnum.WIDE_16_9, 16.0f / 9.0f, MyTextsWrapperEnum.AspectRatioWide_16_9, MyTextsWrapperEnum.AspectRatio_Short_Normal_16_9);
            Add(false, MyAspectRatioEnum.WIDE_16_10, 16.0f / 10.0f, MyTextsWrapperEnum.AspectRatioWide_16_10, MyTextsWrapperEnum.AspectRatio_Short_Normal_16_10);

            Add(false, MyAspectRatioEnum.DUAL_HEAD_NORMAL_4_3, 2 * 4.0f / 3.0f, MyTextsWrapperEnum.AspectRatioDualHeadNormal_4_3, MyTextsWrapperEnum.AspectRatio_Short_Dual_4_3);
            Add(false, MyAspectRatioEnum.DUAL_HEAD_WIDE_16_9, 2 * 16.0f / 9.0f, MyTextsWrapperEnum.AspectRatioDualHeadWide_16_9, MyTextsWrapperEnum.AspectRatio_Short_Dual_16_9);
            Add(false, MyAspectRatioEnum.DUAL_HEAD_WIDE_16_10, 2 * 16.0f / 10.0f, MyTextsWrapperEnum.AspectRatioDualHeadWide_16_10, MyTextsWrapperEnum.AspectRatio_Short_Dual_16_10);
            
            Add(true, MyAspectRatioEnum.TRIPLE_HEAD_NORMAL_4_3, 3 * 4.0f / 3.0f, MyTextsWrapperEnum.AspectRatioTripleHeadNormal_4_3, MyTextsWrapperEnum.AspectRatio_Short_Triple_4_3);
            Add(true, MyAspectRatioEnum.TRIPLE_HEAD_WIDE_16_9, 3 * 16.0f / 9.0f, MyTextsWrapperEnum.AspectRatioTripleHeadWide_16_9, MyTextsWrapperEnum.AspectRatio_Short_Triple_16_9);
            Add(true, MyAspectRatioEnum.TRIPLE_HEAD_WIDE_16_10, 3 * 16.0f / 10.0f, MyTextsWrapperEnum.AspectRatioTripleHeadWide_16_10, MyTextsWrapperEnum.AspectRatio_Short_Triple_16_10);
        }

        static void Add(bool isTripleHead, MyAspectRatioEnum aspectRatioEnum, float aspectRatioNumber, MyTextsWrapperEnum textLong, MyTextsWrapperEnum textShort)
        {
            List[(int)aspectRatioEnum] = new MyAspectRatioEx(isTripleHead, aspectRatioEnum, aspectRatioNumber, textLong, textShort);
        }

        public static MyAspectRatioEx Get(MyAspectRatioEnum aspectRatioEnum)
        {
            return List[(int)aspectRatioEnum];
        }

        //  Finds aspect ration that is closest to actual Windows desktop aspect ratio (we assume that this aspect ration is good)
        public static MyAspectRatioEnum GetWindowsDesktopClosestAspectRatio(int adapterIndex)
        {
            float actualDesktopAspectRatio = (float)GraphicsAdapter.Adapters[adapterIndex].CurrentDisplayMode.Width / (float)GraphicsAdapter.Adapters[adapterIndex].CurrentDisplayMode.Height;

            MyAspectRatioEnum closestAspectRatioEnum = MyAspectRatioEnum.NORMAL_4_3;    //  We assign this value only because compiler needs some value! It's not default or something!

            float closestDistance = float.MaxValue;
            for (int i = 0; i < List.Length; i++)
            {
                float tempDistance = Math.Abs(actualDesktopAspectRatio - List[i].AspectRatioNumber);
                if (tempDistance < closestDistance)
                {
                    closestDistance = tempDistance;
                    closestAspectRatioEnum = List[i].AspectRatioEnum;
                }
            }

            return closestAspectRatioEnum;
        }

        //  Finds aspect ratio that is closest to aspectRatio paremeter aspect ratio (we assume that this aspect ration is good)
        public static MyAspectRatioEnum GetClosestAspectRatio(float aspectRatio)
        {
            MyAspectRatioEnum closestAspectRatioEnum = MyAspectRatioEnum.NORMAL_4_3;    //  We assign this value only because compiler needs some value! It's not default or something!

            float closestDistance = float.MaxValue;
            for (int i = 0; i < List.Length; i++)
            {
                float tempDistance = Math.Abs(aspectRatio - List[i].AspectRatioNumber);
                if (tempDistance < closestDistance)
                {
                    closestDistance = tempDistance;
                    closestAspectRatioEnum = List[i].AspectRatioEnum;
                }
            }

            return closestAspectRatioEnum;
        }
    }
}
