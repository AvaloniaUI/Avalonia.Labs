using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;

namespace Avalonia.Labs.Controls;

/// <summary>
/// A control representing a selected Item in the <see cref="MultiSelectionComboBox"/>
/// </summary>
[TemplatePart (Name = nameof(PART_RemoveButton), Type = typeof(Button))]
public class MultiSelectionComboBoxSelectionBoxItem : ContentControl
{
    private Button? PART_RemoveButton;
    
    /// <summary>
    /// Defines the <see cref="ShowRemoveButton" /> property
    /// </summary>
    public static readonly StyledProperty<bool> ShowRemoveButtonProperty =
        AvaloniaProperty.Register<MultiSelectionComboBoxSelectionBoxItem, bool>(nameof(ShowRemoveButton));

    /// <summary>
    /// Gets or sets wether the remove button should be visible
    /// </summary>
    public bool ShowRemoveButton
    {
        get { return GetValue(ShowRemoveButtonProperty); }
        set { SetValue(ShowRemoveButtonProperty, value); }
    }

    /// <summary>
    /// Removes this item from <see cref="MultiSelectionComboBox" />.<see cref="MultiSelectionComboBox.SelectedItems"/>
    /// </summary>
    public void RemoveItem()
    {
        if (this.FindLogicalAncestorOfType<MultiSelectionComboBox>() is { } parent)
        {
            parent.RemoveItem(this.DataContext);
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        if (PART_RemoveButton is not null)
        {
            PART_RemoveButton.Click -= PART_RemoveButtonOnClick;
        }

        PART_RemoveButton = e.NameScope.Find<Button>(nameof(PART_RemoveButton));
        
        if (PART_RemoveButton is not null)
        {
            PART_RemoveButton.Click += PART_RemoveButtonOnClick;
        }
    }

    private void PART_RemoveButtonOnClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button { Command: null } )
        {
            RemoveItem();
        }
    }
}