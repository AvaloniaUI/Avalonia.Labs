using System;
using Avalonia.Controls.Platform;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;

namespace Avalonia.Labs.Controls
{
    public abstract class Page : TemplatedControl
    {
        public static readonly StyledProperty<Thickness> SafeAreaPaddingProperty =
            AvaloniaProperty.Register<Page, Thickness>(nameof(SafeAreaPadding));

        public static readonly StyledProperty<string?> TitleProperty =
            AvaloniaProperty.Register<Page, string?>(nameof(Title));

        public static readonly StyledProperty<Page?> ActiveChildPageProperty =
            AvaloniaProperty.Register<Page, Page?>(nameof(ActiveChildPage));

        public static readonly RoutedEvent<RoutedEventArgs> PageNavigationSystemBackButtonPressedEvent =
            RoutedEvent.Register<Page, RoutedEventArgs>(nameof(PageNavigationSystemBackButtonPressed), RoutingStrategies.Bubble | RoutingStrategies.Tunnel);

        static Page()
        {
            PageNavigationSystemBackButtonPressedEvent.AddClassHandler<Page>((page, args) =>
            {
                page.ActiveChildPage?.RaiseEvent(args);
            });
        }

        public string? Title
        {
            get => GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public Thickness SafeAreaPadding
        {
            get => GetValue(SafeAreaPaddingProperty);
            set => SetValue(SafeAreaPaddingProperty, value);
        }

        public Page? ActiveChildPage
        {
            get => GetValue(ActiveChildPageProperty);
            set { SetValue(ActiveChildPageProperty, value); }
        }

        public event EventHandler<RoutedEventArgs> PageNavigationSystemBackButtonPressed
        {
            add => AddHandler(PageNavigationSystemBackButtonPressedEvent, value);
            remove => RemoveHandler(PageNavigationSystemBackButtonPressedEvent, value);
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (ActiveChildPage != null)
            {
                if (change.Property == SafeAreaPaddingProperty
                    || change.Property == SafeAreaPaddingProperty
                    || change.Property == PaddingProperty
                    || change.Property == ActiveChildPageProperty)
                {
                    ActiveChildPage.SafeAreaPadding = Padding.GetRemainingSafeAreaPadding(SafeAreaPadding);
                }
            }

            if(change.Property == ActiveChildPageProperty)
            {
                if(change.OldValue is ILogical oldLogical)
                {
                    LogicalChildren.Remove(oldLogical);
                }
                if(change.NewValue is ILogical newLogical)
                {
                    LogicalChildren.Add(newLogical);
                }
            }
        }
    }
}
