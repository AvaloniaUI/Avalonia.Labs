using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;

namespace Avalonia.Labs.Controls
{
    public class TabHeader : SelectingItemsControl
    {
        public static readonly StyledProperty<Dock> TabStripPlacementProperty =
            TabControl.TabStripPlacementProperty.AddOwner<TabHeader>();

        public Dock TabStripPlacement
        {
            get { return GetValue(TabStripPlacementProperty); }
        }

        static TabHeader()
        {
            SelectionModeProperty.OverrideDefaultValue<TabHeader>(SelectionMode.AlwaysSelected);
            FocusableProperty.OverrideDefaultValue(typeof(TabHeader), false);
        }

        protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey) => new TabHeaderItem();

        protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
        {
            return NeedsContainer<TabHeaderItem>(item, out recycleKey);
        }

        protected override void PrepareContainerForItemOverride(Control element, object? item, int index)
        {
            if (element is TabHeaderItem pivotHeaderItem)
            {
                if (ItemTemplate is { } it)
                    pivotHeaderItem.ContentTemplate = it;

                pivotHeaderItem.SetValue(TabHeaderItem.TabStripPlacementProperty, TabStripPlacement);
            }
            base.PrepareContainerForItemOverride(element, item, index);
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == TabStripPlacementProperty)
            {
                RefreshContainers();
            }
        }
    }
}
