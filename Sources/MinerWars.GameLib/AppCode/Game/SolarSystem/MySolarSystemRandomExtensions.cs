
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWarsMath;
using MinerWars.CommonLIB.AppCode.Utils;

namespace MinerWars.AppCode.Game.SolarSystem
{
    public static class MySolarSystemRandomExtensions
    {
        /// <summary>
        /// Return random value from 0 to 1 in normal distribution
        /// </summary>
        /// <param name="random"></param>
        /// <returns></returns>
        //public static float FloatNormal(this Random random)
        //{
          //  return MyMwcUtils.NormalDistribution(random.Float());
        //}

        public static float Float(this Random random)
        {
            return (float)random.NextDouble();
        }

        public static float Float(this Random random, float min, float max)
        {
            return MathHelper.Lerp(min, max, (float)random.NextDouble());
        }

        public static Vector3 Direction(this Random random)
        {
            return MyMwcUtils.Normalize(new Vector3(Float(random, -1, 1), Float(random, -1, 1), Float(random, -1, 1)));
        }

        /// <typeparam name="T">Must be enum</typeparam>
        public static T Enum<T>(this Random random)
        {
            var values = System.Enum.GetValues(typeof(T));
            return (T)random.Item(values);
        }

        public static object Item(this Random random, Array array)
        {
            int i = random.Next(0, array.Length);
            
            //This is BUG in .NET? It happens once in 50000000 tries
            if (i == array.Length)
                i -= 1;

            return array.GetValue(i);
        }

        public static T Item<T>(this Random random, List<T> list)
        {
            int i = random.Next(0, list.Count);
            //This is BUG in .NET? It happens once in 50000000 tries
            if (i == list.Count)
                i -= 1;

            return list[i];
        }

        /// <summary>
        /// Get random direction near base direction
        /// </summary>
        /// <param name="random"></param>
        /// <param name="baseDirection"></param>
        /// <param name="maxAngleDeviation">Max angle deviation in radians PI/2 is right angle</param>
        /// <returns></returns>
        public static Vector3 Direction(this Random random, Vector3 baseDirection, float maxAngleDeviation)
        {
            Vector3 otherDir = Vector3.Up;
            if (baseDirection == otherDir || baseDirection == -otherDir)
            {
                otherDir = Vector3.Left;
            }

            Vector3 planar1 = Vector3.Cross(baseDirection, otherDir);
            Vector3 planar2 = Vector3.Cross(baseDirection, planar1);
            float maxDist = (float) Math.Tan(maxAngleDeviation);

            return MyMwcUtils.Normalize(baseDirection + planar1 * random.Float(-maxDist, maxDist) + planar2 * random.Float(-maxDist, maxDist));
        }

        public static float FloatCubic(this Random random, float min, float max)
        {
            return MathHelper.SmoothStep(min, max, (float)random.NextDouble());
        }

        public static Vector3 Vector(this Random random, float halfSize)
        {
            return random.Vector(new Vector3(halfSize, halfSize, halfSize));
        }

        public static Vector3 Vector(this Random random, Vector3 halfSize)
        {
            return random.Vector(-halfSize, halfSize);
        }

        public static Vector3 Vector(this Random random, Vector3 min, Vector3 max)
        {
            float x = MathHelper.Lerp(min.X, max.X, random.Float());
            float y = MathHelper.Lerp(min.Y, max.Y, random.Float());
            float z = MathHelper.Lerp(min.Z, max.Z, random.Float());
            return new Vector3(x, y, z);
        }

        public static Color Color(this Random random, Color min, Color max)
        {
            Color c = new Color();
            c.R = (byte)MathHelper.Lerp(min.R, max.R, Float(random));
            c.G = (byte)MathHelper.Lerp(min.G, max.G, Float(random));
            c.B = (byte)MathHelper.Lerp(min.B, max.B, Float(random));
            c.A = (byte)MathHelper.Lerp(min.A, max.A, Float(random));
            return c;
        }

        public static Color Color(this Random random, Color baseColor, float variation)
        {
            Vector3 min = baseColor.ToVector3() * (1 - variation);
            Vector3 max = baseColor.ToVector3() * (1 + variation);

            return Color(random, new Color(min), new Color(max));
        }

        public static Vector3 PointInSphere(this Random random, Vector3 center, float radius)
        {
            return center + Direction(random) * radius * Float(random);
        }

        public static Vector3 PointOnOrbit(this Random random, float orbitRadius, float orbitRadiusDev, float angleRad, float angleRadDev)
        {
            float ang = MathHelper.Lerp(angleRad - angleRadDev, angleRad + angleRadDev, Float(random));
            Vector3 orbitPoint = new Vector3((float)Math.Sin(ang) * orbitRadius, 0, (float)Math.Cos(ang) * orbitRadius);
            Vector3 toOrbit = MyMwcUtils.Normalize(orbitPoint);
            return orbitPoint + MyMwcUtils.Normalize(toOrbit * Float(random, -1, 1) + Vector3.Up * Float(random, -1, 1)) * orbitRadiusDev * Float(random);
        }

        /// <summary>
        /// Each element of array contains chance to be selected, sum of elements must be 1
        /// </summary>
        /// <param name="percentageArray"></param>
        /// <returns>index</returns>
        public static int PercentageIndex(this Random random, float[] percentageArray)
        {
            float rnd = random.Float();
            float sum = 0;
            for (int i = 0; i < percentageArray.Length; i++)
            {
                sum += percentageArray[i];
                if (sum > rnd)
                    return i;
            }
            return percentageArray.Length - 1;
        }

        public static T PercentageIndex<T>(this Random random, Dictionary<T, float> percentage)
        {
            float rnd = random.Float();
            float sum = 0;
            foreach (var i in percentage)
            {
                sum += i.Value;
                if (sum >= rnd)
                {
                    return i.Key;
                }
            }
            System.Diagnostics.Debug.Assert(true, "Percentage sum is not 1");
            return default(T);
        }
    }
}
