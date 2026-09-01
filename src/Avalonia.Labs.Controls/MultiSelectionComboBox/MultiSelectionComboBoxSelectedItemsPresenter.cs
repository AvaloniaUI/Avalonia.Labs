using Avalonia.Controls;

namespace Avalonia.Labs.Controls;

public class MultiSelectionComboBoxSelectedItemsPresenter : ItemsControl
{
    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        return new MultiSelectionComboBoxSelectionBoxItem();
    }

    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
    {
        return NeedsContainer<MultiSelectionComboBoxSelectionBoxItem>(item, out recycleKey);
    }
}