using System;

namespace Avalonia.Labs.Controls.Base.Pan;

/// <summary>
/// Sets the pan directions
/// </summary>
[Flags]
public enum PanDirection
{
    /// <summary>
    /// Disables the pan
    /// </summary>
    None = 0,
    /// <summary>
    /// Allows pan to left
    /// </summary>
    Left = 1,
    /// <summary>
    /// Allows pan to right
    /// </summary>
    Right = 2,
    /// <summary>
    /// Allows pan up
    /// </summary>
    Up = 4,
    /// <summary>
    /// Allows pan down
    /// </summary>
    Down = 8,
}
