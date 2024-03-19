using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;

namespace Avalonia.Labs.Controls;

[TemplatePart(Name = nameof(PART_Popup), Type = typeof(Popup))]
[TemplatePart(Name = "PART_SelectedItemsPresenter", Type = typeof(ItemsControl))]
[TemplatePart(Name = nameof(PART_ItemsPresenter), Type = typeof(ItemsPresenter))]
[TemplatePart(Name = nameof(PART_EditableTextBox), Type = typeof(TextBox))]
[TemplatePart(Name = nameof(PART_DropDownOverlay), Type = typeof(Border))]
public class MultiSelectionComboBox : ListBox
{
    //-------------------------------------------------------------------
    //
    //  Private Members
    // 
    //-------------------------------------------------------------------

    // ReSharper disable InconsistentNaming
    private Popup? PART_Popup;
    private TextBox? PART_EditableTextBox;
    private Border? PART_DropDownOverlay;
    private ItemsPresenter? PART_ItemsPresenter;

    // ReSharper restore InconsistentNaming

    private const string s_pcPressed = ":pressed";

    private bool _isUserDefinedTextInputPending;

    private bool
        _isTextChanging; // This flag indicates if the text is changed by us, so we don't want to re-fire the TextChangedEvent.

    private bool _shouldDoTextReset; // Defines if the Text should be reset after selecting items from string

    private bool
        _shouldAddItems; // Defines if the MultiSelectionComboBox should add new items from text input. Don't set this to true while input is pending. We cannot know how long the user needs for typing.

    private DispatcherTimer? _updateSelectedItemsFromTextTimer;

    //-------------------------------------------------------------------
    //
    //  Constructors
    // 
    //-------------------------------------------------------------------

    static MultiSelectionComboBox()
    {
        FocusableProperty.OverrideDefaultValue<MultiSelectionComboBox>(true);

        // Listen for SelectionChanges
        SelectionChangedEvent.AddClassHandler<MultiSelectionComboBox>((s, e) =>
        {
            s.UpdateDisplaySelectedItems();
            s.UpdateEditableText();
        });
    }

    //-------------------------------------------------------------------
    //
    //  Public Properties
    // 
    //-------------------------------------------------------------------

    /// <summary>
    /// Defines the <see cref="DisplaySelectedItems" /> property
    /// </summary>
    internal static readonly DirectProperty<MultiSelectionComboBox, IEnumerable?> DisplaySelectedItemsProperty =
        AvaloniaProperty.RegisterDirect<MultiSelectionComboBox, IEnumerable?>(
            nameof(DisplaySelectedItems),
            o => o.DisplaySelectedItems);

    private IEnumerable? _displaySelectedItems;

    /// <summary>
    /// Gets the <see cref="ListBox.SelectedItems"/> in the specified order which was set via <see cref="OrderSelectedItemsBy"/>
    /// </summary>
    internal IEnumerable? DisplaySelectedItems
    {
        get => _displaySelectedItems;
        private set => SetAndRaise(DisplaySelectedItemsProperty, ref _displaySelectedItems, value);
    }

    /// <summary>
    /// Defines the <see cref="OrderSelectedItemsBy" /> property
    /// </summary>
    public static readonly StyledProperty<SelectedItemsOrderType> OrderSelectedItemsByProperty =
        AvaloniaProperty.Register<MultiSelectionComboBox, SelectedItemsOrderType>(nameof(OrderSelectedItemsBy));

    /// <summary>
    /// Gets or sets how the <see cref="ListBox.SelectedItems"/> should be sorted
    /// </summary>
    public SelectedItemsOrderType OrderSelectedItemsBy
    {
        get => GetValue(OrderSelectedItemsByProperty);
        set => SetValue(OrderSelectedItemsByProperty, value);
    }

    /// <summary>
    /// Defines the <see cref="Text" /> property
    /// </summary>
    public static readonly StyledProperty<string?> TextProperty =
        TextBox.TextProperty.AddOwner<MultiSelectionComboBox>(
            new StyledPropertyMetadata<string?>(defaultBindingMode: BindingMode.TwoWay));

