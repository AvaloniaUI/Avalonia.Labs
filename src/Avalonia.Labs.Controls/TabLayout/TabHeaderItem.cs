using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;

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

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            base.OnPointerPressed(e);
            SelectingItemsControl.ItemsControlFromItemContainer(this)?.UpdateSelectionFromEvent(this, e);
        }


        /// <inheritdoc/>
        protected override void OnGotFocus(GotFocusEventArgs e)
        {
            base.OnGotFocus(e);
            SelectingItemsControl.ItemsControlFromItemContainer(this)?.UpdateSelectionFromEvent(this, e);
        }
    }
}
