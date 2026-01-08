using System;

namespace Avalonia.Labs.Controls.Utils;

internal static class MathExtensions
{
    /// <summary>
    /// Checks if two numbers are equal or close to equal
    /// </summary>
    /// <param name="value">the value to compare</param>
    /// <param name="other">the other value to compare</param>
    /// <param name="tolerance">the tolerance to apply (0.001 is the default)</param>
    /// <returns>true if two numbers are close to equal, otherwise false</returns>
    public static bool IsCloseTo(this double value, double other, double tolerance = 0.001)
    {
        return Math.Abs(value - other) < tolerance;
    }
    
    /// <summary>
    /// Checks if a number is equal to or close to <c>0</c>
    /// </summary>
    /// <param name="value">the value to compare</param>
    /// <param name="tolerance">the tolerance to apply (0.001 is the default)</param>
    /// <returns>true if two numbers are close to equal, otherwise false</returns>
    public static bool IsAlmostZero(this double value, double tolerance = 0.001)
    {
        return value.IsCloseTo(0, tolerance);
    }
}
