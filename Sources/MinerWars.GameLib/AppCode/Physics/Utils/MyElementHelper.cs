using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWarsMath;

namespace MinerWars.AppCode.Physics.Utils
{
    static class MyElementHelper
    {
        public static void GetClosestPointForBox(Vector3 size, Vector3 point, ref Vector3 closestPoint, ref Vector3 normal, ref bool penetration, ref uint customData)
        {
            closestPoint = point;
            bool p = true;

            Vector3 corner = size * 0.5f;

            if (point.X < -corner.X)
            {
                closestPoint.X = -corner.X;
                p = false;
            }
            else if (point.X > corner.X)
            {
                closestPoint.X = corner.X;
                p = false;
            }

            if (point.Y < -corner.Y)
            {
                closestPoint.Y = -corner.Y;
                p = false;
            }
            else if (point.Y > corner.Y)
            {
                closestPoint.Y = corner.Y;
                p = false;
            }

            if (point.Z < -corner.Z)
            {
                closestPoint.Z = -corner.Z;
                p = false;
            }
            else if (point.Z > corner.Z)
            {
                closestPoint.Z = corner.Z;
                p = false;
            }

            if (p)
            {
                float pX = corner.X - System.Math.Abs(closestPoint.X);
                float pY = corner.Y - System.Math.Abs(closestPoint.Y);
                float pZ = corner.Z - System.Math.Abs(closestPoint.Z);
                if (pX < pY)
                {
                    if (pX < pZ)
                    {
                        if (closestPoint.X > 0) closestPoint.X += pX;
                        else closestPoint.X -= pX;
                    }
                    else
                    {
                        if (closestPoint.Z > 0) closestPoint.Z += pZ;
                        else closestPoint.Z -= pZ;
                    }
                }
                else
                {
                    if (pY < pZ)
                    {
                        if (closestPoint.Y > 0) closestPoint.Y += pY;
                        else closestPoint.Y -= pY;
                    }
                    else
                    {
                        if (closestPoint.Z > 0) closestPoint.Z += pZ;
                        else closestPoint.Z -= pZ;
                    }
                }
            }

            penetration = p;

            if (p)
                normal = closestPoint - point;
            else
                normal = point - closestPoint;
        }

        public static void GetClosestPointForSphere(float radius, Vector3 point, ref Vector3 closestPoint, ref Vector3 normal, ref bool penetration, ref uint customData)
        {
            float dist = point.Length();
            penetration = (dist < radius);

            //Handle case when point is in sphere origin
            if (dist == 0)
            {
                normal.X = 1.0f;
                normal.Y = 0.0f;
                normal.Z = 0.0f;
                closestPoint.X = radius;
                closestPoint.Y = 0.0f;
                closestPoint.Z = 0.0f;
            }
            else
            {
                float d = dist;
                normal = point / d;
                closestPoint = point * (radius / d);
            }

            customData = 0;
        }
    }
}
