namespace Avalonia.Labs.Panels
{
    /// <summary>
    /// Describes the wrap behavior of the <see cref="FlexPanel"/>
    /// </summary>
    public enum FlexWrap
    {
        /// <summary>
        /// The <see cref="FlexPanel"/> is single line.
        /// </summary>
        NoWrap,
        
        /// <summary>
        /// The <see cref="FlexPanel"/> is multi line.
        /// </summary>
        Wrap,
        
        /// <summary>
        /// Same as <see cref="Wrap"/> but new lines are added in the opposite cross-axis direction.
        /// </summary>
        WrapReverse
    }
}
