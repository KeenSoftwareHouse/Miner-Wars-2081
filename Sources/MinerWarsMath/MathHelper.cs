using System;

namespace MinerWarsMath
{
    /// <summary>
    /// Contains commonly used precalculated values.
    /// </summary>
    public static class MathHelper
    {
        /// <summary>
        /// Represents the mathematical constant e.
        /// </summary>
        public const float E = 2.718282f;
        /// <summary>
        /// Represents the log base two of e.
        /// </summary>
        public const float Log2E = 1.442695f;
        /// <summary>
        /// Represents the log base ten of e.
        /// </summary>
        public const float Log10E = 0.4342945f;
        /// <summary>
        /// Represents the value of pi.
        /// </summary>
        public const float Pi = 3.141593f;
        /// <summary>
        /// Represents the value of pi times two.
        /// </summary>
        public const float TwoPi = 6.283185f;
        /// <summary>
        /// Represents the value of pi divided by two.
        /// </summary>
        public const float PiOver2 = 1.570796f;
        /// <summary>
        /// Represents the value of pi divided by four.
        /// </summary>
        public const float PiOver4 = 0.7853982f;

        /// <summary>
        /// Converts degrees to radians.
        /// </summary>
        /// <param name="degrees">The angle in degrees.</param>
        public static float ToRadians(float degrees)
        {
            return (degrees / 360.0f) * TwoPi;
        }

        /// <summary>
        /// Converts radians to degrees.
        /// </summary>
        /// <param name="radians">The angle in radians.</param>
        public static float ToDegrees(float radians)
        {
            return radians * 57.29578f;
        }

        /// <summary>
        /// Calculates the absolute value of the difference of two values.
        /// </summary>
        /// <param name="value1">Source value.</param><param name="value2">Source value.</param>
        public static float Distance(float value1, float value2)
        {
            return Math.Abs(value1 - value2);
        }

        /// <summary>
        /// Returns the lesser of two values.
        /// </summary>
        /// <param name="value1">Source value.</param><param name="value2">Source value.</param>
        public static float Min(float value1, float value2)
        {
            return Math.Min(value1, value2);
        }

        /// <summary>
        /// Returns the greater of two values.
        /// </summary>
        /// <param name="value1">Source value.</param><param name="value2">Source value.</param>
        public static float Max(float value1, float value2)
        {
            return Math.Max(value1, value2);
        }

        /// <summary>
        /// Restricts a value to be within a specified range. Reference page contains links to related code samples.
        /// </summary>
        /// <param name="value">The value to clamp.</param><param name="min">The minimum value. If value is less than min, min will be returned.</param><param name="max">The maximum value. If value is greater than max, max will be returned.</param>
        public static float Clamp(float value, float min, float max)
        {
            value = (double)value > (double)max ? max : value;
            value = (double)value < (double)min ? min : value;
            return value;
        }

        /// <summary>
        /// Linearly interpolates between two values.
        /// </summary>
        /// <param name="value1">Source value.</param><param name="value2">Source value.</param><param name="amount">Value between 0 and 1 indicating the weight of value2.</param>
        public static float Lerp(float value1, float value2, float amount)
        {
            return value1 + (value2 - value1) * amount;
        }

        /// <summary>
        /// Returns the Cartesian coordinate for one axis of a point that is defined by a given triangle and two normalized barycentric (areal) coordinates.
        /// </summary>
        /// <param name="value1">The coordinate on one axis of vertex 1 of the defining triangle.</param><param name="value2">The coordinate on the same axis of vertex 2 of the defining triangle.</param><param name="value3">The coordinate on the same axis of vertex 3 of the defining triangle.</param><param name="amount1">The normalized barycentric (areal) coordinate b2, equal to the weighting factor for vertex 2, the coordinate of which is specified in value2.</param><param name="amount2">The normalized barycentric (areal) coordinate b3, equal to the weighting factor for vertex 3, the coordinate of which is specified in value3.</param>
        public static float Barycentric(float value1, float value2, float value3, float amount1, float amount2)
        {
            return (float)((double)value1 + (double)amount1 * ((double)value2 - (double)value1) + (double)amount2 * ((double)value3 - (double)value1));
        }

        /// <summary>
        /// Interpolates between two values using a cubic equation.
        /// </summary>
        /// <param name="value1">Source value.</param><param name="value2">Source value.</param><param name="amount">Weighting value.</param>
        public static float SmoothStep(float value1, float value2, float amount)
        {
            float num = MathHelper.Clamp(amount, 0.0f, 1f);
            return MathHelper.Lerp(value1, value2, (float)((double)num * (double)num * (3.0 - 2.0 * (double)num)));
        }

        /// <summary>
        /// Performs a Catmull-Rom interpolation using the specified positions.
        /// </summary>
        /// <param name="value1">The first position in the interpolation.</param><param name="value2">The second position in the interpolation.</param><param name="value3">The third position in the interpolation.</param><param name="value4">The fourth position in the interpolation.</param><param name="amount">Weighting factor.</param>
        public static float CatmullRom(float value1, float value2, float value3, float value4, float amount)
        {
            float num1 = amount * amount;
            float num2 = amount * num1;
            return (float)(0.5 * (2.0 * (double)value2 + (-(double)value1 + (double)value3) * (double)amount + (2.0 * (double)value1 - 5.0 * (double)value2 + 4.0 * (double)value3 - (double)value4) * (double)num1 + (-(double)value1 + 3.0 * (double)value2 - 3.0 * (double)value3 + (double)value4) * (double)num2));
        }

        /// <summary>
        /// Performs a Hermite spline interpolation.
        /// </summary>
        /// <param name="value1">Source position.</param><param name="tangent1">Source tangent.</param><param name="value2">Source position.</param><param name="tangent2">Source tangent.</param><param name="amount">Weighting factor.</param>
        public static float Hermite(float value1, float tangent1, float value2, float tangent2, float amount)
        {
            float num1 = amount;
            float num2 = num1 * num1;
            float num3 = num1 * num2;
            float num4 = (float)(2.0 * (double)num3 - 3.0 * (double)num2 + 1.0);
            float num5 = (float)(-2.0 * (double)num3 + 3.0 * (double)num2);
            float num6 = num3 - 2f * num2 + num1;
            float num7 = num3 - num2;
            return (float)((double)value1 * (double)num4 + (double)value2 * (double)num5 + (double)tangent1 * (double)num6 + (double)tangent2 * (double)num7);
        }

        /// <summary>
        /// Reduces a given angle to a value between π and -π.
        /// </summary>
        /// <param name="angle">The angle to reduce, in radians.</param>
        public static float WrapAngle(float angle)
        {
            angle = (float)Math.IEEERemainder((double)angle, 6.28318548202515);
            if ((double)angle <= -3.14159274101257)
                angle += 6.283185f;
            else if ((double)angle > 3.14159274101257)
                angle -= 6.283185f;
            return angle;
        }
    }
}