    /// <summary>
    /// Gets or sets the Text in the Editable mode
    /// </summary>
    public string? Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }


    /// <summary>
    /// Defines the <see cref="IsEditable" /> property
    /// </summary>
    public static readonly StyledProperty<bool> IsEditableProperty =
        AvaloniaProperty.Register<MultiSelectionComboBox, bool>(nameof(IsEditable));

    /// <summary>
    /// Gets or sets if the MultiSelectionComboBox is editable
    /// </summary>
    public bool IsEditable
    {
        get { return GetValue(IsEditableProperty); }
        set { SetValue(IsEditableProperty, value); }
    }

    /// <summary>
    /// Defines the <see cref="IsReadOnly" /> property
    /// </summary>
    public static readonly StyledProperty<bool> IsReadOnlyProperty =
        TextBox.IsReadOnlyProperty.AddOwner<MultiSelectionComboBox>();

    /// <summary>
    /// Gets or sets if the editable TextBox is Read-only
    /// </summary>
    public bool IsReadOnly
    {
        get { return GetValue(IsReadOnlyProperty); }
        set { SetValue(IsReadOnlyProperty, value); }
    }

    /// <summary>
    /// Defines the <see cref="Separator" /> property
    /// </summary>
    public static readonly StyledProperty<string?> SeparatorProperty =
        AvaloniaProperty.Register<MultiSelectionComboBox, string?>(nameof(Separator));

    /// <summary>
    /// Gets or Sets the Separator which will be used if the ComboBox is editable.
    /// </summary>
    public string? Separator
    {
        get => GetValue(SeparatorProperty);
        set => SetValue(SeparatorProperty, value);
    }

    /// <summary>
    /// Defines the <see cref="HasCustomText" /> property
    /// </summary>
    public static readonly DirectProperty<MultiSelectionComboBox, bool> HasCustomTextProperty =
        AvaloniaProperty.RegisterDirect<MultiSelectionComboBox, bool>(
            nameof(HasCustomText),
            o => o.HasCustomText);

    private bool _HasCustomText;

    /// <summary>
    /// Indicates if the text is user defined
    /// </summary>
    public bool HasCustomText
    {
        get => _HasCustomText;
        private set => SetAndRaise(HasCustomTextProperty, ref _HasCustomText, value);
    }

    /// <summary>
    /// Defines the <see cref="TextWrapping" /> property
    /// </summary>
    public static readonly StyledProperty<TextWrapping> TextWrappingProperty =
        TextBox.TextWrappingProperty.AddOwner<MultiSelectionComboBox>();

    /// <summary>
    /// The TextWrapping property controls whether or not text wraps
    /// when it reaches the flow edge of its containing block box.
    /// </summary>
    public TextWrapping TextWrapping
    {
        get => GetValue(TextWrappingProperty);
        set => SetValue(TextWrappingProperty, value);
    }


    /// <summary>
    /// Defines the <see cref="AcceptsReturn" /> property
    /// </summary>
    public static readonly StyledProperty<bool> AcceptsReturnProperty =
        TextBox.AcceptsReturnProperty.AddOwner<MultiSelectionComboBox>();

    /// <summary>
    /// Does the editable TextBox accept return key
    /// </summary>
    public bool AcceptsReturn
    {
        get => GetValue(AcceptsReturnProperty);
        set => SetValue(AcceptsReturnProperty, value);
    }

    /// <summary>
    /// Defines the <see cref="ObjectToStringComparer" /> property
    /// </summary>
    public static readonly StyledProperty<ICompareObjectToString?> ObjectToStringComparerProperty =
        AvaloniaProperty.Register<MultiSelectionComboBox, ICompareObjectToString?>(nameof(ObjectToStringComparer));

    /// <summary>
    /// Defines the <see cref="IsDropDownOpen" /> property
    /// </summary>
    public static readonly StyledProperty<bool> IsDropDownOpenProperty =
        ComboBox.IsDropDownOpenProperty.AddOwner<MultiSelectionComboBox>(
            new StyledPropertyMetadata<bool>(defaultBindingMode: BindingMode.TwoWay));

    /// <summary>
    /// Gets or sets if the DropDown is open
    /// </summary>
    public bool IsDropDownOpen
    {
        get => GetValue(IsDropDownOpenProperty);
        set => SetValue(IsDropDownOpenProperty, value);
    }

    /// <summary>
    /// Defines the <see cref="MaxDropDownHeight" /> property
    /// </summary>
    public static readonly StyledProperty<double> MaxDropDownHeightProperty =
        ComboBox.MaxDropDownHeightProperty.AddOwner<MultiSelectionComboBox>();

    /// <summary>
    /// Gets or sets the MaxDropDownHeight
    /// </summary>
    public double MaxDropDownHeight
    {
        get { return GetValue(MaxDropDownHeightProperty); }
        set { SetValue(MaxDropDownHeightProperty, value); }
    }

    /// <summary>
    /// Defines the <see cref="HorizontalContentAlignment" /> property
    /// </summary>
    public static readonly StyledProperty<HorizontalAlignment> HorizontalContentAlignmentProperty =
        ComboBox.HorizontalContentAlignmentProperty.AddOwner<MultiSelectionComboBox>();

    /// <summary>
    /// Gets or sets the horizontal content alignment
    /// </summary>
    public HorizontalAlignment HorizontalContentAlignment
    {
        get { return GetValue(HorizontalContentAlignmentProperty); }
        set { SetValue(HorizontalContentAlignmentProperty, value); }
    }

    /// <summary>
    /// Defines the <see cref="VerticalContentAlignment" /> property
    /// </summary>
    public static readonly StyledProperty<VerticalAlignment> VerticalContentAlignmentProperty =
        ComboBox.VerticalContentAlignmentProperty.AddOwner<MultiSelectionComboBox>();

    /// <summary>
    /// Gets or sets the vertical content alignment
    /// </summary>
    public VerticalAlignment VerticalContentAlignment
    {
        get { return GetValue(VerticalContentAlignmentProperty); }
        set { SetValue(VerticalContentAlignmentProperty, value); }
    }

    /// <summary>
    /// Gets or Sets a function that is used to check if the entered Text is an object that should be selected.
    /// </summary>
    public ICompareObjectToString? ObjectToStringComparer
    {
        get => GetValue(ObjectToStringComparerProperty);
        set => SetValue(ObjectToStringComparerProperty, value);
    }

    /// <summary>
    /// Defines the <see cref="EditableTextStringComparision" /> property
    /// </summary>
    public static readonly StyledProperty<StringComparison> EditableTextStringComparisionProperty =
        AvaloniaProperty.Register<MultiSelectionComboBox, StringComparison>(nameof(EditableTextStringComparision),
            StringComparison.OrdinalIgnoreCase);

    /// <summary>
    ///  Gets or Sets the <see cref="StringComparison"/> that is used to check if the entered <see cref="Text"/> matches to the <see cref="ListBox.SelectedItems"/>
    /// </summary>
    public StringComparison EditableTextStringComparision
    {
        get => GetValue(EditableTextStringComparisionProperty);
        set => SetValue(EditableTextStringComparisionProperty, value);
    }

    /// <summary>
    /// Defines the <see cref="StringToObjectParser" /> property
    /// </summary>
    public static readonly StyledProperty<IParseStringToObject?> StringToObjectParserProperty =
        AvaloniaProperty.Register<MultiSelectionComboBox, IParseStringToObject?>(nameof(StringToObjectParser));

    /// <summary>
    /// comment
    /// </summary>
    public IParseStringToObject? StringToObjectParser
    {
        get => GetValue(StringToObjectParserProperty);
        set => SetValue(StringToObjectParserProperty, value);
    }


    /// <summary>
    /// Defines the <see cref="DisabledPopupOverlayContent" /> property
    /// </summary>
    public static readonly StyledProperty<object?> DisabledPopupOverlayContentProperty =
        AvaloniaProperty.Register<MultiSelectionComboBox, object?>(nameof(DisabledPopupOverlayContent));

    /// <summary>
    /// Gets or Sets the DisabledPopupOverlayContent
    /// </summary>
    public object? DisabledPopupOverlayContent
    {
        get => GetValue(DisabledPopupOverlayContentProperty);
        set => SetValue(DisabledPopupOverlayContentProperty, value);
    }

    /// <summary>
    /// Defines the <see cref="DisabledPopupOverlayContentTemplate" /> property
    /// </summary>
    public static readonly StyledProperty<IDataTemplate?> DisabledPopupOverlayContentTemplateProperty =
        AvaloniaProperty.Register<MultiSelectionComboBox, IDataTemplate?>(
            nameof(DisabledPopupOverlayContentTemplate));

    /// <summary>
    /// Gets or Sets the DisabledPopupOverlayContentTemplate
    /// </summary>
    public IDataTemplate? DisabledPopupOverlayContentTemplate
    {
        get => GetValue(DisabledPopupOverlayContentTemplateProperty);
        set => SetValue(DisabledPopupOverlayContentTemplateProperty, value);
    }


    /// <summary>
    /// Defines the <see cref="SelectedItemTemplate" /> property
    /// </summary>
    public static readonly StyledProperty<IDataTemplate?> SelectedItemTemplateProperty =
        AvaloniaProperty.Register<MultiSelectionComboBox, IDataTemplate?>(nameof(SelectedItemTemplate));

    /// <summary>
    /// Gets or Sets the SelectedItemTemplate
    /// </summary>
    public IDataTemplate? SelectedItemTemplate
    {
        get => GetValue(SelectedItemTemplateProperty);
        set => SetValue(SelectedItemTemplateProperty, value);
    }

    /// <summary>
    /// Defines the <see cref="SelectedItemStringFormat" /> property
    /// </summary>
    public static readonly StyledProperty<string?> SelectedItemStringFormatProperty =
        AvaloniaProperty.Register<MultiSelectionComboBox, string?>(nameof(SelectedItemStringFormat));

    /// <summary>
    /// Gets or Sets the string format for the selected items
    /// </summary>
    public string? SelectedItemStringFormat
    {
        get => GetValue(SelectedItemStringFormatProperty);
        set => SetValue(SelectedItemStringFormatProperty, value);
    }


    /// <summary>
    /// Defines the <see cref="SelectedItemsItemPanel" /> property
    /// </summary>
    public static readonly StyledProperty<ItemsPanelTemplate?> SelectedItemsItemPanelProperty =
        AvaloniaProperty.Register<MultiSelectionComboBox, ItemsPanelTemplate?>(nameof(SelectedItemsItemPanel));

    /// <summary>
    /// Gets or sets the <see cref="ItemsPanelTemplate"/> for the selected items.
    /// </summary>
    public ItemsPanelTemplate? SelectedItemsItemPanel
    {
        get => GetValue(SelectedItemsItemPanelProperty);
        set => SetValue(SelectedItemsItemPanelProperty, value);
    }

    /// <summary>
    /// Defines the <see cref="SelectItemsFromTextInputDelay" /> property
    /// </summary>
    public static readonly StyledProperty<int> SelectItemsFromTextInputDelayProperty =
        AvaloniaProperty.Register<MultiSelectionComboBox, int>(nameof(SelectItemsFromTextInputDelay), -1);

    /// <summary>
    /// Gets or Sets the delay in milliseconds to wait before the selection is updated during text input.
    /// If this value is -1 the selection will not be updated during text input. 
    /// Note: You also need to set <see cref="ObjectToStringComparer"/> to get this to work. 
    /// </summary>
    public int SelectItemsFromTextInputDelay
    {
        get => GetValue(SelectItemsFromTextInputDelayProperty);
        set => SetValue(SelectItemsFromTextInputDelayProperty, value);
    }

    /// <summary>
    /// Defines the <see cref="PlaceholderText" /> property
    /// </summary>
    public static readonly StyledProperty<string?> PlaceholderTextProperty =
        AvaloniaProperty.Register<MultiSelectionComboBox, string?>(nameof(PlaceholderText));

    /// <summary>
    /// Gets or sets the placeholder text
    /// </summary>
    public string? PlaceholderText
    {
        get { return GetValue(PlaceholderTextProperty); }
        set { SetValue(PlaceholderTextProperty, value); }
    }

    /// <summary>
    /// Defines the <see cref="PlaceholderForeground" /> property
    /// </summary>
    public static readonly StyledProperty<IBrush> PlaceholderForegroundProperty =
        AvaloniaProperty.Register<MultiSelectionComboBox, IBrush>(nameof(PlaceholderForeground));

    /// <summary>
    /// Gets or sets the Foreground for the Placeholder
    /// </summary>
    public IBrush PlaceholderForeground
    {
        get { return GetValue(PlaceholderForegroundProperty); }
        set { SetValue(PlaceholderForegroundProperty, value); }
    }

    /// <summary>
    /// Resets the custom Text to the selected Items text 
    /// </summary>
    public void ResetEditableText(bool forceUpdate = false)
    {
        if (PART_EditableTextBox is not null)
        {
            var oldSelectionStart = PART_EditableTextBox.SelectionStart;
            var oldSelectionEnd = PART_EditableTextBox.SelectionEnd;

            HasCustomText = false;
            UpdateEditableText(forceUpdate);

            PART_EditableTextBox.SelectionStart = oldSelectionStart;
            PART_EditableTextBox.SelectionEnd = oldSelectionEnd;
        }
    }

    /// <summary>
    /// Defines the <see cref="IsDropDownHeaderVisible" /> property
    /// </summary>
    public static readonly StyledProperty<bool> IsDropDownHeaderVisibleProperty =
        AvaloniaProperty.Register<MultiSelectionComboBox, bool>(nameof(IsDropDownHeaderVisible));

    /// <summary>
    /// Gets or Sets if the Header in the DropDown is visible
    /// </summary>
    public bool IsDropDownHeaderVisible
    {
        get => GetValue(IsDropDownHeaderVisibleProperty);
        set => SetValue(IsDropDownHeaderVisibleProperty, value);
    }

    /// <summary>
    /// Defines the <see cref="DropDownHeaderContent" /> property
    /// </summary>
    public static readonly StyledProperty<object?> DropDownHeaderContentProperty =
        AvaloniaProperty.Register<MultiSelectionComboBox, object?>(nameof(DropDownHeaderContent));

    /// <summary>
    /// Gets or Sets the content of the DropDown-Header
    /// </summary>
    public object? DropDownHeaderContent
    {
        get => GetValue(DropDownHeaderContentProperty);
        set => SetValue(DropDownHeaderContentProperty, value);
    }

    /// <summary>
    /// Defines the <see cref="DropDownHeaderContentTemplate" /> property
    /// </summary>
    public static readonly StyledProperty<IDataTemplate?> DropDownHeaderContentTemplateProperty =
        AvaloniaProperty.Register<MultiSelectionComboBox, IDataTemplate?>(nameof(DropDownHeaderContentTemplate));

    /// <summary>
    /// Gets or Sets the content template of the DropDown-Header
    /// </summary>
    public IDataTemplate? DropDownHeaderContentTemplate
    {
        get => GetValue(DropDownHeaderContentTemplateProperty);
        set => SetValue(DropDownHeaderContentTemplateProperty, value);
    }

    /// <summary>
    /// Defines the <see cref="IsDropDownFooterVisible" /> property
    /// </summary>
    public static readonly StyledProperty<bool> IsDropDownFooterVisibleProperty =
        AvaloniaProperty.Register<MultiSelectionComboBox, bool>(nameof(IsDropDownFooterVisible));

    /// <summary>
    /// Gets or Sets if the Footer in the DropDown is visible
    /// </summary>
    public bool IsDropDownFooterVisible
    {
        get => GetValue(IsDropDownFooterVisibleProperty);
        set => SetValue(IsDropDownFooterVisibleProperty, value);
    }

    /// <summary>
    /// Defines the <see cref="DropDownFooterContent" /> property
    /// </summary>
    public static readonly StyledProperty<object?> DropDownFooterContentProperty =
        AvaloniaProperty.Register<MultiSelectionComboBox, object?>(nameof(DropDownFooterContent));

    /// <summary>
    /// Gets or Sets the content of the DropDown-Footer
    /// </summary>
    public object? DropDownFooterContent
    {
        get => GetValue(DropDownFooterContentProperty);
        set => SetValue(DropDownFooterContentProperty, value);
    }

    /// <summary>
    /// Defines the <see cref="DropDownFooterContentTemplate" /> property
    /// </summary>
    public static readonly StyledProperty<IDataTemplate?> DropDownFooterContentTemplateProperty =
        AvaloniaProperty.Register<MultiSelectionComboBox, IDataTemplate?>(nameof(DropDownFooterContentTemplate));

    /// <summary>
    /// Gets or Sets the content template of the DropDown-Footer
    /// </summary>
    public IDataTemplate? DropDownFooterContentTemplate
    {
        get => GetValue(DropDownFooterContentTemplateProperty);
        set => SetValue(DropDownFooterContentTemplateProperty, value);
    }


    /// <summary>
    /// Defines the <see cref="TextBoxTheme" /> property
    /// </summary>
    public static readonly StyledProperty<ControlTheme> TextBoxThemeProperty =
        AvaloniaProperty.Register<MultiSelectionComboBox, ControlTheme>(nameof(TextBoxTheme));

    /// <summary>
    /// Gets or sets the style for the editable TextBox
    /// </summary>
    public ControlTheme TextBoxTheme
    {
        get { return GetValue(TextBoxThemeProperty); }
        set { SetValue(TextBoxThemeProperty, value); }
    }


    /// <summary>
    /// Updates the Text of the editable TextBox.
    /// Sets the custom Text if any otherwise the concatenated string.
    /// </summary> 
    private void UpdateEditableText(bool forceUpdate = false)
    {
        if (PART_EditableTextBox is null || (PART_EditableTextBox.IsKeyboardFocusWithin && !forceUpdate))
        {
            return;
        }

        _isTextChanging = true;

        var oldSelectionStart = PART_EditableTextBox.SelectionStart;
        var oldSelectionEnd = PART_EditableTextBox.SelectionEnd;
        var oldTextLength = PART_EditableTextBox.Text?.Length ?? 0;

        var selectedItemsText = GetSelectedItemsText();

        if (!HasCustomText)
        {
            SetCurrentValue(TextProperty, selectedItemsText);
        }

        if (oldSelectionStart == 0 &&
            oldSelectionEnd == oldTextLength) // We had all Text selected, so we select all again
        {
            PART_EditableTextBox.SelectAll();
        }
        else if
            (oldSelectionStart ==
             oldTextLength) // we had the cursor at the last position, so we move the cursor to the end again
        {
            PART_EditableTextBox.SelectionStart = PART_EditableTextBox.Text?.Length ?? 0;
        }
        else // we restore the old selection
        {
            PART_EditableTextBox.SelectionStart = oldSelectionStart;
            PART_EditableTextBox.SelectionEnd = oldSelectionEnd;
        }

        UpdateHasCustomText(selectedItemsText);
        _isTextChanging = false;
    }

    private void UpdateDisplaySelectedItems()
    {
        UpdateDisplaySelectedItems(OrderSelectedItemsBy);
    }

    public string? GetSelectedItemsText()
    {
        if (SelectionMode.HasFlag(SelectionMode.Multiple))
        {
            IEnumerable<object>? values = null;

            if (DisplayMemberBinding is not null || SelectedItemStringFormat != null)
            {
                //TODO: values = this.DisplaySelectedItems?.OfType<object>().Select(o =>
                //         BindingHelper.Eval(o, this.DisplayMemberPath ?? string.Empty,
                //             this.SelectedItemStringFormat)) as
                //     IEnumerable<object>;
            }
            else
            {
                values = DisplaySelectedItems as IEnumerable<object>;
            }

            return values is null ? null : string.Join(Separator ?? string.Empty, values);
        }
        else
        {
            if (DisplayMemberBinding is not null || SelectedItemStringFormat != null)
            {
                // TODO
                // return BindingHelper.Eval(this.SelectedItem, this.DisplayMemberPath ?? string.Empty,
                // this.SelectedItemStringFormat)?.ToString();
                return null;
            }
            else
            {
                return SelectedItem?.ToString();
            }
        }
    }

    private void UpdateHasCustomText(string? selectedItemsText)
    {
        // if the parameter was null lets get the text on our own.
        selectedItemsText ??= GetSelectedItemsText();

        HasCustomText = !((string.IsNullOrEmpty(selectedItemsText) && string.IsNullOrEmpty(Text))
                          || string.Equals(Text, selectedItemsText, EditableTextStringComparision));
    }

    private void UpdateDisplaySelectedItems(SelectedItemsOrderType selectedItemsOrderType)
    {
        DisplaySelectedItems = selectedItemsOrderType switch
        {
            SelectedItemsOrderType.SelectedOrder => SelectedItems,
            SelectedItemsOrderType.ItemsSourceOrder => (SelectedItems as IEnumerable<object>)?.OrderBy(o =>
                Items.IndexOf(o)),
            _ => DisplaySelectedItems
        };
    }

