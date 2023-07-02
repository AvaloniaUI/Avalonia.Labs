using System;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Labs.Controls;

[TemplatePart("PART_ItemsPresenter", typeof(ItemsPresenter))]
[TemplatePart("PART_ProgressBar", typeof(ProgressBar))]
public class StepBar : SelectingItemsControl
{
    private Command? _nextCommand;
    private Command? _backCommand;
    private ProgressBar? _bar;

    public StepBar()
    {
        this.Items.CollectionChanged += Items_CollectionChanged;
    }

    private void Items_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        UpdateProgressBar();
    }

    private void UpdateProgressBar()
    {
        var colCount = Items.Count;
        if (_bar == null || colCount <= 0)
            return;
        _bar.Maximum = colCount - 1;
        _bar.Value = SelectedIndex;

        if (Dock == Dock.Top || Dock == Dock.Bottom)
        {
            _bar.Width = (colCount - 1) * (Bounds.Width / colCount);
        }
        else
        {
            _bar.Height = (colCount - 1) * (Bounds.Height / colCount);
        }
    }


    public static readonly AvaloniaProperty<Dock> DockProperty =
        AvaloniaProperty.Register<StepBar, Dock>(nameof(Dock)
            , Dock.Top);

    public Dock Dock
    {
        get => this.GetValue<Dock>(DockProperty);
        set => SetValue(DockProperty, value);
    }

    public static readonly StyledProperty<bool> IsMouseSelectableProperty =
        AvaloniaProperty.Register<StepBar, bool>(nameof(IsMouseSelectable), false);

    public bool IsMouseSelectable
    {
        get => GetValue(IsMouseSelectableProperty);
        set => SetValue(IsMouseSelectableProperty, value);
    }

    public static readonly DirectProperty<StepBar, ICommand> NextCommandProperty =
        AvaloniaProperty.RegisterDirect<StepBar, ICommand>(nameof(NextCommand),
            o => o.NextCommand);

    public ICommand NextCommand
    {
        get => _nextCommand ??=
            new Command(GoNext, CanGoNext);
    }

    public static readonly DirectProperty<StepBar, ICommand> BackCommandProperty =
        AvaloniaProperty.RegisterDirect<StepBar, ICommand>(nameof(BackCommand),
            o => o.BackCommand);

    public ICommand BackCommand
    {
        get => _backCommand ??=
            new Command(GoBack, CanGoBack);
    }

    bool CanGoBack(object? parameter)
    {
        if (Items?.Count is int count)
            return count > 0 && (SelectedIndex - 1) >= 0;
        return false;
    }

    void GoBack(object? parameter)
    {
        SelectedIndex--;
    }

    bool CanGoNext(object? parameter) =>
     Items?.Count is int count && count > 0 && SelectedIndex + 1 < count;

    void GoNext(object? parameter)
    {
        SelectedIndex++;
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        UpdateProgressBar();
        if (Items.Count > 0 && SelectedIndex == -1)
        {
            SetCurrentValue(SelectedIndexProperty, 0);
        }
    }

    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
    {
        return NeedsContainer<StepBarItem>(item, out recycleKey);
    }

    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        return new StepBarItem();
    }

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);
    }

    protected override void ContainerForItemPreparedOverride(Control container, object? item, int index)
    {
        base.ContainerForItemPreparedOverride(container, item, index);
        if (container is StepBarItem stepBarItem)
        {
            stepBarItem.Index = index + 1;
        }
    }
    protected override void ContainerIndexChangedOverride(Control container, int oldIndex, int newIndex)
    {
        base.ContainerIndexChangedOverride(container, oldIndex, newIndex);
        if (container is StepBarItem item)
        {
            item.Index = newIndex + 1;
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _bar = e.NameScope.Find<ProgressBar>("PART_ProgressBar");
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == SelectedIndexProperty)
        {
            if (change.NewValue is int selectIndex)
            {
                var nItems = Items.Count;
                int stepIndex = 0;
                for (; stepIndex < selectIndex; stepIndex++)
                {
                    if (ContainerFromIndex(stepIndex) is StepBarItem item)
                    {
                        item.Status = StepStatus.Complete;
                    }
                }
                if (ContainerFromIndex(stepIndex++) is StepBarItem stepItemSelected)
                    stepItemSelected.Status = StepStatus.UnderWay;
                for (; stepIndex < nItems; stepIndex++)
                {
                    if (ContainerFromIndex(stepIndex) is StepBarItem item)
                    {
                        item.Status = StepStatus.Waiting;
                    }
                }
                if (_bar is ProgressBar progressBar)
                {
                    var current = progressBar.Value;
                    progressBar.BeginAnimation(ProgressBar.ValueProperty
                        , TimeSpan.FromMilliseconds(200)
                        , (double)selectIndex);
                }
            }
            _backCommand?.RaiseCanExecuteChanged();
            _nextCommand?.RaiseCanExecuteChanged();
        }
        else if (change.Property == BoundsProperty)
        {
            UpdateProgressBar();
        }
    }

    internal bool UpdateSelectionFromPointerEvent(Control source, PointerEventArgs e)
    {
        var hotkeys = Application.Current!.PlatformSettings?.HotkeyConfiguration;
        var toggle = hotkeys is not null && e.KeyModifiers.HasFlag(hotkeys.CommandModifiers);

        return UpdateSelectionFromEventSource(
            source,
            true,
            e.KeyModifiers.HasFlag(KeyModifiers.Shift),
            toggle,
            e.GetCurrentPoint(source).Properties.IsRightButtonPressed);
    }

}
