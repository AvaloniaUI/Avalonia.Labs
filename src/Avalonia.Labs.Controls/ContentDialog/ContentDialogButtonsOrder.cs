namespace Avalonia.Labs.Controls;

/// <summary>
/// Specifies the logical order of buttons in a content dialog.
/// </summary>
public enum ContentDialogButtonsOrder
{
    /// <summary>
    /// Automatically applies the default order based on the current platform:
    /// - Windows: Primary, Secondary, Close
    /// - macOS: Close, Secondary, Primary
    /// </summary>
    Auto,

    /// <summary>
    /// Orders buttons from left to right:
    /// Primary, Secondary, Close (typically used on Windows).
    /// </summary>
    PrimaryFirst,

    /// <summary>
    /// Orders buttons from left to right:
    /// Close, Secondary, Primary (typically used on macOS).
    /// </summary>
    PrimaryLast
}