#if NET6_0_OR_GREATER
    [RequiresUnreferencedCode("Use 'MethodFriendlyToTrimming' instead")]
#endif
    private void SelectItemsFromText(int millisecondsToWait)
    {
        if (!_isUserDefinedTextInputPending || _isTextChanging)
        {
            return;
        }

        // We want to do a text reset or add items only if we don't need to wait for more input. 
        _shouldDoTextReset = millisecondsToWait == 0;
        _shouldAddItems = millisecondsToWait == 0;

        if (_updateSelectedItemsFromTextTimer is null)
        {
            _updateSelectedItemsFromTextTimer = new DispatcherTimer(DispatcherPriority.Background);
            _updateSelectedItemsFromTextTimer.Tick += UpdateSelectedItemsFromTextTimer_Tick;
        }

        if (_updateSelectedItemsFromTextTimer.IsEnabled)
        {
            _updateSelectedItemsFromTextTimer.Stop();
        }

        if (ObjectToStringComparer is not null &&
            (!string.IsNullOrEmpty(Separator) || SelectionMode == SelectionMode.Single))
        {
            _updateSelectedItemsFromTextTimer.Interval =
                TimeSpan.FromMilliseconds(millisecondsToWait > 0 ? millisecondsToWait : 0);
            _updateSelectedItemsFromTextTimer.Start();
        }
    }

