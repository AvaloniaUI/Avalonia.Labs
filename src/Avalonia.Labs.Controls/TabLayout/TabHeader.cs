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

        /// <inheritdoc/>
        protected override void OnGotFocus(GotFocusEventArgs e)
        {
            base.OnGotFocus(e);

            if (e.NavigationMethod == NavigationMethod.Directional)
            {
                e.Handled = UpdateSelectionFromEventSource(e.Source);
            }
        }

        /// <inheritdoc/>
        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            base.OnPointerPressed(e);

            if (e.Source is Visual source)
            {
                var point = e.GetCurrentPoint(source);

                if (point.Properties.IsLeftButtonPressed)
                {
                    e.Handled = UpdateSelectionFromEventSource(e.Source);
                }
            }
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
