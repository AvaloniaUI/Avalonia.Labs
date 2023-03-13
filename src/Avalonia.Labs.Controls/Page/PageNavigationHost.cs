using System;
using Avalonia.Controls;
using Avalonia.Controls.Platform;
using Avalonia.Controls.Templates;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Reactive;

namespace Avalonia.Labs.Controls
{
    public partial class PageNavigationHost : Control
    {
        public static readonly StyledProperty<object?> PageProperty =
            AvaloniaProperty.Register<PageNavigationHost, object?>(nameof(Page));

        internal static readonly StyledProperty<Page?> ContentProperty =
            AvaloniaProperty.Register<PageNavigationHost, Page?>(nameof(Content));
        private TopLevel? _topLevel;
        private IInsetsManager? _insetManager;
        private IDisposable? _systemThemeSubscription;

        public object? Page
        {
            get => GetValue(PageProperty);
            set { SetValue(PageProperty, value); }
        }

        internal Page? Content
        {
            get { return GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
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
        }

        private void TopLevel_BackRequested(object? sender, RoutedEventArgs e)
        {
            if (Content != null)
            {
                var forwaredEvent = new RoutedEventArgs(Controls.Page.PageNavigationSystemBackButtonPressedEvent);
                Content.RaiseEvent(forwaredEvent);

                e.Handled = forwaredEvent.Handled;
            }
        }

        private void InsetManager_SafeAreaChanged(object? sender, SafeAreaChangedArgs e)
        {
            if (Content != null)
            {
                Content.SafeAreaPadding = e.SafeAreaPadding;
            }
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if(change.Property == PageProperty)
            {
                CreatePageControl();

                if (Content != null && _insetManager != null)
                {
                    Content.SafeAreaPadding = _insetManager.SafeAreaPadding;
                }

                if (change.OldValue is ILogical oldLogical)
                {
                    LogicalChildren.Remove(oldLogical);
                }
                if (change.NewValue is ILogical newLogical)
                {
                    LogicalChildren.Add(newLogical);
                }
            }

            if (change.Property == ContentProperty)
            {
                VisualChildren?.Clear();

                _systemThemeSubscription?.Dispose();

                if (Content != null)
                {
                    VisualChildren?.Add(Content);
                }
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            Content?.Measure(availableSize);

            return Content?.DesiredSize ?? base.MeasureOverride(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var size = base.ArrangeOverride(finalSize);

            Content?.Arrange(Bounds);

            return size;
        }

        private void CreatePageControl()
        {
            if(Page != null)
            {
                if(Page is Control control)
                {
                    if(Page is Page page)
                    {
                        Content = page;
                    }
                    else
                    {
                        Content = new ContentPage()
                        {
                            Content = Content
                        };
                    }
                }
                else
                {
                    var dataTemplate = this.FindDataTemplate(typeof(Page)) ?? FuncDataTemplate.Default;

                    if (dataTemplate != null)
                    {
                        var content = dataTemplate.Build(Page);

                        if (content is Page page)
                        {
                            Content = page;
                        }
                        else
                        {
                            Content = new ContentPage()
                            {
                                Content = content
                            };
                        }
                    }
                    else
                    {
                        Content = null;
                    }
                }
            }
        }
    }
}