#if NET6_0_OR_GREATER
    [RequiresUnreferencedCode("Use 'MethodFriendlyToTrimming' instead")]
#endif
    private void UpdateSelectedItemsFromTextTimer_Tick(object? sender, EventArgs e)
    {
        _updateSelectedItemsFromTextTimer?.Stop();

        // We clear the selection if there is no text available. 
        if (string.IsNullOrEmpty(Text))
        {
            Selection.Clear();
            return;
        }

        bool foundItem;

        if (SelectionMode.HasFlag(SelectionMode.Multiple) && SelectedItems is not null)
        {
            var strings = !string.IsNullOrEmpty(Separator) ?
                Text?.Split(new[] { Separator }, StringSplitOptions.RemoveEmptyEntries) :
                null;

            int position = 0;

            if (strings is not null)
            {
                foreach (var stringObject in strings)
                {
                    foundItem = false;
                    if (ObjectToStringComparer is not null)
                    {
                        foreach (var item in Items)
                        {
                            if (ObjectToStringComparer.CheckIfStringMatchesObject(stringObject, item,
                                    EditableTextStringComparision, SelectedItemStringFormat))
                            {
                                var oldPosition = SelectedItems.IndexOf(item);

                                if (oldPosition <
                                    0) // if old index is <0 the item is not in list yet. let's add it.
                                {
                                    SelectedItems.Insert(position, item);
                                    foundItem = true;
                                    position++;
                                }
                                else if
                                    (oldPosition >
                                     position) // if the item has a wrong position in list we need to swap it accordingly.
                                {
                                    SelectedItems.RemoveAt(oldPosition);
                                    SelectedItems.Insert(position, item);

                                    foundItem = true;
                                    position++;
                                }
                                else if (oldPosition == position)
                                {
                                    foundItem = true;
                                    position++;
                                }
                            }
                        }
                    }

                    if (!foundItem)
                    {
                        if (_shouldAddItems && TryAddObjectFromString(stringObject, out var result))
                        {
                            SelectedItems.Insert(position, result);
                            position++;
                        }
                        else
                        {
                            _shouldDoTextReset = false;
                        }
                    }
                }
            }

            // Remove Items if needed.
            while (SelectedItems.Count > position)
            {
                SelectedItems.RemoveAt(position);
            }
        }
        else if (!SelectionMode.HasFlag(SelectionMode.Multiple))
        {
            SetCurrentValue(SelectedItemProperty, null);

            foundItem = false;
            if (ObjectToStringComparer is not null)
            {
                foreach (var item in Items)
                {
                    if (ObjectToStringComparer.CheckIfStringMatchesObject(Text, item,
                            EditableTextStringComparision, SelectedItemStringFormat))
                    {
                        SetCurrentValue(SelectedItemProperty, item);
                        foundItem = true;
                        break;
                    }
                }
            }

            if (!foundItem)
            {
                // We try to add a new item. If we were able to do so we need to update the text as it may differ. 
                if (_shouldAddItems && TryAddObjectFromString(Text, out var result))
                {
                    SetCurrentValue(SelectedItemProperty, result);
                }
                else
                {
                    _shouldDoTextReset =
                        false; // We did not find the needed item so we should not do the text reset.
                }
            }
        }


        // First we need to check if the string matches completely to the selected items. Therefore we need to display the items in the selected order first
        UpdateDisplaySelectedItems(SelectedItemsOrderType.SelectedOrder);
        UpdateHasCustomText(null);

        // If the items should be ordered differently than above we need to reorder them accordingly.
        if (OrderSelectedItemsBy != SelectedItemsOrderType.SelectedOrder)
        {
            UpdateDisplaySelectedItems();
        }

        if (PART_EditableTextBox is not null)
        {
            // We do a text reset if all items were successfully found and we don't have to wait for more input.
            if (_shouldDoTextReset)
            {
                var oldCaretPos = PART_EditableTextBox.CaretIndex;
                ResetEditableText();
                PART_EditableTextBox.CaretIndex = oldCaretPos;
            }

            // If we have the KeyboardFocus we need to update the text later in order to not interrupt the user.
            // Therefore we connect this flag to the KeyboardFocus of the TextBox.
            _isUserDefinedTextInputPending = PART_EditableTextBox.IsKeyboardFocusWithin;
        }
    }

