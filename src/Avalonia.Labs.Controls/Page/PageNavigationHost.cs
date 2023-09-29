using System;
using Avalonia.Controls;
using Avalonia.Controls.Platform;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Styling;

namespace Avalonia.Labs.Controls
{
    public partial class PageNavigationHost : ContentControl, IStyleable
    {
        public static readonly StyledProperty<object?> PageProperty =
            AvaloniaProperty.Register<PageNavigationHost, object?>(nameof(Page));

        private TopLevel? _topLevel;
        private IInsetsManager? _insetManager;
        private ContentPresenter? _contentPresenter;

        Type IStyleable.StyleKey => typeof(ContentControl);

        public object? Page
        {
            get => GetValue(PageProperty);
            set { SetValue(PageProperty, value); }
        }

        static PageNavigationHost()
        {
            ContentTemplateProperty.OverrideDefaultValue<PageNavigationHost>(new DefaultPageDataTemplate());
        }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);

            if (_insetManager != null)
            {
                _insetManager.SafeAreaChanged -= InsetManager_SafeAreaChanged;
            }

            if(_topLevel != null)
            {
                _topLevel.BackRequested -= TopLevel_BackRequested;
            }

            _topLevel = TopLevel.GetTopLevel(this);
            _insetManager = _topLevel?.InsetsManager;

            if (_insetManager != null)
            {
                _insetManager.SafeAreaChanged += InsetManager_SafeAreaChanged;

                // TODO: remove this as it's debug code
                _insetManager.DisplayEdgeToEdge = true;
            }

            if (_topLevel != null)
            {
                _topLevel.BackRequested += TopLevel_BackRequested;
            }

            if (_insetManager != null && _contentPresenter != null && _contentPresenter.Child is Page page)
            {
                page.SafeAreaPadding = _insetManager.SafeAreaPadding;
            }
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            if (_contentPresenter != null)
            {
                _contentPresenter.PropertyChanged -= ContentPresenter_PropertyChanged;
            }

            _contentPresenter = e.NameScope.Get<ContentPresenter>("PART_ContentPresenter");

            if (_contentPresenter != null)
            {
                if (_insetManager != null && _contentPresenter.Child is Page page)
                {
                    page.SafeAreaPadding = _insetManager.SafeAreaPadding;
                }

                _contentPresenter.PropertyChanged += ContentPresenter_PropertyChanged;
            }
        }

        private void ContentPresenter_PropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.Property == ContentPresenter.ChildProperty && Presenter != null && Presenter.Child is Page page && _insetManager != null)
            {
                page.SafeAreaPadding = _insetManager.SafeAreaPadding;
            }
        }

        private void TopLevel_BackRequested(object? sender, RoutedEventArgs e)
        {
            if (Presenter != null && Presenter.Child is Page page)
            {
                var forwaredEvent = new RoutedEventArgs(Controls.Page.PageNavigationSystemBackButtonPressedEvent);
                page.RaiseEvent(forwaredEvent);

                e.Handled = forwaredEvent.Handled;
            }
        }

        private void InsetManager_SafeAreaChanged(object? sender, SafeAreaChangedArgs e)
        {
            if (Content != null && Presenter != null && Presenter.Child is Page page)
            {
                page.SafeAreaPadding = e.SafeAreaPadding;
            }
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if(change.Property == PageProperty)
            {
                Content = change.NewValue;

                if (Content != null && _insetManager != null && Presenter != null && Presenter.Child is Page page)
                {
                    page.SafeAreaPadding = _insetManager.SafeAreaPadding;
                }
            }
        }
    }
}
