namespace Avalonia.Labs.Controls;

/// <summary>
/// Holds the possible states of the Swipe component
/// </summary>
public enum SwipeState
{
    /// <summary>
    /// Means that all sides are hidden (Default/0)
    /// </summary>
    Hidden = 0,

    /// <summary>
    /// Means that the right side is visible
    /// </summary>
    RightVisible,

    /// <summary>
    /// Means that the left side is visible
    /// </summary>
    LeftVisible,

    /// <summary>
    /// Means that the top side is visible
    /// </summary>
    TopVisible,

    /// <summary>
    /// Means that the bottom side is visible
    /// </summary>
    BottomVisible
}