#if NET6_0_OR_GREATER
    [RequiresUnreferencedCode("Use 'MethodFriendlyToTrimming' instead")]
#endif
    private bool TryAddObjectFromString(string? input, out object? result)
    {
        try
        {
            if (StringToObjectParser is null || (PART_EditableTextBox?.IsKeyboardFocusWithin ?? false))
            {
                result = null;
                return false;
            }

            var elementType = DefaultStringToObjectParser.Instance.GetElementType(ItemsSource);

            var foundItem = StringToObjectParser.TryCreateObjectFromString(input, out result,
                CultureInfo.CurrentUICulture, SelectedItemStringFormat, elementType);

            var addingItemEventArgs = new AddingItemEventArgs(AddingItemEvent,
                this,
                input,
                result,
                foundItem,
                ItemsSource as IList,
                elementType,
                SelectedItemStringFormat,
                CultureInfo.CurrentUICulture,
                StringToObjectParser);

            RaiseEvent(addingItemEventArgs);

            if (addingItemEventArgs.Handled)
            {
                addingItemEventArgs.Accepted = false;
            }

            // If the adding event was not handled and the item is marked as accepted and we are allowed to modify the items list we can add the pared item
            if (addingItemEventArgs.Accepted && (!addingItemEventArgs.TargetList?.IsReadOnly ?? false))
            {
                addingItemEventArgs.TargetList?.Add(addingItemEventArgs.ParsedObject);

                RaiseEvent(new AddedItemEventArgs(AddedItemEvent, this, addingItemEventArgs.ParsedObject,
                    addingItemEventArgs.TargetList));
            }

            result = addingItemEventArgs.ParsedObject;
            return addingItemEventArgs.Accepted;
        }
        catch (Exception e)
        {
            Trace.WriteLine(e.Message);
            result = null;
            return false;
        }
    }


