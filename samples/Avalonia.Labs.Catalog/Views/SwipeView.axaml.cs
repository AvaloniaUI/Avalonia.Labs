using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Labs.Controls;
using Avalonia.Labs.Controls.Base;

namespace Avalonia.Labs.Catalog.Views
{
    public partial class SwipeView : UserControl
    {
        public SwipeView()
        {
            InitializeComponent();
        }

        private void TapGestureRecognizer_OnOnTap(object? sender, TapEventArgs e)
        {
            if (sender is StackPanel panel)
            {
                var label = panel.Children.OfType<Label>().First();
                label.Content = "Clicked";
            }
        }

        private void CloseSwipe(object? sender, RoutedEventArgs e)
        {
            var demoSwipe = this.FindControl<Swipe>("DemoSwipe");
            if (demoSwipe != null)
            {
                demoSwipe.SwipeState = SwipeState.Hidden;
            }
        }

        private void OpenLeft(object? sender, RoutedEventArgs e)
        {
            var demoSwipe = this.FindControl<Swipe>("DemoSwipe");
            if (demoSwipe != null)
            {
                demoSwipe.SwipeState = SwipeState.LeftVisible;
            }
        }

        private void OpenRight(object? sender, RoutedEventArgs e)
        {
            var demoSwipe = this.FindControl<Swipe>("DemoSwipe");
            if (demoSwipe != null)
            {
                demoSwipe.SwipeState = SwipeState.RightVisible;
            }
        }

        private void OpenTop(object? sender, RoutedEventArgs e)
        {
            var demoSwipe = this.FindControl<Swipe>("DemoSwipe");
            if (demoSwipe != null)
            {
                demoSwipe.SwipeState = SwipeState.TopVisible;
            }
        }

        private void OpenBottom(object? sender, RoutedEventArgs e)
        {
            var demoSwipe = this.FindControl<Swipe>("DemoSwipe");
            if (demoSwipe != null)
            {
                demoSwipe.SwipeState = SwipeState.BottomVisible;
            }
        }

        private void DemoSwipe_OpenRequested(object? sender, OpenRequestedEventArgs e)
        {
            var eventLog = this.FindControl<TextBlock>("EventLog");
            if (eventLog != null)
            {
                eventLog.Text = $"OpenRequested: {e.OpenSwipeItem}";
            }
        }

        private void DemoSwipe_CloseRequested(object? sender, CloseRequestedEventArgs e)
        {
            var eventLog = this.FindControl<TextBlock>("EventLog");
            if (eventLog != null)
            {
                eventLog.Text = "CloseRequested";
            }
        }
    }
}
