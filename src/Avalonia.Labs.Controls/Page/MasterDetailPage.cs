using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Metadata;

namespace Avalonia.Labs.Controls
{
    [TemplatePart("PART_MasterPresenter", typeof(ContentPresenter))]
    [TemplatePart("PART_DetailPresenter", typeof(ContentPresenter))]
    public class MasterDetailPage : Page
    {
        public static readonly StyledProperty<object?> MasterProperty =
           AvaloniaProperty.Register<MasterDetailPage, object?>(nameof(Master));

        public static readonly StyledProperty<IDataTemplate?> MasterTemplateProperty =
           AvaloniaProperty.Register<MasterDetailPage, IDataTemplate?>(nameof(MasterTemplate), new DefaultPageDataTemplate());

        public static readonly StyledProperty<IDataTemplate?> DetailTemplateProperty =
           AvaloniaProperty.Register<MasterDetailPage, IDataTemplate?>(nameof(DetailTemplate), new DefaultPageDataTemplate());

        public static readonly StyledProperty<object?> DetailProperty =
           AvaloniaProperty.Register<MasterDetailPage, object?>(nameof(Detail));

        public static readonly StyledProperty<bool> IsPresentedProperty =
           AvaloniaProperty.Register<MasterDetailPage, bool>(nameof(IsPresented));

        public static readonly StyledProperty<HorizontalAlignment> HorizontalContentAlignmentProperty =
            ContentControl.HorizontalContentAlignmentProperty.AddOwner<ContentPage>();

        public static readonly StyledProperty<VerticalAlignment> VerticalContentAlignmentProperty =
            ContentControl.VerticalContentAlignmentProperty.AddOwner<ContentPage>();

        public static readonly StyledProperty<SplitViewDisplayMode> DisplayModeProperty =
            SplitView.DisplayModeProperty.AddOwner<MasterDetailPage>();

        private ContentPresenter? _detailPresenter;
        private ContentPresenter? _masterPresenter;
        private Border? _topBar;

        public HorizontalAlignment HorizontalContentAlignment
        {
            get { return GetValue(HorizontalContentAlignmentProperty); }
            set { SetValue(HorizontalContentAlignmentProperty, value); }
        }

        public VerticalAlignment VerticalContentAlignment
        {
            get { return GetValue(VerticalContentAlignmentProperty); }
            set { SetValue(VerticalContentAlignmentProperty, value); }
        }

        [Content]
        [DependsOn(nameof(MasterTemplate))]
        public object? Master
        {
            get => GetValue(MasterProperty);
            set => SetValue(MasterProperty, value);
        }

        [DependsOn(nameof(DetailTemplate))]
        public object? Detail
        {
            get => GetValue(DetailProperty);
            set => SetValue(DetailProperty, value);
        }

        public IDataTemplate? MasterTemplate
        {
            get => GetValue(MasterTemplateProperty);
            set => SetValue(MasterTemplateProperty, value);
        }

        public IDataTemplate? DetailTemplate
        {
            get => GetValue(DetailTemplateProperty);
            set => SetValue(DetailTemplateProperty, value);
        }

        public bool IsPresented
        {
            get => GetValue(IsPresentedProperty);
            set => SetValue(IsPresentedProperty, value);
        }
        public SplitViewDisplayMode DisplayMode
        {
            get => GetValue(DisplayModeProperty);
            set => SetValue(DisplayModeProperty, value);
        }

        static MasterDetailPage()
        {
            PageNavigationSystemBackButtonPressedEvent.AddClassHandler<MasterDetailPage>((sender, eventArgs) =>
            {
                if (sender.IsPresented)
                {
                    sender.IsPresented = false;

                    eventArgs.Handled = true;
                }
            });
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            _detailPresenter = e.NameScope.Get<ContentPresenter>("PART_DetailPresenter");
            _masterPresenter = e.NameScope.Get<ContentPresenter>("PART_MasterPresenter");
            _topBar = e.NameScope.Get<Border>("PART_TopBar");

            UpdateContentSafeAreaPadding();
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == MasterProperty || change.Property == DetailProperty)
            {
                if (change.OldValue is ILogical oldLogical)
                {
                    LogicalChildren.Remove(oldLogical);
                }
                if (change.NewValue is ILogical newLogical)
                {
                    LogicalChildren.Add(newLogical);
                }
                UpdateActivePage();
            }
            if(change.Property == IsPresentedProperty || change.Property == DisplayModeProperty)
            {
                UpdateActivePage();
            }
        }

        protected override void UpdateContentSafeAreaPadding()
        {
            if (_detailPresenter != null && _masterPresenter != null)
            {
                _masterPresenter.Padding = new Thickness(
                    SafeAreaPadding.Left,
                    SafeAreaPadding.Top,
                    0,
                    SafeAreaPadding.Bottom);

                if (_topBar != null)
                {
                    var navPadding = SafeAreaPadding;
                    _topBar.Padding = new Thickness(navPadding.Left, navPadding.Top, navPadding.Right, 0);
                }

                _detailPresenter.Padding = Padding;

                if (_detailPresenter.Child is Page detail)
                {
                    var remainingSafeArea = Padding.GetRemainingSafeAreaPadding(SafeAreaPadding);
                    detail.SafeAreaPadding = new Thickness(remainingSafeArea.Left, 0, remainingSafeArea.Right, remainingSafeArea.Bottom);
                }
            }
        }

        protected override void UpdateActivePage()
        {
            if (_masterPresenter != null && _detailPresenter != null)
            {
                if (IsPresented && (DisplayMode == SplitViewDisplayMode.Overlay || DisplayMode == SplitViewDisplayMode.CompactOverlay))
                    ActiveChildPage = _masterPresenter.Child as Page;
                else
                {
                    ActiveChildPage = _detailPresenter.Child as Page;
                }
            }
        }
    }
}