// Clear Text Command
    public void Clear()
    {
        if (HasCustomText)
        {
            ResetEditableText(true);
        }
        else
        {
            Selection.Clear();
            ResetEditableText(true);
        }
    }


    public void RemoveItem(object? item)
    {
        if (item is null) return;

        if (SelectionMode.HasFlag(SelectionMode.Multiple) && SelectedItems != null && SelectedItems.Contains(item))
        {
            SelectedItems.Remove(item);
        }
        else if (SelectionMode.HasFlag(SelectionMode.Single))
        {
            Selection.SelectedItem = null;
        }
    }
#if NET6_0_OR_GREATER
    [UnconditionalSuppressMessage("Trimming",
        "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code",
        Justification = "<Pending>")]
#endif
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        if (PART_EditableTextBox != null)
        {
            PART_EditableTextBox.LostFocus -= PART_EditableTextBox_LostFocus;
        }

        if (PART_DropDownOverlay != null)
        {
            PART_DropDownOverlay.PointerPressed -= OnDropDownOverlayPointerPressed;
            PART_DropDownOverlay.PointerReleased -= OnDropDownOverlayPointerReleased;
            PART_DropDownOverlay.PointerCaptureLost -= OnDropDownOverlayPointerCaptureLost;
        }

        if (PART_Popup is not null)
        {
            PART_Popup.GotFocus -= PART_PopupOnGotFocus;
        }

        base.OnApplyTemplate(e);

        // Init template parts
        PART_Popup = e.NameScope.Get<Popup>(nameof(PART_Popup));
        PART_EditableTextBox = e.NameScope.Get<TextBox>(nameof(PART_EditableTextBox));
        PART_DropDownOverlay = e.NameScope.Get<Border>(nameof(PART_DropDownOverlay));
        PART_ItemsPresenter = e.NameScope.Get<ItemsPresenter>(nameof(PART_ItemsPresenter));

        PART_EditableTextBox.LostFocus += PART_EditableTextBox_LostFocus;

        PART_DropDownOverlay.PointerPressed += OnDropDownOverlayPointerPressed;
        PART_DropDownOverlay.PointerReleased += OnDropDownOverlayPointerReleased;
        PART_DropDownOverlay.PointerCaptureLost += OnDropDownOverlayPointerCaptureLost;

        PART_Popup.GotFocus += PART_PopupOnGotFocus;

        // Do update the text and selection
        UpdateDisplaySelectedItems();
        UpdateEditableText(true);
    }

    private void PART_PopupOnGotFocus(object? sender, GotFocusEventArgs e)
    {
        PART_ItemsPresenter?.Focus();
    }

    /// <summary>
    ///     An event reporting a key was pressed
    /// </summary>
    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        KeyDownHandler(e);
    }

    private void KeyDownHandler(KeyEventArgs e)
    {
        var handled = false;
        var key = e.Key;

        // We want to handle Alt key. Get the real key if it is Key.System.
        if (key == Key.System)
        {
            key = e.KeyModifiers.HasFlag(KeyModifiers.Alt) ? Key.LeftAlt : Key.None;
        }

        switch (key)
        {
            case Key.Up:
                handled = true;
                if ((e.KeyModifiers & KeyModifiers.Alt) == KeyModifiers.Alt)
                {
                    IsDropDownOpen = !IsDropDownOpen;
                }

                // todo  Move focus to dropdown
                // else
                // {
                //     // When the drop down isn't open then focus is on the ComboBox
                //     // and we can't use KeyboardNavigation.
                //     if (this.IsDropDownOpen)
                //     {
                //         this.MoveFocusToDropDown();
                //     }
                //     else if (!this.IsDropDownOpen && this.InterceptKeyboardSelection &&
                //              this.SelectionMode == SelectionMode.Single)
                //     {
                //         this.SelectPrev();
                //     }
                // }

                break;

            case Key.Down:
                handled = true;
                if ((e.KeyModifiers & KeyModifiers.Alt) == KeyModifiers.Alt)
                {
                    IsDropDownOpen = !IsDropDownOpen;
                }
                // todo
                // else
                // {
                //     // When the drop down isn't open then focus is on the ComboBox
                //     // and we can't use KeyboardNavigation.
                //     if (this.IsDropDownOpen)
                //     {
                //         this.MoveFocusToDropDown();
                //     }
                //     else if (!this.IsDropDownOpen && this.InterceptKeyboardSelection &&
                //              this.SelectionMode == SelectionMode.Single)
                //     {
                //         this.SelectNext();
                //     }
                // }

                break;

            case Key.F4:
                if ((e.KeyModifiers & KeyModifiers.Alt) == KeyModifiers.Alt)
                {
                    IsDropDownOpen = !IsDropDownOpen;
                    handled = true;
                }

                break;

            default:
                handled = false;
                break;
        }

        if (handled)
        {
            e.Handled = true;
        }
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        if (e is { Handled: false, Source: Visual src })
        {
            if (PART_Popup?.IsInsidePopup(src) == true)
            {
                base.OnPointerReleased(e);
                return;
            }

            if (!IsEditable)
            {
                // Only open the dropdown here if we're not editable
                // Editable CB requires clicking on the DropDownOverlay
                bool open = IsDropDownOpen;
                IsDropDownOpen = !open;
            }

            e.Handled = true;
        }

        PseudoClasses.Set(s_pcPressed, false);
        base.OnPointerReleased(e);
    }

