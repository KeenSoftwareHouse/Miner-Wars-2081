using System;
using MinerWarsMath;
using MinerWars.AppCode.Game.Managers.Session;

//  This class can be used for visualization of 2D curve (exp, log, hermite, etc).
//  It will will display curve as series of lines on X interval <0..1>, ofcourse magnified so we can see it.
//  Important is, that X is always in <0..1>. And Y too.

namespace MinerWars.AppCode.Game.Utils
{
    static class MyDebugDrawCurve
    {
        public static void DrawCurve()
        {
            Vector3 previousCoord = new Vector3();

            const int CURVE_INTERPOLATION_POINTS = 30;
            const float CURVE_X_LENGTH_IN_METRES = 10;

            for (int i = 0; i <= CURVE_INTERPOLATION_POINTS; i++)
            {
                //  Right now we are in <0..1> range
                Vector3 coord;
                coord.X = i / (float)CURVE_INTERPOLATION_POINTS;
                coord.Y = GetCurveY(coord.X);
                coord.Z = 0;

                //  This is just magnification so we can see the curve
                coord *= CURVE_X_LENGTH_IN_METRES;

                if (i > 0)
                {
                    MyDebugDraw.DrawLine3D(previousCoord, coord, Color.GreenYellow, Color.GreenYellow);
                }

                previousCoord = coord;                
            }

            //  Display boundings for  a curve
            MyDebugDraw.DrawLine3D(Vector3.Zero, new Vector3(CURVE_X_LENGTH_IN_METRES, 0, 0), Color.White, Color.White);
            MyDebugDraw.DrawLine3D(Vector3.Zero, new Vector3(0, CURVE_X_LENGTH_IN_METRES, 0), Color.White, Color.White);
            MyDebugDraw.DrawLine3D(new Vector3(0, CURVE_X_LENGTH_IN_METRES, 0), new Vector3(CURVE_X_LENGTH_IN_METRES, CURVE_X_LENGTH_IN_METRES, 0), Color.White, Color.White);
            MyDebugDraw.DrawLine3D(new Vector3(CURVE_X_LENGTH_IN_METRES, CURVE_X_LENGTH_IN_METRES, 0), new Vector3(CURVE_X_LENGTH_IN_METRES, 0, 0), Color.White, Color.White);
        }

        static float GetCurveY(float x)
        {
            //return x;
            
            //return 1 - (float)Math.Pow(x, 3f);

            return (float)Math.Pow(x, 5f);

            //return (float) Math.Exp(-(x*x));

            //return (float)Math.Pow(1 - x, 5);

            //return 1 - (float)Math.Pow(1 - x, 5);

            //return (float)Math.Pow(1 - x, 3);

            //return MathHelper.SmoothStep(0, 1, x);

            //return MathHelper.Clamp(x + 0.2f, 0.2f, 1);

            //return MathHelper.SmoothStep(0, 2, 1 - x);

            //return (float)Math.Pow(x, 5);

            //return 1 - (float)Math.Pow(x, 2);

            //return (float)Math.Pow(x, 5);
        }
    }
}
