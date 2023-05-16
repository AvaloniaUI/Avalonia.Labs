using Avalonia.Controls;
using Avalonia.Controls.Metadata;

namespace Avalonia.Labs.Controls
{
    /// <summary>
    /// Represents a tab in a <see cref="TabHeader"/>.
    /// </summary>
    [TemplatePart("PART_LayoutRoot", typeof(Border))]
    public class TabHeaderItem : ListBoxItem
    {
        public static readonly StyledProperty<Dock> TabStripPlacementProperty =
            TabHeader.TabStripPlacementProperty.AddOwner<TabHeaderItem>();

        public Dock TabStripPlacement
        {
            get { return GetValue(TabStripPlacementProperty); }
        }
    }
}