#if NET6_0_OR_GREATER
    [RequiresUnreferencedCode("Use 'MethodFriendlyToTrimming' instead")]
#endif
    private void PART_EditableTextBox_LostFocus(object? sender, RoutedEventArgs e)
    {
        SelectItemsFromText(0);
    }

    private void OnDropDownOverlayPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (PART_DropDownOverlay is not null && e.GetCurrentPoint(PART_DropDownOverlay).Properties.IsLeftButtonPressed)
        {
            ((IPseudoClasses)PART_DropDownOverlay.Classes).Set(s_pcPressed, true);
            e.Handled = true;
        }
    }

    private void OnDropDownOverlayPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (PART_DropDownOverlay is not null && e.GetCurrentPoint(PART_DropDownOverlay).Properties.PointerUpdateKind ==
            PointerUpdateKind.LeftButtonReleased)
        {
            ((IPseudoClasses)PART_DropDownOverlay.Classes).Set(s_pcPressed, false);
            IsDropDownOpen = !IsDropDownOpen;
            e.Handled = true;
        }
    }

    private void OnDropDownOverlayPointerCaptureLost(object? sender, PointerCaptureLostEventArgs e)
    {
        if (PART_DropDownOverlay != null)
        {
            ((IPseudoClasses)PART_DropDownOverlay.Classes).Set(s_pcPressed, false);
        }
    }

