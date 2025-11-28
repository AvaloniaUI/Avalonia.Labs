namespace Avalonia.Labs.Controls;

/// <summary>
/// Specifies how the swipe interaction behaves.
/// </summary>
public enum SwipeMode
{
    /// <summary>
    /// A swipe reveals the swipe items. The user must explicitly tap a swipe item to execute it.
    /// This is the default behavior.
    /// </summary>
    Reveal,

    /// <summary>
    /// A swipe executes the swipe items automatically when the threshold is reached.
    /// Following execution, the swipe items are closed.
    /// </summary>
    Execute
}
