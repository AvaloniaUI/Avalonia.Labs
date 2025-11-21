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
                demoSwipe.Close();
            }
        }

        private void OpenLeft(object? sender, RoutedEventArgs e)
        {
            var demoSwipe = this.FindControl<Swipe>("DemoSwipe");
            demoSwipe?.Open(OpenSwipeItem.LeftItems);
        }

        private void OpenRight(object? sender, RoutedEventArgs e)
        {
            var demoSwipe = this.FindControl<Swipe>("DemoSwipe");
            demoSwipe?.Open(OpenSwipeItem.RightItems);
        }

        private void OpenTop(object? sender, RoutedEventArgs e)
        {
            var demoSwipe = this.FindControl<Swipe>("DemoSwipe");
            demoSwipe?.Open(OpenSwipeItem.TopItems);
        }

        private void OpenBottom(object? sender, RoutedEventArgs e)
        {
            var demoSwipe = this.FindControl<Swipe>("DemoSwipe");
            demoSwipe?.Open(OpenSwipeItem.BottomItems);
        }
    }
}
