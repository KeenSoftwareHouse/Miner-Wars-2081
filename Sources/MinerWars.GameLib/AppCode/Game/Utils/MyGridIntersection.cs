using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWarsMath;
using MinerWars.CommonLIB.AppCode.Utils;
using SysUtils.Utils;
using BulletXNA;

namespace MinerWars.AppCode.Game.Utils
{
    public class MyGridIntersection
    {
        static bool IsPointInside(Vector3 p, MyMwcVector3Int min, MyMwcVector3Int max)
        {
            return (p.X >= min.X && p.X < max.X+1 &&
                    p.Y >= min.Y && p.Y < max.Y+1 &&
                    p.Z >= min.Z && p.Z < max.Z+1);
        }

        static bool IntersectionT(float n, float d, ref float tE, ref float tL)
        {
            if (MyMwcUtils.IsZero(d)) return n <= 0;
            float t = n / d;
            if (d > 0)
            {
                if (t > tL) return false;
                if (t > tE) tE = t;
            }
            else
            {
                if (t < tE) return false;
                if (t < tL) tL = t;
            }
            return true;
        }

        
        // Liang-Barsky line clipping. Return true if the line isn't completely clipped.
        static bool ClipLine(ref Vector3 start, ref Vector3 end, MyMwcVector3Int min, MyMwcVector3Int max)
        {
            Vector3 dir = end - start;
            if (MyMwcUtils.IsZero(dir)) return IsPointInside(start, min, max);
            float tE = 0, tL = 1;

            if (IntersectionT(min.X - start.X, dir.X, ref tE, ref tL) && IntersectionT(start.X - max.X - 1, -dir.X, ref tE, ref tL) &&
                IntersectionT(min.Y - start.Y, dir.Y, ref tE, ref tL) && IntersectionT(start.Y - max.Y - 1, -dir.Y, ref tE, ref tL) &&
                IntersectionT(min.Z - start.Z, dir.Z, ref tE, ref tL) && IntersectionT(start.Z - max.Z - 1, -dir.Z, ref tE, ref tL))
            {
                if (tL < 1) end = start + tL * dir;
                if (tE > 0) start += tE * dir;
                return true;
            }
            return false;
        }

        // Return +1 if a component of v is non-negative, or -1 if it's negative.
        static MyMwcVector3Int SignInt(Vector3 v)
        {
            return new MyMwcVector3Int(v.X >= 0 ? 1 : -1, v.Y >= 0 ? 1 : -1, v.Z >= 0 ? 1 : -1);
        }

        static Vector3 Sign(Vector3 v)
        {
            return new Vector3(v.X >= 0 ? 1 : -1, v.Y >= 0 ? 1 : -1, v.Z >= 0 ? 1 : -1);
        }

        // Get the grid point corresponding to v (in grid coordinates). Guaranteed to lie in the given bounding box (by clamping).
        static MyMwcVector3Int GetGridPoint(ref Vector3 v, MyMwcVector3Int min, MyMwcVector3Int max)
        {
            var r = new MyMwcVector3Int();
            if (v.X < min.X) { v.X = r.X = min.X; }
            else if (v.X >= max.X + 1) { v.X = MathUtil.NextAfter(max.X + 1, float.NegativeInfinity); r.X = max.X; }
            else r.X = (int)Math.Floor(v.X);

            if (v.Y < min.Y) { v.Y = r.Y = min.Y; }
            else if (v.Y >= max.Y + 1) { v.Y = MathUtil.NextAfter(max.Y + 1, float.NegativeInfinity); r.Y = max.Y; }
            else r.Y = (int)Math.Floor(v.Y);
            
            if (v.Z < min.Z) { v.Z = r.Z = min.Z; }
            else if (v.Z >= max.Z + 1) { v.Z = MathUtil.NextAfter(max.Z + 1, float.NegativeInfinity); r.Z = max.Z; }
            else r.Z = (int)Math.Floor(v.Z);

            return r;
        }

        public static void Calculate(List<MyMwcVector3Int> result, float gridSize, Vector3 lineStart, Vector3 lineEnd, MyMwcVector3Int min, MyMwcVector3Int max)
        {
            var dir = lineEnd - lineStart;
            MyCommonDebugUtils.AssertDebug(MyUtils.IsValid(dir));
                        
            // handle start==end
            Vector3 start = lineStart / gridSize;

            if (MyMwcUtils.IsZero(dir))
            {
                if (IsPointInside(start, min, max))
                    result.Add(GetGridPoint(ref start, min, max));
                return;
            }

            // start/end in grid coordinates: clip them to the bounding box, return if no intersection
            Vector3 end = lineEnd / gridSize;
            if (ClipLine(ref start, ref end, min, max) == false) return;


            // reflect coordinates so that dir is always positive
            Vector3 sign = Sign(dir); MyMwcVector3Int signInt = SignInt(dir);

            // current/final grid position
            MyMwcVector3Int cur = GetGridPoint(ref start, min, max) * signInt;
            MyMwcVector3Int final = GetGridPoint(ref end, min, max) * signInt;
            dir *= sign;
            start *= sign;

            // dx = increase of t when we increase x by 1
            // nextX = t of the next point on the line with x whole
            float dx = 1 / dir.X, nextX = dx * ((float)Math.Floor(start.X + 1) - start.X);
            float dy = 1 / dir.Y, nextY = dy * ((float)Math.Floor(start.Y + 1) - start.Y);
            float dz = 1 / dir.Z, nextZ = dz * ((float)Math.Floor(start.Z + 1) - start.Z);

            // 3D DDA
            while (true)
            {
                result.Add(cur * signInt);
                
                if (nextX < nextZ)
                {
                    if (nextX < nextY) { nextX += dx; if (++cur.X > final.X) break; }
                    else { nextY += dy; if (++cur.Y > final.Y) break; }
                }
                else
                {
                    if (nextZ < nextY) { nextZ += dz; if (++cur.Z > final.Z) break; }
                    else { nextY += dy; if (++cur.Y > final.Y) break; }
                }
            }
        }
    }
}
