namespace Avalonia.Labs.Panels
{
    
    /// <summary>
    /// Describes the main-axis alignment of items inside a <see cref="FlexPanel"/> line.
    /// </summary>
    public enum JustifyContent
    {
        /// <summary>
        /// Child items are packed toward the start of the line.
        /// </summary>
        /// <remarks>
        /// This is the default value.
        /// The main-axis start margin edge of the first child item on the line is placed flush with the start edge of the line,
        /// and each subsequent child item is placed flush with the preceding item.
        /// </remarks>
        FlexStart,
        
        /// <summary>
        /// Child items are packed toward the end of the line.
        /// </summary>
        /// <remarks>
        /// The main-axis margin edge of the last child item is placed flush with the end edge of the line,
        /// and each preceding child item is placed flush with the subsequent item.
        /// </remarks>
        FlexEnd,
        
        /// <summary>
        /// Child items are packed toward the center of the line.
        /// </summary>
        /// <remarks>
        /// The child items on the line are placed flush with each other and aligned in the center of the line,
        /// with equal amounts of space between the main-axis start edge of the line and the first item on the line
        /// and between the main-axis end edge of the line and the last item on the line.
        /// If the leftover free-space is negative, the child items will overflow equally in both directions.
        /// </remarks>
        Center,
        
        /// <summary>
        /// Child items are evenly distributed in the line, with no space on either end.
        /// </summary>
        /// <remarks>
        /// If the leftover free-space is negative or there is only a single child item on the line,
        /// this value is identical to <see cref="FlexStart"/>.
        /// Otherwise, the main-axis start margin edge of the first child item on the line is placed flush with the main-axis start edge of the line,
        /// the main-axis end margin edge of the last child item on the line is placed flush with the main-axis end edge of the line,
        /// and the remaining child items on the line are distributed so that the spacing between any two adjacent items is the same.
        /// </remarks>
        SpaceBetween,
        
        /// <summary>
        /// Child items are evenly distributed in the line, with half-size spaces on either end.
        /// </summary>
        /// <remarks>
        /// If the leftover free-space is negative or there is only a single child item on the line,
        /// this value is identical to <see cref="Center"/>.
        /// Otherwise, the child items on the line are distributed such that the spacing between any two adjacent child items on the line is the same,
        /// and the spacing between the first/last child items and the <see cref="FlexPanel"/> edges is half the size of the spacing between child items.
        /// </remarks>
        SpaceAround,
        
        /// <summary>
        /// Child items are evenly distributed in the line, with equal-size spaces between each item and on either end.
        /// </summary>
        SpaceEvenly
    }
}
