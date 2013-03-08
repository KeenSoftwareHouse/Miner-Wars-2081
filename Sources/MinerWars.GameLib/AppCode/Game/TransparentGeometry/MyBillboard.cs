using System;
using MinerWarsMath;
using MinerWars.AppCode.Game.Utils;
using System.Collections.Generic;

namespace MinerWars.AppCode.Game.TransparentGeometry
{
    //  This class is used for storing and sorting particle billboards
    public class MyBillboard : IComparable
    {
        public MyTransparentMaterialEnum MaterialEnum;
        public float BlendTextureRatio;
        public MyTransparentMaterialEnum BlendMaterial;
        
        //  Use these members only for readonly acces. Change them only by calling Init()
        public Vector3 Position0;
        public Vector3 Position1;
        public Vector3 Position2;
        public Vector3 Position3;
        public Vector4 Color;
        public Vector2 UVOffset;

        //  Distance to camera, for sorting
        public float DistanceSquared;
        public float Size;

        public bool EnableColorize = false;
        public bool Near = false;
        public bool Lowres = false;

        // Used for sorting
        public int Priority;

        public List<MyBillboard> ContainedBillboards = new List<MyBillboard>();

        public void Start(ref MyQuad quad, MyTransparentMaterialEnum materialEnum,
            ref Vector4 color, ref Vector3 origin, bool colorize = false, bool near = false, bool lowres = false)
        {
            Start(ref quad, materialEnum, MyTransparentMaterialEnum.Test, 0, ref color, ref origin, colorize, near, lowres);
        }

        public void Start(ref MyQuad quad, MyTransparentMaterialEnum materialEnum,
            ref Vector4 color, ref Vector3 origin, Vector2 uvOffset, bool colorize = false, bool near = false, bool lowres = false)
        {
            Start(ref quad, materialEnum, MyTransparentMaterialEnum.Test, 0, ref color, ref origin, uvOffset, colorize, near, lowres);
        }

        public void Start(ref MyQuad quad, MyTransparentMaterialEnum materialEnum, MyTransparentMaterialEnum blendMaterial, float textureBlendRatio,
        ref Vector4 color, ref Vector3 origin, bool colorize = false, bool near = false, bool lowres = false)
        {
            Start(ref quad, materialEnum, blendMaterial, textureBlendRatio, ref color, ref origin, Vector2.Zero, colorize, near, lowres);
        }

        //  This method is like a constructor (which we can't use because billboards are allocated from a pool).
        //  It starts/initializes a billboard. Refs used only for optimalization
        public void Start(ref MyQuad quad, MyTransparentMaterialEnum materialEnum, MyTransparentMaterialEnum blendMaterial, float textureBlendRatio,
        ref Vector4 color, ref Vector3 origin, Vector2 uvOffset, bool colorize = false, bool near = false, bool lowres = false)
        {
            MaterialEnum = materialEnum;
            BlendMaterial = blendMaterial;
            BlendTextureRatio = textureBlendRatio;

            MyUtils.AssertIsValid(quad.Point0);
            MyUtils.AssertIsValid(quad.Point1);
            MyUtils.AssertIsValid(quad.Point2);
            MyUtils.AssertIsValid(quad.Point3);
            

            //  Billboard vertexes
            Position0 = quad.Point0;
            Position1 = quad.Point1;
            Position2 = quad.Point2;
            Position3 = quad.Point3;

            UVOffset = uvOffset;

            EnableColorize = colorize;

            if (EnableColorize)
                Size = (Position0 - Position2).Length();

            //  Distance for sorting
            //  IMPORTANT: Must be calculated before we do color and alpha misting, because we need distance there
            DistanceSquared = Vector3.DistanceSquared(MyCamera.Position, origin);

            //  Color
            Color = color;

            Near = near;
            Lowres = lowres;

            //  Alpha depends on distance to camera. Very close bilboards are more transparent, so player won't see billboard errors or rotating billboards
            var mat = MyTransparentMaterialConstants.GetMaterialProperties(MaterialEnum);
            if (mat.AlphaMistingEnable)
                Color *= MathHelper.Clamp(((float)Math.Sqrt(DistanceSquared) - mat.AlphaMistingStart) / (mat.AlphaMistingEnd - mat.AlphaMistingStart), 0, 1);

            ContainedBillboards.Clear();
        }

        //  For sorting particles back-to-front (so bigger distance is first in the list)
        public int CompareTo(object compareToObject)
        {
            var compareToParticle = (MyBillboard)compareToObject;
            return Priority == compareToParticle.Priority ?
                compareToParticle.DistanceSquared.CompareTo(this.DistanceSquared) :
                Priority.CompareTo(compareToParticle.Priority);
        }
    }
}