#if NET6_0_OR_GREATER
    [UnconditionalSuppressMessage("Trimming",
        "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code",
        Justification
            = "<Pending>")]
#endif
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (e.Property == TextProperty)
        {
            UpdateHasCustomText(null);
            _isUserDefinedTextInputPending = true;

            // Select the items during typing if enabled
            if (SelectItemsFromTextInputDelay >= 0)
            {
                SelectItemsFromText(SelectItemsFromTextInputDelay);
            }
        }

        if (e.Property == OrderSelectedItemsByProperty)
        {
            UpdateDisplaySelectedItems();
            UpdateEditableText();
        }
    }

    protected override void OnGotFocus(GotFocusEventArgs e)
    {
        base.OnGotFocus(e);
        if (IsEditable && PART_EditableTextBox != null)
        {
            if (!IsDropDownOpen)
            {
                PART_EditableTextBox.Focus(e.NavigationMethod);
                PART_EditableTextBox.SelectAll();
            }
            else
            {
                if (!PART_EditableTextBox.IsFocused)
                {
                    // If focus moves to the dropdown, keep the textbox style looking
                    // like its focused
                    ((IPseudoClasses)PART_EditableTextBox.Classes).Set(":focus", true);
                }
            }
        }
    }

    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        return new MultiSelectionComboBoxItem();
    }

    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
    {
        return NeedsContainer<MultiSelectionComboBoxItem>(item, out recycleKey);
    }


    /// <summary>Identifies the <see cref="AddingItem"/> routed event.</summary>
    public static readonly RoutedEvent AddingItemEvent = RoutedEvent.Register<AddingItemEventArgs>(
        nameof(AddingItem), RoutingStrategies.Bubble, typeof(MultiSelectionComboBox));

    /// <summary>
    ///     Occurs before a new object is added to the Items-List
    /// </summary>
    public event AddingItemEventArgsHandler AddingItem
    {
        add => AddHandler(AddingItemEvent, value);
        remove => RemoveHandler(AddingItemEvent, value);
    }

    /// <summary>Identifies the <see cref="AddedItem"/> routed event.</summary>
    public static readonly RoutedEvent AddedItemEvent = RoutedEvent.Register<AddedItemEventArgs>(
        nameof(AddedItem), RoutingStrategies.Bubble, typeof(MultiSelectionComboBox));

    /// <summary>
    ///     Occurs before a new object is added to the Items-List
    /// </summary>
    public event AddedItemEventArgsHandler AddedItem
    {
        add => AddHandler(AddedItemEvent, value);
        remove => RemoveHandler(AddedItemEvent, value);
    }
}
