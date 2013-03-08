namespace MinerWars.AppCode.Game.VideoMode
{
    using System;

    class MyVideoModeEx : IEquatable<MyVideoModeEx>
    {
        public int Width;
        public int Height;
        public float AspectRatio;
        public MyAspectRatioEnum AspectRatioEnum;
        public bool IsTripleHead;
        public bool IsRecommended;

        public MyVideoModeEx(int width, int height, float trueAspectRatio)
        {
            Width = width;
            Height = height;
            AspectRatioEnum = MyAspectRatioExList.GetClosestAspectRatio(trueAspectRatio);
            IsTripleHead = MyAspectRatioExList.Get(AspectRatioEnum).IsTripleHead;// MyAspectRatioExList.IsTripleHeadAspectRatio(AspectRatioEnum);
            AspectRatio = MyAspectRatioExList.Get(AspectRatioEnum).AspectRatioNumber; //  aspect ratio needs to be adjusted to be one of the system defined
        }

        public bool Equals(MyVideoModeEx other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.Width == Width && other.Height == Height && other.AspectRatio.Equals(AspectRatio) && other.IsTripleHead.Equals(IsTripleHead) && Equals(other.AspectRatioEnum, AspectRatioEnum);
        }
    }
}
